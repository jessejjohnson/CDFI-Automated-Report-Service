using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using CDFIService.Helpers;
using CDFIService.Connectors;

namespace CDFIService
{
    /// <summary>
    /// Implements Main Application Functionality
    /// </summary>
    public partial class CDFIService : ServiceBase
    {
        Thread Worker;
        AutoResetEvent StopRequest = new AutoResetEvent(false);

        /// <summary>
        /// Constructor
        /// </summary>
        public CDFIService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Main Entry Point
        /// </summary>
        public void OnDebug()
        {
            OnStart(null);
        }

        /// <summary>
        /// Starts Main Application Workflow
        /// </summary>
        protected override void OnStart(string[] args)
        {            
            // Start the worker thread
            Worker = new Thread(Process);
            Worker.Start();
        }

        /// <summary>
        /// Stopps Main Application Workflow
        /// </summary>
        protected override void OnStop()
        {
            // Signal worker to stop and wait until it does
            StopRequest.Set();
            Worker.Join();
        }


        /// <summary>
        /// Main Application Workflow
        /// </summary>
        /// <param name="arg">Any Arguments, not being used in current procedure</param>
        private void Process(object arg)
        {
         
            //private ConnectDataNLS _connectData;

            //for (;;)
            //{
            //if (StopRequest.WaitOne(ConfigData.LoopInterval * 1000)) return;

            List<string> sourceFiles;
            ConnectorFTP connectorFTP = null;
            ConnectorSFTP connectorSFTP = null;
            var connectorFileSystem = new ConnectorFileSystem();
            var retval = string.Empty;

            try
            {
                                
                EventLogMessage.DebugMode = ConfigData.DebugMode;
                EventLogMessage.WriteLog(String.Format("Method Main is started - {0}", DateTime.Now));

                // Establish FTP or SFTP Connection
                if (ConfigData.IsSFTP)
                {
                    connectorSFTP = new ConnectorSFTP();
                }
                else
                {
                    connectorFTP = new ConnectorFTP();
                }            

                // Setup SFTP Connection if necessary
                EventLogMessage.WriteLog("Open FTP Conection...");
                if (ConfigData.IsSFTP)
                {
                    connectorSFTP.SetupConnection();
                }
            
                // Get List Of Source Files
                EventLogMessage.WriteLog("Get Report Files in Source Folder...");
                sourceFiles = connectorFileSystem.GetSourceFolderFiles();

                if (sourceFiles.Count > 0)
                {
                    
                    // Upload Source Files to FTP
                    EventLogMessage.WriteLog("Uploading Files...");
                    foreach (string strFile in sourceFiles)
                    {
                        EventLogMessage.WriteLog("Uploading File " + strFile + "...");
                        if (ConfigData.IsSFTP)
                        {
                            connectorSFTP.UploadExistingFile(strFile);
                        }
                        else
                        {
                            connectorFTP.UploadFile(strFile);
                        }
                    }

                    // Delete Source Files
                    EventLogMessage.WriteLog("Deleting Source Files...");
                    connectorFileSystem.DeleteSourceFolderFiles();                    
                }
                else
                {
                    EventLogMessage.WriteLog("No Report Files Found...");
                }
                
                // Close SFTP Connection if necessary
                EventLogMessage.WriteLog("Close FTP Connection...");
                if (ConfigData.IsSFTP)
                {
                    connectorSFTP.Close();
                }

                // Send Status Email
                EventLogMessage.WriteLog("Send Email...");
                MailHelper.Send_Email_Using_Template(sourceFiles);

                EventLogMessage.WriteLog(String.Format("Method Main is finished - {0}", DateTime.Now));
            }
            catch (System.TimeoutException tex)
            {
                // Abotr Active SFTP Connetion
                if (connectorSFTP != null)
                {
                    connectorSFTP.Abort();
                }                
                EventLogMessage.WriteLogError("Method Main was thrown Timeout exception.", tex);
            }
            catch (CommunicationException cex)
            {
                // Abotr Active SFTP Connetion
                if (connectorSFTP != null)
                {
                    connectorSFTP.Abort();
                }
                EventLogMessage.WriteLogError("Method Main was thrown Communication exception.", cex);
            }
            catch (Exception ex)
            {
                // Abotr Active SFTP Connetion
                if (connectorSFTP != null)
                {
                    connectorSFTP.Abort();
                }
                EventLogMessage.WriteLogError("Method Main was thrown exception.", ex);
            }
        }
        //}
    }
}

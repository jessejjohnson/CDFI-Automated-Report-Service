/// -----------------------------------------------------------------------
/// Project   : CDFIService
/// Namespace : CDFIService.Connectors
/// Class     : ConnectorSFTP, ConnectorNLS
/// 
/// -----------------------------------------------------------------------
/// <summary>
/// Implements classes and properties for connection to NLS and SFTP
/// </summary>
/// <history>
/// Art. N        September 20, 2018     Created
/// </history>
/// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Net;
using CDFIService.Helpers;
using System.IO;
using Newtonsoft.Json;
using Renci.SshNet;
using System.Linq;
using Microsoft.VisualBasic;

namespace CDFIService.Connectors
{

    /// <summary>
    /// Implements FTP functionality
    /// </summary>
    public class ConnectorFTP
    {
        private ConnectDataFTP _connectData;

        /// <summary>
        /// The method UploadFile implements file upload functionality
        /// </summary>
        /// <param name="fileName">Local file Path to upload</param>
        public void UploadFile(string fileName)
        {
            Helpers.EventLogMessage.WriteLogDebug("ConnectorFTP: UploadFile method");
            EventLogMessage.WriteLog("Upload File " + fileName + "...");
            _connectData = ConfigData.ReadConfigFTP();            
            string absoluteFileName = Path.GetFileName(fileName);
            string targetURL = (!string.IsNullOrEmpty(_connectData.Folder))
                ? string.Format(@"ftp://{0}/{1}/{2}", _connectData.URL, _connectData.Folder, absoluteFileName)
                : string.Format(@"ftp://{0}/{1}", _connectData.URL, absoluteFileName);        
            WebRequest request = WebRequest.Create(new Uri(targetURL)) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_connectData.UserName, _connectData.Password);
            using (FileStream fs = File.OpenRead(fileName))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Flush();
                requestStream.Close();
            }
        }
    }

    /// <summary>
    /// Implements SFTP functionality
    /// </summary>
    public class ConnectorSFTP
    {
        
        private ConnectDataSFTP _connectData;
        SftpClient client;

        /// <summary>
        /// Close SFTP Connection
        /// </summary>        
        public void Close()
        {
            if (client.IsConnected)
            {
                client.Disconnect();
            }
        }

        /// <summary>
        /// Abort SFTP Connection
        /// </summary>
        public void Abort()
        {
            if (client.IsConnected)
            {
                client.Disconnect();
            }
        }

        /// <summary>
        /// Setup SFTP Connection based on Congfiguration File Parameters
        /// </summary>
        public void SetupConnection()
        {
            EventLogMessage.WriteLogDebug("ConnectorSFTP: SetupConnection method");

            // Read connection configuration
            _connectData = ConfigData.ReadConfigSFTP();

            client = new SftpClient(_connectData.Host, _connectData.Port, _connectData.Username, _connectData.Password);
            client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(_connectData.Timeout);
            client.Connect();

            EventLogMessage.WriteLog("Connected to " + _connectData.Host);

            if (_connectData.WorkingDirectory != "")
            {
                client.ChangeDirectory(_connectData.WorkingDirectory);
            }

        }

        /// <summary>
        /// Upload Existing File to SFTP
        /// </summary>
        /// <param name="fileName">Local file Path to upload</param>
        public void UploadExistingFile(string fileName)
        {
            EventLogMessage.WriteLogDebug("ConnectorSFTP: UploadExistingFile method");
            System.IO.FileStream fileStream = File.Open(fileName, FileMode.Open);
            client.UploadFile(fileStream, fileName);
            fileStream.Close();
        }
        
    }

    /// <summary>
    /// Implements File System Functuionality
    /// </summary>
    public class ConnectorFileSystem
    {

        private ConnectDataFileSystem _connectData;

        /// <summary>
        /// Get List of Source Folder Files
        /// </summary>
        public List<string> GetSourceFolderFiles()
        {
            EventLogMessage.WriteLogDebug("ConnectorFilesystem: GetSourceFolderFiles method");
            // Read connection configuration
            _connectData = ConfigData.ReadConfigFileSystem();
            string folderPath = (_connectData.SourceFolder.Last() == "\\"[0]) ? _connectData.SourceFolder : _connectData.SourceFolder + "\\";
            EventLogMessage.WriteLogDebug("ConnectorFilesystem: GetSourceFolderFiles method, Source Folder Path: "  + folderPath);
            return new List<string>(Directory.EnumerateFiles(folderPath));
        }


        /// <summary>
        /// Delete All Source Folder Files
        /// </summary>
        public void DeleteSourceFolderFiles()
        {
            EventLogMessage.WriteLogDebug("ConnectorFilesystem: DeleteSourceFolderFiles method");
            // Read connection configuration
            _connectData = ConfigData.ReadConfigFileSystem();
            string folderPath = (_connectData.SourceFolder.Last() == "\\"[0]) ? _connectData.SourceFolder : _connectData.SourceFolder + "\\";
            EventLogMessage.WriteLogDebug("ConnectorFilesystem: DeleteSourceFolderFiles method, Source Folder Path: " + folderPath);
            foreach (string sourceFile in Directory.EnumerateFiles(folderPath))
            {
                EventLogMessage.WriteLog("Delete Source Folder File: " + sourceFile);
                File.Delete(sourceFile);
            }           
        }


    }

}

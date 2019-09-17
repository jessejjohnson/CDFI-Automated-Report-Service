/// -----------------------------------------------------------------------
/// Project   : CDFIService
/// Namespace : CDFIService.Helpers
/// Class     : ConfigData
/// 
/// -----------------------------------------------------------------------
/// <summary>
/// Implements loading and storing application settings.
/// </summary>
/// <history>
/// Art. N        September 20, 2018     Created
/// </history>
/// -----------------------------------------------------------------------
using System;
using System.Configuration;
using CDFIService.Connectors;

namespace CDFIService.Helpers
{
    public static class ConfigData
    {
        /// <summary>
        /// Implements loading global application settings
        /// </summary>
        static ConfigData()
        {
            DebugMode = Convert.ToBoolean(ConfigurationManager.AppSettings["DebugMode"]);
            LoopInterval = Convert.ToInt32(ConfigurationManager.AppSettings["LoopInterval"]);
            IsSFTP = (Convert.ToInt32(ConfigurationManager.AppSettings["IsSFTP"]) == 1);
        }
                
        /// <summary>
        /// Implements loading connection settings for SFTP
        /// </summary>
        /// <returns></returns>
        
        public static ConnectDataSFTP ReadConfigSFTP()
        {
            return new ConnectDataSFTP()
            {
                // Read connection parameters
                Host = ConfigurationManager.AppSettings["SFTPhost"].ToString(),
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SFTPport"]),
                Username = ConfigurationManager.AppSettings["SFTPuser"].ToString(),
                Password = ConfigurationManager.AppSettings["SFTPpassword"].ToString(),
                WorkingDirectory = ConfigurationManager.AppSettings["SFTPdirectory"].ToString(),                
                Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SFTPtimeout"]),                
            };
        }

        public static ConnectDataFileSystem ReadConfigFileSystem()
        {
            return new ConnectDataFileSystem()
            {   
                SourceFolder = ConfigurationManager.AppSettings["SourceFolderPath"].ToString(),                
            };
        }

        public static ConnectDataFTP ReadConfigFTP()
        {
            return new ConnectDataFTP()
            {
                URL = ConfigurationManager.AppSettings["FTPURL"].ToString(),                
                UserName = ConfigurationManager.AppSettings["FTPUser"].ToString(),
                Password = ConfigurationManager.AppSettings["FTPPassword"].ToString(),
                Folder = ConfigurationManager.AppSettings["FTPFolder"].ToString(),
            };
        }

        /// <summary>
        /// Returns status of debug mode
        /// </summary>
        public static bool DebugMode { get; private set; }
        public static Int32 LoopInterval { get; private set; }
        public static bool IsSFTP { get; private set; }
    }
}

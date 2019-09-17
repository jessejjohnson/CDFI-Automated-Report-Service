/// -----------------------------------------------------------------------
/// Project   : CDFIService
/// Namespace : CDFIService.Connectors
/// 
/// -----------------------------------------------------------------------
/// <summary>
/// Implements structures for storing application settings.
/// </summary>
/// <history>
/// Art. N        September 20, 2018     Created
/// </history>
/// -----------------------------------------------------------------------
namespace CDFIService.Connectors
{

    /// <summary>
    /// Implements structure for FTP Connector settings
    /// </summary>
    public class ConnectDataFTP
    {
        public string URL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Folder { get; set; }
    }

    /// <summary>
    /// Implements structure for SFTP Connector settings
    /// </summary>
    public class ConnectDataSFTP
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string WorkingDirectory { get; set; }
        public string FileExtension { get; set; }
        public int Timeout { get; set; }
        public string TimeStamp { get; set; }
    }

    /// <summary>
    /// Implements structure for File System Connector settings
    /// </summary>
    public class ConnectDataFileSystem
    {
        public string SourceFolder { get; set; }        
    }

}

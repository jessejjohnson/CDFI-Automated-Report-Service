/// -----------------------------------------------------------------------
/// Project   : CDFIService
/// Namespace : CDFIService.Helpers
/// Class     : EventLogMessage
/// 
/// -----------------------------------------------------------------------
/// <summary>
/// Implements methods for write messages to a Windows Event Log
/// </summary>
/// <history>
/// Art. N        September 20, 2018     Created
/// </history>
/// -----------------------------------------------------------------------
using System;
using System.Diagnostics;

namespace CDFIService.Helpers
{
    /// <summary>
    /// The EventLogMessage class implements methods for write messages to a Windows Event Log
    /// </summary>
    public static class EventLogMessage
    {
        private static bool _bDebug = false;

        public static bool DebugMode
        {
            set {
                _bDebug = value;
            }
        }

        public static void WriteLogDebug(string message)
        {
            if (!_bDebug) return;
            WriteLog(message);
        }
        /// <summary>
        /// The method WriteLog implements save message in a Windows Event log
        /// </summary>
        /// <param name="message">Text message for write to a Windows Event log</param>
        public static void WriteLog(string message)
        {
            if (!EventLog.SourceExists(Constants.SSource))
                EventLog.CreateEventSource(Constants.SSource, Constants.SLog);
            EventLog.WriteEntry(Constants.SSource, message, System.Diagnostics.EventLogEntryType.Information);
            System.Threading.Thread.Sleep(100);
        }        

        /// <summary>
        /// The method WriteLog implements save message in a Windows Event log with Error log level
        /// </summary>
        /// <param name="message">Text message for write to a Windows Event log</param>
        /// <param name="ex">Exception write to a Windows Event log</param>
        public static void WriteLogError(string message, Exception ex = null)
        {
            if (!EventLog.SourceExists(Constants.SSource))
                EventLog.CreateEventSource(Constants.SSource, Constants.SLog);
            EventLog.WriteEntry(Constants.SSource, message + (ex != null ? "\r\n\r\n" + ex.Message + "\r\n\r\n" + ex.StackTrace : ""), System.Diagnostics.EventLogEntryType.Error);
        }
    }
}
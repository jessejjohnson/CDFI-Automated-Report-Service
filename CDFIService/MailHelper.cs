using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Net.Configuration;
using System.Linq;

namespace CDFIService
{
    /// <summary>
    /// Implements Send Email Functionality
    /// </summary>
    public static class MailHelper
    {
        /// <summary>
        /// Send Email using Email Template saved in Project Resources
        /// </summary>
        /// <param name="vFiles">List of Files Uploaded to FTP</param>
        public static void Send_Email_Using_Template(List<string> vFiles)
        {

            Helpers.EventLogMessage.WriteLogDebug("MailHelper: Send_Email_Using_Template method");

            string strMessage;
            if (vFiles.Count > 0)
            {
                string messageTemplate = Properties.Resources.EmailTemplate1;
                string fileList = "- " + vFiles.Aggregate((i, j) => i + ";" + System.Environment.NewLine + "- " + j) + ";"; 
                strMessage = messageTemplate.Replace("<%FILES%>", fileList);
            }
            else
            {
                strMessage = Properties.Resources.EmailTemplate2;
            }
            Helpers.EventLogMessage.WriteLog(strMessage);
            Send_Mail(strMessage, "CDFIService Status");
        }

        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="strMessage">Message to Send</param>
        /// <param name="subject">Message Subject</param>
        /// <param name="strFromAddress">Sent From</param>
        /// /// <param name="strToAddress">Send To</param>
        public static void Send_Mail(string strMessage, string subject, string strFromAddress = null, string strToAddress = null)
        {

            Helpers.EventLogMessage.WriteLogDebug("MailHelper: Send_Mail method");

            int iEnabled = 0;
            int.TryParse(ConfigurationManager.AppSettings["EmailEnabled"], out iEnabled);
            if(iEnabled == 0)
            {
                return;
            }
            
            var fromAddress = (strFromAddress == null) ? new MailAddress(ConfigurationManager.AppSettings["EmailFrom"]) : new MailAddress(strFromAddress);
            var toAddress = (strToAddress == null) ? new MailAddress(ConfigurationManager.AppSettings["EmailTo"]) : new MailAddress(strToAddress);
            string fromPassword = ConfigurationManager.AppSettings["EmailPassword"];
            string body = strMessage;

            int iPort = 0;
            int.TryParse(ConfigurationManager.AppSettings["EmailSMTPPort"], out iPort);
            var smtp = new SmtpClient
            {
                Host = ConfigurationManager.AppSettings["EmailSMTPHost"],
                Port = iPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}

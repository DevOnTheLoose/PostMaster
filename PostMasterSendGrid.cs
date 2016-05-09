using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailerService
{
    public class PostMasterSendGrid:PostMasterBase
    {
        SendGrid.Web transportWeb;
        public PostMasterSendGrid():base()
        {
            // TODO: Read the configurations from the Settings.xml
            transportWeb = new SendGrid.Web("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");
        }

        /// <summary>
        /// This interface is used for sending the mails by the SendGrid Mail service providers.
        /// </summary>
        /// <param name="guidMailId">Unique identity of the specific email</param>
        /// <param name="strToAddress">Address to whom the email is sent</param>
        /// <param name="strToName">Name of the To email address</param>
        /// <param name="strFromAddress">Address from which email is sent</param>
        /// <param name="strFromName">Friendly name of the From email</param>
        /// <param name="strSubject">Subject of the email</param>
        /// <param name="strBody">Body of the email which is in html</param>
        /// <remarks>
        /// The SendMail api leverages the SendGrid email service. Detailed documentation regarding the usage 
        /// can be found at 
        /// https://github.com/sendgrid/sendgrid-csharp
        /// Note: One limitation in the API is that the current API is not letting one set the Name of the To Address as the To member is readonly which just implements get property
        /// </remarks>
        public override bool SendMail(Guid guidMailId, string strToAddress, string strToName,
                   string strFromAddress, string strFromName,
                   string strSubject, string strBody)
        {
            try
            {
                var myMessage = new SendGrid.SendGridMessage();

                // BUGBUG: TODO: 
                // Api Limitation: The current API is not letting one set the Name of the To Address. 
                // myMessage.To[0] = new MailAddress(strToAddres, strToName);
                // It may be a bit tricky which I will look at later.
                myMessage.AddTo(strToAddress);
                myMessage.From = new MailAddress(strFromAddress, strFromName);
                myMessage.Subject = strSubject;
                myMessage.Text = strBody;

                transportWeb.DeliverAsync(myMessage).Wait();
                return true;
            }
            catch (Exception exp)
            {
                // Log the exception here.
                throw new Exception(String.Format("Exception: SendGrid Failed to Send the email having Id {0} due to following error {1}", 
                        MailId, exp.ToString()));
            }
        }
    }
}

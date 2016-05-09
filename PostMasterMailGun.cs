using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailerService
{
    public class PostMasterMailGun : PostMasterBase
    {
        RestClient restClient;
        RestRequest request;
        public PostMasterMailGun():base()
        {
            // TODO: Read the configurations from the Settings.xml
            restClient = new RestClient();
            restClient.BaseUrl = new Uri("https://api.mailgun.net/v3");
            restClient.Authenticator =
                    new HttpBasicAuthenticator("api",
                                              "key-ab7557abe390ee90b7485cfe6f332386");
        }
        
        public void InitRequest()
        {
            
            request = new RestRequest();
            // Note: Mailgun is strict and is only letting one send the emails from a validated 
            // domain as well as to a validated email address. 
            request.AddParameter("domain",
                                "sandboxf8192860e436428fb7587db684ef176d.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";

        }
        /// <summary>
        /// This API is used for sending the mails by the MailGun Mail Service provider.
        /// </summary>
        /// <param name="guidMailId">Unique identity of the specific email</param>
        /// <param name="strToAddress">Address to whom the email is sent</param>
        /// <param name="strToName">Name of the To email address</param>
        /// <param name="strFromAddress">Address from which email is sent</param>
        /// <param name="strFromName">Friendly name of the From email</param>
        /// <param name="strSubject">Subject of the email</param>
        /// <param name="strBody">Body of the email which is in html</param>
        public override bool SendMail(Guid guidMailId, string strToAddress, string strToName,
                   string strFromAddress, string strFromName,
                   string strSubject, string strBody)
        {
            IRestResponse response = SendMailGun(guidMailId, strToAddress, strToName,
                    strFromAddress, strFromName,
                    strSubject, strBody);
            if (response!=null && response.StatusCode == System.Net.HttpStatusCode.OK)
            { 
                return true; 
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// <summary>
        /// This API is used for sending the mails by the MailGun Mail Service provider.
        /// </summary>
        /// <param name="guidMailId">Unique identity of the specific email</param>
        /// <param name="strToAddress">Address to whom the email is sent</param>
        /// <param name="strToName">Name of the To email address</param>
        /// <param name="strFromAddress">Address from which email is sent</param>
        /// <param name="strFromName">Friendly name of the From email</param>
        /// <param name="strSubject">Subject of the email</param>
        /// <param name="strBody">Body of the email which is in html</param>
        /// <remarks>
        /// NOTE: strToAddress needs to be registered with MailGun before it will allow sending it over with 
        /// atleast the free account.
        /// </remarks>
        public IRestResponse SendMailGun(Guid guidMailId, string strToAddress, string strToName,
                   string strFromAddress, string strFromName,
                   string strSubject, string strBody)
        {
            try
            {
                InitRequest();
                
                request.AddParameter("from", strFromAddress); //"Mailgun Sandbox <postmaster@sandboxf8192860e436428fb7587db684ef176d.mailgun.org>");
                // NOTE: strToAddress needs to be registered with MailGun before it will allow sending it over.
                request.AddParameter("to", strToAddress);
                request.AddParameter("subject", strSubject);
                request.AddParameter("text", strBody);
                request.Method = Method.POST;
                IRestResponse irr = restClient.Execute(request);
                return irr;
            }
            catch (Exception exp)
            {
                Console.Write(String.Format("Exception: SendGrid SendMail operation having Id {0} failed due to following exception:{1}",
                MailId, exp.ToString()));
                return null;
            }
        }
    }
}

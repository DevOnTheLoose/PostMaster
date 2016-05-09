using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MailerService
{
    /// <summary>
    /// All the mail service providers derives from this Send Mail abstract class and implements / overrides the SendMail interface.
    /// </summary>
    public class PostMasterBase
    {
        /// <summary>
        /// Mail Id is the unique id assigned to each mail.
        /// </summary>
        public Guid MailId  { get; set; }

        /// <summary>
        /// Id is the identity of each postmaster. There can be multiple types of postmasters.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// This boolean shows whether the specific postmaster is working or not.
        /// It sets to true initially (without any verification which I believe should be done if its a cold start).
        /// The boolean gets to false if there is a failure. Currently, we are not differentiating between bad input failures
        /// assuming that we are detecting and verifying the bad input. Ideally we should differentiate between failrue due to bad data an good data.
        /// if its a failure due to bad data, the bIsWorking should not be disabled by turning it false.
        /// </summary>
        public bool bIsWorking { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PostMasterBase()
        {
            MailId = new Guid(); 
            Id = new Guid();
            bIsWorking = true;
        }
 
        /// <summary>
        /// This interface is used for sending the mails by all the service providers. 
        /// This API validates the inputs and throws an exception if the input parameters are invalid.
        /// </summary>
        /// <param name="guidMailId">Unique identity of the specific email</param>
        /// <param name="strToAddress">Address to whom the email is sent</param>
        /// <param name="strToName">Name of the To email address</param>
        /// <param name="strFromAddress">Address from which email is sent</param>
        /// <param name="strFromName">Friendly name of the From email</param>
        /// <param name="strSubject">Subject of the email</param>
        /// <param name="strBody">Body of the email which is in html</param>
        /// <remarks>
        /// We can create a base class and drive the service provider classes i.e. MailGun and SendGrid and 
        /// do the base operations like data scrapping of html in the base class</remarks>
        /// <returns>True if the mail send operation was successful otherwise false</returns>
        public virtual bool SendMail(Guid guidMailId, string strToAddress, string strToName,
                   string strFromAddress, string strFromName,
                   string strSubject, string strBody)
        {
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;

namespace MailerService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Mailer" in both code and config file together.
    public class Mailer : IMailer
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Mailer()
        {
        }
        PostMasterBase defaultPostMaster;
        List<PostMasterBase> postMasters;
        Guid guidMailId;
        /// <summary>
        /// Mailer's class SendMail implements the business logic for selecting the default mailer,
        /// retrial logic as well as strategy for failure in case a mail fails using one of the mailers.
        /// This includes but is not limited to selected the low cost mailer, higher throughput mailer for mission
        /// critical mails.
        /// </summary>
        /// <param name="strToAddress">Address to whom the email is sent</param>
        /// <param name="strToName">Name of the To email address</param>
        /// <param name="strFromAddress">Address from which email is sent</param>
        /// <param name="strFromName">Friendly name of the From email</param>
        /// <param name="strSubject">Subject of the email</param>
        /// <param name="strBody">Body of the email which is in html</param>
        public ReturnStatus SendMail(string strToAddress, string strToName,
                             string strFromAddress, string strFromName,
                             string strSubject, string strBody)
        {

            ReturnStatus result = new ReturnStatus();
            try
            {
                // Validate Inputs here.
                ValidateInputs(guidMailId, strToAddress, strToName,
                              strFromAddress, strFromName,
                              strSubject, strBody);
                
                // Scrap HTML from the Body
                strBody = ScrapHTML(strBody);
                Init();

                if (!defaultPostMaster.SendMail(guidMailId, strToAddress, strToName,
                          strFromAddress, strFromName,
                          strSubject, strBody))
                {
                    result.IsSuccessful = false;
                    postMasters.Find(r => r.Id == defaultPostMaster.Id).bIsWorking = false;

                    defaultPostMaster = GetNextAvailablePostMaster();
                    if (defaultPostMaster != null)
                    {
                        result.IsSuccessful = defaultPostMaster.SendMail(guidMailId, strToAddress, strToName,
                          strFromAddress, strFromName,
                          strSubject, strBody);
                    }
                    else
                    {
                        // TODO: NOTE:
                        // We can do reverify if any postmaster is working and set the IsWorking status flag of the PostMasters to true if its back online.
                        // Notify the IT Admins if there is no active service available to process 

                    }
                }
            }
            catch (Exception exp)
            {
                // Log Exception from the previous call.
                result.IsSuccessful = false;
                result.StatusMessage = exp.ToString();
                postMasters.Find(r => r.Id == defaultPostMaster.Id).bIsWorking = false;

                defaultPostMaster = GetNextAvailablePostMaster();
                if (defaultPostMaster != null)
                {
                    result.IsSuccessful = defaultPostMaster.SendMail(guidMailId, strToAddress, strToName,
                          strFromAddress, strFromName,
                          strSubject, strBody);
                }
                else
                {
                    // TODO: NOTE:
                    // We can do reverify if any postmaster is working and set the IsWorking status of the PostMasters if its back online.
                    // Notify the IT Admins as there is no active service available to process 

                }

            }
            return result;
        }

        /// <summary>
        /// Scraps the HTML from the Email Body.
        /// </summary>
        /// <param name="strBody"></param>
        /// <returns> A string</returns>
        /// <remarks>Review following code for any limitations in how the HTML regular expression might have.
        /// http://code.tutsplus.com/tutorials/8-regular-expressions-you-should-know--net-6149
        /// </remarks>
        private string ScrapHTML(string strBody)
        {
            // Data Scrapping can controlled (on or off) from the Settings.xml
            strBody = Regex.Replace(strBody, @"<[^>].+?>", String.Empty);
            return strBody;
        }

        /// <summary>
        /// This API gets the next available postmaster for sending the mail if current one fails.
        /// We can define a complex strategy here on how the default is initialized and 
        /// how to get the next available postmoster. Currently, I simply select the next available postmaster
        /// 
        /// </summary>
        /// <returns></returns>
        private PostMasterBase GetNextAvailablePostMaster()
        {
            return defaultPostMaster = postMasters.Find(r => r.Id != defaultPostMaster.Id && r.bIsWorking == true);
        }

        private void Init()
        {
            // Assign a unique mail id for the given mail and log the information.
            // Logger.Log(Guid)
            guidMailId = new Guid();

            // Initialize the default PostMaster for sending the mail. 
            // TODO: The default postmaster / mailer can be defined in the Settings.xml OR 
            // It can be selected based on a strategy using strategy pattern i.e. Cost effective vs
            // more reliable vs other critical factors important for the business.
            PostMasterSendGrid postmasterSendGrid = new PostMasterSendGrid();


            PostMasterMailGun postmasterMailGun = new PostMasterMailGun();

            postMasters = new List<PostMasterBase>();
            postMasters.Add(postmasterSendGrid);
            postMasters.Add(postmasterMailGun);

            // Choose the default post master based on the strategy. We can define cost, and other factors
            // with in the PostMasterBase and then evaluate the postmasters (mailer services) based on the 
            // criteria we define instead of hardcoded selection of the default.
            defaultPostMaster = postmasterSendGrid;
        }
        /// <summary>
        /// This API validates the inputs.
        /// </summary>
        /// <param name="guidMailId">Unique identity of the specific email</param>
        /// <param name="strToAddress">Address to whom the email is sent</param>
        /// <param name="strToName">Name of the To email address</param>
        /// <param name="strFromAddress">Address from which email is sent</param>
        /// <param name="strFromName">Friendly name of the From email</param>
        /// <param name="strSubject">Subject of the email</param>
        /// <param name="strBody">Body of the email which is in html</param>
        protected void ValidateInputs(Guid guidMailId, string strToAddress, string strToName, string strFromAddress, string strFromName, string strSubject, string strBody)
        {
            // TODO: We can specifiy which specific argument is not valid instead of just mentioning one of the input arguments.
            if (guidMailId == null || string.IsNullOrEmpty(strToAddress) || string.IsNullOrEmpty(strFromAddress) ||
                string.IsNullOrEmpty(strFromName) || string.IsNullOrEmpty(strToName) ||
                string.IsNullOrEmpty(strSubject) || string.IsNullOrEmpty(strBody))
            {

                throw new ArgumentException("One of the input arguments to SendMail API is null or empty.");
            }

            IsValidEmailAddress(strFromAddress);
            IsValidEmailAddress(strToAddress);
        }

        /// <summary>
        /// Using a regular expression to validate if the email address is valid or not
        /// </summary>
        /// <param name="strEmailAddress">Email Address</param>
        /// <remarks>
        /// There are several caveats with what this regular expression can detect as far as complexity in email addresses are concerned.
        /// Please refer to following documentation for details.
        /// http://www.regular-expressions.info/email.html
        /// </remarks>
        private void IsValidEmailAddress(string strEmailAddress)
        {
            // This regular expression needs to be optimized quite a bit.
            Regex regEx = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            MatchCollection mc = regEx.Matches(strEmailAddress);
            if (mc.Count == 0)
            {
                throw new ArgumentException(String.Format("The format of email address {0} is invalid.", strEmailAddress));
            }
        }
    }
}

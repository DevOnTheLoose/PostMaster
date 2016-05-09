using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace MailerService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMailer" in both code and config file together.
    [ServiceContract]
    public interface IMailer
    {
        // TODO: Add your service operations here
        [OperationContract]
        [WebInvoke(Method = "POST",UriTemplate = "SendMail", ResponseFormat = WebMessageFormat.Json, 
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ReturnStatus SendMail(string to, string to_name,
                             string from, string from_name,
                             string subject, string body);
    }

    public class ReturnStatus
    {
        bool bIsSuccessful = true;
        string strMessage = String.Empty;

        [DataMember]
        public bool IsSuccessful
        {
            get { return bIsSuccessful; }
            set { bIsSuccessful = value; }
        }

        [DataMember]
        public string StatusMessage
        {
            get { return strMessage; }
            set { strMessage = value; }
        }
    }
}

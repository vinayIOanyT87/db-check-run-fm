using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FMBusinessObjects.ServiceRequests
{
    /// <summary>
    /// Used to communicate information required for a login request
    /// </summary>
    [DataContract]
    public class SecurityLoginRequest
    {
        [DataMember]
        public string SiteID { get; set; }

        [DataMember]
        public string UserID { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public bool CACEnabled { get; set; }

        [DataMember]
        public int TimeOut { get; set; }

        /// <summary>
        /// Performs validation check on the object and throws an exception if the object is not valid.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(SiteID))
            {
                throw new ArgumentNullException("SiteID");
            }

            if (string.IsNullOrWhiteSpace(UserID))
            { 
                throw new ArgumentNullException("UserID");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentNullException("Password");
            }
        }
    }
}

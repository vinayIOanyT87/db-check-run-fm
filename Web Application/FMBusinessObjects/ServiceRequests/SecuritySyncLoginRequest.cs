// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecuritySyncLoginRequest.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Used to communicate information required for a sync login request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.ServiceRequests
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Cryptography.X509Certificates;
    using System.Web;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Used to communicate information required for a sync login request
    /// </summary>
    [DataContract]
    [KnownType(typeof(SYNCREQUESTTYPE))]
    [KnownType(typeof(SYNCTRANSFERTYPE))]
    public class SecuritySyncLoginRequest
    {
        [DataMember]
        public string SiteID { get; set; }

        [DataMember]
        public string UserID { get; set; }

        [DataMember]
        public string Password { get; set; }

		[DataMember]
		public Guid SyncSessionID { get; set; }

		[DataMember]
        public SYNCREQUESTTYPE SyncRequestTypeIndex { get; set; }

        [DataMember]
        public SYNCTRANSFERTYPE SyncTransferTypeIndex { get; set; }

        [DataMember]
        public Guid SourceNodeGuid { get; set; }

        [DataMember]
        public string SourceNodeMachineName { get; set; }

        [DataMember]
        public X509Certificate2 X509ClientCertificate { get; set; }

        [DataMember]
        public byte[] ClientCertificate { get; set; }

        [DataMember]
        public int TimeOut { get; set; }

        /// <summary>
        /// Performs validation check on the object and throws an exception if the object is not valid.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.SiteID))
            {
                throw new ArgumentNullException("SiteID");
            }

            if (string.IsNullOrWhiteSpace(this.UserID))
            { 
                throw new ArgumentNullException("UserID");
            }

            if (string.IsNullOrWhiteSpace(this.Password))
            {
                throw new ArgumentNullException("Password");
            }

			if (this.SyncSessionID == Guid.Empty)
			{
				throw new ArgumentException("SyncSessionID");
			}

			if (this.SourceNodeGuid == Guid.Empty)
            {
                throw new ArgumentException("SourceNodeGuid");
            }

            if (string.IsNullOrWhiteSpace(this.SourceNodeMachineName))
            {
                throw new ArgumentNullException("SourceNodeMachineName");
            }
        }
    }
}

	using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Net;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
	[Serializable]
	[DataContract]
	public class SyncControllerSR
	{
		#region Properties
        [DataMember]
        public SecurityClass Security { get; set; }

        [DataMember]
        public bool EnableBatching { get; set; }

        [DataMember]
        public int RecordsPerBatch { get; set; }

        [DataMember]
        public SYNCTRANSFERTYPE TransferType { get; set; }
        
        [DataMember]
        public SYNCSESSION SessionType { get; set; }

		[DataMember]
		public Guid ClientID { get; set; }

        [DataMember]
        public Guid ClientSessionToken { get; private set; }

        [DataMember]
		public Guid ServerSessionToken { get; private set; }
		
		[DataMember]
		public string SiteID { get; set; }

        [DataMember]
		public System.Nullable<Guid> SiteGuid { get; set; }
        #endregion Properties

        #region Public Methods
        public static SyncControllerSR CreateSyncRequest(SecurityClass pSecurity, Guid pClientID, Guid pClientSessionToken, Guid pServerSessionToken, string pSiteID)
        {
            SyncControllerSR syncRequest = new SyncControllerSR();
            syncRequest.Security = pSecurity;
            syncRequest.ClientID = pClientID;
            syncRequest.ClientSessionToken = pClientSessionToken;
            syncRequest.ServerSessionToken = pServerSessionToken;
            syncRequest.SiteID = pSiteID;

            return (syncRequest);
        }
        #endregion Public Methods
    }
}

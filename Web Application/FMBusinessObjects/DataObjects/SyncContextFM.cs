using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
	using FMBusinessObjects.UtilityObjects;

	public delegate void SyncFailedEventHandler(object Sender, string ErrorMessage);

	public enum OFFLINESYNCACTION
	{
		NotApplicable = 0,
		ExportLocalChanges = 1,
		ImportRemoteChanges = 2
	}

	[Serializable]
	[DataContract]
	[KnownType(typeof(SecurityClass))]
	[KnownType(typeof(SiteClass))]
	[KnownType(typeof(SyncTableToScopeMapColumnCollection))]
	[KnownType(typeof(SyncTableToScopeMapColumnDO))]
	[KnownType(typeof(DBNull))]
	[KnownType(typeof(SYNCSITETYPE))]
	[KnownType(typeof(SYNCANCHORTYPE))]
	[KnownType(typeof(SYNCSINGLEPASSPHASE))]
	public class SyncContextFM : ISynchronizationContext
	{
		const string _UndefinedSyncText = "Undefined Synchronization Session Type";

		private bool _IsBatching = false;
		private int _RecordsPerBatch = 0;
		private bool _MaxBatchSegmentRowCountEncountered = false;
		private SYNCSINGLEPASSPHASE _SyncSinglePassPhase = SYNCSINGLEPASSPHASE.SYNCROOT;

		#region Public Properties

		[DataMember]
		public Guid ClientID { get; set; }
		[DataMember]
		public Guid ServerID { get; set; }
		[DataMember]
		public Guid SyncSessionID { get; set; }
		[DataMember]
		public string ClientName { get; set; }
		[DataMember]
		public string ServerName { get; set; }
		[DataMember]
		public SYNCTRANSFERTYPE TransferType { get; set; }
		[DataMember]
		public SYNCREQUESTTYPE RequestType { get; set; }
		[DataMember]
		public SYNCSESSION SessionType { get; set; }
		[DataMember]
		public string SiteID { get; set; }
		[DataMember]
		public System.Nullable<Guid> SiteGuid { get; set; }
		[DataMember]
		public SYNCSITETYPE SiteType { get; set; }
		[DataMember]
		public bool IsBatching
		{
			get
			{
				return this._IsBatching;
			}

			set
			{
				this._IsBatching = value;
			}
		}
		[DataMember]
		public int RecordsPerBatch
		{
			get
			{
				return this._RecordsPerBatch;
			}
            
			set
			{
				this._RecordsPerBatch = value;

				this._IsBatching = (this._RecordsPerBatch > 0) ? true : false;
			}
		}

		[DataMember]
		public int CurrentBatchSegment { get; set; }
		[DataMember]
		public DateTimeOffset StartDateRange { get; set; }
		[DataMember]
		public DateTimeOffset EndDateRange { get; set; }
		[DataMember]
		public bool UseDateRangeSynchronization { get; set; }
		[DataMember]
		public SecurityClass Security { get; set; }
		[DataMember]
		public SecurityClass ServerSecurity { get; set; }
		[DataMember]
		public string CurrentSyncProfileID { get; set; }
		[DataMember]
		public string CurrentSyncScopeID { get; set; }
		[DataMember]
		public string CurrentSiteID { get; set; }
		[DataMember]
		public System.Nullable<Guid> CurrentSiteGuid { get; set; }
		[DataMember]
		public ArrayList SiteSynchronizationList { get; set; }
		[DataMember]
		public SYNCCONTROLLERSTEP CurrentControllerStep { get; set; }
		[DataMember]
		public VersionInfo ClientVersion { get; set; }
		//[DataMember]
		//public OFFLINESYNCACTION OfflineSyncAction { get; set; }
		[DataMember]
		public DateTimeOffset ContextCreatedDate { get; set; }

		[DataMember]
		public long MaxClientSyncAnchor { get; set; }

		[DataMember]
		public long MaxEnterpriseSyncAnchor { get; set; }

		[DataMember]
		public bool MaxBatchSegmentRowCountEncountered
		{
			get
			{
				return this._MaxBatchSegmentRowCountEncountered;
			}

			set
			{
				this._MaxBatchSegmentRowCountEncountered = value;
			}
		}

		[DataMember]
		public SYNCSINGLEPASSPHASE SyncSinglePassPhase
		{
			get
			{
				return this._SyncSinglePassPhase;
			}

			set
			{
				this._SyncSinglePassPhase = value;
			}
		}


		[DataMember]
		public Dictionary<string, SyncTableToScopeMapColumnCollection> SupportedColumnsByTable { get; set; }

		[DataMember]
		public Dictionary<string, int> SyncTableMaxBatchSegmentRowCountByTable { get; set; }

		[DataMember]
		public Dictionary<string, int> SyncTableFirstTimeSyncOptionsByTable { get; set; }

		#endregion Public Properties

		#region Constructors
		public SyncContextFM()
        {
            this.SiteSynchronizationList = new ArrayList();
            this.ContextCreatedDate = DateTimeOffset.Now;
            this.SupportedColumnsByTable = new Dictionary<string, SyncTableToScopeMapColumnCollection>();
            this.MaxClientSyncAnchor = 0;
            this.MaxEnterpriseSyncAnchor = 0;
            this.ClientVersion = new VersionInfo();
            this.SyncTableMaxBatchSegmentRowCountByTable = new Dictionary<string, int>();
			this.SyncTableFirstTimeSyncOptionsByTable = new Dictionary<string, int>();
		}

        internal SyncContextFM(Guid LocalNodeID)
        {
            this._IsBatching = false;
            this._RecordsPerBatch = 0;

            this.ClientID = LocalNodeID;
            this.ServerID = Guid.Empty;
            this.SyncSessionID = Guid.Empty;
            this.ClientName = string.Empty;
            this.ServerName = string.Empty;
            this.TransferType = SYNCTRANSFERTYPE.ONLINE;
            this.SessionType = SYNCSESSION.DEFAULT;
            this.RequestType = SYNCREQUESTTYPE.MANUAL;
            this.SiteID = null;
            this.SiteGuid = Guid.Empty;
            this.StartDateRange = DateTimeOffset.MinValue;
            this.EndDateRange = DateTimeOffset.MinValue;
            this.UseDateRangeSynchronization = false;
            this.Security = null;
            this.ServerSecurity = null;
            this.CurrentBatchSegment = 0;
            this.CurrentSyncProfileID = null;
            this.CurrentSyncScopeID = null;
            this.CurrentSiteID = null;
            this.CurrentSiteGuid = Guid.Empty;
            this.SiteSynchronizationList = new ArrayList();
            this.CurrentControllerStep = SYNCCONTROLLERSTEP.PROCESS_ALL;
            this.ClientVersion = new VersionInfo();
            //OfflineSyncAction = OFFLINESYNCACTION.NotApplicable;
            this.ContextCreatedDate = DateTimeOffset.Now;
            this.MaxClientSyncAnchor = 0;
            this.MaxEnterpriseSyncAnchor = 0;
            this.SupportedColumnsByTable = new Dictionary<string, SyncTableToScopeMapColumnCollection>();
            this.SyncTableMaxBatchSegmentRowCountByTable = new Dictionary<string, int>();
			this.SyncTableFirstTimeSyncOptionsByTable = new Dictionary<string, int>();
		}
		#endregion Constructors

		#region Static Methods
		/// <summary>
		/// This static method returns an instance of a new SynchronizationContext initialized with the passed in context information.
		/// </summary>
		/// <param name="syncSessionID">This is the unique identifier for the synchronization session.</param>
		/// <param name="clientID">This is the unique identifier for the client (node) that is initiating the synchronization process.  This id cannot be shared by any 
		/// other client that synchronizes with the same Enterprise server.</param>
		/// <param name="clientSecurity">An instance of a <see cref="SecurityClass"/> that contains the client side synchronization session for this session.</param>
		/// <param name="serverID">This is the unique identifier for the server/enterprise (node) that is participating in the synchronization process.  This id unique to a
		/// specific Enterprise server.
		/// </param>
		/// <param name="serverSecurity">An instance of a <see cref="SecurityClass"/> that contains the synchronization session for the remote Enterprise Server.
		/// </param>
		/// <param name="siteID">This is the initial siteID context where synchronization will begin.</param>
		/// <returns>
		/// A new populated instance of a <see cref="SyncContextFM"/> object.
		/// </returns>
		/// <remarks>
		/// The siteID can contain the default siteID defined during the configuration of Synchronization at a remote node.  It may also contain a siteID
		/// that represents the current siteID from which the client initiated Synchronization.  The siteID should NEVER be SiteAdmin.
		/// </remarks>
		/// <exception cref="ArgumentException">Thrown if either <see cref="clientID"/>, <see cref="clientSecurity"/> OR <see cref="serverID"/> has a 
		/// value of Guid.Empty.  Also thrown if <see cref="SiteID"/> contains Null or is Empty.</exception>
		public static SyncContextFM CreateContext(Guid syncSessionID, Guid clientID, SecurityClass clientSecurity, Guid serverID, SecurityClass serverSecurity, string siteID)
		{
			if (syncSessionID == Guid.Empty)
			{
				throw new ArgumentException(@"Must be provided.", "syncSessionID");
			}

			if (clientID == Guid.Empty)
			{
				throw new ArgumentException(@"Must be provided.", "clientID");
			}

			if (clientSecurity == null || (clientSecurity != null && clientSecurity.Token == Guid.Empty))
			{
				throw new ArgumentException(@"A client security object must be provided.", "clientSecurity");
			}

            if (serverSecurity != null && serverSecurity.Token == Guid.Empty)
			{
                throw new ArgumentException(@"A valid server security Token must be provided when passing a server security context.", "serverSecurity");
			}

			if (string.IsNullOrEmpty(siteID))
			{
				throw new ArgumentException(@"A siteID must be provided.", "siteID");
			}

			// The SyncSessionID is based on the server's security Token value since the Sync Session on the Enterprise was initialized with it.
            var context = new SyncContextFM(clientID) { SyncSessionID = syncSessionID, Security = clientSecurity, ServerID = serverID, ServerSecurity = serverSecurity, SiteID = siteID };

			return context;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public static string SyncSessionTypeID(SYNCSESSION syncSessionType)
		{
			switch (syncSessionType)
			{
				case SYNCSESSION.DEFAULT: return "Synchronize Since Last";
				case SYNCSESSION.SCHEMA_UPDATE: return "Re-Synchronize Due to Schema Upgrade";
				case SYNCSESSION.DATE_RANGE: return "Re-Synchronize Changes within the Specified Date Range";
				default:
					return _UndefinedSyncText;
			}
		}
		#endregion Static Methods

		#region Public Methods
		public SyncContextFM Clone()
		{
			return (SyncContextFM)this.MemberwiseClone();
		}
		#endregion Public Methods
	}
}

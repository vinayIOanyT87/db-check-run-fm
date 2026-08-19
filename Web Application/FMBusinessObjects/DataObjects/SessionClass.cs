namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	[Serializable]
	[CollectionDataContract]
	public class SessionClassCollection : List<SessionClass> { }

	[Serializable]
	[DataContract]
	public class SessionClass : BaseDataObject
	{
		[DataMember]
		public Guid UserGuid { get; set; }

		[DataMember]
		public string UserID { get; set; }

		[DataMember]
		public Guid LoginSiteGuid { get; set; }

		[DataMember]
		public string LoginSiteID { get; set; }

		[DataMember]
		public int Timeout { get; set; }

		[DataMember]
		public int MaxConcurrentSessionsPerUser { get; set; }

		[DataMember]
		public Guid Token { get; set; }

		[DataMember]
		public Guid? SynchronizationNodeGuid { get; set; }

		[DataMember]
		public string CSRFToken { get; set; }

		[DataMember]
		public string WebServerName { get; set; }

		[DataMember]
		public string WebServerIpAddress { get; set; }

		[DataMember]
		public string ClientIpAddress { get; set; }


      [DataMember]
      public int OperateAlarmRefreshInterval { get; set; }

      [DataMember]
      public int OperateTagRefreshInterval { get; set; }


		public SessionClass()
		{
			this.WebServerName = Environment.MachineName;
			this.Init();
		}

		public override void Reset()
		{
			this.Init();
		}

		private void Init()
		{
			base.Reset();

			this.UserGuid						= Guid.Empty;
			this.LoginSiteGuid					= Guid.Empty;
			this.UserID							= string.Empty;
			this.SiteID							= string.Empty;
			this.LoginSiteID					= string.Empty;
			this.Timeout						= -1;
			this.MaxConcurrentSessionsPerUser	= 0;
			this.Token							= Guid.NewGuid();
			this.SynchronizationNodeGuid		= null;
			this.CSRFToken						= string.Empty;
			this.WebServerIpAddress				= string.Empty;
			this.ClientIpAddress				= string.Empty;
			this.OperateAlarmRefreshInterval = 0;
			this.OperateTagRefreshInterval = 0;
		}
    }
}
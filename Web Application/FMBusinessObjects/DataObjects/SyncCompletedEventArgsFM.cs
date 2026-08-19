using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
    public delegate void SyncCompletedEventHandler(object Sender, SyncCompletedEventArgsFM Args);
    
    [Serializable]
	[DataContract]
	public class SyncCompletedEventArgsFM : EventArgs
	{
		#region Attributes
		private SyncStatsFM _SyncStats = null;

		private int _MaxSiteCount = 0;
		private int _CurrentSiteCount = 0;
		private int _TotalSitesSynchronized = 0;

		private string _CurrentSiteID = "";
		#endregion Attributes

		#region Properties

		#region SyncStats property
		[DataMember]
		public SyncStatsFM SyncStats
		{
			get { return (_SyncStats); }
			set { _SyncStats = value; }
		}
		#endregion SyncStats property

		#region MaxSiteCount property
		[DataMember]
		public int MaxSiteCount
		{
			get { return (_MaxSiteCount); }
			private set { _MaxSiteCount = value; }
		}
		#endregion MaxSiteCount property

		#region CurrentSiteCount property
		[DataMember]
		public int CurrentSiteCount
		{
			get { return (_CurrentSiteCount); }
			set { _CurrentSiteCount = value; }
		}
		#endregion CurrentSiteCount property

		#region TotalSitesSynchronized property
		[DataMember]
		public int TotalSitesSynchronized
		{
			get { return (_TotalSitesSynchronized); }
			set { _TotalSitesSynchronized = value; }
		}
		#endregion TotalSitesSynchronized property

		#region CurrentSiteID property
		[DataMember]
		public string CurrentSiteID
		{
			get { return _CurrentSiteID; }
			set { _CurrentSiteID = value; }
		}
		#endregion CurrentSiteID property

		#endregion Properties

		#region Constructors / Initialization
		public SyncCompletedEventArgsFM(int MaxSiteCount, 
												 int CurrentSiteCount,
												 int TotalSitesSynchronized,
												 string SiteID)
		{
			_MaxSiteCount = MaxSiteCount;
			_CurrentSiteCount = CurrentSiteCount;
			_TotalSitesSynchronized = TotalSitesSynchronized;
			_CurrentSiteID = SiteID;
		}
		#endregion Constructors / Initialization
	}
}

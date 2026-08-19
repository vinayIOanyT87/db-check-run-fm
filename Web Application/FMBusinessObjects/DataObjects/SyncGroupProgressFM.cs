using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[DataContract]
    [KnownType(typeof(SyncTableProgressFM))]
    public class SyncGroupProgressFM
	{
		#region Attributes
		private string _GroupName = "";
		private int _TotalChanges = 0;
		private int _TotalChangesApplied = 0;
		private int _TotalChangesFailed = 0;
		private int _TotalChangesPending = 0;
		private int _TotalDeletes = 0;
		private int _TotalInserts = 0;
		private int _TotalUpdates = 0;

		private List<SyncTableProgressFM> _TablesProgress = null;
		#endregion Attributes

		#region Properties
		[DataMember]
		public string GroupName
		{
			get { return _GroupName; }
			set { _GroupName = value; }
		}
		[DataMember]
		public int TotalChanges
		{
			get { return _TotalChanges; }
			set { _TotalChanges = value; }
		}
		[DataMember]
		public int TotalChangesApplied
		{
			get { return _TotalChangesApplied; }
			set { _TotalChangesApplied = value; }
		}
		[DataMember]
		public int TotalChangesFailed
		{
			get { return _TotalChangesFailed; }
			set { _TotalChangesFailed = value; }
		}
		[DataMember]
		public int TotalChangesPending
		{
			get { return _TotalChangesPending; }
			set { _TotalChangesPending = value; }
		}
		[DataMember]
		public int TotalDeletes
		{
			get { return _TotalDeletes; }
			set { _TotalDeletes = value; }
		}
		[DataMember]
		public int TotalInserts
		{
			get { return _TotalInserts; }
			set { _TotalInserts = value; }
		}
		[DataMember]
		public int TotalUpdates
		{
			get { return _TotalUpdates; }
			set { _TotalUpdates = value; }
		}
		#endregion Properties

		#region Constructor
		public SyncGroupProgressFM()
		{
			_TablesProgress = new List<SyncTableProgressFM>();
		}
		public SyncGroupProgressFM(string GroupName)
		{
			_GroupName = GroupName;

			_TablesProgress = new List<SyncTableProgressFM>();
		}
		#endregion Constructor
	}
}

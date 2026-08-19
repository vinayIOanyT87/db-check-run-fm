using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[DataContract]
    [KnownType(typeof(SyncConflictFM))]
    public class SyncTableProgressFM
	{
		#region Attributes
		private string _TableName = "";
		private int _ChangesApplied = 0;
		private int _ChangesFailed = 0;
		private int _ChangesPending = 0;
		private Collection<SyncConflictFM> _Conflicts = null;
		private int _Deletes = 0;
		private int _Inserts = 0;
		private int _RowIndex = 0;
		private int _TotalChanges = 0;
		private int _Updates = 0;
		#endregion Attributes

		#region Properties
		[DataMember]
		public string TableName
		{
			get { return _TableName; }
			set { _TableName = value; }
		}
		[DataMember]
		public int ChangesApplied
		{
			get { return _ChangesApplied; }
			set { _ChangesApplied = value; }
		}
		[DataMember]
		public int ChangesFailed
		{
			get { return _ChangesFailed; }
			set { _ChangesFailed = value; }
		}
		[DataMember]
		public int ChangesPending
		{
			get { return _ChangesPending; }
			set { _ChangesPending = value; }
		}
		[DataMember]
		public Collection<SyncConflictFM> Conflicts
		{
			get { return _Conflicts; }
			set { _Conflicts = value; }
		}
		[DataMember]
		public int Deletes
		{
			get { return _Deletes; }
			set { _Deletes = value; }
		}
		[DataMember]
		public int Inserts
		{
			get { return _Inserts; }
			set { _Inserts = value; }
		}
		[DataMember]
		public int RowIndex
		{
			get { return _RowIndex; }
			set { _RowIndex = value; }
		}
		[DataMember]
		public int TotalChanges
		{
			get { return _TotalChanges; }
			set { _TotalChanges = value; }
		}
		[DataMember]
		public int Updates
		{
			get { return _Updates; }
			set { _Updates = value; }
		}
		#endregion Properties

		#region Constructor
		public SyncTableProgressFM()
		{
			_Conflicts = new Collection<SyncConflictFM>();
		}
		public SyncTableProgressFM(string TableName)
		{
			_TableName = TableName;

			_Conflicts = new Collection<SyncConflictFM>();
		}
		#endregion Constructor
	}
}

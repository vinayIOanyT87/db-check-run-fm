// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncRecordConflictDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncRecordConflictCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(SyncRecordConflictDO))]
	public class SyncRecordConflictCollection : List<SyncRecordConflictDO>
	{
	}

	[XmlType("SyncRecordConflict")]
	[DataContract]
	[Serializable]
	[KnownType(typeof(SYNCCONFLICTTYPE))]
	[KnownType(typeof(SYNCCONFLICTRESOLUTIONSTATUS))]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(System.DBNull))]
	public class SyncRecordConflictDO : BaseDataObject, INotifyPropertyChanged
	{
		#region Data Members
		[DataMember]
		private bool _Changed = false;

		[DataMember]
		private Guid _TargetNodeGuid = Guid.Empty;

		[DataMember]
		private string _TargetNodeName = string.Empty;

		[DataMember]
		private string _TableName = String.Empty;

		[DataMember]
		private string _RecordKey = String.Empty;

		[DataMember]
		private long _RecordRowVersion = 0;

		[DataMember]
		private long _ReSyncAnchorMin = 0;

		[DataMember]
		private long _ReSyncAnchorMax = 0;

		[DataMember]
		private SYNCCONFLICTTYPE _SyncConflictTypeIndex = SYNCCONFLICTTYPE.UNKNOWN;

		[DataMember]
		private SYNCCONFLICTRESOLUTIONSTATUS _SyncConflictResolutionStatusIndex = SYNCCONFLICTRESOLUTIONSTATUS.PENDING;

		[DataMember]
		private DateTimeOffset? _ResolvedDate = null;

		[DataMember]
		private string _ResolvedBy = null;

		[DataMember]
		private string _ConflictDescription = string.Empty;

		[DataMember]
		private string _CommandText = null;

		[DataMember]
		private CommandType _CommandType;

		[DataMember]
		private Dictionary<string, object> _Parameters;

		[DataMember]
		private int _Retrys;

		#endregion Data Members

		#region Properties
		public bool Changed
		{
			get { return (this._Changed); }
			set
			{
				if (value == this._Changed)
					return;

				this._Changed = value;

				RaisePropertyChanged("Changed", false);
			}
		}

		public Guid TargetNodeGuid
		{
			get { return this._TargetNodeGuid; }
			set
			{
				if (value == this._TargetNodeGuid)
					return;

				this._TargetNodeGuid = value;

				RaisePropertyChanged("TargetNodeGuid");
			}
		}

		public string TargetNodeName
		{
			get { return this._TargetNodeName; }
			set
			{
				if (value == this._TargetNodeName)
					return;

				SetString("TargetNodeName", 256, value, ref this._TargetNodeName);

				RaisePropertyChanged("TargetNodeName");
			}
		}


		public string TableName
		{
			get { return this._TableName; }
			set
			{
				if (value == this._TableName)
					return;

				SetString("TableName", 128, value, ref this._TableName);

				RaisePropertyChanged("TableName");
			}
		}

		public string RecordKey
		{
			get { return this._RecordKey; }
			set
			{
				if (value == this._RecordKey)
					return;

				SetString("RecordKey", 256, value, ref this._RecordKey);

				RaisePropertyChanged("RecordKey");
			}
		}

		public long RecordRowVersion
		{
			get { return this._RecordRowVersion; }
			set
			{
				this._RecordRowVersion = value;

				RaisePropertyChanged("RecordRowVersion");
			}
		}

		public long ReSyncAnchorMin
		{
			get { return this._ReSyncAnchorMin; }
			set
			{
				this._ReSyncAnchorMin = value;

				RaisePropertyChanged("ReSyncAnchorMin");
			}
		}

		public long ReSyncAnchorMax
		{
			get { return this._ReSyncAnchorMax; }
			set
			{
				this._ReSyncAnchorMax = value;

				RaisePropertyChanged("ReSyncAnchorMax");
			}
		}

		public SYNCCONFLICTTYPE SyncConflictTypeIndex
		{
			get { return this._SyncConflictTypeIndex; }
			set
			{
				if (value == this._SyncConflictTypeIndex)
					return;

				this._SyncConflictTypeIndex = value;

				RaisePropertyChanged("SyncConflictTypeIndex");
			}
		}

		public SYNCCONFLICTRESOLUTIONSTATUS SyncConflictResolutionStatusIndex
		{
			get { return this._SyncConflictResolutionStatusIndex; }
			set
			{
				if (value == this._SyncConflictResolutionStatusIndex)
					return;

				this._SyncConflictResolutionStatusIndex = value;

				RaisePropertyChanged("SyncConflictResolutionStatusIndex");
			}
		}

		public DateTimeOffset? ResolvedDate
		{
			get { return this._ResolvedDate; }
			set
			{
				if (value == this._ResolvedDate)
					return;

				this._ResolvedDate = value;

				RaisePropertyChanged("ResolvedDate");
			}
		}

		public string ResolvedBy
		{
			get { return this._ResolvedBy; }
			set
			{
				if (value == this._ResolvedBy)
					return;

				SetString("ResolvedBy", 50, value, ref this._ResolvedBy);

				RaisePropertyChanged("ResolvedBy");
			}
		}

		public string ConflictDescription
		{
			get { return this._ConflictDescription; }
			set
			{
				if (value == this._ConflictDescription)
					return;

				SetString("ConflictDescription", 2000, value, ref this._ConflictDescription);

				RaisePropertyChanged("ConflictDescription");
			}
		}

		public string CommandText
		{
			get { return this._CommandText; }
			set
			{
				if (value == this._CommandText)
					return;

				SetString("CommandText", 2000, value, ref this._CommandText);

				RaisePropertyChanged("CommandText");
			}
			
		}

		public CommandType CommandType
		{
			get { return this._CommandType; }
			set
			{
				if (value == this._CommandType)
					return;

				this._CommandType=value;

				RaisePropertyChanged("CommandType");
			}
		}

		public Dictionary<string, object> Parameters
		{
			get { return this._Parameters; }
			set
			{
				if (value == this._Parameters)
					return;

				this._Parameters = value;

				RaisePropertyChanged("Parameters");
			}
		}

		public int Retrys
		{
			get { return this._Retrys; }
			set
			{
				if (value == this._Retrys)
					return;

				this._Retrys = value;

				RaisePropertyChanged("Retrys");
			}
		}



		#endregion Properties

		#region Constructors
		/// <summary>
		/// This is the default constructor for the SyncRecordConflictDO class.
		/// </summary>
		public SyncRecordConflictDO()
			: base()
		{
			this.Reset();
		}
		#endregion Constructors

		#region Public methods
		public override void Reset()
		{
			base.Reset();
			this._Changed = false;

			this._IdentityGuid = Guid.Empty;

			this._TargetNodeGuid = Guid.Empty;

			this._TableName = string.Empty;
			this._RecordKey = string.Empty;
			this._RecordRowVersion = 0;
			this._ReSyncAnchorMin = 0;
			this._ReSyncAnchorMax = 0;
			this._SyncConflictTypeIndex = SYNCCONFLICTTYPE.UNKNOWN;
			this._SyncConflictResolutionStatusIndex = SYNCCONFLICTRESOLUTIONSTATUS.PENDING;

			this._ResolvedDate = null;
			this._ResolvedBy = null;

			this._ConflictDescription = string.Empty;
			this._CommandText = string.Empty;
			this._CommandType = CommandType.Text;
			this._Parameters = new Dictionary<string, object>();
			this._Retrys = 0;

			this._CreatedDate = DateTimeOffset.Now;
			this._UpdatedDate = DateTimeOffset.Now;
		}

		public void Load(DataRow row)
		{
			this._IdentityGuid = DataObject.getGuid(row["SyncRecordConflictGuid"]);

			this._TargetNodeGuid = DataObject.getGuid(row["TargetNodeGuid"]);
			this._TableName = DataObject.getString(row["TableName"]);
			this._RecordKey = DataObject.getString(row["RecordKey"]);

			this._RecordRowVersion = DataObject.getLong(row["RecordRowVersion"]);
			this._ReSyncAnchorMin = DataObject.getLong(row["ReSyncAnchorMin"]);
			this._ReSyncAnchorMax = DataObject.getLong(row["ReSyncAnchorMax"]);

			this._SyncConflictTypeIndex = (SYNCCONFLICTTYPE)DataObject.getInt(row["SyncConflictTypeIndex"]);
			this._SyncConflictResolutionStatusIndex = (SYNCCONFLICTRESOLUTIONSTATUS)DataObject.getInt(row["SyncConflictResolutionStatusIndex"]);

			this._ResolvedDate = DataObject.getValue<DateTimeOffset?>(row["ResolvedDate"], null);
			this._ResolvedBy = DataObject.getString(row["ResolvedBy"]);

			this._ConflictDescription = DataObject.getString(row["ConflictDescription"]);
			if (row.Table.Columns.Contains("CommandTest"))
			{
				this._CommandText = DataObject.getString(row["CommandText"]);
			}
			if (row.Table.Columns.Contains("CommandType"))
			{
				this._CommandType = (CommandType)DataObject.getLong(row["CommandType"]);
			}
			if (row.Table.Columns.Contains("Retrys"))
			{
				this._Retrys = DataObject.getInt(row["Retrys"]);
			}

			this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getString(row["CreatedBy"]);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getString(row["UpdatedBy"]);

			this._Changed = false;

			if (row.Table.Columns.Contains("Parameters"))
			{
				byte[] parameterArray = DataObject.getOptionalVarBinary(row["Parameters"]);
				var knownTypeList = new List<Type>();
				knownTypeList.Add(typeof(DateTimeOffset));
				knownTypeList.Add(typeof(DBNull));
				var parameterSerializer = new DataContractSerializer(typeof(Dictionary<string, object>), knownTypeList);
				var stream = new MemoryStream(parameterArray);
				this._Parameters = parameterSerializer.ReadObject(stream) as Dictionary<string, object>;
			}
		}
		#endregion Public Methods

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#region STANDARD RAISE PROPERTY CHANGE NOTIFICATION METHODS FOR DERIVED CLASSES
		protected void RaisePropertyChanged(string propertyName)
		{
			RaisePropertyChanged(propertyName, true);
		}
		protected void RaisePropertyChanged(string propertyName, bool trackChangesFlag)
		{
			if (trackChangesFlag)
				_Changed = true;

			if (null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		protected void RaiseMultiplePropertyChanged(params string[] propertyNames)
		{
			foreach (var each in propertyNames)
			{
				RaisePropertyChanged(each);
			}
		}
		#endregion STANDARD RAISE PROPERTY CHANGE NOTIFICATION METHODS FOR DERIVED CLASSES

		#endregion INotifyPropertyChanged Members
	}
}

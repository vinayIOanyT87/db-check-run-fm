using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using System.ComponentModel;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SyncTableToScopeMapDO))]
    public class SyncTableToScopeMapCollection : List<SyncTableToScopeMapDO>
    {
    }

    [XmlType("SyncTableToScopeMap")]
    [DataContract]
    [Serializable]
    [KnownType(typeof(SyncTableDO))]
    [KnownType(typeof(SyncScopeDO))]
    public class SyncTableToScopeMapDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data Members
        private bool _Changed = false;
        private Guid _SyncScopeGuid = Guid.Empty;
        private Guid _SyncTableGuid = Guid.Empty;
        private int _SyncOrder = 0;
        private SYNCDIRECTION _SyncDirection = SYNCDIRECTION.DOWNLOADONLY;
        private int? _MaxBatchSegmentRowCount = 0;
        private int? _MaxTransferSegmentKB = 0;
        private string _AdditionalFilterJoinClause = null;
        private string _AdditionalFilterWhereClause = null;
        private string _ClientTableNameOverride = null;
		private int? _FirstTimeSyncOption = 0;
        #endregion Data Members

        #region Properties
        [DataMember]
        public bool Changed
        {
            get { return (_Changed); }
            set
            {
                if (value == _Changed)
                    return;

                _Changed = value;

                RaisePropertyChanged("Changed", false);
            }
        }
        [DataMember]
        public Guid SyncScopeGuid
        {
            get { return _SyncScopeGuid; }
            set
            {
                if (value == _SyncScopeGuid)
                    return;

                _SyncScopeGuid = value;

                RaisePropertyChanged("SyncScopeGuid");
            }
        }
        [DataMember]
        public Guid SyncTableGuid
        {
            get { return _SyncTableGuid; }
            set
            {
                if (value == _SyncTableGuid)
                    return;

                _SyncTableGuid = value;

                RaisePropertyChanged("SyncTableGuid");
            }
        }
        [DataMember]
        public int SyncOrder
        {
            get { return _SyncOrder; }
            set
            {
                if (value == _SyncOrder)
                    return;

                _SyncOrder = value;

                RaisePropertyChanged("SyncOrder");
            }
        }
        [DataMember]
        public SYNCDIRECTION SyncDirection
        {
            get { return _SyncDirection; }
            set
            {
                if (value == _SyncDirection)
                    return;

                _SyncDirection = value;

                RaisePropertyChanged("SyncDirection");
            }
        }
        [DataMember]
        public int? MaxBatchSegmentRowCount
        {
            get { return _MaxBatchSegmentRowCount; }
            set
            {
                if (value == _MaxBatchSegmentRowCount)
                    return;

                _MaxBatchSegmentRowCount = value;

                RaisePropertyChanged("MaxBatchSegmentRowCount");
            }
        }
        [DataMember]
        public int? MaxTransferSegmentKB
        {
            get { return _MaxTransferSegmentKB; }
            set
            {
                if (value == _MaxTransferSegmentKB)
                    return;

                _MaxTransferSegmentKB = value;

                RaisePropertyChanged("MaxTransferSegmentKB");
            }
        }
        [DataMember]
        public string AdditionalFilterJoinClause
        {
            get { return _AdditionalFilterJoinClause; }
            set
            {
                if (value == _AdditionalFilterJoinClause)
                    return;

                SetString("AdditionalFilterJoinClause", 1024, value, ref _AdditionalFilterJoinClause);

                RaisePropertyChanged("AdditionalFilterJoinClause");
            }
        }
        [DataMember]
        public string AdditionalFilterWhereClause
        {
            get { return _AdditionalFilterWhereClause; }
            set
            {
                if (value == _AdditionalFilterWhereClause)
                    return;

                SetString("AdditionalFilterWhereClause", 512, value, ref _AdditionalFilterWhereClause);

                RaisePropertyChanged("AdditionalFilterWhereClause");
            }
        }
        [DataMember]
        public string ClientTableNameOverride
        {
            get { return _ClientTableNameOverride; }
            set
            {
                if (value == _ClientTableNameOverride)
                    return;

                SetString("ClientTableNameOverride", 1024, value, ref _ClientTableNameOverride);

                RaisePropertyChanged("ClientTableNameOverride");
            }
        }
		[DataMember]
		public int? FirstTimeSyncOption
		{
			get { return _FirstTimeSyncOption; }
			set
			{
				if (value == _FirstTimeSyncOption)
					return;

				_FirstTimeSyncOption = value;

				RaisePropertyChanged("FirstTimeSyncOption");
			}
		}
		#endregion Properties

		#region Constructors
		/// <summary>
		/// This is the default constructor for the SyncTableToScopeMapDO class.
		/// </summary>
		public SyncTableToScopeMapDO()
            : base()
        {
            this.Reset();
        }

        #endregion Constructors

        #region Public methods
        public override void Reset()
        {
            base.Reset();
            _IdentityGuid = Guid.NewGuid();
            _Changed = false;
            _SyncScopeGuid = Guid.Empty;
            _SyncTableGuid = Guid.Empty;
            _SyncOrder = 0;
            _SyncDirection = SYNCDIRECTION.DOWNLOADONLY;
            _MaxBatchSegmentRowCount = 0;
            _MaxTransferSegmentKB = 0;
            _AdditionalFilterJoinClause = null;
            _AdditionalFilterWhereClause = null;
            _ClientTableNameOverride = null;
            _CreatedDate = DateTimeOffset.Now;
            _UpdatedDate = DateTimeOffset.Now;
			_FirstTimeSyncOption = 0;

		}
        public void Load(DataRow row)
        {
            this.IdentityGuid = DataObject.getGuid(row["SyncProfileGuid"]);
            this.ID = DataObject.getString(row["SyncProfileID"]);
            this.SyncScopeGuid = DataObject.getGuid(row["SyncScopeGuid"]);
            this.SyncTableGuid = DataObject.getGuid(row["SyncTableGuid"]);
            this.SyncOrder = DataObject.getInt(row["SyncOrder"]);
            this.SyncDirection = (SYNCDIRECTION)DataObject.getInt(row["SyncDirection"]);
            this.MaxBatchSegmentRowCount = DataObject.getOptionalInt(row["MaxBatchSegmentRowCount"]);
            this.MaxTransferSegmentKB = DataObject.getOptionalInt(row["MaxTransferSegmentKB"]);
            this.AdditionalFilterJoinClause = DataObject.getString(row["AdditionalFilterJoinClause"]);
            this.AdditionalFilterWhereClause = DataObject.getString(row["AdditionalFilterWhereClause"]);
            this.ClientTableNameOverride = DataObject.getString(row["ClientTableNameOverride"]);
			this.FirstTimeSyncOption = DataObject.getOptionalInt(row["FirstTimeSyncOption"]);

			this.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            this.CreatedBy = DataObject.getString(row["CreatedBy"]);
            this.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], DateTimeOffset.Now);
            this.UpdatedBy = DataObject.getString(row["UpdatedBy"]);

            _Changed = false;
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

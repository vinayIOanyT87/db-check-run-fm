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
    [KnownType(typeof(SyncTableToScopeMapColumnDO))]
    public class SyncTableToScopeMapColumnCollection : List<SyncTableToScopeMapColumnDO>
    {
    }

    [XmlType("SyncTableToScopeMapColumn")]
    [DataContract]
    [Serializable]
    [KnownType(typeof(SyncTableToScopeMapDO))]
    public class SyncTableToScopeMapColumnDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data Members
        private bool _Changed = false;
        private Guid _SyncTableToScopeMapGuid = Guid.Empty;
        private string _ColumnName;
        private int _ColumnIndex;
        private string _ColumnType;
        private int? _ColumnSize;
        private int? _ColumnPrecision;
        private int? _ColumnScale;
        private bool _IsNullableFlag = true;
        private bool _IsPrimaryKeyMemberFlag = false;
        private bool _IsIdentityColumnFlag = false;
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
        public Guid SyncTableToScopeMapGuid
        {
            get { return _SyncTableToScopeMapGuid; }
            set
            {
                if (value == _SyncTableToScopeMapGuid)
                    return;

                _SyncTableToScopeMapGuid = value;

                RaisePropertyChanged("SyncTableToScopeMapGuid");
            }
        }
        [DataMember]
        public string ColumnName
        {
            get { return _ColumnName; }
            set
            {
                if (value == _ColumnName)
                    return;

                SetString("ColumnName", 512, value, ref _ColumnName);

                RaisePropertyChanged("ColumnName");
            }
        }
        [DataMember]
        public int ColumnIndex
        {
            get { return _ColumnIndex; }
            set
            {
                if (value == _ColumnIndex)
                    return;

                _ColumnIndex = value;

                RaisePropertyChanged("ColumnIndex");
            }
        }
        [DataMember]
        public string ColumnType
        {
            get { return _ColumnType; }
            set
            {
                if (value == _ColumnType)
                    return;

                SetString("ColumnType", 256, value, ref _ColumnType);

                RaisePropertyChanged("ColumnType");
            }
        }
        [DataMember]
        public int? ColumnSize
        {
            get { return _ColumnSize; }
            set
            {
                if (value == _ColumnSize)
                    return;

                _ColumnSize = value;

                RaisePropertyChanged("ColumnSize");
            }
        }
        [DataMember]
        public int? ColumnPrecision
        {
            get { return _ColumnPrecision; }
            set
            {
                if (value == _ColumnPrecision)
                    return;

                _ColumnPrecision = value;

                RaisePropertyChanged("ColumnPrecision");
            }
        }
        [DataMember]
        public int? ColumnScale
        {
            get { return _ColumnScale; }
            set
            {
                if (value == _ColumnScale)
                    return;

                _ColumnScale = value;

                RaisePropertyChanged("ColumnScale");
            }
        }
        [DataMember]
        public bool IsNullableFlag
        {
            get { return _IsNullableFlag; }
            set
            {
                if (value == _IsNullableFlag)
                    return;

                _IsNullableFlag = value;

                RaisePropertyChanged("IsNullableFlag");
            }
        }
        [DataMember]
        public bool IsPrimaryKeyMemberFlag
        {
            get { return _IsPrimaryKeyMemberFlag; }
            set
            {
                if (value == _IsPrimaryKeyMemberFlag)
                    return;

                _IsPrimaryKeyMemberFlag = value;

                RaisePropertyChanged("IsPrimaryKeyMemberFlag");
            }
        }
        [DataMember]
        public bool IsIdentityColumnFlag
        {
            get { return _IsIdentityColumnFlag; }
            set
            {
                if (value == _IsIdentityColumnFlag)
                    return;

                _IsIdentityColumnFlag = value;

                RaisePropertyChanged("IsIdentityColumnFlag");
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the SyncTableToScopeMapColumnDO class.
        /// </summary>
        public SyncTableToScopeMapColumnDO()
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
            _SyncTableToScopeMapGuid = Guid.Empty;
            _ColumnName = null;
            _ColumnIndex = 0;
            _ColumnType = null;
            _ColumnSize = 0;
            _ColumnPrecision = 0;
            _ColumnScale = 0;
            _IsNullableFlag = true;
            _IsPrimaryKeyMemberFlag = false;
            _IsIdentityColumnFlag = false;

            _CreatedDate = DateTimeOffset.Now;
            _UpdatedDate = DateTimeOffset.Now;
        }
        public void Load(DataRow row)
        {
            this.IdentityGuid = DataObject.getGuid(row["SyncTableToScopeMapColumnGuid"]);
            this.SyncTableToScopeMapGuid = (Guid)DataObject.getValue<Guid>(row["SyncTableToScopeMapGuid"], Guid.Empty);
            this.ColumnName = DataObject.getString(row["ColumnName"]);
            this.ColumnIndex = DataObject.getInt(row["ColumnIndex"]);
            this.ColumnType = DataObject.getString(row["ColumnType"]);
            this.ColumnSize = DataObject.getOptionalInt(row["ColumnSize"]);
            this.ColumnPrecision = DataObject.getOptionalInt(row["ColumnPrecision"]);
            this.ColumnScale = DataObject.getOptionalInt(row["ColumnScale"]);
            this.IsNullableFlag = DataObject.getBool(row["IsNullableFlag"]);
            this.IsPrimaryKeyMemberFlag = false;
            this.IsIdentityColumnFlag = false;

            this.CreatedBy = DataObject.getString(row["CreatedBy"]);
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

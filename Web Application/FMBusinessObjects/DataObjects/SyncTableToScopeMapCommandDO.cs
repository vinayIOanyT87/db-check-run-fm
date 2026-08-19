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
    [KnownType(typeof(SyncTableToScopeMapCommandDO))]
    public class SyncTableToScopeMapCommandCollection : List<SyncTableToScopeMapCommandDO>
    {
    }

    [XmlType("SyncTableToScopeMapCommand")]
    [DataContract]
    [Serializable]
    [KnownType(typeof(SyncTableToScopeMapDO))]
    public class SyncTableToScopeMapCommandDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data Members
        private bool _Changed = false;
        private Guid _SyncTableToScopeMapGuid = Guid.Empty;
        private string _SelectIncrementalInserts = null;
        private string _ApplyIncrementalInserts = null;
        private string _SelectIncrementalUpdates = null;
        private string _ApplyIncrementalUpdates = null;
        private string _SelectIncrementalDeletes = null;
        private string _ApplyIncrementalDeletes = null;
        private string _SelectUpdateConflicts = null;
        private string _SelectDeleteConflicts = null;
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
        public string SelectIncrementalInserts
        {
            get { return _SelectIncrementalInserts; }
            set
            {
                if (value == _SelectIncrementalInserts)
                    return;

                SetString("_SelectIncrementalInserts", 512, value, ref _SelectIncrementalInserts);

                RaisePropertyChanged("SelectIncrementalInserts");
            }
        }
        [DataMember]
        public string ApplyIncrementalInserts
        {
            get { return _ApplyIncrementalInserts; }
            set
            {
                if (value == _ApplyIncrementalInserts)
                    return;

                SetString("_ApplyIncrementalInserts", 512, value, ref _ApplyIncrementalInserts);

                RaisePropertyChanged("ApplyIncrementalInserts");
            }
        }
        [DataMember]
        public string SelectIncrementalUpdates
        {
            get { return _SelectIncrementalUpdates; }
            set
            {
                if (value == _SelectIncrementalUpdates)
                    return;

                SetString("_SelectIncrementalUpdates", 512, value, ref _SelectIncrementalUpdates);

                RaisePropertyChanged("SelectIncrementalUpdates");
            }
        }
        [DataMember]
        public string ApplyIncrementalUpdates
        {
            get { return _ApplyIncrementalUpdates; }
            set
            {
                if (value == _ApplyIncrementalUpdates)
                    return;

                SetString("_ApplyIncrementalUpdates", 512, value, ref _ApplyIncrementalUpdates);

                RaisePropertyChanged("ApplyIncrementalUpdates");
            }
        }
        [DataMember]
        public string SelectIncrementalDeletes
        {
            get { return _SelectIncrementalDeletes; }
            set
            {
                if (value == _SelectIncrementalDeletes)
                    return;

                SetString("_SelectIncrementalDeletes", 512, value, ref _SelectIncrementalDeletes);

                RaisePropertyChanged("SelectIncrementalDeletes");
            }
        }
        [DataMember]
        public string ApplyIncrementalDeletes
        {
            get { return _ApplyIncrementalDeletes; }
            set
            {
                if (value == _ApplyIncrementalDeletes)
                    return;

                SetString("_ApplyIncrementalDeletes", 512, value, ref _ApplyIncrementalDeletes);

                RaisePropertyChanged("ApplyIncrementalDeletes");
            }
        }
        [DataMember]
        public string SelectUpdateConflicts
        {
            get { return _SelectUpdateConflicts; }
            set
            {
                if (value == _SelectUpdateConflicts)
                    return;

                SetString("_SelectUpdateConflicts", 512, value, ref _SelectUpdateConflicts);

                RaisePropertyChanged("SelectUpdateConflicts");
            }
        }
        [DataMember]
        public string SelectDeleteConflicts
        {
            get { return _SelectDeleteConflicts; }
            set
            {
                if (value == _SelectDeleteConflicts)
                    return;

                SetString("_SelectDeleteConflicts", 512, value, ref _SelectDeleteConflicts);

                RaisePropertyChanged("SelectDeleteConflicts");
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the SyncTableToScopeMapCommandDO class.
        /// </summary>
        public SyncTableToScopeMapCommandDO()
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
            _SelectIncrementalInserts = null;
            _ApplyIncrementalInserts = null;
            _SelectIncrementalUpdates = null;
            _ApplyIncrementalUpdates = null;
            _SelectIncrementalDeletes = null;
            _ApplyIncrementalDeletes = null;
            _SelectUpdateConflicts = null;
            _SelectDeleteConflicts = null;

            _CreatedDate = DateTimeOffset.Now;
            _UpdatedDate = DateTimeOffset.Now;
        }
        public void Load(DataRow row)
        {
            this.IdentityGuid = DataObject.getGuid(row["SyncTableToScopeMapCommandGuid"]);
            this.SyncTableToScopeMapGuid = (Guid)DataObject.getValue<Guid>(row["SyncTableToScopeMapGuid"], Guid.Empty);

            this.SelectIncrementalInserts = DataObject.getString(row["SelectIncrementalInserts"]);
            this.ApplyIncrementalInserts = DataObject.getString(row["ApplyIncrementalInserts"]);
            this.SelectIncrementalUpdates = DataObject.getString(row["SelectIncrementalUpdates"]);
            this.ApplyIncrementalUpdates = DataObject.getString(row["ApplyIncrementalUpdates"]);
            this.SelectIncrementalDeletes = DataObject.getString(row["SelectIncrementalDeletes"]);
            this.ApplyIncrementalDeletes = DataObject.getString(row["ApplyIncrementalDeletes"]);
            this.SelectUpdateConflicts = DataObject.getString(row["SelectUpdateConflicts"]);
            this.SelectDeleteConflicts = DataObject.getString(row["SelectDeleteConflicts"]);

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

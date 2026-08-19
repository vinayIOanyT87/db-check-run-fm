// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncRecordConflictToSyncSessionDetailMapClass.cs" company="Varec, Inc.">
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
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SyncRecordConflictToSyncSessionDetailMap))]
    public class SyncRecordConflictToSyncSessionDetailCollection : List<SyncRecordConflictToSyncSessionDetailMap>
    {
    }

    [XmlType("SyncRecordConflictToSyncSessionDetailMapClass")]
    [DataContract]
    [Serializable]
    public class SyncRecordConflictToSyncSessionDetailMap : BaseDataObject, INotifyPropertyChanged, ICloneable
    {
        #region Data Members
        private bool _Changed = false;

        private Guid _SyncRecordConflictGuid = Guid.Empty;
        private Guid _SyncSessionDetailGuid = Guid.Empty;

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
        public Guid SyncRecordConflictGuid
        {
            get { return _SyncRecordConflictGuid; }
            set
            {
                if (value == _SyncRecordConflictGuid)
                    return;

                _SyncRecordConflictGuid = value;

                RaisePropertyChanged("SyncRecordConflictGuid");
            }
        }
        [DataMember]
        public Guid SyncSessionDetailGuid
        {
            get { return _SyncSessionDetailGuid; }
            set
            {
                if (value == _SyncSessionDetailGuid)
                    return;

                _SyncSessionDetailGuid = value;

                RaisePropertyChanged("SyncSessionDetailGuid");
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the SyncRecordConflictToSyncSessionDetailMap class.
        /// </summary>
        public SyncRecordConflictToSyncSessionDetailMap()
            : base()
        {
            this.Reset();
        }

        public SyncRecordConflictToSyncSessionDetailMap(SyncRecordConflictToSyncSessionDetailMap sourceDO)
            : base()
        {
            this.Changed = sourceDO.Changed;
            this.SyncRecordConflictGuid = sourceDO.SyncRecordConflictGuid;
            this.SyncSessionDetailGuid = sourceDO.SyncSessionDetailGuid;
        }
        #endregion Constructors

        #region Public methods
        public override void Reset()
        {
            base.Reset();
            this._Changed = false;

            this._IdentityGuid = Guid.NewGuid();

            this._SyncRecordConflictGuid = Guid.Empty;
            this._SyncSessionDetailGuid = Guid.Empty;

            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
        }
        public void Load(DataRow row)
        {
            this._IdentityGuid = DataObject.getGuid(row["SyncRecordConflictToSyncSessionDetailGuid"]);

            this._SyncRecordConflictGuid = DataObject.getGuid(row["SyncRecordConflictGuid"]);
            this._SyncSessionDetailGuid = DataObject.getGuid(row["SyncSessionDetailGuid"]);

            this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            this._CreatedBy = DataObject.getString(row["CreatedBy"]);

            this._Changed = false;
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

        #region ICloneable Members

        public object Clone()
        {
            return (this.MemberwiseClone());
        }

        #endregion ICloneable Members
    }
}

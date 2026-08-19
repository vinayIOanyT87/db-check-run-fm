// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncServiceStateDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncServiceStateCollection type.
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
    [KnownType(typeof(SyncServiceStateDO))]
    public class SyncServiceStateCollection : List<SyncServiceStateDO>
    {
    }

    [XmlType("SyncServiceState")]
    [DataContract]
    [Serializable]
    [KnownType(typeof(SYNCSERVICESTATE))]
    public class SyncServiceStateDO : INotifyPropertyChanged
    {
        #region Data Members
        private bool _Changed = false;

        private SYNCSERVICESTATE _SyncServiceState = SYNCSERVICESTATE.UNAVAILABLE;
        private bool _CurrentSessionIsSynchronizing = false;
        private DateTimeOffset _AsOfDate = DateTimeOffset.MinValue;
        private DateTimeOffset _LastSynchronizationDate = DateTimeOffset.MinValue;
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
        public SYNCSERVICESTATE SyncServiceState
        {
            get { return _SyncServiceState; }
            set
            {
                if (value == _SyncServiceState)
                    return;

                _SyncServiceState = value;

                RaisePropertyChanged("SyncServiceState");
            }
        }
        [DataMember]
        public bool CurrentSessionIsSynchronizing
        {
            get { return (_CurrentSessionIsSynchronizing); }
            set
            {
                if (value == _CurrentSessionIsSynchronizing)
                    return;

                _CurrentSessionIsSynchronizing = value;

                RaisePropertyChanged("CurrentSessionIsSynchronizing");
            }
        }
        [DataMember]
        public DateTimeOffset AsOfDate
        {
            get { return (_AsOfDate); }
            set
            {
                if (value == _AsOfDate)
                    return;

                _AsOfDate = value;

                RaisePropertyChanged("AsOfDate");
            }
        }
        [DataMember]
        public DateTimeOffset LastSynchronizationDate
        {
            get { return (_LastSynchronizationDate); }
            set
            {
                if (value == _LastSynchronizationDate)
                    return;

                _LastSynchronizationDate = value;

                RaisePropertyChanged("LastSynchronizationDate");
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the SyncServiceState Settings class.
        /// </summary>
        public SyncServiceStateDO()
            : base()
        {
            this.Reset();
        }

        #endregion Constructors

        #region Public methods
        public virtual void Reset()
        {
            this._Changed = false;

            this._SyncServiceState = SYNCSERVICESTATE.UNAVAILABLE;
            this._CurrentSessionIsSynchronizing = false;
            this._AsOfDate = DateTimeOffset.MinValue;
            this._LastSynchronizationDate = DateTimeOffset.MinValue;
        }
        public void Load(DataRow row)
        {
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
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyServiceStateDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GasboyServiceStateCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using FuelsManager.Afss.BusinessObjects.Constants;
    
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(GasboyServiceState))]
    public class GasboyServiceStateCollection : List<GasboyServiceState>
    {
    }

    [XmlType("GasboyServiceState")]
    [DataContract]
    [Serializable]
    [KnownType(typeof(ExternalStationStatus))]
    public class GasboyServiceState : INotifyPropertyChanged
    {
        #region Data Members
        private bool _Changed = false;

        private ExternalStationServiceProcessState _ServiceState = ExternalStationServiceProcessState.Unavailable;
        private bool _WorkInProgress = false;
        private DateTimeOffset _AsOfDate = DateTimeOffset.MinValue;
        private DateTimeOffset _LastCommunicationDate = DateTimeOffset.MinValue;
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
        public ExternalStationServiceProcessState ServiceState
        {
            get { return _ServiceState; }
            set
            {
                if (value == _ServiceState)
                    return;

                _ServiceState = value;

                RaisePropertyChanged("ServiceState");
            }
        }
        [DataMember]
        public bool WorkInProgress
        {
            get { return (_WorkInProgress); }
            set
            {
                if (value == _WorkInProgress)
                    return;

                _WorkInProgress = value;

                RaisePropertyChanged("WorkInProgress");
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
        public DateTimeOffset LastCommunicationDate
        {
            get { return (_LastCommunicationDate); }
            set
            {
                if (value == _LastCommunicationDate)
                    return;

                _LastCommunicationDate = value;

                RaisePropertyChanged("LastCommunicationDate");
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the GasboyServiceState Settings class.
        /// </summary>
        public GasboyServiceState()
            : base()
        {
            this.Reset();
        }

        #endregion Constructors

        #region Public methods
        public virtual void Reset()
        {
            this._Changed = false;

            this._ServiceState = ExternalStationServiceProcessState.Unavailable;
            this._WorkInProgress = false;
            this._AsOfDate = DateTimeOffset.MinValue;
            this._LastCommunicationDate = DateTimeOffset.MinValue;
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

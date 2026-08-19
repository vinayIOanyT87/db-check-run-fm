// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncSessionDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncSessionLogCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SyncSessionLogDO))]
    public class SyncSessionLogCollection : List<SyncSessionLogDO>
    {
    }

    [XmlType("SyncSession")]
    [DataContract]
    [Serializable]
    [KnownType(typeof(SYNCREQUESTTYPE))]
    [KnownType(typeof(SYNCTRANSFERTYPE))]
    [KnownType(typeof(SYNCSESSIONSTATUS))]
    [KnownType(typeof(SYNCSESSIONSTATE))]
    public class SyncSessionLogDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data Members
        private bool _Changed = false;

        private string _SyncProfileID = string.Empty;
        private SYNCREQUESTTYPE _SyncRequestTypeIndex = SYNCREQUESTTYPE.MANUAL;
        private SYNCTRANSFERTYPE _SyncTransferTypeIndex = SYNCTRANSFERTYPE.ONLINE;
        private SYNCSESSIONSTATUS _SyncSessionStatusIndex = SYNCSESSIONSTATUS.NEW;
        private SYNCSESSIONSTATE _SyncSessionStateIndex = SYNCSESSIONSTATE.INIT;

        private DateTimeOffset? _SyncDateRangeStart = null;
        private DateTimeOffset? _SyncDateRangeEnd = null;

        private DateTimeOffset? _StartDate = DateTimeOffset.MinValue;
        private DateTimeOffset? _EndDate = null;

        private Guid _RemoteNodeGuid = Guid.Empty;
        private string _RemoteNodeMachineName = string.Empty;

        private long _SyncAnchorMin = 0;

        private long _SyncAnchorMax = 0;

	    private int _Conflicts = 0;
        #endregion Data Members

        #region Properties
        [DataMember]
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
        [DataMember]
        public string SyncProfileID
        {
            get { return this._SyncProfileID; }
            set
            {
                if (value == this._SyncProfileID)
                    return;

                SetString("SyncProfileID", 40, value, ref this._SyncProfileID);

                RaisePropertyChanged("SyncProfileID");
            }
        }
        [DataMember]
        public SYNCREQUESTTYPE SyncRequestTypeIndex
        {
            get { return this._SyncRequestTypeIndex; }
            set
            {
                if (value == this._SyncRequestTypeIndex)
                    return;

                this._SyncRequestTypeIndex = value;

                RaisePropertyChanged("SyncRequestTypeIndex");
            }
        }
        [DataMember]
        public SYNCTRANSFERTYPE SyncTransferTypeIndex
        {
            get { return this._SyncTransferTypeIndex; }
            set
            {
                if (value == this._SyncTransferTypeIndex)
                    return;

                this._SyncTransferTypeIndex = value;

                RaisePropertyChanged("SyncTransferTypeIndex");
            }
        }
        [DataMember]
        public SYNCSESSIONSTATUS SyncSessionStatusIndex
        {
            get { return this._SyncSessionStatusIndex; }
            set
            {
                if (value == this._SyncSessionStatusIndex)
                    return;

                this._SyncSessionStatusIndex = value;

                RaisePropertyChanged("SyncSessionStatusIndex");
            }
        }
        [DataMember]
        public SYNCSESSIONSTATE SyncSessionStateIndex
        {
            get { return this._SyncSessionStateIndex; }
            set
            {
                if (value == this._SyncSessionStateIndex)
                    return;

                this._SyncSessionStateIndex = value;

                RaisePropertyChanged("SyncSessionStateIndex");
            }
        }
        [DataMember]
        public DateTimeOffset? SyncDateRangeStart
        {
            get { return this._SyncDateRangeStart; }
            set
            {
                if (value == this._SyncDateRangeStart)
                    return;

                this._SyncDateRangeStart = value;

                RaisePropertyChanged("SyncDateRangeStart");
            }
        }
        [DataMember]
        public DateTimeOffset? SyncDateRangeEnd
        {
            get { return this._SyncDateRangeEnd; }
            set
            {
                if (value == this._SyncDateRangeEnd)
                    return;

                this._SyncDateRangeEnd = value;

                RaisePropertyChanged("SyncDateRangeEnd");
            }
        }
        [DataMember]
        public DateTimeOffset? StartDate
        {
            get { return this._StartDate; }
            set
            {
                if (value == this._StartDate)
                    return;

                this._StartDate = value;

                RaisePropertyChanged("StartDate");
            }
        }
        [DataMember]
        public DateTimeOffset? EndDate
        {
            get { return this._EndDate; }
            set
            {
                if (value == this._EndDate)
                    return;

                this._EndDate = value;

                RaisePropertyChanged("EndDate");
            }
        }
        [DataMember]
        public Guid RemoteNodeGuid
        {
            get { return this._RemoteNodeGuid; }
            set
            {
                if (value == this._RemoteNodeGuid)
                    return;

                this._RemoteNodeGuid = value;

                RaisePropertyChanged("RemoteNodeGuid");
            }
        }
        [DataMember]
        public string RemoteNodeMachineName
        {
            get { return this._RemoteNodeMachineName; }
            set
            {
                if (value == this._RemoteNodeMachineName)
                    return;

                SetString("RemoteNodeMachineName", 128, value, ref this._RemoteNodeMachineName);

                RaisePropertyChanged("RemoteNodeMachineName");
            }
        }
        [DataMember]
        public long SyncAnchorMin
        {
            get { return this._SyncAnchorMin; }
            set
            {
                this._SyncAnchorMin = value;
                RaisePropertyChanged("SyncAnchorMin");
            }
        }
        [DataMember]
        public long SyncAnchorMax
        {
            get { return this._SyncAnchorMax; }
            set
            {
                this._SyncAnchorMax = value;
                RaisePropertyChanged("SyncAnchorMax");
            }
        }

	    [DataMember]
	    public int Conflicts
	    {
			get
			{
				return this._Conflicts; 
			}
		    set
		    {
			    this._Conflicts = value;
				RaisePropertyChanged("Conflicts");
		    }
	    }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// This is the default constructor for the SyncSessionDO class.
        /// </summary>
        public SyncSessionLogDO()
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

            this._IdentityGuid = Guid.NewGuid();

            this._SyncProfileID = string.Empty;
            this._SyncRequestTypeIndex = SYNCREQUESTTYPE.MANUAL;
            this._SyncTransferTypeIndex = SYNCTRANSFERTYPE.ONLINE;
            this._SyncSessionStatusIndex = SYNCSESSIONSTATUS.NEW;
            this._SyncSessionStateIndex = SYNCSESSIONSTATE.INIT;

            this._SyncDateRangeStart = null;
            this._SyncDateRangeEnd = null;
            this._StartDate = null;
            this._EndDate = null;

            this._RemoteNodeGuid = Guid.Empty;
            this._RemoteNodeMachineName = string.Empty;

            this._SyncAnchorMin = 0;
            this._SyncAnchorMax = 0;

	        this._Conflicts = 0;

            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
        }
        #endregion Public Methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #region STANDARD RAISE PROPERTY CHANGE NOTIFICATION METHODS FOR DERIVED CLASSES

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected void RaisePropertyChanged(string propertyName)
        {
            this.RaisePropertyChanged(propertyName, true);
        }

        /// <summary>
        /// The raise property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="trackChangesFlag">
        /// The track changes flag.
        /// </param>
        protected void RaisePropertyChanged(string propertyName, bool trackChangesFlag)
        {
            if (trackChangesFlag)
            {
                this._Changed = true;
            }

            if (null != this.PropertyChanged)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The raise multiple property changed.
        /// </summary>
        /// <param name="propertyNames">
        /// The property names.
        /// </param>
        protected void RaiseMultiplePropertyChanged(params string[] propertyNames)
        {
            foreach (var each in propertyNames)
            {
                this.RaisePropertyChanged(each);
            }
        }
        #endregion STANDARD RAISE PROPERTY CHANGE NOTIFICATION METHODS FOR DERIVED CLASSES

        #endregion INotifyPropertyChanged Members
    }
}

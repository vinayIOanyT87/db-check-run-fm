// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncServerConfigurationDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The sync server configuration collection.
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

    using FMBusinessObjects.ChannelFactories;

	/// <summary>
    /// The sync server configuration collection.
    /// </summary>
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SyncServerConfigurationDO))]
    public class SyncServerConfigurationCollection : List<SyncServerConfigurationDO>
    {
    }

    /// <summary>
    /// The sync server configuration do.
    /// </summary>
    [XmlType("SyncServerConfiguration")]
    [DataContract]
    [Serializable]
    public class SyncServerConfigurationDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data Members

        /// <summary>
        /// The _ enable change tracking.
        /// </summary>
        private bool _EnableChangeTracking = true;

        /// <summary>
        /// The _ changed.
        /// </summary>
        private bool _Changed = false;

        /// <summary>
        /// The _ sync node GUID.
        /// </summary>
        private Guid _SyncNodeGuid = Guid.Empty;    // This field is given to us for quick reference from ConfigurationSettings 
        // because there can only be one NodeGuid per physical instance.

        /// <summary>
        /// The _ allow synchronization flag.
        /// </summary>
        private bool _AllowSynchronizationFlag = false;

        /// <summary>
        /// The _ accept fm user authentication flag.
        /// </summary>
        private bool _AcceptFMUserAuthenticationFlag = false;

        /// <summary>
        /// The _ accept client certificate authentication flag.
        /// </summary>
        private bool _AcceptClientCertificateAuthenticationFlag = false;

        /// <summary>
        /// The _ client signature required for messages flag.
        /// </summary>
        private bool _ClientSignatureRequiredForMessagesFlag = false;

        /// <summary>
        /// The _ client encryption required for messages flag.
        /// </summary>
        private bool _ClientEncryptionRequiredForMessagesFlag = false;

        /// <summary>
        /// The _ offline synchronization working directory.
        /// </summary>
        private string _OfflineSynchronizationWorkingDirectory = @"C:\temp\fmsync";

		/// <summary>
		/// The default hours for node health critical threshold.
		/// </summary>
		private int _NodeHealthCriticalThresholdHours = FMChannelHelper.DefaultNodeHealthCriticalThresholdHours;

		/// <summary>
		/// The default hours for node health caution threshold.
		/// </summary>
		private int _NodeHealthCautionThresholdHours = FMChannelHelper.DefaultNodeHealthCautionThresholdHours;
        #endregion Data Members

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether enable change tracking.
        /// </summary>
        [DataMember]
        public bool EnableChangeTracking
        {
            get
            {
                return this._EnableChangeTracking;
            }

            set
            {
                this._EnableChangeTracking = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether changed.
        /// </summary>
        [DataMember]
        public bool Changed
        {
            get
            {
                return this._Changed;
            }

            set
            {
                if (value == this._Changed)
                {
                    return;
                }

                this._Changed = value;

                this.RaisePropertyChanged("Changed", false);
            }
        }

        /// <summary>
        /// Gets or sets the sync node GUID.
        /// </summary>
        [DataMember]
        public Guid SyncNodeGuid
        {
            get
            {
                return this._SyncNodeGuid;
            }

            set
            {
                if (value == this._SyncNodeGuid)
                {
                    return;
                }

                this._SyncNodeGuid = value;

                this.RaisePropertyChanged("SyncNodeGuid");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether allow synchronization flag.
        /// </summary>
        [DataMember]
        public bool AllowSynchronizationFlag
        {
            get
            {
                return this._AllowSynchronizationFlag;
            }

            set
            {
                if (value == this._AllowSynchronizationFlag)
                {
                    return;
                }

                this._AllowSynchronizationFlag = value;

                this.RaisePropertyChanged("AllowSynchronizationFlag");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether accept fm user authentication flag.
        /// </summary>
        [DataMember]
        public bool AcceptFMUserAuthenticationFlag
        {
            get
            {
                return this._AcceptFMUserAuthenticationFlag;
            }

            set
            {
                if (value == this._AcceptFMUserAuthenticationFlag)
                {
                    return;
                }

                this._AcceptFMUserAuthenticationFlag = value;

                this.RaisePropertyChanged("AcceptFMUserAuthenticationFlag");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether accept client certificate authentication flag.
        /// </summary>
        [DataMember]
        public bool AcceptClientCertificateAuthenticationFlag
        {
            get
            {
                return this._AcceptClientCertificateAuthenticationFlag;
            }

            set
            {
                if (value == this._AcceptClientCertificateAuthenticationFlag)
                {
                    return;
                }

                this._AcceptClientCertificateAuthenticationFlag = value;

                this.RaisePropertyChanged("AcceptClientCertificateAuthenticationFlag");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether client signature required for messages flag.
        /// </summary>
        [DataMember]
        public bool ClientSignatureRequiredForMessagesFlag
        {
            get
            {
                return this._ClientSignatureRequiredForMessagesFlag;
            }

            set
            {
                if (value == this._ClientSignatureRequiredForMessagesFlag)
                {
                    return;
                }

                this._ClientSignatureRequiredForMessagesFlag = value;

                this.RaisePropertyChanged("ClientSignatureRequiredForMessagesFlag");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether client encryption required for messages flag.
        /// </summary>
        [DataMember]
        public bool ClientEncryptionRequiredForMessagesFlag
        {
            get
            {
                return this._ClientEncryptionRequiredForMessagesFlag;
            }

            set
            {
                if (value == this._ClientEncryptionRequiredForMessagesFlag)
                {
                    return;
                }

                this._ClientEncryptionRequiredForMessagesFlag = value;

                this.RaisePropertyChanged("ClientEncryptionRequiredForMessagesFlag");
            }
        }

        /// <summary>
        /// Gets or sets the offline synchronization working directory.
        /// </summary>
        [DataMember]
        public string OfflineSynchronizationWorkingDirectory
        {
            get
            {
                return this._OfflineSynchronizationWorkingDirectory;
            }

            set
            {
                if (value == this._OfflineSynchronizationWorkingDirectory)
                {
                    return;
                }

                this.SetString("OfflineSynchronizationWorkingDirectory", 512, value, ref this._OfflineSynchronizationWorkingDirectory);

                this.RaisePropertyChanged("OfflineSynchronizationWorkingDirectory");
            }
        }

		/// <summary>
		/// Gets or sets a value indicating node health critical threshold hours.
		/// </summary>
		[DataMember]
		public int NodeHealthCriticalThresholdHours
		{
			get
			{
				return this._NodeHealthCriticalThresholdHours;
			}

			set
			{
				if (value == this._NodeHealthCriticalThresholdHours)
				{
					return;
				}

				this._NodeHealthCriticalThresholdHours = value;

				this.RaisePropertyChanged("NodeHealthCriticalThresholdHours");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating node health caution threshold hours.
		/// </summary>
		[DataMember]
		public int NodeHealthCautionThresholdHours
		{
			get
			{
				return this._NodeHealthCautionThresholdHours;
			}

			set
			{
				if (value == this._NodeHealthCautionThresholdHours)
				{
					return;
				}

				this._NodeHealthCautionThresholdHours = value;

				this.RaisePropertyChanged("NodeHealthCautionThresholdHours");
			}
		}
		#endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncServerConfigurationDO"/> class. 
        /// This is the default constructor for the Line Item DO class.
        /// </summary>
        public SyncServerConfigurationDO()
            : base()
        {
            this.Reset();
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// The reset.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this._Changed = false;

            this._IdentityGuid = Guid.Empty;
            this._AllowSynchronizationFlag = false;
            this._AcceptFMUserAuthenticationFlag = false;
            this._AcceptClientCertificateAuthenticationFlag = false;
            this._ClientSignatureRequiredForMessagesFlag = false;
            this._ClientEncryptionRequiredForMessagesFlag = false;
            this._OfflineSynchronizationWorkingDirectory = @"C:\temp\fmsync";
			this._NodeHealthCriticalThresholdHours = FMChannelHelper.DefaultNodeHealthCriticalThresholdHours;
			this._NodeHealthCautionThresholdHours = FMChannelHelper.DefaultNodeHealthCautionThresholdHours;

            this._SyncNodeGuid = Guid.Empty;

            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        public void Load(DataRow row)
        {
            this._IdentityGuid = DataObject.getGuid(row["SyncServerConfigurationGuid"]);
            this._AllowSynchronizationFlag = DataObject.getValue<bool>(row["AllowSynchronizationFlag"], false);
            this._AcceptFMUserAuthenticationFlag = DataObject.getValue<bool>(row["AcceptFMUserAuthenticationFlag"], false);
            this._AcceptClientCertificateAuthenticationFlag = DataObject.getValue<bool>(row["AcceptClientCertificateAuthenticationFlag"], false);
            this._ClientSignatureRequiredForMessagesFlag = DataObject.getValue<bool>(row["ClientSignatureRequiredForMessagesFlag"], false);
            this._ClientEncryptionRequiredForMessagesFlag = DataObject.getValue<bool>(row["ClientEncryptionRequiredForMessagesFlag"], false);
            this._OfflineSynchronizationWorkingDirectory = DataObject.getString(row["OfflineSynchronizationWorkingDirectory"]);
			this._NodeHealthCriticalThresholdHours = DataObject.getInt(row["NodeHealthCriticalThresholdHours"]);
			this._NodeHealthCautionThresholdHours = DataObject.getInt(row["NodeHealthCautionThresholdHours"]);

            this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            this._CreatedBy = DataObject.getString(row["CreatedBy"]);
            this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
            this._UpdatedBy = DataObject.getString(row["UpdatedBy"]);

            this._Changed = false;
        }
        #endregion Public Methods

        #region INotifyPropertyChanged Members

        /// <summary>
        /// The property changed.
        /// </summary>
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
            if (trackChangesFlag && this._EnableChangeTracking)
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

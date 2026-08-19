// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationDataExportImportLogDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MigrationDataExportImportLogDO and MigrationImportExportHistoryCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// The migration data export import log collection.
    /// </summary>
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(MigrationDataExportImportLogDO))]
    public class MigrationDataExportImportLogCollection : List<MigrationDataExportImportLogDO>
    {
    }

    /// <summary>
    /// The migration data export import log do.
    /// </summary>
    [XmlType("MigrationDataExportImportLogDO")]
    [DataContract]
    [Serializable]
    public class MigrationDataExportImportLogDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data Members

        /// <summary>
        /// Boolean value indicating whether or not this data object has been changed.
        /// </summary>
        private bool changed = false;

        /// <summary>
        /// An identifier use to indicate the type of migration activity that took place.
        /// </summary>
        private string activityId = string.Empty;

        /// <summary>
        /// Description of the migration activity that took place.
        /// </summary>
        private string activityDescription = string.Empty;

        /// <summary>
        /// The status of the migration activity that took place.
        /// </summary>
        private string activityStatus = string.Empty;

        /// <summary>
        /// The name of the user who performed the migration activity.
        /// </summary>
        private string performedBy = string.Empty;

        /// <summary>
        /// Client IP address where the migration activity originated.
        /// </summary>
        private string clientIPAddress = string.Empty;

        #endregion Data Members

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether changed.
        /// </summary>
        [DataMember]
        public bool Changed
        {
            get
            {
                return this.changed;
            }

            set
            {
                if (value == this.changed)
                {
                    return;
                }

                this.changed = value;

                this.RaisePropertyChanged("Changed", false);
            }
        }

        /// <summary>
        /// Gets or sets the activity id.
        /// </summary>
        [DataMember]
        public string ActivityId
        {
            get
            {
                return this.activityId;
            }

            set
            {
                if (value == this.activityId)
                {
                    return;
                }

                this.SetString("ActivityID", 30, value, ref this.activityId);

                this.RaisePropertyChanged("ActivityID");
            }
        }

        /// <summary>
        /// Gets or sets the activity description.
        /// </summary>
        [DataMember]
        public string ActivityDescription
        {
            get
            {
                return this.activityDescription;
            }

            set
            {
                if (value == this.activityDescription)
                {
                    return;
                }

                this.SetString("ActivityDescription", 256, value, ref this.activityDescription);

                this.RaisePropertyChanged("ActivityDescription");
            }
        }

        /// <summary>
        /// Gets or sets the activity status.
        /// </summary>
        [DataMember]
        public string ActivityStatus
        {
            get
            {
                return this.activityStatus;
            }

            set
            {
                if (value == this.activityStatus)
                {
                    return;
                }

                this.SetString("ActivityStatus", 100, value, ref this.activityStatus);

                this.RaisePropertyChanged("ActivityStatus");
            }
        }

        /// <summary>
        /// Gets or sets the performed by.
        /// </summary>
        [DataMember]
        public string PerformedBy
        {
            get
            {
                return this.performedBy;
            }

            set
            {
                if (value == this.performedBy)
                {
                    return;
                }

                this.SetString("PerformedBy", 100, value, ref this.performedBy);

                this.RaisePropertyChanged("PerformedBy");
            }
        }

        /// <summary>
        /// Gets or sets the client IP address.
        /// </summary>
        [DataMember]
        public string ClientIPAddress
        {
            get
            {
                return this.clientIPAddress;
            }

            set
            {
                if (value == this.clientIPAddress)
                {
                    return;
                }

                this.SetString("ClientIPAddress", 50, value, ref this.clientIPAddress);

                this.RaisePropertyChanged("ClientIPAddress");
            }
        }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationDataExportImportLogDO"/> class. 
        /// </summary>
        public MigrationDataExportImportLogDO()
            : base()
        {
            this.Reset();
        }
        #endregion Constructors

        #region Public methods

        /// <summary>
        /// Resets the instance to default values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.changed = false;

            this._IdentityGuid = Guid.NewGuid();

            this.activityId = string.Empty;
            this.activityDescription = string.Empty;
            this.activityStatus = string.Empty;
            this.performedBy = string.Empty;

            this.clientIPAddress = string.Empty;

            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
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
            if (trackChangesFlag)
            {
                this.changed = true;
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

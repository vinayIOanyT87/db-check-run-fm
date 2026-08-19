// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncSessionDetailDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncSessionScopeLogCollection type.
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
    [KnownType(typeof(SyncSessionScopeLogDO))]
    public class SyncSessionScopeLogCollection : List<SyncSessionScopeLogDO>
    {
    }

    [XmlType("SyncSessionDetail")]
    [DataContract]
    [Serializable]
    [KnownType(typeof(SYNCSITETYPE))]
    [KnownType(typeof(SYNCSESSIONSTATUS))]
    [KnownType(typeof(SYNCSESSIONSTATE))]
    public class SyncSessionScopeLogDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data Members

        /// <summary>
        /// The changed flag.
        /// </summary>
        private bool changed = false;

        /// <summary>
        /// The sync session Log GUID.
        /// </summary>
        private Guid syncSessionLogGuid = Guid.Empty;

        /// <summary>
        /// The sync session scope ID
        /// </summary>
        private string syncScopeID = string.Empty;

        /// <summary>
        /// The site type index.
        /// </summary>
        private SYNCSITETYPE? siteTypeIndex = null;

        /// <summary>
        /// The sync session status index.
        /// </summary>
        private SYNCSESSIONSTATUS syncSessionStatusIndex = SYNCSESSIONSTATUS.NEW;

        /// <summary>
        /// The sync session state index.
        /// </summary>
        private SYNCSESSIONSTATE syncSessionStateIndex = SYNCSESSIONSTATE.INIT;

        /// <summary>
        /// The start date.
        /// </summary>
        private DateTimeOffset? startDate = DateTimeOffset.MinValue;

        /// <summary>
        /// The end date.
        /// </summary>
        private DateTimeOffset? endDate = null;

        /// <summary>
        /// The table count.
        /// </summary>
        private int tableCount = 0;

        /// <summary>
        /// The table success count.
        /// </summary>
        private int tableSuccessCount = 0;

        /// <summary>
        /// The table error count.
        /// </summary>
        private int tableErrorCount = 0;

        /// <summary>
        /// The total changes count.
        /// </summary>
        private int totalChangesCount = 0;

        /// <summary>
        /// The total changes applied count.
        /// </summary>
        private int totalChangesAppliedCount = 0;

        /// <summary>
        /// The total changes failed count.
        /// </summary>
        private int totalChangesFailedCount = 0;

        /// <summary>
        /// The total changes pending count.
        /// </summary>
        private int totalChangesPendingCount = 0;

        /// <summary>
        /// The total delete count.
        /// </summary>
        private int totalDeleteCount = 0;

        /// <summary>
        /// The total insert count.
        /// </summary>
        private int totalInsertCount = 0;

        /// <summary>
        /// The total update count.
        /// </summary>
        private int totalUpdateCount = 0;

        /// <summary>
        /// The batch file name.
        /// </summary>
        private string batchFileName = string.Empty;
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
        /// Gets or sets the sync session log guid.
        /// </summary>
        [DataMember]
        public Guid SyncSessionLogGuid
        {
            get
            {
                return this.syncSessionLogGuid;
            }

            set
            {
                if (value == this.syncSessionLogGuid)
                {
                    return;
                }

                this.syncSessionLogGuid = value;

                this.RaisePropertyChanged("SyncSessionLogGuid");
            }
        }

        /// <summary>
        /// Gets or sets the sync session scope ID
        /// </summary>
        [DataMember]
        public string SyncScopeID
        {
            get
            {
                return this.syncScopeID;
            }

            set
            {
                if (value == this.syncScopeID)
                {
                    return;
                }

                this.SetString("SyncScopeID", 80, value, ref this.syncScopeID);

                this.RaisePropertyChanged("SyncScopeID");
            }
        }

        /// <summary>
        /// Gets or sets the site type index.
        /// </summary>
        [DataMember]
        public SYNCSITETYPE? SiteTypeIndex
        {
            get
            {
                return this.siteTypeIndex;
            }

            set
            {
                if (value == this.siteTypeIndex)
                {
                    return;
                }

                this.siteTypeIndex = value;

                this.RaisePropertyChanged("SiteTypeIndex");
            }
        }

        /// <summary>
        /// Gets or sets the sync session status index.
        /// </summary>
        [DataMember]
        public SYNCSESSIONSTATUS SyncSessionStatusIndex
        {
            get
            {
                return this.syncSessionStatusIndex;
            }

            set
            {
                if (value == this.syncSessionStatusIndex)
                {
                    return;
                }

                this.syncSessionStatusIndex = value;

                this.RaisePropertyChanged("SyncSessionStatusIndex");
            }
        }

        /// <summary>
        /// Gets or sets the sync session state index.
        /// </summary>
        [DataMember]
        public SYNCSESSIONSTATE SyncSessionStateIndex
        {
            get
            {
                return this.syncSessionStateIndex;
            }

            set
            {
                if (value == this.syncSessionStateIndex)
                {
                    return;
                }

                this.syncSessionStateIndex = value;

                this.RaisePropertyChanged("SyncSessionStateIndex");
            }
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        [DataMember]
        public DateTimeOffset? StartDate
        {
            get
            {
                return this.startDate;
            }

            set
            {
                if (value == this.startDate)
                {
                    return;
                }

                this.startDate = value;

                this.RaisePropertyChanged("StartDate");
            }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [DataMember]
        public DateTimeOffset? EndDate
        {
            get
            {
                return this.endDate;
            }

            set
            {
                if (value == this.endDate)
                {
                    return;
                }

                this.endDate = value;

                this.RaisePropertyChanged("EndDate");
            }
        }

        /// <summary>
        /// Gets or sets the TableCount
        /// </summary>
        [DataMember]
        public int TableCount
        {
            get
            {
                return this.tableCount;
            }

            set
            {
                if (value == this.tableCount)
                {
                    return;
                }

                this.tableCount = value;

                this.RaisePropertyChanged("TableCount");
            }
        }

        /// <summary>
        /// Gets or sets the TableSuccessCount
        /// </summary>
        [DataMember]
        public int TableSuccessCount
        {
            get
            {
                return this.tableSuccessCount;
            }

            set
            {
                if (value == this.tableSuccessCount)
                {
                    return;
                }

                this.tableSuccessCount = value;

                this.RaisePropertyChanged("TableSuccessCount");
            }
        }

        /// <summary>
        /// Gets or sets the TableErrorCount
        /// </summary>
        [DataMember]
        public int TableErrorCount
        {
            get
            {
                return this.tableErrorCount;
            }

            set
            {
                if (value == this.tableErrorCount)
                {
                    return;
                }

                this.tableErrorCount = value;

                this.RaisePropertyChanged("TableErrorCount");
            }
        }

        /// <summary>
        /// Gets or sets the totalChangesCount
        /// </summary>
        [DataMember]
        public int TotalChangesCount
        {
            get
            {
                return this.totalChangesCount;
            }

            set
            {
                if (value == this.totalChangesCount)
                {
                    return;
                }

                this.totalChangesCount = value;

                this.RaisePropertyChanged("TotalChangesCount");
            }
        }

        /// <summary>
        /// Gets or sets the totalChangesAppliedCount
        /// </summary>
        [DataMember]
        public int TotalChangesAppliedCount
        {
            get
            {
                return this.totalChangesAppliedCount;
            }

            set
            {
                if (value == this.totalChangesAppliedCount)
                {
                    return;
                }

                this.totalChangesAppliedCount = value;

                this.RaisePropertyChanged("TotalChangesAppliedCount");
            }
        }

        /// <summary>
        /// Gets or sets the totalChangesFailedCount
        /// </summary>
        [DataMember]
        public int TotalChangesFailedCount
        {
            get
            {
                return this.totalChangesFailedCount;
            }

            set
            {
                if (value == this.totalChangesFailedCount)
                {
                    return;
                }

                this.totalChangesFailedCount = value;

                this.RaisePropertyChanged("TotalChangesFailedCount");
            }
        }

        /// <summary>
        /// Gets or sets the totalChangesPendingCount
        /// </summary>
        [DataMember]
        public int TotalChangesPendingCount
        {
            get
            {
                return this.totalChangesPendingCount;
            }

            set
            {
                if (value == this.totalChangesPendingCount)
                {
                    return;
                }

                this.totalChangesPendingCount = value;

                this.RaisePropertyChanged("TotalChangesPendingCount");
            }
        }

        /// <summary>
        /// Gets or sets the TotalDeleteCount
        /// </summary>
        [DataMember]
        public int TotalDeleteCount
        {
            get
            {
                return this.totalDeleteCount;
            }

            set
            {
                if (value == this.totalDeleteCount)
                {
                    return;
                }

                this.totalDeleteCount = value;

                this.RaisePropertyChanged("TotalDeleteCount");
            }
        }

        /// <summary>
        /// Gets or sets the TotalInsertCount
        /// </summary>
        [DataMember]
        public int TotalInsertCount
        {
            get
            {
                return this.totalInsertCount;
            }

            set
            {
				if (value == this.totalInsertCount)
                {
                    return;
                }

				this.totalInsertCount = value;

                this.RaisePropertyChanged("TotalInsertCount");
            }
        }

        /// <summary>
        /// Gets or sets the TotalUpdateCount
        /// </summary>
        [DataMember]
        public int TotalUpdateCount
        {
            get
            {
                return this.totalUpdateCount;
            }

            set
            {
                if (value == this.totalUpdateCount)
                {
                    return;
                }

                this.totalUpdateCount = value;

                this.RaisePropertyChanged("TotalUpdateCount");
            }
        }

        /// <summary>
        /// Gets or sets the BatchFileName
        /// </summary>
        [DataMember]
        public string BatchFileName
        {
            get
            {
                return this.batchFileName;
            }

            set
            {
                if (value == this.batchFileName)
                {
                    return;
                }

                this.batchFileName = value;

                this.RaisePropertyChanged("BatchFileName");
            }
        }

        #endregion Properties

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncSessionScopeLogDO"/> class. 
        /// </summary>
        public SyncSessionScopeLogDO()
            : base()
        {
            this.Reset();
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// This method resets the current instance back to default values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.changed = false;

            this._IdentityGuid = Guid.NewGuid();

            this.syncSessionLogGuid = Guid.Empty;

            this._SiteGuid = Guid.Empty;
            this.syncScopeID = string.Empty;

            this.siteTypeIndex = null;
            this.syncSessionStatusIndex = SYNCSESSIONSTATUS.NEW;
            this.syncSessionStateIndex = SYNCSESSIONSTATE.INIT;

            this.startDate = null;
            this.endDate = null;

            this.tableCount = 0;
            this.tableSuccessCount = 0;
            this.tableErrorCount = 0;
            this.totalChangesCount = 0;
            this.totalChangesAppliedCount = 0;
            this.totalChangesFailedCount = 0;
            this.totalChangesPendingCount = 0;
            this.totalDeleteCount = 0;
            this.totalInsertCount = 0;
            this.totalUpdateCount = 0;
            this.batchFileName = string.Empty;

            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
        }

        /// <summary>
        /// This method populates the current instance with the passed in data.
        /// </summary>
        /// <param name="row">
        /// A <see cref="DataRow"/> instance containing data to populate this data object with.
        /// </param>
        public void Load(DataRow row)
        {
            this._IdentityGuid = DataObject.getGuid(row["SyncSessionScopeLogGuid"]);

            this.SyncSessionLogGuid = DataObject.getGuid(row["SyncSessionLogGuid"]);
            this._SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
            this.syncScopeID = DataObject.getString(row["ScopeID"]);

            long? indexValue = null;

            if (!DataObject.isNull(row["SiteTypeIndex"]))
            {
                indexValue = DataObject.getValue<long>(row["SiteTypeIndex"], (long)SYNCSITETYPE.REFERENCE);

                SYNCSITETYPE syncSiteType = SYNCSITETYPE.REFERENCE;
                if (Enum.TryParse(indexValue.ToString(), true, out syncSiteType))
                {
                    this.SiteTypeIndex = syncSiteType;
                }
            }
            else
            {
                this.SiteTypeIndex = null;
            }

            this.syncSessionStatusIndex = (SYNCSESSIONSTATUS)DataObject.getInt(row["SyncSessionStatusIndex"]);
            this.syncSessionStateIndex = (SYNCSESSIONSTATE)DataObject.getInt(row["SyncSessionStateIndex"]);

            this.startDate = DataObject.getValue<DateTimeOffset?>(row["StartDate"], DateTimeOffset.Now);
            this.endDate = DataObject.getValue<DateTimeOffset?>(row["EndDate"], null);

            this.tableCount = DataObject.getValue<int>(row["TableCount"], 0);
            this.tableSuccessCount = DataObject.getValue<int>(row["TableSuccessCount"], 0);
            this.tableErrorCount = DataObject.getValue<int>(row["TableErrorCount"], 0);
            this.totalChangesCount = DataObject.getValue<int>(row["TotalChangesCount"], 0);
            this.totalChangesAppliedCount = DataObject.getValue<int>(row["TotalChangesAppliedCount"], 0);
            this.totalChangesFailedCount = DataObject.getValue<int>(row["TotalChangesFailedCount"], 0);
            this.totalChangesPendingCount = DataObject.getValue<int>(row["TotalChangesPendingCount"], 0);
            this.totalDeleteCount = DataObject.getValue<int>(row["TotalDeleteCount"], 0);
            this.totalInsertCount = DataObject.getValue<int>(row["TotalInsertCount"], 0);
            this.totalUpdateCount = DataObject.getValue<int>(row["TotalUpdateCount"], 0);
            this.batchFileName = DataObject.getString(row["BatchFileName"]);

            this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
            this._CreatedBy = DataObject.getString(row["CreatedBy"]);
            this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
            this._UpdatedBy = DataObject.getString(row["UpdatedBy"]);

            this.changed = false;
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

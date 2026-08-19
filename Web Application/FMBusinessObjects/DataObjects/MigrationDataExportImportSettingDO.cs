// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationDataExportImportSettingDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MigrationDataExportImportSettingDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The migration data export import setting do.
    /// </summary>
    [DataContract]
    [Serializable]
    public class MigrationDataExportImportSettingDO : BaseDataObject
    {
        #region Private data members

        /// <summary>
        /// The alarm and event source name.
        /// </summary>
        private string alarmAndEventSourceName;

        /// <summary>
        /// The export archive path.
        /// </summary>
        private string exportArchivePath;

        /// <summary>
        /// The import archive path.
        /// </summary>
        private string importArchivePath;

        /// <summary>
        /// The selected site GUID.
        /// </summary>
        private Guid selectedSiteGuid;

        #endregion Private data members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationDataExportImportSettingDO"/> class.
        /// </summary>
        public MigrationDataExportImportSettingDO()
        {
            this.Init();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the alarm and event source name.
        /// </summary>
        [DataMember]
        public string AlarmAndEventSourceName
        {
            get { return this.alarmAndEventSourceName; }
            set { this.alarmAndEventSourceName = value; }
        }

        /// <summary>
        /// Gets or sets the export archive path.
        /// </summary>
        [DataMember]
        public string ExportArchivePath
        {
            get { return this.exportArchivePath; }
            set { this.exportArchivePath = value; }
        }

        /// <summary>
        /// Gets or sets the import archive path.
        /// </summary>
        [DataMember]
        public string ImportArchivePath
        {
            get { return this.importArchivePath; }
            set { this.importArchivePath = value; }
        }

        /// <summary>
        /// Gets or sets the selected site GUID.
        /// </summary>
        [DataMember]
        public Guid SelectedSiteGuid
        {
            get { return this.selectedSiteGuid; }
            set { this.selectedSiteGuid = value; }
        }

        #endregion Properties

        #region Private Initialization Methods

        /// <summary>
        /// Method to initialize the object instance back to default values.
        /// </summary>
        private void Init()
        {
            this.alarmAndEventSourceName = string.Empty;
            this.exportArchivePath = string.Empty;
            this.importArchivePath = string.Empty;
            this.selectedSiteGuid = Guid.Empty;
        }
        #endregion Private Initialization Methods
    }
}

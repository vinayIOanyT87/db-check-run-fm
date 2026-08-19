// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportExportBase.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ImportExportBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard.InternalClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class ImportExportBase
    {
        #region Attributes

        private string _ServerName = string.Empty;

        private string _DatabaseName = string.Empty;

        private string _ContentType = string.Empty;

        private DateTime _TimeStamp = DateTime.Now;
        #endregion Attributes

        #region Static Properties
        #endregion Static Properties

        #region Constructors
        public ImportExportBase(string serverName, string databaseName, string contentType)
        {
            this._ServerName = serverName;
            this._DatabaseName = databaseName;
            this._ContentType = contentType;
        }
        #endregion Constructors

        #region Properties
        public string ServerName
        {
            get
            {
                return this._ServerName;
            }
        }
        #endregion Properties

        #region Static Methods
        #endregion Static Methods

        #region Public Methods
        public string GetFullyQualifiedTableName(string schemaName, string tableName)
        {
            return string.Format("{0}.{1}.{2}", this._DatabaseName, schemaName, tableName);
        }
        #endregion Public Methods

        #region Protected abstract methods
        protected string GetFilename(string siteId, string tableName)
        {
            return string.Format("{0}_{1}_{2}_{3}.data", siteId, this._ContentType, this._TimeStamp.ToString("mmDDYYYY_HHMM"), tableName);
        }
        #endregion Protected abstract methods

        #region Private Methods
        #endregion Private Methods
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncTableDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncTableCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml.Serialization;

    using FMBusinessObjects.UtilityObjects;

    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SyncTableDO))]
    public class SyncTableCollection : List<SyncTableDO>
    {
    }

    [XmlType("SyncTable")]
    [Serializable]
    [DataContract]
    public class SyncTableDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Attributes
        private bool _Changed = false;
        private string _TableName = string.Empty;
        private Guid _SyncDependencyGroupGuid = Guid.Empty;
        private DateTimeOffset? _LastSchemaDate;
        private bool _IsSiteFilteredFlag = true;
        private bool _IsSiteFilteredOnDeleteFlag = true;
	    private Guid? _ParentSyncTableGuid = null;
	    private string _ParentForeignKeyColumnName = null;
        #endregion Attributes

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
        public Guid SyncDependencyGroupGuid
        {
            get { return (_SyncDependencyGroupGuid); }
            set
            {
                if (value == _SyncDependencyGroupGuid)
                    return;

                _SyncDependencyGroupGuid = value;

                RaisePropertyChanged("SyncDependencyGroupGuid");
            }
        }
        [DataMember]
        public DateTimeOffset? LastSchemaDate
        {
            get { return (_LastSchemaDate); }
            set
            {
                if (value == _LastSchemaDate)
                    return;

                _LastSchemaDate = value;

                RaisePropertyChanged("LastSchemaDate");
            }
        }
        [DataMember]
        public string TableName
        {
            get { return (_TableName); }
            set
            {
                if (value == _TableName)
                    return;

                SetString("TableName", 1024, value, ref _TableName);

                RaisePropertyChanged("TableName");
            }
        }
        [DataMember]
        public bool IsSiteFilteredFlag
        {
            get { return (_IsSiteFilteredFlag); }
            set
            {
                if (value == _IsSiteFilteredFlag)
                    return;

                _IsSiteFilteredFlag = value;

                RaisePropertyChanged("IsSiteFilteredFlag");
            }
        }
        [DataMember]
        public bool IsSiteFilteredOnDeleteFlag
        {
            get { return (_IsSiteFilteredOnDeleteFlag); }
            set
            {
                if (value == _IsSiteFilteredOnDeleteFlag)
                    return;

                _IsSiteFilteredOnDeleteFlag = value;

                RaisePropertyChanged("IsSiteFilteredOnDeleteFlag");
            }
        }
		[DataMember]
		public Guid? ParentSyncTableGuid
		{
			get { return (_ParentSyncTableGuid); }
			set
			{
				if (value == _ParentSyncTableGuid)
					return;

				_ParentSyncTableGuid = value;

				RaisePropertyChanged("ParentSyncTableGuid");
			}
		}
		public string ParentForeignKeyColumnName
		{
			get { return (_ParentForeignKeyColumnName); }
			set
			{
				if (value == _ParentForeignKeyColumnName)
					return;

				SetString("ParentForeignKeyColumnName", 512, value, ref _ParentForeignKeyColumnName);

				RaisePropertyChanged("ParentForeignKeyColumnName");
			}
		}
		#endregion Properties

        #region Constructors
        public SyncTableDO()
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
            this._TableName = string.Empty;
            this._SyncDependencyGroupGuid = Guid.Empty;
            this._LastSchemaDate = null;
            this._IsSiteFilteredFlag = true;
            this._IsSiteFilteredOnDeleteFlag = true;
	        this._ParentSyncTableGuid = null;
	        this._ParentForeignKeyColumnName = null;
            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
        }

        public void Load(DataSet Set)
        {
            if (Set == null)
            {
                throw new ArgumentNullException("Set");
            }

            this.Reset();

            if (Set.Tables.Count == 0)
                return;

            DataTable Table = Set.Tables[0];

            if (Table == null || Table.Rows.Count == 0)
                return;

            DataRow Row = Table.Rows[0];

            this._IdentityGuid = DataObject.getValue<Guid>(Row["SyncTableGuid"], Guid.NewGuid());
            this._TableName = DataObject.getValue<string>(Row["TableName"], this._TableName);
            this._SyncDependencyGroupGuid = DataObject.getValue<Guid>(Row["SyncDependencyGroupGuid"], Guid.NewGuid());
            this._LastSchemaDate = DataObject.getOptionalDateTimeOffset(Row["LastSchemaDate"]);
            this._IsSiteFilteredFlag = DataObject.getValue<bool>(Row["IsSiteFilteredFlag"], true);
            this._IsSiteFilteredOnDeleteFlag = DataObject.getValue<bool>(Row["IsSiteFilteredOnDeleteFlag"], true);
			this._ParentSyncTableGuid = DataObject.getValue<Guid?>(Row["ParentSyncTableGuid"], null);
			this._ParentForeignKeyColumnName = DataObject.getString(Row["ParentForeignKeyColumnName"]);

            this._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
            this._CreatedBy = DataObject.getString(Row["CreatedBy"]);
            this._UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], this._CreatedDate);
            this._UpdatedBy = DataObject.getString(Row["UpdatedBy"]);

            this.RaiseMultiplePropertyChanged("IdentityGuid", "CreatedDate");

            this._Changed = false;
        }

        public string GetFormattedTableName(TABLENAMEFORMAT pFormatOption)
        {
            StringBuilder formattedName = new StringBuilder();

            string serverName = "";
            string databaseName = "";
            string schemaName = "";
            string objectName = "";

            if (!string.IsNullOrEmpty(_TableName))
            {
                // If there is at least 1 period, we're guaranteed to have at least 2 entries (might be blank, but the [0] and [1] index is valid)
                if (_TableName.Contains("."))
                {
                    string[] tableNameParts = _TableName.Split(new char[] { '.' }, StringSplitOptions.None);

                    // At most, we could end up with server.database.schema.objectname (any of these could be blank
                    // but we used the Split option to return even blank entries in the place holders)
                    // 
                    for (int i = tableNameParts.Length; i > 0; i--)
                    {
                        switch (i)
                        {
                            case 4:
                                serverName = tableNameParts[tableNameParts.Length - i];

                                if (pFormatOption == TABLENAMEFORMAT.FULLY_QUALIFIED)
                                {
                                    formattedName.AppendIffDelimited(serverName, ".");
                                }
                                break;
                            case 3:
                                databaseName = tableNameParts[tableNameParts.Length - i];

                                if ((int)pFormatOption >= (int)TABLENAMEFORMAT.TABLENAME_SCHEMA_DATABASE)
                                {
                                    formattedName.AppendIffDelimited(databaseName, ".");
                                }
                                break;
                            case 2:
                                schemaName = tableNameParts[tableNameParts.Length - i];

                                if ((int)pFormatOption >= (int)TABLENAMEFORMAT.TABLENAME_SCHEMA)
                                {
                                    formattedName.AppendIffDelimited(schemaName, ".");
                                }
                                break;
                            case 1:
                                objectName = tableNameParts[tableNameParts.Length - i];

                                if ((int)pFormatOption >= (int)TABLENAMEFORMAT.TABLENAME)
                                {
                                    formattedName.AppendIffDelimited(objectName, ".");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (formattedName.ToString());
        }
        #endregion Public Methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #region STANDARD RAISE PROPERTY CHANGE NOTIFICATION METHODS FOR DERIVED CLASSES
        protected void RaisePropertyChanged(string pPropertyName)
        {
            RaisePropertyChanged(pPropertyName, true);
        }
        protected void RaisePropertyChanged(string pPropertyName, bool pTrackChangesFlag)
        {
            if (pTrackChangesFlag)
                _Changed = true;

            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
            }
        }
        protected void RaiseMultiplePropertyChanged(params string[] pPropertyNames)
        {
            foreach (var each in pPropertyNames)
            {
                RaisePropertyChanged(each);
            }
        }
        #endregion STANDARD RAISE PROPERTY CHANGE NOTIFICATION METHODS FOR DERIVED CLASSES

        #endregion INotifyPropertyChanged Members
    }
}

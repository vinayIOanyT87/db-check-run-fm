// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaChangeDetailDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaChangeDetailDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// The schemaobjecttype.
    /// </summary>
    public enum SCHEMAOBJECTTYPE : long
    {
        /// <summary>
        /// Not Selected
        /// </summary>
        None = 0,

        /// <summary>
        /// Database
        /// </summary>
        Database = 1,

        /// <summary>
        /// The schema.
        /// </summary>
        Schema = 2,

        /// <summary>
        /// The table.
        /// </summary>
        Table = 3,

        /// <summary>
        /// The view.
        /// </summary>
        View = 4,

        /// <summary>
        /// The trigger.
        /// </summary>
        Trigger = 5,

        /// <summary>
        /// The stored procedure.
        /// </summary>
        StoredProcedure = 6,

        /// <summary>
        /// The function.
        /// </summary>
        Function = 7,

        /// <summary>
        /// The constraint default.
        /// </summary>
        ConstraintDefault = 8,

        /// <summary>
        /// The constraint foreign key.
        /// </summary>
        ConstraintFk = 9,

        /// <summary>
        /// The index.
        /// </summary>
        Index = 10
    }

    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SyncSessionLogDO))]
    public class SchemaChangeDetailCollection : List<SchemaChangeDetailDO>
    {
    }

    /// <summary>
    /// The schema version history.
    /// </summary>
    [XmlType("SchemaChangeDetail")]
    [DataContract]
    [Serializable]
    public class SchemaChangeDetailDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data Members

        /// <summary>
        /// The _ changed.
        /// </summary>
        private bool _Changed = false;

        /// <summary>
        /// The _ schema change history guid.
        /// </summary>
        private Guid _SchemaChangeHistoryGuid = Guid.Empty;

        /// <summary>
        /// The _ schema object type index.
        /// </summary>
        private SCHEMAOBJECTTYPE _SchemaObjectTypeIndex = SCHEMAOBJECTTYPE.Table;

        /// <summary>
        /// The _ schema name.
        /// </summary>
        private string _SchemaName = string.Empty;

        /// <summary>
        /// The _ object name.
        /// </summary>
        private string _ObjectName = string.Empty;
        #endregion Data Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaChangeDetailDO"/> class. 
        /// </summary>
        public SchemaChangeDetailDO()
            : base()
        {
            this.Reset();
        }
        #endregion Constructors

        #region Properties

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
            }
        }

        /// <summary>
        /// Gets or sets the schema change history guid
        /// </summary>
        [DataMember]
        public Guid SchemaChangeHistoryGuid
        {
            get
            {
                return this._SchemaChangeHistoryGuid;
            }

            set
            {
                if (value == this._SchemaChangeHistoryGuid)
                {
                    return;
                }

                this._SchemaChangeHistoryGuid = value;

                this.RaisePropertyChanged("SchemaChangeHistoryGuid");
            }
        }

        /// <summary>
        /// Gets or sets the schema object type index.
        /// </summary>
        [DataMember]
        public SCHEMAOBJECTTYPE SchemaObjectTypeIndex
        {
            get
            {
                return this._SchemaObjectTypeIndex;
            }

            set
            {
                if (value == this._SchemaObjectTypeIndex)
                {
                    return;
                }

                this._SchemaObjectTypeIndex = value;

                this.RaisePropertyChanged("SchemaObjectTypeIndex");
            }
        }

        /// <summary>
        /// Gets or sets the schema name.
        /// </summary>
        [DataMember]
        public string SchemaName
        {
            get
            {
                return this._SchemaName;
            }

            set
            {
                if (value == this._SchemaName)
                {
                    return;
                }

                this.SetString("SchemaName", 32, value, ref this._SchemaName);

                this.RaisePropertyChanged("SchemaName");
            }
        }

        /// <summary>
        /// Gets or sets the object name.
        /// </summary>
        [DataMember]
        public string ObjectName
        {
            get
            {
                return this._ObjectName;
            }

            set
            {
                if (value == this._ObjectName)
                {
                    return;
                }

                this.SetString("ObjectName", 256, value, ref this._ObjectName);

                this.RaisePropertyChanged("ObjectName");
            }
        }
        #endregion Properties

        #region Public methods
        public override void Reset()
        {
            base.Reset();
            this._Changed = false;

            this._IdentityGuid = Guid.NewGuid();

            this._SchemaChangeHistoryGuid = Guid.Empty;
            this._SchemaObjectTypeIndex = SCHEMAOBJECTTYPE.Table;
            this._SchemaName = string.Empty;
            this._ObjectName = string.Empty;

            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
        }
        #endregion Public methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #region STANDARD RAISE PROPERTY CHANGE NOTIFICATION METHODS FOR DERIVED CLASSES
        protected void RaisePropertyChanged(string propertyName)
        {
            this.RaisePropertyChanged(propertyName, true);
        }
        protected void RaisePropertyChanged(string propertyName, bool trackChangesFlag)
        {
            if (trackChangesFlag)
                this._Changed = true;

            if (null != this.PropertyChanged)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
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
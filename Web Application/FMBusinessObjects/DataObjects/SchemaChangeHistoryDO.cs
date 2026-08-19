// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaChangeHistoryDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaChangeHistoryDO type.
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
    public class SchemaChangeHistoryCollection : List<SchemaChangeHistoryDO>
    {
    }

    /// <summary>
    /// The schema version history.
    /// </summary>
    [XmlType("SchemaChangeHistory")]
    [DataContract]
    [Serializable]
    public class SchemaChangeHistoryDO : BaseDataObject, INotifyPropertyChanged
    {
        #region Data Members

        /// <summary>
        /// The _ changed.
        /// </summary>
        private bool _Changed = false;

        /// <summary>
        /// Version
        /// </summary>
        private string _Version = string.Empty;

        /// <summary>
        /// The _ has schema change flag.
        /// </summary>
        private bool _HasSchemaChangeFlag = false;

        #endregion Data Members

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaChangeHistoryDO"/> class. 
        /// </summary>
        public SchemaChangeHistoryDO()
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
        /// Gets or sets the schema name.
        /// </summary>
        [DataMember]
        public string Version
        {
            get
            {
                return this._Version;
            }

            set
            {
                if (value == this._Version)
                {
                    return;
                }

                this.SetString("Version", 40, value, ref this._Version);

                this.RaisePropertyChanged("Version");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this history entry had associated schema change records.
        /// </summary>
        [DataMember]
        public bool HasSchemaChangeFlag
        {
            get
            {
                return this._HasSchemaChangeFlag;
            }

            set
            {
                if (value == this._HasSchemaChangeFlag)
                {
                    return;
                }

                this._HasSchemaChangeFlag = value;
            }
        }

        #endregion Properties

        #region Public methods

        /// <summary>
        /// The reset.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this._Changed = false;

            this._IdentityGuid = Guid.NewGuid();
            this._Version = string.Empty;

            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
        }
        #endregion Public methods

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
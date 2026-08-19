// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the VersionDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using FMBusinessObjects.UtilityObjects;

    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(VersionDO))]
    public class VersionCollection : List<VersionDO>
    {
    }

    [XmlType("VersionDO")]
    [DataContract]
    [Serializable]
    public class VersionDO : BaseDataObject
    {
        #region Attributes

        /// <summary>
        /// The _ version index.
        /// </summary>
        private int? _VersionIndex = null;

        /// <summary>
        /// The _ version.
        /// </summary>
        private string _Version = string.Empty;

        /// <summary>
        /// The _ package name.
        /// </summary>
        private string _PackageName = string.Empty;

        /// <summary>
        /// The _ date applied.
        /// </summary>
        private DateTimeOffset? _DateApplied = null;

        /// <summary>
        /// The _ comments.
        /// </summary>
        private string _Comments = string.Empty;

        /// <summary>
        /// The _ check 1.
        /// </summary>
        private long _Check1 = 0;

        /// <summary>
        /// The _ check 2.
        /// </summary>
        private long _Check2 = 0;

        /// <summary>
        /// The _ sync completed flag.
        /// </summary>
        private bool _SyncCompletedFlag = false;

        /// <summary>
        /// The _ row version snapshot.
        /// </summary>
        private byte[] _RowVersionSnapshot = null;

        #endregion Attributes

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionDO"/> class.
        /// </summary>
        public VersionDO()
        {
            this.Reset();
        }
        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the version index.
        /// </summary>
        [DataMember]
        public int? VersionIndex
        {
            get
            {
                return (this._VersionIndex);
            }

            set
            {
                this._VersionIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the version.
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
                this.SetString("Version", 16, value, ref this._Version);
            }
        }

        /// <summary>
        /// Gets or sets the package name.
        /// </summary>
        [DataMember]
        public string PackageName
        {
            get
            {
                return this._PackageName;
            }

            set
            {
                this.SetString("PackageName", 16, value, ref this._PackageName);
            }
        }

        /// <summary>
        /// Gets or sets the date applied.
        /// </summary>
        [DataMember]
        public DateTimeOffset? DateApplied
        {
            get
            {
                return this._DateApplied;
            }

            set
            {
                this._DateApplied = value;
            }
        }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        [DataMember]
        public string Comments
        {
            get
            {
                return this._Comments;
            }

            set
            {
                this.SetString("Comments", 2000, value, ref this._Comments);
            }
        }

        /// <summary>
        /// Gets or sets the check 1.
        /// </summary>
        [DataMember]
        public long Check1
        {
            get
            {
                return this._Check1;
            }

            set
            {
                this._Check1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the check 2.
        /// </summary>
        [DataMember]
        public long Check2
        {
            get
            {
                return this._Check2;
            }

            set
            {
                this._Check2 = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether sync completed flag.
        /// </summary>
        [DataMember]
        public bool SyncCompletedFlag
        {
            get
            {
                return this._SyncCompletedFlag;
            }

            set
            {
                this._SyncCompletedFlag = value;
            }
        }

        /// <summary>
        /// Gets or sets the row version snapshot.
        /// </summary>
        [DataMember]
        public byte[] RowVersionSnapshot
        {
            get
            {
                return this._RowVersionSnapshot;
            }

            set
            {
                this._RowVersionSnapshot = value;
            }
        }
        #endregion Properties

        #region Public methods

        /// <summary>
        /// Resets the data object back to default values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this._IdentityGuid = Guid.NewGuid();
            this._VersionIndex = null;
            this._Version = string.Empty;
            this._PackageName = string.Empty;
            this._DateApplied = null;
            this._Comments = string.Empty;
            this._Check1 = 0;
            this._Check2 = 0;
            this._SyncCompletedFlag = false;
            this._RowVersionSnapshot = null;
        }

        /// <summary>
        /// Converts the current Version string to an instance of a <see cref="VersionInfo"/> class.
        /// </summary>
        /// <returns>
        /// An instance of a <see cref="VersionInfo"/> object, populated with the current version number.
        /// </returns>
        public VersionInfo ToVersionInfo()
        {
            if (!string.IsNullOrEmpty(this._Version))
            {
                return VersionInfo.FromString(this._Version);
            }
            else
            {
                return VersionInfo.FromString("0.0.0.0");
            }
        }

        #endregion Public methods
    }
}

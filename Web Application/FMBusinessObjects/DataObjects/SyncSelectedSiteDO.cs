// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncSelectedSiteDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SyncSelectedSiteDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// The sync selected site do.
    /// </summary>
    [XmlType("SyncSelectedSiteDO")]
    [DataContract]
    [Serializable]
    public class SyncSelectedSiteDO : BaseDataObject
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncSelectedSiteDO"/> class. 
        /// </summary>
        public SyncSelectedSiteDO()
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

            this._IdentityGuid = Guid.NewGuid();
            this._CreatedDate = DateTimeOffset.Now;
            this._UpdatedDate = DateTimeOffset.Now;
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="siteID">
        /// The site id.
        /// </param>
        /// <param name="siteGuid">
        /// Optional site GUID value
        /// </param>
        public void Load(string siteID, Guid? siteGuid)
        {
            this._SiteID = siteID;
            this._SiteGuid = siteGuid.HasValue ? siteGuid.Value : Guid.Empty;
        }
        #endregion Public Methods
    }
}

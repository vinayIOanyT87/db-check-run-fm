// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISitesInfo.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Interface definition for the SitesInfo service class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.BusinessInterfaces
{
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Interface definition for the SitesInfo service class
    /// </summary>
    [ServiceContract]
    public interface ISitesInfo
    {
        #region Public Methods and Operators

        /// <summary>
        /// Refreshes the site info.
        /// </summary>
        /// <param name="security">The security object.</param>
        /// <returns>A SiteInfoDO object the latest site information.</returns>
        [OperationContract]
        SiteInfoDO RefreshSiteInfo(SecurityClass security);

        /// <summary>
        /// Resets this instance.
        /// </summary>
        [OperationContract]
        void Reset();

        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatchRequests.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IDispatchRequests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;
    using System.Data;

    /// <summary>
    /// Interface for Dispatch service requests. Primary interface for Dispatch.
    /// </summary>
    [ServiceContract]
    public interface IReportingRequest
    {
        #region Public Methods and Operators

        /// <summary>
        /// Enumerates equipment entities for use in Dispatch.
        /// </summary>
        /// <param name="security">The security object</param>
        /// <param name="topVersion">The top Version</param>
        /// <returns>A dispatch equipment data object</returns>
        [OperationContract]
        DataSet ProcessReport(SecurityClass security, Dictionary<string, string> parameters);

        [OperationContract]
        List<string> GetReportParameters(SecurityClass security, Dictionary<string, string> parameters);
        #endregion
    }
}

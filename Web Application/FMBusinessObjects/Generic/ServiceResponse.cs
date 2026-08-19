// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceResponse.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   A generic return class that can provide the status of a service call in addition
//   to a collection of key results.  If the service call were for a singular event then
//   the collection would contain one entry, but if it the service processed a collection of items, then
//   it could optionally return an individual result for each item indexed by a corresponding key.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.Generic
{
    using System;
    using System.Collections.Generic;

    using FMBusinessObjects.Constants;

    /// <summary>
    /// Class ServiceResponse.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceResponse<T>
    {
        /// <summary>
        /// Gets or sets the call status.
        /// </summary>
        /// <value>The call status.</value>
        public ServiceStatus CallStatus { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public Dictionary<Guid, T> Results { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResponse{T}"/> class.
        /// </summary>
        public ServiceResponse()
        {
            this.Results = new Dictionary<Guid, T>();
            this.CallStatus = ServiceStatus.Idle;
        }
    }
}

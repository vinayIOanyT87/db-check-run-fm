// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWCFService.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Describes a standardized set of service APIs that all WCF Services must provide in order to provide loose coupling 
//   between custom host processes and WCF Services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.BusinessInterfaces
{
    using System.ServiceModel;

    /// <summary>
    /// Provides a standard interface to access service meta data
    /// </summary>
    /// 
    [ServiceContract]
    public interface IWcfService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Name of the Service Class</returns>
        [OperationContract]
        string GetServiceName();
    }
}


namespace FuelsManager.Afss.BusinessObjects.ServiceInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.BusinessInterfaces;

    [ServiceContract]
    [ServiceKnownType(typeof(SecurityClass))]
    public interface IExternalStationServices : IWcfService
    {
        /// <summary>
        /// This method performs the default tasks associated with normal communication sessions with a remote fuel service station.  It controls the sequence of 
        /// operations and activities unique to each type of remote fuel service station.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// A list of external station GUIDs and the results of the work
        /// </returns>
        [OperationContract]
        void DoWork(SecurityClass security);

        /// <summary>
        /// Downloads all new transactions for all configured stations
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// A list of external station GUIDs and the results of the transaction download
        /// </returns>
        [OperationContract]
        Dictionary<Guid, string> DownloadAllNewTransactions(SecurityClass security);

        /// <summary>
        /// Update the configuration data stored at one or more stations
        /// </summary>
        /// <param name="security">
        /// Contains security information
        /// </param>
        /// <param name="externalStationGuids">
        /// Identifies the external stations to download transactions for
        /// </param>
        /// <returns>
        /// A list of external station GUIDs and the results of the download
        /// </returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Dictionary<Guid, object> UpdateStationConfiguration(SecurityClass security, List<Guid> externalStationGuids);
    }
}

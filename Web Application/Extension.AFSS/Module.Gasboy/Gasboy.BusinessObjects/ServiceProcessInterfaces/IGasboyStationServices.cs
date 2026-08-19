namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.ServiceProcessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using System.Threading;
	using System.Threading.Tasks;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Generic;

	using FuelsManager.Afss.BusinessObjects.ServiceInterfaces;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

	[ServiceContract]
	[ServiceKnownType(typeof(SecurityClass))]
	[ServiceKnownType(typeof(GasboyStation))]
	[ServiceKnownType(typeof(GasboyStationProduct))]
	[ServiceKnownType(typeof(IExternalStationServices))]
	public interface IGasboyStationServices : IExternalStationServices
	{
		/// <summary>
		/// Download all new transactions for the Gasboy Stations listed.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStationGuidList"></param>
		/// <returns>
		/// A list of external station GUIDs and the results of the download
		/// </returns>
		[OperationContract]
		Dictionary<Guid, string> DownloadNewTransactionsForStations(SecurityClass security, List<Guid> externalStationGuidList);

		/// <summary>
		/// Download transactions based on the provided range of Transaction IDs
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStationGuid"></param>
		/// <param name="beginTransactionID"></param>
		/// <param name="endTransactionID"></param>
		/// <returns>
		/// The results of the download
		/// </returns>
		[OperationContract]
		string DownloadSelectedTransaction(SecurityClass security, Guid externalStationGuid, long? beginTransactionID, long? endTransactionID);

		/// <summary>
		/// Get any new events from the stations identified by the provided guids
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="externalStationGuids">Identifies the stations to get events for</param>
		/// <returns>The result of the event download for each station</returns>
		[OperationContract]
		Dictionary<Guid, string> GetNewEventsForStations(SecurityClass security, List<Guid> externalStationGuids);

		/// <summary>
		/// Download all new transactions for the Gasboy Stations listed.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="gasboyStationTransactions"></param>
		/// <returns>
		/// A list of Failed Transaction GUIDs and the results of each reprocessing attempt.s
		/// </returns>
		[OperationContract]
		Dictionary<Guid, string> ReprocessFailedTransactions(SecurityClass security, List<GasboyStationTransaction> gasboyStationTransactions);

		/// <summary>
		/// Downloads all gasboy devices to specified Stations
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStationList">One or more Gasboy stations to download the Gasboy Devices to</param>
		/// <returns>
		/// 
		/// </returns>
		[OperationContract]
		Dictionary<Guid, string> SendAllDevicesToStations(SecurityClass security, List<GasboyStation> externalStationList);

		/// <summary>
		/// Download the specified devices to specified Stations
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStationList">One or more Gasboy stations to download the Gasboy Devices to</param>
		/// <param name="gasboyDeviceList">One or more Gasboy Devices to download to the Gasboy Station</param>
		/// <returns>
		/// 
		/// </returns>
		[OperationContract]
		Dictionary<Guid, string> SendSelectedDevicesToStations(SecurityClass security, List<GasboyStation> externalStationList, List<GasboyDevice> gasboyDeviceList);

		/// <summary>
		/// Download the specified devices to specified Stations
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStationGuids">One or more Gasboy stations to download the Gasboy Devices to</param>
		/// <returns>
		/// 
		/// </returns>
		[OperationContract]
		Dictionary<Guid, string> SendBlacklistedDevicesToStations(SecurityClass security, List<Guid> externalStationGuids);

		/// <summary>
		/// Download the specified devices to specified Stations
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStationGuids">One or more Gasboy stations to download the Gasboy Devices to</param>
		/// <returns>
		/// 
		/// </returns>
		[OperationContract]
		Dictionary<Guid, string> AllowBlacklistedDevicesToStations(SecurityClass security, List<Guid> externalStationGuids);


		/// <summary>
		/// Get a list of Products for the specified Stations
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStationList">One or more Gasboy stations to download the product list from</param>
		/// <returns>
		/// 
		/// </returns>
		[OperationContract]
		Dictionary<Guid, List<GasboyStationProduct>> GetProductList(SecurityClass security, List<GasboyStation> externalStationList);

		/// <summary>
		/// Get a list of Products for a single Gasboy Station
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="externalStation">A Gasboy station to download the product list from</param>
		/// <returns>
		/// 
		/// </returns>
		[OperationContract]
		List<GasboyStationProduct> GetStationProductList(SecurityClass security, GasboyStation externalStation);

		/// <summary>Test the connection to the specified Gasboy station</summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStation">The Gasboy station to test the connection for</param>
		/// <returns>The results of the connection test</returns>
		[OperationContract]
		string TestConnection(SecurityClass security, GasboyStation externalStation);

		/// <summary>Test the connection to the specified Gasboy station</summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStations">A list of Gasboy station to test the connection for</param>
		/// <returns>The results of the connection test</returns>
		[OperationContract]
		Dictionary<Guid, string> TestConnections(SecurityClass security, List<GasboyStation> externalStations);

	}
}

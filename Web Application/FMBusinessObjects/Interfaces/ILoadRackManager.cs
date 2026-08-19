using System;
using System.Collections;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.Interfaces
{
	/// <summary>
	/// This is the interface used by LoadRackManager which is defined in LoadRackLibary
	/// Typically, web page would call GetLoadRackManager in FMFormBase.cs which uses .net remoting to get the singleton object
	/// from LoadRackService which is a Windows service.
	/// </summary>
	public interface ILoadRackManager
	{
		SaveTransactionsResultDO AccountingRequest(SaveTransactionsSR sr);
		TransactionDO AccountingRequest(TransactionSR sr);
		void Add(SecurityClass security, System.Type type, Guid identityGuid);
		void DownloadLocalConfigurationToStation(SecurityClass security, Guid stationGuid);
		byte[] GetSignature(SecurityClass security, Guid stationGuid);
		bool GetStationCommunicationsStatus(Guid siteGuid, Guid stationGuid);
		StationClass GetStation(SecurityClass security, Guid stationGuid);
		TransactionDO GetStationTransaction(SecurityClass security, Guid stationGuid);
		void InitiateEndOfDay(SecurityClass security);
		Hashtable GetEndOfDayStatus(SecurityClass security);
		void Modify(SecurityClass security, System.Type type, Guid identityGuid);
		void Purge(SecurityClass security, System.Type type, Guid identityGuid);
		void ResetOwnerAllocations(SecurityClass security);
      void ResetOwnerAllocationsForSingleProduct(SecurityClass security, string productId);

      void SetAdditiveMeterTotalizer(SecurityClass security, Guid stationGuid, Guid loadArmGuid, Guid productGuid, double value);
	}
}

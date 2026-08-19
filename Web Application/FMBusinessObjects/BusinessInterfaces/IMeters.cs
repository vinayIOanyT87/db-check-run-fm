///***************************************************************************
/// Module Name:  IMeters.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// This interface describes the methods which can be used to interact with the meter functionality in FuelsManager
	/// </summary>
	[ServiceContract]
	public interface IMeters
	{
		[OperationContract]
		List<MeterClass> Enumerate(SecurityClass security);

		[OperationContract]
		List<MeterClass> EnumerateAndFilter(SecurityClass security, string meterIDFilterValue);

		[OperationContract]
		List<MeterClass> EnumerateByAssetGuid(SecurityClass security, Guid assetGuid);

		[OperationContract]
		List<MeterClass> EnumerateByAssetGuidAndFilter(SecurityClass security, Guid assetGuid, string meterIDFilterValue);

		[OperationContract]
		List<MeterAssetClass> EnumerateAssets(SecurityClass security);

		[OperationContract]
		List<MeterAssetClass> EnumerateAssetsAndFilter(SecurityClass security, string assetIDFilterValue);

		[OperationContract]
		MeterClass Get(SecurityClass security, Guid identityGuid);

		[OperationContract]
		Guid Add(SecurityClass security, MeterClass meter);

		[OperationContract]
		void Modify(SecurityClass security, MeterClass meter);

		[OperationContract]
		void Purge(SecurityClass security, Guid meterGuid);

		[OperationContract]
		bool HasForeignKeyReference(SecurityClass security, Guid meterGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string id);

		[OperationContract]
		List<string> GetMeterIdsByAssetGuids(SecurityClass security, List<EquipmentClass> assets);

	}
}

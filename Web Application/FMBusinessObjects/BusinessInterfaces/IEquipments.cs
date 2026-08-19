namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IEquipments
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, EquipmentClass equipment);

		[OperationContract]
		EquipmentClass GetBySite(SecurityClass security, Guid equipmentGuid, SiteClass site);

		[OperationContract]
		EquipmentClass Get(SecurityClass security, Guid equipmentGuid);

		[OperationContract]
		EquipmentClass GetBasicInfo(SecurityClass security, Guid equipmentGuid, Guid siteGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string id);

		[OperationContract]
		Guid GetMasterRecordGuid(SecurityClass security, string id);

		[OperationContract]
		EquipmentCollectionClass Enumerate(SecurityClass security, bool hideHiddenEquipmentRecords = false);

		[OperationContract]
		EquipmentCollectionClass EnumerateExt(SecurityClass security,
															Guid equipmentTypeGuid,
															bool managedEquipmentOnly = false,
															bool secondaryStorageOnly = false,
															EQUIPMENT_TYPE equipmentType = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE,
															string filter = null,
															int limit = 0,
															bool hideHiddenEquipmentRecords = false);

		[OperationContract]
		EquipmentCollectionClass EnumerateBySource(SecurityClass security);

		[OperationContract]
		EquipmentCollectionClass EnumerateByManagedFillstand(SecurityClass security);

		[OperationContract]
		DataSet EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(SecurityClass security, EQUIPMENT_TYPE[] types, object companyGuid, object fuelCardGuid, object productGuid, object secondaryStorage);

		[OperationContract]
		EquipmentCollectionClass EnumerateManagedEquipment(SecurityClass security);

		[OperationContract]
		EquipmentCollectionClass EnumerateManagedEquipmentWithoutQualityTag(SecurityClass security);

		[OperationContract]
		EquipmentCollectionClass EnumerateByTypeAndFilterAndProduct(SecurityClass security, EQUIPMENT_TYPE type, string filter, Guid productGuid, bool excludeNonEditableCompanyGuid, bool excludeNonEditableFuelCardGuid, bool hideHiddenEquipmentRecords = false);

		[OperationContract]
		EquipmentCollectionClass EnumerateByTypeAndProduct(SecurityClass security, EQUIPMENT_TYPE type, Guid productGuid, bool excludeNonEditableCompanyGuid, bool excludeNonEditableFuelCardGuid, bool hideHiddenEquipmentRecords = false);

		[OperationContract]
		EquipmentCollectionClass EnumerateByCompany(SecurityClass security, Guid companyGuid, bool hideHiddenEquipmentRecords = false);

		[OperationContract]
		EquipmentCollectionClass EnumerateByCompanyAndLocalize(SecurityClass security, Guid companyGuid, bool localize, bool hideHiddenEquipmentRecords = false);

		[OperationContract]
		EquipmentCollectionClass EnumerateByCompanyAndSite(SecurityClass security, Guid companyGuid, SiteClass site, bool hideHiddenEquipmentRecords = false);

		[OperationContract]
		EquipmentCollectionClass EnumerateByFuelCard(SecurityClass security, Guid fuelCardGuid, bool hideHiddenEquipmentRecords = false);

		[OperationContract]
		DataSet EnumerateDataSet(SecurityClass security,
										bool managedEquipmentOnly,
										bool secondaryStorageOnly,
										Guid equipmentTypeGuid,
										EQUIPMENT_TYPE equipmentType,
										string translatedUnassigned,
										string filter,
										bool isDefense,
										bool hideHiddenEquipmentRecords = false,
										int limit = 1500);

		[OperationContract]
		EquipmentInfo[] EnumerateInfo(SecurityClass security);

		[OperationContract]
		EquipmentInfo[] EnumerateInfoUndelegated(SecurityClass security, bool excludeCompartments);

		[OperationContract]
		EquipmentInfo[] EnumerateInfoByTypesCompanyFuelCardProductAndSecondaryStorage(SecurityClass security, EQUIPMENT_TYPE[] types, object companyGuid, object fuelCardGuid, object productIndexGuid, object secondaryStorage, bool hideHiddenEquipmentRecords = false);

		[OperationContract]
		Guid GetIdentityGuidByCardNumberAndEquipmentID(SecurityClass security, Guid companyGuid, string truckCardNumber);

		[OperationContract]
		Guid GetIdentityGuidByCompanyGuidAndEquipmentID(SecurityClass security, Guid companyGuid, string companyEquipmentID);

		[OperationContract]
		Guid GetIdentityGuidByTruckCardNumber(SecurityClass security, String truckCardNumber);

		[OperationContract]
		string GetLatestRowVersionBySource(SecurityClass security);

		[OperationContract]
		DataSet EnumerateUpdateVersions(SecurityClass security);

		[OperationContract]
		DataSet EnumerateUpdateVersionsForOpc(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Import(SecurityClass security, EquipmentClass equipment);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, EquipmentClass equipment);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid equipmentGuid);

		[OperationContract]
		EquipmentClass GetByMeterGuid(SecurityClass security, Guid meterGuid);

		[OperationContract]
		Dictionary<string, Guid> GetEquipmentCompartmentGuids(SecurityClass security, Guid parentEquipmentGuid);
	}
}

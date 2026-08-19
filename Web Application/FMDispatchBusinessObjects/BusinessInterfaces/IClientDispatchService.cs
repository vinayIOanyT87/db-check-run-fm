

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Data;

using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;

namespace FMDispatchBusinessObjects.BusinessInterfaces
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface IClientDispatchService
	{

		[OperationContract]
		void IsDefenseKey();

		[OperationContract]
		void ReadHardwareKey();

		[OperationContract]
		SecurityLoginResponse Login(SecurityLoginRequest sr);

		[OperationContract]
		void PingSession(SecurityClass security);


		[OperationContract]
		SiteClass GetSite(SecurityClass security, Guid siteGuid);

		[OperationContract]
		CompanyClass GetCompany(SecurityClass security, Guid companyGuid);

		[OperationContract]
		Guid GetCompanyGuidById(SecurityClass security, string companyId);


		[OperationContract]
		bool ProcessFatalError(SecurityClass security, FMFatalErrorException fatalErrorEx);


		[OperationContract]
		PersonClass GetPerson(SecurityClass security, Guid personGuid);

		[OperationContract]
		Guid GetPersonGuidById(SecurityClass security, string personId);

		[OperationContract]
		ProductClass GetProduct(SecurityClass security, Guid productGuid);

		[OperationContract]
		Guid GetProductGuidById(SecurityClass security, string productId);


		[OperationContract]
		EquipmentClass GetEquipment(SecurityClass security, Guid equipmentGuid);

		[OperationContract]
		Guid GetEquipmentGuidById(SecurityClass security, string equipmentId);

		[OperationContract]
		TransactionDO GetTransactionByTransID(SecurityClass security, string transID);

		[OperationContract]
		TransactionDO GetTransactionByTransactionGuid(SecurityClass security, Guid transactionGuid);


		[OperationContract]
		string GenerateDocumentNumbers(SecurityClass security, TransactionTypes transTypeId);


		[OperationContract]
		SaveTransactionsResultDO SaveTransaction(SecurityClass security, object transactions, PersonClass person);


		[OperationContract]
		Guid SaveTransactionNote(SecurityClass security, Guid transGuid, string note, string transactionNote);


		[OperationContract]
		SaveTransactionsResultDO SaveTransactions(SaveTransactionsSR serviceRequest);

		[OperationContract]
		ControllerLogClass EnumerateControllerLogByIdentityGuid(SecurityClass security, Guid editedItemGuid);


		[OperationContract]
		void EditControllerLog(SecurityClass security, Guid editedItemGuid, ControllerLogClass controller);


		[OperationContract]
		void ModifyPeople(SecurityClass security, List<PersonClass> changedPeople);


		[OperationContract]
		void ChangePassword(SecurityClass security, string currentPassword, string newPassword);

		[OperationContract]
		List<ControllerLogClass> EnumerateControllerLogByStartStopTime(SecurityClass security, DateTime startDate, DateTime stopDate, bool showDeleted);


		[OperationContract]
		void DeleteControllerLogs(SecurityClass security, List<Guid> items, bool undelete);

		[OperationContract]
		AppointmentCollectionClass EnumerateAppointmentsByStartStopTime(SecurityClass security, string appType, DateTime startDate, DateTime endDate);

		[OperationContract]
		string Logout(SecurityClass security);

		[OperationContract]
		SaveTransactionsResultDO CopyTransaction(CopyTransactionsSR sr);

		[OperationContract]
		EquipmentCollectionClass EnumerateEquipmentBySource(SecurityClass security);

		[OperationContract]
		EquipmentCollectionClass EnumerateManagedEquipment(SecurityClass security);

		[OperationContract]
		EquipmentCollectionClass EnumerateByManagedFillstand(SecurityClass security);

		[OperationContract]
		PersonCollectionClass EnumeratePersonnelByRole(SecurityClass security, PERSON_ROLE role);

		[OperationContract]
		CompanyCollectionClass EnumerateCompanyByRole(SecurityClass security, COMPANY_ROLE role);

		[OperationContract]
		DispatchTransactionsDO GetDispatchTransactions(DispatchTransactionsSR sr);

		[OperationContract]
		DataSet EnumerateEquipmentUpdateVersions(SecurityClass security);

		[OperationContract]
		DataSet EnumeratePersonUpdateVersions(SecurityClass security);

		[OperationContract]
		long GetLatestTransactionVersion(SecurityClass security);

		[OperationContract]
		void ModifyControllerLog(SecurityClass security, ControllerLogClass controller);

		[OperationContract]
		void AddControllerLog(SecurityClass security, ControllerLogClass controller);

		[OperationContract]
		UserClass GetUser(SecurityClass security, Guid userGuid);

		[OperationContract]
		UserClass ModifyUserPassword(SecurityClass security, string newPassword);

		[OperationContract]
		void ModifyPerson(SecurityClass security, PersonClass person);

		[OperationContract]
		DispatchTransactionsDO GetLineItems(DispatchTransactionsSR sr);

		[OperationContract]
		EquipmentTypeClass GetEquipmentTypeByGuid(SecurityClass security, Guid equipmentTypeGuid);

		[OperationContract]
		QualityTagClass GetQualityTagByGuid(SecurityClass security, Guid qualityTagGuid);

		[OperationContract]
		EquipmentQualityTagLogClass GetMostRecentQualityTagLogByEquipmentID(SecurityClass security, string equipmentId);

		[OperationContract]
		AccountingSite LoadSiteInfo(SecurityClass security, Guid siteGuid);

		[OperationContract]
		TransactionAliasClass GetTransactionAliasFromAliasId(SecurityClass security, string aliasId, bool byUser);

		[OperationContract]
		TransactionAliasClass GetTransactionAliasFromAliasGuid(SecurityClass security, Guid aliasGuid, bool byUser);

		[OperationContract]
		void ImportEquipment(SecurityClass security, EquipmentClass equipment);

		[OperationContract]
		void ImportPerson(SecurityClass security, PersonClass person);

		[OperationContract]
		DataSet EnumeratePersonByRole(SecurityClass security, PERSON_ROLE role);
		
		[OperationContract]
		InventoryDateDO ProcessInventoryDateServiceRequest(InventoryDateSR inventoryDateSR);

		[OperationContract]
		DispatchTransactionsDO ProcessDispatchTransactionServiceRequest(DispatchTransactionsSR transactionSR);

		[OperationContract]
		void ProcessTransactionImportServiceRequest(TransactionImportSR importSr);

		[OperationContract]
		TransactionDO ProcessTransactionTransactionServiceRequest(TransactionSR transactionSr);

		[OperationContract]
		Guid ProcessTransactionNoteServiceRequest(TransactionNoteSR noteSr);

		[OperationContract]
		DataSet EnumerateEquipmentByTypesCompanyFuelCardProductAndSecondaryStorage1(
			SecurityClass security,
			EQUIPMENT_TYPE[] types,
			object secondaryStorage);

		[OperationContract]
		FuelCardCollectionClass EnumerateFuelCards(SecurityClass security);

		[OperationContract]
		DataSet EnumerateProductsByType(SecurityClass security, ProductType productType);


		[OperationContract]
		Guid GetUserDataFieldsIdentityGuid(
			SecurityClass security,
			ENTITY_TYPE entityType,
			Guid transactionAliasGuid,
			int number,
			bool isDispatch);

		[OperationContract]
		UserDataFieldClass GetUserDataField(SecurityClass security, Guid identityGuid, ENTITY_TYPE entityType);

		[OperationContract]
		Guid GetFuelCardGuidById(SecurityClass security, string fuelCardId);

		[OperationContract]
		FuelCardClass GetFuelCard(SecurityClass security, Guid fuelCardGuid);


		[OperationContract]
		Guid GetTransactionAliasMasterRecordGuid(SecurityClass security, string aliasId);


		[OperationContract]
		ProductCollectionClass EnumerateProducts(SecurityClass security);

		[OperationContract]
		Dictionary<string, string> ReleaseToAccounting(SecurityClass security, DateTimeOffset date);


		// TODO: Add your service operations here
	}


}

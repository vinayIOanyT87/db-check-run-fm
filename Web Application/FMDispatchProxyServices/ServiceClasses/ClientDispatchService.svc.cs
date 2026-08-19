
namespace FMDispatchProxyServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.EnterpriseServices.Internal;
	using System.ServiceModel;
	using System.Data;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMDispatchBusinessObjects.BusinessInterfaces;

	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
	public class ClientDispatchService : IClientDispatchService
	{
		public void ReadHardwareKey()
		{
			FMChannelHelper.MakeCall<IHardwareKey>(
				x =>
				{
					x.ReadHardwareKey();

					// check that the dispatch option is enabled in the key
					if ((x.GetOptionsCell() & 0x1000) == 0)
					{
						throw new Exception("Dispatch Not Authorized For This Computer");
					}
				});
		}

		public void IsDefenseKey()
		{
			//FMChannelFactory<IHardwareKey>.RefreshConfiguration();
			FMChannelHelper.MakeCall<IHardwareKey>(x => x.IsDefenseKey());
		}

		public SecurityLoginResponse Login(SecurityLoginRequest sr)
		{
			SecurityLoginResponse loginResult =
				FMChannelHelper.MakeCall<ISites, SecurityLoginResponse>(
					x => x.Login2(sr));
			return loginResult;

		}

		public void PingSession(SecurityClass security)
		{
			FMChannelHelper.MakeCall<ISessions>(x => x.PingSession(security));

		}

		public bool ProcessFatalError(SecurityClass security, FMFatalErrorException fatalErrorEx)
		{
			throw new NotImplementedException("Complete merge for FMBusinessObject");
			//bool shutdownDispatch = FMChannelHelper.MakeCall<IFMFatalErrorHandler, Boolean>(x => x.ProcessFatalError(security, fatalErrorEx));
			//return shutdownDispatch;
		}

		public CompanyClass GetCompany(SecurityClass security, Guid companyGuid)
		{
			return FMChannelHelper.MakeCall<ICompanies, CompanyClass>( x => x.Get(security, companyGuid, false));
		}

		public Guid GetCompanyGuidById(SecurityClass security, string companyId)
		{
			return FMChannelHelper.MakeCall<ICompanies, Guid>(x => x.GetIdentityGuid(security, companyId));
		}

		public SiteClass GetSite(SecurityClass security, Guid siteGuid)
		{
			var site =
				FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, siteGuid, 
							getMemberSites: false,
							bGetAssociatedAliases: false,
							getSchedulesAndProcessVariables: false));
			return site;

		}

		public EquipmentClass GetEquipment(SecurityClass security, Guid equipmentGuid)
		{
			var equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(security, equipmentGuid));
			return equipment;

		}

		public Guid GetEquipmentGuidById(SecurityClass security, string equipmentId)
		{
			return FMChannelHelper.MakeCall<IEquipments, Guid>(x => x.GetIdentityGuid(security, equipmentId));
		}
		
		public PersonClass GetPerson(SecurityClass security, Guid personGuid)
		{
			var Operator = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(security, personGuid));
			return Operator;

		}

		public Guid GetPersonGuidById(SecurityClass security, string personId)
		{
			var personGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(x => x.GetGuidByID(security, personId));
			return personGuid;
		}

		public ProductClass GetProduct(SecurityClass security, Guid productGuid)
		{
			return FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(security, productGuid));
		}

		public Guid GetProductGuidById(SecurityClass security, string productId)
		{
			return  FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetIdentityGuid(security, productId));
		}

		public TransactionDO GetTransactionByTransID(SecurityClass security, string transID)
		{
			var sr = new TransactionSR { Security = security, TransID = transID };

			TransactionDO transaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(sr));

			if (transaction == null)
			{
				throw new ArgumentException("Transaction not found");
			}
			return transaction;

		}
		public TransactionDO GetTransactionByTransactionGuid(SecurityClass security, Guid transactionGuid)
		{
			var sr = new TransactionSR
			{
				Security = security,
				TransactionGuid = transactionGuid
			};

			var transaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(sr));
			
			if (transaction == null)
			{
				throw new ArgumentException("Transaction not found");
			} 
			
			return transaction;
		}

		public string GenerateDocumentNumbers(SecurityClass security, TransactionTypes transTypeId)
		{
			if (transTypeId == TransactionTypes.T5_PrimaryDisbursement
				|| transTypeId == TransactionTypes.T25_Shipment)
			{
				return FMChannelHelper.MakeCall<ISites, string>(
						x => x.GetNextDocumentNumber(security, DOCUMENT_TYPE.MANUAL_BOL, security.SiteGuid));

			}

			if ((transTypeId == TransactionTypes.T17_Order)
				|| (transTypeId == TransactionTypes.T18_SupplyOrder))
			{
				return FMChannelHelper.MakeCall<ISites, string>(
					x => x.GetNextDocumentNumber(security, DOCUMENT_TYPE.ORDER, security.SiteGuid));

			}

			return FMChannelHelper.MakeCall<ISites, string>(
				x => x.GetNextDocumentNumber(security, DOCUMENT_TYPE.TRANSACTION, security.SiteGuid));
		}

		public SaveTransactionsResultDO SaveTransaction(SecurityClass security, object transactions, PersonClass person)
		{
			SaveTransactionsResultDO results = null;
			List<TransactionDO> transactionList;

			if (transactions is List<TransactionDO>)
			{
				transactionList = transactions as List<TransactionDO>;
			}
			else if (transactions is TransactionDO)
			{
				transactionList = new List<TransactionDO> { transactions as TransactionDO };
			}
			else
			{
				throw new Exception("Invalid Transaction Object passed to SaveTransaction");
			}

			try
			{
				SiteClass site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
						x.Get(
							security,
							security.SiteGuid,
							getMemberSites: false,
							bGetAssociatedAliases: false,
							getSchedulesAndProcessVariables: false));

				var saveSR = new SaveTransactionsSR
				{
					IndividualDbTransaction = false,
					Security = security,
					CurrentSiteGuid = security.SiteGuid,
					ConvertUnits = true,
					Operator = person
				};

				foreach (TransactionDO transaction in transactionList)
				{
					// Check the aviation and capitalize flags against the product configuration
					LineItemDO lineItem = transaction.LineItems[0];

					ProductClass product =
						FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(security, lineItem.ProductGuid));

					transaction.Flag02 = product.UserData1.Equals("YES", StringComparison.CurrentCultureIgnoreCase);
					transaction.Flag01 = product.UserData2.Equals("YES", StringComparison.CurrentCultureIgnoreCase);

					transaction.UserData[TransactionDO.USER_DATA_KEY_09] = "9 (LOCAL)";

					var transactionAlias =
						FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
							x => x.Get(security, transaction.TransactionAliasGuid, false));

					var unitsHelper = new UnitsHelperClass(security, site, transactionAlias, null);
					unitsHelper.SetUnits(transaction, 0);

					foreach (LineItemDO item in transaction.LineItems)
					{
						ProductClass prod =
							FMChannelHelper.MakeCall<IProducts, ProductClass>(
								x => x.Get(security, item.ProductGuid));

						unitsHelper.SetUnits(item, prod.ProductType, product);
					}

					saveSR.Transactions.Add(transaction);

					if (saveSR.Transactions.Count >= 5)
					{
						results = FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(x => x.SaveTransactions(saveSR));

						this.CheckForAndDisplayWarningMessages(results);
						saveSR.Transactions.Clear();
					}
				}

				if (saveSR.Transactions.Count > 0)
				{
					results = FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(x => x.SaveTransactions(saveSR));
					this.CheckForAndDisplayWarningMessages(results);
				}
			}
			catch (FaultException<SaveTransactionsException> saveExcept)
			{
				string errorMessage = "Save Transaction Failed!";

				foreach (TransactionValidationResult result in saveExcept.Detail.Results)
				{
					foreach (string error in result.ErrorList)
					{
						errorMessage += "\n\r" + error;
					}
				}

				throw new Exception(errorMessage);

			}

			return results;
		}

		public Guid SaveTransactionNote(SecurityClass security, Guid transGuid, string note, string transactionNote)
		{
			throw new NotImplementedException("Complete merge for FMBusinessObject");
			Guid notesGuid = Guid.Empty;


			//var sr = new TransactionNoteSR { TransGuid = transGuid, Security = security };

			//if (transactionNote.Length == 0)
			//{
			//	sr.Note = note;
			//}
			//else
			//{
			//	sr.Note = note + " - " + transactionNote;
			//}

			//sr.UpdatedBy = security.UserID;
			//notesGuid = FMChannelHelper.MakeCall<ITransactionNoteProcessor, Guid>(x => x.Process(sr));

			return notesGuid;
		}

		public SaveTransactionsResultDO SaveTransactions(SaveTransactionsSR serviceRequest)
		{

			return FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
					x => x.SaveTransactions(serviceRequest));
		}

		public ControllerLogClass EnumerateControllerLogByIdentityGuid(SecurityClass security, Guid editedItemGuid)
		{
			ControllerLogClass controller = FMChannelHelper.MakeCall<IControllerLogs, ControllerLogClass>(
										x => x.EnumerateControllerLogByIdentityGuid(security, editedItemGuid));
			return controller;
		}

		public void EditControllerLog(SecurityClass security, Guid editedItemGuid, ControllerLogClass controller)
		{
			if (editedItemGuid == Guid.Empty) // add operation
			{
				FMChannelHelper.MakeCall<IControllerLogs>(x => x.Add(security, controller));
			}
			else
			{
				controller.IdentityGuid = editedItemGuid;
				FMChannelHelper.MakeCall<IControllerLogs>(x => x.Modify(security, controller));
			}
		}

		public void ModifyPeople(SecurityClass security, List<PersonClass> changedPeople)
		{
			FMChannelHelper.MakeCall<IPersonnel>(
				x =>
				{
					foreach (PersonClass person in changedPeople)
					{
						x.Modify(security, DATA_TYPE.DYNAMIC, person);
					}
				});
		}

		public void ChangePassword(SecurityClass security, string currentPassword, string newPassword)
		{
			UserClass user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(security, security.UserGuid));

			if (!FMChannelHelper.MakeCall<ISites, bool>(x => x.CheckCurrentPassword(user, currentPassword)))
			{
				throw new ApplicationException("Current password entered incorrectly");
			}

			string oldPassword = user.Password;
			user.Password = newPassword;
			user.ChangePassword = false;
			user.PasswordTimestamp = DateTime.UtcNow;
			FMChannelHelper.MakeCall<IUsers>(x => x.ModifyWithPasswordHistory(security, user, oldPassword));

		}

		public List<ControllerLogClass> EnumerateControllerLogByStartStopTime(SecurityClass security, DateTime startDate, DateTime stopDate, bool showDeleted)
		{
			List<ControllerLogClass> controllerLogCollection =
				FMChannelHelper.MakeCall<IControllerLogs, List<ControllerLogClass>>(
					x =>
					x.EnumerateByStartStopTime(
						security, startDate, stopDate, showDeleted));
			return controllerLogCollection;

		}

		public void DeleteControllerLogs(SecurityClass security, List<Guid> items, bool undelete)
		{
			foreach (var itemToDelete in items)
			{
				if (undelete == false)
				{
					Guid delete = itemToDelete;
					FMChannelHelper.MakeCall<IControllerLogs>(x => x.DeleteControllerLog(security, delete));
				}
				else
				{
					Guid delete = itemToDelete;
					FMChannelHelper.MakeCall<IControllerLogs>(x => x.UnDeleteControllerLog(security, delete));
				}
			}
		}

		public AppointmentCollectionClass EnumerateAppointmentsByStartStopTime(SecurityClass security, string appType, DateTime startDate, DateTime endDate)
		{
			AppointmentCollectionClass appointmentCollection = FMChannelHelper.MakeCall<IAppointments, AppointmentCollectionClass>(
				x => x.EnumerateByStartStopTime(security, appType, startDate, endDate));
			return appointmentCollection;

		}

		public string Logout(SecurityClass security)
		{
			var alertSetting = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_AlertSessionLogoutEnabled));
			FMChannelHelper.MakeCall<ISites>(x => x.Logout(security));
			return alertSetting;

		}



		public SaveTransactionsResultDO CopyTransaction(CopyTransactionsSR sr)
		{
			return FMChannelHelper.MakeCall<ICopyTransactionsProcessor, SaveTransactionsResultDO>(
						copyTransactionsProcessor => copyTransactionsProcessor.Process(sr));
		}


		public EquipmentCollectionClass EnumerateEquipmentBySource(SecurityClass security)
		{
			EquipmentCollectionClass equipmentCache =
						FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(x => x.EnumerateBySource(security));
			return equipmentCache;
		}

		public EquipmentCollectionClass EnumerateManagedEquipment(SecurityClass security)
		{
			return FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(x => x.EnumerateManagedEquipment(security));
		}

		public EquipmentCollectionClass EnumerateByManagedFillstand(SecurityClass security)
		{
			return FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(x => x.EnumerateByManagedFillstand(security));
		}
		


		public PersonCollectionClass EnumeratePersonnelByRole(SecurityClass security, PERSON_ROLE role)
		{
			return FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
					x => x.EnumerateByRole(security, role));
		}

		public CompanyCollectionClass EnumerateCompanyByRole(SecurityClass security, COMPANY_ROLE role)
		{
			return FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
					x => x.EnumerateByRole(security, role, false, true));
		}

		public DispatchTransactionsDO GetDispatchTransactions(DispatchTransactionsSR sr)
		{
			return FMChannelHelper.MakeCall<IDispatchTransactionsProcessor, DispatchTransactionsDO>(x => x.GetLineItems(sr));
		}

		public DataSet EnumerateEquipmentUpdateVersions(SecurityClass security)
		{
			DataSet dataSet = FMChannelHelper.MakeCall<IEquipments, DataSet>(x => x.EnumerateUpdateVersions(security));


			return dataSet;
		}

		public DataSet EnumeratePersonUpdateVersions(SecurityClass security)
		{
			DataSet dataSet = FMChannelHelper.MakeCall<IPersonnel, DataSet>(x => x.EnumerateUpdateVersions(security));
			return dataSet;
		}

		public long GetLatestTransactionVersion(SecurityClass security)
		{
			// Determine if we have any transaction updates to communicate
			long result = FMChannelHelper.MakeCall<IDispatchTransactionsProcessor, long>(
				accountingInterface =>
				{
					var sr = new DispatchTransactionsSR
					{
						SubCommand = DispatchTransactionsSR.SubCommands.GetVersion,
						Security = security
					};

					DispatchTransactionsDO results = accountingInterface.Process(sr);

					// Get the version and determine if it changed.
					if (results.Transactions != null
						&& results.Transactions.Tables.Count > 0
						&& results.Transactions.Tables[0].Rows.Count > 0)
					{
						var version = BitConverter.ToInt64((byte[])results.Transactions.Tables[0].Rows[0]["_RowVersion"], 0);

						return version;
					}

					return 0;
				});

			return result;
		}
		protected void CheckForAndDisplayWarningMessages(SaveTransactionsResultDO resultDO)
		{
			if (resultDO.Results.Count > 0)
			{
				bool found = false;

				string msg = "Save transaction warnings";
				msg = msg + "! ";

				foreach (TransactionValidationResult result in resultDO.Results)
				{
					foreach (string error in result.WarningList)
					{
						msg += "\n\r" + error;
						found = true;
					}
				}

				if (found)
				{
					throw new Exception(msg);
				}
			}
		}


		public void AddControllerLog(SecurityClass security, ControllerLogClass controller)
		{
			FMChannelHelper.MakeCall<IControllerLogs>(x => x.Add(security, controller));
		}


		public void ModifyControllerLog(SecurityClass security, ControllerLogClass controller)
		{
			FMChannelHelper.MakeCall<IControllerLogs>(x => x.Modify(security, controller));
		}


		public UserClass GetUser(SecurityClass security, Guid userGuid)
		{
			return FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(security, userGuid));
		}

		public UserClass ModifyUserPassword(SecurityClass security, string newPassword)
		{
			UserClass user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(security, security.UserGuid));

			if (!FMChannelHelper.MakeCall<ISites, bool>(x => x.CheckCurrentPassword(user, newPassword)))
			{
				throw new ApplicationException("Current password entered incorrectly");
			}

			string oldPassword = user.Password;
			user.Password = newPassword;
			user.ChangePassword = false;
			user.PasswordTimestamp = DateTime.UtcNow;
			FMChannelHelper.MakeCall<IUsers>(x => x.ModifyWithPasswordHistory(security, user, oldPassword));

			return user;
		}


		public void ModifyPerson(SecurityClass security, PersonClass person)
		{
			FMChannelHelper.MakeCall<IPersonnel>(x => x.Modify(security, DATA_TYPE.DYNAMIC, person));
		}


		public DispatchTransactionsDO GetLineItems(DispatchTransactionsSR sr)
		{
			return FMChannelHelper.MakeCall<IDispatchTransactionsProcessor, DispatchTransactionsDO>(x => x.GetLineItems(sr));
		}


		public EquipmentTypeClass GetEquipmentTypeByGuid(SecurityClass security, Guid equipmentTypeGuid)
		{
			return FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
					x => x.Get(security, equipmentTypeGuid));
		}


		public QualityTagClass GetQualityTagByGuid(SecurityClass security, Guid qualityTagGuid)
		{
			return FMChannelHelper.MakeCall<IQualityTags, QualityTagClass>(x => x.Get(security, qualityTagGuid));
		}

		public EquipmentQualityTagLogClass GetMostRecentQualityTagLogByEquipmentID(SecurityClass security, string equipmentId)
		{
			return FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, EquipmentQualityTagLogClass>(
					x => x.GetMostRecentByEquipmentID(security, equipmentId));
		}


		public AccountingSite LoadSiteInfo(SecurityClass security, Guid siteGuid)
		{
			return FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(x => x.LoadSiteInfo(security, siteGuid));
		}


		public TransactionAliasClass GetTransactionAliasFromAliasId(SecurityClass security, string aliasId, bool byUser)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
					x => x.Get(security, x.GetIdentityGuid(security, aliasId), byUser));
		}


		public TransactionAliasClass GetTransactionAliasFromAliasGuid(SecurityClass security, Guid aliasGuid, bool byUser)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
					x => x.Get(security, aliasGuid, byUser));
		}
		

		public void ImportEquipment(SecurityClass security, EquipmentClass equipment)
		{
			FMChannelHelper.MakeCall<IEquipments>(x => x.Import(security, equipment));
		}

		public void ImportPerson(SecurityClass security, PersonClass person)
		{
			FMChannelHelper.MakeCall<IPersonnel>(x => x.Import(security, person));
		}



		public DataSet EnumeratePersonByRole(SecurityClass security, PERSON_ROLE role)
		{

			return FMChannelHelper.MakeCall<IPersonnel, DataSet>(x => x.EnumerateByRole1(security, role));
		}


		public InventoryDateDO ProcessInventoryDateServiceRequest(InventoryDateSR inventoryDateSR)
		{
			return FMChannelHelper.MakeCall<IInventoryDateProcessor, InventoryDateDO>(x => x.Process(inventoryDateSR));
		}



		public DispatchTransactionsDO ProcessDispatchTransactionServiceRequest(DispatchTransactionsSR transactionSR)
		{
			return FMChannelHelper.MakeCall<IDispatchTransactionsProcessor, DispatchTransactionsDO>(x => x.Process(transactionSR));
		}


		public void ProcessTransactionImportServiceRequest(TransactionImportSR importSr)
		{
			FMChannelHelper.MakeCall<ITransactionImportProcessor>(x => x.Process(importSr));
		}


		public TransactionDO ProcessTransactionTransactionServiceRequest(TransactionSR transactionSr)
		{
			return FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(transactionSr));
		}

		public Guid ProcessTransactionNoteServiceRequest(TransactionNoteSR noteSr)
		{
			throw new NotImplementedException("Complete merge for FMBusinessObject");
			//return FMChannelHelper.MakeCall<ITransactionNoteProcessor, Guid>(x => x.Process(noteSr));
		}


		public DataSet EnumerateEquipmentByTypesCompanyFuelCardProductAndSecondaryStorage1(
			SecurityClass security,
			EQUIPMENT_TYPE[] types,
			object secondaryStorage)
		{
			return FMChannelHelper.MakeCall<IEquipments, DataSet>(
				x => x.EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(security, types, null, null, null, secondaryStorage));
		}

		public FuelCardCollectionClass EnumerateFuelCards(SecurityClass security)
		{
			return FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(x => x.EnumerateFuelCards(security));
		}

		public DataSet EnumerateProductsByType(SecurityClass security, ProductType productType)
		{
			return FMChannelHelper.MakeCall<IProducts, DataSet>(x => x.EnumerateByType1(security, ProductType.ComponentProduct));
		}

		public Guid GetUserDataFieldsIdentityGuid(
			SecurityClass security,
			ENTITY_TYPE entityType,
			Guid transactionAliasGuid,
			int number,
			bool isDispatch)
		{
			return FMChannelHelper.MakeCall<IUserDataFields, Guid>(
				x => x.GetIdentityGuid(security, entityType, transactionAliasGuid, number, isDispatch));
		}

		public UserDataFieldClass GetUserDataField(SecurityClass security, Guid identityGuid, ENTITY_TYPE entityType)
		{
			return FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldClass>(
				x => x.Get(security, identityGuid, entityType));
		}


		public Guid GetFuelCardGuidById(SecurityClass security, string fuelCardId)
		{
			return FMChannelHelper.MakeCall<IFuelCards, Guid>(x => x.GetIdentityGuid(security, fuelCardId));
		}

		public FuelCardClass GetFuelCard(SecurityClass security, Guid fuelCardGuid)
		{
			return FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(x => x.Get(security, fuelCardGuid, false));
		}


		public Guid GetTransactionAliasMasterRecordGuid(SecurityClass security, string aliasId)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, Guid>(x => x.GetMasterRecordGuid(security, aliasId));
		}

		public ProductCollectionClass EnumerateProducts(SecurityClass security)
		{
			return FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(security));
		}

		public Dictionary<string, string> ReleaseToAccounting(SecurityClass security, DateTimeOffset date)
		{
			throw new NotImplementedException("Complete merge for FMBusinessObject");
			//return FMChannelHelper.MakeCall<IDispatchRequests, Dictionary<string, string>>(dispatchRequests => dispatchRequests.ReleaseToAccounting(security, date));
		}
	}
}

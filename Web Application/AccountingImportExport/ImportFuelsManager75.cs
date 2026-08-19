// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportFuelsManager75.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ImportFuelsManager75 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AccountingImportExport
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.ServiceModel;
	using System.Text;
	using System.Web;
	using System.Xml;
	using System.Xml.Serialization;
	
	using FM7Accounting;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

	using AssociatedTxDO = FMBusinessObjects.DataObjects.AssociatedTxDO;
	using BaseCollections = FM7Accounting.BaseCollections;
	using BaseLineItemDO = FMBusinessObjects.DataObjects.BaseLineItemDO;
	using CloseoutDO = FMBusinessObjects.DataObjects.CloseoutDO;
	using CloseoutSR = FMBusinessObjects.ServiceRequests.CloseoutSR;
	using DataTransmissionRecordClass = FM7Accounting.DataTransmissionRecordClass;
	using EQUIPMENT_TYPE = ConsolidatedDataObjects.EQUIPMENT_TYPE;
	using LineItemDO = FMBusinessObjects.DataObjects.LineItemDO;
	using MeterReadingDO = FMBusinessObjects.DataObjects.MeterReadingDO;
	using PaymentInfoDO = FMBusinessObjects.DataObjects.PaymentInfoDO;
	using RouteInfoDO = FMBusinessObjects.DataObjects.RouteInfoDO;
	using RouteScheduleDO = FMBusinessObjects.DataObjects.RouteScheduleDO;
	using SubLineItemDO = FMBusinessObjects.DataObjects.SubLineItemDO;
	using TicketModes = FMBusinessObjects.DataObjects.TicketModes;
	using TransactionDO = FMBusinessObjects.DataObjects.TransactionDO;
	using TransactionImportSR = FMBusinessObjects.ServiceRequests.TransactionImportSR;
	using TransactionOrigin = FMBusinessObjects.DataObjects.TransactionOrigin;
	using TransactionPIDXDO = FMBusinessObjects.DataObjects.TransactionPIDXDO;
	using TransactionQuality = FMBusinessObjects.DataObjects.TransactionQuality;
	using TransactionSR = FMBusinessObjects.ServiceRequests.TransactionSR;
	using TransactionStatus = FMBusinessObjects.DataObjects.TransactionStatus;
	using TransportLineItemDO = FMBusinessObjects.DataObjects.TransportLineItemDO;
	using WeightReadingDO = FMBusinessObjects.DataObjects.WeightReadingDO;

	/// <summary>
	/// Import of transmission data from FuelsManager 7.5SP2 clients in order
	/// to facilitate transition from 7.5 to a later version enterprise system.  It uses references to 7.5SP2 assemblies to allow
	/// deserialization of the transmission record.
	/// </summary>
	public class ImportFuelsManager75
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImportFuelsManager75"/> class.
		/// </summary>
		/// <param name="context">The http context.</param>
		public ImportFuelsManager75(HttpContext context)
		{
			this.Context = context;
		}

		/// <summary>
		/// Gets or sets the http context.
		/// </summary>
		/// <value>
		/// The http context.
		/// </value>
		protected HttpContext Context { get; set; }

		/// <summary>
		/// This web method is specifically targeted at supporting import of transmission data from FuelsManager 7.5SP2 clients in order
		/// to facilitate transition from 7.5 to a later version enterprise system.  It uses references to 7.5SP2 assemblies to allow
		/// deserialization of the transmission record.
		/// </summary>
		/// <param name="xml">The XML of the data transmission.</param>
		/// <returns>A response object that describes the results of the import.</returns>
		public FM7Accounting.EntityDataImportResponseDO Import( string xml )
		{
			FM7Accounting.EntityDataImportResponseDO response = null;

			// Open service channels to use in the record import loop to come so we don't
			// open up a channel for each record in the loop.
			FMChannelHelper.MakeCall<IMeters>(
				meters => FMChannelHelper.MakeCall<ICompanies>(
				companies => FMChannelHelper.MakeCall<IGroups>(
				groups => FMChannelHelper.MakeCall<IPersonnel>(
				personnel => FMChannelHelper.MakeCall<IPIDXProfiles>(
				pidxProfiles => FMChannelHelper.MakeCall<ITransactionAliases>(
				aliases => FMChannelHelper.MakeCall<IProducts>(
				products => FMChannelHelper.MakeCall<ICloseoutProcessor>(
				closeout => FMChannelHelper.MakeCall<IApplicationStrings>(
				appStrings => FMChannelHelper.MakeCall<IEquipments>(
				equipments => FMChannelHelper.MakeCall<ITransactionImportProcessor>(
				transactionsProcessor => FMChannelHelper.MakeCall<ITransactionProcessor>(
				transactions => FMChannelHelper.MakeCall<ITanks>(
				tanks => FMChannelHelper.MakeCall<IFuelCards>(
				cards =>
				{
					response = this.InternalImportEntityData(
						products, companies, groups, personnel, pidxProfiles, aliases, meters, closeout, appStrings, equipments, transactionsProcessor, transactions, tanks, cards, xml );
				}))))))))))))));

			return response;
		}

		/// <summary>
		/// Initializes the proper rights.
		/// </summary>
		/// <param name="security">The security.</param>
		private void GiveRights( ref SecurityClass security )
		{
			// Rights
			security.AddRight( RIGHT.VIEW_SITES_AND_SITE_GROUPS );

			// 1 Companies
			// 11 CompanyMaps
			security.AddRight( RIGHT.VIEW_COMPANY_DATA );
			security.AddRight( RIGHT.MODIFY_COMPANY_DATA );

			// 2 Equipment
			security.AddRight( RIGHT.VIEW_EQUIPMENT_DATA );
			security.AddRight( RIGHT.MODIFY_EQUIPMENT_DATA );

			// 3 Fuel Cards
			security.AddRight( RIGHT.VIEW_FUEL_CARD_DATA );
			security.AddRight( RIGHT.MODIFY_FUEL_CARD_DATA );

			// 4 Personnel
			security.AddRight( RIGHT.VIEW_PERSONNEL_DATA );
			security.AddRight( RIGHT.MODIFY_PERSONNEL_DATA );

			// 5 Products
			security.AddRight( RIGHT.VIEW_PRODUCTS );
			security.AddRight( RIGHT.MODIFY_PRODUCTS );

			// 7 Groups
			security.AddRight( RIGHT.VIEW_USER_GROUPS );
			security.AddRight( RIGHT.MODIFY_USER_GROUPS );

			// 8 TransactionAliases
			security.AddRight( RIGHT.VIEW_TRANSACTION_ALIASES );
			security.AddRight( RIGHT.MODIFY_TRANSACTION_ALIASES );

			// 9 CloseoutRecord
			security.AddRight( RIGHT.VIEW_CLOSEOUT_DATA );

			// 14 PiDXProfiles
			// 15 PIDXProfixeCompanyMaps
			security.AddRight( RIGHT.VIEW_PIDX_PROFILES );
			security.AddRight( RIGHT.MODIFY_PIDX_PROFILES );

			// 17 TransactionSubLineItem
			// 18 TransactionNotes
			// 19 TransactionLineItem
			security.AddRight( RIGHT.VIEW_TRANSACTION_DATA );
			security.AddRight( RIGHT.MODIFY_TRANSACTION_DATA );
			security.AddRight( RIGHT.VIEW_FINANCIAL_DATA );
			security.AddRight( RIGHT.VIEW_ALARM_EVENT_LOGS );

			security.AddRight( RIGHT.VIEW_USERS );
			security.AddRight( RIGHT.MODIFY_USERS );

			security.AddRight( RIGHT.VIEW_ALLOCATIONS );
			security.AddRight( RIGHT.MODIFY_ALLOCATIONS );
		}

		/// <summary>
		/// Constructs and logs an event message for the ImportEntityData operation.
		/// </summary>
		/// <param name="eventIndicator">The event indicator.</param>
		/// <param name="dtr">The DTR.</param>
		/// <param name="eei">The eei.</param>
		/// <param name="additionalMessage">The additional message.</param>
		/// <param name="logEntryType">Type of the log entry.</param>
		private void LogEntityDataImportEvent(
			string eventIndicator,
			DataTransmissionRecordClass dtr,
			EnterpriseExportImportUtility eei,
			string additionalMessage,
			EventLogEntryType logEntryType )
		{
			string eventMessage = "Entity Data Import";
			string entityIdentifier = null;
			if (logEntryType.Equals(EventLogEntryType.Information) && (!eei.LogImportProcessInformation))
			{
				return;
			}

			if ( dtr != null )
			{
				entityIdentifier = "OriginatingSiteId: " + dtr.OriginatingSiteID;
				if ( dtr.ChangeQueueRecord != null )
				{
					string eventType = null;
					if ( dtr.ChangeQueueRecord.EventType != null )
					{
						if ( dtr.ChangeQueueRecord.EventType.Equals( "I" ) )
						{
							eventType = "Insert";
						}
						else if ( dtr.ChangeQueueRecord.EventType.Equals( "U" ) )
						{
							eventType = "Update";
						}
						else if ( dtr.ChangeQueueRecord.EventType.Equals( "D" ) )
						{
							eventType = "Delete";
						}
					}

					entityIdentifier = entityIdentifier + ", RecordType: " + dtr.ChangeQueueRecord.RecordType + ", RecordID: "
									   + dtr.ChangeQueueRecord.RecordID + ", EventType: " + eventType;
				}
			}

			string clientIpAddress = this.Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if ( string.IsNullOrEmpty( clientIpAddress ) )
			{
				clientIpAddress = this.Context.Request.ServerVariables["REMOTE_ADDR"];
			}

			string hostName = Dns.GetHostEntry( clientIpAddress ).HostName;
			string clientIdentifier = "ClientIPAddress: " + clientIpAddress + ", ClientHostName: " + hostName;

			if ( eventIndicator != null )
			{
				eventMessage = eventMessage + " - " + eventIndicator;
			}

			eventMessage = eventMessage + ".";
			if ( entityIdentifier != null )
			{
				eventMessage = eventMessage + " " + entityIdentifier + ".";
			}

			eventMessage = eventMessage + " " + clientIdentifier + ".";
			if ( additionalMessage != null )
			{
				eventMessage = eventMessage + " " + additionalMessage;
			}

			eei.WriteToEventLogs( eventMessage, logEntryType );
		}

		/// <summary>
		/// This internal method is specifically targeted at supporting import of transmission data from FuelsManager 7.5SP2 clients in order
		/// to facilitate transition from 7.5 to a later version enterprise system.  It uses references to 7.5SP2 assemblies to allow
		/// deserialization of the transmission record.
		/// </summary>
		/// <param name="products">
		/// The products service interface.
		/// </param>
		/// <param name="companies">
		/// The companies service interface.
		/// </param>
		/// <param name="groups">
		/// The groups service interface.
		/// </param>
		/// <param name="personnel">
		/// The personnel service interface.
		/// </param>
		/// <param name="pidxProfiles">
		/// The pidx profiles service interface.
		/// </param>
		/// <param name="aliases">
		/// The aliases service interface.
		/// </param>
		/// <param name="meters">
		/// The meters service interface.
		/// </param>
		/// <param name="closeout">
		/// The closeout service interface.
		/// </param>
		/// <param name="appStrings">
		/// The application strings service interface.
		/// </param>
		/// <param name="equipments">
		/// The equipment service interface.
		/// </param>
		/// <param name="transactionsProcessor">
		/// The transactions Processor.
		/// </param>
		/// <param name="transactions">
		/// The transactions import processor interface.
		/// </param>
		/// <param name="tanks">
		/// The tanks processor interface.
		/// </param>
		/// <param name="cards">
		/// The cards processor interface.
		/// </param>
		/// <param name="xml">
		/// The XML of the data transmission.
		/// </param>
		/// <returns>
		/// A response object that describes the results of the import.
		/// </returns>
		private FM7Accounting.EntityDataImportResponseDO InternalImportEntityData( IProducts products, ICompanies companies, IGroups groups, IPersonnel personnel, IPIDXProfiles pidxProfiles, ITransactionAliases aliases, IMeters meters, ICloseoutProcessor closeout, IApplicationStrings appStrings, IEquipments equipments, ITransactionImportProcessor transactionsProcessor, ITransactionProcessor transactions, ITanks tanks, IFuelCards cards, string xml )
		{
			const string StrFunctionName = "ImportEntityData(string xml)";

			var importSecurity = this.ObtainSecurityObject();

			var eei = new EnterpriseExportImportUtility( importSecurity, "FuelsManager AccountingImportExport.ImportService" );

			var response = new FM7Accounting.EntityDataImportResponseDO { RequestReceiveTime = DateTime.Now };
			DataTransmissionRecordClass recordClass = null;
			int recordCount = 0;

			try
			{
				var stringReader = new StringReader(xml);
				var xtr = new XmlTextReader(stringReader);
				var xmlSerializer = new XmlSerializer(typeof(FM7Accounting.DataTransmissionRecordCollectionClass));
				var importCollection = (FM7Accounting.DataTransmissionRecordCollectionClass)xmlSerializer.Deserialize(xtr);
				recordCount = importCollection.Count;

				// process the objects that have been serialized
			    // ReSharper disable once ForCanBeConvertedToForeach
				for (int i = 0; i < importCollection.Count; i++)
				{
					var record = importCollection[i];
					recordClass = record;

					SiteClass importSite =
						FMChannelHelper.MakeCall<ISites, SiteClass>(
							sites => sites.GetByID(importSecurity, record.OriginatingSiteID, false));

					if (importSite.SiteGuid == Guid.Empty)
					{
						continue;
					}

					importSecurity.SiteGuid = importSite.SiteGuid;
					importSecurity.SiteID = importSite.ID;
					importSecurity.UserIndex = 0;

					this.LogEntityDataImportEvent("Data Processing Started", record, eei, null, EventLogEntryType.Information);

					switch (record.ChangeQueueRecord.RecordType)
					{
						case ConsolidatedDataObjects.ChangeQueueRecordType.Companies:
							this.ProcessCompanyRecord(companies, meters, record, importSecurity);
							break;

						case ConsolidatedDataObjects.ChangeQueueRecordType.Products:
							this.ProcessProductRecord(products, meters, record, importSecurity);
							break;

						case ConsolidatedDataObjects.ChangeQueueRecordType.Groups:
							this.ProcessGroupRecord(groups, record, importSecurity);
							break;

						case ConsolidatedDataObjects.ChangeQueueRecordType.PIDXProfiles:
							this.ProcessPidxRecord(pidxProfiles, record, importSecurity);
							break;

						case ConsolidatedDataObjects.ChangeQueueRecordType.Personnel:
							this.ProcessPersonnelRecord(personnel, record, importSecurity);
							break;

						case ConsolidatedDataObjects.ChangeQueueRecordType.TransactionAliases:
							this.ProcessAliasRecord(aliases, meters, record, importSecurity);
							break;

						case ConsolidatedDataObjects.ChangeQueueRecordType.CloseoutDO:
							this.ProcessCloseoutRecord(closeout, companies, products, record, importSecurity, eei);
							break;

						case ConsolidatedDataObjects.ChangeQueueRecordType.ApplicationStrings:
							// Not supporting this because the 7.5 Export does not package these kinds of records.
							this.ProcessApplicationStrings(appStrings, record, importSecurity);
							break;

						case ConsolidatedDataObjects.ChangeQueueRecordType.PIDXProfileCompanyMaps:
							throw new ApplicationException("Pidx profile company maps not supported.");

						case ConsolidatedDataObjects.ChangeQueueRecordType.Equipment:
							this.ProcessEquipmentRecord(equipments, companies, products, record, importSecurity);
							break;

						case ConsolidatedDataObjects.ChangeQueueRecordType.Transactions:
							this.ProcessTransactionRecords(
								transactionsProcessor,
								transactions,
								aliases,
								companies,
								equipments,
								cards,
								personnel,
								meters,
								tanks,
								products,
								record,
								importSecurity);
							break;
					}

					record.ChangeQueueRecord.Completed = true;
					response.ProcessedChangeQueueRecords.Add(record.ChangeQueueRecord);
					this.LogEntityDataImportEvent("Data Processing Completed", record, eei, null, EventLogEntryType.Information);
				}

				response.Status = FM7Accounting.EntityDataImportResponseDO.ResponseStatus.SUCCESS;
			}
			catch (Exception ex)
			{
				string strAdditionalMessage = $"Exception in object: {this}, Function {StrFunctionName}, Message: {ex.Message}.";

				if (ex.InnerException != null)
				{
					strAdditionalMessage += $" InnerException:{ex.InnerException}";
				}

				if (eei.Security.SiteGuid != Guid.Empty)
				{
					response.Status = FM7Accounting.EntityDataImportResponseDO.ResponseStatus.FAIL;
					response.ErrorMessage = strAdditionalMessage;
					this.LogEntityDataImportEvent("Data Processing", recordClass, eei, strAdditionalMessage, EventLogEntryType.Error);
				}
				else
				{
					FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(strAdditionalMessage, FMEventLogEntryType.Error));
				}
			}
			finally
			{
				FMChannelHelper.MakeCall<ISites>(x => x.Logout(importSecurity));
			}

			// Archive the XML data
			try
			{
				var encoding = new UTF8Encoding();
				byte[] byteArray = encoding.GetBytes( xml );
				var memoryStream = new MemoryStream( byteArray );
				string archiveFilePath = eei.WriteStreamToFile( memoryStream, eei.ImportArchiveDir );
				DataTransmissionRecordClass dtr = null;
				if ( recordCount == 1 )
				{
					dtr = recordClass;

					// if the service is receiving only one entity record at a time, then attach the details of the data record with the data archiving log entry.
				}

				this.LogEntityDataImportEvent(
					"Data Archiving Completed",
					dtr,
					eei,
					"XML source data archived at: " + archiveFilePath,
					EventLogEntryType.Information );
			}
			catch ( Exception ex )
			{
				string strAdditionalMessage = $"Exception in object: {this}, Function {StrFunctionName}, Message: {ex.Message}.";

				this.LogEntityDataImportEvent( "XML Data Archiving", recordClass, eei, strAdditionalMessage, EventLogEntryType.Error );
			}

			response.ResponseSendTime = DateTime.Now;
			return response;
		}

		/// <summary>
		/// Obtains the security object.
		/// </summary>
		/// <returns>A security object for use in the import process.</returns>
		/// <exception cref="System.ApplicationException">
		/// Security not configured for process.
		/// or
		/// Security not valid for configured process.
		/// </exception>
		private SecurityClass ObtainSecurityObject()
		{
			// Get a temporary security object.
			var importSecurity = new SecurityClass();
			this.GiveRights( ref importSecurity );
			importSecurity.EnableChangeLogging = false;
			importSecurity.UserID = importSecurity.UserID ?? "FuelsManager";
			importSecurity.SiteID = "SiteAdmin";
			importSecurity.SiteGuid = Guids.SiteAdminGuid;
			
			// Get the configured user name to use.
			var userId =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetByKey(importSecurity, "FM75ImportUser").SettingValue);

			// Get the user's password
			var user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.GetByID(importSecurity, userId));
			if (user == null || user.IdentityGuid == Guid.Empty)
			{
				throw new ApplicationException("Security not configured for process.");
			}

			// Login to establish session
			bool changePassword = false;

			FMChannelHelper.MakeCall<ISites>( sites =>
				{
					int daysUntilExpiration;

					var sr = new SecurityLoginRequest
					{
						CACEnabled = false,
						UserID = userId,
						Password = user.Password,
						SiteID = importSecurity.SiteID,
						TimeOut = 300
					};

					sites.Login( out changePassword, out daysUntilExpiration, out importSecurity, sr );

					this.GiveRights(ref importSecurity);
				});

			if (changePassword)
			{
				throw new ApplicationException("Security not valid for configured process.");
			}

			return importSecurity;
		}

		/// <summary>
		/// Processes the transaction records.
		/// </summary>
		/// <param name="transactionsProcessor">The transactions processor.</param>
		/// <param name="transactions">The transactions.</param>
		/// <param name="aliases">The aliases service interface.</param>
		/// <param name="companies">The companies service interface.</param>
		/// <param name="equipments">The equipments service interface.</param>
		/// <param name="cards">The cards service interface.</param>
		/// <param name="personnel">The personnel service interface.</param>
		/// <param name="meters">The meters service interface.</param>
		/// <param name="tanks">The tanks service interface.</param>
		/// <param name="products">The products service interface.</param>
		/// <param name="record">The record service interface.</param>
		/// <param name="importSecurity">The import security object.</param>
		private void ProcessTransactionRecords(
			ITransactionImportProcessor transactionsProcessor, ITransactionProcessor transactions, ITransactionAliases aliases, ICompanies companies, IEquipments equipments, IFuelCards cards, IPersonnel personnel, IMeters meters, ITanks tanks, IProducts products, DataTransmissionRecordClass record, SecurityClass importSecurity )
		{
			if ( record.ChangeQueueRecord.IsDeletion )
			{
				throw new Exception( "Unsupported Request. Deletion of a Transaction record is not supported." );
			}

			try
			{
				TransactionDO trans = this.ImportTransactionDO( importSecurity, record.Transaction, aliases, companies, transactions, equipments, cards, personnel, meters, tanks, products );

				// Note: The SaveTransactionProcessor will bypass the record validation in this case (since the Security UserIndex is set to 0 and the Security UserId is set to DBAccess.ServiceLogin).
				var transactionimportSR = new TransactionImportSR( importSecurity, trans ) {Security = importSecurity};
				transactionsProcessor.Process(transactionimportSR);
			}
			catch ( FaultException<FMBusinessObjects.Exceptions.SaveTransactionsException> saveExcept )
			{
				string errorMessage = "Save Transaction Failed.";
				errorMessage += " TransID: " + record.Transaction.TransID;
				foreach ( TransactionValidationResult result in saveExcept.Detail.Results )
				{
					foreach ( string error in result.ErrorList )
					{
						errorMessage += " " + error + ".";
					}
				}

				throw new Exception( errorMessage );
			}
		}

		/// <summary>
		/// Processes the application strings import record.
		/// </summary>
		/// <param name="appStrings">The app strings.</param>
		/// <param name="record">The record to process.</param>
		/// <param name="importSecurity">The import security object.</param>
		private void ProcessApplicationStrings(IApplicationStrings appStrings, DataTransmissionRecordClass record, SecurityClass importSecurity)
		{
			var recordGuid = appStrings.GetIdentityGuid( importSecurity, (STRING_TYPE) record.ApplicationString.Type, record.ChangeQueueRecord.RecordID );

			if ( record.ChangeQueueRecord.IsDeletion )
			{
				if ( recordGuid != Guid.Empty )
				{
					appStrings.Purge( importSecurity, recordGuid );
				}
			}
			else
			{
				ApplicationStringClass appString = this.ImportApplicationString(record.ApplicationString);

				if (recordGuid == Guid.Empty)
				{
					appStrings.Add(importSecurity, appString);
				}
				else
				{
					appString.IdentityGuid = recordGuid;
					appStrings.Modify(importSecurity, appString);
				}
			}
		}

		/// <summary>
		/// Processes the closeout record.
		/// </summary>
		/// <param name="closeout">The closeout processor service interface.</param>
		/// <param name="companies">The companies service class interface.</param>
		/// <param name="products">The products service class interface.</param>
		/// <param name="record">The record to process.</param>
		/// <param name="importSecurity">The import security.</param>
		/// <param name="eei">The enterprise export import utility object.</param>
		private void ProcessCloseoutRecord( ICloseoutProcessor closeout, ICompanies companies, IProducts products, DataTransmissionRecordClass record, SecurityClass importSecurity, EnterpriseExportImportUtility eei )
		{
			if (record.ChangeQueueRecord.IsDeletion)
			{
				throw new Exception("Unsupported Request. Deletion of a Close-Out record is not supported.");
			}

			var closeOutDO = this.CopyCloseOut(importSecurity, record.Closeout, companies, products);

			var sr = new CloseoutSR
				         {
					         Security = importSecurity,
					         Closeout = closeOutDO,
					         CloseoutCommand = CloseoutSR.CloseoutType.SAVE_TO_IMPORT,
							 CurrentSiteGuid = importSecurity.SiteGuid,
							 ManagerCompanyGuid = closeOutDO.ManagerGuid,
							 ManagerName = closeOutDO.ManagerName,
							 ProductGuid = closeOutDO.ProductGuid,
							 ProductName = closeOutDO.ProductName
				         };

			try
			{
				closeout.Process(sr);
			}
			catch (Exception ex)
			{
				// If this is a duplicate Closeout attempt, then log the exception on the service side, but otherwise treat it as a successful request, i.e. do not stop the processing of DataTransmissionRecord Collection (if it contains more than one record), and do not let this exception return a FAIL response.
				if (ex.Message.Contains("already closed out"))
				{
					string strAdditionalMessage =
					    $"Exception in object: {this}, Function {"ProcessCloseoutRecord"}, Message: {ex.Message}.";

					if (ex.InnerException != null)
					{
						strAdditionalMessage += $" InnerException:{ex.InnerException}";
					}

					this.LogEntityDataImportEvent("Data Processing", record, eei, strAdditionalMessage, EventLogEntryType.Error);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Copies the close out object from 7.5 to current version.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="closeout">The closeout object to copy.</param>
		/// <param name="companies">The companies service class interface.</param>
		/// <param name="products">The products service class interface.</param>
		/// <returns>A current version closeout object.</returns>
		private CloseoutDO CopyCloseOut(SecurityClass security, FM7Accounting.CloseoutDO closeout, ICompanies companies, IProducts products)
		{
			var addCloseOut = new CloseoutDO
				                  {
					                  BookInventory = this.CopyVolumeDO(closeout.BookInventory),
					                  BrokenBlendDate = closeout.BrokenBlendDate,
					                  CloseoutDate = closeout.CloseoutDate,
					                  CloseoutRecordFound = closeout.CloseoutRecordFound,
					                  Flags = new BaseLineItemDO.StatusFlags((BaseLineItemDO.Status)closeout.Flags.Flags),
					                  LastCloseoutDate = closeout.LastcloseoutDate,
					                  ManagerName = closeout.ManagerName,
									  ProductName = closeout.ProductName,
									  SiteID = security.SiteID,
									  SiteGuid = security.SiteGuid,
									  TotalPhysicalInventory = this.CopyVolumeDO(closeout.TotalPhysicalInventory),
									  TotalVariance = this.CopyVolumeDO(closeout.TotalVariance)
				                  };

			addCloseOut.ManagerGuid = companies.GetMasterRecordGuid(security, addCloseOut.ManagerName);
			addCloseOut.ProductGuid = products.GetMasterRecordGuidFromID( security, addCloseOut.ProductName );

			return addCloseOut;
		}

		/// <summary>
		/// Copies the volume DO.
		/// </summary>
		/// <param name="bookInventory">The book inventory volume data object.</param>
		/// <returns>A quantity data object based on the volume data object passed.</returns>
		private QuantityDO CopyVolumeDO(VolumeDO bookInventory)
		{
			var quantity = new QuantityDO
				               {
					               AffectsInventory = bookInventory.AffectsInventory,
					               Gross = bookInventory.Gross,
					               GrossInventoryChange = bookInventory.GrossInventoryChange,
					               GrossPrice = bookInventory.GrossPrice,
					               GrossPriceInventoryChange = bookInventory.GrossPriceInventoryChange,
					               IsGrossDirty = bookInventory.IsGrossDirty,
					               IsNetDirty = bookInventory.IsNetDirty,
					               Net = bookInventory.Net,
					               NetInventoryChange = bookInventory.NetInventoryChange,
					               NetPrice = bookInventory.NetPrice,
					               NullableGross = bookInventory.NullableGross,
					               NullableNet = bookInventory.NullableNet
				               };

			return quantity;
		}

		/// <summary>
		/// Processes the equipment alias record.
		/// </summary>
		/// <param name="equipments">
		/// The equipments service interface.
		/// </param>
		/// <param name="companies">
		/// The companies service interface.
		/// </param>
		/// <param name="products">The products service interface.</param>
		/// <param name="record">
		/// The record to process.
		/// </param>
		/// <param name="importSecurity">
		/// The import security.
		/// </param>
		private void ProcessEquipmentRecord( IEquipments equipments, ICompanies companies, IProducts products, DataTransmissionRecordClass record, SecurityClass importSecurity )
		{
			if ( record.ChangeQueueRecord.IsDeletion )
			{
				var recordGuid = equipments.GetIdentityGuid( importSecurity, record.ChangeQueueRecord.RecordID );
				if ( recordGuid != Guid.Empty )
				{
					equipments.Purge( importSecurity, recordGuid );
				}
			}
			else
			{
				EquipmentClass importedEquipment = this.ImportEquipment( record.Equipment, importSecurity, equipments, companies, products );
				equipments.Import( importSecurity, importedEquipment );
			}
		}

		/// <summary>
		/// Processes the transaction alias record.
		/// </summary>
		/// <param name="aliases">The aliases service interface.</param>
		/// <param name="meters">The meter service interface.</param>
		/// <param name="record">The record to process.</param>
		/// <param name="importSecurity">The import security.</param>
		private void ProcessAliasRecord( ITransactionAliases aliases, IMeters meters, DataTransmissionRecordClass record, SecurityClass importSecurity )
		{
			if ( record.ChangeQueueRecord.IsDeletion )
			{
				var recordGuid = aliases.GetIdentityGuid( importSecurity, record.ChangeQueueRecord.RecordID );
				if ( recordGuid != Guid.Empty )
				{
					aliases.Purge( importSecurity, recordGuid );
				}
			}
			else
			{
				TransactionAliasClass importedAlias = this.ImportTransactionAlias( importSecurity, record.TransactionAlias, aliases, meters );
				aliases.Import( importSecurity, importedAlias );
			}
		}

		/// <summary>
		/// Processes the personnel profile record.
		/// </summary>
		/// <param name="personnel">The personnel service interface.</param>
		/// <param name="record">The record to process.</param>
		/// <param name="importSecurity">The import security.</param>
		private void ProcessPersonnelRecord( IPersonnel personnel, DataTransmissionRecordClass record, SecurityClass importSecurity )
		{
			if ( record.ChangeQueueRecord.IsDeletion )
			{
				var recordGuid = personnel.GetMasterRecordGuid( importSecurity, record.ChangeQueueRecord.RecordID );
				if ( recordGuid != Guid.Empty )
				{
					personnel.Purge( importSecurity, recordGuid );
				}
			}
			else
			{
				PersonClass importedPerson = this.ImportPersonnelProfile( importSecurity, record.Person, personnel );

				// the person class adds the schedule as part of the reset method. During deserilization seven more are added
				// we need to remap these and get rid of the old ones
				if ( record.Person.AccessScheduleCollection.Count == 14 )
				{
					record.Person.AccessScheduleCollection[0] = record.Person.AccessScheduleCollection[7];
					record.Person.AccessScheduleCollection[1] = record.Person.AccessScheduleCollection[8];
					record.Person.AccessScheduleCollection[2] = record.Person.AccessScheduleCollection[9];
					record.Person.AccessScheduleCollection[3] = record.Person.AccessScheduleCollection[10];
					record.Person.AccessScheduleCollection[4] = record.Person.AccessScheduleCollection[11];
					record.Person.AccessScheduleCollection[5] = record.Person.AccessScheduleCollection[12];
					record.Person.AccessScheduleCollection[6] = record.Person.AccessScheduleCollection[13];

					// remove the bad indexes
					record.Person.AccessScheduleCollection.RemoveAt( 13 );
					record.Person.AccessScheduleCollection.RemoveAt( 12 );
					record.Person.AccessScheduleCollection.RemoveAt( 11 );
					record.Person.AccessScheduleCollection.RemoveAt( 10 );
					record.Person.AccessScheduleCollection.RemoveAt( 9 );
					record.Person.AccessScheduleCollection.RemoveAt( 8 );
					record.Person.AccessScheduleCollection.RemoveAt( 7 );
				}

				personnel.Import( importSecurity, importedPerson );
			}
		}

		/// <summary>
		/// Processes the pidx profile record.
		/// </summary>
		/// <param name="pidxProfiles">The pidxProfiles service interface.</param>
		/// <param name="record">The record to process.</param>
		/// <param name="importSecurity">The import security.</param>
		private void ProcessPidxRecord( IPIDXProfiles pidxProfiles, DataTransmissionRecordClass record, SecurityClass importSecurity )
		{
			if ( record.ChangeQueueRecord.IsDeletion )
			{
				var recordGuid = pidxProfiles.GetIdentityGuid( importSecurity, record.ChangeQueueRecord.RecordID );
				if ( recordGuid != Guid.Empty )
				{
					pidxProfiles.Purge( importSecurity, recordGuid );
				}
			}
			else
			{
				PIDXProfileClass importedProfile = this.ImportPidxProfile( importSecurity, record.PidxProfile, pidxProfiles );
				pidxProfiles.Import( importSecurity, importedProfile );
			}
		}

		/// <summary>
		/// Processes the user group record.
		/// </summary>
		/// <param name="groups">The groups service interface.</param>
		/// <param name="record">The record to process.</param>
		/// <param name="importSecurity">The import security.</param>
		private void ProcessGroupRecord( IGroups groups, DataTransmissionRecordClass record, SecurityClass importSecurity )
		{
			if ( record.ChangeQueueRecord.IsDeletion )
			{
				var recordGuid = groups.GetIdentityGuid( importSecurity, record.ChangeQueueRecord.RecordID );
				if ( recordGuid != Guid.Empty )
				{
					groups.Purge( importSecurity, recordGuid );
				}
			}
			else
			{
				GroupClass importedGroup = this.ImportGroup( importSecurity, record.Group, groups );
				groups.Import( importSecurity, importedGroup );
			}
		}

		/// <summary>
		/// Processes the product record.
		/// </summary>
		/// <param name="products">
		/// The products service interface.
		/// </param>
		/// <param name="meters">
		/// The meters service interface.
		/// </param>
		/// <param name="record">
		/// The record to process.
		/// </param>
		/// <param name="importSecurity">
		/// The import security.
		/// </param>
		private void ProcessProductRecord( IProducts products, IMeters meters, DataTransmissionRecordClass record, SecurityClass importSecurity )
		{
			if ( record.ChangeQueueRecord.IsDeletion )
			{
				var recordGuid = products.GetIdentityGuid( importSecurity, record.ChangeQueueRecord.RecordID );
				if ( recordGuid != Guid.Empty )
				{
					products.Purge( importSecurity, recordGuid );
				}
			}
			else
			{
				ProductClass importedProduct = this.ImportProduct( importSecurity, record.Product, products, meters );
				products.Import( importSecurity, importedProduct );
			}
		}

		/// <summary>
		/// Processes the company record.
		/// </summary>
		/// <param name="companies">The companies service interface.</param>
		/// <param name="meters">The meters service interface.</param>
		/// <param name="record">The record to process.</param>
		/// <param name="importSecurity">The import security.</param>
		private void ProcessCompanyRecord( ICompanies companies, IMeters meters, DataTransmissionRecordClass record, SecurityClass importSecurity )
		{
			if ( record.ChangeQueueRecord.IsDeletion )
			{
				var recordGuid = companies.GetIdentityGuid( importSecurity, record.ChangeQueueRecord.RecordID );
				if ( recordGuid != Guid.Empty )
				{
					companies.Purge( importSecurity, recordGuid );
				}
			}
			else
			{
				var importedCompany = this.ImportCompany( importSecurity, record.Company, companies, meters );
				companies.Import( importSecurity, importedCompany );
			}
		}

		/// <summary>
		/// Imports the transaction DO.
		/// </summary>
		/// <param name="importSecurity">The import security object.</param>
		/// <param name="trans">The transaction object.</param>
		/// <param name="aliases">The aliases service interface.</param>
		/// <param name="companies">The companies service interface.</param>
		/// <param name="transactionProcessor">The transaction processor service interface.</param>
		/// <param name="equipments">The equipments service interface.</param>
		/// <param name="cards">The cards service interface.</param>
		/// <param name="personnel">The personnel service interface.</param>
		/// <param name="meters">The meters service interface.</param>
		/// <param name="tanks">The tanks service interface.</param>
		/// <param name="products">The products service interface.</param>
		/// <returns>A current version transaction data object with imported data.</returns>
		/// <exception cref="System.ApplicationException">Transaction alias not found for transaction import.</exception>
		private TransactionDO ImportTransactionDO(SecurityClass importSecurity, FM7Accounting.TransactionDO trans, ITransactionAliases aliases, ICompanies companies, ITransactionProcessor transactionProcessor, IEquipments equipments, IFuelCards cards, IPersonnel personnel, IMeters meters, ITanks tanks, IProducts products )
		{
			// create new transaction object
			var importedTrans = new TransactionDO();

			var sr = new TransactionSR { Security = importSecurity };

			// Copy fields
			importedTrans.Alias = trans.Alias;
			importedTrans.TransactionAliasGuid = aliases.GetMasterRecordGuid( importSecurity, importedTrans.Alias );

			if (importedTrans.TransactionAliasGuid == Guid.Empty)
			{
				throw new ApplicationException("Transaction alias not found for transaction import: " + importedTrans.Alias);
			}
			
			importedTrans.AssociatedCLIN = trans.AssociatedCLIN;
			importedTrans.AssociatedDocumentNumber = trans.AssociatedDocumentNumber;
			importedTrans.AssociatedOrderProduct = trans.AssociatedOrderProduct;
			importedTrans.AssociatedOrderTx = trans.AssociatedOrderTx;
			importedTrans.AssociatedTransportOrderNumber = trans.AssociatedTransportOrderNumber;

			importedTrans.BillToID = trans.BillToID;
			importedTrans.BillToCode = trans.BillToCode;
			importedTrans.BillToCompanyGuid = companies.GetMasterRecordGuid(importSecurity, importedTrans.BillToID);

			importedTrans.CarrierCode = trans.CarrierCode;
			importedTrans.CarrierID = trans.CarrierID;
			importedTrans.CarrierCompanyGuid = companies.GetMasterRecordGuid(importSecurity, importedTrans.CarrierID);

			importedTrans.CloseoutDate = importedTrans.CloseoutDate;
			importedTrans.ConjoinReversedTransID = importedTrans.ConjoinReversedTransID;
			importedTrans.ConjoinedTransID = importedTrans.ConjoinReversedTransID;

			if (string.IsNullOrEmpty(sr.TransID) == false)
			{
				sr.TransID = importedTrans.ConjoinedTransID;
				importedTrans.ConjoinedTransactionGuid = transactionProcessor.Process(sr).TransactionGuid;
			}

			importedTrans.Country = trans.Country;
			importedTrans.ContactFirstName = trans.ContactFirstName;
			importedTrans.ContactSurname = trans.ContactSurname;
			importedTrans.ContactInfo = trans.ContactInfo;

			if (trans.Date01 != null)
			{
				importedTrans.Date01 = trans.Date01.Value;
			}

			if (trans.Date02 != null)
			{
				importedTrans.Date02 = trans.Date02.Value;
			}

			if (trans.Date03 != null)
			{
				importedTrans.Date03 = trans.Date03.Value;
			}

			if (trans.Date04 != null)
			{
				importedTrans.Date04 = trans.Date04.Value;
			}

			importedTrans.DeleteFlag = trans.DeleteFlag;

			if (string.IsNullOrEmpty(trans.DestinationEQ1ID) == false)
			{
				importedTrans.DestinationEQ1.EquipmentGuid = equipments.GetMasterRecordGuid(importSecurity, trans.DestinationEQ1ID);
			}

			if (string.IsNullOrEmpty(trans.DestinationEQ2ID) == false)
			{
				importedTrans.DestinationEQ2.EquipmentGuid = equipments.GetMasterRecordGuid(importSecurity, trans.DestinationEQ2ID);
			}

			if (string.IsNullOrEmpty(trans.DestinationEQ3ID) == false)
			{
				importedTrans.DestinationEQ3.EquipmentGuid = equipments.GetMasterRecordGuid(importSecurity, trans.DestinationEQ3ID);
			}

			if (trans.DispatchedDateTime != null)
			{
				importedTrans.DispatchedDateTime = trans.DispatchedDateTime.Value;
			}

			importedTrans.DocumentNumber = trans.DocumentNumber;
			importedTrans.DriverIDNumber = trans.DriverIDNumber;

			if (trans.EffectiveDate != null)
			{
				importedTrans.EffectiveDate = trans.EffectiveDate.Value;
			}

			if (trans.EstimatedFuelingDuration != null)
			{
				importedTrans.EstimatedFuelingDuration = (int) trans.EstimatedFuelingDuration.Value;
			}

			if (trans.ExpirationDate != null)
			{
				importedTrans.ExpirationDate = trans.ExpirationDate.Value;
			}

			importedTrans.Flag01 = trans.Flag01;
			importedTrans.Flag02 = trans.Flag02;
			importedTrans.Flag03 = trans.Flag03;
			importedTrans.Flag04 = trans.Flag04;
			importedTrans.Flag05 = trans.Flag05;
			importedTrans.Flag06 = trans.Flag06;

			importedTrans.FuelCardID = trans.FuelCardID;

			if (string.IsNullOrEmpty(importedTrans.FuelCardID) == false)
			{
				importedTrans.FuelCardGuid = cards.GetIdentityGuid(importSecurity, importedTrans.FuelCardID);
			}

			importedTrans.InventoryDate = trans.InventoryDate;

			importedTrans.LegacyNumber = trans.LegacyNumber;
			importedTrans.LinkedDocumentNumber = trans.LinkedDocumentNumber;
			importedTrans.LoadID = trans.LoadID;

			importedTrans.ManagerID = trans.ManagerID;
			importedTrans.ManagerCode = trans.ManagerCode;

			if (string.IsNullOrEmpty(importedTrans.ManagerID) == false)
			{
				importedTrans.ManagerCompanyGuid = companies.GetMasterRecordGuid(importSecurity, importedTrans.ManagerID);
			}

			importedTrans.Notes = trans.Notes;

			if (trans.Number01 != null)
			{
				importedTrans.Number01 = trans.Number01.Value;
			}

			if (trans.Number02 != null)
			{
				importedTrans.Number02 = trans.Number02.Value;
			}

			if (trans.Number03 != null)
			{
				importedTrans.Number03 = trans.Number03.Value;
			}

			if (trans.Number04 != null)
			{
				importedTrans.Number04 = trans.Number04.Value;
			}

			if (trans.Number05 != null)
			{
				importedTrans.Number05 = trans.Number05.Value;
			}

			if (trans.Number06 != null)
			{
				importedTrans.Number06 = trans.Number06.Value;
			}

			importedTrans.OperatorID = trans.OperatorID;
			if (string.IsNullOrEmpty(importedTrans.OperatorID) == false)
			{
				importedTrans.OperatorPersonnelGuid = personnel.GetMasterRecordGuid(importSecurity, importedTrans.OperatorID);
			}

			importedTrans.OriginApplication = (TransactionOrigin) trans.OriginApplication;
			
			importedTrans.OwnerID = trans.OwnerID;
			importedTrans.OwnerCode = trans.OwnerCode;

			if (string.IsNullOrEmpty(importedTrans.OwnerID) == false)
			{
				importedTrans.OwnerCompanyGuid = companies.GetMasterRecordGuid(importSecurity, importedTrans.OwnerID);
			}

			importedTrans.PONumber = trans.PONumber;
			importedTrans.PartialCloseout = trans.PartialCloseout;
			importedTrans.PaymentInfo = this.CopyPaymentObject(trans.PaymentInfo);

			importedTrans.ReferenceID = trans.ReferenceID;

			if (trans.RequestedDateTime != null)
			{
				importedTrans.RequestedDateTime = trans.RequestedDateTime.Value;
			}

			if (trans.RequestedDeliveryDate != null)
			{
				importedTrans.RequestedDeliveryDate = trans.RequestedDeliveryDate.Value;
			}

			importedTrans.ReversalType = trans.ReversalType;
			importedTrans.ReversedTransID = trans.ReversedTransID;
			importedTrans.RouteInfo = this.CopyRouteInfo(importSecurity, trans.RouteInfo);
			importedTrans.RouteSchedule = this.CopyRouteSchedule(trans.RouteSchedule);

			importedTrans.SCACCode = trans.SCACCode;

			if (trans.ScheduledDate != null)
			{
				importedTrans.ScheduledDate = trans.ScheduledDate.Value;
			}
			
			importedTrans.ShipToCode = trans.ShipToCode;
			importedTrans.ShipToID = trans.ShipToID;

			if (string.IsNullOrEmpty(importedTrans.ShipToID) == false)
			{
				importedTrans.ShipToCompanyGuid = companies.GetMasterRecordGuid(importSecurity, importedTrans.ShipToID);
			}

			importedTrans.ShipmentNumber = trans.ShipmentNumber;

			importedTrans.ShipperCode = trans.ShipperCode;
			importedTrans.ShipperID = trans.ShipperID;

			if (string.IsNullOrEmpty(importedTrans.ShipperID) == false)
			{
				importedTrans.ShipperCompanyGuid = companies.GetMasterRecordGuid(importSecurity, importedTrans.ShipperID);
			}

			importedTrans.ShippingDocumentNumber = trans.ShippingDocumentNumber;

			importedTrans.Signature = trans.Signature;

			importedTrans.Site = trans.Site;

			if (string.IsNullOrEmpty(trans.SourceEQ1ID) == false)
			{
				importedTrans.SourceEQ1.EquipmentGuid = equipments.GetMasterRecordGuid(importSecurity, trans.SourceEQ1ID);
			}

			if (string.IsNullOrEmpty(trans.SourceEQ2ID) == false)
			{
				importedTrans.SourceEQ2.EquipmentGuid = equipments.GetMasterRecordGuid(importSecurity, trans.SourceEQ2ID);
			}

			if (string.IsNullOrEmpty(trans.SourceEQ3ID) == false)
			{
				importedTrans.SourceEQ3.EquipmentGuid = equipments.GetMasterRecordGuid(importSecurity, trans.SourceEQ3ID);
			}

			importedTrans.Status = (TransactionStatus) trans.Status;

			importedTrans.SubType = trans.SubType;

			if (trans.SubmittedToAccounting != null)
			{
				importedTrans.SubmittedToAccounting = trans.SubmittedToAccounting.Value;
			}

			importedTrans.SupplierCode = trans.SupplierCode;
			importedTrans.SupplierID = trans.SupplierID;

			if (string.IsNullOrEmpty(importedTrans.SupplierID) == false)
			{
				importedTrans.SupplierCompanyGuid = companies.GetMasterRecordGuid(importSecurity, importedTrans.SupplierID);
			}

			importedTrans.TicketMode = (TicketModes)trans.TicketMode;
			importedTrans.TicketSource = trans.TicketSource;

			if (trans.TimeEnd != null)
			{
				importedTrans.TimeEnd = trans.TimeEnd.Value;
			}

			if (trans.TimeIn != null)
			{
				importedTrans.TimeIn = trans.TimeIn.Value;
			}

			if (trans.TimeOut != null)
			{
				importedTrans.TimeOut = trans.TimeOut.Value;
			}

			importedTrans.TransID = trans.TransID;
			importedTrans.ToCarrierID = trans.ToCarrierID;
			importedTrans.ToManagerID = trans.ToManagerID;
			importedTrans.ToOwnerID = trans.ToOwnerID;

			if (trans.TransPIDXCollection != null)
			{
				importedTrans.TransPIDXCollection = this.CopyPidxCollection(importSecurity, trans.TransPIDXCollection, transactionProcessor);
			}

			importedTrans.TransRefID = trans.TransRefID;
			importedTrans.TransTypeID = (TransactionTypes) trans.TransTypeID;
			importedTrans.TransVersion = trans.TransVersion;

			if (trans.TransactionDateTime != null)
			{
				importedTrans.TransactionDateTime = trans.TransactionDateTime.Value;
			}

			if (trans.TransportInfoList != null)
			{
				importedTrans.TransportInfoList = this.CopyTransportInfoList(importSecurity, trans.TransportInfoList, transactionProcessor);
			}

			importedTrans.UserData1 = trans.UserData1;
			importedTrans.UserData2 = trans.UserData2;
			importedTrans.UserData3 = trans.UserData3;
			importedTrans.UserData4 = trans.UserData4;
			importedTrans.UserData5 = trans.UserData5;
			importedTrans.UserData6 = trans.UserData6;
			importedTrans.UserData7 = trans.UserData7;
			importedTrans.UserData8 = trans.UserData8;
			importedTrans.UserData9 = trans.UserData9;

			importedTrans.UserData10 = trans.UserData10;
			importedTrans.UserData11 = trans.UserData11;
			importedTrans.UserData12 = trans.UserData12;
			importedTrans.UserData13 = trans.UserData13;
			importedTrans.UserData14 = trans.UserData14;
			importedTrans.UserData15 = trans.UserData15;
			importedTrans.UserData16 = trans.UserData16;
			importedTrans.UserData17 = trans.UserData17;
			importedTrans.UserData18 = trans.UserData18;
			importedTrans.UserData19 = trans.UserData19;

			importedTrans.UserData20 = trans.UserData20;
			importedTrans.UserData21 = trans.UserData21;
			importedTrans.UserData22 = trans.UserData22;
			importedTrans.UserData23 = trans.UserData23;
			importedTrans.UserData24 = trans.UserData24;

			if (trans.WeightReadings != null)
			{
				importedTrans.WeightReadings = this.CopyWeightReadings(trans.WeightReadings);
			}

			// Now do lineitems and sublineitems
			importedTrans.LineItems.Clear();
			foreach (FM7Accounting.LineItemDO lineItem in trans.LineItems)
			{
				this.ImportLineItem(importSecurity, importedTrans, lineItem, equipments, personnel, meters, tanks, products);
			}

			return importedTrans;
		}

		/// <summary>
		/// Imports the line item.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="trans">The transaction object.</param>
		/// <param name="lineItem">The line item to import.</param>
		/// <param name="equipments">The equipments service interface.</param>
		/// <param name="personnel">The personnel service interface.</param>
		/// <param name="meters">The meters service interface.</param>
		/// <param name="tanks">The tanks service interface.</param>
		/// <param name="products">The products service interface.</param>
		private void ImportLineItem(
			SecurityClass security,
			TransactionDO trans,
			FM7Accounting.LineItemDO lineItem,
			IEquipments equipments,
			IPersonnel personnel,
			IMeters meters,
			ITanks tanks,
			IProducts products)
		{
		    var addItem = new LineItemDO
		                  {
		                      TransactionLineItemGuid = Guid.Empty,
		                      AcknowledgedDateTime = lineItem.AcknowledgedDateTime.Value,
		                      AdditiveProfileID = lineItem.AdditiveProfileID
		                  };


		    if (string.IsNullOrEmpty(addItem.AdditiveProfileID) == false)
			{
				addItem.AdditiveProfileGuid =
					FMChannelHelper.MakeCall<IAdditiveProfiles, Guid>(x => x.GetIdentityGuid(security, addItem.AdditiveProfileID));
			}

			addItem.ArmNumber = (int?)lineItem.ArmNumber.Value;
			addItem.AlternativeGrossVolume = lineItem.AlternativeGrossVolume.Value;
			addItem.AlternativeNetVolume = lineItem.AlternativeNetVolume.Value;
			addItem.AlternativeUnits = (int?)lineItem.AlternativeUnits.Value;

			addItem.AssociatedTransactions.Clear();
			this.CopyAssociatedTransactions(
				addItem.AssociatedTransactions, lineItem.AssociatedTransactions, addItem.TransactionLineItemGuid);

			addItem.BatchNumber = lineItem.BatchNumber;
			addItem.BottomVolume = lineItem.BottomVolume.Value;

			addItem.CLIN = lineItem.CLIN;
			addItem.COAID = lineItem.COAID;
			addItem.COANote = lineItem.COANote;
			addItem.COAWaiver = lineItem.COAWaiver;
			addItem.CleanLineDeductProduct = lineItem.CleanLineDeductProduct;

			if (lineItem.CleanLineDeductQuantity != null)
			{
				addItem.CleanLineDeductQuantity = lineItem.CleanLineDeductQuantity.Value;
			}

			if (lineItem.CleanLinePackQuantity != null)
			{
				addItem.CleanLinePackQuantity = lineItem.CleanLinePackQuantity.Value;
			}

			addItem.CleanLineProduct = lineItem.CleanLineProduct;

			if (lineItem.CloseoutDate != null)
			{
				addItem.CloseoutDate = lineItem.CloseoutDate.Value;
			}

			if (lineItem.CompartmentsEmpty != null)
			{
				addItem.CompartmentsEmpty = lineItem.CompartmentsEmpty.Value;
			}

			if (lineItem.CompartmentsPreviouslyLoaded != null)
			{
				addItem.CompartmentsPreviouslyLoaded = lineItem.CompartmentsPreviouslyLoaded.Value;
			}

			if (lineItem.CompletionDateTime != null)
			{
				addItem.CompletionDateTime = lineItem.CompletionDateTime.Value;
			}

			if (lineItem.ContaminatePrompt != null)
			{
				addItem.ContaminatePrompt = lineItem.ContaminatePrompt.Value;
			}

			addItem.ContractNumber = lineItem.ContractNumber;
			addItem.CustomerProductCode = lineItem.CustomerProductCode;
			addItem.CustomerProductName = lineItem.CustomerProductName;

			if (lineItem.Date01 != null)
			{
				addItem.Date01 = lineItem.Date01.Value;
			}

			if (lineItem.Date02 != null)
			{
				addItem.Date02 = lineItem.Date02.Value;
			}

			if (lineItem.Date03 != null)
			{
				addItem.Date03 = lineItem.Date03.Value;
			}

			if (lineItem.Date04 != null)
			{
				addItem.Date04 = lineItem.Date04.Value;
			}

			addItem.DeleteFlag = lineItem.DeleteFlag;
			addItem.DeliveryLocation = lineItem.DeliveryLocation;

			if (lineItem.Density != null)
			{
				addItem.Density = lineItem.Density.Value;
			}
			
			addItem.DestinationCompartmentID = lineItem.DestinationCompartmentID;

			if (string.IsNullOrEmpty(addItem.DestinationCompartmentID) == false)
			{
				addItem.DestinationCompartmentEquipmentGuid = equipments.GetMasterRecordGuid(
					security, addItem.DestinationCompartmentID);
			}

			if (lineItem.DestinationEQ != null && string.IsNullOrEmpty(lineItem.DestinationEQ.EquipmentRefID) == false)
			{
				addItem.DestinationEQ.EquipmentGuid = equipments.GetMasterRecordGuid(
					security, lineItem.DestinationEQ.EquipmentRefID);
			}

			if (lineItem.DifferentialPressure != null)
			{
				addItem.DifferentialPressure = lineItem.DifferentialPressure.Value;
			}

			if (lineItem.DispatchedDateTime != null)
			{
				addItem.DispatchedDateTime = lineItem.DispatchedDateTime.Value;
			}

			addItem.DocumentNumber = lineItem.DocumentNumber;

			if (lineItem.EndDeliveryDate != null)
			{
				addItem.EndDeliveryDate = lineItem.EndDeliveryDate.Value;
			}

			addItem.EngineeringUnitsIndex = (EngineeringUnit) lineItem.EngineeringUnitsIndex;

			if (lineItem.ExchangeRate != null)
			{
				addItem.ExchangeRate = lineItem.ExchangeRate.Value;
			}

			addItem.Flag01 = lineItem.Flag01;
			addItem.Flag02 = lineItem.Flag02;
			addItem.Flag03 = lineItem.Flag03;
			addItem.Flag04 = lineItem.Flag04;
			addItem.Flag05 = lineItem.Flag05;
			addItem.Flag06 = lineItem.Flag06;

			if (lineItem.FreezePoint != null)
			{
				addItem.FreezePoint = lineItem.FreezePoint.Value;
			}

			addItem.GrossQuantityReceived = lineItem.GrossQuantityReceived;
			addItem.GrossQuantityRemaining = lineItem.GrossQuantityRemaining;

			if (lineItem.ImproperAdditization != null)
			{
				addItem.ImproperAdditization = lineItem.ImproperAdditization.Value;
			}

			addItem.InvoiceLineNumber = lineItem.InvoiceLineNumber;
			addItem.InvoiceNumber = lineItem.InvoiceNumber;

			if (lineItem.LineFill != null)
			{
				addItem.LineFill = lineItem.LineFill.Value;
			}

			if (lineItem.LineNumber != null)
			{
				addItem.LineNumber = (int?) lineItem.LineNumber.Value;
			}

			if (lineItem.LoadRackVariance != null)
			{
				addItem.LoadRackVariance = lineItem.LoadRackVariance.Value;
			}
			
			addItem.LoadingLocationID = lineItem.LoadingLocationID;
			if (string.IsNullOrEmpty(addItem.LoadingLocationID) == false)
			{
				addItem.LoadingLocationStationGuid =
					FMChannelHelper.MakeCall<IStations, Guid>(x => x.GetIdentityGuid(security, addItem.LoadingLocationID));
			}

			addItem.MeterID = lineItem.MeterID;
			addItem.MeterGuid = meters.GetIdentityGuid(security, addItem.MeterID);
			addItem.MeterReading = this.CopyMeterReading(lineItem.MeterReading);

			if (lineItem.NetCapacity != null)
			{
				addItem.NetCapacity = lineItem.NetCapacity.Value;
			}

			addItem.NetQuantityReceived = lineItem.NetQuantityReceived;
			addItem.NetQuantityRemaining = lineItem.NetQuantityRemaining;

			if (lineItem.NonDomesticPrice != null)
			{
				addItem.NonDomesticPrice = lineItem.NonDomesticPrice.Value;
			}

			if (lineItem.Number01 != null)
			{
				addItem.Number01 = lineItem.Number01.Value;
			}

			if (lineItem.Number02 != null)
			{
				addItem.Number02 = lineItem.Number02.Value;
			}

			if (lineItem.Number03 != null)
			{
				addItem.Number03 = lineItem.Number03.Value;
			}

			if (lineItem.Number04 != null)
			{
				addItem.Number04 = lineItem.Number04.Value;
			}

			if (lineItem.Number05 != null)
			{
				addItem.Number05 = lineItem.Number05.Value;
			}

			if (lineItem.Number06 != null)
			{
				addItem.Number06 = lineItem.Number06.Value;
			}

			if (lineItem.Odometer != null)
			{
				addItem.Odometer = lineItem.Odometer.Value;
			}

			if (lineItem.OdometerHours != null)
			{
				addItem.OdometerHours = lineItem.OdometerHours.Value;
			}

			if (lineItem.OnLocationTime != null)
			{
				addItem.OnLocationTime = lineItem.OnLocationTime.Value;
			}
			
			addItem.OperatorID = lineItem.OperatorID;

			if (string.IsNullOrEmpty(addItem.OperatorID) == false)
			{
				addItem.OperatorPersonnelGuid = personnel.GetMasterRecordGuid(security, addItem.OperatorID);
			}

			if (lineItem.PartialFill != null)
			{
				addItem.PartialFill = lineItem.PartialFill.Value;
			}

			addItem.Pit = lineItem.Pit;
			
			if (lineItem.PresetAmount != null)
			{
				addItem.PresetAmount = lineItem.PresetAmount.Value;
			}

			addItem.Product = lineItem.Product;
			addItem.ProductCode = lineItem.ProductCode;
			
			if (lineItem.ProductPrice != null)
			{
				addItem.ProductPrice = lineItem.ProductPrice.Value;
			}

			addItem.ProductType = lineItem.ProductType;
			addItem.ProductGuid = products.GetMasterRecordGuidFromID(security, addItem.Product);

			addItem.Quality = (TransactionQuality) lineItem.Quality;
			addItem.QualityTestNumber = lineItem.QualityTestNumber;

			if (lineItem.ReceiptVariance != null)
			{
				addItem.ReceiptVariance = lineItem.ReceiptVariance.Value;
			}

			addItem.RequestedBy = lineItem.RequestedBy;
			
			if (lineItem.RequestedDateTime != null)
			{
				addItem.RequestedDateTime = lineItem.RequestedDateTime.Value;
			}

			if (lineItem.RequestedDeliveryDate != null)
			{
				addItem.RequestedDeliveryDate = lineItem.RequestedDeliveryDate.Value;
			}

			addItem.SequenceId = lineItem.SequenceNumber;
			addItem.SourceCompartmentID = lineItem.SourceCompartmentID;

			if (string.IsNullOrEmpty(addItem.SourceCompartmentID) == false)
			{
				addItem.SourceCompartmentEquipmentGuid = equipments.GetMasterRecordGuid(security, addItem.SourceCompartmentID);
			}

			if (lineItem.SourceEQ != null && string.IsNullOrEmpty(lineItem.SourceEQ.EquipmentRefID) == false)
			{
				addItem.SourceEQ.EquipmentGuid = equipments.GetMasterRecordGuid(security, lineItem.SourceEQ.EquipmentRefID);
			}

			if (lineItem.SpecialInstructionsNote != null)
			{
				addItem.SpecialInstructionsNote = lineItem.SpecialInstructionsNote.Note;
			}

			if (lineItem.SplashBlendingMap != null)
			{
				addItem.SplashBlendingMap = this.CopyProductMap(security, lineItem.SplashBlendingMap, meters);
			}

			addItem.Status = (TransactionStatus) lineItem.Status;
			addItem.StorageLocationID = lineItem.StorageLocationID;

			if (string.IsNullOrWhiteSpace(addItem.StorageLocationID) == false)
			{
				addItem.StorageLocationTankGuid = tanks.GetIdentityGuid(security, addItem.StorageLocationID);
			}

			if (lineItem.TankLevel != null)
			{
				addItem.TankLevel = lineItem.TankLevel.Value;
			}

			if (lineItem.TankLevelUnits != null)
			{
				addItem.TankLevelUnits = (int?) lineItem.TankLevelUnits.Value;
			}

			addItem.TankStatus = lineItem.TankStatus;

			if (lineItem.Tax1 != null)
			{
				addItem.Tax1 = lineItem.Tax1.Value;
			}

			if (lineItem.Tax2 != null)
			{
				addItem.Tax2 = lineItem.Tax2.Value;
			}

			if (lineItem.Tax3 != null)
			{
				addItem.Tax3 = lineItem.Tax3.Value;
			}

			if (lineItem.Tax4 != null)
			{
				addItem.Tax4 = lineItem.Tax4.Value;
			}

			if (lineItem.Tax5 != null)
			{
				addItem.Tax5 = lineItem.Tax5.Value;
			}

			if (lineItem.Temperature != null)
			{
				addItem.Temperature = lineItem.Temperature.Value;
			}

			addItem.TotalPriceWithTax = lineItem.TotalPriceWithTax;
			addItem.TotalValue = lineItem.TotalValue;

			addItem.UserData1 = lineItem.UserData["Transaction Alias Line Item User Data 1"].ToString();
			addItem.UserData2 = lineItem.UserData["Transaction Alias Line Item User Data 2"].ToString();
			addItem.UserData3 = lineItem.UserData["Transaction Alias Line Item User Data 3"].ToString();
			addItem.UserData4 = lineItem.UserData["Transaction Alias Line Item User Data 4"].ToString();
			addItem.UserData5 = lineItem.UserData["Transaction Alias Line Item User Data 5"].ToString();
			addItem.UserData6 = lineItem.UserData["Transaction Alias Line Item User Data 6"].ToString();
			addItem.UserData7 = lineItem.UserData["Transaction Alias Line Item User Data 7"].ToString();
			addItem.UserData8 = lineItem.UserData["Transaction Alias Line Item User Data 8"].ToString();
			addItem.UserData9 = lineItem.UserData["Transaction Alias Line Item User Data 9"].ToString();

			addItem.UserData10 = lineItem.UserData["Transaction Alias Line Item User Data 10"].ToString();
			addItem.UserData11 = lineItem.UserData["Transaction Alias Line Item User Data 11"].ToString();
			addItem.UserData12 = lineItem.UserData["Transaction Alias Line Item User Data 12"].ToString();
			addItem.UserData13 = lineItem.UserData["Transaction Alias Line Item User Data 13"].ToString();
			addItem.UserData14 = lineItem.UserData["Transaction Alias Line Item User Data 14"].ToString();
			addItem.UserData15 = lineItem.UserData["Transaction Alias Line Item User Data 15"].ToString();
			addItem.UserData16 = lineItem.UserData["Transaction Alias Line Item User Data 16"].ToString();
			addItem.UserData17 = lineItem.UserData["Transaction Alias Line Item User Data 17"].ToString();
			addItem.UserData18 = lineItem.UserData["Transaction Alias Line Item User Data 18"].ToString();
			addItem.UserData19 = lineItem.UserData["Transaction Alias Line Item User Data 19"].ToString();

			addItem.UserData20 = lineItem.UserData["Transaction Alias Line Item User Data 20"].ToString();
			addItem.UserData21 = lineItem.UserData["Transaction Alias Line Item User Data 21"].ToString();
			addItem.UserData22 = lineItem.UserData["Transaction Alias Line Item User Data 22"].ToString();
			addItem.UserData23 = lineItem.UserData["Transaction Alias Line Item User Data 23"].ToString();
			addItem.UserData24 = lineItem.UserData["Transaction Alias Line Item User Data 24"].ToString();

			if (lineItem.VCF != null)
			{
				addItem.VCF = lineItem.VCF.Value;
			}

			if (lineItem.ValidationDateTime != null)
			{
				addItem.ValidationDateTime = lineItem.ValidationDateTime.Value;
			}

			addItem.ValueRemaining = lineItem.ValueRemaining;

			if (addItem.Variance != null)
			{
				addItem.Variance = lineItem.Variance.Value;
			}

			// Sublineitems
			addItem.SubLineItems.Clear();
			foreach (FM7Accounting.SubLineItemDO subline in lineItem.SubLineItems)
			{
				SubLineItemDO addSub = this.ImportSubLineItem(security, subline, meters, products, tanks);
				addItem.SubLineItems.Add(addSub);
			}

			// Add the line item to the transaction
			trans.LineItems.Add(addItem);
		}

		/// <summary>
		/// Imports the sub line item.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="subline">The subline item to import.</param>
		/// <param name="meters">The meters service interface.</param>
		/// <param name="products">The products service interface.</param>
		/// <param name="tanks">The tanks service interface.</param>
		/// <returns>A current version sub line item data object with imported data.</returns>
		private SubLineItemDO ImportSubLineItem(SecurityClass security, FM7Accounting.SubLineItemDO subline, IMeters meters, IProducts products, ITanks tanks)
		{
			var addSub = new SubLineItemDO();

			if (subline.ArmNumber != null)
			{
				addSub.ArmNumber = (int?) subline.ArmNumber.Value;
			}

			addSub.BatchNumber = subline.BatchNumber;

			if (subline.BottomVolume != null)
			{
				addSub.BottomVolume = subline.BottomVolume.Value;
			}

			if (subline.BrokenBlend != null)
			{
				addSub.BrokenBlend = subline.BrokenBlend.Value;
			}

			addSub.COAID = subline.COAID;
			addSub.CleanLineDeductProduct = subline.CleanLineDeductProduct;

			if (subline.CleanLineDeductQuantity != null)
			{
				addSub.CleanLineDeductQuantity = subline.CleanLineDeductQuantity.Value;
			}

			if (subline.CleanLinePackQuantity != null)
			{
				addSub.CleanLinePackQuantity = subline.CleanLinePackQuantity.Value;
			}

			addSub.CleanLineProduct = subline.CleanLineProduct;

			if (subline.CloseoutDate != null)
			{
				addSub.CloseoutDate = subline.CloseoutDate.Value;
			}

			addSub.Customs = subline.Customs;

			if (subline.Date01 != null)
			{
				addSub.Date01 = subline.Date01.Value;
			}

			if (subline.Date02 != null)
			{
				addSub.Date02 = subline.Date02.Value;
			}

			if (subline.Date03 != null)
			{
				addSub.Date03 = subline.Date03.Value;
			}

			if (subline.Date04 != null)
			{
				addSub.Date04 = subline.Date04.Value;
			}

			addSub.DeleteFlag = subline.DeleteFlag;

			if (subline.Density != null)
			{
				addSub.Density = subline.Density.Value;
			}

			if (subline.DifferentialPressure != null)
			{
				addSub.DifferentialPressure = subline.DifferentialPressure.Value;
			}

			addSub.Flag01 = subline.Flag01;
			addSub.Flag02 = subline.Flag02;
			addSub.Flag03 = subline.Flag03;
			addSub.Flag04 = subline.Flag04;
			addSub.Flag05 = subline.Flag05;
			addSub.Flag06 = subline.Flag06;

			if (subline.FreezePoint != null)
			{
				addSub.FreezePoint = subline.FreezePoint.Value;
			}

			if (subline.ImproperAdditization != null)
			{
				addSub.ImproperAdditization = subline.ImproperAdditization.Value;
			}

			if (subline.LineFill != null)
			{
				addSub.LineFill = subline.LineFill.Value;
			}

			if (subline.LineNumber != null)
			{
				addSub.LineNumber = (int?) subline.LineNumber.Value;
			}

			addSub.MeterID = subline.MeterID;

			if (string.IsNullOrEmpty(addSub.MeterID) == false)
			{
				addSub.MeterGuid = meters.GetIdentityGuid(security, subline.MeterID);
			}

			addSub.MeterReading = this.CopyMeterReading( subline.MeterReading );

			if (subline.NetCapacity != null)
			{
				addSub.NetCapacity = subline.NetCapacity.Value;
			}

			if (subline.Number01 != null)
			{
				addSub.Number01 = subline.Number01.Value;
			}

			if (subline.Number02 != null)
			{
				addSub.Number02 = subline.Number02.Value;
			}

			if (subline.Number03 != null)
			{
				addSub.Number03 = subline.Number03.Value;
			}

			if (subline.Number04 != null)
			{
				addSub.Number04 = subline.Number04.Value;
			}

			if (subline.Number05 != null)
			{
				addSub.Number05 = subline.Number05.Value;
			}

			if (subline.Number06 != null)
			{
				addSub.Number06 = subline.Number06.Value;
			}

			if (subline.PresetAmount != null)
			{
				addSub.PresetAmount = subline.PresetAmount.Value;
			}

			addSub.Product = subline.Product;
			addSub.ProductCode = subline.ProductCode;
			addSub.ProductType = subline.ProductType;
			addSub.ProductGuid = products.GetMasterRecordGuidFromID( security, subline.Product );

			addSub.Quality = (TransactionQuality) subline.Quality;

			if (subline.SpecialInstructionsNote != null)
			{
				addSub.SpecialInstructionsNote = subline.SpecialInstructionsNote.Note;
			}

			addSub.Status = (TransactionStatus) subline.Status;
			addSub.StorageLocationID = subline.StorageLocationID;
			addSub.StorageLocationTankGuid = tanks.GetIdentityGuid( security, subline.StorageLocationID );

			addSub.TankStatus = subline.TankStatus;

			if (subline.Tax1 != null)
			{
				addSub.Tax1 = subline.Tax1.Value;
			}

			if (subline.Tax2 != null)
			{
				addSub.Tax2 = subline.Tax2.Value;
			}

			if (subline.Tax3 != null)
			{
				addSub.Tax3 = subline.Tax3.Value;
			}

			if (subline.Tax4 != null)
			{
				addSub.Tax4 = subline.Tax4.Value;
			}

			if (subline.Tax5 != null)
			{
				addSub.Tax5 = subline.Tax5.Value;
			}

			if (subline.Temperature != null)
			{
				addSub.Temperature = subline.Temperature.Value;
			}

			addSub.TransactionSubLineItemGuid = Guid.NewGuid();

			if (subline.VCF != null)
			{
				addSub.VCF = subline.VCF.Value;
			}

			this.CopyVolumeToQuantity(addSub.Quantity, subline.Volume);

			return addSub;
		}

		/// <summary>
		/// Copies the volume to quantity.
		/// </summary>
		/// <param name="quantity">The quantity data object.</param>
		/// <param name="volume">The volume data object.</param>
		private void CopyVolumeToQuantity(QuantityDO quantity, VolumeDO volume)
		{
			if (quantity != null && volume != null)
			{
				quantity.AffectsInventory = volume.AffectsInventory;
				quantity.BadGrossQualityLogged = volume.BadGrossQualityLogged;
				quantity.BadNetQualityLogged = volume.BadNetQualityLogged;
				quantity.Gross = volume.Gross;
				quantity.GrossInventoryChange = volume.GrossInventoryChange;
				quantity.GrossPrice = volume.GrossPrice;
				quantity.GrossPriceInventoryChange = volume.GrossPriceInventoryChange;
				quantity.IsGrossDirty = volume.IsGrossDirty;
				quantity.IsNetDirty = volume.IsNetDirty;
				quantity.Net = volume.Net;
				quantity.NetInventoryChange = volume.NetInventoryChange;
				quantity.NetPrice = volume.NetPrice;
				quantity.NetPriceInventoryChange = volume.NetPriceInventoryChange;
				quantity.NullableGross = volume.NullableGross;
				quantity.NullableNet = volume.NullableNet;
			}
		}

		/// <summary>
		/// Copies the associated transactions.
		/// </summary>
		/// <param name="associatedTransactions">The associated transactions.</param>
		/// <param name="baseCollections">The base collections.</param>
		/// <param name="lineItemGuid">The line item GUID.</param>
		private void CopyAssociatedTransactions(List<AssociatedTxDO> associatedTransactions, BaseCollections baseCollections, Guid lineItemGuid)
		{
			foreach (FM7Accounting.AssociatedTxDO trans in baseCollections)
			{
				var addTrans = new AssociatedTxDO
					               {
						               Associated = trans.Associated,
						               BillToID = trans.BillToID,
						               DeliveryLocation = trans.DeliveryLocation,
						               DocumentNumber = trans.DocumentNumber,
						               Excise = trans.Excise,
						               Flags = new BaseLineItemDO.StatusFlags((BaseLineItemDO.Status)trans.Flags.Flags),
						               GST = trans.GST,
						               GrossQuantity = trans.GrossQuantity,
						               InventoryDate = trans.InventoryDate,
						               InventoryDateTime = trans.InventoryDateTime,
						               Manager = trans.Manager,
						               Markup = trans.Markup,
						               Owner = trans.Owner,
						               PONumber = trans.PONumber,
						               Product = trans.Product,
						               ShipToID = trans.ShipToID,
						               Site = trans.Site,
						               SupplierID = trans.SupplierID,
						               TotalPriceWithTax = trans.TotalPriceWithTax,
						               TotalValue = trans.TotalValue,
						               TransID = trans.TransID,
						               TransTypeID = (TransactionTypes)trans.TransTypeID,
						               TransactionAlias = trans.TransactionAlias,
						               TransactionDate = trans.TransactionDate,
						               TransactionDateTime = trans.TransactionDateTime,
						               TransactionLineItemGuid = lineItemGuid
					               };

				associatedTransactions.Add(addTrans);
			}
		}

		/// <summary>
		/// Copies the meter reading.
		/// </summary>
		/// <param name="meterReading">The meter reading.</param>
		/// <returns>A current version meter reading data object with imported data.</returns>
		private MeterReadingDO CopyMeterReading(FM7Accounting.MeterReadingDO meterReading)
		{
			var addReading = new MeterReadingDO();

			if (meterReading.MeterFactor != null)
			{
				addReading.MeterFactor = meterReading.MeterFactor.Value;
			}

			if (meterReading.MeterStart != null)
			{
				addReading.MeterStart = meterReading.MeterStart.Value;
			}

			if (meterReading.MeterStop != null)
			{
				addReading.MeterStop = meterReading.MeterStop.Value;
			}

			if (meterReading.StartDateTime != null)
			{
				addReading.StartDateTime = meterReading.StartDateTime.Value;
			}

			if (meterReading.StopDateTime != null)
			{
				addReading.StopDateTime = meterReading.StopDateTime.Value;
			}
		
			return addReading;
		}

		/// <summary>
		/// Copies the weight readings.
		/// </summary>
		/// <param name="weightReadings">The weight readings to copy.</param>
		/// <returns>A list of weight reading data objects with imported data.</returns>
		private List<WeightReadingDO> CopyWeightReadings(ArrayList weightReadings)
		{
			var list = new List<WeightReadingDO>();

			foreach (FM7Accounting.WeightReadingDO item in weightReadings)
			{
				var reading = new WeightReadingDO { CompartmentName = item.CompartmentName };

				if (item.BeginQuantity != null)
				{
					reading.BeginQuantity = item.BeginQuantity.Value;
				}

				if (item.FinalQuantity != null)
				{
					reading.FinalQuantity = item.FinalQuantity.Value;
				}

				if (item.RequestedQuantity != null)
				{
					reading.RequestedQuantity = item.RequestedQuantity.Value;
				}

				list.Add(reading);
			}

			return list;
		}

		/// <summary>
		/// Copies the pidx collection.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="transPIDXCollection">The trans PIDX collection.</param>
		/// <param name="proc">The proc to look up transactions.</param>
		/// <returns>
		/// A list of transaction pidx data objects with imported data.
		/// </returns>
		private List<TransactionPIDXDO> CopyPidxCollection(SecurityClass security, ArrayList transPIDXCollection, ITransactionProcessor proc)
		{
			var list = new List<TransactionPIDXDO>();

			foreach (FM7Accounting.TransactionPIDXDO item in transPIDXCollection)
			{
				var addItem = new TransactionPIDXDO
					              {
						              AuthorizationNumber = item.AuthorizationNumber,
						              BrokenBlend = item.BrokenBlend,
						              SentFlag = item.SentFlag,
						              TransID = item.TransID
					              };

				var trans = proc.Process(new TransactionSR { Security = security, TransID = addItem.TransID });
				if (trans != null)
				{
					addItem.TransactionGuid = trans.TransactionGuid;
				}

				list.Add( addItem );
			}

			return list;
		}

		/// <summary>
		/// Copies the transport info list.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="transportInfoList">The transport info list.</param>
		/// <param name="proc">The proc to look up transactions.</param>
		/// <returns>
		/// A collection of transport line item data objects.
		/// </returns>
		private List<TransportLineItemDO> CopyTransportInfoList(SecurityClass security, ArrayList transportInfoList, ITransactionProcessor proc)
		{
			var list = new List<TransportLineItemDO>();

			foreach (FM7Accounting.TransportLineItemDO item in transportInfoList)
			{
				var addItem = new TransportLineItemDO
					              {
						              Address1 = item.Address1,
						              Address2 = item.Address2,
						              City = item.City,
						              LocationName = item.LocationName,
						              POCName = item.POCName,
						              POCPhone = item.POCPhone,
						              State = item.State,
						              TransportOrderNumber = item.TransportOrderNumber,
						              TransVersion = item.TransVersion,
						              Zip = item.Zip
					              };

				var trans = proc.Process(new TransactionSR { TransID = item.TransID, Security = security });
				if (trans != null)
				{
					addItem.TransactionGuid = trans.TransactionGuid;
					list.Add( addItem );
				}
			}

			return list;
		}

		/// <summary>
		/// Copies the route schedule object.
		/// </summary>
		/// <param name="routeSchedule">The route schedule object to copy.</param>
		/// <returns>A current version route schedule object with imported data.</returns>
		private RouteScheduleDO CopyRouteSchedule(FM7Accounting.RouteScheduleDO routeSchedule)
		{
			var addSched = new RouteScheduleDO();

			if (routeSchedule.ETA != null)
			{
				addSched.ETA = routeSchedule.ETA.Value;
			}

			if (routeSchedule.ETD != null)
			{
				addSched.ETD = routeSchedule.ETD.Value;
			}

			if (routeSchedule.FST != null)
			{
				addSched.FST = routeSchedule.FST.Value;
			}

			if (routeSchedule.SFT != null)
			{
				addSched.SFT = routeSchedule.SFT.Value;
			}

			if (routeSchedule.STD != null)
			{
				addSched.STD = routeSchedule.STD.Value;
			}

			if (routeSchedule.STA != null)
			{
				addSched.STA = routeSchedule.STA.Value;
			}

			return addSched;
		}

		/// <summary>
		/// Copies the route info object.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="routeInfo">The route info to copy.</param>
		/// <returns>A current version route info object with imported data.</returns>
		private RouteInfoDO CopyRouteInfo(SecurityClass security, FM7Accounting.RouteInfoDO routeInfo)
		{
			var addRoute = new RouteInfoDO
				               {
					               FinalStationIATAID = routeInfo.FinalStationID,
					               InternationalRouteIndicator = routeInfo.InternationalRouteIndicator,
					               NextStationIATAID = routeInfo.NextStationID,
					               OriginStationIATAID = routeInfo.OriginStationID,
					               PreviousStationIATAID = routeInfo.PreviousStationID,
					               RoutingID = routeInfo.RoutingID
				               };

			if (routeInfo.RouteOriginationDate != null)
			{
				addRoute.RouteOriginationDate = routeInfo.RouteOriginationDate.Value;
			}

			FMChannelHelper.MakeCall<IIATACodes>(
				codes =>
					{
						addRoute.FinalStationIATAGuid = codes.GetIdentityGuid(security, addRoute.FinalStationIATAID);
						addRoute.NextStationIATAGuid = codes.GetIdentityGuid(security, addRoute.NextStationIATAID);
						addRoute.OriginStationIATAGuid = codes.GetIdentityGuid(security, addRoute.OriginStationIATAID);
						addRoute.PreviousStationIATAGuid = codes.GetIdentityGuid(security, addRoute.PreviousStationIATAID);
					});

			return addRoute;
		}

		/// <summary>
		/// Copies the payment object to a current version object.
		/// </summary>
		/// <param name="paymentInfo">The payment info object to copy.</param>
		/// <returns>A current version payment info data object with imported information.</returns>
		private PaymentInfoDO CopyPaymentObject(FM7Accounting.PaymentInfoDO paymentInfo)
		{
			var addPayment = new PaymentInfoDO
				                 {
					                 BillTo = paymentInfo.BillTo,
					                 CashCurrencyType = paymentInfo.CashCurrencyType,
					                 CreditCardCurrencyType = paymentInfo.CreditCardCurrencyType,
					                 CreditCardName = paymentInfo.CreditCardName,
					                 CreditCardNumber = paymentInfo.CreditCardName,
					                 CreditCardType = paymentInfo.CreditCardType
				                 };

			if (paymentInfo.CashAmount != null)
			{
				addPayment.CashAmount = paymentInfo.CashAmount.Value;
			}
			
			if (paymentInfo.CreditCardAmount != null)
			{
				addPayment.CreditCardAmount = paymentInfo.CreditCardAmount.Value;
			}

			if (paymentInfo.CreditCardExpiration != null)
			{
				addPayment.CreditCardExpiration = paymentInfo.CreditCardExpiration.Value;
			}

			return addPayment;
		}

		/// <summary>
		/// Imports the application string.
		/// </summary>
		/// <param name="applicationString">The application string.</param>
		/// <returns>A current version application string object with imported data.</returns>
		private ApplicationStringClass ImportApplicationString( ConsolidatedDataObjects.ApplicationStringClass applicationString )
		{
			var importedString = new ApplicationStringClass
				                     {
					                     ID = applicationString.ID,
					                     Type = (STRING_TYPE)applicationString.Type
				                     };

			return importedString;
		}

		/// <summary>
		/// Imports the transaction alias record.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="alias">The 7.5.2 alias record.</param>
		/// <param name="aliases">The alias interface to use for service calls.</param>
		/// <param name="meters">The meter interface to use for service calls.</param>
		/// <returns>A current version transaction alias class with imported data.</returns>
		private TransactionAliasClass ImportTransactionAlias(
			SecurityClass security, ConsolidatedDataObjects.TransactionAliasClass alias, ITransactionAliases aliases, IMeters meters )
		{
			var importedAlias = new TransactionAliasClass();

			var guid = aliases.GetMasterRecordGuid( security, alias.ID );

			if ( guid != Guid.Empty )
			{
				importedAlias = aliases.Get( security, guid, byUser: false );
			}

			importedAlias.ID = alias.ID;
			importedAlias.TransTypeID = (TransactionTypes) alias.TransTypeID;
			importedAlias.MeterCloseout = alias.MeterCloseout;
			importedAlias.BulkShipment = alias.BulkShipment;
			importedAlias.DistributedImpact = alias.DistributedImpact;
			importedAlias.MultipleLineItems = alias.MultipleLineItems;
			importedAlias.LineItemEditControl = alias.LineItemEditControl;
			importedAlias.MultipleWeightReadings = alias.MultipleWeightReadings;
			importedAlias.LimitSelectionsBasedOnHierarchy = alias.LimitSelectionsBasedOnHierarchy;
			importedAlias.WeightReadingEditControl = alias.WeightReadingEditControl;
			importedAlias.MultipleTransportLineItems = alias.MultipleTransportLineItems;
			importedAlias.AssociatedReport = alias.AssociatedReport;
			importedAlias.AssociatedPreloadReport = alias.AssociatedPreloadReport;
			importedAlias.UseComboxControls = alias.UseComboxControls;
			importedAlias.LookupDefaultStatusIndex = alias.DefaultStatus;

			importedAlias.ExcludedProductCollection.Clear();
			foreach ( ConsolidatedDataObjects.ProductMapClass map in alias.ExcludedProductCollection )
			{
				var addMap = this.CopyProductMap( security, map, meters );
				importedAlias.ExcludedProductCollection.Add( addMap );
			}

			// User Data Fields
			importedAlias.UserDataFieldCollection.Clear();
			foreach ( var fieldClass in alias.UserDataFieldCollection )
			{
			    var field = (ConsolidatedDataObjects.UserDataFieldClass)fieldClass;
			    var addField = new UserDataFieldClass
				{
					AliasName = field.AliasName,
					ClearOnNew = true,
					DbName = field.DbName,
					Deleted = field.Deleted,
					DisplayOrder = field.DisplayOrder,
					FieldRequired = field.FieldRequired,
					ID = field.ID,
					Number = field.Number,
					UserDataType = (USER_DATA_TYPE) field.Type
				};

				foreach ( ConsolidatedDataObjects.UserDataListValueClass item in field.UserDataListValueCollection )
				{
					var addValue = new UserDataListValueClass
					{
						Deleted = item.Deleted,
						ID = item.ID // value stored in ID property
					};

					addField.UserDataListValueCollection.Add( addValue );
				}

				importedAlias.UserDataFieldCollection.Add( addField );
			}

			// Line item user data fields
			importedAlias.LineItemUserDataFieldCollection.Clear();
			foreach ( var fieldClass in alias.LineItemUserDataFieldCollection )
			{
			    var field = (ConsolidatedDataObjects.UserDataFieldClass)fieldClass;
			    var addField = new UserDataFieldClass
				{
					AliasName = field.AliasName,
					ClearOnNew = true,
					DbName = field.DbName,
					Deleted = field.Deleted,
					DisplayOrder = field.DisplayOrder,
					FieldRequired = field.FieldRequired,
					ID = field.ID,
					Number = field.Number,
					UserDataType = (USER_DATA_TYPE) field.Type
				};

				foreach ( ConsolidatedDataObjects.UserDataListValueClass item in field.UserDataListValueCollection )
				{
					var addValue = new UserDataListValueClass
					{
						Deleted = item.Deleted,
						ID = item.ID // value stored in ID property
					};

					addField.UserDataListValueCollection.Add( addValue );
				}

				importedAlias.LineItemUserDataFieldCollection.Add( addField );
			}

			importedAlias.TransactionFieldCollection.Clear();
			foreach ( var fieldClass in alias.TransactionFieldCollection )
			{
			    var field = (ConsolidatedDataObjects.TransactionAliasFieldClass)fieldClass;
			    var addField = this.CreateCopyAliasField( field );
				importedAlias.TransactionFieldCollection.Add( addField );
			}

			importedAlias.LineItemFieldCollection.Clear();
			foreach ( var fieldClass in alias.LineItemFieldCollection )
			{
			    var field = (ConsolidatedDataObjects.TransactionAliasFieldClass)fieldClass;
			    var addField = this.CreateCopyAliasField( field );
				importedAlias.LineItemFieldCollection.Add( addField );
			}

			importedAlias.WeightReadingFieldCollection.Clear();
			foreach ( var fieldClass in alias.WeightReadingFieldCollection )
			{
			    var field = (ConsolidatedDataObjects.TransactionAliasFieldClass)fieldClass;
			    var addField = this.CreateCopyAliasField( field );
				importedAlias.WeightReadingFieldCollection.Add( addField );
			}

			importedAlias.NoteFieldCollection.Clear();
			foreach ( var fieldClass in alias.NoteFieldCollection )
			{
			    var field = (ConsolidatedDataObjects.TransactionAliasFieldClass)fieldClass;
			    var addField = this.CreateCopyAliasField( field );
				importedAlias.NoteFieldCollection.Add( addField );
			}

			// Not enough information to import group transaction alias maps.
			// Load Company Name setting
			importedAlias.ShowCompanyName = (TRANSACTION_SHOW_COMPANY_NAME) alias.ShowCompanyName;

			importedAlias.AssignedStatuses.Clear();
			foreach ( var item in alias.AssignedStatuses )
			{
				importedAlias.AssignedStatuses.Add( item );
			}

			importedAlias.AggregateAssociatedTransactions = alias.AggregateAssociatedTransactions;

			// Copy associated aliases
			importedAlias.AssociatedAliases.Clear();
			foreach ( ConsolidatedDataObjects.TransactionAliasClass associatedAlias in alias.AssociatedAliases )
			{
				// look up the alias
				var childGuid = aliases.GetMasterRecordGuid( security, associatedAlias.ID );
				if ( childGuid != Guid.Empty )
				{
					var addAlias = new TransactionAliasClass { IdentityGuid = childGuid };
					importedAlias.AssociatedAliases.Add( addAlias );
				}
			}

			importedAlias.EnableTotalQtyExceededWarning = alias.EnableTotalQtyExceededWarning;
			importedAlias.EnableTotalValueExceededWarning = alias.EnableTotalValueExceededWarning;
			importedAlias.EnableQtyToleranceExceededWarning = alias.EnableQtyToleranceExceededWarning;
			importedAlias.EnableValueToleranceExceededWarning = alias.EnableValueToleranceExceededWarning;

			importedAlias.LevelUnits = (EngineeringUnit) alias.LevelUnits;
			importedAlias.TemperatureUnits = (EngineeringUnit) alias.TemperatureUnits;
			importedAlias.DensityUnits = (EngineeringUnit) alias.DensityUnits;
			importedAlias.PressureUnits = (EngineeringUnit) alias.PressureUnits;
			importedAlias.FlowUnits = (EngineeringUnit) alias.FlowUnits;
			importedAlias.VolumeUnits = (EngineeringUnit) alias.VolumeUnits;
			importedAlias.MassUnits = (EngineeringUnit) alias.MassUnits;
			importedAlias.AdditiveVolumeUnits = (EngineeringUnit) alias.AdditiveVolumeUnits;

			importedAlias.LevelDecimalPlaces = alias.LevelDecimalPlaces;
			importedAlias.TemperatureDecimalPlaces = alias.TemperatureDecimalPlaces;
			importedAlias.DensityDecimalPlaces = alias.DensityDecimalPlaces;
			importedAlias.PressureDecimalPlaces = alias.PressureDecimalPlaces;
			importedAlias.FlowDecimalPlaces = alias.FlowDecimalPlaces;
			importedAlias.VolumeDecimalPlaces = alias.VolumeDecimalPlaces;
			importedAlias.MassDecimalPlaces = alias.MassDecimalPlaces;
			importedAlias.AdditiveVolumeDecimalPlaces = alias.AdditiveVolumeDecimalPlaces;

			return importedAlias;
		}

		/// <summary>
		/// Creates the copy alias field.
		/// </summary>
		/// <param name="field">The 7.5 field to copy.</param>
		/// <returns>A new field object.</returns>
		private TransactionAliasFieldClass CreateCopyAliasField( ConsolidatedDataObjects.TransactionAliasFieldClass field )
		{
			var addField = new TransactionAliasFieldClass
			{
				AliasName = field.AliasName,
				DbName = field.DbName,
				DefaultAssigned = field.DefaultAssigned,
				Deleted = field.Deleted,
				DisplayName = field.DisplayName,
				DisplayOrder = field.DisplayOrder,
				FieldRequired = field.FieldRequired,
				ID = field.ID,
				UserGroupID = field.UserGroupID,
				Type = (TransactionFieldType) field.Type
			};

			return addField;
		}

		/// <summary>
		/// Imports the personnel record.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="person">The 7.5.2 person record.</param>
		/// <param name="personnel">The personnel interface to use for service calls..</param>
		/// <returns>A current version person class with imported data.</returns>
		private PersonClass ImportPersonnelProfile(
			SecurityClass security, ConsolidatedDataObjects.PersonClass person, IPersonnel personnel )
		{
			var importedPerson = new PersonClass();

			var guid = personnel.GetMasterRecordGuid( security, person.ID );

			if ( guid != Guid.Empty )
			{
				importedPerson = personnel.Get( security, guid );
			}

			importedPerson.ID = person.ID;
			importedPerson.CardNumber = person.CardNumber;
			importedPerson.FirstName = person.FirstName;
			importedPerson.MiddleName = person.MiddleName;
			importedPerson.LastName = person.LastName;
			importedPerson.Title = person.Title;
			importedPerson.Department = person.Department;
			importedPerson.Address1 = person.Address1;
			importedPerson.Address2 = person.Address2;
			importedPerson.City = person.City;
			importedPerson.State = person.State;
			importedPerson.Zip = person.Zip;
			importedPerson.Country = person.Country;
			importedPerson.Phone1 = person.Phone1;
			importedPerson.Phone2 = person.Phone2;
			importedPerson.AssignmentDate = person.AssignmentDate;
			importedPerson.SupervisionDate = person.SupervisionDate;
			importedPerson.SSAN = person.SSAN;
			importedPerson.BirthDate = person.BirthDate;
			importedPerson.PayRate = person.PayRate;
			importedPerson.LaborRate1 = person.LaborRate1;
			importedPerson.LaborRate2 = person.LaborRate2;
			importedPerson.LaborRate3 = person.LaborRate3;
			importedPerson.LaborRate4 = person.LaborRate4;
			importedPerson.Status = (PersonClass.STATUS) person.Status;
			importedPerson.Email = person.Email;
			importedPerson.ResponsibleOfficer = person.ResponsibleOfficer;
			importedPerson.Shift = person.Shift;
			importedPerson.PINNumber = person.PINNumber;
			importedPerson.PINRequired = person.PINRequired;
			importedPerson.LockedOut = person.LockedOut;
			importedPerson.LockedOutReason = person.LockedOutReason;
			importedPerson.LockedOutDate = person.LockedOutDate;
			importedPerson.LastActivityDate = person.LastActivityDate;
			importedPerson.ShortCardNumber = person.ShortCardNumber;
			importedPerson.OnFileSignature = person.OnFileSignature;
			importedPerson.AssignedEquipmentID = person.AssignedEquipmentID;

			importedPerson.RoleCollection.Clear();
			foreach ( ConsolidatedDataObjects.PersonRoleMapClass role in person.RoleCollection )
			{
				var map = new PersonRoleMapClass
				{
					Role = (PERSON_ROLE) role.Role,
					PersonGuid = importedPerson.IdentityGuid
				};

				importedPerson.RoleCollection.Add( map );
			}

			return importedPerson;
		}

		/// <summary>
		/// Imports the pidx profile record.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="pidxProfile">The 7.5.2 pidxProfile.</param>
		/// <param name="profiles">The profiles interface to use for service calls..</param>
		/// <returns>A current version pidx profile class with imported data.</returns>
		private PIDXProfileClass ImportPidxProfile(
			SecurityClass security, ConsolidatedDataObjects.PIDXProfileClass pidxProfile, IPIDXProfiles profiles )
		{
			var importedProfile = new PIDXProfileClass();

			var guid = profiles.GetIdentityGuid( security, pidxProfile.ID );

			if ( guid != Guid.Empty )
			{
				importedProfile = profiles.Get( security, guid, getMaps: true );
			}

			importedProfile.ID = pidxProfile.ID;
			importedProfile.Deleted = pidxProfile.Deleted;
			importedProfile.Enabled = pidxProfile.Enabled;
			importedProfile.IPAddress = pidxProfile.IPAddress;
			importedProfile.LogFilePath = pidxProfile.LogFilePath;
			importedProfile.LoggingEnabled = pidxProfile.LoggingEnabled;
			importedProfile.Password = pidxProfile.Password;
			importedProfile.Port = pidxProfile.Port;
			importedProfile.TerminalID = pidxProfile.TerminalID;
			importedProfile.UserID = pidxProfile.UserID;

			importedProfile.PIDXProfileCompanyMapCollection.Clear();
			foreach ( ConsolidatedDataObjects.PIDXProfileCompanyMapClass map in pidxProfile.PIDXProfileCompanyMapCollection )
			{
				var addMap = new PIDXProfileCompanyMapClass
				{
					SellerID = map.SellerID,
					ShipperID = map.ShipperID,
					ConsigneeNumber = map.ConsigneeNumber,
					DenialOverride = map.DenialOverride,
					UnavailableOverride = map.UnavailableOverride,
					ShipToID = map.ShipToID,
					ShipToName = map.ShipToName,
					ShipToAddress = map.ShipToAddress,
					ShipToCity = map.ShipToCity,
					ShipToState = map.ShipToState
				};

				importedProfile.PIDXProfileCompanyMapCollection.Add( addMap );
			}

			return importedProfile;
		}

		/// <summary>
		/// Imports the group record.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="group">The 7.5.2 group.</param>
		/// <param name="groups">The groups interface to use for service calls..</param>
		/// <returns>A current version group class with imported data.</returns>
		private GroupClass ImportGroup(
			SecurityClass security, ConsolidatedDataObjects.GroupClass group, IGroups groups )
		{
			var importedGroup = new GroupClass();

			var guid = groups.GetIdentityGuid( security, group.ID );

			if ( guid != Guid.Empty )
			{
				importedGroup = groups.Get( security, guid );
			}

			importedGroup.ID = group.ID;
			importedGroup.Description = group.Description;

			foreach ( ConsolidatedDataObjects.CompanyMapClass map in group.CompanyMapCollection )
			{
				CompanyMapClass addMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP);
				addMap.MapID = map.MapID;
				addMap.AssignedToID = map.AssignedToID;
				addMap.AssignedID = map.AssignedID;
				addMap.LockedOut = map.LockedOut;
				addMap.AssignedName = map.AssignedName;
				addMap.AssignedAddress = map.AssignedAddress;
				addMap.AssignedCity = map.AssignedCity;
				addMap.AssignedState = map.AssignedState;
				addMap.AssignedToName = map.AssignedToName;
				addMap.AssignedToAddress = map.AssignedToAddress;
				addMap.AssignedToCity = map.AssignedToCity;
				addMap.AssignedToState = map.AssignedToState;

				// Account for change in version usage of brackets.
				if (addMap.AssignedID == "<All>")
				{
					addMap.AssignedID = "{All}";
				}

				importedGroup.CompanyMapCollection.Add( addMap );
			}

			FMChannelHelper.MakeCall<IUsers>(
				users =>
				{
					foreach ( ConsolidatedDataObjects.UserClass user in group.UserCollection )
					{
						var lookupUser = users.GetByID( security, user.ID );
						if ( lookupUser.IdentityGuid != Guid.Empty )
						{
							var addMap = new UserGroupMapClass
							{
								SiteID = security.SiteID,
								UserID = lookupUser.ID,
								UserGuid = lookupUser.IdentityGuid,
								GroupID = group.ID
							};

							importedGroup.UserGroupMapCollection.Add( addMap );
						}
					}
				});

			foreach ( var right in group.RightCollection )
			{
				// Guard against obsolete rights since they cause serialization errors.
				if ( Enum.IsDefined( typeof( RIGHT ), (int) right ) )
				{
					importedGroup.RightCollection.Add( (RIGHT) ((int) right ));
				}
			}

			return importedGroup;
		}

		/// <summary>
		/// Imports the equipment record.
		/// </summary>
		/// <param name="equipment">The equipment record.</param>
		/// <param name="security">The security object.</param>
		/// <param name="equipments">The equipments service interface.</param>
		/// <param name="companies">The companies service interface.</param>
		/// <param name="products">The products service interface.</param>
		/// <returns>A current version equipment class with imported data.</returns>
		private EquipmentClass ImportEquipment( ConsolidatedDataObjects.EquipmentClass equipment, SecurityClass security, IEquipments equipments, ICompanies companies, IProducts products )
		{
			var importedEquipment = new EquipmentClass();

			var guid = equipments.GetIdentityGuid( security, equipment.ID );

			if ( guid != Guid.Empty )
			{
				importedEquipment = equipments.Get( security, guid );
			}

			importedEquipment.ActualGPM = equipment.ActualGPM;
			importedEquipment.AttachedTo = equipment.AttachedTo;
			importedEquipment.CalibrationDate = equipment.CalibrationDate.ToString();
			importedEquipment.Capacity = equipment.Capacity;
			importedEquipment.CompanyAddress = equipment.CompanyAddress;
			importedEquipment.CompanyCity = equipment.CompanyCity;
			importedEquipment.CompanyEquipmentID = equipment.CompanyEquipmentID;
			
			importedEquipment.CompanyID = equipment.CompanyID;
			importedEquipment.CompanyGuid = companies.GetMasterRecordGuid(security, importedEquipment.CompanyID);
			importedEquipment.CompanyName = equipment.CompanyName;
			importedEquipment.CompanyState = equipment.CompanyState;

			// Compartments
			importedEquipment.CompartmentCollection.Clear();
			foreach (var compartment in equipment.CompartmentCollection)
			{
				var addCompartment = new EquipmentClass
					{
						ID = compartment.ID,
						Capacity = compartment.Capacity,
						SafeFill = compartment.SafeFill,
						VolumeUnits = (EngineeringUnit)compartment.VolumeUnits,
						TemperatureUnits = (EngineeringUnit)compartment.TemperatureUnits,
						DensityUnits = (EngineeringUnit)compartment.DensityUnits,
						MassUnits = (EngineeringUnit)compartment.MassUnits,
						VolumeDecimalPlaces = compartment.VolumeDecimalPlaces,
						TemperatureDecimalPlaces = compartment.TemperatureDecimalPlaces,
						DensityDecimalPlaces = compartment.DensityDecimalPlaces,
						EquipmentSequence = compartment.EquipmentSequence
					};

				importedEquipment.CompartmentCollection.Add(addCompartment);
			}

			importedEquipment.DefuelMeterForwards = equipment.DefuelMeterForwards;
			importedEquipment.Deleted = equipment.Deleted;
			importedEquipment.DensityDecimalPlaces = equipment.DensityDecimalPlaces;
			importedEquipment.DensityUnits = (EngineeringUnit)equipment.DensityUnits;
			importedEquipment.Description = equipment.Description;

			importedEquipment.EqTypeName = equipment.EqTypeName;
			importedEquipment.EquipmentSequence = equipment.EquipmentSequence;

			importedEquipment.EquipmentTypeGuid = 
				FMChannelHelper.MakeCall<IEquipmentTypes, Guid>(x => x.GetIdentityGuid(security, equipment.EquipmentType.ID));
			
			importedEquipment.ExportUseFuelCard = new FuelCardClass { ID = equipment.ExportUseFuelCard.ID };

			importedEquipment.Fixed = equipment.Fixed;
			importedEquipment.FixedVolume = equipment.FixedVolume;
			importedEquipment.FuelAdditiveFlag = equipment.FuelAdditiveFlag;
			importedEquipment.FuelCardID = equipment.FuelCardID;
			importedEquipment.FuelingState = equipment.FuelingState;
			importedEquipment.FuelingType = (FUELING_TYPES) equipment.FuelingType;

			importedEquipment.ID = equipment.ID;
			importedEquipment.InServiceFlag = equipment.InServiceFlag;
			importedEquipment.InUse = equipment.InUse;
			importedEquipment.IntoPlane = equipment.IntoPlane;
			importedEquipment.IssPt = equipment.IssPt;
			importedEquipment.IssPtNum = equipment.IssPtNum;

			importedEquipment.LockedOut = equipment.LockedOut;
			importedEquipment.LockedOutDate = equipment.LockedOutDate;
			importedEquipment.LockedOutReason = equipment.LockedOutReason;
			importedEquipment.LowStockWarning = equipment.LowStockWarning;

			importedEquipment.MaintenanceNote = equipment.MaintenanceNote;
			importedEquipment.Make = equipment.Make;
			importedEquipment.ManagedEquipmentFlag = equipment.ManagedEquipmentFlag;
			importedEquipment.ManufactureDate = equipment.ManufactureDate.ToString();
			importedEquipment.MassDecimalPlaces = equipment.MassDecimalPlaces;
			importedEquipment.MassUnits = (EngineeringUnit)equipment.MassUnits;
			importedEquipment.MediaType = equipment.MediaType;
			importedEquipment.MeterReading = equipment.MeterReading;
			importedEquipment.Meters = equipment.Meters;
			importedEquipment.Mobile = equipment.Mobile;
			importedEquipment.Model = equipment.Model;

			importedEquipment.Notes = equipment.Notes;

			importedEquipment.ProductID = equipment.ProductID;
			importedEquipment.ProductGuid = products.GetMasterRecordGuidFromID(security, importedEquipment.ProductID);
			importedEquipment.PulseRatio = equipment.PulseRatio;

			importedEquipment.RatedGPM = equipment.RatedGPM;
			importedEquipment.Round = equipment.Round;
			importedEquipment.SICapacity.Value = equipment.SICapacity.Value;
			importedEquipment.SISafeFill.Value = equipment.SISafeFill.Value;
			importedEquipment.SafeFill = equipment.SafeFill;
			importedEquipment.SecondaryStorageFlag = equipment.SecondaryStorageFlag;
			importedEquipment.SerialNumber = equipment.SerialNumber;
			importedEquipment.StatusDescription = equipment.StatusDescription;
			importedEquipment.StockTrack = equipment.StockTrack;
			importedEquipment.StorageType = equipment.StorageType;

			importedEquipment.TagAndLicenseCollection.Clear();
			foreach (ConsolidatedDataObjects.QualificationMapClass map in equipment.TagAndLicenseCollection)
			{
				var addMap = new QualificationMapClass { ID = map.ID };
				importedEquipment.TagAndLicenseCollection.Add(addMap);
			}

			importedEquipment.TemperatureDecimalPlaces = equipment.TemperatureDecimalPlaces;
			importedEquipment.TemperatureUnits = (EngineeringUnit)equipment.TemperatureUnits;

			importedEquipment.TestAndInspectionCollection.Clear();
			foreach (ConsolidatedDataObjects.QualificationMapClass map in equipment.TestAndInspectionCollection)
			{
				var addMap = new QualificationMapClass { ID = map.ID };
				importedEquipment.TestAndInspectionCollection.Add(addMap);
			}

			importedEquipment.Totalisor1 = equipment.Totalisor1;
			importedEquipment.Totalisor2 = equipment.Totalisor2;
			importedEquipment.TruckCardNumber = equipment.TruckCardNumber;
			importedEquipment.Type = (FMBusinessObjects.DataObjects.EQUIPMENT_TYPE) equipment.Type;

			importedEquipment.UserData1 = equipment.UserData1;
			importedEquipment.UserData2 = equipment.UserData2;
			importedEquipment.UserData3 = equipment.UserData3;
			importedEquipment.UserData4 = equipment.UserData4;
			importedEquipment.UserData5 = equipment.UserData5;
			importedEquipment.UserData6 = equipment.UserData6;
			importedEquipment.UserData7 = equipment.UserData7;
			importedEquipment.UserData8 = equipment.UserData8;
			importedEquipment.UserData9 = equipment.UserData9;

			importedEquipment.UserData10 = equipment.UserData10;
			importedEquipment.UserData11 = equipment.UserData11;
			importedEquipment.UserData12 = equipment.UserData12;
			importedEquipment.UserData13 = equipment.UserData13;
			importedEquipment.UserData14 = equipment.UserData14;
			importedEquipment.UserData15 = equipment.UserData15;
			importedEquipment.UserData16 = equipment.UserData16;
			importedEquipment.UserData17 = equipment.UserData17;
			importedEquipment.UserData18 = equipment.UserData18;
			importedEquipment.UserData19 = equipment.UserData19;

			importedEquipment.UserData20 = equipment.UserData20;
			importedEquipment.UserData21 = equipment.UserData21;
			importedEquipment.UserData22 = equipment.UserData22;
			importedEquipment.UserData23 = equipment.UserData23;
			importedEquipment.UserData24 = equipment.UserData24;

			importedEquipment.Volume = equipment.Volume;
			importedEquipment.VolumeDecimalPlaces = equipment.VolumeDecimalPlaces;
			importedEquipment.VolumeUnits = (EngineeringUnit) equipment.VolumeUnits;

			importedEquipment.Xref = equipment.Xref;
			importedEquipment.Year = equipment.Year;

			return importedEquipment;
		}

		/// <summary>
		/// Imports the 7.5.2 product.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="product">The 7.5.2 product.</param>
		/// <param name="products">The products interface to use for service calls..</param>
		/// <param name="meters">The meters service interface.</param>
		/// <returns>A current version product class with imported data.</returns>
		private ProductClass ImportProduct( SecurityClass security, ConsolidatedDataObjects.ProductClass product, IProducts products, IMeters meters )
		{
			var importedProduct = new ProductClass();

			var guid = products.GetIdentityGuid( security, product.ID );

			if ( guid != Guid.Empty )
			{
				importedProduct = products.Get( security, guid );
			}

			importedProduct.ID = product.ID;
			importedProduct.StockResetDate = product.StockResetDate;
			importedProduct.DensityHighLimit = product.DensityHighLimit;
			importedProduct.DensityLowLimit = product.DensityLowLimit;
			importedProduct.TemperatureHiHiLimit = product.TemperatureHiHiLimit;
			importedProduct.TemperatureHighLimit = product.TemperatureHighLimit;
			importedProduct.TemperatureLowLimit = product.TemperatureLowLimit;
			importedProduct.TemperatureLoLoLimit = product.TemperatureLoLoLimit;
			importedProduct.TemperatureDeadband = product.TemperatureDeadband;
			importedProduct.LowStockWarning = product.LowStockWarning;
			importedProduct.Price = product.Price;

            //importedProduct.MajorCorrectionMethod = product.MajorCorrectionMethod;
			//importedProduct.MinorCorrectionMethod = product.MinorCorrectionMethod;

			//importedProduct.CorrectionFactor0 = product.CorrectionFactor[0];
			//importedProduct.CorrectionFactor1 = product.CorrectionFactor[1];
			//importedProduct.CorrectionFactor2 = product.CorrectionFactor[2];
			//importedProduct.CorrectionFactor3 = product.CorrectionFactor[3];
			//importedProduct.CorrectionFactor4 = product.CorrectionFactor[4];

			importedProduct.StandardDensity = product.StandardDensity;
			//importedProduct.StandardTemperature = product.StandardTemperature;
			//importedProduct.AlternateTemperature = product.AlternateTemperature;
			//importedProduct.AlternatePressure = product.AlternatePressure;
			importedProduct.OctaneNumber = product.OctaneNumber;
			importedProduct.ReidVaporPressure = product.ReidVaporPressure;
			importedProduct.HazardousMaterial = product.HazardousMaterial;
			importedProduct.ComponentTolerance = product.ComponentTolerance;

			importedProduct.LockedOutDate = product.LockedOutDate;

			importedProduct.TrackingProductID = product.TrackingProductID;
			if ( string.IsNullOrEmpty( importedProduct.TrackingProductID ) == false )
			{
				importedProduct.TrackingProductGuid = products.GetMasterRecordGuidFromID( security, importedProduct.TrackingProductID );
			}

			importedProduct.UserData1 = product.UserData1;
			importedProduct.UserData2 = product.UserData2;
			importedProduct.UserData3 = product.UserData3;
			importedProduct.UserData4 = product.UserData4;
			importedProduct.UserData5 = product.UserData5;
			importedProduct.UserData6 = product.UserData6;
			importedProduct.UserData7 = product.UserData7;

			importedProduct.Description = product.Description;
			importedProduct.GenericType = product.GenericType;
			importedProduct.ProductType = (ProductType) product.ProductType;
			importedProduct.StockTrack = product.StockTrack;

			importedProduct.DensityUnits = (EngineeringUnit) product.DensityUnits;
			importedProduct.TemperatureUnits = (EngineeringUnit) product.TemperatureUnits;

			importedProduct.DensityDeadband = product.DensityDeadband;
			importedProduct.ApplyDensityLimits = product.ApplyDensityLimits;
			importedProduct.ApplyTemperatureLimits = product.ApplyTemperatureLimits;
			importedProduct.Bonded = product.Bonded;
			importedProduct.GroundFuel = product.GroundFuel;
			importedProduct.Code = product.Code;
			importedProduct.AviationFuel = product.AviationFuel;
			importedProduct.ApplyVolumeCorrection = product.ApplyVolumeCorrection;
			importedProduct.DensityDecimalPlaces = product.DensityDecimalPlaces;
			importedProduct.TemperatureDecimalPlaces = product.TemperatureDecimalPlaces;
			importedProduct.Capitalize = product.Capitalize;
			importedProduct.RegulatoryClass = product.RegulatoryClass;
			importedProduct.LoadRackDisplayText = product.LoadRackDisplayText;
			importedProduct.VaporRecovery = product.VaporRecovery;
			importedProduct.LockedOut = product.LockedOut;
			importedProduct.LockedOutReason = product.LockedOutReason;
			importedProduct.VarianceTolerance = product.VarianceTolerance;
			importedProduct.LoadByWeight = product.LoadByWeight;
			importedProduct.PIDXCode = product.PIDXProductCode;
			importedProduct.ContaminationPromptLoadRackText = product.ContaminationPromptLoadRackText;
			importedProduct.InhibitAccounting = product.InhibitAccounting;
			importedProduct.TrackingProductID = product.TrackingProductID;
			importedProduct.SiteID = product.SiteID;

			// Prouduct Messages
			importedProduct.ProductMessageCollection.Clear();
			foreach ( ConsolidatedDataObjects.ApplicationStringMapClass message in product.ProductMessageCollection )
			{
				var addMessage = new ApplicationStringMapClass
				{
					AssignedToAddress = message.AssignedToAddress,
					AssignedToCity = message.AssignedToCity,
					AssignedToCode = message.AssignedToCode,
					AssignedToDescription = message.AssignedToDescription,
					AssignedToID = message.AssignedToID,
					AssignedToName = message.AssignedToName,
					AssignedToProductType = (ProductType) message.AssignedToProductType,
					AssignedToState = message.AssignedToState,
					Deleted = message.Deleted,
					ID = message.ID,
					Sequence = message.Sequence,
					Type = (STRING_MAP_TYPE) message.Type,
					SiteID = message.SiteID
				};

				importedProduct.ProductMessageCollection.Add( addMessage );
			}

			// Hazardous Material Messsages
			importedProduct.HazardousMaterialMessageCollection.Clear();
			foreach ( ConsolidatedDataObjects.ApplicationStringMapClass message in product.HazardousMaterialMessageCollection )
			{
				var addMessage = new ApplicationStringMapClass
				{
					AssignedToAddress = message.AssignedToAddress,
					AssignedToCity = message.AssignedToCity,
					AssignedToCode = message.AssignedToCode,
					AssignedToDescription = message.AssignedToDescription,
					AssignedToID = message.AssignedToID,
					AssignedToName = message.AssignedToName,
					AssignedToProductType = (ProductType) message.AssignedToProductType,
					AssignedToState = message.AssignedToState,
					Deleted = message.Deleted,
					ID = message.ID,
					Sequence = message.Sequence,
					Type = (STRING_MAP_TYPE) message.Type,
					SiteID = message.SiteID
				};

				importedProduct.HazardousMaterialMessageCollection.Add( addMessage );
			}

			// Component Collection
			importedProduct.ComponentCollection.Clear();
			foreach ( ConsolidatedDataObjects.ProductMapClass map in product.ComponentCollection )
			{
				ProductMapClass addMap = this.CopyProductMap( security, map, meters );
				importedProduct.ComponentCollection.Add( addMap );
			}

			// Authorized Customers
			importedProduct.AuthorizedCustomerCollection.Clear();
			foreach ( ConsolidatedDataObjects.ProductMapClass map in product.AuthorizedCustomerCollection )
			{
				var addMap = this.CopyProductMap( security, map, meters );
				importedProduct.AuthorizedCustomerCollection.Add( addMap );
			}

			// Authorized Customer Groups
			importedProduct.AuthorizedCustomerGroupCollection.Clear();
			foreach ( ConsolidatedDataObjects.ProductMapClass map in product.AuthorizedCustomerGroupCollection )
			{
				var addMap = this.CopyProductMap( security, map, meters );
				importedProduct.AuthorizedCustomerGroupCollection.Add( addMap );
			}

			return importedProduct;
		}

		/// <summary>
		/// Copies/imports the product map.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="map">
		/// The source product map.
		/// </param>
		/// <param name="meters">
		/// The meters service interface to use.
		/// </param>
		/// <returns>
		/// A current version product map with imported data.
		/// </returns>
		private ProductMapClass CopyProductMap( SecurityClass security, ConsolidatedDataObjects.ProductMapClass map, IMeters meters )
		{
		    var addMap = new ProductMapClass
		                 {
		                     AdditiveCycleVolume = map.AdditiveCycleVolume,
		                     AdditiveProfileID = map.AdditiveProfileID
		                 };


		    addMap.AdditiveProfileGuid =
				FMChannelHelper.MakeCall<IAdditiveProfiles, Guid>( x => x.GetIdentityGuid( security, addMap.AdditiveProfileID ) );

			addMap.AdditiveRate = map.AdditiveRate;
			addMap.AssignedCode = map.AssignedCode;
			addMap.AssignedDescription = map.AssignedDescription;
			addMap.AssignedID = map.AssignedID;
			addMap.AssignedLoadRackDisplayText = map.AssignedLoadRackDisplayText;
			addMap.AssignedProductType = (ProductType) map.AssignedProductType;
			addMap.AssignedToAddress = map.AssignedToAddress;
			addMap.AssignedToCity = map.AssignedToCity;
			addMap.AssignedToID = map.AssignedToID;
			addMap.AssignedToName = map.AssignedToName;
			addMap.AssignedToState = map.AssignedToState;
			addMap.BlendPercentage = map.BlendPercentage;
			addMap.ContaminationPromptLoadRackText = map.ContaminationPromptLoadRackText;
			addMap.HazardousMaterial = map.HazardousMaterial;
			addMap.ID = map.ID;
			addMap.LoadByWeight = map.LoadByWeight;
			addMap.LockedOut = map.LockedOut;

			addMap.Type = (PRODUCT_MAP_TYPE) map.Type;

			ConsolidatedDataObjects.ProductMapClass map1 = map;

			var meterGuid = meters.GetIdentityGuid( security, map1.MeterID );

			if ( meterGuid != Guid.Empty )
			{
				addMap.Meter = meters.Get( security, meterGuid );
			}

			addMap.Meter = null;

			return addMap;
		}

		/// <summary>
		/// Imports the company.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="company">
		/// The company to import.
		/// </param>
		/// <param name="companies">
		/// The companies service interface to use.
		/// </param>
		/// <param name="meters">
		/// The meters service interface.
		/// </param>
		/// <returns>
		/// A current version company with imported data.
		/// </returns>
		private CompanyClass ImportCompany( SecurityClass security, ConsolidatedDataObjects.CompanyClass company, ICompanies companies, IMeters meters )
		{
			var importedCompany = new CompanyClass();

			// Try to look up the company first.
			var guid = companies.GetIdentityGuid( security, company.ID );

			if ( guid != Guid.Empty )
			{
				importedCompany = companies.Get( security, guid );
			}

			// Import values
			importedCompany.ID = company.ID;
			importedCompany.AccountNumber = company.AccountNumber;
			importedCompany.Code = company.Code;
			importedCompany.Name = company.Name;
			importedCompany.Address1 = company.Address1;
			importedCompany.Address2 = company.Address2;
			importedCompany.City = company.City;
			importedCompany.State = company.State;
			importedCompany.Zip = company.Zip;
			importedCompany.Country = company.Country;
			importedCompany.Phone = company.Phone;
			importedCompany.Fax = company.Fax;
			importedCompany.EmergencyContact = company.EmergencyContact;
			importedCompany.FlightPrefix = company.FlightPrefix;
			importedCompany.EffectiveDate = company.EffectiveDate;
			importedCompany.ExpirationDate = company.ExpirationDate;

			var iata = company.IATAID;
			importedCompany.IATAID = iata;
			importedCompany.IATAGuid = FMChannelHelper.MakeCall<IIATACodes, Guid>(x => x.GetIdentityGuid(security, iata));

			importedCompany.OnHold = company.OnHold;
			importedCompany.PickupFlights = company.PickupFlights;
			importedCompany.StockTrack = company.StockTrack;
			importedCompany.SufferLossGain = company.SufferLossGain;
			importedCompany.LowStockWarning = company.LowStockWarning;
			importedCompany.LockedOut = company.LockedOut;
			importedCompany.LockedOutReason = company.LockedOutReason;
			importedCompany.LockedOutDate = company.LockedOutDate;

			FMChannelHelper.MakeCall<IApplicationStrings>(
				applicationStrings =>
				{
					importedCompany.ShipperTypeID = company.ShipperTypeID;
					importedCompany.ShipperTypeApplicationStringGuid = applicationStrings.GetIdentityGuid( security, STRING_TYPE.COMPANY_TYPE, company.ShipperTypeID );

					importedCompany.CustomerBillToTypeID = company.CustomerBillToTypeID;
					importedCompany.CustomerBillToTypeApplicationStringGuid = applicationStrings.GetIdentityGuid( security, STRING_TYPE.COMPANY_TYPE, company.CustomerBillToTypeID );

					importedCompany.CustomerShipToTypeID = company.CustomerShipToTypeID;
					importedCompany.CustomerShipToTypeApplicationStringGuid = applicationStrings.GetIdentityGuid(
						security, STRING_TYPE.COMPANY_TYPE, company.CustomerShipToTypeID);
				});

			importedCompany.ReceivableAccount = company.ReceivableAccount;
			importedCompany.RefinerCode = company.RefinerCode;
			importedCompany.LastActivityDate = company.LastActivityDate;
			importedCompany.CreditOK = company.CreditOK;
			importedCompany.AdditiveAccounting = company.AdditiveAccounting;
			importedCompany.PurchaseOrderRequired = company.PurchaseOrderRequired;
			importedCompany.EPANumber = company.EPANumber;
			importedCompany.FederalID = company.FederalID;
			importedCompany.TaxNumber = company.TaxNumber;
			importedCompany.FlushPermitted = company.FlushPermitted;
			importedCompany.PumpOffPermitted = company.PumpOffPermitted;
			importedCompany.DeliveryToTerminalPermitted = company.DeliveryToTerminalPermitted;
			importedCompany.LicenseNumber = company.LicenseNumber;
			importedCompany.LicenseExpiration = company.LicenseExpiration;
			importedCompany.InsuranceCompany = company.InsuranceCompany;
			importedCompany.InsurancePolicy = company.InsurancePolicy;
			importedCompany.LiabilityAmount = company.LiabilityAmount;
			importedCompany.HazardousMaterialExclusion = company.HazardousMaterialExclusion;
			importedCompany.InsuranceExpiration = company.InsuranceExpiration;
			importedCompany.AllowDriverEntry = company.AllowDriverEntry;
			importedCompany.PINRequired = company.PINRequired;
			importedCompany.MaximumVehicleWeight = company.MaximumVehicleWeight;
			importedCompany.WeightUnits = (EngineeringUnit) company.WeightUnits;
			importedCompany.AccountNumber = company.AccountNumber;
			importedCompany.SCACCode = company.SCACCode;
			importedCompany.DisableOwnerAllocationsCheck = company.DisableOwnerAllocationsCheck;
			importedCompany.DisableShipperAllocationsCheck = company.DisableShipperAllocationsCheck;
			importedCompany.DisableShipToAllocationsCheck = company.DisableShipToAllocationsCheck;
			importedCompany.LoadRackDisplayText = company.LoadRackDisplayText;

			// User Data
			importedCompany.UserData1 = company.UserData1;
			importedCompany.UserData2 = company.UserData2;
			importedCompany.UserData3 = company.UserData3;
			importedCompany.UserData4 = company.UserData4;
			importedCompany.UserData5 = company.UserData5;
			importedCompany.UserData6 = company.UserData6;
			importedCompany.UserData7 = company.UserData7;
			importedCompany.UserData8 = company.UserData8;

			importedCompany.Note = company.Note.Note;

			importedCompany.RoleCollection.Clear();
			foreach ( ConsolidatedDataObjects.CompanyRoleMapClass role in company.RoleCollection )
			{
				var addRole = new CompanyRoleMapClass
				{
					CompanyAddress1 = role.CompanyAddress1,
					CompanyAddress2 = role.CompanyAddress2,
					CompanyID = role.CompanyID,
					CompanyName = role.CompanyName,
					CreatedBy = role.CreatedBy,
					CreatedDate = role.CreatedDate,
					HasBillToRole = role.HasBillToRole,
					HasCarrierRole = role.HasCarrierRole,
					HasManagerRole = role.HasManagerRole,
					HasOwnerRole = role.HasOwnerRole,
					HasShipToRole = role.HasShipToRole,
					HasShipperRole = role.HasShipperRole,
					HasSupplierRole = role.HasSupplierRole,
					ID = role.ID,
					Role = (COMPANY_ROLE) role.Role
				};

				importedCompany.RoleCollection.Add( addRole );
			}

			importedCompany.AuthorizedCarrierCollection.Clear();
			foreach ( ConsolidatedDataObjects.CompanyMapClass carrier in company.AuthorizedCarrierCollection )
			{
			    var addCarrier = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
			    addCarrier.AssignedAddress = carrier.AssignedAddress;
                addCarrier.AssignedCity = carrier.AssignedCity;
                addCarrier.AssignedID = carrier.AssignedID;
                addCarrier.AssignedName = carrier.AssignedName;
                addCarrier.AssignedState = carrier.AssignedState;
                addCarrier.AssignedToAddress = carrier.AssignedToAddress;
                addCarrier.AssignedToCity = carrier.AssignedToCity;
                addCarrier.AssignedToID = carrier.AssignedToID;
                addCarrier.AssignedToName = carrier.AssignedToName;
                addCarrier.AssignedToState = carrier.AssignedToState;
                addCarrier.CreatedBy = carrier.CreatedBy;
                addCarrier.CreatedDate = carrier.CreatedDate;
                addCarrier.Deleted = carrier.Deleted;
                addCarrier.ID = carrier.ID;
                addCarrier.SiteID = carrier.SiteID;
                addCarrier.UpdatedBy = carrier.UpdatedBy;
			    addCarrier.UpdatedDate = carrier.UpdatedDate;

				importedCompany.AuthorizedCarrierCollection.Add( addCarrier );
			}

			importedCompany.CarrierCustomerShipToCollection.Clear();
			foreach ( ConsolidatedDataObjects.CompanyMapClass carrier in company.CarrierCustomerShipToCollection )
			{
			    var addCarrier = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP);
			    addCarrier.AssignedAddress = carrier.AssignedAddress;
			    addCarrier.AssignedCity = carrier.AssignedCity;
			    addCarrier.AssignedID = carrier.AssignedID;
			    addCarrier.AssignedName = carrier.AssignedName;
			    addCarrier.AssignedState = carrier.AssignedState;
			    addCarrier.AssignedToAddress = carrier.AssignedToAddress;
			    addCarrier.AssignedToCity = carrier.AssignedToCity;
			    addCarrier.AssignedToID = carrier.AssignedToID;
			    addCarrier.AssignedToName = carrier.AssignedToName;
			    addCarrier.AssignedToState = carrier.AssignedToState;
			    addCarrier.CreatedBy = carrier.CreatedBy;
			    addCarrier.CreatedDate = carrier.CreatedDate;
			    addCarrier.Deleted = carrier.Deleted;
			    addCarrier.ID = carrier.ID;
			    addCarrier.SiteID = carrier.SiteID;
			    addCarrier.UpdatedBy = carrier.UpdatedBy;
			    addCarrier.UpdatedDate = carrier.UpdatedDate;

				importedCompany.CarrierCustomerShipToCollection.Add( addCarrier );
			}

			importedCompany.EquipmentCollection.Clear();
			foreach ( ConsolidatedDataObjects.EquipmentClass equipment in company.EquipmentCollection )
			{
				var addEquipment = new EquipmentClass { ID = equipment.ID };

				if ( equipment.Type == EQUIPMENT_TYPE.COMPARTMENT_TYPE )
				{
					importedCompany.EquipmentCollection.Add( addEquipment );
				}
				else
				{
					EquipmentTypeClass type = this.GetEquipmentType( security, equipment.Type );

					if ( type != null )
					{
						addEquipment.EquipmentTypeGuid = type.IdentityGuid;
						importedCompany.EquipmentCollection.Add( addEquipment );
					}
				}
			}

			importedCompany.CarrierCustomerShipToCollection.Clear();
			foreach ( ConsolidatedDataObjects.QualificationMapClass qual in company.CertificateAndPermitCollection )
			{
				var addQual = new QualificationMapClass
				{
					Sequence = qual.Sequence,
					Instructor = qual.Instructor,
					DateCompleted = { Value = qual.DateCompleted.Value },
					DateDue = { Value = qual.DateDue.Value },
					ExpirationDate = { Value = qual.ExpirationDate.Value },
					ID = qual.ID,
					UpdatedDate = qual.UpdatedDate,
					UpdatedBy = qual.UpdatedBy,
					Rating = qual.Rating,
					HistoricalRecord = qual.HistoricalRecord
				};

				importedCompany.CertificateAndPermitCollection.Add( addQual );
			}

			importedCompany.AuthorizedProductCollection.Clear();
			foreach ( ConsolidatedDataObjects.ProductMapClass map in company.AuthorizedProductCollection )
			{
				var addMap = this.CopyProductMap( security, map, meters );
				importedCompany.AuthorizedProductCollection.Add( addMap );
			}

			importedCompany.UnavailableInventoryCollection.Clear();
			foreach ( ConsolidatedDataObjects.ProductMapClass map in company.UnavailableInventoryCollection )
			{
				var addMap = this.CopyProductMap( security, map, meters );
				importedCompany.UnavailableInventoryCollection.Add( addMap );
			}

			importedCompany.GroupMapCollection.Clear();
			foreach ( ConsolidatedDataObjects.CompanyMapClass group in company.GroupMapCollection )
			{
			    var addgroup = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP);
			    addgroup.AssignedAddress = group.AssignedAddress;
			    addgroup.AssignedCity = group.AssignedCity;
			    addgroup.AssignedID = group.AssignedID;
			    addgroup.AssignedName = group.AssignedName;
			    addgroup.AssignedState = group.AssignedState;
			    addgroup.AssignedToAddress = group.AssignedToAddress;
			    addgroup.AssignedToCity = group.AssignedToCity;
			    addgroup.AssignedToID = group.AssignedToID;
			    addgroup.AssignedToName = group.AssignedToName;
			    addgroup.AssignedToState = group.AssignedToState;
			    addgroup.CreatedBy = group.CreatedBy;
			    addgroup.CreatedDate = group.CreatedDate;
			    addgroup.Deleted = group.Deleted;
			    addgroup.ID = group.ID;
			    addgroup.SiteID = group.SiteID;
			    addgroup.UpdatedBy = group.UpdatedBy;
			    addgroup.UpdatedDate = group.UpdatedDate;

				importedCompany.GroupMapCollection.Add( addgroup );
			}

			importedCompany.AccessScheduleCollection.Clear();
			foreach ( ConsolidatedDataObjects.ScheduleClass sched in company.AccessScheduleCollection )
			{
				var addSched = new ScheduleClass
				{
					ClosingTime = { Value = sched.ClosingTime.Value },
					Day = sched.Day,
					Deleted = sched.Deleted,
					Enabled = sched.Enabled,
					EndOfDayEnabled = sched.EndOfDayEnabled,
					EndOfDayTime = { Value = sched.EndOfDayTime.Value },
					ID = sched.ID,
					OpeningTime = { Value = sched.OpeningTime.Value },
					CreatedBy = sched.CreatedBy,
					CreatedDate = sched.CreatedDate,
					Type = (SCHEDULE_TYPE) sched.Type
				};

				importedCompany.AccessScheduleCollection.Add( addSched );
			}

			importedCompany.SupplierAuthorizedProductCollection.Clear();
			foreach ( ConsolidatedDataObjects.ProductMapClass map in company.SupplierAuthorizedProductCollection )
			{
				var addMap = this.CopyProductMap( security, map, meters );
				importedCompany.SupplierAuthorizedProductCollection.Add( addMap );
			}

			return importedCompany;
		}

		/// <summary>
		/// Gets the type of the equipment.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="type">The type.</param>
		/// <returns>The first type with the base type of the import object.</returns>
		private EquipmentTypeClass GetEquipmentType( SecurityClass security, EQUIPMENT_TYPE type )
		{
			var types =
				FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeCollectionClass>( x => x.Enumerate( security, null, null ) );

			if ( types.Count > 0 )
			{
				var equipementType = types.Find( x => x.Attribute == (FMBusinessObjects.DataObjects.EQUIPMENT_TYPE) type );

				if ( equipementType != null )
				{
					return equipementType;
				}
			}

			return null;
		}
	}
}
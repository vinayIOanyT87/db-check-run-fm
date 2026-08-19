// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDispatchFormBase.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMDispatchFormBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.ServiceModel;
	using System.Web.Script.Serialization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	///    Dispatch base form
	/// </summary>
	public class FMDispatchFormBase : FMFormBase
	{
		#region Enums
		/// <summary>
		///    Supported derived page types
		/// </summary>
		public enum PageType
		{
			TabularView = 0,
			DispatchingView = 1
		}

		#endregion

		#region Public Properties

		/// <summary>
		///    Gets the the identity guid of the current dispatch configuration.
		/// </summary>
		public Guid DispatchConfigurationGuid { get; private set; }

		/// <summary>
		///    Gets the JSON serialized list of transaction alias names
		/// </summary>
		public string JsonTransactionAliasNames { get; private set; }

		/// <summary>
		/// Gets the JSON serialized list of transaction status values
		/// </summary>
		public string JsonTransactionStatusValues { get; private set; }

		/// <summary>
		/// Gets the JSON serialized operational lock date value
		/// </summary>
		public string JsonOperationLockDateValue { get; private set; }

		/// <summary>
		/// Gets the JSON serialized operational time arrival flag value
		/// </summary>
		public string JsonOptionalTimeArrivalFlag { get; private set; }

		/// <summary>
		/// Gets the JSON serialized operational time start flag value
		/// </summary>
		public string JsonOptionalTimeStartFlag { get; private set; }

		/// <summary>
		/// Gets the JSON serialized operational time stop flag value
		/// </summary>
		public string JsonOptionalTimeStopFlag { get; private set; }

		/// <summary>
		///    Gets the JSON serialized tabular view grid column definitions.
		/// </summary>
		public string JsonTabularGridColumnDefinitions { get; private set; }

		/// <summary>
		///    Gets the JSON serialized equipment grid column definitions.
		/// </summary>
		public string JsonEquipmentGridColumnDefinitions { get; private set; }

		/// <summary>
		///    Gets the JSON serialized personnel grid column definitions.
		/// </summary>
		public string JsonPersonnelGridColumnDefinitions { get; private set; }

		/// <summary>
		///    Gets the JSON serialized request grid column definitions.
		/// </summary>
		public string JsonRequestGridColumnDefinitions { get; private set; }

		/// <summary>
		///    Gets the address of the dispatch request service.
		/// </summary>
		public string DispatchRequestServiceAddress { get; private set; }

		/// <summary>
		///    Gets a value indicating whether to display the current time.
		/// </summary>
		public bool DisplayCurrentTime { get; private set; }

		/// <summary>
		///    Gets the integer value of the current display time property for use in java script code.
		/// </summary>
		public int DisplayCurrentTimeInt
		{
			get
			{
				return this.DisplayCurrentTime ? 1 : 0;
			}
		}

		/// <summary>
		///    Gets a value indicating whether to use the military julian date format.
		/// </summary>
		public bool DisplayMilitaryJulianDate { get; private set; }

		/// <summary>
		///    Gets the integer value of the DisplayMilitaryJulianDate property for use in java script code.
		/// </summary>
		public int DisplayMilitaryJulianDateInt
		{
			get
			{
				return this.DisplayMilitaryJulianDate ? 1 : 0;
			}
		}

		/// <summary>
		///    Gets a value indicating whether to enable dispatch service requests.
		/// </summary>
		public bool EnableServiceRequests { get; private set; }

		/// <summary>
		///    Gets the integer value of the EnableServiceRequests property for use in java script code.
		/// </summary>
		public int EnableServiceRequestsInt
		{
			get
			{
				return this.EnableServiceRequests ? 1 : 0;
			}
		}

		/// <summary>
		///    Gets the dispatch service request automatic restart delay.
		/// </summary>
		public int ServiceRequestAutomaticRestartDelay { get; private set; }

		/// <summary>
		///    Gets the dispatch service request refresh period.
		/// </summary>
		public int ServiceRequestRefreshPeriod { get; private set; }

		/// <summary>
		///    Gets the reset tabular view session operation value
		/// </summary>
		public string ResetTabularViewSessionOperation { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///    Checks for and display warning messages.
		/// </summary>
		/// <param name="resultDO">The result DO.</param>
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

		/// <summary>
		///    Finds the transaction.
		/// </summary>
		/// <param name="transactionArray">The transaction array.</param>
		/// <param name="itemGuid">The item GUID.</param>
		/// <returns>A transaction data object.</returns>
		protected TransactionDO FindTransaction(IEnumerable<TransactionDO> transactionArray, Guid itemGuid)
		{
			foreach (TransactionDO transaction in transactionArray)
			{
				foreach (LineItemDO lineItem in transaction.LineItems)
				{
					if (lineItem.TransactionLineItemGuid == itemGuid)
					{
						return transaction;
					}
				}
			}

			return null;
		}

		/// <summary>
		///    Gets the specified transaction by transID.
		/// </summary>
		/// <param name="transId">
		///    The transaction id.
		/// </param>
		/// <returns>
		///    A TransactionDO for the specified transID
		/// </returns>
		protected virtual TransactionDO GetTransaction(string transId)
		{
			if (string.IsNullOrEmpty(transId))
			{
				throw new ArgumentNullException("transId");
			}

			var sr = new TransactionSR { Security = this.Security, TransID = transId };

			TransactionDO transaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																	 x => x.Process(sr));

			if (transaction == null)
			{
				throw new ArgumentException("Transaction not found.");
			}

			return transaction;
		}

		/// <summary>
		///    Gets the configuration settings.
		/// </summary>
		/// <returns>The dispatch configuration object.</returns>
		protected DispatchConfigurationClass GetConfigurationSettings()
		{
			// Retrieve the current Dispatch Configuration from the database
			if (this.DispatchConfigurationGuid != Guid.Empty)
			{
				return FMChannelHelper.MakeCall<IDispatchConfigurations, DispatchConfigurationClass>(
					dispatchConfigs =>dispatchConfigs.Get(this.Security, this.DispatchConfigurationGuid));
			}

			return new DispatchConfigurationClass();
		}

		/// <summary>
		///    Initializes common properties and stores them in the specified hidden fields control.
		///    The specified page type (Tabular View, Dispatching View) is used to determine what
		///    properties to initialize.
		/// </summary>
		/// <param name="hiddenFields">The specified hidden fields control</param>
		/// <param name="pageType">The specified page type</param>
		protected void InitializeCommonProperties(FMDispatchHiddenFields hiddenFields, PageType pageType)
		{
			try
			{
				// Get the address of the dispatch request proxy service.  The associated ASPX page contains
				// java script code to store the value in the FuelsManagerServiceLib object.
				string hostName = this.Context.Request.Url.Host;
				int port = this.Context.Request.Url.Port;
				string scheme = this.Context.Request.Url.Scheme;
				string applicationPath = this.Context.Request.ApplicationPath;

				this.DispatchRequestServiceAddress = scheme + "://" + hostName + ':' + port + applicationPath + "/DispatchRequestProxy.svc";
				hiddenFields.DispatchRequestServiceAddress = this.DispatchRequestServiceAddress;

				// Retrieve the current Dispatch Configuration Guid from the database
				bool entityAssigned = false;
				this.DispatchConfigurationGuid = FMChannelHelper.MakeCall<IDispatchConfigurations, Guid>(
					dispatchConfigs => dispatchConfigs.GetIdentityGuidBySiteIdAndAssigned(
						this.Security, this.Security.SiteGuid, DispatchConfigurationClass.DefaultId, true, out entityAssigned));
				hiddenFields.DispatchConfigurationGuid = this.DispatchConfigurationGuid;

				// Retrieve the current Dispatch Configuration from the database
				var dispatchConfig = this.GetConfigurationSettings();

				// Set the dispatch service request properties to the corresponding dispatch configuration settings
				this.EnableServiceRequests = dispatchConfig.EnableServiceRequests;
				hiddenFields.EnableServiceRequests = this.EnableServiceRequests;

				this.ServiceRequestRefreshPeriod = dispatchConfig.DispatchDataRefreshPeriod;
				hiddenFields.ServiceRequestRefreshPeriod = this.ServiceRequestRefreshPeriod;

				this.ServiceRequestAutomaticRestartDelay = dispatchConfig.AutomaticRestartDelay;
				hiddenFields.ServiceRequestAutomaticRestartDelay = this.ServiceRequestAutomaticRestartDelay;

				this.DisplayCurrentTime = dispatchConfig.DisplayCurrentTime;
				hiddenFields.DisplayCurrentTime = this.DisplayCurrentTime;

				this.DisplayMilitaryJulianDate = dispatchConfig.TabularViewDisplayMilitaryDate;
				hiddenFields.DisplayMilitaryJulianDate = this.DisplayMilitaryJulianDate;

				var operation = (string)this.Session["ResetTabularViewSessionOperation"];
				if (string.IsNullOrWhiteSpace(operation))
				{
					operation = string.Empty;
				}
				this.ResetTabularViewSessionOperation = operation;
				this.Session.Remove("ResetTabularViewSessionOperation");

				if (pageType == PageType.TabularView)
				{
					this.JsonTransactionAliasNames = this.GetTransactionAliasNames();
					hiddenFields.JsonTransactionAliasNames = this.JsonTransactionAliasNames;

					this.JsonTransactionStatusValues = this.GetTransactionStatusValues();
					hiddenFields.JsonTransactionStatusValues = this.JsonTransactionStatusValues;

					this.JsonOperationLockDateValue = this.GetOperationalLockDate();
					hiddenFields.JsonOperationalLockDateValue = this.JsonOperationLockDateValue;

					this.JsonTabularGridColumnDefinitions = this.GetGridColumnDefinitions(1, "Dispatch Tabular View");
					hiddenFields.JsonTabularGridColumnDefinitions = this.JsonTabularGridColumnDefinitions;

					this.SetOperationalTimes(hiddenFields);
				}
				else if (pageType == PageType.DispatchingView)
				{
					this.JsonRequestGridColumnDefinitions = this.GetGridColumnDefinitions(2, "Dispatching View - Active Request Queue");
					hiddenFields.JsonRequestGridColumnDefinitions = this.JsonRequestGridColumnDefinitions;
					this.JsonPersonnelGridColumnDefinitions = this.GetGridColumnDefinitions(3, "Dispatching View - Operator");
					hiddenFields.JsonPersonnelGridColumnDefinitions = this.JsonPersonnelGridColumnDefinitions;
					this.JsonEquipmentGridColumnDefinitions = this.GetGridColumnDefinitions(4, "Dispatching View - Servicing Unit");
					hiddenFields.JsonEquipmentGridColumnDefinitions = this.JsonEquipmentGridColumnDefinitions;
				}

			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		/// <summary>
		///    Restores common properties previously stored in the specified hidden fields control
		/// </summary>
		protected void RestoreCommonProperties(FMDispatchHiddenFields hiddenFields)
		{
			try
			{
				this.DispatchConfigurationGuid				= hiddenFields.DispatchConfigurationGuid;
				this.JsonTransactionAliasNames				= hiddenFields.JsonTransactionAliasNames;
				this.JsonTransactionStatusValues			= hiddenFields.JsonTransactionStatusValues;
				this.JsonOperationLockDateValue				= hiddenFields.JsonOperationalLockDateValue;
				this.JsonTabularGridColumnDefinitions		= hiddenFields.JsonTabularGridColumnDefinitions;
				this.JsonEquipmentGridColumnDefinitions		= hiddenFields.JsonEquipmentGridColumnDefinitions;
				this.JsonPersonnelGridColumnDefinitions		= hiddenFields.JsonPersonnelGridColumnDefinitions;
				this.JsonRequestGridColumnDefinitions		= hiddenFields.JsonRequestGridColumnDefinitions;
				this.DispatchRequestServiceAddress			= hiddenFields.DispatchRequestServiceAddress;
				this.EnableServiceRequests					= hiddenFields.EnableServiceRequests;
				this.ServiceRequestRefreshPeriod			= hiddenFields.ServiceRequestRefreshPeriod;
				this.ServiceRequestAutomaticRestartDelay	= hiddenFields.ServiceRequestAutomaticRestartDelay;
				this.DisplayCurrentTime						= hiddenFields.DisplayCurrentTime;
				this.DisplayMilitaryJulianDate				= hiddenFields.DisplayMilitaryJulianDate;
				this.JsonOptionalTimeArrivalFlag			= hiddenFields.JsonOptionalTimesArrivalFlagValue;
				this.JsonOptionalTimeStartFlag				= hiddenFields.JsonOptionalTimesStartFlagValue;
				this.JsonOptionalTimeStopFlag				= hiddenFields.JsonOptionalTimesStopFlagValue;
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		/// <summary>
		/// Creates a list of dispatch transaction alias names.  The list is serialized in JSON format for use
		/// by the IE browser client to dynamically populate the request type dropdown select control.
		/// </summary>
		/// <returns>The JSON serialized list of transaction alias names</returns>
		protected string GetTransactionAliasNames()
		{
			// Look up the dispatch transaction alias names.
			var transactionAliasNames =
				FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
					transactionAliases => transactionAliases.EnumerateDispatchAliasNames(this.Security));
			List<string> aliasNames = transactionAliasNames.Select(x => x.AliasName).ToList();
			var serializer = new JavaScriptSerializer();
			return serializer.Serialize(aliasNames);
		}

		/// <summary>
		/// Creates a list of dispatch transaction status values.  The list is serialized in JSON format for use
		/// by the IE browser client to dynamically populate the status dropdown select control.
		/// </summary>
		/// <returns>The JSON serialized list of transaction status values</returns>
		protected string GetTransactionStatusValues()
		{
			// Make the statuses match the Client Dispatch.
			List<string> transactionStatuses = new List<string>
			                                   {
				                                   "{All}",
				                                   "Requested",
				                                   "Dispatched",
				                                   "Arrived",
				                                   "Started",
				                                   "Stopped",
				                                   "Completed",
				                                   "Cancelled",
				                                   "Pending",
				                                   "Posted"
			                                   };

			var serializer = new JavaScriptSerializer();
			return serializer.Serialize(transactionStatuses);
		}

		/// <summary>
		/// This method will retrieve the operational lock date for the given
		/// site.
		/// </summary>
		/// <returns></returns>
		protected string GetOperationalLockDate()
		{
			SiteClass site =
				FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

			DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();

			DateTime siteOperationalLockDate = Convert.ToDateTime(site.OperationalLockDate, dateTimeFormatInfo);
			DateTimeOffset operationLockDateTimeOffset = new DateTimeOffset(siteOperationalLockDate);

			string operationalLockDateStr = operationLockDateTimeOffset.ToString("ddd MMM d yyyy hh:mm:ss zz") + "00";

			return operationalLockDateStr;
		}

		protected void SetOperationalTimes(FMDispatchHiddenFields hiddenFields)
		{
			this.JsonOptionalTimeArrivalFlag = "F";
			this.JsonOptionalTimeStartFlag = "F";
			this.JsonOptionalTimeStopFlag = "F";

			hiddenFields.JsonOptionalTimesArrivalFlagValue = "F";
			hiddenFields.JsonOptionalTimesStartFlagValue = "F";
			hiddenFields.JsonOptionalTimesStopFlagValue = "F";

			// Get the site-specific dispatch configuration which contains optional times settings.
			DispatchConfigurationClass dispatchConfig = FMChannelHelper.MakeCall<IDispatchConfigurations, DispatchConfigurationClass>(dispatchConfigurations =>
			{
				bool entityAssigned;
				var dispatchConfigurationGuid = dispatchConfigurations.GetIdentityGuidBySiteIdAndAssigned(this.Security, this.Security.SiteGuid, DispatchConfigurationClass.DefaultId, true, out entityAssigned);
				if (dispatchConfigurationGuid != Guid.Empty)
				{
					return dispatchConfigurations.Get(this.Security, dispatchConfigurationGuid);
				}

				return null;
			});

			// Make use of the site-specific dispatch optional time values.
			if (dispatchConfig != null)
			{
				if (dispatchConfig.UseArrivalTime)
				{
					this.JsonOptionalTimeArrivalFlag = "T";
					hiddenFields.JsonOptionalTimesArrivalFlagValue = "T";
				}

				if (dispatchConfig.UseStartTime)
				{
					this.JsonOptionalTimeStartFlag = "T";
					hiddenFields.JsonOptionalTimesStartFlagValue = "T";
				}

				if (dispatchConfig.UseStopTime)
				{
					this.JsonOptionalTimeStopFlag = "T";
					hiddenFields.JsonOptionalTimesStopFlagValue = "T";
				}
				return;
			}

			string optionalTimesSettingValue = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
										x => x.GetKeyValueByKey(this.Security, OptionalTimesPage.WebDispatchOptionTimesConfigSettingKey));

			if (string.IsNullOrEmpty(optionalTimesSettingValue) == false)
			{
				// Parse "Arrival:T|Start:T|Stop:T"
				string[] parts = optionalTimesSettingValue.Split('|');

				if (parts.Length < 3)
				{
					throw new Exception("Invalid parse of: " + optionalTimesSettingValue);
				}

				string[] arrivalTimeParts = parts[0].Split(':');
				string[] startTimeParts = parts[1].Split(':');
				string[] stopTimeParts = parts[2].Split(':');

				if (arrivalTimeParts.Length == 2)
				{
					if (arrivalTimeParts[1].ToUpper().Equals("T"))
					{
						this.JsonOptionalTimeArrivalFlag = "T";
						hiddenFields.JsonOptionalTimesArrivalFlagValue = "T";
					}
				}

				if (startTimeParts.Length == 2)
				{
					if (startTimeParts[1].ToUpper().Equals("T"))
					{
						this.JsonOptionalTimeStartFlag = "T";
						hiddenFields.JsonOptionalTimesStartFlagValue = "T";
					}
				}

				if (stopTimeParts.Length == 2)
				{
					if (stopTimeParts[1].ToUpper().Equals("T"))
					{
						this.JsonOptionalTimeStopFlag = "T";
						hiddenFields.JsonOptionalTimesStopFlagValue = "T";
					}
				}
			}
		}

		/// <summary>
		/// Creates a list of grid column definitions based on the specified grid type and the current 
		/// configuration settings.  If no grid configuration exists for the specified grid type then 
		/// the default grid columns are used.  The list is serialized in JSON format for use by the
		/// IE browser client to dynamically create the specified grid.
		/// </summary>
		/// <param name="gridType">The specified grid type</param>
		/// <param name="gridId">The grid ID of the specified grid type</param>
		/// <returns>The JSON serialized list of grid column definitions</returns>
		protected string GetGridColumnDefinitions(int gridType, string gridId)
		{
			var dispatchGrid = new DispatchGridClass();
			// Get the specified grid
			FMChannelHelper.MakeCall<IDispatchGrids>(
				dispatchGrids =>
				{
					Guid dispatchGridGuid = dispatchGrids.GetIdentityGuidById(
						this.Security, gridId, this.DispatchConfigurationGuid);

					if (dispatchGridGuid != Guid.Empty)
					{
						dispatchGrid = dispatchGrids.Get(this.Security, dispatchGridGuid);
					}
				});

			// Get the list of column types for the specified grid
			var gridColumnTypes = FMChannelHelper.MakeCall<IDispatchGridColumns, List<DispatchGridColumnType>>(
				gridColumns => gridColumns.EnumerateColumnTypes(this.Security, gridType, true));

			var columnDefs = new List<GridColumDefinition>();
			var gridColumnDef = new GridColumDefinition();

			if (dispatchGrid.GridColumnList.Count > 0)
			{
				// Create grid column list based on specified grid configuration
				foreach (DispatchGridColumnClass gridColumn in dispatchGrid.GridColumnList)
				{
					if (gridColumn.GridColumnType == DispatchGridColumnType.TransactionAliasUserDataColumnType)
					{
						gridColumnDef.Id = gridColumn.AliasName + '_' + gridColumn.ID;
						gridColumnDef.DisplayName = gridColumn.ID + '(' + gridColumn.AliasName + ')';
						gridColumnDef.DataField = "UserData" + gridColumn.ColumnOrder;
						gridColumnDef.Width = DispatchGridColumnType.DefaultColumnWidth;
						columnDefs.Add(gridColumnDef);
					}
					else if (gridColumn.GridColumnType == DispatchGridColumnType.TransactionAliasLineItemUserDataColumnType)
					{
						gridColumnDef.Id = gridColumn.AliasName + "_LineItem_" + gridColumn.ID;
						gridColumnDef.DisplayName = gridColumn.ID + '(' + gridColumn.AliasName + ')';
						gridColumnDef.DataField = "LineItemUserData" + gridColumn.ColumnOrder;
						gridColumnDef.Width = DispatchGridColumnType.DefaultColumnWidth;
						columnDefs.Add(gridColumnDef);
					}
					else
					{
						foreach (var gridColumnType in gridColumnTypes)
						{
							if (gridColumnType.LookupIndex == gridColumn.GridColumnType)
							{
								gridColumnDef.Id = gridColumnType.Id;
								gridColumnDef.DisplayName = this.GetTranslatedText(gridColumnType.DisplayName);
								gridColumnDef.DataField = gridColumnType.DataField;
								gridColumnDef.Width = gridColumnType.Width;
								columnDefs.Add(gridColumnDef);
								break;
							}
						}
					}
				}
			}
			else
			{
				// Create default grid column list
				foreach (var gridColumnType in gridColumnTypes)
				{
					if (gridColumnType.DefaultColumnOrder > 0)
					{
						gridColumnDef.Id = gridColumnType.Id;
						gridColumnDef.DisplayName = this.GetTranslatedText(gridColumnType.DisplayName);
						gridColumnDef.DataField = gridColumnType.DataField;
						gridColumnDef.Width = gridColumnType.Width;
						columnDefs.Add(gridColumnDef);
					}
				}
			}

			var serializer = new JavaScriptSerializer();
			return serializer.Serialize(columnDefs);
		}

		/// <summary>
		///    Parses the transaction ids.
		/// </summary>
		/// <param name="transactionIds">The transaction ids.</param>
		/// <returns>An array of transaction data objects.</returns>
		protected List<TransactionDO> ParseTransactionIds(string transactionIds)
		{
			string[] ids = transactionIds.Split(',');
			var transactions = new List<TransactionDO>();

			foreach (string id in ids)
			{
				if (string.IsNullOrEmpty(id) == false)
				{
					if (transactions.FindIndex(x => x.TransID == id) == -1)
					{
						TransactionDO transaction = this.GetTransaction(id);
						transactions.Add(transaction);
					}
				}
			}

			return transactions;
		}

		/// <summary>
		///    Saves the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>A save transaction result data object.</returns>
		protected SaveTransactionsResultDO SaveTransaction(TransactionDO transaction)
		{
			return this.SaveTransaction(transaction, null);
		}

		/// <summary>
		///    Saves the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="operatorObject">The operator.</param>
		/// <returns>A save transaction result data object.</returns>
		protected SaveTransactionsResultDO SaveTransaction(TransactionDO transaction, PersonClass operatorObject)
		{
			SaveTransactionsResultDO results = null;

			try
			{
				// Check the aviation and capitalize flags against the product configuration
				LineItemDO lineItem = transaction.LineItems[0];

				ProductClass product =
					FMChannelHelper.MakeCall<IProducts, ProductClass>(
						x => x.GetByInfoAuthorizedCompanies(this.Security, lineItem.ProductGuid, false, true));

				transaction.Flag02 = false;
				
				if (product.UserData1.Equals("YES", StringComparison.CurrentCultureIgnoreCase))
				{
					transaction.Flag02 = true;
				}

				transaction.Flag01 = false;
				
				if (product.UserData2.Equals("YES", StringComparison.CurrentCultureIgnoreCase))
				{
					transaction.Flag01 = true;
				}

				transaction.UserData[TransactionDO.USER_DATA_KEY_09] = "9 (LOCAL)";

				SiteClass site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
						x.Get(
							this.Security, 
							this.Security.SiteGuid, 
							getMemberSites: true, 
							getSchedulesAndProcessVariables: true, 
							bGetAssociatedAliases: true));

				TransactionAliasClass transactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x => x.Get(this.Security, transaction.TransactionAliasGuid, false));

				var unitsHelper = new UnitsHelperClass(this.Security, site, transactionAlias, null);
				unitsHelper.SetUnits(transaction, 0);

				FMChannelHelper.MakeCall<IProducts>(x => this.GetUnitsValue(x, transaction, unitsHelper));

				var saveSR = new SaveTransactionsSR
					{
						Security = this.Security, 
						CurrentSiteGuid = this.Security.SiteGuid, 
						ConvertUnits = true
					};
				saveSR.Transactions.Add(transaction);

				saveSR.Operator = operatorObject;

				results =
					FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(x => x.SaveTransactions(saveSR));

				this.CheckForAndDisplayWarningMessages(results);
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

				this.ErrorHandler("FuelsManager", errorMessage);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return results;
		}

		/// <summary>
		///    Un-dispatches the transaction.
		/// </summary>
		/// <param name="transaction">The transaction to un-dispatch.</param>
		protected void UndispatchTransaction(TransactionDO transaction)
		{
			PersonClass person = null;
			if (transaction.OperatorPersonnelGuid != Guid.Empty)
			{
				person =
					FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(this.Security, transaction.OperatorPersonnelGuid));

				if (person.IdentityGuid.IsEmpty() == false)
				{
					person.Status = PersonClass.STATUS.In;
				}
			}

			LineItemDO lineItem = transaction.LineItems[0];

			transaction.Status = TransactionStatus.Requested;
			lineItem.Status = TransactionStatus.Requested;

			transaction.OperatorPersonnelGuid = Guid.Empty;
			transaction.OperatorID = string.Empty;
			transaction.OperatorName = string.Empty;
			transaction.DispatchedDateTime = null;
			lineItem.DispatchedDateTime = null;
			transaction.IssuePoint = string.Empty;
			transaction.IssuePointNumber = string.Empty;

			DispatchConfigurationClass dispatchConfig = this.GetConfigurationSettings();

			if (dispatchConfig.EquipmentRequired)
			{
				switch (transaction.TransTypeID)
				{
					case TransactionTypes.T3_PrimaryDefuel:
					case TransactionTypes.T4_SecondaryDefuel:
					case TransactionTypes.T7_FillStand:
						transaction.DestinationEQ1 = new EquipmentDO();
						lineItem.DestinationEQ = new EquipmentDO();
						break;

					case TransactionTypes.T5_PrimaryDisbursement:
					case TransactionTypes.T6_SecondaryDisbursement:
					case TransactionTypes.T10_Unload:
					case TransactionTypes.T12_InventoryNotAffected:
						transaction.SourceEQ1 = new EquipmentDO();
						lineItem.SourceEQ = new EquipmentDO();
						break;

					default:
						throw new ApplicationException(
							"Unhandled transaction type passed to dispatch: " + transaction.TransTypeID.ToString());
				}
			}

			// Finish and save
			this.SaveTransaction(transaction, person);
		}

		/// <summary>
		///    Gets the units value.
		/// </summary>
		/// <param name="products">The products.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="unitsHelper">The units helper.</param>
		private void GetUnitsValue(IProducts products, TransactionDO transaction, UnitsHelperClass unitsHelper)
		{
			foreach (LineItemDO item in transaction.LineItems)
			{
				ProductClass prod = products.GetByInfoAuthorizedCompanies(this.Security, item.ProductGuid, true, false);
				unitsHelper.SetUnits(item, prod.ProductType, prod);
			}
		}

		#endregion

		/// <summary>
		/// Grid column definition used in the dynamic creation of the grids on the tabular and dispatching
		/// view pages.  The names of these fields must not be changed without updating the associated javascript
		/// functions TabularViewLib.getDefaultGridSettings() and DispatchingViewLib.getDefaultRequestGridSettings.
		/// </summary>
		public struct GridColumDefinition
		{
			/// <summary>
			/// Slick grid column id
			/// </summary>
			public string Id;

			/// <summary>
			/// Slick grid column display name
			/// </summary>
			public string DisplayName;

			/// <summary>
			/// Slick grid column data field
			/// </summary>
			public string DataField;

			/// <summary>
			/// Slick grid column width
			/// </summary>
			public int Width;
		}
	}
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchRequests.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchRequests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// Dispatch requests service class for Dispatch use interfacing with FuelsManager Business Services.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class DispatchRequests : IDispatchRequests
	{
		/// <summary>
		/// Determines whether new records exist.
		/// </summary>
		/// <param name="checkValue">The check value</param>
		/// <param name="topVersion">The top version</param>
		/// <returns>True if new records exist, false otherwise.</returns>
		private static bool HasNewRecords(string checkValue, string topVersion)
		{
			uint a = Convert.ToUInt32(checkValue, 16);
			uint b = Convert.ToUInt32(topVersion, 16);

			return a > b;
		}

		/// <summary>
		/// Checks if equipment should be colored gray on view.
		/// </summary>
		/// <param name="equipment">The equipment.</param>
		/// <param name="timeConverter">The time converter.</param>
		/// <returns>True if the equipment should be colored gray on the view.</returns>
		private static bool CheckIfEquipmentShouldBeGray(EquipmentClass equipment, SiteTimeConverter timeConverter)
		{
			if (equipment.LockedOut)
			{
				return true;
			}

			if (equipment._QCDate.Value != DateTimeOffset.MinValue
				&& equipment._QCDate.Value < timeConverter.Today())
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Set the equipment display colors.
		/// </summary>
		/// <param name="equipment">The equipment class</param>
		/// <param name="equipmentDo">The equipment do</param>
		/// <param name="timeConverter">The time converter</param>
		private static void SetEquipmentColors(EquipmentClass equipment, DispatchEquipmentDisplayDO equipmentDo, SiteTimeConverter timeConverter)
		{
			equipmentDo.SelectionBackColor = "Black";

			if (CheckIfEquipmentShouldBeGray(equipment, timeConverter))
			{
				equipmentDo.ForeColor = "Gray";
				equipmentDo.SelectionForeColor = "Gray";
			}
			else
			{
				if (equipment.InServiceFlag)
				{
					if (equipment.FuelingType == FUELING_TYPES.REFUELER)
					{
						equipmentDo.ForeColor = "Blue";
						equipmentDo.SelectionForeColor = "Yellow";
					}
					else if (equipment.FuelingType == FUELING_TYPES.DEFUELER)
					{
						equipmentDo.ForeColor = "Red";
						equipmentDo.SelectionForeColor = "Red";
					}
					else
					{
						equipmentDo.ForeColor = "Black";
						equipmentDo.SelectionForeColor = "White";
					}
				}
				else
				{
					equipmentDo.ForeColor = "Gray";
					equipmentDo.SelectionForeColor = "Gray";
				}
			}
		}

		/// <summary>
		/// Sets the personnel colors.
		/// </summary>
		/// <param name="person">The person class</param>
		/// <param name="personnelDo">The personnel display data object</param>
		private static void SetPersonnelColors(PersonClass person, DispatchPersonnelDisplayDO personnelDo)
		{
			personnelDo.SelectionBackColor = "Black";

			if (person.LockedOut)
			{
				personnelDo.ForeColor = "Gray";
				personnelDo.SelectionForeColor = "Gray";
			}
			else if (person.Status == PersonClass.STATUS.Out)
			{
				personnelDo.ForeColor = "Gray";
				personnelDo.SelectionForeColor = "Gray";
			}
			else
			{
				personnelDo.ForeColor = "Blue";
				personnelDo.SelectionForeColor = "Yellow";
			}
		}

		/// <summary>
		/// Sets the alias display colors.
		/// </summary>
		/// <param name="row">A DispatchTransaction object.</param>
		private static void SetAliasDisplayColors(DispatchTransaction row)
		{
			if (row.Status.Equals("Completed", StringComparison.InvariantCultureIgnoreCase)
				|| row.Status.Equals("Cancelled", StringComparison.InvariantCultureIgnoreCase))
			{
				row.Color = "darkGray";
				row.ForeColor = "darkGray";
				row.SelectionForeColor = "white";
			}
			else if (row.AliasName.Equals("Refuel", StringComparison.InvariantCultureIgnoreCase))
			{
				row.Color = "blue";
				row.ForeColor = "blue";
				row.SelectionForeColor = "Yellow";
			}
			else if (row.AliasName.Equals("Defuel", StringComparison.InvariantCultureIgnoreCase))
			{
				row.Color = "red";
				row.ForeColor = "red";
				row.SelectionForeColor = "Cyan";
			}
			else
			{
				row.Color = "black";
				row.ForeColor = "black";
				row.SelectionForeColor = "white";
			}
		}

		private void GetOptionalTimes(SecurityClass security, out bool arrivalTime, out bool startTime, out bool stopTime)
		{
			arrivalTime = false;
			startTime = false;
			stopTime = false;

			// Get the site-specific dispatch configuration which contains optional times settings.
			DispatchConfigurationClass dispatchConfig = FMChannelHelper.MakeCall<IDispatchConfigurations, DispatchConfigurationClass>(dispatchConfigurations =>
			{
				bool entityAssigned;
				var dispatchConfigurationGuid = dispatchConfigurations.GetIdentityGuidBySiteIdAndAssigned(security, security.SiteGuid, DispatchConfigurationClass.DefaultId, true, out entityAssigned);
				if (dispatchConfigurationGuid != Guid.Empty)
				{
					return dispatchConfigurations.Get(security, dispatchConfigurationGuid);
				}

				return null;
			});

			// Make use of the site-specific dispatch optional time values.
			if (dispatchConfig != null)
			{
				arrivalTime = dispatchConfig.UseArrivalTime;
				startTime = dispatchConfig.UseStartTime;
				stopTime = dispatchConfig.UseStopTime;
				return;
			}

			// Otherwise get and use the global dispatch optional times settings.
			string optionalTimesSettingValue = ((IDispatchRequests)this).RetrieveOptionalTimes(security);
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
						arrivalTime = true;
					}
				}

				if (startTimeParts.Length == 2)
				{
					if (startTimeParts[1].ToUpper().Equals("T"))
					{
						startTime = true;
					}
				}

				if (stopTimeParts.Length == 2)
				{
					if (stopTimeParts[1].ToUpper().Equals("T"))
					{
						stopTime = true;
					}
				}
			}
		}

		private static DateTime ParseToDateTimeOffset(string inDate)
		{
			if (string.IsNullOrEmpty(inDate))
			{
				return TimeConverter.Today().DateTime;
			}

			const int HiddenQuoteMark = 8206;
			char[] charArray = inDate.ToCharArray();
			string strippedDate = String.Empty;

			// Strip the quote marks out of the string.
			foreach (char charValue in charArray)
			{
				if (charValue != HiddenQuoteMark)
				{
					strippedDate = strippedDate + charValue;
				}
			}

			string[] parts = strippedDate.Split('/');

			if (parts.Length < 3)
			{
				return TimeConverter.Today().DateTime;
			}

			try
			{
				int month = Convert.ToInt32(parts[0]);
				int day = Convert.ToInt32(parts[1]);
				int year = Convert.ToInt32(parts[2]);

				DateTime newDateTime = new DateTime(year, month, day, 0, 0, 0);
				return newDateTime;
			}
			catch (Exception)
			{
				return TimeConverter.Today().DateTime;
			}
		}

		#region Public Methods and Operators

		/// <summary>
		/// Enumerates equipment entities for use in Dispatch.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="topVersion">The top Version</param>
		/// <returns>A dispatch equipment data object</returns>
		DispatchEquipmentDO IDispatchRequests.EnumerateEquipment(SecurityClass security, string topVersion)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, bGetMemberSites: false, getSchedulesAndProcessVariables: false, bGetAssociatedAliases: false);
			var timeConverter = new SiteTimeConverter(site);

			var equipments = new EquipmentsClass();
			var equipmentDO = new DispatchEquipmentDO();

			var checkValue = equipments.GetLatestRowVersionBySource(security);
			if (HasNewRecords(checkValue, topVersion))
			{
				equipmentDO.Refreshed = true;
				equipmentDO.TopVersion = checkValue;
				var equipmentList = equipments.EnumerateBySource(security);

				foreach (var equipment in equipmentList)
				{
					var equipmentDo = new DispatchEquipmentDisplayDO(equipment);
					SetEquipmentColors(equipment, equipmentDo, timeConverter);
					equipmentDO.Equipment.Add(equipmentDo);
				}
			}

			return equipmentDO;
		}

		/// <summary>
		/// Enumerates personnel entities for use in Dispatch.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="topVersion">The top Version</param>
		/// <returns>A dispatch personnel data object</returns>
		DispatchPersonnelDO IDispatchRequests.EnumeratePersonnel(SecurityClass security, string topVersion)
		{
			if (security == null)
			{
				throw new ArgumentNullException(nameof(security));
			}

			var personnel = new PersonnelClass();
			var personnelDO = new DispatchPersonnelDO();

			var checkValue = personnel.GetLatestRowVersionByRole(security, PERSON_ROLE.LOADER_ROLE);
			if (HasNewRecords(checkValue, topVersion))
			{
				personnelDO.Refreshed = true;
				personnelDO.TopVersion = checkValue;
				var personnelList = personnel.EnumerateByRole(security, PERSON_ROLE.LOADER_ROLE);

				foreach (var person in personnelList)
				{
					var displayDo = new DispatchPersonnelDisplayDO(person);
					SetPersonnelColors(person, displayDo);
					personnelDO.Personnel.Add(displayDo);
				}
			}

			return personnelDO;
		}

		/// <summary>
		/// Enumerates standby personnel for use in Dispatch.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>A list of dispatch personnel display data objects</returns>
		List<DispatchPersonnelDisplayDO> IDispatchRequests.EnumerateStandbyPersonnel(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var standbyDrivers = new List<DispatchPersonnelDisplayDO>();

			IPersonnel personnelInterface = new PersonnelClass();

			PersonCollectionClass allDrivers = personnelInterface.EnumerateByRole(security, PERSON_ROLE.LOADER_ROLE);

			foreach (PersonClass person in allDrivers)
			{
				if (person.Status == PersonClass.STATUS.STB)
				{
					standbyDrivers.Add(new DispatchPersonnelDisplayDO(person));
				}
			}

			return standbyDrivers;
		}

		/// <summary>
		/// Enumerates the transactions.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="topVersion">The top version.</param>
		/// <param name="beginDate">The begin date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="status">The status.</param>
		/// <param name="requestName">Name of the request.</param>
		/// <returns>A collection of transaction objects.</returns>
		DispatchTransactionDO IDispatchRequests.EnumerateTransactions(SecurityClass security, string topVersion, string beginDate, string endDate, string status, string requestName)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			DateTime beginDateTime = ParseToDateTimeOffset(beginDate);
			DateTime endDateTime = ParseToDateTimeOffset(endDate);

			// Build the service request object for enumerating dispatch transactions
			var serviceRequest = new DispatchTransactionsSR
								{
									Security = security,
									Site = security.SiteID,
									CurrentSiteGuid = security.SiteGuid,
									BeginDate = beginDateTime,
									EndDate = endDateTime
								};

			// If the requestName filter was specified, make it the alias name for the request.
			if (string.IsNullOrEmpty(requestName) == false)
			{
				serviceRequest.AliasNames.Add(requestName);
			}
			else
			{
				// Look up the dispatch transaction aliases.
				var transactionAliases = new TransactionAliasesClass();
				var dispatchAliases = transactionAliases.EnumerateDispatchAliasNames(security);
				serviceRequest.AliasNames = dispatchAliases.Select(x => x.AliasName).ToList();
			}
			// If a status was passed for filter, add it to the service request.
			if (string.IsNullOrEmpty(status) == false)
			{
				string[] statuses = status.Split(',');
				foreach (var s in statuses)
				{
					serviceRequest.Statuses.Add(s);
				}
			}

			var result = new DispatchTransactionDO();

			var dispatchTransactions = new DispatchTransactionsProcessor();

			if (dispatchTransactions.GetTopLineItemVersion(serviceRequest, topVersion))
			{
				var transactions = dispatchTransactions.GetLineItems(serviceRequest);

				if (transactions.Transactions.Tables.Count > 0 && transactions.Transactions.Tables[0].Rows.Count > 0)
				{
					var table = transactions.Transactions.Tables[0];
					Load(table, result);

					// Get the checkVersion
					result.TopVersion = DataObject.getString(table.Rows[table.Rows.Count - 1]["RowVersionString"]);

					result.Refreshed = true;
				}
			}

			return result;
		}

		/// <summary>
		/// Loads the specified table.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="result">The result.</param>
		/// <param name="timePattern">The time pattern.</param>
		private static void Load(DataTable table, DispatchTransactionDO result)
		{
			foreach (DataRow row in table.Rows)
			{
				var trans = new DispatchTransaction(row, "HH:mm");
				SetAliasDisplayColors(trans);
				result.Transactions.Add(trans);
			}
		}

		/// <summary>
		/// Gets the dictionary translation.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="key">The key to translate.</param>
		/// <returns>A translated string value.</returns>
		string IDispatchRequests.GetDictionaryTranslation(SecurityClass security, string key)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			IDataDictionariesClass dict = new DataDictionariesClass();
			return dict.Get(security.SiteGuid, key);
		}

		/// <summary>
		/// Sets status to Arrived for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Dispatched will be processed.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <returns>The number of transactions statuses set to Arrived</returns>
		int IDispatchRequests.SetArrived(SecurityClass security, string[] transactionIds)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);
			var timeConverter = new SiteTimeConverter(site);

			ITransactionProcessor transactionProcessor = new TransactionProcessorClass();
			ISaveTransactionsProcessor saveTransactionProcessor = new SaveTransactionsProcessor();

			int numArrived = 0;
			foreach (string transactionId in transactionIds)
			{
				// Get transaction with specified transaction ID
				var serviceRequest = new TransactionSR { Security = security, TransID = transactionId };
				TransactionDO transaction = transactionProcessor.Process(serviceRequest);

				// Skip transactions that are not dispatched
				if (transaction.Status != TransactionStatus.Dispatched)
				{
					continue;
				}

				// Set status to Arrived for transaction
				transaction.Status = TransactionStatus.Arrived;
				transaction.TimeIn = timeConverter.Now();

				// Save transaction with its updated status
				var saveServiceRequest = new SaveTransactionsSR
				{
					Security = security,
					CurrentSiteGuid = security.SiteGuid,
					IndividualDbTransaction = false,
					ConvertUnits = true
				};
				saveServiceRequest.Transactions.Add(transaction);
				saveTransactionProcessor.SaveTransactions(saveServiceRequest);
				++numArrived;
			}

			return numArrived;
		}

		/// <summary>
		/// Sets status to Started for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Arrived will be processed.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <returns>The number of transactions statuses set to Started</returns>
		int IDispatchRequests.SetServiceStarted(SecurityClass security, string[] transactionIds)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);
			var timeConverter = new SiteTimeConverter(site);

			ITransactionProcessor transactionProcessor = new TransactionProcessorClass();
			ISaveTransactionsProcessor saveTransactionProcessor = new SaveTransactionsProcessor();

			bool arrivalTimeRequired;
			bool startTimeRequired;
			bool stopTimeRequired;

			this.GetOptionalTimes(security, out arrivalTimeRequired, out startTimeRequired, out stopTimeRequired);


			int numStarted = 0;
			foreach (string transactionId in transactionIds)
			{
				// Get transaction with specified transaction ID
				var serviceRequest = new TransactionSR { Security = security, TransID = transactionId };
				TransactionDO transaction = transactionProcessor.Process(serviceRequest);

				// Skip transactions that are not Arrived
				if (arrivalTimeRequired && transaction.Status != TransactionStatus.Arrived)
				{
					continue;
				}

				// Set status to Started for transaction
				transaction.Status = TransactionStatus.Started;
				transaction.RouteSchedule.FST = timeConverter.Now();

				// Save transaction with its updated status
				var saveServiceRequest = new SaveTransactionsSR
				{
					Security = security,
					CurrentSiteGuid = security.SiteGuid,
					IndividualDbTransaction = false,
					ConvertUnits = true
				};
				saveServiceRequest.Transactions.Add(transaction);
				saveTransactionProcessor.SaveTransactions(saveServiceRequest);
				++numStarted;
			}

			return numStarted;
		}

		/// <summary>
		/// Sets status to Stopped for a set of transactions given an array of transaction Ids.
		/// Only transactions with statuses of Started will be processed.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="transactionIds">The array of transaction Ids</param>
		/// <returns>The number of transactions statuses set to Stopped</returns>
		int IDispatchRequests.SetServiceStopped(SecurityClass security, string[] transactionIds)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var sites = new SitesClass();
			SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);
			var timeConverter = new SiteTimeConverter(site);

			bool arrivalTimeRequired;
			bool startTimeRequired;
			bool stopTimeRequired;

			this.GetOptionalTimes(security, out arrivalTimeRequired, out startTimeRequired, out stopTimeRequired);

			ITransactionProcessor transactionProcessor = new TransactionProcessorClass();
			ISaveTransactionsProcessor saveTransactionProcessor = new SaveTransactionsProcessor();

			int numStopped = 0;
			foreach (var transactionId in transactionIds)
			{
				// Get transaction with specified transaction ID
				var serviceRequest = new TransactionSR { Security = security, TransID = transactionId };
				TransactionDO transaction = transactionProcessor.Process(serviceRequest);

				// Skip transactions that are not Started
				if (startTimeRequired && transaction.Status != TransactionStatus.Started)
				{
					continue;
				}

				// Set status to Stopped for transaction
				transaction.Status = TransactionStatus.Stopped;
				transaction.TimeEnd = timeConverter.Now();

				// Save transaction with its updated status
				var saveServiceRequest = new SaveTransactionsSR
				{
					Security = security,
					CurrentSiteGuid = security.SiteGuid,
					IndividualDbTransaction = false,
					ConvertUnits = true
				};
				saveServiceRequest.Transactions.Add(transaction);
				saveTransactionProcessor.SaveTransactions(saveServiceRequest);
				++numStopped;
			}

			return numStopped;
		}

		/// <summary>
		/// This method will retrieve the optional times configuration information
		/// that is saved by the web dispatch optional times page.
		/// </summary>
		/// <param name="security">he security object</param>
		/// <returns>Returns optional times configuration.</returns>
		string IDispatchRequests.RetrieveOptionalTimes(SecurityClass security)
		{
			const string OptionalTimesConfigKey = "WebDispatchOptionalTimes";

			ConfigurationSettingsClass configSettings = new ConfigurationSettingsClass();
			return configSettings.GetKeyValueByKey(security, OptionalTimesConfigKey);
		}

		/// <summary>
		/// Verify that the specified lock out date is not after the current date, i.e. not a future date.
		/// Verify that the specified lock out date is not before the current lock out date.
		/// Verify that all transactions prior to the specified lock out date, with application orgin of 
		/// Dispatch, and submitted to accounting flag of false have a status of either Completed or Cancelled.
		/// Consider only those transactions with refuel and defuel alias names if no transaction alias exists.
		/// Otherwise consider only those transactions where transaction alias IncludeInDispatch flag is set
		/// and the transaction type is a refuel or defuel transaction type.
		/// Set SubmittedToAccounting flag to true for applicable transactions (ones that satisfy above conditions.)
		/// Set the OperationalLockDate in the Site table to the newly specified lock out date.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="lockOutDate">The lock out date</param>
		/// <returns>The result status of the operation</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		Dictionary<string, string> IDispatchRequests.ReleaseToAccounting(SecurityClass security, DateTimeOffset lockOutDate)
		{
			Dictionary<string, string> messages = new Dictionary<string, string>();

			if (security == null)
			{
				throw new ArgumentException("Invalid security token.");
			}

			// Verify that the specified lock out date is not after the current date, i.e. not a future date.
			if (lockOutDate > DateTimeOffset.Now)
			{
				messages.Add("Failed", "Lockout date must not be after the current date.");
				return messages;
			}

			// Verify that the specified lock out date is not before the current lock out date.
			SiteClass currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
				sites => sites.Get(security, security.SiteGuid, false, false, false));

			DateTimeOffset currentLockOutDate;
			bool validValue = DateTimeOffset.TryParse(currentSite.OperationalLockDate, out currentLockOutDate);
			if (!validValue)
			{
				currentLockOutDate = DateTimeOffset.Now;
			}

			if (lockOutDate < currentLockOutDate)
			{
				messages.Add("Failed", "Lockout date must not be before the current lockout date of " + currentLockOutDate.LocalDateTime);
				return messages;
			}

			// Verify that all transactions prior to the specified lock out date, with application orgin of 
			// Dispatch, and submitted to accounting flag of false have a status of either Completed or Cancelled.
			var consolidatedDa = new ConsolidatedDAClass();
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = string.Format(
					"SELECT COUNT(*) AS 'RowCount'" +
					" FROM tblTransactions t WITH (NOLOCK)" +
					" INNER JOIN [erv].[udf_GetTransactionAliasRecordVersions](@TargetSiteGuid) b" +
					" ON b.MasterRecordGuid = t.TransactionAliasGuid" +
					" INNER JOIN tblTransactionAliases ta WITH (NOLOCK)" +
					" ON ta.TransactionAliasGuid = b.TransactionAliasGuid" +
					" WHERE LookupOriginApplicationIndex IN ({0})" +
					" AND SubmittedToAccounting = 0" +
					" AND TransDateTime < @LockOutDate" +
					" AND (t.TransactionAliasGuid IS NOT NULL AND (t.AliasName LIKE '%sale%' OR t.AliasName LIKE '%defuel%')" +
						" OR (IncludeInDispatch > 0 AND t.LookupTransTypeIndex IN (3,4,5,6)))" +
					" AND LookupTransactionStatusIndex NOT IN (0,7)", TransactionOriginExtensions.GetDispatchOriginList());

				cmd.Parameters.AddWithValue("@LockOutDate", lockOutDate);
				cmd.Parameters.AddWithValue("@TargetSiteGuid", currentSite.SiteGuid);
				DataSet set = consolidatedDa.GetDataSet(cmd, security);

				if (set != null && set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0)
				{
					int rowCount = (int)set.Tables[0].Rows[0]["RowCount"];

					if (rowCount > 0)
					{
						messages.Add("Failed", "Not all dispatch transactions prior to the lockout date had a status of either Completed or Cancelled.");
					}
				}
			}

			// Set SubmittedToAccounting flag to true for applicable transactions.
			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = string.Format(
					"UPDATE tblTransactions WITH(ROWLOCK) SET" +
					" SubmittedToAccounting = 1," +
					" UpdatedDate = @UpdatedDate," +
					" UpdatedBy = @UpdatedBy" +
					" WHERE LookupOriginApplicationIndex IN ({0})" +
					" AND SubmittedToAccounting = 0" +
					" AND TransDateTime < @LockOutDate" +
					" AND (TransactionAliasGuid IS NOT NULL AND (AliasName LIKE '%sale%' OR AliasName LIKE '%defuel%')" +
					" OR ((SELECT IncludeInDispatch FROM tblTransactionAliases ta WHERE ta.TransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', tblTransactions.TransactionAliasGuid, @SiteGuid)) > 0" +
					" AND LookupTransTypeIndex IN (3,4,5,6)))" +
					" AND LookupTransactionStatusIndex IN (0,7)", TransactionOriginExtensions.GetDispatchOriginList());

				cmd.Parameters.AddWithValue("@LockOutDate", lockOutDate);
				cmd.Parameters.AddWithValue("@UpdatedDate", DateTimeOffset.Now);
				cmd.Parameters.AddWithValue("@UpdatedBy", security.UserID);
				cmd.Parameters.AddWithValue("@SiteGuid", currentSite.SiteGuid);
				int releaseCount = consolidatedDa.ExecuteQuery(security, cmd);

				if (releaseCount > 0)
				{
					messages.Add("OK", releaseCount + " record(s) successfully released to Accounting.");
				}
				else
				{
					messages.Add("OK", "Zero records were released to Accounting.");
				}
			}

			return messages;
		}
		#endregion
	}
}

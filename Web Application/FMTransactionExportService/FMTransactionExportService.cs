using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using Varec.CommonComponents.EngineeringUnitsLibrary;


namespace FMTransactionExportService
{
	public partial class FMTransactionExportService : ServiceBase
	{
		private ManualResetEvent killEvent;
		private ManualResetEvent killAckEvent;
		private Thread ExportThread;
		public const string UserId = "Administrator";
		protected SecurityClass Security;
		private const string lastRunKey = "TransactionExportServiceLastRowVersion";
		private Dictionary<string, string> ProductDic;
		private Dictionary<Columns, string> DefaultCloumnValuesDic;
		private ConversionType ConversionTypeSetting = ConversionType.DontConvert;
		private string DATE_FORMAT = "MM-dd-yyyy HH:mm:ss";
		private string TIME_FORMAT = "HH:mm:ss";
		private const double MIN_TRMPERATURE_CELSIUS = -28.88888889; // -20 degrees F 
		private const double MAX_TRMPERATURE_CELSIUS = 48.88888889; // 120 degrees F
		private const int ExportCheckRateMilliseconds = 1000;
		private const string ExportInterfaceName = "FMTransactionExportService";
		private HashSet<string> LoadRacks = new HashSet<string>();
		private Columns? OperatorIdColumn;
		private Columns? TransLineOperatorIdColumn;
		private Columns? FstToUserColumn;
		private Columns? TimeEndColumn;
		private Columns? LoadIDColumn;
		private HashSet<string> LoadingLocationIDWithArmNumberSet;
		public enum ConversionType
		{
			ToSiteUnits = 1,
			ToCongfigFileSettings = 2,
			DontConvert = 3
		}

		public enum Columns
		{
			Transaction_Date,
			Transaction_Type,
			Transaction_ID,
			Gross_Volume,
			Direction,
			Notes,
			Temperature,
			Gravity,
			Meter_Stop,
			Meter_Start,
			Net_Volume,
			Transaction_Alias,
			VCF,
			Product_ID,
			Manager,
			Owner,
			Carrier,
			SubType_Code,
			SubType_Code_2,
			SubTypeCode_3,
			Closeout_Date,
			Serial_Number,
			ShipTo,
			Destination_Reg_ID,
			Document_Number,
			Shipping_Mode_Index,
			FinalFlag,
			Request_Transaction_ID,
			Supplier_Index,
			GBL,
			Carrier_Code,
			Request_By_Date,
			Contract_Index,
			CLIN_Index,
			From_Owner,
			To_Owner,
			From_Manager,
			To_Manager,
			From_Source,
			To_Source,
			From_Carrier,
			To_Carrier,
			From_UserData,
			To_UserData,
			From_Product,
			To_Product,
			Gross_Inventory,
			Net_Inventory,
			Location,
			Registration_ID,
			UserData_1,
			UserData_2,
			UserData_3,
			UserData_4,
			UserData_5,
			UserData_6,
			UserData_7,
			UserData_8,
			UserData_9,
			UserData_10,
			UserData_11,
			UserData_12,
			UserData_13,
			UserData_14,
			UserData_15,
			UserData_16,
			UserData_17,
			UserData_18,
			UserData_19,
			UserData_20,
			UserData_21,
			UserData_22,
			UserData_23,
			UserData_24,
			Deleted,
			UpdatedBy,
			UpdatedDateUTC,
			MeterFactor,
			FuelCP,
			ReversalType,
			BillTo,
		}
		//private DateTime currentStartUpTime;
		public FMTransactionExportService()
		{
			InitializeComponent();
			killEvent = new ManualResetEvent(false);
			killAckEvent = new ManualResetEvent(true);
		}

		public void Start()
		{
			OnStart(new string[0]);
		}

		protected override void OnStart(string[] args)
		{
			try
			{
#if DEBUG
				if (AppSettings.GetDebuggingEnabled())
				{
					while (!Debugger.IsAttached)      // Waiting until debugger is attached
					{
						RequestAdditionalTime(1000);  // Prevents the service from timeout
						Thread.Sleep(1000);           // Gives you time to attach the debugger   
					}
					RequestAdditionalTime(20000);     // for Debugging the OnStart method,
													  // increase as needed to prevent timeouts
				}
#endif
                FMChannelHelper.MakeCall<IHardwareKey,ushort>(x => x.CheckActivatedLicenceVersion());

				ExportThread = new Thread(new ThreadStart(ExportThreadHandler));
				ExportThread.Start();

			}
			catch (Exception ex)
			{
				EventLog.WriteEntry("Unable to start Transaction Export Service:" + Environment.NewLine + "Reason: " + ex.Message + Environment.NewLine + "STACK TRACE: " + Environment.NewLine + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
				killAckEvent.Set();
				Stop();
			}
		}

		public void Kill()
		{
			OnStop();
		}

		protected override void OnStop()
		{
			killEvent.Set();
			killAckEvent.WaitOne();
		}

		private string GenerateVersionHeader()
		{
			return "Version 2.0";
		}
		private string GenerateHeader(List<Columns> columns)
		{
			string[] headers = new string[columns.Count];
			int i = 0;
			foreach (var enumValue in columns)
			{
				var headerTitle = enumValue.ToString();
				if (enumValue == Columns.UpdatedDateUTC)
				{
					// Override vale to include required brackets
					headerTitle = "UpdatedDate(UTC)";
				}
				headers[i++] = headerTitle;
			}

			return string.Join(",", headers);
		}

		private void InitializLoadRacksSet()
		{
			LoadRacks = new HashSet<string>();

			var map = AppSettings.GetLoadRackIdentificationString();
			var pairs = map.Split(',');
			foreach (var pair in pairs)
			{
				string[] values = pair.Split('|');
				if (values.Length == 2)
				{
					string[] arms = values[1].Split('_');
					foreach (var arm in arms)
					{
						LoadRacks.Add(values[0] + "|" + arm);
					}
				}
			}

		}
		private void InitializeCustomItems()
		{
			OperatorIdColumn = AppSettings.GetOperatorIdToColumn();
			TransLineOperatorIdColumn = AppSettings.GetTransLineOperatorIdToColumn();
			FstToUserColumn = AppSettings.GetFstToColumn();
			TimeEndColumn = AppSettings.GetTimeEndtoColumn();
			LoadIDColumn = AppSettings.GetLoadIDtoColumn();
			LoadingLocationIDWithArmNumberSet = AppSettings.GetLoadingLocationIDWithArmNumberSet();

		}
		private Dictionary<Columns, string> ProcessRow(DataRow row, List<Columns> columns)
		{
			var values = new Dictionary<Columns, string>();

			// Initialize all possible column values
			foreach (Columns column in Enum.GetValues(typeof(Columns)))
			{
				values.Add(column, string.Empty);
			}

			// Set all columns defaults from app config file
			foreach (var defaultEntry in DefaultCloumnValuesDic)
			{
				values[defaultEntry.Key] = defaultEntry.Value;
			}

			bool isLoadRack = false;
			if (row["LoadingLocationID"].ToString() != null && row["ArmNumber"].ToString() != null)
			{
				isLoadRack = LoadRacks.Contains(row["LoadingLocationID"].ToString() + "|" + row["ArmNumber"].ToString());
			}
			bool bPhysicalInventory = (Convert.ToInt32(row["LookupTransTypeIndex"]) == 14) ? true : false;
			DateTime dt = Convert.ToDateTime(row["InventoryDate"]); //TransactionDate
			values[Columns.Transaction_Date] = dt != null ? dt.ToString(DATE_FORMAT) : "";
			values[Columns.Transaction_Type] = isLoadRack ? "7" : row["LookupTransTypeIndex"].ToString();
			values[Columns.Transaction_ID] = bPhysicalInventory ? row["TransID"].ToString() + "_" + row["LineNumber"].ToString() : row["TransID"].ToString();


			int vol = AppSettings.GetDefaultConverstionUnits("DEFAULT_UNITS_VAL_VOL");

			string grossQuantity = row["GrossQuantity"].ToString();
			if (!String.IsNullOrEmpty(grossQuantity))
			{
				double volume = ConvertUnits(Convert.ToDouble(grossQuantity), EngineeringUnit.FmvKl, (EngineeringUnit)vol);
				volume = -volume; // Customer wants the opposite value importing into their systems
				string strVolume = volume.ToString("#0");
				values[Columns.Gross_Volume] = strVolume;
			}


			values[Columns.Notes] = row["Notes"].ToString();

			double dValueTo;
			string strValueTo;
			int iUnits = AppSettings.GetDefaultConverstionUnits("DEFAULT_UNITS_VAL_TEMP");
			bool goodTemp = false;
			bool goodDensity = false;

			#region Temperature Logic

			//Temperature
			//Range checking of temperature from -20 degrees F to 120 degree F if it is still in SI units
			if (row["Temperature"] != DBNull.Value && (!ConversionType.ToSiteUnits.Equals(ConversionTypeSetting) || (
				Convert.ToDouble(row["Temperature"]) >= MIN_TRMPERATURE_CELSIUS &&
				Convert.ToDouble(row["Temperature"]) <= MAX_TRMPERATURE_CELSIUS)))
			{
				dValueTo = ConvertUnits(Convert.ToDouble(row["Temperature"]), EngineeringUnit.FmtDegC, (EngineeringUnit)iUnits);
				strValueTo = dValueTo.ToString();
				if (strValueTo.Length > 8)
				{
					values[Columns.Temperature] = strValueTo.Substring(0, 8).TrimEnd(".".ToCharArray());
				}
				else
				{
					values[Columns.Temperature] = strValueTo;
				}
				goodTemp = true;
			}
			#endregion

			#region Density Logic
			//Density
			//iIndexDensity = i;
			iUnits = AppSettings.GetDefaultConverstionUnits("DEFAULT_UNITS_VAL_DENSITY");
			//Range checking of temperature from 1 Kg/M3 to 1074 Kg/M3
			if (row["Density"] != DBNull.Value &&
				Convert.ToDouble(row["Density"]) >= 1.0 &&
				Convert.ToDouble(row["Density"]) <= 1074)
			{
				dValueTo = ConvertUnits(Convert.ToDouble(row["Density"]), EngineeringUnit.FmdKgM3, (EngineeringUnit)iUnits);
				if (Math.Round(dValueTo, 5) > 0.0)
				{
					strValueTo = dValueTo.ToString();
					if (strValueTo.Length > 8)
					{
						values[Columns.Gravity] = strValueTo.Substring(0, 8).TrimEnd(".".ToCharArray());
					}
					else
					{
						values[Columns.Gravity] = strValueTo;
					}
					goodDensity = true;
				}
			}
			#endregion


			string meterSt = row["MeterStop"].ToString();
			if (!String.IsNullOrEmpty(meterSt))
			{
				double metStop = Convert.ToDouble(row["MeterStop"]);
				string metStopStr = metStop.ToString("#0");
				values[Columns.Meter_Stop] = metStopStr;
			}

			string meterSt1 = row["MeterStart"].ToString();
			if (!String.IsNullOrEmpty(meterSt1))
			{
				double metStart = Convert.ToDouble(row["MeterStart"]);
				string metStartStr = metStart.ToString("#0");
				values[Columns.Meter_Start] = metStartStr;
			}

			string netQuantity = row["NetQuantity"].ToString();

			if (!String.IsNullOrEmpty(netQuantity))
			{
				double netVolume = ConvertUnits(Convert.ToDouble(netQuantity), EngineeringUnit.FmvKl, (EngineeringUnit)vol);
				netVolume = -netVolume; // Customer wants the opposite value importing into their systems
				string netVolumeStr = netVolume.ToString("#0");
				values[Columns.Net_Volume] = netVolumeStr;
			}

			values[Columns.Transaction_Alias] = isLoadRack ? "Load Rack" : row["AliasName"].ToString();

			#region VCF Logic
			//VCF
			if (goodDensity && goodTemp)
			{
				values[Columns.VCF] = "";
			}
			else
			{
				if (row["VCF"] != DBNull.Value && Convert.ToDouble(row["VCF"]) > 0.0)
				{
					string strVCF = Convert.ToDouble(row["VCF"]).ToString();
					if (strVCF.Length > 8)
					{
						strVCF = strVCF.Substring(0, 8).TrimEnd(".".ToCharArray());
					}
					values[Columns.VCF] = strVCF;
				}
				else
				{
					values[Columns.VCF] = "1.0";
				}
				values[Columns.Gravity] = "";
				values[Columns.Temperature] = "";
			}
			#endregion


			var product = row["Product"].ToString();
			values[Columns.Product_ID] = ProductDic.ContainsKey(product) ? ProductDic[product] : product;



			values[Columns.Manager] = row["ManagerID"].ToString();
			values[Columns.Owner] = row["OwnerID"].ToString();
			values[Columns.Carrier] = row["CarrierID"].ToString();
			values[Columns.ShipTo] = row["ShipToID"].ToString();
			values[Columns.BillTo] = row["BillToID"].ToString();
			values[Columns.Destination_Reg_ID] = row["DestinationRegistrationID1"].ToString();
			values[Columns.Document_Number] = row["DocumentNumber"].ToString();

			if (LoadingLocationIDWithArmNumberSet.Contains(row["LoadingLocationID"].ToString()))
			{
				values[Columns.Location] = row["LoadingLocationID"].ToString() + row["ArmNumber"].ToString();
			}
			else
			{
				values[Columns.Location] = row["LoadingLocationID"].ToString();
			}

			if (OperatorIdColumn.HasValue)
			{
				// Operator may have carrier appended to it (literally space dash space carrier).
				// If the operator id returned ends with the carrier, strip it (and the " - ")
				string operatorId = RemoveSuffix(row["OperatorID"].ToString(), row["CarrierID"].ToString());
				values[OperatorIdColumn.Value] = operatorId;
			}

			if (TransLineOperatorIdColumn.HasValue)
			{
				// Operator may have carrier appended to it (literally space dash space carrier).
				// If the operator id returned ends with the carrier, strip it (and the " - ")
				string operatorId = RemoveSuffix(row["TranLineOperatorID"].ToString(), row["CarrierID"].ToString());
				values[TransLineOperatorIdColumn.Value] = operatorId;
			}

			if (FstToUserColumn.HasValue)
			{
				if (row["FST"] != null && row["FST"] != DBNull.Value)
				{
					DateTimeOffset fst = (DateTimeOffset)row["FST"];
					values[FstToUserColumn.Value] = fst.ToString(TIME_FORMAT);
				}
			}

			if (TimeEndColumn.HasValue)
			{
				if (row["TimeEnd"] != null && row["TimeEnd"] != DBNull.Value)
				{
					DateTimeOffset timeEnd = (DateTimeOffset)row["TimeEnd"];
					values[TimeEndColumn.Value] = timeEnd.ToString(TIME_FORMAT);
				}
			}

			if (LoadIDColumn.HasValue)
			{
				values[LoadIDColumn.Value] = row["LoadID"].ToString();
			}



			values[Columns.Deleted] = Convert.ToBoolean(row["DeleteFlag"]) ? "1" : "0";
			values[Columns.UpdatedBy] = row["UpdatedBy"].ToString();

			values[Columns.ReversalType] = row["ReversalType"].ToString();


			DateTimeOffset dateTimeOffset = (DateTimeOffset)row["UpdatedDate"];
			values[Columns.UpdatedDateUTC] = dateTimeOffset != null ? dateTimeOffset.ToUniversalTime().ToString(DATE_FORMAT) : "";
			return values;
		}

		private static string RemoveSuffix(string value, string suffix)
		{
			if (value != null && !string.IsNullOrEmpty(suffix) && value.EndsWith(suffix))
			{
				value = value.Remove(value.IndexOf(suffix, StringComparison.Ordinal));
				if (value.EndsWith(" - "))
				{
					value = value.Remove(value.Length - 3);
				}
			}
			return value;
		}

		private List<Dictionary<Columns, string>> ProcessTable(DataTable table, byte[] currentTransVersion, List<Columns> columns, out byte[] lastTransVersion)
		{

			lastTransVersion = currentTransVersion;
			var rows = new List<Dictionary<Columns, string>>();

			foreach (DataRow row in table.Rows)
			{
				rows.Add(ProcessRow(row, columns));
				currentTransVersion = (byte[])row["RowVersion"];

				if (((IStructuralComparable)lastTransVersion).CompareTo(currentTransVersion, Comparer<byte>.Default) < 0)
				{
					lastTransVersion = currentTransVersion;
				}
			}
			return rows;
		}
		private bool WriteExportFile(List<Dictionary<Columns, string>> rows, string path, List<Columns> columns)
		{
			StreamWriter writer = null;
			if (rows == null)
			{
				return false;
			}

			if (rows.Count == 0)
			{
				return false;
			}
			try
			{
				Directory.CreateDirectory(AppSettings.GetExportPath());
				var fileExists = File.Exists(path);
				if (fileExists)
				{
					writer = new StreamWriter(File.Open(path, FileMode.Append, FileAccess.Write, FileShare.None));
				}
				else
				{
					writer = new StreamWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None));
				}
				if (writer == null)
				{
					return false;
				}

				if (fileExists == false)
				{
					writer.WriteLine(GenerateVersionHeader());
					writer.WriteLine(GenerateHeader(columns));
				}


				string line = "";
				var cleaned = "";
				foreach (var row in rows)
				{
					line = "";

					foreach (var column in columns)
					{
						var value = row[column];
						// if any field has a comma remove it to prevent csv corruption'
						cleaned = string.IsNullOrEmpty(value) ? string.Empty : value.Replace(",", " ").Replace("\n", " ").Replace("\r", " ");
						line += cleaned + ",";
					}
					writer.Write(line.TrimEnd(",".ToCharArray()));
					writer.WriteLine();
				}
				writer.Close();
				writer = null;
				EventLog.WriteEntry("Export file written to " + path);
				return true;
			}
			catch (Exception ex)
			{
				if (writer != null)
				{
					writer.Close();
				}
				EventLog.WriteEntry("Problem encountered while writing data to file: " +
									Environment.NewLine + path + Environment.NewLine +
									"Reason: " + ex.Message + Environment.NewLine +
									"STACK TRACE: " + Environment.NewLine + ex.StackTrace +
									"The service will retry the operation in " + AppSettings.GetPollingIntervalSeconds() + " seconds." +
									Environment.NewLine, System.Diagnostics.EventLogEntryType.Warning);
				return false;
			}

		}



		/// <summary>
		/// Sets the status of all provided transactions to <see cref="TransactionStatus.Posted"/> if they are not already
		/// </summary>
		private void SetTransactionsStatusToPosted(DataTable transactions)
		{
			var saveRequest = new SaveTransactionsSR
			{
				Security = Security,
				SubType = SaveTransactionsSR.SaveTransactionSubType.SaveTranactionFlagsAndStatus,
			};

			foreach (DataRow row in transactions.Rows)
			{
				var transactionStatus = (TransactionStatus)row["LookupTransactionStatusIndex"];
				if (transactionStatus == TransactionStatus.Posted)
				{
					continue; // No need to update it if its already Posted
				}

				var transactionID = row["TransID"].ToString();
				saveRequest.TransFlagsAndStatusCollection.Add(new TransactionFlagsAndStatusDO()
				{
					TransID = transactionID,
					TransStatus = TransactionStatus.Posted,
				});
			}

			if (saveRequest.TransFlagsAndStatusCollection.Any())
			{
				var saveResult = FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(x => x.SaveTransactions(saveRequest));
			}
		}

		private void ExportTransactions(List<TransactionStatus> exportableTransactionStatuses, bool setTransactionStatusesToPosted, List<Columns> columns)
		{
			try
			{
				DateTime now = DateTime.Now;
				string path = AppSettings.GetExportPath() + "\\FMTransactionExport_" + now.ToString("yyyy-MM-dd") + ".csv";

				// Determine the last run row version
				string lastRunRowVersionSting = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(Security, lastRunKey));
				byte[] lastRunRowVersion = null;
				if (!string.IsNullOrWhiteSpace(lastRunRowVersionSting))
				{
					lastRunRowVersion = StringToByteArray(lastRunRowVersionSting);
				}
				else
				{
					lastRunRowVersion = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
				}

				// Construct our request for transactions
				var getTransactionSR = new GetTransactionSR
				{
					Security = this.Security,
					Request = GetTransactionRequest.ALIAS_ROW_VERSION,
					AliasName = AppSettings.GetAliasNames(),
					RowVersion = lastRunRowVersion,
					ConvertToSiteUnits = ConversionType.ToSiteUnits.Equals(ConversionTypeSetting),
					TransStatuses = exportableTransactionStatuses,
					InterfaceName = ExportInterfaceName,
				};

				// Fetch the transactions
				GetTransactionDO getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(x => x.Process(getTransactionSR));

				// Process the results
				DataTable table = getTransactionDO.TransactionDataSet.Tables[0];
				byte[] lastTransVersion = lastRunRowVersion;
				EventLog.WriteEntry("Received transaction count :" + table.Rows.Count);
				var rows = ProcessTable(table, lastRunRowVersion, columns, out lastTransVersion);
				if (WriteExportFile(rows, path, columns))
				{
					if (setTransactionStatusesToPosted)
					{
						SetTransactionsStatusToPosted(table);
					}
					LogExportToDatabase(table);
					FMChannelHelper.MakeCall<IConfigurationSettings>(x => x.Modify(Security, lastRunKey, ByteArrayToString(lastTransVersion)));
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Submits the details of this export to be logged in the database
		/// </summary>
		/// <param name="transactions">A table of the transactions that were exported</param>
		private void LogExportToDatabase(DataTable transactions)
		{
			// Create a new export result
			var exportResult = new ExportResultClass();
			exportResult.TransDateTime = DateTime.Now;
			exportResult.Type = EXPORT_RESULT_TYPE.TRANSACTION;
			exportResult.SuccessCount = transactions.Rows.Count;
			exportResult.FailedCount = 0;
			exportResult.InterfaceName = ExportInterfaceName;
			exportResult.TransVersion = 0; // To be updated later

			// Populate the export result with details for each transaction that was exported
			foreach (DataRow row in transactions.Rows)
			{
				var exportResultDetail = new ExportResultDetailClass();
				exportResultDetail.Fail = false;
				exportResultDetail.RecordId = row["TransID"].ToString();
				exportResultDetail.TransVersion = (long)row["TransVersion"];
				exportResult.ExportResultDetailCollection.Add(exportResultDetail);

				// Keep track of the maximum transaction version
				if (exportResultDetail.TransVersion > exportResult.TransVersion)
				{
					exportResult.TransVersion = exportResultDetail.TransVersion;
				}
			}

			// Save the export result to the database
			FMChannelHelper.MakeCall<IExportResults>(x => x.Add(Security, exportResult));
		}

		public static string ByteArrayToString(byte[] ba)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
				hex.AppendFormat("{0:X2}", b);
			return "0x" + hex.ToString();
		}

		public static byte[] StringToByteArray(String hex)
		{
			hex = hex.Substring(2);
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}

		public void ExportThreadHandler()
		{
			try
			{
				Security = Login("SiteAdmin");
				ProductDic = AppSettings.GetProductMap();

				var columns = AppSettings.GetExportFileColumns();

				var headerNames = GenerateHeader(columns);
				DefaultCloumnValuesDic = AppSettings.GetDefaultValuesMap();
				InitializeCustomItems();
				InitializLoadRacksSet();
				ConversionTypeSetting = AppSettings.GetConversionType();
				DATE_FORMAT = AppSettings.GetDateFormat();
				TIME_FORMAT = AppSettings.GetTimeFormat();

				killAckEvent.Reset();

				// Fetch the application settings
				var scheduleType = AppSettings.GetExportScheduleType();
				var scheduleIntervalSeconds = AppSettings.GetPollingIntervalSeconds();
				var scheduleFixedTime = AppSettings.GetExportScheduleFixedTime();

				var exportableTransactionStatus = AppSettings.GetExportableTransactionStatuses();
				var setTransactionStatusToPostedAfterExport = AppSettings.GetSetTransactionStatusToPostedAfterExport();
				var nextExportTimestamp = DateTime.Now;

				// Periodically wake up and check if we need to perform an export
				while (!killEvent.WaitOne(ExportCheckRateMilliseconds))
				{
					// Check if it's time to perform an export
					if (DateTime.Now < nextExportTimestamp)
					{
						continue;
					}

					// Determine when we should perform our next export
					if (scheduleType == ExportScheduleType.Interval)
					{
						nextExportTimestamp = DateTime.Now.AddSeconds(scheduleIntervalSeconds);
					}
					else if (scheduleType == ExportScheduleType.FixedTime)
					{
						nextExportTimestamp = DateTime.Now.Date.Add(scheduleFixedTime);
						// If we are past the scheduled export time for today, target tomorrow
						if (nextExportTimestamp < DateTime.Now)
						{
							nextExportTimestamp = nextExportTimestamp.AddDays(1);
						}
					}
					else
					{
						throw new ApplicationException($"Unexpected export schedule type encountered: '{scheduleType}'");
					}

					// Perform the actual export
					ExportTransactions(exportableTransactionStatus, setTransactionStatusToPostedAfterExport, columns);
				}
			}
			catch (Exception ex)
			{
				EventLog.WriteEntry("Problem encountered in Transaction Export Thread. Service will be stopped." + Environment.NewLine + "Reason: " + ex.Message + Environment.NewLine + "STACK TRACE: " + Environment.NewLine + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
				killAckEvent.Set();
				Stop();
			}
			killAckEvent.Set();
		}

		public SecurityClass Login(string siteID)
		{
			var security = new SecurityClass { UserGuid = Guids.UserAdminGuid, SiteGuid = Guids.SiteAdminGuid };
			security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
			security.AddRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS);
			security.AddRight(RIGHT.EXECUTE_IMPORT_EXPORT);
			security.AddRight(RIGHT.IMPORT_ENTERPRISE_DATA);
			security.AddRight(RIGHT.MODIFY_TRANSACTION_DATA);
			security.UserID = UserId;
			if (siteID != "SiteAdmin")
			{
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(security, siteID, false));
				security = new SecurityClass { UserGuid = Guids.UserAdminGuid, SiteGuid = site.SiteGuid };
				security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
				security.AddRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS);
				security.UserID = UserId;
			}
			return security;
		}

		private double ConvertUnits(double source, EngineeringUnit sourceUnits, EngineeringUnit resultUnits)
		{
			// Only do the conversion if set in config file.
			// To site units are done in SQL stored procedure
			if (ConversionTypeSetting == ConversionType.ToCongfigFileSettings)
			{
				return EngineeringUnits.Convert(source, sourceUnits, resultUnits, 0);
			}
			return source;
		}
	}
}
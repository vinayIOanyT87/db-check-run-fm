using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using FMBusinessObjects.DataObjects;
using static FMTransactionExportService.FMTransactionExportService;

namespace FMTransactionExportService
{
	public class AppSettings
	{

		public const string SetTransactionStatusToPostedAfterExport = "SetTransactionStatusToPostedAfterExport";
		public const string ExportableTransactionStatuses = "ExportableTransactionStatuses";
		public const string ExportFileColumns = "ExportFileColumns";
		public const string TransLineOperatorId_to_Column = "TransLineOperatorId_to_Column";
		public const string OperatorId_to_Column = "OperatorId_to_Column";
		public const string FST_to_Column = "FST_to_Column";
		public const string TimeEnd_to_Column = "TimeEnd_to_Column";
		public const string LoadID_to_Column = "LoadID_to_Column";

#if DEBUG
		public static bool GetDebuggingEnabled()
		{
			string oEnable = ConfigurationManager.AppSettings["Debugging Enabled"];
			return (oEnable == null) ? false : oEnable == "false" ? false : true;
		}
#endif
		public static string GetExportPath()
		{
			string defaultReportPath = ".\\";
			string exportPath = ConfigurationManager.AppSettings["Export Path"];
			return (exportPath == null) ? defaultReportPath : exportPath.TrimEnd("\\".ToCharArray());
		}

		public static int GetPollingIntervalSeconds()
		{
			int defaultValue = 30;
			string oInterval = ConfigurationManager.AppSettings["PollingIntervalSeconds"];
			return (oInterval == null) ? defaultValue : Convert.ToInt32(oInterval);
		}

		public static Dictionary<string, string> GetProductMap()
		{
			Dictionary<string, string> productMap = new Dictionary<string, string>();
			var map = ConfigurationManager.AppSettings["ProductsMap"];
			var pairs = map.Split(',');
			foreach (var pair in pairs)
			{
				var product = pair.Split('|');
				if (product.Length == 2)
				{
					productMap.Add(product[0], product[1]);
				}
			}
			return productMap;
		}

		public static Dictionary<Columns, string> GetDefaultValuesMap()
		{
			Dictionary<Columns, string> defaultsMap = new Dictionary<Columns, string>();

			// Expected comma separated list of columnName|defaultValue
			var map = ConfigurationManager.AppSettings["DefaultValuesMap"];
			var pairs = map.Split(',');
			foreach (var pair in pairs)
			{
				string[] values = pair.Split('|');
				if (values.Length == 2)
				{
					if (!Enum.TryParse<Columns>(values[0], true, out var column))
					{
						throw new ApplicationException($"Application setting 'DefaultValuesMap' contains an invalid column name");
					}
					defaultsMap.Add(column, values[1]);
				}
			}
			return defaultsMap;
		}

		public static HashSet<string> GetLoadingLocationIDWithArmNumberSet()
		{
			HashSet<string> LoadingLocationIDset = new HashSet<string>();

			// Expected comma separated list of LoadingLocationID
			var map = ConfigurationManager.AppSettings["LoadingLocationIDWithArmNumber"];
			var loadingLocationIDs = map.Split(',');
			foreach (var loadingLocationID in loadingLocationIDs)
			{
				LoadingLocationIDset.Add(loadingLocationID);
			}
			return LoadingLocationIDset;
		}

		public static int GetDefaultConverstionUnits(string type)
		{
			return Convert.ToInt32(ConfigurationManager.AppSettings[type]);
		}
		public static string GetAliasNames()
		{
			return ConfigurationManager.AppSettings["AliasNames"];
		}

		public static string GetDateFormat()
		{
			return ConfigurationManager.AppSettings["DateFormat"];
		}

		public static string GetTimeFormat()
		{
			return ConfigurationManager.AppSettings["TimeFormat"];
		}

		public static Columns? GetOperatorIdToColumn()
		{
			return GetOptionalColumnSetting(OperatorId_to_Column);
		}

		public static Columns? GetTransLineOperatorIdToColumn()
		{
			return GetOptionalColumnSetting(TransLineOperatorId_to_Column);
		}

		public static string GetLoadRackIdentificationString()
		{
			return ConfigurationManager.AppSettings["LoadRackIdentification"];
		}

		public static Columns? GetFstToColumn()
		{
			return GetOptionalColumnSetting(FST_to_Column);
		}

		public static Columns? GetTimeEndtoColumn()
		{
			return GetOptionalColumnSetting(TimeEnd_to_Column);
		}

		public static Columns? GetLoadIDtoColumn()
		{
			return GetOptionalColumnSetting(LoadID_to_Column);
		}

		public static ConversionType GetConversionType()
		{
			ConversionType value;
			if (Enum.TryParse(ConfigurationManager.AppSettings["ConvertUnitsOption"], out value))
			{
				return value;
			}
			return ConversionType.DontConvert;
		}

		/// <summary>
		/// Get the schedule for exporting
		/// </summary>
		public static ExportScheduleType GetExportScheduleType()
		{
			if (Enum.TryParse<ExportScheduleType>(ConfigurationManager.AppSettings["ExportSchedule"], out var value))
			{
				return value;
			}
			else
			{
				throw new ApplicationException($"The required application setting 'ExportSchedule' is not set or is invalid. Must be one of the following: {string.Join(", ", Enum.GetNames(typeof(ExportScheduleType)))}");
			}
		}

		/// <summary>
		/// Gets the fixed time of day to execute an export.
		/// </summary>
		public static TimeSpan GetExportScheduleFixedTime()
		{
			var appSetting = ConfigurationManager.AppSettings["ExportScheduleFixedTimeLocal"];

			// This setting is optional
			if (string.IsNullOrEmpty(appSetting))
			{
				return TimeSpan.Zero;
			}
			else if (TimeSpan.TryParse(appSetting, CultureInfo.InvariantCulture, out var value))
			{
				if (value < TimeSpan.Zero || value.TotalHours >= 24)
				{
					throw new ApplicationException($"The value of application setting 'ExportScheduleFixedTimeLocal' must be between 0 and 24 hours");
				}
				return value;
			}
			else
			{
				throw new ApplicationException($"The value of application setting 'ExportScheduleFixedTimeLocal' is not set to a valid TimeSpan");
			}
		}

		/// <summary>
		/// What statuses transactions must be in to be exported
		/// </summary>
		public static List<TransactionStatus> GetExportableTransactionStatuses()
		{
			var appSetting = ConfigurationManager.AppSettings[ExportableTransactionStatuses];
			if (string.IsNullOrEmpty(appSetting))
			{
				// Default to allowing all transaction statuses
				return new List<TransactionStatus>((TransactionStatus[])Enum.GetValues(typeof(TransactionStatus)));
			}

			var result = new List<TransactionStatus>();
			var statusStrings = appSetting.Replace(" ", string.Empty).Split(',');
			foreach (var statusString in statusStrings)
			{
				if (Enum.TryParse<TransactionStatus>(statusString, true, out var value))
				{
					result.Add(value);
				}
				else
				{
					throw new ApplicationException($"The application setting '{ExportableTransactionStatuses}' has an invalid value. Must be a comma separated list of the following: {string.Join(", ", Enum.GetNames(typeof(TransactionStatus)))}");
				}
			}

			return result;
		}

		/// <summary>
		/// Whether we should change the status of transactions to <see cref="TransactionStatus.Posted"/> after export, defaults to false.
		/// </summary>
		public static bool GetSetTransactionStatusToPostedAfterExport()
		{
			var appSetting = ConfigurationManager.AppSettings[SetTransactionStatusToPostedAfterExport];
			if (string.IsNullOrEmpty(appSetting))
			{
				return false; // Default to false
			}
			else if (bool.TryParse(appSetting, out var value))
			{
				return value;
			}
			else
			{
				throw new ApplicationException($"The optional application setting '{SetTransactionStatusToPostedAfterExport}' is invalid. It must be a valid boolean: true/false");
			}
		}

		/// <summary>
		/// Gets the requested set of columns and order they should be in the export file
		/// </summary>
		public static List<Columns> GetExportFileColumns()
		{
			var appSetting = ConfigurationManager.AppSettings[ExportFileColumns];
			if (string.IsNullOrEmpty(appSetting))
			{
				// Default include all columns
				return new List<Columns>((Columns[])Enum.GetValues(typeof(Columns)));
			}

			var result = new List<Columns>();
			var enumStrings = appSetting.Replace(" ", string.Empty).Split(',');
			foreach (var enumString in enumStrings)
			{
				if (Enum.TryParse<Columns>(enumString, true, out var value))
				{
					result.Add(value);
				}
				else
				{
					throw new ApplicationException($"The application setting '{ExportFileColumns}' has an invalid value. Must be a comma separated list of the following: {string.Join(", ", Enum.GetNames(typeof(TransactionStatus)))}");
				}
			}

			return result;
		}

		private static Columns? GetOptionalColumnSetting(string settingName)
		{
			var setting = ConfigurationManager.AppSettings[settingName];
			if (string.IsNullOrEmpty(setting))
			{
				return null;
			}
			else if (Enum.TryParse(setting, true, out Columns column))
			{
				return column;
			}
			else
			{
				throw new ApplicationException($"Application setting '{settingName}' doesn't contain a valid column name");
			}
		}
	}
}

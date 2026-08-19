using System;
using System.Data;

namespace FMPointTagArchive.Core.InternalClasses
{
	internal class TankChangeHelper
	{
		public const string KeyspaceName = "FMArchive_Data";
		public const string TableName = "archiveData";

		public const string SiteNameFieldName = "SiteName";
		public const string PointNameFieldName = "PointName";
		public const string ProductIDFieldName = "ProductID";
		public const string LevelProductUnitFieldName = "Level Product Units";
		public const string TemperatureUnitFieldName = "Temperature Units";
		public const string GrossVolUnitFieldName = "Gross Volume Units";
		public const string NetVolUnitFieldName = "Net Volume Units";

		// declare the start values
		public const string StartLevelProductFieldName = "Level Product Start";
		public const string StartTemperatureProductFieldName = "Temperature Product Start";
		public const string StartVolumeGrossObservedFieldName = "Volume Gross Observed Start";
		public const string StartVolumeNetStandardFieldName = "Volume Net Standard Start";
		public const string StartNetRemainingFieldName = "Volume Net Standard Remaining Start";
		public const string StartNetAvailableFieldName = "Volume Net Standard Available Start";
		// declare the start status values
		public const string StartLevelProductStatusFieldName = "Level Product Start Status";
		public const string StartTemperatureProductStatusFieldName = "Temperature Product Start Status";
		public const string StartVolumeGrossObservedStatusFieldName = "Volume Gross Observed Start Status";
		public const string StartVolumeNetStandardStatusFieldName = "Volume Net Standard Start Status";
		public const string StartNetRemainingStatusFieldName = "Volume Net Standard Remaining Start Status";
		public const string StartNetAvailableStatusFieldName = "Volume Net Standard Available Start Status";

		// declare the end values
		public const string EndLevelProductFieldName = "Level Product End";
		public const string EndTemperatureProductFieldName = "Temperature Product End";
		public const string EndVolumeGrossObservedFieldName = "Volume Gross Observed End";
		public const string EndVolumeNetStandardFieldName = "Volume Net Standard End";
		public const string EndNetRemainingFieldName = "Volume Net Standard Remaining End";
		public const string EndNetAvailableFieldName = "Volume Net Standard Available End";
		// declare the end status values
		public const string EndLevelProductStatusFieldName = "Level Product End Status";
		public const string EndTemperatureProductStatusFieldName = "Temperature Product End Status";
		public const string EndVolumeGrossObservedStatusFieldName = "Volume Gross Observed End Status";
		public const string EndVolumeNetStandardStatusFieldName = "Volume Net Standard End Status";
		public const string EndNetRemainingStatusFieldName = "Volume Net Standard Remaining End Status";
		public const string EndNetAvailableStatusFieldName = "Volume Net Standard Available End Status";

		public const string ChangeLevelProductFieldName = "Level Product Change";

		public const string StartLevelProductTimestampFieldName = "Level Product Timestamp Start";

		public const string EndLevelProductTimestampFieldName = "Level Product Timestamp End";

		public const string RefDataTableFieldTagGuid = "PointTagGuid";
		public const string RefDataTableFieldPointGuid = "PointGuid";
		public const string RefDataTableFieldEngrUnitsIndex = "EngineeringUnitsIndex";
		public const string RefDataTableFieldTagID = "PointTagID";
		public const string RefDataTableFieldsPointID = "PointID";
		public const string RefDataTableFieldsPointEnabled = "PointEnabled";

		public static DataTable CreateEmptyPointTagTable()
		{
			var table = new DataTable("Tag");

			table.Columns.Add(new DataColumn(RefDataTableFieldTagGuid, typeof(Guid)));
			table.Columns.Add(new DataColumn(RefDataTableFieldPointGuid, typeof(Guid)));
			table.Columns.Add(new DataColumn(RefDataTableFieldEngrUnitsIndex, typeof(int)));
			table.Columns.Add(new DataColumn(RefDataTableFieldTagID, typeof(string)));
			table.Columns.Add(new DataColumn(RefDataTableFieldsPointID, typeof(string)));
			table.Columns.Add(new DataColumn(RefDataTableFieldsPointEnabled, typeof(int)));

			return table;
		}

		public static DataSet CreateEmptyDataSet()
		{
			var dataSet = new DataSet(KeyspaceName);
			var table = new DataTable(TableName);

			dataSet.Tables.Add(table);

			table.Columns.Add(new DataColumn(SiteNameFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(PointNameFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(StartLevelProductFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(StartLevelProductStatusFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(EndLevelProductFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(EndLevelProductStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(ChangeLevelProductFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(StartTemperatureProductFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(StartTemperatureProductStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(EndTemperatureProductFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(EndTemperatureProductStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(StartVolumeGrossObservedFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(StartVolumeGrossObservedStatusFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(EndVolumeGrossObservedFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(EndVolumeGrossObservedStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(StartVolumeNetStandardFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(StartVolumeNetStandardStatusFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(EndVolumeNetStandardFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(EndVolumeNetStandardStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(ProductIDFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(StartNetRemainingFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(StartNetRemainingStatusFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(EndNetRemainingFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(EndNetRemainingStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(StartNetAvailableFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(StartNetAvailableStatusFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(EndNetAvailableFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(EndNetAvailableStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(LevelProductUnitFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(TemperatureUnitFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(GrossVolUnitFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(NetVolUnitFieldName, typeof(string)));


			table.Columns.Add(new DataColumn(StartLevelProductTimestampFieldName, typeof(DateTimeOffset)));
			table.Columns.Add(new DataColumn(EndLevelProductTimestampFieldName, typeof(DateTimeOffset)));

			return dataSet;
		}

		public static object GetDBValue(object o)
		{
			if (null == o)
			{
				return DBNull.Value;
			}
			else return o;
		}

		public static bool IsInterestingTag(string tagID)
		{
			if (tagID == "Level Product"
				|| tagID == "Temperature Product"
				|| tagID == "Volume Gross Observed"
				|| tagID == "Volume Net Standard"
				|| tagID == "ProductID"
				|| tagID == "Volume Net Standard Remaining"
				|| tagID == "Volume Net Standard Available")
			{
				return true;
			}

			return false;
		}
	}
}

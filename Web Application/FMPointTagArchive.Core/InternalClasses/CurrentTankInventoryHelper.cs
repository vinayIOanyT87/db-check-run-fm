using System;
using System.Data;

namespace FMPointTagArchive.Core.InternalClasses
{
	internal class CurrentTankInventoryHelper
	{
		public const string KeyspaceName = "FMArchive_Data";
		public const string TableName = "archiveData";

		public const string SiteNameFieldName = "SiteName";
		public const string PointNameFieldName = "PointName";

		public const string LevelProductFieldName = "Level Product";
		public const string LevelProductStatusFieldName = "Level Product Status";

		public const string TemperatureProductFieldName = "Temperature Product";
		public const string TemperatureProductStatusFieldName = "Temperature Product Status";

		public const string VolumeGrossObservedFieldName = "Volume Gross Observed";
		public const string VolumeGrossObservedStatusFieldName = "Volume Gross Observed Status";

		public const string VolumeNetStandardFieldName = "Volume Net Standard";
		public const string VolumeNetStandardStatusFieldName = "Volume Net Standard Status";

		public const string ProductIDFieldName = "ProductID";

		public const string NetRemainingFieldName = "Volume Net Standard Remaining";
		public const string NetRemainingStatusFieldName = "Volume Net Standard Remaining Status";

		public const string NetAvailableFieldName = "Volume Net Standard Available";
		public const string NetAvailableStatusFieldName = "Volume Net Standard Available Status";

		public const string LevelProductUnitFieldName = "Level Product Units";
		public const string TemperatureUnitFieldName = "Temperature Units";
		public const string GrossVolUnitFieldName = "Gross Volume Units";
		public const string NetVolUnitFieldName = "Net Volume Units";

        public const string VCFFieldName = "VCF";
        public const string VCFStatusFieldName = "VCF Status";

        public const string DensityProductStandardFieldName = "Density Product Standard";
        public const string DensityProductStandardStatusFieldName = "Density Product Standard Status";
        public const string DensityProductStandardUnitFieldName = "Density Product Standard Units";

        public const string VolumeGrossStandardFieldName = "Volume Gross Standard";
        public const string VolumeGrossStandardStatusFieldName = "Volume Gross Standard Status";
        public const string VolumeGrossStandardUnitFieldName = "Volume Gross Standard Units";

        public const string TemperatureDensityFieldName = "Temperature Density";
        public const string TemperatureDensityStatusFieldName = "Temperature Density Status";
        public const string TemperatureDensityUnitFieldName = "Temperature Density Units";

      public const string LevelWaterFieldName = "Level Water";
      public const string LevelWaterStatusFieldName = "Level Water Status";
      public const string LevelWaterUnitFieldName = "Level Water Units";

      public const string VolumeWaterFieldName = "Volume Water";
      public const string VolumeWaterStatusFieldName = "Volume Water Status";
      public const string VolumeWaterUnitFieldName = "Volume Water Units";

      public const string LastUpdateFieldName = "Last Update Date Time";

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

			table.Columns.Add(new DataColumn(LevelProductFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(LevelProductStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(TemperatureProductFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(TemperatureProductStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(VolumeGrossObservedFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(VolumeGrossObservedStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(VolumeNetStandardFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(VolumeNetStandardStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(ProductIDFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(NetRemainingFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(NetRemainingStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(NetAvailableFieldName, typeof(double)));
			table.Columns.Add(new DataColumn(NetAvailableStatusFieldName, typeof(string)));

			table.Columns.Add(new DataColumn(LevelProductUnitFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(TemperatureUnitFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(GrossVolUnitFieldName, typeof(string)));
			table.Columns.Add(new DataColumn(NetVolUnitFieldName, typeof(string)));

            table.Columns.Add(new DataColumn(VCFFieldName, typeof(double)));
            table.Columns.Add(new DataColumn(VCFStatusFieldName, typeof(string)));
            table.Columns.Add(new DataColumn(DensityProductStandardFieldName, typeof(double)));
            table.Columns.Add(new DataColumn(DensityProductStandardStatusFieldName, typeof(string)));
            table.Columns.Add(new DataColumn(DensityProductStandardUnitFieldName, typeof(string)));
            table.Columns.Add(new DataColumn(VolumeGrossStandardFieldName, typeof(double)));
            table.Columns.Add(new DataColumn(VolumeGrossStandardStatusFieldName, typeof(string)));
            table.Columns.Add(new DataColumn(VolumeGrossStandardUnitFieldName, typeof(string)));
            table.Columns.Add(new DataColumn(TemperatureDensityFieldName, typeof(double)));
            table.Columns.Add(new DataColumn(TemperatureDensityStatusFieldName, typeof(string)));
            table.Columns.Add(new DataColumn(TemperatureDensityUnitFieldName, typeof(string)));

         table.Columns.Add(new DataColumn(LevelWaterFieldName, typeof(string)));
         table.Columns.Add(new DataColumn(LevelWaterStatusFieldName, typeof(string)));
         table.Columns.Add(new DataColumn(LevelWaterUnitFieldName, typeof(string)));

         table.Columns.Add(new DataColumn(VolumeWaterFieldName, typeof(double)));
         table.Columns.Add(new DataColumn(VolumeWaterStatusFieldName, typeof(string)));
         table.Columns.Add(new DataColumn(VolumeWaterUnitFieldName, typeof(string)));


         table.Columns.Add(new DataColumn(LastUpdateFieldName, typeof(DateTime)));
            
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
					|| tagID == "Volume Net Standard Available"
					|| tagID == "Volume Correction Factor"
					|| tagID == "Volume Gross Standard"
               || tagID == "Density Product Standard"
					|| tagID == "Temperature Density"
               || tagID == "Level Water"
               || tagID == "Volume Water"
               )

         {
					return true;
				}

				return false;
		}

	}
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using System.Runtime.Serialization;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(LeakReportClass))]
	public class LeakReportCollectionClass : List<LeakReportClass> { }

	[DataContract]
	[Serializable]
	public class LeakReportClass : BaseDataObject
	{
		[DataMember]
		public Guid LeakReportId { get; set; }
		[DataMember]
		private string pointId;
		[DataMember]
        private string pointDescription;
        [DataMember]
		private string testType;
		[DataMember]
		private string testMethod;
		[DataMember] 
		private string testResult;
		[DataMember]
		public double? LeakRate { get; set; }
		[DataMember]
		public DateTimeOffset? StartTime { get; set; }
		[DataMember]
		public DateTimeOffset? EndTime { get; set; }
		[DataMember]
		public double? LevelStart { get; set; }
		[DataMember]
		public double? LevelEnd { get; set; }
        [DataMember]
        public double? PressureStart { get; set; }
        [DataMember]
        public double? PressureEnd { get; set; }
        [DataMember]
		public double? MinTemp { get; set; }
		[DataMember]
		public double? MaxTemp { get; set; }
		[DataMember]
		public double? MinVolume { get; set; }
		[DataMember]
		public double? MaxVolume { get; set; }
		[DataMember]
		public double? CertRate { get; set; }
		[DataMember]
		public double? WaterLevelStart { get; set; }
		[DataMember]
		public double? WaterLevelStop { get; set; }
		[DataMember]
		public double? LeakThreshold { get; set; }
		[DataMember]
		public DateTimeOffset TimeStamp { get; set; }
		[DataMember]
		public DateTime? DateInstalled { get; set; }
		[DataMember]
		private string tankGauge;
		[DataMember]
		private string leakDetectionSystem;
		[DataMember]
		public double? TankLengthOrHeight { get;set; }
		[DataMember]
		public double? TankRadius { get; set; }
		[DataMember]
		public double? TankVolume { get; set; }
		[DataMember]
		private string liningMaterial;
		[DataMember]
		private string constructionMaterial;
		[DataMember]
		public bool? CathodicProtection { get; set; }
		[DataMember]
		public bool? OverfillProtection { get; set; }
		[DataMember]
		public bool? SpillProtection { get; set; }
		[DataMember]
		public EngineeringUnit WaterLevelUnits { get; set; }
		[DataMember]
		public EngineeringUnit VolumeUnits { get; set; }
		[DataMember]
		public EngineeringUnit TemperatureUnits { get; set; }
		[DataMember]
		public EngineeringUnit ProducLevelUnits { get; set; }
        [DataMember]
        public EngineeringUnit PressureUnits { get; set; }
        [DataMember]
		public EngineeringUnit LeakRateUnits { get; set; }
		[DataMember]
		public int VolumePrecision { get; set; }
		[DataMember]
		public int TemperaturePrecision { get; set; }
		[DataMember]
		public int ProductLevelPrecision { get; set; }
		[DataMember]
		public int WaterLevelPrecision { get; set; }
		[DataMember]
        public int PressurePrecision { get; set; }
        [DataMember]
        public int LeakRatePrecision { get; set; }
		[DataMember]
		public EngineeringUnit BasePointLevelUnits { get; set; }
		[DataMember]
		public EngineeringUnit BasePointVolumeUnits { get; set; }

		public string PointId
		{
			get
			{
				return pointId;
			}
			set
			{
				SetString("PointId", 30, value, ref pointId);
			}
		}

        public string PointDescription
        {
            get
            {
                return pointDescription;
			}
			set
			{
                SetString("PointDescription", 50, value, ref pointDescription);
			}
		}

		public string TestType
		{
			get
			{
				return testType;
			}
			set
			{
				SetString("TestType", 30, value, ref testType);
			}
		}

		public string TestMethod
		{
			get
			{
				return testMethod;
			}
			set
			{
				SetString("TestMethod", 30, value, ref testMethod);
			}
		}

		public string TestResult
		{
			get
			{
				return testResult;
			}
			set
			{
				SetString("TestResult", 30, value, ref testResult);
			}
		}

		public string TankGauge
		{
			get
			{
				return tankGauge;
			}
			set
			{
				SetString("TankGauge", 50, value, ref tankGauge);
			}
		}

		public string LeakDetectionSystem
		{
			get
			{
				return leakDetectionSystem;
			}
			set
			{
				SetString("LeakDetectionSystem", 50, value, ref leakDetectionSystem);
			}
		}

		public string LiningMaterial
		{
			get
			{
				return liningMaterial;
			}
			set
			{
				SetString("LiningMaterial", 50, value, ref liningMaterial);
			}
		}

		public string ConstructionMaterial
		{
			get
			{
				return constructionMaterial;
			}
			set
			{
				SetString("Constructionmaterial", 30, value, ref constructionMaterial);
			}
		}

		public LeakReportClass()
		{
			Reset();
		}

		public override void Reset()
		{
			this.ProducLevelUnits = EngineeringUnit.FmuNone;
			this.VolumeUnits = EngineeringUnit.FmuNone;
			this.WaterLevelUnits = EngineeringUnit.FmuNone;
			this.TemperatureUnits = EngineeringUnit.FmuNone;
			this.LeakRateUnits = EngineeringUnit.FmuNone;
			base.Reset();
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
			{
				throw new ArgumentNullException(nameof(Set));
			}

			Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
				return;

			DataRow Row = Table.Rows[0];

			this.LeakReportId = DataObject.getValue(Row["LeakReportId"], Guid.Empty);
			this.SiteID = DataObject.getValue(Row["SiteId"], string.Empty);
			this.PointId = DataObject.getValue(Row["PointId"], string.Empty);
            this.PointDescription = DataObject.getValue(Row["PointDescription"], string.Empty);
			this.TestType = DataObject.getValue(Row["TestType"], string.Empty);
			this.TestMethod = DataObject.getValue(Row["TestMethod"], string.Empty);
			this.TestResult = DataObject.getValue(Row["TestResult"], string.Empty);
			this.LeakRate = Row.IsNull("LeakRate") ? null : (double?)DataObject.getValue(Row["LeakRate"], (double)0.0);
			this.StartTime = Row.IsNull("StartTime") ? null : (DateTimeOffset?)DataObject.getValue(Row["StartTime"], DateTimeOffset.MinValue);
			this.EndTime = Row.IsNull("EndTime") ? null : (DateTimeOffset?)DataObject.getValue(Row["EndTime"], DateTimeOffset.MinValue);
			this.LevelStart = Row.IsNull("LevelStart") ? null : (double?)DataObject.getValue(Row["LevelStart"], (double)0.0);
			this.LevelEnd = Row.IsNull("LevelEnd") ? null : (double?)DataObject.getValue(Row["LevelEnd"], (double)0.0);
            this.PressureStart = Row.IsNull("PressureStart") ? null : (double?)DataObject.getValue(Row["PressureStart"], (double)0.0);
            this.PressureEnd = Row.IsNull("PressureEnd") ? null : (double?)DataObject.getValue(Row["PressureEnd"], (double)0.0);
            this.MinTemp = Row.IsNull("MinTemp") ? null : (double?)DataObject.getValue(Row["MinTemp"], (double)0.0);
			this.MaxTemp = Row.IsNull("MaxTemp") ? null : (double?)DataObject.getValue(Row["MaxTemp"], (double)0.0);
			this.MinVolume = Row.IsNull("MinVolume") ? null : (double?)DataObject.getValue(Row["MinVolume"], (double)0.0);
			this.MaxVolume = Row.IsNull("MaxVolume") ? null : (double?)DataObject.getValue(Row["MaxVolume"], (double)0.0);
			this.CertRate = Row.IsNull("CertRate") ? null : (double?)DataObject.getValue(Row["CertRate"], (double)0.0);
			this.WaterLevelStart = Row.IsNull("WaterLevelStart") ? null : (double?)DataObject.getValue(Row["WaterLevelStart"], (double)0.0);
			this.WaterLevelStop = Row.IsNull("WaterLevelEnd") ? null : (double?)DataObject.getValue(Row["WaterLevelEnd"], (double)0.0);
			this.LeakThreshold = Row.IsNull("LeakThreshold") ? null : (double?)DataObject.getValue(Row["LeakThreshold"], (double)0.0);
			this.TimeStamp = DataObject.getValue(Row["TimeStamp"], DateTimeOffset.MinValue);
			this.DateInstalled = Row.IsNull("DateInstalled") ? null : (DateTime?)DataObject.getValue(Row["DateInstalled"], DateTime.MinValue.Date);
			this.DateInstalled = (this.DateInstalled.HasValue && this.DateInstalled != DateTime.MinValue.Date) ? this.DateInstalled : null;
			this.TankGauge = DataObject.getValue(Row["TankGauge"], string.Empty);
			this.LeakDetectionSystem = DataObject.getValue(Row["LeakDetectionSystem"], string.Empty);
			this.TankLengthOrHeight = Row.IsNull("TankLengthOrHeight") ? null : (double?)DataObject.getValue(Row["TankLengthOrHeight"], (double)0.0);
			this.TankRadius = Row.IsNull("TankRadius") ? null : (double?)DataObject.getValue(Row["TankRadius"], (double)0.0);
			this.TankVolume = Row.IsNull("TankVolume") ? null : (double?)DataObject.getValue(Row["TankVolume"], (double)0.0);
			this.LiningMaterial = DataObject.getValue(Row["LiningMaterial"], string.Empty);
			this.ConstructionMaterial = DataObject.getValue(Row["ConstructionMaterial"], string.Empty);
			this.CathodicProtection = Row.IsNull("CathodicProtection") ? null : (bool?)DataObject.getValue(Row["CathodicProtection"], false);
			this.OverfillProtection = Row.IsNull("OverfillProtection") ? null : (bool?)DataObject.getValue(Row["OverfillProtection"], false);
			this.SpillProtection = Row.IsNull("SpillProtection") ? null : (bool?)DataObject.getValue(Row["SpillProtection"], false);
			this.ProducLevelUnits = Row.IsNull("ProductLevelEngineeringUnitIndex") ? EngineeringUnit.FmuNone : (EngineeringUnit)DataObject.getValue(Row["ProductLevelEngineeringUnitIndex"], EngineeringUnit.FmuNone);
			this.VolumeUnits = Row.IsNull("VolumeEngineeringUnitIndex") ? EngineeringUnit.FmuNone : (EngineeringUnit)DataObject.getValue(Row["VolumeEngineeringUnitIndex"], EngineeringUnit.FmuNone);
			this.WaterLevelUnits = Row.IsNull("WaterLevelEngineeringUnitIndex") ? EngineeringUnit.FmuNone : (EngineeringUnit)DataObject.getValue(Row["WaterLevelEngineeringUnitIndex"], EngineeringUnit.FmuNone);
			this.TemperatureUnits = Row.IsNull("TemperatureEngineeringUnitIndex") ? EngineeringUnit.FmuNone : (EngineeringUnit)DataObject.getValue(Row["TemperatureEngineeringUnitIndex"], EngineeringUnit.FmuNone);
			this.LeakRateUnits = Row.IsNull("LeakRateEngineeringUnitIndex") ? EngineeringUnit.FmuNone : (EngineeringUnit)DataObject.getValue(Row["LeakRateEngineeringUnitIndex"], EngineeringUnit.FmuNone);
			this.BasePointLevelUnits = Row.IsNull("BasePointLevelUnitsIndex") ? EngineeringUnit.FmuNone : (EngineeringUnit)DataObject.getValue(Row["BasePointLevelUnitsIndex"], EngineeringUnit.FmuNone);
			this.BasePointVolumeUnits = Row.IsNull("BasePointVolumeUnitsIndex") ? EngineeringUnit.FmuNone : (EngineeringUnit)DataObject.getValue(Row["BasePointVolumeUnitsIndex"], EngineeringUnit.FmuNone);
			this.PressureUnits = Row.IsNull("PressureEngineeringUnitIndex") ? EngineeringUnit.FmuNone : (EngineeringUnit)DataObject.getValue(Row["PressureEngineeringUnitIndex"], EngineeringUnit.FmuNone);
            this.VolumePrecision = Row.IsNull("VolumePrecision") ? 0 : (int)DataObject.getValue(Row["VolumePrecision"], 0);
			this.TemperaturePrecision = Row.IsNull("TemperaturePrecision") ? 0 : (int)DataObject.getValue(Row["TemperaturePrecision"], 0);
			this.LeakRatePrecision = Row.IsNull("LeakRatePrecision") ? 0 : (int)DataObject.getValue(Row["LeakRatePrecision"], 0);
			this.ProductLevelPrecision = Row.IsNull("ProductLevelPrecision") ? 0 : (int)DataObject.getValue(Row["ProductLevelPrecision"], 0);
			this.WaterLevelPrecision = Row.IsNull("WaterLevelPrecision") ? 0 : (int)DataObject.getValue(Row["WaterLevelPrecision"], 0);
			this.PressurePrecision = Row.IsNull("PressurePrecision") ? 0 : (int)DataObject.getValue(Row["PressurePrecision"], 0);
        }

		public void InsertSQL(SqlCommand cmd)
		{

			cmd.CommandText = "INSERT INTO tblLeakReport " +
				"(LeakReportId," +
				"SiteId," +
				"PointId," +
				"TestType," +
				"TestMethod," +
				"TestResult," +
				"LeakRate," +
				"StartTime," +
				"EndTime," +
				"LevelStart," +
				"LevelEnd," +
                "PressureStart," +
                "PressureEnd," +
                "MinTemp," +
				"MaxTemp," +
				"MinVolume," +
				"MaxVolume," +
				"CertRate," +
				"WaterLevelStart," +
				"WaterLevelEnd," +
				"LeakThreshold," +
				"TimeStamp," +
				"DateInstalled," +
				"TankGauge," +
				"LeakDetectionSystem," +
				"TankLengthOrHeight," +
				"TankRadius," +
				"TankVolume," +
				"LiningMaterial," +
				"ConstructionMaterial," +
				"CathodicProtection," +
				"OverfillProtection," +
				"SpillProtection," +
				"ProductLevelEngineeringUnitIndex," +
			    "VolumeEngineeringUnitIndex," +
				"WaterLevelEngineeringUnitIndex," +
				"TemperatureEngineeringUnitIndex," +
                "PressureEngineeringUnitIndex," +
                "LeakRateEngineeringUnitIndex," +
				"BasePointLevelUnitsIndex," +
				"BasePointVolumeUnitsIndex," +
                "VolumePrecision," +
				"TemperaturePrecision," +
				"LeakRatePrecision," +
				"ProductLevelPrecision," +
				"WaterLevelPrecision," +
                "PressurePrecision," +
                "PointDescription" +

			") VALUES (" +
					"@LeakReportId," +
					"@SiteId," +
					"@PointId," +
					"@TestType," +
					"@TestMethod," +
					"@TestResult," +
					"@LeakRate," +
					"@StartTime," +
					"@EndTime," +
					"@LevelStart," +
					"@LevelEnd," +
                    "@PressureStart," +
                    "@PressureEnd," +
                    "@MinTemp," +
					"@MaxTemp," +
					"@MinVolume," +
					"@MaxVolume," +
					"@CertRate," +
					"@WaterLevelStart," +
					"@WaterLevelEnd," +
					"@LeakThreshold," +
					"@TimeStamp," +
					"@DateInstalled," +
					"@TankGauge," +
					"@LeakDetectionSystem," +
					"@TankLengthOrHeight," +
					"@TankRadius," +
					"@TankVolume," +
					"@LiningMaterial," +
					"@ConstructionMaterial," +
					"@CathodicProtection," +
					"@OverfillProtection," +
					"@SpillProtection," +
					"@ProductLevelEngineeringUnitIndex," +
					"@VolumeEngineeringUnitIndex," +
					"@WaterLevelEngineeringUnitIndex," +
					"@TemperatureEngineeringUnitIndex," +
                    "@PressureEngineeringUnitIndex," +
                    "@LeakRateEngineeringUnitIndex," +
					"@BasePointLevelUnitsIndex," +
					"@BasePointVolumeUnitsIndex," +
					"@VolumePrecision," +
					"@TemperaturePrecision," +
					"@LeakRatePrecision," +
					"@ProductLevelPrecision," +
					"@WaterLevelPrecision," +
                    "@PressurePrecision," +
                    "@PointDescription " +
					
				")";

			cmd.Parameters.AddWithValue("@LeakReportId", this.LeakReportId);
			cmd.Parameters.AddWithValue("@SiteId", this.SiteID);
			cmd.Parameters.AddWithValue("@PointId", this.PointId);
			cmd.Parameters.AddWithValue("@TestType", this.TestType);
			cmd.Parameters.AddWithValue("@TestMethod", this.TestMethod);
			cmd.Parameters.AddWithValue("@TestResult", this.TestResult);
			if (this.LeakRate.HasValue)
			{
				cmd.Parameters.AddWithValue("@LeakRate", this.LeakRate.Value);
			}
			else 
			{
				cmd.Parameters.AddWithValue("@LeakRate", DBNull.Value);
			}
			if (this.StartTime.HasValue)
			{
				cmd.Parameters.AddWithValue("@StartTime", this.StartTime.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@StartTime", DBNull.Value);
			}
			if (this.EndTime.HasValue)
			{
				cmd.Parameters.AddWithValue("@EndTime", this.EndTime.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@EndTime", DBNull.Value);
			}
			if (this.LevelStart.HasValue)
			{
				cmd.Parameters.AddWithValue("@LevelStart", this.LevelStart.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@LevelStart", DBNull.Value);
			}
            if (this.LevelEnd.HasValue)
			{
				cmd.Parameters.AddWithValue("@LevelEnd", this.LevelEnd.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@LevelEnd", DBNull.Value);
			}
            if (this.PressureStart.HasValue)
            {
                cmd.Parameters.AddWithValue("@PressureStart", this.PressureStart.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@PressureStart", DBNull.Value);
            }
            if (this.PressureEnd.HasValue)
            {
                cmd.Parameters.AddWithValue("@PressureEnd", this.PressureEnd.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@PressureEnd", DBNull.Value);
            }
            if (this.MinTemp.HasValue)
			{
				cmd.Parameters.AddWithValue("@MinTemp", this.MinTemp.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@MinTemp", DBNull.Value);
			}
			if (this.MaxTemp.HasValue)
			{
				cmd.Parameters.AddWithValue("@MaxTemp", this.MaxTemp.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@MaxTemp", DBNull.Value);
			}
			if (this.MinVolume.HasValue)
			{
				cmd.Parameters.AddWithValue("@MinVolume", this.MinVolume.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@MinVolume", DBNull.Value);
			}
			if (this.MaxVolume.HasValue)
			{
				cmd.Parameters.AddWithValue("@MaxVolume", this.MaxVolume.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@MaxVolume", DBNull.Value);
			}
			if (this.CertRate.HasValue)
			{
				cmd.Parameters.AddWithValue("@CertRate", this.CertRate.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@CertRate", DBNull.Value);
			}
			if (this.WaterLevelStart.HasValue)
			{
				cmd.Parameters.AddWithValue("@WaterLevelStart", this.WaterLevelStart.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@WaterLevelStart", DBNull.Value);
			}
			if (this.WaterLevelStop.HasValue)
			{
				cmd.Parameters.AddWithValue("@WaterLevelEnd", this.WaterLevelStop.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@WaterLevelEnd", DBNull.Value);
			}
			if (this.LeakThreshold.HasValue)
			{
				cmd.Parameters.AddWithValue("@LeakThreshold", this.LeakThreshold.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@LeakThreshold", DBNull.Value);
			}
			cmd.Parameters.AddWithValue("@TimeStamp", this.TimeStamp);
			if (this.DateInstalled.HasValue && this.DateInstalled.Value >= SqlDateTime.MinValue.Value && this.DateInstalled.Value <= SqlDateTime.MaxValue.Value)
			{
				cmd.Parameters.AddWithValue("@DateInstalled", this.DateInstalled.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@DateInstalled", DBNull.Value);
			}
			cmd.Parameters.AddWithValue("@TankGauge", this.TankGauge);
			cmd.Parameters.AddWithValue("@LeakDetectionSystem", this.LeakDetectionSystem);
			if (this.TankLengthOrHeight.HasValue)
			{
				cmd.Parameters.AddWithValue("@TankLengthOrHeight", this.TankLengthOrHeight.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@TankLengthOrHeight", DBNull.Value);
			}
			if (this.TankRadius.HasValue)
			{
				cmd.Parameters.AddWithValue("@TankRadius", this.TankRadius.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@TankRadius", DBNull.Value);
			}
			if (this.TankVolume.HasValue)
			{
				cmd.Parameters.AddWithValue("@TankVolume", this.TankVolume.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@TankVolume", DBNull.Value);
			}
			cmd.Parameters.AddWithValue("@LiningMaterial", this.LiningMaterial);
			cmd.Parameters.AddWithValue("@ConstructionMaterial", this.ConstructionMaterial);
			if (this.CathodicProtection.HasValue)
			{
				cmd.Parameters.AddWithValue("@CathodicProtection", this.CathodicProtection.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@CathodicProtection", DBNull.Value);
			}
			if (this.OverfillProtection.HasValue)
			{
				cmd.Parameters.AddWithValue("@OverfillProtection", this.OverfillProtection.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@OverfillProtection", DBNull.Value);
			}
			if (this.SpillProtection.HasValue)
			{
				cmd.Parameters.AddWithValue("@SpillProtection", this.SpillProtection.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@SpillProtection", DBNull.Value);
			}
			cmd.Parameters.AddWithValue("@ProductLevelEngineeringUnitIndex", this.ProducLevelUnits);
			cmd.Parameters.AddWithValue("@VolumeEngineeringUnitIndex", this.VolumeUnits);
			cmd.Parameters.AddWithValue("@WaterLevelEngineeringUnitIndex", this.WaterLevelUnits);
			cmd.Parameters.AddWithValue("@TemperatureEngineeringUnitIndex", this.TemperatureUnits);
            cmd.Parameters.AddWithValue("@PressureEngineeringUnitIndex", this.PressureUnits);
			cmd.Parameters.AddWithValue("@LeakRateEngineeringUnitIndex", this.LeakRateUnits);
			cmd.Parameters.AddWithValue("@BasePointLevelUnitsIndex", this.BasePointLevelUnits);
			cmd.Parameters.AddWithValue("@BasePointVolumeUnitsIndex", this.BasePointVolumeUnits);
			cmd.Parameters.AddWithValue("@VolumePrecision", this.VolumePrecision);
			cmd.Parameters.AddWithValue("@TemperaturePrecision", this.TemperaturePrecision);
			cmd.Parameters.AddWithValue("@LeakRatePrecision", this.LeakRatePrecision);
			cmd.Parameters.AddWithValue("@ProductLevelPrecision", this.ProductLevelPrecision);
			cmd.Parameters.AddWithValue("@WaterLevelPrecision", this.WaterLevelPrecision);
            cmd.Parameters.AddWithValue("@PressurePrecision", this.PressurePrecision);
            cmd.Parameters.AddWithValue("@PointDescription", this.PointDescription);
		}

		public SqlCommand PurgeSQL
		{
			get
			{
				SqlCommand cmd = new SqlCommand
				{
					CommandText = "DELETE FROM tblLeakReportGraph WHERE LeakReportId = @LeakReportId; " +
										"DELETE FROM tblLeakReport WHERE LeakReportId = @LeakReportId;"
				};

				cmd.Parameters.AddWithValue("@LeakReportId", this.LeakReportId);
				return cmd;
			}
		}

		public SqlCommand SelectSQL()
		{
			SqlCommand cmd = new SqlCommand
			{
				CommandText = "SELECT LeakReportId, SiteId, PointId, TestType, TestMethod, TestResult, LeakRate, StartTime, EndTime, " +
									"LevelStart, LevelEnd, MinTemp, MaxTemp, MinVolume, MaxVolume, CertRate, WaterLevelStart, WaterLevelEnd, LeakThreshold, TimeStamp, " +
									"DateInstalled, TankGauge, LeakDetectionSystem, TankLengthOrHeight, TankRadius, TankVolume, LiningMaterial, ConstructionMaterial, CathodicProtection, OverfillProtection, SpillProtection " +
                                    "ProductLevelEngineeringUnitIndex, VolumeEngineeringUnitIndex, WaterLevelEngineeringUnitIndex, TemperatureEngineeringUnitIndex, LeakRateEngineeringUnitIndex, BasePointLevelUnitsIndex, BasePointVolumeUnitsIndex, VolumePrecision, TemperaturePrecision, LeakRatePrecision, ProductLevelPrecision, WaterLevelPrecision, " +
                                    "PointDescription, PressureEngineeringUnitIndex, PressurePrecision, PressureStart, PressureEnd " +
				  " FROM tblLeakReport " +
				  " WHERE LeakReportId = @LeakReportId"
			};
			cmd.Parameters.AddWithValue("@LeakReportId", this.LeakReportId);
			return cmd;
		}
	}
}
namespace FMBusinessObjects.DataObjects
{
	using Attributes;

	using System;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public sealed class PointAccessGroupToExposedSettingMap : BaseDataObject
	{
		[FMPersistedField]
		public Guid PointAccessGroupToExposedSettingGuid
		{
			get
			{
				return base.IdentityGuid;
			}

			set
			{
				base.IdentityGuid = value;
			}
		}

		[DataMember]
		[FMPersistedField]
		public Guid PointAccessGroupGuid { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public Guid PointTemplateGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public Guid ExposedSettingGuid { get; set; }

		[DataMember]
		[FMPersistedField]
		public string PropertyID { get; set; }

		[DataMember]
		[FMPersistedField(ReadOnly = true)]
		public PointValueType ValueType { get; set; }


		[DataMember]
		[FMPersistedField]
		public bool View { get; set; }

		[DataMember]
		[FMPersistedField]
		public bool Modify { get; set; }


		[DataMember]
		[FMPersistedField]
		public bool ModifyDisabled { get; set; }

		public static void PurgeByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
			cmd.CommandText	= " DELETE FROM map.tblPointAccessGroupToExposedPointSetting WHERE PointAccessGroupGuid = @PointAccessGroupGuid"
									+ " DELETE FROM map.tblPointAccessGroupToExposedPropertySetting WHERE PointAccessGroupGuid = @PointAccessGroupGuid";

			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}


		public static void PurgeBySiteGuidAndPointTemplateGuidSQL(SqlCommand cmd, Guid siteGuid, Guid pointTemplateGuid)
		{
			cmd.CommandText = " DELETE pagteps FROM map.tblPointAccessGroupToExposedPointSetting pagteps"
									+ " INNER JOIN dbo.tblPointAccessGroup pag ON SiteGuid = @SiteGuid AND pag.PointAccessGroupGuid = pagteps.PointAccessGroupGuid"
									+ " WHERE PointSettingGuid = @PointTemplateGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PointTemplateGuid", pointTemplateGuid);
		}

		public static void PurgeByPointTemplatePropertyGuidSQL(SqlCommand cmd, Guid pointTemplatePropertyGuid)
		{
			cmd.CommandText = " DELETE FROM map.tblPointAccessGroupToExposedPropertySetting"
									+ " WHERE PointSettingGuid = @PointTemplatePropertyGuid";

			cmd.Parameters.AddWithValue("@PointTemplatePropertyGuid", pointTemplatePropertyGuid);
		}


		public static void EnumerateByPointAccessGroupGuidSQL(SqlCommand cmd, Guid pointAccessGroupGuid)
		{
            cmd.CommandText += "SET NOCOUNT ON";

			cmd.CommandText += " DECLARE @PointSettings Table(ID nvarchar(50), PropertyID nvarchar(50), ValueType int, ModifyDisabled bit)"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Point ID', 'PointId', 2, CAST(1 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Point Description', 'Description', 2, CAST(1 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Point Enabled', 'Enabled', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Product', 'ProductID', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('ProductDescription', 'ProductDescription', 2, CAST(1 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Site Name', 'SiteID', 2, CAST(1 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Site Number', 'SiteNumber', 2, CAST(1 as bit))"

									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Level Units', 'LevelUnits', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Level Min', 'LevelMin', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Level Max', 'LevelMax', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Temperature Units', 'TemperatureUnits', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Temp Min', 'TempMin', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Temp Max', 'TempMax', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Volume Units', 'VolumeUnits', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Volume Min', 'VolumeMin', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Volume Max', 'VolumeMax', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Mass Units', 'MassUnits', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Mass Min', 'MassMin', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Mass Max', 'MassMax', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Density Units', 'DensityUnits', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Density Min', 'DensityMin', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Density Max', 'DensityMax', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Standard Density Units', 'StandardDensityUnits', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Standard Density Min', 'StandardDensityMin', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Standard Density Max', 'StandardDensityMax', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Volume Rate Units', 'VolumeRateUnits', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Pressure Units', 'PressureUnits', 2, CAST(0 as bit))"
									+ " INSERT INTO @PointSettings (ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Point Detail', 'PointDetailDrawingID', 2, CAST(0 as bit))";

			
				cmd.CommandText += " DECLARE @PropertySettings Table(SettingID nvarchar(30), ID nvarchar(50), PropertyID nvarchar(50), ValueType int, ModifyDisabled bit)"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Leak Detection', 'Gauge Type', 'GaugeType', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Leak Detection', 'Leak Analysis Method', 'LeakAnalysisMethodString', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Leak Detection', 'Leak Analysis Type', 'LeakAnalysisTypeString', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Strap Table', 'Product Table', 'ProductTable', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Strap Table', 'Bottoms Table', 'BottomsTable', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Strap Table', 'Solids Table', 'SolidsTable', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Strap Table', 'Strap Temperauture', 'StrapTemperature', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Strap Table', 'Strap Density', 'StrapDensity', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Strap Table', 'Roof Landing Height', 'RoofLandingHeight', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Strap Table', 'Roof Floating Height', 'RoofFloatingHeight', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Strap Table', 'Datum Height', 'DatumHeight', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Strap Table', 'Roof Type', 'RoofType', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Strap Table', 'Roof Mass', 'RoofMass', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Tank Command Settings', 'Movement Alarm Differential', 'MovementAlarmDifferential', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Tank Transfer Settings', 'Transfer Advisory Time', 'TransferAdvisoryTime', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Tank Installation Date', 'TankInstallationDate', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'CST Manufacture Date', 'CSTManufactureDate', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'CST Commission Date', 'CSTCommissionDate', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Tank Geometry', 'TankGeometryEnumText', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Tank Volume', 'TankVolume', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Tank Height', 'TankHeight', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Tank Radius', 'TankRadius', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Tank Shell Thickness', 'TankShellThickness', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Tank Lining Material', 'TankLiningMaterial', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Tank Material', 'TankMaterialEnumText', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Tank Expansion Coefficient', 'TankExpansionCoefficient', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Cathodic Protection Supported', 'CathodicProtectionSupported', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Overfill Protection Supported', 'OverfillProtectionSupported', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Spill Protection Supported', 'SpillProtectionSupported', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Tank Shell Insulated', 'TankShellInsulated', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Area Coefficient', 'AreaCoefficient', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'CST Manufacturer', 'CSTManufacturerName', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'CST Serial Number', 'CSTSerialNumber', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'CST Location Name', 'CSTLocationName', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'CST Latitude', 'CSTLatitude', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'CST Longitude', 'CSTLongitude', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Latitude Degrees', 'LatitudeDegrees', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Latitude Minutes', 'LatitudeMinutes', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Vessel', 'Latitude Seconds', 'LatitudeSeconds', 1, CAST(0 as bit))"


									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Movement Transfer Settings', 'Transfer Advisory Time', 'TransferAdvisoryTime', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Volume Correction', 'Correction Standard/Organization', 'CorrectionStandardOrOrganization', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Volume Correction', 'Correction Revision', 'CorrectionStandardRevision', 1, CAST(0 as bit))"
									+ " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Volume Correction', 'Correction Commodity/Table', 'CorrectionCommodityOrTable', 1, CAST(0 as bit))"
                           + " INSERT INTO @PropertySettings (SettingID, ID, PropertyID, ValueType, ModifyDisabled) VALUES ('Volume Correction', 'Temperature Standard', 'BaseTemperature', 1, CAST(0 as bit))";



            cmd.CommandText	+= " SELECT ID, PropertyID, ValueType, ExposedSettingGuid, PointTemplateGuid, SiteGuid, PointAccessGroupGuid, PointAccessGroupToExposedSettingGuid, [View], Modify, ModifyDisabled, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate FROM"
									+ " (SELECT ps.ID, pt.ID as TemplateID, ps.PropertyID, ps.ValueType, ptp.PointTemplatePropertyGuid as ExposedSettingGuid, pt.PointTemplateGuid, pt.SiteGuid, pagteps.PointAccessGroupGuid, pagteps.PointAccessGroupToExposedSettingGuid,"
									+ " pagteps.[View],"
									+ " pagteps.Modify, "
									+ " ps.ModifyDisabled,"
									+ " pagteps.CreatedBy, pagteps.CreatedDate, pagteps.UpdatedBy, pagteps.UpdatedDate FROM map.tblPointAccessGroupToExposedPropertySetting pagteps"
									+ " LEFT JOIN @PropertySettings ps ON ps.PropertyID = pagteps.PropertyID"
									+ " LEFT JOIN dbo.tblPointTemplateProperty ptp ON pagteps.PointSettingGuid = ptp.PointTemplatePropertyGuid AND pagteps.PropertyID = ps.PropertyID"
									+ " LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = ptp.PointTemplateGuid"
									+ " WHERE pagteps.PointAccessGroupGuid = @PointAccessGroupGuid"
									+ " UNION"
									+ " SELECT ps.ID, pt.ID as TemplateID, ps.PropertyID, ps.ValueType, pt.PointTemplateGuid as ExposedSettingGuid, pt.PointTemplateGuid, pt.SiteGuid, pagteps.PointAccessGroupGuid, pagteps.PointAccessGroupToExposedSettingGuid,"
									+ " pagteps.[View],"
									+ " pagteps.Modify,"
									+ " ps.ModifyDisabled,"
									+ " pagteps.CreatedBy, pagteps.CreatedDate, pagteps.UpdatedBy, pagteps.UpdatedDate FROM map.tblPointAccessGroupToExposedPointSetting pagteps"
									+ " LEFT JOIN @PointSettings ps ON ps.PropertyID = pagteps.PropertyID"
									+ " LEFT JOIN  dbo.tblPointTemplate pt ON pagteps.PointSettingGuid = pt.PointTemplateGuid AND pagteps.PropertyID = ps.PropertyID"
									+ " WHERE pagteps.PointAccessGroupGuid = @PointAccessGroupGuid) s"
									+ " ORDER BY s.ID, s.TemplateID";


			cmd.Parameters.AddWithValue("@PointAccessGroupGuid", pointAccessGroupGuid);
		}
	}
}

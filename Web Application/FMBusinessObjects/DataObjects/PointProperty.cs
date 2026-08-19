namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.Reflection;
	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using System.Text;

	

	#region PointProperty Collection Class
	[KnownType(typeof(StrapTable))]
	[KnownType(typeof(Vessel))]
	[KnownType(typeof(VcfModuleSettings))]
	[KnownType(typeof(QuantityModuleSettings))]
	[KnownType(typeof(RateModuleSettings))]
	[KnownType(typeof(TankCommandModuleSettings))]
	[KnownType(typeof(TankTransferModuleSettings))]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(PointCommandStatusListReference))]
	[KnownType(typeof(VolumeTransferModuleSettings))]
	[KnownType(typeof(MovementModuleSettings))]
	[KnownType(typeof(MovementNodeData))]
	[KnownType(typeof(LeakDetectionSettings))]
	[KnownType(typeof(CodedVariables.MovementType))]
	[Serializable]
	[CollectionDataContract]
	public class PointPropertyCollection : List<PointProperty>
	{
	}
	#endregion


	[KnownType(typeof(StrapTable))]
	[KnownType(typeof(Vessel))]
	[KnownType(typeof(VcfModuleSettings))]
	[KnownType(typeof(QuantityModuleSettings))]
	[KnownType(typeof(RateModuleSettings))]
	[KnownType(typeof(TankCommandModuleSettings))]
	[KnownType(typeof(TankTransferModuleSettings))]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(PointCommandStatusListReference))]
	[KnownType(typeof(VolumeTransferModuleSettings))]
	[KnownType(typeof(MovementModuleSettings))]
	[KnownType(typeof(MovementData))]
	[KnownType(typeof(MovementNodeData))]
	[KnownType(typeof(LeakDetectionSettings))]
	[KnownType(typeof(CodedVariables.MovementType))]
	[DataContract]
	[Serializable]
	public class PointProperty : BaseSerializedDataObject, ICloneable
	{
		[EntityImportExportAttribute("SETTINGID*", 200, "ID")]
		[DataMember]
		[FMPersistedField]
		public override string ID { get { return base.ID; } set { base.ID = value; } }



		public new Guid SiteGuid
		{
				get
				{
					return base.SiteGuid;
				}
				set
				{
					base.SiteGuid = value;
				}
		}


		[EntityImportExportAttribute("VALUETYPE", 100, "ValueTypeString")]
		[DataMember]
		[FMPersistedField("ValueType")]
		new public string ValueTypeString
		{
			get
			{
				return this._valueType.ToString();
			}
			set
			{
				this._valueType = Type.GetType(value);
			}
		}
		/// <summary>
		/// Uses DataContractSerializer instead of XmlSerializer
		/// </summary>
		[EntityImportExportAttribute("VALUE", 100, "ValueXml")]
		[FMPersistedField("Value")]
		new public string ValueXml
		{
			get
			{
				using (MemoryStream stream = new MemoryStream())
				{
					DataContractSerializer serializer = new DataContractSerializer(this.Value.GetType());
					serializer.WriteObject(stream, this.Value);
					return new UTF8Encoding().GetString(stream.ToArray());
				}
			}

			set
			{
				using (MemoryStream memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(value)))
				{
					DataContractSerializer serializer = new DataContractSerializer(this.ValueType);
					this._value = serializer.ReadObject(memoryStream);
				}
			}
		}

		[FMPersistedField]
		[DataMember]
		public Guid PointGuid { get; set; }

		[DataMember]
		[FMPersistedField("PointID", ReadOnly = true)]
		public string PointID { get; set; }

		[DataMember]
		public Boolean WrittenToEnterprise { get; set; }



		[EntityImportExportAttribute("SETTINGGUID", 200, "PointPropertyGuid")]
		[FMPersistedField]
		public Guid PointPropertyGuid
		{
				get
				{
					return this.IdentityGuid;
				}
				set
				{
					this.IdentityGuid = value;
				}
		}

		[EntityImportExportAttribute("POINTTEMPLATEPROPERTYGUID", 200, "PointTemplatePropertyGuid")]
		[FMPersistedField]
		[DataMember]
		public Guid PointTemplatePropertyGuid { get; set; }


		public PointProperty() { }

		public PointProperty(PointTemplateProperty templateProperty)
		{
			base.IdentityGuid = Guid.NewGuid();
			base.ID = templateProperty.ID;
			this.Value = templateProperty.Value;
			this.ValueType = templateProperty.ValueType;
			this.ValueTypeString = templateProperty.ValueTypeString;
			this.PointTemplatePropertyGuid = templateProperty.PointTemplatePropertyGuid;
			this.WrittenToEnterprise = false;
		}


		public object Clone()
		{
				var p = (PointProperty)this.MemberwiseClone();
	//			this.BaseClone(p);
				return p;
		}

		public void GetByPropertyGuid(SqlCommand cmd, Guid propertyGuid)
		{
				cmd.CommandText = "SELECT * FROM tblPointProperty WHERE PointPropertyGuid = @PropertyGuid";
				cmd.Parameters.AddWithValue("@PropertyGuid", propertyGuid);
		}

		public void EnumerateByPointListSQL(SqlCommand cmd, List<Guid> pointGuidList)
		{
				cmd.CommandText = "SELECT pp.*, p.ID AS PointID FROM dbo.tblPointProperty pp"
									+ " LEFT OUTER JOIN tblPoint p ON p.PointGuid = pp.PointGuid"
									+ " INNER JOIN @PointTable ptbl ON ptbl.Guid = pp.PointGuid"
									+ " ORDER BY pp.PointGuid";

				var pointTable = new DataTable();
				pointTable.Columns.Add("Guid", typeof(Guid));
				foreach (var pointGuid in pointGuidList)
				{
					var row = pointTable.NewRow();
					row[0] = pointGuid;

					pointTable.Rows.Add(row);
				}

				SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointTable", SqlDbType.Structured);
				tableValuedParameter.Value = pointTable;
				tableValuedParameter.TypeName = "dbo.GuidListType";
		}

		public static void EnumerateByPointPropertyListSQL(SqlCommand cmd, List<Guid> pointPropertyGuidList)
		{
			cmd.CommandText = "SELECT pp.*, p.ID AS PointID  FROM dbo.tblPointProperty pp"
									+ " LEFT OUTER JOIN tblPoint p ON p.PointGuid = pp.PointGuid"
									+ " INNER JOIN @PointPropertyTable pptbl ON pptbl.Guid = pp.PointPropertyGuid";

			var pointPropertyTable = new DataTable();
			pointPropertyTable.Columns.Add("Guid", typeof(Guid));
			foreach (var pointPropertyGuid in pointPropertyGuidList)
			{
				var row = pointPropertyTable.NewRow();
				row[0] = pointPropertyGuid;

				pointPropertyTable.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@PointPropertyTable", SqlDbType.Structured);
			tableValuedParameter.Value = pointPropertyTable;
			tableValuedParameter.TypeName = "dbo.GuidListType";
		}


		public static List<string> GetFieldNames(PointProperty pointProp)
		{
				var ret = new List<string>();
				foreach (FieldInfo field in pointProp.ValueType.GetFields())
				{
					var obj = field.GetValue(pointProp.Value);
					if (obj.GetType() == typeof(PointPropertyField))
					{
						ret.Add(field.Name);
					}
				}
				return ret;
		}

		protected static PointPropertyField GetPropertyFieldByFieldName(string fieldName, PointProperty pointProp)
		{
				foreach (FieldInfo field in pointProp.ValueType.GetFields())
				{
					if (field.Name == fieldName)
					{
						var obj = field.GetValue(pointProp.Value);
						if (obj.GetType() == typeof(PointPropertyField))
						{
								var tag = (PointPropertyField)obj;
								return tag;
						}
					}
				}
				return null;
		}

		public static string GetFieldValue(string fieldName, PointProperty pointProp)
		{
				string ret = string.Empty;
				var tag = GetPropertyFieldByFieldName( fieldName, pointProp);
				if(tag != null)
				{
					return tag.Value.ToString();
				}
				return ret;
		}

		public static string GetFieldUnitType(string fieldName, PointProperty pointProp)
		{
				var field = GetPropertyFieldByFieldName(fieldName, pointProp);
				return GetFieldUnitType(field);
		}

		public static string GetFieldUnitType(object field)
		{
			const string NoDimensionUnit = "PENoDimensionUnit";
			string ret = NoDimensionUnit;

			EngineeringUnitType type = EngineeringUnitType.FmuNone;
			if (field is PointPropertyField)
			{
				type = (field as PointPropertyField).EngineeringUnitsType;
			}
			else if (field is PointPropertyUnitTypedDouble)
			{
				type = (field as PointPropertyUnitTypedDouble).EngineeringUnitsType;
			}

			if (field != null)
			{
				switch (type)
				{
					case EngineeringUnitType.FmuAll:
						ret = "PEAllEngineeringUnits";
						break;
					case EngineeringUnitType.FmuTemp:
						ret = "PETemperatureEngineeringUnits";
						break;
					case EngineeringUnitType.FmuTime:
						ret = "PETimeEngineeringUnits";
						break;
					case EngineeringUnitType.FmuLength:
						ret = "PELevelEngineeringUnits";
						break;
					case EngineeringUnitType.FmuArea:
						ret = "PEAreaEngineeringUnits";
						break;
					case EngineeringUnitType.FmuVolume:
						ret = "PEVolumeEngineeringUnits";
						break;
					case EngineeringUnitType.FmuMass:
						ret = "PEMassEngineeringUnits";
						break;
					case EngineeringUnitType.FmuPressure:
						ret = "PEPressureEngineeringUnits";
						break;
					case EngineeringUnitType.FmuVolflow:
						ret = "PEVolumetricFlowEngineeringUnits";
						break;
					case EngineeringUnitType.FmuMassflow:
						ret = "PEMassFlowEngineeringUnits";
						break;
					case EngineeringUnitType.FmuVelocity:
						ret = "PEVelocityEngineeringUnits";
						break;
					case EngineeringUnitType.FmuDensity:
						ret = "PEDensityEngineeringUnits";
						break;
					case EngineeringUnitType.FmuEnergy:
						ret = "PEEnergyEngineeringUnits";
						break;
					case EngineeringUnitType.FmuPower:
						ret = "PEPowerEngineeringUnits";
						break;
					case EngineeringUnitType.FmuElect:
						ret = "PEElectricalEngineeringUnits";
						break;
					case EngineeringUnitType.FmuNodim:
						ret = NoDimensionUnit;
						break;
					case EngineeringUnitType.FmuNone:
						ret = "PENoneEngineeringUnits";
						break;
				}
			}
			return ret;
		}

		public static string GetFieldUnitDecimalPlaces(object field)
		{
			const string NoDimensionUnit = "PENoDimensionDecimalPlaces";
			string ret = NoDimensionUnit;

			EngineeringUnitType type = EngineeringUnitType.FmuNone;
			if (field is PointPropertyField)
			{
				type = (field as PointPropertyField).EngineeringUnitsType;
			}
			else if (field is PointPropertyUnitTypedDouble)
			{
				type = (field as PointPropertyUnitTypedDouble).EngineeringUnitsType;
			}
			if (field != null)
			{
				switch (type)
				{
					case EngineeringUnitType.FmuAll:
							ret = "PEAllDecimalPlaces";
							break;
					case EngineeringUnitType.FmuTemp:
							ret = "PETemperatureDecimalPlaces";
							break;
					case EngineeringUnitType.FmuTime:
							ret = "PETimeDecimalPlaces";
							break;
					case EngineeringUnitType.FmuLength:
							ret = "PELevelDecimalPlaces";
							break;
					case EngineeringUnitType.FmuArea:
							ret = "PEAreaDecimalPlaces";
							break;
					case EngineeringUnitType.FmuVolume:
							ret = "PEVolumeDecimalPlaces";
							break;
					case EngineeringUnitType.FmuMass:
							ret = "PEMassDecimalPlaces";
							break;
					case EngineeringUnitType.FmuPressure:
							ret = "PEPressureDecimalPlaces";
							break;
					case EngineeringUnitType.FmuVolflow:
							ret = "PEFlowDecimalPlaces";
							break;
					case EngineeringUnitType.FmuMassflow:
							ret = "PEMassFlowDecimalPlaces";
							break;
					case EngineeringUnitType.FmuVelocity:
							ret = "PEVelocityDecimalPlaces";
							break;
					case EngineeringUnitType.FmuDensity:
							ret = "PEDensityDecimalPlaces";
							break;
					case EngineeringUnitType.FmuEnergy:
							ret = "PEEnergyDecimalPlaces";
							break;
					case EngineeringUnitType.FmuPower:
							ret = "PEPowerDecimalPlaces";
							break;
					case EngineeringUnitType.FmuElect:
							ret = "PEElectricalDecimalPlaces";
							break;
					case EngineeringUnitType.FmuNodim:
							ret = NoDimensionUnit;
							break;
					case EngineeringUnitType.FmuNone:
							ret = "PENoneDecimalPlaces";
							break;
				}
			}
			return ret;
		}


		public static string GetFieldUnitDecimalPlaces(string fieldName, PointProperty pointProp)
		{
				var field = GetPropertyFieldByFieldName(fieldName, pointProp);
				return GetFieldUnitDecimalPlaces(field);
		}

		public List<PointValue> GetExposedSettings(Point point)
		{
			var pointValues = new List<PointValue>();

			if(this.Value == null)
			{
				return pointValues;
			}

			var propertyType = this.Value.GetType();
			var propertyInfos = propertyType.GetProperties();
			foreach (var propertyInfo in propertyInfos)
			{
				var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);
				if (fmExposedSettings.Length == 0)
				{
					continue;
				}

				var pointValueIdentifier = new PointValueIdentifier() { IdentityGuid = this.PointPropertyGuid, PointValueType = PointValueType.Setting, PropertyID = propertyInfo.Name };

				var pointValue = new PointValue(pointValueIdentifier, this, point);
				pointValues.Add(pointValue);
			}

			return pointValues;
		}

		public List<PointValueIdentifier> GetExposedSettingPointValueIdentifiers()
		{
			
			return this.GetExposedSettingPointValueIdentifiersFilterByType(false, string.Empty, PointValueFieldType.VALUE);
		}

		public List<PointValueIdentifier> GetExposedSettingPointValueIdentifiersFilterByType(bool filter, string dataTypeString, PointValueFieldType fieldFilter)
		{
			var pointValueIddentifiers = new List<PointValueIdentifier>();

			var propertyType = this.Value.GetType();
			var propertyInfos = propertyType.GetProperties();

			foreach (var propertyInfo in propertyInfos)
			{
				var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);
				if (fmExposedSettings.Length == 0)
				{
					continue;
				}
				if (!filter || 
					(fieldFilter == PointValueFieldType.VALUE && propertyInfo.PropertyType.FullName == dataTypeString) ||
					fieldFilter == PointValueFieldType.ID ||
					fieldFilter == PointValueFieldType.TIMESTAMP ||
					fieldFilter == PointValueFieldType.UNITS)
				{
					var pointValueIdentifier = new PointValueIdentifier()
					{
						IdentityGuid = this.PointPropertyGuid,
						PointValueType = PointValueType.Setting,
						PropertyID = propertyInfo.Name,
						SiteGuid = this.SiteGuid
					};
					pointValueIddentifiers.Add(pointValueIdentifier);
				}
			}

			return pointValueIddentifiers;
		}

		public string GetExposedSettingID(PointValueIdentifier pointValueIdentifier)
		{
			string localId = string.Empty;
			var propertyType = this.Value.GetType();
			var propertyInfos = propertyType.GetProperties();

			foreach (var propertyInfo in propertyInfos)
			{
				if (pointValueIdentifier.PropertyID != propertyInfo.Name)
				{
					continue;
				}

				var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);

				if (fmExposedSettings.Length == 0)
				{
					break;
				}

				var fmExposedSetting = fmExposedSettings[0] as FMExposedSetting;

				if (fmExposedSetting != null)
				{
					localId = fmExposedSetting.ID;
				}

				break;
			}

			return localId;
		}

		public FMExposedSetting GetExposedSettingAttribute(PointValueIdentifier pointValueIdentifier)
		{
			var propertyType = this.Value.GetType();
			var propertyInfos = propertyType.GetProperties();
			foreach (var propertyInfo in propertyInfos)
			{
				if (pointValueIdentifier.PropertyID != propertyInfo.Name)
				{
					continue;
				}

				var fmExposedSettings = propertyInfo.GetCustomAttributes(typeof(FMExposedSetting), false);
				if (fmExposedSettings.Length == 0)
				{
					break;
				}

				return fmExposedSettings[0] as FMExposedSetting;
			}

			return null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
	[Serializable]
	public class TransactionLineItemSelectionDO
	{
		[DataMember]
		public short? SequenceID;
		[DataMember]
		public double? MeterStart;
		[DataMember]
		public double? MeterStop;
		[DataMember]
		public double? GrossQuantity;
		[DataMember]
		public double? Temperature;
		[DataMember]
		public double? Vcf;
		[DataMember]
		public double? Density;
		[DataMember]
		public string Product;
		[DataMember]
		public string ProductCode;
		[DataMember]
		public string ProductType;
		[DataMember]
		public double? ProductPrice;
		[DataMember]
		public string CLIN;
		[DataMember]
		public double? NetQuantity;
		[DataMember]
		public string ContractNumber;
		[DataMember]
		public string DestinationRegistrationID;
		[DataMember]
		public string DestinationSerialNumber;
		[DataMember]
		public string DestinationEquipmentType;
		[DataMember]
		public string DestinationEquipmentModel;
		[DataMember]
		public string DestinationCompanyEquipmentID;
		[DataMember]
		public string DestinationCompartmentID;
		[DataMember]
		public string SourceRegistrationID;
		[DataMember]
		public string SourceSerialNumber;
		[DataMember]
		public string SourceEquipmentType;
		[DataMember]
		public string SourceEquipmentModel;
		[DataMember]
		public string SourceCompanyEquipmentID;
		[DataMember]
		public string SourceCompartmentID;
		[DataMember]
		public double? MeterFactor;
		[DataMember]
		public string LineItemSequenceNumber;
		[DataMember]
		public string BatchNumber;
		[DataMember]
		public string DocumentNumber;
		[DataMember]
		public double? LineFill;
		[DataMember]
		public double? BottomVolume;
		[DataMember]
		public double? NetCapacity;
		[DataMember]
		public string Customs;
		[DataMember]
		public int? ArmNumber;
		[DataMember]
		public int? LineNumber;
		[DataMember]
		public string OperatorID;
		[DataMember]
		public string TankStatus;
		[DataMember]
		public string MeterStartDateTime;
		[DataMember]
		public string MeterStopDateTime;
		[DataMember]
		public string Pit;
		[DataMember]
		public string RequestedDateTime;
		[DataMember]
		public string DispatchedDateTime;
		[DataMember]
		public string AcknowledgedDateTime;
		[DataMember]
		public string OnLocationTime;
		[DataMember]
		public string ValidationDateTime;
		[DataMember]
		public string CompletionDateTime;
		[DataMember]
		public double? ReceiptVariance;
		[DataMember]
		public double? DifferentialPressure;
		[DataMember]
		public double? LoadRackVariance;
		[DataMember]
		public string RequestedBy;
		[DataMember]
		public double? FreezePoint;
		[DataMember]
		public bool? DeleteFlag;
		[DataMember]
		public string StorageLocationID;
		[DataMember]
		public string MeterID;
		[DataMember]
		public string AdditiveProfileID;
		[DataMember]
		public string CreatedBy;
		[DataMember]
		public string CreatedDate;
		[DataMember]
		public string UpdatedBy;
		[DataMember]
		public string UpdatedDate;
		[DataMember]
		public double? PresetAmount;
		[DataMember]
		public int? EngineeringUnitsIndex;
		[DataMember]
		public string CustomerProductName;
		[DataMember]
		public string CustomerProductCode;
		[DataMember]
		public string TransactionInventoryDate;
		[DataMember]
		public bool? COAWaiver;
		[DataMember]
		public string COANote;
		[DataMember]
		public string COAID;
		[DataMember]
		public double? Tax1;
		[DataMember]
		public double? Tax2;
		[DataMember]
		public double? Tax3;
		[DataMember]
		public double? Tax4;
		[DataMember]
		public double? Tax5;
		[DataMember]
		public long? TransVersion;
		[DataMember]
		public string LoadingLocationID;
		[DataMember]
		public bool? ImproperAdditization;
		[DataMember]
		public bool? BrokenBlend;
		[DataMember]
		public bool? ContaminatePrompt;
		[DataMember]
		public bool? CompartmentsPreviouslyLoaded;
		[DataMember]
		public bool? CompartmentsEmpty;
		[DataMember]
		public bool? Flag01;
		[DataMember]
		public bool? Flag02;
		[DataMember]
		public bool? Flag03;
		[DataMember]
		public bool? Flag04;
		[DataMember]
		public bool? Flag05;
		[DataMember]
		public bool? Flag06;
		[DataMember]
		public double? Number01;
		[DataMember]
		public double? Number02;
		[DataMember]
		public double? Number03;
		[DataMember]
		public double? Number04;
		[DataMember]
		public double? Number05;
		[DataMember]
		public double? Number06;
		[DataMember]
		public double? OdometerHours;
		[DataMember]
		public string EndDeliveryDate;
		[DataMember]
		public string RequestedDeliveryDate;
		[DataMember]
		public string InvoiceNumber;
		[DataMember]
		public string InvoiceLineNumber;
		[DataMember]
		public double? AlternativeGrossVolume;
		[DataMember]
		public double? AlternativeNetVolume;
		[DataMember]
		public int? AlternativeUnits;
		[DataMember]
		public double? TankLevel;
		[DataMember]
		public int? TankLevelUnits;
		[DataMember]
		public string Date01;
		[DataMember]
		public string Date02;
		[DataMember]
		public string Date03;
		[DataMember]
		public string Date04;
		[DataMember]
		public double? NonDomesticPrice;
		[DataMember]
		public int? CurrencyUnit;
		[DataMember]
		public double? ExchangeRate;
		[DataMember]
		public string QualityTestNumber;
		[DataMember]
		public double? Odometer;
		[DataMember]
		public string DeliveryLocation;
		[DataMember]
		public double? Variance;
		[DataMember]
		public bool? PartialFill;
		[DataMember]
		public double? MassQuantity;
		[DataMember]
		public bool? NetManualValueFlag;
		[DataMember]
		public bool? MassManualValueFlag;
		[DataMember]
		public bool? GrossManualValueFlag;
		[DataMember]
		public bool? VcfManualValueFlag;
		[DataMember]
		public Guid TransactionLineItemGuid;
		[DataMember]
		public long _RowVersion;
		[DataMember]
		public int? LookupTransactionStatusIndex;
		[DataMember]
		public int LookupQualityIndex;
		[DataMember]
		public Guid? StorageLocationTankGuid;
		[DataMember]
		public Guid? AdditiveProfileGuid;
		[DataMember]
		public Guid? DestinationCompartmentEquipmentGuid;
		[DataMember]
		public Guid? DestinationEquipmentGuid;
		[DataMember]
		public Guid? OperatorPersonnelGuid;
		[DataMember]
		public Guid? ProductGuid;
		[DataMember]
		public Guid? SourceCompartmentEquipmentGuid;
		[DataMember]
		public Guid? SourceEquipmentGuid;
		[DataMember]
		public Guid TransactionGuid;
		[DataMember]
		public Guid? CurrencyGuid;
		[DataMember]
		public Guid? OrderReferenceTransactionLineItemGuid;
		[DataMember]
		public Guid? LoadingLocationStationGuid;
		[DataMember]
		public Guid? MeterGuid;

		protected static string getDateTimeOffset(object o)
		{
			if (o == System.DBNull.Value)
			{
				return null;
			}
			return (DataObject.getValue<DateTimeOffset>(o, TimeConverter.MinFMDate)).ToString("yyyy-MM-ddTHH:mm:ss.fff");
		}

		protected static short? getShort(object o)
		{
			if (o == System.DBNull.Value)
			{
				return null;
			}
			return (short?)o;
		}

		protected static int? getInt(object o)
		{
			if (o == System.DBNull.Value)
			{
				return null;
			}
			return (int?)o;
		}

		protected static long? getLong(object o)
		{
			if (o == System.DBNull.Value)
			{
				return null;
			}
			return (long?)o;
		}

		protected static bool? getBool(object o)
		{
			if (o == System.DBNull.Value)
			{
				return null;
			}
			return (bool?)o;
		}

		protected static double? getDouble(object o)
		{
			if (o == System.DBNull.Value)
			{
				return null;
			}
			return (double?)o;
		}

		protected static Guid? getGuid(object o)
		{
			if (o == System.DBNull.Value)
			{
				return null;
			}
			return (Guid?)o;
		}

		protected static string getString(object o)
		{
			if (o == System.DBNull.Value)
			{
				return null;
			}
			return Convert.ToString(o);
		}


		public void LoadRow(DataRow row)
		{
			if (row != null)
			{
				SequenceID = getShort(row["SequenceID"]);
				MeterStart = getDouble(row["MeterStart"]);
				MeterStop = getDouble(row["MeterStop"]);
				GrossQuantity = getDouble(row["GrossQuantity"]);
				Temperature = getDouble(row["Temperature"]);
				Vcf = getDouble(row["Vcf"]);
				Density = getDouble(row["Density"]);
				Product = getString(row["Product"]);
				ProductCode = getString(row["ProductCode"]);
				ProductType = getString(row["ProductType"]);
				ProductPrice = getDouble(row["ProductPrice"]);
				CLIN = getString(row["CLIN"]);
				NetQuantity = getDouble(row["NetQuantity"]);
				ContractNumber = getString(row["ContractNumber"]);
				DestinationRegistrationID = getString(row["DestinationRegistrationID"]);
				DestinationSerialNumber = getString(row["DestinationSerialNumber"]);
				DestinationEquipmentType = getString(row["DestinationEquipmentType"]);
				DestinationEquipmentModel = getString(row["DestinationEquipmentModel"]);
				DestinationCompanyEquipmentID = getString(row["DestinationCompanyEquipmentID"]);
				DestinationCompartmentID = getString(row["DestinationCompartmentID"]);
				SourceRegistrationID = getString(row["SourceRegistrationID"]);
				SourceSerialNumber = getString(row["SourceSerialNumber"]);
				SourceEquipmentType = getString(row["SourceEquipmentType"]);
				SourceEquipmentModel = getString(row["SourceEquipmentModel"]);
				SourceCompanyEquipmentID = getString(row["SourceCompanyEquipmentID"]);
				SourceCompartmentID = getString(row["SourceCompartmentID"]);
				MeterFactor = getDouble(row["MeterFactor"]);
				LineItemSequenceNumber = getString(row["LineItemSequenceNumber"]);
				BatchNumber = getString(row["BatchNumber"]);
				DocumentNumber = getString(row["DocumentNumber"]);
				LineFill = getDouble(row["LineFill"]);
				BottomVolume = getDouble(row["BottomVolume"]);
				NetCapacity = getDouble(row["NetCapacity"]);
				Customs = getString(row["Customs"]);
				ArmNumber = getInt(row["ArmNumber"]);
				LineNumber = getInt(row["LineNumber"]);
				OperatorID = getString(row["OperatorID"]);
				TankStatus = getString(row["TankStatus"]);
				MeterStartDateTime = getDateTimeOffset(row["MeterStartDateTime"]);
				MeterStopDateTime = getDateTimeOffset(row["MeterStopDateTime"]);
				Pit = getString(row["Pit"]);
				AcknowledgedDateTime = getDateTimeOffset(row["AcknowledgedDateTime"]);
				OnLocationTime = getDateTimeOffset(row["OnLocationTime"]);
				ValidationDateTime = getDateTimeOffset(row["ValidationDateTime"]);
				CompletionDateTime = getDateTimeOffset(row["CompletionDateTime"]);
				ReceiptVariance = getDouble(row["ReceiptVariance"]);
				DifferentialPressure = getDouble(row["DifferentialPressure"]);
				LoadRackVariance = getDouble(row["LoadRackVariance"]);
				RequestedBy = getString(row["RequestedBy"]);
				FreezePoint = getDouble(row["FreezePoint"]);
				DeleteFlag = getBool(row["DeleteFlag"]);
				StorageLocationID = getString(row["StorageLocationID"]);
				MeterID = getString(row["MeterID"]);
				AdditiveProfileID = getString(row["AdditiveProfileID"]);
				PresetAmount = getDouble(row["PresetAmount"]);
				EngineeringUnitsIndex = getInt(row["EngineeringUnitsIndex"]);
				CustomerProductName = getString(row["CustomerProductName"]);
				CustomerProductCode = getString(row["CustomerProductCode"]);
				TransactionInventoryDate = getDateTimeOffset(row["TransactionInventoryDate"]);
				COAWaiver = getBool(row["COAWaiver"]);
				COANote = getString(row["COANote"]);
				COAID = getString(row["COAID"]);
				Tax1 = getDouble(row["Tax1"]);
				Tax2 = getDouble(row["Tax2"]);
				Tax3 = getDouble(row["Tax3"]);
				Tax4 = getDouble(row["Tax4"]);
				Tax5 = getDouble(row["Tax5"]);
				TransVersion = getLong(row["TransVersion"]);
				LoadingLocationID = getString(row["LoadingLocationID"]);
				ImproperAdditization = getBool(row["ImproperAdditization"]);
				BrokenBlend = getBool(row["BrokenBlend"]);
				ContaminatePrompt = getBool(row["ContaminatePrompt"]);
				CompartmentsPreviouslyLoaded = getBool(row["CompartmentsPreviouslyLoaded"]);
				CompartmentsEmpty = getBool(row["CompartmentsEmpty"]);
				OdometerHours = getDouble(row["OdometerHours"]);
				EndDeliveryDate = getDateTimeOffset(row["EndDeliveryDate"]);
				InvoiceNumber = getString(row["InvoiceNumber"]);
				InvoiceLineNumber = getString(row["InvoiceLineNumber"]);
				AlternativeGrossVolume = getDouble(row["AlternativeGrossVolume"]);
				AlternativeNetVolume = getDouble(row["AlternativeNetVolume"]);
				AlternativeUnits = getInt(row["AlternativeUnits"]);
				TankLevel = getDouble(row["TankLevel"]);
				TankLevelUnits = getInt(row["TankLevelUnits"]);
				NonDomesticPrice = getDouble(row["NonDomesticPrice"]);
				CurrencyUnit = getInt(row["CurrencyUnit"]);
				ExchangeRate = getDouble(row["ExchangeRate"]);
				QualityTestNumber = getString(row["QualityTestNumber"]);
				Odometer = getDouble(row["Odometer"]);
				DeliveryLocation = getString(row["DeliveryLocation"]);
				Variance = getDouble(row["Variance"]);
				PartialFill = getBool(row["PartialFill"]);
				MassQuantity = getDouble(row["MassQuantity"]);
				NetManualValueFlag = getBool(row["NetManualValueFlag"]);
				MassManualValueFlag = getBool(row["MassManualValueFlag"]);
				GrossManualValueFlag = getBool(row["GrossManualValueFlag"]);
				VcfManualValueFlag = getBool(row["VcfManualValueFlag"]);
				TransactionLineItemGuid = (Guid)(row["TransactionLineItemGuid"]);
				LookupTransactionStatusIndex = getInt(row["LookupTransactionStatusIndex"]);
				LookupQualityIndex = (int)(row["LookupQualityIndex"]);
				StorageLocationTankGuid = getGuid(row["StorageLocationTankGuid"]);
				AdditiveProfileGuid = getGuid(row["AdditiveProfileGuid"]);
				DestinationCompartmentEquipmentGuid = getGuid(row["DestinationCompartmentEquipmentGuid"]);
				DestinationEquipmentGuid = getGuid(row["DestinationEquipmentGuid"]);
				ProductGuid = getGuid(row["ProductGuid"]);
				SourceCompartmentEquipmentGuid = getGuid(row["SourceCompartmentEquipmentGuid"]);
				SourceEquipmentGuid = getGuid(row["SourceEquipmentGuid"]);
				TransactionGuid =(Guid)(row["TransactionGuid"]);
				CurrencyGuid = getGuid(row["CurrencyGuid"]);
				OrderReferenceTransactionLineItemGuid = getGuid(row["OrderReferenceTransactionLineItemGuid"]);
				LoadingLocationStationGuid = getGuid(row["LoadingLocationStationGuid"]);
				MeterGuid = getGuid(row["MeterGuid"]);
				CreatedDate = getDateTimeOffset(row["CreatedDate"]);
				CreatedBy = getString(row["CreatedBy"]);
				UpdatedDate = getDateTimeOffset(row["UpdatedDate"]);
				UpdatedBy = getString(row["UpdatedBy"]);
				Flag01 = getBool(row["Flag01"]);
				Flag02 = getBool(row["Flag02"]);
				Flag03 = getBool(row["Flag03"]);
				Flag04 = getBool(row["Flag04"]);
				Flag05 = getBool(row["Flag05"]);
				Flag06 = getBool(row["Flag06"]);
				Number01 = getDouble(row["Number01"]);
				Number02 = getDouble(row["Number02"]);
				Number03 = getDouble(row["Number03"]);
				Number04 = getDouble(row["Number04"]);
				Number05 = getDouble(row["Number05"]);
				Number06 = getDouble(row["Number06"]);
				Date01 = getDateTimeOffset(row["Date01"]);
				Date02 = getDateTimeOffset(row["Date02"]);
				Date03 = getDateTimeOffset(row["Date03"]);
				Date04 = getDateTimeOffset(row["Date04"]);
				_RowVersion = DataObject.getLong(row["_RowVersion"]);
				RequestedDateTime = getDateTimeOffset(row["RequestedDateTime"]);
				DispatchedDateTime = getDateTimeOffset(row["DispatchedDateTime"]);
				RequestedDeliveryDate = getDateTimeOffset(row["RequestedDeliveryDate"]);
				OperatorPersonnelGuid = getGuid(row["OperatorPersonnelGuid"]);
			}
		}
	}

	[Serializable]
	[CollectionDataContract]
	public class TransactionLineItemSelectionCollectionDO : List<TransactionLineItemSelectionDO>
	{
		public void Get(SqlCommand sqlcommand,
								SecurityClass security,
								string operatorID,
								bool filterByOperatorID,
								string vehicleID,
								bool filterByVehicleID,
								string gateID,
								bool filterByGateID,
								int hoursInPast,
								int hoursInFuture)
		{
			sqlcommand.CommandText = "exec usp_MobileTransactionItemsSelectionSelectBy_TimeWindow_Vehicle_Gate 	@OperatorID, @filterOperatorID, @VehicleID," +
				" @filterVehicleID, @GateID, @filterGateID, @HoursInPast, @HoursInFuture, @SiteGuid";

			SqlParameter parm1 = new SqlParameter("@OperatorID", SqlDbType.NVarChar, 100) { Value = operatorID };
			sqlcommand.Parameters.Add(parm1);
			SqlParameter parm2 = new SqlParameter("@filterOperatorID", SqlDbType.Bit) { Value = filterByOperatorID };
			sqlcommand.Parameters.Add(parm2);
			SqlParameter parm3 = new SqlParameter("@VehicleID", SqlDbType.NVarChar, 100) { Value = vehicleID };
			sqlcommand.Parameters.Add(parm3);
			SqlParameter parm4 = new SqlParameter("@filterVehicleID", SqlDbType.Bit) { Value = filterByVehicleID };
			sqlcommand.Parameters.Add(parm4);
			SqlParameter parm5 = new SqlParameter("@GateID", SqlDbType.NVarChar, 100) { Value = gateID };
			sqlcommand.Parameters.Add(parm5);
			SqlParameter parm6 = new SqlParameter("@filterGateID", SqlDbType.Bit) { Value = filterByGateID };
			sqlcommand.Parameters.Add(parm6);
			SqlParameter parm7 = new SqlParameter("@HoursInPast", SqlDbType.Int) { Value = hoursInPast };
			sqlcommand.Parameters.Add(parm7);
			SqlParameter parm8 = new SqlParameter("@HoursInFuture", SqlDbType.Int) { Value = hoursInFuture };
			sqlcommand.Parameters.Add(parm8);
			SqlParameter parm9 = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
			sqlcommand.Parameters.Add(parm9);
		}

		public void Load(DataSet dataSet)
		{
			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];

				if ((table != null) && (table.Rows != null) && (table.Rows.Count > 0))
				{
					for (int i = 0; i < table.Rows.Count; i++)
					{
						DataRow row = table.Rows[i];
						TransactionLineItemSelectionDO transLine = new TransactionLineItemSelectionDO();
						transLine.LoadRow(row);
						Add(transLine);
					}
				}
			}
		}
	}
}


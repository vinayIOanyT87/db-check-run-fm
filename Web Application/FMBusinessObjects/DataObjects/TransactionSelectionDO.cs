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
	public class TransactionSelectionDO
	{
		[DataMember]
		public string TransID;
		[DataMember]
		public string AliasName;
		[DataMember]
		public string SubType;
		[DataMember]
		public string Site;
		[DataMember]
		public string TransReferenceID;
		[DataMember]
		public string InventoryDate;
		[DataMember]
		public string ShipToID;
		[DataMember]
		public string ShipToCode;
		[DataMember]
		public string SupplierID;
		[DataMember]
		public string SupplierCode;
		[DataMember]
		public string CreatedDate;
		[DataMember]
		public string CreatedBy;
		[DataMember]
		public string RequestedDeliveryDate;
		[DataMember]
		public string UpdatedDate;
		[DataMember]
		public string UpdatedBy;
		[DataMember]
		public string TransDateTime;
		[DataMember]
		public long? TransVersion;
		[DataMember]
		public string SCACCode;
		[DataMember]
		public string CardNumber;
		[DataMember]
		public string ShipmentNumber;
		[DataMember]
		public string ShipperID;
		[DataMember]
		public string ShipperCode;
		[DataMember]
		public string OwnerID;
		[DataMember]
		public string OwnerCode;
		[DataMember]
		public string ManagerID;
		[DataMember]
		public string ManagerCode;
		[DataMember]
		public string CarrierID;
		[DataMember]
		public string CarrierCode;
		[DataMember]
		public int? CarrierIndex;
		[DataMember]
		public string ConjoinTransID;
		[DataMember]
		public string ReversedTransID;
		[DataMember]
		public string LinkedDocumentNumber;
		[DataMember]
		public string ReversalType;
		[DataMember]
		public string PONumber;
		[DataMember]
		public string TimeIn;
		[DataMember]
		public string TimeOut;
		[DataMember]
		public string TimeEnd;
		[DataMember]
		public string RoutingID;
		[DataMember]
		public string TicketSource;
		[DataMember]
		public string LoadID;
		[DataMember]
		public string BillToID;
		[DataMember]
		public string BillToCode;
		[DataMember]
		public string DriverIdentificationNumber;
		[DataMember]
		public double? CreditAmount;
		[DataMember]
		public string CardExpiration;
		[DataMember]
		public string CardName;
		[DataMember]
		public string CardType;
		[DataMember]
		public double? CashAmount;
		[DataMember]
		public string RouteOriginationDate;
		[DataMember]
		public bool? InternationalRouteIndicator;
		[DataMember]
		public string PreviousRoutingID;
		[DataMember]
		public string ShippingDocumentNumber;
		[DataMember]
		public string DocumentNumber;
		[DataMember]
		public string STD;
		[DataMember]
		public string ETD;
		[DataMember]
		public string STA;
		[DataMember]
		public string ETA;
		[DataMember]
		public string SFT;
		[DataMember]
		public string FST;
		[DataMember]
		public int? EstimatedFuelingDuration;
		[DataMember]
		public bool? DeleteFlag;
		[DataMember]
		public string TicketMode;
		[DataMember]
		public string DestinationRegistrationID1;
		[DataMember]
		public string DestinationSerialNumber1;
		[DataMember]
		public string DestinationEquipmentType1;
		[DataMember]
		public string DestinationEquipmentModel1;
		[DataMember]
		public string DestinationCompanyEquipmentID1;
		[DataMember]
		public string DestinationRegistrationID2;
		[DataMember]
		public string DestinationSerialNumber2;
		[DataMember]
		public string DestinationEquipmentType2;
		[DataMember]
		public string DestinationEquipmentModel2;
		[DataMember]
		public string DestinationCompanyEquipmentID2;
		[DataMember]
		public string DestinationRegistrationID3;
		[DataMember]
		public string DestinationSerialNumber3;
		[DataMember]
		public string DestinationEquipmentType3;
		[DataMember]
		public string DestinationEquipmentModel3;
		[DataMember]
		public string DestinationCompanyEquipmentID3;
		[DataMember]
		public string SourceRegistrationID1;
		[DataMember]
		public string SourceSerialNumber1;
		[DataMember]
		public string SourceEquipmentType1;
		[DataMember]
		public string SourceEquipmentModel1;
		[DataMember]
		public string SourceCompanyEquipmentID1;
		[DataMember]
		public string SourceRegistrationID2;
		[DataMember]
		public string SourceSerialNumber2;
		[DataMember]
		public string SourceEquipmentType2;
		[DataMember]
		public string SourceEquipmentModel2;
		[DataMember]
		public string SourceCompanyEquipmentID2;
		[DataMember]
		public string SourceRegistrationID3;
		[DataMember]
		public string SourceSerialNumber3;
		[DataMember]
		public string SourceEquipmentType3;
		[DataMember]
		public string SourceEquipmentModel3;
		[DataMember]
		public string SourceCompanyEquipmentID3;
		[DataMember]
		public string OperatorID;
		[DataMember]
		public string EffectiveDate;
		[DataMember]
		public string ExpirationDate;
		[DataMember]
		public string ScheduledDate;
		[DataMember]
		public bool? AutoComplete;
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
		public string ContactFirstName;
		[DataMember]
		public string ContactSurname;
		[DataMember]
		public string Date01;
		[DataMember]
		public string Date02;
		[DataMember]
		public string Date03;
		[DataMember]
		public string Date04;
		[DataMember]
		public string LegacyNumber;
		[DataMember]
		public string Country;
		[DataMember]
		public string ContactInfo;
		[DataMember]
		public string AssociatedDocNumber;
		[DataMember]
		public string AssociatedCLIN;
		[DataMember]
		public bool? SubmittedToAccounting;
		[DataMember]
		public string FuelCardID;
		[DataMember]
		public string AssociatedTransportOrderNumber;
		[DataMember]
		public string RequestedDateTime;
		[DataMember]
		public string DispatchedDateTime;
		[DataMember]
		public bool? ErrorFlag;
		[DataMember]
		public long _RowVersion;
		[DataMember]
		public Guid TransactionGuid;
		[DataMember]
		public Guid? SiteGuid;
		[DataMember]
		public short? LookupTransTypeIndex;
		[DataMember]
		public int? LookupTransactionStatusIndex;
		[DataMember]
		public int? LookupOriginApplicationIndex;
		[DataMember]
		public Guid? TransactionAliasGuid;
		[DataMember]
		public Guid? BillToCompanyGuid;
		[DataMember]
		public Guid? Destination1EquipmentGuid;
		[DataMember]
		public Guid? Destination2EquipmentGuid;
		[DataMember]
		public Guid? Destination3EquipmentGuid;
		[DataMember]
		public Guid? FinalStationIATAGuid;
		[DataMember]
		public Guid? FuelCardGuid;
		[DataMember]
		public Guid? ManagerCompanyGuid;
		[DataMember]
		public Guid? NextStationIATAGuid;
		[DataMember]
		public Guid? OperatorPersonnelGuid;
		[DataMember]
		public Guid? OriginStationIATAGuid;
		[DataMember]
		public Guid? OwnerCompanyGuid;
		[DataMember]
		public Guid? PreviousStationIATAGuid;
		[DataMember]
		public Guid? ShipperCompanyGuid;
		[DataMember]
		public Guid? ShipToCompanyGuid;
		[DataMember]
		public Guid? Source1EquipmentGuid;
		[DataMember]
		public Guid? Source2EquipmentGuid;
		[DataMember]
		public Guid? Source3EquipmentGuid;
		[DataMember]
		public Guid? SupplierCompanyGuid;
		[DataMember]
		public Guid? CarrierCompanyGuid;
	    [DataMember]
	    public string ShippingMethod;
	    [DataMember]
	    public Guid? ReasonCodeGuid;
        [DataMember]
		public string OriginStationIATAID;
		[DataMember]
		public string PreviousStationIATAID;
		[DataMember]
		public string NextStationIATAID;
		[DataMember]
		public string FinalStationIATAID;

		protected static string getDateTime(object o)
		{
			if (o == System.DBNull.Value)
			{
				return null;
			}
			return (DataObject.getValue<DateTime>(o, TimeConverter.MinFMDate.Date)).ToString("yyyy-MM-dd");
		}


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
				this.TransID =getString(row["TransID"]);
				this.AliasName = getString(row["AliasName"]);
				this.SubType = getString(row["SubType"]);
				this.Site = getString(row["Site"]);
				this.TransReferenceID = getString(row["TransReferenceID"]);
				this.InventoryDate = getDateTime(row["InventoryDate"]);
				this.ShipToID = getString(row["ShipToID"]);
				this.ShipToCode = getString(row["ShipToCode"]);
				this.SupplierID = getString(row["SupplierID"]);
				this.SupplierCode = getString(row["SupplierCode"]);
				this.CreatedDate = getDateTimeOffset(row["CreatedDate"]);
				this.CreatedBy = getString(row["CreatedBy"]);
				this.RequestedDeliveryDate = getDateTimeOffset(row["RequestedDeliveryDate"]);
				this.UpdatedDate = getDateTimeOffset(row["UpdatedDate"]);
				this.UpdatedBy = getString(row["UpdatedBy"]);
				this.TransDateTime = getDateTimeOffset(row["TransDateTime"]);
				this.TransVersion = getLong(row["TransVersion"]);
				this.SCACCode = getString(row["SCACCode"]);
				this.CardNumber = getString(row["CardNumber"]);
				this.ShipmentNumber = getString(row["ShipmentNumber"]);
				this.ShipperID = getString(row["ShipperID"]);
				this.ShipperCode = getString(row["ShipperCode"]);
				this.OwnerID = getString(row["OwnerID"]);
				this.OwnerCode = getString(row["OwnerCode"]);
				this.ManagerID = getString(row["ManagerID"]);
				this.ManagerCode = getString(row["ManagerCode"]);
				this.CarrierID = getString(row["CarrierID"]);
				this.CarrierCode = getString(row["CarrierCode"]);
				this.CarrierIndex = getInt(row["CarrierIndex"]);
				this.ConjoinTransID = getString(row["ConjoinTransID"]);
				this.ReversedTransID = getString(row["ReversedTransID"]);
				this.LinkedDocumentNumber = getString(row["LinkedDocumentNumber"]);
				this.ReversalType = getString(row["ReversalType"]);
				this.PONumber = getString(row["PONumber"]);
				this.TimeIn = getDateTimeOffset(row["TimeIn"]);
				this.TimeOut = getDateTimeOffset(row["TimeOut"]);
				this.TimeEnd = getDateTimeOffset(row["TimeEnd"]);
				this.RoutingID = getString(row["RoutingID"]);
				this.TicketSource = getString(row["TicketSource"]);
				this.LoadID = getString(row["LoadID"]);
				this.BillToID = getString(row["BillToID"]);
				this.BillToCode = getString(row["BillToCode"]);
				this.DriverIdentificationNumber = getString(row["DriverIdentificationNumber"]);
				this.CreditAmount = getDouble(row["CreditAmount"]);
				this.CardExpiration = getDateTimeOffset(row["CardExpiration"]);
				this.CardName = getString(row["CardName"]);
				this.CardType = getString(row["CardType"]);
				this.CashAmount = getDouble(row["CashAmount"]);
				this.RouteOriginationDate = row.IsNull("RouteOriginationDate") ? TimeConverter.MinFMDate.ToString() : getDateTimeOffset(row["RouteOriginationDate"]);
				this.InternationalRouteIndicator = getBool(row["InternationalRouteIndicator"]);;
				this.PreviousRoutingID = getString(row["PreviousRoutingID"]);
				this.ShippingDocumentNumber = getString(row["ShippingDocumentNumber"]);
				this.DocumentNumber = getString(row["DocumentNumber"]);
				this.STD = getDateTimeOffset(row["STD"]);
				this.ETD = getDateTimeOffset(row["ETD"]);
				this.STA = getDateTimeOffset(row["STA"]);
				this.ETA = getDateTimeOffset(row["ETA"]);
				this.SFT = getDateTimeOffset(row["SFT"]);
				this.FST = getDateTimeOffset(row["FST"]);
				this.EstimatedFuelingDuration = getInt(row["EstimatedFuelingDuration"]);
				this.DeleteFlag = getBool(row["DeleteFlag"]);
				this.TicketMode = getString(row["TicketMode"]);
				this.DestinationRegistrationID1 = getString(row["DestinationRegistrationID1"]);
				this.DestinationSerialNumber1 = getString(row["DestinationSerialNumber1"]);
				this.DestinationEquipmentType1 = getString(row["DestinationEquipmentType1"]);
				this.DestinationEquipmentModel1 = getString(row["DestinationEquipmentModel1"]);
				this.DestinationCompanyEquipmentID1 = getString(row["DestinationCompanyEquipmentID1"]);
				this.DestinationRegistrationID2 = getString(row["DestinationRegistrationID2"]);
				this.DestinationSerialNumber2 = getString(row["DestinationSerialNumber2"]);
				this.DestinationEquipmentType2 = getString(row["DestinationEquipmentType2"]);
				this.DestinationEquipmentModel2 = getString(row["DestinationEquipmentModel2"]);
				this.DestinationCompanyEquipmentID2 = getString(row["DestinationCompanyEquipmentID2"]);
				this.DestinationRegistrationID3 = getString(row["DestinationRegistrationID3"]);
				this.DestinationSerialNumber3 = getString(row["DestinationSerialNumber3"]);
				this.DestinationEquipmentType3 = getString(row["DestinationEquipmentType3"]);
				this.DestinationEquipmentModel3 = getString(row["DestinationEquipmentModel3"]);
				this.DestinationCompanyEquipmentID3 = getString(row["DestinationCompanyEquipmentID3"]);
				this.SourceRegistrationID1 = getString(row["SourceRegistrationID1"]);
				this.SourceSerialNumber1 = getString(row["SourceSerialNumber1"]);
				this.SourceEquipmentType1 = getString(row["SourceEquipmentType1"]);
				this.SourceEquipmentModel1 = getString(row["SourceEquipmentModel1"]);
				this.SourceCompanyEquipmentID1 = getString(row["SourceCompanyEquipmentID1"]);
				this.SourceRegistrationID2 = getString(row["SourceRegistrationID2"]);
				this.SourceSerialNumber2 = getString(row["SourceSerialNumber2"]);
				this.SourceEquipmentType2 = getString(row["SourceEquipmentType2"]);
				this.SourceEquipmentModel2 = getString(row["SourceEquipmentModel2"]);
				this.SourceCompanyEquipmentID2 = getString(row["SourceCompanyEquipmentID2"]);
				this.SourceRegistrationID3 = getString(row["SourceRegistrationID3"]);
				this.SourceSerialNumber3 = getString(row["SourceSerialNumber3"]);
				this.SourceEquipmentType3 = getString(row["SourceEquipmentType3"]);
				this.SourceEquipmentModel3 = getString(row["SourceEquipmentModel3"]);
				this.SourceCompanyEquipmentID3 = getString(row["SourceCompanyEquipmentID3"]);
				this.OperatorID = getString(row["OperatorID"]);
				this.EffectiveDate = getDateTimeOffset(row["EffectiveDate"]);
				this.ExpirationDate = getDateTimeOffset(row["ExpirationDate"]);
				this.ScheduledDate = getDateTimeOffset(row["ScheduledDate"]);
				this.AutoComplete = getBool(row["AutoComplete"]);
				this.Flag01 = getBool(row["Flag01"]);
				this.Flag02 = getBool(row["Flag02"]);
				this.Flag03 = getBool(row["Flag03"]);
				this.Flag04 = getBool(row["Flag04"]);
				this.Flag05 = getBool(row["Flag05"]);
				this.Flag06 = getBool(row["Flag06"]);
				this.Number01 = getDouble(row["Number01"]);
				this.Number02 = getDouble(row["Number02"]);
				this.Number03 = getDouble(row["Number03"]);
				this.Number04 = getDouble(row["Number04"]);
				this.Number05 = getDouble(row["Number05"]);
				this.Number06 = getDouble(row["Number06"]);
				this.ContactFirstName = getString(row["ContactFirstName"]);
				this.ContactSurname = getString(row["ContactSurname"]);
				this.Date01 = getDateTimeOffset(row["Date01"]);
				this.Date02 = getDateTimeOffset(row["Date02"]);
				this.Date03 = getDateTimeOffset(row["Date03"]);
				this.Date04 = getDateTimeOffset(row["Date04"]);
				this.LegacyNumber = getString(row["LegacyNumber"]);
				this.Country = getString(row["Country"]);
				this.ContactInfo = getString(row["ContactInfo"]);
				this.AssociatedDocNumber = getString(row["AssociatedDocNumber"]);
				this.AssociatedCLIN = getString(row["AssociatedCLIN"]);
				this.SubmittedToAccounting = getBool(row["SubmittedToAccounting"]);
				this.FuelCardID = getString(row["FuelCardID"]);
				this.AssociatedTransportOrderNumber = getString(row["AssociatedTransportOrderNumber"]);
				this.RequestedDateTime = getDateTimeOffset(row["RequestedDateTime"]);
				this.DispatchedDateTime = getDateTimeOffset(row["DispatchedDateTime"]);
				this.ErrorFlag = getBool(row["ErrorFlag"]);
				this._RowVersion = (long)(row["_RowVersion"]);
				this.TransactionGuid = (Guid)(row["TransactionGuid"]);
				this.SiteGuid = getGuid(row["SiteGuid"]);
				this.LookupTransTypeIndex = getShort(row["LookupTransTypeIndex"]);
				this.LookupTransactionStatusIndex = getInt(row["LookupTransactionStatusIndex"]);
				this.LookupOriginApplicationIndex = getInt(row["LookupOriginApplicationIndex"]);
				this.TransactionAliasGuid = getGuid(row["TransactionAliasGuid"]);
				this.BillToCompanyGuid = getGuid(row["BillToCompanyGuid"]);
				this.Destination1EquipmentGuid = getGuid(row["Destination1EquipmentGuid"]);
				this.Destination2EquipmentGuid = getGuid(row["Destination2EquipmentGuid"]);
				this.Destination3EquipmentGuid = getGuid(row["Destination3EquipmentGuid"]);
				this.FinalStationIATAGuid = getGuid(row["FinalStationIATAGuid"]);
				this.FuelCardGuid = getGuid(row["FuelCardGuid"]);
				this.ManagerCompanyGuid = getGuid(row["ManagerCompanyGuid"]);
				this.NextStationIATAGuid = getGuid(row["NextStationIATAGuid"]);
				this.OperatorPersonnelGuid = getGuid(row["OperatorPersonnelGuid"]);
				this.OriginStationIATAGuid = getGuid(row["OriginStationIATAGuid"]);
				this.OwnerCompanyGuid = getGuid(row["OwnerCompanyGuid"]);
				this.PreviousStationIATAGuid = getGuid(row["PreviousStationIATAGuid"]);
				this.ShipperCompanyGuid = getGuid(row["ShipperCompanyGuid"]);
				this.ShipToCompanyGuid = getGuid(row["ShipToCompanyGuid"]);
				this.Source1EquipmentGuid = getGuid(row["Source1EquipmentGuid"]);
				this.Source2EquipmentGuid = getGuid(row["Source2EquipmentGuid"]);
				this.Source3EquipmentGuid = getGuid(row["Source3EquipmentGuid"]);
				this.SupplierCompanyGuid = getGuid(row["SupplierCompanyGuid"]);
				this.CarrierCompanyGuid = getGuid(row["CarrierCompanyGuid"]);
			    this.ShippingMethod = getString(row["ShippingMethod"]);
				this.ReasonCodeGuid = getGuid(row["ReasonCodeGuid"]);
				this.OriginStationIATAID = getString(row["OriginStationIATAID"]);
				this.PreviousStationIATAID = getString(row["PreviousStationIATAID"]);
				this.NextStationIATAID = getString(row["NextStationIATAID"]);
				this.FinalStationIATAID = getString(row["FinalStationIATAID"]);
			}
		}
	}

	[Serializable]
	[CollectionDataContract]
	public class TransactionSelectionCollectionDO : List<TransactionSelectionDO> 
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
			sqlcommand.CommandText = "exec usp_MobileTransactionSelectionSelectBy_TimeWindow_Vehicle_Gate 	@OperatorID, @filterOperatorID, @VehicleID," +
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
						TransactionSelectionDO trans = new TransactionSelectionDO();
						trans.LoadRow(row);
						Add(trans);
					}
				}
			}
		}
	}

}

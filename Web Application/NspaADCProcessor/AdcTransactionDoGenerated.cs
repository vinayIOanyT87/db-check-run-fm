/*
 <copyright file="AdcTransactionDoGenerated.cs" company="Varec, Inc.">
   Copyright (c) Varec, Inc.  All rights reserved.
 </copyright>
 <summary>
	 Generated AdcTransactionDoGenerated class on '12/17/14 8:55:22 AM'.  
	 ***** PLEASE DON'T UPDATE MANUALLY *****
 </summary>
*/
namespace Nspa
{
	using System;
	using System.Xml.Serialization;

	public class AdcTransactionDoGenerated
	{
		public const string DataVersion = "9.0.0.1";

		public string TransID { get; set; }

		public string AliasName { get; set; }

		[XmlElement(IsNullable = true)]
		public int? LookupTransactionStatusIndex { get; set; }

		[XmlElement(IsNullable = true)]
		public DateTime? UpdatedDate { get; set; }

		[XmlElement(IsNullable = true)]
		public DateTime? InventoryDate { get; set; }

		public string AssociatedDocNumber { get; set; }

		public string BillToCode { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? BillToCompanyGuid { get; set; }

		public string BillToID { get; set; }

		[XmlElement(IsNullable = true)]
		public DateTime? CardExpiration { get; set; }

		public string CardNumber { get; set; }

		public string CardType { get; set; }

		public string CarrierCode { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? CarrierCompanyGuid { get; set; }

		public string CarrierID { get; set; }

		[XmlElement(IsNullable = true)]
		public DateTime? CreatedDate { get; set; }

		[XmlElement(IsNullable = true)]
		public DateTime? Date01 { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? Destination1EquipmentGuid { get; set; }

		public string DestinationEquipmentType1 { get; set; }

		public string DestinationRegistrationID1 { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? FinalStationIATAGuid { get; set; }

		public string FinalStationIATAID { get; set; }

		[XmlElement(IsNullable = true)]
		public bool? Flag01 { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? FuelCardGuid { get; set; }

		public string FuelCardID { get; set; }

		public string GateID { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? GateGuid { get; set; }

		public string ManagerCode { get; set; }

		[XmlElement(IsNullable = true)]
		public double? Number02 { get; set; }

		public string OperatorID { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? OperatorPersonnelGuid { get; set; }

		public string OwnerCode { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? OwnerCompanyGuid { get; set; }

		public string OwnerID { get; set; }

		public string PONumber { get; set; }

		public string ShipperCode { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? ShipperCompanyGuid { get; set; }

		public string ShipperID { get; set; }

		public string ShippingDocumentNumber { get; set; }

		public string ShipToCode { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? ShipToCompanyGuid { get; set; }

		public string ShipToID { get; set; }

		public string Site { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? SiteGuid { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? Source1EquipmentGuid { get; set; }

		public string SourceEquipmentType1 { get; set; }

		public string SourceRegistrationID1 { get; set; }

		public string SupplierCode { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? SupplierCompanyGuid { get; set; }

		public string SupplierID { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? TransactionAliasGuid { get; set; }

		[XmlElement(IsNullable = true)]
		public DateTime? TransDateTime { get; set; }

		public string UserData1 { get; set; }

		public string UserData2 { get; set; }

		public string UserData4 { get; set; }

		public string UserData5 { get; set; }

		public string UserData7 { get; set; }

		public string UserData8 { get; set; }

		public string UserData9 { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? BillToCompanyTypeGuid { get; set; }

		[XmlElement(IsNullable = true)]
		public double? Line_Density { get; set; }

		public string Line_DocumentNumber { get; set; }

		[XmlElement(IsNullable = true)]
		public int? Line_EngineeringUnitsIndex { get; set; }

		[XmlElement(IsNullable = true)]
		public double? Line_GrossQuantity { get; set; }

		public string Line_LoadingLocationID { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? Line_LoadingLocationStationGuid { get; set; }

		[XmlElement(IsNullable = true)]
		public double? Line_MassQuantity { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? Line_MeterGuid { get; set; }

		public string Line_MeterID { get; set; }

		[XmlElement(IsNullable = true)]
		public double? Line_MeterStart { get; set; }

		[XmlElement(IsNullable = true)]
		public DateTime? Line_MeterStartDateTime { get; set; }

		[XmlElement(IsNullable = true)]
		public double? Line_MeterStop { get; set; }

		[XmlElement(IsNullable = true)]
		public DateTime? Line_MeterStopDateTime { get; set; }

		[XmlElement(IsNullable = true)]
		public double? Line_NetQuantity { get; set; }

		public string Line_Product { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? Line_ProductGuid { get; set; }

		public string Line_StorageLocationID { get; set; }

		[XmlElement(IsNullable = true)]
		public Guid? Line_StorageLocationTankGuid { get; set; }

		[XmlElement(IsNullable = true)]
		public double? Line_Temperature { get; set; }

		[XmlElement(IsNullable = true)]
		public double? Line_Vcf { get; set; }

		[XmlElement(IsNullable = true)]
		public int? Line_DensityUnitsIndex { get; set; }

		[XmlElement(IsNullable = true)]
		public int? Line_TemperatureUnitsIndex { get; set; }

		[XmlElement(IsNullable = true)]
		public int? Line_MassUnitsIndex { get; set; }

		public string Note_Notes { get; set; }



		public override string ToString()
		{
			string retValue = string.Empty;
			retValue += string.Format("TransID : {0}", ((object)TransID) ?? "");

			retValue += string.Format("AliasName : {0}", ((object)AliasName) ?? "");

			retValue += string.Format("LookupTransactionStatusIndex : {0}", ((object)LookupTransactionStatusIndex) ?? "");

			retValue += string.Format("UpdatedDate : {0}", ((object)UpdatedDate) ?? "");

			retValue += string.Format("InventoryDate : {0}", ((object)InventoryDate) ?? "");

			retValue += string.Format("AssociatedDocNumber : {0}", ((object)AssociatedDocNumber) ?? "");

			retValue += string.Format("BillToCode : {0}", ((object)BillToCode) ?? "");

			retValue += string.Format("BillToCompanyGuid : {0}", ((object)BillToCompanyGuid) ?? "");

			retValue += string.Format("BillToID : {0}", ((object)BillToID) ?? "");

			retValue += string.Format("CardExpiration : {0}", ((object)CardExpiration) ?? "");

			retValue += string.Format("CardNumber : {0}", ((object)CardNumber) ?? "");

			retValue += string.Format("CardType : {0}", ((object)CardType) ?? "");

			retValue += string.Format("CarrierCode : {0}", ((object)CarrierCode) ?? "");

			retValue += string.Format("CarrierCompanyGuid : {0}", ((object)CarrierCompanyGuid) ?? "");

			retValue += string.Format("CarrierID : {0}", ((object)CarrierID) ?? "");

			retValue += string.Format("CreatedDate : {0}", ((object)CreatedDate) ?? "");

			retValue += string.Format("Date01 : {0}", ((object)Date01) ?? "");

			retValue += string.Format("Destination1EquipmentGuid : {0}", ((object)Destination1EquipmentGuid) ?? "");

			retValue += string.Format("DestinationEquipmentType1 : {0}", ((object)DestinationEquipmentType1) ?? "");

			retValue += string.Format("DestinationRegistrationID1 : {0}", ((object)DestinationRegistrationID1) ?? "");

			retValue += string.Format("FinalStationIATAGuid : {0}", ((object)FinalStationIATAGuid) ?? "");

			retValue += string.Format("FinalStationIATAID : {0}", ((object)FinalStationIATAID) ?? "");

			retValue += string.Format("Flag01 : {0}", ((object)Flag01) ?? "");

			retValue += string.Format("FuelCardGuid : {0}", ((object)FuelCardGuid) ?? "");

			retValue += string.Format("FuelCardID : {0}", ((object)FuelCardID) ?? "");

			retValue += string.Format("GateID : {0}", ((object)GateID) ?? "");

			retValue += string.Format("GateGuid : {0}", ((object)GateGuid) ?? "");

			retValue += string.Format("ManagerCode : {0}", ((object)ManagerCode) ?? "");

			retValue += string.Format("Number02 : {0}", ((object)Number02) ?? "");

			retValue += string.Format("OperatorID : {0}", ((object)OperatorID) ?? "");

			retValue += string.Format("OperatorPersonnelGuid : {0}", ((object)OperatorPersonnelGuid) ?? "");

			retValue += string.Format("OwnerCode : {0}", ((object)OwnerCode) ?? "");

			retValue += string.Format("OwnerCompanyGuid : {0}", ((object)OwnerCompanyGuid) ?? "");

			retValue += string.Format("OwnerID : {0}", ((object)OwnerID) ?? "");

			retValue += string.Format("PONumber : {0}", ((object)PONumber) ?? "");

			retValue += string.Format("ShipperCode : {0}", ((object)ShipperCode) ?? "");

			retValue += string.Format("ShipperCompanyGuid : {0}", ((object)ShipperCompanyGuid) ?? "");

			retValue += string.Format("ShipperID : {0}", ((object)ShipperID) ?? "");

			retValue += string.Format("ShippingDocumentNumber : {0}", ((object)ShippingDocumentNumber) ?? "");

			retValue += string.Format("ShipToCode : {0}", ((object)ShipToCode) ?? "");

			retValue += string.Format("ShipToCompanyGuid : {0}", ((object)ShipToCompanyGuid) ?? "");

			retValue += string.Format("ShipToID : {0}", ((object)ShipToID) ?? "");

			retValue += string.Format("Site : {0}", ((object)Site) ?? "");

			retValue += string.Format("SiteGuid : {0}", ((object)SiteGuid) ?? "");

			retValue += string.Format("Source1EquipmentGuid : {0}", ((object)Source1EquipmentGuid) ?? "");

			retValue += string.Format("SourceEquipmentType1 : {0}", ((object)SourceEquipmentType1) ?? "");

			retValue += string.Format("SourceRegistrationID1 : {0}", ((object)SourceRegistrationID1) ?? "");

			retValue += string.Format("SupplierCode : {0}", ((object)SupplierCode) ?? "");

			retValue += string.Format("SupplierCompanyGuid : {0}", ((object)SupplierCompanyGuid) ?? "");

			retValue += string.Format("SupplierID : {0}", ((object)SupplierID) ?? "");

			retValue += string.Format("TransactionAliasGuid : {0}", ((object)TransactionAliasGuid) ?? "");

			retValue += string.Format("TransDateTime : {0}", ((object)TransDateTime) ?? "");

			retValue += string.Format("UserData1 : {0}", ((object)UserData1) ?? "");

			retValue += string.Format("UserData2 : {0}", ((object)UserData2) ?? "");

			retValue += string.Format("UserData4 : {0}", ((object)UserData4) ?? "");

			retValue += string.Format("UserData5 : {0}", ((object)UserData5) ?? "");

			retValue += string.Format("UserData7 : {0}", ((object)UserData7) ?? "");

			retValue += string.Format("UserData8 : {0}", ((object)UserData8) ?? "");

			retValue += string.Format("UserData9 : {0}", ((object)UserData9) ?? "");

			retValue += string.Format("BillToCompanyTypeGuid : {0}", ((object)BillToCompanyTypeGuid) ?? "");

			retValue += string.Format("Line_Density : {0}", ((object)Line_Density) ?? "");

			retValue += string.Format("Line_DocumentNumber : {0}", ((object)Line_DocumentNumber) ?? "");

			retValue += string.Format("Line_EngineeringUnitsIndex : {0}", ((object)Line_EngineeringUnitsIndex) ?? "");

			retValue += string.Format("Line_GrossQuantity : {0}", ((object)Line_GrossQuantity) ?? "");

			retValue += string.Format("Line_LoadingLocationID : {0}", ((object)Line_LoadingLocationID) ?? "");

			retValue += string.Format("Line_LoadingLocationStationGuid : {0}", ((object)Line_LoadingLocationStationGuid) ?? "");

			retValue += string.Format("Line_MassQuantity : {0}", ((object)Line_MassQuantity) ?? "");

			retValue += string.Format("Line_MeterGuid : {0}", ((object)Line_MeterGuid) ?? "");

			retValue += string.Format("Line_MeterID : {0}", ((object)Line_MeterID) ?? "");

			retValue += string.Format("Line_MeterStart : {0}", ((object)Line_MeterStart) ?? "");

			retValue += string.Format("Line_MeterStartDateTime : {0}", ((object)Line_MeterStartDateTime) ?? "");

			retValue += string.Format("Line_MeterStop : {0}", ((object)Line_MeterStop) ?? "");

			retValue += string.Format("Line_MeterStopDateTime : {0}", ((object)Line_MeterStopDateTime) ?? "");

			retValue += string.Format("Line_NetQuantity : {0}", ((object)Line_NetQuantity) ?? "");

			retValue += string.Format("Line_Product : {0}", ((object)Line_Product) ?? "");

			retValue += string.Format("Line_ProductGuid : {0}", ((object)Line_ProductGuid) ?? "");

			retValue += string.Format("Line_StorageLocationID : {0}", ((object)Line_StorageLocationID) ?? "");

			retValue += string.Format("Line_StorageLocationTankGuid : {0}", ((object)Line_StorageLocationTankGuid) ?? "");

			retValue += string.Format("Line_Temperature : {0}", ((object)Line_Temperature) ?? "");

			retValue += string.Format("Line_Vcf : {0}", ((object)Line_Vcf) ?? "");

			retValue += string.Format("Line_DensityUnitsIndex : {0}", ((object)Line_DensityUnitsIndex) ?? "");

			retValue += string.Format("Line_TemperatureUnitsIndex : {0}", ((object)Line_TemperatureUnitsIndex) ?? "");

			retValue += string.Format("Line_MassUnitsIndex : {0}", ((object)Line_MassUnitsIndex) ?? "");

			retValue += string.Format("Note_Notes : {0}", ((object)Note_Notes) ?? "");


			return retValue;
		}

	}
}


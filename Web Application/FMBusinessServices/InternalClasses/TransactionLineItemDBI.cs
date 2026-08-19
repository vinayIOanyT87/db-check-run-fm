// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionLineItemDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionLineItemDBI type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
	using System.Diagnostics;

	using FMBusinessObjects.DataObjects;
    using FMBusinessServices.DataAccessLayer;
    using Microsoft.SqlServer.Server;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Associates a line item with information from the transaction header. When saving a line item some of the
    /// header information is saved along with it.
    /// </summary>
    public class LineItemWithTransactionInformation
    {
        /// <summary>
        /// The line item
        /// </summary>
        public LineItemDO LineItem;

        /// <summary>
        /// The transaction guid identifying the transaction header the line item is associated with
        /// </summary>
        public Guid TransactionGuid;

        /// <summary>
        /// The inventory date of the transaction header the line item is associated with
        /// </summary>
        public DateTime InventoryDate;

        /// <summary>
        /// The TransVersion associated with the current version of the entire transaction record
        /// </summary>
        public long TransVersion;

        /// <summary>
        /// Is the transaction deleted?
        /// </summary>
        public bool DeleteFlag;
    }

    /// <summary>
    /// Writes transaction line item and line item user data information to the database
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TransactionLineItemDBI
    {
        /// <summary>
        /// Allows access to the database
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

        /// <summary>
        /// The user who inserted or modified the transaction
        /// </summary>
        private string User { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionLineItemDBI"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public TransactionLineItemDBI(string user)
        {
            this.User = user;
        }

        /// <summary>
        /// Insert or update the line items provided
        /// </summary>
        /// <param name="security">
        /// Contains Security information
        /// </param>
        /// <param name="lineItemsWithTransactionInformation">
        /// The line items to save along with the header information needed when saving line items
        /// </param>
        public void Save(SecurityClass security, List<LineItemWithTransactionInformation> lineItemsWithTransactionInformation)
        {
            if (lineItemsWithTransactionInformation.Count == 0)
            {
                return;
            }

            using (SqlCommand insertUpdateCommand = new SqlCommand())
            {
                insertUpdateCommand.CommandType = CommandType.StoredProcedure;
                insertUpdateCommand.CommandText = "usp_TransactionLineItemsInsertUpdate";
                insertUpdateCommand.CommandTimeout = Math.Max(30, lineItemsWithTransactionInformation.Count);

                SqlParameter tableValuedParameter = insertUpdateCommand.Parameters.Add("@TransactionLineItems", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecords(lineItemsWithTransactionInformation, this.User);
                tableValuedParameter.TypeName = "dbo.TransactionLineItemsType";

                this.ConsolidatedDa.ExecuteQuery(security, insertUpdateCommand);
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert / update stored procedure
        /// </summary>
        /// <param name="lineItemsWithTransactionInformation">The line items and transaction information to create SqlDataRecords for</param>
        /// <param name="user">The user saving the line items</param>
        /// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert / update stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<LineItemWithTransactionInformation> lineItemsWithTransactionInformation, string user)
        {
            SqlMetaData[] metaData = new SqlMetaData[177];

            int i = 0;            
            metaData[i++] = new SqlMetaData("TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("SequenceID", SqlDbType.SmallInt);
            metaData[i++] = new SqlMetaData("MeterStart", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("MeterStop", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("GrossQuantity", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("DeliveredGrossQuantity", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("Temperature", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Vcf", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("Density", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Product", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("ProductCode", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("ProductType", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("ProductPrice", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("CLIN", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("NetQuantity", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("DeliveredNetQuantity", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("Pressure", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("ContractNumber", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("DestinationRegistrationID", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("DestinationSerialNumber", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("DestinationEquipmentType", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("DestinationEquipmentModel", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("DestinationCompanyEquipmentID", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("DestinationCompartmentID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("SourceRegistrationID", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("SourceSerialNumber", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("SourceEquipmentType", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("SourceEquipmentModel", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("SourceCompanyEquipmentID", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("SourceCompartmentID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("MeterFactor", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("BatchNumber", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("DocumentNumber", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("LineFill", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("BottomVolume", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("NetCapacity", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Customs", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("ArmNumber", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("LineNumber", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("OperatorID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("TankStatus", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("MeterStartDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("MeterStopDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("Pit", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("RequestedDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("DispatchedDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("AcknowledgedDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("OnLocationTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("ValidationDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("CompletionDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("ReceiptVariance", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("DifferentialPressure", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("LoadRackVariance", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("RequestedBy", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("FreezePoint", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("DeleteFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("StorageLocationID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("MeterID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("AdditiveProfileID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("PresetAmount", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("EngineeringUnitsIndex", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("CustomerProductName", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("CustomerProductCode", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("TransactionInventoryDate", SqlDbType.DateTime);
            metaData[i++] = new SqlMetaData("COAWaiver", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("COANote", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("COAID", SqlDbType.NVarChar, 40);
            metaData[i++] = new SqlMetaData("Tax1", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Tax2", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Tax3", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Tax4", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Tax5", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("TransVersion", SqlDbType.BigInt);
            metaData[i++] = new SqlMetaData("LoadingLocationID", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("ImproperAdditization", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("BrokenBlend", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("ContaminatePrompt", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("CompartmentsPreviouslyLoaded", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("CompartmentsEmpty", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("Flag01", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("Flag02", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("Flag03", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("Flag04", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("Flag05", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("Flag06", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("Number01", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Number02", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Number03", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Number04", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Number05", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Number06", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("OdometerHours", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("EndDeliveryDate", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("RequestedDeliveryDate", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("InvoiceNumber", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("InvoiceLineNumber", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("AlternativeGrossVolume", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("AlternativeNetVolume", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("AlternativeUnits", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("TankLevel", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("TankLevelUnits", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("Date01", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("Date02", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("Date03", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("Date04", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("NonDomesticPrice", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("ExchangeRate", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("QualityTestNumber", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("Odometer", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("DeliveryLocation", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("Variance", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("PartialFill", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("MassQuantity", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("NetManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("GrossManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("MassManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("VcfManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("DeliveredGrossManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("DeliveredNetManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("LookupTransactionStatusIndex", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("LookupQualityIndex", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("StorageLocationTankGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("AdditiveProfileGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("DestinationCompartmentEquipmentGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("DestinationEquipmentGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("OperatorPersonnelGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("ProductGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("SourceCompartmentEquipmentGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("SourceEquipmentGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("CurrencyGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("OrderReferenceTransactionLineItemGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("LoadingLocationStationGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("MeterGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("PackageManualValueFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("CleanLineItem", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("CleanLineDeductItem", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("CleanLineDeductQuantity", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("CleanLinePackQuantity", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("DualFuelingModeFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("DualFuelingPrimaryFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("EngineRunTime", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("FlowRate", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("FuelCompressionFactor", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("HydrantPressure", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("MobileDeviceID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("MobileDeviceGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("TemperatureQualityStatus", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("MeterStartObtainedAutomaticallyFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("MeterStopObtainedAutomaticallyFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("NetVolumeIndicator", SqlDbType.Bit);

            // Line item User Data fields
            metaData[i++] = new SqlMetaData("TransactionLineItemUserDataGuid", SqlDbType.UniqueIdentifier);

            // There are 24 columns containing user data.  The columns are named
            // "UserDataX" where X is a number between 1 and 24
            for (int userDataIndex = 1; userDataIndex <= 24; userDataIndex++)
            {
                metaData[i++] = new SqlMetaData("@UserData" + userDataIndex, SqlDbType.NVarChar, 60);
            }

            // Fields common to all records
            metaData[i] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (LineItemWithTransactionInformation lineItemWithTransactionInformation in lineItemsWithTransactionInformation)
            {
                int j = 0;
                LineItemDO lineItem = lineItemWithTransactionInformation.LineItem;
                
					record.SetGuid(j++, lineItem.TransactionLineItemGuid);
					record.SetGuid(j++, lineItemWithTransactionInformation.TransactionGuid);
					record.SetInt16(j++, (short)lineItem.SequenceId.GetValueOrDefault());
					record.SetNullableDouble(j++, lineItem.MeterReading.MeterStart);
					record.SetNullableDouble(j++, lineItem.MeterReading.MeterStop);
					record.SetDouble(j++, lineItem.Quantity.GrossInventoryChange);
					record.SetDouble(j++, lineItem.Quantity.DeliveredGrossInventoryChange);
					record.SetNullableDouble(j++, lineItem.Temperature);
					record.SetNullableDouble(j++, lineItem.VCF);
					record.SetNullableDouble(j++, lineItem.Density);
					record.SetNullableString(j++, lineItem.Product);
					record.SetNullableString(j++, lineItem.ProductCode);
					record.SetNullableString(j++, lineItem.ProductType);
					record.SetNullableDouble(j++, lineItem.ProductPrice);
					record.SetNullableString(j++, lineItem.CLIN);
					record.SetDouble(j++, lineItem.Quantity.NetInventoryChange);
					record.SetDouble(j++, lineItem.Quantity.DeliveredNetInventoryChange);
					record.SetNullableDouble(j++, lineItem.Pressure);
					record.SetNullableString(j++, lineItem.ContractNumber);
					record.SetNullableString(j++, lineItem.DestinationEQ.RegistrationID);
					record.SetNullableString(j++, lineItem.DestinationEQ.SerialNumber);
					record.SetNullableString(j++, lineItem.DestinationEQ.EquipmentType);
					record.SetNullableString(j++, lineItem.DestinationEQ.EquipmentModel);
					record.SetNullableString(j++, lineItem.DestinationEQ.CompanyEquipmentID);
					record.SetNullableString(j++, lineItem.DestinationCompartmentID);
					record.SetNullableString(j++, lineItem.SourceEQ.RegistrationID);
					record.SetNullableString(j++, lineItem.SourceEQ.SerialNumber);
					record.SetNullableString(j++, lineItem.SourceEQ.EquipmentType);
					record.SetNullableString(j++, lineItem.SourceEQ.EquipmentModel);
					record.SetNullableString(j++, lineItem.SourceEQ.CompanyEquipmentID);
					record.SetNullableString(j++, lineItem.SourceCompartmentID);
					record.SetNullableDouble(j++, lineItem.MeterReading.MeterFactor);
					record.SetNullableString(j++, lineItem.BatchNumber);
					record.SetNullableString(j++, lineItem.DocumentNumber);
					record.SetNullableDouble(j++, lineItem.LineFill);
					record.SetNullableDouble(j++, lineItem.BottomVolume);
					record.SetNullableDouble(j++, lineItem.NetCapacity);
					record.SetNullableString(j++, lineItem.Customs);
					record.SetNullableInt(j++, lineItem.ArmNumber);
					record.SetNullableInt(j++, lineItem.LineNumber);
					record.SetNullableString(j++, lineItem.OperatorID);
					record.SetNullableString(j++, lineItem.TankStatus);
					record.SetNullableDateTimeOffset(j++, lineItem.MeterReading.StartDateTime);
					record.SetNullableDateTimeOffset(j++, lineItem.MeterReading.StopDateTime);
					record.SetNullableString(j++, lineItem.Pit);
					record.SetNullableDateTimeOffset(j++, lineItem.RequestedDateTime);
					record.SetNullableDateTimeOffset(j++, lineItem.DispatchedDateTime);
					record.SetNullableDateTimeOffset(j++, lineItem.AcknowledgedDateTime);
					record.SetNullableDateTimeOffset(j++, lineItem.OnLocationTime);
					record.SetNullableDateTimeOffset(j++, lineItem.ValidationDateTime);
					record.SetNullableDateTimeOffset(j++, lineItem.CompletionDateTime);
					record.SetNullableDouble(j++, lineItem.ReceiptVariance);
					record.SetNullableDouble(j++, lineItem.DifferentialPressure);
					record.SetNullableDouble(j++, lineItem.LoadRackVariance);
					record.SetNullableString(j++, lineItem.RequestedBy);
					record.SetNullableDouble(j++, lineItem.FreezePoint);
					record.SetBoolean(j++, lineItem.DeleteFlag);
					record.SetNullableString(j++, lineItem.StorageLocationID);
					record.SetNullableString(j++, lineItem.MeterID);
					record.SetNullableString(j++, lineItem.AdditiveProfileID);
					record.SetNullableDouble(j++, lineItem.PresetAmount);
					record.SetInt32(j++, (int)lineItem.EngineeringUnitsIndex);
					record.SetNullableString(j++, lineItem.CustomerProductName);
					record.SetNullableString(j++, lineItem.CustomerProductCode);
					record.SetDateTime(j++, lineItemWithTransactionInformation.InventoryDate);
					record.SetBoolean(j++, lineItem.COAWaiver);
					record.SetNullableString(j++, lineItem.COANote);
					record.SetNullableString(j++, lineItem.COAID);
					record.SetNullableDouble(j++, lineItem.Tax1);
					record.SetNullableDouble(j++, lineItem.Tax2);
					record.SetNullableDouble(j++, lineItem.Tax3);
					record.SetNullableDouble(j++, lineItem.Tax4);
					record.SetNullableDouble(j++, lineItem.Tax5);
					record.SetInt64(j++, lineItemWithTransactionInformation.TransVersion);
					record.SetNullableString(j++, lineItem.LoadingLocationID);
					record.SetNullableBoolean(j++, lineItem.ImproperAdditization);
					record.SetNullableBoolean(j++, lineItem.BrokenBlend);
					record.SetNullableBoolean(j++, lineItem.ContaminatePrompt);
					record.SetNullableBoolean(j++, lineItem.CompartmentsPreviouslyLoaded);
					record.SetNullableBoolean(j++, lineItem.CompartmentsEmpty);
					record.SetBoolean(j++, lineItem.Flag01);
					record.SetBoolean(j++, lineItem.Flag02);
					record.SetBoolean(j++, lineItem.Flag03);
					record.SetBoolean(j++, lineItem.Flag04);
					record.SetBoolean(j++, lineItem.Flag05);
					record.SetBoolean(j++, lineItem.Flag06);
					record.SetNullableDouble(j++, lineItem.Number01);
					record.SetNullableDouble(j++, lineItem.Number02);
					record.SetNullableDouble(j++, lineItem.Number03);
					record.SetNullableDouble(j++, lineItem.Number04);
					record.SetNullableDouble(j++, lineItem.Number05);
					record.SetNullableDouble(j++, lineItem.Number06);
					record.SetNullableDouble(j++, lineItem.OdometerHours);
					record.SetNullableDateTimeOffset(j++, lineItem.EndDeliveryDate);
					record.SetNullableDateTimeOffset(j++, lineItem.RequestedDeliveryDate);
					record.SetNullableString(j++, lineItem.InvoiceNumber);
					record.SetNullableString(j++, lineItem.InvoiceLineNumber);
					record.SetNullableDouble(j++, lineItem.AlternativeGrossVolume);
					record.SetNullableDouble(j++, lineItem.AlternativeNetVolume);
					record.SetNullableInt(j++, lineItem.AlternativeUnits);
					record.SetNullableDouble(j++, lineItem.TankLevel);
					record.SetNullableInt(j++, lineItem.TankLevelUnits);
					record.SetNullableDateTimeOffset(j++, lineItem.Date01);
					record.SetNullableDateTimeOffset(j++, lineItem.Date02);
					record.SetNullableDateTimeOffset(j++, lineItem.Date03);
					record.SetNullableDateTimeOffset(j++, lineItem.Date04);
					record.SetNullableDouble(j++, lineItem.NonDomesticPrice);
					record.SetNullableDouble(j++, lineItem.ExchangeRate);
					record.SetNullableString(j++, lineItem.QualityTestNumber);
					record.SetNullableDouble(j++, lineItem.Odometer);
					record.SetNullableString(j++, lineItem.DeliveryLocation);
					record.SetNullableDouble(j++, lineItem.Variance);
					record.SetNullableBoolean(j++, lineItem.PartialFill);
					record.SetNullableDouble(j++, lineItem.Quantity.MassInventoryChange);
					record.SetNullableBoolean(j++, lineItem.Quantity.NetManualValueFlag);
					record.SetNullableBoolean(j++, lineItem.Quantity.GrossManualValueFlag);
					record.SetNullableBoolean(j++, lineItem.Quantity.MassManualValueFlag);
					record.SetNullableBoolean(j++, lineItem.Quantity.VcfManualValueFlag);
					record.SetNullableBoolean(j++, lineItem.Quantity.DeliveredGrossManualValueFlag);
					record.SetNullableBoolean(j++, lineItem.Quantity.DeliveredNetManualValueFlag);
					record.SetInt32(j++, (int)lineItem.Status);
					record.SetInt32(j++, (int)lineItem.Quality);
					record.SetNullableGuid(j++, lineItem.StorageLocationTankGuid);
					record.SetNullableGuid(j++, lineItem.AdditiveProfileGuid);
					record.SetNullableGuid(j++, lineItem.DestinationCompartmentEquipmentGuid);
					record.SetNullableGuid(j++, lineItem.DestinationEQ.EquipmentGuid);
					record.SetNullableGuid(j++, lineItem.OperatorPersonnelGuid);
					record.SetNullableGuid(j++, lineItem.ProductGuid);
					record.SetNullableGuid(j++, lineItem.SourceCompartmentEquipmentGuid);
					record.SetNullableGuid(j++, lineItem.SourceEQ.EquipmentGuid);
					record.SetNullableGuid(j++, lineItem.CurrencyGuid);
					record.SetNullableGuid(j++, lineItem.OrderReferenceTransactionLineItemGuid);
					record.SetNullableGuid(j++, lineItem.LoadingLocationStationGuid);
					record.SetNullableGuid(j++, lineItem.MeterGuid);
					record.SetNullableBoolean(j++, lineItem.Quantity.PackageManualValueFlag);
					record.SetBoolean(j++, lineItem.CleanLineProduct);
					record.SetBoolean(j++, lineItem.CleanLineDeductProduct);
					record.SetNullableDouble(j++, lineItem.CleanLineDeductQuantity);
					record.SetNullableDouble(j++, lineItem.CleanLinePackQuantity);
					record.SetNullableBoolean(j++, lineItem.DualFuelingModeFlag);
					record.SetNullableBoolean(j++, lineItem.DualFuelingPrimaryFlag);
					record.SetNullableDouble(j++, lineItem.EngineRunTime);
					record.SetNullableDouble(j++, lineItem.FlowRate);
					record.SetNullableDouble(j++, lineItem.FuelCompressionFactor);
					record.SetNullableDouble(j++, lineItem.HydrantPressure);
					record.SetNullableString(j++, lineItem.MobileDeviceID);
					record.SetNullableGuid(j++, lineItem.MobileDeviceGuid.GetValueOrDefault());
					record.SetNullableString(j++, lineItem.TemperatureQualityStatus);
					record.SetNullableBoolean(j++, lineItem.MeterStartObtainedAutomaticallyFlag);
					record.SetNullableBoolean(j++, lineItem.MeterStopObtainedAutomaticallyFlag);
					record.SetNullableBoolean(j++, lineItem.NetVolumeIndicator);

					// Line Item User Data Fields
					record.SetNullableGuid(j++, lineItem.TransactionLineItemUserDataGuid);

                // There are 24 columns containing user data.  The columns are named
                // "UserDataX" where X is a number between 1 and 24
	            if (lineItem.UserData != null)
	            {
		            for (int userDataIndex = 1; userDataIndex <= 24; userDataIndex++)
		            {
			            string userDataKey = TransactionDO.UserDataLineItemKeyPrefix + userDataIndex;
			            string userDataKey2 = "UserData" + userDataIndex;
			            string userDataValue = null;

			            if (lineItem.UserData.ContainsKey(userDataKey))
			            {
				            userDataValue = lineItem.UserData[userDataKey];
							Debug.WriteLine($"Output TrxLI UD: {userDataKey}={userDataValue}");
			            }
						else if (lineItem.UserData.ContainsKey(userDataKey2))
						{
							userDataValue = lineItem.UserData[userDataKey2];
							Debug.WriteLine($"Output TrxLI UD: {userDataKey2}={userDataValue}");
						}
						else
						{
							Debug.WriteLine($"Output TrxLI UD[{userDataIndex}]=null");
						}
			           
			            record.SetNullableString(j++, userDataValue);
		            }
	            }

				// Fields Common to all records
                record.SetString(j, user);

                yield return record;
            }
        }
    }
}

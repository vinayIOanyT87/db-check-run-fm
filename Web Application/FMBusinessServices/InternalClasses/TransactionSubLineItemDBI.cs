// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionSubLineItemDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Writes transaction sub line item data to the database
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
    using FMBusinessObjects.DataObjects;
    using FMBusinessServices.DataAccessLayer;
    using Microsoft.SqlServer.Server;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    /// <summary>
    /// Associates a sub line item with information from the transaction header and line item. When saving a sub line item some of the
    /// header information and line item information is saved along with it.
    /// </summary>
    public class SubLineItemWithTransactionInformation
    {
        /// <summary>
        /// The sub line item
        /// </summary>
        public SubLineItemDO SubLineItem;

        /// <summary>
        /// The transaction guid identifying the transaction header the sub line item is associated with
        /// </summary>
        public Guid TransactionGuid;

        /// <summary>
        /// The line item guid identifying the line item the sub line item is associated with
        /// </summary>
        public Guid TransactionLineItemGuid;

        /// <summary>
        /// The inventory date of the transaction header the sub line item is associated with
        /// </summary>
        public DateTime InventoryDate;

        /// <summary>
        /// The TransVersion associated with the current version of the entire transaction record
        /// </summary>
        public long TransVersion;
    }

    /// <summary>
    /// Writes transaction sub line item data to the database
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class TransactionSubLineItemDBI
    {
        /// <summary>
        /// Allows database access.
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionSubLineItemDBI"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public TransactionSubLineItemDBI(string user)
        {
            this.User = user;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the user.
        /// The user who inserted or modified the transaction
        /// </summary>
        private string User { get; }

        #endregion

        /// <summary>
        /// Insert or update sub line items 
        /// </summary>
        /// <param name="security">
        /// Contains Security Information
        /// </param>
        /// <param name="subLineItemsWithTransactionInformation">The sub line items along with any associated information required to save the sub line items</param>
        public void Save(SecurityClass security, List<SubLineItemWithTransactionInformation> subLineItemsWithTransactionInformation)
        {
            if (subLineItemsWithTransactionInformation.Count == 0)
            {
                return;
            }

            // Create new Guids for any subLineItems that have an empty Guid. If a sub line item has an empty guid it means it is a new record.
            foreach (SubLineItemWithTransactionInformation subLineItemWithTransactionInformation in 
                subLineItemsWithTransactionInformation.Where(subLineItemWithTransactionInformation => subLineItemWithTransactionInformation.SubLineItem.TransactionSubLineItemGuid == Guid.Empty))
            {
                subLineItemWithTransactionInformation.SubLineItem.TransactionSubLineItemGuid = Guid.NewGuid();
            }

            using (SqlCommand insertUpdateCommand = new SqlCommand())
            {
                insertUpdateCommand.CommandType = CommandType.StoredProcedure;
                insertUpdateCommand.CommandText = "usp_TransactionSubLineItemsInsertUpdate";
                insertUpdateCommand.CommandTimeout = Math.Max(30, subLineItemsWithTransactionInformation.Count);

                SqlParameter tableValuedParameter = insertUpdateCommand.Parameters.Add("@TransactionSubLineItems", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecords(subLineItemsWithTransactionInformation, this.User);
                tableValuedParameter.TypeName = "dbo.TransactionSubLineItemsType";

                this.ConsolidatedDa.ExecuteQuery(security, insertUpdateCommand);
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert / update stored procedure
        /// </summary>
        /// <param name="subLineItemsWithTransactionInformation">The sub line items and transaction information to create SqlDataRecords for</param>
        /// <param name="user">The user saving the sub line items</param>
        /// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert / update stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<SubLineItemWithTransactionInformation> subLineItemsWithTransactionInformation, string user)
        {      
            SqlMetaData[] metaData = new SqlMetaData[79];

            int i = 0;
            metaData[i++] = new SqlMetaData("TransactionSubLineItemGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("SequenceID", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("Product", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("ProductCode", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("ProductType", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("GrossQuantity", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("DeliveredGrossQuantity", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("NetQuantity", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("DeliveredNetQuantity", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("Pressure", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("Vcf", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("Density", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Temperature", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Customs", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("ArmNumber", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("LineNumber", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("BatchNumber", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("LineFill", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("BottomVolume", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("NetCapacity", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("TankStatus", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("MeterFactor", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("MeterStart", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("MeterStop", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("MeterStopDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("MeterStartDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("FreezePoint", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("DifferentialPressure", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("DosageRate", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("DeleteFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("PresetAmount", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("StorageLocationID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("MeterID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("COAID", SqlDbType.NVarChar, 40);
            metaData[i++] = new SqlMetaData("TransactionInventoryDate", SqlDbType.DateTime);
            metaData[i++] = new SqlMetaData("Tax1", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Tax2", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Tax3", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Tax4", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("Tax5", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("TransVersion", SqlDbType.BigInt);
            metaData[i++] = new SqlMetaData("ImproperAdditization", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("BrokenBlend", SqlDbType.Bit);
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
            metaData[i++] = new SqlMetaData("Date01", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("Date02", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("Date03", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("Date04", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("MassQuantity", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("NetManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("GrossManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("MassManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("VcfManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("DeliveredGrossManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("DeliveredNetManualValueFlag", SqlDbType.Bit);
				metaData[i++] = new SqlMetaData("LookupTransactionStatusIndex", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("LookupQualityIndex", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("ProductGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("StorageLocationTankGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("MeterGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("PackageManualValueFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("CleanLineItem", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("CleanLineDeductItem", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("CleanLineDeductQuantity", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("CleanLinePackQuantity", SqlDbType.Float);
            metaData[i] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (SubLineItemWithTransactionInformation subLineItemWithTransactionInformation in subLineItemsWithTransactionInformation)
            {
					int j = 0;
					SubLineItemDO subLineItem = subLineItemWithTransactionInformation.SubLineItem;

					record.SetGuid(j++, subLineItem.TransactionSubLineItemGuid);
					record.SetNullableGuid(j++, subLineItemWithTransactionInformation.TransactionLineItemGuid);
					record.SetNullableGuid(j++, subLineItemWithTransactionInformation.TransactionGuid);
					record.SetInt32(j++, subLineItem.SequenceId.GetValueOrDefault());
					record.SetNullableString(j++, subLineItem.Product);
					record.SetNullableString(j++, subLineItem.ProductCode);
					record.SetNullableString(j++, subLineItem.ProductType);
					record.SetNullableDouble(j++, subLineItem.Quantity.GrossInventoryChange);
					record.SetNullableDouble(j++, subLineItem.Quantity.DeliveredGrossInventoryChange);
					record.SetNullableDouble(j++, subLineItem.Quantity.NetInventoryChange);
					record.SetNullableDouble(j++, subLineItem.Quantity.DeliveredNetInventoryChange);
					record.SetNullableDouble(j++, subLineItem.Pressure);
					record.SetNullableDouble(j++, subLineItem.VCF);
					record.SetNullableDouble(j++, subLineItem.Density);
					record.SetNullableDouble(j++, subLineItem.Temperature);
					record.SetNullableString(j++, subLineItem.Customs);
					record.SetNullableInt(j++, subLineItem.ArmNumber);
					record.SetNullableInt(j++, subLineItem.LineNumber);
					record.SetNullableString(j++, subLineItem.BatchNumber);
					record.SetNullableDouble(j++, subLineItem.LineFill);
					record.SetNullableDouble(j++, subLineItem.BottomVolume);
					record.SetNullableDouble(j++, subLineItem.NetCapacity);
					record.SetNullableString(j++, subLineItem.TankStatus);
					record.SetNullableDouble(j++, subLineItem.MeterReading.MeterFactor);
					record.SetNullableDouble(j++, subLineItem.MeterReading.MeterStart);
					record.SetNullableDouble(j++, subLineItem.MeterReading.MeterStop);
					record.SetNullableDateTimeOffset(j++, subLineItem.MeterReading.StopDateTime);
					record.SetNullableDateTimeOffset(j++, subLineItem.MeterReading.StartDateTime);
					record.SetNullableDouble(j++, subLineItem.FreezePoint);
					record.SetNullableDouble(j++, subLineItem.DifferentialPressure);
					record.SetNullableDouble(j++, subLineItem.DosageRate);
					record.SetBoolean(j++, subLineItem.DeleteFlag);
					record.SetNullableDouble(j++, subLineItem.PresetAmount);
					record.SetNullableString(j++, subLineItem.StorageLocationID);
					record.SetNullableString(j++, subLineItem.MeterID);
					record.SetNullableString(j++, subLineItem.COAID);
					record.SetDateTime(j++, subLineItemWithTransactionInformation.InventoryDate);
					record.SetNullableDouble(j++, subLineItem.Tax1);
					record.SetNullableDouble(j++, subLineItem.Tax2);
					record.SetNullableDouble(j++, subLineItem.Tax3);
					record.SetNullableDouble(j++, subLineItem.Tax4);
					record.SetNullableDouble(j++, subLineItem.Tax5);
					record.SetInt64(j++, subLineItemWithTransactionInformation.TransVersion);
					record.SetNullableBoolean(j++, subLineItem.ImproperAdditization);
					record.SetNullableBoolean(j++, subLineItem.BrokenBlend);
					record.SetBoolean(j++, subLineItem.Flag01);
					record.SetBoolean(j++, subLineItem.Flag02);
					record.SetBoolean(j++, subLineItem.Flag03);
					record.SetBoolean(j++, subLineItem.Flag04);
					record.SetBoolean(j++, subLineItem.Flag05);
					record.SetBoolean(j++, subLineItem.Flag06);
					record.SetNullableDouble(j++, subLineItem.Number01);
					record.SetNullableDouble(j++, subLineItem.Number02);
					record.SetNullableDouble(j++, subLineItem.Number03);
					record.SetNullableDouble(j++, subLineItem.Number04);
					record.SetNullableDouble(j++, subLineItem.Number05);
					record.SetNullableDouble(j++, subLineItem.Number06);
					record.SetNullableDateTimeOffset(j++, subLineItem.Date01);
					record.SetNullableDateTimeOffset(j++, subLineItem.Date02);
					record.SetNullableDateTimeOffset(j++, subLineItem.Date03);
					record.SetNullableDateTimeOffset(j++, subLineItem.Date04);
					record.SetNullableDouble(j++, subLineItem.Quantity.MassInventoryChange);
					record.SetNullableBoolean(j++, subLineItem.Quantity.NetManualValueFlag);
					record.SetNullableBoolean(j++, subLineItem.Quantity.MassManualValueFlag);
					record.SetNullableBoolean(j++, subLineItem.Quantity.GrossManualValueFlag);
					record.SetNullableBoolean(j++, subLineItem.Quantity.VcfManualValueFlag);
					record.SetNullableBoolean(j++, subLineItem.Quantity.DeliveredGrossManualValueFlag);
					record.SetNullableBoolean(j++, subLineItem.Quantity.DeliveredNetManualValueFlag);
					record.SetInt32(j++, (int)subLineItem.Status);
					record.SetInt32(j++, (int)subLineItem.Quality);
					record.SetNullableGuid(j++, subLineItem.ProductGuid);
					record.SetNullableGuid(j++, subLineItem.StorageLocationTankGuid);
					record.SetNullableGuid(j++, subLineItem.MeterGuid);
					record.SetNullableBoolean(j++, subLineItem.Quantity.PackageManualValueFlag);
					record.SetBoolean(j++, subLineItem.CleanLineProduct);
					record.SetBoolean(j++, subLineItem.CleanLineDeductProduct);
					record.SetNullableDouble(j++, subLineItem.CleanLineDeductQuantity);
					record.SetNullableDouble(j++, subLineItem.CleanLinePackQuantity);
					record.SetString(j, user);

					yield return record;
            }
        }
    }
}
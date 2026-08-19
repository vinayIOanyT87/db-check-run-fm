// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionHeaderDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionHeaderDBI type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;
    using FMBusinessServices.DataAccessLayer;
    using Microsoft.SqlServer.Server;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Writes transaction header data to the database along with notes, user data, and signature data
    /// </summary>
    public class TransactionHeaderDBI
    {
        /// <summary>
        /// Allows database access.
        /// </summary>
        internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

        /// <summary>
        /// The user who inserted or modified the transaction
        /// </summary>
        private string User { get; set; }

        public TransactionHeaderDBI(string user)
        {
            this.User = user;
        }

        /// <summary>
        /// Insert or update the transaction header information from the transactions provided
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="transactions">The transactions that contain the header information to be saved</param>
        public void Save(SecurityClass security, List<TransactionDO> transactions)
        {
            // Generate a new transactionGuid for new transactions.
            // Also generate a TransID if one is not already present
            foreach (TransactionDO newTransaction in transactions.Where(transaction => transaction.TransactionGuid == Guid.Empty))
            {
                newTransaction.TransactionGuid = Guid.NewGuid();

                if (string.IsNullOrEmpty(newTransaction.TransID))
                {
                    newTransaction.TransID = FuelsManagerId.NewId();
                }
            }

            using (var insertUpdateCommand = new SqlCommand())
            {
                insertUpdateCommand.CommandType = CommandType.StoredProcedure;
                insertUpdateCommand.CommandText = "usp_TransactionHeaderInsertUpdate";

                SqlParameter tableValuedParameter = insertUpdateCommand.Parameters.Add("@TransactionHeaders", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecords(transactions, this.User);
                tableValuedParameter.TypeName = "dbo.TransactionHeadersType";

                this.ConsolidatedDA.ExecuteQuery(security, insertUpdateCommand);
            }
        }

        /// <summary>
        /// Delete line items, sub line items, transport line items, and any other child records
        /// That might have been deleted when the transaction was updated.
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="existingTransactions"></param>
        public void DeleteRemaining(SecurityClass security, List<TransactionDO> existingTransactions)
        {
            if (existingTransactions.Count == 0)
            {
                return;
            }

            using (var deleteRemainingCommand = new SqlCommand())
            {
                deleteRemainingCommand.CommandType = CommandType.StoredProcedure;
                deleteRemainingCommand.CommandText = "usp_TransactionDeleteRemainingByTransVersion";

                SqlParameter tableValuedParameter = deleteRemainingCommand.Parameters.Add("@TransactionGuidsAndTransVersions", SqlDbType.Structured);
                tableValuedParameter.Value = CreateDeleteRemainingSqlDataRecords(existingTransactions);
                tableValuedParameter.TypeName = "dbo.TransactionGuidAndTransVersionListType";

                this.ConsolidatedDA.ExecuteQuery(security, deleteRemainingCommand);
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert / update stored procedure
        /// </summary>
        /// <param name="transactions">A list of transactions with header information to create SqlDataRecords for</param>
        /// <param name="user">The user saving the transactions</param>
        /// <returns>SqlDataRecords populated with the transaction header information provided</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<TransactionDO> transactions, string user)
        {
            var metaData = new SqlMetaData[191];

            int i = 0;
            metaData[i++] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("TransID", SqlDbType.NVarChar, 64);
            metaData[i++] = new SqlMetaData("AliasName", SqlDbType.NVarChar, 32);
            metaData[i++] = new SqlMetaData("SubType", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("Site", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("TransReferenceID", SqlDbType.NVarChar, 64);
            metaData[i++] = new SqlMetaData("InventoryDate", SqlDbType.DateTime);
            metaData[i++] = new SqlMetaData("ShipToID", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("ShipToCode", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("SupplierID", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("SupplierCode", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("RequestedDeliveryDate", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("TransDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("TransVersion", SqlDbType.BigInt);
            metaData[i++] = new SqlMetaData("SCACCode", SqlDbType.NVarChar, 4);
            metaData[i++] = new SqlMetaData("CardNumber", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("ShipmentNumber", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("ShipperID", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("ShipperCode", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("OwnerID", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("OwnerCode", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("ManagerID", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("ManagerCode", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("CarrierID", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("CarrierCode", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("ConjoinTransID", SqlDbType.NVarChar, 64);
            metaData[i++] = new SqlMetaData("ReversedTransID", SqlDbType.NVarChar, 64);
            metaData[i++] = new SqlMetaData("LinkedDocumentNumber", SqlDbType.NVarChar, 64);
            metaData[i++] = new SqlMetaData("ReversalType", SqlDbType.NVarChar, 2);
            metaData[i++] = new SqlMetaData("PONumber", SqlDbType.NVarChar, 14);
            metaData[i++] = new SqlMetaData("TimeIn", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("TimeOut", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("TimeEnd", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("RoutingID", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("TicketSource", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("LoadID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("BillToID", SqlDbType.NVarChar, 100);
            metaData[i++] = new SqlMetaData("BillToCode", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("DriverIdentificationNumber", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("CreditAmount", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("CardExpiration", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("CardName", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("CardType", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("CashAmount", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("RouteOriginationDate", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("InternationalRouteIndicator", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("PreviousRoutingID", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("ShippingDocumentNumber", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("DocumentNumber", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("STD", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("ETD", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("STA", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("ETA", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("SFT", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("FST", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("EstimatedFuelingDuration", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("DeleteFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("TicketMode", SqlDbType.NVarChar, 15);
            metaData[i++] = new SqlMetaData("DestinationRegistrationID1", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("DestinationSerialNumber1", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("DestinationEquipmentType1", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("DestinationEquipmentModel1", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("DestinationCompanyEquipmentID1", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("DestinationRegistrationID2", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("DestinationSerialNumber2", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("DestinationEquipmentType2", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("DestinationEquipmentModel2", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("DestinationCompanyEquipmentID2", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("DestinationRegistrationID3", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("DestinationSerialNumber3", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("DestinationEquipmentType3", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("DestinationEquipmentModel3", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("DestinationCompanyEquipmentID3", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("SourceRegistrationID1", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("SourceSerialNumber1", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("SourceEquipmentType1", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("SourceEquipmentModel1", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("SourceCompanyEquipmentID1", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("SourceRegistrationID2", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("SourceSerialNumber2", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("SourceEquipmentType2", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("SourceEquipmentModel2", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("SourceCompanyEquipmentID2", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("SourceRegistrationID3", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("SourceSerialNumber3", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("SourceEquipmentType3", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("SourceEquipmentModel3", SqlDbType.NVarChar, 20);
            metaData[i++] = new SqlMetaData("SourceCompanyEquipmentID3", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("OperatorID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("EffectiveDate", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("ExpirationDate", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("ScheduledDate", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("AutoComplete", SqlDbType.Bit);
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
            metaData[i++] = new SqlMetaData("ContactFirstName", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("ContactSurname", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("Date01", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("Date02", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("Date03", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("Date04", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("LegacyNumber", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("Country", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("ContactInfo", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("AssociatedDocNumber", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("AssociatedCLIN", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("SubmittedToAccounting", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("FuelCardID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("AssociatedTransportOrderNumber", SqlDbType.NVarChar, 30);
            metaData[i++] = new SqlMetaData("RequestedDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("DispatchedDateTime", SqlDbType.DateTimeOffset);
            metaData[i++] = new SqlMetaData("ErrorFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("SiteGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("LookupTransTypeIndex", SqlDbType.SmallInt);
            metaData[i++] = new SqlMetaData("LookupTransactionStatusIndex", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("LookupOriginApplicationIndex", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("TransactionAliasGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("BillToCompanyGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("Destination1EquipmentGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("Destination2EquipmentGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("Destination3EquipmentGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("FinalStationIATAGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("FuelCardGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("NextStationIATAGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("OperatorPersonnelGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("OriginStationIATAGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("OwnerCompanyGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("PreviousStationIATAGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("ShipperCompanyGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("ShipToCompanyGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("Source1EquipmentGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("Source2EquipmentGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("Source3EquipmentGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("SupplierCompanyGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("CarrierCompanyGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("ReasonCodeGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("OriginStationIATAID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("PreviousStationIATAID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("NextStationIATAID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("FinalStationIATAID", SqlDbType.NVarChar, 50);
            metaData[i++] = new SqlMetaData("OperatorName", SqlDbType.NVarChar, 150);
            metaData[i++] = new SqlMetaData("FuelAdditiveFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("IssuePoint", SqlDbType.NVarChar, -1);
            metaData[i++] = new SqlMetaData("IssuePointNumber", SqlDbType.NVarChar, -1);
            metaData[i++] = new SqlMetaData("RadioNumber", SqlDbType.NVarChar, -1);
            metaData[i++] = new SqlMetaData("GateID", SqlDbType.NVarChar, 10);
            metaData[i++] = new SqlMetaData("GateGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("ShippingMethod", SqlDbType.NVarChar, 150);
            metaData[i++] = new SqlMetaData("ReferencedTransactionGuid", SqlDbType.UniqueIdentifier);

            // Transaction User Data Fields
            metaData[i++] = new SqlMetaData("TransactionUserDataGuid", SqlDbType.UniqueIdentifier);

            for (int userDataIndex = 1; userDataIndex <= 24; ++userDataIndex)
            {
                // The fields are NVARCHAR(MAX) so the length is -1
                metaData[i++] = new SqlMetaData("UserData" + userDataIndex, SqlDbType.NVarChar, -1);
            }

            // Transaction Notes Fields
            metaData[i++] = new SqlMetaData("TransactionNoteGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("Notes", SqlDbType.NVarChar, 1000);
            metaData[i++] = new SqlMetaData("AdditionalInformation", SqlDbType.NVarChar, 1000);

            // Transaction Signature fields
            metaData[i++] = new SqlMetaData("TransactionSignatureGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("Signature", SqlDbType.VarBinary, -1);

            // Fields common to all records
            metaData[i] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);

            var record = new SqlDataRecord(metaData);

            foreach (TransactionDO transaction in transactions)
            {
                int j = 0;

                record.SetGuid(j++, transaction.TransactionGuid);
                record.SetString(j++, transaction.TransID);
                record.SetNullableString(j++, transaction.Alias);
                record.SetNullableString(j++, transaction.SubType);
                record.SetNullableString(j++, transaction.Site);
                record.SetNullableString(j++, transaction.TransRefID);
                record.SetDateTime(j++, transaction.InventoryDate);
                record.SetNullableString(j++, transaction.ShipToID);
                record.SetNullableString(j++, transaction.ShipToCode);
                record.SetNullableString(j++, transaction.SupplierID);
                record.SetNullableString(j++, transaction.SupplierCode);
                record.SetNullableDateTimeOffset(j++, transaction.RequestedDeliveryDate);
                record.SetNullableDateTimeOffset(j++, transaction.TransactionDateTime);
                record.SetInt64(j++, transaction.TransVersion);
                record.SetNullableString(j++, transaction.SCACCode);
                record.SetNullableString(j++, transaction.PaymentInfo.CreditCardNumber);
                record.SetNullableString(j++, transaction.ShipmentNumber);
                record.SetNullableString(j++, transaction.ShipperID);
                record.SetNullableString(j++, transaction.ShipperCode);
                record.SetNullableString(j++, transaction.OwnerID);
                record.SetNullableString(j++, transaction.OwnerCode);
                record.SetNullableString(j++, transaction.ManagerID);
                record.SetNullableString(j++, transaction.ManagerCode);
                record.SetNullableString(j++, transaction.CarrierID);
                record.SetNullableString(j++, transaction.CarrierCode);
                record.SetNullableString(j++, transaction.ConjoinedTransID);
                record.SetNullableString(j++, transaction.ReversedTransID);
                record.SetNullableString(j++, transaction.LinkedDocumentNumber);

                if (transaction.ReversalType == TransactionDO.None)
                {
                    record.SetDBNull(j++);
                }
                else
                {
                    record.SetNullableString(j++, transaction.ReversalType);
                }

                record.SetNullableString(j++, transaction.PONumber);
                record.SetNullableDateTimeOffset(j++, transaction.TimeIn);
                record.SetNullableDateTimeOffset(j++, transaction.TimeOut);
                record.SetNullableDateTimeOffset(j++, transaction.TimeEnd);
                record.SetNullableString(j++, transaction.RouteInfo.RoutingID);
                record.SetNullableString(j++, transaction.TicketSource);
                record.SetNullableString(j++, transaction.LoadID);
                record.SetNullableString(j++, transaction.BillToID);
                record.SetNullableString(j++, transaction.BillToCode);
                record.SetNullableString(j++, transaction.DriverIDNumber);
                record.SetNullableDouble(j++, transaction.PaymentInfo.CreditCardAmount);
                record.SetNullableDateTimeOffset(j++, transaction.PaymentInfo.CreditCardExpiration);
                record.SetNullableString(j++, transaction.PaymentInfo.CreditCardName);
                record.SetNullableString(j++, transaction.PaymentInfo.CreditCardType);
                record.SetNullableDouble(j++, transaction.PaymentInfo.CashAmount);
                record.SetNullableDateTimeOffset(j++, transaction.RouteInfo.RouteOriginationDate);
                record.SetBoolean(j++, transaction.RouteInfo.InternationalRouteIndicator);
                record.SetNullableString(j++, transaction.RouteInfo.PreviousRoutingID);
                record.SetNullableString(j++, transaction.ShippingDocumentNumber);
                record.SetNullableString(j++, transaction.DocumentNumber);
                record.SetNullableDateTimeOffset(j++, transaction.RouteSchedule.STD);
                record.SetNullableDateTimeOffset(j++, transaction.RouteSchedule.ETD);
                record.SetNullableDateTimeOffset(j++, transaction.RouteSchedule.STA);
                record.SetNullableDateTimeOffset(j++, transaction.RouteSchedule.ETA);
                record.SetNullableDateTimeOffset(j++, transaction.RouteSchedule.SFT);
                record.SetNullableDateTimeOffset(j++, transaction.RouteSchedule.FST);
                record.SetNullableInt(j++, transaction.EstimatedFuelingDuration);
                record.SetBoolean(j++, transaction.DeleteFlag);
                record.SetNullableString(j++, ((int)transaction.TicketMode).ToString(CultureInfo.InvariantCulture));
                record.SetNullableString(j++, transaction.DestinationEQ1.RegistrationID);
                record.SetNullableString(j++, transaction.DestinationEQ1.SerialNumber);
                record.SetNullableString(j++, transaction.DestinationEQ1.EquipmentType);
                record.SetNullableString(j++, transaction.DestinationEQ1.EquipmentModel);
                record.SetNullableString(j++, transaction.DestinationEQ1.CompanyEquipmentID);
                record.SetNullableString(j++, transaction.DestinationEQ2.RegistrationID);
                record.SetNullableString(j++, transaction.DestinationEQ2.SerialNumber);
                record.SetNullableString(j++, transaction.DestinationEQ2.EquipmentType);
                record.SetNullableString(j++, transaction.DestinationEQ2.EquipmentModel);
                record.SetNullableString(j++, transaction.DestinationEQ2.CompanyEquipmentID);
                record.SetNullableString(j++, transaction.DestinationEQ3.RegistrationID);
                record.SetNullableString(j++, transaction.DestinationEQ3.SerialNumber);
                record.SetNullableString(j++, transaction.DestinationEQ3.EquipmentType);
                record.SetNullableString(j++, transaction.DestinationEQ3.EquipmentModel);
                record.SetNullableString(j++, transaction.DestinationEQ3.CompanyEquipmentID);
                record.SetNullableString(j++, transaction.SourceEQ1.RegistrationID);
                record.SetNullableString(j++, transaction.SourceEQ1.SerialNumber);
                record.SetNullableString(j++, transaction.SourceEQ1.EquipmentType);
                record.SetNullableString(j++, transaction.SourceEQ1.EquipmentModel);
                record.SetNullableString(j++, transaction.SourceEQ1.CompanyEquipmentID);
                record.SetNullableString(j++, transaction.SourceEQ2.RegistrationID);
                record.SetNullableString(j++, transaction.SourceEQ2.SerialNumber);
                record.SetNullableString(j++, transaction.SourceEQ2.EquipmentType);
                record.SetNullableString(j++, transaction.SourceEQ2.EquipmentModel);
                record.SetNullableString(j++, transaction.SourceEQ2.CompanyEquipmentID);
                record.SetNullableString(j++, transaction.SourceEQ3.RegistrationID);
                record.SetNullableString(j++, transaction.SourceEQ3.SerialNumber);
                record.SetNullableString(j++, transaction.SourceEQ3.EquipmentType);
                record.SetNullableString(j++, transaction.SourceEQ3.EquipmentModel);
                record.SetNullableString(j++, transaction.SourceEQ3.CompanyEquipmentID);
                record.SetNullableString(j++, transaction.OperatorID);
                record.SetNullableDateTimeOffset(j++, transaction.EffectiveDate);
                record.SetNullableDateTimeOffset(j++, transaction.ExpirationDate);
                record.SetNullableDateTimeOffset(j++, transaction.ScheduledDate);
                record.SetBoolean(j++, transaction.AutoComplete);
                record.SetBoolean(j++, transaction.Flag01);
                record.SetBoolean(j++, transaction.Flag02);
                record.SetBoolean(j++, transaction.Flag03);
                record.SetBoolean(j++, transaction.Flag04);
                record.SetBoolean(j++, transaction.Flag05);
                record.SetBoolean(j++, transaction.Flag06);
                record.SetNullableDouble(j++, transaction.Number01);
                record.SetNullableDouble(j++, transaction.Number02);
                record.SetNullableDouble(j++, transaction.Number03);
                record.SetNullableDouble(j++, transaction.Number04);
                record.SetNullableDouble(j++, transaction.Number05);
                record.SetNullableDouble(j++, transaction.Number06);
                record.SetNullableString(j++, transaction.ContactFirstName);
                record.SetNullableString(j++, transaction.ContactSurname);
                record.SetNullableDateTimeOffset(j++, transaction.Date01);
                record.SetNullableDateTimeOffset(j++, transaction.Date02);
                record.SetNullableDateTimeOffset(j++, transaction.Date03);
                record.SetNullableDateTimeOffset(j++, transaction.Date04);
                record.SetNullableString(j++, transaction.LegacyNumber);
                record.SetNullableString(j++, transaction.Country);
                record.SetNullableString(j++, transaction.ContactInfo);
                record.SetNullableString(j++, transaction.AssociatedDocumentNumber);
                record.SetNullableString(j++, transaction.AssociatedCLIN);
                record.SetNullableBoolean(j++, transaction.SubmittedToAccounting);
                record.SetNullableString(j++, transaction.FuelCardID);
                record.SetNullableString(j++, transaction.AssociatedTransportOrderNumber);
                record.SetNullableDateTimeOffset(j++, transaction.RequestedDateTime);
                record.SetNullableDateTimeOffset(j++, transaction.DispatchedDateTime);
                record.SetBoolean(j++, transaction.ErrorFlag);
                record.SetNullableGuid(j++, transaction.SiteGuid);
                record.SetInt16(j++, (short)transaction.TransTypeID);
                record.SetInt32(j++, (int)transaction.Status);
                record.SetInt32(j++, (int)transaction.OriginApplication);
                record.SetNullableGuid(j++, transaction.TransactionAliasGuid);
                record.SetNullableGuid(j++, transaction.BillToCompanyGuid);
                record.SetNullableGuid(j++, transaction.DestinationEQ1.EquipmentGuid);
				record.SetNullableGuid(j++, transaction.DestinationEQ2.EquipmentGuid);
				record.SetNullableGuid(j++, transaction.DestinationEQ3.EquipmentGuid);
                record.SetNullableGuid(j++, transaction.RouteInfo.FinalStationIATAGuid);
                record.SetNullableGuid(j++, transaction.FuelCardGuid);
                record.SetNullableGuid(j++, transaction.ManagerCompanyGuid);
                record.SetNullableGuid(j++, transaction.RouteInfo.NextStationIATAGuid);
                record.SetNullableGuid(j++, transaction.OperatorPersonnelGuid);
                record.SetNullableGuid(j++, transaction.RouteInfo.OriginStationIATAGuid);
                record.SetNullableGuid(j++, transaction.OwnerCompanyGuid);
                record.SetNullableGuid(j++, transaction.RouteInfo.PreviousStationIATAGuid);
                record.SetNullableGuid(j++, transaction.ShipperCompanyGuid);
                record.SetNullableGuid(j++, transaction.ShipToCompanyGuid);
				record.SetNullableGuid(j++, transaction.SourceEQ1.EquipmentGuid);
				record.SetNullableGuid(j++, transaction.SourceEQ2.EquipmentGuid);
				record.SetNullableGuid(j++, transaction.SourceEQ3.EquipmentGuid);
                record.SetNullableGuid(j++, transaction.SupplierCompanyGuid);
                record.SetNullableGuid(j++, transaction.CarrierCompanyGuid);
                record.SetNullableGuid(j++, transaction.ReasonCodeGuid);
                record.SetNullableString(j++, transaction.RouteInfo.OriginStationIATAID);
                record.SetNullableString(j++, transaction.RouteInfo.PreviousStationIATAID);
                record.SetNullableString(j++, transaction.RouteInfo.NextStationIATAID);
                record.SetNullableString(j++, transaction.RouteInfo.FinalStationIATAID);
                record.SetNullableString(j++, transaction.OperatorName);
                record.SetBoolean(j++, transaction.FuelAdditiveFlag);
                record.SetNullableString(j++, transaction.IssuePoint);
                record.SetNullableString(j++, transaction.IssuePointNumber);
                record.SetNullableString(j++, transaction.RadioNumber);
                record.SetNullableString(j++, transaction.GateID);
                record.SetNullableGuid(j++, transaction.GateGuid);
                record.SetNullableString(j++, transaction.ShippingMethod); 
                record.SetNullableGuid(j++, transaction.ReferencedTransactionGuid);

                // Transaction User Data Fields
                record.SetNullableGuid(j++, transaction.TransactionUserDataGuid);

                for (int userDataIndex = 1; userDataIndex <= 24; ++userDataIndex)
                {
                    string userDataKey = TransactionDO.UserDataKeyPrefix + userDataIndex;
                    string userDataKey2 = "UserData" + userDataIndex;
                    string userDataValue = null;

                    if (transaction.UserData.ContainsKey(userDataKey))
                    {
                        userDataValue = transaction.UserData[userDataKey];
                    }
                    else if (transaction.UserData.ContainsKey(userDataKey2))
                    {
                        userDataValue = transaction.UserData[userDataKey2];
                    }

                    record.SetNullableString(j++, userDataValue);
                }

                // Transaction Notes Fields
                record.SetNullableGuid(j++, transaction.TransactionNoteGuid);
                record.SetNullableString(j++, transaction.Notes);
                record.SetNullableString(j++, transaction.AdditionalInformation);

                // Transaction Signature fields
                record.SetNullableGuid(j++, transaction.TransactionSignatureGuid);

                if (transaction.Signature == null || transaction.Signature.Length == 0)
                {
                    record.SetDBNull(j++);
                }
                else
                {
                    record.SetBytes(j++, 0, transaction.Signature, 0, transaction.Signature.Length);
                }

                // Fields common to all records
                record.SetString(j, user);

                yield return record;
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter passed to the Delete remaining stored procedure
        /// </summary>
        /// <param name="transactions">The transactions to delete remaining records for</param>
        /// <returns>SqlDataRecords corresponding to the table valued parameter passed to the Delete remaining stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateDeleteRemainingSqlDataRecords(IEnumerable<TransactionDO> transactions)
        {
            var metaData = new SqlMetaData[2];

            int i = 0;
            metaData[i++] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);        
            metaData[i] = new SqlMetaData("TransVersion", SqlDbType.BigInt);
        
            var record = new SqlDataRecord(metaData);

            foreach (TransactionDO transaction in transactions)
            {
                int j = 0;

                record.SetGuid(j++, transaction.TransactionGuid);        
                record.SetInt64(j, transaction.TransVersion);
             
                yield return record;
            }
        }
    }
}

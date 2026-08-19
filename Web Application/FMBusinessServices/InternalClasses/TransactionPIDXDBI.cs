// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionPIDXDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Writes transaction PIDX information to the database
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
   /// Associated a transaction PIDX record with information from the corresponding transaction header.
   /// For example, we need the TransactionGuid when saving Transaction PIDX records
   /// </summary>
   public class TransactionPIDXWithTransactionInformation
   {
      /// <summary>
      /// The PIDX record
      /// </summary>
      public TransactionPIDXDO TransactionPIDX;

      /// <summary>
      /// The transaction guid identifying the transaction header the PIDX record is associated with
      /// </summary>
      public Guid TransactionGuid;
   }

   /// <summary>
   /// Writes transaction PIDX information to the database
   /// </summary>
   // ReSharper disable once InconsistentNaming
   public class TransactionPIDXDBI
   {
      /// <summary>
      /// Allows access to the database
      /// </summary>
      internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

      /// <summary>
      /// The user who inserted or modified the transaction
      /// </summary>
      private string User { get; }

      /// <summary>
      /// Initializes a new instance of the <see cref="TransactionPIDXDBI"/> class. 
      /// This the default constructor for the Transaction PIDX DBI class.
      /// </summary>
      /// <param name="user">
      /// </param>
      public TransactionPIDXDBI(string user)
      {
         this.User = user;
      }

      /// <summary>
      /// Add or update transactionPIDX records in the database
      /// </summary>
      /// <param name="security">
      /// Contains security credentials
      /// </param>
      /// <param name="transactionPidxsWithTransactionInformation">
      /// The transaction PIDX records to save along with the corresponding information from the transaction header record 
      /// </param>
      /// <param name="forceNewRecord">
      /// When true, new records will be created even if current records matching exist; this supports resends
      /// </param>
      public void Save(SecurityClass security, List<TransactionPIDXWithTransactionInformation> transactionPidxsWithTransactionInformation, bool forceNewRecord)
      {
         if (transactionPidxsWithTransactionInformation.Count == 0)
         {
            return;
         }

         foreach (TransactionPIDXWithTransactionInformation transactionPIDXWithTransactionInformation in
                 transactionPidxsWithTransactionInformation.Where(pidx => pidx.TransactionPIDX.TransactionPIDXGuid == Guid.Empty))
         {
            // The sent flag is always set to false for new records
            transactionPIDXWithTransactionInformation.TransactionPIDX.SentFlag = false;
         }

         using (var insertUpdateCommand = new SqlCommand())
         {
            // Call the stored procedure, passing in the table of weight readings we constructed
            insertUpdateCommand.CommandType = CommandType.StoredProcedure;
            insertUpdateCommand.CommandText = forceNewRecord ? "usp_TransactionPIDXsInsertNew" : "usp_TransactionPIDXsInsertUpdate";
            insertUpdateCommand.CommandTimeout = Math.Max(30, transactionPidxsWithTransactionInformation.Count);

            SqlParameter tableValuedParameter = insertUpdateCommand.Parameters.Add("@TransactionPIDXs", SqlDbType.Structured);
            tableValuedParameter.Value = CreateSqlDataRecords(transactionPidxsWithTransactionInformation, this.User);
            tableValuedParameter.TypeName = "dbo.TransactionPIDXsType";

            this.ConsolidatedDA.ExecuteQuery(security, insertUpdateCommand);
         }
      }

      /// <summary>
      /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert or update stored procedure
      /// </summary>
      /// <param name="transactionPidxsWithTransactionInformation">The PIDX records and transaction information to create SqlDataRecords for</param>
      /// <param name="user">The user saving the PIDX records</param>
      /// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert or update stored procedure</returns>
      private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<TransactionPIDXWithTransactionInformation> transactionPidxsWithTransactionInformation, string user)
      {
         SqlMetaData[] metaData = new SqlMetaData[10];
         int i = 0;

         metaData[i++] = new SqlMetaData("TransactionPIDXGuid", SqlDbType.UniqueIdentifier);
         metaData[i++] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);
         metaData[i++] = new SqlMetaData("AuthorizationNumber", SqlDbType.NVarChar, 8);
         metaData[i++] = new SqlMetaData("SentFlag", SqlDbType.Bit);
         metaData[i++] = new SqlMetaData("DateSent", SqlDbType.DateTimeOffset);
         metaData[i++] = new SqlMetaData("BrokenBlend", SqlDbType.Bit);
         metaData[i++] = new SqlMetaData("PIDXProfileGuid", SqlDbType.UniqueIdentifier);
         metaData[i++] = new SqlMetaData("CompanyPersonnelToShipToBillToGuid", SqlDbType.UniqueIdentifier);
         metaData[i++] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);
         metaData[i] = new SqlMetaData("BOLVersion", SqlDbType.Int);

         SqlDataRecord record = new SqlDataRecord(metaData);

         foreach (TransactionPIDXWithTransactionInformation transactionPIDXWithTransactionInformation in transactionPidxsWithTransactionInformation)
         {
            int j = 0;
            record.SetNullableGuid(j++, transactionPIDXWithTransactionInformation.TransactionPIDX.TransactionPIDXGuid);
            record.SetNullableGuid(j++, transactionPIDXWithTransactionInformation.TransactionGuid);
            record.SetNullableString(j++, transactionPIDXWithTransactionInformation.TransactionPIDX.AuthorizationNumber);
            record.SetBoolean(j++, transactionPIDXWithTransactionInformation.TransactionPIDX.SentFlag);
            record.SetDateTimeOffset(j++, transactionPIDXWithTransactionInformation.TransactionPIDX.DateSent);
            record.SetBoolean(j++, transactionPIDXWithTransactionInformation.TransactionPIDX.BrokenBlend);
            record.SetNullableGuid(j++, transactionPIDXWithTransactionInformation.TransactionPIDX.PIDXProfileGuid);
            record.SetNullableGuid(j++, transactionPIDXWithTransactionInformation.TransactionPIDX.CompanyPersonnelToShipToBillToGuid);
            record.SetString(j++, user);
            record.SetNullableInt(j, transactionPIDXWithTransactionInformation.TransactionPIDX.BOLVersion);

            yield return record;
         }
      }
   }
}
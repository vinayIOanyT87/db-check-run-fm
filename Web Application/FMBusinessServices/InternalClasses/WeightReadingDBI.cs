// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeightReadingDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Write transaction weight reading information to the database
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

    /// <summary>
    /// Associates a weight reading with information from the transaction header. When saving a weight reading some of the
    /// header information is saved along with it.
    /// </summary>
    public class WeightReadingWithTransactionInformation
    {
        /// <summary>
        /// The weight reading
        /// </summary>
        public WeightReadingDO WeightReading;

        /// <summary>
        /// The transaction guid identifying the transaction header the weight reading is associated with
        /// </summary>
        public Guid TransactionGuid;

        /// <summary>
        /// The TransVersion associated with the current version of the entire transaction record
        /// </summary>
        public long TransVersion;
    }

    /// <summary>
	/// Write transaction weight reading information to the database
	/// </summary>
    // ReSharper disable once InconsistentNaming
	public class WeightReadingDBI
	{
		/// <summary>
		/// Allows database access
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="WeightReadingDBI"/> class.
		/// </summary>
		/// <param name="user">
		/// The user.
		/// </param>
		public WeightReadingDBI(string user)
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
		/// The save.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
        /// <param name="weightReadingsWithTransactionGuidsAndTransVersion">
		/// The weight readings to save.
		/// </param>
        public void Save(SecurityClass security, List<WeightReadingWithTransactionInformation> weightReadingsWithTransactionGuidsAndTransVersion)
		{
			// All previous weight readings associated with this transaction become historical, 
			// so we always insert a new record
            this.Insert(security, weightReadingsWithTransactionGuidsAndTransVersion);
		}

		/// <summary>
		/// Insert weight readings into the database. Existing weight readings become historical records so we are always inserting and not updating 
		/// records.
		/// </summary>
		/// <param name="security">
		/// Contains security information
		/// </param>
        /// <param name="weightReadingsWithTransactionInformation">
        /// The weight readings to save.
		/// </param>
        private void Insert(SecurityClass security, List<WeightReadingWithTransactionInformation> weightReadingsWithTransactionInformation)
		{
            // If there are no weight readings to save then we don't need to call the stored procedure.
            if (weightReadingsWithTransactionInformation.Count == 0)
		    {
		        return;
		    }

			using (var insertCommand = new SqlCommand())
			{
                // Call the stored procedure, passing in the table of weight readings we constructed
			    insertCommand.CommandType = CommandType.StoredProcedure;
				insertCommand.CommandText = "usp_TransactionWeightReadingsInsert";
			    insertCommand.CommandTimeout = Math.Max(30, weightReadingsWithTransactionInformation.Count);

                SqlParameter tableValuedParameter = insertCommand.Parameters.Add("@TransactionWeightReadings", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecords(weightReadingsWithTransactionInformation, this.User);
                tableValuedParameter.TypeName = "dbo.TransactionWeightReadingsType";

				this.ConsolidatedDa.ExecuteQuery(security, insertCommand);
			}
		}

        /// <summary>
        /// Mark all weight readings associated with a specific transaction as historical.
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="transactionGuids">Identifies a transaction record</param>
        public void MarkExistingRecordsAsHistorical(SecurityClass security, List<Guid> transactionGuids)
        {
            if (transactionGuids.Count == 0)
            {
                return;
            }

            using (var updateHistoricalCommand = new SqlCommand())
            {
                updateHistoricalCommand.CommandType = CommandType.StoredProcedure;
                updateHistoricalCommand.CommandText = "usp_TransactionWeightReadingsUpdateExistingAsHistorical";

                SqlParameter tableValuedParameter = updateHistoricalCommand.Parameters.Add("@TransactionGuids", SqlDbType.Structured);
                tableValuedParameter.Value = CreateSqlDataRecordsForHistoricalUpdate(transactionGuids);
                tableValuedParameter.TypeName = "dbo.TransactionGuidListType";

                this.ConsolidatedDa.ExecuteQuery(security, updateHistoricalCommand);
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure
        /// </summary>
        /// <param name="weightReadingsWithTransactionInformation">The weight readings and transaction information to create SqlDataRecords for</param>
        /// <param name="user">The user saving the weight readings</param>
        /// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the insert stored procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<WeightReadingWithTransactionInformation> weightReadingsWithTransactionInformation, string user)
        {
            SqlMetaData[] metaData = new SqlMetaData[10];

            int i = 0;
            metaData[i++] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);
            metaData[i++] = new SqlMetaData("CompartmentID", SqlDbType.NVarChar, 30);       
            metaData[i++] = new SqlMetaData("BeginQuantityValue", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("RequestedQuantityValue", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("FinalQuantityValue", SqlDbType.Float);
            metaData[i++] = new SqlMetaData("SourceVersionNumber", SqlDbType.Int);
            metaData[i++] = new SqlMetaData("HistoricalFlag", SqlDbType.Bit);
            metaData[i++] = new SqlMetaData("TransVersion", SqlDbType.BigInt);
            metaData[i++] = new SqlMetaData("VolumetricTopOffFlag", SqlDbType.Bit);
            metaData[i] = new SqlMetaData("CreatedUpdatedBy", SqlDbType.NVarChar, 100);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (WeightReadingWithTransactionInformation weightReadingWithTransactionInformation in weightReadingsWithTransactionInformation)
            {
                int j = 0;

                record.SetGuid(j++, weightReadingWithTransactionInformation.TransactionGuid);
                record.SetString(j++, weightReadingWithTransactionInformation.WeightReading.CompartmentName);
                record.SetNullableDouble(j++, weightReadingWithTransactionInformation.WeightReading.BeginQuantity);
                record.SetNullableDouble(j++, weightReadingWithTransactionInformation.WeightReading.RequestedQuantity);
                record.SetNullableDouble(j++, weightReadingWithTransactionInformation.WeightReading.FinalQuantity);
                record.SetNullableInt(j++, weightReadingWithTransactionInformation.WeightReading.SourceVersionNumber);
                record.SetBoolean(j++, weightReadingWithTransactionInformation.WeightReading.HistoricalFlag);
                record.SetInt64(j++, weightReadingWithTransactionInformation.TransVersion);
                record.SetNullableBoolean(j++, weightReadingWithTransactionInformation.WeightReading.VolumetricTopOffFlag);
                record.SetString(j, user);

                yield return record;
            }
        }

        /// <summary>
        /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the procedure which marks weight readings as historical
        /// </summary>
        /// <param name="transactionGuidsThatHadWeightReadings">The transaction Guids to create SqlDataRecords for</param>
        /// <returns>SqlDataRecords corresponding to the table valued parameter we pass to the historical update procedure</returns>
        private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForHistoricalUpdate(IEnumerable<Guid> transactionGuidsThatHadWeightReadings)
        {
            SqlMetaData[] metaData = new SqlMetaData[1];

            metaData[0] = new SqlMetaData("TransactionGuid", SqlDbType.UniqueIdentifier);

            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (Guid transactionGuid in transactionGuidsThatHadWeightReadings)
            {
                record.SetGuid(0, transactionGuid);

                yield return record;
            }
        }
	}
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeakGraphDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LeakGraphDBI type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.ServiceClasses;

	using Microsoft.SqlServer.Server;

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

	/// <summary>
	/// Writes leak graph data to the database
	/// </summary>
	internal class LeakGraphDBI
	 {
		  /// <summary>
		  /// Allows database access.
		  /// </summary>
		  internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		  public LeakGraphDBI()
		  {
		  }

		  /// <summary>
		  /// Insert or update the transaction header information from the transactions provided
		  /// </summary>
		  /// <param name="security">Contains security information</param>
		  /// <param name="transactions">The transactions that contain the header information to be saved</param>
		  public void Save(SecurityClass security, Guid leakRecordId, List<MasterSample> graphSamples)
		  {
				using (var insertUpdateCommand = new SqlCommand())
				{
					 insertUpdateCommand.CommandType = CommandType.StoredProcedure;
					 insertUpdateCommand.CommandText = "usp_LeakGraphInsertUpdate";

					 SqlParameter tableValuedParameter = insertUpdateCommand.Parameters.Add("@LeakSamples", SqlDbType.Structured);
					 tableValuedParameter.Value = CreateSqlDataRecords(leakRecordId, graphSamples);
					 tableValuedParameter.TypeName = "dbo.LeakGraphType";

					 this.ConsolidatedDA.ExecuteQuery(security, insertUpdateCommand);
				}
		  }

		  /// <summary>
		  /// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert / update stored procedure
		  /// </summary>
		  /// <param name="graphSamples">A list of graph samples to create SqlDataRecords for</param>
		  /// <returns>SqlDataRecords populated with the transaction header information provided</returns>
		  private static IEnumerable<SqlDataRecord> CreateSqlDataRecords(Guid leakRecordId, IEnumerable<MasterSample> graphSamples)
		  {
				var metaData = new SqlMetaData[4];

				int i = 0;
				metaData[i++] = new SqlMetaData("LeakReportId", SqlDbType.UniqueIdentifier);
				metaData[i++] = new SqlMetaData("SampleTime", SqlDbType.DateTimeOffset);
				metaData[i++] = new SqlMetaData("SampleVolume", SqlDbType.Float);
				metaData[i++] = new SqlMetaData("IsUsed", SqlDbType.Bit);

				var record = new SqlDataRecord(metaData);

				foreach (MasterSample sample in graphSamples)
				{
					 int j = 0;

					 record.SetGuid(j++, leakRecordId);
					 record.SetDateTimeOffset(j++, sample.TimeStamp);
					 record.SetDouble(j++, sample.Volume);
					 record.SetBoolean(j++, string.IsNullOrEmpty(sample.Reason));

					 yield return record;
				}
		  }
	 }
}

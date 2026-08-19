using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
	internal class PointCalculatorRunDBI
	{
		/// <summary>
		/// Allows database access.
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public PointCalculatorRunDBI()
		{
		}

		public void CleanupPointCalculatorRunsFromDB(SecurityClass security, int intervalMinutesToKeep)
		{
			using (var deleteOldRecords = new SqlCommand())
			{
				deleteOldRecords.CommandType = CommandType.StoredProcedure;
				deleteOldRecords.CommandText = "usp_CleanupPointCalculatorRunTables";

				SqlParameter intParam = deleteOldRecords.Parameters.Add("@IntervalMinutesToKeep", SqlDbType.Int);
				intParam.Value = intervalMinutesToKeep;

				this.ConsolidatedDA.ExecuteQuery(security, deleteOldRecords);
			}
		}
		/// <summary>
		/// Insert or update the transaction header information from the transactions provided
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="transactions">The transactions that contain the header information to be saved</param>
		public Guid? Save(SecurityClass security, PointCalculatorResult result)
		{
			using (var insertUpdateCommand = new SqlCommand())
			{
				insertUpdateCommand.CommandType = CommandType.StoredProcedure;
				insertUpdateCommand.CommandText = "usp_PointCalculatorInsertUpdate";

				SqlParameter tableValuedParameter = insertUpdateCommand.Parameters.Add("@CalculatorRunHeader", SqlDbType.Structured);
				tableValuedParameter.Value = CreateSqlDataRecordsForHeader(result);
				tableValuedParameter.TypeName = "dbo.PointCalculatorRunDataType";

				tableValuedParameter = insertUpdateCommand.Parameters.Add("@CalculatorRunDetails", SqlDbType.Structured);
				tableValuedParameter.Value = CreateSqlDataRecordsForDetails(result);
				tableValuedParameter.TypeName = "dbo.PointCalculatorRunDetailsDataType";

				object RunGuidObj = this.ConsolidatedDA.ExecuteScalar(insertUpdateCommand, security);
				if (RunGuidObj != null)
				{
					if (Guid.TryParse(RunGuidObj.ToString(), out Guid RunGuid))
						return RunGuid;
					else
						return null;
				}
			}
			return null;
		}

		/// <summary>
		/// Create the SqlDataRecords corresponding to the table valued parameter we pass to the insert / update stored procedure
		/// </summary>
		/// <param name="graphSamples">A list of graph samples to create SqlDataRecords for</param>
		/// <returns>SqlDataRecords populated with the transaction header information provided</returns>
		private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForHeader(PointCalculatorResult result)
		{
			SqlMetaData[] metaData = new SqlMetaData[]
			{
				new SqlMetaData("SiteId", SqlDbType.NVarChar, 50),
				new SqlMetaData("PointId", SqlDbType.NVarChar, 50),
				new SqlMetaData("CalculationMode", SqlDbType.NVarChar, 50),
				new SqlMetaData("UserId", SqlDbType.NVarChar, 50),
				new SqlMetaData("SiteGuid", SqlDbType.UniqueIdentifier),
				new SqlMetaData("PointGuid", SqlDbType.UniqueIdentifier),
				new SqlMetaData("UserGuid", SqlDbType.UniqueIdentifier),
				new SqlMetaData("Token", SqlDbType.UniqueIdentifier)
			};

			var record = new SqlDataRecord(metaData);

			record.SetString(0, result.SiteId);
			record.SetString(1, result.PointId);
			record.SetString(2, result.CalculationMode);
			record.SetString(3, result.UserId);
			record.SetGuid(4, result.SiteGuid);
			record.SetGuid(5, result.PointGuid);
			record.SetGuid(6, result.UserGuid);
			record.SetGuid(7, result.Token);

			yield return record;
		}

		private static IEnumerable<SqlDataRecord> CreateSqlDataRecordsForDetails(PointCalculatorResult result)
		{
			SqlMetaData[] metaData = new SqlMetaData[]
			{
				new SqlMetaData("TagName", SqlDbType.NVarChar, 50),
				new SqlMetaData("Units", SqlDbType.NVarChar, 50),
				new SqlMetaData("Acronym", SqlDbType.NVarChar, 50),
				new SqlMetaData("BeginValue", SqlDbType.NVarChar, 50),
				new SqlMetaData("EndValue", SqlDbType.NVarChar, 50),
				new SqlMetaData("DiffValue", SqlDbType.NVarChar, 50),
				new SqlMetaData("DisplayOrder", SqlDbType.Int),
			};

			var record = new SqlDataRecord(metaData);

			foreach (PointCalculatorTagValue tagValue in result.TagValues)
			{
				int j = 0;

				record.SetString(j++, tagValue.Tagname);
				record.SetString(j++, tagValue.Units);
				record.SetString(j++, tagValue.Acronym);
				record.SetString(j++, tagValue.BeginValue);
				record.SetString(j++, tagValue.EndValue);
				record.SetString(j++, tagValue.DiffValue);
				record.SetInt32(j++, tagValue.DisplayOrder);

				yield return record;
			}
		}
	}
}


namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
    using System.Globalization;
    using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Record and look up whether for a particular site, manager and date an ExSTARS report has been
	/// created and sent to the IRS
	/// </summary>
	public class ExStarsFilings : ExStarsSqlBase
	{
		public ExStarsFilings(ExStarsSiteConfigExpanded config) : base(config){}

		public void UpdateStatus(Guid exStarsFilingsGuid, FileCreatingStatus filingStatus)
		{
			const string StoredProcName = "[gsp_ExStarsFilingsUpdateStatus]";
			using (var cmd = new SqlCommand("[dbo]." + StoredProcName))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@ExStarsFilingsGuid", exStarsFilingsGuid);
				cmd.Parameters.AddWithValue("@FilingStatus", filingStatus.ToString());
				cmd.Parameters.AddWithValue("@UpdatedBy", this.Config.Security.UserID);
				ExecuteNonQuery(cmd);
			}
		}


		public void UpdateForIrsAcknowledgement(ExStarsFilingClass filingsRow)
		{
			const string StoredProcName = "[gsp_ExStarsFilingsUpdateForAcknowledgement]";
			using (var cmd = new SqlCommand("[dbo]." + StoredProcName))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@TransSetControlNumber", filingsRow.TransSetControlNumber);
				cmd.Parameters.AddWithValue("@FilingStatus",filingsRow.FilingStatus.ToString());
				cmd.Parameters.AddWithValue("@Acknowledgement", filingsRow.Acknowledgement);
				cmd.Parameters.AddWithValue("@AckEasyRead", filingsRow.AckEasyRead);
				cmd.Parameters.AddWithValue("@UpdatedBy", this.Config.Security.UserID);
				ExecuteNonQuery(cmd);
			}
		}


		public bool TransactionSetControlNumberInUse(string transSetControlNumber, bool useOriginalControlNumber = false)
		{
			return this.QueryByTransactionSetControlNumber(transSetControlNumber) != null;
		}

		public ExStarsFilingClass GetByTransactionSetControlNumber(string transSetControlNumber, bool useOriginalControlNumber = false)
		{
			DataRowCollection allRows = this.QueryByTransactionSetControlNumber(transSetControlNumber);
			System.Diagnostics.Debug.Assert(useOriginalControlNumber || allRows == null || allRows.Count <= 1);
			if (allRows == null || allRows.Count == 0)
			{
				return null;
			}
			DataRow row = allRows[0];
			return LoadFiling(row);
		}

		protected DataRowCollection QueryByTransactionSetControlNumber(string transSetControlNumber)
		{
			//string sql = string.Format("select * from [dbo].[tblExStarsFilings] WHERE [{0}]='{1}'"
			//	, useOriginalControlNumber ? "OriginalControlNumber" : "TransSetControlNumber"
			//	, transSetControlNumber);
			//return this.GetDataSet(sql, "QueryByTransactionSetCtrl");

			const string StoredProcName = "[usp_ExStarsFilingSelectByTransSetCtrlNumber]";
			using (var cmd = new SqlCommand("[dbo]." + StoredProcName))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@TransSetControlNumber", transSetControlNumber);
				DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, this.Config.Security);
				if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}
				dataSet.Tables[0].TableName = StoredProcName;
				return dataSet.Tables[0].Rows;
			}
		}

		public ExStarsFilingStatusListClass GetStatus(ReportModifiersEnum modifier, bool skipReplaced = false)
		{
			const string StoredProcName = "[usp_ExStarsFilingStatusSelect]";
			using (var cmd = new SqlCommand("[dbo]." + StoredProcName))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@SiteGuid", this.Config.Site.SiteGuid);
				cmd.Parameters.AddWithValue("@ManagerCompanyGuid", this.Config.ManagerCompanyGuid);
				cmd.Parameters.AddWithValue("@FilingStartDate", this.Config.StartTransactionDateTime);
				cmd.Parameters.AddWithValue("@FilingEndDate", this.Config.EndTransactionDateTime);
				cmd.Parameters.AddWithValue("@Modifier", modifier == ReportModifiersEnum.AllTypes ? "" : modifier.ToString());

				DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, this.Config.Security);

				if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return new ExStarsFilingStatusListClass();
				}
				DataTable table = dataSet.Tables[0];
				table.TableName = StoredProcName;
				return LoadStatusResults(table, skipReplaced);
			}
		}

		public void DeleteFiling(Guid filingsGuid)
		{
			const string StoredProcName = "[gsp_ExStarsDeleteFiling]";
			using (var cmd = new SqlCommand("[dbo]." + StoredProcName))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@ExStarsFilingsGuid", filingsGuid);
				ExecuteNonQuery(cmd);
			}
		}

		public ExStarsFilingStatusListClass GetHistory()
		{
			const string StoredProcName = "[usp_ExStarsFilingHistory]";
			using (var cmd = new SqlCommand("[dbo]." + StoredProcName))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@SiteGuid", this.Config.Site.SiteGuid);
				cmd.Parameters.AddWithValue("@ManagerCompanyGuid", this.Config.ManagerCompanyGuid);
				cmd.Parameters.AddWithValue("@FilingStartDate", this.Config.StartTransactionDateTime);
				cmd.Parameters.AddWithValue("@FilingEndDate", this.Config.EndTransactionDateTime);

				DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, this.Config.Security);

				if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return new ExStarsFilingStatusListClass();
				}
				DataTable table = dataSet.Tables[0];
				table.TableName = StoredProcName;
				return LoadStatusResults(table, true);
			}
		}


		public ExStarsFilingClass QueryByDate()
		{
			const string StoredProcedureExStarsTransSelect = "[dbo].[gsp_ExStarsFilingSelect]";
			using (var cmd = new SqlCommand(StoredProcedureExStarsTransSelect))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@SiteGuid", Config.Site.SiteGuid);
				cmd.Parameters.AddWithValue("@ManagerCompanyGuid", Config.ManagerCompanyGuid);
				cmd.Parameters.AddWithValue("@FilingStartDate", Config.StartTransactionDateTime);
				cmd.Parameters.AddWithValue("@FilingEndDate", Config.EndTransactionDateTime);
				cmd.Parameters.AddWithValue("@ReportType", Config.ReportType.ToString());
				cmd.Parameters.AddWithValue("@UnacknowledgedOnly", false);
				DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, Config.Security);

				if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}
				DataTable table = dataSet.Tables[0];
				table.TableName = "gsp_ExStarsFilingSelect";
				return LoadFiling(table.Rows[0]);
			}
		}

		public ExStarsFilingClass QueryByFlingGuid(string filingGuidAsStr)
		{
			const string StoredProcedureExStarsTransSelect = "[dbo].[gsp_ExStarsFilingSelectByGuid]";
			using (var cmd = new SqlCommand(StoredProcedureExStarsTransSelect))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@SiteGuid", Config.Site.SiteGuid);
				cmd.Parameters.AddWithValue("@ManagerCompanyGuid", Config.ManagerCompanyGuid);
				cmd.Parameters.AddWithValue("@ExStarsFilingsGuid", Guid.Parse(filingGuidAsStr));
				DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, Config.Security);

				if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}
				DataTable table = dataSet.Tables[0];
				table.TableName = "gsp_ExStarsFilingSelectByGuid";
				return LoadFiling(table.Rows[0]);
			}
		}




		public ExStarsFilingStatusClass GetLastStatus()
		{
			const string StoredProcName = "[usp_ExStarsFilingStatusSelectLast]";
			using (var cmd = new SqlCommand("[dbo]." + StoredProcName))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@SiteGuid", this.Config.Site.SiteGuid);
				cmd.Parameters.AddWithValue("@ManagerCompanyGuid", this.Config.ManagerCompanyGuid);

				DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, this.Config.Security);

				if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}
				DataTable table = dataSet.Tables[0];
				table.TableName = StoredProcName;
				return LoadExStarsFilingStatusClass(table.Rows[0]);
			}
		}


		private ExStarsFilingStatusListClass LoadStatusResults(DataTable table, bool skipReplaced = false)
		{
			ExStarsFilingStatusListClass list = new ExStarsFilingStatusListClass();

			foreach (DataRow row in table.Rows)
			{
				ExStarsFilingStatusClass data = LoadExStarsFilingStatusClass(row);
				if (!skipReplaced || data.FilingStatus != FileCreatingStatus.Replaced)
				{
					list.Add(data);
				}
			}
			return list;
		}


		private static ExStarsFilingClass LoadFiling(DataRow row)
		{
			ExStarsFilingClass data = new ExStarsFilingClass();
			data.FilingStartDate = DataObject.getValue(row["FilingStartDate"], DateTime.MinValue);
			data.FilingEndDate = DataObject.getValue(row["FilingEndDate"], DateTime.MinValue);

			data.FilingCreated = DataObject.getValue(row["FilingCreated"], DateTimeOffset.MinValue);
			data.FilingSent = DataObject.getValue(row["FilingSent"], DateTimeOffset.MinValue);
			data.ResponseLoaded = DataObject.getValue(row["ResponseLoaded"], DateTimeOffset.MinValue);

			data.ExStarsFilingsGuid = DataObject.getValue(row["ExStarsFilingsGuid"], Guid.Empty);
			data.ManagerCompanyGuid = DataObject.getValue(row["ManagerCompanyGuid"], Guid.Empty);
			data.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
			data.Acknowledgement = DataObject.getValue(row["Acknowledgement"], "");
			data.AckEasyRead = DataObject.getValue(row["AckEasyRead"], "");
			data.InterchangeControlNumber = DataObject.getValue(row["ControlNumber"], "");
			data.TransSetControlNumber = DataObject.getValue(row["TransSetControlNumber"], "");
			data.EasyReadReport = DataObject.getValue(row["EasyReadReport"], "");
			data.EdiReport = DataObject.getValue(row["EdiReport"], "");
			data.RawDataFileName = DataObject.getValue(row["RawDataFileName"], "");
			data.EasyReadFileName = DataObject.getValue(row["EasyReadFileName"], "");

			data.FilingStatusAsStr = DataObject.getValue(row["FilingStatus"], "");
			data.ModifierAsStr = DataObject.getValue(row["Modifier"], "");
			data.OriginalControlNumber = DataObject.getValue(row["OriginalControlNumber"], "");
			data.ReportTypeAsStr = DataObject.getValue(row["ReportType"], "");
			data.SerializedData = DataObject.getValue(row["SerializedData"], "");

			data.UnresolvedErrors = DataObject.getValue(row["UnresolvedErrors"], 0);
			data.UnresolvedWarnings = DataObject.getValue(row["UnresolvedWarnings"], 0);
			return data;
		}


		private ExStarsFilingStatusClass LoadExStarsFilingStatusClass(DataRow row)
		{
			ExStarsFilingStatusClass data = new ExStarsFilingStatusClass(
				  DataObject.getValue(row["FilingStartDate"], DateTime.MinValue)
				, DataObject.getValue(row["FilingEndDate"], DateTime.MinValue)
				, DataObject.getValue(row["ManagerCompanyGuid"], Guid.Empty)
				, DataObject.getValue(row["reportType"], "")
				, DataObject.getValue(row["Modifier"], "")
				, DataObject.getValue(row["ControlNumber"], "")
				, DataObject.getValue(row["TransSetControlNumber"], "")
				, DataObject.getValue(row["OriginalControlNumber"], "")
				, DataObject.getValue(row["FilingStatus"], "")
				, DataObject.getValue(row["FilingCreated"], DateTimeOffset.MinValue)
				, DataObject.getValue(row["FilingSent"], DateTimeOffset.MinValue)
				, DataObject.getValue(row["ResponseLoaded"], DateTimeOffset.MinValue)
				, DataObject.getValue(row["ExStarsFilingsGuid"], Guid.Empty)
				, DataObject.getValue(row["UnresolvedErrors"], 0)
				, DataObject.getValue(row["UnresolvedWarnings"], 0));

			return data;
		}

#if false // not needed

		public FileCreatingStatus FilingExists()
		{
			return this.FilingExists(config.ReportModifier);
		}


		public FileCreatingStatus FilingExists(ReportModifiersEnum modifier)
		{
			string sql = string.Format(
@"SELECT [FilingStatus]
FROM [dbo].[tblExStarsFilings] 
WHERE 
	[ManagerCompanyGuid]='{0}'
	AND [SiteGuid]='{1}'
	AND [FilingStartDate] ='{2}'
	AND [FilingEndDate]='{3}'
	AND [ReportType] =N'{4}'
	AND [Modifier]=N'{5}'
", config.ManagerCompanyGuid
				, config.Site.SiteGuid
				, config.StartTransactionDateTime.ToString("d")
				, config.EndTransactionDateTime.ToString("d")
				, config.ReportType
				, modifier);

			DataRowCollection rowCollection = this.GetDataSet(sql);
			if (rowCollection == null)
			{
				return FileCreatingStatus.NotCreated;
			}
			return  (FileCreatingStatus)Enum.Parse(typeof(FileCreatingStatus), rowCollection[0][0] as string);				
		}

		public ExStarsFilingListClass QueryByDate(bool originalOnly)
		{
			const string StoredProcedureExStarsTransSelect = "[dbo].[gsp_ExStarsFilingSelect]";
			using (var cmd = new SqlCommand(StoredProcedureExStarsTransSelect))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@SiteGuid", config.Site.SiteGuid);
				cmd.Parameters.AddWithValue("@ManagerCompanyGuid", config.ManagerCompanyGuid);
				cmd.Parameters.AddWithValue("@StartDate", config.StartTransactionDateTime);
				cmd.Parameters.AddWithValue("@EndDate", config.EndTransactionDateTime);
				cmd.Parameters.AddWithValue("@Modifier", originalOnly ? ReportModifiersEnum.Original.ToString() : "");

				DataSet dataSet = this.consolidatedDa.GetDataSet(cmd, config.Security);

				if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return null;
				}
				DataTable table = dataSet.Tables[0];
				table.TableName = "gsp_ExStarsFilingSelect";
				return LoadResults(table);
			}
		}


		private ExStarsFilingListClass LoadResults(DataTable table)
		{
			ExStarsFilingListClass list = new ExStarsFilingListClass();

			foreach (DataRow row in table.Rows)
			{
				ExStarsFilingClass data = new ExStarsFilingClass(
					  DataObject.getValue(row["FilingStartDate"], DateTime.MinValue)
					, DataObject.getValue(row["FilingEndDate"], DateTime.MinValue)
					, DataObject.getValue(row["ManagerCompanyGuid"], Guid.Empty)
					, DataObject.getValue(row["SiteGuid"], Guid.Empty)
					, DataObject.getValue(row["reportType"], "")
					, DataObject.getValue(row["IncludeBeginningInventory"], false)
					, DataObject.getValue(row["Modifier"], "")
					, DataObject.getValue(row["ControlNumber"], "")
					, DataObject.getValue(row["TransSetControlNumber"], "")
					, DataObject.getValue(row["OriginalControlNumber"], "")
					, DataObject.getValue(row["FilingStatus"], "")
					, DataObject.getValue(row["FilingCreated"], DateTimeOffset.MinValue)
					, DataObject.getValue(row["FilingSent"], DateTimeOffset.MinValue)
					, DataObject.getValue(row["ResponseLoaded"], DateTimeOffset.MinValue)
					, DataObject.getValue(row["ediReport"], "")
					, DataObject.getValue(row["easyReadReport"], "")
					, DataObject.getValue(row["serializedData"], "")
					, DataObject.getValue(row["ExStarsFilingsGuid"], Guid.Empty));

				list.Add(data);
			}
			return list;
		}
#endif

		public void DeleteEntry()
		{
			HasRightToInsertUpdate();
			string sql = string.Format(
				"DELETE from [dbo].[tblExStarsFilings]  " +
				"WHERE  [FilingStartDate]='{0}'  " +
				"AND [FilingEndDate]='{1}'  " +
				"AND [ManagerCompanyGuid]='{2}'  " +
				"AND [SiteGuid]='{3}' " +
				"AND [ReportType]='{4}' " +
				"AND [Modifier]='{5}' "
				, this.Config.StartTransactionDateTime.ToString("d")
				, this.Config.EndTransactionDateTime.ToString("d")
				, this.Config.ManagerCompanyGuid
				, this.Config.SiteGuid
				, this.Config.ReportType
				, this.Config.ReportModifier
				);
			using (var cmd = new SqlCommand(sql))
			{
				cmd.CommandType = CommandType.Text;
				this.ConsolidatedDa.ExecuteQueryWithoutSessionContext(this.Config.Security, cmd);
			}
		}

#region Insert into database
		
		public void InsertFilingRecord(
			  DateTime filingStartDate
			, DateTime filingEndDate
			, Guid managerCompanyGuid
			, Guid siteGuid
			, string interchangeControlNumber
			, string originalControlNumber
			, string transSetControlNumber
			, ReportTypeEnum reportType
			, ReportModifiersEnum modifier
			, FileCreatingStatus filingStatus
			, string rawDataFileName
			, string easyReadFileName
			, string ediReport
			, string easyReadReport
			, string serializedData
			, DateTimeOffset filingCreated
			, DateTimeOffset filingSent
			)
		{
			HasRightToInsertUpdate();
			using (var cmd = new SqlCommand("[dbo].[usp_ExStarsFilingsInsert]"))
			{
				try
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@FilingStartDate", filingStartDate);
					cmd.Parameters.AddWithValue("@FilingEndDate", filingEndDate);
					cmd.Parameters.AddWithValue("@ManagerCompanyGuid", managerCompanyGuid);
					cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
					cmd.Parameters.AddWithValue("@ControlNumber", interchangeControlNumber);
					cmd.Parameters.AddWithValue("@OriginalControlNumber", originalControlNumber ?? "");
					cmd.Parameters.AddWithValue("@TransSetControlNumber", transSetControlNumber);
					cmd.Parameters.AddWithValue("@ReportType", reportType.ToString());
					cmd.Parameters.AddWithValue("@Modifier", modifier.ToString());
					cmd.Parameters.AddWithValue("@FilingStatus", FileCreatingStatus.Created.ToString());
					cmd.Parameters.AddWithValue("@RawDataFileName", rawDataFileName);
					cmd.Parameters.AddWithValue("@EasyReadFileName", easyReadFileName);
					cmd.Parameters.AddWithValue("@EdiReport", ediReport); // truncString(ediReport, maxLen));
					cmd.Parameters.AddWithValue("@EasyReadReport", easyReadReport);
					cmd.Parameters.AddWithValue("@SerializedData", serializedData);
                    cmd.Parameters.AddWithValue("@FilingCreated", filingCreated);
                    cmd.Parameters.AddWithValue("@FilingSent", filingSent);
                    cmd.Parameters.AddWithValue("@UpdatedBy", this.Config.Security.UserID);
					this.ConsolidatedDa.ExecuteQueryWithoutSessionContext(this.Config.Security, cmd);
				}
				catch (Exception e)
				{
					throw new ExStarsSqlException(e, "SQL error: {0}", cmd.CommandText);
				}
			}
		}


		public void InsertFilingRecord(ExStarsFilingClass filingRow)
		{
			InsertFilingRecord(
				  filingRow.FilingStartDate
				, filingRow.FilingEndDate
				, filingRow.ManagerCompanyGuid
				, filingRow.SiteGuid
				, filingRow.InterchangeControlNumber
				, filingRow.OriginalControlNumber
				, filingRow.TransSetControlNumber
				, filingRow.ReportType
				, filingRow.Modifier
				, filingRow.FilingStatus
				, filingRow.RawDataFileName
				, filingRow.EasyReadFileName
				, filingRow.EdiReport
				, filingRow.EasyReadReport
				, filingRow.SerializedData
				, filingRow.FilingCreated
				, filingRow.FilingSent
				);
		}

		
		public void InsertFilingRecord(string ediFilePath, string easyReadFilePath, string ediReport, string easyReadReport, string serializedData)
		{
			InsertFilingRecord(
				  this.Config.StartTransactionDateTime
				, this.Config.EndTransactionDateTime
				, this.Config.ManagerCompanyGuid
				, this.Config.SiteGuid
				, this.Config.InterchangeControlNumber
				, this.Config.OriginalTransSetControlNumber
				, this.Config.TransSetControlNumber
				, this.Config.ReportType
				, this.Config.ReportModifier
				, FileCreatingStatus.Created
				, ediFilePath
				, easyReadFilePath
				, ediReport
				, easyReadReport
				, serializedData
				, DateTimeOffset.Now
				, ExStarsConstants.BeginningOfDateTimeOffset
				);
		}
 
	#endregion	
	}
}
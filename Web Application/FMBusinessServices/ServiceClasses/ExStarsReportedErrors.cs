
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;


	/// <summary>
	/// Record and look up whether for a particular site, manager and date an ExSTARS report has been
	/// created and sent to the IRS
	/// </summary>
	public class ExStarsReportedErrors : ExStarsSqlBase
	{
		#region Constants and Fields

		private const string TableName = "tblExStarsReportedErrors";

		#endregion

		public ExStarsReportedErrors(ExStarsSiteConfigExpanded config) :base(config)
		{
		}



		// ref: C_Rec_ExSTARS_Errors::SetAllErrorsAsCorrected ~ 213
		// ref: CExSTARS_ExportDlg::RecordFiling ~ 1558
		/// <summary>
		/// Call when the 151 file says that there are zero errors or when creating a replacement file
		/// </summary>
		/// <param name="transSetControlNumber"></param>
		public void SetAllErrorsAsCorrected(string transSetControlNumber)
		{
			string sql = string.Format(
				"UPDATE [dbo].[tblExStarsReportedErrors] " +
				"WHERE [TransSetControlNumber]='{0}' " +
				"SET ErrorCorrected=1, [UpdatedDate]=GETDATE(), [UpdatedBy]='{1}'"
				, transSetControlNumber
				, this.Config.Security.UserID);
			this.ExecuteNonQuery(sql);
		}



		public void ErrorCorrectedUpdate(Guid exStarsReportedErrorsGuid, bool isCorrected = true)
		{
			string sql = string.Format(
				"UPDATE [dbo].[tblExStarsReportedErrors] "+
				"WHERE [ExStarsReportedErrorsGuid]='{0}' " +
				"SET ErrorCorrected={1}, [UpdatedDate]=GETDATE(), [UpdatedBy]='{2}'"
				, exStarsReportedErrorsGuid
				, isCorrected? 1 : 0
				, this.Config.Security.UserID);
			this.ExecuteNonQuery(sql);				
		}


		public ExStarsReportedErrorClass GetByTransSetControlNumber(string transSetControlNumber)
		{
			DataRow row = this.QueryByTransSetControlNumber(transSetControlNumber);
			if (row == null)
			{
				return null;
			}
			row.Table.TableName = TableName;

			ExStarsReportedErrorClass data = new ExStarsReportedErrorClass();

			data.ManagerCompanyGuid = DataObject.getValue(row["ManagerCompanyGuid"], Guid.Empty);
			data.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
			data.ExStarsFilingsGuid = DataObject.getValue(row["ExStarsFilingsGuid"], Guid.Empty);
			data.SequenceNumber = DataObject.getValue(row["SequenceNumber"], "");
			data.MustCorrect = DataObject.getValue(row["MustCorrect"], false);
			data.PBI01_Primary = DataObject.getValue(row["PBI01_Primary"], "");
			data.PBI01_Secondary = DataObject.getValue(row["PBI01_Secondary"], "");
			data.PBI03_Primary = DataObject.getValue(row["PBI03_Primary"], "");
			data.PBI03_Secondary = DataObject.getValue(row["PBI03_Secondary"], "");
			data.PBI04 = DataObject.getValue(row["PBI04"], "");
			data.OriginalValue = DataObject.getValue(row["OriginalValue"], "");
			data.IrsErrorText = DataObject.getValue(row["IrsErrorText"], "");
			data.ErrorCorrected = DataObject.getValue(row["ErrorCorrected"], false);
			//data.CreatedBy = DataObject.getValue(row["CreatedBy"], "");
			//data.UpdatedBy = DataObject.getValue(row["UpdatedBy"], "");
			data.ExStarsReportedErrorsGuid = DataObject.getValue(row["ExStarsReportedErrorsGuid"], Guid.Empty);

			return data;
		}


		protected DataRow QueryByTransSetControlNumber(string transSetControlNumber)
		{
			string sql = string.Format(
				"select * from [dbo].[tblExStarsReportedErrors] WHERE [TransSetControlNumber]='{0}'"
				, transSetControlNumber);
			try
			{
				using (var cmd = new SqlCommand(sql))
				{
					cmd.CommandType = CommandType.Text;
					DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, this.Config.Security);
					return dataSet.Tables.Count != 0 && dataSet.Tables[0].Rows.Count != 0
						       ? dataSet.Tables[0].Rows[0]
						       : null;
				}
			}
			catch (Exception e)
			{
				throw new ExStarsSqlException(e, "SQL error: {0}", sql);
			}			
		}

		public void DeleteEntry(string transSetControlNumber)
		{
			HasRightToInsertUpdate();
			string sql = string.Format(
				"DELETE from [dbo].[tblExStarsReportedErrors] WHERE  [TransSetControlNumber]='{0}'"
				, transSetControlNumber);
			using (var cmd = new SqlCommand(sql))
			{
				cmd.CommandType = CommandType.Text;
				ConsolidatedDa.ExecuteQueryWithoutSessionContext(Config.Security, cmd);
			}
		}

		public void InsertErrorRecord ( ExStarsReportedErrorClass  reportedError )
		{
			using (var cmd = new SqlCommand("[dbo].[gsp_ExStarsReportedErrorsInsert]"))
			{
				try
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@ManagerCompanyGuid", reportedError.ManagerCompanyGuid);
					cmd.Parameters.AddWithValue("@SiteGuid", reportedError.SiteGuid);
					cmd.Parameters.AddWithValue("@ExStarsFilingsGuid", reportedError.ExStarsFilingsGuid);
					cmd.Parameters.AddWithValue("@SequenceNumber", reportedError.SequenceNumber);
					cmd.Parameters.AddWithValue("@MustCorrect", reportedError.MustCorrect);
					cmd.Parameters.AddWithValue("@PBI01_Primary", reportedError.PBI01_Primary);
					cmd.Parameters.AddWithValue("@PBI01_Secondary", reportedError.PBI01_Secondary);
					cmd.Parameters.AddWithValue("@PBI03_Primary", reportedError.PBI03_Primary);
					cmd.Parameters.AddWithValue("@PBI03_Secondary", reportedError.PBI03_Secondary);
					cmd.Parameters.AddWithValue("@PBI04", reportedError.PBI04);
					cmd.Parameters.AddWithValue("@OriginalValue", reportedError.OriginalValue);
					cmd.Parameters.AddWithValue("@IrsErrorText", reportedError.IrsErrorText);
					cmd.Parameters.AddWithValue("@ErrorCorrected", reportedError.ErrorCorrected);
					cmd.Parameters.AddWithValue("@UpdatedBy", this.Config.Security.UserID);
					ExecuteNonQuery( cmd);
				}
				catch (Exception e)
				{
					throw new ExStarsSqlException(e, "SQL error: {0}", cmd.CommandText);
				}
			}
		}
	}
}
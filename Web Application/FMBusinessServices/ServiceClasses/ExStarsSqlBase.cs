

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessServices.DataAccessLayer;

	public abstract class ExStarsSqlBase
	{
		#region Constants and Properties
		protected readonly ConsolidatedDAClass ConsolidatedDa = new ConsolidatedDAClass();
		protected readonly ExStarsSiteConfigExpanded Config = null;

		#endregion

		protected ExStarsSqlBase(ExStarsSiteConfigExpanded config)
		{
			this.Config = config;
			HasRightToView();
		}

	
		protected void HasRightToInsertUpdate()
		{
			// If a user has the authority to view ExSTARS reports, it is presumed they have the right to see the
			// company data on that report.
			if (!this.Config.Security.HasRight(RIGHT.CREATE_IRS_EXSTARS_REPORT))
			{
				throw new FMInsufficientRightsException();
			}
		}
		protected void HasRightToView()
		{
			// If a user has the authority to view ExSTARS reports, it is presumed they have the right to see the
			// company data on that report.
			if (!this.Config.Security.HasRight(RIGHT.VIEW_IRS_EXSTARS_REPORT))
			{
				throw new FMInsufficientRightsException();
			}
		}

		protected DataRowCollection GetDataSet(string sql, string tableName = "tblExStarsDataSet")
		{
			try
			{
				using (var cmd = new SqlCommand(sql))
				{
					cmd.CommandType = CommandType.Text;
					DataSet dataSet = this.ConsolidatedDa.GetDataSet(cmd, this.Config.Security);
					if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
					{
						return null;
					}
					dataSet.Tables[0].TableName = tableName;
					return dataSet.Tables[0].Rows;
				}
			}
			catch (Exception e)
			{
				throw new ExStarsSqlException(e, "SQL error: {0}", sql);
			}
		}

		protected void ExecuteNonQuery(string sql)
		{
			HasRightToInsertUpdate();
			try
			{
				using (var cmd = new SqlCommand(sql))
				{
					cmd.CommandType = CommandType.Text;
					this.ConsolidatedDa.ExecuteQueryWithoutSessionContext(this.Config.Security, cmd);
				}
			}
			catch (Exception e)
			{
				throw new ExStarsSqlException(e, "SQL error: {0}", sql);
			}
		}

		protected void ExecuteNonQuery(SqlCommand cmd)
		{
			HasRightToInsertUpdate();
			try
			{
				this.ConsolidatedDa.ExecuteQueryWithoutSessionContext(this.Config.Security, cmd);
			}
			catch (Exception e)
			{
				throw new ExStarsSqlException(e, "SQL error: {0}", cmd.CommandText, cmd.Parameters);
			}
		}

 
	}
}
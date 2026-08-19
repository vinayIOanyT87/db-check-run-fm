using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.BusinessInterfaces;

namespace FMBusinessServices.ServiceClasses
{
    using FMBusinessObjects.Exceptions;

    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ArchiveDataProcessorClass : IArchiveDataProcessor
	{
		private SecurityClass Security;

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public string Process(ArchiveDataSR sr)
		{

			Security = sr.Security;

			string DBName = System.Configuration.ConfigurationManager.AppSettings["ArchiveDBName"];
			string sInfo = "";

			using (SqlCommand cmd = new SqlCommand())
			{

				cmd.CommandText = "dbo.usp_SystemDataArchive";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@start_date", SqlDbType.DateTime);
				cmd.Parameters.Add("@end_date", SqlDbType.DateTime);
				cmd.Parameters.Add("@data_selected", SqlDbType.NVarChar,64);

				cmd.Parameters["@start_date"].Value = sr.StartDate.ToString("d");
				cmd.Parameters["@end_date"].Value = sr.EndDate.ToString("d");

				string sRet = "";
				if (sr.CheckAccounting == true)
				{
					cmd.Parameters["@data_selected"].Value = "ACCOUNTING";
					sRet = ExecuteQuery(cmd);

					if (string.IsNullOrEmpty(sRet))
						sInfo += "Archive Accounting Data is successful";
					else
						sInfo += sRet;
					sInfo += "\n";
				}

				if (sr.CheckQC == true)
				{
					cmd.Parameters["@data_selected"].Value = "QUALITY CONTROL";
					sRet = ExecuteQuery(cmd);

					if (string.IsNullOrEmpty(sRet))
						sInfo += "Archive Quality Control Data is successful";
					else
						sInfo += sRet;
					sInfo += "\n";
				}
				if (sr.CheckMaintenance == true)
				{
					cmd.Parameters["@data_selected"].Value = "MAINTENANCE";
					sRet = ExecuteQuery(cmd);
					if (string.IsNullOrEmpty(sRet))
						sInfo += "Archive Maintenance Data is successful";
					else
						sInfo += sRet;
					sInfo += "\n";
				}

				if (sr.CheckAlarm == true)
				{
					cmd.Parameters["@data_selected"].Value = "ALARM LOG";
					sRet = ExecuteQuery(cmd);
					if (string.IsNullOrEmpty(sRet))
						sInfo += "Archive Alarm and Event Data is successful";
					else
						sInfo += sRet;
					sInfo += "\n";
				}

				if (sr.CheckAudit == true)
				{
					cmd.Parameters["@data_selected"].Value = "AUDIT LOG";
					sRet = ExecuteQuery(cmd);
					if (string.IsNullOrEmpty(sRet))
						sInfo += "Archive Audit Data is successful";
					else
						sInfo += sRet;
					sInfo += "\n";

				}
			}

			return sInfo;

		}

		private string ExecuteQuery(SqlCommand cmd)
		{
			ConsolidatedDAClass da = new ConsolidatedDAClass();
			DataSet errorsDataSet = null;
			string sRet = "";

			try
			{
				cmd.CommandTimeout = 600;
				errorsDataSet = da.GetDataSet(cmd, Security);
				if (null != errorsDataSet &&
				errorsDataSet.Tables.Count == 1 &&
				errorsDataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in errorsDataSet.Tables[0].Rows)
					{
						if ((row.IsNull("Status") ? "" : row["Status"] as string) == "Error")
						{
							sRet += row.IsNull("Info") ? "" : row["Info"] as string;
							sRet += " \n";
						}
					}
				}
			}
			catch (ConsolidatedDAException ex)
			{
				sRet = "Archiving Data failed. " + ex.Message;
				//         ErrorHandler(ex);
				//         logger.Error("Archiving Database failed. " + ex.Message);
				//         System.Diagnostics.Trace.WriteLine(String.Format("Archiving Database failed. {0}", ex.Message));
			}
			catch (SqlException ex)
			{
				sRet = "Archiving Data failed. " + ex.Message;
				//        ErrorHandler(ex);
				//        logger.Error("Archiving Database failed. " + ex.Message);
				//        System.Diagnostics.Trace.WriteLine(String.Format("Archiving Database failed. {0}", ex.Message));
			}

			catch (Exception ex)
			{
				sRet = "Archiving Data failed. " + ex.Message;
				//        ErrorHandler(ex);
				//        logger.Error("Archiving Database failed. " + ex.Message);
				//        System.Diagnostics.Trace.WriteLine(String.Format("Archiving Database failed. {0}", ex.Message));
			}
			return sRet;
		}

	}

}
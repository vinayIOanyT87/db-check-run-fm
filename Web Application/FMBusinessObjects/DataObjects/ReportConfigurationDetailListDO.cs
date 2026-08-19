/// <summary>
/// File name:	ReportConfigurationDetailListDO.cs
/// Purpose:	The purpose is to contain all the report configuration report detail list
///				information.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------	--------------------	----------------------------------
///		2005/7/15	Richard Panachida		Added feature to auto print reports
///		11/09/2005	W.Gray					Changed ReportUrl to ReportPath
///		03/31/2009	W.Gray					7.4.6.0 - Added paramter UserIndex to SQLGetAllNonPrintReportDetails (CSI 2500)
///		
/// </summary>
/// 
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[DataContract]
	[KnownType(typeof(ReportConfigurationDetailDO))]
	public class ReportConfigurationDetailListDO : DataObject
	{
		#region Private Attributes
		private List<ReportConfigurationDetailDO> reportDetailDOList;

		private const string SQL_SELECT_STD = "SELECT dbo.tblReportDetails.ReportDetailGuid, dbo.tblReportDetails.ReportName, dbo.tblReportDetails.ReportPath," +
			" dbo.tblReportDetails.ReportDescription, dbo.tblReportDetails.ReportGroupGuid, dbo.tblReportDetails.SiteGuid, dbo.tblReportDetails.CreatedBy," +
			" dbo.tblReportDetails.CreatedDate, dbo.tblReportDetails.UpdatedBy, dbo.tblReportDetails.UpdatedDate, dbo.tblReportDetails.OrderNumber," +
			" dbo.tblReportDetails.PrintOnlyFlag, dbo.tblReportDetails.PrimaryPrinterName, dbo.tblReportDetails.SecondaryPrinterName," +
			" dbo.tblReportDetails.PrintAtEndOfDay, dbo.tblReportDetails.PrintAtEndOfMonth, dbo.tblReportDetails.DWReportFlag, dbo.tblSites.ReportDirectory ";
		private const string SQL_FROM_STD = "FROM dbo.tblReportDetails inner join dbo.tblSites on dbo.tblReportDetails.SiteGuid = dbo.tblSites.SiteGuid ";
		private const string SQL_ORDERBY_STD = "ORDER BY dbo.tblReportDetails.OrderNumber";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report configuration detail list data object.
		/// </summary>
		public ReportConfigurationDetailListDO()
		{
			this.init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns a list of report detail DOs.
		/// </summary>
		[DataMember]
		public List<ReportConfigurationDetailDO> ReportDetailDOList
		{
			get { return this.reportDetailDOList; }
			private set { this.reportDetailDOList = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method returns a SqlCommand to retrieve all the report detail records.
		/// </summary>
		/// <returns></returns>
		public SqlCommand SQLGetAllReportDetails(Guid siteGuid)
		{
			SqlCommand cmd = new SqlCommand();
			string where = "WHERE ((dbo.tblReportDetails.SiteGuid IN ( SELECT map.tblEntityReportConfigurationSettingsToSite.SiteGuid" +
				" from map.tblEntityReportConfigurationSettingsToSite where MapToSiteGuid = @SiteGuid)) OR (dbo.tblReportDetails.SiteGuid = @SiteGuid)) ";
			cmd.CommandText = SQL_SELECT_STD + SQL_FROM_STD + where + SQL_ORDERBY_STD;
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			return cmd;
		}

		/// <summary>
		/// This method returns a SqlCommand to retrieve all the report detail records that are not
		/// print only and are authorized for a particular user by virtue of the User to User Group map
		/// and the User Group to Report Map.
		/// </summary>
		/// <returns></returns>
		public SqlCommand SQLGetAllNonPrintReportDetails(Guid siteGuid, Guid userGuid)
		{
			SqlCommand cmd = new SqlCommand();
			string where = "WHERE ((dbo.tblReportDetails.SiteGuid IN ( SELECT map.tblEntityReportConfigurationSettingsToSite.SiteGuid" +
				" from map.tblEntityReportConfigurationSettingsToSite where MapToSiteGuid = @SiteGuid)) OR (dbo.tblReportDetails.SiteGuid = @SiteGuid))" +
				" AND PrintOnlyFlag = @PrintOnlyFlag  AND ReportDetailGuid IN (SELECT ReportDetailGuid FROM map.tblGroupToReportDetail WHERE GroupGuid" +
				" IN (SELECT GroupGuid FROM map.tblUserToGroup  WHERE UserGuid = @UserGuid ))";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@PrintOnlyFlag", 0);
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);
			cmd.CommandText = SQL_SELECT_STD + SQL_FROM_STD + where + SQL_ORDERBY_STD;
			return cmd;
		}


		/// <summary>
		/// This method returns a SqlCommand to retrieve all the report detail records 
		/// </summary>
		/// <returns></returns>
		private SqlCommand SQLGetAllPrintReportDetails(Guid siteGuid, string printFieldName)
		{
			const string PARAM_NAME_SITEGUID = "@SiteGuid";
			const SqlDbType PARAM_TYPE_SITEGUID = SqlDbType.UniqueIdentifier;
			const string PARAM_NAME_PRINTFLAG = "@PrintFlag";
			const SqlDbType PARAM_TYPE_PRINTFLAG = SqlDbType.Bit;

			SqlCommand cmd = new SqlCommand();
			string where = AddParameter(cmd, "WHERE", "dbo.tblSites.SiteGuid", "=", PARAM_NAME_SITEGUID, PARAM_TYPE_SITEGUID, siteGuid) +
								AddParameter(cmd, true, printFieldName, PARAM_NAME_PRINTFLAG, PARAM_TYPE_PRINTFLAG, true);
			cmd.CommandText = SQL_SELECT_STD + SQL_FROM_STD + where + SQL_ORDERBY_STD;
			return cmd;
		}

		/// <summary>
		/// This method returns a SqlCommand to retrieve all the report detail records that are print
		/// at EndOfDay.
		/// </summary>
		/// <returns></returns>
		public SqlCommand SQLGetAllPrintAtEndOfDayReportDetails(Guid siteGuid)
		{
			return SQLGetAllPrintReportDetails(siteGuid, "PrintAtEndOfDay");

		}

		/// <summary>
		/// This method returns a SQL string to retrieve all the report detail records that are print
		/// at EndOfMonth.
		/// </summary>
		/// <returns></returns>
		public SqlCommand SQLGetAllPrintAtEndOfMonthReportDetails(Guid siteGuid)
		{
			return SQLGetAllPrintReportDetails(siteGuid, "PrintAtEndOfMonth");
		}

		public override string getSelectCommand() { return null; }

		public override string getInsertCommand() { return null; }

		public override string getDeleteCommand() { return null; }

		public override string getUpdateCommand() { return null; }

		#endregion

		#region Public SQL Load Methods
		/// <summary>
		/// This method is a bypass method that calls the load all reports.  The only reason
		/// it exists for for clarity.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		public bool SQLLoadAllNonPrintReportDetails(System.Data.DataSet dataSet)
		{
			return this.SQLLoadAllReportDetails(dataSet);
		}

		/// <summary>
		/// This method loads all the report detail records into a collection.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		public bool SQLLoadAllReportDetails(System.Data.DataSet dataSet)
		{
			bool successful = false;

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table != null)
				{
					System.Data.DataRowCollection rowCollection = table.Rows;

					foreach (System.Data.DataRow row in rowCollection)
					{
						ReportConfigurationDetailDO reportDetail = new ReportConfigurationDetailDO();

						reportDetail.ReportGuid = DataObject.getValue<Guid>(row["ReportDetailGuid"], Guid.Empty);
						reportDetail.ReportName = DataObject.getValue<string>(row["ReportName"], "");
						reportDetail.ReportPath = DataObject.getValue<string>(row["ReportPath"], "");
						reportDetail.ReportDescription = DataObject.getValue<string>(row["ReportDescription"], "");
						reportDetail.ReportGroupGuid = DataObject.getValue<Guid>(row["ReportGroupGuid"], Guid.Empty);
						reportDetail.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guids.SiteAdminGuid);
						reportDetail.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
						reportDetail.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
						reportDetail.UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
						reportDetail.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], reportDetail.CreatedDate);
						reportDetail.OrderNumber = DataObject.getValue<int>(row["OrderNumber"], 99);
						reportDetail.ForPrintingOnly = DataObject.getValue<bool>(row["PrintOnlyFlag"], false);
						reportDetail.PrimaryPrinterName = DataObject.getValue<string>(row["PrimaryPrinterName"], "");
						reportDetail.SecondaryPrinterName = DataObject.getValue<string>(row["SecondaryPrinterName"], "");
						reportDetail.PrintAtEndOfDay = DataObject.getValue<bool>(row["PrintAtEndOfDay"], false);
						reportDetail.PrintAtEndOfMonth = DataObject.getValue<bool>(row["PrintAtEndOfMonth"], false);
						reportDetail.DWReportFlag = DataObject.getValue<bool>(row["DWReportFlag"], false);
						reportDetail.ReportDirectory = DataObject.getValue<string>(row["ReportDirectory"], "/Standard Reports");

						this.reportDetailDOList.Add(reportDetail);

						successful = true;
					}
				}
			}

			return successful;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize thre report configuration detail list data object to its initial state.
		/// </summary>
		private void init()
		{
			this.reportDetailDOList = new List<ReportConfigurationDetailDO>();
		}
		#endregion
	}
}

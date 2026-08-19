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
using System.Collections;

namespace ReportingServices
{
	/// <summary>
	/// Summary description for ReportConfigurationDetailListDO.
	/// </summary>
	public class ReportConfigurationDetailListDO : DataObjectBase
	{
		#region Private Attributes
		private ArrayList reportDetailDOList;
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
		public System.Collections.ArrayList ReportDetailDOList
		{
			get { return this.reportDetailDOList; }
		}
		#endregion

		#region Public SQL Methods
		/// <summary>
		/// This method returns a SQL string to retrieve all the report detail records.
		/// </summary>
		/// <returns></returns>
		public string SQLGetAllReportDetails(int siteIndex)
		{
			string select  = "SELECT ReportIndex, ReportName, ReportPath, ReportDescription, GroupIndex, " +
				"SiteIndex, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, OrderNumber, " +
				"PrintOnlyFlag, PrimaryPrinterName, SecondaryPrinterName, PrintAtEndOfDay, " +
				"PrintAtEndOfMonth ";
			string from    = "FROM tblReportDetails ";
			string where   = "WHERE SiteIndex = " + siteIndex + " ";
			string orderBy = "ORDER BY OrderNumber";

			return (select + from + where + orderBy);
		}

		/// <summary>
		/// This method returns a SQL string to retrieve all the report detail records that are not
		/// print only and are authorized for a particular user by virtue of the User to User Group map
		/// and the User Group to Report Map.
		/// </summary>
		/// <returns></returns>
		public string SQLGetAllNonPrintReportDetails(int siteIndex,int UserIndex)
		{
			string select  = "SELECT ReportIndex, ReportName, ReportPath, ReportDescription, GroupIndex, " +
				"SiteIndex, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, OrderNumber, " +
				"PrintOnlyFlag, PrimaryPrinterName, SecondaryPrinterName, PrintAtEndOfDay, " +
				"PrintAtEndOfMonth ";
			string from    =	" FROM tblReportDetails";
			string where   =	" WHERE SiteIndex = " + siteIndex + " AND PrintOnlyFlag = 0 "+
									" AND ReportIndex IN (SELECT ReportIndex FROM tblGroupReportMap WHERE"+
									" GroupIndex IN (SELECT GroupIndex FROM tblUserGroupMap WHERE UserIndex = "+UserIndex.ToString()+"))";
			string orderBy =	" ORDER BY OrderNumber";

			return (select + from + where + orderBy);
		}

		/// <summary>
		/// This method returns a SQL string to retrieve all the report detail records that are print
		/// at EndOfDay.
		/// </summary>
		/// <returns></returns>
		public string SQLGetAllPrintAtEndOfDayReportDetails(int siteIndex)
		{
			string select  = "SELECT ReportIndex, ReportName, ReportPath, ReportDescription, GroupIndex, " +
				"SiteIndex, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, OrderNumber, " +
				"PrintOnlyFlag, PrimaryPrinterName, SecondaryPrinterName, PrintAtEndOfDay, " +
				"PrintAtEndOfMonth ";
			string from    = "FROM tblReportDetails ";
			string where   = "WHERE SiteIndex = " + siteIndex + " AND PrintAtEndOfDay = 1 ";
			string orderBy = "ORDER BY OrderNumber";

			return (select + from + where + orderBy);
		}

		/// <summary>
		/// This method returns a SQL string to retrieve all the report detail records that are print
		/// at EndOfMonth.
		/// </summary>
		/// <returns></returns>
		public string SQLGetAllPrintAtEndOfMonthReportDetails(int siteIndex)
		{
			string select  = "SELECT ReportIndex, ReportName, ReportPath, ReportDescription, GroupIndex, " +
				"SiteIndex, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, OrderNumber, " +
				"PrintOnlyFlag, PrimaryPrinterName, SecondaryPrinterName, PrintAtEndOfDay, " +
				"PrintAtEndOfMonth ";
			string from    = "FROM tblReportDetails ";
			string where   = "WHERE SiteIndex = " + siteIndex + " AND PrintAtEndOfMonth = 1 ";
			string orderBy = "ORDER BY OrderNumber";

			return (select + from + where + orderBy);
		}


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

						reportDetail.ReportIndex          = base.getLong(row[0]);
						reportDetail.ReportName           = base.getString(row[1]);
						reportDetail.ReportPath           = base.getString(row[2]);
						reportDetail.ReportDescription    = base.getString(row[3]);
						reportDetail.GroupIndex           = base.getLong(row[4]);
						reportDetail.SiteIndex            = base.getInt(row[5]);
						reportDetail.CreatedBy            = base.getString(row[6]);
						reportDetail.UpdatedBy            = base.getString(row[8]);
						reportDetail.OrderNumber          = base.getInt(row[10]);
						reportDetail.ForPrintingOnly      = base.getBool(row[11]);
						reportDetail.PrimaryPrinterName   = base.getString(row[12]);
						reportDetail.SecondaryPrinterName = base.getString(row[13]);
						reportDetail.PrintAtEndOfDay      = base.getBool(row[14]);
						reportDetail.PrintAtEndOfMonth    = base.getBool(row[15]);

						if (base.IsDateValid(row[7]) == true)
							reportDetail.CreatedDate = base.getDateTime(row[7]);

						if (base.IsDateValid(row[9]) == true)
							reportDetail.UpdatedDate = base.getDateTime(row[9]);

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
			this.reportDetailDOList = new ArrayList();
		}
		#endregion
	}
}

/// <summary>
/// File name:	ReportConfigurationGroupListDO.cs
/// Purpose:	The purpose is to contain all the report configuration group
///				information.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			   By:						Reason:
///		----------		--------------------	----------------------------------
///		2009-03-05     Richard Panachida    Defect 877: Added code to handle if a user does not have finance rights.
/// </summary>
/// 
using System;
using System.Collections;
using FMCommon;

namespace ReportingServices
{
	/// <summary>
	/// Summary description for ReportConfigurationGroupListDO.
	/// </summary>
	public class ReportConfigurationGroupListDO : DataObjectBase
	{
		#region Private Attributes
		private ArrayList reportGroupDOList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report configuration group list data objects.
		/// </summary>
		public ReportConfigurationGroupListDO()
		{
			this.init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns a list of report group DOs.
		/// </summary>
		public System.Collections.ArrayList ReportGroupDOList
		{
			get { return this.reportGroupDOList; }
		}
		#endregion

		#region Public SQL Methods
		/// <summary>
		/// This method returns a SQL report group select statement with no where clause.
		/// </summary>
		/// <returns></returns>
		public string SQLGetAllReportGroups(int siteIndex)
		{
			string select  = "SELECT GroupIndex, GroupName, SiteIndex, CreatedBy, CreatedDate, UpdatedBy, " +
				              "UpdatedDate, OrderNumber, SecurityRight ";
			string from    = "FROM tblReportGroups ";
			string where   = "WHERE SiteIndex = " + siteIndex + " ";
			string orderBy = "ORDER BY OrderNumber";

			return (select + from + where + orderBy);
		}
		#endregion

		#region Public SQL Load Methods
		/// <summary>
		/// This method loads all the report group records into a collection.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		public bool SQLLoadAllReportGroups(System.Data.DataSet dataSet, SecurityClass security)
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
						ReportConfigurationGroupDO reportGroup = new ReportConfigurationGroupDO();

                  reportGroup.GroupIndex        = base.getLong(row["GroupIndex"]);
                  reportGroup.GroupName         = base.getString(row["GroupName"]);
                  reportGroup.SiteIndex         = base.getInt(row["SiteIndex"]);
                  reportGroup.CreatedBy         = base.getString(row["CreatedBy"]);
                  reportGroup.UpdatedBy         = base.getString(row["UpdatedBy"]);
                  reportGroup.OrderNumber       = base.getInt(row["OrderNumber"]);
                  reportGroup.SecurityRightEnum = base.getInt(row["SecurityRight"]);


                  if (base.IsDateValid(row["CreatedDate"]) == true)
                  {
                     reportGroup.CreatedDate = base.getDateTime(row["CreatedDate"]);
                  }

                  if (base.IsDateValid(row["UpdatedDate"]) == true)
                  {
                     reportGroup.UpdatedDate = base.getDateTime(row["UpdatedDate"]);
                  }

                  if (reportGroup.SecurityRightEnum == -1)
                  {
                     reportGroup.SecurityRight = ReportConfigurationGroupDO.ALL_VIEW_RIGHT;
                  }
                  else
                  {
                     reportGroup.SecurityRight = security.RightID((RIGHT)reportGroup.SecurityRightEnum);
                  }

						this.ReportGroupDOList.Add(reportGroup);

						successful = true;
					}
				}
			}

			return successful;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize thre report configuration group list data object to its initial state.
		/// </summary>
		private void init()
		{
			this.reportGroupDOList  = new ArrayList();
		}
		#endregion
	}
}

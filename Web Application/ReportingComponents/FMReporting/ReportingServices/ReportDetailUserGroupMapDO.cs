/// <summary>
/// File name:	ReportDetailUserGroupMapDO.cs
/// Purpose:	The purpose is to contain the mapping between a report configuration detail
///				and a User Group and	to house the SQL statement to enumerate, insert, or delete a row
///				in the database.
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	W.Gray
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------	--------------------	----------------------------------
/// </summary>
/// 
using System;
using System.Collections;
using System.Data;

namespace ReportingServices
{
	/// <summary>
	/// Summary description for ReportDetailUserGroupMapDO.
	/// </summary>
	public class ReportDetailUserGroupMapDO
	{
		#region Private Attributes
		private int			groupIndex;
		private string		groupID;
		private string		createdBy;
		private DateTime	createdDate;
		#endregion


		#region Constructors
		/// <summary>
		/// This is the default constructor for the Report Detail User Grouop Map data object.
		/// </summary>
		public ReportDetailUserGroupMapDO()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the group index attribute.
		/// </summary>
		public int GroupIndex
		{
			get { return this.groupIndex; }
			set { this.groupIndex = value; }
		}

		/// <summary>
		/// This property sets and gets the group ID attribute.
		/// </summary>
		public string GroupID
		{
			get { return this.groupID; }
			set { this.groupID = value; }
		}

		/// <summary>
		/// This property sets and gets the created by attribute.
		/// </summary>
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		/// <summary>
		/// This property sets and gets the updated date attribute.
		/// </summary>
		public System.DateTime CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		#endregion

		#region Public SQL Methods
		/// <summary>
		/// This method returns a SQL string to insert one report detail user group map record
		/// </summary>
		/// <returns></returns>
		public string SQLInsert(long reportIndex,string createdBy)
		{
			string insert =	"INSERT INTO tblGroupReportMap ("+
									"GroupIndex,"+
									"ReportIndex,"+
									"CreatedDate,"+
									"CreatedBy"+
									")"+
									" VALUES ("+
									this.groupIndex+","+
									reportIndex + "," +
									DateTime.UtcNow.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}")+","+
									"N'"+createdBy + "'"+
									")";

			return insert;
		}

		/// <summary>
		/// This method returns a SQL string to enumerate report detail user group records
		/// </summary>
		/// <returns></returns>
		public static string SQLEnumerate(long reportIndex)
		{
			string select =	"SELECT GroupIndex,CreatedDate,CreatedBy,"+
									"(SELECT GroupID FROM tblGroups WHERE tblGroups.GroupIndex = tblGroupReportMap.GroupIndex) GroupID"+
									" FROM tblGroupReportMap WHERE ReportIndex="+reportIndex.ToString();

			return select;
		}



		/// <summary>
		/// This method returns a SQL string to delete one report detail user group map record
		/// </summary>
		/// <returns></returns>
		public string SQLDelete(long reportIndex)
		{
			string delete ="DELETE FROM tblGroupReportMap "+
								" WHERE GroupIndex = "+groupIndex+" AND ReportIndex = " + reportIndex;

			return delete;
		}

		#endregion


		#region Load Methods
		/// <summary>
		/// This method will load an array list with the results from the database query.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		public static ArrayList SQLLoadReportDetailUserGroupMap(DataSet dataSet)
		{
			ArrayList reportDetailUserGroupArrayList=new ArrayList();

			if (dataSet != null
			&& dataSet.Tables.Count == 1)
			{
				foreach(DataRow row in dataSet.Tables[0].Rows)
				{
					ReportDetailUserGroupMapDO reportDetailUserGroupMapDO=new ReportDetailUserGroupMapDO();

					reportDetailUserGroupMapDO.GroupIndex=(int) row["GroupIndex"];
					reportDetailUserGroupMapDO.CreatedDate=(DateTime) row["CreatedDate"];
					reportDetailUserGroupMapDO.CreatedBy=row["CreatedBy"] as string;
					reportDetailUserGroupMapDO.GroupID=row["GroupID"] as string;

					reportDetailUserGroupArrayList.Add(reportDetailUserGroupMapDO);
				}				
			}

			return reportDetailUserGroupArrayList;
		}

		#endregion

	}
}

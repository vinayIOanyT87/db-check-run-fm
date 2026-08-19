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
///		2012-02-07	Brian Main				Converted SQL statements to SqlCommand objects and parameters
/// 
/// </summary>
/// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class ReportDetailUserGroupMapDO
	{
		#region Private Attributes
		private Guid groupGuid;
		private string groupID;
		private string createdBy;
		private DateTimeOffset createdDate;
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
		/// This property sets and gets the group Guid attribute.
		/// </summary>
		[DataMember]
		public Guid GroupGuid
		{
			get { return this.groupGuid; }
			set { this.groupGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the group ID attribute.
		/// </summary>
		[DataMember]
		public string GroupID
		{
			get { return this.groupID; }
			set { this.groupID = value; }
		}

		/// <summary>
		/// This property sets and gets the created by attribute.
		/// </summary>
		[DataMember]
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		/// <summary>
		/// This property sets and gets the updated date attribute.
		/// </summary>
		[DataMember]
		public System.DateTimeOffset CreatedDate
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
		//public string SQLInsert(Guid reportGuid, string createdBy)
		//{
		//   string insert = "INSERT INTO map.tblGroupToReportDetail (" +
		//                     "GroupGuid," +
		//                     "ReportDetailGuid," +
		//                     "CreatedDate," +
		//                     "CreatedBy" +
		//                     ")" +
		//                     " VALUES (" +
		//                     "'" + this.groupGuid + "'," +
		//                     "'" + reportGuid + "'," +
		//                     DateTimeOffset.Now.ToString ( "\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}" ) + "," +
		//                     "N'" + createdBy + "'" +
		//                     ")";

		//   return insert;
		//}

		/// <summary>
		/// This method returns a SqlCommand to enumerate report detail user group records
		/// </summary>
		/// <returns></returns>
		public static SqlCommand SQLEnumerate(Guid reportGuid)
		{
			const string PARAM_NAME_REPORTDETAILGUID = "@ReportDetailGuid";
			const SqlDbType PARAM_TYPE_REPORTDETAILGUID = SqlDbType.UniqueIdentifier;

			SqlCommand cmd = new SqlCommand();

			string select = "SELECT rpt.GroupGuid, rpt.CreatedDate, rpt.CreatedBy, grp.GroupID " +
							 "FROM  map.tblGroupToReportDetail rpt WITH(NOLOCK) " +
							 "INNER JOIN dbo.tblGroups grp  WITH(NOLOCK) ON grp.GroupGuid = rpt.GroupGuid " +
							 DataObject.AddParameter(cmd, "WHERE", "ReportDetailGuid", "=", PARAM_NAME_REPORTDETAILGUID, PARAM_TYPE_REPORTDETAILGUID, reportGuid);

			cmd.CommandText = select;

			return cmd;
		}


		/// <summary>
		/// This method returns a SQL string to delete one report detail user group map record
		/// </summary>
		/// <returns></returns>
		//public string SQLDelete(Guid reportGuid)
		//{
		//   string delete = "DELETE FROM map.tblGroupToReportDetail " +
		//                  " WHERE GroupGuid = '" + groupGuid + "' AND ReportDetailGuid = '" + reportGuid + "'";

		//   return delete;
		//}

		#endregion


		#region Public SQL Methods with SqlCommand / Parameters
		/// <summary>
		/// This method returns a SqlCommand to insert one report detail user group map record
		/// </summary>
		/// <returns></returns>
		public void SQLInsert(SqlCommand cmd, Guid reportGuid, string createdBy)
		{
			DateTimeOffset tempCreateDate;

			cmd.CommandText = "INSERT INTO map.tblGroupToReportDetail ( " +
				" GroupGuid, " +
				" ReportDetailGuid, " +
				" CreatedDate, " +
				" CreatedBy, " +
				" UpdatedDate, " +
				" UpdatedBy, " +
				" GroupToReportDetailGuid" +
				" ) VALUES ( " +
				" @GroupGuid, " +
				" @ReportDetailGuid, " +
				" @CreatedDate, " +
				" @CreatedBy, " +
				" @UpdatedDate, " +
				" @UpdatedBy, " +
				" @GroupToReportDetailGuid)";

			cmd.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ReportDetailGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@GroupToReportDetailGuid", SqlDbType.UniqueIdentifier);

			//on insert updateddate and createddate should have exact same value
			tempCreateDate = DateTimeOffset.Now;

			cmd.Parameters["@GroupGuid"].Value = groupGuid;
			cmd.Parameters["@ReportDetailGuid"].Value = reportGuid;
			cmd.Parameters["@CreatedDate"].Value = tempCreateDate;
			cmd.Parameters["@CreatedBy"].Value = createdBy;
			cmd.Parameters["@UpdatedDate"].Value = tempCreateDate;
			cmd.Parameters["@UpdatedBy"].Value = createdBy;
			cmd.Parameters["@GroupToReportDetailGuid"].Value = Guid.NewGuid();
		}


		/// <summary>
		/// This method returns a SqlCommand to delete one report detail user group map record
		/// </summary>
		/// <returns></returns>
		public void SQLDelete(SqlCommand cmd, Guid reportGuid)
		{
			cmd.CommandText = "DELETE FROM map.tblGroupToReportDetail WHERE GroupGuid = @GroupGuid AND ReportDetailGuid = @ReportDetailGuid";
			cmd.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ReportDetailGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@GroupGuid"].Value = groupGuid;
			cmd.Parameters["@ReportDetailGuid"].Value = reportGuid;
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
			ArrayList reportDetailUserGroupArrayList = new ArrayList();

			if (dataSet != null
			&& dataSet.Tables.Count == 1)
			{
				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					ReportDetailUserGroupMapDO reportDetailUserGroupMapDO = new ReportDetailUserGroupMapDO();

					reportDetailUserGroupMapDO.GroupGuid = DataObject.getValue<Guid>(row["GroupGuid"], Guid.Empty);
					reportDetailUserGroupMapDO.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
					reportDetailUserGroupMapDO.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
					reportDetailUserGroupMapDO.GroupID = DataObject.getValue<string>(row["GroupID"], "");

					reportDetailUserGroupArrayList.Add(reportDetailUserGroupMapDO);
				}
			}

			return reportDetailUserGroupArrayList;
		}

		#endregion
	}
}

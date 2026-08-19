/// <summary>
/// File name:	ReportConfigurationGroupDO.cs
/// Purpose:	The purpose is to contain the report configuration group information and
///				to house the SQL statement to get, update, insert, or delete a row in the 
///				database.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			   By:						Reason:
///		----------		--------------------	----------------------------------
///		2006-10-31		Richard Panachida		Fixed the problem with dates not working outside of
///												      US standard.
///		2009-03-05     Richard Panachida    Defect 877: Added code to handle if a user does not have finance rights.
///		
///		2012-02-07		Brian Main				Converted SQL statements to SqlCommand objects and parameters.
/// </summary>
/// 
using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class ReportConfigurationGroupDO : DataObject
	{
		#region Public data members
		public const string ALL_VIEW_RIGHT = "All View Rights";

		#endregion

		#region Private Attributes
		[DataMember]
		private string groupName;
		[DataMember]
		private Guid reportGroupGuid;
		[DataMember]
		private Guid siteGuid;
		[DataMember]
		private int orderNumber;
		[DataMember]
		private string createdBy;
		[DataMember]
		private string updatedBy;
		[DataMember]
		private DateTimeOffset createdDate;
		[DataMember]
		private DateTimeOffset updatedDate;

		private const int EMPTY_STRING = 0;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report configuration group data object.
		/// </summary>
		public ReportConfigurationGroupDO()
		{
			this.init();
		}
		#endregion

		#region Properties

		/// <summary>
		/// This property gets and sets the site Guid attribute.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the group groupGuid attribute.
		/// </summary>
		public Guid ReportGroupGuid
		{
			get { return this.reportGroupGuid; }
			set { this.reportGroupGuid = value; }
		}

		/// <summary>
		/// This property gets and sets the group name attribute.
		/// </summary>
		public string GroupName
		{
			get { return this.groupName; }
			set { this.groupName = value; }
		}

		/// <summary>
		/// This property gets and sets the created by attribute.
		/// </summary>
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		/// <summary>
		/// This property gets and sets the udpated by attribute.
		/// </summary>
		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		/// <summary>
		/// This property gets and sets the created date attribute.
		/// </summary>
		public System.DateTimeOffset CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		/// <summary>
		/// This property gets and sets the udpated date attribute.
		/// </summary>
		public System.DateTimeOffset UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}

		/// <summary>
		/// This property gets and sets the order number attribute.
		/// </summary>
		public int OrderNumber
		{
			get { return this.orderNumber; }
			set { this.orderNumber = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method returns a SqlCommand report group select statement using the group name as the key.
		/// </summary>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public SqlCommand SQLGetReportGroup(string groupName)
		{
			const string PARAM_NAME_GROUPNAME = "@GroupName";
			const int PARAM_SIZE_GROUPNAME = 30;
			const string PARAM_NAME_SITEGUID = "@SiteGuid";

			string select = "SELECT ReportGroupGuid, GroupName, SiteGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, " +
							  "OrderNumber ";
			string from = "FROM tblReportGroups ";
			string where = string.Empty;
			string orderBy = "ORDER BY OrderNumber";

			SqlCommand cmd = new SqlCommand();
			if (!string.IsNullOrEmpty(groupName))
			{
				where = AddParameter(cmd, "WHERE", "GroupName", "=", PARAM_NAME_GROUPNAME, SqlDbType.NVarChar, PARAM_SIZE_GROUPNAME, groupName) +
							AddParameter(cmd, true, "SiteGuid", PARAM_NAME_SITEGUID, SqlDbType.UniqueIdentifier, this.siteGuid);
			}
			cmd.CommandText = select + from + where + orderBy;
			return cmd;
		}

		/// <summary>
		/// This method returns a SqlCommand report group select statement using the group guid as the key.
		/// </summary>
		/// <param name="groupGuid"></param>
		/// <returns></returns>
		public SqlCommand SQLGetReportGroup(Guid groupGuid)
		{
			const string PARAM_NAME_GROUPGUID = "@GroupGuid";

			SqlCommand cmd = new SqlCommand();
			string select = "SELECT ReportGroupGuid, GroupName, SiteGuid, CreatedBy, CreatedDate, UpdatedBy, " +
							 "UpdatedDate, OrderNumber ";
			string from = "FROM tblReportGroups ";
			string where = AddParameter(cmd, "WHERE", "ReportGroupGuid", "=", PARAM_NAME_GROUPGUID, SqlDbType.UniqueIdentifier, groupGuid);
			string orderBy = "ORDER BY OrderNumber";

			cmd.CommandText = select + from + where + orderBy;
			return cmd;
		}

		/// <summary>
		/// This method returns a SQL insert statement.
		/// </summary>
		/// <returns></returns>

		//public string SQLInsertReportGroup()
		//{
		//   string insert = "INSERT INTO tblReportGroups (GroupName, SecurityRight, CreatedBy, CreatedDate, SiteGuid, OrderNumber) ";
		//   string insertValue = "VALUES ('" + this.groupName + "', " + this.securityRightEnum.ToString() + ", '" + this.createdBy + "', '" +
		//                   this.ConvertDateFormat(DateTimeOffset.Now) + "', '" + this.SiteGuid + "', " + this.orderNumber + ")";

		//   return (insert + insertValue);
		//}

		///// <summary>
		///// This method returns a SQL update statement using the group guid as the key.
		///// </summary>
		///// <returns></returns>
		//public string SQLUpdateReportGroup()
		//{
		//   string update = "UPDATE tblReportGroups SET GroupName = '" + this.groupName + "', " +
		//                "UpdatedBy = '" + this.updatedBy + "', UpdatedDate = '" + this.ConvertDateFormat(DateTimeOffset.Now) + "', " +
		//                  "SiteGuid = '" + this.siteGuid + "', SecurityRight = " + this.securityRightEnum.ToString() + " ";
		//   string where = "WHERE ReportGroupGuid = '" + this.reportGroupGuid + "'";

		//   return (update + where);
		//}

		///// <summary>
		///// This method returns a SQL to update the order number for a selected group guid.
		///// </summary>
		///// <param name="groupGuid"></param>
		///// <param name="orderNumber"></param>
		///// <returns></returns>
		//public string SQLUpdateReportGroupOrder(Guid groupGuid, int orderNumber)
		//{
		//   string update = "UPDATE tblReportGroups SET OrderNumber = " + orderNumber + ", " +
		//                  "UpdatedBy = '" + this.updatedBy + "', UpdatedDate = '" + this.updatedDate.ToString("s") + "' ";
		//   string where = "WHERE ReportGroupGuid = '" + groupGuid + "'";

		//   return (update + where);
		//}

		/// <summary>
		/// This method returns a SQL delete statement using the group guid as the key.
		/// </summary>
		/// <returns></returns>
		//public string SQLDeleteReportGroup()
		//{
		//   string delete = "DELETE FROM tblReportGroups ";
		//   string where = "WHERE ReportGroupGuid = '" + this.reportGroupGuid + "'";

		//   return (delete + where);
		//}

		public override string getSelectCommand() { return null; }

		public override string getInsertCommand() { return null; }

		public override string getDeleteCommand() { return null; }

		public override string getUpdateCommand() { return null; }

		#endregion

		#region Public Load Methods
		/// <summary>
		/// This method will load this object with the results from the database query.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <param name="security"></param>
		/// <returns></returns>
		public bool SQLLoadReportGroup(System.Data.DataSet dataSet, SecurityClass security)
		{
			bool successful = false;

			if (dataSet != null
			&& dataSet.Tables.Count == 1
			&& dataSet.Tables[0].Rows.Count == 1)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				System.Data.DataRow row = table.Rows[0];

				this.reportGroupGuid = DataObject.getValue<Guid>(row["ReportGroupGuid"], Guid.Empty);
				this.groupName = DataObject.getValue<string>(row["GroupName"], "");
				this.siteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guids.SiteAdminGuid);
				this.createdBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
				this.createdDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				this.updatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
				this.updatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], createdDate);
				this.orderNumber = DataObject.getValue<int>(row["OrderNumber"], 99);

				successful = true;
			}

			return successful;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize this object to its initial state.
		/// </summary>
		private void init()
		{
			this.reportGroupGuid = Guid.Empty;
			this.groupName = "";
			this.siteGuid = Guids.SiteAdminGuid;
			this.orderNumber = 99;
			this.createdBy = "";
			this.updatedBy = "";
			this.createdDate = DateTimeOffset.Now;
			this.updatedDate = DateTimeOffset.Now;
		}

		/// <summary>
		/// This method will convert the date/time object to a string that SQL server will accept
		/// yyyy-mm-dd hh:mm:ss.
		/// </summary>
		/// <param name="inDate"></param>
		/// <returns></returns>
		private string ConvertDateFormat(DateTimeOffset inDate)
		{
			string newDate = "";
			int year = inDate.Year;
			int month = inDate.Month;
			int day = inDate.Day;
			int hour = inDate.Hour;
			int min = inDate.Minute;
			int sec = inDate.Second;

			newDate = newDate + year + "-" + this.MakeTwoDigits(month) + "-" + this.MakeTwoDigits(day) + " " +
				this.MakeTwoDigits(hour) + ":" + this.MakeTwoDigits(min) + ":" + this.MakeTwoDigits(sec);

			return newDate;
		}

		/// <summary>
		/// This method will ensure that a month, hour, minute, or second will always contain two digits.
		/// It will return a 2 digit value in a string format.
		/// </summary>
		/// <param name="inValue"></param>
		/// <returns></returns>
		private string MakeTwoDigits(int inValue)
		{
			string newValue = "";

			if (inValue < 10)
				newValue = "0" + inValue;
			else
				newValue = inValue.ToString();

			return newValue;
		}
		#endregion

		#region SqlCommand with Parameters

		//This method returns a SqlCommand object with parameters wthat will insert a report group
		public void SQLInsertReportGroup(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblReportGroups ( " +
				" GroupName, " +
				" CreatedBy, " +
				" CreatedDate, " +
				" UpdatedBy, " +
				" UpdatedDate, " +
				" SiteGuid, " +
				" OrderNumber," +
				" ReportGroupGuid" +
				" ) VALUES ( " +
				" @GroupName, " +
				" @CreatedBy, " +
				" @CreatedDate, " +
				" @UpdatedBy, " +
				" @UpdatedDate, " +
				" @SiteGuid, " +
				" @OrderNumber, " +
				" @ReportGroupGuid)";

			cmd.Parameters.Add("@GroupName", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@OrderNumber", SqlDbType.Int);
			cmd.Parameters.Add("@ReportGroupGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@GroupName"].Value = this.groupName;
			cmd.Parameters["@CreatedBy"].Value = this.createdBy;
			cmd.Parameters["@CreatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@UpdatedBy"].Value = this.createdBy;
			cmd.Parameters["@UpdatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@OrderNumber"].Value = this.orderNumber;
			cmd.Parameters["@ReportGroupGuid"].Value = Guid.NewGuid();
		}

		/// <summary>
		/// This method returns a SQL update statement using the group guid as the key.
		/// </summary>
		/// <returns></returns>
		public void SQLUpdateReportGroup(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblReportGroups SET " +
				" GroupName = @GroupName, " +
				" UpdatedBy = @UpdatedBy, " +
				" UpdatedDate = @UpdatedDate, " +
				" SiteGuid = @SiteGuid " +
				" WHERE ReportGroupGuid = @ReportGroupGuid";

			cmd.Parameters.Add("@GroupName", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ReportGroupGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@GroupName"].Value = this.groupName;
			cmd.Parameters["@UpdatedBy"].Value = this.updatedBy;
			cmd.Parameters["@UpdatedDate"].Value = DateTimeOffset.Now;
			cmd.Parameters["@SiteGuid"].Value = this.siteGuid;
			cmd.Parameters["@ReportGroupGuid"].Value = this.reportGroupGuid;
		}

		/// <summary>
		/// This method returns a SqlCommand object to update the order number for a selected group guid.
		/// </summary>
		/// <param name="groupGuid"></param>
		/// <param name="orderNumber"></param>
		/// <returns></returns>
		public void SQLUpdateReportGroupOrder(SqlCommand cmd, Guid groupGuid, int orderNumber)
		{
			cmd.CommandText = "UPDATE tblReportGroups SET " +
				" OrderNumber = @OrderNumber, " +
				" UpdatedBy = @UpdatedBy, " +
				" UpdatedDate = @UpdatedDate " +
				" WHERE ReportGroupGuid = @ReportGroupGuid";

			cmd.Parameters.Add("@OrderNumber", SqlDbType.Int);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@ReportGroupGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@OrderNumber"].Value = orderNumber;
			cmd.Parameters["@UpdatedBy"].Value = this.updatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this.updatedDate;
			cmd.Parameters["@ReportGroupGuid"].Value = groupGuid;
		}

		/// <summary>
		/// This method returns a SQL delete statement using the group guid as the key.
		/// </summary>
		/// <returns></returns>
		public void SQLDeleteReportGroup(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblReportGroups WHERE ReportGroupGuid = @ReportGroupGuid";
			cmd.Parameters.Add("@ReportGroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ReportGroupGuid"].Value = this.reportGroupGuid;
		}

		#endregion
	}
}

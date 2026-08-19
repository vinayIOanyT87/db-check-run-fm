/// <summary>
/// File name:	ReportConfigurationDetailDO.cs
/// Purpose:	The purpose is to contain the report configuration detail information and
///				to house the SQL statement to get, update, insert, or delete a row in the 
///				database.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------	--------------------	----------------------------------
///		2005-7-15	Richard Panachida		Added feature to auto print reports
///		
///		2005-11-09	W.Gray					Changed reportUrl to reportPath
///			
///		2006-10-31	Richard Panachida		Fixed the problem with dates not working outside of
///													US standard.
///													
///		2009-03-30	W.Gray					Added User Group Map array list
///	
///		2012-02-07	Brian Main				Converted SQL statements to use SqlCommand objects
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

using FMBusinessObjects.Constants;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	[DataContract]
	[KnownType(typeof(ReportDetailUserGroupMapDO))]
	public class ReportConfigurationDetailDO : DataObject
	{
		#region Private Attributes
		[DataMember]
		private Guid reportGuid;
		[DataMember]
		private string reportName;
		[DataMember]
		private string reportPath;
		[DataMember]
		private string reportDirectory;
		[DataMember]
		private string reportDescription;
		[DataMember]
		private Guid siteGuid;
		[DataMember]
		private Guid reportGroupGuid;
		[DataMember]
		private string createdBy;
		[DataMember]
		private DateTimeOffset createdDate;
		[DataMember]
		private string updatedBy;
		[DataMember]
		private DateTimeOffset updatedDate;
		[DataMember]
		private int orderNumber;
		[DataMember]
		private bool printOnlyFlag;
		[DataMember]
		private string primaryPrinterName;
		[DataMember]
		private string secondaryPrinterName;
		[DataMember]
		private bool printAtEndOfDay;
		[DataMember]
		private bool printAtEndOfMonth;
		[DataMember]
		private bool dwReportFlag;
		[DataMember]
		private ArrayList userGroupMap = new ArrayList();

		private const int EMPTY_STRING = 0;
		private const string SQL_SELECT_STD =
				"SELECT ReportDetailGuid, ReportName, ReportPath, ReportDescription, ReportGroupGuid, " +
				"SiteGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, OrderNumber, " +
				"PrintOnlyFlag, PrimaryPrinterName, SecondaryPrinterName, PrintAtEndOfDay, " +
				"PrintAtEndOfMonth, DWReportFlag ";
		private const string SQL_FROM_STD = "FROM tblReportDetails ";
		private const string SQL_ORDERBY_STD = "ORDER BY OrderNumber";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report configuration detail data object.
		/// </summary>
		public ReportConfigurationDetailDO()
		{
			this.init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns true if the item already exist. Otherwise, it
		/// will return false.
		/// </summary>
		public bool Exists
		{
			get
			{
				if (this.reportGuid == Guid.Empty)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// This property returns true if the item is assigned to a group. Otherwise,
		/// it returns false.
		/// </summary>
		public bool AssignedToGroup
		{
			get
			{
				if (this.reportGroupGuid == Guid.Empty)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// This property sets and gets the report name attribute.
		/// </summary>
		public string ReportName
		{
			get { return this.reportName; }
			set { this.reportName = value; }
		}

		/// <summary>
		/// This property sets and gets the report Path attribute.
		/// </summary>
		public string ReportPath
		{
			get { return this.reportPath; }
			set { this.reportPath = value; }
		}

		/// <summary>
		/// This property sets and gets the report Directory attribute.
		/// </summary>
		public string ReportDirectory
		{
			get { return reportDirectory; }
			set { reportDirectory = value; }
		}

		/// <summary>
		/// This property sets and gets the report description attribute.
		/// </summary>
		public string ReportDescription
		{
			get { return this.reportDescription; }
			set { this.reportDescription = value; }
		}

		/// <summary>
		/// This property sets and gets the report Guid attribute. Guid.empty
		/// indicates this is a new item.
		/// </summary>
		public Guid ReportGuid
		{
			get { return this.reportGuid; }
			set { this.reportGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the report group guid attribute. Guid.empty
		/// indicates no assignment.
		/// </summary>
		public Guid ReportGroupGuid
		{
			get { return this.reportGroupGuid; }
			set { this.reportGroupGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the site Guid attribute.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
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
		/// This property sets and gets the updated by attribute.
		/// </summary>
		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		/// <summary>
		/// This property sets and gets the updated date attribute.
		/// </summary>
		public DateTimeOffset UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}

		/// <summary>
		/// This property sets and gets the updated date attribute.
		/// </summary>
		public DateTimeOffset CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		/// <summary>
		/// This property sets and gets the order number attribute.
		/// </summary>
		public int OrderNumber
		{
			get { return this.orderNumber; }
			set { this.orderNumber = value; }
		}

		/// <summary>
		/// This property sets and gets the print only flag. True indicates
		/// that the print is for printing only.
		/// </summary>
		public bool ForPrintingOnly
		{
			get { return this.printOnlyFlag; }
			set { this.printOnlyFlag = value; }
		}

		/// <summary>
		/// This property gets and sets the primary printer name attribute.
		/// </summary>
		public string PrimaryPrinterName
		{
			get { return this.primaryPrinterName; }
			set { this.primaryPrinterName = value; }
		}

		/// <summary>
		/// This property gets and sets the secondary printer name attribute.
		/// </summary>
		public string SecondaryPrinterName
		{
			get { return this.secondaryPrinterName; }
			set { this.secondaryPrinterName = value; }
		}

		/// <summary>
		/// This property gets and sets the printAtEndOfDay attribute.
		/// </summary>
		public bool PrintAtEndOfDay
		{
			get { return this.printAtEndOfDay; }
			set { this.printAtEndOfDay = value; }
		}

		/// <summary>
		/// This property gets and sets the printAtEndOfMonth attribute.
		/// </summary>
		public bool PrintAtEndOfMonth
		{
			get { return this.printAtEndOfMonth; }
			set { this.printAtEndOfMonth = value; }
		}


		/// <summary>
		/// This property gets and sets the DWReportFlag attribute.
		/// </summary>
		public bool DWReportFlag
		{
			get { return this.dwReportFlag; }
			set { this.dwReportFlag = value; }
		}


		/// <summary>
		/// This property gets and sets the userGroupMap attribute.
		/// </summary>
		public ArrayList UserGroupMap
		{
			get { return this.userGroupMap; }
			set { this.userGroupMap = value; }
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// This method returns a SqlCommand to retrieve one report detail record for a given guid.
		/// </summary>
		/// <param name="reportGuid"></param>
		/// <returns></returns>
		public SqlCommand SQLGetReportDetail(Guid reportGuid)
		{
			const string PARAM_NAME_REPORTGUID = "@ReportGuid";
			const SqlDbType PARAM_TYPE_REPORTGUID = SqlDbType.UniqueIdentifier;

			SqlCommand cmd = new SqlCommand();

			string where = AddParameter(cmd, "WHERE", "ReportDetailGuid", "=", PARAM_NAME_REPORTGUID, PARAM_TYPE_REPORTGUID, reportGuid);

			cmd.CommandText = SQL_SELECT_STD + SQL_FROM_STD + where + SQL_ORDERBY_STD;
			return cmd;
		}

		/// <summary>
		/// This method returns a SqlCommand to retrieve one report detail record for a given report name.
		/// </summary>
		/// <param name="reportName"></param>
		/// <returns></returns>
		public SqlCommand SQLGetReportDetail(string reportName)
		{
			const string PARAM_NAME_REPORTNAME = "@ReportName";
			const SqlDbType PARAM_TYPE_REPORTNAME = SqlDbType.NVarChar;
			const int PARAM_SIZE_REPORTNAME = 60;
			const string PARAM_NAME_SITEGUID = "@SiteGuid";
			const SqlDbType PARAM_TYPE_SITEGUID = SqlDbType.UniqueIdentifier;

			string where = string.Empty;

			SqlCommand cmd = new SqlCommand();
			if (!string.IsNullOrEmpty(reportName))
			{
				where = AddParameter(cmd, "WHERE", "ReportName", "=", PARAM_NAME_REPORTNAME, PARAM_TYPE_REPORTNAME, PARAM_SIZE_REPORTNAME, reportName) +
							AddParameter(cmd, true, "SiteGuid", PARAM_NAME_SITEGUID, PARAM_TYPE_SITEGUID, this.siteGuid);
			}
			cmd.CommandText = SQL_SELECT_STD + SQL_FROM_STD + where + SQL_ORDERBY_STD;
			return cmd;
		}

		/// <summary>
		/// This method returns a SqlCommand to retrieve one report detail record for a given report name
		/// that is for printing only.
		/// </summary>
		/// <param name="reportName"></param>
		/// <returns></returns>
		public SqlCommand SQLGetPrintReportDetail(string reportName)
		{
			const string PARAM_NAME_REPORTNAME = "@ReportName";
			const SqlDbType PARAM_TYPE_REPORTNAME = SqlDbType.NVarChar;
			const int PARAM_SIZE_REPORTNAME = 60;
			const string PARAM_NAME_SITEGUID = "@SiteGuid";
			const SqlDbType PARAM_TYPE_SITEGUID = SqlDbType.UniqueIdentifier;
			const string PARAM_NAME_PRINTONLYFLAG = "@PrintOnlyFlag";
			const SqlDbType PARAM_TYPE_PRINTONLYFLAG = SqlDbType.Bit;

			string where = "WHERE ";

			SqlCommand cmd = new SqlCommand();

			bool hasReportName = !string.IsNullOrEmpty(reportName);
			if (hasReportName)
			{
				where += AddParameter(cmd, false, "ReportName", PARAM_NAME_REPORTNAME, PARAM_TYPE_REPORTNAME, PARAM_SIZE_REPORTNAME, reportName);
			}

			where += AddParameter(cmd, hasReportName, "SiteGuid", PARAM_NAME_SITEGUID, PARAM_TYPE_SITEGUID, this.siteGuid) +
						AddParameter(cmd, true, "PrintOnlyFlag", PARAM_NAME_PRINTONLYFLAG, PARAM_TYPE_PRINTONLYFLAG, hasReportName);

			cmd.CommandText = SQL_SELECT_STD + SQL_FROM_STD + where + SQL_ORDERBY_STD;
			return cmd;
		}

		/// <summary>
		/// This method returns a SQL string to insert one report detail record into the database.
		/// </summary>
		/// <returns></returns>
		public SqlCommand SQLCmdInsertReportDetail()
		{
			SqlCommand cmd = new SqlCommand();
			cmd.CommandText = "INSERT INTO tblReportDetails (ReportName, ReportPath, ReportDescription, ReportGroupGuid, " +
				"CreatedBy, CreatedDate, SiteGuid, OrderNumber, PrintOnlyFlag, PrimaryPrinterName, " +
				"SecondaryPrinterName, PrintAtEndOfDay, PrintAtEndOfMonth, DWReportFlag ) VALUES " +
				"(@ReportName, @ReportPath, @ReportDescription, @ReportGroupGuid, " +
				"@CreatedBy, @CreatedDate, @SiteGuid, @OrderNumber, @PrintOnlyFlag, @PrimaryPrinterName, " +
				"@SecondaryPrinterName, @PrintAtEndOfDay, @PrintAtEndOfMonth, @DWReportFlag)";

			cmd.Parameters.AddWithValue("@ReportName", this.reportName);
			cmd.Parameters.AddWithValue("@ReportPath", this.reportPath);
			cmd.Parameters.AddWithValue("@ReportDescription", this.reportDescription);

			if (this.reportGroupGuid == Guid.Empty)
			{
				cmd.Parameters.AddWithValue("@ReportGroupGuid", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@ReportGroupGuid", this.reportGroupGuid);
			}

			cmd.Parameters.AddWithValue("@CreatedBy", this.createdBy);
			cmd.Parameters.AddWithValue("@CreatedDate", DateTimeOffset.Now);
			cmd.Parameters.AddWithValue("@SiteGuid", this.siteGuid);
			cmd.Parameters.AddWithValue("@OrderNumber", this.orderNumber);
			cmd.Parameters.AddWithValue("@PrintOnlyFlag", this.printOnlyFlag);
			cmd.Parameters.AddWithValue("@PrimaryPrinterName", this.primaryPrinterName);
			cmd.Parameters.AddWithValue("@SecondaryPrinterName", this.secondaryPrinterName);
			cmd.Parameters.AddWithValue("@PrintAtEndOfDay", this.printAtEndOfDay);
			cmd.Parameters.AddWithValue("@PrintAtEndOfMonth", this.printAtEndOfMonth);
			cmd.Parameters.AddWithValue("@DWReportFlag", this.dwReportFlag);

			return cmd;
		}

		/// <summary>
		/// This method returns a SQL string to update one report detail record in the database
		/// using the report guid as the key.
		/// </summary>
		/// <returns></returns>
		//public string SQLUpdateReportDetail()
		//{
		//   bool needComma = false;
		//   string update = "UPDATE tblReportDetails SET ";
		//   string where = "WHERE ReportDetailGuid = '" + this.reportGuid + "' ";

		//   if ((this.reportName != null) && (this.reportName.Length > EMPTY_STRING))
		//   {
		//      update = update + "ReportName = '" + this.reportName + "' ";
		//      needComma = true;
		//   }

		//   if ((this.reportDescription != null) && (this.reportDescription.Length > EMPTY_STRING))
		//   {
		//      if (needComma == true)
		//         update = update + ", ";

		//      update = update + "ReportDescription = '" + this.reportDescription + "' ";
		//      needComma = true;
		//   }

		//   if ((this.reportPath != null) && (this.reportPath.Length > EMPTY_STRING))
		//   {
		//      if (needComma == true)
		//         update = update + ", ";

		//      update = update + "ReportPath = '" + this.reportPath + "' ";
		//      needComma = true;
		//   }

		//   if ((this.primaryPrinterName != null) && (this.primaryPrinterName.Length > EMPTY_STRING))
		//   {
		//      if (needComma == true)
		//         update = update + ", ";

		//      update = update + "PrimaryPrinterName = '" + this.primaryPrinterName + "' ";
		//      needComma = true;
		//   }

		//   if ((this.secondaryPrinterName != null) && (this.secondaryPrinterName.Length > EMPTY_STRING))
		//   {
		//      if (needComma == true)
		//         update = update + ", ";

		//      update = update + "SecondaryPrinterName = '" + this.secondaryPrinterName + "' ";
		//      needComma = true;
		//   }

		//   if ((this.updatedBy != null) && (this.updatedBy.Length > EMPTY_STRING))
		//   {
		//      if (needComma == true)
		//         update = update + ", ";

		//      update = update + "UpdatedBy = '" + this.updatedBy + "' ";
		//      needComma = true;
		//   }

		//   if (needComma == true)
		//      update = update + ", ";

		//   update = update + "UpdatedDate = '" + this.ConvertDateFormat(DateTimeOffset.Now) + "', ";

		//   if (this.reportGroupGuid == Guid.Empty)
		//   {
		//      update = update + "ReportGroupGuid = NULL, ";
		//   }
		//   else
		//   {
		//      update = update + "ReportGroupGuid = '" + this.reportGroupGuid + "', ";
		//   }

		//   update = update + "SiteGuid = '" + this.siteGuid + "', ";
		//   update = update + "PrintOnlyFlag = " + this.GetBitValue(this.printOnlyFlag) + ", ";
		//   update = update + "PrintAtEndOfDay = " + this.GetBitValue(this.printAtEndOfDay) + ", ";
		//   update = update + "PrintAtEndOfMonth = " + this.GetBitValue(this.printAtEndOfMonth) + " ";

		//   return (update + where);
		//}

		///// <summary>
		///// This methods returns a SQL that will update all the group guids in the report detail record
		///// using the old group guid as the key.
		///// </summary>
		///// <param name="oldGroupGuid"></param>
		///// <param name="newGroupGuid"></param>
		///// <returns></returns>
		//public string SQLUpdateGroupGuid(Guid oldGroupGuid, Guid newGroupGuid)
		//{
		//   string update = "UPDATE tblReportDetails SET ReportGroupGuid = " ;

		//   if(newGroupGuid == Guid.Empty)
		//   {
		//      update += "NULL ";
		//   }
		//   else
		//   {
		//      update += "'" + newGroupGuid + "' ";
		//   }

		//   string where = "WHERE ReportGroupGuid ";

		//   if (oldGroupGuid == Guid.Empty)
		//   {
		//      where += " IS NULL " + "' AND SiteGuid = '" + this.siteGuid + "'";
		//   }
		//   else
		//   {
		//      where += " = '" + oldGroupGuid + "' AND SiteGuid = '" + this.siteGuid + "'";
		//   }


		//   return (update + where);
		//}

		/// <summary>
		/// This method returns a SQL that will update the report detail order number only.
		/// </summary>
		/// <param name="reportDetailGuid"></param>
		/// <param name="orderNumber"></param>
		/// <returns></returns>
		public string SQLUpdateDetailOrder(Guid reportDetailGuid, int orderNumber)
		{
			string update = "UPDATE tblReportDetails SET OrderNumber = " + orderNumber + " ";
			string where = "WHERE ReportDetailGuid = '" + reportDetailGuid + "'";

			return (update + where);
		}

		///// <summary>
		///// This method returns a SQL string to delete one report detail record from the database
		///// using the report detail guid as the key.
		///// </summary>
		///// <returns></returns>
		//public string SQLDeleteReportDetail()
		//{
		//   string delete = "DELETE FROM tblReportDetails ";
		//   string where = "WHERE ReportDetailGuid = '" + this.reportGuid + "'";

		//   return (delete + where);
		//}

		public override string getSelectCommand() { return null; }

		public override string getInsertCommand() { return null; }

		public override string getDeleteCommand() { return null; }

		public override string getUpdateCommand() { return null; }

		#endregion

		#region Load Methods
		/// <summary>
		/// This method will load this object with the results from the database query.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		public bool SQLLoadReportDetail(System.Data.DataSet dataSet)
		{
			bool successful = false;

			if (dataSet != null
			&& dataSet.Tables.Count == 1
			&& dataSet.Tables[0].Rows.Count == 1)
			{
				System.Data.DataTable table = dataSet.Tables[0];
				System.Data.DataRow row = table.Rows[0];

				this.reportGuid = DataObject.getValue<Guid>(row["ReportDetailGuid"], Guid.Empty);
				this.reportName = DataObject.getValue<string>(row["ReportName"], "");
				this.reportPath = DataObject.getValue<string>(row["ReportPath"], "");
				this.reportDescription = DataObject.getValue<string>(row["ReportDescription"], "");
				this.reportGroupGuid = DataObject.getValue<Guid>(row["ReportGroupGuid"], Guid.Empty);
				this.siteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guids.SiteAdminGuid);
				this.createdBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
				this.createdDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				this.updatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
				this.updatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], createdDate);
				this.orderNumber = DataObject.getValue<int>(row["OrderNumber"], 99);
				this.printOnlyFlag = DataObject.getValue<bool>(row["PrintOnlyFlag"], false);
				this.primaryPrinterName = DataObject.getValue<string>(row["PrimaryPrinterName"], "");
				this.secondaryPrinterName = DataObject.getValue<string>(row["SecondaryPrinterName"], "");
				this.printAtEndOfDay = DataObject.getValue<bool>(row["PrintAtEndOfDay"], false);
				this.printAtEndOfMonth = DataObject.getValue<bool>(row["PrintAtEndOfMonth"], false);
				this.dwReportFlag = DataObject.getValue<bool>(row["DWReportFlag"], false);

				successful = true;
			}

			return successful;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void init()
		{
			this.reportName = "";
			this.reportDescription = "";
			this.reportPath = "";
			this.reportGuid = Guid.Empty;
			this.reportGroupGuid = Guid.Empty;
			this.siteGuid = Guid.Empty;
			this.orderNumber = 99;
			this.createdBy = "";
			this.updatedBy = "";
			this.createdDate = DateTimeOffset.Now;
			this.updatedDate = DateTimeOffset.Now;
			this.printOnlyFlag = false;
			this.primaryPrinterName = "";
			this.secondaryPrinterName = "";
			this.printAtEndOfDay = false;
			this.printAtEndOfMonth = false;
			this.dwReportFlag = false;
		}

		/// <summary>
		/// This method will convert the boolean value to an integer value of 0 = false and 
		/// 1 = true.
		/// </summary>
		/// <param name="boolValue"></param>
		/// <returns></returns>
		private int GetBitValue(bool boolValue)
		{
			if (boolValue == true)
				return 1;
			else
				return 0;
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

		/// <summary>
		/// This methods returns a SqlCommand object that will update all the group guids in the report detail record
		/// using the old group guid as the key.
		/// </summary>
		/// <param name="oldGroupGuid"></param>
		/// <param name="newGroupGuid"></param>
		/// <returns></returns>
		public void SQLUpdateGroupGuid(SqlCommand cmd, Guid oldGroupGuid, Guid newGroupGuid)
		{
			String update = "UPDATE tblReportDetails SET ReportGroupGuid = ";

			if (newGroupGuid == Guid.Empty)
			{
				update += " NULL ";
			}
			else
			{
				update += " @NewReportGroupGuid ";
				cmd.Parameters.Add("@NewReportGroupGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@NewReportGroupGuid"].Value = newGroupGuid;
			}

			string where = "WHERE ReportGroupGuid ";

			if (oldGroupGuid == Guid.Empty)
			{
				where += " IS NULL " + "' AND SiteGuid = @SiteGuid";
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = this.siteGuid;
			}
			else
			{
				where += " = @OldGroupGuid AND SiteGuid = @SiteGuid";
				cmd.Parameters.Add("@OldGroupGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@OldGroupGuid"].Value = oldGroupGuid;
				cmd.Parameters["@SiteGuid"].Value = this.siteGuid;
			}
			cmd.CommandText = update + where;
		}

		/// <summary>
		/// This method returns a SqlCommand object to delete one report detail record from the database
		/// using the report detail guid as the key.
		/// </summary>
		/// <returns></returns>
		public void SQLDeleteReportDetail(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblReportDetails WHERE ReportDetailGuid = @ReportDetailGuid";
			cmd.Parameters.Add("@ReportDetailGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ReportDetailGuid"].Value = reportGuid;
		}

		/// <summary>
		/// This method returns a SqlCommand object to update one report detail record in the database
		/// using the report guid as the key.
		/// </summary>
		/// <returns></returns>
		public void SQLUpdateReportDetail(SqlCommand cmd)
		{
			bool needComma = false;
			string update = "UPDATE tblReportDetails SET ";
			string where = "WHERE ReportDetailGuid = @ReportDetailGuid ";

			cmd.Parameters.Add("@ReportDetailGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ReportDetailGuid"].Value = this.reportGuid;

			if ((this.reportName != null) && (this.reportName.Length > EMPTY_STRING))
			{
				update = update + " ReportName = @ReportName ";
				needComma = true;
				cmd.Parameters.Add("@ReportName", SqlDbType.NVarChar, 60);
				cmd.Parameters["@ReportName"].Value = this.reportName;
			}

			if ((this.reportDescription != null) && (this.reportDescription.Length > EMPTY_STRING))
			{
				if (needComma == true)
					update = update + ", ";

				update = update + " ReportDescription = @ReportDescription ";
				needComma = true;
				cmd.Parameters.Add("@ReportDescription", SqlDbType.NVarChar, 255);
				cmd.Parameters["@ReportDescription"].Value = this.reportDescription;
			}

			if ((this.reportPath != null) && (this.reportPath.Length > EMPTY_STRING))
			{
				if (needComma == true)
					update = update + ", ";

				update = update + " ReportPath = @ReportPath ";
				needComma = true;
				cmd.Parameters.Add("@ReportPath", SqlDbType.NVarChar, 200);
				cmd.Parameters["@ReportPath"].Value = this.reportPath;
			}

			if ((this.primaryPrinterName != null) && (this.primaryPrinterName.Length > EMPTY_STRING))
			{
				if (needComma == true)
					update = update + ", ";

				update = update + "PrimaryPrinterName = @PrimaryPrinterName ";
				needComma = true;
				cmd.Parameters.Add("@PrimaryPrinterName", SqlDbType.NVarChar, 100);
				cmd.Parameters["@PrimaryPrinterName"].Value = this.primaryPrinterName;
			}

			if ((this.secondaryPrinterName != null) && (this.secondaryPrinterName.Length > EMPTY_STRING))
			{
				if (needComma == true)
					update = update + ", ";

				update = update + "SecondaryPrinterName = @SecondaryPrinterName ";
				needComma = true;
				cmd.Parameters.Add("@SecondaryPrinterName", SqlDbType.NVarChar, 100);
				cmd.Parameters["@SecondaryPrinterName"].Value = this.secondaryPrinterName;
			}

			if ((this.updatedBy != null) && (this.updatedBy.Length > EMPTY_STRING))
			{
				if (needComma == true)
					update = update + ", ";

				update = update + "UpdatedBy = @UpdatedBy ";
				needComma = true;
				cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
				cmd.Parameters["@UpdatedBy"].Value = this.updatedBy;
			}

			if (needComma == true)
				update = update + ", ";

			update = update + "UpdatedDate = @UpdatedDate, ";
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters["@UpdatedDate"].Value = DateTimeOffset.Now;

			if (this.reportGroupGuid == Guid.Empty)
			{
				update = update + "ReportGroupGuid = NULL, ";
			}
			else
			{
				update = update + "ReportGroupGuid = @ReportGroupGuid, ";
			}
			cmd.Parameters.Add("@ReportGroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ReportGroupGuid"].Value = this.reportGroupGuid;

			update = update + "SiteGuid = @SiteGuid, ";
			update = update + "PrintOnlyFlag = @PrintOnlyFlag, ";
			update = update + "PrintAtEndOfDay = @PrintAtEndOfDay, ";
			update = update + "PrintAtEndOfMonth = @PrintAtEndOfMonth, ";
			update = update + "DWReportFlag = @DWReportFlag ";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PrintOnlyFlag", SqlDbType.Bit);
			cmd.Parameters.Add("@PrintAtEndOfDay", SqlDbType.Bit);
			cmd.Parameters.Add("@PrintAtEndOfMonth", SqlDbType.Bit);
			cmd.Parameters.Add("@DWReportFlag", SqlDbType.Bit);
			cmd.Parameters["@SiteGuid"].Value = this.siteGuid;
			cmd.Parameters["@PrintOnlyFlag"].Value = this.GetBitValue(this.printOnlyFlag);
			cmd.Parameters["@PrintAtEndOfDay"].Value = this.GetBitValue(this.printAtEndOfDay);
			cmd.Parameters["@PrintAtEndOfMonth"].Value = this.GetBitValue(this.printAtEndOfMonth);
			cmd.Parameters["@DWReportFlag"].Value = this.GetBitValue(this.dwReportFlag);

			cmd.CommandText = (update + where);
		}

		/// <summary>
		/// This method returns a SqlCommand object that will update the report detail order number only.
		/// </summary>
		/// <param name="reportDetailGuid"></param>
		/// <param name="orderNumber"></param>
		/// <returns></returns>
		public void SQLUpdateDetailOrder(SqlCommand cmd, Guid reportDetailGuid, int orderNumber)
		{
			string update = "UPDATE tblReportDetails SET OrderNumber = @OrderNumber ";
			string where = " WHERE ReportDetailGuid = @ReportDetailGuid ";

			cmd.CommandText = (update + where);

			cmd.Parameters.Add("@OrderNumber", SqlDbType.Int);
			cmd.Parameters.Add("@ReportDetailGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@OrderNumber"].Value = orderNumber;
			cmd.Parameters["@ReportDetailGuid"].Value = reportDetailGuid;
		}

		#endregion
	}
}

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
/// </summary>
/// 
using System;
using System.Collections;

namespace ReportingServices
{
	public class ReportConfigurationDetailDO : DataObjectBase
	{
		#region Private Attributes
		private long		reportIndex;
		private string		reportName;
		private string		reportPath;
		private string		reportDescription;
		private int			siteIndex;
		private long		groupIndex;
		private string		createdBy;
		private DateTime	createdDate;
		private string		updatedBy;
		private DateTime	updatedDate; 
		private int			orderNumber;
		private bool		printOnlyFlag;
		private string		primaryPrinterName;
		private string		secondaryPrinterName;
		private bool		printAtEndOfDay;
		private bool		printAtEndOfMonth;
		private ArrayList	userGroupMap=new ArrayList();

		private const int EMPTY_STRING = 0;
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
				if (this.reportIndex == -1)
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
				if (this.groupIndex == -1)
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
		/// This property sets and gets the report description attribute.
		/// </summary>
		public string ReportDescription
		{
			get { return this.reportDescription; }
			set { this.reportDescription = value; }
		}

		/// <summary>
		/// This property sets and gets the report index attribute. A negative
		/// one indicates this is a new item.
		/// </summary>
		public long ReportIndex
		{
			get { return this.reportIndex; }
			set { this.reportIndex = value; }
		}

		/// <summary>
		/// This property sets and gets the group index attribute. A negative
		/// one indicates no assignment.
		/// </summary>
		public long GroupIndex
		{
			get { return this.groupIndex; }
			set { this.groupIndex = value; }
		}

		/// <summary>
		/// This property sets and gets the site index attribute.
		/// </summary>
		public int SiteIndex
		{
			get { return this.siteIndex; }
			set { this.siteIndex = value; }
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
		public System.DateTime UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}

		/// <summary>
		/// This property sets and gets the updated date attribute.
		/// </summary>
		public System.DateTime CreatedDate
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
		/// This property gets and sets the userGroupMap attribute.
		/// </summary>
		public ArrayList UserGroupMap
		{
			get { return this.userGroupMap; }
			set { this.userGroupMap = value; }
		}

		#endregion

		#region Public SQL Methods
		/// <summary>
		/// This method returns a SQL string to retrieve one report detail record for a given index.
		/// </summary>
		/// <param name="reportIndex"></param>
		/// <returns></returns>
		public string SQLGetReportDetail(long reportIndex)
		{
			string select  = "SELECT ReportIndex, ReportName, ReportPath, ReportDescription, GroupIndex, " +
				"SiteIndex, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, OrderNumber, " +
				"PrintOnlyFlag, PrimaryPrinterName, SecondaryPrinterName, PrintAtEndOfDay, " +
				"PrintAtEndOfMonth ";
			string from    = "FROM tblReportDetails ";
			string where   = "WHERE ReportIndex = " + reportIndex + " ";
			string orderBy = "ORDER BY OrderNumber";

			return (select + from + where + orderBy);
		}

		/// <summary>
		/// This method returns a SQL string to retrieve one report detail record for a given report name.
		/// </summary>
		/// <param name="reportName"></param>
		/// <returns></returns>
		public string SQLGetReportDetail(string reportName)
		{
			string select  = "SELECT ReportIndex, ReportName, ReportPath, ReportDescription, GroupIndex, " +
				"SiteIndex, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, OrderNumber, " +
				"PrintOnlyFlag, PrimaryPrinterName, SecondaryPrinterName, PrintAtEndOfDay, " +
				"PrintAtEndOfMonth ";
			string from    = "FROM tblReportDetails ";
			string where   = "WHERE ReportName = '";
			string orderBy = "ORDER BY OrderNumber";

			if ((reportName != null) && (reportName.Length > EMPTY_STRING))
				where = where + reportName + "' AND SiteIndex = " + this.siteIndex;
			else
				where = "";

			return (select + from + where + orderBy);
		}

		/// <summary>
		/// This method returns a SQL string to retrieve one report detail record for a given report name
		/// that is for printing only.
		/// </summary>
		/// <param name="reportName"></param>
		/// <returns></returns>
		public string SQLGetPrintReportDetail(string reportName)
		{
			string select  = "SELECT ReportIndex, ReportName, ReportPath, ReportDescription, GroupIndex, " +
				"SiteIndex, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, OrderNumber, " +
				"PrintOnlyFlag, PrimaryPrinterName, SecondaryPrinterName, PrintAtEndOfDay, " +
				"PrintAtEndOfMonth ";
			string from    = "FROM tblReportDetails ";
			string where   = "WHERE ReportName = '";
			string orderBy = "ORDER BY OrderNumber";

			if ((reportName != null) && (reportName.Length > EMPTY_STRING))
				where = where + reportName + "' AND SiteIndex = " + this.siteIndex + " AND PrintOnlyFlag = true";
			else
				where = "SiteIndex = " + this.siteIndex + " AND PrintOnlyFlag = 0";

			return (select + from + where + orderBy);
		}

		/// <summary>
		/// This method returns a SQL string to insert one report detail record into the database.
		/// </summary>
		/// <returns></returns>
		public string SQLInsertReportDetail()
		{
			string insert = "INSERT INTO tblReportDetails (ReportName, ReportPath, ReportDescription, GroupIndex, " +
				"CreatedBy, CreatedDate, SiteIndex, OrderNumber, PrintOnlyFlag, PrimaryPrinterName, " +
				"SecondaryPrinterName, PrintAtEndOfDay, PrintAtEndOfMonth ) ";

			string insertValues = "VALUES('" + this.reportName + "', '" + this.reportPath + "', '" +
				this.reportDescription + "', " + this.groupIndex + ", '" + this.createdBy + "', '" +
				this.ConvertDateFormat(System.DateTime.Now) + "', " + this.siteIndex + ", " + this.orderNumber + ", " +
				this.GetBitValue(this.printOnlyFlag) + ", '" + this.primaryPrinterName + "', '" +
				this.secondaryPrinterName + "', " + this.GetBitValue(this.printAtEndOfDay) + ", " +
				this.GetBitValue(this.printAtEndOfMonth)+")";

			return (insert + insertValues);
		}

		/// <summary>
		/// This method returns a SQL string to update one report detail record in the database
		/// using the report index as the key.
		/// </summary>
		/// <returns></returns>
		public string SQLUpdateReportDetail()
		{
			bool   needComma = false;
			string update = "UPDATE tblReportDetails SET ";
			string where  = "WHERE ReportIndex = " + this.reportIndex + " ";

			if ((this.reportName != null) && (this.reportName.Length > EMPTY_STRING))
			{
				update = update + "ReportName = '" + this.reportName + "' ";
				needComma = true;
			}

			if ((this.reportDescription != null) && (this.reportDescription.Length > EMPTY_STRING))
			{
				if (needComma == true)
					update = update + ", ";

				update = update + "ReportDescription = '" + this.reportDescription + "' ";
				needComma = true;
			}

			if ((this.reportPath != null) && (this.reportPath.Length > EMPTY_STRING))
			{
				if (needComma == true)
					update = update + ", ";

				update = update + "ReportPath = '" + this.reportPath + "' ";
				needComma = true;
			}

			if ((this.primaryPrinterName != null) && (this.primaryPrinterName.Length > EMPTY_STRING))
			{
				if (needComma == true)
					update = update + ", ";

				update = update + "PrimaryPrinterName = '" + this.primaryPrinterName + "' ";
				needComma = true;
			}

			if ((this.secondaryPrinterName != null) && (this.secondaryPrinterName.Length > EMPTY_STRING))
			{
				if (needComma == true)
					update = update + ", ";

				update = update + "SecondaryPrinterName = '" + this.secondaryPrinterName + "' ";
				needComma = true;
			}

			if ((this.updatedBy != null) && (this.updatedBy.Length > EMPTY_STRING))
			{
				if (needComma == true)
					update = update + ", ";

				update = update + "UpdatedBy = '" + this.updatedBy + "' ";
				needComma = true;
			}

			if (needComma == true)
				update = update + ", ";

			update = update + "UpdatedDate = '" + this.ConvertDateFormat(System.DateTime.UtcNow) + "', ";
			update = update + "GroupIndex = " + this.groupIndex + ", ";
			update = update + "SiteIndex = " + this.siteIndex + ", ";
			update = update + "PrintOnlyFlag = " + this.GetBitValue(this.printOnlyFlag) + ", ";
			update = update + "PrintAtEndOfDay = " + this.GetBitValue(this.printAtEndOfDay) + ", ";
			update = update + "PrintAtEndOfMonth = " + this.GetBitValue(this.printAtEndOfMonth) + " ";

			return (update + where);
		}

		/// <summary>
		/// This methods returns a SQL that will update all the group indexes in the report detail record
		/// using the old group index as the key.
		/// </summary>
		/// <param name="oldGroupIndex"></param>
		/// <param name="newGroupIndex"></param>
		/// <returns></returns>
		public string SQLUpdateGroupIndex(long oldGroupIndex, long newGroupIndex)
		{
			string update = "UPDATE tblReportDetails SET GroupIndex = " + newGroupIndex + " ";
			string where  = "WHERE GroupIndex = " + oldGroupIndex + " AND SiteIndex = " + this.siteIndex;

			return (update + where);
		}

		/// <summary>
		/// This method returns a SQL that will update the report detail order number only.
		/// </summary>
		/// <param name="reportDetailIndex"></param>
		/// <param name="orderNumber"></param>
		/// <returns></returns>
		public string SQLUpdateDetailOrder(long reportDetailIndex, int orderNumber)
		{
			string update = "UPDATE tblReportDetails SET OrderNumber = " + orderNumber + " ";
			string where  = "WHERE ReportIndex = " + reportDetailIndex;

			return (update + where);
		}

		/// <summary>
		/// This method returns a SQL string to delete one report detail record from the database
		/// using the report index as the key.
		/// </summary>
		/// <returns></returns>
		public string SQLDeleteReportDetail()
		{
			string delete = "DELETE FROM tblReportDetails ";
			string where  = "WHERE ReportIndex = " + this.reportIndex;

			return (delete + where);
		}
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

				this.reportIndex          = base.getLong(row[0]);
				this.reportName           = base.getString(row[1]);
				this.reportPath           = base.getString(row[2]);
				this.reportDescription    = base.getString(row[3]);
				this.groupIndex           = base.getLong(row[4]);
				this.siteIndex            = base.getInt(row[5]);
				this.createdBy            = base.getString(row[6]);
				this.updatedBy            = base.getString(row[8]);
				this.orderNumber          = base.getInt(row[10]);
				this.printOnlyFlag        = base.getBool(row[11]);
				this.primaryPrinterName   = base.getString(row[12]);
				this.secondaryPrinterName = base.getString(row[13]);
				this.printAtEndOfDay      = base.getBool(row[14]);
				this.printAtEndOfMonth    = base.getBool(row[15]);

				if (base.IsDateValid(row[7]) == true)
					this.createdDate = base.getDateTime(row[7]);

				if (base.IsDateValid(row[9]) == true)
					this.updatedDate = base.getDateTime(row[9]);

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
			this.reportName           = "";
			this.reportDescription    = "";
			this.reportPath           = "";
			this.reportIndex          = -1;
			this.groupIndex           = -1;
			this.siteIndex            = -1;
			this.orderNumber          = 99;
			this.createdBy            = "";
			this.updatedBy            = "";
			this.createdDate          = System.DateTime.UtcNow;
			this.updatedDate          = System.DateTime.UtcNow;
			this.printOnlyFlag        = false;
			this.primaryPrinterName   = "";
			this.secondaryPrinterName = "";
			this.printAtEndOfDay      = false;
			this.printAtEndOfMonth    = false;
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
		/// <param name="inDateTime"></param>
		/// <returns></returns>
		private string ConvertDateFormat(System.DateTime inDateTime)
		{
			string newDate = "";
			int year  = inDateTime.Year;
			int month = inDateTime.Month;
			int day   = inDateTime.Day;
			int hour  = inDateTime.Hour;
			int min   = inDateTime.Minute;
			int sec   = inDateTime.Second;

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
	}
}

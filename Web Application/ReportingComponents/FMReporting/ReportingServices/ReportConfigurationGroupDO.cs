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
/// </summary>
/// 
using System;
using System.Collections;
using FMCommon;

namespace ReportingServices
{
	public class ReportConfigurationGroupDO : DataObjectBase
   {
      #region Public data members
      public const string ALL_VIEW_RIGHT = "All View Rights";
      #endregion

      #region Private Attributes
      private string    groupName;
		private long      groupIndex;
		private int       siteIndex;
		private int       orderNumber;
		private string    createdBy;
		private string    updatedBy;
		private DateTime  createdDate;
		private DateTime  updatedDate;
      private string    securityRight;
      private int       securityRightEnum;

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
		/// This property gets and sets the site index attribute.
		/// </summary>
		public int SiteIndex
		{
			get { return this.siteIndex; }
			set { this.siteIndex = value; }
		}

		/// <summary>
		/// This property gets and sets the group index attribute.
		/// </summary>
		public long GroupIndex
		{
			get { return this.groupIndex; }
			set { this.groupIndex = value; }
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
      /// This property gets and sets the security right.
      /// </summary>
      public string SecurityRight
      {
         get { return this.securityRight; }
         set { this.securityRight = value; }
      }

      /// <summary>
      /// This property gets and sets the security right enum.
      /// </summary>
      public int SecurityRightEnum
      {
         get { return this.securityRightEnum; }
         set { this.securityRightEnum = value; }
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
		public System.DateTime CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		/// <summary>
		/// This property gets and sets the udpated date attribute.
		/// </summary>
		public System.DateTime UpdatedDate
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

		#region Public SQL Methods
		/// <summary>
		/// This method returns a SQL report group select statement using the group name as the key.
		/// </summary>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public string SQLGetReportGroup(string groupName)
		{
			string select  = "SELECT GroupIndex, GroupName, SiteIndex, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, " +
				              "OrderNumber, SecurityRight ";
			string from    = "FROM tblReportGroups ";
			string where   = "WHERE GroupName = '" + groupName + "' AND SiteIndex = " + this.siteIndex + " ";
			string orderBy = "ORDER BY OrderNumber";

         if ((groupName == null) || (groupName.Length == EMPTY_STRING))
         {
            where = "";
         }

			return (select + from + where + orderBy);
		}

		/// <summary>
		/// This method returns a SQL report group select statement using the group index as the key.
		/// </summary>
		/// <param name="groupIndex"></param>
		/// <returns></returns>
		public string SQLGetReportGroup(long groupIndex)
		{
			string select  = "SELECT GroupIndex, GroupName, SiteIndex, CreatedBy, CreatedDate, UpdatedBy, " +
				             "UpdatedDate, OrderNumber, SecurityRight ";
			string from    = "FROM tblReportGroups ";
			string where   = "WHERE GroupIndex = " + groupIndex;
         string orderBy = "ORDER BY OrderNumber";

			return (select + from + where + orderBy);
		}

		/// <summary>
		/// This method returns a SQL insert statement.
		/// </summary>
		/// <returns></returns>
		public string SQLInsertReportGroup()
		{
			string insert = "INSERT INTO tblReportGroups (GroupName, SecurityRight, CreatedBy, CreatedDate, SiteIndex, OrderNumber) ";
			string insertValue = "VALUES ('" + this.groupName + "', " + this.securityRightEnum.ToString() + ", '" + this.createdBy + "', '" +
				                 this.ConvertDateFormat(System.DateTime.UtcNow) + "', " + this.siteIndex + ", " + this.orderNumber + ")";

			return (insert + insertValue);
		}

		/// <summary>
		/// This method returns a SQL update statement using the group index as the key.
		/// </summary>
		/// <returns></returns>
		public string SQLUpdateReportGroup()
		{
			string update = "UPDATE tblReportGroups SET GroupName = '" + this.groupName + "', " +
				             "UpdatedBy = '" + this.updatedBy + "', UpdatedDate = '" + this.ConvertDateFormat(System.DateTime.UtcNow) + "', " +
							    "SiteIndex = " + this.siteIndex + ", SecurityRight = " + this.securityRightEnum.ToString() + " ";
			string where  = "WHERE GroupIndex = " + this.groupIndex;

			return (update + where);
		}

		/// <summary>
		/// This method returns a SQL to update the order number for a selected group index.
		/// </summary>
		/// <param name="groupIndex"></param>
		/// <param name="orderNumber"></param>
		/// <returns></returns>
		public string SQLUpdateReportGroupOrder(long groupIndex, int orderNumber)
		{
			string update = "UPDATE tblReportGroups SET OrderNumber = " + orderNumber + ", " +
							    "UpdatedBy = '" + this.updatedBy + "', UpdatedDate = '" + this.updatedDate.ToString("s") + "' ";
			string where  = "WHERE GroupIndex = " + groupIndex;

			return (update + where);
		}

		/// <summary>
		/// This method returns a SQL delete statement using the group index as the key.
		/// </summary>
		/// <returns></returns>
		public string SQLDeleteReportGroup()
		{
			string delete = "DELETE FROM tblReportGroups ";
			string where  = "WHERE GroupIndex = " + this.groupIndex;

			return (delete + where);
		}
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

				this.groupIndex        = base.getLong(row["GroupIndex"]);
				this.groupName         = base.getString(row["GroupName"]);
				this.siteIndex         = base.getInt(row["SiteIndex"]);
				this.createdBy         = base.getString(row["CreatedBy"]);
				this.updatedBy         = base.getString(row["UpdatedBy"]);
				this.orderNumber       = base.getInt(row["OrderNumber"]);
            this.securityRightEnum = base.getInt(row["SecurityRight"]);

            if (base.IsDateValid(row["CreatedDate"]) == true)
            {
               this.createdDate = base.getDateTime(row["CreatedDate"]);
            }

            if (base.IsDateValid(row["UpdatedDate"]) == true)
            {
               this.updatedDate = base.getDateTime(row["UpdatedDate"]);
            }

            if (this.securityRightEnum == -1)
            {
               this.securityRight = ReportConfigurationGroupDO.ALL_VIEW_RIGHT;
            }
            else
            {
               this.securityRight = security.RightID((RIGHT)this.securityRightEnum);
            }

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
			this.groupIndex        = -1;
			this.groupName         = "";
			this.siteIndex         = -1;
			this.orderNumber       = 99;
         this.securityRight     = "";
         this.securityRightEnum = -1;
			this.createdBy         = "";
			this.updatedBy         = "";
			this.createdDate       = System.DateTime.UtcNow;
			this.updatedDate       = System.DateTime.UtcNow;
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

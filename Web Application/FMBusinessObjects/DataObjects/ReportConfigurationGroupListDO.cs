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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	[KnownType(typeof(ReportConfigurationGroupDO))]
	public class ReportConfigurationGroupListDO : DataObject
	{
		#region Private Attributes
	   private List<ReportConfigurationGroupDO> reportGroupDOList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report configuration group list data objects.
		/// </summary>
		public ReportConfigurationGroupListDO ( )
		{
			this.init ( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns a list of report group DOs.
		/// </summary>
		[DataMember]
		public List<ReportConfigurationGroupDO> ReportGroupDOList
		{
			get { return this.reportGroupDOList; }
			private set { this.reportGroupDOList = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method returns a report group SqlCommand with no where clause.
		/// </summary>
		/// <returns></returns>
		public SqlCommand SQLGetAllReportGroups(Guid siteGuid)
		{
			SqlCommand cmd = new SqlCommand();
			
			string select = "SELECT ReportGroupGuid, GroupName, SiteGuid, CreatedBy, CreatedDate, UpdatedBy, " +
							  "UpdatedDate, OrderNumber ";
			string from = "FROM dbo.tblReportGroups ";
			string where = "WHERE ((dbo.tblReportGroups.SiteGuid IN ( SELECT map.tblEntityReportConfigurationSettingsToSite.SiteGuid" +
				" from map.tblEntityReportConfigurationSettingsToSite where MaptoSiteGuid = @SiteGuid)) OR (dbo.tblReportGroups.SiteGuid = @SiteGuid)) ";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			string orderBy = "ORDER BY dbo.tblReportGroups.OrderNumber";
			
			cmd.CommandText = select + from + where + orderBy;
			return cmd;
		}

		public override string getSelectCommand ( ) { return null; }

		public override string getInsertCommand ( ) { return null; }

		public override string getDeleteCommand ( ) { return null; }

		public override string getUpdateCommand ( ) { return null; }

		#endregion

		#region Public SQL Load Methods
		/// <summary>
		/// This method loads all the report group records into a collection.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		public bool SQLLoadAllReportGroups ( System.Data.DataSet dataSet, SecurityClass security )
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
						ReportConfigurationGroupDO reportGroup = new ReportConfigurationGroupDO ( );

						reportGroup.ReportGroupGuid = DataObject.getValue<Guid>(row["ReportGroupGuid"], Guid.Empty);
						reportGroup.GroupName = DataObject.getValue<string>(row["GroupName"], "");
						reportGroup.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Constants.Guids.SiteAdminGuid);
						reportGroup.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
						reportGroup.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
						reportGroup.UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
						reportGroup.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], reportGroup.CreatedDate);
						reportGroup.OrderNumber = DataObject.getValue<int>(row["OrderNumber"], 99);

						this.ReportGroupDOList.Add ( reportGroup );

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
		private void init ( )
		{
			this.reportGroupDOList = new List<ReportConfigurationGroupDO>();
		}
		#endregion
	}
}

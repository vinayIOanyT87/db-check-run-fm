using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace FMWebApp
{
	using System.Data;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
   using FuelsManager.FMWebApp;

	public partial class AdminDashboard : FMFormBase, IMenuDiscovery
	{
		#region Constants and Fields
		private const string UserSessionsSortDirection = "UserSessionsSortDirection";
		private const string UserSessionsSortExpression = "UserSessionsSortExpression";

		private const string NodeHealthSortDirection = "NodeHealthSortDirection";
		private const string NodeHealthSortExpression = "NodeHealthSortExpression";
		#endregion



		private FMAdminDashboardHelper helper;

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.helper = new FMAdminDashboardHelper();
			this.UserSessionsGrid.Sorting += this.UserSessionsDataGridSort;
			this.NodeHealthGrid.Sorting += this.NodeHealthDataGridSort;
         this.UserSessionsGrid.RowCommand += UserSessionsGrid_RowCommand;
         this.UserSessionsPageSizeDropDown.SelectedIndexChanged += UserSessionsPageSizeDropDown_SelectedIndexChanged;
      }

      private void UserSessionsPageSizeDropDown_SelectedIndexChanged(object sender, EventArgs e)
      {
         this.UpdateView();
      }

      private void UserSessionsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
      {
         try
         {
            if (e.CommandName == "Delete")
            {
               int index = Convert.ToInt32(e.CommandArgument);
               DeleteSessionGridItem(index);

               this.UpdateView();
            }
         }
         catch (Exception except)
         {
            this.ErrorHandler(except);
         }
      }

      protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Security.HasRight(RIGHT.ACCESS_ADMIN_DASHBOARD))
				{
					throw new Exception("Insufficient Rights");
				}

				if (!this.Page.IsPostBack)
				{
					this.UpdateView();
				}
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		private void UpdateView()
		{
			var userSessionsDataSet = this.RetrieveUserSessions();
			this.LoadUserSessionsGrid(userSessionsDataSet);

			var nodeHealthSummaryDataSet = this.RetrieveNodeHealthSummary();
			this.LoadNodeHealthSummaryGrid(nodeHealthSummaryDataSet);

		}
		private void DeleteSessionGridItem(int index)
		{
         if (!this.Security.HasRight(RIGHT.ACCESS_ADMIN_DASHBOARD))
         {
            throw new Exception("Insufficient Rights");
         }
         GridViewRow row = this.UserSessionsGrid.Rows[index];
         if (row != null && row.Cells.Count > 5)
         {
            TableCell sessionGuidCell = row.Cells[5];
            Guid token = Guid.Empty;
            if (Guid.TryParse(sessionGuidCell.Text, out token))
            {
               FMChannelHelper.MakeCall<ISessions>(
                                                    x =>
                                                    x.Purge(this.Security, token)
                                                );
            }
         }
      }

		protected void SelectAllSessions_Command(object sender, EventArgs e)
		{
			
         for (int index = 0; index < this.UserSessionsGrid.Rows.Count; index++)
         {
            GridViewRow row = this.UserSessionsGrid.Rows[index];
            if (row != null && row.Cells.Count > 1)
            {
               TableCell selectedCell = row.Cells[0];
               TableCell statusCell = row.Cells[1];
               CheckBox c = selectedCell.Controls[1] as CheckBox;
               if (c != null)
               {
						if (this.ExcludeActiveSessions.Checked == false || statusCell.Text != "Active")
							c.Checked = true;
               }
            }
         }
      }

      protected void DeselectAllSessions_Command(object sender, EventArgs e)
      {
         for (int index = 0; index < this.UserSessionsGrid.Rows.Count; index++)
         {
            GridViewRow row = this.UserSessionsGrid.Rows[index];
            if (row != null && row.Cells.Count > 1)
            {
               TableCell selectedCell = row.Cells[0];
               CheckBox c = selectedCell.Controls[1] as CheckBox;
               if (c != null)
               {
                  c.Checked = false;
               }
            }
         }
       }

      protected void DeleteSelectedSessions_Command(object sender, EventArgs e)
      {

			for (int index = 0; index < this.UserSessionsGrid.Rows.Count; index++)
			{
            GridViewRow row = this.UserSessionsGrid.Rows[index];
				if (row != null && row.Cells.Count > 1)
				{
               TableCell selectedCell = row.Cells[0];
               CheckBox c = selectedCell.Controls[1] as CheckBox;
					if (c != null && c.Checked)
					{
						DeleteSessionGridItem(index);
					}
				}
			}
         this.UpdateView();
      }

      private void LoadNodeHealthSummaryGrid(DataSet dataSet)
		{
			var gridTable = new DataTable("NodeHealthSummaryTable");

			var column = new DataColumn("nodeName", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("syncSessionGuid", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("siteName", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("siteID", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("conflicts", typeof(int));
			gridTable.Columns.Add(column);

			column = new DataColumn("lastSyncDate", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("syncCount", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("syncTimeMinutes", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("nodeHealthIndicator", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("notes", typeof(string));
			gridTable.Columns.Add(column);

			var healthTotals = new Dictionary<string, int>();
			healthTotals.Add("0", 0);
			healthTotals.Add("1", 0);
			healthTotals.Add("2", 0); 

			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];
				foreach (DataRow summaryRow in table.Rows)
				{
					DataRow row = gridTable.NewRow();
					row["syncSessionGuid"] = summaryRow["syncSessionGuid"];
					row["nodeName"] = summaryRow["nodeName"];
					row["siteName"] = summaryRow["siteName"];
					row["siteID"] = summaryRow["siteID"];
					row["conflicts"] = summaryRow["conflicts"];
					row["lastSyncDate"] = this.helper.GetLastSyncDate(summaryRow["lastSyncDate"], summaryRow["lastSyncHours"]);
					row["syncCount"] = summaryRow["syncCount"];
					row["syncTimeMinutes"] = summaryRow["syncTimeMinutes"];
					row["nodeHealthIndicator"] = summaryRow["nodeHealthIndicator"];
					row["notes"] = summaryRow["notes"];

					if (healthTotals.ContainsKey(row.Field<string>("nodeHealthIndicator")) == false)
						healthTotals[row.Field<string>("nodeHealthIndicator")] = 0;
					healthTotals[row.Field<string>("nodeHealthIndicator")] += 1;

					gridTable.Rows.Add(row);
				}
				this.NodeHealthPageSizeDropDown.SetPageSize(this.NodeHealthGrid, table.Rows.Count);
			}

			var nodeHealth = new DataView(gridTable);
			this.NodeHealthGrid.DataSource = nodeHealth;
			this.NodeHealthGrid.DataBind();

			var totalsGridTable = new DataTable("NodeHealthTotalsTable");

			column = new DataColumn("nodeHealthIndicator", typeof(string));
			totalsGridTable.Columns.Add(column);

			column = new DataColumn("total", typeof(string));
			totalsGridTable.Columns.Add(column);


			DataRow trow = totalsGridTable.NewRow();
			trow["nodeHealthIndicator"] = "Critical";
			trow["total"] = healthTotals["2"].ToString();
			totalsGridTable.Rows.Add(trow);

			trow = totalsGridTable.NewRow();
			trow["nodeHealthIndicator"] = "Caution";
			trow["total"] = healthTotals["1"].ToString();
			totalsGridTable.Rows.Add(trow);

			trow = totalsGridTable.NewRow();
			trow["nodeHealthIndicator"] = "Satisfactory";
			trow["total"] = healthTotals["0"].ToString();
			totalsGridTable.Rows.Add(trow);

			nodeHealth = new DataView(totalsGridTable);
			this.NodeHealthTotalsGrid.DataSource = nodeHealth;
			this.NodeHealthTotalsGrid.DataBind();
		}

		private void LoadUserSessionsGrid(DataSet dataSet)
		{
			var gridTable = new DataTable("UserSessionsTable");

			var column = new DataColumn("Status", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("CreatedDate", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("UserId", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("Timeout", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("SessionGuid", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("LoginSiteId", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("SiteId", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("WebServerName", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("UserGuid", typeof(string));
			gridTable.Columns.Add(column);

			column = new DataColumn("SynchronizationNodeGuid", typeof(string));
			gridTable.Columns.Add(column);

         column = new DataColumn("Delete", typeof(bool));
         gridTable.Columns.Add(column);

         if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];

				var userCount = table.Rows.Count;
				this.SetUserCountLabel(userCount);

				foreach (DataRow summaryRow in table.Rows)
				{
					DataRow row = gridTable.NewRow();
					var createdDate = ((DateTimeOffset)summaryRow["CreatedDate"]).ToString("g");  // Display as general date/time format.
					row["CreatedDate"] = createdDate;
					row["UserId"] = summaryRow["UserId"];
					row["Timeout"] = summaryRow["Timeout"];
					row["SessionGuid"] = summaryRow["SessionGuid"];
					row["LoginSiteID"] = summaryRow["LoginSiteID"];
					row["SiteID"] = summaryRow["SiteID"];
					row["WebServerName"] = summaryRow["WebServerName"];
					row["UserGuid"] = summaryRow["UserGuid"];
					row["SynchronizationNodeGuid"] = summaryRow["SynchronizationNodeGuid"];

					var updatedDate = (DateTimeOffset)summaryRow["UpdatedDate"];  // Display as general date/time format.
					int timeOut = (int)summaryRow["Timeout"];
					row["Status"] = (DateTimeOffset.Now <= updatedDate.AddMinutes(timeOut)) ? "Active" : "Expired";
					row["Delete"] = false;
					gridTable.Rows.Add(row);
				}
				this.UserSessionsPageSizeDropDown.SetPageSize(this.UserSessionsGrid, table.Rows.Count);
            this.UserSessionsPageSizeDropDown.Visible = true;

         }

			var sessions = new DataView(gridTable);
			this.UserSessionsGrid.DataSource = sessions;
			this.UserSessionsGrid.DataBind();
		}

		private void SetUserCountLabel(int userCount)
		{
			this.UserSessionsListLink.Text = string.Format("Currently Logged In Users ({0})", userCount);
		}

		private DataSet RetrieveUserSessions()
		{
			var sortExpression = this.Session[UserSessionsSortExpression] as string;
			var sortDirection = this.Session[UserSessionsSortDirection] as string;
			string orderBy = null;

			if (sortExpression != null && sortDirection != null)
			{
				orderBy = sortExpression + " " + sortDirection;
			}

			DataSet userSessions;

			if (string.IsNullOrEmpty(orderBy))
			{
				userSessions =
					FMChannelHelper.MakeCall<ISessions, DataSet>(
						sessions => sessions.GetUserSessionsList(this.Security));
			}
			else
			{
				userSessions =
					FMChannelHelper.MakeCall<ISessions, DataSet>(
						sessions => sessions.GetUserSessionsListWithOrder(this.Security, orderBy));
			}

			return userSessions;
		}

		protected void UserSessionsGridPageIndexChanged(object sender, GridViewPageEventArgs e)
		{
			try
			{
				this.UserSessionsGrid.PageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void NodeHealthGridPageIndexChanged(object sender, GridViewPageEventArgs e)
		{
			try
			{
				this.NodeHealthGrid.PageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}
		protected void NodeHealthTotalsGridPageIndexChanged(object sender, GridViewPageEventArgs e)
		{

		}
		protected void UserSessionsDataGridSort(object sender, GridViewSortEventArgs e)
		{
			var sortExpression = this.Session[UserSessionsSortExpression] as string;
			var sortDirection = this.Session[UserSessionsSortDirection] as string;

			if (e.SortExpression != sortExpression)
			{
				this.Session[UserSessionsSortDirection] = "DESC";
			}
			else
			{
				if (sortDirection == "DESC")
				{
					this.Session[UserSessionsSortDirection] = "ASC";
				}
				else
				{
					this.Session[UserSessionsSortDirection] = "DESC";
				}
			}

			this.Session[UserSessionsSortExpression] = e.SortExpression;
			this.UpdateView();
		}

		protected void NodeHealthDataGridSort(object sender, GridViewSortEventArgs e)
		{
			var sortExpression = this.Session[NodeHealthSortExpression] as string;
			var sortDirection = this.Session[NodeHealthSortDirection] as string;

			if (e.SortExpression != sortExpression)
			{
				this.Session[NodeHealthSortDirection] = "ASC";
			}
			else
			{
				if (sortDirection == "DESC")
				{
					this.Session[NodeHealthSortDirection] = "ASC";
				}
				else
				{
					this.Session[NodeHealthSortDirection] = "DESC";
				}
			}

			this.Session[NodeHealthSortExpression] = e.SortExpression;
			this.UpdateView();
		}

		private DataSet RetrieveNodeHealthSummary()
		{
			var sortExpression = this.Session[NodeHealthSortExpression] as string;
			var sortDirection = this.Session[NodeHealthSortDirection] as string;
			string orderBy = null;

			if (sortExpression != null && sortDirection != null)
			{
				orderBy = sortExpression + " " + sortDirection;
			}

			DataSet dataSet = FMChannelHelper.MakeCall<IAdminDashboard, DataSet>(x => x.GetNodeHealthSummary(this.Security, null, orderBy, null, null));

			return dataSet;
		}

		protected void NodeHealthRowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType.Equals(DataControlRowType.DataRow))
			{
				this.helper.SetNodeHealthCellColor(e);

				var conflictsColIndex = this.helper.GetColumnIndexByDataField(e.Row, "conflicts");
				if (conflictsColIndex > -1)
				{

					var syncSessionGuid = ((DataRowView)e.Row.DataItem)["SyncSessionGuid"] as string;
					var numConflicts = (int)((DataRowView)e.Row.DataItem)["conflicts"];
					if (numConflicts > 0)
					{
						HyperLink link = new HyperLink { Text = numConflicts.ToString() };
						e.Row.Cells[conflictsColIndex].Controls.Add(link);
						//link.NavigateUrl = string.Format("javascript:window.showModalDialog('../FMEntityImportWebApp/SynchronizationSessionConflicts.aspx?SessionGuid={0}&{1}','','dialogWidth: 1024px; dialogHeight: 768px')", syncSessionGuid, this.Security.CSRFTokenWithParamName);
						link.NavigateUrl = string.Format("javascript:showSessionConflict('"+syncSessionGuid+"');");

					}
				}
				var siteIDColIndex = this.helper.GetColumnIndexByDataField(e.Row, "siteID");
				if (siteIDColIndex > -1)
				{
					var siteID = ((DataRowView)e.Row.DataItem)["siteID"] as string;
					if (!string.IsNullOrWhiteSpace(siteID))
					{
						HyperLink link = new HyperLink { Text = siteID };
						e.Row.Cells[siteIDColIndex].Controls.Add(link);
						link.NavigateUrl = string.Format("SiteHealthList.aspx?SiteID={0}&{1}", siteID, this.Security.CSRFTokenWithParamName);

					}
				}
				var siteNameColIndex = this.helper.GetColumnIndexByDataField(e.Row, "SiteName");
				if (siteNameColIndex > -1)
				{
					var siteID = ((DataRowView)e.Row.DataItem)["siteID"] as string;
					var siteName = ((DataRowView)e.Row.DataItem)["siteName"] as string;
					if (!string.IsNullOrWhiteSpace(siteName))
					{
						HyperLink link = new HyperLink { Text = siteName };
						e.Row.Cells[siteNameColIndex].Controls.Add(link);
						link.NavigateUrl = string.Format("SiteHealthList.aspx?SiteID={0}&{1}", siteID, this.Security.CSRFTokenWithParamName);

					}
				}
				var nodeNameColIndex = this.helper.GetColumnIndexByDataField(e.Row, "NodeName");
				if (nodeNameColIndex > -1)
				{
					var nodeName = ((DataRowView)e.Row.DataItem)["nodeName"] as string;
					if (!string.IsNullOrWhiteSpace(nodeName))
					{
						HyperLink link = new HyperLink { Text = nodeName };
						e.Row.Cells[nodeNameColIndex].Controls.Add(link);
						link.NavigateUrl = string.Format("SiteHealthList.aspx?NodeName={0}&{1}", nodeName, this.Security.CSRFTokenWithParamName);

					}
				}

			}
		}

		protected void NodeHealthTotalsRowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType.Equals(DataControlRowType.DataRow))
			{
				this.helper.SetNodeHealthTotalsCellColor(e);
				this.SetNodeHealthTotalsUrlLink(e);
			}
		}
		private void SetNodeHealthTotalsUrlLink(GridViewRowEventArgs e)
		{
			var colIndex = this.helper.GetColumnIndexByDataField(e.Row, "nodeHealthIndicator");
			if (colIndex.Equals(-1))
			{
				return;
			}

			TableCell cell = e.Row.Cells[colIndex];
			HyperLink link = new HyperLink { Text = cell.Text };
 			e.Row.Cells[colIndex].Controls.Add(link);
			string nodeHealth = "2";
			switch (cell.Text){
				case "Critical": nodeHealth = "2";
					break;
				case "Caution": nodeHealth = "1";
					break;
				case "Satisfactory": nodeHealth = "0";
                    link.ForeColor = System.Drawing.Color.White;
					break;

			}
			link.NavigateUrl = string.Format("../FMWebApp/SiteHealthList.aspx?NodeHealth={0}&{1}", nodeHealth, this.Security.CSRFTokenWithParamName);


		}


		//private void SetNodeHealthUrlLink(GridViewRowEventArgs e)
		//{
		//	var nodeNameColIndex = this.helper.GetColumnIndexByDataField(e.Row, "NodeName");
		//	var sessionGuidColIndex = this.helper.GetColumnIndexByDataField(e.Row, "SyncSessionGuid");
		//	if (nodeNameColIndex.Equals(-1) || sessionGuidColIndex.Equals(-1))
		//	{
		//		return;
		//	}

		//	TableCell sessionGuidCell = e.Row.Cells[sessionGuidColIndex];
		//	TableCell nodeNameCell = e.Row.Cells[nodeNameColIndex];
		//	HyperLink link = new HyperLink { Text = nodeNameCell.Text };
		//	e.Row.Cells[nodeNameColIndex].Controls.Add(link);
		//	link.NavigateUrl = string.Format("javascript:window.showModalDialog('../FMEntityImportWebApp/SynchronizationSessionConflicts.aspx?SessionGuid={0}&{1}')", sessionGuidCell.Text, this.Security.CSRFTokenWithParamName);
		//	sessionGuidCell.Visible = false;
		
		//}

		private void SetQueueDescriptionUrlLink(GridViewRowEventArgs e)
		{
			var colIndex = this.helper.GetColumnIndexByDataField(e.Row, "description");
			if (colIndex.Equals(-1))
			{
				return;
			}

			TableCell cell = e.Row.Cells[colIndex];
			HyperLink link = new HyperLink { Text = cell.Text };
			e.Row.Cells[colIndex].Controls.Add(link);
			int rowStatus = this.helper.GetEnterpriseQueueRowStatusFromText(cell.Text);
			link.NavigateUrl = string.Format("../FMWebApp/EnterpriseQueueList.aspx?status={0}&{1}", rowStatus, this.Security.CSRFTokenWithParamName);
		}

		protected void UserSessionsListLinkOnClick(object sender, EventArgs e)
		{
			this.Redirect("UserSessionsList.aspx");
		}

		protected void NodeHealthListLinkOnClick(object sender, EventArgs e)
		{
			this.Redirect("SiteHealthList.aspx");
		}

		/// <summary>
		/// The has DESC enterprise key.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private bool HasEnterpriseKey()
		{
			return FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsEnterpriseKey());
		}

		public bool HasAdminDashboardPermission(SecurityClass security)
		{
			return security.HasRight(RIGHT.ACCESS_ADMIN_DASHBOARD);
		}

		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
        {
			var menuItems = new List<FMMenuItem>();
			if (this.HasEnterpriseKey())
			{
				if (this.HasAdminDashboardPermission(security))
				{
					var adminDashboardMenuItem = new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ADMIN_SYSTEM_DASHBOARD,
						RootMenuName = "Administration",
						CategoryName = "System",
						ItemName = "Administrator Dashboard",
						NavigateUrl = "AdminDashboard.aspx"
					};
					menuItems.Add(adminDashboardMenuItem);
				}
			}
			return menuItems;
		}
    }
}
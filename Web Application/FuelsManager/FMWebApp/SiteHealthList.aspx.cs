using System;
using System.Web.UI.WebControls;

namespace FMWebApp
{
	using System.Data;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public partial class SiteHealthList : FMFormBase
	{
		#region Constants and Fields
		private const string NodeHealthSortDirection = "NodeHealthSortDirection";
		private const string NodeHealthSortExpression = "NodeHealthSortExpression";
		#endregion

		private FMAdminDashboardHelper helper;

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
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
					string siteID = Request.Params["SiteID"];
					string nodeName = Request.Params["NodeName"];
					if (siteID != null)
					{
						siteID = siteID.Trim().Left(256);
					}
					if (nodeName != null)
					{
						nodeName = nodeName.Trim().Left(256);
					}
					this.Session["NodeName"] = nodeName;
					this.Session["SiteID"] = siteID;
					this.UpdateView();
				}
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		private void InitializeComponent()
		{
			this.helper = new FMAdminDashboardHelper();
			this.NodeHealthGrid.Sorting += this.NodeHealthDataGridSort;
		}

		protected void ReturnButtonClick(object sender, EventArgs e)
		{
			this.Redirect("AdminDashboard.aspx");
		}

		protected void PageSizeDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception error)
			{
				this.ErrorHandler(error);
			}
		}

		protected void NodeHealthRowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType.Equals(DataControlRowType.DataRow))
			{
				this.helper.SetNodeHealthCellColor(e);
				var conflictsColIndex = this.helper.GetColumnIndexByDataField(e.Row, "conflicts");
				if (conflictsColIndex.Equals(-1) )
				{
					return;
				}

				var syncSessionGuid = ((DataRowView)e.Row.DataItem)["SyncSessionGuid"] as string;
				var numConflicts = (int)((DataRowView)e.Row.DataItem)["conflicts"];
				if (numConflicts > 0)
				{
					HyperLink link = new HyperLink { Text = numConflicts.ToString() };
					e.Row.Cells[conflictsColIndex].Controls.Add(link);
					//link.NavigateUrl = string.Format("javascript:window.showModalDialog('../FMEntityImportWebApp/SynchronizationSessionConflicts.aspx?SessionGuid={0}&{1}','','dialogWidth: 1024px; dialogHeight: 768px')", syncSessionGuid, this.Security.CSRFTokenWithParamName);
					link.NavigateUrl = string.Format("javascript:showSessionConflict('" + syncSessionGuid + "');");
				}

			}

		}
		protected void NodeHealthDataGridSort(object sender, GridViewSortEventArgs e)
		{
			var sortExpression = this.Session[NodeHealthSortExpression] as string;
			var sortDirection = this.Session[NodeHealthSortDirection] as string;

			if (e.SortExpression != sortExpression)
			{
				this.Session[NodeHealthSortDirection] = "DESC";
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

		private void UpdateView()
		{
			var nodeHealth = Request.Params["NodeHealth"];
			var nodeHealthDetailsDataSet = this.RetrieveNodeHealthSummary(nodeHealth);

			this.LoadNodeHealthSummaryGrid(nodeHealthDetailsDataSet);
		}

		private DataSet RetrieveNodeHealthSummary(string nodeHealth)
		{
			var sortExpression = this.Session[NodeHealthSortExpression] as string;
			var sortDirection = this.Session[NodeHealthSortDirection] as string;
			var nodeName = this.Session["NodeName"] as string;
			var siteID = this.Session["SiteID"] as string;
			string orderBy = null;

			if (sortExpression != null && sortDirection != null)
			{
				orderBy = sortExpression + " " + sortDirection;
			}

			DataSet dataSet = FMChannelHelper.MakeCall<IAdminDashboard, DataSet>(x => x.GetNodeHealthSummary(this.Security, nodeHealth, orderBy, siteID, nodeName));


			return dataSet;
		}

		private void LoadNodeHealthSummaryGrid(DataSet dataSet)
		{
			var gridTable = new DataTable("NodeHealthSummaryTable");

			var column = new DataColumn("nodeName", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("syncSessionGuid", typeof(string));
			gridTable.Columns.Add(column);


			column = new DataColumn("siteName", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("siteID", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("conflicts", Type.GetType("System.Int32"));
			gridTable.Columns.Add(column);

			column = new DataColumn("lastSyncDate", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("syncCount", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("syncTimeMinutes", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("nodeHealthIndicator", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("notes", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

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

					gridTable.Rows.Add(row);
				}
				this.NodeHealthPageSizeDropDown.SetPageSize(this.NodeHealthGrid, table.Rows.Count);
			}

			var nodeHealth = new DataView(gridTable);
			this.NodeHealthGrid.DataSource = nodeHealth;
			this.NodeHealthGrid.DataBind();
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
	}
}
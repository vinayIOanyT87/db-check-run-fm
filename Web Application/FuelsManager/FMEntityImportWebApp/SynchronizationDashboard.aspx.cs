using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FuelsManager.FMEntityImportWebApp
{
	using System.Data;
	using System.Drawing;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FuelsManager.FMWebApp;

	public partial class SynchronizationDashboard : FMFormBase
	{
		#region Constants and Fields
		private const string UserSessionsSortDirection = "UserSessionsSortDirection";
		private const string UserSessionsSortExpression = "UserSessionsSortExpression";

		private const string NodeHealthSortDirection = "NodeHealthSortDirection";
		private const string NodeHealthSortExpression = "NodeHealthSortExpression";
		#endregion

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.NodeHealthGrid.Sorting += this.NodeHealthDataGridSort;
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


		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Security.HasRight(RIGHT.ACCESS_SYNC_DASHBOARD))
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

			var nodeHealthSummaryDataSet = this.RetrieveNodeHealthSummary();
			this.LoadNodeHealthSummaryGrid(nodeHealthSummaryDataSet);
		}

		

		

		private void LoadNodeHealthSummaryGrid(DataSet dataSet)
		{
			var gridTable = new DataTable("NodeHealthSummaryTable");

			var column = new DataColumn("nodeName", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("siteName", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("dodaac", Type.GetType("System.String"));
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

			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];
				foreach (DataRow summaryRow in table.Rows)
				{
					DataRow row = gridTable.NewRow();
					row["nodeName"] = summaryRow["nodeName"];
					row["siteName"] = summaryRow["siteName"];
					row["dodaac"] = summaryRow["dodaac"];
					row["conflicts"] = summaryRow["conflicts"];
					row["lastSyncDate"] = this.GetLastSyncDate(summaryRow["lastSyncDate"], summaryRow["lastSyncHours"]);
					row["syncCount"] = summaryRow["syncCount"];
					row["syncTimeMinutes"] = summaryRow["syncTimeMinutes"];
					row["nodeHealthIndicator"] = summaryRow["nodeHealthIndicator"];
					
					gridTable.Rows.Add(row);
				}
				this.NodeHealthPageSizeDropDown.SetPageSize(this.NodeHealthGrid, table.Rows.Count);
			}

			var nodeHealth = new DataView(gridTable);
			this.NodeHealthGrid.DataSource = nodeHealth;
			this.NodeHealthGrid.DataBind();
		}

		private string GetLastSyncDate(object lastSyncDate, object lastSyncHours)
		{
			string syncDateTime = "";
			string comment = "No sync detected.";

			if (lastSyncDate != null && lastSyncDate != DBNull.Value)
			{
				syncDateTime = ((DateTimeOffset)lastSyncDate).ToString("g"); // Display as general date/time format.
				var syncHours = Convert.ToInt32(lastSyncHours);

				var totalSyncHours = Convert.ToInt32(syncHours);
				var totalSyncDays = Math.Truncate((totalSyncHours * 1d) / 24);
				var totalSyncMonths = Math.Truncate((totalSyncDays % 356) / 30);

				
				if (totalSyncMonths >= 1.0)
				{
					comment = string.Format("{0} months ago", totalSyncMonths);
				}
				else if (totalSyncDays >= 1.0)
				{
					comment = string.Format("{0} days ago", totalSyncDays);
				}
				else
				{
					comment = string.Format("{0} hours ago", totalSyncHours);
				}
			}

			return string.Format("{0} ({1})", syncDateTime, comment);
		}

		

		private DataSet RetrieveNodeHealthSummary()
		{
			var sortExpression = this.Session[NodeHealthSortExpression] as string;
			var sortDirection = this.Session[NodeHealthSortDirection] as string;
			string orderBy = null;
			int nodeStatus = -1;

			int.TryParse(statusFilterDropDown.SelectedValue, out nodeStatus);

			if (sortExpression != null && sortDirection != null)
			{
				orderBy = sortExpression + " " + sortDirection;
			}

			DataSet nodeHealth;

			if (string.IsNullOrEmpty(orderBy))
			{
				nodeHealth =
					FMChannelHelper.MakeCall<ISyncSessionLogs, DataSet>(results => results.GetNodeHealthSummary(this.Security, nodeStatus));
			}
			else
			{
				nodeHealth =
					FMChannelHelper.MakeCall<ISyncSessionLogs, DataSet>(results => results.GetNodeHealthSummaryWithOrder(this.Security, orderBy, nodeStatus));
			}

			return nodeHealth;
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

		protected void NodeHealthRowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType.Equals(DataControlRowType.DataRow))
			{
				this.SetNodeHealthCellColor(e);
			}
		}

		private void SetNodeHealthCellColor(GridViewRowEventArgs e)
		{
			var colIndex = this.GetColumnIndexByName(e.Row, "nodeHealthIndicator");
			if (colIndex.Equals(-1))
			{
				return;
			}

			// "Hide" the text in Node Health column by setting fore and back color to the same value.
			var nodeHealthIndicator = e.Row.Cells[colIndex].Text;
			if (nodeHealthIndicator == "0")
			{
				e.Row.Cells[colIndex].BackColor = Color.Green;
				e.Row.Cells[colIndex].ForeColor = Color.Green;
			}
			else if (nodeHealthIndicator == "1")
			{
				e.Row.Cells[colIndex].BackColor = Color.Yellow;
				e.Row.Cells[colIndex].ForeColor = Color.Yellow;
			}
			else  // Value should be "2"
			{
				e.Row.Cells[colIndex].BackColor = Color.Red;
				e.Row.Cells[colIndex].ForeColor = Color.Red;
			}
		}

		private int GetColumnIndexByName(GridViewRow row, string columnName)
		{
			var columnIndex = 0;
			foreach (DataControlFieldCell cell in row.Cells)
			{
				if (cell.ContainingField is BoundField)
				{
					if (((BoundField)cell.ContainingField).DataField.Equals(columnName))
					{
						return columnIndex;
					}
				}
				columnIndex++; // keep adding 1 while we don't have the correct name
			}
			return -1;
		}

		protected void EbsQueueRowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType.Equals(DataControlRowType.DataRow))
			{
				this.SetQueueDescriptionUrlLink(e);
			}
		}

		private void SetQueueDescriptionUrlLink(GridViewRowEventArgs e)
		{
			var colIndex = this.GetColumnIndexByName(e.Row, "description");
			if (colIndex.Equals(-1))
			{
				return;
			}

			TableCell cell = e.Row.Cells[colIndex];
			//HyperLink link = new HyperLink { Text = cell.Text };
			//e.Row.Cells[colIndex].Controls.Add(link);
			//link.NavigateUrl = "http://www.google.com";
		}

		protected void StatusFilterDropDownOnSelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}
	}
}
using System;
using System.Web.UI.WebControls;

namespace FMWebApp
{
	using System.Data;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public partial class UserSessionsList : FMFormBase
	{
		#region Constants and Fields
		private const string UserSessionsSortDirection = "UserSessionsSortDirection";
		private const string UserSessionsSortExpression = "UserSessionsSortExpression";
		#endregion

		public UserSessionsList()
		{
			this.UserSessionRowCount = 0;
		}

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.UserSessionsGrid.Sorting += this.UserSessionsDataGridSort;
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
			this.UpdateView(this.UserSessionsPageSizeDropDown);
		}

		private void UpdateView(FMControls.FMPageSizeDropDown pageSizeDropDown)
		{
			if (pageSizeDropDown != null)
			{
				pageSizeDropDown.SetPageSize(this.UserSessionsGrid, this.UserSessionRowCount);
			}

			var userSessions = this.RetrieveUserSessions();
			this.LoadUserSessionsGrid(userSessions);
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

		private void LoadUserSessionsGrid(DataSet dataSet)
		{
			var gridTable = new DataTable("UserSessionsTable");

			var column = new DataColumn("CreatedDate", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("UserId", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("Timeout", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("SessionGuid", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("LoginSiteId", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("SiteId", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("WebServerName", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("UserGuid", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("SynchronizationNodeGuid", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];
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

					gridTable.Rows.Add(row);
				}
				
				this.UserSessionRowCount = table.Rows.Count;
				this.UserSessionsPageSizeDropDown.SetPageSize(this.UserSessionsGrid, this.UserSessionRowCount);
			}

			var sessions = new DataView(gridTable);
			this.UserSessionsGrid.DataSource = sessions;
			this.UserSessionsGrid.DataBind();
		}

		private int UserSessionRowCount { get; set; }

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

		protected void ReturnButtonClick(object sender, EventArgs e)
		{
			this.Redirect("AdminDashboard.aspx");
		}
	}
}
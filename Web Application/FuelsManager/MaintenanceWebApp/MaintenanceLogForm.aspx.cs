
namespace FuelsManager.MaintenanceWebApp
{
	using System;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;
    using FMCore;
    using FMWebApp;

	public partial class MaintenanceLogForm : FMFormBaseAjax
	{
		#region Private attributes
		private const string MaintenancelogSortDirection = "MaintenanceLog_SortDirection";
		private const string MaintenancelogSortExpresion = "MaintenanceLog_SortExpression";

		// Columns 3 through 8 are the data columns from EquipmentQualityTagLogsClass.GetSQL().
		private const int MaintenancelogDeleteButtonCol = 1;
		private const int MaintenancelogAssetTypeHiddenCol = 2;
		private const int MaintenancelogIndexHiddenCol = 3;
		#endregion

		#region Page State Management
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				// This is the first time through.
				if (this.IsPostBack == false)
				{
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		// This method will initialize the security and data dictionary classes.
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// This is the first time through.
				if (this.IsPostBack == false)
				{
					this.InitializeView();
					this.ApplyDataDictionary();
					this.FMButtonAddBottom.Enabled = this.Security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD);
					this.FMButtonAddTop.Enabled = this.Security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD);
					if (this.Session["FMGridViewMaintenanceLog.PageIndex"] != null)
					{
						this.FMGridViewMaintenanceLog.PageIndex = (int)this.Session["FMGridViewMaintenanceLog.PageIndex"];
					}
					this.UpdateView();
				}

			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			// Initialize the session variable.
		}

		//
		protected void InitializeView()
		{
			// Populate and initialize the DateFilterTypeDropDown drop down.
			this.DateFilterTypeDropDown.Items.Clear();
			this.DateFilterTypeDropDown.Items.Add(new ListItem(this.GetTranslatedText("")));
			this.DateFilterTypeDropDown.Items.Add(new ListItem(this.GetTranslatedText("Estimated Return To Service")));
			this.DateFilterTypeDropDown.Items.Add(new ListItem(this.GetTranslatedText("QC Due Date")));
			this.DateFilterTypeDropDown.SelectedIndex = 0;

			// Populate and initialize the date controls.
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, getMemberSites: true,
																		getSchedulesAndProcessVariables: true, bGetAssociatedAliases: true)
																);

			DateTimeOffset siteTimeNow = TimeConverter.Now(site);
			this.StartDate.CurrentValue = siteTimeNow;
			this.StartDate.Enabled = false;

			this.EndDate.CurrentValue = siteTimeNow;
			this.EndDate.Enabled = false;

		}

		// 
		protected void UpdateView()
		{
			try
			{
				string sDateType = this.DateFilterTypeDropDown.SelectedItem.Text;
				DateTimeOffset dateStart = this.StartDate.CurrentValue;
				DateTimeOffset dateEnd = this.EndDate.CurrentValue;

				DataSet ds = FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, DataSet>(
					logs => logs.GetDataSet(this.Security, this.HistoricalDataCheckBox.Checked, sDateType, dateStart, dateEnd, Guid.Empty));
				DataTable table = ds.Tables[0];
				table.ConvertDateTimeOffsetColumns();
				this.FMGridViewMaintenanceLog.DataSource = new DataView(table);
				this.FMGridViewMaintenanceLog.DataBind();
			}
			catch (Exception ex)
			{
				var table = new DataTable();
				this.FMGridViewMaintenanceLog.DataSource = new DataView(table);
				this.FMGridViewMaintenanceLog.DataBind();
				this.ErrorHandler(ex);
			}
		}

		// Overwrite the controls' text from the Dictionary.
		private void ApplyDataDictionary()
		{
			//TitleLabel.Text = Dictionary.getNameFromGlobalDictionary("??");
		}

		protected void RefreshButtonOnClick(object sender, EventArgs e)
		{
			// Update the page with the new contents.
			this.FMGridViewMaintenanceLog.PageIndex = 0;
			this.UpdateView();
		}

		protected void AddButtonOnClick(object sender, EventArgs e)
		{
			// Initialize the session variable.
			this.Session["ReturnPageFromMaintenanceAddRecordForm"] = "MaintenanceLogForm.aspx";
			this.Redirect("MaintenanceAddRecordForm.aspx?MODE=ADD");
		}
		#endregion

		#region Message handlers
		protected void DateFilterTypeDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			string sDateType = this.DateFilterTypeDropDown.SelectedItem.Text;

			this.StartDate.Enabled = ("" != sDateType);
			this.EndDate.Enabled = this.StartDate.Enabled;
		}

		private void InitializeComponent()
		{
			this.FMGridViewMaintenanceLog.PreRender += this.FMGridViewMaintenanceLogPreRender;
			this.FMGridViewMaintenanceLog.RowDataBound += this.FMGridViewMaintenanceLogRowDataBound;
			this.FMGridViewMaintenanceLog.RowCommand += this.FMGridViewMaintenanceLogRowCommandReceived;
			this.FMGridViewMaintenanceLog.Sorting += this.FMGridViewMaintenanceLogSorting;

		}

		protected void FMGridViewMaintenanceLogPreRender(object sender, EventArgs e)
		{
			if (this.FMGridViewMaintenanceLog.Controls.Count > 0)
			{
				// We do this here because autocreated columns do not exist as an object in the grid.
				var t = this.FMGridViewMaintenanceLog.Controls[0] as Table;
				if (t != null && t.Rows.Count > 0)
				{
					if (t.Rows[0].Cells.Count > MaintenancelogAssetTypeHiddenCol)
					{
						t.Rows[0].Cells[MaintenancelogAssetTypeHiddenCol].Visible = false;
					}
					if (t.Rows[0].Cells.Count > MaintenancelogIndexHiddenCol)
					{
						t.Rows[0].Cells[MaintenancelogIndexHiddenCol].Visible = false;
					}
				}
			}
		}

		protected void FMGridViewMaintenanceLogRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow ||
					 e.Row.RowType == DataControlRowType.Header ||
					 e.Row.RowType == DataControlRowType.Footer)
				{
					e.Row.Cells[MaintenancelogAssetTypeHiddenCol].Visible = false;
					e.Row.Cells[MaintenancelogIndexHiddenCol].Visible = false;
				}

				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					var cell = e.Row.Cells[MaintenancelogDeleteButtonCol] as DataControlFieldCell;

					if (cell != null)
					{
						var commandField = cell.ContainingField as FMControls.FMDeleteCommandField;
						if (commandField != null)
						{
							commandField.Enabled = this.Security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD);
						}
					}
				}
			}

			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

		}

		// User clicked a button.
		protected void FMGridViewMaintenanceLogRowCommandReceived(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				if (e.CommandName.Equals("New"))
				{
					int nRow = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.FMGridViewMaintenanceLog.Rows[nRow];

					TableCell indexCell = row.Cells[MaintenancelogAssetTypeHiddenCol];
					string sAssetType = indexCell.Text;

					this.Session.Remove(MaintenanceAddRecordForm.MaintenancelogSessionKey);
					indexCell = row.Cells[MaintenancelogIndexHiddenCol];
					Guid maintenanceLogGuid = Guid.Parse(indexCell.Text);
					if (sAssetType.Equals("EQUIPMENT"))
					{
						EquipmentMaintenanceLogClass oEquipmentMaintenanceLog = FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, EquipmentMaintenanceLogClass>(
							logs => logs.Get(this.Security, maintenanceLogGuid));
						if (oEquipmentMaintenanceLog == null)
						{
							throw new Exception("Equipment Maintenance Log not found.");
						}
						this.Session[MaintenanceAddRecordForm.MaintenancelogSessionKey] = oEquipmentMaintenanceLog;
					}
					else
					{
						TankMaintenanceLogClass oTankMaintenanceLog = FMChannelHelper.MakeCall<ITankMaintenanceLogs, TankMaintenanceLogClass>(
							logs => logs.Get(this.Security, maintenanceLogGuid));
						if (oTankMaintenanceLog == null)
						{
							throw new Exception("Tank Maintenance Log not found.");
						}
						this.Session[MaintenanceAddRecordForm.MaintenancelogSessionKey] = oTankMaintenanceLog;
					}
					this.Session["ReturnPageFromMaintenanceAddRecordForm"] = "MaintenanceLogForm.aspx";
					this.Redirect("MaintenanceAddRecordForm.aspx");
				}
				else if (e.CommandName.Equals("Delete"))
				{

					this.GetSecurity();
					if (!this.Security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD))
					{
						throw new FMInsufficientRightsException();
					}

					int nRow = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.FMGridViewMaintenanceLog.Rows[nRow];

					TableCell indexCell = row.Cells[MaintenancelogAssetTypeHiddenCol];
					string sAssetType = indexCell.Text;

					indexCell = row.Cells[MaintenancelogIndexHiddenCol];
					Guid maintenanceLogGuid = Guid.Parse(indexCell.Text);

					if (sAssetType.Equals("EQUIPMENT"))
					{
						FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs>(
							logs => logs.Purge(this.Security, maintenanceLogGuid));
					}
					else//TANK
					{
						FMChannelHelper.MakeCall<ITankMaintenanceLogs>(
							logs => logs.Purge(this.Security, maintenanceLogGuid));
					}

					this.FMGridViewMaintenanceLog.SelectedIndex = -1;
					this.Session.Remove("Index");
					if (this.FMGridViewMaintenanceLog.Rows.Count == 1
					&& this.FMGridViewMaintenanceLog.PageIndex > 0)
						this.FMGridViewMaintenanceLog.PageIndex--;
					this.Session["FMGridViewMaintenanceLog.PageIndex"] = this.FMGridViewMaintenanceLog.PageIndex;

					this.UpdateView();

					this.FMGridViewMaintenanceLog.DeleteRow(row.RowIndex);

				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		#region Sorting the GridView - See http://forums.asp.net/t/1322269.aspx
		protected void FMGridViewMaintenanceLogSorting(object sender, GridViewSortEventArgs e)
		{
			try
			{
				string sDateType = this.DateFilterTypeDropDown.SelectedItem.Text;
				DateTimeOffset dateStart = this.StartDate.CurrentValue;
				DateTimeOffset dateEnd = this.EndDate.CurrentValue;

				DataSet ds = FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, DataSet>(
					logs => logs.GetDataSet(this.Security, this.HistoricalDataCheckBox.Checked, sDateType, dateStart, dateEnd, Guid.Empty));
				DataTable table = ds.Tables[0];

				this.GridViewSortExpression = e.SortExpression;
				int pageIndex = this.FMGridViewMaintenanceLog.PageIndex;
				this.FMGridViewMaintenanceLog.DataSource = this.SortDataTable(table, /* bIsPageIndexChanging */ false);
				this.FMGridViewMaintenanceLog.DataBind();
				this.FMGridViewMaintenanceLog.PageIndex = pageIndex;
			}

			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		private string GridViewSortDirection
		{
			get { return this.ViewState[MaintenancelogSortDirection] as string ?? "ASC"; }
			set { this.ViewState[MaintenancelogSortDirection] = value; }
		}

		private string GridViewSortExpression
		{
			get { return this.ViewState[MaintenancelogSortExpresion] as string ?? "'Asset ID'"; }
			set { this.ViewState[MaintenancelogSortExpresion] = value; }
		}

		private string GetSortDirection()
		{
			switch (this.GridViewSortDirection)
			{
				case "ASC": this.GridViewSortDirection = "DESC"; break;
				case "DESC": this.GridViewSortDirection = "ASC"; break;
			}

			return this.GridViewSortDirection;
		}

		protected DataView SortDataTable(DataTable dataTable, bool isPageIndexChanging)
		{
			if (dataTable != null)
			{
				var dataView = new DataView(dataTable);

				if (this.GridViewSortExpression != string.Empty)
				{
					if (isPageIndexChanging)
					{
						dataView.Sort = string.Format("{0} {1}", this.GridViewSortExpression, this.GridViewSortDirection);
					}
					else
					{
						dataView.Sort = string.Format("{0} {1}", this.GridViewSortExpression, this.GetSortDirection());
					}
				}

				return dataView;
			}

			return new DataView();
		}

		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}


		#endregion
	}
}

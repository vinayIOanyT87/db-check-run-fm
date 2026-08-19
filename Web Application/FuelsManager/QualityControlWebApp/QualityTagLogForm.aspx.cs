// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityTagLogForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.QualityControlWebApp
{
	using System;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using FuelsManager.FMWebApp;

	public partial class QualityTagLogForm : FMFormBaseAjax
	{
		#region Constants and Fields

		public const string QualityTagLogSelectionSessionKey = "QualityControlWebApp.QualityTagLogForm.Selection";

		// Columns 3 through 8 are the data columns from EquipmentQualityTagLogsClass.GetSQL().
		private const int QualityTagLogAssetTypeHiddenCol = 2;
		private const int QualityTagLogDeleteButtonCol = 1;
		private const int QualityTagLogIdentityGuidHiddenCol = 3;
		private const int QualityTagLogRemovedByCol = 10;
		private const string QualityTagLogSortDirection = "QualityTagLog_SortDirection";
		private const string QualityTagLogSortExpresion = "QualityTagLog_SortExpression";
		private const int QualityTagLogViewButtonCol = 0;
		private const int QualityTagLogTaggedDateCol = 6;
		private const int QualityTagLogRemovedDateCol = 9;

		#endregion

		#region Properties

		public string TimePattern { get; set; }
		public string DatePattern { get; set; }

		private string GridViewSortDirection
		{
			get
			{
				return this.ViewState[QualityTagLogSortDirection] as string ?? "ASC";
			}

			set
			{
				this.ViewState[QualityTagLogSortDirection] = value;
			}
		}

		private string GridViewSortExpression
		{
			get
			{
				return this.ViewState[QualityTagLogSortExpresion] as string ?? "'Asset ID'";
			}

			set
			{
				this.ViewState[QualityTagLogSortExpresion] = value;
			}
		}

		#endregion

		#region Methods

		protected void AddButtonOnClick(object sender, EventArgs e)
		{
			// Initialize the session variable.
			this.Session.Remove(QualityTagLogSelectionSessionKey);
			this.Session["ReturnPageFromQualityTagAddRecordForm"] = "../QualityControlWebApp/QualityTagLogForm.aspx";

			this.Redirect("QualityTagAddRecordForm.aspx?MODE=ADD");
		}

		protected void DateFilterTypeDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			string sDateType = this.DateFilterTypeDropDown.SelectedItem.Text;

			this.StartDate.Enabled = string.Empty != sDateType;
			this.EndDate.Enabled = this.StartDate.Enabled;
		}

		protected void FMGridViewQualityTagLogPreRender(object sender, EventArgs e)
		{
			if (this.FMGridViewQualityTagLog.Controls.Count > 0)
			{
				// We do this here because autocreated columns do not exist as an object in the grid.
				var t = this.FMGridViewQualityTagLog.Controls[0] as Table;
				if (t != null && t.Rows.Count > 0)
				{
					if (t.Rows[0].Cells.Count > QualityTagLogAssetTypeHiddenCol)
					{
						t.Rows[0].Cells[QualityTagLogAssetTypeHiddenCol].Visible = false;
					}

					if (t.Rows[0].Cells.Count > QualityTagLogIdentityGuidHiddenCol)
					{
						t.Rows[0].Cells[QualityTagLogIdentityGuidHiddenCol].Visible = false;
					}
				}
			}
		}

		protected void FMGridViewQualityTagLogRowCommandReceived(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				if (e.CommandName != "Edit" && e.CommandName != "Delete")
				{
					return;
				}

				int nRow = Convert.ToInt32(e.CommandArgument);
				GridViewRow row = this.FMGridViewQualityTagLog.Rows[nRow];

				TableCell assetTypeCell = row.Cells[QualityTagLogAssetTypeHiddenCol];

				// e.g., "EQUIPMENT"
				string sAssetType = assetTypeCell.Text;

				TableCell identityGuidCell = row.Cells[QualityTagLogIdentityGuidHiddenCol];
				Guid qualityTagLogGuid = Guid.Parse(identityGuidCell.Text);

				if (e.CommandName == "Edit")
				{
					if (sAssetType == "EQUIPMENT")
					{
						this.Session["QualityTagLogGuid"] =
							FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, EquipmentQualityTagLogClass>(
								logs => logs.Get(this.Security, qualityTagLogGuid));
					}
					else
					{
						this.Session["QualityTagLogGuid"] =
							FMChannelHelper.MakeCall<ITankQualityTagLogs, TankQualityTagLogClass>(
								logs => logs.Get(this.Security, qualityTagLogGuid));
					}

					this.Redirect("QualityTagAddRecordForm.aspx?MODE=EDIT");
				}
				else if (e.CommandName == "Delete")
				{
					this.GetSecurity();

					if (sAssetType == "EQUIPMENT")
					{
						FMChannelHelper.MakeCall<IEquipmentQualityTagLogs>(logs => logs.Purge(this.Security, qualityTagLogGuid));
					}
					else
					{
						FMChannelHelper.MakeCall<ITankQualityTagLogs>(logs => logs.Purge(this.Security, qualityTagLogGuid));
					}

					this.FMGridViewQualityTagLog.SelectedIndex = -1;
					this.Session.Remove("IdentityGuid");

					if (this.FMGridViewQualityTagLog.Rows.Count == 1 && this.FMGridViewQualityTagLog.PageIndex > 0)
					{
						this.FMGridViewQualityTagLog.PageIndex--;
					}

					this.UpdateView();

					this.FMGridViewQualityTagLog.DeleteRow(row.RowIndex);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void FMGridViewQualityTagLogRowDataBound(object sender, GridViewRowEventArgs e)
		{
			// We do this here because autocreated columns do not exist as an object in the grid.
			if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header
			    || e.Row.RowType == DataControlRowType.Footer)
			{
				e.Row.Cells[QualityTagLogAssetTypeHiddenCol].Visible = false;
				e.Row.Cells[QualityTagLogIdentityGuidHiddenCol].Visible = false;

				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					var cell = e.Row.Cells[QualityTagLogDeleteButtonCol] as DataControlFieldCell;
					TableCell removedbycell = e.Row.Cells[QualityTagLogRemovedByCol];

					if (cell != null)
					{
						var commandField = cell.ContainingField as FMDeleteCommandField;
						if (commandField != null)
						{
							// if the tag has not been removed then it can not be deleted
							removedbycell.Text = removedbycell.Text.Replace("&nbsp;", " ");
							if (removedbycell.Text == " " || removedbycell.Text == string.Empty)
							{
								commandField.Enabled = false;
							}
							else
							{
								commandField.Enabled = this.Security.HasRight(RIGHT.MODIFY_QUALITYTAG_LOGS);
							}
						}
					}

					cell = e.Row.Cells[QualityTagLogViewButtonCol] as DataControlFieldCell;

					if (cell != null)
					{
						var commandField = cell.ContainingField as FMEditCommandField;
						if (commandField != null)
						{
							commandField.Enabled = this.Security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
							                       || this.Security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD);
						}
					}

					cell = e.Row.Cells[QualityTagLogTaggedDateCol] as DataControlFieldCell;
					if (cell != null)
					{
						DateTimeOffset date;

						var success = DateTimeOffset.TryParse(cell.Text, out date);

						if (success)
						{
							cell.Text = date.DateTime.ToString(this.DatePattern + " " + this.TimePattern);
						}
					}

					cell = e.Row.Cells[QualityTagLogRemovedDateCol] as DataControlFieldCell;
					if ( cell != null )
					{
						DateTimeOffset date;

						var success = DateTimeOffset.TryParse( cell.Text, out date );

						if ( success )
						{
							cell.Text = date.DateTime.ToString( this.DatePattern + " " + this.TimePattern );
						}
					}
				}
			}
		}

		protected void FMGridViewQualityTagLogSorting(object sender, GridViewSortEventArgs e)
		{
			string dateType = this.DateFilterTypeDropDown.SelectedItem.Text;
			DateTimeOffset dateStart = this.StartDate.CurrentValue;
			DateTimeOffset dateEnd = this.EndDate.CurrentValue;

			DataSet ds =
				FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, DataSet>(
					logs =>
					logs.GetDataSet(
						this.Security, 
						this.HistoricalDataCheckBox.Checked, 
						dateType, 
						dateStart, 
						dateEnd, 
						this.QualityTagDropDown.SelectedValue, 
						this.TaggedByDropDown.SelectedValue, 
						this.RemovedByDropDown.SelectedValue, 
						this.AssetIDDropDownList.SelectedValue, 
						this.TagStatusFilterDropDown.SelectedValue));

			DataTable table = ds.Tables[0];

			this.GridViewSortExpression = e.SortExpression;
			int pageIndex = this.FMGridViewQualityTagLog.PageIndex;
			this.FMGridViewQualityTagLog.DataSource = this.SortDataTable(table, isPageIndexChanging: false);
			this.FMGridViewQualityTagLog.DataBind();
			this.FMGridViewQualityTagLog.PageIndex = pageIndex;
		}

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			this.InititializeComponents();

			// Want to ignore the disabling of inputs on post backs.
			this.IgnoreInputDisable = true;
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">
		/// The source of the event.
		/// </param>
		/// <param name="e">
		/// The <see cref="System.EventArgs"/> instance containing the event data.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				var site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

				this.DatePattern = site.ShortDatePattern;
				this.TimePattern = site.TimePattern;

				if (this.Security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD))
				{
					this.FMButtonAddBottom.Enabled = true;
					this.FMButtonAddTop.Enabled = true;
				}
				else
				{
					this.FMButtonAddBottom.Enabled = false;
					this.FMButtonAddTop.Enabled = false;
				}

				// This is the first time through.
				if (this.IsPostBack == false)
				{
					this.InitializeView();
					this.UpdateView();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected void RefreshButtonOnClick(object sender, EventArgs e)
		{
			this.FMGridViewQualityTagLog.PageIndex = 0;
			this.UpdateView();
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

		protected void UpdateView()
		{
			try
			{
				string dateType = this.DateFilterTypeDropDown.SelectedItem.Text;
				DateTimeOffset dateStart = this.StartDate.CurrentValue;
				DateTimeOffset dateEnd = this.EndDate.CurrentValue;

				DataSet ds =
					FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, DataSet>(
						logs =>
						logs.GetDataSet(
							this.Security, 
							this.HistoricalDataCheckBox.Checked, 
							dateType, 
							dateStart, 
							dateEnd, 
							this.QualityTagDropDown.SelectedValue, 
							this.TaggedByDropDown.SelectedValue, 
							this.RemovedByDropDown.SelectedValue, 
							this.AssetIDDropDownList.SelectedValue, 
							this.TagStatusFilterDropDown.SelectedValue));

				DataTable table = ds.Tables[0];
				this.FMGridViewQualityTagLog.DataSource = new DataView(table);
				this.FMGridViewQualityTagLog.DataBind();
			}
			catch (Exception ex)
			{
				var table = new DataTable();
				this.FMGridViewQualityTagLog.DataSource = new DataView(table);
				this.FMGridViewQualityTagLog.DataBind();
				this.ErrorHandler(ex);
			}
		}

		protected void InitializeView()
		{
			string translatedShowAllTags = this.GetTranslatedText("{Show All Tags}");
			string translatedAll = this.GetTranslatedText("{All}");
			string translatedAny = this.GetTranslatedText("{Any}");

			// Populate and initialize the DateFilterTypeDropDown drop down.
			this.DateFilterTypeDropDown.Items.Clear();
			this.DateFilterTypeDropDown.Items.Add(new ListItem(this.GetTranslatedText(string.Empty)));
			this.DateFilterTypeDropDown.Items.Add(new ListItem(this.GetTranslatedText("Tagged Date")));
			this.DateFilterTypeDropDown.Items.Add(new ListItem(this.GetTranslatedText("Removed Date")));
			this.DateFilterTypeDropDown.SelectedIndex = 0;

			QualityTagCollectionClass qualityTagColl =
				FMChannelHelper.MakeCall<IQualityTags, QualityTagCollectionClass>(
					tags => tags.Enumerate(this.Security, null, null, true));

			this.QualityTagDropDown.DataTextField = "ID";
			this.QualityTagDropDown.DataValueField = "ID";
			this.QualityTagDropDown.DataSource = qualityTagColl;
			this.QualityTagDropDown.DataBind();
			this.QualityTagDropDown.Items.Insert(0, new ListItem(translatedShowAllTags, string.Empty));
			this.QualityTagDropDown.SelectByText(translatedShowAllTags);

			this.TagStatusFilterDropDown.Items.Clear();
			this.TagStatusFilterDropDown.Items.Add(new ListItem(translatedAll, string.Empty));
			this.TagStatusFilterDropDown.Items.Add(new ListItem("Active Tags Only"));
			this.TagStatusFilterDropDown.Items.Add(new ListItem("Removed Tags Only"));
			this.TagStatusFilterDropDown.SelectByText(translatedAll);

			UserCollectionClass usersColl =
				FMChannelHelper.MakeCall<IUsers, UserCollectionClass>(users => users.Enumerate(this.Security));

			this.TaggedByDropDown.Items.Clear();
			this.TaggedByDropDown.DataTextField = "ID";
			this.TaggedByDropDown.DataValueField = "ID";
			this.TaggedByDropDown.DataSource = usersColl;
			this.TaggedByDropDown.DataBind();
			this.TaggedByDropDown.Items.Insert(0, new ListItem(this.GetTranslatedText("{Any}"), string.Empty));
			this.TaggedByDropDown.SelectByText(this.GetTranslatedText("{Any}"));

			this.RemovedByDropDown.Items.Clear();
			this.RemovedByDropDown.DataTextField = "ID";
			this.RemovedByDropDown.DataValueField = "ID";
			this.RemovedByDropDown.DataSource = usersColl;
			this.RemovedByDropDown.DataBind();
			this.RemovedByDropDown.Items.Insert(0, new ListItem(translatedAny, string.Empty));
			this.RemovedByDropDown.SelectByText(this.GetTranslatedText(translatedAny));

			EquipmentQualityTagLogCollectionClass equipmentQualityTagLogColl =
				FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, EquipmentQualityTagLogCollectionClass>(
					logs => logs.Enumerate(this.Security, false));

			this.AssetIDDropDownList.Items.Clear();
			this.AssetIDDropDownList.DataTextField = "EquipmentID";
			this.AssetIDDropDownList.DataValueField = "EquipmentID";
			this.AssetIDDropDownList.DataSource = equipmentQualityTagLogColl;
			this.AssetIDDropDownList.DataBind();
			this.AssetIDDropDownList.Items.Insert(0, new ListItem(translatedAny, string.Empty));
			this.AssetIDDropDownList.SelectByText(this.GetTranslatedText(translatedAny));

			// Populate and initialize the date controls.
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

			DateTimeOffset siteTimeNow = TimeConverter.Now(site);

			this.StartDate.CurrentValue = siteTimeNow;
			this.StartDate.Enabled = false;

			this.EndDate.CurrentValue = siteTimeNow;
			this.EndDate.Enabled = false;

			// Initialize the session variable.
			this.Session.Remove(QualityTagLogSelectionSessionKey);
		}

		protected void GridViewPageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			string dateType = this.DateFilterTypeDropDown.SelectedItem.Text;
			DateTimeOffset dateStart = this.StartDate.CurrentValue;
			DateTimeOffset dateEnd = this.EndDate.CurrentValue;

			DataSet ds =
				FMChannelHelper.MakeCall<IEquipmentQualityTagLogs, DataSet>(
					logs =>
					logs.GetDataSet(
						this.Security, 
						this.HistoricalDataCheckBox.Checked, 
						dateType, 
						dateStart, 
						dateEnd, 
						this.QualityTagDropDown.SelectedValue, 
						this.TaggedByDropDown.SelectedValue, 
						this.RemovedByDropDown.SelectedValue, 
						this.AssetIDDropDownList.SelectedValue, 
						this.TagStatusFilterDropDown.SelectedValue));

			DataTable table = ds.Tables[0];

			this.FMGridViewQualityTagLog.DataSource = this.SortDataTable(table, isPageIndexChanging: true);
			this.FMGridViewQualityTagLog.PageIndex = e.NewPageIndex;
			this.FMGridViewQualityTagLog.DataBind();
		}

		private string GetSortDirection()
		{
			switch (this.GridViewSortDirection)
			{
				case "ASC":
					this.GridViewSortDirection = "DESC";
					break;
				case "DESC":
					this.GridViewSortDirection = "ASC";
					break;
			}

			return this.GridViewSortDirection;
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		private void InititializeComponents()
		{
			this.FMGridViewQualityTagLog.PreRender += this.FMGridViewQualityTagLogPreRender;
			this.FMGridViewQualityTagLog.RowDataBound += this.FMGridViewQualityTagLogRowDataBound;
			this.FMGridViewQualityTagLog.RowCommand += this.FMGridViewQualityTagLogRowCommandReceived;
			this.FMGridViewQualityTagLog.Sorting += this.FMGridViewQualityTagLogSorting;
			this.FMGridViewQualityTagLog.PageIndexChanging += this.GridViewPageIndexChanging;
		}

		#endregion
	}
}
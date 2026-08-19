// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyAccessSchedulePage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyAccessSchedulePage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Diagnostics.CodeAnalysis;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	/// <summary>
	/// Code behind for CompanyAccessSchedulePage.
	/// </summary>
	public partial class CompanyAccessSchedulePage : CompanyPageBase
	{
		#region Constants and Fields

		/// <summary>
		/// Gets or sets the acess schedule data grid
		/// </summary>
		protected FMDataGrid AcessScheduleDataGrid { get; set; }

		/// <summary>
		/// Gets or sets the current site.
		/// </summary>
		protected SiteClass CurrentSite { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false)
																);

				if (!this.Page.IsPostBack)
				{
					this.UpdateAccessScheduleView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}


        protected void AccessScheduleDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            //Set the availability of the Grid editing buttons for child record versions
            bool currentSiteOwnsRecordVersion = (Company.SiteGuid == Security.SiteGuid);
            if ((Company.IdentityGuid.Equals(Guid.Empty)) 
                || (currentSiteOwnsRecordVersion && Company.IdentityGuid.Equals(Company.MasterRecordGuid)))
                return;
            LinkButton EditButton = (LinkButton)e.Item.FindControl("EditButton");
            if (EditButton != null)
            {
                if ((VersionSpecificFields == null) || !VersionSpecificFields.Contains("AccessSchedule"))
                    EditButton.Enabled = false;
            }
        }


		/// <summary>
		/// Handles the CancelCommand event of the AccessScheduleDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridCommandEventArgs" /> instance containing the event data.</param>
		private void AccessScheduleDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			var accessScheduleDataGrid = (DataGrid)source;
			accessScheduleDataGrid.EditItemIndex = -1;
			this.EnableControls(true);
			this.UpdateAccessScheduleView();
		}

		/// <summary>
		/// Handles the EditCommand event of the AccessScheduleDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridCommandEventArgs" /> instance containing the event data.</param>
		private void AccessScheduleDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			var accessScheduleDataGrid = (DataGrid)source;
			accessScheduleDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.EnableControls(false);
			this.UpdateAccessScheduleView();
		}

		/// <summary>
		/// Handles the UpdateCommand event of the AccessScheduleDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridCommandEventArgs" /> instance containing the event data.</param>
		private void AccessScheduleDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var accessScheduleDataGrid = (DataGrid)source;

				var indexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (indexLabel != null)
				{
					ScheduleClass schedule = this.Company.AccessScheduleCollection[Convert.ToInt32(indexLabel.Text)];

					var enableCheckBox = (CheckBox)e.Item.FindControl("EnabledCheckBox");
					schedule.Enabled = enableCheckBox.Checked;

					string dayOne = TimeConverter.MinFMDate.ToString("d", this.CurrentSite.GetDateTimeFormatInfo());

					var openingTime = (FMTime)e.Item.FindControl("OpeningTime");
					schedule.OpeningTime.Value = DateTimeOffset.Parse(
						dayOne + " " + openingTime.Text, this.CurrentSite.GetDateTimeFormatInfo());

					var closingTime = (FMTime)e.Item.FindControl("ClosingTime");
					schedule.ClosingTime.Value = DateTimeOffset.Parse(
						dayOne + " " + closingTime.Text, this.CurrentSite.GetDateTimeFormatInfo());

					accessScheduleDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdateAccessScheduleView();
				}
			}
			catch (Exception except)
			{
				this.EnableControls(true);
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will enable and disable controls.
		/// </summary>
		/// <param name="enable">if set to <c>true</c> [enable].</param>
		private void EnableControls(bool enable)
		{
			// Call the main form to disable buttons and tabs.
			var companyForm = (CompanyForm)this.Page;
			companyForm.EnableControls(enable);
		}

		/// <summary>
		/// Enumerates the access schedule.
		/// </summary>
		/// <returns>A collection of schedule objects.</returns>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
		private ICollection EnumerateAccessSchedule()
		{
			var scheduleDataTable = new DataTable();

			scheduleDataTable.Columns.Add("Index", typeof(Int32));
			scheduleDataTable.Columns.Add("Day", typeof(string));
			scheduleDataTable.Columns.Add("Enabled", typeof(bool));
			scheduleDataTable.Columns.Add("OpeningTime", typeof(string));
			scheduleDataTable.Columns.Add("ClosingTime", typeof(string));

			int item = 0;

			foreach (ScheduleClass schedule in this.Company.AccessScheduleCollection)
			{
				DataRow scheduleDataRow = scheduleDataTable.NewRow();

				schedule.OpeningTime.Format = this.CurrentSite.GetDateTimeFormatInfo();
				schedule.ClosingTime.Format = this.CurrentSite.GetDateTimeFormatInfo();

				scheduleDataRow["Index"] = item;

				if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
				{
					scheduleDataRow["Day"] = GetDataDictionaryValueByKey(this.Security.LoginSiteGuid, schedule.DayText);
				}
				else
				{
					scheduleDataRow["Day"] = schedule.DayText;
				}

				scheduleDataRow["Enabled"] = schedule.Enabled;
				scheduleDataRow["OpeningTime"] = schedule.OpeningTime.ToString();
				scheduleDataRow["ClosingTime"] = schedule.ClosingTime.ToString();

				scheduleDataTable.Rows.Add(scheduleDataRow);
				item++;
			}

			var scheduleDataView = new DataView(scheduleDataTable);
			return scheduleDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AccessScheduleDataGrid.EditCommand += this.AccessScheduleDataGridEditCommand;
			this.AccessScheduleDataGrid.CancelCommand += this.AccessScheduleDataGridCancelCommand;
			this.AccessScheduleDataGrid.UpdateCommand += this.AccessScheduleDataGridUpdateCommand;
		}

		/// <summary>
		/// Updates the access schedule view.
		/// </summary>
		private void UpdateAccessScheduleView()
		{
			this.AccessScheduleDataGrid.DataSource = this.EnumerateAccessSchedule();
			this.AccessScheduleDataGrid.DataBind();
		}

		#endregion


	}
}
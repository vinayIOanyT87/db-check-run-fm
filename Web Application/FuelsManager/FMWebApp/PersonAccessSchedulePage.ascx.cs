// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonAccessSchedulePage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PersonAccessSchedulePage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	/// <summary>
	///    Summary description for PersonAccessSchedulePage.
	/// </summary>
	public partial class PersonAccessSchedulePage : PersonPageBase
	{
		#region Constants and Fields
		protected FMDataGrid AcessScheduleDataGrid;
		protected SiteClass CurrentSite;
		#endregion

		#region Methods
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

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

		private void AccessScheduleDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			var accessScheduleDataGrid = (DataGrid)source;
			accessScheduleDataGrid.EditItemIndex = -1;
			this.EnableControls(true);
			this.UpdateAccessScheduleView();
		}

		private void AccessScheduleDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			var accessScheduleDataGrid = (DataGrid)source;
			accessScheduleDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.EnableControls(false);
			this.UpdateAccessScheduleView();
		}

		private void AccessScheduleDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var accessScheduleDataGrid = (DataGrid)source;
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					ScheduleClass schedule = this.Person.AccessScheduleCollection[Convert.ToInt32(indexLabel.Text)];

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
		///    This method will enable and disable controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			// Call the main form to disable buttons and tabs.
			var companyForm = (PersonForm)this.Page;
			companyForm.EnableControls(enable);
		}

		private ICollection EnumerateAccessSchedule()
		{
			var scheduleDataTable = new DataTable();

			scheduleDataTable.Columns.Add("Index", typeof(Int32));
			scheduleDataTable.Columns.Add("Day", typeof(string));
			scheduleDataTable.Columns.Add("Enabled", typeof(bool));
			scheduleDataTable.Columns.Add("OpeningTime", typeof(string));
			scheduleDataTable.Columns.Add("ClosingTime", typeof(string));

			int item = 0;

			foreach (ScheduleClass schedule in this.Person.AccessScheduleCollection)
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

			return new DataView(scheduleDataTable);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AccessScheduleDataGrid.EditCommand		+= this.AccessScheduleDataGridEditCommand;
			this.AccessScheduleDataGrid.CancelCommand	+= this.AccessScheduleDataGridCancelCommand;
			this.AccessScheduleDataGrid.UpdateCommand	+= this.AccessScheduleDataGridUpdateCommand;
		}

		private void UpdateAccessScheduleView()
		{
			this.AccessScheduleDataGrid.DataSource = this.EnumerateAccessSchedule();
			this.AccessScheduleDataGrid.DataBind();
		}
		#endregion

		protected void AccessScheduleDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var editButton = (LinkButton)e.Item.FindControl("Fmeditlinkbutton1");
			
			if (editButton != null)
			{
				bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);

				if (this.Person.IdentityGuid.Equals(Guid.Empty)
					  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
				{
					return;
				}

				editButton.Enabled = (editButton.Enabled && (this.VersionSpecificFields != null)
                                                  && this.VersionSpecificFields.Contains("Schedule"));
			}
		}
	}
}
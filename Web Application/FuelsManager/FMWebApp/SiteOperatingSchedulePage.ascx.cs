// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteOperatingSchedulePage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SiteOperatingSchedulePage type.
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
	///    Summary description for SiteOperatingSchedulePage.
	/// </summary>
	public partial class SiteOperatingSchedulePage : FMUserControlBase
	{
        #region Methods

        private void UpdateHolidayScheduleView()
        {
            this.HolidayScheduleDataGrid.DataSource = this.EnumerateHolidaySchedule();
            this.HolidayScheduleDataGrid.DataBind();
        }

        private ICollection EnumerateHolidaySchedule()
        {
            SiteClass site = (SiteClass)this.Session["Site"];

            DataTable scheduleDataTable = new DataTable();

            scheduleDataTable.Columns.Add("ScheduleHolidayGuid", typeof(Int32));
            scheduleDataTable.Columns.Add("DayText", typeof(string));
            scheduleDataTable.Columns.Add("Enabled", typeof(bool));
            scheduleDataTable.Columns.Add("OpeningTime", typeof(string));
            scheduleDataTable.Columns.Add("ClosingTime", typeof(string));
            scheduleDataTable.Columns.Add("EndOfDayEnabled", typeof(bool));
            scheduleDataTable.Columns.Add("EndOfDayTime", typeof(string));

            int item = 0;
            foreach (ScheduleClass schedule in site.HolidayScheduleCollection)
            {
                var scheduleDataRow = scheduleDataTable.NewRow();

                scheduleDataRow["ScheduleHolidayGuid"] = item;
                scheduleDataRow["DayText"] = schedule.DayText;
                scheduleDataRow["Enabled"] = schedule.Enabled;
                scheduleDataRow["OpeningTime"] = schedule.OpeningTime.ToString();
                scheduleDataRow["ClosingTime"] = schedule.ClosingTime.ToString();
                scheduleDataRow["EndOfDayEnabled"] = schedule.EndOfDayEnabled;
                scheduleDataRow["EndOfDayTime"] = schedule.EndOfDayTime.ToString();

                scheduleDataTable.Rows.Add(scheduleDataRow);
                item++;
            }

            DataView scheduleDataView = new DataView(scheduleDataTable);
            return scheduleDataView;
        }

        /// <summary>
        ///    This method enables and disables controls.
        /// </summary>
        /// <param name="enable"></param>
        protected void EnableControls(bool enable)
		{
            this.AddButton.Enabled = enable;

            // Call the main form to disable buttons and tabs.
            var siteForm = (SiteForm)this.Page;
			siteForm.EnableControls(enable);
		}

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
				if (!this.Page.IsPostBack)
				{
					this.UpdateOperatingScheduleView();
				    this.UpdateHolidayScheduleView();
                }
            }
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private ICollection EnumerateOperatingSchedule()
		{
			var site = (SiteClass)this.Session["Site"];

			var scheduleDataTable = new DataTable();

		    scheduleDataTable.Columns.Add("Index", typeof(Int32));
			scheduleDataTable.Columns.Add("Day", typeof(string));
			scheduleDataTable.Columns.Add("Enabled", typeof(bool));
			scheduleDataTable.Columns.Add("OpeningTime", typeof(string));
			scheduleDataTable.Columns.Add("ClosingTime", typeof(string));
			scheduleDataTable.Columns.Add("EndOfDayEnabled", typeof(bool));
			scheduleDataTable.Columns.Add("EndOfDayTime", typeof(string));

			int item = 0;
			foreach (ScheduleClass schedule in site.OperatingScheduleCollection)
			{
				var scheduleDataRow = scheduleDataTable.NewRow();

				scheduleDataRow["Index"] = item;
				if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
				{
					scheduleDataRow["Day"] = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.LoginSiteGuid, schedule.DayText)
																);
				}
				else
				{
					scheduleDataRow["Day"] = schedule.DayText;
				}
				scheduleDataRow["Enabled"] = schedule.Enabled;
				scheduleDataRow["OpeningTime"] = schedule.OpeningTime.ToString();
				scheduleDataRow["ClosingTime"] = schedule.ClosingTime.ToString();
				scheduleDataRow["EndOfDayEnabled"] = schedule.EndOfDayEnabled;
				scheduleDataRow["EndOfDayTime"] = schedule.EndOfDayTime.ToString();

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
			this.OperatingScheduleDataGrid.CancelCommand += this.OperatingScheduleDataGrid_CancelCommand;
			this.OperatingScheduleDataGrid.EditCommand += this.OperatingScheduleDataGrid_EditCommand;
			this.OperatingScheduleDataGrid.UpdateCommand += this.OperatingScheduleDataGrid_UpdateCommand;
            this.HolidayScheduleDataGrid.EditCommand += this.HolidayScheduleDataGrid_EditCommand;
            this.HolidayScheduleDataGrid.PageIndexChanged += this.HolidayScheduleDataGrid_PageIndexChanged;
            this.HolidayScheduleDataGrid.CancelCommand += this.HolidayScheduleDataGrid_CancelCommand;
            this.HolidayScheduleDataGrid.UpdateCommand += this.HolidayScheduleDataGrid_UpdateCommand;
            this.HolidayScheduleDataGrid.DeleteCommand += this.HolidayScheduleDataGrid_DeleteCommand;
            this.HolidayScheduleDataGrid.ItemDataBound += this.HolidayScheduleDataGrid_ItemDataBound;
            this.AddButton.Command += this.AddButton_Command;
        }

        // ReSharper disable once InconsistentNaming
        private void OperatingScheduleDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			var operatingScheduleDataGrid = (DataGrid)source;
			operatingScheduleDataGrid.EditItemIndex = -1;

			// Enable controls when completing line item editing;
			this.EnableControls(true);
			this.UpdateOperatingScheduleView();
		}

	    // ReSharper disable once InconsistentNaming
		private void OperatingScheduleDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			var operatingScheduleDataGrid = (DataGrid)source;
			operatingScheduleDataGrid.EditItemIndex = e.Item.ItemIndex;

			// Disable controls while in line item edit mode;
			this.EnableControls(false);
			this.UpdateOperatingScheduleView();
		}

	    // ReSharper disable once InconsistentNaming
		private void OperatingScheduleDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var operatingScheduleDataGrid = (DataGrid)source;

				var indexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					var site = (SiteClass)this.Session["Site"];
					ScheduleClass schedule = site.OperatingScheduleCollection[Convert.ToInt32(indexLabel.Text)];

					var enableCheckBox = (CheckBox)e.Item.FindControl("EnabledCheckBox");
					schedule.Enabled = enableCheckBox.Checked;

					string dayOne = TimeConverter.MinFMDate.ToString("d", site.GetDateTimeFormatInfo());

					var openingTime = (FMTime)e.Item.FindControl("OpeningTime");
					schedule.OpeningTime.Value = DateTimeOffset.Parse(dayOne + " " + openingTime.Text, site.GetDateTimeFormatInfo());

					var closingTime = (FMTime)e.Item.FindControl("ClosingTime");
					schedule.ClosingTime.Value = DateTimeOffset.Parse(dayOne + " " + closingTime.Text, site.GetDateTimeFormatInfo());

					var endOfDayEnabledCheckBox = (CheckBox)e.Item.FindControl("EndOfDayEnabledCheckBox");
					schedule.EndOfDayEnabled = endOfDayEnabledCheckBox.Checked;

					var endOfDayTime = (FMTime)e.Item.FindControl("EndOfDayTime");
					schedule.EndOfDayTime.Value = DateTimeOffset.Parse(dayOne + " " + endOfDayTime.Text, site.GetDateTimeFormatInfo());

					operatingScheduleDataGrid.EditItemIndex = -1;

					// Enable controls when completing line item editing;
					this.EnableControls(true);
					this.UpdateOperatingScheduleView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

	    // ReSharper disable once InconsistentNaming
        private void HolidayScheduleDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
        {
            DataGrid holidayScheduleDataGrid = (DataGrid)source;
            holidayScheduleDataGrid.EditItemIndex = e.Item.ItemIndex;

            this.Session.Remove("SiteHolidayScheduleModified");
            this.Session.Add("SiteHolidayScheduleModified", true);

            this.EnableControls(false);
            this.UpdateHolidayScheduleView();
        }

	    // ReSharper disable once InconsistentNaming
        private void HolidayScheduleDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            DataGrid holidayScheduleDataGrid = (DataGrid)source;

            Label indexLabel = (Label)e.Item.FindControl("IndexLabel");
            if (indexLabel != null)
            {
                if (holidayScheduleDataGrid.EditItemIndex == e.Item.ItemIndex)
                {
                    this.EnableControls(true);
                    holidayScheduleDataGrid.EditItemIndex = -1;
                }

                else if (holidayScheduleDataGrid.EditItemIndex > e.Item.ItemIndex)
                    holidayScheduleDataGrid.EditItemIndex--;

                SiteClass site = (SiteClass)this.Session["Site"];
                site.HolidayScheduleCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));

                if (holidayScheduleDataGrid.Items.Count == 1
                && holidayScheduleDataGrid.CurrentPageIndex > 0)
                    holidayScheduleDataGrid.CurrentPageIndex--;

                this.Session.Remove("SiteHolidayScheduleModified");
                this.Session.Add("SiteHolidayScheduleModified", true);

                this.UpdateHolidayScheduleView();
            }
        }

	    // ReSharper disable once InconsistentNaming
        private void HolidayScheduleDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            DataGrid holidayScheduleDataGrid = (DataGrid)source;
            Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

            if (indexLabel != null)
            {
                SiteClass site = (SiteClass)this.Session["Site"];
                ScheduleClass schedule = site.HolidayScheduleCollection[Convert.ToInt32(indexLabel.Text)];

                // The schedule will have a Holiday Date if it is not one that has just been added by the add button.
                // If add is pressed and then the item is saved to the grid then the Holiday Date will have a value.
                // If the schedule does not have a holiday date and was therefore just added, then cancel should remove it from the list instead of just cancelling the edit
                if (!schedule.HolidayDate.HasValue)
                {
                    site.HolidayScheduleCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));
                    if ((holidayScheduleDataGrid.Items.Count == 1) && (holidayScheduleDataGrid.CurrentPageIndex > 0))
                    {
                        holidayScheduleDataGrid.CurrentPageIndex--;
                    }
                }

                holidayScheduleDataGrid.EditItemIndex = -1;
                this.EnableControls(true);
                this.UpdateHolidayScheduleView();
            }
        }

        /// <summary>
        /// This method handles the update event for a selected row.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        // ReSharper disable once InconsistentNaming
        private void HolidayScheduleDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            DataGrid holidayScheduleDataGrid = (DataGrid)source;
            Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

            if (indexLabel != null)
            {
                SiteClass site = (SiteClass)this.Session["Site"];
                ScheduleClass schedule = site.HolidayScheduleCollection[Convert.ToInt32(indexLabel.Text)];

                // Get the current site
                SiteClass currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                                     x =>
                                                                     x.Get(this.Security, this.Security.SiteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true,
                                                                     bGetAssociatedAliases: true)
                                                                );
                FMDate holidayDate = (FMDate)e.Item.FindControl("HolidayDate");
                schedule.OpeningTime.Format = currentSite.GetDateTimeFormatInfo();

                schedule.HolidayDate = holidayDate.CurrentValue;

                CheckBox enableCheckBox = (CheckBox)e.Item.FindControl("EnabledCheckBox");
                schedule.Enabled = enableCheckBox.Checked;

                string dayOne = TimeConverter.MinFMDate.ToString("d", currentSite.GetDateTimeFormatInfo());

                try
                {
                    FMTime openingTime = (FMTime)e.Item.FindControl("OpeningTime");
                    schedule.OpeningTime.Value = DateTimeOffset.Parse(dayOne + " " + openingTime.Text, currentSite.GetDateTimeFormatInfo());

                    FMTime closingTime = (FMTime)e.Item.FindControl("ClosingTime");
                    schedule.ClosingTime.Value = DateTimeOffset.Parse(dayOne + " " + closingTime.Text, currentSite.GetDateTimeFormatInfo());

                    CheckBox endOfDayEnabledCheckBox = (CheckBox)e.Item.FindControl("EndOfDayEnabledCheckBox");
                    schedule.EndOfDayEnabled = endOfDayEnabledCheckBox.Checked;

                    FMTime endOfDayTime = (FMTime)e.Item.FindControl("EndOfDayTime");
                    schedule.EndOfDayTime.Value = DateTimeOffset.Parse(dayOne + " " + endOfDayTime.Text, currentSite.GetDateTimeFormatInfo());

                    holidayScheduleDataGrid.EditItemIndex = -1;
                    this.EnableControls(true);
                    this.UpdateHolidayScheduleView();
                }
                catch (Exception)
                {
                    this.ErrorHandler(new Exception("Invalid Time"));
                }
            }
        }

	    // ReSharper disable once InconsistentNaming
        private void HolidayScheduleDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            Label indexLabel = (Label)e.Item.FindControl("IndexLabel");
            FMDate holidayDate = (FMDate)e.Item.FindControl("HolidayDate");
            if (indexLabel != null
            && holidayDate != null)
            {
                // Get the login site

                SiteClass currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
                        x =>
                        x.Get(this.Security, this.Security.SiteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true,
                        bGetAssociatedAliases: true)
                );


                SiteClass site = (SiteClass)this.Session["Site"];

                ScheduleClass schedule = site.HolidayScheduleCollection[Convert.ToInt32(indexLabel.Text)];

                if (schedule.HolidayDate != null)
                {
                    holidayDate.Text = schedule.DayText;
                }
                else
                {
                    holidayDate.Text = TimeConverter.Today(currentSite).ToString(currentSite.GetDateTimeFormatInfo().ShortDatePattern);
                }

            }
        }

	    // ReSharper disable once InconsistentNaming
        private void HolidayScheduleDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            // if we are editing do not allow a page change
            if (this.HolidayScheduleDataGrid.EditItemIndex > -1)
                return;
            this.HolidayScheduleDataGrid.CurrentPageIndex = e.NewPageIndex;
            this.UpdateHolidayScheduleView();
        }

        private void UpdateOperatingScheduleView()
		{
			this.OperatingScheduleDataGrid.DataSource = this.EnumerateOperatingSchedule();
			this.OperatingScheduleDataGrid.DataBind();
		}

	    // ReSharper disable once InconsistentNaming
        private void AddButton_Command(object sender, CommandEventArgs e)
        {
            SiteClass site = (SiteClass)this.Session["Site"];
            ScheduleClass schedule = new ScheduleClass(site.GetDateTimeFormatInfo())
                                         {
                                             Type = SCHEDULE_TYPE.HOLIDAY_TYPE
                                         };

            site.HolidayScheduleCollection.Add(schedule);
            this.HolidayScheduleDataGrid.CurrentPageIndex = (site.HolidayScheduleCollection.Count - 1) / this.HolidayScheduleDataGrid.PageSize;
            this.HolidayScheduleDataGrid.EditItemIndex = (site.HolidayScheduleCollection.Count - 1) % this.HolidayScheduleDataGrid.PageSize;

            this.Session.Remove("SiteHolidayScheduleModified");
            this.Session.Add("SiteHolidayScheduleModified", true);

            this.EnableControls(false);
            this.UpdateHolidayScheduleView();
        }
        #endregion
    }
}
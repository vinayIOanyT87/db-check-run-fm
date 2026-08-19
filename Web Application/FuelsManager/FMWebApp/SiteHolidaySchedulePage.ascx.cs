/******************************************************************************
	FILE NAME:		SiteHolidaySchedulePage.ascx.cs
	PURPOSE:		Implementation of SiteHolidaySchedulePage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:				Reason:
		----------	-----------------	-------------------------------------------
		07/11/2006	Richard Panachida	Fixed the invalid time stack trace error. CSI 3041.
		2007-01-22	Richard Panachida	Added an override method to disable/enable controls. 
										Currently, it disables/enables the Add button (CSI 4083).
		2007-02-09	Richard Panachida	Added an override method to disable/enable controls. 
										Currently, it disables/enables the Add button (CSI 4083).
*******************************************************************************/

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
	/// Summary description for SiteHolidaySchedulePage.
	/// </summary>
	public partial class SiteHolidaySchedulePage : FMUserControlBase
	{

		private void UpdateHolidayScheduleView()
		{
			this.HolidayScheduleDataGrid.DataSource = this.EnumerateHolidaySchedule();
			this.HolidayScheduleDataGrid.DataBind();
		}

		private ICollection EnumerateHolidaySchedule()
		{
			SiteClass Site = (SiteClass)this.Session["Site"];

			DataTable ScheduleDataTable = new DataTable();
			DataRow ScheduleDataRow;

			ScheduleDataTable.Columns.Add("ScheduleHolidayGuid", typeof(Int32));
			ScheduleDataTable.Columns.Add("DayText", typeof(string));
			ScheduleDataTable.Columns.Add("Enabled", typeof(bool));
			ScheduleDataTable.Columns.Add("OpeningTime", typeof(string));
			ScheduleDataTable.Columns.Add("ClosingTime", typeof(string));
			ScheduleDataTable.Columns.Add("EndOfDayEnabled", typeof(bool));
			ScheduleDataTable.Columns.Add("EndOfDayTime", typeof(string));

			int Item = 0; 
			foreach (ScheduleClass Schedule in Site.HolidayScheduleCollection)
			{
				ScheduleDataRow = ScheduleDataTable.NewRow();

				ScheduleDataRow["ScheduleHolidayGuid"] = Item;
				ScheduleDataRow["DayText"] = Schedule.DayText;
				ScheduleDataRow["Enabled"] = Schedule.Enabled;
				ScheduleDataRow["OpeningTime"] = Schedule.OpeningTime.ToString();
				ScheduleDataRow["ClosingTime"] = Schedule.ClosingTime.ToString();
				ScheduleDataRow["EndOfDayEnabled"] = Schedule.EndOfDayEnabled;
				ScheduleDataRow["EndOfDayTime"] = Schedule.EndOfDayTime.ToString();

				ScheduleDataTable.Rows.Add(ScheduleDataRow);
				Item++;
			}

			DataView ScheduleDataView = new DataView(ScheduleDataTable);
			return ScheduleDataView;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					this.UpdateHolidayScheduleView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method enables and disables controls.
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			SiteForm siteForm = (SiteForm)this.Page;
			siteForm.EnableControls(enable);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.HolidayScheduleDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.HolidayScheduleDataGrid_EditCommand);
			this.HolidayScheduleDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.HolidayScheduleDataGrid_PageIndexChanged);
			this.HolidayScheduleDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.HolidayScheduleDataGrid_CancelCommand);
			this.HolidayScheduleDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.HolidayScheduleDataGrid_UpdateCommand);
			this.HolidayScheduleDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.HolidayScheduleDataGrid_DeleteCommand);
			this.HolidayScheduleDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.HolidayScheduleDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		private void AddButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			SiteClass Site = (SiteClass)this.Session["Site"];
			ScheduleClass Schedule = new ScheduleClass(Site.GetDateTimeFormatInfo());

			Schedule.Type = SCHEDULE_TYPE.HOLIDAY_TYPE;
			Site.HolidayScheduleCollection.Add(Schedule);
			this.HolidayScheduleDataGrid.CurrentPageIndex = (Site.HolidayScheduleCollection.Count - 1) / this.HolidayScheduleDataGrid.PageSize;
			this.HolidayScheduleDataGrid.EditItemIndex = (Site.HolidayScheduleCollection.Count - 1) % this.HolidayScheduleDataGrid.PageSize;

			this.Session.Remove("SiteHolidayScheduleModified");
			this.Session.Add("SiteHolidayScheduleModified", true);

			this.EnableControls(false);
			this.UpdateHolidayScheduleView();
		}

		private void HolidayScheduleDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DataGrid HolidayScheduleDataGrid = (DataGrid)source;
			HolidayScheduleDataGrid.EditItemIndex = e.Item.ItemIndex;

			this.Session.Remove("SiteHolidayScheduleModified");
			this.Session.Add("SiteHolidayScheduleModified", true);

			this.EnableControls(false);
			this.UpdateHolidayScheduleView();
		}

		private void HolidayScheduleDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DataGrid HolidayScheduleDataGrid = (DataGrid)source;

			Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				if (HolidayScheduleDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.EnableControls(true);
					HolidayScheduleDataGrid.EditItemIndex = -1;
				}

				else if (HolidayScheduleDataGrid.EditItemIndex > e.Item.ItemIndex)
					HolidayScheduleDataGrid.EditItemIndex--;

				SiteClass Site = (SiteClass)this.Session["Site"];
				Site.HolidayScheduleCollection.RemoveAt(System.Convert.ToInt32(IndexLabel.Text));

				if (HolidayScheduleDataGrid.Items.Count == 1
				&& HolidayScheduleDataGrid.CurrentPageIndex > 0)
					HolidayScheduleDataGrid.CurrentPageIndex--;

				this.Session.Remove("SiteHolidayScheduleModified");
				this.Session.Add("SiteHolidayScheduleModified", true);

				this.UpdateHolidayScheduleView();
			}
		}

		private void HolidayScheduleDataGrid_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DataGrid HolidayScheduleDataGrid = (DataGrid)source;
			Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (indexLabel != null)
			{
				SiteClass site = (SiteClass)this.Session["Site"];
				ScheduleClass schedule = site.HolidayScheduleCollection[System.Convert.ToInt32(indexLabel.Text)];

				// The schedule will have a Holiday Date if it is not one that has just been added by the add button.
				// If add is pressed and then the item is saved to the grid then the Holiday Date will have a value.
				// If the schedule does not have a holiday date and was therefore just added, then cancel should remove it from the list instead of just cancelling the edit
				if (!schedule.HolidayDate.HasValue)
				{
					site.HolidayScheduleCollection.RemoveAt(Convert.ToInt32(indexLabel.Text));
					if ((HolidayScheduleDataGrid.Items.Count == 1) && (HolidayScheduleDataGrid.CurrentPageIndex > 0))
					{
						HolidayScheduleDataGrid.CurrentPageIndex--;
					}
				}

				HolidayScheduleDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateHolidayScheduleView();
			}
		}

		/// <summary>
		/// This method handles the update event for a selected row.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void HolidayScheduleDataGrid_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DataGrid HolidayScheduleDataGrid = (DataGrid)source;
			Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (IndexLabel != null)
			{
				SiteClass Site = (SiteClass)this.Session["Site"];
				ScheduleClass Schedule;
				Schedule = Site.HolidayScheduleCollection[System.Convert.ToInt32(IndexLabel.Text)];

				// Get the current site
				SiteClass CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true,
																	 bGetAssociatedAliases: true)
																);
				FMDate HolidayDate = (FMDate)e.Item.FindControl("HolidayDate");
				Schedule.OpeningTime.Format = CurrentSite.GetDateTimeFormatInfo();

				Schedule.HolidayDate = HolidayDate.CurrentValue;

				CheckBox EnableCheckBox = (CheckBox)e.Item.FindControl("EnabledCheckBox");
				Schedule.Enabled = EnableCheckBox.Checked;

				string DayOne = TimeConverter.MinFMDate.ToString("d", CurrentSite.GetDateTimeFormatInfo());

				try
				{
					FMTime OpeningTime = (FMTime)e.Item.FindControl("OpeningTime");
					Schedule.OpeningTime.Value = DateTimeOffset.Parse(DayOne + " " + OpeningTime.Text, CurrentSite.GetDateTimeFormatInfo());

					FMTime ClosingTime = (FMTime)e.Item.FindControl("ClosingTime");
					Schedule.ClosingTime.Value = DateTimeOffset.Parse(DayOne + " " + ClosingTime.Text, CurrentSite.GetDateTimeFormatInfo());

					CheckBox EndOfDayEnabledCheckBox = (CheckBox)e.Item.FindControl("EndOfDayEnabledCheckBox");
					Schedule.EndOfDayEnabled = EndOfDayEnabledCheckBox.Checked;

					FMTime EndOfDayTime = (FMTime)e.Item.FindControl("EndOfDayTime");
					Schedule.EndOfDayTime.Value = DateTimeOffset.Parse(DayOne + " " + EndOfDayTime.Text, CurrentSite.GetDateTimeFormatInfo());

					HolidayScheduleDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdateHolidayScheduleView();
				}
				catch (Exception)
				{
					base.ErrorHandler(new Exception("Invalid Time"));
				}
			}
		}

		private void HolidayScheduleDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			FMDate HolidayDate = (FMDate)e.Item.FindControl("HolidayDate");
			if (IndexLabel != null
			&& HolidayDate != null)
			{
				// Get the login site

				SiteClass CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
						x.Get(this.Security, this.Security.SiteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true,
						bGetAssociatedAliases: true)
				);


				SiteClass Site = (SiteClass)this.Session["Site"];

				ScheduleClass Schedule;
				Schedule = Site.HolidayScheduleCollection[System.Convert.ToInt32(IndexLabel.Text)];

				if (Schedule.HolidayDate != null && Schedule.HolidayDate.HasValue)
				{
					HolidayDate.Text = Schedule.DayText;
				}
				else
				{
					HolidayDate.Text = TimeConverter.Today(CurrentSite).ToString(CurrentSite.GetDateTimeFormatInfo().ShortDatePattern);
				}

			}
		}

		private void HolidayScheduleDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.HolidayScheduleDataGrid.EditItemIndex > -1)
				return;
			this.HolidayScheduleDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateHolidayScheduleView();
		}
	}
}

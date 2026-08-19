// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceSummaryPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceSummaryPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	///    This class handles the code behind functionality of the page.
	/// </summary>
	public partial class MobileDeviceSummaryPage : FMAutoSubmitFormBase, IMenuDiscovery
	{
		#region Public Methods and Operators

		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if(useNewLicenseKey == 1)
            {

            }
            else
            {

            }
			var items = new List<FMMenuItem>();

			//TODO: Temporary commented out so that QA does not test Mobile Device features.
			//if (security.HasRight(RIGHT.VIEW_MOBILE_DEVICES) == false && security.HasRight(RIGHT.MODIFY_MOBILE_DEVICES) == false)
			//{
			//	return null;
			//}

			//items.Add(
			//	new FMMenuItem
			//		{
			//			MenuItemType = FMMenuItemType.ASSETS_EQUIPMENT_MOBILE_DEVICES,
			//			RootMenuName = "Assets",
			//			CategoryName = "Equipment",
			//			ItemName = "Mobile Devices",
			//			NavigateUrl = "MobileDeviceSummaryPage.aspx",
			//			ApplyDataDictionary = ApplyDataDictionary.Apply,
			//			SortOrder = 1
			//		});

			return items;
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will handle the Add button event and redirect to the Mobile
		///    Device Configuration detail page.
		/// </summary>
		/// <param name="sender">Sender object for the event.</param>
		/// <param name="e">Event arguments.</param>
		protected void AddBtnOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationItemToEdit);
			this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationObject);

			this.Redirect("MobileDeviceConfigurationPage.aspx");
		}

		/// <summary>
		///    This method handles the find button click. It will add the find string
		///    and get the Mobile Devices based on the find string.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void FindButtonOnClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.FindTextBox.Text))
			{
				this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationFindString);
			}
			else
			{
				this.Session.Add(PageSessionKeyConstants.MobileDeviceConfigurationFindString, this.FindTextBox.Text);
			}

			MobileDeviceCollection mobileDeviceCollection = this.RetrieveMobileDevices();
			this.LoadMobileDeviceGrid(mobileDeviceCollection);
		}

		/// <summary>
		///    This method will handle the delete event.  It will remove only one Mobile device
		///    entry from the database based on the Mobile device GUID.
		/// </summary>
		/// <param name="source">Object source of the event.</param>
		/// <param name="e">Event arguments object.</param>
		protected void MobileDeviceDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			TableCell mobileDeviceGuidCell = e.Item.Cells[1];//bds

			Guid mobileDeviceGuid = Guid.Parse(mobileDeviceGuidCell.Text);
			FMChannelHelper.MakeCall<IMobileDevices>(
																	 x =>
																	 x.Purge(this.Security, mobileDeviceGuid)
																);
			// Update the grid with new data.
			MobileDeviceCollection mobileDeviceCollection = this.RetrieveMobileDevices();
			this.LoadMobileDeviceGrid(mobileDeviceCollection);

			this.DisableFields();
		}

		/// <summary>
		///    This method handles the edit event.  It will identify the row to be edited, save the
		///    the items GUID in session, and redirect to the Mobile Device configuration form.
		/// </summary>
		/// <param name="source">Source object</param>
		/// <param name="e">Event object</param>
		protected void MobileDeviceDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationItemToEdit);

			TableCell mobileDeviceGuidCell = e.Item.Cells[1];//bds
			this.Session.Add(PageSessionKeyConstants.MobileDeviceConfigurationItemToEdit, mobileDeviceGuidCell.Text);

			this.Redirect("MobileDeviceConfigurationPage.aspx");
		}

		/// <summary>
		///    This method handles the Mobile Device summary item data bound. It will disable the
		///    delete link button if the user does not have the "Modify Mobile Device"
		///    right.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void MobileDeviceSummaryItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICES) == false)
			{
				var deleteLinkButton = e.Item.FindControl("DeleteLinkButton") as FMDeleteLinkButton;

				if (deleteLinkButton != null)
				{
					deleteLinkButton.Enabled = false;
				}
			}
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    This method handles the page size dropdown change selection.
		/// </summary>
		/// <param name="source">
		///    The source.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			MobileDeviceCollection mobileDeviceCollection = this.RetrieveMobileDevices();
			this.LoadMobileDeviceGrid(mobileDeviceCollection);
		}

		/// <summary>
		///    This method handles the page load event.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.GetSecurity();

			MobileDeviceCollection mobileDeviceCollection = this.RetrieveMobileDevices();
			this.LoadMobileDeviceGrid(mobileDeviceCollection);

			this.DisableFields();
		}

		/// <summary>
		///    This method handles the show all button click. It will remove the find string
		///    and get all the Mobile Devices.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void ShowAllButtonOnClick(object sender, EventArgs e)
		{
			this.FindTextBox.Text = string.Empty;
			this.Session.Remove(PageSessionKeyConstants.MobileDeviceConfigurationFindString);

			MobileDeviceCollection mobileDeviceCollection = this.RetrieveMobileDevices();
			this.LoadMobileDeviceGrid(mobileDeviceCollection);
		}

		/// <summary>
		///    This method will disable all fields if the user does not have the
		///    "modify mobile device" right.
		/// </summary>
		private void DisableFields()
		{
			if (this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICES) == false)
			{
				this.AddBottomBtn.Enabled = false;
				this.AddTopBtn.Enabled = false;
			}
		}

		/// <summary>
		///    This method will return string indicating the Mobile Device type.
		///    The default is "None".
		/// </summary>
		/// <param name="mobileDevice">
		///    The mobile device.
		/// </param>
		/// <returns>
		///    The System.String.
		/// </returns>
		private string GetMobileDeviceType(MobileDeviceClass mobileDevice)
		{
			string deviceType = "None";

			if (mobileDevice.MobileDeviceType == null)
			{
				return deviceType;
			}

			switch ((MobileDeviceClass.MobileDeviceTypes)mobileDevice.MobileDeviceType.Value)
			{
				case MobileDeviceClass.MobileDeviceTypes.Handheld:
					deviceType = "Handheld";
					break;
			}

			return deviceType;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.MobileDeviceDataGrid.EditCommand += this.MobileDeviceDataGridEditCommand;
			this.MobileDeviceDataGrid.DeleteCommand += this.MobileDeviceDataGridDeleteCommand;
			this.MobileDeviceDataGrid.ItemDataBound += this.MobileDeviceSummaryItemDataBound;

			int pageLimit = 10;
			this.MobileDeviceSummaryPageSizeDropDown.SetLimit(pageLimit);
			this.MobileDeviceDataGrid.PageSize = pageLimit;
		}

		/// <summary>
		///    This method loads the Mobile Device grid with information from tblMobileDevice table.
		/// </summary>
		/// <param name="mobileDeviceCollection">
		///    The MobileDeviceCollection.
		/// </param>
		private void LoadMobileDeviceGrid(MobileDeviceCollection mobileDeviceCollection)
		{
			var gridTable = new DataTable("MobileDeviceTable");

			var column = new DataColumn("mobileDeviceGuid", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("SiteGuid", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("mobileDeviceId", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("Description", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			column = new DataColumn("mobileDeviceType", Type.GetType("System.String"));
			gridTable.Columns.Add(column);

			if ((mobileDeviceCollection != null) && (mobileDeviceCollection.Count > 0))
			{
				foreach (MobileDeviceClass mobileDevice in mobileDeviceCollection)
				{
					if (mobileDevice != null)
					{
						DataRow row = gridTable.NewRow();
						row["mobileDeviceGuid"] = mobileDevice.MobileDeviceGuid.ToString();
						row["SiteGuid"] = mobileDevice.SiteGuid.ToString();
						row["mobileDeviceId"] = mobileDevice.MobileDeviceId;
						row["Description"] = mobileDevice.Description;
						row["mobileDeviceType"] = this.GetMobileDeviceType(mobileDevice);

						gridTable.Rows.Add(row);
					}
				}
			}

			var view = new DataView(gridTable);
			this.MobileDeviceDataGrid.DataSource = view;
			this.MobileDeviceDataGrid.DataBind();
		}

		/// <summary>
		///    This method will retrieve the Mobile Device data from the database based on a find filter
		///    or an empty filter.
		/// </summary>
		/// <returns>
		///    The MobileDeviceCollection.
		/// </returns>
		private MobileDeviceCollection RetrieveMobileDevices()
		{
			// Check to see if there is a find string to filter the list of mobile devices.
			string findStr = string.Empty;
			if (this.Page.Session[PageSessionKeyConstants.MobileDeviceConfigurationFindString] != null)
			{
				findStr = this.Page.Session[PageSessionKeyConstants.MobileDeviceConfigurationFindString] as string;
			}

			if (string.IsNullOrEmpty(findStr))
			{
				MobileDeviceCollection mobileDeviceCollection = FMChannelHelper.MakeCall<IMobileDevices, MobileDeviceCollection>(
																	 x =>
																	 x.EnumerateAll(this.Security)
																);
				return mobileDeviceCollection;
			}
			else
			{
				MobileDeviceCollection mobileDeviceCollection = FMChannelHelper.MakeCall<IMobileDevices, MobileDeviceCollection>(
																	 x =>
																	 x.EnumerateByFindFilter(this.Security, findStr)
																);

				return mobileDeviceCollection;
			}
		}

		#endregion
	}
}
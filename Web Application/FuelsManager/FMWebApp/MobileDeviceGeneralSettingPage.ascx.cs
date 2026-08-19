// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceGeneralSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceGeneralSettingPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	///    This class handles the functionality for the Mobile Device General tab page.
	/// </summary>
	public partial class MobileDeviceGeneralSettingPage : FMUserControlBase
	{
		#region Constants and Fields

		/// <summary>
		///    The mobile device.
		/// </summary>
		private MobileDeviceClass mobileDevice;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will update the mobile device configuration from the general page.
		/// </summary>
		public void UpdateChanges()
		{
			this.mobileDevice = this.Session[PageSessionKeyConstants.MobileDeviceConfigurationObject] as MobileDeviceClass;

			if (this.mobileDevice != null)
			{
				if (string.IsNullOrEmpty(this.MobileDeviceIdTxtBox.Text))
				{
					throw new Exception("Must have a Mobile Device ID.");
				}

				this.mobileDevice.MobileDeviceId = this.MobileDeviceIdTxtBox.Text;
				this.mobileDevice.Description = this.DescriptionTxtBox.Text;

				bool isUnique = FMChannelHelper.MakeCall<IMobileDevices, bool>(
																	 x =>
																	 x.IsMobileDeviceUnique(this.Security, this.mobileDevice.MobileDeviceId)
																);
				if (isUnique == false)
				{
					throw new Exception("Mobile Device ID is not unique for site: " + this.Security.SiteID);
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method handles the assign button on click event. It will move
		///    selected items from the unassigned list box to the assigned list
		///    box.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void AssignButtonOnClick(object sender, EventArgs e)
		{
			this.mobileDevice = this.Session[PageSessionKeyConstants.MobileDeviceConfigurationObject] as MobileDeviceClass;

			if (this.mobileDevice == null)
			{
				return;
			}

			int[] selectedIndexes = this.UnassignedListBox.GetSelectedIndices();

			foreach (int selectedIndex in selectedIndexes)
			{
				ListItem selectedItem = this.UnassignedListBox.Items[selectedIndex];
				Guid selectedGuid = Guid.Parse(selectedItem.Value);
				MobileDeviceProfileToMobileDeviceMapClass foundItem =
					this.mobileDevice.UnassignedProfileCollection.Find(x => x.MobileDeviceProfileGuid == selectedGuid);

				this.mobileDevice.AssignedProfileCollection.Add(foundItem);
				this.mobileDevice.UnassignedProfileCollection.Remove(foundItem);

				// This is in case that the item was previous unassigned and now it is
				// being assigned again.
				this.mobileDevice.RemovedAssignedCollection.Remove(foundItem);
			}

			this.LoadAssignedListBox();
			this.LoadUnassignedListBox();
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
			this.mobileDevice = this.Session[PageSessionKeyConstants.MobileDeviceConfigurationObject] as MobileDeviceClass;

			if (this.mobileDevice == null)
			{
				throw new Exception("System error: The MobileDevice object is not found in session.");
			}

			// Disable the controls based on the user rights.
			this.DisableControlsForUser();

			if (this.Page.IsPostBack == false)
			{
				this.LoadData();
			}
		}

		/// <summary>
		///    This method handles the Unassign button on click event. It will move
		///    selected items from the assigned list box to the unassigned list
		///    box.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void UnassignButtonOnClick(object sender, EventArgs e)
		{
			this.mobileDevice = this.Session[PageSessionKeyConstants.MobileDeviceConfigurationObject] as MobileDeviceClass;

			if (this.mobileDevice == null)
			{
				return;
			}

			int[] selectedIndexes = this.AssignedListBox.GetSelectedIndices();

			foreach (int selectedIndex in selectedIndexes)
			{
				ListItem selectedItem = this.AssignedListBox.Items[selectedIndex];
				Guid selectedGuid = Guid.Parse(selectedItem.Value);
				MobileDeviceProfileToMobileDeviceMapClass foundItem =
					this.mobileDevice.AssignedProfileCollection.Find(x => x.MobileDeviceProfileGuid == selectedGuid);

				this.mobileDevice.UnassignedProfileCollection.Add(foundItem);
				this.mobileDevice.AssignedProfileCollection.Remove(foundItem);

				// Keep track of the unassigned items in order to remove them
				// from the mapping table during the update.
				this.mobileDevice.RemovedAssignedCollection.Add(foundItem);
			}

			this.LoadAssignedListBox();
			this.LoadUnassignedListBox();
		}

		/// <summary>
		///    This method enables or disables controls based on the user having the
		///    MODIFY_MOBILE_DEVICES right.
		/// </summary>
		private void DisableControlsForUser()
		{
			this.AssignedListBox.Enabled = this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICES);
			this.UnassignedListBox.Enabled = this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICES);
			this.AssignButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICES);
			this.UnassignButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICES);
		}

		/// <summary>
		///    This method will populate the Assigned list box with a list of Profiles that
		///    have been assigned to the Mobile Device.
		/// </summary>
		private void LoadAssignedListBox()
		{
			var assignedProfileList = new List<ListItem>();

			foreach (
				MobileDeviceProfileToMobileDeviceMapClass profileToMobileDeviceMap in this.mobileDevice.AssignedProfileCollection)
			{
				var assignedProfile = new ListItem
					{
						Text = profileToMobileDeviceMap.MobileDeviceProfileId,
						Value = profileToMobileDeviceMap.MobileDeviceProfileGuid.ToString()
					};
				assignedProfileList.Add(assignedProfile);
			}

			this.AssignedListBox.DataSource = assignedProfileList;
			this.AssignedListBox.DataTextField = "Text";
			this.AssignedListBox.DataValueField = "Value";
			this.AssignedListBox.Sort = true;
			this.AssignedListBox.DataBind();

			// Can only have one profile assigned to a mobile
			// device.
			if (this.mobileDevice.AssignedProfileCollection.Count > 0)
			{
				this.AssignButton.Enabled = false;
			}
			else
			{
				this.AssignButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICES);
			}
		}

		/// <summary>
		///    This method will load the data from the data object to the page.
		/// </summary>
		private void LoadData()
		{
			this.MobileDeviceIdTxtBox.Text = this.mobileDevice.MobileDeviceId;
			this.DescriptionTxtBox.Text = this.mobileDevice.Description;

			this.LoadAssignedListBox();
			this.LoadUnassignedListBox();
		}

		/// <summary>
		///    This method will populate the Unassigned list box with a list of Profiles that
		///    have not been assigned to the Mobile Device.
		/// </summary>
		private void LoadUnassignedListBox()
		{
			var unassignedProfileList = new List<ListItem>();

			// On a new Mobile Device object the unassigned profile collection will be empty since
			// the Mobile Device object did not perform a query. Therefore, we need to
			// do that here and set the object with the unassigned collection.
			if (this.mobileDevice.MobileDeviceGuid == Guid.Empty && this.mobileDevice.UnassignedProfileCollection.Count == 0)
			{
				var profileToMobileDeviceMap = new MobileDeviceProfileToMobileDeviceMapClass();
				MobileDeviceProfileToMobileDeviceMapCollection profileToMobileDeviceMapCollection =
					FMChannelHelper.MakeCall<IMobileDeviceProfileToMobileDeviceMaps, MobileDeviceProfileToMobileDeviceMapCollection>(
																	 x =>
																	 x.EnumerateUnassignedProfiles(
						this.Security, profileToMobileDeviceMap.AssignedToMobileDeviceGuid, inTransaction: false)
																);
				this.mobileDevice.UnassignedProfileCollection = profileToMobileDeviceMapCollection;
			}

			foreach (
				MobileDeviceProfileToMobileDeviceMapClass profileToMobileDeviceMap in this.mobileDevice.UnassignedProfileCollection)
			{
				var unassignedProfile = new ListItem
					{
						Text = profileToMobileDeviceMap.MobileDeviceProfileId,
						Value = profileToMobileDeviceMap.MobileDeviceProfileGuid.ToString()
					};

				unassignedProfileList.Add(unassignedProfile);
			}

			this.UnassignedListBox.DataSource = unassignedProfileList;
			this.UnassignedListBox.DataTextField = "Text";
			this.UnassignedListBox.DataValueField = "Value";
			this.UnassignedListBox.Sort = true;
			this.UnassignedListBox.DataBind();
		}

		#endregion
	}
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileMobileDeviceAssignmentSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfileMobileDeviceAssignmentSettingPage type.
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
	/// This class handles the functionality for the Profile Mobile Device Assignment tab page.
	/// </summary>
	public partial class ProfileMobileDeviceAssignmentSettingPage : FMUserControlBase
	{
		#region Private data members
		/// <summary>
		/// The mobile device profile.
		/// </summary>
		private MobileDeviceProfile mobileDeviceProfile;
		#endregion

		/// <summary>
		/// This method handles the page load event.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load ( object sender, EventArgs e )
		{
			this.mobileDeviceProfile = Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( Page.IsPostBack == false )
			{
				this.PopulateAssignedMobileDeviceListBox( );
				this.PopulateUnassignedMobileDeviceListBox();
			}

			this.DisableFields();
		}

		#region Public methods
		/// <summary>
		/// This method will reset all the fields when the new button is
		/// selected.
		/// </summary>
		public void ResetFieldsForNewEvent( )
		{
			this.PopulateUnassignedMobileDeviceListBox( );
			this.PopulateAssignedMobileDeviceListBox();
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method returns true if the user has the MODIFY_MOBILE_DEVICE_PROFILES right and the
		/// entity has not been assigned down.
		/// </summary>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		private bool HasPermission( )
		{
			this.mobileDeviceProfile = Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( this.mobileDeviceProfile == null )
			{
				return false;
			}

			if ( this.mobileDeviceProfile.SiteGuid == Guid.Empty && this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES) )
			{
				return true;
			}

			return this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES) && (this.Security.SiteGuid == this.mobileDeviceProfile.SiteGuid);
		}

		/// <summary>
		/// This method will disable all fields if the user does not have the
		/// "modify mobile device profile" right.
		/// </summary>
		private void DisableFields( )
		{
			this.AssignBtn.Enabled		= this.HasPermission();
			this.UnassignBtn.Enabled	= this.HasPermission();
		}

		/// <summary>
		/// This method will populate the assigned ADF device list box.
		/// </summary>
		private void PopulateAssignedMobileDeviceListBox()
		{
			if ( this.mobileDeviceProfile == null )
			{
				return;
			}

			var assignedMobileDeviceList = new List<ListItem>();

			foreach (MobileDeviceProfileToMobileDeviceMapClass mobileDeviceMap in this.mobileDeviceProfile.AssignedMobileDeviceCollection)
			{
				if ( mobileDeviceMap != null )
				{
					var item = new ListItem
						{
							Text  = mobileDeviceMap.MobileDeviceId,
							Value = mobileDeviceMap.AssignedToMobileDeviceGuid.ToString()
						};

					assignedMobileDeviceList.Add(item);
				}
			}

			this.AssignedListBox.DataSource		= assignedMobileDeviceList;
			this.AssignedListBox.DataTextField	= "Text";
			this.AssignedListBox.DataValueField = "Value";
			this.AssignedListBox.Sort			= true;
			this.AssignedListBox.DataBind();
		}

		/// <summary>
		/// This method will populate the Unassigned Mobile Device list box.
		/// </summary>
		private void PopulateUnassignedMobileDeviceListBox()
		{
			if ( this.mobileDeviceProfile == null )
			{
				return;
			}

			var unassignedList = new List<ListItem>( );

			if ( this.mobileDeviceProfile.UnassignMobileDeviceCollection.Count > 0 )
			{
				foreach ( MobileDeviceProfileToMobileDeviceMapClass unassignedMobileDevice in this.mobileDeviceProfile.UnassignMobileDeviceCollection )
				{
					if ( unassignedMobileDevice != null )
					{
						var item = new ListItem
							{
								Text  = unassignedMobileDevice.MobileDeviceId,
								Value = unassignedMobileDevice.AssignedToMobileDeviceGuid.ToString()
							};

						unassignedList.Add(item);
					}
				}
			}

			this.UnassignedListBox.DataSource		= unassignedList;
			this.UnassignedListBox.DataTextField	= "Text";
			this.UnassignedListBox.DataValueField	= "Value";
			this.UnassignedListBox.Sort = true;
			this.UnassignedListBox.DataBind();
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// This method handles the assign button click which will remove the selected
		/// items from the Unassigned List Box and add them to the Assigned list box.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void AssignedButtonOnClick(object sender, EventArgs e)
		{
			if ( this.mobileDeviceProfile == null )
			{
				return;
			}

			int[] selectedList = this.UnassignedListBox.GetSelectedIndices( );

			if ( selectedList.Length <= 0 )
			{
				return;
			}

			// The FOR loop is to handle multi-selected items.
			foreach ( int selectedIndex in selectedList )
			{
				ListItem selectedItem = this.UnassignedListBox.Items[selectedIndex];
				
				var selectedGuid = Guid.Parse(selectedItem.Value);
				var foundItem = this.mobileDeviceProfile.UnassignMobileDeviceCollection.Find(x => x.AssignedToMobileDeviceGuid == selectedGuid);

				this.mobileDeviceProfile.AssignedMobileDeviceCollection.Add(foundItem);
				this.mobileDeviceProfile.UnassignMobileDeviceCollection.Remove(foundItem);

				// This is in case that the item was previous unassigned and now it is
				// being assigned again.
				this.mobileDeviceProfile.RemoveMobileDeviceMapCollection.Remove(foundItem);
			}

			// Refresh the assigned and unassigned list boxes
			this.PopulateAssignedMobileDeviceListBox( );
			this.PopulateUnassignedMobileDeviceListBox( );
		}

		/// <summary>
		/// This method handles the Unassign button click which will remove the selected
		/// items from the Assigned List Box and add them to the Unassigned list box.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void UnassignedButtonOnClick(object sender, EventArgs e)
		{
			if ( this.mobileDeviceProfile == null )
			{
				return;
			}
	
			int[] selectedList = this.AssignedListBox.GetSelectedIndices();

			if ( selectedList.Length <= 0 )
			{
				return;
			}

			// The FOR loop is to handle multi-selected items.
			foreach ( int selectedIndex in selectedList )
			{
				ListItem selectedItem = this.AssignedListBox.Items[selectedIndex];
				var selectedGuid = Guid.Parse(selectedItem.Value);
				var foundItem = this.mobileDeviceProfile.AssignedMobileDeviceCollection.Find(x => x.AssignedToMobileDeviceGuid == selectedGuid);

				this.mobileDeviceProfile.UnassignMobileDeviceCollection.Add(foundItem);
				this.mobileDeviceProfile.AssignedMobileDeviceCollection.Remove(foundItem);

				// Keep track of the unassigned items in order to remove them
				// from the mapping table during the update.
				this.mobileDeviceProfile.RemoveMobileDeviceMapCollection.Add(foundItem);
			}

			// Refresh the assigned and unassigned list boxes
			this.PopulateAssignedMobileDeviceListBox();
			this.PopulateUnassignedMobileDeviceListBox();
		}
		#endregion
	}
}
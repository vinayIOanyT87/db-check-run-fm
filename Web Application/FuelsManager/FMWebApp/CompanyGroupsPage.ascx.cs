// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyGroupsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyGroupsPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// Code behind for CompanyGroupsPage.
	/// </summary>
	public partial class CompanyGroupsPage : CompanyPageBase
	{
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
				// Check the user rights and set the controls.
				this.SetUserRights();

				if (this.Page.IsPostBack == false)
				{
					// Populate AssignedGroupsListBox
					foreach (CompanyMapClass groupMap in this.Company.GroupMapCollection)
					{
						this.AddToGroupListBox(this.AssignedGroupsListBox, groupMap.AssignedToID, groupMap.AssignedToGuid);
					}

					// Populate UnassignedGroupsListBox
					GroupCollectionClass groupCollection = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

					if (this.Company.IdentityGuid != Guid.Empty)
					{
						foreach (GroupClass group in groupCollection)
						{
							if (null == this.AssignedGroupsListBox.Items.FindByValue(group.IdentityGuid.ToString()))
							{
								this.AddToGroupListBox(this.UnassignedGroupsListBox, group.ID, group.IdentityGuid);
							}
						}
					}
					else
					{
						var groupList =
							FMChannelHelper.MakeCall<ICompanyMaps, List<Guid>>(
								x => x.EnumerateGroupMapsWithAllCompaniesAssigned(this.Security));

						foreach (GroupClass group in groupCollection)
						{
							if (null == this.AssignedGroupsListBox.Items.FindByValue(group.IdentityGuid.ToString()))
							{
								// Check if assigned by virtue of being part of {All} companies
								if ( groupList.Contains( group.IdentityGuid ) )
								{
									// Add it to the assigned group list box
									this.AddToGroupListBox(this.AssignedGroupsListBox, group.ID, group.IdentityGuid);
								}
								else
								{
									this.AddToGroupListBox(this.UnassignedGroupsListBox, group.ID, group.IdentityGuid);
								}
							}
						}
					}

				    this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddToGroupListBox(ListBox groupsListBox, string assignedToID, Guid assignedToGuid)
		{
			var unassignedGroupItem = new ListItem(assignedToID, assignedToGuid.ToString());

			foreach (ListItem assignedGroupItem in groupsListBox.Items)
			{
				if (string.Compare(assignedGroupItem.Text, unassignedGroupItem.Text, StringComparison.Ordinal) > 0)
				{
					int index = groupsListBox.Items.IndexOf(assignedGroupItem);
					groupsListBox.Items.Insert(index, unassignedGroupItem);
					unassignedGroupItem = null;
					break;
				}
			}

			if (unassignedGroupItem != null)
			{
				groupsListBox.Items.Add(unassignedGroupItem);
			}
		}

		/// <summary>
		/// Handles the Command event of the AssignGroupsButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
		private void AssignGroupsButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedGroupItem;
			while ((unassignedGroupItem = this.UnassignedGroupsListBox.SelectedItem) != null)
			{
				this.UnassignedGroupsListBox.Items.Remove(unassignedGroupItem);
				unassignedGroupItem.Selected = false;

				foreach (ListItem assignedGroupItem in this.AssignedGroupsListBox.Items)
				{
					if (string.Compare(assignedGroupItem.Text, unassignedGroupItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.AssignedGroupsListBox.Items.IndexOf(assignedGroupItem);
						this.AssignedGroupsListBox.Items.Insert(index, unassignedGroupItem);
						unassignedGroupItem = null;
						break;
					}
				}

				if (unassignedGroupItem != null)
				{
					this.AssignedGroupsListBox.Items.Add(unassignedGroupItem);
				}
			}

			this.UpdateGroupMapCollection();
		}

		private void CheckAllCompanyAssignment(ListItem assignedGroupItem)
		{
			Guid groupGuid = Guid.Parse(assignedGroupItem.Value);

			GroupClass group = FMChannelHelper.MakeCall<IGroups, GroupClass>(
																	 x =>
																	 x.Get(this.Security, groupGuid)
																);

		    if (group?.CompanyMapCollection.Count > 0 && @group.CompanyMapCollection[0].AssignedGuid == Guid.Empty)
		    {
		        throw new ApplicationException("Cannot remove company from ALL company configuration of group " + @group.ID);
		    }
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AssignGroupsButton.Command += this.AssignGroupsButtonCommand;
			this.UnassignGroupsButton.Command += this.UnassignGroupsButtonCommand;
		}

		/// <summary>
		/// This method checks the user rights and sets the controls to be disabled.
		/// </summary>
		private void SetUserRights()
		{
			if (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false)
			{
				this.AssignGroupsButton.Enabled = false;
				this.UnassignGroupsButton.Enabled = false;
				this.AssignedGroupsListBox.Enabled = false;
				this.UnassignedGroupsListBox.Enabled = false;
			}
		}

		/// <summary>
		/// Handles the Command event of the UnassignGroupsButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
		private void UnassignGroupsButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				ListItem assignedGroupItem;
				while ((assignedGroupItem = this.AssignedGroupsListBox.SelectedItem) != null)
				{
					// Check to make sure the user is not trying to remove from a {All} company
					// assignment.
					this.CheckAllCompanyAssignment(assignedGroupItem);

					this.AssignedGroupsListBox.Items.Remove(assignedGroupItem);
					assignedGroupItem.Selected = false;

					foreach (ListItem unassignedGroupItem in this.UnassignedGroupsListBox.Items)
					{
						if (string.Compare(unassignedGroupItem.Text, assignedGroupItem.Text, StringComparison.Ordinal) > 0)
						{
							int index = this.UnassignedGroupsListBox.Items.IndexOf(unassignedGroupItem);
							this.UnassignedGroupsListBox.Items.Insert(index, assignedGroupItem);
							assignedGroupItem = null;
							break;
						}
					}

					if (assignedGroupItem != null)
					{
						this.UnassignedGroupsListBox.Items.Add(assignedGroupItem);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			this.UpdateGroupMapCollection();
		}

		/// <summary>
		/// Updates the group map collection.
		/// </summary>
		private void UpdateGroupMapCollection()
		{
			var groupMapCollection = new CompanyMapCollectionClass();
			foreach (ListItem assignedGroupItem in this.AssignedGroupsListBox.Items)
			{
                CompanyMapClass groupMap = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.USER_GROUP_COMPANY_MAP);
			    groupMap.AssignedToGuid = Guid.Parse(assignedGroupItem.Value);
			    groupMap.AssignedToID = assignedGroupItem.Text;

				groupMapCollection.Add(groupMap);
			}

			this.Company.GroupMapCollection = groupMapCollection;
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))))
            {
                return;
            }
            else if (this.VersionSpecificFields != null)
            {
                this.AssignGroupsButton.Enabled = (this.AssignGroupsButton.Enabled && currentSiteOwnsRecordVersion
                                               && this.VersionSpecificFields.Contains("UserGroups"));
                this.UnassignGroupsButton.Enabled = (this.UnassignGroupsButton.Enabled && currentSiteOwnsRecordVersion
                                            && this.VersionSpecificFields.Contains("UserGroups"));
            }
        }

		#endregion
	}
}
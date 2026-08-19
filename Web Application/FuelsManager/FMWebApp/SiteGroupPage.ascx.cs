/******************************************************************************

	FILE NAME:		SiteGroupPage.ascx.cs


	PURPOSE:			Implementation of SiteGroupPage


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for SiteGroupPage.
	/// </summary>
	public partial class SiteGroupPage : FMUserControlBase
	{
        /// <summary>
        ///    This method handles the updating the Site data object with the information on the
        ///    Site Group page.
        /// </summary>
        public void UpdateData()
        {
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var siteClass = (SiteClass)this.Session["Site"];

				if (!this.Page.IsPostBack)
				{
					if (siteClass.SiteGroup)
					{
						var addList = new List<ListItem>();

						// Populate AssignedSitesListBox
						foreach (SiteToSiteMapClass assignedSiteToSiteMap in siteClass.SiteToSiteMapCollection)
						{
							var unassignedSiteItem = new ListItem(assignedSiteToSiteMap.ChildSiteID, assignedSiteToSiteMap.ChildSiteGuid.ToString());
							addList.Add(unassignedSiteItem);
						}

						// Sort the list and then added it to the list box
						addList = addList.OrderBy( li => li.Text ).ToList();
						this.AssignedSitesListBox.Items.AddRange(addList.ToArray());
					}

					if (siteClass.SiteGroup
					|| siteClass.IdentityGuid == Guid.Empty)
					{
						// Populate UnassignedSitesListBox
						SiteCollectionClass siteCollection;

						if (siteClass.IdentityGuid != Guid.Empty)
						{
							siteCollection =
								FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
									x => x.EnumerateByCandidateChildrenSites(this.Security, siteClass.IdentityGuid));
						}
						else
						{
							siteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.Enumerate(this.Security));
						}

						var addList = new List<ListItem>();
						foreach (SiteClass unassignedSite in siteCollection)
						{
							// Skip the current Site
							if (siteClass.SiteGuid == unassignedSite.SiteGuid)
							{
								continue;
							}

							// Skip SiteAdmin
							if (unassignedSite.SiteGuid == Guids.SiteAdminGuid)
							{
								continue;
							}

							if (null == this.AssignedSitesListBox.Items.FindByValue(unassignedSite.SiteGuid.ToString()))
							{
								var assignedSiteItem = new ListItem(unassignedSite.ID, unassignedSite.SiteGuid.ToString());
								addList.Add(assignedSiteItem);
							}
						}

						// Sort the list and then added it to the list box
						addList = addList.OrderBy(li => li.Text).ToList();
						this.UnassignedSitesListBox.Items.AddRange(addList.ToArray());
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
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
			this.UnassignSitesButton.Command += new CommandEventHandler(this.UnassignSitesButtonCommand);
			this.AssignSitesButton.Command += new CommandEventHandler(this.AssignSitesButtonCommand);

		}
		#endregion

		private void AssignSitesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem unassignedSiteItem;
			while ((unassignedSiteItem = this.UnassignedSitesListBox.SelectedItem) != null)
			{
				this.UnassignedSitesListBox.Items.Remove(unassignedSiteItem);
				unassignedSiteItem.Selected = false;

				foreach (ListItem assignedSiteItem in this.AssignedSitesListBox.Items)
				{
					if (string.Compare(assignedSiteItem.Text, unassignedSiteItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.AssignedSitesListBox.Items.IndexOf(assignedSiteItem);
						this.AssignedSitesListBox.Items.Insert(index, unassignedSiteItem);
						unassignedSiteItem = null;
						break;
					}
				}

				if (unassignedSiteItem != null)
					this.AssignedSitesListBox.Items.Add(unassignedSiteItem);
			}

			this.UpdateAssignedSiteCollection();
		}

		private void UnassignSitesButtonCommand(object sender, CommandEventArgs e)
		{
			ListItem assignedSiteItem;
			while ((assignedSiteItem = this.AssignedSitesListBox.SelectedItem) != null)
			{
				this.AssignedSitesListBox.Items.Remove(assignedSiteItem);
				assignedSiteItem.Selected = false;

				foreach (ListItem unassignedSiteItem in this.UnassignedSitesListBox.Items)
				{
					if (string.Compare(unassignedSiteItem.Text, assignedSiteItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = this.UnassignedSitesListBox.Items.IndexOf(unassignedSiteItem);
						this.UnassignedSitesListBox.Items.Insert(index, assignedSiteItem);
						assignedSiteItem = null;
						break;
					}
				}

				if (assignedSiteItem != null)
					this.UnassignedSitesListBox.Items.Add(assignedSiteItem);
			}

			this.UpdateAssignedSiteCollection();
		}

		void UpdateAssignedSiteCollection()
		{
			SiteToSiteMapCollectionClass siteToSiteMapCollection = new SiteToSiteMapCollectionClass();
			SiteClass site = (SiteClass)this.Session["Site"];
			foreach (ListItem assignedSiteItem in this.AssignedSitesListBox.Items)
			{
			    SiteToSiteMapClass assignedSiteToSiteMap = new SiteToSiteMapClass
			                                               {
			                                                   ParentSiteGuid = site.IdentityGuid,
			                                                   ParentSiteID = site.ID,
			                                                   ChildSiteGuid =
			                                                       Guid.Parse(assignedSiteItem.Value),
			                                                   ChildSiteID = assignedSiteItem.Text
			                                               };
			    siteToSiteMapCollection.Add(assignedSiteToSiteMap);
			}

			site.SiteToSiteMapCollection = siteToSiteMapCollection;
		}
	}
}

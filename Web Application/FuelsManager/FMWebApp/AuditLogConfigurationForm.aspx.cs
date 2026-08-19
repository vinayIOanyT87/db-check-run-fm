/******************************************************************************
	FILE NAME:		FMDAuditLogConfigurationForm.aspx.cs
	PURPOSE:		Implementation of FMDAuditLogConfigurationForm

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		   By:					Reason:
		----------	-----------------	-------------------------------------------
		08/15/2005	W.Gray				7.0.0.30 - Changed to not show Tabs for Pipeline, Hydrant, RailCar,
										etc.
		2007-02-08	Richard Panachida	Added an override method to disable/enable controls. 
										Currently, it disables/enables the Add button (CSI 4083).
		2007-03-12	Richard Panachida	Corrected the problem that the tab was not highlighted after
										being disabled.
		2007-03-20	W.Gray				7.1.0.1 - Correciton to New Processing
 
		2009-03-01  A. Coker          Fixed defect 1732. Added ShipToLink to TransferString.
  
		2009-08-15  A. Coker          WI 5056 and 5658 - Added new tab pages to accommodate 
												new fields. Added ability to assign a singl product to 
												equipment.

		2009-09-09  A. Coker          WI 6435 - Rearranged fields.

*******************************************************************************/
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Interfaces;
using FMBusinessObjects.ServiceRequests;

using FMControls;

namespace FMWebApp
{
	using System.Collections.Generic;
	using System.Windows.Forms.Design;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// Summary description for AuditLogConfigurationForm.
	/// </summary>
	public partial class AuditLogConfigurationForm : FMFormBaseAjax, IMenuDiscovery
	{
		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }

            if (!security.HasRight(RIGHT.VIEW_DATABASE_AUDIT_LOG) && !security.HasRight(RIGHT.MODIFY_DATABASE_AUDIT_LOG))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_OTHER_DATABASE_AUDIT_LOG,
				RootMenuName = "Configuration",
				CategoryName = "Other",
				ItemName = "Database Audit Log",
				NavigateUrl = "AuditLogConfigurationForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.Apply
			};

			menuItems.Add(menuItem);

			return menuItems;
		}

		protected void Page_Init ( object sender, System.EventArgs e )
		{
			GetSecurity();

			if (IsPostBack == false)
			{
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{

				if (!Page.IsPostBack)
				{

					if (!Security.HasRight(RIGHT.MODIFY_DATABASE_AUDIT_LOG))
					{
						this.SaveButton.Enabled=false;
					}

					UpdateView();
				}


			}
			catch (Exception except)
			{
				ErrorHandler(except);
				Response.End();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion



		protected void OKButton_Click(object sender, EventArgs e)
		{
			//try
			//{
			//   FMDAuditLogsClass FMDAuditLogs = new FMDAuditLogsClass();
			//   FMDAuditLogClass FMDAuditLog = new FMDAuditLogClass();


			//   FMDAuditLog.ColumnID = 19;
			//   FMDAuditLog.LogicalOperator = 0;
			//   FMDAuditLog.ComparisonOperator = 1;
			//   FMDAuditLog.Value = 1;
			//   if (this.AuditReadAccessCheckBox.Checked)
			//   {
			//      FMDAuditLogs.Add(Security, FMDAuditLog);
			//   }
			//   else
			//   {
			//      FMDAuditLogs.Purge(Security, FMDAuditLog);
			//   }

			//   FMDAuditLog.ColumnID = 11;
			//   FMDAuditLog.LogicalOperator = 1;
			//   FMDAuditLog.ComparisonOperator = 6;
			//   if (this.AuditSelectedUsersCheckBox.Checked)
			//   {
			//      FMDAuditLog.Value = "FMDAdmin";
			//      FMDAuditLogs.Add(Security, FMDAuditLog);
			//      FMDAuditLog.Value = "sa";
			//      FMDAuditLogs.Add(Security, FMDAuditLog);
			//      foreach (ListItem item in this.AssignedUsersListBox.Items)
			//      {
			//         if (item.Value.StartsWith("available"))
			//         {
			//            FMDAuditLog.Value = item.Text;
			//            FMDAuditLogs.Add(Security, FMDAuditLog);

			//         }
			//      }
			//      foreach (ListItem item in this.AvailableUsersListBox.Items)
			//      {
			//         if (item.Value.StartsWith("assigned") &&
			//            item.Text != "FMAdmin" &&
			//            item.Text != "sa")
			//         {

			//            FMDAuditLog.Value = item.Text;
			//            FMDAuditLogs.Purge(Security, FMDAuditLog);

			//         }
			//      }
			//   }
			//   else
			//   {
			//      foreach (ListItem item in this.AssignedUsersListBox.Items)
			//      {
			//         if (item.Value.StartsWith("assigned") ||
			//            item.Text == "FMDAdmin" ||
			//            item.Text == "sa")
			//         {
			//            FMDAuditLog.Value = item.Text;
			//            FMDAuditLogs.Purge(Security, FMDAuditLog);

			//         }
			//      }
			//      foreach (ListItem item in this.AvailableUsersListBox.Items)
			//      {
			//         if (item.Value.StartsWith("assigned"))
			//         {
			//            FMDAuditLog.Value = item.Text;
			//            FMDAuditLogs.Purge(Security, FMDAuditLog);

			//         }
			//      }
			//   }
				

			//   UpdateView();
			//}
			//catch (Exception except)
			//{
			//   ErrorHandler(except);
			//   return;
			//}


		}

		protected void UnAssignUsersButton_Command(object sender, CommandEventArgs e)
		{
			ListItem assignedItem;
			while((assignedItem = this.AssignedUsersListBox.SelectedItem) != null)
			{

				assignedItem.Selected = false;
				if (assignedItem.Text != "FMDAdmin" &&
					assignedItem.Text != "sa")
				{
					this.AvailableUsersListBox.Items.Add(assignedItem);
					this.AssignedUsersListBox.Items.Remove(assignedItem);
				}
			}

		}

		protected void AssignUsersButton_Command(object sender, CommandEventArgs e)
		{
			ListItem availableItem;
			while ((availableItem = this.AvailableUsersListBox.SelectedItem) != null)
			{
				this.AssignedUsersListBox.Items.Add(availableItem);
				availableItem.Selected = false;
				this.AvailableUsersListBox.Items.Remove(availableItem);
			}

		}

		private void UpdateView()
		{
			//this.AssignedUsersListBox.Items.Clear();
			//this.AvailableUsersListBox.Items.Clear();
			//FMDAuditLogsClass FMDAuditLogs = new FMDAuditLogsClass();
			//AuditReadAccessCheckBox.Checked = FMDAuditLogs.Exists(Security, 19, 0, 1, 1);
			//this.AuditSelectedUsersCheckBox.Checked = false;

			//string[] userNames = FMDAuditLogs.EnumerateAssignedUsers(Security);
			//foreach (string name in userNames)
			//{
			//   if (name == "FMDAdmin" || name == "sa")
			//   {
			//      ListItem li = new ListItem(name);
			//      li.Enabled = false;
			//      this.AssignedUsersListBox.Items.Add(li);
			//      this.AuditSelectedUsersCheckBox.Checked = true;
			//   }
			//   else
			//   {
			//      this.AssignedUsersListBox.Items.Add(new ListItem(name, "assigned-"+name));
			//   }
			//}
			//userNames = FMDAuditLogs.EnumerateAvailableUsers(Security);

			//foreach (string name in userNames)
			//{
			//   this.AvailableUsersListBox.Items.Add(new ListItem(name, "available-"+name));
			//}
			//if (!this.AuditSelectedUsersCheckBox.Checked)
			//{
			//   ListItem li = new ListItem("FMDAdmin"); 
			//   li.Enabled = false;
			//   this.AssignedUsersListBox.Items.Add(li);
			//   li = new ListItem("sa");
			//   li.Enabled = false;
			//   this.AssignedUsersListBox.Items.Add(li);
			//}
			//AuditSelectedUsersCheckBox_CheckedChanged(null, null);

		}

		protected void AuditSelectedUsersCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.AssignedUsersListBox.Enabled = this.AuditSelectedUsersCheckBox.Checked;
			this.AssignUsersButton.Enabled = this.AuditSelectedUsersCheckBox.Checked;
			this.AvailableUsersListBox.Enabled = this.AuditSelectedUsersCheckBox.Checked;
			this.UnAssignUsersButton.Enabled = this.AuditSelectedUsersCheckBox.Checked;
		}


	}


}

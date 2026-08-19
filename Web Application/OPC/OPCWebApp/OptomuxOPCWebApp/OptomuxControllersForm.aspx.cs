/******************************************************************************

	FILE NAME:		OptomuxControllersForm.aspx.cs


	PURPOSE:			Implementation of OptomuxControllersForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaOptomuxController.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		2009-03-17  G.Kendall   WI# 1416 - Get OPC allowable from SecurityClass
 
*******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Web.UI.WebControls;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FuelsManager.FMWebApp;
using OpcCom;
using OptomuxOPCObjectsLib;
using OptomuxOPCServerLib;

namespace OPCWebApp.OptomuxOPCWebApp
{
    /// <summary>
	/// Summary description for OptomuxControllersForm.
	/// </summary>
	public partial class OptomuxControllersForm : OsdpFormBase, IMenuDiscovery
	{

        /// <summary>
        /// Gets a list of menu items that should be displayed for the current user.
        /// </summary>
        /// <param name="security">The security object of the current session</param>
        /// <param name="siteGroup">Whether the current logged-in site is a site group</param>
        /// <param name="word1">First word of license key options</param>
        /// <param name="word2">Second word of license key options</param>
        /// <returns>
        /// List of menu items to be displayed
        /// </returns>
        public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
        {
            // Depends upon Load Rack Service
            if (useNewLicenseKey == 1)
            {
                if ((word2 & 0x01) != 0x01)
                    return null;
            }
            else
            {
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
            }

            if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			// Depends On Optomux OPC Server
			if ((FMChannelHelper.MakeCall<IHardwareKey, uint>(x =>x.GetOPCAllowedFunctions()) & 0x200) == 0)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_OPTOMUX_OPC_CONTROLLERS,
				RootMenuName = "Configuration",
				CategoryName = "Optomux OPC",
				ItemName = "Controllers",
				NavigateUrl = "../OptomuxOPCWebApp/OptomuxControllersForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
				DataDictGroupPrefix = "Optomux"
			};

			menuItems.Add(menuItem);

			return menuItems;
		}

		private void UpdateView()
		{
			ICollection controllers = this.EnumerateOptomuxControllers();

			this.OptomuxControllersFormPageSizeDropDown.SetPageSize(this.OptomuxControllersDataGrid, controllers.Count);

			this.OptomuxControllersDataGrid.DataSource = controllers;
			this.OptomuxControllersDataGrid.DataBind();
		}

		private ICollection EnumerateOptomuxControllers()
		{
			IOptomuxControllers optomuxControllers = (IOptomuxControllers)OpcCom.Interop.CreateInstance(
				new Guid("{DD940B4F-C212-4361-8FDE-D4061584E4D0}"),
				this.Session["OptomuxControllersSystem"] as string,
				new NetworkCredential());

			OptomuxControllerCollectionClass optomuxControllerCollection = (OptomuxControllerCollectionClass)optomuxControllers.Enumerate();
			DataTable optomuxControllerDataTable = new DataTable();

		    optomuxControllerDataTable.Columns.Add("Index", typeof(int));
			optomuxControllerDataTable.Columns.Add("ID", typeof(string));
			optomuxControllerDataTable.Columns.Add("Type", typeof(string));
			optomuxControllerDataTable.Columns.Add("NetworkCommunications", typeof(bool));
			optomuxControllerDataTable.Columns.Add("Port", typeof(string));
			optomuxControllerDataTable.Columns.Add("IPAddress", typeof(string));

			for (int iItem = 0; iItem < optomuxControllerCollection.Count; iItem++)
			{
				var optomuxControllerDataRow = optomuxControllerDataTable.NewRow();

				var optomuxController = (OptomuxControllerClass)optomuxControllerCollection.Item(iItem);
				optomuxControllerDataRow["Index"] = optomuxController.Index;
				optomuxControllerDataRow["ID"] = optomuxController.ID;
				optomuxControllerDataRow["Type"] = optomuxController.TypeID(optomuxController.Type);
				optomuxControllerDataRow["NetworkCommunications"] = optomuxController.NetworkCommunications;

				string htmlCompatibleText = this.GetDictionaryText(optomuxController.PortID);
                htmlCompatibleText = htmlCompatibleText.Replace("&", "&amp");
                htmlCompatibleText = htmlCompatibleText.Replace(">", "&gt");
				htmlCompatibleText = htmlCompatibleText.Replace("<", "&lt");
				htmlCompatibleText = htmlCompatibleText.Replace("'", "&apos");
				htmlCompatibleText = htmlCompatibleText.Replace("\"", "&quot");

				optomuxControllerDataRow["Port"] = htmlCompatibleText;
				optomuxControllerDataRow["IPAddress"] = optomuxController.IPAddress;

				optomuxControllerDataTable.Rows.Add(optomuxControllerDataRow);
			}
			DataView optomuxControllerDataView = new DataView(optomuxControllerDataTable);
			return optomuxControllerDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					this.SystemTextBox.Text = @"localhost";
					this.Session["OptomuxControllersSystem"] = this.SystemTextBox.Text;

					// Populate SelectSystemModeDropDownList
					ListItem newItem = new ListItem("List", "0");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					newItem = new ListItem("Text", "1");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					this.SelectSystemModeDropDownList.SelectedIndex = 1;
					this.SelectSystemModeDropDownList_SelectedIndexChanged(null, null);
					if (this.Session["OptomuxControllersPage"] != null)
					{
						this.OptomuxControllersDataGrid.CurrentPageIndex = (int)this.Session["OptomuxControllersPage"];
						this.Session.Remove("OptomuxControllersPage");
					}

					this.UpdateView();

					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
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
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.OptomuxControllersDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.OptomuxControllersDataGrid_EditCommand);
			this.OptomuxControllersDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.OptomuxControllersDataGrid_PageIndexChanged);
			this.OptomuxControllersDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.OptomuxControllersDataGrid_DeleteCommand);
			this.OptomuxControllersDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.OptomuxControllersDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		private void OptomuxControllersDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove("OptomuxController");
			TableCell indexCell = e.Item.Cells[1];//bds
			this.Session["Index"] = indexCell.Text;
			this.Session["OptomuxControllersPage"] = this.OptomuxControllersDataGrid.CurrentPageIndex;
			this.Redirect("OptomuxControllerForm.aspx");
		}

		private void OptomuxControllersDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[1];//bds

			    // ReSharper disable once RedundantNameQualifier
				IOptomuxControllers optomuxControllers = (IOptomuxControllers)OpcCom.Interop.CreateInstance(
					new Guid("{DD940B4F-C212-4361-8FDE-D4061584E4D0}"),
					this.Session["OptomuxControllersSystem"] as string,
					new NetworkCredential());

				optomuxControllers.Purge(Convert.ToInt32(indexCell.Text));

				this.OptomuxControllersDataGrid.SelectedIndex = -1;
				this.Session.Remove("Index");
				if (this.OptomuxControllersDataGrid.Items.Count == 1
				&& this.OptomuxControllersDataGrid.CurrentPageIndex > 0)
					this.OptomuxControllersDataGrid.CurrentPageIndex--;
				this.UpdateView();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("OptomuxController");
			this.Session.Remove("Index");
			this.Session["OptomuxControllersPage"] = this.OptomuxControllersDataGrid.CurrentPageIndex;
			this.Redirect("OptomuxControllerForm.aspx");
		}

		private void OptomuxControllersDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.OptomuxControllersDataGrid.EditItemIndex > -1)
					return;
				this.OptomuxControllersDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void SystemDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["OptomuxControllersSystem"] = this.SystemDropDownList.SelectedItem.Text;
				this.OptomuxControllersDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void OptomuxControllersDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			this.UpdateDeleteButton((LinkButton)e.Item.FindControl("DeleteButton"));
		}

		protected void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.SystemDropDownList.Visible
				&& this.SystemDropDownList.SelectedIndex != -1)
				this.SystemTextBox.Text = this.SystemDropDownList.SelectedItem.Text;

			// Populate SystemDropDownList
			if (this.SelectSystemModeDropDownList.SelectedIndex == 0)
			{
				this.SystemDropDownList.Items.Clear();
				ListItem newItem = new ListItem("localhost", "0");
				this.SystemDropDownList.Items.Add(newItem);
				ServerEnumerator enumerator = new ServerEnumerator();
				string[] systems = enumerator.EnumerateHosts();
				int item = 1;

				foreach (string system in systems)
				{
					newItem = new ListItem(system, item.ToString());
					this.SystemDropDownList.Items.Add(newItem);
                    if (this.Session["OptomuxControllersSystem"] != null
                        && (string)this.Session["OptomuxControllersSystem"] == system)
						this.SystemDropDownList.SelectedIndex = this.SystemDropDownList.Items.Count - 1;
					item++;
				}

				this.Session["OptomuxControllersSystem"] = this.SystemDropDownList.SelectedItem.Text;
			}

			this.SystemDropDownList.Visible = (this.SelectSystemModeDropDownList.SelectedIndex != 1);
			this.SystemTextBox.Visible = !this.SystemDropDownList.Visible;
		}

		protected void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["OptomuxControllersSystem"] = this.SystemTextBox.Text;
				this.OptomuxControllersDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
	}
}

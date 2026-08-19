/******************************************************************************

	FILE NAME:		WeightScalesForm.aspx.cs


	PURPOSE:			Implementation of WeightScalesForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaWeightScale.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		2009-03-17  G.Kendall   WI# 1416 - Get OPC allowable from SecurityClass
 
*******************************************************************************/
using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using WeightScaleOPCObjectsLib;
using WeightScaleOPCServerLib;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

using OpcCom;

namespace WeightScaleOPCWebApp
{
   using FuelsManager.FMWebApp;

   using System.Collections.Generic;

   /// <summary>
   /// Summary description for WeightScalesForm
   /// </summary>
   public partial class WeightScalesForm : WeightScaleFormBase, IDataDictionary, IMenuDiscovery
	{
		string[] IDataDictionary.Keys(SecurityClass Security)
		{
			string[] Keys ={	"System",
									"Weight Scales",
									"ID",
									"Type",
									"Port"};

			return Keys;
		}


		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
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

			// Depends On Weight Scale OPC Server
			if ((FMChannelHelper.MakeCall<IHardwareKey, uint>(x => x.GetOPCAllowedFunctions()) & 0x10) == 0)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_WEIGHT_SCALES_OPC_WEIGHT_SCALES,
				RootMenuName = "Configuration",
				CategoryName = "Weight Scale OPC",
				ItemName = "WeightScales",
				NavigateUrl = "../WeightScaleOPCWebApp/WeightScalesForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
				DataDictGroupPrefix = "WeightScale"
			};

			menuItems.Add(menuItem);

			return menuItems;
		}

        protected override void UpdateView()
		{
			ICollection Scales = EnumerateWeightScales();

			WeightScalesFormPageSizeDropDown.SetPageSize(WeightScalesDataGrid, Scales.Count);

			WeightScalesDataGrid.DataSource = Scales;
			WeightScalesDataGrid.DataBind();
		}

		private ICollection EnumerateWeightScales()
		{
			WeightScaleCollectionClass WeightScaleCollection;

			try
			{
				IWeightScales WeightScales = (IWeightScales)OpcCom.Interop.CreateInstance(
					new Guid("{FB4C3029-D5C9-4BB8-AC5A-1914858D79D5}"),
					Session["WeightScaleSystem"] as string,
					new NetworkCredential());

				WeightScaleCollection = (WeightScaleCollectionClass)WeightScales.Enumerate();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
				WeightScaleCollection = new WeightScaleCollectionClass();
			}

			DataTable WeightScaleDataTable = new DataTable();
			DataRow WeightScaleDataRow;
			WeightScaleClass WeightScale;

			WeightScaleDataTable.Columns.Add("Index", typeof(Int32));
			WeightScaleDataTable.Columns.Add("ID", typeof(string));
			WeightScaleDataTable.Columns.Add("Type", typeof(string));
			WeightScaleDataTable.Columns.Add("Port", typeof(string));

			for (int iItem = 0; iItem < WeightScaleCollection.Count; iItem++)
			{
				WeightScaleDataRow = WeightScaleDataTable.NewRow();

				WeightScale = (WeightScaleClass)WeightScaleCollection.Item(iItem);
				WeightScaleDataRow["Index"] = WeightScale.Index;
				WeightScaleDataRow["ID"] = WeightScale.ID;
				WeightScaleDataRow["Type"] = WeightScale.TypeID(WeightScale.Type);
				WeightScaleDataRow["Port"] = WeightScale.Port;

				WeightScaleDataTable.Rows.Add(WeightScaleDataRow);
			}
			DataView WeightScaleDataView = new DataView(WeightScaleDataTable);
			return WeightScaleDataView;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				GetSecurity();

				if (!Page.IsPostBack)
				{
					SystemTextBox.Text = "localhost";
					Session["WeightScaleSystem"] = SystemTextBox.Text;

					// Populate SelectSystemModeDropDownList
					ListItem NewItem = new ListItem("List", "0");
					SelectSystemModeDropDownList.Items.Add(NewItem);
					NewItem = new ListItem("Text", "1");
					SelectSystemModeDropDownList.Items.Add(NewItem);
					SelectSystemModeDropDownList.SelectedIndex = 1;
					SelectSystemModeDropDownList_SelectedIndexChanged(null, null);

					if (Session["WeightScalesPage"] != null)
					{
						WeightScalesDataGrid.CurrentPageIndex = (int)Session["WeightScalesPage"];
						Session.Remove("WeightScalesPage");
					}

					UpdateView();

					if (!Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						AddButton.Enabled = false;
						AddButton2.Enabled = false;
					}

				}
			}
			catch (Exception except)
			{
				ErrorHandler(except);
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
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.WeightScalesDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.WeightScalesDataGrid_EditCommand);
			this.WeightScalesDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.WeightScalesDataGrid_PageIndexChanged);
			this.WeightScalesDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.WeightScalesDataGrid_DeleteCommand);
			this.WeightScalesDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.WeightScalesDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		protected void PageSizeDropDown_SelectedIndexChanged(object source, System.EventArgs e)
		{
			UpdateView();
		}

		private void WeightScalesDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Session.Remove("WeightScale");
			TableCell indexCell = e.Item.Cells[2];
			Session["Index"] = indexCell.Text;
			Session["WeightScalesPage"] = WeightScalesDataGrid.CurrentPageIndex;
			this.Redirect("WeightScaleForm.aspx");
		}

		private void WeightScalesDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[2];

				IWeightScales WeightScales = (IWeightScales)OpcCom.Interop.CreateInstance(
					new Guid("{FB4C3029-D5C9-4BB8-AC5A-1914858D79D5}"),
					Session["WeightScaleSystem"] as string,
					new NetworkCredential());

				WeightScales.Purge(System.Convert.ToInt32(indexCell.Text));

				WeightScalesDataGrid.SelectedIndex = -1;
				Session.Remove("Index");
				if (WeightScalesDataGrid.Items.Count == 1
				&& WeightScalesDataGrid.CurrentPageIndex > 0)
					WeightScalesDataGrid.CurrentPageIndex--;
				UpdateView();

			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			Session.Remove("WeightScale");
			Session.Remove("Index");
			Session["WeightScalesPage"] = WeightScalesDataGrid.CurrentPageIndex;
			this.Redirect("WeightScaleForm.aspx");
		}

		private void WeightScalesDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (WeightScalesDataGrid.EditItemIndex > -1)
					return;
				WeightScalesDataGrid.CurrentPageIndex = e.NewPageIndex;
				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		protected void SystemDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				Session["WeightScaleSystem"] = SystemDropDownList.SelectedItem.Text;
				WeightScalesDataGrid.CurrentPageIndex = 0;
				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		private void WeightScalesDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			UpdateDeleteButton((LinkButton)e.Item.FindControl("DeleteButton"));
		}

		protected void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (SystemDropDownList.Visible
				&& SystemDropDownList.SelectedIndex != -1)
				SystemTextBox.Text = SystemDropDownList.SelectedItem.Text;

			// Populate SystemDropDownList
			if (SelectSystemModeDropDownList.SelectedIndex == 0)
			{
				SystemDropDownList.Items.Clear();
				ListItem NewItem = new ListItem("localhost", "0");
				SystemDropDownList.Items.Add(NewItem);
				ServerEnumerator Enumerator = new ServerEnumerator();
				string[] Systems = Enumerator.EnumerateHosts();
				int Item = 1;

				foreach (string System in Systems)
				{
					NewItem = new ListItem(System, Item.ToString());
					SystemDropDownList.Items.Add(NewItem);
                    if (Session["WeightScaleSystem"] != null
                        && (string)Session["WeightScaleSystem"] == System)
						SystemDropDownList.SelectedIndex = SystemDropDownList.Items.Count - 1;
					Item++;
				}

				Session["WeightScaleSystem"] = SystemDropDownList.SelectedItem.Text;
			}

			SystemDropDownList.Visible = SelectSystemModeDropDownList.SelectedIndex != 1;
			SystemTextBox.Visible = !SystemDropDownList.Visible;
		}

		protected void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				Session["WeightScaleSystem"] = SystemTextBox.Text;
				WeightScalesDataGrid.CurrentPageIndex = 0;
				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}
	}
}

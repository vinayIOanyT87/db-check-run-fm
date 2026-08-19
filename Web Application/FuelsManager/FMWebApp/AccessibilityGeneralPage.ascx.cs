/******************************************************************************
	FILE NAME:		AccessibilityGeneralPage.ascx.cs
	PURPOSE:		Implementation of AccessibilityGeneralPage
	
	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version


 
*******************************************************************************/

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.UtilityObjects;

	public partial class AccessibilityGeneralPage : AccessibilityPageBase
	{


		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (this.Page.IsPostBack == false)
				{
					this.UpdateData();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method overrides and implements the base class enable controls.
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableControls(bool enable)
		{
			if (this.Page is AccessibilityForm)
			{
				// Call the main form to disable buttons and tabs.
				var accessibilityForm = (AccessibilityForm)this.Page;
				accessibilityForm.EnableControls(enable);
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
			this.AccessibilityDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AccessibilityDataGridEditCommand);
			this.AccessibilityDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.AccessibilityDataGridPageIndexChanged);
			this.AccessibilityDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AccessibilityDataGridCancelCommand);
			this.AccessibilityDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AccessibilityDataGridUpdateCommand);
			this.AccessibilityDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.AccessibilityDataGridItemDataBound);
		}
		#endregion
		protected void AccessibilityDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			object o = e.Item.DataItem;
			if (e.Item.ItemType != ListItemType.EditItem && e.Item.ItemType != ListItemType.Header )
			{
				e.Item.Enabled = (this.AccessibilityDataGrid.SelectedIndex == -1);
			}


		}
		protected void AccessibilityDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{

			this.AccessibilityDataGrid.EditItemIndex = -1;
			this.AccessibilityDataGrid.SelectedIndex = -1;
			this.UpdateData();
			if (this.Page is AccessibilityForm)
			{
				((AccessibilityForm)this.Page).EnableControls(true);
			}

		}

	
		protected void AccessibilityDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{

				this.AccessibilityDataGrid.EditItemIndex = e.Item.ItemIndex;
				this.AccessibilityDataGrid.SelectedIndex = e.Item.ItemIndex;

				this.UpdateData();
				if (this.Page is AccessibilityForm)
				{
					((AccessibilityForm)this.Page).EnableControls(false);
				}

		}
		protected void AccessibilityDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AccessibilityDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AccessibilityDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateData();
		}


		protected void AccessibilityDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			if (this.Page is AccessibilityForm)
			{
				((AccessibilityForm)this.Page).EnableControls(true);
			}

			if (Accessibilities == null)
				return;
			var ds = Accessibilities;
			AccessibilityClass a = ds.Item(this.AccessibilityDataGrid.EditItemIndex + this.AccessibilityDataGrid.CurrentPageIndex * this.AccessibilityDataGrid.PageSize);//] as AccessibilityClass;
			var dropDownList = (DropDownList)e.Item.FindControl("SettingValueDropDown");
			if (dropDownList != null) {
			
				string x = dropDownList.SelectedValue;
				a.SettingValue = x;

	
			}
			this.AccessibilityDataGrid.EditItemIndex = -1;
			this.AccessibilityDataGrid.SelectedIndex = -1;
			this.UpdateData();

		}

		public void UpdateData()
		{
			if (Accessibilities == null)
			{
				return;
			}
			var ds = Accessibilities;
			this.AccessibilityDataGrid.DataSource = ds;
			this.AccessibilityDataGrid.DataBind();
		}

		protected ListItemCollection PopulateList()
		{

			var qualificationItems = new ListItemCollection();

			if (this.AccessibilityDataGrid.EditItemIndex > -1)
			{
				AccessibilityCollectionClass ds = this.AccessibilityDataGrid.DataSource as AccessibilityCollectionClass;
				AccessibilityClass a = ds.Item(this.AccessibilityDataGrid.EditItemIndex + this.AccessibilityDataGrid.CurrentPageIndex * this.AccessibilityDataGrid.PageSize) as AccessibilityClass;
				if (a != null)
				{
					char []del = new char[1]{';'};
					string[] options = a.ValueRange.Split(del);
					foreach (string c in options)
					{
						var listItem = new ListItem(c, c);
						qualificationItems.Add(listItem);
					}
				}
			}

			return qualificationItems;
		}
		public AccessibilityCollectionClass EnumerateAccessibility(Guid userGuid)
		{
			AccessibilityCollectionClass accessibilityCollection = FMChannelHelper.MakeCall<IAccessibilities, AccessibilityCollectionClass>(
					x =>
					x.Enumerate(this.Security, userGuid)
			);
			return accessibilityCollection;
		}
	}
}

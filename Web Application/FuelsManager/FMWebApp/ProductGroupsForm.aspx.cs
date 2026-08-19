// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductGroupsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductGroupsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for ProductGroupsForm.
	/// </summary>
	public partial class ProductGroupsForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IProductGroups);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.PRODUCT_GROUP;
			}
		}

		#endregion

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
            if (useNewLicenseKey == 1)
            {
                if ((word2 & 0x01) != 0x01)
                    return null;
            }
            else
            {
                // Depends upon Load Rack Service
                if ((options & 0x8000) == 0)
                {
                    return null;
                }

                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_PRODUCTS) && !security.HasRight(RIGHT.MODIFY_PRODUCTS))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ASSETS_PRODUCTS_PRODUCT_GROUPS,
						RootMenuName = "Assets",
						CategoryName = "Products",
						ItemName = "Product Groups",
						NavigateUrl = "ProductGroupsForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}
		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			ProductGroupCollectionClass productGroupCollection = 
				FMChannelHelper.MakeCall<IProductGroups, ProductGroupCollectionClass>(x => x.Enumerate(security));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (ProductGroupClass productGroup in productGroupCollection)
			{
				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == productGroup.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != productGroup.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != productGroup.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(productGroup);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}
			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IProductGroups, Guid>(x => x.GetIdentityGuid(security, ID));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			ProductGroupClass productGroup = FMChannelHelper.MakeCall<IProductGroups, ProductGroupClass>(
																	 x => x.Get(security, guid));
			productGroup.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IProductGroups>(x => x.Modify(security, productGroup));
		}
		#endregion

		#region Methods
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["ProductGroupsPage"] != null)
					{
						this.ProductGroupsDataGrid.CurrentPageIndex = (int)this.Session["ProductGroupsPage"];
						this.Session.Remove("ProductGroupsPage");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("IdentityGuid");
			this.Session["ProductGroupsPage"] = this.ProductGroupsDataGrid.CurrentPageIndex;
			this.Redirect("ProductGroupForm.aspx");
		}

		private ICollection EnumerateProductGroups()
		{
			ProductGroupCollectionClass productGroupCollection = FMChannelHelper.MakeCall<IProductGroups, ProductGroupCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			var productGroupDataTable = new DataTable();

			productGroupDataTable.Columns.Add("SiteGuid", typeof(Guid));
			productGroupDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			productGroupDataTable.Columns.Add("ID", typeof(string));

			foreach (ProductGroupClass productGroup in productGroupCollection)
			{
				DataRow productGroupDataRow = productGroupDataTable.NewRow();

				productGroupDataRow["SiteGuid"] = productGroup.SiteGuid;
				productGroupDataRow["IdentityGuid"] = productGroup.IdentityGuid;
				productGroupDataRow["ID"] = productGroup.ID;

				productGroupDataTable.Rows.Add(productGroupDataRow);
			}

			return new DataView(productGroupDataTable);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command						+= this.AddButtonCommand;
			this.ProductGroupsDataGrid.EditCommand		+= this.ProductGroupsDataGridEditCommand;
			this.ProductGroupsDataGrid.PageIndexChanged += this.ProductGroupsDataGridPageIndexChanged;
			this.ProductGroupsDataGrid.DeleteCommand	+= this.ProductGroupsDataGridDeleteCommand;
			this.ProductGroupsDataGrid.ItemDataBound	+= this.ProductGroupsDataGridItemDataBound;
			this.AddButton.Command						+= this.AddButtonCommand;
		}

		private void ProductGroupsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get IdentityGuid
				TableCell guidCell = e.Item.Cells[2];//bds

				FMChannelHelper.MakeCall<IProductGroups>(x => x.Purge(this.Security, Guid.Parse(guidCell.Text)));

				this.ProductGroupsDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
				
				if (this.ProductGroupsDataGrid.Items.Count == 1 && this.ProductGroupsDataGrid.CurrentPageIndex > 0)
				{
					this.ProductGroupsDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ProductGroupsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			TableCell guidCell = e.Item.Cells[2];//bds
			this.Session["IdentityGuid"] = guidCell.Text;
			this.Session["ProductGroupsPage"] = this.ProductGroupsDataGrid.CurrentPageIndex;
			this.Redirect("ProductGroupForm.aspx");
		}

		private void ProductGroupsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			
			if (deleteButton != null)
			{
				TableCell siteGuidCell = e.Item.Cells[1];//bds
				if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS) || this.Security.SiteGuid != Guid.Parse(siteGuidCell.Text))
				{
					deleteButton.Enabled = false;
					deleteButton.Text = "<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
				}
			}
		}

		private void ProductGroupsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.ProductGroupsDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.ProductGroupsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void UpdateView()
		{
			ICollection groups = this.EnumerateProductGroups();

			this.ProductGroupsFormPageSizeDropDown.SetPageSize(this.ProductGroupsDataGrid, groups.Count);

			this.ProductGroupsDataGrid.DataSource = groups;
			this.ProductGroupsDataGrid.DataBind();
		}
		#endregion
	}
}
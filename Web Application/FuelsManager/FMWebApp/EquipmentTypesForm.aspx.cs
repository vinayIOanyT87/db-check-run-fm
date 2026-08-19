// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentTypesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipmentTypesForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for EquipmentTypesForm.
	/// </summary>
	public partial class EquipmentTypesForm : FMFormBaseAjax, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields

		private const string EquipmentTypeFindString = "EquipmentTypeFindString";
		private const string SortDirection = "EquipmentTypeSortDirection";
		private const string SortExpression = "EquipmentTypeSortExpression";

		private string searchString;

		#endregion

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
				return typeof(IEquipmentTypes);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.EQUIPMENT_TYPE;
			}
		}

		#endregion

		#region Public Methods and Operators

		public void EquipmentTypesDataGridRowCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.GetSecurity();
				if (e == null)
				{
					throw new Exception("Null GridViewCommandEventArgs.");
				}

				if (e.CommandName == "Edit")
				{
					int index = Convert.ToInt32(e.CommandArgument);
					DataKey dataKey = this.EquipmentTypesDataGrid.DataKeys[index];
					if (dataKey == null)
					{
						throw new Exception("Null DataKey returned for row " + e.CommandArgument);
					}

					object obj = dataKey["EquipmentTypeGuid"];
					if (obj == null)
					{
						throw new Exception("Null value returned by dataKey[EquipmentTypeGuid] for row " + e.CommandArgument);
					}
					var quipmentTypeGuid = (Guid)obj;

					this.Session["SelectedEquipmentType"] = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
																	 x =>
																	 x.Get(this.Security, quipmentTypeGuid)
																);

					this.Redirect("EquipmentTypeDetailsForm.aspx");
				}
				else if (e.CommandName == "Delete")
				{
					int index = Convert.ToInt32(e.CommandArgument);
					DataKey dataKey = this.EquipmentTypesDataGrid.DataKeys[index];
					if (dataKey == null)
					{
						throw new Exception("Null DataKey returned for row " + e.CommandArgument);
					}

					object obj = dataKey["EquipmentTypeGuid"];
					if (obj == null)
					{
						throw new Exception("Null value returned by dataKey[EquipmentTypeGuid] for row " + e.CommandArgument);
					}
					var equipmentTypeGuid = (Guid)obj;

					FMChannelHelper.MakeCall<IEquipmentTypes>(
																	 x =>
																	 x.Purge(this.Security, equipmentTypeGuid)
																);

					this.EquipmentTypesDataGrid.SelectedIndex = -1;
					this.Session.Remove("SelectedEquipmentType");
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Update the view when the user changes the grid page size
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void PageSizeDropDown_OnSelectedIndexChanged( object sender, EventArgs e )
		{
			try
			{
				this.EquipmentTypesDataGrid.EditIndex = -1;
				this.UpdateView();
			}
			catch ( Exception ex )
			{
				this.ErrorHandler( ex );
			}
		}

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

            }
            else
            {
                // Depends Upon Accounting
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ASSETS_EQUIPMENT_TYPE_CLASSES,
					RootMenuName = "Assets",
					CategoryName = "Equipment",
					ItemName = "Types",
					NavigateUrl = "EquipmentTypesForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods
		
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			EquipmentTypeCollectionClass equipmentTypeCollection = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeCollectionClass>(
				x =>
					x.Enumerate(security, null, null)
				);

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (EquipmentTypeClass equipmentType in equipmentTypeCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == equipmentType.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != equipmentType.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != equipmentType.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(equipmentType);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}
			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IEquipmentTypes, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, id)
																);
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			EquipmentTypeClass equipmentType = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
																	 x =>
																	 x.Get(security, guid)
																);

			equipmentType.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IEquipmentTypes>(
																	 x =>
																	 x.Modify(security, equipmentType)
																);
		}

		#endregion

		#region Methods

		protected void EquipmentTypesDataGridPageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			if (this.EquipmentTypesDataGrid.EditIndex > -1)
			{
				return;
			}
			this.EquipmentTypesDataGrid.PageIndex = e.NewPageIndex;
			this.Session["EquipmentTypesDataGrid.PageIndex"] = e.NewPageIndex;
			this.UpdateView();
		}

		protected void EquipmentTypesDataGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					DataKey dataKey = this.EquipmentTypesDataGrid.DataKeys[e.Row.RowIndex];
					if (dataKey != null)
					{
						var siteGuid = (Guid)(dataKey["SiteGuid"]);

						var editButton = (FMEditLinkButton)e.Row.FindControl("EditButton");
						if (editButton != null)
						{
							editButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
						}

						var deleteButton = (FMDeleteLinkButton)e.Row.FindControl("DeleteButton");
						if (deleteButton != null)
						{
							deleteButton.Enabled = (siteGuid == this.Security.SiteGuid && this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA));
							deleteButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void EquipmentTypesDataGridSort(object sender, GridViewSortEventArgs e)
		{
			var sortExpression = this.Session[SortExpression] as string;
			var sortDirection = this.Session[SortDirection] as string;

			if (e.SortExpression != sortExpression)
			{
				this.Session[SortDirection] = "DESC";
			}
			else
			{
				if (sortDirection == "DESC")
				{
					this.Session[SortDirection] = "ASC";
				}
				else
				{
					this.Session[SortDirection] = "DESC";
				}
			}
			this.Session[SortExpression] = e.SortExpression;

			this.UpdateView();
		}

		//*************************************************************************************************
		// This method is called when the find button is pressed. It will retrieve data from the find
		// text box and set the search string. If there is no data, then the search string is set to null.
		//*************************************************************************************************
		protected void FindBtnOnClick(object sender, EventArgs e)
		{
			if ((this.FindTextBox == null) || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(EquipmentTypeFindString);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.FindTextBox.Text = this.searchString;
				this.Session.Add(EquipmentTypeFindString, this.searchString);
			}

			// Update the page with the new contents.
			this.EquipmentTypesDataGrid.PageIndex = 0;
			this.UpdateView();
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				if (this.Page.IsPostBack == false)
				{
					if (this.Session["EquipmentTypesDataGrid.PageIndex"] == null)
					{
						this.Session["EquipmentTypesDataGrid.PageIndex"] = 0;
					}

					this.EquipmentTypesDataGrid.PageIndex = (int)this.Session["EquipmentTypesDataGrid.PageIndex"];
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}
					if (this.Session[SortExpression] == null)
					{
						this.Session[SortExpression] = "EqTypeName";
					}
					if (this.Session[SortDirection] == null)
					{
						this.Session[SortDirection] = "DESC";
					}
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		//**************************************************************************************************
		// This method is called when the show all button is pressed. It will set the search string to null
		// indicating that we do not want to use the filter on finding companies.  In addition, the find
		// text box is cleared.
		//**************************************************************************************************
		protected void ShowAllBtnOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(EquipmentTypeFindString);
			this.searchString = null;
			this.FindTextBox.Text = "";
			this.EquipmentTypesDataGrid.PageIndex = 0;
			this.UpdateView();
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("SelectedEquipmentType");
			this.Redirect("EquipmentTypeDetailsForm.aspx");
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton.Command += this.AddButtonCommand;
			this.AddButton2.Command += this.AddButtonCommand;
			this.EquipmentTypesDataGrid.RowCommand += this.EquipmentTypesDataGridRowCommand;
			this.EquipmentTypesDataGrid.Sorting += this.EquipmentTypesDataGridSort;
			this.EquipmentTypesDataGrid.RowDataBound += this.EquipmentTypesDataGridRowDataBound;
			this.EquipmentTypesDataGrid.PageIndexChanging += this.EquipmentTypesDataGridPageIndexChanging;
			var limits = new EnumerationLimits();
			this.EquipmentTypesDataGrid.PageSize = limits.GetLimit(EnumerationLimits.EnumerationOptions.EQUIPMENT);
		}

		private void UpdateView()
		{
			// Locate the previous search string from the session. Set the set
			// string if found.
			if (this.Session[EquipmentTypeFindString] != null)
			{
				this.searchString = this.Session[EquipmentTypeFindString] as string;
			}

			DataSet ds = FMChannelHelper.MakeCall<IEquipmentTypes, DataSet>(
					x =>
					x.EnumerateDataSet(this.Security, this.searchString, null)
			);

			if (ds.Tables.Count > 0)
			{
				ds.Tables[0].Columns["LookupEquipmentTypeIndex"].ColumnName = "AttributeInt";
				ds.Tables[0].Columns.Add("LookupEquipmentTypeIndex");
				ds.Tables[0].Columns["LookupEquipmentTypeIndex"].DataType = Type.GetType("System.String");

				foreach (DataRow row in ds.Tables[0].Rows)
				{
					if (!row.IsNull("AttributeInt"))
					{
						row["LookupEquipmentTypeIndex"] =
							this.GetTranslatedText(EquipmentTypeClass.TypeID((EQUIPMENT_TYPE)row["AttributeInt"]));
					}
				}

				

				var dv = new DataView(ds.Tables[0]);
				if (this.Session[SortExpression] != null && this.Session[SortDirection] != null)
				{
					dv.Sort = String.Format("{0} {1}", this.Session[SortExpression], this.Session[SortDirection]);
				}

				this.PageSizeDropDown.SetPageSize(this.EquipmentTypesDataGrid, dv.Count);

				this.EquipmentTypesDataGrid.DataSource = ds;
				this.EquipmentTypesDataGrid.DataBind();
			}
		}

		#endregion
	}

}
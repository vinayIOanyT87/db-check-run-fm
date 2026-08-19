// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HouseCardsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the HouseCardsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;
    using FMBusinessObjects.UtilityObjects;

    /// <summary>
	///    Summary description for HouseCardsForm.
	/// </summary>
	public partial class HouseCardsForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Constants and Fields
		protected int PriorEditItemIndex = -2;
		#endregion

		#region Explicit Interface Properties
		bool IEntityDiscovery.EntityAssignable => false;

        Type IEntityDiscovery.EntityEngineType => typeof(IHouseCards);

        ENTITY_TYPE IEntityDiscovery.EntityType => ENTITY_TYPE.HOUSE_CARD;

        #endregion

		#region Public Methods and Operators

		public ListItemCollection EnumerateDrivers()
		{
			var driverItems = new ListItemCollection { new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString()) };

			var houseCardCollection = (HouseCardCollectionClass)this.Session["HouseCardCollection"];
			HouseCardClass currentHouseCard =
				houseCardCollection[
					this.HouseCardsDataGrid.EditItemIndex + this.HouseCardsDataGrid.PageSize * this.HouseCardsDataGrid.CurrentPageIndex
					];

		    var driverCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(x => x.EnumerateByRole(this.Security, PERSON_ROLE.LOADER_ROLE));
			foreach (PersonClass driver in driverCollection)
			{
				bool driverFound = false;
				foreach (HouseCardClass houseCard in houseCardCollection)
				{
					if (driver.MasterRecordGuid == houseCard.DriverGuid && houseCard.IdentityGuid != currentHouseCard.IdentityGuid)
					{
						driverFound = true;
						break;
					}
				}

				if (!driverFound)
				{
					var item = new ListItem(driver.ID, driver.MasterRecordGuid.ToString());
					driverItems.Add(item);
				}
			}

            driverCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(x => x.EnumerateByRole(this.Security, PERSON_ROLE.OFFLOADER_ROLE));
            foreach (PersonClass driver in driverCollection)
            {
                bool driverFound = false;
                foreach (HouseCardClass houseCard in houseCardCollection)
                {
                    if (driver.MasterRecordGuid == houseCard.DriverGuid && houseCard.IdentityGuid != currentHouseCard.IdentityGuid)
                    {
                        driverFound = true;
                        break;
                    }
                }

                if (!driverFound)
                {
                    var item = new ListItem(driver.ID, driver.MasterRecordGuid.ToString());
                    driverItems.Add(item);
                }
            }

            return driverItems;
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

            var items = new List<FMMenuItem>();

			//ADF Defense product. Don't add node.
			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
			{
				return null;
			}

			// TODO:  Confirm if house cards should use load rack permissions or personnel permissions
			// if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
				&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ASSETS_PERSONNEL_HOUSE_CARDS,
						RootMenuName = "Assets",
						CategoryName = "Personnel",
						ItemName = "House Cards",
						NavigateUrl = "HouseCardsForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}
		#endregion

		#region Explicit Interface Methods
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, 
																			ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			HouseCardCollectionClass houseCardCollection = 
				FMChannelHelper.MakeCall<IHouseCards, HouseCardCollectionClass>(x => x.Enumerate(security));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (HouseCardClass houseCard in houseCardCollection)
			{
				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == houseCard.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != houseCard.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != houseCard.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(houseCard);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return Guid.Empty;
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			HouseCardClass houseCard = FMChannelHelper.MakeCall<IHouseCards, HouseCardClass>(x => x.Get(security, guid));

			houseCard.SiteGuid = siteGuid;
			FMChannelHelper.MakeCall<IHouseCards>(x => x.Modify(security, houseCard));
		}
		#endregion

		#region Methods
		protected void AddButtonCommand(object sender, CommandEventArgs e)
		{
			var houseCardCollection = (HouseCardCollectionClass)this.Session["HouseCardCollection"];
			var houseCard = new HouseCardClass();

			houseCardCollection.Add(houseCard);
			this.HouseCardsDataGrid.CurrentPageIndex = (houseCardCollection.Count - 1) / this.HouseCardsDataGrid.PageSize;
			this.HouseCardsDataGrid.EditItemIndex = (houseCardCollection.Count - 1) % this.HouseCardsDataGrid.PageSize;

			this.EnableControls(false);
			this.UpdateView();
		}

		protected void HouseCardsDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			var houseCardCollection = (HouseCardCollectionClass)this.Session["HouseCardCollection"];
			HouseCardClass houseCard = houseCardCollection[e.Item.DataSetIndex];

			if (houseCard.IdentityGuid.IsEmpty())
			{
				houseCardCollection.RemoveAt(e.Item.DataSetIndex);

				if ((this.HouseCardsDataGrid.Items.Count == 1) && (this.HouseCardsDataGrid.CurrentPageIndex > 0))
				{
					this.HouseCardsDataGrid.CurrentPageIndex--;
				}
			}

			this.PriorEditItemIndex = this.HouseCardsDataGrid.EditItemIndex;
			this.HouseCardsDataGrid.EditItemIndex = -1;
			this.EnableControls(true);
			this.UpdateView();
		}

		protected void HouseCardsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var houseCardCollection = (HouseCardCollectionClass)this.Session["HouseCardCollection"];

				HouseCardClass houseCard = houseCardCollection[e.Item.DataSetIndex];

				if (this.HouseCardsDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.HouseCardsDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}

				else if (this.HouseCardsDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.HouseCardsDataGrid.EditItemIndex--;
				}

				// Non empty indicates HouseCard has been committed to database
				if (!houseCard.IdentityGuid.IsEmpty())
				{
					FMChannelHelper.MakeCall<IHouseCards>(x => x.Purge(this.Security, houseCard.IdentityGuid));
				}

				houseCardCollection.RemoveAt(e.Item.DataSetIndex);

				if (this.HouseCardsDataGrid.Items.Count == 1 && this.HouseCardsDataGrid.CurrentPageIndex > 0)
				{
					this.HouseCardsDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void HouseCardsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.HouseCardsDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.EnableControls(false);
			this.UpdateView();
		}

		protected void HouseCardsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.HouseCardsDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.HouseCardsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		protected void HouseCardsDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var houseCardCollection = (HouseCardCollectionClass)this.Session["HouseCardCollection"];

				var idTextBox = (TextBox)e.Item.FindControl("IDTextBox");
				var numberTextBox = (TextBox)e.Item.FindControl("NumberTextBox");
				var driversDropDownList = (DropDownList)e.Item.FindControl("DriversDropDownList");

				HouseCardClass houseCard = houseCardCollection[e.Item.DataSetIndex];
				houseCard.ID = idTextBox.Text;
				houseCard.Number = numberTextBox.Text;
				Guid tempGuid;

				if (Guid.TryParse(driversDropDownList.SelectedValue, out tempGuid))
				{
					houseCard.DriverGuid = tempGuid;
				}

				if (houseCard.DriverGuid.IsEmpty())
				{
					houseCard.DriverID = string.Empty;
				}
				else
				{
					houseCard.DriverID = driversDropDownList.SelectedItem.Text;
				}

				if (houseCard.IdentityGuid.IsEmpty())
				{
					houseCard.IdentityGuid = FMChannelHelper.MakeCall<IHouseCards, Guid>(
																	 x =>
																	 x.Add(this.Security, houseCard)
																);

					houseCard.SiteGuid = this.Security.SiteGuid;
				}
				else
				{
					FMChannelHelper.MakeCall<IHouseCards>(x => x.Modify(this.Security, houseCard));
				}

				this.PriorEditItemIndex = this.HouseCardsDataGrid.EditItemIndex;
				this.HouseCardsDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

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
					// TODO: Determine if house cards should use load rack rights or personnel rights
					// if (!this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
					if (!this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					// Enumerate 
					HouseCardCollectionClass houseCardCollection = FMChannelHelper.MakeCall<IHouseCards, HouseCardCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

					this.Session["HouseCardCollection"] = houseCardCollection;

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void UpdateView()
		{
			ICollection houseCards = this.EnumerateHouseCards();

			this.HouseCardsFormPageSizeDropDown.SetPageSize(this.HouseCardsDataGrid, houseCards.Count);

			this.HouseCardsDataGrid.DataSource = houseCards;
			this.HouseCardsDataGrid.DataBind();
		}

		/// <summary>
		///    This method enables/disables controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;
			this.HouseCardsFormPageSizeDropDown.Enabled = enable;
		}

		private ICollection EnumerateHouseCards()
		{
			var houseCardCollection = (HouseCardCollectionClass)this.Session["HouseCardCollection"];

			var houseCardDataTable = new DataTable();

			houseCardDataTable.Columns.Add("ID", typeof(string));
			houseCardDataTable.Columns.Add("Number", typeof(string));
			houseCardDataTable.Columns.Add("DriverID", typeof(string));

			foreach (HouseCardClass houseCard in houseCardCollection)
			{
				DataRow houseCardDataRow = houseCardDataTable.NewRow();

				houseCardDataRow["ID"] = houseCard.ID;
				houseCardDataRow["Number"] = houseCard.Number;
				houseCardDataRow["DriverID"] = houseCard.DriverID;

				houseCardDataTable.Rows.Add(houseCardDataRow);
			}

			var houseCardDataView = new DataView(houseCardDataTable);
			return houseCardDataView;
		}

		private void HouseCardsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var editButton = (LinkButton)e.Item.FindControl("EditButton");
			var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

			if (editButton != null && deleteButton != null)
			{
				if (!this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				{
					editButton.Enabled = false;
					deleteButton.Enabled = false;
				}
			}

			if (e.Item.ItemIndex != -1 && e.Item.ItemIndex == this.HouseCardsDataGrid.EditItemIndex)
			{
				var houseCardCollection = (HouseCardCollectionClass)this.Session["HouseCardCollection"];
				HouseCardClass houseCard = houseCardCollection[e.Item.DataSetIndex];
				var driversDropDownList = (DropDownList)e.Item.FindControl("DriversDropDownList");
				ListItem item = driversDropDownList.Items.FindByValue(houseCard.DriverGuid.ToString());

				if (item != null)
				{
					driversDropDownList.SelectedIndex = driversDropDownList.Items.IndexOf(item);
				}
			}

			if ((this.HouseCardsDataGrid != null && this.HouseCardsDataGrid.EditItemIndex == e.Item.ItemIndex)
			    || this.PriorEditItemIndex == e.Item.ItemIndex)
			{
				Control ctrl;

				var houseCardsDataGrid = this.HouseCardsDataGrid;

				if (houseCardsDataGrid != null && houseCardsDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					ctrl = e.Item.FindControl("IDTextBox");
				}
				else
				{
					ctrl = e.Item.FindControl("EditButton");
				}

				if (ctrl != null)
				{
					const string Script = @"<script language='javascript'> document.getElementById('{0}').focus(); </script>";
					this.Page.ClientScript.RegisterStartupScript(this.GetType(), 
																"page_set_focus", 
																string.Format(Script, ctrl.ClientID));
				}
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command						+= this.AddButtonCommand;
			this.HouseCardsDataGrid.EditCommand			+= this.HouseCardsDataGridEditCommand;
			this.HouseCardsDataGrid.PageIndexChanged	+= this.HouseCardsDataGridPageIndexChanged;
			this.HouseCardsDataGrid.CancelCommand		+= this.HouseCardsDataGridCancelCommand;
			this.HouseCardsDataGrid.UpdateCommand		+= this.HouseCardsDataGridUpdateCommand;
			this.HouseCardsDataGrid.DeleteCommand		+= this.HouseCardsDataGridDeleteCommand;
			this.HouseCardsDataGrid.ItemDataBound		+= this.HouseCardsDataGridItemDataBound;
			this.AddButton.Command						+= this.AddButtonCommand;
		}
		#endregion
	}
}
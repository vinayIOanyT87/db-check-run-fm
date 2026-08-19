// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReserveLevelConfigPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.ReserveLevelWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMWebApp;

	/// <summary>
	///     Summary description for ReserveLevelConfigPage.
	/// </summary>
	public partial class ReserveLevelConfigPage : FMFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		public const string NAVIGATE_URL = "../ReserveLevelWebApp/ReserveLevelConfigPage.aspx";

		private const string COLLECTION = "ReserveLevels_Collection";

		private const string MSG001 = "Minimum Level must be a number.";

		private const string MSG002 = "Warning Level must be a number.";

		private const string MSG003 = "Minimum Level must be greater than zero.";

		private const string MSG004 = "Warning Level must be greater than zero.";

		private const string MSG005 = "Minimum Level must be less than Warning Level.";

		private const string MSG006 = "is a required field.";

		private const string MSG007 = "Could not update item ";

		private const string MSG008 = "Could not perform cancellation on item ";

		private const string MSG009 = "Could not delete item ";

		#endregion

		#region Properties

		/// <summary>
		///     This method will return an array of product ID & guids.  It is used
		///     by the dropdown list in the grid.
		/// </summary>
		/// <returns></returns>
		protected ArrayList EnumerateProducts
		{
			get
			{
				var productList = new ArrayList();
				ProductCollectionClass productCollection =
					FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
						products => products.EnumerateByType(this.Security, ProductType.ComponentProduct));

				foreach (ProductClass product in productCollection)
				{
					var valuePair = new DropdownValuePairDO
						{
							Text = product.ID,
							TextValue = product.MasterRecordGuid.ToString()
						};

					productList.Add(valuePair);
				}

				return productList;
			}
		}

		/// <summary>
		///     This method will enumerate the reserve levels and map the data to a
		///     dataview to match the grid.
		/// </summary>
		/// <returns></returns>
		private ICollection EnumerateReserveLevels
		{
			get
			{
				AccountingSite accountingSite =
					FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
						x => x.LoadSiteInfo(this.Security, this.Security.SiteGuid));

				var reserveLevelCollection = this.Session[COLLECTION] as ReserveLevelCollectionClass;

				var mapDataTable = new DataTable();

				mapDataTable.Columns.Add("ReserveLevelGuid", typeof(Guid));
				mapDataTable.Columns.Add("SiteGuid", typeof(Guid));
				mapDataTable.Columns.Add("MinimumLevel", typeof(float));
				mapDataTable.Columns.Add("WarningLevel", typeof(float));
				mapDataTable.Columns.Add("ProductGuid", typeof(Guid));
				mapDataTable.Columns.Add("ProductID", typeof(string));

				FMChannelHelper.MakeCall<IProducts>(
					products =>
						{
							if (reserveLevelCollection != null)
							{
								foreach (ReserveLevelClass reserveLevel in reserveLevelCollection)
								{
                                    AccountingUnitConversion converter = null;
                                    
                                    // New reserve levels won't have a product yet, so only get the converter if the product is provided
								    if (!string.IsNullOrEmpty(reserveLevel.ProductID))
								    {
								        ProductClass product = products.GetByID(this.Security, reserveLevel.ProductID);
								        converter = new AccountingUnitConversion(accountingSite.CurrentSite, product);
								    }

								    DataRow mapDataRow = mapDataTable.NewRow();

								    mapDataRow["ReserveLevelGuid"] = reserveLevel.IdentityGuid;
								    mapDataRow["SiteGuid"] = reserveLevel.SiteGuid;
                                    mapDataRow["MinimumLevel"] = converter != null ? reserveLevel.MinimumLevel * converter.VolumeConversionFactorFromSI : reserveLevel.MinimumLevel;
                                    mapDataRow["WarningLevel"] = converter != null ? reserveLevel.WarningLevel * converter.VolumeConversionFactorFromSI : reserveLevel.WarningLevel;
								    mapDataRow["ProductGuid"] = reserveLevel.ProductGuid;
								    mapDataRow["ProductID"] = reserveLevel.ProductID;

								    mapDataTable.Rows.Add(mapDataRow);
								}
							}
						});

				var reserveLevelDataView = new DataView(mapDataTable);
				return reserveLevelDataView;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group
		/// </param>
		/// <param name="options">
		/// Hardware key options
		/// </param>
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
                // Depends Upon Accounting
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }
            var items = new List<FMMenuItem>();

			// The Login Site must be a Site.
			if (siteGroup)
			{
				return null;
			}

			// Security checks from parent SitesForm()
			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}


			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.CONFIG_SITES_RESERVE_LEVELS, 
						RootMenuName = "Configuration", 
						CategoryName = "Sites", 
						ItemName = "Reserve Levels", 
						NavigateUrl = NAVIGATE_URL, 
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Methods

		protected void AddButton_Clicked(object sender, EventArgs e)
		{
			ReserveLevelCollectionClass reserveLevelCollection = (ReserveLevelCollectionClass)this.Session[COLLECTION];
			var reserveLevel = new ReserveLevelClass { SiteGuid = this.Security.SiteGuid, IdentityGuid = Guid.Empty };

			reserveLevelCollection.Add(reserveLevel);

			int numItems = reserveLevelCollection.Count;
			if (numItems > 0)
			{
				this.ReserveLevelDataGrid.CurrentPageIndex = (numItems - 1) / this.ReserveLevelDataGrid.PageSize;
				this.ReserveLevelDataGrid.EditItemIndex = (numItems - 1) % this.ReserveLevelDataGrid.PageSize;
			}

			this.EnableControls(false);

			this.UpdateView();
		}

		/// <summary>
		/// This method will handle the page index change event.
		/// </summary>
		/// <param name="source">
		/// </param>
		/// <param name="eventArgs">
		/// </param>
		protected void DataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs eventArgs)
		{
			// if we are editing do not allow a page change
			if (this.ReserveLevelDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.ReserveLevelDataGrid.CurrentPageIndex = eventArgs.NewPageIndex;
			this.UpdateView();
		}

		protected void EnableControls(bool bEnable)
		{
			this.Add1Btn.Enabled = this.Add2Btn.Enabled = bEnable && this.Security.HasRight(RIGHT.CONFIGURE_RESERVE_LEVEL);
			this.PageSizeDropdown.Enabled = bEnable;
		}

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.Security.HasRight(RIGHT.CONFIGURE_RESERVE_LEVEL))
				{
					this.Add1Btn.Enabled = true;
					this.Add2Btn.Enabled = true;
				}
				else
				{
					this.Add1Btn.Enabled = false;
					this.Add2Btn.Enabled = false;
				}

				if (this.Page.IsPostBack == false)
				{
					this.Session[COLLECTION] =
						FMChannelHelper.MakeCall<IReserveLevels, ReserveLevelCollectionClass>(x => x.Enumerate(this.Security));

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ReserveLevelDataGrid.EditCommand += this.ReserveLevel_EditCommand;
			this.ReserveLevelDataGrid.CancelCommand += this.ReserveLevel_CancelCommand;
			this.ReserveLevelDataGrid.UpdateCommand += this.ReserveLevel_UpdateCommand;
			this.ReserveLevelDataGrid.DeleteCommand += this.ReserveLevel_DeleteCommand;
			this.ReserveLevelDataGrid.PageIndexChanged += this.DataGrid_PageIndexChanged;
			this.ReserveLevelDataGrid.ItemDataBound += this.ReserveLevelDataGrid_ItemDataBound;
		}

		/// <summary>
		/// This method handles the item data bound for the grid. It will enable or disable the Edit
		///     and Delete buttons based on if the user has Configure Reserve Level rights.
		/// </summary>
		/// <param name="sender">
		/// </param>
		/// <param name="e">
		/// </param>
		private void ReserveLevelDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var deleteButton = (LinkButton)e.Item.FindControl("Fmdeletelinkbutton1");
			var editButton = (LinkButton)e.Item.FindControl("Fmeditlinkbutton1");

			if (deleteButton != null)
			{
				deleteButton.Enabled = this.Security.HasRight(RIGHT.CONFIGURE_RESERVE_LEVEL);
			}

			if (editButton != null)
			{
				editButton.Enabled = this.Security.HasRight(RIGHT.CONFIGURE_RESERVE_LEVEL);
			}

			// If in the process of editing, set the drop-down list item
			if (e.Item.ItemIndex != -1 && e.Item.ItemIndex == this.ReserveLevelDataGrid.EditItemIndex)
			{
				var reserveLevelCollection = (ReserveLevelCollectionClass)this.Session[COLLECTION];
				ReserveLevelClass reserveLevel = reserveLevelCollection[e.Item.DataSetIndex];
				var productDropDownList = (DropDownList)e.Item.FindControl("ProductDropDownList");
				ListItem item = productDropDownList.Items.FindByValue(reserveLevel.ProductGuid.ToString());
				if (item != null)
				{
					productDropDownList.SelectedIndex = productDropDownList.Items.IndexOf(item);
				}
			}
		}

		private void ReserveLevel_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			const int ItemIndex = -99;

			try
			{
				this.ReserveLevelDataGrid.EditItemIndex = -1;
				
				this.Session[COLLECTION] =
					FMChannelHelper.MakeCall<IReserveLevels, ReserveLevelCollectionClass>(x => x.Enumerate(this.Security));

				if ((this.ReserveLevelDataGrid.Items.Count == 1) && (this.ReserveLevelDataGrid.CurrentPageIndex > 0))
				{
					this.ReserveLevelDataGrid.CurrentPageIndex--;
				}

				this.EnableControls(true);
				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(new Exception(MSG008 + " #" + ItemIndex.ToString(CultureInfo.InvariantCulture) + ". " + ex.Message));
			}
		}

		private void ReserveLevel_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			const int ItemIndex = -99;

			try
			{
				var identityGuidLabel = e.Item.FindControl("ReserveLevelGuidLabel") as Label;

				if (identityGuidLabel != null)
				{
					Guid identityGuid = Guid.Parse(identityGuidLabel.Text);

					// Handle the situation where another row is being edited while this one is being deleted
					if (this.ReserveLevelDataGrid.EditItemIndex == e.Item.ItemIndex)
					{
						this.ReserveLevelDataGrid.EditItemIndex = -1;
						this.EnableControls(true);
					}
					else if (this.ReserveLevelDataGrid.EditItemIndex > e.Item.ItemIndex)
					{
						this.ReserveLevelDataGrid.EditItemIndex--;
					}

					ReserveLevelCollectionClass reserveLevelCollection = null;

					FMChannelHelper.MakeCall<IReserveLevels>(
						reserveLevels =>
							{
								if (identityGuid != Guid.Empty)
								{
									reserveLevels.Purge(this.Security, identityGuid);
								}

								reserveLevelCollection = reserveLevels.Enumerate(this.Security);

								this.Session[COLLECTION] = reserveLevelCollection;
							});

					this.UpdateView();

					if ((this.ReserveLevelDataGrid.CurrentPageIndex > 0)
					    && (this.ReserveLevelDataGrid.CurrentPageIndex * this.ReserveLevelDataGrid.PageSize
					        >= reserveLevelCollection.Count))
					{
						this.ReserveLevelDataGrid.CurrentPageIndex--;
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(new Exception(MSG009 + " #" + ItemIndex.ToString(CultureInfo.InvariantCulture) + ". " + ex.Message));
			}
		}

		private void ReserveLevel_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.EnableControls(false);
			this.ReserveLevelDataGrid.EditItemIndex = e.Item.ItemIndex;

			this.UpdateView();

			// Set selected product
			var l = e.Item.FindControl("ProductLabel") as Label;
			if (l != null)
			{
				string txt = l.Text;
				DataGridItem dg = this.ReserveLevelDataGrid.Items[e.Item.ItemIndex];
				var dd = dg.FindControl("ProductDropDownList") as DropDownList;
				if (dd != null)
				{
					foreach (ListItem item in dd.Items)
					{
						if (txt == item.Text)
						{
							item.Selected = true;
							break;
						}
					}
				}
			}
		}

		private void ReserveLevel_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			int itemIndex = -99;
			bool errorFlag = false;

			try
			{
				var reserveLevelCollection = (ReserveLevelCollectionClass)this.Session[COLLECTION];

				var productDropdownList = (DropDownList)e.Item.FindControl("ProductDropDownList");
				var minLevelTb = (TextBox)e.Item.FindControl("MinLevelTextBox");
				var warningLevelTb = (TextBox)e.Item.FindControl("WarningLevelTextBox");

				var oldReserveLevel = new ReserveLevelClass();
				int inx = this.ReserveLevelDataGrid.CurrentPageIndex * this.ReserveLevelDataGrid.PageSize
				          + this.ReserveLevelDataGrid.EditItemIndex;
				itemIndex = inx;

				ReserveLevelClass reserveLevel = reserveLevelCollection[inx];

				oldReserveLevel.MinimumLevel = reserveLevel.MinimumLevel;
				oldReserveLevel.WarningLevel = reserveLevel.WarningLevel;
				oldReserveLevel.ProductGuid = reserveLevel.ProductGuid;
				oldReserveLevel.ProductID = reserveLevel.ProductID;

				if (productDropdownList != null && productDropdownList.SelectedIndex > -1)
				{
					try
					{
						reserveLevel.ProductGuid = Guid.Parse(productDropdownList.SelectedItem.Value);
						reserveLevel.ProductID = productDropdownList.SelectedItem.Text;
					}
					catch (InvalidCastException)
					{
						errorFlag = true;
					}
				}
				else
				{
					errorFlag = true;
					this.ErrorHandler(new Exception("Product " + MSG006));
				}

				// Convert the minimum reserve level to float prior to saving.
				if (minLevelTb != null)
				{
					try
					{
						reserveLevel.MinimumLevel = Convert.ToDouble(minLevelTb.Text);
					}
					catch (InvalidCastException)
					{
						errorFlag = true;
						this.ErrorHandler(new Exception(MSG001));
					}
				}

				if (warningLevelTb != null)
				{
					try
					{
						reserveLevel.WarningLevel = Convert.ToDouble(warningLevelTb.Text);
					}
					catch (InvalidCastException)
					{
						errorFlag = true;
						this.ErrorHandler(new Exception(MSG002));
					}
				}

				if (reserveLevel.MinimumLevel <= 0)
				{
					// Error if negative
					errorFlag = true;
					this.ErrorHandler(new Exception(MSG003));
				}

				if (reserveLevel.WarningLevel <= 0)
				{
					// Error if negative
					errorFlag = true;
					this.ErrorHandler(new Exception(MSG004));
				}

				if (reserveLevel.MinimumLevel > reserveLevel.WarningLevel)
				{
					// Error if minimum level is greater than warning level.
					errorFlag = true;
					this.ErrorHandler(new Exception(MSG005));
				}

				// On error, set the original values back.
				if (errorFlag)
				{
					if (reserveLevel.IdentityGuid != Guid.Empty)
					{
						reserveLevel.MinimumLevel = oldReserveLevel.MinimumLevel;
						reserveLevel.WarningLevel = oldReserveLevel.WarningLevel;
						reserveLevel.ProductGuid = oldReserveLevel.ProductGuid;
						reserveLevel.ProductID = oldReserveLevel.ProductID;
					}
					else
					{
						AccountingSite accountingSite =
							FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
								x => x.LoadSiteInfo(this.Security, this.Security.IdentityGuid));

						ProductClass product = null;
						
						FMChannelHelper.MakeCall<IProducts>(
							products =>
								{
									product = products.Get(
										this.Security, products.GetIdentityGuid(this.Security, reserveLevel.ProductID));
								});

						var converter = new AccountingUnitConversion(accountingSite.CurrentSite, product);
						reserveLevel.MinimumLevel *= converter.VolumeConversionFactorToSI;
						reserveLevel.WarningLevel *= converter.VolumeConversionFactorToSI;
					}
				}
				else
				{
					AccountingSite accountingSite =
						FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
							x => x.LoadSiteInfo(this.Security, this.Security.IdentityGuid));

					ProductClass product = null;
					
					FMChannelHelper.MakeCall<IProducts>(
						products =>
							{
								product = products.Get(
									this.Security, products.GetIdentityGuid(this.Security, reserveLevel.ProductID));
							});

					var converter = new AccountingUnitConversion(accountingSite.CurrentSite, product);
					reserveLevel.MinimumLevel *= converter.VolumeConversionFactorToSI;
					reserveLevel.WarningLevel *= converter.VolumeConversionFactorToSI;

					FMChannelHelper.MakeCall<IReserveLevels>(
						reserveLevels =>
							{
								if (reserveLevel.IdentityGuid == Guid.Empty)
								{
									reserveLevel.SiteGuid = this.Security.SiteGuid;
									reserveLevel.IdentityGuid = reserveLevels.Add(this.Security, reserveLevel);
								}
								else
								{
									reserveLevels.Modify(this.Security, reserveLevel);
								}

								this.ReserveLevelDataGrid.EditItemIndex = -1;

								this.Session[COLLECTION] = reserveLevels.Enumerate(this.Security);
							});
				}

				this.EnableControls(true);
				this.UpdateView();

				// Set selected product
				DataGridItem dg = this.ReserveLevelDataGrid.Items[e.Item.ItemIndex];
				var dd = dg.FindControl("ProductDropDownList") as DropDownList;
				if (dd != null)
				{
					foreach (ListItem item in dd.Items)
					{
						if (reserveLevel.ProductID == item.Text)
						{
							item.Selected = true;
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(new Exception(MSG007 + " #" + itemIndex.ToString(CultureInfo.InvariantCulture) + ". " + ex.Message));
			}
		}

		/// <summary>
		///     This method updates the grid with new information.
		/// </summary>
		private void UpdateView()
		{
			ICollection dataCollection = this.EnumerateReserveLevels;
			if (this.PageSizeDropdown != null)
			{
				this.PageSizeDropdown.SetPageSize(this.ReserveLevelDataGrid, dataCollection.Count);
			}

			this.ReserveLevelDataGrid.DataSource = dataCollection;
			this.ReserveLevelDataGrid.DataBind();
		}

		#endregion
	}
}
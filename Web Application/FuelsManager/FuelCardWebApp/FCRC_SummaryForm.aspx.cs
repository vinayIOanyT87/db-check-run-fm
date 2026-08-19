// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FCRC_SummaryForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for FCRC_SummaryForm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FuelCardWebApp
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;
    using FMBusinessObjects.UtilityObjects;

    using FuelsManager.FMWebApp;

    using global::FMWebApp;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;
    using System.Web.UI.WebControls;

	/// <summary>
	/// Summary description for Fuel Common Request Configuration Summary Page.
	/// </summary>
	public partial class FCRC_SummaryForm : FMAutoSubmitFormBase, IEntityDiscovery, IMenuDiscovery
	{
		public const string NavigateUrl = "../FuelCardWebApp/FCRC_SummaryForm.aspx";

		#region Protected Attributes
		protected string AllText = "{All}";
		#endregion

		#region Entity related methods and properties
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.FUEL_CARD;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IFuelCards);
			}
		}

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass inSecurity, 
																			ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			FuelCardCollectionClass fuelCardCollection = FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(
																							x => x.EnumerateFuelCards(inSecurity));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (FuelCardClass fuelCard in fuelCardCollection)
			{
				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (inSecurity.SiteGuid == fuelCard.SiteGuid)
					{
						continue;
					}

					if (inSecurity.LoginSiteGuid != fuelCard.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (inSecurity.SiteGuid != fuelCard.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(fuelCard);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			FMChannelHelper.MakeCall<IFuelCards>(
				fuelCards =>
					{
						FuelCardClass fuelCard = fuelCards.Get(security, guid, true);
						fuelCard.SiteGuid = siteGuid;
						fuelCards.Modify(security, fuelCard);
					});
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IFuelCards, Guid>(x => x.GetIdentityGuid(security, id));
		}
		#endregion

		#region Public methods
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
                // Depends Upon Accounting
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }
            var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_FUEL_CARD_DATA) && !security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
			{
				return null;
			}


			items.Add(new FMMenuItem {
										MenuItemType		= FMMenuItemType.CONFIG_OTHER_FUEL_CARDS,
										RootMenuName		= "Configuration",
										CategoryName		= "Other",
										ItemName			= "Fuel Cards",
										NavigateUrl			= NavigateUrl,
										ApplyDataDictionary = ApplyDataDictionary.Apply
			});

			return items;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will update the view (grid) with the most current data. If there is a search,
		/// the grid is set to display the search item.
		/// </summary>
		private void UpdateView()
		{
			var limits = new EnumerationLimits();
			int limit  = limits.GetLimit(EnumerationLimits.EnumerationOptions.FUEL_CARD);

			Guid managerGuid	= Guid.Empty;
			Guid ownerGuid		= Guid.Empty;
			Guid shipperGuid	= Guid.Empty;
			Guid billToGuid		= Guid.Empty;
			Guid shipToGuid		= Guid.Empty;

			FMChannelHelper.MakeCall<ICompanies>(
				companies =>
				{
					managerGuid = companies.GetIdentityGuid(this.Security, this.ManagerSelect.Text);
					ownerGuid	= companies.GetIdentityGuid(this.Security, this.OwnerSelect.Text);
					shipperGuid = companies.GetIdentityGuid(this.Security, this.ShipperSelect.Text);
					billToGuid	= companies.GetIdentityGuid(this.Security, this.BillToSelect.Text);
					shipToGuid	= companies.GetIdentityGuid(this.Security, this.ShipToSelect.Text);
				});

		    Guid fuelCardTypeApplicationStringGuid;

		    Guid.TryParse(this.FuelCardTypeDropDownList.SelectedValue, out fuelCardTypeApplicationStringGuid);

			// Determine if the user entered in a filter to narrow the equip list. If so,
			// then call the method in equipments that will use the filter. Otherwise, use the
			// original method to get equipments.
			DataSet fuelCardDataSet = FMChannelHelper.MakeCall<IFuelCards, DataSet>(
																	x =>
																	x.EnumerateFuelCardsForSummary(
																									this.Security, 
																									managerGuid, 
																									ownerGuid, 
																									shipperGuid, 
																									billToGuid, 
																									shipToGuid, 
                                                                                                    fuelCardTypeApplicationStringGuid,
																									this.FindTextBox.Text, 
																									this.TransientCheckBox.Checked));

			var fuelCardDataTable = new DataTable();

			fuelCardDataTable.Columns.Add("SiteGuid", typeof(Guid));
			fuelCardDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			fuelCardDataTable.Columns.Add("ID", typeof(string));
			fuelCardDataTable.Columns.Add("Manager", typeof(string));
			fuelCardDataTable.Columns.Add("ManagerTip", typeof(string));
			fuelCardDataTable.Columns.Add("Owner", typeof(string));
			fuelCardDataTable.Columns.Add("OwnerTip", typeof(string));
			fuelCardDataTable.Columns.Add("Shipper", typeof(string));
			fuelCardDataTable.Columns.Add("ShipperTip", typeof(string));
			fuelCardDataTable.Columns.Add("BillTo", typeof(string));
			fuelCardDataTable.Columns.Add("BillToTip", typeof(string));
			fuelCardDataTable.Columns.Add("ShipTo", typeof(string));
			fuelCardDataTable.Columns.Add("ShipToTip", typeof(string));
			fuelCardDataTable.Columns.Add("Provider", typeof(string));
			fuelCardDataTable.Columns.Add("Status", typeof(string));
            fuelCardDataTable.Columns.Add("FuelCardTypeApplicationStringID", typeof(string));

			int rowCount = 0;

			if (fuelCardDataSet != null && fuelCardDataSet.Tables.Count > 0 && fuelCardDataSet.Tables[0].Rows.Count > 0)
			{
				rowCount = fuelCardDataSet.Tables[0].Rows.Count;

				foreach (DataRow row in fuelCardDataSet.Tables[0].Rows)
				{
					DataRow fuelCardDataRow = fuelCardDataTable.NewRow();

					fuelCardDataRow["SiteGuid"]		= row.IsNull("SiteGuid") ? Guid.Empty : (Guid) row["SiteGuid"];
					fuelCardDataRow["IdentityGuid"] = row.IsNull("FuelCardGuid") ? Guid.Empty : (Guid) row["FuelCardGuid"];
					fuelCardDataRow["ID"]			= row.IsNull("ID") ? string.Empty : row["ID"];
					fuelCardDataRow["Manager"]		= HttpUtility.HtmlEncode(row.IsNull("ManagerID") ? string.Empty : row["ManagerID"]);
					fuelCardDataRow["ManagerTip"]	= HttpUtility.HtmlEncode(this.GetToolTip(row, "Manager"));
					fuelCardDataRow["Owner"]		= HttpUtility.HtmlEncode(row.IsNull("OwnerID") ? string.Empty : row["OwnerID"]);
					fuelCardDataRow["OwnerTip"]		= HttpUtility.HtmlEncode(this.GetToolTip(row, "Owner"));
					fuelCardDataRow["Shipper"]		= HttpUtility.HtmlEncode(row.IsNull("ShipperID") ? string.Empty : row["ShipperID"]);
					fuelCardDataRow["ShipperTip"]	= HttpUtility.HtmlEncode(this.GetToolTip(row, "Shipper"));
					fuelCardDataRow["BillTo"]		= HttpUtility.HtmlEncode(row.IsNull("BillToID") ? string.Empty : row["BillToID"]);
					fuelCardDataRow["BillToTip"]	= HttpUtility.HtmlEncode(this.GetToolTip(row, "BillTo"));
					fuelCardDataRow["ShipTo"]		= HttpUtility.HtmlEncode(row.IsNull("ShipToID") ? string.Empty : row["ShipToID"]);
					fuelCardDataRow["ShipToTip"]	= HttpUtility.HtmlEncode(this.GetToolTip(row, "ShipTo"));
					fuelCardDataRow["Provider"]		= row.IsNull("Provider") ? string.Empty : row["Provider"];
					fuelCardDataRow["Status"]		= this.GetFuelCardStatus(row);
                    fuelCardDataRow["FuelCardTypeApplicationStringID"] = row.IsNull("FuelCardTypeApplicationStringID") ? string.Empty : row["FuelCardTypeApplicationStringID"];
					fuelCardDataTable.Rows.Add(fuelCardDataRow);
				}
			}

			var fuelCardDataView = new DataView(fuelCardDataTable);

			if (fuelCardDataView.Count >= limit && limit > 0)
			{
				this.lblWarning.Text = string.Format("Results limited to first {0} records.  Use filters to narrow search.", limit);
				this.lblWarning.Visible = true;
			}
			else
			{
				this.lblWarning.Visible = false;
			}

			this.FuelCardSummaryPageSizeDropDown.SetPageSize(this.fuelCardsDataGrid, rowCount);
			this.fuelCardsDataGrid.DataSource = fuelCardDataView;
			this.fuelCardsDataGrid.DataBind();
		}

		/// <summary>
		/// This method will build a tool tip for an given company role.
		/// </summary>
		/// <param name="row">The row to pull the tool tip information.</param>
		/// <param name="columnName">The Root column name in the dataset.</param>
		/// <returns>Returns a tool tip.</returns>
		private string GetToolTip(DataRow row, string columnName)
		{
			string toolTip;

			string name		= columnName + "Name";
			string id		= columnName + "ID";
			string address	= columnName + "Address";
			string city		= columnName + "City";
			string state	= columnName + "State";


			string entityName		= row.IsNull(name) ? string.Empty : (string) row[name];
			string entityId			= row.IsNull(id) ? string.Empty : (string) row[id];
			string entityAddress	= row.IsNull(address) ? string.Empty : (string) row[address];
			string entityCity		= row.IsNull(city) ? string.Empty : (string) row[city];
			string entityState		= row.IsNull(state) ? string.Empty : (string) row[state];

			if (string.IsNullOrEmpty(entityName) == false)
			{
				toolTip = entityName;
			}
			else
			{
				toolTip = entityId;
			}

			if (string.IsNullOrEmpty(entityAddress) == false)
			{
				toolTip += ", " + entityAddress;
			}
			if (string.IsNullOrEmpty(entityCity) == false)
			{
				toolTip += ", " + entityCity;
			}
			if (string.IsNullOrEmpty(entityState) == false)
			{
				toolTip += ", " + entityState;
			}

			return toolTip;
		}

		/// <summary>
		/// This method returns the fuel card status name from the data row.
		/// </summary>
		/// <param name="row">Row that contains the status value.</param>
		/// <returns>Returns status value.</returns>
		private string GetFuelCardStatus(DataRow row)
		{
			var status = DataObject.getValue<FuelCardClass.Statuses>(row["ActivationStatus"], FuelCardClass.Statuses.ACTIVE);
			return FuelCardClass.STATUS_NAMES[(int) status];
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// This method handles the page load event.
		/// </summary>
		/// <param name="sender">The sending object.</param>
		/// <param name="e">Event arguments.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
              
				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

                    this.FuelCardTypeDropDownList.DataBind();

					var context = this.Session["FuelCardSummaryContext"] as FuelCardSummaryContext;
					if (context == null)
					{
						this.AllText = this.GetTranslatedText("{All}");
						context = new FuelCardSummaryContext(this.AllText);
					}

					this.ManagerSelect.Text			= context.ManagerID;
					this.OwnerSelect.Text			= context.OwnerID;
					this.ShipperSelect.Text			= context.ShipperID;
					this.BillToSelect.Text			= context.BillToID;
					this.ShipToSelect.Text			= context.ShipToID;
					this.FindTextBox.Text			= context.FindText;
					this.TransientCheckBox.Checked	= context.TransientFlag;
				    if (this.FuelCardTypeDropDownList.Items.FindByValue(context.FuelCardTypeApplicationStringGuid.ToString()) != null)
				    {
				        this.FuelCardTypeDropDownList.SelectedValue = context.FuelCardTypeApplicationStringGuid.ToString();
				    }

					this.Session["FuelCardSummaryContext"] = context;

					if (this.Session["FuelCardPageIndex"] != null)
					{
						this.fuelCardsDataGrid.CurrentPageIndex = (int) this.Session["FuelCardPageIndex"];
						this.Session.Remove("FuelCardPageIndex");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the PageSizeDropDown control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			try
			{
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the refresh button event.
		/// </summary>
		/// <param name="sender">The sending object.</param>
		/// <param name="e">Event agruments.</param>
		protected void RefreshButtonClick(object sender, EventArgs e)
		{
			this.FindBtnOnClick(sender, e);
		}

		/// <summary>
		/// This method handles the fuel card data grid edit command event.
		/// </summary>
		/// <param name="source">The source grid object.</param>
		/// <param name="e">Event arguments.</param>
		protected void FuelCardsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.Session.Remove(PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST);

				TableCell identityGuidCell = e.Item.Cells[2];//bds
				string identityGuid = identityGuidCell.Text;

				FuelCardClass fuelCard =
					FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(x => x.Get(this.Security, Guid.Parse(identityGuid), true));

				var fuelCardArrayList = new ArrayList { fuelCard };
				this.Session[PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST] = fuelCardArrayList;

				this.Session["FuelCardPageIndex"] = this.fuelCardsDataGrid.CurrentPageIndex;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("FCRC_DetailForm.aspx");
		}

		/// <summary>
		/// This method handles the fuel card data grid delete command event.
		/// </summary>
		/// <param name="source">The source grid object.</param>
		/// <param name="e">Event arguments.</param>
		protected void FuelCardsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.GetSecurity();

				// Get identityGuid
				TableCell identityGuidCell = e.Item.Cells[2];//bds

				FuelCardClass fuelCard =
					FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(x => x.Get(this.Security, Guid.Parse(identityGuidCell.Text), true));

				if (fuelCard != null)
				{
					//Check if fueld card has been inactive for more than two years. 
					//Do not delete if inactive and less than two years.
					DateTimeOffset statusChangeDate = fuelCard.StatusModifiedDate;
					DateTimeOffset okayToDeleteDate = statusChangeDate.AddYears(2);

					if (FuelCardClass.Statuses.INACTIVE != fuelCard.Status || DateTimeOffset.Now > okayToDeleteDate)
					{
						FMChannelHelper.MakeCall<IFuelCards>(x => x.Purge(this.Security, Guid.Parse(identityGuidCell.Text)));

						this.fuelCardsDataGrid.SelectedIndex = -1;
						this.Session.Remove("IdentityGuid");

						if (this.fuelCardsDataGrid.Items.Count == 1 && this.fuelCardsDataGrid.CurrentPageIndex > 0)
						{
							this.fuelCardsDataGrid.CurrentPageIndex--;
						}

						this.UpdateView();
					}
					else
					{
						throw new Exception("Fuel Card inactive for less than 2 years cannot be deleted.");
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the Add button event.
		/// </summary>
		/// <param name="sender">The sending object.</param>
		/// <param name="e">Event agruments.</param>
		protected void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove(PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST);

			var fuelCard = new FuelCardClass { SiteGuid = this.Security.SiteGuid };

			var fuelCardArrayList = new ArrayList { fuelCard };
			this.Session[PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST] = fuelCardArrayList;

			this.Session["FuelCardPageIndex"] = this.fuelCardsDataGrid.CurrentPageIndex;
			
			this.Redirect("FCRC_DetailForm.aspx");
		}

		/// <summary>
		/// Handles the PageIndexChanged event of the FuelCardsDataGrid control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="DataGridPageChangedEventArgs" /> instance containing the event data.</param>
		protected void FuelCardsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.fuelCardsDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.fuelCardsDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the grid's item data bound event.
		/// </summary>
		/// <param name="sender">The sending object.</param>
		/// <param name="e">Event agruments.</param>
		protected void FuelCardsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex != -1)
			{
				var deleteButton = (LinkButton)e.Item.FindControl("Fmdeletelinkbutton1");

				if (deleteButton != null)
				{
					TableCell siteGuidCell = e.Item.Cells[1];//bds

					if (!this.Security.HasRight(RIGHT.MODIFY_FUEL_CARD_DATA) || this.Security.SiteGuid != Guid.Parse(siteGuidCell.Text))
					{
						deleteButton.Enabled = false;
					}
				}
			}
		}

		/// <summary>
		/// This method is called when the find button is pressed. It will retrieve data from the find
		/// text box and set the search string. If there is no data, then the search string is set to null.
		/// </summary>
		/// <param name="sender">The sending object.</param>
		/// <param name="e">Event agruments.</param>
		protected void FindBtnOnClick(object sender, EventArgs e)
		{
			var context = this.Session["FuelCardSummaryContext"] as FuelCardSummaryContext;

			if (context != null)
			{
				context.ManagerID		= this.ManagerSelect.Text;
				context.OwnerID			= this.OwnerSelect.Text;
				context.ShipperID		= this.ShipperSelect.Text;
				context.BillToID		= this.BillToSelect.Text;
				context.ShipToID		= this.ShipToSelect.Text;
				context.FindText		= this.FindTextBox.Text;
				context.TransientFlag	= this.TransientCheckBox.Checked;

			    Guid fuelCardTypeApplicationStringGuid;

                if (this.FuelCardTypeDropDownList.SelectedValue != null 
					&& Guid.TryParse(this.FuelCardTypeDropDownList.SelectedValue, out fuelCardTypeApplicationStringGuid))
			    {
                    context.FuelCardTypeApplicationStringGuid = fuelCardTypeApplicationStringGuid;
			    }
			}

			// Update the page with the new contents.
			this.fuelCardsDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		/// <summary>
		/// This method is called when the show all button is pressed. It will set the search string to null
		/// indicating that we do not want to use the filter on finding companies.  In addition, the find
		/// text box is cleared.
		/// </summary>
		/// <param name="sender">The sending object.</param>
		/// <param name="e">Event agruments.</param>
		protected void ShowAllBtnOnClick(object sender, EventArgs e)
		{
			this.ManagerSelect.Text = this.AllText;
			this.OwnerSelect.Text	= this.AllText;
			this.ShipperSelect.Text = this.AllText;
			this.BillToSelect.Text	= this.AllText;
			this.ShipToSelect.Text	= this.AllText;
			this.FindTextBox.Text	= string.Empty;

			var context = this.Session["FuelCardSummaryContext"] as FuelCardSummaryContext;

			if (context != null)
			{
				context.ManagerID		= this.ManagerSelect.Text;
				context.OwnerID			= this.OwnerSelect.Text;
				context.ShipperID		= this.ShipperSelect.Text;
				context.BillToID		= this.BillToSelect.Text;
				context.ShipToID		= this.ShipToSelect.Text;
				context.FindText		= this.FindTextBox.Text;
				context.TransientFlag	= this.TransientCheckBox.Checked;

                Guid fuelCardTypeApplicationStringGuid;

                if (this.FuelCardTypeDropDownList.SelectedValue != null 
					&& Guid.TryParse(this.FuelCardTypeDropDownList.SelectedValue, out fuelCardTypeApplicationStringGuid))
                {
                    context.FuelCardTypeApplicationStringGuid = fuelCardTypeApplicationStringGuid;
                }
			}

			this.fuelCardsDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}
		#endregion

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
			this.AddButton2.Command					+= new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);
			this.AddButton.Command					+= new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);
			this.fuelCardsDataGrid.EditCommand		+= new System.Web.UI.WebControls.DataGridCommandEventHandler(this.FuelCardsDataGridEditCommand);
			this.fuelCardsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.FuelCardsDataGridPageIndexChanged);
			this.fuelCardsDataGrid.DeleteCommand	+= new System.Web.UI.WebControls.DataGridCommandEventHandler(this.FuelCardsDataGridDeleteCommand);
			this.fuelCardsDataGrid.ItemDataBound	+= new System.Web.UI.WebControls.DataGridItemEventHandler(this.FuelCardsDataGridItemDataBound);
			this.RefreshButton.Click				+= new EventHandler(this.RefreshButtonClick);

			var limits = new EnumerationLimits();
			int pageLimit = limits.GetLimit(EnumerationLimits.EnumerationOptions.FUEL_CARD);
			this.FuelCardSummaryPageSizeDropDown.SetLimit(pageLimit);
			this.fuelCardsDataGrid.PageSize = pageLimit;
		}
		#endregion

		#region Fuel Card Summary Context Class
		/// <summary>
		/// Contains the fuel card summary context information.
		/// </summary>
		[Serializable]
		class FuelCardSummaryContext
		{
			public string ManagerID { get; set; }
			public string OwnerID { get; set; }
			public string ShipperID { get; set; }
			public string BillToID { get; set; }
			public string ShipToID { get; set; }
			public string FindText { get; set; }
			public bool TransientFlag { get; set; }

            /// <summary>
            /// The Fuel Card Type the user last searched for
            /// </summary>
		    public Guid FuelCardTypeApplicationStringGuid;

			public FuelCardSummaryContext(string allText)
			{
				this.ManagerID		= allText;
				this.OwnerID		= allText;
				this.ShipperID		= allText;
				this.BillToID		= allText;
				this.ShipToID		= allText;
				this.FindText		= string.Empty;
				this.TransientFlag	= false;
			    this.FuelCardTypeApplicationStringGuid = Guid.Empty;
			}
		}
		#endregion

	    /// <summary>
	    /// Get the fuel card types to display in the fuel card type drop down. 
	    /// In addition to the ones configured, display an "All" selection
	    /// </summary>
	    /// <returns>
        /// The fuel card types to display in the fuel card type drop down. 
	    /// </returns>
	    protected ICollection EnumerateFuelCardTypes()
        {
            ApplicationStringCollectionClass fuelCardTypes = new ApplicationStringCollectionClass();

            ApplicationStringClass allFuelCardType = new ApplicationStringClass
                                                                     {
                                                                         Type = STRING_TYPE.FUEL_CARD_TYPE,
                                                                         ID = this.AllText,
                                                                         IdentityGuid = Guid.Empty
                                                                     };
            fuelCardTypes.Add(allFuelCardType);

            ApplicationStringCollectionClass fuelCardTypeCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
                                                         applicationStrings =>
                                                         applicationStrings.EnumerateByType(this.Security, STRING_TYPE.FUEL_CARD_TYPE));

            fuelCardTypes.AddRange(fuelCardTypeCollection);

            return fuelCardTypes;
        }
	}
}

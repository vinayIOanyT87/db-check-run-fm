// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandingOfferPriceForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the StandingOfferPriceForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FinanceWebApp
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FMControls;

    using FuelsManager.Accounting;

    /// <summary>
	///    Summary description for WebForm1.
	/// </summary>
	public partial class StandingOfferPriceForm : AccountingWebFormView, IEntityDiscovery
	{
		#region Constants and Fields

		private const string Msg001 = "Must enter Effective End Date";

		private const string Msg002 = "Must enter Effective Start Date";

		private const string Msg003 = "Start date must be older than end date";

		private const string Msg004 = "Could not perform cancellation on item";

		private const string Msg005 = "Value must be numeric";

		private const string Msg006 = "Must select an effective date";

		private const string Msg007 = "Must select an expiration date";

		private const string Msg008 = "Effective date must be before the expiration date";

		private const string Msg009 = "Must select a supplier";

		private const string Msg010 = "Must select a fuel type";

		private const string Msg011 = "Could not update item";

		private const string Msg012 = "Could not delete item";

		private const string Msg013 = "Must enter Lower Bound value";

		private const string Msg014 = "Lower Bound value must be numeric";

		private const string Msg015 = "Must enter Upper Bound value";

		private const string Msg016 = "Upper Bound value must be numeric";

		private const string Msg017 = "Lower Bound must be less than or equal to Upper Bound";

		private const string Msg018 =
			"Lower/Upper bound cannot overlap an existing price list entry that has "
			+ "the same supplier, fuel type, effective date and expiration date.";

		private AccountingSite accountingSite;

		#endregion

		#region Explicit Interface Properties

		/// <summary>
		///    This property returns true when the entity is assignable. It is
		///    hard coded to return assignable (true).
		/// </summary>
		bool IEntityDiscovery.EntityAssignable => false;

        /// <summary>
		///    This property returns the type of the Standing Offers (aka Price List) business class.
		/// </summary>
		Type IEntityDiscovery.EntityEngineType => typeof(IStandingOffers);

        ENTITY_TYPE IEntityDiscovery.EntityType => ENTITY_TYPE.STANDING_OFFER;

        #endregion

		#region Explicit Interface Methods

		/// <summary>
		///    This method will return a collection of entity to site mapping for Standing Offers (aka Price List).
		/// </summary>
		/// <param name="securityParam"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass securityParam, ENTITY_ASSIGNMENT_TYPE type)
		{
		    var standingOfferCollection = FMChannelHelper.MakeCall<IStandingOffers, StandingOfferCollectionClass>(
		        x =>
		            x.Enumerate(securityParam)
		        );

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (StandingOfferClass standingOffer in standingOfferCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (securityParam.SiteGuid == standingOffer.SiteGuid)
					{
						continue;
					}

					if (securityParam.LoginSiteGuid != standingOffer.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (securityParam.SiteGuid != standingOffer.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(standingOffer);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass securityParam, string id)
		{
			return FMChannelHelper.MakeCall<IStandingOffers, Guid>(
																	 x =>
																	 x.GetIdentityGuid(securityParam,id)
																);
		}

		/// <summary>
		///    This method sets the securityParam and site guid for the price list entry (aka standing offer) via
		///    entity discovery.
		/// </summary>
		/// <param name="securityParam"></param>
		/// <param name="guid"></param>
		/// <param name="siteGuid"></param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass securityParam, Guid guid, Guid siteGuid)
		{
			StandingOfferClass standingOffer = FMChannelHelper.MakeCall<IStandingOffers, StandingOfferClass>(
																	 x =>
																	 x.Get(securityParam,guid)
																);

			standingOffer.SiteGuid = siteGuid;

			FMChannelHelper.MakeCall<IStandingOffers>(
																	 x =>
																	 x.Modify(securityParam,standingOffer)
																);
		}

		#endregion

		#region Methods

		/// <summary>
		///    This is a pass through method to add an item to the grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddBtn1OnClick(object sender, EventArgs e)
		{
			try
			{
				this.AddOnClick();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This is a pass through method to add an item to the grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddBtn2OnClick(object sender, EventArgs e)
		{
			try
			{
				this.AddOnClick();
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

		/// <summary>
		///    This method will update the view when the grid size is selected.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		/// <summary>
		///    This is the main entry point into the page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			// Get site information.
			this.accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																	 x =>
																	 x.LoadSiteInfo(this.security,this.security.SiteGuid)
																);

			this.accountingSite.GetUserCompanies = false;

			// Ensure the user has permissions.
			this.CheckPermissions();

			if (this.Page.IsPostBack == false)
			{
				this.UpdateHeaderView();
				this.RefreshBtnOnClick(null, null);
			}
		}

		/// <summary>
		///    This method will persist the header view when the page is unloaded.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Unload(object sender, EventArgs e)
		{
			this.PersistHeaderView();
		}

		/// <summary>
		///    This method will refresh the grid according to the supplier, product, and effective
		///    date filters.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void RefreshBtnOnClick(object sender, EventArgs e)
		{
			try
			{
				bool datesAreValid = this.ValidateDates();

				var filter = new StandingOfferFilterClass();

				if (!string.IsNullOrEmpty(this.ProductTextBox.Text))
				{
					if (this.ProductTextBox.Text.StartsWith("<") == false)
					{
						filter.ProductGuid = this.GetProductGuid(this.ProductTextBox.Text);
					}
				}

				if (!string.IsNullOrEmpty(this.SupplierTextBox.Text))
				{
					if (this.SupplierTextBox.Text.StartsWith("<") == false)
					{
						filter.SupplierGuid = this.GetCompanyGuid(this.SupplierTextBox.Text);
					}
				}

				if (this.LocationSelect.SelectedValue != Guid.Empty.ToString())
				{
					filter.LocationGuid = Guid.Parse(this.LocationSelect.SelectedValue);
				}

				if (!string.IsNullOrEmpty(this.EffectiveDateDate.Text)
				    && (!string.IsNullOrEmpty(this.EndDateTextBox.Text)) && datesAreValid)
				{
					this.Session.Add(PageSessionKeyConstants.SOP_EFFECTIVE_DATE, this.EffectiveDateDate.Text);
					this.Session.Add(PageSessionKeyConstants.SOP_EFF_END_DATE, this.EndDateTextBox.Text);

					filter.EffectiveStartDate = this.EffectiveDateDate.CurrentValue;
					filter.EffectiveEndDate = this.EndDateTextBox.CurrentValue;
				}

				if (!string.IsNullOrEmpty(this.ReferenceNumberTextBox.Text))
				{
					this.Page.Session.Add(PageSessionKeyConstants.SOP_REFERENCE_NUMBER, this.ReferenceNumberTextBox.Text.ToUpper());
					filter.ReferenceNumber = this.ReferenceNumberTextBox.Text.ToUpper();
					this.ReferenceNumberTextBox.Text = this.ReferenceNumberTextBox.Text.ToUpper();
				}

			    var limits = new EnumerationLimits();
				var limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.STANDING_OFFER);

				var standingOfferCollection = FMChannelHelper.MakeCall<IStandingOffers, StandingOfferCollectionClass>(
				    x =>
				        x.EnumerateWithFilter(this.security,filter)
				    );

				if (standingOfferCollection.Count >= limit && limit > 0)
				{
					this.lblWarning.Text = "Results limited to first " + limit + " records.  Use filters to narrow search.";
					this.lblWarning.Visible = true;
				}
				else
				{
					this.lblWarning.Visible = false;
				}

				this.Session.Add(PageSessionKeyConstants.SOP_COLLECTION, standingOfferCollection);
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will handle the cancel line item editing event.  It will reinstate
		///    the original values.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		protected void StandingOfferDataGridCancelCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			int itemIndex = -99;

			try
			{
				var standingOfferIndexLabel = (Label)eventArgs.Item.FindControl("StandingOfferIndexLabel");

				if (standingOfferIndexLabel != null)
				{
					itemIndex = Convert.ToInt32(standingOfferIndexLabel.Text);
				    var standingOfferCollection = (StandingOfferCollectionClass)this.Session[PageSessionKeyConstants.SOP_COLLECTION];
					StandingOfferClass standingOffer = standingOfferCollection[itemIndex];

					if (standingOffer.IdentityGuid == Guid.Empty)
					{
						standingOfferCollection.RemoveAt(itemIndex);

						if ((this.StandingOfferDataGrid.Items.Count == 1) && (this.StandingOfferDataGrid.CurrentPageIndex > 0))
						{
							this.StandingOfferDataGrid.CurrentPageIndex--;
						}
					}
					else
					{
						StandingOfferClass originalStandingOffer = FMChannelHelper.MakeCall<IStandingOffers, StandingOfferClass>(
																	 x =>
																	 x.Get(this.security, standingOffer.IdentityGuid)
																);

						standingOffer.SupplierID = originalStandingOffer.SupplierID;
						standingOffer.ProductID = originalStandingOffer.ProductID;
						standingOffer.LocationID = originalStandingOffer.LocationID;
						standingOffer.StandingOfferPrice = originalStandingOffer.StandingOfferPrice;
						standingOffer.EffectiveDate = originalStandingOffer.EffectiveDate;
						standingOffer.ExpirationDate = originalStandingOffer.ExpirationDate;
						standingOffer.LowerBound = originalStandingOffer.LowerBound;
						standingOffer.UpperBound = originalStandingOffer.UpperBound;
					}

					this.StandingOfferDataGrid.EditItemIndex = -1;

					this.UpdateView();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(new Exception(Msg004 + " #" + itemIndex + ". " + ex.Message));
			}
		}

		/// <summary>
		///    This method will handle the delete event. It will delete the item from the database
		///    and refresh the view.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		protected void StandingOfferDataGridDeleteCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			int itemIndex = -99;

			try
			{
				var standingOfferIndexLabel = (Label)eventArgs.Item.FindControl("StandingOfferIndexLabel");

				if (standingOfferIndexLabel != null)
				{
					itemIndex = Convert.ToInt32(standingOfferIndexLabel.Text);
				    var standingOfferCollection = (StandingOfferCollectionClass)this.Session[PageSessionKeyConstants.SOP_COLLECTION];

				    var standingOffer = standingOfferCollection[itemIndex];

					// Non empty guid indicates StandingOffer has been committed to database
					if (standingOffer.IdentityGuid != Guid.Empty)
					{
						FMChannelHelper.MakeCall<IStandingOffers>(
																	 x =>
																	 x.Purge(this.security,standingOffer.IdentityGuid)
																);
					}

					if (this.StandingOfferDataGrid.EditItemIndex == eventArgs.Item.ItemIndex)
					{
						this.StandingOfferDataGrid.EditItemIndex = -1;
					}
					else if (this.StandingOfferDataGrid.EditItemIndex > eventArgs.Item.ItemIndex)
					{
						this.StandingOfferDataGrid.EditItemIndex--;
					}

					standingOfferCollection.RemoveAt(itemIndex);

					if ((this.StandingOfferDataGrid.CurrentPageIndex > 0)
					    && (this.StandingOfferDataGrid.CurrentPageIndex * this.StandingOfferDataGrid.PageSize
					        >= standingOfferCollection.Count))
					{
						this.StandingOfferDataGrid.CurrentPageIndex--;
					}

					this.UpdateView();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(new Exception(Msg012 + " #" + itemIndex + ". " + ex.Message));
			}
		}

		/// <summary>
		///    This method will handle the edit line item editing event. It will place the line
		///    into edit mode.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		protected void StandingOfferDataGridEditCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			try
			{
				this.StandingOfferDataGrid.EditItemIndex = eventArgs.Item.ItemIndex;
				this.EnableControls(false, true);
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will handle the line item data binding event.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		protected void StandingOfferDataGridItemDataBound(object source, DataGridItemEventArgs eventArgs)
		{
			try
			{
				var editButton = (LinkButton)eventArgs.Item.FindControl("EditLinkButton");
				var deleteButton = (LinkButton)eventArgs.Item.FindControl("DeleteLinkButton");
				var siteGuidLabel = (Label)eventArgs.Item.FindControl("SiteGuidLabel");

				// Disable the edit and delete buttons if the user does not have modify rights or
				// not login to a site group.
				if ((editButton != null) && (deleteButton != null))
				{
					if ((this.security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) == false)
					    || this.security.SiteGuid != Guid.Parse(siteGuidLabel.Text))
					{
						editButton.Enabled = false;
						deleteButton.Enabled = false;
					}
				}

				if ((this.StandingOfferDataGrid != null) && (this.StandingOfferDataGrid.EditItemIndex == eventArgs.Item.ItemIndex))
				{
					// Now set the focus based on the row that is being edited. If not in edit
					// mode, then set it to the edit icon.
					Control ctrl;

					if (this.StandingOfferDataGrid.EditItemIndex == eventArgs.Item.ItemIndex)
					{
						ctrl = eventArgs.Item.FindControl("UpdateLinkButton");
					}
					else
					{
						ctrl = eventArgs.Item.FindControl("EditLinkButton");
					}

					if (ctrl != null)
					{
						this.ScriptManager.SetFocus(ctrl);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will handle the page index change event.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		protected void StandingOfferDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs eventArgs)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.StandingOfferDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.StandingOfferDataGrid.CurrentPageIndex = eventArgs.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method will handle the update event. It will determine if the update is to an existing item or
		///    a new. It will update the database with a new item or update an existing one.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="eventArgs"></param>
		protected void StandingOfferDataGridUpdateCommand(object source, DataGridCommandEventArgs eventArgs)
		{
			int itemIndex = -99;
			DateTimeOffset siteTimeToday = TimeConverter.Today(this.accountingSite.CurrentSite);
			DateTimeOffset newEffDate = siteTimeToday;
			DateTimeOffset newExpDate = siteTimeToday;
			bool errorFlag = false;
			int lowerBound = 0;
			int upperBound = 0;
			bool foundLowerBound = false;
			bool foundUpperBound = false;

			try
			{
				var standingOfferIndexLabel = (Label)eventArgs.Item.FindControl("StandingOfferIndexLabel");

				if (standingOfferIndexLabel != null)
				{
					itemIndex = Convert.ToInt32(standingOfferIndexLabel.Text);
				    var standingOfferCollection = (StandingOfferCollectionClass)this.Session[PageSessionKeyConstants.SOP_COLLECTION];

				    var standingOfferPriceTb = (TextBox)eventArgs.Item.FindControl("StandingOfferPriceTextBox");
					var effectiveDateTb = (FMDate)eventArgs.Item.FindControl("EffectiveGridDate");
					var expirationDateTb = (FMDate)eventArgs.Item.FindControl("ExpirationGridDate");
					var supplierTb = (FMCompanyTextBox)eventArgs.Item.FindControl("FMSupplierGridTextBox");
					var productTb = (FMProductTextBox)eventArgs.Item.FindControl("FMProductGridTextBox");
					var locationDd = (FMLocationSelectDropDown)eventArgs.Item.FindControl("FMLocationDropDownList");
					var lowerBoundTextBox = (TextBox)eventArgs.Item.FindControl("LowerBoundTextBox");
					var upperBoundTextBox = (TextBox)eventArgs.Item.FindControl("UpperBoundTextBox");
					var referenceNumberTextBox = (TextBox)eventArgs.Item.FindControl("ReferenceNumberTextBox");

				    var standingOffer = standingOfferCollection[itemIndex];

					// Convert the price list (aka standing offer) price to float prior to saving.
					if (standingOfferPriceTb != null)
					{
						try
						{
							standingOffer.StandingOfferPrice = Convert.ToDouble(standingOfferPriceTb.Text);
						}
						catch (InvalidCastException)
						{
							errorFlag = true;
							this.ErrorHandler(new Exception(Msg005));
						}
					}

					// Must contain a value and it must be a valid numeric value.
					if (string.IsNullOrEmpty(lowerBoundTextBox?.Text))
					{
						errorFlag = true;
						this.ErrorHandler(new Exception(Msg013));
					}
					else
					{
						try
						{
							lowerBound = Convert.ToInt32(lowerBoundTextBox.Text);
							standingOffer.LowerBound = lowerBound;
							foundLowerBound = true;
						}
						catch (Exception)
						{
							errorFlag = true;
							this.ErrorHandler(new Exception(Msg014));
						}
					}

					// Must contain a value and it must be a valid numeric value.
					if (string.IsNullOrEmpty(upperBoundTextBox?.Text))
					{
						errorFlag = true;
						this.ErrorHandler(new Exception(Msg015));
					}
					else
					{
						try
						{
							upperBound = Convert.ToInt32(upperBoundTextBox.Text);
							standingOffer.UpperBound = upperBound;
							foundUpperBound = true;
						}
						catch (Exception)
						{
							errorFlag = true;
							this.ErrorHandler(new Exception(Msg016));
						}
					}

					// Ensure that the lower bound value is less than or equal to the
					// upper bound value.
					if (foundUpperBound && foundLowerBound)
					{
						if (lowerBound > upperBound)
						{
							errorFlag = true;
							this.ErrorHandler(new Exception(Msg017));
						}
					}

					if (!string.IsNullOrEmpty(referenceNumberTextBox?.Text))
					{
						standingOffer.ReferenceNumber = referenceNumberTextBox.Text;
					}

					// Test to see if a date is present. Only save if there is a date.
					if (!string.IsNullOrEmpty(effectiveDateTb?.Text))
					{
						newEffDate = effectiveDateTb.CurrentValue;
					}
					else
					{
						errorFlag = true;
						this.ErrorHandler(new Exception(Msg006));
					}

					// Test to see if a date is present. Only save if there is a date.
					if (!string.IsNullOrEmpty(expirationDateTb?.Text))
					{
						newExpDate = expirationDateTb.CurrentValue;
					}
					else
					{
						errorFlag = true;
						this.ErrorHandler(new Exception(Msg007));
					}

					// Ensure that the effective date is before the expiration date.
					// Throw an error if not.
					if (newEffDate > newExpDate)
					{
						errorFlag = true;
						this.ErrorHandler(new Exception(Msg008));
					}
					else
					{
						standingOffer.EffectiveDate = newEffDate;
						standingOffer.ExpirationDate = newExpDate;
					}

					if (supplierTb != null)
					{
						standingOffer.SupplierID = supplierTb.Text;
						standingOffer.SupplierGuid = this.GetCompanyGuid(supplierTb.Text);
					}
					else
					{
						errorFlag = true;
						this.ErrorHandler(new Exception(Msg009));
					}

					if (productTb != null)
					{
						standingOffer.ProductID = productTb.Text;
						standingOffer.ProductGuid = this.GetProductGuid(productTb.Text);
					}
					else
					{
						errorFlag = true;
						this.ErrorHandler(new Exception(Msg010));
					}

					if (locationDd != null)
					{
						standingOffer.LocationID = locationDd.SelectedItem.Text;
						standingOffer.LocationGuid = Guid.Parse(locationDd.SelectedValue);
					}

					if (errorFlag == false)
					{
						bool isOverlapping = FMChannelHelper.MakeCall<IStandingOffers, bool>(
																	 x =>
																	 x.IsStandingOfferOverlapping(this.security,standingOffer)
																);

						if (isOverlapping)
						{
							this.ErrorHandler(new Exception(Msg018));
						}
						else
						{
							if (standingOffer.IdentityGuid == Guid.Empty)
							{
								standingOffer.SiteGuid = this.security.SiteGuid;
								standingOffer.IdentityGuid = FMChannelHelper.MakeCall<IStandingOffers, Guid>(
																	 x =>
																	 x.Add(this.security, standingOffer)
																);

							}
							else
							{
								FMChannelHelper.MakeCall<IStandingOffers>(
																	 x =>
																	 x.Modify(this.security,standingOffer)
																);
							}

							// When the price list entry (aka standing offer) changes then all the transactions within the current
							// month must be updated.
							this.UpdateAssociatedTransactions(this.security, standingOffer);

							this.StandingOfferDataGrid.EditItemIndex = -1;
						}
					}

					this.EnableControls(true, true);
					this.UpdateView();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(new Exception(Msg011 + " #" + itemIndex + ". " + ex.Message));
				this.UpdateView();
			}
		}

		/// <summary>
		///    This method will perform adding a new item to the collection and placing the
		///    grid in edit mode for the new object.
		/// </summary>
		private void AddOnClick()
		{
		    var standingOfferCollection = (StandingOfferCollectionClass)this.Session[PageSessionKeyConstants.SOP_COLLECTION];
		    var standingOffer = new StandingOfferClass { SiteGuid = this.security.SiteGuid };

		    standingOfferCollection.Add(standingOffer);

			this.StandingOfferDataGrid.CurrentPageIndex = (standingOfferCollection.Count - 1)
			                                              / this.StandingOfferDataGrid.PageSize;
			this.StandingOfferDataGrid.EditItemIndex = (standingOfferCollection.Count - 1) % this.StandingOfferDataGrid.PageSize;

			this.EnableControls(false, true);
			this.UpdateView();
		}

		/// <summary>
		///    This method will check for permission for modifying. If the site is not a site group or the
		///    user does not have modify rights, then the editing is disabled.
		/// </summary>
		private void CheckPermissions()
		{
			if ((this.security.HasRight(RIGHT.MODIFY_STANDING_OFFERS) == false))
			{
				this.EnableControls(false, false);
			}
			else
			{
				this.EnableControls(true, true);
			}
		}

		/// <summary>
		///    This method enables and disables the controls.
		/// </summary>
		/// <param name="enable"></param>
		/// <param name="all"></param>
		private void EnableControls(bool enable, bool all)
		{
			this.AddButton1.Enabled = enable;
			this.AddButton2.Enabled = enable;

			if (all)
			{
				this.RefreshButton.Enabled = enable;
				this.SupplierTextBox.Enabled = enable;
				this.ProductTextBox.Enabled = enable;
				this.EffectiveDateDate.Enabled = enable;
				this.EndDateTextBox.Enabled = enable;
				this.LocationSelect.Enabled = enable;
			}
		}

		/// <summary>
		///    This method will enumerate the Standing Offers (aka Price List) and map the data to a
		///    dataview to match the grid.
		/// </summary>
		/// <returns></returns>
		private DataView EnumerateStandingOffers()
		{
		    var standingOfferCollection = (StandingOfferCollectionClass)this.Session[PageSessionKeyConstants.SOP_COLLECTION];

			var mapDataTable = new DataTable();

		    mapDataTable.Columns.Add("StandingOfferIndex", typeof(int));
			mapDataTable.Columns.Add("SiteGuid", typeof(Guid));
			mapDataTable.Columns.Add("StandingOfferID", typeof(string));
			mapDataTable.Columns.Add("SupplierID", typeof(string));
			mapDataTable.Columns.Add("ProductID", typeof(string));
			mapDataTable.Columns.Add("LowerBound", typeof(double));
			mapDataTable.Columns.Add("UpperBound", typeof(double));
			mapDataTable.Columns.Add("LocationID", typeof(string));
			mapDataTable.Columns.Add("LocationGuid", typeof(Guid));
			mapDataTable.Columns.Add("StandingOfferPrice", typeof(double));
			mapDataTable.Columns.Add("EffectiveDate", typeof(string));
			mapDataTable.Columns.Add("ExpirationDate", typeof(string));
			mapDataTable.Columns.Add("ReferenceNumber", typeof(string));

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.security,
																			this.security.SiteGuid,
																			getMemberSites: true,
																			getSchedulesAndProcessVariables: true,
																			bGetAssociatedAliases: true)
																	);

			DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();
			string format = dateTimeFormatInfo.ShortDatePattern;

			for (int nextItem = 0; nextItem < standingOfferCollection.Count; nextItem++)
			{
				var mapDataRow = mapDataTable.NewRow();
				var standingOffer = standingOfferCollection[nextItem];

				mapDataRow["StandingOfferIndex"] = nextItem;
				mapDataRow["SiteGuid"] = standingOffer.SiteGuid;
				mapDataRow["StandingOfferID"] = standingOffer.ID;
				mapDataRow["SupplierID"] = standingOffer.SupplierID;
				mapDataRow["ProductID"] = standingOffer.ProductID;
				mapDataRow["LowerBound"] = standingOffer.LowerBound;
				mapDataRow["UpperBound"] = standingOffer.UpperBound;
				mapDataRow["LocationID"] = standingOffer.LocationID;
				mapDataRow["LocationGuid"] = standingOffer.LocationGuid;
				mapDataRow["StandingOfferPrice"] = standingOffer.StandingOfferPrice;
				mapDataRow["EffectiveDate"] = standingOffer.EffectiveDate.ToString(format);
				mapDataRow["ExpirationDate"] = standingOffer.ExpirationDate.ToString(format);
				mapDataRow["ReferenceNumber"] = standingOffer.ReferenceNumber;

				mapDataTable.Rows.Add(mapDataRow);
			}

			var standingOfferDataView = new DataView(mapDataTable);
			return standingOfferDataView;
		}

		/// <summary>
		///    This method will return the company Guid that matches the company ID.
		///    It will return an empty guid if no match is found.
		/// </summary>
		/// <param name="companyID"></param>
		/// <returns></returns>
		private Guid GetCompanyGuid(string companyID)
		{
			Guid companyGuid = Guid.Empty;

			if (!String.IsNullOrWhiteSpace(companyID))
			{
				companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.security,companyID)
																);
			}

			return companyGuid;
		}

		/// <summary>
		///    This method will return the product Guid that matches the product ID.
		///    It will return an empty guid if no match is found.
		/// </summary>
		/// <param name="productID"></param>
		/// <returns></returns>
		private Guid GetProductGuid(string productID)
		{
			Guid productGuid = Guid.Empty;

			if (!String.IsNullOrWhiteSpace(productID))
			{
				productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetMasterRecordGuidFromID(this.security,productID)
																);
			}

			return productGuid;
		}

        /// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.StandingOfferDataGrid.EditCommand +=
				this.StandingOfferDataGridEditCommand;
			this.StandingOfferDataGrid.PageIndexChanged +=
				this.StandingOfferDataGridPageIndexChanged;
			this.StandingOfferDataGrid.CancelCommand +=
				this.StandingOfferDataGridCancelCommand;
			this.StandingOfferDataGrid.UpdateCommand +=
				this.StandingOfferDataGridUpdateCommand;
			this.StandingOfferDataGrid.DeleteCommand +=
				this.StandingOfferDataGridDeleteCommand;
			this.StandingOfferDataGrid.ItemDataBound +=
				this.StandingOfferDataGridItemDataBound;
		}

		/// <summary>
		///    This method will update the header information with the appropriate header data.
		/// </summary>
		private void PersistHeaderView()
		{
			if (this.Session[PageSessionKeyConstants.SOP_SUPPLIER] == null)
			{
				this.Session.Add(PageSessionKeyConstants.SOP_SUPPLIER, this.SupplierTextBox.Text);
			}
			else
			{
				this.Session[PageSessionKeyConstants.SOP_SUPPLIER] = this.SupplierTextBox.Text;
			}

			if (this.Session[PageSessionKeyConstants.SOP_PRODUCT] == null)
			{
				this.Session.Add(PageSessionKeyConstants.SOP_PRODUCT, this.ProductTextBox.Text);
			}
			else
			{
				this.Session[PageSessionKeyConstants.SOP_PRODUCT] = this.ProductTextBox.Text;
			}

			if (this.Session[PageSessionKeyConstants.SOP_LOCATION] == null)
			{
				this.Session.Add(PageSessionKeyConstants.SOP_LOCATION, this.LocationSelect.SelectedIndex.ToString());
			}
			else
			{
				this.Session[PageSessionKeyConstants.SOP_LOCATION] = this.LocationSelect.SelectedIndex.ToString();
			}

			if (this.Session[PageSessionKeyConstants.SOP_EFFECTIVE_DATE] == null)
			{
				this.Session.Add(PageSessionKeyConstants.SOP_EFFECTIVE_DATE, this.EffectiveDateDate.Text);
			}
			else
			{
				this.Session[PageSessionKeyConstants.SOP_EFFECTIVE_DATE] = this.EffectiveDateDate.Text;
			}

			if (this.Session[PageSessionKeyConstants.SOP_EFF_END_DATE] == null)
			{
				this.Session.Add(PageSessionKeyConstants.SOP_EFF_END_DATE, this.EndDateTextBox.Text);
			}
			else
			{
				this.Session[PageSessionKeyConstants.SOP_EFF_END_DATE] = this.EndDateTextBox.Text;
			}

			if (this.Session[PageSessionKeyConstants.SOP_REFERENCE_NUMBER] == null)
			{
				this.Session.Add(PageSessionKeyConstants.SOP_REFERENCE_NUMBER, this.ReferenceNumberTextBox.Text);
			}
			else
			{
				this.Session[PageSessionKeyConstants.SOP_REFERENCE_NUMBER] = this.ReferenceNumberTextBox.Text;
			}
		}

		/// <summary>
		///    Updates transaction line items that are within current month and effective date of added/modified price list (aka standing offer) price.
		/// </summary>
		/// <param name="securityParam"></param>
		/// <param name="standingOffer"></param>
		private void UpdateAssociatedTransactions(SecurityClass securityParam, StandingOfferClass standingOffer)
		{
			try
			{
				// Update the associated transactions with the price list (aka standing offer) price.
			    var sr = new StandingOffersSR { StandingOfferGuid = standingOffer.IdentityGuid, Security = securityParam };


			    // Retrieve the list of associated transactions
				FMChannelHelper.MakeCall<IStandingOffersProcessor>(
																	 x =>
																	 x.Process(sr)
																);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		/// <summary>
		///    This method will update the header information with the appropriate header data.
		/// </summary>
		private void UpdateHeaderView()
		{
			if (this.Session[PageSessionKeyConstants.SOP_SUPPLIER] != null)
			{
				this.SupplierTextBox.Text = (string)this.Session[PageSessionKeyConstants.SOP_SUPPLIER];
			}
			else
			{
				this.SupplierTextBox.Text = "{All}";
			}

			if (this.Session[PageSessionKeyConstants.SOP_PRODUCT] != null)
			{
				this.ProductTextBox.Text = (string)this.Session[PageSessionKeyConstants.SOP_PRODUCT];
			}
			else
			{
				this.ProductTextBox.Text = "{All}";
			}

			if (this.Session[PageSessionKeyConstants.SOP_LOCATION] != null)
			{
				this.LocationSelect.SelectedIndex = Convert.ToInt32(this.Session[PageSessionKeyConstants.SOP_LOCATION]);
			}

			if (this.Session[PageSessionKeyConstants.SOP_EFFECTIVE_DATE] != null)
			{
				this.EffectiveDateDate.Text = (string)this.Session[PageSessionKeyConstants.SOP_EFFECTIVE_DATE];
			}

			if (this.Session[PageSessionKeyConstants.SOP_EFF_END_DATE] != null)
			{
				this.EndDateTextBox.Text = (string)this.Session[PageSessionKeyConstants.SOP_EFF_END_DATE];
			}

			if (this.Session[PageSessionKeyConstants.SOP_REFERENCE_NUMBER] != null)
			{
				this.ReferenceNumberTextBox.Text = (string)this.Session[PageSessionKeyConstants.SOP_REFERENCE_NUMBER];
			}

			this.SiteTextBox.Text = this.security.SiteID;
		}

		/// <summary>
		///    This method will update the view (grid) with the new data.
		/// </summary>
		private void UpdateView()
		{
			try
			{
				DataView dataCollection = this.EnumerateStandingOffers();

			    this.PageSizeDropDown?.SetPageSize(this.StandingOfferDataGrid, dataCollection.Count);

			    this.StandingOfferDataGrid.DataSource = dataCollection;
				this.StandingOfferDataGrid.DataBind();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    This method validates the date range fields (Effective start and end). If one of the
		///    date fields is populated, then the other must be too. The start date must be older
		///    than the end date.
		/// </summary>
		/// <returns></returns>
		private bool ValidateDates()
		{
			bool successful = true;

			if (!string.IsNullOrEmpty(this.EffectiveDateDate.Text))
			{
				if ((string.IsNullOrEmpty(this.EndDateTextBox.Text)) || (this.EndDateTextBox.Text.Length <= 0))
				{
					successful = false;
					this.ErrorHandler(new Exception(Msg001));
				}
			}

			if (!string.IsNullOrEmpty(this.EndDateTextBox.Text))
			{
				if (string.IsNullOrEmpty(this.EffectiveDateDate.Text))
				{
					successful = false;
					this.ErrorHandler(new Exception(Msg002));
				}
			}

			if (this.EffectiveDateDate.CurrentValue > this.EndDateTextBox.CurrentValue)
			{
				successful = false;
				this.ErrorHandler(new Exception(Msg003));
			}

			return successful;
		}

		#endregion
	}
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllocationForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AllocationForm type.
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
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	/// <summary>
	///    Summary description for AllocationForm.
	/// </summary>
	public partial class AllocationForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields
		protected Guid AssignedGuid = Guid.Empty;
		protected SiteClass CurrentSite;
		protected string LimitText;
		protected string NextText;
		protected ALLOCATION_RESET_METHOD ResetMethod;
		protected int ResetMultiple;
		protected ALLOCATION_RESET_PERIOD ResetPeriod;
		protected ALLOCATION_TYPE Type;
		#endregion

		#region Public Methods and Operators

		public void EnableControls(bool Enable)
		{
			this.Cancel.Enabled = Enable;
			this.RefreshButton.Enabled = Enable;

			if (this.Security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
			{
				this.OK.Enabled = Enable;
				this.AddButton.Enabled = Enable;
				this.ResetButton.Enabled = Enable;
				this.ExpirationDate.Enabled = Enable;
				this.EffectiveDate.Enabled = Enable;
			}
		}

		public void MethodDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			DataGridItem Item = this.LineItemsDataGrid.Items[this.LineItemsDataGrid.EditItemIndex];
			var IDDropDownList = (DropDownList)Item.FindControl("IDDropDownList");

			if (IDDropDownList != null && IDDropDownList.SelectedIndex != -1)
			{
				this.AssignedGuid = Guid.Parse(IDDropDownList.SelectedValue);
			}

			var LimitTextBox = (TextBox)Item.FindControl("LimitTextBox");
			this.LimitText = LimitTextBox.Text;
			var NextTextBox = (TextBox)Item.FindControl("NextTextBox");
			this.NextText = NextTextBox.Text;
			var ResetPeriodDropDownList = (FMDropDownList)Item.FindControl("ResetPeriodDropDownList");
			this.ResetPeriod = (ALLOCATION_RESET_PERIOD)Convert.ToInt32(ResetPeriodDropDownList.SelectedValue);
			var ResetMultipleTextBox = (TextBox)Item.FindControl("ResetMultipleTextBox");
			this.ResetMultiple = Convert.ToInt32(ResetMultipleTextBox.Text);
			var ResetMethodDropDownList = (FMDropDownList)Item.FindControl("ResetMethodDropDownList");
			this.ResetMethod = (ALLOCATION_RESET_METHOD)Convert.ToInt32(ResetMethodDropDownList.SelectedValue);
			var TypeDropDownList = (FMDropDownList)Item.FindControl("TypeDropDownList");
			this.Type = (ALLOCATION_TYPE)Convert.ToInt32(TypeDropDownList.SelectedValue);
			try
			{
				this.UpdateLineItemsView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.EnableControls(true);
				this.LineItemsDataGrid.EditItemIndex = -1;
				this.UpdateLineItemsView();
			}
		}

		public void TypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			DataGridItem Item = this.LineItemsDataGrid.Items[this.LineItemsDataGrid.EditItemIndex];

			var IDDropDownList = (DropDownList)Item.FindControl("IDDropDownList");
			{
				if (IDDropDownList != null && IDDropDownList.SelectedIndex != -1)
				{
					this.AssignedGuid = Guid.Parse(IDDropDownList.SelectedValue);
				}
			}

			var LimitTextBox = (TextBox)Item.FindControl("LimitTextBox");
			this.LimitText = LimitTextBox.Text;

			var NextTextBox = (TextBox)Item.FindControl("NextTextBox");
			this.NextText = NextTextBox.Text;

			var ResetPeriodDropDownList = (FMDropDownList)Item.FindControl("ResetPeriodDropDownList");
			this.ResetPeriod = (ALLOCATION_RESET_PERIOD)Convert.ToInt32(ResetPeriodDropDownList.SelectedValue);

			var ResetMultipleTextBox = (TextBox)Item.FindControl("ResetMultipleTextBox");
			this.ResetMultiple = Convert.ToInt32(ResetMultipleTextBox.Text);

			var ResetMethodDropDownList = (FMDropDownList)Item.FindControl("ResetMethodDropDownList");
			this.ResetMethod = (ALLOCATION_RESET_METHOD)Convert.ToInt32(ResetMethodDropDownList.SelectedValue);

			var TypeDropDownList = (FMDropDownList)Item.FindControl("TypeDropDownList");
			this.Type = (ALLOCATION_TYPE)Convert.ToInt32(TypeDropDownList.SelectedValue);

			try
			{
				this.UpdateLineItemsView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.EnableControls(true);
				this.LineItemsDataGrid.EditItemIndex = -1;
				this.UpdateLineItemsView();
			}
		}

		#endregion

		#region Methods

		protected void CompanyAssignedDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.CompanyAssignedToDropDownList.Items.Clear();

			var Allocation = (AllocationClass)this.Session["Allocation"];

			CompanyMapCollectionClass companyMapCollection =
				FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
					x =>
					x.EnumerateByAssignedGuidAndType(
						this.Security, Guid.Parse(this.CompanyAssignedDropDownList.SelectedValue), Allocation.CompanyMapType));

			foreach (CompanyMapClass CompanyMap in companyMapCollection)
			{
				var CompanyAssignedToItem = new ListItem(CompanyMap.AssignedToID, CompanyMap.IdentityGuid.ToString());

				foreach (ListItem ExistingCompanyMapItem in this.CompanyAssignedToDropDownList.Items)
				{
					if (ExistingCompanyMapItem.Text.CompareTo(CompanyAssignedToItem.Text) > 0)
					{
						int Index = this.CompanyAssignedToDropDownList.Items.IndexOf(ExistingCompanyMapItem);
						this.CompanyAssignedToDropDownList.Items.Insert(Index, CompanyAssignedToItem);
						CompanyAssignedToItem = null;
						break;
					}
				}

				if (CompanyAssignedToItem != null)
				{
					this.CompanyAssignedToDropDownList.Items.Add(CompanyAssignedToItem);
				}
			}

			if (this.CompanyAssignedToDropDownList.Items.Count == 0)
			{
				throw new Exception("No available Company Hierarchies");
			}

			Allocation.ID = this.CompanyAssignedToDropDownList.SelectedItem.Text + "->"
			                + this.CompanyAssignedDropDownList.SelectedItem.Text;
			Allocation.CompanyMapGuid = Guid.Parse(this.CompanyAssignedToDropDownList.SelectedValue);
		}

		protected void CompanyAssignedToDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			var Allocation = (AllocationClass)this.Session["Allocation"];
			Allocation.ID = this.CompanyAssignedToDropDownList.SelectedItem.Text + "->"
			                + this.CompanyAssignedDropDownList.SelectedItem.Text;
			Allocation.CompanyMapGuid = Guid.Parse(this.CompanyAssignedToDropDownList.SelectedValue);
			foreach (AllocationLineItemClass LineItem in Allocation.LineItemCollection)
			{
				LineItem.Loaded.Value = this.GetAmountLoaded(
					this.Security,
					Allocation.ID,
					LineItem.AssignedGuid,
					LineItem.Type,
					LineItem.ResetPeriod,
					LineItem.ResetMultiple,
					LineItem.ResetDate.Value,
					LineItem.ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD ? LineItem.ResetDate.Value : Allocation.LastAllocationResetDate.Value,
					Allocation._ExpirationDate.Value,
					this.Security.SiteGuid,
					STATION_TYPE.MAX_STATION_TYPE,
					"");
			}
		}

		private double GetAmountLoaded(SecurityClass securityClass, string allocationId, Guid itemAssignedGuid, ALLOCATION_TYPE allocationType,
			ALLOCATION_RESET_PERIOD allocationResetPeriod, int itemResetMultiple, DateTimeOffset resetDate, DateTimeOffset lastAllocationResetDate, 
			DateTimeOffset expirationDate, Guid siteGuid, STATION_TYPE stationType, string transactionId)
		{
			return FMChannelHelper.MakeCall<IAllocationLineItems, double>(
						x =>
						x.GetAmountLoaded(securityClass,
												allocationId,
												itemAssignedGuid,
												allocationType,
												allocationResetPeriod,
												itemResetMultiple,
												resetDate,
												lastAllocationResetDate,
												expirationDate,
												siteGuid,
												stationType,
												transactionId)
					);
		}

		protected ListItemCollection EnumerateIDs()
		{
			var ProductItems = new ListItemCollection();

			int CurrentIndex = this.LineItemsDataGrid.EditItemIndex
			                   + this.LineItemsDataGrid.CurrentPageIndex * this.LineItemsDataGrid.PageSize;

			var Allocation = (AllocationClass)this.Session["Allocation"];

			if (this.Type == ALLOCATION_TYPE.PRODUCT_ALLOCATION)
			{
				ProductCollectionClass ProductCollection =
					FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.Security));

				foreach (ProductClass Product in ProductCollection)
				{
					if (Product.ProductType == ProductType.AdditiveProduct)
					{
						continue;
					}

					var NewProductItem = new ListItem(Product.ID, Product.MasterRecordGuid.ToString());
					foreach (ListItem ExistingProductItem in ProductItems)
					{
						if (ExistingProductItem.Text.CompareTo(NewProductItem.Text) > 0)
						{
							int ItemIndex = ProductItems.IndexOf(ExistingProductItem);
							ProductItems.Insert(ItemIndex, NewProductItem);
							NewProductItem = null;
							break;
						}
					}

					if (NewProductItem != null)
					{
						ProductItems.Add(NewProductItem);
					}
				}
			}

			else if (this.Type == ALLOCATION_TYPE.PRODUCT_GROUP_ALLOCATION)
			{
				ProductGroupCollectionClass ProductGroupCollection =
					FMChannelHelper.MakeCall<IProductGroups, ProductGroupCollectionClass>(x => x.Enumerate(this.Security));

				foreach (ProductGroupClass ProductGroup in ProductGroupCollection)
				{
					var NewProductItem = new ListItem(ProductGroup.ID, ProductGroup.IdentityGuid.ToString());
					foreach (ListItem ExistingProductItem in ProductItems)
					{
						if (ExistingProductItem.Text.CompareTo(NewProductItem.Text) > 0)
						{
							int ItemIndex = ProductItems.IndexOf(ExistingProductItem);
							ProductItems.Insert(ItemIndex, NewProductItem);
							NewProductItem = null;
							break;
						}
					}

					if (NewProductItem != null)
					{
						ProductItems.Add(NewProductItem);
					}
				}
			}

         return ProductItems.Count == 0 && this.Type != ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION
                ?              throw new Exception("No available items")
                : ProductItems;
      }

      protected ListItemCollection EnumerateResetMethods()
		{
			var Allocation = (AllocationClass)this.Session["Allocation"];

			var ResetMethodItems = new ListItemCollection();

			for (var ResetMethod = ALLOCATION_RESET_METHOD.REPEAT_METHOD;
			     ResetMethod < ALLOCATION_RESET_METHOD.MAX_ALLOCATION_METHOD;
			     ResetMethod++)
			{
				if ((Allocation.CompanyMapType != COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP
				     || this.Type != ALLOCATION_TYPE.PRODUCT_ALLOCATION)
				    && ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD)
				{
					continue;
				}

				string MethodID = this.GetTranslatedText(AllocationLineItemClass.ResetMethodID(ResetMethod));
				var NewResetMethodItem = new ListItem(MethodID, ((int)ResetMethod).ToString());
				ResetMethodItems.Add(NewResetMethodItem);
			}
			return ResetMethodItems;
		}

		protected ListItemCollection EnumerateResetPeriods()
		{
			var ResetPeriodItems = new ListItemCollection();

			for (var ResetPeriod = ALLOCATION_RESET_PERIOD.DAY_RESET_PERIOD;
			     ResetPeriod < ALLOCATION_RESET_PERIOD.MAX_RESET_PERIOD;
			     ResetPeriod++)
			{
				string PeriodID = this.GetTranslatedText(AllocationLineItemClass.ResetPeriodID(ResetPeriod));
				var NewResetPeriodItem = new ListItem(PeriodID, ((int)ResetPeriod).ToString());
				ResetPeriodItems.Add(NewResetPeriodItem);
			}
			return ResetPeriodItems;
		}

		protected ListItemCollection EnumerateTypes()
		{
			var TypeItems = new ListItemCollection();

			for (var Type = ALLOCATION_TYPE.PRODUCT_ALLOCATION; Type < ALLOCATION_TYPE.MAX_ALLOCATION_TYPE; Type++)
			{
				if (this.ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD
				    && (Type == ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION || Type == ALLOCATION_TYPE.PRODUCT_GROUP_ALLOCATION))
				{
					continue;
				}

				string TypeID = this.GetTranslatedText(AllocationLineItemClass.TypeID(Type));
				var NewTypeItem = new ListItem(TypeID, ((int)Type).ToString());
				TypeItems.Add(NewTypeItem);
			}
			return TypeItems;
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove("Status");

				this.GetSecurity();

				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.Security,
																			this.Security.SiteGuid,
																			getMemberSites: false,
																			getSchedulesAndProcessVariables: false,
																			bGetAssociatedAliases: true)
																);

				if (this.Page.IsPostBack == false)
				{
					if (this.Security.HasRight(RIGHT.MODIFY_ALLOCATIONS) == false)
					{
						this.OK.Enabled = false;
						this.AddButton.Enabled = false;
						this.ResetButton.Enabled = false;
						this.EffectiveDate.Enabled = false;
						this.ExpirationDate.Enabled = false;
					}

					AllocationClass Allocation;

					// Get IdentityGuid
					if (this.Session["IdentityGuid"] != null)
					{
						Allocation =
							FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
								x => x.Get(this.Security, Guid.Parse(this.Session["IdentityGuid"] as string), STATION_TYPE.MAX_STATION_TYPE, ""));

						this.CompanyAssignedDropDownList.Enabled = false;
						this.CompanyAssignedToDropDownList.Enabled = false;

						int indexOfLastDelimeter = Allocation.ID.LastIndexOfAny(new[] { '>' });
						this.CompanyAssignedDropDownList.Items.Add(new ListItem(Allocation.ID.Substring(indexOfLastDelimeter + 1), ""));
						this.CompanyAssignedToDropDownList.Items.Add(
							new ListItem(Allocation.ID.Substring(0, indexOfLastDelimeter - 1), Allocation.CompanyMapGuid.ToString()));

						this.Session["Allocation"] = Allocation;
					}

					else
					{
						Allocation = new AllocationClass(this.CurrentSite);

						if (this.Session["CompanyMapType"] == null)
						{
							throw new Exception("Invalid CompanyMapType");
						}

						Allocation.CompanyMapType = (COMPANY_MAP_TYPE)this.Session["CompanyMapType"];

						if (this.Session["AllocationGroupGuid"] != null)
						{
							Allocation.AllocationGroupGuid = (Guid)this.Session["AllocationGroupGuid"];
						}

						// Get the user group allocation status.
						var userGroupAllocationStatus = FMChannelHelper.MakeCall<IAllocations, AllocationClass.UserAllocationStatus>
																(x => x.UserHasAllocationRightsAndCompanyMapCollection(this.Security));

						// Only proceed if the user has allocation rights.
						if (userGroupAllocationStatus != AllocationClass.UserAllocationStatus.DoesNotHaveAllocationRights)
						{
							// Get the groups associated to the user.
							List <GroupClass> userGroupList = FMChannelHelper.MakeCall<IAllocations, List<GroupClass>>(x => x.GetUserGroups(this.Security));

							// CompanyAssignedDropDownList
							var CompanyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
														x => x.EnumerateByType(this.Security, (COMPANY_MAP_TYPE)this.Session["CompanyMapType"]));

							foreach (CompanyMapClass companyMap in CompanyMapCollection)
							{
								var CompanyAssignedItem = new ListItem(companyMap.AssignedID, companyMap.AssignedGuid.ToString());

								if (userGroupAllocationStatus == AllocationClass.UserAllocationStatus.HasGroupMappingToSome)
								{
									bool canViewAlloc = FMChannelHelper.MakeCall<IAllocations, bool>(
														x => x.CanViewAllocation(this.Security, companyMap, (COMPANY_MAP_TYPE)this.Session["CompanyMapType"], userGroupList));

									if (canViewAlloc == false)
									{
										continue;
									}
								}

								var existingItem = this.CompanyAssignedDropDownList.Items.FindByText(companyMap.AssignedID);
								if(existingItem != null)
                                {
									if (existingItem.Text.CompareTo(CompanyAssignedItem.Text) > 0)
									{
										int Index = this.CompanyAssignedDropDownList.Items.IndexOf(existingItem);
										this.CompanyAssignedDropDownList.Items.Insert(Index, CompanyAssignedItem);
										CompanyAssignedItem = null;
									}
									else if (existingItem.Text.CompareTo(CompanyAssignedItem.Text) == 0)
									{
										CompanyAssignedItem = null;
									}
								}

								if (CompanyAssignedItem != null)
								{
									this.CompanyAssignedDropDownList.Items.Add(CompanyAssignedItem);
								}
							}
						}
						

						if (this.CompanyAssignedDropDownList.Items.Count == 0)
						{
							this.Session.Remove( "Allocation" );
							this.Redirect( "AllocationsForm.aspx?Error=NoHierarchies" );
							return;
						}

						this.Session["Allocation"] = Allocation;

						this.CompanyAssignedDropDownList_SelectedIndexChanged(null, null);
					}

					// AllocationGroupDropDownList
					ApplicationStringCollectionClass AllocationGroups =
						FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
							x => x.EnumerateByType(this.Security, STRING_TYPE.ALLOCATION_GROUP));

					ListItem NewAllocationGroupItem;
					
					for (int iItem = 0; iItem < AllocationGroups.Count; iItem++)
					{
						ApplicationStringClass AllocationGroup = AllocationGroups[iItem];

						NewAllocationGroupItem = new ListItem(AllocationGroup.ID, AllocationGroup.IdentityGuid.ToString());

						foreach (ListItem ExistingAllocationGroupItem in this.AllocationGroupsDropDownList.Items)
						{
							if (ExistingAllocationGroupItem.Text.CompareTo(NewAllocationGroupItem.Text) > 0)
							{
								int Index = this.AllocationGroupsDropDownList.Items.IndexOf(ExistingAllocationGroupItem);
								this.AllocationGroupsDropDownList.Items.Insert(Index, NewAllocationGroupItem);
								if (AllocationGroup.IdentityGuid == Allocation.AllocationGroupGuid)
								{
									this.AllocationGroupsDropDownList.SelectedIndex = Index;
								}
								NewAllocationGroupItem = null;
								break;
							}
						}

						if (NewAllocationGroupItem != null)
						{
							this.AllocationGroupsDropDownList.Items.Add(NewAllocationGroupItem);
							if (AllocationGroup.IdentityGuid == Allocation.AllocationGroupGuid)
							{
								this.AllocationGroupsDropDownList.SelectedIndex = this.AllocationGroupsDropDownList.Items.Count - 1;
							}
						}
					}

                    NewAllocationGroupItem = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
                    this.AllocationGroupsDropDownList.Items.Insert(0, NewAllocationGroupItem);

                    this.EffectiveDate.Text = Allocation.EffectiveDate;
					this.ExpirationDate.Text = Allocation.ExpirationDate;
					this.LoadWarningTextbox.Text = Allocation.LoadWarning.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
                    this.LoadDenialTextbox.Text = Allocation.LoadDenial.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
					this.ContractNumberTextbox.Text = Allocation.ContractNumber;
					this.LastAllocationDateTextbox.Text = Allocation.LastAllocationResetDate.ToString();

					this.UpdateLineItemsView();
				}

				else
				{
					// If the Date Controls are disabled there values are not posted
					var Allocation = (AllocationClass)this.Session["Allocation"];
					if (!this.EffectiveDate.Enabled)
					{
						this.EffectiveDate.Text = Allocation.EffectiveDate;
					}
					if (!this.ExpirationDate.Enabled)
					{
						this.ExpirationDate.Text = Allocation.ExpirationDate;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler( except );
			}
		}

        private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.UpdateAllocationDates();
			var Allocation = (AllocationClass)this.Session["Allocation"];
			AllocationLineItemCollectionClass LineItems = Allocation.LineItemCollection;
			var LineItem = new AllocationLineItemClass(this.CurrentSite);

			DateTimeOffset siteTimeToday = TimeConverter.Today(this.CurrentSite);
			LineItem.SetResetDate(Allocation._EffectiveDate.Value, Allocation._ExpirationDate.Value, siteTimeToday);
			this.AssignedGuid = LineItem.AssignedGuid;
			this.LimitText = LineItem.Limit.ToString();
			this.NextText = LineItem.Next.ToString();
			this.ResetPeriod = LineItem.ResetPeriod;
			this.ResetMultiple = LineItem.ResetMultiple;
			this.ResetMethod = LineItem.ResetMethod;
			LineItems.Add(LineItem);
			this.LineItemsDataGrid.CurrentPageIndex = (LineItems.Count - 1) / this.LineItemsDataGrid.PageSize;
			this.LineItemsDataGrid.EditItemIndex = (LineItems.Count - 1) % this.LineItemsDataGrid.PageSize;
			this.AddButton.Enabled = false;
			this.Type = LineItem.Type;
			try
			{
				this.EnableControls(false);
				this.UpdateLineItemsView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.EnableControls(true);
				LineItems.Remove(LineItems.Count - 1);
				if (this.LineItemsDataGrid.CurrentPageIndex > 0 && this.LineItemsDataGrid.EditItemIndex == 0)
				{
					this.LineItemsDataGrid.CurrentPageIndex--;
				}
				this.LineItemsDataGrid.EditItemIndex = -1;
				this.UpdateLineItemsView();
			}
		}

		private void Cancel_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("Allocation");
			this.Redirect("AllocationsForm.aspx");
		}

		private ICollection EnumerateLineItems()
		{
			var Allocation = (AllocationClass)this.Session["Allocation"];

			var LineItemsDataTable = new DataTable();
			DataRow LineItemDataRow;

			LineItemsDataTable.Columns.Add("Index", typeof(int));
			LineItemsDataTable.Columns.Add("Type", typeof(string));
			LineItemsDataTable.Columns.Add("ID", typeof(string));
			LineItemsDataTable.Columns.Add("Limit", typeof(string));
			LineItemsDataTable.Columns.Add("Loaded", typeof(string));
			LineItemsDataTable.Columns.Add("Next", typeof(string));
			LineItemsDataTable.Columns.Add("ResetPeriod", typeof(string));
			LineItemsDataTable.Columns.Add("ResetMultiple", typeof(string));
			LineItemsDataTable.Columns.Add("ResetMethod", typeof(string));
			LineItemsDataTable.Columns.Add("ResetDate", typeof(string));

			int Item = 0;
			foreach (AllocationLineItemClass LineItem in Allocation.LineItemCollection)
			{
				LineItemDataRow = LineItemsDataTable.NewRow();

				LineItemDataRow["Index"] = Item;
				LineItemDataRow["Type"] = this.GetTranslatedText(AllocationLineItemClass.TypeID(LineItem.Type));
				LineItemDataRow["ID"] = LineItem.AssignedID;
				LineItemDataRow["Limit"] = LineItem.Limit.ToString();
				LineItemDataRow["Loaded"] = LineItem.Loaded.ToString();
				if (LineItem.ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD)
				{
					LineItemDataRow["Next"] = "";
					LineItemDataRow["ResetPeriod"] = "";
					LineItemDataRow["ResetMultiple"] = "";
				}
				else
				{
					LineItemDataRow["Next"] = LineItem.Next.ToString();
					LineItemDataRow["ResetPeriod"] = this.GetTranslatedText(
						AllocationLineItemClass.ResetPeriodID(LineItem.ResetPeriod));
					LineItemDataRow["ResetMultiple"] = LineItem.ResetMultiple;
				}
				LineItemDataRow["ResetMethod"] = this.GetTranslatedText(AllocationLineItemClass.ResetMethodID(LineItem.ResetMethod));
				LineItemDataRow["ResetDate"] = LineItem.ResetDate.ToString();

				LineItemsDataTable.Rows.Add(LineItemDataRow);
				Item++;
			}
			var LineItemsDataView = new DataView(LineItemsDataTable);
			return LineItemsDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CompanyAssignedDropDownList.SelectedIndexChanged += this.CompanyAssignedDropDownList_SelectedIndexChanged;
			this.CompanyAssignedToDropDownList.SelectedIndexChanged += this.CompanyAssignedToDropDownList_SelectedIndexChanged;
			this.RefreshButton.Command += this.RefreshButton_Command;
			this.ResetButton.Command += this.ResetButton_Command;
			this.Cancel.Command += this.Cancel_Command;
			this.OK.Command += this.OK_Command;
			this.LineItemsDataGrid.EditCommand += this.LineItemsDataGrid_EditCommand;
			this.LineItemsDataGrid.PageIndexChanged += this.LineItemsDataGrid_PageIndexChanged;
			this.LineItemsDataGrid.CancelCommand += this.LineItemsDataGrid_CancelCommand;
			this.LineItemsDataGrid.UpdateCommand += this.LineItemsDataGrid_UpdateCommand;
			this.LineItemsDataGrid.DeleteCommand += this.LineItemsDataGrid_DeleteCommand;
			this.LineItemsDataGrid.ItemDataBound += this.LineItemsDataGrid_ItemDataBound;
			this.AddButton.Command += this.AddButton_Command;
		}

		private void LineItemsDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				var Allocation = (AllocationClass)this.Session["Allocation"];
				AllocationLineItemCollectionClass LineItems;
				LineItems = Allocation.LineItemCollection;
				AllocationLineItemClass LineItem = LineItems.Item(Convert.ToInt32(IndexLabel.Text));
				if (LineItem.AssignedID == "" && LineItem.Type != ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION)
				{
					LineItems.Remove(Convert.ToInt32(IndexLabel.Text));
					if (this.LineItemsDataGrid.Items.Count == 1 && this.LineItemsDataGrid.CurrentPageIndex > 0)
					{
						this.LineItemsDataGrid.CurrentPageIndex--;
					}
				}
				this.EnableControls(true);
				this.LineItemsDataGrid.EditItemIndex = -1;
				this.UpdateLineItemsView();
			}
		}

		private void LineItemsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				var Allocation = (AllocationClass)this.Session["Allocation"];
				AllocationLineItemCollectionClass LineItems;
				LineItems = Allocation.LineItemCollection;

				if (this.LineItemsDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.LineItemsDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (this.LineItemsDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.LineItemsDataGrid.EditItemIndex--;
				}

				LineItems.Remove(Convert.ToInt32(IndexLabel.Text));
				if (this.LineItemsDataGrid.Items.Count == 1 && this.LineItemsDataGrid.CurrentPageIndex > 0)
				{
					this.LineItemsDataGrid.CurrentPageIndex--;
				}
				this.UpdateLineItemsView();
			}
		}

		private void LineItemsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				var Allocation = (AllocationClass)this.Session["Allocation"];
				AllocationLineItemCollectionClass LineItems = Allocation.LineItemCollection;
				AllocationLineItemClass LineItem = LineItems.Item(Convert.ToInt32(IndexLabel.Text));
				this.Type = LineItem.Type;
				this.AssignedGuid = LineItem.AssignedGuid;
				this.LimitText = LineItem.Limit.ToString();
				this.NextText = LineItem.Next.ToString();
				this.ResetPeriod = LineItem.ResetPeriod;
				this.ResetMultiple = LineItem.ResetMultiple;
				this.ResetMethod = LineItem.ResetMethod;
				this.LineItemsDataGrid.EditItemIndex = e.Item.ItemIndex;
				this.UpdateLineItemsView();
				this.EnableControls(false);
			}
		}

		private void LineItemsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			// CSI 5815 - Disable the datagrid edit & delete buttons if no modify right.
			var EditButton = (LinkButton)e.Item.FindControl("EditButton");
			if (EditButton != null)
			{
				EditButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_ALLOCATIONS);
			}

			var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			if (DeleteButton != null)
			{
				DeleteButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_ALLOCATIONS);
			}

			var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (IndexLabel != null)
			{
				var IDDropDownList = (DropDownList)e.Item.FindControl("IDDropDownList");
				if (IDDropDownList != null)
				{
					var Allocation = (AllocationClass)this.Session["Allocation"];
					AllocationLineItemCollectionClass LineItems;
					LineItems = Allocation.LineItemCollection;
					AllocationLineItemClass LineItem = LineItems.Item(Convert.ToInt32(IndexLabel.Text));

					var TypeDropDownList = (FMDropDownList)e.Item.FindControl("TypeDropDownList");
					if (TypeDropDownList != null)
					{
						ListItemCollection Items = TypeDropDownList.Items;
						int Index = Items.IndexOf(Items.FindByValue(((int)this.Type).ToString()));
						TypeDropDownList.SelectedIndex = Index;
					}

					if (this.Type != ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION)
					{
						if (this.Type == LineItem.Type && this.AssignedGuid != Guid.Empty)
						{
							ListItemCollection Items = IDDropDownList.Items;
							int Index = Items.IndexOf(Items.FindByValue(this.AssignedGuid.ToString()));
							IDDropDownList.SelectedIndex = Index;
						}
					}
					else
					{
						IDDropDownList.Visible = false;
					}

					var LimitTextBox = (TextBox)e.Item.FindControl("LimitTextBox");
					if (LimitTextBox != null)
					{
						LimitTextBox.Text = this.LimitText;
					}

					var NextTextBox = (TextBox)e.Item.FindControl("NextTextBox");
					if (NextTextBox != null)
					{
						NextTextBox.Text = this.NextText;
					}

					var ResetPeriodDropDownList = (FMDropDownList)e.Item.FindControl("ResetPeriodDropDownList");
					if (ResetPeriodDropDownList != null)
					{
						ListItemCollection Items = ResetPeriodDropDownList.Items;
						int Index = Items.IndexOf(Items.FindByValue(((int)this.ResetPeriod).ToString()));
						ResetPeriodDropDownList.SelectedIndex = Index;
					}

					var ResetMultipleTextBox = (TextBox)e.Item.FindControl("ResetMultipleTextBox");
					if (ResetMultipleTextBox != null)
					{
						ResetMultipleTextBox.Text = this.ResetMultiple.ToString();
					}

					var ResetMethodDropDownList = (FMDropDownList)e.Item.FindControl("ResetMethodDropDownList");
					if (ResetMethodDropDownList != null)
					{
						ListItemCollection Items = ResetMethodDropDownList.Items;
						int Index = Items.IndexOf(Items.FindByValue(((int)this.ResetMethod).ToString()));
						ResetMethodDropDownList.SelectedIndex = Index;

						if (this.ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD)
						{
							if (LimitTextBox != null)
							{
								LimitTextBox.Visible = false;
							}
							if (NextTextBox != null)
							{
								NextTextBox.Visible = false;
							}
							if (ResetPeriodDropDownList != null)
							{
								ResetPeriodDropDownList.Visible = false;
							}
							if (ResetMultipleTextBox != null)
							{
								ResetMultipleTextBox.Visible = false;
							}

							var ResetDateLabel = (Label)e.Item.FindControl("ResetDateLabel");
							if (ResetDateLabel != null)
							{
								LineItem.ResetDate.Value = TimeConverter.Today(this.CurrentSite);
								ResetDateLabel.Text = LineItem.ResetDate.ToString();
							}
						}
					}
				}
			}
		}

		private void LineItemsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.LineItemsDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.LineItemsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateLineItemsView();
		}

		private void LineItemsDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var IndexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (IndexLabel != null)
				{
					int Index = Convert.ToInt32(IndexLabel.Text);
					var Allocation = (AllocationClass)this.Session["Allocation"];
					AllocationLineItemCollectionClass LineItems = Allocation.LineItemCollection;
					AllocationLineItemClass LineItem = LineItems.Item(Index);

					ALLOCATION_TYPE Type = LineItem.Type;
					Guid assignedGuid = LineItem.AssignedGuid;
					ALLOCATION_RESET_PERIOD ResetPeriod = LineItem.ResetPeriod;
					long ResetMultiple = LineItem.ResetMultiple;
					ALLOCATION_RESET_METHOD ResetMethod = LineItem.ResetMethod;

					var TypeDropDownList = (FMDropDownList)e.Item.FindControl("TypeDropDownList");
					LineItem.Type = (ALLOCATION_TYPE)Convert.ToInt32(TypeDropDownList.SelectedValue);

					var IDDropDownList = (DropDownList)e.Item.FindControl("IDDropDownList");
					if (LineItem.Type == ALLOCATION_TYPE.PRODUCT_ALLOCATION
					    || LineItem.Type == ALLOCATION_TYPE.PRODUCT_GROUP_ALLOCATION)
					{
						LineItem.AssignedGuid = Guid.Parse(IDDropDownList.SelectedValue);
						LineItem.AssignedID = IDDropDownList.SelectedItem.Text;
					}
					else
					{
						LineItem.AssignedGuid = Guid.Empty;
						LineItem.AssignedID = "";
					}

					var LimitTextBox = (TextBox)e.Item.FindControl("LimitTextBox");
					LineItem.Limit.Value = Convert.ToDouble(LimitTextBox.Text, LineItem.Limit.Format);

					var NextTextBox = (TextBox)e.Item.FindControl("NextTextBox");
					LineItem.Next.Value = Convert.ToDouble(NextTextBox.Text, LineItem.Next.Format);

					var ResetPeriodDropDownList = (FMDropDownList)e.Item.FindControl("ResetPeriodDropDownList");
					LineItem.ResetPeriod = (ALLOCATION_RESET_PERIOD)Convert.ToInt32(ResetPeriodDropDownList.SelectedValue);

					var ResetMultipleTextBox = (TextBox)e.Item.FindControl("ResetMultipleTextBox");
					LineItem.ResetMultiple = Convert.ToInt32(ResetMultipleTextBox.Text);

					var ResetMethodDropDownList = (FMDropDownList)e.Item.FindControl("ResetMethodDropDownList");
					LineItem.ResetMethod = (ALLOCATION_RESET_METHOD)Convert.ToInt32(ResetMethodDropDownList.SelectedValue);

					if (Type != LineItem.Type || assignedGuid != LineItem.AssignedGuid || ResetPeriod != LineItem.ResetPeriod
					    || ResetMultiple != LineItem.ResetMultiple || ResetMethod != LineItem.ResetMethod)
					{
						int Item = -1;
						foreach (AllocationLineItemClass ExistingLineItem in Allocation.LineItemCollection)
						{
							Item++;

							if (Item != Index && LineItem.Type == ExistingLineItem.Type
							    && LineItem.AssignedGuid == ExistingLineItem.AssignedGuid
							    && LineItem.ResetPeriod == ExistingLineItem.ResetPeriod)
							{
								throw new Exception("Duplicate Allocation Line Item");
							}
						}

						DateTimeOffset siteTimeToday = TimeConverter.Today(this.CurrentSite);

						LineItem.Loaded.Value = 0.0;
						LineItem.SetResetDate(Allocation._EffectiveDate.Value, Allocation._ExpirationDate.Value, siteTimeToday);

						LineItem.Loaded.Value =
							FMChannelHelper.MakeCall<IAllocationLineItems, double>(
								x =>
								x.GetAmountLoaded(
									this.Security,
									Allocation.ID,
									LineItem.AssignedGuid,
									LineItem.Type,
									LineItem.ResetPeriod,
									LineItem.ResetMultiple,
									LineItem.ResetDate.Value,
									Allocation.LastAllocationResetDate.Value,
									Allocation._ExpirationDate.Value,
									this.Security.SiteGuid,
									STATION_TYPE.MAX_STATION_TYPE,
									""));

						if (LineItem.ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD)
						{
							CompanyMapClass OwnerManagerMap =
								FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
									x => x.Get(this.Security, Allocation.CompanyMapGuid, Allocation.CompanyMapType));

							ProductMapCollectionClass UnavailableProductMapCollection =
								FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
									x =>
									x.EnumerateByAssignedToGuidAndType(
										this.Security, OwnerManagerMap.AssignedGuid, PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP));

							Guid companyGuid =
							FMChannelHelper.MakeCall<IFieldLevelConfigMaps, Guid>(
							x => x.GetRecordVersionGuid(this.Security, "COMPANY", OwnerManagerMap.AssignedGuid, this.Security.SiteGuid));

							Guid productGuid =
							FMChannelHelper.MakeCall<IFieldLevelConfigMaps, Guid>(
							x => x.GetRecordVersionGuid(this.Security, "PRODUCT", LineItem.AssignedGuid, this.Security.SiteGuid));

							//OwnerManagerMap: Assigned: company/owner; AssignedTo: Manager
							//UnavailableProductMapCollection: Assigned: product; AssignedTo: company/owner
							ProductMapClass UnavailableProductMap =UnavailableProductMapCollection.Find(x => x.AssignedToGuid == companyGuid && x.AssignedGuid == productGuid);

							// Force UserGuid to Guid.Empty all companies authorized
							//this.Security.UserGuid = Guid.Empty;

							// Get the ledger data
							var ledgerSR = new LedgerSR
							{
								Security = this.Security,
								Site = this.CurrentSite.ID,
								CurrentSiteGuid = this.CurrentSite.IdentityGuid
							};
							ledgerSR.SetRequestType(LedgerSR.LedgerRequests.Refresh);
							ledgerSR.Manager = OwnerManagerMap.AssignedToID;
							ledgerSR.Owner = OwnerManagerMap.AssignedID;
							ledgerSR.Product = LineItem.AssignedID;
							ledgerSR.Month = LineItem.ResetDate.Value.ToString("MMMM yyyy");
							ledgerSR.Units = QuantityDisplay.NET;
							ledgerSR.ShowCost = false;

							LedgerDO ledgerDO = FMChannelHelper.MakeCall<ILedgerProcessor, LedgerDO>(x => x.Process(ledgerSR));

							var ledgerLineItemDO = ledgerDO.LedgerLineItems[LineItem.ResetDate.Value.Day - 1] as LedgerLineItemDO;
							LineItem.Limit.Value = ledgerLineItemDO.BookInventory.NetInventoryChange;
							LineItem.Limit.Value += LineItem.Loaded.Value;

							if (UnavailableProductMap != null)
							{
								LineItem.Limit.Value -= UnavailableProductMap._UnavailableInventoryNet.Value;
							}
						}
					}

					this.EnableControls(true);
					this.LineItemsDataGrid.EditItemIndex = -1;
					this.UpdateLineItemsView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void OK_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				this.UpdateAllocationDates();

				var Allocation = (AllocationClass)this.Session["Allocation"];

                Allocation.LoadWarning = Convert.ToDouble(this.LoadWarningTextbox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
                Allocation.LoadDenial = Convert.ToDouble(this.LoadDenialTextbox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				Allocation.ContractNumber = this.ContractNumberTextbox.Text;
				if (this.AllocationGroupsDropDownList.SelectedIndex != -1)
				{
					Allocation.AllocationGroupGuid = Guid.Parse(this.AllocationGroupsDropDownList.SelectedValue);
				}

				Allocation.AllocationGroupID = this.AllocationGroupsDropDownList.SelectedItem.Text;

				this.Session["AllocationGroupGuid"] = Allocation.AllocationGroupGuid;

				if (Allocation.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IAllocations>(x => x.Modify(this.Security, Allocation));
				}
				else
				{
					FMChannelHelper.MakeCall<IAllocations, Guid>(x => x.Add(this.Security, Allocation));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}
			this.Session.Remove("Allocation");
			this.Redirect("AllocationsForm.aspx");
		}

		private void RefreshButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				var Allocation = (AllocationClass)this.Session["Allocation"];

				// For Owner type allocation we need to retrieve from the database to deal with the Book-Unavailable cases
				if ( Allocation.CompanyMapType == COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP && this.Session["IdentityGuid"] != null) {
					Allocation =
						FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
							x => x.Get(this.Security, Guid.Parse(this.Session["IdentityGuid"] as string), STATION_TYPE.MAX_STATION_TYPE, ""));

					this.CompanyAssignedDropDownList.Enabled = false;
					this.CompanyAssignedToDropDownList.Enabled = false;

					int indexOfLastDelimeter = Allocation.ID.LastIndexOfAny(new[] { '>' });
					this.CompanyAssignedDropDownList.Items.Add(new ListItem(Allocation.ID.Substring(indexOfLastDelimeter + 1), ""));
					this.CompanyAssignedToDropDownList.Items.Add(
						new ListItem(Allocation.ID.Substring(0, indexOfLastDelimeter - 1), Allocation.CompanyMapGuid.ToString()));

					this.Session["Allocation"] = Allocation;
				}

				bool DatesChanged = false;
				if (Allocation.EffectiveDate != this.EffectiveDate.Text || Allocation.ExpirationDate != this.ExpirationDate.Text)
				{
					Allocation.EffectiveDate = this.EffectiveDate.Text;
					Allocation.ExpirationDate = this.ExpirationDate.Text;
					DatesChanged = true;
				}

				DateTimeOffset siteTimeToday = TimeConverter.Today(this.CurrentSite);

				foreach (AllocationLineItemClass LineItem in Allocation.LineItemCollection)
				{
					if (DatesChanged)
					{
						LineItem.Loaded.Value = 0;
					}
					LineItem.SetResetDate(Allocation._EffectiveDate.Value, Allocation._ExpirationDate.Value, siteTimeToday);
					LineItem.Loaded.Value = this.GetAmountLoaded(
						this.Security,
						Allocation.ID,
						LineItem.AssignedGuid,
						LineItem.Type,
						LineItem.ResetPeriod,
						LineItem.ResetMultiple,
						LineItem.ResetDate.Value,
						LineItem.ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD ? LineItem.ResetDate.Value : Allocation.LastAllocationResetDate.Value,
						Allocation._ExpirationDate.Value,
						this.Security.SiteGuid,
						STATION_TYPE.MAX_STATION_TYPE,
						"");
				}

				this.UpdateLineItemsView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ResetButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				DateTimeOffset siteTimeToday = TimeConverter.Today(this.CurrentSite);

				var Allocation = (AllocationClass)this.Session["Allocation"];
				Allocation.LastAllocationResetDate.Value = siteTimeToday;
				this.LastAllocationDateTextbox.Text = Allocation.LastAllocationResetDate.ToString();

				bool DatesChanged = false;
				if (Allocation.EffectiveDate != this.EffectiveDate.Text || Allocation.ExpirationDate != this.ExpirationDate.Text)
				{
					Allocation.EffectiveDate = this.EffectiveDate.Text;
					Allocation.ExpirationDate = this.ExpirationDate.Text;
					DatesChanged = true;
				}

				foreach (AllocationLineItemClass LineItem in Allocation.LineItemCollection)
				{
					if (DatesChanged)
					{
						LineItem.Loaded.Value = 0;
					}
					LineItem.SetResetDate(Allocation._EffectiveDate.Value, Allocation._ExpirationDate.Value, siteTimeToday);
					LineItem.Loaded.Value = this.GetAmountLoaded(
						this.Security,
						Allocation.ID,
						LineItem.AssignedGuid,
						LineItem.Type,
						LineItem.ResetPeriod,
						LineItem.ResetMultiple,
						LineItem.ResetDate.Value,
						LineItem.ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD ? LineItem.ResetDate.Value : Allocation.LastAllocationResetDate.Value,
						Allocation._ExpirationDate.Value,
						this.Security.SiteGuid,
						STATION_TYPE.MAX_STATION_TYPE,
						"");
				}
				this.UpdateLineItemsView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateAllocationDates()
		{
			var Allocation = (AllocationClass)this.Session["Allocation"];

			if (Allocation.EffectiveDate != this.EffectiveDate.Text || Allocation.ExpirationDate != this.ExpirationDate.Text)
			{
				Allocation.EffectiveDate = this.EffectiveDate.Text;
				Allocation.ExpirationDate = this.ExpirationDate.Text;

				DateTimeOffset siteTimeToday = TimeConverter.Today(this.CurrentSite);

				foreach (AllocationLineItemClass LineItem in Allocation.LineItemCollection)
				{
					LineItem.Loaded.Value = 0;
					LineItem.SetResetDate(Allocation._EffectiveDate.Value, Allocation._ExpirationDate.Value, siteTimeToday);
					LineItem.Loaded.Value = this.GetAmountLoaded(
						this.Security,
						Allocation.ID,
						LineItem.AssignedGuid,
						LineItem.Type,
						LineItem.ResetPeriod,
						LineItem.ResetMultiple,
						LineItem.ResetDate.Value,
						LineItem.ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD ? LineItem.ResetDate.Value : Allocation.LastAllocationResetDate.Value,
						Allocation._ExpirationDate.Value,
						this.Security.SiteGuid,
						STATION_TYPE.MAX_STATION_TYPE,
						"");
				}
			}
		}

		private void UpdateLineItemsView()
		{
			this.LineItemsDataGrid.DataSource = this.EnumerateLineItems();
			this.LineItemsDataGrid.DataBind();
		}

		#endregion
	}
}
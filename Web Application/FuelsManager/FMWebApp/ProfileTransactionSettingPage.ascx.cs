// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileTransactionSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfileTransactionSettingPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	/// <summary>
	///    This class is the code behind to handle the control of the Profile Transaction
	///    page that is part of a multi-tab page.
	/// </summary>
	public partial class ProfileTransactionSettingPage : FMUserControlBase
	{
		#region Constants and Fields

		private List<ListItem> billToList;

		private List<ListItem> carrierList;

		private List<ListItem> deIceList;

		private List<ListItem> defuelList;

		private List<ListItem> gseList;

		private List<ListItem> issueList;

		private List<ListItem> managerList;

		private List<ListItem> meterCloseoutList;

		/// <summary>
		///    The mobile device profile.
		/// </summary>
		private MobileDeviceProfile mobileDeviceProfile;

		private List<ListItem> ownerList;

		private List<ListItem> productList;

		private List<ListItem> rotationList;

		private List<ListItem> shipToList;

		private List<ListItem> shipperList;

		private List<ListItem> supplierList;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will reset all the fields when the new button is
		///    selected.
		/// </summary>
		public void ResetFieldsForNewEvent()
		{
			this.GetTransactionAliases();
			this.BuildTransactionDropdowns();

			this.GetCompanies();
			this.BuildCompanyDropdowns();

			this.GetProducts();
			this.BuildProductDropdowns();

			this.UpdateView();
			this.DisableFields();
		}

		/// <summary>
		///    This method will update the profile configuration table from the OPs page.
		/// </summary>
		public void UpdateChanges()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			this.mobileDeviceProfile.IssueTransaction = Guid.Parse(this.IssueTransactionDD.SelectedItem.Value);
			this.mobileDeviceProfile.DefuelTransaction = Guid.Parse(this.DefuelTransactionDD.SelectedItem.Value);
			this.mobileDeviceProfile.DeIceTransaction = Guid.Parse(this.DefuelTransactionDD.SelectedItem.Value);
			this.mobileDeviceProfile.GseTransaction = Guid.Parse(this.GSETransactionDD.SelectedItem.Value);
			this.mobileDeviceProfile.MeterCloseout = Guid.Parse(this.MeterCloseoutDD.SelectedItem.Value);
			this.mobileDeviceProfile.RotationTransaction = Guid.Parse(this.RotationTransactionDD.SelectedItem.Value);

			this.mobileDeviceProfile.ManualBillTo = Guid.Parse(this.BillToDD.SelectedItem.Value);
			this.mobileDeviceProfile.ManualConsumer = Guid.Parse(this.ConsumerManualDD.SelectedItem.Value);
			this.mobileDeviceProfile.ManualManager = Guid.Parse(this.ManagerDD.SelectedItem.Value);
			this.mobileDeviceProfile.ManualShipper = Guid.Parse(this.ShipperDD.SelectedItem.Value);
			this.mobileDeviceProfile.ManualSupplier = Guid.Parse(this.SupplierDD.SelectedItem.Value);
			this.mobileDeviceProfile.ManualVendor = Guid.Parse(this.VendorManualDD.SelectedItem.Value);
			this.mobileDeviceProfile.CloseoutConsumer = Guid.Parse(this.ConsumerCloseoutDD.SelectedItem.Value);
			this.mobileDeviceProfile.CloseoutOwner = Guid.Parse(this.OwnerDD.SelectedItem.Value);
			this.mobileDeviceProfile.CloseoutVendor = Guid.Parse(this.VendorCloseoutDD.SelectedItem.Value);
			this.mobileDeviceProfile.ManualProduct = Guid.Parse(this.ProductDD.SelectedItem.Value);

			this.mobileDeviceProfile.InhibitOverridingTemperature = this.InhibitOverridingTemperatureCB.Checked;

			this.mobileDeviceProfile.ManualTemperature = null;
			if (string.IsNullOrEmpty(this.TemperatureTB.Text) == false)
			{
				try
				{
					this.mobileDeviceProfile.ManualTemperature = Convert.ToDouble(this.TemperatureTB.Text);
				}
				catch (Exception)
				{
					string errMsg = "Temperature must be numeric.";
					throw new Exception(errMsg);
				}
			}

			this.mobileDeviceProfile.ManualDensity = null;
			if (string.IsNullOrEmpty(this.DensityTB.Text) == false)
			{
				try
				{
					this.mobileDeviceProfile.ManualDensity = Convert.ToDouble(this.DensityTB.Text);
				}
				catch (Exception)
				{
					string errMsg = "Density must be numeric.";
					throw new Exception(errMsg);
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method handles the inhibit override temperature checkbox change.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void InhibitTempCheckedChanged(object sender, EventArgs e)
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			if (this.InhibitOverridingTemperatureCB.Checked)
			{
				this.TemperatureTB.Enabled = true;
				this.DensityTB.Enabled = true;
				this.DensityLB.Enabled = true;
				this.TemperatureLB.Enabled = true;

				this.mobileDeviceProfile.InhibitOverridingTemperature = true;
			}
			else
			{
				this.TemperatureTB.Text = string.Empty;
				this.DensityTB.Text = string.Empty;

				this.TemperatureTB.Enabled = false;
				this.TemperatureLB.Enabled = false;
				this.DensityTB.Enabled = false;
				this.DensityLB.Enabled = false;

				this.mobileDeviceProfile.ManualTemperature = null;
				this.mobileDeviceProfile.ManualDensity = null;
				this.mobileDeviceProfile.InhibitOverridingTemperature = false;
			}
		}

		/// <summary>
		///    This method will handle the page load event for the transaction page.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.ApplyDataDictionaryToPanes();

			if (this.Page.IsPostBack == false)
			{
				this.GetTransactionAliases();
				this.BuildTransactionDropdowns();

				this.GetCompanies();
				this.BuildCompanyDropdowns();

				this.GetProducts();
				this.BuildProductDropdowns();

				this.UpdateView();
			}

			this.DisableFields();
		}

		/// <summary>
		///    This method applies the data dictionary to the Grouping Pane text.
		/// </summary>
		private void ApplyDataDictionaryToPanes()
		{
			this.TxAssociationPanel.GroupingText = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "Transaction Association");
			this.DefaultManualTransaction.GroupingText = this.GetDataDictionaryValueByKey(
				this.Security.SiteGuid, "Default Value for Manual Transaction");
			this.DefaultMeterCloseout.GroupingText = this.GetDataDictionaryValueByKey(
				this.Security.SiteGuid, "Default Value for Meter Closeout");
		}

		/// <summary>
		///    This method builds the company dropdowns and sets the selected item.
		/// </summary>
		private void BuildCompanyDropdowns()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			this.ManagerDD.DataSource = this.managerList;
			this.ManagerDD.DataTextField = "Text";
			this.ManagerDD.DataValueField = "Value";
			this.ManagerDD.Sort = false;
			this.DataBind();

			this.OwnerDD.DataSource = this.ownerList;
			this.OwnerDD.DataTextField = "Text";
			this.OwnerDD.DataValueField = "Value";
			this.OwnerDD.Sort = false;
			this.DataBind();

			this.BillToDD.DataSource = this.billToList;
			this.BillToDD.DataTextField = "Text";
			this.BillToDD.DataValueField = "Value";
			this.BillToDD.Sort = false;
			this.DataBind();

			this.ConsumerCloseoutDD.DataSource = this.shipToList;
			this.ConsumerCloseoutDD.DataTextField = "Text";
			this.ConsumerCloseoutDD.DataValueField = "Value";
			this.ConsumerCloseoutDD.Sort = false;
			this.DataBind();

			this.ConsumerManualDD.DataSource = this.shipToList;
			this.ConsumerManualDD.DataTextField = "Text";
			this.ConsumerManualDD.DataValueField = "Value";
			this.ConsumerManualDD.Sort = false;
			this.DataBind();

			this.SupplierDD.DataSource = this.supplierList;
			this.SupplierDD.DataTextField = "Text";
			this.SupplierDD.DataValueField = "Value";
			this.SupplierDD.Sort = false;
			this.DataBind();

			this.ShipperDD.DataSource = this.shipperList;
			this.ShipperDD.DataTextField = "Text";
			this.ShipperDD.DataValueField = "Value";
			this.ShipperDD.Sort = false;
			this.DataBind();

			this.VendorManualDD.DataSource = this.carrierList;
			this.VendorManualDD.DataTextField = "Text";
			this.VendorManualDD.DataValueField = "Value";
			this.VendorManualDD.Sort = false;
			this.DataBind();

			this.VendorCloseoutDD.DataSource = this.carrierList;
			this.VendorCloseoutDD.DataTextField = "Text";
			this.VendorCloseoutDD.DataValueField = "Value";
			this.VendorCloseoutDD.Sort = false;
			this.DataBind();

			this.ManagerDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.ManualManager != Guid.Empty)
			{
				this.ManagerDD.SelectedIndex = this.FindSelectedIndex(this.managerList, this.mobileDeviceProfile.ManualManager);
			}

			this.OwnerDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.CloseoutOwner != Guid.Empty)
			{
				this.OwnerDD.SelectedIndex = this.FindSelectedIndex(this.ownerList, this.mobileDeviceProfile.CloseoutOwner);
			}

			this.BillToDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.ManualBillTo != Guid.Empty)
			{
				this.BillToDD.SelectedIndex = this.FindSelectedIndex(this.billToList, this.mobileDeviceProfile.ManualBillTo);
			}

			this.ConsumerManualDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.ManualConsumer != Guid.Empty)
			{
				this.ConsumerManualDD.SelectedIndex = this.FindSelectedIndex(
					this.shipToList, this.mobileDeviceProfile.ManualConsumer);
			}

			this.ConsumerCloseoutDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.CloseoutConsumer != Guid.Empty)
			{
				this.ConsumerCloseoutDD.SelectedIndex = this.FindSelectedIndex(
					this.shipToList, this.mobileDeviceProfile.CloseoutConsumer);
			}

			this.ShipperDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.ManualShipper != Guid.Empty)
			{
				this.ShipperDD.SelectedIndex = this.FindSelectedIndex(this.shipperList, this.mobileDeviceProfile.ManualShipper);
			}

			this.SupplierDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.ManualSupplier != Guid.Empty)
			{
				this.SupplierDD.SelectedIndex = this.FindSelectedIndex(this.supplierList, this.mobileDeviceProfile.ManualSupplier);
			}

			this.VendorCloseoutDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.CloseoutVendor != Guid.Empty)
			{
				this.VendorCloseoutDD.SelectedIndex = this.FindSelectedIndex(
					this.carrierList, this.mobileDeviceProfile.CloseoutVendor);
			}

			this.VendorManualDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.ManualVendor != Guid.Empty)
			{
				this.VendorManualDD.SelectedIndex = this.FindSelectedIndex(this.carrierList, this.mobileDeviceProfile.ManualVendor);
			}
		}

		/// <summary>
		///    This method is a helper method to build the company lists.
		/// </summary>
		/// <param name="companyCollection">
		///    The company collection.
		/// </param>
		/// <returns>
		///    The System.Collections.Generic.List`1[T -&gt; System.Web.UI.WebControls.ListItem].
		/// </returns>
		private List<ListItem> BuildCompanyList(CompanyCollectionClass companyCollection)
		{
			var companyList = new List<ListItem>();

			foreach (CompanyClass company in companyCollection)
			{
				var item = new ListItem { Text = company.ID, Value = company.MasterRecordGuid.ToString() };
				companyList.Add(item);
			}

			companyList = companyList.OrderBy(item => item.Text).ToList();

			var noneItem = new ListItem { Text = "None", Value = Guid.Empty.ToString() };
			companyList.Insert(0, noneItem);

			return companyList;
		}

		/// <summary>
		///    This method builds the product dropdowns and sets the selected item.
		/// </summary>
		private void BuildProductDropdowns()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			this.ProductDD.DataSource = this.productList;
			this.ProductDD.DataTextField = "Text";
			this.ProductDD.DataValueField = "Value";
			this.ProductDD.Sort = false;
			this.DataBind();

			this.ProductDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.ManualProduct != Guid.Empty)
			{
				this.ProductDD.SelectedIndex = this.FindSelectedIndex(this.managerList, this.mobileDeviceProfile.ManualProduct);
			}
		}

		/// <summary>
		///    This method builds the transaction dropdowns and sets the selected item.
		/// </summary>
		private void BuildTransactionDropdowns()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			this.IssueTransactionDD.DataSource = this.issueList;
			this.IssueTransactionDD.DataTextField = "Text";
			this.IssueTransactionDD.DataValueField = "Value";
			this.IssueTransactionDD.Sort = false;
			this.DataBind();

			this.DefuelTransactionDD.DataSource = this.defuelList;
			this.DefuelTransactionDD.DataTextField = "Text";
			this.DefuelTransactionDD.DataValueField = "Value";
			this.DefuelTransactionDD.Sort = false;
			this.DataBind();

			this.RotationTransactionDD.DataSource = this.rotationList;
			this.RotationTransactionDD.DataTextField = "Text";
			this.RotationTransactionDD.DataValueField = "Value";
			this.RotationTransactionDD.Sort = false;
			this.DataBind();

			this.MeterCloseoutDD.DataSource = this.meterCloseoutList;
			this.MeterCloseoutDD.DataTextField = "Text";
			this.MeterCloseoutDD.DataValueField = "Value";
			this.MeterCloseoutDD.Sort = false;
			this.DataBind();

			this.DeIceTransactionDD.DataSource = this.deIceList;
			this.DeIceTransactionDD.DataTextField = "Text";
			this.DeIceTransactionDD.DataValueField = "Value";
			this.DeIceTransactionDD.Sort = false;
			this.DataBind();

			this.GSETransactionDD.DataSource = this.gseList;
			this.GSETransactionDD.DataTextField = "Text";
			this.GSETransactionDD.DataValueField = "Value";
			this.GSETransactionDD.Sort = false;
			this.DataBind();

			this.IssueTransactionDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.IssueTransaction != Guid.Empty)
			{
				this.IssueTransactionDD.SelectedIndex = this.FindSelectedIndex(
					this.issueList, this.mobileDeviceProfile.IssueTransaction);
			}

			this.DefuelTransactionDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.DefuelTransaction != Guid.Empty)
			{
				this.DefuelTransactionDD.SelectedIndex = this.FindSelectedIndex(
					this.defuelList, this.mobileDeviceProfile.DefuelTransaction);
			}

			this.DeIceTransactionDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.DeIceTransaction != Guid.Empty)
			{
				this.DeIceTransactionDD.SelectedIndex = this.FindSelectedIndex(
					this.deIceList, this.mobileDeviceProfile.DeIceTransaction);
			}

			this.GSETransactionDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.GseTransaction != Guid.Empty)
			{
				this.GSETransactionDD.SelectedIndex = this.FindSelectedIndex(this.gseList, this.mobileDeviceProfile.GseTransaction);
			}

			this.RotationTransactionDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.RotationTransaction != Guid.Empty)
			{
				this.RotationTransactionDD.SelectedIndex = this.FindSelectedIndex(
					this.rotationList, this.mobileDeviceProfile.RotationTransaction);
			}

			this.MeterCloseoutDD.SelectedIndex = 0;
			if (this.mobileDeviceProfile.MeterCloseout != Guid.Empty)
			{
				this.MeterCloseoutDD.SelectedIndex = this.FindSelectedIndex(
					this.meterCloseoutList, this.mobileDeviceProfile.MeterCloseout);
			}
		}

		/// <summary>
		///    This method will disable all fields if the user does not have the
		///    "modify mobile device profile" right.
		/// </summary>
		private void DisableFields()
		{
			this.IssueTransactionDD.Enabled = this.HasPermission();
			this.DefuelTransactionDD.Enabled = this.HasPermission();
			this.RotationTransactionDD.Enabled = this.HasPermission();
			this.MeterCloseoutDD.Enabled = this.HasPermission();
			this.DeIceTransactionDD.Enabled = this.HasPermission();
			this.GSETransactionDD.Enabled = this.HasPermission();
			this.ConsumerCloseoutDD.Enabled = this.HasPermission();
			this.OwnerDD.Enabled = this.HasPermission();
			this.VendorCloseoutDD.Enabled = this.HasPermission();
			this.ConsumerManualDD.Enabled = this.HasPermission();
			this.ShipperDD.Enabled = this.HasPermission();
			this.ManagerDD.Enabled = this.HasPermission();
			this.SupplierDD.Enabled = this.HasPermission();
			this.BillToDD.Enabled = this.HasPermission();
			this.ProductDD.Enabled = this.HasPermission();
			this.VendorManualDD.Enabled = this.HasPermission();
			this.InhibitOverridingTemperatureCB.Enabled = this.HasPermission();
			this.TemperatureTB.Enabled = this.HasPermission();
			this.DensityTB.Enabled = this.HasPermission();
		}

		/// <summary>
		///    This method finds the index for a given GUID in the list and returns the index.
		/// </summary>
		/// <param name="listItems">
		///    The list items.
		/// </param>
		/// <param name="selectedGuid">
		///    The selected guid.
		/// </param>
		/// <returns>
		///    The System.Int32.
		/// </returns>
		private int FindSelectedIndex(List<ListItem> listItems, Guid selectedGuid)
		{
			int selectedCount = 0;
			int count = 0;

			foreach (ListItem item in listItems)
			{
				if (item.Value.Equals(selectedGuid.ToString()))
				{
					selectedCount = count;
					break;
				}

				count++;
			}

			return selectedCount;
		}

		/// <summary>
		///    This method retrieves the companies from the database to add to the dropdowns.
		/// </summary>
		private void GetCompanies()
		{
			this.managerList = new List<ListItem>();
			this.ownerList = new List<ListItem>();
			this.billToList = new List<ListItem>();
			this.shipToList = new List<ListItem>();
			this.shipperList = new List<ListItem>();
			this.supplierList = new List<ListItem>();
			this.carrierList = new List<ListItem>();

			CompanyCollectionClass managerCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, false, false)
																);

			CompanyCollectionClass ownerCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.OWNER, false, false)
																);

			CompanyCollectionClass billToCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.CUSTOMER_BILLTO, false, false)
																);

			CompanyCollectionClass shipToCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.CUSTOMER_SHIPTO, false, false)
																);

			CompanyCollectionClass shipperCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.SHIPPER, false, false)
																);

			CompanyCollectionClass supplierCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.SUPPLIER, false, false)
																);

			CompanyCollectionClass carrierCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.CARRIER, false, false)
																);


			this.managerList = this.BuildCompanyList(managerCollection);
			this.ownerList = this.BuildCompanyList(ownerCollection);
			this.billToList = this.BuildCompanyList(billToCollection);
			this.shipToList = this.BuildCompanyList(shipToCollection);
			this.shipperList = this.BuildCompanyList(shipperCollection);
			this.supplierList = this.BuildCompanyList(supplierCollection);
			this.carrierList = this.BuildCompanyList(carrierCollection);
		}

		/// <summary>
		///    This method will retrieve the products from the database and build a list
		///    for the dropdown.
		/// </summary>
		private void GetProducts()
		{
			this.productList = new List<ListItem>();

			ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);


			foreach (ProductClass product in productCollection)
			{
				var item = new ListItem { Text = product.ID, Value = product.IdentityGuid.ToString() };
				this.productList.Add(item);
			}

			this.productList = this.productList.OrderBy(item => item.Text).ToList();

			var noneItem = new ListItem { Text = "None", Value = Guid.Empty.ToString() };
			this.productList.Insert(0, noneItem);
		}

		/// <summary>
		///    This method retrieves the transaction aliases from the database and builds a list
		///    for Issues, Defuels, GSE, Meter Closeout, Rotations, and De-Ice.
		/// </summary>
		private void GetTransactionAliases()
		{
			this.issueList = new List<ListItem>();
			this.deIceList = new List<ListItem>();
			this.defuelList = new List<ListItem>();
			this.gseList = new List<ListItem>();
			this.meterCloseoutList = new List<ListItem>();
			this.rotationList = new List<ListItem>();

			TransactionAliasCollectionClass transCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			foreach (TransactionAliasClass transAlias in transCollection)
			{
				if (transAlias != null)
				{
					ListItem item;

					switch (transAlias.TransTypeID)
					{
						case TransactionTypes.T5_PrimaryDisbursement:
							item = new ListItem { Text = transAlias.ID, Value = transAlias.IdentityGuid.ToString() };
							this.issueList.Add(item);
							this.gseList.Add(item);
							this.deIceList.Add(item);
							break;

						case TransactionTypes.T6_SecondaryDisbursement:
							item = new ListItem { Text = transAlias.ID, Value = transAlias.IdentityGuid.ToString() };
							this.issueList.Add(item);
							break;

						case TransactionTypes.T3_PrimaryDefuel:
						case TransactionTypes.T4_SecondaryDefuel:
							item = new ListItem { Text = transAlias.ID, Value = transAlias.IdentityGuid.ToString() };
							this.defuelList.Add(item);
							break;

						case TransactionTypes.T12_InventoryNotAffected:
							item = new ListItem { Text = transAlias.ID, Value = transAlias.IdentityGuid.ToString() };
							this.meterCloseoutList.Add(item);
							this.rotationList.Add(item);
							this.deIceList.Add(item);
							break;
					}
				}
			}

			// Sort the list in alphabetically order.
			this.issueList = this.issueList.OrderBy(item => item.Text).ToList();
			this.gseList = this.gseList.OrderBy(item => item.Text).ToList();
			this.deIceList = this.deIceList.OrderBy(item => item.Text).ToList();
			this.defuelList = this.defuelList.OrderBy(item => item.Text).ToList();
			this.meterCloseoutList = this.meterCloseoutList.OrderBy(item => item.Text).ToList();
			this.rotationList = this.rotationList.OrderBy(item => item.Text).ToList();

			// Add "None" as the first entry.
			var noneItem = new ListItem { Text = "None", Value = Guid.Empty.ToString() };
			this.issueList.Insert(0, noneItem);
			this.gseList.Insert(0, noneItem);
			this.deIceList.Insert(0, noneItem);
			this.defuelList.Insert(0, noneItem);
			this.meterCloseoutList.Insert(0, noneItem);
			this.rotationList.Insert(0, noneItem);
		}

		/// <summary>
		///    This method returns true if the user has the MODIFY_MOBILE_DEVICE_PROFILES right and the
		///    entity has not been assigned down.
		/// </summary>
		/// <returns>
		///    The System.Boolean.
		/// </returns>
		private bool HasPermission()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return false;
			}

			if (this.mobileDeviceProfile.SiteGuid == Guid.Empty && this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES))
			{
				return true;
			}

			return this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES)
			       && (this.Security.SiteGuid == this.mobileDeviceProfile.SiteGuid);
		}

		/// <summary>
		///    This method will load the profile communication page with the data from the database.
		/// </summary>
		private void UpdateView()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			// Set transaction selections
			var itemList = (List<ListItem>)this.IssueTransactionDD.DataSource;
			this.IssueTransactionDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.IssueTransaction);

			itemList = (List<ListItem>)this.DefuelTransactionDD.DataSource;
			this.DefuelTransactionDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.DefuelTransaction);

			itemList = (List<ListItem>)this.DeIceTransactionDD.DataSource;
			this.DeIceTransactionDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.DeIceTransaction);

			itemList = (List<ListItem>)this.GSETransactionDD.DataSource;
			this.GSETransactionDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.GseTransaction);

			itemList = (List<ListItem>)this.MeterCloseoutDD.DataSource;
			this.MeterCloseoutDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.MeterCloseout);

			itemList = (List<ListItem>)this.RotationTransactionDD.DataSource;
			this.RotationTransactionDD.SelectedIndex = this.FindSelectedIndex(
				itemList, this.mobileDeviceProfile.RotationTransaction);

			// Set Company selections.
			itemList = (List<ListItem>)this.BillToDD.DataSource;
			this.BillToDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.ManualBillTo);

			itemList = (List<ListItem>)this.ConsumerManualDD.DataSource;
			this.ConsumerManualDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.ManualConsumer);

			itemList = (List<ListItem>)this.ManagerDD.DataSource;
			this.ManagerDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.ManualManager);

			itemList = (List<ListItem>)this.ShipperDD.DataSource;
			this.ShipperDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.ManualShipper);

			itemList = (List<ListItem>)this.SupplierDD.DataSource;
			this.SupplierDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.ManualSupplier);

			itemList = (List<ListItem>)this.VendorManualDD.DataSource;
			this.VendorManualDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.ManualVendor);

			itemList = (List<ListItem>)this.ConsumerCloseoutDD.DataSource;
			this.ConsumerCloseoutDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.CloseoutConsumer);

			itemList = (List<ListItem>)this.OwnerDD.DataSource;
			this.OwnerDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.CloseoutOwner);

			itemList = (List<ListItem>)this.VendorCloseoutDD.DataSource;
			this.VendorCloseoutDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.CloseoutVendor);

			itemList = (List<ListItem>)this.ProductDD.DataSource;
			this.ProductDD.SelectedIndex = this.FindSelectedIndex(itemList, this.mobileDeviceProfile.ManualProduct);

			// Set the inhibit, temperature and density.
			this.InhibitOverridingTemperatureCB.Checked = this.mobileDeviceProfile.InhibitOverridingTemperature;
			this.TemperatureTB.Text = string.Empty;
			this.DensityTB.Text = string.Empty;
			this.TemperatureTB.Enabled = false;
			this.TemperatureLB.Enabled = false;
			this.DensityTB.Enabled = false;
			this.DensityLB.Enabled = false;

			if (this.mobileDeviceProfile.InhibitOverridingTemperature)
			{
				this.TemperatureTB.Enabled = true;
				this.TemperatureLB.Enabled = true;
				this.DensityTB.Enabled = true;
				this.DensityLB.Enabled = true;

				if (this.mobileDeviceProfile.ManualTemperature != null)
				{
					this.TemperatureTB.Text = this.mobileDeviceProfile.ManualTemperature.Value.ToString(CultureInfo.InvariantCulture);
				}

				if (this.mobileDeviceProfile.ManualDensity != null)
				{
					this.DensityTB.Text = this.mobileDeviceProfile.ManualDensity.Value.ToString(CultureInfo.InvariantCulture);
				}
			}
		}

		#endregion
	}
}
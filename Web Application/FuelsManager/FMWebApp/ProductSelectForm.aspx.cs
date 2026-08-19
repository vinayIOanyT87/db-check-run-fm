// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Linq;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

    using FMCore;
	using global::FMWebApp;

	public enum IDLINK_TYPE
	{
		MANAGER,
		SHIPTO,
		SUPPLIER,
		NONE
	};

	/// <summary>
	/// Allows a user to choose a product
	/// </summary>
	public partial class ProductSelectForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields
		protected ProductSelectContextClass ProductSelectContext = null;
		protected string SelectThisItemText = null;
		#endregion

		#region Methods
		protected void FindAllBtn_OnClick(object sender, EventArgs e)
		{
			this.ProductSelectContext.SearchString = null;
			this.FindTextBox.Text = "";
			this.UpdateView();
		}

		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			if (this.FindTextBox.Text.Length < 1)
			{
				this.ProductSelectContext.SearchString = null;
			}
			else
			{
				this.ProductSelectContext.SearchString = this.FindTextBox.Text.ToUpper();
			}

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

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.SelectThisItemText = this.GetTranslatedText("Select this item");

				if (this.Page.IsPostBack == false)
				{
					this.ProductSelectContext = new ProductSelectContextClass();

					if (this.Request.GetQueryOrFormValue("Type") != null)
					{
						this.ProductSelectContext.Type = (ProductType)Enum.Parse(typeof(ProductType), this.Request.GetQueryOrFormValue("Type"));
					}

					if (this.Request.GetQueryOrFormValue("All") != null)
					{
						this.ProductSelectContext.All = Convert.ToBoolean(this.Request.GetQueryOrFormValue("All"));
					}

                    if (this.Request.GetQueryOrFormValue("None") != null)
                    {
                        this.ProductSelectContext.None = Convert.ToBoolean(this.Request.GetQueryOrFormValue("None"));
                    }

                    if (this.Request.GetQueryOrFormValue("Null") != null)
					{
						this.ProductSelectContext.Null = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Null"));
					}

					if (this.Request.GetQueryOrFormValue("Unassigned") != null)
					{
						this.ProductSelectContext.Unassigned = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Unassigned"));
					}

					if (this.Request.GetQueryOrFormValue("Map") != null)
					{
						try
						{
							this.ProductSelectContext.Map = (int)Enum.Parse(typeof(PRODUCT_MAP_TYPE), this.Request.GetQueryOrFormValue("Map"));
							this.ProductSelectContext.MapType = typeof(PRODUCT_MAP_TYPE);
						}
						catch
						{
							try
							{
								this.ProductSelectContext.Map = (int)Enum.Parse(typeof(STRING_MAP_TYPE), this.Request.GetQueryOrFormValue("Map"));
								this.ProductSelectContext.MapType = typeof(STRING_MAP_TYPE);
							}
							catch
							{
								throw new Exception("Unknown Map Type");
							}
						}
					}

					if (this.Request.GetQueryOrFormValue("Mode") != null)
					{
						this.ProductSelectContext.Mode = this.Request.GetQueryOrFormValue("Mode");
					}

					if (!this.Security.HasRight(RIGHT.MODIFY_PRODUCTS) || this.ProductSelectContext.Mode == "Unassign")
					{
						this.AddButton1.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					// Used to link the ShipTo to the product. Contains the ShipTo ID.
					if (this.Request.GetQueryOrFormValue("IDLink") != null)
					{
						this.ProductSelectContext.IDLink = this.ParseIDLink(this.Request.GetQueryOrFormValue("IDLink"));
					}

					if (this.Request.GetQueryOrFormValue("SearchString") != null)
					{
						this.ProductSelectContext.SearchString = this.Request.GetQueryOrFormValue("SearchString");
						this.FindTextBox.Text = this.ProductSelectContext.SearchString;
					}

                    if (this.Request.GetQueryOrFormValue("HideHidden") != null)
                    {
                        this.ProductSelectContext.HideHidden = Convert.ToBoolean(this.Request.GetQueryOrFormValue("HideHidden"));
                    }

					this.Session["ProductSelectContext"] = this.ProductSelectContext;

					this.UpdateView();
				}
				else
				{
					this.ProductSelectContext = this.Session["ProductSelectContext"] as ProductSelectContextClass;

					// Determine action
					bool btnPressed = ( string.IsNullOrEmpty( this.Request.GetQueryOrFormValue( "FindBtn" ) ) == false
									   || string.IsNullOrEmpty( this.Request.GetQueryOrFormValue( "ShowAllBtn" ) ) == false
									   || string.IsNullOrEmpty( this.Request.GetQueryOrFormValue( "AddButton1" ) ) == false
									   || string.IsNullOrEmpty( this.Request.GetQueryOrFormValue( "AddButton2" ) ) == false );

					if (!btnPressed)
					{
						// default action is find
						this.FindBtn_OnClick(sender, e);
					}
				}

				var productSelectContextClass = this.ProductSelectContext;

				if (productSelectContextClass != null && productSelectContextClass.Mode != null)
				{
					var form1 = (HtmlForm)this.FindControl("Form1");
					var okButton = new HtmlInputButton();

					okButton.Attributes.Add("value", this.GetTranslatedText("OK"));
					okButton.Attributes.Add("id", "OkButton");
					okButton.Attributes.Add("class", "formfieldtitle");
					okButton.Attributes.Add("onclick", "MultipleSelect()");
					okButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 662px; POSITION: absolute; TOP: 8px");
					form1.Controls.Add(okButton);

					var cancelButton = new HtmlInputButton();
					cancelButton.Attributes.Add("value", this.GetTranslatedText("Cancel"));
					cancelButton.Attributes.Add("id", "CancelButton");
					cancelButton.Attributes.Add("class", "formfieldtitle");
					cancelButton.Attributes.Add("onclick", "NoSelect()");
					cancelButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 758px; POSITION: absolute; TOP: 8px");
					form1.Controls.Add(cancelButton);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			var productArrayList = this.Session["ProductArrayList"] as ArrayList;
			
			if (productArrayList == null)
			{
				productArrayList = new ArrayList();
				this.Session["ProductArrayList"] = productArrayList;
			}

			var siteClass = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(	this.Security,
																							this.Security.SiteGuid,
																							getMemberSites: false,
																							getSchedulesAndProcessVariables: false,
																							bGetAssociatedAliases: false));

			var product = new ProductClass(siteClass);
			productArrayList.Add(product);
			var productSelectContextArrayList = this.Session["ProductSelectContextArrayList"] as ArrayList;

			if (productSelectContextArrayList == null)
			{
				productSelectContextArrayList = new ArrayList { this.Session["ProductSelectContext"] };
				this.Session["ProductSelectContextArrayList"] = productSelectContextArrayList;
			}
			else
			{
				productSelectContextArrayList.Add(this.Session["ProductSelectContext"]);
			}

			this.Redirect("ProductForm.aspx?Modal=true");
		}

		/// <summary>
		///    This method will return true if the transaction alias has the Storage Location (tanks) field
		///    configured.  Otherwise, it will return false.  Note: the transaction alias type must be a
		///    type 14 (physical inventory).
		/// </summary>
		/// <returns></returns>
		private bool AliasHasTanksConfigured()
		{
			const string TankDbName = "STORAGELOCATIONID";
			bool hasTanksConfigured = false;

			if (this.Session["TransactionDetailTransaction"] != null)
			{
				var transDO = this.Session["TransactionDetailTransaction"] as TransactionDO;

				if (transDO != null && transDO.TransTypeID == TransactionTypes.T14_PhysicalInventory)
				{
					// Get the associated transaction alias.
					TransactionAliasClass alias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, transDO.TransactionAliasGuid, false)
																);

					// Check to see if the transaction alias has the StorageLocation configured
					if (alias != null)
					{
						foreach (var fieldClass in alias.LineItemFieldCollection)
						{
							var field = (TransactionAliasFieldClass)fieldClass;

							// If so, then set the flag to true.
							if (field.DbName.ToUpper().Equals(TankDbName))
							{
								hasTanksConfigured = true;
								break;
							}
						}
					}
				}
			}

			return hasTanksConfigured;
		}

		/// <summary>
		///    This method will determine if there is a linked manager ID and filter the
		///    product list to the associated products of the manager and associated tanks.
		///    If manager ID does not exist, then enumerate all products.
		/// </summary>
		private ProductCollectionClass FilterByManagerID()
		{
			var productCollection = new ProductCollectionClass();

			if (this.ProductSelectContext.IDLink != null)
			{
				string managerID = this.ProductSelectContext.IDLink;

				if (managerID.Length > 0)
				{
					var transaction = this.Session["TransactionDetailTransaction"] as TransactionDO;

					productCollection.Clear();
					ProductCollectionClass managerProducts;

					// When tranaction is null the caller is the InventoryReconciliation form.
					if (transaction == null)
					{
						SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.Security,
																			this.Security.SiteGuid,
																			getMemberSites: true,
																			getSchedulesAndProcessVariables: true,
																			bGetAssociatedAliases: true)
																	);
						managerProducts = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);


						foreach (ProductClass product in managerProducts)
						{
							if (product.InhibitAccounting)
							{
								continue;
							}

							if (!site.EnableAdditiveAccounting && product.ProductType == ProductType.AdditiveProduct)
							{
								continue;
							}

							if (product.ProductType == ProductType.AdditizedProduct || product.ProductType == ProductType.BlendProduct)
							{
								continue;
							}

							productCollection.Add(product);
						}
					}
					else
					{
						if (this.AliasHasTanksConfigured())
						{
							managerProducts = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByManagerAndTanks(this.Security, managerID)
																);
						}

						else
						{
							managerProducts = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByManagerAndTanks(this.Security, string.Empty)
																);
						}

						foreach (ProductClass product in managerProducts)
						{
							productCollection.Add(product);
						}
					}
				}
			}

			// Filter collection on excluded products
			productCollection = this.FilterOnAlias(productCollection);

			// Filter collection on the search string.
			productCollection = this.FilterOnFind(productCollection);

			productCollection = this.FilterOnSite(productCollection);

			return productCollection;
		}

		/// <summary>
		///    This method will determine if there is a linked ShipTo ID and filter the
		///    product list to the associated products of the ShipTo.
		/// </summary>
		private ProductCollectionClass FilterByShipToID()
		{
			var productCollection = new ProductCollectionClass();

			if (this.ProductSelectContext.IDLink != null)
			{
				string shipToID = this.ProductSelectContext.IDLink;

				if (shipToID.Length > 0)
				{
					Guid shipToGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, shipToID)
																);
					
					CompanyClass shipToCompany = null;

					if (shipToGuid != Guid.Empty)
					{
						shipToCompany = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, shipToGuid)
																);
					}

					if (shipToCompany != null)
					{
						var transaction = this.Session["TransactionDetailTransaction"] as TransactionDO;
						var currentLineItem = this.Session["TransactionDetailLineItem"] as LineItemDO;
						var currentSubLineItem = this.Session["TransactionDetailSubLineItem"] as SubLineItemDO;

						if (currentSubLineItem == null)
						{
							ProductMapCollectionClass authorizedProducts = shipToCompany.AuthorizedProductCollection;

							foreach (ProductMapClass productMap in authorizedProducts)
							{
								var product = new ProductClass
								              {
									              IdentityGuid = productMap.AssignedGuid,
									              MasterRecordGuid = productMap.ProductMasterRecordGuid,
									              ID = productMap.AssignedID,
									              Code = productMap.AssignedCode,
									              Description = productMap.AssignedDescription,
									              ProductType = productMap.AssignedProductType,
									              SiteGuid = this.Security.SiteGuid
								              };

								// For orders preclude duplicate product line items
								if ((transaction != null)
								    && ((transaction.TransTypeID == TransactionTypes.T17_Order)
								        || (transaction.TransTypeID == TransactionTypes.T18_SupplyOrder)) && (currentLineItem != null))
								{
									foreach (LineItemDO lineItem in transaction.LineItems)
									{
										if (lineItem == currentLineItem || lineItem.ProductGuid == Guid.Empty)
										{
											continue;
										}

										if (lineItem.ProductGuid == productMap.AssignedGuid)
										{
											product = null;
											break;
										}
									}
								}

								if (product != null)
								{
									productCollection.Add(product);
								}
							}
						}

						else if (currentLineItem != null)
						{
							if (currentLineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct))
							{
								ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.Get(this.Security, currentLineItem.ProductGuid)
																);
								
								foreach (ProductMapClass componentMap in product.ComponentCollection)
								{
									var component = new ProductClass
									                {
										                IdentityGuid = componentMap.AssignedGuid,
										                ID = componentMap.AssignedID,
										                Code = componentMap.AssignedCode,
										                Description = componentMap.AssignedDescription,
										                SiteGuid = this.Security.SiteGuid
									                };

									// Preclude duplicates
									foreach (SubLineItemDO subLineItem in currentLineItem.SubLineItems)
									{
										if (subLineItem == currentSubLineItem || subLineItem.ProductGuid == Guid.Empty)
										{
											continue;
										}

										if (subLineItem.ProductGuid == componentMap.AssignedGuid)
										{
											component = null;
											break;
										}
									}

									if (component != null)
									{
										productCollection.Add(component);
									}
								}
							}

							if (currentLineItem.AdditiveProfileGuid != Guid.Empty)
							{
								AdditiveProfileClass additiveProfile = FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
																	 x =>
																	 x.Get(this.Security, currentLineItem.AdditiveProfileGuid)
																);

								foreach (ProductMapClass additiveMap in additiveProfile.AdditiveCollection)
								{
									var additive = new ProductClass
									               {
										               IdentityGuid = additiveMap.AssignedGuid,
										               ID = additiveMap.AssignedID,
										               Code = additiveMap.AssignedCode,
										               Description = additiveMap.AssignedDescription,
										               SiteGuid = this.Security.SiteGuid
									               };

									// Preclude duplicates
									foreach (SubLineItemDO subLineItem in currentLineItem.SubLineItems)
									{
										if (subLineItem == currentSubLineItem || subLineItem.ProductGuid == Guid.Empty)
										{
											continue;
										}

										if (subLineItem.ProductGuid == additiveMap.AssignedGuid)
										{
											additive = null;
											break;
										}
									}

									if (additive != null)
									{
										productCollection.Add(additive);
									}
								}
							}
						}
					}
				}
			}

			// Filter collection on excluded products
			productCollection = this.FilterOnAlias(productCollection);

			// Filter collection on the search string.
			productCollection = this.FilterOnFind(productCollection);

			// Filter collection on the site.
			productCollection = this.FilterOnSite(productCollection);

			return productCollection;
		}

		private ProductCollectionClass FilterBySupplierID()
		{
			var productCollection = new ProductCollectionClass();

			if (this.ProductSelectContext.IDLink != null)
			{
				string supplierID = this.ProductSelectContext.IDLink;

				if (supplierID.Length > 0)
				{
					Guid supplierGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, supplierID)
																);

					CompanyClass supplierCompany = null;

					if (supplierGuid != Guid.Empty)
					{
						supplierCompany = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, supplierGuid)
																);
					}

					if (supplierCompany != null)
					{
						var transaction = this.Session["TransactionDetailTransaction"] as TransactionDO;
						var currentLineItem = this.Session["TransactionDetailLineItem"] as LineItemDO;
						var currentSubLineItem = this.Session["TransactionDetailSubLineItem"] as SubLineItemDO;

						if (currentSubLineItem == null)
						{
							ProductMapCollectionClass authorizedProducts = supplierCompany.SupplierAuthorizedProductCollection;

							foreach (ProductMapClass productMap in authorizedProducts)
							{
								var product = new ProductClass
								              {
									              IdentityGuid = productMap.AssignedGuid,
									              ID = productMap.AssignedID,
									              Code = productMap.AssignedCode,
									              Description = productMap.AssignedDescription,
									              ProductType = productMap.AssignedProductType,
									              SiteGuid = this.Security.SiteGuid
								              };

								// For orders preclude duplicate product line items
								if ((transaction != null)
								    && ((transaction.TransTypeID == TransactionTypes.T17_Order)
								        || (transaction.TransTypeID == TransactionTypes.T18_SupplyOrder)) && (currentLineItem != null))
								{
									foreach (LineItemDO lineItem in transaction.LineItems)
									{
										if (lineItem == currentLineItem || lineItem.ProductGuid == Guid.Empty)
										{
											continue;
										}

										if (lineItem.ProductGuid == productMap.AssignedGuid)
										{
											product = null;
											break;
										}
									}
								}

								if (product != null)
								{
									productCollection.Add(product);
								}
							}
						}

						else if (currentLineItem != null)
						{
							if (currentLineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct))
							{
								ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.Get(this.Security, currentLineItem.ProductGuid)
																);

								foreach (ProductMapClass componentMap in product.ComponentCollection)
								{
									var component = new ProductClass
									                {
										                IdentityGuid = componentMap.AssignedGuid,
										                ID = componentMap.AssignedID,
										                Code = componentMap.AssignedCode,
										                Description = componentMap.AssignedDescription,
										                SiteGuid = this.Security.SiteGuid
									                };

									// Preclude duplicates
									foreach (SubLineItemDO subLineItem in currentLineItem.SubLineItems)
									{
										if (subLineItem == currentSubLineItem || subLineItem.ProductGuid == Guid.Empty)
										{
											continue;
										}

										if (subLineItem.ProductGuid == componentMap.AssignedGuid)
										{
											component = null;
											break;
										}
									}

									if (component != null)
									{
										productCollection.Add(component);
									}
								}
							}

							if (currentLineItem.AdditiveProfileGuid != Guid.Empty)
							{
								AdditiveProfileClass additiveProfile = FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
																	 x =>
																	 x.Get(this.Security, currentLineItem.AdditiveProfileGuid)
																);

								foreach (ProductMapClass additiveMap in additiveProfile.AdditiveCollection)
								{
									var additive = new ProductClass
									               {
										               IdentityGuid = additiveMap.AssignedGuid,
										               ID = additiveMap.AssignedID,
										               Code = additiveMap.AssignedCode,
										               Description = additiveMap.AssignedDescription,
										               SiteGuid = this.Security.SiteGuid
									               };

									// Preclude duplicates
									foreach (SubLineItemDO subLineItem in currentLineItem.SubLineItems)
									{
										if (subLineItem == currentSubLineItem || subLineItem.ProductGuid == Guid.Empty)
										{
											continue;
										}

										if (subLineItem.ProductGuid == additiveMap.AssignedGuid)
										{
											additive = null;
											break;
										}
									}

									if (additive != null)
									{
										productCollection.Add(additive);
									}
								}
							}
						}
					}
				}
			}

			// Filter collection on excluded products
			productCollection = this.FilterOnAlias(productCollection);

			// Filter collection on the search string.
			productCollection = this.FilterOnFind(productCollection);

			// Filter collection on site
			productCollection = this.FilterOnSite(productCollection);

			return productCollection;
		}

		private ProductCollectionClass FilterOnAlias(ProductCollectionClass productCollection)
		{
			var transaction = this.Session["TransactionDetailTransaction"] as TransactionDO;
			ProductMapCollectionClass excludedProducts = null;

			if ((transaction != null) && (transaction.TransactionAliasGuid != Guid.Empty))
			{
				excludedProducts = 
					FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
							x =>
							x.EnumerateByAssignedToGuidAndType(this.Security, transaction.TransactionAliasGuid, 
														PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP)
					);

			}

			var newProductCollection = new ProductCollectionClass();

			foreach (ProductClass product in productCollection)
			{
				if ((excludedProducts != null) && (excludedProducts.Find(x => x.AssignedGuid == product.IdentityGuid) != null))
				{
					continue;
				}

				newProductCollection.Add(product);
			}

			return newProductCollection;
		}

		private void FilterOnAssociationToFootNote(ref ProductCollectionClass productCollection)
		{
			var footNote = this.Session["FootNote"] as FootNoteClass;

			if (footNote == null)
			{
				return;
			}

			if (this.ProductSelectContext.Mode == "Assign")
			{
				// Test for Assignment of {All}
				if (footNote.FootNoteProductMapCollection.Count == 1
				    && footNote.FootNoteProductMapCollection[0].AssignedToGuid == Guid.Empty)
				{
					productCollection.Clear();
				}
				else
				{
					var unassignedProductCollection = new ProductCollectionClass();
					{
						var product = new ProductClass { ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}")) };
						unassignedProductCollection.Insert(0, product);
					}

					foreach (ProductClass product in productCollection)
					{
						bool assigned = false;
						foreach (ApplicationStringMapClass assignedApplicationStringMap in footNote.FootNoteProductMapCollection)
						{
							if (product.IdentityGuid == assignedApplicationStringMap.AssignedToGuid)
							{
								assigned = true;
								break;
							}
						}

						if (!assigned)
						{
							unassignedProductCollection.Add(product);
						}
					}

					productCollection = unassignedProductCollection;
				}
			}
			else if (this.ProductSelectContext.Mode == "Unassign")
			{
				// Test for Assignment of {All}
				if (footNote.FootNoteProductMapCollection.Count == 1
				    && footNote.FootNoteProductMapCollection[0].AssignedToGuid == Guid.Empty)
				{
					productCollection.Clear();
					var product = new ProductClass { ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}")) };
					productCollection.Insert(0, product);
				}
				else
				{
					var assignedProductCollection = new ProductCollectionClass();

					foreach (ProductClass product in productCollection)
					{
						bool assigned = footNote.FootNoteProductMapCollection.Any(
											assignedApplicationStringMap => product.IdentityGuid == assignedApplicationStringMap.AssignedToGuid);

						if (assigned)
						{
							assignedProductCollection.Add(product);
						}
					}

					productCollection = assignedProductCollection;
				}
			}
		}

		/// <summary>
		///    This method will remove all products that do not match the find string.
		/// </summary>
		/// <param name="productCollection"></param>
		private ProductCollectionClass FilterOnFind(ProductCollectionClass productCollection)
		{
			if (!string.IsNullOrEmpty(this.FindTextBox.Text))
			{
				var newProductCollection = new ProductCollectionClass();
				string searchStr = this.FindTextBox.Text;

				foreach (ProductClass product in productCollection)
				{
					string productID = product.ID;
					int found = productID.ToUpper().IndexOf(searchStr.ToUpper(), StringComparison.Ordinal);

					if (found != -1)
					{
						newProductCollection.Add(product);
					}
				}

				return newProductCollection;
			}

			return productCollection;
		}

		/// <summary>
		///    This method will remove all products that do not match the find string.
		/// </summary>
		/// <param name="productCollection"></param>
		private ProductCollectionClass FilterOnSite(ProductCollectionClass productCollection)
		{
			ProductCollectionClass siteProductCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);


			var newProductCollection = new ProductCollectionClass();

			foreach (ProductClass product in productCollection)
			{
                if ((siteProductCollection.Find(x => x.IdentityGuid == product.IdentityGuid) != null) 
					|| (siteProductCollection.Find(x => x.MasterRecordGuid == product.MasterRecordGuid) != null))
				{
					newProductCollection.Add(product);
				}
			}

			return newProductCollection;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command				+= this.AddButtonCommand;
			this.ProductDataGrid.EditCommand	+= this.ProductDataGridEditCommand;
			this.ProductDataGrid.DeleteCommand	+= this.ProductDataGridDeleteCommand;
			this.ProductDataGrid.ItemDataBound	+= this.ProductDataGridItemDataBound;
			this.AddButton1.Command				+= this.AddButtonCommand;
		}

		/// <summary>
		///    This method will return true if the transaction is a type 14 (physical inventory).
		///    Otherwise, it will return false.
		/// </summary>
		/// <returns></returns>
		private bool IsTransTypePhysicalInventory()
		{
			bool physicalInventoryType = false;

			if (this.Session["TransactionDetailTransaction"] != null)
			{
				var transDO = this.Session["TransactionDetailTransaction"] as TransactionDO;

				if (transDO.TransTypeID == TransactionTypes.T14_PhysicalInventory)
				{
					physicalInventoryType = true;
				}
			}

			return physicalInventoryType;
		}

		/// <summary>
		///    This method will parse out the IDLink request parameter into the ID and
		///    the type.  The type may be Manager or ShipTo.  It returns the ID and sets
		///    the type.
		/// </summary>
		/// <param name="inRequest"></param>
		/// <returns></returns>
		private string ParseIDLink(string inRequest)
		{
			string idLink = "";
			this.ProductSelectContext.IDLinkType = IDLINK_TYPE.NONE;

			if (!string.IsNullOrEmpty(inRequest))
			{
				int indexFound = inRequest.IndexOf("|", StringComparison.Ordinal);

				if (indexFound != -1)
				{
					idLink = inRequest.Substring(0, indexFound);
					string type = (inRequest.Substring(indexFound + 1)).ToUpper();

					if (type.Equals("MANAGER"))
					{
						this.ProductSelectContext.IDLinkType = IDLINK_TYPE.MANAGER;
					}

					if (type.Equals("SHIPTO"))
					{
						this.ProductSelectContext.IDLinkType = IDLINK_TYPE.SHIPTO;
					}

					if (type.Equals("SUPPLIER"))
					{
						this.ProductSelectContext.IDLinkType = IDLINK_TYPE.SUPPLIER;
					}
				}
			}

			return idLink;
		}

		private void ProductDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get identity guid
				TableCell identityGuidCell = e.Item.Cells[3];//bds
				FMChannelHelper.MakeCall<IProducts>(x => x.Purge(this.Security, Guid.Parse(identityGuidCell.Text)));
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ProductDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				TableCell identityGuidCell = e.Item.Cells[3];//bds
				var productArrayList = this.Session["ProductArrayList"] as ArrayList;

				if (productArrayList == null)
				{
					productArrayList = new ArrayList();
					this.Session["ProductArrayList"] = productArrayList;
				}

				// Get Product
				ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.Get(this.Security, Guid.Parse(identityGuidCell.Text))
																);

				productArrayList.Add(product);
				var productSelectContextArrayList = this.Session["ProductSelectContextArrayList"] as ArrayList;

				if (productSelectContextArrayList == null)
				{
					productSelectContextArrayList = new ArrayList { this.Session["ProductSelectContext"] };
					this.Session["ProductSelectContextArrayList"] = productSelectContextArrayList;
				}
				else
				{
					productSelectContextArrayList.Add(this.Session["ProductSelectContext"]);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("ProductForm.aspx?Modal=true");
		}

		/// <summary>
		///    This method create all the links for the product list and places them
		///    on the client side.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ProductDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				if (e.Item.ItemType == ListItemType.Header)
				{
					e.Item.Cells[0].Text = this.GetTranslatedText(this.ProductSelectContext.Mode ?? "Select");
					if (this.ProductDataGrid.Columns.Count > 0)
						this.ProductDataGrid.Columns[0].HeaderText = e.Item.Cells[0].Text;
				}
			}

			else
			{
				string id = string.Empty;

				// Leave hard space zero length string
				if (e.Item.Cells[4].Text != "&nbsp;")//bds
				{
					id = HttpUtility.HtmlDecode(e.Item.Cells[4].Text);//bds
				}

				if (this.ProductSelectContext.Mode != null)
				{
					var select = new HtmlInputCheckBox();
					select.ID = "Select";
					e.Item.Cells[0].Controls.Add(select);
					select.Attributes.Add("Title", HttpUtility.JavaScriptStringEncode(this.ProductDataGrid.Columns[0].HeaderText + " " + id));
					e.Item.Cells[4].Text = e.Item.Cells[4].Text.Replace(" ", "&nbsp;");//bds
				}
				else
				{
					string toolTip = ((e.Item.Cells[5].Text != "&nbsp;") ? e.Item.Cells[5].Text + ", " : "")//bds
					                 + ((e.Item.Cells[6].Text != "&nbsp;") ? e.Item.Cells[6].Text + ", " : "");//bds

					var select = new HtmlAnchor();
					select.ID = "Select";
					select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(id) + "','" 
																			+ HttpUtility.JavaScriptStringEncode(toolTip) + "')");
					Image im = new Image();
					im.ImageUrl = "../FMWebApp/Images/Select.gif";
					im.BorderWidth = 0;
					im.Style.Add("align", "absmiddle");
					select.Controls.Add(im);

					e.Item.Cells[0].Controls.Add(select);
				}

				Guid siteGuid = Guid.Parse(e.Item.Cells[2].Text);//bds
				Guid productGuid = Guid.Parse(e.Item.Cells[3].Text);//bds

				var deleteButton = (LinkButton)e.Item.FindControl("Fmdeletelinkbutton1");
				
				if (deleteButton != null)
				{
					deleteButton.Enabled = (this.Security.HasRight(RIGHT.MODIFY_PRODUCTS) && this.Security.SiteGuid == siteGuid
					                        && productGuid != Guid.Empty && this.ProductSelectContext.Mode != "Unassign");

					//Child record versions cannot be created or deleted directly. Their lifetime is controlled by the Entity-To-Site assignment only.
					if (deleteButton.Enabled)
					{
						int indx = (this.ProductDataGrid.CurrentPageIndex * this.ProductDataGrid.PageSize) + e.Item.ItemIndex;
						var dv = (DataView)this.ProductDataGrid.DataSource;
						
						if (!dv.Table.Rows[indx]["IdentityGuid"].Equals(dv.Table.Rows[indx]["MasterRecordGuid"]))
						{
							deleteButton.Enabled = false;
						}
					}
				}

				var editButton = (LinkButton)e.Item.FindControl("Fmeditlinkbutton1");
				
				if (editButton != null)
				{
					editButton.Enabled = ((this.ProductSelectContext.Mode != "Unassign") && (productGuid != Guid.Empty)
					                      && (this.Security.HasRight(RIGHT.MODIFY_PRODUCTS)
					                          || this.Security.HasRight(RIGHT.VIEW_PRODUCTS)));
				}
			}
		}

		private void UpdateView()
		{
			bool getProductsWithoutIDLink = true;

			this.FindTextBox.Text = this.ProductSelectContext.SearchString;

			var productCollection = new ProductCollectionClass();
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
			// Product Select for ShipTo or Manager
			if (this.ProductSelectContext.IDLink != null)
			{
				if (this.ProductSelectContext.IDLinkType == IDLINK_TYPE.SHIPTO)
				{
					productCollection = this.FilterByShipToID();
					getProductsWithoutIDLink = false;
				}

				if (this.ProductSelectContext.IDLinkType == IDLINK_TYPE.MANAGER)
				{
					productCollection = this.FilterByManagerID();
					getProductsWithoutIDLink = false;
				}

				if (this.ProductSelectContext.IDLinkType == IDLINK_TYPE.SUPPLIER && 
					! FMChannelHelper.MakeCall<IHardwareKey, bool>(x =>x.IsADFKey() ))
				{
					productCollection = this.FilterBySupplierID();
					getProductsWithoutIDLink = false;
				}
			}

			if (getProductsWithoutIDLink)
			{
				if (this.ProductSelectContext.Type == ProductType.MaxProduct)
				{
					if (!string.IsNullOrEmpty(this.FindTextBox.Text))
					{
						productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByFilter(this.Security, this.FindTextBox.Text)
																);
					}
					else
					{
						productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
					}

					productCollection = this.FilterOnAlias(productCollection);
				}
				else
				{
					if (this.FindTextBox.Text != string.Empty)
					{
						productCollection = 
							FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
									x =>
									x.EnumerateByTypeAndFilter(this.Security, this.ProductSelectContext.Type, this.FindTextBox.Text)
							);

					}
					else
					{
						productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, this.ProductSelectContext.Type)
																);
					}
				}

				if (this.ProductSelectContext.MapType == typeof(PRODUCT_MAP_TYPE)
				    && (PRODUCT_MAP_TYPE)this.ProductSelectContext.Map != PRODUCT_MAP_TYPE.MAX_MAP)
				{
					if ((PRODUCT_MAP_TYPE)this.ProductSelectContext.Map == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP)
					{
						var companyGroup = (CompanyGroupClass)this.Session["CompanyGroup"];

						if ("Assign" == this.ProductSelectContext.Mode)
						{
							foreach (ProductMapClass productMap in companyGroup.AuthorizedProductCollection)
							{
								var product = new ProductClass { IdentityGuid = productMap.AssignedGuid };
								productCollection.RemoveAll(x => x.IdentityGuid == product.IdentityGuid);
							}
						}
						else
						{
							productCollection.Clear();

							foreach (ProductMapClass productMap in companyGroup.AuthorizedProductCollection)
							{
								var product = new ProductClass
								              {
									              IdentityGuid = productMap.AssignedGuid,
									              ID = productMap.AssignedID,
									              Code = productMap.AssignedCode,
									              Description = productMap.AssignedDescription,
									              SiteGuid = this.Security.SiteGuid
								              };
								productCollection.Add(product);
							}
						}
					}
				}
				else if (this.ProductSelectContext.MapType == typeof(STRING_MAP_TYPE))
				{
					if (this.ProductSelectContext.Type == ProductType.MaxProduct)
					{
						productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

						productCollection = this.FilterOnFind(productCollection);
					}
					else
					{
						if (this.FindTextBox.Text != string.Empty)
						{
							productCollection = 
								FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
									x =>
									x.EnumerateByTypeAndFilter(this.Security, this.ProductSelectContext.Type, this.FindTextBox.Text)
								);

						}
						else
						{
							productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, this.ProductSelectContext.Type)
																);

						}
					}

					this.FilterOnAssociationToFootNote(ref productCollection);
				}

					// Simple Product Select.  This is Ledger, Closeout, & Inventory Rec
				else
				{
					var newProductCollection = new ProductCollectionClass();

					foreach (ProductClass product in productCollection)
					{
						if (product.InhibitAccounting)
						{
							continue;
						}

						newProductCollection.Add(product);
					}

					productCollection = newProductCollection;

					if (this.ProductSelectContext.Null)
					{
						var product = new ProductClass { ID = string.Empty };
						productCollection.Insert(0, product);
					}

                    if (this.ProductSelectContext.None)
                    {
                        var product = new ProductClass { ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{None}")) };
                        productCollection.Insert(0, product);
                    }

                    if (this.ProductSelectContext.All)
					{
						var product = new ProductClass { ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{All}")) };
						productCollection.Insert(0, product);
					}

					if (this.ProductSelectContext.Unassigned)
					{
						var product = new ProductClass { ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{Unassigned}")) };
						productCollection.Insert(0, product);
					}
				}
			}

			var productDataTable = new DataTable();

			productDataTable.Columns.Add("SiteGuid", typeof(Guid));
			productDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			productDataTable.Columns.Add("ID", typeof(string));
			productDataTable.Columns.Add("Code", typeof(string));
			productDataTable.Columns.Add("Description", typeof(string));
			productDataTable.Columns.Add("Type", typeof(string));
			productDataTable.Columns.Add("VaporRecovery", typeof(bool));
			productDataTable.Columns.Add("MasterRecordGuid", typeof(Guid));

			foreach (ProductClass product in productCollection)
			{
				// Exclude all additive type products if the additive accounting is inhibited.
				if ((site.EnableAdditiveAccounting == false) && (product.ProductType == ProductType.AdditiveProduct))
				{
					continue;
				}

				DataRow productDataRow = productDataTable.NewRow();

				productDataRow["SiteGuid"] = product.SiteGuid;
				productDataRow["IdentityGuid"] = product.IdentityGuid;
				productDataRow["ID"] = product.ID;
				productDataRow["Code"] = product.Code;
				productDataRow["Description"] = product.Description;
				productDataRow["Type"] = this.GetTranslatedText(ProductClass.ProductTypeID(product.ProductType));
				productDataRow["VaporRecovery"] = product.VaporRecovery;
				productDataRow["MasterRecordGuid"] = product.MasterRecordGuid;

				productDataTable.Rows.Add(productDataRow);
			}

			var productDataView = new DataView(productDataTable);

			this.ProductDataGrid.DataSource = productDataView;
			this.ProductDataGrid.DataBind();
			/*
			if (ProductCollection.Count >= limit && limit > 0)
			{
				 lblWarning.Text = "Results limited to first " + limit + " records.  Use filters to narrow search.";
				 lblWarning.Visible = true;
			}
			else
			{
				 lblWarning.Visible = false;
			}*/
		}
		#endregion
	}

	[Serializable]
	public class ProductSelectContextClass
	{
		#region Constants and Fields
		public bool All = false;
        public bool Null = false;
        public string IDLink = null;
		public IDLINK_TYPE IDLinkType = IDLINK_TYPE.NONE;
		public int Map;
		public Type MapType = null;
		public string Mode = null;
		public bool None = false;
		public string SearchString = null;
		public ProductType Type = ProductType.MaxProduct;
		public bool Unassigned = false;

        /// <summary>
        /// Should hidden products be displayed on the form?
        /// </summary>
        public bool HideHidden = false;
		#endregion
	}
}
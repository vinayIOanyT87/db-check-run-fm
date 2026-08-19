namespace TransactionFields
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using AjaxControlToolkit;
	using FMControls;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	abstract public class ProductTextButtonGenerator : FieldGenerator
	{
		#region Public Attributes
		public const short FieldLength = 30;
		#endregion

		#region Protected Attributes
		/// <summary>
		/// True indicates to auto post back.
		/// </summary>
		protected bool AutoPostBack;
		#endregion

		#region Abstract Properties
		/// <summary>
		/// This property is an abstract property that forces implementation of
		/// returning the maximum column size.
		/// </summary>
		abstract protected short MaxColumns
		{
			get;
		}
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public ProductTextButtonGenerator()
		{
			this.AutoPostBack = true;
		}
		#endregion

		#region Override methods
		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMCompanyTextBox control is being generated.
		/// </summary>
		/// <param name="editable"></param>
		public override void Generate(bool editable)
		{
			var localMaxColumns = this.MaxColumns;

			this.cell.Controls.Clear();

			if ( this.transContext.EnableAutoComplete && this.sublineItem == null )
			{
				var fieldValue = this.GetDataValue();
				var value = fieldValue == null ? string.Empty : fieldValue.ToString();

				var control = new FMAutoComplete
				{
					ID = this.ID,
					Columns = localMaxColumns,
					FieldKey = this.FieldID,
					CssClass = "formfield",
					MaxLength = localMaxColumns,
					Text = value,
					Enabled = editable,
					ClientAutoPost = this.AutoPostBack,
					LineItemNumber = this.lineItem == null ? "na" : this.lineItem.TransactionLineItemGuid.ToString(),
					CallbackAddress = "TransactionDetail.aspx/GetProductsAutoComplete",
					Width = new Unit("150px")
				};

				control.ToolTip = this.DisplayName;

				control.TextChanged += this.TextChanged;

				this.cell.Controls.Add( control );
			}
			else if ( transContext.aliasClass.UseComboxControls == false )
			{
				var updatePanel = new UpdatePanel
				                  {
					                  UpdateMode = UpdatePanelUpdateMode.Conditional, 
									  ID = this.ID + "Panel"
				                  };

				var textBoxButtonCombo = new FMProductTextBox
				                         {
					                         ID				= this.ID,
					                         MaxLength		= this.MaxColumns,
					                         Columns		= this.MaxColumns,
					                         AutoPostBack	= true,
					                         BackColor		= this.VarecBkgrndReadOnlyGray,
					                         Enabled		= editable
				                         };

				textBoxButtonCombo.ToolTip = this.displayName;

				updatePanel.ContentTemplateContainer.Controls.Add(textBoxButtonCombo);
				this.cell.Controls.Add(updatePanel);

				object fieldValue = GetDataValue();

				if (fieldValue != null)
				{
					textBoxButtonCombo.Text = fieldValue.ToString();
				}

				textBoxButtonCombo.TextChanged += this.TextChanged;
			}
			else
			{
				var updatePanel = new UpdatePanel
				                  {
					                  UpdateMode = UpdatePanelUpdateMode.Conditional, 
									  ID = this.ID + "Panel"
				                  };

				var comboBox = new FMComboBox
				               {
					               ID					= this.ID,
					               MaxLength			= 0,
					               AutoPostBack			= true,
					               Enabled				= editable,
					               CssClass				= "formfield txFieldComboBox",
					               RenderMode			= ComboBoxRenderMode.Block,
					               AutoCompleteMode		= ComboBoxAutoCompleteMode.Suggest,
					               DropDownStyle		= ComboBoxStyle.DropDownList,
					               ItemInsertLocation	= ComboBoxItemInsertLocation.OrdinalText
				               };

				comboBox.TextChanged += this.TextChanged;
				comboBox.ItemInserted += this.ItemInserted;

				comboBox.TextBoxCntrl.MaxLength = 0;
				comboBox.TextBoxCntrl.Columns = 0;

				comboBox.ToolTip =  this.displayName;

				updatePanel.ContentTemplateContainer.Controls.Add(comboBox);
				this.cell.Controls.Add(updatePanel);

				if (!cell.Page.IsPostBack || transContext.aliasClass.MultipleLineItems || transContext.reload)
				{
					CompanyClass company = null;
					ProductCollectionClass managerProductCollection = null;
					AdditiveProfileClass additiveProfile = null;
					ProductClass lineItemProduct = null;

					if (sublineItem == null)
					{
						if (trans.ShipToCompanyGuid != Guid.Empty && FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()) == false)
						{
							company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(transContext.security, trans.ShipToCompanyGuid)
																);
						}

						if (trans.SupplierCompanyGuid != Guid.Empty && FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()) == false)
						{
							company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(transContext.security, trans.SupplierCompanyGuid)
																);
						}

						if (transContext.aliasClass.TransTypeID == TransactionTypes.T14_PhysicalInventory
							&& transContext.aliasClass.LineItemFieldCollection.Find("LineItem StorageLocationID") != null
							&& trans.ManagerCompanyGuid != Guid.Empty)
						{
							managerProductCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByManagerAndTanks(transContext.security, trans.ManagerID)
																);
						}
					}

					var fieldValue = GetDataValue();

					comboBox.Items.Add(new ListItem(string.Empty, string.Empty));

					bool itemInList = false;

					var productList = this.GetEntries();

					foreach (ProductClass product in productList)
					{
						if (product.InhibitAccounting)
							continue;

						if (!transContext.accountingSite.CurrentSite.EnableAdditiveAccounting &&
							product.ProductType == ProductType.AdditiveProduct)
						{
							continue;
						}

						if (sublineItem == null)
						{
                            ProductMapClass productMap = transContext.aliasClass.ExcludedProductCollection.Find(x => x.AssignedGuid == product.IdentityGuid);
							if (productMap != null)
							{
								continue;
							}

							if (company != null)
							{
                                ProductMapClass authorizeProductMap = company.AuthorizedProductCollection.Find(x => x.AssignedGuid == product.IdentityGuid);

								if (trans.ShipToCompanyGuid != Guid.Empty && authorizeProductMap == null)
								{
									continue;
								}

                                ProductMapClass supplierAuthorizeProductMap = company.SupplierAuthorizedProductCollection.Find(x => x.AssignedGuid == product.IdentityGuid);

								if (trans.SupplierCompanyGuid != Guid.Empty && supplierAuthorizeProductMap == null)
								{
									continue;
								}
							}

                            if (managerProductCollection != null && managerProductCollection.Find(x => x.IdentityGuid == product.IdentityGuid) == null)
							{
								continue;
							}

							if (product.ProductType != ProductType.ComponentProduct && product.ProductType != ProductType.BlendProduct)
							{
								continue;
							}

						}

						else
						{
							if (lineItemProduct != null
								&& lineItemProduct.ProductType == ProductType.BlendProduct
								&& product.ProductType == ProductType.ComponentProduct)
							{
                                if (lineItemProduct.ComponentCollection.Find(x => x.AssignedGuid == product.IdentityGuid) == null)
								{
									continue;
								}

								bool found = false;
								foreach (SubLineItemDO subLineItemDO in lineItem.SubLineItems)
								{
                                    if (subLineItemDO != sublineItem && subLineItemDO.ProductGuid == product.IdentityGuid)
									{
										found = true;
										break;
									}
								}

								if (found)
								{
									continue;
								}
							}

							if (additiveProfile != null && product.ProductType == ProductType.AdditiveProduct)
							{
                                if (additiveProfile.AdditiveCollection.Find(x => x.AssignedGuid == product.IdentityGuid) == null)
								{
									continue;
								}

								bool found = false;
								foreach (SubLineItemDO subLineItemDO in lineItem.SubLineItems)
								{
									if (subLineItemDO != sublineItem
                                    && subLineItemDO.ProductGuid == product.IdentityGuid)
									{
										found = true;
										break;
									}
								}

								if (found)
								{
									continue;
								}
							}
						}

						comboBox.Items.Add(new ListItem(product.ID, product.ID));

						if (product.ID.Length > comboBox.TextBoxCntrl.Columns && product.ID.Length <= this.MaxColumns)
						{
							comboBox.MaxLength = product.ID.Length;
							comboBox.TextBoxCntrl.MaxLength = product.ID.Length;
							comboBox.TextBoxCntrl.Columns = product.ID.Length;
						}


						if (fieldValue != null && fieldValue.ToString() == product.ID)
						{
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
							comboBox.Text = product.ID;
							itemInList = true;
						}
					}

					if (!itemInList && fieldValue != null && fieldValue.ToString() != "")
					{
						comboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));

						if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length < this.MaxColumns)
						{
							comboBox.MaxLength = fieldValue.ToString().Length;
							comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
							comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
						}
		
						comboBox.SelectedIndex = comboBox.Items.Count - 1;
						comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
						comboBox.Text = fieldValue.ToString();
					}
				}

				if (comboBox.TextBoxCntrl.MaxLength == 0)
				{
					comboBox.MaxLength = this.MaxColumns;
					comboBox.TextBoxCntrl.MaxLength = this.MaxColumns;
					comboBox.TextBoxCntrl.Columns = this.MaxColumns;
				}
			}
		}

		private List<ProductClass> GetEntries()
		{
			CompanyClass company = null;
			ProductCollectionClass managerProductCollection = null;

			var productList = new List<ProductClass>();

			if ( sublineItem == null )
			{
				bool isDescKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());

				if ( !isDescKey )
				{
					if ( trans.SupplierCompanyGuid != Guid.Empty )
					{
						company =
							FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
								x => x.Get(transContext.security, trans.SupplierCompanyGuid, getExtendedInfo: true));
					}
					else if ( trans.ShipToCompanyGuid != Guid.Empty )
					{
						company =
							FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
								x => x.Get( transContext.security, trans.ShipToCompanyGuid, getExtendedInfo: true ) );
					}
				}

				if ( transContext.aliasClass.TransTypeID == TransactionTypes.T14_PhysicalInventory
				&& transContext.aliasClass.LineItemFieldCollection.Find( "LineItem StorageLocationID" ) != null
				&& trans.ManagerCompanyGuid != Guid.Empty )
				{
					managerProductCollection =
						FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
							products => products.EnumerateByManagerAndTanks(transContext.security, trans.ManagerID));
				}
			}

			var fieldValue = GetDataValue();

			ProductCollectionClass productCollection =
				FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(products => products.Enumerate(transContext.security, hideHiddenProducts: true));

			foreach ( ProductClass product in productCollection )
			{
				if (!transContext.accountingSite.CurrentSite.EnableAdditiveAccounting
				    && product.ProductType == ProductType.AdditiveProduct)
				{
					continue;
				}

				if ( sublineItem == null )
				{
					if ( transContext.aliasClass.ExcludedProductCollection.Find( x => x.AssignedGuid == product.IdentityGuid ) != null )
					{
						continue;
					}

					if ( company != null )
					{
						if ( trans.ShipToCompanyGuid != Guid.Empty
							&& company.AuthorizedProductCollection.Find( x => x.AssignedGuid == product.IdentityGuid ) == null )
						{
							continue;
						}

						if ( trans.SupplierCompanyGuid != Guid.Empty
							&& company.SupplierAuthorizedProductCollection.Find( x => x.AssignedGuid == product.IdentityGuid ) == null )
						{
							continue;
						}
					}

					// This one searches by IdentityGuid because it is a product collection rather than a product map collection.
					if ( managerProductCollection != null
						&& managerProductCollection.Find( x => x.IdentityGuid == product.IdentityGuid ) == null )
					{
						continue;
					}

					if ( product.ProductType != ProductType.ComponentProduct
						&& product.ProductType != ProductType.BlendProduct )
					{
						continue;
					}
				}

				productList.Add( product );
			}

			return productList;
		}

		public void SetProduct()
		{
			object fieldValue = GetDataValue();

			if ( transContext.EnableAutoComplete && this.sublineItem == null )
			{
				TextBox box;

				var updatePanel = cell.Controls[0] as UpdatePanel;
				if ( updatePanel != null )
				{
					updatePanel.Update();
					box = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
				}
				else
				{
					box = (TextBox) cell.Controls[0];
				}

				if ( fieldValue != null )
				{
					if (box != null)
					{
						box.Text = fieldValue.ToString();
					}
				}
				else
				{
					if (box != null)
					{
						box.Text = string.Empty;
					}
				}

			}
			else if ( !transContext.aliasClass.UseComboxControls )
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					updatePanel.Update();
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMProductTextBox;

					if (textBoxButtonCombo != null)
					{
						if (fieldValue != null)
						{
							textBoxButtonCombo.Text = fieldValue.ToString();
						}
						else
						{
							textBoxButtonCombo.Text = string.Empty;
						}
					}
				}
			}
			else
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					updatePanel.Update();
					var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

					if (comboBox != null)
					{
						comboBox.Clear();
						comboBox.MaxLength = 0;
						comboBox.Items.Add(new ListItem(string.Empty, string.Empty));

				CompanyClass company = null;
				ProductCollectionClass managerProductCollection = null;
				AdditiveProfileClass additiveProfile = null;
				ProductClass lineItemProduct = null;

						if (this.sublineItem == null)
						{
							if (this.trans.ShipToCompanyGuid != Guid.Empty && FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()) == false)
							{
								company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
									x =>
										x.Get(this.transContext.security, this.trans.ShipToCompanyGuid)
									);
							}

							if (this.trans.SupplierCompanyGuid != Guid.Empty && FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()) == false)
							{
								company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
									x =>
										x.Get(this.transContext.security, this.trans.SupplierCompanyGuid)
									);
							}


					if (transContext.aliasClass.TransTypeID == TransactionTypes.T14_PhysicalInventory
						&& transContext.aliasClass.LineItemFieldCollection.Find("LineItem StorageLocationID") != null
						&& trans.ManagerCompanyGuid != Guid.Empty)
					{
						managerProductCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByManagerAndTanks(transContext.security, trans.ManagerID)
																);

					}
				}

				fieldValue = GetDataValue();

				bool itemInList = false;
				ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(transContext.security, hideHiddenProducts: true)
																);

				foreach (ProductClass product in productCollection)
				{
					if (product.InhibitAccounting)
						continue;

					if (!transContext.accountingSite.CurrentSite.EnableAdditiveAccounting && product.ProductType == ProductType.AdditiveProduct)
					{
						continue;
					}

					if (sublineItem == null)
					{
                        ProductMapClass productMap = transContext.aliasClass.ExcludedProductCollection.Find(x => x.AssignedGuid == product.IdentityGuid);
						if (productMap != null)
						{
							continue;
						}

						if (company != null)
						{
                            ProductMapClass authorizeProductMap = company.AuthorizedProductCollection.Find(x => x.AssignedGuid == product.IdentityGuid);

							if (trans.ShipToCompanyGuid != Guid.Empty && authorizeProductMap == null)
							{
								continue;
							}

                            ProductMapClass supplierAuthorizeProductMap = company.SupplierAuthorizedProductCollection.Find(x => x.AssignedGuid == product.IdentityGuid);

							if (trans.SupplierCompanyGuid != Guid.Empty && supplierAuthorizeProductMap == null)
							{
								continue;
							}
						}

                        if (managerProductCollection != null && managerProductCollection.Find(x => x.IdentityGuid == product.IdentityGuid) == null)
						{
							continue;
						}

						if (product.ProductType != ProductType.ComponentProduct && product.ProductType != ProductType.BlendProduct)
						{
							continue;
						}
					}

					else
					{
						if (lineItemProduct != null
							&& lineItemProduct.ProductType == ProductType.BlendProduct
							&& product.ProductType == ProductType.ComponentProduct)
						{
                            if (lineItemProduct.ComponentCollection.Find(x => x.AssignedGuid == product.IdentityGuid) == null)
							{
								continue;
							}

							bool found = false;
							foreach (SubLineItemDO subLineItemDO in lineItem.SubLineItems)
							{
                                if (subLineItemDO != sublineItem && subLineItemDO.ProductGuid == product.IdentityGuid)
								{
									found = true;
									break;
								}
							}

							if (found)
							{
								continue;
							}
						}

						if (additiveProfile != null
						&& product.ProductType == ProductType.AdditiveProduct)
						{
                            if (additiveProfile.AdditiveCollection.Find(x => x.AssignedGuid == product.IdentityGuid) == null)
							{
								continue;
							}

							bool found = false;
							foreach (SubLineItemDO subLineItemDO in lineItem.SubLineItems)
							{
                                if (subLineItemDO != sublineItem && subLineItemDO.ProductGuid == product.IdentityGuid)
								{
									found = true;
									break;
								}
							}

							if (found)
							{
								continue;
							}
						}
					}

					comboBox.Items.Add(new ListItem(product.ID, product.ID));

							if (product.ID.Length > comboBox.TextBoxCntrl.Columns && product.ID.Length <= this.MaxColumns)
							{
								comboBox.MaxLength = product.ID.Length;
								comboBox.TextBoxCntrl.MaxLength = product.ID.Length;
								comboBox.TextBoxCntrl.Columns = product.ID.Length;
							}

							if (fieldValue != null && fieldValue.ToString() == product.ID)
							{
								comboBox.SelectedIndex = comboBox.Items.Count - 1;
								comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
								comboBox.Text = product.ID;
								itemInList = true;
							}
						}

						if (!itemInList && fieldValue != null && fieldValue.ToString() != string.Empty)
						{
							if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length <= this.MaxColumns)
							{
								comboBox.MaxLength = fieldValue.ToString().Length;
								comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
								comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
							}

							int insertIndex = 0;
							foreach (ListItem item in comboBox.Items)
							{
								if (item.Text != string.Empty && item.Text.CompareTo(fieldValue.ToString()) > 0)
								{
									comboBox.Items.Insert(insertIndex, new ListItem(fieldValue.ToString(), fieldValue.ToString()));
									break;
								}

								insertIndex++;
							}

							if (insertIndex == comboBox.Items.Count)
							{
								comboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));
							}

							comboBox.SelectedIndex = insertIndex;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
							comboBox.Text = fieldValue.ToString();
						}

						if (comboBox.TextBoxCntrl.MaxLength == 0)
						{
							comboBox.MaxLength = this.MaxColumns;
							comboBox.TextBoxCntrl.MaxLength = this.MaxColumns;
							comboBox.TextBoxCntrl.Columns = this.MaxColumns;
						}
					}
				}
			}
		}

		/// <summary>
		/// The purpose of this method is to give auto complete controls access to the 
		/// entries determination logic of this field generator.  Intended for use by
		/// WebMethod on transaction detail page.
		/// </summary>
		/// <param name="startsWith"></param>
		/// <param name="maxRows"></param>
		/// <returns></returns>
		public List<string> GetBaseEntries( string startsWith, int maxRows )
		{
			var products = this.GetEntries();
			int count = 0;
			var productList = new List<string>();

			for ( int index = 0; index < products.Count && count < maxRows; ++index )
			{
				var company = products[index];

				if ( company.ID.StartsWith( startsWith, StringComparison.InvariantCultureIgnoreCase ) )
				{
					productList.Add( company.ID );
					++count;
				}
			}

			return productList;
		}

		/// <summary>
		/// This method will calculated the real field length based on the largest
		/// field length of the entries.  The default is the MaxColumns size.
		/// </summary>
		/// <returns></returns>
		public int RealFieldLength(List<ProductClass> productCollection)
		{
			var localMaxColumns = MaxColumns;

			if (productCollection == null)
			{
				return localMaxColumns;
			}

			int calculatedLength = 0;

			foreach (ProductClass product in productCollection)
			{
				if (string.IsNullOrEmpty(product.ID) == false)
				{
					if (product.ID.Length > calculatedLength)
					{
						calculatedLength = product.ID.Length;
					}

					// Do not allow the length to be greater than
					// the maximum length by the object.
					if (calculatedLength > localMaxColumns)
					{
						calculatedLength = localMaxColumns;
						break;
					}
				}
			}

			if (calculatedLength == 0)
			{
				return localMaxColumns;
			}

			return calculatedLength;
		}

		/// <summary>
		/// This method is an override method that will return the contents of the FMCompanyTextBox
		/// control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override object GetNewValue(WebControl control)
		{
			UpdatePanel updatePanel;

			if ( transContext.EnableAutoComplete && sublineItem == null )
			{
				var textBox = (TextBox) cell.Controls[0];
				return textBox.Text;
			}

			if (!transContext.aliasClass.UseComboxControls)
			{
				updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMProductTextBox;

					if (textBoxButtonCombo != null)
					{
						return textBoxButtonCombo.Text;
					}
				}
			}

			updatePanel = cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

				if (comboBox != null)
				{
					return comboBox.Text;
				}
			}

			return string.Empty;
		}
		#endregion

		#region Protected Methods
		protected void TextChanged(object sender, EventArgs e)
		{
			this.lineItem = null;
			this.sublineItem = null;

			if (cell.ID.Contains("LineItem"))
			{
				if (transContext.aliasClass.MultipleLineItems)
				{
					char[] separatorList = { '.' };
					string[] stringList = cell.Parent.ID.Split(separatorList);
					int lineItemIndex = int.Parse(stringList[0]);
					int sublineItemIndex = int.Parse(stringList[1]);

					if (lineItemIndex > -1)
					{
						if ((this is ILineItemField) || (this is ISublineItemField))
						{
							this.lineItem = this.trans.LineItems[lineItemIndex];
							if (sublineItemIndex > -1) this.sublineItem = this.lineItem.SubLineItems[sublineItemIndex];
						}
					}
				}
				else
				{
					this.lineItem = this.trans.LineItems[0];
				}
			}

			if ( transContext.EnableAutoComplete )
			{
				TextBox textBox;

				var updatePanel = cell.Controls[0] as UpdatePanel;
				if ( updatePanel != null )
				{
					textBox = (TextBox) updatePanel.ContentTemplateContainer.Controls[0];
				}
				else
				{
					textBox = (TextBox) cell.Controls[0];
				}

				SetDataValue( textBox.Text );
			}
			else if ( !transContext.aliasClass.UseComboxControls )
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMProductTextBox;

					if (textBoxButtonCombo != null)
					{
						this.SetDataValue(textBoxButtonCombo.Text);
					}
				}
			}
			else
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

					if (comboBox != null)
					{
						this.SetDataValue(comboBox.Text);
					}
				}
			}
		}

		protected void ItemInserted(object sender, ComboBoxItemInsertEventArgs e)
		{
			var updatePanel = cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;


				if (comboBox != null)
				{
					comboBox.TextBoxCntrl.TextChanged += this.TextChanged;
				}
			}
		}

		/// <summary>
		/// This method will return a product that matches the product ID.
		/// </summary>
		/// <param name="productID"></param>
		/// <returns></returns>
		protected ProductClass GetProductObject(string productID)
		{
			ProductClass product = null;

			// Find the product identityGuid that matches the product ID.
			if (!string.IsNullOrEmpty(productID))
			{
				Guid identityGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.transContext.security, productID)
																);

				if (identityGuid != Guid.Empty)
				{
					product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByInfoAuthorizedCompanies(this.transContext.security, identityGuid, false, false)
																);

				}
			}

			return product;
		}
		#endregion
	}
}

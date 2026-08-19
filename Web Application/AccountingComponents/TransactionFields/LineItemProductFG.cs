
//#define FilterSublineItemProducts
namespace TransactionFields
{
	using System;
	using System.Web.UI.WebControls;

	using FMControls;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.UtilityObjects;
	using Varec.CommonComponents.VolumeCorrection;
	using Varec.CommonComponents.EngineeringUnitsLibrary;


	public class LineItemProductFG : ProductTextButtonGenerator, ILineItemField, ISublineItemField
	{
		#region Public data members
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCT_FG = "CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCT_FG";
		public const string CLIENT_SIDE_KEY_LINEITEM_PRODUCT_FG = "CLIENT_SIDE_KEY_LINEITEM_PRODUCT_FG";
		#endregion

		#region Public Constants
		/// <summary>
		/// Error Message indicated that the product must be selected.
		/// </summary>
		public const string ErrMsg001 = "Must select product";

		/// <summary>
		/// Error Message indicated that the product is invalid.
		/// </summary>
		public const string ErrMsg002 = "Invalid product : {0}";

		/// <summary>
		/// Error message indicated that the product is excluded for this transaction
		/// alias.
		/// </summary>
		public const string ErrMsg003 = "Product {0} is excluded for this alias.";

		/// <summary>
		/// Error message indicated that the product is not authorized for this customer
		/// alias.
		/// </summary>
		public const string ErrMsg004 = "Product {0} is not authorized for this ShipTo customer.";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the product field class.
		/// </summary>
		public LineItemProductFG()
		{
		}
		#endregion

		#region Override properties
		/// <summary>
		/// This property return true if the editable.
		/// </summary>
		override public bool Editable
		{
			get { return true; }
		}

		/// <summary>
		/// This property will return the field ID for the product object.
		/// </summary>
		public override string FieldID
		{
			get { return "LineItem Product"; }
		}

		/// <summary>
		/// This property will return true if the product is required.
		/// </summary>
		public override bool Required
		{
			get { return true; }
		}

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, FieldLength); }
		}
		#endregion

		/// <summary>
		/// This method will return the product ID for an authorized product
		/// object.
		/// </summary>
		/// <param name="authorizedProduct"></param>
		/// <returns></returns>
		protected string GetDataText(ProductMapClass authorizedProduct)
		{
			string productID = string.Empty;

			if (authorizedProduct != null)
			{
				productID = authorizedProduct.AssignedID;
			}

			return productID;
		}

		/// <summary>
		/// This method will return the product ID for a product object.
		/// </summary>
		/// <param name="product"></param>
		/// <returns></returns>
		protected string GetDataText(ProductClass product)
		{
			string productID = string.Empty;

			if (product != null)
			{
				productID = product.ID;
			}

			return productID;
		}

		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control">The product web control.</param>
		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var comboBox = control.Controls[0] as FMComboBox;

			if (comboBox == null)
			{
				return;
			}

			string clientID = comboBox.ClientID;
			TextBox textBox = comboBox.TextBoxCntrl;

			if (textBox == null)
			{
				return;
			}

			clientID = clientID + "_TextBox";

			// Register client scripts for this control if the custom client script registered is registered.
			var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

			if (string.IsNullOrEmpty(customClientScript) == false)
			{
				//Delay client side scripting until page pre-render event in case user clicks edit button of a
				//line item while editing another line item. Such situation causes this method to be called 
				//twice, once for for each line item. Since client side script is  allowed only once to be registered,
				//later line item's client script is ignored, which is the one we actually want.
				comboBox.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCT_FG] =
									"<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
									"var oLineItemProductFGComboBox  = document.getElementById('" + clientID + "'); " +
									"\n//--></script>";

				textBox.Attributes.Add("onBlur", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
			}
		}

		#region ILineItemField Members
		/// <summary>
		/// This method is used to retrieve the product ID from the Line Item
		/// DO. It is used for the grid mode.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns>Returns an object.</returns>
		virtual public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.Product;
		}

		/// <summary>
		/// This method is used to retrieve the product ID from the Line Item
		/// DO. It is used for the grid mode.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <returns>Returns the product ID.</returns>
		virtual public string GetDataText(LineItemDO inLineItem)
		{
			string productID = string.Empty;

			if (inLineItem != null)
			{
				productID = inLineItem.Product;
			}

			return productID;
		}

		virtual protected string GetAllowableEquipmentId(LineItemDO inLineItem, EquipmentDO equipmentDo)
		{
			string equipmentId = string.Empty;

			if (equipmentDo.EquipmentGuid != Guid.Empty)
			{
				EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
															x => x.Get(transContext.security, equipmentDo.EquipmentGuid));

				if (inLineItem.ProductGuid != Guid.Empty && 
					equipment.ProductGuid != Guid.Empty &&
					equipment.ProductGuid != inLineItem.ProductGuid)
				{
					equipmentId = string.Empty;
				}
				else
				{
					equipmentId = equipmentDo.RegistrationID;
				}
			}
			else
			{
				equipmentId = equipmentDo.RegistrationID;
			}

			if (equipmentId == null)
			{
				equipmentId = string.Empty;
			}

			return equipmentId;
		}

		/// <summary>
		/// This method will set the product information in the line item data object.
		/// </summary>
		/// <param name="inLineItem"></param>
		/// <param name="newValue"></param>
		public virtual void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (!(newValue is string) || (newValue as string).Length == 0)
			{
				inLineItem.Product = null;
				inLineItem.ProductCode = null;
				inLineItem.ProductType = null;
				inLineItem.ProductGuid = Guid.Empty;

				inLineItem.SubLineItems.Clear();
				this.RenderErrorMessage(ErrMsg001);
			}
			else
			{
				var productID = newValue as string;
				ProductClass product = this.GetProductObject(productID);

				LoadArmClass loadArm = null;
				StationClass station;
				TankClass tank;


				if (product == null)
				{
					inLineItem.Product = null;
					inLineItem.ProductCode = null;
					inLineItem.ProductType = null;
					inLineItem.ProductGuid = Guid.Empty;

					inLineItem.SubLineItems.Clear();
					this.RenderErrorMessage(string.Format(ErrMsg002, productID));
				}
				else if (this.transContext.aliasClass.IsProductExcluded(product.IdentityGuid))
				{
					inLineItem.Product = null;
					inLineItem.ProductCode = null;
					inLineItem.ProductType = null;
					inLineItem.ProductGuid = Guid.Empty;

					inLineItem.SubLineItems.Clear();
					this.RenderErrorMessage(string.Format(ErrMsg003, product.ID));
				}
				else
				{
					inLineItem.Product = product.ID;
					inLineItem.ProductCode = product.Code;
					inLineItem.ProductType = ProductClass.ProductTypeID(product.ProductType);
					inLineItem.ProductGuid = product.MasterRecordGuid;
					inLineItem.BrokenBlend = false;
					inLineItem.ImproperAdditization = false;

					inLineItem.SubLineItems.Clear();

					var unitsHelper = new UnitsHelperClass(
						transContext.security,
						transContext.accountingSite.CurrentSite,
						transContext.aliasClass,
						product);

					if (inLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
					{
						unitsHelper.SetUnits(inLineItem, ProductType.AdditiveProduct, product);
					}
					else
					{
						unitsHelper.SetUnits(inLineItem, ProductType.ComponentProduct, product);
					}

					if (string.IsNullOrEmpty(lineItem.LoadingLocationID))
					{
						station = null;
						loadArm = null;
					}
					else
					{
						Guid stationGuid = FMChannelHelper.MakeCall<IStations, Guid>(x => x.GetIdentityGuid(base.transContext.security, lineItem.LoadingLocationID));
						if (stationGuid != Guid.Empty)
						{
							station = FMChannelHelper.MakeCall<IStations, StationClass>(x => x.Get(base.transContext.security, stationGuid));

							if (station == null)
							{
								this.RenderErrorMessage(ErrMsg001);
							}
							else
							{
								if (lineItem.ArmNumber != null)
								{
									foreach (LoadArmClass checkedLoadArm in station.LoadArmCollection)
									{
										if (checkedLoadArm.BayAArmNumber == (int)lineItem.ArmNumber.Value)
										{
											loadArm = checkedLoadArm;
											break;
										}
										else if (checkedLoadArm.BayBArmNumber == (int)lineItem.ArmNumber.Value)
										{
											loadArm = checkedLoadArm;
											break;
										}
									}
								}
							}
						}							
					}

					if (transContext.aliasClass.MultipleLineItems 
							&& trans.TransTypeID != TransactionTypes.T17_Order
							&& trans.TransTypeID != TransactionTypes.T18_SupplyOrder
							&& trans.TransTypeID != TransactionTypes.T15_PrimaryRegrade
							&& trans.TransTypeID != TransactionTypes.T16_SecondaryRegrade
							&& product.ProductType == ProductType.ComponentProduct)
					{
						if (loadArm != null)
						{
							ProductMapClass loadArmProductMap = loadArm.ComponentCollection.Find(x => x.AssignedGuid == inLineItem.ProductGuid);
							if (loadArmProductMap != null)
							{
								LineItemMeterIDFG meterFG = base.fieldGenerator.GetFieldGenerator("LineItem MeterID") as LineItemMeterIDFG;
								LineItemDensityFG densityFG = base.fieldGenerator.GetFieldGenerator("LineItem Density") as LineItemDensityFG;
								LineItemTemperatureFG temperatureFG = base.fieldGenerator.GetFieldGenerator("LineItem Temperature") as LineItemTemperatureFG;

								if (meterFG == null)
								{
									inLineItem.MeterID = loadArmProductMap.Meter.ID;
								}
								else
								{
									meterFG.SetDataValue(inLineItem, loadArmProductMap.Meter.ID);
								}

								if (transContext.aliasClass.LineItemFieldCollection.Find("StorageLocationID") == null)
								{
									if (loadArmProductMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP)
									{
										inLineItem.StorageLocationID = loadArmProductMap.TankOrGroupID;
										inLineItem.StorageLocationTankGuid = loadArmProductMap.TankOrGroupGuid;

										tank = FMChannelHelper.MakeCall<ITanks, TankClass>(x => x.Get(transContext.security, loadArmProductMap.TankOrGroupGuid));
										if (tank != null)
										{
											object temperatureObject = this.GetProcessVariableValue(PROCESS_VARIABLE_TYPE.TEMPERATURE_PV, tank);
											object densityObject = this.GetProcessVariableValue(PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV, tank);
											if (typeof(double).IsInstanceOfType(temperatureObject))
											{
												if (temperatureFG == null)
												{
													inLineItem.Temperature = (double)temperatureObject;
												}
												else
												{
													temperatureFG.SetDataValue(inLineItem, temperatureObject);
												}
											}
											if (typeof(double).IsInstanceOfType(densityObject))
											{
												if (densityFG == null)
												{
													inLineItem.Density = (double)densityObject;
												}
												else
												{
													densityFG.SetDataValue(inLineItem, densityObject);
												}
											}
										}
									}
								}
								else
								{
									if (loadArmProductMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP)
									{
										LineItemStorageLocationFG tankFG = base.fieldGenerator.GetFieldGenerator("LineItem StorageLocationID") as LineItemStorageLocationFG;
										tankFG.SetDataValue(inLineItem, loadArmProductMap.TankOrGroupID);
									}
								}
								//calculate LineItem vcf
								CalculateLineItemVCF(inLineItem);
							}
						}
					}

					// If the product is a blend a sub-line item for each
					// component should be added
					if (transContext.aliasClass.MultipleLineItems && trans.TransTypeID != TransactionTypes.T17_Order
					&& trans.TransTypeID != TransactionTypes.T18_SupplyOrder
					&& trans.TransTypeID != TransactionTypes.T15_PrimaryRegrade
					&& trans.TransTypeID != TransactionTypes.T16_SecondaryRegrade
					&& product.ProductType == ProductType.BlendProduct)
					{
						// Now add a sub-line item for each of the product's components
						foreach (ProductMapClass productMap in product.ComponentCollection)
						{
							var subLineItem = new SubLineItemDO
														{
															ArmNumber = inLineItem.ArmNumber,
															BatchNumber = inLineItem.BatchNumber,
															Status = inLineItem.Status,
															Product = productMap.AssignedID,
															ProductCode = productMap.AssignedCode,
															ProductType = ProductClass.ProductTypeID(productMap.AssignedProductType),
															ProductGuid = productMap.AssignedGuid,
															IsEthanol = productMap.IsEthanol
														};

							ProductClass assignedProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(transContext.security, productMap.AssignedGuid));

							subLineItem.VcfModuleSettings = assignedProduct._VcfModuleSettings;

							unitsHelper.SetUnits(subLineItem, productMap.AssignedProductType, assignedProduct);

							if (loadArm != null)
							{
								ProductMapClass loadArmProductMap = loadArm.ComponentCollection.Find(x => x.AssignedGuid == productMap.AssignedGuid);
								if (loadArmProductMap != null)
								{
									subLineItem.MeterID = loadArmProductMap.Meter.ID;
									if (loadArmProductMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP)
									{
										subLineItem.StorageLocationID = loadArmProductMap.TankOrGroupID;
										subLineItem.StorageLocationTankGuid = loadArmProductMap.TankOrGroupGuid;
														
										tank = FMChannelHelper.MakeCall<ITanks, TankClass>(x => x.Get(transContext.security, loadArmProductMap.TankOrGroupGuid));
										if (tank != null)
										{
											object temperatureObject = this.GetProcessVariableValue(PROCESS_VARIABLE_TYPE.TEMPERATURE_PV, tank);
											object densityObject = this.GetProcessVariableValue(PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV, tank);
											if (typeof(double).IsInstanceOfType(temperatureObject))
											{
												subLineItem.Temperature = (double)temperatureObject;
											}
											if (typeof(double).IsInstanceOfType(densityObject))
											{
												subLineItem.Density =(double)densityObject;
											}
										}
									}
								}
								else
								{
									loadArmProductMap = loadArm.ExternalComponentCollection.Find(x => x.AssignedGuid == productMap.AssignedGuid);
									if (loadArmProductMap != null)
									{
										if (loadArmProductMap.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
										{
											subLineItem.StorageLocationID = loadArmProductMap.TankOrGroupID;
											subLineItem.StorageLocationTankGuid = loadArmProductMap.TankOrGroupGuid;
															
											tank = FMChannelHelper.MakeCall<ITanks, TankClass>(x => x.Get(transContext.security, loadArmProductMap.TankOrGroupGuid));
											if (tank != null)
											{
												object temperatureObject = this.GetProcessVariableValue(PROCESS_VARIABLE_TYPE.TEMPERATURE_PV, tank);
												object densityObject = this.GetProcessVariableValue(PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV, tank);
												if (typeof(double).IsInstanceOfType(temperatureObject))
												{
													subLineItem.Temperature = (double)temperatureObject;
												}
												if (typeof(double).IsInstanceOfType(densityObject))
												{
													subLineItem.Density = (double)densityObject;
												}
											}
										}
									}
								}

								//calculate subLineItem vcf
								CalculateSubLineItemVCF(subLineItem);
							}

							inLineItem.SubLineItems.Add(subLineItem);

							if(subLineItem.IsEthanol)
							{
								inLineItem.IsEthanolBlend = true;
							}
						}
					}

					if (this.trans.ShipToCompanyGuid != Guid.Empty)
					{
						CompanyClass shipTo =
							FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
								x => x.Get(this.transContext.security, trans.ShipToCompanyGuid));

						ProductMapClass authorizedProduct =
							shipTo.AuthorizedProductCollection.Find(x => x.AssignedGuid == product.IdentityGuid);

						if (authorizedProduct == null)
						{
							inLineItem.Product = null;
							inLineItem.ProductCode = null;
							inLineItem.ProductType = null;
							inLineItem.ProductGuid = Guid.Empty;

							inLineItem.SubLineItems.Clear();
							this.RenderErrorMessage(string.Format(ErrMsg004, product.ID));
						}
					}

					if (transContext.aliasClass.MultipleLineItems
					&& transContext.aliasClass.LineItemFieldCollection.Find("AdditiveProfileID") != null
					&& trans.TransTypeID != TransactionTypes.T17_Order && trans.TransTypeID != TransactionTypes.T18_SupplyOrder
					&& trans.ShipToCompanyGuid != Guid.Empty)
					{
						CompanyClass shipTo =
							FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(transContext.security, trans.ShipToCompanyGuid));

						ProductMapClass authorizedProduct =
							shipTo.AuthorizedProductCollection.Find(x => x.AssignedGuid == product.IdentityGuid);

						if (authorizedProduct == null)
						{
							inLineItem.AdditiveProfileGuid = Guid.Empty;
							inLineItem.AdditiveProfileID = string.Empty;
							inLineItem.CustomerProductName = string.Empty;
							inLineItem.CustomerProductCode = string.Empty;
						}
						else
						{
							if (authorizedProduct.AdditiveProfileGuid == Guid.Empty)
							{
								inLineItem.AdditiveProfileGuid = Guid.Empty;
								inLineItem.AdditiveProfileID = string.Empty;
							}
							else
							{
								inLineItem.AdditiveProfileGuid = authorizedProduct.AdditiveProfileGuid;
								inLineItem.AdditiveProfileID = authorizedProduct.AdditiveProfileID;

								AdditiveProfileClass additiveProfile =
									FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
										x => x.Get(this.transContext.security, authorizedProduct.AdditiveProfileGuid));

								// Now add a sub-line item for each of the profiles's additives
								foreach (ProductMapClass productMap in additiveProfile.AdditiveCollection)
								{
									var subLineItem = new SubLineItemDO
																{
																	ArmNumber = inLineItem.ArmNumber,
																	BatchNumber = inLineItem.BatchNumber,
																	Status = inLineItem.Status,
																	Product = productMap.AssignedID,
																	ProductCode = productMap.AssignedCode,
																	ProductType = ProductClass.ProductTypeID(productMap.AssignedProductType),
																	IsEthanol = productMap.IsEthanol
																};

									ProductClass assignedProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(transContext.security, productMap.AssignedGuid));

									subLineItem.VcfModuleSettings = assignedProduct._VcfModuleSettings;

									subLineItem.ProductGuid = assignedProduct.MasterRecordGuid;
									unitsHelper.SetUnits(subLineItem, productMap.AssignedProductType, assignedProduct);

									if (loadArm != null)
									{
										ProductMapClass loadArmProductMap = loadArm.AdditiveInjectorCollection.Find(x => x.AssignedGuid == subLineItem.ProductGuid);
										if (loadArmProductMap != null)
										{
											subLineItem.MeterID = loadArmProductMap.Meter.ID;
											if (loadArmProductMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
											{
													subLineItem.StorageLocationID = loadArmProductMap.TankOrGroupID;
													subLineItem.StorageLocationTankGuid = loadArmProductMap.TankOrGroupGuid;
													tank = FMChannelHelper.MakeCall<ITanks, TankClass>(x => x.Get(transContext.security, loadArmProductMap.TankOrGroupGuid));
													if (tank != null)
													{
														object temperatureObject = this.GetProcessVariableValue(PROCESS_VARIABLE_TYPE.TEMPERATURE_PV, tank);
														object densityObject = this.GetProcessVariableValue(PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV, tank);
														if (typeof(double).IsInstanceOfType(temperatureObject))
														{
															subLineItem.Temperature = (double)temperatureObject;
														}
														if (typeof(double).IsInstanceOfType(densityObject))
														{
															subLineItem.Density = (double)densityObject;
														}
													}
											}
										}

										//calculate Addtive subLineItem vcf
										CalculateSubLineItemVCF(subLineItem);
									}

									inLineItem.SubLineItems.Add(subLineItem);

									if (subLineItem.IsEthanol)
									{
										inLineItem.IsEthanolBlend = true;
									}
								}

								inLineItem.CustomerProductName = authorizedProduct.ShipToProductID;
								inLineItem.CustomerProductCode = authorizedProduct.ShipToProductCode;
							}

							inLineItem.CustomerProductName = authorizedProduct.ShipToProductID;
							inLineItem.CustomerProductCode = authorizedProduct.ShipToProductCode;
						}
					}
				}

				if (!transContext.aliasClass.MultipleLineItems && inLineItem.ProductGuid != Guid.Empty)
				{
					if (transContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID1") != null)
					{
						var destinationFG1 = fieldGenerator.GetFieldGenerator("DestinationRegistrationID1") as DestinationEquipmentFG;

						if (destinationFG1 != null)
						{
							destinationFG1.SetValue(this.GetAllowableEquipmentId(inLineItem, this.trans.DestinationEQ1));
						}
					}

					if (transContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID2") != null)
					{
						var destinationFG2 = fieldGenerator.GetFieldGenerator("DestinationRegistrationID2") as DestinationEquipmentFG;

						if (destinationFG2 != null)
						{
							destinationFG2.SetValue(this.GetAllowableEquipmentId(inLineItem, this.trans.DestinationEQ2));
						}
					}

					if (transContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID3") != null)
					{
						var destinationFG3 = fieldGenerator.GetFieldGenerator("DestinationRegistrationID3") as DestinationEquipmentFG;

						if (destinationFG3 != null)
						{
							destinationFG3.SetValue(this.GetAllowableEquipmentId(inLineItem, this.trans.DestinationEQ3));
						}
					}

					if (transContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID1") != null)
					{
						var sourceFG1 = fieldGenerator.GetFieldGenerator("SourceRegistrationID1") as SourceEquipmentFG;

						if (sourceFG1 != null)
						{
							sourceFG1.SetValue(this.GetAllowableEquipmentId(inLineItem, this.trans.SourceEQ1));
						}
					}

					if (transContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID2") != null)
					{
						var sourceFG2 = fieldGenerator.GetFieldGenerator("SourceRegistrationID2") as SourceEquipmentFG;

						if (sourceFG2 != null)
						{
							sourceFG2.SetValue(GetAllowableEquipmentId(inLineItem, trans.SourceEQ2));
						}
					}

					if (transContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID3") != null)
					{
						var sourceFG3 = fieldGenerator.GetFieldGenerator("SourceRegistrationID3") as SourceEquipmentFG;

						if (sourceFG3 != null)
						{
							sourceFG3.SetValue(GetAllowableEquipmentId(inLineItem, trans.SourceEQ3));
						}
					}
				}

				this.SetUnitFields();

				if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem PackageQuantity") != null)
				{
					var packageQuantityFG = fieldGenerator.GetFieldGenerator("LineItem PackageQuantity") as LineItemPackageQuantityFG;
					if (packageQuantityFG != null)
					{
						packageQuantityFG.Generate(false);
					}
				}

				if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem MassPackageSize") != null)
				{
					var massPackageSizeFG = fieldGenerator.GetFieldGenerator("LineItem MassPackageSize") as LineItemMassPackageSizeFG;
					if (massPackageSizeFG != null)
					{
						massPackageSizeFG.SetNewValue(inLineItem);
					}
				}

				if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem VolumePackageSize") != null)
				{
					var volumePackageSizeFG =
						fieldGenerator.GetFieldGenerator("LineItem VolumePackageSize") as LineItemVolumePackageSizeFG;
					if (volumePackageSizeFG != null)
					{
						volumePackageSizeFG.SetNewValue(inLineItem);
					}
				}
			}

			this.SetTank(inLineItem);
			this.SetProduct();
			this.OnFieldChanged();
		}
		#endregion

		protected void CalculateLineItemVCF(LineItemDO lineItem)
		{
			ProductClass product = this.GetProductObject(lineItem.Product);

			if (lineItem.Quantity.VcfManualValueFlag == false || lineItem.Quantity.VcfManualValueFlag == null)
			{
				if (lineItem.Temperature != null
				&& lineItem.Density != null
				&& product != null
				&& product._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE
				&& product._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE_1980)
				{
					double standardDensity = this.ConvertUnits(
					lineItem.Density.Value,
					lineItem.DensityUnits,
					transContext.accountingSite.CurrentSite.DensityUnits);

					try
					{
						Vcf volumeCorrection = new Vcf();

						volumeCorrection.VcfSettings = product._VcfModuleSettings.GetCommonComponentVcfModuleSettings(product.PressureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.PressureUnits : product.PressureUnits);

						lineItem.VCF = volumeCorrection.VcfCalculation((ECorrectionTypeMajor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
						(ECorrectionTypeMinor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
						lineItem.Temperature.Value,
						lineItem.TemperatureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.TemperatureUnits : lineItem.TemperatureUnits,
						product._VcfModuleSettings.BaseTemperature.Value,
						product.TemperatureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.TemperatureUnits : product.TemperatureUnits,
						standardDensity,
						product.DensityUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.DensityUnits : product.DensityUnits,
						0.0, // line item doesn't have pressure available
						product.PressureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.PressureUnits : product.PressureUnits,
						0.0,
						product.TemperatureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.TemperatureUnits : product.TemperatureUnits,
						0.0,
						product.PressureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.PressureUnits : product.PressureUnits,
						new[] { product.CorrectionFactor0, product.CorrectionFactor1, product.CorrectionFactor2, product.CorrectionFactor3, product.CorrectionFactor4 });
					}
					catch (Exception e)
					{
						this.RenderErrorMessage(e.Message);
					}
				}
			}
		}

		protected void CalculateSubLineItemVCF(SubLineItemDO subLineItem)
		{
			ProductClass subLineItemProduct = this.GetProductObject(subLineItem.Product);

			if (subLineItem.Quantity.VcfManualValueFlag == false || subLineItem.Quantity.VcfManualValueFlag == null)
			{
				if (subLineItem.Temperature != null
				&& subLineItem.Density != null
				&& subLineItemProduct !=null
				&& subLineItemProduct._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE
				&& subLineItemProduct._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE_1980)
				{						
					double standardDensity = this.ConvertUnits(
					subLineItem.Density.Value,
					subLineItem.DensityUnits,
					transContext.accountingSite.CurrentSite.DensityUnits);

					try
					{

						Vcf vcf = new Vcf();

						vcf.VcfSettings = subLineItemProduct._VcfModuleSettings.GetCommonComponentVcfModuleSettings(subLineItemProduct.PressureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.PressureUnits : subLineItemProduct.PressureUnits);

						subLineItem.VCF = vcf.VcfCalculation((ECorrectionTypeMajor)Convert.ToInt32(subLineItemProduct._VcfModuleSettings.CorrectionMethodType),
							(ECorrectionTypeMinor)Convert.ToInt32(subLineItemProduct._VcfModuleSettings.CorrectionMethodSpecific),
							subLineItem.Temperature.Value,
							subLineItem.TemperatureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.TemperatureUnits : subLineItem.TemperatureUnits,
							subLineItemProduct._VcfModuleSettings.BaseTemperature.Value,
							subLineItemProduct.TemperatureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.TemperatureUnits : subLineItemProduct.TemperatureUnits,
							standardDensity,
							subLineItemProduct.DensityUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.DensityUnits : subLineItemProduct.DensityUnits,
							0.0, // line item doesn't have pressure available
							subLineItemProduct.PressureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.PressureUnits : subLineItemProduct.PressureUnits,
							0.0,
							subLineItemProduct.TemperatureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.TemperatureUnits : subLineItemProduct.TemperatureUnits,
							0.0,
							subLineItemProduct.PressureUnits == EngineeringUnit.FmSiteUnits ? transContext.accountingSite.CurrentSite.PressureUnits : subLineItemProduct.PressureUnits,
							new[] { subLineItemProduct.CorrectionFactor0, subLineItemProduct.CorrectionFactor1, subLineItemProduct.CorrectionFactor2, subLineItemProduct.CorrectionFactor3, subLineItemProduct.CorrectionFactor4 });
					}
					catch(Exception e)
					{
						this.RenderErrorMessage(e.Message);
					}
				}
			}
		}

		#region ISublineItemField Members
		/// <summary>
		/// This method will return a product object back to the requestor.
		/// </summary>
		/// <param name="inSublineItem"></param>
		/// <returns>Return a product object.</returns>
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Product;
		}

		/// <summary>
		/// This method will return a product ID for the subline item.  It will return an empty string
		/// if the sublineItem is not defined.
		/// </summary>
		/// <param name="inSublineItem"></param>
		/// <returns>Returns a product ID</returns>
		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			string productID = string.Empty;

			if (inSublineItem != null)
			{
				productID = inSublineItem.Product;
			}

			return productID;
		}

		/// <summary>
		/// This method will set the product information in the subline item data object.
		/// </summary>
		/// <param name="inSublineItem">The sub line item to change.</param>
		/// <param name="newValue">The new product value.</param>
		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			if (!(newValue is string) || (newValue as string).Length == 0)
			{
				inSublineItem.Product = null;
				inSublineItem.ProductCode = null;
				inSublineItem.ProductType = null;
				inSublineItem.ProductGuid = Guid.Empty;

				this.RenderErrorMessage(ErrMsg001);
			}

			else
			{
				var productID = newValue as string;
				ProductClass product = this.GetProductObject(productID);

				if (product == null)
				{
					inSublineItem.Product = null;
					inSublineItem.ProductCode = null;
					inSublineItem.ProductType = null;
					inSublineItem.ProductGuid = Guid.Empty;

					this.RenderErrorMessage(string.Format(ErrMsg002, productID));
				}
				else
				{
					inSublineItem.Product = product.ID;
					inSublineItem.ProductCode = product.Code;
					inSublineItem.ProductType = ProductClass.ProductTypeID(product.ProductType);
					inSublineItem.ProductGuid = product.MasterRecordGuid;

					var unitsHelper = new UnitsHelperClass(
						transContext.security,
						transContext.accountingSite.CurrentSite,
						transContext.aliasClass,
						product);
					unitsHelper.SetUnits(inSublineItem, product.ProductType, product);

					if (inSublineItem.ProductType != ProductClass.ProductTypeID(ProductType.AdditiveProduct)
						&& (inSublineItem.ImproperAdditization != null && inSublineItem.ImproperAdditization.Value))
					{
						inSublineItem.ImproperAdditization = false;
						FieldGenerator improperAdditizationObject = this.fieldGenerator.GetFieldGenerator("LineItem ImproperAdditization");

						if (improperAdditizationObject != null)
						{
							var improperAdditizationFG = improperAdditizationObject as ImproperAdditizationFG;
							if (improperAdditizationFG != null)
							{
								((ISublineItemField)improperAdditizationFG).SetDataValue(inSublineItem, false);
							}
						}
					}

					if (inSublineItem.ProductType != ProductClass.ProductTypeID(ProductType.ComponentProduct)
						&& (inSublineItem.BrokenBlend != null && inSublineItem.BrokenBlend.Value))
					{
						inSublineItem.BrokenBlend = false;
						FieldGenerator brokenBlendObject = this.fieldGenerator.GetFieldGenerator("LineItem BrokenBlend");

						if (brokenBlendObject != null)
						{
							var brokenBlendFG = brokenBlendObject as BrokenBlendFG;
							var sublineItemField = (ISublineItemField)brokenBlendFG;

							if (sublineItemField != null)
							{
								sublineItemField.SetDataValue(inSublineItem, false);
							}
						}
					}


					if (inSublineItem.ProductType != ProductClass.ProductTypeID(ProductType.AdditiveProduct)
						&& (inSublineItem.ImproperAdditization != null && inSublineItem.ImproperAdditization.Value))
					{
						inSublineItem.ImproperAdditization = false;
						FieldGenerator improperAdditizationObject = this.fieldGenerator.GetFieldGenerator("LineItem ImproperAdditization");

						if (improperAdditizationObject != null)
						{
							var improperAdditizationFG = improperAdditizationObject as ImproperAdditizationFG;
							var sublineItemField = (ISublineItemField)improperAdditizationFG;

							if (sublineItemField != null)
							{
								sublineItemField.SetDataValue(inSublineItem, false);
							}
						}
					}

					if (inSublineItem.ProductType != ProductClass.ProductTypeID(ProductType.ComponentProduct)
						&& (inSublineItem.BrokenBlend != null && inSublineItem.BrokenBlend.Value))
					{
						inSublineItem.BrokenBlend = false;
						FieldGenerator brokenBlendObject = this.fieldGenerator.GetFieldGenerator("LineItem BrokenBlend");

						if (brokenBlendObject != null)
						{
							var brokenBlendFG = brokenBlendObject as BrokenBlendFG;
							var sublineItemField = (ISublineItemField)brokenBlendFG;

							if (sublineItemField != null)
							{
								sublineItemField.SetDataValue(inSublineItem, false);
							}
						}
					}

					this.SetTank(inSublineItem);
					this.SetProduct();
				}
			}

			OnFieldChanged();
		}

		/// <summary>
		/// This method sets the storage location associated to the product.
		/// </summary>
		/// <param name="inLineItem">Line item that has been changed.</param>
		private void SetTank(LineItemDO inLineItem)
		{
			if (inLineItem.GetType() == typeof(StorageTransferLineItemDO))
			{
				if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem FromStorageLocationID") != null)
				{
					var fromStorageLocationFG =
						fieldGenerator.GetFieldGenerator("LineItem FromStorageLocationID") as LineItemFromStorageLocationFG;

					if (fromStorageLocationFG != null)
					{
						var tankID = fromStorageLocationFG.GetDataValue(inLineItem) as string;

						if (inLineItem.StorageLocationTankGuid != Guid.Empty)
						{
							TankClass tank =
								FMChannelHelper.MakeCall<ITanks, TankClass>(
									x => x.Get(this.transContext.security, inLineItem.StorageLocationTankGuid));

							if (tank.ProductGuid != inLineItem.ProductGuid)
							{
								tankID = string.Empty;
							}
						}

						fromStorageLocationFG.SetDataValue(inLineItem, tankID);
					}
				}

				if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem ToStorageLocationID") != null)
				{
					var toStorageLocationFG =
						fieldGenerator.GetFieldGenerator("LineItem ToStorageLocationID") as LineItemToStorageLocationFG;

					if (toStorageLocationFG != null)
					{
						var tankID = toStorageLocationFG.GetDataValue(inLineItem) as string;

						if (inLineItem.StorageLocationTankGuid != Guid.Empty)
						{
							TankClass tank =
								FMChannelHelper.MakeCall<ITanks, TankClass>(
									x => x.Get(this.transContext.security, inLineItem.StorageLocationTankGuid));

							if (tank.ProductGuid != inLineItem.ProductGuid)
							{
								tankID = string.Empty;
							}
						}

						toStorageLocationFG.SetDataValue(inLineItem, tankID);
					}
				}
			}
			else
			{
				if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem StorageLocationID") != null)
				{
					var storageLocationFG = fieldGenerator.GetFieldGenerator("LineItem StorageLocationID") as LineItemStorageLocationFG;

					if (storageLocationFG != null)
					{
						var tankID = storageLocationFG.GetDataValue(inLineItem) as string;

						if (inLineItem.StorageLocationTankGuid != Guid.Empty)
						{
							TankClass tank =
								FMChannelHelper.MakeCall<ITanks, TankClass>(
									x => x.Get(this.transContext.security, inLineItem.StorageLocationTankGuid));

							if (tank.ProductGuid != inLineItem.ProductGuid)
							{
								tankID = string.Empty;
							}
						}

						storageLocationFG.SetDataValue(inLineItem, tankID);
					}
				}
			}
		}

		/// <summary>
		/// This method sets the storage location associated to the product.
		/// </summary>
		/// <param name="subLineItem">Sub line item that has been changed.</param>
		private void SetTank(SubLineItemDO subLineItem)
		{
			if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem StorageLocationID") != null)
			{
				var storageLocationFG = fieldGenerator.GetFieldGenerator("LineItem StorageLocationID") as LineItemStorageLocationFG;
				var locationFG = (ISublineItemField)storageLocationFG;

				if (locationFG != null)
				{
					var tankID = locationFG.GetDataValue(subLineItem) as string;

					if (subLineItem.StorageLocationTankGuid != Guid.Empty)
					{
						TankClass tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
							x =>
								x.Get(this.transContext.security, subLineItem.StorageLocationTankGuid)
							);


						if (tank.ProductGuid != subLineItem.ProductGuid)
						{
							tankID = string.Empty;
						}
					}

					var sublineItemField = locationFG;

					sublineItemField.SetDataValue(subLineItem, tankID);
				}
			}
		}

		/// <summary>
		/// This method sets the unit field for the product.
		/// </summary>
		private void SetUnitFields()
		{
			if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem VolumeUnit") != null)
			{
				var volumeUnitFG = fieldGenerator.GetFieldGenerator("LineItem VolumeUnit") as LineItemVolumeUnitFG;
				
				if (volumeUnitFG != null)
				{
					volumeUnitFG.Generate(false);
				}
			}

			if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem TemperatureUnit") != null)
			{
				var temperatureUnitFG = fieldGenerator.GetFieldGenerator("LineItem TemperatureUnit") as LineItemTemperatureUnitFG;
				
				if (temperatureUnitFG != null)
				{
					temperatureUnitFG.Generate(false);
				}
			}

			if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem PressureUnit") != null)
			{
				var pressureUnitFG = fieldGenerator.GetFieldGenerator("LineItem PressureUnit") as LineItemPressureUnitFG;
				
				if (pressureUnitFG != null)
				{
					pressureUnitFG.Generate(false);
				}
			}

			if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem DensityUnit") != null)
			{
				var densityUnitFG = fieldGenerator.GetFieldGenerator("LineItem DensityUnit") as LineItemDensityUnitFG;

				if (densityUnitFG != null)
				{
					densityUnitFG.Generate(false);
				}
			}

			if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem MassUnit") != null)
			{
				var massUnitFG = fieldGenerator.GetFieldGenerator("LineItem MassUnit") as LineItemMassUnitFG;

				if (massUnitFG != null)
				{
					massUnitFG.Generate(false);
				}
			}

			if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem LevelUnit") != null)
			{
				var levelUnitFG = fieldGenerator.GetFieldGenerator("LineItem LevelUnit") as LineItemLevelUnitFG;
				
				if (levelUnitFG != null)
				{
					levelUnitFG.Generate(false);
				}
			}

			if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem FlowUnit") != null)
			{
				var flowUnitFG = fieldGenerator.GetFieldGenerator("LineItem FlowUnit") as LineItemFlowUnitFG;
				
				if (flowUnitFG != null)
				{
					flowUnitFG.Generate(false);
				}
			}

			if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem MassQuantity") != null)
			{
				var massQuantityFG = fieldGenerator.GetFieldGenerator("LineItem MassQuantity") as LineItemMassQuantityFG;

				if (massQuantityFG != null)
				{
					massQuantityFG.Generate(false);
				}
			}
		}

		private object GetProcessVariableValue(PROCESS_VARIABLE_TYPE variableType, TankClass tank)
		{
				ProcessVariableClass processVariable =
					tank.ProcessVariableCollection[variableType];

				// The site is needed so that temperature units, density units, temperature
				// decimal places and density decimal places can be used to get the
				// values of the Process Variables			
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(transContext.security, trans.SiteGuid, false, true, false));

				object returnValue = null;

				// Density
				if (variableType == PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV)
				{
					returnValue =
						processVariable.GetValue(site.DensityUnits, site._DensityDecimalPlaces);
				}
				// Temperature
				else if (variableType == PROCESS_VARIABLE_TYPE.TEMPERATURE_PV)
				{
					returnValue =
						processVariable.GetValue(site.TemperatureUnits, site._TemperatureDecimalPlaces);
				}

				return returnValue;
		}
		#endregion
	}
}

namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	using FMControls;

	public class ShipToFG : CompanyTextButtonGenerator, IHeaderField
	{
		public const string CLIENT_SIDE_SCRIPT_SHIPTO = "CLIENT_SIDE_SCRIPT_SHIPTO";
		public const string CLIENT_SIDE_KEY_SHIPTO = "CLIENT_SIDE_KEY_SHIPTO";

		#region Contructors
		/// <summary>
		/// This is the default constructor for the ShipToFG class.
		/// </summary>
		public ShipToFG()
		{
			this.companyRole = SHIPTO_ROLE;
		}
		#endregion

		#region Override Properties
		/// <summary>
		/// This property returns the AutoPostBack
		/// </summary>
		protected override bool AutoPostBack
		{
			get
			{
				return (this.transContext.aliasClass.LimitSelectionsBasedOnHierarchy
				        || this.transContext.aliasClass.TransactionFieldCollection.Find("CarrierID") != null
				        || !this.transContext.aliasClass.MultipleLineItems);
			}
		}

		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID
		{
			get { return "ShipToID"; }
		}

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(FieldID, FIELD_LENGTH); }
		}
		#endregion

		#region Override Methods
		protected override CompanyCollectionClass GetEntries()
		{
            Guid fuelCardGuid = Guid.Empty;

            if (transContext.aliasClass.TransactionFieldCollection.Find("FuelCardID") != null)
            {
                fuelCardGuid = trans.FuelCardGuid;
            }
            CompanyCollectionClass companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
                                                                     x =>
                                                                     x.GetEntriesForFieldGeneratorByRole(transContext.security, COMPANY_ROLE.CUSTOMER_SHIPTO, trans.ShipToCompanyGuid, fuelCardGuid, hideHiddenCompanies: true));

			if (!transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				if (trans.CarrierCompanyGuid != Guid.Empty)
				{
					CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
						x =>
							x.EnumerateByAssignedGuidAndType(this.transContext.security, this.trans.CarrierCompanyGuid,
								COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP));

					var limitedCompanyCollection = new CompanyCollectionClass();

					foreach (CompanyClass company in companyCollection)
					{
						foreach (CompanyMapClass authorizedCarrier in companyMapCollection)
						{
							if (authorizedCarrier.AssignedToGuid == company.MasterRecordGuid)
							{
								limitedCompanyCollection.Add(company);
								break;
							}
						}
					}

					companyCollection = limitedCompanyCollection;
				}
			}

			else
			{
				if (trans.BillToCompanyGuid == Guid.Empty || 
					trans.ShipperCompanyGuid == Guid.Empty || 
					trans.OwnerCompanyGuid == Guid.Empty || 
					trans.ManagerCompanyGuid == Guid.Empty)
				{
					companyCollection.Clear();
				}
				else
				{
					Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
											x =>
											x.GetIdentityGuidByGuidsAndType(transContext.security, 
																			trans.ManagerCompanyGuid, 
																			trans.OwnerCompanyGuid, 
																			COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);

					if (companyMapGuid == Guid.Empty)
					{
						companyCollection.Clear();
					}
					else
					{
						companyMapGuid =
							FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
								x =>
								x.GetIdentityGuidByGuidsAndType(
									transContext.security, companyMapGuid, trans.ShipperCompanyGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP));

						if (companyMapGuid == Guid.Empty)
						{
							companyCollection.Clear();
						}
						else
						{
							companyMapGuid =
								FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
									x =>
									x.GetIdentityGuidByGuidsAndType(
										transContext.security, companyMapGuid, trans.BillToCompanyGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP));

							if (companyMapGuid == Guid.Empty)
							{
								companyCollection.Clear();
							}
							else
							{
								CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
									x => x.EnumerateByAssignedToGuidAndType(this.transContext.security, 
																			companyMapGuid, 
																			COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP));

								var limitedCompanyCollection = new CompanyCollectionClass();
								
								foreach (CompanyClass company in companyCollection)
								{
									if (companyMapCollection.Find(company.MasterRecordGuid) == null)
									{
										continue;
									}

									limitedCompanyCollection.Add(company);
								}

								companyCollection = limitedCompanyCollection;
							}
						}
					}
				}
			}

			return companyCollection;
		}

		protected override void SetCompanyID(TransactionDO transaction, string newID)
		{
			transaction.ShipToID = newID;
		}

		protected override void SetCompanyCode(TransactionDO transaction, string newCode)
		{
			transaction.ShipToCode = newCode;
		}

		protected override void SetCompanyGuid(TransactionDO transaction, Guid newGuid)
		{
			transaction.ShipToCompanyGuid = newGuid;
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return transaction.ShipToID;
		}

		public override void SetDataValue(TransactionDO transaction, object newValue)
		{
			SetValue(newValue);

			if (transContext.aliasClass.TransactionFieldCollection.Find("CarrierID") != null)
			{

				var carrierFG = fieldGenerator.GetFieldGenerator("CarrierID") as CompanyTextButtonGenerator;

				if (transaction.ShipToCompanyGuid == Guid.Empty)
				{
					if (carrierFG != null)
					{
						if (transaction.CarrierCompanyGuid != Guid.Empty)
						{
							carrierFG.SetValue(transaction.CarrierID);
						}
						else
						{
							carrierFG.SetValue(string.Empty);
						}
					}
				}
				else
				{
					if (transaction.CarrierCompanyGuid != Guid.Empty)
					{
						CompanyMapCollectionClass authorizedCarriers =
							FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
								x =>
									x.EnumerateByAssignedToGuidAndType(
										transContext.security,
										transaction.ShipToCompanyGuid,
										COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP));
						bool authorized = false;

						foreach (CompanyMapClass authorizedCarrier in authorizedCarriers)
						{
							if (authorizedCarrier.AssignedGuid == transaction.CarrierCompanyGuid)
							{
								authorized = true;

								if (carrierFG != null)
								{
									carrierFG.SetValue(transaction.CarrierID);
								}
								break;
							}
						}

						if (!authorized && carrierFG != null)
						{
							carrierFG.SetValue(string.Empty);
						}
					}
					else if (carrierFG != null)
					{
						carrierFG.SetValue(string.Empty);
					}
				}
			}

			if (!transContext.aliasClass.MultipleLineItems)
			{
				if (transContext.aliasClass.LineItemFieldCollection.Find("LineItem Product") != null)
				{
					var productFG = fieldGenerator.GetFieldGenerator("LineItem Product") as LineItemProductFG;

					if (productFG != null)
					{
						productFG.SetProduct();
					}
				}
			}

			if (!transContext.aliasClass.LimitSelectionsBasedOnHierarchy && 
				transaction.ShipToCompanyGuid != Guid.Empty)
			{
				CompanyMapCollectionClass shipToBillToMaps = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(transContext.security, 
																										transaction.ShipToCompanyGuid, 
																										COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
																);

				if (shipToBillToMaps.Count == 1)
				{
					CompanyMapClass billToShipperMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(transContext.security, 
																			shipToBillToMaps[0].AssignedToGuid, 
																			COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP)
																);


					if (transContext.aliasClass.TransactionFieldCollection.Find("BillToID") != null)
					{
						var billToFG = fieldGenerator.GetFieldGenerator("BillToID") as CompanyTextButtonGenerator;

						if (billToFG != null)
						{
							billToFG.SetDataValue(transaction, billToShipperMap.AssignedID);
						}
					}
					else
					{
						CompanyClass billTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(transContext.security, billToShipperMap.AssignedGuid, false)
																);
						;
						transaction.BillToID = billTo.ID;
						transaction.BillToCode = billTo.Code;
						transaction.BillToCompanyGuid = billTo.MasterRecordGuid;

						CompanyMapClass shipperOwnerMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(transContext.security, 
																			billToShipperMap.AssignedToGuid, 
																			COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
																);


						if (transContext.aliasClass.TransactionFieldCollection.Find("ShipperID") != null)
						{
							var shipperFG = fieldGenerator.GetFieldGenerator("ShipperID") as CompanyTextButtonGenerator;

							if (shipperFG != null)
							{
								shipperFG.SetDataValue(transaction, shipperOwnerMap.AssignedID);
							}
						}
						else
						{
							CompanyClass shipper = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(transContext.security, shipperOwnerMap.AssignedGuid, false)
																);

							transaction.ShipperID = shipper.ID;
							transaction.ShipperCode = shipper.Code;
							transaction.ShipperCompanyGuid = shipper.MasterRecordGuid;

							CompanyMapClass ownerManagerMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(transContext.security, 
																			shipperOwnerMap.AssignedToGuid, 
																			COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);


							if (transContext.aliasClass.TransactionFieldCollection.Find("OwnerID") != null)
							{
								var ownerFG = fieldGenerator.GetFieldGenerator("OwnerID") as CompanyTextButtonGenerator;

								if (ownerFG != null)
								{
									ownerFG.SetDataValue(transaction, ownerManagerMap.AssignedID);
								}
							}
							else
							{
								CompanyClass owner = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(transContext.security, ownerManagerMap.AssignedGuid, false)
																);

								transaction.OwnerID = owner.ID;
								transaction.OwnerCode = owner.Code;
								transaction.OwnerCompanyGuid = owner.MasterRecordGuid;

								if (transContext.aliasClass.TransactionFieldCollection.Find("ManagerID") != null)
								{
									var managerFG = fieldGenerator.GetFieldGenerator("ManagerID") as CompanyTextButtonGenerator;

									if (managerFG != null)
									{
										managerFG.SetDataValue(transaction, ownerManagerMap.AssignedToID);
									}
								}
								else
								{
									CompanyClass manager = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(transContext.security, ownerManagerMap.AssignedToGuid, false)
																);

									transaction.ManagerID = manager.ID;
									transaction.ManagerCode = manager.Code;
									transaction.ManagerCompanyGuid = manager.MasterRecordGuid;
								}
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control"></param>
		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

				if (comboBox == null)
				{
					return;
				}

				TextBox textBox = comboBox.TextBoxCntrl;

				if (textBox == null)
				{
					return;
				}

				// Register client scripts for this control if the custom client script registered is registered.
				var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

				if (!string.IsNullOrEmpty(customClientScript))
				{

					//Delay client side scripting until page pre-render event in case user clicks edit button of a
					//line item while editing another line item. Such situation causes this method to be called 
					//twice, once for for each line item. Since client side script is  allowed only once to be registered,
					//later line item's client script is ignored, which is the one we actually want.
					comboBox.Page.Session[CLIENT_SIDE_SCRIPT_SHIPTO] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oShipToComboBox  = document.getElementById('" + comboBox.ClientID + "');\n " +
						"\n//--></script>";

					textBox.Attributes.Add("onBlur", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
				}
			}
		}
		#endregion

		#region public methods
		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}
		#endregion
	}
}

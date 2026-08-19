namespace TransactionFields
{
	using System;
	using System.Web.UI;

	using FMControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class ManagerFG : CompanyTextButtonGenerator, IHeaderField
	{
		#region Public data members
		public const string CLIENT_SIDE_SCRIPT_MANAGER_FG = "CLIENT_SIDE_SCRIPT_MANAGER_FG";
		public const string CLIENT_SIDE_KEY_MANAGER_FG = "CLIENT_SIDE_KEY_MANAGER_FG";
		#endregion

		#region Contructors
		/// <summary>
		/// This is the default constructor for the ManagerFG class.
		/// </summary>
		public ManagerFG()
		{
			this.companyRole = MANAGER_ROLE;
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
				return (transContext.aliasClass.LimitSelectionsBasedOnHierarchy || transContext.aliasClass.EnableAutoCompleteControls) ? true : false;
			}
		}

		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "ManagerID";
			}
		}

		/// <summary>
		/// This property return true if the field is required. Otherwise,
		/// it returns false.
		/// </summary>
		public override bool Required
		{
			get
			{
				switch (trans.TransTypeID)
				{
					case TransactionTypes.T1_PrimaryAdjustment:
					case TransactionTypes.T2_SecondaryAdjustment:
					case TransactionTypes.T3_PrimaryDefuel:
					case TransactionTypes.T4_SecondaryDefuel:
					case TransactionTypes.T5_PrimaryDisbursement:
					case TransactionTypes.T6_SecondaryDisbursement:
					case TransactionTypes.T7_FillStand:
					case TransactionTypes.T8_Receipt:
					case TransactionTypes.T9_Request:
					case TransactionTypes.T10_Unload:
					case TransactionTypes.T11_ConsumerTransfer:
					case TransactionTypes.T12_InventoryNotAffected:
					case TransactionTypes.T13_OwnerTransfer:
					case TransactionTypes.T14_PhysicalInventory:
					case TransactionTypes.T15_PrimaryRegrade:
					case TransactionTypes.T16_SecondaryRegrade:
					case TransactionTypes.T17_Order:
					case TransactionTypes.T18_SupplyOrder:
					case TransactionTypes.T23_StorageTransfer:
					case TransactionTypes.T25_Shipment:
						return true;

					default:
						return false;
				}
			}
		}

		/// <summary>
		/// This property returns the field's maximum column width.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, FIELD_LENGTH);
			}
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
                                                                     x.GetEntriesForFieldGeneratorByRole(transContext.security, COMPANY_ROLE.MANAGER, trans.ManagerCompanyGuid, fuelCardGuid, hideHiddenCompanies: true));
			return companyCollection;
		}

		protected override void SetCompanyID(TransactionDO transaction, string newID)
		{
			transaction.ManagerID = newID;
		}

		protected override void SetCompanyCode(TransactionDO transaction, string newCode)
		{
			transaction.ManagerCode = newCode;
		}

		protected override void SetCompanyGuid(TransactionDO transaction, Guid newGuid)
		{
			transaction.ManagerCompanyGuid = newGuid;
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return transaction.ManagerID;
		}

		public override void SetDataValue(TransactionDO transaction, object newValue)
		{
			this.SetValue(newValue);

			if (transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				if (transContext.aliasClass.TransactionFieldCollection.Find("OwnerID") == null)
				{
					return;
				}

				var ownerFG = fieldGenerator.GetFieldGenerator("OwnerID") as CompanyTextButtonGenerator;

				if (transaction.ManagerCompanyGuid == Guid.Empty)
				{
					if (ownerFG != null)
					{
						ownerFG.SetDataValue(transaction, string.Empty);
					}
				}
				else
				{
					CompanyMapCollectionClass companyMapCollection = null;

					if (transaction.TransTypeID == TransactionTypes.T8_Receipt || 
						transaction.TransTypeID == TransactionTypes.T18_SupplyOrder || 
						transaction.TransTypeID == TransactionTypes.T9_Request)
					{
						companyMapCollection =
							FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
												x =>
												x.EnumerateByAssignedToGuidAndType(transContext.security, transaction.ManagerCompanyGuid, COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
																);
					}

					else if (transaction.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
							|| transaction.TransTypeID == TransactionTypes.T6_SecondaryDisbursement
							|| transaction.TransTypeID == TransactionTypes.T11_ConsumerTransfer
							|| transaction.TransTypeID == TransactionTypes.T17_Order
							|| transaction.TransTypeID == TransactionTypes.T25_Shipment)
					{
						companyMapCollection =
							FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
										x =>
										x.EnumerateByAssignedToGuidAndType(transContext.security, transaction.ManagerCompanyGuid, COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
																);
					}

					if (companyMapCollection != null && companyMapCollection.Count == 0)
					{
						if (ownerFG != null)
						{
							ownerFG.SetDataValue(transaction, string.Empty);
						}
					}

					else if (companyMapCollection != null && companyMapCollection.Count == 1)
					{
						if (transaction.OwnerCompanyGuid == Guid.Empty || transaction.OwnerCompanyGuid != companyMapCollection[0].AssignedGuid)
						{
							if (ownerFG != null)
							{
								ownerFG.SetDataValue(transaction, companyMapCollection[0].AssignedID);
							}
						}
					}

					else if (companyMapCollection != null && 
						     (transaction.OwnerCompanyGuid == Guid.Empty || companyMapCollection.Find(transaction.OwnerCompanyGuid) == null))
					{
						if (ownerFG != null)
						{
							ownerFG.SetDataValue(transaction, string.Empty);
						}
					}
				}
			}
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control"></param>
		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = cell.Controls[0] as UpdatePanel;

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
					comboBox.Page.Session[CLIENT_SIDE_SCRIPT_MANAGER_FG] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oManagerFGComboBox  = document.getElementById('" + comboBox.ClientID + "');\n " +
						"\n//--></script>";

					textBox.Attributes.Add("onBlur", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
				}
			}
		}
		#endregion
	}
}

namespace TransactionFields
{
	using System;
	using System.Web.UI;

	using FMControls;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using System.Web.UI.WebControls;

	public class SupplierFG : CompanyTextButtonGenerator, IHeaderField
	{
		#region Public data members
		public const string CLIENT_SIDE_SCRIPT_SUPPLIER_FG = "CLIENT_SIDE_SCRIPT_SUPPLIER_FG";
		public const string CLIENT_SIDE_KEY_SUPPLIER_FG = "CLIENT_SIDE_KEY_SUPPLIER_FG";
		#endregion

		#region Contructors
		/// <summary>
		/// This is the default constructor for the OwnerFG class.
		/// </summary>
		public SupplierFG()
		{
			this.companyRole = SUPPLIER_ROLE;
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
				return true;
			}
		}

		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "SupplierID";
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
			CompanyCollectionClass companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(transContext.security, COMPANY_ROLE.SUPPLIER, false, false)
																);


			if (transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				if (trans.OwnerCompanyGuid == Guid.Empty
				|| trans.ManagerCompanyGuid == Guid.Empty)
				{
					if (transContext.aliasClass.TransactionFieldCollection.Find("OwnerID") != null)
						companyCollection.Clear();
				}
				else
				{
					Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security,
																									trans.ManagerCompanyGuid,
																									trans.OwnerCompanyGuid,
																									COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
																);

					if (companyMapGuid == Guid.Empty)
					{
						companyCollection.Clear();
					}
					else
					{

						CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(transContext.security,
																										companyMapGuid,
																										COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP)
																);

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

			return companyCollection;
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return transaction.SupplierID;
		}

		public override void SetDataValue(TransactionDO transaction, object newValue)
		{
			SetValue(newValue);

			if (!transContext.aliasClass.LimitSelectionsBasedOnHierarchy && transaction.SupplierCompanyGuid != Guid.Empty)
			{
				CompanyMapCollectionClass supplierOwnerMaps = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(transContext.security,
																									transaction.SupplierCompanyGuid,
																									COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP)
																);

				if (supplierOwnerMaps.Count == 1)
				{
					CompanyMapClass ownerManagerMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(transContext.security, 
																			supplierOwnerMaps[0].AssignedToGuid, 
																			COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
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
								managerFG.SetDataValue(transaction, ownerManagerMap.AssignedID);
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

		protected override void SetCompanyID(TransactionDO transaction, string newID)
		{
			transaction.SupplierID = newID;
		}

		protected override void SetCompanyCode(TransactionDO transaction, string newCode)
		{
			transaction.SupplierCode = newCode;
		}

		protected override void SetCompanyGuid(TransactionDO transaction, Guid newGuid)
		{
			transaction.SupplierCompanyGuid = newGuid;
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
					comboBox.Page.Session[CLIENT_SIDE_SCRIPT_SUPPLIER_FG] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oSupplierFGComboBox  = document.getElementById('" + comboBox.ClientID + "');\n " +
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

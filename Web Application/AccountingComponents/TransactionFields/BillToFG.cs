/// <summary>
/// File name:	BillToFG.cs
/// Purpose:	The purpose of this class is to define the Bill TO field.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Thomas Beckum
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	--------------------------------------------
///		2006-11-29		Richard Panachida		Modification to use the company text box button
///														combo field (CSI 3644).
///														
///		2006-12-05		Richard Panachida		Fixed the problem with the company index and code
///		
///		2007-02-15		Richard Panachida		Fixed the "Required" functionality (CSI 3903)
///		
///		2007-09-18		I.Orndorff				7.3.0.0 - Added new transaction type (T18_SupplyOrder).
///		
///		2009-06-03		W.Gray					7.4.6.0 - Change to SetDataValue clear TransPIDXCollection
///														so that collection is reevaluated by Save() TransactionDetail (CSI 3984)
///
///		2009-07-29		W.Gray					Added support for ComboBox controls (WI 4660)
/// </summary>

namespace TransactionFields
{
	using System;
	using System.Web.UI;

	using FMControls;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	public class BillToFG : CompanyTextButtonGenerator, IHeaderField
	{
		#region Public data members
		public const string CLIENT_SIDE_SCRIPT_BILLTO_FG = "CLIENT_SIDE_SCRIPT_BILLTO_FG";
		public const string CLIENT_SIDE_KEY_BILLTO_FG = "CLIENT_SIDE_KEY_BILLTO_FG";
		#endregion

		#region Contructors
		/// <summary>
		/// This is the default constructor for the BillToFG class.
		/// </summary>
		public BillToFG()
		{
			this.companyRole = BILLTO_ROLE;
		}
		#endregion

		#region Override Properties
		/// <summary>
		/// This property returns the AutoPostBack
		/// </summary>
		protected override bool AutoPostBack
		{
			get { return true; }
		}


		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID
		{
			get { return "BillToID"; }
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
																	 x.GetEntriesForFieldGeneratorByRole(transContext.security, COMPANY_ROLE.CUSTOMER_BILLTO, trans.BillToCompanyGuid, fuelCardGuid, hideHiddenCompanies: true));

			if (transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				if (trans.ShipperCompanyGuid == Guid.Empty || trans.OwnerCompanyGuid == Guid.Empty || trans.ManagerCompanyGuid == Guid.Empty)
				{
					companyCollection.Clear();
				}
				else
				{
					Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, trans.ManagerCompanyGuid, trans.OwnerCompanyGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);

					if (companyMapGuid == Guid.Empty)
					{
						companyCollection.Clear();
					}
					else
					{
						companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, companyMapGuid, trans.ShipperCompanyGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
																);

						if (companyMapGuid == Guid.Empty)
						{
							companyCollection.Clear();
						}
						else
						{
							CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(transContext.security, companyMapGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP)
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
			}


			return companyCollection;
		}

		protected override void SetCompanyID(TransactionDO transaction, string newID)
		{
			transaction.BillToID = newID;
		}

		protected override void SetCompanyCode(TransactionDO transaction, string newCode)
		{
			transaction.BillToCode = newCode;
		}

		protected override void SetCompanyGuid(TransactionDO transaction, Guid newGuid)
		{
			transaction.BillToCompanyGuid = newGuid;
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return transaction.BillToID;
		}

		public override void SetDataValue(TransactionDO transaction, object newValue)
		{
			SetValue(newValue);

			if (transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				if (transContext.aliasClass.TransactionFieldCollection.Find("ShipToID") == null)
				{
					return;
				}

				var shipToFG = fieldGenerator.GetFieldGenerator("ShipToID") as CompanyTextButtonGenerator;

				if (transaction.BillToCompanyGuid == Guid.Empty
					|| transaction.ShipperCompanyGuid == Guid.Empty
					|| transaction.OwnerCompanyGuid == Guid.Empty
					|| transaction.ManagerCompanyGuid == Guid.Empty)
				{
					if (shipToFG != null)
					{
						shipToFG.SetDataValue(transaction, string.Empty);
					}

					return;
				}

				Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, transaction.ManagerCompanyGuid, transaction.OwnerCompanyGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);

				if (companyMapGuid == Guid.Empty)
				{
					if (shipToFG != null)
					{
						shipToFG.SetDataValue(transaction, string.Empty);
					}

					return;
				}

				companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, companyMapGuid, transaction.ShipperCompanyGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
																);

				if (companyMapGuid == Guid.Empty)
				{
					if (shipToFG != null)
					{
						shipToFG.SetDataValue(transaction, string.Empty);
					}

					return;
				}

				companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, companyMapGuid, transaction.BillToCompanyGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP)
																);

				if (companyMapGuid == Guid.Empty)
				{
					if (shipToFG != null)
					{
						shipToFG.SetDataValue(transaction, string.Empty);
					}

					return;
				}

				CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(transContext.security, companyMapGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
																);

				if (companyMapCollection.Count == 0)
				{
					if (shipToFG != null)
					{
						shipToFG.SetDataValue(transaction, string.Empty);
					}
				}
				else if (companyMapCollection.Count == 1)
				{
					if (transaction.ShipToCompanyGuid == Guid.Empty || transaction.ShipToCompanyGuid != companyMapCollection[0].AssignedGuid)
					{
						if (shipToFG != null)
						{
							shipToFG.SetDataValue(transaction, companyMapCollection[0].AssignedID);
						}
					}
				}
				else if (transaction.ShipToCompanyGuid == Guid.Empty || companyMapCollection.Find(transaction.ShipToCompanyGuid) == null)
				{
					if (shipToFG != null)
					{
						shipToFG.SetDataValue(transaction, string.Empty);
					}
				}
			}
			else if (transaction.BillToCompanyGuid != Guid.Empty)
			{
				CompanyMapCollectionClass billToShipperMaps = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(transContext.security, transaction.BillToCompanyGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP)
																);

				if (billToShipperMaps.Count == 1)
				{
					CompanyMapClass shipperOwnerMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(transContext.security, billToShipperMaps[0].AssignedToGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
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
																	 x.Get(transContext.security, shipperOwnerMap.AssignedToGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
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


		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control"></param>
		protected override void SpecializeControl(System.Web.UI.WebControls.WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

				if (comboBox != null)
				{
					var textBox = comboBox.TextBoxCntrl;

					if (textBox != null)
					{

						if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()) == true)
						{
							if (this.trans.Alias.ToUpper().Contains("ISSUE")
							|| this.trans.Alias.ToUpper().Contains("COMMERCIAL")
							|| this.trans.Alias.ToUpper().Contains("DIRECT FUEL PURCHASE"))
							{
								this.companySubRole = ADF_SUBROLE;
							}
							else if (this.trans.Alias.ToUpper().Contains("SALE"))
							{
								this.companySubRole = OTHER_SUBROLE;
							}
						}
						// Register client scripts for this control if the custom client script registered is registered.
						var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

						if (!string.IsNullOrEmpty(customClientScript))
						{

							//Delay client side scripting until page pre-render event in case user clicks edit button of a
							//line item while editing another line item. Such situation causes this method to be called 
							//twice, once for for each line item. Since client side script is  allowed only once to be registered,
							//later line item's client script is ignored, which is the one we actually want.
							comboBox.Page.Session[BillToFG.CLIENT_SIDE_SCRIPT_BILLTO_FG] =
													"<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
													"var oBillToFGComboBox  = document.getElementById('" + comboBox.ClientID + "');\n " +
													"\n//--></script>";

							textBox.Attributes.Add("onBlur", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
						}
					}
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

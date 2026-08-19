/// <summary>
/// File name:	ShipperFG.cs
/// Purpose:	The purpose of this class is to define the Shipper field.
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
///														not being stored in the database.
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

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using System.Web.UI.WebControls;

	public class ShipperFG : CompanyTextButtonGenerator, IHeaderField
	{
		#region Public data members
		public const string CLIENT_SIDE_SCRIPT_SHIPPER_FG = "CLIENT_SIDE_SCRIPT_SHIPPER_FG";
		public const string CLIENT_SIDE_KEY_SHIPPER_FG = "CLIENT_SIDE_KEY_SHIPPER_FG";
		#endregion

		#region Contructors
		/// <summary>
		/// This is the default constructor for the ShipperFG class.
		/// </summary>
		public ShipperFG()
		{
			this.companyRole = SHIPPER_ROLE;
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
			get { return "ShipperID"; }
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
                                                                     x.GetEntriesForFieldGeneratorByRole(transContext.security, COMPANY_ROLE.SHIPPER, trans.ShipperCompanyGuid, fuelCardGuid, hideHiddenCompanies: true));

			if (transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				if (trans.OwnerCompanyGuid == Guid.Empty || trans.ManagerCompanyGuid == Guid.Empty)
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
						CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(transContext.security, companyMapGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
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

		protected override void SetCompanyID(TransactionDO transaction, string newID)
		{
			transaction.ShipperID = newID;
		}

		protected override void SetCompanyCode(TransactionDO transaction, string newCode)
		{
			transaction.ShipperCode = newCode;
		}

		protected override void SetCompanyGuid(TransactionDO transaction, Guid newGuid)
		{
			transaction.ShipperCompanyGuid = newGuid;
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return transaction.ShipperID;
		}

		public override void SetDataValue(TransactionDO transaction, object newValue)
		{
			this.SetValue(newValue);

			if (transContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				if (transContext.aliasClass.TransactionFieldCollection.Find("BillToID") == null)
				{
					return;
				}

				var billToFG = fieldGenerator.GetFieldGenerator("BillToID") as CompanyTextButtonGenerator;

				if (transaction.OwnerCompanyGuid == Guid.Empty
					|| transaction.ShipperCompanyGuid == Guid.Empty
					|| transaction.ManagerCompanyGuid == Guid.Empty)
				{
					if (billToFG != null)
					{
						billToFG.SetDataValue(transaction, string.Empty);
					}
				}
				else
				{
					Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, transaction.ManagerCompanyGuid, transaction.OwnerCompanyGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);

					if (companyMapGuid == Guid.Empty)
					{
						if (billToFG != null)
						{
							billToFG.SetDataValue(transaction, string.Empty);
						}
						
						return;
					}

					companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(transContext.security, companyMapGuid, transaction.ShipperCompanyGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
																);

					if (companyMapGuid == Guid.Empty)
					{
						if (billToFG != null)
						{
							billToFG.SetDataValue(transaction, string.Empty);
						}
						
						return;
					}

					CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(transContext.security, companyMapGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP)
																);

					if (companyMapCollection.Count == 0)
					{
						if (billToFG != null)
						{
							billToFG.SetDataValue(transaction, string.Empty);
						}
					}
					else if (companyMapCollection.Count == 1)
					{
						if (transaction.BillToCompanyGuid == Guid.Empty || transaction.BillToCompanyGuid != companyMapCollection[0].AssignedGuid)
						{
							if (billToFG != null)
							{
								billToFG.SetDataValue(transaction, companyMapCollection[0].AssignedID);
							}
						}
					}

					else if (transaction.BillToCompanyGuid == Guid.Empty || companyMapCollection.Find(transaction.BillToCompanyGuid) == null)
					{
						if (billToFG != null)
						{
							billToFG.SetDataValue(transaction, string.Empty);
						}
					}
				}
			}
			else if (transaction.ShipperCompanyGuid != Guid.Empty)
			{
				CompanyMapCollectionClass shipperOwnerMaps = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(transContext.security, transaction.ShipperCompanyGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
																);

				if (shipperOwnerMaps.Count == 1)
				{
					CompanyMapClass ownerManagerMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(transContext.security, shipperOwnerMaps[0].AssignedToGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
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

		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control"></param>
		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = control.Controls[0] as UpdatePanel;

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
				comboBox.Page.Session[CLIENT_SIDE_SCRIPT_SHIPPER_FG] =
												"<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
												"var oShipperFGComboBox  = document.getElementById('" + comboBox.ClientID + "');\n " +
												"\n//--></script>";

				textBox.Attributes.Add("onBlur", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
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

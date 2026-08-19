/// <summary>
/// File name:	CarrierFG.cs
/// Purpose:	The purpose of this class is to define the Carrier field.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Thomas Beckum
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:				By:						Reason:
///		----------		--------------------	--------------------------------------------
///		2006-12-01		Richard Panachida		Modification to use the company text box button
///														combo field (CSI 3644).
///														
///		2006-12-05		Richard Panachida		Fixed the problem with the company index and code
///														not being stored in the database.
///														
///		2009-07-29		W.Gray					Added support for ComboBox controls (WI 4660)
/// </summary>

namespace TransactionFields
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMControls;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	public class CarrierFG : CompanyTextButtonGenerator, IHeaderField
	{
		#region Public data members
		public const string CLIENT_SIDE_SCRIPT_CARRIER_FG = "CLIENT_SIDE_SCRIPT_CARRIER_FG";
		public const string CLIENT_SIDE_KEY_CARRIER_FG = "CLIENT_SIDE_KEY_CARRIER_FG";
		#endregion

		#region Contructors
		/// <summary>
		/// This is the default constructor for the CarrierFG class.
		/// </summary>
		public CarrierFG()
		{
			this.companyRole = CARRIER_ROLE;
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
			get { return "CarrierID"; }
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
			CompanyCollectionClass companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(transContext.security, COMPANY_ROLE.CARRIER, false, false, hideHiddenCompanies: true)
																);

			if (trans.ShipToCompanyGuid != Guid.Empty)
			{
				var shipTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
				x => x.GetBasicInfo(transContext.security, trans.ShipToCompanyGuid, trans.SiteGuid));


				CompanyMapCollectionClass companyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
					x =>
					x.EnumerateByAssignedToGuidAndType(this.transContext.security, shipTo.IdentityGuid, COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP)
					);

				var limitedCompanyCollection = new CompanyCollectionClass();

				foreach(var companyMap in companyMapCollection)
                {
					var company = companyCollection.Find(companyMap.AssignedGuid);
					if(company == null)
                    {
						continue;
                    }
				
					limitedCompanyCollection.Add(company);
				}

				companyCollection = limitedCompanyCollection;
			}

			return companyCollection;
		}

		protected override void SetCompanyID(TransactionDO transaction, string newID)
		{
			transaction.CarrierID = newID;
		}

		protected override void SetCompanyCode(TransactionDO transaction, string newCode)
		{
			transaction.CarrierCode = newCode;
		}

		protected override void SetCompanyGuid(TransactionDO transaction, Guid newGuid)
		{
			transaction.CarrierCompanyGuid = newGuid;
		}

		public override object GetDataValue(TransactionDO transaction)
		{
			return transaction.CarrierID;
		}

		public override void SetDataValue(TransactionDO transaction, object newValue)
		{
			SetValue(newValue);

			if (transContext.aliasClass.TransactionFieldCollection.Find("ShipToID") != null)
			{
				var shipToFG = fieldGenerator.GetFieldGenerator("ShipToID") as CompanyTextButtonGenerator;

				if (transaction.CarrierCompanyGuid == Guid.Empty)
				{
					if (transaction.ShipToCompanyGuid != Guid.Empty)
					{
						if (shipToFG != null)
						{
							shipToFG.SetValue(transaction.ShipToID);
						}
					}
					else
					{
						if (shipToFG != null)
						{
							shipToFG.SetValue(string.Empty);
						}
					}
				}

				else
				{
					if (transaction.ShipToCompanyGuid != Guid.Empty)
					{
						var carrier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
								x => x.GetBasicInfo(transContext.security, transaction.CarrierCompanyGuid, transaction.SiteGuid));
						
						CompanyMapCollectionClass authorizedCarriers =
							FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
								x => x.EnumerateByAssignedGuidAndType(
										transContext.security,
										carrier.IdentityGuid,
										COMPANY_MAP_TYPE.AUTHORIZED_CARRIER_MAP));

						bool authorized = false;

						foreach (CompanyMapClass authorizedCarrier in authorizedCarriers)
						{
							CompanyClass authorizedCarrierCompanyClass =
								FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
									x => x.Get(transContext.security, authorizedCarrier.AssignedToGuid, false, false));

							if (authorizedCarrierCompanyClass.MasterRecordGuid == transaction.ShipToCompanyGuid)
							{
								authorized = true;

								if (shipToFG != null)
								{
									shipToFG.SetValue(transaction.ShipToID);
								}
								break;
							}
						}

						if (!authorized)
						{
							if (shipToFG != null)
							{
								shipToFG.SetValue(string.Empty);
							}
						}
					}
					else
					{
						if (shipToFG != null)
						{
							shipToFG.SetValue(string.Empty);
						}
					}
				}
			}

			if (transContext.aliasClass.TransactionFieldCollection.Find("OperatorID") != null)
			{
				var operatorFG = fieldGenerator.GetFieldGenerator("OperatorID") as OperatorTextButtonGenerator;

				if (operatorFG != null)
				{
					if (transaction.CarrierCompanyGuid != Guid.Empty)
					{
						PersonCollectionClass carrierDrivers = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
																		x =>
																		x.EnumerateByCompany(transContext.security, transaction.CarrierCompanyGuid)
																);

						if (carrierDrivers.Find(x => x.IdentityGuid == transaction.OperatorPersonnelGuid) != null)
						{
							operatorFG.SetValue(transaction.OperatorID);
						}
						else
						{
                            // Defect 117610 - do not clear user's chosen operator if it is not associated with carrier
							//operatorFG.SetValue(string.Empty);
						}
					}
					else
					{
						operatorFG.SetValue(transaction.OperatorID);
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

				if (comboBox != null)
				{
					var textBox = comboBox.TextBoxCntrl;

					if (textBox != null)
					{

						// Register client scripts for this control if the custom client script registered is registered.
						var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

						if (!string.IsNullOrEmpty(customClientScript))
						{

							//Delay client side scripting until page pre-render event in case user clicks edit button of a
							//line item while editing another line item. Such situation causes this method to be called 
							//twice, once for for each line item. Since client side script is  allowed only once to be registered,
							//later line item's client script is ignored, which is the one we actually want.
							comboBox.Page.Session[CLIENT_SIDE_SCRIPT_CARRIER_FG] =
													"<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
													"var oCarrierFGComboBox  = document.getElementById('" + comboBox.ClientID + "');\n " +
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

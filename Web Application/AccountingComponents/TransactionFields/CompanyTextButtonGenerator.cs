/// <summary>
/// File name:	CompanyTextButtonGenerator.cs
/// Purpose:	The purpose of this abstract class is to generate company text field button combination control.
///				It inherits from Field Generator.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:				By:						Reason:
///		----------		--------------------	--------------------------------------------
///		2006-11-29		Richard Panachida		Initial creation (CSI 3644).
///		2006-12-05		Richard Panachida		Fixed the problem with the company index and code
///														not being stored in the database.
///		2006-12-05		Richard Panachida		Set auto post back default to false.
///		
///		2008-04-15		V. Thompson				(CSI 5560)
///														Retrieve the ShowCompanyName value from the 
///														Transaction Alias table.  This value is used to 
///														set the FMCompanyTextBox's ShowCompanyName property
///												
///		2008-06-18		W.Gray					Revision to use Transaction Alias from Transaction Context
///		
///		10-15-2008     V. Thompson          Commented out the place where the textbox's readonly property was set
///     
///		06-12-2009		W.Gray					Revision to provide Text property
///		
///		07-24-2009		W.Gray					Revised to support AJAX ComboBox (WI 4660)
/// </summary>

namespace TransactionFields
{
	using System;
	using System.Globalization;
	using System.Web.UI.WebControls;
	using FMControls;
	using AjaxControlToolkit;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	using System.Collections.Generic;
	using System.Web.UI;

	abstract public class CompanyTextButtonGenerator : FieldGenerator
	{
		#region Public Attributes
		public const string MANAGER_ROLE    = "MANAGER";
		public const string OWNER_ROLE      = "OWNER";
		public const string SHIPPER_ROLE    = "SHIPPER";
		public const string CARRIER_ROLE    = "CARRIER";
		public const string BILLTO_ROLE     = "CUSTOMER_BILLTO";
		public const string SHIPTO_ROLE     = "CUSTOMER_SHIPTO";
		public const string SUPPLIER_ROLE   = "SUPPLIER";
		public const string NONE_ROLE       = "NONE";
		public const short FIELD_LENGTH     = 100;

		// Sub-roles
		public const string ADF_SUBROLE     = "ADF";
		public const string OTHER_SUBROLE   = "OTHER";
		public const string NONE_SUBROLE    = "NONE";
		#endregion

		#region Protected Attributes
		protected string companyRole;
		protected string companySubRole;
		#endregion

		#region Public Properties
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the company text box button combo
		/// abstract class.
		/// </summary>
		public CompanyTextButtonGenerator()
		{
			this.companyRole = NONE_ROLE;
			this.companySubRole = NONE_SUBROLE;
		}
		#endregion

		#region Abstract Properties
		/// <summary>
		/// This property is an abstract property that forces implementation of
		/// returning the maximum column size.
		/// </summary>
		abstract protected short MaxColumns { get; }
		abstract protected bool AutoPostBack { get; }

		#endregion

		#region Abstract Methods
		abstract protected CompanyCollectionClass GetEntries();
		abstract public object GetDataValue(TransactionDO transaction);
		abstract public void SetDataValue(TransactionDO transaction, object newValue);
		abstract protected void SetCompanyID(TransactionDO trans, string newID);
		abstract protected void SetCompanyCode(TransactionDO trans, string newCode);
		abstract protected void SetCompanyGuid(TransactionDO trans, Guid newGuid);
		#endregion

		#region Override methods
		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMCompanyTextBox control is being generated.
		/// </summary>
		/// <param name="editable"></param>
		public override void Generate(bool editable)
		{
			var maxColumns = this.MaxColumns;

			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
			{
				// only applies to shipTo and billTo controls
				if (this.GetType() == typeof(BillToFG) ||
					this.GetType() == typeof(ShipToFG))
				{
					if (transContext.aliasClass.ID.ToUpper().Contains("ISSUE") ||
						transContext.aliasClass.ID.ToUpper().Contains("COMMERCIAL") ||
						transContext.aliasClass.ID.ToUpper().Contains("DIRECT FUEL PURCHASE"))
					{
						companySubRole = ADF_SUBROLE;
					}
					else if (transContext.aliasClass.ID.ToUpper().Contains("SALE"))
					{
						companySubRole = OTHER_SUBROLE;
					}
				}
			}

			if (this.transContext.EnableAutoComplete)
			{
				var fieldValue = this.GetDataValue();
				var value = fieldValue == null ? string.Empty : fieldValue.ToString();

				var control = new FMAutoComplete
				{
					ID				= this.ID,
					Columns			= maxColumns,
					FieldKey		= this.FieldID,
					CssClass		= "formfield",
					MaxLength		= maxColumns,
					Text			= value,
					Enabled			= editable,
					ClientAutoPost	= this.AutoPostBack,
					CallbackAddress = "TransactionDetail.aspx/GetCompaniesAutoComplete",
					Width = new Unit("150px")
				};

				control.TextChanged += this.TextChanged;
				control.ToolTip = this.DisplayName;

				var updatePanel = new UpdatePanel
				{
					UpdateMode = UpdatePanelUpdateMode.Conditional,
					ID = this.ID + "Panel"
				};

				updatePanel.ContentTemplateContainer.Controls.Add(control);
				this.cell.Controls.Add(updatePanel);
			}
			else if (!transContext.aliasClass.UseComboxControls)
			{
				var updatePanel = new UpdatePanel
				                  {
					                  UpdateMode = UpdatePanelUpdateMode.Conditional,
					                  ID = this.ID + "Panel"
				                  };

				var textBoxButtonCombo = new FMCompanyTextBox
				                         {
					                         ID					= this.ID,
					                         MaxLength			= this.MaxColumns,
					                         Columns			= this.MaxColumns,
					                         AutoPostBack		= this.AutoPostBack,
					                         Role				= this.companyRole,
					                         SubRole			= this.companySubRole,
					                         ShowCompanyName	= this.transContext.aliasClass.ShowCompanyName,
					                         BackColor			= this.VarecBkgrndReadOnlyGray,
					                         Enabled			= editable
				                         };

				textBoxButtonCombo.ToolTip = this.DisplayName;
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
				var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };
				var comboBox = new FMComboBox { ID = this.ID, MaxLength = 0 };

                comboBox.TextBoxCntrl.Columns   = this.MaxColumns;
                comboBox.TextBoxCntrl.MaxLength = this.MaxColumns;
				comboBox.AutoPostBack			= AutoPostBack;
				comboBox.Enabled				= editable;
				comboBox.CssClass				= "formfield txFieldComboBox";
				comboBox.RenderMode				= ComboBoxRenderMode.Block;
				comboBox.AutoCompleteMode		= ComboBoxAutoCompleteMode.Suggest;
				comboBox.ToolTip				= this.DisplayName;

				updatePanel.ContentTemplateContainer.Controls.Add(comboBox);
				this.cell.Controls.Add(updatePanel);

				if (transContext.aliasClass.PermitNonReferenceData && this.GetType() != typeof(FromManagerFG)
						&& this.GetType() != typeof(ToManagerFG) && this.GetType() != typeof(ManagerFG)
						&& this.GetType() != typeof(FromOwnerFG) && this.GetType() != typeof(ToOwnerFG)
						&& this.GetType() != typeof(OwnerFG))
				{
					comboBox.DropDownStyle = ComboBoxStyle.DropDown;
				}
				else
				{
					comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				}

				comboBox.ItemInsertLocation = ComboBoxItemInsertLocation.OrdinalText;

				comboBox.TextChanged += this.TextChanged;
				comboBox.ItemInserted += this.ItemInserted;

				if (cell.Page.IsPostBack == false || this.transContext.reload)
				{
					object fieldValue = GetDataValue();
					comboBox.Items.Add(new ListItem(string.Empty, string.Empty));

					bool itemInList = false;
					CompanyCollectionClass companyCollection = GetEntries();

					foreach (CompanyClass company in companyCollection)
					{
						comboBox.Items.Add(new ListItem(company.ID, company.ID));

                        if (!this.TransFieldConfiguration.FileFound)
                        {
                            if (company.ID.Length > comboBox.TextBoxCntrl.Columns && company.ID.Length <= this.MaxColumns)
                            {
                                comboBox.MaxLength = company.ID.Length;
                                comboBox.TextBoxCntrl.Columns = company.ID.Length;
                                comboBox.TextBoxCntrl.MaxLength = company.ID.Length;
                            }
                        }
						if (fieldValue != null && fieldValue.ToString() == company.ID)
						{
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString();
							comboBox.Text = company.ID;
							itemInList = true;
						}
					}

					if (!itemInList && fieldValue != null && fieldValue.ToString() != string.Empty)
					{
						comboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));

                        if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length <= this.MaxColumns)
                        {
                            if (!this.TransFieldConfiguration.FileFound)
                            {
                                comboBox.MaxLength = fieldValue.ToString().Length;
                                comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
                                comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
                            }
                        }

						comboBox.SelectedIndex = comboBox.Items.Count - 1;
						comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString();
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

		/// <summary>
		/// This method is an override method that will return the contents of the control
		/// control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override object GetNewValue(WebControl control)
		{
			if (control == null)
			{
				return null;
			}

			if (this.transContext.EnableAutoComplete)
			{
				var updatePanel = control.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var autoComplete = updatePanel.ContentTemplateContainer.Controls[0] as FMAutoComplete;

					if (autoComplete != null)
					{
						return autoComplete.Text;
					}
				}
			}
			else if (!transContext.aliasClass.UseComboxControls)
			{
				var updatePanel = control.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMCompanyTextBox;

					if (textBoxButtonCombo != null)
					{
						return textBoxButtonCombo.Text;
					}
				}
			}
			else
			{
				string textValue = string.Empty;
				var updatePanel = control.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

					if (comboBox != null)
					{
						TextBox textBox = comboBox.TextBoxCntrl;

						if (textBox != null)
						{
							textValue = textBox.Text;
						}
					}

					return textValue;
				}
			}

			return null;
		}

		/// <summary>
		/// The purpose of this method is to give auto complete controls access to the 
		/// entries determination logic of this field generator.  Intended for use by
		/// WebMethod on transaction detail page.
		/// </summary>
		/// <param name="startsWith"></param>
		/// <param name="maxRows"></param>
		/// <returns></returns>
		public List<string> GetBaseEntries(string startsWith, int maxRows)
		{
			var companies = this.GetEntries();
			int count = 0;
			var companyList = new List<string>();

			for (int index = 0; index < companies.Count && count < maxRows; ++index)
			{
				var company = companies[index];

				if (company.ID.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase))
				{
					companyList.Add(company.ID);
					++count;
				}
			}

			return companyList;
		}

		/// <summary>
		/// This method will set the new value of the control into the transaction
		/// data object.
		/// </summary>
		/// <param name="newValue"></param>
		public void SetValue(object newValue)
		{
			Guid companyGuid = Guid.Empty;

			if (newValue is string && newValue as string != string.Empty)
			{
				companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(transContext.security, newValue as string)
																);
			}

			if (companyGuid == Guid.Empty)
			{
				if (companyGuid == Guid.Empty
					&& (this.GetType() == typeof(FromManagerFG)
					|| this.GetType() == typeof(ToManagerFG)
					|| this.GetType() == typeof(ManagerFG)
					|| this.GetType() == typeof(FromOwnerFG)
					|| this.GetType() == typeof(ToOwnerFG)
					|| this.GetType() == typeof(OwnerFG)))
				{
					SetCompanyID(trans, string.Empty);
				}
				else
				{
					SetCompanyID(trans, newValue as string);

				}
				SetCompanyGuid(trans, Guid.Empty);
				SetCompanyCode(trans, null);
			}

			else
			{
				CompanyClass company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(transContext.security, companyGuid, false));
				company.RoleCollection = FMChannelHelper.MakeCall<ICompanyRoleMaps, CompanyRoleMapCollectionClass>(
																	 x =>
																	 x.EnumerateByCompany(transContext.security, company.MasterRecordGuid)
																);

				if (((this.GetType() == typeof(FromManagerFG)
					|| this.GetType() == typeof(ToManagerFG)
					|| this.GetType() == typeof(ManagerFG))
					&& !company.HasRole(COMPANY_ROLE.MANAGER))
					|| ((this.GetType() == typeof(FromOwnerFG)
					|| this.GetType() == typeof(ToOwnerFG)
					|| this.GetType() == typeof(OwnerFG))
					&& !company.HasRole(COMPANY_ROLE.OWNER))
					|| (this.GetType() == typeof(ShipperFG)
					&& !company.HasRole(COMPANY_ROLE.SHIPPER))
					|| ((this.GetType() == typeof(CarrierFG)
					|| this.GetType() == typeof(ToCarrierFG)
					|| this.GetType() == typeof(FromCarrierFG))
					&& !company.HasRole(COMPANY_ROLE.CARRIER))
					|| ((this.GetType() == typeof(BillToFG)
					|| this.GetType() == typeof(ToBillToFG)
					|| this.GetType() == typeof(FromBillToFG))
					&& !company.HasRole(COMPANY_ROLE.CUSTOMER_BILLTO))
					|| ((this.GetType() == typeof(ShipToFG)
					|| this.GetType() == typeof(ToShipToFG)
					|| this.GetType() == typeof(FromShipToFG))
					&& !company.HasRole(COMPANY_ROLE.CUSTOMER_SHIPTO))
					|| (this.GetType() == typeof(SupplierFG)
					&& !company.HasRole(COMPANY_ROLE.SUPPLIER)))
				{
					SetCompanyID(trans, string.Empty);
					SetCompanyGuid(trans, Guid.Empty);
					SetCompanyCode(trans, null);
				}
				else
				{
					SetCompanyID(trans, company.ID);
					SetCompanyGuid(trans, company.MasterRecordGuid);
					SetCompanyCode(trans, company.Code);
				}
			}

			object fieldValue = GetDataValue();

			if (this.transContext.EnableAutoComplete)
			{
				TextBox textBox;

				var updatePanel = cell.Controls[0] as UpdatePanel;
				if (updatePanel != null)
				{
					updatePanel.Update();
					textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
				}
				else
				{
					textBox = (TextBox)cell.Controls[0];
				}

				if (textBox != null)
				{
					textBox.Text = fieldValue.ToString();
				}
			}
			else if (!transContext.aliasClass.UseComboxControls)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					updatePanel.Update();
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMCompanyTextBox;

					if (fieldValue != null)
					{
						if (textBoxButtonCombo != null)
						{
							textBoxButtonCombo.Text = fieldValue.ToString();
						}
					}
					else
					{
						if (textBoxButtonCombo != null)
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
                        comboBox.MaxLength = this.MaxColumns;
                        comboBox.TextBoxCntrl.Columns = this.MaxColumns;
                        comboBox.TextBoxCntrl.MaxLength = this.MaxColumns;
						comboBox.Items.Add(new ListItem(string.Empty, string.Empty));

						bool itemInList = false;
						CompanyCollectionClass companyCollection = this.GetEntries();

						foreach (CompanyClass company in companyCollection)
						{
							comboBox.Items.Add(new ListItem(company.ID, company.ID));
                            if (!this.TransFieldConfiguration.FileFound)
                            {

                                if (company.ID.Length > comboBox.TextBoxCntrl.Columns && company.ID.Length <= this.MaxColumns)
                                {
                                    comboBox.MaxLength = company.ID.Length;
                                    comboBox.TextBoxCntrl.MaxLength = company.ID.Length;
                                    comboBox.TextBoxCntrl.Columns = company.ID.Length;
                                }
                            }
							if (fieldValue != null  && fieldValue.ToString() == company.ID)
							{
								comboBox.SelectedIndex = comboBox.Items.Count - 1;
								comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString();
								comboBox.Text = company.ID;
								itemInList = true;
							}
						}

						if (!itemInList && fieldValue != null && fieldValue.ToString() != string.Empty)
						{
							comboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));

                            if (!this.TransFieldConfiguration.FileFound)
                            {
                                if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length <= this.MaxColumns)
                                {
                                    comboBox.MaxLength = fieldValue.ToString().Length;
                                    comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
                                    comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
                                }
                            }

                            comboBox.SelectedIndex = comboBox.Items.Count - 1;
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

			OnFieldChanged();
		}

		/// <summary>
		/// This method will calculated the real field length based on the largest
		/// field length of the entries.  The default is the MaxColumns size.
		/// </summary>
		/// <returns></returns>
		public int RealFieldLength(CompanyCollectionClass companyCollection)
		{
			var localMaxColumns = MaxColumns;

			if (companyCollection == null)
			{
				return localMaxColumns;
			}

			int calculatedLength = 0;

			foreach (CompanyClass company in companyCollection)
			{
				if (string.IsNullOrEmpty(company.ID) == false)
				{
					if (company.ID.Length > calculatedLength)
					{
						calculatedLength = company.ID.Length;
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
		#endregion

		#region Protected Methods
		protected void TextChanged(object sender, EventArgs e)
		{
			if (this.transContext.EnableAutoComplete)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;
				var textBox = updatePanel != null ? updatePanel.ContentTemplateContainer.Controls[0] as TextBox : cell.Controls[0] as TextBox;

				if (textBox != null)
				{
					this.SetDataValue(this.trans, textBox.Text);
				}
			}
			else if (!transContext.aliasClass.UseComboxControls)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMCompanyTextBox;

					if (textBoxButtonCombo != null)
					{
						this.SetDataValue(this.trans, textBoxButtonCombo.Text);
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
						this.SetDataValue(this.trans, comboBox.Text);
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
		#endregion
	}
}

namespace TransactionFields
{
	using System;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMControls;
	using AjaxControlToolkit;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	abstract public class TransportCompanyComboBoxGeneratorBase : FieldGenerator
	{
		#region Public Attributes
		public const string MANAGER_ROLE	= "MANAGER";
		public const string OWNER_ROLE		= "OWNER";
		public const string SHIPPER_ROLE	= "SHIPPER";
		public const string CARRIER_ROLE	= "CARRIER";
		public const string BILLTO_ROLE		= "CUSTOMER_BILLTO";
		public const string SHIPTO_ROLE		= "CUSTOMER_SHIPTO";
		public const string SUPPLIER_ROLE	= "SUPPLIER";
		public const string NONE_ROLE		= "NONE";
		public const short FIELD_LENGTH		= 30;
		#endregion

		#region Protected Attributes
		protected string companyRole;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transport company combo box button combo
		/// abstract class.
		/// </summary>
		public TransportCompanyComboBoxGeneratorBase()
		{
			this.companyRole = NONE_ROLE;
		}
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
		abstract protected bool AutoPostBack
		{
			get;
		}
		#endregion

		#region Abstract Methods
		abstract protected CompanyCollectionClass GetEntries();
		abstract public object GetDataValue(TransportLineItemDO transportLineItemDO);
		abstract public void SetDataValue(TransportLineItemDO transportLineItemDO, object newValue);

		abstract protected void SetCompanyID(TransportLineItemDO transportLineItemDO, string newID);
		abstract protected void SetCompanyCode(TransportLineItemDO transportLineItemDO, string newCode);
		abstract protected void SetCompanyGuid(TransportLineItemDO transportLineItemDO, Guid newGuid);
		#endregion

		#region Override methods
		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMCompanyTextBox control is being generated.
		/// </summary>
		/// <param name="editable"></param>
		public override void Generate(bool editable)
		{
			if (transContext.aliasClass.UseComboxControls == false)
			{
				var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };

				var textBoxButtonCombo = new FMCompanyTextBox
				                         {
					                         ID					= this.ID,
					                         MaxLength			= this.MaxColumns,
					                         Columns			= this.MaxColumns,
					                         AutoPostBack		= this.AutoPostBack,
					                         Role				= this.companyRole,
					                         ShowCompanyName	= this.transContext.aliasClass.ShowCompanyName,
					                         BackColor			= this.VarecBkgrndReadOnlyGray,
					                         Enabled			= editable
				                         };

				updatePanel.ContentTemplateContainer.Controls.Add(textBoxButtonCombo);
				this.cell.Controls.Add(updatePanel);

				object fieldValue = GetDataValue();

				if (fieldValue != null)
				{
					textBoxButtonCombo.Text = fieldValue.ToString();
				}
			}
			else
			{
				var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };

				var comboBox = new FMComboBox
				               {
					               ID				= this.ID,
					               MaxLength		= 0,
					               AutoPostBack		= this.AutoPostBack,
					               Enabled			= editable,
					               CssClass			= "formfield txFieldComboBox",
					               RenderMode		= ComboBoxRenderMode.Block,
					               AutoCompleteMode = ComboBoxAutoCompleteMode.Suggest
				               };

				updatePanel.ContentTemplateContainer.Controls.Add(comboBox);
				this.cell.Controls.Add(updatePanel);

				if (transContext.aliasClass.PermitNonReferenceData)
				{
					comboBox.DropDownStyle = ComboBoxStyle.DropDown;
				}
				else
				{
					comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				}

				comboBox.ItemInsertLocation = ComboBoxItemInsertLocation.OrdinalText;


				comboBox.TextChanged += this.TextChanged;
				comboBox.PreRender += this.PreRender;

				if (cell.Page.IsPostBack == false || this.transContext.aliasClass.MultipleTransportLineItems)
				{
					object fieldValue = GetDataValue();

					comboBox.Items.Add(new ListItem(string.Empty, string.Empty));

					string clientScript = "\n<script type='text/javascript'>\n" +
										 "<!-- \n" +
										 "	function Get" + FieldID + "UserData(CompanyID)\n" +
										 "	{\n";

					UserDataFieldCollectionClass userDataFieldCollection = FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
														x =>
														x.EnumerateByEntityType(this.transContext.security,
																				ENTITY_TYPE.COMPANY,
																				Guid.Empty,
																				false,
																				false)
												);

					bool itemInList = false;
					CompanyCollectionClass companyCollection = GetEntries();

					foreach (CompanyClass company in companyCollection)
					{
						string conditionScript = "		if(CompanyID == '" + company.ID + "')\n" +
												"			return '";
						foreach (var fieldClass in userDataFieldCollection)
						{
							var userDataField = (UserDataFieldClass)fieldClass;
							conditionScript += userDataField.DisplayName + "=" + company.UserData[userDataField.Number] + "&";
						}

						conditionScript += "';\n";

						clientScript += conditionScript;
						comboBox.Items.Add(new ListItem(company.ID, company.ID));

						if (company.ID.Length > comboBox.TextBoxCntrl.Columns && company.ID.Length <= this.MaxColumns)
						{
							comboBox.MaxLength = company.ID.Length;
							comboBox.TextBoxCntrl.MaxLength = company.ID.Length;
							comboBox.TextBoxCntrl.Columns = company.ID.Length;
						}


						if (fieldValue != null && fieldValue.ToString() == company.ID)
						{
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
							comboBox.Text = fieldValue.ToString();
							itemInList = true;
						}
					}

					if ((itemInList == false) && (fieldValue != null) && (fieldValue.ToString() != string.Empty))
					{
						comboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));

						if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length < this.MaxColumns)
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

					clientScript += "	}\n" + "-->\n</script>";
					cell.Page.Session[this.GetType() + "COMPANY PARAMETERS"] = clientScript;
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
			UpdatePanel updatePanel;

			if (transContext.aliasClass.UseComboxControls == false)
			{
				updatePanel = control.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMCompanyTextBox;

					if (textBoxButtonCombo != null)
					{
						return textBoxButtonCombo.Text;
					}
				}
			}

			updatePanel = control.Controls[0] as UpdatePanel;

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

		/// <summary>
		/// This method will set the new value of the control into the transaction
		/// data object.
		/// </summary>
		/// <param name="newValue"></param>
		public void SetValue(object newValue)
		{
			Guid companyGuid = Guid.Empty;

			if (newValue is string && (newValue as string != string.Empty))
			{
				companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.transContext.security, newValue as string)
																);
			}

			if (companyGuid == Guid.Empty)
			{
				SetCompanyID(this.transportLineItemDO, null);
				SetCompanyGuid(this.transportLineItemDO, Guid.Empty);
				SetCompanyCode(this.transportLineItemDO, null);
			}

			else
			{
				CompanyClass company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.transContext.security, companyGuid, false)
																);

				SetCompanyID(this.transportLineItemDO, company.ID);
				SetCompanyGuid(this.transportLineItemDO, company.MasterRecordGuid);
				SetCompanyCode(this.transportLineItemDO, company.Code);
			}

			object fieldValue = GetDataValue();

			if (transContext.aliasClass.UseComboxControls == false)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					updatePanel.Update();
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMCompanyTextBox;

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

				UserDataFieldCollectionClass userDataFieldCollection = FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
					x =>
						x.EnumerateByEntityType(this.transContext.security, ENTITY_TYPE.COMPANY, Guid.Empty, false, false)
					);

				string clientScript = "\n<script type='text/javascript'>\n" +
									 "<!-- \n" +
									 "	function Get" + this.FieldID + "UserData(CompanyID)\n" +
									 "	{\n";

				bool itemInList = false;
				CompanyCollectionClass companyCollection = this.GetEntries();

				foreach (CompanyClass company in companyCollection)
				{
					string conditionScript = "		if(CompanyID == '" + company.ID + "')\n" +
											 "			return '";
					foreach (var fieldClass in userDataFieldCollection)
					{
						var userDataField = (UserDataFieldClass)fieldClass;
						conditionScript += userDataField.DisplayName + "=" + company.UserData[userDataField.Number] + "&";
					}

					conditionScript += "';\n";

					clientScript += conditionScript;
					comboBox.Items.Add(new ListItem(company.ID, company.ID));

							if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length <= this.MaxColumns)
							{
								comboBox.MaxLength = fieldValue.ToString().Length;
								comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
								comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
							}

							if (fieldValue.ToString() == company.ID)
							{
								comboBox.SelectedIndex = comboBox.Items.Count - 1;
								comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
								comboBox.Text = fieldValue.ToString();
								itemInList = true;
							}
						}

						if ((itemInList == false) && (fieldValue != null) && (fieldValue.ToString() != string.Empty))
						{
							comboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));

							if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length <= this.MaxColumns)
							{
								comboBox.MaxLength = fieldValue.ToString().Length;
								comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
								comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
							}

					comboBox.SelectedIndex = comboBox.Items.Count - 1;
					comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString();
					comboBox.Text = fieldValue.ToString();
				}

				if (comboBox.TextBoxCntrl.MaxLength == 0)
				{
					comboBox.MaxLength = this.MaxColumns;
					comboBox.TextBoxCntrl.MaxLength = this.MaxColumns;
					comboBox.TextBoxCntrl.Columns = this.MaxColumns;
				}

						clientScript += "	}\n" + "-->\n</script>";
						this.cell.Page.Session[this.GetType() + "COMPANY PARAMETERS"] = clientScript;
					}
				}
			}
		}
		#endregion

		#region Protected Methods
		protected void TextChanged(object sender, EventArgs e)
		{
			if (transContext.aliasClass.UseComboxControls == false)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMCompanyTextBox;

					if (textBoxButtonCombo != null)
					{
						this.SetDataValue(this.transportLineItemDO, textBoxButtonCombo.Text);
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
						this.SetDataValue(this.transportLineItemDO, comboBox.Text);
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

		protected void PreRender(object sender, EventArgs e)
		{
			if (cell.Page.Session[this.GetType() + "COMPANY PARAMETERS"] != null)
			{
				cell.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
																"COMPANY PARAMETERS",
																cell.Page.Session[this.GetType() + "COMPANY PARAMETERS"] as string);
			}
		}
		#endregion
	}
}

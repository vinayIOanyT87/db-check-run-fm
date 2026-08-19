namespace TransactionFields
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using AjaxControlToolkit;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMControls;


	abstract public class EquipmentTextButtonGenerator : FieldGenerator
	{
		#region Public constants
		public const string ERR_MSG_001 = "Invalid Equipment : {0}.";
		public const string ERR_MSG_002 = "Transaction Alias doesn't support {0} Equipment Type : {1}.";
		#endregion

		#region Public Attributes
		public const short FIELD_LENGTH = 30;
		#endregion

		#region Protected Attributes
		protected string equipmentRole;
		protected bool autoPostBack;
		protected bool destination;
		protected byte eqNumber;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the equipment text box button combo
		/// abstract class.
		/// </summary>
		public EquipmentTextButtonGenerator ( bool destination, byte eqNumber )
		{
			this.destination = destination;
			this.eqNumber = eqNumber;
		}
		#endregion

		#region Abstract Properties
		abstract protected EquipmentInfo[] GetEntries ( );
		/// <summary>
		/// This property is an abstract property that forces implementation of
		/// returning the maximum column size.
		/// </summary>
		abstract protected short MaxColumns
		{
			get;
		}
		#endregion


		#region Override methods
		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMEquipmentTextBox control is being generated.
		/// </summary>
		/// <param name="editable"></param>
		public override void Generate ( bool editable )
		{
			if ( this.transContext.EnableAutoComplete )
			{
				this.GenerateAutoCompleteControl(editable);
			}
			else if ( !transContext.aliasClass.UseComboxControls )
			{
				this.GenerateEquipmentTextBoxControl(editable);
			}
			else
			{
				this.GenerateComboBoxControl(editable);
			}
		}

		/// <summary>
		/// This method is an override method that will return the contents of the FMEquipmentTextBox
		/// control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override object GetNewValue ( WebControl control )
		{
			UpdatePanel updatePanel; 

			if ( this.transContext.EnableAutoComplete )
			{
				var autoComplete = (TextBox)control.Controls[0];
				return autoComplete.Text;
			}

			if ( !this.transContext.aliasClass.UseComboxControls )
			{
				updatePanel = control.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMEquipmentTextBox;

					if (textBoxButtonCombo != null)
					{
						return textBoxButtonCombo.Text;
					}
				}
			}

			string textValue = string.Empty;
			updatePanel = control.Controls[0] as UpdatePanel;

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
			}

			return textValue;
		}

		/// <summary>
		/// The purpose of this method is to give auto complete controls access to the 
		/// entries determination logic of this field generator.  Intended for use by
		/// WebMethod on transaction detail page.
		/// </summary>
		/// <param name="startsWith">The entry starts with a particular string.</param>
		/// <param name="maxRows">Max entries to display.</param>
		/// <returns>A list of equipments.</returns>
		public List<string> GetBaseEntries(string startsWith, int maxRows)
		{
			var equipmentEntries = this.GetEntries();
			int count = 0;
			var equipmentList = new List<string>();

			for ( int index = 0; index < equipmentEntries.Length && count < maxRows; ++index )
			{
				var company = equipmentEntries[index];

				if ( company.ID.StartsWith( startsWith, StringComparison.InvariantCultureIgnoreCase ) )
				{
					equipmentList.Add( company.ID );
					++count;
				}
			}

			return equipmentList;
		}

		#endregion

		#region Protected Methods
		protected void TextChanged ( object sender, EventArgs e )
		{
			this.lineItem = null;
			this.sublineItem = null;

			if (cell.ID.Contains ( "LineItem" ))
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

							if (sublineItemIndex > -1)
							{
								this.sublineItem = this.lineItem.SubLineItems[sublineItemIndex];
							}
						}
					}
				}
				else
				{
					this.lineItem = this.trans.LineItems[0];
				}
			}

			if ( transContext.EnableAutoComplete && this.sublineItem == null )
			{
				var textBox = (TextBox)cell.Controls[0];
				this.SetDataValue( textBox.Text );
			}
			else if ( !transContext.aliasClass.UseComboxControls )
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMEquipmentTextBox;

					if (textBoxButtonCombo != null)
					{
						this.SetDataValue ( textBoxButtonCombo.Text );
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
						this.SetDataValue ( comboBox.Text );
					}
				}
			}
		}

		protected void ItemInserted ( object sender, ComboBoxItemInsertEventArgs e )
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
		/// This method sets the equipment value into the equipment data object.
		/// If the equipment ID is not found, then only the new equipment ID is 
		/// saved. Otherwise, the model, type, serial number, etc. are saved in
		/// addition to the ID.
		/// </summary>
		/// <param name="eqID"></param>
		/// <param name="equipmentDO"></param>
		protected void SetEquipment ( string eqID, EquipmentDO equipmentDO )
		{
			equipmentDO.RegistrationID		= string.Empty;
			equipmentDO.EquipmentModel		= null;
			equipmentDO.SerialNumber		= null;
			equipmentDO.CompanyEquipmentID	= null;
			equipmentDO.EquipmentGuid		= Guid.Empty;

			Guid equipmentGuid = Guid.Empty;

			if (!string.IsNullOrEmpty(eqID))
			{
				equipmentGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
					x => x.GetIdentityGuid(transContext.security, eqID));

				equipmentDO.EquipmentRefID = "xxxx";

				if (equipmentGuid == Guid.Empty)
				{
					equipmentDO.RegistrationID = eqID;

					// Get the last 4 characters of the equipment ID in order to create
					// a default equipment reference ID.
					if ((eqID != null) && (eqID.Length >= 4))
					{
						int startIndex = eqID.Length - 4;
						equipmentDO.EquipmentRefID = eqID.Substring(startIndex, 4);
					}
				}

				else
				{
					EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(transContext.security, equipmentGuid)
																);

					var eqTypes = this.transContext.aliasClass.GetEquipmentTypes(this.destination, this.eqNumber);

					// Validate Equipment Type for non-BSME versions
					bool isBsme = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
					if (eqTypes.Length > 0 && !isBsme && !this.transContext.aliasClass.IncludeType(this.destination, this.eqNumber, equipment.Type))
					{
						this.RenderErrorMessage(string.Format(ERR_MSG_002, eqID, EquipmentTypeClass.TypeID(equipment.Type)));
					}
					else
					{
						equipmentDO.RegistrationID = equipment.ID;
						equipmentDO.EquipmentModel = equipment.Model;
						equipmentDO.EquipmentType = EquipmentTypeClass.TypeID(equipment.Type);
						equipmentDO.SerialNumber = equipment.SerialNumber;
						equipmentDO.CompanyEquipmentID = equipment.CompanyEquipmentID;
						equipmentDO.EquipmentGuid = equipment.MasterRecordGuid;
						equipmentDO.EquipmentRefID = equipment.Xref;
					}
				}
			}

			object fieldValue = GetDataValue ( );

			if ( transContext.EnableAutoComplete )
			{
				var textBox = (TextBox)cell.Controls[0];
				if ( fieldValue != null )
				{
					textBox.Text = fieldValue.ToString();
				}
				else
				{
					textBox.Text = string.Empty;
				}
			}
			else if ( !transContext.aliasClass.UseComboxControls )
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					updatePanel.Update();
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMEquipmentTextBox;

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
						comboBox.Clear ( );
						comboBox.MaxLength = 0;
						comboBox.TextBoxCntrl.Columns = 0;
						comboBox.TextBoxCntrl.MaxLength = 0;
						comboBox.Items.Add(new ListItem(string.Empty, string.Empty));
				
						bool itemInList = false;
						EquipmentInfo[] equipmentInfoArray = this.GetEntries ( );

						foreach (EquipmentInfo equipmentInfo in equipmentInfoArray)
						{
							comboBox.Items.Add ( new ListItem ( equipmentInfo.ID, equipmentInfo.ID ) );

							if (!this.TransFieldConfiguration.FileFound)
							{
								if (equipmentInfo.ID.Length > comboBox.TextBoxCntrl.Columns && equipmentInfo.ID.Length <= this.MaxColumns)
								{
									comboBox.MaxLength = equipmentInfo.ID.Length;
									comboBox.TextBoxCntrl.MaxLength = equipmentInfo.ID.Length;
									comboBox.TextBoxCntrl.Columns = equipmentInfo.ID.Length;
								}
							}

							if (fieldValue != null && fieldValue.ToString ( ) == equipmentInfo.ID)
							{
								comboBox.SelectedIndex = comboBox.Items.Count - 1;
								comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString (CultureInfo.InvariantCulture);
								comboBox.Text = equipmentInfo.ID;
								comboBox.TextBoxCntrl.Text = equipmentInfo.ID;
								itemInList = true;
							}
						}

						if (!itemInList && fieldValue != null && fieldValue.ToString ( ) != string.Empty)
						{
							comboBox.Items.Add ( new ListItem ( fieldValue.ToString ( ), fieldValue.ToString ( ) ) );

							if (!this.TransFieldConfiguration.FileFound)
							{
								if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns
								    && fieldValue.ToString().Length <= this.MaxColumns)
								{
									comboBox.MaxLength = fieldValue.ToString().Length;
									comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
									comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
								}
							}

							comboBox.SelectedIndex = comboBox.Items.Count - 1;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString (CultureInfo.InvariantCulture);
							comboBox.Text = fieldValue.ToString ( );
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

			OnFieldChanged ( );
		}

		/// <summary>
		/// This method will retrieve the equipment type from the selected equipment type.
		/// </summary>
		/// <param name="dependentFieldValue">The equipment type that was selected.</param>
		/// <returns>Equipment type that was selected.</returns>
		protected EQUIPMENT_TYPE GetSelectedEquipmentType(string dependentFieldValue)
		{
			var returnEquipmentType = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;

			if (string.IsNullOrEmpty(dependentFieldValue) == false)
			{
				EQUIPMENT_TYPE equipmentType = EquipmentTypeClass.Type(dependentFieldValue);

				if (equipmentType != EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
				{
					returnEquipmentType = equipmentType;
				}
			}

			return returnEquipmentType;
		}
		#endregion

		#region Private methods

		/// <summary>
		/// This method will create an auto complete control.
		/// </summary>
		/// <param name="editable">Edit mode.</param>
		private void GenerateAutoCompleteControl(bool editable)
		{
			var localMaxColumns = this.MaxColumns;
			var fieldValue		= this.GetDataValue();
			var value			= fieldValue == null ? string.Empty : fieldValue.ToString();

			var control = new FMAutoComplete
			{
				ID					= this.ID + eqNumber,
				Columns			= localMaxColumns,
				FieldKey			= this.FieldID,
				CssClass			= "formfield",
				MaxLength		= localMaxColumns,
				Text				= value,
				Enabled			= editable,
				ClientAutoPost	= this.autoPostBack,
				LineItemNumber	= this.lineItem == null ? "na" : this.lineItem.TransactionLineItemGuid.ToString(),
				CallbackAddress= "TransactionDetail.aspx/GetEquipmentAutoComplete",
			};

			control.TextChanged += this.TextChanged;
			this.cell.Controls.Add( control );
		}

		/// <summary>
		/// This method will create an FM Equipment Text Box control.
		/// </summary>
		/// <param name="editable">Edit mode.</param>
		private void GenerateEquipmentTextBoxControl(bool editable)
		{
			var updatePanel = new UpdatePanel { ID = this.ID + "Panel", UpdateMode = UpdatePanelUpdateMode.Conditional };

			var textBoxButtonCombo = new FMEquipmentTextBox
			                         {
				                         ID				= this.ID + this.eqNumber.ToString(CultureInfo.InvariantCulture),
				                         MaxLength		= this.MaxColumns,
				                         Columns		= this.MaxColumns,
				                         AutoPostBack	= this.autoPostBack,
				                         BackColor		= this.VarecBkgrndReadOnlyGray,
				                         Enabled		= editable
			                         };

			object fieldValue = GetDataValue();

			if (fieldValue != null)
			{
				textBoxButtonCombo.Text = fieldValue.ToString();
			}
			textBoxButtonCombo.ToolTip = this.displayName;

			updatePanel.ContentTemplateContainer.Controls.Add(textBoxButtonCombo);
			this.cell.Controls.Add(updatePanel);

			textBoxButtonCombo.AutoPostBack = autoPostBack;
			textBoxButtonCombo.TextChanged += this.TextChanged;
		}

		/// <summary>
		/// This method will create an FM Combo Box control.
		/// </summary>
		/// <param name="editable">Edit mode.</param>
		private void GenerateComboBoxControl(bool editable)
		{
			var updatePanel = new UpdatePanel { ID =ID + "Panel", UpdateMode = UpdatePanelUpdateMode.Conditional };
			var comboBox = new FMComboBox
			               {
				               ID = this.ID + this.eqNumber.ToString(CultureInfo.InvariantCulture),
				               MaxLength = 0
			               };
			comboBox.ToolTip = this.displayName;

			comboBox.TextBoxCntrl.Columns	= 0;
			comboBox.TextBoxCntrl.MaxLength = 0;
			comboBox.AutoPostBack			= this.autoPostBack;
			comboBox.Enabled				= editable;
			comboBox.CssClass				= "formfield txFieldComboBox";
			comboBox.RenderMode				= ComboBoxRenderMode.Block;
			comboBox.AutoCompleteMode		= ComboBoxAutoCompleteMode.Suggest;

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

			comboBox.TextChanged += this.TextChanged;
			comboBox.ItemInserted += this.ItemInserted;

			comboBox.ItemInsertLocation = ComboBoxItemInsertLocation.OrdinalText;

			if (!cell.Page.IsPostBack
				|| ( ( typeof ( LineItemDestinationEquipmentFG ).IsInstanceOfType ( this )
					|| typeof ( LineItemSourceEquipmentFG ).IsInstanceOfType ( this ) )
					&& transContext.aliasClass.MultipleLineItems )
				|| transContext.reload)
			{
				object fieldValue = GetDataValue ( );
				comboBox.Items.Add ( new ListItem (string.Empty, string.Empty) );

				bool itemInList = false;
				EquipmentInfo[] equipmentInfoArray = GetEntries ( );

				foreach (EquipmentInfo equipmentInfo in equipmentInfoArray)
				{
					comboBox.Items.Add ( new ListItem ( equipmentInfo.ID, equipmentInfo.ID ) );

					if (!this.TransFieldConfiguration.FileFound)
					{
						if (equipmentInfo.ID.Length > comboBox.TextBoxCntrl.Columns && equipmentInfo.ID.Length <= this.MaxColumns)
						{
							comboBox.MaxLength = equipmentInfo.ID.Length;
							comboBox.TextBoxCntrl.MaxLength = equipmentInfo.ID.Length;
							comboBox.TextBoxCntrl.Columns = equipmentInfo.ID.Length;
						}
					}

					if (fieldValue != null && fieldValue.ToString ( ) == equipmentInfo.ID)
					{
						comboBox.SelectedIndex = comboBox.Items.Count - 1;
						comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString (CultureInfo.InvariantCulture);
						comboBox.Text = equipmentInfo.ID;
						itemInList = true;
					}
				}

				if (!itemInList && fieldValue != null && fieldValue.ToString ( ) != string.Empty)
				{
					comboBox.Items.Add ( new ListItem ( fieldValue.ToString ( ), fieldValue.ToString ( ) ) );

					if (!this.TransFieldConfiguration.FileFound)
					{
						if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns
						    && fieldValue.ToString().Length <= this.MaxColumns)
						{
							comboBox.MaxLength = fieldValue.ToString().Length;
							comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
							comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
						}
					}

					comboBox.SelectedIndex = comboBox.Items.Count - 1;
					comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString (CultureInfo.InvariantCulture);
					comboBox.Text = fieldValue.ToString ( );
				}
			}

			if (comboBox.TextBoxCntrl.MaxLength == 0)
			{
				comboBox.MaxLength = this.MaxColumns;
				comboBox.TextBoxCntrl.MaxLength = this.MaxColumns;
				comboBox.TextBoxCntrl.Columns = this.MaxColumns;
			}
		}
		#endregion
	}
}

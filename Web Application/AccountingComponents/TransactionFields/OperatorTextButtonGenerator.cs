namespace TransactionFields
{
	using System;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using AjaxControlToolkit;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	abstract public class OperatorTextButtonGenerator : FieldGenerator
	{
		#region Public Attributes
		public const short FIELD_LENGTH = 30;
		#endregion

		#region Protected Attributes
		#endregion

		#region Abstract Properties
		/// <summary>
		/// This property is an abstract property that forces implementation of
		/// returning the maximum column size.
		/// </summary>
		abstract protected short MaxColumns 
		{ 
			get ; 
		}

		protected abstract void SetOperatorID(string newID);

		protected abstract void SetOperatorGuid(Guid newGuid);

		protected abstract void SetSignature(byte[] Signature);

		protected abstract void SetOperatorName(string operatorName);

		protected abstract bool AutoPostBack { get; }

		#endregion

		#region Abstract Methods
		#endregion

		#region Override methods
		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMCompanyTextBox control is being generated.
		/// </summary>
		/// <param name="editable"></param>
		public override void Generate(bool editable)
		{
			var localMaxColumns = MaxColumns;
			
			this.cell.Controls.Clear();

			if ( this.transContext.EnableAutoComplete && this.sublineItem == null )
			{
				var fieldValue = this.GetDataValue();
				var value = fieldValue == null ? string.Empty : fieldValue.ToString();

				var control = new FMAutoComplete
				{
					ID = this.ID,
					Columns = localMaxColumns,
					FieldKey = this.FieldID,
					CssClass = "formfield",
					MaxLength = localMaxColumns,
					Text = value,
					Enabled = editable,
					ClientAutoPost = this.AutoPostBack,
					CallbackAddress = "TransactionDetail.aspx/GetOperatorAutoComplete"
				};

				control.ToolTip = this.DisplayName;

				this.cell.Controls.Add( control );

				object textValue = GetDataValue();

				if ( textValue != null )
				{
					control.Text = textValue.ToString();
				}
			}
			else if ( transContext.aliasClass.UseComboxControls == false )
			{
				var updatePanel = new UpdatePanel
				                  {
					                  UpdateMode = UpdatePanelUpdateMode.Conditional, 
									  ID = this.ID + "Panel"
				                  };

				var textBoxButtonCombo = new FMOperatorTextBox();
				updatePanel.ContentTemplateContainer.Controls.Add(textBoxButtonCombo);
				this.cell.Controls.Add(updatePanel);

				textBoxButtonCombo.ID = this.ID;
				textBoxButtonCombo.MaxLength = MaxColumns;
				textBoxButtonCombo.Columns = MaxColumns;
				textBoxButtonCombo.AutoPostBack = this.AutoPostBack;

				textBoxButtonCombo.BackColor = this.VarecBkgrndReadOnlyGray;
				textBoxButtonCombo.Enabled = editable;
				textBoxButtonCombo.ToolTip = this.DisplayName;

				object fieldValue = GetDataValue();

				if (fieldValue != null)
				{
					textBoxButtonCombo.Text = fieldValue.ToString();
				}
			}
			else
			{
				var updatePanel = new UpdatePanel
				{
					UpdateMode = UpdatePanelUpdateMode.Conditional,
					ID = this.GetType() + "Panel"
				};

				var comboBox = new FMComboBox();
				updatePanel.ContentTemplateContainer.Controls.Add(comboBox);
				this.cell.Controls.Add(updatePanel);

				comboBox.ID = this.GetType().ToString();
				comboBox.MaxLength = 0;
				comboBox.TextBoxCntrl.MaxLength = 0;
				comboBox.TextBoxCntrl.Columns = 0;
				comboBox.AutoPostBack = AutoPostBack;
				comboBox.Enabled = editable;
				comboBox.CssClass = "formfield txFieldComboBox";
				comboBox.RenderMode = ComboBoxRenderMode.Block;
				comboBox.AutoCompleteMode = ComboBoxAutoCompleteMode.Suggest;
				comboBox.ToolTip = this.DisplayName;
				
				comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

				comboBox.ItemInsertLocation = ComboBoxItemInsertLocation.OrdinalText;

				comboBox.TextChanged += this.TextChanged;
				comboBox.ItemInserted += this.ItemInserted;

				if (!cell.Page.IsPostBack || transContext.aliasClass.MultipleLineItems || transContext.reload)
				{

					object fieldValue = GetDataValue();

					comboBox.Items.Add(new ListItem(string.Empty, string.Empty));

					bool itemInList = false;
					PersonCollectionClass personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
																		x =>
																		x.EnumerateByRole(transContext.security, PERSON_ROLE.LOADER_ROLE, hideHiddenPersonnel: true)
																);

					foreach (PersonClass person in personCollection)
					{
						if (trans.CarrierCompanyGuid != Guid.Empty && person.CompanyGuid != trans.CarrierCompanyGuid)
						{
							continue;
						}

						comboBox.Items.Add(new ListItem(person.ID, person.ID));

						if (person.ID.Length > comboBox.TextBoxCntrl.Columns && person.ID.Length <= this.MaxColumns)
						{
							comboBox.MaxLength = person.ID.Length;
							comboBox.TextBoxCntrl.MaxLength = person.ID.Length;
							comboBox.TextBoxCntrl.Columns = person.ID.Length;
						}


						if (fieldValue != null && fieldValue.ToString() == person.ID)
						{
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
							comboBox.Text = person.ID;
							itemInList = true;
						}
					}

                    // add OffLoaders
                    personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
                                                                        x =>
                                                                        x.EnumerateByRole(transContext.security, PERSON_ROLE.OFFLOADER_ROLE, hideHiddenPersonnel: true)
                                                                );

                    foreach (PersonClass person in personCollection)
                    {
                        if (trans.CarrierCompanyGuid != Guid.Empty
                        && person.CompanyGuid != trans.CarrierCompanyGuid)
                        {
                            continue;
                        }

                        // Don't add somebody already in the list
                        if (comboBox.Items.FindByText(person.ID) != null)
                        {
                            continue;
                        }

                        comboBox.Items.Add(new ListItem(person.ID, person.ID));

                        if (person.ID.Length > comboBox.TextBoxCntrl.Columns && person.ID.Length < this.MaxColumns)
                        {
                            comboBox.MaxLength = person.ID.Length;
                            comboBox.TextBoxCntrl.MaxLength = person.ID.Length;
                            comboBox.TextBoxCntrl.Columns = person.ID.Length;
                        }


                        if (fieldValue != null && fieldValue.ToString() == person.ID)
                        {
                            comboBox.SelectedIndex = comboBox.Items.Count - 1;
                            comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString();
                            comboBox.Text = person.ID;
                            itemInList = true;
                        }
                    }

                    if (!itemInList && fieldValue != null && fieldValue.ToString() != string.Empty)
					{
						comboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));

						if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length <= this.MaxColumns)
						{
							comboBox.MaxLength = fieldValue.ToString().Length;
							comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
							comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
						}

						comboBox.SelectedIndex = comboBox.Items.Count - 1;
						comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
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
		/// This method is an override method that will return the contents of the FMCompanyTextBox
		/// control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override object GetNewValue(WebControl control)
		{
			if ( this.transContext.EnableAutoComplete && this.sublineItem == null )
			{
				var textBox = (TextBox)cell.Controls[0];
				return textBox.Text;
			}

			if ( !this.transContext.aliasClass.UseComboxControls )
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBox = updatePanel.ContentTemplateContainer.Controls[0] as FMOperatorTextBox;

					if (textBox != null)
					{
						return textBox.Text;
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
						return comboBox.Text;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// This method will set the new value of the control into the transaction
		/// data object.
		/// </summary>
		/// <param name="newValue"></param>
		public void SetValue(object newValue)
		{
			Guid personGuid = Guid.Empty;

			if (newValue is string && newValue as string != string.Empty)
			{
				personGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(
																	 x =>
																	 x.GetMasterRecordGuid(transContext.security, newValue as string)
																);

			}

			if (personGuid == Guid.Empty)
			{
				if (!this.transContext.aliasClass.PermitNonReferenceData)
				{
					this.SetOperatorID(string.Empty);
				}
				else
				{
					this.SetOperatorID(newValue as string);
				}

				this.SetOperatorGuid(Guid.Empty);
				this.SetOperatorName(string.Empty);
				this.SetSignature(null);
			}
			else
			{
				PersonClass person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																	 x =>
																	 x.Get(transContext.security, personGuid));

				this.SetOperatorID(person.ID);
				this.SetOperatorName(person.FirstName + " " + person.MiddleName + " " + person.LastName);
				this.SetOperatorGuid(personGuid);
				this.SetSignature(person.OnFileSignature);
			}

			object fieldValue = GetDataValue();

			if (transContext.EnableAutoComplete && this.sublineItem == null)
			{
				var textBox = (TextBox)cell.Controls[0];

				textBox.Text = (fieldValue == null) ? string.Empty : fieldValue.ToString();
			}
			else if (!transContext.aliasClass.UseComboxControls)
			{
				var textBoxButtonCombo = cell.Controls[0] as FMOperatorTextBox;

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
			else
			{
				var comboBox = cell.Controls[0] as FMComboBox;

				if (comboBox != null)
				{
					comboBox.Clear();
					comboBox.MaxLength = 0;
					comboBox.TextBoxCntrl.Columns = 0;
					comboBox.TextBoxCntrl.MaxLength = 0;
					comboBox.Items.Add(new ListItem(string.Empty, string.Empty));


					bool itemInList = false;
					PersonCollectionClass personCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
						x =>
							x.EnumerateByRole(this.transContext.security, PERSON_ROLE.LOADER_ROLE)
						);

					foreach (PersonClass person in personCollection)
					{
						if (this.trans.CarrierCompanyGuid != Guid.Empty
						    && person.CompanyGuid != this.trans.CarrierCompanyGuid)
						{
							continue;
						}

						comboBox.Items.Add(new ListItem(person.ID, person.ID));

						if (person.ID.Length > comboBox.TextBoxCntrl.Columns && person.ID.Length <= this.MaxColumns)
						{
							comboBox.MaxLength = person.ID.Length;
							comboBox.TextBoxCntrl.MaxLength = person.ID.Length;
							comboBox.TextBoxCntrl.Columns = person.ID.Length;
						}

						if (fieldValue != null
						    && fieldValue.ToString() == person.ID)
						{
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString();
							comboBox.Text = person.ID;
							itemInList = true;
						}

					}

					if (!itemInList && fieldValue != null && fieldValue.ToString() != string.Empty)
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
				}
			}

			OnFieldChanged();
		}
		#endregion

		#region Protected Methods
		protected void TextChanged(object sender, EventArgs e)
		{
			this.lineItem = null;
			this.sublineItem = null;

			if (cell.ID.Contains("LineItem"))
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

            if (this.transContext.EnableAutoComplete)
            {
                var updatePanel = this.cell.Controls[0] as UpdatePanel;
                var textBox = updatePanel != null ? updatePanel.ContentTemplateContainer.Controls[0] as TextBox : this.cell.Controls[0] as TextBox;

                if (textBox != null)
                {
                    this.SetDataValue(textBox.Text);
                }
            }
            else if (!this.transContext.aliasClass.UseComboxControls)
			{
				var textBoxButtonCombo = cell.Controls[0] as FMOperatorTextBox;

				if (textBoxButtonCombo != null)
				{
					this.SetDataValue(textBoxButtonCombo.Text);
				}
			}
			else
			{
				var comboBox = cell.Controls[0] as ComboBox;

				if (comboBox != null)
				{
					this.SetDataValue(comboBox.Text);
				}
			}
		}

		protected void ItemInserted(object sender, ComboBoxItemInsertEventArgs e)
		{
			var comboBox = cell.Controls[0] as FMComboBox;

			if (comboBox != null)
			{
				comboBox.TextBoxCntrl.TextChanged += this.TextChanged;
			}
		}

		/// <summary>
		/// This method will return a person that matches the person ID.
		/// </summary>
		/// <returns></returns>
		protected PersonClass GetOperatorObject(string personID)
		{
			var person = new PersonClass();

			// Find the person index that matches the person ID.
			if (!string.IsNullOrEmpty(personID))
			{
				Guid personGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(
																	 x =>
																	 x.GetGuidByID(this.transContext.security, personID)
																);

				person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																	 x =>
																	 x.Get(this.transContext.security, personGuid)
																);
			}
			
			return person;
		}
		#endregion
	}
}

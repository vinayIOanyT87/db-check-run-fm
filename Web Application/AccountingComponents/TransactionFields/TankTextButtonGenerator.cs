namespace TransactionFields
{
    using System.Web.UI;

    using AjaxControlToolkit;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMControls;
    using System;
    using System.Globalization;
    using System.Web.UI.WebControls;

    abstract public class TankTextButtonGenerator : FieldGenerator
	{
		#region Public Attributes
		public const short FIELD_LENGTH = 20;
		#endregion

		#region Protected Attributes
		protected bool autoPostBack;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the tank text box button combo
		/// abstract class.
		/// </summary>
		public TankTextButtonGenerator ( )
		{
			this.autoPostBack = false;
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
		#endregion

		#region Abstract Methods
		#endregion

		#region Override methods
		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMCompanyTextBox control is being generated.
		/// </summary>
		/// <param name="editable"></param>
		public override void Generate ( bool editable )
		{
			if (!transContext.aliasClass.UseComboxControls)
			{
				var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };

				var textBoxButtonCombo = new FMTankTextBox
				                         {
					                         ID = this.ID,
					                         MaxLength = this.MaxColumns,
					                         Columns = this.MaxColumns,
					                         AutoPostBack = this.autoPostBack,
					                         Width = new Unit(".5 in", CultureInfo.InvariantCulture),
					                         BackColor = this.VarecBkgrndReadOnlyGray
				                         };

				updatePanel.ContentTemplateContainer.Controls.Add(textBoxButtonCombo);
				this.cell.Controls.Add(updatePanel);

				object fieldValue = GetDataValue ( );

				if (fieldValue != null)
				{
					textBoxButtonCombo.Text = fieldValue.ToString ( );
				}

				textBoxButtonCombo.TextChanged += this.TextChanged;
			}
			else
			{
				var updatePanel = new UpdatePanel
				                          {
					                          UpdateMode = UpdatePanelUpdateMode.Conditional,
					                          ID = this.ID + "Panel"
				                          };

				var comboBox = new FMComboBox
				               {
					               ID = this.ID,
					               MaxLength = 0,
					               AutoPostBack = this.autoPostBack,
					               Enabled = editable,
					               CssClass = "formfield txFieldComboBox",
					               RenderMode = ComboBoxRenderMode.Block,
					               AutoCompleteMode = ComboBoxAutoCompleteMode.Suggest,
					               DropDownStyle = ComboBoxStyle.DropDownList,
					               ItemInsertLocation = ComboBoxItemInsertLocation.OrdinalText
				               };

				comboBox.TextBoxCntrl.Columns = 0;
				comboBox.TextBoxCntrl.MaxLength = 0;
				comboBox.TextChanged += this.TextChanged;

				updatePanel.ContentTemplateContainer.Controls.Add(comboBox);
				this.cell.Controls.Add(updatePanel);

				if (!cell.Page.IsPostBack ||
					(typeof(LineItemStorageLocationFG).IsInstanceOfType(this)  &&
						 transContext.aliasClass.MultipleLineItems) ||
						 transContext.reload)
				{
					object fieldValue = GetDataValue ( );
					comboBox.Items.Add ( new ListItem ( string.Empty, string.Empty ) );

					bool itemInList = false;
					Guid? productGuid = null;

					if (sublineItem != null)
					{
						productGuid = sublineItem.ProductGuid;
					}
					else if (lineItem != null)
					{
						productGuid = lineItem.ProductGuid;
					}

                    TankCollectionClass tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
                                                                     x =>
                                                                     x.Enumerate(transContext.security, hideHiddenTanks: true)
                                                                );

                    foreach (TankClass tank in tankCollection)
                    {
                        if (productGuid != null && (Guid)productGuid.Value != tank.ProductGuid)
                        {
                            continue;
                        }

                        if (trans.ManagerCompanyGuid != Guid.Empty && trans.ManagerCompanyGuid != tank.ManagerGuid)
                        {
                            continue;
                        }

                        comboBox.Items.Add(new ListItem(tank.ID, tank.ID));

						if (tank.ID.Length > comboBox.TextBoxCntrl.Columns && tank.ID.Length <= this.MaxColumns)
						{
							comboBox.MaxLength = tank.ID.Length;
							comboBox.TextBoxCntrl.MaxLength = tank.ID.Length;
							comboBox.TextBoxCntrl.Columns = tank.ID.Length;
						}


						if (fieldValue != null && fieldValue.ToString ( ) == tank.ID)
						{
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString (CultureInfo.InvariantCulture);
							comboBox.Text = tank.ID;
							itemInList = true;
						}
					}

					if (!itemInList && fieldValue != null && fieldValue.ToString ( ) != string.Empty)
					{
						if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length <= this.MaxColumns)
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
				}

				if (comboBox.TextBoxCntrl.MaxLength == 0)
				{
					comboBox.MaxLength = this.MaxColumns;
					comboBox.TextBoxCntrl.MaxLength = this.MaxColumns;
					comboBox.TextBoxCntrl.Columns = this.MaxColumns;
				}
			}
		}


        public void SetTank()
        {
            object fieldValue = GetDataValue();

			if (!transContext.aliasClass.UseComboxControls)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMTankTextBox;

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
						comboBox.Clear ( );
						comboBox.MaxLength = 0;
						comboBox.Items.Add ( new ListItem ( string.Empty, string.Empty ) );

                bool itemInList = false;

                Guid? productGuid = null;

                if (this.sublineItem != null)
                {
                    productGuid = this.sublineItem.ProductGuid;
                }
                else if (this.lineItem != null)
                {
                    productGuid = this.lineItem.ProductGuid;
                }

                TankCollectionClass tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
                                                                     x =>
                                                                     x.Enumerate(transContext.security, hideHiddenTanks: true)
                                                                );

                foreach (TankClass tank in tankCollection)
                {
                    if (productGuid != null && (Guid)productGuid.Value != tank.ProductGuid)
                    {
                        continue;
                    }

                    if (this.trans.ManagerCompanyGuid != Guid.Empty && this.trans.ManagerCompanyGuid != tank.ManagerGuid)
                    {
                        continue;
                    }

                    comboBox.Items.Add(new ListItem(tank.ID, tank.ID));

							if (tank.ID.Length > comboBox.TextBoxCntrl.Columns && tank.ID.Length <= this.MaxColumns)
							{
								comboBox.MaxLength = tank.ID.Length;
								comboBox.TextBoxCntrl.MaxLength = tank.ID.Length;
								comboBox.TextBoxCntrl.Columns = tank.ID.Length;
							}

							if (fieldValue != null && fieldValue.ToString() == tank.ID)
							{
								comboBox.SelectedIndex = comboBox.Items.Count - 1;
								comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString (CultureInfo.InvariantCulture);
								comboBox.Text = tank.ID;
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
		}

		/// <summary>
		/// This method is an override method that will return the contents of the FMCompanyTextBox
		/// control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override object GetNewValue ( WebControl control )
		{
			UpdatePanel updatePanel;

			if (!transContext.aliasClass.UseComboxControls)
			{
				updatePanel = control.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMTankTextBox;

					if (textBoxButtonCombo != null && 
					    textBoxButtonCombo.TextMode == TextBoxMode.MultiLine &&
					    textBoxButtonCombo.Text.Length > this.MaxColumns)
					{
						string message = this.GetLabel ( control ) + " length must be " + this.MaxColumns + " or less.";
						this.RenderErrorMessage(message);
						throw new RetrieveException ( message );
					}

					if (textBoxButtonCombo != null)
					{
						return textBoxButtonCombo.Text;
					}
				}
			}

			updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as ComboBox;

				if (comboBox != null)
				{
					return comboBox.Text;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will calculated the real field length based on the largest
		/// field length of the entries.  The default is the MaxColumns size.
		/// </summary>
		/// <param name="tankCollection"></param>
		/// <returns></returns>
		public int RealFieldLength(TankCollectionClass tankCollection)
		{
			var localMaxColumns = MaxColumns;

			if (tankCollection == null)
			{
				return localMaxColumns;
			}

			int calculatedLength = 0;

			foreach (TankClass tank in tankCollection)
			{
				if (string.IsNullOrEmpty(tank.ID) == false)
				{
					if (tank.ID.Length > calculatedLength)
					{
						calculatedLength = tank.ID.Length;
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
		protected void TextChanged ( object sender, EventArgs e )
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

			if (!transContext.aliasClass.UseComboxControls)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMTankTextBox;

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

		/// <summary>
		/// This method will return a tank that matches the tank ID.  It will
		/// return an empty tank class if there are no matches.
		/// </summary>
		/// <param name="tankID"></param>
		/// <returns></returns>
		protected TankClass GetTankObject ( string tankID )
		{
			var tank = new TankClass ( );

			// Find the tank that matches the tank ID.
			if (!string.IsNullOrEmpty(tankID))
			{
				Guid identityGuid = FMChannelHelper.MakeCall<ITanks, Guid>(
																	 x =>
																	 x.GetIdentityGuid ( this.transContext.security, tankID )
																);

				tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
																	 x =>
																	 x.Get(this.transContext.security, identityGuid)
																);

			}

			return tank;
		}
		#endregion
	}
}

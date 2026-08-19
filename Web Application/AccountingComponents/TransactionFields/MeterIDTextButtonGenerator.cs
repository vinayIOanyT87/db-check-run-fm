///***************************************************************************
/// Module Name:  MeterIDTextButtonGenerator.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace TransactionFields
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;
	using AjaxControlToolkit;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMControls;

	/// <summary>
	/// Used to generate a text box which allows a user to select a meter on the transaction detail form.
	/// Can also be a combo box depending on the accounting settings.
	/// 
	/// Much of the code has been adapted from the other TextButtonGenerator classes.
	/// </summary>
	abstract public class MeterIDTextButtonGenerator : FieldGenerator
	{
		/// <summary>
		/// The default maximum length of the meter ID, which is used 
		/// when the field is a combo box
		/// </summary>
		public const short FIELD_LENGTH = 30;

		/// <summary>
		/// This is the default constructor for the meter text box button combo
		/// abstract class.
		/// </summary>
		public MeterIDTextButtonGenerator()
		{
		}

		/// <summary>
		/// This property is an abstract property that forces implementation of
		/// returning the maximum column size.
		/// </summary>
		abstract protected short MaxColumns
		{
			get;
		}

		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMMeterTextBox control is being generated.
		/// </summary>
		/// <param name="editable">true if the control should be editable</param>
		public override void Generate(bool editable)
		{
			bool autoPostBack = true;

			this.cell.Controls.Clear();

			if (transContext.aliasClass.UseComboxControls == false) // display as the standard meter select text box control
			{
				FMControls.FMMeterTextBox textBoxButtonCombo = new FMControls.FMMeterTextBox();
				this.cell.Controls.Add(textBoxButtonCombo);

				textBoxButtonCombo.ID = this.GetType().ToString();
				textBoxButtonCombo.MaxLength = MaxColumns;
				textBoxButtonCombo.Columns = MaxColumns;
				textBoxButtonCombo.AutoPostBack = autoPostBack;

				textBoxButtonCombo.BackColor = System.Drawing.Color.LightGray;
				textBoxButtonCombo.Enabled = editable;
				textBoxButtonCombo.ToolTip = this.DisplayName;

				object fieldValue = GetDataValue();

				if (fieldValue != null)
				{
					textBoxButtonCombo.Text = fieldValue.ToString();
				}

				textBoxButtonCombo.TextChanged += new System.EventHandler(this.TextChanged);
			}
			else // display as a combo box
			{
				FMComboBox comboBox = new FMComboBox();
				this.cell.Controls.Add(comboBox);

				comboBox.ID = this.GetType().ToString();
				comboBox.MaxLength = MaxColumns;
				comboBox.TextBoxCntrl.MaxLength = MaxColumns;
				comboBox.TextBoxCntrl.Columns = MaxColumns;
				comboBox.AutoPostBack = autoPostBack;
				comboBox.Enabled = editable;
				comboBox.CssClass = "formfield txFieldComboBox";
				comboBox.RenderMode = ComboBoxRenderMode.Block;
				comboBox.AutoCompleteMode = ComboBoxAutoCompleteMode.Suggest;
				comboBox.ToolTip = this.DisplayName;

				if (transContext.aliasClass.PermitNonReferenceData)
				{
					comboBox.DropDownStyle = ComboBoxStyle.DropDown;
				}
				else
				{
					comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				}

				comboBox.ItemInsertLocation = ComboBoxItemInsertLocation.OrdinalText;

				comboBox.TextChanged += new System.EventHandler(this.TextChanged);
				comboBox.ItemInserted += new EventHandler<ComboBoxItemInsertEventArgs>(ItemInserted);

				if (!cell.Page.IsPostBack || transContext.aliasClass.MultipleLineItems || transContext.reload)
				{
					object fieldValue = GetDataValue();

					List<MeterClass> meterCollection = null;

					comboBox.Items.Add(new ListItem(string.Empty, string.Empty));

					bool itemInList = false;
					meterCollection = FMChannelHelper.MakeCall<IMeters, List<MeterClass>>(
																	 x =>
																	 x.Enumerate(transContext.security)
																);

					foreach (MeterClass meter in meterCollection)
					{
						comboBox.Items.Add(new ListItem(meter.ID, meter.ID));

						if (meter.ID.Length > comboBox.TextBoxCntrl.Columns && meter.ID.Length < this.MaxColumns)
						{
							comboBox.MaxLength = meter.ID.Length;
							comboBox.TextBoxCntrl.MaxLength = meter.ID.Length;
							comboBox.TextBoxCntrl.Columns = meter.ID.Length;
						}

						if (fieldValue != null && fieldValue.ToString() == meter.ID)
						{
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString();
							comboBox.Text = meter.ID;
							itemInList = true;
						}
					}

					if (!itemInList && fieldValue != null && !string.IsNullOrEmpty(fieldValue.ToString()))
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
		/// When meter information is selected or loaded from the database, this method sets the control so it displays the meter information
		/// </summary>
		public void SetMeter()
		{
			object fieldValue = GetDataValue();

			if (cell.Controls.Count > 0)
			{
				if (!transContext.aliasClass.UseComboxControls)
				{
					FMMeterTextBox textBoxButtonCombo = cell.Controls[0] as FMMeterTextBox;

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
				else // display as a combo box
				{
					FMComboBox comboBox = cell.Controls[0] as FMComboBox;

					comboBox.Clear();
					comboBox.MaxLength = 0;

					//add an empty selection
					comboBox.Items.Add(new ListItem(string.Empty, string.Empty));

					bool itemInList = false;

					List<MeterClass> meterCollection = FMChannelHelper.MakeCall<IMeters, List<MeterClass>>(
																	 x =>
																	 x.Enumerate(transContext.security)
																);

					//add all of the meters in the system
					foreach (MeterClass meter in meterCollection)
					{
						comboBox.Items.Add(new ListItem(meter.ID, meter.ID));

						if (meter.ID.Length > comboBox.TextBoxCntrl.Columns && meter.ID.Length <= this.MaxColumns)
						{
							comboBox.MaxLength = meter.ID.Length;
							comboBox.TextBoxCntrl.MaxLength = meter.ID.Length;
							comboBox.TextBoxCntrl.Columns = meter.ID.Length;
						}

						//if the meter matches the one in the line item, select it
						if (fieldValue != null && fieldValue.ToString() == meter.ID)
						{
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString();
							comboBox.Text = meter.ID;
							itemInList = true;
						}
					}

					//if the meter for the line item wasn't found in the system, add it to the available meters
					if (!itemInList && fieldValue != null && !string.IsNullOrEmpty(fieldValue.ToString()))
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
		}


		/// <summary>
		/// This method is an override method that will return the contents of the FMMeterTextBox
		/// control.
		/// </summary>
		/// <param name="control">the control to get the value for</param>
		/// <returns>The meter ID as text</returns>
		public override object GetNewValue(System.Web.UI.WebControls.WebControl control)
		{
			if (control != null && control.Controls.Count > 0)
			{
				if (!transContext.aliasClass.UseComboxControls)
				{
					FMControls.FMMeterTextBox textBoxButtonCombo = control.Controls[0] as FMControls.FMMeterTextBox;

					if (textBoxButtonCombo != null)
					{
						return textBoxButtonCombo.Text;
					}
				}
				else
				{
					ComboBox comboBox = control.Controls[0] as ComboBox;

					if (comboBox != null)
					{
						return comboBox.Text;
					}
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will return a meter that matches the meter ID.
		/// </summary>
		/// <param name="meterID">the meter ID to search for matches of</param>
		/// <returns>The meter that matches meter ID, or null if none is found</returns>
		protected MeterClass GetMeterObject(string meterID)
		{
			MeterClass meter = null;

			if (!string.IsNullOrEmpty(meterID))
			{
				Guid identityGuid = FMChannelHelper.MakeCall<IMeters, Guid>(
																	 x =>
																	 x.GetIdentityGuid(base.transContext.security, meterID)
																);

				if (identityGuid != Guid.Empty)
				{
					meter = FMChannelHelper.MakeCall<IMeters, MeterClass>(
																	 x =>
																	 x.Get(base.transContext.security, identityGuid)
																);
				}
			}

			return meter;
		}

		/// <summary>
		/// When the text changes, update the line item or sub line item's data
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void TextChanged(object sender, EventArgs e)
		{
			this.lineItem = null;
			this.sublineItem = null;

			if (cell.ID.Contains("LineItem"))
			{
				if (transContext.aliasClass.MultipleLineItems)
				{
					//if we support multiple line items, find the line item that this control belongs to.
					char[] separatorList = { '.' };
					string[] stringList = cell.Parent.ID.Split(separatorList);
					int lineItemIndex = int.Parse(stringList[0]);
					int sublineItemIndex = int.Parse(stringList[1]);

					if (lineItemIndex > -1)
					{
						if ((this is ILineItemField) || (this is ISublineItemField))
						{
							this.lineItem = trans.LineItems[lineItemIndex] as LineItemDO;

							if (sublineItemIndex > -1)
							{
								this.sublineItem = lineItem.SubLineItems[sublineItemIndex] as SubLineItemDO;
							}
						}
					}
				}
				else
				{
					this.lineItem = trans.LineItems[0] as LineItemDO;
				}
			}

			if (cell != null && cell.Controls.Count > 0)
			{
				if (!transContext.aliasClass.UseComboxControls)
				{
					FMMeterTextBox textBoxButtonCombo = cell.Controls[0] as FMMeterTextBox;

					if (textBoxButtonCombo != null)
					{
						SetDataValue(textBoxButtonCombo.Text);
					}
				}
				else
				{
					ComboBox comboBox = cell.Controls[0] as ComboBox;

					if (comboBox != null)
					{
						SetDataValue(comboBox.Text);
					}
				}
			}
		}

		/// <summary>
		/// This fires when an item is added into the combo box
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void ItemInserted(object sender, ComboBoxItemInsertEventArgs e)
		{
			if (this.cell != null && this.cell.Controls.Count > 0)
			{
				FMComboBox comboBox = cell.Controls[0] as FMComboBox;

				if (comboBox != null)
				{
					comboBox.TextBoxCntrl.TextChanged += new System.EventHandler(this.TextChanged);
				}
			}
		}
	}
}

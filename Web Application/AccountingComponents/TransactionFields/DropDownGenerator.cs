// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropDownGenerator.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TransactionFields
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Drawing;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using AjaxControlToolkit;

	using FMBusinessObjects.DataObjects;

	using FMControls;

	public abstract class DropDownGenerator : FieldGenerator
	{
		#region Constants and Fields

		protected bool autoPostBack;

		protected bool m_multiple;
		private bool autoComplete;
		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DropDownGenerator"/> class. 
		///	This is the default constructor for the dropdown generator
		///	abstract class.
		/// </summary>
		public DropDownGenerator()
		{
			this.autoPostBack = false;
			this.m_multiple = false;
			this.autoComplete = false;
		}

		public DropDownGenerator(bool multiple)
		{
			this.m_multiple = multiple;
			this.autoComplete = false;
		}

		#endregion

		#region Public Properties

		/// <summary>
		///	This property will return true if the Auto Post Back flag is set,
		///	false otherwise.
		/// </summary>
		public bool AutoPostBack
		{
			get { return this.autoPostBack; }
		}

		/// <summary>
		/// This property will return true if the Auto Complete flag is set,
		/// false otherwise.
		/// </summary>
		public bool AutoComplete
		{
			get { return this.autoComplete; }
		}
		#endregion

		#region Properties


		/// <summary>
		///     This property will returned either a figured data length or the
		///     default length of 20.
		/// </summary>
		protected virtual short MaxColumns
		{
			get { return this.GetFieldLength(this.FieldID, 20); }
		}

		protected virtual string NotSetText
		{
			get { return this.Required ? selectedText : notSelectedText; }
		}

		/// <summary>
		///     This virtual property returns the selected text from the
		///     dropdown.
		/// </summary>
		protected virtual string SelectText
		{
			get { return selectedText; }
		}
		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// This method generates the control. It can be overriden by the derived class.
		///	A dropdown control is generated if there are entries in the database.  Otherwise,
		///	a text box control is generated.
		/// </summary>
		/// <param name="editable">
		/// </param>
		public override void Generate(bool editable)
		{
			HybridDictionary entries = this.GetEntries();

			if (this.transContext.aliasClass.UseComboxControls)
			{
				this.GenerateComboBoxControl(editable, entries);
			}
			else
			{
				// When there are Entries create a Select
				if (entries.Count > 0)
				{
					this.GenerateDropdownControl(editable, entries);
				}
				else
				{
					// When there are no entires make a TextBox
					this.GenerateTextboxControl(editable);
				}
			}
		}

		/// <summary>
		/// This method will return the user data field new value.
		/// </summary>
		/// <param name="control">
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		public override object GetNewValue(WebControl control)
		{
			HtmlSelect selectList = null;
			TextBox textBox = null;
			ListControl comboBox = null;
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel == null)
			{
				if (this.cell.Controls[0] is FMComboBox ||
					this.cell.Controls[0] is DropDownList)
				{
					comboBox = this.cell.Controls[0] as ListControl;
				}
			}
			else
			{
				selectList = updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;
				textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
				comboBox = updatePanel.ContentTemplateContainer.Controls[0] as ListControl;
			}

			if (selectList != null)
			{
				this.CheckIfRequired(selectList.Value);
				return selectList.Value;
			}

			if (textBox != null)
			{
				this.CheckIfRequired(textBox.Text);
				return textBox.Text;
			}

			if (comboBox != null)
			{
				this.CheckIfRequired(comboBox.Text);
				return string.IsNullOrEmpty(comboBox.SelectedValue) ? null : comboBox.SelectedValue;
			}

			return null;
		}
		#endregion

		#region Methods
		/// <summary>
		/// This abstract methods must be implemented by the derived class.
		/// </summary>
		/// <returns>
		/// The <see cref="HybridDictionary"/>.
		/// </returns>
		public abstract HybridDictionary GetEntries();

		/// <summary>
		/// This method will generate a combo cox control.
		/// </summary>
		/// <param name="editable">
		/// </param>
		/// <param name="entries">
		/// </param>
		protected virtual void GenerateComboBoxControl(bool editable, HybridDictionary entries)
		{
			var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional };
			var comboBox = new FMComboBox();
			string typeString = ID;

			if (typeString.ToUpper().EndsWith("USERDATALISTFG"))
			{
				updatePanel.ID = typeString + FieldID + "Panel";
				comboBox.ID = typeString + FieldID;
			}
			else
			{
				updatePanel.ID = typeString + "Panel";
				comboBox.ID = typeString;
			}

			updatePanel.ContentTemplateContainer.Controls.Add(comboBox);
			this.cell.Controls.Add(updatePanel);

			comboBox.MaxLength				= this.RealFieldLength(entries); 
			comboBox.TextBoxCntrl.MaxLength = 0;
			comboBox.TextBoxCntrl.Columns	= 0;
			comboBox.AutoPostBack			= this.autoPostBack;
			comboBox.Enabled				= editable;
			comboBox.CssClass				= "formfield txFieldComboBox";
			comboBox.RenderMode				= ComboBoxRenderMode.Block;
			comboBox.AutoCompleteMode		= ComboBoxAutoCompleteMode.Suggest;

			if (this.transContext.aliasClass.PermitNonReferenceData && this.AutoComplete)
			{
				comboBox.DropDownStyle = ComboBoxStyle.DropDown;
			}
			else
			{
				comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			}

			comboBox.Attributes["alt"] = this.displayName;
			comboBox.ToolTip = this.displayName;

			comboBox.ItemInsertLocation = ComboBoxItemInsertLocation.OrdinalText;
			comboBox.TextChanged += this.TextChanged;
			comboBox.ItemInserted += this.ItemInserted;

			if (this.cell.Page.IsPostBack == false 
			|| this.transContext.aliasClass.MultipleLineItems
			|| this.transContext.reload)
			{
				comboBox.Items.Add(new ListItem(string.Empty, string.Empty));

				var sortedList = new ArrayList();

				// Load entries into an array list for sorting since the
				// HybridDictionary does not sort.
				foreach (DictionaryEntry entry in entries)
				{
					var key = entry.Key as string;
					var value = entry.Value as string;

					sortedList.Add(new ListItem(key, value));
				}

				var fieldValue = GetDataValue() as string;

				// If there are no entries and there is a field value, then
				// add to the list.
				if (entries.Count == 0 && string.IsNullOrEmpty(fieldValue) == false)
				{
					sortedList.Add(new ListItem(fieldValue, fieldValue));
				}

				// Sort the items in the Array List
				var dropdownSorter = new DropdownSorter();
				sortedList.Sort(dropdownSorter);

				foreach (ListItem listItem in sortedList)
				{
					comboBox.Items.Add(listItem);

					if (!this.TransFieldConfiguration.FileFound)
					{
						if (listItem.Value.Length <= this.MaxColumns && listItem.Value.Length > comboBox.TextBoxCntrl.MaxLength)
						{
							comboBox.MaxLength = listItem.Value.Length;
							comboBox.TextBoxCntrl.MaxLength = listItem.Value.Length;
							comboBox.TextBoxCntrl.Columns = listItem.Value.Length;
						}
					}

					if ((string.IsNullOrEmpty(fieldValue) == false) && (fieldValue == listItem.Value))
					{
						comboBox.SelectedIndex			= comboBox.Items.Count - 1;
						comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
						comboBox.Text					= fieldValue;
					}
				}
			}

			if (comboBox.TextBoxCntrl.MaxLength == 0)
			{
				comboBox.MaxLength				= this.MaxColumns;
				comboBox.TextBoxCntrl.MaxLength = this.MaxColumns;
				comboBox.TextBoxCntrl.Columns	= this.MaxColumns;
			}
		}

		/// <summary>
		/// This method generates a dropdown control.
		/// </summary>
		/// <param name="editable">
		/// </param>
		/// <param name="entries">
		/// </param>
		public virtual void GenerateDropdownControl(bool editable, HybridDictionary entries)
		{
			// Set the flag to indicate that this is a dropdown field type.
			this.isDropdownField = true;

			var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional };
			var list = new HtmlSelect();

			// Since there can be many user data fields (up to 24) ensure that
			// the client HTML ID is unique.
			string typeString = ID;

			if (typeString.ToUpper().EndsWith("USERDATALISTFG"))
			{
				list.ID = typeString + FieldID;
				updatePanel.ID = typeString + FieldID + "Panel";
			}
			else
			{
				list.ID = typeString;
				updatePanel.ID = typeString + "Panel";
			}
			updatePanel.ContentTemplateContainer.Controls.Add(list);
			this.cell.Controls.Add(updatePanel);

			list.Items.Clear();
			list.Disabled = !(editable && this.Editable);

			// JS20100416 CCP-042
			list.Multiple = this.m_multiple;

			if (typeString.ToUpper().EndsWith("USERDATALISTFG"))
			{
				list.ID = typeString + this.FieldID;
			}
			else
			{
				list.ID = typeString;
			}

			var selectedValue = this.GetDataValue() as string;
			string selectedStringValue;
			ListItem listItem;

			if ((string.IsNullOrEmpty(selectedValue)) || selectedValue.Equals(string.Empty) || selectedValue.Equals("-1"))
			{
				selectedStringValue = this.NotSetText;
			}
			else
			{
				selectedStringValue = this.GetDataText();
			}

			foreach (DictionaryEntry entry in entries)
			{
				listItem = new ListItem((string)entry.Key, (string)entry.Value);

				foreach (ListItem existingItem in list.Items)
				{
					if (existingItem.Text.CompareTo(listItem.Text) > 0)
					{
						int index = list.Items.IndexOf(existingItem);
						list.Items.Insert(index, listItem);

						if (listItem.Value.Equals(selectedValue))
						{
							list.SelectedIndex = index;
							listItem.Selected = true;
						}

						listItem = null;
						break;
					}
				}

				if (listItem != null)
				{
					list.Items.Add(listItem);

					if (listItem.Value.Equals(selectedValue))
					{
						list.SelectedIndex = list.Items.Count - 1;
						listItem.Selected = true;
					}
				}
			}

			// No selection option
			listItem = new ListItem(this.NotSetText, null);
			list.Items.Insert(0, listItem);

			if (list.Items.Count > 0
			&& (list.SelectedIndex == -1
			|| list.Items[list.SelectedIndex].Value != selectedValue))
			{
				if (selectedStringValue == this.NotSetText
					|| string.IsNullOrEmpty(selectedValue)
					|| selectedValue.Equals("-1"))
				{
					list.SelectedIndex = 0;
					listItem.Selected = true;
				}
				else
				{
					listItem = new ListItem(selectedStringValue, selectedValue);
					listItem.Selected = true;

					foreach (ListItem existingItem in list.Items)
					{
						if (existingItem.Text.CompareTo(listItem.Text) > 0)
						{
							int index = list.Items.IndexOf(existingItem);
							list.Items.Insert(index, listItem);
							list.SelectedIndex = index;
							listItem = null;
							break;
						}
					}

					if (listItem != null)
					{
						list.Items.Add(listItem);
						list.SelectedIndex = list.Items.Count - 1;
					}
				}

				if (selectedValue != null)
				{
					list.Items[list.SelectedIndex].Attributes.Add("class", "formfieldWarning");
				}
			}
		}

		/// <summary>
		/// This method will generate text box control.
		/// </summary>
		/// <param name="editable">
		/// </param>
		protected virtual void GenerateTextboxControl(bool editable)
		{
			var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional };
			var textBox = new TextBox();
			string typeString = ID;

			textBox.ToolTip = this.DisplayName;

			if (typeString.ToUpper().EndsWith("USERDATALISTFG"))
			{
				updatePanel.ID = typeString + FieldID + "Panel";
				textBox.ID = typeString + FieldID;
			}
			else
			{
				updatePanel.ID = typeString + FieldID + "Panel";
				textBox.ID = typeString;
			}

			updatePanel.ContentTemplateContainer.Controls.Add(textBox);
			this.cell.Controls.Add(updatePanel);

			textBox.ReadOnly = (!base.Editable || !editable);

			if (textBox.ReadOnly)
			{
				textBox.BackColor = VarecBkgrndReadOnlyGray;
			}

			// We need a length
			textBox.MaxLength = MaxColumns;
			textBox.Columns = textBox.MaxLength;

			object fieldValue = GetDataValue();

			if (fieldValue != null)
			{
				textBox.Text = fieldValue.ToString();
			}
		}

		protected void ItemInserted(object sender, ComboBoxItemInsertEventArgs e)
		{
			UpdatePanel updatePanel = cell.Controls[0] as UpdatePanel;
			FMComboBox comboBox = null;

			if (updatePanel == null)
			{
					comboBox = this.cell.Controls[0] as FMComboBox;
			}
			else
			{
					comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;
			}

			if (comboBox != null)
			{
				comboBox.TextBoxCntrl.TextChanged += this.TextChanged;
			}
		}

		/// <summary>
		/// This method will calculated the real field length based on the largest
		/// field length of the entries.  The default is the MaxColumns size.
		/// </summary>
		/// <param name="entries"></param>
		/// <returns></returns>
		protected virtual int RealFieldLength(HybridDictionary entries)
		{
			if (entries == null)
			{
				return this.MaxColumns;
			}

			int calculatedLength = 0;

			foreach (DictionaryEntry entry in entries)
			{
				string value = entry.Value as string;

				if (string.IsNullOrEmpty(value) == false)
				{
					if (value.Length > calculatedLength)
					{
						calculatedLength = value.Length;
					}

					// Do not allow the length to be greater than
					// the maximum length by the object.
					if (calculatedLength > this.MaxColumns)
					{
						calculatedLength = this.MaxColumns;
						break;
					}
				}
			}

			if (calculatedLength == 0)
			{
				return this.MaxColumns;
			}
			else
			{
				return calculatedLength;
			}
		}

		/// <summary>
		/// This method handles the Text change for combo boxes.
		/// </summary>
		/// <param name="sender">
		/// </param>
		/// <param name="e">
		/// </param>
		protected virtual void TextChanged(object sender, EventArgs e)
		{
			this.lineItem = null;
			this.sublineItem = null;

			if (this.cell.ID.Contains("LineItem") || this.cell.ID.Contains("Line Item"))
			{
				if (this.transContext.aliasClass.MultipleLineItems)
				{
					char[] separatorList = { '.' };
					string[] stringList = this.cell.Parent.ID.Split(separatorList);
					int lineItemIndex = int.Parse(stringList[0]);
					int sublineItemIndex = int.Parse(stringList[1]);

					if (lineItemIndex > -1)
					{
						if ((this is ILineItemField) || (this is ISublineItemField))
						{
							this.lineItem = this.trans.LineItems[lineItemIndex];
							if (sublineItemIndex > -1)
							{
								this.sublineItem = this.lineItem.SubLineItems[sublineItemIndex] as SubLineItemDO;
							}
						}
					}
				}
				else
				{
					this.lineItem = this.trans.LineItems[0];
				}
			}

			var updatePanel = cell.Controls[0] as UpdatePanel;
			ListControl comboBox = null;

			if (updatePanel == null)
			{
				comboBox = this.cell.Controls[0] as ListControl;
			}
			else
			{
				comboBox = updatePanel.ContentTemplateContainer.Controls[0] as ListControl;
			}
			if ((comboBox != null && comboBox is FMComboBox && comboBox.SelectedIndex != -1 && comboBox.SelectedValue != string.Empty) ||
				(comboBox != null && comboBox is DropDownList && comboBox.SelectedIndex > 0 && comboBox.SelectedValue != string.Empty))
			{
				this.SetDataValue(comboBox.SelectedValue);
			}
			else
			{
				this.SetDataValue(null);
			}
		}

		/// <summary>
		/// This method will check to see if the field is require. If so, and the field
		///	is not populate, then an exception is thrown.
		/// </summary>
		/// <param name="inValue">
		/// </param>
		private void CheckIfRequired(string inValue)
		{
			this.cell.BackColor = Color.Red;

			if (base.Required)
			{
				if ((inValue == null) || (inValue == string.Empty) || inValue.Equals(selectedText))
				{
					throw new FMFieldRequiredException();
				}
			}

			this.cell.BackColor = Color.Transparent;
		}

		#endregion
	}

	#region Dropdown Sorter Class

	/// <summary>
	///	The purpose of this class is to sort the items for the new combo box list.
	/// </summary>
	public class DropdownSorter : IComparer
	{
		public int Compare(object objectA, object objectB)
		{
			int result = ((ListItem)objectA).Text.CompareTo(((ListItem)objectB).Text);
			return result;
		}
	}
	#endregion
}
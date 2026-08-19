// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineItemTransactionStatusFG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TransactionFields
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Drawing;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	///     Summary description for LineItemTransactionStatusFG.
	/// </summary>
	public class LineItemTransactionStatusFG : DropDownGenerator, ILineItemField, ISublineItemField
	{
		#region Public Properties
		public override bool Editable
		{
			get
			{
				return true;
			}
		}

		public override string FieldID
		{
			get
			{
				return "LineItem LookupTransactionStatusIndex";
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Override of DropDownGenerator.Generate.
		///     Required because status "Posted" should only be shown if the status is
		///     already "Posted" - the end user should never be able to manually set
		///     the status to "Posted"; "Posted" is the result of a Send to Enterprise
		///     type function
		/// </summary>
		/// <param name="editable">
		/// </param>
		public override void Generate(bool editable)
		{
			HybridDictionary entries = this.GetEntries();

			// When there are Entries create a Select
			if (entries.Count > 0)
			{
				var updatePanel = new UpdatePanel
				{
					UpdateMode = UpdatePanelUpdateMode.Conditional,
					ID = this.ID + "Panel"
				};

				var list = new HtmlSelect();
				updatePanel.ContentTemplateContainer.Controls.Add(list);
				this.cell.Controls.Add(updatePanel);

				list.Items.Clear();
				list.Disabled = !(editable && this.Editable);
				list.ID = this.ID;

				var selectedValue = this.GetDataValue() as string;
				string selectedStringValue;
				ListItem listItem;

				if (string.IsNullOrEmpty(selectedValue))
				{
					selectedStringValue = this.NotSetText;
				}
				else
				{
					selectedStringValue = this.GetDataText();
				}

				foreach (DictionaryEntry entry in entries)
				{
					// Block out "Posted" unless the status is already "Posted"
					// Invariant status is in entry.value
					if ((string)entry.Value == TransactionStatus.Posted.ToString()
						 && (selectedValue != TransactionStatus.Posted.ToString()))
					{
						continue;
					}

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

				if (list.Items.Count > 0 && (list.Items[list.SelectedIndex].Value != selectedValue))
				{
					listItem = new ListItem(selectedStringValue, selectedValue) { Selected = true };

					if (selectedStringValue == this.NotSetText)
					{
						list.Items.Insert(0, listItem);
						list.SelectedIndex = 0;
					}
					else
					{
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

			// When there are no entires make a TextBox
			else
			{
				var updatePanel = new UpdatePanel
				                  {
					                  UpdateMode = UpdatePanelUpdateMode.Conditional, 
									  ID = ID + "Panel"
				                  };

				var textBox = new TextBox
				              {
					              ID = this.ID, 
								  ReadOnly = !(this.Editable && editable)
				              };

				updatePanel.ContentTemplateContainer.Controls.Add(textBox);
				this.cell.Controls.Add(updatePanel);

				if (textBox.ReadOnly)
				{
					textBox.BackColor = this.VarecBkgrndReadOnlyGray;
				}

				// We need a length
				textBox.MaxLength = 20;
				textBox.Columns = 20;

				object fieldValue = this.GetDataValue();
				if (fieldValue != null)
				{
					textBox.Text = fieldValue.ToString();
				}
			}
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (this.transContext.useDataDictonary)
			{
				string datatext = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
														x =>
														x.Get(this.transContext.accountingSite.CurrentSiteGuid, this.GetDataValue(inLineItem).ToString())
													);
				return datatext;
			}

			if (this.GetDataValue(inLineItem) != null)
			{
				return this.GetDataValue(inLineItem).ToString();
			}
			
			return null;
		}

		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.Status.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			var newStringValue = newValue as string;

			if (string.IsNullOrEmpty(newStringValue))
			{
				newStringValue = string.Empty;
			}

			inLineItem.Status = (TransactionStatus) Enum.Parse(typeof(TransactionStatus), newStringValue);
			this.OnFieldChanged();
		}
		#endregion

		#region Explicit Interface Methods
		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			if (this.transContext.useDataDictonary)
			{
				string datatext = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
								x =>
								x.Get(this.transContext.accountingSite.CurrentSiteGuid, ((ISublineItemField) this).GetDataValue(inSublineItem).ToString())
						);
				return datatext;
			}

			if (((ISublineItemField) this).GetDataValue(inSublineItem) != null)
			{
				return ((ISublineItemField) this).GetDataValue(inSublineItem).ToString();
			}
			
			return null;
		}

		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.Status.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			var newStringValue = newValue as string;

			if (string.IsNullOrEmpty(newStringValue))
			{
				newStringValue = string.Empty;
			}

			inSublineItem.Status = (TransactionStatus) Enum.Parse(typeof(TransactionStatus), newStringValue);
			this.OnFieldChanged();
		}
		#endregion

		#region Methods

		/// <summary>
		///     Returns the statuses configured for use with the transaction alias
		/// </summary>
		/// <returns>A HybridDictionary containing the configured statuses</returns>
		public override HybridDictionary GetEntries()
		{
			// Create a new dictionary
			var newDictionary = new HybridDictionary();

			// Check to see if the datadictionary is used
			bool useDataDictionary = this.transContext.useDataDictonary;

			// vt: If no transaction statuses have been assigned then throw an error
			if (this.transContext.aliasClass.AssignedStatuses.Count == 0)
			{
				throw new ApplicationException(
					"No transaction statuses have been assigned to the transaction alias.  "
					+ "Please use the transaction alias configuration page to assign statuses " + "to the transaction alias.");
			}

			// vt: Updated to pull only statuses that are configured for the transaction alias
			// instead of all available statuses.
			foreach (int status in this.transContext.aliasClass.AssignedStatuses)
			{
				string value = Enum.GetName(typeof(TransactionStatus), status);

				if (useDataDictionary)
				{
					string value2 = this.GetDictionaryValueByKey(this.transContext.accountingSite.CurrentSiteGuid, value);

					if (newDictionary.Contains(value2) == false)
					{
						newDictionary.Add(value2, value);
					}
				}
				else
				{
					if (value != null && newDictionary.Contains(value) == false)
					{
						newDictionary.Add(value, value);
					}
				}
			}

			return newDictionary;
		}

		/// <summary>
		/// This method will get the data dictionary by using the key.
		/// </summary>
		/// <param name="guid">GUID</param>
		/// <param name="key">Key to retrieve the dictionary value.</param>
		/// <returns></returns>
		private string GetDictionaryValueByKey(Guid guid, string key)
		{
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(guid, key));
		}
		#endregion
	}
}
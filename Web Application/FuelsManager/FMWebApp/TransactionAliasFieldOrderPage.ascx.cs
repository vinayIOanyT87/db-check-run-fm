// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasFieldOrderPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionAliasFieldOrderPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	/// Partial definition of the TransactionAliasFieldOrderPage class.  Provides functionality for the
	/// Transaction Alias Field Order web page.
	/// </summary>
	public partial class TransactionAliasFieldOrderPage : TransactionAliasPageBase
	{
		/// <summary>
		/// Executes when the page is loaded.  Disables command buttons if security requirements are not satisfied.
		/// Populates the section type dropdown list with the available transaction section types.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					// CSI 5856 - disable buttons if user has no modify right.
					if (!this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
					{
						this.upButton.Enabled = false;
						this.downButton.Enabled = false;
						this.dispatchUpButton.Enabled = false;
						this.dispatchDownButton.Enabled = false;
					}

					// Populate the SectionTypeDropDownList
					const TRANSACTION_SECTION_TYPE Type = TRANSACTION_SECTION_TYPE.BODY;
					var item = new ListItem(TransactionAliasClass.TransactionSectionTypeID(Type), ((int)Type).ToString());
					this.sectionTypeDropDownList.Items.Add(item);
					this.SectionTypeDropDownListSelectedIndexChanged(null, null);

                    this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Executed when the section type dropdown list selection changes.
		/// Populates the standard field and dispatch field list boxes.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void SectionTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.IsPostBack)
			{
				this.GetTabControl().ActiveTabIndex = 4;
			}

			this.ReloadSectionTypeDropDown();
		}

		/// <summary>
		/// This method will reload the selection Type dropdown control.
		/// </summary>
		public void ReloadSectionTypeDropDown()
		{
			var selectedItems = new List<string>();
			foreach (ListItem listItem in this.fieldsListBox.Items)
			{
				if (listItem.Selected)
				{
					selectedItems.Add(listItem.Text);
				}
			}

			var selectedDispatchItems = new List<string>();
			foreach (ListItem listItem in this.dispatchFieldsListBox.Items)
			{
				if (listItem.Selected)
				{
					selectedDispatchItems.Add(listItem.Text);
				}
			}

			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
			this.sectionTypeDropDownList.Enabled = transactionAlias.MultipleLineItems 
													|| transactionAlias.MultipleWeightReadings
													|| transactionAlias.MultipleTransportLineItems;

			var type = TRANSACTION_SECTION_TYPE.LINE_ITEMS;
			string translatedText = this.GetTranslatedText(TransactionAliasClass.TransactionSectionTypeID(type));

			var item = new ListItem(translatedText, ((int)type).ToString(CultureInfo.InvariantCulture));
			int index = this.sectionTypeDropDownList.Items.IndexOf(item);

			if (transactionAlias.MultipleLineItems)
			{
				if (index == -1)
				{
					this.sectionTypeDropDownList.Items.Insert(1, item);
				}
			}
			else
			{
				if (index != -1)
				{
					this.sectionTypeDropDownList.Items.Remove(item);
				}
			}

			type = TRANSACTION_SECTION_TYPE.WEIGHT_READINGS;
			translatedText = this.GetTranslatedText(TransactionAliasClass.TransactionSectionTypeID(type));

			item = new ListItem(translatedText, ((int)type).ToString());
			index = this.sectionTypeDropDownList.Items.IndexOf(item);

			if (transactionAlias.MultipleWeightReadings)
			{
				if (index == -1)
				{
					this.sectionTypeDropDownList.Items.Insert(1, item);
				}
			}
			else
			{
				if (index != -1)
				{
					this.sectionTypeDropDownList.Items.Remove(item);
				}
			}

			type = TRANSACTION_SECTION_TYPE.TRANPORT_INFO;
			translatedText = this.GetTranslatedText(TransactionAliasClass.TransactionSectionTypeID(type));

			item = new ListItem(translatedText, ((int)type).ToString());
			index = this.sectionTypeDropDownList.Items.IndexOf(item);

			if (transactionAlias.MultipleTransportLineItems)
			{
				if (index == -1)
				{
					this.sectionTypeDropDownList.Items.Insert(1, item);
				}
			}
			else
			{
				if (index != -1)
				{
					this.sectionTypeDropDownList.Items.Remove(item);
				}
			}

			if (this.sectionTypeDropDownList.SelectedIndex > this.sectionTypeDropDownList.Items.Count - 1)
			{
				this.sectionTypeDropDownList.SelectedIndex = this.sectionTypeDropDownList.Items.Count - 1;
			}

			FieldClass[] fields =
				transactionAlias.DisplayOrder((TRANSACTION_SECTION_TYPE)Convert.ToInt32(this.sectionTypeDropDownList.SelectedValue));
			this.fieldsListBox.Items.Clear();
			index = 0;

			foreach (FieldClass field in fields)
			{
				// We do not want the Notes, Additional Information or Error fields to be displayed in the list of fields
				// to be ordered. These three fields will always be at the botton of the transaction
				// detail page.
				if (field.DbName.ToUpper().Equals("NOTES") == false
				    && field.DbName.ToUpper().Equals("ADDITIONALINFORMATION") == false
				    && field.DbName.ToUpper().Equals("ERROR") == false)
				{
					field.DisplayOrder = index;
					item = new ListItem(field.DisplayName, field.DisplayOrder.ToString());
					if (this.InList(selectedItems, item.Text))
					{
						item.Selected = true;
					}

					this.fieldsListBox.Items.Add(item);

					item = new ListItem(field.DisplayName, field.DisplayOrder.ToString());
					this.dispatchFieldsListBox.Items.Add(item);
					if (this.InList(selectedDispatchItems, item.Text))
					{
						item.Selected = true;
					}

					index++;
				}
			}

			fields =
				transactionAlias.DispatchDisplayOrder(
					(TRANSACTION_SECTION_TYPE)Convert.ToInt32(this.sectionTypeDropDownList.SelectedValue));
			this.dispatchFieldsListBox.Items.Clear();
			index = 0;

			foreach (FieldClass field in fields)
			{
				// We do not want the Notes, Additional Information or Error fields to be displayed in the list of fields
				// to be ordered. These three fields will always be at the botton of the transaction
				// detail page.
				if (field.DbName.ToUpper().Equals("NOTES") == false
				    && field.DbName.ToUpper().Equals("ADDITIONALINFORMATION") == false
				    && field.DbName.ToUpper().Equals("ERROR") == false)
				{
					field.DisplayOrder = index;
					item = new ListItem(field.DisplayName, field.DisplayOrder.ToString());
					this.dispatchFieldsListBox.Items.Add(item);
					index++;
				}
			}
		}

		private bool InList(List<string> selectedItems, string text)
		{
			foreach (var item in selectedItems)
			{
				if (item == text)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Moves the selected list of fields up one row in the appropriate field order list box.  Use either
		/// the standard fields list box or the dispatch fields list box depending on which up button is pushed.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void UpButtonOnClick(object sender, EventArgs e)
		{
			this.GetTabControl().ActiveTabIndex = 4;
			var button = sender as FMButton;

			if (button == null)
			{
				return;
			}

			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
			ListBox listBox;
			FieldClass[] fields;

			if (button.ID == "upButton")
			{
				listBox = this.fieldsListBox;
				fields = transactionAlias.DisplayOrder((TRANSACTION_SECTION_TYPE)Convert.ToInt32(this.sectionTypeDropDownList.SelectedValue));
			}
			else
			{
				listBox = this.dispatchFieldsListBox;
				fields = transactionAlias.DispatchDisplayOrder((TRANSACTION_SECTION_TYPE)Convert.ToInt32(this.sectionTypeDropDownList.SelectedValue));
			}

			int itemIndex = 0;
			int beginIndex = 0;
			int endIndex = 0;
			bool beginFound = false;
			bool endFound = false;

			while (itemIndex < listBox.Items.Count)
			{
				if (!beginFound && listBox.Items[itemIndex].Selected)
				{
					beginIndex = itemIndex;
					beginFound = true;
				}

				if (beginFound)
				{
					if (!listBox.Items[itemIndex].Selected)
					{
						endIndex = itemIndex - 1;
						endFound = true;
					}
					else if (itemIndex == listBox.Items.Count - 1)
					{
						endIndex = itemIndex;
						endFound = true;
					}
				}

				itemIndex++;

				if (beginFound && endFound)
				{
					if (beginIndex > 0)
					{
						var endItem = new ListItem(listBox.Items[beginIndex - 1].Text, listBox.Items[beginIndex - 1].Value);

						for (int index = beginIndex; index <= endIndex; index++)
						{
							fields[index].DisplayOrder--;
							listBox.Items[index - 1].Text = listBox.Items[index].Text;
							listBox.Items[index - 1].Value = listBox.Items[index].Value;
							listBox.Items[index - 1].Selected = listBox.Items[index].Selected;
						}

						fields[beginIndex - 1].DisplayOrder = endIndex;
						listBox.Items[endIndex].Text = endItem.Text;
						listBox.Items[endIndex].Value = endItem.Value;
						listBox.Items[endIndex].Selected = endItem.Selected;
					}

					beginFound = false;
					endFound = false;
				}
			}
		}

		/// <summary>
		/// Moves the selected list of fields down one row in the appropriate field order list box.  Use either
		/// the standard fields list box or the dispatch fields list box depending on which down button is pushed.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void DownButtonOnClick(object sender, EventArgs e)
		{
			this.GetTabControl().ActiveTabIndex = 4;	
			var button = sender as FMButton;

			if (button == null)
			{
				return;
			}

			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			ListBox listBox;
			FieldClass[] fields;

			if (button.ID == "downButton")
			{
				listBox = this.fieldsListBox;
				fields = transactionAlias.DisplayOrder((TRANSACTION_SECTION_TYPE)Convert.ToInt32(this.sectionTypeDropDownList.SelectedValue));
			}
			else
			{
				listBox = this.dispatchFieldsListBox;
				fields = transactionAlias.DispatchDisplayOrder((TRANSACTION_SECTION_TYPE)Convert.ToInt32(this.sectionTypeDropDownList.SelectedValue));
			}

			int itemIndex = listBox.Items.Count - 1;
			int beginIndex = 0;
			int endIndex = 0;
			bool beginFound = false;
			bool endFound = false;

			while (itemIndex >= 0)
			{
				if (!beginFound && listBox.Items[itemIndex].Selected)
				{
					beginIndex = itemIndex;
					beginFound = true;
				}

				if (beginFound)
				{
					if (!listBox.Items[itemIndex].Selected)
					{
						endIndex = itemIndex + 1;
						endFound = true;
					}
					else if (itemIndex == 0)
					{
						endIndex = itemIndex;
						endFound = true;
					}
				}

				itemIndex--;

				if (beginFound && endFound)
				{
					if (beginIndex < listBox.Items.Count - 1)
					{
						var endItem = new ListItem(listBox.Items[beginIndex + 1].Text, listBox.Items[beginIndex + 1].Value);

						for (int index = beginIndex; index >= endIndex; index--)
						{
							fields[index].DisplayOrder++;
							listBox.Items[index + 1].Text = listBox.Items[index].Text;
							listBox.Items[index + 1].Value = listBox.Items[index].Value;
							listBox.Items[index + 1].Selected = listBox.Items[index].Selected;
						}

						fields[beginIndex + 1].DisplayOrder = endIndex;
						listBox.Items[endIndex].Text = endItem.Text;
						listBox.Items[endIndex].Value = endItem.Value;
						listBox.Items[endIndex].Selected = endItem.Selected;
					}

					beginFound = false;
					endFound = false;
				}
			}
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (versionSpecificFields != null && (transactionAlias.IdentityGuid.Equals(Guid.Empty)
                                              || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid))))
            {
                return;
            }

            if (versionSpecificFields != null)
            {
                this.upButton.Enabled = (this.upButton.Enabled && versionSpecificFields.Contains("FieldOrder"));
                this.downButton.Enabled = (this.downButton.Enabled && versionSpecificFields.Contains("FieldOrder"));
                this.sectionTypeDropDownList.Enabled = (this.sectionTypeDropDownList.Enabled && versionSpecificFields.Contains("FieldOrder"));
                this.fieldsListBox.Enabled = (this.fieldsListBox.Enabled && versionSpecificFields.Contains("FieldOrder"));
                this.dispatchFieldsListBox.Enabled = (this.dispatchFieldsListBox.Enabled && versionSpecificFields.Contains("FieldOrder"));
                this.dispatchUpButton.Enabled = (this.dispatchUpButton.Enabled && versionSpecificFields.Contains("FieldOrder"));
                this.dispatchDownButton.Enabled = (this.dispatchDownButton.Enabled && versionSpecificFields.Contains("FieldOrder"));
            }
        }
	}
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionStatusFG.cs" company="Varec, Inc.">
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
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class TransactionStatusFG : DropDownGenerator, IHeaderField
	{
		#region Constants and Fields
		public const string CLIENT_SIDE_KEY_TRANS_STATUS = "CLIENT_SIDE_KEY_TRANS_STATUS";
		public const string CLIENT_SIDE_SCRIPT_TRANS_STATUS = "CLIENT_SIDE_SCRIPT_TRANS_STATUS";
		#endregion

		#region Public Properties
		public override bool Editable
		{
			get
			{
				if ((this.trans.TransTypeID == TransactionTypes.T17_Order)
					 || (this.trans.TransTypeID == TransactionTypes.T18_SupplyOrder))
				{
					// Only editable if you have MODIFY security priviledge
					return this.transContext.security.HasModifyTransactionRightByAliasName(this.trans.Alias);
				}

				return this.transContext.security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA);
			}
		}

		public override string FieldID
		{
			get
			{
				return "LookupTransactionStatusIndex";
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Override of DropDownGenerator.Generate.
		///     Required because status "Posted" & "Pending" should only be shown if the status is
		///     already "Posted" or "Pending" - the end user should never be able to manually set
		///     the status to "Posted" or "Pending"; "Posted" & "Pending" is the result of a Send to Enterprise
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
				var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };
				var list = new HtmlSelect { ID = this.ID };

				list.Items.Clear();
				list.Disabled = !(editable && this.Editable);

				updatePanel.ContentTemplateContainer.Controls.Add(list);
				this.cell.Controls.Add(updatePanel);

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

					// Block out "Pending" unless the status is already "Pending"
					// Invariant status is in entry.value
					if ((string)entry.Value == TransactionStatus.Pending.ToString()
						 && (selectedValue != TransactionStatus.Pending.ToString()))
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
			else
			{
				// When there are no entires make a TextBox
				var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };

				var textBox = new TextBox
				              {
					              ID = this.ID,
					              MaxLength = 20,
					              Columns = 20,
					              ReadOnly = !(this.Editable && editable)
				              };

				if (textBox.ReadOnly)
				{
					textBox.BackColor = VarecBkgrndReadOnlyGray;
				}

				updatePanel.ContentTemplateContainer.Controls.Add(textBox);
				this.cell.Controls.Add(updatePanel);

				object fieldValue = this.GetDataValue();

				if (fieldValue != null)
				{
					textBox.Text = fieldValue.ToString();
				}
			}
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (this.transContext.useDataDictonary)
			{

                string datatext = GetDataDictionaryValueByKey(this.transContext.accountingSite.CurrentSiteGuid, this.GetDataValue(transaction).ToString());

                return datatext;

			}

			if (this.GetDataValue(transaction) != null)
			{
				return this.GetDataValue(transaction).ToString();
			}

			return null;
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.Status.ToString();
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.Status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), newValue.ToString(), true);
			this.OnFieldChanged();
		}

		public void SetNewValue(TransactionDO transaction, TransactionStatus transactionStatusValue)
		{
			this.SetDataValue(transaction, transactionStatusValue);

			// Scheduled date may exist without a status FG.
			if (this.cell != null)
			{
				var updatePanel = this.cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var control = updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;

					string valueText = Enum.GetName(typeof(TransactionStatus), transactionStatusValue);

					if (control != null)
					{
						for (int nextItem = 0; nextItem < control.Items.Count; ++nextItem)
						{
							if (control.Items[nextItem].Text == valueText)
							{
								control.SelectedIndex = nextItem;
								break;
							}
						}
					}
				}
			}
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
					string value2 = this.GetDataDictionaryValueByKey(this.transContext.accountingSite.CurrentSiteGuid, value);

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
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x =>  x.Get(guid, key));
		}

		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control">
		/// </param>
		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var dropdownList = updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;

				if (dropdownList == null)
				{
					return;
				}

				// Register client scripts for this control if the custom client script registered is registered.
				var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

				if (!string.IsNullOrEmpty(customClientScript))
				{
					// Delay client side scripting until page pre-render event in case user clicks edit button of a
					// line item while editing another line item. Such situation causes this method to be called 
					// twice, once for for each line item. Since client side script is  allowed only once to be registered,
					// later line item's client script is ignored, which is the one we actually want.
					dropdownList.Page.Session[CLIENT_SIDE_SCRIPT_TRANS_STATUS] =
						"<script type=\"text/javascript\"><!--\n"
						+ "var oTransStatusDropdown = document.getElementById('" + dropdownList.ClientID + "');\n " + "\n//--></script>";

					dropdownList.Attributes.Add("onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
				}
			}
		}
		#endregion
	}
}
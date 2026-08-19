namespace TransactionFields
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using System.Web.UI;
	using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ServiceRequests;

    using FMControls;

    public class UserDataListFG : DropDownGenerator, IHeaderField, ILineItemField
	{
		public const string CLIENT_SIDE_SCRIPT_USERDATA_LIST = "CLIENT_SIDE_SCRIPT_USERDATA_LIST";
		public const string CLIENT_SIDE_KEY_USERDATA_LIST = "CLIENT_SIDE_KEY_USERDATA_LIST";

		protected string key;

		public UserDataListFG ( string key, string displayName )
		{
			this.key = key;
			base.displayName = displayName;
		}

		public override string FieldID
		{
			get
			{
				return key;
			}
		}

		public override HybridDictionary GetEntries()
		{
			var listEntries = new HybridDictionary();

			foreach (FieldClass fieldClass in transContext.aliasClass.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY))
			{
				if (key == fieldClass.ID)
				{
					var userField = fieldClass as UserDataFieldClass;

					if (userField != null)
					{
						listEntries = new HybridDictionary(userField.UserDataListValueCollection.Count);

						var fieldValue = this.GetDataValue() as string;
						bool fieldFound = false;

						foreach (UserDataListValueClass listValue in userField.UserDataListValueCollection)
						{
							listEntries.Add(listValue.ID, listValue.ID);

							if (!string.IsNullOrEmpty(fieldValue) && fieldValue == listValue.ID)
							{
								fieldFound = true;
							}
						}

						if (!fieldFound && !string.IsNullOrEmpty(fieldValue))
						{
							listEntries.Add(fieldValue, fieldValue);
						}
					}

                    // This is NSPA-specific. Need to refactor or otherwise
                    // correct this code behavior's location - derived UserDataListFG?
                    // this was put here because UserDataList was third-class citizen
                    // and deriving from it did not work well in the NSPA 91SP1 branch.
                    if (key == "TAUD16")
                    {
                        List<string> documentNumbers = GetNspaDutyExemptionNumbers();

                        if (documentNumbers.Count > 0)
                        {
                            listEntries.Clear();
                        }

                        foreach (string documentNumber in documentNumbers)
                        {
                            listEntries.Add(documentNumber, documentNumber);
                        }
                    }

                    return listEntries;
				}
			}
			return listEntries;
		}

        private List<string> GetNspaDutyExemptionNumbers()
        {
            List<string> documentNumbers = new List<string>();
            string siteGroupSiteId = "NSPA";
            Guid siteGroupSiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(transContext.security, siteGroupSiteId));

            string query =
                string.Format(
                    "SELECT Site, DocumentNumber, Date02 FROM tblTransactions "
                    + "WHERE AliasName = 'Duty Exemption' AND Site IN ('CHAMAN', 'NCS-NDC1', 'NCS-WDC1', 'TORK') "
                    + "ORDER BY DocumentNumber DESC");

            GetTransactionSR getTransactionsSr = new GetTransactionSR
            {
                Security = transContext.security,
                Request = GetTransactionRequest.CUSTOM_INTERFACE_QUERY,
                CustomQuery = query,
                CurrentSiteGuid = transContext.security.SiteGuid
            };

            GetTransactionDO getTransactionDo = null;
            try
            {
                getTransactionDo =
                    FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
                        x => x.Process(getTransactionsSr));
            }
            catch (Exception)
            {
                return documentNumbers;
            }

            if (getTransactionDo == null)
            {
                return documentNumbers;
            }

            foreach (DataRow row in getTransactionDo.TransactionDataSet.Tables[0].Rows)
            {
                string documentNumber = row["DocumentNumber"].ToString();
                if (string.IsNullOrWhiteSpace(documentNumber))
                {
                    continue;
                }

                documentNumbers.Add(documentNumber);
            }
            return documentNumbers;
        }

        /// <summary>
        /// This method handles special ASP control functions such as client side scripting.
        /// </summary>
        /// <param name="control"></param>
        protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var selectList = updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;
				var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

			// Register client scripts for this control if the custom client script registered is registered.
			var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

				if (!string.IsNullOrEmpty(customClientScript))
				{
					if (selectList != null)
					{
						//Delay client side scripting until page pre-render event in case user clicks edit button of a
						//line item while editing another line item. Such situation causes this method to be called 
						//twice, once for for each line item. Since client side script is  allowed only once to be registered,
						//later line item's client script is ignored, which is the one we actually want.
						selectList.Page.Session[CLIENT_SIDE_SCRIPT_USERDATA_LIST] =
							"<script type=\"text/javascript\"><!--\n" +
							"var oUserDataDropdown = document.getElementById('" + selectList.ClientID + "');\n " +
							"\n//--></script>";

						selectList.Attributes.Add("onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
					}

					if (comboBox != null)
					{
						TextBox textBox = comboBox.TextBoxCntrl;

						if (textBox != null)
						{
							comboBox.Page.Session[CLIENT_SIDE_SCRIPT_USERDATA_LIST] =
								"<script type=\"text/javascript\"><!--\n" +
								"var oUserDataDropdown = document.getElementById('" + comboBox.ClientID + "');\n " +
								"\n//--></script>";

							textBox.Attributes.Add("onBlur", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
						}
					}
				}
			}
		}

		#region IHeaderField Members
		public virtual object GetDataValue(TransactionDO transaction)
		{
			if (transaction.UserData.ContainsKey(this.key))
			{
				return transaction.UserData[key];
			}
			
			return null;
		}

		public virtual string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		public virtual void SetDataValue ( TransactionDO transaction, object newValue )
		{
			var newStringValue = newValue as string;

			if (string.IsNullOrEmpty(newStringValue))
			{
				newStringValue = string.Empty;
			}

			transaction.UserData[key] = newStringValue;

			if (cell != null)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					updatePanel.Update();
					var selectList = updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;
					var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
					var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

					if (selectList != null)
					{
						selectList.SelectedIndex = selectList.Items.IndexOf(selectList.Items.FindByText(newStringValue));
					}
					else if (textBox != null)
					{
						textBox.Text = newValue as string;
					}
					else if (comboBox != null)
					{
						if (comboBox.Items.FindByText(newStringValue) == null)
						{
							var newItem = new ListItem(newValue as string, newValue as string);

							foreach (ListItem item in comboBox.Items)
							{
								if (item.Text.CompareTo(newStringValue) > 0)
								{
									comboBox.Items.Insert(comboBox.Items.IndexOf(item) + 1, newItem);
									newItem = null;
									break;
								}
							}

							if (newItem != null)
							{
								comboBox.Items.Add(newItem);
							}
						}

						comboBox.Text = newValue as string;
					}
				}
			}

			OnFieldChanged ( );
		}
		#endregion

		#region ILineItemField Members
		public virtual object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.UserData.ContainsKey(key))
			{
				return inLineItem.UserData[key];
			}
			return null;
		}

		public virtual string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}
			
			return null;
		}

		public virtual void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			var newStringValue = newValue as string;

			if (string.IsNullOrEmpty(newStringValue))
			{
				newStringValue = string.Empty;
			}

			inLineItem.UserData[key] = newStringValue;

			if (cell != null)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					updatePanel.Update();
					var selectList = updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;
					var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;
					var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

					if (selectList != null)
					{
						selectList.SelectedIndex = selectList.Items.IndexOf(selectList.Items.FindByText(newStringValue));
					}
					else if (textBox != null)
					{
						textBox.Text = newValue as string;
					}
					else if (comboBox != null)
					{
						comboBox.Text = newValue as string;
					}
				}
			}

			OnFieldChanged();
		}
		#endregion
	}
}

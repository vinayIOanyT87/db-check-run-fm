namespace TransactionFields
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;    

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    using FMControls;
	/// <summary>
	/// Summary description for LineItemArmNumberFG.
	/// </summary>
	public class LineItemArmNumberFG : DropDownGenerator, ILineItemField, ISublineItemField
	{
		public LineItemArmNumberFG()
		{
            this.autoPostBack = true;
        }

        #region Public data members
        public const string ClientSideScriptLineitemLoadinglocationFG = "CLIENT_SIDE_SCRIPT_LINEITEM_ARMNUMBER_FG";
        public const string ClientSideKeyLineitemProductFG = "CLIENT_SIDE_KEY_LINEITEM_ARMNUMBER_FG";
        #endregion
        #region Override properties
        /// <summary>
        /// This property return true if the editable.
        /// </summary>
        override public bool Editable => true;

        /// <summary>
        /// The NotSetText for this control is overridden to display an empty string when no arm is selected, instead of "None"
        /// </summary>
        protected override string NotSetText => this.Required ? selectedText : "";

        /// <summary>
        /// This property will return the field ID for the Arm Number object.
        /// </summary>
        public override string FieldID => "LineItem ArmNumber";

        /// <summary>
        /// This property will returned either a figured data length or the 
        /// default length of 2.
        /// </summary>
        protected override short MaxColumns => this.GetFieldLength(this.FieldID, 2);

        #endregion        

        /// <summary>
        /// This method generates the control. It can be overriden by the derived class.
        ///     A dropdown control is generated if there are entries in the database.  Otherwise,
        ///     a text box control is generated.
        /// </summary>
        /// <param name="editable">
        /// </param>
        public override void Generate(bool editable)
        {
            HybridDictionary entries = this.GetEntries();

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

        public override void GenerateDropdownControl(bool editable, HybridDictionary entries)
        {
            // Set the flag to indicate that this is a dropdown field type.
            this.isDropdownField = true;

            var list = new DropDownList();
            this.cell.Controls.Add(list);
            list.Items.Clear();
            list.Enabled = (editable && this.Editable);
            list.TextChanged += new EventHandler(this.TextChanged);
            list.AutoPostBack = true;

            // Since there can be many user data fields (up to 24) ensure that
            // the client HTML ID is unique.
            list.ID = this.ID;

            var selectedValue = this.GetDataValue() as string;
            string selectedStringValue;
            ListItem listItem;

            if (string.IsNullOrEmpty(selectedValue) || selectedValue.Equals("-1"))
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
                    if (string.Compare(existingItem.Text, listItem.Text, StringComparison.Ordinal) > 0)
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
                    listItem = new ListItem(selectedStringValue, selectedValue) { Selected = true };

                    foreach (ListItem existingItem in list.Items)
                    {
                        if (string.Compare(existingItem.Text, listItem.Text, StringComparison.Ordinal) > 0)
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
        /// This method handles special ASP control functions such as client side scripting.
        /// </summary>
        /// <param name="control"></param>       
        protected override void SpecializeControl(WebControl control)
        {
            base.SpecializeControl(control);
            HtmlSelect comboBox = control.Controls[0] as HtmlSelect;
            TextBox textBox = control.Controls[0] as TextBox;
            string clientID;

            if (comboBox != null)
            {
                clientID = comboBox.ClientID;
            }
            else if (textBox != null)
            {
                clientID = textBox.ClientID;
            }
            else
            {
                return;
            }

            // Register client scripts for this control if the custom client script registered is registered.
            string customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

            if (!string.IsNullOrEmpty(customClientScript))
            {
                //Delay client side scripting until page pre-render event in case user clicks edit button of a
                //line item while editing another line item. Such situation causes this method to be called 
                //twice, once for for each line item. Since client side script is  allowed only once to be registered,
                //later line item's client script is ignored, which is the one we actually want.
                if (comboBox != null)
                {
                    comboBox.Page.Session[LineItemProductFG.CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCT_FG] =
                        "<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
                        "var oLineItemArmFGComboBox  = document.getElementById('" + clientID + "'); " +
                        "\n//--></script>";
                }

                textBox?.Attributes.Add("onBlur", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}");
            }
        }

        #region ILineItemField Members
        /// <summary>
        /// This method is used to retrieve the Arm Number from the Line Item
        /// DO. It is used for the grid mode.
        /// </summary>
        /// <param name="lineItemParam"></param>
        /// <returns></returns>
        virtual public object GetDataValue(LineItemDO lineItemParam)
        {
            return lineItemParam?.ArmNumber?.ToString("G", this.transContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
        }

        /// <summary>
        /// This method is used to retrieve the Arm Number from the Line Item
        /// DO. It is used for the grid mode.
        /// </summary>
        /// <param name="lineItemParam"></param>
        /// <returns></returns>
        virtual public string GetDataText(LineItemDO lineItemParam)
        {
            string armNumber = "";

            if (lineItemParam?.ArmNumber != null)
            {
                armNumber = lineItemParam.ArmNumber.Value.ToString("G", this.transContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
            }

            return armNumber;
        }

        /// <summary>
        /// This method will set the product information in the line item data object.
        /// </summary>
        /// <param name="lineItemParam"></param>
        /// <param name="newValue"></param>
        virtual public void SetDataValue(LineItemDO lineItemParam, object newValue)
        {
            if (newValue == null)
            {
                lineItemParam.ArmNumber = null;
            }
            else
            {
                var s = newValue as string;
                if (s != null)
                {
                    string armNumberString = s;
                    int armNumber;

                    if (int.TryParse(armNumberString, NumberStyles.Integer, this.transContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT), out armNumber))
                    {
                        lineItemParam.ArmNumber = armNumber;
                    }
                    else
                    {
                        lineItemParam.ArmNumber = null;
                    }
                }
                else
                {
                    lineItemParam.ArmNumber = (int)newValue;
                }
            }
            this.OnFieldChanged();
        }
        #endregion

        #region ISublineItemField Members
        object ISublineItemField.GetDataValue(
            SubLineItemDO sublineItemParam)
        {
            return sublineItemParam?.ArmNumber?.ToString("G", this.transContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
        }

        string ISublineItemField.GetDataText(SubLineItemDO sublineItemParam)
        {
            string armNumber = string.Empty;

            if (sublineItemParam?.ArmNumber != null)
            {
                armNumber = sublineItemParam.ArmNumber.Value.ToString("G", this.transContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
            }

            return armNumber;
        }

        void ISublineItemField.SetDataValue(
            SubLineItemDO sublineItemParam, object newValue)
        {
            if (newValue == null)
            {
                sublineItemParam.ArmNumber = null;
            }
            else
            {
                var s = newValue as string;
                if (s != null)
                {
                    string armNumberString = s;
                    int armNumber;

                    if (int.TryParse(armNumberString, NumberStyles.Integer, this.transContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT), out armNumber))
                    {
                        sublineItemParam.ArmNumber = armNumber;
                    }
                    else
                    {
                        sublineItemParam.ArmNumber = null;
                    }
                }
                else
                {
                    sublineItemParam.ArmNumber = (int)newValue;
                }
            }
            this.OnFieldChanged();
        }
        #endregion

        public override HybridDictionary GetEntries()
        {
            HybridDictionary listEntries = new HybridDictionary();

            if (!this.lineItem.LoadingLocationStationGuid.IsEmpty())
            {
                StationClass station = FMChannelHelper.MakeCall<IStations, StationClass>(x => x.Get(this.transContext.security, this.lineItem.LoadingLocationStationGuid));

                if (station?.LoadArmCollection != null && station.LoadArmCollection.Count > 0)
                {
                    foreach (LoadArmClass loadArm in station.LoadArmCollection)
                    {
                        if (station.SwingArmPosition == "B")
                        {
                            listEntries.Add(loadArm.BayBArmNumber.ToString(this.transContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT)),
                                loadArm.BayBArmNumber.ToString(this.transContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT)));
                        }
                        else
                        {
                            listEntries.Add(loadArm.BayAArmNumber.ToString(this.transContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT)),
                                loadArm.BayAArmNumber.ToString(this.transContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT)));
                        }
                    }
                }
            }
            return listEntries;
        }

        internal void Clear()
        {
            this.cell.Controls.Clear();
        }
    }
}

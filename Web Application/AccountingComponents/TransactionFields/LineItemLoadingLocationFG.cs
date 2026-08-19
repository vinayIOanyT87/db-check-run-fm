using System.Collections.Specialized;
using System.Web.UI.WebControls;

using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
    using System;
    using System.Collections;
    using System.Web.UI.HtmlControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;

    /// <summary>
	/// Summary description for LineItemLoadingLocationFG.
	/// </summary>
	public class LineItemLoadingLocationFG : DropDownGenerator, ILineItemField
	{
        public const string ClientSideScriptLineitemLoadinglocationFG = "CLIENT_SIDE_SCRIPT_LINEITEM_LOADINGLOCATION_FG";
        public const string ClientSideKeyLineitemLoadinglocationFG = "CLIENT_SIDE_KEY_LINEITEM_LOADINGLOCATION_FG";

        private const string ErrMsg001 = "Must select a Loading Location.";

        public LineItemLoadingLocationFG()
		{
            this.autoPostBack = true;
        }

        /// <summary>
        /// This property return true if the editable.
        /// </summary>
        public override bool Editable => true;

	    public override string FieldID => "LineItem LoadingLocationID";

        /// <summary>
        /// This property will return true if the field is required.
        /// </summary>
        public override bool Required => true;

	    /// <summary>
        /// This property will returned either a figured data length or the 
        /// default length of 25.
        /// </summary>
        protected override short MaxColumns => this.GetFieldLength(this.FieldID, 25);

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

        /// <summary>
        /// This method generates a dropdown control.
        /// </summary>
        /// <param name="editable">
        /// </param>
        /// <param name="entries">
        /// </param>
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
        /// This method will return the station ID for the selected station
        /// object.
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        protected string GetDataText(StationClass station)
        {
            string stationID = string.Empty;

            if (station != null)
            {
                stationID = station.ID;
            }

            return stationID;
        }

        /// <summary>
        /// This method handles special ASP control functions such as client side scripting.
        /// </summary>
        /// <param name="control"></param>
        protected override void SpecializeControl(WebControl control)
        {
            base.SpecializeControl(control);
            var comboBox = control.Controls[0] as DropDownList;

            if (comboBox == null)
            {
                return;
            }

            string clientID = comboBox.ClientID;

            // Register client scripts for this control if the custom client script registered is registered.
            var customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

            if (!string.IsNullOrEmpty(customClientScript))
            {
                // Delay client side scripting until page pre-render event in case user clicks edit button of a
                // line item while editing another line item. Such situation causes this method to be called 
                // twice, once for for each line item. Since client side script is  allowed only once to be registered,
                // later line item's client script is ignored, which is the one we actually want.
                comboBox.Page.Session[ClientSideScriptLineitemLoadinglocationFG] =
                                        "<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
                                        "var oLineItemLoadingLocationIFGComboBox  = document.getElementById('" + clientID + "'); " +
                                        "\n//--></script>";
            }
        }

        #region ILineItemField Members
        /// <summary>
        /// This method is used to retrieve the product ID from the Line Item
        /// DO. It is used for the grid mode.
        /// </summary>
        /// <param name="lineItemParam"></param>
        /// <returns></returns>
        virtual public object GetDataValue(LineItemDO lineItemParam)
        {
            return lineItemParam.LoadingLocationID;
        }

        /// <summary>
        /// This method is used to retrieve the product ID from the Line Item
        /// DO. It is used for the grid mode.
        /// </summary>
        /// <param name="lineItemParam"></param>
        /// <returns></returns>
        virtual public string GetDataText(LineItemDO lineItemParam)
        {
            string loadingLocationID = string.Empty;

            if (lineItemParam != null)
            {
                loadingLocationID = lineItemParam.LoadingLocationID;
            }

            return loadingLocationID;
        }

        /// <summary>
        /// This method will set the product information in the line item data object.
        /// </summary>
        /// <param name="lineItemParam"></param>
        /// <param name="newValue"></param>
        virtual public void SetDataValue(LineItemDO lineItemParam, object newValue)
        {
            var loadingLocationID = newValue as string;

            if (string.IsNullOrEmpty(loadingLocationID))
            {
                this.RenderErrorMessage(ErrMsg001);
                return;
            }

            StationClass station = null;
            Guid stationGuid = FMChannelHelper.MakeCall<IStations, Guid>(x => x.GetIdentityGuid(this.transContext.security, loadingLocationID));
            if (stationGuid != Guid.Empty)
            {
                station = FMChannelHelper.MakeCall<IStations, StationClass>(x => x.Get(this.transContext.security, stationGuid));
            }

            if (station == null)
            {
                this.RenderErrorMessage(ErrMsg001);
            }
            else
            {
                lineItemParam.LoadingLocationID = loadingLocationID;
                lineItemParam.LoadingLocationStationGuid = stationGuid;

                var armFG = this.fieldGenerator.GetFieldGenerator("LineItem ArmNumber") as LineItemArmNumberFG;
                if (armFG == null)
                {
                }
                else
                {
                    armFG.Clear();
                    armFG.Generate(true);
                }
            }

            this.OnFieldChanged();
        }
        #endregion

        public override HybridDictionary GetEntries()
        {
            var listEntries = new HybridDictionary();

            StationCollectionClass stationCollection = FMChannelHelper.MakeCall<IStations, StationCollectionClass>(x => x.Enumerate(this.transContext.security));

            foreach (StationClass station in stationCollection)
            {
                switch (this.transContext.aliasClass.TransTypeID)
                {
                    case TransactionTypes.T5_PrimaryDisbursement:
                    case TransactionTypes.T6_SecondaryDisbursement:
                    case TransactionTypes.T7_FillStand:
                        switch (station.Type)
                        {
                            case STATION_TYPE.LOAD_RACK:
                            case STATION_TYPE.MANUAL_BOL:
                                listEntries.Add(station.ID, station.ID);
                                break;
                        }

                        break;
                    case TransactionTypes.T3_PrimaryDefuel:
                    case TransactionTypes.T4_SecondaryDefuel:
                    case TransactionTypes.T8_Receipt:
                    case TransactionTypes.T10_Unload:
                        switch (station.Type)
                        {
                            case STATION_TYPE.OFF_LOADING:
                                listEntries.Add(station.ID, station.ID);
                                break;
                        }

                        break;
                    default:
                        listEntries.Add(station.ID, station.ID);
                        break;
                }
            }

            return listEntries;
        }
    }
}


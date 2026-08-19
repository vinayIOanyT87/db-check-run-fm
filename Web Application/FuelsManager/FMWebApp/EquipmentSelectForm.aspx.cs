// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentSelectForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipmentSelectForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

    using FMCore;

    /// <summary>
    ///    Summary description for EquipmentSelectForm.
    /// </summary>
    public partial class EquipmentSelectForm : FMAutoSubmitFormBase
    {
        #region Constants and Fields

        protected EquipmentSelectContextClass EquipmentSelectContext = null;

        protected string SelectThisItemText = null;

        #endregion

        #region Methods

        protected void FindAllBtn_OnClick(object sender, EventArgs e)
        {
            this.EquipmentSelectContext.SearchString = null;
            this.FindTextBox.Text = "";
            this.UpdateView();
        }

        protected void FindBtn_OnClick(object sender, EventArgs e)
        {
            if (this.FindTextBox.Text.Length < 1)
            {
                this.EquipmentSelectContext.SearchString = null;
            }
            else
            {
                this.EquipmentSelectContext.SearchString = this.FindTextBox.Text.ToUpper();
            }

            this.UpdateView();
        }

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetSecurity();

                this.SelectThisItemText = this.GetTranslatedText("Select this item");

                if (this.Page.IsPostBack == false)
                {
                    this.EquipmentSelectContext = new EquipmentSelectContextClass();

                    if (this.Request.GetQueryOrFormValue("Type") != null)
                    {
                        this.EquipmentSelectContext.Type = (EQUIPMENT_TYPE)Enum.Parse(typeof(EQUIPMENT_TYPE), this.Request.GetQueryOrFormValue("Type"));
                    }

                    if (this.Request.GetQueryOrFormValue("Unassigned") != null)
                    {
                        this.EquipmentSelectContext.Unassigned = Convert.ToBoolean(this.Request.GetQueryOrFormValue("Unassigned"));
                    }

                    if (this.Request.GetQueryOrFormValue("Mode") != null)
                    {
                        this.EquipmentSelectContext.Mode = this.Request.GetQueryOrFormValue("Mode");
                    }

                    if (this.Request.GetQueryOrFormValue("Source") != null)
                    {
                        this.EquipmentSelectContext.RequestSource = this.Request.GetQueryOrFormValue("Source");
                    }

                    if (this.Request.GetQueryOrFormValue("EntityType") != null)
                    {
                        this.EquipmentSelectContext.EntityType = this.Request.GetQueryOrFormValue("EntityType");
                    }

                    if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) || this.EquipmentSelectContext.Mode == "Unassign")
                    {
                        this.AddButton1.Enabled = false;
                        this.AddButton2.Enabled = false;
                    }

                    if (this.Request.GetQueryOrFormValue("EquipmentTextBoxID") != null)
                    {
                        this.EquipmentSelectContext.EquipmentTextBoxID = this.Request.GetQueryOrFormValue("EquipmentTextBoxID");

                        if ((this.EquipmentSelectContext.EquipmentTextBoxID != null)
                            && this.EquipmentSelectContext.EquipmentTextBoxID.ToUpper().Contains("LINEITEM"))
                        {
                            this.EquipmentSelectContext.IsLineItem = true;
                        }
                    }

                    if (this.Request.GetQueryOrFormValue("IDCarrierLink") != null)
                    {
                        this.EquipmentSelectContext.IDCarrierLink = this.Request.GetQueryOrFormValue("IDCarrierLink");
                    }

                    if (this.Request.GetQueryOrFormValue("SearchString") != null)
                    {
                        this.EquipmentSelectContext.SearchString = this.Request.GetQueryOrFormValue("SearchString");
                        this.FindTextBox.Text = this.EquipmentSelectContext.SearchString;
                    }

                    if (this.Request.GetQueryOrFormValue("HideHidden") != null)
                    {
                        // Only honor the HideHidden value if we aren't in unassignment mode.
                        // This is to handle a scenario when a record has been assigned, then subsequently made hidden.
                        // If we didn't ignore the HideHidden value when unassigning then the record would not be unassignable without 
                        // unhiding it.
                        this.EquipmentSelectContext.HideHidden = Convert.ToBoolean(this.Request.GetQueryOrFormValue("HideHidden")) && this.EquipmentSelectContext.Mode != "Unassign";
                    }

                    this.Session["EquipmentSelectContext"] = this.EquipmentSelectContext;

                    this.UpdateView();
                }
                else
                {
                    this.EquipmentSelectContext = this.Session["EquipmentSelectContext"] as EquipmentSelectContextClass;
                }

                if (this.EquipmentSelectContext.Mode != null)
                {
                    var Form1 = (HtmlForm)this.FindControl("Form1");
                    var OkButton = new HtmlInputButton();
                    OkButton.Attributes.Add("value", this.GetTranslatedText("OK"));
                    OkButton.Attributes.Add("id", "OkButton");
                    OkButton.Attributes.Add("class", "formfieldtitle");
                    OkButton.Attributes.Add("onclick", "MultipleSelect()");
                    OkButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 662px; POSITION: absolute; TOP: 8px");
                    Form1.Controls.Add(OkButton);

                    var CancelButton = new HtmlInputButton();
                    CancelButton.Attributes.Add("value", this.GetTranslatedText("Cancel"));
                    CancelButton.Attributes.Add("id", "CancelButton");
                    CancelButton.Attributes.Add("class", "formfieldtitle");
                    CancelButton.Attributes.Add("onclick", "NoSelect()");
                    CancelButton.Attributes.Add("style", "width:66px;Z-INDEX: 107; LEFT: 758px; POSITION: absolute; TOP: 8px");
                    Form1.Controls.Add(CancelButton);
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private void AddButton_Command(object sender, CommandEventArgs e)
        {
            var EquipmentArrayList = this.Session["EquipmentArrayList"] as ArrayList;
            if (EquipmentArrayList == null)
            {
                EquipmentArrayList = new ArrayList();
                this.Session["EquipmentArrayList"] = EquipmentArrayList;
            }

            SiteClass Site = FMChannelHelper.MakeCall<ISites, SiteClass>(
                        x =>
                        x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false)
                );

            var Equipment = new EquipmentClass(Site);
            EquipmentArrayList.Add(Equipment);

            if (this.Session["EquipmentSelectContextArrayList"] == null)
            {
                var EquipmentSelectContextArrayList = new ArrayList();
                EquipmentSelectContextArrayList.Add(this.Session["EquipmentSelectContext"]);
                this.Session["EquipmentSelectContextArrayList"] = EquipmentSelectContextArrayList;
            }
            else
            {
                (this.Session["EquipmentSelectContextArrayList"] as ArrayList).Add(this.Session["EquipmentSelectContext"]);
            }

            this.Session.Remove("EquipmentFormTabIndex");
            this.Redirect("EquipmentForm.aspx?Modal=true");
        }

        private void AddEquipmentToList(
            EquipmentCollectionClass EquipmentCollection, EQUIPMENT_TYPE[] Types, EquipmentCollectionClass EnumeratedEquipment)
        {
            foreach (EquipmentClass Equipment in EnumeratedEquipment)
            {
                foreach (EQUIPMENT_TYPE EquipmentType in Types)
                {
                    if (Equipment.Type == EquipmentType)
                    {
                        if (this.FindTextBox.Text != "")
                        {
                            if (Equipment.ID.ToUpper().IndexOf(this.FindTextBox.Text.ToUpper()) != -1
                                || Equipment.Description.ToUpper().IndexOf(this.FindTextBox.Text.ToUpper()) != -1)
                            {
                                EquipmentCollection.Add(Equipment);
                            }
                        }
                        else
                        {
                            EquipmentCollection.Add(Equipment);
                        }

                        break;
                    }
                }
            }
        }

        private bool CheckIfEquipmentInCollection(EquipmentCollectionClass EquipmentCollection, EquipmentClass Equipment)
        {
            foreach (EquipmentClass checkEquipment in EquipmentCollection)
            {
                if (checkEquipment.ID == Equipment.ID)
                {
                    return true;
                }
            }

            return false;
        }

        private void EquipmentDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                // Get identityGuid
                TableCell identityGuidCell = e.Item.Cells[3];//bds

                FMChannelHelper.MakeCall<IEquipments>(
                                                                     x =>
                                                                     x.Purge(this.Security, Guid.Parse(identityGuidCell.Text))
                                                                );
                this.UpdateView();
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private void EquipmentDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                TableCell identityGuidCell = e.Item.Cells[3];//bds

                var EquipmentArrayList = this.Session["EquipmentArrayList"] as ArrayList;
                if (EquipmentArrayList == null)
                {
                    EquipmentArrayList = new ArrayList();
                    this.Session["EquipmentArrayList"] = EquipmentArrayList;
                }

                // Get Equipment
                EquipmentClass Equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
                                                                     x =>
                                                                     x.Get(this.Security, Guid.Parse(identityGuidCell.Text))
                                                                );

                EquipmentArrayList.Add(Equipment);

                if (this.Session["EquipmentSelectContextArrayList"] == null)
                {
                    var EquipmentSelectContextArrayList = new ArrayList();
                    EquipmentSelectContextArrayList.Add(this.Session["EquipmentSelectContext"]);
                    this.Session["EquipmentSelectContextArrayList"] = EquipmentSelectContextArrayList;
                }
                else
                {
                    (this.Session["EquipmentSelectContextArrayList"] as ArrayList).Add(this.Session["EquipmentSelectContext"]);
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
                return;
            }

            this.Session.Remove("EquipmentFormTabIndex");
            this.Redirect("EquipmentForm.aspx?Modal=true");
        }

        private void EquipmentDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemIndex == -1)
            {
                if (e.Item.ItemType == ListItemType.Header)
                {
                    if (this.EquipmentSelectContext.Mode != null)
                    {
                        e.Item.Cells[0].Text = this.GetTranslatedText(this.EquipmentSelectContext.Mode);
                    }
                    else
                    {
                        e.Item.Cells[0].Text = this.GetTranslatedText("Select");
                    }

                    if (this.EquipmentDataGrid.Columns.Count > 0)
                        this.EquipmentDataGrid.Columns[0].HeaderText = e.Item.Cells[0].Text;

                }
            }

            else
            {
                string ID = "";

                // Leave hard space zero length string
                if (e.Item.Cells[4].Text != "&nbsp;")//bds
                {
                    ID = HttpUtility.HtmlDecode(e.Item.Cells[4].Text);//bds
                }

                if (this.EquipmentSelectContext.Mode != null)
                {
                    var Select = new HtmlInputCheckBox();
                    Select.ID = "Select";
                    e.Item.Cells[0].Controls.Add(Select);
                    Select.Attributes.Add("Title", HttpUtility.JavaScriptStringEncode(this.EquipmentDataGrid.Columns[0].HeaderText + " " + ID));

                    e.Item.Cells[5].Text = e.Item.Cells[5].Text.Replace(" ", "&nbsp;");//bds
                }
                else
                {
                    string ToolTip = ((e.Item.Cells[6].Text != "&nbsp;") ? e.Item.Cells[6].Text : "")//bds
                                     + ((e.Item.Cells[7].Text != "&nbsp;") ? ", " + e.Item.Cells[7].Text : "")//bds
                                     + ((e.Item.Cells[8].Text != "&nbsp;") ? ", " + e.Item.Cells[8].Text : "")//bds
                                     + ((e.Item.Cells[9].Text != "&nbsp;") ? ", " + e.Item.Cells[9].Text : "");//bds

                    var Select = new HtmlAnchor();
                    Select.ID = "Select";
                    Select.HRef = HttpUtility.HtmlEncode("javascript:Select('" + HttpUtility.JavaScriptStringEncode(ID) + "','" + HttpUtility.JavaScriptStringEncode(ToolTip) + "')");
                    Image im = new Image();
                    im.ImageUrl = "../FMWebApp/Images/Select.gif";
                    im.BorderWidth = 0;
                    im.Style.Add("align", "absmiddle");
                    Select.Controls.Add(im);

                    e.Item.Cells[0].Controls.Add(Select);
                }

                Guid siteGuid = Guid.Parse(e.Item.Cells[2].Text);//bds
                Guid equipmentGuid = Guid.Parse(e.Item.Cells[3].Text);//bds
                Guid masterRecordGuid = Guid.Parse(e.Item.Cells[10].Text);//bds

                var DeleteButton = (LinkButton)e.Item.FindControl("Fmdeletelinkbutton1");
                if (DeleteButton != null)
                {
                    DeleteButton.Enabled = (this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) && this.Security.SiteGuid == siteGuid
                                            && equipmentGuid != Guid.Empty && this.EquipmentSelectContext.Mode != "Unassign" && (equipmentGuid == masterRecordGuid))
                                               ? true
                                               : false;
                }

                var EditButton = (LinkButton)e.Item.FindControl("Fmeditlinkbutton1");
                if (EditButton != null)
                {
                    EditButton.Enabled = (this.EquipmentSelectContext.Mode != "Unassign" && equipmentGuid != Guid.Empty) ? true : false;
                }
            }
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AddButton1.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
            this.EquipmentDataGrid.ItemDataBound +=
                new System.Web.UI.WebControls.DataGridItemEventHandler(this.EquipmentDataGrid_ItemDataBound);
            this.EquipmentDataGrid.EditCommand +=
                new System.Web.UI.WebControls.DataGridCommandEventHandler(this.EquipmentDataGrid_EditCommand);
            this.EquipmentDataGrid.DeleteCommand +=
                new System.Web.UI.WebControls.DataGridCommandEventHandler(this.EquipmentDataGrid_DeleteCommand);
            this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
        }

        private void UpdateView()
        {
            int limit = -1;

            var limits = new EnumerationLimits();
            limit = limits.GetLimit(EnumerationLimits.EnumerationOptions.EQUIPMENT);

            this.FindTextBox.Text = this.EquipmentSelectContext.SearchString;

            var EquipmentCollection = new EquipmentCollectionClass();

            // 7-20-206 Dexter Story (Bug) #60429
            // moved this declartion outside the scope of the 'if' statement where it originally was, so I could properly return the data to the calling page
            EquipmentCollectionClass equipmentCollection = null;

            var transaction = this.Session["TransactionDetailTransaction"] as TransactionDO;

            // EquipmentTextBoxID indicates load from TransactionDetail
            string EquipmentTextBoxID = this.EquipmentSelectContext.EquipmentTextBoxID;

            if (transaction != null && EquipmentTextBoxID != null)
            {
                bool destination = (EquipmentTextBoxID.IndexOf("Destination") != -1) ? true : false;

                TransactionAliasClass transactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
                                                                     x =>
                                                                     x.Get(this.Security, transaction.TransactionAliasGuid, false)
                                                                );

                EQUIPMENT_TYPE[] types = transactionAlias.GetEquipmentTypes(
                    destination, this.EquipmentSelectContext.EquipmentNumber);

                // Line Item Equipment includes items selected from header with compartments
                if (EquipmentTextBoxID.StartsWith("LineItemDataGrid"))
                {
                    var equipmentDOs = new ArrayList();

                    if (destination)
                    {
                        if (transactionAlias.TransactionFieldCollection.Find("DestinationRegistrationID1") != null)
                        {
                            equipmentDOs.Add(transaction.DestinationEQ1);
                        }
                        if (transactionAlias.TransactionFieldCollection.Find("DestinationRegistrationID2") != null)
                        {
                            equipmentDOs.Add(transaction.DestinationEQ2);
                        }
                        if (transactionAlias.TransactionFieldCollection.Find("DestinationRegistrationID3") != null)
                        {
                            equipmentDOs.Add(transaction.DestinationEQ3);
                        }
                    }

                    else
                    {
                        if (transactionAlias.TransactionFieldCollection.Find("SourceRegistrationID1") != null)
                        {
                            equipmentDOs.Add(transaction.SourceEQ1);
                        }
                        if (transactionAlias.TransactionFieldCollection.Find("SourceRegistrationID2") != null)
                        {
                            equipmentDOs.Add(transaction.SourceEQ2);
                        }
                        if (transactionAlias.TransactionFieldCollection.Find("SourceRegistrationID3") != null)
                        {
                            equipmentDOs.Add(transaction.SourceEQ3);
                        }
                    }

                    if (equipmentDOs.Count > 0)
                    {
                        foreach (EquipmentDO equipmentDO in equipmentDOs)
                        {
                            if (equipmentDO.EquipmentGuid != Guid.Empty
                                && EquipmentTypeClass.HasCompartments(EquipmentTypeClass.Type(equipmentDO.EquipmentType)))
                            {
                                EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
                                                                     x =>
                                                                     x.Get(this.Security, equipmentDO.EquipmentGuid)
                                                                );

                                if (this.FindTextBox.Text != "")
                                {
                                    if (equipment.ID.ToUpper().IndexOf(this.FindTextBox.Text.ToUpper()) != -1
                                        || equipment.Description.ToUpper().IndexOf(this.FindTextBox.Text.ToUpper()) != -1)
                                    {
                                        EquipmentCollection.Add(equipment);
                                    }
                                }
                                else
                                {
                                    EquipmentCollection.Add(equipment);
                                }
                            }
                        }
                    }
                }

                // Enumerate based upon TransTypeID
                else
                {
                    if (this.EquipmentSelectContext.Unassigned)
                    {
                        var equipment = new EquipmentClass();
                        equipment.ID = HttpUtility.HtmlEncode(this.GetTranslatedText("{Unassigned}"));
                        EquipmentCollection.Add(equipment);
                    }
                    else
                    {
                        var equipment = new EquipmentClass();
                        equipment.ID = "";
                        EquipmentCollection.Add(equipment);
                    }

                    Guid productGuid = Guid.Empty;
                    Guid companyGuid = Guid.Empty;
                    Guid fuelCardGuid = Guid.Empty;
                    object secondaryStorage = null;

                    // JS20101014 WI-14935 for the following types of transactions, filter refuell tank id on site name 
                    // as company
                    if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()) && ( // refueller tank ID for sales and issues are in source equipment
                                                      (!destination
                                                       && (transaction.Alias.ToUpper().Equals("SALE (AVIATION)")
                                                           || transaction.Alias.ToUpper().EndsWith("ISSUE (AVIATION)")))
                                                      ||
                                                      // refueller tank ID for defuel, fill stand and return to bulk are in destination equipment
                                                      (destination
                                                       && (transaction.TransTypeID == TransactionTypes.T3_PrimaryDefuel
                                                           || transaction.TransTypeID == TransactionTypes.T4_SecondaryDefuel
                                                           || transaction.TransTypeID == TransactionTypes.T7_FillStand
                                                           || transaction.TransTypeID == TransactionTypes.T10_Unload // return to bulk
                                                          ))))
                    {
                        companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
                                                                     x =>
                                                                     x.GetIdentityGuid(this.Security, transaction.Site)
                                                                );
                    }
                    // Consumer Transfers enumerate based upon From or To ShipTo.  ShipTo must have carrier role and have equipment
                    // assigned.
                    else if (transaction.TransTypeID == TransactionTypes.T11_ConsumerTransfer)
                    {
                        var consumerTransferDO = transaction as ConsumerTransferDO;
                        if (destination)
                        {
                            if ((transactionAlias.TransactionFieldCollection.Find("FuelCardID") != null)
                                && (consumerTransferDO.FuelCardGuid != Guid.Empty))
                            {
                                fuelCardGuid = consumerTransferDO.FuelCardGuid;
                            }
                            else if ((transactionAlias.TransactionFieldCollection.Find("ToShipToID") != null)
                                     && (consumerTransferDO.ToShipToCompanyGuid != Guid.Empty))
                            {
                                companyGuid = consumerTransferDO.ToShipToCompanyGuid;
                            }
                        }
                        else
                        {
                            if ((transactionAlias.TransactionFieldCollection.Find("FromShipToID") != null)
                                && (consumerTransferDO.ShipToCompanyGuid != Guid.Empty))
                            {
                                companyGuid = consumerTransferDO.ShipToCompanyGuid;
                            }
                        }
                    }

                    else if (transaction.TransTypeID == TransactionTypes.T6_SecondaryDisbursement
                             || transaction.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
                             || transaction.TransTypeID == TransactionTypes.T25_Shipment)
                    {
                        if (destination)
                        {
                            if ((transactionAlias.TransactionFieldCollection.Find("FuelCardID") != null)
                                && (transaction.FuelCardGuid != Guid.Empty))
                            {
                                fuelCardGuid = transaction.FuelCardGuid;
                            }

                            else if ((transactionAlias.TransactionFieldCollection.Find("CarrierID") != null)
                                     && (transaction.CarrierCompanyGuid != Guid.Empty))
                            {
                                companyGuid = transaction.CarrierCompanyGuid;
                            }

                            else if ((transactionAlias.TransactionFieldCollection.Find("ShipToID") != null)
                                     && (transaction.ShipToCompanyGuid != Guid.Empty))
                            {
                                companyGuid = transaction.ShipToCompanyGuid;
                            }
                        }
                        else if (transaction.TransTypeID == TransactionTypes.T6_SecondaryDisbursement)
                        {
                            secondaryStorage = true;
                        }
                    }

                    else if (transaction.TransTypeID == TransactionTypes.T4_SecondaryDefuel
                             || transaction.TransTypeID == TransactionTypes.T3_PrimaryDefuel)
                    {
                        if (destination == false)
                        {
                            if ((transactionAlias.TransactionFieldCollection.Find("FuelCardID") != null)
                                && (transaction.FuelCardGuid != Guid.Empty))
                            {
                                fuelCardGuid = transaction.FuelCardGuid;
                            }
                            else if ((transactionAlias.TransactionFieldCollection.Find("CarrierID") != null)
                                     && (transaction.CarrierCompanyGuid != Guid.Empty))
                            {
                                companyGuid = transaction.CarrierCompanyGuid;
                            }
                            else if ((transactionAlias.TransactionFieldCollection.Find("ShipToID") != null)
                                     && (transaction.ShipToCompanyGuid != Guid.Empty))
                            {
                                companyGuid = transaction.ShipToCompanyGuid;
                            }
                        }
                        else if (transaction.TransTypeID == TransactionTypes.T6_SecondaryDisbursement)
                        {
                            secondaryStorage = true;
                        }
                    }

                    else if (transaction.TransTypeID == TransactionTypes.T12_InventoryNotAffected)
                    {
                        if (destination)
                        {
                            if ((transactionAlias.TransactionFieldCollection.Find("FuelCardID") != null)
                                && (transaction.FuelCardGuid != Guid.Empty))
                            {
                                fuelCardGuid = transaction.FuelCardGuid;
                            }
                            else if ((transactionAlias.TransactionFieldCollection.Find("CarrierID") != null)
                                     && (transaction.CarrierCompanyGuid != Guid.Empty))
                            {
                                companyGuid = transaction.CarrierCompanyGuid;
                            }
                            else if ((transactionAlias.TransactionFieldCollection.Find("ShipToID") != null)
                                     && (transaction.ShipToCompanyGuid != Guid.Empty))
                            {
                                companyGuid = transaction.ShipToCompanyGuid;
                            }
                        }
                    }

                    else if (transaction.TransTypeID == TransactionTypes.T8_Receipt)
                    {
                        if (destination == false)
                        {
                            if ((transactionAlias.TransactionFieldCollection.Find("CarrierID") != null)
                                && (transaction.CarrierCompanyGuid != Guid.Empty))
                            {
                                companyGuid = transaction.CarrierCompanyGuid;
                            }

                            else if ((transactionAlias.TransactionFieldCollection.Find("SupplierID") != null)
                                     && (transaction.ShipToCompanyGuid != Guid.Empty))
                            {
                                companyGuid = transaction.ShipToCompanyGuid;
                            }
                        }
                    }

                    else if ((transactionAlias.TransactionFieldCollection.Find("CarrierID") != null)
                             && (transaction.CarrierCompanyGuid != Guid.Empty))
                    {
                        companyGuid = transaction.CarrierCompanyGuid;
                    }
                    else if ((transactionAlias.TransactionFieldCollection.Find("ShipToID") != null)
                             && (transaction.ShipToCompanyGuid != Guid.Empty))
                    {
                        companyGuid = transaction.ShipToCompanyGuid;
                    }

                    if (!transactionAlias.MultipleLineItems && transaction.LineItems.Count == 1
                        && (transaction.LineItems[0]).ProductGuid != Guid.Empty)
                    {
                        productGuid = (transaction.LineItems[0]).ProductGuid;
                    }

                    // removed this declaration and placed it at the beginning of the method so the scope would increase.  Needed to return data to calling page
                    //EquipmentCollectionClass equipmentCollection = null;

                    if (companyGuid != Guid.Empty)
                    {
                        equipmentCollection = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
                                                                     x =>
                                                                     x.EnumerateByCompany(this.Security, companyGuid, hideHiddenEquipmentRecords: this.EquipmentSelectContext.HideHidden)
                                                                );


                        // JS20100930 WI-16686 Enumerate by authorised carriers as well
                        CompanyClass company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
                                                                     x =>
                                                                     x.Get(this.Security, companyGuid)
                                                                );

                        foreach (CompanyMapClass map in company.AuthorizedCarrierCollection)
                        {
                            if (map.AssignedGuid == companyGuid)
                            {
                                continue;
                            }

                            EquipmentCollectionClass col = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
                                                                     x =>
                                                                     x.EnumerateByCompany(this.Security, map.AssignedGuid, hideHiddenEquipmentRecords: this.EquipmentSelectContext.HideHidden)
                                                                );

                            foreach (EquipmentClass equipment in col)
                            {
                                equipmentCollection.Add(equipment);
                            }
                        }
                    }
                    else if (fuelCardGuid != Guid.Empty)
                    {
                        equipmentCollection = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
                                                                     x =>
                                                                     x.EnumerateByFuelCard(this.Security, fuelCardGuid, hideHiddenEquipmentRecords: this.EquipmentSelectContext.HideHidden)
                                                                );

                    }
                    else
                    {
                        equipmentCollection = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
                                                                     x =>
                                                                     x.Enumerate(this.Security, hideHiddenEquipmentRecords: this.EquipmentSelectContext.HideHidden)
                                                                );

                    }

                    foreach (EquipmentClass equipment in equipmentCollection)
                    {
                        if (productGuid != Guid.Empty && equipment.ProductGuid != Guid.Empty && productGuid != equipment.ProductGuid)
                        {
                            continue;
                        }

                        if (secondaryStorage != null && (bool)secondaryStorage != equipment.SecondaryStorageFlag)
                        {
                            continue;
                        }

                        foreach (EQUIPMENT_TYPE type in types)
                        {
                            if (type != equipment.Type)
                            {
                                continue;
                            }

                            if (this.FindTextBox.Text != "")
                            {
                                if (equipment.ID.ToUpper().IndexOf(this.FindTextBox.Text.ToUpper()) != -1
                                    || equipment.Description.ToUpper().IndexOf(this.FindTextBox.Text.ToUpper()) != -1)
                                {
                                    EquipmentCollection.Add(equipment);
                                }
                            }

                            else
                            {
                                EquipmentCollection.Add(equipment);
                            }

                            break;
                        }
                    }
                }
            }

            else
            {
                if (this.FindTextBox.Text != "")
                {
                    EquipmentCollection =
                        FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
                                x =>
                                x.EnumerateByTypeAndFilterAndProduct(
                                    this.Security, this.EquipmentSelectContext.Type, this.FindTextBox.Text, Guid.Empty,
                                    (this.EquipmentSelectContext.RequestSource == "CompanyEquipment"),
                                    (this.EquipmentSelectContext.RequestSource == "FuelCardEquipment"),
                                    hideHiddenEquipmentRecords: this.EquipmentSelectContext.HideHidden)
                        );

                }
                else
                {
                    EquipmentCollection =
                        FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
                            x =>
                            x.EnumerateByTypeAndProduct(
                                this.Security, this.EquipmentSelectContext.Type, Guid.Empty,
                                (this.EquipmentSelectContext.RequestSource == "CompanyEquipment"),
                                (this.EquipmentSelectContext.RequestSource == "FuelCardEquipment"),
                                hideHiddenEquipmentRecords: this.EquipmentSelectContext.HideHidden)
                        );
                }

                var assignedEquipmentCollection = new EquipmentCollectionClass();
                CompanyClass Company = null;

				if (this.EquipmentSelectContext.EntityType == CompanyClass.EntityTypeID)
				{

                    var CompanyArrayList = this.Session["CompanyArrayList"] as ArrayList;
                    if (CompanyArrayList != null)
                    {
                        Company = CompanyArrayList[CompanyArrayList.Count - 1] as CompanyClass;
                    }

                    if (Company == null)
                    {
                        throw new Exception("No Company In Session");
                    }

                    assignedEquipmentCollection = Company.EquipmentCollection;
                }

                else if (this.EquipmentSelectContext.EntityType == FuelCardClass.ENTITY_TYPE_ID)
                {
                    var fuelCardArrayList = this.Session[PageSessionKeyConstants.FUEL_CARD_ARRAY_LIST] as ArrayList;
                    if (fuelCardArrayList == null)
                    {
                        throw new Exception("FuelCardArrayList not in session");
                    }

                    var fuelCard = fuelCardArrayList[fuelCardArrayList.Count - 1] as FuelCardClass;

                    assignedEquipmentCollection = fuelCard.EquipmentCollection;
                }

                if (this.EquipmentSelectContext.Mode != null)
                {

                    if ("Assign" == this.EquipmentSelectContext.Mode)
                    {
                        var UnassignedEquipmentCollection = new EquipmentCollectionClass();

                        foreach (EquipmentClass Equipment in EquipmentCollection)
                        {
                            if (Equipment.CompanyGuid == Guid.Empty
                            && (Company == null
                            || (Equipment.CompanyRoleAssignmentConstraint == COMPANY_ROLE.MAX_COMPANY_ROLE
                            || Company.HasRole(Equipment.CompanyRoleAssignmentConstraint))))
                            {
                                UnassignedEquipmentCollection.Add(Equipment);
                            }
                        }

                        EquipmentCollection = UnassignedEquipmentCollection;

                        foreach (EquipmentClass Equipment in assignedEquipmentCollection)
                        {
                            if (Equipment.Type != this.EquipmentSelectContext.Type)
                            {
                                continue;
                            }

                            EquipmentCollection.Remove(Equipment);
                        }
                    }

                    else
                    {
                        for (int nLoop = EquipmentCollection.Count - 1; nLoop >= 0; --nLoop)
                        {
                            EquipmentClass Equipment = EquipmentCollection[nLoop];

                            if (assignedEquipmentCollection.Find(x => x.IdentityGuid == Equipment.IdentityGuid) == null)
                            {
                                EquipmentCollection.Remove(Equipment);
                            }
                        }
                    }
                }
            }

            this.EquipmentDataGrid.DataSource = EquipmentCollection;
            this.EquipmentDataGrid.DataBind();
        }

        #endregion
    }

    [Serializable]
    public class EquipmentSelectContextClass
    {
        #region Constants and Fields

        public string EntityType = null;

        public string EquipmentTextBoxID = null;

        public string IDCarrierLink = null;

        public bool IsLineItem = false;

        public string Mode = null;

        public string RequestSource = null;

        public string SearchString = null;

        public EQUIPMENT_TYPE Type = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;

        public bool Unassigned = false;

        /// <summary>
        /// If true, equipment records that are marked as hidden will not be displayed
        /// </summary>
	    public bool HideHidden = false;

        private byte equipmentNumber;


        #endregion

        #region Public Properties

        public byte EquipmentNumber
        {
            get
            {
                this.equipmentNumber = 0;

                try
                {
                    if (this.EquipmentTextBoxID != null)
                    {
                        this.equipmentNumber = byte.Parse(this.EquipmentTextBoxID.Substring(this.EquipmentTextBoxID.Length - 1, 1));
                    }
                }
                catch (Exception)
                {
                }

                return this.equipmentNumber;
            }
        }

        #endregion
    }
}
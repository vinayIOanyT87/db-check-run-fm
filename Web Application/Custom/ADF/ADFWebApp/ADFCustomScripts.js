
///////////////////////////////////////////////////////////////////////////////
// The purpose of this js function to ensure that custom business logic on the
// client side scripting is invoked after a page postback. It will call all the
// the custom functions with this file.
///////////////////////////////////////////////////////////////////////////////
function TxDetailOnload() {
    var fromTxDetailOnload = 1;

    AssetLeavingAustraliaFunctionality();
    InitializeNonDomesticPriceAndCurrencyUnit();
    InitializeAlternateQuantityRelationship();
    SetExciseAndGstBasedOnCountry(fromTxDetailOnload);
    DisableFields();
    SetActualFuelPriceField();
    SetActualGSTField();
    SetActualExciseField();
    SetActualOnCostField();
    SetActualRevenueField();
    SetActualTotalCostField();
}

///////////////////////////////////////////////////////////////////////////////
// The purpose of this js function is to handle all the OnChange events that 
// were registered for the field controls.
///////////////////////////////////////////////////////////////////////////////
function MasterOnChange(fieldName) {
    if (fieldName != null) {
        if (fieldName == "LineItem AlternativeUnits") {
            HandleAlternateQuantity();
        }
        else if (fieldName == "LineItem ProductPrice") {
            HandleProductPrice();
            SetActualFuelPriceField();
        }
        else if (fieldName == "LineItem Tax1") {
            HandleLineItemTax1();
            SetActualExciseField();
        }
        else if (fieldName == "LineItem Tax2") {
            HandleLineItemTax2();
            SetActualGSTField();
        }
        else if (fieldName == "LineItem Tax3") {
            HandleLineItemTax3();
            SetActualOnCostField();
        }
        else if (fieldName == "TAUD3") {
            var fromMasterOnChange = 0;
            SetExciseAndGstBasedOnCountry(fromMasterOnChange);
        }
        else if (fieldName == "LineItem TotalValue") {
            SetActualRevenueField();
        }
        else if (fieldName == "LineItem TotalPriceWithTax") {
            SetActualTotalCostField();
        }
    }
}

///////////////////////////////////////////////////////////////////////////////
// The purpose of this js function is to handle all the OnClick events that 
// were registered for the field controls.
///////////////////////////////////////////////////////////////////////////////
function MasterOnClick(fieldName) {
    if (fieldName != null) {
        // The line item Flag04 field is used for Asset Leaving Australia functionality
        // that manipulates several other fields on the Tx detail page. This
        // field has registered an OnClick event.
        if (fieldName == "LineItem Flag04") {
            AssetLeavingAustraliaFunctionality();
        }

        var aliasName = document.getElementById("TransactionFields.TransAliasFG");

        if (aliasName != null && aliasName.value == "Invoice") {
            // only applicable to invoices
            var readyForPayment = document.getElementById("TransactionFields.Flag05FG");
            var rejectedForPayment = document.getElementById("TransactionFields.Flag06FG");

            if (fieldName == "Flag06" && rejectedForPayment.checked) // rejected for payment
            {
                readyForPayment.checked = false;
            }
            else if (fieldName == "Flag05" && readyForPayment.checked) // ready for payment
            {
                rejectedForPayment.checked = false;
            }
        }
    }
}

///////////////////////////////////////////////////////////////////////////////
// The purpose of this function is to handle the leaving Australia functionality
// which manipulate several fields when the line item flag04 is checked.
///////////////////////////////////////////////////////////////////////////////
function AssetLeavingAustraliaFunctionality() {
    var aliasName = document.getElementById("TransactionFields.TransAliasFG");
    var actualGst = document.getElementById("TransactionFields.LineItemNumber03FG");
    var actualExcise = document.getElementById("TransactionFields.LineItemNumber02FG");
    var leavingAustaliaCheckBox = null;
    var exciseTaxTextBox = null;
    var gstTaxTextBox = null;

    if ((aliasName != null)
       && ((aliasName.value == "Sale (Aviation)")
          || (aliasName.value == "Sale (Ground)")
          || (aliasName.value == "Sale (Marine)")
          || (aliasName.value == "Issue (Aviation)")
          || (aliasName.value == "Issue (Ground)")
          || (aliasName.value == "Issue (Marine)"))) {
        // Retrieve all the control objects. These object names were created by
        // the individual transaction field controls. The object names must match
        // the C# code.
        try {
            //leavingAustaliaCheckBox = oLineItemFlag04CheckBox;
            leavingAustaliaCheckBox = document.getElementById("TransactionFields.LineItemFlag04FG");
        }
        catch (err) {
        }

        try {
            exciseTaxTextBox = oLineItemTax1TextBox;
        }
        catch (err) {
        }

        try {
            //gstTaxTextBox = oLineItemTax2TextBox;
            gstTaxTextBox = document.getElementById("TransactionFields.LineItemTax2FG");
        }
        catch (err) {
        }

        // The following business logic is per the ADF SDD.
        if ((leavingAustaliaCheckBox != null) && (leavingAustaliaCheckBox.checked == true)) {
            if (gstTaxTextBox != null) {
                gstTaxTextBox.value = "";
                gstTaxTextBox.readOnly = true;
                gstTaxTextBox.style.background = "LightGrey";
            }

            if (actualGst != null) {
                actualGst.value = "";
                actualGst.readOnly = true;
                actualGst.style.background = "LightGrey";
            }
        }
        else {
            if (exciseTaxTextBox != null) {
                exciseTaxTextBox.readOnly = false;
                exciseTaxTextBox.style.background = "White";
            }

            if (gstTaxTextBox != null) {
                gstTaxTextBox.readOnly = false;
                gstTaxTextBox.style.background = "White";
            }

            if (actualExcise != null) {
                actualExcise.readOnly = false;
                actualExcise.style.background = "White";
            }

            if (actualGst != null) {
                actualGst.readOnly = false;
                actualGst.style.background = "White";
            }
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////
// The purpose of this function is to default the actual fuel price field with the fuel price
// field only if the actual fuel price field is empty.  The settings are also based on transaction
// aliases.
/////////////////////////////////////////////////////////////////////////////////////////////////////
function SetActualFuelPriceField() {
    var aliasName = document.getElementById("TransactionFields.TransAliasFG");
    var fuelPrice = document.getElementById("TransactionFields.LineItemProductPriceFG");
    var actualFuelPrice = document.getElementById("TransactionFields.LineItemNumber01FG");

    if ((aliasName != null) && (fuelPrice != null) && (actualFuelPrice != null)) {
        if ((aliasName.value == "Sale (Marine)")
           || (aliasName.value == "Sale (Ground)")
           || (aliasName.value == "Sale (Aviation)")) {
            if ((actualFuelPrice.value == null) || (actualFuelPrice.value.length <= 0)) {
                actualFuelPrice.value = fuelPrice.value;
            }
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////
// The purpose of this function is to default the actual GST field with the GST
// field only if the actual GST field is empty. The settings are also based on transaction
// aliases.
/////////////////////////////////////////////////////////////////////////////////////////////////////
function SetActualGSTField() {
    var aliasName = document.getElementById("TransactionFields.TransAliasFG");
    var gst = document.getElementById("TransactionFields.LineItemTax2FG");
    var actualGst = document.getElementById("TransactionFields.LineItemNumber03FG");

    if ((aliasName != null) && (gst != null) && (actualGst != null)) {
        if ((aliasName.value == "Sale (Marine)")
           || (aliasName.value == "Sale (Ground)")
           || (aliasName.value == "Sale (Aviation)")) {
            if ((actualGst.value == null) || (actualGst.value.length <= 0)) {
                actualGst.value = gst.value;
            }
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////
// The purpose of this function is to default the actual excise field with the excise
// field only if the actual excise field is empty. The settings are also based on transaction
// aliases.
/////////////////////////////////////////////////////////////////////////////////////////////////////
function SetActualExciseField() {
    var aliasName = document.getElementById("TransactionFields.TransAliasFG");
    var excise = document.getElementById("TransactionFields.LineItemTax1FG");
    var actualExcise = document.getElementById("TransactionFields.LineItemNumber02FG");

    if ((aliasName != null) && (excise != null) && (actualExcise != null)) {
        if ((aliasName.value == "Sale (Marine)")
           || (aliasName.value == "Sale (Ground)")
           || (aliasName.value == "Sale (Aviation)")) {
            if ((actualExcise.value == null) || (actualExcise.value.length <= 0)) {
                actualExcise.value = excise.value;
            }
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////
// The purpose of this function is to default the actual on-cost field with the on-cost
// field only if the actual on-cost field is empty. The settings are also based on transaction
// aliases.
/////////////////////////////////////////////////////////////////////////////////////////////////////
function SetActualOnCostField() {
    var aliasName = document.getElementById("TransactionFields.TransAliasFG");
    var onCost = document.getElementById("TransactionFields.LineItemTax3FG");
    var actualOnCost = document.getElementById("TransactionFields.LineItemNumber04FG");

    if ((aliasName != null) && (onCost != null) && (actualOnCost != null)) {
        if ((aliasName.value == "Sale (Marine)")
           || (aliasName.value == "Sale (Ground)")
           || (aliasName.value == "Sale (Aviation)")) {
            if ((actualOnCost.value == null) || (actualOnCost.value.length <= 0)) {
                actualOnCost.value = onCost.value;
            }
        }
    }
}

function SetActualTotalCostField() {
    var aliasName = document.getElementById("TransactionFields.TransAliasFG");
    var totalCost = document.getElementById("TransactionFields.LineItemTotalPriceWithTaxFG");
    var actualCost = document.getElementById("TransactionFields.LineItemNumber06FG");

    if ((aliasName != null) && (totalCost != null) && (actualCost != null)) {
        if ((aliasName.value == "Sale (Marine)")
           || (aliasName.value == "Sale (Ground)")
           || (aliasName.value == "Sale (Aviation)")) {
            if (((actualCost.value == null) || (actualCost.value.length <= 0)) && totalCost.value != "0.00") {
                actualCost.value = totalCost.value;
            }
        }
    }
}

function SetActualRevenueField() {
    var aliasName = document.getElementById("TransactionFields.TransAliasFG");
    var revenue = document.getElementById("TransactionFields.LineItemTotalValueFG");
    var actualRevenue = document.getElementById("TransactionFields.LineItemNumber05FG");

    if ((aliasName != null) && (revenue != null) && (actualRevenue != null)) {
        if ((aliasName.value == "Sale (Marine)")
           || (aliasName.value == "Sale (Ground)")
           || (aliasName.value == "Sale (Aviation)")) {
            if (((actualRevenue.value == null) || (actualRevenue.value.length <= 0)) && revenue.value != "0.00") {
                actualRevenue.value = revenue.value;
            }
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////////
// The purpose of this function is to disable certain fields based on aliases.
/////////////////////////////////////////////////////////////////////////////////////////////////////
function DisableFields() {
    var aliasName = document.getElementById("TransactionFields.TransAliasFG");
    var fuelPrice = document.getElementById("TransactionFields.LineItemProductPriceFG");

    if ((aliasName != null) && (fuelPrice != null)) {
        if ((aliasName.value == "Sale (Marine)")
           || (aliasName.value == "Sale (Ground)")
           || (aliasName.value == "Sale (Aviation)")
           || (aliasName.value == "Issue (Marine)")
           || (aliasName.value == "Issue (Ground)")
           || (aliasName.value == "Issue (Aviation)")) {
            fuelPrice.readOnly = true;
            fuelPrice.style.background = "LightGrey";
        }
    }
}

function HandleProductPrice() {
    var lineItemProductPrice = null;
    var hiddenLineItemProductPrice = null;

    try {
        lineItemProductPrice = oLineItemProductPrice;
        hiddenLineItemProductPrice = oHiddenLineItemProductPrice;
    }
    catch (Error) {
    }
    if (lineItemProductPrice != null && hiddenLineItemProductPrice != null) {

        hiddenLineItemProductPrice.value = "true";
    }
}

function HandleLineItemTax1() {
    var lineItemTax1TextBox = null;
    var hiddenLineItemTax1TextBox = null;
    try {
        lineItemTax1TextBox = oLineItemTax1TextBox;
        hiddenLineItemTax1TextBox = oHiddenLineItemTax1TextBox;
    }
    catch (Error) {
    }
    if (lineItemTax1TextBox != null && hiddenLineItemTax1TextBox != null) {

        hiddenLineItemTax1TextBox.value = "true";
    }
}

function HandleLineItemTax2() {
    var lineItemTax2TextBox = null;
    var hiddenLineItemTax2TextBox = null;

    try {
        lineItemTax2TextBox = oLineItemTax2TextBox;
        hiddenLineItemTax2TextBox = oHiddenLineItemTax2TextBox;
    }
    catch (Error) {
    }
    if (lineItemTax2TextBox != null && hiddenLineItemTax2TextBox != null) {

        hiddenLineItemTax2TextBox.value = "true";
    }
}

function HandleLineItemTax3() {
    var lineItemTax3TextBox = null;
    var hiddenLineItemTax3TextBox = null;
    try {
        lineItemTax3TextBox = oLineItemTax3TextBox;
        hiddenLineItemTax3TextBox = oHiddenLineItemTax3TextBox;
    }
    catch (Error) {
    }
    if (lineItemTax3TextBox != null && hiddenLineItemTax3TextBox != null) {

        hiddenLineItemTax3TextBox.value = "true";
    }
}

/////////////////////////////////////////////////////////////////////////////
// The purpose of this function is to handle the relationship between the
// gross quantity, alternate quantity, and alternate unit fields. When the
// alternate unit is selected, then the gross quantity is readonly and the
// alternate quantity is set to editable. Otherwise the gross quantity will
// be editable and the alternate quantity readonly.
/////////////////////////////////////////////////////////////////////////////
function HandleAlternateQuantity() {
    var lineItemAlternativeUnits = null;
    var lineItemGrossQuantity = null;
    var lineItemAlternativeGrossVolume = null;

    try {
        lineItemAlternativeUnits = oLineItemAlternativeUnits;
        lineItemGrossQuantity = oLineItemGrossQuanity;
        lineItemAlternativeGrossVolume = oLineItemAlternativeGrossVolume;

        // re-retrieve the controls because originals are not relevant after apply, but are handy for the ID
        lineItemAlternativeUnits = document.getElementById(lineItemAlternativeUnits.id);
        lineItemGrossQuantity = document.getElementById(lineItemGrossQuantity.id);
        lineItemAlternativeGrossVolume = document.getElementById(lineItemAlternativeGrossVolume.id);
    }
    catch (Error) {
    }

    if ((lineItemAlternativeUnits != null)
        && (lineItemGrossQuantity != null)
        && (lineItemAlternativeGrossVolume != null)) {
        if (lineItemAlternativeUnits.selectedIndex <= 0) {
            lineItemAlternativeGrossVolume.readOnly = true;
            lineItemAlternativeGrossVolume.value = "";
            lineItemAlternativeGrossVolume.style.background = "LightGrey";

            lineItemGrossQuantity.readOnly = false;
            lineItemGrossQuantity.style.background = "White";
        }
        else {
            lineItemGrossQuantity.readOnly = true;
            lineItemGrossQuantity.value = 0.0;
            lineItemGrossQuantity.style.background = "LightGrey";

            lineItemAlternativeGrossVolume.readOnly = false;
            lineItemAlternativeGrossVolume.style.background = "White";
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////
// This function will set the initial dependency between the Alternate Units, Alternate Quantity
// and Gross Quantity fields.
/////////////////////////////////////////////////////////////////////////////////////////////////
function InitializeAlternateQuantityRelationship() {
    var lineItemAlternativeUnits = null;
    var lineItemGrossQuantity = null;
    var lineItemAlternativeGrossVolume = null;

    try {
        lineItemAlternativeUnits = oLineItemAlternativeUnits;
        lineItemGrossQuantity = oLineItemGrossQuanity;
        lineItemAlternativeGrossVolume = oLineItemAlternativeGrossVolume;
    }
    catch (Error) {
    }

    if ((lineItemAlternativeUnits != null)
         && (lineItemGrossQuantity != null)
         && (lineItemAlternativeGrossVolume != null)) {
        if (lineItemAlternativeUnits.selectedIndex == 0) {
            lineItemAlternativeGrossVolume.readOnly = true;
            lineItemAlternativeGrossVolume.value = "";
            lineItemAlternativeGrossVolume.style.background = "LightGrey";

            lineItemGrossQuantity.readOnly = false;
            lineItemGrossQuantity.style.background = "White";
        }
        else {
            lineItemAlternativeGrossVolume.readOnly = false;
            lineItemAlternativeGrossVolume.style.background = "White";

            lineItemGrossQuantity.readOnly = true;
            lineItemGrossQuantity.style.background = "LightGrey";

            if ((lineItemGrossQuantity.value == null) || (lineItemGrossQuantity.value == "")) {
                lineItemGrossQuantity.value = 0.0;
            }
        }
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////
// The purpose of this function is to enable and disable the GST and Excise fields for a Direct
// Fuel Purchase or Commercial transaction based on the Country field setting. If the country field
// is set to Australia, then enable the fields, otherwise, disable the fields.
//////////////////////////////////////////////////////////////////////////////////////////////////////
function SetExciseAndGstBasedOnCountry(source) {
    var aliasName = document.getElementById("TransactionFields.TransAliasFG");
    var fromMasterOnChange = 0;

    if ((aliasName != null) && ((aliasName.value == "Direct Fuel Purchase") || (aliasName.value == "Commercial"))) {
        var userDataDropdown = null;

        try {
            //userDataDropdown = oUserDataDropdown;
            userDataDropdown = document.getElementById("TransactionFields.UserDataListFGTAUD3");
        }
        catch (Error) {
            ;
        }

        if (userDataDropdown != null) {
            var exciseCntrl = document.getElementById("TransactionFields.LineItemTax1FG");
            var gstCntrl = document.getElementById("TransactionFields.LineItemTax2FG");

            if (gstCntrl != null) {
                gstCntrl.readOnly = true;
                gstCntrl.style.background = "LightGrey";
            }

            if (exciseCntrl != null) {
                exciseCntrl.readOnly = true;
                exciseCntrl.style.background = "LightGrey";
            }

            if (userDataDropdown.selectedIndex > 0) {
                var selectedIndex = userDataDropdown.selectedIndex;
                var countryValue = userDataDropdown.options[selectedIndex].text;

                var exciseHasRight = FieldHasRight("LineItemTax1FG");
                var gstHasRight = FieldHasRight("LineItemTax2FG");

                if (countryValue != null) {
                    var index = countryValue.indexOf("AUSTRALIA");

                    if (index >= 0) {
                        if ((gstCntrl != null) && (gstHasRight == true)) {
                            if (source == fromMasterOnChange) {
                                gstCntrl.value = "";
                            }

                            gstCntrl.readOnly = false;
                            gstCntrl.style.background = "White";
                        }

                        if ((exciseCntrl != null) && (exciseHasRight == true)) {
                            if (source == fromMasterOnChange) {
                                exciseCntrl.value = "";
                            }

                            exciseCntrl.readOnly = false;
                            exciseCntrl.style.background = "White";
                        }
                    }
                }
            }
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////////
// This function will set dependency between the currency unit field and the non-domestic price
// field. If the currency unit field has a selection other than NONE, then the non-domestic
// price file should be editable. Otherwise, it should be read only and cleared out.
/////////////////////////////////////////////////////////////////////////////////////////////////
function InitializeNonDomesticPriceAndCurrencyUnit() {
    var nonDomesticPriceTextBox = null;
    var currencyUnitSelect = null;

    try {
        nonDomesticPriceTextBox = oNonDomesticPriceTextBox;
        currencyUnitSelect = document.getElementById("TransactionFields.LineItemCurrencyUnitFG");
    }
    catch (err) {
        ;
    }

    if ((nonDomesticPriceTextBox != null) && (currencyUnitSelect != null)) {
        var totalForeignPriceCntrl = document.getElementById("TransactionFields.UserDataTextFGTALUD3");
        var totalPriceCntrl = document.getElementById("TransactionFields.UserDataTextFGTALUD2");
        var priceCntrl = document.getElementById("TransactionFields.LineItemProductPriceFG");

        if (currencyUnitSelect.selectedIndex > 0) {
            nonDomesticPriceTextBox.readOnly = false;
            nonDomesticPriceTextBox.style.background = "White";

            if (totalForeignPriceCntrl != null) {
                totalForeignPriceCntrl.readOnly = false;
                totalForeignPriceCntrl.style.background = "White";
            }

            if (totalPriceCntrl != null) {
                totalPriceCntrl.readOnly = true;
                totalPriceCntrl.style.background = "LightGrey";
            }

            if (priceCntrl != null) {
                priceCntrl.readOnly = true;
                priceCntrl.style.background = "LightGrey";
            }
        }
        else {
            nonDomesticPriceTextBox.readOnly = true;
            nonDomesticPriceTextBox.style.background = "LightGrey";

            if (totalForeignPriceCntrl != null) {
                totalForeignPriceCntrl.readOnly = true;
                totalForeignPriceCntrl.style.background = "LightGrey";
            }

            if (totalPriceCntrl != null) {
                totalPriceCntrl.readOnly = false;
                totalPriceCntrl.style.background = "White";
            }

            if (priceCntrl != null) {
                priceCntrl.readOnly = false;
                priceCntrl.style.background = "White";
            }
        }
        CurrencyChange();
    }
}

function InvoiceQuerySelect(queryTextBoxID) {
    var sFeatures = "dialogWidth: 855px; dialogHeight: 560px";
    var queryTextBox = document.getElementById(queryTextBoxID);
    var result = null;

    result = window.showModalDialog("../ADFWebApp/InvoiceQueriesForm.aspx", "", sFeatures);

    if (result != null) {
        queryTextBox.value = result[0];
        queryTextBox.title = result[1];
    }
}

function BulkPaymentInvoiceSelect(selectionTextBoxID) {
    var i;
    var sFeatures = "dialogWidth: 855px;dialogHeight: 530px;";
    var param = "";
    var result = null;

    var selectionTextBox = document.getElementById(selectionTextBoxID);

    var sectionParam = "";
    var selectedSection = document.getElementById("ddlSection").selectedIndex;
    if (selectedSection >= 0) {
        sectionParam = document.getElementById("ddlSection").options[selectedSection].text;
    }
    var supplierParam = document.getElementById("tbSupplier").value;

    var forexParam = "-1";
    var selectedForex = document.getElementById("ddlForeignCurrency").selectedIndex;
    if (selectedForex >= 0) {
        forexParam = document.getElementById("ddlForeignCurrency").options[selectedForex].value;
    }

    var param = "?Section=" + sectionParam + "&Supplier=" + supplierParam + "&Forex=" + forexParam;

    result = window.showModalDialog("../ADFWebApp/BulkPaymentInvoiceSelectForm.aspx" + param, "", sFeatures);

    selectionTextBox.value = "blah";
    selectionTextBox.onchange();

}

function AssociateSingleTx(lineItemIndex) {
    var productTextBox = document.getElementById("LineItemDataGrid$" + lineItemIndex +
                                     ".-1$TransactionFields.LineItemProductFG");
    if (productTextBox == null) {
        productTextBox = document.getElementById("LineItemDataGrid_" + lineItemIndex + ".-1_TransactionFields.LineItemProductFG_TextBox");
    }
    if (productTextBox == null) {
        productTextBox = document.getElementById("TransactionFields.LineItemProductFG");
    }

    var deliveryLocationTextBox = document.getElementById("LineItemDataGrid$" + lineItemIndex +
                                     ".-1$TransactionFields.LineItemDeliveryLocationFG");
    if (deliveryLocationTextBox == null) {
        deliveryLocationTextBox = document.getElementById("LineItemDataGrid_" + lineItemIndex + ".-1_TransactionFields.LineItemDeliveryLocationFG_TextBox");
    }
    if (deliveryLocationTextBox == null) {
        deliveryLocationTextBox = document.getElementById("TransactionFields.LineItemDeliveryLocationFG");
    }

    var deliveryLocationValue = "";

    if (deliveryLocationTextBox != null) {
        deliveryLocationValue = encodeURIComponent(deliveryLocationTextBox.value);
    }

    if (productTextBox == null) {
        alert("Product Textbox not found.");
    }
    else if (productTextBox.value == "") {
        alert("Please select a Fuel Type.");
    }
    else {
        var sFeatures = "dialogWidth: 855px;dialogHeight: 530px;status: yes;";
        var result = null;
        result = window.showModalDialog("../ADFWebApp/SingleSelectAssociateTxDialog.aspx?lineItemID=" + lineItemIndex +
                                  "&product=" + encodeURIComponent(productTextBox.value) + "&deliveryLocation=" + deliveryLocationValue
                                  , "", sFeatures);

        // Retrieve the hidden field holding the transaction id's
        if (result != null) {
            __mydoPostBack('ASSOCIATIONS_CHANGED', lineItemIndex);
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////
// The purpose of this function is to fine a field in the FieldsAndRights hidden field in
// order to determine if the field has rights. If the field does not have rights, then it
// becomes read only. The default return is TRUE.
////////////////////////////////////////////////////////////////////////////////////////////////
function FieldHasRight(fieldName) {
    var fields = document.getElementById("FieldsAndRights");

    var hasRight = null;
    var fieldInQuestion = null;

    if (fields != null) {
        var pipeIndex = fields.value.indexOf('|');

        if (pipeIndex > -1) {
            fieldArray = fields.value.split('|');

            for (nextField = 0; nextField < fieldArray.length; nextField++) {
                fieldInQuestion = fieldArray[nextField];
                hasRight = fieldInQuestion.substring(0, 1).toUpperCase();
                fieldInQuestion = fieldInQuestion.substring(1).toUpperCase();

                if (fieldInQuestion == fieldName.toUpperCase()) {
                    if (hasRight == "F") {
                        return false;
                    }
                }
            }
        }
        else {
            try {
                hasRight = fields.value.substring(0, 1).toUpperCase();
                fieldInQuestion = fields.value.substring(1).toUpperCase();

                if (fieldInQuestion == fieldName.toUpperCase()) {
                    if (hasRight == "F") {
                        return false;
                    }
                }
            }
            catch (err) {
            }
        }
    }

    return true;
}

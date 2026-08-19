// This function is called by the all company text box/button controls.
function CompanySelect(role, companyTextBoxId) {
	var companyTextBox = document.getElementById(companyTextBoxId);
	var companyNameTextBox = document.getElementById("CompanyName" + companyTextBoxId);
	var managerString = null;
	var ownerString = null;
	var shipperString = null;
	var billToString = null;
	var supplierString = null;
	var limitSelectionsBasedOnHierarchy = "false";
	var companyShipToTextBox;
	var url = null;

	if (role === "CARRIER") {
		companyShipToTextBox = document.getElementById("TransactionFields.ShipToFG");
		if (companyShipToTextBox != null) {
			var shipToId = companyShipToTextBox.value;

			if (shipToId.substr(0, 1) === "<") {
				shipToId = "";
			}

			url = "../FMWebApp/CompanySelectForm.aspx?Unassigned=true&Role=" + role + "&Map=AUTHORIZED_CARRIER_MAP" + "&IDLink=" + encodeURIComponent(shipToId);
		}
		else {
			url = "../FMWebApp/CompanySelectForm.aspx?Unassigned=true&Role=" + role;
		}

		showModalDialogFrame({
			url: url,
			width: 855,
			height: 690,
			title: "Company Select",
			onClose: function () {
				if (this.returnValue != null) {
					if (companyTextBox.value !== this.returnValue[0]) {
						var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
						var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);
						var asciiValue3 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[2]);

						companyTextBox.value = asciiValue1;
						companyTextBox.title = asciiValue2;
						companyNameTextBox.value = asciiValue3;

						__doPostBack(companyTextBox.id, asciiValue1);
						return;
					}

					completeCompanySelection(this.returnValue, role, companyTextBoxId);
				}
			}
		});
	}
	else {
		limitSelectionsBasedOnHierarchy = document.getElementById("LimitSelectionsBasedOnHierarchy");

		if (limitSelectionsBasedOnHierarchy != null && limitSelectionsBasedOnHierarchy.value === "true") {
			if (companyTextBoxId.indexOf("TransactionFields.To") != -1) {
				managerString = document.getElementById("TransactionFields.ToManagerFG");
				ownerString = document.getElementById("TransactionFields.ToOwnerFG");
				shipperString = document.getElementById("TransactionFields.ToShipperFG");
				billToString = document.getElementById("TransactionFields.ToBillToFG");
				supplierString = document.getElementById("TransactionFields.ToSupplierFG");
			}
			else if (companyTextBoxId.indexOf("TransactionFields.From") != -1) {
				managerString = document.getElementById("TransactionFields.FromManagerFG");
				ownerString = document.getElementById("TransactionFields.FromOwnerFG");
				shipperString = document.getElementById("TransactionFields.FromShipperFG");
				billToString = document.getElementById("TransactionFields.FromBillToFG");
				supplierString = document.getElementById("TransactionFields.FromSupplierFG");
			}
			else {
				managerString = document.getElementById("TransactionFields.ManagerFG");
				ownerString = document.getElementById("TransactionFields.OwnerFG");
				shipperString = document.getElementById("TransactionFields.ShipperFG");
				billToString = document.getElementById("TransactionFields.BillToFG");
				supplierString = document.getElementById("TransactionFields.SupplierFG");
			}

			if (role === "MANAGER") {
				url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role;
			}
			else if (role === "OWNER" && managerString != null) {
				url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&UseHierarchy=true&ManagerST=" + managerString.value;
			}
			else if (role === "SHIPPER" && managerString != null && ownerString != null) {
				url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&UseHierarchy=true&ManagerST=" + managerString.value + "&OwnerST=" + ownerString.value;
			}
			else if (role === "CUSTOMER_BILLTO" && managerString != null && ownerString != null && shipperString != null) {
				url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&UseHierarchy=true&ManagerST=" + managerString.value + "&OwnerST=" + ownerString.value + "&ShipperST=" + shipperString.value;
			}
			else if (role === "CUSTOMER_SHIPTO" && managerString != null && ownerString != null && shipperString != null && billToString != null) {
				url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&UseHierarchy=true&ManagerST=" + managerString.value + "&OwnerST=" + ownerString.value + "&ShipperST=" + shipperString.value + "&BillToST=" + billToString.value;
			}
			else if (role == "SUPPLIER" && managerString != null && ownerString != null) {
				url = "../FMWebApp/CompanySelectForm.aspx?Role=" + role + "&UseHierarchy=true&ManagerST=" + managerString.value + "&OwnerST=" + ownerString.value;
			}


			showModalDialogFrame({
				url: url,
				width: 855,
				height: 690,
				title: "Company Select",
				onClose: function () {
					if (this.returnValue != null) {
						HandleCompanySelection(this.returnValue, role, companyTextBoxId);
					}
				}
			});
		}
		else {
			showModalDialogFrame({
				url: "../FMWebApp/CompanySelectForm.aspx?Role=" + role,
				width: 855,
				height: 690,
				title: "Company Select",
				onClose: function () {
					if (this.returnValue != null) {
						completeCompanySelection(this.returnValue, role, companyTextBoxId);
					}
				}
			});
		}
	}
}

function HandleCompanySelection(result, role, companyTextBoxId) {
	var companyTextBox = document.getElementById(companyTextBoxId);
	var managerString = document.getElementById("TransactionFields.ManagerFG");
	var ownerString = document.getElementById("TransactionFields.OwnerFG");
	var shipperString = document.getElementById("TransactionFields.ShipperFG");
	var billToString = document.getElementById("TransactionFields.BillToFG");
	var shipToString = document.getElementById("TransactionFields.ShipToFG");
	var supplierString = document.getElementById("TransactionFields.SupplierFG");

	var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(result[0]);
	var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(result[1]);

	if (role !== "CARRIER") {
		var loadIdBox = document.getElementById("TransactionFields.LoadID_FG");
		if (loadIdBox != null && loadIdBox.value != "") {
			loadIdBox.value = "";
		}
	}

	if (role === "MANAGER" && result != null && (managerString !== asciiValue1)) {
		companyTextBox.value = asciiValue1;
		companyTextBox.title = asciiValue2;
		__doPostBack(companyTextBox.id, asciiValue1);
	}
	else if (role === "OWNER" && result != null && (ownerString !== asciiValue1)) {
		companyTextBox.value = asciiValue1;
		companyTextBox.title = asciiValue2;
		__doPostBack(companyTextBox.id, asciiValue1);
	}
	else if (role === "SHIPPER" && result != null && (shipperString !== asciiValue1)) {
		companyTextBox.value = asciiValue1;
		companyTextBox.title = asciiValue2;
		__doPostBack(companyTextBox.id, asciiValue1);
	}
	else if (role === "CUSTOMER_BILLTO" && result != null && (billToString !== asciiValue1)) {
		companyTextBox.value = asciiValue1;
		companyTextBox.title = asciiValue2;
		__doPostBack(companyTextBox.id, asciiValue1);
	}
	else if (role === "CUSTOMER_SHIPTO" && result != null && (shipToString !== asciiValue1)) {
		companyTextBox.value = asciiValue1;
		companyTextBox.title = asciiValue2;
		__doPostBack(companyTextBox.id, asciiValue1);
	}
	else if (role === "SUPPLIER" && result != null && (supplierString !== asciiValue1)) {
		companyTextBox.value = asciiValue1;
		companyTextBox.title = asciiValue2;
		__doPostBack(companyTextBox.id, asciiValue1);
	}
}

function completeCompanySelection(result, role, companyTextBoxId) {
	var companyTextBox = document.getElementById(companyTextBoxId);
	var companyNameTextBox = document.getElementById("CompanyName" + companyTextBoxId);

	if (result != null) {
		if (role === "CUSTOMER_SHIPTO") {
			var companyShipToTextBox = document.getElementById("TransactionFields.ShipToFG");

			if (companyShipToTextBox != null) {
				var oldShipToValue = companyShipToTextBox.value;
			}
		}

		if (role === "CARRIER") {
			var carrierTextBox = document.getElementById("TransactionFields.CarrierFG");
			var oldCarrierValue = carrierTextBox.value;
		}

		var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(result[0]);
		var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(result[1]);
		var asciiValue3 = ReplaceNonBreakingSpaceHexWithSpace(result[2]);

		companyTextBox.value = asciiValue1;
		companyTextBox.title = asciiValue2;
		companyNameTextBox.value = asciiValue3;

		if (role === "CUSTOMER_SHIPTO") {
			__mydoPostBack('SHIPTO_REFRESH', asciiValue1);
		}

		if (role !== "CARRIER") {
			var loadIdBox = document.getElementById("TransactionFields.LoadID_FG");
			if (loadIdBox != null && loadIdBox.value != "" ) {
				loadIdBox.value = "";
				__mydoPostBack('LOADID_REFRESH', '');
			}
		}
	}
}

function FuelCardSelect(fuelCardTextBoxId) {
	var fuelCardTextBox = document.getElementById(fuelCardTextBoxId);

	showModalDialogFrame({
		url: "../FMWebApp/FuelCardSelectForm.aspx?Null=true",
		width: 855,
		height: 560,
		title: "Fuel Card Select",
		onClose: function () {
			if (this.returnValue != null) {
				var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				fuelCardTextBox.value = asciiValue1;
				fuelCardTextBox.title = asciiValue2;
				__doPostBack(fuelCardTextBox.id, '');
			}
		}
	});
}

////////////////////////////////////////////////////////////////////////////
// This function will handle the post back.
///////////////////////////////////////////////////////////////////////////
function __mydoPostBack(eventTarget, eventArgument) {
	var theform;
	if (window.navigator.appName.toLowerCase().indexOf("microsoft") > -1) {
		theform = document.Form1;
	}

	if (theform == null) {
		theform = document.forms["Form1"];
	}

	var element = document.activeElement;
	if (element != null) {
		document.cookie = " ActiveElement=" + element.id;
	}

	if (theform != null) {
		if (document.getElementsByName('__MYEVENTTARGET').length === 0) {
			var myEventTarget = document.createElement('input');
			myEventTarget.setAttribute('name', '__MYEVENTTARGET');
			myEventTarget.setAttribute('value', eventTarget);
			myEventTarget.setAttribute('type', 'hidden');
			theform.appendChild(myEventTarget);
		} else {
			document.getElementsByName('__MYEVENTTARGET')[0].setAttribute('value', eventTarget);
		}


		if (document.getElementsByName('__MYEVENTARGUMENT').length === 0) {
			var myEventArgument = document.createElement('input');
			myEventArgument.setAttribute('name', '__MYEVENTARGUMENT');
			myEventArgument.setAttribute('value', eventArgument);
			myEventArgument.setAttribute('type', 'hidden');
			theform.appendChild(myEventArgument);
		} else {
			document.getElementsByName('__MYEVENTARGUMENT')[0].setAttribute('value', eventArgument);
		}

		theform.submit();
	}
	else {
		alert("Form1 is null.");
	}
}

function formSubmit() {

	var element = document.activeElement;

	if (element != null && element.id !== '') {
		document.cookie = " ActiveElement=" + element.id;
	}

	var updatePanelDiv = document.getElementById("UpdatePanel1");
	if (updatePanelDiv != null) {
		updatePanelDiv.disabled = true;
	}

	// Display a wait message
	var waitImage = document.getElementById("waitDiv");
	waitImage.style.display = "inline";
}

function ProductSelect(productTextBoxId) {
	var productTextBox = document.getElementById(productTextBoxId);
	var companyShipToTextBox = document.getElementById("TransactionFields.ShipToFG");
	var companyManagerTextBox = document.getElementById("TransactionFields.ManagerFG");
	var companySupplierTextBox = document.getElementById("TransactionFields.SupplierFG");
	var companyId = "";

	if (companyShipToTextBox == null) {
		if (companySupplierTextBox != null) {
			companyId = companySupplierTextBox.value + "|supplier";
		}
		else if (companyManagerTextBox != null) {
			companyId = companyManagerTextBox.value + "|manager";
		}
	}
	else {
		companyId = companyShipToTextBox.value + "|shipto";
	}

	if (companyId.substr(0, 1) === "<") {
		companyId = "";
	}

	showModalDialogFrame({
		url: "../FMWebApp/ProductSelectForm.aspx?Type=MaxProduct&Map=MAX_MAP&IDLink=" + encodeURIComponent(companyId),
		width: 855,
		height: 560,
		title: "Product Select",
		onClose: function () {
			if (this.returnValue != null) {
				var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				productTextBox.value = asciiValue1;
				productTextBox.title = asciiValue2;

				__mydoPostBack('PRODUCT_REFRESH', asciiValue1);
			}
		}
	});
}

function OperatorSelect(operatorTextBoxId) {
	var operatorTextBox = document.getElementById(operatorTextBoxId);
	var carrierTextBox = document.getElementById("TransactionFields.CarrierFG");
	var carrierComboBoxTextbox = document.getElementById("TransactionFields.CarrierFG_TransactionFields.CarrierFG_TextBox");
	var carrierId = "";
	var driverIdentificationNumberTextBox = document.getElementById("TransactionFields.DriverIdentificationNumberFG");

	if (carrierComboBoxTextbox == null) {
		if (carrierTextBox != null && carrierTextBox.value != null) {
			carrierId = carrierTextBox.value;

			if (carrierId.substr(0, 1) === "<") {
				carrierId = "";
			}
		}
	}
	else {
		if (carrierComboBoxTextbox.value != null) {
			carrierId = carrierComboBoxTextbox.value;

			if (carrierId.substr(0, 1) === "<") {
				carrierId = "";
			}
		}
	}

	showModalDialogFrame({
		url: "../FMWebApp/PersonSelectForm.aspx?Role=MAX_PERSON_ROLE&IDCarrierLink=" + encodeURIComponent(carrierId),
		width: 855,
		height: 560,
		title: "Operator Select",
		onClose: function () {
			if (this.returnValue != null) {
				var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				operatorTextBox.value = asciiValue1;
				operatorTextBox.title = asciiValue2;

				if (driverIdentificationNumberTextBox != null) {
					var asciiValue3 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[2]);
					driverIdentificationNumberTextBox.value = asciiValue3;
				}

				__doPostBack(operatorTextBox.id, asciiValue1);
			}
		}
	});
}

function TankSelect(tankTextBoxId) {
	var tankTextBox = document.getElementById(tankTextBoxId);
	var productId = "";
	var productTextBox = null;
	var prodIdListId = tankTextBoxId.replace("StorageLocation", "Product");

	if (prodIdListId != null) {
		productTextBox = document.getElementById(prodIdListId);

		if (productTextBox == null) {
			if (prodIdListId.indexOf("ToProduct") > -1) {
				prodIdListId = prodIdListId.replace("ToProduct", "Product");
			}
			else if (prodIdListId.indexOf("FromProduct") > -1) {
				prodIdListId = prodIdListId.replace("FromProduct", "Product");
			}

			productTextBox = document.getElementById(prodIdListId);
		}
	}

	if (productTextBox != null) {
		productId = productTextBox.value;

		if (productId.substr(0, 1) === "<") {
			productId = "";
		}
	}

	var managerTextBox = document.getElementById("TransactionFields.ManagerFG");
	var managerId = "";

	if (managerTextBox != null) {
		managerId = managerTextBox.value;
	}

	showModalDialogFrame({
		url: "../FMWebApp/TankSelectForm.aspx?IDProductLink=" + encodeURIComponent(productId) +
			"&IDManagerLink=" + encodeURIComponent(managerId),
		width: 855,
		height: 560,
		title: "Tank Select",
		onClose: function () {
			if (this.returnValue != null) {
				var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				tankTextBox.value = asciiValue1;
				tankTextBox.title = asciiValue2;

				__mydoPostBack('TANK_REFRESH', asciiValue1);
			}
		}
	});
}

function EquipmentSelect(equipmentTextBoxId) {
	var equipmentTextBox = document.getElementById(equipmentTextBoxId);
	var carrierToTextBox = document.getElementById("TransactionFields.CarrierFG");
	var carrierId = null;
	var transactionTypeTextBox = document.getElementById("TransactionFields.TransAliasFG");
	var shipToId = null;
	var shipToTextBox = document.getElementById("TransactionFields.ShipToFG");
	var toShipToTextBox = document.getElementById("TransactionFields.ToShipToFG");

	if (carrierToTextBox != null) {
		carrierId = carrierToTextBox.value;
	}

	if (equipmentTextBoxId === "TransactionFields.DestinationEquipmentFG2") {
		if (toShipToTextBox != null) {
			shipToId = toShipToTextBox.value;
		}
	}
	else {
		if (shipToTextBox != null) {
			shipToId = shipToTextBox.value;
		}
	}

	showModalDialogFrame({
		url: "../FMWebApp/EquipmentSelectForm.aspx?EquipmentTextBoxID=" + equipmentTextBoxId +
			((carrierId != null) ? "&IDCarrierLink=" + encodeURIComponent(carrierId) : "") +
			((shipToId != null) ? "&IDShipToLink=" + encodeURIComponent(shipToId) : ""),
		width: 855,
		height: 560,
		title: "Equipment Select",
		onClose: function () {
			if (this.returnValue != null && this.returnValue.length > 1) {
				var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				equipmentTextBox.value = asciiValue1;
				equipmentTextBox.title = asciiValue2;

				__mydoPostBack('TAIL_NUMBER_CHANGED', equipmentTextBox.value);
			}
		}
	});
}

function CompartmentSelect(compartmentTextBoxId) {
	var compartmentTextBox = document.getElementById(compartmentTextBoxId);
	var equipmentTextBoxId = compartmentTextBoxId.replace("CompartmentID_FG", "EquipmentFG0");
	var equipmentTextBox = document.getElementById(equipmentTextBoxId);
	var equipmentId = "";

	if (equipmentTextBox != null) {
		equipmentId = equipmentTextBox.value;
	}
	else {
		// Line Item equipment isn't configured, use last equipment in header
		// the idea here is that the last equipment will be the one with compartments
		if (compartmentTextBoxId.indexOf("Destination") === -1) {
			equipmentTextBox = document.getElementById("TransactionFields.SourceEquipmentFG3");
			if (equipmentTextBox == null) {
				equipmentTextBox = document.getElementById("TransactionFields.SourceEquipmentFG2");
			}
			if (equipmentTextBox == null) {
				equipmentTextBox = document.getElementById("TransactionFields.SourceEquipmentFG1");
			}
		}
		else {
			equipmentTextBox = document.getElementById("TransactionFields.DestinationEquipmentFG3");
			if (equipmentTextBox == null) {
				equipmentTextBox = document.getElementById("TransactionFields.DestinationEquipmentFG2");
			}
			if (equipmentTextBox == null) {
				equipmentTextBox = document.getElementById("TransactionFields.DestinationEquipmentFG1");
			}
		}

		if (equipmentTextBox != null) {
			equipmentId = equipmentTextBox.value;
		}
	}

	showModalDialogFrame({
		url: "../FMWebApp/CompartmentSelectForm.aspx?EquipmentID=" + encodeURIComponent(equipmentId),
		width: 855,
		height: 560,
		title: "Compartment Select",
		onClose: function () {
			if (this.returnValue != null) {
				var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				compartmentTextBox.value = asciiValue1;
				compartmentTextBox.title = asciiValue2;
			}
		}
	});
}

function InstructionsButton_Click(itemIdentityGuid) {
	showModalDialogFrame({
		url: "../FMWebApp/SpecialInstructionsForm.aspx?mode=txdetail&ItemIdentityGuid=" + itemIdentityGuid,
		width: 855,
		height: 560,
		title: "Instructions"
	});
}

function AssociateTx(lineItemIndex, aggregate) {
	var productControl = document.getElementById("LineItemDataGrid$" + lineItemIndex + ".-1$TransactionFields.LineItemProductFG");
	var productValue = "";

	if (productControl == null) {
		productControl = document.getElementById("LineItemDataGrid_" + lineItemIndex + ".-1_TransactionFields.LineItemProductFG");

		if (productControl != null) {
			var productTextbox = productControl.control.get_textBoxControl();
			productValue = productTextbox.value;
		}
	}
	else {
		productValue = productControl.value;
	}

	var deliveryLocationTextBox = document.getElementById("LineItemDataGrid$" + lineItemIndex +
		".-1$TransactionFields.LineItemDeliveryLocationFG");

	if (deliveryLocationTextBox == null) {
		deliveryLocationTextBox = document.getElementById("LineItemDataGrid_" + lineItemIndex + ".-1_TransactionFields.LineItemDeliveryLocationFG_TextBox");
	}

	var deliveryLocationValue = "";

	if (deliveryLocationTextBox != null) {
		deliveryLocationValue = "&deliveryLocation=" + encodeURIComponent(deliveryLocationTextBox.value);
	}

	if (productControl == null) {
		alert("Product Control not found.");
	}
	else if (productValue === "") {
		alert("Please select a product.");
	}
	else {
		showModalDialogFrame({
			url: "../Accounting/SelectAssociatedTxDialog.aspx?product=" + encodeURIComponent(productValue) + deliveryLocationValue,
			width: 855,
			height: 560,
			onClose: function () {
				// Retrieve the hidden field holding the transaction id's
				if (this.returnValue != null) {
					__mydoPostBack('ASSOCIATIONS_CHANGED', lineItemIndex);
				}
			}
		});
	}
}

function MeterSelect(meterTextBoxId) {
	var meterTextBox = document.getElementById(meterTextBoxId);

	showModalDialogFrame({
		url: "../FMWebApp/MeterSelectForm.aspx?MeterTextBoxID=" + meterTextBoxId + "&All=false&FilterOnAsset=false&ShowEmptyRow=true",
		width: 855,
		height: 560,
		title: "Meter Select",
		onClose: function () {
			if (this.returnValue != null && this.returnValue.length > 1) {
				var asciiValue1 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[0]);
				var asciiValue2 = ReplaceNonBreakingSpaceHexWithSpace(this.returnValue[1]);

				meterTextBox.value = asciiValue1;
				meterTextBox.title = asciiValue2;
			}
		}
	});
}
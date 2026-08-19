var FMGeoTrackingSideMenu = FMGeoTrackingSideMenu || {
	selectedEquipment: ""
	, selectedFacility: ""
	, selectedTank: ""
	, selectedDeliveryLocation: ""
};
FMGeoTrackingSideMenu.imageRootPath = null;

//=============================================================
// This function handles the window resize in order to refresh
// the map.  The only way that I found to reposition the pan/zoom
// object.
//=============================================================
FMGeoTrackingSideMenu.RefreshMap = function() 
{
	$("#MapBaseForm").submit();
}

//==================================================================================
// This function handles the select event of the Breadcrumb dropdown control.
// It will set the selected value into a hidden field to be consumed
// by the controller.
//==================================================================================
FMGeoTrackingSideMenu.GetSelectedBreadcrumb = function()
{
	var selectedBreadcrumb = $("#DropdownModel_BreadcrumbId").val();
	$('#DropdownModel_SelectedBreadcrumb').val(selectedBreadcrumb);		
	$('#MenuItemChange').val(true);
}

/* Set the width of the side navigation to 300px */
FMGeoTrackingSideMenu.openNav = function ()
{
	document.getElementById("mySidenav").style.width = "300px";
	document.getElementById('OpenMenuBtn').style.visibility = 'hidden';
	document.getElementById('RefreshIconBtn').style.visibility = 'hidden';
	document.getElementById("mySidenav").style.overflowY = "auto";
	document.getElementById("mySidenav").style.overflowX = "auto";
}

/* Set the width of the side navigation to 0 */
FMGeoTrackingSideMenu.closeNav = function ()
{
	document.getElementById("mySidenav").style.width = "0";
	document.getElementById('OpenMenuBtn').style.visibility = 'visible';
	document.getElementById('RefreshIconBtn').style.visibility = 'visible';
	document.getElementById("mySidenav").style.overflowY = "hidden";
	document.getElementById("mySidenav").style.overflowX = "auto";
}

//================================================================
// This function will get the URL path for the "images".
//================================================================
FMGeoTrackingSideMenu.GetImagesPath = function ()
{
	var protocol	= window.location.protocol;
	var host		= window.location.host;
	var sourcePath	= protocol + "//" + host + "/" + FMGeoTrackingSideMenu.imageRootPath;

	return sourcePath;
}

//==========================================================================
// This function will toggle the equipment menu.
//==========================================================================
FMGeoTrackingSideMenu.EquipmentMenuSpan = function ()
{
	var menuExpandArrowImage = document.getElementById("EquipmentMenuExpandArrow");
	var model = FMGeoTrackingMap.GetMapViewModel();
	var count = model.DropdownModel.EquipmentDropdownList.length;


	// Do not expand or collapse of there are not items.
	if (count === 0)
	{
		return;
	}

	if ($("#DropdownModel_EquipmentMenuExpanded").val() === "0")
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("EquipmentMenuDivId", "block");
		menuExpandArrowImage.src = FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-down.png";
		$("#DropdownModel_EquipmentMenuExpanded").val(1);
	}
	else
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("EquipmentMenuDivId", "none");
		menuExpandArrowImage.src = FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-up.png";
		$("#DropdownModel_EquipmentMenuExpanded").val(0);
	}
}

//==========================================================================
// This function will toggle the delivery location menu.
//==========================================================================
FMGeoTrackingSideMenu.DeliveryLocationMenuSpan = function ()
{
	var menuExpandArrowImage = document.getElementById("DeliveryLocationMenuExpandArrow");
	var model = FMGeoTrackingMap.GetMapViewModel();
	var count = model.DropdownModel.DeliveryLocationDropdownList.length;

	// Do not expand or collapse of there are not items.
	if (count === 0)
	{
		return;
	}

	if ($("#DropdownModel_DeliveryLocationMenuExpanded").val() === "0")
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("DeliveryLocationMenuDivId", "block");
		menuExpandArrowImage.src = FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-down.png";
		$("#DropdownModel_DeliveryLocationMenuExpanded").val(1);
	}
	else
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("DeliveryLocationMenuDivId", "none");
		menuExpandArrowImage.src = FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-up.png";
		$("#DropdownModel_DeliveryLocationMenuExpanded").val(0);
	}
}

//==========================================================================
// This function will toggle the facility menu.
//==========================================================================
FMGeoTrackingSideMenu.FacilityMenuSpan = function ()
{
	var menuExpandArrowImage = document.getElementById("FacilityMenuExpandArrow");
	var model = FMGeoTrackingMap.GetMapViewModel();
	var count = model.DropdownModel.FacilityDropdownList.length;

	// Do not expand or collapse of there are not items.
	if (count === 0)
	{
		return;
	}

	if ($("#DropdownModel_FacilityMenuExpanded").val() === "0")
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("FacilityMenuDivId", "block");
		menuExpandArrowImage.src = FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-down.png";
		$("#DropdownModel_FacilityMenuExpanded").val(1);
	}
	else
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("FacilityMenuDivId", "none");
		menuExpandArrowImage.src = FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-up.png";
		$("#DropdownModel_FacilityMenuExpanded").val(0);
	}
}

//==========================================================================
// This function will toggle the tank menu.
//==========================================================================
FMGeoTrackingSideMenu.TankMenuSpan = function()
{
	var menuExpandArrowImage = document.getElementById("TankMenuExpandArrow");
	var model = FMGeoTrackingMap.GetMapViewModel();
	var count = model.DropdownModel.TankDropdownList.length;

	// Do not expand or collapse of there are not items.
	if (count === 0)
	{
		return;
	}

	if ($("#DropdownModel_TankMenuExpanded").val() === "0")
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("TankMenuDivId", "block");
		menuExpandArrowImage.src = FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-down.png";
		$("#DropdownModel_TankMenuExpanded").val(1);
	}
	else
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("TankMenuDivId", "none");
		menuExpandArrowImage.src = FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-up.png";
		$("#DropdownModel_TankMenuExpanded").val(0);
	}
}

//==========================================================================
// This function will toggle the breadcrumb menu.
//==========================================================================
FMGeoTrackingSideMenu.BreadcrumbMenuSpan = function ()
{
	var menuExpandArrowImage = document.getElementById("BreadcrumbMenuExpandArrow");

	if ($("#DropdownModel_BreadcrumbMenuExpanded").val() === "0")
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("BreadcrumbMenuDivId", "block");
		menuExpandArrowImage.src = FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-down.png";
		$("#DropdownModel_BreadcrumbMenuExpanded").val(1);
	}
	else
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("BreadcrumbMenuDivId", "none");
		menuExpandArrowImage.src = FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-up.png";
		$("#DropdownModel_BreadcrumbMenuExpanded").val(0);
	}
}

//==================================================================
// This function will check or uncheck all the checkboxes based on
// if the select all item was selected.
//==================================================================
FMGeoTrackingSideMenu.CheckUncheckAll = function (checked, inputItem, count, includeSelectAll)
{
	var startingIndex = 1;

	if (includeSelectAll === "T")
	{
		startingIndex = 0;
	}

	for (var nextItem = startingIndex; nextItem < count; nextItem++)
	{
		var inputId = inputItem + nextItem;
		var inputObj = document.getElementById(inputId);
		inputObj.checked = checked;
		inputObj.disabled = false;
	}
}

//==================================================================
// This function will add a new checkbox selection value to the
// selection list and return the new selection list.
//==================================================================
FMGeoTrackingSideMenu.AddSelection = function (selectionStr, newValue)
{
	var localSelectionStr;

	if (selectionStr == null || selectionStr.length === 0)
	{
		return newValue;
	}

	var parts = selectionStr.split(",");

	if(parts.length === 1)
	{
		localSelectionStr = selectionStr;

		if (selectionStr !== newValue)
		{
			localSelectionStr = selectionStr + "," + newValue;
		}

		return localSelectionStr;
	}

	localSelectionStr = selectionStr;
	var found = false;

	for (var nextItem = 0; nextItem < parts.length; nextItem++)
	{
		if (parts[nextItem] === newValue)
		{
			found = true;
			break;
		}
	}

	if (found === false)
	{
		localSelectionStr = selectionStr + "," + newValue;
	}

	return localSelectionStr;
}

//==================================================================
// This function will remove a new checkbox selection value from the
// selection list and return the new selection list.
//==================================================================
FMGeoTrackingSideMenu.RemoveSelection = function (selectionStr, removeValue)
{
	if (selectionStr == null || selectionStr.length === 0)
	{
		return "";
	}

	var parts = selectionStr.split(",");

	if (parts.length === 1)
	{
		if (selectionStr !== removeValue)
		{
			return selectionStr;
		}

		return "";
	}

	var localSelectionStr = "";

	for (var nextItem = 0; nextItem < parts.length; nextItem++)
	{
		if (parts[nextItem] !== removeValue)
		{
			if (localSelectionStr.length === 0)
			{
				localSelectionStr = parts[nextItem];
			}
			else
			{
				localSelectionStr = localSelectionStr + "," + parts[nextItem];
			}
		}
	}

	return localSelectionStr;
}

//==================================================================
// This function handles the equipment checkbox onchange
// event.
//==================================================================
FMGeoTrackingSideMenu.EquipmentCbOnChange = function (equipmentObj)
{
	var model = FMGeoTrackingMap.GetMapViewModel();
	var count = model.DropdownModel.EquipmentDropdownList.length;

	if (equipmentObj.id === "CheckboxEquipment0")
	{
		if (equipmentObj.checked)
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(true, "CheckboxEquipment", count, "F");
		}
		else
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(false, "CheckboxEquipment", count, "F");
		}
	}
	else
	{
		if (equipmentObj.checked === false)
		{
			$("#CheckboxEquipment0").attr('checked', false);
		}
	}

	FMGeoTrackingSideMenu.SetEquipmentSelections();
	$('#MenuItemChange').val(true);
}

//==================================================================
// This function handles the facility checkbox onchange
// event.
//==================================================================
FMGeoTrackingSideMenu.FacilityCbOnChange = function (facilityObj)
{
	var model = FMGeoTrackingMap.GetMapViewModel();
	var count = model.DropdownModel.FacilityDropdownList.length;

	if (facilityObj.id === "CheckboxFacility0")
	{
		if (facilityObj.checked)
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(true, "CheckboxFacility", count, "F");
		}
		else
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(false, "CheckboxFacility", count, "F");
		}
	}
	else
	{
		if (facilityObj.checked === false)
		{
			$("#CheckboxFacility0").attr('checked', false);
		}
	}

	FMGeoTrackingSideMenu.SetFacilitySelections();
	$('#MenuItemChange').val(true);
}

//==================================================================
// This function handles the delivery location checkbox onchange
// event.
//==================================================================
FMGeoTrackingSideMenu.LocationCbOnChange = function (deliveryLocationObj)
{
	var model = FMGeoTrackingMap.GetMapViewModel();
	var count = model.DropdownModel.DeliveryLocationDropdownList.length;

	if (deliveryLocationObj.id === "CheckboxDeliveryLocation0")
	{
		if (deliveryLocationObj.checked)
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(true, "CheckboxDeliveryLocation", count, "F");
		}
		else
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(false, "CheckboxDeliveryLocation", count, "F");
		}
	}
	else
	{
		if (deliveryLocationObj.checked === false)
		{
			$("#CheckboxDeliveryLocation0").attr('checked', false);
		}
	}

	FMGeoTrackingSideMenu.SetDeliveryLocationSelections();
	$('#MenuItemChange').val(true);
}

//==================================================================
// This function handles the tank checkbox onchange
// event.
//==================================================================
FMGeoTrackingSideMenu.TankCbOnChange = function (tankObj)
{
	var model = FMGeoTrackingMap.GetMapViewModel();
	var count = model.DropdownModel.TankDropdownList.length;

	if (tankObj.id === "CheckboxTank0")
	{
		if (tankObj.checked)
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(true, "CheckboxTank", count, "F");
		}
		else
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(false, "CheckboxTank", count, "F");
		}
	}
	else
	{
		if (tankObj.checked === false)
		{
			$("#CheckboxTank0").attr('checked', false);
		}
	}

	FMGeoTrackingSideMenu.SetTankSelections();
	$('#MenuItemChange').val(true);
}

//==================================================================
// This function handles the breadcrumb radio button onchange
// event.
//==================================================================
FMGeoTrackingSideMenu.BreadcrumbRbOnChange = function (breadcrumbObj) 
{
	var model = FMGeoTrackingMap.GetMapViewModel();
	var count = model.DropdownModel.BreadcrumbDropdownList.length;

	var settingValue;
	var inputItem = "RadioBtnBreadcrumb";

	var length = breadcrumbObj.id.length - 17;
	var selectedIndexStr = breadcrumbObj.id.substr(18, length);
	var selectedIndex = parseInt(selectedIndexStr);

	for (var nextItem = 0; nextItem < count; nextItem++)
	{
		if (nextItem === selectedIndex) 
		{
			if (nextItem === 0) 
			{
				settingValue = "Current Position";
			}
			else if (nextItem === 1) 
			{
				settingValue = nextItem + " day";
			}
			else 
			{
				settingValue = nextItem + " days";
			}

			$("#DropdownModel_SelectedBreadcrumb").val(settingValue);
			$("#DropdownModel_SelectedBreadcrumbIndex").val(nextItem);
			continue;
		}

		var inputId = inputItem + nextItem;
		var inputObj = document.getElementById(inputId);
		inputObj.checked = false;
	}

	$('#MenuItemChange').val(true);
}

//===================================================================
// This function will set the radio buttons back to their state from
// the postback.
//===================================================================
FMGeoTrackingSideMenu.SetMenuRadioBtns = function () 
{
	var nextItem;
	var elementId;
	var itemChecked;
	var menuItem;

	var model = FMGeoTrackingMap.GetMapViewModel();
	var radioBtnCount = model.DropdownModel.BreadcrumbDropdownList.length;

	// Initialize the equipment menu checkboxes.
	for (nextItem = 0; nextItem < radioBtnCount; nextItem++)
	{
		elementId = "RadioBtnBreadcrumb" + nextItem;
		menuItem = model.DropdownModel.BreadcrumbDropdownList[nextItem];
		itemChecked = menuItem.Checked;

		FMGeoTrackingSideMenu.InitializeCheckbox(elementId, itemChecked);
	}
}

//===================================================================
// This function will set the checkboxes back to their state from
// the postback.
//===================================================================
FMGeoTrackingSideMenu.SetMenuCheckboxes = function ()
{
	var nextItem;
	var elementId;
	var selectedValue;
	var itemChecked;
	var menuItem;

	var model					= FMGeoTrackingMap.GetMapViewModel();
	var equipmentCount			= model.DropdownModel.EquipmentDropdownList.length;
	var deliveryLocationCount	= model.DropdownModel.DeliveryLocationDropdownList.length;
	var facilityCount			= model.DropdownModel.FacilityDropdownList.length;
	var tankCount				= model.DropdownModel.TankDropdownList.length;

	// Initialize the equipment menu checkboxes.
	for (nextItem = 0; nextItem < equipmentCount; nextItem++)
	{
		elementId = "CheckboxEquipment" + nextItem;
		menuItem = model.DropdownModel.EquipmentDropdownList[nextItem];

		selectedValue = menuItem.Value;
		itemChecked = menuItem.Checked;

		if (itemChecked && selectedValue === "-88")
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(true, "CheckboxEquipment", equipmentCount, "T");
			break;
		}
		else
		{
			FMGeoTrackingSideMenu.InitializeCheckbox(elementId, itemChecked);
		}
	}

	// Initialize the delivery location menu checkboxes.
	for (nextItem = 0; nextItem < deliveryLocationCount; nextItem++)
	{
		elementId = "CheckboxDeliveryLocation" + nextItem;
		menuItem = model.DropdownModel.DeliveryLocationDropdownList[nextItem];

		selectedValue = menuItem.Value;
		itemChecked = menuItem.Checked;

		if (itemChecked && selectedValue === "-88")
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(true, "CheckboxDeliveryLocation", deliveryLocationCount, "T");
			break;
		}
		else
		{
			FMGeoTrackingSideMenu.InitializeCheckbox(elementId, itemChecked);
		}
	}

	// Initialize the facility menu checkboxes.
	for (nextItem = 0; nextItem < facilityCount; nextItem++)
	{
		elementId = "CheckboxFacility" + nextItem;
		menuItem = model.DropdownModel.FacilityDropdownList[nextItem];

		selectedValue = menuItem.Value;
		itemChecked = menuItem.Checked;

		if (itemChecked && selectedValue === "-88")
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(true, "CheckboxFacility", facilityCount, "T");
			break;
		}
		else
		{
			FMGeoTrackingSideMenu.InitializeCheckbox(elementId, itemChecked);
		}
	}

	// Initialize the tank menu checkboxes.
	for (nextItem = 0; nextItem < tankCount; nextItem++)
	{
		elementId = "CheckboxTank" + nextItem;
		menuItem = model.DropdownModel.TankDropdownList[nextItem];

		selectedValue = menuItem.Value;
		itemChecked = menuItem.Checked;

		if (itemChecked && selectedValue === "-88")
		{
			FMGeoTrackingSideMenu.CheckUncheckAll(true, "CheckboxTank", tankCount, "T");
			break;
		}
		else
		{
			FMGeoTrackingSideMenu.InitializeCheckbox(elementId, itemChecked);
		}
	}
}

//=================================================================
// This function will initialize the given checkbox.
//=================================================================
FMGeoTrackingSideMenu.InitializeCheckbox = function (elementName, checked)
{
	var elementObj = document.getElementById(elementName);
	elementObj.checked = false;

	if (checked)
	{
		elementObj.checked = true;
	}
}

//=================================================================
// This function will set the menu item to expanded or collapse
// based on the model settings.
//=================================================================
FMGeoTrackingSideMenu.SetInitialMenuExpansion = function () 
{
	var downArrow	= FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-down.png";
	var upArrow		= FMGeoTrackingSideMenu.GetImagesPath() + "/Arrow-up.png";

	var menuExpandArrowImage;

	if ($("#DropdownModel_EquipmentMenuExpanded").val() === "1") 
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("EquipmentMenuDivId", "block");
		menuExpandArrowImage = document.getElementById("EquipmentMenuExpandArrow");
		menuExpandArrowImage.src = downArrow;
	}
	else 
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("EquipmentMenuDivId", "none");
		menuExpandArrowImage = document.getElementById("EquipmentMenuExpandArrow");
		menuExpandArrowImage.src = upArrow;			
	}

	if ($("#DropdownModel_DeliveryLocationMenuExpanded").val() === "1") 
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("DeliveryLocationMenuDivId", "block");
		menuExpandArrowImage = document.getElementById("DeliveryLocationMenuExpandArrow");
		menuExpandArrowImage.src = downArrow;
	}
	else 
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("DeliveryLocationMenuDivId", "none");
		menuExpandArrowImage = document.getElementById("DeliveryLocationMenuExpandArrow");
		menuExpandArrowImage.src = upArrow;			
	}

	if ($("#DropdownModel_FacilityMenuExpanded").val() === "1") 
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("FacilityMenuDivId", "block");
		menuExpandArrowImage = document.getElementById("FacilityMenuExpandArrow");
		menuExpandArrowImage.src = downArrow;
	}
	else 
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("FacilityMenuDivId", "none");
		menuExpandArrowImage = document.getElementById("FacilityMenuExpandArrow");
		menuExpandArrowImage.src = upArrow;			
	}

	if ($("#DropdownModel_TankMenuExpanded").val() === "1") 
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("TankMenuDivId", "block");
		menuExpandArrowImage = document.getElementById("TankMenuExpandArrow");
		menuExpandArrowImage.src = downArrow;
	}
	else 
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("TankMenuDivId", "none");
		menuExpandArrowImage = document.getElementById("TankMenuExpandArrow");
		menuExpandArrowImage.src = upArrow;			
	}

	if ($("#DropdownModel_BreadcrumbMenuExpanded").val() === "1") 
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("BreadcrumbMenuDivId", "block");
		menuExpandArrowImage = document.getElementById("BreadcrumbMenuExpandArrow");
		menuExpandArrowImage.src = downArrow;
	}
	else 
	{
		FMGeoTrackingSideMenu.SetMenuItemExpandState("BreadcrumbMenuDivId", "none");
		menuExpandArrowImage = document.getElementById("BreadcrumbMenuExpandArrow");
		menuExpandArrowImage.src = upArrow;			
	}
}

//=============================================================
// This function will set the menu item expand/collapse state.
// "none" = collapse, "block" = expand
//=============================================================
FMGeoTrackingSideMenu.SetMenuItemExpandState = function (elementId, menuState)
{
	var spanObj					= document.getElementById(elementId);
	var parentNode				= spanObj.parentNode;
	var childList				= parentNode.getElementsByTagName('UL');
	childList[0].style.display	= menuState;
}

//=============================================================
// This function will extrpolation the new location for the Pan
// Zoom object on the page.
//=============================================================
FMGeoTrackingSideMenu.CalculatePanZoomObjLocation = function () 
{
	// windowSize - minWinSize    x - minCalSize
	// -----------------------  = -----------------------
	// maxWinSize - minWinSize    maxCalSize - minCalSize
	//
	// Where x is the new value.

	var minWinSize = 273;
	var maxWinSize = 1655;
	var minCalSize = 91;
	var maxCalSize = 780;
	var windowSize = window.innerWidth;

	var a = windowSize - minWinSize;
	var b = maxWinSize - minWinSize;
	var ab = a / b;
	var c = maxCalSize - minCalSize;
	var x = minCalSize + (ab * c);

	return x;
}

//=====================================================================================
// This function will get all the equipment checkboxes that are checked and build a 
// selection string that is stored in hidden selected equipment control.
//=====================================================================================
FMGeoTrackingSideMenu.SetEquipmentSelections = function ()
{
	$("#DropdownModel_SelectedEquipment").val("");
	FMGeoTrackingSideMenu.selectedEquipment = "";
	var loop = true;
	var allItemsChecked = true;

	$('#EquipmentUlId > li > table > tbody > tr > td > label > input').each(function ()
	{
		if (loop === false)
		{
			return;
		}

		var checked			= $(this).is(":checked");
		var inputId			= $(this).attr("id");
		var selectedValue	= $(this).attr("value");

		if (typeof (checked) != "undefined" && typeof (inputId) != "undefined")
		{
			if (inputId === "CheckboxEquipment0")
			{
				if (checked)
				{
					$("#DropdownModel_SelectedEquipment").val("-88");
					FMGeoTrackingSideMenu.selectedEquipment = "-88";
					loop = false;
				}
			}
			else
			{
				if (checked)
				{
					var selectionStr = FMGeoTrackingSideMenu.AddSelection($("#DropdownModel_SelectedEquipment").val(), selectedValue);
					$("#DropdownModel_SelectedEquipment").val(selectionStr);
					FMGeoTrackingSideMenu.selectedEquipment = FMGeoTrackingSideMenu.AddSelection(FMGeoTrackingSideMenu.selectedEquipment, selectedValue);
				}
				else
				{
					allItemsChecked = false;
				}
			}
		}
	});

	if (allItemsChecked)
	{
		$("#DropdownModel_SelectedEquipment").val("-88");
		FMGeoTrackingSideMenu.selectedEquipment = "-88";
		document.getElementById("CheckboxEquipment0").checked = true;
	}

	FMGeoTrackingMap.MapFilter(FMGeoTrackingSideMenu.selectedEquipment, "Equipment");
}

//=====================================================================================
// This function will get all the facility checkboxes that are checked and build a 
// selection string that is stored in hidden selected facility control.
//=====================================================================================
FMGeoTrackingSideMenu.SetFacilitySelections = function ()
{
	$("#DropdownModel_SelectedFacility").val("");
	FMGeoTrackingSideMenu.selectedFacility = "";
	var loop = true;
	var allItemsChecked = true;

	$('#FacilityUlId > li > table > tbody > tr > td > label > input').each(function ()
	{
		if (loop === false)
		{
			return;
		}

		var checked			= $(this).is(":checked");
		var inputId			= $(this).attr("id");
		var selectedValue	= $(this).attr("value");

		if (typeof (checked) != "undefined" && typeof (inputId) != "undefined")
		{
			if (inputId === "CheckboxFacility0")
			{
				if (checked)
				{
					$("#DropdownModel_SelectedFacility").val("-88");
					FMGeoTrackingSideMenu.selectedFacility = "-88";
					loop = false;
				}
			}
			else
			{
				if (checked)
				{
					var selectionStr = FMGeoTrackingSideMenu.AddSelection($("#DropdownModel_SelectedFacility").val(), selectedValue);
					$("#DropdownModel_SelectedFacility").val(selectionStr);
					FMGeoTrackingSideMenu.selectedFacility = FMGeoTrackingSideMenu.AddSelection(FMGeoTrackingSideMenu.selectedFacility, selectedValue);
				}
				else
				{
					allItemsChecked = false;
				}
			}
		}
	});

	if (allItemsChecked)
	{
		$("#DropdownModel_SelectedFacility").val("-88");
		FMGeoTrackingSideMenu.selectedFacility = "-88";
		document.getElementById("CheckboxFacility0").checked = true;
	}

	FMGeoTrackingMap.MapFilter(FMGeoTrackingSideMenu.selectedFacility, "Facility");
}

//=====================================================================================
// This function will get all the Delivery Location checkboxes that are checked and build a 
// selection string that is stored in hidden selected Delivery Location control.
//=====================================================================================
FMGeoTrackingSideMenu.SetDeliveryLocationSelections = function ()
{
	$("#DropdownModel_SelectedDeliveryLocation").val("");
	FMGeoTrackingSideMenu.selectedDeliveryLocation = "";
	var loop = true;
	var allItemsChecked = true;

	$('#DeliveryLocationUlId > li > table > tbody > tr > td > label > input').each(function ()
	{
		if (loop === false)
		{
			return;
		}

		var checked			= $(this).is(":checked");
		var inputId			= $(this).attr("id");
		var selectedValue	= $(this).attr("value");

		if (typeof (checked) != "undefined" && typeof (inputId) != "undefined")
		{
			if (inputId === "CheckboxDeliveryLocation0")
			{
				if (checked)
				{
					$("#DropdownModel_SelectedDeliveryLocation").val("-88");
					FMGeoTrackingSideMenu.selectedDeliveryLocation = "-88";
					loop = false;
				}
			}
			else
			{
				if (checked)
				{
					var selectionStr = FMGeoTrackingSideMenu.AddSelection($("#DropdownModel_SelectedDeliveryLocation").val(), selectedValue);
					$("#DropdownModel_SelectedDeliveryLocation").val(selectionStr);
					FMGeoTrackingSideMenu.selectedDeliveryLocation = FMGeoTrackingSideMenu.AddSelection(FMGeoTrackingSideMenu.selectedDeliveryLocation, selectedValue);
				}
				else
				{
					allItemsChecked = false;
				}
			}
		}
	});

	if (allItemsChecked)
	{
		$("#DropdownModel_SelectedDeliveryLocation").val("-88");
		FMGeoTrackingSideMenu.selectedDeliveryLocation = "-88";
		document.getElementById("CheckboxDeliveryLocation0").checked = true;
	}

	FMGeoTrackingMap.MapFilter(FMGeoTrackingSideMenu.selectedDeliveryLocation, "DeliveryLocation");
}

//=====================================================================================
// This function will get all the Tank checkboxes that are checked and build a 
// selection string that is stored in hidden Tank Location control.
//=====================================================================================
FMGeoTrackingSideMenu.SetTankSelections = function ()
{
	$("#DropdownModel_SelectedTank").val("");
	FMGeoTrackingSideMenu.selectedTank = "";
	var loop = true;
	var allItemsChecked = true;

	$('#TankUlId > li > table > tbody > tr > td > label > input').each(function ()
	{
		if (loop === false)
		{
			return;
		}

		var checked			= $(this).is(":checked");
		var inputId			= $(this).attr("id");
		var selectedValue	= $(this).attr("value");

		if (typeof (checked) != "undefined" && typeof (inputId) != "undefined")
		{
			if (inputId === "CheckboxTank0")
			{
				if (checked)
				{
					$("#DropdownModel_SelectedTank").val("-88");
					FMGeoTrackingSideMenu.selectedTank = "-88";
					loop = false;
				}
			}
			else
			{
				if (checked)
				{
					var selectionStr = FMGeoTrackingSideMenu.AddSelection($("#DropdownModel_SelectedTank").val(), selectedValue);
					$("#DropdownModel_SelectedTank").val(selectionStr);
					FMGeoTrackingSideMenu.selectedTank = FMGeoTrackingSideMenu.AddSelection(FMGeoTrackingSideMenu.selectedTank, selectedValue);
				}
				else
				{
					allItemsChecked = false;
				}
			}
		}
	});

	if (allItemsChecked)
	{
		$("#DropdownModel_SelectedTank").val("-88");
		FMGeoTrackingSideMenu.selectedTank = "-88";
		document.getElementById("CheckboxTank0").checked = true;
	}

	FMGeoTrackingMap.MapFilter(FMGeoTrackingSideMenu.selectedTank, "Tank");
}

var FMGeoTrackingEquipmentHistory = FMGeoTrackingEquipmentHistory ||
{
	expandRowIdList: [],
	expandRowStateList: [],
	imageRootPath: "",
	previousBreadcrumbSelection: 0,
	assetTrackingDetailGuidStr: "",
	startInvestigateModalDialog: "StartInvestigateDialog",
	completeInvestigateModalDialog: "CompleteInvestigateDiv",
	remarksModalDialogId: "RemarksModalDiv",
	startInvestigationNoBtnId: "StartInvestigationNoBtn",
	startInvestigationYesBtnId: "StartInvestigationYesBtn",
	startInvestigateDialogXCloseId: "StartInvestigateDialogXClose",
	startInvestigateBtnId: "StartInvestigateBtn",
	completeInvestigateBtnId: "CompleteInvestigateBtn",
	passedRadioButtonId: "RadioBtnPassed",
	failedRadioButtonId: "RadioBtnFailed",
	hasStartInvestigateRight: false,
	hasCompleteInvestigateRight: false,
	equipmentHistoryModel: null,
	selectionRecordList: [],
	messageStateEnum: { Normal: 0, Contaminated: 1, Investigate: 2, InvestigateCompletedFailed: 3, InvestigateCompletedPassed: 4 },
	radioActiveSymbolCode: 0x2622,
	yellow: "#E6E600",
	red: "#FA4654",
	orange: "#FF8C00",
	green: "#0BA50E",
	white: "#FFFFFF",
	darkGray: "#777777",
	lightGray: "#E8E8E8",
	black: "#000000"
};

//====================================================================================
// This function will create a row in the equipment history table dynamically.
//====================================================================================
FMGeoTrackingEquipmentHistory.CreateEquipmentHistoryRow = function (rowContentObj, expandRowId, setBackground)
{
	var labelRowFontSize = "10px";
	var labelTextColor = FMGeoTrackingEquipmentHistory.darkGray;

	var equipmentTable = document.getElementById("EquipmentHistoryTable");
	var length = equipmentTable.rows.length;

	// Create a new html table row "<tr>" at the bottom of the table.
	var newRow = equipmentTable.insertRow(length);
	newRow.id = expandRowId + "_" + rowContentObj.AssetTrackingDetailGuidStr;
	newRow.style.display = "table-row";
	newRow.setAttribute("DateFilter", rowContentObj.MessageTimestamp);

	if (setBackground)
	{
		newRow.style.background = FMGeoTrackingEquipmentHistory.lightGray;
		labelTextColor = FMGeoTrackingEquipmentHistory.darkGray;
	}

	labelTextColor = FMGeoTrackingEquipmentHistory.SetRowColor(newRow, rowContentObj.MessageState, labelTextColor);

	FMGeoTrackingEquipmentHistory.expandRowIdList.push(expandRowId);
	FMGeoTrackingEquipmentHistory.expandRowStateList.push("collapse");

	// Create the 8 columns "<td>" for the new table row.
	var deviceIdCell	= newRow.insertCell(0);
	var selectIconCell	= newRow.insertCell(1);
	var expandIconCell	= newRow.insertCell(2);
	var gpsCell			= newRow.insertCell(3);
	var timestampCell	= newRow.insertCell(4);
	var productCell		= newRow.insertCell(5);
	var volumeCell		= newRow.insertCell(6);
	var waterCell		= newRow.insertCell(7);
	var densityCell		= newRow.insertCell(8);
	var dielectricCell	= newRow.insertCell(9);
	var remarksCell		= newRow.insertCell(10);

	deviceIdCell.classList.add("equipmentHistoryColumn1");
	selectIconCell.classList.add("equipmentHistoryColumn2");
	expandIconCell.classList.add("equipmentHistoryColumn3");
	gpsCell.classList.add("equipmentHistoryColumn4");
	timestampCell.classList.add("equipmentHistoryColumn5");
	productCell.classList.add("equipmentHistoryColumn6");
	volumeCell.classList.add("equipmentHistoryColumn7");
	waterCell.classList.add("equipmentHistoryColumn8");
	densityCell.classList.add("equipmentHistoryColumn9");
	dielectricCell.classList.add("equipmentHistoryColumn10");
	remarksCell.classList.add("equipmentHistoryColumn11");

	var deviceIdValue = rowContentObj.AssetTrackingDeviceId;

	// Add the radio active symbol to the device ID when contaminated.
	if (rowContentObj.IsContaminated)
	{
		deviceIdValue = deviceIdValue + " " + String.fromCharCode(FMGeoTrackingEquipmentHistory.radioActiveSymbolCode);
	}

	var rowSelectionImage = FMGeoTrackingEquipmentHistory.GetImagesPath() + "/select.gif";
	var isRowSelectable = FMGeoTrackingEquipmentHistory.IsRowSelectable(rowContentObj.AssetTrackingDetailGuidStr);

	if (FMGeoTrackingEquipmentHistory.equipmentHistoryModel.FoundInvestigateState
		|| FMGeoTrackingEquipmentHistory.equipmentHistoryModel.HasStartInvestigateRight === false
		|| isRowSelectable === false)
	{
		rowSelectionImage = FMGeoTrackingEquipmentHistory.GetImagesPath() + "/unselect.gif";
	}

	// Create the asset tracking device ID label.
	var newLabel = document.createElement("label");
	newLabel.innerHTML = deviceIdValue;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = "DeviceIdLabel_" + rowContentObj.AssetTrackingDetailGuidStr;
	deviceIdCell.appendChild(newLabel);

	// Create selection Icon
	var selectButtonId = "SelectBtn_" + rowContentObj.AssetTrackingDetailGuidStr;
	var selectButtonImageId = "SelectBtnImg_" + rowContentObj.AssetTrackingDetailGuidStr;
	var newSelectButton = document.createElement("button");
	newSelectButton.style.border = "none";
	newSelectButton.style.background = "none";
	newSelectButton.setAttribute("onclick", "FMGeoTrackingEquipmentHistory.HandleRowSelectionEvent('" + newRow.id + "')");
	newSelectButton.id = selectButtonId;
	selectIconCell.appendChild(newSelectButton);

	var newSelectImage = document.createElement("img");
	newSelectImage.src = rowSelectionImage;
	newSelectImage.width = "30";
	newSelectImage.height = "15";
	newSelectImage.id = selectButtonImageId;
	newSelectButton.appendChild(newSelectImage);

	// If there is more than compartment, then create an image tag
	// with an expansion icon.  If not, then create an empty label tag.
	if (rowContentObj.HasExpansion === true)
	{
		var newButton = document.createElement("button");
		newButton.style.border = "none";
		newButton.style.background = "none";
		newButton.setAttribute("onclick", "FMGeoTrackingEquipmentHistory.ExpandCollapseRow('" + expandRowId + "')");
		expandIconCell.appendChild(newButton);

		var newImage = document.createElement("img");
		newImage.src = FMGeoTrackingEquipmentHistory.GetImagesPath() + "/Arrow-Up.png";
		newImage.width = "15";
		newImage.height = "15";
		newImage.id = expandRowId + "-Img";
		newButton.appendChild(newImage);

		expandIconCell.style.paddingRight = "10px";
		expandIconCell.style.paddingLeft = "0px";
	}
	else
	{
		newLabel = document.createElement("label");
		newLabel.innerHTML = "";
		newLabel.style.fontSize = labelRowFontSize;
		expandIconCell.appendChild(newLabel);
	}

	// Create the GPS coordinate label.
	newLabel = document.createElement("label");
	newLabel.innerHTML = rowContentObj.GpsCoordinatesStr;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = "GpsLabel_" + rowContentObj.AssetTrackingDetailGuidStr;
	gpsCell.appendChild(newLabel);

	// Create the Message Timestamp label.
	newLabel = document.createElement("label");
	newLabel.innerHTML = rowContentObj.SessionDatetimeStr;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = "MessageTimestampLabel_" + rowContentObj.AssetTrackingDetailGuidStr;
	timestampCell.appendChild(newLabel);

	// Create the Product label.
	newLabel = document.createElement("label");
	newLabel.innerHTML = rowContentObj.ProductId;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = "ProductLabel_" + rowContentObj.AssetTrackingDetailGuidStr;
	productCell.appendChild(newLabel);

	// Create the Volume label.
	newLabel = document.createElement("label");
	newLabel.innerHTML = rowContentObj.VolumeStr;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = "VolumeLabel_" + rowContentObj.AssetTrackingDetailGuidStr;
	volumeCell.appendChild(newLabel);

	// Create the Water label.
	newLabel = document.createElement("label");
	newLabel.innerHTML = rowContentObj.WaterStr;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = "WaterLabel_" + rowContentObj.AssetTrackingDetailGuidStr;
	waterCell.appendChild(newLabel);

	// Create the Density label.
	newLabel = document.createElement("label");
	newLabel.innerHTML = rowContentObj.DensityStr;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = "DensityLabel_" + rowContentObj.AssetTrackingDetailGuidStr;
	densityCell.appendChild(newLabel);

	// Create the Dielectric label.
	newLabel = document.createElement("label");
	newLabel.innerHTML = rowContentObj.DielectricStr;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = "DielectricLabel_" + rowContentObj.AssetTrackingDetailGuidStr;
	dielectricCell.appendChild(newLabel);

	// Create the Remarks textbox.
	newLabel = document.createElement("label");
	newLabel.innerHTML = FMGeoTrackingEquipmentHistory.TruncateRemarks(rowContentObj.Remarks);
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = "RemarksLabel_" + rowContentObj.AssetTrackingDetailGuidStr;
	remarksCell.appendChild(newLabel);

	if (rowContentObj.Remarks != null && rowContentObj.Remarks !== "")
	{
		remarksCell.ondblclick = function ()
		{
			FMGeoTrackingEquipmentHistory.HandleRemarksDoubleClick(rowContentObj.AssetTrackingDetailGuidStr, rowContentObj.Remarks);
		};
	}
}

//===========================================================================
// This function will truncate the remarks fields to 12 characters plus the
// elipse.
//===========================================================================
FMGeoTrackingEquipmentHistory.TruncateRemarks = function(remarks)
{
	if (remarks == null)
	{
		return remarks;
	}

	if (remarks.length > 15)
	{
		var truncRemarks = remarks.substring(0, 12) + "...";
		return truncRemarks;
	}

	return remarks;
}

//====================================================================================
// This function will create a hidden compartment row in the equipment history 
// table dynamically.
//====================================================================================
FMGeoTrackingEquipmentHistory.CreateEquipmentHistoryHiddenRow = function (	inCompartmentName,
																			compartmentVolume,
																			waterContent,
																			hiddenRowId,
																			setBackground,
																			contaminatedFlag,
																			compartmentDielectric,
																			messageState)
	
{
	var labelRowFontSize = "10px";
	var labelRowPaddingLeft = "10px";
	var labelTextColor = FMGeoTrackingEquipmentHistory.darkGray;

	var equipmentTable = document.getElementById("EquipmentHistoryTable");
	var length = equipmentTable.rows.length;

	// Create a new html table row "<tr>" at the bottom of the table.
	var row = equipmentTable.insertRow(length);
	row.id = hiddenRowId;
	row.style.display = "none";
	row.setAttribute("DateFilter", "none");

	if (setBackground)
	{
		row.style.background = FMGeoTrackingEquipmentHistory.lightGray;
		labelTextColor = FMGeoTrackingEquipmentHistory.darkGray;
	}

	var compartmentName = inCompartmentName;
	if (contaminatedFlag)
	{
		compartmentName = compartmentName + " " + String.fromCharCode(FMGeoTrackingEquipmentHistory.radioActiveSymbolCode);
	}

	labelTextColor = FMGeoTrackingEquipmentHistory.SetRowColor(row, messageState, labelTextColor);

	// Create the 8 columns "<td>" for the new table row.
	var deviceIdCell	= row.insertCell(0);
	var selectIconCell	= row.insertCell(1);
	var expandIconCell	= row.insertCell(2);
	var gpsCell			= row.insertCell(3);
	var timestampCell	= row.insertCell(4);
	var productCell		= row.insertCell(5);
	var volumeCell		= row.insertCell(6);
	var waterCell		= row.insertCell(7);
	var densityCell		= row.insertCell(8);
	var dielectricCell	= row.insertCell(9);
	var remarksCell		= row.insertCell(10);

	deviceIdCell.classList.add("equipmentHistoryColumn1");
	selectIconCell.classList.add("equipmentHistoryColumn2");
	expandIconCell.classList.add("equipmentHistoryColumn3");
	gpsCell.classList.add("equipmentHistoryColumn4");
	timestampCell.classList.add("equipmentHistoryColumn6");
	productCell.classList.add("equipmentHistoryColumn7");
	volumeCell.classList.add("equipmentHistoryColumn7");
	waterCell.classList.add("equipmentHistoryColumn8");
	densityCell.classList.add("equipmentHistoryColumn9");
	dielectricCell.classList.add("equipmentHistoryColumn10");
	remarksCell.classList.add("equipmentHistoryColumn11");

	var newLabel = document.createElement("label");
	newLabel.innerHTML = compartmentName;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = 'DeviceIdLabel_' + hiddenRowId;
	deviceIdCell.style.paddingLeft = labelRowPaddingLeft;
	deviceIdCell.appendChild(newLabel);

	newLabel = document.createElement("label");
	newLabel.innerHTML = "";
	newLabel.style.fontSize = labelRowFontSize;
	expandIconCell.appendChild(newLabel);

	newLabel = document.createElement("label");
	newLabel.innerHTML = "";
	newLabel.style.fontSize = labelRowFontSize;
	gpsCell.appendChild(newLabel);

	newLabel = document.createElement("label");
	newLabel.innerHTML = "";
	newLabel.style.fontSize = labelRowFontSize;
	timestampCell.appendChild(newLabel);

	newLabel = document.createElement("label");
	newLabel.innerHTML = "";
	newLabel.style.fontSize = labelRowFontSize;
	productCell.appendChild(newLabel);

	newLabel = document.createElement("label");
	newLabel.innerHTML = compartmentVolume;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = 'VolumeLabel_' + hiddenRowId;
	volumeCell.appendChild(newLabel);

	newLabel = document.createElement("label");
	newLabel.innerHTML = waterContent;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = 'WaterLabel_' + hiddenRowId;
	waterCell.appendChild(newLabel);

	newLabel = document.createElement("label");
	newLabel.innerHTML = "";
	newLabel.style.fontSize = labelRowFontSize;
	densityCell.appendChild(newLabel);

	newLabel = document.createElement("label");
	newLabel.innerHTML = compartmentDielectric;
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	newLabel.id = 'DielectricLabel_' + hiddenRowId;
	dielectricCell.appendChild(newLabel);

	newLabel = document.createElement("label");
	newLabel.innerHTML = "";
	newLabel.style.fontSize = labelRowFontSize;
	newLabel.style.color = labelTextColor;
	remarksCell.appendChild(newLabel);
}

//=========================================================================
// This function will handle the row expand/collapse event.
//=========================================================================
FMGeoTrackingEquipmentHistory.ExpandCollapseRow = function (rowId)
{
	var state = "collapse";

	// Find the current state of the row and toggle it.
	for (var nextRowIndex = 0; nextRowIndex < FMGeoTrackingEquipmentHistory.expandRowIdList.length; nextRowIndex++)
	{
		if (rowId === FMGeoTrackingEquipmentHistory.expandRowIdList[nextRowIndex])
		{
			if ("collapse" === FMGeoTrackingEquipmentHistory.expandRowStateList[nextRowIndex])
			{
				state = "expand";
				FMGeoTrackingEquipmentHistory.expandRowStateList[nextRowIndex] = state;
				break;
			}
			else if ("expand" === FMGeoTrackingEquipmentHistory.expandRowStateList[nextRowIndex])
			{
				state = "collapse";
				FMGeoTrackingEquipmentHistory.expandRowStateList[nextRowIndex] = state;
				break;
			}
		}
	}

	// Change the image arrow to either an Up or Down arrow based on the state.
	var imageExpandCollapseId = rowId + "-Img";
	var imageElement = document.getElementById(imageExpandCollapseId);

	if (imageElement != null)
	{
		if (state === "collapse")
		{
			imageElement.src = FMGeoTrackingEquipmentHistory.GetImagesPath() + "/Arrow-Up.png";
		}
		else
		{
			imageElement.src = FMGeoTrackingEquipmentHistory.GetImagesPath() + "/Arrow-down.png";
		}
	}

	// Expand or collapse the main row based on the state.
	for (var nextIndex = 1; nextIndex < 5; nextIndex++)
	{
		var compartmentRowId = rowId + "-C" + nextIndex;
		var compartmentRowToExpandOrCollapse = document.getElementById(compartmentRowId);

		if (compartmentRowToExpandOrCollapse == null)
		{
			break;
		}
		else
		{
			if (state === "collapse")
			{
				compartmentRowToExpandOrCollapse.style.display = "none";
			}
			else
			{
				compartmentRowToExpandOrCollapse.style.display = "";
			}
		}
	}
}

//====================================================================================
// This function will handle the period filter on change event.  It will filter
// the equipment history table based on the selection.
//====================================================================================
FMGeoTrackingEquipmentHistory.PeriodDropdownOnChange = function ()
{
	var all = "-88";
	var ignore = "-99";

	var periodDropdownElement = document.getElementById("PeriodDropdown");

	if (periodDropdownElement != null)
	{
		var selectedValue = periodDropdownElement.options[periodDropdownElement.selectedIndex].value;

		// Ingore setting, the user is filtering on date range.  The user shall not select
		// the "Blank" selection, it is only used for when the date filters are selected.
		// Revert back to the previous selection.
		if (selectedValue === ignore)
		{
			periodDropdownElement.selectedIndex = FMGeoTrackingEquipmentHistory.previousBreadcrumbSelection;
			return;
		}

		FMGeoTrackingEquipmentHistory.previousBreadcrumbSelection = periodDropdownElement.selectedIndex;
		var breadcrumbState;

		if (selectedValue === all)
		{
			breadcrumbState = "All";
		}
		else if (selectedValue === "0")
		{
			breadcrumbState = "Current";
		}
		else
		{
			breadcrumbState = selectedValue;
		}

		FMGeoTrackingEquipmentHistory.RefreshEquipmentHistoryGrid("", "", breadcrumbState);
	}

	// Clear the Date filters
	$("#EquipHistoryStartDate").val("");
	$("#EquipHistoryEndDate").val("");
}

//===============================================================================
// This function will handle the start date picker on select event. It will
// clear out the period dropdown, check to see if there is an end date and that
// the end date is less current than the start date, and filter the list.
//===============================================================================
FMGeoTrackingEquipmentHistory.StartDateOnSelect = function (startDateStr, obj)
{
	// Clear the period dropdown.
	var periodDropdownElement = document.getElementById("PeriodDropdown");
	periodDropdownElement.selectedIndex = 0;

	var endDateValueStr = $("#EquipHistoryEndDate").val();

	// If there is not end date, then we are not filtering.
	if (typeof (endDateValueStr) === "undefined" || endDateValueStr === "")
	{
		return;
	}

	var startDateValue = new Date(startDateStr);
	var endDateValue = new Date(endDateValueStr);

	// The start date must be more current than the end date.
	if (startDateValue < endDateValue)
	{
		alert("Start date must be more current than the end date.");
		return;
	}

	// Filter list.
	FMGeoTrackingEquipmentHistory.RefreshEquipmentHistoryGrid(startDateValue, endDateValue, "");
}

//===============================================================================
// This function will handle the end date picker on select event. It will
// clear out the period dropdown, check to see if there is a start date and that
// the start date is more current than the end date, and filter the list.
//===============================================================================
FMGeoTrackingEquipmentHistory.EndDateOnSelect = function (endDateStr, obj)
{
	// Clear the period dropdown.
	var periodDropdownElement = document.getElementById("PeriodDropdown");
	periodDropdownElement.selectedIndex = 0;

	var startDateValueStr = $("#EquipHistoryStartDate").val();

	// If there is not start date, then we are not filtering.
	if (typeof (startDateValueStr) === "undefined" || startDateValueStr === "")
	{
		return;
	}

	var startDateValue = new Date(startDateValueStr);
	var endDateValue = new Date(endDateStr);

	// The end date must be less current than the start date.
	if (startDateValue < endDateValue)
	{
		alert("End date must be less current than the start date.");
		return;
	}

	// Filter list.
	FMGeoTrackingEquipmentHistory.RefreshEquipmentHistoryGrid(startDateValue, endDateValue, "");
}

//===============================================================================
// This function will add all the options to the period dropdown.
//===============================================================================
FMGeoTrackingEquipmentHistory.CreatePeriodDropdown = function ()
{
	var periodDropdownElement = document.getElementById("PeriodDropdown");

	if (periodDropdownElement != null)
	{
		var newOption = new Option(" ", "-99");
		periodDropdownElement.appendChild(newOption);

		newOption = new Option("All", "-88");
		periodDropdownElement.appendChild(newOption);

		newOption = new Option("Current", "0");
		periodDropdownElement.appendChild(newOption);

		for (var nextOption = 1; nextOption < 61; nextOption++)
		{
			newOption = new Option(nextOption.toString(), nextOption.toString());
			periodDropdownElement.appendChild(newOption);
		}
	}
}

//================================================================
// This function will close the equipment history dialog.
//================================================================
FMGeoTrackingEquipmentHistory.CloseEquipmentHistory = function()
{
	// Clear the Period dropdown (set to blank selection)
	var periodDropdownElement = document.getElementById("PeriodDropdown");
	periodDropdownElement.selectedIndex = 0;

	// Clear the Date filters
	$("#EquipHistoryStartDate").val("");
	$("#EquipHistoryEndDate").val("");

	var equipmentTableDiv = document.getElementById("EquipmentHistoryDiv");
	equipmentTableDiv.style.display = "none";

	// This will force a mouse event on the map canvas forcing the popup to close.
	FMGeoTrackingEquipmentHistory.SimulateMouseEvent();
	FMGeoTrackingMap.HandleRefreshEvent(false);
}

//======================================================================
// This function will simulate a mouse event on the map canvas forcing
// the popup to close. There is an event already registered for the 
// popup feature to close on a mouse click on the map. Therefore,
// just fire the event.
//======================================================================
FMGeoTrackingEquipmentHistory.SimulateMouseEvent = function ()
{
	var eventType = "click";
	var canvasElement = document.getElementById("OpenLayers_Map_2_OpenLayers_ViewPort");

	if (canvasElement)
	{
		if (canvasElement.fireEvent)
		{
			canvasElement.fireEvent('on' + eventType);
		}
		else
		{
			var evObj = document.createEvent('Events');
			evObj.initEvent(eventType, true, false);
			canvasElement.dispatchEvent(evObj);
		}
	}
}

//================================================================
// This function will handles the start investigation button
// press.
//================================================================
FMGeoTrackingEquipmentHistory.StartInvestigationButtonEvent = function ()
{
	var results = FMGeoTrackingEquipmentHistory.ValidateSelections();

	if (results.Passed === false)
	{
		alert(results.ErrorMsg);
		return;
	}

	var startInvestigateModal = document.getElementById(FMGeoTrackingEquipmentHistory.startInvestigateModalDialog);
	startInvestigateModal.style.display = "block";
}

//================================================================
// This function will enable a button.
//================================================================
FMGeoTrackingEquipmentHistory.EnableButton = function (buttonId)
{
	$("#" + buttonId).removeAttr("disabled");
	$("#" + buttonId).removeClass("buttonDisabled");
	$("#" + buttonId).addClass("buttonEnabled");
}

//================================================================
// This function will disable a button.
//================================================================
FMGeoTrackingEquipmentHistory.DisableButton = function (buttonId)
{
	$("#" + buttonId).attr("disabled", "disabled");
	$("#" + buttonId).removeClass("buttonEnabled");
	$("#" + buttonId).addClass("buttonDisabled");
}

//=========================================================================================
// This function will handle the complete investigate text area on blur event. It will
// enable the complete investigate Ok button if data was entered into the text area.
//=========================================================================================
FMGeoTrackingEquipmentHistory.HandleCompleteInvestigateTextareaOnBlurEvent = function ()
{
	FMGeoTrackingEquipmentHistory.DisableButton("CompleteInvestigateOkBtn");
	var investigateTextareaValue = $("#CompleteInvestigateRemarksTextarea").val();

	if (investigateTextareaValue != null && investigateTextareaValue !== "")
	{
		FMGeoTrackingEquipmentHistory.EnableButton("CompleteInvestigateOkBtn");
	}
}

//================================================================
// This function will handles the complete investigation button
// press.
//================================================================
FMGeoTrackingEquipmentHistory.CompleteInvestigationButtonEvent = function ()
{
	var completeInvestigateModal = document.getElementById(FMGeoTrackingEquipmentHistory.completeInvestigateModalDialog);
	completeInvestigateModal.style.display = "block";

	// Initialize the dialog box radio buttons.
	FMGeoTrackingEquipmentHistory.InitializeCompleteInvestigateRadioBtns();

	var investigateTextareaValue = $("#CompleteInvestigateRemarksTextarea").val();
	if (investigateTextareaValue == null || investigateTextareaValue === "")
	{
		FMGeoTrackingEquipmentHistory.DisableButton("CompleteInvestigateOkBtn");
	}
}

//================================================================
// This function will handles the complete investigation confirm 
// button press.
//================================================================
FMGeoTrackingEquipmentHistory.CompleteInvestigateOkButtonEvent = function ()
{
	var remarks = $("#CompleteInvestigateRemarksTextarea").val();
	var completeState = FMGeoTrackingEquipmentHistory.GetCompleteInvestigateRadioBtnState();
	var equipRowRecord = FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList[0];
	var deviceId = equipRowRecord.AssetTrackingDeviceId;

	var updateResults = "";
	var updateDetailInvestigateCompleteUrl = "UpdateRecordsToCompleteInvestigationStateJson";

	var token = $('#MapBaseForm input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	$.ajax({
		cache: false,
		type: "POST",
		//async: false,
		url: updateDetailInvestigateCompleteUrl,
		headers: headers,
		dataType: "json",
		data: { deviceId: deviceId, investigateState: completeState, remarks: remarks },
		success: function (updateResult)
		{
			updateResults = updateResult;
			//FMGeoTrackingEquipmentHistory.RefreshHelper();
			FMGeoTrackingEquipmentHistory.UpdateRowsForCompleteInvestigateState(completeState);
			$("#CompleteInvestigateRemarksTextarea").val("");

			if (updateResults.length > 0)
			{
				alert(updateResults);
			}
		},
		error: function ()
		{
			$("#CompleteInvestigateRemarksTextarea").val("");
			alert("Error updating investigate complete.");
		}
	});

	$("#CompleteInvestigateDiv").hide();
	FMGeoTrackingEquipmentHistory.DisableButton(FMGeoTrackingEquipmentHistory.completeInvestigateBtnId);
}

//==============================================================================================
// This function will update the rows to a completed investigation state. It will set the color
// of the rows, the selection enable/disable, and the model with the correct state.
//==============================================================================================
FMGeoTrackingEquipmentHistory.UpdateRowsForCompleteInvestigateState = function (messageState)
{
	FMGeoTrackingEquipmentHistory.equipmentHistoryModel.FoundInvestigateState = false;

	// Clear the selection row array.
	FMGeoTrackingEquipmentHistory.selectionRecordList = [];

	// Set all the selection icons to enable select.
	$("#EquipmentHistoryTable tr td button img").each(
		function ()
		{
			var parts = this.id.split("_");
			if (parts[0] === "SelectBtnImg")
			{
				this.src = FMGeoTrackingEquipmentHistory.GetImagesPath() + "/select.gif";
			}
		});

	var firstInvRecIndex = -1;
	var lastContaminateRecIndex = -1;

	$("#EquipmentHistoryTable tr").each(
		function ()
		{
			var parts = this.id.split("_");

			if (parts == null || parts.length !== 2)
			{
				return;
			}

			var rowId = parts[0];
			var rowGuidStr = parts[1];

			for (var nextRecIndex = 0; nextRecIndex < FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList.length; nextRecIndex++)
			{
				var equipRec = FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList[nextRecIndex];

				if (equipRec.MessageState === FMGeoTrackingEquipmentHistory.messageStateEnum.Investigate && equipRec.AssetTrackingDetailGuidStr === rowGuidStr)
				{
					equipRec.MessageState = messageState;

					if (firstInvRecIndex === -1)
					{
						firstInvRecIndex = nextRecIndex;
					}

					var labelTextColor = FMGeoTrackingEquipmentHistory.SetRowColor(this, messageState, FMGeoTrackingEquipmentHistory.darkGray);
					FMGeoTrackingEquipmentHistory.SetLabelTextColor(labelTextColor, rowGuidStr);

					// Set any compartment row color to the same as the parent.
					FMGeoTrackingEquipmentHistory.SetCompartmentRowColor(rowId, FMGeoTrackingEquipmentHistory.messageStateEnum.Investigate, labelTextColor);
					FMGeoTrackingEquipmentHistory.SetCompartmentLabelTextColor(rowId, labelTextColor);
				}

				// Set all completed investigation rows to be unselectable.
				if (equipRec.MessageState === FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedPassed
					|| equipRec.MessageState === FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedFailed)
				{
					var selectBtnImageId = "SelectBtnImg_" + equipRec.AssetTrackingDetailGuidStr;
					var unselectImageUrl = FMGeoTrackingEquipmentHistory.GetImagesPath() + "/unselect.gif";
					$("#" + selectBtnImageId).attr("src", unselectImageUrl);
				}

				if (equipRec.MessageState !== FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedPassed
					&& equipRec.MessageState !== FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedFailed
					&& equipRec.MessageState !== FMGeoTrackingEquipmentHistory.messageStateEnum.Investigate
					&& equipRec.IsContaminated)
				{
					lastContaminateRecIndex = nextRecIndex;
				}

			}
		});

	// If there was a contaminated record that was not investigated or complete investigated
	// and that record is before the one being completed, then we want the rows above the 
	// completed record to remain orange.  Therefore, set the first investigate record
	// to zero so that we can ignore changing row colors to a normal setting.
	if (lastContaminateRecIndex !== -1 && lastContaminateRecIndex > firstInvRecIndex)
	{
		firstInvRecIndex = 0;
	}

	// Set index to the row before the first row that completed the investigation.
	firstInvRecIndex--;

	if (firstInvRecIndex > -1)
	{
		// Set rows above the completed investigation to normal color if it is not contaminated.
		// Stop once a contaminated row is found.
		for (var nextRecIndex = firstInvRecIndex; nextRecIndex >= 0; nextRecIndex--)
		{
			var equipRec = FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList[nextRecIndex];

			// Stop when the row is contaminated and is not under investigation or completed investigation state.
			if (equipRec.MessageState !== FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedPassed
				&& equipRec.MessageState !== FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedFailed
				&& equipRec.MessageState !== FMGeoTrackingEquipmentHistory.messageStateEnum.Investigate
				&& equipRec.IsContaminated)
			{
				break;
			}

			var rowId = "ExpandRow" + (nextRecIndex + 1);
			var rowKey = rowId + "_" + equipRec.AssetTrackingDetailGuidStr;
			var rowObj = document.getElementById(rowKey);

			// Set to normal color.
			var labelTextColor = FMGeoTrackingEquipmentHistory.SetRowColor(rowObj, FMGeoTrackingEquipmentHistory.messageStateEnum.Normal, FMGeoTrackingEquipmentHistory.darkGray);
			FMGeoTrackingEquipmentHistory.SetLabelTextColor(labelTextColor, equipRec.AssetTrackingDetailGuidStr);

			// Set any compartment row color to the same as the parent.
			FMGeoTrackingEquipmentHistory.SetCompartmentRowColor(rowId, FMGeoTrackingEquipmentHistory.messageStateEnum.Normal, labelTextColor);
			FMGeoTrackingEquipmentHistory.SetCompartmentLabelTextColor(rowId, labelTextColor);
		}
	}
}

//================================================================
// This function will handles the complete investigation 
// cancel button press.
//================================================================
FMGeoTrackingEquipmentHistory.CompleteInvestigateCancelButtonEvent = function ()
{
	$("#CompleteInvestigateDiv").hide();
	$("#CompleteInvestigateRemarksTextarea").val("");
}

//================================================================
// This function will get the URL path for the "images".
//================================================================
FMGeoTrackingEquipmentHistory.GetImagesPath = function ()
{
	if (FMGeoTrackingEquipmentHistory.imageRootPath == null || FMGeoTrackingEquipmentHistory.imageRootPath === "")
	{
		FMGeoTrackingEquipmentHistory.imageRootPath = "FuelsManager/Areas/images/AssetMapImages";
	}

	var protocol = window.location.protocol;
	var host = window.location.host;
	var sourcePath = protocol + "//" + host + "/" + FMGeoTrackingEquipmentHistory.imageRootPath;

	return sourcePath;
}

//=================================================================================
// This function will add equipment history records to the equipment history grid.
//=================================================================================
FMGeoTrackingEquipmentHistory.AddEquipmentRecordRows = function ()
{
	// Remove all rows in the equipment history.
	FMGeoTrackingEquipmentHistory.ClearEquipmentHistoryTableRows();

	if (typeof (FMGeoTrackingEquipmentHistory.equipmentHistoryModel) === "undefined")
	{
		return;
	}

	var toggleBackGround = true;

	for (var nextEquipHistoryIndex = 0; nextEquipHistoryIndex < FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList.length; nextEquipHistoryIndex++)
	{
		var equipmentRecord = FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList[nextEquipHistoryIndex];

		var expandRowId = "ExpandRow" + (nextEquipHistoryIndex + 1);
		FMGeoTrackingEquipmentHistory.CreateEquipmentHistoryRow(equipmentRecord, expandRowId, toggleBackGround);

		for (var nextCompHistoryIndex = 0; nextCompHistoryIndex < equipmentRecord.CompartmentRecordList.length; nextCompHistoryIndex++)
		{
			var compartmentRecordRow	= equipmentRecord.CompartmentRecordList[nextCompHistoryIndex];
			var compartmentName			= compartmentRecordRow.AssetTrackingDeviceId;
			var compartmentVolume		= compartmentRecordRow.VolumeStr;
			var waterContent			= compartmentRecordRow.WaterStr;
			var contamintedFlag			= compartmentRecordRow.IsContaminated;
			var hiddenRowId				= expandRowId + "-C" + (nextCompHistoryIndex + 1);
			var compartmentDielectric	= compartmentRecordRow.DielectricStr;
			var messageState			= equipmentRecord.MessageState;

			FMGeoTrackingEquipmentHistory.CreateEquipmentHistoryHiddenRow(	compartmentName,
																			compartmentVolume,
																			waterContent,
																			hiddenRowId,
																			toggleBackGround,
																			contamintedFlag,
																			compartmentDielectric,
																			messageState);
		}

		if (toggleBackGround)
		{
			toggleBackGround = false;
		}
		else
		{
			toggleBackGround = true;
		}
	}

	// In the case there are rows in an investigate state, enable the Complete Investigation button.
	if (FMGeoTrackingEquipmentHistory.equipmentHistoryModel.FoundInvestigateState &&
		FMGeoTrackingEquipmentHistory.equipmentHistoryModel.HasCompleteInvestigateRight)
	{
		FMGeoTrackingEquipmentHistory.EnableButton(FMGeoTrackingEquipmentHistory.completeInvestigateBtnId);
	}
}

//=================================================================================
// This function will retrieve equipment history data from the server.  It returns
// a list of equipment history records.
//=================================================================================
FMGeoTrackingEquipmentHistory.RetrieveHistoryDataFromServer = function (vehicleId, fromDateStr, endDateStr, breadcrumbSlectionStr)
{
	var equipmentHistoryDataUrl = "GetEquipmentHistoryDataJson";
	FMGeoTrackingEquipmentHistory.equipmentHistoryModel = null;

	var token = $('#MapBaseForm input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// Get the requested vehicle history information.
	$.ajax({
		cache: false,
		type: "POST",
		async: false,
		url: equipmentHistoryDataUrl,
		headers: headers,
		dataType: "json",
		data: { equipmentId: vehicleId, fromDate: fromDateStr, endDate: endDateStr, breadcrumbSelection: breadcrumbSlectionStr },
		success: function (equipHistoryModel)
		{
			if (equipHistoryModel)
			{
				FMGeoTrackingEquipmentHistory.equipmentHistoryModel = equipHistoryModel;

				FMGeoTrackingEquipmentHistory.DisableButton(FMGeoTrackingEquipmentHistory.startInvestigateBtnId);
				FMGeoTrackingEquipmentHistory.DisableButton(FMGeoTrackingEquipmentHistory.completeInvestigateBtnId);

				if (FMGeoTrackingEquipmentHistory.equipmentHistoryModel)
				{
					FMGeoTrackingEquipmentHistory.hasStartInvestigateRight = FMGeoTrackingEquipmentHistory.equipmentHistoryModel.HasStartInvestigateRight;
					FMGeoTrackingEquipmentHistory.hasCompleteInvestigateRight = FMGeoTrackingEquipmentHistory.equipmentHistoryModel.HasCompleteInvestigateRight;
				}
			}
			else
			{
				alert("Error retrieve history data.");
			}
		},
		error: function() {
			alert("Error retrieve history data.");
		}
	});
}

//=================================================================================
// This function will refresh the equipment history grid based on the filter
// selections.
//=================================================================================
FMGeoTrackingEquipmentHistory.RefreshEquipmentHistoryGrid = function (fromDate, endDate, breadcrumbState)
{
	var vehicleId = "";
	var vehicleIdElement = document.getElementById("EquipmentIdFilterLabel");

	if (vehicleIdElement != null)
	{
		vehicleId = vehicleIdElement.innerHTML;
	}

	var fromDateStr = "";
	var endDateStr = "";

	if (breadcrumbState === "")
	{
		fromDateStr = FMGeoTrackingEquipmentHistory.ConvertDateToString(fromDate);
		endDateStr = FMGeoTrackingEquipmentHistory.ConvertDateToString(endDate);
	}

	FMGeoTrackingEquipmentHistory.RetrieveHistoryDataFromServer(vehicleId, fromDateStr, endDateStr, breadcrumbState);
	FMGeoTrackingEquipmentHistory.AddEquipmentRecordRows();

	// Clear the selection record collection;
	FMGeoTrackingEquipmentHistory.selectionRecordList = [];
}

//=================================================================================
// This function will convert a date to the following string format: yyyy/mm/dd
//=================================================================================
FMGeoTrackingEquipmentHistory.ConvertDateToString = function (dateToConvert)
{
	var yyyy = dateToConvert.getFullYear();
	var mm = dateToConvert.getMonth() + 1;
	var dd = dateToConvert.getDate();

	var ddStr = dd.toString();
	var mmStr = mm.toString();

	if (dd < 10)
	{
		ddStr = "0" + ddStr;
	}

	if (mm < 10)
	{
		mmStr = "0" + mmStr;
	}

	var strDate = yyyy + "/" + mmStr + "/" + ddStr;
	return strDate;
}

//========================================================================
// This method will remove all the rows in the equipment history table.
//========================================================================
FMGeoTrackingEquipmentHistory.ClearEquipmentHistoryTableRows = function ()
{
	$("#EquipmentHistoryTable").empty();
}

//================================================================================
// This function will handle the equipment history scroll down arrow event.
//================================================================================
FMGeoTrackingEquipmentHistory.ScrollDownEvent = function ()
{
	var scrollPosition = $("#EquipmentHistoryScrollDiv").scrollTop();
	var scrolled = scrollPosition + 21;
	$("#EquipmentHistoryScrollDiv").animate({scrollTop: scrolled});
}

//================================================================================
// This function will handle the equipment history scroll up arrow event.
//================================================================================
FMGeoTrackingEquipmentHistory.ScrollUpEvent = function ()
{
	var scrollPosition = $("#EquipmentHistoryScrollDiv").scrollTop();
	var scrolled = scrollPosition - 21;
	$("#EquipmentHistoryScrollDiv").animate({scrollTop: scrolled});
}

//=================================================================================
// This function will handle the investigate button on click event.
//=================================================================================
FMGeoTrackingEquipmentHistory.HandleStartInvestigateBtnOnClick = function (buttonId)
{
	var investigateModal = document.getElementById(FMGeoTrackingEquipmentHistory.startInvestigateModalDialog);

	if (buttonId === FMGeoTrackingEquipmentHistory.startInvestigationNoBtnId || buttonId === FMGeoTrackingEquipmentHistory.startInvestigateDialogXCloseId)
	{
		// Close the modal dialog.
		investigateModal.style.display = "none";
	}

	if (buttonId === FMGeoTrackingEquipmentHistory.startInvestigationYesBtnId)
	{
		var selectedRowGuids = [];

		for (var nextSelectIndex = 0; nextSelectIndex < FMGeoTrackingEquipmentHistory.selectionRecordList.length; nextSelectIndex++)
		{
			var selectedRow = FMGeoTrackingEquipmentHistory.selectionRecordList[nextSelectIndex];
			selectedRowGuids.push(selectedRow.RowGuidStr);
		}

		// Update the asset tracking detail record with an investigation state and date.
		var updateDetailInvestigateUrl = "UpdateRecordsToInvestigationStateJson";

		var token = $('#MapBaseForm input[name =__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		// Get the requested vehicle history information.
		$.ajax({
			cache: false,
			type: "POST",
			//async: true,
			url: updateDetailInvestigateUrl,
			headers: headers,
			dataType: "json",
			data: { selectedGuids: selectedRowGuids },
			success: function (message)
			{
				if (message != null && message !== "")
				{
					// Close the modal dialog.		
					investigateModal.style.display = "none";
					alert(message);
					return;
				}

				investigateModal.style.display = "none";
				FMGeoTrackingEquipmentHistory.MakeAllRowsUnselectable();
				FMGeoTrackingEquipmentHistory.EnableButton(FMGeoTrackingEquipmentHistory.completeInvestigateBtnId);
			},
			error: function (jqXhr, textStatus, errorThrown)
			{
				// Close the modal dialog.
				investigateModal.style.display = "none";

				var errorMsg = "Could not update the Investigation State and Date.";

				if (textStatus != null)
				{
					errorMsg = errorMsg + " Error type: " + textStatus;
				}

				if (errorThrown != null)
				{
					errorMsg = errorMsg + "; HTTP status: " + errorThrown;
				}
				alert(errorMsg);
			}
		});

		investigateModal.style.display = "none";
		FMGeoTrackingEquipmentHistory.DisableButton(FMGeoTrackingEquipmentHistory.startInvestigateBtnId);
	}
}

//======================================================================================
// This function will make all the rows unselectable. It will also mark the rows under
// investigation to INVESTIGATE in the model.
//======================================================================================
FMGeoTrackingEquipmentHistory.MakeAllRowsUnselectable = function ()
{
	FMGeoTrackingEquipmentHistory.equipmentHistoryModel.FoundInvestigateState = true;	

	if (FMGeoTrackingEquipmentHistory.selectionRecordList.length !== 0)
	{
		for (var nextRecIndex = 0; nextRecIndex < FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList.length; nextRecIndex++)
		{
			var equipRec = FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList[nextRecIndex];

			for (var nextSelectedRecIndex = 0; nextSelectedRecIndex < FMGeoTrackingEquipmentHistory.selectionRecordList.length; nextSelectedRecIndex++)
			{
				var selectionObj = FMGeoTrackingEquipmentHistory.selectionRecordList[nextSelectedRecIndex];

				if (equipRec.AssetTrackingDetailGuidStr === selectionObj.RowGuidStr)
				{
					equipRec.MessageState = FMGeoTrackingEquipmentHistory.messageStateEnum.Investigate;
					break;
				}
			}
		}
	}

	$("#EquipmentHistoryTable tr td button img").each(
		function ()
		{
			var parts = this.id.split("_");
			if (parts[0] === "SelectBtnImg")
			{
				this.src = FMGeoTrackingEquipmentHistory.GetImagesPath() + "/unselect.gif";
			}
		});
}

//=======================================================================
// This function is a refresh helper for the investigate events.
//=======================================================================
FMGeoTrackingEquipmentHistory.RefreshHelper = function ()
{
	var startDateValueStr = $("#EquipHistoryStartDate").val();
	var endDateValueStr = $("#EquipHistoryEndDate").val();
	var hasStartDate = true;
	var hasEndDate = true;

	// If there is not start date, then we are not filtering.
	if (typeof (startDateValueStr) === "undefined" || startDateValueStr === "")
	{
		hasStartDate = false;
	}

	// If there is not end date, then we are not filtering.
	if (typeof (endDateValueStr) === "undefined" || endDateValueStr === "")
	{
		hasEndDate = false;
	}

	if (hasStartDate && hasEndDate)
	{
		var startDateValue = new Date(startDateValueStr);
		var endDateValue = new Date(endDateValueStr);

		// Refresh the equipment history page.
		FMGeoTrackingEquipmentHistory.RefreshEquipmentHistoryGrid(startDateValue, endDateValue, "");
		return;
	}

	var periodDropdownElement = document.getElementById("PeriodDropdown");
	var all = "-88";
	var ignore = "-99";

	if (periodDropdownElement != null)
	{
		var selectedValue = periodDropdownElement.options[periodDropdownElement.selectedIndex].value;

		// Ingore setting, the user is filtering on date range.  The user shall not select
		// the "Blank" selection, it is only used for when the date filters are selected.
		// Revert back to the previous selection.
		if (selectedValue === ignore)
		{
			periodDropdownElement.selectedIndex = FMGeoTrackingEquipmentHistory.previousBreadcrumbSelection;
			return;
		}

		var breadcrumbState;

		if (selectedValue === all)
		{
			breadcrumbState = "All";
		}
		else if (selectedValue === "0")
		{
			breadcrumbState = "Current";
		}
		else
		{
			breadcrumbState = selectedValue;
		}

		FMGeoTrackingEquipmentHistory.RefreshEquipmentHistoryGrid("", "", breadcrumbState);
	}
}

//=============================================================================================
// This function will handle the remarks double click event to display the remarks modal
// dialog.
//=============================================================================================
FMGeoTrackingEquipmentHistory.HandleRemarksDoubleClick = function (assetTrackingDetailGuid, remarks)
{
	FMGeoTrackingEquipmentHistory.assetTrackingDetailGuidStr = assetTrackingDetailGuid;
	$("#RemarksModalTextarea").val(remarks);

	var remarksModal = document.getElementById(FMGeoTrackingEquipmentHistory.remarksModalDialogId);
	remarksModal.style.display = "block";
}

//=========================================================================
// This function will handle the remarks modal test area on blur event.
//=========================================================================
FMGeoTrackingEquipmentHistory.HandleRemarksTextareaOnBlurEvent = function()
{
	FMGeoTrackingEquipmentHistory.DisableButton("RemarksModalOkBtn");
	var remarksTextareaValue = $("#RemarksModalTextarea").val();

	if (remarksTextareaValue != null && remarksTextareaValue !== "")
	{
		FMGeoTrackingEquipmentHistory.EnableButton("RemarksModalOkBtn");
	}
}

//===========================================================================
// This function will handle the remarks modal Ok button event. It will
// update the remarks to the database.
//===========================================================================
FMGeoTrackingEquipmentHistory.HandleRemarksModalOkButtonEvent = function ()
{
	var remarksModal = document.getElementById(FMGeoTrackingEquipmentHistory.remarksModalDialogId);
	var remarks = $("#RemarksModalTextarea").val();
	$("#RemarksModalTextarea").val("");
	var assetTrackingDetailGuid = FMGeoTrackingEquipmentHistory.assetTrackingDetailGuidStr;

	FMGeoTrackingEquipmentHistory.assetTrackingDetailGuidStr = "";

	// Update the asset tracking detail record with a new remark.
	var updateDetailInvestigateUrl = "UpdateRemarksJson";

	var token = $('#MapBaseForm input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// Get the requested vehicle history information.
	$.ajax({
		cache: false,
		type: "POST",
		async: false,
		url: updateDetailInvestigateUrl,
		headers: headers,
		dataType: "json",
		data: { assetTrackingDetailGuid: assetTrackingDetailGuid, remarks: remarks },
		success: function (message)
		{
			if (message != null && message !== "")
			{
				// Close the modal dialog.		
				remarksModal.style.display = "none";
				alert(message);
				return;
			}

			remarksModal.style.display = "none";
			FMGeoTrackingEquipmentHistory.RefreshHelper();
		},
		error: function ()
		{
			// Close the modal dialog.
			remarksModal.style.display = "none";
			alert("Could not update the Remarks.");
		}
	});
}

//=============================================================================
// This function will handle the remarks modal cancel button event.
//=============================================================================
FMGeoTrackingEquipmentHistory.HandleRemarksModalCancelButtonEvent = function ()
{
	$("#" + FMGeoTrackingEquipmentHistory.remarksModalDialogId).hide();
	$("#RemarksModalTextarea").val("");
	FMGeoTrackingEquipmentHistory.assetTrackingDetailGuidStr = "";
}

//===================================================================================
// This function will handle a row selection event.
//===================================================================================
FMGeoTrackingEquipmentHistory.HandleRowSelectionEvent = function (selectedRow)
{
	if (FMGeoTrackingEquipmentHistory.equipmentHistoryModel)
	{
		// User cannot select rows if there is a record that is in investigate state or
		// the user does not have start investigate rights.
		if (FMGeoTrackingEquipmentHistory.equipmentHistoryModel.FoundInvestigateState
			|| FMGeoTrackingEquipmentHistory.equipmentHistoryModel.HasStartInvestigateRight === false)
		{
			return;
		}

		var parts = selectedRow.split("_");
		var rowId = parts[0];
		var rowGuid = parts[1];

		if (FMGeoTrackingEquipmentHistory.IsRowSelectable(rowGuid) === false)
		{
			return;
		}

		var selectedRowObj = document.getElementById(selectedRow);

		// The first selection needs to be a row that has a contamination.
		if (FMGeoTrackingEquipmentHistory.selectionRecordList.length === 0)
		{
			if (FMGeoTrackingEquipmentHistory.IsSelectedRecordContaminated(rowGuid) === false)
			{
				alert("The selected row must be contaminated.");
				return;
			}
		}

		var selectionRecordIndex = FMGeoTrackingEquipmentHistory.SelectionRecordExist(rowId);

		// The selected row has not been assigned, therefore assign it.
		if (selectionRecordIndex === -1)
		{
			var newSelectRecord = FMGeoTrackingEquipmentHistory.CreateSelectionRecord();
			newSelectRecord.RowId = rowId;
			newSelectRecord.RowGuidStr = rowGuid;
			newSelectRecord.OriginalRowColor = selectedRowObj.style.backgroundColor;
			newSelectRecord.OriginalLabelTextColor = FMGeoTrackingEquipmentHistory.GetOriginalLabelTextColor(rowGuid);
			newSelectRecord.RowNumber = parseInt(rowId.replace("ExpandRow", ""));

			if (FMGeoTrackingEquipmentHistory.selectionRecordList.length === 0)
			{
				newSelectRecord.InitialRowSelection = true;
			}

			FMGeoTrackingEquipmentHistory.selectionRecordList.push(newSelectRecord);
			FMGeoTrackingEquipmentHistory.ReorderSelectionRecordList();

			var labelTextColor = FMGeoTrackingEquipmentHistory.SetRowColor(selectedRowObj, FMGeoTrackingEquipmentHistory.messageStateEnum.Investigate, FMGeoTrackingEquipmentHistory.darkGray);
			FMGeoTrackingEquipmentHistory.SetLabelTextColor(labelTextColor, rowGuid);

			// Set any compartment row color to the same as the parent.
			FMGeoTrackingEquipmentHistory.SetCompartmentRowColor(rowId, FMGeoTrackingEquipmentHistory.messageStateEnum.Investigate, labelTextColor);
			FMGeoTrackingEquipmentHistory.SetCompartmentLabelTextColor(rowId, labelTextColor);

			FMGeoTrackingEquipmentHistory.EnableButton(FMGeoTrackingEquipmentHistory.startInvestigateBtnId);

			return;
		}

		var origRowId;
		var rowObj;

		// Index of zero means that was the initial row that was selected and was contaminated. Since it
		// exists, then this means the user is unselecting it.  All selections are removed.
		if (FMGeoTrackingEquipmentHistory.selectionRecordList[selectionRecordIndex].InitialRowSelection)
		{		
			for (var nextIndex = 0; nextIndex < FMGeoTrackingEquipmentHistory.selectionRecordList.length; nextIndex++) 
			{
				var selectedItem = FMGeoTrackingEquipmentHistory.selectionRecordList[nextIndex];
				origRowId = selectedItem.RowId + "_" + selectedItem.RowGuidStr;
				rowObj = document.getElementById(origRowId);

				// Reset the original row un selected colors.
				rowObj.style.background = selectedItem.OriginalRowColor;
				FMGeoTrackingEquipmentHistory.SetLabelTextColor(selectedItem.OriginalLabelTextColor, selectedItem.RowGuidStr);

				// Reset the compartment row color based on the parent's setting.
				FMGeoTrackingEquipmentHistory.ResetCompartmentRowColor(selectedItem.RowId, selectedItem.OriginalRowColor);
				FMGeoTrackingEquipmentHistory.SetCompartmentLabelTextColor(selectedItem.RowId, selectedItem.OriginalLabelTextColor);
			}

			// Clear all selections since this was the first row that was selected and was contaminated.
			FMGeoTrackingEquipmentHistory.selectionRecordList = [];

			FMGeoTrackingEquipmentHistory.DisableButton(FMGeoTrackingEquipmentHistory.startInvestigateBtnId);

			return;
		}

		// Since the index is not the first one selected, then remove the selection.
		var removeRecord = FMGeoTrackingEquipmentHistory.selectionRecordList[selectionRecordIndex];
		origRowId = removeRecord.RowId + "_" + removeRecord.RowGuidStr;
		rowObj = document.getElementById(origRowId);

		// Reset the original row un selected colors.
		rowObj.style.background = removeRecord.OriginalRowColor;
		FMGeoTrackingEquipmentHistory.SetLabelTextColor(removeRecord.OriginalLabelTextColor, removeRecord.RowGuidStr);

		// Reset the compartment row color based on the parent's setting.
		FMGeoTrackingEquipmentHistory.ResetCompartmentRowColor(removeRecord.RowId, removeRecord.OriginalRowColor);
		FMGeoTrackingEquipmentHistory.SetCompartmentLabelTextColor(removeRecord.RowId, removeRecord.OriginalLabelTextColor);

		FMGeoTrackingEquipmentHistory.selectionRecordList.splice(selectionRecordIndex, 1);
		FMGeoTrackingEquipmentHistory.ReorderSelectionRecordList();

		if (FMGeoTrackingEquipmentHistory.selectionRecordList.length === 0)
		{
			FMGeoTrackingEquipmentHistory.DisableButton(FMGeoTrackingEquipmentHistory.startInvestigateBtnId);
		}
	}
}

//===============================================================================
// This function will determine if the selected record had been already selected
// by looking in the selection record collection. It will return a -1
// if it does not exist, and return the index of it does.
//===============================================================================
FMGeoTrackingEquipmentHistory.SelectionRecordExist = function (rowId)
{
	if (FMGeoTrackingEquipmentHistory.selectionRecordList.length === 0)
	{
		return -1;
	}
	
	for (var nextRecordIndex = 0; nextRecordIndex < FMGeoTrackingEquipmentHistory.selectionRecordList.length; nextRecordIndex++)
	{
		var record = FMGeoTrackingEquipmentHistory.selectionRecordList[nextRecordIndex];

		if (record.RowId === rowId)
		{
			return nextRecordIndex;
		}
	}

	return -1;
}

//=====================================================================================
// This function will determine if the selected row is a row that has a contamination.
// It will true if it does, otherwise it returns false.
//=====================================================================================
FMGeoTrackingEquipmentHistory.IsSelectedRecordContaminated = function (rowGuidStr)
{
	for (var nextRecordIndex = 0; nextRecordIndex < FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList.length; nextRecordIndex++) 
	{
		var equipRecord = FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList[nextRecordIndex];

		if (rowGuidStr === equipRecord.AssetTrackingDetailGuidStr)
		{
			return equipRecord.IsContaminated;
		}
	}

	return false;
}

//=====================================================================================
// This function will determine if a row is selectable. It will return true if it
// is, otherwise it will return false.
//=====================================================================================
FMGeoTrackingEquipmentHistory.IsRowSelectable = function (rowGuidStr)
{
	for (var nextRecordIndex = 0; nextRecordIndex < FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList.length; nextRecordIndex++)
	{
		var equipRecord = FMGeoTrackingEquipmentHistory.equipmentHistoryModel.EquipmentHistoryRecordList[nextRecordIndex];

		if (rowGuidStr === equipRecord.AssetTrackingDetailGuidStr)
		{
			if (equipRecord.MessageState === FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedFailed
				|| equipRecord.MessageState === FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedPassed)
			{
				return false;
			}

			return true;
		}
	}

	return true; 
}

//==============================================================================
// This function a newly created selection record object.
//==============================================================================
FMGeoTrackingEquipmentHistory.CreateSelectionRecord = function ()
{
	var selectionRecord = new Object();
	selectionRecord.RowId = "";
	selectionRecord.RowGuidStr = "";
	selectionRecord.OriginalLabelTextColor = "";
	selectionRecord.OriginalRowColor = "";
	selectionRecord.RowNumber = -1;
	selectionRecord.InitialRowSelection = false;

	return selectionRecord;
}

//================================================================================
// This function will retrieve the original label text color of a selected row.
//================================================================================
FMGeoTrackingEquipmentHistory.GetOriginalLabelTextColor = function (rowGuid)
{
	var labelId = "DeviceIdLabel_" + rowGuid;
	var labelObj = document.getElementById(labelId);

	return labelObj.style.color;
}

//=====================================================================================
// This function will set the label text color for all the columns in the selected
// row.
//=====================================================================================
FMGeoTrackingEquipmentHistory.SetLabelTextColor = function (labelTextColor, rowGuid)
{
	var labelId = "DeviceIdLabel_" + rowGuid;
	var labelObj = document.getElementById(labelId);
	labelObj.style.color = labelTextColor;

	labelId = "GpsLabel_" + rowGuid;
	labelObj = document.getElementById(labelId);
	labelObj.style.color = labelTextColor;

	labelId = "MessageTimestampLabel_" + rowGuid;
	labelObj = document.getElementById(labelId);
	labelObj.style.color = labelTextColor;

	labelId = "ProductLabel_" + rowGuid;
	labelObj = document.getElementById(labelId);
	labelObj.style.color = labelTextColor;

	labelId = "VolumeLabel_" + rowGuid;
	labelObj = document.getElementById(labelId);
	labelObj.style.color = labelTextColor;

	labelId = "WaterLabel_" + rowGuid;
	labelObj = document.getElementById(labelId);
	labelObj.style.color = labelTextColor;

	labelId = "DensityLabel_" + rowGuid;
	labelObj = document.getElementById(labelId);
	labelObj.style.color = labelTextColor;

	labelId = "DielectricLabel_" + rowGuid;
	labelObj = document.getElementById(labelId);
	labelObj.style.color = labelTextColor;

	labelId = "RemarksLabel_" + rowGuid;
	labelObj = document.getElementById(labelId);
	labelObj.style.color = labelTextColor;
}

//=================================================================================================
// This function will set the compartment label text colors.
//=================================================================================================
FMGeoTrackingEquipmentHistory.SetCompartmentLabelTextColor = function (parentRowId, labelTextColor)
{
	var compartmentMax = 4;
	var compartmentNumber = 1;

	for (var nextIndex = 0; nextIndex < compartmentMax; nextIndex++)
	{
		var rowId = parentRowId + "-C" + compartmentNumber;
		var rowObj = document.getElementById(rowId);
		compartmentNumber++;

		if (rowObj)
		{
			var labelId = "DeviceIdLabel_" + rowId;
			var labelObj = document.getElementById(labelId);
			labelObj.style.color = labelTextColor;

			labelId = "VolumeLabel_" + rowId;
			labelObj = document.getElementById(labelId);
			labelObj.style.color = labelTextColor;

			labelId = "WaterLabel_" + rowId;
			labelObj = document.getElementById(labelId);
			labelObj.style.color = labelTextColor;

			labelId = "DielectricLabel_" + rowId;
			labelObj = document.getElementById(labelId);
			labelObj.style.color = labelTextColor;
		}
		else
		{
			break;
		}
	}
}

//===============================================================================================================
// This function will set the compartment row color based on the parent's setting.
//===============================================================================================================
FMGeoTrackingEquipmentHistory.SetCompartmentRowColor = function (parentRowId, messageState, inLabelTextColor)
{
	var compartmentMax = 4;
	var compartmentNumber = 1;

	for (var nextIndex = 0; nextIndex < compartmentMax; nextIndex++)
	{
		var rowId = parentRowId + "-C" + compartmentNumber;
		var rowObj = document.getElementById(rowId);
		compartmentNumber++;

		if (rowObj)
		{
			FMGeoTrackingEquipmentHistory.SetRowColor(rowObj, messageState, inLabelTextColor);
		}
		else
		{
			break;
		}
	}
}

//===============================================================================================================
// This function will reset the compartment row color back to the parent's setting.
//===============================================================================================================
FMGeoTrackingEquipmentHistory.ResetCompartmentRowColor = function (parentRowId, rowColor)
{
	var compartmentMax = 4;
	var compartmentNumber = 1;

	for (var nextIndex = 0; nextIndex < compartmentMax; nextIndex++)
	{
		var rowId = parentRowId + "-C" + compartmentNumber;
		var rowObj = document.getElementById(rowId);
		compartmentNumber++;

		if (rowObj)
		{
			rowObj.style.background = rowColor;
		}
		else
		{
			break;
		}
	}
}

//==============================================================================================
// This function will set the row background color and return the correct label text color.
//==============================================================================================
FMGeoTrackingEquipmentHistory.SetRowColor = function (rowObj, messageState, inLabelTextColor)
{
	var labelTextColor = inLabelTextColor;

	if (messageState === FMGeoTrackingEquipmentHistory.messageStateEnum.Investigate)
	{
		// Set the row to be a yellow color to indicate it is under investigation.
		rowObj.style.background = FMGeoTrackingEquipmentHistory.yellow;
		labelTextColor = FMGeoTrackingEquipmentHistory.black;
	}
	else if (messageState === FMGeoTrackingEquipmentHistory.messageStateEnum.Contaminated)
	{
		// Set the row to be a orange color to indicate it is contaminted.
		rowObj.style.background = FMGeoTrackingEquipmentHistory.orange;
		labelTextColor = FMGeoTrackingEquipmentHistory.black;
	}
	else if (messageState === FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedFailed)
	{
		// Set the row to be a red color to indicate investigated with contamination.
		rowObj.style.background = FMGeoTrackingEquipmentHistory.red;
		labelTextColor = FMGeoTrackingEquipmentHistory.white;
	}
	else if (messageState === FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedPassed)
	{
		// Set the row to be a green color to indicate investigated with no contamination.
		rowObj.style.background = FMGeoTrackingEquipmentHistory.green;
		labelTextColor = FMGeoTrackingEquipmentHistory.white;
	}
	else if (messageState === FMGeoTrackingEquipmentHistory.messageStateEnum.Normal)
	{
		// Set the row to be a white color to indicate no contamination.
		rowObj.style.background = FMGeoTrackingEquipmentHistory.white;
		labelTextColor = FMGeoTrackingEquipmentHistory.darkGray;
	}

	return labelTextColor;
}

//======================================================================================
// This function will order the selection in row order.
//======================================================================================
FMGeoTrackingEquipmentHistory.ReorderSelectionRecordList = function ()
{
	if (FMGeoTrackingEquipmentHistory.selectionRecordList.length < 2)
	{
		return;
	}

	var newSelectionList = FMGeoTrackingEquipmentHistory.selectionRecordList.sort(function(a, b) { return a.RowNumber - b.RowNumber});
	FMGeoTrackingEquipmentHistory.selectionRecordList = newSelectionList;
}

//=============================================================================
// This function will determine if the selected block of rows are in a 
// contiguous block. It will return true if they are, otherwise return false.
// In addition, it will check to see if rows are selected above and below the 
// initial selection.  If so, it will return false, otherwise return true.
//=============================================================================
FMGeoTrackingEquipmentHistory.ValidateSelections = function ()
{
	var resultObj = new Object();
	resultObj.Passed = true;
	resultObj.ErrorMsg = "";

	// Check for a contiguous block of selections.
	if (FMGeoTrackingEquipmentHistory.selectionRecordList.length > 0)
	{
		var nextIndex;
		var selectedRecord = FMGeoTrackingEquipmentHistory.selectionRecordList[0];
		var seqCount = selectedRecord.RowNumber;

		for (nextIndex = 0; nextIndex < FMGeoTrackingEquipmentHistory.selectionRecordList.length; nextIndex++)
		{
			selectedRecord = FMGeoTrackingEquipmentHistory.selectionRecordList[nextIndex];

			if (seqCount !== selectedRecord.RowNumber)
			{
				resultObj.ErrorMsg = "Multiple selects must be in a contiguous block.";
				resultObj.Passed = false;
				return resultObj;
			}

			seqCount++;
		}
	}

	// Check for selections above and below the initial selections.
	// This is an error.
	if (FMGeoTrackingEquipmentHistory.selectionRecordList.length > 2)
	{
		var nextItem;
		var selectedRec;
		var initialSelectionRowNumber = -1;

		for (nextItem = 0; nextItem < FMGeoTrackingEquipmentHistory.selectionRecordList.length; nextItem++)
		{
			selectedRec = FMGeoTrackingEquipmentHistory.selectionRecordList[nextItem];

			if (selectedRec.InitialRowSelection)
			{
				initialSelectionRowNumber = selectedRec.RowNumber;
				break;
			}
		}

		if (initialSelectionRowNumber > -1)
		{
			var selectionAbove = false;
			var selectionBelow = false;

			for (nextItem = 0; nextItem < FMGeoTrackingEquipmentHistory.selectionRecordList.length; nextItem++)
			{
				selectedRec = FMGeoTrackingEquipmentHistory.selectionRecordList[nextItem];

				if (selectedRec.RowNumber > initialSelectionRowNumber)
				{
					selectionAbove = true;
				}

				if (selectedRec.RowNumber < initialSelectionRowNumber)
				{
					selectionBelow = true;
				}
			}

			if (selectionAbove && selectionBelow)
			{
				resultObj.ErrorMsg = "Cannot select messages above and below the initial selected message.";
				resultObj.Passed = false;
				return resultObj;
			}
		}
	}

	return resultObj;
}

//===============================================================================
// This function handles the passed radio button on change event.
//===============================================================================
FMGeoTrackingEquipmentHistory.HandlePassedRadioBtnEvent = function (radioBtnObj)
{
	if (radioBtnObj.checked)
	{
		var failedRadioBtn = document.getElementById(FMGeoTrackingEquipmentHistory.failedRadioButtonId);
		failedRadioBtn.checked = false;
	}
}

//===============================================================================
// This function handles the failed radio button on change event.
//===============================================================================
FMGeoTrackingEquipmentHistory.HandleFailedRadioBtnEvent = function (radioBtnObj)
{
	if (radioBtnObj.checked)
	{
		var passedRadioBtn = document.getElementById(FMGeoTrackingEquipmentHistory.passedRadioButtonId);
		passedRadioBtn.checked = false;
	}
}

//==================================================================================
// This function will initialize the complete investigate dialog radio buttons.
//==================================================================================
FMGeoTrackingEquipmentHistory.InitializeCompleteInvestigateRadioBtns = function ()
{
	var passedRadioBtn = document.getElementById(FMGeoTrackingEquipmentHistory.passedRadioButtonId);
	var failedRadioBtn = document.getElementById(FMGeoTrackingEquipmentHistory.failedRadioButtonId);
	passedRadioBtn.checked = true;
	failedRadioBtn.checked = false;
}

//==================================================================================
// This function will initialize the complete investigate dialog radio buttons.
//==================================================================================
FMGeoTrackingEquipmentHistory.GetCompleteInvestigateRadioBtnState = function ()
{
	var passedRadioBtn = document.getElementById(FMGeoTrackingEquipmentHistory.passedRadioButtonId);

	if (passedRadioBtn.checked)
	{
		return FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedPassed;
	}

	return FMGeoTrackingEquipmentHistory.messageStateEnum.InvestigateCompletedFailed;
}

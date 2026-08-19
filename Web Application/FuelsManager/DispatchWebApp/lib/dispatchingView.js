/// <reference path="jquery-1.7.1.js" />
/// <reference path="jquery-ui-1.8.17.custom.min.js" />
/// <reference path="..\slickGrid2.0\slick.grid.js" />
/// <reference path="..\slickGrid2.0\slick.dataview.js" />
/// <reference path="dispatch.js" />
/// <reference path="jquery.hotkey.js"/>

// The dispatching view scope object.  Variables and functions specific to the dispatching view page
// should be added to this object rather than the global windows object.
var DispatchingViewLib = {};
DispatchingViewLib.securityToken = '';
DispatchingViewLib.siteGuid = '';

DispatchingViewLib.requestGridSettingKey = '';
DispatchingViewLib.equipmentGridSettingKey = '';
DispatchingViewLib.personnelGridSettingKey = '';

DispatchingViewLib.jsonRequestGridColumnDefinitions = '';
DispatchingViewLib.jsonEquipmentGridColumnDefinitions = '';
DispatchingViewLib.jsonPersonnelGridColumnDefinitions = '';

DispatchingViewLib.requestGrid = undefined;
DispatchingViewLib.requestData = undefined;
DispatchingViewLib.requestView = undefined;

DispatchingViewLib.requestGridSettings = undefined;
DispatchingViewLib.requestViewInitialized = false;

DispatchingViewLib.equipmentGrid = undefined;
DispatchingViewLib.equipmentData = undefined;
DispatchingViewLib.equipmentView = undefined;

DispatchingViewLib.equipmentGridSettings = undefined;
DispatchingViewLib.equipmentViewInitialized = false;
DispatchingViewLib.equipmentFilterType = '0';
DispatchingViewLib.equipmentSelectChanging = false;

DispatchingViewLib.personnelGrid = undefined;
DispatchingViewLib.personnelData = undefined;
DispatchingViewLib.personnelView = undefined;

DispatchingViewLib.personnelGridSettings = undefined;
DispatchingViewLib.personnelViewInitialized = false;
DispatchingViewLib.operatorFilterType = '0';
DispatchingViewLib.operatorSelectChanging = false;

DispatchingViewLib.refreshTime = new Date();
DispatchingViewLib.refreshTime.setHours(0, 0, 0, 0);

DispatchingViewLib.setSelections = false;
DispatchingViewLib.hasModifyRight = false;

DispatchingViewLib.equipmentGridCellInFocus = false;
DispatchingViewLib.personnelGridCellInFocus = false;
DispatchingViewLib.requestGridCellInFocus = false;
DispatchingViewLib.equipmentSelectInFocus = false;
DispatchingViewLib.equipmentFilterInFocus = false;
DispatchingViewLib.operatorSelectInFocus = false;
DispatchingViewLib.operatorFilterInFocus = false;
DispatchingViewLib.dispatchButtonInFocus = false;
DispatchingViewLib.unDispatchButtonInFocus = false;
DispatchingViewLib.radioButtonInFocus = false;
DispatchingViewLib.fillStandButtonInFocus = false;
DispatchingViewLib.returnToBulkButtonInFocus = false;
DispatchingViewLib.standbyButton2InFocus = false;
DispatchingViewLib.refreshButtonInFocus = false;
DispatchingViewLib.closeButtonInFocus = false;
DispatchingViewLib.homeButtonInFocus = false;
DispatchingViewLib.outButtonInFocus = false;
DispatchingViewLib.standbyButtonInFocus = false;

DispatchingViewLib.equipmentGridInFocus = function() {
	return DispatchingViewLib.equipmentGridCellInFocus && !DispatchingViewLib.equipmentSelectInFocus
		&& !DispatchingViewLib.equipmentFilterInFocus;
};

DispatchingViewLib.personnelGridInFocus = function() {
	return DispatchingViewLib.personnelGridCellInFocus && !DispatchingViewLib.operatorSelectInFocus
		&& !DispatchingViewLib.operatorFilterInFocus;
};

DispatchingViewLib.requestGridInFocus = function() {
	return DispatchingViewLib.requestGridCellInFocus;
};

// Provide custom tab key navigation for the grid controls since they don't accept focus automatically.
// One aspect of this function is to simulate the tab order specified in the associated aspx file via
// the "tabindex" attribute.  The other aspect is to provide custom behavior when the equipment grid
// or equipment select control is tabbed out of focus.
DispatchingViewLib.customTabKeyPocessing = function (e) {
	var tabForward = e.which == 9 && !e.shiftKey && !e.ctrlKey && !e.altKey;
	var tabBackward = e.which == 9 && e.shiftKey && !e.ctrlKey && !e.altKey;
	var rows;
	if (tabForward) {
		if (DispatchingViewLib.equipmentSelectInFocus ||
			DispatchingViewLib.equipmentGridInFocus()) {
			// Simulate tab to operator select control
		    e.preventDefault();
		    e.stopImmediatePropagation();
			DispatchingViewLib.onEquipmentTabSelectPersonnel();
			$('#OperatorSelect').focus();
		} else if (DispatchingViewLib.operatorSelectInFocus ||
					DispatchingViewLib.personnelGridInFocus()) {
			// Simulate tab to request grid
		    e.preventDefault();
		    e.stopImmediatePropagation();
			if (DispatchingViewLib.requestGrid.getRenderedRange().bottom < 0) {
				$('#gridRequests .grid-canvas').focus();
			} else {
				rows = DispatchingViewLib.requestGrid.getSelectedRows();
				// If no rows are selected simulate a click in the first cell to give focus to the grid.
				// The second cell must be clicked if the first cell is selected but not in focus.
				if (rows.length < 1) {
					$('#gridRequests .slick-cell:eq(0)').click();
					if (DispatchingViewLib.requestGrid.getSelectedRows().length < 1) {
						$('#gridRequests .slick-cell:eq(1)').click();
					}
				} else {
					$('#gridRequests .grid-canvas').focus();
				}
			}
		} else if (DispatchingViewLib.requestGridInFocus()) {
			// Simulate tab to dispatch button or next enabled bottom toolbar button
		    e.preventDefault();
		    e.stopImmediatePropagation();
			if ($('#DispatchButton')[0].disabled == false) {
				$('#DispatchButton').focus();
			} else if ($('#UnDispatchButton')[0].disabled == false) {
				$('#UnDispatchButton').focus();
			} else if ($('#RadioButton')[0].disabled == false) {
				$('#RadioButton').focus();
			} else if ($('#FillStandButton')[0].disabled == false) {
				$('#FillStandButton').focus();
			} else if ($('#ReturnToBulkButton')[0].disabled == false) {
				$('#ReturnToBulkButton').focus();
			} else if ($('#StandbyButton2')[0].disabled == false) {
				$('#StandbyButton2').focus();
			} else if ($('#RefreshButton')[0].disabled == false) {
				$('#RefreshButton').focus();
			} else {
				$('#CloseButton').focus(); // Close button always enabled
			}
		} else if (DispatchingViewLib.equipmentFilterInFocus ||
					DispatchingViewLib.operatorFilterInFocus ||
					DispatchingViewLib.homeButtonInFocus ||
					DispatchingViewLib.outButtonInFocus ||
					DispatchingViewLib.standbyButtonInFocus ||
					DispatchingViewLib.closeButtonInFocus) {
			// Simulate tab to equipment select control, back to the beginning of the tab order
		    e.preventDefault();
		    e.stopImmediatePropagation();
			$('#EquipmentSelect').focus();
		}
	} else if (tabBackward) {
		if (DispatchingViewLib.equipmentSelectInFocus
			|| DispatchingViewLib.equipmentFilterInFocus
			|| DispatchingViewLib.equipmentGridInFocus()
			|| DispatchingViewLib.operatorFilterInFocus
			|| DispatchingViewLib.homeButtonInFocus
			|| DispatchingViewLib.outButtonInFocus
			|| DispatchingViewLib.standbyButtonInFocus) {
			// Simulate tab to close button
		    e.preventDefault();
		    e.stopImmediatePropagation();
			$('#CloseButton').focus();
		} else if (DispatchingViewLib.closeButtonInFocus && $('#RefreshButton')[0].disabled && $('#StandbyButton2')[0].disabled
						&& $('#ReturnToBulkButton')[0].disabled && $('#FillStandButton')[0].disabled && $('#RadioButton')[0].disabled
						&& $('#UnDispatchButton')[0].disabled && $('#DispatchButton')[0].disabled
				|| DispatchingViewLib.refreshButtonInFocus && $('#StandbyButton2')[0].disabled
						&& $('#ReturnToBulkButton')[0].disabled && $('#FillStandButton')[0].disabled && $('#RadioButton')[0].disabled
						&& $('#UnDispatchButton')[0].disabled && $('#DispatchButton')[0].disabled
				|| DispatchingViewLib.standbyButton2InFocus && $('#ReturnToBulkButton')[0].disabled && $('#FillStandButton')[0].disabled
						&& $('#RadioButton')[0].disabled && $('#UnDispatchButton')[0].disabled && $('#DispatchButton')[0].disabled
				|| DispatchingViewLib.returnToBulkButtonInFocus && $('#FillStandButton')[0].disabled
						&& $('#RadioButton')[0].disabled && $('#UnDispatchButton')[0].disabled && $('#DispatchButton')[0].disabled
				|| DispatchingViewLib.fillStandButtonInFocus && $('#RadioButton')[0].disabled && $('#UnDispatchButton')[0].disabled
					&& $('#DispatchButton')[0].disabled
				|| DispatchingViewLib.radioButtonInFocus && $('#UnDispatchButton')[0].disabled && $('#DispatchButton')[0].disabled
				|| DispatchingViewLib.unDispatchButtonInFocus && $('#DispatchButton')[0].disabled
				|| DispatchingViewLib.dispatchButtonInFocus) {
			// Simulate tab to request grid
		    e.preventDefault();
		    e.stopImmediatePropagation();
			if (DispatchingViewLib.requestGrid.getRenderedRange().bottom < 0) {
				$('#gridRequests .grid-canvas').focus();
			} else {
				rows = DispatchingViewLib.requestGrid.getSelectedRows();
				// If no rows are selected simulate a click in the first cell to give focus to the grid.
				// The second cell must be clicked if the first cell is selected but not in focus.
				if (rows.length < 1) {
					$('#gridRequests .slick-cell:eq(0)').click();
					if (DispatchingViewLib.requestGrid.getSelectedRows().length < 1) {
						$('#gridRequests .slick-cell:eq(1)').click();
					}
				} else {
					$('#gridRequests .grid-canvas').focus();
				}
			}
		} else if (DispatchingViewLib.requestGridInFocus()) {
			// Simulate tab to operator select control
		    e.preventDefault();
		    e.stopImmediatePropagation();
			$('#OperatorSelect').focus();
		} else if (DispatchingViewLib.operatorSelectInFocus ||
					DispatchingViewLib.personnelGridInFocus()) {
			// Simulate tab to equipment select control, back to the beginning of the reverse tab order
		    e.preventDefault();
		    e.stopImmediatePropagation();
			$('#EquipmentSelect').focus();
		}
	}
};

// Bind various keydown handlers for use in custom key handling for the grids
DispatchingViewLib.bindKeyDownHandlers = function() {
	$(document).bind('keydown', 'home', function(event) {
		if (DispatchingViewLib.requestGridInFocus()) {
			DispatchingViewLib.requestGrid.setSelectedRows([0]);
			DispatchingViewLib.requestGrid.setActiveCell(0, 0);
		} else if (DispatchingViewLib.equipmentGridInFocus()) {
			DispatchingViewLib.equipmentGrid.setSelectedRows([0]);
			DispatchingViewLib.equipmentGrid.setActiveCell(0, 0);
		} else if (DispatchingViewLib.personnelGridInFocus()) {
			DispatchingViewLib.personnelGrid.setSelectedRows([0]);
			DispatchingViewLib.personnelGrid.setActiveCell(0, 0);
		}
	});

	$(document).bind('keydown', 'end', function (event) {
		var row;
		if (DispatchingViewLib.requestGridInFocus()) {
			row = DispatchingViewLib.requestData.length - 1;
			DispatchingViewLib.requestGrid.setSelectedRows([row]);
			DispatchingViewLib.requestGrid.setActiveCell(row, 0);
		} else if (DispatchingViewLib.equipmentGridInFocus()) {
			row = DispatchingViewLib.equipmentData.length - 1;
			DispatchingViewLib.equipmentGrid.setSelectedRows([row]);
			DispatchingViewLib.equipmentGrid.setActiveCell(row, 0);
		} else if (DispatchingViewLib.personnelGridInFocus()) {
			row = DispatchingViewLib.personnelData.length - 1;
			DispatchingViewLib.personnelGrid.setSelectedRows([row]);
			DispatchingViewLib.personnelGrid.setActiveCell(row, 0);
		}
	});

	$(document).bind('keydown', 'pageup', function(event) {
		var row;
		if (DispatchingViewLib.requestGridInFocus()) {
			row = DispatchingViewLib.requestGrid.getViewport().top;
			if (row < 0) {
				row = 0;
			}
			DispatchingViewLib.requestGrid.setSelectedRows([row]);
			DispatchingViewLib.requestGrid.setActiveCell(row, 0);
		} else if (DispatchingViewLib.equipmentGridInFocus()) {
			row = DispatchingViewLib.equipmentGrid.getViewport().top;
			if (row < 0) {
				row = 0;
			}
			DispatchingViewLib.equipmentGrid.setSelectedRows([row]);
			DispatchingViewLib.equipmentGrid.setActiveCell(row, 0);
		} else if (DispatchingViewLib.personnelGridInFocus()) {
			row = DispatchingViewLib.personnelGrid.getViewport().top;
			if (row < 0) {
				row = 0;
			}
			DispatchingViewLib.personnelGrid.setSelectedRows([row]);
			DispatchingViewLib.personnelGrid.setActiveCell(row, 0);
		}
	});

	$(document).bind('keydown', 'pagedown', function(event) {
		var row;
		if (DispatchingViewLib.requestGridInFocus()) {
			row = DispatchingViewLib.requestGrid.getViewport().bottom;
			if (row >= DispatchingViewLib.requestData.length) {
				row = DispatchingViewLib.requestData.length - 1;
			}
			DispatchingViewLib.requestGrid.setSelectedRows([row]);
			DispatchingViewLib.requestGrid.setActiveCell(row, 0);
		} else if (DispatchingViewLib.equipmentGridInFocus()) {
			row = DispatchingViewLib.equipmentGrid.getViewport().bottom;
			if (row >= DispatchingViewLib.equipmentData.length) {
				row = DispatchingViewLib.equipmentData.length - 1;
			}
			DispatchingViewLib.equipmentGrid.setSelectedRows([row]);
			DispatchingViewLib.equipmentGrid.setActiveCell(row, 0);
		} else if (DispatchingViewLib.personnelGridInFocus()) {
			row = DispatchingViewLib.personnelGrid.getViewport().bottom;
			if (row >= DispatchingViewLib.personnelData.length) {
				row = DispatchingViewLib.personnelData.length - 1;
			}
			DispatchingViewLib.personnelGrid.setSelectedRows([row]);
			DispatchingViewLib.personnelGrid.setActiveCell(row, 0);
		}
	});
};

// Bind various focus handlers for use in tab key navgation
DispatchingViewLib.bindFocusHandlers = function () {
	$('#equipmentGridCell').focusin(function () {
		DispatchingViewLib.equipmentGridCellInFocus = true;
		$('#equipmentGridCell').css('border-color', '#dede00');
	});
	$('#equipmentGridCell').focusout(function () {
		DispatchingViewLib.equipmentGridCellInFocus = false;
		$('#equipmentGridCell').css('border-color', 'black');
	});
	$('#personnelGridCell').focusin(function () {
		DispatchingViewLib.personnelGridCellInFocus = true;
		$('#personnelGridCell').css('border-color', '#dede00');
	});
	$('#personnelGridCell').focusout(function () {
		DispatchingViewLib.personnelGridCellInFocus = false;
		$('#personnelGridCell').css('border-color', 'black');
	});
	$('#requestGridCell').focusin(function () {
		DispatchingViewLib.requestGridCellInFocus = true;
		$('#requestGridCell').css('border-color', '#dede00');
	});
	$('#requestGridCell').focusout(function () {
		DispatchingViewLib.requestGridCellInFocus = false;
		$('#requestGridCell').css('border-color', 'black');
	});
	$('#EquipmentSelect').focus(function () {
		DispatchingViewLib.equipmentSelectInFocus = true;
		$('#EquipmentSelect').css('border-color', '#dede00');
	});
	$('#EquipmentSelect').blur(function () {
		DispatchingViewLib.equipmentSelectInFocus = false;
		$('#EquipmentSelect').css('border-color', 'black');
	});
	$('#EquipmentFilter').focus(function () {
		DispatchingViewLib.equipmentFilterInFocus = true;
		$('#EquipmentFilter').css('border-color', '#dede00');
	});
	$('#EquipmentFilter').blur(function () {
		DispatchingViewLib.equipmentFilterInFocus = false;
		$('#EquipmentFilter').css('border-color', 'black');
	});
	$('#OperatorSelect').focus(function () {
		DispatchingViewLib.operatorSelectInFocus = true;
		$('#OperatorSelect').css('border-color', '#dede00');
	});
	$('#OperatorSelect').blur(function () {
		DispatchingViewLib.operatorSelectInFocus = false;
		$('#OperatorSelect').css('border-color', 'black');
	});
	$('#OperatorFilter').focus(function () {
		DispatchingViewLib.operatorFilterInFocus = true;
		$('#OperatorFilter').css('border-color', '#dede00');
	});
	$('#OperatorFilter').blur(function () {
		DispatchingViewLib.operatorFilterInFocus = false;
		$('#OperatorFilter').css('border-color', 'black');
	});
	$('#DispatchButton').focus(function () {
		DispatchingViewLib.dispatchButtonInFocus = true;
	});
	$('#DispatchButton').blur(function () {
		DispatchingViewLib.dispatchButtonInFocus = false;
	});
	$('#UnDispatchButton').focus(function () {
		DispatchingViewLib.unDispatchButtonInFocus = true;
	});
	$('#UnDispatchButton').blur(function () {
		DispatchingViewLib.unDispatchButtonInFocus = false;
	});
	$('#RadioButton').focus(function () {
		DispatchingViewLib.radioButtonInFocus = true;
	});
	$('#RadioButton').blur(function () {
		DispatchingViewLib.radioButtonInFocus = false;
	});
	$('#FillStandButton').focus(function () {
		DispatchingViewLib.fillStandButtonInFocus = true;
	});
	$('#FillStandButton').blur(function () {
		DispatchingViewLib.fillStandButtonInFocus = false;
	});
	$('#ReturnToBulkButton').focus(function () {
		DispatchingViewLib.returnToBulkButtonInFocus = true;
	});
	$('#ReturnToBulkButton').blur(function () {
		DispatchingViewLib.returnToBulkButtonInFocus = false;
	});
	$('#StandbyButton2').focus(function () {
		DispatchingViewLib.standbyButton2InFocus = true;
	});
	$('#StandbyButton2').blur(function () {
		DispatchingViewLib.standbyButton2InFocus = false;
	});
	$('#RefreshButton').focus(function () {
		DispatchingViewLib.refreshButtonInFocus = true;
	});
	$('#RefreshButton').blur(function () {
		DispatchingViewLib.refreshButtonInFocus = false;
	});
	$('#CloseButton').focus(function () {
		DispatchingViewLib.closeButtonInFocus = true;
	});
	$('#CloseButton').blur(function () {
		DispatchingViewLib.closeButtonInFocus = false;
	});
	$('#HomeButton').focus(function () {
		DispatchingViewLib.homeButtonInFocus = true;
	});
	$('#HomeButton').blur(function () {
		DispatchingViewLib.homeButtonInFocus = false;
	});
	$('#OutButton').focus(function () {
		DispatchingViewLib.outButtonInFocus = true;
	});
	$('#OutButton').blur(function () {
		DispatchingViewLib.outButtonInFocus = false;
	});
	$('#StandbyButton').focus(function () {
		DispatchingViewLib.standbyButtonInFocus = true;
	});
	$('#StandbyButton').blur(function () {
		DispatchingViewLib.standbyButtonInFocus = false;
	});
};

DispatchingViewLib.dispatchingViewLoad = function () {
	window.FMMenuBarLib.showFullScreenButton();

	// Bind the FM menu bar on size changed handler
	window.FMMenuBarLib.onSizeChanged = DispatchingViewLib.onMenuBarSizeChanged;

	// Bind various keydown handlers for use in custom key handling for the grids
	DispatchingViewLib.bindKeyDownHandlers();

	// Bind various focus handlers for use in tab key navgation
	DispatchingViewLib.bindFocusHandlers();

	// Bind the keydown handler for use in tab key navgation
	$(document).keydown(DispatchingViewLib.customTabKeyPocessing);

	if (!DispatchLib.displayCurrentTime || window.FMMenuBarLib.inFullScreenMode) {
		$('#currentTime').hide();
		$('#currentDate').hide();
	} else {
		// Set time immediately so view initially resizes correctly
		DispatchingViewLib.updateTime(new Date());
	}

	DispatchingViewLib.requestGrid = undefined;
	DispatchingViewLib.equipmentGrid = undefined;
	DispatchingViewLib.personnelGrid = undefined;

	DispatchingViewLib.loadRequestGridSettings();
	DispatchingViewLib.loadEquipmentGridSettings();
	DispatchingViewLib.loadPersonnelGridSettings();

	$('#EquipmentFilter').change(function () {
		DispatchingViewLib.equipmentFilterType = $('#EquipmentFilter option:selected').val();
		if (DispatchingViewLib.equipmentViewInitialized) {
			DispatchingViewLib.equipmentView.setFilterArgs({ FilterType: DispatchingViewLib.equipmentFilterType });

			var translatedNone;
			var firstOption = $('#EquipmentSelect')[0].options[0];
			if (firstOption) {
				translatedNone = firstOption.text;
			} else {
				translatedNone = '{None}';
			}

			$('#EquipmentSelect option').remove();
			$('#EquipmentSelect').append($("<option />").val(-1).text(translatedNone));

			var count =  DispatchingViewLib.equipmentData.length;
		
			for (var i = 0; i < count; i++) {
				var equipmentData = DispatchingViewLib.equipmentData[i];

				if (DispatchingViewLib.vehicleFilterBaseFunction(equipmentData, DispatchingViewLib.equipmentFilterType)){

					var regId = equipmentData.RegID;
					var identityGuid = equipmentData.IdentityGuid;
					if (regId != undefined && regId != "") {
						$('#EquipmentSelect').append(new Option(regId, identityGuid, true, true));
					}
				}
			}

			$('#EquipmentSelect').val(-1);  // {None} option




			DispatchingViewLib.equipmentView.refresh();
		}

		if (DispatchingViewLib.equipmentFilterType == '4' && DispatchingViewLib.operatorFilterType != '4') {
			// Change the Operator filter type to Flight-Line Status as well
			$('#OperatorFilter').val('4');
			$('#OperatorFilter').change();
		}
	});

	$('#OperatorFilter').change(function () {
		DispatchingViewLib.operatorFilterType = $('#OperatorFilter option:selected').val();
		if (DispatchingViewLib.personnelViewInitialized) {
			DispatchingViewLib.personnelView.setFilterArgs({ FilterType: DispatchingViewLib.operatorFilterType });

			var translatedNone;
			var firstOption = $('#OperatorSelect')[0].options[0];
			if (firstOption) {
				translatedNone = firstOption.text;
			} else {
				translatedNone = '{None}';
			}

			$('#OperatorSelect option').remove();
			$('#OperatorSelect').append($("<option />").val(-1).text(translatedNone));

			var count = DispatchingViewLib.personnelData.length;
			

			for (var i = 0; i < count; i++) {
				var personData = DispatchingViewLib.personnelData[i];

				if (DispatchingViewLib.operatorFilterBaseFunction(personData, DispatchingViewLib.operatorFilterType)) {
					var fullName = personData.FullName;
					var identityGuid = personData.IdentityGuid;
					if (fullName != undefined && fullName != "") {
						$('#OperatorSelect').append(new Option(fullName, identityGuid, true, true));
					}
				}
			}

			$('#OperatorSelect').val(-1);  // {None} option

			DispatchingViewLib.personnelView.refresh();
		}

		if (DispatchingViewLib.operatorFilterType == '4' && DispatchingViewLib.equipmentFilterType != '4') {
			// Change the Equipment filter type to Flight-Line Status as well
			$('#EquipmentFilter').val('4');
			$('#EquipmentFilter').change();
		}
	});

	var isEnabled = DispatchingViewLib.hasModifyRight == 'True';
	DispatchingViewLib.enableControl('#DispatchButton', false);
	DispatchingViewLib.enableControl('#UnDispatchButton', false);
	DispatchingViewLib.enableControl('#RadioButton', false);
	DispatchingViewLib.enableControl('#OutButton', false);
	DispatchingViewLib.enableControl('#HomeButton', isEnabled);
	DispatchingViewLib.enableControl('#StandbyButton', false);
	DispatchingViewLib.enableControl('#StandbyButton2', false);
	DispatchingViewLib.enableControl('#FillStandButton', isEnabled);
	DispatchingViewLib.enableControl('#ReturnToBulkButton', isEnabled);

	// Set up resize event call for when the browser size changes.
	$(window).resize(DispatchingViewLib.resizeDispatchingView);

	var queryStringParams = DispatchLib.getQueryStringParams();
	if (queryStringParams['dispatchStatus'] == 'FlightLine') {
		DispatchingViewLib.setFlightLineSelectionFilters();
	}

	DispatchingViewLib.updateRequestGrid();
	DispatchingViewLib.updateEquipmentGrid();
	DispatchingViewLib.updatePersonnelGrid();

	DispatchingViewLib.personnelGrid.onKeyDown.subscribe(DispatchingViewLib.customTabKeyPocessing);
	DispatchingViewLib.equipmentGrid.onKeyDown.subscribe(DispatchingViewLib.customTabKeyPocessing);
	DispatchingViewLib.requestGrid.onKeyDown.subscribe(DispatchingViewLib.customTabKeyPocessing);

	$('#EquipmentSelect').focus();
};

// If the tab key is pressed to leave the equipment grid or the equipment select control,
// then the selected operator in the personnel grid is changed to the operator assigned
// to the currently selected equipment.
DispatchingViewLib.onEquipmentTabSelectPersonnel = function () {
	var selectedEquipment = $('#EquipmentSelect').val();
	if (selectedEquipment != -1) {
		for (var i = 0; i < DispatchingViewLib.personnelData.length; i++) {
			var person = DispatchingViewLib.personnelData[i];
			if (person.EquipmentGuid == selectedEquipment) {
				$('#OperatorSelect').val(person.IdentityGuid);
				var row = DispatchingViewLib.personnelView.getRowById(person.IdentityGuid);
				DispatchingViewLib.personnelGrid.setSelectedRows([row]);
				// Simulate click on selected row so it has input focus
				var columnCount = DispatchingViewLib.personnelGrid.getColumns().length;
				var cell = row * columnCount;
				$('#gridPersonnel .slick-cell:eq(' + cell.toString() + ')').click();
				break;
			}
		}
	}
};

DispatchingViewLib.onMenuBarSizeChanged = function () {
	if (window.FMMenuBarLib.inFullScreenMode) {
		$('#currentTime').hide();
		$('#currentDate').hide();
	} else {
		$('#currentTime').show();
		$('#currentDate').show();
	}
	DispatchingViewLib.resizeDispatchingView();
};

DispatchingViewLib.vehicleFilterFunction = function (item, args) {
	if (args == undefined || args.FilterType == undefined) {
		return true;
	}
	return DispatchingViewLib.vehicleFilterBaseFunction(item, args.FilterType);

};

DispatchingViewLib.vehicleFilterBaseFunction = function (item, filterType) {
	if (filterType == '0') {
		return true;
	} else if (filterType == '1') {
		console.log('Equipment Type: ' + item.TypeEnum);
		return item.TypeEnum == 'HYDRANT_CART_TYPE';
	} else if (filterType == '2') {
		return (item.InService && !item.LockedOut);
	} else if (filterType == '3') {
		return (item.TypeEnum == 'TANKER_TYPE' || item.TypeEnum == 'TRAILER_TYPE');
	} else if (filterType == '4') {
		// Filter on only assigned equipment
		return DispatchingViewLib.isEquipmentAssigned(item.IdentityGuid);
	}

	return false;
}

DispatchingViewLib.operatorFilterBaseFunction = function (item, filterType) {
	if (filterType == '0') {
		return true;
	} else if (filterType == '1') {
		return item.Status != 'Out' && (item.LockedOut == false);
	} else if (filterType == '2') {
		return item.Status == 'In' && (item.LockedOut == false);
	} else if (filterType == '3') {
		return item.Status == 'STB' && (item.LockedOut == false);
	} else if (filterType == '4') {
		return item.EquipmentGuid != '00000000-0000-0000-0000-000000000000';
	}
	return false;
}

DispatchingViewLib.operatorFilterFunction = function (item, args) {
	if (args == undefined || args.FilterType == undefined) {
		return true;
	}
	return DispatchingViewLib.operatorFilterBaseFunction(item, args.FilterType);


};

DispatchingViewLib.setFlightLineSelectionFilters = function () {
	// Change the Operator filter type to Flight-Line Status
	$('#OperatorFilter').val('4');
	$('#OperatorFilter').change();

	// Change the Equipment filter type to Flight-Line Status
	$('#EquipmentFilter').val('4');
	$('#EquipmentFilter').change();
};

DispatchingViewLib.setGridRowNumber = function (gridData, count) {
	for (var i = 0; i < count; i++) {
		gridData[i].RowNum = i + 1;
	}
};

DispatchingViewLib.updateRequestGrid = function () {
	var newGridCreated = false;

	if (DispatchingViewLib.requestGrid == undefined) {
		DispatchingViewLib.createRequestGrid();
		newGridCreated = true;
	}

	var count = 0;
	DispatchingViewLib.requestData = [];
	if (window.FuelsManagerServiceLib.requestsDataFM) {
		DispatchingViewLib.requestData = window.FuelsManagerServiceLib.requestsDataFM;
		count = DispatchingViewLib.requestData.length;
	}

	// Filter out certain requests
	if (count > 0) {
		var newData = [];
		for (var i = 0; i < count; ++i) {
			var row = DispatchingViewLib.requestData[i];

			if (row.Status != 'Completed' && row.Status != 'Cancelled' && row.Status != 'Posted') {
				newData.add(row);
			}
		}

		DispatchingViewLib.requestData = newData;
		count = newData.length;
	}

	// Set and sort the grid data.
	DispatchingViewLib.requestGrid.invalidateAllRows();
	DispatchingViewLib.requestView.beginUpdate();
	DispatchingViewLib.requestView.setItems(DispatchingViewLib.requestData, 'LineItemGuid');
	DispatchingViewLib.requestView.fastSort(DispatchingViewLib.requestGridSettings.sortColumn, DispatchingViewLib.requestGridSettings.sortAscending);
	DispatchingViewLib.setGridRowNumber(DispatchingViewLib.requestData, count);
	DispatchingViewLib.requestView.endUpdate();

	if (newGridCreated) {
		// Resize the grid
		DispatchingViewLib.resizeDispatchingView();
	} else {
		// Draw the grid
		DispatchingViewLib.requestGrid.render();
	}

	DispatchingViewLib.requestViewInitialized = true;

	if (DispatchingViewLib.setSelections) {
		DispatchingViewLib.setGridSelections(DispatchingViewLib.requestGridSettings, DispatchingViewLib.requestData, DispatchingViewLib.requestGrid);
	}
	else {
		// Attempt to select the transaction that was selected on the tabular view.  This should only occur when transferring from the
		// tabular view page, which will include the transId in the page request params.
		for (i = 0; i < DispatchingViewLib.requestData.length; ++i) {
			if (DispatchingViewLib.requestData[i].TransId == DispatchingViewLib.referenceTransId) {
				DispatchingViewLib.requestGridSettings.guids = [];
				DispatchingViewLib.requestGridSettings.guids.add(DispatchingViewLib.referenceTransId);
				DispatchingViewLib.referenceTransId = undefined;
				DispatchingViewLib.setGridSelections(DispatchingViewLib.requestGridSettings, DispatchingViewLib.requestData, DispatchingViewLib.requestGrid);
			}
		}
	}
};

DispatchingViewLib.updateEquipmentGrid = function () {
	var newGridCreated = false;

	if (DispatchingViewLib.equipmentGrid == undefined) {
		DispatchingViewLib.createEquipmentGrid();
		newGridCreated = true;
	}

	// Turn off selection handling while we populate the list
	$('#EquipmentSelect').change(function () {
		// do nothing
	});

	var translatedNone;
	var firstOption = $('#EquipmentSelect')[0].options[0];
	if (firstOption) {
		translatedNone = firstOption.text;
	} else {
		translatedNone = '{None}';
	}

	$('#EquipmentSelect option').remove();
	$('#EquipmentSelect').append($("<option />").val(-1).text(translatedNone));

	var count = 0;
	DispatchingViewLib.equipmentData = [];
	if (window.FuelsManagerServiceLib.equipmentDataFM) {
		DispatchingViewLib.equipmentData = window.FuelsManagerServiceLib.equipmentDataFM;
		count = window.FuelsManagerServiceLib.equipmentDataFM.length;
	}

	for (var i = 0; i < count; i++) {
		var regId = DispatchingViewLib.equipmentData[i].RegID;
		var identityGuid = DispatchingViewLib.equipmentData[i].IdentityGuid;
		if (regId != undefined && regId != "") {
			$('#EquipmentSelect').append(new Option(regId, identityGuid, true, true));
		}
	}

	$('#EquipmentSelect').val(-1);  // {None} option

	$('#EquipmentSelect').change(function () {
		DispatchingViewLib.equipmentSelectChanging = true;
		if ($('#EquipmentSelect').val() == -1) {
			DispatchingViewLib.equipmentGrid.setSelectedRows([]);
		} else {
			// Select row in equipment grid with specified servicing unit
			var row = DispatchingViewLib.equipmentView.getRowById($('#EquipmentSelect').val());
			DispatchingViewLib.equipmentGrid.setSelectedRows([row]);
			// Simulate click on selected row so it has input focus
			var columnCount = DispatchingViewLib.equipmentGrid.getColumns().length;
			var cell = row * columnCount;
			$('#gridEquipment .slick-cell:eq(' + cell.toString() + ')').click();
		}
		DispatchingViewLib.equipmentSelectChanging = false;
	});

	// Set and sort the grid data.
	DispatchingViewLib.equipmentGrid.invalidateAllRows();
	DispatchingViewLib.equipmentView.beginUpdate();
	DispatchingViewLib.equipmentView.setItems(DispatchingViewLib.equipmentData, 'IdentityGuid');
	DispatchingViewLib.equipmentView.fastSort(DispatchingViewLib.equipmentGridSettings.sortColumn, DispatchingViewLib.equipmentGridSettings.sortAscending);
	DispatchingViewLib.setGridRowNumber(DispatchingViewLib.equipmentData, count);
	DispatchingViewLib.equipmentView.setFilterArgs({ FilterType: DispatchingViewLib.equipmentFilterType });
	DispatchingViewLib.equipmentView.setFilter(DispatchingViewLib.vehicleFilterFunction);
	DispatchingViewLib.equipmentView.endUpdate();

	if (newGridCreated) {
		// Resize the grid
		DispatchingViewLib.resizeDispatchingView();
	} else {
		// Draw the grid
		DispatchingViewLib.equipmentGrid.render();
	}

	DispatchingViewLib.equipmentViewInitialized = true;

	if (DispatchingViewLib.setSelections) {
		DispatchingViewLib.setGridSelections(DispatchingViewLib.equipmentGridSettings, DispatchingViewLib.equipmentData, DispatchingViewLib.equipmentGrid);
	}

};

DispatchingViewLib.updatePersonnelGrid = function () {
	var newGridCreated = false;

	if (DispatchingViewLib.personnelGrid == undefined) {
		DispatchingViewLib.createPersonnelGrid();
		newGridCreated = true;
	}

	// Turn off selection handling while we populate the list
	$('#OperatorSelect').change(function () {
		// do nothing
	});

	var translatedNone;
	var firstOption = $('#OperatorSelect')[0].options[0];
	if (firstOption) {
		translatedNone = firstOption.text;
	} else {
		translatedNone = '{None}';
	}

	$('#OperatorSelect option').remove();
	$('#OperatorSelect').append($("<option />").val(-1).text(translatedNone));

	var count = 0;
	DispatchingViewLib.personnelData = [];
	if (window.FuelsManagerServiceLib.personnelDataFM) {
		DispatchingViewLib.personnelData = window.FuelsManagerServiceLib.personnelDataFM;
		count = window.FuelsManagerServiceLib.personnelDataFM.length;
	}

	for (var i = 0; i < count; i++) {
		var fullName = DispatchingViewLib.personnelData[i].FullName;
		var identityGuid = DispatchingViewLib.personnelData[i].IdentityGuid;
		if (fullName != undefined && fullName != "") {
			$('#OperatorSelect').append(new Option(fullName, identityGuid, true, true));
		}
	}

	$('#OperatorSelect').val(-1);  // {None} option

	$('#OperatorSelect').change(function () {
		DispatchingViewLib.operatorSelectChanging = true;
		if ($('#OperatorSelect').val() == -1) {
			DispatchingViewLib.personnelGrid.setSelectedRows([]);
		} else {
			// Select row in personnel grid with specified operator;
			var row = DispatchingViewLib.personnelView.getRowById($('#OperatorSelect').val());
			DispatchingViewLib.personnelGrid.setSelectedRows([row]);
			// Simulate click on selected row so it has input focus
			var columnCount = DispatchingViewLib.personnelGrid.getColumns().length;
			var cell = row * columnCount;
			$('#gridPersonnel .slick-cell:eq(' + cell.toString() + ')').click();
		}
		DispatchingViewLib.operatorSelectChanging = false;
	});

	// Set and sort the grid data.
	DispatchingViewLib.personnelGrid.invalidateAllRows();
	DispatchingViewLib.personnelView.beginUpdate();
	DispatchingViewLib.personnelView.setItems(DispatchingViewLib.personnelData, 'IdentityGuid');
	DispatchingViewLib.personnelView.fastSort(DispatchingViewLib.personnelGridSettings.sortColumn, DispatchingViewLib.personnelGridSettings.sortAscending);
	DispatchingViewLib.setGridRowNumber(DispatchingViewLib.personnelData, count);
	DispatchingViewLib.personnelView.setFilterArgs({ FilterType: DispatchingViewLib.operatorFilterType });
	DispatchingViewLib.personnelView.setFilter(DispatchingViewLib.operatorFilterFunction);
	DispatchingViewLib.personnelView.endUpdate();

	if (newGridCreated) {
		// Resize the grid
		DispatchingViewLib.resizeDispatchingView();
	} else {
		// Draw the grid
		DispatchingViewLib.personnelGrid.render();
	}

	DispatchingViewLib.personnelViewInitialized = true;

	if (DispatchingViewLib.setSelections) {
		DispatchingViewLib.setGridSelections(DispatchingViewLib.personnelGridSettings, DispatchingViewLib.personnelData, DispatchingViewLib.personnelGrid);
	}
};

DispatchingViewLib.setGridSelections = function (settings, data, grid) {
	if (settings != undefined
		&& settings.guids != undefined
			&& data.length != 0) {
		var count = settings.guids.length;

		var rows = [];

		for (var i = 0; i < count; i++) {
			// Look up the row number of ths guid
			var rowNumber = DispatchingViewLib.findGuidRowNumber(data, settings.guids[i]);

			if (rowNumber != undefined) {
				rows.add(rowNumber);
			}
		}

		grid.setSelectedRows(rows);
	}
};

DispatchingViewLib.findGuidRowNumber = function (data, guid) {
	var count = data.length;

	for (var i = 0; i < count; i++) {
		if (data[i].IdentityGuid == guid) {
			return i;
		}
	}

	return undefined;
};

DispatchingViewLib.createRequestGrid = function () {
	var columns = [];

	DispatchLib.setGridColumnDefaults(DispatchingViewLib.requestGridSettings, columns, DispatchingViewLib.rowFormatterRequestGrid);

	var options = {
		editable: false,
		enableAddRow: false,
		enableCellNavigation: true,
		enableColumnReorder: true,
		forceFitColumns: false,
		multiSelect: true,
		rowHeight: 30
	};

	DispatchingViewLib.requestData = [];
	DispatchingViewLib.requestView = new Slick.Data.DataView({ inlineFilters: false });
	DispatchingViewLib.requestGrid = new Slick.Grid('#gridRequests', DispatchingViewLib.requestView, columns, options);
	DispatchingViewLib.requestGrid.setSelectionModel(new Slick.RowSelectionModel());
	DispatchingViewLib.requestView.syncGridSelection(DispatchingViewLib.requestGrid, true);

	// wire up model events to save user settings and drive the grid
	DispatchingViewLib.requestGrid.onColumnsResized.subscribe(function () {
		if (DispatchLib.useLocalStorage && DispatchingViewLib.requestViewInitialized) {
			var columns1 = DispatchingViewLib.requestGrid.getColumns();
			for (var loop = 0; loop < columns1.length; ++loop) {
				DispatchingViewLib.requestGridSettings.columnDef[columns1[loop].id].width = columns1[loop].width;
			}
			DispatchingViewLib.saveRequestGridSettings();
		}
		DispatchingViewLib.resizeDispatchingView();
	});

	DispatchingViewLib.requestGrid.onColumnsReordered.subscribe(function () {
		if (DispatchLib.useLocalStorage && DispatchingViewLib.requestViewInitialized) {
			var columns1 = DispatchingViewLib.requestGrid.getColumns();
			for (var loop = 0; loop < columns1.length; ++loop) {
				DispatchingViewLib.requestGridSettings.columnOrder[loop] = columns1[loop].id;
			}
			DispatchingViewLib.saveRequestGridSettings();
		}
	});

	DispatchingViewLib.requestGrid.onSort.subscribe(function (e, args) {
		if (DispatchingViewLib.requestViewInitialized) {
			DispatchingViewLib.requestGridSettings.sortAscending = args.sortAsc;
			DispatchingViewLib.requestGridSettings.sortColumn = args.sortCol.field;
			DispatchingViewLib.requestView.beginUpdate();
			DispatchingViewLib.requestView.fastSort(args.sortCol.field, args.sortAsc);
			DispatchingViewLib.setGridRowNumber(DispatchingViewLib.requestData, DispatchingViewLib.requestData.length);
			DispatchingViewLib.requestView.endUpdate();

			if (DispatchLib.useLocalStorage) {
				DispatchingViewLib.saveRequestGridSettings();
			}
		}
	});

	DispatchingViewLib.requestGrid.onSelectedRowsChanged.subscribe(function (e, args) {
		if (DispatchingViewLib.requestViewInitialized) {
			var count = args.rows.length;

			DispatchingViewLib.enableControl('#DispatchButton', false);
			DispatchingViewLib.enableControl('#UnDispatchButton', false);

			var hasModifyRight = DispatchingViewLib.hasModifyRight == 'True';

			if (count > 0) {
				var newCancelState = hasModifyRight;
				var newDispatchState = hasModifyRight;

				var requestedData = DispatchingViewLib.requestView.getItem(args.rows[0]);

				var transIDs = requestedData.TransId;
				var lineGuids = requestedData.LineItemGuid;

				for (var i = 0; i < count; i++) {
					var rowNumber = args.rows[i];
					var request = DispatchingViewLib.requestView.getItem(rowNumber);

					newCancelState = newCancelState && request.Status == 'Dispatched';
					newDispatchState = newDispatchState && (request.Status == 'Requested' || request.Status == 'Scheduled');

					if (i > 0) {
						transIDs = transIDs + ',' + request.TransId;
						lineGuids = lineGuids + ',' + request.LineItemGuid;
					}
				}

				DispatchingViewLib.enableControl('#UnDispatchButton', newCancelState);
				DispatchingViewLib.enableControl('#DispatchButton', newDispatchState);

				$('#RequestGridSelection').val(transIDs);
				$('#RequestGridSelectionGuids').val(lineGuids);

			}

			var isRadioEnabled = (count == 1) && hasModifyRight;
			DispatchingViewLib.enableControl('#RadioButton', isRadioEnabled);
		}
	});

	DispatchingViewLib.requestView.onRowCountChanged.subscribe(function (e, args) {
		if (DispatchingViewLib.requestViewInitialized) {
			DispatchingViewLib.requestGrid.updateRowCount();
			DispatchingViewLib.requestGrid.render();
		}
	});

	DispatchingViewLib.requestView.onRowsChanged.subscribe(function (e, args) {
		if (DispatchingViewLib.requestViewInitialized) {
			DispatchingViewLib.requestGrid.invalidateRows(args.rows);
			DispatchingViewLib.requestGrid.render();
		}
	});

};

DispatchingViewLib.createEquipmentGrid = function () {
	var columns = [];

	DispatchLib.setGridColumnDefaults(DispatchingViewLib.equipmentGridSettings, columns, DispatchingViewLib.rowFormatterEquipmentGrid);

	var options = {
		editable: false,
		enableAddRow: false,
		enableCellNavigation: true,
		enableColumnReorder: true,
		forceFitColumns: false,
		multiSelect: false,
		rowHeight: 30
	};

	DispatchingViewLib.equipmentData = [];
	DispatchingViewLib.equipmentView = new Slick.Data.DataView({ inlineFilters: false });
	DispatchingViewLib.equipmentGrid = new Slick.Grid('#gridEquipment', DispatchingViewLib.equipmentView, columns, options);
	DispatchingViewLib.equipmentGrid.setSelectionModel(new Slick.RowSelectionModel());
	DispatchingViewLib.equipmentView.syncGridSelection(DispatchingViewLib.equipmentGrid, true);

	// wire up model events to save user settings and drive the grid
	DispatchingViewLib.equipmentGrid.onColumnsResized.subscribe(function () {
		if (DispatchLib.useLocalStorage && DispatchingViewLib.equipmentViewInitialized) {
			var columns1 = DispatchingViewLib.equipmentGrid.getColumns();
			for (var loop = 0; loop < columns1.length; ++loop) {
				DispatchingViewLib.equipmentGridSettings.columnDef[columns1[loop].id].width = columns1[loop].width;
			}
			DispatchingViewLib.saveEquipmentGridSettings();
		}
		DispatchingViewLib.resizeDispatchingView();
	});

	DispatchingViewLib.equipmentGrid.onColumnsReordered.subscribe(function () {
		if (DispatchLib.useLocalStorage && DispatchingViewLib.equipmentViewInitialized) {
			var columns1 = DispatchingViewLib.equipmentGrid.getColumns();
			for (var loop = 0; loop < columns1.length; ++loop) {
				DispatchingViewLib.equipmentGridSettings.columnOrder[loop] = columns1[loop].id;
			}
			DispatchingViewLib.saveEquipmentGridSettings();
		}
	});

	DispatchingViewLib.equipmentGrid.onSort.subscribe(function (e, args) {
		if (DispatchingViewLib.equipmentViewInitialized) {
			DispatchingViewLib.equipmentGridSettings.sortAscending = args.sortAsc;
			DispatchingViewLib.equipmentGridSettings.sortColumn = args.sortCol.field;
			DispatchingViewLib.equipmentView.beginUpdate();
			DispatchingViewLib.equipmentView.fastSort(args.sortCol.field, args.sortAsc);
			DispatchingViewLib.setGridRowNumber(DispatchingViewLib.equipmentData, DispatchingViewLib.equipmentData.length);
			DispatchingViewLib.equipmentView.endUpdate();

			if (DispatchLib.useLocalStorage) {
				DispatchingViewLib.saveEquipmentGridSettings();
			}
		}
	});

	DispatchingViewLib.equipmentGrid.onSelectedRowsChanged.subscribe(function (e, args) {
		if (DispatchingViewLib.equipmentViewInitialized) {
			var count = args.rows.length;

			// Set button states based on selected item
			if (count > 0) {
				var equipment = DispatchingViewLib.equipmentView.getItem(args.rows[0]);

				// Set the selection combo
				if (!DispatchingViewLib.equipmentSelectChanging) {
					$('#EquipmentSelect').val(equipment.IdentityGuid).attr('selected', true);
				}

				$('#EquipmentGridSelection').val(equipment.IdentityGuid);
			}
		}
	});

	DispatchingViewLib.equipmentView.onRowCountChanged.subscribe(function (e, args) {
		if (DispatchingViewLib.equipmentViewInitialized) {
			DispatchingViewLib.equipmentGrid.updateRowCount();
			DispatchingViewLib.equipmentGrid.render();
		}
	});

	DispatchingViewLib.equipmentView.onRowsChanged.subscribe(function (e, args) {
		if (DispatchingViewLib.equipmentViewInitialized) {
			DispatchingViewLib.equipmentGrid.invalidateRows(args.rows);
			DispatchingViewLib.equipmentGrid.render();
			DispatchingViewLib.InitializeEquipmentSelection();
		}
	});

	// callback function is generated in page code behind
	DispatchingViewLib.equipmentGrid.onDblClick.subscribe(DispatchingViewLib.equipmentGridDoubleClick);
};

DispatchingViewLib.createPersonnelGrid = function () {
	var columns = [];

	DispatchLib.setGridColumnDefaults(DispatchingViewLib.personnelGridSettings, columns, DispatchingViewLib.rowFormatterPersonnelGrid);

	var options = {
		editable: false,
		enableAddRow: false,
		enableCellNavigation: true,
		enableColumnReorder: true,
		forceFitColumns: false,
		multiSelect: false,
		rowHeight: 30
	};

	DispatchingViewLib.personnelData = [];
	DispatchingViewLib.personnelView = new Slick.Data.DataView({ inlineFilters: false });
	DispatchingViewLib.personnelGrid = new Slick.Grid('#gridPersonnel', DispatchingViewLib.personnelView, columns, options);
	DispatchingViewLib.personnelGrid.setSelectionModel(new Slick.RowSelectionModel());
	DispatchingViewLib.personnelView.syncGridSelection(DispatchingViewLib.personnelGrid, true);

	// wire up model events to save user settings and drive the grid
	DispatchingViewLib.personnelGrid.onColumnsResized.subscribe(function () {
		if (DispatchLib.useLocalStorage && DispatchingViewLib.personnelViewInitialized) {
			var columns1 = DispatchingViewLib.personnelGrid.getColumns();
			for (var loop = 0; loop < columns1.length; ++loop) {
				DispatchingViewLib.personnelGridSettings.columnDef[columns1[loop].id].width = columns1[loop].width;
			}
			DispatchingViewLib.savePersonnelGridSettings();
		}
		DispatchingViewLib.resizeDispatchingView();
	});

	DispatchingViewLib.personnelGrid.onColumnsReordered.subscribe(function () {
		if (DispatchLib.useLocalStorage && DispatchingViewLib.personnelViewInitialized) {
			var columns1 = DispatchingViewLib.personnelGrid.getColumns();
			for (var loop = 0; loop < columns1.length; ++loop) {
				DispatchingViewLib.personnelGridSettings.columnOrder[loop] = columns1[loop].id;
			}
			DispatchingViewLib.savePersonnelGridSettings();
		}
	});

	DispatchingViewLib.personnelGrid.onSort.subscribe(function (e, args) {
		if (DispatchingViewLib.personnelViewInitialized) {
			DispatchingViewLib.personnelGridSettings.sortAscending = args.sortAsc;
			DispatchingViewLib.personnelGridSettings.sortColumn = args.sortCol.field;
			DispatchingViewLib.personnelView.beginUpdate();
			DispatchingViewLib.personnelView.fastSort(args.sortCol.field, args.sortAsc);
			DispatchingViewLib.setGridRowNumber(DispatchingViewLib.personnelData, DispatchingViewLib.personnelData.length);
			DispatchingViewLib.personnelView.endUpdate();

			if (DispatchLib.useLocalStorage) {
				DispatchingViewLib.savePersonnelGridSettings();
			}
		}
	});

	DispatchingViewLib.personnelGrid.onSelectedRowsChanged.subscribe(function (e, args) {
		if (DispatchingViewLib.personnelViewInitialized) {
			var count = args.rows.length;

			// Set button states based on selected item
			if (count > 0) {
				var isEnabled = DispatchingViewLib.hasModifyRight == 'True';

				var selInx = args.rows[0];
				var person = DispatchingViewLib.personnelView.getItem(selInx);
			
				var buttonEnabled = isEnabled && person.Status != 'Out';
				DispatchingViewLib.enableControl('#OutButton', buttonEnabled);

				buttonEnabled = isEnabled && person.Status != 'STB';
				DispatchingViewLib.enableControl('#StandbyButton', buttonEnabled);
				DispatchingViewLib.enableControl('#StandbyButton2', buttonEnabled);

				// Also set the selection combo
				$('#OperatorSelect').val(person.IdentityGuid).attr('selected', true);

				$('#OperatorGridSelection').val(person.IdentityGuid);
			}
		}
	});

	DispatchingViewLib.personnelView.onRowCountChanged.subscribe(function (e, args) {
		if (DispatchingViewLib.personnelViewInitialized) {
			DispatchingViewLib.personnelGrid.updateRowCount();
			DispatchingViewLib.personnelGrid.render();
		}
	});

	DispatchingViewLib.personnelView.onRowsChanged.subscribe(function (e, args) {
		if (DispatchingViewLib.personnelViewInitialized) {
			DispatchingViewLib.personnelGrid.invalidateRows(args.rows);
			DispatchingViewLib.personnelGrid.render();
			DispatchingViewLib.InitializePersonnelSelection();
		}
	});

	// callback function is generated in page code behind
	DispatchingViewLib.personnelGrid.onDblClick.subscribe(DispatchingViewLib.personnelGridDoubleClick);
};

DispatchingViewLib.enableControl = function (name, enabled) {
	if (enabled) {
		$(name).attr('disabled', false);
	} else {
		$(name).attr('disabled', 'disabled');
	}
};

DispatchingViewLib.loadRequestGridSettings = function () {
	DispatchingViewLib.requestGridSettingKey = DispatchLib.currentUserGuid + DispatchLib.requestGridSettingKeySuffix;
	if (!DispatchLib.useLocalStorage || window.localStorage[DispatchingViewLib.requestGridSettingKey] == undefined) {
		DispatchingViewLib.requestGridSettings = DispatchLib.getDefaultGridSettings(DispatchingViewLib.jsonRequestGridColumnDefinitions);
	} else {
		try {
			DispatchingViewLib.requestGridSettings = JSON.parse(window.localStorage[DispatchingViewLib.requestGridSettingKey]);
		} catch (err) {
			DispatchingViewLib.requestGridSettings = DispatchLib.getDefaultGridSettings(DispatchingViewLib.jsonRequestGridColumnDefinitions);
			window.localStorage.removeItem(DispatchingViewLib.requestGridSettingKey);
		}
	}
};

DispatchingViewLib.loadEquipmentGridSettings = function () {
	DispatchingViewLib.equipmentGridSettingKey = DispatchLib.currentUserGuid + DispatchLib.equipmentGridSettingKeySuffix;
	if (!DispatchLib.useLocalStorage || window.localStorage[DispatchingViewLib.equipmentGridSettingKey] == undefined) {
		DispatchingViewLib.equipmentGridSettings = DispatchLib.getDefaultGridSettings(DispatchingViewLib.jsonEquipmentGridColumnDefinitions);
	} else {
		try {
			DispatchingViewLib.equipmentGridSettings = JSON.parse(window.localStorage[DispatchingViewLib.equipmentGridSettingKey]);
		} catch (err) {
			DispatchingViewLib.equipmentGridSettings = DispatchLib.getDefaultGridSettings(DispatchingViewLib.jsonEquipmentGridColumnDefinitions);
			window.localStorage.removeItem(DispatchingViewLib.equipmentGridSettingKey);
		}
	}
};

DispatchingViewLib.loadPersonnelGridSettings = function () {
	DispatchingViewLib.personnelGridSettingKey = DispatchLib.currentUserGuid + DispatchLib.personnelGridSettingKeySuffix;
	if (!DispatchLib.useLocalStorage || window.localStorage[DispatchingViewLib.personnelGridSettingKey] == undefined) {
		DispatchingViewLib.personnelGridSettings = DispatchLib.getDefaultGridSettings(DispatchingViewLib.jsonPersonnelGridColumnDefinitions);
	} else {
		try {
			DispatchingViewLib.personnelGridSettings = JSON.parse(window.localStorage[DispatchingViewLib.personnelGridSettingKey]);
		} catch (err) {
			DispatchingViewLib.personnelGridSettings = DispatchLib.getDefaultGridSettings(DispatchingViewLib.jsonPersonnelGridColumnDefinitions);
			window.localStorage.removeItem(DispatchingViewLib.personnelGridSettingKey);
		}
	}
};

DispatchingViewLib.setGridSelectionsFlag = function () {
	DispatchingViewLib.setSelections = true;
};

DispatchingViewLib.rowFormatterRequestGrid = function (row, cell, value) {
	if (value == undefined) {
		value = "";
	}

	if (row >= DispatchingViewLib.requestData.length) {
		return value;
	}

	var rowData = DispatchingViewLib.requestData[row];
	return "<span style='color: " + rowData.ForeColor + "'>" + value + "</span>";
};

DispatchingViewLib.rowFormatterEquipmentGrid = function (row, cell, value) {
	if (value == undefined) {
		value = "";
	}

	if (row >= DispatchingViewLib.equipmentData.length) {
		return value;
	}

	var rowData = DispatchingViewLib.equipmentData[row];
	return "<span style='color: " + rowData.ForeColor + "'>" + value + "</span>";
};

DispatchingViewLib.rowFormatterPersonnelGrid = function (row, cell, value) {
	if (value == undefined) {
		value = "";
	}

	if (row >= DispatchingViewLib.personnelData.length) {
		return value;
	}

	var rowData = DispatchingViewLib.personnelData[row];
	return "<span style='color: " + rowData.ForeColor + "'>" + value + "</span>";
};

DispatchingViewLib.saveRequestGridSettings = function () {
	window.localStorage[DispatchingViewLib.requestGridSettingKey] = JSON.stringify(DispatchingViewLib.requestGridSettings);
};

DispatchingViewLib.saveEquipmentGridSettings = function () {
	window.localStorage[DispatchingViewLib.equipmentGridSettingKey] = JSON.stringify(DispatchingViewLib.equipmentGridSettings);
};

DispatchingViewLib.savePersonnelGridSettings = function () {
	window.localStorage[DispatchingViewLib.personnelGridSettingKey] = JSON.stringify(DispatchingViewLib.personnelGridSettings);
};

DispatchingViewLib.resizeDispatchingView = function () {
	var panelElem = $('#dispatchingViewPanel');
	if (panelElem) {
		// Limit minimum panel width to the width of menu header bar
		var headerBarWidth = window.FMMenuBarLib.headerBarWidth();
		var widthoffset = window.FMMenuBarLib.inFullScreenMode ? 33 : 33;
		var panelWidth = Math.max($(window).width() - widthoffset, headerBarWidth - widthoffset);
		panelElem.width(panelWidth);

		// Limit minimum panel height to 500 pixels
		var menuBarHeight = window.FMMenuBarLib.clientHeight();
		var heightoffset = menuBarHeight + (window.FMMenuBarLib.inFullScreenMode ? 30 : 15);
		var panelHeight = Math.max($(window).height() - heightoffset, 500);
		panelElem.height(panelHeight);

		// Compute total grid height based on panel height and the remaining panel element heights
		var currentTimeHeight = window.FMMenuBarLib.inFullScreenMode ? 0 : $('#currentTime').height();
		var equipmentGridHeaderHeight = $('#equipmentGridHeader').height();
		var requestGridHeaderHeight = $('#requestGridHeader').height();
		var requestCommandButtonsHeight = $('#requestCommandButtons').height();
		var totalGridHeight = panelHeight - currentTimeHeight - equipmentGridHeaderHeight -
									requestGridHeaderHeight - requestCommandButtonsHeight - 5;

		// Compute maximum grid width based on panel width and the width of the grid columns and command buttons
		var homeButtonWidthOffset = $('#HomeButton').width() + 20;
		var maximumGridWidth = 0;

		var requestGridWidth = 0;
		var requestGridElem = $('#gridRequests');
		if (requestGridElem && DispatchingViewLib.requestGrid) {
			// Compute total width of request grid columns
			var requestColumns = DispatchingViewLib.requestGrid.getColumns();
			for (var i = 0; i < requestColumns.length; i++) {
				requestGridWidth += requestColumns[i].width;
			}
		}

		var equipmentGridWidth = 0;
		var equipmentGridElem = $('#gridEquipment');
		if (equipmentGridElem && DispatchingViewLib.equipmentGrid) {
			// Compute total width of equipment grid columns
			var equipmentColumns = DispatchingViewLib.equipmentGrid.getColumns();
			for (i = 0; i < equipmentColumns.length; i++) {
				equipmentGridWidth += equipmentColumns[i].width;
			}
		}

		var personnelGridWidth = 0;
		var personnelGridElem = $('#gridPersonnel');
		if (personnelGridElem && DispatchingViewLib.personnelGrid) {
			// Compute total width personnel of grid columns
			var personnelColumns = DispatchingViewLib.personnelGrid.getColumns();
			for (i = 0; i < personnelColumns.length; i++) {
				personnelGridWidth += personnelColumns[i].width;
			}
		}

		maximumGridWidth = Math.max(requestGridWidth + 17, equipmentGridWidth + personnelGridWidth + homeButtonWidthOffset);
		var requestGridElemWidth = Math.min(panelWidth - 5, maximumGridWidth);
		var equipmentGridElemWidth = Math.floor(5 * requestGridElemWidth / 9);
		var gridHeight = Math.floor(totalGridHeight / 2);

		if (requestGridElem && DispatchingViewLib.requestGrid) {
			requestGridElem.width(requestGridElemWidth);
			requestGridElem.height(gridHeight);
			DispatchingViewLib.requestGrid.resizeCanvas();
		}

		if (equipmentGridElem && DispatchingViewLib.equipmentGrid) {
			equipmentGridElem.width(equipmentGridElemWidth);
			equipmentGridElem.height(gridHeight);
			DispatchingViewLib.equipmentGrid.resizeCanvas();
		}

		if (personnelGridElem && DispatchingViewLib.personnelGrid) {
			personnelGridElem.width(requestGridElemWidth - equipmentGridElemWidth - homeButtonWidthOffset);
			personnelGridElem.height(gridHeight);
			DispatchingViewLib.personnelGrid.resizeCanvas();
		}
	}
};

DispatchingViewLib.updateTime = function (newTime) {
	if (DispatchLib.displayCurrentTime && !window.FMMenuBarLib.inFullScreenMode) {
		$("#currentTime").text(newTime.toLocaleTimeString());
		if (DispatchLib.displayMilitaryJulianDate) {
			$("#currentDate").text(DispatchLib.militaryJulianDate(newTime));
		} else {
			$("#currentDate").text(newTime.toLocaleDateString());
		}
	}
};

DispatchingViewLib.refreshDispatchViewData = function (currentTime) {
	var newTime = currentTime;

	if (currentTime == undefined) {
		window.FuelsManagerServiceLib.topTransactionVersion = 0;
		window.FuelsManagerServiceLib.topEquipmentVersion = 0;
		window.FuelsManagerServiceLib.topPersonnelVersion = 0;
		newTime = new Date();
	}

	if (window.FuelsManagerServiceLib.serviceRequestsStopped &&
		(newTime - window.FuelsManagerServiceLib.serviceRequestsStopTime >
			window.FuelsManagerServiceLib.serviceRequestAutomaticRestartDelay * 1000)) {
		if (console) {
			console.log('Automatic restart delay period has elapsed. Starting service requests.');
		}
		window.FuelsManagerServiceLib.serviceRequestsStopped = false;
	}

	if (currentTime == undefined ||
		(currentTime - DispatchingViewLib.refreshTime) > window.FuelsManagerServiceLib.serviceRequestRefreshPeriod * 1000) {
		if (window.FuelsManagerServiceLib.enableServiceRequests &&
			!window.FuelsManagerServiceLib.serviceRequestsStopped) {
			DispatchingViewLib.CallRequestUpdate();
			DispatchingViewLib.CallPersonnelUpdate();
			DispatchingViewLib.CallEquipmentUpdate();
		}

		DispatchingViewLib.refreshTime = newTime;
	}
};

DispatchingViewLib.CallRequestUpdate = function () {
	var request = {};
	request.beginDate = window.sessionStorage.beginDateFilter;
	request.endDate = window.sessionStorage.endDateFilter;
	request.status = window.sessionStorage.statusFilter;
	request.alias = window.sessionStorage.requestTypeFilter;
	request.securityToken = DispatchingViewLib.securityToken;
	request.siteGuid = DispatchingViewLib.siteGuid;
	request.topTransactionVersion = window.FuelsManagerServiceLib.topTransactionVersion;

	// Call jQuery Ajax interface to FuelsManager
	window.FuelsManagerServiceLib.CallDispatchRequestEnumerateTransactions(request);
};

DispatchingViewLib.CallEquipmentUpdate = function ()
{
	var dispatchRequest = {};
	dispatchRequest.securityToken = DispatchingViewLib.securityToken;
	dispatchRequest.siteGuid = DispatchingViewLib.siteGuid;
	dispatchRequest.topEquipmentVersion = window.FuelsManagerServiceLib.topEquipmentVersion;

	window.FuelsManagerServiceLib.CallDispatchRequestEnumerateEquipment(dispatchRequest);
};

DispatchingViewLib.CallPersonnelUpdate = function () 
{
	var dispatchRequest = {};
	dispatchRequest.securityToken = DispatchingViewLib.securityToken;
	dispatchRequest.siteGuid = DispatchingViewLib.siteGuid;
	dispatchRequest.topPersonnelVersion = window.FuelsManagerServiceLib.topPersonnelVersion;

	window.FuelsManagerServiceLib.CallDispatchRequestEnumeratePersonnel(dispatchRequest);
};

DispatchingViewLib.isEquipmentAssigned = function (identityGuid) {
	var count = DispatchingViewLib.personnelData.length;

	for (var i = 0; i < count; i++) {
		if (DispatchingViewLib.personnelData[i].EquipmentGuid == identityGuid) {
			return true;
		}
	}

	return false;
};

// Called when the Refresh button is clicked.
DispatchingViewLib.RefreshButtonOnClick = function () {
	DispatchingViewLib.refreshDispatchViewData();
	return false;
};

// Called when the Close button is clicked.
DispatchingViewLib.CloseButtonOnClick = function () {
	window.window_location_assign("TabularView.aspx");
	return false;
};

DispatchingViewLib.InitializePersonnelSelection = function () {
	if (DispatchLib.standByStatusValues != null &&
		DispatchLib.standByStatusValues != undefined &&
		DispatchLib.standByStatusValues != '') {
		var postData = JSON.parse(DispatchLib.standByStatusValues);
		var operatorGuid = postData.selectedPersonnel;
		if (operatorGuid != null && operatorGuid != undefined) {
			for (var i = 0; i <= DispatchingViewLib.personnelGrid.getDataLength() - 1; i++) {
				if (DispatchingViewLib.personnelGrid.getDataItem(i).IdentityGuid == operatorGuid) {
					DispatchingViewLib.personnelGrid.setSelectedRows([i]);
				}
			}
		}
	}
};

DispatchingViewLib.InitializeEquipmentSelection = function () {
	if (DispatchLib.standByStatusValues != null &&
		DispatchLib.standByStatusValues != undefined &&
		DispatchLib.standByStatusValues != '') {
		var postData = JSON.parse(DispatchLib.standByStatusValues);
		var equipmentGuid = postData.selectedEquipment;
		if (equipmentGuid != null && equipmentGuid != undefined) {
			for (var i = 0; i <= DispatchingViewLib.equipmentGrid.getDataLength() - 1; i++) {
				if (DispatchingViewLib.equipmentGrid.getDataItem(i).IdentityGuid == equipmentGuid) {
					DispatchingViewLib.equipmentGrid.setSelectedRows([i]);
				}
			}
		}
	}
};

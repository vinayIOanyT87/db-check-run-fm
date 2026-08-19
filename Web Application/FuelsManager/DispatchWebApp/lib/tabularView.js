/// <reference path="jquery-1.7.1.js" />
/// <reference path="jquery-ui-1.8.17.custom.min.js" />
/// <reference path="..\slickGrid2.0\slick.grid.js" />
/// <reference path="..\slickGrid2.0\slick.dataview.js" />
/// <reference path="dispatch.js" />
/// <reference path="jquery.hotkey.js"/>

// The tabular view scope object.  Variables and functions specific to the tabular view page
// should be added to this object rather than the global windows object.
var TabularViewLib = {};
TabularViewLib.securityToken = '';
TabularViewLib.siteGuid = '';
TabularViewLib.selectedRows = [];
TabularViewLib.OperatorStatusSelectedRows = [];
TabularViewLib.gridSettingKey = '';
TabularViewLib.jsonTransactionAliasNames = '';
TabularViewLib.jsonTransactionStatusValues = '';
TabularViewLib.jsonGridColumnDefinitions = '';
TabularViewLib.jsonOperationalLockDateValue = '';
TabularViewLib.jsonOptionalTimesArrivalFlagValue = '';
TabularViewLib.jsonOptionalTimesStartFlagValue = '';
TabularViewLib.jsonOptionalTimesStopFlagValue = '';

TabularViewLib.initialized = false;
TabularViewLib.useArrivalTime = false;
TabularViewLib.useStartTime = false;
TabularViewLib.useStopTime = false;
TabularViewLib.validateBeginDate = false;
TabularViewLib.validateEndDate = false;

TabularViewLib.vehicleFilter = '';

TabularViewLib.tabularGridCellInFocus = false;
TabularViewLib.beginDateInFocus = false;
TabularViewLib.endDateInFocus = false;
TabularViewLib.statusSelectInFocus = false;
TabularViewLib.requestTypeSelectInFocus = false;
TabularViewLib.vehicleSelectInFocus = false;

$.ajaxSetup({
    type: "post",
    contentType: "application/json; charset=utf-8",
    dataType: "json"
});

TabularViewLib.tabularGridInFocus = function () {
	return TabularViewLib.tabularGridCellInFocus && !TabularViewLib.beginDateInFocus
		&& !TabularViewLib.endDateInFocus && !TabularViewLib.statusSelectInFocus
		&& !TabularViewLib.requestTypeSelectInFocus && !TabularViewLib.vehicleSelectInFocus;
};

// Bind various focus handlers for use in tab key navgation
TabularViewLib.bindFocusHandlers = function () {
	$('#tabularGridCell').focusin(function () {
		TabularViewLib.tabularGridCellInFocus = true;
		if (TabularViewLib.tabularGridInFocus) {
			$('#tabularGridCell').css('border-color', '#dede00');
		}
	});
	$('#tabularGridCell').focusout(function () {
		TabularViewLib.tabularGridCellInFocus = false;
		$('#tabularGridCell').css('border-color', 'black');
	});
	$('#begindate').focus(function () {
		TabularViewLib.beginDateInFocus = true;
		$('#tabularGridCell').css('border-color', 'black');
	});
	$('#begindate').blur(function () {
		TabularViewLib.beginDateInFocus = false;
		if (TabularViewLib.validateBeginDate) {
			$("#begindate").datepicker("setDate", $("#begindate").datepicker("getDate"));
			var newDate = $("#begindate").datepicker("getDate");
			var newDateString = newDate.toLocaleDateString();
			if (window.sessionStorage.beginDateFilter != newDateString) {
				$("#enddate").datepicker("option", "minDate", newDate);
				window.sessionStorage.beginDateFilter = newDateString;
				TabularViewLib.ClearGrid();
				TabularViewLib.refreshData();
			}
		}
		TabularViewLib.validateBeginDate = false;
	});
	$('#begindate').keydown(function () {
		TabularViewLib.validateBeginDate = true;
	});
	$('#enddate').focus(function () {
		TabularViewLib.endDateInFocus = true;
		$('#tabularGridCell').css('border-color', 'black');
	});
	$('#enddate').blur(function () {
		TabularViewLib.endDateInFocus = false;
		if (TabularViewLib.validateEndDate) {
			$("#enddate").datepicker("setDate", $("#enddate").datepicker("getDate"));
			var newDate = $("#enddate").datepicker("getDate");
			var newDateString = newDate.toLocaleDateString();
			if (window.sessionStorage.endDateFilter != newDateString) {
				$("#begindate").datepicker("option", "maxDate", newDate);
				window.sessionStorage.endDateFilter = newDateString;
				TabularViewLib.ClearGrid();
				TabularViewLib.refreshData();
			}
		}
		TabularViewLib.validateEndDate = false;
	});
	$('#enddate').keydown(function () {
		TabularViewLib.validateEndDate = true;
	});
	$('#StatusSelect').focus(function () {
		TabularViewLib.statusSelectInFocus = true;
		$('#tabularGridCell').css('border-color', 'black');
	});
	$('#StatusSelect').blur(function () {
		TabularViewLib.statusSelectInFocus = false;
	});
	$('#RequestTypeSelect').focus(function () {
		TabularViewLib.requestTypeSelectInFocus = true;
		$('#tabularGridCell').css('border-color', 'black');
	});
	$('#RequestTypeSelect').blur(function () {
		TabularViewLib.requestTypeSelectInFocus = false;
	});
	$('#VehicleSelect').focus(function () {
		TabularViewLib.vehicleSelectInFocus = true;
		$('#tabularGridCell').css('border-color', 'black');
	});
	$('#VehicleSelect').blur(function () {
		TabularViewLib.vehicleSelectInFocus = false;
	});
};

// Provide custom tab key navigation since the grid control does not accept tab key focus automatically.
// Simulate the tab order specified in the associated aspx file via the "tabindex" attribute.
TabularViewLib.customTabKeyPocessing = function (e) {
	var tabForward = e.keyCode == 9 && !e.shiftKey && !e.ctrlKey && !e.altKey;
	var tabBackward = e.keyCode == 9 && e.shiftKey && !e.ctrlKey && !e.altKey;
	var lastButtonId = $('#toolBarTabular').attr('LastButtonId');
	var enterKey = e.keyCode == 13 && !e.shiftKey && !e.ctrlKey && !e.altKey;
	var firstButtonTabIndex = $('#toolBarTabular').attr('FirstButtonTabIndex');
	var lastButtonTabIndex = $('#toolBarTabular').attr('LastButtonTabIndex');
	var rows;
	if (tabForward) {
		if (document.activeElement.id == lastButtonId) {
			// Simulate tab to grid control
			e.preventDefault();
			if (TabularViewLib.grid.getRenderedRange().bottom < 0) {
				$('#gridTabular .grid-canvas').focus();
			} else {
				rows = TabularViewLib.grid.getSelectedRows();
				// If no rows are selected simulate a click in the first cell to give focus to the grid.
				// The second cell must be clicked if the first cell is selected but not in focus.
				if (rows.length < 1) {
					$('#gridTabular .slick-cell:eq(0)').click();
					if (TabularViewLib.grid.getSelectedRows().length < 1) {
						$('#gridTabular .slick-cell:eq(1)').click();
					}
				} else {
					$('#gridTabular .grid-canvas').focus();
				}
			}
		} else if (TabularViewLib.tabularGridInFocus()) {
			// Simulate tab to begin date control
			e.preventDefault();
			$('#begindate').focus();
			$('#begindate').select();
		}
	} else if (tabBackward) {
		if (TabularViewLib.tabularGridInFocus()) {
			// Simulate tab to last toolbar button
			e.preventDefault();
			$('#' + lastButtonId).focus();
		} else if (TabularViewLib.beginDateInFocus) {
			// Simulate tab to grid control
			e.preventDefault();
			if (TabularViewLib.grid.getRenderedRange().bottom < 0) {
				$('#gridTabular .grid-canvas').focus();
			} else {
				rows = TabularViewLib.grid.getSelectedRows();
				// If no rows are selected simulate a click in the first cell to give focus to the grid.
				// The second cell must be clicked if the first cell is selected but not in focus.
				if (rows.length < 1) {
					$('#gridTabular .slick-cell:eq(0)').click();
					if (TabularViewLib.grid.getSelectedRows().length < 1) {
						$('#gridTabular .slick-cell:eq(1)').click();
					}
				} else {
					$('#gridTabular .grid-canvas').focus();
				}
			}
		}
	} else if (enterKey) {
		if (document.activeElement.tabIndex >= firstButtonTabIndex &&
			document.activeElement.tabIndex <= lastButtonTabIndex) {
			e.preventDefault();
			$('#' + document.activeElement.id).click();
		}
	}
};

TabularViewLib.refreshTime = new Date();
TabularViewLib.refreshTime.setHours(0, 0, 0, 0);

TabularViewLib.grid = undefined;
TabularViewLib.data = undefined;
TabularViewLib.view = undefined;

TabularViewLib.saveGridSettings = function() {
	window.localStorage[TabularViewLib.gridSettingKey] = JSON.stringify(TabularViewLib.gridSettings);
};

TabularViewLib.loadGridSettings = function () {
	TabularViewLib.gridSettingKey = DispatchLib.currentUserGuid + DispatchLib.tabularGridSettingKeySuffix;
	if (!DispatchLib.useLocalStorage || window.localStorage[TabularViewLib.gridSettingKey] == undefined) {
		TabularViewLib.gridSettings = DispatchLib.getDefaultGridSettings(TabularViewLib.jsonGridColumnDefinitions);
	} else {
		try {
			TabularViewLib.gridSettings = JSON.parse(window.localStorage[TabularViewLib.gridSettingKey]);
		} catch(err) {
			TabularViewLib.gridSettings = DispatchLib.getDefaultGridSettings(TabularViewLib.jsonGridColumnDefinitions);
			window.localStorage.removeItem(TabularViewLib.gridSettingKey);
		}
	}
};

TabularViewLib.setGridRowNumber = function (gridData, count) {
	for (var i = 0; i < count; i++) {
		gridData[i].RowNum = i + 1;
	}
};

TabularViewLib.rowFormatter = function (row, cell, value, columnDef, dataContext) {
	if (value == undefined) {
		value = "";
	}

	if (TabularViewLib.data[row].Color != undefined && TabularViewLib.data[row].Color != "") {
		return "<span style='color:" + TabularViewLib.data[row].Color + "'>" + value + "</span>";
	}

	return value;
};

TabularViewLib.linkFormatter = function linkFormatter(row, cell, columnDef, dataContext) {
	return '<span style="margin-left:35px;"><a href="#" onclick="PopupWindow(' + row + ');">View</a></span>';
};

TabularViewLib.createGrid = function () {
	var columns = [];

	DispatchLib.setGridColumnDefaults(TabularViewLib.gridSettings, columns,
										TabularViewLib.rowFormatter, TabularViewLib.linkFormatter);

	var options = {
		editable: false,
		enableAddRow: false,
		enableCellNavigation: true,
		enableColumnReorder: true,
		forceFitColumns: false,
		multiSelect: true,
		rowHeight: 30
	};

	TabularViewLib.data = [];
	TabularViewLib.view = new Slick.Data.DataView({ inlineFilters: false });
	TabularViewLib.grid = new Slick.Grid('#gridTabular', TabularViewLib.view, columns, options);
	TabularViewLib.grid.setSelectionModel(new Slick.RowSelectionModel());
	TabularViewLib.view.syncGridSelection(TabularViewLib.grid, true);

	// wire up model events to save user settings and drive the grid
	TabularViewLib.grid.onColumnsResized.subscribe(function () {
		if (DispatchLib.useLocalStorage && TabularViewLib.initialized) {
			var columns1 = TabularViewLib.grid.getColumns();
			for (var loop = 0; loop < columns1.length; ++loop) {
				TabularViewLib.gridSettings.columnDef[columns1[loop].id].width = columns1[loop].width;
			}
			TabularViewLib.saveGridSettings();
		}
		TabularViewLib.resizeTabularView();
	});

	TabularViewLib.grid.onColumnsReordered.subscribe(function () {
		if (DispatchLib.useLocalStorage && TabularViewLib.initialized) {
			var columns1 = TabularViewLib.grid.getColumns();
			for (var loop = 0; loop < columns1.length; ++loop) {
				TabularViewLib.gridSettings.columnOrder[loop] = columns1[loop].id;
			}
			TabularViewLib.saveGridSettings();
		}
	});

	TabularViewLib.grid.onSort.subscribe(function (e, args) {
		if (TabularViewLib.initialized) {
			TabularViewLib.gridSettings.sortAscending = args.sortAsc;
			TabularViewLib.gridSettings.sortColumn = args.sortCol.field;
			TabularViewLib.view.beginUpdate();
			TabularViewLib.view.fastSort(args.sortCol.field, args.sortAsc);
			TabularViewLib.setGridRowNumber(TabularViewLib.data, TabularViewLib.data.length);
			TabularViewLib.view.endUpdate();

			if (DispatchLib.useLocalStorage) {
				TabularViewLib.saveGridSettings();
			}
		}
	});

	TabularViewLib.grid.onSelectedRowsChanged.subscribe(function (e, args) {
		if (TabularViewLib.initialized) {
			TabularViewLib.selectedRows = args.rows;
			window.sessionStorage.tabularGridSelectedRows = JSON.stringify(args.rows);

			var count = args.rows.length;

			if (count > 0) {
				var transIDs = TabularViewLib.data[args.rows[0]].TransId;
				var lineGuids = TabularViewLib.data[args.rows[0]].LineItemGuid;

				for (var i = 0; i < count; i++) {
					var rowNumber = args.rows[i];
					var request = TabularViewLib.data[rowNumber];

					if (i > 0) {
						transIDs = transIDs + ',' + request.TransId;
						lineGuids = lineGuids + ',' + request.LineItemGuid;
					}
				}

				$('#RequestGridSelection').val(transIDs);
				$('#RequestGridSelectionGuids').val(lineGuids);
			}
		}
	});

	/////////////////////////////////////////////////////////////////////////////////////////////
	// This method will handle the row double click event.
	/////////////////////////////////////////////////////////////////////////////////////////////
	TabularViewLib.grid.onDblClick.subscribe(function (e, args)
	{
		if (TabularViewLib.initialized)
		{
			if (args.row != undefined) 
			{
				var aliasName = TabularViewLib.data[args.row].AliasName;
				var transactionGuid = TabularViewLib.data[args.row].TransactionGuid;
				//var lineGuids = TabularViewLib.data[args.row].LineItemGuid;

				if (aliasName == "Recirculation")
				{
					var sFeatures = "dialogWidth: 800px; dialogHeight: 480px";
					window.window_showModalDialog("RecirculationForm.aspx?TransactionGuid=" + transactionGuid, "", sFeatures);
				}
				else 
				{
					// If the transaction type is a refuel or a defuel we need to check the operational lock date and if it is before
					// that date not allow any modification if it is complete
					var transactionStatus = TabularViewLib.data[args.row].Status;
					var completedStatus = "Completed";

					if (transactionStatus == completedStatus
						&& (aliasName == "Sale" || aliasName == "Defuel")) 
					{
						var completedDateTime = new Date();
						var startYear = completedDateTime.getUTCFullYear() - 100;
						completedDateTime.setUTCFullYear(startYear);

						var requestedTimeStr = TabularViewLib.data[args.row].RequestedTime;
						var transDateTimeStr = TabularViewLib.data[args.row].TransactionDate;

						// Since the requested time is only a time, add the transaction
						// date to the request.
						if (requestedTimeStr.length > 0 && transDateTimeStr.length > 0)
						{
							var timeParts = requestedTimeStr.split(':');

							var requestedDateTime = new Date(transDateTimeStr);
							var hour = parseInt(timeParts[0]);
							var minute = parseInt(timeParts[1]);
							var second = parseInt(timeParts[2]);

							requestedDateTime.setHours(hour, minute, second);
							completedDateTime = requestedDateTime;
						}

						try
						{
							var operationalLockDate = new Date(TabularViewLib.jsonOperationalLockDateValue);

							// check the lock out date
							if (completedDateTime <= operationalLockDate)
							{
								alert("Cannot edit completed transactions before the lock out date.\n\rEdit request ended.");
								return;
							}
						}
						catch (err)
						{
							alert("Invalid operational lock date.\n\rEdit request ended.");
							return;
						}
					}

					var requestType = aliasName + "=True&";

					if (aliasName == "Return to Bulk")
					{
						requestType = "FillStand=True&";
					}

					sFeatures = "dialogWidth: 800px; dialogHeight: 480px";
					window.window_showModalDialog("FuelRequestForm.aspx?" + requestType + "TransactionGuid=" + transactionGuid, "", sFeatures);
				}
			}
		}
	});

	TabularViewLib.grid.onContextMenu.subscribe(function (e) {
		e.preventDefault();
		var cell = TabularViewLib.grid.getCellFromEvent(e);

		// Check to see if we need to select the row based on the row that was right-clicked on.  If the row is not one of the
		// ones in the current row selection, select the row that was clicked on.
		TabularViewLib.CheckGridRowSelection(cell);

		var tableTop = $("#tabularViewPanel").offset().top;

		TabularViewLib.SetPopupMenuEnableDisable();

		$("#contextMenu").data("row", cell.row)
			.css("top", e.pageY - tableTop)
			.css("left", e.pageX)
			.show();

		$("body").one("click", function () { $("#contextMenu").hide(); });
	});

	TabularViewLib.view.onRowCountChanged.subscribe(function (e, args) {
		if (TabularViewLib.initialized) {
			TabularViewLib.grid.updateRowCount();
			TabularViewLib.grid.render();
		}
	});

	TabularViewLib.view.onRowsChanged.subscribe(function (e, args) {
		if (TabularViewLib.initialized) {
			TabularViewLib.grid.invalidateRows(args.rows);
			TabularViewLib.grid.render();
		}
	});

	$("#contextMenu").click(function (e) {
		if (!$(e.target).is("li")) {
			return;
		}

		// Don't respond if the menu item is disabled
		if (e.target.disabled) {
			return;
		}

		switch ($(e.target).attr("data")) {
			case "Arrived":
				TabularViewLib.ArrivalButtonOnClick();
				break;
			case "Started":
				TabularViewLib.StartOfServiceButtonOnClick();
				break;
			case "Stopped":
				TabularViewLib.StopOfServiceButtonOnClick();
				break;
			case "Completed":
				TabularViewLib.ServiceCompletionButtonOnClick();
				break;
			case "FillstandComplete":
				TabularViewLib.FillstandCompletionButtonOnClick();
				break;
			case "Relog":
				TabularViewLib.RelogButtonOnClick();
				break;
			case "Cancel":
				TabularViewLib.CancelButtonOnClick();
				break;
			case "Uncancel":
				TabularViewLib.UncancelButtonOnClick();
				break;
			case "QualityResults":
				window.window_location_assign("../QualityControlWebApp/TestResults.aspx");
				break;
			case "TrainingAssignments":
				window.window_location_assign("../TrainingWebApp/TrainingAssignments.aspx");
				break;
			default:
				var row = $(this).data("row");
				console.log("Context Menu: " + TabularViewLib.data[row].TransId + " - " + $(e.target).attr("data"));
		}

	});
};

TabularViewLib.CheckGridRowSelection = function(cell) {
	// Is the row already part of the selection?
	if (TabularViewLib.AreGridRowsSelected()) {
		var numRows = TabularViewLib.selectedRows.length;
		for (var index = 0; index < numRows; ++index) {
			if (TabularViewLib.selectedRows[index] == cell.row) {
				return;
			}
		}
	}

	// If not, make this row the only selected row.
	var rows = [];
	rows.push(cell.row);
	TabularViewLib.grid.setSelectedRows(rows);
};

TabularViewLib.AreGridRowsSelected = function() {
	return !(TabularViewLib.selectedRows == undefined || TabularViewLib.selectedRows.length == 0);
};

TabularViewLib.SetPopupMenuEnableDisable = function ()
{

	// Start with all enabled
	$("#ArrivedItem")[0].disabled = false;
	$("#StartedItem")[0].disabled = false;
	$("#StoppedItem")[0].disabled = false;
	$("#CompletedItem")[0].disabled = false;
	$("#FillstandCompleteItem")[0].disabled = false;
	$("#RelogItem")[0].disabled = false;
	$("#CancelItem")[0].disabled = false;
	$("#UncancelItem")[0].disabled = false;
	$("#QualityResultsItem")[0].disabled = false;
	$("#TrainingAssignmentsItem")[0].disabled = false;

	var anyPostedSelected = false;
	var anyDispatched = false;
	var anyStarted = false;
	var anyStopped = false;
	var anyArrived = false;
	var allCompleteCapable = true;
	var allFillstandCapable = true;
	var refuelDefuelSelected = true;
	var allCancelCapable = true;
	var anyRecirculation = false;
	var anyCompletedCanceled = false;
	var anyRequested = false;
	var allUncancelCapable = true;

	// Are there any rows selected on the grid?
	var areRowsSelected = TabularViewLib.AreGridRowsSelected();

	if (areRowsSelected)
	{
		var numRows = TabularViewLib.selectedRows.length;
		for (var index = 0; index < numRows; ++index)
		{
			// Check the alias name
			var selectedRowNumber = TabularViewLib.selectedRows[index];

			var rowData = TabularViewLib.data[selectedRowNumber];

			var transType = rowData.TransType;

			// Recirculation
			if (transType == '12')
			{
				allCompleteCapable = false;
				allFillstandCapable = false;

				if (rowData.Status == 'Cancelled' || rowData.Status == 'Completed')
				{
					allCancelCapable = false;
					anyCompletedCanceled = true;
				}

				anyRecirculation = true;
			} else
			{
				if (rowData.Status == 'Requested' || rowData.Status == 'Scheduled')
				{
					anyRequested = true;
				}

				if (rowData.Status == 'Dispatched')
				{
					anyDispatched = true;
				}

				if (rowData.Status == 'Arrived')
				{
					anyArrived = true;
				}

				if (rowData.Status == 'Started')
				{
					anyStarted = true;
				}

				if (rowData.Status == 'Stopped')
				{
					anyStopped = true;
				}

				if (rowData.Status == 'Posted')
				{
					anyPostedSelected = true;
					break;
				}

				// Fillstand or Return To Bulk
				if (transType == '7' || transType == '10')
				{
					allCompleteCapable = false;
				} else
				{
					allFillstandCapable = false;
				}

				if (rowData.Status == 'Cancelled' || rowData.Status == 'Completed')
				{
					allCancelCapable = false;
					allCompleteCapable = false;
					allFillstandCapable = false;
					anyCompletedCanceled = true;
				}

				if (rowData.Status != 'Cancelled')
				{
					allUncancelCapable = false;
				}

				if (transType != '4' && transType != '6')
				{
					refuelDefuelSelected = false;
				}
			}
		}
	}

	// If any posted items are selected, disable all menu items.
	if (anyPostedSelected)
	{
		$("#ArrivedItem")[0].disabled = true;
		$("#StartedItem")[0].disabled = true;
		$("#StoppedItem")[0].disabled = true;
		$("#CompletedItem")[0].disabled = true;
		$("#FillstandCompleteItem")[0].disabled = true;
		$("#RelogItem")[0].disabled = true;
		$("#CancelItem")[0].disabled = true;
		$("#UncancelItem")[0].disabled = true;
	}
	else
	{
		TabularViewLib.useArrivalTime = false;
		TabularViewLib.useStartTime = false;
		TabularViewLib.useStopTime = false;

		if (TabularViewLib.jsonOptionalTimesArrivalFlagValue == 'T')
		{
			TabularViewLib.useArrivalTime = true;
		}

		if (TabularViewLib.jsonOptionalTimesStartFlagValue == 'T')
		{
			TabularViewLib.useStartTime = true;
		}

		if (TabularViewLib.jsonOptionalTimesStopFlagValue == 'T')
		{
			TabularViewLib.useStopTime = true;
		}

		if (TabularViewLib.useArrivalTime == false && TabularViewLib.useStartTime == false && TabularViewLib.useStopTime == false)
		{
			$("#ArrivedItem")[0].disabled = true;
			$("#StartedItem")[0].disabled = true;
			$("#StoppedItem")[0].disabled = true;

			if (anyRequested == false && allCompleteCapable)
			{
				$("#CompletedItem")[0].disabled = false;
			} else
			{
				$("#CompletedItem")[0].disabled = true;
			}

			if (anyRequested == false && allFillstandCapable)
			{
				$("#FillstandCompleteItem")[0].disabled = false;
			} else
			{
				$("#FillstandCompleteItem")[0].disabled = true;
			}
		} else if (TabularViewLib.useArrivalTime == true && TabularViewLib.useStartTime == false && TabularViewLib.useStopTime == false)
		{
			$("#ArrivedItem")[0].disabled = (anyRequested || anyArrived || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);
			$("#StartedItem")[0].disabled = true;
			$("#StoppedItem")[0].disabled = true;

			if (!(anyRequested || anyDispatched) && allCompleteCapable)
			{
				$("#CompletedItem")[0].disabled = false;
			} else
			{
				$("#CompletedItem")[0].disabled = true;
			}

			if (!(anyRequested || anyDispatched) && allFillstandCapable)
			{
				$("#FillstandCompleteItem")[0].disabled = false;
			} else
			{
				$("#FillstandCompleteItem")[0].disabled = true;
			}
		} else if (TabularViewLib.useArrivalTime == false && TabularViewLib.useStartTime == true && TabularViewLib.useStopTime == false)
		{
			$("#ArrivedItem")[0].disabled = true;
			$("#StartedItem")[0].disabled = (anyRequested || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);
			$("#StoppedItem")[0].disabled = true;

			if (!(anyRequested || anyDispatched) && allCompleteCapable)
			{
				$("#CompletedItem")[0].disabled = false;
			} else
			{
				$("#CompletedItem")[0].disabled = true;
			}

			if (!(anyRequested || anyDispatched) && allFillstandCapable)
			{
				$("#FillstandCompleteItem")[0].disabled = false;
			} else
			{
				$("#FillstandCompleteItem")[0].disabled = true;
			}
		} else if (TabularViewLib.useArrivalTime == false && TabularViewLib.useStartTime == false && TabularViewLib.useStopTime == true)
		{
			$("#ArrivedItem")[0].disabled = true;
			$("#StartedItem")[0].disabled = true;
			$("#StoppedItem")[0].disabled = (anyRequested || anyStopped || anyCompletedCanceled || anyRecirculation);

			if (!(anyRequested || anyDispatched) && allCompleteCapable)
			{
				$("#CompletedItem")[0].disabled = false;
			} else
			{
				$("#CompletedItem")[0].disabled = true;
			}

			if (!(anyRequested || anyDispatched) && allFillstandCapable)
			{
				$("#FillstandCompleteItem")[0].disabled = false;
			} else
			{
				$("#FillstandCompleteItem")[0].disabled = true;
			}
		} else if (TabularViewLib.useArrivalTime == true && TabularViewLib.useStartTime == true && TabularViewLib.useStopTime == false)
		{
			$("#ArrivedItem")[0].disabled = (anyRequested || anyArrived || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);
			$("#StartedItem")[0].disabled = (anyRequested || anyDispatched || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);
			$("#StoppedItem")[0].disabled = true;

			if (!(anyRequested || anyDispatched || anyArrived) && allCompleteCapable)
			{
				$("#CompletedItem")[0].disabled = false;
			} else
			{
				$("#CompletedItem")[0].disabled = true;
			}

			if (!(anyRequested || anyDispatched || anyArrived) && allFillstandCapable)
			{
				$("#FillstandCompleteItem")[0].disabled = false;
			} else
			{
				$("#FillstandCompleteItem")[0].disabled = true;
			}
		} else if (TabularViewLib.useArrivalTime == true && TabularViewLib.useStartTime == false && TabularViewLib.useStopTime == true)
		{
			$("#ArrivedItem")[0].disabled = (anyRequested || anyArrived || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);
			$("#StartedItem")[0].disabled = true;
			$("#StoppedItem")[0].disabled = (anyRequested || anyDispatched || anyStopped || anyCompletedCanceled || anyRecirculation);

			if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allCompleteCapable)
			{
				$("#CompletedItem")[0].disabled = false;
			} else
			{
				$("#CompletedItem")[0].disabled = true;
			}

			if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allFillstandCapable)
			{
				$("#FillstandCompleteItem")[0].disabled = false;
			} else
			{
				$("#FillstandCompleteItem")[0].disabled = true;
			}
		} else if (TabularViewLib.useArrivalTime == false && TabularViewLib.useStartTime == true && TabularViewLib.useStopTime == true)
		{
			$("#ArrivedItem")[0].disabled = true;
			$("#StartedItem")[0].disabled = (anyRequested || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);
			$("#StoppedItem")[0].disabled = (anyRequested || anyDispatched || anyStopped || anyCompletedCanceled || anyRecirculation);

			if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allCompleteCapable)
			{
				$("#CompletedItem")[0].disabled = false;
			} else
			{
				$("#CompletedItem")[0].disabled = true;
			}

			if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allFillstandCapable)
			{
				$("#FillstandCompleteItem")[0].disabled = false;
			} else
			{
				$("#FillstandCompleteItem")[0].disabled = true;
			}
		} else if (TabularViewLib.useArrivalTime == true && TabularViewLib.useStartTime == true && TabularViewLib.useStopTime == true)
		{
			$("#ArrivedItem")[0].disabled = (anyRequested || anyArrived || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);
			$("#StartedItem")[0].disabled = (anyRequested || anyDispatched || anyStarted || anyStopped || anyCompletedCanceled || anyRecirculation);
			$("#StoppedItem")[0].disabled = (anyRequested || anyDispatched || anyArrived || anyStopped || anyCompletedCanceled || anyRecirculation);

			if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allCompleteCapable)
			{
				$("#CompletedItem")[0].disabled = false;
			} else
			{
				$("#CompletedItem")[0].disabled = true;
			}

			if (!(anyRequested || anyDispatched || anyArrived || anyStarted) && allFillstandCapable)
			{
				$("#FillstandCompleteItem")[0].disabled = false;
			} else
			{
				$("#FillstandCompleteItem")[0].disabled = true;
			}
		}

		$("#RelogItem")[0].disabled = !refuelDefuelSelected;
		$("#CancelItem")[0].disabled = !allCancelCapable;
		$("#UncancelItem")[0].disabled = !allUncancelCapable;
	}

	// Set style of any disabled items
	TabularViewLib.DisableContextMenuItem($("#ArrivedItem"));
	TabularViewLib.DisableContextMenuItem($("#StartedItem"));
	TabularViewLib.DisableContextMenuItem($("#StoppedItem"));
	TabularViewLib.DisableContextMenuItem($("#CompletedItem"));
	TabularViewLib.DisableContextMenuItem($("#FillstandCompleteItem"));
	TabularViewLib.DisableContextMenuItem($("#RelogItem"));
	TabularViewLib.DisableContextMenuItem($("#CancelItem"));
	TabularViewLib.DisableContextMenuItem($("#UncancelItem"));
	TabularViewLib.DisableContextMenuItem($("#QualityResultsItem"));
	TabularViewLib.DisableContextMenuItem($("#TrainingAssignmentsItem"));
};

TabularViewLib.DisableContextMenuItem = function(item) {
	if (item[0].disabled) {
		item.css("color", "darkgray");
		item.css("cursor", "default");
	} else {
		item.css("color", "black");
		item.css("cursor", "pointer");
	}
};

TabularViewLib.tabularPageLoad = function () {
	TabularViewLib.refreshData();
	
	window.FMMenuBarLib.showFullScreenButton();
	window.FMMenuBarLib.onSizeChanged = TabularViewLib.onMenuBarSizeChanged;

	if (!DispatchLib.displayCurrentTime || window.FMMenuBarLib.inFullScreenMode)
	{
		$('#currentTime').hide();
		$('#currentDate').hide();
	} else
	{
		// Set time immediately so view initially resizes correctly
		TabularViewLib.updateTime(new Date());
	}

	// Bind the window resize event to TabularViewLib.resizeTabularView() function
	$(window).resize(TabularViewLib.resizeTabularView);

	// Bind various focus handlers for use in tab key navgation
	TabularViewLib.bindFocusHandlers();

	// Bind the keydown handler for use in tab key navgation
	$(document).keydown(TabularViewLib.customTabKeyPocessing);

	TabularViewLib.initializeDateControls();
	TabularViewLib.initializeStatusControl();
	TabularViewLib.initializeRequestTypeControl();

	TabularViewLib.loadGridSettings();
	TabularViewLib.selectedRows = JSON.parse(window.sessionStorage.tabularGridSelectedRows);

	TabularViewLib.grid = undefined;
	TabularViewLib.data = undefined;
	TabularViewLib.updateGrid();

	setUpHotKeys();

	$('#begindate').focus();
	$('#begindate').select();
};

TabularViewLib.onMenuBarSizeChanged = function ()
{
	if (window.FMMenuBarLib.inFullScreenMode)
	{
		$('#currentTime').hide();
		$('#currentDate').hide();
	}
	else
	{
		$('#currentTime').show();
		$('#currentDate').show();
	}
	
	TabularViewLib.resizeTabularView();
};

TabularViewLib.initializeRequestTypeControl = function() {
	try {
		var aliasNames = JSON.parse(TabularViewLib.jsonTransactionAliasNames);
	} catch (err) {
		aliasNames = [];
	}
	
	// Set the request type select options
	for (var i = 0; i < aliasNames.length; i++) {
		if (aliasNames[i] != '') {
			$('#RequestTypeSelect').append(new Option(aliasNames[i], aliasNames[i]));
		}
	}

	if (window.sessionStorage.requestTypeFilter != undefined) {
		if (window.sessionStorage.requestTypeFilter == '') {
			$('#RequestTypeSelect').val('{All}');
		} else {
			$('#RequestTypeSelect').val(window.sessionStorage.requestTypeFilter);
		}
	}

	$('#RequestTypeSelect').change(function() {
		window.sessionStorage.requestTypeFilter = $('#RequestTypeSelect option:selected').val();
		if (window.sessionStorage.requestTypeFilter == '{All}') {
			window.sessionStorage.requestTypeFilter = '';
		}
		
		TabularViewLib.ClearGrid();
		TabularViewLib.refreshData();
	});
};

TabularViewLib.initializeDateControls = function() {
	// Set up the date range pickers, default to today's date, and save changes to session storage.
	$("#begindate").datepicker({
		onSelect: function (selectedDate) {
			var newDate = new Date(selectedDate).toLocaleDateString();
			if (window.sessionStorage.beginDateFilter != newDate) {
				$("#enddate").datepicker("option", "minDate", selectedDate);
				window.sessionStorage.beginDateFilter = newDate;
				TabularViewLib.ClearGrid();
				TabularViewLib.refreshData();
			}
		},
		buttonImage: "images/calendar.gif",
		buttonImageOnly: true,
		showOn: "button"
	});

	if (window.sessionStorage.beginDateFilter != undefined) {
		$("#begindate").datepicker("setDate", new Date(window.sessionStorage.beginDateFilter));
	} else {
		$("#begindate").datepicker("setDate", new Date);
	}

	$("#enddate").datepicker({
		onSelect: function(selectedDate) {
			var newDate = new Date(selectedDate).toLocaleDateString();
			if (window.sessionStorage.endDateFilter != newDate) {
				$("#begindate").datepicker("option", "maxDate", selectedDate);
				window.sessionStorage.endDateFilter = newDate;
				TabularViewLib.ClearGrid();
				TabularViewLib.refreshData();
			}
		},
		buttonImage: "images/calendar.gif",
		buttonImageOnly: true,
		showOn: "button"
	});

	if (window.sessionStorage.endDateFilter != undefined) {
		$("#enddate").datepicker("setDate", new Date(window.sessionStorage.endDateFilter));
	} else {
		$("#enddate").datepicker("setDate", new Date);
	}
};

TabularViewLib.initializeStatusControl = function() {
	try {
		var statusValues = JSON.parse(TabularViewLib.jsonTransactionStatusValues);
	} catch (err) {
		statusValues = [];
	}

	// Set the status select options
	for (var i = 0; i < statusValues.length; i++) {
		if (statusValues[i] != '') {
			$('#StatusSelect').append(new Option(statusValues[i], statusValues[i]));
		}
	}

	if (window.sessionStorage.statusFilter != undefined) {
		if (window.sessionStorage.statusFilter == '') {
			$('#StatusSelect').val('{All}');
		} else {
			$('#StatusSelect').val(window.sessionStorage.statusFilter);
		}
	}

	$('#StatusSelect').change(function() {
		window.sessionStorage.statusFilter = $('#StatusSelect option:selected').val();
		if (window.sessionStorage.statusFilter == '{All}') {
			window.sessionStorage.statusFilter = '';
		}

		TabularViewLib.ClearGrid();
		TabularViewLib.refreshData();
	});
};

TabularViewLib.filterOnVehicle = function() {
	var value = $('#VehicleSelect option:selected').val();

	if (value == '{All}') {
		value = '';
	}

	TabularViewLib.vehicleFilter = value;

	TabularViewLib.view.setFilterArgs({ vehicleID: TabularViewLib.vehicleFilter });
	TabularViewLib.view.refresh();
};

TabularViewLib.updateGrid = function () {
	var newGridCreated = false;

	if (TabularViewLib.grid == undefined) {
		TabularViewLib.createGrid();
		newGridCreated = true;
	}

	// Turn off selection handling while we populate the list
	$('#VehicleSelect').change(function () {
		// Do nothing
	});

	var translatedAll;
	var firstOption = $('#VehicleSelect')[0].options[0];
	if (firstOption) {
		translatedAll = firstOption.text;
	} else {
		translatedAll = '{All}';
	}

	$('#VehicleSelect option').remove();
	$('#VehicleSelect').append($("<option />").val('{All}').text(translatedAll));

	var count = 0;
	TabularViewLib.data = [];
	if (window.FuelsManagerServiceLib.requestsDataFM) {
		TabularViewLib.data = window.FuelsManagerServiceLib.requestsDataFM;
		count = TabularViewLib.data.length;
	}

	// Set the vehicle select options
	for (var i = 0; i < count; i++) {
		if (TabularViewLib.data[i].IssuePointNumber != "" ) {
			var xref = TabularViewLib.data[i].IssuePointNumber;
			// Don't add value if it already exists
			if (!$("#VehicleSelect option[value='" + xref + "']").length) {
				$('#VehicleSelect').append(new Option(xref, xref, true, true));
			}
		}
	}

	$('#VehicleSelect').val('{All}');

	$('#VehicleSelect').change(function () {
		TabularViewLib.filterOnVehicle();
	});

	// Set and sort the grid data.
	TabularViewLib.grid.invalidateAllRows();
	TabularViewLib.view.beginUpdate();
	TabularViewLib.view.setItems(TabularViewLib.data, 'LineItemGuid');
	TabularViewLib.view.fastSort(TabularViewLib.gridSettings.sortColumn, TabularViewLib.gridSettings.sortAscending);
	TabularViewLib.setGridRowNumber(TabularViewLib.data, count);
	TabularViewLib.view.setFilterArgs({ vehicleID: TabularViewLib.vehicleFilter });
	TabularViewLib.view.setFilter(TabularViewLib.vehicleFilterFunction);
	TabularViewLib.view.endUpdate();

	if (TabularViewLib.selectedRows.length > 0 && TabularViewLib.data.length > TabularViewLib.selectedRows[length]) {
		TabularViewLib.grid.setSelectedRows(TabularViewLib.selectedRows);
		TabularViewLib.grid.scrollRowIntoView(TabularViewLib.selectedRows[0], false);
	}

	if (newGridCreated) {
		// Resize the grid
		TabularViewLib.resizeTabularView();
	} else {
		// Draw the grid
		TabularViewLib.grid.render();
	}

	TabularViewLib.initialized = true;
};

TabularViewLib.vehicleFilterFunction = function(item, args) {
	if (args.vehicleID != '' && item.IssuePointNumber != args.vehicleID) {
		return false;
	}

	return true;
};

TabularViewLib.updateTime = function(newTime) {
	if (DispatchLib.displayCurrentTime && !window.FMMenuBarLib.inFullScreenMode) {
		$("#currentTime").text(newTime.toLocaleTimeString());
		if (DispatchLib.displayMilitaryJulianDate) {
			$("#currentDate").text(DispatchLib.militaryJulianDate(newTime));
		} else {
			$("#currentDate").text(newTime.toLocaleDateString());
		}
	}
};

TabularViewLib.refreshData = function(currentTime) {
	var dispatchRequest = {};

	var newTime = currentTime;
	if (currentTime == undefined) {
		dispatchRequest.topTransactionVersion = 0;
		newTime = new Date();
	} else {
		dispatchRequest.topTransactionVersion = window.FuelsManagerServiceLib.topTransactionVersion;
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
		(currentTime - TabularViewLib.refreshTime) > window.FuelsManagerServiceLib.serviceRequestRefreshPeriod * 1000) {
		if (window.FuelsManagerServiceLib.enableServiceRequests &&
			!window.FuelsManagerServiceLib.serviceRequestsStopped) {

			// Get the saved filter values from session storage
			dispatchRequest.beginDate = window.sessionStorage.beginDateFilter;
			dispatchRequest.endDate = window.sessionStorage.endDateFilter;
			dispatchRequest.status = window.sessionStorage.statusFilter;
			dispatchRequest.alias = window.sessionStorage.requestTypeFilter;

			// Call jQuery Ajax interface to FuelsManager
			dispatchRequest.securityToken = TabularViewLib.securityToken;
			dispatchRequest.siteGuid = TabularViewLib.siteGuid;

			window.FuelsManagerServiceLib.CallDispatchRequestEnumerateTransactions(dispatchRequest);
		}

		TabularViewLib.refreshTime = newTime;
	}
};

TabularViewLib.resizeTabularView = function () {
	var panelElem = $('#tabularViewPanel');
	if (panelElem) {
		// Limit minimum panel width to the width of menu header bar
		var headerBarWidth = window.FMMenuBarLib.headerBarWidth();
		var widthoffset = window.FMMenuBarLib.inFullScreenMode ? 15 : 18;
		var panelWidth = Math.max($(window).width() - widthoffset, headerBarWidth - widthoffset);
		panelElem.width(panelWidth);

		// Limit minimum panel height to 300 pixels
		var menuBarHeight = window.FMMenuBarLib.clientHeight();
		var heightoffset = menuBarHeight + (window.FMMenuBarLib.inFullScreenMode ? 18 : 3);
		var panelHeight = Math.max($(window).height() - heightoffset, 300);
		panelElem.height(panelHeight);

		var gridElem = $('#gridTabular');
		if (gridElem && TabularViewLib.grid) {
			// Compute total width of grid columns
			var columns = TabularViewLib.grid.getColumns();
			var gridWidth = 0;
			for (var i = 0; i < columns.length; i++) {
				gridWidth += columns[i].width;
			}
			// Limit maximum grid width based on width of grid columns
			gridElem.width(Math.min(panelWidth - 5, gridWidth + 17));

			// Limit grid height based on panel height and the remaining panel element heights
			var currentTimeHeight = window.FMMenuBarLib.inFullScreenMode ? 0 : $('#currentTime').height();
			var toolBarTabularHeight = $('#toolBarTabular').height();
			var gridHeaderHeight = $('#gridHeader').height();
			var gridHeight = panelHeight - currentTimeHeight - toolBarTabularHeight - gridHeaderHeight - 5;
			gridElem.height(gridHeight);

			TabularViewLib.grid.resizeCanvas();
		}
	}
};

// Called when the Arrival button is clicked.
TabularViewLib.ArrivalButtonOnClick = function ()
{
	var numRows = TabularViewLib.selectedRows.length;
	if (numRows < 1)
	{
		return;
	}

	// Check for Dispatched transactions
	for (var index = 0; index < numRows; ++index)
	{
		var rowNum = TabularViewLib.selectedRows[index];
		if (TabularViewLib.data[rowNum].Status != "Dispatched")
		{
			alert("Request must be in Dispatached status first!  Operation will be aborted!");
			return;
		}
	}

	var arrivedRequest = {};
	arrivedRequest.transactionIds = [];
	arrivedRequest.lineItemGuids = [];
	for (index = 0; index < numRows; ++index)
	{
		rowNum = TabularViewLib.selectedRows[index];
		arrivedRequest.transactionIds.push(TabularViewLib.data[rowNum].TransId);
		arrivedRequest.lineItemGuids.push(TabularViewLib.data[rowNum].LineItemGuid);
	}

	if (arrivedRequest.transactionIds.length > 0)
	{
		// Call jQuery Ajax interface to FuelsManager
		arrivedRequest.securityToken = TabularViewLib.securityToken;
		arrivedRequest.siteGuid = TabularViewLib.siteGuid;
		window.FuelsManagerServiceLib.CallDispatchRequestSetArrived(arrivedRequest);
	}
};

// Called when the Control Log button is clicked.  Navigates to the Control Log
// page.  The navigateAction parameter is passed to the page as a query string.
TabularViewLib.ControlLogButtonOnClick = function() {
	var queryString = "?navigateAction=openClick";
	window.window_location_assign("ControlLogForm.aspx" + queryString);
};

// Called when the Dispatching View button is clicked.
TabularViewLib.DispatchButtonOnClick = function ()
{
	var transId = '';

	if (TabularViewLib.selectedRows != undefined
		&& TabularViewLib.selectedRows.length > 0
		&& TabularViewLib.data[TabularViewLib.selectedRows[0]].TransId != undefined)
	{
		transId = TabularViewLib.data[TabularViewLib.selectedRows[0]].TransId;
	}

	window.window_location_assign("DispatchingView.aspx?transId=" + transId);
};

// Called when the Dispatchers List button is clicked.  Navigates to the Dispatchers List
// page.  The navigateAction parameter is passed to the page as a query string.
TabularViewLib.DispatchersListButtonOnClick = function () {
	var queryString = "?navigateAction=openClick";
	window.window_location_assign("ListOfDispatchers.aspx" + queryString);
};

// Called when the Evacuate button is clicked.  Navigates to the Dispatch Evacuate
// page.  The navigateAction parameter is passed to the page as a query string.
TabularViewLib.EvacuateButtonOnClick = function () {
	var queryString = "?navigateAction=openClick";
	window.window_location_assign("DispatchEvacuatePage.aspx" + queryString);
};

// Called when the Release To Accounting button is clicked.
TabularViewLib.ReleaseToAccountingButtonOnClick = function () {
	var sFeatures = "dialogWidth: 370px; dialogHeight: 450px";
	window.window_showModalDialog("ReleaseToAccountingForm.aspx", "", sFeatures);
};

// Called when the Fast Log button is clicked.
TabularViewLib.FastLogButtonOnClick = function() {
	var sFeatures = "dialogWidth: 800px; dialogHeight: 480px";
	window.window_showModalDialog("FuelRequestForm.aspx?FastLog=true", "", sFeatures);
};

// Called when the Fast Log Fillstand button is clicked.
TabularViewLib.FastLogFillstandButtonOnClick = function() {
	var sFeatures = "dialogWidth: 800px; dialogHeight: 480px";
	window.window_showModalDialog("FuelRequestForm.aspx?FastLogFillStand=true", "", sFeatures);
};

// Called when the Fillstand Completion button is clicked.
TabularViewLib.FillstandCompletionButtonOnClick = function ()
{
	var numRows = TabularViewLib.selectedRows.length;

	if (numRows < 1)
	{
		return;
	}

	TabularViewLib.useArrivalTime = false;
	TabularViewLib.useStartTime = false;
	TabularViewLib.useStopTime = false;

	if (TabularViewLib.jsonOptionalTimesArrivalFlagValue == 'T')
	{
		TabularViewLib.useArrivalTime = true;
	}

	if (TabularViewLib.jsonOptionalTimesStartFlagValue == 'T')
	{
		TabularViewLib.useStartTime = true;
	}

	if (TabularViewLib.jsonOptionalTimesStopFlagValue == 'T')
	{
		TabularViewLib.useStopTime = true;
	}

	var transactionGuid = '';

	// Check for Fillstand or Defuel transaction types and Stopped transaction status
	for (var index = 0; index < numRows; ++index)
	{
		var rowNum = TabularViewLib.selectedRows[index];

		if (TabularViewLib.data[rowNum].TransType != '3' &&		// T3_PrimaryDefuel
			TabularViewLib.data[rowNum].TransType != '7' &&		// T7_Fillstand
			TabularViewLib.data[rowNum].TransType != '10')		// T10_Unload
		{		
			alert("Request Type must be Fillstand or Defuel!  Operation will be aborted!");
			return;
		}

		// Do not have to check for status when all optional times are
		// inhibited
		if (TabularViewLib.data[rowNum].Status == "Arrived")
		{
			if (TabularViewLib.useStartTime == false &&
				TabularViewLib.useStopTime == false)
			{
				transactionGuid = TabularViewLib.data[rowNum].TransactionGuid;
				break; // Currently the Fuel Request Form supports processing only a single transaction at a time
			}

			if (TabularViewLib.useStartTime)
			{
				alert("Request must be in Started status first!  Operation will be aborted!");
				return;
			}

			if (TabularViewLib.useStopTime)
			{
				alert("Request must be in Stopped status first!  Operation will be aborted!");
				return;
			}
		}

		if (TabularViewLib.data[rowNum].Status == "Started")
		{
			if (TabularViewLib.useStopTime == false)
			{
				transactionGuid = TabularViewLib.data[rowNum].TransactionGuid;
				break; // Currently the Fuel Request Form supports processing only a single transaction at a time
			}

			alert("Request must be in Started status first!  Operation will be aborted!");
			return;
		}

		if (TabularViewLib.data[rowNum].Status == "Stopped")
		{
			transactionGuid = TabularViewLib.data[rowNum].TransactionGuid;
			break; // Currently the Fuel Request Form supports processing only a single transaction at a time
		}
		else
		{
			if (TabularViewLib.useStopTime == false)
			{
				transactionGuid = TabularViewLib.data[rowNum].TransactionGuid;
				break; // Currently the Fuel Request Form supports processing only a single transaction at a time
			}

			alert("Request must be in Stopped status first!  Operation will be aborted!");
			return;
		}
	}

	var sFeatures = "dialogWidth: 800px; dialogHeight: 480px";
	window.window_showModalDialog("FuelRequestForm.aspx?FillStand=true&TransactionGuid=" + transactionGuid + "&CompletionMode=true", "", sFeatures);
};

// Called when the Flight Line Status button is clicked.  Navigates to the Dispatching
// View page.  The dispatchStatus filter parameter is passed to the page as a query string.
TabularViewLib.FlightLineButtonOnClick = function () {
	var queryString = "?dispatchStatus=FlightLine";
	window.window_location_assign("DispatchingView.aspx" + queryString);
};

// Called when the Optional Times button is clicked.
TabularViewLib.OptionalTimesButtonOnClick = function ()
{
	var sFeatures = "dialogWidth: 500px; dialogHeight: 450px; scroll: no";
	window.window_showModalDialog("OptionalTimesPage.aspx", "", sFeatures);

	var request = {};
	request.securityToken = TabularViewLib.securityToken;
	request.siteGuid = TabularViewLib.siteGuid;

	window.FuelsManagerServiceLib.CallDispatchRequestRetrieveOptionalTimes(request);
	//TabularViewLib.SetPopupMenuEnableDisable();
};

// Called when the Recirculation button is clicked.
TabularViewLib.RecirculationButtonOnClick = function() {
	var sFeatures = "dialogWidth: 800px; dialogHeight: 480px";
	window.window_showModalDialog("RecirculationForm.aspx", "", sFeatures);
};

TabularViewLib.ClearGrid = function() {
	window.FuelsManagerServiceLib.requestsDataFM = undefined;
	TabularViewLib.updateGrid();
};

// Called when the Refresh button is clicked.
TabularViewLib.RefreshButtonOnClick = function () {
	if (window.FuelsManagerServiceLib.enableServiceRequests) {
		var dispatchRequest = {};
		dispatchRequest.TopVersion = '0';
		
		TabularViewLib.ClearGrid();

		// Get the saved filter values from session storage
		dispatchRequest.BeginDate = window.sessionStorage.beginDateFilter;
		dispatchRequest.EndDate = window.sessionStorage.endDateFilter;
		dispatchRequest.Status = window.sessionStorage.statusFilter;
		dispatchRequest.requestName = window.sessionStorage.requestTypeFilter;

		TabularViewLib.CallServer(JSON.stringify(dispatchRequest));
	}
};

// Callback from server after Refresh button is clicked.
TabularViewLib.ReceiveServerData = function (returnValue) {
	var result = JSON.parse(returnValue);
	if (result.Refreshed) {
		window.FuelsManagerServiceLib.requestsDataFM = result.Transactions;
		TabularViewLib.updateGrid();
	}
};

// Called when the Request button is clicked.
TabularViewLib.RequestButtonOnClick = function() {
	var sFeatures = "dialogWidth: 800px; dialogHeight: 480px";
	window.window_showModalDialog("FuelRequestForm.aspx", "", sFeatures);
};

// Called when the Service Completion button is clicked.
TabularViewLib.ServiceCompletionButtonOnClick = function() {
	var numRows = TabularViewLib.selectedRows.length;
	if (numRows < 1)
	{
		return;
	}

	TabularViewLib.useArrivalTime = false;
	TabularViewLib.useStartTime = false;
	TabularViewLib.useStopTime = false;

	if (TabularViewLib.jsonOptionalTimesArrivalFlagValue == 'T')
	{
		TabularViewLib.useArrivalTime = true;
	}

	if (TabularViewLib.jsonOptionalTimesStartFlagValue == 'T')
	{
		TabularViewLib.useStartTime = true;
	}

	if (TabularViewLib.jsonOptionalTimesStopFlagValue == 'T')
	{
		TabularViewLib.useStopTime = true;
	}

	var transactionGuid = '';

	// Check for Fillstand or Return to Bulk transaction types and Stopped transactions
	for (var index = 0; index < numRows; ++index)
	{
		var rowNum = TabularViewLib.selectedRows[index];

		if (TabularViewLib.data[rowNum].TransType == '7' ||		// T7_Fillstand
			TabularViewLib.data[rowNum].TransType == '10')		// T10_Unload
		{
			alert("Request Type must not be Fillstand or Return to Bulk!  Operation will be aborted!");
			return;
		}

		// Do not have to check for status when all optional times are
		// inhibited
		if (TabularViewLib.data[rowNum].Status == "Arrived")
		{
			if (TabularViewLib.useStartTime == false &&
				TabularViewLib.useStopTime == false)
			{
				transactionGuid = TabularViewLib.data[rowNum].TransactionGuid;
				break; // Currently the Fuel Request Form supports processing only a single transaction at a time
			}

			if (TabularViewLib.useStartTime)
			{
				alert("Request must be in Started status first!  Operation will be aborted!");
				return;
			}

			if (TabularViewLib.useStopTime)
			{
				alert("Request must be in Stopped status first!  Operation will be aborted!");
				return;
			}
		}

		if (TabularViewLib.data[rowNum].Status == "Started")
		{
			if (TabularViewLib.useStopTime == false)
			{
				transactionGuid = TabularViewLib.data[rowNum].TransactionGuid;
				break; // Currently the Fuel Request Form supports processing only a single transaction at a time
			}

			alert("Request must be in Stopped status first!  Operation will be aborted!");
			return;
		}

		if (TabularViewLib.data[rowNum].Status == "Stopped")
		{
			transactionGuid = TabularViewLib.data[rowNum].TransactionGuid;
			break; // Currently the Fuel Request Form supports processing only a single transaction at a time
		}
		else
		{
			if (TabularViewLib.useStopTime == false)
			{
				transactionGuid = TabularViewLib.data[rowNum].TransactionGuid;
				break; // Currently the Fuel Request Form supports processing only a single transaction at a time
			}

			alert("Request must be in Started status first!  Operation will be aborted!");
			return;
		}
	}

	var sFeatures = "dialogWidth: 800px; dialogHeight: 480px";
	var OkPressed = window.window_showModalDialog("FuelRequestForm.aspx?TransactionGuid=" + transactionGuid + "&CompletionMode=true", "", sFeatures);
	if (OkPressed) //If dispatch was completed, return to the dispatch screen
	{
		TabularViewLib.DispatchButtonOnClick();
	}
};

TabularViewLib.rowFormatterOperatorStatusGrid = function (row, cell, value) {
    if (value == undefined) {
        value = "";
    }

    if (row >= window.FuelsManagerServiceLib.operatorStatusDataFM.length) {
        return value;
    }

    var rowData = window.FuelsManagerServiceLib.operatorStatusDataFM[row];
    return "<span style='color: " + rowData.ForeColor + "'>" + value + "</span>";
};

// Display the Standby Status Board with Standby Personnel results.
TabularViewLib.DisplayOperatorStatusBoard = function () {
    if (window.FuelsManagerServiceLib.operatorStatusDataFM) {
        var operatorStatusDataFM = window.FuelsManagerServiceLib.operatorStatusDataFM;

        // Create standby status board grid
        var grid;
        var columns = [
            { id: "Personnel", name: $('#operatorLabel').text(), field: "FullName", minWidth: 180, width: 200, formatter: TabularViewLib.rowFormatterOperatorStatusGrid },
            { id: "StatusCode", name: $('#statusLabel').text(), field: "StatusCode", minWidth: 180, width: 100, formatter: TabularViewLib.rowFormatterOperatorStatusGrid },
            { id: "Equipment", name: $('#equipmentLabel').text(), field: "EquipmentID", minWidth: 180, width: 200, formatter: TabularViewLib.rowFormatterOperatorStatusGrid }
        ];

        var options = {
            editable: false,
            enableAddRow: false,
            enableCellNavigation: true,
            enableColumnReorder: false,
            forceFitColumns: false,
            topPanelHeight: 15,
            rowHeight: 30,
            autoHeight: true
        };

        grid = new Slick.Grid("#gridStatus", operatorStatusDataFM, columns, options);

        grid.onSelectedRowsChanged.subscribe(function(e, args) {
            if (args != null & args != undefined) {
                TabularViewLib.OperatorStatusSelectedRows = args.rows;
            }
        });

        grid.setSelectionModel(new Slick.RowSelectionModel());
    }
};

TabularViewLib.SelectRegistrationOK = function() {
    $(this).dialog('close');

    var equipmentIndex = $("#RegistrationSelect option:selected").val();

    if (equipmentIndex >= 0 && equipmentIndex < FuelsManagerServiceLib.equipmentStatusDataFM.length) {

        var record = FuelsManagerServiceLib.equipmentStatusDataFM[equipmentIndex];

        var personGuidText = $("#EmployeeGuid").val();
        var assignedEquipmentGuidText = $("#AssignedEquipmentGuid").val();
        var equipmentGuidText = record.IdentityGuid;
        
        if (assignedEquipmentGuidText != ''
            && assignedEquipmentGuidText != '00000000-0000-0000-0000-000000000000'
            && assignedEquipmentGuidText != equipmentGuidText)
        {
            var fullName = $('#OperatorNameText').val();
            var assignedEquipmentID = $('#EquipmentID').val();
            
            var message = fullName + ' is currently assigned to ' + assignedEquipmentID + '.  Do you wish to reassign ' + fullName + ' to vehicle ' + record.ID;
            
            if (!confirm(message)) {
                return;
            }
        }

        var dataPacket = {
            securityToken: TabularViewLib.securityToken,
            personGuidText: personGuidText,
            equipmentGuidText: equipmentGuidText
        };

        $.ajax({
            url: 'TabularView.aspx/SetOperatorStandbyStatus',
            data: JSON.stringify(dataPacket),
            success: function(data) {
                var operatorStatusRequest = {};
                operatorStatusRequest.securityToken = TabularViewLib.securityToken;
                window.FuelsManagerServiceLib.CallDispatchRequestEnumerateOperatorStatus(operatorStatusRequest);
            },
        });
    }
};

// Called when the Change Operator Status button is clicked.
TabularViewLib.SelectRegistrationID = function (personName, employeeId, employeeGuid, employeeEquipmentGuid, equipmentID) {
    var equipmentRequest = {};
    equipmentRequest.securityToken = TabularViewLib.securityToken;
    //window.FuelsManagerServiceLib.CallDispatchRequestEnumerateOperatorStatus(equipmentRequest);

    var buttonsObj = {};

    buttonsObj[$('#okLabel2').text()] = TabularViewLib.SelectRegistrationOK;

    buttonsObj[$('#cancelLabel2').text()] = function () { $(this).dialog('close'); };

    $('#OperatorNameText').val(personName);
    $('#EmployeeIdText').val(employeeId);
    $('#EmployeeGuid').val(employeeGuid);
    $('#AssignedEquipmentGuid').val(employeeEquipmentGuid);
    $('#EquipmentID').val(equipmentID);
    
    // Create standby status board dialog
    $('#RegistrationSelectForm').dialog(
    {
        autoOpen: false,
        modal: true,
        width: 450,
        height: 300,
        resizable: false,
        buttons: buttonsObj
    });

    $("#RegistrationSelect").empty();
        
    $.each(FuelsManagerServiceLib.equipmentStatusDataFM, function (key, value) {
        $('#RegistrationSelect')
                .append($('<option>', { value: key })
                .text(value.ID));
    });

    // Display results
    $('#RegistrationSelectForm').dialog('open');
};

// Called when the Change Operator Status button is clicked.
TabularViewLib.ChangeOperatorStatusButtonOnClick = function () {
    var operatorStatusRequest = {};
    operatorStatusRequest.securityToken = TabularViewLib.securityToken;
    window.FuelsManagerServiceLib.operatorStatusCallback = TabularViewLib.DisplayOperatorStatusBoard;
    window.FuelsManagerServiceLib.CallDispatchRequestEnumerateOperatorStatus(operatorStatusRequest);

    var buttonsObj = {};

    buttonsObj[$('#closeLabel2').text()] = function () { $(this).dialog('close'); };

    // Create standby status board dialog
    $('#ChangeOperatorStatusDialog').dialog(
    {
        autoOpen: false,
        modal: true,
        width: 675,
        height: 450,
        resizable: false,
        buttons: buttonsObj
    });

    $('#HomeButton').unbind("click");
    $('#HomeButton').click(TabularViewLib.OperatorStatusHomeButtonClick);

    $('#OutButton').unbind("click");
    $('#OutButton').click(TabularViewLib.OperatorStatusOutButtonClick);

    $('#StandButton').unbind("click");
    $('#StandButton').click(TabularViewLib.OperatorStatusStandbyButtonClick);

    // Display results
    $('#ChangeOperatorStatusDialog').dialog('open');
};

TabularViewLib.OperatorStatusHomeButtonClick = function () {
    TabularViewLib.ChangeOperatorStatusData("TabularView.aspx/SetOperatorHomeStatus");
};

TabularViewLib.OperatorStatusOutButtonClick = function () {
    TabularViewLib.ChangeOperatorStatusData("TabularView.aspx/SetOperatorOutStatus");
};

TabularViewLib.OperatorStatusStandbyButtonClick = function () {
    var count = TabularViewLib.OperatorStatusSelectedRows.length;

    if (count > 0) {
        var rowIndex = TabularViewLib.OperatorStatusSelectedRows[0];
        var record = window.FuelsManagerServiceLib.operatorStatusDataFM[rowIndex];
        TabularViewLib.SelectRegistrationID(record.FullName, record.EmployeeID, record.PersonGuid, record.EquipmentGuid, record.EquipmentID);
    }
};

TabularViewLib.ChangeOperatorStatusData = function (methodName)
{
    var count = TabularViewLib.OperatorStatusSelectedRows.length;

    var dataPacket = {
        securityToken: TabularViewLib.securityToken,
        guids: []
    };

    for (var index = 0; index < count; ++index) {
        var rowIndex = TabularViewLib.OperatorStatusSelectedRows[index];
        dataPacket.guids[index] = window.FuelsManagerServiceLib.operatorStatusDataFM[rowIndex].PersonGuid;
    }

    $.ajax({
        type: "post",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: methodName,
        data: JSON.stringify(dataPacket),
        success: function (data) {
            var operatorStatusRequest = {};
            operatorStatusRequest.securityToken = TabularViewLib.securityToken;
            window.FuelsManagerServiceLib.CallDispatchRequestEnumerateOperatorStatus(operatorStatusRequest);
        },
    });
};

// Called when the Standby button is clicked.
TabularViewLib.StandbyButtonOnClick = function () {
	var standbyRequest = {};
	standbyRequest.securityToken = TabularViewLib.securityToken;
	standbyRequest.siteGuid = TabularViewLib.siteGuid;
	window.FuelsManagerServiceLib.CallDispatchRequestEnumerateStandbyPersonnel(standbyRequest);
};

// Display the Standby Status Board with Standby Personnel results.
TabularViewLib.DisplayStandbyStatusBoard = function () {
	if (window.FuelsManagerServiceLib.standbyPersonnelDataFM) {
		var standbyPersonnelData = window.FuelsManagerServiceLib.standbyPersonnelDataFM;
		var buttonsObj = {};
		if (standbyPersonnelData.length > 0) {
			buttonsObj[$('#dispatchLabel').text()] = TabularViewLib.OnDispatch;
		}
		buttonsObj[$('#closeLabel').text()] = function () { $(this).dialog('close'); };

		// Create standby status board dialog
		$('#StandbyButtonDialog').dialog(
		{
			autoOpen: false,
			modal: true,
			width: 435,
			height: 300,
			buttons: buttonsObj
		});

		// Create standby status board grid
		var grid;
		var columns = [
			{ id: "Personnel", name: $('#personnelLabel').text(), field: "FullName", minWidth: 180, width: 200 },
			{ id: "Equipment", name: $('#equipmentLabel').text(), field: "Equipment", minWidth: 180, width: 200 }
		];

		var options = {
			editable: false,
			enableAddRow: false,
			enableCellNavigation: true,
			enableColumnReorder: false,
			forceFitColumns: false,
			topPanelHeight: 15,
			rowHeight: 30,
			autoHeight: true
		};

		grid = new Slick.Grid("#gridStandby", standbyPersonnelData, columns, options);
		grid.onSelectedRowsChanged.subscribe(function (e, args) {
			if (args != null & args != undefined) {
				TabularViewLib.selectedPersonnel = standbyPersonnelData[args.rows].IdentityGuid;
				TabularViewLib.selectedEquipment = standbyPersonnelData[args.rows].EquipmentGuid;
			}
		});
		grid.setSelectionModel(new Slick.RowSelectionModel());

		// Display results
		$('#StandbyButtonDialog').dialog('open');
	}
};

// Called when the dispatch button on the standby status board dialog is clicked
TabularViewLib.OnDispatch = function () {
	var url = window.query("../DispatchWebApp/DispatchingView.aspx"); //get the CSRF tokenified url
	var standByStatusObj = JSON.stringify({
		selectedPersonnel: TabularViewLib.selectedPersonnel,
		selectedEquipment: TabularViewLib.selectedEquipment
	});
	var form = $('<form action="' + url + '" method="post">' +
			 '<input type="hidden" name="StandByStatusValues" value=' + standByStatusObj + ' /></form>');
	$('body').append(form);
	$(form).submit();
	$(this).dialog('close');
};

// Called when the Start Of Service button is clicked.
TabularViewLib.StartOfServiceButtonOnClick = function() {
	var numRows = TabularViewLib.selectedRows.length;
	if (numRows < 1) {
		return;
	}

	// Check for Arrived transactions
	for (var index = 0; index < numRows; ++index) {
		var rowNum = TabularViewLib.selectedRows[index];
		if (TabularViewLib.useArrivalTime && TabularViewLib.data[rowNum].Status != "Arrived" ) {
			alert("Request must be in Arrived status first!  Operation will be aborted!");
			return;
		}
	}

	var setServiceStartedRequest = {};
	setServiceStartedRequest.transactionIds = [];
	setServiceStartedRequest.lineItemGuids = [];
	for (index = 0; index < numRows; ++index)
	{
		rowNum = TabularViewLib.selectedRows[index];
		setServiceStartedRequest.transactionIds.push(TabularViewLib.data[rowNum].TransId);
		setServiceStartedRequest.lineItemGuids.push(TabularViewLib.data[rowNum].LineItemGuid);
	}

	if (setServiceStartedRequest.transactionIds.length > 0) {
		// Call jQuery Ajax interface to FuelsManager
		setServiceStartedRequest.securityToken = TabularViewLib.securityToken;
		setServiceStartedRequest.siteGuid = TabularViewLib.siteGuid;
		window.FuelsManagerServiceLib.CallDispatchRequestSetServiceStarted(setServiceStartedRequest);
	}
};

// Called when the Stop Of Service button is clicked.
TabularViewLib.StopOfServiceButtonOnClick = function ()
{
	var numRows = TabularViewLib.selectedRows.length;
	if (numRows < 1)
	{
		return;
	}

	// Check for Started transactions
	for (var index = 0; index < numRows; ++index)
	{
		var rowNum = TabularViewLib.selectedRows[index];
		if (TabularViewLib.useStartTime && TabularViewLib.data[rowNum].Status != "Started")
		{
			alert("Request must be in Started status first!  Operation will be aborted!");
			return;
		}
	}

	var setServiceStopped = {};
	setServiceStopped.transactionIds = [];
	setServiceStopped.lineItemGuids = [];
	for (index = 0; index < numRows; ++index)
	{
		rowNum = TabularViewLib.selectedRows[index];
		setServiceStopped.transactionIds.push(TabularViewLib.data[rowNum].TransId);
		setServiceStopped.lineItemGuids.push(TabularViewLib.data[rowNum].LineItemGuid);
	}

	if (setServiceStopped.transactionIds.length > 0)
	{
		// Call jQuery Ajax interface to FuelsManager
		setServiceStopped.securityToken = TabularViewLib.securityToken;
		setServiceStopped.siteGuid = TabularViewLib.siteGuid;
		window.FuelsManagerServiceLib.CallDispatchRequestSetServiceStopped(setServiceStopped);
	}
};

// Called when the Total And Average button is clicked.  Navigates to the Dispatch
// Total and Average page.  The navigateAction parameter and various filter parameters
// are passed to the page as a query string.
TabularViewLib.TotalAndAverageButtonOnClick = function () {

	// Get the saved date and status filters from session storage
	var beginDate = window.sessionStorage.beginDateFilter;
	var endDate = window.sessionStorage.endDateFilter;
	var status = window.sessionStorage.statusFilter;

	var queryString = "?navigateAction=openClick&beginDate=" + beginDate + "&endDate=" + endDate + "&status=" + status;
	window.window_location_assign("DispatchTotalAndAveragePage.aspx" + queryString);
};

// Called when the Transient button is clicked.
TabularViewLib.TransientButtonOnClick = function() {
	var sFeatures = "dialogWidth: 800px; dialogHeight: 480px";
	window.window_showModalDialog("FuelRequestForm.aspx?Transient=true", "", sFeatures);
};

// Called when a transaction alias button is clicked.
TabularViewLib.TransactionAliasButtonOnClick = function(aliasId) {
	alert("TabularViewLib.TransactionAliasButtonOnClick(" + aliasId + ") called");
};

function setUpHotKeys()
{
	// Status Filter - Show All
	$(document).bind('keydown', 'f10', function (event)
	{
		event.preventDefault();
		$('#StatusSelect').val('{All}');
		$('#StatusSelect').change();
	});

	// Status Filter - Show Requested
	$(document).bind('keydown', 'f11', function (event)
	{
		event.preventDefault();
		$('#StatusSelect').val('Requested');
		$('#StatusSelect').change();
	});

	// Status Filter - Show Dispatched
	$(document).bind('keydown', 'shift+f3', function (event)
	{
		event.preventDefault();
		$('#StatusSelect').val('Dispatched');
		$('#StatusSelect').change();
	});

	// Status Filter - Show Arrived
	$(document).bind('keydown', 'shift+f7', function (event)
	{
		event.preventDefault();
		$('#StatusSelect').val('Arrived');
		$('#StatusSelect').change();
	});

	// Status Filter - Show Started
	$(document).bind('keydown', 'shift+f10', function (event)
	{
		event.preventDefault();
		$('#StatusSelect').val('Started');
		$('#StatusSelect').change();
	});

	// Status Filter - Show Stopped
	$(document).bind('keydown', 'shift+f4', function (event)
	{
		event.preventDefault();
		$('#StatusSelect').val('Stopped');
		$('#StatusSelect').change();
	});

	// Status Filter - Show Completed
	$(document).bind('keydown', 'f12', function (event)
	{
		event.preventDefault();
		$('#StatusSelect').val('Completed');
		$('#StatusSelect').change();
	});

	// Status Filter - Show Cancelled
	$(document).bind('keydown', 'ctrl+f10', function (event)
	{
		event.preventDefault();
		$('#StatusSelect').val('Cancelled');
		$('#StatusSelect').change();
	});

	// Operation - Request F2
	$(document).bind('keydown', 'f2', function (event)
	{
		event.preventDefault();
		TabularViewLib.RequestButtonOnClick();
	});

	// Operation - Transient F4
	$(document).bind('keydown', 'f4', function (event) {
		event.preventDefault();
		TabularViewLib.TransientButtonOnClick();
	});

	// Operation - Fast Log F9
	$(document).bind('keydown', 'f9', function (event) {
		event.preventDefault();
		TabularViewLib.FastLogButtonOnClick();
	});

	// Operation - Fast Log Fillstand Shift+F9
	$(document).bind('keydown', 'shift+f9', function (event) {
		event.preventDefault();
		TabularViewLib.FastLogFillstandButtonOnClick();
	});

	// Operation - Recirculation F3
	$(document).bind('keydown', 'f3', function (event) {
		event.preventDefault();
		TabularViewLib.RecirculationButtonOnClick();
	});

	// Operation - Dispatch F6
	$(document).bind('keydown', 'f6', function (event)
	{
		event.preventDefault();
		TabularViewLib.DispatchButtonOnClick();
	});

	// Operation - Standby - Shift+F5
	$(document).bind('keydown', 'shift+f5', function (event)
	{
		event.preventDefault();
		TabularViewLib.StandbyButtonOnClick();
	});

	// Operation - Flight Line Status Ctrl+F3
	$(document).bind('keydown', 'ctrl+f3', function (event)
	{
		event.preventDefault();
		TabularViewLib.FlightLineStatusButtonOnClick();
	});

	// Operation - Copy Ctrl+F5
	$(document).bind('keydown', 'ctrl+f5', function (event)
	{
		event.preventDefault();
		TabularViewLib.RelogButtonOnClick();
	});

	// Operation - Cancel Ctrl+Shift+F4
	$(document).bind('keydown', 'ctrl+shift+f4', function (event) {
		event.preventDefault();
		TabularViewLib.CancelButtonOnClick();
	});

	// Operation - Arrival F7
	$(document).bind('keydown', 'f7', function(event) {
		event.preventDefault();
		TabularViewLib.ArrivalButtonOnClick();
	});

	// Operation - Start of Service Ctrl+F7
	$(document).bind('keydown', 'ctrl+f7', function(event) {
		event.preventDefault();
		TabularViewLib.StartOfServiceButtonOnClick();
	});

	// Operation - Stop of Service Ctrl+F8
	$(document).bind('keydown', 'ctrl+f8', function(event) {
		event.preventDefault();
		TabularViewLib.StopOfServiceButtonOnClick();
	});

	// Operation - Service Completion F8
	$(document).bind('keydown', 'f8', function(event) {
		event.preventDefault();
		TabularViewLib.ServiceCompletionButtonOnClick();
	});

	// Operation - Fillstand Completion Shift+F8
	$(document).bind('keydown', 'shift+f8', function(event) {
		event.preventDefault();
		TabularViewLib.FillstandCompletionButtonOnClick();
	});

	// Operation - Change Operator Status F5
	$(document).bind('keydown', 'f5', function(event) {
		event.preventDefault();
		TabularViewLib.ChangeOperatorStatusButtonOnClick();
	});

	// Operation - Controllers log Ctrl+F9
	$(document).bind('keydown', 'ctrl+f9', function(event) {
		event.preventDefault();
		TabularViewLib.ControlLogButtonOnClick();
	});

	$(document).bind('keydown', 'home', function (event) {
		if (TabularViewLib.tabularGridInFocus()) {
			TabularViewLib.grid.setSelectedRows([0]);
			TabularViewLib.grid.setActiveCell(0, 0);
		}
	});

	$(document).bind('keydown', 'end', function (event) {
		if (TabularViewLib.tabularGridInFocus()) {
			var row = TabularViewLib.data.length - 1;
			TabularViewLib.grid.setSelectedRows([row]);
			TabularViewLib.grid.setActiveCell(row, 0);
		}
	});

	$(document).bind('keydown', 'pageup', function (event) {
		if (TabularViewLib.tabularGridInFocus()) {
			var row = TabularViewLib.grid.getViewport().top;
			if (row < 0) {
				row = 0;
			}
			TabularViewLib.grid.setSelectedRows([row]);
			TabularViewLib.grid.setActiveCell(row, 0);
		}
	});

	$(document).bind('keydown', 'pagedown', function (event) {
		if (TabularViewLib.tabularGridInFocus()) {
			var row = TabularViewLib.grid.getViewport().bottom;
			if (row >= TabularViewLib.data.length) {
				row = TabularViewLib.data.length - 1;
			}
			TabularViewLib.grid.setSelectedRows([row]);
			TabularViewLib.grid.setActiveCell(row, 0);
		}
	});
}

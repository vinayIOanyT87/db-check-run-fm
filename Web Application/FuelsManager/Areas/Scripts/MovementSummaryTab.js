class AsyncMutex {
	constructor() {
		this.locked = false;
		this.queue = []; // Stores functions waiting for the lock
	}

	async acquire() {
		if (!this.locked) {
			this.locked = true;
			return Promise.resolve(); // Acquire immediately if not locked
		}

		// If locked, add a function to the queue that will resolve when unlocked
		return new Promise(resolve => {
			this.queue.push(resolve);
		});
	}

	release() {
		if (this.queue.length > 0) {
			const nextResolver = this.queue.shift();
			nextResolver(); // Resolve the next waiting promise, effectively granting the lock
		} else {
			this.locked = false; // No one is waiting, so release the lock entirely
		}
	}

	async runExclusive(fn) {
		await this.acquire();
		try {
			return await fn();
		} finally {
			this.release();
		}
	}
}


var FMMovementSummaryTab = FMMovementSummaryTab ||
{
	messageAttributes: {},
	refreshFrequency:5000, //ms
};

if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

//==============================================================
// Get a list of existing movement names that the user has 
// added.
//==============================================================
FMMovementSummaryTab.GetListOfExistingMovements = function (grid)
{
	var dataView = grid.getData();
	return dataView.getItems().filter(row => row.rowType === 'movement').map(row => row.point);
};

//======================================================================================
// This function will call the controller to initiate a movement.
//======================================================================================
FMMovementSummaryTab.InitiateMovement = function (movementPointGuidString)
{
	$.ajax({
		type: 'POST',
		url: 'InitiateMovement',
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify({ "movementPointGuidString": movementPointGuidString }),
		cache: false,
		async: false,
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) { });
		},
		error: function (xhr, ajaxOptions, thrownError) {
			FMErrorAndExceptionHandling.ShowError(thrownError);
			$("#MovementSummarySelectionModal").modal("hide");
		}
	});
};

//======================================================================================
// This function will call the controller to initiate a movement node.
//======================================================================================
FMMovementSummaryTab.InitiateMovementNode = function (movementPointGuidString, movementNodePointGuidString) {
	$.ajax({
		type: 'POST',
		url: 'InitiateMovementNode',
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify({
			"movementPointGuidString": movementPointGuidString, "movementNodePointGuidString": movementNodePointGuidString }),
		cache: false,
		async: false,
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) { });
		},
		error: function (xhr, ajaxOptions, thrownError) {
			FMErrorAndExceptionHandling.ShowError(thrownError);
			$("#MovementSummarySelectionModal").modal("hide");
		}
	});
};

//======================================================================================
// This function will call the controller to stop a movement
//======================================================================================
FMMovementSummaryTab.StopMovement = function (movementPointGuidString)
{
	$.ajax({
		type: 'POST',
		url: 'StopMovement',
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify({ "movementPointGuidString": movementPointGuidString }),
		cache: false,
		async: false,
		success: function (response)
		{
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) { });
		},
		error: function (xhr, ajaxOptions, thrownError) {
			FMErrorAndExceptionHandling.ShowError(thrownError);
			$("#MovementSummarySelectionModal").modal("hide");
		}
	});
};

//======================================================================================
// This function will call the controller to stop a movement node
//======================================================================================
FMMovementSummaryTab.StopMovementNode = function (movementPointGuidString, movementNodePointGuidString) {
	$.ajax({
		type: 'POST',
		url: 'StopMovementNode',
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify({"movementPointGuidString": movementPointGuidString, "movementNodePointGuidString": movementNodePointGuidString }),
		cache: false,
		async: false,
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) { });
		},
		error: function (xhr, ajaxOptions, thrownError) {
			FMErrorAndExceptionHandling.ShowError(thrownError);
			$("#MovementSummarySelectionModal").modal("hide");
		}
	});
};

//================================================================================
// This function will call to open the movement module settings to create
// a new movement.
//================================================================================
FMMovementSummaryTab.CreateNewMovement = function (newId)
{
	var emptyGuid = "00000000-0000-0000-0000-000000000000";
	var caller = "OperateCreateNew";
	var isTemplatePoint = false;

	FMOperateIndex.OpenMovementModuleSettingsClickPropertyScreen(newId, isTemplatePoint, emptyGuid, emptyGuid, caller);
};

//================================================================================
// This function will call to delete movement
//================================================================================
FMMovementSummaryTab.DeleteMovement = function (movementPointGuidString) {

	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };

	$.ajax({
		type: 'POST',
		url: 'DeleteMovement',
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify({ "movementPointGuidString": movementPointGuidString }),
		cache: false,
		async: false,
		success: function (data) {
			PNotify.removeStack(messageAttributes.stack);
			FMErrorAndExceptionHandling.HandleMessages(data, function () {
			}, messageAttributes);
		},
		error: function (request, status, error) {
			// remove previous notifications
			PNotify.removeStack(messageAttributes.stack);

			FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
			}, messageAttributes);
		}
	});
};


//================================================================================
// This function will call to open the movement module settings to edit
// an existing movement.
//================================================================================
FMMovementSummaryTab.SetMovementSettings = function (movementPointGuidString)
{
	var emptyGuid = "00000000-0000-0000-0000-000000000000";
	var caller = "OperateSetMovementSettings";
	var isTemplatePoint = false;

	FMOperateIndex.OpenMovementModuleSettingsClickPropertyScreen('', isTemplatePoint, movementPointGuidString, emptyGuid, caller);
};

//================================================================================
// This function will call to open the movement user data to edit
// based on the movement point Guid.
//================================================================================
FMMovementSummaryTab.EditMovementUserData = function (movementPointGuidString)
{
	FMOperateIndex.OpenMovementUserDataClickPropertyScreen(movementPointGuidString);
};

//================================================================================
// This function will call to open the movement handgauge to edit
// based on the movement point Guid.
//================================================================================
FMMovementSummaryTab.EditMovementHandgauge = function (movementPointGuidString)
{
	var caller = 0; // MovementSummery
	var movementHistoryRecordGuid = "00000000-0000-0000-0000-000000000000";
	FMOperateIndex.OpenMovementHandgaugeClickPropertyScreen(movementPointGuidString, caller, movementHistoryRecordGuid);
};

//================================================================================
// This function will call to open the movement start data to edit
// based on the movement point Guid.
//================================================================================
FMMovementSummaryTab.EditMovementStartData = function (movementPointGuidString) {
	FMOperateIndex.OpenMovementStartDataClickPropertyScreen(movementPointGuidString);
};

//================================================================================
// This function will call to open the movement node start data to edit
// based on the movement point Guid and the movement node point Guid
//================================================================================
FMMovementSummaryTab.EditMovementNodeStartData = function (movementPointGuidString, movementNodePointGuidString) {
	FMOperateIndex.OpenMovementNodeStartDataClickPropertyScreen(movementPointGuidString, movementNodePointGuidString);
};

//================================================================================
// This function will call to open the movement disabled by dialog
// based on the movement point Guid.
//================================================================================
FMMovementSummaryTab.MovementDisabledBy = function (movementPointId, movementPointGuidString) {
	FMOperateIndex.OpenMovementDisabledBy(movementPointId, movementPointGuidString);
};


//==================================================================
// This function finds the selected row, displays a movement list
// dialog, and adds the movement if it does not already exist.
//==================================================================
FMMovementSummaryTab.SelectRow = function (activeTab, newId, grid, rowNumber)
{

	// Get a list of existing movements that have already been added.
	// This will be used as a filter when retreiving a list of movements.
	var existingMovementNameList = FMMovementSummaryTab.GetListOfExistingMovements(grid);

	// create the backdrop and wait for next modal to be triggered
	$('body').modalmanager('loading');

	$("#MovementSummarySelectionModalBody").html('<div id="MovementSummaryModalMenuLoader" class="LoadingAnimation transparent"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>');
	$("#MovementSummarySelectionModal").modal("show");

	$.ajax({
		type: 'Get',
		url: 'GetListOfMovementSummaryPointsPartialView',
		dataType: "html",
		data: { "ID": existingMovementNameList, "parentControl": "#MovementSummarySelectionModalBody", "persistChanges": false },
		cache: false,
		success: function (view) {
			$("#MovementSummarySelectionModalBody").html(view);

			$('#MovementSummarySelectionModalBody .operateSubMenuList').css("height", $('.operateSubMenuList').parent().height());

			// we need to remove the onclick event since by default it will open a new point when clicked and instead we need to add a 'selected' class
			$("#MovementSummarySelectionModalBody .operateSubMenuElement").each(function () {
				$(this).attr('onclick', "$(this).hasClass('selected') ? $(this).removeClass('selected'): $(this).addClass('selected')");
			});

			$("#MovementSummarySelectionModalBody .operateSubMenuElement").each(function () {
				$(this).attr('ondblclick', "$(this).removeClass('selected').addClass('selected'); FMOperateIndex.MovementSummarySelectionModalSelectButton();");
			});

			$('#MovementSummarySelectionModalBody .operateSubMenuList').uncolumnize();
			$('#MovementSummarySelectionModalBody .operateSubMenuList').columnize({
				columns: 2,
				buildOnce: true,
				cssClassPrefix: "points",
				lastNeverTallest: true
			});

			$("#MovementSummarySelectionModalBody .operateSubMenuList").niceScroll({ cursorwidth: '10px', horizrailenabled: false, autohidemode: false, cursorcolor: "#486899", background: "white" });

			//=============================================================================================
			// This function overrides the code executed on the Select Button of the selection 
			// modal to deal with new movements.
			//=============================================================================================
			FMOperateIndex.MovementSummarySelectionModalSelectButton = async function () {
				if ($('.operateSubMenuElement.selected').length === 0) {
					FMLayout.Alert("No Movement selected.");
				}
				else {
					var dataView = grid.getData();

					dataView.beginUpdate();

					var selected = $('.operateSubMenuElement.selected').sort(function (a, b) {  
						return $(a).attr('data-name').toUpperCase().localeCompare($(b).attr('data-name').toUpperCase());
					}).toArray();

					for (i = 0; i < selected.length; i ++) {
						var newPoint = $(selected[i]).attr('data-name');
						var newPointGuid = $(selected[i]).attr('data-guid');
						await FMMovementSummaryTab.AddMovementRowAsync(newId, grid, newPoint, newPointGuid);
					}

					dataView.endUpdate();
					var rows = dataView.getItems();
					grid.resizeCanvas();
					grid.scrollRowIntoView(0, false);
					grid.invalidateAllRows();
					grid.render();

					FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);

					$("#MovementSummarySelectionModal").modal("hide");
				}
			};
		},
		error: function (xhr, ajaxOptions, thrownError) {
			FMErrorAndExceptionHandling.ShowError(thrownError);
			$("#MovementSummarySelectionModal").modal("hide");
		}
	});
};

//==================================================================
// This function adds a movement async
//==================================================================
FMMovementSummaryTab.AddMovementRowAsync = async function (newId, grid, pointId, pointGuid) {
	let mutex = FMOperateIndex.movementSummaryControllers[newId].getMutex();
	await mutex.acquire();
	try {
		FMMovementSummaryTab.AddMovementRow(newId, grid, pointId, pointGuid);

	}
	finally {
		mutex.release(); // Ensure the lock is released
		return Promise.resolve();
	}
}


//==================================================================
// This function adds a movement
//==================================================================
FMMovementSummaryTab.AddMovementRow = function (newId, grid, pointId, pointGuid) {

	// Get the associated movement nodes
	let movementNodeInfo = FMMovementSummaryTab.GetAssociatedMovementNodes(pointGuid);

	var movementRowId = FMOperateIndex.newGuid();

	var newRowDefinition =
	{
		id: movementRowId,
		type: "point",
		point: pointId,
		pointguid: pointGuid,
		rowType: "movement",
		direction: "",
		parentRowId: null,
		masterRowId: null,
		individualNodeControl: false,
		movementguid: pointGuid
	};

	var dataView = grid.getData();

	var id = FMMovementSummaryTab.GetBlankRowId(grid);
	if (id === null) {
		return;
	}

	let rowIndex = dataView.getIdxById(id);

	dataView.insertItem(rowIndex, newRowDefinition);

	rowIndex++;

	var theShadowGrid = FMOperateIndex.movementSummaryControllers[newId];
	theShadowGrid.addRow(newRowDefinition);

	// Insert any associated movement nodes.
	FMMovementSummaryTab.InsertMovementNodeRows(grid, movementRowId, movementRowId, pointGuid, rowIndex, movementNodeInfo);


	// refresh the indented groups
	var data = grid.getData().getItems();
	var itemMap = FMMovementSummaryTab.groupBy(data, 'parentRowId');
	var data = FMMovementSummaryTab.initTree(data, itemMap);
};

//==================================================================
// This function removes the selected movement async
//==================================================================
FMMovementSummaryTab.RemoveMovementRowAsync = async function (newId, grid, rowNumber) {
	let mutex = FMOperateIndex.movementSummaryControllers[newId].getMutex();
	await mutex.acquire();
	try {
		FMMovementSummaryTab.RemoveMovementRow(newId, grid, rowNumber);
	}
	finally {
		mutex.release(); // Ensure the lock is released
		return Promise.resolve();
	}
}


//==================================================================
// This function removes the selected movement
//==================================================================
FMMovementSummaryTab.RemoveMovementRow = function (newId, grid, rowNumber) {

	var dataView = grid.getData();
	var item = dataView.getItem(rowNumber);

	// Delete the associated rows first so that the row count
	// doesn't change.
	FMMovementSummaryTab.DeleteAssociatedNodes(newId, item, dataView);

	// Delete the shadow row item.
	var theShadowGrid = FMOperateIndex.movementSummaryControllers[newId];
	theShadowGrid.deleteRow(item);

	var id = dataView.getItem(rowNumber).id;
	dataView.deleteItem(id);

	// if we don't have any more rows displayed we need to add an empty row
	if (dataView.getFilteredItems().length === 0) {
		var newRowDefinition = { id: FMOperateIndex.newGuid(), type: "empty" };
		dataView.addItem(newRowDefinition);
		dataView.refresh();
	}

	grid.scrollRowIntoView(rowNumber - 1);
};

FMMovementSummaryTab.GetBlankRowId = function (grid) {
	dataView = grid.getData();
	rows = dataView.getItems();
	for (var i = 0; i < rows.length; i++) {
		var row = rows[i];
		if (row && row.type === "blank") return row.id;
	}
	return null;
};

//===============================================================================
// This function will create row definition for a movement node and insert it
// it into the data view.
//===============================================================================
FMMovementSummaryTab.InsertMovementNodeRow = function (grid, currentRowIndex, parentRowId, masterRowId, movementGuid, nodePointGuid, nodePointId, transferDirection, individualNodeControl) {

	var dataView = grid.getData();

	var newRowDefinition =
	{
		id: FMOperateIndex.newGuid(),
		type: "point",
		point: nodePointId,
		pointguid: nodePointGuid,
		rowType: "node",
		direction: transferDirection,
		parentRowId: parentRowId,
		masterRowId: masterRowId,
		individualNodeControl: individualNodeControl,
		movementguid: movementGuid
	};

	dataView.insertItem(currentRowIndex, newRowDefinition);
};



//===============================================================================
// This function will create row definition for a movement nodes and insert them
// it into the data view.
//===============================================================================
FMMovementSummaryTab.InsertMovementNodeRows = function (grid, parentRowId, masterRowId, movementGuid, rowIndex, movementNodeInfo)
{
	var length = (movementNodeInfo && movementNodeInfo.length) || 0;

	for (var nextNodeIndex = 0; nextNodeIndex < length; nextNodeIndex++)	{
		var nodeInfoItem = movementNodeInfo[nextNodeIndex];
		var nodePointGuid = nodeInfoItem.MovementNodeGuid;
		var nodePointId = nodeInfoItem.MovementNodeId;
		var transferDirection = nodeInfoItem.TransferDirection;
		var individualNodeControl = nodeInfoItem.IndividualNodeControl;

		FMMovementSummaryTab.InsertMovementNodeRow(grid, rowIndex, parentRowId, masterRowId, movementGuid, nodePointGuid, nodePointId, transferDirection, individualNodeControl);

		rowIndex++;
   }
};

//================================================================================
// This function gets the associated movement node data associated to the movement
// point.
//================================================================================
FMMovementSummaryTab.GetAssociatedMovementNodes = function(pointGuidStr)
{
    var movementNodes = [];

	$.ajax({
		type: 'POST',
		url: 'GetAssociatedMovementNodes',
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify({ "pointGuidStr": pointGuidStr }),
		cache: false,
		async: false,
		success: function (data)
		{
			movementNodes = data;
		},
		error: function (xhr, ajaxOptions, thrownError)
		{
			FMErrorAndExceptionHandling.ShowError(thrownError);
			$("#MovementSummarySelectionModal").modal("hide");
		}
    });

    return movementNodes;
};

//============================================================================
// This function will delete the movement's associated movement nodes.
//============================================================================
FMMovementSummaryTab.DeleteAssociatedNode = function (grid, row) {
	var dataView = grid.getData();

	dataView.deleteItem(row.id);
};



//============================================================================
// This function will delete the movement's associated movement nodes.
//============================================================================
FMMovementSummaryTab.DeleteAssociatedNodes = function (newId,movementItem, dataView)
{
	var rows = dataView.getItems();
	var theShadowGrid = FMOperateIndex.movementSummaryControllers[newId];

	if (rows && rows.length > 0)
	{
		var startRowIndex = rows.length - 1;

		// Note: start from the bottom up in order for the row indexes not to change.
		for (var nextItemIndex = startRowIndex; nextItemIndex >= 0; nextItemIndex--)
		{
			var row = rows[nextItemIndex];

			// If the item has a node type, then it is a movement node row.
			// In this case then we want to check if the parent row ID are equal.
			// If so, then delete.
			if (row.rowType && (row.rowType === "node") && movementItem)
			{
				if (row.masterRowId === movementItem.id)
				{
					theShadowGrid.deleteRow(movementItem);
					dataView.deleteItem(row.id);
				}
			}
		}
	}
};


//==========================================================
// this function will apply tag filters to the grid.
//===========================================================
FMMovementSummaryTab.FilterMovementSummaryGrid = function(item, args)
{
	var columns = args.columnsToFilter;
	var metadata = args.metadata;
	var returnValue = true;

	for (var i = 0; i < columns.length; i++)
	{
		if (columns[i].hasOwnProperty('filter')
			&& item.hasOwnProperty('type') && item.type === 'point'
			&& (item.hasOwnProperty('rowType') && item.rowType === 'movement'
			|| item.hasOwnProperty('rowType') && item.rowType === 'node'))
		{
			// All rows are filtered based upon Movement Data.
			var tagMetaData = FMMovementSummaryGrid.getTagInfo(metadata, item.movementguid, columns[i].field);

			if (tagMetaData === null) {
				returnValue &= false;
			}

			else {
				if (columns[i].filter.type === 'numeric'
					&& (tagMetaData.ValueTypeString === "System.Double"
						|| tagMetaData.ValueTypeString === "System.Int16" || tagMetaData.ValueTypeString === "System.Int32" || tagMetaData.ValueTypeString === "System.Int64"))
				{
					if (tagMetaData.Value != null || columns[i].filter.operator === "not_equal" || columns[i].filter.operator === "not_between") {
						var unit = tagMetaData.Units;
						var value = math.bignumber(tagMetaData.Value);
						var minRawValue = math.bignumber(columns[i].filter.minValue);
						var minFilterValue = 0;

						// if we don't have unit no need for unit conversion
						if (parseInt(unit) === -1) {
							minFilterValue = minRawValue;
						}
						else {
							minFilterValue = FMConvertEngUnits.Convert(minRawValue, parseInt(columns[i].filter.unit), parseInt(unit));

						}
						switch (columns[i].filter.operator) {
							case "equal":
								if (!math.equal(value, minFilterValue)) {
									returnValue &= false;
								}
								break;
							case "not_equal":
								if (math.equal(value, minFilterValue)) {
									returnValue &= false;
								}
								break;
							case "greater":
								if (!math.larger(value, minFilterValue)) {
									returnValue &= false;
								}
								break;
							case "greater_equal":
								if (!math.largerEq(value, minFilterValue)) {
									returnValue &= false;
								}
								break;
							case "less":
								if (!math.smaller(value, minFilterValue)) {
									returnValue &= false;
								}
								break;
							case "less_equal":
								if (!math.smallerEq(value, minFilterValue)) {
									returnValue &= false;
								}
								break;
							case "between":
								var maxFilterValue = math.bignumber(columns[i].filter.maxValue);
								// if we don't have unit no need for unit conversion
								if (parseInt(unit) !== -1) {
									maxFilterValue = FMConvertEngUnits.Convert(maxFilterValue, parseInt(columns[i].filter.unit), parseInt(unit));
								}

								if (!(math.largerEq(value, minFilterValue) && math.smallerEq(value, maxFilterValue))) {
									returnValue &= false;
								}
								break;
							case "not_between":
								var maxFilterValue = math.bignumber(columns[i].filter.maxValue);
								// if we don't have unit no need for unit conversion
								if (parseInt(unit) !== -1) {
									maxFilterValue = FMConvertEngUnits.Convert(maxFilterValue, parseInt(columns[i].filter.unit), parseInt(unit));
								}
								if (!(math.smaller(value, minFilterValue) || math.larger(value, maxFilterValue))) {
									returnValue &= false;
								}
								break;
							default:
								returnValue &= true;
						}
					}
					else  // if the column has no value then don't show the row
					{
						returnValue &= false;
					}
				}
				else if (columns[i].filter.type === 'boolean' && tagMetaData.ValueTypeString === "System.Boolean") {
					if (item[columns[i].field] !== null) {
						returnValue &= (item[columns[i].field].toLowerCase() === columns[i].filter.Value.toLowerCase());
					}
					else
						returnValue &= false; // convert to boolean and compare the filter and value
				}
				else if (columns[i].filter.type === 'enum' && tagMetaData.ValueTypeString.startsWith("FMBusinessObjects.DataObjects.CodedVariables")) {
					returnValue &= columns[i].filter.Value.indexOf(tagMetaData.Value) >= 0;  // value must be in the array of filters
				}
				else if (columns[i].filter.type === 'string' && tagMetaData.ValueTypeString === "System.String") {
					// if filter is specified
					if (columns[i].filter.Value !== "") {
						var searchValue = columns[i].filter.Value.toLowerCase();
						returnValue &= (tagMetaData.Value.toLowerCase().indexOf(searchValue) !== -1); // check if the value contains the string we are looking for
					}
					else //filter value is blank so show things with empty or no value
					{
						if (tagMetaData.Value) {
							returnValue &= false;
						}
						else {
							returnValue &= true;
						}
					}

				}
				else if (columns[i].filter.type === 'datetimeoffset' && tagMetaData.ValueTypeString === "System.DateTimeOffset") {
					if (tagMetaData.Value != null || columns[i].filter.operator === "not_equal" || columns[i].filter.operator === "not_between") {
						var re = /-?\d+/;
						var m = re.exec(tagMetaData.Value);
						var value = m != null ? new Date(parseInt(m[0])) : null;
						var minFilterValue = new Date(columns[i].filter.minValue);

						switch (columns[i].filter.operator) {
							case "equal":
								if (value.toString() !== minFilterValue.toString()) {
									returnValue &= false;
								}
								break;
							case "not_equal":
								if (value.toString() === minFilterValue.toString()) {
									returnValue &= false;
								}
								break;
							case "greater":
								if (value <= minFilterValue) {
									returnValue &= false;
								}
								break;
							case "greater_equal":
								if (value < minFilterValue) {
									returnValue &= false;
								}
								break;
							case "less":
								if (value >= minFilterValue) {
									returnValue &= false;
								}
								break;
							case "less_equal":
								if (value > minFilterValue) {
									returnValue &= false;
								}
								break;
							case "between":
								var maxFilterValue = new Date(columns[i].filter.maxValue);

								if (!(value >= minFilterValue && value <= maxFilterValue)) {
									returnValue &= false;
								}
								break;
							case "not_between":
								var maxFilterValue = new Date(columns[i].filter.maxValue);

								if (!(value < minFilterValue || value > maxFilterValue)) {
									returnValue &= false;
								}
								break;
							default:
								returnValue &= true;
						}
					}
					else  // if the column has no value then don't show the row
					{
						returnValue &= false;
					}
				}
				else if (columns[i].filter.type === 'timespan' && tagMetaData.ValueTypeString === "System.TimeSpan") {

					if (tagMetaData.Value != null || columns[i].filter.operator === "not_equal" || columns[i].filter.operator === "not_between") {
						var value = tagMetaData.Value != null ? tagMetaData.Value.Days + (((tagMetaData.Value.Hours * 60 * 60) + (tagMetaData.Value.Minutes * 60) + tagMetaData.Value.Seconds) / 86400) : null;
						var minFilterValue = columns[i].filter.minValue.days + (((columns[i].filter.minValue.hours * 60 * 60) + (columns[i].filter.minValue.minutes * 60) + columns[i].filter.minValue.seconds) / 86400);

						switch (columns[i].filter.operator) {
							case "equal":
								if (value !== minFilterValue) {
									returnValue &= false;
								}
								break;
							case "not_equal":
								if (value === minFilterValue) {
									returnValue &= false;
								}
								break;
							case "greater":
								if (value <= minFilterValue) {
									returnValue &= false;
								}
								break;
							case "greater_equal":
								if (value < minFilterValue) {
									returnValue &= false;
								}
								break;
							case "less":
								if (value >= minFilterValue) {
									returnValue &= false;
								}
								break;
							case "less_equal":
								if (value > minFilterValue) {
									returnValue &= false;
								}
								break;
							case "between":
								var maxFilterValue = columns[i].filter.maxValue.days + (((columns[i].filter.maxValue.hours * 60 * 60) + (columns[i].filter.maxValue.minutes * 60) + columns[i].filter.maxValue.seconds) / 86400);

								if (!(value >= minFilterValue && value <= maxFilterValue)) {
									returnValue &= false;
								}
								break;
							case "not_between":
								var maxFilterValue = columns[i].filter.maxValue.days + (((columns[i].filter.maxValue.hours * 60 * 60) + (columns[i].filter.maxValue.minutes * 60) + columns[i].filter.maxValue.seconds) / 86400);

								if (!(value < minFilterValue && value > maxFilterValue)) {
									returnValue &= false;
								}
								break;
							default:
								returnValue &= true;
						}
					}
					else  // if the column has no value then don't show the row
					{
						returnValue &= false;
					}
				}
			}
		}
	}

	if (item.parent != null) {
		var parent = data[item.parent];

		var seenId = [];
		while (parent) {
			if (parent._collapsed) {
				returnValue &= false;
			}

			// prevent infinte loop
			if (seenId.includes(parent.id)) {
				break;
			}
			seenId.push(parent.id);

			parent = data[parent.parent];
		}
	}

	return returnValue;
};

//=========================================================================
// This function will filter dateTimeOffset column
//=========================================================================
FMMovementSummaryTab.DateTimeOffsetFilter = function (args)
{
	let ActiveTab = (FMOperateIndex.isTabGroupEnabled) ? $(".tab-pane.active").attr("id") : 'mainTab';
	let TabIdNumber = (FMOperateIndex.isTabGroupEnabled) ? $(".tab-pane.active .tab-pane.active").attr("id") : $(".tab-pane.active").attr("id");

	var popover = $(args.headercell).popover("destroy").popover({
		container: 'body',
		placement: 'bottom',
		html: true,
		content: $('#MovementSummaryFilterDateTimeOffsetHeader').html(),
		trigger: "manual"
	});

	var dataPopover = popover.data('bs.popover');
	$(args.headercell).popover('show');
	$("#customModalBackground").removeClass("hidden");

	dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').datetimepicker({
		buttonImage: window.applicationRootName + '/dispatchwebapp/images/calendar.gif',
		buttonImageOnly: true,
		 showOn: "button",
		 showTimezone: false,
		 useLocalTimezone: false,
		 defaultTimezone: $("#datepickerTimezoneString").val(),
		dateFormat: FMLayout.dateFormat,
		timeFormat: FMLayout.timeFormat,
		showSecond: (FMLayout.timeFormat.indexOf('ss') === -1) ? false : true,
		beforeShow: function () {
			setTimeout(function () {
				$('.ui-datepicker').css('z-index', 1100);
			}, 0);
		},
		onSelect: function (d, i) {
			if (d !== i.lastVal) {
				$(this).change();
			}
		}
	});

	dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').datetimepicker({
		buttonImage: window.applicationRootName + '/dispatchwebapp/images/calendar.gif',
		buttonImageOnly: true,
		 showOn: "button",
		 showTimezone: false,
		 useLocalTimezone: false,
		 defaultTimezone: $("#datepickerTimezoneString").val(),
		dateFormat: FMLayout.dateFormat,
		timeFormat: FMLayout.timeFormat,
		showSecond: (FMLayout.timeFormat.indexOf('ss') === -1) ? false : true,
		beforeShow: function () {
			setTimeout(function () {
				$('.ui-datepicker').css('z-index', 1100);
			}, 0);
		},
		onSelect: function (d, i) {
			if (d !== i.lastVal) {
				$(this).change();
			}
		}
	});

	dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().find('img').addClass('hidden');

	//populate values from the stored filter
	if (args.column.hasOwnProperty('filter')) {
		dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterOperator]').val(args.column.filter.operator);
		if (args.column.filter.operator === "between" || args.column.filter.operator === "not_between") {
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMaxLabel]').removeClass('hidden');
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').removeClass('hidden');
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().find('img').removeClass('hidden');
		}
		else {
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMaxLabel]').removeClass('hidden').addClass('hidden');
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').removeClass('hidden').addClass('hidden');
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().find('img').removeClass('hidden').addClass('hidden');
		}

		if (args.column.filter.minValue !== "") {
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').datetimepicker('setDate', new Date(args.column.filter.minValue));
		}
		if (args.column.filter.maxValue !== "" && (args.column.filter.operator === "between" || args.column.filter.operator === "not_between")) {
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').datetimepicker('setDate', new Date(args.column.filter.maxValue));
		}
	}

	// change the operator
	dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterOperator]').on('change', function (event) {
		var operator = $(this).val();
		if (operator === "between" || operator === "not_between") {
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMaxLabel]').removeClass('hidden');
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').removeClass('hidden');
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().find('img').removeClass('hidden');

		}
		else {
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMaxLabel]').removeClass('hidden').addClass('hidden');
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').removeClass('hidden').addClass('hidden');
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().find('img').addClass('hidden');
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').val('');
		}
	});

	// remove filter
	dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMovementSummaryReset]').on('click', function (event) {

		$.map(args.grid.getColumns(), function (elem, idx)
		{
			if (args.column.field === elem.field)
			{
				delete elem.filter;
			}
		});

		$(args.headercell).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		args.grid.setColumns(args.grid.getColumns());
		args.grid.getData().refresh();
		args.grid.invalidateAllRows();
		args.grid.render();
		event.stopPropagation();
	});

	// close the pop over when clicking cancel
	dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMovementSummaryCancel]').on('click', function (event) {

		$(args.headercell).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});

	// Apply filter
	dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMovementSummaryApply]').on('click', function (event)
	{
		dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').parent().removeClass('has-error');
		var rawMinValue = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').val();

		// check to see if we have a valid date entered
		try
		{
			var validateMinDate = $.datepicker.parseDateTime(FMLayout.dateFormat, FMLayout.timeFormat, rawMinValue, {}, {});
		}
		catch (e)
		{
			dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').parent().addClass('has-error');
			return;
		}

		var operator = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterOperator]').val();

		var minValue = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').datepicker('getDate');
		var minTime = { hour: minValue.getHours(), minute: minValue.getMinutes(), second: minValue.getSeconds(), timezone: minValue.getTimezoneOffset() }
		var formattedMinDateTime = $.datepicker.formatDate(FMLayout.dateFormat, minValue) + ' ' + $.datepicker.formatTime(FMLayout.timeFormat, minTime);

		var maxValue = "";
		var formattedMaxDateTime = "";

		if (operator === "between" || operator === "not_between")
		{
			var rawMaxValue = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').val();
			try
			{
				var validateMaxDate = $.datepicker.parseDateTime(FMLayout.dateFormat, FMLayout.timeFormat, rawMaxValue, {}, {});
			}
			catch (e)
			{
				dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().addClass('has-error');
				return;
			}

			maxValue = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').datepicker('getDate');
			var maxTime = { hour: maxValue.getHours(), minute: maxValue.getMinutes(), second: maxValue.getSeconds(), timezone: maxValue.getTimezoneOffset() }

			formattedMaxDateTime = $.datepicker.formatDate(FMLayout.dateFormat, maxValue) + ' ' + $.datepicker.formatTime(FMLayout.timeFormat, maxTime);
		}

		// create a description to show in a tooltip
		var description = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterOperator] option:selected').text() +
			" " + formattedMinDateTime;
		if (operator === "between" || operator === "not_between") {
			description += " and " + formattedMaxDateTime;
		}

		var filter = {
			type: 'datetimeoffset',
			operator: operator,
			minValue: minValue !== "" ? minValue.toISOString() : "",
			maxValue: maxValue !== "" ? maxValue.toISOString() : "",
			description: encodeURIComponent(description)
		};

		// copy the filter to all the columns of the same field
		$.map(args.grid.getColumns(), function (elem, idx)
		{
			if (args.column.field === elem.field) {
				elem.filter = filter;
			}
		});

		$(args.headercell).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");

		args.grid.setColumns(args.grid.getColumns());
		args.grid.getData().refresh();
		args.grid.invalidateAllRows();
		args.grid.render();
		event.stopPropagation();
	});
};

FMMovementSummaryTab.AddColumnToTheSummaryBasedOnDBChange = function (addedColumns, rcvdColDefinitions, newId) {
	var grid = FMOperateIndex.movementSummaryControllers[newId]._grid;
	var theShadowGrid = FMOperateIndex.movementSummaryControllers[newId];
	var selectedRows = grid.getSelectedRows();

	for (let i = 0; i < addedColumns.length; i++) {
		const posToInsert = rcvdColDefinitions.findIndex(col => col.id === addedColumns[i].id);

		if (posToInsert < 0) continue;

		var columnDefinition = {
			id: addedColumns[i].id,
			name: decodeURIComponent(addedColumns[i].name),
			field: addedColumns[i].field,
			headerCssClass: addedColumns[i].headerCssClass,
			cssClass: addedColumns[i].cssClass,
			formatter: FMOperateIndex.movementSummaryControllers[newId] ? FMOperateIndex.movementSummaryControllers[newId].summaryColumnFormatter : () => '',
			sortable: addedColumns[i].sortable,
			width: addedColumns[i].width,
			minWidth: addedColumns[i].minWidth
		};

		columnDefinition.header = {
			menu: { items: FMOperateIndex.MovementSummaryCreateHeaderMenu() }
		};

		// add the filter if already defined for a column for the same field
		$.each(grid.getColumns(), function (index, columnElem) {
			if (columnElem.field === decodeURIComponent(addedColumns[i].name)) {
				columnDefinition.filter = columnElem.filter;
			}
		});

		var columns = grid.getColumns().slice(0);
		columns.splice(posToInsert, 0, columnDefinition);

		grid.setColumns(columns);

		// update the filter parameters for the dataview
		FMOperateIndex.updateFilterParameters(grid, FMOperateIndex.movementSummaryControllers[newId].getMetadata());
		grid.getData().refresh();

		grid.resizeCanvas();

		theShadowGrid.addColumn(columnDefinition);
	}
	grid.setSelectedRows(selectedRows);
};

FMMovementSummaryTab.RemoveColumnFromTheSummaryBasedOnDBChange = function (colId, field, newId) {
	var grid = FMOperateIndex.movementSummaryControllers[newId]._grid;
	var theShadowGrid = FMOperateIndex.movementSummaryControllers[newId];
	var dataView = grid.getData();
	var columns = grid.getColumns().slice(0);
	var pos = columns.map(function (e) {
		return e.id;
	}).indexOf(colId);

	columns.splice(pos, 1);

	var pos = columns.map(function (e) {
		return e.field;
	}).indexOf(field);

	if (pos === -1) {
		theShadowGrid.deleteColumn(field);
	}

	grid.setColumns(columns);

	FMOperateIndex.updateFilterParameters(grid, FMOperateIndex.movementSummaryControllers[newId].getMetadata());
	dataView.refresh();
};

FMMovementSummaryTab.ReinitializeMovementSummaryAsync = async function (movementSummaryConfiguration, newId) {
	let grid = FMOperateIndex.movementSummaryControllers[newId].getGrid();
	let mutex = FMOperateIndex.movementSummaryControllers[newId].getMutex();
	await mutex.acquire();
	try {
		let rowDefinitions = FMOperateIndex.movementSummaryControllers[newId].rowDefinitions;
		let columnDefinitions = FMOperateIndex.movementSummaryControllers[newId].columnDefinitions;

		if (FMOperateIndex.movementSummaryControllers.hasOwnProperty(newId)) {
			let dataView = grid.getData();

			let rcvdColDefinitions = JSON.parse(movementSummaryConfiguration.Columns);
			let rcvdRowDefinitions = JSON.parse(movementSummaryConfiguration.Rows);

			if (JSON.stringify(rcvdColDefinitions) !== columnDefinitions) {

				if (typeof (columnDefinitions) === 'string') {
					columnDefinitions = JSON.parse(columnDefinitions);
				}

				// find removed columns
				const removedColumns = columnDefinitions.filter(colExisting =>
					!rcvdColDefinitions.some(colReceived => colExisting.id === colReceived.id)
				);

				for (let i = 0; i < removedColumns.length; i++) {
					FMMovementSummaryTab.RemoveColumnFromTheSummaryBasedOnDBChange(decodeURIComponent(removedColumns[i].id), decodeURIComponent(removedColumns[i].field), newId);
				}

				// find added columns
				const addedColumns = rcvdColDefinitions.filter(colReceived =>
					!columnDefinitions.some(colExisting => colReceived.id === colExisting.id)
				);

				if (addedColumns.length > 0) {
					FMMovementSummaryTab.AddColumnToTheSummaryBasedOnDBChange(addedColumns, rcvdColDefinitions, newId);
				}

				// save received for the next reference
				FMOperateIndex.movementSummaryControllers[newId].columnDefinitions = JSON.stringify(rcvdColDefinitions);
			}

			if (JSON.stringify(rcvdRowDefinitions) !== rowDefinitions) {


				if (typeof (rowDefinitions) === 'string') {
					rowDefinitions = JSON.parse(rowDefinitions);
				}

				// find removed movements
				const removedMovements = rowDefinitions.filter(pointExisting =>
					!rcvdRowDefinitions.some(pointReceived => (pointExisting.point === pointReceived.point && pointExisting.rowType === 'movement'))
				);

				for (let i = 0; i < removedMovements.length; i++) {
					if (removedMovements[i].type !== 'blank'
					&& removedMovements[i].rowType !== 'node') {

						dataView.beginUpdate();

						FMMovementSummaryTab.RemoveMovementRow(newId,
							FMOperateIndex.movementSummaryControllers[newId]._grid,
							dataView.getItems().findIndex(row => (row.rowType === 'movement' && row.point === removedMovements[i].point)));

						dataView.endUpdate();
					}
				}

				// find added movements
				const addedMovements = rcvdRowDefinitions.filter(pointReceived =>
					!rowDefinitions.some(pointExisting => pointReceived.point === pointExisting.point && pointExisting.rowType === 'movement')
				);

				//add to the grid
				for (let i = 0; i < addedMovements.length; i++) {
                    if (addedMovements[i].type !== 'blank'
					&& addedMovements[i].rowType !== 'node') {

						dataView.beginUpdate();


						FMMovementSummaryTab.AddMovementRow(newId,
							FMOperateIndex.movementSummaryControllers[newId]._grid,
							addedMovements[i].point, addedMovements[i].pointguid);

						dataView.endUpdate();
					}
				}

				// save received for the next reference
				FMOperateIndex.movementSummaryControllers[newId].rowDefinitions = JSON.stringify(rcvdRowDefinitions);

				grid.resizeCanvas();
				grid.scrollRowIntoView(0, false);
				grid.invalidateAllRows();
				grid.render();

			}
		}
	} finally {
		mutex.release(); // Ensure the lock is released
		return Promise.resolve();
	}
}

//===============================================================================
// This function will create a movement summary tab grid.
//===============================================================================
FMMovementSummaryTab.CreateMovementSummaryTabGrid = function (movementSummaryGuid, movementSummaryId, activeTab, newId, isNewMovementSummary, stack_bottomright_operatortab)
{
	FMMovementSummaryTab.movementSummaryGuid = movementSummaryGuid;

	$.ajax({
		type: 'get',
		dataType: 'json',
		cache: false,
		url: 'GetOperateMovementSummary',
		activeTab: activeTab,
		newId: newId,
		data: { "id": movementSummaryGuid, "movementName": movementSummaryId },
		success: function (response)
		{
			$("#loadermovementsummary" + newId).remove();

			var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

			FMErrorAndExceptionHandling.HandleMessages(response,
				function (movementSummaryConfiguration, inError)
				{
					//if it was not in error load and update the movement summary
					if (!inError) {
						_movementSummaryGuid = movementSummaryConfiguration.movementSummaryGuid;
						var data = [];
						var columns = [];
						var fontSize = (movementSummaryConfiguration && movementSummaryConfiguration.FontSize) ? parseInt(movementSummaryConfiguration.FontSize) : 14;

						// Formatter for the cells in the slickgrid 
						//( this is just a proxy to call the formatter in Movement Summary which may not have been defined yet, it gets defined after the grid is created but this function can be called before is actually created)
						function movementSummaryFormatter(row, cell, value, columnDef, dataContext) {
							if (FMOperateIndex.movementSummaryControllers[newId])
							{
								return FMOperateIndex.movementSummaryControllers[newId].movementSummaryFormatter(row, cell, value, columnDef, dataContext);
							}
							else
							{
								return '';
							}
						}					

						function movementSummaryNameFormatter(row, cell, value, columnDef, dataContext) {
							if (FMOperateIndex.movementSummaryControllers[newId]) {
								return FMOperateIndex.movementSummaryControllers[newId].movementSummaryNameFormatter(row, cell, value, columnDef, dataContext);
							}
							else {
								return '';
							}
						}


						columns = JSON.parse(movementSummaryConfiguration.Columns);
						data = JSON.parse(movementSummaryConfiguration.Rows);

						data = $.map(data, function (row, idx)
						{
							if (row.type === "point")
							{
								// Match with row definition
								return {
									id: row.id,
									type: row.type,
									point: row.point,
									pointguid: row.pointguid,
									rowType: row.rowType,
									direction: row.direction,
									parentRowId: row.parentRowId,
									masterRowId: row.masterRowId,
									individualNodeControl: row.individualNodeControl,
									movementguid: row.movementguid
								};
							}
							else
							{
								return row;
							}

						});

						// Recreate menu items in columns
						for (var i = 0; i < columns.length; i++) {
							columns[i].header = {
								menu: { items: FMOperateIndex.MovementSummaryCreateHeaderMenu() }
							};
							columns[i].formatter = (i == 0) ? movementSummaryNameFormatter : movementSummaryFormatter;
							columns[i].name = decodeURIComponent(columns[i].name);
							if ($('#ModifyMovementSummaryRight').val() == 'False') {
								columns[i].resizable = false;
							}
							else {
								columns[i].resizable = true;
							}
						}

						var options =
						{
							editable: true,
							enableCellNavigation: true,
							enableColumnReorder: true,
							forceFitColumns: false,
							asyncEditorLoading: false,
							autoEdit: false,
							fontSize: fontSize,
							rowHeight: 35,
							cellMenu: { items: FMOperateIndex.MovementSummaryCreateCellMenu() }
						};

						// Add menu items to columns
						for (var j = 0; j < columns.length; j++) {
							columns[j].header = {
								menu: { items: FMOperateIndex.MovementSummaryCreateHeaderMenu() }
							};
						}

						itemMap = FMMovementSummaryTab.groupBy(data, 'parentRowId');
						data = FMMovementSummaryTab.initTree(data, itemMap);

						dataView = new Slick.Data.DataView({ inlineFilters: true });

						let grid = new Slick.Grid("#movementsummary" + newId + 'container', dataView, columns, options);

						grid[newId] = grid;

						grid.setSelectionModel(new Slick.RowSelectionModel());

						// wire up model events to drive the grid
						dataView.onRowCountChanged.subscribe(function (e, args) {
							grid.updateRowCount();
							grid.render();
						});

						dataView.onRowsChanged.subscribe(function (e, args) {
							grid.invalidateRows(args.rows);
							grid.render();
						});

						grid.onSort.subscribe(function (e, args) {
							var dataView = grid.getData();
							var data = dataView.getItems();

							if (!data) {
								return;
							}

							FMMovementSummaryTab.DisableSummaryRefreshTimer(newId, true);
							var itemMap = FMMovementSummaryTab.groupBy(data, 'parentRowId');
							FMMovementSummaryTab.sortTree(itemMap, grid, args);

							data = FMMovementSummaryTab.initTree(data, itemMap);

							dataView.setItems(data);
							dataView.refresh();
							grid.invalidate();
							grid.render();
							FMMovementSummaryTab.DisableSummaryRefreshTimer(newId, false);

						});

						grid.onClick.subscribe(function (e, args) {
							if ($(e.target).hasClass("cell-toggle")) {
								var item = dataView.getItem(args.row);
								if (item) {
									if (!item._collapsed) {
										item._collapsed = true;
									} else {
										item._collapsed = false;
									}

									dataView.updateItem(item.id, item);
								}
								e.stopImmediatePropagation();

								FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
							}
						});

						$(grid.getCanvasNode()).on('dblclick', function (e) {

							// if we are not clicking on the left mouse button then ignore the event
							if (e.which !== 1) {
								return;
							}

							var cell = grid.getCellFromEvent(e);
							if (!cell) {
								return;
							}

							if (e.isImmediatePropagationStopped()) {
								return;
							}

							var rows = grid.getData().getFilteredItems();
							if (rows[cell.row].type != 'point') {
								return;
							}

							if (columns[cell.cell].field === 'point') {
								FMOperateIndex.openPoint(rows[cell.row].point, rows[cell.row].pointguid);
								return;
							}
						});

						$(grid.getCanvasNode()).on('mouseup', function (e) {

							// if we are not clicking on the left mouse button then ignore the event
							if (e.which !== 1) {
								return;
							}

							var gridCell = grid.getCellFromEvent(e);

							if (!gridCell) {
								return;
							}

							if (e.isImmediatePropagationStopped()) {
								return;
							}

							var rows = grid.getData().getFilteredItems();
							if (rows[gridCell.row].type != 'point') {
								return;
							}


							var columns = grid.getColumns();
							if (columns[gridCell.cell].field == ''
								|| columns[gridCell.cell].field == 'ID'
								|| columns[gridCell.cell].field == 'nodeName'
								|| columns[gridCell.cell].field == 'direction' 
								|| columns[gridCell.cell].field == 'IndividualNodeControl') {
								return;
							}

							var container = grid.getContainerNode();
							var tabContent = $(container).parent().parent().parent();
							var movementSummaryControllerId = $(tabContent).children('.active').attr("id");
							var movementSummaryGrid = FMOperateIndex.movementSummaryControllers[movementSummaryControllerId];
							var metadata = movementSummaryGrid.getMetadata();


							// metadata doesn't necessarily align with grid.
							var metaDataRow;
							for (metaDataRow = 0; metaDataRow < metadata.length; metaDataRow++) {
								if (metadata[metaDataRow].point == rows[gridCell.row].point) {
									break;
								}
							}

							if (metaDataRow == metadata.length) {
								return;
							}

							var metaDataCell;
							for (metaDataCell = 0; metaDataCell < metadata[metaDataRow].tags.length; metaDataCell++) {
								if (metadata[metaDataRow].tags[metaDataCell].ID == columns[gridCell.cell].field) {
									break;
								}
							}

							if (metaDataCell == metadata[metaDataRow].tags.length) {
								return;
							}

							var tag = metadata[metaDataRow].tags[metaDataCell];

							var pointValueIdentifier = { IdentityGuid: tag.PointTagGuid, PointValueType: 0, PropertyID: null };
							if (tag.Access
								&& tag.Access.Modify
								&& (tag.InputOutputType === 1
									|| (tag.Access.Override == true
										&& tag.InhibitOverride == false))) {
								FMOperateIndex.editValue(pointValueIdentifier);
							}
						});

						// initialize the model after all the events have been hooked up
						dataView.beginUpdate();
						dataView.setItems(data);
						FMOperateIndex.movementSummaryControllers[newId] = new FMMovementSummaryGrid(activeTab, newId, grid, movementSummaryConfiguration.Description, movementSummaryConfiguration.MovementSummaryType, movementSummaryConfiguration.FontSize, movementSummaryConfiguration.Owner, movementSummaryConfiguration.IsOwnedByMe, movementSummaryConfiguration.IsEditable);
						var theShadowGrid = FMOperateIndex.movementSummaryControllers[newId];

						FMOperateIndex.updateFilterParameters(grid, FMOperateIndex.movementSummaryControllers[newId].getMetadata());
						
						dataView.setFilter(FMMovementSummaryTab.FilterMovementSummaryGrid);

						dataView.endUpdate();

						// if you don't want the items that are not visible (due to being filtered out
						// or being on a different page) to stay selected, pass 'false' to the second arg
						dataView.syncGridSelection(grid, true);

						/*--------------- DISPLAY FILTER INDICATOR FOR COLUMN -----------------*/
						var filterIndicatorPlugin = new Slick.Plugins.HeaderFilterIndicator();

						grid.registerPlugin(filterIndicatorPlugin);
						/*--------------- DISPLAY FILTER INDICATOR FOR COLUMN  -----------------*/


						// Persist the new tab so it can be re-open when the screen is reloaded
						var columnsWithNoMenu = $.extend(true, [], columns);  // copy array by value so we don't lose the original menu
						columnsWithNoMenu = $.map(columnsWithNoMenu, function (val, i) { val.header = null; return val; });

						var myGridColumns = grid.getColumns();

						// disable the column reordering in the first column (the name)
						grid.onColumnsReordered.subscribe(function (e, args)
						{
							if (myGridColumns[0].id !== grid.getColumns()[0].id)
							{
								grid.setColumns(myGridColumns);
							}
							else
							{
								myGridColumns = grid.getColumns();
							}

							FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);

						});

						/*--------------- EVENTS  -----------------*/
						// event to switch between configuration and grid
						$('#movementsummary' + newId + 'switch').on('click', function () {

							if ($('#movementsummary' + newId + 'container').hasClass('active')) {
								$('#movementsummary' + newId + 'container').fadeOut("slow", function () {
									$('#movementsummary' + newId + 'container').removeClass("active");
								});
								$('#movementsummary' + newId + 'settings').fadeIn("slow", function () {
									$('#movementsummary' + newId + 'settings').addClass("active");
									$('#movementsummary' + newId + 'switch').removeClass('glyphicon-cog').addClass('glyphicon-th').attr('title', 'Grid');
									$('#movementsummary' + newId + 'switch label').text('Grid');
								});
							}
							else {
								$('#movementsummary' + newId + 'settings').fadeOut("slow", function () {
									$('#movementsummary' + newId + 'settings').removeClass("active");
								});
								$('#movementsummary' + newId + 'container').fadeIn("slow", function () {
									$('#movementsummary' + newId + 'container').addClass("active");
									$('#movementsummary' + newId + 'switch').removeClass('glyphicon-th').addClass('glyphicon-cog').attr('title', 'Configuration');
									$('#movementsummary' + newId + 'switch label').text('Configuration');
									grid.resizeCanvas();
								});
							}
						});

						// if movement summary is not editable no need for menus or events
						if (movementSummaryConfiguration.IsEditable)
						{
							// Double click on the tab name to rename the point group
							$('a[data-target="#' + newId + '"]').attr('ondblclick', "FMOperateIndex.RenameTab( this );");

							/*--------------- DRAG ROWS TO MOVE  -----------------*/
							grid.setSelectionModel(new Slick.RowSelectionModel());

							var moveRowsPlugin = new Slick.RowMoveManager({
								cancelEditOnDrag: true
							});

							moveRowsPlugin.onBeforeMoveRows.subscribe(function (e, data) {
								for (var i = 0; i < data.rows.length; i++) {
									// no point in moving before or after itself
									if (data.rows[i] == data.insertBefore || data.rows[i] == data.insertBefore - 1) {
										e.stopPropagation();
										return false;
									}
								}
								return true;
							});

							moveRowsPlugin.onMoveRows.subscribe(function (e, args) {
								var extractedRows = [], left, right;
								var rows = args.rows;
								var insertBefore = args.insertBefore;
								var dataView = grid.getData();
								var data = dataView.getItems();

								dataView.beginUpdate();

								// delete the rows from the grid
								for (var i = 0; i < rows.length; i++) {
									extractedRows.push(dataView.getItem(rows[i]));
									dataView.deleteItem(dataView.getItem(rows[i]).id);
								}

								// find where we need to add them
								if (dataView.getItem(insertBefore))  // if not in the last row
								{
									var insertBeforeId = dataView.getItem(insertBefore).id;
									var dataViewInsertBefore = dataView.getIdxById(insertBeforeId);

									for (var i = 0; i < extractedRows.length; i++) {
										dataView.insertItem(dataViewInsertBefore, extractedRows[i]);
									}
								}
								else {
									var lastPosition = dataView.getLength();

									for (var i = 0; i < extractedRows.length; i++) {
										dataView.insertItem(lastPosition, extractedRows[i]);
									}
								}
								dataView.endUpdate();
								FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);

								grid.invalidateAllRows();
								grid.render();
							});

							grid.registerPlugin(moveRowsPlugin);

							/*--------------- END DRAG ROWS TO MOVE  -----------------*/

							/*--------------- COLUMN MENU  -----------------*/
							var headerMenuPlugin = new Slick.Plugins.HeaderContextMenu({});

							headerMenuPlugin.onBeforeMenuShow.subscribe(function (e, args)
							{
								// get the different engineering unit types that we have for the points selected in the grid
								var tagValueTypes = FMOperateIndex.movementSummaryControllers[newId].getValueTypesForTag(args.column.field);
								// default the menu items to be enabled
								args.menu.items[2].disabled = false;	// Delete Column
								args.menu.items[4].disabled = true;	// Filter
								args.menu.items[5].disabled = false;	// Set Display Precision
								args.menu.items[6].disabled = false;	// Set Display Unit
								args.menu.items[7].disabled = false;	// Show Units
								args.menu.items[8].disabled = false;	// Show Quality

								// if no rights, disable all except the show commands
								if ($('#ModifyMovementSummaryRight').val() == 'False') {
									args.menu.items[0].disabled = true;	// Cell Aignment
									args.menu.items[1].disabled = true;	// Insert Column
									args.menu.items[2].disabled = true;	// Delete Column
									args.menu.items[3].disabled = true;	// Rename
									args.menu.items[4].disabled = true;	// Filter
									args.menu.items[5].disabled = true;	// Set Display Precision
									args.menu.items[6].disabled = true;	// Set Display Unit
								}
								// only allow filter the selected columns which pertain to movement
								if (FMMovementSummaryTab.IsMovementColumn(args.column.field)) {
									args.menu.items[4].disabled = false;	// Filter
								}
								// columns to not allow to delete and are non numeric
								if (args.column.field === 'PointId'
									|| args.column.field === 'Status'
									|| args.column.field === 'TransferDirection'
									|| args.column.field === 'TransferStatus'
									|| args.column.field === 'Type')
								{
									args.menu.items[2].disabled = true;	// Delete Column
									args.menu.items[5].disabled = true;	// Set Display Precision
									args.menu.items[6].disabled = true;	// Set Display Unit
									args.menu.items[7].disabled = true;	// Show Units
									args.menu.items[8].disabled = true;	// Show Quality
								}

								// if we don't have any points (no unit types ) or we have points of multiple types then we cannot set the unit or precision
								else if ((tagValueTypes.length === 0 || tagValueTypes.length > 1)) {

									// do not diable Filter if one is present
									if (!args.column.hasOwnProperty('filter')) {
										args.menu.items[4].disabled = true;	// Filter
									}
									args.menu.items[5].disabled = true;	// Set Display Precision
									args.menu.items[6].disabled = true;	// Set Display Unit
									args.menu.items[7].disabled = true;	// Show Units
									args.menu.items[8].disabled = true;	// Show Quality
								}
								else {

									var tagValue = tagValueTypes[0];

									if (tagValue === "System.Boolean")
									{
										args.menu.items[5].disabled = true;	// Set Display Precision
										args.menu.items[6].disabled = true;	// Set Display Unit
										args.menu.items[7].disabled = true;	// Show Units
									}
									else if (tagValue === "System.Double" || tagValue === "System.Int16" || tagValue === "System.Int32" || tagValue === "System.Int64") {
										var numericUnitTypes = FMOperateIndex.movementSummaryControllers[newId].getNumericUnitTypesForTag(args.column.field);
										var numericValueTypes = FMOperateIndex.movementSummaryControllers[newId].getValueTypesForTag(args.column.field);

										var numericUnitType = -9999;

										if (numericUnitTypes.length === 1)
										{
											numericUnitType = numericUnitTypes[0];
										}

										var numericValueType = "None";

										if (numericValueTypes.length === 1)
										{
											numericValueType = numericValueTypes[0];
										}

										if (numericUnitType !== -9999
											&& numericValueType !== "System.Double"
											&& numericValueType !== "System.Int16"
											&& numericValueType !== "System.Int32"
											&& numericValueType !== "System.Int64")
										{
											args.menu.items[4].disabled = true;	// Filter
											args.menu.items[5].disabled = true;	// Set Display Precision
											args.menu.items[6].disabled = true;	// Set Display Unit
											args.menu.items[7].disabled = true;	// Show Units
										}
										else if (numericUnitType === 15) // if no units then disable the menus
										{
											args.menu.items[6].disabled = true;	// Set Display Unit
											args.menu.items[7].disabled = true;	// Show Units
										}

										// integers don't need precision since they its always zero
										if (numericValueType === "System.Int16" && numericValueType === "System.Int32" && numericValueType === "System.Int64")
										{
											args.menu.items[5].disabled = true;
										}
									}
									else if (tagValue === "System.String")
									{
										args.menu.items[5].disabled = true;	// Set Display Precision
										args.menu.items[6].disabled = true;	// Set Display Unit
										args.menu.items[7].disabled = true;	// Show Units
									}
									else if (tagValue === "System.DateTimeOffset")
									{
										args.menu.items[5].disabled = true;	// Set Display Precision
										args.menu.items[6].disabled = true;	// Set Display Unit
										args.menu.items[7].disabled = true;	// Show Units
									}
									else if (tagValue === "System.TimeSpan")
									{
										args.menu.items[5].disabled = true;	// Set Display Precision
										args.menu.items[6].disabled = true;	// Set Display Unit
										args.menu.items[7].disabled = true;	// Show Units
									}
									else if (tagValue && tagValue.startsWith("FMBusinessObjects.DataObjects.CodedVariables"))
									{
										args.menu.items[5].disabled = true;	// Set Display Precision
										args.menu.items[6].disabled = true;	// Set Display Unit
										args.menu.items[7].disabled = true;	// Show Units
									}
									else // not a valid combination so disable the menus (no boolean, or numeric types)
									{
										args.menu.items[4].disabled = true;	// Filter
										args.menu.items[5].disabled = true;	// Set Display Precision
										args.menu.items[6].disabled = true;	// Set Display Unit
										args.menu.items[7].disabled = true;	// Show Units
									}
								}

								if (args.column.hasOwnProperty('showunit'))
								{
									if (args.column.showunit)
									{
										args.menu.items[7].title = 'Hide Units';
										args.menu.items[7].iconCssClass = 'header-menu-hide-unit';
									}
									else
									{
										args.menu.items[7].title = 'Show Units';
										args.menu.items[7].iconCssClass = 'header-menu-show-unit';
									}
								}
								if (args.column.hasOwnProperty('showquality'))
								{
									if (args.column.showquality)
									{
										args.menu.items[8].title = 'Hide Quality';
										args.menu.items[8].iconCssClass = 'header-menu-hide-quality';
									}
									else
									{
										args.menu.items[8].title = 'Show Quality';
										args.menu.items[8].iconCssClass = 'header-menu-show-quality';
									}
								}

								// if we have a cell menu displayed we also want to close them before displaying the menu (we do it here because we stop the event propagation )
								$("#" + FMMovementSummaryTab.newId).find('.point-group').find('.slick-cellcontext-menu').each(function () {
									$(this).hide();
								});


								e.preventDefault();
							});

							headerMenuPlugin.onCommand.subscribe(function (e, args) {
								if (args.command === "insert-column-tag") {
									selectTagColumn(args);
								}
								else if (args.command === "insert-product-name") {
									insertPointPropertyColumn(args, "ProductID", "Product Name");
								}
								else if (args.command === "insert-product-description") {
									insertPointPropertyColumn(args, "ProductDescription", "Product Description");
								}
								else if (args.command === "insert-empty-column") {
									insertEmptyColumn(args);
								}
								else if (args.command === "center-align") {
									var headerCss = "";
									if (args.column.cssClass) {
										headerCss = args.column.cssClass.trim();
									}
									headerCss = headerCss.replace('text-left', '');
									headerCss = headerCss.replace('text-right', '');
									headerCss = headerCss.replace('text-center', '');
									headerCss += ' text-center';
									args.column.cssClass = headerCss;
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
								}
								else if (args.command === "left-align") {
									var headerCss = "";
									if (args.column.cssClass) {
										headerCss = args.column.cssClass.trim();
									}
									headerCss = headerCss.replace('text-left', '');
									headerCss = headerCss.replace('text-right', '');
									headerCss = headerCss.replace('text-center', '');
									headerCss += ' text-left';
									args.column.cssClass = headerCss;
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
								}
								else if (args.command === "right-align") {
									var headerCss = "";
									if (args.column.cssClass) {
										headerCss = args.column.cssClass.trim();
									}
									headerCss = headerCss.replace('text-left', '');
									headerCss = headerCss.replace('text-right', '');
									headerCss = headerCss.replace('text-center', '');
									headerCss += ' text-right';
									args.column.cssClass = headerCss;
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
								}
								else if (args.command === "rename") {
									var popover = $(args.headercell).popover("destroy").popover({
										container: 'body',
										placement: 'bottom',
										html: true,
										content: $('#renameMovementSummaryHeader').html(),
										trigger: "manual"
									});
									var dataPopover = popover.data('bs.popover');
									$("#customModalBackground").removeClass("hidden");
									$(args.headercell).popover('show');

									// update values in the popover
									dataPopover.tip().find('.popover-content').find('#fieldname').val(args.column.field);
									dataPopover.tip().find('.popover-content').find('#header').val(args.column.name);
									dataPopover.tip().find('.popover-content').find('#header').focus();

									// click on reset name
									dataPopover.tip().find('.popover-content').find('[name=renamemovementsummaryResetName]').on('click', function (event) {
										var resetName = args.column.field;
										//Point Properties need to be properly renamed since we cannot use the propertyID
										if (resetName === "ProductID") {
											resetName = "Product Name";
										}
										if (resetName === "ProductDescription") {
											resetName = "Product Description";
										}
										dataPopover.tip().find('.popover-content').find('#header').val(resetName);
									});

									// close the pop over when clicking cancel
									dataPopover.tip().find('.popover-content').find('[name=renamemovementsummarycancel]').on('click', function (event) {
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										event.stopPropagation();
									});
									// update the column name when clicking ok
									dataPopover.tip().find('.popover-content').find('[name=renamemovementsummaryok]').on('click', function (event) {
										args.grid.updateColumnHeader(args.column.id, dataPopover.tip().find('.popover-content').find('#header').val());
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
										event.stopPropagation();
									});
								}
								else if (args.command === "delete-column") {
									FMLayout.ConfirmYesNo("Are you sure you want to delete the selected column: " + args.column.name + "?", "Delete Column", function () {
										var columns = grid.getColumns().slice(0);
										var pos = columns.map(function (e) {
											return e.id;
										}).indexOf(args.column.id);
										columns.splice(pos, 1);

										var pos = columns.map(function (e) {
											return e.field;
										}).indexOf(args.column.field);

										if (pos === -1) {
											theShadowGrid.deleteColumn(args.column.field);
										}

										grid.setColumns(columns);

										FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);

										FMOperateIndex.updateFilterParameters(grid, FMOperateIndex.movementSummaryControllers[newId].getMetadata());
										dataView.refresh();

									});
								}
								else if (args.command === "changeprecision") {
									var popover = $(args.headercell).popover("destroy").popover({
										container: 'body',
										placement: 'bottom',
										html: true,
										content: $('#changePrecisionMovementSummaryHeader').html(),
										trigger: "manual"
									});
									var dataPopover = popover.data('bs.popover');
									$("#customModalBackground").removeClass("hidden");
									$(args.headercell).popover('show');

									// add an id to the checkbox so the label automatically changes the checkbox (if specified in the html we get duplicates because it makes a copy of the html and it will not work)
									dataPopover.tip().find('.popover-content').find('input[type=checkbox]').attr("id", "changePrecisionDefaultToPoint");

									var precisionField = dataPopover.tip().find('.popover-content').find('[name=numDecimals]');
									$(precisionField).spinner({
										min: 0,
										max: 9,
										step: 1
									}).on('input', function () {
										if ($(this).data('onInputPrevented'))
											return;
										var val = this.value,
											$this = $(this),
											max = $this.spinner('option', 'max'),
											min = $this.spinner('option', 'min');
										// We want only number, no alpha. 
										// We set it to previous default value.         
										if (!val.match(/^[+-]?[\d]{0,}$/))
											val = $(this).data('defaultValue');
										this.value = val > max ? max : val < min ? min : val;
									}).on('keydown', function (e) {
										// we set default value for spinner.
										if (!$(this).data('defaultValue'))
											$(this).data('defaultValue', this.value);
										// To handle backspace
										$(this).data('onInputPrevented', e.which === 8 ? true : false);
									}); // set default value
									// update values in the popover
									if (args.column.hasOwnProperty('DecimalPlaces')) {
										if (args.column['DecimalPlaces'] === -1) {
											$(precisionField).spinner("value", 0);
											$(precisionField).spinner('disable');
											dataPopover.tip().find('.popover-content').find('[name=changePrecisionDefaultToPoint]').prop('checked', true);
										}
										else {
											$(precisionField).spinner("value", args.column['DecimalPlaces']);
											$(precisionField).spinner('enable');
											dataPopover.tip().find('.popover-content').find('[name=changePrecisionDefaultToPoint]').prop('checked', false);
										}
									}
									else {
										$(precisionField).spinner("value", 0);
										$(precisionField).spinner('disable');
										dataPopover.tip().find('.popover-content').find('[name=changePrecisionDefaultToPoint]').prop('checked', true);
									}


									// close the pop over when clicking cancel
									dataPopover.tip().find('.popover-content').find('[name=changePrecisionMovementSummaryCancel]').on('click', function (event) {
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										event.stopPropagation();
									});
									// update the column name when clicking ok
									dataPopover.tip().find('.popover-content').find('[name=changePrecisionMovementSummaryOk]').on('click', function (event) {
										if (dataPopover.tip().find('.popover-content').find('[name=changePrecisionDefaultToPoint]').prop('checked')) {
											args.column['DecimalPlaces'] = -1;
										}
										else {
											args.column['DecimalPlaces'] = $(precisionField).spinner('value');
										}
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										grid.invalidateAllRows();
										grid.render();
										FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
										event.stopPropagation();
									});
								}
								else if (args.command === "changeunit") {
									var popover = $(args.headercell).popover("destroy").popover({
										container: 'body',
										placement: 'bottom',
										html: true,
										content: $('#changeUnitMovementSummaryHeader').html(),
										trigger: "manual"
									});
									var dataPopover = popover.data('bs.popover');
									$(args.headercell).popover('show');
									$("#customModalBackground").removeClass("hidden");

									// add an id to the checkbox so the label automatically changes the checkbox (if specified in the html we get duplicates because it makes a copy of the html and it will not work)
									dataPopover.tip().find('.popover-content').find('input[type=checkbox]').attr("id", "changeUnitDefaultToPoint");

									// update values in the popover
									loadUnitsByUnitType(dataPopover.tip().find('.popover-content').find('[name=changeunitUOMList]'), args.column);

									// close the pop over when clicking cancel
									dataPopover.tip().find('.popover-content').find('[name=changeUnitMovementSummaryCancel]').on('click', function (event) {
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										event.stopPropagation();
									});

									// update unit when clicking ok
									dataPopover.tip().find('.popover-content').find('[name=changeUnitMovementSummaryOk]').on('click', function (event) {
										if (dataPopover.tip().find('.popover-content').find('[name=changeUnitDefaultToPoint]').prop('checked')) {
											args.column['Unit'] = -1;
										}
										else {
											var selectedUnit = dataPopover.tip().find('.popover-content').find('.list-group-item.active');
											if (selectedUnit.length === 1) {
												args.column['Unit'] = parseInt(selectedUnit.attr('data-value'));
											}
										}
										grid.invalidateAllRows();
										grid.render();
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
										event.stopPropagation();
									});
								}
								else if (args.command === "showunits") {
									if (args.column.hasOwnProperty('showunit')) {
										args.column.showunit = !args.column.showunit;
									}
									else {
										args.column.showunit = true;
									}
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
									grid.invalidateAllRows();
									grid.render();
								}
								else if (args.command === "showquality") {
									if (args.column.hasOwnProperty('showquality')) {
										args.column.showquality = !args.column.showquality;
									}
									else {
										args.column.showquality = true;
									}
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
									grid.invalidateAllRows();
									grid.render();
								}
								else if (args.command === "filter") {
									var tagValueTypes = FMOperateIndex.movementSummaryControllers[newId].getValueTypesForTag(args.column.field);

									// if no  tagValueTypes then menu filter is enabled based upon current filter
									if (tagValueTypes.length === 0) {
										if (args.column.filter.type === 'numeric') {
											numericFilter(args);
										}
										else if (args.column.filter.type === 'boolean') {
											booleanFilter(args);
										}
										else if (args.column.filter.type === 'datetimeoffset') {
											FMMovementSummaryTab.DateTimeOffsetFilter(args);
										}
										else if (args.column.filter.type === 'timespan') {
											timeSpanFilter(args);
										}
										else if (args.column.filter.type === 'string') {
											stringFilter(args);
										}
										else if (args.column.filter.type === 'enum') {
											enumFilter(args);
										}
									}
									else {
										var valueType = tagValueTypes[0];
										if (valueType === "System.Double" || valueType === "System.Int16" || valueType === "System.Int32" || valueType === "System.Int64") {
											numericFilter(args);
										}
										else if (valueType === "System.Boolean") {
											booleanFilter(args);
										}
										else if (valueType === "System.DateTimeOffset") {
											FMMovementSummaryTab.DateTimeOffsetFilter(args);
										}
										else if (valueType === "System.TimeSpan") {
											timeSpanFilter(args);
										}
										else if (valueType === "System.String") {
											stringFilter(args);
										} else if (valueType.startsWith("FMBusinessObjects.DataObjects.CodedVariables")) {
											enumFilter(args);
										}
									}
								}
							});

							// filter numeric columns
							var numericFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#NumericFilterMovementSummaryHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								//populate values from the stored filter
								if (args.column.hasOwnProperty('filter')) {
									dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitOperator]').val(args.column.filter.operator);
									if (args.column.filter.operator === "between" || args.column.filter.operator === "not_between") {
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMaxLabel]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeClass('hidden');
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMaxLabel]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeClass('hidden').addClass('hidden');
									}

									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val(args.column.filter.minValue);
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val(args.column.filter.maxValue);
								}

								dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').focus();

								var numericUnitType;
								var numericUnitTypes = FMOperateIndex.movementSummaryControllers[newId].getNumericUnitTypesForTag(args.column.field);
								if (numericUnitTypes.length === 1) {
									numericUnitType = numericUnitTypes[0];
								}
								else {
									if (args.column.filter.unitType) {
										numericUnitType = args.column.filter.unitType;
									}
								}

								// the default number of decimals is 2 unless is an int
								var defaultPrecision = 2;
								var numericValueTypes = FMOperateIndex.movementSummaryControllers[newId].getValueTypesForTag(args.column.field);

								if (numericValueTypes.length === 1 && numericValueTypes[0].startsWith("System.Int")) {
									defaultPrecision = 0;
								}


								// if we have a unit type of  FMU_All, FMU_NODIM or FMU_NONE we don't need units
								if (numericUnitType === 0 || numericUnitType === 15 || numericUnitType === 16) {
									dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').addClass("hidden");
									dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitLabel]').addClass("hidden");
								}
								else {
									loadUnitsByUnitType(dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]'), args.column);
								}

								// remember the unit before it can be changed
								dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').on('focus', function () {
									// Store the current value on focus
									$(this).data('oldValue', this.value);
								});
								// change the unit selection
								dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').on('change', function (event) {
									var newUnit = parseInt($(this).val());
									var oldUnit = parseInt($(this).data('oldValue'));

									// convert the unit
									var minValue = "";
									var maxValue = "";

									var numformatInfo = FMOperateIndex.numformatInfo;

									var numDecimals = defaultPrecision;
									if (args.column.hasOwnProperty('DecimalPlaces')) {
										if (args.column['DecimalPlaces'] !== -1)
											numDecimals = args.column['DecimalPlaces'];
									}
									numformatInfo.NumberDecimalDigits = numDecimals;

									// if old unit was feet-in-16th or feet-in-8th we were using a mask so we need to get the raw value and remove the mask
									if (oldUnit === 27 || oldUnit === 19) {
										minValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val();
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').unmask();
										maxValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val();
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').unmask();
									}
									else {
										minValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val();
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').removeNumeric();
										maxValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val();
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeNumeric();
									}

									if (minValue !== "") {
										var minRawValue = FMFormatValues.ParseValue(oldUnit, numformatInfo, minValue);
										var convertedMinRawValue = FMConvertEngUnits.Convert(minRawValue, oldUnit, newUnit);
										var newFormattedMinValue = FMFormatValues.FormatValue(newUnit, numformatInfo, convertedMinRawValue);
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val(newFormattedMinValue);
									}

									if (maxValue !== "") {
										var maxRawValue = FMFormatValues.ParseValue(oldUnit, numformatInfo, maxValue);
										var convertedMaxRawValue = FMConvertEngUnits.Convert(maxRawValue, oldUnit, newUnit);
										var newFormattedMaxValue = FMFormatValues.FormatValue(newUnit, numformatInfo, convertedMaxRawValue);
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val(newFormattedMaxValue);
									}

									// add the mask to the editor fields
									// if feet-in-16th or feet-in-8th use 00-00-00 as mask, otherwise is just plain numeric
									if (newUnit === 27) //FML_FtIn16th
									{
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').mask('S99-99-99', {
											translation: {
												'S': {
													pattern: /-/,
													optional: true
												}
											},
											placeholder: '__-__-__'
										});
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').mask('S99-99-99', {
											translation: {
												'S': {
													pattern: /-/,
													optional: true
												}
											},
											placeholder: '__-__-__'
										});
									}
									else if (newUnit === 19) //FML_FtIn8th
									{
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').mask('S99-99-9', {
											translation: {
												'S': {
													pattern: /-/,
													optional: true
												}
											},
											placeholder: '__-__-__'
										});
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').mask('S99-99-9', {
											translation: {
												'S': {
													pattern: /-/,
													optional: true
												}
											},
											placeholder: '__-__-__'
										});
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').attr('placeholder', '');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').attr('placeholder', '');

										var numDecimals = defaultPrecision;

										if (args.column.hasOwnProperty('DecimalPlaces')) {
											if (args.column['DecimalPlaces'] !== -1)
												numDecimals = args.column['DecimalPlaces'];
										}

										if (numDecimals === 0) {
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').numeric({
												decimal: false,
												negative: true
											});
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').numeric({
												decimal: false,
												negative: true
											});
										}
										else {
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').numeric({
												decimal: FMOperateIndex.numformatInfo.NumberDecimalSeparator,
												negative: true,
												decimalPlaces: numDecimals
											});
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').numeric({
												decimal: FMOperateIndex.numformatInfo.NumberDecimalSeparator,
												negative: true,
												decimalPlaces: numDecimals
											});
										}
									}

									// remember the old unit value
									$(this).data('oldValue', this.value);
								});

								// change the operator
								dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitOperator]').on('change', function (event) {
									var operator = $(this).val();
									if (operator === "between" || operator === "not_between") {
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMaxLabel]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeClass('hidden');
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMaxLabel]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val('');
									}
								});

								// change the values
								dataPopover.tip().find('.popover-content').find('[name=numericFilterMin], [name=numericFilterMax]').on('blur', function (event) {
									var numformatInfo = FMOperateIndex.numformatInfo;

									var numDecimals = defaultPrecision;
									if (args.column.hasOwnProperty('DecimalPlaces')) {
										if (args.column['DecimalPlaces'] !== -1)
											numDecimals = args.column['DecimalPlaces'];
									}

									var unit = dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').val();
									if ($(this).val() != "") {
										var newFormattedLevel = '';
										if (unit === '27') { //FML_FtIn16th
											newFormattedLevel = FMOperateIndex.convertFeetInch16thReadings($(this).val());
										}
										else if (unit === '19') { //FML_FtIn8th
											newFormattedLevel = FMOperateIndex.convertFeetInch8thReadings($(this).val());
										}
										else {
											numformatInfo.NumberDecimalDigits = numDecimals;
											var newLevel = FMFormatValues.ParseValue(unit, numformatInfo, $(this).val());
											newFormattedLevel = FMFormatValues.FormatValue(unit, numformatInfo, newLevel);
										}
										$(this).val(newFormattedLevel);
									}
								});


								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=numericFilterMovementSummaryReset]').on('click', function (event) {
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').removeNumeric();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeNumeric();

									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());

									grid.getData().refresh();
									grid.invalidateAllRows();
									grid.render();

									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=numericFilterMovementSummaryCancel]').on('click', function (event) {
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').removeNumeric();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeNumeric();

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=numericFilterMovementSummaryApply]').on('click', function (event) {
									var numformatInfo = FMOperateIndex.numformatInfo;
									var unit = -1;
									if (!dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').hasClass("hidden")) {
										unit = parseInt(dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').val());
									}

									var unitType = parseInt(dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitType]').val());


									var minValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val();
									var maxValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val();
									var operator = dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitOperator]').val();

									var missingMinValue = (minValue === "");
									var missingMaxValue = (maxValue === "" && (operator === "between" || operator === "not_between"));
									if (missingMinValue || missingMaxValue) {
										if (missingMinValue) {
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').parent().addClass('has-error');
										}
										if (missingMaxValue) {
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').parent().addClass('has-error');
										}
										return false;
									}

									// create a description to show in a tooltip
									var description = dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitOperator] option:selected').text() +
										" " + dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val();
									if (operator === "between" || operator === "not_between") {
										description += " and " + dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val();
									}
									if (!dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').hasClass("hidden")) {
										description += " (" + dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit] option:selected').text() + ")";
									}
									var filter = {
										type: 'numeric',
										unit: unit,
										unitType: unitType,
										operator: operator,
										minValue: minValue !== "" ? FMFormatValues.ParseValue(unit, numformatInfo, minValue).toString() : "",
										maxValue: maxValue !== "" ? FMFormatValues.ParseValue(unit, numformatInfo, maxValue).toString() : "",
										description: encodeURIComponent(description)
									};

									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').removeNumeric();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeNumeric();

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									// copy the filter to all the columns of the same field
									grid.setColumns(grid.getColumns());

									grid.getData().refresh();
									grid.invalidateAllRows();
									grid.render();

									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
								});
							}

							// filter boolean column
							var booleanFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#MovementSummaryFilterBooleanGroupHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								//populate values from the stored filter
								if (args.column.hasOwnProperty('filter')) {
									dataPopover.tip().find('.popover-content').find('[name=MovementSummaryFilterBoolean][value=' + args.column.filter.Value + ']').prop("checked", true);
								}
								else {
									// default filter to true value
									dataPopover.tip().find('.popover-content').find('[name=MovementSummaryFilterBoolean][value=true]').prop("checked", true);
								}



								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=booleanFilterMovementSummaryReset]').on('click', function (event) {

									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=booleanFilterMovementSummaryCancel]').on('click', function (event) {

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=booleanFilterMovementSummaryApply]').on('click', function (event) {
									var setValue = dataPopover.tip().find('.popover-content').find('[name=MovementSummaryFilterBoolean]:checked').val();

									// create a description to show in a tooltip
									var description = "Value is: " + dataPopover.tip().find('.popover-content').find('[name=MovementSummaryFilterBoolean]:checked').parent().text().trim();

									var filter = {
										type: 'boolean',
										Value: setValue,
										description: encodeURIComponent(description)
									};

									// copy the filter to all the columns of the same field
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
								});
							}

							// filter string column
							var stringFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#MovementSummaryFilterStringGroupHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								//populate values from the stored filter
								if (args.column.hasOwnProperty('filter')) {
									dataPopover.tip().find('.popover-content').find('[name=stringFilterMovementSummaryValue]').val(args.column.filter.Value);
								}
								dataPopover.tip().find('.popover-content').find('[name=stringFilterMovementSummaryValue]').focus();

								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=stringFilterMovementSummaryReset]').on('click', function (event) {
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=stringFilterMovementSummaryCancel]').on('click', function (event) {
									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=stringFilterMovementSummaryApply]').on('click', function (event) {
									var setValue = dataPopover.tip().find('.popover-content').find('[name=stringFilterMovementSummaryValue]').val();

									// create a description to show in a tooltip
									var description = "Value contains: " + (setValue === "" ? "Empty" : setValue);

									var filter = {
										type: 'string',
										Value: setValue,
										description: encodeURIComponent(description)
									};

									// copy the filter to all the columns of the same field
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
								});
							}

							// filter timeSpan column
							var timeSpanFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#MovementSummaryFilterTimeSpanHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMin]').mask('###.00:00:00', { reverse: true, placeholder: "__:__:__" });
								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').mask('###.00:00:00', { reverse: true, placeholder: "__:__:__" });

								//populate values from the stored filter
								if (args.column.hasOwnProperty('filter')) {
									dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterOperator]').val(args.column.filter.operator);
									if (args.column.filter.operator === "between" || args.column.filter.operator === "not_between") {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMaxLabel]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').removeClass('hidden');
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMaxLabel]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').removeClass('hidden').addClass('hidden');
									}

									var formatMinValue = args.column.filter.minValue.days + "." + args.column.filter.minValue.hours + ":" + args.column.filter.minValue.minutes + ":" + args.column.filter.minValue.seconds;
									dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMin]').val(formatMinValue);

									if (args.column.filter.maxValue != null && args.column.filter.minValue.days != 0 && args.column.filter.minValue.hours != 0 && args.column.filter.minValue.minutes != 0 && args.column.filter.minValue.seconds != 0) {
										var formatMaxValue = args.column.filter.maxValue.days + "." + args.column.filter.maxValue.hours + ":" + args.column.filter.maxValue.minutes + ":" + args.column.filter.maxValue.seconds;
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').val(formatMaxValue);
									}
								}

								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMin]').focus();

								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMovementSummaryReset]').on('click', function (event) {

									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMovementSummaryCancel]').on('click', function (event) {

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMovementSummaryApply]').on('click', function (event) {
									var minRawValue = dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMin]').val();
									var maxRawValue = dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').val();
									var operator = dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterOperator]').val();

									var minValue = validateTimeSpan(minRawValue);
									var maxValue = validateTimeSpan(maxRawValue);

									if (minValue == null) {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMin]').parent().addClass('has-error');
										return false;
									}
									if (maxValue == null && (operator === "between" || operator === "not_between")) {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').parent().addClass('has-error');
										return false;
									}

									// create a description to show in a tooltip
									var description = dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterOperator] option:selected').text() +
										" " + (minValue.days + "." + minValue.hours + ":" + minValue.minutes + ":" + minValue.seconds);
									if (operator === "between" || operator === "not_between") {
										description += " and " + (maxValue.days + "." + maxValue.hours + ":" + maxValue.minutes + ":" + maxValue.seconds);
									}

									var filter = {
										type: 'timespan',
										operator: operator,
										minValue: minValue,
										maxValue: maxValue,
										description: encodeURIComponent(description)
									};

									// copy the filter to all the columns of the same field
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
								});

								// change the operator
								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterOperator]').on('change', function (event) {
									var operator = $(this).val();
									if (operator === "between" || operator === "not_between") {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMaxLabel]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').removeClass('hidden');
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMaxLabel]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').val('');
									}
								});

							}

							// filter for enumerated columns
							var enumFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#MovementSummaryFilterEnumGroupHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								dataPopover.tip().find("[name=pointFilterEnumTagValues]").select2({
									multiple: true,
									placeholder: "Loading..."
								});

								loadOptionsForEnumValueType(dataPopover.tip().find('.popover-content').find('[name=pointFilterEnumTagValues]'), args.column);

								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=enumFilterMovementSummaryReset]').on('click', function (event) {
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=enumFilterMovementSummaryCancel]').on('click', function (event) {
									dataPopover.tip().find('.popover-content').find('[name=pointFilterEnumTagValues]').select2("destroy");
									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=enumFilterMovementSummaryApply]').on('click', function (event) {

									var setValue = $(dataPopover.tip().find('.popover-content').find('[name=pointFilterEnumTagValues]')).val();
									if (setValue === null) {
										//---
										dataPopover.tip().find('.popover-content').find('[name=pointFilterEnumTagValues]').parent().addClass('has-error');
										return false;
									}

									// create a description to show in a tooltip
									var description = "Value in: " + (setValue === "" ? "Empty" : setValue.join());

									var filter = {
										type: 'enum',
										Value: setValue,
										description: encodeURIComponent(description)
									};

									// copy the filter to all the columns of the same field
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
									event.stopPropagation();
								});
							}

							var defaultNumericInputMaskForFilter = function (unit, popupContainer, numberDecimalSeparator, numDecimals) {
								$(popupContainer).find('[name=numericFilterMin]').unmask();
								$(popupContainer).find('[name=numericFilterMax]').unmask();
								$(popupContainer).find('[name=numericFilterMin]').removeNumeric();
								$(popupContainer).find('[name=numericFilterMax]').removeNumeric();

								// set the mask for how to enter values
								if (unit === 27) //FML_FtIn16th
								{
									$(popupContainer).find('[name=numericFilterMin]').mask('S99-99-99', {
										translation: {
											'S': {
												pattern: /-/,
												optional: true
											}
										},
										placeholder: '__-__-__'
									});
									$(popupContainer).find('[name=numericFilterMax]').mask('S99-99-99', {
										translation: {
											'S': {
												pattern: /-/,
												optional: true
											}
										},
										placeholder: '__-__-__'
									});
								}
								else if (unit === 19) //FML_FtIn8th
								{
									$(popupContainer).find('[name=numericFilterMin]').mask('S99-99-9', {
										translation: {
											'S': {
												pattern: /-/,
												optional: true
											}
										},
										placeholder: '__-__-__'
									});
									$(popupContainer).find('[name=numericFilterMax]').mask('S99-99-9', {
										translation: {
											'S': {
												pattern: /-/,
												optional: true
											}
										},
										placeholder: '__-__-__'
									});
								}
								else {
									$(popupContainer).find('[name=numericFilterMin]').attr('placeholder', '');
									$(popupContainer).find('[name=numericFilterMax]').attr('placeholder', '');

									if (numDecimals === 0) {
										$(popupContainer).find('[name=numericFilterMin]').numeric({
											decimal: false,
											negative: true
										});
										$(popupContainer).find('[name=numericFilterMax]').numeric({
											decimal: false,
											negative: true
										});
									}
									else {
										$(popupContainer).find('[name=numericFilterMin]').numeric({
											decimal: FMOperateIndex.numformatInfo.NumberDecimalSeparator,
											negative: true,
											decimalPlaces: numDecimals
										});
										$(popupContainer).find('[name=numericFilterMax]').numeric({
											decimal: FMOperateIndex.numformatInfo.NumberDecimalSeparator,
											negative: true,
											decimalPlaces: numDecimals
										});
									}
								}
							}

							var loadUnitsByUnitType = function (container, column) {
								var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $("#MovementSummary" + FMMovementSummaryTab.newId) };

								// get the different engineering unit types that we have for the points selected in the grid
								var unitType;
								var unitTypes = FMOperateIndex.movementSummaryControllers[newId].getNumericUnitTypesForTag(column.field);
								if (unitTypes.length === 0) {
									unitType = column.filter.unitType;
								}
								else {
									unitType = unitTypes[0];
								}

								$.ajax({
									type: 'Get',
									url: 'GetUnitsByUnitType',
									dataType: "json",
									data: { "unitType": unitType },
									cache: false,
									success: function (response) {
										var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };
										FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
											if (inError) {
												//--- We need to display some type of message
												return;
											}
											$(container).html('');

											var columnUnit = -1;
											if (column.Unit) {
												columnUnit = column.Unit;
											}
											// sort the units alphabetically
											data = data.sort(function (a, b) {
												return a.UnitAbbreviation.localeCompare(b.UnitAbbreviation);
											});


											for (var i = 0; i < data.length; i++) {
												var uomtemplate = '';
												if ($(container).is('select')) {
													uomtemplate = '<option value="' + data[i].Unit + '">' + data[i].UnitAbbreviation + '</option>';
												}
												else {
													var uomEventHandler = "$(this).parent().find('.list-group-item').removeClass('active');$(this).addClass('active');$(this).closest( '.popover-content' ).find( '[name=changeUnitDefaultToPoint]' ).prop( 'checked', false );";
													uomtemplate = '<a href="#" class="list-group-item' + (columnUnit == data[i].Unit ? ' active' : '') + ' " data-value="' + data[i].Unit + '" onclick="' + uomEventHandler + '" title="' + data[i].UnitDescription + '">' + data[i].UnitAbbreviation + '</a>';
												}
												$(container).append(uomtemplate);
											}

											if (!$(container).is('select')) {
												if (columnUnit === -1) {
													$(container).closest('.popover-content').find('[name=changeUnitDefaultToPoint]').prop('checked', true);
												}
												else {
													$(container).closest('.popover-content').find('[name=changeUnitDefaultToPoint]').prop('checked', false);
												}

												$(container).niceScroll({ cursorwidth: '10px', autohidemode: true, cursorcolor: '#486899', background: 'transparent', horizrailenabled: false });

											}
											else {
												var numformatInfo = FMOperateIndex.numformatInfo;

												var numDecimals = 2;
												if (column.hasOwnProperty('DecimalPlaces')) {
													if (column['DecimalPlaces'] !== -1)
														numDecimals = column['DecimalPlaces'];
												}
												numformatInfo.NumberDecimalDigits = numDecimals;

												if (column.hasOwnProperty('filter')) {
													$(container).val(column.filter.unit);

													defaultNumericInputMaskForFilter(column.filter.unit, $(container).parent().find('.popover-content'), FMOperateIndex.numformatInfo.NumberDecimalSeparator, numDecimals);

													$(container).parent().find('[name=numericFilterMin]').val(FMFormatValues.FormatValue(column.filter.unit, numformatInfo, column.filter.minValue));
													if (column.filter.maxValue !== "") {
														$(container).parent().find('[name=numericFilterMax]').val(FMFormatValues.FormatValue(column.filter.unit, numformatInfo, column.filter.maxValue));
													}
												}
												else {
													if (columnUnit !== -1) {
														$(container).val(columnUnit);
													}

													// set the mask based on the default unit
													defaultNumericInputMaskForFilter(parseInt($(container).val()), $(container).parent(), FMOperateIndex.numformatInfo.NumberDecimalSeparator, numDecimals);

												}
											}

											$(container).parent().find('[name=numericFilterUnitType]').val(unitType);

										}, messageAttributes);
									},
									error: function (xhr, ajaxOptions, thrownError) {
										FMErrorAndExceptionHandling.ShowError(thrownError);
									}
								});
							}

							var loadOptionsForEnumValueType = function (container, column) {
								var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $("#MovementSummary" + FMMovementSummaryTab.newId) };

								// get the different value types that we have for the points selected in the grid
								var valueTypes = FMOperateIndex.movementSummaryControllers[newId].getValueTypesForTag(column.field);
								valueTypes = valueTypes[0];

								$.ajax({
									type: 'Get',
									url: 'GetOptionsForEnumValueType',
									dataType: "json",
									data: { "valueType": valueTypes },
									cache: false,
									success: function (response) {
										var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };
										FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
											if (inError) {
												//--- We need to display some type of message
												return;
											}
											$(container).html('');

											for (var i = 0; i < data.length; i++) {
												var optiontemplate = '<option value="' + data[i].Text + '">' + data[i].Text + '</option>';
												$(container).append(optiontemplate);
											}

											if (column.hasOwnProperty('filter')) {
												$(container).val(column.filter.Value);
											}
											$(container).select2();

										}, messageAttributes);
									},
									error: function (xhr, ajaxOptions, thrownError) {
										FMErrorAndExceptionHandling.ShowError(thrownError);
									}
								});
							}

							var validateTimeSpan = function (newValue) {

								// make sure that the value is valid
								if (newValue !== "") {
									var days = 0;
									var hours = 0;
									var minutes = 0;
									var seconds = 0;
									var dateSeparator = newValue.split(".");
									var timePart = "";
									if (dateSeparator.length === 1) {
										timePart = dateSeparator[0];
									}
									else {
										days = parseInt(dateSeparator[0]);
										timePart = dateSeparator[1];
									}

									var timeParts = timePart.split(":");

									if (timeParts.length > 0) {
										hours = parseInt(timeParts[0]);
									}
									if (timeParts.length > 1) {
										minutes = parseInt(timeParts[1]);
									}
									if (timeParts.length > 2) {
										seconds = parseInt(timeParts[2]);
									}
									if (hours > 24 || minutes > 60 || seconds > 60) {
										return null;
									}
									else if (hours >= 24 && minutes > 0 && seconds > 0) {
										return null;
									}
									return { days: days, hours: hours, minutes: minutes, seconds: seconds }
								}
								return null;
							}

							grid.registerPlugin(headerMenuPlugin);

							/*--------------- END COLUMN MENU  -----------------*/

							/*--------------- ROW MENU  -----------------*/
							var cellMenuPlugin = new Slick.Plugins.CellContextMenu({});

							cellMenuPlugin.onBeforeMenuShow.subscribe(function (e, args)
							{

								// Reset the context menu.
								FMMovementSummaryTab.ResetContextMenu(args);

								// Set context menu based on the blank row selection.
								FMMovementSummaryTab.SetContextMenuForTheBlankRow(args);


								// Set context menu based on the Node row selection.
								FMMovementSummaryTab.SetContextMenuForTheNodeRow(args);

								// Set context menu based on the Movement row selection.
								 FMMovementSummaryTab.SetContextMenuForTheMovementRow(args);

								 // disable based on rights
								 FMMovementSummaryTab.DisableContextMenuBasedOnRights(args);

								var dataView = args.grid.getData();
								var _rows = dataView.getItems();

								e.preventDefault();
							});

							cellMenuPlugin.onCommand.subscribe(function (e, args)
							{
								let grid = args.grid;
								var rowNumber = args.cellClicked.row;

								if (!args.column.totalizerConfig)
								{
									args.column.totalizerConfig = {};
								}

								if (args.command === 'insert-movement')
								{
									FMMovementSummaryTab.SelectRow(activeTab, newId, grid, rowNumber);
								}

								if (args.command === 'initiate-movement')
								{
									var movementPointGuidString = args.row.pointguid;
									FMMovementSummaryTab.InitiateMovement(movementPointGuidString);
								}

								if (args.command === 'set-movement-settings')
								{
									var movementPointGuidString = args.row.pointguid;
									FMMovementSummaryTab.SetMovementSettings(movementPointGuidString);
								}

								if (args.command === 'edit-movement-user-data')
								{
									var movementPointGuidString = args.row.pointguid;
									FMMovementSummaryTab.EditMovementUserData(movementPointGuidString);
								}

								if (args.command === 'edit-movement-start-data') {
									var movementPointGuidString = args.row.pointguid;
									FMMovementSummaryTab.EditMovementStartData(movementPointGuidString);
								}

								if (args.command === 'create-new-movement')
								{
									var movementPointGuidString = args.row.pointguid;
									FMMovementSummaryTab.CreateNewMovement(newId);
								}

								else if (args.command === 'delete-movement') {
									var dataView = grid.getData();
									var item = dataView.getItem(rowNumber);

									// only allow to delete movement rows
									if (!item.rowType || item.rowType !== "node") {

										FMLayout.ConfirmYesNo("Are you sure you want to delete the selected movement?", "Delete Movement?", function () {

											var movementPointGuidString = args.row.pointguid;
											FMMovementSummaryTab.DeleteMovement(movementPointGuidString);
										});
									}
								}


								if (args.command === 'stop-movement')
								{
									var movementPointGuidString = args.row.pointguid;
									FMMovementSummaryTab.StopMovement(movementPointGuidString);
								}

								if (args.command === 'initiate-movement-node')
								{
									var movementPointGuidString = args.row.movementguid;
									var movementNodePointGuidString = args.row.pointguid;
									FMMovementSummaryTab.InitiateMovementNode(movementPointGuidString, movementNodePointGuidString);
								}

								if (args.command === 'stop-movement-node')
								{
									var movementPointGuidString = args.row.movementguid;
									var movementNodePointGuidString = args.row.pointguid;
									FMMovementSummaryTab.StopMovementNode(movementPointGuidString, movementNodePointGuidString);
								}

								if (args.command === 'edit-movement-node-start-data') {
									var movementPointGuidString = args.row.movementguid;
									var movementNodePointGuidString = args.row.pointguid;
									FMMovementSummaryTab.EditMovementNodeStartData(movementPointGuidString, movementNodePointGuidString);
								}

								if (args.command === 'movement-disabled-by') {
									var movementPointId = args.row.point;
									var movementPointGuidString = args.row.movementguid;
									FMMovementSummaryTab.MovementDisabledBy(movementPointId, movementPointGuidString);
								}

								if (args.command === 'hold-for-hand-gauge')
								{
									var movementOrNodePointGuidStr = args.row.pointguid;
									// TODO:
								}

								if (args.command === 'edit-movement-handgauge-data')
								{
									var handgaugePointGuidStr = args.row.pointguid;
									FMMovementSummaryTab.EditMovementHandgauge(handgaugePointGuidStr);
								}

								else if (args.command === 'insert-emptyrow') {
									var newRowDefinition = { id: FMOperateIndex.newGuid(), type: "empty" };
									var dataview = grid.getData();
									var id = dataview.getItem(rowNumber).id;
									dataView.insertItem(dataview.getIdxById(id), newRowDefinition);
									grid.scrollRowIntoView(0, false);

									FMOperateIndex.PersistMovementSummary(activeTab, newId, _grid);
								}
								else if (args.command === 'remove-movement-row') {

									var dataView = grid.getData();
									var item = dataView.getItem(rowNumber);

									// only allow to removee movement rows
									if (!item.rowType || item.rowType !== "node") {

										FMLayout.ConfirmYesNo("Are you sure you want to remove the selected movement row?", "Remove Movement?", async function () {
											await FMMovementSummaryTab.RemoveMovementRowAsync(newId, grid, rowNumber);
											FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
										});
									}
								}


								else if (args.command === 'open-pointdetail') {
									var openPointDetailDataView = _grid.getData();
									var openPointDetailRow = openPointDetailDataView.getItem(rowNumber);
									FMOperateIndex.openPoint(openPointDetailRow.point, openPointDetailRow.pointguid);
								}
							});

							grid.registerPlugin(cellMenuPlugin);

							var insertPointPropertyColumn = function (args, columnID, columnHeader)
							{
								var newColumn = columnID;
								var newColumnName = columnHeader;
								var newid = newColumn.replace(/ /g, '') + new Date().getTime().toString(); // generate a unique id (in case there are multiple columns for the same tag)
								var fontSize = grid.getOptions().fontSize;
								var columnDefinition = { id: newid, name: newColumnName, field: newColumn, headerCssClass: "text-center grid-font-" + fontSize, cssClass: "grid-font-" + fontSize, formatter: movementSummaryFormatter };
								columnDefinition.header = {
									menu: { items: FMOperateIndex.MovementSummaryCreateHeaderMenu() }
								};

								// add the filter if already defined for a column for the same field
								$.each(grid.getColumns(), function (index, columnElem)
								{

									if (columnElem.field === newColumn) {
										columnDefinition.filter = columnElem.filter;
									}
								});

								var columns = grid.getColumns().slice(0);
								// insert the new column in the middle somewhere
								var pos = columns.map(function (e)
								{
									return e.id;
								}).indexOf(args.column.id);

								// if we are inserting in the Point Name put the column next to it, otherwise create it in the place the user clicked
								if (pos === 0)
								{
									pos++;
								}

								columns.splice(pos, 0, columnDefinition);

								grid.setColumns(columns);
								// update the filter parameters for the dataview
								FMOperateIndex.updateParms(grid, FMOperateIndex.movementSummaryControllers[newId].getMetadata());
								grid.getData().refresh();

								grid.resizeCanvas();
								// resize the newly created column to fit the column name (doing this by double clicking on the resize handle of the header)
								var newCreatedColumn = $(grid.getContainerNode()).find(".slick-header-column")[pos];

								FMOperateIndex.MovementSummarySaveOnColumnResize = false;
								$(newCreatedColumn).find('.slick-resizable-handle').dblclick();
								FMOperateIndex.MovementSummarySaveOnColumnResize = true;

								theShadowGrid.addColumn(columnDefinition);

								FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
							}


							var selectTagColumn = function (args)
							{
								// create the backdrop and wait for next modal to be triggered
								$('body').modalmanager('loading');

								$("#MovementSummarySelectionModalBody").html('<div id="MovementSummaryModalMenuLoader" class="LoadingAnimation transparent"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>');
								$("#MovementSummarySelectionModal").modal("show");

								// put messages on the actual tab
								var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $("#MovementSummary" + FMMovementSummaryTab.newId) };

								$.ajax({
									type: 'Get',
									url: 'GetListOfMovementTagNamesPartialView',
									dataType: "json",
									cache: false,
									success: function (response) {
										var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

										FMErrorAndExceptionHandling.HandleMessages(response, function (view, inError) {
											if (inError) {
												$("#MovementSummarySelectionModal").modal("hide");
												return;
											}
											$("#MovementSummarySelectionModalBody").html(view);
											$('#MovementSummarySelectionModalBody .operateSubMenuList').css("height", $('.operateSubMenuList').parent().height());

											// we need to remove the onclick event since by default it will open a new point when clicked and instead we need to add a 'selected' class
											$("#MovementSummarySelectionModalBody .operateSubMenuElement").each(function () {
												$(this).attr('onclick', "$(this).hasClass('selected') ? $(this).removeClass('selected'): $(this).addClass('selected')");
											});

											$("#MovementSummarySelectionModalBody .operateTagsSubMenuElement").each(function () {
												$(this).attr('ondblclick', "$(this).removeClass('selected').addClass('selected'); FMOperateIndex.MovementSummarySelectionModalSelectButton();");
											});

											$('#MovementSummarySelectionModalBody .operateSubMenuList').uncolumnize();
											$('#MovementSummarySelectionModalBody .operateSubMenuList').columnize({
												columns: 2,
												buildOnce: true,
												cssClassPrefix: "points",
												lastNeverTallest: true
											});

											$("#MovementSummarySelectionModalBody .operateSubMenuList").niceScroll({ cursorwidth: '10px', horizrailenabled: false, autohidemode: false, cursorcolor: "#486899", background: "white" });

											// override the code executed on the Select Button of the selection modal to deal with new tags
											FMOperateIndex.MovementSummarySelectionModalSelectButton = function () {
												if ($('.operateTagsSubMenuElement.selected').length === 0)
												{
													FMLayout.Alert("No Tag selected.");
												}
												else
												{
													var selectedRows = grid.getSelectedRows();
													$('.operateTagsSubMenuElement.selected').sort(function (a, b) { // sort in reverse since we are inserting in the same position
														return $(a).attr('data-name').toUpperCase().localeCompare($(b).attr('data-name').toUpperCase());
													}).each(function (index)
													{
														var newColumn = $(this).attr('data-name');
														var newField = $(this).attr('data-value');
														var newid = newColumn.replace(/ /g, '') + new Date().getTime().toString(); // generate a unique id (in case there are multiple columns for the same tag)
														var fontSize = grid.getOptions().fontSize;
														var sortable = false;
														if (FMMovementSummaryTab.IsMovementColumn(newField)) {
															sortable = true;
														}

														var columnDefinition = { id: newid, name: newColumn, field: newField, headerCssClass: "text-center grid-font-" + fontSize, cssClass: "grid-font-" + fontSize, formatter: movementSummaryFormatter, "sortable": sortable };
														columnDefinition.header = {
															menu: { items: FMOperateIndex.MovementSummaryCreateHeaderMenu() }
														};

														// add the filter if already defined for a column for the same field
														$.each(grid.getColumns(), function (index, columnElem) {
															if (columnElem.field === newColumn) {
																columnDefinition.filter = columnElem.filter;
															}
														});

														var columns = grid.getColumns().slice(0);
														// insert the new column in the middle somewhere
														var pos = columns.map(function (e) {
															return e.id;
														}).indexOf(args.column.id);

														// if we are inserting in the Point Name put the column next to it, otherwise create it in the place the user clicked
														if (pos === 0) {
															pos++;
														}

														columns.splice(pos, 0, columnDefinition);

														grid.setColumns(columns);

														// update the filter parameters for the dataview
														FMOperateIndex.updateFilterParameters(grid, FMOperateIndex.movementSummaryControllers[newId].getMetadata());
														grid.getData().refresh();

														grid.resizeCanvas();
														// resize the newly created column to fit the column name (doing this by double clicking on the resize handle of the header)
														var newCreatedColumn = $(grid.getContainerNode()).find(".slick-header-column")[pos];

														FMOperateIndex.MovementSummarySaveOnColumnResize = false;
														$(newCreatedColumn).find('.slick-resizable-handle').dblclick();
														FMOperateIndex.MovementSummarySaveOnColumnResize = true;

														theShadowGrid.addColumn(columnDefinition);
													});
													grid.setSelectedRows(selectedRows);
													$("#MovementSummarySelectionModal").modal("hide");
													FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
												}
											}
										}, messageAttributes);
									},
									error: function (xhr, ajaxOptions, thrownError) {
										FMErrorAndExceptionHandling.ShowError(thrownError);
										$("#MovementSummarySelectionModal").modal("hide");
									}
								});
							};

							var insertEmptyColumn = function (args)
							{
								var newColumn = "empty";
								var newColumnName = "";
								var newid = newColumn.replace(/ /g, '') + new Date().getTime().toString(); // generate a unique id (in case there are multiple columns for the same tag)
								var fontSize = grid.getOptions().fontSize;
								var columnDefinition = { id: newid, name: newColumnName, field: newColumn, headerCssClass: "text-center grid-font-" + fontSize, cssClass: "grid-font-" + fontSize, formatter: movementSummaryFormatter };
								columnDefinition.header = {
									menu: { items: FMOperateIndex.MovementSummaryCreateHeaderMenu() }
								};

								// add the filter if already defined for a column for the same field
								$.each(grid.getColumns(), function (index, columnElem) {

									if (columnElem.field === newColumn) {
										columnDefinition.filter = columnElem.filter;
									}
								});

								var columns = grid.getColumns().slice(0);
								// insert the new column in the middle somewhere
								var pos = columns.map(function (e) {
									return e.id;
								}).indexOf(args.column.id);

								// if we are inserting in the Point Name put the column next to it, otherwise create it in the place the user clicked
								if (pos === 0) {
									pos++;
								}

								columns.splice(pos, 0, columnDefinition);

								grid.setColumns(columns);

								grid.getData().refresh();

								grid.resizeCanvas();

								FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
							}

							/*--------------- END ROW MENU  -----------------*/

							/*--------------- AUTO SIZE COLUMNS -----------------*/
							var columnSizePlugin = new Slick.AutoColumnSize();
							grid.onColumnsResized.subscribe(function (e, data)
							{
								if (FMOperateIndex.MovementSummarySaveOnColumnResize)
								{
									FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
								}
							});

							grid.registerPlugin(columnSizePlugin);
							/*--------------- END AUTO SIZE COLUMNS  -----------------*/

						}

						// done with the process of restoring the tab
						FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
						// if we are not in the process of restoring the persisten state then open new groups so we can rename them
						if (FMOperateIndex.restoringView === false && isNewMovementSummary) {
							setTimeout(function () {
								FMOperateIndex.RenameTab($('a[data-target="#' + FMMovementSummaryTab.newId + '"]'));
							}, 1);
						}

						// force a resize of the grid when resizing the window
						$(window).resize(function () {
							grid.resizeCanvas();
						});

						// save the received initial data
						FMOperateIndex.movementSummaryControllers[newId].columnDefinitions = movementSummaryConfiguration.Columns;
						FMOperateIndex.movementSummaryControllers[newId].rowDefinitions = movementSummaryConfiguration.Rows; 
					}
				},
				messageAttributes);
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;

			FMOperateIndex.movementSummaryControllers[newId].rowVersionStr = response.Data.RowVersion;
			FMOperateIndex.movementSummaryControllers[newId].movementSummaryId = movementSummaryId;
			FMOperateIndex.movementSummaryControllers[newId].movementSummaryGuid = movementSummaryGuid;
			FMOperateIndex.movementSummaryControllers[newId].movementSummaryGuidStr = response.Data.MovementSummaryGuid;

			FMOperateIndex.movementSummaryControllers[newId].refreshTimer = setTimeout(() => { FMMovementSummaryTab.Refresh(newId); }, 5000);
		},
		error: function (xhr, textStatus, error) { FMOperateIndex.OpenMovementSummaryError(xhr, textStatus, error); }
	});
};

FMMovementSummaryTab.DisableSummaryRefreshTimer = function (newId, disable) {
	if (newId && FMOperateIndex.movementSummaryControllers[newId]) {

		//Abort any pending requests
		if (disable && FMOperateIndex.movementSummaryControllers[newId].getSummaryIfNewerAjaxRequest) {
			FMOperateIndex.movementSummaryControllers[newId].getSummaryIfNewerAjaxRequest.abort();
		}

		if (disable) {
			if (FMOperateIndex.movementSummaryControllers[newId].refreshTimer) {
				clearInterval(FMOperateIndex.movementSummaryControllers[newId].refreshTimer);
				FMOperateIndex.movementSummaryControllers[newId].refreshTimer = null;
			}
		} else {
			FMOperateIndex.movementSummaryControllers[newId].refreshTimer = setTimeout(() => { FMMovementSummaryTab.Refresh(newId); }, 5000);
		}
	}
};

FMMovementSummaryTab.Refresh = function (newId) {
	if (FMOperateIndex.movementSummaryControllers[newId].getRowsDeleted()) {
		let grid = FMOperateIndex.movementSummaryControllers[newId].getGrid();
		let activeTab = FMOperateIndex.movementSummaryControllers[newId].getActiveTab()
		FMOperateIndex.PersistMovementSummary(activeTab, newId, grid);
		FMOperateIndex.movementSummaryControllers[newId].clearRowsDeleted();
		FMOperateIndex.movementSummaryControllers[newId].refreshTimer = setTimeout(() => { FMMovementSummaryTab.Refresh(newId); }, 5000);
		return;
	}

	let movementSummaryGuidStr = FMOperateIndex.movementSummaryControllers[newId].movementSummaryGuidStr;
	let rowVersionStr = FMOperateIndex.movementSummaryControllers[newId].rowVersionStr;

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	var loadImage = $("#loadingimage");
	// put messages on the actual tab
	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#movementsummary' + newId) };
	FMOperateIndex.movementSummaryControllers[newId].getSummaryIfNewerAjaxRequest =
	$.ajax({
		type: 'get',
		dataType: 'json',
		cache: false,
		headers: headers,
		url: 'GetMovementSummaryIfNewer',
		data: { 'movementSummaryGuidStr': movementSummaryGuidStr, 'prevRowVersion': rowVersionStr },
		success: function (response) {
			if (response && response.Data) {
				var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };
				FMErrorAndExceptionHandling.HandleMessages(response,
					async function (movementSummaryConfiguration, inError) {
						// if it was not in error load and update the drawing
						if (!inError) {
							if (rowVersionStr !== movementSummaryConfiguration.RowVersion) {
								FMOperateIndex.movementSummaryControllers[newId].rowVersionStr = movementSummaryConfiguration.RowVersion;
								await FMMovementSummaryTab.ReinitializeMovementSummaryAsync(movementSummaryConfiguration, newId);
							}
						}
					}, messageAttributes);
			}
			FMOperateIndex.movementSummaryControllers[newId].refreshTimer = setTimeout(() => { FMMovementSummaryTab.Refresh(newId); }, 5000);
		},
		error: function (xhr, textStatus, error) {
			if (textStatus !== 'abort' && (PNotify.notices.length === 0
				|| PNotify.notices[PNotify.notices.length - 1].state === 'closed')) {
				PNotify.removeStack(FMMovementSummaryTab.messageAttributes.stack);
				FMErrorAndExceptionHandling.ShowError($('#CommunicationsFailureText').val(),
					function () {
					}, FMMovementSummaryTab.messageAttributes);
			}
			else {
			}
			FMOperateIndex.movementSummaryControllers[newId].refreshTimer = setTimeout(() => { FMMovementSummaryTab.Refresh(newId); }, 5000);
		}
	});
};

FMMovementSummaryTab.percentCompleteSort = function (a, b) {
	return a["percentComplete"] - b["percentComplete"];
};

FMMovementSummaryTab.groupBy = function (objectArray, property) {
	if (!objectArray) {
		return {};
	}
	objectArray = objectArray.reduce(function (acc, obj) {
		var key = obj[property];
		if (!acc[key]) {
			acc[key] = [];
		}
		acc[key].push(obj);
		return acc;
	}, {});
	return objectArray;
};

FMMovementSummaryTab.getDataMap = function (data, property) {
	var resultMap = {};
	if (data) {
		resultMap = data.reduce(function (map, obj) {
			map[obj.id] = obj[property];
			return map;
		}, {});
	}
	return resultMap;
};

FMMovementSummaryTab.addChilder = function (currentList, itemMap, targetList) {
	if (!currentList) {
		return [];
	}
	if (!targetList || null == targetList) {
		targetList = [];
	}

	currentList.forEach(function (element) {
		targetList.push(element);
		if (!itemMap || null == itemMap) {
			return;
		}

		var id = element["id"];
		if (!id || null == id) {
			return;
		}
		var list = itemMap[id];
		if (!list || list.length < 1) {
			return;
		}
		FMMovementSummaryTab.addChilder(list, itemMap, targetList);
	});
	return targetList;
};

FMMovementSummaryTab.initIndexMap = function (data) {
	if (!data) {
		return;
	}
	let resultMap = data.reduce(function (mappedArray, currentValue, index, array) {
		mappedArray[currentValue.id] = index;
		return mappedArray;
	}, {});
	return resultMap;
};

FMMovementSummaryTab.getIndent = function (idMap, itemId, max) {
	if (!idMap || idMap.length < 1) {
		return 0;
	}
	if (!itemId || null == itemId) {
		return 0;
	}
	let parentRowId = idMap[itemId];
	if (!parentRowId || null == parentRowId) {
		return 0;
	}
	max--;
	if (max < 0) {
		return 0;
	}
	return 1 + FMMovementSummaryTab.getIndent(idMap, parentRowId, max);
};

FMMovementSummaryTab.addIndentInfo = function (currentData, indentMap, indexMap) {
	if (!currentData || !indentMap || !indexMap) {
		return;
	}
	currentData.forEach(function (element) {
		let id = element["id"];
		if (!id || null == id) {
			return;
		}
		element["indent"] = FMMovementSummaryTab.getIndent(indentMap, id, 100);
		let parentRowId = element["parentRowId"];
		if (!parentRowId || null == parentRowId) {
			element["parent"] = null;
		} else {
			element["parent"] = indexMap[parentRowId];
		}
	});
};

FMMovementSummaryTab.getSubs = function (dRows, subs) {
	if (!subs) {
		subs = [];
	}
	if (!dRows || dRows.length < 1) {
		return subs;
	}
	dRows.forEach(function (element) {
		var childs = itemMap[element["id"]];
		if (childs && childs.length > 0) {
			subs = subs.concat(childs)
		}
		// HACK may get childs enough
		// subs = getSubs(childs, subs);
	});

	return subs;
};

FMMovementSummaryTab.initTree = function (gridData, itemMap) {
	let indentMap = FMMovementSummaryTab.getDataMap(gridData, 'parentRowId');
	data = FMMovementSummaryTab.addChilder(itemMap["null"], itemMap);
	let indexMap = FMMovementSummaryTab.initIndexMap(data);
	FMMovementSummaryTab.addIndentInfo(data, indentMap, indexMap);
	return data;
};

FMMovementSummaryTab.sortTree = function (dataMap, grid, args) {
	if (!dataMap || !args || !grid) {
		return;
	}

	var container = grid.getContainerNode();
	var tabContent = $(container).parent().parent().parent();
	var movementSummaryControllerId = $(tabContent).children('.active').attr("id");
	var movementSummaryGrid = FMOperateIndex.movementSummaryControllers[movementSummaryControllerId];
	var metadata = movementSummaryGrid.getMetadata();


	for (item in dataMap) {
		dataMap[item].sort(function (dataRow1, dataRow2) {
			return FMMovementSummaryTab.sortData(dataRow1, dataRow2, metadata, args);
		});
	};
};

FMMovementSummaryTab.sortData = function (dataRow1, dataRow2, metadata, args) {
	if (!dataRow1 || !dataRow2 || !metadata || !args) {
		return 0;
	}

	var field = args.sortCol.field;
	var sign = args.sortAsc ? 1 : -1;

	// All rows are filtered based upon Movement Data.
	var tagMetaData1 = FMMovementSummaryGrid.getTagInfo(metadata, dataRow1.movementguid, field);
	var tagMetaData2 = FMMovementSummaryGrid.getTagInfo(metadata, dataRow2.movementguid, field);

	if (tagMetaData1 == null
	|| tagMetaData2 == null) {
		return 0;
	}

	var value1 = tagMetaData1.Value;
	var value2 = tagMetaData2.Value;

	if (typeof value1 === "undefined") {
		if (typeof value2 === "undefined") {
			return 0;
		}
		else {
			return 1;
		}
	}
	else {
		if (typeof value2 === "undefined") {
			return -1;
		}
		else {
			var res = (value1 === value2 ? 0 : (value1 > value2 ? 1 : -1)) * sign;
			return res;
		}
	}
};


/**
 * dataRows1 and dataRows2 are Array
 **/
FMMovementSummaryTab.comparerTree = function (dataRows1, dataRows2, args) {
	var nRows1 = getSubs(dataRows1, []);
	var nRows2 = getSubs(dataRows2, []);

	if (nRows1.length < 1) {
		if (nRows2.length < 1) {
			return 0;
		} else {
			return 1;
		}
	} else {
		if (nRows2.length < 1) {
			return -1;
		} else {
			nRows1.sort(function (r1, r2) {
				return compareRow(r1, r2, args);
			});
			nRows2.sort(function (r1, r2) {
				return compareRow(r1, r2, args);
			});
			let rs = 0;
			if (args.sortAsc) {
				rs = FMMovementSummaryTab.compareRow(nRows1[nRows1.length - 1], nRows2[nRows2.length - 1], args);
			} else {
				rs = FMMovementSummaryTab.compareRow(nRows1[0], nRows2[0], args);
			}

			if (rs === 0) {
				rs = FMMovementSummaryTab.comparerTree(nRows1, nRows2, args)
			}
			return rs;
		}
	}
};

FMMovementSummaryTab.compareRow = function (dataRow1, dataRow2, args) {
	if (!dataRow1 || !dataRow2 || !args) {
		return 0;
	}
	let field = args.sortCol.field;
	let sign = args.sortAsc ? 1 : -1;

	let value1 = dataRow1[field], value2 = dataRow2[field];
	if (typeof value1 === "undefined") {
		if (typeof value2 === "undefined") {
			return 0;
		} else {
			return 1;
		}
	} else {
		if (typeof value2 === "undefined") {
			return -1;
		} else {
			var res = (value1 === value2 ? 0 : (value1 > value2 ? 1 : -1)) * sign;
			return res;
		}
	}
};


//============================================================
// This function resets the context menu to its initial state.
//============================================================
FMMovementSummaryTab.ResetContextMenu = function (args)
{
	args.menu.items[0].disabled = false; // Add Movement
	args.menu.items[1].disabled = false; // Remove Movement
	args.menu.items[2].disabled = false; // Initiate Movement
	args.menu.items[3].disabled = false; // Stop Movement
	args.menu.items[4].disabled = false; // Initiate Movement Node
	args.menu.items[5].disabled = false; // Stop Movement Node
	args.menu.items[6].disabled = false; // Hold for Hand Gauge
	args.menu.items[7].disabled = false; // Create new movement
	args.menu.items[8].disabled = false; // Delete movement
	args.menu.items[9].disabled = false; // Set movement settings
	args.menu.items[10].disabled = false; // Edit movement user data
	args.menu.items[11].disabled = false; // Edit movement start data
	args.menu.items[12].disabled = false; // Edit movement handgauge data
	args.menu.items[13].disabled = false; // Edit movement node start data
	args.menu.items[14].disabled = false; // Movement Disabled By

	args.menu.items[0].hidden = false;
	args.menu.items[1].hidden = false;
	args.menu.items[2].hidden = false;
	args.menu.items[3].hidden = false;
	args.menu.items[4].hidden = false;
	args.menu.items[5].hidden = false;
	args.menu.items[6].hidden = true;  // Want to hide the hold for Handgauge until future release
	args.menu.items[7].hidden = false;
	args.menu.items[8].hidden = false;
	args.menu.items[9].hidden = false;
	args.menu.items[10].hidden = false;
	args.menu.items[11].hidden = false;
	args.menu.items[12].hidden = true; // Want to hide the edit Handgauge until future release
	args.menu.items[13].hidden = false;
	args.menu.items[14].hidden = false;
};

//====================================================================
// This function will disable the context based on the security
// rights.
//====================================================================
FMMovementSummaryTab.DisableContextMenuBasedOnRights = function (args)
{
	if ($('#ModifyMovementSummaryRight').val() == 'False')
	{
		args.menu.items[0].disabled = true; // Add Movement
		args.menu.items[1].disabled = true; // Remove Movement
		args.menu.items[2].disabled = true; // Initiate Movement
		args.menu.items[3].disabled = true; // Stop Movement
		args.menu.items[6].disabled = true; // Hold for Hand Gauge
		args.menu.items[7].disabled = true; // Create new movement
		args.menu.items[8].disabled = true; // Delete movement
		args.menu.items[9].disabled = true; // Set movement settings
		args.menu.items[10].disabled = true; // Edit movement user data
		args.menu.items[11].disabled = true; // Edit movement start data
		args.menu.items[12].disabled = true; // Edit movement handgauge data
		args.menu.items[13].disabled = true; // Edit movement node start data
		args.menu.items[14].disabled = true; // Movement Disabled By
	}
};

//==================================================================
// This function sets the context menu on the blank row
// selection.
//==================================================================
FMMovementSummaryTab.SetContextMenuForTheBlankRow = function (args)
{
	if (args.row.type === "blank")
	{
		args.menu.items[0].hidden = false; // Add Movement
		args.menu.items[1].hidden = true; // Remove Movement
		args.menu.items[2].hidden = true; // Initiate Movement
		args.menu.items[3].hidden = true; // Stop Movement
		args.menu.items[4].hidden = true; // Initiate Movement Node
		args.menu.items[5].hidden = true; // Stop Movement Node
		args.menu.items[6].hidden = true; // Hold for Hand Gauge
		args.menu.items[7].hidden = false; // Create new movement
		args.menu.items[8].hidden = true; // Delete movement
		args.menu.items[9].hidden = true; // Set movement settings
		args.menu.items[10].hidden = true; // Edit movement user data
		args.menu.items[11].hidden = true; // Edit movement start data
		args.menu.items[12].hidden = true; // Edit movement handgauge data
		args.menu.items[13].hidden = true; // Edit movement node start data
		args.menu.items[14].hidden = true; // Movement Disabled By
	}
};


//=========================================================================
// This function set the context menu on the Node row selection.
//=========================================================================
FMMovementSummaryTab.SetContextMenuForTheNodeRow = function (args) 
{
	if (args.row.type !== "blank" && args.row.rowType && args.row.rowType.toUpperCase() === "NODE") 
	{
		args.menu.items[0].hidden = true; // Add Movement
		args.menu.items[1].hidden = true; // Remove Movement
		args.menu.items[2].hidden = true; // Initiate Movement
		args.menu.items[3].hidden = true; // Stop Movement
		//args.menu.items[6].hidden = true; // Hold for Hand Gauge
		args.menu.items[7].hidden = true; // Create New Movement
		args.menu.items[8].hidden = true; // Delete Movement
		args.menu.items[9].hidden = true; // Set Movement Setting
		args.menu.items[10].hidden = true; // Edit Movement User Data
		args.menu.items[11].hidden = true; // Edit Movement Start Data
		//args.menu.items[12].hidden = true; // Edit Movement handgauge Data
		args.menu.items[14].hidden = true; // Movement Disabled By

		// Disable the initiate and stop movement node context menu items
		// if the individual node control is disabled.
		if (args.row.individualNodeControl == false || $('#ModifyMovementSummaryRight').val() == 'False') 
		{
			args.menu.items[4].disabled = true; // Initiate Movement Node
			args.menu.items[5].disabled = true; // Stop Movement Node
		}
		else 
		{
			// The status column cannot be deleted, therefore we can hard code the name.
			var statusColValue = FMMovementSummaryTab.GetMovementStatus(args.grid, args.row.parentRowId);
			var transferStatusColValue = FMMovementSummaryTab.GetNodeTransferStatus(args.grid, args.row.id);

			if (transferStatusColValue && transferStatusColValue.toUpperCase() === $('#InactiveText').val().toUpperCase()
				|| statusColValue && statusColValue.toUpperCase() === $('#InactiveText').val().toUpperCase()) 
			{
				args.menu.items[4].disabled = false; // Initiate Movement Node
				args.menu.items[5].disabled = true; // Stop Movement Node
			}
			else 
			{
				args.menu.items[4].disabled = true; // Initiate Movement Node
				args.menu.items[5].disabled = false; // Stop Movement Node
			}
		}
	}
};

//=========================================================================
// This function set the context menu on the Handgauge row selection.
//=========================================================================
FMMovementSummaryTab.SetContextMenuForTheHandgaugeRow = function (args) 
{
	if (args.row.type !== "blank" && args.row.rowType && args.row.rowType.toUpperCase() === "HANDGAUGE") 
	{
		args.menu.items[0].hidden = false; // Add Movement
		args.menu.items[1].hidden = true; // Remove Movement
		args.menu.items[2].hidden = true; // Initiate Movement
		args.menu.items[3].hidden = true; // Stop Movement
		args.menu.items[4].hidden = true; // Initiate Movement Node
		args.menu.items[5].hidden = true; // Stop Movement Node
		//args.menu.items[6].hidden = true; // Hold for Hand Gauge
		args.menu.items[7].hidden = false; // Create new movement
		args.menu.items[8].hidden = true; // Delete movement
		args.menu.items[9].hidden = true; // Set movement settings
		args.menu.items[10].hidden = true; // Edit movement user data
		args.menu.items[11].hidden = true; // Edit movement start data

		//args.menu.items[12].disabled = true; // Edit Movement hand gauge Data

		var statusColValue = FMMovementSummaryTab.GetMovementStatus(args.grid, args.row.parentRowId);
		var transferStatusColValue = FMMovementSummaryTab.GetNodeTransferStatus(args.grid, args.row.id);

		if (transferStatusColValue && transferStatusColValue.toUpperCase() === $('#HoldForHangaugeDataText').val().toUpperCase()
			|| statusColValue && statusColValue.toUpperCase() === $('#HoldForHangaugeDataText').val().toUpperCase())
		{
			//args.menu.items[12].disabled = false; // Edit Movement hand gauge Data
		}
	}
};

//=========================================================================
// This function will return the movement status for a given movement.
//=========================================================================
FMMovementSummaryTab.GetMovementStatus = function (grid, movementRowId) {
	var movementStatus = "";
	var rowIndex = grid.getData().getRowById(movementRowId);
	var columnIndex = grid.getColumnIndex('Status');
	var cellNode = grid.getCellNode(rowIndex, columnIndex);
	return cellNode.textContent;
};

//=========================================================================
// This function will return the Transfer Status for a given node.
//=========================================================================
FMMovementSummaryTab.GetNodeTransferStatus = function (grid, nodeRowId) {
	var nodeTransferStatus = "";
	var rowIndex = grid.getData().getRowById(nodeRowId);
	var columnIndex = grid.getColumnIndex('TransferStatus');
	var cellNode = grid.getCellNode(rowIndex, columnIndex);
	return cellNode.textContent;
};

//=========================================================================
// This function returns true if field is supported by Movement Row  
//=========================================================================
FMMovementSummaryTab.IsMovementColumn = function (field) {

	if (field === 'PointId'
		|| field === 'Status'
		|| field === 'InitiationCount'
		|| field === 'TransferTimeRemaining'
		|| field === 'TransferStartTime'
		|| field === 'Deviation'
		|| field === 'PercentDeviation'
		|| field === 'TransferredGOV'
		|| field === 'TransferredNSV'
		|| field === 'Comment'
		|| field === 'OrderNumber'
		|| field === 'PlannedStartTime'
		|| field === 'CreatedBy'
		|| field === 'UserData01'
		|| field === 'UserData02'
		|| field === 'UserData03'
		|| field === 'UserData04'
		|| field === 'UserData05'
		|| field === 'UserData06'
		|| field === 'UserData07'
		|| field === 'UserData08'
		|| field === 'UserData09'
		|| field === 'UserData10'
	) {
		return true;	// Filter
	}
	else {
		return false;
	}
}

//=========================================================================
// This function set the context menu on the Movement row selection.
//=========================================================================
FMMovementSummaryTab.SetContextMenuForTheMovementRow = function (args)
{
	if (args.row.type !== "blank" && args.row.rowType && args.row.rowType.toUpperCase() === "MOVEMENT") 
	{
		// The Name and Status columns cannot be deleted, therefore we can hard code the column index.
		args.menu.items[0].hidden = true; // Add Movement
		args.menu.items[4].hidden = true; // Initiate Movement Node
		args.menu.items[5].hidden = true; // Stop Movement Node
		args.menu.items[7].hidden = true; // Create New Movement
		args.menu.items[8].hidden = false; // Delete Movement
		//args.menu.items[12].hidden = true; // Edit Movement handgauge Data
		args.menu.items[13].hidden = true; // Edit Movement Node Start Data

		args.menu.items[2].disabled = true; // Initiate Movement
		args.menu.items[3].disabled = true; // Stop Movement
		//args.menu.items[6].disabled = true;  // Hold for Hand Gauge
		args.menu.items[8].disabled = true;  // Delete Movement
		args.menu.items[9].disabled = false;  // Set Movement Settings
		args.menu.items[10].disabled = false; // Edit Movement User Data
		args.menu.items[11].disabled = true; // Edit Movement Start Data
		args.menu.items[14].disabled = true; // Movement Disabled By

		// The status column cannot be deleted, therefore we can hard code the name.
		var statusColValue = FMMovementSummaryTab.GetMovementStatus(args.grid, args.row.id);

		if (statusColValue) {

			if (statusColValue.toUpperCase() === $('#InactiveText').val().toUpperCase()){
				if (statusColValue.toUpperCase() !== $('#DisabledText').val().toUpperCase()) {
					args.menu.items[2].disabled = false; // Initiate Movement
				}
				args.menu.items[8].disabled = false;  // Delete Movement
				args.menu.items[9].disabled = false;  // Set Movement Settings
			}

			if (statusColValue.toUpperCase() === $('#ActiveText').val().toUpperCase()) {
				//args.menu.items[6].disabled = false; // Hold for Hand Gauge
			}

			if (statusColValue.toUpperCase() === $('#ActiveText').val().toUpperCase()
			|| statusColValue.toUpperCase() === $('#StartingText').val().toUpperCase()
			|| statusColValue.toUpperCase() === $('#StoppingText').val().toUpperCase()){
				args.menu.items[3].disabled = false; // Stop Movement
				args.menu.items[11].disabled = false; // Edit Movement Start Data
			}

			if (statusColValue.toUpperCase() === $('#DisabledText').val().toUpperCase()) {
				args.menu.items[14].disabled = false; // Movement Disabled By
			}
		}
	}
};

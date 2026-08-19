// defines the menu to be displayed on cells
if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}


FMOperateIndex.MovementSummaryCreateCellMenu = function () {
	return [
		{
			iconCssClass: "cell-menu-insert-row",
			title: "Add Movement",
			command: "insert-movement"
		},
		{
			iconCssClass: "cell-menu-remove-row",
			title: "Remove Movement",
			command: "remove-movement-row"
		},
		{
			iconCssClass: "cell-menu-initiate-movement",
			title: "Initiate Movement",
			command: "initiate-movement"
		},
		{
			iconCssClass: "cell-menu-stop-movement",
			title: "Stop Movement",
			command: "stop-movement"
		},
		{
			iconCssClass: "cell-menu-initiate-movement-node",
			title: "Initiate Node",
			command: "initiate-movement-node"
		},
		{
			iconCssClass: "cell-menu-stop-movement-node",
			title: "Stop Node",
			command: "stop-movement-node"
		},
		{
			iconCssClass: "cell-menu-hold-for-hand-gauge",
			title: "Hold for Hand Gauge",
			command: "hold-for-hand-gauge"
		},
		{
			iconCssClass: "cell-menu-create-new-movement-row",
			title: "Create New Movement",
			command: "create-new-movement"
		},
		{
			iconCssClass: "cell-menu-remove-row",
			title: "Delete Movement",
			command: "delete-movement"
		},
		{
			iconCssClass: "cell-menu-edit-movement-settings-row",
			title: "Set Movement Settings",
			command: "set-movement-settings"
		},
		{
			iconCssClass: "cell-menu-edit-movement-settings-row",
			title: "Edit User Data",
			command: "edit-movement-user-data"
		},
		{
			iconCssClass: "cell-menu-edit-movement-settings-row",
			title: "Edit Start Data",
			command: "edit-movement-start-data"
		},
		{
			iconCssClass: "cell-menu-edit-movement-settings-row",
			title: "Edit Handgauge Data",
			command: "edit-movement-handgauge-data"
		},
		{
			iconCssClass: "cell-menu-edit-movement-settings-row",
			title: "Edit Node Start Data",
			command: "edit-movement-node-start-data"
		},
		{
			iconCssClass: "cell-menu-movement-disabled-by",
			title: "Movement Disabled By",
			command: "movement-disabled-by"
		}
	];
};

// defines the menu to be displayed on headers
FMOperateIndex.MovementSummaryCreateHeaderMenu = function () {
	return [
		{
			iconCssClass: "header-menu-cell-alignment",
			title: "Cell Alignment",
			items: [
				{
					iconCssClass: "header-menu-cell-alignment-left",
					title: "Left",
					command: "left-align"
				},
				{
					iconCssClass: "header-menu-cell-alignment-center",
					title: "Center",
					command: "center-align"
				},
				{
					iconCssClass: "header-menu-cell-alignment-right",
					title: "Right",
					command: "right-align"
				}
			]

		},
		{
			iconCssClass: "header-menu-add-column",
			title: "Insert Column",
			items: [
				{
					iconCssClass: "header-menu-tag",
					title: "Tag",
					command: "insert-column-tag"
				},
				{
					iconCssClass: "header-menu-empty-column",
					title: "Empty Column",
					command: "insert-empty-column"
				}
			]
		},
		{
			iconCssClass: "header-menu-delete",
			title: "Delete Column",
			command: "delete-column"
		},
		{
			iconCssClass: "header-menu-rename",
			title: "Rename",
			command: "rename"
		},
		{
			iconCssClass: "header-menu-filter",
			title: "Filter",
			command: "filter"
		},
		{
			iconCssClass: "header-menu-set-display-precision",
			title: "Set Display Precision",
			command: "changeprecision"
		},
		{
			iconCssClass: "header-menu-set-display-unit",
			title: "Set Display Unit",
			command: "changeunit"
		},
		{
			iconCssClass: "header-menu-show-unit",
			title: "Show Units",
			command: "showunits"
		},
		{
			iconCssClass: "header-menu-show-quality",
			title: "Show Quality",
			command: "showquality"
		}
	];
};
//===========================================================================================
// This function will delete a movement summary.
//===========================================================================================
FMOperateIndex.DeleteMovementSummary = function (movementSummaryGuid, movementSummaryId)
{
    $("#menuPane").removeClass("hidden").addClass("hidden");
	$(".hamburger-menu").removeClass("active");

	FMLayout.ConfirmYesNo('Are you sure you want to close all open instances and delete the Movement Summary "' + movementSummaryId + '"?', "Delete Movement Summary", function ()
	{
		var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };

		$.ajax({
			type: 'Get',
			url: 'DeleteMovementSummary',
			dataType: "json",
			data: { "id": movementSummaryGuid },
			cache: false,
			success: function (response)
			{
				var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator, width: '450px' };
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError)
				{
					if (inError)
					{
						//--- We need to display some type of message
						return;
					}

					var tabsToDelete = [];

					// find all the instances of the movement summary that are open and close them
					$.each(FMOperateIndex.contents, function (index, tab)
					{
						// if dealing with tab groups go through each page
						if (tab.type === "group")
						{
							$.each(tab.settings, function (index, subtab)
							{
								if (subtab.settings.movementSummaryGuid === movementSummaryGuid)
								{
									tabsToDelete.push(subtab.id);
								}
							});
						}
						else if (tab.type === "movementSummary")
						{
							if (tab.settings.movementSummaryGuid === movementSummaryGuid)
							{
								tabsToDelete.push(tab.id);
							}
						}
					});

					// delete the tabs
					$.each(tabsToDelete, function (index, tabname)
					{
						// get the li control that is the tab
						FMOperateIndex.RemoveTab($("li a[data-target='#" + tabname + "']").parent());
					});

					// refresh the hamburger the menu (list of movement summary)
					if ($(".operateMenuItem.active a").attr("id") === "menuMovementSummary")
					{
						FMOperateIndex.refreshHamburgerMenu = true;
					}
				});
			},
			error: function (xhr, ajaxOptions, thrownError)
			{
				FMErrorAndExceptionHandling.ShowError(thrownError);
			}
		});
	});
};
FMOperateIndex.DeleteMultipleMovementSummaries = function () {//movementSummaryGuid, movementSummaryId) {
	$("#menuPane").removeClass("hidden").addClass("hidden");
	$(".hamburger-menu").removeClass("active");

	FMLayout.ConfirmYesNo('Are you sure you want to close all open instances and delete all the listed Movement Summaries?', "Delete Movement Summary", function () {
		var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };
	//	debugger;
		var movementSummaryGuidList = $(".operateMovementSummarySubMenuElement").not(".hidden").map(function () { return $(this).data("guid"); }).get();
		$.ajax({
			type: 'Post',
			url: 'DeleteMultipleMovementSummary',
			dataType: 'json',
			contentType: "application/json",
			data: JSON.stringify({ guidList: movementSummaryGuidList }),
			cache: false,
			success: function (response) {
				var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator, width: '450px' };
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
					if (inError) {
						//--- We need to display some type of message
						return;
					}

					var tabsToDelete = [];

					// find all the instances of the movement summary that are open and close them
					$.each(FMOperateIndex.contents, function (index, tab) {
						// if dealing with tab groups go through each page
						for (var i = 0; i < movementSummaryGuidList.length; i++) {
                            let movementSummaryGuid = movementSummaryGuidList[i];
							if (tab.type === "group") {
								$.each(tab.settings, function (index, subtab) {
									if (subtab.settings.movementSummaryGuid === movementSummaryGuid) {
										tabsToDelete.push(subtab.id);
									}
								});
							}
							else if (tab.type === "movementSummary") {
								if (tab.settings.movementSummaryGuid === movementSummaryGuid) {
									tabsToDelete.push(tab.id);
								}
							}
						}
					});

					// delete the tabs
					$.each(tabsToDelete, function (index, tabname) {
						// get the li control that is the tab
						FMOperateIndex.RemoveTab($("li a[data-target='#" + tabname + "']").parent());
					});

					// refresh the hamburger the menu (list of movement summary)
					if ($(".operateMenuItem.active a").attr("id") === "menuMovementSummary") {
						FMOperateIndex.refreshHamburgerMenu = true;
					}
				});
			},
			error: function (xhr, ajaxOptions, thrownError) {
				FMErrorAndExceptionHandling.ShowError(thrownError);
			}
		});
	});
};
//============================================================================
// This function will handle the Add Movement Summary event.  It will display
// the popup dialog that will allow the user to enter in a new movement
// summary.
//============================================================================
FMOperateIndex.AddMovementSummary = function ()
{
    $("#movementSummarySaveScreen #movementSummaryNewName").val('');
	$("#movementSummarySaveScreen #movementSummaryNewDesc").val('');

	if ($("#movementSummarySaveScreen input[name=MovementSummaryPrivateSaveAs][value='0']").attr('disabled') === "disabled")
	{
		// default movement summary to private if cannot create public
		$("#movementSummarySaveScreen input[name=MovementSummaryPrivateSaveAs][value='1']").prop("checked", true);
	}
	else
	{
		// default movement summary to public
		$("#movementSummarySaveScreen input[name=MovementSummaryPrivateSaveAs][value='0']").prop("checked", true);
	}

	$('body').modalmanager('loading');
	$("#movementSummarySaveScreen").modal("show");
	$("#movementSummarySaveScreen #movementSummarySaveSaveButton").off('click');

	$("#movementSummarySaveScreen #movementSummarySaveSaveButton").on('click', function () {
		// Try to save the movement summary
		var movementSummaryVisibilityType = $("#movementSummarySaveScreen input[type=radio][name=MovementSummaryPrivateSaveAs]:checked").val();
		var movementSummaryName = $("#movementSummarySaveScreen #movementSummaryNewName").val();
		var movementSummaryDescription = $("#movementSummarySaveScreen #movementSummaryNewDesc").val();

		// make sure we have a name
		if (movementSummaryName === "")
		{
			$("#movementSummarySaveScreen #movementSummaryNewName").parent().addClass('has-error');
			return false;
		}

		var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };
		var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator };

		// remove previous notifications
		PNotify.removeStack(stack_bottomright_operator);
		$('<div id="loadermovementsummarymain" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('body');

		$.ajax({
			url: 'AddMovementSummary',
			type: 'Post',
			dataType: 'json',
			contentType: "application/json",
			data: JSON.stringify({ "id": movementSummaryName, "description": movementSummaryDescription, "movementSummaryType": movementSummaryVisibilityType }),
			success: function (response)
			{
				$("#loadermovementsummarymain").remove();
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError)
				{
					if (inError == false)
					{
						// if there is a duplicate show an error and go to the first page
						if (data.duplicateFound)
						{
							FMLayout.Alert("There is already a Movement Summary with the same Name and Visibility.", "Duplicate", null);

						}
						else
						{
							$("#movementSummarySaveScreen").modal("hide");
							FMOperateIndex.OpenMovementSummary(movementSummaryName, data.movementSummaryGuid);
						}

						// refresh the hamburger the menu (list of movement summary)
						if ($(".operateMenuItem.active a").attr("id") === "menuMovementSummary")
						{
							FMOperateIndex.refreshHamburgerMenu = true;
						}
					}
				}, messageAttributes);
			},
			error: function (request, status, error)
			{
				$("#loadermovementsummarymain").remove();
				FMErrorAndExceptionHandling.ShowException(request, status, error, function () {}, messageAttributes);
			}
		});
	});
};

//===========================================================================
// This function closes the movement summary.
//===========================================================================
FMOperateIndex.CloseMovementSummary = function (id)
{
	if (FMOperateIndex.movementSummaryControllers.hasOwnProperty(id))
	{
		var fmMovementSummaryGrid = FMOperateIndex.movementSummaryControllers[id];

		FMOperateIndex.unsubscribeTagWebWorker(fmMovementSummaryGrid._uniqueId);

		if (FMOperateIndex.movementSummaryControllers[id].refreshTimer != null) {
			clearTimeout(FMOperateIndex.movementSummaryControllers[id].refreshTimer);
		}

		delete FMOperateIndex.movementSummaryControllers[id];
	}
};

//============================================================================
// This function will save the movement summary as another name.
//============================================================================
FMOperateIndex.SaveMovementSummaryAs = function (parentId, controlId)
{
	var ID = "";
	if (parentId === 'mainTab')
	{
		FMOperateIndex.contents = $.map(FMOperateIndex.contents, function (obj)
		{
			if (obj.id === controlId)
			{
				ID = obj.name;
			}

			return obj;
		});
	}
	else {
		var parentGroupIdx = $.map(FMOperateIndex.contents, function (obj, index)
		{
			if (obj.id === parentId)
			{
				return 1;
			}

			return 0;
		});

		var foundTabGroupIdx = parentGroupIdx.indexOf(1);

		if (foundTabGroupIdx >= 0)
		{
			$.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj)
			{
				if (obj.id === controlId)
				{
					ID = obj.name;
				}

				return obj;
			});
		}
	}

	$("#movementSummarySaveScreen #movementSummaryNewName").val(ID);
	$("#movementSummarySaveScreen #movementSummaryNewDesc").val('');

	// Default movement summary to private
	$("#movementSummarySaveScreen input[name=MovementSummaryPrivateSaveAs][value='1']").prop("checked", true); 

	$('body').modalmanager('loading');
	$("#movementSummarySaveScreen").modal("show");
	$("#movementSummarySaveScreen #movementSummarySaveSaveButton").off('click');

	$("#movementSummarySaveScreen #movementSummarySaveSaveButton").on('click', function ()
	{
		// Try to save the movement summary
		var movementSummaryVisibilityType = $("#movementSummarySaveScreen input[type=radio][name=MovementSummaryPrivateSaveAs]:checked").val();
		var movementSummaryName = $("#movementSummarySaveScreen #movementSummaryNewName").val();
		var movementSummaryDescription = $("#movementSummarySaveScreen #movementSummaryNewDesc").val();

		// Make sure we have a name
		if (movementSummaryName === "")
		{
			$("#movementSummarySaveScreen #movementSummaryNewName").parent().addClass('has-error');
			return false;
		}
		var sourceGrid = FMOperateIndex.movementSummaryControllers[controlId]._grid;
		FMOperateIndex.CloneMovementSummary(movementSummaryName, movementSummaryDescription, movementSummaryVisibilityType, sourceGrid, null);
	});
};

//===================================================================================================
// This function will clone a movement summary.
//===================================================================================================
FMOperateIndex.CloneMovementSummary = function (ID, Description, movementSummaryType, grid, movementSummaryGuid)
{
	if (!movementSummaryGuid)
	{
		movementSummaryGuid = '';
	}

	// We can reuse the same call to save the Drawing settings
	// Get the items in the dataview used by the grid
	 var rows = grid.getData().getItems();
	 
	 var jsRowVersion = "";
	
	// There is no need to save the header menu since it's being rebuilt
	var columns = grid.getColumns();

	// Copy array by value so we don't lose the original menu
	var columnsWithNoMenu = $.extend(true, [], columns);
	
	// We don't need to save the header menu since it's being rebuilt on load or the total/subtotals
	columnsWithNoMenu = $.map(columnsWithNoMenu, function (val, i)
	{
		val.header = null;
		val.name = encodeURIComponent(val.name); // We have to encode strings so we can support escape characters ( '\', '|' )
		return val;
	});

	var fontSize = grid.getOptions().fontSize;

	var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };
	var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator };

	// Remove previous notifications
	PNotify.removeStack(stack_bottomright_operator);
	$('<div id="loadermovementsummarymain" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('body');

	$.ajax({
		url: 'SaveMovementSummary',
		type: 'Post',
		dataType: 'json',
		 contentType: "application/json",
		 data: '{movementSummaryGuid:\'' + movementSummaryGuid + '\', id: ' + JSON.stringify(ID) + ', description: ' + JSON.stringify(Description) + ', rows: \'' + JSON.stringify(rows) + '\', columns: \'' + JSON.stringify(columnsWithNoMenu) + '\', fontSize: \'' + fontSize + '\', jsRowVersion: \'' + jsRowVersion + '\', movementSummaryType: \'' + parseInt(movementSummaryType) + '\' }',
		success: function (response)
		{
			$("#loadermovementsummarymain").remove();

			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError)
			{
				if (inError == false)
				{
					// We get back the movement Summary Guid during the save, we need to check is a new movement summary Id and not a duplicate
					var newMovementSummaryGuid = data.movementSummaryGuid;

					// If we found an existing movement summary with the same ID, visibility, site and owner
					if (data.duplicateFound)
					{
						// check if we want to overwrite the existing movement summary
						FMLayout.ConfirmYesNo("There is already a Movement Summary with the same Name and Visibility.  Do you want to overwrite the existing Movement Summary?",
							"Duplicate Movement Summary",
							function ()
							{
								FMOperateIndex.CloneMovementSummary(ID, Description, movementSummaryType, grid, newMovementSummaryGuid);
							});
					}
					else
					{
						$("#movementSummarySaveScreen").modal("hide");
					}
					// refresh the hamburger the menu (list of movement summaries)
					if ($(".operateMenuItem.active a").attr("id") === "menuMovementSummary")
					{
						FMOperateIndex.refreshHamburgerMenu = true;
					}
				}
			}, messageAttributes);
		},
		error: function (request, status, error)
		{
			$("#loadermovementsummarymain").remove();
			FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
			}, messageAttributes);
		}
	});
};

//===========================================================================
// Update Movement Summary settings 
//===========================================================================
FMOperateIndex.UpdateMovementSummarySettings = function (parentControl, movementSummaryControllerId, parentGroupTab)
{
	var popover = $(parentControl).popover("destroy").popover({
		container: 'body',
		placement: 'bottom',
		html: true,
		content: $('#MovementSummaryConfigurationSettings').html(),
		trigger: "manual"
	});

	var dataPopover = popover.data('bs.popover');
	$(parentControl).popover('show');
	$("#customModalBackground").removeClass("hidden");

	dataPopover.tip().find('.popover-content').find('[name=movementSummarySettingsDescription]').val(FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].description);
	dataPopover.tip().find('.popover-content').find('[name=movementSummaryOwnerName]').text(FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].owner);
	dataPopover.tip().find('.popover-content').find('[name=movementSummary-fontsize]').val(FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].fontSize);
	dataPopover.tip().find('.popover-content').find("input[name=MovementSummaryVisibilitySetting][value='" + FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].visibility + "']").prop("checked", true);

	// if movement summary is shared and and we don't own it we can't change the settings
	if (!FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].isOwnByMe)
	{
		dataPopover.tip().find('.popover-content').find('[name=MovementSummaryVisibilitySetting]').prop("disabled", "disabled");
	}

	if (!FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].isEditable)
	{
		dataPopover.tip().find('.popover-content').find('[name=movementSummarySettingsDescription]').attr("disabled", "disabled");
		dataPopover.tip().find('.popover-content').find('[name=movementSummary-fontsize]').attr("disabled", "disabled");
	}

	// when losing focus on the description field save the changes 
	dataPopover.tip().find('.popover-content').find('[name=movementSummarySettingsDescription]').on('blur', function () {
		FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].description = $(this).val();
		FMOperateIndex.PersistMovementSummary(parentGroupTab, movementSummaryControllerId, FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].getGrid());
		FMOperateIndex.refreshHamburgerMenu = true;
	});

	// when changing the private flag force a save
	dataPopover.tip().find('.popover-content').find("input[name=MovementSummaryVisibilitySetting]").on("change", function () {
		FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].visibility = $(this).val();
		FMOperateIndex.PersistMovementSummary(parentGroupTab, movementSummaryControllerId, FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].getGrid());
		FMOperateIndex.refreshHamburgerMenu = true;
	});

	// event to switching the font size
	dataPopover.tip().find('.popover-content').find("[name=movementSummary-fontsize]").on('change', function () {
		var grid = FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].getGrid();
		var columns = grid.getColumns();
		for (var i = 0; i < columns.length; i++) {
			var headerCss = columns[i].headerCssClass ? columns[i].headerCssClass : '';
			headerCss = headerCss.replace('grid-font-8', '')
				.replace('grid-font-9', '')
				.replace('grid-font-10', '')
				.replace('grid-font-11', '')
				.replace('grid-font-12', '')
				.replace('grid-font-13', '')
				.replace('grid-font-14', '')
				.replace('grid-font-15', '')
				.replace('grid-font-16', '')
				.replace('grid-font-17', '')
				.replace('grid-font-18', '')
				.replace('grid-font-19', '')
				.replace('grid-font-20', '');
			headerCss += ' grid-font-' + $(this).val();
			columns[i].headerCssClass = headerCss;

			var columnCss = columns[i].cssClass ? columns[i].cssClass : '';
			columnCss = columnCss.replace('grid-font-8', '')
				.replace('grid-font-9', '')
				.replace('grid-font-10', '')
				.replace('grid-font-11', '')
				.replace('grid-font-12', '')
				.replace('grid-font-13', '')
				.replace('grid-font-14', '')
				.replace('grid-font-15', '')
				.replace('grid-font-16', '')
				.replace('grid-font-17', '')
				.replace('grid-font-18', '')
				.replace('grid-font-19', '')
				.replace('grid-font-20', '');
			columnCss += ' grid-font-' + $(this).val();
			columns[i].cssClass = columnCss;
		}

		grid.setOptions({
			fontSize: parseInt($(this).val())
		});
		grid.setColumns(columns);
		grid.invalidateAllRows();
		grid.render();
		grid.resizeCanvas();
		FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].fontSize = $(this).val();
		FMOperateIndex.PersistMovementSummary(parentGroupTab, movementSummaryControllerId, FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].getGrid());
	});

	// click on Save As button
	dataPopover.tip().find('.popover-content').find('[name=configurationMovementSummarySaveAs]').on('click', function (event) {
		// remove events
		dataPopover.tip().find('.popover-content').find('[name=movementSummarySettingsDescription]').off('blur');
		dataPopover.tip().find('.popover-content').find("input[name=MovementSummaryVisibilitySetting]").off("change");
		dataPopover.tip().find('.popover-content').find("[name=movementSummary-fontsize]").off('change');
		dataPopover.tip().find('.popover-content').find("input[name=MovementSummaryVisibilitySetting]").off("change");
		dataPopover.tip().find('.popover-content').find('[name=configurationMovementSummarySaveAs]').off('click');
		dataPopover.tip().find('.popover-content').find('[name=configurationMovementSummaryCancel]').off('click');

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();

		FMOperateIndex.SaveMovementSummaryAs(parentGroupTab, movementSummaryControllerId);
	});

	// click on Save As button
	dataPopover.tip().find('.popover-content').find('[name=configurationMovementSummaryPrint]').on('click', function (event) {

		var grid = FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].getGrid();

		var printPlugin = new Slick.Plugins.Print();
		grid.registerPlugin(printPlugin);

		$("#movementsummaryprint").html('');
		printPlugin.printToElement('#movementsummaryprint');
		grid.unregisterPlugin(printPlugin);

		var tabName = $("a[data-target='#" + FMOperateIndex.movementSummaryControllers[movementSummaryControllerId]._id + "'] .tab-name").text();

		$("#movementsummaryprint").printThis({
			debug: false,
			importCSS: true,
			importStyle: false,
			copyTagClasses: true,
			removeInline: true,
			afterPrint: function () { $("#movementsummaryprint").html(''); },
			header: "<h4 class='text-center'>" + tabName + "</h4>"
		})

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});

	// click on Auto Print (from external service)
	dataPopover.tip().find('.popover-content').find('[name=configurationMovementSummaryAutoPrintHidden]').on('click', function (event) {

		var grid = FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].getGrid();

		var printPlugin = new Slick.Plugins.Print();
		grid.registerPlugin(printPlugin);

		$("#movementsummaryprint").html('');
		printPlugin.printToElement('#movementsummaryprint');
		grid.unregisterPlugin(printPlugin);

		var tabName = $("a[data-target='#" + FMOperateIndex.movementSummaryControllers[movementSummaryControllerId]._id + "'] .tab-name").text();

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});


	// click on Save As button
	dataPopover.tip().find('.popover-content').find('[name=configurationMovementSummaryAutoSchedule]').on('click', function (event) {
		event.stopPropagation();

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");

		// get movementSummaryGuid
		var movementSummaryGuid = "";
		var controlId = FMOperateIndex.movementSummaryControllers[movementSummaryControllerId]._id;

		if (parentGroupTab === 'mainTab') {
			FMOperateIndex.contents = $.map(FMOperateIndex.contents, function (obj) {
				if (obj.id === controlId) {
					ID = obj.name;
					movementSummaryGuid = obj.settings.movementSummaryGuid;
				}
				return obj;
			});
		}
		else {
			var parentGroupIdx = $.map(FMOperateIndex.contents, function (obj, index) {
				if (obj.id === parentGroupTab) {
					return 1;
				}
				return 0;
			});

			var foundTabGroupIdx = parentGroupIdx.indexOf(1);
			if (foundTabGroupIdx >= 0) {
				$.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj) {
					if (obj.id === controlId) {
						ID = obj.name;
						movementSummaryGuid = obj.settings.movementSummaryGuid;
					}
					return obj;
				});
			}
		}


		$('body').modalmanager('loading');

		$("#MovementSummaryReportScheduleRepeatNever").click();

		var tabName = $("a[data-target='#" + FMOperateIndex.movementSummaryControllers[movementSummaryControllerId]._id + "'] .tab-name").text();

		$("#MovementSummaryReportScheduleName").val(tabName);
		$("#MovementSummaryReportScheduleName").attr("data-guid", movementSummaryGuid);

		FMOperateIndex.reportScheduleOpen(tabName, movementSummaryGuid);
	});

	// close the pop over when clicking cancel
	dataPopover.tip().find('.popover-content').find('[name=configurationMovementSummaryCancel]').on('click', function (event) {
		// remove events
		dataPopover.tip().find('.popover-content').find('[name=movementSummarySettingsDescription]').off('blur');
		dataPopover.tip().find('.popover-content').find("input[name=MovementSummaryVisibilitySetting]").off("change");
		dataPopover.tip().find('.popover-content').find("[name=movementSummary-fontsize]").off('change');
		dataPopover.tip().find('.popover-content').find("input[name=MovementSummaryVisibilitySetting]").off("change");
		dataPopover.tip().find('.popover-content').find('[name=configurationMovementSummarySaveAs]').off('click');
		dataPopover.tip().find('.popover-content').find('[name=configurationMovementSummaryCancel]').off('click');

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});
};

/////////////////////////////////////////////////////
// Open the selected MovementSummary on a Tab
/////////////////////////////////////////////////////
FMOperateIndex.OpenMovementSummary = function (movementSummaryId, movementSummaryGuid)
{
	// can't open movementSummary if there are no groups
	if (FMOperateIndex.isTabGroupEnabled && $('#tabList').children().length === 1)
	{
		FMLayout.Alert('Cannot open a MovementSummary without a Group.', 'Error');
		return null;
	}

	var activeTab = 'mainTab';
	if (FMOperateIndex.isTabGroupEnabled)
	{
		activeTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
	}

	//check to see if we have one open already (only when clicking on the menu, not when restoring the screen)
	if (FMOperateIndex.restoringView === false)
	{
		var parentTab = "";
		var movementSummaryTab = "";

		var parentGroup = FMOperateIndex.contents;
		FMOperateIndex.UpdateOperateHelpKey('movementSummaryGroup');

		if (activeTab !== 'mainTab')
		{
			parentTab = activeTab;

			var parentGroupFiltered = $.grep(parentGroup, function (e)
			{
				return e.id === activeTab;
			});

			if (parentGroupFiltered.length > 0)
			{
				parentGroup = parentGroupFiltered[0];
			}
		}

		$.each(parentGroup.settings, function (index, level1Tab)
		{
			if (level1Tab.type === "movementSummary" && level1Tab.settings.movementSummaryGuid === movementSummaryGuid)
			{
				movementSummaryTab = level1Tab.id;
			}
		});

		if (parentTab !== "")
		{
			$('a[data-target=\'#' + parentTab + '\'').click();
		}

		if (movementSummaryTab !== "")
		{
			$('a[data-target=\'#' + movementSummaryTab + '\'').click();
			return false;
        }
	}

	if (!movementSummaryGuid)
		movementSummaryGuid = '00000000-0000-0000-0000-000000000000';

	var isNewMovementSummary = (movementSummaryGuid === '00000000-0000-0000-0000-000000000000');

	if (!movementSummaryId)
	{
		movementSummaryId = 'Movement Summary ' + Math.floor(Math.random() * 100000);
	}

	var newId = FMOperateIndex.AddTab(movementSummaryId);

	if (newId === null) return false;

	FMOperateIndex.PersistNewControl(activeTab, newId, movementSummaryId, 'movementSummary', { movementSummaryGuid: movementSummaryGuid });

	// start the process of restoring the tab
	FMOperateIndex.restoringScreenQueueInProgress[newId] = true;

	$("#" + newId).getNiceScroll().remove();

	var dynamicStyleSheetAlarms = $("<style type='text/css' rel='stylesheet' />").appendTo($("head"));
	var rules = [];

	$('<div id="movementsummary' + newId + '" class="movement-summary movementsummarytemp" style="position: absolute; top: 5px; left: 5px;right: 5px;bottom: 5px;"></div>').appendTo('#' + newId);
	$('<div id="movementsummary' + newId + 'container" class="movement-summary-panel active" style="overflow:hidden"></div>').appendTo('#movementsummary' + newId);

	$('<div id="loadermovementsummary' + newId + '" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#' + newId);

	// put messages on the actual tab
	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#movementsummary' + newId) };

	// Create the movement summary tab grid.
	FMMovementSummaryTab.CreateMovementSummaryTabGrid(movementSummaryGuid, movementSummaryId, activeTab, newId, isNewMovementSummary, stack_bottomright_operatortab);
	return newId;
};

FMOperateIndex.OpenMovementSummarySuccess = function (response, _activeTab, _newId) { };

//=======================================================================================
// This function will presist the movement summary.
//=======================================================================================
FMOperateIndex.PersistMovementSummary = function (parentId, newId, grid) {
	var restoring = window.localStorage.getItem('operateBeingRestored') || "false";
	var movementSummaryTagRefresh = window.localStorage.getItem('movementSummaryTagRefresh') || "false";

	if (FMOperateIndex.movementSummaryControllers[newId].isEditable == false || restoring === "true" || movementSummaryTagRefresh === "true") {
		return;
	}
	 
	// We can reuse the same call to save the Drawing settings
	var dataview = grid.getData();
	 var rows = dataview.getItems();
	 var jsRowVersion = FMOperateIndex.movementSummaryControllers[newId].rowVersionStr;


	// There is no need to save the header menu since it's being rebuilt
	var columns = grid.getColumns();
	var columnsWithNoMenu = $.extend(true, [], columns);  // copy array by value so we don't lose the original menu

	// We don't need to save the header menu since it's being rebuilt on load or the total/subtotals
	columnsWithNoMenu = $.map(columnsWithNoMenu, function (val, i) {
		val.header = null;
		val.name = encodeURIComponent(val.name); // We have to encode strings so we can support escape characters ( '\', '|' )
		return val;
	});


	var fontSize = grid.getOptions().fontSize;

	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#movementsummary' + newId) };
	var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab };

	// Remove previous notifications
	PNotify.removeStack(stack_bottomright_operatortab);

	// Get the ID and description to save them
	var ID = "";
	var description = FMOperateIndex.movementSummaryControllers[newId].description;
	var movementSummaryGuid = "";
	var movementSummaryType = FMOperateIndex.movementSummaryControllers[newId].visibility;

	if (parentId === 'mainTab') {
		FMOperateIndex.contents = $.map(FMOperateIndex.contents, function (obj) {
			if (obj.id === newId) {
				ID = obj.name;
				movementSummaryGuid = obj.settings.movementSummaryGuid;
			}

			return obj;
		});
	}
	else {
		var parentGroupIdx = $.map(FMOperateIndex.contents, function (obj, index) {
			if (obj.id === parentId) {
				return 1;
			}

			return 0;
		});

		var foundTabGroupIdx = parentGroupIdx.indexOf(1);
		if (foundTabGroupIdx >= 0) {
			$.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj) {
				if (obj.id === newId) {
					ID = obj.name;
					movementSummaryGuid = obj.settings.movementSummaryGuid;
				}
				return obj;
			});
		}
	}

	// check to see if the data has really changed before making the call
	if (ID === FMOperateIndex.movementSummaryControllers[newId].movementSummaryId &&
		fontSize === FMOperateIndex.movementSummaryControllers[newId].fontSize &&
		FMOperateIndex.movementSummaryControllers[newId].rowDefinitions === JSON.stringify(rows) &&
		FMOperateIndex.movementSummaryControllers[newId].columnDefinitions === JSON.stringify(columnsWithNoMenu))
	{
		return;
	}

	$.ajax({
		url: 'SaveMovementSummary',
		type: 'Post',
		dataType: 'json',
		contentType: "application/json",
		 data: JSON.stringify({ "movementSummaryGuid": movementSummaryGuid, "id": ID, "description": description, "rows": JSON.stringify(rows), "columns": JSON.stringify(columnsWithNoMenu), "fontSize": fontSize, "jsRowVersion": jsRowVersion, "movementSummaryType": parseInt(movementSummaryType) }),
		 success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (inError == false) {
					// We get back the movementSummaryGuid during the save, for when we add a new movement summary
					var movementSummaryGuid = data.movementSummaryGuid;

					// If we found an existing movement summary with the same ID, visibility, site and owner
					if (data.duplicateFound) {
						FMErrorAndExceptionHandling.ShowError("Cannot Save Changes, duplicate Movement Summary.", null, messageAttributes);
						return;
					}
					var updatedRowVersion = data.rowVersion;

					// save the updates to prevent un-necessary refresh
					if (parentId === 'mainTab') {
						FMOperateIndex.contents = $.map(FMOperateIndex.contents, function (obj) {
							if (obj.id === newId) {
								obj.settings.movementSummaryGuid = movementSummaryGuid;
							}

							return obj;
						});
					}
					else {
						var parentGroupIdx = $.map(FMOperateIndex.contents, function (obj, index) {
							if (obj.id === parentId) {
								return 1;
							}

							return 0;
						});

						var foundTabGroupIdx = parentGroupIdx.indexOf(1);

						if (foundTabGroupIdx >= 0) {
							$.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj) {
								if (obj.id === newId) {
									obj.settings.movementSummaryGuid = movementSummaryGuid;
								}

								return obj;
							});
						}
					}

					FMOperateIndex.movementSummaryControllers[newId].columnDefinitions = JSON.stringify(columnsWithNoMenu);
					FMOperateIndex.movementSummaryControllers[newId].rowDefinitions = JSON.stringify(rows);
					FMOperateIndex.movementSummaryControllers[newId].rowVersionStr = updatedRowVersion;
				}

			}, messageAttributes);
		},
		 error: function (request, status, error) {
			FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
			}, messageAttributes);
		}
	});
};

FMOperateIndex.OpenMovementSummaryError = function (xhr, textStatus, error) {
	var activeDrawing = this.activeDrawing;
	var activeTab = this.activeTab;
	var newId = this.newId;

	// need to make  sure that the error we are getting is because we close the page before getting the response
	if (xhr.status != 0) {
		FMErrorAndExceptionHandling.ShowException(xhr,
			textStatus,
			error,
			function () {
				$("#loadermovementsummary" + newId).remove();
			});
	}
	// done reloading the tab
	FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
};


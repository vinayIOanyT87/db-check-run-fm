
if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	debugger;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

//===========================================================================
// This function closes the movement history.
//===========================================================================
FMOperateIndex.CloseMovementHistory = function (id)
{
	var tabName = $("#MovementHistoryTabName").text();
	if (tabName) {
		// Save the view state if the tab is closed.
		if (tabName === id) {
			MovementHistoryTab.SaveViewState();
		}
	}
};

//===========================================================================
// Update Movement History settings 
//===========================================================================
FMOperateIndex.UpdateMovementHistorySettings = function (parentControl, movementSummaryControllerId, parentGroupTab)
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
	if (!FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].isOwnByMe) {
		dataPopover.tip().find('.popover-content').find('[name=MovementSummaryVisibilitySetting]').prop("disabled", "disabled");
	}

	if (!FMOperateIndex.movementSummaryControllers[movementSummaryControllerId].isEditable) {
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
		//		$("#pointgroupprint").html('');
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
	dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupAutoSchedule]').on('click', function (event) {
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

		$("#PointGroupReportScheduleRepeatNever").click();

		var tabName = $("a[data-target='#" + FMOperateIndex.movementSummaryControllers[movementSummaryControllerId]._id + "'] .tab-name").text();

		$("#PointGroupReportScheduleName").val(tabName);
		$("#PointGroupReportScheduleName").attr("data-guid", movementSummaryGuid);

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
// Open the selected MovementHistory on a Tab
/////////////////////////////////////////////////////
FMOperateIndex.OpenMovementHistory = function (movementHistoryTabId)
{
	//check to see if we have one open already (only when clicking on the menu, not when restoring the screen)
	if (FMOperateIndex.restoringView === false)
	{
		var parentTab = "";
		var summaryTab = "";

		FMOperateIndex.UpdateOperateHelpKey("movementHistory");
		$.each(FMOperateIndex.contents, function (index, level1Tab)
		{
			if (level1Tab.type === "group")
			{
				$.each(FMOperateIndex.contents[index].settings, function (index, level2Tab)
				{
					if (level2Tab.type === "movementHistory")
					{
						summaryTab = level2Tab.id;
						parentTab = level1Tab.id;
					}
				});
			} else if (level1Tab.type === "movementHistory")
			{
				summaryTab = level1Tab.id;
				parentTab = "";
			}
		});

		if (parentTab !== "")
		{
			$('a[data-target=\'#' + parentTab + '\'').click();
		}
		if (summaryTab !== "")
		{
			$('a[data-target=\'#' + summaryTab + '\'').click();
			return false;
		}
	}

	// can't open movement history in a group if there are no groups (there is always 1 tab, the tab to add groups )
	if (FMOperateIndex.isTabGroupEnabled && $('#tabList').children().length === 1)
	{
		FMLayout.Alert('Cannot open a Movement History without a Group.', 'Error');
		return null;
	}

	FMOperateIndex.restoringView = false;


	if (!movementHistoryTabId)
	{
		// this should only be hit when openning a new summary. Not restoring.
		FMOperateIndex.openingNewMovementHistory = true;

		movementHistoryTabId = 'Movement History';
	}
	else
	{
		FMOperateIndex.openingNewMovementHistory = false;
	}

	var newId = FMOperateIndex.AddTab(movementHistoryTabId);

	if (newId === null) return false;

	var activeTab = 'mainTab';

	if (FMOperateIndex.isTabGroupEnabled)
	{
		activeTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
	}

	FMOperateIndex.PersistNewControl(activeTab, newId, movementHistoryTabId, 'movementHistory', {});

	// start the process of restoring the tab
	FMOperateIndex.restoringScreenQueueInProgress[newId] = true;

	$("#" + newId).getNiceScroll().remove();

	$('<div id="MovementHistory' + newId + '" class="movement-history movementhistorytemp" style="position: absolute; top: 5px; left: 5px;right: 5px;bottom: 5px;"></div>').appendTo('#' + newId);
	$('<div id="MovementHistory' + newId + 'container" class="movement-history-panel active" style="overflow:hidden"></div>').appendTo('#MovementHistory' + newId);

	$('<div id="MovementHistoryTabName" hidden="hidden" >' + newId + '</div>').appendTo('#MovementHistory' + newId);
	$('<div id="LoadingImageMovementHistory' + newId + '" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#' + newId);

	// put messages on the actual tab
	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#movementhistory' + newId) };

	// Create the movement history tab.
	MovementHistoryTab.CreateMovementHistoryTab(activeTab, newId, stack_bottomright_operatortab);
	return newId;
};

//==================================================================================
// This function will return the max movement history tab number.
//==================================================================================
FMOperateIndex.GetMovementHistoryMaxId = function (movementHistoryIndex, level1Tab)
{
	var parts = level1Tab.name.split(" ");

	if (parts.length != 3)
	{
		return 0;
	}

	var movementHistoryNumber = parseInt(parts[2]);

	if (movementHistoryNumber > movementHistoryIndex)
	{
		return movementHistoryNumber;
	}

	return movementHistoryIndex;
};

FMOperateIndex.OpenMovementHistoryError = function (xhr, textStatus, error)
{
	var newId = this.newId;

	// need to make  sure that the error we are getting is because we close the page before getting the response
	if (xhr.status != 0)
	{
		FMErrorAndExceptionHandling.ShowException(xhr,
			textStatus,
			error,
			function () {
				$("#LoaderMovementHistory" + newId).remove();
			});
	}

	// done reloading the tab
	FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
};



// main object for the trend screen
var FMTrendIndex = {
	activeTrends: [],
	drawingIdPrefix: "diagram"
};

if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	debugger;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

FMTrendIndex.editTrend = function (parentControl, trendDisplayId, parentGroupTab) {

	drawingNumber = trendDisplayId.replace("trendDisplay","");

	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph) {
		return;
	}

	var trend = trendGraph.GetTrend();
	if (trend.PointTemplateGuid != '00000000-0000-0000-0000-000000000000') {
		return;
	}

	var popover = $(parentControl).popover("destroy").popover({
		container: 'body',
		placement: 'bottom',
		html: true,
		content: $('#TrendConfigurationSettings').html(),
		trigger: "manual"
	});

	var dataPopover = popover.data('bs.popover');
	$(parentControl).popover('show');
	$("#customModalBackground").removeClass("hidden");

	dataPopover.tip().find('.popover-content').find('#trendEditName').val(trend.ID);
	dataPopover.tip().find('.popover-content').find('#trendEditDescription').val(trend.Description);


	dataPopover.tip().find('.popover-content').find('#trendEditSaveButton').on('click', function (event) {
		if ($('#ModifyTrendRight').val() == 'False')
			return;

		trend.ID = dataPopover.tip().find('.popover-content').find('#trendEditName').val();
		trend.Description = dataPopover.tip().find('.popover-content').find('#trendEditDescription').val()

		// remove events
		dataPopover.tip().find('.popover-content').find('#trendEditSaveButton').off('click');
		dataPopover.tip().find('.popover-content').find('#trendEditCancelButton').off('click');

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();

		FMTrendIndex.saveTrend(trend);

		// update the trends
		var controls = FMOperateIndex.contents;
		$.each(controls, function (index, control) {
			if (control.type === 'group') {
				$.each(control.settings, function (index, setting) {
					if (setting.type === 'trend'
					&& trend.TrendGuid === setting.settings.guid) {
						$('#' + setting.id + '-tab-name').text(trend.ID);
						setting.name = trend.ID;
						var id = $('#' + setting.id).children().first().attr("id")
						drawingNumber = id.replace("trendDisplay", "");
						FMTrendMenuBar.setDescription(drawingNumber, trend.Description);

					}
				});
			}
			else if (control.type === 'trend'
			&& trend.TrendGuid === control.settings.guid) {
				$('#' + control.id + '-tab-name').text(trend.ID);
				control.name = trend.ID;
				var id = $( '#' + control.id ).children().first().attr( "id" );
				drawingNumber = id.replace("trendDisplay", "");
				FMTrendMenuBar.setDescription(drawingNumber, trend.Description);
			}
		});

		FMOperateIndex.PersistScreenConfiguration();

		if ($(".operateMenuItem.active a").attr("id") === "menuTrends") {
			FMOperateIndex.refreshHamburgerMenu = true;
		}



	});

	dataPopover.tip().find('.popover-content').find('#trendEditCancelButton').on('click', function (event) {

		// remove events
		dataPopover.tip().find('.popover-content').find('#trendEditSaveButton').off('click');
		dataPopover.tip().find('.popover-content').find('#trendEditCancelButton').off('click');

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});

};

FMTrendIndex.addTrend = function () {
	$("#trendSaveScreen #trendNewName").val('');
	$("#trendSaveScreen #trendNewDesc").val('');

	$('body').modalmanager('loading');
	$("#trendSaveScreen").modal("show");
	$("#trendSaveScreen #trendSaveSaveButton").off('click');

	$("#trendSaveScreen #trendSaveSaveButton").on('click', function () {
		// Try to save the trend
		var trendVisibilityType = $("#trendSaveScreen input[type=radio][name=trendPrivateSaveAs]:checked").val();
		var trendName = $("#trendSaveScreen #trendNewName").val();
		var trendDescription = $("#trendSaveScreen #trendNewDesc").val();

		// make sure we have a name
		if (trendName === "") {
			$("#trendSaveScreen #trendNewName").parent().addClass('has-error');
			return false;
		}

		var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };
		var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator };

		// remove previous notifications
		PNotify.removeStack(stack_bottomright_operator);
		$('<div id="loadertrendmain" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('body');

		$.ajax({
			url: 'AddTrend',
			type: 'Post',
			dataType: 'json',
			contentType: "application/json",
			data: JSON.stringify({ "id": trendName, "description": trendDescription }),
			success: function (response) {
				$("#loadertrendmain").remove();
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
					if (!inError) {
						// if there is a duplicate show an error and go to the first page
						if (data.duplicateFound) {
							FMLayout.Alert("There is already a Trend with the same Name.", "Duplicate", null);

						}
						else {
                            // this is called for a new trend
						    $("#trendSaveScreen").modal("hide");
							FMOperateIndex.openTrend(trendName, {pointTrend: false, guid: data.trendGuid},true);
						}
						// refresh the hamburger the menu (list of trends)
						if ($(".operateMenuItem.active a").attr("id") === "menuTrends") {
							FMOperateIndex.refreshHamburgerMenu = true;
						}
					}
				}, messageAttributes);
			},
			error: function (request, status, error) {
				$("#loadertrendmain").remove();
				FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
				}, messageAttributes);
			}
		});
	});
};



FMTrendIndex.openTrend = function (trendId, settings, newTrendDisplay) {
    // Persist the new tab so it can be re-open when the screen is reloaded
	var activeTab = 'mainTab';
	if (FMOperateIndex.isTabGroupEnabled) {
		activeTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
	}

	var newId = FMOperateIndex.AddTab(trendId, true);

	if (newId === null) return false;

	var activeDrawing = FMOperateIndex.numberDrawings = FMOperateIndex.numberDrawings + 1;

	// do not specify the settings when creating the drawing.  We will update it when we retrieve the actual drawing
	FMOperateIndex.PersistNewControl(activeTab, newId, trendId, 'trend', {});

	FMOperateIndex.restoringScreenQueueInProgress[newId] = true;

	$('<div id="loader' + newId + '0" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#' + newId);

	//$('<div id="Trendtabname" style="hidden" >' + newId + '</div>').appendTo('#' + newId);

	// put messages on the actual tab
	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#' + newId) };

	$('#' + newId + '-tab-name').text('');
	var imageId = "TrendTabImageId-" + newId;

	$('<id="TextImage' + newId + '0" ><img id="' + imageId + '" src="' + window.applicationRootName + '/FMWebApp/images/trendNoBorder.png" >' + trendId + '</>').appendTo('#' + newId + '-tab-name');
	$.ajax({
		type: 'get',
		dataType: 'json',
		cache: false,
		url: 'GetOperateTrend',
		activeDrawing: activeDrawing,
		activeTab: activeTab,
		newId: newId,
		data: 'pointTrend=' + settings.pointTrend + '&guidString=' + settings.guid,
		success: function (response) {
			var activeDrawing = this.activeDrawing;
			var activeTab = this.activeTab;
			var newId = this.newId;

			$('#loader' + newId + '0').remove();
			var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

			FMErrorAndExceptionHandling.HandleMessages(response,
				function (data, inError) {
					if (!inError) {
						var trend = response.Data;

						$('#' + newId + '-tab-name').text('');
						imageId = "TrendTabImageId-" + newId;

						$('<id="TextImage' + newId + '0" ><img id="' + imageId + '" src="' + window.applicationRootName + '/FMWebApp/images/trendNoBorder.png" >' + trend.ID + '</>').appendTo('#' + newId + '-tab-name');

						settings.name = trend.ID;

						$('<div id="trendDisplay' + activeDrawing + '" class="FMTrend"></div>').appendTo('#' + newId);
						$('<div id="graphMenuBar' + activeDrawing + '" class="FMTrendMenuBar"></div>').appendTo('#trendDisplay' + activeDrawing);
						$('<div id="diagram' + activeDrawing + '" class="FMTrendGraph" data-drawing-number="' + FMOperateIndex.numberDrawings + '"> Your browser does not support the HTML5 canvas.</div>').appendTo('#trendDisplay' + activeDrawing);
						$('<div id="graphLegend' + activeDrawing + '" class="FMTrendLegend"></div>').appendTo('#trendDisplay' + activeDrawing);
						$('<div id="graphTable' + activeDrawing + '" class="FMTrendTable hidden"></div>').appendTo('#trendDisplay' + activeDrawing);



						settings.name = trend.ID;
						var fmTrendGraph = new FMTrendGraph(settings);
						var trendInstance = {};
						trendInstance.DrawingNumber = activeDrawing;
						trendInstance.TrendGraph = fmTrendGraph;
						FMTrendIndex.activeTrends.push(trendInstance);

						FMOperateIndex.PersistUpdateDrawingSettings(activeTab, newId, { pointTrend: settings.pointTrend, guid: settings.guid, drawingIndex: activeDrawing });

						FMTrendMenuBar.initMenuBar(activeDrawing, trend);
						FMTrendLegend.initLegend(activeDrawing, trend);

						fmTrendGraph.LoadTrend(activeDrawing, trend, newTrendDisplay);
						FMTrendMenuBar.selectTrendZoomTypeOnChange(null, activeDrawing);
						FMTrendMenuBar.setStartAndEnd(activeDrawing);

						$('#' + newId).getNiceScroll().remove();  // we don't need a scrollbar on the page

						if ( activeTab === 'mainTab' )
						{
							// refresh the scroll buttons for the tabs (do it async to give time to display the contents of the tab)
							$("#tabList").scrollingTabs('refresh');	
						}

					}
				},
				messageAttributes);
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		},
		error: function (xhr, textStatus, error) {
			var activeDrawing = this.activeDrawing;
			var activeTab = this.activeTab;
			var newId = this.newId;

			if (xhr.status != 0) {
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
						$('#loader' + newId + '0').remove();
					});
			}
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;

		}
	});
	return newId;
}



//Reload the graph section of a trend that is already being displayed, with an updated trend object
FMTrendIndex.reloadTrend = function (drawingNumber, trend) {
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (trendGraph)
		trendGraph.ReloadTrend(trend);
}



FMTrendIndex.saveTrend = function (trend) { 

	var stack_bottomright_operatortab = { "dir1" : 'up', "dir2": 'left', "firstpos1": 25, "firstpos2" : 25, "context": $('#trendSubMenuList')};
	var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	var result = false;

	$.ajax({
		url: 'SaveTrend',
		type: 'post',
		headers: headers,
		async: false,
		contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
		data: 'trendString=' + JSON.stringify(trend).replace(/\/Date/g, "\\\/Date").replace(/\)\//g, "\)\\\/")
,
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					result = true;
				}
			});
		},
		error:
			function (request, status, error) {
				FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
			}
	});

	return result;
}




FMTrendIndex.getTrendGraph = function (drawingNumber) {
	var trendGraph = null;
	for (var i = 0; i < FMTrendIndex.activeTrends.length; i++) {
		if (FMTrendIndex.activeTrends[i].DrawingNumber == drawingNumber) {
				trendGraph = FMTrendIndex.activeTrends[i].TrendGraph;
		}
	}
	return trendGraph;
}


FMTrendIndex.refreshTrend = function (drawingNumber) {
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (trendGraph)
		trendGraph.RefreshTrend();

	if (FMOperateIndex.restoringSaveInitialConfig === false) {
		FMOperateIndex.restoringSaveInitialConfig = true;
		FMOperateIndex.PersistScreenConfiguration();
		FMOperateIndex.restoringSaveInitialConfig = false;
	}
	else
		FMOperateIndex.PersistScreenConfiguration();
}


FMTrendIndex.closeTrend = function (drawingNumber)
{
	for (var i = 0; i < FMTrendIndex.activeTrends.length; i++) {
		if (FMTrendIndex.activeTrends[i].DrawingNumber == drawingNumber) {

			// destroy the table to avoid memory leaks
			var trendGraph = FMTrendIndex.activeTrends[i].TrendGraph;
			if (!trendGraph)
				return;

			FMOperateIndex.unsubscribeTagWebWorker(drawingNumber + FMTrendIndex.activeTrends[i].TrendGraph.GetTrend().TrendGuid);

			// if we have a grid already clean it up
			var grid = trendGraph.GetTable();
			if (grid !== null) {
				grid.destroy();
			}

			FMTrendIndex.activeTrends[i].TrendGraph.SetActive(false);
			FMTrendIndex.activeTrends.splice(i, 1);
			break;
		}
	}
}


//Retrieves the drawing number from an id with the format "diagram<drawingNumber>"
FMTrendIndex.getDrawingNumberFromId = function (drawingId) {
	var result = null;
	var value = null;
	if ((drawingId) && (drawingId.length > drawingIdPrefix.length))
		value = parseInt(drawingId.substring(drawingIdPrefix.length));
	if (isNaN(value))
		result = value;
	return result;		
}

FMTrendIndex.convertUTCDateToLocalDate = function (date) {
	date = new Date(date);
	var localOffset = date.getTimezoneOffset() * 60000;
	var localTime = date.getTime();
	date = localTime - localOffset;
	return new Date(date);
}

FMTrendIndex.convertLocalDateToUTCDate = function (date) {
	date = new Date(date);
	var localOffset = date.getTimezoneOffset() * 60000;
	var localTime = date.getTime();
	date = localTime + localOffset;
	return new Date(date);
}


//Return the new content for the tooltip of a given graph, using the Tooltip ContentFormatter event parameter
FMTrendIndex.getGraphTooltipContent = function (drawingNumber, e) {
	var content = "";
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (trendGraph)
		content = trendGraph.GetTooltipContent(e);
	return content;
}


//Update all the controls that are sensitive to the timebar location in a given graph
FMTrendIndex.updateClientsForTimebar = function (drawingNumber, xLocation, penDataCollection) {
	var penValues = new Array();
	if (penDataCollection != null) {
		for (var i = 0; i < penDataCollection.length; i++) {
			var value = null;
			if (penDataCollection[i] != null)
					value = penDataCollection[i].y;
			penValues.push(value);
		}
	}
	FMTrendLegend.UpdatePenValues(drawingNumber, penValues);
	FMTrendLegend.UpdatePenStatus(drawingNumber, penDataCollection);
	FMTrendLegend.UpdatePenAlarmStates(drawingNumber, penDataCollection);
}


//Update all the controls that are sensitive to the Min/Max pen value settings in a given graph, for a given pen
FMTrendIndex.updateClientsForPenMinMax = function (drawingNumber, penIndex, min, max) {
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (trendGraph)
		trendGraph.UpdatePenMinMaxRange(penIndex, min, max);
}

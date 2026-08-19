
// Trend Menu Bar object
var FMTrendMenuBar = {
	disableDateTimePickerEvent: false
	, headerCols: {}

};

if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	debugger;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

FMTrendMenuBar.initMenuBar = function (drawingNumber, trend) {
    var menuBarHTMLString = '<div id="graphMenuBar' + drawingNumber + '" class="FMTrendMenuBar"> \n' +

			'<div id="trendMenuContainer' + drawingNumber + '" class="container-fluid"> \n' +

			'<div id="trendMenuFirstRow' + drawingNumber + '" class="row"> \n' +
				'<div class="col-lg-3 col-md-3"> \n' +
					'<h3 id="trendMenuDescription' + drawingNumber + '" class="control-label" style="margin-top: 15px;">' + ((trend.Description) ? trend.Description : '') + '</h3> \n' +
				'</div> \n' +

				'<div class="col-lg-2 col-md-2 text-center"> \n' +
					'<div id="trendMenuTable' + drawingNumber + '" title="' + $('#TrendMenu-Table').val() + '" class="FMTrendMenuTable" onmousedown = "FMTrendMenuBar.tableButtonDown(this,' + drawingNumber + ')" onmouseup = "FMTrendMenuBar.tableButtonUp(this,' + drawingNumber + ')"> \n' +
					'</div> \n' +
					'<div id="trendMenuTrend' + drawingNumber + '" title="' + $('#TrendMenu-Trend').val() + '" class="FMTrendMenuTrend" onmousedown = "FMTrendMenuBar.trendButtonDown(this,' + drawingNumber + ')" onmouseup = "FMTrendMenuBar.trendButtonUp(this,' + drawingNumber + ')"> \n' +
					'</div> \n' +
				'</div> \n' +

				'<div class="col-lg-3 col-md-3 text-center"> \n' +
					'<div id="trendMenuPan' + drawingNumber + '" title="' + $('#TrendMenu-Pan').val() + '" class="FMTrendMenuPan" onmousedown = "FMTrendMenuBar.panButtonDown(this,' + drawingNumber + ')" onmouseup = "FMTrendMenuBar.panButtonUp(this,' + drawingNumber + ')"> \n' +
					'</div> \n' +
					'<div id="trendMenuZoomOut' + drawingNumber + '" title="' + $('#TrendMenu-ZoomOut').val() + '" class="FMTrendMenuZoomOut" onmousedown = "FMTrendMenuBar.zoomOutButtonDown(this,' + drawingNumber + ')" onmouseup = "FMTrendMenuBar.zoomOutButtonUp(this,' + drawingNumber + ')"> \n' +
					'</div> \n' +
					'<div id="trendMenuZoom' + drawingNumber + '" title="' + $('#TrendMenu-Zoom').val() + '"  class="FMTrendMenuZoom" onmousedown = "FMTrendMenuBar.zoomButtonDown(this,' + drawingNumber + ')" onmouseup = "FMTrendMenuBar.zoomButtonUp(this,' + drawingNumber + ')"> \n' +
					'</div> \n' +
					'<select id="selectTrendZoomType' + drawingNumber + '" title="' + $('#TrendMenu-ZoomType').val() + '" class="form-control FMTrendMenuZoomType" onchange="FMTrendMenuBar.selectTrendZoomTypeOnChange(this,' + drawingNumber + ')"> \n' +
					'<option value="x">X</option> \n' +
					'<option value="y">Y</option> \n' +
					'<option value="xy"  selected>XY</option> \n' +
					'</select> \n' +

				'</div> \n' +

				'<div class="col-lg-2 col-md-2 text-center"> \n' +
					'<div id="trendMenuExport' + drawingNumber + '" title="' + $('#TrendMenu-Export').val() + '" class="FMTrendMenuExport" onclick = "FMTrendMenuBar.exportButtonDown(event, this,' + drawingNumber + ')" > \n' +
					'<a id="trendMenuExportAction' + drawingNumber + '" style="display: none"> \n' +
					'</a> \n' +
					'</div> \n' +
					'<div id="trendMenuPrint' + drawingNumber + '" title="' + $('#TrendMenu-Print').val() + '" class="FMTrendMenuPrint" onmousedown = "FMTrendMenuBar.printButtonDown(this,' + drawingNumber + ')" onmouseup = "FMTrendMenuBar.printButtonUp(this,' + drawingNumber + ')"> \n' +
					'</div> \n' +
				'</div> \n' +

				'<div class="col-lg-2 col-md-2 text-right" style="padding-right: 30px;"> \n' +
					'<div id="trendMenuLeft' + drawingNumber + '" title="' + $('#TrendMenu-Left').val() + '" class="FMTrendMenuLeft" onmousedown = "FMTrendMenuBar.leftButtonDown(this,' + drawingNumber + ')" onmouseup = "FMTrendMenuBar.leftButtonUp(this,' + drawingNumber + ')"> \n' +
					'</div> \n' +
					'<div id="trendMenuRight' + drawingNumber + '" title="' + $('#TrendMenu-Right').val() + '" class="FMTrendMenuRight" onmousedown = "FMTrendMenuBar.rightButtonDown(this,' + drawingNumber + ')" onmouseup = "FMTrendMenuBar.rightButtonUp(this,' + drawingNumber + ')"> \n' +
					'</div> \n' +
					'<div id="trendMenuPlayPause' + drawingNumber + '" title="' + $('#TrendMenu-Pause').val() + '" class="FMTrendMenuPause" onmousedown = "FMTrendMenuBar.playPauseButtonDown(this,' + drawingNumber + ')" onmouseup = "FMTrendMenuBar.playPauseButtonUp(this,' + drawingNumber + ')"> \n' +
					'</div> \n' +
				'</div> \n' +

			'</div> \n' +

			'<div id="trendMenuSecondRow' + drawingNumber + '" class="row" style="padding-top:  7px;"> \n' +
				'<div class="col-sm-4 text-left" style="padding-left: 50px;"> \n' +
					'<label class="setting-radio-control setting-radio-control--radio">\n' +
					'	<input id="trendMenuInTheLast' + drawingNumber + '" type="radio" name="trendMenuTimeOptions' + drawingNumber + '"  value="0"' + ((trend.Mode === 0) ? "checked" : "") + ' onchange="FMTrendMenuBar.timeOptionsOnChange(this,' + drawingNumber + ')"/>\n' +
					'	<div class="setting-radio-control__indicator"></div>\n' +
					'</label>\n' +
					'<label style="margin-top: 7px;" id="trendMenuInTheLastLabel' + drawingNumber + '">' + $('#TrendMenu-InTheLast').val() + '</label> \n' +
					'<input style="    margin-left: 10px;max-width: 100px;display: inline-block;width: calc( 100% - 220px);" title="' + $('#TrendMenu-Period').val() + '" class="form-control FMTrendMenuPeriod" id="trendMenuPeriod' + drawingNumber + '" type="number" maxlength=5 value="' + trend.Period + '" oninput="FMTrendMenuBar.periodInputOnInput(this,' + drawingNumber + ')" /> \n' +
					'<select style="    margin-left: 10px;max-width: 95px;display: inline-block;width: calc( 100% - 180px);" title="' + $('#TrendMenu-PeriodType').val() + '" class="form-control FMTrendMenuPeriodType" id="trendMenuPeriodType' + drawingNumber + '" change="FMTrendMenuBar.periodTypeOnChange(this, ' + drawingNumber + ')" onclick="FMTrendMenuBar.periodTypeSelectOnClick(this,' + drawingNumber + ')" onchange="FMTrendMenuBar.periodTypeSelectOnChange(this,' + drawingNumber + ')">' +
					'<option value="0" ' + ((trend.PeriodType === 0) ? "selected" : "") + '>' + $('#TrendMenu-Minutes').val() + '</option>' +
					'<option value="1" ' + ((trend.PeriodType === 1) ? "selected" : "") + '>' + $('#TrendMenu-Hours').val() + '</option>' +
					'<option value="2" ' + ((trend.PeriodType === 2) ? "selected" : "") + '>' + $('#TrendMenu-Days').val() + '</option>' +
					'</select>' +
				'</div> \n' +

				'<div class="col-sm-7 text-center"> \n' +
					'<label class="setting-radio-control setting-radio-control--radio">\n' +
					'	<input id="trendMenuTimeRange' + drawingNumber + '" type="radio" name="trendMenuTimeOptions' + drawingNumber + '"  value="1"' + ((trend.Mode === 1) ? "checked" : "") + ' onchange="FMTrendMenuBar.timeOptionsOnChange(this,' + drawingNumber + ')"/>\n' +
					'	<div class="setting-radio-control__indicator"></div>\n' +
					'</label>\n' +
					'<label style="" id="trendMenuTimeRangeLabel' + drawingNumber + '">' + $('#TrendMenu-TimeRange').val() + '</label> \n' +
					'<label style="display: inline-block;height:20px; margin-right: 5px; margin-left: 20px; font-style:italic" id="trendMenuStartDateLabel' + drawingNumber + '">' + $('#TrendMenu-FromDate').val() + '</label> \n' +
					'<input type="text" class="form-control FMTrendMenuDateTime text-center" id="trendMenuStartDate' + drawingNumber + '" value="" onchange="FMTrendMenuBar.startInputOnChange(this,' + drawingNumber + ')" /> \n' +
					'<label style="display: inline-block;height:25px; margin-right: 5px; margin-left: 15px; font-style:italic" id="trendMenuEndDateLabel' + drawingNumber + '" >' + $('#TrendMenu-ToDate').val() + '</label> \n' +
					'<input type="text" class="form-control FMTrendMenuDateTime text-center" id="trendMenuEndDate' + drawingNumber + '" value=""  onchange="FMTrendMenuBar.endInputOnChange(this,' + drawingNumber + ')" /> \n' +
				'</div> \n' +

				'<div class="col-sm-1 text-right" style="padding-right: 30px;"> \n' +
					'<div id="trendMenuRefresh' + drawingNumber + '" title="' +$('#TrendMenu-Refresh').val() + '"  class="form-group FMTrendMenuRefresh" onmousedown = "FMTrendMenuBar.refreshButtonDown(this,' +drawingNumber + ')" onmouseup = "FMTrendMenuBar.refreshButtonUp(this,' + drawingNumber + ')"> \n' +
		         '</div> \n' +
		      '</div> \n' +
			'</div> \n' +

        '</div> \n';

    var menuBar = $('#graphMenuBar' + drawingNumber);
    menuBar.replaceWith(menuBarHTMLString);

    FMTrendMenuBar.disableDateTimePickerEvent = true;


	var numFormatInfoString = $('#NumberFormatInfoString').val();
	var numFormatInfo = JSON.parse(numFormatInfoString);
	FMLayout.dateFormat = ConvertToJQueryUIDateFormat(numFormatInfo.ShortDatePattern);
	FMLayout.timeFormat = ConvertToJQueryUITimeFormat(numFormatInfo.TimePattern);
	FMLayout.calendarLocation = window.applicationRootName + '/dispatchwebapp/images';

    $('#trendMenuStartDate' + drawingNumber).datetimepicker({
		buttonImage: FMLayout.calendarLocation + '/calendar.gif',
    	buttonImageOnly: true,
		  showOn: "button",
		  showTimezone: false,
		  useLocalTimezone: false,
		  defaultTimezone: $("#datepickerTimezoneString").val(),
    	dateFormat: FMLayout.dateFormat,
    	timeFormat: FMLayout.timeFormat,
    	showSecond: (FMLayout.timeFormat.indexOf('ss') == -1) ? false : true,
    	beforeShow: function () {
    		setTimeout(function () {
    			$('.ui-datepicker').css('z-index', 500);
    		}, 0);
    	},
    	onSelect: function (d, i) {
    		if (d !== i.lastVal) {
    			$(this).change();
    		}
    	}
    });

	 $('#trendMenuStartDate' + drawingNumber).datetimepicker("setDate", FMOperateIndex.translateClientDateTimeToSiteMomentTime(trend.Start).format(numFormatInfo.ShortDatePattern.toUpperCase() + " " + ConvertToMomentUITimeFormat(numFormatInfo.TimePattern)));

    $('#trendMenuEndDate' + drawingNumber).datetimepicker({
		  buttonImage: FMLayout.calendarLocation + '/calendar.gif',
    	buttonImageOnly: true,
		  showOn: "button",
		  showTimezone: false,
		  useLocalTimezone: false,
		  defaultTimezone: $("#datepickerTimezoneString").val(),
    	dateFormat: FMLayout.dateFormat,
    	timeFormat: FMLayout.timeFormat,
    	showSecond: (FMLayout.timeFormat.indexOf('ss') == -1) ? false :true,
    	beforeShow: function () {
    		setTimeout(function () {
    			$('.ui-datepicker').css('z-index', 500);
    		}, 0);
    	},
    	onSelect: function (d, i) {
    		if (d !== i.lastVal) {
    			$(this).change();
    		}
    	}
    });

	 $('#trendMenuEndDate' + drawingNumber).datetimepicker("setDate", FMOperateIndex.translateClientDateTimeToSiteMomentTime(trend.End).format(numFormatInfo.ShortDatePattern.toUpperCase() + " " + ConvertToMomentUITimeFormat(numFormatInfo.TimePattern)));
	 FMTrendMenuBar.setStartAndEnd(drawingNumber);

    var periodTypeSelect = $('#trendMenuPeriodType' + drawingNumber);
    periodTypeSelect.data("previous", periodTypeSelect.val());

    FMTrendMenuBar.disableDateTimePickerEvent = false;

    $('#trendMenuPlayPause' + drawingNumber).prop("disabled", (trend.Mode === 1) ? true : false);
    $('#trendMenuPan' + drawingNumber).prop("disabled", (trend.Mode === 0) ? true : false);
    $('#trendMenuZoomOut' + drawingNumber).prop("disabled", (trend.Mode === 0) ? true : false);
    $('#trendMenuZoom' + drawingNumber).prop("disabled", (trend.Mode === 0) ? true : false);

    if (trend.Mode === 1) {
    	$('#trendMenuZoom' + drawingNumber).addClass('FMTrendMenuZoomActive').removeClass('FMTrendMenuZoom');
    }
    else {
    	$('#trendMenuZoom' + drawingNumber).addClass('FMTrendMenuZoom').removeClass('FMTrendMenuZoomActive');
    }



    FMTrendMenuBar.setTimeControls(drawingNumber);

    var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
    if (!trendGraph) {
    	return;
    }
}

FMTrendMenuBar.tableButtonDown = function (e, drawingNumber) {
	$('#trendMenuTable' + drawingNumber).addClass('FMTrendMenuTableClick').removeClass('FMTrendMenuTable');

	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	// if we have a grid already clean it up
	var grid = trendGraph.GetTable();
	if (grid !== null) {
		grid.destroy();
	}

	$('#diagram' + drawingNumber).removeClass("hidden").addClass("hidden");
	$('#graphLegend' + drawingNumber).removeClass("hidden").addClass("hidden");
	$('#trendMenuSecondRow' + drawingNumber).removeClass("hidden").addClass("hidden");
	$('#graphTable' + drawingNumber).removeClass("hidden");
	$('#graphMenuBar' + drawingNumber).removeClass("tableShown").addClass('tableShown');
	$('#TrendLegendRows' + drawingNumber).getNiceScroll().hide();

	// the following code is to work around a nicescroll bug.  The vertical bars are blinking if not forced to show as a block
	var scrollId = $('#TrendLegendRows' + drawingNumber).getNiceScroll(0).id;
	$('#' + scrollId).removeClass('nicescroll-rails-vr');

	var columns = [
	  { id: "Time Stamp", name: "TimeStamp", field: "TimeStamp", minWidth: 100, width: 200 }
	];

	var pens = trendGraph.GetTrend().Pens;
	for (var penIdx = 0; penIdx < pens.length; penIdx++)
	{
		columns.push({ id: pens[penIdx].PointID + '.' + pens[penIdx].TagID,
			name: pens[penIdx].PointID + '.' + pens[penIdx].TagID,
			field: pens[penIdx].PointID + '.' + pens[penIdx].TagID + '.Value',
			minWidth: 100,
			width: 200,
			formatter: FMTrendMenuBar.trendDataFormatter
		});

		columns.push({
			id: "Status",
			name: "Status",
			field: pens[penIdx].PointID + '.' + pens[penIdx].TagID + '.Status',
			minWidth: 100,
			width: 200,
			formatter: FMTrendMenuBar.trendDataFormatter
		});

	}

	var options = {
		enableCellNavigation: true,
		rowHeight: 24,
		enableColumnReorder: false
	};

	grid = new Slick.Grid("#graphTable" + drawingNumber, trendGraph.GetChartDataAsArrayObject(), columns, options);
	trendGraph.SetTable( grid );

	grid.resizeCanvas();

	$('#trendMenuTable' + drawingNumber).addClass('FMTrendMenuTable').removeClass('FMTrendMenuTableClick');

}

FMTrendMenuBar.trendDataFormatter = function( row, cell, value, columnDef, dataContext )
{
	return value;
}

FMTrendMenuBar.tableButtonUp = function (e, drawingNumber)
{
}

FMTrendMenuBar.trendButtonDown = function (e, drawingNumber) {
	$('#trendMenuTrend' + drawingNumber).addClass('FMTrendMenuTrendClick').removeClass('FMTrendMenuTrend');

	$('#diagram' + drawingNumber).removeClass("hidden");
	$('#graphLegend' + drawingNumber).removeClass("hidden");
	$('#trendMenuSecondRow' + drawingNumber).removeClass("hidden");
	$('#graphTable' + drawingNumber).removeClass("hidden").addClass("hidden");
	$('#graphMenuBar' + drawingNumber).removeClass("tableShown");
	// the following code is to work around a nicescroll bug.  The vertical bars are blinking if not forced to show as a block
	var scrollId = $('#TrendLegendRows' + drawingNumber).getNiceScroll(0).id;
	$('#' + scrollId).removeClass('nicescroll-rails-vr').addClass('nicescroll-rails-vr');

	$('#TrendLegendRows' + drawingNumber).getNiceScroll().show();
	$('#TrendLegendRows' + drawingNumber).getNiceScroll().resize();


}

FMTrendMenuBar.trendButtonUp = function (e, drawingNumber) {
	$('#trendMenuTrend' + drawingNumber).addClass('FMTrendMenuTrend').removeClass('FMTrendMenuTrendClick');
}

FMTrendMenuBar.panButtonDown = function (e, drawingNumber) {
	$('#trendMenuPan' + drawingNumber).addClass('FMTrendMenuPanActive').removeClass('FMTrendMenuPan');
	$('#trendMenuZoom' + drawingNumber).addClass('FMTrendMenuZoom').removeClass('FMTrendMenuZoomActive');

	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	trendGraph.ClickCanvasJSMenuButton('Pan');
}

FMTrendMenuBar.panButtonUp = function (e, drawingNumber) {
}

FMTrendMenuBar.zoomOutButtonDown = function (e, drawingNumber) {
	$('#trendMenuZoomOut' + drawingNumber).addClass('FMTrendMenuZoomOutClick').removeClass('FMTrendMenuZoomOut');
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	trendGraph.ClickCanvasJSMenuButton('Reset');

	$('#trendMenuPan' + drawingNumber).addClass('FMTrendMenuPan').removeClass('FMTrendMenuPanActive');
	$('#trendMenuZoom' + drawingNumber).addClass('FMTrendMenuZoomActive').removeClass('FMTrendMenuZoom');
}

FMTrendMenuBar.zoomOutButtonUp = function (e, drawingNumber) {
	$('#trendMenuZoomOut' + drawingNumber).addClass('FMTrendMenuZoomOut').removeClass('FMTrendMenuZoomOutClick');
}

FMTrendMenuBar.zoomButtonDown = function (e, drawingNumber) {
	$('#trendMenuZoom' + drawingNumber).addClass('FMTrendMenuZoomActive').removeClass('FMTrendMenuZoom');
	$('#trendMenuPan' + drawingNumber).addClass('FMTrendMenuPan').removeClass('FMTrendMenuPanActive');

	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	trendGraph.ClickCanvasJSMenuButton('Zoom');

}

FMTrendMenuBar.zoomButtonUp = function (e, drawingNumber) {
}

FMTrendMenuBar.exportButtonDown = function (event, self, drawingNumber)
{
	if (event.target !== self) return; // avoid click event bubling from the <a> that is a children
	$('#trendMenuExport' + drawingNumber).addClass('FMTrendMenuExportClick').removeClass('FMTrendMenuExport');

	function exportTableToCSV($table, filename) {
			 // Temporary delimiter characters unlikely to be typed by keyboard
			 // This is to avoid accidentally splitting the actual contents
		var tmpColDelim = String.fromCharCode(11) // vertical tab character
			 , tmpRowDelim = String.fromCharCode(0) // null character

			 // actual delimiter characters for CSV format
			 , colDelim = '","'
			 , rowDelim = '"\r\n"';

		var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
		var grid = trendGraph.GetTable();
		var header = parseHeader(grid.getColumns());
		var rows = grid.getData().map(grabRow);

		// Grab text from table into CSV formatted string
		var csv = '"';
		csv += formatRows(header);
		csv += rowDelim;
		csv += formatRows(rows) + '"';

		// Data URI
		var csvData = 'data:application/csv;charset=utf-8,' + encodeURIComponent(csv);

		// For IE (tested 10+)
		if (window.navigator.msSaveOrOpenBlob) {
			var blob = new Blob([decodeURIComponent(encodeURI(csv))], {
				type: "text/csv;charset=utf-8;"
			});
			navigator.msSaveBlob(blob, filename);
		} else
		{
			$(this).find( 'a' )
				 .attr({
				 	'download': filename
					  , 'href': csvData
				 });
			$(this).find('a')[0].click();

		}

		//------------------------------------------------------------
		// Helper Functions 
		//------------------------------------------------------------
		// Format the output so it has the appropriate delimiters
		function formatRows(rows)
		{
            return rows.join(tmpRowDelim)
                .split(tmpRowDelim).join(rowDelim)
                .split(tmpColDelim).join(colDelim);
		}

		// Grab and format header from the table
		function parseHeader(header)
		{
			FMTrendMenuBar.headerCols = header;
			var headerArray = [];
			headerArray.push($.map(header, function (object, index)
			{
				return object["name"].replace('"', '""');
			}).join(tmpColDelim));

			return headerArray;
		}

		//***********************************************************************
		// This function will grab and format a row from the table. It will
		// match up the columns for each Pen for the values and statuses.
		// This function is called by function exportTableToCSV() for each
		// row in the table.
		//***********************************************************************
		function grabRow( row, i )
		{
			var pens = trendGraph.GetTrend().Pens;
			var numPens = trendGraph.GetTrend().Pens.length;

			// Create an array of empty columns for each pen (has two columns) and the 
			// timestamp column.
			var columns = Array.apply(null, Array((numPens * 2) + 1)).map(function () { return "" });

			for (var property in row)
			{
				if (row.hasOwnProperty(property))
				{
					if (property === "TimeStamp")
					{
						columns[0] = row[property].replace('"', '""');
					}
					else
					{
						// The property is made up of 3 parts: Point ID, Tag ID, and value/status.
						var parts = property.split(".");

						if (parts != null && parts.length === 3)
						{
							var pointAndTagCombination = parts[0] + "." + parts[1];
							var columnIndex = FMTrendMenuBar.FindHeaderColumnIndex(pointAndTagCombination);

							var offsetIndex = 0;

							var penIndex = FMTrendMenuBar.FindPenIndex(pens, parts[0], parts[1]);
							var pen = pens[penIndex];

							if (parts[2] === "Value")
							{
								offsetIndex = 0;
							}
							else if (parts[2] === "Status")
							{
								offsetIndex = 1;
							}
							else
							{
								// There should always be a column index set for value or status.
								// If not return so that an index out of bounds error will not occur.
								return;
                            }

							if (row[property].indexOf('<') !== -1)
							{
								// if not numeric or format is ft-in-16th or ft-in-8th
								if (pen.ValueType !== "System.Double" || pen.Units === 27 || pen.Units === 19)
								{
									columns[columnIndex + offsetIndex] = row[property].replace(/<.*?>/g, "").replace(/ +/g, "");
								}
								else // numeric value
								{
									// Instead of getting the raw value from the formatted data in the cell and then reformat with '.' 
									// as decimal separator and no',' as group separator we can do it by manipulating the string
									var numberFormatInfo = FMOperateIndex.numformatInfo;

									var formatter = new Formatting.NumberFormatter(new Formatting.NumberFormatInfo(numberFormatInfo));
									var unformatNumericValue = row[property].replace(/<.*?>/g, "").replace(/ +/g, "");

									if (unformatNumericValue == "Bad")
									{
										columns[columnIndex + offsetIndex] = unformatNumericValue;
									}
									else
									{
										unformatNumericValue = (unformatNumericValue === "" ? "" : formatter.TryParse(unformatNumericValue, function (errorMessage) { }));

										// remove the ',' if is the group separator
										columns[columnIndex + offsetIndex] = unformatNumericValue;
									}
								}
							}
							else
							{
								columns[columnIndex + offsetIndex] = row[property].replace('"', '""');
							}
                        }
                    }
				}
			}

			return columns.join(tmpColDelim);
		}
	}

	// var outputFile = 'export'
	FMLayout.ConfirmSaveCancel( '<label>File Name:</label><div class="form-group"><input class="form-control exportFileName"></input></div>', "Export Trend Data", function( event )
	{
		// CSV
		var outputFile = $(".exportFileName").last().val();
		if (outputFile === "")
		{
			$(".exportFileName").last().parent().addClass("has-error");
			return false;
		}
		outputFile = outputFile.replace( '.csv', '' ) + '.csv';
		exportTableToCSV.apply(self, [$('#graphTable' + drawingNumber), outputFile]);
	}, null
	);

	$('#trendMenuExport' + drawingNumber).addClass('FMTrendMenuExport').removeClass('FMTrendMenuExportClick');
}

//**********************************************************
// This function will return a header column index that
// matches the property. If none is found, then zero is
// returned.
//**********************************************************
FMTrendMenuBar.FindHeaderColumnIndex = function (property)
{
	if (FMTrendMenuBar.headerCols == null || FMTrendMenuBar.headerCols.length === 0)
	{
		return 0;
	}

	for (var nextIndex = 0; nextIndex < FMTrendMenuBar.headerCols.length; nextIndex++)
	{
		var name = FMTrendMenuBar.headerCols[nextIndex].name;

		if (property === name)
		{
			return nextIndex;
		}
	}

	return 0;
}

//**********************************************************
// This function will return the pen index that
// matches the point ID and Tag ID. If none is found, 
// then zero is returned.
//**********************************************************
FMTrendMenuBar.FindPenIndex = function (penList, pointId, tagId)
{
	if (penList == null || penList.length === 0)
	{
		return 0;
	}

	for (var penIndex = 0; penIndex < penList.length; penIndex++)
	{
		var penPointId = penList[penIndex].PointID;
		var penTagId = penList[penIndex].TagID;

		if (pointId === penPointId && tagId === penTagId)
		{
			return penIndex;
		}
	}

	return 0;
}

FMTrendMenuBar.exportButtonUp = function (e, drawingNumber) {
	$('#trendMenuExport' + drawingNumber).addClass('FMTrendMenuExport').removeClass('FMTrendMenuExportClick');
}

FMTrendMenuBar.printButtonDown = function (e, drawingNumber) {
	$('#trendMenuPrint' + drawingNumber).addClass('FMTrendMenuPrintClick').removeClass('FMTrendMenuPrint');
}

FMTrendMenuBar.printButtonUp = function (e, drawingNumber) {
	$('#trendMenuPrint' + drawingNumber).addClass('FMTrendMenuPrint').removeClass('FMTrendMenuPrintClick');
}

FMTrendMenuBar.leftButtonDown = function (e, drawingNumber) {
	$('#trendMenuLeft' + drawingNumber).addClass('FMTrendMenuLeftClick').removeClass('FMTrendMenuLeft');

	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	trendGraph.PageLeft();
}

FMTrendMenuBar.leftButtonUp = function (e, drawingNumber) {
	$('#trendMenuLeft' + drawingNumber).addClass('FMTrendMenuLeft').removeClass('FMTrendMenuLeftClick');
}

FMTrendMenuBar.rightButtonDown = function (e, drawingNumber) {
	$('#trendMenuRight' + drawingNumber).addClass('FMTrendMenuRightClick').removeClass('FMTrendMenuRight');

	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	trendGraph.PageRight();
}

FMTrendMenuBar.rightButtonUp = function (e, drawingNumber) {
	$('#trendMenuRight' + drawingNumber).addClass('FMTrendMenuRight').removeClass('FMTrendMenuRightClick');
}

FMTrendMenuBar.playPauseButtonDown = function (e, drawingNumber) {
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	if ($('#trendMenuPlayPause' + drawingNumber).hasClass('FMTrendMenuPlay')){
		$('#trendMenuPlayPause' + drawingNumber).addClass('FMTrendMenuPlayClick').removeClass('FMTrendMenuPlay');
		$('#trendMenuZoom' + drawingNumber).addClass('FMTrendMenuZoom').removeClass('FMTrendMenuZoomActive');
		$('#trendMenuPan' + drawingNumber).addClass('FMTrendMenuPan').removeClass('FMTrendMenuPanActive');
		$('#trendMenuPan' + drawingNumber).prop("disabled", true);
		$('#trendMenuZoomOut' + drawingNumber).prop("disabled", true);
		$('#trendMenuZoom' + drawingNumber).prop("disabled", true);
        trendGraph.ClickCanvasJSMenuButton('Reset');
		trendGraph.ResumeTrend();
		$('#trendMenuPlayPause' + drawingNumber).prop('title', $('#TrendMenu-Pause').val());
	}
	else {
		$('#trendMenuPlayPause' + drawingNumber).addClass('FMTrendMenuPauseClick').removeClass('FMTrendMenuPause');
		$('#trendMenuPan' + drawingNumber).prop("disabled", false);
		$('#trendMenuZoomOut' + drawingNumber).prop("disabled", false);
		$('#trendMenuZoom' + drawingNumber).prop("disabled", false);
		trendGraph.PauseTrend();
		$('#trendMenuPlayPause' + drawingNumber).prop('title', $('#TrendMenu-Play').val());
		$('#trendMenuZoom' + drawingNumber).addClass('FMTrendMenuZoomActive').removeClass('FMTrendMenuZoom');
	}
}

FMTrendMenuBar.playPauseButtonUp = function (e, drawingNumber) {
	if ($('#trendMenuPlayPause' + drawingNumber).hasClass('FMTrendMenuPlayClick')) {
		$('#trendMenuPlayPause' + drawingNumber).addClass('FMTrendMenuPause').removeClass('FMTrendMenuPlayClick');
	}
	else {
		$('#trendMenuPlayPause' + drawingNumber).addClass('FMTrendMenuPlay').removeClass('FMTrendMenuPauseClick');
	}
}


FMTrendMenuBar.refreshButtonDown = function (e, drawingNumber) {
	$('#trendMenuRefresh' + drawingNumber).addClass('FMTrendMenuRefreshClick').removeClass('FMTrendMenuRefresh');

	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	var trend = trendGraph.GetTrend();
	if (!trend) {
		return;
	}

	var periodType = parseInt($('#trendMenuPeriodType' + drawingNumber).val());

	if (isNaN(periodType)) {
		return;
	}


	trend.Mode = ($('#trendMenuTimeRange' + drawingNumber).is(':checked')) ? 1 : 0;
	trend.PeriodType = periodType;

	var period;
	if (trend.Mode === 1)
	{
		trend.Start = $( '#trendMenuStartDate' + drawingNumber ).datetimepicker( 'getDate' );
		trend.End = $( '#trendMenuEndDate' + drawingNumber ).datetimepicker( 'getDate' );

		 var timezoneOffsetStr = $("#TimezoneOffsetString").val();
		 var timezoneOffset = parseInt(timezoneOffsetStr);

		 var startMomentTime = moment(trend.Start); // ! The translation is intentionally backwards here. The datepicker library expects client time. !
		 startMomentTime = startMomentTime.subtract(timezoneOffset, 'minutes'); //go to UTC time
		 startMomentTime = startMomentTime.add(startMomentTime.utcOffset(), 'minutes'); //go to client time
		 trend.Start = startMomentTime.toDate();

		 var endMomentTime = moment(trend.End);
		 endMomentTime = endMomentTime.subtract(timezoneOffset, 'minutes'); //go to UTC time
		 endMomentTime = endMomentTime.add(endMomentTime.utcOffset(), 'minutes'); //go to client time
		 trend.End = endMomentTime.toDate();



		if ( periodType === 0 )
		{
			period = (trend.End.getTime() - trend.Start.getTime()) / 60000;
		}
		else if ( periodType === 1 )
		{
			period = (trend.End.getTime() - trend.Start.getTime()) / 3600000;
		}
		else
		{
			period = (trend.End.getTime() - trend.Start.getTime()) / 86400000;
		}

		$( '#trendMenuPeriod' + drawingNumber ).val( period );
	}
	else
	{
		period = parseFloat($('#trendMenuPeriod' + drawingNumber).val());
		if (isNaN(period)) {
			return;
		}
	}

	trend.Period = period;

	if ( $( '#ModifyTrendRight' ).val() == 'True' )
	{
		if ( !FMTrendIndex.saveTrend( trend ) )
			return;
	}

	var FMTrendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!FMTrendGraph)
		return;

	FMTrendGraph.ReloadTrend(trend);

	 FMTrendMenuBar.setStartAndEnd(drawingNumber);


	$('#trendMenuPlayPause' + drawingNumber).prop("disabled", (trend.Mode === 1) ? true : false);
	$('#trendMenuPan' + drawingNumber).prop("disabled", (trend.Mode === 0) ? true : false);
	$('#trendMenuZoomOut' + drawingNumber).prop("disabled", (trend.Mode === 0) ? true : false);
	$('#trendMenuZoom' + drawingNumber).prop("disabled", (trend.Mode === 0) ? true : false);

	$('#trendMenuPlayPause' + drawingNumber).addClass('FMTrendMenuPause').removeClass('FMTrendMenuPlay');
	$('#trendMenuPan' + drawingNumber).addClass('FMTrendMenuPan').removeClass('FMTrendMenuPanActive');
	if (trend.Mode === 1) {
		$('#trendMenuZoom' + drawingNumber).addClass('FMTrendMenuZoomActive').removeClass('FMTrendMenuZoom');
	}
	else {
		$('#trendMenuZoom' + drawingNumber).addClass('FMTrendMenuZoom').removeClass('FMTrendMenuZoomActive');
	}

	FMTrendMenuBar.selectTrendZoomTypeOnChange(null, drawingNumber);
}

FMTrendMenuBar.refreshButtonUp = function (e, drawingNumber) {
	$('#trendMenuRefresh' + drawingNumber).addClass('FMTrendMenuRefresh').removeClass('FMTrendMenuRefreshClick');
}


FMTrendMenuBar.setTimeControls = function (drawingNumber) {

	var mode = ($('#trendMenuTimeRange' + drawingNumber).is(':checked')) ? 1 : 0;

	var state = (mode === 1) ? 'enable' : 'disable';

	$('#trendMenuStartDate' + drawingNumber).datetimepicker(state);
	$('#trendMenuEndDate' + drawingNumber).datetimepicker(state);


	if (mode === 1) {
		$('#trendMenuInTheLastLabel' + drawingNumber).addClass('FMTrendMenuRadioLabelDisabled').removeClass('FMTrendMenuRadioLabelEnabled');
		$('#trendMenuTimeRangeLabel' + drawingNumber).addClass('FMTrendMenuRadioLabelEnabled').removeClass('FMTrendMenuRadioLabelDisabled');
		$('#trendMenuStartDateLabel' + drawingNumber).addClass('FMTrendMenuDateLabelEnabled').removeClass('FMTrendMenuDateLabelDisabled');
		$('#trendMenuEndDateLabel' + drawingNumber).addClass('FMTrendMenuDateLabelEnabled').removeClass('FMTrendMenuDateLabelDisabled');
	}
	else {
		$('#trendMenuInTheLastLabel' + drawingNumber).addClass('FMTrendMenuRadioLabelEnabled').removeClass('FMTrendMenuRadioLabelDisabled');
		$('#trendMenuTimeRangeLabel' + drawingNumber).addClass('FMTrendMenuRadioLabelDisabled').removeClass('FMTrendMenuRadioLabelEnabled');
		$('#trendMenuStartDateLabel' + drawingNumber).addClass('FMTrendMenuDateLabelDisabled').removeClass('FMTrendMenuDateLabelEnabled');
		$('#trendMenuEndDateLabel' + drawingNumber).addClass('FMTrendMenuDateLabelDisabled').removeClass('FMTrendMenuDateLabelEnabled');
	}
}

FMTrendMenuBar.timeOptionsOnChange = function (e, drawingNumber) {

	var mode = ($('#trendMenuTimeRange' + drawingNumber).is(':checked')) ? 1 : 0;

	FMTrendMenuBar.setTimeControls(drawingNumber);

	if (mode === 0) {
		FMTrendMenuBar.disableDateTimePickerEvent = true;
		FMTrendMenuBar.periodInputOnInput(e, drawingNumber);
		FMTrendMenuBar.disableDateTimePickerEvent = false;
	}
}

FMTrendMenuBar.checkPeriod = function (period, periodType, drawingNumber) {

	var returnedPeriod = null;


	if (periodType === 0) {
		if (period > 3679200.00) {
			returnedPeriod = 3679200.00;
			FMLayout.Alert($('#TrendMenu-MaximumMinutePeriodExceeded').val(), $('#TrendMenu-MaximumPeriodExceeded').val(), null);
		}
	}
	else if (periodType === 1) {
		if (period > 61320) {
			returnedPeriod = 61320;
			FMLayout.Alert($('#TrendMenu-MaximumHourPeriodExceeded').val(), $('#TrendMenu-MaximumPeriodExceeded').val(), null);
		}
	}
	else {
		if (period > 2555) {
			returnedPeriod = 2555;
			FMLayout.Alert($('#TrendMenu-MaximumDayPeriodExceeded').val(), $('#TrendMenu-MaximumPeriodExceeded').val(), null);
		}
	}

	if (period < 1) {
		returnedPeriod = 1;
		FMLayout.Alert($('#TrendMenu-PeriodLessThanMinimum').val(), $('#TrendMenu-MaximumPeriodExceeded').val(), null);
	}

	return returnedPeriod;
}


FMTrendMenuBar.periodInputOnInput = function (e, drawingNumber) {

	if (FMTrendMenuBar.disableDateTimePickerEvent) {
		return;
	}

	var period = parseFloat($('#trendMenuPeriod' + drawingNumber).val());
	if(isNaN(period))	{
		return;
	}

	var periodType = parseInt($('#trendMenuPeriodType' + drawingNumber).val());

	if (isNaN(periodType)) {
		return;
	}

	var returnedPeriod = FMTrendMenuBar.checkPeriod(period, periodType, drawingNumber);
	if (returnedPeriod != null) {
		period = returnedPeriod;
	}

	var timezoneOffsetStr = $("#TimezoneOffsetString").val();
	var timezoneOffset = parseInt(timezoneOffsetStr);

	var mode = ($('#trendMenuTimeRange' + drawingNumber).is(':checked')) ? 1 : 0;
	var end = new Date();
	var endMomentTime = FMOperateIndex.translateClientDateTimeToSiteMomentTime(end);
	FMTrendMenuBar.disableDateTimePickerEvent = true;
	$('#trendMenuEndDate' + drawingNumber).datetimepicker("setDate", endMomentTime.toDate());
	FMTrendMenuBar.disableDateTimePickerEvent = false;


	var start;
	if (periodType === 0) {
		start = new Date(end.getTime() - period * 60000);
	}
	else if (periodType === 1) {
		start = new Date(end.getTime() - period * 3600000);
	}
	else {
		start = new Date(end.getTime() - period * 86400000);
	}
	var startMomentTime = FMOperateIndex.translateClientDateTimeToSiteMomentTime(start);
	FMTrendMenuBar.disableDateTimePickerEvent = true;
	$('#trendMenuStartDate' + drawingNumber).datetimepicker("setDate", startMomentTime.toDate());
	$('#trendMenuPeriod' + drawingNumber).val(period)
	FMTrendMenuBar.disableDateTimePickerEvent = false;
}


FMTrendMenuBar.startInputOnChange = function (e, drawingNumber) {
	if (FMTrendMenuBar.disableDateTimePickerEvent) {
		return;
	}

	var periodType = parseInt($('#trendMenuPeriodType' + drawingNumber).val());
	if (isNaN(periodType)) {
		return;
	 }

	 var minPeriod;
	 if (periodType === 0) {
		  minPeriod = 60000;
	 }
	 else if (periodType === 1) {
		  minPeriod = 3600000;
	 }
	 else {
		  minPeriod = 86400000;
	 }

	 var start;
	 if ($('#trendMenuStartDate' + drawingNumber).datetimepicker('getDate') > FMOperateIndex.translateClientDateTimeToSiteMomentTime(moment().subtract(minPeriod,'ms').toDate())) {
		  FMTrendMenuBar.disableDateTimePickerEvent = true;
		  $('#trendMenuStartDate' + drawingNumber).datetimepicker("setDate", FMOperateIndex.translateClientDateTimeToSiteMomentTime(moment().subtract(minPeriod, 'ms')).toDate());
		  FMTrendMenuBar.disableDateTimePickerEvent = false;
		  start = FMTrendIndex.convertLocalDateToUTCDate(FMOperateIndex.translateClientDateTimeToSiteMomentTime(moment().subtract(minPeriod, 'ms')).toDate());
	 }
	 else {
		  start = FMTrendIndex.convertLocalDateToUTCDate($('#trendMenuStartDate' + drawingNumber).datetimepicker('getDate'));
	 }
	end = FMTrendIndex.convertLocalDateToUTCDate($('#trendMenuEndDate' + drawingNumber).datetimepicker('getDate'));
	var period;


	if (periodType === 0) {
		period = (end.getTime() - start.getTime()) / 60000;
	}
	else if (periodType === 1) {
		period = (end.getTime() - start.getTime()) / 3600000;
	}
	else {
		period = (end.getTime() - start.getTime()) / 86400000;
	}

	var returnedPeriod = FMTrendMenuBar.checkPeriod(period, periodType, drawingNumber);
	if (returnedPeriod != null) {
		period = returnedPeriod;
	}

	var end;
	if (periodType === 0) {
		end = new Date(start.getTime() + period * 60000);
	}
	else if (periodType === 1) {
		end = new Date(start.getTime() + period * 3600000);
	}
	else {
		end = new Date(start.getTime() + period * 86400000);
	 }

	FMTrendMenuBar.disableDateTimePickerEvent = true;
	$('#trendMenuEndDate' + drawingNumber).datetimepicker("setDate", FMTrendIndex.convertUTCDateToLocalDate(end));
	$('#trendMenuPeriod' + drawingNumber).val(period)
	FMTrendMenuBar.disableDateTimePickerEvent = false;
}

FMTrendMenuBar.endInputOnChange = function (e, drawingNumber) {

	if (FMTrendMenuBar.disableDateTimePickerEvent) {
		return;
	}

	var periodType = parseInt($('#trendMenuPeriodType' + drawingNumber).val());
	if (isNaN(periodType)) {
		return;
	}


	 start = FMTrendIndex.convertLocalDateToUTCDate($('#trendMenuStartDate' + drawingNumber).datetimepicker('getDate'));
	 if ($('#trendMenuEndDate' + drawingNumber).datetimepicker('getDate') > FMOperateIndex.translateClientDateTimeToSiteMomentTime(moment()).toDate()) {
		  FMTrendMenuBar.disableDateTimePickerEvent = true;
		  $('#trendMenuEndDate' + drawingNumber).datetimepicker("setDate", FMOperateIndex.translateClientDateTimeToSiteMomentTime(moment()).toDate());
		  FMTrendMenuBar.disableDateTimePickerEvent = false;
		  end = FMTrendIndex.convertLocalDateToUTCDate(FMOperateIndex.translateClientDateTimeToSiteMomentTime(moment()).toDate());

	 }
	 else {
		  end = FMTrendIndex.convertLocalDateToUTCDate($('#trendMenuEndDate' + drawingNumber).datetimepicker('getDate'));
	 }
	var period;


	if (periodType === 0) {
		period = (end.getTime() - start.getTime()) / 60000;
	}
	else if (periodType === 1) {
		period = (end.getTime() - start.getTime()) / 3600000;
	}
	else {
		period = (end.getTime() - start.getTime()) / 86400000;
	}

	var returnedPeriod = FMTrendMenuBar.checkPeriod(period, periodType, drawingNumber);
	if (returnedPeriod != null) {
		period = returnedPeriod;
	}

	var start;
	if (periodType === 0) {
		start = new Date(end.getTime() - period * 60000);
	}
	else if (periodType === 1) {
		start = new Date(end.getTime() - period * 3600000);
	}
	else {
		start = new Date(end.getTime() - period * 86400000);
	}

	FMTrendMenuBar.disableDateTimePickerEvent = true;
	$('#trendMenuStartDate' + drawingNumber).datetimepicker("setDate", FMTrendIndex.convertUTCDateToLocalDate(start));
	$('#trendMenuPeriod' + drawingNumber).val(period)
	FMTrendMenuBar.disableDateTimePickerEvent = false;
}


// The periodTypeSelectOnClick prevents an unusual result of the periodInput
// receving focus when periodTypeSelect onfocus if the input is prior to the select
// and the two are bounded by input type radio as in this layout.
FMTrendMenuBar.periodTypeSelectOnClick = function (e, drawingNumber) {
	event.preventDefault();
}

// Period Type Changed, update the Period accordingly
FMTrendMenuBar.periodTypeSelectOnChange = function (e, drawingNumber) {

	var periodTypeSelect = $('#trendMenuPeriodType' + drawingNumber);

	var priorPeriodType = parseInt(periodTypeSelect.data('previous'));
	var periodType = parseInt(periodTypeSelect.val());
	periodTypeSelect.data('previous', periodType);

	var periodInMinutes = parseFloat($('#trendMenuPeriod' + drawingNumber).val());
	if (isNaN(periodInMinutes)) {
		return;
	}


	if (priorPeriodType === 1) {
		periodInMinutes *= 60;
	}
	else if(priorPeriodType === 2){
		periodInMinutes *= 1440;
	}

	var period;

	if (periodType === 0) {
		period = periodInMinutes;
	}
	else if (periodType === 1) {
		period = periodInMinutes / 60;
	}
	else if (periodType === 2) {
		period = periodInMinutes / 1440;
	}

	$('#trendMenuPeriod' + drawingNumber).val(period);
}


FMTrendMenuBar.selectLegendClickFunctionOnChange = function (e, drawingNumber) {
    var hideUnhideOn = (e.value === "0");
    var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
    trendGraph.SetLegendClickFunction(hideUnhideOn);
}


FMTrendMenuBar.toggleAutoSecondaryYScaling = function (e, drawingNumber) {
    var autoScaling = e.currentTarget.checked;
    var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
    trendGraph.SetAutoSecondaryYScaling(autoScaling);
}


FMTrendMenuBar.selectTrendZoomTypeOnChange = function (e, drawingNumber) {
	var zoomType = $('#selectTrendZoomType' + drawingNumber).val();
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph) {
		return;
	}
	trendGraph.SetZoomType(zoomType);
}


FMTrendMenuBar.getTrendPlayPauseState = function (drawingNumber) {

	return ($('#trendMenuPlayPause' + drawingNumber).hasClass('FMTrendMenuPlay'));
}

FMTrendMenuBar.setStartAndEnd = function (drawingNumber){
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	var trend = trendGraph.GetTrend();
	if (!trend) {
		return;
	}

	 var timezoneOffsetStr = $("#TimezoneOffsetString").val();
	 var timezoneOffset = parseInt(timezoneOffsetStr);
	 var startMomentTime = FMOperateIndex.translateClientDateTimeToSiteMomentTime(trend.Start);
	 var endMomentTime = FMOperateIndex.translateClientDateTimeToSiteMomentTime(trend.End);

	FMTrendMenuBar.disableDateTimePickerEvent = true;
	 $('#trendMenuStartDate' + drawingNumber).datetimepicker("setDate", startMomentTime.toDate());
	 $('#trendMenuEndDate' + drawingNumber).datetimepicker("setDate", endMomentTime.toDate());
	FMTrendMenuBar.disableDateTimePickerEvent = false;
}

FMTrendMenuBar.setDescription = function (drawingNumber, description) {
	$('#trendMenuDescription' + drawingNumber).html(description);
};



// Trend Legend object
var FMTrendLegend = {
	projectOnYAxisImgIdPrefix: "ProjectOnYAxisImg_",
	penValueInputIdPrefix: "PenValueInput_",
	penStatusLabelIdPrefix: "PenStatusLabel_",
	penAlarmStateLabelIdPrefix: "PenAlarmStateLabel_",
	penMinValueInputIdPrefix: "PenMinValueInput_",
	penMaxValueInputIdPrefix: "PenMaxValueInput_",
	trendPalette:	[['#000000', '#FF0000', '#00FF00', '#FFFF00', '#0000FF', '#FF00FF', '#00FFFF', '#FFFFFF'],
						['#2D2D2D', '#D70000', '#00D700', '#D7D700', '#0000D7', '#D700D7', '#00D7D7', '#DCDCDC'],
						['#555555', '#AF0000', '#00AF00', '#AFAF00', '#0000AF', '#AF00AF', '#00AFAF', '#B9B9B9'],
						['#737373', '#640000', '#006400', '#646400', '#000064', '#640064', '#006464', '#969696'],
						['#FFC000', '#FF8200', '#FF5A2D', '#66400F', '#805236', '#EFEFC9', '#037392', '#B7E8FF'],
						['#0080FF', '#8080FF', '#FF80C0', '#800040', '#008080', '#004080', '#FF0080', '#000040'],
						['#0080C0', '#0000A0', '#8080C0', '#800080', '#008040', '#000080', '#8000FF', '#009FC5'],
						['#423A22', '#A28D68', '#AFAF61', '#5C5230', '#004000', '#599764', '#6F8938', '#35462B']],

	trendDefaultColor: [1, 2, 3, 4, 5, 6, 32, 33, 39, 40, 41, 55, 56, 57, 61, 62],
	DrawingNumber: null,
	PenIndex: null
};


FMTrendLegend.getPenColor = function (trend) {
	var color = null;
	for (var defaultColorIndex = 0; defaultColorIndex < FMTrendLegend.trendDefaultColor.length; defaultColorIndex++)
	{
		var colorIndex = FMTrendLegend.trendDefaultColor[defaultColorIndex];
		var color = FMTrendLegend.trendPalette[parseInt(colorIndex / 8)][colorIndex % 8];
		var colorInUse = false;
		for (var penIndex = 0; penIndex < trend.Pens.length; penIndex++) {
			if (trend.Pens[penIndex].PenColor == color) {
				colorInUse = true;
				break;
			}
		}

		if (!colorInUse) {
			break;
		}
	}
	return color;
}

//Initialise the legend
FMTrendLegend.initLegend = function (drawingNumber, trend) {
	var addNewPenImgSrc = '../../FMWebApp/images/Add-icon.png';
	var ModifyRightStyle = "";
	if ($('#ModifyTrendRight').val() == 'False')
	{
		ModifyRightStyle = "display: none";
	}

	var legendHTMLString = '<div id="graphLegend' + drawingNumber + '" class="FMTrendLegend"> \n';
	legendHTMLString += '<div id="legendSectionDivider' + drawingNumber + '" class="FMTrendLegendSectionDivider"> \n' +
	'<div class="col-xs-12"> \n' +
	'<div class="col-xs-8 FMTrendLegendSectionDividerHeader">' + $('#TrendLegend-Legend').val() + '</div> \n' +
	'<div class="col-xs-4 FMTrendLegendSectionDividerEntry" style="' + ModifyRightStyle + '"> <img class="FMTrendLegendSectionDividerEntryPos" src=' + addNewPenImgSrc + ' onclick="FMTrendLegend.OnAddPenClick(this, ' + drawingNumber + ')" /> <a onclick="FMTrendLegend.OnAddPenClick(this,' + drawingNumber + ');"> ' + $('#TrendLegend-AddPen').val() + '</a> </div> \n' +
	'</div> \n' +
	'</div> \n';

	legendHTMLString += '<div id="TrendLegendHeader' + drawingNumber + '" class="col-sm-12 FMTrendLegendHeaderRow"> \n' +
	'<div class="col-md-1 text-center">' + $('#TrendLegend-Visibility').val() + '</div> \n' +
	'<div class="col-md-1 text-center">' + $('#TrendLegend-ProjectOnAxis').val() + '</div> \n' +
	'<div class="col-md-1 text-center">' + $('#TrendLegend-Colour').val() + '</div> \n' +
   '<div class="col-md-2 text-center">' + $('#TrendLegend-PenName').val() + '</div> \n' +
	'<div class="col-md-1 text-center">' + $('#TrendLegend-Value').val() + '</div> \n' +
	'<div class="col-md-1 text-center">' + $('#TrendLegend-Status').val() + '</div> \n' +
	'<div class="col-md-1 text-center">' + $('#TrendLegend-AlarmState').val() + '</div> \n' +
	'<div class="col-md-1 text-center">' + $('#TrendLegend-Units').val() + '</div> \n' +
	'<div class="col-md-1 text-center">' + $('#TrendLegend-Minimum').val() + '</div> \n' +
	'<div class="col-md-1 text-center">' + $('#TrendLegend-Maximum').val() + '</div> \n' +
	'<div class="col-md-1 text-center">' + $('#TrendLegend-DelPen').val() + '</div> \n' +
	'</div> \n';

	legendHTMLString += '<div class="FMTrendLegendRowsContainer" id="TrendLegendRowsContainer' + drawingNumber + '"><div class="FMTrendLegendRows" id="TrendLegendRows' + drawingNumber + '"> \n';
	for (var i = 0; i < trend.Pens.length; i++)
		legendHTMLString += FMTrendLegend.LegendListItem(drawingNumber, trend, i);
	legendHTMLString += '</div> \n</div> \n';

	var legend = $('#graphLegend' + drawingNumber);
	legend.replaceWith(legendHTMLString);
	FMTrendLegend.setControlMasks(drawingNumber, trend);

	$('#TrendLegendRows' + drawingNumber).css({ "overflow": "hidden" });
	$('#TrendLegendRowsContainer' + drawingNumber).css({ "overflow-x": "hidden" });

	$('#TrendLegendRowsContainer' + drawingNumber).niceScroll('#TrendLegendRows' + drawingNumber, {
		autohidemode: false, // Do not hide scrollbar when mouse out
		background: '#E5E9E7', // The scrollbar rail color
		cursorwidth: '10px', // Scroll cursor width
		cursorcolor: '#999999', // Scroll cursor color
		horizrailenabled: false,
		railoffset: true,
		railpadding: { top: 0, right: 0, left: 10, bottom: 0 },
		smoothscroll: true
	}); 

}


//Draw a row in the legend for a given pen
FMTrendLegend.LegendListItem = function (drawingNumber, trend, penIndex) {
	var trendPen = trend.Pens[penIndex];
	var penName = trendPen.PointID + "." + trendPen.TagID;
	var penId = penIndex;
	var ModifyRightStyle = "";
	var ColorPickerDisabled = "false";
	if ($('#ModifyTrendRight').val() == 'False') {
		ModifyRightStyle = "display: none";
		ColorPickerDisabled = "true";
	}
	if (trendPen.visible == null)
		trendPen.visible = true;
	if (trendPen.isProjectedOnYAxis == null)
		trendPen.isProjectedOnYAxis = false;

	var visibleImgId = 'VisibleImg_' + drawingNumber + '_' + penIndex;
	var visibleImgSrc = '../../FMWebApp/images/' + ((trendPen.visible) ? 'Visible-on-icon-without-BG.png' : 'Visible-off-icon-without-BG.png');
	var projectOnYAxisImgId = FMTrendLegend.projectOnYAxisImgIdPrefix + drawingNumber + '_' + penIndex;
	var projectOnYAxisImgSrc = '../../FMWebApp/images/' + ((trendPen.isProjectedOnYAxis) ? 'Toggle-Button-ON.png' : 'Toggle-Button-OFF.png');
	var penColorInputId = 'PenColorInput_' + drawingNumber + '_' + penIndex;
	var penValueInputId = 'PenValueInput_' + drawingNumber + '_' + penIndex;
	var penMinValueInputId = FMTrendLegend.penMinValueInputIdPrefix + drawingNumber + '_' + penIndex;
	var penMaxValueInputId = FMTrendLegend.penMaxValueInputIdPrefix + drawingNumber + '_' + penIndex;
	var numFormatInfoString = $('#NumberFormatInfoString').val();
	var numformatInfo = JSON.parse(numFormatInfoString);
	numformatInfo.NumberDecimalDigits = trendPen.DecimalPlaces;
	var penStatusLabelId = FMTrendLegend.penStatusLabelIdPrefix + drawingNumber + '_' + penIndex;
	var penAlarmStateLabelId = FMTrendLegend.penAlarmStateLabelIdPrefix + drawingNumber + '_' + penIndex;
	var penMinValue = '';
	if (trend.Pens[penIndex].Minimum != null)
		penMinValue = FMFormatValues.FormatValue(trendPen.Units, numformatInfo, trend.Pens[penIndex].Minimum);
	var penMaxValue = '';
	if (trend.Pens[penIndex].Maximum != null)
		penMaxValue = FMFormatValues.FormatValue(trendPen.Units, numformatInfo, trend.Pens[penIndex].Maximum);
	var deleteImgId = 'DeleteImg_' + drawingNumber + '_' + penIndex;
	var deleteImgSrc = '../../FMWebApp/images/Trash-icon.png';
	
	var listItem = '<div class="legend-li-wrap col-xs-12 FMTrendLegendRow" id="legendRow_' + drawingNumber + '_' + penIndex +'">'
			+ '<div class="col-md-1 text-center"><img class="FMTrendLegendCol1Pos FMTrendLegendVisibility" src=' + visibleImgSrc + ' onclick="FMTrendLegend.OnTrendPenVisibleImgClick(this, ' + drawingNumber + ',' + penIndex + ')" name="' + penIndex + '" id="' + visibleImgId + '"/></div>'
			+ '<div class="col-md-1 text-center"><img class="FMTrendLegendCol2Pos FMTrendLegendProjectOnYAxis" src=' + projectOnYAxisImgSrc + ' onclick="FMTrendLegend.OnTrendPenProjectOnYAxisImgClick(this, ' + drawingNumber + ',' + penIndex + ')" name="' + penIndex + '" id="' + projectOnYAxisImgId + '"/></div>'
			+ '<div class="col-md-1 text-center"><input type="text" id="' + penColorInputId + '" class="FMTrendLegendCol3Pos" value="' + trendPen.PenColor + '" /></div>'
			+ '<script>'
			+ '	$("#' + penColorInputId + '").spectrum({'
			+ '		preferredFormat: "hex",'
			+ '		showInput: "true",'
			+ '		color: "' + trendPen.PenColor + '",'
			+ '		showPalette: "true",'
			+ '		palette: FMTrendLegend.trendPalette,'
			+ '		hide: FMTrendLegend.OnPenColorPickerHide,'
			+ '		disabled: ' + ColorPickerDisabled + ''
			+ '	});'
			+ '</script>'
			+ '<div class="col-md-2 text-center"><span class="FMTrendLegendCol4Pos FMTrendLegendEntryLabel" onclick="FMTrendLegend.OnPenTagClick(this, ' + drawingNumber + ',' + penIndex + ')">' + penName + '</span></div>'
			+ '<div class="col-md-1 text-center FMTrendLegendInputCell"><input class="form-control input-md text-left FMTrendLegendCol5Pos FMTrendLegendNumericTextBox" id="' + penValueInputId + '" type="text" disabled value=""/></div>'
			+ '<div class="col-md-1 text-center"><label class="FMTrendLegendCol6Pos FMTrendLegendEntryLabel" id="' + penStatusLabelId + '"/></div>'
			+ '<div class="col-md-1 text-center"><label class="FMTrendLegendCol7Pos FMTrendLegendEntryLabel" id="' + penAlarmStateLabelId + '"/></div>'
			+ '<div class="col-md-1 text-center"><span class="FMTrendLegendCol8Pos FMTrendLegendEntryLabel">' + trendPen.UnitString + '</span></div>'
			+ '<div class="col-md-1 text-center FMTrendLegendInputCell"><input class="form-control input-md text-left FMTrendLegendCol9Pos FMTrendLegendNumericInputTextBox" maxlength="10" onfocus="FMTrendLegend.OnTrendPenMinMaxTextBoxFocus(this, ' + drawingNumber + ',' + penIndex + ')" onblur="FMTrendLegend.OnTrendPenMinMaxTextBoxBlur(this, ' + drawingNumber + ',' + penIndex + ')" id="' + penMinValueInputId + '" type="text" value="' + penMinValue + '"/></div>'
			+ '<div class="col-md-1 text-center FMTrendLegendInputCell"><input class="form-control input-md text-left FMTrendLegendCol10Pos FMTrendLegendNumericInputTextBox" maxlength="10" onfocus="FMTrendLegend.OnTrendPenMinMaxTextBoxFocus(this, ' + drawingNumber + ',' + penIndex + ')" onblur="FMTrendLegend.OnTrendPenMinMaxTextBoxBlur(this, ' + drawingNumber + ',' + penIndex + ')" id="' + penMaxValueInputId + '" type="text" value="' + penMaxValue + '"/></div>'
			+ '<div class="col-md-1 text-center" style="' + ModifyRightStyle + '"><img class="FMTrendLegendCol11Pos" src=' + deleteImgSrc + ' onclick="FMTrendLegend.OnTrendPenDeleteImgClick(this, ' + drawingNumber + ',' + penIndex + ')" name="' + penIndex + '" id="' + deleteImgId + '"/></div>'

			+ '</div>';
	
	return listItem;
};



//Responds to a visibility toggle request on a trend pen.
FMTrendLegend.OnTrendPenVisibleImgClick = function (img, drawingNumber, penIndex) {
	var penIndex = Number(img.name);
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;
	var trend = trendGraph.GetTrend();
	if (trend.Pens[penIndex].HasAccess == false)
	{
		return;
	}
	trendGraph.ToggleSeriesVisibility(penIndex);
	if (img.src.match(/-on-/i)) {
		img.src = '../../FMWebApp/images/Visible-off-icon-without-BG.png';
	}
	else {
		img.src = '../../FMWebApp/images/Visible-on-icon-without-BG.png';
	}
};


//Respond to a pen projection onto the secondary Y-Axis request
FMTrendLegend.OnTrendPenProjectOnYAxisImgClick = function (img, drawingNumber, penIndex) {
	var penIndex = Number(img.name);
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;
	trendGraph.ToggleYAxisForSeries(penIndex);
	if (img.src.match(/Toggle-Button-ON.png/i)) {
	    img.src = '../../FMWebApp/images/Toggle-Button-OFF.png';
	}
	else {
		FMTrendLegend.ResetProjectOnYAxisIcons(drawingNumber);
		img.src = '../../FMWebApp/images/Toggle-Button-ON.png';
	}
};

//Respond to pen color selection
FMTrendLegend.OnPenColorPickerHide = function (color) {
	var colorString = color.toHexString().toUpperCase();
	var idParsed = this.id.split("_");
	drawingNumber = idParsed[1];
	penIndex = idParsed[2];

	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	trendGraph.SetLineColor(penIndex, colorString);
};


//Respond to a pen tag change request
FMTrendLegend.OnPenTagClick = function (span, drawingNumber, penIndex) {
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph || $('#ModifyTrendRight').val() == 'False')
		return;

	TagSelection.TagSelectionOKCallBackFunction = FMTrendLegend.EditPenPointTagCallSuccess;
	TagSelection.TagSelectionSaveCallBackFunction = FMTrendLegend.SaveLastTagSelectionModel;
	FMTrendLegend.DrawingNumber = drawingNumber;
	FMTrendLegend.PenIndex = penIndex;

	var trend = trendGraph.GetTrend();
	var trendPen = trend.Pens[penIndex];
	var pointValueIdentifier = { IdentityGuid: trendPen.PointTagGuid, PointValueType: 0, PropertyID: null };

	FMTrendLegend.InvokeTagSelection(trendGraph.GetSettings(), trendPen.PointID, trendPen.PointGuid, trendPen.TagID, pointValueIdentifier);

}

//Reset all the ProjectOnYAxis icons for a given Trend
FMTrendLegend.ResetProjectOnYAxisIcons = function (drawingNumber) {
	$('.FMTrendLegendProjectOnYAxis').each(function (index) {
		var img = $(this);
		var imgId = img[0].id;
		if (imgId.length > FMTrendLegend.projectOnYAxisImgIdPrefix.length) {
				var penIndexDelimiterPos = imgId.indexOf("_", FMTrendLegend.projectOnYAxisImgIdPrefix.length);
				if (penIndexDelimiterPos) {
					var imgDrawingNumber = imgId.substr(FMTrendLegend.projectOnYAxisImgIdPrefix.length,(penIndexDelimiterPos - FMTrendLegend.projectOnYAxisImgIdPrefix.length));
					imgDrawingNumber = Number(imgDrawingNumber);
				}
				if (imgDrawingNumber === drawingNumber)
				    img.attr("src", "../../FMWebApp/images/Toggle-Button-OFF.png");
		}
	});

};


//Add a new pen
FMTrendLegend.OnAddPenClick = function (e, drawingNumber) {

	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	var trend = trendGraph.GetTrend();

	if (trend.Pens.length >= 16) {
		FMLayout.Alert($('#TrendLegend-MaxPens').val(), $('#TrendLegend-AddPen').val(), null);
		return;
	}

	TagSelection.TagSelectionOKCallBackFunction = FMTrendLegend.AddPenPointTagCallSuccess;
	TagSelection.TagSelectionSaveCallBackFunction = FMTrendLegend.SaveLastTagSelectionModel;
	FMTrendLegend.DrawingNumber = drawingNumber;


	FMTrendLegend.InvokeTagSelection(trendGraph.GetSettings(), null, null, null, null);
}


FMTrendLegend.InvokeTagSelection = function(settings, pointId, pointGuid, valueId, pointValueIdentifier){

	// hide any other notification
	FMErrorAndExceptionHandling.CloseNotifications();

	$('body').modalmanager('loading');

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	$.ajax({
		type: 'POST',
		url: $('#urlTagSelectionGetPointListEx').val(),
		cache: false,
		headers: headers,
		dataType: 'json',
		contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
		traditional: false,
		data: {
			showValueTypes: false,
			showTags: true,
			showFields: false,
			allowMultiple: false,
			allowPoint: (settings.pointTrend) ? false : true,
			pointId: pointId,
			pointGuidStr: (settings.pointTrend) ? settings.guid : pointGuid,
			valueId: valueId,
			pointValueIdentifier: pointValueIdentifier,
			applyPointAccess: true
		},
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					// replace the holder with the partial view
					$('#PointSelection').html(data);
					$('#PointTagSelectScreen').modal('show');
				}
				else {
					// remove the loading of the modal
					var modalManager = $('body').data('modalmanager');
					modalManager.removeLoading();
				}
			});
		},
		error: function (xhr, textStatus, error) {
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () {
					// remove the loading of the modal
					var modalManager = $('body').data('modalmanager');
					modalManager.removeLoading();
				});
		}
	});
}


FMTrendLegend.SaveLastTagSelectionModel = function (tagSelectionModel) {
	tagSelectionModel.PointValues = [];
	$('#Trend-Legend-PointTagSelectionModel').val(JSON.stringify(tagSelectionModel));
};

FMTrendLegend.CreateGuid = function() {  
	function S4() {  
		return (((1+Math.random())*0x10000)|0).toString(16).substring(1);  
	}  
	return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0,3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();  
}  


FMTrendLegend.AddPenPointTagCallSuccess = function (response) {
	if (response !== null) {
		var drawingNumber = FMTrendLegend.DrawingNumber;
		var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
		if (!trendGraph)
			return;

		var trend = trendGraph.GetTrend();

		var tagSelectionModel = response;
		var pointValues = tagSelectionModel.PointValues;
		var pointValue = pointValues[0];

		var pen = {
			DecimalPlaces: pointValue.DecimalPlaces,
			Deleted: false,
			EngineeringUnitsType: pointValue.EngineeringUnitsType,
			IdentityGuid: FMTrendLegend.CreateGuid(),
			isProjectedOnYAxis: false,
			Maximum : pointValue.Maximum,
			Minimum: pointValue.Minimum,
			PenColor: FMTrendLegend.getPenColor(trend),
			PointGuid: pointValue.PointGuid,
			PointID: pointValue.PointID,
			PointTagGuid: pointValue.PointValueIdentifier.IdentityGuid,
			PointTemplateTagGuid: pointValue.PointTemplateTagGuid,
			TagID: pointValue.ID,
			TrendGuid: trend.TrendGuid,
			Units: pointValue.Units,
			UnitString: FMConvertEngUnits.GetEngineeringUnitAbbreviation(pointValue.Units),
			HasAccess: true
		};

		var seriesIndex = trendGraph.GetSecondaryYAxisSeriesIndex();

		if (seriesIndex != -1) {
			trendGraph.RemoveSecondaryYAxis()
		}

		trendGraph.AddPen(pen);

		FMTrendLegend.initLegend(drawingNumber, trend);

		for (var index = 0; index < trend.Pens.length; index++) {
			if (!trendGraph.GetSeriesVisibility(index)) {
				var visibleImgId = 'VisibleImg_' + drawingNumber + '_' + index;
				$('#' + visibleImgId).attr('src', '../../FMWebApp/images/Visible-off-icon-without-BG.png');
			}
		};


		if (seriesIndex != -1) {
			trendGraph.ToggleYAxisForSeries(seriesIndex);
			var projectOnYAxisImgId = 'ProjectOnYAxisImg_' + drawingNumber + '_' + seriesIndex;
			$('#' + projectOnYAxisImgId).attr('src', '../../FMWebApp/images/Toggle-Button-ON.png');
		}

		trendGraph.RefreshTimebar();
	}
};


FMTrendLegend.EditPenPointTagCallSuccess = function (response) {

	if (response !== null) {
		var drawingNumber = FMTrendLegend.DrawingNumber;
		var penIndex = FMTrendLegend.PenIndex;
		var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
		if (!trendGraph)
			return;

		var trend = trendGraph.GetTrend();

		var tagSelectionModel = response;
		var pointValues = tagSelectionModel.PointValues;
		var pointValue = pointValues[0];

		var seriesIndex = trendGraph.GetSecondaryYAxisSeriesIndex();

		if (seriesIndex != -1) {
			trendGraph.RemoveSecondaryYAxis()
		}

		trendGraph.EditPen(penIndex, pointValue);

		FMTrendLegend.initLegend(drawingNumber, trend);

		for (var index = 0; index < trend.Pens.length; index++) {
			if (!trendGraph.GetSeriesVisibility(index)) {
				var visibleImgId = 'VisibleImg_' + drawingNumber + '_' + index;
				$('#' + visibleImgId).attr('src', '../../FMWebApp/images/Visible-off-icon-without-BG.png');
			}
		};


		if (seriesIndex != -1) {
			trendGraph.ToggleYAxisForSeries(seriesIndex);
			var projectOnYAxisImgId = 'ProjectOnYAxisImg_' + drawingNumber + '_' + seriesIndex;
			$('#' + projectOnYAxisImgId).attr('src', '../../FMWebApp/images/Toggle-Button-ON.png');
		}

		trendGraph.RefreshTimebar();
	}
}

//Respond to a Delete request on a trend pen.
FMTrendLegend.OnTrendPenDeleteImgClick = function (img, drawingNumber, penIndex) {

	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;

	var trend = trendGraph.GetTrend();
	var trendId = trend.Pens[penIndex].PointID + '.' + trend.Pens[penIndex].TagID;
	var deletePenText = $('#TrendLegend-DeletePen').val();

	FMLayout.Confirm(deletePenText + ' : ' + trendId + '?',
								deletePenText,
								function () {


									var seriesIndex = trendGraph.GetSecondaryYAxisSeriesIndex();

									if (seriesIndex >= penIndex) {
										trendGraph.RemoveSecondaryYAxis()
									}

									trendGraph.DeletePen(penIndex);

									var trend = trendGraph.GetTrend();

									FMTrendLegend.initLegend(drawingNumber, trend);

									for (var index = 0; index < trend.Pens.length; index++){
										if (!trendGraph.GetSeriesVisibility(index)) {
											var visibleImgId = 'VisibleImg_' + drawingNumber + '_' + index;
											$('#' + visibleImgId).attr('src', '../../FMWebApp/images/Visible-off-icon-without-BG.png');
										}
									};


									if (seriesIndex > penIndex) {
										trendGraph.ToggleYAxisForSeries(seriesIndex - 1);
										var projectOnYAxisImgId = 'ProjectOnYAxisImg_' + drawingNumber + '_' + (seriesIndex - 1);
										$('#' + projectOnYAxisImgId).attr('src', '../../FMWebApp/images/Toggle-Button-ON.png');
									}

									trendGraph.RefreshTimebar();
								});
};



//Update all the pen Value textboxes for a given graph
FMTrendLegend.UpdatePenValues = function (drawingNumber, penValues) {
	if ((drawingNumber === null) || (penValues === null))
		return;
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;
	var trend = trendGraph.GetTrend();
	if (!trend || !trend.Pens || trend.Pens.length == 0)
		return;
	for (var i = 0; i < penValues.length; i++)
	{
		var trendPen = trend.Pens[i];  
		if (trendPen) // new pens may not have been configured yet
		{
			var penValueInputId = 'PenValueInput_' + drawingNumber + '_' + i;
			var numFormatInfoString = $( '#NumberFormatInfoString' ).val();
			var numformatInfo = JSON.parse( numFormatInfoString );
			numformatInfo.NumberDecimalDigits = trendPen.DecimalPlaces;
			var formattedValue = null;

			$( "#" + penValueInputId ).each( function( index )
			{
				var input = $( this );
				if ( penValues[i] != null )
				{
					if (typeof (penValues[i]) === 'string') {
						input[0].value = penValues[i];
						input[0].title = penValues[i];
					}
					else {
						formattedValue = FMFormatValues.FormatValue(trendPen.Units, numformatInfo, penValues[i]);
						input[0].value = formattedValue;
						input[0].title = '';
					}
				}
				else
					input[0].value = "";
			} );
		}
	}
}


//Update all the pen Status labels for a given graph
FMTrendLegend.UpdatePenStatus = function (drawingNumber, penDataCollection) {
	if ((drawingNumber === null) || (penDataCollection === null))
		return;
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;
	var trend = trendGraph.GetTrend();
	if (!trend || !trend.Pens || trend.Pens.length == 0)
		return;
	for (var i = 0; i < penDataCollection.length; i++) {
		var trendPen = trend.Pens[i];
		var penStatusLabelId = FMTrendLegend.penStatusLabelIdPrefix + drawingNumber + '_' + i;
		var statusText = '';
		$("#" + penStatusLabelId).each(function (index) {
			if ((penDataCollection[i] != null) && (penDataCollection[i].y !== $('#RestrictedText').val()) && (penDataCollection[i].status != null))
				statusText = FMOperateIndex.GetStatusCode(penDataCollection[i].status);
			$(this).html(statusText);
			$(this).removeClass();
			$(this).addClass('FMTrendLegendCol6Pos');
			$(this).addClass('FMTrendLegendEntryLabel');
			if ((penDataCollection[i] != null) && (penDataCollection[i].aGuid != null) && (penDataCollection[i].aGuid != "")) {
				var statusClass = ''
				// statusClass = "AlarmPriority-" + (penDataCollection[i].aAck ? "" : "blink-") + penDataCollection[i].aGuid;
				$(this).addClass(statusClass);
			}
		});
	}
}



//Update all the pen Alarm State labels for a given graph
FMTrendLegend.UpdatePenAlarmStates = function (drawingNumber, penDataCollection) {
    if ((drawingNumber === null) || (penDataCollection === null))
        return;
    var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
    if (!trendGraph)
        return;
    var trend = trendGraph.GetTrend();
    if (!trend || !trend.Pens || trend.Pens.length == 0)
        return;
    for (var i = 0; i < penDataCollection.length; i++) {
        var trendPen = trend.Pens[i];
        var penAlarmStateLabelId = FMTrendLegend.penAlarmStateLabelIdPrefix + drawingNumber + '_' + i;
        var alarmStateText = '';
        $("#" + penAlarmStateLabelId).each(function (index) {         
			  if ((penDataCollection[i] != null) && (penDataCollection[i].y !== $('#RestrictedText').val()) && (penDataCollection[i].aState != null))
                alarmStateText = penDataCollection[i].aState;
            $(this).html(alarmStateText);
            $(this).removeClass();
            $(this).addClass('FMTrendLegendCol7Pos');
            $(this).addClass('FMTrendLegendEntryLabel');
            if ((penDataCollection[i] != null) && (penDataCollection[i].aGuid != null) && (penDataCollection[i].aGuid != "")) {
                var alarmPriorityClass = ''
                alarmPriorityClass = "AlarmPriority-" + (penDataCollection[i].aAck ? "" : "blink-") + penDataCollection[i].aGuid;
                $(this).addClass(alarmPriorityClass);
            }
        });
    }
}





//Respond to the Pen Min or Max Value textbox receiving focus
FMTrendLegend.OnTrendPenMinMaxTextBoxFocus = function (textbox, drawingNumber, penIndex) {
	if (!textbox || !textbox.id)
		return;
	textbox.defaultValue = textbox.value; //captures the pen min/max value before it is changed by the user.
};


//Respond to a change in the Pen Min/Max textboxes
FMTrendLegend.OnTrendPenMinMaxTextBoxBlur = function (textbox, drawingNumber, penIndex) {
	var min = null;
	var max = null;
	var value = null;
	var abortChange = false;

	if (!textbox || !textbox.id)
		return;
	var trendGraph = FMTrendIndex.getTrendGraph(drawingNumber);
	if (!trendGraph)
		return;
	var trend = trendGraph.GetTrend();
	if (!trend || !trend.Pens || trend.Pens.length <= penIndex)
		return;
	var trendPen = trend.Pens[penIndex];
	var numFormatInfoString = $('#NumberFormatInfoString').val();
	var numformatInfo = JSON.parse(numFormatInfoString);
	numformatInfo.NumberDecimalDigits = trendPen.DecimalPlaces;

	var formattedValue = $('#' + textbox.id).val();
	var newRawValue = math.bignumber(FMFormatValues.ParseValue(trendPen.Units, numformatInfo, formattedValue));
	if (formattedValue.length == 0)
		abortChange = true;

	if (!abortChange && (textbox.id.startsWith(FMTrendLegend.penMinValueInputIdPrefix))) {
		min = newRawValue;
		if ((min != null) && (min >= trend.Pens[penIndex].Maximum))
				abortChange = true;
	}
	else if (!abortChange && (textbox.id.startsWith(FMTrendLegend.penMaxValueInputIdPrefix))) {
		max = newRawValue;
		if ((max != null) && (max <= trend.Pens[penIndex].Minimum))				
				abortChange = true;
	}

	if (abortChange)
		textbox.value = textbox.defaultValue;	
	else {		
		FMTrendIndex.updateClientsForPenMinMax(drawingNumber, penIndex, min, max);
		textbox.value = FMFormatValues.FormatValue(trendPen.Units, numformatInfo, newRawValue);
	}
};


FMTrendLegend.setControlMasks = function (drawingNumber, trend) {
	//Apply masking on the numeric data input controls
	$(".FMTrendLegendNumericInputTextBox").each(function (index) {
		var ctrl = $(this);
		var ctrlId = ctrl[0].id;
		if (ctrlId.length > FMTrendLegend.penMinValueInputIdPrefix.length) {
				var penIndexDelimiterPos = ctrlId.indexOf("_", FMTrendLegend.penMinValueInputIdPrefix.length);
				if (penIndexDelimiterPos) {
					var ctrlDrawingNumber = ctrlId.substr(FMTrendLegend.penMinValueInputIdPrefix.length, (penIndexDelimiterPos - FMTrendLegend.penMinValueInputIdPrefix.length));
					ctrlDrawingNumber = Number(ctrlDrawingNumber);
				}
				if (ctrlDrawingNumber === drawingNumber)
				{
					var penIndex = ctrlId.substr(penIndexDelimiterPos + 1);
					penIndex = Number(penIndex);
					if (trend && trend.Pens && (trend.Pens.length > penIndex))
						FMTrendLegend.setPenControlMask(ctrl, trend.Pens[penIndex]);
				}
		}
	});
}

FMTrendLegend.setPenControlMask = function (control, trendPen) {
	var unit = trendPen.Units;
	var precision = trendPen.DecimalPlaces;
	var numFormatInfoString = $('#NumberFormatInfoString').val();
	var numformatInfo = JSON.parse(numFormatInfoString);
	numformatInfo.NumberDecimalDigits = trendPen.DecimalPlaces;

	var controlx = $(control);
	$(control).removeNumeric(); // remove numeric mask if there was one
	$(control).unmask(); // if it had a mask remove it

	// add the mask to the edit fields and populate them with the initial formatted value
	// if feet-in-16th or feet-in-8th use 00-00-00 as mask, otherwise is just plain numeric
	if (unit == "27") { //"FML_FtIn16th"
		$(control).mask('S99-99-99', {
			translation: {
				'S': {
					pattern: /-/,
					optional: true
				}
			},
			placeholder: "__-__-__"
		});
	} else if (unit == "19") { //"FML_FtIn8th"
		$(control).mask('S99-99-9', {
			translation: {
				'S': {
					pattern: /-/,
					optional: true
				}
			},
			placeholder: "__-__-__"
		});
	} else {
		if (precision === 0) {
				$(control).attr("placeholder", "");
				$(control).numeric({
					decimal: false
					, negative: true
				});
		} else {
				var numFormatInfo = JSON.parse($('#NumberFormatInfoString').val());
				$(control).attr("placeholder", "");
				$(control).numeric({
					decimal: numFormatInfo.NumberDecimalSeparator
					, negative: true
					, decimalPlaces: precision
				});
		}
	}
}
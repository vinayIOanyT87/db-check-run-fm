var FMTrendGraph = (function (settings) {

	const TIMEBAR_LOCKED_COLOUR = "#666666"; // Darker grey
	const TIMEBAR_ACTIVE_COLOUR = "orange";
	const HISTORICAL_BKGD_COLOUR = "lightgrey";
	const REALTIME_BKGD_COLOUR = "white";

	var chart = null;
	var trend = null;
	var table = null;
	var dataPointActualFull = [];
	var dataPointPercentageFull = [];
	var dataPointActualSnapshot = [];
	var dataPointPercentageSnapshot = [];
	var dataPointActualZoom = [];
	var dataPointPercentageZoom = [];

	var isZoomed = false;
	var isActive = false;
	var maxBrowserDataPoints = 100000;
	var drawingNumber = null;
	var inScrollMode = false;
	var inPauseMode = false;
	var firstRealtimeRun = true;
	var projectedPenIndex = -1;  //index of pen currently projected onto the secondary Y-axis
	var hideUnhidePenOnLegendClick = true;
	var autoSecondaryYScaling = false;
	var nextDataUpdateTimeout = null;
	var realtimeSnapShotStartTime = null;
	var ajaxRequest = null;
	var timebarDocked = true;
	var timebarLocked = true;
	var timebarPrevXData = null;
	var lastSamplingTime = null;
	var lastSamplingTimeElapsed = null;
	var timebarStriplineIndex = -1;
	var settings = settings;
	var initializeTrendtimer = null;
	var refreshTimerID = null;
	var loadingDataInProgress = false;

	var _resetGraph = function () {
		if (ajaxRequest)
				ajaxRequest.abort();
		ajaxRequest = null;
		chart = null;
		trend = null;
		dataPointActualFull = [];
		dataPointPercentageFull = [];
		dataPointActualSnapshot = [];
		dataPointPercentageSnapshot = [];
		dataPointActualZoom = [];
		dataPointPercentageZoom = [];
		isZoomed = false;
		isActive = false;
		inScrollMode = false;
		inPauseMode = false;
		firstRealtimeRun = true;
		projectedPenIndex = -1;  //index of pen currently projected onto the secondary Y-axis
		hideUnhidePenOnLegendClick = true;
		autoSecondaryYScaling = false;
		if (nextDataUpdateTimeout)
				clearTimeout(nextDataUpdateTimeout);
		nextDataUpdateTimeout = null;
		realtimeSnapShotStartTime = null;
		timebarDocked = true;
		timebarLocked = true;
		timebarPrevXData = null;
		lastSamplingTime = null;
		lastSamplingTimeElapsed = null;
		timebarStriplineIndex = -1;
		initializeTrendtimer = null;
		refreshTimerID = null;
	}

	initializeTrend = function () {
	    clearTimeout(initializeTrendtimer);
	    initializeTrendtimer = null;
	    if (isActive === false) {
	        _updateTrendHistoricalData();
	        isActive = true;
	    }
	}
    //Load a Trend object for viewing
	var _loadTrend = function (activeDrawing, trendObj, newTrendDisplay) {
		drawingNumber = activeDrawing;
		trend = trendObj;
		_createGraphStructure();
		_attachContainerEventHandlers();
		_initGraph();
		if (newTrendDisplay === true) {
		    _updateTrendHistoricalData();
		    isActive = true;
		}
		else {
		    // set a five second timer since this is not the active tab. This only happens when operate
		    // in initialy opened and the trend is not the active tab.
		    initializeTrendtimer = setTimeout(initializeTrend, 5000);
        }
	};

	//Reload a Trend object for viewing
	var _reloadTrend = function (trendObj) {
		FMOperateIndex.unsubscribeTagWebWorker(drawingNumber + trendObj.TrendGuid);

		isActive = false;
		var projectedPenIndex = _getSecondaryYAxisSeriesIndex();
		var visibility = [];
		
		for (var index = 0; index < trendObj.Pens.length; index++) {
			visibility.push(_getSeriesVisibility(index));
		}
		_resetGraph();
		chart = null;
		trend = trendObj;
		_createGraphStructure();
		_attachContainerEventHandlers();
		_initGraph();
		_updateTrendHistoricalData();
		if (projectedPenIndex >= 0) {
			_toggleYAxisForSeries(projectedPenIndex);
		}
		isActive = true;
		for (var index = 0; index < trendObj.Pens.length; index++) {
			if (!visibility[index]) {
				_toggleSeriesVisibility(index);
			}
		}

	};


	//Create the basic chart object that will be used to support the Trend
	var _createGraphStructure = function () {
		var container = "diagram" + drawingNumber;
		var isHistoricalTrend = (trend.Mode === 1);
		var yAxisLabel = $('#TrendGraph-Percentage').val();
		chart = new CanvasJS.Chart(container, {
				zoomEnabled: isHistoricalTrend,
				exportEnabled: true,
				backgroundColor: REALTIME_BKGD_COLOUR,
				toolTip: {
					shared: true,
					contentFormatter: function (e) {
						var content = FMTrendIndex.getGraphTooltipContent(drawingNumber, e);
						return content;
					}
				},
				axisX: [{
					labelAngle: -25,
					labelFontSize: 16,
					stripLines: [
					{
						color: "#FF0000",
						showOnTop: true
					}
					]
				}],
				axisY: [
					{
						title: yAxisLabel,
						labelFontSize: 16,
						minimum: 0,
						maximum: 100
					}
				],
				rangeChanging: _rangeChanging,
				rangeChanged: _rangeChanged,
				data: []
		});

	}

	var _rangeChanging = function (e) {
		if (e.trigger == 'zoom') {
				isZoomed = true;
				var start = e.axisX[0].viewportMinimum;
				var end = e.axisX[0].viewportMaximum;
				if (start && end) {
					var startDate = moment(start).toDate();
					var endDate = moment(end).toDate();
					var zoomPeriod = FMTrendIndex.convertLocalDateToUTCDate(endDate).getTime() - FMTrendIndex.convertLocalDateToUTCDate(startDate).getTime();
					startDate = FMTrendIndex.convertUTCDateToLocalDate(moment(FMTrendIndex.convertLocalDateToUTCDate(startDate).getTime() - 2 * zoomPeriod).toDate());
					endDate = FMTrendIndex.convertUTCDateToLocalDate(moment(FMTrendIndex.convertLocalDateToUTCDate(endDate).getTime() + 2 * zoomPeriod).toDate());

					var trendStart = moment(trend.Start).toDate();
					var trendEnd = moment(trend.End).toDate();

					// skip requery if zoom is in the real time data area
					if (startDate > trendEnd) {

						dataPointActualZoom = new Array();
						dataPointPercentageZoom = new Array();

						for (var penIndex = 0; penIndex < trend.Pens.length; penIndex++) {

								dataPointActualZoom.push(new Array());
								dataPointPercentageZoom.push(new Array());

								// when trend is real time must combine realtime data with requery
								if (trend.Mode === 0) {
									dataPointActualSnapshot[penIndex].forEach(function (data) {
										if (data.x > startDate
								&& data.x <= endDate) {
												dataPointActualZoom[penIndex].push(data);
										}
									});
									dataPointPercentageSnapshot[penIndex].forEach(function (data) {
										if (data.x > startDate
								&& data.x <= endDate) {
												dataPointPercentageZoom[penIndex].push(data);
										}
									});

								}

								if (penIndex === projectedPenIndex) {
									chart.data[penIndex].set("dataPoints", dataPointActualZoom[penIndex], false);
								}
								else {
									chart.data[penIndex].set("dataPoints", dataPointPercentageZoom[penIndex], false);
								}
						}


						return;
					}

					// limit requery to original limits
					if (startDate < trendStart) {
						startDate = trendStart;
					}

					var requeryEndDate = endDate;
					if (requeryEndDate > trendEnd) {
						requeryEndDate = trendEnd;
					}

					var tags = [];
					// notification position
					var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };

					trend.Pens.forEach(function (pen) {
						tags.push(pen.PointTagGuid);
					});

					// Call controller to get archive values - Note: that end is limited by trendEnd
					$.ajax({
						url: 'GetTrendArchiveData',
						//async: false,
						cache: false,
						type: 'POST',
						contentType: 'application/json',
						data: JSON.stringify({ tagGuids: tags, start: moment(startDate).format("YYYY-MM-DD HH:mm:ss Z"), end: moment(requeryEndDate).format("YYYY-MM-DD HH:mm:ss Z") }),
						success: function (response) {

								// remove previous notifications
								PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

								FMErrorAndExceptionHandling.HandleMessages(response,
					            function (data) {_rangeChangingRequerySuccess(data, startDate, requeryEndDate, endDate);},
                                                                     messageAttributes);
						},
						error: function (request, status, error) {
								// remove previous notifications
								PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

								FMErrorAndExceptionHandling.ShowError('Failed to Read Tag Data', null, messageAttributes);
						}
					});
				}
		}
		else if (e.trigger == 'reset') {
				isZoomed = false;
				for (var i = 0; i < trend.Pens.length; i++) {
					if (trend.Mode === 0) {
						if (i === projectedPenIndex) {
								chart.data[i].set("dataPoints", dataPointActualSnapshot[i], false);
						}
						else {
								chart.data[i].set("dataPoints", dataPointPercentageSnapshot[i], false);
						}
					}
					else {
						if (i === projectedPenIndex) {
								chart.data[i].set("dataPoints", dataPointActualFull[i], false);
						}
						else {
								chart.data[i].set("dataPoints", dataPointPercentageFull[i], false);
						}
					}
				}

				chart.axisX[0].set("viewportMinimum", null, false);
				chart.axisX[0].set("viewportMaximum", null, false);

				var intervalInfo = _getIntervalDetails(trend.Period);
				chart.axisX[0].set("intervalType", intervalInfo.IntervalType);
				chart.axisX[0].set("interval", intervalInfo.Interval);
				_refreshSecondaryYAxis();

				chart.render();
		}
	}


	var _rangeChangingRequerySuccess = function (trendArchiveData, startDate, requeryEndDate, endDate) {

		dataPointActualZoom = new Array();
		dataPointPercentageZoom = new Array();
		
		for (var penIndex = 0; penIndex < trendArchiveData.length; penIndex++) {

				dataPointActualZoom.push(new Array());
				dataPointPercentageZoom.push(new Array());

				_loadArchiveData(trend.Pens[penIndex], trendArchiveData[penIndex], dataPointActualZoom[penIndex], dataPointPercentageZoom[penIndex]);

				// when trend is real time must combine realtime data with requery
				if (trend.Mode === 0) {
					dataPointActualSnapshot[penIndex].forEach(function (data) {
						if (data.x > requeryEndDate
					&& data.x <= endDate) {
								dataPointActualZoom[penIndex].push(data);
						}
					});
					dataPointPercentageSnapshot[penIndex].forEach(function (data) {
						if (data.x > requeryEndDate
					&& data.x <= endDate) {
								dataPointPercentageZoom[penIndex].push(data);
						}
					});

				}

				if (penIndex === projectedPenIndex) {
					chart.data[penIndex].set("dataPoints", dataPointActualZoom[penIndex], false);
				}
				else {
					chart.data[penIndex].set("dataPoints", dataPointPercentageZoom[penIndex], false);
				}
		}

		var zoomPeriod = FMTrendIndex.convertLocalDateToUTCDate(endDate).getTime() - FMTrendIndex.convertLocalDateToUTCDate(startDate).getTime();

		if (trend.PeriodType == 0) {
			zoomPeriod = zoomPeriod / 60000;
		}
		else if (trend.PeriodType == 1) {
			zoomPeriod = zoomPeriod / 3600000;
		}
		else if (trend.PeriodType == 2) {
			zoomPeriod = zoomPeriod / 86400000;
		}

		var intervalInfo = _getIntervalDetails(zoomPeriod);
		chart.axisX[0].set("intervalType", intervalInfo.IntervalType);
		chart.axisX[0].set("interval", intervalInfo.Interval);

		_refreshSecondaryYAxis();

		chart.render();

	}

	//Perform basic configuration of the chart object and its dataseries based on the Trend configuration
	var _initGraph = function () {
		dataPointActualFull = new Array();
		dataPointPercentageFull = new Array();
		dataPointActualSnapshot = new Array();
		dataPointPercentageSnapshot = new Array();
		dataPointActualZoom = new Array();
		dataPointPercentageZoom = new Array();


		var dataSeries = null;
		for (var penIndex = 0; penIndex < trend.Pens.length; penIndex++) {
				dataPointActualFull.push(new Array());
				dataPointPercentageFull.push(new Array());
				dataPointActualSnapshot.push(new Array());
				dataPointPercentageSnapshot.push(new Array());
				dataPointActualZoom.push(new Array());
				dataPointPercentageZoom.push(new Array());

				dataSeries = new Object();
				dataSeries.type = "stepLine";
				dataSeries.xValueType = "dateTime";
				dataSeries.axisYIndex = 0;
				dataSeries.name = trend.Pens[penIndex].PointID + "." + trend.Pens[penIndex].TagID;
				dataSeries.color = trend.Pens[penIndex].PenColor;
				dataSeries.lineColor = trend.Pens[penIndex].PenColor;
				dataSeries.fillOpacity = 0;
				dataSeries.dataPoints = dataPointPercentageFull[penIndex];
				chart.options.data.push(dataSeries);
		}

		//Add a dummy dataseries tied to the default (primary) y-axis. This dummy dataseries will be used to help maintain the primary axis visibility in the event all the dataseries are projected to the secondary y-axis.
		chart.addTo("data", { dataPoints: [] });
	};


	//Attach event handlers to the graph container object
	var _attachContainerEventHandlers = function () {
		var container = "diagram" + drawingNumber;
		//Respond to the mouse leaving the graph. Lock the timebar.
		document.getElementById(container).onmouseout = function (event) {
				if (timebarLocked)
					return;
				var e = event.toElement || event.relatedTarget;
				//Ignore mouseout events that correspond to the mouse either entering the chart tooltip area or entering back into the chart after leaving the chart tooltip area
				if (e && (e.parentNode) && ((e.parentNode.className === "canvasjs-chart-tooltip") || (e.parentNode.className === "canvasjs-chart-container")))
					return;
				var timebarCurrentXData = document.getElementById("xData" + drawingNumber).innerHTML;
				timebarCurrentXData = Number(timebarCurrentXData);
				timebarLocked = true;
				_updateTimebar(timebarCurrentXData);
		}

		//Respond to a double-click on the graph. Lock/unlock the timebar.
		document.getElementById(container).ondblclick = function () {
				if ((trend.Mode === 0) && (!inPauseMode))
					return;
				timebarLocked = !timebarLocked;
				var timebarCurrentXData = document.getElementById("xData" + drawingNumber).innerHTML;
				timebarCurrentXData = Number(timebarCurrentXData);
				if (!timebarLocked)
					timebarCurrentXData = _getAdjustedTimebarLocationForViewport(timebarCurrentXData);
				_updateTimebar(timebarCurrentXData);
		}

		//Respond to a mouse move on the graph. Update the timebar display to reflect the new position of the mouse.
		document.getElementById(container).onmousemove = function () {
				if (timebarLocked)
					return;
				if (document.getElementById("xData" + drawingNumber)) {
					var timebarCurrentXData = document.getElementById("xData" + drawingNumber).innerHTML;
					timebarCurrentXData = Number(timebarCurrentXData);
					if (timebarPrevXData !== timebarCurrentXData) {
						timebarDocked = false;
						_updateTimebar(timebarCurrentXData);
						timebarPrevXData = timebarCurrentXData;
					}
				}
		}
	}


	//Initiate the archive data retrieval
	var _updateTrendHistoricalData = function () {
		if (!loadingDataInProgress) {
		loadingDataInProgress = true;

			 if (trend.Mode === 0) {
				trend.End = new Date();
				var end = FMTrendIndex.convertLocalDateToUTCDate(trend.End);

				var start = new Date(end.getTime() - _getWindowPeriodInTicks());

				trend.Start = FMTrendIndex.convertUTCDateToLocalDate(start);
			}


			var startDate = moment(trend.Start).toDate();
			var endDate = moment(trend.End).toDate();

			var tags = [];
			// notification position
			var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };

			trend.Pens.forEach(function (pen) {
				tags.push(pen.PointTagGuid);
			});

			if (tags.length <= 0) {
				return;
			}

			// Call controller to get archive values
			var ajaxRequest = $.ajax({
				url: 'GetTrendArchiveData',
				cache: false,
				type: 'POST',
				contentType: 'application/json',
				data: JSON.stringify({ tagGuids: tags, start: moment(startDate).format("YYYY-MM-DD HH:mm:ss Z"), end: moment(endDate).format("YYYY-MM-DD HH:mm:ss Z") }),
				success: function (response) {
					// remove previous notifications
					PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

					if (ajaxRequest == null) {
						return;
					}

					FMErrorAndExceptionHandling.HandleMessages(response,
						function (data) {
							_updateTrendHistoricalDataSuccess(data);
						}, messageAttributes);
					loadingDataInProgress = false;
				},
				error: function (request, status, error) {
					// remove previous notifications
					PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

					if (ajaxRequest == null) {
						return;
					}

					FMErrorAndExceptionHandling.ShowError('Failed to Read Tag Data', null, messageAttributes);
					loadingDataInProgress = false;
				}
			});

		}};


	//Respond to a successfull data retrieval of archive data
	var _updateTrendHistoricalDataSuccess = function (trendArchiveData) {
		if ( trendArchiveData != null )
		{
			for ( var penIndex = 0; penIndex < trendArchiveData.length; penIndex++ )
			{
				_loadArchiveData( trend.Pens[penIndex], trendArchiveData[penIndex], dataPointActualFull[penIndex], dataPointPercentageFull[penIndex] );
			}
		}
		_resetTimebar();
		var intervalInfo = _getIntervalDetails(trend.Period);
		chart.axisX[0].set( "intervalType", intervalInfo.IntervalType );
		chart.axisX[0].set( "interval", intervalInfo.Interval );
		chart.render();
		if (trend.Mode === 0) {
			if (!lastSamplingTime) {
				lastSamplingTime = (new Date()).getTime();
			}
			var sampleRate = _getSampleRate();
			nextDataUpdateTimeout = setTimeout( function(){
				_updateTrendRealtimeData()
				}, sampleRate );
		}
		_setHistoricalBackgroundColour( trendArchiveData );
	};



	//Set the background colour of the historical portion of a real-time trend.
	var _setHistoricalBackgroundColour = function (trendArchiveData) {
		var minDate = null;
		var maxDate = null;
		var runningDate = null;
		if (trendArchiveData === null)
		    return;

		for (var penIndex = 0; penIndex < trendArchiveData.length; penIndex++) {
				for (var penDataIndex = 0; penDataIndex < trendArchiveData[penIndex].length; penDataIndex++) {
					runningDate = moment(trendArchiveData[penIndex][penDataIndex].ValueTimeStamp).toDate().getTime();
					if (minDate == null) {
						minDate = runningDate;
						maxDate = runningDate;
					}
					if (runningDate < minDate)
						minDate = runningDate;
					else if (runningDate > maxDate)
						maxDate = runningDate;
				}
		}
		if (minDate == null)
				return;
		chart.axisX[0].addTo("stripLines", { startValue: minDate, endValue: maxDate, color: HISTORICAL_BKGD_COLOUR });
	}


	//Load archive data
	var _loadArchiveData = function (pen, penTrendArchiveData, penDataPointActualFull, penDataPointPercentageFull) {
		var xValue = null;
		var yValue = null;
		var yValuePercent = null;
		var penMinVal = null;
		var penMaxVal = null;
		penMinVal = pen.Minimum;
		penMaxVal = pen.Maximum;
		
		for (var penDataIndex = 0; penDataIndex < penTrendArchiveData.length; penDataIndex++) {
			var data = penTrendArchiveData[penDataIndex];
			xValue = moment(data.ValueTimeStamp).toDate();
			if (data.Value === null) {
				yValue = null;
				yValuePercent = null;
				pen.HasAccess = true;
			}
			else if (data.Value === $('#RestrictedText').val())
			{
				var penIndex = _getPenIndex( pen.PointID +"."+ pen.TagID );
				yValue = $('#RestrictedText').val();
				yValuePercent = null;
				if (_getSeriesVisibility(penIndex)) {
					//var img = $('#VisibleImg_' + drawingNumber + '_' + penIndex);
					var img = document.getElementById('VisibleImg_' + drawingNumber + '_' + penIndex);
					FMTrendLegend.OnTrendPenVisibleImgClick(img, drawingNumber, penIndex);
				}
				pen.HasAccess = false;

			}
			else {
				data.Value = Number(data.Value);
				yValue = data.Value;
				if (data.EngineeringUnitsIndex != pen.Units) {
					yValue = FMConvertEngUnits.Convert(yValue, data.EngineeringUnitsIndex, pen.Units);
					yValue = math.number(yValue);
				}
				yValuePercent = 100 * (yValue - penMinVal) / (penMaxVal - penMinVal);
				pen.HasAccess = true;
			}
			penDataPointActualFull.push({ x: xValue, y: yValue, status : data.ValueOpcStatus, aGuid: data.AlarmPriorityGuid, aAck: data.Acknowledged, aState: data.AlarmState });
			penDataPointPercentageFull.push({ x: xValue, y: yValuePercent });
		}
	};


	//Initiate an update of a Realtime time.
	var _updateTrendRealtimeData = function ()
	{

		var pointValueList = [];

		trend.Pens.forEach(function (pen) {
			var newPointValue = {
				IdentityGuid: pen.PointTagGuid,
				PointValueType: 0, //tag
				PropertyID: null 
			};

			pointValueList.push(newPointValue);
		});
		
		FMOperateIndex.subscribeTagWebWorker(drawingNumber + trend.TrendGuid, pointValueList, function (data) {
			_updateTrendRealtimeDataSuccess(data);
		}, true);

	};


	//Respond to a successfull real-time data retrieval from the server
	var _updateTrendRealtimeDataSuccess = function (trendCurrentData) {
		 if (!loadingDataInProgress) {
			  loadingDataInProgress = true;
			  var currentDT = new Date();
			  var xValue = currentDT;
			  var yValue = null;
			  var yValuePercent = null;
			  var penMinVal = null;
			  var penMaxVal = null;

			  if (!trendCurrentData) {
					return;
			  }

			  for (var penIndex = 0; penIndex < trend.Pens.length; penIndex++) {
					for (var dataIndex = 0; dataIndex < trendCurrentData.length; dataIndex++) {
						 if (trend.Pens[penIndex].PointTagGuid != trendCurrentData[dataIndex].PointValueIdentifier_IdentityGuid) {
							  continue;
						 }
						 var pen = trend.Pens[penIndex];
						 var data = trendCurrentData[dataIndex];
						 penMinVal = pen.Minimum;
						 penMaxVal = pen.Maximum;
						 if (data.CommunicationsFailure) {
							  yValue = $('#CommunicationsFailureText').val();
						 }
						 else if (data.Access && !(data.Access.View || data.Access.Modify)) {
							  yValue = $('#RestrictedText').val();
						 }
						 else if ((data.Status & 0x80000000) !== 0) {
							  yValue = FMOperateIndex.GetStatusCode(data.Status);
						 }
						 else {
							  yValue = data.Value;
						 }

						 if (yValue === $('#RestrictedText').val()) {
							  if (_getSeriesVisibility(penIndex)) {
									//var img = $('#VisibleImg_' + drawingNumber + '_' + penIndex);
									var img = document.getElementById('VisibleImg_' + drawingNumber + '_' + penIndex);
									FMTrendLegend.OnTrendPenVisibleImgClick(img, drawingNumber, penIndex);
							  }
							  pen.HasAccess = false;
						 }
						 else {
							  pen.HasAccess = true;
						 }

						 if (yValue === null || typeof (yValue) === 'string') {
							  yValuePercent = null;
						 }

						 else {
							  if (data.Units != pen.Units) {
									yValue = FMConvertEngUnits.Convert(yValue, data.Units, pen.Units);
									yValue = math.number(yValue);
							  }
							  yValuePercent = 100 * (yValue - penMinVal) / (penMaxVal - penMinVal);
						 }
						 if (!dataPointActualFull[penIndex]) {
							  dataPointActualFull[penIndex] = [];
							  dataPointPercentageFull[penIndex] = [];
						 }
						 dataPointActualFull[penIndex].push({ x: xValue, y: yValue, status: data.Status, aGuid: data.AlarmPriorityGuid, aAck: data.Acknowledged, aState: data.AlarmState });
						 dataPointPercentageFull[penIndex].push({ x: xValue, y: yValuePercent });
						 break;
					}
					var pen = trend.Pens[penIndex];
					penMinVal = pen.Minimum;
					penMaxVal = pen.Maximum;

					if (!dataPointActualFull[penIndex]) {
						 continue;
					}
					prevData = dataPointActualFull[penIndex][dataPointActualFull[penIndex].length - 1];
					yValue = prevData.y;

					if (yValue === $('#RestrictedText').val()) {
						 if (_getSeriesVisibility(penIndex)) {
							  //var img = $('#VisibleImg_' + drawingNumber + '_' + penIndex);
							  var img = document.getElementById('VisibleImg_' + drawingNumber + '_' + penIndex);
							  FMTrendLegend.OnTrendPenVisibleImgClick(img, drawingNumber, penIndex);
						 }
						 pen.HasAccess = false;
					}
					else {
						 pen.HasAccess = true;
					}

					if (yValue === null || typeof (yValue) === 'string') {
						 yValuePercent = null;
					}

					else {
						 yValuePercent = 100 * (yValue - penMinVal) / (penMaxVal - penMinVal);
					}

					if (!dataPointActualFull[penIndex]) {
						 dataPointActualFull[penIndex] = [];
						 dataPointPercentageFull[penIndex] = [];
					}
					dataPointActualFull[penIndex].push({ x: xValue, y: yValue, status: prevData.status, aGuid: prevData.aGuid, aAck: prevData.aAck, aState: prevData.aState });
					dataPointPercentageFull[penIndex].push({ x: xValue, y: yValuePercent });
			  }

					if (lastSamplingTime != null)
						 lastSamplingTimeElapsed = currentDT.getTime() - lastSamplingTime;
					lastSamplingTime = currentDT.getTime();
					_shiftDatasets();
					firstRealtimeRun = false;
					var playPauseState = FMTrendMenuBar.getTrendPlayPauseState(drawingNumber);
					if (playPauseState == 0)
						 chart.render();
			  }
			  loadingDataInProgress = false;
		};


	//Clip the scrolling datasets to simulate the scrolling action on the real-time trends. 
	var _shiftDatasets = function () {

		var cutoffTime = _getScrollingEndTime();
		cutoffTime = moment(cutoffTime.getTime() - _getWindowPeriodInTicks()).toDate();
		var datasetEmpty = false;
		for (var penIndex = 0; penIndex < trend.Pens.length; penIndex++) {
			if (dataPointPercentageFull[penIndex] && dataPointPercentageFull[penIndex].length > 0) {
					datasetEmpty = false;
					var firstDataPointTime = dataPointPercentageFull[penIndex][0].x;
					while (!datasetEmpty && (firstDataPointTime < cutoffTime)) {
						if (dataPointPercentageFull[penIndex].length > 1
						&&	dataPointPercentageFull[penIndex][1].x > cutoffTime){
							dataPointPercentageFull[penIndex][0].x = cutoffTime;
							dataPointActualFull[penIndex][0].x = cutoffTime;
							firstDataPointTime = dataPointPercentageFull[penIndex][0].x;
						}
						else {
							dataPointPercentageFull[penIndex].shift();
							dataPointActualFull[penIndex].shift();
							if (dataPointPercentageFull[penIndex].length > 0) {
								firstDataPointTime = dataPointPercentageFull[penIndex][0].x;
							}
							else {
								datasetEmpty = true;
							}
						}
					}
				}
		}
		_shiftTimebar();

		//Drop past entries from the Full datasets if they have reached their limit as well
		var dataPointsCount = 0;
		var minDate = new Date();
		for (var penIndex = 0; penIndex < trend.Pens.length; penIndex++) {
			if ( dataPointPercentageFull[penIndex] )
			{
				dataPointsCount += dataPointPercentageFull[penIndex].length;
				dataPointsCount += dataPointActualFull[penIndex].length;
				firstDataPointTime = moment( dataPointPercentageFull[penIndex][0].x ).toDate();
				if ( firstDataPointTime < minDate )
					minDate = firstDataPointTime;
			}
		}
		if (dataPointsCount > maxBrowserDataPoints) {
				for (var penIndex = 0; penIndex < trend.Pens.length; penIndex++) {
					if (dataPointPercentageFull[penIndex] && dataPointPercentageFull[penIndex].length > 0) {
						firstDataPointTime = moment(dataPointPercentageFull[penIndex][0].x).toDate();
						if (firstDataPointTime <= minDate) {
								dataPointPercentageFull[penIndex].shift();
								dataPointActualFull[penIndex].shift();
						}
					}
				}
		}
	}


	//Return the first PenIndex for a dataseries with a name in the format <PointID>.<TagID>
	var _getPenIndex = function (dataSeriesName) {
		var result = null;
		for (var i = 0; i < trend.Pens.length; i++) {
				var seriesName = trend.Pens[i].PointID + "." + trend.Pens[i].TagID;
				if (seriesName === dataSeriesName) {
					result = i;
					break;
				}
		}
		return result;
	}




	//Derive the Interval Type and Interval values for the XAxis tick marks
	var _getIntervalDetails = function (period) {
		var intervalType = "seconds";
		var seconds = 1;
		if (trend.PeriodType == 0) {
			seconds = period * 60;
		}
		else if (trend.PeriodType == 1) {
			seconds = period * 3600;
		}
		else if (trend.PeriodType == 2) {
			seconds = period * 86400;
		}

		if (seconds <= 10) {
			intervalType = "second";
			interval = 1;
		}
		else if (seconds > 10 && seconds <= 30) {
			intervalType = "second";
			interval = 5;
		}
		else if (seconds > 30 && seconds <= 60) {
			intervalType = "second";
			interval = 10;
		}
		else if (seconds > 60 && seconds <= 600) {
			intervalType = "second";
			interval = 30;
		}
		else if (seconds > 600 && seconds <= 1800) {
			intervalType = "minute";
			interval = 1;
		}
		else if (seconds > 1800 && seconds <= 3600) {
			intervalType = "minute";
			interval = 10;
		}
		else if (seconds > 3600 && seconds <= 86400) {
			intervalType = "hour";
			interval = 1;
		}
		else if (seconds > 86400 && seconds <= 604800) {
			intervalType = "day";
			interval = 1;
		}
		else if (seconds > 604800 && seconds <= 2678400) {
			intervalType = "day";
			interval = 7;
		}
		else {
			intervalType = "month";
			interval = 1;
		}


		var intervalInfo = {};
		intervalInfo.IntervalType = intervalType;
		intervalInfo.Interval = interval;
		return intervalInfo;
	}


	//As we zoom in down to resolutions higher to that of the timebar datetime label display, e.g. down to milliseconds, because of the rounding of timebar display it 
	//might not be located excatly on a data point anymore which will prevent it from appearing when activated, since striplines can only be located on a data point. 
	//This method returns the closest valid data point location in the current viewport next to the current location of the timebar if the latter does not fall exactly
	//on a data point location.
	var _getAdjustedTimebarLocationForViewport = function (timebarCurrentXLoc) {
		var result = timebarCurrentXLoc;
		var xLoc = null;
		var closestXLoc = null;
		for (var i = 0; i < dataPointActualFull.length; i++) {
				if (dataPointActualFull[i].length > 0) {
					for (var j = 0; j < dataPointActualFull[i].length; j++) {
						xLoc = dataPointActualFull[i][j].x.getTime();
						if (xLoc < chart.axisX[0].viewportMinimum)
								continue;
						if (xLoc > chart.axisX[0].viewportMaximum)
								break;
						if (xLoc == timebarCurrentXLoc)
								return timebarCurrentXLoc;
						if (xLoc < chart.axisX[0].viewportMaximum) {
								if ((closestXLoc == null) || ((Math.abs(timebarCurrentXLoc - xLoc) < Math.abs(timebarCurrentXLoc - closestXLoc))))
									closestXLoc = xLoc;
						}

					}
				}
		}
		if (closestXLoc != null)
				result = closestXLoc;
		return result;
	}


	//Pause the display of a Real-time trend (without actually pausing the update of the underlying dataseries of the graph)
	var _pauseTrend = function () {
		var xValue = 0;
		var yValue = 0;
		var yValuePercent = 0;
		if (trend.Mode !== 0) {
				return;
		}

		inPauseMode = true;

		//capture the start time of the realtime trend at this exact point in time
		var start = _getScrollingStartTime();
		if (start == null) {
			var end = new Date();
			end = FMTrendIndex.convertLocalDateToUTCDate(end);
			start = new Date(end.getTime() - _getWindowPeriodInTicks());
			start = FMTrendIndex.convertUTCDateToLocalDate(start);
		}
		realtimeSnapShotStartTime = moment(start).toDate();

		//take a snapshot of the full range datasets
		for (var penIndex = 0; penIndex < trend.Pens.length; penIndex++) {
				dataPointActualSnapshot[penIndex] = new Array();
				dataPointPercentageSnapshot[penIndex] = new Array();

				for (var penDataIndex = 0; penDataIndex < dataPointPercentageFull[penIndex].length; penDataIndex++) {
					xValue = moment(dataPointPercentageFull[penIndex][penDataIndex].x).toDate();
					yValue = dataPointActualFull[penIndex][penDataIndex].y;
					yValuePercent = dataPointPercentageFull[penIndex][penDataIndex].y;

					dataPointActualSnapshot[penIndex].push({ x: xValue, y: yValue, status: dataPointActualFull[penIndex][penDataIndex].status, aGuid: dataPointActualFull[penIndex][penDataIndex].aGuid, aAck: dataPointActualFull[penIndex][penDataIndex].aAck, aState: dataPointActualFull[penIndex][penDataIndex].aState });
					dataPointPercentageSnapshot[penIndex].push({ x: xValue, y: yValuePercent });
				}

				//switch the datasource
				if (penIndex === projectedPenIndex)
					chart.data[penIndex].set("dataPoints", dataPointActualSnapshot[penIndex], false);
				else
					chart.data[penIndex].set("dataPoints", dataPointPercentageSnapshot[penIndex], false);
		}

		//Enable Zoom
		chart.render();
		chart.set("zoomEnabled", true);

		//Modify the operation of the CanvasJS Reset button to always reset the chart to the initial window at which the chart was paused
		var resetButton = _getCanvasJSMenuButton("Reset");
		if (resetButton) {
				$(resetButton).click(function () {
					_resetViewport(true);
				});
		}
	}



	//Return the time of the first data point in the scrolling window
	var _getScrollingStartTime = function () {
		var startTime = null;
		for (var i = 0; i < dataPointActualFull.length; i++) {
				if (dataPointActualFull[i].length > 0) {
					if ((startTime == null) || (dataPointActualFull[i][0].x < startTime))
						startTime = dataPointActualFull[i][0].x;
				}
		}
		return startTime;
	}


	//Return the time of the last data point in the scrolling window
	var _getScrollingEndTime = function () {
		var endTime = null;
		var lastIndex = 0;
		for (var i = 0; i < dataPointActualFull.length; i++) {
				if (dataPointActualFull[i].length > 0) {
					lastIndex = dataPointActualFull[i].length - 1;
					if ((endTime == null) || (dataPointActualFull[i][lastIndex].x > endTime))
						endTime = dataPointActualFull[i][lastIndex].x;
				}
		}
		return endTime;
	}


	//Return the time of the last data point in the whole trend
	var _getTrendCurrentEndTime = function () {
		var endTime = null;
		var lastIndex = 0;
		for (var i = 0; i < dataPointActualFull.length; i++) {
				if (dataPointActualFull[i].length > 0) {
					lastIndex = dataPointActualFull[i].length - 1;
					if ((endTime == null) || (dataPointActualFull[i][lastIndex].x > endTime))
						endTime = dataPointActualFull[i][lastIndex].x.getTime();
				}
		}
		return endTime;
	}


	//Reset the Viewport either to the full view captured in the source datasets, or to the initial pause window.
	var _resetViewport = function (resetToInitialPauseWindow) {
		var startTime = null;
		if ((trend.Mode === 0) && (resetToInitialPauseWindow))
				startTime = realtimeSnapShotStartTime

		for (var i = 0; i < chart.axisX.length; i++) {
				chart.axisX[i].set("viewportMinimum", startTime, false);
				chart.axisX[i].set("viewportMaximum", null, true);
		}
		for (var j = 0; j < chart.axisY.length; j++) {
				chart.axisY[j].set("viewportMinimum", null, false);
				chart.axisY[j].set("viewportMaximum", null, true);
		}
		for (var j = 0; j < chart.axisY2.length; j++) {
				chart.axisY2[j].set("viewportMinimum", null, false);
				chart.axisY2[j].set("viewportMaximum", null, true);
		}
	}


	//Resume the real-time display of a Real-time trend
	var _resumeTrend = function () {
		if (trend.Mode != 0) {
			return;
		}

		inPauseMode = false;
		isZoomed = false;

		//reset the zoom view
		for (var i = 0; i < chart.axisX.length; i++) {
				chart.axisX[i].set("viewportMinimum", null, false);
				chart.axisX[i].set("viewportMaximum", null, true);
		}
		for (var j = 0; j < chart.axisY.length; j++) {
				chart.axisY[j].set("viewportMinimum", null, false);
				chart.axisY[j].set("viewportMaximum", null, true);
		}
		//switch the datasource
		for (var i = 0; i < trend.Pens.length; i++) {
				if (i === projectedPenIndex)
					chart.options.data[i].dataPoints = dataPointActualFull[i];
				else
					chart.options.data[i].dataPoints = dataPointPercentageFull[i];
		}
		//set the interval and disable zoom
		var intervalInfo = _getIntervalDetails(trend.Period);
		chart.axisX[0].set("intervalType", intervalInfo.IntervalType);
		chart.axisX[0].set("interval", intervalInfo.Interval);
		chart.render();
		chart.set("zoomEnabled", false);
		isZoomed = false;
		_resetTimebar();
	}


	//Get the index of the series currectly projected against the secondary Y-Axis
	var _getSecondaryYAxisSeriesIndex = function () {
		var seriesIndex = -1;
		for (var i = 0; i < trend.Pens.length; i++) {
				if ((chart.data != null) && (chart.data[i].axisYType == "secondary")) {
					seriesIndex = i;
					break;
				}
		}
		return seriesIndex;
	}


	//Remove the Secondary Y-Axis
	var _removeSecondaryYAxis = function () {
		var seriesIndex = _getSecondaryYAxisSeriesIndex();
		if (seriesIndex >= 0) {
				if (isZoomed)
					chart.options.data[seriesIndex].dataPoints = dataPointPercentageZoom[seriesIndex];
				else if ((trend.Mode == 0) && (inPauseMode))
					chart.options.data[seriesIndex].dataPoints = dataPointPercentageSnapshot[seriesIndex];
				else
					chart.options.data[seriesIndex].dataPoints = dataPointPercentageFull[seriesIndex];

				chart.axisY2[0].remove();
				chart.data[seriesIndex].set("axisYType", "primary");//link dataSeries back to the primary Y-axis
				chart.data[seriesIndex].set("axisYIndex", 0);//link dataSeries to the first (default) axis of the primary Y-axis set
				projectedPenIndex = -1;
		}
		return seriesIndex;
	}


	//Add a Secondary Y-Axis to the graph for a given dataseries
	var _toggleYAxisForSeries = function (seriesIndex) {
		var defaultIntervalCount = 10;
		var seriesIndexOfRemovedAxis = _removeSecondaryYAxis();
		if (seriesIndexOfRemovedAxis == seriesIndex) {
			projectedPenIndex = -1;
			return;
		}
		projectedPenIndex = seriesIndex;
		if (isZoomed)
			chart.options.data[seriesIndex].dataPoints = dataPointActualZoom[seriesIndex];
		else if ((trend.Mode == 0) && (inPauseMode))
			chart.options.data[seriesIndex].dataPoints = dataPointActualSnapshot[seriesIndex];
		else
			chart.options.data[seriesIndex].dataPoints = dataPointActualFull[seriesIndex];

		seriesName = trend.Pens[seriesIndex].PointID + "." + trend.Pens[seriesIndex].TagID;
		seriesUnits = trend.Pens[seriesIndex].UnitString;
		seriesColour = trend.Pens[seriesIndex].PenColor;

		chart.addTo("axisY2", { title: seriesName + " (" + seriesUnits + ")", lineColor: seriesColour });
		chart.data[seriesIndex].set("axisYType", "secondary");//link second dataSeries to second
		chart.data[seriesIndex].set("axisYIndex", 0);//link second dataSeries to second
		if (!isZoomed && !autoSecondaryYScaling) {
			yAxisAdjustedIntervalValues = _getYIntervalAdjustmentValues(trend.Pens[seriesIndex].Minimum, trend.Pens[seriesIndex].Maximum, defaultIntervalCount);
			if (yAxisAdjustedIntervalValues.intervalValue == null) {
					//undo adjustments
					yAxisAdjustedIntervalValues.yMin = trend.Pens[seriesIndex].Minimum;
					yAxisAdjustedIntervalValues.yMax = trend.Pens[seriesIndex].Maximum;
					yAxisAdjustedIntervalValues.intervalValue = null;
					if ((trend.Pens[seriesIndex].Minimum != null) && (trend.Pens[seriesIndex].Minimum != null) && (defaultIntervalCount != null))
						yAxisAdjustedIntervalValues.intervalValue = Math.abs(trend.Pens[seriesIndex].Maximum - trend.Pens[seriesIndex].Minimum) / defaultIntervalCount;
			}
			chart.axisY2[0].set("minimum", yAxisAdjustedIntervalValues.yMin);
			chart.axisY2[0].set("maximum", yAxisAdjustedIntervalValues.yMax);
			chart.axisY2[0].set("interval", yAxisAdjustedIntervalValues.intervalValue);				
		}
		chart.options.axisY2[0].lineThickness = 4;
		chart.options.axisY2[0].labelFormatter = function (e) { return _formatPenValue(seriesIndex, e.value) };

		chart.render();
	}


	var _refreshSecondaryYAxis = function () {
		var projectedPenIndex = _getSecondaryYAxisSeriesIndex();
		if (projectedPenIndex >= 0) {
			_removeSecondaryYAxis();
			_toggleYAxisForSeries(projectedPenIndex);
		}
		chart.render();
	}


	// precision is 10 for 10ths, 100 for 100ths, etc.
	var _roundUp = function (num, precision) {
		return Math.ceil(num * precision) / precision;
	}

	// precision is 10 for 10ths, 100 for 100ths, etc.
	var _roundDown = function (num, precision) {
		return Math.floor(num * precision) / precision;
	}


	//This interval value adjustment operation addresses two distinct issues of CanvasJS: 
	//(a) A rounding off issue. CanvasJS rounds off the ticks marks (e.g. to two decimal places), but does not round off the provided interval value itself. This can cause the last tick mark not to match the upper boundary.
	//(b) A zero-mark baseline approach. Irrespective of the Ymin value, CanvasJS always starts applying the interval value starting from the zero-mark. This can cause both the first tick mark and the last tick mark not to match the lower and upper boundaries respectively. This second issue only occurs when Ymin is not zero.
	//
	//Requirements: 
	//(i) If the algorithm modifies the YMin value (e.g. to YMinAdj) and/or the YMax value (e.g. to YMaxAdj), then YMinAdj and YMaxAdj must be so that they span YMin and YMax, i.e. YMinAdj <= YMin and YMaxAdj >= YMax
	//(ii) The number of tick mark intervals between YMinAdj and YMaxAdj must be exactly = the specified intervalCount. 
	//(iii) Between the zero-mark and YMinAdj, there must be zero or an integral number of intervals.
	//
	//The operation tries to locate the exact interval value for which the two requirements above will be met using an iterative approach.
	//Starts by using the distance between zero and YMinAdj, i.e. YMinAdj itself, as the interval value. 
	//For each iteration either divide or multiply this interval value by an increasing integral number (1, 2, 3...) depending on whether the interval value is initially too large or too small.
	//For each iteration, tests for the first requirement. Tries to match the requiremets without changing YMin.
	//Initially, YMinAdj is set as YMin rounded to the nearest lower value to two decimal places, e.g. 2.3466 becomes 2.34.
	//Initially, YMaxAdj is set as YMax rounded to the nearest upper value to two decimal places, e.g. 12.3466 becomes 12.35.
	//Unlike GetYIntervalFinerAdjustmentValues(), GetYIntervalAdjustmentValues() does not modify YMin, except for the initial rounding off, but will adjust YMax as necessary.
	//If the number of tick mark intervals between YMinAdj and YMaxAdj is exactly = intervalCount, then we have obtained the perfect interval value and can exit the iteration.
	//If the number of tick mark intervals between YMinAdj and YMaxAdj < intervalCount, it means the interval value is too big, and we keep iterating to fine a better/smaller value.
	//If the number of tick mark intervals between YMinAdj and YMaxAdj > intervalCount, it means the interval value is too small to meet the requirements. We then try to refine the results by trying to find a value between the last known good interval value and the one that was invalid, using  GetYIntervalFinerAdjustmentValues().
	var _getYIntervalAdjustmentValues = function (yMin, yMax, intervalCount) {
		var axisIntervalValues = {
			yMin: yMin,
			yMax: yMax,
			intervalValue: null
		};
		var yMinAdj = _roundDown(yMin, 10);
		var yMaxAdj = _roundUp(yMax, 10);
		var intervalValue = null;
		var prevIntervalValue = null;
		var targetIntervalValue = null;
		var i = 1;
		var maxIterations = 1000;
		var intervalTooLarge = false;

		intervalValue = _roundDown(Math.abs(yMinAdj), 10);
		yMaxAdj = yMinAdj + (intervalValue * intervalCount);
		if (yMaxAdj > yMax)
			intervalTooLarge = true;
		if (yMinAdj == 0) //We are only dealing with the first issue of CanvasJS, the rounding off issue.
		{
			intervalValue = Math.abs(yMax - yMin) / intervalCount;
			intervalValue = _roundUp(intervalValue, 10);
				targetIntervalValue = intervalValue;
		}
		else { //We need to factor in the zero-mark even if YMin is not zero.
			while (i < maxIterations) {
					if (intervalTooLarge)
						intervalValue = _roundDown((Math.abs(yMinAdj) / i), 10);  //interval value too large, need to reduce it through each iteration
					else
						intervalValue = _roundDown((Math.abs(yMinAdj) * i), 10);  //interval value too small, need to increase it through each iteration
					yMaxAdj = yMinAdj +(intervalValue * intervalCount);
					if (yMaxAdj == yMax) {
						targetIntervalValue = intervalValue;
						break;
					}
					else if ((intervalTooLarge && (yMaxAdj < yMax)) || (!intervalTooLarge && (yMaxAdj > yMax))) 
					{
						if (prevIntervalValue == null) {
							prevIntervalValue = intervalValue;
								intervalValue = _roundDown((Math.abs(yMinAdj) / (i +1)), 10);
						}
						axisIntervalRefinedValues = _getYIntervalFinerAdjustmentValues(_roundDown(yMin, 10), _roundUp(yMax, 10), intervalCount, prevIntervalValue, intervalValue);						
						if (axisIntervalRefinedValues.intervalValue != null) {
								targetIntervalValue = axisIntervalRefinedValues.intervalValue;
								yMinAdj = axisIntervalRefinedValues.yMinIndex * targetIntervalValue;
								if (yMin < 0)
									yMinAdj = yMinAdj * -1;
						}
						break;
					}
					prevIntervalValue = intervalValue;
					i++;
			}
		}
		axisIntervalValues.intervalValue = null;
		if (targetIntervalValue != null) {
			yMaxAdj = yMinAdj +(targetIntervalValue * intervalCount);
			if (yMinAdj < yMaxAdj) {
					axisIntervalValues.yMin = yMinAdj;
					axisIntervalValues.yMax = yMaxAdj;
					axisIntervalValues.intervalValue = targetIntervalValue;
			}
		}
		return axisIntervalValues;
	}


	//Try to find a more accurate interval value between IntervalValueStart and IntervalValueEnd.
	//Proceed by dividing the interval between IntervalValue1 and IntervalValue2 into a number (stepCount) of smaller intervals.
	//Start with the larger interval value and then in each iteration, decrease the interval value by the step amount.
	//Unlike the GetYIntervalAdjustmentValues() operation which tries to find an interval value between 0 and YMin, without changing YMin (even if it means changing YMax), 
	//the GetYIntervalFinerAdjustmentValues() operation ignores the YMin value, and simply adjusts the given intervalValueStart value in small steps. This means
	//that GetYIntervalFinerAdjustmentValues() could adjust both the YMinAdj and YMaxAdj values as it runs through the iterations.
	//For each small interval (step), the number of intervals between YMinAdj and YMaxAdj is verified to be still less or equal to the required interval count.
	//If not, it means that it has reached an invalid interval value, and it can stop the iteration. The last known valid interval value is the returned as the result of the process.
	var _getYIntervalFinerAdjustmentValues = function (yMin, yMax, intervalCount, intervalValue1, intervalValue2) {
		var intervalValueStart = intervalValue1;
		var intervalValueEnd = intervalValue2;
		if (intervalValue2 > intervalValue1)
		{
				intervalValueStart = intervalValue2;
				intervalValueEnd = intervalValue1;
		}
		var stepCount = 10;
		var axisIntervalRefinedValues = {
			yMinIndex: null,
			intervalValue: null
		};
		if ((yMin == null) || (yMax == null) || (yMin >= yMax) | (intervalValueStart == null) || (intervalValueEnd == null) || (intervalValueEnd > intervalValueStart))
			return axisIntervalRefinedValues;

		var step = (intervalValueStart - intervalValueEnd) / stepCount;
		var intervalValue = null;
		var prevIntervalValue = null;
		var yMinIndex = null;
		var prevYMinIndex = null;
		var yMaxIndex = null;
		var continueIteration = false;
		for (var i = 0; i < stepCount; i++) {
			intervalValue = _roundDown((intervalValueStart - (step * i)), 10);
			if ((prevIntervalValue != null) && (intervalValue == prevIntervalValue))
					continue; //the interval value increment is so small that it is rounding to the same value as that of the previous iteration. keep iterating until ther is a change in value.
			if (yMin >= 0)
					yMinIndex = Math.floor(Math.abs(yMin) / intervalValue);
			else
					yMinIndex = Math.ceil(Math.abs(yMin) / intervalValue);
			if (yMax >= 0)
					yMaxIndex = Math.ceil(Math.abs(yMax) / intervalValue);
			else
					yMaxIndex = Math.floor(Math.abs(yMax) / intervalValue);

			if ((yMin >= 0) && (yMax >= 0))
					continueIteration = ((yMaxIndex - yMinIndex) <= intervalCount)
			else if ((yMin < 0) && (yMax >= 0))
					continueIteration = ((yMinIndex + yMaxIndex) <= intervalCount)
			else //if ((yMin < 0) && (yMax < 0))
					continueIteration = ((yMinIndex - yMaxIndex) <= intervalCount)

			if (continueIteration) {
					prevIntervalValue = intervalValue;
					prevYMinIndex = yMinIndex;
					continue;
			}
			else {
					axisIntervalRefinedValues.intervalValue = prevIntervalValue;
					axisIntervalRefinedValues.yMinIndex = prevYMinIndex;
					break;
			}
		}
		return axisIntervalRefinedValues;
	}


	//Find a CanvasJS Menu button by its title attribute
	var _getCanvasJSMenuButton = function (buttonTitle) {
		var btn = null;
		$(".canvasjs-chart-toolbar").each(function (index) {
				if (!btn) {
					btn = $(this).find("[title='" + buttonTitle + "']");
					if (btn) {
						var toolbar = null;
						var container = null;
						var graph = null;
						toolbar = btn.parent();
						if (toolbar && (toolbar.length > 0))
								container = toolbar.parent();
						if (container && (container.length > 0)) {
								graph = container.parent();
						}
						if (graph && (graph.length > 0)) {
								var drawingId = graph[0].id;
								if (drawingId != FMTrendIndex.drawingIdPrefix + drawingNumber)
									btn = null;
						}
						else
								btn = null;
					}
				}
		});
		return btn;
	}



	//Click a CanvasJS Menu button identified by its title attribute
	var _clickCanvasJSMenuButton = function (buttonTitle) {
		var btn = _getCanvasJSMenuButton(buttonTitle);
		if (btn)
				btn.click();
	}



	//Return the state of the ZoomPan CanvasJS menu button. Null: Zoom/Pan button is not visible. Zoom: button is in the Zoom state. Pan: button is in the Pan state.
	var _getZoomPanButtonState = function () {
		var state = null;
		var btnTitle = "Pan"
		var btn = _getCanvasJSMenuButton(btnTitle);
		if (!btn) {
				btnTitle = "Zoom";
				btn = _getCanvasJSMenuButton(btnTitle);
		}
		if (btn) {
				var displayStyle = btn.css("display");
				if (displayStyle === "inline")
					state = btnTitle;
		}
		return state;
	}




	//Set the Zoom Type
	var _setZoomType = function (zoomType) {
		chart.set("zoomType", zoomType);
	}


	//Zoom out completely
	var _zoomOut = function () {
		if ((trend.Mode === 0) && (!inPauseMode))
				return;

		_resetViewport(false);
	}


	//Format the given Value of a pen according to the format configuration for the corresponding Point Tag
	var _formatPenValue = function (penIndex, value) {
		var formattedValue = null;
		if ((penIndex < 0) || (penIndex > (trend.Pens.length - 1)))
				return null;
		var trendPen = trend.Pens[penIndex];
		if ($('#NumberFormatInfoString')) {
				var numFormatInfoString = $('#NumberFormatInfoString').val();
				var numformatInfo = JSON.parse(numFormatInfoString);
				numformatInfo.NumberDecimalDigits = trendPen.DecimalPlaces;
				formattedValue = FMFormatValues.FormatValue(trendPen.Units, numformatInfo, value);
		}
		return formattedValue;
	}


	//Return the YValue of a dataseries (identified by its PenIndex), at a given point (identified by its data array index)
	var _getPenValueAtIndex = function (seriesIndex, pointIndex) {
		var result = null;
		var dataSource = dataPointActualFull[seriesIndex];
		if (isZoomed)
				dataSource = dataPointActualZoom[seriesIndex];
		else if ((trend.Mode == 0) && (inPauseMode))
				dataSource = dataPointActualSnapshot[seriesIndex];
		result = dataSource[pointIndex].y;
		return result;
	}


	//Return the data object of each pen in the Trend at a given xlocation
	var _getPenDataAtXLocation = function (xLocation) {
		var result = new Array();
		var firstIndex = null;
		var dataPointCollection = dataPointActualFull;
		if (isZoomed) {
			dataPointCollection = dataPointActualZoom;
		}
		else if (!inPauseMode) {
			dataPointCollection = dataPointActualFull;
		}
		else {
			dataPointCollection = dataPointActualSnapshot;
		}

		for (var i = 0; i < dataPointCollection.length; i++) {
			var penData = null;
			if (dataPointCollection[i].length > 0) {
					if ((firstIndex) && (dataPointCollection[i].length >= (firstIndex + 1)) && (dataPointCollection[i][firstIndex].x === xLocation))
						penData = dataPointCollection[i][firstIndex];
					else {
						for (var j = 0; j < dataPointCollection[i].length; j++) {
							if (dataPointCollection[i][j].x.getTime() === xLocation) {
									penData = dataPointCollection[i][j];
									if (!firstIndex)
										firstIndex = j;
									break;
							}
						}
					}
			}
			result.push(penData);
		}
		return result;
	}


	//Return the Trend Sample Rate in milliseconds
	var _getSampleRate = function () {
		var sampleRate = 1000;
		return sampleRate;
	}


	//Turn auto-scaling on/off on the secondary Y-axis
	var _setAutoSecondaryYScaling = function (autoScaling) {
		autoSecondaryYScaling = autoScaling;
		if (projectedPenIndex >= 0) {
				if (autoScaling) {
					chart.options.axisY2[0].minimum = null;
					chart.options.axisY2[0].maximum = null;
				}
				else {
					chart.options.axisY2[0].minimum = trend.Pens[projectedPenIndex].Minimum;
					chart.options.axisY2[0].maximum = trend.Pens[projectedPenIndex].Maximum;
					chart.options.axisY2[0].interval = (trend.Pens[projectedPenIndex].Maximum - trend.Pens[projectedPenIndex].Minimum) / 10;
				}
				chart.render();
		}
	}


	//Toggle the visibility of a dataseries on the graph
	var _toggleSeriesVisibility = function (seriesIndex) {
		if (seriesIndex < chart.data.length) {
				var isVisible = chart.data[seriesIndex].visible;
				chart.data[seriesIndex].set("visible", !isVisible);
		}
	}

	// Get the visibility of a dataseries on the graph
	var _getSeriesVisibility = function (seriesIndex) {
		return chart.data[seriesIndex].visible;
	}


	//Return the new content for the graph tooltip, following a Tooltip ContentFormatter event on the Graph
	var _getTooltipContent = function (e) {
		var content = '<p id="xData' + drawingNumber + '" style="display:none;">' + e.entries[0].dataPoint.x.getTime() + '</p>';
		return content;
	}


	//Add or update a vertical stripline along the X-axis of the chart to act as a timebar
	var _updateTimebar = function (xLocation) {

		if (!chart.options.axisX) {
				chart.options.axisX = {};
		 }


		 chart.options.axisX[0].labelFormatter = function (e) {
			  momentTime = FMOperateIndex.translateClientDateTimeToSiteMomentTime(e.value);
           switch (e.axis.intervalType) {
               case "second":
                   return momentTime.format("h:mm:ss a");
               case "minute":
                   return momentTime.format("h:mm a");
               case "hour":
                   return momentTime.format("h:mm a");
               case "day":
	                return momentTime.format("MMM Do YYYY");
	            case "month":
	                return momentTime.format("MMM YYYY");
           }
		 };

		var striplineColour = TIMEBAR_ACTIVE_COLOUR;
		if (timebarLocked)
				striplineColour = TIMEBAR_LOCKED_COLOUR;

		if ((!chart.axisX[0].stripLines) || (chart.axisX[0].stripLines.length === 0) || (timebarStriplineIndex < 0)) {
				chart.axisX[0].addTo("stripLines", { value: xLocation, color: striplineColour, showOnTop: true });
				timebarStriplineIndex = chart.axisX[0].stripLines.length - 1;
		 }

		 var momentTime = FMOperateIndex.translateClientDateTimeToSiteMomentTime(xLocation);
		 var timebarLabel = FMFormatValues.FormatDateTimeString(momentTime, FMOperateIndex.dateTimeFormatInfo);

		chart.axisX[0].stripLines[timebarStriplineIndex].set("value", xLocation, false);
		chart.axisX[0].stripLines[timebarStriplineIndex].set("thickness", 3, false);
		chart.axisX[0].stripLines[timebarStriplineIndex].set("color", striplineColour, false);
		chart.axisX[0].stripLines[timebarStriplineIndex].set("showOnTop", true, false);
		chart.axisX[0].stripLines[timebarStriplineIndex].set("label", timebarLabel, false);
		chart.axisX[0].stripLines[timebarStriplineIndex].set("labelPlacement", "outside", false);
		chart.axisX[0].stripLines[timebarStriplineIndex].set("labelMaxWidth", 200, false);
		chart.axisX[0].stripLines[timebarStriplineIndex].set("labelWrap", "true", false);
		chart.axisX[0].stripLines[timebarStriplineIndex].set("labelFontColor", "white", false);
		chart.axisX[0].stripLines[timebarStriplineIndex].set("labelBackgroundColor", striplineColour, false);
		chart.render();
		_updateClientsForTimebar(xLocation);
	}


	//Update all the controls that are sensitive to the position of the Timebar
	var _updateClientsForTimebar = function (xLocation) {
		var penDataCollection = new Array();
		if (!xLocation)
			return;

		if ((trend.Mode === 0) && (!inPauseMode)) {
			for (var i = 0; i < dataPointActualFull.length; i++) {
					var dataPoint = null;
					var lastIndex = 0;
					if (dataPointActualFull[i].length > 0) {
						lastIndex = dataPointActualFull[i].length - 1;
						dataPoint = dataPointActualFull[i][lastIndex];
					}
					penDataCollection.push(dataPoint);
			}
		}
		else
			penDataCollection = _getPenDataAtXLocation(xLocation);
		FMTrendIndex.updateClientsForTimebar(drawingNumber, xLocation, penDataCollection);
	}


	//Reset the timebar to the far right-hand side of the current viewport
	var _resetTimebar = function () {
		var xpos = _getLastDataPointTimeInViewport();
		_updateTimebar(xpos);
		timebarDocked = true;
		timebarLocked = true;
	}


	//Refresh the timebar
	var _refreshTimebar = function () {
		var xPos = null;  //chart.axisX[0].stripLines[timebarStriplineIndex].value;
		if (timebarDocked)
				xPos = _getLastDataPointTimeInViewport();
		else {
				xPos = timebarPrevXData;
				xPos = Number(xPos);
		}
		if (xPos)
				_updateTimebar(xPos);
	}


	//Get the datetime location of the last data point in the viewport
	var _getLastDataPointTimeInViewport = function () {
		var endTime = _getTrendCurrentEndTime();
		//Account for zooming and panning, where the last data point might not be in view
		if ((endTime < chart.axisX[0].viewportMinimum) || (endTime > chart.axisX[0].viewportMaximum)) {
				if (isFinite(chart.axisX[0].viewportMaximum)) {
					endTime = null;
					var dataPointCollection = dataPointActualFull;
					if (isZoomed)
						dataPointCollection = dataPointActualZoom;
					for (var i = 0; i < dataPointCollection.length; i++) {
						if (dataPointCollection[i].length > 0) {
								for (var j = 0; j < dataPointCollection[i].length; j++) {
									if ((endTime == null) || ((dataPointCollection[i][j].x > endTime) && (dataPointCollection[i][j].x <= chart.axisX[0].viewportMaximum)))
										endTime = dataPointCollection[i][j].x.getTime();
									if (dataPointCollection[i][j].x > chart.axisX[0].viewportMaximum)
										break;
								}
						}
					}
					if ((endTime == null) || (endTime > chart.axisX[0].viewportMaximum) || (endTime < chart.axisX[0].viewportMinimum))
						endTime = chart.axisX[0].viewportMaximum;
				}
		}
		return endTime;
	}


	//Shift the timebar to account for the scrolling effect in real-time mode. 
	//This operation does not apply to historical graphs, or real-time graphs that are in the pause mode, when the graph is not scrolling in the viewport.
	var _shiftTimebar = function () {
		var xpos = null;
		if ((trend.Mode === 0) && (inPauseMode))
				return;
		//Whenever the mouse is on the graph and the timebar is not locked, the onmousemove event will be responsible of auto-updating the timebar.
		if (!timebarLocked || inPauseMode)
				return;
		if (timebarDocked || !timebarPrevXData) {
				xpos = _getTrendCurrentEndTime();
		}
		else {
				xpos = timebarPrevXData + lastSamplingTimeElapsed;
				timebarPrevXData = xpos;
		}

		_updateTimebar(xpos);
	}


	//Update the Min/Max range of a pen
	var _updatePenMinMaxRange = function (penIndex, min, max) {
		if ((min != null) && (!isNaN(Number(min))) && (Number(min) < trend.Pens[penIndex].Maximum))
				trend.Pens[penIndex].Minimum = Number(min);
		if ((max != null) && (!isNaN(Number(max))) && (Number(max) > trend.Pens[penIndex].Minimum))
				trend.Pens[penIndex].Maximum = Number(max);
		var yValue = null;
		var yValuePercent = null;
		if (dataPointPercentageFull.length > penIndex) {
				for (var i = 0; i < dataPointPercentageFull[penIndex].length; i++) {
					yValue = dataPointActualFull[penIndex][i].y;
					yValuePercent = 100 * (yValue - trend.Pens[penIndex].Minimum) / (trend.Pens[penIndex].Maximum - trend.Pens[penIndex].Minimum);
					dataPointPercentageFull[penIndex][i].y = yValuePercent;
				}
		}
		if (dataPointPercentageSnapshot.length > penIndex) {
				for (var i = 0; i < dataPointPercentageSnapshot[penIndex].length; i++) {
					yValue = dataPointActualSnapshot[penIndex][i].y;
					yValuePercent = 100 * (yValue - trend.Pens[penIndex].Minimum) / (trend.Pens[penIndex].Maximum - trend.Pens[penIndex].Minimum);
					dataPointPercentageSnapshot[penIndex][i].y = yValuePercent;
				}
		}
		if (dataPointPercentageZoom.length > penIndex) {
				for (var i = 0; i < dataPointPercentageZoom[penIndex].length; i++) {
					yValue = dataPointActualZoom[penIndex][i].y;
					yValuePercent = 100 * (yValue - trend.Pens[penIndex].Minimum) / (trend.Pens[penIndex].Maximum - trend.Pens[penIndex].Minimum);
					dataPointPercentageZoom[penIndex][i].y = yValuePercent;
				}
		}
		var projectedPenIndex = _getSecondaryYAxisSeriesIndex();
		if (projectedPenIndex >= 0) {
				_removeSecondaryYAxis();
				_toggleYAxisForSeries(projectedPenIndex);
		}
		_refreshSecondaryYAxis();
		chart.render();
	}


	//Update the vertical timebar, following a range changing event (zoom, pan, or reset).
	var _rangeChanged = function (e) {
		if (timebarDocked || (timebarPrevXData < chart.axisX[0].viewportMinimum) || (timebarPrevXData > chart.axisX[0].viewportMaximum))
				_resetTimebar();
	}


	var _setActive = function (active) {
		isActive = active;
		if (!active) {
			if (nextDataUpdateTimeout) {
				clearTimeout(nextDataUpdateTimeout);
				nextDataUpdateTimeout = null;
			}
			if (ajaxRequest) {
				ajaxRequest.abort();
				ajaxRequest = null;
			}
		}
	}


	var _getTrend = function () {
		return trend;
	}

	var _getDrawingNumber = function () {
		return drawingNumber;
	}

	var _getSettings = function () {
		return settings;
	}


	var _refreshTrend = function () {
	    if (refreshTimerID === null) {
	        refreshTimerID = setTimeout(function () {
	            _wakeUpTrend()
	        }, 10);
	    }
	}

    
	var _wakeUpTrend = function () {
	    if (refreshTimerID !== null) {
	        refreshTimerID = null;
	        clearTimeout(refreshTimerID);
	    }
	    if (isActive === false) {
	        _updateTrendHistoricalData();
	        isActive = true;
	    }
		chart.render();
	}
    


	var _setLegendClickFunction = function (hideUnhideOn) {
		hideUnhidePenOnLegendClick = hideUnhideOn;
	}


	var _deletePen = function (penIndex) {
		if (ajaxRequest)
				ajaxRequest.abort();
		ajaxRequest = null;

		if (nextDataUpdateTimeout)
				clearTimeout(nextDataUpdateTimeout);
		nextDataUpdateTimeout = null;

		trend.Pens.splice(penIndex, 1);

		FMTrendIndex.saveTrend(trend);

		dataPointActualFull.splice(penIndex, 1);
		dataPointPercentageFull.splice(penIndex, 1);
		dataPointActualSnapshot.splice(penIndex, 1);
		dataPointPercentageSnapshot.splice(penIndex, 1);
		dataPointActualZoom.splice(penIndex, 1);
		dataPointPercentageZoom.splice(penIndex, 1);

		chart.data[penIndex].remove();
		FMOperateIndex.unsubscribeTagWebWorker(drawingNumber + trend.TrendGuid);
		if (trend.Mode === 0
		&& isActive
		&& trend.Pens.length > 0)
		{
			_updateTrendRealtimeData();
		}

	}

	var _getWindowPeriodInTicks = function () {
		if (trend.PeriodType === 0) {
				return trend.Period * 60000;
		}
		else if (trend.PeriodType === 1) {
				return trend.Period * 3600000;
		}
		else {
				return trend.Period * 86400000;
		}
	}

	var _addPen = function (pen) {
		if (trend.Pens.length > 16) {
				return;
		}

		if (ajaxRequest)
				ajaxRequest.abort();
		ajaxRequest = null;

		if (nextDataUpdateTimeout)
				clearTimeout(nextDataUpdateTimeout);
		nextDataUpdateTimeout = null;

		trend.Pens.push(pen);

		if (!FMTrendIndex.saveTrend(trend)) {
				return;
		}


		var startDate;
		var endDate;
		if (trend.Mode === 0) {
				endDate = new Date();
				if (!inPauseMode) {
					var end = FMTrendIndex.convertLocalDateToUTCDate(endDate);
					start = new Date(end.getTime() - _getWindowPeriodInTicks());
					startDate = FMTrendIndex.convertUTCDateToLocalDate(start);
				}
				else {
					startDate = realtimeSnapShotStartTime;
				}
		}
		else {
				startDate = moment(trend.Start).toDate();
				endDate = moment(trend.End).toDate();
		}

		if (trend.Pens.length == 1) {
				if (trend.Mode === 0) {
					var intervalInfo = _getIntervalDetails(trend.Period);
					chart.axisX[0].set("intervalType", intervalInfo.IntervalType);
					chart.axisX[0].set("interval", intervalInfo.Interval);
					if (!lastSamplingTime)
						lastSamplingTime = (new Date()).getTime();
					viewportInitStartDate = startDate;
				}
		}


		var tags = [];
		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };

		tags.push(pen.PointTagGuid);

		// Call controller to get archive values
		$.ajax({
				url: 'GetTrendArchiveData',
				cache: false,
				type: 'POST',
				//async: false,
				contentType: 'application/json',
				data: JSON.stringify({ tagGuids: tags, start: moment(startDate).format("YYYY-MM-DD HH:mm:ss Z"), end: moment(endDate).format("YYYY-MM-DD HH:mm:ss Z") }),
				success: function (response) {
					PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);
					FMErrorAndExceptionHandling.HandleMessages(response,
					function (data) {
						dataPointActualFull.push(new Array());
						dataPointPercentageFull.push(new Array());
						dataPointActualSnapshot.push(new Array());
						dataPointPercentageSnapshot.push(new Array());
						dataPointActualZoom.push(new Array());
						dataPointPercentageZoom.push(new Array());

						var penIndex = trend.Pens.length - 1;

						dataSeries = new Object();
						dataSeries.type = "stepLine";
						dataSeries.xValueType = "dateTime";
						dataSeries.axisYIndex = 0;
						dataSeries.name = trend.Pens[penIndex].PointID + "." + trend.Pens[penIndex].TagID;
						dataSeries.color = trend.Pens[penIndex].PenColor;
						dataSeries.lineColor = trend.Pens[penIndex].PenColor;
						dataSeries.fillOpacity = 0;
						if ((trend.Mode === 0) && (inPauseMode))
							dataSeries.dataPoints = dataPointPercentageSnapshot[penIndex];
						else
							dataSeries.dataPoints = dataPointPercentageFull[penIndex];

						chart.options.data.push(dataSeries);

						_loadArchiveData(trend.Pens[penIndex], data[0], dataPointActualFull[penIndex], dataPointPercentageFull[penIndex]);

						if (trend.Mode === 0  && inPauseMode) {
							var start = FMTrendIndex.convertLocalDateToUTCDate(realtimeSnapShotStartTime);
							var end = new Date(start.getTime() + _getWindowPeriodInTicks());
							var realTimeSnapShotMaxTime = FMTrendIndex.convertUTCDateToLocalDate(end);
							var xValue = 0;
							var yValue = 0;
							var yValuePercent = 0;


							dataPointActualFull[penIndex].forEach(function (data) {
								if (data.x <= realTimeSnapShotMaxTime) {
									xValue = moment(data.x).toDate();
									yValue = data.y;
									dataPointActualSnapshot[penIndex].push({ x: xValue, y: yValue, status: data.ValueOpcStatus, aGuid: data.aGuid, aAck: data.aAck, aState: data.aState });
								}
							});
							dataPointPercentageFull[penIndex].forEach(function (data) {
								if (data.x <= realTimeSnapShotMaxTime) {
									xValue = moment(data.x).toDate();
									yValuePercent = data.y;
									dataPointPercentageSnapshot[penIndex].push({ x: xValue, y: yValuePercent });
								}
							});
						}

						// Re-add a dummy dataseries tied to the default (primary) y-axis.
						chart.data[trend.Pens.length - 1].remove();
						chart.addTo("data", { dataPoints: [] });

						if (trend.Pens.length == 1) {
							_setHistoricalBackgroundColour(data);
						}

						if (!isZoomed) {
							chart.render();
						}
						else {

							var start = moment(chart.axisX[0].viewportMinimum).toDate();
							var end = moment(chart.axisX[0].viewportMaximum).toDate();

							var startDate = moment(start).toDate();
							var endDate = moment(end).toDate();
							var zoomPeriod = FMTrendIndex.convertLocalDateToUTCDate(endDate).getTime() - FMTrendIndex.convertLocalDateToUTCDate(startDate).getTime();
							startDate = FMTrendIndex.convertUTCDateToLocalDate(moment(FMTrendIndex.convertLocalDateToUTCDate(startDate).getTime() - 2 * zoomPeriod).toDate());
							endDate = FMTrendIndex.convertUTCDateToLocalDate(moment(FMTrendIndex.convertLocalDateToUTCDate(endDate).getTime() + 2 * zoomPeriod).toDate());


							$.ajax({
									url: 'GetTrendArchiveData',
									cache: false,
									type: 'POST',
									//async: false,
									contentType: 'application/json',
									data: JSON.stringify({ tagGuids: tags, start: moment(startDate).format("YYYY-MM-DD HH:mm:ss Z"), end: moment(endDate).format("YYYY-MM-DD HH:mm:ss Z") }),
									success: function (response) {
										PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);
										FMErrorAndExceptionHandling.HandleMessages(response,
										function (data) {
											_loadArchiveData(trend.Pens[penIndex], data[0], dataPointActualZoom[penIndex], dataPointPercentageZoom[penIndex]);
											chart.data[penIndex].set("dataPoints", dataPointPercentageZoom[penIndex], false);
											//switch to automatic interval handling
											chart.axisX[0].set("intervalType", null);
											chart.axisX[0].set("interval", null);

											chart.render();
										}, messageAttributes);
									},
									error: function (request, status, error) {
										PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);
										FMErrorAndExceptionHandling.ShowError('Failed to Read Tag Data', null, messageAttributes);
									}
							});
						}

						if (trend.Pens.length == 1) {
							_resetTimebar();
						}

					}, messageAttributes);
				},
				error: function (request, status, error) {
					// remove previous notifications
					PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);


					FMErrorAndExceptionHandling.ShowError('Failed to Read Tag Data', null, messageAttributes);
				}
		});

		if (trend.Mode === 0
		&& isActive
		&& trend.Pens.length > 0) {
				_updateTrendRealtimeData()
		}
	}

	var _editPen = function (penIndex, pointValue) {

		if (ajaxRequest)
			ajaxRequest.abort();
		ajaxRequest = null;

		if (nextDataUpdateTimeout)
			clearTimeout(nextDataUpdateTimeout);
		nextDataUpdateTimeout = null;

		var pen = trend.Pens[penIndex];

		pen.DecimalPlaces = pointValue.DecimalPlaces;
		pen.EngineeringUnitsType = pointValue.EngineeringUnitsType;
		pen.Maximum = pointValue.Maximum;
		pen.Minimum = pointValue.Minimum;
		pen.PointGuid = pointValue.PointGuid;
		pen.PointID = pointValue.PointID,
		pen.PointTagGuid = pointValue.PointValueIdentifier.IdentityGuid,
		pen.PointTemplateTagGuid = pointValue.PointTemplateTagGuid,
		pen.TagID = pointValue.ID;
		pen.Units = pointValue.Units;
		pen.UnitString = FMConvertEngUnits.GetEngineeringUnitAbbreviation(pointValue.Units)

		if (!FMTrendIndex.saveTrend(trend)) {
			return;
		}


		var startDate;
		var endDate;
		if (trend.Mode === 0) {
			endDate = new Date();
			if (!inPauseMode) {
				var end = FMTrendIndex.convertLocalDateToUTCDate(endDate);
				start = new Date(end.getTime() - _getWindowPeriodInTicks());
				startDate = FMTrendIndex.convertUTCDateToLocalDate(start);
			}
			else {
				startDate = realtimeSnapShotStartTime;
			}
		}
		else {
			startDate = moment(trend.Start).toDate();
			endDate = moment(trend.End).toDate();
		}


		var tags = [];
		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };

		tags.push(pen.PointTagGuid);

		// Call controller to get archive values
		$.ajax({
			url: 'GetTrendArchiveData',
			cache: false,
			type: 'POST',
			//async: false,
			contentType: 'application/json',
			data: JSON.stringify({ tagGuids: tags, start: moment(startDate).format("YYYY-MM-DD HH:mm:ss Z"), end: moment(endDate).format("YYYY-MM-DD HH:mm:ss Z") }),
			success: function (response) {
				PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);
				FMErrorAndExceptionHandling.HandleMessages(response,
					function (data) {

						dataPointActualFull[penIndex] = new Array();
						dataPointPercentageFull[penIndex] = new Array();
						dataPointActualSnapshot[penIndex] = new Array();
						dataPointPercentageSnapshot[penIndex] = new Array();
						dataPointActualZoom[penIndex] = new Array();
						dataPointPercentageZoom[penIndex] = new Array();

						_loadArchiveData(trend.Pens[penIndex], data[0], dataPointActualFull[penIndex], dataPointPercentageFull[penIndex]);

						if (trend.Mode === 0 && inPauseMode) {
							var start = FMTrendIndex.convertLocalDateToUTCDate(realtimeSnapShotStartTime);
							var end = new Date(start.getTime() + _getWindowPeriodInTicks());
							var realTimeSnapShotMaxTime = FMTrendIndex.convertUTCDateToLocalDate(end);
							var xValue = 0;
							var yValue = 0;
							var yValuePercent = 0;

							dataPointActualFull[penIndex].forEach(function (data) {
								if (data.x <= realTimeSnapShotMaxTime) {
									xValue = moment(data.x).toDate();
									yValue = data.y;
									dataPointActualSnapshot[penIndex].push({ x: xValue, y: yValue, status: data.ValueOpcStatus, aGuid: data.aGuid, aAck: data.aAck, aState: data.aState });
								}
							});
							dataPointPercentageFull[penIndex].forEach(function (data) {
								if (data.x <= realTimeSnapShotMaxTime) {
									xValue = moment(data.x).toDate();
									yValuePercent = data.y;
									dataPointPercentageSnapshot[penIndex].push({ x: xValue, y: yValuePercent });
								}
							});
						}

						if (!isZoomed) {

							if (penIndex === projectedPenIndex) {
								if (inPauseMode) {
									chart.data[penIndex].set("dataPoints", dataPointActualSnapshot[penIndex], false);
								}
								else {
									chart.data[penIndex].set("dataPoints", dataPointActualFull[penIndex], false);
								}
							}
							else {
								if (inPauseMode) {
									chart.data[penIndex].set("dataPoints", dataPointPercentageSnapshot[penIndex], false);
								}
								else {
									chart.data[penIndex].set("dataPoints", dataPointPercentageFull[penIndex], false);
								}
							}

							chart.render();
						}
						else {

							var start = moment(chart.axisX[0].viewportMinimum).toDate();
							var end = moment(chart.axisX[0].viewportMaximum).toDate();

							var startDate = moment(start).toDate();
							var endDate = moment(end).toDate();
							var zoomPeriod = FMTrendIndex.convertLocalDateToUTCDate(endDate).getTime() - FMTrendIndex.convertLocalDateToUTCDate(startDate).getTime();
							startDate = FMTrendIndex.convertUTCDateToLocalDate(moment(FMTrendIndex.convertLocalDateToUTCDate(startDate).getTime() - 2 * zoomPeriod).toDate());
							endDate = FMTrendIndex.convertUTCDateToLocalDate(moment(FMTrendIndex.convertLocalDateToUTCDate(endDate).getTime() + 2 * zoomPeriod).toDate());


							$.ajax({
								url: 'GetTrendArchiveData',
								cache: false,
								type: 'POST',
								//async: false,
								contentType: 'application/json',
								data: JSON.stringify({ tagGuids: tags, start: moment(startDate).format("YYYY-MM-DD HH:mm:ss Z"), end: moment(endDate).format("YYYY-MM-DD HH:mm:ss Z") }),
								success: function (response) {
									PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);
									FMErrorAndExceptionHandling.HandleMessages(response,
										function (data) {
											_loadArchiveData(trend.Pens[penIndex], data[0], dataPointActualZoom[penIndex], dataPointPercentageZoom[penIndex]);
											if (penIndex === projectedPenIndex) {
												chart.data[penIndex].set("dataPoints", dataPointActualZoom[penIndex], false)
											}
											else {
												chart.data[penIndex].set("dataPoints", dataPointPercentageZoom[penIndex], false);
											}
											chart.render();
										}, messageAttributes);
								},
								error: function (request, status, error) {
									PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);
									FMErrorAndExceptionHandling.ShowError('Failed to Read Tag Data', null, messageAttributes);
								}
							});
						}
					}, messageAttributes);
			},
			error: function (request, status, error) {
				// remove previous notifications
				PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);


				FMErrorAndExceptionHandling.ShowError('Failed to Read Tag Data', null, messageAttributes);
			}
		});

		if (trend.Mode === 0
		&& isActive
		&& trend.Pens.length > 0) {
			_updateTrendRealtimeData()
		}
	}



	var _pageLeft = function () {
		if ((trend.Mode === 0) && (!inPauseMode)) {
				return;
		}

		if (!chart) {
				return;
		}

		var delta = chart.axisX[0].viewportMaximum - chart.axisX[0].viewportMinimum;
		if ((chart.axisX[0].viewportMinimum - delta) <= chart.axisX[0].minimum) {
				chart.axisX[0].set("viewportMinimum", chart.axisX[0].minimum, false);
				chart.axisX[0].set("viewportMaximum", chart.axisX[0].minimum + delta, false);
		}
		else {
				chart.axisX[0].set("viewportMinimum", chart.axisX[0].viewportMinimum - delta, false);
				chart.axisX[0].set("viewportMaximum", chart.axisX[0].viewportMaximum - delta, false);
		}

		chart.render();

		_rangeChanged();
	}

	var _pageRight = function () {
		if ((trend.Mode === 0) && (!inPauseMode)) {
				return;
		}

		if (!chart) {
				return;
		}

		var delta = chart.axisX[0].viewportMaximum - chart.axisX[0].viewportMinimum;
		if (chart.axisX[0].maximum <= chart.axisX[0].viewportMaximum + delta) {
				chart.axisX[0].set("viewportMaximum", chart.axisX[0].maximum, false);
				chart.axisX[0].set("viewportMinimum", chart.axisX[0].maximum - delta, false);
		}
		else {
				chart.axisX[0].set("viewportMaximum", chart.axisX[0].viewportMaximum + delta, false);
				chart.axisX[0].set("viewportMinimum", chart.axisX[0].viewportMinimum + delta, false);
		}

		chart.render();

		_rangeChanged();
	}

	var _getStatus = function (status) {


	}

	var _setLineColor = function (penIndex, color) {
		trend.Pens[penIndex].PenColor = color;
		chart.data[penIndex].set("color", color, false);
		chart.data[penIndex].set("lineColor", color, false);

		var seriesIndex = _getSecondaryYAxisSeriesIndex();
		if (seriesIndex == penIndex) {
			chart.axisY2[0].set("lineColor", color, false);
		}
		chart.render();


		FMTrendIndex.saveTrend(trend);
		return;
	}

	var _getTable = function()
	{
		return table;
	};

	var _setTable = function ( tableInstance ) {
		table = tableInstance;
	};

	var _getChartDataAsArrayObject = function ()
	{
		var result = [];
		var pens = trend.Pens;

		var dataPointCollection = dataPointActualFull;
		
		if ( inPauseMode )
		{
			dataPointCollection = dataPointActualSnapshot;
		}

		if (isZoomed)
		{
			dataPointCollection = dataPointActualZoom;
		}

		var numPens = dataPointCollection.length;

		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);

		for (var penIdx = 0; penIdx < numPens; penIdx++)
		{
			for (var timeEntry = 0; timeEntry < dataPointCollection[penIdx].length; timeEntry++)
			{
				var timestamp = dataPointCollection[penIdx][timeEntry].x;

				var foundTimeEntry = foundTimeEntry = $.grep(result, function (n, i) {
					return n.TimeStamp.valueOf() === timestamp.valueOf();
					});

				numformatInfo.NumberDecimalDigits = pens[penIdx].DecimalPlaces;
				var restrictedTest = $('#RestrictedText').val();
				var communicationsFailureText = $('#CommunicationsFailureTexst').val();
				var formattedValue;
				if (dataPointCollection[penIdx][timeEntry].y === restrictedTest) {
					formattedValue = restrictedTest;
				}
				else if (dataPointCollection[penIdx][timeEntry].y === communicationsFailureText) {
					formattedValue = communicationsFailureText;
				}
				else if (typeof dataPointCollection[penIdx][timeEntry].y == 'string') {
					formattedValue = dataPointCollection[penIdx][timeEntry].y;
				}
				else {
					formattedValue = dataPointCollection[penIdx][timeEntry].y === null ? "" : FMFormatValues.FormatValue(pens[penIdx].Units, numformatInfo, dataPointCollection[penIdx][timeEntry].y);
				}

				var annunciatedValue;


				if (dataPointCollection[penIdx][timeEntry].aGuid !== "" && dataPointCollection[penIdx][timeEntry].y !== null) {
					var cellStyle = "<div style='padding-left: 3px; padding-right: 3px;' class='AlarmPriority-" + (dataPointCollection[penIdx][timeEntry].aAck ? "" : "blink-") + dataPointCollection[penIdx][timeEntry].aGuid + "' title='" + (!!dataPointCollection[penIdx][timeEntry].aState ? dataPointCollection[penIdx][timeEntry].aState : "") + "'>";
					annunciatedValue = cellStyle + formattedValue + "</div>";
				}
				else {
					annunciatedValue = "<div style='padding-left: 3px; padding-right: 3px;' title=''>" + formattedValue + "</div>";
				}

				var status;
				if (dataPointCollection[penIdx][timeEntry].y === restrictedTest) {
					status = restrictedTest;
				}
				else if (dataPointCollection[penIdx][timeEntry].y === communicationsFailureText) {
					status = communicationsFailureText;
				}
				else {
					status = FMOperateIndex.GetStatusCode(dataPointCollection[penIdx][timeEntry].status);
				}

				if (foundTimeEntry.length > 0)
				{
					foundTimeEntry[0][pens[penIdx].PointID + '.' + pens[penIdx].TagID + '.Value'] = annunciatedValue;
					foundTimeEntry[0][pens[penIdx].PointID + '.' + pens[penIdx].TagID + '.Status'] = status;
				}
				else
				{
					var newEntry = {};
					newEntry.TimeStamp = timestamp;
					newEntry[pens[penIdx].PointID + '.' + pens[penIdx].TagID + '.Value'] = annunciatedValue;
					newEntry[pens[penIdx].PointID + '.' + pens[penIdx].TagID + '.Status'] = status;
					result.push(newEntry);
				}
			}
		}

		// sort the data by date using moment.js
		result.sort(function (left, right)
		{
			return moment.utc( left.TimeStamp ).diff( moment.utc( right.TimeStamp ) );
		});

		 result.forEach(function (entry) {
			  var momentTime = FMOperateIndex.translateClientDateTimeToSiteMomentTime(entry.TimeStamp);
			entry.TimeStamp = FMFormatValues.FormatDateTimeString(momentTime, FMOperateIndex.dateTimeFormatInfo);
		});



		return result;
	}

	return {
		LoadTrend: _loadTrend,
		ReloadTrend: _reloadTrend,
		SetActive: _setActive,
		GetDrawingNumber: _getDrawingNumber,
		GetTrend: _getTrend,
		RefreshTrend: _refreshTrend,
		PauseTrend: _pauseTrend,
		ResumeTrend: _resumeTrend,
		SetLegendClickFunction: _setLegendClickFunction,
		SetAutoSecondaryYScaling: _setAutoSecondaryYScaling,
		SetZoomType: _setZoomType,
		ZoomOut: _zoomOut,
		ToggleSeriesVisibility: _toggleSeriesVisibility,
		ToggleYAxisForSeries: _toggleYAxisForSeries,
		ClickCanvasJSMenuButton: _clickCanvasJSMenuButton,
		GetZoomPanButtonState: _getZoomPanButtonState,
		GetTooltipContent: _getTooltipContent,
		UpdateTimebar: _updateTimebar,
		RefreshTimebar: _refreshTimebar,
		DeletePen: _deletePen,
		AddPen: _addPen,
		GetSecondaryYAxisSeriesIndex: _getSecondaryYAxisSeriesIndex,
		RemoveSecondaryYAxis: _removeSecondaryYAxis,
		GetSeriesVisibility: _getSeriesVisibility,
		UpdatePenMinMaxRange: _updatePenMinMaxRange,
		PageLeft: _pageLeft,
		PageRight: _pageRight,
		SetLineColor: _setLineColor,
		EditPen: _editPen,
		GetSettings: _getSettings,
		GetChartDataAsArrayObject: _getChartDataAsArrayObject,
		GetTable: _getTable,
		SetTable: _setTable
	};


});
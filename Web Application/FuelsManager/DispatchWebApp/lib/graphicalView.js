/// <reference path="lib/jquery-1.7.1.js" />
/// <reference path="lib/canvasExtensions.js" />
/// <reference path="lib/dispatch.js" />

// The  graphical view scope object.  Variables and functions specific to the graphical view page
// should be added to this object rather than the global windows object.
var GraphicalViewLib = {};
GraphicalViewLib.securityToken = '';

// Variables
GraphicalViewLib.diagonalCrossHatch = {};
GraphicalViewLib.displayRectWidth = 0;
GraphicalViewLib.hourStops = [];
GraphicalViewLib.lastAnimationFrameTime = new Date();
GraphicalViewLib.loadingLocations = [];
GraphicalViewLib.locationInterval = 0;
GraphicalViewLib.numberOfHours = 7;
GraphicalViewLib.scale = 1.0;
GraphicalViewLib.shapeArray = [];
GraphicalViewLib.startingHour = 12;

// Constants
GraphicalViewLib.LABEL_MARGIN = 70;
GraphicalViewLib.TICK_MARK_SIZE = 7;
GraphicalViewLib.TIME_MARGIN = 30;
GraphicalViewLib.TOOLBAR_MARGIN = 40;
GraphicalViewLib.MAXIMUM_HEIGHT = 250;

// Enum Status
GraphicalViewLib.STATUS = {
	PENDING: { value: 0, colorStop1: "#00C8C8", colorStop2: "#00FFFF" },
	DISPATCHED: { value: 1, colorStop1: "#00C800", colorStop2: "#00FF00" },
	OVERDUE: { value: 2, colorStop1: "#C80000", colorStop2: "#FF0000" },
	COMPLETE: { value: 3, colorStop1: "#969696", colorStop2: "#C8C8C8" },
	CANCELLED: { value: 4, colorStop1: "#9F00C5", colorStop2: "#C700E3" },
	OUTSTANDINGNORESOURCEALLOCATED: { value: 5, colorStop1: "#FFDC00", colorStop2: "#FFFFFF" },
	OUTSTANDINGDISPATCHED: { value: 6, colorStop1: "#FF8C00", colorStop2: "#FFCD00" }
};

// class GraphicalViewLib.Shape
GraphicalViewLib.Shape = function() {
	this.color = "#FFFFFF";
	this.x = 0;
	this.y = 0;
	this.width = 75;
	this.fullWidth = this.width + 50;
	this.height = 150;
	this.status = GraphicalViewLib.STATUS.PENDING;
	this.flightNumber = "undefined";
	this.loadingLocation = 'A03';
	this.fuelLoadIndicator = false;
	this.updateIndicator = false;
	this.visible = true;
	this.findFlag = false;
	this.overlapped = false;
};

// class GraphicalViewLib.LoadingLocation
GraphicalViewLib.LoadingLocation = function() {
	this.ID = '';
	this.graphicalY = 0;
};

GraphicalViewLib.graphicalPageLoad = function() {
	window.FMMenuBarLib.showFullScreenButton();
	window.FMMenuBarLib.onSizeChanged = GraphicalViewLib.resizeGraphicalView;

	GraphicalViewLib.initDemo();

	GraphicalViewLib.diagonalCrossHatch = new Image();
	GraphicalViewLib.diagonalCrossHatch.src = "images/diagonal.gif";

	// Bind the window resize event to GraphicalViewLib.resizeGraphicalView() function
	$(window).resize(function () { GraphicalViewLib.resizeGraphicalView(); });

	GraphicalViewLib.resizeGraphicalView();

	$('#graphicalCanvas')[0].className = 'mainCanvas';
};

GraphicalViewLib.resizeGraphicalView = function () {
	var panelElem = $('#graphicalViewPanel');
	if (panelElem) {
		// Limit minimum panel width to the width of menu header bar
		var headerBarWidth = window.FMMenuBarLib.headerBarWidth();
		var widthoffset = window.FMMenuBarLib.inFullScreenMode ? 10 : 15;
		var panelWidth = Math.max($(window).width() - widthoffset, headerBarWidth - widthoffset);
		panelElem.width(panelWidth);

		// Limit minimum panel height to 300 pixels
		var menuBarHeight = window.FMMenuBarLib.clientHeight();
		var heightoffset = menuBarHeight + (window.FMMenuBarLib.inFullScreenMode ? 25 : 10);
		var panelHeight = Math.max($(window).height() - heightoffset, 300);
		panelElem.height(panelHeight);

		var graphicalCanvas = $('#graphicalCanvas')[0];
		if (graphicalCanvas) {
			graphicalCanvas.width = panelWidth;
			var toolbarHeight = $('#toolBarGraphical').height();
			graphicalCanvas.height = panelHeight - toolbarHeight + 5;
			GraphicalViewLib.redrawGraphicalView();
		}
	}
};

GraphicalViewLib.initDemo = function() {
	var loc = new GraphicalViewLib.LoadingLocation();
	loc.ID = 'A01';
	GraphicalViewLib.loadingLocations.add(loc);

	loc = new GraphicalViewLib.LoadingLocation();
	loc.ID = 'A03';
	GraphicalViewLib.loadingLocations.add(loc);

	loc = new GraphicalViewLib.LoadingLocation();
	loc.ID = 'A05';
	GraphicalViewLib.loadingLocations.add(loc);

	loc = new GraphicalViewLib.LoadingLocation();
	loc.ID = 'A07';
	GraphicalViewLib.loadingLocations.add(loc);

	loc = new GraphicalViewLib.LoadingLocation();
	loc.ID = 'A09';
	GraphicalViewLib.loadingLocations.add(loc);

	GraphicalViewLib.initDrawingTest();
};

GraphicalViewLib.redrawGraphicalView = function() {
	var graphicalCanvas = $('#graphicalCanvas')[0];
	var context = graphicalCanvas.getContext('2d');

	// Start buffering canvas
	var bufferCanvas = document.createElement("canvas");
	var bufferCanvasCtx = bufferCanvas.getContext("2d");
	bufferCanvasCtx.canvas.width = context.canvas.width;
	bufferCanvasCtx.canvas.height = context.canvas.height;

	// Scale the buffer to the current scaling value
	bufferCanvasCtx.scale(GraphicalViewLib.scale, GraphicalViewLib.scale);

	// Draw display on buffering canvas
	GraphicalViewLib.drawGraphicalView(bufferCanvasCtx);
};

GraphicalViewLib.activateDrawingBuffer = function(bufferCanvasCtx) {
	var graphicalCanvas = $('#graphicalCanvas')[0];
	var context = graphicalCanvas.getContext('2d');

	// Activate the buffered content
	window.CanvasExtensions.clearCanvas(context);
	context.drawImage(bufferCanvasCtx.canvas, 0, 0, bufferCanvasCtx.canvas.width, bufferCanvasCtx.canvas.height);
};

GraphicalViewLib.drawGraphicalView = function(context) {
	// Draw the main box around the graphical view area
	context.lineWidth = 1;
	context.strokeStyle = 'black';
	context.fillStyle = '#EEEEEE';

	GraphicalViewLib.displayRectWidth = $('#graphicalViewPanel').width() - GraphicalViewLib.LABEL_MARGIN - 20;
	var interval = GraphicalViewLib.displayRectWidth / GraphicalViewLib.numberOfHours;
	var height = $('#graphicalViewPanel').height() - GraphicalViewLib.TIME_MARGIN - 50;

	// Draw main service schedule background
	context.save();
	var gradient2 = context.createLinearGradient(0, GraphicalViewLib.TIME_MARGIN, 0, height);
	gradient2.addColorStop(0, "#F5F5F5");
	gradient2.addColorStop(1, "#DDDDDD");
	context.fillStyle = gradient2;
	window.CanvasExtensions.roundRect(context, GraphicalViewLib.LABEL_MARGIN, GraphicalViewLib.TIME_MARGIN, GraphicalViewLib.displayRectWidth, height);
	context.restore();

	var hourStop = 0;
	GraphicalViewLib.hourStops = [];
	GraphicalViewLib.hourStops[0] = hourStop;

	// Draw the move left button
	var buttonTop = GraphicalViewLib.TIME_MARGIN - 30;
	var buttonLeft = GraphicalViewLib.LABEL_MARGIN;
	var buttonWidth = 25;
	var buttonHeight = 20;

	window.CanvasExtensions.roundRect(context, buttonLeft, buttonTop, buttonWidth, buttonHeight);
	context.save();
	context.fillStyle = '#777';
	context.beginPath();
	context.moveTo(buttonLeft + 2, GraphicalViewLib.TIME_MARGIN - 30 + 10);
	context.lineTo(buttonLeft + buttonWidth - 4, 2);
	context.lineTo(buttonLeft + buttonWidth - 4, buttonHeight - 4);
	context.closePath();
	context.fill();
	context.restore();
	
	// Draw the move right button
	buttonTop = GraphicalViewLib.TIME_MARGIN - 30;
	buttonLeft = GraphicalViewLib.displayRectWidth + 30;
	buttonWidth = 25;
	buttonHeight = 20;

	window.CanvasExtensions.roundRect(context, buttonLeft, buttonTop, buttonWidth, buttonHeight);
	context.save();
	context.fillStyle = '#777';
	context.beginPath();
	context.moveTo(buttonLeft + buttonWidth - 2, GraphicalViewLib.TIME_MARGIN - 30 + 10);
	context.lineTo(buttonLeft + 4, 2);
	context.lineTo(buttonLeft + 4, buttonHeight - 4);
	context.closePath();
	context.fill();
	context.restore();

	// Draw the time line
	for (var i = 1; i < GraphicalViewLib.numberOfHours; ++i) {
		hourStop = GraphicalViewLib.LABEL_MARGIN + (i * interval);
		GraphicalViewLib.hourStops[i] = hourStop;

		// Draw the half-hour line
		context.fillStyle = "red";
		context.fillRect(hourStop - (interval / 2), (GraphicalViewLib.TIME_MARGIN - GraphicalViewLib.TICK_MARK_SIZE), 1, GraphicalViewLib.TICK_MARK_SIZE);
		context.fillRect(hourStop - (interval / 2), (GraphicalViewLib.TIME_MARGIN + height), 1, GraphicalViewLib.TICK_MARK_SIZE);

		// Draw the tick mark for the time stop
		context.fillStyle = "black";
		context.fillRect(hourStop, (GraphicalViewLib.TIME_MARGIN - GraphicalViewLib.TICK_MARK_SIZE), 1, GraphicalViewLib.TICK_MARK_SIZE);
		context.fillRect(hourStop, GraphicalViewLib.TIME_MARGIN + height, 1, GraphicalViewLib.TICK_MARK_SIZE);

		var currentHour = (GraphicalViewLib.startingHour - 1 + i) % 24;

		var text = currentHour + ':00';
		if (currentHour < 10) {
			text = '0' + text;
		}

		context.font = '10pt Arial';
		var x = hourStop - (context.measureText(text).width / 2);

		context.fillText(text, x, (GraphicalViewLib.TIME_MARGIN - GraphicalViewLib.TICK_MARK_SIZE) - 5);
		context.fillText(text, x, (GraphicalViewLib.TIME_MARGIN + height + GraphicalViewLib.TICK_MARK_SIZE) + 15);

		// TODO: Display midnight with a date value instead of 00:00
	}

	// Draw the final half-hour line
	context.fillStyle = "red";
	context.fillRect(hourStop + (interval / 2), (GraphicalViewLib.TIME_MARGIN - GraphicalViewLib.TICK_MARK_SIZE), 1, GraphicalViewLib.TICK_MARK_SIZE);
	context.fillRect(hourStop + (interval / 2), (GraphicalViewLib.TIME_MARGIN + height), 1, GraphicalViewLib.TICK_MARK_SIZE);

	// Draw the loading locations markings
	GraphicalViewLib.drawLoadingLocations(context);

	GraphicalViewLib.drawShapes(context, height);
};

GraphicalViewLib.drawLoadingLocations = function(context) {
	var numberOfLocations = GraphicalViewLib.loadingLocations.length;
	var labelColor = '#0D246A';

	if (numberOfLocations > 0) {
		var height = ($('#graphicalViewPanel').height() - GraphicalViewLib.TIME_MARGIN - 50);
		GraphicalViewLib.locationInterval = Math.min(height / numberOfLocations, GraphicalViewLib.MAXIMUM_HEIGHT);
		var halfInterval = GraphicalViewLib.locationInterval / 2;
		var tickMarkLocation = GraphicalViewLib.TIME_MARGIN;

		// Save the location for use by other drawing items1
		GraphicalViewLib.loadingLocations[0].graphicalY = tickMarkLocation;

		// Draw the text labeling area background
		context.fillStyle = labelColor;
		window.CanvasExtensions.roundRect(context, 0, tickMarkLocation + 2, GraphicalViewLib.LABEL_MARGIN - 10, GraphicalViewLib.locationInterval - 4, 5, true, 0);

		// Label the location
		context.fillStyle = "white";
		var y = tickMarkLocation + halfInterval + 5;
		var x = ((GraphicalViewLib.LABEL_MARGIN - 10) / 2) - (context.measureText(GraphicalViewLib.loadingLocations[0].ID).width / 2);
		context.fillText(GraphicalViewLib.loadingLocations[0].ID, x, y);

		for (var loop = 1; loop < numberOfLocations; ++loop) {
			// Calculate the next location
			tickMarkLocation += GraphicalViewLib.locationInterval;
			GraphicalViewLib.loadingLocations[loop].graphicalY = tickMarkLocation;

			// Draw the grid lines on the display
			context.beginPath();
			context.strokeStyle = "#000088";
			context.moveTo(GraphicalViewLib.LABEL_MARGIN, tickMarkLocation);
			context.lineTo(GraphicalViewLib.LABEL_MARGIN + GraphicalViewLib.displayRectWidth - 2, tickMarkLocation);
			//context.dashedLineTo(GraphicalViewLib.LABEL_MARGIN, tickMarkLocation, GraphicalViewLib.LABEL_MARGIN + GraphicalViewLib.displayRectWidth - 2, tickMarkLocation, [5, 5]);
			context.stroke();

			// Draw the text labeling area background
			context.fillStyle = labelColor;
			window.CanvasExtensions.roundRect(context, 0, tickMarkLocation + 2, GraphicalViewLib.LABEL_MARGIN - 10, GraphicalViewLib.locationInterval - 4, 5, true, 0);

			// Label the location
			x = ((GraphicalViewLib.LABEL_MARGIN - 10) / 2) - (context.measureText(GraphicalViewLib.loadingLocations[loop].ID).width / 2);
			y = tickMarkLocation + halfInterval + 5;
			context.fillStyle = "white";
			context.fillText(GraphicalViewLib.loadingLocations[loop].ID, x, y);
		}

		// Draw the bottom line if necessary
		if (GraphicalViewLib.locationInterval == GraphicalViewLib.MAXIMUM_HEIGHT) {
			tickMarkLocation += GraphicalViewLib.locationInterval;
			context.beginPath();
			context.strokeStyle = "#000088";
			context.moveTo(GraphicalViewLib.LABEL_MARGIN, tickMarkLocation);
			context.lineTo(GraphicalViewLib.LABEL_MARGIN + GraphicalViewLib.displayRectWidth - 2, tickMarkLocation);
			context.stroke();
		}

	}
};

GraphicalViewLib.drawShapes = function (context, height)
{
	var crossHatch = new Image();

	crossHatch.onload = function() {
		GraphicalViewLib.shapeArray.forEach(function (shape) {
			GraphicalViewLib.lookupYLocation(shape);

			if (shape.visible) {
				GraphicalViewLib.drawShape(context, shape, GraphicalViewLib.diagonalCrossHatch);
			}
		});

		// Draw example current time indicator
		context.beginPath();
		context.strokeStyle = '#00AA00';
		context.lineWidth = 1;
		context.moveTo(550, GraphicalViewLib.TIME_MARGIN);
		context.lineTo(550, GraphicalViewLib.TIME_MARGIN + height);
		context.stroke();
		
		GraphicalViewLib.activateDrawingBuffer(context);
	};

	crossHatch.src = 'images/diagonal.gif';
};

GraphicalViewLib.lookupYLocation = function (shape)
{
	shape.visible = false;

	for (var loop = 0; loop < GraphicalViewLib.loadingLocations.length; ++loop) {
		if (GraphicalViewLib.loadingLocations[loop].ID == shape.loadingLocation) {
			shape.visible = true;
			shape.y = GraphicalViewLib.loadingLocations[loop].graphicalY + 2;
			shape.height = GraphicalViewLib.locationInterval - 4;

			// TODO: Put in real value based on time
			return;
		}
	}
};

GraphicalViewLib.drawShape = function (context, shape, imageObj)
{
	if (context) {
		
		if (shape.findFlag && !GraphicalViewLib.animateFind) {
			return;
		}

		// Render time on ground area
		context.lineWidth = 1;
		context.fillStyle = "#FFF";
		context.globalAlpha = 0.2;
		window.CanvasExtensions.roundRect(context, shape.x - shape.fullWidth, shape.y, shape.width + shape.fullWidth, shape.height, 11, true, 3);

		context.fillStyle = context.createPattern(imageObj, "repeat");
		window.CanvasExtensions.roundRect(context, shape.x - shape.fullWidth, shape.y, shape.width + shape.fullWidth, shape.height, 11, true, 3);

		context.lineWidth = 1;
		context.fillStyle = "#FFF";

		// Set up shadowing
		context.globalAlpha = 1.0;
		context.shadowOffsetX = 3;
		context.shadowOffsetY = 3;
		context.shadowBlur = 4;
		context.shadowColor = 'rgba(0, 0, 0, 0.3)';
		context.strokeStyle = '#000';
		context.lineWidth = 2;

		// Create fueling service gradient color
		var gradient2 = context.createLinearGradient(0, shape.y, 0, shape.y + shape.height);
		gradient2.addColorStop(0, shape.status.colorStop1);
		gradient2.addColorStop(1, shape.status.colorStop2);
		context.fillStyle = gradient2;

		// Draw main flight rectangle
		if (shape.overlapped) {
			context.strokeStyle = '#00F';
			context.lineWidth = 6;
		}

		window.CanvasExtensions.roundRect(context, shape.x, shape.y, shape.width, shape.height, 11, true, 1);

		// Draw flight number text
		context.strokeStyle = '#000';
		context.lineWidth = 2;
		context.shadowColor = "transparent";
		context.font = "bold 11pt Arial";
		context.fillStyle = "#FFF";
		context.fillText(shape.flightNumber, shape.x + 3, shape.y + 17);

		// Fuel Load Indicator
		var fuelX = shape.x + shape.width - 8;
		var fuelY = shape.y + 2;
		var fuelSize = 20;
		var fuelInterval = 15;

		if (shape.fuelLoadIndicator) {
			context.beginPath();
			context.fillStyle = "#00CC00";
			context.strokeStyle = "#000";
			context.lineWidth = 1;
			context.moveTo(fuelX, fuelY);
			context.lineTo(fuelX + fuelSize, fuelY + (fuelSize / 2));
			context.lineTo(fuelX, fuelY + fuelSize);
			context.lineTo(fuelX, fuelY);
			context.fill();
			context.stroke();
		}

		// Update Indicator
		if (shape.updateIndicator)
		{
			GraphicalViewLib.drawUpdateIndicator(context, fuelX, shape, fuelSize, 'T');
			fuelX += fuelInterval;

			GraphicalViewLib.drawUpdateIndicator(context, fuelX, shape, fuelSize, 'D');
			fuelX += fuelInterval;

			GraphicalViewLib.drawUpdateIndicator(context, fuelX, shape, fuelSize, 'A');
			fuelX += fuelInterval;

			GraphicalViewLib.drawUpdateIndicator(context, fuelX, shape, fuelSize, 'R');
			fuelX += fuelInterval;

			GraphicalViewLib.drawUpdateIndicator(context, fuelX, shape, fuelSize, 'L');
			fuelX += fuelInterval;

			GraphicalViewLib.drawUpdateIndicator(context, fuelX, shape, fuelSize, 'U');
		}
	}
};

GraphicalViewLib.drawUpdateIndicator = function (context, fuelX, shape, fuelSize, letter)
{
	context.beginPath();
	context.fillStyle = "#CC0000";
	context.strokeStyle = "#000";
	context.lineWidth = 1;
	var fuelY = shape.y + shape.height - fuelSize - 2;
	context.moveTo(fuelX, fuelY);
	context.lineTo(fuelX + fuelSize, fuelY + (fuelSize / 2));
	context.lineTo(fuelX, fuelY + fuelSize);
	context.lineTo(fuelX, fuelY);
	context.fill();
	context.stroke();
	
	context.font = "bold 9pt Arial";
	context.fillStyle = "#EEE";
	context.fillText(letter, fuelX + 1, fuelY + fuelSize - 5);
};

GraphicalViewLib.initDrawingTest = function() {
	GraphicalViewLib.scale = 1.0;

	GraphicalViewLib.shapeArray = [];

	var testShape = new GraphicalViewLib.Shape();
	testShape.status = GraphicalViewLib.STATUS.DISPATCHED;
	testShape.flightNumber = "1111";
	testShape.loadingLocation = 'A03';
	testShape.x = 600;
	testShape.findFlag = true;
	GraphicalViewLib.shapeArray.add(testShape);

	testShape = new GraphicalViewLib.Shape();
	testShape.status = GraphicalViewLib.STATUS.OUTSTANDINGDISPATCHED;
	testShape.flightNumber = "1112";
	testShape.loadingLocation = 'A07';
	testShape.x = 740;
	testShape.updateIndicator = true;
	GraphicalViewLib.shapeArray.add(testShape);

	testShape = new GraphicalViewLib.Shape();
	testShape.status = GraphicalViewLib.STATUS.COMPLETE;
	testShape.flightNumber = "1114";
	testShape.loadingLocation = 'A05';
	testShape.x = 350;
	GraphicalViewLib.shapeArray.add(testShape);

	testShape = new GraphicalViewLib.Shape();
	testShape.status = GraphicalViewLib.STATUS.CANCELLED;
	testShape.flightNumber = "1113";
	testShape.loadingLocation = 'A05';
	testShape.x = 500;
	GraphicalViewLib.shapeArray.add(testShape);

	testShape = new GraphicalViewLib.Shape();
	testShape.status = GraphicalViewLib.STATUS.PENDING;
	testShape.flightNumber = "1115";
	testShape.loadingLocation = 'A09';
	testShape.x = 525;
	testShape.overlapped = true;
	GraphicalViewLib.shapeArray.add(testShape);

	testShape = new GraphicalViewLib.Shape();
	testShape.status = GraphicalViewLib.STATUS.OVERDUE;
	testShape.flightNumber = "1116";
	testShape.loadingLocation = 'A01';
	testShape.x = 625;
	testShape.fuelLoadIndicator = false;
	GraphicalViewLib.shapeArray.add(testShape);

	testShape = new GraphicalViewLib.Shape();
	testShape.status = GraphicalViewLib.STATUS.COMPLETE;
	testShape.flightNumber = "1117";
	testShape.loadingLocation = 'A01';
	testShape.x = 300;
	GraphicalViewLib.shapeArray.add(testShape);

	testShape = new GraphicalViewLib.Shape();
	testShape.status = GraphicalViewLib.STATUS.COMPLETE;
	testShape.flightNumber = "1118";
	testShape.loadingLocation = 'A03';
	testShape.x = 280;
	testShape.overlapped = true;
	GraphicalViewLib.shapeArray.add(testShape);

	testShape = new GraphicalViewLib.Shape();
	testShape.status = GraphicalViewLib.STATUS.COMPLETE;
	testShape.flightNumber = "1119";
	testShape.loadingLocation = 'A07';
	testShape.x = 320;
	GraphicalViewLib.shapeArray.add(testShape);

	testShape = new GraphicalViewLib.Shape();
	testShape.status = GraphicalViewLib.STATUS.COMPLETE;
	testShape.flightNumber = "1120";
	testShape.loadingLocation = 'A09';
	testShape.x = 270;
	GraphicalViewLib.shapeArray.add(testShape);
};

GraphicalViewLib.refreshGraphical = function() {
	// TODO: put in actual event handler

	GraphicalViewLib.shapeArray.forEach(function (shape) {
		shape.updateIndicator = !shape.updateIndicator;
	});

	GraphicalViewLib.redrawGraphicalView();
};

GraphicalViewLib.addLocation = function() {
	var loc = new GraphicalViewLib.LoadingLocation();
	loc.ID = 'A' + (GraphicalViewLib.loadingLocations.length + 1);
	GraphicalViewLib.loadingLocations.add(loc);

	// Add a demo shape
	var shape = new GraphicalViewLib.Shape();
	shape.flightNumber = '' + (1100 + GraphicalViewLib.loadingLocations.length);
	shape.loadingLocation = loc.ID;
	shape.x = 200 + (Math.random() * 900);

	shape.status = GraphicalViewLib.STATUS.DISPATCHED;
	if (shape.x < 500) {
		shape.status = GraphicalViewLib.STATUS.COMPLETE;
	} else if (shape.x > 750) {
		shape.status = GraphicalViewLib.STATUS.PENDING;
	}

	GraphicalViewLib.shapeArray.add(shape);

	GraphicalViewLib.redrawGraphicalView();
};

GraphicalViewLib.subtractLocation = function() {
	GraphicalViewLib.loadingLocations.splice(GraphicalViewLib.loadingLocations.length - 1, 1);
	GraphicalViewLib.redrawGraphicalView();
};

GraphicalViewLib.animateFind = true;

GraphicalViewLib.animate = function() {
	var now = new Date();

	var checkInterval = 100;
	if (GraphicalViewLib.animateFind) {
		checkInterval = 500;
	}

	if (now - GraphicalViewLib.lastAnimationFrameTime > checkInterval) {
		GraphicalViewLib.lastAnimationFrameTime = now;

		// Draw
		/*
		for (var i = 0; i < GraphicalViewLib.shapeArray.length; ++i) {
			var item = GraphicalViewLib.shapeArray[i];
			item.updateIndicator = !item.updateIndicator;
		}
		*/
		
		GraphicalViewLib.redrawGraphicalView();

		GraphicalViewLib.animateFind = !GraphicalViewLib.animateFind;
	}

	// Request new frame
	window.requestAnimFrame(function() {
		GraphicalViewLib.animate();
	});
};

// Called when the Add Locations button is clicked.
GraphicalViewLib.AddLocationsButtonOnClick = function() {
	GraphicalViewLib.addLocation();
};

// Called when the Add One Hour button is clicked.
GraphicalViewLib.AddOneHourButtonOnClick = function() {
	//alert("GraphicalViewLib.AddOneHourButtonOnClick() called");
	GraphicalViewLib.animate();
};

// Called when the Add Three Hours button is clicked.
GraphicalViewLib.AddThreeHoursButtonOnClick = function() {
	alert("GraphicalViewLib.AddThreeHoursButtonOnClick() called");
};

// Called when the Add Six Hours button is clicked.
GraphicalViewLib.AddSixHoursButtonOnClick = function() {
	alert("GraphicalViewLib.AddSixHoursButtonOnClick() called");
};

// Called when the Decrease Scale button is clicked.
GraphicalViewLib.DecreaseScaleButtonOnClick = function() {
	alert("GraphicalViewLib.DecreaseScaleButtonOnClick() called");
};

// Called when the Flight Changes button is clicked.
GraphicalViewLib.FlightChangesButtonOnClick = function() {
	alert("GraphicalViewLib.FlightChangesButtonOnClick() called");
};

// Called when the Increase Scale button is clicked.
GraphicalViewLib.IncreaseScaleButtonOnClick = function() {
	alert("GraphicalViewLib.IncreaseScaleButtonOnClick() called");
};

// Called when the Operator Log button is clicked.
GraphicalViewLib.OperatorLogButtonOnClick = function() {
	alert("GraphicalViewLib.OperatorLogButtonOnClick() called");
};

// Called when the Refresh button is clicked.
GraphicalViewLib.RefreshButtonOnClick = function () {
	GraphicalViewLib.refreshGraphical();
};

// Called when the Remove Locations button is clicked.
GraphicalViewLib.RemoveLocationsButtonOnClick = function() {
	GraphicalViewLib.subtractLocation();
};

// Called when the Subtract One Hour button is clicked.
GraphicalViewLib.SubtractOneHourButtonOnClick = function() {
	alert("GraphicalViewLib.SubtractOneHourButtonOnClick() called");
};

// Called when the Subtract Three Hours button is clicked.
GraphicalViewLib.SubtractThreeHoursButtonOnClick = function() {
	alert("GraphicalViewLib.SubtractThreeHoursButtonOnClick() called");
};

// Called when the Subtract Six Hours button is clicked.
GraphicalViewLib.SubtractSixHoursButtonOnClick = function() {
	alert("GraphicalViewLib.SubtractSixHoursButtonOnClick() called");
};

// Called when the Tabular View button is clicked.
GraphicalViewLib.TabularViewButtonOnClick = function() {
	window.window_location_assign("TabularView.aspx");
};

// Called when a transaction alias button is clicked.
GraphicalViewLib.TransactionAliasButtonOnClick = function(aliasId) {
	alert("GraphicalViewLib.TransactionAliasButtonOnClick(" + aliasId + ") called");
};

// Called when the Dispatchers List button is clicked.
GraphicalViewLib.DispatchersListButtonOnClick = function () {
	window.window_location_assign("ListOfDispatchers.aspx");
};
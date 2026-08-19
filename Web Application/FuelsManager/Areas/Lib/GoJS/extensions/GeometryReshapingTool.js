"use strict";

/*  EDITED BY VAREC */


/*
*  Copyright (C) 1998-2017 by Northwoods Software Corporation. All Rights Reserved.
*/

/**
* @constructor
* @extends Tool
* @class
* This GeometryReshapingTool class allows for a Shape's Geometry to be modified by the user
* via the dragging of tool handles. 
* This does not handle Links, whose routes should be reshaped by the LinkReshapingTool.
* The {@link #reshapeObjectName} needs to identify the named {@link Shape} within the
* selected {@link Part}.
* If the shape cannot be found or if its {@link Shape#geometry} is not of type {@link Geometry#Path},
* this will not show any GeometryReshaping {@link Adornment}.
* At the current time this tool does not support adding or removing {@link PathSegment}s to the Geometry.
*/
function GeometryReshapingTool() {
	go.Tool.call(this);
	this.name = "GeometryReshaping";

	var h = new go.Shape();
	h.figure = "Diamond";
	h.desiredSize = new go.Size(7, 7);
	h.fill = "lightblue";
	h.stroke = "dodgerblue";
	h.cursor = "move";
	/** @type {GraphObject} */
	this._handleArchetype = h;

	/** @type {string} */
	this._reshapeObjectName = 'SHAPE';  //??? can't add Part.reshapeObjectName property
	// there's no Part.reshapeAdornmentTemplate either

	// internal state
	/** @type {GraphObject} */
	this._handle = null;
	/** @type {Shape} */
	this._adornedShape = null;
	/** @type {Geometry} */
	this._originalGeometry = null;  // in case the tool is cancelled and the UndoManager is not enabled
	this._documentPoint = {}; // the data for a new mouse position
}
go.Diagram.inherit(GeometryReshapingTool, go.Tool);


/*
* A small GraphObject used as a reshape handle for each segment.
* The default GraphObject is a small blue diamond.
* @name GeometryReshapingTool#handleArchetype 
* @function.
* @return {GraphObject}
*/
Object.defineProperty(GeometryReshapingTool.prototype, "handleArchetype", {
	get: function () { return this._handleArchetype; },
	set: function (val) { this._handleArchetype = value; }
});

/*
* The name of the GraphObject to be reshaped.
* @name GeometryReshapingTool#reshapeObjectName
* @function.
* @return {string}
*/
Object.defineProperty(GeometryReshapingTool.prototype, "reshapeObjectName", {
	get: function () { return this._reshapeObjectName; },
	set: function (val) { this._reshapeObjectName = value; }
});

/*
* This read-only property returns the {@link GraphObject} that is the tool handle being dragged by the user.
* This will be contained by an {@link Adornment} whose category is "GeometryReshaping".
* Its {@link Adornment#adornedObject} is the same as the {@link #adornedShape}.
* @name GeometryReshapingTool#handle
* @function.
* @return {GraphObject}
*/
Object.defineProperty(GeometryReshapingTool.prototype, "handle", {
	get: function () { return this._handle; }
});

/*
* Gets the {@link Shape} that is being reshaped.
* This must be contained within the selected Part.
* @name GeometryReshapingTool#adornedShape
* @function.
* @return {Shape}
*/
Object.defineProperty(GeometryReshapingTool.prototype, "adornedShape", {
	get: function () { return this._adornedShape; }
});

/*
* This read-only property remembers the original value for {@link Shape#geometry},
* so that it can be restored if this tool is cancelled.
* @name GeometryReshapingTool#originalGeometry
* @function.
* @return {Geometry}
*/
Object.defineProperty(GeometryReshapingTool.prototype, "originalGeometry", {
	get: function () { return this._originalGeometry; }
});


/**
* Show an {@link Adornment} with a reshape handle at each point of the geometry.
* Don't show anything if {@link #reshapeObjectName} doesn't identify a {@link Shape}
* that has a {@link Shape#geometry} of type {@link Geometry#Path}.
* @this {GeometryReshapingTool}
* @param {Part} part the part.
*/
GeometryReshapingTool.prototype.updateAdornments = function (part) {
	if (part === null || part instanceof go.Link) return;  // this tool never applies to Links
	if (part.isSelected && !this.diagram.isReadOnly) {
		var selelt = part.findObject(this.reshapeObjectName);
		if (selelt instanceof go.Shape && selelt.actualBounds.isReal() && selelt.isVisibleObject() &&
			 part.canReshape() && part.actualBounds.isReal() && part.isVisible() &&
			 selelt.geometry.type === go.Geometry.Path) {
			var adornment = part.findAdornment(this.name);
			if (adornment === null) {
				adornment = this.makeAdornment(selelt);
			}
			if (adornment !== null) {
				// update the position/alignment of each handle
				var geo = selelt.geometry;
				var b = geo.bounds;
				// update the size of the adornment
				adornment.findObject("BODY").desiredSize = b.size;
				adornment.elements.each(function (h) {
					if (h._typ === undefined) return;
					var fig = geo.figures.elt(h._fig);
					var seg = fig.segments.elt(h._seg);
					var x = 0;
					var y = 0;
					switch (h._typ) {
						case 0: x = fig.startX; y = fig.startY; break;
						case 1: x = seg.endX; y = seg.endY; break;
						case 2: x = seg.point1X; y = seg.point1Y; break;
						case 3: x = seg.point2X; y = seg.point2Y; break;
					}
					var bw = b.width;
					if (bw === 0) bw = 0.001;
					var bh = b.height;
					if (bh === 0) bh = 0.001;
					h.alignment = new go.Spot(Math.max(0, Math.min((x - b.x) / bw, 1)),
													  Math.max(0, Math.min((y - b.y) / bh, 1)));
				});

				part.addAdornment(this.name, adornment);
				adornment.location = selelt.getDocumentPoint(go.Spot.TopLeft);
				if (geo && geo.figures && geo.figures.first().segments.count === 1) {
					adornment.angle = part.angle;
				}
				else {
					adornment.angle = selelt.getDocumentAngle();
				}
				return;
			}
		}
	}
	part.removeAdornment(this.name);
};

/*
* @this {GeometryReshapingTool}
*/
GeometryReshapingTool.prototype.makeAdornment = function (selelt) {
	var adornment = new go.Adornment();
	adornment.type = go.Panel.Spot;
	adornment.locationObjectName = "BODY";
	adornment.locationSpot = new go.Spot(0, 0, -selelt.strokeWidth / 2, -selelt.strokeWidth / 2);
	var h = new go.Shape();
	h.name = "BODY";
	h.fill = null;
	h.stroke = null;
	h.strokeWidth = 0;
	adornment.add(h);

	var geo = selelt.geometry;
	// requires Path Geometry, checked above in updateAdornments
	for (var f = 0; f < geo.figures.count; f++) {
		var fig = geo.figures.elt(f);
		for (var g = 0; g < fig.segments.count; g++) {
			var seg = fig.segments.elt(g);
			var h;
			if (g === 0) {
				h = this.makeHandle(selelt, fig, seg);
				if (h !== null) {
					h._typ = 0;
					h._fig = f;
					h._seg = g;
					adornment.add(h);
				}
			}
			h = this.makeHandle(selelt, fig, seg);
			if (h !== null) {
				h._typ = 1;
				h._fig = f;
				h._seg = g;
				adornment.add(h);
			}
			if (seg.type === go.PathSegment.QuadraticBezier || seg.type === go.PathSegment.Bezier) {
				h = this.makeHandle(selelt, fig, seg);
				if (h !== null) {
					h._typ = 2;
					h._fig = f;
					h._seg = g;
					adornment.add(h);
				}
				if (seg.type === go.PathSegment.Bezier) {
					h = this.makeHandle(selelt, fig, seg);
					if (h !== null) {
						h._typ = 3;
						h._fig = f;
						h._seg = g;
						adornment.add(h);
					}
				}
			}
		}
	}
	adornment.category = this.name;
	adornment.adornedObject = selelt;
	return adornment;
};

/*
* @this {GeometryReshapingTool}
*/
GeometryReshapingTool.prototype.makeHandle = function (selelt, fig, seg) {
	var h = this.handleArchetype;
	if (h === null) return null;
	return h.copy();
};


/*
* @this {GeometryReshapingTool}
*/
GeometryReshapingTool.prototype.canStart = function () {
	if (!this.isEnabled) return false;

	var diagram = this.diagram;
	if (diagram === null || diagram.isReadOnly) return false;
	if (!diagram.allowReshape) return false;
	if (!diagram.lastInput.left) return false;
	var h = this.findToolHandleAt(diagram.firstInput.documentPoint, this.name);
	return (h !== null);
};

GeometryReshapingTool.prototype.conditionAngle = function (angle) {
	//reverse the sign
	var ret = 0 - angle;
	//var ret = angle;
	while (ret <= -360) {
		ret += 360;
	}
	while (ret >= 360) {
		ret -= 360;
	}
	//only deal with positive angles
	if (ret < 0) {
		ret = 360 + ret;
	}
	//put in first and fourth quadrant
	if (ret > 90 && ret <= 180) {
		ret = 180 - ret;
		ret = 0 - ret;
	}
	if (ret > 180 && ret <= 270) {
		ret = ret - 180;
	}
	if (ret > 270 && ret <= 360) {
		ret = 360 - ret;
		ret = 0 - ret;
	}
	return ret;
}

GeometryReshapingTool.prototype.getGeomertyString = function (coords) {
	var ret = "F M" + Math.round(coords.startX) + " " + Math.round(coords.startY) + " L" + Math.round(coords.endX) + " " + Math.round(coords.endY);
	return ret;
}

GeometryReshapingTool.prototype.getGeomertyCoords = function (geoString) {
	var coords = {
		startX: 0,
		startY: 0,
		endX: 0,
		endY: 0
	};
	var midx = geoString.indexOf('M');
	var lidx = geoString.indexOf('L');
	var strCoords = geoString.substring(midx + 1, lidx).trim();
	var coordArray = strCoords.split(' ');
	coords.startX = Number(coordArray[0]);
	coords.startY = Number(coordArray[1]);
	strCoords = geoString.substring(lidx + 1).trim();
	coordArray = strCoords.split(' ');
	coords.endX = Number(coordArray[0]);
	coords.endY = Number(coordArray[1]);
	return coords;
}

GeometryReshapingTool.prototype.ResetPartAngle = function () {
	var shape = this._handle.part.adornedObject;
	var geo = shape.geometry;
	if ( geo && geo.figures && geo.figures.first().segments.count === 1 )
	{
		var part = this._adornedShape.part;
		if ( part.angle !== 0 )
		{
			var angle = part.data.angle + part.angle;
			var coords = this.getGeomertyCoords( part.data.geo );
			var xLength = coords.endX - coords.startX;
			var yLength = coords.endY - coords.startY;
			var length = Math.sqrt( xLength * xLength + yLength * yLength );
			angle = this.conditionAngle( angle );
			var absAngle = Math.abs( angle );
			var rad = Math.PI * absAngle / 180;
			var endY = Math.round( length * Math.sin( rad ) );
			var endX = Math.round( length * Math.cos( rad ) );
			var newCoords = {
				startX: 0,
				startY: 0,
				endX: 0,
				endY: 0
			};
			if ( angle < 0 )
			{
				newCoords.endY = endY;
			}
			else
			{
				newCoords.startY = endY;
			}
			newCoords.endX = endX;
			var geoStr = this.getGeomertyString( newCoords );
			var shape = part.findObject( 'SHAPE' );
			shape.geometryString = geoStr;
			var b = shape.geometry.bounds;
			shape.desiredSize = b.size;
			part.angle = 0;
			this.updateAdornments( part );
			part.diagram.maybeUpdate(); // force more frequent drawing for smoother looking behavior
			this.oldDoActivate();

		}
	}
}

/**
* @this {GeometryReshapingTool}
*/
GeometryReshapingTool.prototype.oldDoActivate = function () {
	var diagram = this.diagram;
	if (diagram === null) return;
	this._handle = this.findToolHandleAt(diagram.firstInput.documentPoint, this.name);
	if (this._handle === null) return;
	var shape = this._handle.part.adornedObject;
	if (!shape) return;
	this._adornedShape = shape;
	diagram.isMouseCaptured = true;
	this.startTransaction(this.name);
	this._originalGeometry = shape.geometry;
	this.isActive = true;
};


/**
* @this {GeometryReshapingTool}
*/
GeometryReshapingTool.prototype.doActivate = function ()
{
	this.oldDoActivate();

	this.ResetPartAngle(this._adornedShape.part);
};

/**
* @this {GeometryReshapingTool}
*/
GeometryReshapingTool.prototype.doDeactivate = function () {
	this.stopTransaction();

	this._handle = null;
	this._adornedShape = null;
	var diagram = this.diagram;
	if (diagram !== null) diagram.isMouseCaptured = false;
	this.isActive = false;
};

/**
* @this {GeometryReshapingTool}
*/
GeometryReshapingTool.prototype.doCancel = function () {
	var shape = this._adornedShape;
	if (shape !== null) {
		// explicitly restore the original route, in case !UndoManager.isEnabled
		shape.geometry = this._originalGeometry;
	}
	this.stopTool();
};



/**
calculate the nearest point when snap to grid is onn
*/
GeometryReshapingTool.prototype.setLastinputonGridPoint = function () {
    //alert(this.diagram.lastInput.documentPoint);
    // the grid always starts at 0,0 so move to the left and down until we hit the mouse location
    var lastPosition = this.diagram.lastInput.documentPoint.copy();
    var diagram = this.diagram;

    // use the grid snap to settings
    var xdistance = diagram.toolManager.draggingTool.gridSnapCellSize.width;
    var ydistance = diagram.toolManager.draggingTool.gridSnapCellSize.height;

    // the grid is always in integer spacing so round the current mouse position
    // use modulus to determine where we are
    lastPosition.x = Math.round(lastPosition.x);
    lastPosition.y = Math.round(lastPosition.y);
    var xExtra = lastPosition.x % xdistance;
    var yExtra = lastPosition.y % ydistance;

    // reset the values
    if (xExtra <= (xdistance / 2)) {
        lastPosition.x -= xExtra;
    }
    else {
        lastPosition.x += (xdistance - xExtra);
    }
    if (yExtra <= (ydistance / 2)) {
        lastPosition.y -= yExtra;
    }
    else {
        lastPosition.y += (ydistance - yExtra);
    }

    //console.log("Geometry setlastposition x = " + lastPosition.x + " y = " + lastPosition.y);

    this.diagram.lastInput._documentPoint = lastPosition;

	var newpt = this.computeReshape(diagram.lastInput._documentPoint);
	this.reshape(newpt);
	this.SetLineStartandEndValues(diagram.lastInput._documentPoint);
}


/**
* @this {GeometryReshapingTool}
*/
GeometryReshapingTool.prototype.doMouseMove = function () {
	var diagram = this.diagram;
	if (this.isActive && diagram !== null) {
	    // if snap to grid is on make sure we are on a snap point
	    var diagram = this.diagram;
	    if (diagram.toolManager.draggingTool.isGridSnapEnabled === true) {
	        this.setLastinputonGridPoint();
	    }
	    else {
	        var newpt = this.computeReshape(diagram.lastInput.documentPoint);
	        this.reshape(newpt);
	        this.SetLineStartandEndValues(diagram.lastInput.documentPoint);
	    }
	}
};

/**
* @this {GeometryReshapingTool}
*/
GeometryReshapingTool.prototype.doMouseUp = function () {
	var diagram = this.diagram;
	if (this.isActive && diagram !== null) {
	    if (diagram.toolManager.draggingTool.isGridSnapEnabled === true) {
	        this.setLastinputonGridPoint();
	    }
	    else {
	        var newpt = this.computeReshape(diagram.lastInput.documentPoint);
	        this.reshape(newpt);
	        this.SetLineStartandEndValues(diagram.lastInput.documentPoint);
        }
		this.transactionResult = this.name;  // success
	}
	this.stopTool();
};


GeometryReshapingTool.prototype.reshape = function (newPoint) {
	var shape = this.adornedShape;

	//shape.part.data.loc = go.Point.stringify(shape.part.location);
	//shape.part.data.pos = go.Point.stringify(shape.part.position);

	var locpt = shape.getLocalPoint(newPoint);
	var geo = shape.geometry.copy();
	if (this.handle == null)
	    return;
	var type = this.handle._typ;
	if (type === undefined) return;
	var fig = geo.figures.elt(this.handle._fig);
	var seg = fig.segments.elt(this.handle._seg);

	var part;

	if (geo && geo.figures && geo.figures.first().segments.count === 1) {
		var offsetForLine, offsetX = 0, offsetY = 0;
		var geoStringOrig = shape.geometryString;
		var rect = this.getGeomertyCoords(geoStringOrig);
		var startX = rect.startX,
			 startY = rect.startY,
			 endX = rect.endX,
			 endY = rect.endY;
	    //Calculate transformation coordinates of line geometry inside go.Shape object
		switch (type) {

			case 0:
				if (locpt.x < 0) {
					offsetX = -locpt.x;
					endX -= locpt.x;
				}
				else {
					startX = locpt.x;
				}

				if (locpt.y < 0) {
					offsetY = -locpt.y;
					endY -= locpt.y;
				}
				else {
					startY = locpt.y;
				}

				break;

			case 1:
				if (locpt.x < 0) {
					offsetX = -locpt.x;
					startX -= locpt.x;
				}
				else {
					endX = locpt.x;
				}

				if (locpt.y < 0) {
					offsetY = -locpt.y;
					startY -= locpt.y;
				}
				else {
					endY = locpt.y;
				}

				break;

		}

		var lesserX = 0;
		var lesserY = 0;
		var geoString = '';

		//Adjust coordinate calculations to deal with quadrant shifting
		lesserX = (startX < endX) ? startX : endX;
		lesserY = (startY < endY) ? startY : endY;
		startX -= lesserX;
		endX -= lesserX;
		startY -= lesserY;
		endY -= lesserY;
		offsetX -= lesserX;
		offsetY -= lesserY;

	    //make sure start and end are on a snap if enabled
		//console.log("coords " + startX + " " + startY + " " + endX + " " + endY);
		//console.log(shape.part.position);
	    //console.log(PartYoffset + " x " + PartXoffset);
		//console.log("coords " + startX + " " + startY + " " + endX + " " + endY);

		//Regenerate new SVG geometry string
		geoString = 'F M' + startX + ' ' + startY + ' L' + endX + ' ' + endY;
		//console.log('GeoOrig: ' + geoStringOrig + ' [locpt.x = ' + locpt.x + ' startX: ' + startX + '] [locpt.y = ' + locpt.y + ' startY: ' + startY + '] Geo: ' + geoString);

	    //Construct a new offset point to counteract move of part base on change of geometry
		offsetForLine = new go.Point(offsetX, offsetY);
        //console.log("offset " + offsetForLine.x + " y " + offsetForLine.y)

		shape.geometryString = geoString; // modify the Shape
		part = shape.part; // move the Part holding the Shape
		//this.diagram.skipsUndoManager = true;
		//console.log("GeoString " + part.geometryString);
		//console.log( "offsetX " + offsetX + " offsetY " + offsetY + " part.angle " + this._handle.part.angle );
		//console.log( "Before Part Position " + part.position + " offsetForLine " + offsetForLine );

		//Something is not happening properly with the part.move when an angle is present on the this._handle.part
		part.move(part.position.copy().subtract(offsetForLine)); //Shift part back to position so that segment handle not being moved is put back into original position
		//part.move(part.position.copy()); //Shift part back to position so that segment handle not being moved is put back into original position
	    //console.log("After Part Position " + part.position);
		//this.diagram.skipsUndoManager = false;
	}
	else {
		switch (type) {
			case 0: fig.startX = locpt.x; fig.startY = locpt.y; break;
			case 1: seg.endX = locpt.x; seg.endY = locpt.y; break;
			case 2: seg.point1X = locpt.x; seg.point1Y = locpt.y; break;
			case 3: seg.point2X = locpt.x; seg.point2Y = locpt.y; break;
		}

		var offset = geo.normalize();  // avoid any negative coordinates in the geometry
		shape.geometry = geo;  // modify the Shape
		part = shape.part;  // move the Part holding the Shape
		if (!part.locationSpot.equals(go.Spot.Center)) {  // but only if the locationSpot isn't Center
			part.move(part.position.copy().subtract(offset));
		}
	}
	var b = shape.geometry.bounds;
	shape.desiredSize = b.size;
	this.updateAdornments(part);  // update any Adornments of the Part
	this.diagram.maybeUpdate();  // force more frequent drawing for smoother looking behavior
};


/**
* @expose
* @this {GeometryReshapingTool}
* @param {Point} p the point where the handle is being dragged.
* @return {Point}
*/
GeometryReshapingTool.prototype.computeReshape = function (p) {
	return p;  // no constraints on the points
};

GeometryReshapingTool.prototype.SetLineStartandEndValues = function (p) {
    var part = this._adornedShape.part;

    if (part === null)
        return;
    /*
    console.log("goint Start X= " + part.data.LineStartPositionX);
    console.log("goint Start Y= " + part.data.LineStartPositionY);
    console.log("goint End X= " + part.data.LineEndPositionX);
    console.log("goint End Y= " + part.data.LineEndPositionY);
    console.log("px= " + p.x + " py= " + p.y);
    */
    // since we are dragging an end point we need to find out which end we are the closest to
    var startX = Math.abs(part.data.LineStartPositionX - p.x);
    var startY = Math.abs(part.data.LineStartPositionY - p.y);
    var endX = Math.abs(part.data.LineEndPositionX - p.x);
    var endY = Math.abs(part.data.LineEndPositionY - p.y);

    //console.log("startx = " + startX + " starty = " + startY + " endx = " + endX + " endy = " + endY);
    //console.log("angle = " + part.data.angle);

    if((startX + startY) < (endX + endY))
    {
        //console.log("Start");
        part.data.LineStartPositionX = p.x;
        part.data.LineStartPositionY = p.y;
    }
    else
    {
        //console.log("End");
        part.data.LineEndPositionX = p.x;
        part.data.LineEndPositionY = p.y;
    }
}



Object.defineProperty(GeometryReshapingTool.prototype, "documentPoint", {
    get: function () { return this._documentPoint; },
    set: function (val) { this._documentPoint = val; }
});

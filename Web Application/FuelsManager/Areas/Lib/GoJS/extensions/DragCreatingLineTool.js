"use strict";
/*
*  Copyright (C) 1998-2016 by Northwoods Software Corporation. All Rights Reserved.
*/

// A custom Tool for drawing lines

/**
* @constructor
* @extends Tool
* @class
* This tool allows the user to draw a new line shape by clicking where the corners should go.
* Right click or type ENTER to finish the operation.
* <p/>
* Set {@link #archetypePartData} to customize the node data object that is added to the model.
* Data-bind to those properties in your node template to customize the appearance and behavior of the part.
* <p/>
* This tool uses a temporary {@link Shape}, {@link #temporaryShape}, held by a {@link Part} in the "Tool" layer,
* to show interactively what the user is drawing.
*/
function DragCreatingLineTool() {
	go.Tool.call(this);
	this.name = "DragCreatingLine";
	this._hasArcs = false;
	this._archetypePartData = {}; // the data to copy for a new line Part
	this._documentPoint = {}; // the data for a new mouse position
	this._lineStartPosition = {};   // start coordinates for the line used for the from arrow positioning
	this._lineEndPosition = {};   // end coordinates for the line used for the to arrow positioning

	// this is the Shape that is shown during a drawing operation
	this._temporaryShape = go.GraphObject.make(go.Shape, { name: "SHAPE", fill: "lightgray", strokeWidth: 2.0 });
	// the Shape has to be inside a temporary Part that is used during the drawing operation
	go.GraphObject.make(go.Part, { layerName: "Tool" }, this._temporaryShape);
}
go.Diagram.inherit(DragCreatingLineTool, go.Tool);

/**
* Don't start this tool in a mode-less fashion when the user's mouse-down is on an existing Part.
* When this tool is a mouse-down tool, it requires using the left mouse button in the background of a modifiable Diagram.
* Modal uses of this tool will not call this canStart predicate.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.canStart = function () {
	if (!this.isEnabled) return false;
	var diagram = this.diagram;
	if (diagram === null || diagram.isReadOnly || diagram.isModelReadOnly) return false;
	var model = diagram.model;
	if (model === null) return false;
	// require left button
	if (!diagram.firstInput.left) return false;
	// can't start when mouse-down on an existing Part
	var obj = diagram.findObjectAt(diagram.firstInput.documentPoint, null, null);
	return (obj === null);
};

/**
* Start a transaction, capture the mouse, use a "crosshair" cursor,
* and start accumulating points in the geometry of the {@link #temporaryShape}.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.doActivate = function () {
	go.Tool.prototype.doActivate.call(this);
	var diagram = this.diagram;
	this.startTransaction(this.name);
	if (!diagram.lastInput.isTouchEvent){
	    diagram.isMouseCaptured = true;
            }
	diagram.currentCursor = "crosshair";
    // the first point
	if (!diagram.lastInput.isTouchEvent) {
	    if (diagram.toolManager.draggingTool.isGridSnapEnabled === true) {
	        this.setLastinputonGridPoint();
	        this._lineStartPosition = diagram.lastInput._documentPoint.copy();
	        this._lineStartPosition.x = this._lineStartPosition.x - FMDrawIndex.defaultArchetype.strokeWidth / 2;
	        this._lineStartPosition.y = this._lineStartPosition.y - FMDrawIndex.defaultArchetype.strokeWidth / 2;
        }
	    else {
	        // a new temporary end point, the previous one is now "accepted"
	        this.addPoint(diagram.lastInput.documentPoint);
	        this._lineStartPosition = diagram.lastInput.documentPoint.copy();
	        this._lineStartPosition.x = this._lineStartPosition.x - FMDrawIndex.defaultArchetype.strokeWidth / 2;
	        this._lineStartPosition.y = this._lineStartPosition.y - FMDrawIndex.defaultArchetype.strokeWidth / 2;
        }
	}
};

/**
* Stop the transaction and clean up.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.doDeactivate = function () {
	go.Tool.prototype.doDeactivate.call(this);
	var diagram = this.diagram;
	if (this.temporaryShape !== null) {
		diagram.remove(this.temporaryShape.part);
	}
	diagram.currentCursor = "";
	if (diagram.isMouseCaptured) diagram.isMouseCaptured = false;
	this.stopTransaction();
};

/**
* This internal method adds a segment to the geometry of the {@link #temporaryShape}.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.addPoint = function (p) {
	var shape = this.temporaryShape;
	if (shape === null) return;

    // for the temporary Shape, normalize the geometry to be in the viewport
	var viewpt = this.diagram.viewportBounds.position;
	//var q = new go.Point(Math.round(p.x) - Math.round(viewpt.x), Math.round(p.y) - Math.round(viewpt.y));
	//var q = new go.Point(p.x - viewpt.x, p.y - viewpt.y);

	//console.log("viewpt = " + viewpt);

	var penoffset = FMDrawIndex.defaultArchetype.strokeWidth / 2;// * shape.strokeWidth);

	//viewpt.x -= penoffset;
	//viewpt.y -= penoffset;

	var xinset = viewpt.x;
	var yinset = viewpt.y;

	var vgx = (p.x - xinset - (penoffset));// * 2));
	var vgy = (p.y - yinset - (penoffset));// * 2));

	//console.log("point = " + vgx + " y= " + vgy);
	//var q = new go.Point(Math.round(p.x) - Math.round(viewpt.x), Math.round(p.y) - Math.round(viewpt.y));
	var q = new go.Point(vgx, vgy);
	//var q = new go.Point(p.x -viewpt.x, p.y -viewpt.y);

	var part = shape.part;
	// if it's not in the Diagram, re-initialize the Shape's geometry and add the Part to the Diagram
	if (part.diagram === null) {
	    var fig = new go.PathFigure(q.x, q.y, true);  // possibly filled, depending on Shape.fill
		var geo = new go.Geometry().add(fig);  // the Shape.geometry consists of a single PathFigure
		this.temporaryShape.geometry = geo;
	    // position the Shape's Part, accounting for the stroke width
        	    
		var offset = 0.0;//((shape.strokeWidth / 2));
		//part.position = viewpt.copy().offset(-offset, -offset);
		part.position = viewpt.copy().offset(-penoffset, -penoffset);
		this.diagram.startX = (p.x);
		//this.diagram.startX -= penoffset;
		this.diagram.startY = (p.y);
		//this.diagram.startY -= penoffset;
		this.diagram.add(part);
		//console.log("part add " + this.diagram.startX + " " +this.diagram.startY);

        /*
		var offset = Math.round((shape.strokeWidth / 2));
		part.position = viewpt.copy().offset(-offset, -offset);
		this.diagram.startX = Math.round(p.x);// - .5);
		this.diagram.startX -= offset;
		this.diagram.startY = Math.round(p.y);// - .5);
		this.diagram.startY -= offset;
		this.diagram.add(part);
        */
	    }
	else {
	    // must copy whole Geometry in order to add a PathSegment
		var geo = shape.geometry.copy();
		var fig = geo.figures.first();
		if (this.hasArcs) {
			var lastseg = fig.segments.last();
			if (lastseg === null) {
			    fig.add(new go.PathSegment(go.PathSegment.QuadraticBezier, Math.round(q.x), Math.round(q.y), Math.round((fig.startX + q.x) / 2), Math.round((fig.startY + q.y) / 2)));
			} else {
			    fig.add(new go.PathSegment(go.PathSegment.QuadraticBezier, Math.round(q.x), Math.round(q.y), Math.round((lastseg.endX + q.x) / 2), Math.round((lastseg.endY + q.y) / 2)));
		    }
		} else {
		    //fig.add(new go.PathSegment(go.PathSegment.Line, Math.round((Math.round(q.x) - (shape.strokeWidth / 2)) - .5), Math.round((Math.round(q.y) - (shape.strokeWidth / 2)) - .5)));
		    //fig.add(new go.PathSegment(go.PathSegment.Line, (q.x) - (shape.strokeWidth / 2), (q.y) - (shape.strokeWidth / 2)));
		    //fig.add(new go.PathSegment(go.PathSegment.Line, Math.round(q.x), Math.round(q.y)));
		    fig.add(new go.PathSegment(go.PathSegment.Line, q.x, q.y));
		    //console.log("fig.add " + q.x + " " + q.y);
		}
	}
	shape.geometry = geo;
};

/**
* This internal method changes the last segment of the geometry of the {@link #temporaryShape} to end at the given point.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.moveLastPoint = function (p) {
	// must copy whole Geometry in order to change a PathSegment
	var shape = this.temporaryShape;
	var geo = shape.geometry.copy();
	var fig = geo.figures.first();
	var segs = fig.segments;
	if (segs.count > 0) {
		// for the temporary Shape, normalize the geometry to be in the viewport
		var viewpt = this.diagram.viewportBounds.position;
		var seg = segs.elt(segs.count - 1);
	    // modify the last PathSegment to be the given Point p

		var penoffset = FMDrawIndex.defaultArchetype.strokeWidth/2;

       	seg.endX = (p.x - viewpt.x +penoffset);
       	seg.endY = (p.y - viewpt.y +penoffset);
		if (seg.type === go.PathSegment.QuadraticBezier) {
		    var prevx = 0.0;
		    var prevy = 0.0;
		    if (segs.count > 1) {
		        var prevseg = segs.elt(segs.count - 2);
		        prevx = (prevseg.endX);
		        prevy = (prevseg.endY);
		    } else {
		        prevx = (fig.startX);
		        prevy = (fig.startY);
		    }
		    seg.point1X = ((seg.endX + prevx) / 2);
		    seg.point1Y = ((seg.endY + prevy) / 2);
		}
        
        /*
		seg.endX = Math.round(p.x - viewpt.x);
		seg.endY = Math.round(p.y - viewpt.y);
		if (seg.type === go.PathSegment.QuadraticBezier) {
			var prevx = 0.0;
			var prevy = 0.0;
			if (segs.count > 1) {
				var prevseg = segs.elt(segs.count - 2);
				prevx = Math.round(prevseg.endX);
				prevy = Math.round(prevseg.endY);
			} else {
			    prevx = Math.round(fig.startX);
			    prevy = Math.round(fig.startY);
			}
			seg.point1X = Math.round((seg.endX + prevx) / 2);
			seg.point1Y = Math.round((seg.endY + prevy) / 2);
		}
        */
		shape.geometry = geo;
	}
};

/**
* This internal method removes the last segment of the geometry of the {@link #temporaryShape}.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.removeLastPoint = function () {
	// must copy whole Geometry in order to remove a PathSegment
	var shape = this.temporaryShape;
	var geo = shape.geometry.copy();
	var segs = geo.figures.first().segments;
	if (segs.count > 0) {
		segs.removeAt(segs.count - 1);
		shape.geometry = geo;
	}
};

/**
* Add a new node data JavaScript object to the model and initialize the Part's
* position and its Shape's geometry by copying the {@link #temporaryShape}'s {@link Shape#geometry}.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.finishShape = function () {
	var diagram = this.diagram;
	var shape = this.temporaryShape;
	var part = null;
	if (shape !== null && this.archetypePartData !== null) {
	    // remove the temporary point, which is last, except on touch devices

        // need to check and add
	    if (diagram.toolManager.draggingTool.isGridSnapEnabled === false) {
	        if (!diagram.lastInput.isTouchEvent) this.removeLastPoint();
	    }

		var tempgeo = shape.geometry;
		if (tempgeo.figures.first().segments.count >= 1) {
			// normalize geometry and node position
			var viewpt = diagram.viewportBounds.position;
			var geo = tempgeo.copy();

			// create the node data for the model
			var d = diagram.model.copyNodeData(this.archetypePartData);
		    // adding data to model creates the actual Part
			if (d.lineStyleTransparency !== 0) {
			    d.lineStyleTransparency = 0;
			    if (d.lineStroke !== null && typeof (d.lineStroke) === "string") {
			        if (d.lineStroke.search("rgb") >= 0) {
			            var c = d.lineStroke;
			            var rgb = c.replace(/^(rgb|rgba)\(/, '').replace(/\)$/, '').replace(/\s/g, '').split(',');
			            var temp = "#" + ((1 << 24) + (parseInt(rgb[0]) << 16) + (parseInt(rgb[1]) << 8) + parseInt(rgb[2])).toString(16).slice(1);
			            d.lineStroke = temp;
			        }
			    }
			}
			diagram.model.addNodeData(d);
			            
			diagram.model.setDataProperty(d, "category", "line");

			diagram.model.setDataProperty(d, 'LineStartPositionX', this._lineStartPosition.x - FMDrawIndex.defaultArchetype.strokeWidth / 2);
			diagram.model.setDataProperty(d, 'LineStartPositionY', this._lineStartPosition.y - FMDrawIndex.defaultArchetype.strokeWidth / 2);
			diagram.model.setDataProperty(d, 'LineEndPositionX', this._lineEndPosition.x -FMDrawIndex.defaultArchetype.strokeWidth / 2);
			diagram.model.setDataProperty(d, 'LineEndPositionY', this._lineEndPosition.y - FMDrawIndex.defaultArchetype.strokeWidth / 2);

			diagram.model.setDataProperty(d, 'arrowLineOffset', new go.Point(0, 0));
			diagram.model.setDataProperty(d, 'forceGeoEndPositionBindings', false);
			diagram.model.setDataProperty(d, 'forceGeoStartPositionBindings', false);

			var part = diagram.findPartForData(d);
            
			// assign the position for the whole Part
			var pos = geo.normalize();

			var penoffset = FMDrawIndex.defaultArchetype.strokeWidth;

		    //pos.x = viewpt.x - pos.x + (shape.strokeWidth / 2);
			//pos.y = viewpt.y - pos.y + (shape.strokeWidth / 2);
			//pos.x = Math.round((Math.round(viewpt.x) - Math.round(pos.x) - (shape.strokeWidth / 2)) - .5);
	        //pos.y = Math.round((Math.round(viewpt.y) - Math.round(pos.y) - (shape.strokeWidth / 2)) - .5);
	        //pos.x = Math.round(Math.round(viewpt.x) -Math.round(pos.x));
	        //pos.y = Math.round(Math.round(viewpt.y) -Math.round(pos.y));
           	pos.x = viewpt.x - pos.x - penoffset;
           	pos.y = viewpt.y - pos.y - penoffset;
			part.position = pos;
			var shape = part.findObject("SHAPE");
			if (shape !== null) shape.geometry = geo;
			this.transactionResult = this.name;

			diagram.select(part);
		}
	}
	this.stopTool();
	return part;
};


/**
calculate the nearest point when snap to grid is onn
*/
DragCreatingLineTool.prototype.setLastinputonGridPoint = function () {
	var shape = this.temporaryShape;
	if(shape === null) return;
    //alert(this.diagram.lastInput.documentPoint);
    // the grid always starts at 0,0 so move to the left and down until we hit the mouse location
    var lastPosition = this.diagram.lastInput.documentPoint.copy();
    var diagram = this.diagram;

    // use the grid snap to settings
    var xdistance = diagram.toolManager.draggingTool.gridSnapCellSize.width;
    var ydistance = diagram.toolManager.draggingTool.gridSnapCellSize.height;

    // use modulus to determine where we are
    //lastPosition.x = Math.round(lastPosition.x);
    //lastPosition.y = Math.round(lastPosition.y);
    var penoffset = FMDrawIndex.defaultArchetype.strokeWidth;

    lastPosition.x = Math.round(lastPosition.x);
    lastPosition.y = Math.round(lastPosition.y);
    var xExtra = lastPosition.x % xdistance;
    var yExtra = lastPosition.y % ydistance;

    // reset the values
    if (xExtra <= (xdistance / 2))
    {
        lastPosition.x -= xExtra;
    }
    else
    {
        lastPosition.x += (xdistance - xExtra);
    }
    if (yExtra <= (ydistance / 2)) {
        lastPosition.y -= yExtra;
    }
    else {
        lastPosition.y += (ydistance - yExtra);
    }

     lastPosition.y += penoffset;
    lastPosition.x += penoffset;

    //console.log("dragcreating setlastposition x = " + lastPosition.x + " y = " +lastPosition.y);

    this.diagram.lastInput._documentPoint = lastPosition;

    this.addPoint(this.diagram.lastInput._documentPoint);

}


/**
* Add another point to the geometry of the {@link #temporaryShape}.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.doMouseDown = function () {
	if (!this.isActive) {
		this.doActivate();
	}

    // reset the mouse position based on the grid location if snap is on
    
	var diagram = this.diagram;
	if (diagram.toolManager.draggingTool.isGridSnapEnabled === true) {
	    this.setLastinputonGridPoint();
	}
	else {
	    // a new temporary end point, the previous one is now "accepted"
	    this.addPoint(this.diagram.lastInput.documentPoint);
	}
    

	if (!this.diagram.lastInput.left) {  // e.g. right mouse down
		this.finishShape();
	} else if (this.diagram.lastInput.clickCount > 1) {  // e.g. double-click
		this.removeLastPoint();
		this.finishShape();
	}
};

/**
* Move the last point of the {@link #temporaryShape}'s geometry to follow the mouse point.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.doMouseMove = function () {
    if (this.isActive) {
		this.moveLastPoint(this.diagram.lastInput.documentPoint);
	}
};

DragCreatingLineTool.prototype.doMouseUp = function () {
    // reset the mouse position based on the grid location if snap is on
    var xsize = this.diagram.startX - this.diagram.lastInput.documentPoint.x;
    var ysize = this.diagram.startY - this.diagram.lastInput.documentPoint.y;
    var csize = Math.sqrt((xsize * xsize) + (ysize * ysize));
    if (csize < 4) {
        this.undo();
    }
    else {
        var diagram = this.diagram;
        if (diagram.toolManager.draggingTool.isGridSnapEnabled === true) {
            this.removeLastPoint();
            this.setLastinputonGridPoint();
            this._lineEndPosition = diagram.lastInput._documentPoint.copy();
        }
        else {
            this.addPoint(this.diagram.lastInput.documentPoint);
            this._lineEndPosition = diagram.lastInput.documentPoint.copy();
        }
    }
    this.finishShape();
    
};


/**
* Typing the "ENTER" key accepts the current geometry (excluding the current mouse point)
* and creates a new part in the model by calling {@link #finishShape}.
* <p/>
* Typing the "Z" key causes the previous point to be discarded.
* <p/>
* Typing the "ESCAPE" key causes the temporary Shape and its geometry to be discarded and this tool to be stopped.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.doKeyDown = function () {
	if (!this.isActive) return;
	var e = this.diagram.lastInput;
	if (e.key === '\r') {  // accept
		this.finishShape();  // all done!
	} else if (e.key === 'Z') {  // undo
		this.undo();
	} else {
		go.Tool.prototype.doKeyDown.call(this);
	}
};

/**
* Undo: remove the last point and continue the drawing of new points.
* @this {DragCreatingLineTool}
*/
DragCreatingLineTool.prototype.undo = function () {
	// remove a point, and then treat the last one as a temporary one
	this.removeLastPoint();
	var lastInput = this.diagram.lastInput;
	if (lastInput.event instanceof window.MouseEvent) this.moveLastPoint(lastInput.documentPoint);
};


// Public properties

/**
* Gets or sets whether this tools draws shapes with quadratic bezier curves for each segment, or just straight lines.
* The default value is false -- only use straight lines.
* @name DragCreatingLineTool#hasArcs
* @function.
* @return {boolean}
*/
Object.defineProperty(DragCreatingLineTool.prototype, "hasArcs", {
	get: function () { return this._hasArcs; },
	set: function (val) { this._hasArcs = val; }
});

/**
* Gets or sets the Shape that is used to hold the line as it is being drawn.
* The default value is a simple Shape drawing an unfilled open thin black line.
* @name DragCreatingLineTool#temporaryShape
* @function.
* @return {Shape}
*/
Object.defineProperty(DragCreatingLineTool.prototype, "temporaryShape", {
	get: function () { return this._temporaryShape; },
	set: function (val) {
		if (this._temporaryShape !== val && val !== null) {
			val.name = "SHAPE";
			var panel = this._temporaryShape.panel;
			panel.remove(this._temporaryShape);
			this._temporaryShape = val;
			panel.add(this._temporaryShape);
		}
	}
});


/**
* Gets or sets the node data object that is copied and added to the model
* when the drawing operation completes.
* @name DragCreatingLineTool#archetypePartData
* @function.
* @return {Object}
*/
Object.defineProperty(DragCreatingLineTool.prototype, "archetypePartData", {
	get: function () { return this._archetypePartData; },
	set: function (val) { this._archetypePartData = val; }
});

Object.defineProperty(DragCreatingLineTool.prototype, "documentPoint", {
    get: function () { return this._documentPoint; },
    set: function (val) { this._documentPoint = val; }
});

Object.defineProperty(DragCreatingLineTool.prototype, "lineStartPosition", {
    get: function () { return this._lineStartPosition; },
    set: function (val) { this._lineStartPosition = val; }
});

Object.defineProperty(DragCreatingLineTool.prototype, "lineEndPosition", {
    get: function () { return this._lineEndPosition; },
    set: function (val) { this._lineEndPosition = val; }
});

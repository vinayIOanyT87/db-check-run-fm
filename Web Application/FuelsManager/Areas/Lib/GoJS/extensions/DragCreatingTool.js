"use strict";

/*  EDITED BY VAREC */

/*
*  Copyright (C) 1998-2017 by Northwoods Software Corporation. All Rights Reserved.
*/

// A custom Tool for creating a new Node with custom size by dragging its outline in the background.

/**
* @constructor
* @extends Tool
* @class
* The DragCreatingTool lets the user create a new node by dragging in the background
* to indicate its size and position.
* <p/>
* The default drag selection box is a magenta rectangle.
* You can modify the {@link #box} to customize its appearance.
* <p/>
* This tool will not be able to start running unless you have set the
* {@link #archetypeNodeData} property to an object that can be copied and added to the diagram's model.
* <p/>
* You can use this tool in a modal manner by executing:
* <pre><code>
*   diagram.currentTool = new DragCreatingTool();
* </code></pre>
* <p/>
* Use this tool in a mode-less manner by executing:
* <pre><code>
*   myDiagram.toolManager.mouseMoveTools.insertAt(2, new DragCreatingTool());
* </code></pre>
* However when used mode-lessly as a mouse-move tool, in {@link ToolManager#mouseMoveTools},
* this cannot start running unless there has been a motionless delay
* after the mouse-down event of at least {@link #delay} milliseconds.
* <p/>
* This tool does not utilize any {@link Adornment}s or tool handles,
* but it does temporarily add the {@link #box} Part to the diagram.
* This tool does conduct a transaction when inserting the new node.
*/
function DragCreatingTool() {
  go.Tool.call(this);
  this.name = "DragCreating";

  /** @type {Object} */
  this._archetypeNodeData = null;

  var b = new go.Part();
  b.layerName = "Tool";
  b.selectable = false;
  var r = new go.Shape();
  r.name = "SHAPE";
  r.figure = "Rectangle";
  r.fill = null;
  r.stroke = "magenta";
  r.position = new go.Point(0, 0);
  b.add(r);
  /** @type {Part} */
  this._box = b;

  /** @type {number} */
  this._delay = 175;
}

go.Diagram.inherit(DragCreatingTool, go.Tool);

/**
* This tool can run when there has been a mouse-drag, far enough away not to be a click,
* and there has been delay of at least {@link #delay} milliseconds
* after the mouse-down before a mouse-move.
* <p/>
* This method may be overridden.
* @this {DragCreatingTool}
* @return {boolean}
*/
DragCreatingTool.prototype.canStart = function() {
  if (!this.isEnabled) return false;

  // gotta have some node data that can be copied
  if (this.archetypeNodeData === null) return false;
  
  var diagram = this.diagram;
  if (diagram === null) return false;
  // heed IsReadOnly & AllowInsert
  if (diagram.isReadOnly || diagram.isModelReadOnly) return false;
  if (!diagram.allowInsert) return false;

  var e = diagram.lastInput;
  // require left button & that it has moved far enough away from the mouse down point, so it isn't a click
  if (!e.left) return false;
  // don't include the following checks when this tool is running modally
  if (diagram.currentTool !== this) {
    if (!this.isBeyondDragSize()) return false;
    // must wait for "delay" milliseconds before that tool can run
    if (e.timestamp - diagram.firstInput.timestamp < this.delay) return false;
  }
  return true;
};

/**
* Capture the mouse and show the {@link #box}.
* @this {DragCreatingTool}
*/
DragCreatingTool.prototype.doActivate = function() {
  var diagram = this.diagram;
  if (diagram === null) return;
  if (this.activateFunction) {
  	this.activateFunction();
  }
  this.isActive = true;
  diagram.isMouseCaptured = true;
  diagram.add(this.box);
  this.doMouseMove();
};

/**
* Release the mouse and remove any {@link #box}.
* @this {DragCreatingTool}
*/
DragCreatingTool.prototype.doDeactivate = function() {
  var diagram = this.diagram;
  if (diagram === null) return;
  diagram.remove(this.box);
  diagram.isMouseCaptured = false;
  this.isActive = false;
};

/**
* Update the {@link #box}'s position and size according to the value
* of {@link #computeBoxBounds}.
* @this {DragCreatingTool}
*/
DragCreatingTool.prototype.doMouseMove = function() {
  var diagram = this.diagram;
  if (diagram === null) return;
  if (this.isActive && this.box !== null) {
    var r = this.computeBoxBounds();
    var shape = this.box.findObject("SHAPE");
    if (shape === null) shape = this.box.findMainElement();
    shape.desiredSize = r.size;
    this.box.position = r.position;
  }
};

/**
* Call {@link #insertPart} with the value of a call to {@link #computeBoxBounds}.
* @this {DragCreatingTool}
*/
DragCreatingTool.prototype.doMouseUp = function() {
  if (this.isActive) {
    var diagram = this.diagram;
    diagram.remove(this.box);
    try {
      diagram.currentCursor = "wait";
      this.insertPart(this.computeBoxBounds());
    } finally {
      diagram.currentCursor = "";
    }
  }
  this.stopTool();
  if ( this.successCallBackFunction )
  {
  	this.successCallBackFunction();
  }
};

/**
* This just returns a {@link Rect} stretching from the mouse-down point to the current mouse point.
* <p/>
* This method may be overridden.
* @this {DragCreatingTool}
* @return {Rect} a {@link Rect} in document coordinates.
*/
DragCreatingTool.prototype.computeBoxBounds = function() {
    var diagram = this.diagram;
    if (diagram === null) return new go.Rect(0, 0, 0, 0);
    var start = diagram.firstInput.documentPoint;
    var squareBoundingBox = false;
    var latest = diagram.lastInput.documentPoint;

    //Save Latest Point to variable so we don't incur any side effects on the diagram as a result of recomputing
    //for square bounding boxes
    var newPoint = new go.Point(latest.x, latest.y);

    var arch = this.archetypeNodeData;

    // If the type of node being created requires a square bounding box (i.e. a Circle) then the archetypeNodeData should 
    //define the squareBoundingBox property as a boolean and set that property value to 'true'
    if (arch !== null) {
        squareBoundingBox = (typeof arch.squareBoundingBox !== 'boolean') ? false : arch.squareBoundingBox;
    }

    //Logic to transform a rectangle bounding box to a square.  The algorithm determines the minimum between (width and length)
    //And then provides a new stopping point coordinate so that the resulting bounding box is a square.
    if (squareBoundingBox) {
        var length = Math.abs(latest.x - start.x); //X
        var width = Math.abs(latest.y - start.y); //Y
        var x1 = start.x;
        var y1 = start.y;
        var x2 = latest.x;
        var y2 = latest.y;
        var newX2 = 0.0;
        var newY2 = 0.0;
        if (length > width) { //truncate length to match width
            if (x1 > x2) {
                newX2 = x1 - width;
            }
            else {
                newX2 = x1 + width;
            }
            newPoint.x = newX2;
        }
        else if (width > length) { //truncate width to match length

            if (y1 > y2) {
                newY2 = y1 - length;
            }
            else {
                newY2 = y1 + length;
            }
            newPoint.y = newY2;
        }
    }
    return new go.Rect(start, newPoint);
};

/**
* Create a node by adding a copy of the {@link #archetypeNodeData} object
* to the diagram's model, assign its {@link GraphObject#position} and {@link GraphObject#desiredSize}
* according to the given bounds, and select the new part.
* <p>
* The actual part that is added to the diagram may be a {@link Part}, a {@link Node},
* or even a {@link Group}, depending on the properties of the {@link #archetypeNodeData}
* and the type of the template that is copied to create the part.
* @this {DragCreatingTool}
* @param {Rect} bounds a Point in document coordinates.
* @return {Part} the newly created Part, or null if it failed.
*/
DragCreatingTool.prototype.insertPart = function (bounds) {
  var diagram = this.diagram;
  if (diagram === null) return null;
  var arch = this.archetypeNodeData;
  if (arch === null) return null;

  this.startTransaction(this.name);
  var part = null;
  if (arch !== null) {
      var data = diagram.model.copyNodeData(arch);
    if (data) {
        var SetColorObject = false;
        if (data.transparency !== 0) {
            data.transparency = 0;
            if (data.color !== null && typeof (data.color) === "string") {
                if (data.color.search("rgb") >= 0) {
                    var c = data.color;
                    var rgb = c.replace(/^(rgb|rgba)\(/, '').replace(/\)$/, '').replace(/\s/g, '').split(',');
                    var temp = "#" + ((1 << 24) + (parseInt(rgb[0]) << 16) + (parseInt(rgb[1]) << 8) + parseInt(rgb[2])).toString(16).slice(1);
                    data.color = temp;
                    FMDrawIndex.defaultArchetype.color = data.color;
                    FMDrawIndex.RefreshPreview();
                }
            }
            if (data.color !== null && typeof (data.color) === 'object') {
                SetColorObject = true;
            }

        }   // end if
        if (data.lineStyleTransparency !== 0) {
            data.lineStyleTransparency = 0;
            if (data.lineStroke !== null && typeof (data.lineStroke) === "string") {
                if (data.lineStroke.search("rgb") >= 0) {
                    var c = data.lineStroke;
                    var rgb = c.replace(/^(rgb|rgba)\(/, '').replace(/\)$/, '').replace(/\s/g, '').split(',');
                    var temp = "#" + ((1 << 24) + (parseInt(rgb[0]) << 16) + (parseInt(rgb[1]) << 8) + parseInt(rgb[2])).toString(16).slice(1);
                    data.lineStroke = temp;
                }
            }
        }

        diagram.model.addNodeData(data);
        part = diagram.findPartForData(data);

    }
  }
  if (part !== null) {
    part.position = bounds.position;
    part.resizeObject.desiredSize = bounds.size;
    if (diagram.allowSelect) {
        diagram.select(part);  // raises ChangingSelection/Finished

        if (SetColorObject === true && data.color !== null && typeof (data.color) === 'object') {

            var patternNumber = parseInt(data.patternImageName);
            var patternTagId = 'canvasPalatte-propertiesMenu-FILLPATTERNPALETTE-' + data.patternImageName;
            FMDrawPropertyMenu.HandleFillPatternOnClick(patternTagId, patternNumber, false, -99, true);
        }

    }
  }

  // set the TransactionResult before raising event, in case it changes the result or cancels the tool
  this.transactionResult = this.name;
  this.stopTransaction();
  return part;
};


// Public properties

/**
* Gets or sets the {@link Part} used as the "rubber-band box"
* that is stretched to follow the mouse, as feedback for what area will
* be passed to {@link #insertPart} upon a mouse-up.
* <p/>
* Initially this is a {@link Part} containing only a simple magenta rectangular {@link Shape}.
* The object to be resized should be named "SHAPE".
* Setting this property does not raise any events.
* <p/>
* Modifying this property while this tool {@link Tool#isActive} might have no effect.
* @name DragCreatingTool#box
* @function.
* @return {Part}
*/
Object.defineProperty(DragCreatingTool.prototype, "box", {
  get: function() { return this._box; },
  set: function(val) { this._box = val; }
});

/**
* Gets or sets the time in milliseconds for which the mouse must be stationary
* before this tool can be started.
* The default value is 175 milliseconds.
* A value of zero will allow this tool to run without any wait after the mouse down.
* Setting this property does not raise any events.
* @name DragCreatingTool#delay
* @function.
* @return {number}
*/
Object.defineProperty(DragCreatingTool.prototype, "delay", {
  get: function() { return this._delay; },
  set: function(val) { this._delay = val; }
});

/**
* Gets or sets a data object that will be copied and added to the diagram's model each time this tool executes.
* The default value is null.
* The value must be non-null for this tool to be able to run.
* Setting this property does not raise any events.
* @name DragCreatingTool#archetypeNodeData
* @function.
* @return {Object}
*/
Object.defineProperty(DragCreatingTool.prototype, "archetypeNodeData", {
  get: function() { return this._archetypeNodeData; },
  set: function(val) { this._archetypeNodeData = val; }
});

Object.defineProperty(DragCreatingTool.prototype, "successCallBackFunction", {
	get: function () { return this._successCallBackFunction; },
	set: function (val) { this._successCallBackFunction = val; }
});

Object.defineProperty(DragCreatingTool.prototype, "activateFunction", {
	get: function () { return this._activateFunction; },
	set: function (val) { this._activateFunction = val; }
});

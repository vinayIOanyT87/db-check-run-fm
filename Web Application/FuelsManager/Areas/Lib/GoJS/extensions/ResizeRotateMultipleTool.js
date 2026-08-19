"use strict";
/*
*  Copyright (C) 1998-2016 by Northwoods Software Corporation. All Rights Reserved.
*/

/**
* @constructor
* @class
* A custom tool for resizing and rotating multiple objects at once.  When more than one
* part is selected, each non-Link gets a new position and a new desiredSize,
* to maintain the overall arrangement of the Nodes.
* <p>
* It is likely that you will want to set the Part.resizeObjectName to refer to an element
* inside your Part, rather than having this tool set the desiredSize of the whole part.
* Otherwise a resizing after a rotation may cause the parts to sized a lot bigger than might be expected,
* because the desiredSize of a Part is always in document coordinates, not rotated.
* <p>
* This tool probably will not do what you want with Groups that have Placeholders,
* because Placeholders always have a size based on the document coordinates of the member nodes,
* not on the size of some temporary rectangle that has been rotated.
*/
function ResizeRotateMultipleTool() {
  go.ResizingTool.call(this);
  this.name = "Multiple";

  // extra space between the bounds of the selected Parts and the multiSelectionBox
  this.padding = new go.Margin(8);
  // rotation parameterization, copied from RotatingTool
  this.snapAngleMultiple = 45;
  this.snapAngleEpsilon = 2;

  // internal state
  // holds references to all selected non-Link Parts and their relative bounds
  this.initialInfo = null;
  // original bounds, set in doActivate
  this.originalSize = null;
  this.originalPos = null;
  this.startingSize = null;
  this.startingCenter = null;
  this.startingAngle = 0;
  this.isUpdating = false;

  // listeners for updating the multiSelectionBox
  var tool = this;
  this.changedSelectionHandler = function(e) {
    tool.updateMultiSelectionBox();
  };
  this.modelChangedHandler = function(e) {
    if (e.isTransactionFinished && e.oldValue !== "Multiple") {
      tool.updateMultiSelectionBox();
    }
  };
  // the Tool Part surrounding all selected Parts when there is more than one
  this.multiSelectionBox =
      go.GraphObject.make(go.Part,
        {
          layerName: "Tool",  // this transient unmodeled part is not in the document bounds nor is it undoable
          isAnimated: false,  // don't animate its movement
          pickable: false,  // users cannot interact with this Part directly, but only via its Adornment's handles
          layerChanged: function(part, oldlayer, newlayer) {
            var diagram = tool.diagram;
            if (diagram === null) return;
            if (oldlayer === null) {  // add listeners
              diagram.addDiagramListener("ChangedSelection", tool.changedSelectionHandler);
              diagram.addModelChangedListener(tool.modelChangedHandler);
            } else if (newlayer === null) {  // remove listeners
              diagram.removeDiagramListener("ChangedSelection", tool.changedSelectionHandler);
              diagram.removeModelChangedListener(tool.modelChangedHandler);
            }
          },
          isSelected: true,  // always selected, to show this tool's adornment
          selectionAdorned: false,  // but don't show the standard selection adornment
          locationSpot: go.Spot.Center, locationObjectName: "SHAPE",  // the natural location when rotating
          resizable: false, resizeObjectName: "SHAPE",  // NOT resizable by ResizingTool, but this tool uses .resizeObject
          rotatable: false, rotateObjectName: "SHAPE",  // NOT rotatable by RotatingTool, but this tool uses .rotateObject
          resizeAdornmentTemplate:
            go.GraphObject.make(go.Adornment, "Spot",
              { layerName: "Tool", locationSpot: go.Spot.Center },
              go.GraphObject.make(go.Placeholder),
              go.GraphObject.make(go.Shape, "Square",
                { alignment: go.Spot.BottomRight, width: 8, height: 8, stroke: "dodgerblue", fill: "deepskyblue", cursor: "pointer" }),
              go.GraphObject.make(go.Shape, "Square",
                { alignment: go.Spot.Right, width: 8, height: 8, stroke: "dodgerblue", fill: "deepskyblue", cursor: "pointer" }),
              go.GraphObject.make(go.Shape, "Square",
                { alignment: go.Spot.Bottom, width: 8, height: 8, stroke: "dodgerblue", fill: "deepskyblue", cursor: "pointer" }),
              go.GraphObject.make(go.Shape, "LineV",
                { alignment: new go.Spot(0.5, 0, 0, -15), width: 1, height: 26, stroke: "deepskyblue", strokeWidth: 2, strokeDashArray: [4, 2] }),
              go.GraphObject.make(go.Shape, "Circle",
                { alignment: new go.Spot(0.5, 0, 0, -30), width: 8, height: 8, stroke: "dodgerblue", fill: "deepskyblue", cursor: "grab" })
            )
        },
        go.GraphObject.make(go.Shape,
            { name: "SHAPE", stroke: "deepskyblue", strokeWidth: 2, strokeDashArray: [4, 2], fill: null })
      );
}
go.Diagram.inherit(ResizeRotateMultipleTool, go.ResizingTool);

/**
* Update the position and size of the multiSelectionBox based on the bounds of the selection.
* @this {ResizeRotateMultipleTool}
*/
ResizeRotateMultipleTool.prototype.updateMultiSelectionBox = function() {
  var diagram = this.diagram;
  if (diagram === null) return;
  if (this.isUpdating) return;  // avoid infinite recursion
  this.isUpdating = true;

  var show = this.isEnabled && ((diagram.selection.count > 1) || (diagram.selection.first() instanceof go.Group));
  var msp = this.multiSelectionBox;
  msp.visible = show;
  if (show) {
    msp.rotateObject.angle = 0;
    var b = diagram.computePartsBounds(diagram.selection);
    b.addMargin(this.padding);
    var shape = msp.elt(0);
    shape.desiredSize = b.size;
    msp.location = b.center;
    if (msp.diagram === null) {
      diagram.add(msp);
      // Because msp is a Part but is added when Adornments are added,
      // it must manually ensure its own bounds
      msp.ensureBounds();
    }
  }
  this.initialInfo = null;

  this.isUpdating = false;
};

/**
* Only update the multiSelectionBox.
* @this {ResizeRotateMultipleTool}
*/
ResizeRotateMultipleTool.prototype.updateAdornments = function(part) {
  if (!this.isActive) {
    this.updateMultiSelectionBox();
  }
  if (part === this.multiSelectionBox) {
    var ad = part.findAdornment(this.name);
    if (ad === null) {
      ad = this.makeAdornment(part.resizeObject);
    }
    if (ad !== null) {
      ad.angle = part.rotateObject.angle;
      ad.location = part.resizeObject.getDocumentPoint(ad.locationSpot);
      var place = ad.placeholder;
      if (place !== null) {
        place.desiredSize = part.resizeObject.desiredSize;
      }
      this.updateResizeHandles(ad, ad.angle);
      part.addAdornment(this.name, ad);
      return;
    }
    part.removeAdornment(this.name);
  }
}

/**
* This tool may run when there is a mouse-down event on a resize or a rotate handle,
* the diagram is not read-only and it allows resizing or rotating,
* the left mouse button is being used,
* and this tool's adornment's resize or rotate handle is at the current mouse point.
* @this {ResizeRotateMultipleTool}
* @return {boolean}
*/
ResizeRotateMultipleTool.prototype.canStart = function() {
  if (!this.isEnabled) return false;
  var diagram = this.diagram;
  if (diagram === null || diagram.isReadOnly) return false;
  if (!((diagram.selection.count > 1) || (diagram.selection.first() instanceof go.Group))) return false;
  if (!diagram.allowRotate || !diagram.allowResize) return false;
  if (!diagram.lastInput.left) return false;

  var h = this.findToolHandleAt(diagram.firstInput.documentPoint, this.name);
  return (h !== null && h.part.adornedPart === this.multiSelectionBox);
};

/**
* Calls ResizingTool.doActivate, and then remembers the startingSize and originalPos
* and the initial positions and sizes of the selected Parts and their and distances and angles from the center.
* @this {ResizeRotateMultipleTool}
*/
ResizeRotateMultipleTool.prototype.doActivate = function() {
  go.ResizingTool.prototype.doActivate.call(this);
  var diagram = this.diagram;
  if (diagram === null) return;

  var msp = this.multiSelectionBox;
  if (this.initialInfo === null) {
    // center point of the collection
    this.startingCenter = diagram.computePartsBounds(diagram.selection).center;
    this.startingAngle = 0;
    // compute the original size and position of the collection
    var b = diagram.computePartsBounds(diagram.selection);
    b.addMargin(this.padding);
    this.startingSize = b.size;
    this.originalSize = b.size;
    this.originalPos = b.position;

    // remember initial bounds for each Part
    var infos = new go.Map(go.Part, MultiplePartInfo);
    var tool = this;
    diagram.selection.each(function(part) {
      tool.walkTree(part, infos);
    });
    this.initialInfo = infos;
  } else {
    this.startingCenter = msp.actualBounds.center;
    this.startingAngle = msp.rotateObject.angle;
    this.startingSize = msp.resizeObject.desiredSize;
  }
};

/**
* @ignore
* @this {ResizeRotateMultipleTool}
* @param {Part} part
* @param {Map} infos
*/
ResizeRotateMultipleTool.prototype.walkTree = function(part, infos) {
  if (part === null || part instanceof go.Link) return;
  // saves relative position and size
  var b = part.actualBounds;
  var loc = part.locationObject.getDocumentPoint(go.Spot.Center);
  var locoffset = loc.copy().subtract(part.location);
  var relloc = loc.subtract(this.startingCenter);
  infos.add(part, new MultiplePartInfo(part.resizeObject.actualBounds.size, relloc, locoffset, part.rotateObject.angle));
  // recurse into Groups
  if (part instanceof go.Group) {
    var it = part.memberParts.iterator;
    while (it.next()) this.walkTree(it.value, infos);
  }
};

/**
* @ignore
* Internal class that remembers a Part's relative locations and size and original angle.
*/
function MultiplePartInfo(size, relativeloc, locoffset, rotationAngle) {
  this.size = size;
  this.relativeLocation = relativeloc;
  this.centerLocationOffset = locoffset;
  this.rotationAngle = rotationAngle;  // in degrees
}

/**
* Restore the original location, size, and angle of the selected parts.
* @this {ResizeRotateMultipleTool}
*/
ResizeRotateMultipleTool.prototype.doCancel = function() {
  this.rotate(this.startingAngle);
  this.resize(new go.Rect(this.originalPos, this.startingSize));
  go.ResizingTool.prototype.doCancel.call(this);
};

/**
* When resizing, call resize with a new size determined by calling computeResize;
* otherwise call rotate with a new angle determined by calling computeRotate.
* @this {ResizeRotateMultipleTool}
*/
ResizeRotateMultipleTool.prototype.doMouseMove = function() {
  var diagram = this.diagram;
  if (this.isActive && diagram !== null) {
    var h = this.handle;
    if (h instanceof go.Shape && h.figure === "Circle") {
      var newangle = this.computeRotate(diagram.lastInput.documentPoint);
      this.rotate(newangle);
    } else {
      var docpt = diagram.lastInput.documentPoint;
      var locpt = this.adornedObject.getLocalPoint(docpt);
      var newr = this.computeResize(locpt, this.handle.alignment, new go.Size(1, 1), new go.Size(9999, 9999), new go.Size(1, 1), true);
      this.resize(newr);
    }
  }
};

/**
* Either call computeRotate and then rotate, or call computeResize and then resize.
* @this {ResizeRotateMultipleTool}
*/
ResizeRotateMultipleTool.prototype.doMouseUp = function() {
  var diagram = this.diagram;
  if (this.isActive && diagram !== null) {
    var h = this.handle;
    if (h instanceof go.Shape && h.figure === "Circle") {
      var newangle = this.computeRotate(diagram.lastInput.documentPoint);
      this.rotate(newangle);
    } else {
      var docpt = diagram.lastInput.documentPoint;
      var locpt = this.adornedObject.getLocalPoint(docpt);
      var newr = this.computeResize(locpt, this.handle.alignment, new go.Size(1, 1), new go.Size(9999, 9999), new go.Size(1, 1), true);
      this.resize(newr);
    }
    this.transactionResult = this.name;
  }
  this.stopTool();
};

/**
* Overrides ResizingTool.resize to resize all selected Parts to the proportional new size,
* and reposition them accordingly to maintain relative positions.
* @this {ResizeRotateMultipleTool}
* @param {Rect} newr
*/
ResizeRotateMultipleTool.prototype.resize = function(newr) {
  var diagram = this.diagram;
  if (diagram === null) return;

  var origsz = this.originalSize;
  var fx = newr.width / origsz.width;
  var fy = newr.height / origsz.height;
  var angle = this.startingAngle;
  var center = this.startingCenter;
  var offcntr = new go.Point(newr.width / 2 - this.startingSize.width / 2, newr.height / 2 - this.startingSize.height / 2);
  offcntr.rotate(angle);

  this.initialInfo.each(function(kvp) {
    var part = kvp.key;
    if (part instanceof go.Link) return; // only Nodes and simple Parts
    var info = kvp.value;

    part.resizeObject.desiredSize = new go.Size(fx * info.size.width, fy * info.size.height);

    var loc = new go.Point(fx * (info.relativeLocation.x + origsz.width / 2), fy * (info.relativeLocation.y + origsz.height / 2));
    var cntr = newr.center;
    loc.subtract(cntr).rotate(angle).add(cntr);
    loc.add(center).offset(-newr.width / 2, -newr.height / 2).add(offcntr);
    var locoffset = info.centerLocationOffset.copy();
    locoffset.x *= fx;
    locoffset.y *= fy;
    locoffset.rotate(angle);
    loc.subtract(locoffset);
    part.location = loc;
  });

  // resize the multiSelectionBox itself
  var msp = this.multiSelectionBox;
  msp.location = center.copy().add(offcntr);
  msp.resizeObject.desiredSize = newr.size;
}

/**
* Compute the new angle given a point. If the Shift key is pressed, this method ignores the snapAngleMultiple and snapAngleEpsilon.
* <p>
* If the angle is close (by snapAngleEpsilon degrees)
* to a multiple of snapAngleMultiple degrees,
* make it exactly that multiple.
* @this {ResizeRotateMultipleTool}
* @param {Point} newPoint in document coordinates.
* @return {number} the new angle, in degrees.
*/
ResizeRotateMultipleTool.prototype.computeRotate = function(newPoint) {
  var a = this.startingCenter.directionPoint(newPoint);
  var interval = Math.min(Math.abs(this.snapAngleMultiple), 180);
  var epsilon = Math.min(Math.abs(this.snapAngleEpsilon), interval / 2);
  // if it's close to a multiple of INTERVAL degrees, make it exactly so
  if ((this.diagram === null || !this.diagram.lastInput.shift) && interval > 0 && epsilon > 0) {
    if (a % interval < epsilon) {
      a = Math.floor(a / interval) * interval;
    } else if (a % interval > interval - epsilon) {
      a = (Math.floor(a / interval) + 1) * interval;
    }
  }
  var ang = a + 90;  // for zero angle, the rotate handle is above Placeholder, not to the right
  if (ang >= 360) ang -= 360;
  else if (ang < 0) ang += 360;
  return ang;
};

/**
* Rotate all selected Parts about their collective center.
* @this {ResizeRotateMultipleTool}
* @param {number} newangle
*/
ResizeRotateMultipleTool.prototype.rotate = function(newangle) {
  var diagram = this.diagram;
  if (diagram === null) return;
  var cp = this.startingCenter;
  var sz = this.startingSize;
  var origsz = this.originalSize;
  var fx = sz.width / origsz.width;
  var fy = sz.height / origsz.height;
  var newcntr = new go.Point(sz.width / 2, sz.height / 2);

  this.initialInfo.each(function(kvp) {
    var part = kvp.key;
    if (part instanceof go.Link) return; // only Nodes and simple Parts
    var info = kvp.value;

    part.rotateObject.angle = info.rotationAngle + newangle;

    var loc = new go.Point(fx * (info.relativeLocation.x + origsz.width / 2), fy * (info.relativeLocation.y + origsz.height / 2));
    var dist = Math.sqrt(newcntr.distanceSquaredPoint(loc));
    var dir = newcntr.directionPoint(loc);
    var newrad = (newangle + dir) * (Math.PI / 180);
    var locoffset = info.centerLocationOffset.copy();
    locoffset.x *= fx;
    locoffset.y *= fy;
    locoffset.rotate(newangle);
    part.location = new go.Point(cp.x + dist * Math.cos(newrad), cp.y + dist * Math.sin(newrad)).subtract(locoffset);
  });

  // rotate the multiSelectionBox itself
  var msp = this.multiSelectionBox;
  msp.rotateObject.angle = newangle;
}

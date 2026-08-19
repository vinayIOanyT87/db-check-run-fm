"use strict";

/*  EDITED BY VAREC */

/*
*  Copyright (C) 1998-2016 by Northwoods Software Corporation. All Rights Reserved.
*/

/**
* @constructor
* @extends CommandHandler
* @class 
* This CommandHandler class allows the user to position selected Parts in a diagram
* relative to the first part selected, in addition to overriding the doKeyDown method
* of the CommandHandler for handling the arrow keys in additional manners.
* <p>
* Typical usage:
* <pre>
*   $(go.Diagram, "myDiagramDiv",
*     {
*       commandHandler: $(DrawCommandHandler),
*       . . .
*     }
*   )
* </pre>
* or:
* <pre>
*    myDiagram.commandHandler = new DrawCommandHandler();
* </pre>*/
function DrawCommandHandler() {
  go.CommandHandler.call(this);
  var thisObj = this;
  this._arrowKeyBehavior = "move";
  this._pasteOffset = new go.Point(10, 10);
  this._lastPasteOffset = new go.Point(0, 0);

  this.UpdateGroupPartsLocationAndPosition = function (obj)
  {

      //Prevent Object Null References
      if (!obj || !thisObj.diagram || !thisObj.diagram.model) {
          return;
      }

      //Get Reference to diagram model
      var m = thisObj.diagram.model;

      if (obj.data) {
         
          m.setDataProperty(obj.data, 'loc', go.Point.stringify(obj.location));
          m.setDataProperty(obj.data, 'pos', go.Point.stringify(obj.position));
      }
      if (obj instanceof go.Group) {

          obj.memberParts.each(function (part) {
              thisObj.UpdateGroupPartsLocationAndPosition(part);
          });
      }

  }

  this.UpdateObjectDataModelWithPositionAndLocation = function ()
  {
      if (!thisObj ||
          !thisObj.diagram ||
          !thisObj.diagram.selection ||
          thisObj.diagram.selection.count === 0)
      {
          return;
      }

      var objs = thisObj.diagram.selection;

      if ((objs instanceof go.Set || objs instanceof go.List)) {
          objs.each(function (o) {
              thisObj.UpdateGroupPartsLocationAndPosition(o);
          });
          return;
      }
  }

  // This function will persist draw object z-orders to be restored after the
  // copy. For some reason GoJS wants to change the z order and put the selected
  // objects on top.
  this.PersistOriginalZorderValues = function()
  {
		if (!thisObj ||
			!thisObj.diagram ||
			!thisObj.diagram.selection ||
			thisObj.diagram.selection.count === 0)
		{
  			return null;
		}

		var objectZorderList = [];
		var selectedObjectList = thisObj.diagram.selection.toArray();

  		// Need to save the original z-order since GoJS reset them on copy.
		for (var nextObj = 0; nextObj < selectedObjectList.length; nextObj++)
		{
			this.diagram.model.setDataProperty(selectedObjectList[nextObj].data, 'senderDiagramgoHashid', thisObj.diagram.__gohashid);
			this.diagram.model.setDataProperty(selectedObjectList[nextObj].data, 'senderDiagramType', thisObj.diagram.model.modelData.PanelType);
			this.diagram.model.setDataProperty(selectedObjectList[nextObj].data, 'senderDiagramGUID', thisObj.diagram.model.modelData.PointTemplateGuid);
			var objectZorder = new Object();
			objectZorder.originalZorder = selectedObjectList[nextObj].zOrder;
			objectZorder.drawObject = selectedObjectList[nextObj];

			objectZorderList.push(objectZorder);
		}

	    return objectZorderList;
  }
}
go.Diagram.inherit(DrawCommandHandler, go.CommandHandler);

// Override the copySelection function to ensure that all selected objects data model
// properties (loc and pos) are updated.  This is to overcome the need to have two way bindings
// on location and position.   Having two way bindings on location and position is a major performance
// issue when moving hundreds of objects all at once. By ensuring that the data model is updated
// pasted objects will be properly set in the diagram.
DrawCommandHandler.prototype.copySelection = function()
{
	// Persist the original draw object z ordering.
	var objectZorderList = this.PersistOriginalZorderValues();

    this.UpdateObjectDataModelWithPositionAndLocation();
    go.CommandHandler.prototype.copySelection.call(this);

	// Restore the draw object z ordering to their original settings.
    if ( objectZorderList != null && objectZorderList.length > 0 )
    {
    	for (var nextObj = 0; nextObj < objectZorderList.length; nextObj++)
    	{
    		var objectZorder = objectZorderList[nextObj];
    		objectZorder.drawObject.zOrder = objectZorder.originalZorder;
    	}
    }
}

/**
* This controls whether or not the user can invoke the {@link #alignLeft}, {@link #alignRight}, 
* {@link #alignTop}, {@link #alignBottom}, {@link #alignCenterX}, {@link #alignCenterY} commands.
* @this {DrawCommandHandler}
* @return {boolean}
* This returns true:
* if the diagram is not {@link Diagram#isReadOnly},
* if the model is not {@link Model#isReadOnly}, and
* if there are at least two selected {@link Part}s.
*/
DrawCommandHandler.prototype.canAlignSelection = function() {
  var diagram = this.diagram;
  if (diagram === null || diagram.isReadOnly || diagram.isModelReadOnly) return false;
  if (diagram.selection.count < 2) return false;
  return true;
};

/**
* Aligns selected parts along the left-most edge of the left-most part.
* @this {DrawCommandHandler}
*/
DrawCommandHandler.prototype.alignLeft = function() {
  var diagram = this.diagram;
  diagram.startTransaction("aligning left");
  var minPosition = Infinity;
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    minPosition = Math.min(current.position.x, minPosition);
  });
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    current.move(new go.Point(minPosition, current.position.y));
  });
  diagram.commitTransaction("aligning left");
};

/**
* Aligns selected parts at the right-most edge of the right-most part.
* @this {DrawCommandHandler}
*/
DrawCommandHandler.prototype.alignRight = function() {
  var diagram = this.diagram;
  diagram.startTransaction("aligning right");
  var maxPosition = -Infinity;
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    var rightSideLoc = current.actualBounds.x + current.actualBounds.width;
    maxPosition = Math.max(rightSideLoc, maxPosition);
  });
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    current.move(new go.Point(maxPosition - current.actualBounds.width, current.position.y));
  });
  diagram.commitTransaction("aligning right");
};

/**
* Aligns selected parts at the top-most edge of the top-most part.
* @this {DrawCommandHandler}
*/
DrawCommandHandler.prototype.alignTop = function() {
  var diagram = this.diagram;
  diagram.startTransaction("alignTop");
  var minPosition = Infinity;
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    minPosition = Math.min(current.position.y, minPosition);
  });
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    current.move(new go.Point(current.position.x, minPosition));
  });
  diagram.commitTransaction("alignTop");
};

/**
* Aligns selected parts at the bottom-most edge of the bottom-most part.
* @this {DrawCommandHandler}
*/
DrawCommandHandler.prototype.alignBottom = function() {
  var diagram = this.diagram;
  diagram.startTransaction("aligning bottom");
  var maxPosition = -Infinity;
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    var bottomSideLoc = current.actualBounds.y + current.actualBounds.height;
    maxPosition = Math.max(bottomSideLoc, maxPosition);
  });
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    current.move(new go.Point(current.actualBounds.x, maxPosition - current.actualBounds.height));
  });
  diagram.commitTransaction("aligning bottom");
};

/**
* Aligns selected parts at the x-value of the center point of the first selected part. 
* @this {DrawCommandHandler}
*/
DrawCommandHandler.prototype.alignCenterX = function() {
  var diagram = this.diagram;
  var firstSelection = diagram.selection.first();
  if (!firstSelection) return;
  diagram.startTransaction("aligning Center X");
  var centerX = firstSelection.actualBounds.x + firstSelection.actualBounds.width / 2;
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    current.move(new go.Point(centerX - current.actualBounds.width / 2, current.actualBounds.y));
  });
  diagram.commitTransaction("aligning Center X");
};


/**
* Aligns selected parts at the y-value of the center point of the first selected part. 
* @this {DrawCommandHandler}
*/
DrawCommandHandler.prototype.alignCenterY = function() {
  var diagram = this.diagram;
  var firstSelection = diagram.selection.first();
  if (!firstSelection) return;
  diagram.startTransaction("aligning Center Y");
  var centerY = firstSelection.actualBounds.y + firstSelection.actualBounds.height / 2;
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    current.move(new go.Point(current.actualBounds.x, centerY - current.actualBounds.height / 2));
  });
  diagram.commitTransaction("aligning Center Y");
};


/**
* Aligns selected parts top-to-bottom in order of the order selected.
* Distance between parts can be specified. Default distance is 0.
* @this {DrawCommandHandler}
* @param {number} distance 
*/
DrawCommandHandler.prototype.alignColumn = function(distance) {
  var diagram = this.diagram;
  diagram.startTransaction("align Column");
  if (distance === undefined) distance = 0; // for aligning edge to edge
  distance = parseFloat(distance);
  var selectedParts = new Array();
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    selectedParts.push(current);
  });
  for (var i = 0; i < selectedParts.length - 1; i++) {
    var current = selectedParts[i];
    // adds distance specified between parts
    var curBottomSideLoc = current.actualBounds.y + current.actualBounds.height + distance;
    var next = selectedParts[i + 1];
    next.move(new go.Point(current.actualBounds.x, curBottomSideLoc));
  }
  diagram.commitTransaction("align Column");
};

/**
* Aligns selected parts left-to-right in order of the order selected.
* Distance between parts can be specified. Default distance is 0.
* @this {DrawCommandHandler}
* @param {number} distance 
*/
DrawCommandHandler.prototype.alignRow = function(distance) {
  if (distance === undefined) distance = 0; // for aligning edge to edge
  distance = parseFloat(distance);
  var diagram = this.diagram;
  diagram.startTransaction("align Row");
  var selectedParts = new Array();
  diagram.selection.each(function(current) {
    if (current instanceof go.Link) return; // skips over go.Link
    selectedParts.push(current);
  });
  for (var i = 0; i < selectedParts.length - 1; i++) {
    var current = selectedParts[i];
    // adds distance specified between parts
    var curRightSideLoc = current.actualBounds.x + current.actualBounds.width + distance;
    var next = selectedParts[i + 1];
    next.move(new go.Point(curRightSideLoc, current.actualBounds.y));
  }
  diagram.commitTransaction("align Row");
};


/**
* This controls whether or not the user can invoke the {@link #rotate} command.
* @this {DrawCommandHandler}
* @param {number=} angle the positive (clockwise) or negative (counter-clockwise) change in the rotation angle of each Part, in degrees.
* @return {boolean}
* This returns true:
* if the diagram is not {@link Diagram#isReadOnly},
* if the model is not {@link Model#isReadOnly}, and
* if there is at least one selected {@link Part}.
*/
DrawCommandHandler.prototype.canRotate = function(number) {
  var diagram = this.diagram;
  if (diagram === null || diagram.isReadOnly || diagram.isModelReadOnly) return false;
  if (diagram.selection.count < 1) return false;
  return true;
};

/**
* Change the angle of the parts connected with the given part. This is in the command handler
* so it can be easily accessed for the purpose of creating commands that change the rotation of a part. 
* @this {DrawCommandHandler}
* @param {number=} angle the positive (clockwise) or negative (counter-clockwise) change in the rotation angle of each Part, in degrees.
*/
DrawCommandHandler.prototype.rotate = function(angle) {
  if (angle === undefined) angle = 90;
  var diagram = this.diagram;
  diagram.startTransaction("rotate " + angle.toString());
  var diagram = this.diagram;
  diagram.selection.each(function(current) {
    if (current instanceof go.Link || current instanceof go.Group) return; // skips over Links and Groups
    current.angle += angle;
  });
  diagram.commitTransaction("rotate " + angle.toString());
};


/**
* This implements custom behaviors for arrow key keyboard events.
* Set {@link #arrowKeyBehavior} to "select", "move" (the default), "scroll" (the standard behavior), or "none"
* to affect the behavior when the user types an arrow key.
* @this {DrawCommandHandler}*/
DrawCommandHandler.prototype.doKeyDown = function() {
  var diagram = this.diagram;
  if (diagram === null) return;
  var e = diagram.lastInput;
  var control = e.control;
	var shift = e.shift;
  // determines the function of the arrow keys
  if (e.key === "Up" || e.key === "Down" || e.key === "Left" || e.key === "Right") {
    var behavior = this.arrowKeyBehavior;
    if (behavior === "none") {
      // no-op
      return;
    } else if (behavior === "select") {
      this._arrowKeySelect();
      return;
    } else if ( behavior === "move" )
      { 
          if(e.shift === false )
          {
              this._arrowKeyMove();
              return;
          }
          else
          {
              this._arrowKeySize();
              return;
          }
      }
  }

  if (e.key=== 'Z' && control && shift) {
  	FMDrawIndex.SetZoomLevel(100.0);
	  return;
  }
  if (e.key === 'Y' && control && shift)
  {
	  FMDrawIndex.ResetViewportToOrigin();
  	return;
  }
  // otherwise drop through to get the default scrolling behavior

  // otherwise still does all standard commands
  go.CommandHandler.prototype.doKeyDown.call(this);
};

/**
* Collects in an Array all of the non-Link Parts currently in the Diagram.
* @this {DrawCommandHandler}
* @return {Array}
*/
DrawCommandHandler.prototype._getAllParts = function() {
  var allParts = new Array();
  this.diagram.nodes.each(function(node) { allParts.push(node); });
  this.diagram.parts.each(function(part) { allParts.push(part); });
  // note that this ignores Links
  return allParts;
};

DrawCommandHandler.prototype.adjustdataforkeystrokewhensnap  = function()
{
    // adjust the spacing to align to the grid
    //var lastPosition = part.actualBounds.copy();
    // use the grid snap to settings
    var xdistance = diagram.toolManager.draggingTool.gridSnapCellSize.width;
    var ydistance = diagram.toolManager.draggingTool.gridSnapCellSize.height;
    var xoffset = 0;
    var yoffset = 0;
    /*
    // the grid is always in integer spacing so round the current mouse position
    // use modulus to determine where we are
    lastPosition.x = Math.round(lastPosition.x);
    lastPosition.y = Math.round(lastPosition.y);
    var xExtra = lastPosition.x % xdistance;
    var yExtra = lastPosition.y % ydistance;

    // reset the values
    if (xExtra <= (xdistance / 2)) {
        xoffset = xExtra;
    }
    else {
        xoffset = (xdistance - xExtra) * -1;
    }
    if (yExtra <= (ydistance / 2)) {
        yoffset = yExtra * -1;
    }
    else {
        yoffset = (ydistance - yExtra);
    }
    hdistance += xoffset;
    vdistance += yoffset;
    */
}

/**
* To be called when arrow keys should move the Diagram.selection.
* @this {DrawCommandHandler}
*/
DrawCommandHandler.prototype._arrowKeyMove = function() {
  var diagram = this.diagram;
  var e = diagram.lastInput;
  // moves all selected parts in the specified direction
  var vdistance = 0;
  var hdistance = 0;
    // if control is being held down, move pixel by pixel. Else, moves by grid cell size  
    // modified by CT, now if snap to grid is on, distance is equal to cellsize, otherwise, move one pixel
  if (diagram.toolManager.draggingTool.isGridSnapEnabled == true)
  {
      hdistance = diagram.toolManager.draggingTool.gridSnapCellSize.width;
      vdistance = diagram.toolManager.draggingTool.gridSnapCellSize.height;
  }
  else {
    vdistance = 1;
    hdistance = 1;
  }
  diagram.startTransaction("arrowKeyMove");
  diagram.selection.each(function (part) {
    if (diagram.toolManager.draggingTool.isGridSnapEnabled == true) {


    var shape = part.findObject("SHAPE");
    if ( !shape )
    {
	    shape = part;
    }
    var lastPosition = shape.actualBounds.copy();
    // use the grid snap to settings
    var xdistance = diagram.toolManager.draggingTool.gridSnapCellSize.width;
    var ydistance = diagram.toolManager.draggingTool.gridSnapCellSize.height;
    var xoffset = 0;
    var yoffset = 0;
    // the grid is always in integer spacing so round the current mouse position
    	// use modulus to determine where we are
    	// using part.actualBounds.x allows us to snap to edges if this is something we want in the future.
    	// Currently using the center as that is what we use elsewhere (mouse drag)
    if (part.category == 'line') {
    	lastPosition.x = Math.round(part.data.LineStartPositionX);
    	lastPosition.y = Math.round(part.data.LineStartPositionY);
    }
    else {
    	lastPosition.x = Math.round(part.actualBounds.centerX);
    	lastPosition.y = Math.round(part.actualBounds.centerY);
    }
    var xExtra = lastPosition.x % xdistance;
    var yExtra = lastPosition.y % ydistance;

    	// reset the values
    if (xExtra <= (xdistance / 2)) {
        xoffset = xExtra;// * -1;
    }
    else {
        xoffset = (xdistance - xExtra);
    }
    if (yExtra <= (ydistance / 2)) {
        yoffset = yExtra;
    }
    else {
        yoffset = (ydistance - yExtra);
    }
    hdistance += xoffset;
    vdistance += yoffset;
    }

    if (e.key === "Up") {
    	part.move(new go.Point(part.actualBounds.x, part.actualBounds.y - vdistance));
      if (part.category == 'line') {
          part.data.LineStartPositionY -= vdistance;
          part.data.LineEndPositionY -= vdistance;
      }
    } else if (e.key === "Down") {
    	part.move(new go.Point(part.actualBounds.x, part.actualBounds.y + vdistance));
      if (part.category == 'line') {
          part.data.LineStartPositionY += vdistance;
          part.data.LineEndPositionY += vdistance;
      }
  } else if (e.key === "Left") {
  	part.move(new go.Point(part.actualBounds.x - hdistance, part.actualBounds.y));
      if (part.category == 'line') {
          part.data.LineStartPositionX -= hdistance;
          part.data.LineEndPositionX -= hdistance;
      }
  } else if (e.key === "Right") {
      part.move(new go.Point(part.actualBounds.x + hdistance, part.actualBounds.y));
      if (part.category == 'line') {
          part.data.LineStartPositionX += hdistance;
          part.data.LineEndPositionX += hdistance;
      }
  } 
  });
  diagram.commitTransaction("arrowKeyMove");
};


/**
* To be called when arrow keys should resize the Diagram.selection.
* @this {DrawCommandHandler}
*/
DrawCommandHandler.prototype._arrowKeySize = function()
{
    var diagram = this.diagram;
    var e = diagram.lastInput;
    // moves all selected parts in the specified direction
    var vdistance = 0;
    var hdistance = 0;
    // if control is being held down, move pixel by pixel. Else, moves by grid cell size  
    // modified by CT, now if snap to grid is on, distance is equal to cellsize, otherwise, move one pixel
    if ( diagram.toolManager.draggingTool.isGridSnapEnabled === true )
    {
        hdistance = diagram.toolManager.draggingTool.gridSnapCellSize.width;
        vdistance = diagram.toolManager.draggingTool.gridSnapCellSize.height;
    }
    else
    {
        vdistance = 1;
        hdistance = 1;
    }
    diagram.startTransaction( 'arrowKeySize' );
    diagram.selection.each( function( part )
    {
        var obj = part.resizeObject;
        if ( obj && obj.desiredSize )
        {
            if ( part.position )
            {
                var pos = new go.Point( part.position.x, part.position.y );
                var newSize = new go.Size( obj.desiredSize.width, obj.desiredSize.height );
                if ( e.key === 'Up' )
                {
                    newSize.height -= vdistance;
                }
                else if ( e.key === 'Down' )
                {
                    newSize.height += vdistance;
                }
                else if ( e.key === 'Left' )
                {
                    newSize.width -= hdistance;
                }
                else if ( e.key === 'Right' )
                {
                    newSize.width += hdistance;
                }
                obj.desiredSize = newSize;
                part.position = pos;
            }
        }
        

    } );
    diagram.commitTransaction( 'arrowKeySize' );
};

/**
* To be called when arrow keys should change selection.
* @this {DrawCommandHandler}
*/
DrawCommandHandler.prototype._arrowKeySelect = function() {
  var diagram = this.diagram;
  var e = diagram.lastInput;
  // with a part selected, arrow keys change the selection
  // arrow keys + shift selects the additional part in the specified direction
  // arrow keys + control toggles the selection of the additional part
  var nextPart = null;
  if (e.key === "Up") {
    nextPart = this._findNearestPartTowards(270);
  } else if (e.key === "Down") {
    nextPart = this._findNearestPartTowards(90);
  } else if (e.key === "Left") {
    nextPart = this._findNearestPartTowards(180);
  } else if (e.key === "Right") {
    nextPart = this._findNearestPartTowards(0);
  }
  if (nextPart !== null) {
    if (e.shift) {
      nextPart.isSelected = true;
    } else if (e.control || e.meta) {
      nextPart.isSelected = !nextPart.isSelected;
    } else {
      diagram.select(nextPart);
    }
  }
};

/**
* Finds the nearest Part in the specified direction, based on their center points.
* if it doesn't find anything, it just returns the current Part.
* @this {DrawCommandHandler}
* @param {number} dir the direction, in degrees
* @return {Part} the closest Part found in the given direction
*/
DrawCommandHandler.prototype._findNearestPartTowards = function(dir) {
  var originalPart = this.diagram.selection.first();
  if (originalPart === null) return null;
  var originalPoint = originalPart.actualBounds.center;
  var allParts = this._getAllParts();
  var closestDistance = Infinity;
  var closest = originalPart;  // if no parts meet the criteria, the same part remains selected

  for (var i = 0; i < allParts.length; i++) {
    var nextPart = allParts[i];
    if (nextPart === originalPart) continue;  // skips over currently selected part
    var nextPoint = nextPart.actualBounds.center;
    var angle = originalPoint.directionPoint(nextPoint);
    var anglediff = this._angleCloseness(angle, dir);
    if (anglediff <= 45) {  // if this part's center is within the desired direction's sector,
      var distance = originalPoint.distanceSquaredPoint(nextPoint);
      distance *= 1+Math.sin(anglediff*Math.PI/180);  // the more different from the intended angle, the further it is
      if (distance < closestDistance) {  // and if it's closer than any other part,
        closestDistance = distance;      // remember it as a better choice
        closest = nextPart;
      }
    }
  }
  return closest;
};

/**
* @this {DrawCommandHandler}
* @param {number} a
* @param {number} dir
* @return {number}
*/
DrawCommandHandler.prototype._angleCloseness = function(a, dir) {
  return Math.min(Math.abs(dir - a), Math.min(Math.abs(dir + 360 - a), Math.abs(dir - 360 - a)));
};

/**
* Reset the last offset for pasting.
* @override
* @this {DrawCommandHandler}
* @param {Iterable.<Part>} coll a collection of {@link Part}s.
*/
DrawCommandHandler.prototype.copyToClipboard = function(coll) {
  go.CommandHandler.prototype.copyToClipboard.call(this, coll);
  this._lastPasteOffset.set(this.pasteOffset);
};

/**
* Paste from the clipboard with an offset incremented on each paste, and reset when copied.
* @override
* @this {DrawCommandHandler}
* @return {Set.<Part>} a collection of newly pasted {@link Part}s
*/
DrawCommandHandler.prototype.pasteFromClipboard = function() {
  var coll = go.CommandHandler.prototype.pasteFromClipboard.call(this);
  this.diagram.moveParts(coll, this._lastPasteOffset);
  this._lastPasteOffset.add(this.pasteOffset);
  var it = coll.iterator;
  while (it.next()) {
  var data = it.value.data;

	//if we are copying to a new diagram, we need to remove p and pt links (guids)
  if ((it.value.data.senderDiagramgoHashid != this.diagram.__gohashid) &&
	it.value.data.senderDiagramType === "Detail" && this.diagram.model.modelData.PanelType == "Detail"
  && it.value.data.senderDiagramGUID != this.diagram.model.modelData.PointTemplateGuid) {

  		this.diagram.model.setDataProperty(it.value.data, 'AnimationPointValueAssignments', undefined);

  		switch (it.value.data.category) {
  			case "button":
  				FMDrawPropertyMenu.ClearButtonTagData(it.value, false);
  				this.diagram.model.setDataProperty(it.value.data, 'buttonActionObjectGuid', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'buttonActionObjectId', undefined);
  				if (this.diagram.model.modelData.PanelType == "Detail") { //set the point guid to the details point template guid
  					this.diagram.model.setDataProperty(it.value.data, 'PointGUID', this.diagram.model.modelData.PointTemplateGuid);
                        if (it.value.data.buttonActionType != "BUTTON_POINT_TREND")
                            this.diagram.model.setDataProperty(it.value.data, 'PointTemplateTagSelectionIndicator', false);
                        else
                            this.diagram.model.setDataProperty(it.value.data, 'PointTemplateTagSelectionIndicator', true);
  				}
  				break;
  			case "bar":
  				this.diagram.model.setDataProperty(it.value.data, 'PointGUID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagGUID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagPointID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagPointIDAndTagID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagTagID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'PointTemplateTagSelectionIndicator', false);
  				if (this.diagram.model.modelData.PanelType == "Detail") {
  					this.diagram.model.setDataProperty(it.value.data, 'PointTemplateTagSelectionIndicator', true)
  					this.diagram.model.setDataProperty(it.value.data, 'PointGUID', this.diagram.model.modelData.PointTemplateGuid);
  				}
  				break;
  			case "tag":
  				this.diagram.model.setDataProperty(it.value.data, 'PointGUID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagGUID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagPointID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagPointIDAndTagID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagTagID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'ToolTipString', "Unassigned");
  				this.diagram.model.setDataProperty(it.value.data, 'PointTemplateTagSelectionIndicator', false);
  				if (this.diagram.model.modelData.PanelType == "Detail") {
  					this.diagram.model.setDataProperty(it.value.data, 'PointTemplateTagSelectionIndicator', true)
  					this.diagram.model.setDataProperty(it.value.data, 'PointGUID', this.diagram.model.modelData.PointTemplateGuid);
  				}
  				break;
  		}
  }
  	else if  ((it.value.data.senderDiagramgoHashid != this.diagram.__gohashid) &&it.value.data.senderDiagramType === "Detail" && this.diagram.model.modelData.PanelType == "Standard")
{
  		if (it.value.data.PointTemplateTagSelectionIndicator == true) {
  				this.diagram.model.setDataProperty(it.value.data, 'PointTemplateTagSelectionIndicator', false);
  				this.diagram.model.setDataProperty(it.value.data, 'PointGUID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagGUID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagPointID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagPointIDAndTagID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'TagTagID', undefined);
  				this.diagram.model.setDataProperty(it.value.data, 'ToolTipString', "Unassigned");
  				this.diagram.model.setDataProperty(it.value.data, 'PointTemplateTagSelectionIndicator', false);
  				if (it.value.data.category == "button")
  		{
  					FMDrawPropertyMenu.ClearButtonTagData(it.value, false);
  					this.diagram.model.setDataProperty(it.value.data, 'buttonActionObjectGuid', undefined);
  					this.diagram.model.setDataProperty(it.value.data, 'buttonActionObjectId', undefined);
			  }
  	}
  }

  };

  return coll;
};

/**
* Gets or sets the arrow key behavior. Possible values are "move", "select", and "scroll".  
* The default value is "move".
* @name DrawCommandHandler#arrowKeyBehavior
* @function.
* @return {string}
*/
Object.defineProperty(DrawCommandHandler.prototype, "arrowKeyBehavior", {
  get: function() { return this._arrowKeyBehavior; },
  set: function(val) {
    if (val !== "move" && val !== "select" && val !== "scroll" && val !== "none") {
      throw new Error("DrawCommandHandler.arrowKeyBehavior must be either \"move\", \"select\", \"scroll\", or \"none\", not: " + val);
    }
    this._arrowKeyBehavior = val;
  }
});

/**
* Gets or sets the offset at which each repeated pasteSelection() puts the new copied parts from the clipboard.
* @name DrawCommandHandler#pasteOffset
* @function.
* @return {Point}
*/
Object.defineProperty(DrawCommandHandler.prototype, "pasteOffset", {
  get: function() { return this._pasteOffset; },
  set: function(val) {
    if (!(val instanceof go.Point)) throw new Error("DrawCommandHandler.pasteOffset must be a Point, not: " + val);
    this._pasteOffset.set(val);
  }
});

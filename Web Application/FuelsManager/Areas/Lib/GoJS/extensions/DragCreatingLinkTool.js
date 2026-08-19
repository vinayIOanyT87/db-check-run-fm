'use strict';

/*
*  Copyright (C) 1998-2016 by Northwoods Software Corporation. All Rights Reserved.
*/

// A custom Tool for creating a new Node with custom size by dragging its outline in the background.

/**
* @constructor
* @extends Tool
* @class
* The DragCreatingLinkTool lets the user create a new node by dragging in the background
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
*   diagram.currentTool = new DragCreatingLinkTool();
* </code></pre>
* <p/>
* Use this tool in a mode-less manner by executing:
* <pre><code>
*   myDiagram.toolManager.mouseMoveTools.insertAt(2, new DragCreatingLinkTool());
* </code></pre>
* However when used mode-lessly as a mouse-move tool, in {@link ToolManager#mouseMoveTools},
* this cannot start running unless there has been a motionless delay
* after the mouse-down event of at least {@link #delay} milliseconds.
* <p/>
* This tool does not utilize any {@link Adornment}s or tool handles,
* but it does temporarily add the {@link #box} Part to the diagram.
* This tool does conduct a transaction when inserting the new node.
*/
function DragCreatingLinkTool()
{
    go.Tool.call( this );
    this.name = 'DragCreatingLink';

    /** @type {Object} */
    this._archetypeNodeData = null;

    var b = new go.Part();
    b.layerName = 'Tool';
    b.selectable = false;
    var r = new go.Shape();
    r.name = 'SHAPE';
    r.figure = 'Rectangle';
    r.fill = null;
    r.stroke = 'magenta';
    r.position = new go.Point( 0, 0 );
    b.add( r );
    /** @type {Part} */
    this._box = b;

    /** @type {number} */
    this._delay = 175;
}

go.Diagram.inherit( DragCreatingLinkTool, go.Tool );

/**
* This tool can run when there has been a mouse-drag, far enough away not to be a click,
* and there has been delay of at least {@link #delay} milliseconds
* after the mouse-down before a mouse-move.
* <p/>
* This method may be overridden.
* @this {DragCreatingLinkTool}
* @return {boolean}
*/
DragCreatingLinkTool.prototype.canStart = function()
{
    if ( !this.isEnabled )
    {
        return false;
    }

    // gotta have some node data that can be copied
    if ( this.archetypeNodeData === null )
    {
        return false;
    }

    var diagram = this.diagram;
    if ( !diagram )
    {
        return false;
    }
    // heed IsReadOnly & AllowInsert
    if ( diagram.isReadOnly || diagram.isModelReadOnly )
    {
        return false;
    }
    if ( !diagram.allowInsert )
    {
        return false;
    }

    var e = diagram.lastInput;
    // require left button & that it has moved far enough away from the mouse down point, so it isn't a click
    if ( !e.left )
    {
        return false;
    }
    // don't include the following checks when this tool is running modally
    if ( diagram.currentTool !== this )
    {
        if ( !this.isBeyondDragSize() )
        {
            return false;
        }
        // must wait for "delay" milliseconds before that tool can run
        if ( e.timestamp - diagram.firstInput.timestamp < this.delay )
        {
            return false;
        }
    }
    return true;
};

/**
* Capture the mouse and show the {@link #box}.
* @this {DragCreatingLinkTool}
*/
DragCreatingLinkTool.prototype.doActivate = function()
{
    var diagram = this.diagram;
    if ( !diagram )
    {
        return;
    }
    this.isActive = true;
    diagram.isMouseCaptured = true;
    this.box = diagram.linkTemplateMap.getValue( this.boxArchetypeNodeData.category );
    this.box.stroke = this.boxArchetypeNodeData.stroke;
    diagram.skipsUndoManager = true;
    diagram.add( this.box );
    diagram.skipsUndoManager = false;
    this.doMouseMove();
};

/**
* Release the mouse and remove any {@link #box}.
* @this {DragCreatingLinkTool}
*/
DragCreatingLinkTool.prototype.doDeactivate = function()
{
    var diagram = this.diagram;
    if ( !diagram )
    {
        return;
    }
    diagram.skipsUndoManager = true;
    diagram.remove( this.box );
    diagram.skipsUndoManager = false;
    diagram.isMouseCaptured = false;
    this.isActive = false;
};

/**
* Update the {@link #box}'s position and size according to the value
* of {@link #computeBoxBounds}.
* @this {DragCreatingLinkTool}
*/
DragCreatingLinkTool.prototype.doMouseMove = function()
{
    var diagram = this.diagram;
    if ( !diagram )
    {
        return;
    }
    diagram.skipsUndoManager = true;
    if ( this.isActive && this.box !== null )
    {
        var r = this.computePoints();
        var link = this.box;
        link.points = r;
    }
    diagram.skipsUndoManager = false;
};

/**
* Call {@link #insertPart} with the value of a call to {@link #computeBoxBounds}.
* @this {DragCreatingLinkTool}
*/
DragCreatingLinkTool.prototype.doMouseUp = function()
{
    if ( this.isActive )
    {
        var diagram = this.diagram;
        diagram.skipsUndoManager = true;
        diagram.remove( this.box );
        diagram.skipsUndoManager = false;
        try
        {
            diagram.currentCursor = 'wait';
            this.insertPart( this.computePoints() );
        }
        finally
        {
            diagram.currentCursor = '';
        }
    }
    this.stopTool();
};

/**
* This just returns a {@link Rect} stretching from the mouse-down point to the current mouse point.
* <p/>
* This method may be overridden.
* @this {DragCreatingLinkTool}
* @return {Rect} a {@link Rect} in document coordinates.
*/
DragCreatingLinkTool.prototype.computePoints = function()
{
    var diagram = this.diagram;
    if ( !diagram )
    {
        return new go.List( go.Point ).addAll( [new go.Point( 0, 0 ), new go.Point( 0, 0 )] );
    }
    var start = diagram.firstInput.documentPoint;
    var latest = diagram.lastInput.documentPoint;
    return new go.List( go.Point ).addAll( [start.copy(), latest.copy()] );
};

/**
* Create a node by adding a copy of the {@link #archetypeNodeData} object
* to the diagram's model, assign its {@link GraphObject#position} and {@link GraphObject#desiredSize}
* according to the given bounds, and select the new part.
* <p>
* The actual part that is added to the diagram may be a {@link Part}, a {@link Node},
* or even a {@link Group}, depending on the properties of the {@link #archetypeNodeData}
* and the type of the template that is copied to create the part.
* @this {DragCreatingLinkTool}
* @param {Rect} bounds a Point in document coordinates.
* @return {Part} the newly created Part, or null if it failed.
*/
DragCreatingLinkTool.prototype.insertPart = function( points )
{
    var diagram = this.diagram;
    if ( !diagram )
    {
        return null;
    }
    var arch = diagram.linkTemplateMap.getValue( this.archetypeNodeData.category );
    if ( arch === null )
    {
        return null;
    }

    this.startTransaction( this.name );
    var linkData = {
        points: points,
        category: this.archetypeNodeData.category,
        layerName: this.archetypeNodeData.layerName,
        zOrder: this.archetypeNodeData.zOrder,
        stroke: this.archetypeNodeData.stroke,
        strokeWidth: this.archetypeNodeData.strokeWidth

    };
    diagram.model.addLinkData( linkData );
    var link = diagram.findLinkForData( linkData );
    diagram.select( link );

    // set the TransactionResult before raising event, in case it changes the result or cancels the tool
    this.transactionResult = this.name;
    this.stopTransaction();
    return link;
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
* @name DragCreatingLinkTool#box
* @function.
* @return {Part}
*/
Object.defineProperty( DragCreatingLinkTool.prototype, 'box', {
    get: function()
    {
        return this._box;
    },
    set: function( val )
    {
        this._box = val;
    }
} );

/**
* Gets or sets the time in milliseconds for which the mouse must be stationary
* before this tool can be started.
* The default value is 175 milliseconds.
* A value of zero will allow this tool to run without any wait after the mouse down.
* Setting this property does not raise any events.
* @name DragCreatingLinkTool#delay
* @function.
* @return {number}
*/
Object.defineProperty( DragCreatingLinkTool.prototype, 'delay', {
    get: function()
    {
        return this._delay;
    },
    set: function( val )
    {
        this._delay = val;
    }
} );

/**
* Gets or sets a data object that will be copied and added to the diagram's model each time this tool executes.
* The default value is null.
* The value must be non-null for this tool to be able to run.
* Setting this property does not raise any events.
* @name DragCreatingLinkTool#archetypeNodeData
* @function.
* @return {Object}
*/
Object.defineProperty( DragCreatingLinkTool.prototype, 'archetypeNodeData', {
    get: function()
    {
        return this._archetypeNodeData;
    },
    set: function( val )
    {
        this._archetypeNodeData = val;
    }
} );

Object.defineProperty( DragCreatingLinkTool.prototype, 'boxArchetypeNodeData', {
    get: function()
    {
        return this._boxArchetypeNodeData;
    },
    set: function( val )
    {
        this._boxArchetypeNodeData = val;
    }
} );
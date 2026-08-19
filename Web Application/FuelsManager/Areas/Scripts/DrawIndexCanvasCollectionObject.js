FMDrawIndex = FMDrawIndex || {};

FMDrawIndex._CanvasCollectionObject = function( canvasId )
{
    this.PerforCopyForDuplication = true;
    this.SelectionChangeBasedOnPasteFromDuplication = false;

    this.TagSelectionModel = null;

    canvasId = canvasId || canvasIdDefaultString;
    //var jQueryCanvasId = '#' + canvasId + ' > canvas';
    var thisObj = this; //Create closure so that internal event functions can access this object.
    var diagram = $$( go.Diagram, canvasId, {

       
        commandHandler: $$( DrawCommandHandler ),
        //"ChangedSelection" : updateMultiSelectionPart,
        "grid.gridCellSize": new go.Size( 30, 20 ),
        "undoManager.isEnabled": true,
        "undoManager.maxHistoryLength": -1,
        "panningTool.isEnabled": true,
        "dragSelectingTool.isEnabled": false,
        "draggingTool.dragsLink": true,
        "draggingTool.isGridSnapEnabled": true,
        "draggingTool.isComplexRoutingRealtime": false,
        linkReshapingTool: $$(SnapLinkReshapingTool),
        relinkingTool: $$(SnappingRelinkingTool),
        "linkingTool.isUnconnectedLinkValid": true,
        "linkingTool.portGravity": 20,
        "relinkingTool.isUnconnectedLinkValid": true,
        "relinkingTool.portGravity": 20,
        //"relinkingTool.temporaryLink": linkTemplate,
        //"relinkingTool.fromHandleArchetype":
        //    $$( go.Shape, 'Diamond', {
        //        segmentIndex: 0,
        //        cursor: 'pointer',
        //        desiredSize: new go.Size( 8, 8 ),
        //        fill: 'tomato',
        //        stroke: 'darkred'

        //    } ),
        //"relinkingTool.toHandleArchetype":
        //    $$( go.Shape, 'Diamond', {
        //        segmentIndex: -1,
        //        cursor: 'pointer',
        //        desiredSize: new go.Size( 8, 8 ),
        //        fill: 'darkred',
        //        stroke: 'tomato'

        //    } ),
        //"linkReshapingTool.handleArchetype":
        //    $$( go.Shape, 'Diamond', {
        //        desiredSize: new go.Size( 7, 7 ),
        //        fill: 'lightblue',
        //        stroke: 'deepskyblue'

    	//    } ),
        rotatingTool: $$(RotateMultipleTool),
    	//resizingTool: $$(ResizeMultipleTool)
        mouseOver: function( e )
        {
        	var point = e.documentPoint;
        	if ( point )
        		UpdateCoordinatePanel( point );
        },
        "animationManager.isEnabled": false,
    } );

    diagram.groupTemplate = groupTemplate;

    var model = diagram.model;
    this.SetModel = function( mod )
    {
        model = mod;
    };
    diagram.grid =
        $$( go.Panel, 'Grid',
            {
                gridCellSize: new go.Size( 10, 10 )
            },
            $$( go.Shape, 'LineH', { stroke: 'lightgray' } ),
            $$( go.Shape, 'LineV', {
                stroke: 'lightgray'
            } ),
            $$( go.Shape, 'LineH', {
                stroke: 'gray',
                interval: 5
            } ),
            $$( go.Shape, 'LineV', { stroke: 'gray', interval: 5 } )
        );
    diagram.grid.visible = false;

    diagram.toolManager.mouseDownTools.insertAt(3, new GeometryReshapingTool());
    diagram.toolManager.mouseWheelBehavior = go.ToolManager.WheelNone;
    // ReSharper disable once InconsistentNaming
    var layerManager = new FMDrawIndex._LayerManager();

    var tool = $$( PolygonDrawingTool,
        // provide the default JavaScript object for a new polygon in the model
        {
            isPolygon: true, // for a polyline drawing tool set this property to false
            isEnabled: false,
            archetypePartData:
                {
                    fill: FMDrawIndex.shapeFillColor,
                    stroke: FMDrawIndex.shapeStrokeColor,
                    strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
                    layerName: '',
                    zOrder: 0
                },
            doMouseMove: function () {
            	var point = diagram.lastInput.documentPoint;
					if (point)
						UpdateCoordinatePanel(point);
            	PolygonDrawingTool.prototype.doMouseMove.call(this);
            },
            finishShape: function()
            {
                // override PolygonDrawingTool.finsihShape
            	// Assign latest zOrder from the active layer
            	var copy = JSON.parse(JSON.stringify(FMDrawIndex.defaultArchetype)); //copying default node data to get current default properties
					 this.archetypePartData = 	 copy;
					 this.archetypePartData.category = 'polygon';
                var primaryLayerName = layerManager.GetPrimaryLayerName();
                this.archetypePartData.layerName = primaryLayerName;
                this.archetypePartData.zOrder = FMDrawIndex.GetNextPartZOrder(primaryLayerName);
					 if (FMDrawIndex.defaultArchetype.alignment)
					 	this.archetypePartData.alignment = FMDrawIndex.defaultArchetype.alignment.copy();
					 if (FMDrawIndex.defaultArchetype.color)
						if (typeof FMDrawIndex.defaultArchetype.color === 'object')
							this.archetypePartData.color = FMDrawIndex.defaultArchetype.color.copy();
	            if ( !this.isPolygon )
		            this.archetypePartData.color = null;
                // call the base method to do normal behavior and return its result
                var part = PolygonDrawingTool.prototype.finishShape.call( this );
                //Resize Part based on Acutal Bounds so that the part.data.size attribute is set and the Properties window can view the width and height.
                if ( part )
                {
                    //Ignore Resizing of polygon
                    diagram.skipsUndoManager = true;
                    part.resizeObject.desiredSize = new go.Size( part.resizeObject.actualBounds.size.width, part.resizeObject.actualBounds.size.height );
                    diagram.skipsUndoManager = false;
                }
                return part;
            }
        } );

    // install as first mouse-down-tool
    diagram.toolManager.mouseDownTools.insertAt( 0, tool );

    tool = $$( DragCreatingLineTool,
        // provide the default JavaScript object for a new polygon in the model
        {
            isEnabled: false,
            archetypePartData:
                {
                    fill: FMDrawIndex.shapeFillColor,
                    stroke: FMDrawIndex.shapeStrokeColor,
                    strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
                    layerName: '',
                    zOrder: 0
                },
            doMouseMove: function () {
            	var point = diagram.lastInput.documentPoint;
					if (point)
						UpdateCoordinatePanel(point);
            	DragCreatingLineTool.prototype.doMouseMove.call(this);
            },
            finishShape: function()
            {
                // override DragCreateingLineTool.finsihShape
            	// Assign latest zOrder from the active layer
					 var copy = JSON.parse(JSON.stringify(FMDrawIndex.defaultArchetype)); //copying default node data to get current default properties
					 this.archetypePartData = 	 copy;
					 this.archetypePartData.category = 'line';
                var primaryLayerName = layerManager.GetPrimaryLayerName();
                this.archetypePartData.layerName = primaryLayerName;
                this.archetypePartData.zOrder = FMDrawIndex.GetNextPartZOrder(primaryLayerName);
					 if (FMDrawIndex.defaultArchetype.alignment)
						this.archetypePartData.alignment = FMDrawIndex.defaultArchetype.alignment.copy();
					 if (FMDrawIndex.defaultArchetype.color)
						 if ( typeof FMDrawIndex.defaultArchetype.color === 'object' )
							 this.archetypePartData.color = FMDrawIndex.defaultArchetype.patternFillColor;

                // call the base method to do normal behavior and return its result
                var part = DragCreatingLineTool.prototype.finishShape.call( this );
                //Resize Part based on Acutal Bounds so that the part.data.size attribute is set and the Properties window can view the width and height.
                if ( part )
                {
                	//Ignore Resizing of polygon
                	diagram.skipsUndoManager = true;
						
                	part.resizeObject.desiredSize = new go.Size(part.resizeObject.naturalBounds.size.width, part.resizeObject.naturalBounds.size.height);
                    diagram.skipsUndoManager = false;
                }
                return part;
            }
        } );

    // install as first mouse-down-tool
    diagram.toolManager.mouseDownTools.insertAt( 0, tool );

    //Override standardMouseSelect to disregard right mouse click
    diagram.toolManager.clickSelectingTool.standardMouseSelect = function()
    {
        if ( this.diagram && this.diagram.lastInput && this.diagram.lastInput.left )
        {
            go.ClickSelectingTool.prototype.standardMouseSelect.call( this );
        }
    };
    diagram.linkTemplate = linkTemplate;

    diagram.nodeTemplateMap.add( 'triangle', triangleTemplate );
    diagram.nodeTemplateMap.add( 'circle', circleTemplate );
    diagram.nodeTemplateMap.add( 'rectangle', rectangleTemplate );
    diagram.nodeTemplateMap.add( 'text', textTemplate );
    diagram.nodeTemplateMap.add( 'ellipse', ellipseTemplate );
    diagram.nodeTemplateMap.add( 'picture', pictureTemplate );
    diagram.nodeTemplateMap.add( 'tag', tagTemplate );
    diagram.nodeTemplateMap.add( 'button', buttonTemplate );
    diagram.nodeTemplateMap.add( 'polygon', polygonTemplate );
    diagram.nodeTemplateMap.add( 'bar', barTemplate );
    diagram.nodeTemplateMap.add( 'line', lineTemplate );
    diagram.linkTemplateMap.add( 'lineLink', linkTemplate );

    diagram.toolManager.draggingTool.isGridSnapEnabled = false;
    diagram.toolManager.draggingTool.gridSnapCellSize = new go.Size( 10, 10 );
    diagram.toolManager.resizingTool.isGridSnapEnabled = false;

    diagram.scrollMode = go.Diagram.InfiniteScroll;
    diagram.hasHorizontalScrollbar = false;
    diagram.hasVerticalScrollbar = false;

	diagram.toolManager.draggingTool.doDragOver = function( pt, obj )
	{
		var point = pt;

		if ( typeof ( xCordText ) != "undefined" && typeof( yCordText ) != "undefined" )
		{
			xCordText.innerHTML = Math.round( point.x ).toString();
			yCordText.innerHTML = Math.round(point.y).toString();
		}
	};

	diagram.toolManager.resizingTool.doMouseMove = function()
	{
		var point = diagram.lastInput.documentPoint;
	   if (point)
		   UpdateCoordinatePanel(point);
		go.ResizingTool.prototype.doMouseMove.call(diagram.toolManager.resizingTool);
	};

    //"Esc" key switches to select mode
    diagram.commandHandler.doKeyDown = function()
    {
        var e = diagram.lastInput;
        // The meta (Command) key substitutes for "control" for Mac commands
        var key = e.key;
        // Quit on any undo/redo key combination:
        if ( ( key === 'Esc' ) )
        {
            var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
            FMDrawIndex.ResetAllControls( canvas );
            FMDrawIndex.DrawingMode( 'select' );
        }

        if ( ( key === 'Add' ) )
        {
            FMDrawIndex.IncreaseZoom();
            return;
        }

        if ( ( key === 'Subtract' ) )
        {
            FMDrawIndex.DecreaseZoom();
            return;
        }

        if ( e.control === true )
        {
            var bHandled = false;
            switch ( e.key )
            {
                case 'G':
                    if ( e.shift )
                    {
                        bHandled = true; //Disregard base class handling.  GoJS by default support Ctrl + Shift + G. Not desired.
                    }
                    else
                    {
                        //(Ctrl + G)
                        FMDrawIndex.Group();
                        bHandled = true;
                    }
                    break;
                case 'U':
                    if ( e.shift )
                    {
                        //Mimic Visio Command Sequence (Ctrl + Shift + U)
                        FMDrawIndex.Ungroup();
                        bHandled = true;
                    }
                    break;
            }
            if ( bHandled )
            {
                return;
            }
        }


        // call base method with no arguments (default functionality)
        DrawCommandHandler.prototype.doKeyDown.call( this );
    };

    // overridden function to allow for a wider selection area
    // this is neccessary because people would draw a single thickness line
    // and then complain it was too hard to select. We will give it a +/- 6px range vertical and horizontal.
    
    diagram.toolManager.clickSelectingTool.standardMouseSelect = function () {
        // these functions are not really exported so we need to set what we want and then call the original function
        var diagram = this.diagram;
        var exitLoop = false;
        if (diagram === null || !diagram.allowSelect) return;
        var e = diagram.lastInput;
        var curobj = diagram.findPartAt(e.documentPoint, false);  // to select containing Group if Part.canSelect() is false
        if (curobj === null) {
            var incrementAmount = 0.1;
            var Amounttoincrement = incrementAmount;
            var sign = 1.0;
            var maximumPixelDeviation = 6;
            while (exitLoop === false) {
                e.documentPoint.L += (incrementAmount * sign);
                e.documentPoint.M += (incrementAmount * sign);
                curobj = diagram.findPartAt(e.documentPoint, false);
                if (curobj === null) {
                    if (sign > 0) {
                        sign = -1.0;
                        e.documentPoint.L += (incrementAmount * sign);
                        e.documentPoint.M += (incrementAmount * sign);
                    }
                    else {
                        sign = 1.0;
                        e.documentPoint.L += (incrementAmount * sign);
                        e.documentPoint.M += (incrementAmount * sign);
                        incrementAmount += Amounttoincrement;
                        if (incrementAmount > maximumPixelDeviation) {
                            exitLoop = true;
                        }
                    }
                }
                else {
                    exitLoop = true;
                }
            }
        }
        if (curobj !== null) {
            // set the last mouse position so it can be selected
            diagram.lastInput = e;
        }
        // call the default function
        go.Tool.prototype.standardMouseSelect.call(this);
    }
    
    
    diagram.toolManager.draggingTool.findDraggablePart = function () {
        var diagram = this.diagram;
        var exitLoop = false;
        if (diagram === null) return null;
        // to select containing Group if Part.canSelect() is false, allow nonselectable
        var part = diagram.findPartAt(diagram.firstInput.documentPoint, false);
        if (part === null) {
            var incrementAmount = 0.1;
            var Amounttoincrement = incrementAmount;
            var sign = 1.0;
            var maximumPixelDeviation = 6;
            while (exitLoop === false) {
                diagram.firstInput.documentPoint.L += (incrementAmount * sign);
                diagram.firstInput.documentPoint.M += (incrementAmount * sign);
                part = diagram.findPartAt(diagram.firstInput.documentPoint, false);
                if (part === null) {
                    if (sign > 0) {
                        sign = -1.0;
                        diagram.firstInput.documentPoint.L += (incrementAmount * sign);
                        diagram.firstInput.documentPoint.M += (incrementAmount * sign);
                    }
                    else {
                        sign = 1.0;
                        diagram.firstInput.documentPoint.L += (incrementAmount * sign);
                        diagram.firstInput.documentPoint.M += (incrementAmount * sign);
                        incrementAmount += Amounttoincrement;
                        if (incrementAmount > maximumPixelDeviation) {
                            exitLoop = true;
                        }
                    }
                }
                else {
                    exitLoop = true;
                }
            }
        }


        if (part === null) return null;
        while (part !== null && !part.canSelect()) part = part.containingGroup;
        if (part !== null && (part.canMove() || part.canCopy())) return part;
        return null;

        // call the default function
//        go.DraggingTool.prototype.findDraggablePart.call(this);
    }
    
    diagram.toolManager.mouseMoveTools.insertAt( 2,
        $$( DragCreatingTool,
            {
                isEnabled: false, // disabled by the checkbox
                delay: 0, // always canStart(), so PanningTool never gets the chance to run
                box: $$( go.Part,
                    { layerName: 'Tool' },
                    $$( go.Shape,
                        { name: 'SHAPE', fill: null, stroke: 'cyan', strokeWidth: FMDrawIndex.defaultShapeStrokeWidth } )
                ),
                archetypeNodeData: { color: 'white' }, // initial properties shared by all nodes
                doMouseMove: function()
                {
                	var point = diagram.lastInput.documentPoint;
						if (point)
							UpdateCoordinatePanel(point);
                	DragCreatingTool.prototype.doMouseMove.call(this);
                },
                insertPart: function( bounds )
                {
                    // override DragCreatingTool.insertPart
                	// Assign latest zOrder from the active layer
						  var copy = JSON.parse(JSON.stringify(FMDrawIndex.defaultArchetype)); //copying default node data to get current default properties
						  copy.category = this.archetypeNodeData.category;
						  if (this.archetypeNodeData.size)
						  	copy.size = this.archetypeNodeData.size;
						  if (this.archetypeNodeData.layerName)
						  	copy.layerName = this.archetypeNodeData.layerName;
						  if (this.archetypeNodeData.bgsize)
						  	copy.bgsize = this.archetypeNodeData.bgsize;
						  if (this.archetypeNodeData.TagFieldSelection)
						  	copy.TagFieldSelection = this.archetypeNodeData.TagFieldSelection;
						  if (this.archetypeNodeData.barType)
						  	copy.barType = this.archetypeNodeData.barType;
						  if (this.archetypeNodeData.demoPercent)
						  	copy.demoPercent = this.archetypeNodeData.demoPercent;
						 if (FMDrawIndex.defaultArchetype.alignment)
						 	copy.alignment = FMDrawIndex.defaultArchetype.alignment.copy();

						 if (FMDrawIndex.defaultArchetype.color)
						 {
						 	if ( typeof FMDrawIndex.defaultArchetype.color === 'object' )
							{
								 copy.color = FMDrawIndex.defaultArchetype.color.copy();
						 	}
						 	else
						 	{
						 		// It was decided in MVP Testing that transparency be defaulted to NO
						 		// transparency.
						 		copy.color = FMDrawIndex.RemoveTransparencyFromColor(FMDrawIndex.defaultArchetype.color);
						 		copy.transparency = "0";
						 	}
						 }

						 if (FMDrawIndex.defaultArchetype.lineStroke)
						 {
						 	if (typeof FMDrawIndex.defaultArchetype.lineStroke === 'object')
						 	{
						 		copy.lineStroke = FMDrawIndex.defaultArchetype.lineStroke.copy();
						 	}
						 	else
						 	{
						 		// It was decided in MVP Testing that transparency be defaulted to NO
						 		// transparency.
						 		copy.lineStroke = FMDrawIndex.RemoveTransparencyFromColor(FMDrawIndex.defaultArchetype.lineStroke);
						 		copy.lineStyleTransparency = "0";
						 	}
						 }

	                 this.archetypeNodeData = copy; 
                    var primaryLayerName = layerManager.GetPrimaryLayerName();
                    this.archetypeNodeData.layerName = primaryLayerName;
                    this.archetypeNodeData.zOrder = FMDrawIndex.GetNextPartZOrder( primaryLayerName );
                    // call the base method to do normal behavior and return its result
                    var part = DragCreatingTool.prototype.insertPart.call(this, bounds);
                    if (this.archetypeNodeData.category &&
                        this.archetypeNodeData.category === 'button') {
                        FMDrawPropertyMenu.OpenPropertiesPopupMenu( [ButtonActionTargetTextBoxID,ButtonActionTypeDropDownID] );
                    }
                    return part;
                }
            } ) );

    var dclt = $$(DisconnectedLinkingTool,
        {
            isEnabled: false,
            //delay: 0,
            //box: $$( go.Part, { layerName: 'Tool' },                $$( go.Shape, { name: 'SHAPE', fill: null, stroke: 'cyan', strokeWidth: FMDrawIndex.defaultLineStrokeWidth } ) ),
            //archetypeNodeData: { color: 'black' },
            //insertPart: function( points )
            //{
            //    // override DragCreatingLinkTool.insertPart
            //    // Assign latest zOrder from the active layer
            //    var primaryLayerName = layerManager.GetPrimaryLayerName();
            //    this.archetypeNodeData.layerName = primaryLayerName;
            //    this.archetypeNodeData.zOrder = FMDrawIndex.GetNextPartZOrder( primaryLayerName );
            //    // call the base method to do normal behavior and return its result
            //    return DisconnectedLinkingTool.prototype.insertPart.call(this, points);
        	//}

        } );
    diagram.toolManager.mouseMoveTools.insertAt( 3, dclt );

    //Add a diagram listener to react to a click to any background area.  This
    //will allow the users to click on the canvas and create a text block if they
    //are in text block mode.
    diagram.addDiagramListener( 'BackgroundSingleClicked', function( e )
    {
        if ( !e || !e.diagram )
        {
            return;
        }
        e.diagram.toolManager.textEditingTool.doCancel();
        FMDrawIndex.AddTextWithSingleClick( e );
    } );

    tool = diagram.toolManager.textEditingTool;
    tool.canStart = function()
    {
        if ( diagram.lastInput.control )
        {
            return;
        }
        else
        {
            go.TextEditingTool.prototype.canStart.call( tool );
        }
    }; //Saves a reference to the diagram that contained objects copied to the clipboard which have unsaved image data.
    diagram.addDiagramListener( 'ClipboardChanged', function( e )
    {
        if ( !e || !e.diagram )
        {
            return;
        }
        var unSavedPictureDataFound = false;
        e.subject.toArray().forEach( function( o )
        {
            if ( o instanceof go.Node &&
                o.data &&
                o.data.category &&
                o.data.category === 'picture' &&
                ( o.data.imageGuid == undefined || o.data.imageGuid === '' ) )
            {
                unSavedPictureDataFound = true;
                return false;
            }
            return true;
        } );
        if ( unSavedPictureDataFound )
        {
            FMDrawIndex.clipboardDiagram = e.diagram;
        }
        else
        {
            FMDrawIndex.clipboardDiagram = null;
        }
    } );

    this.UpdateImageSources = function( e, fromClipboard )
    {
        if ( !e || !e.diagram || !( e.subject instanceof go.Set ) )
        {
            return;
        }
        var diagram = e.diagram;
        var selection = [];
        var allNodes = [];

        //Build Array of Selected Objects that are Pictures with No Source
        e.subject.each( function( o )
        {
            if ( o instanceof go.Node &&
                o.data &&
                o.data.category &&
                o.data.category === 'picture' &&
                o.findObject( 'SHAPE' ) &&
                typeof o.findObject( 'SHAPE' ).source !== 'undefined' &&
                o.findObject( 'SHAPE' ).source !== null &&
                o.findObject( 'SHAPE' ).source.length === 0 )
            {
                selection.push( o );
            }
        } );

        //No need processing further if we don't have any pictures to copy to.
        if ( selection.count === 0 )
        {
            return;
        }

        if ( !fromClipboard )
        {
            //Build Array of nodes from all picture nodes that have the source set
            diagram.nodes.each( function( o )
            {
                if ( o instanceof go.Node &&
                    o.data &&
                    o.data.category &&
                    o.data.category === 'picture' &&
                    o.findObject( 'SHAPE' ) &&
                    typeof o.findObject( 'SHAPE' ).source !== 'undefined' &&
                    o.findObject( 'SHAPE' ).source !== null &&
                    o.findObject( 'SHAPE' ).source.length > 0 )
                {
                    allNodes.push( o );
                }
            } );
        }
        else
        {
            if ( FMDrawIndex.clipboardDiagram && FMDrawIndex.clipboardDiagram instanceof go.Diagram )
            {
                FMDrawIndex.clipboardDiagram.nodes.each( function( o )
                {
                    if ( o instanceof go.Node &&
                        o.data &&
                        o.data.category &&
                        o.data.category === 'picture' &&
                        o.findObject( 'SHAPE' ) &&
                        typeof o.findObject( 'SHAPE' ).source !== 'undefined' &&
                        o.findObject( 'SHAPE' ).source !== null &&
                        o.findObject( 'SHAPE' ).source.length > 0 )
                    {
                        allNodes.push( o );
                    }
                } );
            }
        }


        //No need processing further if we don't have any pictures to copy from.
        if ( allNodes.length === 0 )
        {
            return;
        }


        var currentImgSrc = '';
        var currentImageHash = '';
        selection.forEach( function( o )
        {
            if ( o.data && currentImageHash !== o.data.imageHash )
            {
                currentImageHash = o.data.imageHash;

                allNodes.forEach( function( part )
                {
                    if ( part.data.imageHash === currentImageHash )
                    {
                        currentImgSrc = part.findObject( 'SHAPE' ).source;
                        return false;
                    }
                    else
                    {
                        currentImgSrc = '';
                    }
                    return true;
                } );
                diagram.skipsUndoManager = true;
                o.findObject( 'SHAPE' ).source = currentImgSrc;
                diagram.skipsUndoManager = false;
            }
            else
            {
                diagram.skipsUndoManager = true;
                o.findObject( 'SHAPE' ).source = currentImgSrc;
                diagram.skipsUndoManager = false;
            }
        } );
    };

    

    //Add a diagram listener to react to a pasting of objects to diagram canvas.
    //This will ensure that the objects added get assigned proper layer Names and ZOrder indexes
    diagram.addDiagramListener('ClipboardChanged', function (e) {
        thisObj.UpdateImageSources(e, true);
        thisObj.AssignLayersForNewObjects();
    });
    
    //Add a diagram listener to react to a pasting of objects to diagram canvas.
    //This will ensure that the objects added get assigned proper layer Names and ZOrder indexes
    diagram.addDiagramListener( 'ClipboardPasted', function( e )
    {
        thisObj.UpdateImageSources( e, true );
        thisObj.AssignLayersForNewObjects();
        thisObj.RemoveIncompatibleTagObjects();
    } );

    diagram.addDiagramListener( 'SelectionCopied', function( e )
    {
        thisObj.UpdateImageSources( e, false );
        thisObj.AssignLayersForNewObjects();
    } );

    diagram.nodeTemplate = polygonTemplate;

    this.goJsDiagram = diagram;
    this.activeLayerIndex = 0;
    this.metaData = {};
    this.gridModified = false;

    //diagram.toolManager.findTool("GeometryReshaping").isEnabled = true;
    //FMdrawindex.PolygonMode(false, false, diagram);

    diagram.addDiagramListener( 'ChangedSelection', function( e )
    {
        if ( !thisObj.SelectionChangeBasedOnPasteFromDuplication )
        {
            thisObj.PerforCopyForDuplication = true;
        }
        thisObj.SelectionChangeBasedOnPasteFromDuplication = false;

        if ( !e || !e.diagram || !e.diagram.selection || e.diagram.selection.count === 0 )
        {
            return;
        }
        e.diagram.skipsUndoManager = true;
        e.diagram.selection.each( function( o )
        {
            if ( o && typeof o.isSelected === 'boolean' )
            {
                if ( o.isSelected )
                {
                    if ( o.data )
                    {
                    }
                }
            }
        } );
        e.diagram.skipsUndoManager = false;
    } );

    diagram.addDiagramListener( 'PartResized', function( e )
    {
        if ( !e.subject )
        {
            return;
        }
        var obj = e.subject;
        if ( obj &&
            obj.part &&
            obj.part.data &&
            obj.part.data.category === 'picture' &&
            obj.part.data.imageGuid &&
            obj.part.data.imageGuid !== '' )
        {
            model.setDataProperty( obj.part.data, 'source', FMDrawIndex.GeneratePictureURL( obj.part.data.imageGuid, ~~obj.desiredSize.width, ~~obj.desiredSize.height ) );
        }
    } );


    diagram.addDiagramListener( 'ChangingSelection', function( e )
    {
        //Defensive Coding to verify the parameter e
        if ( !e || !e.diagram || !e.diagram.selection || e.diagram.selection.count === 0 )
        {
            return;
        }
        e.diagram.skipsUndoManager = true;
        e.diagram.selection.each( function( o )
        {
            if ( o && typeof o.isSelected === 'boolean' )
            {
                if ( o.isSelected )
                {
                }
            }
        } );
        e.diagram.skipsUndoManager = false;
    } );

    diagram.addDiagramListener( 'SelectionMoved', function( e )
    {
        //Defensive Coding to verify the parameter e
        thisObj.PerforCopyForDuplication = true;
       
    });

    diagram.addDiagramListener('PartRotated', function (e) {
        //Defensive Coding to verify the parameter e
        thisObj.PerforCopyForDuplication = true;
       
    });

    diagram.model.addChangedListener( function( e )
    {
        if ( e.propertyName === 'FinishedUndo' || e.propertyName === 'FinishedRedo' || e.propertyName === 'CommittedTransaction' )
        {
            if ( !e.object )
            {
                return;
            }
            if ( e.object.name === 'Initial Layout' )
            {
                return;
            }

            FMDrawIndex.UpdateUndoButtons();
            //Update Property Window so that it reacts to a selected objects undo and redo
            if ( typeof FMDrawPropertyMenu === 'object' &&
                typeof FMDrawPropertyMenu.InitiatizePropertiesMenu === 'function' &&
                FMDrawPropertyMenu.PropertyActiveObject)
            {
                FMDrawPropertyMenu.InitiatizePropertiesMenu();
            }
}
    } );


    diagram.model.modelData.gridXCellSize = 10;
    diagram.model.modelData.gridYCellSize = 10;
    diagram.model.modelData.snapXCellSize = 10;
    diagram.model.modelData.snapYCellSize = 10;

    this.AssignLayersForNewObjects = function()
    {
        // ReSharper disable once InconsistentNaming
        var layerManager = new FMDrawIndex._LayerManager();
        var primaryLayerName = layerManager.GetPrimaryLayerName();

        if ( !primaryLayerName || !diagram.selection || diagram.selection.count === 0 )
        {
            return;
        }

        var nextZOrder;
        diagram.startTransaction( 'AssignToNewLayer' );
        var partsArray = FMDrawIndex.OrderPartsListByLayerAndZOrder(diagram.selection, true);
        for ( var i = 0; i < partsArray.length; i++ )
        {
            var targetLayerName = partsArray[i].LayerName;
            if ( !diagram.findLayer( targetLayerName ) )
            {
                targetLayerName = primaryLayerName;
            }
            nextZOrder = FMDrawIndex.GetNextPartZOrder( targetLayerName );
            model.setDataProperty( partsArray[i].PartData, 'layerName', targetLayerName );
            model.setDataProperty( partsArray[i].PartData, 'zOrder', nextZOrder );
        }
        diagram.commitTransaction( 'AssignToNewLayer' );
    };

    this.RemoveIncompatibleTagObjects = function()
    {
        var panelTypePropertyName = 'PanelType';
        var pointTemplateGuidPropertyName = 'PointTemplateGuid';
        if ( diagram && diagram.model && diagram.model.modelData && panelTypePropertyName in diagram.model.modelData )
        {
            var isStandardPanel = diagram.model.modelData[panelTypePropertyName] === 'Standard';

            var pointTemplateGuid = ( pointTemplateGuidPropertyName in diagram.model.modelData ) ? diagram.model.modelData[pointTemplateGuidPropertyName] : '';

            var coll = [];
            diagram.nodes.each( function( node )
            {
                if ( node && node.data )
                {
                    //If the panel is standard and the object is a PointTemplate Tag Selector then add to collection to be removed.
                    if ( isStandardPanel )
                    {
                        if ( node.data.PointTemplateTagSelectionIndicator )
                        {
                            coll.push( node );
                        }
                    }
                    else
                    {
                        if ( node.data.PointTemplateTagSelectionIndicator && node.data.PointGUID !== pointTemplateGuid )
                        {
                            coll.push( node );
                        }
                    }
                }
            } );
            diagram.removeParts( coll, true );
        };
    }
    diagram.initialPosition = new go.Point(0, 0);
};
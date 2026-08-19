FMDrawIndex = FMDrawIndex || {};

if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	debugger;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

FMDrawIndex.PolygonMode = function( draw, polygon, diagram )
{
	diagram.nodeTemplate = polygonTemplate;
	diagram.toolManager.findTool( 'GeometryReshaping' ).isEnabled = true;
	var tool = diagram.toolManager.findTool( polygonDrawingToolName );
	// ReSharper disable once InconsistentNaming
	var layerManager = new FMDrawIndex._LayerManager();
	tool.archetypeNodeData = {
		category: 'polygon',
		stroke: FMDrawIndex.shapeStrokeColor,
		color: FMDrawIndex.shapeFillColor,
		layerName: layerManager.GetPrimaryLayerName()
	};

	tool.isEnabled = draw;
	tool.isPolygon = polygon;
	//tool.archetypePartData.fill = (polygon ? FMdrawindex.shapeFillColor : null);
	tool.archetypePartData.category = 'polygon';
	tool.archetypePartData.layerName = layerManager.GetPrimaryLayerName();
	tool.archetypePartData.color = ( polygon ? FMDrawIndex.shapeFillColor : null );
	tool.archetypePartData.stroke = FMDrawIndex.defaultArchetype.lineStroke;
	tool.archetypePartData.strokeWidth = ( polygon ? FMDrawIndex.defaultShapeStrokeWidth : FMDrawIndex.defaultLineStrokeWidth );
	tool.temporaryShape.fill = ( polygon ? FMDrawIndex.shapeFillColor : null );
	tool.temporaryShape.stroke = FMDrawIndex.defaultArchetype.lineStroke;
	tool.temporaryShape.strokeWidth = ( polygon ? FMDrawIndex.defaultShapeStrokeWidth : FMDrawIndex.defaultLineStrokeWidth );
};
FMDrawIndex.LineMode = function( draw, diagram )
{
	//diagram.nodeTemplate = lineTemplate; //holdover from links, not necessary any longer
	diagram.toolManager.findTool( 'GeometryReshaping' ).isEnabled = true;
	var tool = diagram.toolManager.findTool( lineToolName );
	// ReSharper disable once InconsistentNaming
	var layerManager = new FMDrawIndex._LayerManager();
	tool.archetypeNodeData = {
		category: 'line',
		stroke: FMDrawIndex.defaultArchetype.lineStroke,
		color: FMDrawIndex.shapeFillColor,
		layerName: layerManager.GetPrimaryLayerName()
	};

	tool.isEnabled = draw;
	//tool.archetypePartData.fill = (polygon ? FMdrawindex.shapeFillColor : null);
	tool.archetypePartData.category = 'line';
	tool.archetypePartData.layerName = layerManager.GetPrimaryLayerName();
	tool.archetypePartData.color = null;
	tool.archetypePartData.stroke = FMDrawIndex.defaultArchetype.lineStroke;
	tool.archetypePartData.strokeWidth = FMDrawIndex.defaultLineStrokeWidth;
	tool.temporaryShape.fill = null;
	tool.temporaryShape.stroke = FMDrawIndex.defaultArchetype.lineStroke;
	tool.temporaryShape.strokeWidth = FMDrawIndex.defaultLineStrokeWidth;
};
FMDrawIndex.InitEventListeners = function()
{
};
FMDrawIndex.InitContextMenu = function( canvas )
{
	// ReSharper disable once UnusedLocals
	//var diagram = FMdrawindex.GetActiveTabGoJSDiagramObject();
	FMDrawIndex.currentContextMenu = canvas.contextPopup( {
		items: [
				{
					label: 'Delete',
					icon: '../../FMWebApp/images/page_white_delete.png',
					action: function()
					{
						FMDrawIndex.Delete();
					},
					isEnabled: FMDrawIndex.AreObjectsSelected
				}, {
					label: 'Cut',
					icon: '../../FMWebApp/images/cut.png',
					action: function()
					{
						FMDrawIndex.Cut();
					},
					isEnabled: FMDrawIndex.AreObjectsSelected
				}, {
					label: 'Copy',
					icon: '../../FMWebApp/images/page_white_copy.png',
					action: function()
					{
						FMDrawIndex.Copy();
					},
					isEnabled: FMDrawIndex.AreObjectsSelected
				}, {
					label: 'Paste',
					icon: '../../FMWebApp/images/page_white_paste.png',
					action: function()
					{
						FMDrawIndex.Paste();
					},
					isEnabled: FMDrawIndex.AreObjectsOnClipboard
				}, 
				null, // divider
				{
					label: 'Bring to Front',
					icon: '../../FMWebApp/images/BringToFrontIcon.png',
					action: FMDrawIndex.BringToFront,
					isEnabled: FMDrawIndex.AreObjectsSelected
				},
				{
					label: 'Send to Back',
					icon: '../../FMWebApp/images/SendToBackIcon.png',
					action: FMDrawIndex.SendToBack,
					isEnabled: FMDrawIndex.AreObjectsSelected
				},
				{
					label: 'Bring Forward',
					icon: '../../FMWebApp/images/BringForwardIcon.png',
					action: FMDrawIndex.BringForward,
					isEnabled: FMDrawIndex.AreObjectsSelected
				}, {
					label: 'Send Backward',
					icon: '../../FMWebApp/images/SendBackwardIcon.png',
					action: FMDrawIndex.SendBackward,
					isEnabled: FMDrawIndex.AreObjectsSelected
				},
				null, // divider
				{
					label: 'Layers',
					icon: '../../FMWebApp/images/LayersContextMenu.png',
					action: FMDrawIndex.OpenLayersDialog,
					isEnabled: FMDrawIndex.LayersDialogEnabled
				},
				{
					label: 'Pan On',
					icon: '../../FMWebApp/images/PanOnContextMenu.png',
					action: FMDrawIndex.TogglePanning,
					isEnabled: FMDrawIndex.IsPanningOff
				},
				{
					label: 'Pan Off',
					icon: '../../FMWebApp/images/PanOffContextMenu.png',
					action: FMDrawIndex.TogglePanning,
					isEnabled: FMDrawIndex.IsPanningOn
				},
				{
					label: 'Properties',
					icon: '../../FMWebApp/images/PropertiesContextMenu.png',
					action: FMDrawPropertyMenu.OpenPropertiesPopupMenu,
					isEnabled: FMDrawIndex.AreObjectsSelected
				},
				{
					label: 'Group',
					icon: '../../FMWebApp/images/GroupContextMenu.png',
					action: FMDrawIndex.Group,
					isEnabled: FMDrawIndex.GroupEnabled
				},
   		        {
   		        	label: 'Ungroup',
   		        	icon: '../../FMWebApp/images/UngroupContextMenu.png',
                    action: FMDrawIndex.Ungroup,
                    isEnabled: FMDrawIndex.UngroupEnabled,
   		        },
                null, // divider
                {
                	label: 'Animation Manager',
                	icon: '../../FMWebApp/images/AnimationsContextMnu.png',
                    action: FMDrawIndex.OpenAnimationManager,
                    isEnabled: FMDrawIndex.AnimationEnabled
                },
]
	}, true );
};
FMDrawIndex.GroupEnabled = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram || !diagram.selection )
	{
		return false;
	}

	return diagram.selection.count > 1;
};
FMDrawIndex.UngroupEnabled = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram || !diagram.selection || diagram.selection.count === 0 )
	{
		return false;
	}

	var returnvalue = false;
	diagram.selection.each( function( obj )
	{
		if ( obj && obj.data && typeof obj.data.isGroup === 'boolean' && obj.data.isGroup === true )
		{
				returnvalue = true;
				return false;
		}
		return true;
	} );
	return returnvalue;
};
FMDrawIndex.Group = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	if ( diagram.selection.count < 2 )
	{
		return;
	}
	diagram.startTransaction( 'Group Objects' );
	var groupObj = { isGroup: true };

	var model = diagram.model;

	model.addNodeData( groupObj );

	var items = [];

	var group = diagram.findPartForData( groupObj );
	if ( group )
	{
		diagram.selection.each( function( obj )
		{
				obj.containingGroup = group;
				items.push( obj );
				model.setDataProperty( obj.data, 'selectable', false );
				model.setDataProperty( obj.data, 'reshapable', false );
		} );
	}
	items.forEach( function( o )
	{
		o.isSelected = false;
	} );
	group.isSelected = true;
	diagram.commitTransaction( 'Group Objects' );
};
FMDrawIndex.Ungroup = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram || !diagram.selection || diagram.selection.count === 0 )
	{
		return;
	}

	var items = [];
	var groups = [];
	var model = diagram.model;
	diagram.startTransaction( 'Ungroup Objects' );
	diagram.selection.each( function( group )
	{
		if ( typeof group.data.isGroup === 'boolean' && group.data.isGroup === true )
		{
				groups.push( group );
				group.memberParts.each( function( o )
				{
					items.push( o );
				} );
		}
		return true;
	} );
	//Disassociate Objects from Groups
	items.forEach( function( o )
	{
		o.containingGroup = null;
		model.setDataProperty( o.data, 'selectable', true );
		model.setDataProperty( o.data, 'reshapable', true );
		o.isSelected = true;
	} );
	//Remove Groups
	groups.forEach( function( group )
	{
		diagram.remove( group );
	} );
	diagram.commitTransaction( 'Ungroup Objects' );
};

FMDrawIndex.OpenAnimationManager = function()
{
    FMDrawPropertyMenu.InvokeAnimationButtonAction();
};

FMDrawIndex.AnimationEnabled = function () {
    return FMDrawPropertyMenu.IsAnimationManagerEnabled();
};


FMDrawIndex.GetActiveTabDOMElement = function()
{
	var index = 0;
	var domElement = null;
	$( '.drawingTab' ).each( function()
	{
		if ( index === FMDrawIndex.activeTabIndex )
		{
				domElement = $( this );
				return false;
		}
		else
		{
				index++;
		}
		return true;
	} );
	return domElement;
};
FMDrawIndex.GetActiveTabName = function()
{
	var dom = FMDrawIndex.GetActiveTabDOMElement();
	if ( dom === null || typeof dom === 'undefined' )
	{
		return '';
	}
	var el = dom.find( 'a:first' );
	if ( el === null || typeof el === 'undefined' )
	{
		return '';
	}
	return el.html();
};
FMDrawIndex.GetTabDOMElement = function( tabNumber )
{
	var index = 0;
	var domElement = null;
	$( '.drawingTab' ).each( function()
	{
		if ( index === tabNumber )
		{
				domElement = $( this );
				return false;
		}
		else
		{
				index++;
		}
		return true;
	} );
	return domElement;
};
FMDrawIndex.GetTabName = function( tabNumber )
{
	var tabname = '#tab' + tabNumber;
	var index = $( '#tabs a[href=\'' + tabname + '\']' ).parent().index();
	var name = $( 'div#tabs li:eq(' + index + ') a:first' ).html();
	return name;
};
FMDrawIndex.GetActiveTabCanvasContainerObject = function()
{
	if ( !FMDrawIndex.tabCanvasContainerCollection || !FMDrawIndex.tabCanvasContainerCollection[FMDrawIndex.activeTabCanvasContainerIndex] )
	{
		return null;
	}
	return FMDrawIndex.tabCanvasContainerCollection[FMDrawIndex.activeTabCanvasContainerIndex];
};
FMDrawIndex.GetActiveTabGoJSDiagramObject = function()
{
	if ( !FMDrawIndex.tabCanvasContainerCollection || !FMDrawIndex.tabCanvasContainerCollection[FMDrawIndex.activeTabCanvasContainerIndex] )
	{
		return null;
	}
	return FMDrawIndex.tabCanvasContainerCollection[FMDrawIndex.activeTabCanvasContainerIndex].goJsDiagram;
};
FMDrawIndex.GetSelectedObjects = function( canvas )
{
	if ( !canvas )
	{
		return [];
	}

	return canvas.selection.toArray();
};
FMDrawIndex.disableCheck = function()
{
	FMDrawIndex.checkChangesEnabled = false;
	setTimeout( enableCheck(), 100 );
};
FMDrawIndex.enableCheck = function()
{
	FMDrawIndex.checkChangesEnabled = true;
}; //Tab and Canvas Management Functions
FMDrawIndex.StoreMetadata = function( tabnumber, data )
{
	FMDrawIndex.tabCanvasContainerCollection[tabnumber].metaData = data;
};
FMDrawIndex.RenderDrawing = function( result, isDraw )
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();

	var drawingData = JSON.parse( result );
	//Add Layer Data
	var layerManager = new FMDrawIndex._LayerManager();
	if ( drawingData.layers )
	{
		var hasInvisibleLayers = false;
		drawingData.layers.forEach( function( o )
		{
				if ( layerManager.IsValidLayerParameters( o.layerName, o.layerDisplayName ) )
				{
					layerManager.AddLayer(o.layerName, o.layerDisplayName, o.visible, o.active);
					if (o.visible == false)
						hasInvisibleLayers = true;
				}
		});
		if (hasInvisibleLayers && isDraw) {
			FMErrorAndExceptionHandling.ShowNotification("There are invisible layers in this drawing.");
		}
		layerManager.SetPrimaryLayer( drawingData.primaryLayerName );
	}

	diagram.model = go.Model.fromJson( drawingData.model );

	var canvasCollectionObject = FMDrawIndex.GetActiveTabCanvasContainerObject();
	canvasCollectionObject.SetModel( diagram.model );

	if ( diagram.model.modelData )
	{
		if ( diagram.model.modelData.position )
		{
				diagram.initialPosition = go.Point.parse( diagram.model.modelData.position );
				//Fixes an issue in IE where the position of the diagram does
				//not always honor the intialPosition loaded from the model in the database.
				//Need to submit a issue to GoJS folkes about this.
				diagram.position = go.Point.parse( diagram.model.modelData.position );
		}
		else
		{
				diagram.initialViewportSpot = go.Spot.Center;
		}

		if ( diagram.model.modelData.scale )
		{
				diagram.initialScale = diagram.model.modelData.scale;
		}

		if ( diagram.model.modelData.gridEnabled )
		{
				diagram.grid.visible = diagram.model.modelData.gridEnabled;
				$( '#gridcheckbox' ).prop( 'checked', diagram.model.modelData.gridEnabled );
		}

		if ( diagram.model.modelData.snapEnabled )
		{
				diagram.toolManager.draggingTool.isGridSnapEnabled = diagram.model.modelData.snapEnabled;
				$( '#snapcheckbox' ).prop( 'checked', diagram.model.modelData.snapEnabled );
		}

		if ( diagram.model.modelData.gridXCellSize && diagram.model.modelData.gridYCellSize )
		{
				diagram.grid.gridCellSize = new go.Size( diagram.model.modelData.gridXCellSize, diagram.model.modelData.gridYCellSize );
				$( '#Xspacingtextbox' ).val( diagram.model.modelData.gridXCellSize );
				$( '#Yspacingtextbox' ).val( diagram.model.modelData.gridYCellSize );
		}

		if ( diagram.model.modelData.gridInterval )
		{
				diagram.grid.elt( 2 ).interval = diagram.model.modelData.gridInterval;
				diagram.grid.elt( 3 ).interval = diagram.model.modelData.gridInterval;
				$( '#GridlineInterval' ).val( diagram.model.modelData.gridInterval );
		}

		if ( diagram.model.modelData.snapXCellSize && diagram.model.modelData.snapYCellSize )
		{
		    diagram.toolManager.draggingTool.gridSnapCellSize = new go.Size(diagram.model.modelData.snapXCellSize, diagram.model.modelData.snapYCellSize);
			$( '#snapXspacingtextbox' ).val( ( diagram.model.modelData.snapXCellSize ) );
			$( '#snapYspacingtextbox' ).val( ( diagram.model.modelData.snapYCellSize ) );
		}

		if (diagram.model.modelData.defaultArchetype)
		{
			Object.keys( diagram.model.modelData.defaultArchetype ).forEach( function( key )
				{
					FMDrawIndex.defaultArchetype[key] = diagram.model.modelData.defaultArchetype[key];
				}
			);
		}
		if (isDraw)
			FMDrawIndex.RefreshPreview();
	}

	//Ensure that all text boxes for objects are disable if layer is inActive
	var layers = layerManager.GetLayers();
	var i = 0;
	var parts = [];
	for ( i = 0; i < layers.length; i++ )
	{
		// ReSharper disable once ClosureOnModifiedVariable
		layers[i].parts.each( function( part )
		{
				parts.push( part );
		} );
		parts.forEach( function( o )
		{
				if ( o && o.textEditable !== undefined )
				{
					// ReSharper disable once ClosureOnModifiedVariable
					o.textEditable = layers[i].allowSelect;
				}
		} );
		parts = [];
	}

	// Call the Property Menu to set up the events and initial
	// the drawing objects with the correct property settings.
	FMDrawPropertyMenu.SetPropertyMenuEvents( diagram );
	FMDrawPropertyMenu.InitializeObjectWithPattern( diagram );

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
		}
	} );
	diagram.requestUpdate();
	if ( isDraw )
	{
		FMDrawIndex.InitializeDrawing( diagram );
	}
};
FMDrawIndex.InitializeDrawing = function( diagram )
{
	if ( !diagram )
	{
		return;
	}
	diagram.nodes.each( function( node )
	{
		this.selectable = false;
		if ( node.name === 'Tag' )
		{
				FMDrawIndex.UpdateTagFormat( diagram.model, node.data );
		}
	} );
};
FMDrawIndex.AttatchGoJSDiagramToNewCanvas = function( result )
{
	var currentcanvas = canvasIDDefaultPrefix + result.toString();
	var canvasIndex = parseInt( result );
	FMDrawIndex.tabCanvasContainerCollection[canvasIndex] = new FMDrawIndex._CanvasCollectionObject( currentcanvas );
	var diagram = FMDrawIndex.tabCanvasContainerCollection[canvasIndex].goJsDiagram;
	//$( '#' + currentcanvas ).focusout( FMdrawindex.DeactivateTextEditingTool );
	return diagram;
};
FMDrawIndex.UpdateactiveCanvas = function( canvasContainerCollectionIndex )
{
	var canvasContainerClassName = canvasContainerString + canvasContainerCollectionIndex.toString();
	FMDrawIndex.activeTabCanvasContainerIndex = canvasContainerCollectionIndex;

	if ( document.getElementsByClassName( canvasContainerString ).length === 1 )
	{
		document.getElementsByClassName( canvasContainerString )[0].setAttribute( 'class', canvasContainerClassName );
	}
}; //================================================================================================
// This function handles the Tab switching.  It will also reset the Property Window to the 
// canvas and properties.
//================================================================================================
FMDrawIndex.OnTabSwitch = function( activeTabIndex, canvasContainerCollectionIndex )
{
	FMDrawIndex.activeTabIndex = activeTabIndex;
	FMDrawIndex.activeTabCanvasContainerIndex = canvasContainerCollectionIndex;
	FMDrawIndex.PersistActiveTool( FMDrawIndex.currentDrawControl );
	FMDrawIndex.UpdateGridDialog();

	// Call property window to update.
	FMDrawPropertyMenu.SetPropertyMenuEvents(FMDrawIndex.GetActiveTabGoJSDiagramObject());
	FMDrawPropertyMenu.ClearPropertyWindow();
	FMDrawPropertyMenu.InitiatizePropertiesMenu();

	FMDrawIndex.InitializeZoomLevel();
	FMDrawIndex.UpdateMode();
	if ($('#layers-dialog').dialog('isOpen')) {
		FMDrawIndex.OpenLayersDialog();
	}
};
FMDrawIndex.ClearTabMemory = function( tabIndex )
{
	//Clear Memory in canvasCollection array item pointed to by tabIndex
	if ( FMDrawIndex.tabCanvasContainerCollection && tabIndex >= 0 && tabIndex <= FMDrawIndex.tabCanvasContainerCollection.length - 1 )
	{
		FMDrawIndex.tabCanvasContainerCollection[tabIndex] = null;
	}
};
FMDrawIndex.InitSaveDialog = function()
{
	$( '#overwrite-dialog' ).dialog( {
		autoOpen: false,
		height: 200,
		width: 550,
		dialogClass: 'dialog-title-icon-info',
		modal: true,
		open: FMDrawIndex.DeactivateTextEditingTool, //Ensures that GoJS Text Controls don't retain focus and cause broswer to lockup
		resizable: false,
		create: function( event, ui )
		{
				// add the keyboard shortcut since jquery ui dialog does not support it out of the box
				$( this ).parent().find( 'button' ).each( function()
				{
					if ( $( this ).find( '.ui-button-text' ).text() === 'Yes' )
					{
						$( this ).find( '.ui-button-text' ).html( '<u>Y</u>es' );
						$( this ).attr( 'accesskey', 'y' );
					}
					if ( $( this ).find( '.ui-button-text' ).text() === 'No' )
					{
						$( this ).find( '.ui-button-text' ).html( '<u>N</u>o' );
						$( this ).attr( 'accesskey', 'n' );
					}
				} );
		},
		buttons: [
				{
					text: 'Yes',
					click: function()
					{
						var tabindex = $( 'div#tabs' ).tabs( 'option', 'active' );
						$( this ).dialog( 'close' );
						FMDrawIndex.confirmSave = true;
						FMDrawIndex.SaveDrawing( $( '#name' ).val(), $( '#desc' ).val() );
						$( 'div#tabs li:eq(' + tabindex + ') a:first' ).text( $( '#name' ).val() );
						$( '#name' ).val( '' );
						$( '#desc' ).val( '' );

						if ( FMDraw.closeAfterSaving )
						{
								FMDraw.closetab();
								FMDraw.closeAfterSaving = false;
						}
					}
				},
				{
					text: 'No',
					click: function()
					{
						$( this ).dialog( 'close' );
					}
				}
		]
	});

	$( '#save-dialog' ).dialog( {
		autoOpen: false,
		height: 300,
		width: 350,
		modal: true,
		open: FMDrawIndex.DeactivateTextEditingTool, //Ensures that GoJS Text Controls don't retain focus and cause broswer to lockup
		resizable: false,
		dialogClass: 'no-background-image', // do not show the standard background image
		create: function( event, ui )
		{
				// add the keyboard shortcut since jquery ui dialog does not support it out of the box
				$( this ).parent().find( 'button' ).each( function()
				{
					if ( $( this ).find( '.ui-button-text' ).text() === 'Save' )
					{
						$( this ).find( '.ui-button-text' ).html( '<u>S</u>ave' );
						$( this ).attr( 'accesskey', 's' );
					}
					if ( $( this ).find( '.ui-button-text' ).text() === 'Cancel' )
					{
						$( this ).find( '.ui-button-text' ).html( '<u>C</u>ancel' );
						$( this ).attr( 'accesskey', 'c' );
					}
				} );
		},
		buttons: {
				'Save': function()
				{
					$( this ).dialog( 'close' );
					var tabindex = $( 'div#tabs' ).tabs( 'option', 'active' );
					if ( $( '#name' ).val().toUpperCase() !== $( 'div#tabs li:eq(' + tabindex + ') a:first' ).html().toUpperCase() )
					{
						FMDrawIndex.CheckTabNames();
					}
					if ( FMDrawIndex.confirmSave === true )
					{
						FMDrawIndex.FetchDrawingNames();
					}
					if ( FMDrawIndex.confirmSave === true )
                    {
                        if (FMDraw.exportAfterSaving) {
                            FMDraw.exportAfterSaving = false;
                            FMDrawIndex.SaveDrawing($('#name').val(), $('#desc').val(), function (drawingGuid) {
                                var href = $('#exportLink').attr('href');
                                if (href.indexOf('?') != -1) {
                                    href = href.substring(0, href.indexOf('?'));
                                }
                                $('#exportLink').attr('href', href + '?id=' + drawingGuid);
                                $('#exportLink')[0].click(); // click on a hidden link to trigger the export (we cannot use AJAX for security reasons)			
                            });

                        }
                        else {
                            FMDrawIndex.SaveDrawing($('#name').val(), $('#desc').val());
                        }
						$( 'div#tabs li:eq(' + tabindex + ') a:first' ).text( $( '#name' ).val() );
						$( '#name' ).val( '' );
                        $( '#desc' ).val( '' );
						if ( FMDraw.closeAfterSaving )
						{
								FMDraw.closetab();
								FMDraw.closeAfterSaving = false;
						}
					}
					else
					{
						FMDrawIndex.confirmSave = true;
					}
				},
				'Cancel': function()
				{
					$( '#name' ).val( '' );
					$( '#desc' ).val( '' );
					$( this ).dialog( 'close' );
				}
		}
	} );
};

FMDrawIndex.CheckTabNames = function()
{
	var name;
	var activeTabs = FMDrawIndex.ReturnCanvasIndices();
	for ( var index = 0; index < activeTabs.length; ++index )
	{
		name = $( 'div#tabs li:eq(' + index + ') a:first' ).html();
		if ( name.toUpperCase() === $( '#name' ).val().toUpperCase() )
		{
				$( 'save-dialog' ).dialog( 'close' );
				$( '#alreadyopen-dialog' ).dialog( 'open' );
				$( '#alreadyopen-dialog' ).html( name + ' is already open. Please close the drawing first, or choose a different name.' );
				FMDrawIndex.confirmSave = false;
		}
	}
};
FMDrawIndex.FetchDrawingNames = function()
{
	var token = $( 'input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	$.ajax( {
		type: 'get',
		dataType: 'json',
		cache: false,
		headers: headers,
		url: 'GetDrawingNames',
		async: false,
		success: function( response )
		{
				FMErrorAndExceptionHandling.HandleMessages( response, function( drawings, inError )
				{
					if ( inError )
					{
						FMDrawIndex.confirmSave = false; // do not save if we have a problem getting the drawing names
						return;
					}
					FMDrawIndex.CompareNames( drawings );
				} );
		},

		error: function( e )
		{
				FMErrorAndExceptionHandling.ShowError( 'Error loading drawing names: ' + e.responseText );
		}
	} );
};
FMDrawIndex.CompareNames = function( result )
{
	var name = $( '#name' ).val();
	for ( var index = 0; index < result.length; ++index )
	{
		if ( name.toUpperCase() === result[index].ID.toUpperCase() )
		{
				//FMdrawindex.confirmSave = window.confirm(result[index].ID + " already exists. Do you want to save over it?");
				$( '#overwrite-dialog' ).html( result[index].ID + ' already exists. Do you want to save over it?' );
				FMDrawIndex.confirmSave = false;
				$( '#overwrite-dialog' ).dialog( 'open' );
		}
	}
};
FMDrawIndex.UpdatePositionAndLocationData = function( diagram )
{
	if ( !diagram )
	{
		return;
	}
	//Update any parts that are not nodes or links (Not 
	var parts = diagram.parts;
	if ( parts && parts.count > 0 )
	{
		parts.reset();
		var part = parts.first();
		if ( part )
		{
				if ( part.data )
				{
					part.data.loc = go.Point.stringify( part.location );
					part.data.pos = go.Point.stringify( part.position );
				}
		}
		while ( parts.next() )
		{
				part = parts.value;
				if ( part && part.data )
				{
					part.data.loc = go.Point.stringify( part.location );
					part.data.pos = go.Point.stringify( part.position );
				}
		}
	}

	//Update Nodes
	var nodes = diagram.nodes;
	if ( nodes && nodes.count > 0 )
	{
		nodes.reset();
		var node = nodes.first();
		if ( node )
		{
				if ( node.data )
				{
					node.data.loc = go.Point.stringify( node.location );
					node.data.pos = go.Point.stringify( node.position );
				}
		}
		while ( nodes.next() )
		{
				node = nodes.value;
				if ( node && node.data )
				{
					node.data.loc = go.Point.stringify( node.location );
					node.data.pos = go.Point.stringify( node.position );
				}
		}
	}
};

FMDrawIndex.SaveDrawing = function( name, desc, callback )
{

	$( '#saving-drawing-dialog' ).dialog( 'open' );
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();

	//Disregard any changes made to the data model or graphical objects becase we are saving.
	diagram.skipsUndoManager = true;

	//We need to deactivate any tools to make sure they all objects data is locked in before we save.  
	//This ensures that if any objects are selected by the SelectionTool, they are deselected and returend to their original layers and zOrder before saving.
	if ( diagram )
	{
		diagram.commandHandler.stopCommand();
	}
	//Attempt to save images first before saving drawing

	FMDrawIndex.UpdatePositionAndLocationData( diagram );

	//Update data model with position and location information for all objects to ensure they are saved to data model

	FMDrawIndex.SaveDefaultstoModelData();

	if ( FMDrawIndex.SaveDrawingImages() )
	{
		var data = {};
		data.name = name;
		data.description = desc;
		data.image = FMDrawIndex.GetCurrentImage();
		FMDrawIndex.StoreMetadata( FMDrawIndex.activeTabCanvasContainerIndex, name + ',' + desc );


		//Optimize Drawing Data String By Removing Image Data and Replacing with Reference call to DisplayImage.ashx
		var drawingData = FMDrawIndex.OptimizeDrawingData( data.image );
		// Send drawing objects to controller
		$.ajax( {
				type: 'POST',
				async: false,
				processdata: false,
				dataType: 'json',
				url: 'SaveDrawing',
				data: {
					name: data.name,
					description: data.description,
					image: drawingData,
					panelTypeString: FMDrawIndex.GetDiagramModelDataValue('PanelType'),
					pointTemplateGuidString: FMDrawIndex.GetDiagramModelDataValue('PointTemplateGuid'),
					published: (FMDrawIndex.GetDiagramModelDataValue('Published') != null) ? FMDrawIndex.GetDiagramModelDataValue('Published') : 'true',
					animationGuidList: FMDrawAnimation.GetAllAnimationGuids(diagram),
               '__RequestVerificationToken': $( 'input[name=__RequestVerificationToken]' ).val()
				},
				success: function( response )
				{
					$( '#saving-drawing-dialog' ).dialog( 'close' );
					FMErrorAndExceptionHandling.CloseNotifications();
					FMErrorAndExceptionHandling.HandleMessages( response, function( drawings, inError )
					{
						if ( inError )
						{
								return;
						}
						//Ensure that diagram is no longer marked as modified
						FMDrawIndex.GetActiveTabGoJSDiagramObject().isModified = false;
						var canvasTabObject = FMDrawIndex.GetActiveTabCanvasContainerObject();
						canvasTabObject.gridModified = false;
						
						FMDrawIndex.UpdateTabDrawingGuid(drawings || '');

						if (callback) {
							callback(drawings);
						}
					} );
				},
				error: function( e )
				{
					$( '#saving-drawing-dialog' ).dialog( 'close' );
					FMErrorAndExceptionHandling.CloseNotifications();
					FMErrorAndExceptionHandling.ShowError( 'Error saving drawing.  Drawing not saved -- ' + e.responseText );
				}
		} );
	}
	//Re-enable UndoManager
	diagram.skipsUndoManager = false;
	$( '#saving-drawing-dialog' ).dialog( 'close' );
};
FMDrawIndex.OptimizeDrawingData = function( str )
{
	var data = JSON.parse( str );
	var model = JSON.parse( data.model );
	model.nodeDataArray.forEach( function( o )
	{
		if ( o.category === 'picture' && !o.source )
		{
				var size = go.Size.parse( o.size );
				o.source = FMDrawIndex.GeneratePictureURL( o.imageGuid, size.width, size.height );
		}
	} );
	data.model = model;

	return JSON.stringify( data );
};
FMDrawIndex.SaveDrawingImages = function()
{
	var returnValue = true;
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();

	var token = $( 'input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	diagram.nodes.each( function( node )
	{
		if ( node.data.category === 'picture' && node.data.imageGuid === '' )
		{
				// ReSharper disable once UnusedLocals
				//var existingImageGuid = '';
				var pic = node.findObject( 'SHAPE' );
				$.ajax( {
					//AJAX CALL TO CHECK FOR HASH EXISTENCE
					type: 'GET',
					dataType: 'json',
					cache: false,
					async: false,
					headers: headers,
					url: 'ImageHashExists',
					data: 'imageHash=' + node.data.imageHash,
					success: function( response )
					{ //SUCCESS: AJAX CALL TO CHECK FOR HASH EXISTENCE

						FMErrorAndExceptionHandling.HandleMessages( response, function( existingImageGuid, inError )
						{
								if ( !inError )
								{
									// if we found the image in the database
									if ( existingImageGuid === '' )
									{
										$.ajax( {
												//AJAX CALL TO SAVE IMAGE
												type: 'POST',
												async: false,
												processdata: false,
												dataType: 'json',
												url: 'SaveImage',
												data: {
													name: node.data.fileName,
													type: node.data.imageType,
													imageString: pic.source,
													'__RequestVerificationToken': $( 'input[name=__RequestVerificationToken]' ).val()
												},
												success: function( response )
												{ //SUCCESS: AJAX CALL TO SAVE IMAGE
													FMErrorAndExceptionHandling.HandleMessages( response, function( resultOfImageSave, inError )
													{
														if ( resultOfImageSave instanceof Array && resultOfImageSave.length >= 2 )
														{
																var newimageGuid = resultOfImageSave[0];
																var imageHash = resultOfImageSave[1];
																//Disregard any changes made to the data model or graphical objects becase we are saving.
																diagram.skipsUndoManager = true;
                                                                diagram.model.setDataProperty(node.data, 'imageGuid', newimageGuid); 
                                                                var size = go.Size.parse(node.data.size);
                                                                diagram.model.setDataProperty(node.data, 'source', FMDrawIndex.GeneratePictureURL(newimageGuid, size.width, size.height));

																//Update any other image controls who have the same hash value but does not have the image guid set.  This will 
																//deal with any unncessary saves of image controls that were cut and pasted from the image control in question.
																//Compare the imageHash based on the calculation from the server (results[2]) to ensure that the server and client 
																//algorithms are in synch.  The setting of the ImageStream property ensures that the imageHash of the picture is 
																//set internally.
																diagram.nodes.each( function( nodeAfterSave )
																{
																	if ( nodeAfterSave.data.category === 'picture' && nodeAfterSave.data.imageGuid === '' && nodeAfterSave.data.imageHash === imageHash )
																	{
                                                                        diagram.model.setDataProperty(nodeAfterSave.data, 'imageGuid', newimageGuid);
                                                                        var size = go.Size.parse(nodeAfterSave.data.size);
                                                                        diagram.model.setDataProperty(nodeAfterSave.data, 'source', FMDrawIndex.GeneratePictureURL(newimageGuid, size.width, size.height));
																	}
																} );
														}
													} );
												},
												error: function( xhr, textStatus, error )
												{ //ERROR: AJAX CALL TO SAVE IMAGE
													FMErrorAndExceptionHandling.CloseNotifications();
													FMErrorAndExceptionHandling.ShowException( xhr,
														textStatus,
														error,
														function()
														{
																returnValue = false;
														} );
												}
										} );
									}
									else
									{
										//Disregard any changes made to the data model or graphical objects becase we are saving.
										diagram.skipsUndoManager = true;
										diagram.model.setDataProperty( node.data, 'imageGuid', existingImageGuid );
										//Update any other image controls who have the same hash value but does not have the image guid set.  This will 
										//deal with any unncessary saves of image controls that were cut and pasted from the image control in question.
										//Compare the imageHash based on the calculation from the server (results[2]) to ensure that the server and client 
										//algorithms are in synch.  The setting of the ImageStream property ensures that the imageHash of the picture is 
										//set internally.
										diagram.nodes.each( function( nodeAfterFindingGuid )
										{
												if ( nodeAfterFindingGuid.data.category === 'picture' && nodeAfterFindingGuid.data.imageGuid === '' && nodeAfterFindingGuid.data.imageHash === node.data.imageHash )
												{
													diagram.model.setDataProperty( nodeAfterFindingGuid.data, 'imageGuid', existingImageGuid );
												}
										} );
									}
								}
						} );
					},
					error: function( xhr, textStatus, error )
					{
						FMErrorAndExceptionHandling.CloseNotifications();
						FMErrorAndExceptionHandling.ShowException( xhr,
								textStatus,
								error,
								function()
								{
									returnValue = false;
								} );
					}
				} );
		}
		//If save error occurs returnValue will be false, and this will cause diagram.nodes.each loop to break;
		return returnValue;
	} );
	return returnValue;
};
FMDrawIndex.GetCurrentImage = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();

	var drawingData = {};
	drawingData.layers = [];


	//Save the current ViewPort Position into the modelData
	diagram.model.modelData.position = go.Point.stringify( diagram.position );
	diagram.model.modelData.scale = diagram.scale;
	diagram.model.modelData.gridEnabled = diagram.grid.visible;
	diagram.model.modelData.snapEnabled = diagram.toolManager.draggingTool.isGridSnapEnabled;
	diagram.model.modelData.gridXCellSize = diagram.grid.gridCellSize.width;
	diagram.model.modelData.gridYCellSize = diagram.grid.gridCellSize.height;
	diagram.model.modelData.gridInterval = diagram.grid.elt( 2 ).interval;
	diagram.model.modelData.snapXCellSize = diagram.toolManager.draggingTool.gridSnapCellSize.width;
	diagram.model.modelData.snapYCellSize = diagram.toolManager.draggingTool.gridSnapCellSize.height;
	if ( !diagram.model.modelData.defaultArchetype )
		diagram.model.modelData.defaultArchetype = {};
	Object.keys( FMDrawIndex.defaultArchetype ).forEach( function( key )
			{
				diagram.model.modelData.defaultArchetype[key] = FMDrawIndex.defaultArchetype[key];
			}
	);

	//Persist DrawingData LayerInfo
	var layerManager = new FMDrawIndex._LayerManager();
	var layers = layerManager.GetLayers();
	var i;
	var layer;
	for ( i = 0; i < layers.length; i++ )
	{
		layer = layers[i];
		var layerObj = new FMDrawIndex._LayerDO( layer.name, layer.displayName, layer.allowSelect, layer.visible );
		drawingData.layers.push( layerObj );
	}

	drawingData.primaryLayerName = layerManager.GetPrimaryLayerName();

	//Persist DrawingData Model
	drawingData.model = diagram.model;
	return JSON.stringify( drawingData,
		function replacer( key, value )
		{
				// Filtering out properties
				if ( key === 'goJsLayer' )
				{
					return undefined;
				}
				return value;
		}, '\t' );
};
FMDrawIndex.ClearUndoTransactions = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	diagram.undoManager.clear();
	FMDrawIndex.UpdateUndoButtons();
}
FMDrawIndex.UpdateUndoButtons = function()
{
	if ( !undobutton || !undobutton.id || !redobutton || !redobutton.id )
	{
		return;
	}
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( diagram.undoManager.canUndo() )
	{
		undobutton.src = window.applicationRootName + '/FMWebApp/Images/Undo.png';
	}
	else
	{
		undobutton.src = window.applicationRootName + '/FMWebApp/Images/Undoinactive.png';
	}
	if ( diagram.undoManager.canRedo() )
	{
		redobutton.src = window.applicationRootName + '/FMWebApp/Images/Redo.png';
	}
	else
	{
		redobutton.src = window.applicationRootName + '/FMWebApp/Images/Redoinactive.png';
	}
};
FMDrawIndex.UpdateFlipButtons = function()
{
	if ( !flipverticalbutton || !flipverticalbutton.id || !fliphorizontalbutton || !fliphorizontalbutton.id )
	{
		return;
	}

	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	var selectedObjects = FMDrawIndex.GetSelectedObjects( diagram );


	if ( selectedObjects.length )
	{
		flipverticalbutton.src = window.applicationRootName + '/FMWebApp/Images/Flip-Vertical.png';
		fliphorizontalbutton.src = window.applicationRootName + '/FMWebApp/Images/Flip-Horizontal.png';
	}
	else
	{
		flipverticalbutton.src = window.applicationRootName + '/FMWebApp/Images/Flip-Vertical-Inactive.png';
		fliphorizontalbutton.src = window.applicationRootName + '/FMWebApp/Images/Flip-Horizontal-Inactive.png';
	}
};

FMDrawIndex.UpdateExportButton = function ()
{
	if (!exportbutton || !exportbutton.id)
	{
		return;
	}

	if (FMDrawIndex.CanExport())
	{
		exportbutton.src = window.applicationRootName + '/FMWebApp/Images/Data-Export-24.png';
	}
	else
	{
		exportbutton.src = window.applicationRootName + '/FMWebApp/Images/Data-Export-24-inactive.png';
	}	
}

FMDrawIndex.CanExport = function ()
{
    var diagramType = FMDrawIndex.GetDiagramModelDataValue("PanelType");
	if (diagramType)
	{
		if (diagramType === "Detail")
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}

FMDrawIndex.SaveDefaultstoModelData = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( diagram )
	{
		if ( !diagram.model.modelData.defaultArchetype )
			diagram.model.modelData.defaultArchetype = {};
		Object.keys( FMDrawIndex.defaultArchetype ).forEach( function( key )
		{
			diagram.model.modelData.defaultArchetype[key] = FMDrawIndex.defaultArchetype[key];
		}
		);
	}
}
FMDrawIndex.LoadDefaultsFromModelData = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if (diagram.model.modelData.defaultArchetype) {
		Object.keys(diagram.model.modelData.defaultArchetype).forEach(function (key) {
			FMDrawIndex.defaultArchetype[key] = diagram.model.modelData.defaultArchetype[key];
		}
		);
	}
}
FMDrawIndex.ResetDefaults = function()
{
	FMDrawIndex.defaultArchetype = {
		color: '#99ccff',
		textAlign: 'center',
		strokeWidth: 2,
		font: '12px sans-serif',
		isUnderline: false,
		strokeDashArray: [0, 0],
		stroke: '#000000',
		lineStroke: '#000000',
		patternFillColor: '#99ccff',
		transparency: 0,
		patternStrokeColor: '#ffffff',
	    lineStyleTransparency: 0
	}
}	
FMDrawIndex.RefreshPreview = function()
{
	if ( FMDrawIndex.defaultArchetype.strokeWidth )
		FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty( FMDrawPropertyMenu.propertyPreviewNode.data, "strokeWidth", parseInt( FMDrawIndex.defaultArchetype.strokeWidth ) );
	if ( typeof FMDrawIndex.defaultArchetype.isUnderline !== 'undefined' )
		FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty( FMDrawPropertyMenu.propertyPreviewNode.data, 'isUnderline', FMDrawIndex.defaultArchetype.isUnderline );
	if ( FMDrawIndex.defaultArchetype.font )
	{
		FMDrawPropertyMenu.ParseFontString(FMDrawIndex.defaultArchetype.font);
		var fontStrNoSize = FMDrawPropertyMenu.FontObject.fontStyle + ' '
			+ FMDrawPropertyMenu.FontObject.fontVariant + ' '
			+ FMDrawPropertyMenu.FontObject.fontWeight + ' '
			+ "18px" + ' '
			+ FMDrawPropertyMenu.FontObject.fontFamily;
		FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty( FMDrawPropertyMenu.propertyPreviewNode.data, 'font', fontStrNoSize );
	}
	if ( typeof FMDrawIndex.defaultArchetype.strokeDashArray !== 'undefined' )
		FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty( FMDrawPropertyMenu.propertyPreviewNode.data, 'strokeDashArray', FMDrawIndex.defaultArchetype.strokeDashArray );
	if ( FMDrawIndex.defaultArchetype.stroke )
		FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty( FMDrawPropertyMenu.propertyPreviewNode.data, 'stroke', FMDrawIndex.defaultArchetype.stroke );
	if ( FMDrawIndex.defaultArchetype.lineStroke )
		FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty( FMDrawPropertyMenu.propertyPreviewNode.data, 'lineStroke', FMDrawIndex.defaultArchetype.lineStroke );
	if ( typeof FMDrawIndex.defaultArchetype.transparency !== 'undefined' )
		FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty( FMDrawPropertyMenu.propertyPreviewNode.data, 'transparency', FMDrawIndex.defaultArchetype.transparency );
	if ( FMDrawIndex.defaultArchetype.patternFillColor )
		FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty( FMDrawPropertyMenu.propertyPreviewNode.data, 'patternFillColor', FMDrawIndex.defaultArchetype.patternFillColor );
	if ( FMDrawIndex.defaultArchetype.patternStrokeColor )
	    FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'patternStrokeColor', FMDrawIndex.defaultArchetype.patternStrokeColor);
	if ( FMDrawIndex.defaultArchetype.color )
	{
		if ( typeof ( FMDrawIndex.defaultArchetype.color ) === 'object' )
		{
		var transparency = 0;
		if ( typeof FMDrawIndex.defaultArchetype.transparency !== 'undefined' )
		transparency = FMDrawPropertyMenu.ConvertTransparencyToFloat( FMDrawIndex.defaultArchetype.transparency);
		var hexfill = FMDrawPropertyMenu.Rgb2Hex(FMDrawIndex.defaultArchetype.patternFillColor);
		var hexstroke = FMDrawPropertyMenu.Rgb2Hex(FMDrawIndex.defaultArchetype.patternStrokeColor);
		var patternNumber = parseInt( FMDrawIndex.defaultArchetype.patternImageName);
		var dynamicPattern = FMDrawPatternPalette.CreatePatternForOperate(patternNumber, FMDrawPropertyMenu.ConvertToRgbaString(hexfill, transparency), FMDrawPropertyMenu.ConvertToRgbaString(hexstroke, transparency));
		var brush = new go.Brush( go.Brush.Pattern );
		brush.pattern = dynamicPattern;
		FMDrawIndex.defaultArchetype.color = brush.copy();
		FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty( FMDrawPropertyMenu.propertyPreviewNode.data, 'color', brush.copy() );
		}
else
	FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty( FMDrawPropertyMenu.propertyPreviewNode.data, 'color', FMDrawIndex.defaultArchetype.color );
	}
	FMDrawPropertyMenu.ClearPropertyWindow();
	FMDrawPropertyMenu.SetLineAndPolygonStroke( FMDrawIndex.GetActiveTabGoJSDiagramObject() );
}
FMDrawIndex.ResetSelectedControls = function( controlid, diagram )
{
	if ( !controlid || !diagram )
	{
		return;
	}
	$( '#' + controlid ).css( 'border', '1px solid #4469a2' );
	FMDrawIndex.UnSelectionMode( diagram );
	FMDrawIndex.currentDrawControl = '';
};
FMDrawIndex.ResetAllControls = function( diagram )
{
	$( '#tools img' ).css( 'border', '1px solid #4469a2' );
	if ( !diagram )
	{
		return;
	}
	FMDrawIndex.UnSelectionMode( diagram );
	FMDrawIndex.currentDrawControl = '';
};
FMDrawIndex.DrawingMode = function( controlid )
{
	if ( FMDrawIndex.shapeDragged === true )
	{
		FMDrawIndex.shapeDragged = false;
		return;
	}
	if ( !FMDrawIndex.GetActiveTabGoJSDiagramObject() )
	{
		return;
	}
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	var currentControl = FMDrawIndex.currentDrawControl;
	FMDrawIndex.ResetSelectedControls( currentControl, diagram );
	if ( currentControl === controlid )
	{
		return;
	}
	$( '#' + controlid ).css( 'border', '1px solid #FFFFFF' );

	switch ( controlid )
	{
		case 'quadrangle':
				FMDrawIndex.GenerateRectangle( diagram );
				break;
		case 'triangle':
				FMDrawIndex.GenerateTriangle( diagram );
				break;
		case 'line':
				FMDrawIndex.GenerateLine( diagram );
				break;
		case 'pipe':
				FMDrawIndex.GeneratePipe( diagram );
				break;
		case 'ellipse':
				FMDrawIndex.GenerateCircle( diagram, 0 );
				break;
		case 'circle':
				FMDrawIndex.GenerateCircle( diagram, 1 );
				break;
		case 'text':
				FMDrawIndex.GenerateText( diagram );
				break;
		case 'select':
				FMDrawIndex.SelectionMode( diagram );
				break;
		case 'polyline':
				FMDrawIndex.GeneratePolyLine( diagram );
				break;
		case 'polygon':
				FMDrawIndex.GeneratePolygon( diagram );
				break;
		case 'bar':
				FMDrawIndex.GenerateBar( diagram );
				break;
		case 'picture':
				FMDrawIndex.GeneratePicture( diagram );
				break;
		case 'button':
			   FMDrawPropertyMenu.OpenPropertiesPopupMenu();
			   FMDrawIndex.GenerateButton(diagram);
			   break;
		case 'tag':
				FMDrawIndex.GenerateTag( diagram );
	}
	FMDrawIndex.currentDrawControl = controlid;
};
FMDrawIndex.PersistActiveTool = function( controlid )
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	FMDrawIndex.ResetAllControls( canvas );
	FMDrawIndex.DrawingMode( controlid );
};
FMDrawIndex.SelectionMode = function( diagram )
{
	if ( !diagram )
	{
		return;
	}
	var tool = diagram.toolManager.findTool( 'DragCreating' );
	tool.isEnabled = false;
	tool.successCallBackFunction = undefined;
	tool.activateFunction = undefined;
	tool = diagram.toolManager.findTool( 'DragSelecting' );
	tool.isEnabled = true;
	tool = diagram.toolManager.findTool( 'Panning' );
	tool.isEnabled = false;
	if (FMDrawIndex.panningEnabled === true)
		tool.restorePan = true;
	else
		tool.restorePan = false;
	FMDrawIndex.panningEnabled = false;
	tool = diagram.toolManager.findTool( polygonDrawingToolName );
	tool.isEnabled = false;
	tool = diagram.toolManager.findTool('DisconnectedLinkingTool');
	tool.isEnabled = false;
	tool = diagram.toolManager.findTool( lineToolName );
	tool.isEnabled = false;



	return;
};
FMDrawIndex.UnSelectionMode = function( diagram )
{
	if ( !diagram )
	{
		return;
	}
	var tool = diagram.toolManager.findTool( 'DragCreating' );
	tool.isEnabled = false;
	tool.successCallBackFunction = undefined;
	tool.activateFunction = undefined;
	tool = diagram.toolManager.findTool( 'DragSelecting' );
	tool.isEnabled = false;
	tool = diagram.toolManager.findTool('Panning');
	if (tool.restorePan === true) {
		tool.isEnabled = true;
		FMDrawIndex.panningEnabled = true;
	}
	tool = diagram.toolManager.findTool(polygonDrawingToolName);
	tool.isEnabled = false;
	tool = diagram.toolManager.findTool('DisconnectedLinkingTool');
	tool.isEnabled = false;
	tool = diagram.toolManager.findTool( lineToolName );
	tool.isEnabled = false;


	return;
};
FMDrawIndex.UpdateTabDrawingGuid = function( guidString )
{
	var domElement = FMDrawIndex.GetActiveTabDOMElement();
	if ( !domElement )
	{
		return;
	}
	domElement.attr( 'drawingguid', guidString );
};
FMDrawIndex.compareStrings = function( string1, string2, ignoreCase, useLocale )
{
	if ( ignoreCase )
	{
		if ( useLocale )
		{
				string1 = string1.toLocaleLowerCase();
				string2 = string2.toLocaleLowerCase();
		}
		else
		{
				string1 = string1.toLowerCase();
				string2 = string2.toLowerCase();
		}
	}

	return string1 === string2;
};
FMDrawIndex.LoadDrawing = function( drawingGuid )
{
	var namearray = drawingGuid.split( ',' );
	if ( FMDrawPropertyMenu.currentButtonActionAssociation &&
		FMDrawPropertyMenu.onlyAssociateDrawingIdToButton &&
		FMDrawPropertyMenu.PropertyActiveObject &&
		FMDrawPropertyMenu.PropertyActiveObject.data &&
		namearray[0] )
	{
		var obj = FMDrawPropertyMenu.PropertyActiveObject;
		if ( obj && obj.diagram )
		{
				FMDrawPropertyMenu.SetButtonActionTypeConfiguration(obj, { drawingGuid: namearray[0] }, namearray[1], true, true);
				FMDrawPropertyMenu.ClearButtonTagData( obj, true );
				FMDrawPropertyMenu.onlyAssociateDrawingIdToButton = false;
		}
		return;
	}

	var foundGuid = false;
	var indexFound = 0;
	$( '.drawingTab' ).each( function()
	{
		if ( FMDrawIndex.compareStrings( ( $( this ).attr( 'drawingguid' ) || '' ), ( namearray[0] || 'NoValue' ), true, false ) )
		{
				foundGuid = true;
				indexFound = $(this).attr('id').replace('tabs', '');
				return false;
		}
		return true;
	} );
	if ( foundGuid )
	{
		$( '#alreadyopen-dialog' ).dialog( 'open' );
		$( '#alreadyopen-dialog' ).html( namearray[1] + ' is already open.' );
		$( 'div#tabs' ).tabs( 'option', 'active', indexFound );
		return;
	}

	FMDrawIndex.SaveDefaultstoModelData();
	FMDraw.num_tabs = FMDraw.num_tabs + 1;
	var diagramID = 'diagram' + ( FMDraw.num_tabs - 1 );
	var tabID = 'tab' + ( FMDraw.num_tabs - 1 );
	var diagramJQueryID = '#' + diagramID;
	var tabJQueryID = '#' + tabID;
	var canvasJQueryID = '#' + diagramID + ' > canvas';
	var tabIndex = FMDraw.num_tabs - 1;

	$( 'div#tabs ul' ).append(
		'<li class=\'drawingTab\' drawingguid=\'' + namearray[0] + '\' id=\'tabs' + tabIndex + '\' class=\'ui-closable-tab\'><a href=\'#' + tabID + '\'>' + namearray[1] + '</a><a id=\'close\' onClick=\'return FMDraw.close(' + ( FMDraw.num_tabs - 1 ) + ');\' style=\'margin-left:-10px; font-weight: 900;\'>X</a></li>'
	);
	$( 'div#tabs' ).append(
		'<div id=\'' + tabID + '\'>' + '<div id=\'' + diagramID + '\'> Your browser does not support the HTML5 canvas.</canvas>' + '</div>'
	);
	$( 'div#tabs' ).tabs( 'refresh' );
	FMDrawIndex.AttatchGoJSDiagramToNewCanvas( FMDraw.num_tabs - 1 );
	FMDrawIndex.UpdateactiveCanvas( FMDraw.num_tabs - 1 );
	FMDrawIndex.StoreMetadata( FMDraw.num_tabs - 1, namearray[1] + ',' + namearray[2] );
	var tabname = '#tab' + FMDraw.num_tabs - 1;
	var index = $( '#tabs a[href=\'' + tabname + '\']' ).parent().index();
	$( 'div#tabs' ).tabs( 'option', 'active', index );

	var tabDiv = $( tabJQueryID );
	var diagramDiv = $( diagramJQueryID );
	var canvas = $(canvasJQueryID);
    //Ensure that initial panel during load of page is a standard panel
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	FMDrawIndex.InitContextMenu( canvas, diagram );
	diagramDiv.width( tabDiv.width() );
	diagramDiv.height( tabDiv.height() );
	canvas.width( diagramDiv.width() );
	canvas.height( diagramDiv.height() );
	canvas.addClass( 'upper-canvas' );

	var token = $( 'input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	$.ajax( {
		type: 'GET',
		dataType: 'json',
		cache: false,
		headers: headers,
		url: 'GetDrawing',
		data: 'id=' + namearray[0],
		success: function( response )
		{
				FMErrorAndExceptionHandling.HandleMessages( response, function( results, inError )
				{
					if ( inError )
					{
						return;
					}
					FMDrawIndex.RenderDrawing( results, true );
					FMDrawIndex.InitializeZoomLevel();
					FMDrawIndex.ClearUndoTransactions();
                    FMDrawIndex.ResizeDrawingWorkSpace();
                    FMDrawIndex.UpdateExportButton();
				    FMDrawIndex.UpdateMode();
				} );
		},
		error: function( xhr, textStatus, error )
		{
				FMErrorAndExceptionHandling.ShowException( xhr,
					textStatus,
					error,
					function()
					{
						$( tabname ).remove(); //Ensure that Tab is Removed if Drawing could not be loaded.
					} );
		}
	} );
};
FMDrawIndex.DeleteDrawing = function (drawingGuid) {
	var namearray = drawingGuid.split(',');
	if (FMDrawPropertyMenu.currentButtonActionAssociation &&
		FMDrawPropertyMenu.onlyAssociateDrawingIdToButton &&
		FMDrawPropertyMenu.PropertyActiveObject &&
		FMDrawPropertyMenu.PropertyActiveObject.data &&
		namearray[0]) {
		var obj = FMDrawPropertyMenu.PropertyActiveObject;
		if (obj && obj.diagram) {
			FMDrawPropertyMenu.SetButtonActionTypeConfiguration(obj, { drawingGuid: namearray[0] }, namearray[1], true, true);
			FMDrawPropertyMenu.ClearButtonTagData(obj, true);
			FMDrawPropertyMenu.onlyAssociateDrawingIdToButton = false;
		}
		return;
	}


	var foundGuid = false;
	var indexFound = 0;
	$('.drawingTab').each(function () {
		if (FMDrawIndex.compareStrings(($(this).attr('drawingguid') || ''), (namearray[0] || 'NoValue'), true, false)) {
			foundGuid = true;
			indexFound = $(this).attr('id').replace('tabs','');
			return false;
		}
		return true;
	});

	if ($("div#tabs >ul >li").size() == 1 && foundGuid) {
		FMLayout.Alert("One tab must remain active!", 'Alert', null);
		return;
	}


	FMLayout.ConfirmYesNo('Are you sure you want to delete drawing : ' + namearray[1] + '.',
		'Delete Drawing',
		function () {
			if (foundGuid) {
				FMDraw.closeTarget = indexFound;
				FMDraw.closetab(true);
			}

			var token = $('input[name=__RequestVerificationToken]').val();
			var headers = {};
			headers['__RequestVerificationToken'] = token;

			$.ajax({
				type: 'GET',
				dataType: 'json',
				cache: false,
				headers: headers,
				url: 'DeleteDrawing',
				data: 'id=' + namearray[0],
				success: function (response) {
					FMErrorAndExceptionHandling.HandleMessages(response, function (results, inError) {
						if (inError) {
							return;
						}
					});
				},
				error: function (xhr, textStatus, error) {
					FMErrorAndExceptionHandling.ShowException(xhr,
						textStatus,
						error,
						function () {
							$(tabname).remove(); //Ensure that Tab is Removed if Drawing could not be loaded.
						});
				}
			});
		},
		function () {
			return;
		});



};
FMDrawIndex.SetTextProperties = function( shape )
{
	var width = 200;
	var height = 15;
	shape.set( {
		width: width,
		height: height,
		fill: 'black',
		stroke: 'transparent',
		strokeWidth: 0,
		realStrokeWidth: 0,
		editable: true,
		fontWeight: 'normal',
		fontStyle: 'normal',
		fontFamily: 'Arial',
		fontSize: 24,
		selectable: true,
		lockScalingFlip: true
	} );
};
FMDrawIndex.GenerateRectangle = function( diagram )
{
	if ( !diagram )
	{
		return;
	}

	var tool = diagram.toolManager.findTool( 'DragCreating' );
	var layerManager = new FMDrawIndex._LayerManager();
	tool.archetypeNodeData = {
		category: 'rectangle',
		color: FMDrawIndex.shapeFillColor,
		strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
		layerName: layerManager.GetPrimaryLayerName()
	};

	tool.box = $$( go.Part,
				{
					layerName: 'Tool'
				},
				$$( go.Shape,
					{
						name: 'SHAPE',
						fill: null,
						stroke: 'cyan',
						strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
						figure: 'rectangle'
					} )
		),
		tool.isEnabled = true;
};
FMDrawIndex.GenerateArc = function( diagram )
{
};
FMDrawIndex.GeneratePipe = function( diagram )
{
	if ( !diagram )
	{
		return;
	}

	var tool = diagram.toolManager.findTool('DisconnectedLinkingTool');
	var layerManager = new FMDrawIndex._LayerManager();
	tool.archetypeNodeData = {	category: 'lineLink', color: FMDrawIndex.defaultArchetype.color, strokeWidth: FMDrawIndex.defaultArchetype.strokeWidth, layerName: layerManager.GetPrimaryLayerName()
	};
	tool.isEnabled = true;
//tool.isComplexRoutingRealtime = true;
	tool.boxArchetypeNodeData = { category: 'lineLink', stroke: 'red' };
};
FMDrawIndex.GenerateText = function( diagram )
{
	if ( !diagram )
	{
		return;
	}
	var tool = diagram.toolManager.findTool( 'DragCreating' );
	var layerManager = new FMDrawIndex._LayerManager();
	tool.archetypeNodeData = {
		category: 'text',
		color: 'white',
		key: '',
		layerName: layerManager.GetPrimaryLayerName()
	};
	tool.box = $$( go.Part,
				{ layerName: 'Tool' },
				$$( go.Shape,
					{
						name: 'SHAPE',
						fill: null,
						stroke: 'cyan',
						strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
						figure: 'rectangle'
					} )
		),
		tool.isEnabled = true;
};
FMDrawIndex.GenerateTriangle = function( diagram )
{
	if ( !diagram )
	{
		return;
	}

	// Define the template for Nodes
	var tool = diagram.toolManager.findTool( 'DragCreating' );
	var layerManager = new FMDrawIndex._LayerManager();
	tool.archetypeNodeData = {
		category: 'triangle',
		color: FMDrawIndex.shapeFillColor,
		strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
		layerName: layerManager.GetPrimaryLayerName()
	};
	tool.box = $$( go.Part,
				{ layerName: 'Tool' },
				$$( go.Shape,
					{ name: 'SHAPE', fill: null, stroke: 'cyan', strokeWidth: FMDrawIndex.defaultShapeStrokeWidth, figure: 'triangle' } )
		),
		tool.isEnabled = true;
};
FMDrawIndex.GenerateLine = function( diagram )
{
	if ( !diagram )
	{
		return;
	}
	// Define the template for Nodes


	FMDrawIndex.LineMode( true, diagram );
};
FMDrawIndex.GeneratePolyLine = function( diagram )
{
	if ( !diagram )
	{
		return;
	}
	// Define the template for Nodes


	FMDrawIndex.PolygonMode( true, false, diagram );
};
FMDrawIndex.GeneratePolygon = function( diagram )
{
	if ( !diagram )
	{
		return;
	}
	// Define the template for Nodes


	FMDrawIndex.PolygonMode( true, true, diagram );
};
FMDrawIndex.GenerateCircle = function( layer, mode )
{
	if ( !layer || typeof mode == 'undefined' )
	{
		return;
	}

	var tool = layer.toolManager.findTool( 'DragCreating' );
	var categoryString = ( mode === 0 ) ? 'ellipse' : 'circle';
	var layerManager = new FMDrawIndex._LayerManager();
	tool.archetypeNodeData = {
		category: categoryString,
		color: FMDrawIndex.shapeFillColor,
		strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
		// If the type of node being created requires a square bounding box (i.e. a Circle) then the archetypeNodeData should 
		//define the squareBoundingBox property as a boolean and set that property value to 'true'
		squareBoundingBox: ( mode !== 0 ),
		layerName: layerManager.GetPrimaryLayerName()
	};
	tool.box = $$( go.Part, {
		layerName: 'Tool'
	}, $$( go.Shape, {
		name: 'SHAPE',
		fill: null,
		stroke: 'cyan',
		strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
		figure: categoryString
	} ) ), tool.isEnabled = true;
};
FMDrawIndex.GenerateBarSuccessCallBack = function()
{
	FMDrawIndex.OpenTagDialog( false, FMDrawIndex.InitializeBarSuccess );
};
FMDrawIndex.GenerateBar = function( diagram )
{
	if ( !diagram )
	{
		return;
	}

	var tool = diagram.toolManager.findTool( 'DragCreating' );
	var layerManager = new FMDrawIndex._LayerManager();
	tool.archetypeNodeData = {
		category: 'bar',
		color: FMDrawIndex.shapeFillColor,
		strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
		layerName: layerManager.GetPrimaryLayerName(),
		barType: 'Standard',
		TagFieldSelection: FMTAGFIELDSELECTION.VALUE
	};
	tool.box = $$( go.Part,
		{
				layerName: 'Tool'
		},
		$$( go.Shape,
				{
					name: 'SHAPE',
					fill: null,
					stroke: 'cyan',
					strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
					figure: 'rectangle'
				} )
	);
	tool.successCallBackFunction = FMDrawIndex.GenerateBarSuccessCallBack;
	tool.activateFunction = FMDrawIndex.StartBarTransaction;
	tool.isEnabled = true;
};
FMDrawIndex.StartBarTransaction = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	diagram.startTransaction( FMDrawIndex.transactionBar );
};
FMDrawIndex.GeneratePicture = function( diagram )
{
	if ( !diagram )
	{
		return;
	}
	// Define the template for Nodes

	var tool = diagram.toolManager.findTool( 'DragCreating' );
	var layerManager = new FMDrawIndex._LayerManager();
	tool.archetypeNodeData = {
		category: 'picture',
		color: FMDrawIndex.shapeFillColor,
		layerName: layerManager.GetPrimaryLayerName()
	};
	tool.box = $$( go.Part,
				{
					layerName: 'Tool'
				},
				$$( go.Shape,
					{
						name: 'SHAPE',
						fill: null,
						stroke: 'cyan',
						strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
						figure: 'rectangle'
					} )
		),
		tool.isEnabled = true;
};
FMDrawIndex.GenerateButton = function( diagram )
{
	if ( !diagram )
	{
		return;
	}

	// Define the template for Nodes
	var tool = diagram.toolManager.findTool( 'DragCreating' );
	// ReSharper disable once UnusedLocals
	var layerManager = new FMDrawIndex._LayerManager();
	tool.archetypeNodeData = {
		category: 'button',
		layerName: layerManager.GetPrimaryLayerName(),
		color: go.GraphObject.make( go.Brush, 'Linear', { 0: 'white', 1: 'lightgray' } ),
		lineStroke: 'gray'
	};
	tool.box = $$( go.Part,
				{
					layerName: 'Tool'
				},
				$$( go.Shape,
					{
						name: 'SHAPE',
						fill: null,
						stroke: 'cyan',
						strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
						figure: 'rectangle'
					} )
		),
		tool.isEnabled = true;
};
FMDrawIndex.GenerateTagSuccessCallBack = function()
{
	FMDrawIndex.OpenTagDialog( true, FMDrawIndex.PointTagCallSuccess );
};
FMDrawIndex.GenerateTag = function( diagram )
{
	if ( !diagram )
	{
		return;
	}
	// Define the template for Nodes
	var tool = diagram.toolManager.findTool( 'DragCreating' );
	var layerManager = new FMDrawIndex._LayerManager();
	var primaryLayerName = layerManager.GetPrimaryLayerName();
	tool.archetypeNodeData = {
		category: 'tag',
		color: FMDrawIndex.shapeFillColor,
		layerName: primaryLayerName,
		zOrder: FMDrawIndex.GetNextPartZOrder( primaryLayerName )
	};
	tool.box = $$( go.Part,
		{
				layerName: 'Tool'
		},
		$$( go.Shape,
				{
					name: 'SHAPE',
					fill: null,
					stroke: 'cyan',
					strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
					figure: 'rectangle'
				}
		)
	);
	tool.successCallBackFunction = FMDrawIndex.GenerateTagSuccessCallBack;
	tool.activateFunction = FMDrawIndex.StartTagTransaction;
	tool.isEnabled = true;
};
FMDrawIndex.StartTagTransaction = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	diagram.startTransaction( FMDrawIndex.transactionTag );
};
FMDrawIndex.DrawRectangle = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'quadrangle' );
};
FMDrawIndex.DrawPolyLine = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'polyline' );
};
FMDrawIndex.DrawPolygon = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'polygon' );
};
FMDrawIndex.DrawBar = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'bar' );
};
FMDrawIndex.DrawText = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'text' );
};
FMDrawIndex.DrawEllipse = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'ellipse' );
};
FMDrawIndex.DrawCircle = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'circle' );
};
FMDrawIndex.DrawLine = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'line' );
};
FMDrawIndex.DrawPipe = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'pipe' );
};
FMDrawIndex.DrawTriangle = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'triangle' );
};
FMDrawIndex.SelectionClick = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'select' );
};
FMDrawIndex.DrawPicture = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'picture' );
};
FMDrawIndex.DrawButton = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'button' );
};
FMDrawIndex.DrawTag = function( e )
{
	e.preventDefault();
	FMDrawIndex.DrawingMode( 'tag' );
};
FMDrawIndex.AddNodeToDiagram = function( diagram, nodeData, point )
{
	if ( typeof diagram === 'undefined' || diagram === null )
	{
		return null;
	}

	if ( nodeData === null || typeof nodeData === 'undefined' )
	{
		return null;
	}

	point = point || new go.Point( 0, 0 );

	//Add layerName and zOrder to shapeData Data
	var layerManager = new FMDrawIndex._LayerManager();
	nodeData.layerName = layerManager.GetPrimaryLayerName();
	nodeData.zOrder = FMDrawIndex.GetNextPartZOrder( nodeData.layerName );
	diagram.startTransaction( 'new node' );

	var part = null;

	var data = diagram.model.copyNodeData( nodeData );
	if ( data )
	{
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

	    }
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
		part = diagram.findPartForData( data );
	}

	if ( part !== null && typeof part !== 'undefined' )
	{
		part.position = point;
		part.resizeObject.size = go.Size.parse( nodeData.size );
		if ( diagram.allowSelect )
		{
		    diagram.select(part); // raises ChangingSelection/Finished

		    if (SetColorObject === true && data.color !== null && typeof (data.color) === 'object') {

		        var patternNumber = parseInt(data.patternImageName);
		        var patternTagId = 'canvasPalatte-propertiesMenu-FILLPATTERNPALETTE-' + data.patternImageName;
		        FMDrawPropertyMenu.HandleFillPatternOnClick(patternTagId, patternNumber, false, -99, true);
		    }
        }
	}

	// set the TransactionResult before raising event, in case it changes the result or cancels the tool

	diagram.commitTransaction( 'new node' );

	if ( FMDrawIndex.IsSnapToGridOn && part !== null )
	{
		var list = new go.List( go.Part );
		list.add( part );
		var offset = new go.Point( 0, 0 );
		diagram.moveParts( list, offset, false );
	}

	return part;
};
FMDrawIndex.AddShapeFromDragAndDrop = function( nodeData, event )
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();

	if ( typeof diagram === 'undefined' || diagram === null )
	{
		return null;
	}

	if ( nodeData === null || typeof nodeData === 'undefined' )
	{
		return null;
	}

	if ( !event )
	{
		return null;
	}

	var point;
	if ( event instanceof go.DiagramEvent && event.name === 'BackgroundSingleClicked' )
	{
		point = event.diagram.lastInput.documentPoint.copy();
	}
	else
	{
		var pixelratio = diagram.computePixelRatio();
		var canvas = $( '#' + diagram.div.id + ' > canvas' );
		var bbox = canvas.get( 0 ).getBoundingClientRect();
		var bbw = bbox.width;
		if ( bbw === 0 )
		{
				bbw = 0.001;
		}
		var bbh = bbox.height;
		if ( bbh === 0 )
		{
				bbh = 0.001;
		}
		var mx = event.clientX - bbox.left * ( ( canvas.width() / pixelratio ) / bbw );
		var my = event.clientY - bbox.top * ( ( canvas.height() / pixelratio ) / bbh );
		point = diagram.transformViewToDoc( new go.Point( mx, my ) );
	}
	var copy = JSON.parse( JSON.stringify( FMDrawIndex.defaultArchetype ) ); //copying default node data to get current default properties
	if ( nodeData.category )
	{
		copy.category = nodeData.category;
	}
	if ( nodeData.size )
	{
		copy.size = nodeData.size;
	}
	if ( nodeData.layerName )
	{
		copy.layerName = nodeData.layerName;
	}
	if ( nodeData.bgsize )
	{
		copy.bgsize = nodeData.bgsize;
	}
	if ( nodeData.TagFieldSelection )
	{
		copy.TagFieldSelection = nodeData.TagFieldSelection;
	}
	if ( nodeData.barType )
	{
		copy.barType = nodeData.barType;
	}
	if ( nodeData.demoPercent )
	{
		copy.demoPercent = nodeData.demoPercent;
	}
	if ( FMDrawIndex.defaultArchetype.alignment )
	{
		copy.alignment = FMDrawIndex.defaultArchetype.alignment.copy();
	}

	if (FMDrawIndex.defaultArchetype.color)
	{
		if (typeof FMDrawIndex.defaultArchetype.color === 'object')
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

	return FMDrawIndex.AddNodeToDiagram( diagram, copy, point );
};

//==========================================================================
// This function will set the color to no transparency. It will return
// a new RGBA color string. If not an RGBA string then, it will return
// the original color.
//==========================================================================
FMDrawIndex.RemoveTransparencyFromColor = function( originalColor )
{
	var newColor = originalColor;

	if (typeof (originalColor) != "string")
	{
	    return originalColor;
	}
	// It was decided in MVP Testing that transparency be defaulted to NO
	// transparency.
	var rgbaIndex = originalColor.indexOf("rgba");

	if (rgbaIndex >= 0)
	{
		var parts = originalColor.split(",");
		if (parts != null && parts.length === 4)
		{
			newColor = parts[0] + ", " + parts[1] + ", " + parts[2] + ", 1)";
		}
	}

	return newColor;
};

FMDrawIndex.AddRectangleFromDragAndDrop = function( startPos, event, ui )
{
	if ( !FMDrawIndex.GetActiveTabGoJSDiagramObject() )
	{
		return;
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 10 )
	{
		FMDrawIndex.DrawingMode( 'quadrangle' );
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 50 )
	{
		return;
	}
	var layerManager = new FMDrawIndex._LayerManager();
	FMDrawIndex.AddShapeFromDragAndDrop( {
		category: 'rectangle',
		size: '150 100',
		//color: FMdrawindex.shapeFillColor,
		//strokeWidth: FMdrawindex.defaultShapeStrokeWidth,
		layerName: layerManager.GetPrimaryLayerName()
	}, event );
};
FMDrawIndex.AddTextFromDragAndDrop = function( startPos, event, ui )
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 10 )
	{
		FMDrawIndex.DrawingMode( 'text' );
	}
	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 50 )
	{
		return;
	}

	var layerManager = new FMDrawIndex._LayerManager();
	var part = FMDrawIndex.AddShapeFromDragAndDrop( {
		category: 'text',
		color: 'white',
		key: '',
		size: '100 20',
		layerName: layerManager.GetPrimaryLayerName()
	}, event );

	//Activate Text Editing tool right away
	if ( part )
	{
		var tool = diagram.toolManager.textEditingTool;
		var obj = part.findObject( 'TEXTBLOCK' );
		tool.textBlock = obj;
		//Ensure that text editing tool is current tool so when navigating away from object it will cause tool to deactivate.
		diagram.currentTool = tool;
		tool.doStart();
	}
};
FMDrawIndex.AddCircleFromDragAndDrop = function( startPos, event, ui )
{
	if ( !FMDrawIndex.GetActiveTabGoJSDiagramObject() )
	{
		return;
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 10 )
	{
		FMDrawIndex.DrawingMode( 'circle' );
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 50 )
	{
		return;
	}

	var layerManager = new FMDrawIndex._LayerManager();
	FMDrawIndex.AddShapeFromDragAndDrop(
		{
				category: 'circle',
				squareBoundingBox: true,
				size: '100 100',
				color: FMDrawIndex.shapeFillColor,
				strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
				layerName: layerManager.GetPrimaryLayerName()
		}, event );
};
FMDrawIndex.AddEllipseFromDragAndDrop = function( startPos, event, ui )
{
	if ( !FMDrawIndex.GetActiveTabGoJSDiagramObject() )
	{
		return;
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 10 )
	{
		FMDrawIndex.DrawingMode( 'ellipse' );
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 50 )
	{
		return;
	}

	var layerManager = new FMDrawIndex._LayerManager();
	FMDrawIndex.AddShapeFromDragAndDrop( {
		category: 'ellipse',
		size: '150 100',
		color: FMDrawIndex.shapeFillColor,
		strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
		layerName: layerManager.GetPrimaryLayerName()
	}, event );
};
FMDrawIndex.AddTriangleFromDragAndDrop = function( startPos, event, ui )
{
	if ( !FMDrawIndex.GetActiveTabGoJSDiagramObject() )
	{
		return;
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 10 )
	{
		FMDrawIndex.DrawingMode( 'triangle' );
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 50 )
	{
		return;
	}

	FMDrawIndex.AddShapeFromDragAndDrop( {
		category: 'triangle',
		size: '100 100',
		color: FMDrawIndex.shapeFillColor,
		strokeWidth: FMDrawIndex.defaultShapeStrokeWidth
	}, event );
};
FMDrawIndex.GeneratePictureURL = function( pictureGuid, width, height )
{
	 return $("#applicationName").val() + '/DisplayImage.ashx?PictureGuid=' + pictureGuid + '&Width=' + width + '&Height=' + height;
};
FMDrawIndex.AddPictureFromDragAndDrop = function( startPos, event, ui )
{
	if ( !FMDrawIndex.GetActiveTabGoJSDiagramObject() )
	{
		return;
	}
	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 10 )
	{
		FMDrawIndex.DrawingMode( 'picture' );
	}
	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 50 )
	{
		return;
	}
	var width = 200, height = 200;
	var layerManager = new FMDrawIndex._LayerManager();
	FMDrawIndex.AddShapeFromDragAndDrop( {
		category: 'picture',
		size: width + ' ' + height,
		color: FMDrawIndex.shapeFillColor,
		layerName: layerManager.GetPrimaryLayerName()
	}, event );
};
FMDrawIndex.AddBarFromDragAndDrop = function( startPos, event, ui )
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !canvas )
	{
		return;
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 10 )
	{
		FMDrawIndex.DrawingMode( 'bar' );
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 50 )
	{
		return;
	}

	canvas.startTransaction( FMDrawIndex.transactionBar );

	var layerManager = new FMDrawIndex._LayerManager();
	FMDrawIndex.AddShapeFromDragAndDrop( {
		category: 'bar',
		bgsize: '50 200',
		size: '50 200',
		color: FMDrawIndex.shapeFillColor,
		strokeWidth: FMDrawIndex.defaultShapeStrokeWidth,
		layerName: layerManager.GetPrimaryLayerName(),
		barType: 'Standard',
		TagFieldSelection: FMTAGFIELDSELECTION.VALUE,
		demoPercent: 100
	}, event );
	FMDrawIndex.OpenTagDialog( false, FMDrawIndex.InitializeBarSuccess );
};
FMDrawIndex.BarSuccess = function( pointValue, node )
{
	var data = node.data;

	if ( !data.maxHeight )
	{
		node.diagram.model.setDataProperty( data, 'maxHeight', 200 );
	}
	if ( !data.bothwidth )
	{
		node.diagram.model.setDataProperty(data, 'bothwidth', 50);
	}
	node.diagram.model.setDataProperty( data, 'maxVal', 100 );
	node.diagram.model.setDataProperty( data, 'minVal', 0 );
	node.diagram.model.setDataProperty( data, 'maxUserVal', 100 );
	node.diagram.model.setDataProperty( data, 'minUserVal', 0 );
	node.diagram.model.setDataProperty( data, 'useTagLimits', 'true' );
	node.diagram.model.setDataProperty( data, 'demoPercent', 100 );
	node.diagram.model.setDataProperty( data, 'val', 0 );
	node.diagram.model.setDataProperty( data, 'bgcolor', '#000000' );
	node.diagram.model.setDataProperty(data, 'barType', 'Standard');
	node.diagram.model.setDataProperty(data, 'useProductColor', false);
	node.diagram.model.setDataProperty(data, 'useAlarmLevel', false);

    node.diagram.model.setDataProperty(data, 'PointTemplateTagSelectionIndicator', pointValue.IsPointTemplateValue);
	node.diagram.model.setDataProperty(data, 'TagGUID', pointValue.PointValueIdentifier.IdentityGuid);
	node.diagram.model.setDataProperty(data, 'TagPointValueType', pointValue.PointValueIdentifier.PointValueType);
	node.diagram.model.setDataProperty(data, 'TagPropertyID', pointValue.PointValueIdentifier.PropertyID);
	node.diagram.model.setDataProperty(data, 'PointGUID', pointValue.PointGuid);
	node.diagram.model.setDataProperty(data, 'TagPointID', pointValue.PointID);
	node.diagram.model.setDataProperty(data, 'TagTagID', pointValue.ID);
	node.diagram.model.setDataProperty(data, 'TagPointIDAndTagID', pointValue.PointID + ' : ' + pointValue.ID);
	node.diagram.model.setDataProperty(data, 'maxVal', pointValue.Maximum);
	node.diagram.model.setDataProperty(data, 'minVal', pointValue.Minimum);
	node.diagram.model.setDataProperty(data, 'maxUserVal', pointValue.Maximum);
	node.diagram.model.setDataProperty(data, 'minUserVal', pointValue.Minimum);
	node.diagram.model.raiseDataChanged( data, 'maxHeight', 200, 200 );
};
FMDrawIndex.BarTagChangeSuccess = function( pointValue, node )
{
    var data = node.data;
    node.diagram.model.setDataProperty(data, 'PointTemplateTagSelectionIndicator', pointValue.IsPointTemplateValue);
	node.diagram.model.setDataProperty(data, 'TagGUID', pointValue.PointValueIdentifier.IdentityGuid);
	node.diagram.model.setDataProperty(data, 'TagPointValueType', pointValue.PointValueIdentifier.PointValueType);
	node.diagram.model.setDataProperty(data, 'TagPropertyID', pointValue.PointValueIdentifier.PropertyID);
	node.diagram.model.setDataProperty(data, 'PointGUID', pointValue.PointGuid);
	node.diagram.model.setDataProperty(data, 'TagPointID', pointValue.PointID);
	node.diagram.model.setDataProperty(data, 'TagTagID', pointValue.ID);
	node.diagram.model.setDataProperty(data, 'TagPointIDAndTagID', pointValue.PointID + ' : ' + pointValue.ID);
	node.diagram.model.setDataProperty(data, 'maxVal', pointValue.Maximum);
	node.diagram.model.setDataProperty(data, 'minVal', pointValue.Minimum);
	if (data.maxUserVal > pointValue.Maximum)
	{
		node.diagram.model.setDataProperty(data, 'maxUserVal', pointValue.Maximum);
	}
	if (data.minUserVal < pointValue.Minimum)
	{
		node.diagram.model.setDataProperty(data, 'minUserVal', pointValue.Minimum);
	}
};
FMDrawIndex.InitializeBarSuccess = function( response )
{
	if ( response )
	{
		var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
		var selectedObjects = FMDrawIndex.GetSelectedObjects( canvas );
		var node = selectedObjects[0];
		var tagSelectionModel = response;
		var pointValues = tagSelectionModel.PointValues;
		var pointValue = pointValues[0];
		FMDrawIndex.BarSuccess( pointValue, node );
		node.diagram.commitTransaction( FMDrawIndex.transactionBar );
	}
	else
	{
		FMDrawIndex.TagSelectionCancel();
	}
};
FMDrawIndex.UpdatePointTrendButtonSuccessHelper = function (canvas, pointValue, button) {
	var transactionTag = 'attach modify point';

	canvas.startTransaction(transactionTag);
	FMDrawIndex.ButtonTagChangeSuccess(pointValue, button, true);
	canvas.commitTransaction(transactionTag);

};
FMDrawIndex.UpdatePointTrendButtonSuccessPrime = function (canvas, selectedObjects, pointValues, errMsg) {
	var node, pointValue;
	pointValue = null;

	if (pointValues)
	{
		pointValue = pointValues[0];
	}

	if (FMDrawPropertyMenu.MultiSelectionFlag)
	{

		for (var nextObjIndex = 0; nextObjIndex < selectedObjects.length; nextObjIndex++)
		{
			node = selectedObjects[nextObjIndex];

			if (FMDrawPropertyMenu.isObjectGroup(node))
			{
				var arr = FMDrawPropertyMenu.IteratorToArray(node.memberParts);
				errMsg += FMDrawIndex.UpdatePointTrendButtonSuccessPrime(canvas, arr, pointValues, errMsg);
			}
			else
			{
				if (pointValue) {
					FMDrawIndex.UpdatePointTrendButtonSuccessHelper(canvas, pointValue, node);
					FMDrawPropertyMenu.manualSetPointIDFlag = true;
				}
				else {
					if (errMsg && errMsg.length > 0) {
						errMsg += ', ';
					}
					errMsg += node.data.TagTagID;
				}
			}
		}
	}
	else
	{
		node = selectedObjects[0];
		FMDrawIndex.UpdatePointTrendButtonSuccessHelper(canvas, pointValue, node);
	}

	FMDrawPropertyMenu.onlyAssociateTagDataToButton = false;
	FMDrawPropertyMenu.currentButtonActionAssociation = null;
	return errMsg;
};
FMDrawIndex.UpdatePointTrendButtonSuccess = function (response) {
	var alertTitle = 'Point Error';

	if (response)
	{
		var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
		var selectedObjects = FMDrawIndex.GetSelectedObjects(canvas);
		var tagSelectionModel = response;
		var pointValues = tagSelectionModel.PointValues;
		var errMsg = '';
		errMsg = FMDrawIndex.UpdatePointTrendButtonSuccessPrime(canvas, selectedObjects, pointValues, errMsg);

		if (errMsg.length > 0)
		{
			errMsg = 'Tag ID ' + errMsg;
			errMsg += ' not found on selected point!';
			FMLayout.Alert(errMsg, alertTitle, null);
		}
	}
};

FMDrawIndex.UpdatePointHistoryButtonSuccessHelper = function (canvas, pointValue, button) {
	var transactionTag = 'attach modify point';

	canvas.startTransaction(transactionTag);
	FMDrawIndex.ButtonTagChangeSuccess(pointValue, button, true);
	canvas.commitTransaction(transactionTag);

};
FMDrawIndex.UpdatePointHistoryButtonSuccessPrime = function (canvas, selectedObjects, pointValues, errMsg) {
	var node, pointValue;
	pointValue = null;

	if (pointValues) {
		pointValue = pointValues[0];
	}

	if (FMDrawPropertyMenu.MultiSelectionFlag) {

		for (var nextObjIndex = 0; nextObjIndex < selectedObjects.length; nextObjIndex++) {
			node = selectedObjects[nextObjIndex];

			if (FMDrawPropertyMenu.isObjectGroup(node)) {
				var arr = FMDrawPropertyMenu.IteratorToArray(node.memberParts);
				errMsg += FMDrawIndex.UpdatePointHistoryButtonSuccessPrime(canvas, arr, pointValues, errMsg);
			}
			else {
				if (pointValue) {
					FMDrawIndex.UpdatePointHistoryButtonSuccessHelper(canvas, pointValue, node);
					FMDrawPropertyMenu.manualSetPointIDFlag = true;
				}
				else {
					if (errMsg && errMsg.length > 0) {
						errMsg += ', ';
					}
					errMsg += node.data.TagTagID;
				}
			}
		}
	}
	else {
		node = selectedObjects[0];
		FMDrawIndex.UpdatePointHistoryButtonSuccessHelper(canvas, pointValue, node);
	}

	FMDrawPropertyMenu.onlyAssociateTagDataToButton = false;
	FMDrawPropertyMenu.currentButtonActionAssociation = null;
	return errMsg;
};
FMDrawIndex.UpdatePointHistoryButtonSuccess = function (response) {
	var alertTitle = 'Point Error';

	if (response) {
		var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
		var selectedObjects = FMDrawIndex.GetSelectedObjects(canvas);
		var tagSelectionModel = response;
		var pointValues = tagSelectionModel.PointValues;
		var errMsg = '';
		errMsg = FMDrawIndex.UpdatePointHistoryButtonSuccessPrime(canvas, selectedObjects, pointValues, errMsg);

		if (errMsg.length > 0) {
			errMsg = 'Tag ID ' + errMsg;
			errMsg += ' not found on selected point!';
			FMLayout.Alert(errMsg, alertTitle, null);
		}
	}
};

FMDrawIndex.UpdateTagSuccessHelper = function ( canvas, tag, node )
{
	var transactionTag = 'attach modify point';
	var alertTitle = 'Input Error';
	switch ( node.data.category )
	{
		case 'bar':
			canvas.startTransaction( transactionTag );
			FMDrawIndex.BarTagChangeSuccess( tag, node );
			canvas.commitTransaction( transactionTag );
			break;
		case 'tag':
			canvas.startTransaction( transactionTag );
			FMDrawIndex.PointTagChangeSuccess( tag, node );
			canvas.commitTransaction( transactionTag );
			break;
		case 'button':
			canvas.startTransaction( transactionTag );
			FMDrawIndex.ButtonTagChangeSuccess( tag, node, false );
			canvas.commitTransaction( transactionTag );
			break;
		default:
			//Should nver get to this message.
			var errMsg = 'Invalid object type.( ' + node.data.category + ' ) Must be bar, button, or tag!';
			FMLayout.Alert( errMsg, alertTitle, null );
	}
};
FMDrawIndex.UpdateTagSuccessPrime = function (canvas, selectedObjects, pointValues, errMsg)
{
	var node, pointValue;
	if ( FMDrawPropertyMenu.MultiSelectionFlag )
	{
		for ( var nextObjIndex = 0; nextObjIndex < selectedObjects.length; nextObjIndex++ )
		{
			node = selectedObjects[nextObjIndex];

			if ( FMDrawPropertyMenu.isObjectGroup( node ) )
			{
				var arr = FMDrawPropertyMenu.IteratorToArray( node.memberParts );
				errMsg += FMDrawIndex.UpdateTagSuccessPrime( canvas, arr, pointValues, errMsg );
			}
			else
			{
				pointValue = FMDrawIndex.FindTagsWithSameId( pointValues, node );

				if (pointValue)
				{
					FMDrawIndex.UpdateTagSuccessHelper(canvas, pointValue, node);
					FMDrawPropertyMenu.manualSetPointIDFlag = true;
				}
				else
				{
					if ( errMsg && errMsg.length > 0 )
					{
						errMsg += ', ';
					}
					errMsg += node.data.TagTagID;
				}
			}
		}
	}
	else
	{
	    node = selectedObjects[0];
	    pointValue = pointValues[0];
	    FMDrawIndex.UpdateTagSuccessHelper(canvas, pointValue, node);
	}
	FMDrawPropertyMenu.onlyAssociateTagDataToButton = false;
	FMDrawPropertyMenu.currentButtonActionAssociation = null;
	return errMsg;
};
FMDrawIndex.UpdateTagSuccess = function( response )
{
	var alertTitle = 'Tag Error';
	if ( response && response !== "Cancel" )
	{
		var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
		var selectedObjects = FMDrawIndex.GetSelectedObjects( canvas );
		var tagSelectionModel = response;
		var pointValues = tagSelectionModel.PointValues;
		var errMsg = '';
		errMsg = FMDrawIndex.UpdateTagSuccessPrime( canvas, selectedObjects, pointValues, errMsg );

		if ( errMsg.length > 0 )
		{
			errMsg = 'Tag ID ' + errMsg;
			errMsg += ' not found on selected point!';
			FMLayout.Alert( errMsg, alertTitle, null );
		}
	}
	else
	{
		FMDrawIndex.TagSelectionCancel();
	}
};
FMDrawIndex.FindTagsWithSameId = function( pointValues, node )
{
	for ( var i = 0; i < pointValues.length; i++ )
	{
		if ( node.data.TagTagID === pointValues[i].ID )
		{
				return pointValues[i];
		}
	}
	return undefined;
};
FMDrawIndex.AddButtonFromDragAndDrop = function( startPos, event, ui )
{
	if ( !FMDrawIndex.GetActiveTabGoJSDiagramObject() )
	{
		return;
	}

	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 10 )
	{
		FMDrawIndex.DrawingMode( 'button' );
	}
	// ReSharper disable once UnusedLocals
	var layerManager = new FMDrawIndex._LayerManager();
	if ( FMDrawIndex.DistanceBetweenTwoPoints( startPos, ui.position ) <= 50 )
	{
		return;
	}
	FMDrawIndex.AddShapeFromDragAndDrop( {
		category: 'button',
		size: '150 100',
		layerName: layerManager.GetPrimaryLayerName(),
		color: go.GraphObject.make( go.Brush, 'Linear', { 0: 'white', 1: 'lightgray' } ),
		lineStroke: 'gray'
	}, event);
	FMDrawPropertyMenu.OpenPropertiesPopupMenu( [ButtonActionTargetTextBoxID,ButtonActionTypeDropDownID] );
};

FMDrawIndex.CreateParameterDataForTagDialogSwitch = function (showValueTypesParam,
    showTagsParam,
    showFieldsParam,
    allowMultipleParam,
    allowPointParam,
    pointIdParam,
    pointGuidParam,
    tagIdParam,
    pointValueIdentifierParam,
    panelTypeStrParam,
    isPointDetailDrawing,
    isPointDetailObjectParam,
    isPointTrendButton,
	 applyPointAccess
    )
{
    var params = {
        showValueTypes: showValueTypesParam,
        showTags: showTagsParam,
        showFields: showFieldsParam,
        allowMultiple: allowMultipleParam,
        allowPoint: allowPointParam,
        pointId: pointIdParam,
        pointGuidStr: pointGuidParam,
        valueId: tagIdParam,
        pointValueIdentifier: pointValueIdentifierParam,
		  applyPointAccess: applyPointAccess
    }
    if ( isPointDetailDrawing )
    {
        params.panelTypeStr = ( panelTypeStrParam ) ? panelTypeStrParam : 'Standard';
        params.pointTemplateGuidStr = (pointGuidParam) ? pointGuidParam : '';
        params.isPointDetailObject = (isPointDetailObjectParam) ? true : false;
        params.isPointTrendButton = ( isPointTrendButton ) ? true : false;
    }

    return params;
}


FMDrawIndex.OpenTagDialogForSwitch = function( showTags, showFields, callbackFunction, pointId, pointGuid, tagId, tagGuid, tagPointValueType, tagPropertyID, isPointDetailObject, isPointTrendButton )
{
	TagSelection.TagSelectionOKCallBackFunction = callbackFunction;
	TagSelection.TagSelectionSaveCallBackFunction = undefined;
	FMDrawIndex.switchingTags = true;
    var panelType = FMDrawIndex.GetDiagramModelDataValue( 'PanelType' );
    var isPointDetailDrawing = (panelType === 'Detail');
    var applyPointAccess = false;
	// hide any other notification
	FMErrorAndExceptionHandling.CloseNotifications();

	$('body').modalmanager('loading');

	var url = (isPointDetailDrawing) ? $('#urlTagSelectionGetPointListWithPanelTemplateContextEx').val(): $('#urlTagSelectionGetPointListEx').val();
	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	var pointValueIdentifier = { IdentityGuid: tagGuid, PointValueType: tagPointValueType, PropertyID: tagPropertyID };

	var params = FMDrawIndex.CreateParameterDataForTagDialogSwitch(
			true,
			showTags,
			showFields,
			false,
			true,
			pointId,
			pointGuid,
			tagId,
			pointValueIdentifier,
			panelType,
			isPointDetailDrawing,
			isPointDetailObject,
			isPointTrendButton,
			applyPointAccess
    );

	$.ajax( {
		type: 'POST',
		url: url,
		headers: headers,
		cache: false,
		data: params,
		success: function( response )
		{
				FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
				{
					if ( !inError )
					{
						// replace the holder with the partial view
						$( '#PointSelection' ).html( data );
						$( '#PointTagSelectScreen' ).modal( 'show' );
					}
					else
					{
						// remove the loading of the modal
						var modalManager = $( 'body' ).data( 'modalmanager' );
						modalManager.removeLoading();
					}
				} );
		},
		error: function( xhr, textStatus, error )
		{
				FMDrawPropertyMenu.onlyAssociateTagDataToButton = false;
				FMErrorAndExceptionHandling.ShowException( xhr,
					textStatus,
					error,
					function()
					{
						// remove the loading of the modal
						var modalManager = $( 'body' ).data( 'modalmanager' );
						modalManager.removeLoading();
					} );
		}
	} );
};

FMDrawIndex.CreateParameterDataForOpenTagDialog = function (showFieldsParam, modelParam, panelTypeStrParam, pointTemplateGuidStrParam, isPointDetailDrawing)
{
    var modelStrParam = (modelParam) ? JSON.stringify( modelParam ) : '';
    if (isPointDetailDrawing) {
        return {
            showFields: showFieldsParam,
            modelStr: modelStrParam,
            panelTypeStr: panelTypeStrParam,
            pointTemplateGuidStr: pointTemplateGuidStrParam,
        }
    }
    else {
        return {
            showFields: showFieldsParam,
            modelStr: modelStrParam
        }
    }
}

FMDrawIndex.FinishOpenTagSelectionDialog = function (success)
{
	if (success)
	{
		$('#PointTagSelectScreen').modal('show');
		document.getElementById('PointTagSelectScreen').focus();
	}
}

FMDrawIndex.OpenTagDialog = function( showFields, callbackFunction )
{
	TagSelection.TagSelectionOKCallBackFunction = callbackFunction;
	TagSelection.TagSelectionSaveCallBackFunction = FMDrawIndex.SaveLastTagSelectionModel;
	FMDrawIndex.switchingTags = false;
	var panelType = FMDrawIndex.GetDiagramModelDataValue('PanelType');
	var isPointDetail = (panelType === 'Detail');
	var url = (isPointDetail) ? $('#urlTagSelectionGetPointListWithPanelTemplateContext').val() : $('#urlTagSelectionGetPointList').val();
	var pointTemplateGuidStr = FMDrawIndex.GetDiagramModelDataValue('PointTemplateGuid');
	// hide any other notification
	FMErrorAndExceptionHandling.CloseNotifications();

	$( 'body' ).modalmanager( 'loading' );
	var model = FMDrawIndex.GetLastTagSelectionModel();

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
    var params = FMDrawIndex.CreateParameterDataForOpenTagDialog( showFields, model, panelType, pointTemplateGuidStr, isPointDetail );

	$.ajax( {
		type: 'POST',
		url: url,
		headers: headers,
		cache: false,
		data: params,
		success: function( response )
		{
				FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
				{
					if ( !inError )
					{
						// replace the holder with the partial view
						$( '#PointSelection' ).html( data );
						$('#PointTagSelectScreen').modal('show');
					}
					else
					{
						// remove the loading of the modal
						var modalManager = $( 'body' ).data( 'modalmanager' );
						modalManager.removeLoading();
					}
				} );
		},
		error: function( xhr, textStatus, error )
		{
				FMErrorAndExceptionHandling.ShowException( xhr,
					textStatus,
					error,
					function()
					{
						// remove the loading of the modal
						var modalManager = $( 'body' ).data( 'modalmanager' );
						modalManager.removeLoading();
					} );
		}
	} );
};
FMDrawIndex.AddTagFromDragAndDrop = function( startPos, event , ui)
{
	if ( !FMDrawIndex.GetActiveTabGoJSDiagramObject() )
	{
		return;
	}

	if (FMDrawIndex.DistanceBetweenTwoPoints(startPos, ui.position) <= 10) {
		FMDrawIndex.DrawingMode('tag');
	}

	if (FMDrawIndex.DistanceBetweenTwoPoints(startPos, ui.position) <= 50) {
		return;
	}

	var layerManager = new FMDrawIndex._LayerManager();
	var primaryLayerName = layerManager.GetPrimaryLayerName();
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	diagram.startTransaction( FMDrawIndex.transactionTag );
	var data = JSON.parse( JSON.stringify( FMDrawIndex.defaultArchetype ) ); //copying default node data to get current default properties
	data.category = 'tag';
	data.layerName = primaryLayerName;
	data.zOrder = FMDrawIndex.GetNextPartZOrder( primaryLayerName );
	data.size = '300 30';
	if ( FMDrawIndex.defaultArchetype.alignment )
	{
		data.alignment = FMDrawIndex.defaultArchetype.alignment.copy();
	}
	if ( FMDrawIndex.defaultArchetype.color )
	{
		if ( typeof FMDrawIndex.defaultArchetype.color === 'object' )
		{
				data.color = FMDrawIndex.defaultArchetype.color.copy();
		}
	}
	FMDrawIndex.AddTagsFromDragAndDrop( data, event );


	FMDrawIndex.OpenTagDialog( true, FMDrawIndex.PointTagCallSuccess );
};
FMDrawIndex.GetTagFormatNumber = function( max, min, decimalPlaces )
{
	var maxString = Math.floor( max ).toString();
	var minString = Math.floor( min ).toString();
	if ( decimalPlaces > 0 )
	{
		var decimalString = '.';
		for ( var i = 0; i < decimalPlaces; i++ )
		{
				decimalString += '0';
		}
		maxString += decimalString;
		minString += decimalString;
	}
	if ( minString.length > maxString.length )
	{
		return minString;
	}
	return maxString;
};
FMDrawIndex.GetSiteNumFormatInfoString = function()
{
	var siteNumFormatInfoString = document.getElementById( 'SiteNumFormatInfo' ).value;
	var siteNumFormatInfo = JSON.parse( siteNumFormatInfoString );
	return siteNumFormatInfo;
};
FMDrawIndex.GetLastTagSelectionModelStr = function()
{
    var tab = FMDrawIndex.GetActiveTabCanvasContainerObject();
    return ( tab && tab.TagSelectionModel ) ? JSON.stringify(tab.TagSelectionModel) : '';
};

FMDrawIndex.GetLastTagSelectionModel = function ()
{
    var tab = FMDrawIndex.GetActiveTabCanvasContainerObject();
    return (tab && tab.TagSelectionModel) ? tab.TagSelectionModel: null;
    };

FMDrawIndex.SaveLastTagSelectionModel = function( tagSelectionModel )
{
    var tab = FMDrawIndex.GetActiveTabCanvasContainerObject();
    if ( tab && tagSelectionModel )
    {
        //Create a Clone incase the tagSelectionModel is used by the calling method
        tab.TagSelectionModel = JSON.parse(JSON.stringify(tagSelectionModel));
        //Clear the Point Values
        tab.TagSelectionModel.PointValues = [];
    }
};
FMDrawIndex.GetDateTimeFormatInfo = function()
{
	var dateTimeFormatInfoString = document.getElementById( 'DateTimeFormatInfo' ).value;
	var dateTimeFormatInfo = JSON.parse( dateTimeFormatInfoString );
	return dateTimeFormatInfo;
};
FMDrawIndex.GetTagUnitToUnitTypeList = function()
{
	var tagUnitToUnitTypeListString = document.getElementById( 'TagUnitToUnitTypeList' ).value;
	var tagUnitToUnitTypeList = JSON.parse( tagUnitToUnitTypeListString );
	return tagUnitToUnitTypeList;
};
FMDrawIndex.UpdateTagFormat = function( dataModel, node )
{
	var activeData = node;
	if ( !node )
	{
		activeData = FMDrawPropertyMenu.PropertyActiveObject.data;
	}
	var formatString = '';
	var tagDataType = activeData.TagDataType;
	switch ( activeData.TagFieldSelection )
	{
		case FMTAGFIELDSELECTION.ID:
		case FMTAGFIELDSELECTION.UNITS:
	case FMTAGFIELDSELECTION.ALARMSTATUS:
		tagDataType = 'System.String';
		break;
	case FMTAGFIELDSELECTION.TIMESTAMP:
		tagDataType = 'System.DateTimeOffset';
		break;
}

	var width = activeData.TagFieldWidth;

	if (tagDataType === 'System.Double' || tagDataType === 'System.Single')
	{
		var precision = activeData.TagPrecision;
		var tagUnits = FMDrawIndex.RetrieveTagUnits(activeData);
		if (tagUnits === FMENGINEERINGUNIT.FML_FtIn16th || tagUnits === FMENGINEERINGUNIT.FML_FtIn8th)
		{
			var minWidth = tagUnits === FMENGINEERINGUNIT.FML_FtIn16th ? 6 : 5;
			var adjWidth = width - minWidth;
			formatString = FMDrawIndex.CreateFormatStringForNumber(adjWidth, 0);
		}
		else
		{
			formatString = FMDrawIndex.CreateFormatStringForNumber(width, precision);
		}
		var siteNumFormatInfo = FMDrawIndex.GetSiteNumFormatInfoString();
		siteNumFormatInfo.NumberDecimalDigits = activeData.TagPrecision;
		formatString = FMFormatValues.FormatValue(tagUnits, siteNumFormatInfo, parseFloat(formatString)).toString();
	}

	else if (tagDataType === 'System.Boolean')
	{
		formatString = 'False';
		formatString = formatString.substring(0, width);
	}

	else if (tagDataType === 'System.String')
	{
		formatString = FMDrawIndex.CreateFormatStringForString(width);
	}

	else if (tagDataType === 'System.DateTime')
	{
		var dateTimeFormatInfo = FMDrawIndex.GetDateTimeFormatInfo();
		var s = '11-22-1976 00:00:00 -0500';
		var d = moment.parseZone(s);
		formatString = FMFormatValues.FormatDateString(d, dateTimeFormatInfo);
		formatString = formatString.substring(0, width);
	}


	else if (tagDataType === 'System.DateTimeOffset')
	{
		var dateTimeFormatInfo = FMDrawIndex.GetDateTimeFormatInfo();
		var s = '11-22-1976 10:04:03 -0500';
		var d = moment.parseZone(s);
		formatString = FMFormatValues.FormatDateTimeString(d, dateTimeFormatInfo);
		formatString = formatString.substring(0, width);
	}

    else if (tagDataType !== undefined && tagDataType.indexOf('FMBusinessObject.DataObjects.CodedVariables'))
	{
		formatString = FMDrawIndex.CreateFormatStringForString(width);
	}

	dataModel.setDataProperty(activeData, 'TagFormat', formatString);
	if (activeData.TagFieldSelection === FMTAGFIELDSELECTION.ALARMSTATUS)
	{
		dataModel.setDataProperty(activeData, 'TagAlarmAnnunciation', true);
	}
};
FMDrawIndex.UpdateUnitsDropDown = function (id, unitType)
{
	if (!unitType || !id || typeof id !== 'string')
	{
		return;
	}
	var unitTypeAsString = unitType.toString();
	var units = FMDrawIndex.GetUnitsByUnitsType(unitTypeAsString);
	if (!units)
	{
		return;
	}

	//Clear Option Box
	$('#' + id + ' option').remove();

	//Always add FM_SiteUnits
	$('#' + id)
		.append($('<option></option>')
				.attr('value', '0')
				.attr('data-value', 'FM_SiteUnits')
				.text('{Tag Units}'));

	//Populate with New units
	units.each(function (unit)
	{
		$('#' + id)
				.append($('<option></option>')
					.attr('value', unit.Unit.toString())
					.attr('data-value', unit.UnitStr)
					//.text(unit.UnitStr));  //We should consider using the unit.UnitDescription
					.text(unit.UnitAbbreviation));
	});
};
FMDrawIndex.ConvertTagUnitToUnitTypeListToMap = function ()
{
	if (!FMDrawIndex.unitTypeToUnitMap)
	{
		FMDrawIndex.unitTypeToUnitMap = new go.Map('int', go.Set);
	}
	var a = FMDrawIndex.unitTypeToUnitMap;

	FMDrawIndex.GetTagUnitToUnitTypeList().forEach(function (o)
	{
		var units;
		if (!a.has(o.UnitType.toString()))
		{
			units = new go.Set;
			units.add(o);
			a.set(o.UnitType.toString(), units);
		}
		else
		{
			units = a.get(o.UnitType.toString());
			units.add(o);
		}
	});
};
FMDrawIndex.GetUnitsByUnitsType = function (unitTypeAsString)
{
	if (!FMDrawIndex.unitTypeToUnitMap || typeof unitTypeAsString != 'string')
	{
		return null;
	}
	var a = FMDrawIndex.unitTypeToUnitMap;
	if (!a.has(unitTypeAsString))
	{
		return null;
	}
	return a.getValue(unitTypeAsString);
};
FMDrawIndex.CreateFormatStringForNumber = function (fieldWidth, precision)
{
	var formatString = '';
	var i;
	var j;
	if (precision > 0)
	{
		for (i = 0; i < fieldWidth - precision - 1; i++)
		{
			formatString += '9';
		}
		formatString += '.';
		for (j = 0; j < precision; j++)
		{
			formatString += '9';
		}
	}
	else
	{
		for (i = 0; i < fieldWidth; i++)
		{
			formatString += '9';
		}
	}
	return formatString;
};
FMDrawIndex.CreateFormatStringForString = function (fieldWidth)
{
	var formatString = '';
	for (var i = 0; i < fieldWidth; i++)
	{
		formatString += 'A';
	}
	return formatString;
};
FMDrawIndex.AddTagsFromDragAndDrop = function (shapeType, event, noTranslate)
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if (typeof diagram === 'undefined' || diagram === null)
	{
		return null;
	}
	if (shapeType === null || typeof shapeType === 'undefined')
	{
		return null;
	}
	if (!event)
	{
		return null;
	}

	var point;
	if (event instanceof go.DiagramEvent && event.name === 'BackgroundSingleClicked')
	{
		point = event.diagram.lastInput.documentPoint.copy();
	}
	else
	{
		if (noTranslate)
		{
			point = new go.Point(event.clientX, event.clientY);
		}
		else
		{
			var pixelratio = diagram.computePixelRatio();
			var canvas = $('#' + diagram.div.id + ' > canvas');
			var bbox = canvas.get(0).getBoundingClientRect();
			var bbw = bbox.width;
			if (bbw === 0)
			{
				bbw = 0.001;
			}
			var bbh = bbox.height;
			if (bbh === 0)
			{
				bbh = 0.001;
			}
			var mx = event.clientX - bbox.left * ((canvas.width() / pixelratio) / bbw);
			var my = event.clientY - bbox.top * ((canvas.height() / pixelratio) / bbh);
			point = diagram.transformViewToDoc(new go.Point(mx, my));
		}
	}


	var part = null;

	var data = diagram.model.copyNodeData(shapeType);
	if (data)
	{
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

	    }
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

	if (part !== null && typeof part !== 'undefined')
	{
		part.position = point;
		part.resizeObject.size = go.Size.parse(shapeType.size);
		if (diagram.allowSelect)
		{
		    diagram.select(part); // raises ChangingSelection/Finished

		    if (SetColorObject === true && data.color !== null && typeof (data.color) === 'object') {

		        var patternNumber = parseInt(data.patternImageName);
		        var patternTagId = 'canvasPalatte-propertiesMenu-FILLPATTERNPALETTE-' + data.patternImageName;
		        FMDrawPropertyMenu.HandleFillPatternOnClick(patternTagId, patternNumber, false, -99, true);
		    }
        }
	}

	// set the TransactionResult before raising event, in case it changes the result or cancels the tool

	return part;
};
FMDrawIndex.TagSelectionCancel = function ()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if (FMDrawIndex.switchingTags === false)
	{
		if (FMDrawIndex.IsATagSelected())
		{
			diagram.commitTransaction(FMDrawIndex.transactionTag);
			diagram.commandHandler.undo();
		}
		if (FMDrawIndex.IsABarSelected())
		{
			diagram.commitTransaction(FMDrawIndex.transactionBar);
			diagram.commandHandler.undo();
		}
	}
};
FMDrawIndex.ButtonTagChangeSuccess = function (pointValue, button, pointOnly)
{
    button.diagram.model.setDataProperty(button.data, 'PointTemplateTagSelectionIndicator', pointValue.IsPointTemplateValue);
    if (pointValue.PointValueIdentifier)
    {
        button.diagram.model.setDataProperty(button.data, 'TagGUID', pointValue.PointValueIdentifier.IdentityGuid);
        button.diagram.model.setDataProperty(button.data, 'TagPointValueType', pointValue.PointValueIdentifier.PointValueType);
        button.diagram.model.setDataProperty(button.data, 'TagPropertyID', pointValue.PointValueIdentifier.PropertyID);
    }
    if (pointValue.PointGuid)
        button.diagram.model.setDataProperty(button.data, 'PointGUID', pointValue.PointGuid);
    if (pointValue.PointID)
        button.diagram.model.setDataProperty(button.data, 'TagPointID', pointValue.PointID);
    if (pointValue.ID)
        button.diagram.model.setDataProperty(button.data, 'TagTagID', pointValue.ID);
	if (pointOnly)
	{
		button.diagram.model.setDataProperty(button.data, 'TagPointIDAndTagID', pointValue.PointID);
		FMDrawPropertyMenu.SetButtonActionTypeConfiguration(button, button.data.PointGUID, button.data.TagPointIDAndTagID, false, false);
	}
	else
	{
		button.diagram.model.setDataProperty(button.data, 'TagPointIDAndTagID', pointValue.PointID + ' : ' + pointValue.ID);
		FMDrawPropertyMenu.SetButtonActionTypeConfiguration(button, button.data.TagGUID, button.data.TagPointIDAndTagID, false, false);
	}


};
FMDrawIndex.PointTagCallSuccess = function (response)
{
    if (response && response !== null && response != "Cancel")
	{
		var tagSelectionModel = response;
		var pointValues = tagSelectionModel.PointValues;
		var fields = tagSelectionModel.Fields;
		var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();

		if (typeof diagram === 'undefined' || diagram === null ||
            typeof pointValues === 'undefined' || pointValues === null)
		{
			return;
		}
		var selectedObjects = FMDrawIndex.GetSelectedObjects(diagram);
		var slctOjbect = selectedObjects[0];
		var obj = slctOjbect;
		var lSz = go.Size.parse(obj.data.size);
		var tX = obj.part.position.x, tY = obj.part.position.y, tWidth = lSz.width, tHeight = lSz.height;
		var layerManager = new FMDrawIndex._LayerManager();
		var primaryLayerName = layerManager.GetPrimaryLayerName();

		for (var valueIndex = 0; valueIndex < pointValues.length; ++valueIndex)
		{
			var pointValue = pointValues[valueIndex];

			for (var fieldIndex = 0; fieldIndex < fields.length; ++fieldIndex)
			{
				var field = fields[fieldIndex];
				if (field === FMTAGFIELDSELECTION.ALARMSTATUS && pointValue.HasAlarm === false)
				{
					field = FMTAGFIELDSELECTION.VALUE;
				}

				if ((valueIndex > 0 || fieldIndex > 0) && !FMDrawPropertyMenu.onlyAssociateTagDataToButton)
				{
					var event = { clientX: tX, clientY: tY + (tHeight + 10) * (valueIndex * fields.length + fieldIndex) };
					var copy = JSON.parse(JSON.stringify(FMDrawIndex.defaultArchetype)); //copying default node data to get current default properties
					copy.category = 'tag';
					copy.layerName = primaryLayerName;
					copy.zOrder = FMDrawIndex.GetNextPartZOrder(primaryLayerName);
					if (FMDrawIndex.defaultArchetype.alignment)
					{
						copy.alignment = FMDrawIndex.defaultArchetype.alignment.copy();
					}
					if (FMDrawIndex.defaultArchetype.color)
					{
						if (typeof FMDrawIndex.defaultArchetype.color === 'object')
						{
							copy.color = FMDrawIndex.defaultArchetype.color.copy();
						}
					}
					obj = FMDrawIndex.AddTagsFromDragAndDrop(copy, event, true);
					obj.resizeObject.desiredSize = new go.Size(tWidth, tHeight);
				}

				var formatString;
				var fieldWidth;
				var precision = 0;
				var tagDataType = pointValue.ValueTypeString;

				switch (field)
				{
					case FMTAGFIELDSELECTION.ID:
					case FMTAGFIELDSELECTION.UNITS:
					case FMTAGFIELDSELECTION.ALARMSTATUS:
						tagDataType = 'System.String';
						break;
					case FMTAGFIELDSELECTION.TIMESTAMP:
						tagDataType = 'System.DateTimeOffset';
						break;
				}

				if (tagDataType === 'System.Double' || tagDataType === 'System.Single')
				{
					formatString = FMDrawIndex.GetTagFormatNumber(pointValue.Maximum, pointValue.Minimum, pointValue.DecimalPlaces);
					formatString = FMDrawIndex.CreateFormatStringForNumber(formatString.length, pointValue.DecimalPlaces);
					fieldWidth = formatString.length;
					var siteNumFormatInfo = FMDrawIndex.GetSiteNumFormatInfoString();
					siteNumFormatInfo.NumberDecimalDigits = pointValue.DecimalPlaces;
					formatString = FMFormatValues.FormatValue(pointValue.Units, siteNumFormatInfo, parseFloat(formatString)).toString();
					if (pointValue.Units === FMENGINEERINGUNIT.FML_FtIn16th || pointValue.Units === FMENGINEERINGUNIT.FML_FtIn8th)
					{
						fieldWidth = formatString.length;
					}
					precision = pointValue.DecimalPlaces;
				}

				else if (tagDataType === 'System.Boolean')
				{
					formatString = 'False';
					fieldWidth = formatString.length;
				}

				else if (tagDataType === 'System.String')
				{
					formatString = FMDrawIndex.CreateFormatStringForString(30);
					fieldWidth = formatString.length;
				}

				else if (tagDataType === 'System.DateTime')
				{
					var dateTimeFormatInfo = FMDrawIndex.GetDateTimeFormatInfo();
					var s = '11-22-1976 00:00:00 -0500';
					var d = moment.parseZone(s);
					formatString = FMFormatValues.FormatDateString(d, dateTimeFormatInfo);
					fieldWidth = formatString.length;
				}

				else if (tagDataType === 'System.DateTimeOffset')
				{
					var dateTimeFormatInfo = FMDrawIndex.GetDateTimeFormatInfo();
					var s = '11-22-1976 10:04:03 -0500';
					var d = moment.parseZone(s);
					formatString = FMFormatValues.FormatDateTimeString(d, dateTimeFormatInfo);
					fieldWidth = formatString.length;
				}

				else if (tagDataType.indexOf('FMBusinessObject.DataObjects.CodedVariables'))
				{
					formatString = FMDrawIndex.CreateFormatStringForString(30);
					fieldWidth = formatString.length;
				}

				obj.diagram.model.setDataProperty(obj.data, 'PointTemplateTagSelectionIndicator', tagSelectionModel.PointTemplateTagSelectionIndicator);
				obj.diagram.model.setDataProperty(obj.data, 'TagGUID', pointValue.PointValueIdentifier.IdentityGuid);
				obj.diagram.model.setDataProperty(obj.data, 'TagPointValueType', pointValue.PointValueIdentifier.PointValueType);
				obj.diagram.model.setDataProperty(obj.data, 'TagPropertyID', pointValue.PointValueIdentifier.PropertyID);
				obj.diagram.model.setDataProperty(obj.data, 'PointGUID', pointValue.PointGuid);
				obj.diagram.model.setDataProperty(obj.data, 'TagPointID', pointValue.PointID);
				obj.diagram.model.setDataProperty(obj.data, 'TagTagID', pointValue.ID);
				obj.diagram.model.setDataProperty(obj.data, 'TagPointIDAndTagID', pointValue.PointID + ' : ' + pointValue.ID);
				obj.diagram.model.setDataProperty(obj.data, 'TagUnitsOriginal', pointValue.Units);
				obj.diagram.model.setDataProperty(obj.data, 'TagUnits', 0);
				obj.diagram.model.setDataProperty(obj.data, 'TagUnitType', pointValue.EngineeringUnitsType);
				obj.diagram.model.setDataProperty(obj.data, 'TagFormat', formatString);
				obj.diagram.model.setDataProperty(obj.data, 'TagPrecision', precision);
				obj.diagram.model.setDataProperty(obj.data, 'TagFieldWidth', fieldWidth);
				obj.diagram.model.setDataProperty(obj.data, 'TagFieldSelection', field);
				obj.diagram.model.setDataProperty(obj.data, 'TagValue', '');
				obj.diagram.model.setDataProperty(obj.data, 'TagDataType', pointValue.ValueTypeString);
				obj.diagram.model.setDataProperty(obj.data, 'TagShowStatus', pointValue.PointValueIdentifier.PointValueType === 0 && field === FMTAGFIELDSELECTION.VALUE);
				obj.diagram.model.setDataProperty(obj.data, 'TagShowWeightsAndMeasures', false);
				obj.diagram.model.setDataProperty(obj.data, 'TagStatus', field === FMTAGFIELDSELECTION.VALUE ? 'FRC' : '');
				obj.diagram.model.setDataProperty(obj.data, 'TagWeightsAndMeasures', '');
				var halfSizeFont = FMDrawPropertyMenu.CreateHalfSizedFont(obj.data.font);
				obj.diagram.model.setDataProperty(obj.data, 'SuperScriptFont', halfSizeFont);
				obj.diagram.model.setDataProperty(obj.data, 'SubScriptFont', halfSizeFont);
				obj.diagram.model.setDataProperty(obj.data, 'ToolTipString', pointValue.PointID + ' : ' + pointValue.ID + ' : ' + FMTAGFIELDSELECTION.GetFieldString(field));
				if (field === FMTAGFIELDSELECTION.ALARMSTATUS)
				{
					obj.diagram.model.setDataProperty(obj.data, 'TagAlarmAnnunciation', true);
				}
				else
				{
					obj.diagram.model.setDataProperty(obj.data, 'TagAlarmAnnunciation', false);
				}
				obj.diagram.model.setDataProperty(obj.data, 'TagAlarmAnunciationHasAlarm', pointValue.HasAlarm);

				if (FMDrawPropertyMenu.onlyAssociateTagDataToButton && obj.data.TagGUID)
				{
					FMDrawPropertyMenu.SetButtonActionTypeConfiguration(obj, obj.data.TagGUID, obj.data.TagPointIDAndTagID, true, false);
				}

				FMDrawPropertyMenu.onlyAssociateTagDataToButton = false;
			}
		}
		diagram.commitTransaction(FMDrawIndex.transactionTag);
	}
	else
	{
		FMDrawIndex.TagSelectionCancel();
	}
};
FMDrawIndex.PointTagChangeSuccess = function (pointValue, data)
{
	var field = data.data.TagFieldSelection;
	var formatString;
	var fieldWidth;
	var precision = 0;
	var tagDataType = pointValue.ValueTypeString;

	if (field === FMTAGFIELDSELECTION.ALARMSTATUS && pointValue.HasAlarm === false)
	{
		field = FMTAGFIELDSELECTION.VALUE;
	}

	switch (field)
	{
		case FMTAGFIELDSELECTION.ID:
		case FMTAGFIELDSELECTION.UNITS:
		case FMTAGFIELDSELECTION.ALARMSTATUS:
			tagDataType = 'System.String';
			break;
		case FMTAGFIELDSELECTION.TIMESTAMP:
			tagDataType = 'System.DateTimeOffset';
			break;
	}

	if (tagDataType === 'System.Double' || tagDataType === 'System.Single')
	{
		formatString = FMDrawIndex.GetTagFormatNumber(pointValue.Maximum, pointValue.Minimum, pointValue.DecimalPlaces);
		formatString = FMDrawIndex.CreateFormatStringForNumber(formatString.length, pointValue.DecimalPlaces);
		fieldWidth = formatString.length;
		var siteNumFormatInfo = FMDrawIndex.GetSiteNumFormatInfoString();
		siteNumFormatInfo.NumberDecimalDigits = pointValue.DecimalPlaces;
		formatString = FMFormatValues.FormatValue(pointValue.Units, siteNumFormatInfo, parseFloat(formatString)).toString();

		if (pointValue.Units === FMENGINEERINGUNIT.FML_FtIn16th || pointValue.Units === FMENGINEERINGUNIT.FML_FtIn8th)
		{
			fieldWidth = formatString.length;
		}

		precision = pointValue.DecimalPlaces;
	}

	else if (tagDataType === 'System.Boolean')
	{
		formatString = 'False';
		fieldWidth = formatString.length;
	}

	else if (tagDataType === 'System.String')
	{
		formatString = FMDrawIndex.CreateFormatStringForString(30);
		fieldWidth = formatString.length;
	}

	else if (tagDataType === 'System.DateTime')
	{
		var dateTimeFormatInfo = FMDrawIndex.GetDateTimeFormatInfo();
		var s = '11-22-1976 00:00:00 -0500';
		var d = moment.parseZone(s);
		formatString = FMFormatValues.FormatDateString(d, dateTimeFormatInfo);
		fieldWidth = formatString.length;
	}

	else if (tagDataType === 'System.DateTimeOffset')
	{
		var dateTimeFormatInfo = FMDrawIndex.GetDateTimeFormatInfo();
		var s = '11-22-1976 10:04:03 -0500';
		var d = moment.parseZone(s);
		formatString = FMFormatValues.FormatDateTimeString(d, dateTimeFormatInfo);
		fieldWidth = formatString.length;
	}

	else if (tagDataType.indexOf('FMBusinessObject.DataObjects.CodedVariables'))
	{
		formatString = FMDrawIndex.CreateFormatStringForString(30);
		fieldWidth = formatString.length;
	}

	data.diagram.model.setDataProperty(data.data, 'PointTemplateTagSelectionIndicator', pointValue.IsPointTemplateValue);
	data.diagram.model.setDataProperty(data.data, 'TagGUID', pointValue.PointValueIdentifier.IdentityGuid);
	data.diagram.model.setDataProperty(data.data, 'TagPointValueType', pointValue.PointValueIdentifier.PointValueType);
	data.diagram.model.setDataProperty(data.data, 'TagPropertyID', pointValue.PointValueIdentifier.PropertyID);
	data.diagram.model.setDataProperty(data.data, 'PointGUID', pointValue.PointGuid);
	data.diagram.model.setDataProperty(data.data, 'TagPointID', pointValue.PointID);
	data.diagram.model.setDataProperty(data.data, 'TagTagID', pointValue.ID);
	data.diagram.model.setDataProperty(data.data, 'TagPointIDAndTagID', pointValue.PointID + ' : ' + pointValue.ID);
	data.diagram.model.setDataProperty(data.data, 'ToolTipString', pointValue.PointID + ' : ' + pointValue.ID + ' : ' + FMTAGFIELDSELECTION.GetFieldString(field));
	data.diagram.model.setDataProperty(data.data, 'TagAlarmAnunciationHasAlarm', pointValue.HasAlarm);

	if (data.data.TagUnitsOriginal !== pointValue.Units || data.data.tagDataType !== pointValue.ValueTypeString)
	{
		data.diagram.model.setDataProperty(data.data, 'TagUnitsOriginal', pointValue.Units);
		data.diagram.model.setDataProperty(data.data, 'TagUnits', 0);
		data.diagram.model.setDataProperty(data.data, 'TagUnitType', pointValue.EngineeringUnitsType);
		data.diagram.model.setDataProperty(data.data, 'TagFormat', formatString);
		data.diagram.model.setDataProperty(data.data, 'TagPrecision', precision);
		data.diagram.model.setDataProperty(data.data, 'TagFieldWidth', fieldWidth);
		data.diagram.model.setDataProperty(data.data, 'TagFieldSelection', field);
		data.diagram.model.setDataProperty(data.data, 'TagDataType', pointValue.ValueTypeString);
	}
	if (field === FMTAGFIELDSELECTION.ALARMSTATUS)
	{
		data.diagram.model.setDataProperty(data.data, 'TagAlarmAnnunciation', true);
	}
};
FMDrawIndex.PointTagCallError = function( response )
{
	// TODO: Add error logging
	alert( 'Got error: ' + response.responseText );
	FMDrawPropertyMenu.onlyAssociateTagDataToButton = false;
};
FMDrawIndex.ReturnCanvasIndices = function()
{
	var tabarray = [];
	var activetab = '';
	$( 'div#tabs .ui-tabs-nav a' ).each( function()
	{
		activetab = $( this ).attr( 'href' );
		if ( typeof activetab !== 'undefined' )
		{
				activetab = activetab.substring( activetab.indexOf( 'b' ) + 1 );
				tabarray.push( activetab );
		}
	} );
	return tabarray; //returns an array of the active canvas indicies
};
FMDrawIndex.SetShapeDragged = function( dragged )
{
	FMDrawIndex.shapeDragged = dragged;
}; /*
 * 
 * CUT AND PASTE FUNCTIONS
 * 
 */
FMDrawIndex.Copy = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	diagram.commandHandler.copySelection();
};
FMDrawIndex.Cut = function()
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !canvas )
	{
		return;
	}
	canvas.commandHandler.cutSelection();
};
FMDrawIndex.Paste = function()
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !canvas )
	{
		return;
	}
	canvas.commandHandler.pasteSelection( canvas.lastInput.documentPoint );
};
FMDrawIndex.Duplicate = function()
{
    var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
    var canvasContainerObj = FMDrawIndex.GetActiveTabCanvasContainerObject();

    if ( !diagram ||
        !diagram.selection ||
        diagram.selection.count === 0 )
    {
        return;
    }

    if ( diagram.selection.count === 0 )
    {
        return;
    }

	diagram.startTransaction( 'duplicate' );

    if (canvasContainerObj.PerforCopyForDuplication)
    {
        diagram.commandHandler.copySelection();
        canvasContainerObj.PerforCopyForDuplication = false;
    }
    canvasContainerObj.SelectionChangeBasedOnPasteFromDuplication = true;
    diagram.commandHandler.pasteSelection(); 


    diagram.commitTransaction( 'duplicate' );
};
FMDrawIndex.Delete = function()
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !canvas )
	{
		return;
	}
	canvas.commandHandler.deleteSelection();
};
FMDrawIndex.Rotate = function (angle) {
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if (!canvas) {
		return;
	}
	canvas.startTransaction('rotate');
	var it = canvas.selection.iterator;
	while (it.next()) {
		if (it.value.data.category != 'lineLink')
		{
		var node = it.value;
		node.angle += angle;
		}
	}
	canvas.commitTransaction('rotate');
};
FMDrawIndex.ShiftZOrderToExtremity = function( index )
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	var layers = [];
	var layerFound = false;
	var nextZOrder = 0;
	var offset = 0;
	var i;
	var j;
	var selectedPartsArray = FMDrawIndex.OrderPartsListByLayerAndZOrder( diagram.selection, true );
	//Capture parts information for each layer involved in the parts selection
	var part = null;
	for ( i = 0; i < selectedPartsArray.length; i++ )
	{
		part = selectedPartsArray[i];
		layerFound = false;
		for ( j = 0; j < layers.length; j++ )
		{
				if ( layers[j].LayerName === part.LayerName )
				{
					layerFound = true;
					if ( layers[j].FirstSelPartZOrder > part.ZOrder )
					{
						layers[j].FirstSelPartZOrder = part.ZOrder;
					}
					else if ( layers[j].LastSelPartZOrder < part.ZOrder )
					{
						layers[j].LastSelPartZOrder = part.ZOrder;
					}
					layers[j].SelPartsCount++;
				}
		}
		if ( !layerFound )
		{
				nextZOrder = FMDrawIndex.GetNextPartZOrder( part.LayerName );
				layers.push( { "LayerName": part.LayerName, "IgnoreLayer": false, "PartBaseZOrder": nextZOrder, "FirstSelPartZOrder": part.ZOrder, "LastSelPartZOrder": part.ZOrder, "SelPartsCount": 1 } );
		}
	}

	//Retrieve layer-specific information and if it is a SendToBack request, push all parts on each layer up to make room at the low ZOrder end for the selected parts.
	diagram.model.startTransaction( 'Extremity zOrder Adjustment' );
	var layerPartsCount;
	var layer;
	part = null;
	var layerManager = new FMDrawIndex._LayerManager();
	var partsArray = [];

    for ( i = 0; i < layers.length; i++ )
    {
        layer = layerManager.GetLayerByName(layers[i].LayerName);
        if ( !layer )
            continue;
        layerPartsCount = layer.parts.count;
        if ( layers[i].SelPartsCount === layerPartsCount )
        {
            layers[i].IgnoreLayer = true;
            break;
        }
        if ( isNaN( index ) ) //SendToBack
        {
            offset = layers[i].SelPartsCount + layers[i].LastSelPartZOrder;
            layer.parts.each( function( p )
            {
                partsArray.push( p );
            } );
            for ( j = 0; j < partsArray.length; j++ )
            {
                part = partsArray[j];
                part.zOrder = part.zOrder + offset;
            }
        }
    }
    //Adjust the zOrder of the selected parts, while maintaining the relative zOrder difference between them.
    var baseZOrder = 0;
    var startZOrder = 0;
    var ignoreZOrderChangeRequest = false;
    var targetPart = null;
    for ( i = 0; i < selectedPartsArray.length; i++ )
    {
        targetPart = selectedPartsArray[i];
        ignoreZOrderChangeRequest = false;
        baseZOrder = 1; //SendToBack
        offset = 0; //BringToFront;
        for ( j = 0; j < layers.length; j++ )
        {
            if ( layers[j].LayerName === targetPart.LayerName )
            {
                if ( layers[j].IgnoreLayer === true )
                {
                    ignoreZOrderChangeRequest = true;
                    break;
                }
                startZOrder = layers[j].FirstSelPartZOrder;
                if ( index === Number.MAX_VALUE ) //BringToFront
                {
                    baseZOrder = layers[j].PartBaseZOrder;
                }
                else //SendToBack - The offset is only applicable to non-selected parts and has to be cancelled out for selected parts
                {
                    offset = layers[j].SelPartsCount + layers[j].LastSelPartZOrder;
                }
                break;
            }
        }
        if ( !ignoreZOrderChangeRequest )
        {
            targetPart.ZOrder = baseZOrder - offset + ( targetPart.ZOrder - startZOrder );
            diagram.model.setDataProperty( targetPart.PartData, 'zOrder', targetPart.ZOrder );
        }
    }
    diagram.model.commitTransaction( 'Extremity zOrder Adjustment' );
};
FMDrawIndex.ShiftZOrderBySingleStep = function( isBackwardStep )
{
    if ( typeof ( isBackwardStep ) === 'undefined' )
    {
        return;
    }
    var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
    if ( !diagram )
    {
        return;
    }
    var layers = [];
    var layerFound = false;
    var selectedPartsArray = FMDrawIndex.OrderPartsListByLayerAndZOrder( diagram.selection, isBackwardStep );
    //Capture parts information for each layer involved in the parts selection
    var part = null;
    var i;
    var j;
    for ( i = 0; i < selectedPartsArray.length; i++ )
    {
        part = selectedPartsArray[i];
        layerFound = false;
        for ( j = 0; j < layers.length; j++ )
        {
            if ( layers[j].LayerName === part.LayerName )
            {
                layerFound = true;
                layers[j].SelPartsCount++;
            }
        }
        if ( !layerFound )
        {
            layers.push( { "LayerName": part.LayerName, "IgnoreLayer": false, "SelPartsCount": 1 } );
        }
    }
    var layer = null;
    var layerManager = new FMDrawIndex._LayerManager();
    for ( i = 0; i < layers.length; i++ )
    {
        layer = layerManager.GetLayerByName(layers[i].LayerName);
        if ( !layer )
            continue;
        if ( layers[i].SelPartsCount === layer.parts.count )
        {
            layers[i].IgnoreLayer = true;
            break;
        }
    }

	var layerPartsArray = null;
	var targetLayerName = null;
	var targetLayer = null;
	var targetPart = null;
	var targetGoJsLayer = null;
	diagram.model.startTransaction( 'Single Step zOrder Adjustment' );
	for ( i = 0; i < selectedPartsArray.length; i++ )
	{
		targetPart = selectedPartsArray[i];
		if ( targetPart.LayerName !== targetLayerName )
		{
				targetLayerName = targetPart.LayerName;
				for ( j = 0; j < layers.length; j++ )
				{
					if ( layers[j].LayerName === targetLayerName )
					{
						targetLayer = layers[j];
						break;
					}
				}
				targetGoJsLayer = diagram.findLayer( targetLayerName );
				layerPartsArray = FMDrawIndex.OrderPartsListByLayerAndZOrder( targetGoJsLayer.parts, !isBackwardStep );
		}
		if ( ( !targetGoJsLayer ) || ( targetLayer.IgnoreLayer ) )
		{
				continue;
		}
		var nextPartZOrder = null;
		var targetPartZOrder = null;
		var targetPartIndex = null;
		for ( var k = 0; k < layerPartsArray.length; k++ )
		{
				if ( layerPartsArray[k].PartData.__gohashid === targetPart.PartData.__gohashid )
				{
					if ( k === ( layerPartsArray.length - 1 ) )
					{
						break;
					} //selected object does not need to be moved
					if ( layerPartsArray[k + 1].IsSelected )
					{
						continue;
					} //next object is also marked for moving
					//swap the zOrder between the next part and the target part
					targetPartIndex = k;
					targetPartZOrder = targetPart.ZOrder;
					nextPartZOrder = layerPartsArray[k + 1].ZOrder;
					diagram.model.setDataProperty( targetPart.PartData, 'zOrder', nextPartZOrder );
					layerPartsArray[k].ZOrder = nextPartZOrder;
					diagram.model.setDataProperty( layerPartsArray[k + 1].PartData, 'zOrder', targetPartZOrder );
					layerPartsArray[k + 1].ZOrder = targetPartZOrder;
					break;
				}
		}
		//re-order the parts array to match the latest zOrder changes
		var tempPart = null;
		if ((targetPartIndex) && (targetPartIndex >= 0) && (targetPartIndex < layerPartsArray.length - 1))
		{
				tempPart = layerPartsArray[targetPartIndex];
				layerPartsArray[targetPartIndex] = layerPartsArray[targetPartIndex + 1];
				layerPartsArray[targetPartIndex + 1] = tempPart;
		}
	}
	diagram.model.commitTransaction( 'Single Step zOrder Adjustment' );
};
FMDrawIndex.MoveSelectionToLayer = function (targetLayerName) {
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if (!diagram) {
		return;
	}
	var layers = [];
	var layerFound = false;
	var nextZOrder = 0;
	var offset = 0;
	var i;
	var j;
	var selectedPartsArray = FMDrawIndex.GetSelectedPartsForLayerChange(diagram.selection, targetLayerName);
	var baseZOrder = FMDrawIndex.GetNextPartZOrder(targetLayerName);
	//Move each selected part that has been verified as being valid for the layer change to the target layer
	var part = null;
	diagram.model.startTransaction('Parent Layer Change');
	for (i = 0; i < selectedPartsArray.length; i++) {
		part = selectedPartsArray[i];
		if (part.LayerName != targetLayerName)
				diagram.model.setDataProperty(part.PartData, 'layerName', targetLayerName);
		diagram.model.setDataProperty(part.PartData, 'zOrder', baseZOrder + i);
	}
	diagram.model.commitTransaction('Parent Layer Change');
};
FMDrawIndex.GetNextPartZOrder = function( layerName )
{
    var layerManager = new FMDrawIndex._LayerManager();
    var layer = layerManager.GetLayerByName(layerName);
    var lastZOrder = 0;
    if ( !layer )
        return lastZOrder;
    var it = layer.parts.iterator;
    while ( it.next() )
    {
        var part = it.value;
        if ( part.zOrder > lastZOrder )
        {
            lastZOrder = part.zOrder;
        }
    }
    return lastZOrder + 1;
};
FMDrawIndex.OrderPartsListByLayerAndZOrder = function( partsList, isAscendingZOrder )
{
	var partsArray = [];
	partsList.each( function( p )
	{
		FMDrawIndex.ExtractGoJsPart( p, partsArray );
	} );
	partsArray.sort( function( a, b )
	{
		var result = 0;
		if ( a.LayerNumber > b.LayerNumber )
		{
				result = 1;
		}
		else if ( a.LayerNumber < b.LayerNumber )
		{
				result = -1;
		}
		if ( result === 0 ) //a.LayerNumber = b.LayerNumber
		{
				if ( a.ZOrder > b.ZOrder )
				{
					if ( isAscendingZOrder )
					{
						result = 1;
					}
					else //descendingZOrder
					{
						result = -1;
					}
				}
				else if ( a.ZOrder < b.ZOrder )
				{
					if ( isAscendingZOrder )
					{
						result = -1;
					}
					else //descendingZOrder
					{
						result = 1;
					}
				}
		}
		return result;
	} );
	return partsArray;
};
FMDrawIndex.ExtractGoJsPart = function( part, partsArray )
{
	if ( part.memberParts )
	{
		part.memberParts.each( function( p )
		{
				FMDrawIndex.ExtractGoJsPart( p, partsArray );
		} );
	}
	else
	{
		var layerManager = new FMDrawIndex._LayerManager();
		var layerNumber = layerManager.GetLayerNumber( part.layer.name );
		var partSelected = part.isSelected;
		if ( part.containingGroup )
		{
				partSelected = part.containingGroup.isSelected;
		}
		partsArray.push( { "LayerName": part.layer.name, "LayerNumber": layerNumber, "ZOrder": part.zOrder, "IsSelected": partSelected, "PartData": part.data } );
	}
};
FMDrawIndex.GetSelectedPartsForLayerChange = function (partsList, targetLayerName) {
	var partsArray = [];
	partsList.each(function (p) {
		if (FMDrawIndex.IsPartValidForLayerChange(p, partsArray, targetLayerName))
				FMDrawIndex.ExtractGoJsPart(p, partsArray);
	});
	partsArray.sort(function (a, b) {
		var result = 0;
		if (a.LayerNumber > b.LayerNumber) {
				result = 1;
		}
		else if (a.LayerNumber < b.LayerNumber) {
				result = -1;
		}
		if (result === 0) //a.LayerNumber = b.LayerNumber
		{
				if (a.ZOrder > b.ZOrder)
					result = 1;
				else if (a.ZOrder < b.ZOrder)
					result = -1;
		}
		return result;
	});
	return partsArray;
};
FMDrawIndex.IsPartValidForLayerChange = function (part, partsArray, targetLayerName) {
	var differentLayerFound = false;
	if (part.memberParts) {
		part.memberParts.each(function (p) {
				if (differentLayerFound)
					return true;
				if (FMDrawIndex.IsPartValidForLayerChange(p, partsArray, targetLayerName)) {
					differentLayerFound = true;
					return true;
				}
		});
	}
	else if (part.layer.name != targetLayerName)
		differentLayerFound = true;
	return differentLayerFound;
};
FMDrawIndex.BringForward = function()
{
	FMDrawIndex.ShiftZOrderBySingleStep( false );
};
FMDrawIndex.SendBackward = function()
{
	FMDrawIndex.ShiftZOrderBySingleStep( true );
};
FMDrawIndex.BringToFront = function()
{
	FMDrawIndex.ShiftZOrderToExtremity( Number.MAX_VALUE );
};
FMDrawIndex.SendToBack = function()
{
	FMDrawIndex.ShiftZOrderToExtremity( Number.NaN );
};
FMDrawIndex.AreObjectsSelected = function()
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !canvas )
	{
		return false;
	}
	return ( canvas.selection.size > 0 );
};
FMDrawIndex.IsAButtonSelected = function()
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !canvas )
	{
		return false;
	}
	return ( canvas.selection.size === 1 && canvas.selection.first().name === 'button' );
};
FMDrawIndex.IsATagSelected = function()
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !canvas )
	{
		return false;
	}
	return ( canvas.selection.size === 1 && canvas.selection.first().name === 'Tag' );
};
FMDrawIndex.IsABarSelected = function()
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !canvas )
	{
		return false;
	}
	return ( canvas.selection.size === 1 && canvas.selection.first().name === 'Bar' );
};
FMDrawIndex.IsAnObjectSelected = function()
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !canvas )
	{
		return false;
	}
	return ( canvas.selection.size === 1 );
};
FMDrawIndex.AreObjectsOnClipboard = function()
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !canvas )
	{
		return false;
	}
	return ( canvas.commandHandler.canPasteSelection() );
};
FMDrawIndex.DistanceBetweenTwoPoints = function( point1, point2 )
{
	var xs;
	var ys;
	xs = point2.left - point1.left;
	xs = xs * xs;
	ys = point2.top - point1.top;
	ys = ys * ys;
	var dist = Math.sqrt( xs + ys );
	return dist;
};
FMDrawIndex.CalculateHypotenuse = function( x, y )
{
	var dist = Math.sqrt( ( x * x ) + ( y * y ) );
	return dist;
};

///BEGIN LAYER MANAGEMENT FUNCTIONS

//Create a new layer (layer1) to be used as the initial default layer of every new diagram.
FMDrawIndex.InitLayersForDiagram = function()
{
	var layerManager = new FMDrawIndex._LayerManager();
	if ( layerManager.GetLayers().length === 0 )
	{
		var layer = layerManager.AddLayer();
		layerManager.SetPrimaryLayer( layer.name );
	}
};

//Configure the layer configuration page
FMDrawIndex.InitLayersDialog = function()
{
	$( '#layers-dialog' ).dialog( {
		autoOpen: false,
		modal: false,
		open: FMDrawIndex.DeactivateTextEditingTool,
		resizable: true
	} );
	$( '#layers-dialog' ).parent().attr( 'id', 'layersDialogBox' );
	FMDrawIndex.InitLayerContextMenu();
};

FMDrawIndex.SetTemplatesToPrimaryLayer = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	var layerManager = new FMDrawIndex._LayerManager();
	var primaryLayerName = layerManager.GetPrimaryLayerName();
	diagram.nodeTemplateMap.each( function( o )
	{
		if ( o && o.value )
		{
				o.value.layerName = primaryLayerName;
		}
	} );

	//Update all the node arch or part arch data objects who have a layerName defined
	diagram.toolManager.mouseDownTools.each( function( o )
	{
		FMDrawIndex.SetArchNodeDataTypeLayerName( o, primaryLayerName );
	} );
	diagram.toolManager.mouseMoveTools.each( function( o )
	{
		FMDrawIndex.SetArchNodeDataTypeLayerName( o, primaryLayerName );
	} );
	diagram.toolManager.mouseUpTools.each( function( o )
	{
		FMDrawIndex.SetArchNodeDataTypeLayerName( o, primaryLayerName );
	} );
};

FMDrawIndex.SetArchNodeDataTypeLayerName = function( o, layerName )
{
	if ( o )
	{
		if ( o.archetypeNodeData && o.archetypeNodeData.layerName !== undefined )
		{
				o.archetypeNodeData.layerName = layerName;
		}

		if ( o.archetypePartData && o.archetypePartData.layerName !== undefined )
		{
				o.archetypePartData.layerName = layerName;
		}
	}
};

FMDrawIndex.LayerListItem = function( layer, primaryLayerNumber )
{
	var layerNameId = layer.name + '_LayerNameTextBox';
	var visibleCheckId = layer.name + '_VisibleCheckBox';
	var activeCheckId = layer.name + '_ActiveCheckBox';
	var visibleChecked = ( layer.visible ) ? ' checked="checked"' : '';
	var visibleDisabled = '';
	var activeChecked = ( layer.allowSelect ) ? ' checked="checked"' : '';
	var activeDisabled = ( !layer.visible ) ? ' disabled="true"' : '';
	var diagram = FMDrawIndex.GetActiveTabCanvasContainerObject().goJsDiagram;
	var primaryLayerClass = '';
	if ( layer.name === diagram.primaryLayerName )
	{
		primaryLayerClass = 'primaryLayerIndicator';
		visibleDisabled = ' disabled="true"';
		activeDisabled = ' disabled="true"';
	}
	var listItem = '<div class="li-wrap col-md-12 ' + primaryLayerClass + '">'
		+ '<li class="col-md-6 layer-cell"><span class="layerNameLabel">' + layer.displayName + '</span><input class="form-control input-md text-left" style="display:none" maxlength="25" onfocusout="FMDrawIndex.LayerTextControlOnFocusOut(this)" id="' + layerNameId + '" type="text" value= "' + layer.displayName + '"/></li>'
		+ '<li class="col-md-3 layer-cell"><input type="checkbox" ' + visibleChecked + ' ' + visibleDisabled + ' onchange="FMDrawIndex.OnLayerVisibleCheckBoxChanged(this)" value="' + layer.name + '" id="' + visibleCheckId + '"/></li>'
		+ '<li class="col-md-3 layer-cell"><input type="checkbox" ' + activeChecked + ' ' + activeDisabled + '" onchange="FMDrawIndex.OnLayerActiveCheckBoxChanged(this)" value="' + layer.name + '" id="' + activeCheckId + '"/></li>'
		+ '</div>';
	return listItem;
};

FMDrawIndex.GenerateLayersDialogTableHTML = function()
{
	var layerHTMLString = '<div id="Layers"> \n' +
		'<div class="layerRow"> \n' +
		'<ul id="ulLayer" class="layer-block"> \n' +
		'<div class="col-md-12 header"> \n' +
		'<div class="col-md-5">Layer</div> \n' +
		'<div class="col-md-3">Visible</div> \n' +
		'<div class="col-md-3">Active</div> \n' +
		'</div> \n' +
		'<div class="layerRow" id="list-row"> \n';
	var layerManager = new FMDrawIndex._LayerManager();
	var layers = layerManager.GetLayers();
	FMDrawIndex.SetTemplatesToPrimaryLayer();
	layers.forEach( function( layer )
	{
		layerHTMLString += FMDrawIndex.LayerListItem( layer );
	} );
	layerHTMLString += '</div> \n' +
		'</ul> \n' +
		'</div> \n' +
		'</div> \n';

	layerHTMLString += '<div id="layerButtonPanel" class="layerConfigButtons"> \n' +
		'<div class="layerAddRemove"><img src="' + window.applicationRootName + '/fmwebapp/images/Add-Transparent.png" id="btnAddLayer" class="layerConfigButtonImage"> </div> \n' +
		'<div class="layerAddRemove"><img src="' + window.applicationRootName + '/fmwebapp/images/Trash-can.png" id="btnRemoveLayer" class=""></div> \n' +
		'</div> \n';

	$( '#layerButtonPanel' ).remove();
	var list = $( '#Layers' );
	list.replaceWith( layerHTMLString );

	$( '#list-row' ).on( 'dblclick', '.li-wrap', function( e )
	{
		FMDrawIndex.SetTargetRowAsPrimaryLayer( this );
	} );

	$( '#btnAddLayer' ).on( 'click', function( e )
	{
		var layerManager = new FMDrawIndex._LayerManager();
		layerManager.AddLayer();
		FMDrawIndex.GenerateLayersDialogTableHTML();
	} );


	$( '#btnRemoveLayer' ).on( 'click', function( e )
	{
		var layerManager = new FMDrawIndex._LayerManager();
		var primaryLayerName = layerManager.GetPrimaryLayerName();
		FMDrawIndex.DeleteLayer( primaryLayerName );
	} );
};

FMDrawIndex.OpenLayersDialog = function()
{
	FMDrawIndex.GenerateLayersDialogTableHTML();

	$( '#layers-dialog' ).dialog( 'open' );
	$( '#layers-dialog' ).dialog( 'option', 'title', 'Layers for ' + FMDrawIndex.GetActiveTabName() );
};

FMDrawIndex.LayersDialogEnabled = function()
{
	return true;
};

FMDrawIndex.OnLayerVisibleCheckBoxChanged = function( checkbox )
{
	var layerName = checkbox.value;

	if ( !checkbox.checked )
	{
		$( '#' + layerName + '_ActiveCheckBox' ).prop( 'disabled', true );
		$( '#' + layerName + '_ActiveCheckBox' ).prop( 'checked', false );
	}
	else
	{
		$( '#' + layerName + '_ActiveCheckBox' ).prop( 'disabled', false );
	}
	FMDrawIndex.SetLayerVisibility( layerName, checkbox.checked );
};

FMDrawIndex.SetLayerVisibility = function( layerName, isVisible )
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	var layerManager = new FMDrawIndex._LayerManager();
	layerManager.SetVisible( layerName, isVisible );
	diagram.requestUpdate();
	FMDrawIndex.GenerateLayersDialogTableHTML();
};

FMDrawIndex.OnLayerActiveCheckBoxChanged = function( checkbox )
{
	var layerName = checkbox.value;
	var active = checkbox.checked;
	var layerManager = new FMDrawIndex._LayerManager();
	layerManager.SetAllowSelect( layerName, active );
};

FMDrawIndex.DeleteLayer = function( layerName )
{
	var layerManager = new FMDrawIndex._LayerManager();
	if ( layerManager.GetLayers().length <= 1 )
	{
		return;
	}
	var layer = layerManager.GetLayerByName( layerName );
	if ( !layer )
	{
		return;
	}
	FMLayout.ConfirmYesNo( 'All the objects that belong to layer [' + layer.displayName + '] will be deleted. Click Yes to confirm the layer deletion, No to cancel the layer deletion.',
		'Input Error',
		function()
		{
				layerManager.DeleteLayer( layerName );
				FMDrawIndex.GenerateLayersDialogTableHTML();
				return;
		},
		function()
		{
				return;
		} );
};

FMDrawIndex.SetTargetRowAsPrimaryLayer = function( targetRow )
{
    var layerManager = new FMDrawIndex._LayerManager();
    //re-enable the checkboxes of the current Primary Layer
    var inputVisibleElement = $( '.li-wrap.primaryLayerIndicator' ).find( 'input' )[1];
    inputVisibleElement.removeAttribute( 'disabled' );
    var inputActiveElement = $( '.li-wrap.primaryLayerIndicator' ).find( 'input' )[2];
    inputActiveElement.removeAttribute( 'disabled' );
    //set the target layer as the new Primary Layer
    $( '.li-wrap' ).removeClass( 'primaryLayerIndicator' );
    $( targetRow ).addClass( 'primaryLayerIndicator' );
    var targetLayer = $( targetRow ).find( 'input' )[1];
    if ( typeof targetLayer !== 'undefined' && targetLayer !== null )
    {
        var targetLayerName = targetLayer.value;
        layerManager.SetPrimaryLayer( targetLayerName );
        targetLayer = layerManager.GetLayerByName(targetLayerName);
        if ( !targetLayer )
            return;
        if ( targetLayer.visible )
        {
            inputVisibleElement = $( targetRow ).find( 'input' )[1];
            inputVisibleElement.setAttribute( 'checked', 'true' );
            inputVisibleElement.setAttribute( 'disabled', 'true' );
        }
        if ( targetLayer.allowSelect )
        {
            inputActiveElement = $( targetRow ).find( 'input' )[2];
            inputActiveElement.setAttribute( 'checked', 'true' );
            inputActiveElement.setAttribute( 'disabled', 'true' );
        }
    }
};

FMDrawIndex.InitLayerContextMenu = function()
{
	var layerManager = new FMDrawIndex._LayerManager();
	$.contextMenu( {
		// define which elements trigger this menu
		selector: '.li-wrap',
		// define the elements of the menu
		items:
				{
					Rename:
						{
								name: 'Rename',
								callback: function( key, opt )
								{
									var targetLayerInput = $( this ).find( 'input' )[1];
									if ( typeof targetLayerInput != 'undefined' && targetLayerInput !== null )
									{
										var layerDisplayName = $( this ).find( '.layerNameLabel' ).text();
										$( this ).find( '.layerNameLabel' ).hide();
										$( this ).find( '.input-md' ).show().val( layerDisplayName ).focus();
									}
								}
						},
					Delete:
						{
								name: 'Delete',
								callback: function( key, opt )
								{
									var layerDisplayName = $( this ).find( '.layerNameLabel' ).text();
									var layer = layerManager.GetLayerByDisplayName( layerDisplayName );
									if ( layer )
									{
										FMDrawIndex.DeleteLayer( layer.name );
									}
								}
						},
					SetAsPrimary:
						{
								name: 'Set as Default',
								callback: function( key, opt )
								{
									FMDrawIndex.SetTargetRowAsPrimaryLayer( this );
								}
						}
				}
	} );
};

FMDrawIndex.LayerTextControlOnFocusOut = function( textbox )
{
    var layerManager = new FMDrawIndex._LayerManager();
    var layerName = FMDrawIndex.GetLayerNameFromElementId( textbox.id );
    var targetLayer = layerManager.GetLayerByName(layerName);
    if ( !targetLayer )
        return;
    var oldDisplayName = targetLayer.displayName;
    var newDisplayName = textbox.value.trim();
    FMDrawIndex.ChangeLayerName( layerName, newDisplayName, oldDisplayName, textbox.id );
};

FMDrawIndex.GetLayerNameFromElementId = function( elementId )
{
	if ( typeof elementId !== 'string' )
	{
		return null;
	}
	var pos = elementId.indexOf( '_' );
	if ( pos > 0 )
	{
		var layerName = elementId.substring( 0, pos );
		return layerName;
	}
	return null;
};

FMDrawIndex.ChangeLayerName = function( layerName, newDisplayName, oldDisplayName, textboxid )
{
	var layerManager = new FMDrawIndex._LayerManager();
	if ( systemLayers.indexOf( newDisplayName.toLowerCase() ) >= 0 )
	{
		FMLayout.ConfirmYesNo( '[' + newDisplayName + '] is a reserved layer name. Do you want to revert back to the old name? Click Yes to revert to the old name, No to try a new name.',
				'Input Error',
				function()
				{
					FMDrawIndex.DisplayLayerNameLabel( textboxid, oldDisplayName );
					return;
				},
				function()
				{
					return;
				} );
	}
	else if ( layerManager.IsLayerDisplayNameInUse( newDisplayName, layerName ) )
	{
		FMLayout.ConfirmYesNo( 'Duplicate Layer Name attempt. This name is already in use on the current diagram. Do you want to revert back to the old name? Click Yes to revert to the old name, No to try a new name.',
				'Input Error',
				function()
				{
					FMDrawIndex.DisplayLayerNameLabel( textboxid, oldDisplayName );
					return;
				},
				function()
				{
					return;
				} );
	}
	else
	{
		if ( layerManager.ChangeLayerDisplayName( layerName, newDisplayName ) )
		{
				FMDrawIndex.DisplayLayerNameLabel( textboxid, newDisplayName );
		}
		else
		{
				FMDrawIndex.DisplayLayerNameLabel( textboxid, oldDisplayName );
		}
	}
};

FMDrawIndex.DisplayLayerNameLabel = function( textboxidParam, newLayerDisplayName )
{
	var textboxid = '#' + textboxidParam;
	$( textboxid ).hide().siblings( '.layerNameLabel' ).show().text( newLayerDisplayName );
};
///END LAYER MANAGEMENT FUNCTIONS


FMDrawIndex.createListenersKeyboard = function()
{
	document.onkeydown = FMDrawIndex.onKeyDownHandler;
};
FMDrawIndex.onKeyDownHandler = function( event )
{
	//Actually, you have to explicitly compare it to true. If the dialog doesn't exist yet, it will not return false (as you would expect), it will return a DOM object.
	//http://stackoverflow.com/questions/3313784/detect-if-a-jquery-ui-dialog-box-is-open
	if (
		$( '#save-dialog' ).dialog( 'isOpen' ) === true ||
				$( '#overwrite-dialog' ).dialog( 'isOpen' ) === true ||
				$( '#load-dialog' ).dialog( 'isOpen' ) === true ||
				$( '#alreadyopen-dialog' ).dialog( 'isOpen' ) === true ||
				$( '#saved-drawing-dialog' ).dialog( 'isOpen' ) === true ||
				$( '#confirm-save-dialog' ).dialog( 'isOpen' ) === true
	)
	{
		return;
	}

	var keyCode;
	if ( window.event )
	{
		keyCode = window.event.keyCode;
	}
	else
	{
		keyCode = event.keyCode;
	}
	if ( event.ctrlKey )
	{
		event.preventDefault();
		switch ( keyCode )
		{
				// Save (Ctrl+S)
				case 83:
					$( '#save' ).trigger( 'click' );
					break;
				case 66:
					if ( event.shiftKey )
					{
						FMDrawIndex.SendToBack();
					}
					break;
				case 68:
					FMDrawIndex.Duplicate();
					break;
				case 70:
					if ( event.shiftKey )
					{
						FMDrawIndex.BringToFront();
					}
					break;
				case 76:
					FMDrawIndex.Rotate( -90 );
					break;
				case 82:
					FMDrawIndex.Rotate( 90 );
					break;
				case 87:
					if ( event.shiftKey )
					{
						FMDrawIndex.FitToWindow();
					}
					break;
				case 89:
					FMDrawIndex.RedoAction();
					break;
				case 90:
					if (event.shiftKey)
					{
						FMDrawIndex.SetZoomLevel(100.0);
					}
					else
					FMDrawIndex.UndoAction();

					break;
				default:
					// TODO
					break;
		}
	}
	else if ( !event.ctrlKey && !event.shiftKey && !event.altKey )
	{
		switch ( keyCode )
		{
				case 27:
					event.preventDefault();
					var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
					if ( !canvas )
					{
						break;
					}
					FMDrawIndex.ResetAllControls( canvas );
					FMDrawIndex.DrawingMode( 'select' );
					break;
				default:
					break;
		}
	}
};
FMDrawIndex.UndoAction = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	diagram.commandHandler.undo();
	FMDrawIndex.GenerateLayersDialogTableHTML();
};
FMDrawIndex.RedoAction = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	diagram.commandHandler.redo();
	FMDrawIndex.GenerateLayersDialogTableHTML();
};
FMDrawIndex.guid = function()
{
	return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace( /[xy]/g, function( c )
	{
		var r = Math.random() * 16 | 0, v = c === 'x' ? r : ( r & 0x3 | 0x8 );
		return v.toString( 16 );
	} );
};
FMDrawIndex.AttachTag = function()
{
	FMDrawIndex.OpenTagDialog( false, FMDrawIndex.UpdateTagSuccess );
};
FMDrawIndex.ChangeSuccess = function( info )
{
	alert( 'Change success.  ' + info );
};
FMDrawIndex.ChangeFailure = function( info )
{
	alert( 'Error updating tag. -- ' + info );
};
FMDrawIndex.TabDiagramModified = function( tabindex )
{
	if ( !FMDrawIndex ||
		!FMDrawIndex.tabCanvasContainerCollection ||
		!( FMDrawIndex.tabCanvasContainerCollection instanceof Array ) ||
		!FMDrawIndex.tabCanvasContainerCollection[tabindex] ||
		!FMDrawIndex.tabCanvasContainerCollection[tabindex].goJsDiagram ||
		!( FMDrawIndex.tabCanvasContainerCollection[tabindex].goJsDiagram instanceof go.Diagram ) )
	{
		return false;
	}

	var canvasTabObject = FMDrawIndex.tabCanvasContainerCollection[tabindex];
	var diagram = FMDrawIndex.tabCanvasContainerCollection[tabindex].goJsDiagram;
	return ( diagram.isModified || canvasTabObject.gridModified );
};
FMDrawIndex.ConfirmClearClipboard = function( tabindex )
{
	if ( !FMDrawIndex ||
		!FMDrawIndex.tabCanvasContainerCollection ||
		!( FMDrawIndex.tabCanvasContainerCollection instanceof Array ) ||
		!FMDrawIndex.tabCanvasContainerCollection[tabindex] ||
		!FMDrawIndex.tabCanvasContainerCollection[tabindex].goJsDiagram ||
		!( FMDrawIndex.tabCanvasContainerCollection[tabindex].goJsDiagram instanceof go.Diagram ) ||
		!FMDrawIndex.clipboardDiagram ||
		!( FMDrawIndex.clipboardDiagram instanceof go.Diagram ) )
	{
		return false;
	}

	var diagram = FMDrawIndex.tabCanvasContainerCollection[tabindex].goJsDiagram;
	return ( diagram.__gohashid === FMDrawIndex.clipboardDiagram.__gohashid );
};
FMDrawIndex.AnyTabDiagramModified = function()
{
	var modified = false;

	FMDrawIndex.tabCanvasContainerCollection.forEach( function( tab )
	{
		if ( !tab )
		{
				return true;
		}
		var diagram = tab.goJsDiagram;
		if ( !diagram )
		{
				return false;
		}
		if ( diagram.isModified || tab.gridModified )
		{
				modified = true;
				return false;
		}
		return true;
	} );
	return modified;
};
FMDrawIndex.ToggleGrid = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	diagram.grid.visible = !diagram.grid.visible;
	FMDrawIndex.ToggleSnapToGrid2( diagram.grid.visible );
	var canvasTabObject = FMDrawIndex.GetActiveTabCanvasContainerObject();
	canvasTabObject.gridModified = true;
};
FMDrawIndex.IsGridOff = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return false;
	}
	return !diagram.grid.visible;
};
FMDrawIndex.IsGridOn = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return false;
	}
	return diagram.grid.visible;
};
FMDrawIndex.ToggleGrid2 = function( onOff )
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	var gridEnabled = ( onOff !== undefined ) ? onOff : !diagram.grid.visible;
	diagram.grid.visible = gridEnabled;
	var canvasTabObject = FMDrawIndex.GetActiveTabCanvasContainerObject();
	canvasTabObject.gridModified = true;
};
FMDrawIndex.ApplyGridConfiguration = function()
{
	FMDrawIndex.ToggleGrid2( $( '#gridcheckbox' ).is( ':checked' ) );
	FMDrawIndex.ToggleSnapToGrid2($('#snapcheckbox').is(':checked'));
	if ($('#coordinatescheckbox').is(':checked'))
	{
			$('#xyCoords').css( 'display', 'inline');
	}
	else
	{
			$('#xyCoords').css('display', 'none');
	};
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	if ( $( '#Xspacingtextbox' ).val() )
	{
		var xSpacing = parseFloat( $( '#Xspacingtextbox' ).val() );
	}
	if ( $( '#Xspacingtextbox' ).val() )
	{
		var ySpacing = parseFloat( $( '#Yspacingtextbox' ).val() );
	}
	if ( xSpacing !== 'NaN' && ySpacing !== 'NaN' )
	{
		if ( ( xSpacing >= 0 && xSpacing <= 100 ) && ( ySpacing >= 0 && ySpacing <= 100 ) )
		{
				diagram.grid.gridCellSize = new go.Size( ( xSpacing ), ( ySpacing ) );
		}
	}
	if ( $( '#GridlineInterval' ).val() )
	{
		var interval = parseInt( $( '#GridlineInterval' ).val() );
		if ( interval >= 1 && interval <= 50 )
		{
				diagram.grid.elt( 2 ).interval = interval; //2 and 3 are the major gridlines (0 and 1 are the minors. see canvascollectionobject.js to see the template)
				diagram.grid.elt( 3 ).interval = interval;
		}
	}
	if ( $( '#snapXspacingtextbox' ).val() )
	{
		var snapXSpacing = parseFloat( $( '#snapXspacingtextbox' ).val() );
	}
	if ( $( '#snapYspacingtextbox' ).val() )
	{
		var snapYSpacing = parseFloat( $( '#snapYspacingtextbox' ).val() );
	}
	if ( snapXSpacing !== 'NaN' && snapYSpacing !== 'NaN' )
	{
		if ( ( snapXSpacing >= 0 && snapXSpacing <= 100 ) && ( snapYSpacing >= 0 && snapYSpacing <= 100 ) )
		{
				diagram.toolManager.draggingTool.gridSnapCellSize = new go.Size( ( snapXSpacing ), ( snapYSpacing ) );
				diagram.model.modelData.snapXCellSize = snapXSpacing;
				diagram.model.modelData.snapYCellSize = snapYSpacing;
		}
	}
};
FMDrawIndex.UpdateGridDialog = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	$( '#gridcheckbox' ).prop( 'checked', diagram.grid.visible );
	$( '#snapcheckbox' ).prop( 'checked', diagram.toolManager.draggingTool.isGridSnapEnabled );
	$( '#Xspacingtextbox' ).val( ( diagram.grid.gridCellSize.width ) );
	$( '#Yspacingtextbox' ).val( ( diagram.grid.gridCellSize.height ) );
	$( '#GridlineInterval' ).val( diagram.grid.elt( 2 ).interval );
	$( '#snapXspacingtextbox' ).val( ( diagram.toolManager.draggingTool.gridSnapCellSize.width ) );
	$('#snapYspacingtextbox').val((diagram.toolManager.draggingTool.gridSnapCellSize.height));
	var elem = document.getElementById('xyCoords');
	if (elem)
		var style = window.getComputedStyle(elem);
	if (style)
		var display = style.getPropertyValue('display');
	if ( display )
	{
		if ( display === "inline" )
			$( '#coordinatescheckbox' ).prop( 'checked', true );
		else
			$( '#coordinatescheckbox' ).prop( 'checked', false );
	}
};
FMDrawIndex.gridValidate = function( field, min, max )
{
	var errFlag = true;
	if ( $( field ).val() )
	{
		var input = parseFloat( $( field ).val() );
		if ( ( input >= min && input <= max ) )
		{
				errFlag = false;
		}
	}
	if ( errFlag )
	{
		FMLayout.Alert( 'Invalid entry. Must be numeric ' + min + '-' + max + '.', 'Input Error', FMDrawIndex.RestoreGridDialogAccessKeys );
		( $( field ).val( '' ) );
		$( '.grid-dialog' ).find( 'button' ).each( function()
		{
				if ( $( this ).find( '.ui-button-text' ).text() === 'Ok' )
				{
					$( this ).removeAttr( 'accesskey' );
				}
				if ( $( this ).find( '.ui-button-text' ).text() === 'Cancel' )
				{
					$( this ).removeAttr( 'accesskey' );
				}
		} );
	}
};
FMDrawIndex.RestoreGridDialogAccessKeys = function()
{
	$( '.grid-dialog' ).find( 'button' ).each( function()
	{
		if ( $( this ).find( '.ui-button-text' ).text() === 'Ok' )
		{
				$( this ).find( '.ui-button-text' ).html( '<u>O</u>k' );
				$( this ).attr( 'accesskey', 'o' );
		}
		if ( $( this ).find( '.ui-button-text' ).text() === 'Cancel' )
		{
				$( this ).find( '.ui-button-text' ).html( '<u>C</u>ancel' );
				$( this ).attr( 'accesskey', 'c' );
		}
	} );
	$( '.grid-dialog' ).focus();
};
FMDrawIndex.TogglePanning = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	FMDrawIndex.panningEnabled = !FMDrawIndex.panningEnabled;
	if ( FMDrawIndex.panningEnabled )
	{
		var selectingtool = diagram.toolManager.findTool( 'DragSelecting' );
		if ( false === selectingtool.isEnabled )
		{
				diagram.toolManager.panningTool.isEnabled = FMDrawIndex.panningEnabled;
		}
		else
		{
				diagram.toolManager.panningTool.isEnabled = FMDrawIndex.panningEnabled;
				selectingtool.isEnabled = false;
				var currentControl = FMDrawIndex.currentDrawControl;
				FMDrawIndex.ResetSelectedControls( currentControl, diagram );
		}
	}
	else
	{
		diagram.toolManager.panningTool.isEnabled = false;
	}
};
FMDrawIndex.IsPanningOff = function()
{
	return !FMDrawIndex.panningEnabled;
};
FMDrawIndex.IsPanningOn = function()
{
	return FMDrawIndex.panningEnabled;
};
FMDrawIndex.IsSnapToGridOff = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram || FMDrawIndex.IsGridOff() )
	{
		return false;
	}
	return !diagram.toolManager.draggingTool.isGridSnapEnabled;
};
FMDrawIndex.IsSnapToGridOn = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return false;
	}
	return diagram.toolManager.draggingTool.isGridSnapEnabled;
};
FMDrawIndex.ToggleSnapToGrid = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram || FMDrawIndex.IsGridOff() )
	{
		return;
	}
	var snapToGrid = !diagram.toolManager.draggingTool.isGridSnapEnabled;
	diagram.toolManager.draggingTool.isGridSnapEnabled = snapToGrid;
	diagram.toolManager.resizingTool.isGridSnapEnabled = snapToGrid;
};
FMDrawIndex.ToggleSnapToGrid2 = function( onOff )
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( !diagram )
	{
		return;
	}
	var snapToGrid = ( onOff !== undefined ) ? onOff : !diagram.toolManager.draggingTool.isGridSnapEnabled;
	diagram.toolManager.draggingTool.isGridSnapEnabled = snapToGrid;
	diagram.toolManager.resizingTool.isGridSnapEnabled = snapToGrid;
};
FMDrawIndex.RefreshAll = function()
{
	FMDrawIndex.tabCanvasContainerCollection.forEach( function( o )
	{
	    if ( o && o.goJsDiagram )
	    {
	        o.goJsDiagram.requestUpdate();
	    }
	} );
};
FMDrawIndex.LoadImageFile = function( input )
{
	//Verify that a file was selected.
	if ( !input ||
		!input.files ||
		!input.files.length ||
		!input.files[0] )
	{
		return;
	}

	var file = input.files[0];
	var imageType = /image.*/;


	//Verify that file type is an image content.
	if ( !file.type.match( imageType ) )
	{
		return;
	}

	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	var reader = new FileReader();
	reader.onload = function( e )
	{
		var img = document.createElement( 'img' );
		img.src = e.target.result;
		var commaIndex = img.src.indexOf( ',' );
		//Sha1 Hash Generated using algorithm found at: http://www.movable-type.co.uk/scripts/sha1.html
		var hash = ( Sha1 && Sha1.hash ) ? Sha1.hash( ( commaIndex !== -1 ) ? img.src.substring( commaIndex + 1 ) : img.src ) : '';
		img.onload = function()
		{
				//Determine if Image exceeds percentage of viewportSize (As pert setting in FMdrawindex.percentViewPortSize)

				var imgWidth = ~~img.width,
					imgHeight = ~~img.height,
					viewPortWidth = ~~diagram.viewportBounds.width || 1,
					viewPortHeight = ~~diagram.viewportBounds.height || 1;

				var imageAspectRatio = imgWidth / ( ( imgHeight === 0 ) ? 1 : imgHeight );
				var viewPortAspectRatio = viewPortWidth / viewPortHeight;

				//var areaOfViewPort = viewPortWidth * viewPortHeight;
				var percentMinimum = FMDrawIndex.percentViewPortSize * .01;
				var viewPortWidthMin = ~~( viewPortWidth * percentMinimum );
				var viewPortHeightMin = ~~( viewPortHeight * percentMinimum );
				//var dimensionCoeff = Math.sqrt(FMdrawindex.percentViewPortSize * .01);

				if ( imgWidth > viewPortWidthMin ||
					imgHeight > viewPortHeightMin )
				{
					if ( viewPortAspectRatio >= 1 )
					{
						//viewPort Aspect is Width >= Height (Horizontal Rectangle or Square) - Height of View Port is restrictive
						imgHeight = viewPortHeightMin;
						imgWidth = ~~( imgHeight * imageAspectRatio );
					}
					else
					{
						//viewPort Aspect is Width < Height (Vertical Rectangle - Width of View Port is restrictive
						imgWidth = viewPortWidthMin;
						imgHeight = ~~( imgWidth / imageAspectRatio );
					}
				}
				var existingImageGuid = '';

				var token = $( 'input[name=__RequestVerificationToken]' ).val();
				var headers = {};
				headers['__RequestVerificationToken'] = token;

				//AJAX CALL TO CHECK FOR HASH EXISTENCE
				$.ajax( {
						type: 'GET',
						dataType: 'json',
						cache: false,
						async: false,
						headers: headers,
						url: 'ImageHashExists',
						data: 'imageHash=' + hash,
						success: function( response )
						{
								FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
								{
									if ( !inError )
									{
										existingImageGuid = data;
									}
								} );
						},
						error: function( xhr, textStatus, error )
						{
								FMErrorAndExceptionHandling.ShowException( xhr,
									textStatus,
									error,
									function()
									{
										existingImageGuid = '';
									} );
						}
					}
				);

				var imgsource = ( existingImageGuid.length > 0 ) ? FMDrawIndex.GeneratePictureURL( existingImageGuid, ~~imgWidth, ~~imgHeight ) : '';
				var copy = JSON.parse(JSON.stringify(FMDrawIndex.defaultArchetype));
				if ( loadingPictureFromDoubleClick === true )
				{
					diagram.selection.each( function( node )
					{
						if ( !( node instanceof go.Node ) )
						{
								return;
						}
						var pic = node.findObject( 'SHAPE' );
						var newSize = imgWidth + ' ' + imgHeight;
						diagram.model.startTransaction( 'update img' );
						if ( existingImageGuid === '' )
						{
								pic.source = img.src;
						}
						else
						{
								diagram.model.setDataProperty( node.data, 'source', imgsource );
						}
						diagram.model.setDataProperty( node.data, 'fileName', file.name );
						diagram.model.setDataProperty( node.data, 'imageType', file.type );
						diagram.model.setDataProperty( node.data, 'imageGuid', existingImageGuid );
						diagram.model.setDataProperty( node.data, 'imageHash', hash );
						diagram.model.setDataProperty(node.data, 'size', newSize);

						if ( copy.transparency )
						{
							// Default the transparency to no transparency (per MVP testing).
							node.data.transparency = "0";
						}

						if ( copy.color )
						{
							// Default the transparency to no transparency (per MVP testing).
							node.data.color = FMDrawIndex.RemoveTransparencyFromColor(copy.color);
						}

						diagram.model.commitTransaction('update img');

						});

					// ReSharper disable once AssignToImplicitGlobalInFunctionScope
					loadingPictureFromDoubleClick = false;
				}
				else
				{
					var layerManager = new FMDrawIndex._LayerManager();
					var primaryLayerName = layerManager.GetPrimaryLayerName();
					var pictureNodeData = {
						category: 'picture',
						pos: '0 0',
						size: imgWidth + ' ' + imgHeight,
						layerName: primaryLayerName,
						zOrder: FMDrawIndex.GetNextPartZOrder( primaryLayerName ),
						source: imgsource,
						fileName: file.name,
						imageType: file.type,
						imageGuid: existingImageGuid,
						imageHash: hash
					};
						if ( copy.transparency )
						{
							// Default the transparency to no transparency (per MVP testing).
							pictureNodeData.transparency = "0";
						}

						if ( copy.color )
						{
							// Default the transparency to no transparency (per MVP testing).
							pictureNodeData.color = FMDrawIndex.RemoveTransparencyFromColor(copy.color);
						}
					

					var node = FMDrawIndex.AddNodeToDiagram( diagram, pictureNodeData, null );
					if ( imgsource === '' )
					{
						if ( node && node.findObject( 'SHAPE' ) )
						{
								diagram.skipsUndoManager = true;
								node.findObject( 'SHAPE' ).source = img.src;
								diagram.skipsUndoManager = false;
						}
					}
				}
		};
	};
	reader.readAsDataURL( file );
};
FMDrawIndex.ResizeDrawingWorkSpace = function()
{
	var win = $( window );
	var leftBar = $( '#sidebar-backpanel' );
	//var topBar = $('#header');
	var rightBar = $( '#rightsidebar' );
	var bottomBar = $( '#footer' );


	FMDrawIndex.tabCanvasContainerCollection.forEach( function( tabObj )
	{
		if ( tabObj )
		{
				var div = $( '#' + tabObj.goJsDiagram.div.id );
				var parentDiv = div.parent();

				//var divPaddingTop = parseInt(div.css('padding-top'));
				//var divPaddingBottom = parseInt(div.css('padding-bottom'));
				//var divPaddingLeft = parseInt(div.css('padding-left'));
				//var divPaddingPaddingRight = parseInt(div.css('padding-right'));


				//Set Width and Height of Parent Tab Div
				parentDiv.height( win.height() - 100 );
				parentDiv.width( win.width() - rightBar.width() - leftBar.width() - 50 );

				//Set Width and Height of Diagram Div
				div.height( parentDiv.height() - bottomBar.height() +7);
				div.width( parentDiv.width() +22 );

				//Instruct GoJS to update Canavs Element Size
				tabObj.goJsDiagram.requestUpdate();
		}
	} );
};
FMDrawIndex.AddTextWithSingleClick = function( event )
{
	if ( !event || !event.diagram )
	{
		return;
	}
	if ( FMDrawIndex.currentDrawControl === 'text' )
	{
		var part = FMDrawIndex.AddShapeFromDragAndDrop( {
				category: 'text',
				color: 'white',
				key: '',
				size: '100 20'
		}, event );

		//Activate Text Editing tool right away
		if ( part )
		{
				var tool = event.diagram.toolManager.textEditingTool;
				var obj = part.findObject( 'TEXTBLOCK' );
				tool.textBlock = obj;
				//Ensure that text editing tool is current tool so when navigating away from object it will cause tool to deactivate.
				event.diagram.currentTool = tool;
				tool.doStart();
		}
	}
};
FMDrawIndex.RetrieveTagUnits = function( tagObject )
{
	var tagUnits = tagObject.TagUnits;
	if ( tagUnits === 0 )
	{
		tagUnits = tagObject.TagUnitsOriginal;
	}
	return tagUnits;
};
FMDrawIndex.OpenImageSelectionDialog = function()
{
	$( '#chooseImageFile' ).dialog( 'open' );
}; //Bind Keyboard Events to Listeners
FMDrawIndex.createListenersKeyboard();

FMDrawIndex.ZoomSliderUpdate = function()
{
	FMDrawIndex.SetZoomLevel( FMDrawIndex.GetZoomLevelForSlider() );
};
FMDrawIndex.DecreaseZoom = function()
{
	var zoomLevel = FMDrawIndex.GetZoomLevel();
	if ( zoomLevel <= 100 )
	{
		zoomLevel = zoomLevel - 10.0;
	}
	else
	{
		zoomLevel = zoomLevel - 50.0;
	}
	if ( zoomLevel < 12.5 )
	{
		zoomLevel = 12.5;
	}
	FMDrawIndex.SetZoomLevel( zoomLevel );
};
FMDrawIndex.IncreaseZoom = function()
{
	var zoomLevel = FMDrawIndex.GetZoomLevel();
	if ( zoomLevel < 100 )
	{
		zoomLevel = zoomLevel + 10.0;
	}
	else
	{
		zoomLevel = zoomLevel + 50.0;
	}
	if ( zoomLevel > 800.0 )
	{
		zoomLevel = 800.0;
	}
	FMDrawIndex.SetZoomLevel( zoomLevel );
};
FMDrawIndex.RoundToPlaces = function( places )
{
	return +( Math.round( this + 'e+' + places ) + 'e-' + places );
};
FMDrawIndex.SetZoomLevel = function( zoomLevel )
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( diagram )
	{
		var scale = zoomLevel / 100.0;
		diagram.scale = scale;
	}
	FMDrawIndex.SetZoomLevelForLabel( zoomLevel );
	FMDrawIndex.SetZoomLevelForSlider( zoomLevel );
};
FMDrawIndex.FitToWindow = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( diagram )
	{
		diagram.alignDocument( go.Spot.Center, go.Spot.Center );
		diagram.zoomToFit();
		FMDrawIndex.SetZoomLevelForSlider(FMDrawIndex.GetZoomLevel());
		FMDrawIndex.SetZoomLevelForLabel(FMDrawIndex.GetZoomLevel());
	}
}
FMDrawIndex.ResetViewportToOrigin = function () {
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if (diagram) {
		//FMdrawindex.SetZoomLevel(100.0);
		diagram.position = new go.Point(0, 0);

	}
}
FMDrawIndex.InitializeZoomLevel = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( diagram )
	{
		diagram.skipsUndoManager = true;
		diagram.maxScale = 8;
		diagram.minScale = 0.125;
		diagram.skipsUndoManager = false;
		var zoomLevel = diagram.scale * 100.0;
		FMDrawIndex.SetZoomLevelForLabel( zoomLevel );
		FMDrawIndex.SetZoomLevelForSlider( zoomLevel );
	}
};
FMDrawIndex.GetZoomLevel = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if ( diagram )
	{
		return diagram.scale * 100.0;
	}
	return FMDrawIndex.GetZoomLevelForSlider();
};
FMDrawIndex.SetZoomLevelForLabel = function( zoomLevel )
{
	var zoomValue = document.getElementById( 'ZoomValue' );
	var zoomLevelStr = zoomLevel.toFixed( 1 ) + '%';
	zoomValue.innerHTML = zoomLevelStr;
};
FMDrawIndex.GetZoomLevelForLabel = function()
{
	var zoomValue = document.getElementById( 'ZoomValue' );
	var zoomLevelStr = zoomValue.innerHTML;
	zoomLevelStr = zoomLevelStr.replace( '%', '' );
	var zoomLevel = parseFloat( zoomLevelStr );
	return zoomLevel;
};
FMDrawIndex.SetZoomLevelForSlider = function( zoomLevel )
{
	var zoomSlider = document.getElementById( 'ZoomSlider' );
	zoomSlider.value = zoomLevel;
};
FMDrawIndex.GetZoomLevelForSlider = function()
{
	var zoomSlider = document.getElementById( 'ZoomSlider' );
	return parseFloat( zoomSlider.value );
};
FMDrawIndex.ApplyZoomConfiguration = function()
{
	if ($('#fittoviewport').is(':checked')) {
		FMDrawIndex.FitToWindow();
		$("#zoom-dialog").dialog("close");
	FMDrawIndex.ClearZoomDialog();
		}

	var zoomPercent = parseFloat($('#customzoom').val());
	zoomPercent = Math.round(zoomPercent * 10) / 10;
	if (!( zoomPercent && zoomPercent >= 12.5 && zoomPercent <= 800 ))
	{
		FMLayout.Alert('Invalid entry. Must be numeric between 12.5 and 800.');
						$('#customzoom').val('');
						}
	else
	{
		FMDrawIndex.SetZoomLevel(zoomPercent);
	if ($('#resettoorigin').is(':checked'))
			{
	FMDrawIndex.ResetViewportToOrigin();
	}
	FMDrawIndex.ClearZoomDialog();
		$("#zoom-dialog").dialog("close");
	}

};

FMDrawIndex.ClearZoomDialog = function()
{
			$('input[name=viewportzoom]').attr('checked', false);
			$('input[name=zoomradio]').attr('checked', false);
			$('#customzoom').val(FMDrawIndex.GetZoomLevel().toFixed(1));
}

FMDrawIndex.OpenDrawing = function( e, onlyShowStandardDrawings )
{
    
    if ( $( '#graphicselctionIsForButton' ).val() === 'true' )
    {
        $('#graphicselctionIsForButton').val('false');
        return;
    }
	if ($('#layers-dialog').dialog('isOpen')) {
	$('#layers-dialog').dialog('close');}
        
    var token = $( 'input[name=__RequestVerificationToken]' ).val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;


    var panelTypes = '';
    if ( onlyShowStandardDrawings)
    {
        $('#graphicPanelTypeFilterpanelID').hide();
        $( '#graphicselctionIsForButton' ).val( 'true' );
        panelTypes = 'Standard';
    }
    else
    {
        $('#graphicPanelTypeFilterpanelID').show();
        var standardChecked = $( '#standarDrawingCheckbox' ).is( ':checked' );
        var pointDetailChecked = $( '#pointDetailCheckbox' ).is( ':checked' );
        if ( ( standardChecked && pointDetailChecked ) || e )
            panelTypes = 'Standard,Detail';
        else if ( standardChecked && !pointDetailChecked )
            panelTypes = 'Standard';
        else if ( !standardChecked && pointDetailChecked )
            panelTypes = 'Detail';
    }


    $.ajax( {
        type: 'get',
        dataType: 'json',
        cache: false,
        headers: headers,
        url: 'GetDrawingNamesByPanelType',
        data: 'panelTypesToFilter=' + panelTypes,
        success: function( response )
        {
            FMErrorAndExceptionHandling.HandleMessages( response, function( drawings, inError )
            {
                if ( inError )
                {
                    return;
                }
                FMDraw.GetDrawingNamesSuccess( drawings );
            } );
        },
        error: function( ex )
        {
            FMErrorAndExceptionHandling.ShowError( 'Error loading drawing names: ' + ex.responseText );
        }
    } );

    $( '#load-dialog' ).dialog( 'open' );
};

FMDrawIndex.DeactivateTextEditingTool = function()
{
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if (diagram)
	{
		if (diagram.currentTool instanceof go.TextEditingTool) {
				diagram.currentTool.acceptText(go.TextEditingTool.LostFocus);
		}
	}
}

FMDrawIndex.SaveDrawPersistentData = function (rect)
{
    var token = $('input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;
    var posleft = rect.left - 20;
    var posTop = rect.top - 84;

    $.ajax( {
        type: 'POST',
        async: true,
        processdata: false,
        dataType: 'json',
        url: 'SaveDrawViewStateSettings',
        data: {
            ToolbarLeftCoord: posleft,
            ToolbarTopCoord: posTop,
            '__RequestVerificationToken': $('input[name=__RequestVerificationToken]').val()
        },
        success: function (response) {
        },

        error: function (e) {
            FMErrorAndExceptionHandling.ShowError('Error saving persistent data: ' + e.responseText);
        }
    });
}

FMDrawIndex.ReadDrawPersistentData = function (e) {
    var token = $('input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;
    var v1 = 0;

    $.ajax({
        type: 'GET',
        async: false,
        dataType: 'json',
        cache: false,
        headers: headers,
        url: 'ReadDrawViewStateSettings',
        //data: 'id=' + namearray[0],
        success: function (response) {
            FMErrorAndExceptionHandling.HandleMessages(response, function (results, inError) {
                if (inError) {
                    return -1;
                }
                var temp = response.Data.split(";");
                var tpos = temp[0] + 'px';//'80px';
                var lpos = temp[1] + 'px';//'0px';
                //alert(response.Data + " " + lpos + " " + tpos);
                $('#sidebar').css('left', lpos);
                $('#sidebar').css('top', tpos);
                if(temp[0] < -30)
                {
                    v1 = 1;
                }

            });
        },
        error: function (xhr, textStatus, error) {
            FMErrorAndExceptionHandling.ShowException(xhr,
                textStatus,
                error
                );
        }
    });
    return v1;
}

FMDrawIndex.DisplayPointTemplateDetailOnSelectDrawingDialog = function (row) {
    var pointTemplateName = (row && row.attr) ? row.attr('data-pointtemplatename') : '';
    if (pointTemplateName === '') {
        $('#label-pointDetailTitle').hide();
        $('#label-pointTemplateName').hide();
        $('#load-pointTemplateName').text('');
        $('#label-pointType').hide();
        $('#load-pointType').text('');
    }
    else {
        var pointType = row.attr('data-pointtemplatetype');
        $('#label-pointDetailTitle').show();
        $('#label-pointTemplateName').show();
        $('#load-pointTemplateName').text(pointTemplateName);
        $('#label-pointType').show();
        $('#load-pointType').text(pointType);
    }
}


FMDrawIndex.NewCanvas = function(e,FMDraw, panelType, pointTemplateGuid, published)
{
	if ( e )
	{
		e.preventDefault();
	}

	FMDrawIndex.SaveDefaultstoModelData();

	FMDraw.num_tabs = FMDraw.num_tabs + 1;
	var diagramID = 'diagram' + (FMDraw.num_tabs - 1);
	var tabID = 'tab' + (FMDraw.num_tabs - 1);
	var diagramJQueryID = '#' + diagramID;
	var tabJQueryID = '#' + tabID;
	var canvasJQueryID = "#" + diagramID + " > canvas";
	var tabIndex = FMDraw.num_tabs - 1;

	$("div#tabs ul").append(
		"<li class='drawingTab' drawingguid='" + FMDrawIndex.guid() + "' id='tabs" + tabIndex + "' class='ui-closable-tab'><a href='#" + tabID + "'>Drawing " + FMDraw.num_tabs + "</a><a id='close' onClick='return FMDraw.close(" + (FMDraw.num_tabs - 1) + ");' style='margin-left:-10px; font-weight: 900;'>X</a></li>"
	);

	$("div#tabs").append(
		"<div id='" + tabID + "'>" + "<div id='" + diagramID + "'> Your browser does not support the HTML5 canvas.</canvas>" + "</div>"
	);
	$("div#tabs").tabs("refresh");
	FMDrawIndex.AttatchGoJSDiagramToNewCanvas(FMDraw.num_tabs - 1);
	var tabDiv = $(tabJQueryID);
	var diagramDiv = $(diagramJQueryID);
	FMDrawIndex.InitContextMenu($(canvasJQueryID));
	FMDrawIndex.UpdateactiveCanvas(FMDraw.num_tabs - 1);
	FMDrawIndex.InitLayersForDiagram();
	FMDrawIndex.ResizeDrawingWorkSpace();
	FMDrawIndex.InitializeZoomLevel();
	var tabname = "#tab" + FMDraw.num_tabs - 1;
	var index = $("#tabs a[href='" + tabname + "']").parent().index();
	$('div#tabs').tabs("option", "active", index);
	var tabindex = $("div#tabs").tabs('option', 'active');
	$(canvasJQueryID).addClass("upper-canvas");
	FMDrawIndex.ResetDefaults();
	FMDrawIndex.RefreshPreview();
	FMDrawIndex.SetDiagramModelDataValue("PanelType", (panelType) ? panelType : 'Standard');
	FMDrawIndex.SetDiagramModelDataValue("PointTemplateGuid", pointTemplateGuid);
	FMDrawIndex.SetDiagramModelDataValue("Published", (published) ? published : "false");
	FMDrawIndex.UpdateMode();
	FMDrawIndex.UpdateExportButton();
}

FMDrawIndex.NewPointDetail = function (e, FMDraw)
{
	if (e)
	{
		e.preventDefault();
	}
    //make sure that drop list is cleared so we don't include duplicate entries.
	$('#PointTemplateDropdownList option').remove();
	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	$.ajax({
		type: 'get',
		dataType: 'json',
		cache: false,
		headers: headers,
		url: 'GePointTemplates',
		async: false,
		success: function (response)
		{
			FMErrorAndExceptionHandling.HandleMessages(response, function (pointTemplateList, inError)
			{
				if (inError)
				{
					return;
				}
				var firstTimeThrough = true;
				for ( var i = 0; i < pointTemplateList.length; i++ )
				{
					if (firstTimeThrough)
					{
				        $('#PointTemplateDropdownList').append('<option value="' + pointTemplateList[i].Item1 +
                            '" selected>' + pointTemplateList[i].Item2 + '</option>');
				        firstTimeThrough = false;
				    }
					else
					{
				        $('#PointTemplateDropdownList').append('<option value="' + pointTemplateList[i].Item1 +
                           '">' + pointTemplateList[i].Item2 + '</option>');
				    }
				}
				$('#selectPointTemplate').dialog('open');
				FMDrawIndex.UpdateExportButton();
			});
		},

		error: function (e) {
			FMErrorAndExceptionHandling.ShowError('Error loading point template names: ' + e.responseText);
		}
	});
}

FMDrawIndex.NewPointDetailWorker = function(pointTemplateGuid)
{
	FMDrawIndex.NewCanvas(null, FMDraw, "Detail", pointTemplateGuid, false);
}

FMDrawIndex.SetDiagramModelDataValue = function (name, value, requireUndo) {
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if (diagram && diagram.model && diagram.model.modelData) {
	    if (requireUndo) {
	        diagram.model.startTransaction('SetDiagramModelDataValue');
	        diagram.model.setDataProperty(diagram.model.modelData, name, value);
	        diagram.model.commitTransaction('SetDiagramModelDataValue');
	    }
	    else {
	        diagram.skipsUndoManager = true;
	        diagram.model.setDataProperty(diagram.model.modelData, name, value);
	        diagram.skipsUndoManager = false;
	    }
	}
}

FMDrawIndex.GetDiagramModelDataValue = function (name) {
	var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
	if (diagram && diagram.model && diagram.model.modelData && name in diagram.model.modelData) {
		return diagram.model.modelData[name];
	}
	return null;
}

FMDrawIndex.UpdateMode = function()
{
	var panelTypeStr = FMDrawIndex.GetDiagramModelDataValue("PanelType");
	var published = FMDrawIndex.GetDiagramModelDataValue("Published");
	if (published == null || panelTypeStr === 'Detail') {
		published = 'true';
		FMDrawIndex.SetDiagramModelDataValue("Published", published);
	}
	var publishedLabel = document.getElementById("publishedLabel");
	var publishedCheckBox = document.getElementById("publishedCheckBox");
	if (panelTypeStr && publishedLabel && publishedCheckBox)
	{
		var modeLabel = document.getElementById("modeIndicatorLabel");
		if ( panelTypeStr === "Standard" )
		{
			modeLabel.classList.remove("modeIndicatorGraphicLabelClass");
			modeLabel.classList.remove("modeIndicatorPointDetailLabelClass");
			modeLabel.classList.add("modeIndicatorGraphicLabelClass");
			modeLabel.removeAttribute( "title" );
			modeLabel.innerHTML = "Graphic";
			publishedLabel.style.display = '';
			publishedCheckBox.style.display = '';
			publishedCheckBox.checked = (published == 'true') ? 1 : 0;
		}
		else
		{
			modeLabel.classList.remove("modeIndicatorGraphicLabelClass");
			modeLabel.classList.remove("modeIndicatorPointDetailLabelClass");
			modeLabel.classList.add("modeIndicatorPointDetailLabelClass");
			modeLabel.removeAttribute("title");
			modeLabel.innerHTML = "Detail";
			publishedLabel.style.display = 'none';
			publishedCheckBox.style.display = 'none';
			publishedCheckBox.checked = (published == 'true') ? 1 : 0;

			FMDrawIndex.GetPointIdAndType();
		}
	}
}

FMDrawIndex.GetPointIdAndType = function () {
	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var pointTemplateGuid = FMDrawIndex.GetDiagramModelDataValue("PointTemplateGuid");
	$.ajax({
		type: 'get',
		dataType: 'json',
		cache: false,
		headers: headers,
		url: 'GePointTemplateIdAndType',
		async: false,
		data: {
			pointTemplateGuidStr: pointTemplateGuid
		},
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (pointIdAndTypeTuple, inError) {
				if (inError) {
					return;
				}
				FMDrawIndex.SetModeTitle( pointIdAndTypeTuple.Item1, pointIdAndTypeTuple.Item2 );
			});
		},

		error: function (e) {
			FMErrorAndExceptionHandling.ShowError('Error loading point template names: ' + e.responseText);
		}
	});
}

FMDrawIndex.SetModeTitle = function (pointTemplateId, pointTemplateType)
{
	var title = "";
	if ( pointTemplateType )
	{
		title += "Point Template Type: " + pointTemplateType + "\r\n";
	}
	if ( pointTemplateId )
	{
		title += "Point Template ID: " + pointTemplateId;
	}
	if ( title.length > 0 )
	{
		var modeLabel = document.getElementById("modeIndicatorLabel");
		modeLabel.setAttribute('title', title);
	}
}






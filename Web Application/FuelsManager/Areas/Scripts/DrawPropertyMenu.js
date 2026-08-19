var SelectedObjectsAreAllTagsAndAllRelated = 0;
var SelectedObjectsAreAllTagsButNotAllRelated = 1;
var SelectedObjectsContainsAPointTemplateTag = 2;
var SelectedObjectsAreNotAllTags = 3;
var SelectedObjectsInvalidInformation = 4;


FMDrawPropertyMenu =
{
        currentButtonActionAssociation: '',		//Used to communicate which action to associate with the button.
        onlyAssociateTagDataToButton: false,	//Used to communicate to PointTagSuccess whether to associate selected tag with button control or add new button control.
        onlyAssociateDrawingIdToButton: false,	//Used to communicate to OpenDrawing whether to associate selected drawing ID with button control.
        defaultControlFocusIds: [],				//Used to set the focus on controls.  The last control is set to focus.  The other controls will force the properties dialog to expand if they are after the last control id in the array. (I.E. Hack)
        propertyPreviewDiagram: '',
        propertyPreviewNode: '',
        IgnoreEvent: false,
        SectionExpandCollapseStateList: null,	// Used to maintain the section expand/collapse state.
		SectionSubStateFilterList: null,
		imageRootPath: null,					// Used to find the root image path.
		FMDrawPropertiesMenuDockSetting: null,
		manualColorEntryError: false
};

if (!window.applicationRootName) {
    let p = window.location.pathname.indexOf('/', 1);
    let p0 = window.location.pathname.indexOf('/(S(', 1);
    let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
    debugger;
    window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

var FMTAGFIELDSELECTION = FMTAGFIELDSELECTION || {
    VALUE: 0,
    ID: 1,
    TIMESTAMP: 2,
    UNITS: 3,
	 ALARMSTATUS: 4,
    GetFieldString: function( field )
    {
        switch ( field )
        {
            case FMTAGFIELDSELECTION.VALUE:
                return 'VALUE';
            case FMTAGFIELDSELECTION.ID:
                return 'ID';
            case FMTAGFIELDSELECTION.TIMESTAMP:
                return 'TIMESTAMP';
            case FMTAGFIELDSELECTION.UNITS:
            	return 'UNITS';
        		case FMTAGFIELDSELECTION.ALARMSTATUS:
        			return 'ALARM STATUS';
            default:
                return 'UNKNOWN';
        }
    }
};

FMDrawPropertyMenu.IsAllSelectedPointTrendButtonActions = function (selectedObjects)
{
	var node;

	for (var nextObjIndex = 0; nextObjIndex < selectedObjects.length; nextObjIndex++)
	{
		node = selectedObjects[nextObjIndex];

		if (FMDrawPropertyMenu.isObjectGroup(node))
		{
			var arr = FMDrawPropertyMenu.IteratorToArray(node.memberParts);
			if ( !FMDrawPropertyMenu.IsAllSelectedPointTrendButtonActions( arr ) )
			{
				return false;
			}
		}
		else
		{
			if (node.data.category !== "button"
				|| node.data.buttonActionType !== ButtonActionTypePointTrend
				|| node.data.ButtonActionTypeDetail !== ButtonActionTypeDetail)
			{
				return false;
			}
		}
	}

	return true;
};

//====================================================================
// This function will open the Properties menu and initialize the
// individual properties based on the selected object.
//====================================================================
FMDrawPropertyMenu.OpenPropertiesPopupMenu = function(defaultControlFocusIds)
{
    if ( FMDrawIndex && FMDrawIndex.DeactivateTextEditingTool )
    {
        FMDrawIndex.DeactivateTextEditingTool();
    }
    FMDrawPropertyMenu.defaultControlFocusIds = defaultControlFocusIds;

    FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
    
    FMDrawPropertyMenu.ClearPropertyWindow( );
    FMDrawPropertyMenu.InitiatizePropertiesMenu();

    // beginning
    $( '#textbox-propertiesMenu-POINTANDTAGID' ).hover( function()
    {
        $( this ).css( 'border-color', '#ffffff' );
    }, function()
    {
        $( this ).css( 'border-color', '#9b9b9b' );
    } );

    $( '#textbox-propertiesMenu-POINTANDTAGID' ).click( function()
    {
        if ( FMDrawPropertyMenu.PropertyActiveObject == null )
        {
            return;
        }

        $( '#textbox-propertiesMenu-POINTANDTAGID' ).val( 'Loading tags...' );

        var data = FMDrawPropertyMenu.PropertyActiveObject.data;

         if (FMDrawPropertyMenu.IsAllSelectedPointTrendButtonActions(FMDrawPropertyMenu.selectedObjects))
        {
             FMDrawIndex.OpenTagDialogForSwitch(!FMDrawPropertyMenu.MultiSelectionFlag, false, FMDrawIndex.UpdatePointTrendButtonSuccess, data.TagPointID, data.PointGUID, data.TagTagID, data.TagGUID, data.TagPointValueType, data.TagPropertyID, data.PointTemplateTagSelectionIndicator, true);
        }
        else
        {
             FMDrawIndex.OpenTagDialogForSwitch(!FMDrawPropertyMenu.MultiSelectionFlag, false, FMDrawIndex.UpdateTagSuccess, data.TagPointID, data.PointGUID, data.TagTagID, data.TagGUID, data.TagPointValueType, data.TagPropertyID, data.PointTemplateTagSelectionIndicator);
        }

        setTimeout( function()
        {
            var data = FMDrawPropertyMenu.PropertyActiveObject.data;
            if (data.PointTemplateTagSelectionIndicator) {
                FMDrawPropertyMenu.SetAlternateLabelText('label-propertiesMenu-POINTANDTAGID');
            }
            else {
                FMDrawPropertyMenu.SetOriginalLabelText('label-propertiesMenu-POINTANDTAGID');
            }
            $( '#textbox-propertiesMenu-POINTANDTAGID' ).val( data.TagPointIDAndTagID );
        }, 500 );
    });
    // end pointandtagid

    // beginning
    $('#textbox-propertiesMenu-ANIMATIONBUTTON').hover(function () {
        $(this).css('border-color', '#ffffff');
    }, function () {
        $(this).css('border-color', '#9b9b9b');
    });

    $('#textbox-propertiesMenu-ANIMATIONBUTTON').click(function () {
        if (FMDrawPropertyMenu.PropertyActiveObject == null) {
            return;
        }

        FMDrawPropertyMenu.InvokeAnimationButtonAction();
    });
    // end animationbutton

    $('#textbox-propertiesMenu-POINTID').hover(function ()
    {
        $( this ).css( 'border-color', '#ffffff' );
    }, function()
    {
        $( this ).css( 'border-color', '#9b9b9b' );
    } );

    $( '#textbox-propertiesMenu-POINTID' ).click( function()
    {
        if ( FMDrawPropertyMenu.PropertyActiveObject == null )
        {
            return;
        }

        $( '#textbox-propertiesMenu-POINTID' ).val( 'Loading values...' );

        var data = FMDrawPropertyMenu.PropertyActiveObject.data;

        if (FMDrawPropertyMenu.IsAllSelectedPointTrendButtonActions(FMDrawPropertyMenu.selectedObjects))
        {
            FMDrawIndex.OpenTagDialogForSwitch(!FMDrawPropertyMenu.MultiSelectionFlag, false, FMDrawIndex.UpdatePointTrendButtonSuccess, data.TagPointID, data.PointGUID, data.TagTagID, data.TagGUID, data.TagPointValueType, data.TagPropertyID, data.PointTemplateTagSelectionIndicator, true);
        }
        else
        {
            FMDrawIndex.OpenTagDialogForSwitch(!FMDrawPropertyMenu.MultiSelectionFlag, false, FMDrawIndex.UpdateTagSuccess, data.TagPointID, data.PointGUID, data.TagTagID, data.TagGUID, data.TagPointValueType, data.TagPropertyID, data.PointTemplateTagSelectionIndicator);
        }

        setTimeout( function()
        {
            $( '#textbox-propertiesMenu-POINTID' ).val( FMDrawPropertyMenu.PropertyActiveObject.data.TagPointID );
        }, 500 );
    } );

    FMDrawPropertyMenu.ShowPropertiesMenu();

    // This section will find the pattern dropdown position which is used to calculate the scrollbar position. The reason
    // is the nice scroll will not recalculate these position after the main property window scrollbar is moved.
    FMDrawPropertyMenu.fillPatternScrollLocation = ( $( '#tr-propertiesMenu-FILLPATTERN' ).position().top + 21.95 ).toString() + 'px';
    FMDrawPropertyMenu.lineStyleScrollLocation = ( $( '#tr-propertiesMenu-LINESTYLE' ).position().top + 21.95 ).toString() + 'px';
    FMDrawPropertyMenu.lineFromScrollLocation = ( $( '#tr-propertiesMenu-LINEFROMARROW' ).position().top + 21.95 ).toString() + 'px';
    FMDrawPropertyMenu.lineToScrollLocation = ($('#tr-propertiesMenu-LINETOARROW').position().top + 21.95).toString() + 'px';

    //Timeout is a hack to handle conflicting focus events caused by GoJS when
    //drawing button with mouse.
    setTimeout( FMDrawPropertyMenu.SetFocusOnDefaultControl, 100 );
};

//==================================================================
// This method will show the properties menu window. It will check
// to see if the property menu window is dock and if so, it will
// set the docking area width.
//==================================================================
FMDrawPropertyMenu.ShowPropertiesMenu = function()
{
  if (!FMDraw.HasPropertyMenuInitiallyDocked) {
    FMDraw.InitiallyDockThePropertyMenu();
  }
	$('#propertiespopupmenu').show();

	if (FMDrawPropertyMenu.FMDrawPropertiesMenuDockSetting != null && FMDrawPropertyMenu.FMDrawPropertiesMenuDockSetting.docked != null)
	{
		var dockAreaWidth = $('#propertiespopupmenu').width() + 1;

		if ( FMDrawPropertyMenu.FMDrawPropertiesMenuDockSetting.dockedLocation === "RIGHT" )
		{
			$("#docking-right").css({ 'width': dockAreaWidth + "px" });
		}

		if (FMDrawPropertyMenu.FMDrawPropertiesMenuDockSetting.dockedLocation === "LEFT")
		{
			// This if for another iteration when the left side docking is resizable.
		}
	}
}

FMDrawPropertyMenu.SetFocusOnDefaultControl = function()
{
    if ( FMDrawPropertyMenu.defaultControlFocusIds &&
        FMDrawPropertyMenu.defaultControlFocusIds instanceof Array &&
        FMDrawPropertyMenu.defaultControlFocusIds.length > 0)
    {
        FMDrawPropertyMenu.defaultControlFocusIds.forEach( function( id )
        {
            if ( $( '#' + id ).length > 0 ) //Check if control ID exists in DOM
            {
                var isVisible = $( '#' + id ).is( ':visible' );
                var isHidden = $( '#' + id ).is( ':hidden' );
                if ( isVisible && !isHidden )
                {
                    $( '#' + id ).focus();
                }
            }
        } );
    }

    FMDrawPropertyMenu.defaultControlFocusIds = [];
}


FMDrawPropertyMenu.isObjectGroup = function( object )
{
    return ( object instanceof go.Group );
};
FMDrawPropertyMenu.getFirstNonGroupObject = function( group )
{
    if ( !FMDrawPropertyMenu.isObjectGroup( group ) )
    {
        return group;
    }
    var object = group.memberParts.first();
    if ( FMDrawPropertyMenu.isObjectGroup( object ) )
    {
        object = FMDrawPropertyMenu.getFirstNonGroupObject( object );
    }
    return object;
}; //====================================================================
// This function will initialize the Properties menu properties 
// based on the selected object.
//====================================================================
FMDrawPropertyMenu.InitiatizePropertiesMenu = function()
{
	var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();

    if ( !canvas )
    {
        return;
    }

    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    FMDrawPropertyMenu.selectedObjects = FMDrawIndex.GetSelectedObjects( canvas );
    FMDrawPropertyMenu.PropertyActiveObject = null;

    // In a group selection always choose the first selection as the one to
    // populate the property window.
    if ( FMDrawPropertyMenu.selectedObjects.length === 1 )
    {
        if ( FMDrawPropertyMenu.isObjectGroup( FMDrawPropertyMenu.selectedObjects[0] ) )
        {
            FMDrawPropertyMenu.PropertyActiveObject = FMDrawPropertyMenu.getFirstNonGroupObject( FMDrawPropertyMenu.selectedObjects[0] );
            FMDrawPropertyMenu.MultiSelectionFlag = true;
        }
        else
        {
            FMDrawPropertyMenu.PropertyActiveObject = FMDrawPropertyMenu.selectedObjects[0];
            FMDrawPropertyMenu.InitMultiSelectionManualEntryFlags();
            FMDrawPropertyMenu.MultiSelectionFlag = false;
        }
    }
    else if ( FMDrawPropertyMenu.selectedObjects.length > 1 )
    {
        FMDrawPropertyMenu.PropertyActiveObject = FMDrawPropertyMenu.getFirstNonGroupObject( canvas.selection.first() );
        FMDrawPropertyMenu.MultiSelectionFlag = true;
    }

    if ( !FMDrawPropertyMenu.PropertyActiveObject )
    {
        // Show properties of canvas itself
        FMDrawPropertyMenu.InitCanvasProperties();
    }
    else
    {
        // Create recent color data structure in the model if one does
        // not exist.
    	FMDrawPropertyMenu.CreateModelRecentColorObject();

    	// Collapse the control section.  It should only be opened when the the Bar, Tag, or Button control is
		// selected.
    	FMDrawPropertyMenu.SectionExpandCollapseForce( "Section-Controls", "collapse" );

        switch ( FMDrawPropertyMenu.PropertyActiveObject.category )
        {
            case 'rectangle':
                FMDrawPropertyMenu.InitRectProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
            case 'circle':
                FMDrawPropertyMenu.InitCircleProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
            case 'triangle':
                FMDrawPropertyMenu.InitTriangleProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
            case 'ellipse':
                FMDrawPropertyMenu.InitEllipseProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
            case 'bar':
                FMDrawPropertyMenu.InitBarProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
            case 'polygon':
                FMDrawPropertyMenu.InitPolygonProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
            case 'tag':
                FMDrawPropertyMenu.InitTagProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
            case 'line':
                FMDrawPropertyMenu.InitLineProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
				case 'lineLink':
                FMDrawPropertyMenu.InitLineLinkProperties(FMDrawPropertyMenu.PropertyActiveObject);
                break;
            case 'polyline':
                FMDrawPropertyMenu.InitPolylineProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
            case 'text':
                FMDrawPropertyMenu.InitTextProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
            case 'picture':
                FMDrawPropertyMenu.InitPictureProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
        	case 'button':
        		FMDrawPropertyMenu.InitButtonProperties( FMDrawPropertyMenu.PropertyActiveObject );
                break;
        }
    }
};


//=========================================================
// This function will initialize the default property preview
//=========================================================
FMDrawPropertyMenu.InitializePropertyPreview = function()
{
	var diagram = $$(go.Diagram, "CommonPropertyPreview", {
		        "panningTool.isEnabled" : false,
        "dragSelectingTool.isEnabled": false
	});
	diagram.hasHorizontalScrollbar = false;
	diagram.hasVerticalScrollbar = false;
	diagram.autoScale = go.Diagram.Uniform;
	FMDrawPropertyMenu.propertyPreviewDiagram = diagram;
	diagram.nodeTemplate =
	  $$(go.Node, "Auto", new go.Binding('position', 'pos', go.Point.parse), new go.Binding('selectable', 'selectable'),  // the Shape automatically fits around the TextBlock
		 $$(go.Shape, "rectangle",  // use this kind of figure for the Shape
			// bind Shape.fill to Node.data.color
			new go.Binding( 'desiredSize', 'size', go.Size.parse ), new go.Binding( 'fill', 'color', go.Brush.parse ), new go.Binding( 'desiredSize', 'size', go.Size.parse ).makeTwoWay( go.Size.stringify ), new go.Binding( 'strokeWidth', 'strokeWidth' ), new go.Binding( 'stroke', 'lineStroke' ).makeTwoWay(), new go.Binding( 'strokeDashArray', 'strokeDashArray' )),
		 $$(go.TextBlock,
			{ margin: 1 },  // some room around the text
			// bind TextBlock.text to Node.data.key
			new go.Binding("text", "text"), new go.Binding('isUnderline'), new go.Binding('stroke'), new go.Binding('font'))
	  );
	var data = { key: "A", pos: "0 0", color: "#99ccff", size: "25 25", text: "A", strokeWidth: 2, selectable: false, font: '18px sans-serif', };
	diagram.model.nodeDataArray = [data];
	FMDrawPropertyMenu.propertyPreviewNode = diagram.findPartForData(data);

	var parent = document.getElementById("CommonPropertyPreview");
	var canvas = parent.getElementsByTagName("canvas");
	canvas[0].style.outline='0';
}

//=========================================================
// This function will clear the properties on the menus.
//=========================================================
FMDrawPropertyMenu.ClearPropertyWindow = function( )
{
    $( '#propertiesPopupMenu-table > div > div > input' ).each( function()
    {
        $( this ).val( '' );
    } );

    $( '#dropdown-propertiesMenu-TEXTFONT option[data-value=\'NONE\']' ).prop( 'selected', true );
    $( '#dropdown-propertiesMenu-TEXTSIZE option[data-value=\'NONE\']' ).prop( 'selected', true );
    $( '#dropdown-propertiesMenu-TEXTSTYLE option[data-value=\'NONE\']' ).prop( 'selected', true );
    $( '#dropdown-propertiesMenu-TEXTUNDERLINE option[data-value=\'NONE\']' ).prop( 'selected', true );
    $( '#dropdown-propertiesMenu-TEXTALIGNMENT option[data-value=\'NONE\']' ).prop( 'selected', true );
    $( '#dropdown-propertiesMenu-TEXTPOSITION option[data-value=\'NONE\']' ).prop( 'selected', true );
    $( '#dropdown-propertiesMenu-TEXTBLOCKALIGNMENT option[data-value=\'NONE\']' ).prop( 'selected', true );
    $( '#dropdown-propertiesMenu-TAGUNITS option[data-value=\'FM_NONE\']' ).prop( 'selected', true );
    $( '#dropdown-propertiesMenu-LINESIZE option[data-value=\'NONE\']' ).prop( 'selected', true );
    $( '#dropdown-propertiesMenu-BUTTONACTIONTYPE option[data-value=\'' + ButtonActionTypeNoneValue + '\']' ).prop( 'selected', true );
    $('#tr-propertiesMenu-BUTTONACTIONTARGET > div > label').text(ButtonActionTargetDefaultLabel);
    $('#dropdown-propertiesMenu-LAYER')[0].selectedIndex = -1;


    // Clear the Color and Pattern dropdowns
    var rgbObj = FMDrawPropertyMenu.HexToRgb( '#ffffff' );
    var rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, 1 );
    $( '#textbox-propertiesMenu-FILLCOLOR' ).css( 'background-color', rgbaStr );
    $( '#textbox-propertiesMenu-PATTERNCOLOR' ).css( 'background-color', rgbaStr );
    $( '#textbox-propertiesMenu-BGFILLCOLOR' ).css( 'background-color', rgbaStr );
    $( '#textbox-propertiesMenu-LINECOLOR' ).css( 'background-color', rgbaStr );
    $( '#textbox-propertiesMenu-TEXTCOLOR' ).css( 'background-color', rgbaStr );
    FMDrawPatternPalette.CreatePattern( 'canvas-propertiesMenu-FILLPATTERN', 1 );
    FMDrawPatternPalette.CreateLineStylePattern( 'canvas-propertiesMenu-LINESTYLE', 1 );
    FMDrawPatternPalette.CreateToArrowPattern( 'canvas-propertiesMenu-LINETOARROW', 1 );
    FMDrawPatternPalette.CreateFromArrowPattern( 'canvas-propertiesMenu-LINEFROMARROW', 1 );

    // Collapses the color palettes.
    var spectrumTagIdList = [];
    spectrumTagIdList.push( 'td-fillColorSpectrum-propertiesMenu-FILLCOLORSPECTRUM' );
    spectrumTagIdList.push( 'td-fillColorSpectrum-propertiesMenu-LINECOLORSPECTRUM' );
    spectrumTagIdList.push( 'td-fillColorSpectrum-propertiesMenu-TEXTCOLORSPECTRUM' );
    spectrumTagIdList.push( 'td-fillColorSpectrum-propertiesMenu-BGFILLCOLORSPECTRUM' );
    spectrumTagIdList.push( 'td-fillColorSpectrum-propertiesMenu-PATTERNCOLORSPECTRUM' );

    for ( var nextSpectrum = 0; nextSpectrum < 5; nextSpectrum++ )
    {
        var divSpectrum = document.getElementById( spectrumTagIdList[nextSpectrum] );

        if ( divSpectrum != null )
        {
            divSpectrum.style.display = 'none';
        }
    }

    // Collapses the pattern palettes.
    var patternTagIdList = [];
    patternTagIdList.push( 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' );
    patternTagIdList.push( 'td-fillPatternPalette-propertiesMenu-LINESTYLEPALETTE' );
    patternTagIdList.push( 'td-fillPatternPalette-propertiesMenu-LINETOARROWPALETTE' );
    patternTagIdList.push( 'td-fillPatternPalette-propertiesMenu-LINEFROMARROWPALETTE' );

    for ( var nextPattern = 0; nextPattern < 4; nextPattern++ )
    {
        var divPattern = document.getElementById( patternTagIdList[nextPattern] );

        if ( divPattern != null )
        {
            divPattern.style.display = 'none';
        }
    }
};

//=========================================================
// This function will initialize the properties menu common
// properties.
//=========================================================
FMDrawPropertyMenu.InitCommonProperties = function( obj )
{
    if ( obj.data == null )
    {
        return;
    }

    FMDrawPropertyMenu.SetTopCoordinate( obj );
    FMDrawPropertyMenu.SetLeftCoordinate( obj );
    FMDrawPropertyMenu.SetAngle( obj );
    FMDrawPropertyMenu.SetWidth( obj );
    FMDrawPropertyMenu.SetHeight( obj );
    FMDrawPropertyMenu.SetTextPosition( obj );

    var textBlock = obj.findObject( 'TEXTBLOCK' );

    if ( textBlock != null )
    {
        FMDrawPropertyMenu.ParseFontString( textBlock.font );
        FMDrawPropertyMenu.SetTextAlignmentDropdown( textBlock.textAlign );
        FMDrawPropertyMenu.SetTextUnderlineDropdown( textBlock.isUnderline );
        FMDrawPropertyMenu.SetTextStyleDropdown();
        FMDrawPropertyMenu.SetTextSizeDropdown();
        FMDrawPropertyMenu.SetTextFontDropdown();
        FMDrawPropertyMenu.SetTextColorDropdown( textBlock.stroke );
        FMDrawPropertyMenu.SetTextBlockPositionDropdown();
        FMDrawPropertyMenu.SetTextBlockAlignmentDropdown();
    }

    FMDrawPropertyMenu.SetLayerDropdown();
    FMDrawPropertyMenu.SetZorder( obj );
    FMDrawPropertyMenu.SetTransparency();
    FMDrawPropertyMenu.SetBackgroundTransparency();
    FMDrawPropertyMenu.SetLineSizeDropdown( obj.data.strokeWidth, textBlock );
    FMDrawPropertyMenu.SetLineStyleTransparency();
    FMDrawPropertyMenu.SetLineStyleDropdown();

    // The color dropdowns must be set prior to the pattern down
    // being set.
    FMDrawPropertyMenu.SetLineColorDropdown();
    FMDrawPropertyMenu.SetFillColorDropdown();
    FMDrawPropertyMenu.SetBgFillColorDropdown();
    FMDrawPropertyMenu.SetPatternColorDropdown();
    FMDrawPropertyMenu.SetFillPatternDropdown();

    // Clear line arrow dropdowns.
    FMDrawPatternPalette.CreateToArrowPattern( 'canvas-propertiesMenu-LINETOARROW', 1 );
    FMDrawPatternPalette.CreateFromArrowPattern( 'canvas-propertiesMenu-LINEFROMARROW', 1 );
};

//=========================================================
// This function will initialize the tag specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitTagProperties = function( object )
{
    if ( !object || !object.data )
    {
        return;
    }

    //Init common properties
    FMDrawPropertyMenu.InitCommonProperties(object);

	// Expand the control section for the tag properties.
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false )
    {
    	// This call displays the tag fields in the Control
    	// Section. Note: if adding new tag fields, this function
		// must be updated.
    	FMDrawPropertyMenu.ControlSectionFilter("tag", "show");
    	
        //POINTANDTAGID
    	FMDrawPropertyMenu.SetPointAndTagID();

        //ANIMATIONBUTTON
    	FMDrawPropertyMenu.SetAnimationID();

        //TAGWIDTH
        var tagWidth = object.data.TagFieldWidth;
        $( '#textbox-propertiesMenu-TAGWIDTH' ).val( tagWidth );

        //TAGPRECISION
        var precision = object.data.TagPrecision;
        $( '#textbox-propertiesMenu-TAGPRECISION' ).val( precision );
        FMDrawPropertyMenu.SetTagUnitsDropdown();

        //TAGFIELD
        FMDrawPropertyMenu.SetSelectionForDropdown( 'TAGFIELD', 'TagFieldSelection' );
        FMDrawPropertyMenu.UpdateSuperscriptAndSubScriptShow();
    }
    else
    {
        var relatedResponse = FMDrawPropertyMenu.AllSelectedObjectsAreTagRelated( FMDrawIndex.GetSelectedObjects( FMDrawIndex.GetActiveTabGoJSDiagramObject() ) );
        if ( relatedResponse === SelectedObjectsAreAllTagsAndAllRelated )
        {
        	var fieldList = [];
        	fieldList.push("#tr-propertiesMenu-POINTID"); 
	        FMDrawPropertyMenu.ShowIndividualControlSectionFilter( fieldList );
            FMDrawPropertyMenu.SetPointID();
        }
        else {
            FMDrawPropertyMenu.ControlSectionFilter("tag", "hide", relatedResponse);
        }
    }
};

FMDrawPropertyMenu.IteratorToArray = function( iter )
{
    var arr = [];
    iter.each( function( node )
    {
        arr.push( node );
    } );
    return arr;
};
FMDrawPropertyMenu.AllSelectedObjectsAreTagRelated = function( selectedObjects)
{
    //var pointTemplateObjectSelected = pointTemplateObjectSelectedParam;
    for ( var nextObjIndex = 0; nextObjIndex < selectedObjects.length; nextObjIndex++ )
    {
        var node = selectedObjects[nextObjIndex];
        if ( !node /*|| node == null (Resharper flags this as warning.  == expression is always false since ! expression takes care of this)*/ )
        {
            return SelectedObjectsInvalidInformation;
        }
        if ( FMDrawPropertyMenu.isObjectGroup( node ) )
        {
            var arr = FMDrawPropertyMenu.IteratorToArray( node.memberParts );
            var ret = FMDrawPropertyMenu.AllSelectedObjectsAreTagRelated( arr );
            if ( ret !== SelectedObjectsAreAllTagsAndAllRelated )
            {
                return ret;
            }
        }
        else
        {
            var data = node.data;
            if ( !data /* || data == null (Resharper flags this as warning.  == expression is always false since ! expression takes care of this)*/ )
            {
                return SelectedObjectsInvalidInformation;
            }
            var category = data.category;
            if ( !category /* || category == null (Resharper flags this as warning.  == expression is always false since ! expression takes care of this)*/ )
            {
                return SelectedObjectsInvalidInformation;
            }

            if (data.PointTemplateTagSelectionIndicator)
            {
                return SelectedObjectsContainsAPointTemplateTag;
            }
           
            switch ( category )
            {
                case 'bar':
                    continue;
                case 'tag':
                    continue;
                case 'button':
                    if ( !data.TagTagID || data.TagTagID == null )
                    {
                        return SelectedObjectsAreAllTagsButNotAllRelated;
                    }
                    continue;
                default:
                    return SelectedObjectsAreNotAllTags;
            }
        }
    }
    return SelectedObjectsAreAllTagsAndAllRelated;
};

//=========================================================
// This function will initialize the rectangle specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitRectProperties = function( object )
{
    FMDrawPropertyMenu.InitCommonProperties( object );
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");
    FMDrawPropertyMenu.HideAllControlSectionFilter();
};

//=========================================================
// This function will initialize the circle specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitCircleProperties = function( object )
{
    FMDrawPropertyMenu.InitCommonProperties(object);
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");
    FMDrawPropertyMenu.HideAllControlSectionFilter();
};

//=========================================================
// This function will initialize the triangle specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitTriangleProperties = function( object )
{
    FMDrawPropertyMenu.InitCommonProperties( object );
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");
    FMDrawPropertyMenu.HideAllControlSectionFilter();
};

//=========================================================
// This function will initialize the ellipse specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitEllipseProperties = function( object )
{
    FMDrawPropertyMenu.InitCommonProperties( object );
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");
    FMDrawPropertyMenu.HideAllControlSectionFilter();
};

//=========================================================
// This function will initialize the polygon specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitPolygonProperties = function( object )
{
    FMDrawPropertyMenu.InitCommonProperties( object );
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");
    FMDrawPropertyMenu.HideAllControlSectionFilter();
};

//=========================================================
// This function will initialize the line specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitLineProperties = function( object )
{
    FMDrawPropertyMenu.InitCommonProperties( object );
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");
    FMDrawPropertyMenu.HideAllControlSectionFilter();

    if ( FMDrawPropertyMenu.PropertyActiveObject == null )
    {
        return;
    }

    var nextIndex;
    var data = FMDrawPropertyMenu.PropertyActiveObject.data;

    if ( data != null )
    {
        if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetLineFromArrowFlag )
        {
            if ( data.fromArrow == null || data.fromArrow === 'None' || data.fromArrow === '' )
            {
                FMDrawPatternPalette.CreateFromArrowPattern( 'canvas-propertiesMenu-LINEFROMARROW', 1 );
            }
            else
            {
                for ( nextIndex = 1; nextIndex < FMDrawPropertyMenu.LineArrowPatternCount; nextIndex++ )
                {
                    if ( FMDrawPatternPalette.FromArrowNames[nextIndex] === data.fromArrow )
                    {
                        FMDrawPatternPalette.CreateFromArrowPattern( 'canvas-propertiesMenu-LINEFROMARROW', nextIndex + 1 );
                        break;
                    }
                }
            }
        }

        if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetLineToArrowFlag )
        {
            if ( data.toArrow == null || data.toArrow === 'None' || data.toArrow === '' )
            {
                FMDrawPatternPalette.CreateToArrowPattern( 'canvas-propertiesMenu-LINETOARROW', 1 );
            }
            else
            {
                for ( nextIndex = 1; nextIndex < FMDrawPropertyMenu.LineArrowPatternCount; nextIndex++ )
                {
                    if ( FMDrawPatternPalette.ToArrowNames[nextIndex] === data.toArrow )
                    {
                        FMDrawPatternPalette.CreateToArrowPattern( 'canvas-propertiesMenu-LINETOARROW', nextIndex + 1 );
                        break;
                    }
                }
            }
        }
    }

    $( '#textbox-propertiesMenu-WIDTH' ).val( ' ' );
    $( '#textbox-propertiesMenu-HEIGHT' ).val( ' ' );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetWidthFlag )
    {
        //If Panel is bound to a resizing object then use that object's width
        var width = ( object.resizeObject ) ? object.resizeObject.width : object.width;
        var roundedWidth = Math.round( width );

        if ( isNaN( roundedWidth ) )
        {
            $( '#textbox-propertiesMenu-WIDTH' ).val( '0' );
        }
        else
        {
            $( '#textbox-propertiesMenu-WIDTH' ).val( roundedWidth );
        }
    }

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetHeightFlag )
    {
        //If Panel is bound to a resizing object then use that object's height
        var height = ( object.resizeObject ) ? object.resizeObject.height : object.height;
        var roundedHeight = Math.round( height );

        if ( isNaN( roundedHeight ) )
        {
            $( '#textbox-propertiesMenu-HEIGHT' ).val( '0' );
        }
        else
        {
            $( '#textbox-propertiesMenu-HEIGHT' ).val( roundedHeight );
        }
    }
};

//=========================================================
// This function will initialize the line specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitLineLinkProperties = function (object) {
	FMDrawPropertyMenu.InitCommonProperties(object);
	FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");
	FMDrawPropertyMenu.HideAllControlSectionFilter();
	FMDrawPropertyMenu.HideLinkCommonProperties();

	if (FMDrawPropertyMenu.PropertyActiveObject == null) {
		return;
	}

	var nextIndex;
	var data = FMDrawPropertyMenu.PropertyActiveObject.data;

	if (data != null) {
		if (FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetLineFromArrowFlag) {
			if (data.fromArrow == null || data.fromArrow === 'None' || data.fromArrow === '') {
				FMDrawPatternPalette.CreateFromArrowPattern('canvas-propertiesMenu-LINEFROMARROW', 1);
			}
			else {
				for (nextIndex = 1; nextIndex < FMDrawPropertyMenu.LineArrowPatternCount; nextIndex++) {
					if (FMDrawPatternPalette.FromArrowNames[nextIndex] === data.fromArrow) {
						FMDrawPatternPalette.CreateFromArrowPattern('canvas-propertiesMenu-LINEFROMARROW', nextIndex + 1);
						break;
					}
				}
			}
		}

		if (FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetLineToArrowFlag) {
			if (data.toArrow == null || data.toArrow === 'None' || data.toArrow === '') {
				FMDrawPatternPalette.CreateToArrowPattern('canvas-propertiesMenu-LINETOARROW', 1);
			}
			else {
				for (nextIndex = 1; nextIndex < FMDrawPropertyMenu.LineArrowPatternCount; nextIndex++) {
					if (FMDrawPatternPalette.ToArrowNames[nextIndex] === data.toArrow) {
						FMDrawPatternPalette.CreateToArrowPattern('canvas-propertiesMenu-LINETOARROW', nextIndex + 1);
						break;
					}
				}
			}
		}
	}

	$('#textbox-propertiesMenu-WIDTH').val(' ');
	$('#textbox-propertiesMenu-HEIGHT').val(' ');

	if (FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetWidthFlag) {
	
			var width = object.data.width;


			if (isNaN(width)) {
			$('#textbox-propertiesMenu-WIDTH').val('0');
		}
		else {
			$('#textbox-propertiesMenu-WIDTH').val(width);
		}
	}

	if (FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetHeightFlag) {
		//If Panel is bound to a resizing object then use that object's height
		var height = (object.resizeObject) ? object.resizeObject.height : object.height;
		var roundedHeight = Math.round(height);

		if (isNaN(roundedHeight)) {
			$('#textbox-propertiesMenu-HEIGHT').val('0');
		}
		else {
			$('#textbox-propertiesMenu-HEIGHT').val(roundedHeight);
		}
	}
};

//=========================================================
// This function hides N/A properties for line link
//=========================================================
FMDrawPropertyMenu.HideLinkCommonProperties = function () {
	$("#tr-propertiesMenu-HEIGHT").hide();
	$("#tr-propertiesMenu-ANGLE").hide();
}

//=========================================================
// This function restores all main section properties
//=========================================================
FMDrawPropertyMenu.ShowMainSectionCommonProperties = function () {
	$("#tr-propertiesMenu-HEIGHT").show();
	$("#tr-propertiesMenu-ANGLE").show();
}

//=========================================================
// This function will initialize the polyline specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitPolylineProperties = function( object )
{
    FMDrawPropertyMenu.InitCommonProperties( object );
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");
    FMDrawPropertyMenu.HideAllControlSectionFilter();
};

//=========================================================
// This function will initialize the text specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitTextProperties = function( object )
{
    FMDrawPropertyMenu.InitCommonProperties( object );
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");
    FMDrawPropertyMenu.HideAllControlSectionFilter();
};

//=========================================================
// This function will initialize the Picture specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitPictureProperties = function( object )
{
    FMDrawPropertyMenu.InitCommonProperties( object );
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");
    FMDrawPropertyMenu.HideAllControlSectionFilter();
};

//=========================================================
// This function will initialize the Button specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitButtonProperties = function( object )
{
    FMDrawPropertyMenu.InitCommonProperties( object );

    if (object && object.data)
    {
    	// Expand the control section for the button properties.
    	FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");

        if ( FMDrawPropertyMenu.MultiSelectionFlag )
        {
            if ( object.data.TagGUID && object.data.TagGUID !== null )
            {
                var relatedResponse = FMDrawPropertyMenu.AllSelectedObjectsAreTagRelated( FMDrawIndex.GetSelectedObjects( FMDrawIndex.GetActiveTabGoJSDiagramObject() ) );
                if ( relatedResponse === SelectedObjectsAreAllTagsAndAllRelated )
                {
                    var fieldList = [];
                    fieldList.push( "#tr-propertiesMenu-POINTID" );
                    FMDrawPropertyMenu.ShowIndividualControlSectionFilter( fieldList );

                    FMDrawPropertyMenu.SetPointID();
                }
                else
                {
                    FMDrawPropertyMenu.ControlSectionFilter("tag", "hide", relatedResponse);
                }
            }
        }
        else
        {
        	// This call displays the button fields in the Control
        	// Section. Note: if adding new button fields, this function
        	// must be updated.
        	FMDrawPropertyMenu.ControlSectionFilter("button", "show");
            FMDrawPropertyMenu.SetButonActionValues( object.data );
        }
    }
};

//=========================================================
// This function will initialize the bar specific
// properties on the menus.
//=========================================================
FMDrawPropertyMenu.InitBarProperties = function( object )
{
    if ( object.data == null )
    {
        return;
    }

	// Expand the control section for the Bar properties.
    FMDrawPropertyMenu.SectionExpandCollapseForce("Section-Controls", "expand");

    FMDrawPropertyMenu.InitCommonProperties( object );
    FMDrawPropertyMenu.SetDemoPercent();
    FMDrawPropertyMenu.SetBarType();
    FMDrawPropertyMenu.SetUseProductColor();
    FMDrawPropertyMenu.SetUseAlarmLevel();
    FMDrawPropertyMenu.SetPointAndTagID();
    FMDrawPropertyMenu.SetAnimationID();
    FMDrawPropertyMenu.SetPointID();
    FMDrawPropertyMenu.SetUserVariableLimits();
    FMDrawPropertyMenu.SetMinimumValue();
    FMDrawPropertyMenu.SetMaximumValue();

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false )
    {
    	// This call displays the bar fields in the Control
    	// Section. Note: if adding new bar fields, this function
    	// must be updated.
    	FMDrawPropertyMenu.ControlSectionFilter("bar", "show");
    }
    else
    {
        var relatedResponse = FMDrawPropertyMenu.AllSelectedObjectsAreTagRelated( FMDrawIndex.GetSelectedObjects( FMDrawIndex.GetActiveTabGoJSDiagramObject() ) );
        if ( relatedResponse === SelectedObjectsAreAllTagsAndAllRelated )
        {
            var fieldList = [];
            fieldList.push( "#tr-propertiesMenu-POINTID" );
            FMDrawPropertyMenu.ShowIndividualControlSectionFilter( fieldList );
        }
        else
        {
            FMDrawPropertyMenu.ControlSectionFilter("tag", "hide", relatedResponse);
        }
    }
};

//============================================================================
// This method will set the updated property on the active object.
//============================================================================
FMDrawPropertyMenu.SetPropertyOnObject = function( propertyName, propertyObjectId )
{
    
    if ( FMDrawPropertyMenu.PropertyActiveObject == null )
    {
        return;
    }

    var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();

    var newValue;
    var transFormedPosition;
    var newPosition;
    var selectedValue;
    var fontStr;
	 var fontStrNoSize;
    var obj = null;
    var textBlockPosition;
    var textBlockAlignment;
    var errFlag;
    var dataModel;
    var tagPrecision;
    var tagUnits;
    var tagUnitsSelected;
    var tagUnitsSelectedInt;
    var tagWidth;
    var minNewValueWidth;
    var wholeNumberWidth;
    var errMsg;
    var alertTitle = 'Input Error';
    //var newPointsList;  //Resharper found that this variable is never used.  Therefore it has been commented out.  It will be removed if peer code reviewer confirms.  Search was done across all *.js;*.cs;*.cshtml files.
    var nextObjIndex;
    var activeObject;
    var newColor;
    var transparencyValue;
    var rgbObj;
    var rgbStr;
    var patternNumber;
    var patternTagId;

    var propertyValue = $( '#' + propertyObjectId ).val();

    switch ( propertyName )
    {
        case 'TOP':
            // During multi select, if the property value is empty, then return.
            if ( FMDrawPropertyMenu.MultiSelectionFlag && FMDrawPropertyMenu.IsPropertyValueNullOrEmpty( propertyValue ) )
            {
                return;
            }

            FMDrawPropertyMenu.IgnoreEvent = true;
            newValue = parseFloat( propertyValue );
            errFlag = true;

            if ( newValue !== 'NaN' && newValue >= 0 && newValue <= 1700 )
            {
                dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

                for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                {
                    canvas.startTransaction( 'propertyUpdate' );
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                    canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();

                    transFormedPosition = activeObject.position;
                    newPosition = new go.Point(transFormedPosition.x, Math.round(newValue));

                    if (newPosition.y === Math.round(activeObject.position.y)) {
                    	canvas.rollbackTransaction('propertyUpdate');
                    	continue;
                    }

                    activeObject.position = newPosition;
                    if (activeObject.data.category == "lineLink")
                    	activeObject.move(newPosition);

                    $( '#textbox-propertiesMenu-TOP' ).val( Math.round( newValue ) );
                    canvas.commitTransaction( 'propertyUpdate' );
                }

                FMDrawPropertyMenu.manualSetTopFlag = true;
                FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
                errFlag = false;
            }

            FMDrawPropertyMenu.IgnoreEvent = false;

            if ( errFlag )
            {
                errMsg = 'Invalid Top coordinate.  Must be numeric and between 0 - 1700.';
                FMLayout.Alert( errMsg, alertTitle, null );
                for (nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++) {
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                    if (activeObject != null) {
                        canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
                        FMDrawPropertyMenu.SetTopCoordinate(activeObject);
                    }
                }
            }
            break;
        case 'LEFT':
            // During multi select, if the property value is empty, then return.
            if ( FMDrawPropertyMenu.MultiSelectionFlag && FMDrawPropertyMenu.IsPropertyValueNullOrEmpty( propertyValue ) )
            {
                return;
            }

            FMDrawPropertyMenu.IgnoreEvent = true;
            newValue = parseFloat( propertyValue );
            errFlag = true;

            if ( newValue !== 'NaN' && newValue >= 0 && newValue <= 1700 )
            {
                dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

                for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                {
                    canvas.startTransaction( 'propertyUpdate' );
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                    canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
                    transFormedPosition = activeObject.position;
                    newPosition = new go.Point(Math.round(newValue), transFormedPosition.y);

                    if (newPosition.x === Math.round(activeObject.position.x)) {
                    	canvas.rollbackTransaction('propertyUpdate');
                    	continue;
                    }

                    activeObject.position = newPosition;
                    if (activeObject.data.category == "lineLink")
                    	activeObject.move(newPosition);

                    $( '#textbox-propertiesMenu-LEFT' ).val( Math.round( newValue ) );
                    canvas.commitTransaction( 'propertyUpdate' );
                }

                FMDrawPropertyMenu.manualSetLeftFlag = true;
                FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
                errFlag = false;
            }

            FMDrawPropertyMenu.IgnoreEvent = false;

            if ( errFlag )
            {
                errMsg = 'Invalid Left coordinate.  Must be numeric and between 0 - 1700.';
                FMLayout.Alert( errMsg, alertTitle, null );
                for (nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++) {
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                    if (activeObject != null) {
                        canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();
                        FMDrawPropertyMenu.SetLeftCoordinate(activeObject);
                    }
                }
            }
            break;
        case 'WIDTH':
            // During multi select, if the property value is empty, then return.
            if ( FMDrawPropertyMenu.MultiSelectionFlag && FMDrawPropertyMenu.IsPropertyValueNullOrEmpty( propertyValue ) )
            {
                return;
            }

            FMDrawPropertyMenu.IgnoreEvent = true;
            newValue = parseFloat( propertyValue );
            errFlag = true;

            if ( newValue !== 'NaN' && newValue > 0 && newValue <= 1700 )
            {
                for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                {
                    canvas.startTransaction( 'propertyUpdate' );
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                    if (activeObject.data.category == "lineLink") {
                    	activeObject.diagram.model.setDataProperty(activeObject.data, 'width', math.round(newValue));
                    }
                    else {
                    	//If the Panel Object is bound to a resizing object, then set that object's width
                    	obj = (activeObject.resizeObject) ? activeObject.findObject(activeObject.resizeObjectName) : activeObject;

                    	if (obj == null) {
                    		obj = activeObject;
                    	}

                    	if (obj.width === math.round(newValue)) {
                    		canvas.rollbackTransaction('propertyUpdate');
                    		continue;
                    	}
                    	obj.width = Math.round(newValue);

                    	// For a circle we need to ensure the both width and height have the same value.
                    	if (activeObject.category === 'circle') {
                    		obj.height = Math.round(newValue);
                    	}
                    }

                    $( '#textbox-propertiesMenu-WIDTH' ).val( math.round( newValue ) );
                    if (activeObject.category === 'line') {
                        var convertedAngle = FMDrawPropertyMenu.setangleBasedOnGeometry(activeObject, newValue, false);
                        $('#textbox-propertiesMenu-ANGLE').val(Math.round(convertedAngle));
                    }
                    canvas.commitTransaction('propertyUpdate');
                }

                obj = null;
                FMDrawPropertyMenu.manualSetWidthFlag = true;
                FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
                errFlag = false;
            }

            FMDrawPropertyMenu.IgnoreEvent = false;
            
            if ( errFlag )
            {
            	errMsg = 'Invalid Width.  Must be numeric and between 1 - 1700.';
            	FMLayout.Alert(errMsg, alertTitle, null);
            	for (nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++) {
            		activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
            		//If the Panel Object is bound to a resizing object, then set that object's height
            		obj = (activeObject.resizeObject) ? activeObject.findObject(activeObject.resizeObjectName) : activeObject;

            		if (obj == null) {
            			obj = activeObject;
            		}
            		if (obj != null) {
            			FMDrawPropertyMenu.SetWidth(obj);
		            }
	            }
            }
            else
            {
            	for (nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++)
            	{
            		obj = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
            		//If the Panel Object is bound to a resizing object, then set that object's height
            		if (obj != null)
            		{
            			FMDrawPropertyMenu.SetLeftCoordinateForce(obj.position.x);
            			// For a circle we need to ensure the both width and height have the same value.
            			if (activeObject.category === 'circle')
            			{
            				FMDrawPropertyMenu.SetTopCoordinateForce(obj.position.y);
            				var height = (obj.resizeObject) ? obj.resizeObject.height : obj.height;
				            FMDrawPropertyMenu.SetHeightForce( height );
			            }
			            obj = null;
		            }
            	}
            }
            
            break;
        case 'HEIGHT':
            // During multi select, if the property value is empty, then return.
        	if ( FMDrawPropertyMenu.MultiSelectionFlag && FMDrawPropertyMenu.IsPropertyValueNullOrEmpty( propertyValue ) )
            {
                return;
            }

            FMDrawPropertyMenu.IgnoreEvent = true;
            newValue = parseFloat( propertyValue );
            errFlag = true;

            if ( newValue !== 'NaN' && newValue > 0 && newValue <= 1700 )
            {
                for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                {
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                    if (activeObject.data.category == "lineLink")
                    { return;}
                    canvas.startTransaction( 'propertyUpdate' );

                    //If the Panel Object is bound to a resizing object, then set that object's height
                    obj = ( activeObject.resizeObject ) ? activeObject.findObject( activeObject.resizeObjectName ) : activeObject;

                    if ( obj == null )
                    {
                        obj = activeObject;
                    }

                    if ( obj.height === math.round( newValue ) )
                    {
                        canvas.rollbackTransaction( 'propertyUpdate' );
                        continue;
                    }

                    obj.height = Math.round( newValue );

                    // For a circle we need to ensure the both width and height have the same value.
                    if ( activeObject.category === 'circle' )
                    {
                        obj.width = Math.round( newValue );
                    }

                    if ( activeObject.category === 'bar' )
                    {
                        obj.diagram.model.setDataProperty( activeObject.data, 'maxheight', Math.round( newValue ) );
                    }

                    $( '#textbox-propertiesMenu-HEIGHT' ).val( Math.round( newValue ) );
                    if (activeObject.category === 'line') {
                        var convertedAngle = FMDrawPropertyMenu.setangleBasedOnGeometry(activeObject, newValue,true);
                        $('#textbox-propertiesMenu-ANGLE').val(Math.round(convertedAngle));
                    }
                    canvas.commitTransaction('propertyUpdate');

                }

                FMDrawPropertyMenu.manualSetHeightFlag = true;
                FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
                obj = null;
                errFlag = false;
            }

            FMDrawPropertyMenu.IgnoreEvent = false;

            if ( errFlag )
            {
                errMsg = 'Invalid Height.  Must be numeric and between 1 - 1700.';
                FMLayout.Alert(errMsg, alertTitle, null);
                for (nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++) {
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                    //If the Panel Object is bound to a resizing object, then set that object's height
                    obj = (activeObject.resizeObject) ? activeObject.findObject(activeObject.resizeObjectName) : activeObject;

                    if (obj == null) {
                        obj = activeObject;
                    }
                    if (obj != null) {
                        FMDrawPropertyMenu.SetHeight(obj);
                    }
                }
            }
            else
            {
            	for (nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++)
            	{
            		obj = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
            		//If the Panel Object is bound to a resizing object, then set that object's height
            		if (obj != null)
            		{
            			FMDrawPropertyMenu.SetTopCoordinateForce(obj.position.y);
            			// For a circle we need to ensure the both width and height have the same value.
            			if (activeObject.category === 'circle')
            			{
            				FMDrawPropertyMenu.SetLeftCoordinateForce(obj.position.x);
            				var width = (obj.resizeObject) ? obj.resizeObject.width : obj.width;
            				FMDrawPropertyMenu.SetWidthForce(width);
            			}
            			obj = null;
            		}
            	}
            }
				break;
			case 'ANGLE':
				// During multi select, if the property value is empty, then return.
				if ( FMDrawPropertyMenu.MultiSelectionFlag && FMDrawPropertyMenu.IsPropertyValueNullOrEmpty( propertyValue ) )
            {
                return;
            }

            FMDrawPropertyMenu.IgnoreEvent = true;
            newValue = parseFloat( propertyValue );
            errFlag = true;

            if ( newValue !== 'NaN' )
            {
                if ( newValue <= 360.0 && newValue >= -360.0 )
                {
                    dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
                    errFlag = false;
                    FMDrawPropertyMenu.manualSetAngleFlag = true;

                    for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                    {
                        activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                        // User wants the counter clockwise rotation to be positive and 
                        // GoJS has the positive going clockwise. Therefore, negate the
                        // value.
                        var convertedAngle = Math.round(newValue) * -1;
                        if (activeObject.data.angle == convertedAngle)
                            continue;

                        canvas.startTransaction('propertyUpdate');

                        dataModel.setDataProperty(activeObject.data, 'angle', convertedAngle);

                        $('#textbox-propertiesMenu-ANGLE').val(Math.round(newValue));
                        if (activeObject.category === 'line')
                        {
                        	FMDrawPropertyMenu.SetLineAngle(Math.round(newValue), activeObject);
                        }
                        canvas.commitTransaction( 'propertyUpdate' );

                        //if (math.round(activeObject.data.angle) === convertedAngle)
                        //{
                        //	canvas.rollbackTransaction("propertyUpdate");
                        //	continue;
                        //}
                    }
                }
            }

            FMDrawPropertyMenu.IgnoreEvent = false;

            if ( errFlag )
            {
                errMsg = 'Invalid angle. Must be numeric between -360 and 360.';
                FMLayout.Alert( errMsg, alertTitle, null );
                for (nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++) {
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                    if (activeObject != null) {
                        FMDrawPropertyMenu.SetAngle(activeObject);
                    }
                }
            }
            break;
        case 'LAYER':
            selectedValue = $('#dropdown-propertiesMenu-LAYER').find(':selected').val();
            FMDrawPropertyMenu.IgnoreEvent = true;
            FMDrawIndex.MoveSelectionToLayer(selectedValue);
            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'ZORDER':
            // During multi select, if the property value is empty, then return.
            if ( FMDrawPropertyMenu.MultiSelectionFlag && FMDrawPropertyMenu.IsPropertyValueNullOrEmpty( propertyValue ) )
            {
                return;
            }

            FMDrawPropertyMenu.IgnoreEvent = true;
            newValue = parseInt( propertyValue );
            errFlag = true;

            if ( newValue !== 'NaN' )
            {
                if ( newValue >= -60000 && newValue <= 60000.0 )
                {
                    errFlag = false;
                    dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

                    for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                    {
                        canvas.startTransaction( 'propertyUpdate' );
                        activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                        dataModel.setDataProperty( activeObject.data, 'zOrder', newValue );
                        canvas.commitTransaction( 'propertyUpdate' );
                    }

                    $( '#textbox-propertiesMenu-ZORDER' ).val( newValue );
                }
            }

            FMDrawPropertyMenu.manualSetZOderFlag = true;
            FMDrawPropertyMenu.IgnoreEvent = false;

            if ( errFlag )
            {
                errMsg = 'Invalid Z-Order. Must be numeric between -60,000 and 60,000.';
                FMLayout.Alert( errMsg, alertTitle, null );
            }
            break;
        case 'LINESIZE':
            selectedValue = $( '#dropdown-propertiesMenu-LINESIZE' ).find( ':selected' ).text();
            FMDrawPropertyMenu.IgnoreEvent = true;
            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {

                var lineWeight = selectedValue.replace( 'pt', '' );

                canvas.startTransaction( 'propertyUpdate' );
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                var oldWeight = activeObject.data.strokeWidth;
                // If the selection is NONE, then default to a line weight.
                if ( lineWeight === 'None' )
                {
                    // Do not permit a line to have lineWeight of 0
                    if ( activeObject.category === 'line'
                        || activeObject.category === 'polyline' )
                    {
                        lineWeight = '1';
                    }
                    else
                    {
                        lineWeight = '0';
                    }
                }
                if (activeObject.category === 'line')
                {
                	var offset = (oldWeight - parseInt(lineWeight)) / 2;
                	var offsetForLine = new go.Point(offset, offset);
                	activeObject.move(activeObject.position.copy().add(offsetForLine));
                	var shape = activeObject.findObject('SHAPE');
                	//canvas.toolManager.resizingTool.updateAdornments(shape);
                	//dataModel.setDataProperty(activeObject.data, 'LineStartPositionX', activeObject.data.LineStartPositionX - offset);
                	//dataModel.setDataProperty(activeObject.data, 'LineStartPositionY', activeObject.data.LineStartPositionY - offset);
                	//dataModel.setDataProperty(activeObject.data, 'LineEndPositionX', activeObject.data.LineEndPositionX - offset);
                	//dataModel.setDataProperty(activeObject.data, 'LineEndPositionY', activeObject.data.LineEndPositionY - offset);
                	dataModel.setDataProperty(activeObject.data, 'arrowLineOffset', new go.Point(0, 0));
                	//var newspot = activeObject.locationSpot.copy();
                	//newspot.offsetX = -offset;
                	//newspot.offsetY = -offset;
                	//activeObject.locationSpot = newspot;


                	activeObject.isSelected = false;

                }

                dataModel.setDataProperty( activeObject.data, 'calMargin', parseInt( lineWeight ) );
                dataModel.setDataProperty( activeObject.data, 'fromArrowScale', parseInt( lineWeight ) );
                dataModel.setDataProperty( activeObject.data, 'toArrowScale', parseInt( lineWeight ) );
                dataModel.setDataProperty( activeObject.data, 'strokeWidth', parseInt( lineWeight ) );
                FMDrawIndex.defaultArchetype.fromArrowScale = parseInt( lineWeight );
                FMDrawIndex.defaultArchetype.toArrowScale = parseInt( lineWeight );
                FMDrawIndex.defaultArchetype.strokeWidth = parseInt(lineWeight);
                FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty (FMDrawPropertyMenu.propertyPreviewNode.data,"strokeWidth", parseInt(lineWeight));

                canvas.commitTransaction('propertyUpdate');
                if (activeObject.category === 'line') {
                	activeObject.isSelected = true;
                }
            }

            FMDrawPropertyMenu.manualSetLineSizeFlag = true;
            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'LINESTYLETRANSPARENCY':
            // During multi select, if the property value is empty, then return.
            if ( FMDrawPropertyMenu.MultiSelectionFlag && FMDrawPropertyMenu.IsPropertyValueNullOrEmpty( propertyValue ) )
            {
                return;
            }

            FMDrawPropertyMenu.IgnoreEvent = true;
            newValue = parseInt( propertyValue );
            errFlag = true;

            if ( newValue !== 'NaN' && newValue >= 0 && newValue <= 100 )
            {
                dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
                var currentColor = $( '#manualColor-textbox-propertiesMenu-LINECOLORSPECTRUM' ).val();

                var lineStyleTransparencyValue = FMDrawPropertyMenu.ConvertTransparencyToFloat( propertyValue );
                var rgbObject = FMDrawPropertyMenu.HexToRgb( currentColor );
                var rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObject, lineStyleTransparencyValue );

                FMDrawPropertyMenu.IgnoreEvent = true;

                for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                {
                    canvas.startTransaction( 'propertyUpdate' );
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                    dataModel.setDataProperty( activeObject.data, 'fromArrowStroke', rgbaStr );
                    dataModel.setDataProperty( activeObject.data, 'toArrowStroke', rgbaStr );

                    if ( activeObject.data.fromArrowFill != null )
                    {
                        dataModel.setDataProperty( activeObject.data, 'fromArrowFill', rgbaStr );
                    }

                    if ( activeObject.data.toArrowFill != null )
                    {
                        dataModel.setDataProperty( activeObject.data, 'toArrowFill', rgbaStr );
                    }

                    dataModel.setDataProperty( activeObject.data, 'lineStyleTransparency', propertyValue );
                    dataModel.setDataProperty( activeObject.data, 'lineStroke', rgbaStr );

                    $( '#textbox-propertiesMenu-LINESTYLETRANSPARENCY' ).val( newValue );
                    canvas.commitTransaction( 'propertyUpdate' );
                }
                FMDrawIndex.defaultArchetype.lineStroke = rgbaStr;
                FMDrawIndex.defaultArchetype.lineStyleTransparency = propertyValue;
                FMDrawPropertyMenu.manualSetLineTransparencyFlag = true;
                FMDrawPropertyMenu.IgnoreEvent = false;
                FMDrawPatternPalette.newTransparencyValue = null;

                errFlag = false;
            }

            FMDrawPropertyMenu.IgnoreEvent = false;

            if ( errFlag )
            {
                errMsg = 'Invalid Transparency.  Must be numeric and between 0 - 100.';
                FMLayout.Alert( errMsg, alertTitle, null );
            }
            break;
        case 'TEXTALIGNMENT':
            selectedValue = $( '#dropdown-propertiesMenu-TEXTALIGNMENT' ).find( ':selected' ).text();
            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
            FMDrawPropertyMenu.IgnoreEvent = true;
            var align;

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                canvas.startTransaction( 'propertyUpdate' );
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                if ( selectedValue === 'Left' )
                {
                    dataModel.setDataProperty( activeObject.data, 'textAlign', 'start' );
                    align = 'start';
                }

                if ( selectedValue === 'Center' )
                {
                    dataModel.setDataProperty( activeObject.data, 'textAlign', 'center' );
                    align = 'center';
                }

                if ( selectedValue === 'Right' )
                {
                    dataModel.setDataProperty( activeObject.data, 'textAlign', 'end' );
                    align = 'end';
                }

                canvas.commitTransaction( 'propertyUpdate' );
            }

            FMDrawIndex.defaultArchetype.textAlign = align;


            FMDrawPropertyMenu.manualSetTextJustificatiohFlag = true;
            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'TEXTPOSITION':
            textBlockPosition = $( '#dropdown-propertiesMenu-TEXTPOSITION' ).find( ':selected' ).text();
            textBlockAlignment = $( '#dropdown-propertiesMenu-TEXTBLOCKALIGNMENT' ).find( ':selected' ).text();

            FMDrawPropertyMenu.IgnoreEvent = true;

            if ( FMDrawPropertyMenu.MultiSelectionFlag && textBlockAlignment === 'None' )
            {
                textBlockAlignment = 'Center';
            }

            FMDrawPropertyMenu.SetTextBlockLocation( textBlockPosition, textBlockAlignment, canvas );
            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.manualSetTextBlockPositionFlag = true;

            var spot = FMDrawPropertyMenu.GetGoSpotFromPositionAlignment( textBlockPosition, textBlockAlignment );
            FMDrawIndex.defaultArchetype.alignment = spot;


            FMDrawPropertyMenu.IgnoreEvent = false;
            break;

        case 'TEXTBLOCKALIGNMENT':
            textBlockPosition = $( '#dropdown-propertiesMenu-TEXTPOSITION' ).find( ':selected' ).text();
            textBlockAlignment = $( '#dropdown-propertiesMenu-TEXTBLOCKALIGNMENT' ).find( ':selected' ).text();

            FMDrawPropertyMenu.IgnoreEvent = true;

            if ( FMDrawPropertyMenu.MultiSelectionFlag && textBlockPosition === 'None' )
            {
                textBlockPosition = 'Middle';
            }


            FMDrawPropertyMenu.SetTextBlockLocation( textBlockPosition, textBlockAlignment, canvas );
            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.manualSetTextBlockAlignmentFlag = true;

            var spot2 = FMDrawPropertyMenu.GetGoSpotFromPositionAlignment( textBlockPosition, textBlockAlignment );
            FMDrawIndex.defaultArchetype.alignment = spot2;

            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'TEXTUNDERLINE':
            selectedValue = $( '#dropdown-propertiesMenu-TEXTUNDERLINE' ).find( ':selected' ).text();
            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

            FMDrawPropertyMenu.IgnoreEvent = true;
            var underlinebool;
            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                canvas.startTransaction( 'propertyUpdate' );
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                if ( selectedValue.toUpperCase() === 'TRUE' )
                {
                    dataModel.setDataProperty( activeObject.data, 'isUnderline', true );
                    underlinebool = true;
                }
                else
                {
                    dataModel.setDataProperty( activeObject.data, 'isUnderline', false );
                    underlinebool = false;
                }

                canvas.commitTransaction( 'propertyUpdate' );
            }

            FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'isUnderline', underlinebool);
            FMDrawIndex.defaultArchetype.isUnderline = underlinebool;


            FMDrawPropertyMenu.manualSetTextUnderlineFlag = true;
            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'TEXTSTYLE':
            selectedValue = $( '#dropdown-propertiesMenu-TEXTSTYLE' ).find( ':selected' ).text();
            FMDrawPropertyMenu.IgnoreEvent = true;

            switch ( selectedValue )
            {
                case 'Regular':
                    FMDrawPropertyMenu.FontObject.fontStyle = 'normal';
                    FMDrawPropertyMenu.FontObject.fontWeight = 'normal';
                    break;
                case 'Bold':
                    FMDrawPropertyMenu.FontObject.fontStyle = 'normal';
                    FMDrawPropertyMenu.FontObject.fontWeight = 'bold';
                    break;
                case 'Italic':
                    FMDrawPropertyMenu.FontObject.fontStyle = 'italic';
                    FMDrawPropertyMenu.FontObject.fontWeight = 'normal';
                    break;
                case 'Bold Italic':
                    FMDrawPropertyMenu.FontObject.fontStyle = 'italic';
                    FMDrawPropertyMenu.FontObject.fontWeight = 'bold';
                    break;
            }

            fontStr = FMDrawPropertyMenu.FontObject.fontStyle + ' '
                + FMDrawPropertyMenu.FontObject.fontVariant + ' '
                + FMDrawPropertyMenu.FontObject.fontWeight + ' '
                + FMDrawPropertyMenu.FontObject.fontSize + ' '
                +FMDrawPropertyMenu.FontObject.fontFamily;

            fontStrNoSize = FMDrawPropertyMenu.FontObject.fontStyle + ' '
                +FMDrawPropertyMenu.FontObject.fontVariant + ' '
                +FMDrawPropertyMenu.FontObject.fontWeight + ' '
                +"18px" + ' '
                +FMDrawPropertyMenu.FontObject.fontFamily;

            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
            FMDrawIndex.defaultArchetype.font = fontStr;
	         FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'font', fontStrNoSize);

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                canvas.startTransaction( 'propertyUpdate' );

                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                dataModel.setDataProperty( activeObject.data, 'font', fontStr );
                FMDrawPropertyMenu.UpdateSuperscriptAndSubScriptFont( dataModel, activeObject );

                canvas.commitTransaction( 'propertyUpdate' );
            }

            FMDrawPropertyMenu.manualSetTextStyleFlag = true;
            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'TEXTSIZE':
            selectedValue = $( '#dropdown-propertiesMenu-TEXTSIZE' ).find( ':selected' ).text();
            FMDrawPropertyMenu.IgnoreEvent = true;

            FMDrawPropertyMenu.FontObject.fontSize = selectedValue.replace( 'pt', 'px' );

            fontStr = FMDrawPropertyMenu.FontObject.fontStyle + ' '
                + FMDrawPropertyMenu.FontObject.fontVariant + ' '
                + FMDrawPropertyMenu.FontObject.fontWeight + ' '
                + FMDrawPropertyMenu.FontObject.fontSize + ' '
                + FMDrawPropertyMenu.FontObject.fontFamily;

            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
            FMDrawIndex.defaultArchetype.font = fontStr;

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                canvas.startTransaction( 'propertyUpdate' );
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                dataModel.setDataProperty( activeObject.data, 'font', fontStr );
                FMDrawPropertyMenu.UpdateSuperscriptAndSubScriptFont( dataModel, activeObject );
                canvas.commitTransaction( 'propertyUpdate' );
            }

            FMDrawPropertyMenu.manualSetTextSizeFlag = true;
            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
				FMDrawPropertyMenu.IgnoreEvent = false;
				// if the size changed redraw the quality or status area to calculate the new size. This will also need to be done when weights and measures is added
			 
				 // this is a little screwy. GoJS will not process the change unless the value actually changes. So we will issue the status flag twice to get the
				 // rectangle area calculated. This only happens on a font change in draw.
				 selectedValue = 'false' === $('#dropdown-propertiesMenu-TAGSHOWSTATUS').find(':selected').val() ? false : true;
				 if (selectedValue === true) {
					 selectedValue = false;
				 }
				 else {
					 selectedValue = true;
				 }
				FMDrawPropertyMenu.IgnoreEvent = true;
				canvas.startTransaction('propertyUpdate');
				dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
				dataModel.setDataProperty(FMDrawPropertyMenu.PropertyActiveObject.data, 'TagShowStatus', selectedValue);
				dataModel.setDataProperty(FMDrawPropertyMenu.PropertyActiveObject.data, 'TagStatus', selectedValue === true ? 'FRC' : '');
				canvas.commitTransaction('propertyUpdate');
				FMDrawPropertyMenu.IgnoreEvent = false;
			 
				 // put it back to what it was
				 if (selectedValue === true) {
					 selectedValue = false;
				 }
				 else {
					 selectedValue = true;
				 }
				 FMDrawPropertyMenu.IgnoreEvent = true;
				 canvas.startTransaction('propertyUpdate');
				 dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
				 dataModel.setDataProperty(FMDrawPropertyMenu.PropertyActiveObject.data, 'TagShowStatus', selectedValue);
				 dataModel.setDataProperty(FMDrawPropertyMenu.PropertyActiveObject.data, 'TagStatus', selectedValue === true ? 'FRC' : '');
				 canvas.commitTransaction('propertyUpdate');
				 FMDrawPropertyMenu.IgnoreEvent = false;

            break;
        case 'TEXTFONT':
            selectedValue = $( '#dropdown-propertiesMenu-TEXTFONT' ).find( ':selected' ).text();
            FMDrawPropertyMenu.IgnoreEvent = true;

            FMDrawPropertyMenu.FontObject.fontFamily = selectedValue;

            fontStr = FMDrawPropertyMenu.FontObject.fontStyle + ' '
                + FMDrawPropertyMenu.FontObject.fontVariant + ' '
                + FMDrawPropertyMenu.FontObject.fontWeight + ' '
                + FMDrawPropertyMenu.FontObject.fontSize + ' '
                + FMDrawPropertyMenu.FontObject.fontFamily;

            fontStrNoSize = FMDrawPropertyMenu.FontObject.fontStyle + ' '
                +FMDrawPropertyMenu.FontObject.fontVariant + ' '
                +FMDrawPropertyMenu.FontObject.fontWeight + ' '
                +"18px" + ' '
                +FMDrawPropertyMenu.FontObject.fontFamily;

            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
            FMDrawIndex.defaultArchetype.font = fontStr;
	         FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'font', fontStrNoSize);

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                canvas.startTransaction( 'propertyUpdate' );
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                dataModel.setDataProperty( activeObject.data, 'font', fontStr );
                FMDrawPropertyMenu.UpdateSuperscriptAndSubScriptFont( dataModel, activeObject );
                canvas.commitTransaction( 'propertyUpdate' );
            }

            FMDrawPropertyMenu.manualSetTextFontFlag = true;
            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'TRANSPARENCY':
            // During multi select, if the property value is empty, then return.
            if ( FMDrawPropertyMenu.MultiSelectionFlag && FMDrawPropertyMenu.IsPropertyValueNullOrEmpty( propertyValue ) )
            {
                return;
            }

            newValue = parseInt( propertyValue );
            errFlag = true;

            if ( newValue !== 'NaN' && newValue >= 0 && newValue <= 100 )
            {
                newColor = $( '#manualColor-textbox-propertiesMenu-FILLCOLORSPECTRUM' ).val();

                // Update all the patterns with the new transparency value.
                FMDrawPatternPalette.newTransparencyValue = propertyValue;
                FMDrawPropertyMenu.CreatePatterns();

                transparencyValue = FMDrawPropertyMenu.ConvertTransparencyToFloat( propertyValue );
                rgbObj = FMDrawPropertyMenu.HexToRgb( newColor );
                rgbStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyValue );

                FMDrawPropertyMenu.IgnoreEvent = true;
                dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

                for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                {
                    canvas.startTransaction( 'propertyUpdate' );
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                    dataModel.setDataProperty(activeObject.data, 'transparency', propertyValue);

                    if ( typeof ( activeObject.data.color ) === 'object' || activeObject.category === 'line' )
                    {
                        patternNumber = parseInt( activeObject.data.patternImageName );
                        patternTagId = 'canvasPalatte-propertiesMenu-FILLPATTERNPALETTE-' + activeObject.data.patternImageName;
                        FMDrawPropertyMenu.PatternOnClick( patternTagId, patternNumber, null );
                        FMDrawPropertyMenu.IgnoreEvent = true;
                    }
                    else
                    {
                        dataModel.setDataProperty( activeObject.data, 'color', rgbStr );
                    }

                    $( '#textbox-propertiesMenu-TRANSPARENCY' ).val( newValue );
                }
                FMDrawIndex.defaultArchetype.transparency = propertyValue;
                FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'transparency', propertyValue);
                if (typeof (FMDrawIndex.defaultArchetype.color) === 'object')
                {
                	var hexfill = FMDrawPropertyMenu.Rgb2Hex(FMDrawIndex.defaultArchetype.patternFillColor);
                	var hexstroke = FMDrawPropertyMenu.Rgb2Hex(FMDrawIndex.defaultArchetype.patternStrokeColor);

                	patternNumber = parseInt(FMDrawIndex.defaultArchetype.patternImageName);
                	var dynamicPattern = FMDrawPatternPalette.CreatePatternForOperate(patternNumber, FMDrawPropertyMenu.ConvertToRgbaString(hexfill, transparencyValue), FMDrawPropertyMenu.ConvertToRgbaString(hexstroke,transparencyValue));
                	var brush = new go.Brush(go.Brush.Pattern);
                	brush.pattern = dynamicPattern;
                	FMDrawIndex.defaultArchetype.color = brush.copy();
                	FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'color', brush.copy());
                }
                else
                {
                	//apply transparency to preview color
                	var hex = FMDrawPropertyMenu.Rgb2Hex(FMDrawIndex.defaultArchetype.color);
	                FMDrawIndex.defaultArchetype.color = FMDrawPropertyMenu.ConvertToRgbaString( hex, transparencyValue );
	                FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'color', FMDrawPropertyMenu.ConvertToRgbaString(hex, transparencyValue));
                }

                FMDrawPropertyMenu.manualSetTransparencyFlag = true;
                FMDrawPatternPalette.newTransparencyValue = null;
                canvas.commitTransaction( 'propertyUpdate' );
                errFlag = false;
            }

            FMDrawPropertyMenu.IgnoreEvent = false;

            if ( errFlag )
            {
                errMsg = 'Invalid Transparency.  Must be numeric and between 0 - 100.';
                FMLayout.Alert( errMsg, alertTitle, null );
            }
            break;
        case 'BGTRANSPARENCY':
            newValue = parseInt( propertyValue );
            errFlag = true;
            FMDrawPropertyMenu.IgnoreEvent = true;

            if ( newValue !== 'NaN' && newValue >= 0 && newValue <= 100 )
            {
                newColor = $( '#manualColor-textbox-propertiesMenu-BGFILLCOLORSPECTRUM' ).val();

                transparencyValue = FMDrawPropertyMenu.ConvertTransparencyToFloat( propertyValue );
                rgbObj = FMDrawPropertyMenu.HexToRgb( newColor );
                rgbStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyValue );

                dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

                for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                {
                    canvas.startTransaction( 'propertyUpdate' );
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                    dataModel.setDataProperty( activeObject.data, 'bgtransparency', propertyValue );
                    dataModel.setDataProperty(activeObject.data, 'bgcolor', rgbStr);

                    if ( typeof ( activeObject.data.color ) === 'object' )
                    {
                        patternNumber = parseInt( activeObject.data.patternImageName );
                        patternTagId = 'canvasPalatte-propertiesMenu-FILLPATTERNPALETTE-' + activeObject.data.patternImageName;
                        FMDrawPropertyMenu.PatternOnClick( patternTagId, patternNumber, null );
                        FMDrawPropertyMenu.IgnoreEvent = true;
                    }

                    $( '#textbox-propertiesMenu-BGTRANSPARENCY' ).val( newValue );
                    canvas.commitTransaction( 'propertyUpdate' );
                }

                errFlag = false;
            }

            FMDrawPropertyMenu.IgnoreEvent = false;

            if ( errFlag )
            {
                errMsg = 'Invalid Background Transparency.  Must be numeric and between 0 - 100.';
                FMLayout.Alert( errMsg, alertTitle, null );
            }
            break;

        case 'BARTYPE':
            selectedValue = $( '#dropdown-propertiesMenu-BARTYPE' ).find( ':selected' ).text();
            FMDrawPropertyMenu.IgnoreEvent = true;
            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                if ( activeObject.category === 'bar' )
                {
                    canvas.startTransaction( 'propertyUpdate' );
                    dataModel.setDataProperty( activeObject.data, 'barType', selectedValue );

                    if ( selectedValue === 'Standard' )
                    {
                        dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.Bottom );
                    }

                    if ( selectedValue === 'Deviation' )
                    {
                        dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.Center );
                    }

                    canvas.commitTransaction( 'propertyUpdate' );
                }
            }

            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;

    	case 'USEPRODUCTCOLOR':
    		// During multi select, if the property value is empty, then return.
    		if (FMDrawPropertyMenu.MultiSelectionFlag && FMDrawPropertyMenu.IsPropertyValueNullOrEmpty(propertyValue))
    		{
    			return;
    		}

    		selectedValue = 'false' === $('#dropdown-propertiesMenu-USEPRODUCTCOLOR').find(':selected').val() ? false : true;
    		dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
    		FMDrawPropertyMenu.IgnoreEvent = true;

    		for (nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++)
    		{
    			activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

    			if ( activeObject.category === 'bar' )
			    {
    				canvas.startTransaction('propertyUpdate');
    				activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
    				dataModel.setDataProperty(activeObject.data, 'useProductColor', selectedValue);
    				canvas.commitTransaction('propertyUpdate');
    			}
    		}

    		FMDrawPropertyMenu.IgnoreEvent = false;
    		break;

    	case 'USEALARMLEVEL':
    		// During multi select, if the property value is empty, then return.
    		if (FMDrawPropertyMenu.MultiSelectionFlag && FMDrawPropertyMenu.IsPropertyValueNullOrEmpty(propertyValue))
    		{
    			return;
    		}

    		selectedValue = 'false' === $('#dropdown-propertiesMenu-USEALARMLEVEL').find(':selected').val() ? false : true;
    		dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
    		FMDrawPropertyMenu.IgnoreEvent = true;

    		for (nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++)
    		{
    			activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

    			if (activeObject.category === 'bar')
    			{
    				canvas.startTransaction('propertyUpdate');
    				activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
    				dataModel.setDataProperty(activeObject.data, 'useAlarmLevel', selectedValue);
    				canvas.commitTransaction('propertyUpdate');
    			}
    		}

    		FMDrawPropertyMenu.IgnoreEvent = false;
    		break;

        case 'DEMOVALUEPERCENT':
            selectedValue = $( '#dropdown-propertiesMenu-DEMOVALUEPERCENT' ).find( ':selected' ).text();
            selectedValue = selectedValue.substring( 0, selectedValue.length - 1 ); //remove the % sign
            FMDrawPropertyMenu.IgnoreEvent = true;

            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                if ( activeObject.category === 'bar' )
                {
                    canvas.startTransaction( 'propertyUpdate' );
                    dataModel.setDataProperty( activeObject.data, 'demoPercent', selectedValue );
                    canvas.commitTransaction( 'propertyUpdate' );
                }
            }

            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'USETAGLIMITS':
            FMDrawPropertyMenu.IgnoreEvent = true;
            selectedValue = $( '#dropdown-propertiesMenu-USETAGLIMITS' ).find( ':selected' ).text();
            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                if ( activeObject.category === 'bar' )
                {
                    canvas.startTransaction( 'propertyUpdate' );
                    dataModel.setDataProperty( activeObject.data, 'useTagLimits', selectedValue );

                    if ( selectedValue === 'true' )
                    {
                        $( '#textbox-propertiesMenu-MAXVALUE' ).val( FMDrawPropertyMenu.PropertyActiveObject.data.maxVal );
                        $( '#textbox-propertiesMenu-MINVALUE' ).val( FMDrawPropertyMenu.PropertyActiveObject.data.minVal );
                        $( '#textbox-propertiesMenu-MAXVALUE' ).attr( 'disabled', true );
                        $( '#textbox-propertiesMenu-MINVALUE' ).attr( 'disabled', true );
                    }
                    else
                    {
                        $( '#textbox-propertiesMenu-MAXVALUE' ).attr( 'disabled', false );
                        $( '#textbox-propertiesMenu-MINVALUE' ).attr( 'disabled', false );
                        $( '#textbox-propertiesMenu-MAXVALUE' ).val( FMDrawPropertyMenu.PropertyActiveObject.data.maxUserVal );
                        $( '#textbox-propertiesMenu-MINVALUE' ).val( FMDrawPropertyMenu.PropertyActiveObject.data.minUserVal );
                    }

                    canvas.commitTransaction( 'propertyUpdate' );
                }
            }

            FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'MINVALUE':
            FMDrawPropertyMenu.IgnoreEvent = true;
            errFlag = true;
            newValue = parseInt( propertyValue, 10 );

            if ( newValue !== 'NaN'
                && newValue < FMDrawPropertyMenu.PropertyActiveObject.data.maxUserVal )
            {
                dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

                for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                {
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                    if ( activeObject.category === 'bar' )
                    {
                        canvas.startTransaction( 'propertyUpdate' );
                        dataModel.setDataProperty( activeObject.data, 'minUserVal', newValue );

                        canvas.commitTransaction( 'propertyUpdate' );
                    }
                }

                FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
                errFlag = false;
            }

            FMDrawPropertyMenu.IgnoreEvent = false;

            if ( errFlag )
            {
                errMsg = 'Invalid Minimum value.  Must be less than entered maximum value.';
                FMLayout.Alert( errMsg, alertTitle, null );
                $( '#textbox-propertiesMenu-MINVALUE' ).val( FMDrawPropertyMenu.PropertyActiveObject.data.minUserVal );
            }
            break;
        case 'MAXVALUE':
            FMDrawPropertyMenu.IgnoreEvent = true;
            errFlag = true;
            newValue = parseInt( propertyValue, 10 );

            if ( newValue !== 'NaN'
                && newValue > FMDrawPropertyMenu.PropertyActiveObject.data.minUserVal )
            {
                dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

                for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
                {
                    activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                    if ( activeObject.category === 'bar' )
                    {
                        canvas.startTransaction( 'propertyUpdate' );
                        dataModel.setDataProperty( activeObject.data, 'maxUserVal', newValue );

                        canvas.commitTransaction( 'propertyUpdate' );
                    }
                }

                FMDrawIndex.GetActiveTabGoJSDiagramObject().requestUpdate();
                errFlag = false;
            }

            FMDrawPropertyMenu.IgnoreEvent = false;

            if ( errFlag )
            {
                errMsg = 'Invalid Maximum value.  Must be greater than entered minimum value.';
                FMLayout.Alert( errMsg, alertTitle, null );
                $( '#textbox-propertiesMenu-MAXVALUE' ).val( FMDrawPropertyMenu.PropertyActiveObject.data.maxUserVal );
            }
            break;
        case 'TAGWIDTH':
            canvas.startTransaction( 'propertyUpdate' );
            tagUnits = FMDrawIndex.RetrieveTagUnits( FMDrawPropertyMenu.PropertyActiveObject.data );
            newValue = parseInt( propertyValue, 10 );

            if (newValue !== 'NaN' && newValue >= 0)
            {
	            if ( FMDrawPropertyMenu.PropertyActiveObject.data.TagFieldSelection === FMTAGFIELDSELECTION.VALUE )
	            {
		            //Have to add handling for ft-in-16 and ft-in-8
		            tagPrecision = FMDrawPropertyMenu.PropertyActiveObject.data.TagPrecision;
		            if ( tagUnits === FMENGINEERINGUNIT.FML_FtIn16th || tagUnits === FMENGINEERINGUNIT.FML_FtIn8th )
		            {
			            minNewValueWidth = ( tagUnits === FMENGINEERINGUNIT.FML_FtIn16th ) ? 8 : 7;

			            if ( newValue >= minNewValueWidth )
			            {
				            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
				            dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagFieldWidth', newValue );
				            FMDrawIndex.UpdateTagFormat( dataModel );
				            canvas.commitTransaction( 'propertyUpdate' );
			            }
			            else
			            {
				            canvas.commitTransaction( 'propertyUpdate' );
				            errMsg = 'Value Width too small for values whose units are Ft-In-16 or Ft-In-8.';
				            FMLayout.Alert( errMsg, alertTitle, null );
			            }
		            }
		            else
		            {
			            if ( tagPrecision === 0 || newValue - tagPrecision >= 2 )
			            {
				            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
				            dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagFieldWidth', newValue );
				            FMDrawIndex.UpdateTagFormat( dataModel );
				            canvas.commitTransaction( 'propertyUpdate' );
			            }
			            else
			            {
				            canvas.commitTransaction( 'propertyUpdate' );
				            errMsg = 'Value Width too small to support Value Precision.';
				            FMLayout.Alert( errMsg, alertTitle, null );
			            }
		            }
	            }
	            else
	            {
	            	dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
	            	dataModel.setDataProperty(FMDrawPropertyMenu.PropertyActiveObject.data, 'TagFieldWidth', newValue);
	            	FMDrawIndex.UpdateTagFormat(dataModel);
	            	canvas.commitTransaction('propertyUpdate');
	            }
            }
            else
            {
                canvas.commitTransaction( 'propertyUpdate' );
                errMsg = 'Invalid Positive Integer.';
                FMLayout.Alert( errMsg, alertTitle, null );
            }
            break;
        case 'TAGPRECISION':
            canvas.startTransaction( 'propertyUpdate' );
            tagUnits = FMDrawIndex.RetrieveTagUnits( FMDrawPropertyMenu.PropertyActiveObject.data );
            newValue = parseInt( propertyValue, 10 );
            if ( newValue !== 'NaN' && newValue >= 0 )
            {
                if ( tagUnits !== FMENGINEERINGUNIT.FML_FtIn16th && tagUnits !== FMENGINEERINGUNIT.FML_FtIn8th )
                {
                    tagWidth = FMDrawPropertyMenu.PropertyActiveObject.data.TagFieldWidth;

                    if ( newValue === 0 || tagWidth - newValue >= 2 )
                    {
                        dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
                        dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagPrecision', newValue );
                        FMDrawIndex.UpdateTagFormat( dataModel );
                        canvas.commitTransaction( 'propertyUpdate' );
                    }
                    else
                    {
                        canvas.commitTransaction( 'propertyUpdate' );
                        errMsg = 'Value Width too small to support Value Precision.';
                        FMLayout.Alert( errMsg, alertTitle, null );
                    }
                }
                else
                {
                    dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
                    dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagPrecision', newValue );
                    FMDrawIndex.UpdateTagFormat( dataModel );
                    canvas.commitTransaction( 'propertyUpdate' );
                }
            }
            else
            {
                canvas.commitTransaction( 'propertyUpdate' );
                errMsg = 'Invalid Positive Integer.';
                FMLayout.Alert( errMsg, alertTitle, null );
            }
            break;
        case 'TAGUNITS':
            tagUnitsSelected = $( '#dropdown-propertiesMenu-TAGUNITS' ).find( ':selected' ).val();
            tagUnitsSelectedInt = parseInt( tagUnitsSelected, 10 );
            FMDrawPropertyMenu.IgnoreEvent = true;
            canvas.startTransaction( 'propertyUpdate' );
            tagUnits = FMDrawIndex.RetrieveTagUnits( FMDrawPropertyMenu.PropertyActiveObject.data );
            FMDrawPropertyMenu.EnableDisiablePrecsion( tagUnitsSelectedInt );
            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
            dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagUnits', tagUnitsSelectedInt );

            if ( tagUnitsSelectedInt === 0 )
            {
                tagUnitsSelectedInt = FMDrawPropertyMenu.PropertyActiveObject.data.TagUnitsOriginal;
            }

            if ( ( tagUnitsSelectedInt === FMENGINEERINGUNIT.FML_FtIn16th || tagUnitsSelectedInt === FMENGINEERINGUNIT.FML_FtIn8th )
                && tagUnits !== FMENGINEERINGUNIT.FML_FtIn16th
                && tagUnits !== FMENGINEERINGUNIT.FML_FtIn8th )
            {
                minNewValueWidth = ( tagUnitsSelectedInt === FMENGINEERINGUNIT.FML_FtIn16th ) ? 8 : 7;
                tagPrecision = FMDrawPropertyMenu.PropertyActiveObject.data.TagPrecision;
                tagWidth = FMDrawPropertyMenu.PropertyActiveObject.data.TagFieldWidth;
                wholeNumberWidth = ( tagPrecision > 0 ) ? tagWidth - tagPrecision - 1 : tagWidth;
                tagWidth = ( wholeNumberWidth < 2 ) ? minNewValueWidth : wholeNumberWidth + minNewValueWidth - 2;
                dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagFieldWidth', tagWidth );
            }

            if ( tagUnitsSelectedInt === FMENGINEERINGUNIT.FML_FtIn16th && tagUnits === FMENGINEERINGUNIT.FML_FtIn8th )
            {
                tagWidth = FMDrawPropertyMenu.PropertyActiveObject.data.TagFieldWidth + 1;
                dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagFieldWidth', tagWidth );
            }

            if ( tagUnitsSelectedInt === FMENGINEERINGUNIT.FML_FtIn8th && tagUnits === FMENGINEERINGUNIT.FML_FtIn16th )
            {
                tagWidth = FMDrawPropertyMenu.PropertyActiveObject.data.TagFieldWidth - 1;
                dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagFieldWidth', tagWidth );
            }

            if ( ( tagUnits === FMENGINEERINGUNIT.FML_FtIn16th || tagUnits === FMENGINEERINGUNIT.FML_FtIn8th )
                && tagUnitsSelectedInt !== FMENGINEERINGUNIT.FML_FtIn16th
                && tagUnitsSelectedInt !== FMENGINEERINGUNIT.FML_FtIn8th )
            {
                minNewValueWidth = ( tagUnits === FMENGINEERINGUNIT.FML_FtIn16th ) ? 8 : 7;
                tagPrecision = FMDrawPropertyMenu.PropertyActiveObject.data.TagPrecision;
                tagWidth = FMDrawPropertyMenu.PropertyActiveObject.data.TagFieldWidth;
                wholeNumberWidth = tagWidth - minNewValueWidth + 2;
                tagWidth = ( tagPrecision > 0 ) ? wholeNumberWidth + tagPrecision + 1 : wholeNumberWidth;
                dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagFieldWidth', tagWidth );
            }

            FMDrawIndex.UpdateTagFormat( dataModel );
            canvas.commitTransaction( 'propertyUpdate' );
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'TAGFIELD':
            selectedValue = parseInt( $( '#dropdown-propertiesMenu-TAGFIELD' ).find( ':selected' ).val(), 10 );
            FMDrawPropertyMenu.IgnoreEvent = true;
            canvas.startTransaction( 'propertyUpdate' );

            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
            dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagFieldSelection', selectedValue );
            FMDrawIndex.UpdateTagFormat( dataModel );
            FMDrawPropertyMenu.UpdateSuperscriptAndSubScriptShow();
            dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'ToolTipString', FMDrawPropertyMenu.PropertyActiveObject.data.TagPointIDAndTagID + ' : ' + FMTAGFIELDSELECTION.GetFieldString( selectedValue ) );
            canvas.commitTransaction( 'propertyUpdate' );
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
		 case 'TAGSHOWSTATUS':
    			selectedValue = 'false' === $('#dropdown-propertiesMenu-TAGSHOWSTATUS').find(':selected').val() ? false : true;
    			FMDrawPropertyMenu.IgnoreEvent = true;
    			canvas.startTransaction('propertyUpdate');
    			dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
    			dataModel.setDataProperty(FMDrawPropertyMenu.PropertyActiveObject.data, 'TagShowStatus', selectedValue);
    			dataModel.setDataProperty(FMDrawPropertyMenu.PropertyActiveObject.data, 'TagStatus', selectedValue === true ? 'FRC' : '');
    			canvas.commitTransaction('propertyUpdate');
    			FMDrawPropertyMenu.IgnoreEvent = false;
    			break;
        case 'TAGALARMANNUNCIATION':
	    		selectedValue = 'false' === $('#dropdown-propertiesMenu-TAGALARMANNUNCIATION').find(':selected').val() ? false : true;
            FMDrawPropertyMenu.IgnoreEvent = true;
            canvas.startTransaction( 'propertyUpdate' );
            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
            dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagAlarmAnnunciation', selectedValue );
            canvas.commitTransaction( 'propertyUpdate' );
            FMDrawPropertyMenu.IgnoreEvent = false;
            FMDrawPropertyMenu.SetAnimationID();
            break;
        case 'TAGSHOWWEIGHTSANDMEASURES':
            selectedValue = 'false' === $( '#dropdown-propertiesMenu-TAGSHOWWEIGHTSANDMEASURES' ).find( ':selected' ).val() ? false : true;
            FMDrawPropertyMenu.IgnoreEvent = true;
            canvas.startTransaction( 'propertyUpdate' );
            dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
            dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagShowWeightsAndMeasures', selectedValue );
            dataModel.setDataProperty( FMDrawPropertyMenu.PropertyActiveObject.data, 'TagWeightsAndMeasures', selectedValue === true ? '' : '' );	// bds
            canvas.commitTransaction( 'propertyUpdate' );
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
        case 'BUTTONACTIONTYPE':
            selectedValue = $('#dropdown-propertiesMenu-BUTTONACTIONTYPE').find(':selected').val();
            FMDrawPropertyMenu.currentButtonActionAssociation = selectedValue;
            $( '#textbox-propertiesMenu-BUTTONACTIONTARGET' ).val( '' );
            FMDrawPropertyMenu.IgnoreEvent = true;
            canvas.startTransaction('propertyUpdate');
            FMDrawPropertyMenu.SetButtonActionTypeConfiguration( FMDrawPropertyMenu.PropertyActiveObject, null, null, false, true );
            canvas.commitTransaction('propertyUpdate');
            FMDrawPropertyMenu.SetButtonActionTargetLabel( selectedValue );
            FMDrawPropertyMenu.IgnoreEvent = false;
            break;
    }
};

FMDrawPropertyMenu.setangleBasedOnGeometry = function (activeObject, newValue,useHeight) {
    // need to calculate the new angle
    var coords = FMDrawPropertyMenu.getGeomertyCoords(activeObject.data.geo);
    if (activeObject.data.angle < 0) {
        if (useHeight == true) {
            coords.startY = Math.round(newValue);
        }
        else {
            coords.startX = Math.round(newValue);
        }
    }
    else {
        if (useHeight == true) {
            coords.endY = Math.round(newValue);
        }
        else {
            coords.endX = Math.round(newValue);
        }
    }

    var geoStr = FMDrawPropertyMenu.getGeomertyString(coords);
    activeObject.diagram.model.setDataProperty(activeObject.data, 'geo', geoStr);

    var convertedAngle = activeObject.data.angle * -1;
    return convertedAngle;

}


FMDrawPropertyMenu.conditionAngle = function (angle) {
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

FMDrawPropertyMenu.getGeomertyString = function(coords) {
	var ret = "F M" + Math.round(coords.startX) + " " + Math.round(coords.startY) + " L" + Math.round(coords.endX) + " " + Math.round(coords.endY);
	return ret;
}

FMDrawPropertyMenu.getGeomertyCoords = function(geoString)
{
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

FMDrawPropertyMenu.SetLineAngle = function(angleVal, lineNode )
{
	var angle = angleVal;
	var coords = FMDrawPropertyMenu.getGeomertyCoords(lineNode.data.geo);
	var xLength = coords.endX - coords.startX;
	var yLength = coords.endY - coords.startY;
	var length = Math.sqrt(xLength * xLength + yLength * yLength);
	var midPointX = coords.startX + xLength / 2;
	var midPointY = coords.startY + yLength / 2;
	angle = FMDrawPropertyMenu.conditionAngle(angle);
	var absAngle = Math.abs(angle);
	var rad = Math.PI * absAngle / 180;
	var endY = Math.round(length * Math.sin(rad));
	var endX = Math.round(length * Math.cos(rad));
	var newCoords = {
		startX: 0,
		startY: 0,
		endX: 0,
		endY: 0
	};
	if (angle > 0) {
		newCoords.endY = endY;
	}
	else {
		newCoords.startY = endY;
	}
	newCoords.endX = endX;
	var oldHeigth = Math.abs(coords.endY - coords.startY);
	var oldWidth = Math.abs(coords.endX - coords.startX);
	var height = Math.abs(newCoords.endY - newCoords.startY);
	var width = Math.abs(newCoords.endX - newCoords.startX);
	var yOffset = (oldHeigth - height) / 2;
	var xOffset = (oldWidth - width) / 2;
	var offsetForLine = new go.Point(xOffset, yOffset);
	var geoStr = FMDrawPropertyMenu.getGeomertyString( newCoords );
	lineNode.diagram.model.setDataProperty(lineNode.data, 'geo', geoStr);
	var shape = lineNode.findObject('SHAPE');
	shape.geometryString = geoStr;
	var b = shape.geometry.bounds;
	shape.desiredSize = b.size;

	var part = lineNode.diagram.findPartForData(lineNode.data);
	//part.move(part.position.copy().add(offsetForLine));
	lineNode.move(lineNode.position.copy().add(offsetForLine));
	lineNode.diagram.maybeUpdate();  // force more frequent drawing for smoother looking behavior
	var oldIgnoreEvent = FMDrawPropertyMenu.IgnoreEvent;
	FMDrawPropertyMenu.IgnoreEvent = false;
	FMDrawPropertyMenu.SetHeight(lineNode);
	FMDrawPropertyMenu.SetWidth(lineNode, lineNode.diagram);
	FMDrawPropertyMenu.SetTopCoordinate(lineNode);
	FMDrawPropertyMenu.IgnoreEvent = oldIgnoreEvent;
}
//============================================================================================
// This function will handle the user's selection on the text block location. It will
// position the text block within the object.
//============================================================================================
FMDrawPropertyMenu.SetTextBlockLocation = function( textBlockPosition, textBlockAlignment, canvas )
{
    var dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

    for ( var nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
    {
        canvas.startTransaction( 'propertyUpdate' );
        var activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

        if ( textBlockPosition === 'Middle' && textBlockAlignment === 'Center' )
        {
            dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.Center );
        }

        if ( textBlockPosition === 'Middle' && textBlockAlignment === 'Left' )
        {
            dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.LeftCenter );
        }

        if ( textBlockPosition === 'Middle' && textBlockAlignment === 'Right' )
        {
            dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.RightCenter );
        }

        if ( textBlockPosition === 'Top' && textBlockAlignment === 'Left' )
        {
            dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.TopLeft );
        }

        if ( textBlockPosition === 'Top' && textBlockAlignment === 'Right' )
        {
            dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.TopRight );
        }

        if ( textBlockPosition === 'Top' && textBlockAlignment === 'Center' )
        {
            dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.TopCenter );
        }

        if ( textBlockPosition === 'Bottom' && textBlockAlignment === 'Left' )
        {
            dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.BottomLeft );
        }

        if ( textBlockPosition === 'Bottom' && textBlockAlignment === 'Right' )
        {
            dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.BottomRight );
        }

        if ( textBlockPosition === 'Bottom' && textBlockAlignment === 'Center' )
        {
            dataModel.setDataProperty( activeObject.data, 'alignment', go.Spot.BottomCenter );
        }

        canvas.commitTransaction( 'propertyUpdate' );
    }
};

//============================================================================================
// This function will return the goSpot for a textBlockPosition and Alignment
//============================================================================================
FMDrawPropertyMenu.GetGoSpotFromPositionAlignment = function( textBlockPosition, textBlockAlignment )
{
    if ( textBlockPosition === 'Middle' && textBlockAlignment === 'Center' )
    {
        return go.Spot.Center;
    }

    if ( textBlockPosition === 'Middle' && textBlockAlignment === 'Left' )
    {
        return go.Spot.LeftCenter;
    }

    if ( textBlockPosition === 'Middle' && textBlockAlignment === 'Right' )
    {
        return go.Spot.RightCenter;
    }

    if ( textBlockPosition === 'Top' && textBlockAlignment === 'Left' )
    {
        return go.Spot.TopLeft;
    }

    if ( textBlockPosition === 'Top' && textBlockAlignment === 'Right' )
    {
        return go.Spot.TopRight;
    }

    if ( textBlockPosition === 'Top' && textBlockAlignment === 'Center' )
    {
        return go.Spot.TopCenter;
    }

    if ( textBlockPosition === 'Bottom' && textBlockAlignment === 'Left' )
    {
        return go.Spot.BottomLeft;
    }

    if ( textBlockPosition === 'Bottom' && textBlockAlignment === 'Right' )
    {
        return go.Spot.BottomRight;
    }

    if ( textBlockPosition === 'Bottom' && textBlockAlignment === 'Center' )
    {
        return go.Spot.BottomCenter;
    }

    return go.Spot.Center; //Since function always returns a value, we need a default value to return if all if statement are false.  Resharper caught this.
};


//===================================================================================
// This function will return true if the property value is null or empty.
//===================================================================================
FMDrawPropertyMenu.IsPropertyValueNullOrEmpty = function( propertyValue )
{
    if ( propertyValue == null || propertyValue === 'undefined' || propertyValue === '' || propertyValue === ' ' )
    {
        return true;
    }

    return false;
};

//===================================================================================
// This function will calculate the top position for a line. Since a line has two
// sets of points, need to determine which point is the highest and increase or
// decrease both sets of points proportionally based on the new value.
//===================================================================================
FMDrawPropertyMenu.CalculateTopPositionForLine = function( activeObject, newValue )
{
    var currentPoints = activeObject.data.points.toArray();
    var pointX1 = currentPoints[0].x;
    var pointY1 = currentPoints[0].y;
    var pointX2 = currentPoints[1].x;
    var pointY2 = currentPoints[1].y;

    var isY1Higher = pointY1 < pointY2 ? true : false;
    var yDelta;

    // No sure why the point value is always 3 units off.
    var newValueRounded = newValue - 3;

    if ( isY1Higher )
    {
        // Return null if there was no change in the Top position.
        // No sure why the point value is always 3 units off.
        if ( Math.round( newValue ) === Math.round( pointY1 + 3 ) )
        {
            return null;
        }

        yDelta = Math.round( pointY1 ) - newValueRounded;
        pointY1 = newValueRounded;
        pointY2 = Math.round( pointY2 ) - yDelta;
    }
    else
    {
        // Return null if there was no change in the Top position.
        if ( Math.round( newValue ) === Math.round( pointY2 ) )
        {
            return null;
        }

        yDelta = Math.round( pointY2 ) - newValueRounded;
        pointY2 = newValueRounded;
        pointY1 = Math.round( pointY1 ) - yDelta;
    }

    var newPointList = new go.List( go.Point );
    newPointList.add( new go.Point( pointX1, pointY1 ) );
    newPointList.add( new go.Point( pointX2, pointY2 ) );

    return newPointList;
};

//===================================================================================
// This function will calculate the left position for a line. Since a line has two
// sets of points, need to determine which point is the highest and increase or
// decrease both sets of points proportionally based on the new value.
//===================================================================================
FMDrawPropertyMenu.CalculateLeftPositionForLine = function( activeObject, newValue )
{
    var currentPoints = activeObject.data.points.toArray();
    var pointX1 = currentPoints[0].x;
    var pointY1 = currentPoints[0].y;
    var pointX2 = currentPoints[1].x;
    var pointY2 = currentPoints[1].y;

    var isX1MoreLeft = pointX1 < pointX2 ? true : false;
    var xDelta;

    // No sure why the point value is always 3 units off.
    var newValueRounded = newValue - 3;

    if ( isX1MoreLeft )
    {
        // Return null if there was no change in the Left position.
        // No sure why the point value is always 3 units off.
        if ( Math.round( newValue ) === Math.round( pointX1 + 3 ) )
        {
            return null;
        }

        xDelta = Math.round( pointX1 ) - newValueRounded;
        pointX1 = newValueRounded;
        pointX2 = Math.round( pointX2 ) - xDelta;
    }
    else
    {
        // Return null if there was no change in the Left position.
        if ( Math.round( newValue ) === Math.round( pointX2 ) )
        {
            return null;
        }

        xDelta = Math.round( pointX2 ) - newValueRounded;
        pointX2 = newValueRounded;
        pointX1 = Math.round( pointX1 ) - xDelta;
    }

    var newPointList = new go.List( go.Point );
    newPointList.add( new go.Point( pointX1, pointY1 ) );
    newPointList.add( new go.Point( pointX2, pointY2 ) );

    return newPointList;
};

//===================================================================================================
// This function creates the recent color object that contains the values of the most recent
// text color and fill color to be save.  It sets the default to be white.
//===================================================================================================
FMDrawPropertyMenu.CreateModelRecentColorObject = function()
{
    if ( FMDrawPropertyMenu.PropertyActiveObject.diagram == null )
    {
        return;
    }

    var model = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
    if ( model.modelData.PropertyMenuRecentColors == null )
    {
        var recentColorObject = new Object();
        recentColorObject.TextColor1 = '#ffffff';
        recentColorObject.TextColor2 = '#ffffff';
        recentColorObject.TextColor3 = '#ffffff';
        recentColorObject.TextColor4 = '#ffffff';
        recentColorObject.TextColor5 = '#ffffff';
        recentColorObject.TextColor6 = '#ffffff';
        recentColorObject.TextColor7 = '#ffffff';
        recentColorObject.TextColor8 = '#ffffff';
        recentColorObject.FillColor1 = '#ffffff';
        recentColorObject.FillColor2 = '#ffffff';
        recentColorObject.FillColor3 = '#ffffff';
        recentColorObject.FillColor4 = '#ffffff';
        recentColorObject.FillColor5 = '#ffffff';
        recentColorObject.FillColor6 = '#ffffff';
        recentColorObject.FillColor7 = '#ffffff';
        recentColorObject.FillColor8 = '#ffffff';
        recentColorObject.PatternColor1 = '#ffffff';
        recentColorObject.PatternColor2 = '#ffffff';
        recentColorObject.PatternColor3 = '#ffffff';
        recentColorObject.PatternColor4 = '#ffffff';
        recentColorObject.PatternColor5 = '#ffffff';
        recentColorObject.PatternColor6 = '#ffffff';
        recentColorObject.PatternColor7 = '#ffffff';
        recentColorObject.PatternColor8 = '#ffffff';
        recentColorObject.LineColor1 = '#ffffff';
        recentColorObject.LineColor2 = '#ffffff';
        recentColorObject.LineColor3 = '#ffffff';
        recentColorObject.LineColor4 = '#ffffff';
        recentColorObject.LineColor5 = '#ffffff';
        recentColorObject.LineColor6 = '#ffffff';
        recentColorObject.LineColor7 = '#ffffff';
        recentColorObject.LineColor8 = '#ffffff';

        model.modelData.PropertyMenuRecentColors = recentColorObject;
    }
};

//=============================================================================
// This function will set the Top coordinate value on the
// text box in the property menu with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetTopCoordinateForce = function (y)
{

	$('#textbox-propertiesMenu-TOP').val(' ');

	if (FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetTopFlag)
	{
		$('#textbox-propertiesMenu-TOP').val(Math.round(y));
	}
};

//=============================================================================
// This function will set the Top coordinate value on the
// text box in the property menu with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetTopCoordinate = function( obj )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    var transFormedPosition = obj.position;
	FMDrawPropertyMenu.SetTopCoordinateForce( transFormedPosition.y );
};

//=============================================================================
// This function will set the Left coordinate value on the
// text box in the property menu with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetLeftCoordinateForce = function (x)
{

	$('#textbox-propertiesMenu-LEFT').val(' ');

	if (FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetLeftFlag)
	{
		$('#textbox-propertiesMenu-LEFT').val(Math.round(x));
	}
};

//=============================================================================
// This function will set the Left coordinate value on the
// text box in the property menu with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetLeftCoordinate = function( obj )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    var transFormedPosition = obj.position;
	FMDrawPropertyMenu.SetLeftCoordinateForce( transFormedPosition.x );
};

//=============================================================================
// This function will set the angle value on the
// text box in the property menu with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetAngle = function( obj )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#textbox-propertiesMenu-ANGLE' ).val( ' ' );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetAngleFlag )
    {
        var currentAngle = obj.data.angle;
        var convertedAngle = FMDrawPropertyMenu.ConvertAngleToVisioStyle( currentAngle );
        $( '#textbox-propertiesMenu-ANGLE' ).val( Math.round( convertedAngle ) );
    }
};

//=============================================================================
// This function will set the width value on the
// text box in the property menu with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetWidthForce = function (width)
{

	$('#textbox-propertiesMenu-WIDTH').val(' ');

	if (FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetWidthFlag)
	{
		//If Panel is bound to a resizing object then use that object's width
		$('#textbox-propertiesMenu-WIDTH').val(Math.round(width));
	}
};

//=============================================================================
// This function will set the width value on the
// text box in the property menu with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetWidth = function( obj )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    var width = (obj.resizeObject) ? obj.resizeObject.width : obj.width;

	FMDrawPropertyMenu.SetWidthForce( width );
};


//=============================================================================
// This function will set the height value on the
// text box in the property menu with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetHeightForce = function (height)
{

	$('#textbox-propertiesMenu-HEIGHT').val(' ');

	if (FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetHeightFlag)
	{
		//If Panel is bound to a resizing object then use that object's height
		$('#textbox-propertiesMenu-HEIGHT').val(Math.round(height));
	}
};


//=============================================================================
// This function will set the height value on the
// text box in the property menu with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetHeight = function( obj )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    var height = (obj.resizeObject) ? obj.resizeObject.height : obj.height;

	FMDrawPropertyMenu.SetHeightForce( height );
};

//=============================================================================
// This function will set the text position value on the
// text box in the property menu with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetTextPosition = function( obj )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#textbox-propertiesMenu-TEXTPOSITION' ).val( obj.textPosition );
};

//=========================================================
// This function will set the layer dropdown based on the object setting.
//=========================================================
FMDrawPropertyMenu.SetLayerDropdown = function () {
    // When true, ignore this event since it is coming from the canvas.startTransaction and not from the user.
    if (FMDrawPropertyMenu.IgnoreEvent) {
        return;
    }

    $("#dropdown-propertiesMenu-LAYER").empty();
    var layerManager = new FMDrawIndex._LayerManager();
    var layers = layerManager.GetLayers();
    layers.forEach(function (layer) {
        if (layer.allowSelect)
            $("#dropdown-propertiesMenu-LAYER").append($("<option></option>").val(layer.name).html(layer.displayName));
    });

    $('#dropdown-propertiesMenu-LAYER')[0].selectedIndex = -1;
    var selectedLayerName = null;
    var isMultipleLayerSel = false;
    var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
    var selectedParts = diagram.selection;
    if (!selectedParts)
        return;
    selectedParts.each(function (p) {
        if (!selectedLayerName)
            selectedLayerName = p.layer.name;
        else
            isMultipleLayerSel = true;
    });
    if (!selectedLayerName || isMultipleLayerSel)
        return;
    $('#dropdown-propertiesMenu-LAYER > option').each(function () {
        var optionValue = $(this).val();
        if (optionValue === selectedLayerName) {
            $('#dropdown-propertiesMenu-LAYER').val(selectedLayerName);
        }
    });
};

//=============================================================================
// This function will set the Z-Order text box in the property menu
// with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetZorder = function( activeObj )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#textbox-propertiesMenu-ZORDER' ).val( ' ' );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetZOrderFlag )
    {
        $( '#textbox-propertiesMenu-ZORDER' ).val( '' );

        if ( activeObj == null || activeObj.data == null || activeObj.data.zOrder == null )
        {
            return;
        }

        $( '#textbox-propertiesMenu-ZORDER' ).val( activeObj.data.zOrder );
        $( '#textbox-propertiesMenu-ZORDER' ).attr( 'disabled', 'disabled' );
    }
};

//=============================================================================
// This function will set the transparency text box in the property menu
// with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetTransparency = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#textbox-propertiesMenu-TRANSPARENCY' ).val( ' ' );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetTransparencyFlag )
    {
        if ( $( '#textbox-propertiesMenu-TRANSPARENCY' ).val() == null || $( '#textbox-propertiesMenu-TRANSPARENCY' ).val() === '' )
        {
            $( '#textbox-propertiesMenu-TRANSPARENCY' ).val( '0' );
        }

        if ( FMDrawPropertyMenu.PropertyActiveObject.data.transparency == null || FMDrawPropertyMenu.PropertyActiveObject.data.transparency === '' )
        {
            $( '#textbox-propertiesMenu-TRANSPARENCY' ).val( '0' );
        }
        else
        {
            $( '#textbox-propertiesMenu-TRANSPARENCY' ).val( FMDrawPropertyMenu.PropertyActiveObject.data.transparency );
        }
    }
};

//=============================================================================
// This function will set the bckground transparency text box in the property menu
// with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetBackgroundTransparency = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    if ( $( '#textbox-propertiesMenu-BGTRANSPARENCY' ).val() == null || $( '#textbox-propertiesMenu-BGTRANSPARENCY' ).val() === '' )
    {
        $( '#textbox-propertiesMenu-BGTRANSPARENCY' ).val( '0' );
    }

    if ( FMDrawPropertyMenu.PropertyActiveObject.data.bgtransparency == null || FMDrawPropertyMenu.PropertyActiveObject.data.bgtransparency === '' )
    {
        $( '#textbox-propertiesMenu-BGTRANSPARENCY' ).val( '0' );
    }
    else
    {
        $( '#textbox-propertiesMenu-BGTRANSPARENCY' ).val( FMDrawPropertyMenu.PropertyActiveObject.data.bgtransparency );
    }
};


//=============================================================================
// This function will set the transparency text box in the property menu
// with the object's value if it exists.
//=============================================================================
FMDrawPropertyMenu.SetLineStyleTransparency = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#textbox-propertiesMenu-LINESTYLETRANSPARENCY' ).val( ' ' );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetLineTransparencyFlag )
    {
        if ( $( '#textbox-propertiesMenu-LINESTYLETRANSPARENCY' ).val() == null || $( '#textbox-propertiesMenu-LINESTYLETRANSPARENCY' ).val() === '' )
        {
            $( '#textbox-propertiesMenu-LINESTYLETRANSPARENCY' ).val( '0' );
        }

        if ( FMDrawPropertyMenu.PropertyActiveObject.data.lineStyleTransparency == null || FMDrawPropertyMenu.PropertyActiveObject.data.lineStyleTransparency === '' )
        {
            $( '#textbox-propertiesMenu-LINESTYLETRANSPARENCY' ).val( '0' );
        }
        else
        {
            $( '#textbox-propertiesMenu-LINESTYLETRANSPARENCY' ).val( FMDrawPropertyMenu.PropertyActiveObject.data.lineStyleTransparency );
        }
    }
};

//=========================================================
// This function will set the text align dropdown based on
// the object setting.
//=========================================================
FMDrawPropertyMenu.SetTextAlignmentDropdown = function( textAlignment )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#dropdown-propertiesMenu-TEXTALIGNMENT option[data-value=\'NONE\']' ).prop( 'selected', true );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetTextJustificatiohFlag )
    {
        if ( textAlignment === 'start' || textAlignment === 'left' )
        {
            $( '#dropdown-propertiesMenu-TEXTALIGNMENT option[value=\'1\']' ).prop( 'selected', true );
        }

        if ( textAlignment === 'end' || textAlignment === 'right' )
        {
            $( '#dropdown-propertiesMenu-TEXTALIGNMENT option[value=\'3\']' ).prop( 'selected', true );
        }

        if ( textAlignment === 'center' )
        {
            $( '#dropdown-propertiesMenu-TEXTALIGNMENT option[value=\'2\']' ).prop( 'selected', true );
        }
    }
};

//=========================================================
// This function will set the text underline dropdown based 
// on the object setting.
//=========================================================
FMDrawPropertyMenu.SetTextUnderlineDropdown = function( textUnderline )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#dropdown-propertiesMenu-TEXTUNDERLINE option[data-value=\'NONE\']' ).prop( 'selected', true );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetTextUnderlineFlag )
    {
        if ( textUnderline === true )
        {
            $( '#dropdown-propertiesMenu-TEXTUNDERLINE option[value=\'1\']' ).prop( 'selected', true );
        }
        else
        {
            $( '#dropdown-propertiesMenu-TEXTUNDERLINE option[value=\'2\']' ).prop( 'selected', true );
        }
    }
};

//=========================================================
// This function will set the text style dropdown based 
// on the object setting.
//=========================================================
FMDrawPropertyMenu.SetTextStyleDropdown = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#dropdown-propertiesMenu-TEXTSTYLE option[data-value=\'NONE\']' ).prop( 'selected', true );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetTextStyleFlag )
    {
        // Style is: Italic, Normal, or Oblique
        var textStyle = FMDrawPropertyMenu.FontObject.fontStyle;

        //Weight is: Normal or Bold
        var textWeight = FMDrawPropertyMenu.FontObject.fontWeight;

        // Dropdown list values: Regular 1, bold 2, italic 3, bold italic 4
        if ( textStyle === 'italic' && textWeight === 'bold' )
        {
            $( '#dropdown-propertiesMenu-TEXTSTYLE option[value=\'4\']' ).prop( 'selected', true );
        }
        else if ( textStyle === 'italic' && textWeight === 'normal' )
        {
            $( '#dropdown-propertiesMenu-TEXTSTYLE option[value=\'3\']' ).prop( 'selected', true );
        }
        else if ( textStyle === 'normal' && textWeight === 'bold' )
        {
            $( '#dropdown-propertiesMenu-TEXTSTYLE option[value=\'2\']' ).prop( 'selected', true );
        }
        else
        {
            $( '#dropdown-propertiesMenu-TEXTSTYLE option[value=\'1\']' ).prop( 'selected', true );
        }
    }
};

//=========================================================
// This function will set the text size dropdown based 
// on the object setting.
//=========================================================
FMDrawPropertyMenu.SetTextSizeDropdown = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#dropdown-propertiesMenu-TEXTSIZE option[data-value=\'NONE\']' ).prop( 'selected', true );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetTextSizeFlag )
    {
        var found = false;

        $( '#dropdown-propertiesMenu-TEXTSIZE > option' ).each( function()
        {
            var optionText = $( this ).text();
            var fontSize = FMDrawPropertyMenu.FontObject.fontSize.replace( 'px', 'pt' );

            if ( optionText === fontSize )
            {
                var optionId = '#dropdown-propertiesMenu-TEXTSIZE option[data-value=\'' + FMDrawPropertyMenu.FontObject.fontSize.replace( 'px', '' ) + '\']';
                $( optionId ).prop( 'selected', true );
                found = true;
            }
        } );

        if ( found === false )
        {
            $( '#dropdown-propertiesMenu-TEXTSIZE option[data-value=\'13\']' ).prop( 'selected', true );
        }
    }
};

//=========================================================
// This function will set the text font dropdown based 
// on the object setting.
//=========================================================
FMDrawPropertyMenu.SetTextFontDropdown = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#dropdown-propertiesMenu-TEXTFONT option[data-value=\'NONE\']' ).prop( 'selected', true );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetTextFontFlag )
    {
        var found = false;

        $( '#dropdown-propertiesMenu-TEXTFONT > option' ).each( function()
        {
            var optionText = $( this ).text();

            if ( optionText === FMDrawPropertyMenu.FontObject.fontFamily )
            {
                var optionId = '#dropdown-propertiesMenu-TEXTFONT option[data-value=\'' + FMDrawPropertyMenu.FontObject.fontFamily + '\']';
                $( optionId ).prop( 'selected', true );
                found = true;
            }
        } );

        if ( found === false )
        {
            $( '#dropdown-propertiesMenu-TEXTFONT option[data-value=\'Arial\']' ).prop( 'selected', true );
        }
    }
};

//=========================================================
// This function will set the text color based on
// the object setting.
//=========================================================
FMDrawPropertyMenu.SetTextColorDropdown = function( color )
{
    var rgbObj = FMDrawPropertyMenu.HexToRgb( '#ffffff' );
    var rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, 1 );
    $( '#textbox-propertiesMenu-TEXTCOLOR' ).css( 'background-color', rgbaStr );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetTextColorFlag )
    {
        $( '#textbox-propertiesMenu-TEXTCOLOR' ).css( 'background-color', color );

        var currentColor = $( '#textbox-propertiesMenu-TEXTCOLOR' ).css( 'background-color' );
        $( '#manualColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM' ).val( FMDrawPropertyMenu.Rgb2Hex( currentColor ) );
        $( '#manualColorSampler-textbox-propertiesMenu-TEXTCOLORSPECTRUM' ).css( 'background-color', FMDrawPropertyMenu.Rgb2Hex( currentColor ) );
        $( '#textbox-fillColorSpectrum-propertiesMenu-TEXTCOLORSPECTRUM' ).spectrum( 'set', FMDrawPropertyMenu.Rgb2Hex( currentColor ) );

        // When true, ignore this event since it is coming from the model change
        // and not from the user.
        if ( FMDrawPropertyMenu.IgnoreEvent == null || FMDrawPropertyMenu.IgnoreEvent === false )
        {
            var recentColors = FMDrawPropertyMenu.PropertyActiveObject.diagram.model.modelData.PropertyMenuRecentColors;
            $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-1' ).css( 'background-color', recentColors.TextColor1 );
            $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-2' ).css( 'background-color', recentColors.TextColor2 );
            $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-3' ).css( 'background-color', recentColors.TextColor3 );
            $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-4' ).css( 'background-color', recentColors.TextColor4 );
            $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-5' ).css( 'background-color', recentColors.TextColor5 );
            $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-6' ).css( 'background-color', recentColors.TextColor6 );
            $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-7' ).css( 'background-color', recentColors.TextColor7 );
            $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-8' ).css( 'background-color', recentColors.TextColor8 );
        }
    }
};

//=========================================================
// This function will set the fill color based on
// the object setting.
//=========================================================
FMDrawPropertyMenu.SetFillColorDropdown = function()
{
    // Clear the Color and Pattern dropdowns
    var rgbObj = FMDrawPropertyMenu.HexToRgb( '#ffffff' );
    var rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, 1 );
    $( '#textbox-propertiesMenu-FILLCOLOR' ).css( 'background-color', rgbaStr );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetFillColorFlag )
    {
        var data = FMDrawPropertyMenu.PropertyActiveObject.data;
        var color = data.color;

        if ( data.color == null || typeof ( data.color ) === 'undefined' )
        {
            color = data.fill;

            if ( color == null || typeof ( color ) === 'undefined' )
            {
                color = '#99ccff';
            }
        }

        if ( typeof ( color ) === 'object' || ( color.indexOf( '#' ) === -1 && color.indexOf( 'rgb' ) === -1 ) )
        {
            color = '#ffffff';
        }

        // Initialize the pattern fill color property only if the color property is a
        // hex string and the pattern fill color has not be defined.
        if ( color != null && data.patternFillColor == null )
        {
            var dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
            var diagram = FMDrawPropertyMenu.PropertyActiveObject.diagram;
            rgbObj = FMDrawPropertyMenu.HexToRgb( color );

            if ( rgbObj == null )
            {
                rgbObj = FMDrawPropertyMenu.Rgb2Hex( color );
            }

            rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, 1 );

            diagram.startTransaction('patternFillColor_propertyUpdate');
            dataModel.setDataProperty(data, 'patternFillColor', rgbaStr);
            diagram.commitTransaction('patternFillColor_propertyUpdate');
        }

        var previousColor = data.patternFillColor;

        // Remove the transparency value.
        if ( data.patternFillColor )
        {
            if ( data.patternFillColor.indexOf( 'rgba' ) !== -1 )
            {
                var parts = data.patternFillColor.split( ',' );
                previousColor = parts[0] + ', ' + parts[1] + ', ' + parts[2] + ')';
            }
        }

        $( '#textbox-propertiesMenu-FILLCOLOR' ).css( 'background-color', previousColor );

        $( '#manualColor-textbox-propertiesMenu-FILLCOLORSPECTRUM' ).val( FMDrawPropertyMenu.Rgb2Hex( previousColor ) );
        $( '#manualColorSampler-textbox-propertiesMenu-FILLCOLORSPECTRUM' ).css( 'background-color', FMDrawPropertyMenu.Rgb2Hex( previousColor ) );
        $( '#textbox-fillColorSpectrum-propertiesMenu-FILLCOLORSPECTRUM' ).spectrum( 'set', FMDrawPropertyMenu.Rgb2Hex( previousColor ) );

        // When true, ignore this event since it is coming from the model change
        // and not from the user.
        if ( FMDrawPropertyMenu.IgnoreEvent == null || FMDrawPropertyMenu.IgnoreEvent === false )
        {
            var recentColors = FMDrawPropertyMenu.PropertyActiveObject.diagram.model.modelData.PropertyMenuRecentColors;
            $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-1' ).css( 'background-color', recentColors.FillColor1 );
            $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-2' ).css( 'background-color', recentColors.FillColor2 );
            $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-3' ).css( 'background-color', recentColors.FillColor3 );
            $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-4' ).css( 'background-color', recentColors.FillColor4 );
            $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-5' ).css( 'background-color', recentColors.FillColor5 );
            $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-6' ).css( 'background-color', recentColors.FillColor6 );
            $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-7' ).css( 'background-color', recentColors.FillColor7 );
            $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-8' ).css( 'background-color', recentColors.FillColor8 );
        }
    }
};

//=========================================================
// This function will set the background bfill color based on
// the object setting.
//=========================================================
FMDrawPropertyMenu.SetBgFillColorDropdown = function()
{
    var data = FMDrawPropertyMenu.PropertyActiveObject.data;

    // Initialize the pattern fill color property only if the color property is a
    // hex string and the pattern fill color has not be defined.
    if ( typeof ( data.bgcolor ) !== 'undefined' && data.bgpatternFillColor == null )
    {
        var dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
        var diagram = FMDrawPropertyMenu.PropertyActiveObject.diagram;
        var rgbObj = FMDrawPropertyMenu.HexToRgb( data.bgcolor );

        if ( rgbObj == null )
        {
            rgbObj = FMDrawPropertyMenu.Rgb2Hex( data.bgcolor );
        }

        var rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, 1 );

        diagram.startTransaction( 'patternFillColor_propertyUpdate' );
        dataModel.setDataProperty( data, 'bgpatternFillColor', rgbaStr );
        diagram.commitTransaction( 'patternFillColor_propertyUpdate' );
    }

    var previousColor = data.bgpatternFillColor;

    if ( previousColor == null )
    {
        previousColor = data.color;

        if ( data.color == null || typeof ( data.color ) === 'undefined' || typeof ( data.color ) === 'object' )
        {
            previousColor = '#99ccff';
        }
    }

    // Remove the transparency value.
    if ( previousColor != null && previousColor.indexOf( 'rgba' ) !== -1 )
    {
        var parts = previousColor.split( ',' );
        previousColor = parts[0] + ', ' + parts[1] + ', ' + parts[2] + ')';
    }

    $( '#textbox-propertiesMenu-BGFILLCOLOR' ).css( 'background-color', previousColor );

    $( '#manualColor-textbox-propertiesMenu-BGFILLCOLORSPECTRUM' ).val( FMDrawPropertyMenu.Rgb2Hex( previousColor ) );
    $( '#manualColorSampler-textbox-propertiesMenu-BGFILLCOLORSPECTRUM' ).css( 'background-color', FMDrawPropertyMenu.Rgb2Hex( previousColor ) );
    $( '#textbox-fillColorSpectrum-propertiesMenu-BGFILLCOLORSPECTRUM' ).spectrum( 'set', FMDrawPropertyMenu.Rgb2Hex( previousColor ) );

    // When true, ignore this event since it is coming from the model change
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent == null || FMDrawPropertyMenu.IgnoreEvent === false )
    {
        var recentColors = FMDrawPropertyMenu.PropertyActiveObject.diagram.model.modelData.PropertyMenuRecentColors;
        $( '#recentColor-textbox-propertiesMenu-BGFILLCOLORSPECTRUM-1' ).css( 'background-color', recentColors.FillColor1 );
        $( '#recentColor-textbox-propertiesMenu-BGFILLCOLORSPECTRUM-2' ).css( 'background-color', recentColors.FillColor2 );
        $( '#recentColor-textbox-propertiesMenu-BGFILLCOLORSPECTRUM-3' ).css( 'background-color', recentColors.FillColor3 );
        $( '#recentColor-textbox-propertiesMenu-BGFILLCOLORSPECTRUM-4' ).css( 'background-color', recentColors.FillColor4 );
        $( '#recentColor-textbox-propertiesMenu-BGFILLCOLORSPECTRUM-5' ).css( 'background-color', recentColors.FillColor5 );
        $( '#recentColor-textbox-propertiesMenu-BGFILLCOLORSPECTRUM-6' ).css( 'background-color', recentColors.FillColor6 );
        $( '#recentColor-textbox-propertiesMenu-BGFILLCOLORSPECTRUM-7' ).css( 'background-color', recentColors.FillColor7 );
        $( '#recentColor-textbox-propertiesMenu-bgFILLCOLORSPECTRUM-8' ).css( 'background-color', recentColors.FillColor8 );
    }
};

//=========================================================
// This function will set the pattern color based on
// the object setting.
//=========================================================
FMDrawPropertyMenu.SetPatternColorDropdown = function()
{
    var rgbObj = FMDrawPropertyMenu.HexToRgb( '#ffffff' );
    var rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, 1 );
    $( '#textbox-propertiesMenu-PATTERNCOLOR' ).css( 'background-color', rgbaStr );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetPatternColorFlag )
    {
        var previousColor = FMDrawPropertyMenu.PropertyActiveObject.data.patternStrokeColor;

        // Set the pattern stroke to white if it is undefined.
        if ( previousColor == null || previousColor === 'undefined' )
        {
            previousColor = '#ffffff';
        }

        $( '#textbox-propertiesMenu-PATTERNCOLOR' ).css( 'background-color', previousColor );

        var currentColor = $( '#textbox-propertiesMenu-PATTERNCOLOR' ).css( 'background-color' );
        $( '#manualColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM' ).val( FMDrawPropertyMenu.Rgb2Hex( currentColor ) );
        $( '#manualColorSampler-textbox-propertiesMenu-PATTERNCOLORSPECTRUM' ).css( 'background-color', FMDrawPropertyMenu.Rgb2Hex( currentColor ) );
        $( '#textbox-fillColorSpectrum-propertiesMenu-PATTERNCOLORSPECTRUM' ).spectrum( 'set', FMDrawPropertyMenu.Rgb2Hex( currentColor ) );

        // When true, ignore this event since it is coming from the model change
        // and not from the user.
        if ( FMDrawPropertyMenu.IgnoreEvent == null || FMDrawPropertyMenu.IgnoreEvent === false )
        {
            var recentColors = FMDrawPropertyMenu.PropertyActiveObject.diagram.model.modelData.PropertyMenuRecentColors;
            $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-1' ).css( 'background-color', recentColors.FillColor1 );
            $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-2' ).css( 'background-color', recentColors.FillColor2 );
            $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-3' ).css( 'background-color', recentColors.FillColor3 );
            $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-4' ).css( 'background-color', recentColors.FillColor4 );
            $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-5' ).css( 'background-color', recentColors.FillColor5 );
            $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-6' ).css( 'background-color', recentColors.FillColor6 );
            $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-7' ).css( 'background-color', recentColors.FillColor7 );
            $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-8' ).css( 'background-color', recentColors.FillColor8 );
        }
    }
};

//=========================================================
// This function will set the text block position dropdown 
// based on the object setting.
//=========================================================
FMDrawPropertyMenu.SetTextBlockPositionDropdown = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#dropdown-propertiesMenu-TEXTPOSITION option[data-value=\'NONE\']' ).prop( 'selected', true );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetTextBlockPositionFlag )
    {
        $( '#dropdown-propertiesMenu-TEXTPOSITION option[data-value=\'CENTER\']' ).prop( 'selected', true );
        var blockLocationStr = FMDrawPropertyMenu.GetTextBlockSetting( FMDrawPropertyMenu.PropertyActiveObject.data.alignment );

        if ( blockLocationStr === 'topCenter'
            || blockLocationStr === 'topLeft'
            || blockLocationStr === 'topRight' )
        {
            $( '#dropdown-propertiesMenu-TEXTPOSITION option[data-value=\'TOP\']' ).prop( 'selected', true );
        }

        if ( blockLocationStr === 'center' )
        {
            $( '#dropdown-propertiesMenu-TEXTPOSITION option[data-value=\'CENTER\']' ).prop( 'selected', true );
        }

        if ( blockLocationStr === 'bottomCenter'
            || blockLocationStr === 'bottomLeft'
            || blockLocationStr === 'bottomRight' )
        {
            $( '#dropdown-propertiesMenu-TEXTPOSITION option[data-value=\'BOTTOM\']' ).prop( 'selected', true );
        }
    }
};

//=========================================================
// This function will set the text block position dropdown 
// based on the object setting.
//=========================================================
FMDrawPropertyMenu.SetTagUnitsDropdown = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    //$( '#tr-propertiesMenu-TAGUNITS' ).show();
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }
    FMDrawIndex.UpdateUnitsDropDown( 'dropdown-propertiesMenu-TAGUNITS', FMDrawPropertyMenu.PropertyActiveObject.data.TagUnitType );
    var tagUnits = FMDrawPropertyMenu.PropertyActiveObject.data.TagUnits;
    var selectString = '#dropdown-propertiesMenu-TAGUNITS option[value=\'' + tagUnits + '\']';
    var propMenuItem = $( selectString );
    propMenuItem.prop( 'selected', true );
    FMDrawPropertyMenu.EnableDisiablePrecsion( FMDrawPropertyMenu.PropertyActiveObject.data.TagUnits );
};

//=================================================================================
// This function Enable and Disables the precision.
//=================================================================================
FMDrawPropertyMenu.EnableDisiablePrecsion = function( tagUnitsSelectedInt )
{
    if ( tagUnitsSelectedInt === FMENGINEERINGUNIT.FML_FtIn16th || tagUnitsSelectedInt === FMENGINEERINGUNIT.FML_FtIn8th ||
        ( tagUnitsSelectedInt === FMENGINEERINGUNIT.FM_SiteUnits &&
            ( FMDrawPropertyMenu.PropertyActiveObject.data.TagUnitsOriginal === FMENGINEERINGUNIT.FML_FtIn16th ||
                FMDrawPropertyMenu.PropertyActiveObject.data.TagUnitsOriginal === FMENGINEERINGUNIT.FML_FtIn8th ) ) )
    {
        FMDrawPropertyMenu.EnableDisableTextBox( 'textbox-propertiesMenu-TAGPRECISION', true );
    }
    else
    {
        FMDrawPropertyMenu.EnableDisableTextBox( 'textbox-propertiesMenu-TAGPRECISION', false );
    }
};

//=================================================================================
// This function updates superscript and subscript.
//=================================================================================
FMDrawPropertyMenu.UpdateSuperscriptAndSubScriptShow = function()
{
    if ( FMDrawPropertyMenu.PropertyActiveObject.name === 'Tag' )
    {
        var dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
        var data = FMDrawPropertyMenu.PropertyActiveObject.data;
        var oldIgnoreEvent;

        if ( !(data.TagPointValueType === 0 &&data.TagFieldSelection === FMTAGFIELDSELECTION.VALUE ))
        {
            dataModel.setDataProperty( data, 'TagShowStatus', false );
            dataModel.setDataProperty(data, 'TagShowWeightsAndMeasures', false);
				if ( data.TagFieldSelection !== FMTAGFIELDSELECTION.ALARMSTATUS )
				{
					dataModel.setDataProperty( data, 'TagAlarmAnnunciation', false );
				}
				dataModel.setDataProperty( data, 'TagStatus', '' );
            dataModel.setDataProperty(data, 'TagWeightsAndMeasures', '');

            oldIgnoreEvent = FMDrawPropertyMenu.IgnoreEvent;
            FMDrawPropertyMenu.IgnoreEvent = false;

            FMDrawPropertyMenu.SetSelectionForDropdown( 'TAGSHOWSTATUS', 'TagShowStatus' );
            FMDrawPropertyMenu.SetSelectionForDropdown('TAGSHOWWEIGHTSANDMEASURES', 'TagShowWeightsAndMeasures');
            FMDrawPropertyMenu.SetSelectionForDropdown('TAGALARMANNUNCIATION', 'TagAlarmAnnunciation');

            FMDrawPropertyMenu.IgnoreEvent = oldIgnoreEvent;

            FMDrawPropertyMenu.EnableDisableTextBox( 'dropdown-propertiesMenu-TAGSHOWSTATUS', true );
            FMDrawPropertyMenu.EnableDisableTextBox('dropdown-propertiesMenu-TAGSHOWWEIGHTSANDMEASURES', true);
				if ( data.TagFieldSelection !== FMTAGFIELDSELECTION.ALARMSTATUS )
				{
					FMDrawPropertyMenu.EnableDisableTextBox( 'dropdown-propertiesMenu-TAGALARMANNUNCIATION', true );
				}
				else
				{
					FMDrawPropertyMenu.EnableDisableTextBox('dropdown-propertiesMenu-TAGALARMANNUNCIATION', false);
				}
        }
        else
        {
            oldIgnoreEvent = FMDrawPropertyMenu.IgnoreEvent;
            FMDrawPropertyMenu.IgnoreEvent = false;

            FMDrawPropertyMenu.SetSelectionForDropdown( 'TAGSHOWSTATUS', 'TagShowStatus' );
            FMDrawPropertyMenu.SetSelectionForDropdown('TAGSHOWWEIGHTSANDMEASURES', 'TagShowWeightsAndMeasures');
            FMDrawPropertyMenu.SetSelectionForDropdown('TAGALARMANNUNCIATION', 'TagAlarmAnnunciation');

            FMDrawPropertyMenu.IgnoreEvent = oldIgnoreEvent;

            FMDrawPropertyMenu.EnableDisableTextBox( 'dropdown-propertiesMenu-TAGSHOWSTATUS', false );
            FMDrawPropertyMenu.EnableDisableTextBox('dropdown-propertiesMenu-TAGSHOWWEIGHTSANDMEASURES', false);
            FMDrawPropertyMenu.EnableDisableTextBox('dropdown-propertiesMenu-TAGALARMANNUNCIATION', false);
        }
    }
};

//=================================================================================
// This function updates superscript and subscript font.
//=================================================================================
FMDrawPropertyMenu.UpdateSuperscriptAndSubScriptFont = function( dataModel, activeObject )
{
    if ( activeObject.name === 'Tag' )
    {
        var fontStrNew = FMDrawPropertyMenu.CreateHalfSizedFont( activeObject.data.font );
        dataModel.setDataProperty( activeObject.data, 'SuperScriptFont', fontStrNew );
        dataModel.setDataProperty( activeObject.data, 'SubScriptFont', fontStrNew );
    }
};

//=================================================================================
// This function creates a half size font.
//=================================================================================
FMDrawPropertyMenu.CreateHalfSizedFont = function( fontStr )
{
    FMDrawPropertyMenu.ParseFontString( fontStr );
    var fontSizeStr = FMDrawPropertyMenu.FontObject.fontSize.substring( 0, FMDrawPropertyMenu.FontObject.fontSize.length - 1 );
    var fontSize = parseInt( fontSizeStr, 10 );

    var newFontSizeStr = FMDrawPropertyMenu.FontObject.fontSize;

    if ( fontSize !== undefined )
    {
        fontSize = Math.ceil( fontSize / 2 );
        newFontSizeStr = fontSize.toString() + 'px';
    }

    var fontStrNew = FMDrawPropertyMenu.FontObject.fontStyle + ' '
        + FMDrawPropertyMenu.FontObject.fontVariant + ' '
        + FMDrawPropertyMenu.FontObject.fontWeight + ' '
        + newFontSizeStr + ' '
        + FMDrawPropertyMenu.FontObject.fontFamily;
    return fontStrNew;
};

//=========================================================
// This function will set the text block position dropdown 
// based on the object setting.
//=========================================================
FMDrawPropertyMenu.SetSelectionForDropdown = function( propertyName, fieldName )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
	if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
	}

    var fieldSelection = FMDrawPropertyMenu.PropertyActiveObject.data[fieldName];
    var selectString = '#dropdown-propertiesMenu-' + propertyName + ' option[value=\'' + fieldSelection + '\']';
    var propMenuItem = $(selectString);

    propMenuItem.prop( 'selected', true );
};

//=========================================================
// This function will set the text block alignment dropdown 
// based on the object setting.
//=========================================================
FMDrawPropertyMenu.SetTextBlockAlignmentDropdown = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    $( '#dropdown-propertiesMenu-TEXTBLOCKALIGNMENT option[data-value=\'NONE\']' ).prop( 'selected', true );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetTextBlockAlignmentFlag )
    {
        // Set default to the Middle.
        $( '#dropdown-propertiesMenu-TEXTBLOCKALIGNMENT option[data-value=\'Center\']' ).prop( 'selected', true );
        var blockLocationStr = FMDrawPropertyMenu.GetTextBlockSetting( FMDrawPropertyMenu.PropertyActiveObject.data.alignment );

        if ( blockLocationStr === 'leftCenter'
            || blockLocationStr === 'topLeft'
            || blockLocationStr === 'bottomLeft' )
        {
            $( '#dropdown-propertiesMenu-TEXTBLOCKALIGNMENT option[data-value=\'Left\']' ).prop( 'selected', true );
        }

        if ( blockLocationStr === 'rightCenter'
            || blockLocationStr === 'topRight'
            || blockLocationStr === 'bottomRight' )
        {
            $( '#dropdown-propertiesMenu-TEXTBLOCKALIGNMENT option[data-value=\'Right\']' ).prop( 'selected', true );
        }
    }
};

//=========================================================
// This function will set the Line size dropdown based 
// on the object setting.
//=========================================================
FMDrawPropertyMenu.SetLineSizeDropdown = function( strokeWidth, textBlock )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    var optionId = '#dropdown-propertiesMenu-LINESIZE option[data-value=\'NONE\']';
    $( optionId ).prop( 'selected', true );

    // Set dropdown to "None" if stroke width not present.
    if ( strokeWidth == null )
    {
        return;
    }

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetLineSizeFlag )
    {
        var found = false;

        $( '#dropdown-propertiesMenu-LINESIZE > option' ).each( function()
        {
            var optionText = $( this ).text();
            var strokeWidthStr = strokeWidth.toString() + 'pt';

            if ( optionText === strokeWidthStr )
            {
                optionId = '#dropdown-propertiesMenu-LINESIZE option[data-value=\'' + strokeWidth + '\']';
                $( optionId ).prop( 'selected', true );
                found = true;

                if ( textBlock != null )
                {
                    textBlock.calMargin = strokeWidth;
                }
            }
        } );

        if ( found === false )
        {
            $( '#dropdown-propertiesMenu-LINESIZE option[data-value=\'NONE\']' ).prop( 'selected', true );
        }
    }
};

//=========================================================
// This function will set the line color based on
// the object setting.
//=========================================================
FMDrawPropertyMenu.SetLineColorDropdown = function()
{
    var rgbObj = FMDrawPropertyMenu.HexToRgb( '#ffffff' );
    var rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, 1 );
    $( '#textbox-propertiesMenu-LINECOLOR' ).css( 'background-color', rgbaStr );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetLineColorFlag )
    {
        var data = FMDrawPropertyMenu.PropertyActiveObject.data;
        var color = '#000000';

        if ( data.lineStroke != null )
        {
            color = data.lineStroke;
        }

        if ( color.indexOf( 'rgba' ) !== -1 )
        {
            var parts = color.split( ',' );
            color = parts[0] + ', ' + parts[1] + ', ' + parts[2] + ')';
        }

        $( '#textbox-propertiesMenu-LINECOLOR' ).css( 'background-color', color );

        var currentColor = $( '#textbox-propertiesMenu-LINECOLOR' ).css( 'background-color' );
        $( '#manualColor-textbox-propertiesMenu-LINECOLORSPECTRUM' ).val( FMDrawPropertyMenu.Rgb2Hex( currentColor ) );
        $( '#manualColorSampler-textbox-propertiesMenu-LINECOLORSPECTRUM' ).css( 'background-color', FMDrawPropertyMenu.Rgb2Hex( currentColor ) );
        $( '#textbox-fillColorSpectrum-propertiesMenu-LINECOLORSPECTRUM' ).spectrum( 'set', FMDrawPropertyMenu.Rgb2Hex( currentColor ) );

        // When true, ignore this event since it is coming from the model change
        // and not from the user.
        if ( FMDrawPropertyMenu.IgnoreEvent == null || FMDrawPropertyMenu.IgnoreEvent === false )
        {
            var recentColors = FMDrawPropertyMenu.PropertyActiveObject.diagram.model.modelData.PropertyMenuRecentColors;
            $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-1' ).css( 'background-color', recentColors.LineColor1 );
            $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-2' ).css( 'background-color', recentColors.LineColor2 );
            $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-3' ).css( 'background-color', recentColors.LineColor3 );
            $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-4' ).css( 'background-color', recentColors.LineColor4 );
            $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-5' ).css( 'background-color', recentColors.LineColor5 );
            $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-6' ).css( 'background-color', recentColors.LineColor6 );
            $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-7' ).css( 'background-color', recentColors.LineColor7 );
            $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-8' ).css( 'background-color', recentColors.LineColor8 );
        }
    }
};

//======================================================================================================
// This function will set the line style pattern based on the saved value.
//======================================================================================================
FMDrawPropertyMenu.SetLineStyleDropdown = function()
{
    var lineStylePatternDropdownCanvas = document.getElementById( 'canvas-propertiesMenu-LINESTYLE' );

    if ( lineStylePatternDropdownCanvas == null )
    {
        return;
    }

    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    FMDrawPatternPalette.CreateLineStylePattern( 'canvas-propertiesMenu-LINESTYLE', 1 );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetLineStyleFlag )
    {
        var data = FMDrawPropertyMenu.PropertyActiveObject.data;

        if ( data.lineStylePatternNumber != null )
        {
            var lineStylePatternNumberInt = parseInt( data.lineStylePatternNumber );
            FMDrawPatternPalette.CreateLineStylePattern( 'canvas-propertiesMenu-LINESTYLE', lineStylePatternNumberInt );
        }
    }
};

//================================================================================
// This function will populate the fill pattern dropdown with an selected image
// or empty image.
//================================================================================
FMDrawPropertyMenu.SetFillPatternDropdown = function()
{
    var patternDropdownCanvas = document.getElementById( 'canvas-propertiesMenu-FILLPATTERN' );

    if ( patternDropdownCanvas == null )
    {
        return;
    }

    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    FMDrawPatternPalette.CreatePattern( 'canvas-propertiesMenu-FILLPATTERN', 1 );

    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetPatternFlag )
    {
        var patternNumber = null;
        var data = FMDrawPropertyMenu.PropertyActiveObject.data;

        if ( typeof ( data.color ) === 'object' )
        {
            patternNumber = data.patternImageName;
        }

        if ( patternNumber != null )
        {
            var patternNumberInt = parseInt( patternNumber );
            FMDrawPatternPalette.CreatePattern( 'canvas-propertiesMenu-FILLPATTERN', patternNumberInt );
        }
    }
};

//================================================================================
// This function will populate the Demo Percent Textbox
//================================================================================
FMDrawPropertyMenu.SetBarType = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }
    var data = FMDrawPropertyMenu.PropertyActiveObject.data;
    $( '#dropdown-propertiesMenu-BARTYPE' ).val( data.barType );
};

//================================================================================
// This function will populate the use product color dropdown.
//================================================================================
FMDrawPropertyMenu.SetUseProductColor = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

	var onOffSetting = "false";
	var data = FMDrawPropertyMenu.PropertyActiveObject.data;

	if ( data.useProductColor )
	{
		onOffSetting = "true";
	}

	$('#dropdown-propertiesMenu-USEPRODUCTCOLOR').val(onOffSetting);
};

//================================================================================
// This function will populate the use alarm level dropdown.
//================================================================================
FMDrawPropertyMenu.SetUseAlarmLevel = function ()
{
	// When true, ignore this event since it is coming from the canvas.startTransaction
	// and not from the user.
	if (FMDrawPropertyMenu.IgnoreEvent)
	{
		return;
	}

	var onOffSetting = "false";
	var data = FMDrawPropertyMenu.PropertyActiveObject.data;

	if (data.useAlarmLevel)
	{
		onOffSetting = "true";
	}

	$('#dropdown-propertiesMenu-USEALARMLEVEL').val(onOffSetting);
};

//================================================================================
// This function will populate the Demo Percent Textbox
//================================================================================
FMDrawPropertyMenu.SetDemoPercent = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }
    var data = FMDrawPropertyMenu.PropertyActiveObject.data;
    var percent = data.demoPercent;
    $( '#dropdown-propertiesMenu-DEMOVALUEPERCENT' ).val( percent );
};

FMDrawPropertyMenu.SetAlternateLabelText = function( id )
{
    var label = $( '#' + id );
    var alternateLabelName = $('#' + id).attr('alternateLabelName');
    var originalLabelName = $('#' + id).attr('originalLabelName');
    if (!alternateLabelName)
    {
        $('#' + id).text(originalLabelName);
    }
    else
    {
        $('#' + id).text(alternateLabelName);
    }
}

FMDrawPropertyMenu.SetOriginalLabelText = function (id)
{
    var originalLabelName = $('#' + id).attr('originalLabelName');
    if (originalLabelName) {
        $('#' + id).text(originalLabelName);
    }
}
//================================================================================
// This function will populate the Point tag id textbox
//================================================================================
FMDrawPropertyMenu.SetPointAndTagID = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }
    var data = FMDrawPropertyMenu.PropertyActiveObject.data;
    $( '#textbox-propertiesMenu-POINTANDTAGID' ).attr( 'readonly', true );
    $( '#textbox-propertiesMenu-POINTANDTAGID' ).val( data.TagPointIDAndTagID );
    $('#textbox-propertiesMenu-POINTANDTAGID').css('border-color', '#9b9b9b');
    if ( data.PointTemplateTagSelectionIndicator )
    {
        FMDrawPropertyMenu.SetAlternateLabelText( 'label-propertiesMenu-POINTANDTAGID' );
    }
    else
    {
        FMDrawPropertyMenu.SetOriginalLabelText( 'label-propertiesMenu-POINTANDTAGID' );
    }
};

//================================================================================
// This function will return true if animations are allowed or false if they are not
//================================================================================
FMDrawPropertyMenu.IsAnimationManagerEnabled = function () {
    // SetAnimationID should of already been called so we just need to check if the animation button is visible
    var animationSelection = document.getElementById("textbox-propertiesMenu-ANIMATIONBUTTON");
    // return the inverse of hidden to control wheter the menu item is enabled or not
    if (animationSelection != undefined &&
        animationSelection != null) {
        return !animationSelection.hidden;
    }
    // return false if the animation button does not exist in the properties window
    return false;
}
//================================================================================
// This function will populate the animation textbox
//================================================================================
FMDrawPropertyMenu.SetAnimationID = function () {
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    var checkAlarmSetting = false;
    
    if (FMDrawPropertyMenu.IgnoreEvent) {
        return;
    }

    var animationPointValueAssignments = null;
    var selecteddisplay = "######";

    for (nextFilter = 0; nextFilter < FMDrawPropertyMenu.SectionSubStateFilterList.length; nextFilter++) {
        var fieldName = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].FieldName;

        if (fieldName === "#tr-propertiesMenu-TAGALARMANNUNCIATION") {
            if(FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display === "show")
            {
                checkAlarmSetting = true;
            }
            break;
        }
    }

    var dropDown = document.getElementById("dropdown-propertiesMenu-TAGALARMANNUNCIATION");

    if (dropDown != undefined &&
        dropDown != null)
    {
        var animationSelection = document.getElementById("textbox-propertiesMenu-ANIMATIONBUTTON");
        if (checkAlarmSetting === true && dropDown.value == "true")
        {
            if (animationSelection != undefined &&
                animationSelection != null)
            {
                animationSelection.hidden = true;
                return;
            }
        }
        else {
            if (animationSelection != undefined &&
                animationSelection != null) {
                animationSelection.hidden = false;
            }
        }
    }
    if (FMDrawPropertyMenu.PropertyActiveObject != null)
    {
        var activeData = FMDrawPropertyMenu.PropertyActiveObject.data;
        animationPointValueAssignments = activeData.AnimationPointValueAssignments;
        if (animationPointValueAssignments != undefined &&
            animationPointValueAssignments != null)
        {
            // if the id field is present use that if not use the guid
            if (animationPointValueAssignments.AnimationID != null &&
                animationPointValueAssignments.AnimationID != undefined)
            {
                selecteddisplay = animationPointValueAssignments.AnimationID;
            }
            else
            {
                selecteddisplay = animationPointValueAssignments.AnimationGuid.toString();
            }

            if (dropDown != undefined &&
                dropDown != null)
            {
                dropDown.hidden = true;
            }
        }
        else
        {
            if (dropDown != undefined &&
                dropDown != null)
            {
                dropDown.hidden = false;
            }
        }
    }
    else
    {
        if (dropDown != undefined &&
            dropDown != null) {
            dropDown.hidden = false;
        }
    }

    $('#textbox-propertiesMenu-ANIMATIONBUTTON').attr('readonly', true);
    $('#textbox-propertiesMenu-ANIMATIONBUTTON').val(selecteddisplay);
    $('#textbox-propertiesMenu-ANIMATIONBUTTON').css('border-color', '#9b9b9b');
     FMDrawPropertyMenu.SetOriginalLabelText('label-propertiesMenu-ANIMATIONBUTTON');
};

//================================================================================
// This function will populate the Point id textbox
//================================================================================
FMDrawPropertyMenu.SetPointID = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }


    var data = FMDrawPropertyMenu.PropertyActiveObject.data;
    $( '#textbox-propertiesMenu-POINTID' ).attr( 'readonly', true );
    $( '#textbox-propertiesMenu-POINTID' ).css( 'border-color', '#9b9b9b' );
    if ( FMDrawPropertyMenu.MultiSelectionFlag === false || FMDrawPropertyMenu.manualSetPointIDFlag )
    {
        $( '#textbox-propertiesMenu-POINTID' ).val( data.TagPointID );
    }
    else
    {
        $( '#textbox-propertiesMenu-POINTID' ).val( ' ' );
    }
};

FMDrawPropertyMenu.SetButtonActionTargetLabel = function( actionType )
{
    
    var label = ButtonActionTargetDefaultLabel;
    var obj = FMDrawPropertyMenu.PropertyActiveObject;
    var isPointTemplateObject = ( obj && obj.data && obj.data.PointTemplateTagSelectionIndicator );
    switch ( actionType )
    {
    	case ButtonActionTypePointTrend:
    	    label = (isPointTemplateObject) ? 'Point Template ID' : 'Point ID';
	        break;
	 	case ButtonActionTypeCommand:
	 	    label = (isPointTemplateObject) ? 'Point Template and Value ID' : 'Point and Value ID';
	 		break;
	 	case ButtonActionTypeGraphic:
	 		label = 'Graphic';
	 		break;
	 	case ButtonActionTypeLinkedGraphic:
	 		label = 'Linked Graphic';
	 		break;
	 	case ButtonActionTypeReport:
	 		label = 'Report';
	 		break;
	 	case ButtonActionTypeHelp:
	 		label = 'Help';
	 		break;
	 	case ButtonActionTypeDetail:
	 		label = (isPointTemplateObject) ? 'Point Template ID' : 'Point ID';
	 		break;
    case ButtonActionTypePointHistory:
      label = 'Point ID';
      break;
    case ButtonActionTypeTemplate:
	 		label = 'Template';
	 		break;
	 	case ButtonActionTypeLinkedTemplate:
	 		label = 'Linked Template';
	 		break;
	 	case ButtonActionTypeUrlLink:
	 		label = 'URL';
	 		break;
    }
    $( '#tr-propertiesMenu-BUTTONACTIONTARGET > div > label' ).text( label );
}; //================================================================================
// This function will populate the Button Action Values
//================================================================================
FMDrawPropertyMenu.SetButonActionValues = function( data )
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent || !data )
    {
        return;
    }

    //$( '#tr-propertiesMenu-BUTTONACTIONTYPE' ).show();
    //$( '#tr-propertiesMenu-BUTTONACTIONTARGET' ).show();

    FMDrawPropertyMenu.SetButtonActionTargetLabel( data.buttonActionType );
    $( '#dropdown-propertiesMenu-BUTTONACTIONTYPE' ).val( ( data.buttonActionType ) ? data.buttonActionType : ButtonActionTypeNoneValue );
    $( '#textbox-propertiesMenu-BUTTONACTIONTARGET' ).attr( 'readonly', true );
    $( '#textbox-propertiesMenu-BUTTONACTIONTARGET' ).css( 'border-color', '#9b9b9b' );
    $( '#textbox-propertiesMenu-BUTTONACTIONTARGET' ).val( ( data.buttonActionObjectId ) ? data.buttonActionObjectId : '' );
}; //================================================================================
// This function will populate the UserVariableLimits value textbox
//================================================================================
FMDrawPropertyMenu.SetUserVariableLimits = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    var data = FMDrawPropertyMenu.PropertyActiveObject.data;
    $( '#dropdown-propertiesMenu-USETAGLIMITS' ).val( data.useTagLimits );
    if ( data.useTagLimits === 'true' )
    {
        $( '#textbox-propertiesMenu-MAXVALUE' ).attr( 'disabled', true );
        $( '#textbox-propertiesMenu-MINVALUE' ).attr( 'disabled', true );
    }

    else
    {
        $( '#textbox-propertiesMenu-MAXVALUE' ).attr( 'disabled', false );
        $( '#textbox-propertiesMenu-MINVALUE' ).attr( 'disabled', false );
    }
};

//================================================================================
// This function will populate the Mininum value textbox
//================================================================================
FMDrawPropertyMenu.SetMinimumValue = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }

    var data = FMDrawPropertyMenu.PropertyActiveObject.data;
    if ( data.useTagLimits === 'true' )
    {
        $( '#textbox-propertiesMenu-MINVALUE' ).val( data.minVal );
    }
    else
    {
        $( '#textbox-propertiesMenu-MINVALUE' ).val( data.minUserVal );
    }
};

//================================================================================
// This function will populate the Maximum value textbox
//================================================================================
FMDrawPropertyMenu.SetMaximumValue = function()
{
    // When true, ignore this event since it is coming from the canvas.startTransaction
    // and not from the user.
    if ( FMDrawPropertyMenu.IgnoreEvent )
    {
        return;
    }
    var data = FMDrawPropertyMenu.PropertyActiveObject.data;
    if ( data.useTagLimits === 'true' )
    {
        $( '#textbox-propertiesMenu-MAXVALUE' ).val( data.maxVal );
    }
    else
    {
        $( '#textbox-propertiesMenu-MAXVALUE' ).val( data.maxUserVal );
    }
};

//=============================================================================================
// This function is a pass through to handle pattern selections.  It will be either a
// fill pattern or a line style pattern.
//=============================================================================================
FMDrawPropertyMenu.PatternOnClick = function( canvasTagId, patternNumber, propertyName )
{
    if ( FMDrawPropertyMenu.PropertyActiveObject == null )
    {
        return;
    }

    if ( propertyName == null || propertyName === 'FILLPATTERNPALETTE' )
    {
        FMDrawPropertyMenu.HandleFillPatternOnClick( canvasTagId, patternNumber, false, -99, null );
    }
    else if ( propertyName === 'LINESTYLEPALETTE' )
    {
        FMDrawPropertyMenu.HandleLineStylePatternOnClick( canvasTagId, patternNumber );
    }
    else if ( propertyName === 'LINEFROMARROWPALETTE' )
    {
        FMDrawPropertyMenu.HandleLineArrowPatternOnClick( canvasTagId, patternNumber, 'FromArrow' );
    }
    else if ( propertyName === 'LINETOARROWPALETTE' )
    {
        FMDrawPropertyMenu.HandleLineArrowPatternOnClick( canvasTagId, patternNumber, 'ToArrow' );
    }
};

//====================================================================================
// This function will handle the user line arrow pattern selection.
//====================================================================================
FMDrawPropertyMenu.HandleLineArrowPatternOnClick = function( canvasTagId, arrowNumber, arrowDirection )
{
    // Set the Line Arrow Pattern dropdown pattern.
    var lineArrowDropdownId = 'canvas-propertiesMenu-LINEFROMARROW';

    if ( arrowDirection === 'ToArrow' )
    {
        lineArrowDropdownId = 'canvas-propertiesMenu-LINETOARROW';
    }

    var patternCanvas = document.getElementById( lineArrowDropdownId );

    if ( patternCanvas == null )
    {
        return;
    }

    var dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
    var canvas = FMDrawIndex.GetActiveTabGoJSDiagramObject();

    var lineColor;
    var lineSize;
    var nextObjIndex;
    var activeObject;

    // Populate the Line Style Pattern textbox with the selected pattern.
    var arrowNumberInt = parseInt( arrowNumber );
    canvas.startTransaction( 'propertyUpdate' );

    if ( arrowDirection === 'ToArrow' )
    {
        FMDrawPatternPalette.CreateToArrowPattern( lineArrowDropdownId, arrowNumberInt );

        for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
        {
            activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

            // Arrow number equal to 1 means no arrow. Set the toArrow to empty string and that
            // removes the arrow.
            if ( arrowNumber === '1' )
            {
                dataModel.setDataProperty( activeObject.data, 'toArrow', '' );
                dataModel.setDataProperty(activeObject.data, 'toArrowFill', '');
                dataModel.setDataProperty(activeObject.data, 'toArrowScale', '');
                dataModel.setDataProperty(activeObject.data, 'toArrowStroke', '');
                FMDrawIndex.defaultArchetype.toArrow = '';
                FMDrawIndex.defaultArchetype.toArrowFill = '';
                FMDrawIndex.defaultArchetype.toArrowScale = '';
                FMDrawIndex.defaultArchetype.toArrowStroke = '';
            }
            else
            {
                lineColor = activeObject.data.lineStroke == null ? '#000000' : activeObject.data.lineStroke;
                lineSize = activeObject.data.strokeWidth == null ? 2 : activeObject.data.strokeWidth;

                dataModel.setDataProperty( activeObject.data, 'toArrow', FMDrawPatternPalette.ToArrowNames[arrowNumber - 1] );
                dataModel.setDataProperty( activeObject.data, 'toArrowFill', lineColor );
                dataModel.setDataProperty( activeObject.data, 'toArrowScale', lineSize );
                dataModel.setDataProperty( activeObject.data, 'toArrowStroke', lineColor );

                FMDrawIndex.defaultArchetype.toArrow = FMDrawPatternPalette.ToArrowNames[arrowNumber - 1];
                FMDrawIndex.defaultArchetype.toArrowFill = lineColor;
                FMDrawIndex.defaultArchetype.toArrowScale = lineSize;
                FMDrawIndex.defaultArchetype.toArrowStroke = lineColor;
                dataModel.setDataProperty(activeObject.data, 'forceGeoEndPositionBindings', true);
                dataModel.setDataProperty(activeObject.data, 'forceGeoStartPositionBindings', true);
            }
        }

        FMDrawPropertyMenu.manualSetLineToArrowFlag = true;
    }
    else
    {
        FMDrawPatternPalette.CreateFromArrowPattern( lineArrowDropdownId, arrowNumberInt );

        for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
        {
            activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

            // Arrow number equal to 1 means no arrow. Set the fromArrow to empty string and that
            // removes the arrow.
            if ( arrowNumber === '1' )
            {
                dataModel.setDataProperty(activeObject.data, 'fromArrow', '');
                dataModel.setDataProperty(activeObject.data, 'fromArrowFill', '');
                dataModel.setDataProperty(activeObject.data, 'fromArrowScale', '');
                dataModel.setDataProperty(activeObject.data, 'fromArrowStroke', '');
                FMDrawIndex.defaultArchetype.fromArrow = '';
                FMDrawIndex.defaultArchetype.fromArrowFill = '';
                FMDrawIndex.defaultArchetype.fromArrowScale = '';
                FMDrawIndex.defaultArchetype.fromArrowStroke = '';
            }
            else
            {
                lineColor = activeObject.data.lineStroke == null ? '#000000' : activeObject.data.lineStroke;
                lineSize = activeObject.data.strokeWidth == null ? 2 : activeObject.data.strokeWidth;

                dataModel.setDataProperty( activeObject.data, 'fromArrow', FMDrawPatternPalette.FromArrowNames[arrowNumber - 1] );
                dataModel.setDataProperty( activeObject.data, 'fromArrowFill', lineColor );
                dataModel.setDataProperty( activeObject.data, 'fromArrowScale', lineSize );
                dataModel.setDataProperty( activeObject.data, 'fromArrowStroke', lineColor );

                FMDrawIndex.defaultArchetype.fromArrow = FMDrawPatternPalette.FromArrowNames[arrowNumber - 1];
                FMDrawIndex.defaultArchetype.fromArrowFill = lineColor;
                FMDrawIndex.defaultArchetype.fromArrowScale = lineSize;
                FMDrawIndex.defaultArchetype.fromArrowStroke = lineColor;
            }
        }

        FMDrawPropertyMenu.manualSetLineFromArrowFlag = true;
    }

    canvas.commitTransaction( 'propertyUpdate' );
};

//====================================================================================
// This function will handle the user line style pattern selection.
//====================================================================================
FMDrawPropertyMenu.HandleLineStylePatternOnClick = function( canvasTagId, patternNumber )
{
    // Set the Line Style Pattern dropdown pattern.
    var patternCanvas = document.getElementById( 'canvas-propertiesMenu-LINESTYLE' );

    if ( patternCanvas == null )
    {
        return;
    }

    // Populate the Line Style Pattern textbox with the selected pattern.
    var patternNumberInt = parseInt( patternNumber );
    FMDrawPatternPalette.CreateLineStylePattern( 'canvas-propertiesMenu-LINESTYLE', patternNumberInt );

    FMDrawPropertyMenu.IgnoreEvent = true;
    var dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

    for ( var nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
    {
        var activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
        dataModel.setDataProperty( activeObject.data, 'strokeDashArray', FMDrawPatternPalette.LineStylePatterns[patternNumberInt - 1] );
        dataModel.setDataProperty( activeObject.data, 'lineStylePatternNumber', patternNumber );
}
	 FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'strokeDashArray', FMDrawPatternPalette.LineStylePatterns[patternNumberInt - 1]);
    FMDrawIndex.defaultArchetype.strokeDashArray = FMDrawPatternPalette.LineStylePatterns[patternNumberInt - 1];
    FMDrawIndex.defaultArchetype.lineStylePatternNumber = patternNumber;
    FMDrawPropertyMenu.IgnoreEvent = false;
};

//====================================================================================
// This function will handle the user pattern selection.
//====================================================================================
FMDrawPropertyMenu.HandleFillPatternOnClick = function( canvasTagId, patternNumber, ignoreTransaction, activeObjIndex, forceTransparencyToZero )
{
    // Set the Fill Pattern dropdown pattern.
    var patternCanvas = document.getElementById( 'canvas-propertiesMenu-FILLPATTERN' );

    if ( patternCanvas == null )
    {
        return;
    }
    
    // Populate the Fill Pattern textbox with the selected pattern.
    var patternNumberInt = parseInt( patternNumber );
    FMDrawPatternPalette.CreatePattern( 'canvas-propertiesMenu-FILLPATTERN', patternNumberInt );

    var dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;
    var nextObjIndex;
    var activeObjectlocal;
    var currentFillColorHex = FMDrawPropertyMenu.Rgb2Hex( $( '#textbox-propertiesMenu-FILLCOLOR' ).css( 'background-color' ) );
    var rgbObj = FMDrawPropertyMenu.HexToRgb(currentFillColorHex);
    var transparencyFloat = 1;
    
    if (forceTransparencyToZero === null) {
        transparencyFloat = FMDrawPatternPalette.GetCurrentTransparencyAsFloat();
    }
    var rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );
    
    
    // Fill Pattern Palette 1 is a blank seletion. We want to set the shape object with
    // the fill color RGBA string value.
    if ( canvasTagId === 'canvasPalatte-propertiesMenu-FILLPATTERNPALETTE-1' )
    {
        FMDrawPropertyMenu.IgnoreEvent = true;

        // Index of -99 means to do all objects. Else do just the specific object.
        if ( activeObjIndex === -99 )
        {
            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                activeObjectlocal = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                dataModel.setDataProperty(activeObjectlocal.data, 'color', rgbaStr);
                dataModel.setDataProperty(activeObjectlocal.data, 'patternImageName', undefined);
            }
        }
        else
        {
            activeObjectlocal = FMDrawPropertyMenu.selectedObjects[activeObjIndex];
            dataModel.setDataProperty(activeObjectlocal.data, 'color', rgbaStr);
            dataModel.setDataProperty(activeObjectlocal.data, 'patternImageName', undefined);
        }
		FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'color', rgbaStr);
		FMDrawIndex.defaultArchetype.color = FMDrawPropertyMenu.ConvertToRgbaString(rgbObj, transparencyFloat);
		FMDrawPropertyMenu.IgnoreEvent = false;
		FMDrawIndex.defaultArchetype.patternImageName = undefined;
        return;
    }
    
    var dynamicPattern = FMDrawPatternPalette.CreatePatternForOperate( patternNumberInt );

    // Set the shape object pattern.
    var brush = new go.Brush( go.Brush.Pattern );
    brush.pattern = dynamicPattern;

    FMDrawPropertyMenu.IgnoreEvent = true;

    //Make sure to wrap these changes into a transaction
    if ( ignoreTransaction === false )
    {
        FMDrawPropertyMenu.PropertyActiveObject.diagram.startTransaction( 'updatecolor' );
    }

    // Index of -99 means to do all objects. Else do just the specific object.
    if ( activeObjIndex === -99 )
    {
        for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
        {
            activeObjectlocal = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
            dataModel.setDataProperty( activeObjectlocal.data, 'color', brush.copy() );
            dataModel.setDataProperty( activeObjectlocal.data, 'patternImageName', patternNumber );
        }
    }
    else
    {
        activeObjectlocal = FMDrawPropertyMenu.selectedObjects[activeObjIndex];
        dataModel.setDataProperty( activeObjectlocal.data, 'color', brush.copy() );
        dataModel.setDataProperty( activeObjectlocal.data, 'patternImageName', patternNumber );
    }
    	FMDrawIndex.defaultArchetype.patternFillColor = currentFillColorHex;
    FMDrawIndex.defaultArchetype.color = brush.copy();
    FMDrawIndex.defaultArchetype.patternImageName = patternNumber;
    FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'color', FMDrawIndex.defaultArchetype.color);
    FMDrawPropertyMenu.manualSetPatternFlag = true;

    if ( ignoreTransaction === false )
    {
        FMDrawPropertyMenu.PropertyActiveObject.diagram.commitTransaction( 'updatecolor' );
    }
    
    FMDrawPropertyMenu.IgnoreEvent = false;
};

//=========================================================
// This function will create the pattern palette.
//=========================================================
FMDrawPropertyMenu.CreatePatterns = function()
{
    for ( var nextPatternIndex = 1; nextPatternIndex <= FMDrawPropertyMenu.FillPatternCount; nextPatternIndex++ )
    {
        var canvasPatternId = 'canvasPalatte-propertiesMenu-FILLPATTERNPALETTE-' + nextPatternIndex;
        FMDrawPatternPalette.CreatePattern( canvasPatternId, nextPatternIndex );
    }
};

//=========================================================
// This function will create the line style pattern palette.
//=========================================================
FMDrawPropertyMenu.CreateLineStylePatterns = function()
{
    for ( var nextStyleIndex = 1; nextStyleIndex <= FMDrawPropertyMenu.LineStylePatternCount; nextStyleIndex++ )
    {
        var canvasStylePatternId = 'canvasPalatte-propertiesMenu-LINESTYLEPALETTE-' + nextStyleIndex;
        FMDrawPatternPalette.CreateLineStylePattern( canvasStylePatternId, nextStyleIndex );
    }
}; //==============================================================
// This function will create the line To arrow pattern palette.
//==============================================================
FMDrawPropertyMenu.CreateLineToArrowPatterns = function()
{
    for ( var nextArrowIndex = 1; nextArrowIndex <= FMDrawPropertyMenu.LineArrowPatternCount; nextArrowIndex++ )
    {
        var canvasStylePatternId = 'canvasPalatte-propertiesMenu-LINETOARROWPALETTE-' + nextArrowIndex;
        FMDrawPatternPalette.CreateToArrowPattern( canvasStylePatternId, nextArrowIndex );
    }
};

//===============================================================
// This function will create the line From arrow pattern palette.
//===============================================================
FMDrawPropertyMenu.CreateLineFromArrowPatterns = function()
{
    for ( var nextArrowIndex = 1; nextArrowIndex <= FMDrawPropertyMenu.LineArrowPatternCount; nextArrowIndex++ )
    {
        var canvasStylePatternId = 'canvasPalatte-propertiesMenu-LINEFROMARROWPALETTE-' + nextArrowIndex;
        FMDrawPatternPalette.CreateFromArrowPattern( canvasStylePatternId, nextArrowIndex );
    }
};

//=========================================================
// This function will handle the Pattern Palette menu
// expansion.
//=========================================================
FMDrawPropertyMenu.ExpandPatternPalette = function( tagId )
{
    var tdspect = document.getElementById( tagId );

    var newTagId;

    if ( tdspect == null )
    {
        newTagId = tagId + 'PALETTE';
        tdspect = document.getElementById( newTagId );
    }

    if ( tdspect != null )
    {
        // Collapses the color palettes.
        var spectrumTagIdList = [];
        spectrumTagIdList.push( 'td-fillColorSpectrum-propertiesMenu-FILLCOLORSPECTRUM' );
        spectrumTagIdList.push( 'td-fillColorSpectrum-propertiesMenu-LINECOLORSPECTRUM' );
        spectrumTagIdList.push( 'td-fillColorSpectrum-propertiesMenu-TEXTCOLORSPECTRUM' );
        spectrumTagIdList.push( 'td-fillColorSpectrum-propertiesMenu-BGFILLCOLORSPECTRUM' );
        spectrumTagIdList.push( 'td-fillColorSpectrum-propertiesMenu-PATTERNCOLORSPECTRUM' );

        for ( var nextSpectrum = 0; nextSpectrum < 5; nextSpectrum++ )
        {
            var divSpectrum = document.getElementById( spectrumTagIdList[nextSpectrum] );

            if ( divSpectrum != null )
            {
                divSpectrum.style.display = 'none';
            }
        }

        tdspect.style.display = 'block';

        if ( newTagId === 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' )
        {
            FMDrawPropertyMenu.CreatePatterns();
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINESTYLEPALETTE' );
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINETOARROWPALETTE' );
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINEFROMARROWPALETTE' );

            $( '#FillPatternPalette-scroll-FILLPATTERNPALETTE' ).niceScroll().show();
            $( '#FillPatternPalette-scroll-FILLPATTERNPALETTE' ).getNiceScroll().resize();

            // When expanded the scroll will be at 200.54 + 245 = 445.54
            $( '#ascrail2001' ).css( { top: FMDrawPropertyMenu.fillPatternScrollLocation } );
        }

        if ( newTagId === 'td-fillPatternPalette-propertiesMenu-LINESTYLEPALETTE' )
        {
            FMDrawPropertyMenu.CreateLineStylePatterns();
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' );
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINETOARROWPALETTE' );
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINEFROMARROWPALETTE' );

            $( '#FillPatternPalette-scroll-LINESTYLEPALETTE' ).niceScroll().show();
            $( '#FillPatternPalette-scroll-LINESTYLEPALETTE' ).getNiceScroll().resize();

            // When expanded the scroll will be at 308.34 + 245 = 553.34
            $( '#ascrail2002' ).css( { top: FMDrawPropertyMenu.lineStyleScrollLocation } );
        }

        if ( newTagId === 'td-fillPatternPalette-propertiesMenu-LINETOARROWPALETTE' )
        {
            FMDrawPropertyMenu.CreateLineToArrowPatterns();
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINESTYLEPALETTE' );
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' );
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINEFROMARROWPALETTE' );

            $( '#FillPatternPalette-scroll-LINETOARROWPALETTE' ).niceScroll().show();
            $( '#FillPatternPalette-scroll-LINETOARROWPALETTE' ).getNiceScroll().resize();

            $( '#ascrail2003' ).css( { top: FMDrawPropertyMenu.lineToScrollLocation } );
        }

        if ( newTagId === 'td-fillPatternPalette-propertiesMenu-LINEFROMARROWPALETTE' )
        {
            FMDrawPropertyMenu.CreateLineFromArrowPatterns();
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINESTYLEPALETTE' );
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' );
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINETOARROWPALETTE' );

            $( '#FillPatternPalette-scroll-LINEFROMARROWPALETTE' ).niceScroll().show();
            $( '#FillPatternPalette-scroll-LINEFROMARROWPALETTE' ).getNiceScroll().resize();

            $( '#ascrail2004' ).css( { top: FMDrawPropertyMenu.lineFromScrollLocation } );
        }
    }
};

//============================================================================
// This function will handle the pattern palette menu
// collapsing.
//============================================================================
FMDrawPropertyMenu.CollapsePatternPalette = function( tagId )
{
    var tdspect = document.getElementById( tagId );

    if ( tagId === 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' )
    {
        $( '#FillPatternPalette-scroll-FILLPATTERNPALETTE' ).niceScroll().hide();
    }

    if ( tagId === 'td-fillPatternPalette-propertiesMenu-LINESTYLEPALETTE' )
    {
        $( '#FillPatternPalette-scroll-LINESTYLEPALETTE' ).niceScroll().hide();
    }

    if ( tagId === 'td-fillPatternPalette-propertiesMenu-LINETOARROWPALETTE' )
    {
        $( '#FillPatternPalette-scroll-LINETOARROWPALETTE' ).niceScroll().hide();
    }

    if ( tagId === 'td-fillPatternPalette-propertiesMenu-LINEFROMARROWPALETTE' )
    {
        $( '#FillPatternPalette-scroll-LINEFROMARROWPALETTE' ).niceScroll().hide();
    }

    // The drawing object is not selected.
    if ( FMDrawPropertyMenu.PropertyActiveObject == null )
    {
        tdspect.style.display = 'none';
        return;
    }

    tdspect.style.display = 'none';
};

//=========================================================
// This function will handle the Color Spectrum menu
// expansion.
//=========================================================
FMDrawPropertyMenu.ExpandColorSpectrum = function( tagId )
{
    var tdspect = document.getElementById( tagId );

    if ( tdspect == null )
    {
        var newTagId = tagId + 'SPECTRUM';
        tdspect = document.getElementById( newTagId );
    }

    if ( tdspect != null )
    {
        if ( tagId === 'td-fillColorSpectrum-propertiesMenu-FILLCOLOR' || tagId === 'td-fillColorSpectrum-propertiesMenu-PATTERNCOLOR' )
        {
            FMDrawPropertyMenu.FillColorExpanded = true;
        }

        if ( tagId === 'td-fillColorSpectrum-propertiesMenu-LINECOLOR' )
        {
            FMDrawPropertyMenu.LineColorExpanded = true;
        }

        // Collapsing the pattern palette for the scrollbar to reset.
        FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINESTYLEPALETTE' );
        FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' );
        FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINEFROMARROWPALETTE' );
        FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-LINETOARROWPALETTE' );
        tdspect.style.display = 'block';
    }
};

FMDrawPropertyMenu.SetLineAndPolygonStroke = function( diagram )
{
	var lineTool = diagram.toolManager.findTool(lineToolName);
	if (lineTool) {
		if (lineTool.archetypePartData && lineTool.archetypePartData.stroke) {
			lineTool.archetypePartData.stroke = FMDrawIndex.defaultArchetype.lineStroke;
		}
		if (lineTool.temporaryShape && lineTool.temporaryShape.stroke) {
			lineTool.temporaryShape.stroke = FMDrawIndex.defaultArchetype.lineStroke;
		}
	}

	var polyTool = diagram.toolManager.findTool(polygonDrawingToolName);
	if (polyTool) {
		if (polyTool.archetypePartData && polyTool.archetypePartData.stroke) {
			polyTool.archetypePartData.stroke = FMDrawIndex.defaultArchetype.lineStroke;
		}
		if (polyTool.temporaryShape && polyTool.temporaryShape.stroke) {
			polyTool.temporaryShape.stroke = FMDrawIndex.defaultArchetype.lineStroke;
		}
	}
}

//=========================================================
// This function will handle the Color Spectrum menu
// collapsing.
//=========================================================
FMDrawPropertyMenu.CollapseColorSpectrum = function( tagId, propertyName )
{
    var recentColors;
    var transparency;
    var transparencyFloat;
    var rgbObj;
    var rgbStr;
    var patternNumber;
    var patternTagId;
    var nextObjIndex;
    var activeObject;
    var ignoreTransaction = false;

    var manualColorTextboxId = '#manualColor-textbox-propertiesMenu-' + propertyName;
    var manualColorSampleTextboxId = '#manualColorSampler-textbox-propertiesMenu-' + propertyName;
    var textboxColorSpectrumId = '#textbox-fillColorSpectrum-propertiesMenu-' + propertyName;
    var textboxColorId = '#textbox-propertiesMenu-' + propertyName.replace( 'SPECTRUM', '' );
    var newColor = $( manualColorTextboxId ).val();
    var tdspect = document.getElementById( tagId );

    // The drawing object is not selected.
    if ( FMDrawPropertyMenu.PropertyActiveObject == null )
    {
        tdspect.style.display = 'none';
        return;
    }

    if ( newColor != null )
    {
        newColor = newColor.trim();
    }

    if ( newColor != null && newColor.length === 7 && FMDrawPropertyMenu.ValidateColorHexString( newColor ) )
    {
        
        tdspect.style.display = 'none';

        $( textboxColorId ).css( 'background-color', newColor );
        FMDrawPropertyMenu.UpdateTextColorRecent( newColor, propertyName );

        var dataModel = FMDrawPropertyMenu.PropertyActiveObject.diagram.model;

        if ( propertyName === 'TEXTCOLORSPECTRUM' )
        {
            FMDrawPropertyMenu.IgnoreEvent = true;

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                FMDrawPropertyMenu.PropertyActiveObject.diagram.startTransaction( 'updatecolor' );
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                dataModel.setDataProperty( activeObject.data, 'stroke', newColor );
                FMDrawPropertyMenu.PropertyActiveObject.diagram.commitTransaction( 'updatecolor' );
            }

            FMDrawPropertyMenu.manualSetTextColorFlag = true;

	         FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'stroke', newColor);
            FMDrawIndex.defaultArchetype.stroke = newColor;
            recentColors = FMDrawPropertyMenu.PropertyActiveObject.diagram.model.modelData.PropertyMenuRecentColors;
            recentColors.TextColor1 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-1' ).css( 'background-color' ) );
            recentColors.TextColor2 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-2' ).css( 'background-color' ) );
            recentColors.TextColor3 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-3' ).css( 'background-color' ) );
            recentColors.TextColor4 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-4' ).css( 'background-color' ) );
            recentColors.TextColor5 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-5' ).css( 'background-color' ) );
            recentColors.TextColor6 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-6' ).css( 'background-color' ) );
            recentColors.TextColor7 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-7' ).css( 'background-color' ) );
            recentColors.TextColor8 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-TEXTCOLORSPECTRUM-8' ).css( 'background-color' ) );

            FMDrawPropertyMenu.IgnoreEvent = false;
        }
        
        if (propertyName === 'FILLCOLORSPECTRUM')
        {
            FMDrawPropertyMenu.IgnoreEvent = true;
            transparency = $( '#textbox-propertiesMenu-TRANSPARENCY' ).val();
            transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat( transparency );

            rgbObj = FMDrawPropertyMenu.HexToRgb( newColor );
            rgbStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );

            ignoreTransaction = true;
            FMDrawPropertyMenu.CreatePatterns();

            // This is in place for multi select
            var patternStrokeColor;
            if ( FMDrawPropertyMenu.MultiSelectionFlag )
            {
                patternStrokeColor = $( '#manualColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM' ).val();
            }
            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                activeObject = null;

                FMDrawPropertyMenu.PropertyActiveObject.diagram.startTransaction( 'updatecolor' );
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                if (activeObject === null)
                {
                    continue;
                }
                dataModel.setDataProperty( activeObject.data, 'color', rgbStr );
                dataModel.setDataProperty( activeObject.data, 'patternFillColor', rgbStr );

                // This is in place for multi select
                if ( FMDrawPropertyMenu.MultiSelectionFlag )
                {
                    dataModel.setDataProperty( activeObject.data, 'patternStrokeColor', patternStrokeColor );

                }

                // Update the shape object with the stroke color.
                if (activeObject.data.patternImageName != null && activeObject.data.patternImageName !== '1' && activeObject.data.patternImageName !== undefined)
                {
                    patternNumber = parseInt( activeObject.data.patternImageName );
                    patternTagId = 'canvasPalatte-propertiesMenu-FILLPATTERNPALETTE-' + activeObject.data.patternImageName;
                    FMDrawPropertyMenu.HandleFillPatternOnClick( patternTagId, patternNumber, ignoreTransaction, nextObjIndex, null );
                    FMDrawPropertyMenu.IgnoreEvent = true;
                }
                FMDrawPropertyMenu.PropertyActiveObject.diagram.commitTransaction( 'updatecolor' );
            }

            if (typeof (FMDrawIndex.defaultArchetype.color) === 'object') {
            	//apply transparency to pattern
            	FMDrawIndex.defaultArchetype.patternStrokeColor = $('#manualColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM').val();
            	FMDrawIndex.defaultArchetype.patternFillColor = newColor;
            	FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'patternFillColor', newColor);
            	FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'patternStrokeColor', $('#manualColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM').val());
            }
            else {
            	//apply transparency to preview color
            	FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'color', FMDrawPropertyMenu.ConvertToRgbaString(rgbObj, FMDrawPropertyMenu.ConvertTransparencyToFloat(FMDrawPropertyMenu.propertyPreviewNode.data.transparency)));
	            var defaultTransparency = FMDrawIndex.defaultArchetype.transparency || 0;
	            FMDrawIndex.defaultArchetype.color = newColor;
	            FMDrawIndex.defaultArchetype.patternFillColor = newColor;
            }
            FMDrawPropertyMenu.IgnoreEvent = true;
            FMDrawPropertyMenu.manualSetFillColorFlag = true;
            recentColors = FMDrawPropertyMenu.PropertyActiveObject.diagram.model.modelData.PropertyMenuRecentColors;
            recentColors.FillColor1 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-1' ).css( 'background-color' ) );
            recentColors.FillColor2 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-2' ).css( 'background-color' ) );
            recentColors.FillColor3 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-3' ).css( 'background-color' ) );
            recentColors.FillColor4 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-4' ).css( 'background-color' ) );
            recentColors.FillColor5 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-5' ).css( 'background-color' ) );
            recentColors.FillColor6 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-6' ).css( 'background-color' ) );
            recentColors.FillColor7 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-7' ).css( 'background-color' ) );
            recentColors.FillColor8 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-8' ).css( 'background-color' ) );


            // Collaspe the Pattern Palette in order for the color redraw.
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' );
            FMDrawPropertyMenu.IgnoreEvent = false;
            FMDrawPropertyMenu.FillColorExpanded = false;
            
        }
        
        if ( propertyName === 'BGFILLCOLORSPECTRUM' )
        {
            FMDrawPropertyMenu.IgnoreEvent = true;

            transparency = $('#textbox-propertiesMenu-BGTRANSPARENCY').val();
            transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat( transparency );

            rgbObj = FMDrawPropertyMenu.HexToRgb( newColor );
            rgbStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                FMDrawPropertyMenu.PropertyActiveObject.diagram.startTransaction( 'updatecolor' );
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                dataModel.setDataProperty( activeObject.data, 'bgcolor', rgbStr );
                dataModel.setDataProperty( activeObject.data, 'bgpatternFillColor', rgbStr );
                dataModel.setDataProperty( activeObject.data, 'bgpatternImageName', null );
                FMDrawPropertyMenu.PropertyActiveObject.diagram.commitTransaction( 'updatecolor' );
            }

            recentColors = FMDrawPropertyMenu.PropertyActiveObject.diagram.model.modelData.PropertyMenuRecentColors;
            recentColors.FillColor1 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-1' ).css( 'background-color' ) );
            recentColors.FillColor2 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-2' ).css( 'background-color' ) );
            recentColors.FillColor3 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-3' ).css( 'background-color' ) );
            recentColors.FillColor4 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-4' ).css( 'background-color' ) );
            recentColors.FillColor5 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-5' ).css( 'background-color' ) );
            recentColors.FillColor6 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-6' ).css( 'background-color' ) );
            recentColors.FillColor7 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-7' ).css( 'background-color' ) );
            recentColors.FillColor8 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-FILLCOLORSPECTRUM-8' ).css( 'background-color' ) );

            FMDrawPropertyMenu.IgnoreEvent = false;
        }

        if ( propertyName === 'PATTERNCOLORSPECTRUM' )
        {
            FMDrawPropertyMenu.IgnoreEvent = true;
            ignoreTransaction = true;

            // This is in place for multi select.
            var fillColor;
            if ( FMDrawPropertyMenu.MultiSelectionFlag )
            {
                transparency = $( '#textbox-propertiesMenu-TRANSPARENCY' ).val();
                transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat( transparency );
                fillColor = $( '#manualColor-textbox-propertiesMenu-FILLCOLORSPECTRUM' ).val();

                rgbObj = FMDrawPropertyMenu.HexToRgb( fillColor );
                rgbStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );
            }

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                FMDrawPropertyMenu.PropertyActiveObject.diagram.startTransaction( 'updatePatternStrokeColor' );
                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];

                dataModel.setDataProperty( activeObject.data, 'patternStrokeColor', newColor );

                // This is in place for multi select.
                if ( FMDrawPropertyMenu.MultiSelectionFlag )
                {
                    dataModel.setDataProperty( activeObject.data, 'color', rgbStr );
                    dataModel.setDataProperty( activeObject.data, 'patternFillColor', rgbStr );
                    }
						  FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'color', rgbStr);

                FMDrawPropertyMenu.CreatePatterns();

                // Update the shape object with the stroke color.
                if (activeObject.data.patternImageName != null && activeObject.data.patternImageName !== '1' && activeObject.data.patternImageName !== undefined)
                {
                    patternNumber = parseInt( activeObject.data.patternImageName );
                    patternTagId = 'canvasPalatte-propertiesMenu-FILLPATTERNPALETTE-' + activeObject.data.patternImageName;
                    FMDrawPropertyMenu.HandleFillPatternOnClick( patternTagId, patternNumber, ignoreTransaction, nextObjIndex, null );
                    FMDrawPropertyMenu.IgnoreEvent = true;
                }

                FMDrawPropertyMenu.PropertyActiveObject.diagram.commitTransaction( 'updatePatternStrokeColor' );
            }
            transparency = $( '#textbox-propertiesMenu-TRANSPARENCY' ).val();
            transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat( transparency );
            fillColor = $( '#manualColor-textbox-propertiesMenu-FILLCOLORSPECTRUM' ).val();

            rgbObj = FMDrawPropertyMenu.HexToRgb( fillColor );
            rgbStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );
            FMDrawIndex.defaultArchetype.patternFillColor = fillColor;
            FMDrawIndex.defaultArchetype.patternStrokeColor = newColor;
            FMDrawPropertyMenu.manualSetPatternColorFlag = true;
            FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'patternFillColor', fillColor);
            FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'patternStrokeColor', newColor);
            recentColors = FMDrawPropertyMenu.PropertyActiveObject.diagram.model.modelData.PropertyMenuRecentColors;
            recentColors.PatternColor1 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-1' ).css( 'background-color' ) );
            recentColors.PatternColor2 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-2' ).css( 'background-color' ) );
            recentColors.PatternColor3 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-3' ).css( 'background-color' ) );
            recentColors.PatternColor4 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-4' ).css( 'background-color' ) );
            recentColors.PatternColor5 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-5' ).css( 'background-color' ) );
            recentColors.PatternColor6 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-6' ).css( 'background-color' ) );
            recentColors.PatternColor7 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-7' ).css( 'background-color' ) );
            recentColors.PatternColor8 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-PATTERNCOLORSPECTRUM-8' ).css( 'background-color' ) );

            // Collaspe the Pattern Palette in order for the color redraw.
            FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' );
            FMDrawPropertyMenu.IgnoreEvent = false;
        }

        if ( propertyName === 'LINECOLORSPECTRUM' )
        {
            FMDrawPropertyMenu.IgnoreEvent = true;

            transparency = $( '#textbox-propertiesMenu-LINESTYLETRANSPARENCY' ).val();
            transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat( transparency );

            rgbObj = FMDrawPropertyMenu.HexToRgb( newColor );
            rgbStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );

            for ( nextObjIndex = 0; nextObjIndex < FMDrawPropertyMenu.selectedObjects.length; nextObjIndex++ )
            {
                FMDrawPropertyMenu.PropertyActiveObject.diagram.startTransaction( 'updatecolor' );

                activeObject = FMDrawPropertyMenu.selectedObjects[nextObjIndex];
                dataModel.setDataProperty( activeObject.data, 'fromArrowStroke', rgbStr );
                dataModel.setDataProperty( activeObject.data, 'toArrowStroke', rgbStr );

                if ( activeObject.data.fromArrowFill != null )
                {
                    dataModel.setDataProperty( activeObject.data, 'fromArrowFill', rgbStr );
                }

                if ( activeObject.data.toArrowFill != null )
                {
                    dataModel.setDataProperty( activeObject.data, 'toArrowFill', rgbStr );
                }
                FMDrawIndex.defaultArchetype.toArrowFill = rgbStr;
                FMDrawIndex.defaultArchetype.toArrowStroke = rgbStr;
                FMDrawIndex.defaultArchetype.fromArrowFill = rgbStr;
                FMDrawIndex.defaultArchetype.fromArrowStroke = rgbStr;

                dataModel.setDataProperty(activeObject.data, 'lineStroke', rgbStr);
                FMDrawPropertyMenu.propertyPreviewDiagram.model.setDataProperty(FMDrawPropertyMenu.propertyPreviewNode.data, 'lineStroke', rgbStr);
                FMDrawPropertyMenu.PropertyActiveObject.diagram.commitTransaction( 'updatecolor' );
            }


            FMDrawIndex.defaultArchetype.lineStroke = rgbStr;
	        FMDrawPropertyMenu.SetLineAndPolygonStroke( FMDrawPropertyMenu.PropertyActiveObject.diagram );

	        FMDrawPropertyMenu.manualSetLineColorFlag = true;

            recentColors = FMDrawPropertyMenu.PropertyActiveObject.diagram.model.modelData.PropertyMenuRecentColors;
            recentColors.LineColor1 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-1' ).css( 'background-color' ) );
            recentColors.LineColor2 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-2' ).css( 'background-color' ) );
            recentColors.LineColor3 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-3' ).css( 'background-color' ) );
            recentColors.LineColor4 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-4' ).css( 'background-color' ) );
            recentColors.LineColor5 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-5' ).css( 'background-color' ) );
            recentColors.LineColor6 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-6' ).css( 'background-color' ) );
            recentColors.LineColor7 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-7' ).css( 'background-color' ) );
            recentColors.LineColor8 = FMDrawPropertyMenu.Rgb2Hex( $( '#recentColor-textbox-propertiesMenu-LINECOLORSPECTRUM-8' ).css( 'background-color' ) );

            FMDrawPropertyMenu.IgnoreEvent = false;
            FMDrawPropertyMenu.LineColorExpanded = false;
        }

        var currentColor = $( textboxColorId ).css( 'background-color' );
        $( textboxColorSpectrumId ).spectrum( 'set', FMDrawPropertyMenu.Rgb2Hex( currentColor ) );
        $(manualColorSampleTextboxId).css('background-color', FMDrawPropertyMenu.Rgb2Hex(currentColor));
        
    }
    else
    {
        var errMsg = 'Invalid HEX color value.';
        var alertTitle = 'Input Error';
        FMLayout.Alert( errMsg, alertTitle, null );
    }
};

//=========================================================
// This function will handle the Text Color Spectrum menu
// recent color updates.
//=========================================================
FMDrawPropertyMenu.UpdateTextColorRecent = function( newColor, propertyName )
{
    var previousColor;
    for ( var nextColorIndex = 1; nextColorIndex <= 8; nextColorIndex++ )
    {
        var recentId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-' + nextColorIndex.toString();
        previousColor = $( recentId ).css( 'background-color' );
        var previousColorHex = FMDrawPropertyMenu.Rgb2Hex( previousColor );

        // Just return and do not update if the current color matches the first most
        // recent color.  The user just decided to close the palette without any
        // changes.
        if ( newColor === previousColorHex )
        {
            return;
        }
    }

    var recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-7';
    previousColor = $( recentColorId ).css( 'background-color' );
    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-8';
    $( recentColorId ).css( 'background-color', previousColor );

    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-6';
    previousColor = $( recentColorId ).css( 'background-color' );
    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-7';
    $( recentColorId ).css( 'background-color', previousColor );

    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-5';
    previousColor = $( recentColorId ).css( 'background-color' );
    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-6';
    $( recentColorId ).css( 'background-color', previousColor );

    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-4';
    previousColor = $( recentColorId ).css( 'background-color' );
    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-5';
    $( recentColorId ).css( 'background-color', previousColor );

    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-3';
    previousColor = $( recentColorId ).css( 'background-color' );
    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-4';
    $( recentColorId ).css( 'background-color', previousColor );

    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-2';
    previousColor = $( recentColorId ).css( 'background-color' );
    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-3';
    $( recentColorId ).css( 'background-color', previousColor );

    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-1';
    previousColor = $( recentColorId ).css( 'background-color' );
    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-2';
    $( recentColorId ).css( 'background-color', previousColor );

    recentColorId = '#recentColor-textbox-propertiesMenu-' + propertyName + '-1';
    $( recentColorId ).css( 'background-color', newColor );
};

//===============================================================
// This function will set the palette color and color hex texbox
// to the selected recent selection.
//===============================================================
FMDrawPropertyMenu.RecentColorSelection = function( tagId, propertyName )
{
    var id = '#' + tagId;
    var manualColorTextboxId = '#manualColor-textbox-propertiesMenu-' + propertyName;
    var manualColorSamperTextboxId = '#manualColorSampler-textbox-propertiesMenu-' + propertyName;
    var textBoxFillColorSpectrumId = '#textbox-fillColorSpectrum-propertiesMenu-' + propertyName;

    var selectedRecentColor = $( id ).css( 'background-color' );
    $( manualColorTextboxId ).val( FMDrawPropertyMenu.Rgb2Hex( selectedRecentColor ) );
    $( manualColorSamperTextboxId ).css( 'background-color', FMDrawPropertyMenu.Rgb2Hex( selectedRecentColor ) );
    $( textBoxFillColorSpectrumId ).spectrum( 'set', FMDrawPropertyMenu.Rgb2Hex( selectedRecentColor ) );
};

//===============================================================
// This function will set the color hex texbox based on the 
// movement in the palette.
//===============================================================
FMDrawPropertyMenu.SetColorHexValue = function( color, thisObj )
{
    var parts = thisObj.id.split( '-' );
    var propertyNameIndex = parts.length - 1;
    var propertyName = parts[propertyNameIndex];

    var manualColorTextboxId = '#manualColor-textbox-propertiesMenu-' + propertyName;
    var manualColorSamplerTextboxId = '#manualColorSampler-textbox-propertiesMenu-' + propertyName;


    $( manualColorTextboxId ).val( color.toHexString() );
    $( manualColorSamplerTextboxId ).css( 'background-color', color.toHexString() );
};

//==========================================================================
// This function will set the sampler text box and the spectrum based on
// the input from the manual entry.
//==========================================================================
FMDrawPropertyMenu.ManualColorChange = function( propertyName )
{
	FMDrawPropertyMenu.manualColorEntryError = false;
    var manualColorTextboxId = '#manualColor-textbox-propertiesMenu-' + propertyName;
    var newHexValue = $( manualColorTextboxId ).val();

    if ( newHexValue == null )
    {
        return;
    }

    newHexValue = newHexValue.trim();

    if ( newHexValue.length < 7 )
    {
        return;
    }

    if ( newHexValue.length === 7 && FMDrawPropertyMenu.ValidateColorHexString( newHexValue ) )
    {
        var manualColorSamplerTextboxId = '#manualColorSampler-textbox-propertiesMenu-' + propertyName;
        var spectrumTextboxId = '#textbox-fillColorSpectrum-propertiesMenu-' + propertyName;

        $( manualColorSamplerTextboxId ).css( 'background-color', newHexValue );
        $( spectrumTextboxId ).spectrum( 'set', newHexValue );
    }
    else
    {
	    FMDrawPropertyMenu.manualColorEntryError = true;
        var errMsg = 'Invalid HEX color value.';
        var alertTitle = 'Input Error';
        FMLayout.Alert(errMsg, alertTitle, null);
    }
};

//==========================================================================
// This function handles the on fucus event for the manual color textbox.
// The purpose is to set the spectrum picker based on the what is in the
// manual color textbox.
//==========================================================================
FMDrawPropertyMenu.ManualColorFocus = function (propertyName)
{
	var manualColorTextboxId = '#manualColor-textbox-propertiesMenu-' + propertyName;
	var newHexValue = $(manualColorTextboxId).val();

	if (newHexValue == null)
	{
		return;
	}

	newHexValue = newHexValue.trim();

	if (newHexValue.length < 7)
	{
		return;
	}

	if (newHexValue.length === 7 && FMDrawPropertyMenu.ValidateColorHexString(newHexValue))
	{
		var spectrumTextboxId = '#textbox-fillColorSpectrum-propertiesMenu-' + propertyName;

		// The reflow makes the selection point go to the correct location.
		$(spectrumTextboxId).spectrum("reflow");
		$(spectrumTextboxId).spectrum('set', newHexValue);
	}
	else
	{
		// This is here so there isn't an infinite loop presenting the error dialog.
		if ( FMDrawPropertyMenu.manualColorEntryError === true )
		{
			return;
		}

		var errMsg = 'Invalid HEX color value.';
		var alertTitle = 'Input Error';
		FMLayout.Alert(errMsg, alertTitle, null);
	}
};

//=======================================================================
// This function will convert the angle from a GoJS style to Visio style.
// Visio goes clockwise for negative angles and counter clockwise
// for positive angles. If the user entered in a negative value, they
// want to see the value as negative.
//=======================================================================
FMDrawPropertyMenu.ConvertAngleToVisioStyle = function( angle )
{
    if ( angle == null || angle === 'NaN' )
    {
        return 0.0;
    }

    var convertedAngle = 0.0;

    if ( angle >= 180 )
    {
        convertedAngle = 360 - angle;
    }

    if ( angle <= 180 )
    {
        convertedAngle = 0 - angle;
    }

    return convertedAngle;
};

//=======================================================================
// This function will convert the transparency value for percentage to
// a hex value.
//=======================================================================
FMDrawPropertyMenu.ConvertTransparencyToFloat = function( transparency )
{
    // Go JS: 0.0 is totally transparent. 1.0 is not transparent at all
    // We want 0% to be not transparent and 100% to be totally transparent.
    if ( transparency == null || transparency === '' || transparency === ' ' )
    {
        return 1.0;
    }

    var transparencyInt = parseInt( transparency );
    var reverseValue = 100 - transparencyInt;
    var decimalValue = reverseValue / 100.0;
    return decimalValue;
};

//=========================================================
// This function will convert rgb string to a hex 
// equivalent string.
//=========================================================
FMDrawPropertyMenu.Rgb2Hex = function( orig )
{
    var rgb = orig.replace( /\s/g, '' ).match( /^rgba?\((\d+),(\d+),(\d+)/i );
    var hexValue = ( rgb && rgb.length === 4 ) ? '#' +
                       ( '0' + parseInt( rgb[1], 10 ).toString( 16 ) ).slice( -2 ) +
                       ( '0' + parseInt( rgb[2], 10 ).toString( 16 ) ).slice( -2 ) +
                       ( '0' + parseInt( rgb[3], 10 ).toString( 16 ) ).slice( -2 ) : orig;

    return hexValue;
};

//=========================================================
// This function will convert the RGB object into a
// RGBA string.
//=========================================================
FMDrawPropertyMenu.ConvertToRgbaString = function( rgbObj, transparencyValue )
{
    var rgb = ( typeof ( rgbObj ) === 'string' ) ? FMDrawPropertyMenu.HexToRgb( rgbObj ) : rgbObj;
    var rgbStr = 'rgba(' + rgb.r.toString() + ', ' + rgb.g.toString() + ', ' + rgb.b.toString() + ', ' + transparencyValue.toString() + ')';
    return rgbStr;
};

//=========================================================
// This function will convert hex to a RGB equivalent.
//=========================================================
FMDrawPropertyMenu.HexToRgb = function( hex )
{
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec( hex );
    return result ? {
                   r: parseInt( result[1], 16 ),
                   g: parseInt( result[2], 16 ),
                   b: parseInt( result[3], 16 )
               } : null;
};

//========================================================================
// This function will validate the user HEX color value input.  It must
// be a HEX value of six characters and prefixed by a # symbol.
//========================================================================
FMDrawPropertyMenu.ValidateColorHexString = function( colorHexValueStr )
{
    if ( colorHexValueStr == null )
    {
        return false;
    }

    var rx = /#[a-f0-9]{6}/i;
    var found = colorHexValueStr.search( rx );

    if ( found === -1 )
    {
        return false;
    }

    return true;
};

function shadeColor(color, percent) {

    var R = parseInt(color.substring(1, 3), 16);
    var G = parseInt(color.substring(3, 5), 16);
    var B = parseInt(color.substring(5, 7), 16);

    R = parseInt(R * (100 + percent) / 100);
    G = parseInt(G * (100 + percent) / 100);
    B = parseInt(B * (100 +percent) / 100);

    R = (R<255) ?R: 255;
    G = (G<255) ?G: 255;
    B = (B<255) ?B: 255;

    var RR = ((R.toString(16).length==1) ?"0"+R.toString(16): R.toString(16));
    var GG = ((G.toString(16).length==1) ?"0"+G.toString(16): G.toString(16));
    var BB = ((B.toString(16).length==1) ? "0" + B.toString(16): B.toString(16));

    return "#" + RR +GG+BB;
    }

//=========================================================
// This function will parse the font string into six parts.
//=========================================================
FMDrawPropertyMenu.ParseFontString = function( fontString )
{
    var rx = /^\s*(?=(?:(?:[-a-z]+\s*){0,2}(italic|oblique))?)(?=(?:(?:[-a-z]+\s*){0,2}(small-caps))?)(?=(?:(?:[-a-z]+\s*){0,2}(bold(?:er)?|lighter|[1-9]00))?)(?:(?:normal|\1|\2|\3)\s*){0,3}((?:xx?-)?(?:small|large)|medium|smaller|larger|[.\d]+(?:\%|in|[cem]m|ex|p[ctx]))(?:\s*\/\s*(normal|[.\d]+(?:\%|in|[cem]m|ex|p[ctx])))?\s*([-,\"\sa-z]+?)\s*$/i;
    var parts = rx.exec( fontString );

    if ( parts != null )
    {
        var fontParts = new Object();
        fontParts.fontRawString = parts[0];
        fontParts.fontStyle = parts[1] || 'normal';
        fontParts.fontVariant = parts[2] || 'normal';
        fontParts.fontWeight = parts[3] || 'normal';
        fontParts.fontSize = parts[4];
        fontParts.fontLineHeight = parts[5];
        fontParts.fontFamily = parts[6];

        FMDrawPropertyMenu.FontObject = fontParts;
    }
};

//==============================================================================
// This function will parse the alignment object and return the text block
// location.
//==============================================================================
FMDrawPropertyMenu.GetTextBlockSetting = function( alignment )
{
    if ( alignment == null )
    {
        return 'center';
    }

    if ( alignment === go.Spot.TopLeft )
    {
    	return 'topLeft';
    }

    if ( alignment === go.Spot.TopCenter )
    {
    	return 'topCenter';
    }

    if (alignment === go.Spot.TopRight)
    {
    	return 'topRight';
    }

    if (alignment === go.Spot.BottomLeft)
    {
    	return 'bottomLeft';
    }

    if (alignment === go.Spot.BottomCenter)
    {
    	return 'bottomCenter';
    }

    if (alignment === go.Spot.BottomRight)
    {
	    return 'bottomRight';
    }

    if (alignment === go.Spot.Center)
    {
    	return 'center';
    }

    if (alignment === go.Spot.LeftCenter)
    {
    	return 'leftCenter';
    }

    if (alignment === go.Spot.RightCenter)
    {
	    return 'rightCenter';
    }

    return 'center';
};

//==============================================================================
// This function will enable / disable a textbox.
//==============================================================================
FMDrawPropertyMenu.EnableDisableTextBox = function( tagId, state )
{
    var textBox = document.getElementById( tagId );

    if ( textBox != null && state != null )
    {
        textBox.disabled = state;
    }
};

//=========================================================
// This function will initialize the properties menu based
// on the canvas object properties.
//=========================================================
FMDrawPropertyMenu.InitCanvasProperties = function()
{
    //TODO: set properties from canvas object
    //FMDrawIndex.SetSpectrumProperty('Fill', canvas.backgroundColor);
};

//=================================================================================
// This function is called by RenderDrawing and sets the shape's pattern if the
// shape has a pattern.  If not it ignores it.
//=================================================================================
FMDrawPropertyMenu.InitializeObjectWithPattern = function( diagram )
{
    diagram.startTransaction( 'InitializeObjectWithPattern' );

    var fillColorDropdown = document.getElementById( 'textbox-propertiesMenu-FILLCOLOR' );
    FMDrawPatternPalette.OperateMode = false;

    if ( fillColorDropdown == null )
    {
        FMDrawPatternPalette.OperateMode = true;
    }

    diagram.nodes.each( function( node )
    {
        var part = node.findObject( 'SHAPE' );

        if ( part == null )
        {
            part = node.findObject( 'BUTTON' );
        }

        if ( part != null )
        {
            if ( node.data.patternFillColor != null )
            {
                FMDrawPatternPalette.patternFillColorRgba = node.data.patternFillColor;
                $( '#textbox-propertiesMenu-FILLCOLOR' ).css( 'background-color', FMDrawPatternPalette.patternFillColorRgba );
            }
            else
            {
                var rgbObj = FMDrawPropertyMenu.HexToRgb( '#000000' );
                FMDrawPatternPalette.patternFillColorRgba = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, 1 );
                $( '#textbox-propertiesMenu-FILLCOLOR' ).css( 'background-color', '#000000' );

                if ( node.data.color != null && typeof ( node.data.color ) !== 'object' )
                {
                    var hex = FMDrawPropertyMenu.Rgb2Hex( node.data.color );
                    rgbObj = FMDrawPropertyMenu.HexToRgb( hex );
                    FMDrawPatternPalette.patternFillColorRgba = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, 1 );

                    $( '#textbox-propertiesMenu-FILLCOLOR' ).css( 'background-color', FMDrawPatternPalette.patternFillColorRgba );
                }
            }

            if ( node.data.transparency != null )
            {
                FMDrawPatternPalette.transparencyStr = node.data.transparency;
                $( '#textbox-propertiesMenu-TRANSPARENCY' ).val( FMDrawPatternPalette.transparencyStr );
            }
            else
            {
                FMDrawPatternPalette.transparencyStr = '0';
                $( '#textbox-propertiesMenu-TRANSPARENCY' ).val( '0' );
            }

            FMDrawPatternPalette.patternStrokeColorHex = '#ffffff';
            $( '#textbox-propertiesMenu-PATTERNCOLOR' ).css( 'background-color', '#ffffff' );

            if ( node.data.patternStrokeColor != null && node.data.patternStrokeColor !== 'undefined' )
            {
                FMDrawPatternPalette.patternStrokeColorHex = node.data.patternStrokeColor;
                $( '#textbox-propertiesMenu-PATTERNCOLOR' ).css( 'background-color', FMDrawPatternPalette.patternStrokeColorHex );
            }

            if (node.data.patternImageName != null && typeof (node.data.patternImageName) != 'undefined' && node.data.patternImageName !== 1)
            {
                var patternNumber = node.data.patternImageName;
                var dynamicPattern = FMDrawPatternPalette.CreatePatternForOperate( parseInt( patternNumber ) );

                // Set the shape object pattern.
                var brush = new go.Brush( go.Brush.Pattern );
                brush.pattern = dynamicPattern;

                FMDrawPropertyMenu.IgnoreEvent = true;

                var dataModel = node.diagram.model;
                dataModel.setDataProperty( node.data, 'color', brush );
                dataModel.setDataProperty( node.data, 'patternImageName', patternNumber );
                FMDrawPropertyMenu.IgnoreEvent = false;
            }
        }
    } );

    FMDrawPatternPalette.OperateMode = false;
    diagram.commitTransaction( 'InitializeObjectWithPattern' );
};

//=====================================================================================
// This function will initializes the manual set property value flags.  False means
// there are no manually entry.
//=====================================================================================
FMDrawPropertyMenu.InitMultiSelectionManualEntryFlags = function()
{
    FMDrawPropertyMenu.manualSetAngleFlag = false;
    FMDrawPropertyMenu.manualSetTopFlag = false;
    FMDrawPropertyMenu.manualSetLeftFlag = false;
    FMDrawPropertyMenu.manualSetHeightFlag = false;
    FMDrawPropertyMenu.manualSetWidthFlag = false;
    FMDrawPropertyMenu.manualSetZOrderFlag = false;
    FMDrawPropertyMenu.manualSetFillColorFlag = false;
    FMDrawPropertyMenu.manualSetPatternColorFlag = false;
    FMDrawPropertyMenu.manualSetPatternFlag = false;
    FMDrawPropertyMenu.manualSetTransparencyFlag = false;
    FMDrawPropertyMenu.manualSetLineTransparencyFlag = false;
    FMDrawPropertyMenu.manualSetLineSizeFlag = false;
    FMDrawPropertyMenu.manualSetLineStyleFlag = false;
    FMDrawPropertyMenu.manualSetLineColorFlag = false;
    FMDrawPropertyMenu.manualSetLineToArrowFlag = false;
    FMDrawPropertyMenu.manualSetLineFromArrowFlag = false;
    FMDrawPropertyMenu.manualSetTextFontFlag = false;
    FMDrawPropertyMenu.manualSetTextSizeFlag = false;
    FMDrawPropertyMenu.manualSetTextStyleFlag = false;
    FMDrawPropertyMenu.manualSetTextUnderlineFlag = false;
    FMDrawPropertyMenu.manualSetTextJustificatiohFlag = false;
    FMDrawPropertyMenu.manualSetTextBlockPositionFlag = false;
    FMDrawPropertyMenu.manualSetTextBlockAlignmentFlag = false;
    FMDrawPropertyMenu.manualSetTextColorFlag = false;
    FMDrawPropertyMenu.manualSetPointIDFlag = false;
};

//=========================================================
// This function will initialize the properties menu based
// on the canvas object properties.
//=========================================================
FMDrawPropertyMenu.SetPropertyMenuEvents = function( currentCanvas )
{
    if ( currentCanvas )
    {
        var objectSelectionChange = function( evt )
        {
            var selectedList = evt.diagram.selection.toArray();

            if ( selectedList.length === 0 )
            {
                // Collaspe the Pattern Palette in order for the color redraw.
                FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' );

                FMDrawPropertyMenu.ClearPropertyWindow( );
                FMDrawPropertyMenu.PropertyActiveObject = null;
                FMDrawIndex.UpdateFlipButtons();
                FMDrawPropertyMenu.InitMultiSelectionManualEntryFlags();

            	// Reset the section sub filters for the bar, tag, and button controls
				// to show.
                FMDrawPropertyMenu.InitializeSectionSubFilters();
            }
            else
            {
                var selectedObject = selectedList[0];

                if ( selectedObject.diagram == null )
                {
                    return;
                }

                if ( selectedObject.isSelected )
                {
                    FMDrawPropertyMenu.InitiatizePropertiesMenu();

                    // Collaspe the Pattern Palette in order for the color redraw.
                    FMDrawPropertyMenu.CollapsePatternPalette( 'td-fillPatternPalette-propertiesMenu-FILLPATTERNPALETTE' );

                    FMDrawIndex.UpdateFlipButtons();
                }
            }
        };
        var objectModified = function( e )
        {
            
            if ( e.isTransactionFinished || e.propertyName === 'size' || e.propertyName === 'pos' )
            {
                FMDrawPropertyMenu.InitiatizePropertiesMenu();
                FMDrawIndex.UpdateFlipButtons();
            }
        };
        currentCanvas.addDiagramListener( 'ChangedSelection', objectSelectionChange );
        currentCanvas.model.addChangedListener( objectModified );
    }
};

FMDrawPropertyMenu.InvokeButtonActionConfiguration = function( actionId )
{
    if ( !actionId
        || ( typeof actionId !== 'string' )
        || !FMDrawPropertyMenu.PropertyActiveObject
        || !FMDrawPropertyMenu.PropertyActiveObject.data )
    {
        return;
    }

    var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();

    if ( !diagram )
    {
        return;
    }

    var obj = FMDrawPropertyMenu.PropertyActiveObject;
    var data = obj.data;

    //If the object does not have a PointGUID or PointTemplateTagSelectionIndicator, the properly
    //populate if attempting to add one for a Point Detail drawing.
    var pointGuid = ( data.PointGUID ) ? data.PointGuid : FMDrawIndex.GetDiagramModelDataValue( 'PointTemplateGuid' );
    var pointTemplateTagSelectionIndicator =
    ( typeof data.PointTemplateTagSelectionIndicator != 'undefined' ) ? data.PointTemplateTagSelectionIndicator :
        FMDrawIndex.GetDiagramModelDataValue('PanelType') === 'Detail';

    var setPointTemplateTransName = 'SetPointTemplateMetaData';
    if (FMDrawIndex.GetDiagramModelDataValue('PanelType') === 'Detail' &&
            typeof data.PointTemplateTagSelectionIndicator === 'undefined')
    {
        obj.diagram.startTransaction(setPointTemplateTransName);
        obj.diagram.model.setDataProperty(data, 'PointTemplateTagSelectionIndicator', pointTemplateTagSelectionIndicator);
        obj.diagram.model.setDataProperty(data, 'PointGUID', pointGuid);
        obj.diagram.commitTransaction(setPointTemplateTransName);
    }

    var actionType = $( '#' + actionId ).val();
    var validActionType = true;

    switch (actionType)
    {
    	case ButtonActionTypeCommand:
    		FMDrawPropertyMenu.onlyAssociateTagDataToButton = true;
    		FMDrawIndex.OpenTagDialogForSwitch(!FMDrawPropertyMenu.MultiSelectionFlag, false, FMDrawIndex.UpdateTagSuccess, data.TagPointID, data.PointGUID, data.TagTagID, data.TagGUID, data.TagPointValueType, data.TagPropertyID, data.PointTemplateTagSelectionIndicator);

    		//FMdrawindex.OpenTagDialog(false, FMdrawindex.PointTagCallSuccess);
    		break;
    	case ButtonActionTypePointTrend:
    		FMDrawPropertyMenu.onlyAssociateTagDataToButton = true;
    		FMDrawIndex.OpenTagDialogForSwitch(false, false, FMDrawIndex.UpdatePointTrendButtonSuccess, data.TagPointID, data.PointGUID, data.TagTagID, data.TagGUID, data.TagPointValueType, data.TagPropertyID, data.PointTemplateTagSelectionIndicator, true);
    		break;
    	case ButtonActionTypeGraphic:
    		FMDrawPropertyMenu.onlyAssociateDrawingIdToButton = true;
    		FMDrawIndex.OpenDrawing( null, true);
    		break;
    	case ButtonActionTypeDetail:
    		FMDrawPropertyMenu.onlyAssociateTagDataToButton = true;
    		FMDrawIndex.OpenTagDialogForSwitch(false,
														false, FMDrawIndex.UpdatePointTrendButtonSuccess,
														data.TagPointID,
														data.PointGUID,
														data.TagTagID,
														data.TagGUID,
														data.TagPointValueType,
														data.TagPropertyID,
														data.PointTemplateTagSelectionIndicator,
														true);
    		break;
      case ButtonActionTypePointHistory:
        FMDrawPropertyMenu.onlyAssociateTagDataToButton = true;
        FMDrawIndex.OpenTagDialogForSwitch(false, false, FMDrawIndex.UpdatePointHistoryButtonSuccess, data.TagPointID, data.PointGUID, data.TagTagID, data.TagGUID, data.TagPointValueType, data.TagPropertyID, data.PointTemplateTagSelectionIndicator, true);
        break;
    	default:
    		validActionType = false;
    		break;
    }
    FMDrawPropertyMenu.currentButtonActionAssociation = (validActionType) ? actionType : null;
};

 FMDrawPropertyMenu.SetButtonActionTypeConfiguration = function (obj, buttonActionObjectGuid, buttonActionObjectId, clearButtonAction, performTransaction)
{
    if ( !obj || !obj.diagram || !obj.data )
    {
        return;
    }

    if ( performTransaction )
    {
        obj.diagram.startTransaction( 'UpdateDrawingButtonConfig' );
    }

    if ( FMDrawPropertyMenu.currentButtonActionAssociation )
    {
    	obj.diagram.model.setDataProperty(obj.data, 'buttonActionType', FMDrawPropertyMenu.currentButtonActionAssociation);

    	if (FMDrawPropertyMenu.currentButtonActionAssociation !== ButtonActionTypeCommand
			&& FMDrawPropertyMenu.currentButtonActionAssociation !== ButtonActionTypePointTrend
      && FMDrawPropertyMenu.currentButtonActionAssociation !== ButtonActionTypeDetail
      && FMDrawPropertyMenu.currentButtonActionAssociation !== ButtonActionTypePointHistory)
        {
            FMDrawPropertyMenu.ClearButtonTagData( obj, false );
        }
    }

    obj.diagram.model.setDataProperty( obj.data, 'buttonActionObjectGuid', buttonActionObjectGuid );
    obj.diagram.model.setDataProperty(obj.data, 'buttonActionObjectId', buttonActionObjectId);

    if ( performTransaction )
    {
        obj.diagram.commitTransaction( 'UpdateDrawingButtonConfig' );
    }

    if ( clearButtonAction )
    {
        FMDrawPropertyMenu.currentButtonActionAssociation = null;
    }
};

FMDrawPropertyMenu.ClearButtonTagData = function( obj, performTransaction )
{
    if ( !obj || !obj.data || !obj.diagram )
    {
        return;
    }

    if ( performTransaction )
    {
        obj.diagram.startTransaction( 'ClearButtonTagData' );
    }

    obj.diagram.model.setDataProperty( obj.data, 'TagGUID', undefined );
    obj.diagram.model.setDataProperty( obj.data, 'PointGUID', undefined );
    obj.diagram.model.setDataProperty( obj.data, 'TagPointID', undefined );
    obj.diagram.model.setDataProperty( obj.data, 'TagTagID', undefined );
    obj.diagram.model.setDataProperty( obj.data, 'TagPointIDAndTagID', undefined);
    obj.diagram.model.setDataProperty( obj.data, 'PointTemplateTagSelectionIndicator', undefined);

    if ( performTransaction )
    {
        obj.diagram.commitTransaction( 'ClearButtonTagData' );
    }
};

//=============================================================================================
// This function will hide all the control section fields with the exception of the ones in the
// list.
//=============================================================================================
FMDrawPropertyMenu.ShowIndividualControlSectionFilter = function( fieldList )
{
	var sectionId = "Section-Controls";

	var sectionIndex = -1;
	var sectionState = "";
	var nextFilter;
	var fieldName;

	// Find the current state for the section selected.  After setting the current state, toggle the persisted state.
	for ( var nextSectionIndex = 0; nextSectionIndex < FMDrawPropertyMenu.SectionExpandCollapseStateList.length; nextSectionIndex++ )
	{
		if ( FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionName === sectionId )
		{
			sectionIndex = nextSectionIndex;
			sectionState = FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionState;
			break;
		}
	}

	if ( sectionIndex !== -1 )
	{
		for ( nextFilter = 0; nextFilter < FMDrawPropertyMenu.SectionSubStateFilterList.length; nextFilter++ )
		{
			FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
			fieldName = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].FieldName;

			for ( var nextItem = 0; nextItem < fieldList.length; nextItem++ )
			{
				if ( fieldName === fieldList[nextItem] )
				{
					FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "show";
				}
			}
		}
	}

// If the control section is in a state of expand, show only the selected fields.
	if (sectionIndex !== -1 && sectionState === "expand")
	{
		for (nextFilter = 0; nextFilter < FMDrawPropertyMenu.SectionSubStateFilterList.length; nextFilter++)
		{
			fieldName = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].FieldName;
			var displayType = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display;

			if ( displayType === "show" )
			{
				$( fieldName ).show();
			}
			else
			{
				$(fieldName).hide();
			}
		}
	}
}

//================================================================================
// This function will set the control section sub filter state to show all
// the control section fields if the control section state is set to expand.
// It is only called by the SetPropertyMenuEvents function which determines
// if there are items selected.
//================================================================================
FMDrawPropertyMenu.ShowAllControlSectionFilter = function()
{
	var sectionId = "Section-Controls";

	var sectionIndex = -1;
	var sectionState = "";
	var nextFilter;

	// Find the current state for the section selected.  After setting the current state, toggle the persisted state.
	for (var nextSectionIndex = 0; nextSectionIndex < FMDrawPropertyMenu.SectionExpandCollapseStateList.length; nextSectionIndex++)
	{
		if (FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionName === sectionId)
		{
			sectionIndex = nextSectionIndex;
			sectionState = FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionState;
			break;
		}
	}

	// If the control section is in a state of expand, then determine the control type (bar, tag, or button)
	// was selected. Then display the associated fields.
	if (sectionIndex !== -1 && sectionState === "expand")
	{
		for (nextFilter = 0; nextFilter < FMDrawPropertyMenu.SectionSubStateFilterList.length; nextFilter++)
		{
			var fieldName = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].FieldName;
			var displayType = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display;
			
			if ( displayType === "show" )
			{
				$( fieldName ).show();
			}
			else
			{
				$(fieldName).hide();
			}
		}
	}
}

//================================================================================
// This function will set the control section sub filter state to hide with the exception
// of the animation edit push button
//================================================================================
FMDrawPropertyMenu.HideAllControlSectionFilter = function () {
    var sectionId = "Section-Controls";

    var sectionIndex = -1;
    var sectionState = "";
    var nextFilter;

    // Find the current state for the section selected.  After setting the current state, toggle the persisted state.
    for (var nextSectionIndex = 0; nextSectionIndex < FMDrawPropertyMenu.SectionExpandCollapseStateList.length; nextSectionIndex++) {
        if (FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionName === sectionId) {
            sectionIndex = nextSectionIndex;
            sectionState = FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionState;
            break;
        }

    else 
		if (FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionState == "expand")
			{
			if (FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionName == "Section-MainProperties")
				{
					FMDrawPropertyMenu.ShowMainSectionCommonProperties();
				}
			 }
		}
    

    // If the control section is in a state of expand, then determine the control type (bar, tag, or button)
    // was selected. Then display the associated fields.

    if (sectionIndex !== -1) {// && sectionState === "expand") {
        for (nextFilter = 0; nextFilter < FMDrawPropertyMenu.SectionSubStateFilterList.length; nextFilter++) {
            var fieldName = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].FieldName;
            var displayType = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display;

            if (fieldName === "#tr-propertiesMenu-ANIMATIONBUTTON") {
                    $(fieldName).show();
                    FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "show";
                    FMDrawPropertyMenu.SetAnimationID();
             }
            else {
                 $(fieldName).hide();
                 FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
             }

        }
    }
}

//====================================================================================
// This function will filter the Control Section based on the drawing object (bar,
// button, or tag) that was selected.
//====================================================================================
FMDrawPropertyMenu.ControlSectionFilter = function( controlType, desiredState, relatedResponse )
{
	var sectionId = "Section-Controls";

	var sectionIndex = -1;
	var sectionState = "";
	var nextFilter;
	var fieldName;

	// Find the current state for the section selected.  After setting the current state, toggle the persisted state.
	for (var nextSectionIndex = 0; nextSectionIndex < FMDrawPropertyMenu.SectionExpandCollapseStateList.length; nextSectionIndex++)
	{
		if (FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionName === sectionId)
		{
			sectionIndex = nextSectionIndex;
			sectionState = FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionState;
			break;
		}
	}

	// If the control section is in a state of expand, then determine the control type (bar, tag, or button)
    // was selected. Then display the associated fields.
	if ( sectionIndex !== -1 && sectionState === "expand" )
	{
		for ( nextFilter = 0; nextFilter < FMDrawPropertyMenu.SectionSubStateFilterList.length; nextFilter++ )
		{
		    fieldName = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].FieldName;
            
		    switch(relatedResponse)
		    {
		        case SelectedObjectsContainsAPointTemplateTag:
		        case SelectedObjectsAreNotAllTags:
		        case SelectedObjectsInvalidInformation:
		            $(fieldName).hide();
		            FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
		            continue;
		    }
		    

			if ( controlType === FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].ControlName )
			{
				if ( desiredState === "show" )
				{
					if ( fieldName === "#tr-propertiesMenu-TAGFIELD" && controlType === "tag" )
					{
						var alarmStatusOption = $("#dropdown-propertiesMenu-TAGFIELD option[value='" + FMTAGFIELDSELECTION.ALARMSTATUS + "']");
						if ( alarmStatusOption )
						{
							alarmStatusOption.remove();
						}
						if ( FMDrawPropertyMenu.PropertyActiveObject.data.TagAlarmAnunciationHasAlarm )
						{
							$("#dropdown-propertiesMenu-TAGFIELD").append("<option value='" + FMTAGFIELDSELECTION.ALARMSTATUS + "'>ALARM STATUS</option>");
						}
					}
					$( fieldName ).show();
					FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "show";
				}
				else
				{
					$( fieldName ).hide();
					FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
				}

				// Exclude Point ID.  Only display if multi-select.
				if (fieldName === "#tr-propertiesMenu-POINTID")
				{
					$(fieldName).hide();
					FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
				}

				if (fieldName === "#tr-propertiesMenu-TAGALARMANNUNCIATION")
				{
					if (FMDrawPropertyMenu.PropertyActiveObject.data.TagPointValueType !== 0
						|| FMDrawPropertyMenu.PropertyActiveObject.data.TagAlarmAnunciationHasAlarm === false)
					{
						$(fieldName).hide();
						FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
					}
				}
			}
			else
			{
				// Point and Tag ID appears in both the Tag and Bar properties.  This is to ensure it is displayed
			    // since it will be turned to hidden in the case where bar or tag is the control name.
			    if (fieldName === "#tr-propertiesMenu-POINTANDTAGID" && (controlType === "tag" || controlType === "bar"))
				{
					$( fieldName ).show();
					FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "show";
				}
			    else if (fieldName === "#tr-propertiesMenu-ANIMATIONBUTTON" && (controlType === "tag" || controlType === "bar" || controlType === "button")) {
			        $(fieldName).show();
			        FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "show";
			        FMDrawPropertyMenu.SetAnimationID();
			    }
			    else
				{
					$( fieldName ).hide();
					FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
			    }

				if (fieldName === "#tr-propertiesMenu-TAGALARMANNUNCIATION")
			    {
					if (FMDrawPropertyMenu.PropertyActiveObject.data.TagPointValueType !== 0
						|| FMDrawPropertyMenu.PropertyActiveObject.data.TagAlarmAnunciationHasAlarm === false)
			    	{
			    		$(fieldName).hide();
			    		FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
			    	}
				}
			}
		}
	}
	else
	{

		// Set all the section sub filters state to filter on the selected control but do not
		// display the field since the section is in collapse state.
		for ( nextFilter = 0; nextFilter < FMDrawPropertyMenu.SectionSubStateFilterList.length; nextFilter++ )
		{
		    fieldName = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].FieldName;

		    switch (relatedResponse) {
		        case SelectedObjectsContainsAPointTemplateTag:
		        case SelectedObjectsAreNotAllTags:
		        case SelectedObjectsInvalidInformation:
		            $(fieldName).hide();
		            FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
		            continue;
		    }

			if (controlType === FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].ControlName)
			{
				$(fieldName).hide();

				if (desiredState === "show")
				{
					FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "show";
				}
				else
				{
					FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
				}
			}
			else
			{
				$(fieldName).hide();

				// Point and Tag ID appears in both the Tag and Bar properties.  This is to ensure it is displayed
				// since it will be turned to hidden in the case where bar or tag is the control name.
				if (fieldName === "#tr-propertiesMenu-POINTANDTAGID" && (controlType === "tag" || controlType === "bar"))
				{
					FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "show";
				}
				else
				{
					FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display = "hide";
				}
			}
		}
	}
}

//=====================================================================================
// This method will force the a section to either expand or collapse.  This is mainly
// used for the control section.
//=====================================================================================
FMDrawPropertyMenu.SectionExpandCollapseForce = function( sectionId, forceState )
{
	if ( typeof ( sectionId ) === "undefined" || sectionId === "" || typeof ( forceState ) === "undefined" || forceState === "" )
	{
		return;
	}

	var sectionIndex = -1;
	var sectionState = "";

	// Find the current state for the section selected.  After setting the current state, toggle the persisted state.
	for ( var nextSectionIndex = 0; nextSectionIndex < FMDrawPropertyMenu.SectionExpandCollapseStateList.length; nextSectionIndex++ )
	{
		if ( FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionName === sectionId )
		{
			sectionIndex = nextSectionIndex;

			// Get the image icon to toggle it between an Right Arrow and a Down Arrow.
			var imagename = "propertryMenu-SectionCollapseImage-" + sectionId;
			var imageElement = document.getElementById( imagename );

			if ( forceState === "collapse" )
			{
				FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionState = "collapse";
				imageElement.src = FMDrawPropertyMenu.GetImagesPath() + "/PropertyMenuSection-Arrow-right.png";
			}
			else
			{
				FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionState = "expand";
				imageElement.src = FMDrawPropertyMenu.GetImagesPath() + "/PropertyMenuSection-Arrow-down.png";
			}
			break;
		}
	}


	// Find each section element to expand or collapse.
	$("div").each(function ()
	{
		var dividerSectionNameAttr = $(this).attr("dividersectionName");

		if (typeof (dividerSectionNameAttr) != "undefined")
		{
			if (dividerSectionNameAttr === sectionId)
			{
				var ignore = false;
				var divId = $(this).attr("id");

				// Determine if the div ID is a section divider. If it is, then
				// we want to ignore it.
				if (typeof (divId) != "undefined")
				{
					var parts = divId.split("-");
					if (parts != null && parts.length > 2)
					{
						if (parts[0] === "tr" && parts[1] === "propertiesMenu" && parts[2].indexOf("DIVIDER") > 0)
						{
							ignore = true;
						}
					}
				}

				// If the element is a section divider, then ignore.
				if (ignore === false)
				{
					if (sectionIndex !== -1)
					{
						if (forceState === "collapse")
						{
							$(this).hide();
						}
						else
						{
							if (sectionId !== "Section-Controls")
							{
								$(this).show();
							}
						}
					}
				}
			}
		}
	});

	// Set the control section sub filter to display or not display the fields in the control section
	// based on the display state.
	if (sectionId === "Section-Controls")
	{
		var fieldName;
		var nextFilter;

		if ( forceState === "collapse" )
		{
			// Set all the section sub filters state to hide and the controls.
			for (nextFilter = 0; nextFilter < FMDrawPropertyMenu.SectionSubStateFilterList.length; nextFilter++)
			{
				fieldName = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].FieldName;
				$(fieldName).hide();
			}
		}
		else
		{
			// Set all the section sub filters state to hide and the controls.
			for (nextFilter = 0; nextFilter < FMDrawPropertyMenu.SectionSubStateFilterList.length; nextFilter++)
			{
				fieldName = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].FieldName;
				var displayState = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display;

				if (displayState === "show")
				{
					$(fieldName).show();
				}
				else
				{
					$(fieldName).hide();
				}
			}
		}
	}
};

//=======================================================================================
// This function will handle the section expand/collapse event.
//=======================================================================================
FMDrawPropertyMenu.SectionExpandCollapseEvent = function( sectionId )
{
	if ( typeof ( sectionId ) === "undefined" || sectionId === "" )
	{
		return;
	}

	var sectionIndex = -1;
	var sectionState = "";

	// Find the current state for the section selected.  After setting the current state, toggle the persisted state.
	for (var nextSectionIndex = 0; nextSectionIndex < FMDrawPropertyMenu.SectionExpandCollapseStateList.length; nextSectionIndex++)
	{
		if (FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionName === sectionId)
		{
			sectionIndex = nextSectionIndex;
			sectionState = FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionState;

			// Get the image icon to toggle it between an Right Arrow and a Down Arrow.
			var imagename = "propertryMenu-SectionCollapseImage-" + sectionId;
			var imageElement = document.getElementById(imagename);

			if ( sectionState === "expand" )
			{
				FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionState = "collapse";
				imageElement.src = FMDrawPropertyMenu.GetImagesPath() + "/PropertyMenuSection-Arrow-right.png";
			}
			else
			{
				FMDrawPropertyMenu.SectionExpandCollapseStateList[nextSectionIndex].SectionState = "expand";
				imageElement.src = FMDrawPropertyMenu.GetImagesPath() + "/PropertyMenuSection-Arrow-down.png";
			}
			break;
		}
	}

	// Find each section element to expand or collapse.
	$("div").each(function()
	{
		var dividerSectionNameAttr = $(this).attr("dividersectionName");

		if (typeof (dividerSectionNameAttr) != "undefined")
		{
			if (dividerSectionNameAttr === sectionId)
			{
				var ignore = false;
				var divId = $(this).attr("id");

				// Determine if the div ID is a section divider. If it is, then
				// we want to ignore it.
				if ( typeof ( divId ) != "undefined" )
				{
					var parts = divId.split("-");
					if ( parts != null && parts.length > 2 )
					{
						if (parts[0] === "tr" && parts[1] === "propertiesMenu" && parts[2].indexOf("DIVIDER") > 0)
						{
							ignore = true;
						}
					}
				}

				// If the element is a section divider, then ignore.
				if (ignore === false)
				{
					if (sectionIndex !== -1)
					{
						if (sectionState === "expand")
						{
							$( this ).hide();
						}
						else
						{
							if ( sectionId !== "Section-Controls" )
							{
								$(this).show();
							}
						}
					}
				}
			}
		}
	});

	// Set the control section sub filter to display or not display the fields in the control section
	// based on the display state.
	if (sectionId === "Section-Controls" && sectionState === "collapse")
	{
		// Set all the section sub filters state to hide and the controls.
		for (var nextFilter = 0; nextFilter < FMDrawPropertyMenu.SectionSubStateFilterList.length; nextFilter++)
		{
			var fieldName = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].FieldName;
			var displayState = FMDrawPropertyMenu.SectionSubStateFilterList[nextFilter].Display;

			if ( displayState === "show" )
			{
				$( fieldName ).show();
			}
			else
			{
				$(fieldName).hide();
			}
		}
	}
};

//=======================================================================================
// This function will handle the section expand/collapse event.
//=======================================================================================
FMDrawPropertyMenu.InitSectionExpandCollapseState = function ()
{
	// Initialize the section state array.
	FMDrawPropertyMenu.SectionExpandCollapseStateList = [];

	// Find all the sections and build a state list.  Remember that all the sections
	// are expanded.
	$( "div" ).each( function()
	{
		var dividerSectionNameAttr = $( this ).attr( "dividersectionName" );

		if ( typeof ( dividerSectionNameAttr ) != "undefined" )
		{
			var isSectionNameUnique = true;

			if ( FMDrawPropertyMenu.SectionExpandCollapseStateList.length > 0 )
			{
				for ( var nextIndex = 0; nextIndex < FMDrawPropertyMenu.SectionExpandCollapseStateList.length; nextIndex++ )
				{
					if ( FMDrawPropertyMenu.SectionExpandCollapseStateList[nextIndex].SectionName === dividerSectionNameAttr )
					{
						isSectionNameUnique = false;
					}
				}
			}

			if (isSectionNameUnique)
			{
				var sectionStateObj			 = new Object();
				sectionStateObj.SectionName  = dividerSectionNameAttr;
				sectionStateObj.SectionState = "expand";

				FMDrawPropertyMenu.SectionExpandCollapseStateList.push(sectionStateObj);

				// Initialize the section sub filters for the Bar, Tag, and Button
				// controls.  This is only for the Controls Section.
				if ( dividerSectionNameAttr === "Section-Controls" )
				{
					FMDrawPropertyMenu.InitializeSectionSubFilters();
				}
				FMDrawPropertyMenu.SectionExpandCollapseForce( dividerSectionNameAttr, "collapse" );
			}
		}
	} );
};

//====================================================================================
// This function will invoke the animation button action which opens the animation
// dialog.
//====================================================================================
FMDrawPropertyMenu.InvokeAnimationButtonAction = function()
{
	FMDrawAnimation.OpenAnimationDialog(true);
};

//====================================================================================
// This function will initialize the section sub filter list that contains the 
// controls to be shown or filtered out.
//====================================================================================
FMDrawPropertyMenu.InitializeSectionSubFilters = function()
{
    FMDrawPropertyMenu.SectionSubStateFilterList = [];

	//=================================
	// Filters tag fields
	//=================================
	// Point and value ID
	var filterObj = new Object();
	filterObj.ControlName = "tag";
	filterObj.FieldName = "#tr-propertiesMenu-POINTANDTAGID";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Point ID
	filterObj = new Object();
	filterObj.ControlName = "tag";
	filterObj.FieldName = "#tr-propertiesMenu-POINTID";
	filterObj.Display = "hide";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Value Width
	filterObj = new Object();
	filterObj.ControlName = "tag";
	filterObj.FieldName = "#tr-propertiesMenu-TAGWIDTH";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Value Units
	filterObj = new Object();
	filterObj.ControlName = "tag";
	filterObj.FieldName = "#tr-propertiesMenu-TAGUNITS";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Value Percision
	filterObj = new Object();
	filterObj.ControlName = "tag";
	filterObj.FieldName = "#tr-propertiesMenu-TAGPRECISION";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Value Field
	filterObj = new Object();
	filterObj.ControlName = "tag";
	filterObj.FieldName = "#tr-propertiesMenu-TAGFIELD";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Show Alarm Annunciation
	filterObj = new Object();
	filterObj.ControlName = "tag";
	filterObj.FieldName = "#tr-propertiesMenu-TAGALARMANNUNCIATION";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Show Quality
	filterObj = new Object();
	filterObj.ControlName = "tag";
	filterObj.FieldName = "#tr-propertiesMenu-TAGSHOWSTATUS";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Show W&M
	filterObj = new Object();
	filterObj.ControlName = "tag";
	filterObj.FieldName = "#tr-propertiesMenu-TAGSHOWWEIGHTSANDMEASURES";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	//=================================
	// Filters Bar fields
	//=================================
	// Bar Type
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-BARTYPE";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Use Product Color (bar only)
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-USEPRODUCTCOLOR";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Use alarm level (bar only)
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-USEALARMLEVEL";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Demo Percent
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-DEMOVALUEPERCENT";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Point and Value ID
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-POINTANDTAGID";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Use Value Limits
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-USETAGLIMITS";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Minimum Value
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-MINVALUE";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Maximum Value
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-MAXVALUE";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Background Fill Color
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-BGFILLCOLOR";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Background Fill Color Spectrum
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-BGFILLCOLORSPECTRUM";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	//Background Transparency
	filterObj = new Object();
	filterObj.ControlName = "bar";
	filterObj.FieldName = "#tr-propertiesMenu-BGTRANSPARENCY";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	//=================================
	// Filters Button fields
	//=================================
	// Button Action Type
	filterObj = new Object();
	filterObj.ControlName = "button";
	filterObj.FieldName = "#tr-propertiesMenu-BUTTONACTIONTYPE";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Button Action Target
	filterObj = new Object();
	filterObj.ControlName = "button";
	filterObj.FieldName = "#tr-propertiesMenu-BUTTONACTIONTARGET";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

	// Button Action Animation
	filterObj = new Object();
	filterObj.ControlName = "tag";
	filterObj.FieldName = "#tr-propertiesMenu-ANIMATIONBUTTON";
	filterObj.Display = "show";
	FMDrawPropertyMenu.SectionSubStateFilterList.push(filterObj);

}

//================================================================
// This function will get the URL path for the "images".
//================================================================
FMDrawPropertyMenu.GetImagesPath = function ()
{
	if (FMDrawPropertyMenu.imageRootPath == null || FMDrawPropertyMenu.imageRootPath === "")
	{
        FMDrawPropertyMenu.imageRootPath = window.applicationRootName + "/Areas/images";
	}

	var protocol = window.location.protocol;
	var host = window.location.host;
	var sourcePath = protocol + "//" + host + "/" + FMDrawPropertyMenu.imageRootPath;

	return sourcePath;
};
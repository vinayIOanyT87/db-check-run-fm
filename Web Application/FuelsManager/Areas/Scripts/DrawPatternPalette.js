if ( typeof ( FMDrawPatternPalette ) === "undefined" )
	FMDrawPatternPalette = {};

FMDrawPatternPalette = FMDrawPatternPalette || {};

if (!window.applicationRootName) {
    let p = window.location.pathname.indexOf('/', 1);
    let p0 = window.location.pathname.indexOf('/(S(', 1);
    let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
    debugger;
    window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

// This is used when the user changes the transparency textbox. Since the the transparency
// value has not been committed it will not be in the textbox, so the changed value is 
// needed.
FMDrawPatternPalette.newTransparencyValue = null;
FMDrawPatternPalette.OperateMode = false;
FMDrawPatternPalette.patternFillColorRgba = null;
FMDrawPatternPalette.transparencyStr = null;
FMDrawPatternPalette.patternStrokeColorHex = null;

FMDrawPatternPalette.canvasWidth = 600;
FMDrawPatternPalette.canvasHeight = 600;

FMDrawPatternPalette.lineStyleCanvasWidth = 80;
FMDrawPatternPalette.lineStyleCanvasHeight = 15;

FMDrawPatternPalette.LineStylePatterns = [];
FMDrawPatternPalette.LineStylePatterns.push( [0, 0] );
FMDrawPatternPalette.LineStylePatterns.push( [5, 10] );
FMDrawPatternPalette.LineStylePatterns.push( [10, 5] );
FMDrawPatternPalette.LineStylePatterns.push( [6, 2] );
FMDrawPatternPalette.LineStylePatterns.push( [10, 2] );

FMDrawPatternPalette.ToArrowNames = [];
FMDrawPatternPalette.ToArrowNames.push( 'None' );
FMDrawPatternPalette.ToArrowNames.push( 'Standard' );
FMDrawPatternPalette.ToArrowNames.push( 'Triangle' );
FMDrawPatternPalette.ToArrowNames.push( 'OpenTriangle' );
FMDrawPatternPalette.ToArrowNames.push( 'SidewaysV' );

FMDrawPatternPalette.FromArrowNames = [];
FMDrawPatternPalette.FromArrowNames.push( 'None' );
FMDrawPatternPalette.FromArrowNames.push( 'Backward' );
FMDrawPatternPalette.FromArrowNames.push( 'BackwardTriangle' );
FMDrawPatternPalette.FromArrowNames.push( 'BackwardOpenTriangle' );
FMDrawPatternPalette.FromArrowNames.push( 'BackwardV' );

//=====================================================================================================
// This function manages which pattern to create.
//=====================================================================================================
FMDrawPatternPalette.CreatePattern = function( canvasPaletteId, patternNumber )
{
    switch ( patternNumber )
    {
        case 1:
            FMDrawPatternPalette.MakePattern1( canvasPaletteId );
            break;
        case 2:
            FMDrawPatternPalette.MakePattern2( canvasPaletteId );
            break;
        case 3:
            FMDrawPatternPalette.MakePattern3( canvasPaletteId );
            break;
        case 4:
            FMDrawPatternPalette.MakePattern4( canvasPaletteId );
            break;
        case 5:
            FMDrawPatternPalette.MakePattern5( canvasPaletteId );
            break;
        case 6:
            FMDrawPatternPalette.MakePattern6( canvasPaletteId );
            break;
        case 7:
            FMDrawPatternPalette.MakePattern7( canvasPaletteId );
            break;
    }
};

//=====================================================================================================
// This function will create a dynamic canvas based on the pattern number.  This function is 
// called by operate.
//=====================================================================================================
FMDrawPatternPalette.CreatePatternForOperate = function( patternNumber, fillColor, strokeColor )
{
    switch ( patternNumber )
    {
        case 2:
            return FMDrawPatternPalette.MakePattern2( null, fillColor, strokeColor );
        case 3:
            return FMDrawPatternPalette.MakePattern3( null, fillColor, strokeColor );
        case 4:
            return FMDrawPatternPalette.MakePattern4( null, fillColor, strokeColor );
        case 5:
            return FMDrawPatternPalette.MakePattern5( null, fillColor, strokeColor );
        case 6:
            return FMDrawPatternPalette.MakePattern6( null, fillColor, strokeColor );
        case 7:
            return FMDrawPatternPalette.MakePattern7( null, fillColor, strokeColor );
    }
    return null;
};

//=====================================================================================================
// This function creates a pattern of blank rectangle spaced 14 pixels apart.
//=====================================================================================================
FMDrawPatternPalette.MakePattern1 = function( canvasPaletteId )
{
    var patternCanvas = document.getElementById( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    context.clearRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

    // Must create a rectangle first in order to fill the background.
    context.fillStyle = '#ffffff';
    context.fillRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

    return patternCanvas;
};

//=====================================================================================================
// This function creates a pattern of vertical lines spaced 14 pixels apart.
//=====================================================================================================
FMDrawPatternPalette.MakePattern2 = function( canvasPaletteId, fillColor, strokeColor )
{
    var patternCanvas = FMDrawPatternPalette.GetCanvasPalette( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    context.clearRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

	// Must create a rectangle first in order to fill the background.
	if ( fillColor )
	{
		context.fillStyle = fillColor;
		var notRbg = fillColor.indexOf( "rgb" );

		if (FMDrawPatternPalette.transparencyStr != null && notRbg === -1)
		{
			var transparencyValue = parseInt(FMDrawPatternPalette.transparencyStr);
			var transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat(transparencyValue);
			var parts = FMDrawPropertyMenu.HexToRgb(fillColor);
			var newRgbaStr = FMDrawPropertyMenu.ConvertToRgbaString(parts, transparencyFloat);

			context.fillStyle = newRgbaStr;
		}
	}
	else
	{
		context.fillStyle = FMDrawPatternPalette.GetFillColor();
	}

    context.fillRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

	// Set the line stroke first before drawing.
    if (strokeColor)
		context.strokeStyle = strokeColor;
	else
    context.strokeStyle = FMDrawPatternPalette.GetStrokeColor();
    context.lineWidth = 1;
    context.beginPath();

    for ( var next = 0; next < 100; next++ )
    {
        context.moveTo( next * 14, 0.0 );
        context.lineTo( next * 14, 600 );
        context.stroke();
    }

    context.closePath();

    return patternCanvas;
};

//=====================================================================================================
// This function creates a pattern of horizontal lines spaced 15 pixels apart.
//=====================================================================================================
FMDrawPatternPalette.MakePattern3 = function (canvasPaletteId, fillColor, strokeColor)
{
    var patternCanvas = FMDrawPatternPalette.GetCanvasPalette( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    context.clearRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

    // Must create a rectangle first in order to fill the background.
    if (fillColor)
    {
    	context.fillStyle = fillColor;
    	var notRbg = fillColor.indexOf("rgb");

    	if (FMDrawPatternPalette.transparencyStr != null && notRbg === -1)
    	{
    		var transparencyValue = parseInt(FMDrawPatternPalette.transparencyStr);
    		var transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat(transparencyValue);
    		var parts = FMDrawPropertyMenu.HexToRgb(fillColor);
    		var newRgbaStr = FMDrawPropertyMenu.ConvertToRgbaString(parts, transparencyFloat);

    		context.fillStyle = newRgbaStr;
    	}
    }
    else
    {
    	context.fillStyle = FMDrawPatternPalette.GetFillColor();
    }

    context.fillRect(0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight);

    // Set the line stroke first before drawing.
    if (strokeColor)
    	context.strokeStyle = strokeColor;
    else
    	context.strokeStyle = FMDrawPatternPalette.GetStrokeColor();
    context.lineWidth = 1;
    context.beginPath();

    for ( var next = 0; next < 100; next++ )
    {
        context.moveTo( 0.0, next * 15 );
        context.lineTo( 600, next * 15 );
        context.stroke();
    }

    context.closePath();

    return patternCanvas;
};

//=====================================================================================================
// This function creates a pattern of forward diagonal lines spaced 15 pixels apart.
//=====================================================================================================
FMDrawPatternPalette.MakePattern4 = function (canvasPaletteId, fillColor, strokeColor)
{
    var patternCanvas = FMDrawPatternPalette.GetCanvasPalette( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    context.clearRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

    // Must create a rectangle first in order to fill the background.
    if (fillColor)
    {
    	context.fillStyle = fillColor;
    	var notRbg = fillColor.indexOf("rgb");

    	if (FMDrawPatternPalette.transparencyStr != null && notRbg === -1)
    	{
    		var transparencyValue = parseInt(FMDrawPatternPalette.transparencyStr);
    		var transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat(transparencyValue);
    		var parts = FMDrawPropertyMenu.HexToRgb(fillColor);
    		var newRgbaStr = FMDrawPropertyMenu.ConvertToRgbaString(parts, transparencyFloat);

    		context.fillStyle = newRgbaStr;
    	}
    }
    else
    {
    	context.fillStyle = FMDrawPatternPalette.GetFillColor();
    }

    context.fillRect(0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight);

    // Set the line stroke first before drawing.
    if (strokeColor)
    	context.strokeStyle = strokeColor;
    else
    	context.strokeStyle = FMDrawPatternPalette.GetStrokeColor();
    context.lineWidth = 1;
    context.beginPath();

    var yPoint = 1200;
    var xPoint = 1200;
    var increment = 20;

    for ( var next = 0; next < 60; next++ )
    {
        context.moveTo( 0, yPoint );
        context.lineTo( xPoint, 0 );
        context.stroke();

        yPoint = yPoint - increment;
        xPoint = xPoint - increment;
    }

    context.closePath();

    return patternCanvas;
};

//=====================================================================================================
// This function creates a pattern of backward diagonal lines spaced 15 pixels apart.
//=====================================================================================================
FMDrawPatternPalette.MakePattern5 = function (canvasPaletteId, fillColor, strokeColor)
{
    var patternCanvas = FMDrawPatternPalette.GetCanvasPalette( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    context.clearRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

    // Must create a rectangle first in order to fill the background.
    if (fillColor)
    {
    	context.fillStyle = fillColor;
    	var notRbg = fillColor.indexOf("rgb");

    	if (FMDrawPatternPalette.transparencyStr != null && notRbg === -1)
    	{
    		var transparencyValue = parseInt(FMDrawPatternPalette.transparencyStr);
    		var transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat(transparencyValue);
    		var parts = FMDrawPropertyMenu.HexToRgb(fillColor);
    		var newRgbaStr = FMDrawPropertyMenu.ConvertToRgbaString(parts, transparencyFloat);

    		context.fillStyle = newRgbaStr;
    	}
    }
    else
    {
    	context.fillStyle = FMDrawPatternPalette.GetFillColor();
    }

    context.fillRect(0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight);

    // Set the line stroke first before drawing.
    if (strokeColor)
    	context.strokeStyle = strokeColor;
    else
    	context.strokeStyle = FMDrawPatternPalette.GetStrokeColor();

    context.lineWidth = 1;
    context.beginPath();

    var yPoint = 600;
    var xPoint = 0;
    var increment = 20;

    for ( var next = 0; next < 60; next++ )
    {
        context.moveTo( xPoint, 600 );
        context.lineTo( 0, yPoint );
        context.stroke();

        yPoint = yPoint - increment;
        xPoint = xPoint + increment;
    }

    context.closePath();

    return patternCanvas;
};

//=====================================================================================================
// This function creates a pattern of backward & forward diagonal lines spaced 20 pixels apart.
//=====================================================================================================
FMDrawPatternPalette.MakePattern6 = function (canvasPaletteId, fillColor, strokeColor)
{
    var patternCanvas = FMDrawPatternPalette.GetCanvasPalette( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    context.clearRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

    // Must create a rectangle first in order to fill the background.
    if (fillColor)
    {
    	context.fillStyle = fillColor;
    	var notRbg = fillColor.indexOf("rgb");

    	if (FMDrawPatternPalette.transparencyStr != null && notRbg === -1)
    	{
    		var transparencyValue = parseInt(FMDrawPatternPalette.transparencyStr);
    		var transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat(transparencyValue);
    		var parts = FMDrawPropertyMenu.HexToRgb(fillColor);
    		var newRgbaStr = FMDrawPropertyMenu.ConvertToRgbaString(parts, transparencyFloat);

    		context.fillStyle = newRgbaStr;
    	}
    }
    else
    {
    	context.fillStyle = FMDrawPatternPalette.GetFillColor();
    }

    context.fillRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

    // Set the line stroke first before drawing.
    if (strokeColor)
    	context.strokeStyle = strokeColor;
    else
    	context.strokeStyle = FMDrawPatternPalette.GetStrokeColor();
    context.lineWidth = 1;
    context.beginPath();

    var next;
    var yPoint = 600;
    var xPoint = 0;
    var increment = 20;

    for ( next = 0; next < 60; next++ )
    {
        context.moveTo( xPoint, 600 );
        context.lineTo( 0, yPoint );
        context.stroke();

        yPoint = yPoint - increment;
        xPoint = xPoint + increment;
    }

    yPoint = 1200;
    xPoint = 1200;
    increment = 20;

    for ( next = 0; next < 60; next++ )
    {
        context.moveTo( 0, yPoint );
        context.lineTo( xPoint, 0 );
        context.stroke();

        yPoint = yPoint - increment;
        xPoint = xPoint - increment;
    }
    context.closePath();

    return patternCanvas;
};

//=====================================================================================================
// This function creates a pattern of vertical & horizontal lines spaced 14 pixels apart.
//=====================================================================================================
FMDrawPatternPalette.MakePattern7 = function (canvasPaletteId, fillColor, strokeColor)
{
    var patternCanvas = FMDrawPatternPalette.GetCanvasPalette( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    context.clearRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

    if (fillColor)
    {
    	context.fillStyle = fillColor;
    	var notRbg = fillColor.indexOf("rgb");

    	if (FMDrawPatternPalette.transparencyStr != null && notRbg === -1)
    	{
    		var transparencyValue = parseInt(FMDrawPatternPalette.transparencyStr);
    		var transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat(transparencyValue);
    		var parts = FMDrawPropertyMenu.HexToRgb(fillColor);
    		var newRgbaStr = FMDrawPropertyMenu.ConvertToRgbaString(parts, transparencyFloat);

    		context.fillStyle = newRgbaStr;
    	}
    }
    else
    {
    	context.fillStyle = FMDrawPatternPalette.GetFillColor();
    }

    context.fillRect( 0, 0, FMDrawPatternPalette.canvasWidth, FMDrawPatternPalette.canvasHeight );

    // Set the line stroke first before drawing.
    if (strokeColor)
    	context.strokeStyle = strokeColor;
    else
    	context.strokeStyle = FMDrawPatternPalette.GetStrokeColor();

    context.lineWidth = 1;
    context.beginPath();
    var next;

    for ( next = 0; next < 100; next++ )
    {
        context.moveTo( next * 14, 0.0 );
        context.lineTo( next * 14, 600 );
        context.stroke();
    }

    for ( next = 0; next < 100; next++ )
    {
        context.moveTo( 0.0, next * 14 );
        context.lineTo( 600, next * 14 );
        context.stroke();
    }

    context.closePath();

    return patternCanvas;
};

//=====================================================================================================
// This function manages which line style pattern to create.
//=====================================================================================================
FMDrawPatternPalette.CreateLineStylePattern = function( canvasPaletteId, patternNumber )
{
    switch ( patternNumber )
    {
        case 1:
            return FMDrawPatternPalette.MakeEmptyLineStylePattern( canvasPaletteId );
        default:
            return FMDrawPatternPalette.MakeLineStylePattern( canvasPaletteId, patternNumber );
    }
};

//=====================================================================================================
// This function creates a line style pattern for the selected pattern number.
//=====================================================================================================
FMDrawPatternPalette.MakeLineStylePattern = function( canvasPaletteId, patternNumber )
{
    var patternCanvas = FMDrawPatternPalette.GetCanvasPalette( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    context.clearRect( 0, 0, FMDrawPatternPalette.lineStyleCanvasWidth, FMDrawPatternPalette.lineStyleCanvasHeight );

    // Must create a rectangle first in order to fill the background.
    context.fillStyle = '#ffffff';
    context.fillRect( 0, 0, FMDrawPatternPalette.lineStyleCanvasWidth, FMDrawPatternPalette.lineStyleCanvasHeight );

    // Set the line stroke first before drawing.
    context.lineWidth = 1;
    context.beginPath();
    context.setLineDash( FMDrawPatternPalette.LineStylePatterns[patternNumber - 1] );

    context.moveTo( 0.0, 9 );
    context.lineTo( 80, 9 );
    context.stroke();

    context.closePath();

    return patternCanvas;
};

//=====================================================================================================
// This function creates a line style pattern of blank rectangle.
//=====================================================================================================
FMDrawPatternPalette.MakeEmptyLineStylePattern = function( canvasPaletteId )
{
    var patternCanvas = document.getElementById( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    context.clearRect( 0, 0, FMDrawPatternPalette.lineStyleCanvasWidth, FMDrawPatternPalette.lineStyleCanvasHeight );

    // Must create a rectangle first in order to fill the background.
    context.fillStyle = '#ffffff';
    context.fillRect( 0, 0, FMDrawPatternPalette.lineStyleCanvasWidth, FMDrawPatternPalette.lineStyleCanvasHeight );

    // Set the line stroke first before drawing.
    context.lineWidth = 1;
    context.beginPath();
    context.setLineDash( [80, 1] );

    context.moveTo( 0.0, 9 );
    context.lineTo( 80, 9 );
    context.stroke();

    context.closePath();

    return patternCanvas;
}; //=====================================================================================================
// This function manages which To Arrow pattern to create.
//=====================================================================================================
FMDrawPatternPalette.CreateToArrowPattern = function( canvasPaletteId, patternNumber )
{
    switch ( patternNumber )
    {
        case 1:
            return FMDrawPatternPalette.MakeEmptyArrowPattern( canvasPaletteId );
        default:
            return FMDrawPatternPalette.MakeArrowPattern( canvasPaletteId, FMDrawPatternPalette.ToArrowNames[patternNumber - 1], 'ToArrow' );
    }
};

//=====================================================================================================
// This function manages which From Arrow pattern to create.
//=====================================================================================================
FMDrawPatternPalette.CreateFromArrowPattern = function( canvasPaletteId, patternNumber )
{
    switch ( patternNumber )
    {
        case 1:
            return FMDrawPatternPalette.MakeEmptyArrowPattern( canvasPaletteId );
        default:
            return FMDrawPatternPalette.MakeArrowPattern( canvasPaletteId, FMDrawPatternPalette.FromArrowNames[patternNumber - 1], 'FromArrow' );
    }
};

//=====================================================================================================
// This function creates an arrow pattern of blank rectangle.
//=====================================================================================================
FMDrawPatternPalette.MakeEmptyArrowPattern = function( canvasPaletteId )
{
    var patternCanvas = document.getElementById( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    context.clearRect( 0, 0, FMDrawPatternPalette.lineStyleCanvasWidth, FMDrawPatternPalette.lineStyleCanvasHeight );

    // Must create a rectangle first in order to fill the background.
    context.fillStyle = '#ffffff';
    context.fillRect( 0, 0, FMDrawPatternPalette.lineStyleCanvasWidth, FMDrawPatternPalette.lineStyleCanvasHeight );

    return patternCanvas;
};

//=====================================================================================================
// This function creates an arrow pattern based on the arrow name.
//=====================================================================================================
FMDrawPatternPalette.MakeArrowPattern = function( canvasPaletteId, arrowName, arrowDirection )
{
    var patternCanvas = FMDrawPatternPalette.GetCanvasPalette( canvasPaletteId );

    var context = patternCanvas.getContext( '2d' );
    var arrowImage = new Image();

    arrowImage.onload = function()
    {
        if ( arrowDirection === 'ToArrow' )
        {
            context.drawImage( arrowImage, 0, -2 );
        }
        else
        {
            context.drawImage( arrowImage, 0, -6 );
        }
    };
    switch ( arrowName )
    {
        case 'Standard':
            arrowImage.src = window.applicationRootName + '/fmwebapp/images/LineArrowStandard.png';
            break;
        case 'Backward':
            arrowImage.src = window.applicationRootName + '/fmwebapp/images/LineArrowBackwardStandard.png';
            break;
        case 'Triangle':
            arrowImage.src = window.applicationRootName + '/fmwebapp/images/LineArrowTriangle.png';
            break;
        case 'BackwardTriangle':
            arrowImage.src = window.applicationRootName + '/fmwebapp/images/LineArrowBackwardTriangle.png';
            break;
        case 'OpenTriangle':
            arrowImage.src = window.applicationRootName + '/fmwebapp/images/LineArrowOpenTriangle.png';
            break;
        case 'BackwardOpenTriangle':
            arrowImage.src = window.applicationRootName + '/fmwebapp/images/LineArrowBackwardOpenTriangle.png';
            break;
        case 'SidewaysV':
            arrowImage.src = window.applicationRootName + '/fmwebapp/images/LineArrowSidewaysV.png';
            break;
        case 'BackwardV':
            arrowImage.src = window.applicationRootName + '/fmwebapp/images/LineArrowBackwardSidewaysV.png';
            break;
    }

    return patternCanvas;
};

//=====================================================================================================
// This function will return the fill color RGBA string.
//=====================================================================================================
FMDrawPatternPalette.GetFillColor = function()
{
    var fillColorDropdown = document.getElementById( 'textbox-propertiesMenu-FILLCOLOR' );
    var rgbObj;
    var rgbaStr;

    var transparencyFloat = FMDrawPatternPalette.GetCurrentTransparencyAsFloat();

    // Draw mode
    if ( FMDrawPatternPalette.OperateMode === false )
    {
        if ( fillColorDropdown == null )
        {
            rgbObj = FMDrawPropertyMenu.HexToRgb( '#ffffff' );
            rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );
            return rgbaStr;
        }

        var hex = FMDrawPropertyMenu.Rgb2Hex( $( '#textbox-propertiesMenu-FILLCOLOR' ).css( 'background-color' ) );
        rgbObj = FMDrawPropertyMenu.HexToRgb( hex );
        rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );

        return rgbaStr;
    }

    // Operate mode
    // this could of been stored in rgb or color format so we need to check and parse based on the storage

    var parts;
    var newRgbaStr;
    if (FMDrawPatternPalette.patternFillColorRgba[0] === '#')
    {
        parts = FMDrawPropertyMenu.HexToRgb(FMDrawPatternPalette.patternFillColorRgba);
        newRgbaStr = FMDrawPropertyMenu.ConvertToRgbaString(parts, transparencyFloat);
    }
    else {
        parts = FMDrawPatternPalette.patternFillColorRgba.split( ',' );
        newRgbaStr = parts[0] + ', ' + parts[1] + ', ' + parts[2] + ', ' + transparencyFloat.toString() + ')';
    }

    return newRgbaStr;
};

//=====================================================================================================
// This function will return the RGB string for the pattern color (a.k.a. "stroke").
//=====================================================================================================
FMDrawPatternPalette.GetStrokeColor = function()
{
    var strokeColorDropdown = document.getElementById( 'textbox-propertiesMenu-PATTERNCOLOR' );
    var transparencyFloat = FMDrawPatternPalette.GetCurrentTransparencyAsFloat();

    var rgbObj;
    var rgbaStr;

    // Draw mode
    if ( FMDrawPatternPalette.OperateMode === false )
    {
        if ( strokeColorDropdown == null )
        {
            rgbObj = FMDrawPropertyMenu.HexToRgb( '#000000' );
            rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );
            return rgbaStr;
        }

        var hex = FMDrawPropertyMenu.Rgb2Hex( $( '#textbox-propertiesMenu-PATTERNCOLOR' ).css( 'background-color' ) );
        rgbObj = FMDrawPropertyMenu.HexToRgb( hex );
        rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );

        return rgbaStr;
    }

    // Operate mode
    rgbObj = FMDrawPropertyMenu.HexToRgb( FMDrawPatternPalette.patternStrokeColorHex );
    rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, transparencyFloat );

    return rgbaStr;
};

//====================================================================================================
// This method will return the current transparency value from the textbox as a float.
//====================================================================================================
FMDrawPatternPalette.GetCurrentTransparencyAsFloat = function()
{
    var transparencyTextbox = document.getElementById( 'textbox-propertiesMenu-TRANSPARENCY' );
    var transparencyValue = 0.0;
    var transparencyInt;

    if (transparencyTextbox != null
		&& transparencyTextbox.value != null
		&& transparencyTextbox.value !== ''
		&& transparencyTextbox.value !== ' ')
    {
        transparencyInt = parseInt( transparencyTextbox.value );

        if ( transparencyInt !== 'NaN' )
        {
            transparencyValue = transparencyInt;
        }
    }

    // This is used when the user changes the transparency text box and the value
    // has not been committed within the context.
    if ( FMDrawPatternPalette.newTransparencyValue != null )
    {
        transparencyInt = parseInt( FMDrawPatternPalette.newTransparencyValue );

        if ( transparencyInt !== 'NaN' )
        {
            transparencyValue = transparencyInt;
        }
    }

    // In operate mode, use the transparency string that was set in DrawPropertyMenu.
    if ( FMDrawPatternPalette.OperateMode )
    {
        transparencyValue = parseInt( FMDrawPatternPalette.transparencyStr );
    }

    var transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat( transparencyValue );
    return transparencyFloat;
};

//====================================================================================================
// This method will return the current background transparency value from the textbox as a float.
//====================================================================================================
FMDrawPatternPalette.GetCurrentBackgroundTransparencyAsFloat = function () {
	var transparencyTextbox = document.getElementById('textbox-propertiesMenu-BGTRANSPARENCY');
	var transparencyValue = 0.0;
	var transparencyInt;

	if (transparencyTextbox != null && transparencyTextbox.value != null) {
		transparencyInt = parseInt(transparencyTextbox.value);

		if (transparencyInt !== 'NaN') {
			transparencyValue = transparencyInt;
		}
	}


	var transparencyFloat = FMDrawPropertyMenu.ConvertTransparencyToFloat(transparencyValue);
	return transparencyFloat;
};

//==============================================================================================
// This function will return an existing canvas based on the canvas palette ID (for draw) or
// it will return a new dynamic canvas (for operate).
//==============================================================================================
FMDrawPatternPalette.GetCanvasPalette = function( canvasPaletteId )
{
    var patternCanvas = document.getElementById( canvasPaletteId );

    // This is used for when the shape object is created from operate.
    if ( canvasPaletteId == null )
    {
        patternCanvas = document.createElement( 'canvas' );
        patternCanvas.width = FMDrawPatternPalette.canvasWidth;
        patternCanvas.height = FMDrawPatternPalette.canvasHeight;
    }

    return patternCanvas;
};

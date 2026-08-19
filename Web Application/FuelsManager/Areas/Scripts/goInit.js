var $$ = go.GraphObject.make;

var isDrawBoolean = true;

var loadingPictureFromDoubleClick = false;

go.Diagram.licenseKey = "73f14fe7b00537c702d90776423d6af919a17564ce8149a30a0413f3ec0d6b06329ee02851d3d89380ac1bfc1d7dc4d1dfc03b209248563ce63583db13e085aab42563b44158418ef65327d189f92ba2fd6774edc1b3";

//=======================================================================
// This function will calculate a new margin for the text block that is
// inside of a shape. The reason why is that if the shape has a border
// stroke width that is large it will over up the text if the text is
// position to the left, right, top, or bottom. This function is called
// DrawPropertyMenu.SetPropertyOnObject() when the line sizes changes.
// It is initialized in the text block bindings for shape.
//=======================================================================
function MarginCalculator( calMargin, part )
{
    // Set the default margin to be 5.
    if ( calMargin == null )
    {
        return 5;
    }

    // The calMargin is an object when the drawing is saved and reopened.
    // Therefore, use the part to find the previous setting.
    if ( typeof ( calMargin ) === 'object' )
    {
        calMargin = part.part.data.strokeWidth;
    }

    var newMargin = 5 + calMargin;
    return newMargin;
}

function TransparencyToOpacity( transparency )
{
    var transparencyStr = transparency.replace( '%', '' );
    var t = parseFloat( transparencyStr );
    t = t / 100.00;
    t = 1.0 - t;
    return t;
}

function startPointToLocation(geo, node)
{
	//SRM
    var coord = getGeomertyCoords(geo);
    var a = node.actualBounds;
    var h = a.height;
	var w = a.width;
	var lw = node.part.data.strokeWidth;

	if (h !== 9 * lw)
		h = 9 * lw;

	if (w !== 9 * lw)
		w = 9 * lw;

	if (lw <= 2)
		lw = 2;

	//console.log("init params = " + "h = " + h + " w = " + w + " lw = " + lw);
	var start = new go.Point(coord.startX - (w / 2) + (lw / 2), coord.startY - (h / 2) + (lw / 2));

	var shape = node.part.part.findObject("FROMARROW");
	a = shape.actualBounds;
	h = a.height;
	w = a.width;
	lw = shape.part.data.strokeWidth;

	if (h !== 9 * lw)
		h = 9 * lw;

	if (w !== 9 * lw)
		w = 9 * lw;

	if (lw <= 2)
		lw = 2;

	//console.log("sec init params = " + "h = " + h + " w = " + w + " lw = " + lw);
	var end = new go.Point(coord.endX - (w / 2) + (lw / 2), coord.endY - (h / 2) + (lw / 2));

	var offset = new go.Point(0, 0);
	if ( start.y < 0 || end.y < 0 )
	{
		offset.y = start.y < end.y ? start.y : end.y;
	}
	if ( start.x < 0 || end.x < 0 )
	{
		offset.x = start.x < end.x ? start.x : end.x;
	}

	//console.log("startx = " + start.x + " endx = " + end.x);
	//console.log("starty = " + start.y + " endy = " + end.y);
	//console.log("Offset Y = " + offset.y);
	//console.log("Offset X = " + offset.x);

	var arrowLineOffset = node.part.data.arrowLineOffset;
	if ( arrowLineOffset.x !== offset.x || arrowLineOffset.y !== offset.y )
	{
		var moveOffset = new go.Point(arrowLineOffset.x - offset.x, arrowLineOffset.y - offset.y);
		node.part.part.move(node.part.part.position.copy().subtract(new go.Point(moveOffset.x, moveOffset.y)));
		node.part.data.arrowLineOffset = offset;
	}

	// Since the arrow does not have the correct angle when the line
	// is rotate, the angle must be set based on the line angle.
	shape.angle = node.part.data.angle;
	var fromArrowObj = node.part.part.findObject("FROMARROW");
	fromArrowObj.angle = shape.angle;

	return start;
}

function endPointToLocation(geo, node)
{
	//SRM
    var coord = getGeomertyCoords(geo);
    var a = node.actualBounds;
    var h = a.height;
	var w = a.width;
	var lw = node.part.data.strokeWidth;

	if ( h !== 9 * lw )
		h = 9 * lw;

	if ( w !== 9 * lw )
		 w = 9 * lw;

	if ( lw <= 2 )
		lw = 2;

	var end = new go.Point(coord.endX - (w / 2) + (lw / 2), coord.endY - (h / 2) + (lw / 2));

	var shape = node.part.part.findObject("TOARROW");
	a = shape.actualBounds;
	h = a.height;
	w = a.width;
	lw = shape.part.data.strokeWidth;

	if (h !== 9 * lw)
		h = 9 * lw;

	if (w !== 9 * lw)
		w = 9 * lw;

	if (lw <= 2)
		lw = 2;

	var start = new go.Point(coord.startX - (w / 2) + (lw / 2), coord.startY - (h / 2) + (lw / 2));

	//console.log(a.height + " " + a.width + " " + shape.part.data.strokeWidth);

	var offset = new go.Point(0, 0);
	if (start.y < 0 || end.y < 0)
	{
		offset.y = start.y < end.y ? start.y : end.y;
	}

	if (start.x < 0 || end.x < 0)
	{
		offset.x = start.x < end.x ? start.x : end.x;
	}

	var arrowLineOffset = node.part.data.arrowLineOffset;

	if (arrowLineOffset.x !== offset.x || arrowLineOffset.y !== offset.y)
	{
		var moveOffset = new go.Point(arrowLineOffset.x - offset.x, arrowLineOffset.y - offset.y);
		node.part.part.move(node.part.part.position.copy().subtract(new go.Point(moveOffset.x, moveOffset.y)));
		node.part.data.arrowLineOffset = offset;
	}

	// Since the arrow does not have the correct angle when the line
	// is rotate, the angle must be set based on the line angle.
	shape.angle = node.part.data.angle;
	var toArrowObj = node.part.part.findObject("TOARROW");
	toArrowObj.angle = shape.angle;

	return end;
}

function endPointToLocationForce(val, node) {
    //SRM
	if(val)
    { 
		node.part.data.forceGeoEndPositionBindings = false;
		var geo = node.part.data.geo;
		return endPointToLocation(geo, node);
}
	return node.part.position;
}

function startPointToLocationForce(val, node) {
    //SRM
	if (val) {
		node.part.data.forceGeoStartPositionBindings = false;
		var geo = node.part.data.geo;
		return startPointToLocation(geo, node);
		}
	return node.part.position;
}

function TagFormatToTagText(tagFormat, textNode)
{
    if ( isDrawBoolean )
    {
        return tagFormat;
    }
    else
    {
        return textNode.text;
    }
}

function TagValueToTagText( tagValue, textNode )
{
    if ( !isDrawBoolean )
    {
        return tagValue;
    }
    else
    {
        return textNode.text;
    }
}

function setBarHeight( height, part )
{
    if ( isDrawBoolean )
    {
        if ( part.part.data )
        {
            part.part.data.maxHeight = height;
        }
        return height;
    }
    else
    {
        return part.height;
    }
};

function setfgheight( bgheight, part )
{
    if ( isDrawBoolean )
    {
        if ( part.part.data.barType === 'Standard' )
        {
            return ( bgheight * ( part.part.data.demoPercent / 100 ) );
        }
        if ( part.part.data.barType === 'Deviation' )
        {
            return ( bgheight * ( Math.abs( ( 50 - part.part.data.demoPercent ) ) / 100 ) );
        }
    }
    else
    {
        return part.part.height;
    }
    return 0;
}

function calculateBarHeightFromVal( val, part )
{
    var value = val;
    var max = ( part.part.data.useTagLimits === 'true' ) ? part.part.data.maxVal : part.part.data.maxUserVal;
    var min = ( part.part.data.useTagLimits === 'true' ) ? part.part.data.minVal : part.part.data.minUserVal;
    return calculateBarHeight( part, value, min, max );
}

function calculateBarHeightFromMax( maxVal, part )
{
    var value = part.part.data.val;
    var max = ( part.part.data.useTagLimits === 'true' ) ? maxVal : part.part.data.maxUserVal;
    var min = ( part.part.data.useTagLimits === 'true' ) ? part.part.data.minVal : part.part.data.minUserVal;
    if ( isDrawBoolean )
    {
        var delta = max - min;
        var percent = part.part.data.demoPercent / 100.00;
        value = delta * percent + min;
    }
    return calculateBarHeight( part, value, min, max );
}

function calculateBarHeightFromMin( minVal, part )
{
    var value = part.part.data.val;
    var max = ( part.part.data.useTagLimits === 'true' ) ? part.part.data.maxVal : part.part.data.maxUserVal;
    var min = ( part.part.data.useTagLimits === 'true' ) ? minVal : part.part.data.minUserVal;
    if ( isDrawBoolean )
    {
        var delta = max - min;
        var percent = part.part.data.demoPercent / 100.00;
        value = delta * percent + min;
    }
    return calculateBarHeight( part, value, min, max );
}


function calculateBarHeight( part, value, min, max )
{
    if ( isNaN( value )
        || isNaN( min )
        || isNaN( max ) )
    {
        return 0;
    }

    if ( part.part.data )
    {
        if ( part.part.data.barType === 'Standard' )
        {
            return ( part.part.data.maxHeight * ( ( value - min ) / ( max - min ) ) );
        }
        else if ( part.part.data.barType === 'Deviation' )
        {
            var midPoint = ( max + min ) / 2.0;
            var deviationFromMidPoint = value - midPoint;
            return ( part.part.data.maxHeight * Math.abs( ( deviationFromMidPoint ) / ( max - min ) ) );
        }
    }
    return part.height;
};

function calculateBarAlignmentFromVal( val, part )
{
    var value = val;
    var max = ( part.part.data.useTagLimits === 'true' ) ? part.part.data.maxVal : part.part.data.maxUserVal;
    var min = ( part.part.data.useTagLimits === 'true' ) ? part.part.data.minVal : part.part.data.minUserVal;
    return calculateBarAlignment( part, value, min, max );
}

function calculateBarAlignmentFromMax( maxVal, part )
{
    var value = part.part.data.val;
    var max = ( part.part.data.useTagLimits === 'true' ) ? maxVal : part.part.data.maxUserVal;
    var min = ( part.part.data.useTagLimits === 'true' ) ? part.part.data.minVal : part.part.data.minUserVal;
    if ( isDrawBoolean )
    {
        var delta = max - min;
        var percent = part.part.data.demoPercent / 100.00;
        value = delta * percent + min;
    }
    return calculateBarAlignment( part, value, min, max );
}

function calculateBarAlignmentFromMin( minVal, part )
{
    var value = part.part.data.val;
    var max = ( part.part.data.useTagLimits === 'true' ) ? part.part.data.maxVal : part.part.data.maxUserVal;
    var min = ( part.part.data.useTagLimits === 'true' ) ? minVal : part.part.data.minUserVal;
    if ( isDrawBoolean )
    {
        var delta = max - min;
        var percent = part.part.data.demoPercent / 100.00;
        value = delta * percent + min;
    }
    return calculateBarAlignment( part, value, min, max );
}


function calculateBarAlignment( part, value, min, max )
{
    if ( isNaN( value )
        || isNaN( min )
        || isNaN( max ) )
    {
        return ( go.Spot.Bottom );
    }
    if ( part.part.data )
    {
        if ( part.part.data.barType === 'Standard' )
        {
            return ( go.Spot.Bottom );
        }
        else if ( part.part.data.barType === 'Deviation' )
        {
            var spot = go.Spot.Center;
            var midPoint = ( max + min ) / 2;
            var deviationFromMidPoint = value - midPoint;
            var offset = -( part.part.data.maxHeight * deviationFromMidPoint * .5 / ( max - min ) );
            var newspot = spot.copy();
            newspot = newspot.setTo( spot.x, spot.y, 0, offset );
            return newspot;
        }
        else
        {
            return ( go.Spot.Bottom );
        }
    }
    return ( go.Spot.Bottom );
}

function calculateBarAlignmentBarType( barType, part )
{
    if ( isDrawBoolean )
    {
        if ( part.part.data )
        {
            if ( part.part.data.barType === 'Standard' )
            {
                return ( go.Spot.Bottom );
            }
        }
        if ( part.part.data.barType === 'Deviation' )
        {
            var spot = go.Spot.Center;
            var offset = part.part.data.maxHeight * ( ( 50 - part.part.data.demoPercent ) / 200 );
            var newspot = spot.copy();
            newspot = newspot.setTo( spot.x, spot.y, 0, offset );
            return newspot;
        }
        else
        {
            return ( go.Spot.Bottom );
        }
    }
    return ( go.Spot.Bottom );
}

function calculateBarHeightFromBarType( barType, part )
{
    if ( isDrawBoolean )
    {
        if ( part.part.data )
        {
            if ( barType === 'Standard' )
            {
                return ( part.part.data.maxHeight * ( part.part.data.demoPercent / 100 ) );
            }
        }
        if ( barType === 'Deviation' )
        {
            return ( part.part.data.maxHeight * ( Math.abs( ( 50 - part.part.data.demoPercent ) ) / 100 ) );
        }
    }
    else
    {
        return part.height;
    }
    return part.height;
}


function calculateBarHeightFromPercent( percent, part )
{
    if ( isDrawBoolean )
    {
        if ( part.part.data )
        {
            if ( part.part.data.barType === 'Standard' )
            {
                return ( part.part.data.maxHeight * ( percent / 100 ) );
            }
        }
        if ( part.part.data.barType === 'Deviation' )
        {
            return ( part.part.data.maxHeight * ( Math.abs( ( 50 - percent ) ) / 100 ) );
        }
    }
    else
    {
        return part.height;
    }
    return part.height;
}

function calculateBarAlignmentFromPercent( percent, part )
{
    if ( isDrawBoolean )
    {
        if ( part.part.data )
        {
            if ( part.part.data.barType === 'Standard' )
            {
                return ( go.Spot.Bottom );
            }
        }
        if ( part.part.data.barType === 'Deviation' )
        {
            var spot = go.Spot.Center;
            var offset = part.part.data.maxHeight * ( ( 50 - percent ) / 200 );
            var newspot = spot.copy();
            newspot = newspot.setTo( spot.x, spot.y, 0, offset );
            return newspot;
        }
        else
        {
            return ( go.Spot.Bottom );
        }
    }
    return ( go.Spot.Bottom );
}

function calculateBarAlignmentFrommaxHeight( maxHeight, part )
{
    if ( isDrawBoolean )
    {
        if ( part.part.data )
        {
            if ( part.part.data.barType === 'Standard' )
            {
                return ( go.Spot.Bottom );
            }
        }
        if ( part.part.data.barType === 'Deviation' )
        {
            var spot = go.Spot.Center;
            var offset = maxHeight * ( ( 50 - part.part.data.demoPercent ) / 200 );
            var newspot = spot.copy();
            newspot = newspot.setTo( spot.x, spot.y, 0, offset );
            return newspot;
        }
        else
        {
            return ( go.Spot.Bottom );
        }
    }
    return ( go.Spot.Bottom );
}


function shadeColor(color, percent) {

    var R = parseInt(color.substring(1, 3), 16);
    var G = parseInt(color.substring(3, 5), 16);
    var B = parseInt(color.substring(5, 7), 16);

    R = parseInt(R *(100 +percent) / 100);
    G = parseInt(G * (100 +percent) / 100);
    B = parseInt(B * (100 +percent) / 100);

    R = (R<255) ?R: 255;
    G = (G < 255) ?G: 255;
    B = (B<255) ?B: 255;

    var RR = ((R.toString(16).length==1) ?"0"+R.toString(16): R.toString(16));
    var GG =((G.toString(16).length==1) ?"0" +G.toString(16): G.toString(16));
    var BB = ((B.toString(16).length==1) ? "0" +B.toString(16): B.toString(16));

    return "#" +RR +GG+BB;
    }

function calculateLinkWidth1 (val, part)
{
			return (val + 8);
}

function calculateLinkGradient1 (val, part)
{
	var hex = FMDrawPropertyMenu.Rgb2Hex(val);
			var rgbObjlink = FMDrawPropertyMenu.HexToRgb(shadeColor(hex, -95));
			var rgbStrlink = FMDrawPropertyMenu.ConvertToRgbaString(rgbObjlink, 1);
			return (rgbStrlink);
}

function calculateLinkWidth2(val, part) {
	return (val + 6);
}

function calculateLinkGradient2(val, part) {
	var hex = FMDrawPropertyMenu.Rgb2Hex(val);
			var rgbObjlink = FMDrawPropertyMenu.HexToRgb(shadeColor(hex, -75));
			var rgbStrlink = FMDrawPropertyMenu.ConvertToRgbaString(rgbObjlink, 1);
			return (rgbStrlink);

}

function calculateLinkWidth3(val, part) {

	return (val + 4);
	}

function calculateLinkGradient3 (val, part)
{
	var hex = FMDrawPropertyMenu.Rgb2Hex(val);
			var rgbObjlink = FMDrawPropertyMenu.HexToRgb(shadeColor(hex, -50));
			var rgbStrlink = FMDrawPropertyMenu.ConvertToRgbaString(rgbObjlink, 1);
			return (rgbStrlink);

			}

function calculateLinkWidth4(val, part) {
	return (val +2);
}

function calculateLinkGradient4(val, part) {
	var hex = FMDrawPropertyMenu.Rgb2Hex(val);
			var rgbObjlink = FMDrawPropertyMenu.HexToRgb(shadeColor(hex, -25));
			var rgbStrlink = FMDrawPropertyMenu.ConvertToRgbaString(rgbObjlink, 1);
			return (rgbStrlink);

}


function showTagWeightsAndMeasuresFunc( val, node )
{
	if (node.part.part.data.TagShowWeightsAndMeasures && node.part.part.data.TagPointValueType === 0 && node.part.part.data.TagFieldSelection === FMTAGFIELDSELECTION.VALUE)
    {
        return val;
    }
    return '';
}

function showTagStatusFunc( val, node )
{
	if (node.font !== "")	// calculate the size of the rectangle for the quality and always offset for it. This is what people want.
	{
		const splitString = node.font.split(" ");
		var iFontWidth = parseInt(splitString[3]);
		if (iFontWidth !== NaN)
		{
			node.width = iFontWidth * 2.5;
			//confirm("In showTagStatusFunc");
		}
	}

	if (node.part.part.data.TagShowStatus && node.part.part.data.TagPointValueType === 0 && node.part.part.data.TagFieldSelection === FMTAGFIELDSELECTION.VALUE)
    {
        return val;
    }
	return '';	//bds
}

function computeSnap( part, pt, gridpt )
{
    // this routine is used when the whole object is moved
    //Enures that snapping does not occur on objects that are part of a group, but the group will snap
    if ( !part || !gridpt || part.containingGroup )
    {
        return pt;
    }

    var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
    var obj = part.resizeObject;
    if ( !diagram ||
        !obj ||
        !FMDrawIndex.IsSnapToGridOn() )
    {
        if (part.category == 'line') {
            // the whole line is moveing and pt is the point to top left corner of the bounding rectangle
            // recalculate the start and stop points on the line so the arrows can be drawn properly
            // determine how much the x and y have changed by picking one coord
            //console.log("pt = " + pt);
            var changeinX = pt.x - obj.part.data.LineStartPositionX;
            var changeinY = pt.y - obj.part.data.LineStartPositionY;

            //console.log("changex = " + changeinX + " changey = " + changeinY);

            // reset the variables by the above offsets
            obj.part.data.LineStartPositionX += changeinX;
            obj.part.data.LineStartPositionY += changeinY;
            obj.part.data.LineEndPositionX += changeinX;
            obj.part.data.LineEndPositionY += changeinY;
        }
        return pt;
    }

    //gridpt is where we are
    //pt is where we are going too
    var offSet = 0;
    if (part.category == 'line')
    {
        /*
        console.log("goint Start X= " + obj.part.data.LineStartPositionX);
        console.log("goint Start Y= " + obj.part.data.LineStartPositionY);
        console.log("goint End X= " + obj.part.data.LineEndPositionX);
        console.log("goint End Y= " + obj.part.data.LineEndPositionY);
        */
        // the value sent to us from gojs is the left top of the rectangle
        // we need to calculate the actual spot for the left most point
        // if the angle is between 0 and 180 we do not need to do anything
        if ((obj.part.data.angle < 0.0 && obj.part.data.angle > -90.0) ||
            (obj.part.data.angle > 90.0 && obj.part.data.angle < 180.0)) {
            // we need to calculate the offset from the topleft to the left position
        	// this should be equal to bottom - top
            //var shape = part.findObject("SHAPE");
            //var top = shape.actualBounds.top;
            //var bottom = shape.actualBounds.bottom;

            ////offSet = bottom - top;
            //offSet = Math.round(bottom - top);
            ////console.log(offSet);
        }
        var startOffsetx = part.data.LineStartPositionX - part.position.x;
        var startOffsety = part.data.LineStartPositionY - part.position.y;
        startOffsetx = (startOffsetx % diagram.model.modelData.snapXCellSize);
        startOffsety = (startOffsety % diagram.model.modelData.snapYCellSize);
    }

    var target = gridpt;
    var targetpos = pt;

    targetpos.y += offSet;

    // adjust the targetpos for the snap position
    var xdistance = diagram.model.modelData.snapXCellSize;
    var ydistance = diagram.model.modelData.snapYCellSize;

//    targetpos.x = Math.round(targetpos.x);
//    targetpos.y = Math.round(targetpos.y);
    var xExtra = targetpos.x % xdistance;
    var yExtra = targetpos.y % ydistance;

    // reset the values
    if (xExtra <= (xdistance / 2)) {
        targetpos.x -= xExtra;
    }
    else {
        targetpos.x += (xdistance - xExtra);
    }
    if (yExtra <= (ydistance / 2)) {
        targetpos.y -= yExtra;
    }
    else {
        targetpos.y += (ydistance - yExtra);
    }
    targetpos.y -= offSet;
    targetpos.x = Math.round(targetpos.x);
    targetpos.y = Math.round(targetpos.y);

    if (part.category == 'line') {
        //console.log("snap targetpos = " + targetpos);
        var changeinX = targetpos.x - obj.part.data.LineStartPositionX;
        var changeinY = targetpos.y - obj.part.data.LineStartPositionY;
        /*
        console.log("changex = " + changeinX + " changey = " + changeinY);

        console.log("goint Start X= " + obj.part.data.LineStartPositionX);
        console.log("goint Start Y= " + obj.part.data.LineStartPositionY);
        console.log("goint End X= " + obj.part.data.LineEndPositionX);
        console.log("goint End Y= " + obj.part.data.LineEndPositionY);
        */
        // reset the variables by the above offsets
        obj.part.data.LineStartPositionX += changeinX;
        obj.part.data.LineStartPositionY += changeinY;
        obj.part.data.LineEndPositionX += changeinX;
        obj.part.data.LineEndPositionY += changeinY;
        //pt.x = ((Math.round(pt.x / 100) * 100) - 8);
        //pt.y = ((Math.round(pt.y / 100) * 100) - 8);
        //return pt;
        targetpos.x = targetpos.x - startOffsetx;
        targetpos.y = targetpos.y - startOffsety;
    }
    return targetpos;
}


function computeLinkSnap( part, pt, gridpt )
{
	gridpt.x = gridpt.x - 4 - part.data.width/2;
	gridpt.y = gridpt.y - 4 - part.data.width/2;
	return gridpt;
}

if (!window.applicationRootName) {
    let p = window.location.pathname.indexOf('/', 1);
    let p0 = window.location.pathname.indexOf('/(S(', 1);
    let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
    window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}


var nodeRotateAdornmentTemplate = $$( go.Adornment,
    { locationSpot: go.Spot.Center, locationObjectName: 'CIRCLE' },
    $$(go.Shape, 'Circle', { name: 'CIRCLE', cursor: 'url(' + window.applicationRootName + '/FMWebApp/images/rotatecursor.cur),default', desiredSize: new go.Size(7, 7), fill: 'lightblue', stroke: 'deepskyblue' }));

function GenerateShapeTemplate( shapeType )
{
    var node = {};
    switch ( shapeType )
    {
        case 'Tag':
            {
                node = $$( go.Node, 'Auto',
                    {
                        name: 'Tag',
                        minSize: new go.Size( 20, 20 ),
                        avoidable: false,
                        resizable: true,
                        rotatable: true,
                        layerName: 'Foreground',
                        resizeObjectName: 'SHAPE',
                        dragComputation: computeSnap,
                        locationSpot: go.Spot.Center,
                        rotateAdornmentTemplate: nodeRotateAdornmentTemplate,
                        toolTip: // define a tooltip for each node that displays the color as text
                            $$( go.Adornment, 'Auto',
                                $$( go.Shape, { fill: '#FFFFCC' } ),
                                $$( go.TextBlock, { margin: 4 },
                                    new go.Binding( 'text', 'ToolTipString' ) )
                            ), // end of Adornment
                        mouseOver: function( e )
                        {
                            var point = e.documentPoint;
                            if ( point )
                            {
                                UpdateCoordinatePanel( point );
                            }
                        }
                    },
                    new go.Binding( 'zOrder', 'zOrder' ).makeTwoWay(),
                    new go.Binding( 'layerName', 'layerName' ).makeTwoWay(),
                    new go.Binding( 'location', 'loc', go.Point.parse ),
                    new go.Binding( 'angle' ).makeTwoWay(),
                    new go.Binding( 'position', 'pos', go.Point.parse ),
                    new go.Binding('selectable', 'selectable'),
						  new go.Binding('visible', 'visible'),
                    $$(go.Shape, 'Rectangle', { name: 'SHAPE', strokeWidth: 2, minSize: new go.Size(20, 20) },
                        new go.Binding('fill', 'color'),//, go.Brush.parse),
                        new go.Binding('desiredSize', 'size', go.Size.parse).makeTwoWay(go.Size.stringify),
                        new go.Binding('strokeWidth', 'strokeWidth') //.makeTwoWay()
                        , new go.Binding( 'stroke', 'lineStroke' ) //.makeTwoWay()
                        , new go.Binding( 'strokeDashArray', 'strokeDashArray' ) //.makeTwoWay()
                    ), $$( go.Panel, 'Table',
                        {
                            padding: 0.5,
                            name: 'VALUETABLE'
                        },
                        new go.Binding( 'padding', 'calMargin', MarginCalculator ).makeTwoWay(),
                        $$( go.RowColumnDefinition, { column: 1 } ),
                        new go.Binding( 'alignment' ),//.makeTwoWay(),
                        new go.Binding( 'angle', 'valuetableangle' ),
                        $$( go.TextBlock,
                            new go.Binding( 'text', 'TagFormat', TagFormatToTagText ),
                            new go.Binding( 'text', 'TagValue', TagValueToTagText ),
                            new go.Binding( 'textAlign' ), //.makeTwoWay(),
                            new go.Binding( 'isUnderline' ), //.makeTwoWay(),
                            new go.Binding( 'stroke' ), //.makeTwoWay(),
                            new go.Binding( 'font' ), //.makeTwoWay(),
                            {
											row: 0,
											column: 0,
											name: 'TEXTBLOCK', // named so that editText can start editing it
											margin: 2,
											editable: false,
											overflow: go.TextBlock.OverflowEllipsis,
											maxLines: 5,
											font: '12px sans-serif',
											visible: true
                            } ),
                        $$( go.Panel, 'Table',
                            {
                                row: 0,
                                column: 1,
                                padding: 0.5
                            },
                            $$( go.RowColumnDefinition, { row: 1 } ),
                            $$( go.TextBlock,
                                new go.Binding( 'font', 'SuperScriptFont' ),
                                new go.Binding( 'text', 'TagStatus', showTagStatusFunc ),
                                new go.Binding( 'stroke' ),
                                {
                                    row: 0,
                                    column: 0,
                                    name: 'SUPERSCRIPTTEXTBLOCK', // named so that editText can start editing it
                                    alignment: go.Spot.TopLeft,
                                    margin: 0,
                                    editable: false,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 5,
											   font: '6px sans-serif',
                                    visible: true
                                }
                            ),
                            $$( go.TextBlock,
                                new go.Binding( 'font', 'SubScriptFont' ),
                                new go.Binding( 'text', 'TagWeightsAndMeasures', showTagWeightsAndMeasuresFunc ),
                                new go.Binding( 'stroke' ),
                                {
                                    row: 1,
                                    column: 0,
                                    name: 'SUBSCRIPTTEXTBLOCK', // named so that editText can start editing it
                                    alignment: go.Spot.BottomLeft,
                                    margin: 0,
                                    editable: false,
                                    overflow: go.TextBlock.OverflowEllipsis,
                                    maxLines: 5,
                                    font: '7px sans-serif',
                                    visible: true
                                }
                            )
                        )
                    )
                );
            }
            break;

        case 'Text':
            {
                node =
                    $$( go.Node, 'Auto',
                        {
                            minSize: new go.Size( 20, 20 ),
                            avoidable: false,
                            resizable: true,
                            rotatable: true,
                            layerName: 'Foreground',
                            dragComputation: computeSnap,
                            resizeObjectName: 'TEXTBLOCK',
                            locationSpot: go.Spot.Center,
                            rotateAdornmentTemplate: nodeRotateAdornmentTemplate,
                            doubleClick: function( e, node )
                            {
                                if ( node.textEditable === false )
                                {
                                    return;
                                }
                                var obj = node.findObject( 'TEXTBLOCK' );
                                var diagram = node.diagram;
                                var oldeditor = diagram.toolManager.textEditingTool.defaultTextEditor;
                                diagram.toolManager.textEditingTool.defaultTextEditor = window.TextEditor;
                                diagram.currentTool = diagram.toolManager.textEditingTool;
                                var tool = diagram.currentTool;
                                tool.textBlock = obj;
                                tool.doStart();
                                diagram.toolManager.textEditingTool.defaultTextEditor = oldeditor;
                            },
                            mouseOver: function( e )
                            {
                                var point = e.documentPoint;
                                if ( point )
                                {
                                    UpdateCoordinatePanel( point );
                                }
                            }
                        },

                        //GENERATE NODE DATA BINDINGS FOR DATA MODEL
                        new go.Binding( 'zOrder', 'zOrder' ).makeTwoWay(),
                        new go.Binding( 'layerName', 'layerName' ).makeTwoWay(),
                        new go.Binding( 'location', 'loc', go.Point.parse ),
                        new go.Binding( 'angle' ).makeTwoWay(),
                        new go.Binding( 'position', 'pos', go.Point.parse ),
                        new go.Binding('selectable', 'selectable'),
								new go.Binding('visible', 'visible'),
                        $$( go.TextBlock,
                            {
                                name: 'TEXTBLOCK',
                                text: 'Click to edit',
                                stroke: 'black',
                                editable: true,
                                margin: 0,
                                textAlign: 'center',
                                wrap: go.TextBlock.None,
                                font: '12px sans-serif'

                            }, new go.Binding('text').makeTwoWay(),
                            new go.Binding('textAlign'),
                            new go.Binding('isUnderline'),
                            new go.Binding('stroke'),
                            new go.Binding('alignment'),
                            new go.Binding('font'),
                            new go.Binding('desiredSize', 'size', go.Size.parse).makeTwoWay(go.Size.stringify),
                            new go.Binding('angle', 'textangle')
                        )
                    );
            }
            break;

        default:
        	{
        		node =
					 $$( go.Node, 'Auto',
						  //SET PROPERTIES OF NODE
						  {
						  	avoidable: false,
						  	minSize: new go.Size( 2, 2 ),
						  	resizable: true,
						  	rotatable: true,
						  	locationSpot: go.Spot.Center,
						  	resizeObjectName: 'SHAPE',
						  	layerName: 'Foreground',
						  	dragComputation: computeSnap,
						  	rotateAdornmentTemplate: nodeRotateAdornmentTemplate,
						  	doubleClick: function( e, node )
						  	{
						  		if ( node.textEditable === false )
						  		{
						  			return;
						  		}
						  		var obj = node.findObject( 'TEXTBLOCK' );
						  		var diagram = node.diagram;
						  		diagram.currentTool = diagram.toolManager.textEditingTool;
						  		var tool = diagram.toolManager.textEditingTool;
						  		tool.textBlock = obj;
						  		tool.doStart();
						  	},
						  	mouseOver: function( e )
						  	{
						  		var point = e.documentPoint;
						  		if ( point )
						  		{
						  			UpdateCoordinatePanel( point );
						  		}
						  	}
						  },

						  //GENERATE NODE DATA BINDINGS FOR DATA MODEL
						  new go.Binding( 'zOrder', 'zOrder' ).makeTwoWay(),
						  new go.Binding( 'layerName', 'layerName' ).makeTwoWay(),
						  new go.Binding( 'location', 'loc', go.Point.parse ),
						  new go.Binding( 'angle' ).makeTwoWay(),
						  new go.Binding( 'position', 'pos', go.Point.parse ),
						  new go.Binding('selectable', 'selectable'),
						  new go.Binding('visible', 'visible'),

						  //GENERATE SHAPE CONTROL THAT WILL RESIDE INSIDE NODE
						  $$( go.Shape, shapeType,
								//SET PROPERTIES OF SHAPE INSIDE THE NODE
								{
									name: 'SHAPE',
									strokeWidth: 2
								},
								//GENERATE DATA BINDINGS FOR THE SHAPE OBJECT INSIDE THE NODE for DATA MODEL
								new go.Binding('fill', 'color', go.Brush.parse),
								new go.Binding('desiredSize', 'size', go.Size.parse).makeTwoWay(go.Size.stringify),
								new go.Binding('strokeWidth', 'strokeWidth'),
								new go.Binding('stroke', 'lineStroke').makeTwoWay(),
								new go.Binding('strokeDashArray', 'strokeDashArray')
						  ), //end of $$(go.Shape

						  //GENERATE TEXT BLOCK CONTROL THAT WILL RESIDE INSIDE NODE
						  $$( go.TextBlock,

								//SET PROPERTIES OF TEXT BLOCK INSIDE THE NODE
								{
									name: 'TEXTBLOCK', // named so that editText can start editing it
									margin: 5,
									// use the following property if you want users to interactively start
									// editing the text by clicking on it or by F2 if the node is selected:
									editable: true,
									overflow: go.TextBlock.OverflowEllipsis,
									maxLines: 5,
									font: '12px sans-serif'
								}

								//GENERATE DATA BINDINGS FOR THE TEXT BLOCK OBJECT INSIDE THE NODE for DATA MODEL
							  , new go.Binding('text').makeTwoWay(), new go.Binding( 'textAlign' ), new go.Binding( 'isUnderline' ), new go.Binding( 'stroke' ), new go.Binding( 'alignment' ).makeTwoWay(), new go.Binding( 'font' ), new go.Binding( 'margin', 'calMargin', MarginCalculator ).makeTwoWay(), new go.Binding( 'angle', 'textangle' )
						  ), //end of $$(go.TextBlock

	  makePort("T", go.Spot.Top, true, true, shapeType),
	  makePort("L", go.Spot.Left, true, true, shapeType),
	  makePort("R", go.Spot.Right, true, true, shapeType),
	  makePort("B", go.Spot.Bottom, true, true, shapeType),
        	
        		//GENERATE BUTTON PANEL CONTROL THAT WILL RESIDE INSIDE NODE
							$$( go.Panel, 'Auto',

        		//SET PROPERTIES OF BUTTON PANEL INSIDE THE NODE
        	{
        		//GENERATE PANEL CONTROL THAT WILL RESIDE INSIDE BUTTON PANEL AND WILL SERVE AS THE itemTemplate.
        		itemTemplate:
                                    $$( go.Panel, 'Vertical',

                                        //GENERATE TEXT BLOCK CONTROL THAT WILL RESIDE INSIDE itemTemplate PANEL
                                        $$( go.TextBlock,
                                            //SET PROPERTIES OF TEXT BLOCK INSIDE THE itemTemplage PANEL
                                            {
                                            	background: 'white',
                                            	margin: 2,
                                            	editable: true
                                    },
                                            //SET DATA BINDINGS OF TEXT BLOCK INSIDE THE itemTemplate PANEL
                                            new go.Binding( 'text', 'prompttext' ) //.makeTwoWay()
                                        ),

                                        //GENERATE BUTTON CONTROL THAT WILL RESIDE INSIDE itemTemplate PANEL
                                        $$( 'Button',
                                            //SET PROPERTIES OF BUTTON CONTROL INSIDE THE itemTemplage PANEL
                                            {
                                            		click: ChangeTagValue,
                                            	margin: 2,
                                            	height: 25
                                    },
                                            //GENERATE TEXT BLOCK CONTROL THAT WILL RESIDE INSIDE BUTTON CONTROL
                                            $$( go.TextBlock,
                                                {
                                                	margin: 2
                                    },
                                                //SET DATA BINDINGS OF TEXT BLOCK INSIDE THE BUTTON CONTROL
                                                new go.Binding( 'text', 'buttontext' )
                                            )
                                        )
											)
                            },
                            //GENERATE DATA BINDINGS FOR THE BUTTON PANNEL INSIDE THE NODE for DATA MODEL
                            new go.Binding( 'itemArray', 'buttonarray' )
                        ) //end of $$(go.Panel
                    ); //end of $$(go.Node
            }
            break;
    }
    return node;
}

// Define a function for creating a "port" that is normally transparent.
// The "name" is used as the GraphObject.portId, the "spot" is used to control how links connect
// and where the port is positioned on the node, and the boolean "output" and "input" arguments
// control whether the user can draw links from or to the port.
function makePort(name, spot, output, input, shapeType) {
	if (shapeType != 'Rectangle' && shapeType != 'Tag' && shapeType != 'Text')
		return $$(go.Shape, "Circle",
		{
			desiredSize: new go.Size(0, 0),
		});

	// the port is basically just a small transparent square
	return $$(go.Shape, "Circle",
{
	fill: null,  // not seen, by default; set to a translucent gray by showSmallPorts, defined below
	stroke: null,
	desiredSize: new go.Size(7, 7),
	alignment: spot,  // align the port on the main Shape
	alignmentFocus: spot,  // just inside the Shape
	portId: name,  // declare this object to be a "port"
	fromSpot: spot, toSpot: spot,  // declare where links may connect at this port
	fromLinkable: output, toLinkable: input,  // declare whether the user may draw links to/from here
	cursor: "pointer"  // show a different cursor to indicate potential link point
});
}

function GeneratePolygonTemplate()
{
    var node =
        $$( go.Node, 'Spot',
            //SET PROPERTIES OF NODE
            {
                locationSpot: go.Spot.Center,
                avoidable: false,
                resizable: true,
                resizeObjectName: 'SHAPE',
                rotatable: true,
                layerName: 'Foreground',
                dragComputation: computeSnap,
                reshapable: true, // GeometryReshapingTool assumes nonexistent Part.reshapeObjectName would be "SHAPE"
                selectionAdorned: true,
                selectionObjectName: 'SHAPE',
                rotateAdornmentTemplate: nodeRotateAdornmentTemplate,
                doubleClick: function( e, node )
                {
                    if ( node.textEditable === false )
                    {
                        return;
                    }
                    var obj = node.findObject( 'TEXTBLOCK' );
                    var diagram = node.diagram;
                    diagram.currentTool = diagram.toolManager.textEditingTool;
                    var tool = diagram.toolManager.textEditingTool;
                    tool.textBlock = obj;
                    tool.doStart();
                },
                mouseOver: function( e )
                {
                    var point = e.documentPoint;
                    if ( point )
                    {
                        UpdateCoordinatePanel( point );
                    }
                }
            },

            //GENERATE NODE DATA BINDINGS FOR DATA MODEL
            new go.Binding( 'zOrder', 'zOrder' ).makeTwoWay(),
            new go.Binding( 'layerName', 'layerName' ).makeTwoWay(),
            new go.Binding( 'location', 'loc', go.Point.parse ),
            new go.Binding( 'position', 'pos', go.Point.parse ),
            new go.Binding( 'angle' ).makeTwoWay(),
            new go.Binding( 'selectable', 'selectable' ),
            new go.Binding('reshapable', 'reshapable'),
				new go.Binding('visible', 'visible'),

            //GENERATE SHAPE CONTROL THAT WILL RESIDE INSIDE NODE
            $$( go.Shape,
                {
                    name: 'SHAPE',
                    fill: 'lightblue',
                    strokeWidth: 2
                },

                //GENERATE DATA BINDINGS FOR THE SHAPE OBJECT for DATA MODEL
                new go.Binding( 'desiredSize', 'size', go.Size.parse ).makeTwoWay( go.Size.stringify ),
                new go.Binding( 'geometryString', 'geo' ).makeTwoWay(),
                new go.Binding( 'fill', 'color', go.Brush.parse ), //.makeTwoWay(go.Brush.stringify),
                new go.Binding( 'strokeWidth', 'strokeWidth' ), //.makeTwoWay(),
                new go.Binding( 'stroke', 'lineStroke' ), //.makeTwoWay(),
                new go.Binding( 'strokeDashArray', 'strokeDashArray' )
            ), //end $$(go.Shape

            //GENERATE TEXT BLOCK CONTROL THAT WILL RESIDE INSIDE NODE
            $$( go.TextBlock,

                //SET PROPERTIES OF TEXT BLOCK INSIDE THE NODE
                {
                    name: 'TEXTBLOCK', // named so that editText can start editing it
                    margin: 5,
                    // use the following property if you want users to interactively start
                    // editing the text by clicking on it or by F2 if the node is selected:
                    editable: true,
                    overflow: go.TextBlock.OverflowEllipsis,
                    maxLines: 5,
                    font: '12px sans-serif'
                },

                //GENERATE DATA BINDINGS FOR THE TEXT BLOCK OBJECT INSIDE THE NODE for DATA MODEL
                new go.Binding( 'text' ).makeTwoWay(),
					new go.Binding('textAlign'),
                new go.Binding( 'isUnderline' ),
                new go.Binding( 'stroke' ),
                new go.Binding( 'alignment' ),
                new go.Binding( 'font' ),
                new go.Binding( 'angle', 'textangle' )
            ) //end of $$(go.TextBlock
        ); //end of $$(go.Node

    return node;
}

function GeneratePictureTemplate()
{
    var node =
        $$( go.Node, 'Position',

            //SET PROPERTIES OF NODE
            {
                resizable: true,
                avoidable: false,
                resizeObjectName: 'SHAPE', //Ensures that entire node size is governed by the go.Picture object created below
                rotatable: true,
                locationSpot: go.Spot.Center,
                dragComputation: computeSnap,
                rotateAdornmentTemplate: nodeRotateAdornmentTemplate,
                doubleClick: function()
                {
                    loadingPictureFromDoubleClick = true;
                    $( '#chooseImageFile' ).dialog( 'open' );
                },
                mouseOver: function( e )
                {
                    var point = e.documentPoint;
                    if ( point )
                    {
                        UpdateCoordinatePanel( point );
                    }
                }
            },

            //GENERATE NODE DATA BINDINGS FOR DATA MODEL
            new go.Binding( 'position', 'pos', go.Point.parse ),
            new go.Binding( 'zOrder', 'zOrder' ).makeTwoWay(),
            new go.Binding( 'layerName', 'layerName' ).makeTwoWay(),
            new go.Binding( 'location', 'loc', go.Point.parse ),
            new go.Binding( 'angle' ).makeTwoWay(),
            new go.Binding('selectable', 'selectable'),
				new go.Binding('visible', 'visible'),

            //GENERATE PICTURE CONTROL THAT WILL RESIDE INSIDE NODE
            $$( go.Picture,
                {
                    minSize: new go.Size( 1, 1 ),
                    desiredSize: new go.Size( 60, 20 ),
                    name: 'SHAPE'
                },
                //GENERATE DATA BINDINGS FOR THE PICTURE OBJECT for DATA MODEL
                new go.Binding( 'source', 'source' ),
                new go.Binding( 'desiredSize', 'size', go.Size.parse ).makeTwoWay( go.Size.stringify ),
                new go.Binding( 'opacity', 'transparency', TransparencyToOpacity )
                /*new go.Binding("stroke"),
			new go.Binding("strokeWidth", "strokeWidth").makeTwoWay(),
			new go.Binding("stroke", "lineStroke").makeTwoWay(),
			new go.Binding("strokeDashArray", "strokeDashArray").makeTwoWay()*/
            )
        ); //end of $$(go.Node
    return node;
}

function GenerateButtonTemplate()
{
    var offset = 2.761423749153968;
    var node =
        $$( go.Node, 'Auto',
            //SET PROPERTIES OF NODE
            {
                name: 'button',
                isActionable: false,
                avoidable: false,
                resizable: true,
                locationSpot: go.Spot.Center,
                rotatable: true,
                click: ButtonCommand,
                dragComputation: computeSnap,
                rotateAdornmentTemplate: nodeRotateAdornmentTemplate,
                resizeObjectName: 'BUTTON',
                // save these values for the mouseEnter and mouseLeave event handlers
                "_buttonFillNormal": go.GraphObject.make( go.Brush, 'Linear', { 0: 'white', 1: 'lightgray' } ),
                "_buttonStrokeNormal": 'gray',
                "_buttonFillOver": go.GraphObject.make( go.Brush, 'Linear', { 0: 'white', 1: 'dodgerblue' } ),
                "_buttonStrokeOver": 'blue',
                doubleClick: function( e, node )
                {
                    if ( node.textEditable === false )
                    {
                        return;
                    }
                    var obj = node.findObject( 'TEXTBLOCK' );
                    var diagram = node.diagram;
                    diagram.currentTool = diagram.toolManager.textEditingTool;
                    var tool = diagram.toolManager.textEditingTool;
                    tool.textBlock = obj;
                    tool.doStart();
                },
                mouseOver: function( e )
                {
                    var point = e.documentPoint;
                    if ( point )
                    {
                        UpdateCoordinatePanel( point );
                    }
                }

            },

            //GENERATE NODE DATA BINDINGS FOR DATA MODEL
            new go.Binding( 'zOrder', 'zOrder' ).makeTwoWay(),
            new go.Binding( 'layerName', 'layerName' ).makeTwoWay(),
            new go.Binding( 'location', 'loc', go.Point.parse ),
            new go.Binding( 'angle' ).makeTwoWay(),
            new go.Binding( 'position', 'pos', go.Point.parse ),
            new go.Binding('selectable', 'selectable'),
				new go.Binding('visible', 'visible'),

            //GENERATE SHAPE THAT WILL RESIDE INSIDE NODE
            go.GraphObject.make( go.Shape, // the border
                {
                    name: 'BUTTON',
                    figure: 'Rectangle',
                    spot1: new go.Spot( 0, 0, offset, offset ),
                    spot2: new go.Spot( 1, 1, -offset, -offset ),
                    fill: go.GraphObject.make( go.Brush, 'Linear', { 0: 'white', 1: 'lightgray' } ),
                    stroke: 'gray'
                },
                //GENERATE DATA BINDINGS FOR THE SHAPE OBJECT INSIDE THE NODE for DATA MODEL
                new go.Binding( 'fill', 'color', go.Brush.parse ),
                new go.Binding( 'strokeWidth', 'strokeWidth' ),
                new go.Binding( 'stroke', 'lineStroke' ),
                new go.Binding( 'strokeDashArray', 'strokeDashArray' ),
                new go.Binding( 'desiredSize', 'size', go.Size.parse ).makeTwoWay( go.Size.stringify )
            ),
            $$( go.TextBlock,

                //SET PROPERTIES OF TEXT BLOCK INSIDE THE NODE
                {
                    name: 'TEXTBLOCK', // named so that editText can start editing it
                    margin: 5,
                    // use the following property if you want users to interactively start
                    // editing the text by clicking on it or by F2 if the node is selected:
                    editable: true,
                    overflow: go.TextBlock.OverflowEllipsis,
                    maxLines: 5
                }

                //GENERATE DATA BINDINGS FOR THE TEXT BLOCK OBJECT INSIDE THE NODE for DATA MODEL
                , new go.Binding( 'text' ).makeTwoWay(), new go.Binding( 'textAlign' ), new go.Binding( 'isUnderline' ), new go.Binding( 'stroke' ), new go.Binding( 'alignment' ), new go.Binding( 'font' ), new go.Binding( 'margin', 'calMargin', MarginCalculator ).makeTwoWay(), new go.Binding( 'angle', 'textangle' )
            ) //end of $$(go.TextBlock
        ); //end of $$(go.Node

    return node;
}

function GenerateBarTemplate()
{
    var node = $$( go.Node, 'Auto',
        {
            name: 'Bar',
            minSize: new go.Size( 2, 2 ),
            avoidable: false,
            resizable: true,
            rotatable: true,
            locationSpot: go.Spot.Center,
            resizeObjectName: 'BGSHAPE',
            dragComputation: computeSnap,
            layerName: 'Foreground',
            rotateAdornmentTemplate: nodeRotateAdornmentTemplate,
            doubleClick: function( e, node )
            {
                if ( node.textEditable === false )
                {
                    return;
                }
                var obj = node.findObject( 'TEXTBLOCK' );
                var diagram = node.diagram;
                diagram.currentTool = diagram.toolManager.textEditingTool;
                var tool = diagram.toolManager.textEditingTool;
                tool.textBlock = obj;
                tool.doStart();
            },
            mouseOver: function( e )
            {
                var point = e.documentPoint;
                if ( point )
                {
                    UpdateCoordinatePanel( point );
                }
            }
        },
        new go.Binding( 'zOrder', 'zOrder' ).makeTwoWay(),
        new go.Binding( 'layerName', 'layerName' ).makeTwoWay(),
        new go.Binding( 'location', 'loc', go.Point.parse ),
        new go.Binding( 'angle' ).makeTwoWay(),
        new go.Binding( 'position', 'pos', go.Point.parse ),
        new go.Binding('selectable', 'selectable'),
		  new go.Binding('visible', 'visible'),
        $$( go.Shape, 'rectangle', {
                name: 'BGSHAPE'
            },
            new go.Binding( 'fill', 'bgcolor', go.Brush.parse ), //.makeTwoWay(go.Brush.stringify),
            new go.Binding( 'desiredSize', 'bgsize', go.Size.parse ).makeTwoWay( go.Size.stringify ),
            //new go.Binding("patternImageName", "bgpatternImageName").makeTwoWay(),
            new go.Binding( 'width', 'bothwidth' ).makeTwoWay(),
            //new go.Binding("bgtransparency", "bgtransparency").makeTwoWay(),
            new go.Binding( 'strokeWidth', 'strokeWidth' ),
            new go.Binding( 'stroke', 'lineStroke' ),
            new go.Binding( 'strokeDashArray', 'strokeDashArray' ),
            new go.Binding( 'height', 'maxHeight', setBarHeight ).makeTwoWay() ),
        $$( go.Shape, 'rectangle', {
                alignment: go.Spot.Bottom,
                name: 'SHAPE'
            },
            new go.Binding( 'fill', 'color', go.Brush.parse ), //.makeTwoWay(go.Brush.stringify),
            new go.Binding( 'height', 'barType', calculateBarHeightFromBarType ),
            new go.Binding( 'alignment', 'barType', calculateBarAlignmentBarType ),
            new go.Binding( 'desiredSize', 'size', go.Size.parse ).makeTwoWay( go.Size.stringify ),
            new go.Binding( 'height', 'val', calculateBarHeightFromVal ),
            new go.Binding( 'alignment', 'val', calculateBarAlignmentFromVal ),
            new go.Binding( 'height', 'minVal', calculateBarHeightFromMin ),
            new go.Binding( 'alignment', 'minVal', calculateBarAlignmentFromMin ),
            new go.Binding( 'height', 'maxVal', calculateBarHeightFromMax ),
            new go.Binding( 'alignment', 'maxVal', calculateBarAlignmentFromMax ),
            new go.Binding( 'width', 'bothwidth' ),
            new go.Binding( 'height', 'demoPercent', calculateBarHeightFromPercent ),
            new go.Binding( 'alignment', 'demoPercent', calculateBarAlignmentFromPercent ),
            new go.Binding( 'alignment', 'maxHeight', calculateBarAlignmentFrommaxHeight ),
            new go.Binding( 'strokeWidth', 'strokeWidth' ),
            new go.Binding( 'stroke', 'lineStroke' ),
            new go.Binding( 'strokeDashArray', 'strokeDashArray' ),
            new go.Binding( 'height', 'maxHeight', setfgheight ) ),
        $$( go.TextBlock,
            {
                name: 'TEXTBLOCK', // named so that editText can start editing it
                margin: 3,
                // use the following property if you want users to interactively start
                // editing the text by clicking on it or by F2 if the node is selected:
                editable: true,
                overflow: go.TextBlock.OverflowEllipsis,
                maxLines: 5
            },
            new go.Binding( 'text' ).makeTwoWay(),
            new go.Binding( 'textAlign' ), //.makeTwoWay(),
            new go.Binding( 'isUnderline' ), //.makeTwoWay(),
            new go.Binding( 'stroke' ), //.makeTwoWay(),
            new go.Binding( 'alignment' ), //.makeTwoWay(),
            new go.Binding( 'font' ), //.makeTwoWay()),
            new go.Binding( 'angle', 'textangle' ) ),
        $$( go.Panel, 'Auto',
            new go.Binding( 'itemArray', 'buttonarray' ),
            {
                itemTemplate:
                    $$( go.Panel, 'Vertical',
                        $$( go.TextBlock, new go.Binding( 'text', 'prompttext' ).makeTwoWay(), { background: 'white', margin: 2, editable: true } ),
                        $$( 'Button', { click: ChangeTagValue, margin: 2, height: 25 },
                            $$( go.TextBlock, new go.Binding( 'text', 'buttontext' ), { margin: 2 } ) ) )
            } ) );


    return node;
}

function GenerateLinkTemplate()
{
    var linkSelectionAdornmentTemplate =
        $$( go.Adornment, 'Link',
            $$( go.Shape,
                // isPanelMain declares that this Shape shares the Link.geometry
                {
                    isPanelMain: true,
                    fill: null,
                    stroke: null,
                    strokeWidth: 1,
                    toArrow: 'None'
                }
            ) // use selection object's strokeWidth
        );
    var linkTemplate =
        $$(go.Link, // the whole link panel
            {
            	selectable: true,
            	resizeObjectName: 'SHAPE',
            	selectionAdornmentTemplate: linkSelectionAdornmentTemplate,
            	resizeAdornmentTemplate: linkSelectionAdornmentTemplate,
            	relinkableFrom: true,
            	relinkableTo: true,
            	reshapable: true,
            	routing: go.Link.Orthogonal,
            	dragComputation: computeLinkSnap,
            	//routing: go.Link.Orthogonal,
            	resegmentable: true,
            	adjusting: go.Link.End,
            	curve: go.Link.None,
            	//toShortLength: 0,
            	corner: 10,
            	smoothness: 1
            	//toShortLength: 4
            },
            new go.Binding('zOrder', 'zOrder').makeTwoWay(),
            new go.Binding('layerName', 'layerName').makeTwoWay(),
            new go.Binding('points', 'points').makeTwoWay(),
				//new go.Binding('location', 'loc', go.Point.parse),
				new go.Binding('position', 'pos', go.Point.parse),
            //new go.Binding('desiredSize', 'size', go.Size.parse).makeTwoWay(go.Size.stringify),
            new go.Binding('selectable', 'selectable'),
            new go.Binding('reshapable', 'reshapable'),
				new go.Binding('visible', 'visible'),
        $$(go.Shape, {
        	isPanelMain: true, stroke: "black", strokeWidth: 11
        },
                new go.Binding('strokeWidth', 'width', calculateLinkWidth1),
                new go.Binding('stroke', 'color', calculateLinkGradient1)//.makeTwoWay(),
				),
        $$(go.Shape, {
        	isPanelMain: true, stroke: "#5e5e5e", strokeWidth: 9
        },
                new go.Binding('strokeWidth', 'width', calculateLinkWidth2),
                new go.Binding('stroke', 'color', calculateLinkGradient2)//.makeTwoWay(),
				),
        $$(go.Shape, {
        	isPanelMain: true, stroke: "#8c8c8c", strokeWidth: 7
        },
                new go.Binding('strokeWidth', 'width', calculateLinkWidth3),
                new go.Binding('stroke', 'color', calculateLinkGradient3)//.makeTwoWay(),
				),
        $$(go.Shape, {
        	isPanelMain: true, stroke: "#b2b2b2", strokeWidth: 5
        },
                new go.Binding('strokeWidth', 'width', calculateLinkWidth4),
                new go.Binding('stroke', 'color', calculateLinkGradient4)//.makeTwoWay(),
				),
            $$(go.Shape,
                {
                	isPanelMain: true,
                	name: 'SHAPE',
                	fill: 'lightblue',
                	stroke: '#99ccff',
                	strokeWidth: 3,
                },
               // new go.Binding('desiredSize', 'size', go.Size.parse).makeTwoWay(go.Size.stringify),
                //new go.Binding('geometryString', 'geo').makeTwoWay(),
                //new go.Binding('fill', 'color', go.Brush.parse), //.makeTwoWay(go.Brush.stringify),
                new go.Binding('strokeWidth', 'width'),//.makeTwoWay(),
                new go.Binding('stroke', 'color'), //.makeTwoWay(),
                //new go.Binding('geometryString', 'angle', SetGeometryStringFromAngle).makeTwoWay(SetAngleFromGeometryString),
                new go.Binding('strokeDashArray', 'strokeDashArray')
            ),


            // Set the arrows to default to Standard. The "toArrow/fromArrow" attribute set to 
            // empty string makes the arrow to not show.  The fromArrow will always have the 
            // word Backward in the name (i.e. Triangle, BackwardTriangle, etc.). Standard is 
            // the exception.
            $$( go.Shape,
                { name: 'TOARROW', toArrow: '', fill: null, scale: 2, stroke: '#000' }, new go.Binding( 'toArrow' ), new go.Binding( 'fill', 'toArrowFill' ), new go.Binding( 'scale', 'toArrowScale' ), new go.Binding( 'stroke', 'toArrowStroke' ) ),
            $$( go.Shape,
                { name: 'FROMARROW', fromArrow: '', fill: null, scale: 2, stroke: '#000' }, new go.Binding( 'fromArrow' ), new go.Binding( 'fill', 'fromArrowFill' ), new go.Binding( 'scale', 'fromArrowScale' ), new go.Binding( 'stroke', 'fromArrowStroke' ) )
        );
    return linkTemplate;
}

function GenerateGroupTemplate()
{
    var groupTemplate = $$( go.Group, 'Position',
        {
            resizable: true,
            avoidable: false,
            rotatable: true,
            locationSpot: go.Spot.Center,
            rotateAdornmentTemplate: nodeRotateAdornmentTemplate,
            resizeObjectName: 'MyPanel'
        },

        //GENERATE GROUP DATA BINDINGS FOR DATA MODEL
        new go.Binding( 'zOrder', 'zOrder' ).makeTwoWay(),
        new go.Binding( 'selectable', 'selectable' ),
        new go.Binding( 'layerName', 'layerName' ).makeTwoWay(),
        new go.Binding( 'location', 'loc', go.Point.parse ),
        new go.Binding( 'position', 'pos', go.Point.parse ),
        new go.Binding('angle').makeTwoWay(),
		  new go.Binding('visible', 'visible'),
        $$( go.Panel, 'Auto',
            {
                name: 'MyPanel'

            },
            $$( go.Shape, 'Rectangle', // surrounds the Placeholder
                {
                    fill: 'rgba(0,0,255,0)',
                    strokeWidth: 0
                },
                //GENERATE DATA BINDINGS FOR THE SHAPE OBJECT for DATA MODEL
                new go.Binding( 'desiredSize', 'size', go.Size.parse ).makeTwoWay( go.Size.stringify ),
                new go.Binding( 'geometryString', 'geo' ).makeTwoWay(),
                new go.Binding( 'fill', 'color', go.Brush.parse ),
                new go.Binding( 'strokeWidth', 'strokeWidth' ),
                new go.Binding( 'stroke', 'lineStroke' ),
                new go.Binding( 'strokeDashArray', 'strokeDashArray' )
            ),
            $$( go.Placeholder, // represents the area of all member parts,
                {
                    padding: 5

                }
            ) // with some extra padding around them
        )
    );
    return groupTemplate;
}

function stayInViewport( part, pt )
{
    var diagram = part.diagram;

    if ( !diagram )
    {
        return pt;
    }

    // compute the area inside the viewport
    var v = diagram.viewportBounds.copy();
    v.subtractMargin( diagram.padding );

    // get the bounds of the part being dragged
    var b = part.actualBounds;
    var loc = part.location;

    // now limit the location appropriately
    var x = Math.max( v.x + 1, Math.min( pt.x, v.right - b.width - 2 ) ) + ( loc.x - b.x );
    var y = Math.max( v.y + 1, Math.min( pt.y, v.bottom - b.height - 2 ) ) + ( loc.y - b.y );

    return new go.Point( x, y );
}

function ButtonCommand( e, obj )
{
    if ( !obj || !obj.data || !obj.data.buttonActionType || typeof FMOperateIndex !== 'object' )
    {
        return;
    }

    var pointGuidObj;
	var pointId;

    switch ( obj.data.buttonActionType )
    {
        case ButtonActionTypeCommand:
        	if (typeof FMOperateIndex.editValue == 'function')
            {
                var tagGuid = obj.data.buttonActionObjectGuid;
                if ( !tagGuid )
                {
                    FMLayout.Alert( 'Button is not attached to point value.', 'Point Value Command' );
                }
                else
                {
                	var pointValueIdentifier = { IdentityGuid: obj.data.TagGUID, PointValueType: obj.data.TagPointValueType, PropertyID: obj.data.TagPropertyID };
                	FMOperateIndex.editValue(pointValueIdentifier);
                }
            }
        	break;

        case ButtonActionTypeGraphic:
            var drawingGuidObj = obj.data.buttonActionObjectGuid;
            var drawingId = obj.data.buttonActionObjectId;

            if ( typeof FMOperateIndex == 'object' && typeof FMOperateIndex.openDraw == 'function' && drawingGuidObj )
            {
                if ( !drawingGuidObj.drawingGuid )
                {
                    FMLayout.Alert( 'Button is not attached to graphic.', 'Open Graphic' );
                }
                else
                {
                    FMOperateIndex.openDraw( drawingId, drawingGuidObj );
                }
            }
            break;

    	case ButtonActionTypePointTrend:
    		pointGuidObj = obj.data.buttonActionObjectGuid;
    		pointId = obj.data.buttonActionObjectId;

     		if (typeof FMOperateIndex == 'object' && typeof FMOperateIndex.openPointTrend == 'function' && pointGuidObj)
     		{
				 FMOperateIndex.openPointTrend(pointId, pointGuidObj);
		    }
     		break;

    	case ButtonActionTypeDetail:
    		pointGuidObj = obj.data.buttonActionObjectGuid;
    		pointId = obj.data.buttonActionObjectId;

    		if (typeof FMOperateIndex == 'object' && typeof FMOperateIndex.openPointTrend == 'function' && pointGuidObj)
    		{
    			FMOperateIndex.openPoint(pointId, pointGuidObj);
		    }
    		break;

      case ButtonActionTypePointHistory:
        pointGuidObj = obj.data.PointGUID;
        pointId = obj.data.TagPointID;

        if (typeof FMOperateIndex == 'object' && typeof FMOperateIndex.OpenNewPointHistory == 'function' && pointGuidObj) {
          FMOperateIndex.OpenNewPointHistory(pointId, pointGuidObj);
        }
        break;
    }
}

function ChangeTagValue( e, obj )
{
    var node = obj.part;
    var data = node.data;
    var target = data.buttonarray[0].prompttext;
    if ( data )
    {
        node.diagram.startTransaction( 'guidupdate' );
        node.diagram.model.setDataProperty( node.data['buttonarray'], 'store', node.data.text );
        node.diagram.commitTransaction( 'guidupdate' );
    }
    var value = 0;
    $.ajax( {
        type: 'POST',
        async: false,
        processdata: false,
        dataType: 'json',
        url: 'ButtonTagEdit',
        data: {
            guid: target,
            value: value,
            '__RequestVerificationToken': $( 'input[name=__RequestVerificationToken]' ).val()
        },
        success: function( info )
        {
            alert( 'Success. -- ' + info );
        },
        error: function()
        {
            alert( 'Error updating tag. -- ' );
        }
    } );
}

function getGeomertyCoords(geoString)
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

function SetAngleFromGeometryString(val, node)
{
	var coords = getGeomertyCoords(val);

	// calculate the angle based on the geometry string
	var xsize = math.abs(coords.startX - coords.endX);
	var ysize = math.abs(coords.startY - coords.endY);
	var hypotenuse = Math.sqrt((xsize * xsize) + (ysize * ysize));
	var opposite = ysize;
	var calculatedangle = 0.0;
	if (hypotenuse !== 0.0) {
		var sinOfAngleX = opposite / hypotenuse;
		calculatedangle = Math.asin(sinOfAngleX) * 180 / Math.PI;
	}

	if (coords.endY > coords.startY) {
		if (coords.endX > coords.startX) {
			calculatedangle += 0;
		}
		else {
			calculatedangle = 90 + (90 - calculatedangle);
		}
		calculatedangle *= 1;
	}
	else {
		if (coords.endX > coords.startX) {
			calculatedangle += 0;
		}
		else {
			calculatedangle = 90 + (90 - calculatedangle);
		}
		calculatedangle *= -1;
	}

	var ret = Math.round(calculatedangle);
	return ret;
}


function SetGeometryStringFromAngle(val, node)
{
	return node.geometryString;
}

function ConvertAngle(angle) {
	if (angle >= 0 && angle < 360) {
		return angle;
	}
	while (angle < 0) {
		angle += 360;
	}
	while (angle >= 360) {
		angle -= 360;
	}
	return angle;
}

function InvertAngle(angle) {
	angle = ConvertAngle(angle);
	if (angle != 0) {
		angle = 360 - angle;
	}
	return angle;
}

function SetPartAngleFromNodeAngle(val, node) {
	var temporaryVal = val;

	if (!val) {
		temporaryVal = 0;
	}

	var returnVal = temporaryVal;// - node.part.data.PreviousAngleOffSet;
	//returnVal = ConvertAngle( returnVal ); //InvertAngle(returnVal);
	return returnVal;
}

function GenerateLineTemplate()//
{
    var node =
        $$( go.Node, 'Position',
            //SET PROPERTIES OF NODE
            {
                name: 'Line',
                avoidable: false,
                locationSpot: go.Spot.TopLeft,
                resizable: false,
                resizeObjectName: 'SHAPE',
                rotatable: false,
                layerName: 'Foreground',
                dragComputation: computeSnap,
                reshapable: true, // GeometryReshapingTool assumes nonexistent Part.reshapeObjectName would be "SHAPE"
                selectionAdorned: false,
                selectionObjectName: 'SHAPE',
                rotateAdornmentTemplate: nodeRotateAdornmentTemplate,
                doubleClick: function( e, node )
                {
                    if ( node.textEditable === false )
                    {
                        return;
                    }
                    var obj = node.findObject( 'TEXTBLOCK' );
                    var diagram = node.diagram;
                    diagram.currentTool = diagram.toolManager.textEditingTool;
                    var tool = diagram.toolManager.textEditingTool;
                    tool.textBlock = obj;
                    tool.doStart();
                },
                mouseOver: function( e )
                {
                    var point = e.documentPoint;
                    if ( point )
                    {
                        UpdateCoordinatePanel( point );
                    }
                }
            },

            //GENERATE NODE DATA BINDINGS FOR DATA MODEL
            new go.Binding( 'zOrder', 'zOrder' ).makeTwoWay(),
            new go.Binding( 'layerName', 'layerName' ).makeTwoWay(),
            new go.Binding('location', 'loc', go.Point.parse),//.makeTwoWay(go.Point.stringify),
            new go.Binding( 'position', 'pos', go.Point.parse ),//.makeTwoWay(go.Point.stringify),
            new go.Binding('selectable', 'selectable'),
            new go.Binding('reshapable', 'reshapable'),
			   new go.Binding('visible', 'visible'),
            //new go.Binding('angle', 'geo', SetAngleFromGeometryString).makeTwoWay(SetGeometryStringFromAngle),
            //new go.Binding('angle').makeTwoWay(),
            $$(go.Shape,
                {
                	name: 'SELECTASSIST',
                	fill: 'lightblue',
                	strokeWidth: 12,
                	stroke: 'transparent', //Transparent line that follows the actual shape. Makes the line easier to select. 
                	geometryString: 'M 0 0 L 10 0',
                	isGeometryPositioned: true,
                },

                //GENERATE DATA BINDINGS FOR THE SHAPE OBJECT for DATA MODEL
                new go.Binding('desiredSize', 'size', go.Size.parse).makeTwoWay(go.Size.stringify),
                new go.Binding('geometryString', 'geo').makeTwoWay(),
                //new go.Binding( 'fill', 'color', go.Brush.parse ), //.makeTwoWay(go.Brush.stringify),
                //new go.Binding( 'strokeWidth', 'strokeWidth' ),//.makeTwoWay(),
                //new go.Binding('stroke', 'lineStroke'), //.makeTwoWay(),
                new go.Binding('geometryString', 'angle', SetGeometryStringFromAngle).makeTwoWay(SetAngleFromGeometryString)
                //new go.Binding('strokeDashArray', 'strokeDashArray')
            ),
            //GENERATE SHAPE CONTROL THAT WILL RESIDE INSIDE NODE
            $$( go.Shape,
                {
                    name: 'SHAPE',
                    fill: 'lightblue',
                    strokeWidth: 1,
                    geometryString: 'M 0 0 L 10 0'
                },

                //GENERATE DATA BINDINGS FOR THE SHAPE OBJECT for DATA MODEL
                new go.Binding( 'desiredSize', 'size', go.Size.parse ).makeTwoWay( go.Size.stringify ),
                new go.Binding( 'geometryString', 'geo' ).makeTwoWay(),
                new go.Binding( 'fill', 'color', go.Brush.parse ), //.makeTwoWay(go.Brush.stringify),
                new go.Binding( 'strokeWidth', 'strokeWidth' ),//.makeTwoWay(),
                new go.Binding('stroke', 'lineStroke'), //.makeTwoWay(),
                new go.Binding('geometryString', 'angle', SetGeometryStringFromAngle).makeTwoWay(SetAngleFromGeometryString),
                new go.Binding('strokeDashArray', 'strokeDashArray')
            ), //end $$(go.Shape

            $$( go.Shape,
                {
                	name: 'TOARROW',
                	toArrow: '',
                	fill: null,
                	scale: 2,
                	stroke: '#000'
                },
					 new go.Binding('toArrow'),
					 new go.Binding('fill', 'toArrowFill'),
					 new go.Binding('scale', 'toArrowScale'),
					 new go.Binding('position', 'geo', endPointToLocation),
					 //new go.Binding('position', 'forceGeoEndPositionBindings', endPointToLocationForce),
					 new go.Binding('stroke', 'toArrowStroke'),
                    new go.Binding('stroke', 'lineStroke'),
                    new go.Binding('fill', 'lineStroke')
				),
            $$( go.Shape,
                {
                	name: 'FROMARROW',
                	fromArrow: '',
                	fill: null,
                	scale: 2,
                	stroke: '#000'
                },
					 new go.Binding('fromArrow'),
					 new go.Binding('fill', 'fromArrowFill'),
					 new go.Binding('scale', 'fromArrowScale'),
					 new go.Binding('position', 'geo', startPointToLocation),
					 //new go.Binding('position', 'forceGeoStartPositionBindings', startPointToLocationForce),
					 new go.Binding('stroke', 'fromArrowStroke'),
                    new go.Binding('stroke', 'lineStroke'),
                    new go.Binding('fill', 'lineStroke')
				),

            //GENERATE TEXT BLOCK CONTROL THAT WILL RESIDE INSIDE NODE
            $$( go.TextBlock,

                //SET PROPERTIES OF TEXT BLOCK INSIDE THE NODE
                {
                    name: 'TEXTBLOCK', // named so that editText can start editing it
                    margin: 5,
                    // use the following property if you want users to interactively start
                    // editing the text by clicking on it or by F2 if the node is selected:
                    editable: true,
                    overflow: go.TextBlock.OverflowEllipsis,
                    maxLines: 5,
                    font: '12px sans-serif'
                },

                //GENERATE DATA BINDINGS FOR THE TEXT BLOCK OBJECT INSIDE THE NODE for DATA MODEL
                new go.Binding( 'text' ).makeTwoWay(),
                new go.Binding( 'textAlign' ),
                new go.Binding( 'isUnderline' ),
                new go.Binding( 'stroke' ),
                new go.Binding( 'alignment' ),
                new go.Binding( 'font' )//,
                //new go.Binding('angle').makeTwoWay()
                //new go.Binding('angle', 'textangle')
            ) //end of $$(go.TextBlock
        ); //end of $$(go.Node

    return node;
}

function UpdateCoordinatePanel( point )
{
	if ( xCordText && yCordText )
    {
        xCordText.innerHTML = Math.round( point.x ).toString();
        yCordText.innerHTML = Math.round( point.y ).toString();
    }
}

var triangleTemplate = GenerateShapeTemplate( 'Triangle' );
var circleTemplate = GenerateShapeTemplate( 'Circle' );
var rectangleTemplate = GenerateShapeTemplate( 'Rectangle' );
var ellipseTemplate = GenerateShapeTemplate( 'Ellipse' );
var tagTemplate = GenerateShapeTemplate( 'Tag' );
var textTemplate = GenerateShapeTemplate( 'Text' );
var pictureTemplate = GeneratePictureTemplate();
var buttonTemplate = GenerateButtonTemplate();
var polygonTemplate = GeneratePolygonTemplate( 'SHAPE' );
var linkTemplate = GenerateLinkTemplate();
var barTemplate = GenerateBarTemplate();
var groupTemplate = GenerateGroupTemplate();
var lineTemplate = GenerateLineTemplate();
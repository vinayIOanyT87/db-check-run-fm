//Initialize Object Creation
var FMFormatValues = FMFormatValues || {
    version: '1.1.0'
};

var FMENGINEERINGUNITTYPE = FMENGINEERINGUNITTYPE || {
    FmuNodim: 15
};
var FMENGINEERINGUNIT = FMENGINEERINGUNIT || {
    FM_SiteUnits: 0,
    FML_FtIn16th: 27,
    FML_FtIn8th: 19,
    FM_NONE: 255
}; //Default Number Formatting for US numbers
FMFormatValues.DefaultFormat = {
    //Negative sign property
    NegativeSign: '-', //Default number of digits used by the numberformat
    NumberDecimalDigits: 0, //Seperator used to seperate digits from integers
    NumberDecimalSeparator: '.', //Seperator used to split integer groups (ex: official US formatting of a number is 1,150.50 where "," if the group seperator)
    NumberGroupSeparator: ',', //Group sizes originally an array in .net but normally groups numbers are either by 3 or not grouped at all
    NumberGroupSizes: 3, //Negative patterns used by .net
    NumberNegativePattern: Formatting.NumberNegativePattern.Pattern1
};
FMFormatValues.CreateNumberFormatInfo = function(
                                            negativeSign, numberDecimalPlaces, numberDecimalSeparator, numberGroupSeparator, numberGroupSizes, numberNegativePattern )
{
    return {
        //Negative sign property
        NegativeSign: negativeSign || '-', //Default number of digits used by the numberformat
        NumberDecimalDigits: numberDecimalPlaces || 0, //Seperator used to seperate digits from integers
        NumberDecimalSeparator: numberDecimalSeparator || '.', //Seperator used to split integer groups (ex: official US formatting of a number is 1,150.50 where "," if the group seperator)
        NumberGroupSeparator: numberGroupSeparator || ',', //Group sizes originally an array in .net but normally groups numbers are either by 3 or not grouped at all
        NumberGroupSizes: numberGroupSizes || 3, //Negative patterns used by .net
        NumberNegativePattern: numberNegativePattern || Formatting.NumberNegativePattern.Pattern1
    };
};
if ( typeof exports !== 'undefined' )
{
    exports.FMFormatValues = FMFormatValues;
}

if ( typeof document !== 'undefined' && typeof window !== 'undefined' )
{
    FMFormatValues.document = document;
    FMFormatValues.window = window;
    // ensure globality even if entire library were function wrapped (as in Meteor.js packaging system)
    window.FMFormatValues = FMFormatValues;
}
else
{
    // assume we're running under node.js when document/window are not present
    FMFormatValues.document = require( 'jsdom' )
        .jsdom( '<!DOCTYPE html><html><head></head><body></body></html>' );

    if ( FMFormatValues.document.createWindow )
    {
        FMFormatValues.window = FMFormatValues.document.createWindow();
    }
    else
    {
        FMFormatValues.window = FMFormatValues.document.parentWindow;
    }
}


FMFormatValues.zeroPad = function( num, places )
{
    var zero = places - num.toString().length + 1;
    return Array( +( zero > 0 && zero ) ).join( '0' ) + num;
};
FMFormatValues.IsNullOrEmptyString = function( str )
{
    return ( !str || /^\s*$/.test( str ) );
};

FMFormatValues.FormatValue = function( units, numberFormatInfo, value )
{
    var returnValue;
    

    //Ensure that numberFormatInfo is initialized.  If not, then assume US format
    //The double use of JSON creates copy of formatting object so we don't change it.
    var numberFormatInfoLocal = numberFormatInfo || this.DefaultFormat;

	 if (isNaN(value)) {
		 returnValue = value;
	 }

    else if ( ( units === FMENGINEERINGUNIT.FML_FtIn16th || units === FMENGINEERINGUNIT.FML_FtIn8th ) && !isNaN( value ) )
    {
        // Get Whole Feet to Integer
        var negative = ( value < 0.00 );

        if ( negative )
        {
            value = -value;
        }

        var feet = ~~value;
        var fraction = value - feet;

        // Convert to Inches
        fraction *= 12.0000;
        var inch = ~~fraction;
        fraction -= inch;

        var factor = ( units === FMENGINEERINGUNIT.FML_FtIn16th ) ? ~~16 : ~~8;

        // Convert to Fraction
        fraction *= factor;
       var fract = ~~(fraction + .5);

        if ( fract >= factor )
        {
            inch++;
            fract = 0;

            if ( inch >= 12 )
            {
                feet++;
                inch = 0;
            }
        }

        if ( negative )
        {
            if ( units === FMENGINEERINGUNIT.FML_FtIn16th )
            {
                returnValue = '-' + this.zeroPad( feet, 2 ) + '-' + this.zeroPad( inch, 2 ) + '-' + this.zeroPad( fract, 2 );
            }
            else
            {
                returnValue = '-' + this.zeroPad( feet, 2 ) + '-' + this.zeroPad( inch, 2 ) + '-' + this.zeroPad( fract, 1 );
            }
        }
        else
        {
            if ( units === FMENGINEERINGUNIT.FML_FtIn16th )
            {
                returnValue = this.zeroPad( feet, 2 ) + '-' + this.zeroPad( inch, 2 ) + '-' + this.zeroPad( fract, 2 );
            }
            else
            {
                returnValue = this.zeroPad( feet, 2 ) + '-' + this.zeroPad( inch, 2 ) + '-' + this.zeroPad( fract, 1 );
            }
        }
    }
    else
    {
        var formatter = new Formatting.NumberFormatter( new Formatting.NumberFormatInfo( numberFormatInfoLocal ) );
        returnValue = formatter.ToString( value );
    }

    return returnValue;
};
// bds added for configuration displays
FMFormatValues.FormatValueFullPrecision = function (units, numberFormatInfo, value) {
    var returnValue;
    

    //Ensure that numberFormatInfo is initialized.  If not, then assume US format
    //The double use of JSON creates copy of formatting object so we don't change it.
	 var numberFormatInfoLocal = numberFormatInfo || this.DefaultFormat;

	 if (isNaN(value)) {
		 returnValue = value;
	 }


    else if ((units === FMENGINEERINGUNIT.FML_FtIn16th || units === FMENGINEERINGUNIT.FML_FtIn8th) && !isNaN(value)) {
        // Get Whole Feet to Integer
        var negative = (value < 0.00);

        if (negative) {
            value = -value;
        }

        var feet = ~~value;
        var fraction = value - feet;

        // Convert to Inches
        fraction *= 12.0000;
        var inch = ~~fraction;
        fraction -= inch;

        var factor = (units === FMENGINEERINGUNIT.FML_FtIn16th) ? ~~16 : ~~8;

        // Convert to Fraction
        fraction *= factor;
        var fract = ~~(fraction + .5);

        if (fract >= factor) {
            inch++;
            fract = 0;

            if (inch >= 12) {
                feet++;
                inch = 0;
            }
        }

        if (negative) {
            if (units === FMENGINEERINGUNIT.FML_FtIn16th) {
                returnValue = '-' + this.zeroPad(feet, 2) + '-' + this.zeroPad(inch, 2) + '-' + this.zeroPad(fract, 2);
            }
            else {
                returnValue = '-' + this.zeroPad(feet, 2) + '-' + this.zeroPad(inch, 2) + '-' + this.zeroPad(fract, 1);
            }
        }
        else {
            if (units === FMENGINEERINGUNIT.FML_FtIn16th) {
                returnValue = this.zeroPad(feet, 2) + '-' + this.zeroPad(inch, 2) + '-' + this.zeroPad(fract, 2);
            }
            else {
                returnValue = this.zeroPad(feet, 2) + '-' + this.zeroPad(inch, 2) + '-' + this.zeroPad(fract, 1);
            }
        }
    }
    else {
        
        numberFormatInfoLocal.NumberDecimalDigits = 9;
        var formatter = new Formatting.NumberFormatter(new Formatting.NumberFormatInfo(numberFormatInfoLocal));
        returnValue = formatter.ToString(value);
        if (returnValue.indexOf(numberFormatInfoLocal.NumberDecimalSeparator) > -1) {
            // java script is cra$
            var stLength = returnValue.length;
            for (var loop = stLength; loop > 0; loop--) {
                if (returnValue[loop - 1] === '0') {
                    returnValue = returnValue.substring(0,loop -1);
                }
                else {
                    loop = 0;
                }
            }
            if (returnValue.indexOf(numberFormatInfoLocal.NumberDecimalSeparator) === returnValue.length - 1) {
                returnValue = returnValue.substring(0, returnValue.length - 1);
            }
        }
    }

    return returnValue;
};

FMFormatValues.ParseValue = function( units, numberFormatInfo, valueString, doNotShowAlertOnError )
{
    var returnValue = math.bignumber( 0.0 );
    
    if ( FMFormatValues.IsNullOrEmptyString( valueString ) )
    {
        return returnValue;
    }

    //Ensure that numberFormatInfo is initialized.  If not, then assume US format
    //The double use of JSON creates copy of formatting object so we don't change it.
    var numberFormatInfoLocal = numberFormatInfo || this.DefaultFormat;

    if ( units === FMENGINEERINGUNIT.FML_FtIn16th || units === FMENGINEERINGUNIT.FML_FtIn8th )
    {
        var negative = false;

        if ( valueString.Length === 0 || valueString.Length > 19 )
        {
            return 'Invalid';
        }

        // Trim Leading Spaces if Any
        valueString = valueString.trim();

        if ( valueString.charAt( 0 ) === '-' )
        {
            negative = true;
            valueString = valueString.substring( 1 );
        }

        var iDelimiter = valueString.indexOf( '-' );

        if ( iDelimiter === -1 )
        {
            returnValue = Number( valueString );
        }
        else
        {
            returnValue = Number( valueString.substring( 0, iDelimiter ) );
            valueString = valueString.substring( iDelimiter + 1 );

            iDelimiter = valueString.indexOf( '-' );

            if ( iDelimiter === -1 )
            {
                returnValue += Number( valueString ) / 12;
            }
            else
            {
                returnValue += Number( valueString.substring( 0, iDelimiter ) ) / 12;
                valueString = valueString.substring( iDelimiter + 1 );
                var iFactor = ( units === FMENGINEERINGUNIT.FML_FtIn16th ) ? 192 : 96;
                returnValue += Number( valueString ) / iFactor;
            }
        }

        if ( negative )
        {
            returnValue = -returnValue;
        }
    }
    else
    {
    	var formatter = new Formatting.NumberFormatter(new Formatting.NumberFormatInfo(numberFormatInfoLocal));
    	if ( valueString.toUpperCase().indexOf( "E" ) != -1 ) //don't want scientific notation
	    {
		    valueString = Number( valueString ).toString();
        }
        returnValue = formatter.TryParse(valueString, function (errorMessage) {
            if (doNotShowAlertOnError) {
            }
            else {
                if (FMLayout) {
                    FMLayout.Alert(errorMessage, 'Error');
                }
                else {
                    alert(errorMessage);
                }
            }
        }
            
        );
    }

    return returnValue;
};


FMFormatValues.FormatDateTimeString = function( m, dateTimeFormatInfo )
{
    if ( !m || !dateTimeFormatInfo )
    {
        return 'Invalid Moment or Format';
    }
    var shortTimePattern = dateTimeFormatInfo.ShortTimePattern.replace( /TT/gi, 'A' );
    var formattedDateTime = m.format( dateTimeFormatInfo.ShortDatePattern.toUpperCase() + ' ' + shortTimePattern );
    if ( formattedDateTime.indexOf( 'AM' ) >= 0 || formattedDateTime.indexOf( 'am' ) >= 0 )
    {
        formattedDateTime = formattedDateTime.replace( /AM/gi, dateTimeFormatInfo.AMDesignator );
    }
    else
    {
        if ( formattedDateTime.indexOf( 'PM' ) >= 0 || formattedDateTime.indexOf( 'pm' ) >= 0 )
        {
            formattedDateTime = formattedDateTime.replace( /PM/gi, dateTimeFormatInfo.PMDesignator );
        }
    }
    return formattedDateTime;
};

FMFormatValues.FormatDateString = function (m, dateTimeFormatInfo) {
	if (!m || !dateTimeFormatInfo) {
		return 'Invalid Moment or Format';
	}
	var formattedDate = m.format(dateTimeFormatInfo.ShortDatePattern.toUpperCase());
	return formattedDate;
};

FMFormatValues.FormatTimeSpan = function (m) {
	if (!m ) {
		return 'Invalid TimeSpan';
	}
	var formattedDateTime = (m.Days > 0 ? m.Days + "." : "") + ("000000" + m.Hours).slice(-2) + ":" + ("000000" + m.Minutes).slice(-2) + ":" + ("000000" + m.Seconds).slice(-2);
	return formattedDateTime;
};

FMFormatValues.IsNormalInteger = function( str )
{
    return /^\+?\d+$/.test( str );
};
FMFormatValues.ConvertDateTimeOffsetToMoment = function( timestamp )
{
    /*
     (Eric Simmons - 2016-10-20)
     Got this warning in the Google Chrome console when using the moment function.   We will need to address this   
     Deprecation warning: moment construction falls back to js Date. This is discouraged and will be removed in upcoming major release. 
     Please refer to https://github.com/moment/moment/issues/1407 for more info.
    */

    var m = moment( 'invalid' );
    if ( !timestamp || ( typeof timestamp !== 'string' ) || timestamp.length === 0 )
    {
        return m;
    }
    var len = timestamp.length;
    var index = timestamp.indexOf( '(' );
    if ( index === -1 && FMFormatValues.IsNormalInteger( timestamp ) ) //If timestamp string does not contain ( then perhaps it is already in proper format.
    {
        m = moment( parseInt( timestamp ) );
    }
    else
    {
        var str = timestamp.substring( index + 1, len - 2 );
        if ( FMFormatValues.IsNormalInteger( str ) )
        {
            m = moment( parseInt( str, 10 ) );
        }
    }
    return m;
};
function ConvertToJQueryUIDateFormat( dateFormat )
{
    /*
			 *  Converts a .NET date format into a JQueryUI date format
			 *
			 *  .NET    JQueryUI        Output      Comment
			 *  --------------------------------------------------------------
			 *  d       d               5           day of month(No leading zero)
			 *  dd      dd              05          day of month(two digit)
			 *  ddd     D               Thu         day short name
			 *  dddd    DD              Thursday    day long name
			 *  M       m               11          month of year(No leading zero)
			 *  MM      mm              11          month of year(two digit)
			 *  MMM     M               Nov         month name short
			 *  MMMM    MM              November    month name long.
			 *  yy      y               09          Year(two digit)
			 *  yyyy    yy              2009        Year(four digit)             *
			 */

    var targetFormat = dateFormat;
    // Convert the date
    targetFormat = targetFormat.replace( 'dddd', 'DD' );
    targetFormat = targetFormat.replace( 'ddd', 'D' );

    // Convert month
    if ( targetFormat.search( 'MMMM' ) >= 0 )
    {
        targetFormat = targetFormat.replace( 'MMMM', 'MM' );
    }
    else if ( targetFormat.search( 'MMM' ) >= 0 )
    {
        targetFormat = targetFormat.replace( 'MMM', 'M' );
    }
    else if ( targetFormat.search( 'MM' ) >= 0 )
    {
        targetFormat = targetFormat.replace( 'MM', 'mm' );
    }
    else
    {
        targetFormat = targetFormat.replace( 'M', 'm' );
    }

    // Convert year
    if ( targetFormat.search( 'yyyy' ) >= 0 )
    {
        targetFormat = targetFormat.replace( 'yyyy', 'yy' );
    }
    else if ( targetFormat.search( 'yy' ) >= 0 )
    {
        targetFormat = targetFormat.replace( 'yy', 'y' );
    }

    return targetFormat;
}

function ConvertToJQueryUITimeFormat(timeFormat) {
	var targetFormat = timeFormat;

	if (targetFormat.search('tt') >= 0) {
		targetFormat = targetFormat.replace('tt', 'TT');
	}

	return targetFormat;
}

function ConvertToMomentUITimeFormat(timeFormat) {
	var targetFormat = timeFormat;

	if (targetFormat.search('tt') >= 0) {
		targetFormat = targetFormat.replace('tt', 'a');
	}

	return targetFormat;
}

//================================================================
// This function will create a Moment.js formatted date string
// to be used by the Moment.js format function when formatting
// to the site format.  It returns a Moment.js recognized date
// string.
//================================================================
function GetMomentDateTimeFormattedStr (inDate)
{
	var yr = inDate.getUTCFullYear();
	var mm = inDate.getUTCMonth() + 1;
	var dd = inDate.getUTCDate();
	var hh = inDate.getUTCHours();
	var mn = inDate.getUTCMinutes();
	var ss = inDate.getUTCSeconds();

	var momStr = yr + "-"
				+ (mm < 10 ? "0" + mm : mm.toString())
				+ "-" + (dd < 10 ? "0" + dd : dd.toString())
				+ "T" + (hh < 10 ? "0" + hh : hh.toString())
				+ ":" + (mn < 10 ? "0" + mn : mn.toString())
				+ ":" + (ss < 10 ? "0" + ss : ss.toString())
				+ "+00:00";

	return momStr;
}

//================================================================
// This function will convert the Date object to the sites time
// zone and return a new date object.  The time offset is the 
// hours from GMT (i.e. EST is -5).  Note:  if you look at the 
// value in the debugger is converts it to local which is confusing.
//================================================================
function ConvertToSiteTimezone (inDate, timeOffset)
{
	var newDate = new Date(inDate);
	var offset = timeOffset * 60;
	newDate.setUTCMinutes(newDate.getUTCMinutes() + offset);

	return newDate;
}
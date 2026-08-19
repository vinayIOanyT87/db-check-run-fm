var $dialog = null;

showModalDialogFrame = function (options, zindex)
{

	var defaultOptns = {
		url: null,
		dialogArguments: null,
		height: 'auto',
		width: 'auto',
		position: 'center',
		resizable: true,
		scrollable: true,
		onClose: function () { },
		returnValue: null,
		doPostBackAfterCloseCallback: false,
		postBackElementId: null
	};

	var fns = {
		close: function ()
		{
			opts.returnValue = $dialog.returnValue;
			$dialogWindow.remove();
			$dialog = null;
			opts.onClose();
			if (opts.doPostBackAfterCloseCallback)
			{
				postBackForm(opts.postBackElementId);
			}


		},
		adjustWidth: function ()
		{
			$frame.css("width", "100%");
		}

	};

	var opts = $.extend({}, defaultOptns, options);

	var $frame = $('<iframe id="iframeDialog" />');

	if (opts.scrollable) $frame.css('overflow', 'auto');

	$frame.css({
		'padding': 0,
		'margin': 0,
		'padding-bottom': 10,
        'z-index': 900
	});

	var $dialogWindow = $frame.dialog({
		autoOpen: true,
		modal: true,
		width: opts.width,
		height: opts.height,
		resizable: opts.resizable,
		position: opts.position,
		overlay: {
			opacity: 0.5,
			background: "black"
		},
		close: fns.close,
		resizeStop: fns.adjustWidth,
		dialogClass: 'no-background-image',
		create: function (event, ui) {
			$(event.target).parent().css('position', 'fixed');
			if (zindex != undefined) {
				$(event.target).parent().css('z-index', zindex);
			}

		}, 
	});

	$frame.attr('src', AddCSRFTokenToUrl(opts.url));
	fns.adjustWidth();

	$frame.load(function ()
	{
		if ($dialogWindow)
		{
			var maxTitleLength = 50;
			var title = $(this).contents().find("title").html();

			if ( title == null )
			{
				title = "";
			}

			if (title.length > maxTitleLength)
			{
				title = title.substring(0, maxTitleLength) + '...';
			}

			$dialogWindow.dialog('option', 'title', title);
			var fd = $frame[0].contentDocument;
			var JS = fd.createElement('script');
			JS.text = "function OnPageLoaded() {var f = window.frameElement;/*f.ownerDocument.defaultView.hidePleaseWaitMessage();*/ $(f.parentNode).css('visibility', 'visible');};	OnPageLoaded();";
			fd.head.appendChild(JS);
		}
	});

	$dialog = new Object();
	$dialog.dialogArguments = opts.dialogArguments;
	$dialog.dialogWindow = $dialogWindow;
	$dialog.returnValue = null;

	//displayPleaseWaitMessage();

	var parentDiv = $dialogWindow[0].parentNode;
	$(parentDiv).css("visibility", "hidden");
}


var prntWindow = getParentWindowWithDialog(); //$(top)[0];

//$(document).ready(function() { prntWindow = getParentWindowWithDialog();
//    alert(prntWindow);
//});

var $dlg = prntWindow && prntWindow.$dialog;

function getParentWindowWithDialog()
{
	var p = window.parent;
	var previousParent = p;
	while (p != null)
	{
		if ($(p.document).find('#iframeDialog').length) return p;

		p = p.parent;

		if (previousParent == p) return null;

		// save previous parent

		previousParent = p;
	}
	return null;
}

function setWindowReturnValue(value)
{
	if ($dlg) $dlg.returnValue = value;
}

function getWindowReturnValue()
{
	return $dlg && $dlg.returnValue;
}

//=================================================================
// This function will replace the non breaking space hex value 0xa0
// that Chrome uses instead of the 0x20 space character.
// It will return a new string with the correct space character.
//=================================================================
function ReplaceNonBreakingSpaceHexWithSpace(str)
{
	if ( str == null || typeof ( str ) == "undefined" )
	{
		return str;
	}

	var hexValue = ConvertToHex(str);
	var newHexValue = hexValue;

	if (hexValue.indexOf("A0") >= 0)
	{
		newHexValue = hexValue.replace(new RegExp("A0", 'g'), "20");
	}

	if (hexValue.indexOf("a0") >= 0)
	{
		newHexValue = hexValue.replace(new RegExp("a0", 'g'), "20");
	}

	var newStr = ConvertFromHex(newHexValue);
	return newStr;
}

//=================================================================
// This function converts a string into a string of hex values.
//=================================================================
function ConvertToHex(str)
{
	var hex = '';
	for (var i = 0; i < str.length; i++)
	{
		hex += '' + str.charCodeAt(i).toString(16);
	}
	return hex;
}

//=================================================================
// This function converts a string of hex values into an ASCII
// string.
//=================================================================
function ConvertFromHex(hex)
{
	var hexStr = hex.toString();//force conversion
	var str = '';
	for (var i = 0; i < hexStr.length; i += 2)
	{
		str += String.fromCharCode(parseInt(hexStr.substr(i, 2), 16));
	}
	return str;
}

if ($dlg) window.dialogArguments = $dlg.dialogArguments;
function closeDialogWindow()
{
	if ($dlg) $dlg.dialogWindow.dialog('close');
};
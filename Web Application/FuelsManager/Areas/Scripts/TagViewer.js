// open AddPointTags
function AddPointTags()
{
	// create the backdrop and wait for next modal to be triggered
	$( 'body' ).modalmanager( 'loading' );
	var token = $( '#TagViewerForm input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var url = $( '#urlTagViewerAddPointTag' ).val();
	// close all the notifications currently openned
	FMErrorAndExceptionHandling.CloseNotifications();

	$.ajax( {
		type: 'GET',
		headers: headers,
		url: url,
		cache: false,
		success: function( response )
		{
				FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
				{
					if ( !inError || ( data != null ) )
					{
						// replace the holder with the partial view
						$( '#tagViewerPointTagSelectScreenBody' ).html( data );
						$( '#tagViewerPointTagSelectScreen' ).modal( 'show' );
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
}

function EditValue( pointValueIdentifierString )
{
	var strings = pointValueIdentifierString.split(' ');

	if (strings.length < 2) {
		return;
	}

	var pointValueIdentifier = { IdentityGuid: strings[0], PointValueType: strings[1], PropertyID: (strings.length == 3) ? strings[2] : '' };


	// create the backdrop and wait for next modal to be triggered
	$( 'body' ).modalmanager( 'loading' );
	var token = $( '#TagViewerForm input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	var url = $( '#urlTagViewerEditValue' ).val();

	$.ajax( {
		type: 'POST',
		headers: headers,
		url: url,
		cache: false,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		data: JSON.stringify({ 'pointValueIdentifier': pointValueIdentifier }),
		success: function( response, xhr, settings )
		{
				// instead of using notifications we are using alerts to force the user to acknowledge the error 
				if ( response && response.ErrorMessage )
				{
					var count = 0;

					$.each( response.ErrorMessage, function( key, message )
					{
						count = count + 1;
					} );


					if ( count === 0 )
					{
						$( '#PointTagEditValueScreenBody' ).html( response.Data );
						$( '#PointTagEditValueScreen' ).modal( 'show' );
					}
					else
					{
						// add messages but avoid duplicates
						var seen = {};

						$.each( response.ErrorMessage, function( key, message )
						{
								$.each( message, function( i )
								{
									if ( !seen[message[i]] )
									{
										seen[message[i]] = true;
										FMLayout.Alert( message[i], 'Tag Value Editor' );
									}
								} );
						} );

						$( 'body' ).modalmanager( 'removeLoading' );;
					}
				}
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
}

function SetSortOrder()
{
	var valueTextBox = document.getElementById( 'TVModelObject' );
	if ( valueTextBox != null )
	{
		var currentvalueTextBox = valueTextBox.value;
		var mod = JSON.parse( valueTextBox.value );
		mod.SortOrder = $( '#TagTable' )[0].config.sortList;
		valueTextBox.value = JSON.stringify( mod );
		if ( currentvalueTextBox !== valueTextBox.value )
		{
				var token = $( '#TagViewerForm input[name=__RequestVerificationToken]' ).val();
				var headers = {};
				headers['__RequestVerificationToken'] = token;

				// close all the notifications currently openned
				FMErrorAndExceptionHandling.CloseNotifications();

				var url = $( '#urlTagViewerSetSortOrder' ).val();

				$.ajax( {
					type: 'POST',
					headers: headers,
					url: url,
					datatype: 'text',
					data: { 'modstring': valueTextBox.value },
					success: function( response )
					{
						FMErrorAndExceptionHandling.HandleMessages( response );
					},
					error: function( xhr, textStatus, error )
					{
						FMErrorAndExceptionHandling.ShowException( xhr, textStatus, error );
					}
				} );
		}
	}
}

function GetSortOrder()
{
	var valueTextBox = document.getElementById( 'TVModelObject' );
	if ( valueTextBox != null )
	{
		var mod = JSON.parse( valueTextBox.value );
		return mod.SortOrder;
	}
	return null;
}

function StoreScrollPosition()
{
	var valueTextBox = document.getElementById( 'TVModelObject' );
	if ( valueTextBox != null )
	{
		var mod = JSON.parse( valueTextBox.value );
		mod.lastScrollPosition = $( document ).scrollTop();
		mod.edititemrowposition = GetScrollRowPosition();
		valueTextBox.value = JSON.stringify( mod );
	}
}

function GetScrollBarHeight()
{
	var elmnt = document.getElementById( 'TagTable' );
	var y = elmnt.scrollHeight;
	return ( y );
}

function GetNumberTableRows()
{
	var table = document.getElementById( 'TagTable' );
	var numberRows = 0;
	for ( var i = 0, row; row = table.rows[i]; i++ )
	{
		//iterate through rows
		++numberRows;
	}
	return ( numberRows );
}

function GetScrollRowPosition()
{
	var valueTextBox = document.getElementById( 'TVModelObject' );
	if ( valueTextBox != null )
	{
		var mod = JSON.parse( valueTextBox.value );
		if ( mod.EditItemGuid != null &&
				mod.EditItemGuid.length > 0 )
		{
				var tagtolookfor = 'Value_' + mod.EditItemGuid;
				// because the screen may of been reordered we need to go through the grid and see if we
				// can match the last selected tag guid and scroll to there
				var table = document.getElementById( 'TagTable' );
				for ( var i = 0, row; row = table.rows[i]; i++ )
				{
					//iterate through rows
					//rows would be accessed using the "row" variable assigned in the for loop
					for ( var j = 0, col; col = row.cells[j]; j++ )
					{
						if ( tagtolookfor === col.id )
						{
								return i;
						}
						//iterate through columns
						//columns would be accessed using the "col" variable assigned in the for loop
					}
				}
		}
	}
	return 0;
}

function ReadScrollPosition()
{
	var valueTextBox = document.getElementById( 'TVModelObject' );
	if ( valueTextBox != null )
	{
		var mod = JSON.parse( valueTextBox.value );
		var selectedrow = GetScrollRowPosition();
		var scrollbarheight = GetScrollBarHeight();
		var numberTableRows = GetNumberTableRows();

		if ( selectedrow > 0 )
		{
				if ( selectedrow === mod.edititemrowposition )
				{
					return mod.lastScrollPosition;
				}

				// now that we have the values lets calculate the off set
				if ( numberTableRows > 0 && scrollbarheight > 0 )
				{
					mod.lastScrollPosition = ( selectedrow + 1 ) * ( scrollbarheight / numberTableRows );
				}
				else
				{
					mod.lastScrollPosition = ( selectedrow - 1 ) * 25;
				}
				return mod.lastScrollPosition;
		}
		return null;
	}
	return null;
}


function Refresh()
{
	refreshTimerID = null;

	var token = $( '#TagViewerForm input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	var url = $( '#urlTagViewerRefresh' ).val();

	window.refreshAjaxRequest = $.ajax({
		type: 'GET',
		headers: headers,
		url: url,
		cache: false,
		data: 'initialRefresh=' + initialRefresh,
		success: function (response, xhr, settings)
		{

			if (!window.refreshAjaxRequest) {
				return;
			}

			window.refreshAjaxRequest = null;

			FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
			{
				if ( !inError )
				{
					var updatesort = false;
					var i;
					for ( i = 0; i < data.length; i++ )
					{
						var item = data[i];
						var control = document.getElementById( item.Text );
						if ( control != null )
						{
							if ( control.nodeName === 'IMG' )
							{
								control.style.display = ( item.Value === 'True' ) ? 'inline' : 'none';
							}
							else if(control.nodeName === 'A')
							{
								control.className = item.Value;
								$('#' + control.id).attr('disabled',(item.Value === 'editLinkClass') ? false : true);
							}
							else
							{
								if (control.id.indexOf('Type') != -1) {
									if(item.Value === 'Calculated'){
										control.innerText = $('#calculatedText').val();
									}
									else if(item.Value === 'Manual'){
										control.innerText = $('#manualText').val();
									}
									else if (item.Value === 'OpcUa') {
										control.innerText = $('#opcUaText').val();
									}
									else if (item.Value === 'UnAssigned') {
										control.innerText = $('#unassignedText').val();
									}

								}
								else {
									control.innerText = item.Value;
								}
								var attr = item.Attributes;
								if ( attr != null && attr.Count > 0 )
								{
									control.style.color = attr.Keys[0];
								}
								updatesort = true;
							}
						}
					}
					initialRefresh = false;
					if ( updatesort === true )
					{
						var sortOrder = GetSortOrder();
						$( '#TagTable' ).trigger( 'update' );
						if ( sortOrder != null )
						{
							$( '#TagTable' ).trigger( 'sorton', [sortOrder] );
						}
						AdjustScrollPosition();
					}

					if ( refreshTimerID == null )
					{
						refreshTimerID = window.setTimeout( Refresh, 1000 );
					}
				}
				else
				{
					initialRefresh = true;
					if ( refreshTimerID == null )
					{
						refreshTimerID = window.setTimeout( Refresh, 1000 );
					}
				}
			} );
		},
		error:
			function( xhr, status, error ) {
				FMErrorAndExceptionHandling.CloseNotifications();

				if (!window.refreshAjaxRequest) {
					return;
				}

				window.refreshAjaxRequest = null;
				if (!(status == 'abort')) {

					$('#TagTable tr').each(function (i, row) {
						$(row).find('td.tagColumnStatus').text($('#communicationsFailureText').val()).css({'color' : '#FF4040'});
						$(row).find('td.tagColumnValue').text('');
					});

					initialRefresh = true;
					if (refreshTimerID == null) {
						refreshTimerID = window.setTimeout(Refresh, 1000);
					}
				}
			}
	} );
}


function ClearRefreshTimer()
{
	if ( window.refreshTimerID != null )
	{
		window.clearTimeout(window.refreshTimerID);
		window.refreshTimerID = null;
	}

	if ( window.refreshAjaxRequest != null )
	{
		window.refreshAjaxRequest.abort();
		window.refreshAjaxRequest = null;
	}
}

function GetCurrentValues( control )
{
	var valueTextBox = document.getElementById( 'ValueTextBox' );
	if ( valueTextBox != null )
	{
		control.href = control.href.replace( 'xxxx', valueTextBox.value );
	}
}

function GetModelValues( control )
{
	var valueTextBox = document.getElementById( 'TVModelObject' );
	if ( valueTextBox != null )
	{
		var str = control.nameProp;
		var newstr = str.replace( '?modstring=yyyy', '' );

		var mod = JSON.parse( valueTextBox.value );
		mod.EditItemGuid = newstr;
		valueTextBox.value = JSON.stringify( mod );
		StoreScrollPosition();
		control.href = control.href.replace( 'yyyy', document.getElementById( 'TVModelObject' ).value );
	}
}

function AdjustScrollPosition()
{
	var lastscrollposition = ReadScrollPosition();
	if ( lastscrollposition != null )
	{
		$( document ).scrollTop( ReadScrollPosition() );
	}
}

/* global variables */
var console = {
	log: function()
	{
	}
};
var refreshTimerID = window.setTimeout( Refresh, 1000 );
var initialRefresh = true;
var refreshAjaxRequest = null;

$( document ).ready( function()
{
	var addButton = $('#addButton');
	var ampersandIndex = addButton.text().indexOf('&');
	if (ampersandIndex !== -1) {
		var hotKey = addButton.text().charAt(ampersandIndex + 1);
		addButton.html(addButton.text().replace('&' + hotKey, '<span style="text-decoration: underline;">' + hotKey + '</span>'));
		addButton.attr('accesskey', hotKey);
	}

	var okButton = $('#tagViewerPointTagSelectOkButton');
	ampersandIndex = okButton.text().indexOf('&');
	if (ampersandIndex !== -1) {
		var hotKey = okButton.text().charAt(ampersandIndex + 1);
		okButton.html(okButton.text().replace('&' + hotKey, '<span style="text-decoration: underline;">' + hotKey + '</span>'));
		okButton.attr('accesskey', hotKey);
	}

	var cancelButton = $('#tagViewerPointTagSelectionCancelButton');
	ampersandIndex = cancelButton.text().indexOf('&');
	if (ampersandIndex !== -1) {
		var hotKey = cancelButton.text().charAt(ampersandIndex + 1);
		cancelButton.html(cancelButton.text().replace('&' + hotKey, '<span style="text-decoration: underline;">' + hotKey + '</span>'));
		cancelButton.attr('accesskey', hotKey);
	}




	/************************************** Sorting *********************************************************/
	//Initialize table to have sortable columns
	$( '#TagTable' ).tablesorter(
		{
				dateFormat: 'mmddyyyy', // we always use mmddyyyy when sorting because although the dates are displayed according to regional settings, the column's data-text is formatted mmddyyyy
				theme: 'default', // the default tableSorter theme, which we slightly customize
				headers:
					{
						0: { sorter: false, parser: false },
						3: { sorter: false, parser: false },
						4: { sorter: false, parser: false }
					},

				// Use the stickyHeaders widget so that when the user scrolls down on the grid the headers stay visible
				// Use the zebra widget so we get alternating row colors on the grid.
				widgets: ['stickyHeaders', 'zebra'],
				widgetOptions: {
					zebra: ['odd', 'even'],
					stickyHeaders_attachTo: $( '.stickyHeaderGrid' )
				}

		} );

	//Change the sort order to what is defined in the model
	var sortOrder = GetSortOrder();
	if ( sortOrder != null )
	{
		$( '#TagTable' ).trigger( 'update' );
		$( '#TagTable' ).trigger( 'sorton', [sortOrder] );
	}

	//Set event to update sort order in model in hidden tag every time it changes
	$( '#TagTable' ).on( 'sortEnd', function()
	{
		SetSortOrder( $( '#TagTable' )[0].config.sortList );
	} );

	/*************************************** Scrolling *******************************************************/
	/*		$("#TagTable").niceScroll({
				cursorwidth: '10px'
					, autohidemode: false
					, cursorcolor: "#486899"
					, background: "rgb(240, 240, 240)"
					, railoffset: true
					, railpadding: { top: 0, right: 0, left: -20, bottom: 0 }
					, smoothscroll: false
					, mousescrollstep: 5
			});
	*/
	$( '#tagViewerPointTagSelectOkButton' ).click(
		function()
		{
				var form = $( '#tagViewerPointTagSelectScreen' ).find( 'form' );
				form[0].action += '?command=addButton';
				var valueTextBox = document.getElementById( 'TVModelObject' );
				if ( valueTextBox != null )
				{
					SetSortOrder();
					StoreScrollPosition();
					var hiddenField = document.createElement( 'input' );
					hiddenField.setAttribute( 'type', 'hidden' );
					hiddenField.setAttribute( 'name', 'modStr' );
					hiddenField.setAttribute( 'value', document.getElementById( 'TVModelObject' ).value );

					form[0].appendChild( hiddenField );
				}
				form.submit();
				$( '#tagViewerPointTagSelectScreen' ).html( '' );
		} );


	$( '#DeleteAllCheckBox' ).click( function()
	{
		var deleteChecked = $( '#DeleteAllCheckBox' )[0].checked;
		$( '.NewCheckBox' ).each( function()
		{
				$( this )[0].checked = deleteChecked;
		} );
	} );

	$( '#tagViewerPointTagSelectCancelButton' ).click(
		function()
		{
				$( '#tagViewerPointTagSelectScreen' ).html( '' );
		} );

	// scroll to last position
	AdjustScrollPosition();

	//var niceScroll =  $("#TagTable").getNiceScroll(0);

	//niceScroll.onscrollend = function(info)
	//{
	//	var valueTextBox = document.getElementById('TVModelObject');
	//	if (valueTextBox != null)
	//	{
	//		var mod = JSON.parse(valueTextBox.value);
	//		mod.columnSortDirection= sortdirection;
	//		mod.selectedSortColumn= sortcolumn;
	//		mod.lastScrollPosition = $("#TagTableWrap").niceScroll().getScrollTop();;
	//		var modStr = JSON.stringify(mod);
	//	}
	//};

	window.addEventListener( 'beforeunload', ClearRefreshTimer );
} );
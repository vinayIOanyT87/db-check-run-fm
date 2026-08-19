//--------------------------------------- RUN after page has been loaded but before render -----------------------------


// create a class with helper functions for the OpcUaBrowser view
var FMOpcUaBrowser = function()
{
	//set the position for the messages from the server
	var _stack_bottomright_opcuaeditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 15, "firstpos2": 25, "context": $('#OPCEditorScreen') };

	var _saveChanges = function( action, method )
	{
		var url = $( '#urlSaveChanges' ).val();
		var token = $( '#opcUaBrowserForm input[name=__RequestVerificationToken]' ).val();
		var headers = {};
		var opcUaBrowsePath = $( '#PointTagOpcUaBrowsePathTextBox' ).val();
		var opcUaNodeId = $( '#PointTagOpcUaNodeIdTextBox' ).val();
		var opcUaPublishingInterval = $( '#PointTagOpcUaPublishingIntervalTextBox' ).val();
		var opcUaIsReadable = $( '#PointTagOpcUaIsReadableCheckBox').is(":checked");
		var opcUaWriteHoldoffTime = $('#PointTagOpcUaWriteHoldoffTimeTextBox').val();
		var opcUaWritePeriodicUpdateInterval = $('#PointTagOpcUaWritePeriodicUpdateIntervalTextBox').val();
		var opcUaServerDataType = $('#PointTagOpcUaServerDataTypeDropDownList').val();

		var opcUaHoldoff = $('#PointTagOpcUaClientHoldoffTextBox').val();
		var opcUaDeadband = $('#PointTagOpcUaClientDeadbandTextBox').val();


		headers['__RequestVerificationToken'] = token;

		// specify location for server notifications
		var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaBrowser.stack_bottomright_opcuaeditor };
		// remove any notification
		PNotify.removeStack( FMOpcUaBrowser.stack_bottomright_opcuaeditor );

		$.ajax( {
				url: url,
				cache: false,
				type: 'POST',
				headers: headers,
				contentType: 'application/json; charset=utf-8',
			data: JSON.stringify({
				'opcUaBrowsePath': opcUaBrowsePath, 'opcUaNodeId': opcUaNodeId,
				'opcUaPublishingInterval': opcUaPublishingInterval,
				'opcUaWriteHoldoffTime': opcUaWriteHoldoffTime,
				'opcUaWritePeriodicUpdateInterval': opcUaWritePeriodicUpdateInterval,
				'opcUaIsReadable': opcUaIsReadable,
				'opcUaServerDataType': opcUaServerDataType,
				'opcUaHoldoff': opcUaHoldoff,
				'opcUaDeadband': opcUaDeadband
			}),
				evalScripts: true,
				success: function( response )
				{
					FMErrorAndExceptionHandling.HandleMessages( response, function( result )
					{
						if ( result )
						{
								$( '#opcUaBrowserError' ).html( result );
						}
					}, notificationAttributes );
				},
				error: function( request, status, error )
				{
					FMErrorAndExceptionHandling.ShowError( 'SaveChanges failure' );
				}
		} );
	};
	var _serverAddressSpaceSelectNode = function( e, data )
	{
		console.log('_serverAddressSpaceSelectNode');
		var tree = $( '#serverAddressSpace' ).jstree();

		// update the browse path
		var browsePath = '';
		if (data.node.data.BrowseName) {
			browsePath = data.node.data.BrowseName;
		}
		else {
			browsePath = data.node.text;
		}
		var parentId = tree.get_parent(data.node);
		while ( parentId !== '#' )
		{
			var parentNode = tree.get_node(parentId);
			if (parentNode.data.BrowseName) {
				browsePath = parentNode.data.BrowseName + '.' + browsePath;
			}
			else {
				browsePath = parentNode.text + '.' + browsePath;
			}
			parentId = tree.get_parent(parentNode);
		}
		$( '#PointTagOpcUaBrowsePathTextBox' ).val( browsePath ).attr( 'title', browsePath );

		// update the nodeId
		$('#PointTagOpcUaNodeIdTextBox').val(data.node.id).attr('title', data.node.id);

		// update the Server Data Type
		//$('#PointTagOpcUaServerDataTypeDropDownList').find('option[value="' + data.node.data.DataType + '"]').attr("selected", "selected");
		// changed to below text because the above does not work in Chrome
		$('#PointTagOpcUaServerDataTypeDropDownList').val(data.node.data.DataType).change();


		// return if node already populated
		if ( data.node.children.length > 0 )
		{
				return;
		}
		var url = $( '#urlServerAddressSpaceSelectionChanged' ).val();
		var token = $( '#opcUaBrowserForm input[name=__RequestVerificationToken]' ).val();
		var headers = {};

		headers['__RequestVerificationToken'] = token;
		var nodeIdText = data.node.id;

		// specify location for server notifications
		var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaBrowser.stack_bottomright_opcuaeditor };
		// remove any notification
		PNotify.removeStack( FMOpcUaBrowser.stack_bottomright_opcuaeditor );

		$.ajax( {
				url: url,
				cache: false,
				type: 'POST',
				headers: headers,
				contentType: 'application/json; charset=utf-8',
				data: JSON.stringify({ 'nodeIdText': nodeIdText }),
				success: function( response )
				{
					FMErrorAndExceptionHandling.HandleMessages( response, function( nodes, inError )
					{
						if ( inError )
						{
								return;
						}


						$.each( nodes, function( i, node )
						{
							tree.create_node(data.node, { 'id': node.Id, 'text': node.DisplayName, 'data': node, 'type': 'valid_child' });
						} );

						if ( data.node.children.length > 0 )
						{
								tree.open_node( data.node );
						}
					}, notificationAttributes );
				},
				error: function( request, status, error )
				{
					FMErrorAndExceptionHandling.ShowError( 'AddressSpaceSelecNode failure' );
				}
		} );
	};
	var _serverEndpointChanged = function( postSuccessProcess ) {
		if ($("#EndpointUrlTextBox").val() === '') {
			FMErrorAndExceptionHandling.ShowError('OPC UA Server EndPoint not specified');
			return;
		}

		$( '#PointTagOpcUaBrowsePathTextBox' ).val( '' ).attr( 'title', '' );

		// update the nodeId
		$( '#PointTagOpcUaNodeIdTextBox' ).val( '' ).attr( 'title', '' );

		var tree = $( '#serverAddressSpace' ).jstree();
		tree.destroy();

		// recreate the tree
		tree = $( '#serverAddressSpace' ).jstree( {
				'core': {
					'check_callback': true
				}
		} );

		$( '#serverAddressSpace' ).on( 'select_node.jstree', function( e, data )
		{
				FMOpcUaBrowser.serverAddressSpaceSelectNode( e, data );
		} );

		tree = $( '#serverAddressSpace' ).jstree();

		var url = $( '#urlServerAddressSpaceSelectionChanged' ).val();
		var token = $( '#opcUaBrowserForm input[name=__RequestVerificationToken]' ).val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		// specify location for server notifications
		var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaBrowser.stack_bottomright_opcuaeditor };
		// remove any notification
		PNotify.removeStack( FMOpcUaBrowser.stack_bottomright_opcuaeditor );


		$.ajax( {
				url: url,
				cache: false,
				type: 'POST',
				headers: headers,
				data: 'nodeIdText=',
				success: function( response )
				{
					FMErrorAndExceptionHandling.HandleMessages( response, function( nodes, inError )
					{
						if ( inError )
						{
								return;
						}

						$.each( nodes, function( i, node )
						{
							tree.create_node(null, { 'id': node.Id, 'text': node.DisplayName, 'data': node, 'type': 'valid_child' });
						} );
					}, notificationAttributes );
				},
				error: function( request, status, error )
				{
					FMErrorAndExceptionHandling.ShowError( 'ServerEndpointChanged failure' );
				}
		} );
	};
	var _browseToNode = function( nodeId, browsePath ) {
		var tree = $( '#serverAddressSpace' ).jstree( true );

		var v = tree.get_json( nodeId, { 'flat': true } );
		var i;
		for ( i = 0; i < v.length; i++ )
		{
			var z = v[i];

			// root nodes have null node in data		
			if (( !z.data.BrowseName && browsePath.substr(0, z.text.length ) === z.text )
			|| (z.data.BrowseName && browsePath.substr( 0, z.data.BrowseName.length ) === z.data.BrowseName ))
			{
				if (!z.data.BrowseName && browsePath.length > z.text.length + 1) {
					browsePath = browsePath.substr(z.text.length + 1);
				}
				else if(z.data.BrowseName && browsePath.substr(z.data.BrowseName.length +1)) {
					browsePath = browsePath.substr(z.data.BrowseName.length + 1);
				}
				else {
					browsePath = null;
					tree.select_node(nodeId);
					return;
				}

				var url = $( '#urlServerAddressSpaceSelectionChanged' ).val();
				var token = $( '#opcUaBrowserForm input[name=__RequestVerificationToken]' ).val();
				var headers = {};
				headers['__RequestVerificationToken'] = token;

				// specify location for server notifications
				var notificationAttributes = { addclass: 'stack-bottomright', stack: FMOpcUaBrowser.stack_bottomright_opcuaeditor };
				// remove any notification
				PNotify.removeStack( FMOpcUaBrowser.stack_bottomright_opcuaeditor );

				$.ajax( {
					url: url,
					cache: false,
					type: 'POST',
					headers: headers,
					async: false,
					contentType: 'application/json; charset=utf-8',
					data: JSON.stringify({ 'nodeIdText': z.id }),
					success: function( response )
					{
						FMErrorAndExceptionHandling.HandleMessages( response, function( nodes, inError )
						{
							if ( inError )
							{
								return;
							}

							$.each( nodes, function( i, node )
							{
								tree.create_node( z, { 'id': node.Id, 'text': node.DisplayName, 'data': node, 'type': 'valid_child' } );
								if ( browsePath && browsePath.substr( 0, node.BrowseName.length ) === node.BrowseName )
								{
									nodeId = node.Id;
								}
							} );
							tree.open_node(z.id);
						}, notificationAttributes);
					},
					error: function( request, status, error )
					{
						FMErrorAndExceptionHandling.ShowError( 'BrowseToNode failure' );
					}
				} );

				if ( browsePath )
				{
					_browseToNode( nodeId, browsePath );
				}

				return;
			}
		}
	};
	return {
		saveChanges: _saveChanges,
		serverAddressSpaceSelectNode: _serverAddressSpaceSelectNode,
		serverEndpointChanged: _serverEndpointChanged,
		browseToNode: _browseToNode,
		stack_bottomright_opcuaeditor: _stack_bottomright_opcuaeditor
	};
}();

$( document ).ready( function()
{

	// manually hookup to the submit the form to make sure we pass all the entries from the table
	$( '#opcUaBrowserForm' ).submit( function()
	{
		var action = this.action;
		var method = this.method;
		FMOpcUaBrowser.saveChanges( action, method );

		// This section will get the selected row for the browse path and Node columns
		// in order to update the columns based on the values in the tag modal dialog.
		var selectedPointTagIndex = $( '#PointTagSelectedIndex' ).val();
		var opcUaBrowsePathSelectedRowId = 'Tags_' + selectedPointTagIndex + '__OpcUaBrowsePath';
		var opcUaNodeSelectedRowId = 'Tags_' + selectedPointTagIndex + '__OpcUaNodeId';

		var opcUaBrowsePathObj = document.getElementById( opcUaBrowsePathSelectedRowId );
		var opcUaNodeObj = document.getElementById( opcUaNodeSelectedRowId );

		opcUaBrowsePathObj.value = $( '#PointTagOpcUaBrowsePathTextBox' ).val();
		opcUaNodeObj.value = $( '#PointTagOpcUaNodeIdTextBox' ).val();
		opcUaBrowsePathObj.title = $( '#PointTagOpcUaBrowsePathTextBox' ).val();
		opcUaNodeObj.title = $( '#PointTagOpcUaNodeIdTextBox' ).val();

		return false;
	} );

	$( '#serverAddressSpace' ).jstree( {
		'core': {
			'check_callback': true
		}
	} );


	// must browse prior to binding to select_node ajax calls must by synchronous
	var browsePath = $( '#PointTagOpcUaBrowsePathTextBox' ).val();
	if (browsePath !== '') {
		FMOpcUaBrowser.browseToNode('#', browsePath);

		setTimeout(function() {
			$('#serverAddressSpace').animate({
				scrollTop: $(".jstree-clicked").position().top
			}, 1000);
		}, 1000);
	}
	else {
		$("#opcTagInfoPanel").removeClass("active");
		$("#opcServerInfoPanel").removeClass("active").addClass("active");
	}

	$( '#serverAddressSpace' ).on( 'select_node.jstree', function( e, data )
	{
		FMOpcUaBrowser.serverAddressSpaceSelectNode( e, data );
	} );


	FMErrorAndExceptionHandling.CloseNotifications();
} );
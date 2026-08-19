if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	debugger;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}


// create a class with helper functions for the strap table editor
var FMPointCommandStatusEditor = function()
{
	var _valuesChanged = false;
	var _processingEndEditRow = false;
	var _numEntriesPerList = 255;
	var _stack_bottomright_pointcommandstatus = { "dir1": 'up', "dir2": 'left', "firstpos1": 135, "firstpos2": 25, "context": $('#PointCommandStatusEditorScreen') };
	var _deletedPointCommandStatusLists = [];
	var _processingOnBlur = false;

	var _newGuid = function () {
	function s4() {
		return Math.floor((1 +Math.random()) * 0x10000)
		  .toString(16)
		  .substring(1);
		}
	return s4() +s4() + '-' + s4() + '-' +s4() + '-' +s4() + '-' +s4() + s4() +s4();
	}

	var _isStandardTankDefault = function () {
		if($('#PointTemplateId').val() === 'Standard Tank') {
			var tableDescription = $('#tabList li.active .tab-name').text();

			return (tableDescription === 'Enraf Model 854' || tableDescription === 'Varec Model 6000') ? true : false;
		}

		return false;
	}

	var _enableChangesToTab = function () {
		var enableChanges = false;
		var activeTab = $('#tabList li.active');
		var tabPane = $('.tab-pane.active');

		if (!_isStandardTankDefault()) {
			var tabCount = $('#tabList li.tab-table').length;
			enableChanges = (tabCount > 0) ? true : false;
		}

		if(!enableChanges){
			$('#PCEDeleteList').addClass("disabled").prop('disabled', true);
			tabPane.find('.addCommandStatusEntryButton').addClass("disabled").prop('disabled', true);
		}
		else{
			$('#PCEDeleteList').removeClass("disabled").prop('disabled', false);
			tabPane.find('.addCommandStatusEntryButton').removeClass("disabled").prop('disabled', false);
		}

		return enableChanges;
	}

	var _saveChanges = function () {
		// we need to make sure we are done processing the onBlur event which validates entries
		if ( FMPointCommandStatusEditor.processingOnBlur === true )
		{
			setTimeout( function()
			{
				FMPointCommandStatusEditor.saveChanges();
			}, 100);
			return;
		}
		else
		{
			// if the edit fields are displayed is because there is an error and we cannot save
			var tabContents = $('#tabList li.active').find('a').attr('data-target');
			var editFields = $(tabContents).find('#KeyEdit');
			if ( editFields.length > 0 )
			{
				return;
			}
		}
		var url = $('#urlSavePointCommandStatus').val();
		var token = $('#PointCommandStatusForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMPointCommandStatusEditor.stack_bottomright_pointcommandstatus, width: '250px' };
		// remove previous notifications
		PNotify.removeStack(FMPointCommandStatusEditor.stack_bottomright_pointcommandstatus);

		$("#PointCommandStatusEditorScreen").append('<div id="savePointCommandStatusLoader" class="LoadingAnimation transparent"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>');

		$.ajax({
			url: url,
			cache: false,
			headers: headers,
			type: 'post',
			dataType: 'json',
			data: (function ()
			{

				var pointCommandStatusList = [];
				
				$('#tabList li.tab-table').each(function () {
					var tabContents = $(this).find('a').attr('data-target');
					var pointCommandStatusEntries = [];
					var commandStatusListID = $(this).find('.tab-name').text();
					var commandStatusListGuid = $(this).find('.tab-guid').val();
					$(tabContents + ' .table tr').each(function () {
						if ($(this).find('td.pointCommandStatusKeyColumn').length > 0) {
							if ( $( this ).find( 'td.pointCommandStatusKeyColumn' ).text().trim() !== "" && $( this ).find( 'td.pointCommandStatusValueColumn' ).text().trim() !== "" )
							{
								pointCommandStatusEntries.push( {
									KeyEntry: $( this ).find( 'td.pointCommandStatusKeyColumn' ).text().trim(),
									ValueEntry: $( this ).find( 'td.pointCommandStatusValueColumn' ).text().trim()
								} );
							}
						}
					});
					pointCommandStatusList.push({ CommandStatusListID: commandStatusListID, CommandStatusListGuid: commandStatusListGuid, PointCommandStatusEntries: pointCommandStatusEntries });
				});

				return { editorEntries: JSON.stringify( pointCommandStatusList), pointTemplateGuid: $( '#PointCommandStatusForm #PointTemplateGuid' ).val(), deletedPointCommandStatusLists: FMPointCommandStatusEditor.deletedPointCommandStatusLists } ;
			})(),
			success: function (result) {
				FMErrorAndExceptionHandling.HandleMessages(result,
					 function (data, inError) {
					 	if (!inError) {
					 		FMPointCommandStatusEditor.valuesChanged = false;
					 		FMPointCommandStatusEditor.deletedPointCommandStatusLists =[];
					 	}
					 },
					 messageAttributes);

				$( "#savePointCommandStatusLoader" ).remove();
			},
			error:
				 function (request, status, error) {
				 	$("#savePointCommandStatusLoader").remove();
				 	FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				 }
		});
	};

	var _renameTab = function (control) {
		if (!FMPointCommandStatusEditor.enableChangesToTab()) {
			return;
		}

		var firstChildren = $(control).children().first();
		// the <a>nchor in the tab control either contains a label (to display the name) or an input box (to change the name)
		if (firstChildren.is('label')) {
			var oldName = firstChildren.text();
			$(control).find(".tab-name").remove();
			$(control).prepend("<input id='" +$(control).attr('data-target').replace('#', '') + "' type='text' class='tab-rename' value='" + oldName + "' onblur='FMPointCommandStatusEditor.endRenameTab( this );' onkeyup='if(event.keyCode == 13){ FMPointCommandStatusEditor.endRenameTab( this ); return true;}'> </input> ");
			$(control).children().first().focus().val(oldName);

			// terminate the edit if user presses the Esc key
			$('#' + $(control).attr('data-target').replace('#', '')).on('keydown', function (e)
			{
				if (e.keyCode === 27 || e.keyCode === 13 || e.keyCode === 9 ) { // escape key maps to keycode `27`
					FMPointCommandStatusEditor.endRenameTab(this);
					return false;
				}
				return true;
			});
		}
	}; 

	// Finish renaming tab. When we lose focus on the tab we want to finish the renaming.
	var _endRenameTab = function (control) {
		var name = $(control).val();

		// do not allow empty tab names
		if (name === "")
		{
			return false;
		}

		var parent = $(control).parent();
		$(control).remove();
		$(parent).find('.old-tab-name').val(name);
		$(parent).prepend('<label class="tab-name">' + name + '</label>');

		// refresh the scroll buttons for the tabs (do it async to give time to display the contents of the tab)
		$('#mainTab').scrollingTabs('refresh');
	};

	var _addRow = function () {
		var tabContents = $('#tabList li.active').find('a').attr('data-target');
		var PCEPointCommandStatusList = $(tabContents + ' .table').DataTable();
		var rowIndex = PCEPointCommandStatusList.rows().count();

		PCEPointCommandStatusList.row.add({
			"0": rowIndex + 1,
			"1": '',
			"2": ''
		});

		PCEPointCommandStatusList.columns.adjust().draw();

		FMPointCommandStatusEditor.editRow(PCEPointCommandStatusList.row(rowIndex).node(), true);
	};

	var _onblurEditRow = function (input, add) {
		if ($('#KeyEdit').is(':focus')) {
			FMPointCommandStatusEditor.processingOnBlur = false;
			return;
		}
		if ($('#ValueEdit').is(':focus')) {
			FMPointCommandStatusEditor.processingOnBlur = false;
			return;
		}
		
		if (!FMPointCommandStatusEditor.processingEndEditRow) {
			FMPointCommandStatusEditor.endEditRow(null, input, add);
		}
		FMPointCommandStatusEditor.processingOnBlur = false;
	};

	var _editRow = function (row, add) {
		if (!FMPointCommandStatusEditor.enableChangesToTab()) {
			return;
		}

		$('#PEMPESavePropertyScreen').prop('disabled', true);
		var tabContents = $('#tabList li.active').find('a').attr('data-target');
		var PCEPointCommandStatusList = $(tabContents + ' .table').DataTable();
		var data = PCEPointCommandStatusList.row(row).data();
		var cell = $('>td', row);

		// Empty row, handle as add
		if (cell.length < 3) {
			FMPointCommandStatusEditor.addRow();
			return;
		}
		// FJM Note: In chrome the blur event seems to call some event hadlers in a different sequence than in IE and it may result on the next field not being in focus
		//           when the onblur event handler fires.  Added a timeout (asynchronous) to let the other event finish before evaluating the event.
		$(cell[1]).html('<input id="KeyEdit" type="text" maxlength="30" value="' + data[1] + '" onblur="var that = this; FMPointCommandStatusEditor.processingOnBlur = true; setTimeout( function() { FMPointCommandStatusEditor.onblurEditRow(that, ' + add + ');}, 100 );" onkeyup="if(event.keyCode == 13){ FMPointCommandStatusEditor.endEditRow( event, this, ' + add + ');}">');
		$(cell[2]).html('<input id="ValueEdit" type="number" value="' + data[2] + '"  onblur="var that = this; FMPointCommandStatusEditor.processingOnBlur = true; setTimeout( function() { FMPointCommandStatusEditor.onblurEditRow(that, ' + add + ');}, 100 );" onkeypress="return event.charCode >= 48 && event.charCode <= 57" onkeyup="if(event.keyCode == 13){ FMPointCommandStatusEditor.endEditRow( event, this, ' + add + ');}" onkeydown="if(event.keyCode == 9){ FMPointCommandStatusEditor.endEditRow( event, this, ' + add + ');}">');

		// terminate the edit if user presses the Esc key
		$('#KeyEdit').on('keyup', function (e) {
			if (e.keyCode === 27 ) { // escape key maps to keycode `27`
				FMPointCommandStatusEditor.cancelEditRow(this, add);
				return false;
			}
			return true;
		});

		$('#ValueEdit').on('keyup', function (e) {
			if (e.keyCode === 27) { // escape key maps to keycode `27`
				FMPointCommandStatusEditor.cancelEditRow(this, add);
				return false;
			}
			return true;
		});

		setTimeout( function()
		{
			$( '#KeyEdit' ).focus();
		}, 100 );
	};

	var _endEditRow = function (event, input, add) {
		if (event && event.shiftKey && event.keyCode === 9) {
			return;
		}

		// only run this method once, don't call it again if we are already in the middle of processing
		if (FMPointCommandStatusEditor.processingEndEditRow === true) {
			return;
		}
		FMPointCommandStatusEditor.processingEndEditRow = true;
		
		var tabContents = $('#tabList li.active').find('a').attr('data-target');
		var PCEPointCommandStatusList = $(tabContents + ' .table').DataTable();
		var row = $(tabContents).find('#' + $(input).attr('id')).parent().parent();
		var data = PCEPointCommandStatusList.row(row).data();
		var cell = $('>td', row);

		if ( $( '#KeyEdit' ).length > 0 && $( '#KeyEdit' ).val() != '' && $( '#ValueEdit' ).val() != '' )
		{
			// Test for duplicate key
			var firstMatchAssending = PCEPointCommandStatusList.rows().data().filter( function( value, index, instance )
			{
				return value[0] !== data[0] && value[1] === $( '#KeyEdit' ).val() ? true : false;
			} )[0];
			if ( firstMatchAssending )
			{
				FMLayout.Alert( $( '#PCEDuplicateKey' ).val(), 'Error', function()
				{
					FMPointCommandStatusEditor.processingEndEditRow = false;
					setTimeout( function()
					{
						input.focus();
					}, 100 );
				} );
				return;
			}

			var firstMatchDescending = PCEPointCommandStatusList.rows().data().filter( function( value, index, instance )
			{
				return value[0] !== data[0] && value[1] === $( '#KeyEdit' ).val() ? true : false;
			} )[0];
			if ( firstMatchDescending )
			{
				FMLayout.Alert( $( '#PCEDuplicateKey' ).val(), 'Error', function()
				{
					FMPointCommandStatusEditor.processingEndEditRow = false;
					setTimeout( function()
					{
						input.focus();
					}, 100 );
				} );
				return;
			}

			// Test for duplicate value
			var firstMatchAssending = PCEPointCommandStatusList.rows().data().filter( function( value, index, instance )
			{
				return value[0] !== data[0] && value[2] === $( '#ValueEdit' ).val() ? true : false;
			} )[0];
			if ( firstMatchAssending )
			{
				FMLayout.Alert( $( '#PCEDuplicateValue' ).val(), 'Error', function()
				{
					FMPointCommandStatusEditor.processingEndEditRow = false;
					setTimeout( function()
					{
						input.focus();
					}, 100 );
				} );
				return;
			}

			var firstMatchDescending = PCEPointCommandStatusList.rows().data().filter( function( value, index, instance )
			{
				return value[0] !== data[0] && value[2] === $( '#ValueEdit' ).val() ? true : false;
			} )[0];
			if ( firstMatchDescending )
			{
				FMLayout.Alert( $( '#PCEDuplicateValue' ).val(), 'Error', function()
				{
					FMPointCommandStatusEditor.processingEndEditRow = false;
					setTimeout( function()
					{
						input.focus();
					}, 100 );
				} );
				return;
			}

			data[1] = $( '#KeyEdit' ).val();
			data[2] = $( '#ValueEdit' ).val();
			cell[1].innerHTML = data[1];
			cell[2].innerHTML = data[2];

			FMPointCommandStatusEditor.valuesChanged = true;

			if ( !add )
			{
				PCEPointCommandStatusList.row( row ).invalidate();
			}
			PCEPointCommandStatusList.order( PCEPointCommandStatusList.order() );
			PCEPointCommandStatusList.columns.adjust().draw();

			PCEPointCommandStatusList.column( 0, { search: 'applied', order: 'applied' } ).nodes().each( function( cell, i )
			{
				cell.innerHTML = i + 1;
			} );

			$( '#PEMPESavePropertyScreen' ).removeAttr( 'disabled' );

			if ( event && event.keyCode === 9 && add && PCEPointCommandStatusList.rows().count() < FMPointCommandStatusEditor.numEntriesPerList )
			{
				FMPointCommandStatusEditor.processingEndEditRow = false;
				FMPointCommandStatusEditor.addRow();
				return;
			}

			// if we have reach the limit of entries in the list we need to disable the add button
			if ( PCEPointCommandStatusList.rows().count() === FMPointCommandStatusEditor.numEntriesPerList )
			{
				$( tabContents + ' .addCommandStatusEntryButton' ).prop( "disabled", true );
			}
		}
		// if one field has a value but the other not then we cannot end the row edit
		else if ($('#KeyEdit').length > 0 && ( $('#KeyEdit').val() != '' || $('#ValueEdit').val() != ''))
		{
			if ( $( '#KeyEdit' ).val() == '' )
			{
				$( '#KeyEdit' ).focus();
			}
			if ($('#ValueEdit').val() == '') {
				$('#ValueEdit').focus();
			}
		}
		else {
			FMPointCommandStatusEditor.cancelEditRow(input, add);
		}
		FMPointCommandStatusEditor.processingEndEditRow = false;
	};

	var _cancelEditRow = function (input, add) {
		var tabContents = $('#tabList li.active').find('a').attr('data-target');
		var PCEPointCommandStatusList = $(tabContents + ' .table').DataTable();
		var row = $(input).parent().parent();
		if ( row.length > 0 )
		{
			var data = PCEPointCommandStatusList.row( row ).data();
			var cell = $( '>td', row );
			cell[1].innerHTML = data[1];
			cell[2].innerHTML = data[2];
		}

		if (add )
		{
			PCEPointCommandStatusList.row(row).remove();
			PCEPointCommandStatusList.columns.adjust().draw();
		}

		var tableIndex = tabContents.substring(tabContents.indexOf('_') + 1);
		$('#addStrapEntryButton_' + tableIndex).removeAttr('disabled');

		$('#PointCommandStatusEditorTable_' + tableIndex + ' tbody').on('dblclick', 'tr', function (e) {
			if (e.target.nodeName === 'INPUT') {
				return;
			}

			FMPointCommandStatusEditor.editRow(this, false);
		});

		$('#PEMPESavePropertyScreen').removeAttr('disabled');
	};

	var _addTable = function()	{
		var newGuid = FMPointCommandStatusEditor.newGuid();
		var newId = 'PointCommandStatus_' + newGuid;

		$('.tab-add-table').before('<li class="tab-table"><a data-target="#' + newId + '" data-toggle="tab" onclick="FMPointCommandStatusEditor.onTabChange(this);" ondblclick="FMPointCommandStatusEditor.renameTab(this);"><label class="tab-name">' + 'New Point Command-Status' + '</label><input class="old-tab-name" type="hidden" value="' + 'New Point Command-Status' + '"><input class="tab-guid" type="hidden" value="' + newGuid + '"></a></li>');

		var numberTextDd = $("#NumberTextDD").val();
		var keyTextDd = $("#KeyTextDD").val();
		var valueTextDd = $("#ValueTextDD").val();
		var addRowDd = $("#AddRowDD").val();
		var deleteRowDd = $("#DeleteRowDD").val();

		var pointCommandStatusTable = '<div class="pointCommandStatus col-xs-12 col-sm-12 col-md-12 col-lg-12">' +
			'<table id="PointCommandStatusEditorTable_' + newGuid + '" class="table table-striped table-bordered hover" style="width:100%">' +
			'<thead class="">' +
			'<tr>' +
			'<th style="" class="text-center">' + numberTextDd + '</th>' +
			'<th class="text-center">' + keyTextDd + '</th>' +
			'<th class="text-center">' + valueTextDd + '</th>' +
			'</tr>' +
			'</thead>' +
			'<tbody id="PointCommandStatusEditorTableBody">' +
			'</tbody>' +
			'</table>' +
			'<br />' +
			'<div class="" role="group">' +
			'	<button id="addCommandStatusEntryButton_' + newGuid + '" name="addCommandStatusEntryButton_' + newGuid + '" type="button" value="addCommandStatusEntryButton" accesskey="A" class="addCommandStatusEntryButton formfieldtitle tabPushButton" title="Add New Command-Status Entry" style="width: 100px;">' + addRowDd +'</button>' +
			'	<button id="deleteCommandStatusEntriesButton_' + newGuid + '" name="deleteCommandStatusEntriesButton_' + newGuid + '" type="button" value="deleteCommandStatusEntriesButton" accesskey="l" class="deleteCommandStatusEntriesButton formfieldtitle tabPushButton" title="Delete Command-Status Entries" disabled="disabled" style="width: 100px;">' + deleteRowDd + '</button>' +
			'</div>' +
			'</div>';

		var tabPanel = $( '<div class="tab-pane" id="' + newId + '">' + pointCommandStatusTable + '</div>' ).appendTo( '.tab-content' );

		var PCEPointCommandStatusList = $( '#PointCommandStatusEditorTable_' + newGuid ).DataTable(
			{
				"retrieve": true,
				"select": true,
				"ordering": false,
				"scrollY": '300px',
				"sScrollX": '100%',
				"sScrollXInner": '100%',
				"scrollCollapse": false,
				"paging": false,
				"autoWidth": true,
				"columnDefs": [
					{ "targets": [0], "name": 'Number', "orderable": false, className: 'pointCommandStatusNumberColumn text-center' },
					{ "targets": [1], "name": 'Key', "orderable": false, className: 'pointCommandStatusKeyColumn text-center' },
					{ "targets": [2], "name": 'Value', "orderable": false, className: 'pointCommandStatusValueColumn text-right' }
				],
				"dom": 'rt',
				"fnInitComplete": function()
				{
					// custom scroll bars
					$( this ).parent()
						.niceScroll( {
							cursorwidth: '10px',
							autohidemode: false,
							cursorcolor: '#486899',
							background: 'rgb(240, 240, 240)',
							horizrailenabled: false
						} );
				}
			} );


		PCEPointCommandStatusList.draw();

		FMPointCommandStatusEditor.initializeTabContent( newGuid );

		$( 'a[data-target="#' + newId + '"]' ).tab( 'show' );
		PCEPointCommandStatusList.columns.adjust().draw();

		// refresh the scroll buttons for the tabs (do it async to give time to display the contents of the tab)
		$('#mainTab').scrollingTabs('refresh');

		FMPointCommandStatusEditor.enableChangesToTab();

		return newId;
	};

	var _enableAddTab = function (tabCount) {
		if (tabCount >= 25) {
			$('#tabList li.tab-add-table').addClass('disabled');
			$('#addTable').click(function (e) {
				e.preventDefault();;
			});
		}
		else {
			$('#tabList li.tab-add-table').removeClass('disabled');
			$('#addTable').click(function () {
				FMPointCommandStatusEditor.addTable();
			});
		}
	};
	 
	var _initializeTabContent = function( tabId ) {
		// if we have reach the limit of entries in the list we need to disable the add button
		var tabContents = "#PointCommandStatus_" + tabId;
		var PCEPointCommandStatusList = $(tabContents + ' .table').DataTable();
		if (PCEPointCommandStatusList.rows().count() === FMPointCommandStatusEditor.numEntriesPerList) {
			$(tabContents + ' .addCommandStatusEntryButton').prop("disabled", true);
		}

		$('#PointCommandStatusEditorTable_' + tabId + ' tbody').keyup(function (e) {
			if (e.keyCode === 46) {
				var tabContents = $('#tabList li.active').find('a').attr('data-target');
				var PCEPointCommandStatusList = $(tabContents + ' .table').DataTable();
				if (!_isStandardTankDefault() && $('#KeyEdit').length === 0 && PCEPointCommandStatusList.rows('.selected').count() > 0) {
					FMPointCommandStatusEditor.deleteRows();
				}
			}
		});

		// enable delete button when 1 or more rows and selected rows
		$('#PointCommandStatusEditorTable_' + tabId + ' tbody').on('click', 'tr', function () {
			setTimeout(function () {
				var tabContents = $('#tabList li.active').find('a').attr('data-target');
				var PCEPointCommandStatusList = $(tabContents + ' .table').DataTable();
				var selectedCount = PCEPointCommandStatusList.rows('.selected').count();
				$(tabContents + ' .deleteCommandStatusEntriesButton').prop('disabled', (selectedCount === 0 || _isStandardTankDefault()) ? true : false);
			}, 500);
		});

		// click on the delete button
		$('#deleteCommandStatusEntriesButton_' + tabId).on('click', function () {
			FMPointCommandStatusEditor.deleteRows();
		});

		// double click to edit a row 
		$('#PointCommandStatusEditorTable_' + tabId + ' tbody').on('dblclick', 'tr', function (e) {
			if(e.target.nodeName === 'INPUT') {
				return;
			}

			FMPointCommandStatusEditor.editRow(this, false);
		});


		// click to add a strap table entry row
		$('#addCommandStatusEntryButton_' + tabId).on('click', function () {
			FMPointCommandStatusEditor.addRow();
		});

		var buttonObj = $('#addCommandStatusEntryButton_' + tabId);
		var ampersandIndex = buttonObj.text().indexOf('&');
		var hotKey;
		if (ampersandIndex !== -1)
		{
			hotKey = buttonObj.text().charAt(ampersandIndex + 1);
			buttonObj.html(buttonObj.text().replace('&' + hotKey, '<span style="text-decoration: underline;">' + hotKey + '</span>'));
			buttonObj.attr('accesskey', hotKey);
		}

		buttonObj = $('#deleteCommandStatusEntriesButton_' + tabId);
		ampersandIndex = buttonObj.text().indexOf('&');
		if (ampersandIndex !== -1)
		{
			hotKey = buttonObj.text().charAt(ampersandIndex + 1);
			buttonObj.html(buttonObj.text().replace('&' + hotKey, '<span style="text-decoration: underline;">' + hotKey + '</span>'));
			buttonObj.attr('accesskey', hotKey);
		}
	}

	_onTabChange = function( )	{
		// since the new tab has not been rendered yet we want to processs after the rendering
		setTimeout( function()
		{
			$('#mainTab').scrollingTabs('refresh');
			var tabContents = $( '#tabList li.active' ).find( 'a' ).attr( 'data-target' );
			var PCEPointCommandStatusList = $( tabContents + ' .table' ).DataTable();
			PCEPointCommandStatusList.columns.adjust().draw();
			FMPointCommandStatusEditor.enableChangesToTab();
		}, 1);
	}

	var _deleteRows = function ()
	{
		FMLayout.Confirm($('#SEDeleteConfirmation').val(),
			 null,
			 function () {
			 	var tabContents = $('#tabList li.active').find('a').attr('data-target');
			 	var PCEPointCommandStatusList = $(tabContents + ' .table').DataTable();
			 	PCEPointCommandStatusList.rows('.selected').remove().draw(false);
			 	FMPointCommandStatusEditor.valuesChanged = true;
			 	PCEPointCommandStatusList.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
			 		cell.innerHTML = i + 1;
			 	});

			 	// if we have reach the limit of entries in the list we need to disable the add button
			 	if (PCEPointCommandStatusList.rows().count() === FMPointCommandStatusEditor.numEntriesPerList) {
			 		$(tabContents + ' .addCommandStatusEntryButton').prop("disabled", false);
			 	}

			 });
	};

    var _deleteTable = function()
    {
        // Check if table is selected
        var activeTab = $( '#tabList li.active' );
        if (!activeTab || activeTab.hasClass('tab-add-table'))
        {
            return;
        }
		  
    	// find the previous tab so we can set it as active
        var newActiveTab = activeTab.prev();
        if ( newActiveTab.length === 0 )
        {
        		newActiveTab = activeTab.next();
        }

		 // we need to find the contents so we can delete them too
        var tabContents = $(activeTab).find('a').attr('data-target');

        var tableDescription = $('#tabList li.active .tab-name').text();
        var tableGuid = $('#tabList li.active .tab-guid').val();

        FMLayout.Confirm( $( '#SEDeleteTableConfirmation' ).val() + ' ' + tableDescription + '?',
            null,
            function()
            {
	            $(tabContents).find('.dataTable').DataTable().destroy();
	            $(tabContents).remove();
	            $(activeTab).remove();
	            FMPointCommandStatusEditor.valuesChanged = true;
	            newActiveTab.find('a').tab('show');
	            FMPointCommandStatusEditor.onTabChange();
	            FMPointCommandStatusEditor.deletedPointCommandStatusLists.push(tableGuid);
            } );
    };
	return {
		valuesChanged: _valuesChanged,
		saveChanges: _saveChanges,
		renameTab: _renameTab,
		endRenameTab: _endRenameTab,
		addRow: _addRow,
		editRow: _editRow,
		endEditRow: _endEditRow,
		cancelEditRow: _cancelEditRow,
		onblurEditRow: _onblurEditRow,
		processingOnBlur: _processingOnBlur,
		addTable: _addTable,
		enableAddTab: _enableAddTab,
		initializeTabContent: _initializeTabContent,
		onTabChange: _onTabChange,
		deleteRows: _deleteRows,
		deleteTable: _deleteTable,
		processingEndEditRow: _processingEndEditRow, 
		stack_bottomright_pointcommandstatus: _stack_bottomright_pointcommandstatus,
		newGuid: _newGuid,
		numEntriesPerList: _numEntriesPerList,
		deletedPointCommandStatusLists: _deletedPointCommandStatusLists,
		enableChangesToTab: _enableChangesToTab
	};
}();

// manually hookup to the submit the form to make sure we pass all the entries from the table
$(function () {
	$('#PointCommandStatusForm').submit(function () {
		var action = this.action;
		var method = this.method;
		FMPointCommandStatusEditor.saveChanges();

		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	});
});


//--------------------------------------- RUN after page has been loaded but before render -----------------------------

$(document).ready(function () {

	$('#mainTab').scrollingTabs({ scrollToTabEdge: true });

	// make the first tab active
	var activeTab = 0;
	$('a[data-target="#PointCommandStatus_' + activeTab + '"]').tab('show');

	var tabCount = $('#tabList li.tab-table').length;

	for ( var pointCommandStatusListIndex = 0; pointCommandStatusListIndex < tabCount; pointCommandStatusListIndex++ )
	{
		var PCEPointCommandStatusList = $('#PointCommandStatusEditorTable_' + pointCommandStatusListIndex).DataTable(
			{
				"retrieve": true,
				"select": true,
				"ordering": false,
				"scrollY": '300px',
				"sScrollX": '100%',
				"sScrollXInner": '100%',
				"scrollCollapse": false,
				"paging": false,
				"autoWidth": true,
				"columnDefs": [
					{ "targets": [0], "name": 'Number', "orderable": false, className: 'pointCommandStatusNumberColumn text-center' },
					{ "targets": [1], "name": 'Key', "orderable": false, className: 'pointCommandStatusKeyColumn text-center' },
					{ "targets": [2], "name": 'Value', "orderable": false, className: 'pointCommandStatusValueColumn text-right' }
				],
				"dom": 'rt',
				"fnInitComplete": function()
				{
					// custom scroll bars
					$( this ).parent()
						.niceScroll( {
							cursorwidth: '10px',
							autohidemode: false,
							cursorcolor: '#486899',
							background: 'rgb(240, 240, 240)',
							horizrailenabled: false
						} );
				}
			} );


		PCEPointCommandStatusList.draw();
		PCEPointCommandStatusList.columns.adjust().draw();
		FMPointCommandStatusEditor.initializeTabContent(pointCommandStatusListIndex);

	}


	// adjust the column headers for the active tab
	setTimeout(function () {
		var PCEPointCommandStatusList = $('#PointCommandStatusEditorTable_' + activeTab).DataTable();
		PCEPointCommandStatusList.columns.adjust().draw();
	}, 100);

	// Add Point Command Status List Delete button to footer
	if ( $( '#ModifyRight' ).val() == 'True' )
	{
		var leftFooter = $( '#PointCommandStatusEditorScreen .modal-footer .btn-toolbar .pull-left' );
		leftFooter.html( '<button id=\"PCEDeleteList\" type=\"button\" class=\"formfieldtitle pushButton\" style=\"width: 100px;margin-left: 5px;\">' + $( '#PCEDeleteListText' ).val() + '</button>' );
	}

	var buttonObj = $('#PCEDeleteList');   //bds
	var ampersandIndex = buttonObj.text().indexOf('&');
	var hotKey;
	if (ampersandIndex !== -1)
	{
	    hotKey = buttonObj.text().charAt(ampersandIndex + 1);
	    buttonObj.html(buttonObj.text().replace('&' + hotKey, '<span style="text-decoration: underline;">' + hotKey + '</span>'));
	    buttonObj.attr('accesskey', hotKey);
	}

	// Add alignment for the close button in the footer
	$('#PEMPEClosePropertyScreen').css('marginRight', '42px');

	FMPointCommandStatusEditor.enableChangesToTab();

	FMPointCommandStatusEditor.enableAddTab(tabCount);

	$('#PCEDeleteList').on('click', function () {
		FMPointCommandStatusEditor.deleteTable();
	});

	FMErrorAndExceptionHandling.CloseNotifications();

});
// create a class with helper functions for the strap table editor
if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	debugger;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

var FMDeviceAlarmMapsEditor = function () {
	var _valuesChanged = false;
	var _processingEndEditRow = false;
	var _numEntriesPerMap = 32;
	var _stack_bottomright_DeviceAlarmMaps = { "dir1": 'up', "dir2": 'left', "firstpos1": 135, "firstpos2": 25, "context": $('#DeviceAlarmMapsEditorScreen') };
	var _deletedDeviceAlarmMaps = [];
	var _processingOnBlur = false;
	var _bitMaskSelectOpen = false;
	var _alarmPrioritySelectOpen = false;
	var _add = false;

	var _newGuid = function () {
		function s4() {
			return Math.floor((1 + Math.random()) * 0x10000)
			  .toString(16)
			  .substring(1);
		}
		return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
	}

	var _isStandardTankDefault = function (tabName) {
		if ($('#PointTemplateId').val() === 'Standard Tank') {
			var tableDescription = tabName;
			if (tableDescription === undefined)
				tableDescription = $('#tabList li.active .tab-name').text();

			tableDescription = tableDescription.trim();

			if (tableDescription === '8810 RTU - Tank Status')
				return true;
			else if (tableDescription === '8130 RTU')
				return true;
			else if (tableDescription === '8810 RTU - Module Status')
				return true;
			else if (tableDescription === '8810 RTU - Channel Status')
				return true;
			else if (tableDescription === 'EN811 EN873 EN990 - Gauge Status')
				return true;
			else if (tableDescription === 'EN854 - Gauge Status')
				return true;
			else if (tableDescription === 'ATT4000 - Gauge Status')
				return true;
			else if (tableDescription === 'FTT29xx - Gauge Status')
				return true;
			else if (tableDescription === 'GSI2000 V1800 V1900 V6500 - Gauge Status')
				return true;

			return false;
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

		if (!enableChanges) {
			$('#DAMEDeleteMap').addClass("disabled").prop('disabled', true);
			tabPane.find('.addDeviceAlarmMapEntryButton').prop('disabled', true);
		}
		else {
			$('#DAMEDeleteMap').removeClass("disabled").prop('disabled', false);
			tabPane.find('.addDeviceAlarmMapEntryButton').prop('disabled', false);
		}

		return enableChanges;
	}

	var _saveChanges = function () {
		// we need to make sure we are done processing the onBlur event which validates entries
		if (FMDeviceAlarmMapsEditor.processingOnBlur === true) {
			setTimeout(function () {
				FMDeviceAlarmMapsEditor.saveChanges();
			}, 100);
			return;
		}
		else {
			// if the edit fields are displayed is because there is an error and we cannot save
			var tabContents = $('#tabList li.active').find('a').attr('data-target');
			var editFields = $(tabContents).find('#KeyEdit');
			if (editFields.length > 0) {
				return;
			}
		}
		var url = $('#urlSaveDeviceAlarmMaps').val();
		var token = $('#DeviceAlarmMapsForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMDeviceAlarmMapsEditor.stack_bottomright_DeviceAlarmMaps, width: '250px' };
		// remove previous notifications
		PNotify.removeStack(FMDeviceAlarmMapsEditor.stack_bottomright_DeviceAlarmMaps);

		$("#DeviceAlarmMapsEditorScreen").append('<div id="saveDeviceAlarmMapsLoader" class="LoadingAnimation transparent"> <img src=' + window.applicationRootName + '"/fmwebapp/images/loader_squares_120.gif"></div>');

		$.ajax({
			url: url,
			cache: false,
			headers: headers,
			type: 'post',
			dataType: 'json',
			data: (function () {

				var DeviceAlarmMaps = [];

				$('#tabList li.tab-table').each(function () {
					var tabContents = $(this).find('a').attr('data-target');
					var DeviceAlarmMapEntries = [];
					var deviceAlarmMapID = $(this).find('.tab-name').text();
					var deviceAlarmMapGuid = $(this).find('.tab-guid').val();
					var notAlarmText = $(tabContents).find('.not-alarm-text').val();
					if (notAlarmText === '') {
						notAlarmText = 'Normal';
						$(tabContents).find('.not-alarm-text').val(notAlarmText);
					}
					var alarmCategoryGuid = $(tabContents).find('.alarm-category').val();
					var normalUnacknowledgedPriorityGuid = $(tabContents).find('.normal-unacknowledged-priority').val();
					var textText = $('#DAMETextText').val();
					$(tabContents + ' .table tr').each(function () {
						if ($(this).find('td.DeviceAlarmMapNameColumn').length > 0) {
							if ($(this).find('td.DeviceAlarmMapNameColumn').text().trim() !== "") {
								var alarmPriorityText = $(this).find('td.DeviceAlarmMapPriorityColumn').text().trim();
								var selectedAlarmPriority = jQuery.grep(dameAlarmPriorityData, function (a) {
									return (a.id + textText) === alarmPriorityText;
								});
								if (selectedAlarmPriority.length > 0) {
									DeviceAlarmMapEntries.push({
										BitMask: $(this).find('td.DeviceAlarmMapBitMaskColumn').text().trim(),
										TestName: $(this).find('td.DeviceAlarmMapNameColumn').text().trim(),
										AlarmPriority: selectedAlarmPriority[0].identityGuid
									});
								}
							}
						}
					});
					DeviceAlarmMaps.push({ ID: deviceAlarmMapID, DeviceAlarmMapGuid: deviceAlarmMapGuid, NotAlarmText: notAlarmText, AlarmCategory: alarmCategoryGuid, NormalUnacknowledgedPriority: normalUnacknowledgedPriorityGuid, DeviceAlarmMapEntryList: DeviceAlarmMapEntries });
				});

				return { editorEntries: JSON.stringify(DeviceAlarmMaps), pointTemplateGuid: $('#DeviceAlarmMapsForm #PointTemplateGuid').val(), deletedDeviceAlarmMaps: FMDeviceAlarmMapsEditor._deletedDeviceAlarmMaps };
			})(),
			success: function (result) {
				FMErrorAndExceptionHandling.HandleMessages(result,
					 function (data, inError) {
					 	if (!inError) {
					 		FMDeviceAlarmMapsEditor.valuesChanged = false;
					 		FMDeviceAlarmMapsEditor.deletedDeviceAlarmMaps = [];
					 	}
					 },
					 messageAttributes);

				$("#saveDeviceAlarmMapsLoader").remove();
			},
			error:
				 function (request, status, error) {
				 	$("#saveDeviceAlarmMapsLoader").remove();
				 	FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				 }
		});
	};

	var _renameTab = function (control) {
		if (!FMDeviceAlarmMapsEditor.enableChangesToTab()) {
			return;
		}

		var firstChildren = $(control).children().first();
		// the <a>nchor in the tab control either contains a label (to display the name) or an input box (to change the name)
		if (firstChildren.is('label')) {
			var oldName = firstChildren.text();
			$(control).find(".tab-name").remove();
			$(control).prepend("<input id='" + $(control).attr('data-target').replace('#', '') + "' type='text' class='tab-rename' maxlength = '50' value='" + oldName + "' onblur='FMDeviceAlarmMapsEditor.endRenameTab( this );' onkeyup='if(event.keyCode == 13){ FMDeviceAlarmMapsEditor.endRenameTab( this ); return true;}'> </input> ");
			$(control).children().first().focus().val(oldName);

			// terminate the edit if user presses the Esc key
			$('#' + $(control).attr('data-target').replace('#', '')).on('keydown', function (e) {
				if (e.keyCode === 27 || e.keyCode === 13 || e.keyCode === 9) { // escape key maps to keycode `27`
					FMDeviceAlarmMapsEditor.endRenameTab(this);
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
		if (name === "") {
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
		var DeviceAlarmMap = $(tabContents + ' .table').DataTable();
		var rowIndex = DeviceAlarmMap.rows().count();

		var lowestUnusedBitMask = 1;

		DeviceAlarmMap.rows().every(function (index, element) {
			var data = this.data();
			if (parseInt(data[1], 16) == lowestUnusedBitMask) {
				lowestUnusedBitMask *= 2;
			}
		});

		var hexBitMask = lowestUnusedBitMask.toString(32);
		while (hexBitMask.length < 8) {
			hexBitMask = '0' + hexBitMask;
		}
		hexBitMask = '0x' + hexBitMask;

		DeviceAlarmMap.row.add({
			"0": rowIndex+1,
			"1": hexBitMask,
			"2": '',
			"3": ''
		});

		FMDeviceAlarmMapsEditor.add = true;

		FMDeviceAlarmMapsEditor.editRow(DeviceAlarmMap.row(rowIndex).node());
	};

	var _onblurEditRow = function () {

		if (FMDeviceAlarmMapsEditor.bitMaskSelectOpen === true) {
			FMDeviceAlarmMapsEditor.processingOnBlur = false;
			return;
		}

		if ($('#NameEdit').is(':focus')) {
			FMDeviceAlarmMapsEditor.processingOnBlur = false;
			return;
		}

		if (FMDeviceAlarmMapsEditor.alarmPrioritySelectOpen === true) {
			FMDeviceAlarmMapsEditor.processingOnBlur = false;
			return;
		}


		if (!FMDeviceAlarmMapsEditor.processingEndEditRow) {
			FMDeviceAlarmMapsEditor.endEditRow(null);
		}

		FMDeviceAlarmMapsEditor.processingOnBlur = false;
	};

	var _editRow = function (row) {
		if (!FMDeviceAlarmMapsEditor.enableChangesToTab()) {
			return;
		}

		$('#DAMESaveButton').prop('disabled', true);

		if ($('#NameEdit').length > 0) {
			$('#NameEdit').off('blur').off('keydown').blur();
		}

		var tabContents = $('#tabList li.active').find('a').attr('data-target');
		var DeviceAlarmMap = $(tabContents + ' .table').DataTable();
		var data = DeviceAlarmMap.row(row).data();
		var rowIndex = DeviceAlarmMap.row(row).index();
		var cell = $('>td', row);

		// FJM Note: In chrome the blur event seems to call some event hadlers in a different sequence than in IE and it may result on the next field not being in focus
		//           when the onblur event handler fires.  Added a timeout (asynchronous) to let the other event finish before evaluating the event.
		var bitMask = ['0x00000001', '0x00000002', '0x00000004', '0x00000008', '0x00000010', '0x00000020', '0x00000040', '0x00000080',
							'0x00000100', '0x00000200', '0x00000400', '0x00000800', '0x00001000', '0x00002000', '0x00004000', '0x00008000',
							'0x00010000', '0x00020000', '0x00040000', '0x00080000', '0x00100000', '0x00200000', '0x00400000', '0x00800000',
							'0x01000000', '0x02000000', '0x04000000', '0x08000000', '0x10000000', '0x20000000', '0x40000000', '0x80000000'];
		var deviceAlarmMapAlarmBitMask = '<select class="form-control normal-unacknowledged-priority" id="BitMask">';
		for (var i = 0; i < 32; i++) {
			if (parseInt(bitMask[i], 16) == data[1]) {
				deviceAlarmMapAlarmBitMask += '<option value="' + bitMask[i] + '" selected>' + bitMask[i] + '</option>';
			}
			else{
				deviceAlarmMapAlarmBitMask += '<option value="' + bitMask[i] + '">' + bitMask[i] + '</option>';
			}
		}
		deviceAlarmMapAlarmBitMask += '</select>';

		$(cell[0]).html(rowIndex + 1);

		$(cell[1]).html(deviceAlarmMapAlarmBitMask);

		var alarmPriorities = JSON.parse($('#DAMEAlarmPriorities').val());

		var deviceAlarmMapAlarmPriorities = '<select class="form-control alarm-priority" id="AlarmPriority">';
		var indexOfSpan = data[2].indexOf('<span')
		var alarmId = (indexOfSpan != -1) ? data[2].substring(0, indexOfSpan) : '';
		alarmPriorities.forEach(function (alarmPriority) {
			if (alarmPriority.Text === alarmId || alarmId === '') {
				alarmId = alarmPriority.Text;
				deviceAlarmMapAlarmPriorities += '<option value="' + alarmPriority.Value + '" selected>' + alarmPriority.Text + '</option>';
			}
			else {
				deviceAlarmMapAlarmPriorities += '<option value="' + alarmPriority.Value + '">' + alarmPriority.Text + '</option>';
			}
		});
		deviceAlarmMapAlarmPriorities += '</select>';

		$(cell[2]).html(deviceAlarmMapAlarmPriorities);

		$(cell[3]).html('<input id="NameEdit" class="form-control" type="text" value="' + data[3] + '">');

		DeviceAlarmMap.draw();

		if (FMDeviceAlarmMapsEditor.add) {
			$(tabContents + ' .dataTables_scrollBody').scrollTop($('.dataTables_scrollBody').height());
		}


		$('#BitMask').select2();

		$('#BitMask').on('select2:opening', function () {
			FMDeviceAlarmMapsEditor.bitMaskSelectOpen = true;
		});

		$('#BitMask').on('select2:closing', function () {
			FMDeviceAlarmMapsEditor.bitMaskSelectOpen = false;
		});


		$('#AlarmPriority').select2({
			templateSelection: function (selection) {
				if (selection.selected) {
					var selectedAlarmPriority = jQuery.grep(dameAlarmPriorityData, function (a) {
						return a.identityGuid === selection.id;
					});

					if (selectedAlarmPriority.length > 0) {
						return $.parseHTML(selection.text + selectedAlarmPriority[0].html);
					}
					return $.parseHTML(selection.text);
				}

			},
			templateResult: function (selection) {
				var selectedAlarmPriority = jQuery.grep(dameAlarmPriorityData, function (a) {
					return a.identityGuid === selection.id;
				});

				if (selectedAlarmPriority.length > 0) {
					return $.parseHTML(selection.text + selectedAlarmPriority[0].html);
				}
				return $.parseHTML(selection.text);
			}
		});

		$('#AlarmPriority').on('select2:opening', function () {
			FMDeviceAlarmMapsEditor.alarmPrioritySelectOpen = true;
		});

		$('#AlarmPriority').on('select2:closing', function () {
			FMDeviceAlarmMapsEditor.alarmPrioritySelectOpen = false;
		});

		$(tabContents + ' .addDeviceAlarmMapEntryButton').prop('disabled', true);

		$('#NameEdit').on('keydown', function (e) {
			if (e.keyCode === 9) { // tab key
				$(document).detach('keydown');
				FMDeviceAlarmMapsEditor.endEditRow(event);
				return false;
			}
			if (e.keyCode === 13) { // enter key
				$(document).detach('keydown');
				FMDeviceAlarmMapsEditor.endEditRow(event);
				return false;
			}

			return true;
		});

		$('#NameEdit').on('blur', function () {
			FMDeviceAlarmMapsEditor.processingOnBlur = true;
			FMDeviceAlarmMapsEditor.onblurEditRow(this)
		});

		$(document).on('keydown', function (e) {
			if (e.keyCode === 27) { // escape key
				$(document).detach('keydown');
				FMDeviceAlarmMapsEditor.cancelEditRow();
				e.stopImmediatePropagation();
				return false;
			}
			return true;
		});

	};

	var _endEditRow = function (event) {
		if (event && event.shiftKey && event.keyCode === 9) {
			return;
		}

		// only run this method once, don't call it again if we are already in the middle of processing
		if (FMDeviceAlarmMapsEditor.processingEndEditRow === true) {
			return;
		}
		FMDeviceAlarmMapsEditor.processingEndEditRow = true;


		var tabContents = $('#tabList li.active').find('a').attr('data-target');
		var DeviceAlarmMap = $(tabContents + ' .table').DataTable();
		var row = $(tabContents).find('#' + $('#NameEdit').attr('id')).parent().parent();
		var data = DeviceAlarmMap.row(row).data();
		var rowIndex = DeviceAlarmMap.row(row).index();
		var cell = $('>td', row);

		// Test for duplicate Bit Mask
		var firstMatchAssending = DeviceAlarmMap.rows().data().filter(function (value, index, instance) {
			return index !== rowIndex && value[1] === $('#BitMask').val() ? true : false;
		})[0];

		if (firstMatchAssending) {
			FMLayout.Alert($('#DAMEDuplicateBitMaskText').val(), 'Error', function () {
				FMDeviceAlarmMapsEditor.processingEndEditRow = false;
			});
			return;
		}

		var firstMatchDescending = DeviceAlarmMap.rows().data().filter(function (value, index, instance) {
			return index !== rowIndex && value[1] === $('#BitMask').val() ? true : false;
		})[0];

		if (firstMatchDescending) {
			FMLayout.Alert($('#DAMEDuplicateBitMaskText').val(), 'Error', function () {
				FMDeviceAlarmMapsEditor.processingEndEditRow = false;
			});
			return;
		}


		if ($('#NameEdit').length > 0 && $('#NameEdit').val() != '') {

			if ((/[^a-zA-Z0-9\ ]/.test($('#NameEdit').val()))) {
				FMLayout.Alert($("#DAMAlarmNameMusBeAlphaNumeric").val() + ' : ' + $('#NameEdit').val(), '', function () {
					$("#addAlarmTestTagName").focus();
					FMDeviceAlarmMapsEditor.processingEndEditRow = false;
				});
				return;
			}


			// Test for duplicate Name
			var firstMatchAssending = DeviceAlarmMap.rows().data().filter(function (value, index, instance) {
				return index !== rowIndex && value[3] === $('#NameEdit').val() ? true : false;
			})[0];

			if (firstMatchAssending) {
				FMLayout.Alert($('#DAMEDuplicateNameText').val(), 'Error', function () {
					FMDeviceAlarmMapsEditor.processingEndEditRow = false;
					setTimeout(function () {
						input.focus();
					}, 100);
				});
				return;
			}

			var firstMatchDescending = DeviceAlarmMap.rows().data().filter(function (value, index, instance) {
				return index !== rowIndex && value[3] === $('#NameEdit').val() ? true : false;
			})[0];

			if (firstMatchDescending) {
				FMLayout.Alert($('#DAMEDuplicateNameText').val(), 'Error', function () {
					FMDeviceAlarmMapsEditor.processingEndEditRow = false;
				});
				return;
			}

			data[1] = $('#BitMask').val();

			var alarmPriorityGuid = $('#AlarmPriority').val();
			var selectedAlarmPriority = jQuery.grep(dameAlarmPriorityData, function (a) {
				return a.identityGuid === alarmPriorityGuid;
			});
			if (selectedAlarmPriority.length > 0) {
				data[2] = selectedAlarmPriority[0].id + selectedAlarmPriority[0].html;
			}

			data[3] = $('#NameEdit').val();

			$('#NameEdit').off('blur').off('keydown').blur();

			cell[0].innerHTML = data[0];
			cell[1].innerHTML = data[1];
			cell[2].innerHTML = data[2];
			cell[3].innerHTML = data[3];

			FMDeviceAlarmMapsEditor.valuesChanged = true;

			if (!FMDeviceAlarmMapsEditor.add) {
				DeviceAlarmMap.row(row).invalidate();
			}

			$('#DAMESaveButton').prop('disabled', false);

			if (event && event.keyCode === 9 && FMDeviceAlarmMapsEditor.add && DeviceAlarmMap.rows().count() < FMDeviceAlarmMapsEditor.numEntriesPerMap) {
				FMDeviceAlarmMapsEditor.processingEndEditRow = false;
				FMDeviceAlarmMapsEditor.addRow();
				return;
			}

			// if we have reach the limit of entries in the list we need to disable the add button
			if (DeviceAlarmMap.rows().count() === FMDeviceAlarmMapsEditor.numEntriesPerMap) {
				$(tabContents + ' .addDeviceAlarmMapEntryButton').prop("disabled", true);
			}
			else {
				$(tabContents + ' .addDeviceAlarmMapEntryButton').prop("disabled", false);
			}
		}
			// if name field doesn't have a value we cannot end the row edit
		else if ($('#NameEdit').length > 0 && $('#NameEdit').val() == '') {
			$('#NameEdit').focus();
		}
		else {
			FMDeviceAlarmMapsEditor.cancelEditRow();
		}

		FMDeviceAlarmMapsEditor.processingEndEditRow = false;
	};

	var _cancelEditRow = function () {

		if ($('#NameEdit').length == 0) {
			return;
		}

		$('#NameEdit').off('blur').off('keydown').blur();
		

		var tabContents = $('#tabList li.active').find('a').attr('data-target');

		var DeviceAlarmMap = $(tabContents + ' .table').DataTable();
		var row = $('#NameEdit').parent().parent();

		if(row.index() == -1)	{
			return;
		}

		if (!FMDeviceAlarmMapsEditor.add && row.length > 0) {
			var data = DeviceAlarmMap.row(row).data();
			var cell = $('>td', row);
			cell[0].innerHtml = data[0];
			cell[1].innerHTML = data[1];
			cell[2].innerHTML = data[2];
			cell[3].innerHTML = data[3];
		}

		if (FMDeviceAlarmMapsEditor.add) {
			DeviceAlarmMap.row(row).remove();
		}

		DeviceAlarmMap.draw('page');
		$(tabContents + ' .addDeviceAlarmMapEntryButton').prop('disabled', false);
		var selectedCount = DeviceAlarmMap.rows('.selected').count();
		$(tabContents + ' .deleteDeviceAlarmMapEntriesButton').prop('disabled', (selectedCount === 0 || _isStandardTankDefault()) ? true : false);
		$('#DAMESaveButton').prop('disabled', false);
	};

	var _addTable = function () {
		var newGuid = FMDeviceAlarmMapsEditor.newGuid();
		var newId = 'DeviceAlarmMap_' + newGuid;

		$('.tab-add-table').before('<li class="tab-table"><a data-target="#' + newId + '" data-toggle="tab" onclick="FMDeviceAlarmMapsEditor.onTabChange(this);" ondblclick="FMDeviceAlarmMapsEditor.renameTab(this);"><label class="tab-name">' + 'New Device Alarm Map' + '</label><input class="old-tab-name" type="hidden" maxlength = "50" value="' + 'New Device Alarm Map' + '"><input class="tab-guid" type="hidden" value="' + newGuid + '"></a></li>');

		var DeviceAlarmMapNotAlarmText = 
				'<div class="form-group  col-xs-3 col-sm-3 col-md-3 col-lg-3">' +
				'<label for="NotInAlarmText_' + newGuid + '">' + $('#DAMENotAlarmText').val() + '</label>' +
				'<input id="NotInAlarmText_' + newGuid + '" type="text" class="form-control not-alarm-text" maxlength="100" value="" placeholder="' + $('#DAMENotInAlarmText').val() + '" required />' +
			'</div>'

		var alarmCategories = JSON.parse($('#DAMEAlarmCategories').val());

		var DeviceAlarmMapAlarmCategories =
				'<div class="form-group col-xs-3 col-sm-3 col-md-3 col-lg-3" style="height: 59px">' +
					'<label for="AlarmCategory_' + newGuid +'">' + $('#DAMEAlarmCategoryText').val() + '</label>' +
					'<select class="form-control alarm-category" id="AlarmCategory_' + newGuid + '">';
						alarmCategories.forEach (function(alarmCategory){
							DeviceAlarmMapAlarmCategories +=	'<option value="' + alarmCategory.Value + '">' + alarmCategory.Text + '</option>';
						});
						DeviceAlarmMapAlarmCategories +=
					'</select>' +
					'</div>';

		var normalPriorities = JSON.parse($('#DAMENormalPriorities').val());

		var DeviceAlarmMapAlarmPriorities =
				'<div class="form-group col-xs-6 col-sm-6 col-md-6 col-lg-6" style="height: 59px">' +
					'<label for="NormalUnacknowledgedAlarmPriority_' + newGuid + '">' + $('#DAMENormalUnacknowledgedAlarmPriority').val() + '</label>' +
					'<select class="form-control normal-unacknowledged-priority" id="NormalUnacknowledgedAlarmPriority_' + newGuid + '">';
						normalPriorities.forEach(function (normalPriority) {
							DeviceAlarmMapAlarmPriorities += '<option value="' + normalPriority.Value + '">' + normalPriority.Text + '</option>';
						});
						DeviceAlarmMapAlarmPriorities += 
					'</select>' +
				'</div>';

		var addRowDd = $("#AddRowDD").val();
		var deleteRowDd = $("#DeleteRowDD").val();

		var DeviceAlarmMapTable = '<div class="DeviceAlarmMap col-xs-12 col-sm-12 col-md-12 col-lg-12">' +
			'<table id="DeviceAlarmMapEditorTable_' + newGuid + '" class="table table-striped table-bordered hover" style="width:100%">' +
			'<thead class="">' +
			'<tr>' +
			'<th class="text-center">' + $('#DAMESeqText').val() + '</th>' +
			'<th class="text-center">' + $('#DAMEBitMaskText').val() + '</th>' +
			'<th class="text-center">' + $('#DAMEPriorityText').val() + '</th>' +
			'<th class="text-center">' + $('#DAMENameText').val() + '</th>' +
			'</tr>' +
			'</thead>' +
			'<tbody id="DeviceAlarmMapEditorTableBody">' +
			'</tbody>' +
			'</table>' +
			'<br />' +
			'<div class="" role="group">' +
			'	<button id="addDeviceAlarmMapEntryButton_' + newGuid + '" name="addDeviceAlarmMapEntryButton_' + newGuid + '" type="button" value="addDeviceAlarmMapEntryButton" accesskey="A" class="addDeviceAlarmMapEntryButton formfieldtitle tabPushButton" title="' + $('#DAMEAddNewDeviceAlarmEntry').val() + '" style="width: 100px;">' + addRowDd + '</button>' +
			'	<button id="deleteDeviceAlarmMapEntriesButton_' + newGuid + '" name="deleteDeviceAlarmMapEntriesButton_' + newGuid + '" type="button" value="deleteDeviceAlarmMapEntriesButton" accesskey="l" class="deleteDeviceAlarmMapEntriesButton formfieldtitle tabPushButton" title="' + $('#DAMEDeleteDeviceAlarmMapEntries').val() + '" disabled="disabled" style="width: 100px;">' + deleteRowDd + '</button>' +
			'</div>' +
			'</div>';

		var tabPanel = $('<div class="tab-pane" id="' + newId + '">' + DeviceAlarmMapNotAlarmText + DeviceAlarmMapAlarmCategories + DeviceAlarmMapAlarmPriorities + DeviceAlarmMapTable + '</div>').appendTo('.tab-content');

		$('#AlarmCategory_' + newGuid).select2();

		$('#NormalUnacknowledgedAlarmPriority_' + newGuid).select2({
			templateSelection: function (selection) {
				if (selection.selected) {
					var selectedAlarmPriority = jQuery.grep(dameNormalPriorityData, function (a) {
						return a.identityGuid === selection.id;
					});

					if (selectedAlarmPriority.length > 0) {
						return $.parseHTML(selection.text + selectedAlarmPriority[0].html);
					}
					return $.parseHTML(selection.text);
				}

			},
			templateResult: function (selection) {
				var selectedAlarmPriority = jQuery.grep(dameNormalPriorityData, function (a) {
					return a.identityGuid === selection.id;
				});

				if (selectedAlarmPriority.length > 0) {
					return $.parseHTML(selection.text + selectedAlarmPriority[0].html);
				}
				return $.parseHTML(selection.text);
			}
		});



		var DeviceAlarmMap = $('#DeviceAlarmMapEditorTable_' + newGuid).DataTable(
			{
				"retrieve": true,
				"select": true,
				"scrollY": '300px',
				"sScrollX": '100%',
				"sScrollXInner": '100%',
				"scrollCollapse": false,
				"paging": false,
				"autoWidth": false,
				"rowReorder": true,
				"columnDefs": [
							{ "width": "5%", "targets": [0], "name": 'Seq.', "orderable": false, className: 'reorder text-center' },
							{ "width": "20%", "targets": [1], "name": 'Bit Mask', "orderable": false, className: 'DeviceAlarmMapBitMaskColumn text-center' },
							{ "width": "45%", "targets": [2], "name": 'Priority', "orderable": false, className: 'DeviceAlarmMapPriorityColumn text-center' },
							{ "width": "30%", "targets": [3], "name": 'Name', "orderable": false, className: 'DeviceAlarmMapNameColumn text-center' },
				],
				"dom": 'rt',
				"fnInitComplete": function () {
					// custom scroll bars
					$(this).parent()
						.niceScroll({
							cursorwidth: '10px',
							autohidemode: false,
							cursorcolor: '#486899',
							background: 'rgb(240, 240, 240)',
							horizrailenabled: false
						});
				}
			});


		DeviceAlarmMap.draw();

		FMDeviceAlarmMapsEditor.initializeTabContent(newGuid);

		$('a[data-target="#' + newId + '"]').tab('show');
		DeviceAlarmMap.columns.adjust().draw();

		// refresh the scroll buttons for the tabs (do it async to give time to display the contents of the tab)
		$('#mainTab').scrollingTabs('refresh');

		FMDeviceAlarmMapsEditor.enableChangesToTab();

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
				FMDeviceAlarmMapsEditor.addTable();
			});
		}
	};

	var _initializeTabContent = function (tabId) {
		// if we have reach the limit of entries in the list we need to disable the add button
		var tabContents = "#DeviceAlarmMap_" + tabId;
		var DeviceAlarmMap = $(tabContents + ' .table').DataTable();
		if (DeviceAlarmMap.rows().count() === FMDeviceAlarmMapsEditor.numEntriesPerMap) {
			$(tabContents + ' .addCdeviceAlarmMapElementButton').prop("disabled", true);
		}

		$('#DeviceAlarmMapEditorTable_' + tabId + ' tbody').keyup(function (e) {
			if (e.keyCode === 46) {
				var tabContents = $('#tabList li.active').find('a').attr('data-target');
				var DeviceAlarmMap = $(tabContents + ' .table').DataTable();
				if (!_isStandardTankDefault() && $('#KeyEdit').length === 0 && DeviceAlarmMap.rows('.selected').count() > 0) {
					FMDeviceAlarmMapsEditor.deleteRows();
				}
			}
		});

		// enable delete button when 1 or more rows and selected rows
		$('#DeviceAlarmMapEditorTable_' + tabId + ' tbody').on('click', 'tr', function () {
			setTimeout(function () {
				var tabContents = $('#tabList li.active').find('a').attr('data-target');
				var DeviceAlarmMap = $(tabContents + ' .table').DataTable();
				var selectedCount = DeviceAlarmMap.rows('.selected').count();
				$(tabContents + ' .deleteDeviceAlarmMapEntriesButton').prop('disabled', (selectedCount === 0 || _isStandardTankDefault()) ? true : false);
			}, 500);
		});

		// click on the delete button
		$('#deleteDeviceAlarmMapEntriesButton_' + tabId).on('click', function () {
			FMDeviceAlarmMapsEditor.deleteRows();
		});

		// double click to edit a row 
		$('#DeviceAlarmMapEditorTable_' + tabId + ' tbody').on('dblclick', 'tr', function (e) {
			if (e.target.nodeName === 'INPUT') {
				return;
			}

			FMDeviceAlarmMapsEditor.cancelEditRow();
			FMDeviceAlarmMapsEditor.add = false;
			FMDeviceAlarmMapsEditor.editRow(this);
		});


		// click to add a device alarm map entry row
		$('#addDeviceAlarmMapEntryButton_' + tabId).on('click', function () {
			FMDeviceAlarmMapsEditor.addRow();
		});

		var buttonObj = $('#addDeviceAlarmMapEntryButton_' + tabId);
		var ampersandIndex = buttonObj.text().indexOf('&');
		var hotKey;
		if (ampersandIndex !== -1)
		{
			hotKey = buttonObj.text().charAt(ampersandIndex + 1);
			buttonObj.html(buttonObj.text().replace('&' + hotKey, '<span style="text-decoration: underline;">' + hotKey + '</span>'));
			buttonObj.attr('accesskey', hotKey);
		}

		buttonObj = $('#deleteDeviceAlarmMapEntriesButton_' + tabId);
		ampersandIndex = buttonObj.text().indexOf('&');
		if (ampersandIndex !== -1)
		{
			hotKey = buttonObj.text().charAt(ampersandIndex + 1);
			buttonObj.html(buttonObj.text().replace('&' + hotKey, '<span style="text-decoration: underline;">' + hotKey + '</span>'));
			buttonObj.attr('accesskey', hotKey);
		}
	}

	_onTabChange = function () {
		FMDeviceAlarmMapsEditor.cancelEditRow();

		// since the new tab has not been rendered yet we want to processs after the rendering
		setTimeout(function () {
			$('#mainTab').scrollingTabs('refresh');
			var tabContents = $('#tabList li.active').find('a').attr('data-target');
			var DeviceAlarmMap = $(tabContents + ' .table').DataTable();
			DeviceAlarmMap.columns.adjust().draw();
			FMDeviceAlarmMapsEditor.enableChangesToTab();
		}, 1);
	}

	var _deleteRows = function () {
		FMLayout.Confirm($('#DAMEDeleteConfirmation').val(),
			 null,
			 function () {
			 	var tabContents = $('#tabList li.active').find('a').attr('data-target');
			 	var DeviceAlarmMap = $(tabContents + ' .table').DataTable();
			 	DeviceAlarmMap.rows('.selected').remove();
			 	DeviceAlarmMap.draw('page');
			 	FMDeviceAlarmMapsEditor.valuesChanged = true;

			 	$(tabContents + ' .addDeviceAlarmMapEntryButton').prop("disabled", false);
		 		$(tabContents + ' .deleteDeviceAlarmMapEntriesButton').prop("disabled", true);
		 		$('#DAMESaveButton').prop('disabled', false);

		 		if($('#NameEdit').length == 0) {
		 			$(document).detach('keydown');
		 		}
			 });
	};

	var _deleteTable = function () {
		// Check if table is selected
		var activeTab = $('#tabList li.active');
		if (!activeTab || activeTab.hasClass('tab-add-table')) {
			return;
		}

		// find the previous tab so we can set it as active
		var newActiveTab = activeTab.prev();
		if (newActiveTab.length === 0) {
			newActiveTab = activeTab.next();
		}

		// we need to find the contents so we can delete them too
		var tabContents = $(activeTab).find('a').attr('data-target');

		var tableDescription = $('#tabList li.active .tab-name').text();
		var tableGuid = $('#tabList li.active .tab-guid').val();

		FMLayout.Confirm($('#DAMEDeleteTableConfirmation').val() + ' ' + tableDescription + '?',
			 null,
			 function () {
				$(tabContents).find('.dataTable').DataTable().destroy();
			 	$(tabContents).remove();
			 	$(activeTab).remove();
			 	FMDeviceAlarmMapsEditor.valuesChanged = true;
			 	newActiveTab.find('a').tab('show');
			 	FMDeviceAlarmMapsEditor.onTabChange();
			 	FMDeviceAlarmMapsEditor.deletedDeviceAlarmMaps.push(tableGuid);
			 });
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
		bitMaskSelectOpen: _bitMaskSelectOpen,
		alarmPrioritySelectOpen: _alarmPrioritySelectOpen,
		addTable: _addTable,
		enableAddTab: _enableAddTab,
		initializeTabContent: _initializeTabContent,
		onTabChange: _onTabChange,
		deleteRows: _deleteRows,
		deleteTable: _deleteTable,
		processingEndEditRow: _processingEndEditRow,
		stack_bottomright_DeviceAlarmMaps: _stack_bottomright_DeviceAlarmMaps,
		newGuid: _newGuid,
		numEntriesPerMap: _numEntriesPerMap,
		deletedDeviceAlarmMaps: _deletedDeviceAlarmMaps,
		enableChangesToTab: _enableChangesToTab,
		add: _add,
		isStandardTankDefault: _isStandardTankDefault
	};
}();

// manually hookup to the submit the form to make sure we pass all the entries from the table
$(function () {
	$('#DeviceAlarmMapsForm').submit(function () {
		var action = this.action;
		var method = this.method;
		FMDeviceAlarmMapsEditor.saveChanges();

		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	});
});



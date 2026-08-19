var FMMovementStartDataEditor = function () {
	var _inError = false;
	var _emptyGuid = '00000000-0000-0000-0000-000000000000';
	var _stack_bottomright_movementstartdataeditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 135, "firstpos2": 25, "context": $('#MovementStartDataEditorPropertyScreen') };

	//===============================================================
	// This function is a hookup to the main property page.
	// It is called by the Save button (id = MovementStartDataEditorSavePropertyScreen)
	//===============================================================
	var _SaveChanges = function () {
		// Update the model based on the UI changes.
		FMMovementStartDataEditor.UpdateModel();

		var url = $('#MovementStartDataSaveUrl').val();
		var token = $('#MovementStartDataEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMMovementStartDataEditor.Stack_bottomright_movementstartdataeditor, width: '450px' };

		// remove previous notifications
		PNotify.removeStack(FMMovementStartDataEditor.Stack_bottomright_movementstartdataeditor);

		var movementStartDataEditorModelStr = FMMovementStartDataEditor.GetModelString();

		$.ajax({
			cache: false,
			url: url,
			type: 'POST',
			headers: headers,
			async: false,
			dataType: "json",
			contentType: 'application/json; charset=UTF-8',
			data: JSON.stringify({ 'movementStartDataEditorModelStr': movementStartDataEditorModelStr }),
			success: function (result) {
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError) {
						if (!inError) {
						}
					},
					messageAttributes);
			},
			error:
				function (request, status, error) {
					FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				}
		});
	};

	//=====================================================================
	// This function initializes the movement module settings editor
	//=====================================================================
	var _Initialize = function () {
		FMMovementStartDataEditor.InitializeDateControls();
		FMMovementStartDataEditor.LoadData();
	};

	//============================================================
	// This function initializes the date time functions.
	//============================================================
	_InitializeDateControls = function () {
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numFormatInfo = JSON.parse(numFormatInfoString);
		FMLayout.dateFormat = ConvertToJQueryUIDateFormat(numFormatInfo.ShortDatePattern);
		FMLayout.timeFormat = ConvertToJQueryUITimeFormat(numFormatInfo.TimePattern);
		FMLayout.calendarLocation = $("#CalendarLocationUrl").val();

		$("#TransferStartTimePicker").datetimepicker({
			buttonImage: FMLayout.calendarLocation + '/calendar.gif',
			buttonImageOnly: true,
			 showOn: "button",
			 showTimezone: false,
			 useLocalTimezone: false,
			 defaultTimezone: $("#datepickerTimezoneString").val(),
			dateFormat: FMLayout.dateFormat,
			timeFormat: FMLayout.timeFormat,
			showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
			beforeShow: function () {
				setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
			}
		});
	};

	//=======================================================================
	// This function handles the apply to nodes checkbox on change event.
	// It will update the model based on the change.
	//=======================================================================
	_applyToNodesCbOnChange = function () {
		var model = FMMovementStartDataEditor.GetModel();

		if (model) {
			var checkboxValue = $("#ApplyToNodesCb").is(":checked");
			model.ApplyToNodes = checkboxValue;
			FMMovementStartDataEditor.SetModel(model);
		}
	};


	//===================================================
	// This function returns the movement user data 
	// model as a string.
	//===================================================
	_GetModelString = function () {
		return $('#MovementStartDataEditorModelStr').val();
	};

	//===================================================
	// This function returns the movement user data 
	// model as an object.
	//===================================================
	_GetModel = function () {
		return JSON.parse(_GetModelString());
	};

	//===================================================
	// This function set the movement user data model as
	// a string.
	//===================================================
	_SetModelString = function (modelStr) {
		$('#MovementStartDataEditorModelStr').val(modelStr);
	};

	//===================================================
	// This function set the movement start data model as
	// into a hidden tag.
	//===================================================
	_SetModel = function (model) {
		var modelStr = JSON.stringify(model);
		_SetModelString(modelStr);
	};

	//=====================================================================
	// This function will load the page with data from the model.
	//=====================================================================
	_LoadData = function () {
		var model = FMMovementStartDataEditor.GetModel();

		if (!model) {
			return;
		}

		$("#TransferStartTimePicker").val(model.TransferStartTime);
	};

	//========================================================================
	// This function will update the model based on the values from the UI.
	//========================================================================
	_UpdateModel = function () {
		var model = FMMovementStartDataEditor.GetModel();

		if (model) {
			model.TransferStartTime = $.trim($("#TransferStartTimePicker").val());

			FMMovementStartDataEditor.SetModel(model);
		}
	};

	//======================================================
	// Return function pointers
	//======================================================
	return {
		inError: _inError
		, emptyGuid: _emptyGuid
		, SaveChanges: _SaveChanges
		, Initialize: _Initialize
		, ApplyToNodesCbOnChange: _applyToNodesCbOnChange
		, InitializeDateControls: _InitializeDateControls
		, GetModelString: _GetModelString
		, GetModel: _GetModel
		, SetModelString: _SetModelString
		, SetModel: _SetModel
		, Stack_bottomright_movementstartdataeditor: _stack_bottomright_movementstartdataeditor
		, LoadData: _LoadData
		, UpdateModel: _UpdateModel
	};
}();


//=======================================================
// This function manually hooks up to the submit the form
//=======================================================
$(function () {
	$('#MovementStartDataEditorForm').on('keyup keypress', function (e) {
		var keyCode = e.keyCode || e.which;
		if (keyCode === 13) {
			e.preventDefault();
			return false;
		}
	});

	$('#MovementStartDataEditorForm').submit(function () {
		FMMovementStartDataEditor.SaveChanges();

		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	});
});


//=======================================================================
// RUN after page has been loaded but before render
//=======================================================================
$(document).ready(function () {
	// Initialize the movement user data
	FMMovementStartDataEditor.Initialize();

	// Hide the Header, as MovementStartDataEditor provides one
	$('.modal-header').hide();

	if ($('#Readonly').val() === 'True') {
		$('#MovementStartDataEditorSavePropertyScreen').attr('disabled', true);
	}

	FMErrorAndExceptionHandling.CloseNotifications();
});





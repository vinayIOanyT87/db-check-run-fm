var MovementHistoryMovementDataEditor = function ()
{
	var _inError = false;
	var _emptyGuid = '00000000-0000-0000-0000-000000000000';
	var _stack_bottomright_movementhistorymovementdataeditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 60, "firstpos2": 20, "context": $('#MovementHistoryMovementDataEditorPropertyScreen') };

	//===============================================================
	// This function is a hookup to the main property page.
	// It is called by the Save button (id = MovementStartDataEditorSavePropertyScreen)
	//===============================================================
	var _SaveChanges = function ()
	{
		// remove previous notifications
		PNotify.removeStack(MovementHistoryMovementDataEditor.Stack_bottomright_movementhistorymovementdataeditor);

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: MovementHistoryMovementDataEditor.Stack_bottomright_movementhistorymovementdataeditor, width: '450px' };

		// Verify the date time is valid.
		var startDateTimeStr = $("#StartDateTimePicker").val();
		var closeoutDateTimeStr = $("#CloseoutDateTimePicker").val();

		if (MovementHistoryMovementDataEditor.ValidateDateTime(startDateTimeStr) == false)
		{
			FMErrorAndExceptionHandling.ShowError('Start Date/Time is invalid.', null, messageAttributes);
			return;
		}

		if (MovementHistoryMovementDataEditor.ValidateDateTime(closeoutDateTimeStr) == false)
		{
			FMErrorAndExceptionHandling.ShowError('Closeout Date/Time is invalid.', null, messageAttributes);
			return;
		}

		// Update the model based on the UI changes.
		MovementHistoryMovementDataEditor.UpdateModel();

		var url = $('#MovementHistoryMovementDataSaveUrl').val();
		var token = $('#MovementHistoryMovementDataEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		var MovementHistoryMovementDataEditorModelStr = MovementHistoryMovementDataEditor.GetModelString();

		$.ajax({
			cache: false,
			url: url,
			type: 'POST',
			headers: headers,
			async: false,
			dataType: "json",
			contentType: 'application/json; charset=UTF-8',
			data: JSON.stringify({ 'MovementHistoryMovementDataEditorModelStr': MovementHistoryMovementDataEditorModelStr }),
			success: function (result)
			{
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError)
					{
						if (!inError)
						{
						}
					},
					messageAttributes);
			},
			error:
				function (request, status, error)
				{
					FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				}
		});
	};

	//=====================================================================
	// This function initializes the movement module settings editor
	//=====================================================================
	var _Initialize = function ()
	{
		MovementHistoryMovementDataEditor.InitializeDateControls();
		MovementHistoryMovementDataEditor.LoadData();
		MovementHistoryMovementDataEditor.CheckModifyRights();
	};

	//============================================================
	// This function initializes the date time functions.
	//============================================================
	_InitializeDateControls = function ()
	{
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numFormatInfo = JSON.parse(numFormatInfoString);
		FMLayout.dateFormat = ConvertToJQueryUIDateFormat(numFormatInfo.ShortDatePattern);
		FMLayout.timeFormat = ConvertToJQueryUITimeFormat(numFormatInfo.TimePattern);
		FMLayout.calendarLocation = $("#CalendarLocationUrl").val();

		$("#StartDateTimePicker").datetimepicker({
			buttonImage: FMLayout.calendarLocation + '/calendar.gif',
			buttonImageOnly: true,
			 showOn: "button",
			 showTimezone: false,
			 useLocalTimezone: false,
			 defaultTimezone: $("#datepickerTimezoneString").val(),
			dateFormat: FMLayout.dateFormat,
			timeFormat: FMLayout.timeFormat,
			showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
			beforeShow: function ()
			{
				setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
			}
		});

		$("#CloseoutDateTimePicker").datetimepicker({
			buttonImage: FMLayout.calendarLocation + '/calendar.gif',
			buttonImageOnly: true,
			 showOn: "button",
			 showTimezone: false,
			 useLocalTimezone: false,
			 defaultTimezone: $("#datepickerTimezoneString").val(),
			dateFormat: FMLayout.dateFormat,
			timeFormat: FMLayout.timeFormat,
			showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
			beforeShow: function ()
			{
				setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
			}
		});
	};

	//===================================================
	// This function returns the movement user data 
	// model as a string.
	//===================================================
	_GetModelString = function ()
	{
		return $('#MovementHistoryMovementDataEditorModelStr').val();
	};

	//===================================================
	// This function returns the movement user data 
	// model as an object.
	//===================================================
	_GetModel = function ()
	{
		return JSON.parse(MovementHistoryMovementDataEditor.GetModelString());
	};

	//===================================================
	// This function set the movement user data model as
	// a string.
	//===================================================
	_SetModelString = function (modelStr)
	{
		$('#MovementHistoryMovementDataEditorModelStr').val(modelStr);
	};

	//===================================================
	// This function set the movement start data model as
	// into a hidden tag.
	//===================================================
	_SetModel = function (model)
	{
		var modelStr = JSON.stringify(model);
		MovementHistoryMovementDataEditor.SetModelString(modelStr);
	};

	//=====================================================================
	// This function will load the page with data from the model.
	//=====================================================================
	_LoadData = function ()
	{
		var model = MovementHistoryMovementDataEditor.GetModel();
		if (!model)
		{
			return;
		}

		$("#StartDateTimePicker").val(model.StartDateTimeStr);
		$("#CloseoutDateTimePicker").val(model.CloseoutDateTimeStr);
	};

	//========================================================================
	// This function will update the model based on the values from the UI.
	//========================================================================
	_UpdateModel = function ()
	{
		var model = MovementHistoryMovementDataEditor.GetModel();

		if (model)
		{
			model.StartDateTimeStr = $.trim($("#StartDateTimePicker").val());
			model.CloseoutDateTimeStr = $.trim($("#CloseoutDateTimePicker").val());

			MovementHistoryMovementDataEditor.SetModel(model);
		}
	};

	//=====================================================================
	// This function will validate whether the date/time is valid. False
	// indicates invalid.
	//=====================================================================
	_ValidateDateTime = function (inDateTime)
	{
		if (inDateTime == null || inDateTime === "")
		{
			return true;
		}

		var numFormatInfoString = $("#NumberFormatInfoString").val();
		var numFormatInfo = JSON.parse(numFormatInfoString);
		var pattern = numFormatInfo.ShortDatePattern;

		// Moment does not like the slashes in a pattern, but handles the slashes in the date.
		pattern = pattern.replaceAll("/", "-");
		pattern = pattern.replaceAll("y", "Y");
		pattern = pattern.replaceAll("d", "D");
		pattern = pattern.replaceAll("m", "M");

		pattern = pattern + " " + numFormatInfo.TimePattern;

		var dateTime = moment(inDateTime, pattern);
		var valid = dateTime.isValid();

		if (valid == false)
		{
			return false;
		}

		return true;
	};

	//==================================================================
	// This function will disable all the controls and save button
	// if the user does not have the modify rights.
	//==================================================================
	var _CheckModifyRights = function ()
	{
		var model = MovementHistoryMovementDataEditor.GetModel();
		
		if (model && model.HasModifyRights)
		{
			$("#StartDateTimePicker").removeAttr("disabled");
			$("#StartDateTimePicker").datetimepicker("enable");

			$("#CloseoutDateTimePicker").removeAttr("disabled");
			$("#CloseoutDateTimePicker").datetimepicker("enable");

			$("#MovementHistoryMovementDataEditorSavePropertyScreen").removeAttr("disabled");
		}
        else
		{
			$("#StartDateTimePicker").attr("disabled", true);
			$("#StartDateTimePicker").datetimepicker("option", "disabled", true);

			$("#CloseoutDateTimePicker").attr("disabled", true);
			$("#CloseoutDateTimePicker").datetimepicker("option", "disabled", true);

			$("#MovementHistoryMovementDataEditorSavePropertyScreen").attr("disabled", true);
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
		, InitializeDateControls: _InitializeDateControls
		, GetModelString: _GetModelString
		, GetModel: _GetModel
		, SetModelString: _SetModelString
		, SetModel: _SetModel
		, Stack_bottomright_movementhistorymovementdataeditor: _stack_bottomright_movementhistorymovementdataeditor
		, LoadData: _LoadData
		, UpdateModel: _UpdateModel
		, ValidateDateTime: _ValidateDateTime
		, CheckModifyRights: _CheckModifyRights
	};
}();


//=======================================================
// This function manually hooks up to the submit the form
//=======================================================
$(function ()
{
	$('#MovementHistoryMovementDataEditorForm').on('keyup keypress', function (e)
	{
		var keyCode = e.keyCode || e.which;
		if (keyCode === 13)
		{
			e.preventDefault();
			return false;
		}
	});
});


//=======================================================================
// RUN after page has been loaded but before render
//=======================================================================
$(document).ready(function ()
{
	// Initialize the movement nod start data
	MovementHistoryMovementDataEditor.Initialize();

	// Hide the Header, as MovementStartDataEditor provides one
	$('.modal-header').hide();

	if ($('#Readonly').val() === 'True')
	{
		$('#MovementHistoryMovementDataEditorSavePropertyScreen').attr('disabled', true);
	}

	FMErrorAndExceptionHandling.CloseNotifications();
});





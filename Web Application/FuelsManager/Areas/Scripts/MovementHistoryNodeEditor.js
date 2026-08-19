var MovementHistoryNodeEditor = function ()
{
	var _inError = false;
	var _emptyGuid = '00000000-0000-0000-0000-000000000000';
	var _stack_bottomright_movementhistorynodeeditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 60, "firstpos2": 20, "context": $('#MovementHistoryNodeEditorPropertyScreen') };
	var _archivedData = false;

	//===============================================================
	// This function is a hookup to the main property page.
	// It is called by the Save button (id = MovementStartDataEditorSavePropertyScreen)
	//===============================================================
	var _SaveChanges = function ()
	{
		// remove previous notifications
		PNotify.removeStack(MovementHistoryNodeEditor.Stack_bottomright_movementhistorynodeeditor);

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: MovementHistoryNodeEditor.Stack_bottomright_movementhistorynodeeditor, width: '450px' };

		// Verify the date time is valid.
		var dateTimeStr = $("#DateTimePicker").val();
		if (MovementHistoryNodeEditor.ValidateDateTime(dateTimeStr) == false)
		{
			FMErrorAndExceptionHandling.ShowError('Date/Time is invalid.', null, messageAttributes);
			return;
		}

		// Update the model based on the UI changes.
		MovementHistoryNodeEditor.UpdateModel();

		var url = $('#MovementHistoryNodeSaveUrl').val();
		var token = $('#MovementHistoryNodeEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		var movementHistoryNodeEditorModelStr = MovementHistoryNodeEditor.GetModelString();

		$.ajax({
			cache: false,
			url: url,
			type: 'POST',
			headers: headers,
			async: false,
			dataType: "json",
			contentType: 'application/json; charset=UTF-8',
			data: JSON.stringify({ 'movementHistoryNodeEditorModelStr': movementHistoryNodeEditorModelStr }),
			success: function (result)
			{
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError)
					{
						if (!inError)
						{
							MovementHistoryTab.HandleRefreshBtnEvent();
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
		MovementHistoryNodeEditor.InitializeDateControls();
		MovementHistoryNodeEditor.LoadData();
		MovementHistoryNodeEditor.CheckModifyRights();
	};

	//============================================================
	// This function initializes the date time functions.
	//============================================================
	_InitializeDateControls = function ()
	{
		if (MovementHistoryNodeEditor.UseArchivedData) {
			return;
		}
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numFormatInfo = JSON.parse(numFormatInfoString);
		FMLayout.dateFormat = ConvertToJQueryUIDateFormat(numFormatInfo.ShortDatePattern);
		FMLayout.timeFormat = ConvertToJQueryUITimeFormat(numFormatInfo.TimePattern);
		FMLayout.calendarLocation = $("#CalendarLocationUrl").val();

		$("#DateTimePicker").datetimepicker({
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
		return $('#MovementHistoryNodeEditorModelStr').val();
	};

	//===================================================
	// This function returns the movement user data 
	// model as an object.
	//===================================================
	_GetModel = function ()
	{
		return JSON.parse(MovementHistoryNodeEditor.GetModelString());
	};

	//===================================================
	// This function set the movement user data model as
	// a string.
	//===================================================
	_SetModelString = function (modelStr)
	{
		$('#MovementHistoryNodeEditorModelStr').val(modelStr);
	};

	//===================================================
	// This function set the movement start data model as
	// into a hidden tag.
	//===================================================
	_SetModel = function (model)
	{
		var modelStr = JSON.stringify(model);
		MovementHistoryNodeEditor.SetModelString(modelStr);
	};

	//=====================================================================
	// This function will load the page with data from the model.
	//=====================================================================
	_LoadData = function ()
	{
		var model = MovementHistoryNodeEditor.GetModel();
		if (!model)
		{
			return;
		}
		
		$("#Level").val(model.LevelStr);				
		$("#Temperature").val(model.TemperatureStr);	
		$("#WaterLevel").val(model.WaterLevelStr);		
		$("#GrossVolume").val(model.GrossVolumeStr);	
		$("#NetVolume").val(model.NetVolumeStr);		
		$("#Mass").val(model.MassStr);					
		$("#Density").val(model.DensityStr);			
		$("#StdDensity").val(model.StdDensityStr);		
		$("#Bsw").val(model.BswStr);
		$("#AmbientTemperature").val(model.AmbientTemperatureStr);


		$("#LevelUnits").text(model.LevelUnitsStr);
		$("#TemperatureUnits").text(model.TemperatureUnitsStr);
		$("#WaterLevelUnits").text(model.WaterLevelUnitsStr);
		$("#GrossVolumeUnits").text(model.GrossVolumeUnitsStr);
		$("#NetVolumeUnits").text(model.NetVolumeUnitsStr);
		$("#MassUnits").text(model.MassUnitsStr);
		$("#DensityUnits").text(model.DensityUnitsStr);
		$("#StdDensityUnits").text(model.StdDensityUnitsStr);
		$("#AmbientTemperatureUnits").text(model.AmbientTemperatureUnitsStr);

		if (!MovementHistoryNodeEditor.UseArchivedData) {
			$("#DateTimePicker").val(model.StartOrClosoutTime);
		}
	};

	//========================================================================
	// This function will update the model based on the values from the UI.
	//========================================================================
	_UpdateModel = function ()
	{
		var model = MovementHistoryNodeEditor.GetModel();

		if (model)
		{
			model.StartOrClosoutTime = $.trim($("#DateTimePicker").val());
			model.ArchiveDataMode = MovementHistoryNodeEditor.UseArchivedData;
			if (!MovementHistoryNodeEditor.UseArchivedData) {
				model.LevelStr = $.trim($("#Level").val());
				model.TemperatureStr = $.trim($("#Temperature").val());
				model.WaterLevelStr = $.trim($("#WaterLevel").val());
				model.GrossVolumeStr = $.trim($("#GrossVolume").val());
				model.NetVolumeStr = $.trim($("#NetVolume").val());
				model.MassStr = $.trim($("#Mass").val());
				model.DensityStr = $.trim($("#Density").val());
				model.StdDensityStr = $.trim($("#StdDensity").val());
				model.BswStr = $.trim($("#Bsw").val());
				model.AmbientTemperatureStr = $.trim($("#AmbientTemperature").val());
			}

			MovementHistoryNodeEditor.SetModel(model);
		}
	};

	//==================================================================
	// This function will disable all the controls and save button
	// if the user does not have the modify rights.
	//==================================================================
	var _CheckModifyRights = function ()
	{
		var model = MovementHistoryNodeEditor.GetModel();
		$("#DateTimePicker").attr("disabled", "disabled");
		$("#DateTimePicker").datetimepicker("option", "disabled", true);

		$("#Level").attr("disabled", "disabled");
		$("#Temperature").attr("disabled", "disabled");
		$("#WaterLevel").attr("disabled", "disabled");
		//$("#GrossVolume").attr("disabled", "disabled");
		//$("#NetVolume").attr("disabled", "disabled");
		//$("#Mass").attr("disabled", "disabled");
		$("#StdDensity").attr("disabled", "disabled");
		$("#Bsw").attr("disabled", "disabled");
		//$("#AmbientTemperature").attr("disabled", "disabled");
		$("#MovementHistoryNodeEditorSavePropertyScreen").attr("disabled", "disabled");
		if (model && model.HasModifyRights )
		{
			$("#DateTimePicker").removeAttr("disabled");
			$("#DateTimePicker").datetimepicker("enable");
			$("#MovementHistoryNodeEditorSavePropertyScreen").removeAttr("disabled");

			if (!MovementHistoryNodeEditor.UseArchivedData) {
				$("#Level").removeAttr("disabled");
				$("#Temperature").removeAttr("disabled");
				$("#WaterLevel").removeAttr("disabled");
				//$("#GrossVolume").removeAttr("disabled");
				//$("#NetVolume").removeAttr("disabled");
				//$("#Mass").removeAttr("disabled");
				$("#StdDensity").removeAttr("disabled");
				$("#Bsw").removeAttr("disabled");
			//	$("#AmbientTemperature").removeAttr("disabled");
			}
		}
	};

    //============================================================================
    // This function will update will call to recalculate the values.
    //============================================================================
    var _HandleFieldOnblur = function (fieldId, fieldType)
	{

		if (MovementHistoryNodeEditor.UseArchivedData) {
			return;
		}

		var fieldValue = $("#" + fieldId).val();

        // Check to see if the field value has changed.
		var hasValueChanged = MovementHistoryNodeEditor.HasFieldChanged(fieldId, fieldValue);

        // If no change, then return.
        if (hasValueChanged == false)
        {
            return;
        }

        // Perform validation.
		var isFieldValid = MovementHistoryNodeEditor.ValidFieldValue(fieldId, fieldValue, fieldType);

        if (isFieldValid == false)
        {
            return;
		}

		var modelStr = MovementHistoryNodeEditor.GetModelString();
		var url = $("#CalculateMovementHistoryNodeDataUrl").val();
		var token = $('#MovementHistoryNodeEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

        $.ajax({
			type: 'POST',
			headers: headers,
            url: url,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ "fieldId": fieldId, "modelStr": modelStr }),
            cache: false,
            async: false,
            success: function (data)
            {
                if (data)
                {
                    // Update the model and update the fields.
					MovementHistoryNodeEditor.SetModel(data.Data);
					MovementHistoryNodeEditor.LoadData();
                }
            },
            error: function (xhr, ajaxOptions, thrownError)
            {
                FMErrorAndExceptionHandling.ShowError(thrownError);
                $("#PointGroupSelectionModal").modal("hide");
            }
        });
    };

    //==========================================================================
    // This function will determine if a field value has changed. If so, then
    // it returns true. Otherwise, it returns false.
    //==========================================================================
    var _HasFieldChanged = function (fieldId, fieldValue)
    {
		var model = MovementHistoryNodeEditor.GetModel();
		model.StartOrClosoutTime = $("#DateTimePicker").val();

        if (fieldId === "Level" && model.LevelStr !== fieldValue)
        {
            model.LevelStr = fieldValue;
			MovementHistoryNodeEditor.SetModel(model);
            return true;
        }

        if (fieldId === "Temperature" && model.TemperatureStr !== fieldValue)
        {
            model.TemperatureStr = fieldValue;
			MovementHistoryNodeEditor.SetModel(model);
            return true;
		}

		if (fieldId === "WaterLevel" && model.WaterLevelStr !== fieldValue)
		{
			model.WaterLevelStr = fieldValue;
			MovementHistoryNodeEditor.SetModel(model);
			return true;
		}

		if (fieldId === "GrossVolume" && model.GrossVolumeStr !== fieldValue)
		{
			model.GrossVolumeStr = fieldValue;
			MovementHistoryNodeEditor.SetModel(model);
			return true;
		}

		if (fieldId === "NetVolume" && model.NetVolumeStr !== fieldValue)
		{
			model.NetVolumeStr = fieldValue;
			MovementHistoryNodeEditor.SetModel(model);
			return true;
		}

		if (fieldId === "Mass" && model.MassStr !== fieldValue)
		{
			model.MassStr = fieldValue;
			MovementHistoryNodeEditor.SetModel(model);
			return true;
		}

        if (fieldId === "StdDensity" && model.StdDensityStr !== fieldValue) 
        {
            model.StdDensityStr = fieldValue;
			MovementHistoryNodeEditor.SetModel(model);
            return true;
        }

        if (fieldId === "Bsw" && model.BswStr !== fieldValue)
        {
            model.BswStr = fieldValue;
			MovementHistoryNodeEditor.SetModel(model);
            return true;
        }

		if (fieldId === "AmbientTemperature" && model.AmbientTemperatureStr !== fieldValue)
        {
			model.AmbientTemperatureStr = fieldValue;
			MovementHistoryNodeEditor.SetModel(model);
            return true;
        }

        // Return false if there was a change.
        return false;
	};

	//============================================================================
	// This function will validate the field value based on the type.
	//============================================================================
	var _ValidFieldValue = function (fieldId, fieldValue, fieldType)
	{
		if (fieldValue == null || fieldValue === "") return true;

		var model = MovementHistoryNodeEditor.GetModel();
		var messageAttributes = { addclass: 'stack-bottomright', stack: MovementHistoryNodeEditor.Stack_bottomright_movementhistorynodeeditor, width: '450px' };

		if (fieldType === "LEVEL")
		{
			var levelValue = MovementHistoryNodeEditor.IsLevelValid(fieldValue);

			if (levelValue == "")
			{
				$("#" + fieldId).val("");

				if (fieldId === "WaterLevel") {
					model.WaterLevelStr = "";
				}
				else {
					model.LevelStr = "";
				}
				FMErrorAndExceptionHandling.ShowError("Invalid Level value for field '" + fieldId + "', must be xxx-xx-xx.", null, messageAttributes);
				return false;
			}

			$("#" + fieldId).val(levelValue);

			if (fieldId === "WaterLevel") {
				model.WaterLevelStr = levelValue;
			}
			else {
				model.LevelStr = levelValue;
			}


			MovementHistoryNodeEditor.SetModel(model);
			return true;
		}

		if (fieldType === "DOUBLE")
		{
			var newFieldValue = fieldValue.replace(model.NumberGroupSeparator, "");
			if (isNaN(newFieldValue))
			{
				$("#" + fieldId).val("");
				FMErrorAndExceptionHandling.ShowError("Invalid value for field '" + fieldId + "', must be numeric.", null, messageAttributes);
				return false;
			}
			else
			{
				return true;
			}
		}
	};

	//=================================================================
	// This function will valid if the level string is valid. Returns
	// true if valid.
	//=================================================================
	var _IsLevelValid = function (levelStr)
	{
		if (levelStr == null || levelStr === "")
		{
			return "";
		}

		var parts = levelStr.split("-");

		if (parts.length == 1)
		{
			if (isNaN(parts[0]))
			{
				return "";
			}
            else
			{
				return parts[0] + "-" + "00-00";
            }
		}

		if (parts.length == 2)
		{
			if (isNaN(parts[0]) || isNaN(parts[1]))
			{
				return "";
			}
			else
			{
				if (parts[1].length > 2)
				{
					return "";
				}

				return parts[0] + "-" + parts[1] + "-00";
			}
        }

		if (parts.length == 3)
		{
			if (isNaN(parts[0]) || isNaN(parts[1]) || isNaN(parts[2]))
			{
				return "";
			}
			else
			{
				if (parts[1].length > 2 || parts[2].length > 2)
				{
					return "";
				}

				return parts[0] + "-" + parts[1] + "-" + parts[2];
			}
		}

		return "";
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

	var _GetArchivedData = function () {
		var url = $('#MovementHistoryNodeGetArchivedDataUrl').val();
		var token = $('#MovementHistoryNodeEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: MovementHistoryNodeEditor.Stack_bottomright_movementhistorynodeeditor, width: '450px' };

		// remove previous notifications
		PNotify.removeStack(MovementHistoryNodeEditor.Stack_bottomright_movementnodestartdataeditor);
		var model = MovementHistoryNodeEditor.GetModel();
		//	debugger;
		var archiveDateTime = $("#DateTimePicker").val();
        model.StartOrClosoutTime = archiveDateTime;
		MovementHistoryNodeEditor.SetModel(model);

		var movementHistoryNodeEditorModelStr = MovementHistoryNodeEditor.GetModelString();

		$.ajax({
			cache: false,
			url: url,
			type: 'POST',
			headers: headers,
			async: false,
			dataType: "json",
			contentType: 'application/json; charset=UTF-8',
			data: JSON.stringify({ 'MovementHistoryNodeEditorModelStr': movementHistoryNodeEditorModelStr, 'dateTime': archiveDateTime }),
			success: function (result) {
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError) {
						if (!inError) {
							let modelStr = $("#MovementHistoryNodeEditorModelStr")[0];//, MovementNodeStartDataEditorController.SerializeModel(Model)
							modelStr.value = data;
						}
					},
					messageAttributes);
			},
			error:
				function (request, status, error) {
					FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				}
		});

	}

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
		, Stack_bottomright_movementhistorynodeeditor: _stack_bottomright_movementhistorynodeeditor
		, LoadData: _LoadData
		, UpdateModel: _UpdateModel
		, HandleFieldOnblur: _HandleFieldOnblur
		, HasFieldChanged: _HasFieldChanged
		, ValidFieldValue: _ValidFieldValue
		, ValidateDateTime: _ValidateDateTime
		, CheckModifyRights: _CheckModifyRights
		, IsLevelValid: _IsLevelValid
		, GetArchivedData: _GetArchivedData
		, UseArchivedData: _archivedData
	};
}();


//=======================================================
// This function manually hooks up to the submit the form
//=======================================================
$(function ()
{
	$('#MovementNodeStartDataEditorForm').on('keyup keypress', function (e)
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
	MovementHistoryNodeEditor.Initialize();

	// Hide the Header, as MovementStartDataEditor provides one
	$('.modal-header').hide();

	if ($('#Readonly').val() === 'True')
	{
		$('#MovementHistoryNodeEditorSavePropertyScreen').attr('disabled', 'disabled');
	}

	let getDataFromArchiveDiv = $("#GetDataFromArchiveDiv");
	if (getDataFromArchiveDiv && getDataFromArchiveDiv.length > 0) {

		let getDataFromArchive = $("#GetDataFromArchive");
		getDataFromArchive.click(function () {
			MovementHistoryNodeEditor.UseArchivedData = this.checked;
			var model = MovementHistoryNodeEditor.GetModel();
			//console.log("model.ArchiveDataMode=" + model.ArchiveDataMode);
			if (this.checked) {
				MovementHistoryNodeEditor.GetArchivedData();
				$('#MovementHistoryNodeEditorTopDiv input:not([type="checkbox"]):not(#DateTimePicker)').attr('disabled', 'disabled');
			}
			//console.log("On click UseArchivedData=" + MovementHistoryNodeEditor.UseArchivedData);
			MovementHistoryNodeEditor.Initialize();
			console.log("Model=" + MovementHistoryNodeEditor.GetModelString());
		});
	}

	$("#DateTimePicker").on('change', function () {
		console.log("On change DateTimePicker 1  -  Use archive data=" + MovementHistoryNodeEditor.UseArchivedData);
		if (MovementHistoryNodeEditor.UseArchivedData) {
			var model = MovementHistoryNodeEditor.GetModel();
		//	if (model.TransferStatus == 3) { //3 = Complete
				console.log("On change DateTimePicker 2")
				MovementHistoryNodeEditor.GetArchivedData();
				MovementHistoryNodeEditor.LoadData();
			console.log("Model=" + MovementHistoryNodeEditor.GetModelString());
		//	}
		}
	});

	FMErrorAndExceptionHandling.CloseNotifications();
});





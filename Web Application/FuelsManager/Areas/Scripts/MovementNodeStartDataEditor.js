var FMMovementNodeStartDataEditor = function () {
	var _inError = false;
	var _emptyGuid = '00000000-0000-0000-0000-000000000000';
	var _stack_bottomright_movementnodestartdataeditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 135, "firstpos2": 25, "context": $('#MovementNodeStartDataEditorPropertyScreen') };

	//===============================================================
	// This function is a hookup to the main property page.
	// It is called by the Save button (id = MovementStartDataEditorSavePropertyScreen)
	//===============================================================
	var _SaveChanges = function () {
		// Update the model based on the UI changes.
		FMMovementNodeStartDataEditor.UpdateModel();

		var url = $('#MovementNodeStartDataSaveUrl').val();
		var token = $('#MovementNodeStartDataEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMMovementNodeStartDataEditor.Stack_bottomright_movementnodestartdataeditor, width: '450px' };

		// remove previous notifications
		PNotify.removeStack(FMMovementNodeStartDataEditor.Stack_bottomright_movementnodestartdataeditor);

		var movementNodeStartDataEditorModelStr = FMMovementNodeStartDataEditor.GetModelString();

		$.ajax({
			cache: false,
			url: url,
			type: 'POST',
			headers: headers,
			async: false,
			dataType: "json",
			contentType: 'application/json; charset=UTF-8',
			data: JSON.stringify({ 'movementNodeStartDataEditorModelStr': movementNodeStartDataEditorModelStr }),
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
		//debugger;
		FMMovementNodeStartDataEditor.InitializeDateControls();
		FMMovementNodeStartDataEditor.LoadData();
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

		$("#TransferNodeStartTimePicker").datetimepicker({
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
		var model = FMMovementNodeStartDataEditor.GetModel();

		if (model) {
			var checkboxValue = $("#ApplyToNodesCb").is(":checked");
			model.ApplyToNodes = checkboxValue;
			FMMovementNodeStartDataEditor.SetModel(model);
		}
	};


	//===================================================
	// This function returns the movement user data 
	// model as a string.
	//===================================================
	_GetModelString = function () {
		return $('#MovementNodeStartDataEditorModelStr').val();
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
		$('#MovementNodeStartDataEditorModelStr').val(modelStr);
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
		var model = FMMovementNodeStartDataEditor.GetModel();

		if (!model) {
			return;
		}

		$("#TransferNodeStartTimePicker").val(model.TransferStartTime);
		FMMovementNodeStartDataEditor.UpdateUIXValues();
	};

	//========================================================================
	// This function will update the model based on the values from the UI.
	//========================================================================
	_UpdateModel = function () {
		var model = FMMovementNodeStartDataEditor.GetModel();
		if (model) {
			model.TransferStartTime = $.trim($("#TransferNodeStartTimePicker").val());
			let getArchiveDataCheckbox = $("#MovementNodeStartDataGetDataFromArchive");
			if (!(getArchiveDataCheckbox && getArchiveDataCheckbox[0].checked)) {//don't update the level, temp,etc in model if it is archive data since it is read only and with less precision
				var level = $("#MovementNodeStartDataLevel").val();

				// #Level is excluded for VolumeTransferModule based points
				if (level !== undefined) {
					model.Level = FMFormatValues.ParseValue(model.LevelUnits, model.NumberFormatInfo, $.trim(level), true); // Level could be ft-in-16th or ft-in-8th, which requires parsing back to a double
				}
				model.Temperature = $.trim($("#MovementNodeStartDataTemperature").val());
				model.GrossVolume = $.trim($("#MovementNodeStartDataGrossVolume").val());
				model.NetVolume = $.trim($("#MovementNodeStartDataNetVolume").val());
				model.Mass = $.trim($("#MovementNodeStartDataMass").val());
				model.Density = $.trim($("#MovementNodeStartDataDensity").val());
				model.StdDensity = $.trim($("#MovementNodeStartDataStdDensity").val());
			}

			FMMovementNodeStartDataEditor.SetModel(model);
		}
	};

	var _UpdateUIXValues = function () {
		var model = FMMovementNodeStartDataEditor.GetModel();

		$("#MovementNodeStartDataLevel").val(model.LevelFmtStr);
		$("#MovementNodeStartDataTemperature").val(model.TemperatureFmtStr);
		$("#MovementNodeStartDataGrossVolume").val(model.GrossVolumeFmtStr);
		$("#MovementNodeStartDataNetVolume").val(model.NetVolumeFmtStr);
		$("#MovementNodeStartDataMass").val(model.MassFmtStr);
		$("#MovementNodeStartDataDensity").val(model.DensityFmtStr);
		$("#MovementNodeStartDataStdDensity").val(model.StdDensityFmtStr);
		console.log("_UpdateUIXValues model.LevelFmtStr  =" + model.LevelFmtStr);
		//console.log("_UpdateUIXValues #Level  =" + $("#Level").length);
		console.log("_UpdateUIXValues model.TemperatureFmtStr  =" + model.TemperatureFmtStr);
		console.log("_UpdateUIXValues model.GrossVolumeFmtStr  =" + model.GrossVolumeFmtStr);
		console.log("_UpdateUIXValues model.NetVolumeFmtStr  =" + model.NetVolumeFmtStr);
		console.log("_UpdateUIXValues model.MassFmtStr  =" + model.MassFmtStr);
		console.log("_UpdateUIXValues model.DensityFmtStr  =" + model.DensityFmtStr);
		console.log("_UpdateUIXValues model.StdDensityFmtStr  =" + model.StdDensityFmtStr);

		console.log("_CheckModifyRights  level.count=" + $("#MovementNodeStartDataLevel").length);
		
		$("#LevelUnits").val(model.LevelUnitsStr);
		$("#TemperatureUnits").val(model.TemperatureUnitsStr);
		$("#GrossVolumeUnits").val(model.GrossVolumeUnitsStr);
		$("#NetVolumeUnits").val(model.NetVolumeUnitsStr);
		$("#MassUnits").val(model.MassUnitsStr);
		$("#DensityUnits").val(model.DensityUnitsStr);
		$("#StdDensityUnits").val(model.StdDensityUnitsStr);

		var xyz = FMMovementNodeStartDataEditor.GetModelString();
		console.log("_UpdateUIXValues ModelString="+xyz);
	}

	var _GetArchivedData = function () {
		var url = $('#MovementNodeStartDataGetArchivedDataUrl').val();
		var token = $('#MovementNodeStartDataEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMMovementNodeStartDataEditor.Stack_bottomright_movementnodestartdataeditor, width: '450px' };

		// remove previous notifications
		PNotify.removeStack(FMMovementNodeStartDataEditor.Stack_bottomright_movementnodestartdataeditor);
		var model = FMMovementNodeStartDataEditor.GetModel();
	//	debugger;
		var archiveDateTime = $("#TransferNodeStartTimePicker").val();
		model.TransferStartTime = archiveDateTime;
		FMMovementNodeStartDataEditor.SetModel(model);

		var movementNodeStartDataEditorModelStr = FMMovementNodeStartDataEditor.GetModelString();

//		alert(movementNodeStartDataEditorModelStr);
		$.ajax({
			cache: false,
			url: url,
			type: 'POST',
			headers: headers,
			async: false,
			dataType: "json",
			contentType: 'application/json; charset=UTF-8',
		  data: JSON.stringify({ 'movementNodeStartDataEditorModelStr': movementNodeStartDataEditorModelStr }),
			success: function (result) {
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError) {
						if (!inError) {
							let modelStr = $("#MovementNodeStartDataEditorModelStr")[0];//, MovementNodeStartDataEditorController.SerializeModel(Model)
							modelStr.value = data;
							var xyz = FMMovementNodeStartDataEditor.GetModelString();
							//alert(xyz);
							//console.log("_GetArchivedData  modelString=" + xyz);

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
		, ApplyToNodesCbOnChange: _applyToNodesCbOnChange
		, InitializeDateControls: _InitializeDateControls
		, GetModelString: _GetModelString
		, GetModel: _GetModel
		, SetModelString: _SetModelString
		, SetModel: _SetModel
		, Stack_bottomright_movementnodestartdataeditor: _stack_bottomright_movementnodestartdataeditor
		, LoadData: _LoadData
		, UpdateModel: _UpdateModel
		, GetArchivedData: _GetArchivedData
        , UpdateUIXValues: _UpdateUIXValues
	};
}();


//=======================================================
// This function manually hooks up to the submit the form
//=======================================================
$(function () {
	$('#MovementNodeStartDataEditorForm').on('keyup keypress', function (e) {
		var keyCode = e.keyCode || e.which;
		if (keyCode === 13) {
			e.preventDefault();
			return false;
		}
	});

	$('#MovementNodeStartDataEditorForm').submit(function () {
		FMMovementNodeStartDataEditor.SaveChanges();

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
	//debugger;
	// Initialize the movement nod start data
	FMMovementNodeStartDataEditor.Initialize();

	// Hide the Header, as MovementStartDataEditor provides one
	$('.modal-header').hide();

	if ($('#Readonly').val() === 'True') {
		$('#MovementNodeStartDataEditorSavePropertyScreen').attr('disabled', 'disabled');
	}
	let getDataFromArchiveDiv = $("#MovementNodeStartDataGetDataFromArchiveDiv");
	if (getDataFromArchiveDiv && getDataFromArchiveDiv.length > 0) {
		var model = FMMovementNodeStartDataEditor.GetModel();
		console.log(" transferStatus=" + model.TransferStatus);
		getDataFromArchiveDiv.hide();
		if (model.TransferStatus == 3) { //3 = Complete
			getDataFromArchiveDiv.show();
			let getDataFromArchive = $("#MovementNodeStartDataGetDataFromArchive");
			getDataFromArchive.click(function () {
				$('#MovementUserDataTopDiv input:not([type="checkbox"]):not(#TransferNodeStartTimePicker)').removeAttr('disabled');
				if (this.checked) {
					//alert("checked")
					FMMovementNodeStartDataEditor.GetArchivedData();
				}

				FMMovementNodeStartDataEditor.Initialize();
				FMMovementNodeStartDataEditor.UpdateUIXValues();
				if (this.checked) {
					$('#MovementUserDataTopDiv input:not([type="checkbox"]):not(#TransferNodeStartTimePicker)').attr('disabled', 'disabled');
				}
			});
		}
	}

	$("#TransferNodeStartTimePicker").on('change', function () {
	//	console.log("On change TransferNodeStartTimePicker 1")
		let getArchiveDataCheckbox = $("#MovementNodeStartDataGetDataFromArchive");
	//	debugger;
		if (getArchiveDataCheckbox && getArchiveDataCheckbox.length > 0 && getArchiveDataCheckbox[0].checked) {
			var model = FMMovementNodeStartDataEditor.GetModel();
			if (model.TransferStatus == 3) { //3 = Complete
		//		console.log("On change TransferNodeStartTimePicker 2")
				FMMovementNodeStartDataEditor.GetArchivedData();
				FMMovementNodeStartDataEditor.LoadData();
				FMMovementNodeStartDataEditor.UpdateUIXValues();
				$('#MovementUserDataTopDiv input:not([type="checkbox"]):not(#TransferNodeStartTimePicker)').attr('disabled', 'disabled');
			}
		}
	});

	FMErrorAndExceptionHandling.CloseNotifications();
});





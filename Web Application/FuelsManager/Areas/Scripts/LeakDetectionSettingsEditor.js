var FMLeakDetectionSettingsEditor = function () {

	var _Stack_bottomright_leakdetectionmodulesettingseditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 55, "firstpos2": 25, "context": $('#ModulePropertyEditorPropertyScreen') };

	$("#MinimumFillPercentage").on('keydown', function (e) {

		if (e.key == 'Backspace' || e.key == 'Delete') {
			return;
        }
		const val = parseInt(this.value + e.key);

		// Remove leading zeros
		if (this.value == '0' && !isNaN(val)) {
			this.value = ''
        }
		if (isNaN(val) || e.key == '+' || e.key == '-' || e.key == '.' || e.key == 'e' || val > 100 || val < 0) {
			return false;
		}
	});

	//===============================================================
	// This function is a hookup to the main property page.
	// It is called by the Save button (id = PEMPESavePropertyScreen)
	//===============================================================
	var _SaveChanges = function () {
		var url = $('#urlSaveLeakDetectionModuleSettings').val();
		var token = $('#leakDetectionEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMLeakDetectionSettingsEditor.Stack_bottomright_leakdetectionmodulesettingseditor, width: '450px' };

		// remove previous notifications
		PNotify.removeStack(FMLeakDetectionSettingsEditor.Stack_bottomright_leakdetectionmodulesettingseditor);


		var formData = {
			'isTemplatePoint': $("#IsTemplatePoint").val(),
			'pointGuid': $("#PointGuid").val(),
			'pointPropertyGuid': $("#PointPropertyGuid").val(),
			'LeakAnalysisMethod': $("#LeakAnalysisMethodList").val(),
			'LeakAnalysisType': $("#LeakAnalysisTypeList").val(),
			'LeakAutoPrint': $('input[name="LeakAutoPrint"]:checked').val(),
			'LeakPrintDaysBeforeEndOfMonth': $("#LeakPrintDaysBeforeEndOfMonth").val(),
			'GaugeType': $("#GaugeTypeList").val(),
			'LeakPrintTime': $("#LeakPrintTime").val(),
			'MinimumFillPercentageStr': $("#MinimumFillPercentage").val()
		};


		$.ajax({
			cache: false,
			url: url,
			type: 'POST',
			headers: headers,
			async: false,
			dataType: "json",
			contentType: 'application/json; charset=UTF-8',
			data: JSON.stringify(formData),
			success: function (result) {
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError) {
						if (!inError) {
							FMLeakDetectionSettingsEditor.valuesChanged = false;
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
	// This function Set Auto print off off is it is not a real time
	//=====================================================================
	var CheckRealTimeSettings = function () {
		if ($('#LeakAnalysisTypeList').val() != 'RealTime') {
			$('input[name="LeakAutoPrint"]').filter('[value=false]').prop('checked', true);
			$('input[name="LeakAutoPrint"]').prop("disabled", true);

		} else {
			$('input[name="LeakAutoPrint"]').prop("disabled", false);
		}

		CheckPrintSettings();
	}

	//=====================================================================
	// This function Set Auto print off off is it is not a real time
	//=====================================================================
	var CheckPrintSettings = function () {
		if ($('input[name="LeakAutoPrint"]').filter('[value=false]').prop('checked') == true) {
			$("#LeakPrintDaysBeforeEndOfMonth").prop("disabled", true);
			$("#LeakPrintTime").prop("disabled", true);
			$("#LeakPrintTime").timepicker("option", "disabled", true);

		} else {
			$("#LeakPrintDaysBeforeEndOfMonth").prop("disabled", false);
			$("#LeakPrintTime").prop("disabled", false);
			$("#LeakPrintTime").timepicker("option", "disabled", false);
		}
	}


	//=====================================================================
	// This function initializes the leak detection module settings editor
	//=====================================================================
	var _Initialize = function () {
		setTimeout(CheckRealTimeSettings, 100);
		$('input[name="LeakAutoPrint"]').on('change', function () {
			CheckPrintSettings();
		});
		$('#LeakAnalysisTypeList').on('change', function () {
			CheckRealTimeSettings();
		});
		
	};

	//======================================================
	// Return function pointers
	//======================================================
	return {
		 SaveChanges: _SaveChanges
		, Initialize: _Initialize
		, Stack_bottomright_leakdetectionmodulesettingseditor: _Stack_bottomright_leakdetectionmodulesettingseditor
	};
}();


//=======================================================
// This function manually hooks up to the submit the form
//=======================================================
$(function () {
	$('#leakDetectionEditorForm').on('keyup keypress', function (e) {
		var keyCode = e.keyCode || e.which;
		if (keyCode === 13) {
			e.preventDefault();
			return false;
		}
	});

	$('#leakDetectionEditorForm').submit(function (event) {
		event.preventDefault();
		FMLeakDetectionSettingsEditor.SaveChanges();
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

	// Initialize the movement node tabl
	FMLeakDetectionSettingsEditor.Initialize();

	// Hide the Header, as ModuleSettingsEditor provides one
	$('.modal-header').hide();

	if ($('#Readonly').val() === 'True') {
		$('#PEMPESavePropertyScreen').attr('disabled', true);
	}

	FMErrorAndExceptionHandling.CloseNotifications();
});


//# sourceURL=LeakDetectionSettingsEditor.js
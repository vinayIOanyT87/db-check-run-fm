//debugger;

// create a class with helper functions for the vessel settings editor
var FMTankCommandSettingsEditor = function () {

	var _stack_bottomright_vesselsettings = { "dir1": "up", "dir2": "left", "firstpos1": 75, "firstpos2": 25, "context": $("#ModulePropertyEditorPropertyScreen") };

	var _SaveChanges = function () {
		var url = $('#urlSaveTankCommandSettings').val();
		var token = $('#TankCommandSettingsEditorForm input[name =__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		var messageAttributes = { addclass: "stack-bottomright", stack: FMTankCommandSettingsEditor.stack_bottomright_vesselsettings, width: "450px" };
		// remove any notification
		PNotify.removeStack(FMTankCommandSettingsEditor.stack_bottomright_vesselsettings);
		
		$.ajax({
			url: url,
			type: 'post',
			headers: headers,
			data: 'PointGuid=' + $('[name=PointGuid]').val() + '&PointPropertyGuid=' + $('[name=PointPropertyGuid]').val() + FMPointEditor.serializeSettings("TankCommandSettingsEditorPartial"),
			success: function (result) {
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError) { },
					 messageAttributes);
			},
			error:
				function (request, status, error) {
					FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				}
		});
	}
	
	return {
		SaveChanges: _SaveChanges,
		stack_bottomright_vesselsettings: _stack_bottomright_vesselsettings
	}

}();

// manually hookup to the submit the form
$(function () {
	$('#TankCommandSettingsEditorForm').submit(function () {
		var action = this.action;
		var method = this.method;

		FMTankCommandSettingsEditor.SaveChanges();

		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	});

});




//--------------------------------------- RUN after page has been loaded but before render -----------------------------

$(document).ready(function () {

	// Initialize the Settings
	$('#TankCommandSettingsEditorPartial [data-unit]').each(function (index, control) {
		FMPointEditor.formatSetting(control);
	});


	// when one of the settings gets focus store the value so we can check if we have change it when losing focus
	$('#TankCommandSettingsEditorPartial').on("focus", '#TankCommandSettingsPropertyList > > [data-display=input] input', function (data) {
		$(this).data("oldValue", $(this).val());
		return true;
	});

	// change a setting a property value in general settings and lose focus( we need to get the new raw data and the formatted value)
	$("#TankCommandSettingsEditorPartial").on("blur", '#TankCommandSettingsPropertyList > > [data-display=input] input', function (data) {
		var controlId = $(this).attr("id");
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);

		var formattedValue = $(this).val();

		var unitType = $(this).attr('data-unit');

		// if we don't have a unit attribute then ignore the parsing (probably not numeric value)
		// For some browsers, `attr` is undefined; for others,
		// `attr` is false.  Check for both.
		if (typeof unitType === typeof undefined || unitType === false) {
			return true;
		}

		// if we are not using default units no need to format
		if ($(this).attr("data-unit") === "PENoneEngineeringUnits") {
			$('#' + controlId).attr('data-raw-value', formattedValue);
			return true;
		}

		var unit = $("#" + $(this).attr("data-unit")).val(); // get the unit used by the unit type selected
		var numDecimalsControl = $(this).attr("data-format");
		numformatInfo.NumberDecimalDigits = ~~Number($("#" + numDecimalsControl).val());;
		var newRawValue = $(this).attr('data-raw-value');

		//Parse Value (only if it has changed)
		if ($(this).data("oldValue") != $(this).val()) {
			newRawValue = FMFormatValues.ParseValue(parseInt(unit), numformatInfo, formattedValue);
			$(this).attr('data-raw-value', newRawValue.toString());
		}

		//Format Value
		var newFormattedValue = FMFormatValues.FormatValue(parseInt(unit), numformatInfo, newRawValue);
		$(this).val(newFormattedValue);

		return true;

	});

	// change a setting a property value in general settings and lose focus ( set to zero if left blank )
	$("#TankCommandSettingsEditorPartial .onlynumbers").on("blur", function (data) {
		var formattedValue = $(this).val();
		if ( formattedValue === "" )
		{
			$(this).val( "0");
		}

		return true;

	});


	// only allow numeric values in the HoldOff fields
	$("#TankCommandSettingsEditorPartial .onlynumbers").keydown(function (e) {
		// Allow: backspace, delete, tab, escape, enter and .
		if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
			// Allow: Ctrl/cmd+A
			 (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
			// Allow: Ctrl/cmd+C
			 (e.keyCode == 67 && (e.ctrlKey === true || e.metaKey === true)) ||
			// Allow: Ctrl/cmd+X
			 (e.keyCode == 88 && (e.ctrlKey === true || e.metaKey === true)) ||
			// Allow: home, end, left, right
			 (e.keyCode >= 35 && e.keyCode <= 39)) {
			// let it happen, don't do anything
			return;
		}
		// Ensure that it is a number and stop the keypress
		if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
			e.preventDefault();
		}
	});

	// Hide the Header, as VesselSettingsEditor provides one
	$('.modal-header').hide();

	FMErrorAndExceptionHandling.CloseNotifications();
});

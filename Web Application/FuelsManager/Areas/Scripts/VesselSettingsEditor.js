//debugger;

// create a class with helper functions for the vessel settings editor
var FMVesselSettingsEditor = function () {

	var _stack_bottomright_vesselsettings = { "dir1": "up", "dir2": "left", "firstpos1": 75, "firstpos2": 25, "context": $("#ModulePropertyEditorPropertyScreen") };

	var _SaveChanges= function(){
		var url = $('#urlSaveVesselSettings').val();
		var token = $('#VesselSettingsEditorForm input[name =__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		var messageAttributes = { addclass: "stack-bottomright", stack: FMVesselSettingsEditor.stack_bottomright_vesselsettings, width: "450px" };
		// remove any notification
		PNotify.removeStack(FMVesselSettingsEditor.stack_bottomright_vesselsettings);

		$.ajax({
			url: url,
			type: 'post',
			headers: headers,
			data: 'IsTemplatePoint=' + $('[name=IsTemplatePoint]').val() + '&PointGuid=' + $('[name=PointGuid]').val() + '&PointPropertyGuid=' + $('[name=PointPropertyGuid]').val() + FMPointEditor.serializeSettings("vesselSettingsEditorPartial"),
			success: function (result) {
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError) {	},
					 messageAttributes);
			},
			error:
				function (request, status, error) {
					FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				}
		});
	}

	var _SetCoefficientReadOnly = function () {
		var tankMaterial = parseInt($('#TankMaterialDropDownList').val());
		var readonly = true;
		if(tankMaterial === 0
		|| tankMaterial === 6) {
			readonly = false;
		}
		$('#TankExpansionCoefficient').prop('readonly', readonly);
		$('#AreaCoefficient').prop('readonly', readonly);
	}

	var _TankMaterialDropDownListChanged = function () {
		var tankMaterial = parseInt($('#TankMaterialDropDownList').val());
		var temperatureUnit = parseInt($('[name=TemperatureUnit]').val());
		var expansionCoefficient = 0;
		var areaCoefficient = 0;
		switch(tankMaterial)
		{
			case 0: // Unknown
				expansionCoefficient = 0;
				areaCoefficient = 0;
				break;

			case 1: // Mild Carbon
				if (temperatureUnit == 1 || temperatureUnit == 3) {
					expansionCoefficient = 1.12E-05;
				}
				else {
					expansionCoefficient = 6.20E-06;
				}
				areaCoefficient = 4.0e-009;
				break;

			case 2: // 304 StanlessSteel
				if (temperatureUnit == 1 || temperatureUnit == 3) {
					expansionCoefficient = 1.73E-05;
				}
				else {
					expansionCoefficient = 9.60E-06;
				}
				areaCoefficient = 4.0e-009;
				break;

			case 3: // 316 StanlessSteel
				if (temperatureUnit == 1 || temperatureUnit == 3) {
					expansionCoefficient = 1.59E-05;
				}
				else {
					expansionCoefficient = 8.83E-06;
				}
				areaCoefficient = 4.0e-009;
				break;

			case 4: // 17-4PH StanlessSteel
				if (temperatureUnit == 1 || temperatureUnit == 3) {
					expansionCoefficient = 1.08E-05;
				}
				else {
					expansionCoefficient = 6.00E-06;
				}
				areaCoefficient = 4.0e-009;
				break;


			case 5: // Aluminum
				if (temperatureUnit == 1 || temperatureUnit == 3) {
					expansionCoefficient = 23.0E-06
				}
				else {
					expansionCoefficient = 12.8E-06
				}
				areaCoefficient = 4.0e-009;
				break;

			case 6: // Other
				expansionCoefficient = 0;
				areaCoefficient = 0;
				break;

			default:
				break;

			
		}

		if (expansionCoefficient != 0) {
			expansionCoefficient = expansionCoefficient.toExponential().replace("e", "E");
		}

		if (areaCoefficient != 0) {
			areaCoefficient = areaCoefficient.toExponential().replace("e", "E");
		}

		$('#TankExpansionCoefficient').val(expansionCoefficient).attr('data-raw-value', expansionCoefficient);
		$('#AreaCoefficient').val(areaCoefficient).attr('data-raw-value', areaCoefficient);

		_SetCoefficientReadOnly();
	}

	//=============================================================================
	// This function will handle the vessel setup section selection event.
	//=============================================================================
	_DisplayVesselSetupSection = function () {
		$("#VesselMfrLocTab").removeClass("selected");
		$("#VesselSetupTab").addClass("selected");

		$("#VesselMfrLocItemBtag").removeClass("selected");
		$("#VesselSetupItemBtag").addClass("selected");

		$("#VesselMfrLocSection").addClass("hidden");
		$("#VesselSetupSection").removeClass("hidden");
	}

	//=============================================================================
	// This function will handle the vessel Mfr/Loc info section selection event.
	//=============================================================================
	_DisplayMfrLocSection = function () {
		$("#VesselMfrLocTab").addClass("selected");
		$("#VesselSetupTab").removeClass("selected");

		$("#VesselMfrLocItemBtag").addClass("selected");
		$("#VesselSetupItemBtag").removeClass("selected");

		$("#VesselMfrLocSection").removeClass("hidden");
		$("#VesselSetupSection").addClass("hidden");
	}

	//======================================================
	// Return function pointers
	//======================================================
	return {
		SaveChanges: _SaveChanges,
		TankMaterialDropDownListChanged: _TankMaterialDropDownListChanged,
		stack_bottomright_vesselsettings: _stack_bottomright_vesselsettings,
		SetCoefficientReadOnly: _SetCoefficientReadOnly,
		DisplayVesselSetupSection: _DisplayVesselSetupSection,
		DisplayMfrLocSection: _DisplayMfrLocSection,
	}

}();

// manually hookup to the submit the form
$(function () {
	$('#VesselSettingsEditorForm').submit(function () {
		var action = this.action;
		var method = this.method;

		FMVesselSettingsEditor.SaveChanges();

		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	});

});




//--------------------------------------- RUN after page has been loaded but before render -----------------------------

$(document).ready(function () {

	// Initialize the Settings
	$('#vesselSettingsEditorPartial [data-unit]').each(function (index, control) {
		FMPointEditor.formatSetting(control);
	});

	$('#TankMaterialDropDownList').change(function () {
		FMVesselSettingsEditor.TankMaterialDropDownListChanged();
	});

	// Initialize Coefficient Inputs
	FMVesselSettingsEditor.SetCoefficientReadOnly();
	
	// when one of the settings gets focus store the value so we can check if we have change it when losing focus
	$('#vesselSettingsEditorPartial').on("focus", '#VesselSettingsPropertyList > > [data-display=input] input', function (data) {
		$(this).data("oldValue", $(this).val());
		return true;
	});

	// change a setting a property value in general settings and lose focus( we need to get the new raw data and the formatted value)
	$("#vesselSettingsEditorPartial").on("blur", '#VesselSettingsPropertyList > > [data-display=input] input, fieldset td > [data-display=input] input', function (data) {
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
			$(this).attr('data-raw-value', formattedValue.toString());
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

		var curretRawValue = $(this).attr('data-raw-value');
		//Format Value
		var newFormattedValue = FMFormatValues.FormatValueFullPrecision(parseInt(unit), numformatInfo, newRawValue);
		$(this).val(newFormattedValue);

		return true;

	});


	// Hide the Header, as VesselSettingsEditor provides one
	$('.modal-header').hide();

	FMErrorAndExceptionHandling.CloseNotifications();
});

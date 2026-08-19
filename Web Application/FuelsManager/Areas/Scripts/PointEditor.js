// create a class with helper functions for the point editor
if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

var FMPointEditor = function () {
	math.config({ precision: 17 });
	var _valuesChanged = false;
	var _alarmValuesChanged = false;
	var _tagList = [];
	var localStorageKey = "FMPointEditor_";
	var pointGuid = "";
	var _unitConversionHistory = [];  // history of the changes to the default units between saves
	var _fceeInputOutputType = "4";

	var _notification_stack = FMErrorAndExceptionHandling.stack_bottomright;
	var _notification_class = "stack-bottomright ui-pnotify-translucent";

	// notifications for the point selector on the point editor screen
	// we are goign to display error messages inside the menu 
	//(note that we have to declare the contents of this variable in the onready event since this code is executed before the DOM is created and PointSettingListRolloutMenu does not exists yet
	var _notification_pointselector_stack = { "dir1": "up", "dir2": "left", "context": $("#PointSettingListRolloutMenu") };
	var _notification_imageselector_stack = { "dir1": "up", "dir2": "left", "context": $("#PointTemplateImageMenu") };

	var _getDefaultControlUnitsType = function(controlID)
	{
		if(controlID.indexOf("Level"))
		{
			return parseInt($('#PETemperatureEngineeringUnitsType').val());

		}
	}

	var _updateServerUnitControlState = function (row) {
		var serverUnitSelect = row.find('.tagColumnServerUnits select');
		if (serverUnitSelect.length === 0) {
			return;
		}

		var canModify = $('#ModifyEnabled').length >  0 && $('#ModifyEnabled').val().toLowerCase() !== 'false';
		var hasFceeRight = $('#HasFCEERight').length > 0 && $('#HasFCEERight').val().toLowerCase() === 'true';
		var hasModifyFceeRight = $('#HasModifyFCEERight').length > 0 && $('#HasModifyFCEERight').val().toLowerCase() === 'true';
		var canViewServerUnit = hasFceeRight && row.find('.tagColumnServerUnits').css('display') !== 'none';
		var isFceeDataSource = row.find('.tagColumnInputOutputType select').val() === _fceeInputOutputType;
		serverUnitSelect.prop('disabled', !(canModify && canViewServerUnit && hasModifyFceeRight && isFceeDataSource));
	}

	var _getDefaultUnits = function (unitType) {
		switch (unitType)
		{
			case "FmuTemp": // Temperature
				return parseInt($('#PETemperatureEngineeringUnits').val());
			case "FmuLength": // Level
				return parseInt($('#PELevelEngineeringUnits').val());
			case "FmuVolume": // Volume
				return parseInt($('#PEVolumeEngineeringUnits').val());
			case "FmuMass": // Mass
				return parseInt($('#PEMassEngineeringUnits').val());
			case "FmuPressure": // Pressure
				return parseInt($('#PEPressureEngineeringUnits').val());
			case "FmuVolflow": // Volumetric Flow
				return parseInt($('#PEVolumetricFlowEngineeringUnits').val());
			case "FmuMassflow": // Mass Flow
				return parseInt($('#PEMassFlowEngineeringUnits').val());
			case "FmuVelocity": // Velocity
				return parseInt($('#PEVelocityEngineeringUnits').val());
			case "FmuDensity": // Density
				return parseInt($('#PEDensityEngineeringUnits').val());
			case "FmuElect": // Electrical
				return parseInt($('#PEElectricalEngineeringUnits').val());
			default:
				return 0;
		}
	}

	var _getDefaultUnitsControl = function (unitType) {
		switch (unitType) {
			case "FmuTemp": // Temperature
				return $('#PETemperatureEngineeringUnits');
			case "FmuLength": // Level
				return $('#PELevelEngineeringUnits');
			case "FmuVolume": // Volume
				return $('#PEVolumeEngineeringUnits');
			case "FmuMass": // Mass
				return $('#PEMassEngineeringUnits');
			case "FmuPressure": // Pressure
				return $('#PEPressureEngineeringUnits');
			case "FmuVolflow": // Volumetric Flow
				return $('#PEVolumetricFlowEngineeringUnits');
			case "FmuMassflow": // Mass Flow
				return $('#PEMassFlowEngineeringUnits');
			case "FmuVelocity": // Velocity
				return $('#PEVelocityEngineeringUnits');
			case "FmuDensity": // Density
				return $('#PEDensityEngineeringUnits');
			//case "FmuElect": // Electrical
			//	return $('#PEElectricalEngineeringUnits');
			default:
				return null;
		}
	}


	var _getDefaultDecimalPlaces = function (unitType) {
		switch (unitType) {
			case "FmuAll": // All
				return 6;
			case "FmuTemp": // Temperature
				return $('#PETemperatureDecimalPlaces').val();
			case "FmuLength": // Level
				return $('#PELevelDecimalPlaces').val();
			case "FmuVolume": // Volume
				return $('#PEVolumeDecimalPlaces').val();
			case "FmuMass": // Mass
				return $('#PEMassDecimalPlaces').val();
			case "FmuPressure": // Pressure
				return $('#PEPressureDecimalPlaces').val();
			case "FmuVolflow": // Volumetric Flow
				return $('#PEVolumetricFlowDecimalPlaces').val();
			case "FmuMassflow": // Mass Flow
				return $('#PEMassFlowDecimalPlaces').val();
			case "FmuVelocity": // Velocity
				return $('#PEVelocityDecimalPlaces').val();
			case "FmuDensity": // Density
				return $('#PEDensityDecimalPlaces').val();
			default:
				return "0";
		}
	}


	var _getDefaultMinimum = function (unitType) {
		switch (unitType) {
			case "FmuTemp": // Temperature
				return $("#PETemperatureMinimum").attr('data-raw-value');
			case "FmuLength": // Level
				return $("#PELevelMinimum").attr('data-raw-value');
			case "FmuVolume": // Volume
				return $("#PEVolumeMinimum").attr('data-raw-value');
			case "FmuMass": // Mass
				return $("#PEMassMinimum").attr('data-raw-value');
			case "FmuPressure": // Pressure
				return $("#PEPressureMinimum").attr('data-raw-value');
			case "FmuVolflow": // Volumetric Flow
				return $("#PEVolumetricFlowMinimum").attr('data-raw-value');
			case "FmuMassflow": // Mass Flow
				return $("#PEMassFlowMinimum").attr('data-raw-value');
			case "FmuVelocity": // Velocity
				return $("#PEVelocityMinimum").attr('data-raw-value');
			case "FmuDensity": // Density
				return $("#PEDensityMinimum").attr('data-raw-value');
			default:
				return 0;
		}
	}


	var _getDefaultMaximum = function (unitType) {
		switch (unitType) {
			case "FmuTemp": // Temperature
				return $("#PETemperatureMaximum").attr('data-raw-value');
			case "FmuLength": // Level
				return $("#PELevelMaximum").attr('data-raw-value');
			case "FmuVolume": // Volume
				return $("#PEVolumeMaximum").attr('data-raw-value');
			case "FmuMass": // Mass
				return $("#PEMassMaximum").attr('data-raw-value');
			case "FmuPressure": // Pressure
				return $("#PEPressureMaximum").attr('data-raw-value');
			case "FmuVolflow": // Volumetric Flow
				return $("#PEVolumetricFlowMaximum").attr('data-raw-value');
			case "FmuMassflow": // Mass Flow
				return $("#PEMassFlowMaximum").attr('data-raw-value');
			case "FmuVelocity": // Velocity
				return $("#PEVelocityMaximum").attr('data-raw-value');
			case "FmuDensity": // Density
				return $("#PEDensityMaximum").attr('data-raw-value');
			default:
				return 0;
		}
	}

	var _setTagUnitsFromDefaultUnits = function (row) {
		FMPointEditor.unitsChanged(row.find(".tagColumnUnits select"))
	}

	var _setTagDecimalPlacesFromDefaultDecimalPlaces = function (row) {
		var unitsType = row.find(".tagColumnUnitType select").val();
		var decimalPlaces = FMPointEditor.getDefaultDecimalPlaces(unitsType);
		row.find('.tagColumnDecimalPlaces input').val(decimalPlaces);
		
		var rowIndex = row.find('.column-tag-name').attr('id').replace('Tags_', '').replace('__Name', '');
		FMPointEditor.tagList[rowIndex].DecimalPlaces = parseInt(decimalPlaces);
	}

	var _setTagMinimumFromDefaultMinimum = function (row) {
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);
		numformatInfo.NumberDecimalDigits = parseInt(row.find('.tagColumnDecimalPlaces input').val());
		var unitIndex = parseInt(row.find(".tagColumnUnits select").val());
		var unitType = row.find(".tagColumnUnitType select").val();
		var defUnitIndex = FMPointEditor.getDefaultUnits(unitType);
		
		var newRawDataValue;
		if ((defUnitIndex === 191 && unitIndex !== 191)
		|| (defUnitIndex !== 191 && unitIndex === 191)) {
			newRawDataValue = FMPointEditor.getDefaultMaximum(unitType);
		}
		else {
			newRawDataValue = FMPointEditor.getDefaultMinimum(unitType);
		}
		var minimum = FMConvertEngUnits.Convert(newRawDataValue, defUnitIndex, unitIndex);
		row.find('.tagColumnMinimum input').attr('data-raw-value', minimum.toString());
		var newMinimumFormattedValue = FMFormatValues.FormatValueFullPrecision(unitIndex, numformatInfo, minimum);
		row.find('.tagColumnMinimum input').val(newMinimumFormattedValue);
		_resetTagInputMask(row.find('.tagColumnMinimum input').attr('id'));

		var rowIndex = row.find('.column-tag-name').attr('id').replace('Tags_', '').replace('__Name', '');
		FMPointEditor.tagList[rowIndex].Minimum = math.number(newRawDataValue);

	}


	var _setTagMinimum = function (row) {
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);
		numformatInfo.NumberDecimalDigits = parseInt(row.find('.tagColumnDecimalPlaces input').val());
		var unitIndex = parseInt(row.find(".tagColumnUnits select").val());
		var rawDataValue = row.find('.tagColumnMinimum input').attr('data-raw-value');
		var newMinimumFormattedValue = FMFormatValues.FormatValueFullPrecision(unitIndex, numformatInfo, rawDataValue);
		row.find('.tagColumnMinimum input').val(newMinimumFormattedValue);

		var rowIndex = row.find('.column-tag-name').attr('id').replace('Tags_', '').replace('__Name', '');
		FMPointEditor.tagList[rowIndex].Minimum = math.number(rawDataValue);
	}


	var _setTagMaximumFromDefaultMaximum = function (row) {
		
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);
		numformatInfo.NumberDecimalDigits = parseInt( row.find( '.tagColumnDecimalPlaces input' ).val() );
		var unitIndex = parseInt(row.find(".tagColumnUnits select").val());
		var unitType = row.find(".tagColumnUnitType select").val();
		var defUnitIndex = FMPointEditor.getDefaultUnits(unitType);
		var newRawDataValue;
		if ((defUnitIndex === 191 && unitIndex !== 191)
		|| (defUnitIndex !== 191 && unitIndex === 191)) {
			newRawDataValue = FMPointEditor.getDefaultMinimum(unitType);
		}
		else {
			newRawDataValue = FMPointEditor.getDefaultMaximum(unitType);
		}
		var maximum = FMConvertEngUnits.Convert(newRawDataValue, defUnitIndex, unitIndex);
		row.find('.tagColumnMaximum input').attr('data-raw-value', maximum.toString());
		var newMaximumFormattedValue = FMFormatValues.FormatValueFullPrecision(unitIndex, numformatInfo, maximum);
		row.find('.tagColumnMaximum input').val(newMaximumFormattedValue);
		_resetTagInputMask(row.find('.tagColumnMaximum input').attr('id'));

		var rowIndex = row.find('.column-tag-name').attr('id').replace('Tags_', '').replace('__Name', '');
		FMPointEditor.tagList[rowIndex].Maximum = math.number(newRawDataValue);

	}


	var _setTagMaximum = function (row) {
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);
		numformatInfo.NumberDecimalDigits = parseInt(row.find('.tagColumnDecimalPlaces input').val());
		var unitIndex = parseInt(row.find(".tagColumnUnits select").val());
		var rawDataValue = row.find('.tagColumnMaximum input').attr('data-raw-value');
		var newMaximumFormattedValue = FMFormatValues.FormatValueFullPrecision(unitIndex, numformatInfo, rawDataValue);
		row.find('.tagColumnMaximum input').val(newMaximumFormattedValue);

		var rowIndex = row.find('.column-tag-name').attr('id').replace('Tags_', '').replace('__Name', '');
		FMPointEditor.tagList[rowIndex].Maximum = math.number(rawDataValue);
	}

	var _unitsChanged = function (control){
		FMPointEditor.valuesChanged = true;

		var numDecimals = control.closest('tr').find('.tagColumnDecimalPlaces input').val();
		numDecimals = (isNaN(numDecimals)) ? numDecimals = 0 : ~~Number(numDecimals);

		var minimum = control.closest('tr').find('.tagColumnMinimum input').attr('data-raw-value');
		minimum = (isNaN(minimum)) ? math.bignumber(0) : math.bignumber(minimum);
		var minimumId = control.closest('tr').find('.tagColumnMinimum input').attr("id");

		var maximum = control.closest('tr').find('.tagColumnMaximum input').attr('data-raw-value');
		maximum = (isNaN(maximum)) ? math.bignumber(0) : math.bignumber(maximum);
		var maximumId = control.closest('tr').find('.tagColumnMaximum input').attr("id");
		var oldUnitIndex = control.data("prev");
		oldUnitIndex = (isNaN(oldUnitIndex)) ? 0 : ~~Number(oldUnitIndex);
		var newUnitIndex = control.val();
		newUnitIndex = (isNaN(newUnitIndex)) ? 0 : ~~Number(newUnitIndex);

		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);
		numformatInfo.NumberDecimalDigits = numDecimals;
		
		var newMimimumRawValue = FMConvertEngUnits.Convert(minimum, oldUnitIndex, newUnitIndex);
		var newMimimumFormattedValue = FMFormatValues.FormatValueFullPrecision(newUnitIndex, numformatInfo, newMimimumRawValue);

		var newMaximumRawValue = FMConvertEngUnits.Convert(maximum, oldUnitIndex, newUnitIndex);
		var newMaximumFormattedValue = FMFormatValues.FormatValueFullPrecision(newUnitIndex, numformatInfo, newMaximumRawValue);
		var rowId = parseInt(control.attr('id').replace('Tags_', '').replace('__Unit', ''));

		//If switching from or to Degrees API then swap values else leave them as they are.
		if ((oldUnitIndex === 191 && newUnitIndex !== 191)
		|| (oldUnitIndex !== 191 && newUnitIndex === 191)) {
			FMPointEditor.resetTagInputMask($('#' + minimumId));
			$('#' + minimumId).val(newMaximumFormattedValue); //Max Value in In Min Field
			$('#' + minimumId).attr('data-raw-value', newMaximumRawValue.toString()); //Max Raw Value in Min Raw Value
			FMPointEditor.tagList[rowId].Minimum = math.number(newMaximumRawValue);

			FMPointEditor.resetTagInputMask($('#' + maximumId));
			$('#' + maximumId).val(newMimimumFormattedValue); //Min Value in In Max Field
			$('#' + maximumId).attr('data-raw-value', newMimimumRawValue.toString()); //Min Value in Max Field
			FMPointEditor.tagList[rowId].Maximum = math.number(newMimimumRawValue);
		} else {
			FMPointEditor.resetTagInputMask($('#' + minimumId));
			$('#' + minimumId).val(newMimimumFormattedValue);
			$('#' + minimumId).attr('data-raw-value', newMimimumRawValue.toString());
			FMPointEditor.tagList[rowId].Minimum = math.number(newMimimumRawValue);

			FMPointEditor.resetTagInputMask($('#' + maximumId));
			$('#' + maximumId).val(newMaximumFormattedValue);
			$('#' + maximumId).attr('data-raw-value', newMaximumRawValue.toString());
			FMPointEditor.tagList[rowId].Maximum = math.number(newMaximumRawValue);
		}

		var valueControl = control.closest('tr').find('.tagColumnValue input');
		if (valueControl.length > 0
		&& valueControl.attr('data-raw-value') != '') {
			var value = valueControl.attr('data-raw-value');
			value = (isNaN(value)) ? math.bignumber(0) : math.bignumber(value);

			var newRawValue = FMConvertEngUnits.Convert(value, oldUnitIndex, newUnitIndex);
			var newFormattedValue = FMFormatValues.FormatValueFullPrecision(newUnitIndex, numformatInfo, newRawValue);

			valueControl.val(newFormattedValue);
			valueControl.attr('data-raw-value', newRawValue.toString());
		}

		// Update ServerUnits if have not been customized
		if (FMPointEditor.tagList[rowId].ServerUnits == FMPointEditor.tagList[rowId].Units) {
			FMPointEditor.tagList[rowId].ServerUnits = newUnitIndex;
		}
		FMPointEditor.tagList[rowId].Units = newUnitIndex;
		control.data("prev", control.val());
	}

	var _decimalPlacesChanged = function (control) {
		FMPointEditor.valuesChanged = true;
		var numDecimals = ~~Number(control.val());

		
		// find the unit for the tag, if it's ft-in-16th or ft-in-8th we can ignore the decimals
		var unit = control.closest('tr').find('.tagColumnUnits').find('select').val();
		if (unit == '27' || unit == '19') {
			return true;
		}

		// update the minimum
		var minimum = control.closest('tr').find('.tagColumnMinimum').find('input');
		if (!isNaN(parseFloat(minimum.attr('data-raw-value')))) {
			minimum.val(math.bignumber(minimum.attr('data-raw-value')).toFixed(numDecimals));
			FMPointEditor.resetTagInputMask(minimum);
		}

		// update the maximum
		var maximum = control.closest('tr').find('.tagColumnMaximum').find('input');
		if (!isNaN(parseFloat(maximum.attr('data-raw-value')))) {
			maximum.val(math.bignumber(maximum.attr('data-raw-value')).toFixed(numDecimals));
			FMPointEditor.resetTagInputMask(maximum);
		}

		// update the value
		var valueControl = control.closest('tr').find('.tagColumnValue input');
		if (valueControl.length > 0
		&& valueControl.attr('data-raw-value') != '') {
			var value = valueControl.attr('data-raw-value');
			valueControl.val(math.bignumber(valueControl.attr('data-raw-value')).toFixed(numDecimals));
		}


		var decimalPlaces = control.closest('tr').find('.tagColumnDecimalPlaces').find('input');
		var rowId = parseInt(decimalPlaces.attr('id').replace('Tags_', '').replace('__DecimalPlaces', ''));
		FMPointEditor.tagList[rowId].DecimalPlaces = parseInt(numDecimals);

		FMPointEditor.valuesChanged = true;

	}

	var _minimumOrMaximumChanged = function (control) {

		
		var unit = control.closest('tr').find('.tagColumnUnits select').val();
		var numDecimals = ~~Number(control.closest('tr').find('.tagColumnDecimalPlaces input').val());
		var unitIndex = (isNaN(unit)) ? ~~0 : ~~Number(unit);

		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);
		numformatInfo.NumberDecimalDigits = numDecimals;
		var formattedValue = control.val();
		var controlId = control.attr("id");
		
		//Parse Value
		var newRawValueRounded = math.bignumber(FMFormatValues.ParseValue(unitIndex, numformatInfo, formattedValue, true));
		var currentRawValue = math.bignumber($('#' + controlId).attr('data-raw-value'));
		var currentRawValueRounded = math.bignumber(currentRawValue.toFixed(numformatInfo.NumberDecimalDigits));
		if (!newRawValueRounded.equals(currentRawValueRounded)) {
			$('#' + controlId).attr('data-raw-value', newRawValueRounded.toString());

			if (controlId.indexOf('__Maximum') != -1) {
				
				var rowId = parseInt(controlId.replace('Tags_', '').replace('__Maximum', ''));
				FMPointEditor.tagList[rowId].Maximum = math.number($('#' + controlId).attr('data-raw-value'));
				numformatInfo.NumberDecimalDigits = 9;
			}
			else {
				var rowId = parseInt(controlId.replace('Tags_', '').replace('__Minimum', ''));
				FMPointEditor.tagList[rowId].Minimum = math.number($('#' + controlId).attr('data-raw-value'));
				numformatInfo.NumberDecimalDigits = 9;
			}
			FMPointEditor.valuesChanged = true;
		}

		//Format Value
		var newFormattedValue = FMFormatValues.FormatValueFullPrecision(unitIndex, numformatInfo, newRawValueRounded);
		$('#' + controlId).val(newFormattedValue);
	}



	var _initializeTagGrid = function () {
		$('#TagEditTable tr').each(function () {
			if ( $( this ).find( 'td.tagColumnDataType select' ).length > 0 )
			{
				var dataType = $( this ).find( 'td.tagColumnDataType select' ).val();
			}
			else
			{
				var dataType = $( this ).find( 'td.tagColumnDataType' )[0].innerHTML;
			}
			if (!_isTagNumeric(dataType)
			&& dataType !== 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference') {
				$(this).find('td.tagColumnDecimalPlaces input').addClass( 'hidden' );
				$(this).find('td.tagColumnMinimum input').addClass('hidden');
				$(this).find('td.tagColumnMaximum input').addClass('hidden');
			}

			if ($(this).find("td.tagColumnAssociations img[name='alarm']").attr('data-value') === 'true'
			|| $(this).find("td.tagColumnAssociations img[name='module']").attr('data-value') === 'true') {
				$(this).find('td.tagColumnInput img').addClass("disabled").prop('disabled', true);
			}
		});

		// initialize the Grid for the default Units (Settings menu)
		$(".tagDefaultMinimum, .tagDefaultMaximum").each(function (index, value) {
			FMPointEditor.resetDefaultInputMask($(this));
		});

		// initialize the mask for the precision so we don't allow characters or negative values
		$(".tagDefaultDecimalPlaces, .tagColumnDecimalPlaces").each(function (index, value) {
			$(this).removeNumeric(); // remove numeric mask if there was one
			$(this).numeric({
				decimal: false
				,negative: false
			});
		});

		// record the original unit in case we change it and need to do a conversion
		$(".tagDefaultUnits").each(function (index, value) {
				$(this).data("prev", $(value).val());
		});

		// change the tag unit
		$(".tagColumnUnits select").bind('change', function (data) {
			FMPointEditor.unitsChanged($(this));
		});

		// the user changes the precision in the tag grid
		$(".tagColumnDecimalPlaces input").on('input', function (data) {
			FMPointEditor.decimalPlacesChanged($(this));
		});

		// change the min or max ( we need to get the new raw data and the formatted value)
		$(".tagColumnMinimum input, .tagColumnMaximum input").on("blur", function (data) {
			FMPointEditor.minimumOrMaximumChanged($(this));
		});

		$("#PEfilter_tags").on('input', function () {

			var filterTxt = $("#PEfilter_tags").val();

			if (filterTxt == "") {
				$('#TagEditTable > tbody > tr').each(function () {
					$(this).removeClass("hidden");
				});
			} else {
				$('#TagEditTable > tbody > tr').each(function () {
					if ($("#IsTemplatePoint").val() == 'True') {
						if ($(this).find('.column-tag-name').val().toLowerCase().indexOf(filterTxt.toLowerCase()) >= 0) {
							$(this).removeClass("hidden");
						} else {
							$(this).removeClass("hidden").addClass("hidden");
						}
					}
					else {
						if ($(this).find('.column-tag-name').text().toLowerCase().indexOf(filterTxt.toLowerCase()) >= 0) {
							$(this).removeClass("hidden");
						} else {
							$(this).removeClass("hidden").addClass("hidden");
						}
					}
				});
			}
		});

		// initialize the Grid for the Tags (Tags menu)
		$.each(FMPointEditor.tagList, function (index, value) {
			// hide the Units drop down if there is nothing to select
			if (value.EngineeringUnitsType == 'FmuNone' || value.EngineeringUnitsType == 'FmuNodim') {
				$('#Tags_' + value.index + '__Unit').addClass( "hidden");
				$('#Tags_' + value.index + '__ServerUnit').addClass("hidden");
			}

			$('#Tags_' + value.index + '__Unit').val(value.Units);
			// take each select for units and store the initial value so we know what we are changing to for unit conversion
			$('#Tags_' + value.index + '__Unit').data("prev", value.Units);
			$('#Tags_' + value.index + '__ServerUnit').val(value.ServerUnits);
			// take each select for units and store the initial value so we know what we are changing to for unit conversion
			$('#Tags_' + value.index + '__ServerUnit').data("prev", value.ServerUnits);

			$('#Tags_' + value.index + '__InputOutputType').val(value.InputOutputType);
			_updateServerUnitControlState($('#Tags_' + value.index + '__InputOutputType').closest('tr'));

			FMPointEditor.resetTagInputMask($('#Tags_' + value.index + '__Minimum'));
			FMPointEditor.resetTagInputMask($('#Tags_' + value.index + '__Maximum'));

			//apply formatting for the first time on the minimum and maximum input fields
			FMPointEditor.minimumOrMaximumChanged($('#Tags_' + value.index + '__Minimum'));
			FMPointEditor.minimumOrMaximumChanged($('#Tags_' + value.index + '__Maximum'));
		});

		$("#TagEditTableWrap").niceScroll({
			cursorwidth: '10px'
			, autohidemode: false
			, cursorcolor: "#486899"
			, background: "rgb(240, 240, 240)"
		});

		// changing the input output of a tag. For inuts, only Calculated tags will show the icon configuration icon disabled
		$('.tagColumnInputOutputType select').on('change', function () {
			FMPointEditor.valuesChanged = true;
			var row = $(this).closest('tr');
			var rowIndex = row.find('.column-tag-name').attr('id').replace('Tags_', '').replace('__Name', '');
			var input =	FMPointEditor.tagList[rowIndex].Input;
			FMPointEditor.tagList[rowIndex].InputOutputType = $(this).val();
			_updateServerUnitControlState(row);

			if ($(this).val() == 3 || !input) {
				$(this).parent().parent().find('.tagColumnOpcConnectionEditor').find('span').removeClass('disabledIcon');
			}
			else {
				var iconTag = $(this).parent().parent().find('.tagColumnOpcConnectionEditor').find('span');
				if (!iconTag.hasClass('disabledIcon')) {
					iconTag.addClass('disabledIcon');
				}
			}
		});
	}

	var _initializeModuleSettings = function () {

		// sort the list of settings
		var settings = $('#PEModulePropertyList > div').get();
		settings.sort(function (a, b) {
				var keyA = $(a).find('label:first-of-type').text().toLowerCase();
				var keyB = $(b).find('label:first-of-type').text().toLowerCase();

				if (keyA < keyB) return -1;
				if (keyA > keyB) return 1;
				return 0;
		});

		// Clear the original list.
		$('#PEModulePropertyList > div').remove();

		var ul = $('#PEModulePropertyList');
		$.each(settings, function (i, li) {
				ul.append(li);
		});

		// set the stripes in the 2 column list
		var first_column = Math.min.apply(null, $('#PEModulePropertyList > div').map(function (i, obj) { return $(obj).offset().left }));
		var second_column = Math.max.apply(null, $('#PEModulePropertyList > div').map(function (i, obj) { return $(obj).offset().left }));

		// loop through all the divs (entries) and add the class 'settings-stripes' every other column
		var i = 0;
		var last_column_checked = first_column;
		$('#PEModulePropertyList > div:not(.hidden)').each(function (idx, obj) {
				// are we in the same column or we are switching to the second column
				if ($(obj).offset().left == last_column_checked) {
					if (i % 2 == 0) $(obj).addClass("settings-stripes");
					i++;
				}
				else {
					i = 1;
					last_column_checked = second_column;
					$(obj).addClass("settings-stripes");
				}

		});
	

		//format values from the raw-data
		$('#PEModulePropertyList [data-unit]').each(function (index, control) {
			if ($(control).attr("type") === "text") {
				_formatSetting(control);
			}
			else if ($(control).attr("type") === "label") {
				_setSettingUnits(control);
			}
		});

		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numFormatInfo = JSON.parse(numFormatInfoString);
		FMLayout.dateFormat = ConvertToJQueryUIDateFormat(numFormatInfo.ShortDatePattern);
		FMLayout.timeFormat = "hh:mm:ss tt";
		if (numFormatInfo.TimePattern != null && numFormatInfo.TimePattern !== "" && typeof (numFormatInfo.TimePattern) != "undefined") {
			FMLayout.timeFormat = ConvertToJQueryUITimeFormat(numFormatInfo.TimePattern);
		}


		FMLayout.calendarLocation = window.applicationRootName + '/dispatchwebapp/images';

		$('#PEModulePropertyList > div').each(function () {
			switch ($(this).attr('data-display')) {
				case "calendar":
					$(this).find('input').datepicker({
						buttonImage: FMLayout.calendarLocation + '/calendar.gif',
						buttonImageOnly: true,
						showOn: "button",
						dateFormat: FMLayout.dateFormat,
					});

					break;

				case "clockcalendar":
					$(this).find('input').datetimepicker({
						buttonImage: FMLayout.calendarLocation + '/calendar.gif',
						buttonImageOnly: true,
						showOn: "button",
						dateFormat: FMLayout.dateFormat,
						timeFormat: FMLayout.timeFormat,
						showSecond: (FMLayout.timeFormat.indexOf('ss') === -1) ? false : true
					});

					break;

				case "timespan":
					var id = $(this).find('input').attr('id');
					$('#' + id).mask('###.00:00:00', { reverse: true, placeholder: "__:__:__" });
					break;

				default:
					break;
			}
		});

		// NOTE, I have to bind the chnage event because the input event doesn't seem to be working with checkboxes and radio buttons
		$(".mainPage").on("change", ".PESettingPanel input", function () {
			FMPointEditor.valuesChanged = true;
		});

	}

	var _setSettingUnits = function (control) {
		// if we are not using default units no need to format
		if ($(control).attr("data-unit") === "PENoneEngineeringUnits") {
			return true;
		}

		var unit = $("#" + $(control).attr("data-unit")).val();  // get the unit used by the unit type selected
		var unitText = $("#" + $(control).attr("data-unit") + " option[value= '" + unit + "']").text();
		var settingText = $(control).text();
		var unitIndex = settingText.indexOf('(');
		if (unitIndex === -1) {
			$(control).text(settingText + ' (' + unitText + ')');
		}
		else {
			$(control).text(settingText.substring(0,unitIndex) + ' (' + unitText + ')');
		}
	}

	var _formatSetting = function (control) {
		
		// if we are not using default units no need to format
		if ($(control).attr("data-unit") === "PENoneEngineeringUnits") {
			return true;
		}
		
		$(control).removeNumeric(); // remove numeric mask if there was one
		$(control).unmask(); // if it had a mask remove it

		var numDecimalsControl = $(control).attr("data-format");
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);
		if (numDecimalsControl != 'PESystemDecimalPlaces') {
			numformatInfo.NumberDecimalDigits = ~~Number($("#" + numDecimalsControl).val());
		}

		numformatInfo.NumberDecimalDigits = 9;

		// update the value
		var unit = $("#" + $(control).attr("data-unit")).val();  // get the unit used by the unit type selected
		var rawFloat = FMFormatValues.ParseValue(parseInt(unit), numformatInfo, $(control).attr('data-raw-value'), true);
		var newFormattedValue = FMFormatValues.FormatValueFullPrecision(parseInt(unit), numformatInfo, rawFloat);
		$(control).val(newFormattedValue);
		FMPointEditor.resetSettingInputMask($(control));

		return true;
	}
	

	var _saveChangesSuccessful = function (actionOnSuccessful, inError) {
			// hide the saving animation
		$(".loadingDiv").remove();

		// to determine if we have a succesful save we can check if the error panel is displayed
		if (!inError) {
			// reset the unit conversion history
			FMPointEditor.unitConversionHistory = [];

			FMPointEditor.valuesChanged = false;
			actionOnSuccessful();
		}
	}

	var _getTagTable = function()
	{
		var tagTable = [];
		$('#TagEditTable tr').each(function () {
			if ($(this).find('td.tagColumnID').length > 0) {
				tagTable.push({
					PointTagGuid: $(this).find('td.tagColumnPointTagGuid').text().trim(),
					Name: ($(this).find('td.tagColumnID input').length > 0 ? $(this).find('td.tagColumnID input').val() : $(this).find('td.tagColumnID').text().trim()),
					Unit: $(this).find('td.tagColumnUnits select').prop('value') == "" ? "255" : $(this).find('td.tagColumnUnits select').prop('value'),
					DecimalPlaces: $(this).find('td.tagColumnDecimalPlaces input').val(),
					ServerUnit: $(this).find('td.tagColumnServerUnits select').prop('value') == "" ? "255" : $(this).find('td.tagColumnServerUnits select').prop('value'),
					Minimum: encodeURIComponent($(this).find('td.tagColumnMinimum input').attr('data-raw-value')),
					Maximum: encodeURIComponent($(this).find('td.tagColumnMaximum input').attr('data-raw-value')),
					Value: JSON.stringify($(this).find('td.tagColumnValue input').attr('data-raw-value')),
					InputOutputType: $(this).find('td.tagColumnInputOutputType select').prop('value'),
					Input: $(this).find('td.tagColumnInput img').length > 0 ? $(this).find('td.tagColumnInput img').attr('data-value') : $(this).find('td.tagColumnInput').text().trim(),
					InhibitInputOutputTypeConfiguration: $(this).find('td.tagColumnInhibitInputOutputTypeConfiguration img').length > 0 ? $(this).find('td.tagColumnInhibitInputOutputTypeConfiguration img').attr('data-value') : $(this).find('td.tagColumnInhibitInputOutputTypeConfiguration').text().trim(),
					InhibitOverride: $(this).find('td.tagColumnInhibitOverride img').length > 0 ? $(this).find('td.tagColumnInhibitOverride img').attr('data-value') : $(this).find('td.tagColumnInhibitOverride').text().trim(),
					Archived: $(this).find('td.tagColumnArchived img').length > 0 ? $(this).find('td.tagColumnArchived img').attr('data-value') : $(this).find('td.tagColumnArchived').text().trim(),
					ApplyPointEngineeringUnits: $(this).find('td.tagColumnApplyPointEngineeringUnits img').length > 0 ? $(this).find('td.tagColumnApplyPointEngineeringUnits img').attr('data-value') : $(this).find('td.tagColumnApplyPointEngineeringUnits').text().trim(),
					ApplyPointDecimalPlaces: $(this).find('td.tagColumnApplyPointDecimalPlaces img').length > 0 ? $(this).find('td.tagColumnApplyPointDecimalPlaces img').attr('data-value') : $(this).find('td.tagColumnApplyPointDecimalPlaces').text().trim(),
					ApplyPointMinimum: $(this).find('td.tagColumnApplyPointMinimum img').length > 0 ? $(this).find('td.tagColumnApplyPointMinimum img').attr('data-value') : $(this).find('td.tagColumnApplyPointMinimum').text().trim(),
					ApplyPointMaximum: $(this).find('td.tagColumnApplyPointMaximum img').length > 0 ? $(this).find('td.tagColumnApplyPointMaximum img').attr('data-value') : $(this).find('td.tagColumnApplyPointMaximum').text().trim(),
					EngineeringUnitsType: $(this).find('td.tagColumnUnitType select').length > 0 ? $(this).find('td.tagColumnUnitType select').val() : $(this).find('td.tagColumnEngineeringUnits').text().trim(),
					DataType: $(this).find('td.tagColumnDataType select').length > 0 ? $(this).find('td.tagColumnDataType select').val() : $(this).find('td.tagColumnDataType').text().trim(),
					OpcUaNamespaceUri: $(this).find('td.tagColumnNamespace').text().trim(),
					OpcUaNodeId: $(this).find('td.tagColumnNode').text().trim(),
					OpcUaPublishingInterval: $(this).find('td.tagColumnInterval').length > 0 ? $(this).find('td.tagColumnInterval').text().trim() : 0,
					WellKnownIdentityGuidString: $(this).find('td.tagColumnWellKnownTag select').length > 0 ? $(this).find('td.tagColumnWellKnownTag select').val() : '00000000-0000-0000-0000-000000000000'
				});
			}
		});
		return tagTable;
	}
 
	var _saveChanges = function (action, method, actionOnSuccessful, deviceAlarmMap, tagGuid) {

		if (!AlarmEditor.ReadyForSubmit(FMPointEditor.getTagTable())) {
			return;
		}


		// hide any other notification
		FMErrorAndExceptionHandling.CloseNotifications();

		// display animation
		$('<div class=loadingDiv><img src="' + window.applicationRootName  + '/fmwebapp/images/loader_squares_120.gif" /></img></div>').prependTo(document.body);

		var notificationAttributes = { addclass: FMPointEditor.notification_class, stack: FMPointEditor.notification_stack, width: (FMPointEditor.notification_stack === FMErrorAndExceptionHandling.stack_bar_top || FMPointEditor.notification_stack === FMErrorAndExceptionHandling.stack_bar_bottom ? "100%" : "450px") };
        /*
         * PointsEdit(PointEditDetailModel model, string tagGrid, string assignedCategories, string productId, string settings, string unitConversionHistory, AlarmEditorModel alarmModel)
         */
		$.ajax( {
			url: action,
			type: method,
			data: ( function()
			{
				// serialize the form
				var serializedData = $( "#pointPropertiesForm" ).serialize();
				// Serialize the raw values from the default units (they are attributes of the control)
				$( '.tagDefaultMinimum, .tagDefaultMaximum' ).each( function()
				{
					var name = $( this ).attr( 'name' );
					serializedData = serializedData + '&' + name + "Raw=" + $( this ).attr( 'data-raw-value' );
				} );

				var categories = [];
				$( '#PEPointCategory option' ).each( function()
				{
					if ( this.selected )
					{
						categories.push( { Key: this.value, Value: this.text } );
					}
				} );


				// i want to serialize the table so MVC binds it automatically
				var tagTable = _getTagTable();
				var settings = [];
				// serialize the general settings
				$( '#PEModulePropertyList > div' ).each( function()
				{
					var value = null;
					switch ( $( this ).attr( 'data-display' ) )
					{
						case "input":
							// if we have a raw value pass the raw value otherwise just pass the value
							var rawValue = $( this ).find( 'input' ).attr( 'data-raw-value' );

							// For some browsers, `attr` is undefined; for others,
							// `attr` is false.  Check for both.
							if ( typeof rawValue !== typeof undefined && rawValue !== false )
							{
								value = rawValue;
								break;
							}
							value = $( this ).find( 'input' ).val();
							break;
						case "boolean":
							var name = $( this ).find( 'label:first' ).attr( 'for' );
							value = $( "input[type=radio][name=" + name + "]:checked" ).val();
							break;
						case "dropdown":
							value = $( this ).find( 'select' ).val();
							break;
						case "calendar":
							value = $( this ).find( 'input' ).val();
							break;
						case "clockcalendar":
							value = $(this).find('input').val();
							break;
						case "timespan":
							value = $(this).find('input').val();
							break;

					}
					if ( value !== null )
					{
						settings.push( {
							PointPropertyGuid: $( this ).attr( 'data-guid' ),
							Name: $( this ).find( 'label:first' ).attr( 'for' ),
							Value: value
						} );
					}
				});

				var productId = $('#PEPointProduct').val();
				if (!productId) {
					productId = '';
				}
				productId = productId.replace("<", "");
				productId = productId.replace(">", "");

				var ProfileImageGuid = $('#PointDetailImageContainer').attr("data-value");
				var overrideDefaultDrawingGuid = $('#PEPointOverrideDefaultDrawing').val();

				var alarmModel = AlarmEditor.GetAlarmEditorModelString();

				var moduleInstances = [];
				$('#PTTModuleList > li').each(function () {
					moduleInstances.push($(this).attr('data-target').replace('#', ''));
				});

				serializedData = serializedData
					+ "&AssignedCategories=" + encodeURIComponent( JSON.stringify( categories ))
					+ "&TagGrid=" + encodeURIComponent( JSON.stringify(tagTable))
               + "&ProductId=" + productId
               + "&ProfileImageGuid=" + ProfileImageGuid
					+ "&Settings=" + encodeURIComponent( JSON.stringify( settings ))
					+ "&UnitConversionHistory=" + encodeURIComponent( JSON.stringify(FMPointEditor.unitConversionHistory))
			      + "&AlarmModel=" + encodeURIComponent( alarmModel )
			      + "&OverrideDefaultDrawingGuidString=" + overrideDefaultDrawingGuid
					+ "&ModuleInstances=" + encodeURIComponent(JSON.stringify(moduleInstances))
					+ "&DeviceAlarmMap=" + encodeURIComponent(JSON.stringify(deviceAlarmMap))					
					+ "&TagGuid=" + encodeURIComponent(JSON.stringify(tagGuid));
				return serializedData;
			} )(),
			success: function( response )
			{

				FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
				{
					//check if there is a redirect key in the response
					if (data != null) {
						var redirectKey = $.grep(data, function (e) { return e.Key === "redirectGuid"; });
						if (redirectKey.length == 1 && redirectKey[0].Value !== "") {
							PointTemplateRedirect(redirectKey[0].Value);
						}
					}

					_saveChangesSuccessful(actionOnSuccessful, inError);

					// check if we got a new view for the modules
					if (data != null) {
						var moduleView = $.grep(data, function (e) { return e.Key === "modulesView"; });
						if (moduleView.length == 1 && moduleView[0].Value !== "") {
							//replace the modules view
							$("#PTTModuleEditHolder").html(moduleView[0].Value);
						}
					}
				}, notificationAttributes );
			},
			error: function( xhr, ajaxOptions, thrownError )
			{
				FMErrorAndExceptionHandling.ShowException( xhr, ajaxOptions, thrownError, function()
				{
					// hide the saving animation
					$(".loadingDiv").remove();
				}, notificationAttributes);
			}
		} );
	}

	var _serializeSettings = function (section) {

		var settings='';

		$('#' + section + '  [data-display]').each(function () {
			var value = null;
			switch ($(this).attr('data-display')) {
				case "input":
					// if we have a raw value pass the raw value otherwise just pass the value
					var rawValue = $(this).find('input').attr('data-raw-value');

					// For some browsers, `attr` is undefined; for others,
					// `attr` is false.  Check for both.
					if (typeof rawValue !== typeof undefined && rawValue !== false) {
						value = rawValue;
						break;
					}
					value = $(this).find('input').val();
					break;
				case "boolean":
					var name = $(this).find('label:first').attr('for');
					value = $("input[type=radio][name='" + name + "']:checked").val();
					break;
				case "dropdown":
					value = $(this).find('select').val();
					break;
				case "calendar":
					value = moment($(this).find('input').datepicker("getDate")).format();
					break;
			}
			if (value !== null) {
				settings+='&' + $(this).find('label:first').attr('for') + '=' + value;
			}
		});

		return settings;
	}

	_replicateDefault = function (controlID, rescaleAlarmLimits) {
		var newDefaultValue = $('#' + controlID).val();
		var newDefaultRawValue = $('#' + controlID).attr('data-raw-value');
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);

		$.each(FMPointEditor.tagList, function () {
			var index = this.index;
			var newTagValue = newDefaultValue;
			var newTagRawValue = newDefaultRawValue;

			if ((this.EngineeringUnitsType == "FmuTemp" && controlID.indexOf("Temperature") != -1)
			|| (this.EngineeringUnitsType == "FmuLength" && controlID.indexOf("Level") != -1)
			|| (this.EngineeringUnitsType == "FmuVolume" && (controlID.indexOf("Volume") != -1 && controlID.indexOf("VolumetricFlow") == -1))
			|| (this.EngineeringUnitsType == "FmuMass" && (controlID.indexOf("Mass") != -1  && controlID.indexOf("MassFlow") == -1))
			|| (this.EngineeringUnitsType == "FmuPressure" && controlID.indexOf("Pressure") != -1)
			|| (this.EngineeringUnitsType == "FmuVolflow" && controlID.indexOf("VolumetricFlow") != -1)
			|| (this.EngineeringUnitsType == "FmuMassflow" && controlID.indexOf("MassFlow") != -1)
			|| (this.EngineeringUnitsType == "FmuVelocity" && controlID.indexOf("Velocity") != -1)
			|| (this.EngineeringUnitsType == "FmuDensity" && controlID.indexOf("Density") != -1)) {
				if (this.ApplyPointDecimalPlaces === true && controlID.indexOf("DecimalPlaces") != -1) {
					$("#Tags_" + index + "__DecimalPlaces").val(newTagValue);
					this.DecimalPlaces = parseInt(newTagValue);

					var unitIndex = parseInt(this.Units);
					var defUnitIndex;
					if (this.ApplyPointEngineeringUnits === true) {
						defUnitIndex = FMPointEditor.getDefaultUnits(this.EngineeringUnitsType);
					}
					else {
						defUnitIndex = unitIndex;
					}

					var valueControl = $("#Tags_" + index + "__Value");
					if (valueControl.length > 0
					&& valueControl.attr('data-raw-value') != '') {
						var value = valueControl.attr('data-raw-value');
						value = (isNaN(value)) ? math.bignumber(0) : math.bignumber(value);
						numformatInfo.NumberDecimalDigits = this.DecimalPlaces;
						var newFormattedValue = FMFormatValues.FormatValueFullPrecision(defUnitIndex, numformatInfo, value);
						valueControl.val(newFormattedValue);
					}
				}

				else if (controlID.indexOf("Minimum") != -1) {
					var unitIndex = parseInt(this.Units);
					var alarmTestGuid = null;
					if (rescaleAlarmLimits) {
						alarmTestGuid = AlarmEditor.GetAlarmTestGuid(this.PointTagGuid);
					}
					var defUnitIndex;
					
					if (this.ApplyPointMinimum === false) {

						if (this.ApplyPointEngineeringUnits === true) {
							defUnitIndex = FMPointEditor.getDefaultUnits(this.EngineeringUnitsType);
						}
						else {
							defUnitIndex = unitIndex;
						}


						if (this.ApplyPointEngineeringUnits === false
						&& ((defUnitIndex === 191 && unitIndex !== 191)
						|| (defUnitIndex !== 191 && unitIndex === 191))) {
							newTagRawValue = $("#Tags_" + index + "__Maximum").attr('data-raw-value');
						}
						else {
							newTagRawValue = $("#Tags_" + index + "__Minimum").attr('data-raw-value');
						}
					}
					else {
						defUnitIndex = FMPointEditor.getDefaultUnits(this.EngineeringUnitsType);
					}

					var oldRange = this.Maximum - this.Minimum;
					var valuePercentOfRange = 0.0;
					if (alarmTestGuid != null) {
						var limitValue = AlarmEditor.GetAlarmTestLimitValue(this.PointTagGuid);
						var rawLimitValue = FMFormatValues.ParseValue(parseInt(unitIndex), numformatInfo, limitValue);
						valuePercentOfRange = (rawLimitValue - this.Minimum) / oldRange;
					}

					numformatInfo.NumberDecimalDigits = this.DecimalPlaces;

					newTagRawValue = FMConvertEngUnits.Convert(newTagRawValue, defUnitIndex, unitIndex);
					newTagValue = FMFormatValues.FormatValueFullPrecision(this.Units, numformatInfo, newTagRawValue);

					// Special case where if density units do not match and one is API then target the Maximum
					if (this.ApplyPointEngineeringUnits === false
					&&	((defUnitIndex === 191 && unitIndex !== 191)
					|| (defUnitIndex !== 191 && unitIndex === 191))) {
						$("#Tags_" + index + "__Maximum").val(newTagValue);
						$("#Tags_" + index + "__Maximum").attr('data-raw-value', newTagRawValue.toString());
						this.Maximum = math.number(newTagRawValue);
					}
					else {
						$("#Tags_" + index + "__Minimum").val(newTagValue);
						$("#Tags_" + index + "__Minimum").attr('data-raw-value', newTagRawValue.toString());
						this.Minimum = math.number(newTagRawValue);
					}

					if (alarmTestGuid != null) {
						newTagRawValue = (valuePercentOfRange * (this.Maximum - this.Minimum)) + this.Minimum;
						newTagValue = FMFormatValues.FormatValueFullPrecision(this.Units, numformatInfo, newTagRawValue);
						if ($("#IsTemplatePoint").val() == 'True') {
							$('#Tags_' + index + '__Value').val(newTagValue);
							$('#Tags_' + index + '__Value').attr("data-raw-value", newTagRawValue.toString());
						}
						var newRange = this.Maximum - this.Minimum;
						AlarmEditor.UpdateAlarmTestValue(alarmTestGuid, newTagValue);
						var newHoldoffRawValue = (AlarmEditor.GetAlarmTestHoldoff(this.PointTagGuid) / oldRange) * newRange;
						var newHoldoffValue = FMFormatValues.FormatValueFullPrecision(this.Units, numformatInfo, newHoldoffRawValue);
						AlarmEditor.UpdateAlarmTestHoldoff(alarmTestGuid, newHoldoffRawValue, newHoldoffValue);
					}
				}

				else if (controlID.indexOf("Maximum") != -1) {
					var unitIndex = parseInt(this.Units);
					var alarmTestGuid = null;
					if (rescaleAlarmLimits) {
						alarmTestGuid = AlarmEditor.GetAlarmTestGuid(this.PointTagGuid);
					}
					var defUnitIndex;
					if (this.ApplyPointMaximum === false) {
						if (this.ApplyPointEngineeringUnits === true) {
							defUnitIndex = FMPointEditor.getDefaultUnits(this.EngineeringUnitsType);
						}
						else {
							defUnitIndex = unitIndex;
						}

						if (this.ApplyPointEngineeringUnits === false
						&& ((defUnitIndex === 191 && unitIndex !== 191)
						|| (defUnitIndex !== 191 && unitIndex === 191))) {
							newTagRawValue = $("#Tags_" + index + "__Minimum").attr('data-raw-value');
						}
						else {
							newTagRawValue = $("#Tags_" + index + "__Maximum").attr('data-raw-value');
						}
					}
					else {
						defUnitIndex = FMPointEditor.getDefaultUnits(this.EngineeringUnitsType);
					}

					var oldRange = this.Maximum - this.Minimum;
					var valuePercentOfRange = 0.0;
					if (alarmTestGuid != null) {
						var limitValue = AlarmEditor.GetAlarmTestLimitValue(this.PointTagGuid);
						var rawLimitValue = FMFormatValues.ParseValue(parseInt(unitIndex), numformatInfo, limitValue);
						valuePercentOfRange = (rawLimitValue - this.Minimum) / oldRange;
					}

					numformatInfo.NumberDecimalDigits = this.DecimalPlaces;
					
					newTagRawValue = FMConvertEngUnits.Convert(newTagRawValue, defUnitIndex, unitIndex);
					newTagValue = FMFormatValues.FormatValueFullPrecision(this.Units, numformatInfo, newTagRawValue);

					// Special case where if density units do not match and one is API then target the Minimum
					if (this.ApplyPointEngineeringUnits === false
					&&	((defUnitIndex === 191 && unitIndex !== 191)
					|| (defUnitIndex !== 191 && unitIndex === 191))) {
						$("#Tags_" + index + "__Minimum").val(newTagValue);
						$("#Tags_" + index + "__Minimum").attr('data-raw-value', newTagRawValue.toString());
						this.Minimum = math.number(newTagRawValue);
					}
					else {
						$("#Tags_" + index + "__Maximum").val(newTagValue);
						$("#Tags_" + index + "__Maximum").attr('data-raw-value', newTagRawValue.toString());
						this.Maximum = math.number(newTagRawValue);
					}

					if (alarmTestGuid != null) {
						newTagRawValue = (valuePercentOfRange * (this.Maximum - this.Minimum)) + this.Minimum;
						newTagValue = FMFormatValues.FormatValueFullPrecision(this.Units, numformatInfo, newTagRawValue);
						if($("#IsTemplatePoint").val() == 'True') {
							$('#Tags_' + index + '__Value').val(newTagValue);
							$('#Tags_' + index + '__Value').attr("data-raw-value", newTagRawValue.toString());
						}
						var newRange = this.Maximum - this.Minimum;
						AlarmEditor.UpdateAlarmTestValue(alarmTestGuid, newTagValue);
						var newHoldoffRawValue = (AlarmEditor.GetAlarmTestHoldoff(this.PointTagGuid) / oldRange) * newRange;
						var newHoldoffValue = FMFormatValues.FormatValueFullPrecision(this.Units, numformatInfo, newHoldoffRawValue);
						AlarmEditor.UpdateAlarmTestHoldoff(alarmTestGuid, newHoldoffRawValue, newHoldoffValue);
					}
				}

				else if (this.ApplyPointEngineeringUnits === true && controlID.indexOf("EngineeringUnits") != -1) {
					// If Server Unit matches Unit update Server Unit
					if ($("#Tags_" + index + "__Unit").val() === $("#Tags_" + index + "__ServerUnit").val()) {
						$("#Tags_" + index + "__ServerUnit").val(newTagValue);
					}
					$("#Tags_" + index + "__Unit").val(newTagValue);
					FMPointEditor.unitsChanged($("#Tags_" + index + "__Unit"));
					_resetTagInputMask("#Tags_" + index + "__Minimum");
					_resetTagInputMask("#Tags_" + index + "__Maximum");
				}
			}
		});
	}

	_resetDefaultInputMask = function (control) {
		var row = $(control).closest('tr');
		var unit = $(row).find('.tagDefaultUnits').val();
		var precision = ~~Number($(row).find('.tagDefaultDecimalPlaces').val());

		$(control).removeNumeric(); // remove numeric mask if there was one
		$(control).unmask(); // if it had a mask remove it

		// add the mask to the edit fields and populate them with the initial formatted value
		// if feet-in-16th or feet-in-8th use 00-00-00 as mask, otherwise is just plain numeric
		if (unit == "27") { //"FML_FtIn16th"	S99-99-99
				$(control).mask('S99-99-99', {
					translation: {
						'S': {
							pattern: /-/,
							optional: true
						}
					},
					placeholder: "__-__-__"
				});
		} else if (unit == "19") { //"FML_FtIn8th"
				$(control).mask('S99-99-9', {
					translation: {
						'S': {
							pattern: /-/,
							optional: true
						}
					},
					placeholder: "__-__-__"
				});
		} else {
			var numFormatInfo = JSON.parse($('#NumberFormatInfoString').val());
			$(control).attr("placeholder", "");
			$(control).numeric({
				decimal: numFormatInfo.NumberDecimalSeparator
				, negative: true
				, decimalPlaces: parseInt(9)
			});
		}
	}

	_resetTagInputMask = function (control) {
		var row = $(control).closest('tr');
		var unit = $(row).find('.tagColumnUnits select').val();
		var precision = ~~Number($(row).find('.tagColumnDecimalPlaces input').val());

		$(control).removeNumeric(); // remove numeric mask if there was one
		$(control).unmask(); // if it had a mask remove it

		// add the mask to the edit fields and populate them with the initial formatted value
		// if feet-in-16th or feet-in-8th use 00-00-00 as mask, otherwise is just plain numeric
		if (unit == "27") { //"FML_FtIn16th"	s99-99-99
			$(control).mask('S99-99-99', {
					translation: {
						'S': {
							pattern: /-/,
							optional: true
						}
					},
					placeholder: "__-__-__"
				});
		} else if (unit == "19") { //"FML_FtIn8th"
				$(control).mask('S99-99-9', {
					translation: {
						'S': {
							pattern: /-/,
							optional: true
						}
					},
					placeholder: "__-__-__"
				});
		} else {
				if (precision === 0) {
					$(control).attr("placeholder", "");
					$(control).numeric({
						decimal: false
						, negative: true
					});
				} else {
					var numFormatInfo = JSON.parse($('#NumberFormatInfoString').val());
					$(control).attr("placeholder", "");
					$(control).numeric({
						decimal: numFormatInfo.NumberDecimalSeparator
						, negative: true
						, decimalPlaces: parseInt(9)
					});
				}
		}
	}

	_resetSettingInputMask = function (control) {

		var unit = $("#" + control.attr("data-unit")).val();
		var precision = $("#" + control.attr("data-format")).val();

		$(control).removeNumeric(); // remove numeric mask if there was one
		$(control).unmask(); // if it had a mask remove it

		// add the mask to the edit fields and populate them with the initial formatted value
		// if feet-in-16th or feet-in-8th use 00-00-00 as mask, otherwise is just plain numeric
		if (unit == "27") { //"FML_FtIn16th"	S99-99-99
			$(control).mask('S99-99-99', {
					translation: {
						'S': {
							pattern: /-/,
							optional: true
						}
					},
					placeholder: "__-__-__"
				});
		} else if (unit == "19") { //"FML_FtIn8th"
				$(control).mask('S99-99-9', {
					translation: {
						'S': {
							pattern: /-/,
							optional: true
						}
					},
					placeholder: "__-__-__"
				});
		} else {
				if (precision == "") {
					$(control).attr("placeholder", "");
					$(control).numeric({
						decimal: false
						, negative: true
					});
				} else {
					$(control).attr("placeholder", "");
					$(control).numeric({
						decimal: "."
						, negative: true
						, decimalPlaces: parseInt(9)//precision)
					});
				}
		}
	}

	_performReScaling = function (performConversion) {

		if (!this.unitSelectionControl) return;
		
		var selectControl = this.unitSelectionControl;

		var numDecimals = selectControl.closest('tr').find('.tagDefaultDecimalPlaces').val();
		numDecimals = (isNaN(numDecimals)) ? numDecimals = 0 : ~~Number(numDecimals);

		var minimum = selectControl.closest('tr').find('.tagDefaultMinimum').attr('data-raw-value');
		minimum = (isNaN(minimum)) ? math.bignumber(0) : math.bignumber(minimum);
		var minimumId = selectControl.closest('tr').find('.tagDefaultMinimum').attr("id");

		var maximum = selectControl.closest('tr').find('.tagDefaultMaximum').attr('data-raw-value');
		maximum = (isNaN(maximum)) ? math.bignumber(0) : math.bignumber(maximum);
		var maximumId = selectControl.closest('tr').find('.tagDefaultMaximum').attr("id");
		var oldUnitIndex = selectControl.data("prev");
		oldUnitIndex = (isNaN(oldUnitIndex)) ? 0 : ~~Number(oldUnitIndex);
		var newUnitIndex = selectControl.val();
		newUnitIndex = (isNaN(newUnitIndex)) ? 0 : ~~Number(newUnitIndex);

		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);
		numformatInfo.NumberDecimalDigits = numDecimals;


		var newMimimumRawValue = (performConversion) ? FMConvertEngUnits.Convert(minimum, oldUnitIndex, newUnitIndex) : minimum;
		var newMimimumFormattedValue = FMFormatValues.FormatValueFullPrecision(newUnitIndex, numformatInfo, newMimimumRawValue);

		var newMaximumRawValue = (performConversion) ? FMConvertEngUnits.Convert(maximum, oldUnitIndex, newUnitIndex) : maximum;
		var newMaximumFormattedValue = FMFormatValues.FormatValueFullPrecision(newUnitIndex, numformatInfo, newMaximumRawValue);

		//If switching from or to Degrees API then swap values else leave them as they are.
		if (((oldUnitIndex !== 191 && newUnitIndex === 191)
		|| (oldUnitIndex === 191 && newUnitIndex !== 191))
		&& performConversion) {
			FMPointEditor.resetDefaultInputMask($('#' + minimumId));
			$('#' + minimumId).val(newMaximumFormattedValue); //Max Value in In Min Field
			$('#' + minimumId).attr('data-raw-value', newMaximumRawValue.toString()); //Max Raw Value in Min Raw Value

			FMPointEditor.resetDefaultInputMask($('#' + maximumId));
			$('#' + maximumId).val(newMimimumFormattedValue); //Min Value in In Max Field
			$('#' + maximumId).attr('data-raw-value', newMimimumRawValue.toString()); //Min Value in Max Field

		} else {
			FMPointEditor.resetDefaultInputMask($('#' + minimumId));
			$('#' + minimumId).val(newMimimumFormattedValue);
			$('#' + minimumId).attr('data-raw-value', newMimimumRawValue.toString());

			FMPointEditor.resetDefaultInputMask($('#' + maximumId));
			$('#' + maximumId).val(newMaximumFormattedValue);
			$('#' + maximumId).attr('data-raw-value', newMaximumRawValue.toString());

		}

		//replicate changes to the tags
		FMPointEditor.replicateDefault(selectControl.attr("id"), false);
		FMPointEditor.replicateDefault(minimumId, false);
		FMPointEditor.replicateDefault(maximumId, false);

		/* update the settings that use the same units */
		var defaultUnit = selectControl.attr("id");
		$("input[data-unit=" + defaultUnit + "]").each(function (index, control) {
			var value = $(control).attr('data-raw-value');
			var controlId = $(control).attr("id");

			var newRawValue = (performConversion) ? FMConvertEngUnits.Convert(value, oldUnitIndex, newUnitIndex) : value;
			var newFormattedValue = FMFormatValues.FormatValueFullPrecision(newUnitIndex, numformatInfo, newRawValue);

			FMPointEditor.resetSettingInputMask($('#' + controlId));
			$('#' + controlId).val(newFormattedValue);
			$('#' + controlId).attr('data-raw-value', newRawValue.toString());
		});

		$("label[data-unit=" + defaultUnit + "]").each(function (index, control) {
			_setSettingUnits(control);
		});


		//Update Limit Values
		AlarmEditor.PerformRescaling(FMPointEditor.getTagTable(), performConversion);

		FMPointEditor.unitConversionHistory.push({
				"UnitType": $(selectControl).attr('name'),
				"PerformConversion": performConversion,
				"OldUnit": oldUnitIndex,
				"NewUnit": newUnitIndex
		});

		selectControl.data("prev", selectControl.val());
	}

	_resetUnitSelection = function () {
		/*if (!this.unitSelectionControl) return;

		var selectControl = this.unitSelectionControl;

		selectControl.val(selectControl.data("prev"));*/
	}

	_getAlarmEditor = function (pointGuidStr) {
		var url = $('#urlGetAlarmEditor').val();
		var token = $('input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		$.ajax({

			url: url,
			cache: false,
			type: 'GET',
			headers: headers,
			dataType: 'json',
			contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
			//data: 'pointTemplateGuidString=' + pointTemplateGuidString + '&modelString=' + modelString,
			//type: 'Post',
			//url: 'GetAlarmEditor',
			//cache: false,
			data: {
				'pointGuidStr': pointGuidStr
			},
			success: function (response) {
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
					if (!inError) {
						// replace the holder with the partial view
						$('#PTTAlarmEditHolder').html(data);
					}
					else {
						FMErrorAndExceptionHandling.ShowError("Error Getting Alarm Editor");
					}
				});
			},
			error: function (xhr, textStatus, error) {
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
					});
			}

		});
	}
	
	_changeTemplatePointIcon = function changeTemplatePointIcon(imageGuid)
	{
		$("#PointDetailImageContainer").attr("data-value", imageGuid);
		$("#PointDetailImage").attr("src", window.applicationRootName + "/DisplayImage.ashx?PictureGuid=" + imageGuid);
		CloseImageSelection();
	}


	// function to generate a unique guid
	_newGuid = function () {
		function s4() {
			return Math.floor((1 + Math.random()) * 0x10000)
			  .toString(16)
			  .substring(1);
		}
		return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
	}

	// returns true if the value passed is a Guid
	_isGuid = function (stringToTest) {
		if (stringToTest[0] === "{") {
			stringToTest = stringToTest.substring(1, stringToTest.length - 1);
		}
		var regexGuid = /^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$/gi;
		return regexGuid.test(stringToTest);
	}

	_isTagNumeric = function (dataType)
	{
		return (dataType === "System.Double"
			|| dataType === "System.Single"
			|| dataType === "System.Int16"
			|| dataType === "System.Int32"
			|| dataType === "System.Int64"
			|| dataType === "System.UInt16"
			|| dataType === "System.UInt32"
			|| dataType === "System.UInt64") ? true : false;
	}

	_addTag = function (newGuid, newName, dataType, inputOutputType, unitsType, units, decimalPlaces, value, maximum, minimum, applyPointDecimalPlaces, applyPointEngineeringUnits, applyPointMaximum, applyPointMinimum) {
		// copy the empty row and insert it to the end of the list
		var newRow = $("#empty_tagrow tbody").children().clone();
		
		var rowNum = 0;

		// get the highest row id by looking at the column name
		$('#TagEditTable tr .column-tag-name').each(function (index) {
			var rowId = parseInt($(this).attr('id').replace('Tags_', '').replace('__Name', ''));
			if (rowId >= rowNum) {
				rowNum = rowId + 1;
			}
		});

		// change the id and name for all the columns to be unique
		newRow.find("[id]").each(function (index) {
			var id = $(this).attr('id');
			$(this).attr('id', id.replace("{id}", rowNum));
		});
		newRow.find("[name]").each(function (index) {
			var name = $(this).attr('name');
			$(this).attr('name', name.replace("{id}", rowNum));
		});
		
		var defaultUnitsTypeControl = FMPointEditor.getDefaultUnitsControl(unitsType);
		// set the defaults
		newRow.find('.tagColumnPointTagGuid').text(newGuid);
		newRow.find('#Tags_' + rowNum + '__Name').val(newName);
		newRow.find('#Tags_' + rowNum + '__DataType').val(dataType);
		newRow.find('#Tags_' + rowNum + '__InputOutputType').val(inputOutputType);
		newRow.find('#Tags_' + rowNum + '__EngineeringUnitsType').val(unitsType);
		newRow.find('.tagColumnUnits select').children().remove();
		if (defaultUnitsTypeControl != null) {
			newRow.find('.tagColumnUnits select').append(defaultUnitsTypeControl.children().clone());
			newRow.find('.tagColumnUnits select').val(units);
			newRow.find('.tagColumnUnits select').data('prev', units);
			newRow.find('.tagColumnServerUnits select').append(defaultUnitsTypeControl.children().clone());
			newRow.find('.tagColumnserverUnits select').val(units);
			newRow.find('.tagColumnServerUnits select').data('prev', units);
		}
		else {
			newRow.find('.tagColumnUnits select').append('<option value="255"></option>');
			newRow.find('.tagColumnUnits select').val(units);
			newRow.find('.tagColumnUnits select').data('prev', units);
			newRow.find('.tagColumnServerUnits select').append('<option value="255"></option>');
			newRow.find('.tagColumnServerUnits select').val(units);
			newRow.find('.tagColumnServerUnits select').data('prev', units);
		}
		newRow.find('#Tags_' + rowNum + '__Unit').val(units);
		newRow.find('#Tags_' + rowNum + '__DecimalPlaces').val(decimalPlaces);
		// set the mask on the precision so we cannot add negative values or characters
		newRow.find('#Tags_' + rowNum + '__DecimalPlaces').numeric({
			decimal: false,
			negative: false
		});
		if (FMPointEditor.isTagNumeric(dataType)) {
			newRow.find('#Tags_' + rowNum + '__Value').val(value);
			newRow.find('#Tags_' + rowNum + '__Value').attr("data-raw-value", parseFloat(value));
			if (Number.isNaN(parseFloat(value))) {
				newRow.find('#Tags_' + rowNum + '__Value').attr("data-raw-value", null);
			}
		}
		else {
			if (dataType === 'FMBusinessObjects.DataObjects.PointCommandStatusListReference') {
				newRow.find('#Tags_' + rowNum + '__Value').val(value.CurrentKey);
			}
			else {
				newRow.find('#Tags_' + rowNum + '__Value').val(value);
			}
			newRow.find('#Tags_' + rowNum + '__Value').attr("data-raw-value", value);
		}
		newRow.find('#Tags_' + rowNum + '__Minimum').val(minimum);
		newRow.find('#Tags_' + rowNum + '__Minimum').attr("data-raw-value", parseFloat(minimum));
		newRow.find('#Tags_' + rowNum + '__Maximum').val(maximum);
		newRow.find('#Tags_' + rowNum + '__Maximum').attr("data-raw-value", parseFloat(maximum));
		newRow.find('#Tags_' + rowNum + '__WellKnownTag').val("0");
		newRow.find('tr').addClass('selected');
		newRow.appendTo("#TagEditTable tbody").find('#Tags_' + rowNum + '__Name').focus();

		newRow.find('#Tags_' + rowNum + '__Name').on('blur', function () {
			// renameTag not available in Point Editor
			if (FMPointEditor.renameTag) {
				FMPointEditor.renameTag(this);
			}
		});

		FMPointEditor.tagList.push({
			ApplyPointDecimalPlaces: applyPointDecimalPlaces,
			ApplyPointEngineeringUnits: applyPointEngineeringUnits,
			ApplyPointMaximum: applyPointMaximum,
			ApplyPointMinimum: applyPointMinimum,
			DataType: dataType,
			DecimalPlaces: decimalPlaces,
			EngineeringUnitsType: unitsType,
			InputOutputType: inputOutputType,
			Input: 1,
			InhibitInputOutputTypeConfiguration: 0,
			InhibitOverride: 0,
			Archived: 1,
			Maximum: maximum,
			Minimum: minimum,
			Name: newName,
			PointTagGuid: newGuid,
			Units: parseInt(units),
			ServerUnits: parseInt(units),
			WellKnownIdentityGuidString: "00000000-0000-0000-0000-000000000000",
			index: rowNum
		});

		// scroll to the end of the tag list
		var scrollBottom = Math.max($('#TagEditTable').height() - $('#TagEditTableWrap').height() + 20, 0);
		$('#TagEditTableWrap').scrollTop(scrollBottom);

		// change the tag unit
		$('#Tags_' + rowNum + '__Unit').bind('change', function (data) {
			FMPointEditor.unitsChanged($(this));
		});

		// the user changes the precision in the tag grid
		$('#Tags_' + rowNum + '__DecimalPlaces').on('input', function (data) {
			FMPointEditor.decimalPlacesChanged($(this));
		});

		// the user changes the minimum in the tag grid
		$('#Tags_' + rowNum + '__Minimum').on('blur', function (data) {
			
			FMPointEditor.minimumOrMaximumChanged($(this));
		});

		FMPointEditor.resetTagInputMask('#Tags_' + rowNum + '__Minimum');

		// the user changes the maximum in the tag grid
		$('#Tags_' + rowNum + '__Maximum').on('blur', function (data) {
			FMPointEditor.minimumOrMaximumChanged($(this));
		});


		FMPointEditor.resetTagInputMask('#Tags_' + rowNum + '__Maximum');

		if (dataType === "FMBusinessObjects.DataObjects.PointCommandStatusListReference")
			$('#Tags_' + rowNum + '_TagPointCommandStatusListSelector').removeClass("hidden");

		if (dataType === "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
			$('#Tags_' + rowNum + '_TagDeviceAlarmMapSelector').removeClass("hidden");

		if (dataType === "System.Double"
		|| dataType === "System.Single") {
			newRow.find('.tagColumnUnitType select').removeClass("hidden");
			newRow.find('.tagColumnDecimalPlaces input').removeClass("hidden");
			newRow.find('.tagColumnMinimum input').removeClass("hidden");
			newRow.find('.tagColumnMaximum input').removeClass("hidden");
			if (unitsType !== 'FmuNodim'
			&& unitsType !== 'FmuNone') {
				newRow.find('.tagColumnUnits select').removeClass("hidden");
				newRow.find('.tagColumnApplyPointEngineeringUnits img').prop('disabled', false).removeClass('disabled');
				newRow.find('.tagColumnApplyPointDecimalPlaces img').prop('disabled', false).removeClass('disabled');
				newRow.find('.tagColumnApplyPointMinimum img').prop('disabled', false).removeClass('disabled');
				newRow.find('.tagColumnApplyPointMaximum img').prop('disabled', false).removeClass('disabled');
			}
			else {
				newRow.find('.tagColumnDecimalPlaces input').prop('disabled', false);
				newRow.find('.tagColumnMinimum input').prop('disabled', false);
				newRow.find('.tagColumnMaximum input').prop('disabled', false);
				newRow.find('.tagColumnUnits select').removeClass("hidden").addClass("hidden");
				newRow.find('.tagColumnApplyPointEngineeringUnits img').prop('disabled', true).removeClass("disabled").addClass("disabled").attr("data-value", "false").attr("alt", "toggleOff").attr("title", "No").attr("src", window.applicationRootName + "/fmwebapp/images/Off.png");
				newRow.find('.tagColumnApplyPointDecimalPlaces img').prop('disabled', true).removeClass("disabled").addClass("disabled").attr("data-value", "false").attr("alt", "toggleOff").attr("title", "No").attr("src", window.applicationRootName + "/fmwebapp/images/Off.png");
				newRow.find('.tagColumnApplyPointMinimum img').prop('disabled', true).removeClass("disabled").addClass("disabled").attr("data-value", "false").attr("alt", "toggleOff").attr("title", "No").attr("src", window.applicationRootName + "/fmwebapp/images/Off.png");
				newRow.find('.tagColumnApplyPointMaximum img').prop('disabled', true).removeClass("disabled").addClass("disabled").attr("data-value", "false").attr("alt", "toggleOff").attr("title", "No").attr("src", window.applicationRootName + "/fmwebapp/images/Off.png");
			}

			if (FMPointEditor.tagList[rowNum].ApplyPointDecimalPlaces) {
				FMPointEditor.setTagDecimalPlacesFromDefaultDecimalPlaces(newRow);
			}

			if (FMPointEditor.tagList[rowNum].ApplyPointEngineeringUnits) {
				FMPointEditor.setTagUnitsFromDefaultUnits(newRow);
			}

			if (FMPointEditor.tagList[rowNum].ApplyPointMinimum) {
				FMPointEditor.setTagMinimumFromDefaultMinimum(newRow);
			}
			else {
				FMPointEditor.setTagMinimum(newRow);
			}

			if (FMPointEditor.tagList[rowNum].ApplyPointMaximum) {
				FMPointEditor.setTagMaximumFromDefaultMaximum(newRow);
			}
			else {
				FMPointEditor.setTagMaximum(newRow);
			}
		}
		else {
			newRow.find('.tagColumnApplyPointEngineeringUnits img').prop('disabled', true).removeClass("disabled").addClass("disabled").attr("data-value", "false").attr("alt", "toggleOff").attr("title", "No").attr("src", window.applicationRootName + "/fmwebapp/images/Off.png");
			newRow.find('.tagColumnApplyPointDecimalPlaces img').prop('disabled', true).removeClass("disabled").addClass("disabled").attr("data-value", "false").attr("alt", "toggleOff").attr("title", "No").attr("src", window.applicationRootName + "/fmwebapp/images/Off.png");
			newRow.find('.tagColumnApplyPointMinimum img').prop('disabled', true).removeClass("disabled").addClass("disabled").attr("data-value", "false").attr("alt", "toggleOff").attr("title", "No").attr("src", window.applicationRootName + "/fmwebapp/images/Off.png");
			newRow.find('.tagColumnApplyPointMaximum img').prop('disabled', true).removeClass("disabled").addClass("disabled").attr("data-value", "false").attr("alt", "toggleOff").attr("title", "No").attr("src", window.applicationRootName + "/fmwebapp/images/Off.png");
			newRow.find('.tagColumnDecimalPlaces input').removeClass("hidden").addClass("hidden");

			if (dataType === "System.Int16"
			|| dataType === "System.Int32"
			|| dataType === "System.Int64"
			|| dataType === "System.UInt16"
			|| dataType === "System.UInt32"
			|| dataType === "System.UInt64") {
				newRow.find('.tagColumnMinimum input').removeClass("hidden");
				newRow.find('.tagColumnMaximum input').removeClass("hidden");
				newRow.find('.tagColumnUnits select').removeClass("hidden").addClass("hidden");
				newRow.find('.tagColumnUnitType select').removeClass("hidden").addClass("hidden");
				newRow.find('.tagColumnUnitType select').val('FmuNone');  // default to None
				newRow.find('.tagColumnUnits select').val('255');

				FMPointEditor.setTagMinimum(newRow);
				FMPointEditor.setTagMaximum(newRow);
			}
			else {
				newRow.find('.tagColumnMinimum input').removeClass("hidden").addClass("hidden");
				newRow.find('.tagColumnMaximum input').removeClass("hidden").addClass("hidden");
				newRow.find('.tagColumnUnits select').removeClass("hidden").addClass("hidden");
				newRow.find('.tagColumnUnitType select').removeClass("hidden").addClass("hidden");
				newRow.find('.tagColumnUnitType select').val('FmuNone');  // default to None
				newRow.find('.tagColumnUnits select').val('255');
			}
		}


		if (!FMPointEditor.tagList[rowNum].ApplyPointEngineeringUnits) {
			$('#Tags_' + rowNum + '__ApplyPointEngineeringUnits').click();
		}

		if (!FMPointEditor.tagList[rowNum].ApplyPointDecimalPlaces) {
			$('#Tags_' + rowNum + '__ApplyPointDecimalPlaces').click();
		}

		if (!FMPointEditor.tagList[rowNum].ApplyPointMinimum) {
			$('#Tags_' + rowNum + '__ApplyPointMinimum').click();
		}

		if (!FMPointEditor.tagList[rowNum].ApplyPointMaximum) {
			$('#Tags_' + rowNum + '__ApplyPointMaximum').click();
		}

		_updateServerUnitControlState(newRow);
	}


	_deleteTagbyGuid = function (pointTemplateTagGuid) {

		// find the input tag since we need to get the datatype
		var tagInfo = jQuery.grep(FMPointEditor.tagList, function (row) { return row.PointTagGuid === pointTemplateTagGuid; });
		if (tagInfo.length > 0) {
			var rowNum = tagInfo[0].index;
			$("#Tags_" + rowNum + "__Name").closest('tr').remove();
			FMPointEditor.tagList.splice(rowNum, 1);
			FMPointEditor.resetRowNumbers();
			FMPointEditor.valuesChanged = true;
		}
	}

	_sortTagTable = function () {
		var $tbody = $('#TagEditTable tbody');
		$tbody.find('tr').sort(function (a, b) {
			var tda = $(a).find('.column-tag-name').val().toUpperCase(); // Use your wished column index
			var tdb = $(b).find('.column-tag-name').val().toUpperCase(); // Use your wished column index

			// if a > b return 1
			return tda > tdb ? 1
						// else if a < b return -1
						: tda < tdb ? -1
						// else they are equal - return 0
						: 0;
		}).appendTo($tbody);

		FMPointEditor.tagList.sort(function (a, b) {
			// if a > b return 1
			return a.Name.toUpperCase() > b.Name.toUpperCase() ? 1
						// else if a < b return -1
						: a.Name.toUpperCase() < b.Name.toUpperCase() ? -1
						// else they are equal - return 0
						: 0;

		});

		FMPointEditor.resetRowNumbers();
	}

	_resetRowNumbers = function(){
		rowNum = 0;
		$('#TagEditTable tr .column-tag-name').each(function (index) {
			var rowId = parseInt($(this).attr('id').replace('Tags_', '').replace('__Name', ''));
			if (rowNum == rowId) {
				rowNum++;
				return;
			}

			var row = $(this).closest('tr');

			row.find("[id]").each(function (index) {
				var id = $(this).attr('id');
				$(this).attr('id', id.replace(rowId, rowNum));
			});
			row.find("[name]").each(function (index) {
				var name = $(this).attr('name');
				$(this).attr('name', name.replace(rowId, rowNum));
			});

			FMPointEditor.tagList[rowNum].index = rowNum;
			rowNum++;
		});
	};


    return {
	    valuesChanged: _valuesChanged
		, alarmValuesChanged: _alarmValuesChanged
		, tagList: _tagList
		, initializeTagGrid: _initializeTagGrid
		, initializeModuleSettings: _initializeModuleSettings
		, saveChanges: _saveChanges
		, replicateDefault: _replicateDefault
		, resetDefaultInputMask: _resetDefaultInputMask
		, resetTagInputMask: _resetTagInputMask
		, resetSettingInputMask: _resetSettingInputMask
		, unitSelectionControl: null
		, performRescaling: _performReScaling
		, resetUnitSelection: _resetUnitSelection
		, unitConversionHistory: _unitConversionHistory
		, notification_stack: _notification_stack
		, notification_class: _notification_class
		, notification_pointselector_stack: _notification_pointselector_stack
    	, notification_imageselector_stack: _notification_imageselector_stack
		, formatSetting: _formatSetting
		, serializeSettings: _serializeSettings
		, getAlarmEditor: _getAlarmEditor
		, getTagTable: _getTagTable
		, changeTemplatePointIcon: _changeTemplatePointIcon
		, newGuid: _newGuid
		, isGuid: _isGuid
		, getDefaultUnits: _getDefaultUnits
		, getDefaultUnitsControl: _getDefaultUnitsControl
		, getDefaultDecimalPlaces: _getDefaultDecimalPlaces
		, getDefaultMinimum: _getDefaultMinimum
		, getDefaultMaximum: _getDefaultMaximum
		, setTagDecimalPlacesFromDefaultDecimalPlaces: _setTagDecimalPlacesFromDefaultDecimalPlaces
		, setTagUnitsFromDefaultUnits: _setTagUnitsFromDefaultUnits
		, setTagMinimumFromDefaultMinimum: _setTagMinimumFromDefaultMinimum
		, setTagMinimum: _setTagMinimum
		, setTagMaximumFromDefaultMaximum: _setTagMaximumFromDefaultMaximum
		, setTagMaximum: _setTagMaximum
		, unitsChanged: _unitsChanged
		, decimalPlacesChanged: _decimalPlacesChanged
		, minimumOrMaximumChanged: _minimumOrMaximumChanged
		, isTagNumeric: _isTagNumeric
		, addTag: _addTag
    	, deleteTagbyGuid: _deleteTagbyGuid
		, sortTagTable: _sortTagTable
		, resetRowNumbers: _resetRowNumbers
    };

}();



$(document).ready(function () {
    //Ensure that only 1 Notification Dialog Appears
    FMErrorAndExceptionHandling.OnlyOneNotification = true;
	// we are goign to display error messages inside the menu
	FMPointEditor.notification_pointselector_stack = { "dir1": "up", "dir2": "left", "context": $("#PointSettingListRolloutMenu") };

	// get the list of tags from the hidden field and reset the value
	var passedTagList = $("#PointTagListObject").val();
	if (passedTagList != "") {
		FMPointEditor.tagList = eval(passedTagList);
		FMPointEditor.initializeTagGrid();
		$("#PointTagListObject").val('');
	}

	FMPointEditor.initializeModuleSettings();

	$("#PESettings").click(function () {
		// hide the panels for the other editors
		$("#PEMenuItems li").removeClass("selected");
		$("#PESettings").addClass("selected");
		$("#PTTPointEditHolder").removeClass('hidden');
		$("#PTTTagEditHolder").addClass('hidden');
		$("#PTTModuleEditHolder").addClass('hidden');
		$("#PTTAlarmEditHolder").addClass('hidden');
		if ($("#IsTemplatePoint").val() == "False") {
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Points/PointsDetail";
		}
		else
		{
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointCustomTemplateDetail/PointTemplateDetail";
		}
		localStorage.setItem(FMPointEditor.localStorageKey, JSON.stringify({ pointGuid: FMPointEditor.pointGuid, activeTab: "SETTING" }));
	});

	$("#PETags").click(function () {
		// hide the panels for the other editors
		$("#PEMenuItems li").removeClass("selected");
		$("#PETags").addClass("selected");
		$("#PTTPointEditHolder").addClass('hidden');
		$("#PTTTagEditHolder").removeClass('hidden').addClass("selected");
		$("#PTTModuleEditHolder").addClass('hidden');
		$("#PTTAlarmEditHolder").addClass('hidden');
		if ($("#IsTemplatePoint").val() == "False") {
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Points/PointsDetailTags";
		}
		else
		{
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointCustomTemplateDetail/PointTemplateDetailTags";
		}
		localStorage.setItem(FMPointEditor.localStorageKey, JSON.stringify({ pointGuid: FMPointEditor.pointGuid, activeTab: "TAG" }));
	});

	$("#PEModules").click(function () {
		// hide the panels for the other editors
		$("#PEMenuItems li").removeClass("selected");
		$("#PEModules").addClass("selected");
		$("#PTTPointEditHolder").addClass('hidden');
		$("#PTTTagEditHolder").addClass('hidden');
		$("#PTTModuleEditHolder").removeClass('hidden').addClass("selected");
		$("#PTTAlarmEditHolder").addClass('hidden');
		if ($("#IsTemplatePoint").val() == "False") {
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Points/PointsDetailModules";
		}
		else
		{
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointCustomTemplateDetail/PointTemplateDetailModules";
		}
		localStorage.setItem(FMPointEditor.localStorageKey, JSON.stringify({ pointGuid: FMPointEditor.pointGuid, activeTab: "MODULE" }));
	});

	$("#PEAlarms").click(function () {
	    // hide the panels for the other editors
	    var alarmIsActiveTab = JSON.parse(localStorage.getItem(FMPointEditor.localStorageKey)).activeTab === "ALERT";
		$("#PEMenuItems li").removeClass("selected");
		$("#PEAlarms").addClass("selected");
		$("#PTTPointEditHolder").addClass('hidden');
		$("#PTTTagEditHolder").addClass('hidden');
		$("#PTTModuleEditHolder").addClass('hidden');
		$("#PTTAlarmEditHolder").removeClass('hidden');
		if ($("#IsTemplatePoint").val() == "False") {
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/AlarmEditor/AlarmEditorView";
		}
		else
		{
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointCustomTemplateDetail/PointTemplateDetailAlarms";
		}
		localStorage.setItem(FMPointEditor.localStorageKey, JSON.stringify({ pointGuid: FMPointEditor.pointGuid, activeTab: "ALERT" }));
	    //Protects against situations where the user clicks the alarm tab while the alarm tab is active.
        //No need to update the AlarmModel if the Alarm Tab is active.
        if (!alarmIsActiveTab)
	    {
        	AlarmEditor.UpdateAlarmModelFromPointTags(FMPointEditor.getTagTable(), false);
	    }
	});
	
	if ( $( "#IsTemplatePoint" ).val() == "False" )
	{
		FMPointEditor.getAlarmEditor( $( "#IdentityGuid" ).val() );
	}

	$("#PEfilter_modules").on('input', function (){

		var filterTxt = $("#PEfilter_modules").val();

		if (filterTxt == "") {
				$('#PTTModuleList > li').each(function () {
					$(this).removeClass("hidden");
				});
		} else {
				$('#PTTModuleList > li').each(function () {
					if ($(this).text().toLowerCase().indexOf(filterTxt.toLowerCase()) >= 0) {
						$(this).removeClass("hidden");
					} else {
						$(this).removeClass("hidden").addClass("hidden");
					}
				});
		}
	});

	//====================================================================
	// This function will handle the point list rollout menu filtering
	// for the Point Name filter.
	//====================================================================
	$("#PE_RolloutMenuPointNameFilterTB").on('input', function ()
	{
		PointListRolloutMenuFiltering();
	});

	//================================================================
	// This function will handle the filtering on the Point Setting
	// section. It will ensure that the filtering will contain the
	// correct alphabetical ordering and the top to down, then 
	// left to right ordering.
	//================================================================
	$("#PEfilter_Settings").on('input', function ()
	{

		var filterTxt = $("#PEfilter_Settings").val();

		if (filterTxt === "")
		{
			$("#PEModulePropertyList > div").each(function ()
			{
				$(this).removeClass("hidden");
			});
		}
		else {
			$("#PEModulePropertyList > div").each(function() {
					if ($(this).find('label').text().toLowerCase().indexOf(filterTxt.toLowerCase()) >= 0) {
						$(this).removeClass("hidden");
					}
					else {
						$(this).removeClass("hidden").addClass("hidden");
					}
			});

		}

		$("#PEModulePropertyList > div").removeClass("settings-stripes");

		// set the stripes in the 2 column list
		var first_column = Math.min.apply(null, $('#PEModulePropertyList > div:not(.hidden)').map(function (i, obj) { return $(obj).offset().left}));
		var second_column = Math.max.apply(null, $('#PEModulePropertyList > div:not(.hidden)').map(function (i, obj) {return $(obj).offset().left}));

		// loop through all the divs (entries) and add the class 'settings-stripes' every other column
		var i = 0;
		var last_column_checked = first_column;
		$('#PEModulePropertyList > div:not(.hidden)').each(function(idx, obj) {
				// are we in the same column or we are switching to the second column
				if ($(obj).offset().left == last_column_checked) {
					if (i % 2 == 0) $(obj).addClass("settings-stripes");
					i++;
				}
				else {
					i = 1;
					last_column_checked = second_column;
					$(obj).addClass("settings-stripes");
				}

		});


		// rewire the even handles since they get lost while copying tags 
		$(".PESettingPanel select").on("change", function () {
			FMPointEditor.valuesChanged = true;
		});

	});

	//====================================================================
	// This function will handle the image list rollout menu filtering
	// for the Point Template Image filter.
	//====================================================================
	$("#PE_IconNameFilterTB").on('input', function () {
		PointImageRolloutMenuFiltering();
	});


	// manually hookup to the submit the form to make sure we pass all the entries from the table
	$('#pointPropertiesForm').submit(function () {
		var action = this.action;
		var method = this.method;
		FMPointEditor.saveChanges(action, method, function() {}, '', '');
		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	});

	// **temporary fix until the Module screen is designed, click on the first element of the Modules List to populate the screen
	$("#PTTModuleList > li:first").click();


	// change the default unit
	$(".tagDefaultUnits").on('change', function (data) {
		FMPointEditor.valuesChanged = true;
		FMPointEditor.unitSelectionControl = $(this);

		// Prompt to confirm changes 
		FMLayout.ConfirmYesNo($("#confirm-scaling-confirmation-dialog"),
			"Confirm Scaling Change",
			function() {
				FMPointEditor.performRescaling(true);
				FMPointEditor.unitSelectionControl = null;
			},
			function() {
				FMPointEditor.performRescaling(false);
				FMPointEditor.unitSelectionControl = null;
			});

	});


	// change template name
	$("#PEID").on('blur', function (data)
	{
		var afterAction = null;
		if ($("#ApplyButtonId").data("mouseDown") == true) {
			afterAction = "save";
		}
		else if ($("#PETags").data("mouseDown") == true) {
			afterAction = "tags";
			$("#PETags").data("mouseDown", false);
		}
		else if ($("#PEModules").data("mouseDown") == true) {
			afterAction = "modules";
			$("#PEModules").data("mouseDown", false);
		}
		else if ( $( "#PEAlarms" ).data( "mouseDown" ) == true )
		{
			afterAction = "alarms";
			$("#PEAlarms").data("mouseDown", false);
		}
		else
			afterAction = null;

		if ($('#PEID').val() != $('#PointTemplateTitle').text() && $("#IsTemplatePoint").val() == 'True') {
			// Prompt to confirm changes 
			FMLayout.ConfirmYesNo($("#confirm-name-confirmation-dialog"),
				"Confirm Name Change",
				function () {
					switch (afterAction) {
						case "save":
							$("#ApplyButtonId").data("mouseDown", false);
							ApplyButtonOnClick();
							break;
						case "tags":
							$("#PETags").click();
							break;
						case "modules":
							$("#PEModules").click();
							break;
						case "alarms":
							$("#PEAlarms").click();
							break;
						default:
							return;
					}
				},
				function () {
					$('#PEID').val($('#PointTemplateTitle').text());
					return;
				});
		}

	});

	$("#ApplyButtonId").on("mousedown", function(e){
		$("#ApplyButtonId").data("mouseDown", true);
	});

	$("#PETags").on("mousedown", function (e) {
		$("#PETags").data("mouseDown", true);
	});

	$("#PEModules").on("mousedown", function (e) {
		$("#PEModules").data("mouseDown", true);
	});

	$("#PEAlarms").on("mousedown", function (e) {
		$("#PEAlarms").data("mouseDown", true);
	});


	$("#ApplyButtonId").on("mouseup", function (e) {
		$("#ApplyButtonId").data("mouseDown", false);
	});

	$("#PETags").on("mouseup", function (e) {
		$("#PETags").data("mouseDown", false);
	});

	$("#PEModules").on("mouseup", function (e) {
		$("#PEModules").data("mouseDown", false);
	});

	$("#PEAlarms").on("mouseup", function (e) {
		$("#PEAlarms").data("mouseDown", false);
	});


	// the user changes the min or max the backspace does not work so we will correct it here BDS
	$(".tagDefaultMinimum").on('keydown', function (e) {
		// Get cursor position and key code
		let cursorPos = Number(e.target.selectionStart)
		let keyCode = Number(e.keyCode)

		// only on backspace
		if (keyCode === 8) {
			// Disable text mask events for this cursor positions
			e.preventDefault()
			e.stopPropagation()

			// do backspace delete
			e.target.value = e.target.value.substring(0, cursorPos - 1) + '' + e.target.value.substring(cursorPos, e.target.value.length)
			e.target.setSelectionRange(cursorPos - 1, cursorPos - 1)
		}
		return true;
	});

	$(".tagDefaultMaximum").on('keydown', function (e) {
		// Get cursor position and key code
		let cursorPos = Number(e.target.selectionStart)
		let keyCode = Number(e.keyCode)

		// only on backspace
		if (keyCode === 8) {
			// Disable text mask events for this cursor positions
			e.preventDefault()
			e.stopPropagation()

			// do backspace delete
			e.target.value = e.target.value.substring(0, cursorPos - 1) + '' + e.target.value.substring(cursorPos, e.target.value.length)
			e.target.setSelectionRange(cursorPos - 1, cursorPos - 1)
		}
		return true;
	});

		// the user changes the precision in the settings grid (default units)
	$(".tagDefaultDecimalPlaces").on('input', function (data) {
		FMPointEditor.valuesChanged = true;
		var numDecimals = ~~Number($(this).val());
		
		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);
		numformatInfo.NumberDecimalDigits = parseInt(numDecimals);

		FMPointEditor.replicateDefault($(this).attr("id")); //replicate the number of decimals

		// find the unit for the tag, if it's ft-in-16th or ft-in-8th we can ignore the decimals
		var unit = $(this).closest('tr').find('.tagDefaultUnits').val();
		if (unit == '27' || unit == '19') {
			return true;
		}

		// update the minimum
		var minimum = $(this).closest('tr').find('.tagDefaultMinimum');
		if (!isNaN(parseFloat(minimum.attr('data-raw-value')))) {
			var value = minimum.attr('data-raw-value');
			var newFormattedValue = FMFormatValues.FormatValueFullPrecision(unit, numformatInfo, value);
			minimum.val(newFormattedValue);
			FMPointEditor.resetDefaultInputMask(minimum);
			FMPointEditor.replicateDefault(minimum.attr("id"), false); //replicate the new minimum
		}

		// update the maximum
		var maximum = $(this).closest('tr').find('.tagDefaultMaximum');
		if (!isNaN(parseFloat(maximum.attr('data-raw-value')))) {
			var value = maximum.attr('data-raw-value');
			var newFormattedValue = FMFormatValues.FormatValueFullPrecision(unit, numformatInfo, value);
			maximum.val(newFormattedValue);
			FMPointEditor.resetDefaultInputMask(maximum);
			FMPointEditor.replicateDefault(maximum.attr("id"), false); //replicate the new maximum
		}

		/* update the settings that use the same units */
		var defaultPrecission = $(this).attr("id");

		$("input[data-format=" + defaultPrecission + "]").each(function (index, control) {
			var value = $(control).attr('data-raw-value');
			var controlId = $(control).attr("id");

			if (!isNaN(parseFloat(maximum.attr('data-raw-value')))) {
				var newFormattedValue = FMFormatValues.FormatValueFullPrecision(unit, numformatInfo, value);
				$('#' + controlId).val(newFormattedValue);
				FMPointEditor.resetSettingInputMask($(control));
			}
		});

		return true;
	});

	// leave the default min or max field ( we need to get the formatted value )
	$(".tagDefaultMinimum, .tagDefaultMaximum").on("blur", function (data) {
		var unit = $(this).closest('tr').find('.tagDefaultUnits').val();
		var numDecimals = ~~Number($(this).closest('tr').find('.tagDefaultDecimalPlaces').val());

		var numFormatInfoString = $('#NumberFormatInfoString').val();
		var numformatInfo = JSON.parse(numFormatInfoString);
		numformatInfo.NumberDecimalDigits = numDecimals;
		var formattedValue = $(this).val();
		var unitIndex = (isNaN(unit)) ? ~~0 : ~~Number(unit);
		var controlId = $(this).attr("id");

		//Parse Value
		var newRawValueRounded = math.bignumber(FMFormatValues.ParseValue(unitIndex, numformatInfo, formattedValue, true));
		var currentRawValue = math.bignumber($('#' + controlId).attr('data-raw-value'));
		var currentRawValueRounded = math.bignumber(currentRawValue.toFixed(numformatInfo.NumberDecimalDigits));
		if (!newRawValueRounded.equals(currentRawValueRounded)) {
			$('#' + controlId).attr('data-raw-value', newRawValueRounded.toString());
			// Prompt to confirm changes 
			FMLayout.ConfirmYesNo($("#confirm-range-confirmation-dialog"),
				"Confirm Range Change",
				function () {
					FMPointEditor.replicateDefault(controlId, true); //replicate changes to the tags and update alarm limits
				},
				function () {
					FMPointEditor.replicateDefault(controlId, false); //replicate changes to the tags and do not update alarm limits
				});

			FMPointEditor.valuesChanged = true;
		}

		//Format Value
		var newFormattedValue = FMFormatValues.FormatValueFullPrecision(unitIndex, numformatInfo, newRawValueRounded);
		$('#' + controlId).val(newFormattedValue);
	});

	// when one of the settings gets focus store the value so we can check if we have change it when losing focus
	$(".PESettingPanel").on("focus", "#PEModulePropertyList > div[data-display=input] input", function (data) {
		$(this).data("oldValue", $(this).val());
		return true;

	});

	// change a setting a property value in general settings and lose focus( we need to get the new raw data and the formatted value)
	$(".PESettingPanel").on("blur", "#PEModulePropertyList > div[data-display=input] input", function (data) {
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

		// if we are not using default units no need to format	bds
		if ($(this).attr("data-unit") === "PENoneEngineeringUnits") {
				$('#' + controlId).attr('data-raw-value', formattedValue);
				return true;
		}

		var unit = $("#" + $(this).attr("data-unit")).val(); // get the unit used by the unit type selected
		var numDecimalsControl = $(this).attr("data-format");
		if (numDecimalsControl != 'PESystemDecimalPlaces') {
			numformatInfo.NumberDecimalDigits = ~~Number($("#" + numDecimalsControl).val());
		}
		var newRawValue = $('#' + controlId).attr('data-raw-value');

		//Parse Value (only if it has changed)
		if ($(this).data("oldValue") != $(this).val())
		{
			// 15 digits is the maximum we can reliably work with for double
			if (unit != '27' && unit != '19')
				formattedValue = parseFloat(formattedValue).toPrecision(15);
			
		//formattedValue = Number(formattedValue).toFixed(2);
			newRawValue = FMFormatValues.ParseValue(parseInt(unit), numformatInfo, formattedValue);
				$('#' + controlId).attr('data-raw-value', newRawValue.toString());
		}
		//Format Value
		var newFormattedValue = FMFormatValues.FormatValueFullPrecision(parseInt(unit), numformatInfo, newRawValue);
		$('#' + controlId).val(newFormattedValue);

		return true;

	});



	// if user clicks on the icon in the tag grid header for the Data Source we want to change the grid view
	$('#tagGridEditorSwitch').on('click', function (e) {
		$("#TagEditTable .mainview").toggleClass('hidden');
		$("#TagEditTable .opcview").toggleClass('hidden');
		$("#TagEditTableHeader .mainview").toggleClass('hidden');
		$("#TagEditTableHeader .opcview").toggleClass('hidden');
	});

	// disable dragging pictures around the screen.
	$('img').on('dragstart', function (event) {
		event.preventDefault();
	});

	$('#PEMenuRolloutViewMode').on('click', function (e) {
		if ($("#PEMenuRolloutViewMode").hasClass("listViewSwitch")) {
				$("#PEMenuRolloutViewMode").removeClass("listViewSwitch").addClass("detailViewSwitch");
				$("#PEMenuRolloutViewMode").removeClass("glyphicon-th-list").addClass("glyphicon-align-justify");
				$("#PEMenuRolloutViewMode").attr("title", "Switch to Compact View");
				$('.list-view').css('display', 'none');
				$('.detail-view').css('display', 'inherit');
				$('.PE_RolloutMenuLI').css('height', '100px');
		} else {
				$("#PEMenuRolloutViewMode").removeClass("detailViewSwitch").addClass("listViewSwitch");
				$("#PEMenuRolloutViewMode").removeClass("glyphicon-align-justify").addClass("glyphicon-th-list");
				$("#PEMenuRolloutViewMode").attr("title", "Switch to Detail View");
				$('.list-view').css('display', 'inherit');
				$('.detail-view').css('display', 'none');
				$('.PE_RolloutMenuLI').css('height', '30px');
		}
		$("#PointListDivID").getNiceScroll().resize();
	});

	$('#PEImageRolloutViewMode').on('click', function (e) {
		if ($("#PEImageRolloutViewMode").hasClass("listViewSwitch")) {
			$("#PEImageRolloutViewMode").removeClass("listViewSwitch").addClass("detailViewSwitch");
			$("#PEImageRolloutViewMode").removeClass("glyphicon-th-list").addClass("glyphicon-align-justify");
			$("#PEImageRolloutViewMode").attr("title", "Switch to Compact View");
			$('.list-view').css('display', 'none');
			$('.detail-view').css('display', 'inherit');
			$('.PE_RolloutMenuLI').css('height', '100px');
		} else {
			$("#PEImageRolloutViewMode").removeClass("detailViewSwitch").addClass("listViewSwitch");
			$("#PEImageRolloutViewMode").removeClass("glyphicon-align-justify").addClass("glyphicon-th-list");
			$("#PEImageRolloutViewMode").attr("title", "Switch to Detail View");
			$('.list-view').css('display', 'inherit');
			$('.detail-view').css('display', 'none');
			$('.PE_RolloutMenuLI').css('height', '30px');
		}
		$("#PointImageDivID").getNiceScroll().resize();
	});


	$('#PEPointProduct').select2();
	$('#PEPointOverrideDefaultDrawing').select2();

});


//====================================================================
// This function will handle the point list rollout menu filtering
// for the Point Name filter.
//====================================================================
function PointListRolloutMenuFiltering() {
	
	// No points to be found, just return.
	if ( $('.PE_RolloutMenuLI').length == 0 )
		return;

	$('#PE_RolloutMenuUL').uncolumnize();
	resetMenuRolloutViewMode();

	var pointNameFilterTxt	= $("#PE_RolloutMenuPointNameFilterTB").val();

	if (pointNameFilterTxt === "" && window.selectedPointType === "" && window.selectedCategory === "") {
		$('.PE_RolloutMenuLI').removeClass("hidden");
	}
	else {
		$('.PE_RolloutMenuLI').each(function() {
				var pointTypeFound = false;
				var categoryFound = false;

				var liText = $(this).attr("data-name").toLowerCase();

				if (window.selectedPointType === "") {
					pointTypeFound = true;
				}
				else {
					if ($(this).attr("data-point-type") === window.selectedPointType) {
						pointTypeFound = true;
					}
				}

				if (window.selectedCategory === "") {
					categoryFound = true;
				}
				else {
					// category names are delimited by "|"
					if ($(this).attr("data-point-category").indexOf("|" + window.selectedCategory + "|") >= 0) {
						categoryFound = true;
					}
				}

				if (liText.indexOf(pointNameFilterTxt.toLowerCase()) >= 0 && pointTypeFound && categoryFound) {
					$(this).removeClass("hidden");
				}
				else {
					$(this).removeClass("hidden").addClass("hidden");
				}
		});
	}
	$('#PE_RolloutMenuUL').columnize({
		columns: 2,
		buildOnce: true,
		cssClassPrefix: "points",
		lastNeverTallest: true
	});
}



function OpenNav() {
	
	if ($("#PointSelector").hasClass("opened")) {
		CloseNav();
		return;
	}
	else {
		$("#PointSelector").removeClass("closed").addClass("opened");
	}
	var messageAttributes = { addclass: "stack-bottomright", stack: FMPointEditor.notification_pointselector_stack};
	// remove any previous error messages; 
	PNotify.removeStack(FMPointEditor.notification_pointselector_stack);

	$("#PointSettingListRolloutMenu").removeClass("hidden");

	if ($("#PointListDivID").getNiceScroll()) $("#PointListDivID").getNiceScroll().remove();

	var token = $('#pointPropertiesForm input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	$("#PointListDivID").html('<div id="pointMenuLoader" class="LoadingAnimation transparent"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>');
	$.ajax({
		type: 'Get',
		url: $("#PointMenuSrvURL").val(),
		cache: false,
		headers: headers,
		success: function (response) {
			// we want to display error messages within the menu
			FMErrorAndExceptionHandling.HandleMessages(response, function (view, inError) {
				if (!inError) {
					$("#PointListDivID").html(view);

					$('#PE_RolloutMenuUL').css("height", $('PointListDivID').height());

					resetMenuRolloutViewMode();

					$('#PE_RolloutMenuUL').columnize({
						columns: 2,
						buildOnce: true,
						cssClassPrefix: "points",
						lastNeverTallest: true
					});
					$("#PointListDivID").niceScroll({
						cursorwidth: '10px',
						autohidemode: false,
						cursorcolor: "#486899",
						background: "rgb(240, 240, 240)",
						railoffset: true,
						railpadding: { top: 0, right: 0, left: 10, bottom: 0 },
						smoothscroll: true
					});
				}
				else {
					// remove the content of the list (removes the loading graphic)
					$("#PointListDivID").html('');
				}

			}, messageAttributes);
		},
		error: function(xhr, ajaxOptions, thrownError) {
			FMErrorAndExceptionHandling.ShowException(xhr, ajaxOptions, thrownError, function() { $("#PointListDivID").html(''); }, messageAttributes);
		}
	});

}
/* Based on the current view selection in the point rolldown menu, set the view to display */
function resetMenuRolloutViewMode() {
	if ($("#PEMenuRolloutViewMode").hasClass("listViewSwitch")) {
		$('.list-view').css('display', 'inherit');
		$('.detail-view').css('display', 'none');
		$('.PE_RolloutMenuLI').css('height', '30px');
	} else {
		$('.list-view').css('display', 'none');
		$('.detail-view').css('display', 'inherit');
		$('.PE_RolloutMenuLI').css('height', '100px');
	}
}

/* Set the width of the side navigation (Points list rollout menu) to 0 */
function CloseNav() {
	$("#PointSelector").removeClass("opened").addClass("closed");
	$("#PointSettingListRolloutMenu").removeClass("hidden").addClass("hidden");
	// remove the contents of the point list rolling menu so the DOM is smaller and faster to traverse.
	$("#PointListDivID").html("");
}

function OpenImageSelection() {


	var messageAttributes = { addclass: "stack-bottomright", stack: FMPointEditor.notification_imageselector_stack };
	// remove any previous error messages; 
	PNotify.removeStack(FMPointEditor.notification_imageselector_stack);

	$("#PointTemplateImageMenu").removeClass("hidden");

	if ($("#PointImageDivID").getNiceScroll()) $("#PointImageDivID").getNiceScroll().remove();

	var token = $('#pointPropertiesForm input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	$("#PointImageDivID").html('<div id="pointMenuLoader" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>');
	$.ajax({
		type: 'Get',
		url: $("#ImageSelectionSrvURL").val(),
		cache: false,
		headers: headers,
		success: function (response) {
			// we want to display error messages within the menu
			FMErrorAndExceptionHandling.HandleMessages(response, function (view, inError) {
				if (!inError) {
					$("#PointImageDivID").html(view);
					
					$('#PE_IconNameFilterMenuUL').css("height", $('#PointImageDivID').height());

					resetImageRolloutViewMode();
					
					$('#PE_IconNameFilterMenuUL').columnize({
						columns: 2,
						buildOnce: true,
						cssClassPrefix: "icons",
						lastNeverTallest: true
					});
					$("#PointImageDivID").niceScroll({
						cursorwidth: '10px',
						autohidemode: false,
						cursorcolor: "#486899",
						background: "rgb(240, 240, 240)",
						railoffset: true,
						railpadding: { top: 0, right: 0, left: 10, bottom: 0 },
						smoothscroll: true
					});
				}
				else {
					// remove the content of the list (removes the loading graphic)
					$("#PointImageDivID").html('');
				}

			}, messageAttributes);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			FMErrorAndExceptionHandling.ShowException(xhr, ajaxOptions, thrownError, function () { $("#PointImageDivID").html(''); }, messageAttributes);
		}
	});

}

function CloseImageSelection() {

	$("#PointTemplateImageMenu").removeClass("hidden").addClass("hidden");
	// remove the contents of the point list rolling menu so the DOM is smaller and faster to traverse.
	$("#PointImageDivID").html("");
}

/* Based on the current view selection in the point rolldown menu, set the view to display */
function resetImageRolloutViewMode() {
	if ($("#PEImageRolloutViewMode").hasClass("listViewSwitch")) {
		$('.list-view').css('display', 'inherit');
		$('.detail-view').css('display', 'none');
		$('.PE_RolloutMenuLI').css('height', '30px');
	} else {
		$('.list-view').css('display', 'none');
		$('.detail-view').css('display', 'inherit');
		$('.PE_RolloutMenuLI').css('height', '100px');
	}
}


//====================================================================
// This function will handle the point list rollout menu filtering
// for the Point Name filter.
//====================================================================
function PointImageRolloutMenuFiltering()
{
	// No points to be found, just return.
	if ($('.PE_RolloutMenuLI').length == 0)
		return;

	$('#PE_IconNameFilterMenuUL').uncolumnize();
	resetImageRolloutViewMode();

	var pointNameFilterTxt = $("#PE_IconNameFilterTB").val();

	if (pointNameFilterTxt === "" ) {
		$('.PE_RolloutMenuLI').removeClass("hidden");
	}
	else {
		$('.PE_RolloutMenuLI').each(function () {

			var liText = $(this).attr("data-name").toLowerCase();

			if (liText.indexOf(pointNameFilterTxt.toLowerCase()) >= 0) {
				$(this).removeClass("hidden");
			}
			else {
				$(this).removeClass("hidden").addClass("hidden");
			}
		});
	}
	
	$('#PE_IconNameFilterMenuUL').columnize({
		columns: 2,
		buildOnce: true,
		cssClassPrefix: "icons",
		lastNeverTallest: true
	});
}

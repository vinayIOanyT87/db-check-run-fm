var MovementHandgauge = function ()
{
    var _disableDateTimePickerEvent = null;
    var _imageRootPath = null;
    var _stack_bottomright_movementhandgaugeeditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 60, "firstpos2": 20, "context": $('#MovementHandgaugeEditorPropertyScreen') };

    //=================================================================
    // This function initialize the movement tracking setup.
    //=================================================================
    var _Initialize = function ()
    {
        MovementHandgauge.InitializeDateControls();

        // Load the data from the controller.
        MovementHandgauge.LoadData();

        MovementHandgauge.HandleStartIndividualTimestampsEvent();
        MovementHandgauge.HandleEndIndividualTimestampsEvent();

        MovementHandgauge.CheckModifyRights();
    };

    //=====================================================================
    // This function will return the movement tracking handgauge model 
    // as a string.
    //=====================================================================
    var _GetModelString = function ()
    {
        return $('#MovementHandgaugeModel').val();
    };

    //=====================================================================
    // This function will return the movement tracking setup model as 
    // an object.
    //=====================================================================
    var _GetModel = function ()
    {
        var strModel = MovementHandgauge.GetModelString();

        if (strModel === undefined)
        {
            return undefined;
        }

        var model = JSON.parse(strModel);
        return model;
    };

    //==============================================================
    // This function will reset the model that has changes.
    //==============================================================
    var _SetModel = function (model)
    {
        var modelStr = JSON.stringify(model);
        MovementHandgauge.SetModelString(modelStr);
    };

    //==============================================================
    // This function will reset the model string that has changes.
    //==============================================================
    var _SetModelString = function (modelStr)
    {
        $('#MovementHandgaugeModel').val(modelStr);
    };

    //============================================================
    // This function initializes the date time functions.
    //============================================================
    var _InitializeDateControls = function ()
    {
        var numFormatInfoString = $('#NumberFormatInfoString').val();
        var numFormatInfo = JSON.parse(numFormatInfoString);
        FMLayout.dateFormat = ConvertToJQueryUIDateFormat(numFormatInfo.ShortDatePattern);
        FMLayout.timeFormat = ConvertToJQueryUITimeFormat(numFormatInfo.TimePattern);

        // The start date initializers
        $("#StartLevelTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#StartLevelTimePicker").on("change", function (e)
        {
            MovementHandgauge.UpdateAllStartTimePickers();
        });

        $("#StartTempTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#StartDensityTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#StartStdDensityTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#StartDensityTempTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#StartAmbTempTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#StartWaterLevelTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#StartRefHeightTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        // The end date initializers
        $("#EndLevelTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#EndLevelTimePicker").on("change", function (e) { MovementHandgauge.UpdateAllEndTimePickers(); });

        $("#EndTempTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#EndDensityTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#EndStdDensityTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#EndDensityTempTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#EndAmbTempTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#EndWaterLevelTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });

        $("#EndRefHeightTimePicker").datetimepicker({
            dateFormat: FMLayout.dateFormat,
            timeFormat: FMLayout.timeFormat,
            showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
            beforeShow: function ()
            {
                setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
            }
        });
    };

    //=====================================================================
    // This function will load the page with data from the model.
    //=====================================================================
    var _LoadData = function ()
    {
        var model = MovementHandgauge.GetModel();

        if (!model)
        {
            return;
        }

        MovementHandgauge.LoadStartDataSection(model);
        MovementHandgauge.LoadEndDataSection(model);

        $("#SaveToFinalRowCB").prop('checked', model.SaveToFinalRecord);
    };

    //==================================================================
    // This function will load the start section fields.
    //==================================================================
    var _LoadStartDataSection = function (model)
    {
        $("#StartLevel").val(model.StartLevel);
        $("#StartLevelTimePicker").val(model.StartLevelTime);
        $("#StartTemperature").val(model.StartTemperature);
        $("#StartTempTimePicker").val(model.StartTemperatureTime);
        $("#StartDensity").val(model.StartDensity);
        $("#StartDensityTimePicker").val(model.StartDensityTime);
        $("#StartStdDensity").val(model.StartStdDensity);
        $("#StartStdDensityTimePicker").val(model.StartStdDensityTime);
        $("#StartDensityTemp").val(model.StartDensityTemperature);
        $("#StartDensityTempTimePicker").val(model.StartDensityTemperatureTime);
        $("#StartAmbTemp").val(model.StartAmbientTemperature);
        $("#StartAmbTempTimePicker").val(model.StartAmbientTemperatureTime);
        $("#StartWaterLevel").val(model.StartWaterLevel);
        $("#StartWaterLevelTimePicker").val(model.StartWaterLevelTime);
        $("#StartRefHeight").val(model.StartRefHeight);
        $("#StartRefHeightTimePicker").val(model.StartRefHeightTime);
        $("#StartVolumeTov").val(model.StartVolumeTov);
        $("#StartGrossVolume").val(model.StartGrossVolume);
        $("#StartNetVolume").val(model.StartNetVolume);
        $("#StartWaterVolume").val(model.StartVolumeWater);
        $("#StartMass").val(model.StartMass);
        $("#StartVcf").val(model.StartVcf);
        $("#StartCtsh").val(model.StartCtsh);
        $("#StartEmployeeId").val(model.StartEmployeeId);

        $("#StartEnterIndTimeStampsCB").prop('checked', model.StartEnterIndTimestamps);

        // Update the start unit labels
        $("#StartLevelUnitsLbl").text(model.StartLevelUnits);
        $("#StartTemperatureUnitsLbl").text(model.StartTemperatureUnits);
        $("#StartDensityUnitsLbl").text(model.StartDensityUnits);
        $("#StartStdDensityUnitsLbl").text(model.StartStandardDensityUnits);
        $("#StartDensityTemperatureUnitsLbl").text(model.StartDensityTemperatureUnits);
        $("#StartAmbTemperatureUnitsLbl").text(model.StartAmbientTemperatureUnits);
        $("#StartWaterLevelUnitsLbl").text(model.StartWaterLevelUnits);
        $("#StartVolumeUnitsLbl").text(model.StartVolumeUnits);
        $("#StartGrossVolumeUnitsLbl").text(model.StartGrossVolumeUnits);
        $("#StartNetVolumeUnitsLbl").text(model.StartNetVolumeUnits);
        $("#StartWaterVolumeUnitsLbl").text(model.StartWaterVolumeUnits);
        $("#StartMassUnitsLbl").text(model.StartMassUnits);
    };

    //==================================================================
    // This function will load the end section fields.
    //==================================================================
    var _LoadEndDataSection = function (model)
    {
        $("#EndLevel").val(model.EndLevel);
        $("#EndLevelTimePicker").val(model.EndLevelTime);
        $("#EndTemperature").val(model.EndTemperature);
        $("#EndTempTimePicker").val(model.EndTemperatureTime);
        $("#EndDensity").val(model.EndDensity);
        $("#EndDensityTimePicker").val(model.EndDensityTime);
        $("#EndStdDensity").val(model.EndStdDensity);
        $("#EndStdDensityTimePicker").val(model.EndStdDensityTime);
        $("#EndDensityTemp").val(model.EndDensityTemperature);
        $("#EndDensityTempTimePicker").val(model.EndDensityTemperatureTime);
        $("#EndAmbTemp").val(model.EndAmbientTemperature);
        $("#EndAmbTempTimePicker").val(model.EndAmbientTemperatureTime);
        $("#EndWaterLevel").val(model.EndWaterLevel);
        $("#EndWaterLevelTimePicker").val(model.EndWaterLevelTime);
        $("#EndRefHeight").val(model.EndRefHeight);
        $("#EndRefHeightTimePicker").val(model.EndRefHeightTime);
        $("#EndVolumeTov").val(model.EndVolumeTov);
        $("#EndGrossVolume").val(model.EndGrossVolume);
        $("#EndNetVolume").val(model.EndNetVolume);
        $("#EndWaterVolume").val(model.EndVolumeWater);
        $("#EndMass").val(model.EndMass);
        $("#EndVcf").val(model.EndVcf);
        $("#EndCtsh").val(model.EndCtsh);
        $("#EndEmployeeId").val(model.EndEmployeeId);

        $("#EndEnterIndTimeStampsCB").prop('checked', model.EndEnterIndTimestamps);

        // Update the end unit labels
        $("#EndLevelUnitsLbl").text(model.EndLevelUnits);
        $("#EndTemperatureUnitsLbl").text(model.EndTemperatureUnits);
        $("#EndDensityUnitsLbl").text(model.EndDensityUnits);
        $("#EndStdDensityUnitsLbl").text(model.EndStandardDensityUnits);
        $("#EndDensityTemperatureUnitsLbl").text(model.EndDensityTemperatureUnits);
        $("#EndAmbTemperatureUnitsLbl").text(model.EndAmbientTemperatureUnits);
        $("#EndWaterLevelUnitsLbl").text(model.EndWaterLevelUnits);
        $("#EndVolumeUnitsLbl").text(model.EndVolumeUnits);
        $("#EndGrossVolumeUnitsLbl").text(model.EndGrossVolumeUnits);
        $("#EndNetVolumeUnitsLbl").text(model.EndNetVolumeUnits);
        $("#EndWaterVolumeUnitsLbl").text(model.EndWaterVolumeUnits);
        $("#EndMassUnitsLbl").text(model.EndMassUnits);
    };

    //==================================================================
    // This function will disable all the controls and save button
    // if the user does not have the modify rights.
    //==================================================================
    var _CheckModifyRights = function ()
    {
        var model = MovementHandgauge.GetModel();
        $("#StartLevelTimePicker").attr("disabled", true);
        $("#StartLevelTimePicker").datetimepicker("option", "disabled", true);

        $("#StartTempTimePicker").attr("disabled", true);
        $("#StartTempTimePicker").datetimepicker("option", "disabled", true);

        $("#StartStdDensityTimePicker").attr("disabled", true);
        $("#StartStdDensityTimePicker").datetimepicker("option", "disabled", true);

        $("#StartDensityTempTimePicker").attr("disabled", true);
        $("#StartDensityTempTimePicker").datetimepicker("option", "disabled", true);

        $("#StartAmbTempTimePicker").attr("disabled", true);
        $("#StartAmbTempTimePicker").datetimepicker("option", "disabled", true);

        $("#StartWaterLevelTimePicker").attr("disabled", true);
        $("#StartWaterLevelTimePicker").datetimepicker("option", "disabled", true);

        $("#EndLevelTimePicker").attr("disabled", true);
        $("#EndLevelTimePicker").datetimepicker("option", "disabled", true);

        $("#EndTempTimePicker").attr("disabled", true);
        $("#EndTempTimePicker").datetimepicker("option", "disabled", true);

        $("#EndStdDensityTimePicker").attr("disabled", true);
        $("#EndStdDensityTimePicker").datetimepicker("option", "disabled", true);

        $("#EndDensityTempTimePicker").attr("disabled", true);
        $("#EndDensityTempTimePicker").datetimepicker("option", "disabled", true);

        $("#EndAmbTempTimePicker").attr("disabled", true);
        $("#EndAmbTempTimePicker").datetimepicker("option", "disabled", true);

        $("#EndWaterLevelTimePicker").attr("disabled", true);
        $("#EndWaterLevelTimePicker").datetimepicker("option", "disabled", true);

        $("#StartLevel").attr("disabled", true);
        $("#StartTemperature").attr("disabled", true);
        $("#StartStdDensity").attr("disabled", true);
        $("#StartDensityTemp").attr("disabled", true);
        $("#StartAmbTemp").attr("disabled", true);
        $("#StartWaterLevel").attr("disabled", true);

        $("#EndLevel").attr("disabled", true);
        $("#EndTemperature").attr("disabled", true);
        $("#EndStdDensity").attr("disabled", true);
        $("#EndDensityTemp").attr("disabled", true);
        $("#EndAmbTemp").attr("disabled", true);
        $("#EndWaterLevel").attr("disabled", true);

        $("#StartEnterIndTimeStampsCB").attr("disabled", true);
        $("#EndEnterIndTimeStampsCB").attr("disabled", true);
        $("#SaveToFinalRowCB").attr("disabled", true);

        $("#MovementHandgaugeEditorSavePropertyScreen").attr("disabled", true);

        if (model && model.HasModifyRights)
        {
            $("#StartLevelTimePicker").removeAttr("disabled");
            $("#StartLevelTimePicker").datetimepicker("enable");

            $("#StartTempTimePicker").removeAttr("disabled");
            $("#StartTempTimePicker").datetimepicker("enable");

            $("#StartStdDensityTimePicker").removeAttr("disabled");
            $("#StartStdDensityTimePicker").datetimepicker("enable");

            $("#StartDensityTempTimePicker").removeAttr("disabled");
            $("#StartDensityTempTimePicker").datetimepicker("enable");

            $("#StartAmbTempTimePicker").removeAttr("disabled");
            $("#StartAmbTempTimePicker").datetimepicker("enable");

            $("#StartWaterLevelTimePicker").removeAttr("disabled");
            $("#StartWaterLevelTimePicker").datetimepicker("enable");

            $("#EndLevelTimePicker").removeAttr("disabled");
            $("#EndLevelTimePicker").datetimepicker("enable");

            $("#EndTempTimePicker").removeAttr("disabled");
            $("#EndTempTimePicker").datetimepicker("enable");

            $("#EndStdDensityTimePicker").removeAttr("disabled");
            $("#EndStdDensityTimePicker").datetimepicker("enable");

            $("#EndDensityTempTimePicker").removeAttr("disabled");
            $("#EndDensityTempTimePicker").datetimepicker("enable");

            $("#EndAmbTempTimePicker").removeAttr("disabled");
            $("#EndAmbTempTimePicker").datetimepicker("enable");

            $("#EndWaterLevelTimePicker").removeAttr("disabled");
            $("#EndWaterLevelTimePicker").datetimepicker("enable");

            $("#StartLevel").removeAttr("disabled");
            $("#StartTemperature").removeAttr("disabled");
            $("#StartStdDensity").removeAttr("disabled");
            $("#StartDensityTemp").removeAttr("disabled");
            $("#StartAmbTemp").removeAttr("disabled");
            $("#StartWaterLevel").removeAttr("disabled");

            $("#EndLevel").removeAttr("disabled");
            $("#EndTemperature").removeAttr("disabled");
            $("#EndStdDensity").removeAttr("disabled");
            $("#EndDensityTemp").removeAttr("disabled");
            $("#EndAmbTemp").removeAttr("disabled");
            $("#EndWaterLevel").removeAttr("disabled");

            $("#StartEnterIndTimeStampsCB").removeAttr("disabled");
            $("#EndEnterIndTimeStampsCB").removeAttr("disabled");
            $("#SaveToFinalRowCB").removeAttr("disabled");

            $("#MovementHandgaugeEditorSavePropertyScreen").removeAttr("disabled");
        }
    };

    //============================================================================
    // This function will update will call to recalculate the values.
    //============================================================================
    var _HandleFieldOnblur = function (fieldId, fieldType)
    {
        var fieldValue = $("#" + fieldId).val();

        // Check to see if the field value has changed.
        var hasValueChanged = MovementHandgauge.HasFieldChanged(fieldId, fieldValue);

        // If no change, then return.
        if (hasValueChanged == false)
        {
            return;
        }

        // Perform validation.
        var isFieldValid = MovementHandgauge.ValidFieldValue(fieldId, fieldValue, fieldType);

        if (isFieldValid == false)
        {
            return;
        }

        var modelStr = MovementHandgauge.GetModelString();
        var url = $("#CalculateMovementHandgaugeDataUrl").val();
        var token = $('#MovementHandgaugeEditorForm input[name=__RequestVerificationToken]').val();
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
                    MovementHandgauge.SetModel(data.Data);
                    MovementHandgauge.LoadData();
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
        var model = MovementHandgauge.GetModel();

        if (fieldId === "StartLevel" && model.StartLevel !== fieldValue)
        {
            model.StartLevel = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartLevelTimePicker" && model.StartLevelTime !== fieldValue) 
        {
            model.StartLevelTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartTemperature" && model.StartTemperature !== fieldValue)
        {
            model.StartTemperature = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartTempTimePicker" && model.StartTemperatureTime !== fieldValue) 
        {
            model.StartTemperatureTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartStdDensity" && model.StartStdDensity !== fieldValue) 
        {
            model.StartStdDensity = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartStdDensityTimePicker" && model.StartStdDensityTime !== fieldValue)
        {
            model.StartStdDensityTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartDensityTemp" && model.StartDensityTemperature !== fieldValue)
        {
            model.StartDensityTemperature = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartDensityTempTimePicker" && model.StartDensityTemperatureTime !== fieldValue)
        {
            model.StartDensityTemperatureTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartAmbTemp" && model.StartAmbientTemperature !== fieldValue)
        {
            model.StartAmbientTemperature = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartAmbTempTimePicker" && model.StartAmbientTemperatureTime !== fieldValue)
        {
            model.StartAmbientTemperatureTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartWaterLevel" && model.StartWaterLevel !== fieldValue)
        {
            model.StartWaterLevel = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartWaterLevelTimePicker" && model.StartWaterLevelTime !== fieldValue)
        {
            model.StartWaterLevelTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartRefHeight" && model.StartRefHeight !== fieldValue) 
        {
            model.StartRefHeight = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "StartRefHeightTimePicker" && model.StartRefHeightTime !== fieldValue)
        {
            model.StartRefHeightTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndLevel" && model.EndLevel !== fieldValue)
        {
            model.EndLevel = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndLevelTimePicker" && model.EndLevelTime !== fieldValue)
        {
            model.EndLevelTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndTemperature" && model.EndTemperature !== fieldValue)
        {
            model.EndTemperature = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndTempTimePicker" && model.EndTemperatureTime !== fieldValue)
        {
            model.EndTemperatureTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndStdDensity" && model.EndStdDensity !== fieldValue)
        {
            model.EndStdDensity = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndStdDensityTimePicker" && model.EndStdDensityTime !== fieldValue)
        {
            model.EndStdDensityTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndDensityTemp" && model.EndDensityTemperature !== fieldValue)
        {
            model.EndDensityTemperature = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndDensityTempTimePicker" && model.EndDensityTemperatureTime !== fieldValue)
        {
            model.EndDensityTemperatureTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndAmbTemp" && model.EndAmbientTemperature !== fieldValue)
        {
            model.EndAmbientTemperature = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndAmbTempTimePicker" && model.EndAmbientTemperatureTime !== fieldValue)
        {
            model.EndAmbientTemperatureTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndWaterLevel" && model.EndWaterLevel !== fieldValue)
        {
            model.EndWaterLevel = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndWaterLevelTimePicker" && model.EndWaterLevelTime !== fieldValue)
        {
            model.EndWaterLevelTime = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndRefHeight" && model.EndRefHeight !== fieldValue)
        {
            model.EndRefHeight = fieldValue;
            MovementHandgauge.SetModel(model);
            return true;
        }

        if (fieldId === "EndRefHeightTimePicker" && model.EndRefHeightTime !== fieldValue)
        {
            model.EndRefHeightTime = fieldValue;
            MovementHandgauge.SetModel(model);
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

        if (fieldType === "LEVEL")
        {
            var isValid = MovementHandgauge.IsLevelValid(fieldValue);

            if (isValid == false)
            {
                $("#" + fieldId).val("");
                alert("Invalid Level value for field '" + fieldId + "', must be xx-xx-xx.");
            }

            return MovementHandgauge.IsLevelValid(fieldValue);
        }

        if (fieldType === "DOUBLE")
        {
            if (isNaN(fieldValue))
            {
                $("#" + fieldId).val("");
                alert("Invalid value for field '" + fieldId + "', must be numeric.");
            }
            else
            {
                return true;
            }
        }
    };

    //=======================================================================
    // This function will convert a numeric level into a level format
    // 00-00-00.
    //=======================================================================
    var _ConvertToLevelFormat = function (levelDouble)
    {
        var fractional = 16;

        // Get the Feet part of the double
        var feetInt = parseInt(levelDouble, 10);
        var fractionPartDouble = levelDouble - feetInt;

        // Get the inches part of the fraction.
        var inchesDouble = fractionPartDouble * 12;
        var inchesInt = parseInt(inchesDouble, 10);

        // Get the sixteenths or eighths part of the fraction.
        fractionPartDouble = inchesDouble - inchesInt;
        var sixteenthEighthInt = parseInt((fractionPartDouble * fractional), 10);

        // Format the level string as xx-xx-xx.
        var level = (feetInt < 10 ? "0" + feetInt.ToString() : feetInt.ToString()) + "-"
            + (inchesInt < 10 ? "0" + inchesInt.ToString() : inchesInt.ToString()) + "-"
            + (sixteenthEighthInt < 10 ? "0" + sixteenthEighthInt.ToString() : sixteenthEighthInt.ToString());

        return level;
    };

    //=================================================================
    // This function will valid if the level string is valid. Returns
    // true if valid.
    //=================================================================
    var _IsLevelValid = function (levelStr)
    {
        if (levelStr == null || levelStr === "")
        {
            return false;
        }

        var parts = levelStr.split("-");

        if (parts.length != 3)
        {
            return false;
        }

        if (isNaN(parts[0]) || isNaN(parts[1]) || isNaN(parts[2]))
        {
            return false;
        }

        return true;
    };

    //====================================================================
    // This function will convert the Level into a double.
    //====================================================================
    var _ConvertToLevelDouble = function (levelStr)
    {
        var parts = levelStr.split("-");

        var fractional = 16.0;
        var feet = parseFloat(parts[0]);
        var inches = parseFloat(parts[1]);
        var sixteenthEighth = parseFloat(parts[2]);

        var levelDouble = (((sixteenthEighth / fractional) + inches) / 12.0) + feet;
        return levelDouble;
    };

    //======================================================================
    // This function handles the Start Enter Individual Timestamps checkbox
    // on change event.
    //======================================================================
    var _HandleStartIndividualTimestampsEvent = function ()
    {
        var checked = $("#StartEnterIndTimeStampsCB").prop("checked");

        $('#StartTempTimePicker').removeAttr('disabled');
        $('#StartStdDensityTimePicker').removeAttr('disabled');
        $('#StartDensityTempTimePicker').removeAttr('disabled');
        $('#StartAmbTempTimePicker').removeAttr('disabled');
        $('#StartWaterLevelTimePicker').removeAttr('disabled');

        // For future
        //$('#StartRefHeightTimePicker').removeAttr('disabled');
        $('#StartRefHeightTimePicker').prop('disabled', true);

        if (checked == false)
        {
            $('#StartTempTimePicker').prop('disabled', true);
            $('#StartStdDensityTimePicker').prop('disabled', true);
            $('#StartDensityTempTimePicker').prop('disabled', true);
            $('#StartAmbTempTimePicker').prop('disabled', true);
            $('#StartWaterLevelTimePicker').prop('disabled', true);
            $('#StartRefHeightTimePicker').prop('disabled', true);
        }
    };

    //======================================================================
    // This function handles the End Enter Individual Timestamps checkbox
    // on change event.
    //======================================================================
    var _HandleEndIndividualTimestampsEvent = function ()
    {
        var checked = $("#EndEnterIndTimeStampsCB").prop("checked");

        $('#EndTempTimePicker').removeAttr('disabled');
        $('#EndStdDensityTimePicker').removeAttr('disabled');
        $('#EndDensityTempTimePicker').removeAttr('disabled');
        $('#EndAmbTempTimePicker').removeAttr('disabled');
        $('#EndWaterLevelTimePicker').removeAttr('disabled');

        // For future
        //$('#EndRefHeightTimePicker').removeAttr('disabled');
        $('#EndRefHeightTimePicker').prop('disabled', true);

        if (checked == false)
        {
            $('#EndTempTimePicker').prop('disabled', true);
            $('#EndStdDensityTimePicker').prop('disabled', true);
            $('#EndDensityTempTimePicker').prop('disabled', true);
            $('#EndAmbTempTimePicker').prop('disabled', true);
            $('#EndWaterLevelTimePicker').prop('disabled', true);
            $('#EndRefHeightTimePicker').prop('disabled', true);
        }
    };

    //==================================================================
    // This function handles the on change event for the start level
    // date time picker. If enter individual timestamps is not set,
    // then populate all the timestamps with the start level value.
    //==================================================================
    var _UpdateAllStartTimePickers = function ()
    {
        var checked = $("#StartEnterIndTimeStampsCB").prop("checked");

        if (checked == false)
        {
            var selectedDateTime = $("#StartLevelTimePicker").val();

            $('#StartTempTimePicker').val(selectedDateTime);
            $('#StartStdDensityTimePicker').val(selectedDateTime);
            $('#StartDensityTempTimePicker').val(selectedDateTime);
            $('#StartAmbTempTimePicker').val(selectedDateTime);
            $('#StartWaterLevelTimePicker').val(selectedDateTime);
            $('#StartRefHeightTimePicker').val(selectedDateTime);
        }
    };

    //==================================================================
    // This function handles the on change event for the end level
    // date time picker. If enter individual timestamps is not set,
    // then populate all the timestamps with the end level value.
    //==================================================================
    var _UpdateAllEndTimePickers = function ()
    {
        var checked = $("#EndEnterIndTimeStampsCB").prop("checked");

        if (checked == false)
        {
            var selectedDateTime = $("#EndLevelTimePicker").val();

            $('#EndTempTimePicker').val(selectedDateTime);
            $('#EndStdDensityTimePicker').val(selectedDateTime);
            $('#EndDensityTempTimePicker').val(selectedDateTime);
            $('#EndAmbTempTimePicker').val(selectedDateTime);
            $('#EndWaterLevelTimePicker').val(selectedDateTime);
            $('#EndRefHeightTimePicker').val(selectedDateTime);
        }
    };

    //======================================================================
    // This function handles the End Enter Individual Timestamps checkbox
    // on change event.
    //======================================================================
    var _HandleSaveToFinalRowEvent = function ()
    {
        var model = MovementHandgauge.GetModel();

        if (!model)
        {
            return;
        }

        model.SaveToFinalRecord = $("#SaveToFinalRowCB").is(":checked");
        MovementHandgauge.SetModel(model);
    };

    //==========================================================
    // This function will update the model based on the changes
    // on the UI.
    //==========================================================
    var _UpdateModel = function ()
    {
        var model = MovementHandgauge.GetModel();

        if (!model)
        {
            return;
        }

        model.StartLevel = $("#StartLevel").val();
        model.StartLevelTime = $("#StartLevelTimePicker").val();
        model.StartTemperature = $("#StartTemperature").val();
        model.StartTemperatureTime = $("#StartTempTimePicker").val();
        model.StartDensity = $("#StartDensity").val();
        model.StartDensityTime = $("#StartDensityTimePicker").val();
        model.StartStdDensity = $("#StartStdDensity").val();
        model.StartStdDensityTime = $("#StartStdDensityTimePicker").val();
        model.StartDensityTemperature = $("#StartDensityTemp").val();
        model.StartDensityTemperatureTime = $("#StartDensityTempTimePicker").val();
        model.StartAmbientTemperature = $("#StartAmbTemp").val();
        model.StartAmbientTemperatureTime = $("#StartAmbTempTimePicker").val();
        model.StartWaterLevel = $("#StartWaterLevel").val();
        model.StartWaterLevelTime = $("#StartWaterLevelTimePicker").val();
        model.StartRefHeight = $("#StartRefHeight").val();
        model.StartRefHeightTime = $("#StartRefHeightTimePicker").val();
        model.StartVolumeTov = $("#StartVolumeTov").val();
        model.StartGrossVolume = $("#StartGrossVolume").val();
        model.StartNetVolume = $("#StartNetVolume").val();
        model.StartWaterVolume = $("#StartWaterVolume").val();
        model.StartMass = $("#StartMass").val();
        model.StartVcf = $("#StartVcf").val();
        model.StartCtsh = $("#StartCtsh").val();
        model.StartEmployeeId = $("#StartEmployeeId").val();
        model.StartEnterIndTimestamps = $("#StartEnterIndTimeStampsCB").is(":checked");

        model.EndLevel = $("#EndLevel").val();
        model.EndLevelTime = $("#EndLevelTimePicker").val();
        model.EndTemperature = $("#EndTemperature").val();
        model.EndTemperatureTime = $("#EndTempTimePicker").val();
        model.EndDensity = $("#EndDensity").val();
        model.EndDensityTime = $("#EndDensityTimePicker").val();
        model.EndStdDensity = $("#EndStdDensity").val();
        model.EndStdDensityTime = $("#EndStdDensityTimePicker").val();
        model.EndDensityTemperature = $("#EndDensityTemp").val();
        model.EndDensityTemperatureTime = $("#EndDensityTempTimePicker").val();
        model.EndAmbientTemperature = $("#EndAmbTemp").val();
        model.EndAmbientTemperatureTime = $("#EndAmbTempTimePicker").val();
        model.EndWaterLevel = $("#EndWaterLevel").val();
        model.EndWaterLevelTime = $("#EndWaterLevelTimePicker").val();
        model.EndRefHeight = $("#EndRefHeight").val();
        model.EndRefHeightTime = $("#EndRefHeightTimePicker").val();
        model.EndVolumeTov = $("#EndVolumeTov").val();
        model.EndGrossVolume = $("#EndGrossVolume").val();
        model.EndNetVolume = $("#EndNetVolume").val();
        model.EndWaterVolume = $("#EndWaterVolume").val();
        model.EndMass = $("#EndMass").val();
        model.EndVcf = $("#EndVcf").val();
        model.EndCtsh = $("#EndCtsh").val();
        model.EndEmployeeId = $("#EndEmployeeId").val();
        model.EndEnterIndTimestamps = $("#EndEnterIndTimeStampsCB").is(":checked");

        model.SaveToFinalRecord = $("#SaveToFinalRowCB").is(":checked");

        MovementHandgauge.SetModel(model);
    };

    //======================================================================
    // This function will save the changes on the handgauge page.
    //======================================================================
    var _SaveChanges = function ()
    {
        var caller = 1; // CallerMovementHistory

        // Update the model based on the UI changes.
        MovementHandgauge.UpdateModel();

        var url = $('#SaveMovementHandgaugeDataUrl').val();
        var token = $('#MovementHandgaugeEditorForm input[name=__RequestVerificationToken]').val();
        var headers = {};
        headers['__RequestVerificationToken'] = token;

        // notification position
        var messageAttributes = { addclass: 'stack-bottomright', stack: MovementHandgauge.Stack_bottomright_movementhandgaugeeditor, width: '450px' };

        // remove previous notifications
        PNotify.removeStack(MovementHandgauge.Stack_bottomright_movementhandgaugeeditor);

        var movementHandgaugeEditorModelStr = MovementHandgauge.GetModelString();

        $.ajax({
            cache: false,
            url: url,
            type: 'POST',
            headers: headers,
            async: false,
            dataType: "json",
            contentType: 'application/json; charset=UTF-8',
            data: JSON.stringify({ 'movementHandgaugeEditorModelStr': movementHandgaugeEditorModelStr, "caller": caller }),
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

    //======================================================
    // Return function pointers
    //======================================================
    return {
        SaveChanges: _SaveChanges
        , UpdateModel: _UpdateModel
        , HandleSaveToFinalRowEvent: _HandleSaveToFinalRowEvent
        , UpdateAllEndTimePickers: _UpdateAllEndTimePickers
        , UpdateAllStartTimePickers: _UpdateAllStartTimePickers
        , HandleEndIndividualTimestampsEvent: _HandleEndIndividualTimestampsEvent
        , HandleStartIndividualTimestampsEvent: _HandleStartIndividualTimestampsEvent
        , ConvertToLevelDouble: _ConvertToLevelDouble
        , IsLevelValid: _IsLevelValid
        , ConvertToLevelFormat: _ConvertToLevelFormat
        , ValidFieldValue: _ValidFieldValue
        , HasFieldChanged: _HasFieldChanged
        , HandleFieldOnblur: _HandleFieldOnblur
        , LoadEndDataSection: _LoadEndDataSection
        , LoadStartDataSection: _LoadStartDataSection
        , LoadData: _LoadData
        , InitializeDateControls: _InitializeDateControls
        , SetModelString: _SetModelString
        , SetModel: _SetModel
        , GetModel: _GetModel
        , GetModelString: _GetModelString
        , Initialize: _Initialize
        , Stack_bottomright_movementhandgaugeeditor: _stack_bottomright_movementhandgaugeeditor
        , CheckModifyRights: _CheckModifyRights
    };
}();
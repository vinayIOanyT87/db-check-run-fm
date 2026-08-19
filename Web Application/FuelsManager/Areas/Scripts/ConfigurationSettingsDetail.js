var FMConfigurationSettingsDetail = FMConfigurationSettingsDetail ||
	{
	    currentMarkerUrl: ""
	, crumbMarkerUrl: ""
	};


//=====================================================================
// This function will return the configuration settings detail model 
// as a string.
//=====================================================================
FMConfigurationSettingsDetail.GetConfigSettingsDetailModelString = function ()
{
    return $('#ConfigSettingDetailModel').val();
};

//=====================================================================
// This function will return the map view model as an object.
//=====================================================================
FMConfigurationSettingsDetail.GetConfigSettingsDetailModel = function ()
{
    var strModel = FMConfigurationSettingsDetail.GetConfigSettingsDetailModelString();

    if (strModel === undefined)
    {
        return undefined;
    }

    var model = JSON.parse(strModel);
    return model;
};

//========================================================================
// This function will convert the model to a string and save it.
//========================================================================
FMConfigurationSettingsDetail.SetModel = function (model)
{
    var modelStr = JSON.stringify(model);
    FMConfigurationSettingsDetail.SetModelString(modelStr);
};

//========================================================================
// This function will set the configuration settings detail model as 
// a string. In the ConfigSettingDetailModel HTML field.
//========================================================================
FMConfigurationSettingsDetail.SetModelString = function (modelStr) {
    return $('#ConfigSettingDetailModel').val(modelStr);
};

//========================================================================
// This function will handle the OK button event.  It will call the
// controller to update the changes.
//========================================================================
FMConfigurationSettingsDetail.HandleOkBtnEvent = function ()
{
    FMConfigurationSettingsDetail.UpdateChangesToModel();
    var modelStr = FMConfigurationSettingsDetail.GetConfigSettingsDetailModelString();

    var url = $("#ConfigSettingDetailUrl").val();
    var callData = { modelStr: modelStr };

    var token = $('input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    $.ajax({
        type: 'Post',
        async: false,
        data: callData,
        url: url,
        headers: headers,
        cache: false,
        success: function (result) {
            if (result.ErrorFlag)
            {
                // display error
                alert(result.ErrorMessage);
                //return;
            }

            var redirectUrl = $("#ConfigSettingSummaryUrl").val();
            location.href = redirectUrl;
        },
        error: function (xhr, textStatus, error) {
            //var modalManager = $('body').data('modalmanager');
            //modalManager.removeLoading();
            var errorMessage = xhr + "; " + textStatus + "; " + error;
            alert(errorMessage);
        }
    });
}

//===========================================================================
// This function will update the model with the new setting value.
//===========================================================================
FMConfigurationSettingsDetail.UpdateChangesToModel = function ()
{
    var model = FMConfigurationSettingsDetail.GetConfigSettingsDetailModel();

    // Update model with page changes
    var settingValue = $("#SettingValue").val();
    model.settingValue = settingValue;

    FMConfigurationSettingsDetail.SetModel(model);
}
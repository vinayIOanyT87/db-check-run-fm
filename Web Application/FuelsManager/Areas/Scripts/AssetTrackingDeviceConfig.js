var AssetTrackingDeviceConfiguration = AssetTrackingDeviceConfiguration ||
{

};

//=========================================================================
// This function handles the select event of the Equipment dropdown control.
// It will set the selected value into a hidden field to be consumed
// by the controller.
//=========================================================================
AssetTrackingDeviceConfiguration.GetSelectedEquipment = function ()
{
    var selectedEquipment = $("#EquipmentIdDD").val();
    $('#selectedEquipmentInput').val(selectedEquipment);
}

//=========================================================================
// This function handles the select event of the Equipment dropdown control.
// It will set the selected value into a hidden field to be consumed
// by the controller.
//=========================================================================
AssetTrackingDeviceConfiguration.GetSelectedSourceUnit = function ()
{
    var selectedSourceUnit = $("#SourceUnitDD").val();
    $('#selectedSourceUnitInput').val(selectedSourceUnit);
}

//=========================================================================
// This function handles the select event of the Device Type dropdown control.
// It will set the selected value into a hidden field to be consumed
// by the controller.
//=========================================================================
AssetTrackingDeviceConfiguration.GetSelectedDeviceType = function()
{
    var selectedDeviceType = $("#DeviceTypesDD").val();
    $('#selectedDeviceTypeInput').val(selectedDeviceType);

    if (selectedDeviceType === "Tdu")
    {
        var model = AssetTrackingDeviceConfiguration.GetModel();
        var previousEquipValue = model.EquipmentGuidStr;
        $('#previousEquipmentSelection').val(previousEquipValue);

        $("#EquipmentIdDD").val("-99");
        $("#EquipmentIdDD").attr("disabled", true);
        $('#selectedEquipmentInput').val("-99");
    }
    else
    {
        var previousEquip = $('#previousEquipmentSelection').val();

        if (previousEquip != null && previousEquip !== "")
        {
            $("#EquipmentIdDD").val(previousEquip);
            $("#EquipmentIdDD").attr("disabled", false);
            $('#selectedEquipmentInput').val(previousEquip);
        }
    }
}

//=================================================================
// This function will set the equipment dropdown mode on page
// startup.
//=================================================================
AssetTrackingDeviceConfiguration.SetEquipmentDropdownMode = function()
{
    var selectedDeviceType = $("#DeviceTypesDD").val();
    $('#selectedDeviceTypeInput').val(selectedDeviceType);

    var previousEquipValue = $("#EquipmentIdDD").val();
    $('#previousEquipmentSelection').val(previousEquipValue);

    $("#EquipmentIdDD").attr("disabled", false);

    if (selectedDeviceType == "Tdu")
    {
        $("#EquipmentIdDD").attr("disabled", true);
    }
}

//============================================================
// This function will disable the OK and New buttons.
//============================================================
AssetTrackingDeviceConfiguration.DisableButtons = function ()
{
    $("#NewDeviceBtn").prop("disabled", true);
    $("#OkDeviceBtn").prop("disabled", true);
}

//============================================================
// This function will enable the OK and New buttons.
//============================================================
AssetTrackingDeviceConfiguration.EnableButtons = function ()
{
    var model = AssetTrackingDeviceConfiguration.GetModel();
    AssetTrackingDeviceConfiguration.DisableButtons();

    if (model.IsEditable)
    {
        $("#NewDeviceBtn").removeAttr("disabled");
        $("#OkDeviceBtn").removeAttr("disabled");
    }
}

//==============================================================
// This function will load the page with the data.
//==============================================================
AssetTrackingDeviceConfiguration.LoadPage = function()
{
    var model = AssetTrackingDeviceConfiguration.GetModel();
    $("#DeviceIdTB").val(model.DeviceId);
    $("#DescriptionTB").val(model.Description);
    $("#ModelNumberTB").val(model.ModelNumber);
    $("#SerialNumberTB").val(model.SerialNumber);
    $("#ActiveCB").prop("checked", model.Active);
    $("#RedirectToEquipmentUrl").val(model.RedirectToEquipmentUrl);

    AssetTrackingDeviceConfiguration.DisableButtons();
    if (model.DeviceId != null && model.DeviceId !== "")
    {
        AssetTrackingDeviceConfiguration.EnableButtons();
    }

    var sourceUnitDropdown = document.getElementById("SourceUnitDD");
    for(var itemIndex = 0; itemIndex < model.ActionSourceUnits.length; itemIndex++)
    {
        var selectedListItem = model.ActionSourceUnits[itemIndex];
        var opt = document.createElement("option");
        opt.value = selectedListItem.Value;
        opt.innerHTML = selectedListItem.Text;
        sourceUnitDropdown.appendChild(opt);

        if(selectedListItem.Selected)
        {
            sourceUnitDropdown.selectedIndex = itemIndex;
            AssetTrackingDeviceConfiguration.GetSelectedSourceUnit();
        }
    }

    var equipmentIdDropdown = document.getElementById("EquipmentIdDD");
    for (var itemIndex = 0; itemIndex < model.ActionListEquipment.length; itemIndex++)
    {
        var selectedListItem = model.ActionListEquipment[itemIndex];
        var opt = document.createElement("option");
        opt.value = selectedListItem.Value;
        opt.innerHTML = selectedListItem.Text;
        equipmentIdDropdown.appendChild(opt);

        if (selectedListItem.Selected)
        {
            equipmentIdDropdown.selectedIndex = itemIndex;
            AssetTrackingDeviceConfiguration.GetSelectedEquipment();
        }
    }

    var deviceTypesDropdown = document.getElementById("DeviceTypesDD");
    for (var itemIndex = 0; itemIndex < model.ActionListDeviceTypes.length; itemIndex++)
    {
        var selectedListItem = model.ActionListDeviceTypes[itemIndex];
        var opt = document.createElement("option");
        opt.value = selectedListItem.Value;
        opt.innerHTML = selectedListItem.Text;
        deviceTypesDropdown.appendChild(opt);

        if (selectedListItem.Selected)
        {
            deviceTypesDropdown.selectedIndex = itemIndex;
            AssetTrackingDeviceConfiguration.GetSelectedDeviceType();
        }
    }

    var associatedTanksListbox = document.getElementById("AssociatedTanksLB");
    for (var itemIndex = 0; itemIndex < model.ActionListAssociatedTanks.length; itemIndex++)
    {
        var selectedListItem = model.ActionListAssociatedTanks[itemIndex];
        var opt = document.createElement("option");
        opt.value = selectedListItem.Value;
        opt.innerHTML = selectedListItem.Text;
        associatedTanksListbox.appendChild(opt);
    }
}

//===============================================================
// This function will reset the fields on the page.
//===============================================================
AssetTrackingDeviceConfiguration.ResetFields = function()
{
    $("#DeviceIdTB").val("");
    $("#DescriptionTB").val("");
    $("#ModelNumberTB").val("");
    $("#SerialNumberTB").val("");
    $("#ActiveCB").prop("checked", false);

    $("#selectedEquipmentInput").val("");
    $("#selectedDeviceTypeInput").val("");
    $("#previousEquipmentSelection").val("");
    $("#selectedSourceUnitInput").val("");

    $("#EquipmentIdDD").empty();
    $("#DeviceTypesDD").empty();
    $("#SourceUnitDD").empty();
    $("#AssociatedTanksLB").empty();
}

//======================================================================
// This function will handle the Device ID textbox change event.
//======================================================================
AssetTrackingDeviceConfiguration.HandleDeviceTbEvent = function()
{
    var deviceIdValue = $("#DeviceIdTB").val();

    if (deviceIdValue.length === 0)
    {
        AssetTrackingDeviceConfiguration.DisableButtons();
    }
    else
    {
        AssetTrackingDeviceConfiguration.EnableButtons();
    }
}

//===============================================================
// This function handles the OK button on click event.
//===============================================================
AssetTrackingDeviceConfiguration.HandleOkButtonEvent = function()
{
    var url = "";
    var callData;
    var modelStr;

    // Return if all fields are not populated.
    if ($("#DeviceIdTB").val() == null || $("#DeviceIdTB").val() === "")
    {
        return;
    }

    var model = AssetTrackingDeviceConfiguration.GetModel();

    model.DeviceId          = $("#DeviceIdTB").val();
    model.Description       = $("#DescriptionTB").val();
    model.ModelNumber       = $("#ModelNumberTB").val();
    model.SerialNumber      = $("#SerialNumberTB").val();
    model.Active            = $("#ActiveCB").prop("checked");
    model.SelectedEquipment = $('#selectedEquipmentInput').val();
    model.AssetTrackingDeviceType = $("#selectedDeviceTypeInput").val();

    if ($("#selectedSourceUnitInput").val() !== "")
    {
        model.SourceUnit = $("#selectedSourceUnitInput").val();
    }

    AssetTrackingDeviceConfiguration.SetModel(model);
    modelStr = AssetTrackingDeviceConfiguration.GetModelString();

    url = $("#GetAssetTrackingConfigurationUpdateUrl").val();
    callData = { modelStr: modelStr };

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
        success: function (result)
        {
            if (result.ErrorFlag)
            {
                // display error
                AssetTrackingDeviceConfiguration.DisplayError(result.ErrorMessage);
                AssetTrackingDeviceConfiguration.SetModel(result.AssetDeviceConfigModel);
                return;
            }

            var redirectUrl = $("#RedirectToDeviceSummary").val();
            var equipDirectUrl = $("#RedirectToEquipmentUrl").val();

            if (equipDirectUrl != null && equipDirectUrl !== "")
            {
                var rootLength = redirectUrl.indexOf("))/") + 3;
                var rootUrl = redirectUrl.substr(0, rootLength);

                redirectUrl = rootUrl + equipDirectUrl;
            }

            location.href = redirectUrl;
        },
        error: function (xhr, textStatus, error)
        {
            //var modalManager = $('body').data('modalmanager');
            //modalManager.removeLoading();
            var errorMessage = xhr + "; " + textStatus + "; " + error;
            AssetTrackingDeviceConfiguration.DisplayError(errorMessage);
        }
    });
}

//===============================================================
// This function handles the NEW button on click event.
//===============================================================
AssetTrackingDeviceConfiguration.HandleNewButtonEvent = function ()
{
    var url = "";
    var callData;
    var modelStr;

    // Return if all fields are not populated.
    if ($("#DeviceIdTB").val() == null || $("#DeviceIdTB").val() === "")
    {
        return;
    }

    var model = AssetTrackingDeviceConfiguration.GetModel();

    model.DeviceId          = $("#DeviceIdTB").val();
    model.Description       = $("#DescriptionTB").val();
    model.ModelNumber       = $("#ModelNumberTB").val();
    model.SerialNumber      = $("#SerialNumberTB").val();
    model.Active            = $("#ActiveCB").prop("checked");
    model.SelectedEquipment = $('#selectedEquipmentInput').val();
    model.AssetTrackingDeviceType = $("#selectedDeviceTypeInput").val();

    if ($("#selectedSourceUnitInput").val() !== "")
    {
        model.SourceUnit = $("#selectedSourceUnitInput").val();
    }

    AssetTrackingDeviceConfiguration.SetModel(model);
    modelStr = AssetTrackingDeviceConfiguration.GetModelString();

    url = $("#GetAssetTrackingConfigurationNewUrl").val();
    callData = { modelStr: modelStr };

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
        success: function (result)
        {
            if (result.ErrorFlag)
            {
                // display error
                AssetTrackingDeviceConfiguration.DisplayError(result.ErrorMessage);
                AssetTrackingDeviceConfiguration.SetModel(result.AssetDeviceConfigModel);
                return;
            }

            AssetTrackingDeviceConfiguration.ResetFields();
            AssetTrackingDeviceConfiguration.SetModel(result.AssetDeviceConfigModel);
            AssetTrackingDeviceConfiguration.LoadPage();

        },
        error: function (xhr, textStatus, error)
        {
            //var modalManager = $('body').data('modalmanager');
            //modalManager.removeLoading();
            var errorMessage = xhr + "; " + textStatus + "; " + error;
            AssetTrackingDeviceConfiguration.DisplayError(errorMessage);
        }
    });
}

//===============================================================
// This function handles the Cancel button on click event.
//===============================================================
AssetTrackingDeviceConfiguration.HandleCancelButtonEvent = function ()
{
    var redirectUrl = $("#RedirectToDeviceSummary").val();
    var equipDirectUrl = $("#RedirectToEquipmentUrl").val();

    if (equipDirectUrl != null && equipDirectUrl !== "")
    {
        var rootLength = redirectUrl.indexOf("))/") + 3;
        var rootUrl = redirectUrl.substr(0, rootLength);

        redirectUrl = rootUrl + equipDirectUrl;
    }

    location.href = redirectUrl;
}

//=============================================================
// This function will display an error message if there was
// a message.
//=============================================================
AssetTrackingDeviceConfiguration.DisplayError = function(errorMsg)
{
    alert(errorMsg);
}

//=========================================================
// This function will initialize the page.
//=========================================================
AssetTrackingDeviceConfiguration.Init = function ()
{
    AssetTrackingDeviceConfiguration.LoadPage();
    AssetTrackingDeviceConfiguration.SetEquipmentDropdownMode();
}

//========================================================================
// This function will return the custom page menu model as an object.
//========================================================================
AssetTrackingDeviceConfiguration.GetModel = function ()
{
    var strModel = AssetTrackingDeviceConfiguration.GetModelString();
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
AssetTrackingDeviceConfiguration.SetModel = function (model)
{
    var modelStr = JSON.stringify(model);
    AssetTrackingDeviceConfiguration.SetModelString(modelStr);
};

//========================================================================
// This function will return the custom page menu model as a string. It
// is retrieved from the CustomPageMenuModel HTML field.
//========================================================================
AssetTrackingDeviceConfiguration.GetModelString = function ()
{
    return $('#AssetDeviceConfigModel').val();
};

//========================================================================
// This function will set the custom page menu model as a string. In the
// CustomPageMenuModel HTML field.
//========================================================================
AssetTrackingDeviceConfiguration.SetModelString = function (modelStr)
{
    return $('#AssetDeviceConfigModel').val(modelStr);
};
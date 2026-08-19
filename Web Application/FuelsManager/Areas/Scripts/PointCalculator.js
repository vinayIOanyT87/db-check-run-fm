var FMPointCalculator = {
  SelectedTag: "",
  Model: {},
  PrevHelpKey: "",
  Acronyms: {},
  IsBatchModeTags: {},
  RowId: [
     'LevelProduct',
     'VolumeTotalObserved',
     'LevelWater',
     'VolumeWater',
     'LevelSolids',
     'VolumeSolids',
     'VolumeBottoms',
     'TemperatureProduct',
     'TemperatureVapor',
     'TemperatureAmbient',
     'TankShellCorrection',
     'DensityProductStandard',
     'TemperatureDensity',
     'DensityProductObserved',
     'VolumeRoofCorrection',
     'VolumeGrossObserved',
     'VolumeCorrectionFactor',
     'VolumeGrossStandard',
     'PercentBSW',
     'VolumeBSW',
     'VolumeNetStandard',
     'VolumeTotalCalculated',
     'WeightGrossStandard',
     'VolumeGrossObservedAvailable',
     'VolumeGrossObservedRemaining',
     'WeightNetStandard',
     'VolumeNetStandardAvailable',
     'VolumeNetStandardRemaining',
     'PressureVapor',
     'DensityVapor',
     'MassLiquid',
     'MassVapor'
   ]
};

FMPointCalculator.Init = function () {
  // the model is being passed as a hidden input tag which we won't need anymore
  var strModel = $('#PointCalculatorModel').val();
  $('#PointCalculatorModel').remove();

  if (strModel === "") { strModel = "{}"; }

  FMPointCalculator.Model = JSON.parse(strModel);

  if (!FMPointCalculator.Model.isBatchMode) {
    $("#pointCalculatorMode").val("differential");
  }

  // Initialize event handlers
  $("#CalculatorTable .calculator-start-value").click(function () {
    var row = $(this).closest('tr');

    // if edit is diabled don't do anything
    if ($(this).find('a').attr('disabled') === "disabled") {
      return;
    }

    // Call the value editor
    FMPointCalculator.EditValue('startvalueguid',
      row.attr('id').replace('calculatortag_', ''),
      row.attr('data-datatype'),
      $(row).find('.calculator-tag-name').attr('data-name'),
      $(row).find('.calculator-start-value').attr("data-rawdata"),
      row.attr('data-numberdecimals'),
      row.attr('data-unitstype'),
      row.attr('data-units'),
      row.attr('data-maximum'),
      row.attr('data-minimum'),
      FMPointCalculator.Model.selectedBasePoint);
  });

  $("#CalculatorTable .calculator-end-value").click(function () {
    var row = $(this).closest('tr');

    // if edit is disabled don't do anything
    if ($(this).find('a').attr('disabled') === "disabled") {
      return;
    }

    // Call the value editor
    FMPointCalculator.EditValue('endvalueguid',
      row.attr('id').replace('calculatortag_', ''),
      row.attr('data-datatype'),
      $(row).find('.calculator-tag-name').attr('data-name'),
      $(row).find('.calculator-end-value').attr("data-rawdata"),
      row.attr('data-numberdecimals'),
      row.attr('data-unitstype'),
      row.attr('data-units'),
      row.attr('data-maximum'),
      row.attr('data-minimum'),
      FMPointCalculator.Model.selectedBasePoint);
  });

  $("#CalculatorTable .calculator-diff-value").click(function () {
		var row = $(this).closest('tr');

		// if edit is disabled don't do anything
		if ($(this).find('a').attr('disabled') === "disabled") {
			return;
		}

		var dataMax = row.attr('data-maximum');
		var dataMin = row.attr('data-minimum');
		var dataEndValue = $(row).find('.calculator-start-value').attr("data-rawdata");

		var dataMinCalc = dataMin;
		var dataMaxCalc = dataMax;

		var tagName = $(row).find('.calculator-tag-name').attr('data-name');
		var mode = $("#pointCalculatorMode").val();


		if (mode == "differential"
		|| (tagName != "Density Product Standard"
		&& tagName != "Density Product Observed"
		&& tagName != "Temperature Product")) {
		
			// offset maximum by the minimum if greater than 0
			if (dataMin > 0.0)
			dataMax -= dataMin;

			dataMinCalc = dataMin - dataEndValue;
			dataMaxCalc = dataMax - dataEndValue;
		}

		// Call the value editor
		FMPointCalculator.EditValue('difvalueguid',
			row.attr('id').replace('calculatortag_', ''),
			row.attr('data-datatype'),
			$(row).find('.calculator-tag-name').attr('data-name'),
			$(row).find('.calculator-diff-value').attr("data-rawdata"),
			row.attr('data-numberdecimals'),
			row.attr('data-unitstype'),
			row.attr('data-units'),
			dataMaxCalc,
			dataMinCalc,
			FMPointCalculator.Model.selectedBasePoint);
  });

  $('#PointCalculatorScreen').on('shown.bs.modal', function () {
    FMPointCalculator.PrevHelpKey = window.parent.CurrentHelpKey;
    window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexPointCalculator";
  })

  $('#PointCalculatorScreen').on('hidden.bs.modal', function () {
    window.parent.CurrentHelpKey = FMPointCalculator.PrevHelpKey;
  })

  $(document).click(function (event) {
    if ($(event.target).closest('#rowsInputArea, #RowFilterDiv').length === 0) {
      if (!$('#RowFilterDiv').hasClass('hidden')) {
        $('#RowFilterDiv').addClass('hidden');
        FMPointCalculator.ProcessRowsVisibility();
      }
    }
  });

  FMPointCalculator.EnableDisableTransfer();
  FMPointCalculator.EnableDisableRowsVisibilityConfigDropdown();
  FMPointCalculator.GetAcronyms();
  FMPointCalculator.GetIsBatchModeTags();
};

FMPointCalculator.EnableDisableTransfer = function () {
  if (FMPointCalculator.Model.isBatchMode
    && FMPointCalculator.Model.enableTransfer) {
    $("#InitiateTransferLevel").prop("disabled", false);
    $("#InitiateTransferBatch").prop("disabled", false);
    $("#InitiateTransferLevel").removeClass('pointCalculatorDisableButtonClass')
    $("#InitiateTransferBatch").removeClass('pointCalculatorDisableButtonClass');
  }
  else {
    $("#InitiateTransferLevel").prop("disabled", true);
    $("#InitiateTransferBatch").prop("disabled", true);
    $("#InitiateTransferLevel").removeClass('pointCalculatorDisableButtonClass').addClass('pointCalculatorDisableButtonClass');
    $("#InitiateTransferBatch").removeClass('pointCalculatorDisableButtonClass').addClass('pointCalculatorDisableButtonClass');
  }
}

FMPointCalculator.EnableDisableRowsVisibilityConfigDropdown = function () {
  if (FMPointCalculator.Model.EnableRowVisibilityConfigDropdown) {
    $("#rowsInputArea").removeClass('disabled');
    $("#rowsInputArea").prop('data-disabled', 'false');
  }
  else {
    $("#rowsInputArea").addClass('disabled');
    $("#rowsInputArea").prop('data-disabled', 'true');
  }
}


FMPointCalculator.OpenPointCalculatorDialog = function () {
  $('#PointCalculatorScreen').modal('show');

};

FMPointCalculator.FinishOpenPointCalculator = function (success) {
  if (success) {
    $('#PointCalculatorScreen').modal('show');
  }
};

FMPointCalculator.Reset = function () {
  var url = $("#urlPointCalculator").val();
  FMPointCalculator.GetForm(url, FMPointCalculator.Model.selectedBasePoint, FMPointCalculator.Model.selectedBasePointGuid, FMPointCalculator.Model.isBatchMode);
}

FMPointCalculator.CheckIfOkToTransfer = function (mode) {
  var target = '';
  var targetRaw = '';
  var targetUnits = '';
  var targetBatchAsLevelRaw = '';

  if (mode === 'Batch') {
    var volumeTag = null;
    if (FMPointCalculator.Model.transferByNet) {
      volumeTag = jQuery.grep(FMPointCalculator.Model.calculatorItemList, function (tag) { return tag.tagName === 'Volume Net Standard'; });
    } else {
      volumeTag = jQuery.grep(FMPointCalculator.Model.calculatorItemList, function (tag) { return tag.tagName === 'Volume Gross Observed'; });
    }
    if (volumeTag.length === 1) {
      target = volumeTag[0].diffValue;
      targetRaw = volumeTag[0].diffValueRaw;
      targetUnits = volumeTag[0].unitsString;
    }
    var levelTag = jQuery.grep(FMPointCalculator.Model.calculatorItemList, function (tag) { return tag.tagName === 'Level Product'; });
    if (levelTag.length === 1) {
      targetBatchAsLevelRaw = levelTag[0].endValueRaw;
    }
  } else {
    var levelTag = jQuery.grep(FMPointCalculator.Model.calculatorItemList, function (tag) { return tag.tagName === 'Level Product'; });
    if (levelTag.length === 1) {
      target = levelTag[0].endValue;
      targetRaw = levelTag[0].endValueRaw;
      targetUnits = levelTag[0].unitsString;
    }
  }

  var title_msg = "Initiate " + mode + " Transfer";
  var output_msg = "Point : " + FMPointCalculator.Model.selectedBasePoint + " Target : " + target + " (" + targetUnits + ")";

  FMLayout.Confirm(output_msg, title_msg, function () {

    var token = $('input[name=__RequestVerificationToken]').val();
    var url = $("#urlPointCalculatorCheckIfOkToTransfer").val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;
    $.ajax({
      type: 'POST',
      url: url,
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify({
        "pointGuidString": FMPointCalculator.Model.selectedBasePointGuid, "targetRaw": targetRaw, "targetBatchAsLevelRaw": targetBatchAsLevelRaw, "mode": mode }),
      cache: false,
      async: false,
      success: function (response) {
        FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
          switch (data) {
            case 1:
              FMLayout.Alert("There is an existing Completed transfer. Please set the Transfer Mode to 'Inactive' and re-initiate the transfer.", "Alert", null);
              break;
            case 2:
              var output_message = "This transfer will lower the level to or below the minimum operating level. Are you sure you wish to proceed?";

              FMLayout.ConfirmYesNo(output_message, title_msg, function () {
                FMPointCalculator.InitiateTransfer(targetRaw, mode);
              });
              break;
            case 3:
              var output_message = "This transfer will raise the level to or above the maximum operating level. Are you sure you wish to proceed?";

              FMLayout.ConfirmYesNo(output_message, title_msg, function () {
                FMPointCalculator.InitiateTransfer(targetRaw, mode);
              });
              break;
            case 4:
              FMPointCalculator.InitiateTransfer(targetRaw, mode);
              break;
            case 5:
              FMLayout.Alert("The specified value is at or below minimum operating level. User does not have Exceed Range permission.", "Alert", null);
              break;
            case 6:
              FMLayout.Alert("The specified value is at or above maximum operating level. User does not have Exceed Range permission.", "Alert", null);
              break;
            default:
              FMLayout.Alert("Unable to initiate transfer.", "Alert", null);
              break;
          }
        });
      },
      error: function (xhr, ajaxOptions, thrownError) {
        FMErrorAndExceptionHandling.ShowError(thrownError);
        $("#PointGroupSelectionModal").modal("hide");
      }
    });
  }
    , null);
}

FMPointCalculator.InitiateTransfer = function (targetRaw, mode) {
  var token = $('input[name=__RequestVerificationToken]').val();
  var url = $("#urlPointCalculatorInitiateTransfer").val();
  var headers = {};
  headers['__RequestVerificationToken'] = token;


  $.ajax({
    type: 'POST',
    url: url,
    dataType: "json",
    contentType: "application/json; charset=utf-8",
    data: JSON.stringify({ "pointGuidString": FMPointCalculator.Model.selectedBasePointGuid, "target": targetRaw, "mode": mode }),
    cache: false,
    async: false,
    success: function (response) {
      FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) { });
    },
    error: function (xhr, ajaxOptions, thrownError) {
      FMErrorAndExceptionHandling.ShowError(thrownError);
      $("#PointGroupSelectionModal").modal("hide");
    }
  });

}

FMPointCalculator.SetMode = function () {

  FMPointCalculator.Model.batchModeKey = 0;
  FMPointCalculator.Model.isBatchMode = ($("#pointCalculatorMode").val() == "batch")
  FMPointCalculator.Reset();
}

FMPointCalculator.GenerateReport = function () {

  var url = $("#urlPointCalculatorReport").val();

  FMErrorAndExceptionHandling.CloseNotifications();
  $('body').modalmanager('loading');

  var token = $('input[name=__RequestVerificationToken]').val();
  var headers = {};
  headers['__RequestVerificationToken'] = token;

  $.ajax({
    type: 'Post',
    url: url,
    dataType: 'json',
    data: JSON.stringify(FMPointCalculator.Model),
    headers: headers,
    cache: false,
    success: function (response) {
      var modalManager = $('body').data('modalmanager');
      modalManager.removeLoading();
      // Open report in a separate Window
      FMPointCalculator.OpenReport(response.RunId);
    },
    error: function (xhr, textStatus, error) {
      var modalManager = $('body').data('modalmanager');
      modalManager.removeLoading();
      FMErrorAndExceptionHandling.ShowException(xhr,
        textStatus,
        error);
    }
  });
}

FMPointCalculator.OpenReport = function (pointCalculatorRunId) {

  if (pointCalculatorRunId === undefined || pointCalculatorRunId === "00000000-0000-0000-0000-000000000000") {
    return;
  }

  url = $('#urlReportViewer').val();

  // remove previous notifications
  PNotify.removeStack();

  url += "?ReportType=10";
  url += "&ReportName=FM_PointCalculatorReport";
  url += "&RunId=" + pointCalculatorRunId;
  url += "&CSRFToken=" + window.csrfToken;
  window.open(url);
};

FMPointCalculator.GetForm = function (url, pointIdString, pointGuidString, isBatchMode) {
   var callData = {
    pointIdString: pointIdString,
    pointGuidString: pointGuidString,
    isBatchMode: isBatchMode
  };

  FMErrorAndExceptionHandling.CloseNotifications();
  $('body').modalmanager('loading');

  var token = $('input[name=__RequestVerificationToken]').val();
  var headers = {};
  headers['__RequestVerificationToken'] = token;

  $.ajax({
    type: 'Post',
    url: url,
    dataType: 'json',
    data: JSON.stringify(callData),
    headers: headers,
    cache: false,
    success: function (response) {
      var modalManager = $('body').data('modalmanager');
      modalManager.removeLoading();
      FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
        if (!inError) {
          // replace the holder with the partial view
          $("#PointCalculator").html(data);

          FMPointCalculator.FinishOpenPointCalculator(true);
        }
        else {
          FMPointCalculator.FinishOpenPointCalculator(false);
        }
      });
    },
    error: function (xhr, textStatus, error) {

      var modalManager = $('body').data('modalmanager');
      modalManager.removeLoading();
      FMErrorAndExceptionHandling.ShowException(xhr,
        textStatus,
        error,
        function () {
          // remove the loading of the modal
          FMPointCalculator.FinishOpenPointCalculator(false);
        });
    }
  });
};

FMPointCalculator.EditValue = function (columnChanged, tagGuid, dataType, tagName, currentValue, numberOfDecimals, unitsType, units, maxValue, minValue, pointName) {

  // remove changed visual cues
  FMPointCalculator.ResetCalculatorScreenColors();

  var TagType = "Unknown";

  if (columnChanged === "startvalueguid") {
    TagType = "Start ";
  }
  else if (columnChanged === "endvalueguid") {
    TagType = "End ";
  }
  else if (columnChanged === "difvalueguid") {
    TagType = "Differential ";
    if (FMPointCalculator.Model.isBatchMode) {
      TagType = "Batch ";
    }
  }

  var id = TagType + tagName;
  var PointValueType = "Tag";
  var PropertyID = "";

  pointName += " Calculator";

  var url = $('#urlEditPointTemplateTags').val();
  var token = $('#pointPropertiesForm input[name=__RequestVerificationToken]').val();
  var headers = {};
  headers['__RequestVerificationToken'] = token;

  var requestParms = {
    identityGuid: tagGuid,
    pointValueType: PointValueType,
    propertyID: PropertyID,
    valueTypeString: dataType,
    id: id,
    value: currentValue,
    decimalPlaces: numberOfDecimals,
    unitType: unitsType,
    unit: units,
    maximum: maxValue,
    minimum: minValue,
    pointName: pointName
  }

  // redefine the function that will be executed when applying changes to tag values
  window.HandlePointTagEditorNewValueUpdate = function () {
    // get the new value
    var newVal = $("#PointTagNewValueInputId").val();
    var newRawVal = $("#PointTagNewRawValueInputId").val();

    // update the Model
    var changedTag = jQuery.grep(FMPointCalculator.Model.calculatorItemList, function (tag) { return tag.tagGuid === tagGuid; });
    if (changedTag.length > 0) {
      if (columnChanged === "startvalueguid") {
        changedTag[0].startValueRaw = newRawVal;
        changedTag[0].startValue = newVal;
         FMPointCalculator.Model.changedTagColumn = "start";
      }
      else if (columnChanged === "endvalueguid") {
        changedTag[0].endValueRaw = newRawVal;
        changedTag[0].endtValue = newVal;
        FMPointCalculator.Model.changedTagColumn = "end";
      }
      else {
        changedTag[0].diffValueRaw = newRawVal;
        changedTag[0].diffValue = newVal;
         FMPointCalculator.Model.changedTagColumn = "diff";
      }
    }

    FMPointCalculator.Model.changedTagGuid = tagGuid;

    FMPointCalculator.Model.calculatorItemList.forEach(function (element) {
      element.startSourceDateTime = '0001-01-01T00:00:00+00:00';
      element.endSourceDateTime = '0001-01-01T00:00:00+00:00';
    });

    FMErrorAndExceptionHandling.CloseNotifications();
    $('body').modalmanager('loading');

    var token = $('input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;
    // call the get the calculated values
    $.ajax({
      type: 'Post',
      url: $("#urlPointCalculatorUpdateValues").val(),
      dataType: 'json',
      data: JSON.stringify(FMPointCalculator.Model),
      headers: headers,
      cache: false,
      success: function (response) {
        var modalManager = $('body').data('modalmanager');
        modalManager.removeLoading();
        FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
          if (!inError) {
            if (columnChanged === "startvalueguid") {
              FMPointCalculator.Model.enableTransfer = false;
              FMPointCalculator.EnableDisableTransfer();
            }

            // need to redraw the screen
            $.each(data.calculatorItemList, function (index, tag) {

              // add the visual cues to denote value changes
              var oldStartRawData = parseFloat($("#calculatortag_" + tag.tagGuid + " .calculator-start-value").attr('data-rawdata'));
              var newStartRawData = parseFloat(tag.startValueRaw);
              $("#calculatortag_" + tag.tagGuid + " .calculator-start-value a").addClass('hidden');
              if (newStartRawData > oldStartRawData) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-start-value .value-up").removeClass('hidden');
                $("#calculatortag_" + tag.tagGuid + " .calculator-start-value span.value").addClass('value-up');
              } else if (newStartRawData < oldStartRawData) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-start-value .value-down").removeClass('hidden');
                $("#calculatortag_" + tag.tagGuid + " .calculator-start-value span.value").addClass('value-down');
              }
              else {
                $("#calculatortag_" + tag.tagGuid + " .calculator-start-value .value-no-change").removeClass('hidden');
              }

              var oldEndRawData = parseFloat($("#calculatortag_" + tag.tagGuid + " .calculator-end-value").attr('data-rawdata'));
              var newEndRawData = parseFloat(tag.endValueRaw);
              $("#calculatortag_" + tag.tagGuid + " .calculator-end-value a").addClass('hidden');
              if (newEndRawData > oldEndRawData) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value .value-up").removeClass('hidden');
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value span.value").addClass('value-up');
              } else if (newEndRawData < oldEndRawData) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value .value-down").removeClass('hidden');
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value span.value").addClass('value-down');
              }
              else {
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value .value-no-change").removeClass('hidden');
              }

              if ($("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").text() != "") {
                var oldDiffRawData = parseFloat($("#calculatortag_" + tag.tagGuid + " .calculator-diff-value").attr('data-rawdata'));
                var newDiffRawData = parseFloat(tag.diffValueRaw);
                $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value a").addClass('hidden');
                if (newDiffRawData > oldDiffRawData) {
                  $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value .value-up").removeClass('hidden');
                  $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").addClass('value-up');
                } else if (newDiffRawData < oldDiffRawData) {
                  $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value .value-down").removeClass('hidden');
                  $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").addClass('value-down');
                }
                else {
                  $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value .value-no-change").removeClass('hidden');
                }
              }


              if (tag.tagGuid === FMPointCalculator.Model.changedTagGuid) {
                if (columnChanged === "startvalueguid") $("#calculatortag_" + tag.tagGuid + " .calculator-start-value span.value").addClass("font-bold");
                if (columnChanged === "endvalueguid") $("#calculatortag_" + tag.tagGuid + " .calculator-end-value span.value").addClass("font-bold");
                if (columnChanged === "diffvalueguid") $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").addClass("font-bold");
              }

              // change the values in the screen
              $("#calculatortag_" + tag.tagGuid + " .calculator-start-value span.value").text(tag.startValue);
              $("#calculatortag_" + tag.tagGuid + " .calculator-start-value span.value").attr("title", tag.startValue);
              $("#calculatortag_" + tag.tagGuid + " .calculator-start-value").attr('data-rawdata', tag.startValueRaw);
              $("#calculatortag_" + tag.tagGuid + " .calculator-end-value span.value").text(tag.endValue);
              $("#calculatortag_" + tag.tagGuid + " .calculator-end-value span.value").attr("title", tag.endValue);
              $("#calculatortag_" + tag.tagGuid + " .calculator-end-value").attr('data-rawdata', tag.endValueRaw);

              if ($("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").text() != "") {
                $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").text(tag.diffValue);
                $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").attr("title", tag.diffValue);
                $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value").attr('data-rawdata', tag.diffValueRaw);
              }

              // hide pencils and disable edit based on keyvalue
              if (data.BatchModeChangedColumn === 'batch') {
                $("#calculatortag_" + tag.tagGuid + " .calculator-start-value a").removeClass('editLinkClass');
                $("#calculatortag_" + tag.tagGuid + " .calculator-start-value a").addClass('editLinkClassDisabled');
                $("#calculatortag_" + tag.tagGuid + " .calculator-start-value a").attr('disabled', 'disabled');

                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value a").removeClass('editLinkClass');
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value a").addClass('editLinkClassDisabled');
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value a").attr('disabled', 'disabled');
              }
              else if (data.BatchModeChangedColumn === 'end') {
                $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value a").removeClass('editLinkClass');
                $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value a").addClass('editLinkClassDisabled');
                $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value a").attr('disabled', 'disabled');
              }

              // set the keyvalue
              $("#calculatortag_" + tag.tagGuid + " .calculator-end-value span.value").removeClass("keyvalue");
              $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").removeClass("keyvalue");

              if (tag.tagName === "Volume Gross Observed" && data.batchModeKey === 1) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").addClass("keyvalue");
              }
              else if (tag.tagName === "Volume Net Standard" && data.batchModeKey === 2) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").addClass("keyvalue");
              }
              else if (tag.tagName === "Volume Total Observed" && data.batchModeKey === 3) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-diff-value span.value").addClass("keyvalue");
              }
              else if (tag.tagName === "Level Product" && data.batchModeKey === 4) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value span.value").addClass("keyvalue");
              }
              else if (tag.tagName === "Volume Gross Observed" && data.batchModeKey === 5) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value span.value").addClass("keyvalue");
              }
              else if (tag.tagName === "Volume Net Standard" && data.batchModeKey === 6) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value span.value").addClass("keyvalue");
              }
              else if (tag.tagName === "Volume Total Observed" && data.batchModeKey === 7) {
                $("#calculatortag_" + tag.tagGuid + " .calculator-end-value span.value").addClass("keyvalue");
              }
            });
            FMPointCalculator.Model = data;

            window.setTimeout(function () {
              FMPointCalculator.ResetCalculatorScreenColors();
            }, 5000);
          }
        });
      },
      error: function (xhr, textStatus, error) {
        FMErrorAndExceptionHandling.ShowException(xhr,
          textStatus,
          error,
          function () {
            // remove the loading of the modal
            var modalManager = $('body').data('modalmanager');
            modalManager.removeLoading();
          });
      }
    });

  }


  FMErrorAndExceptionHandling.CloseNotifications();

  // create the backdrop and wait for next modal to be triggered
  var modalManager = $('body').modalmanager('loading');

  $.ajax({
    type: 'POST',
    cache: false,
    dataType: 'json',
    data: JSON.stringify(requestParms),
    headers: headers,
    url: url,
    success: function (response) {
      FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
        if (!inError || (data != null)) {
          $('#PointTagEditValueScreenBody').html(response.Data);
          $('#PointTagEditValueScreen').modal('show');
        }
        else {
          $('body').modalmanager('loading'); // remove the loading background
        }
      });
    },
    error: function (xhr, textStatus, error) {
      FMErrorAndExceptionHandling.ShowException(xhr, textStatus, error, function () {
        $('body').modalmanager('loading'); // remove the loading background
      });
    }
  });

}

FMPointCalculator.ResetCalculatorScreenColors = function () {
  $("#CalculatorTable .value").removeClass('font-bold').removeClass('value-up').removeClass('value-down');
  $("#CalculatorTable a").removeClass('hidden');
  $("#CalculatorTable .value-up").removeClass('hidden').addClass('hidden');
  $("#CalculatorTable .value-down").removeClass('hidden').addClass('hidden');
  $("#CalculatorTable .value-no-change").removeClass('hidden').addClass('hidden');
};

FMPointCalculator.GetAcronyms = function () {
  var url = $("#urlPointCalculatorAcronyms").val();

  FMErrorAndExceptionHandling.CloseNotifications();

  var token = $('input[name=__RequestVerificationToken]').val();
  var headers = {};
  headers['__RequestVerificationToken'] = token;

  $.ajax({
    type: 'GET',
    url: url,
    dataType: 'json',
    headers: headers,
    cache: false,
    success: function (response) {
      // save acronyms for later use
      FMPointCalculator.Acronyms = response.Data;
    },
    error: function (xhr, textStatus, error) {
      var modalManager = $('body').data('modalmanager');
      modalManager.removeLoading();
      FMErrorAndExceptionHandling.ShowException(xhr,
        textStatus,
        error);
    }
  });
}

FMPointCalculator.GetIsBatchModeTags = function () {
  var url = $("#urlPointCalculatorIsBatchModeTags").val();

  FMErrorAndExceptionHandling.CloseNotifications();

  var token = $('input[name=__RequestVerificationToken]').val();
  var headers = {};
  headers['__RequestVerificationToken'] = token;

  $.ajax({
    type: 'GET',
    url: url,
    dataType: 'json',
    headers: headers,
    cache: false,
    success: function (response) {
      //save isbatchmodetags for later use
      FMPointCalculator.IsBatchModeTags = response.Data;
    },
    error: function (xhr, textStatus, error) {
      var modalManager = $('body').data('modalmanager');
      modalManager.removeLoading();
      FMErrorAndExceptionHandling.ShowException(xhr,
        textStatus,
        error);
    }
  });
}

FMPointCalculator.GenerateAndDownloadCSV = function () {
  var currentDate = new Date();
  var csvContents = '\uFEFF'
  csvContents += '"","","","","';
  csvContents += moment(currentDate).format(FMPointCalculator.Model.datePattern.toUpperCase());
  csvContents += '","';
  csvContents += moment(currentDate).format(FMPointCalculator.Model.timePattern.replace(/tt/i, 'A'));
  csvContents += '"\n';
  csvContents += '"';
  csvContents += FMPointCalculator.Model.siteId;
  csvContents += '","';
  csvContents += FMPointCalculator.Model.selectedBasePoint;
  csvContents += '","","","Calculator Mode:","';
  if (FMPointCalculator.Model.isBatchMode) {
    csvContents += 'Batch Mode"\n';
  }
  else {
    csvContents += 'Differential Mode"\n';
  }
  csvContents += '"","","","","",""\n';
  csvContents += '"Tag","Units","","Start Value","End Value",';
  if (FMPointCalculator.Model.isBatchMode) {
    csvContents += '"Batch Value"\n';
  }
  else {
    csvContents += '"Differential Value"\n';
  }

    for (let i = 0; i < FMPointCalculator.Model.calculatorItemList.length; i++) {
    if (!FMPointCalculator.Model.calculatorItemList[i].isVisible)
        continue;
    csvContents += '"' + FMPointCalculator.Model.calculatorItemList[i].tagName + '","' + FMPointCalculator.Model.calculatorItemList[i].unitsString + '","';
    csvContents += FMPointCalculator.Acronyms[i];
    csvContents += '","';
    csvContents += FMPointCalculator.Model.calculatorItemList[i].startValue + '","' + FMPointCalculator.Model.calculatorItemList[i].endValue + '","';
    if (!FMPointCalculator.Model.isBatchMode || FMPointCalculator.IsBatchModeTags[i]) {
      csvContents += FMPointCalculator.Model.calculatorItemList[i].diffValue;
    }
    csvContents += '"\n';
  }

  var hiddenElement = document.createElement('a');
  hiddenElement.href = 'data:text/csv,' + encodeURI(csvContents);
  hiddenElement.target = '_blank';
  hiddenElement.download = 'PointCalculator ' + FMPointCalculator.Model.siteId + ' ' + FMPointCalculator.Model.selectedBasePoint + ' - ' + moment(currentDate).format('YYYYMMDDHHmmss') + '.csv';
  hiddenElement.click();
}

//================================================================================
// This function will process rows visibility when the drop down is closed
//================================================================================
FMPointCalculator.ProcessRowsVisibility = function () {
   let i = 0;
   let configValue = 0n;
   FMPointCalculator.RowId.forEach(row => {
      let currentItem = "#" + row + "Checkbox";
      let tagIsVisible = $(currentItem).is(":checked");
      let tagName = $(currentItem).attr("value");
      const listItem = FMPointCalculator.Model.calculatorItemList.find(item => item.tagName === tagName);
      if (listItem) {
         listItem.isVisible = tagIsVisible;
          if (tagIsVisible) {
              configValue |= (1n << BigInt(i));
          }
       }
      i++;
   });

   FMPointCalculator.Model.PointCalculatorRowVisibilityConfig = configValue;

   FMPointCalculator.SaveRowVisibilityConfigValue(Number(configValue));

   FMPointCalculator.Reset();
}

FMPointCalculator.SaveRowVisibilityConfigValue = function (newValue) {
   var url = $('#urlUpdateRowVisibilityConfigValue').val();
   var token = $('input[name=__RequestVerificationToken]').val();
   var headers = {};
   headers['__RequestVerificationToken'] = token;

   $.ajax({
      cache: false,
      type: 'POST',
      async: false,
      dataType: 'json',
      url: url,
      headers: headers,
      data: JSON.stringify({ configValue: newValue }),
      success: function (result) {
         if (result === 'ERROR') {
            FMErrorAndExceptionHandling.ShowError('Error saving Row Visibility Config Value.', null, null);
         }
      },
      error: function (e) {
         FMErrorAndExceptionHandling.ShowError('Error saving Row Visibility Config Value.', null, null);
      }
   });
}; //==========================================================================================


//================================================================================
// This function will toggle all rows visibility.
//================================================================================
FMPointCalculator.ToggleAllRowsVisibility = function (checked) {
   FMPointCalculator.RowId.forEach(row => {
      $("#" + row + "Checkbox").prop("checked", checked);
   });
}

//================================================================================
// This function will handle the select all or select none checkbox being 
// checked / unchecked event.
//================================================================================
FMPointCalculator.HandleRowFilterCheckboxChange = function (currentItem) {
   let selectAllChecked = true;
   FMPointCalculator.RowId.forEach(row => {
      let currentItem = "#" + row + "Checkbox";
      let tagIsVisible = $(currentItem).is(":checked");
      if (!tagIsVisible) {
         selectAllChecked = false;
      }
   });

   $("#SelectAllCheckbox").prop("checked", selectAllChecked);
}

//================================================================================
// This function will handle the select all or select none checkbox being 
// checked / unchecked event.
//================================================================================
FMPointCalculator.HandleAllRowsFilterCheckboxChange = function (currentItem) {
   let checked = $(currentItem).is(":checked");
   let inputId = $(currentItem).attr("id");

   if (typeof (checked) != "undefined" && typeof (inputId) != "undefined") {
      if (inputId === "SelectAllCheckbox") {
         FMPointCalculator.ToggleAllRowsVisibility(checked);
      }
   }
}
//================================================================================
// This function will handle the column filter dropdown on click event. It will
// expand or collapse the dropdown based on the "hidden" class state.
//================================================================================
FMPointCalculator.HandleRowFilterDropdownExpandCollapse = function (event) {
   if (event) {
      event.stopPropagation();
   }

   // Check if the dropdown is disabled
   if ($("#rowsInputArea").hasClass('disabled')) {
      return;
   }

   let hiddenClass = $("#RowFilterDiv").attr('class');
   if (hiddenClass === "") {
      $("#RowFilterDiv").addClass('hidden');
      FMPointCalculator.ProcessRowsVisibility();
   }
   else {
      $("#RowFilterDiv").removeClass('hidden');

      let areAllRowsSelected = true;
      // update checkboxes before displaying dropdown
      FMPointCalculator.RowId.forEach(row => {
         let currentItem = "#" + row + "Checkbox";
         let tagName = $(currentItem).attr("value");

         const listItem = FMPointCalculator.Model.calculatorItemList.find(item => item.tagName === tagName);

         if (listItem) {
            $(currentItem).prop("checked", listItem.isVisible);

            if (!listItem.isVisible) {
               areAllRowsSelected = false;
            }
         }
      });

      $("#SelectAllCheckbox").prop("checked", areAllRowsSelected);
   }
}

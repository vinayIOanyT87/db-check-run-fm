//--------------------------------------- RUN after page has been loaded but before render -----------------------------


// create a class with helper functions for the OpcUaBrowser view
var FMPointValueConfigurationEditor = function () {
	//set the position for the messages from the server
	var _stack_bottomright_pointvalueconfigurationeditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 15, "firstpos2": 25, "context": $('#pointValueConfigurationEditorPartial') };

	var _initialiseView = function (action, method) {
		$('#PointCommandStatusElementListBox option').attr("disabled", "disabled");
	}

	var _saveChanges = function (action, method) {
		var url = $('#urlSaveValueConfigurationChanges').val();
		var token = $('#pointValueConfigurationEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;
		var valueReferenceGuid = $('#ValueReferenceDropDown').val();
		if (valueReferenceGuid == '') {
			valueReferenceGuid = '00000000-0000-0000-0000-000000000000';
		}
		var pointObjectGuid = $("#PointObjectGuidString").val();
		var isTemplatePoint = $('#IsTemplatePoint').val();
		var isSetting = $('#IsSetting').val();
		var valueTypeString = $('#ValueTypeString').val();

		if (valueTypeString == 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference') {
			var tagItem = $("#TagName_" + pointObjectGuid);

			// delete any prior alarm configuuration
			AlarmEditor.DeleteAlarmsForTagNoConfirmation(tagItem);
			var valueReferenceString = $('#ValueReferenceObject').val();
			if (valueReferenceString && valueReferenceString !== '') {
				var valueReference = JSON.parse(valueReferenceString);
				var normalPriorities = JSON.parse($('#NormalPriorities').val());
				var alarmCategories = JSON.parse($('#AlarmCategories').val());

				// addd new alarm configuration
				if (valueReference && normalPriorities && alarmCategories) {
					var normalUnacknowledgedPriority = jQuery.grep(normalPriorities, function (a) {
						return a.Value === valueReference.NormalUnacknowledgedPriority;
					});

					if (normalUnacknowledgedPriority.length == 0) {
						// todo add error message
						return;
					}

					var normalUnacknowledgeAlarmPriorityID = normalUnacknowledgedPriority[0].Text;

					var alarmCategory = jQuery.grep(alarmCategories, function (a) {
						return a.Value === valueReference.AlarmCategory;
					});

					if (alarmCategory.length == 0) {
						// todo add error message
						return;
					}

					var alarmCategoryID = alarmCategory[0].Text;
					var order = valueReference.DeviceAlarmMapEntryList.length - 1;

					valueReference.DeviceAlarmMapEntryList.forEach(function (deviceAlarmEntry) {
						AlarmEditor.AddDeviceAlarmEntry(pointObjectGuid, valueReference, deviceAlarmEntry, normalUnacknowledgeAlarmPriorityID, alarmCategoryID, order--);
					});
				}
			}

			FMPointEditor.sortTagTable();

			AlarmEditor.SortAlarmTable(pointObjectGuid);

			var row = $("#TagEditTable .tagColumnPointTagGuid:contains('" + pointObjectGuid + "')").closest('tr');
			var gridIndex = row.index();

			if(isTemplatePoint.toLowerCase() === "true") {
				$("#Tags_" + gridIndex + "__Value").attr("data-raw-value", '{"DeviceAlarmMapGuid":"' + valueReferenceGuid + '","CurrentValue":null}');
				$("#Tags_" + gridIndex + "__Value").val("");
			}

			var tagGuid = FMPointEditor.tagList[gridIndex].PointTagGuid;

			$('#PointValueConfigurationEditorModal').modal('hide');
			$('#PointValueConfigurationEditorScreen').html('');

			// call the save form and tell it to take no action
			FMPointEditor.saveChanges($('#pointPropertiesForm').attr("action"), $('#pointPropertiesForm').attr("method"), function () { }, '{"DeviceAlarmMapGuid":"' + valueReferenceGuid + '","CurrentValue":null}', tagGuid);

			return;
		}	

		var pointTagData = { isTemplatePoint: isTemplatePoint, isSetting: isSetting, pointObjectGuid: pointObjectGuid, valueReferenceGuid: valueReferenceGuid };


		// specify location for server notifications
		var notificationAttributes = { addclass: 'stack-bottomright', stack: FMPointValueConfigurationEditor.stack_bottomright_pointvalueconfigurationeditor };
		// remove any notification
		PNotify.removeStack(FMPointValueConfigurationEditor.stack_bottomright_pointvalueconfigurationeditor);

		$.ajax({
			url: url,
			cache: false,
			type: 'POST',
			headers: headers,
			data: pointTagData,
			evalScripts: true,
			success: function (response) {
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
					if (!inError) {
						$('#pointValueConfigurationEditorError').html(data);

						if ((isTemplatePoint.toLowerCase() === "true") && (isSetting.toLowerCase() === "false")) // if we are working with template tags
						{
							// find and update the point template tag value with the new list
							var gridIndex = $("#TagEditTable .tagColumnPointTagGuid:contains('" + pointObjectGuid + "')").closest('tr').index();
							if (valueTypeString == 'FMBusinessObjects.DataObjects.PointCommandStatusListReference') {
								var pointCommandStatusListReference = null;
								var currentPointCommandStatusListReference = $("#Tags_" + gridIndex + "__Value").attr("data-raw-value");
								if (typeof currentPointCommandStatusListReference === 'string' && currentPointCommandStatusListReference.indexOf('PointCommandStatusListGuid') !== -1){
									pointCommandStatusListReference = JSON.parse($("#Tags_" + gridIndex + "__Value").attr("data-raw-value"));
								}
								if (pointCommandStatusListReference === null || pointCommandStatusListReference.PointCommandStatusListGuid !== valueReferenceGuid) {
									$("#Tags_" + gridIndex + "__Value").attr("data-raw-value", '{"PointCommandStatusListGuid":"' + valueReferenceGuid + '","CurrentValue":null,"CurrentKey":null}');
									$("#Tags_" + gridIndex + "__Value").val("");
								}
							}
						}

						if (isSetting) {
							var list = '<option value=""></option>';
							$("#ValueReferenceEntryListBox").find('option').each(function (index, element) {
								list += '<option value="' + element.value + '">' + element.text.substring(element.text.indexOf(' - ') + 3) + '</option>';
							});
							$('#PMEEditPropertyDropDownList' + pointObjectGuid).html(list);
						}
					}
				}, notificationAttributes);
			},
			error: function (request, status, error) {
				FMErrorAndExceptionHandling.ShowError('SavePointValueConfigurationChanges failure');
			}
		});
	};


	var _fillValueReferenceEntryList = function (PointTemplateGuid) {
		var url = $('#urlGetValueReferenceEntryList').val();
		var token = $('#pointValueConfigurationEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;
		var valueReferenceGuid = $('#ValueReferenceDropDown').val();
		if (valueReferenceGuid === '') {
			$("#ValueReferenceObject").val('');
			$("#ValueReferenceEntryListBox").html("");
			return;
		}

		var valueTypeString = $('#ValueTypeString').val();
		var pointTagData = { pointTemplateGuid: PointTemplateGuid, valueReferenceGuid: valueReferenceGuid, valueTypeString: valueTypeString };
		$.ajax({
			type: "GET",
			cache: false,
			headers: headers,
			url: url,
			data: pointTagData,
			dataType: "JSON",
			success: function (valueReference) {
				$("#ValueReferenceEntryListBox").html(""); // clear before appending new list
				if (valueTypeString == 'FMBusinessObjects.DataObjects.PointCommandStatusListReference') {
					$.each(valueReference, function (i, valueReferenceEntry) {
						$("#ValueReferenceEntryListBox").append(
						$('<option disabled></option>').val(valueReferenceEntry.Value).html(valueReferenceEntry.Value + " - " + valueReferenceEntry.Key));
					});
				}
				else if (valueTypeString == 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference') {
					$.each(valueReference.DeviceAlarmMapEntryList, function (i, deviceAlarmMapEntry) {
						var hex = deviceAlarmMapEntry.BitMask.toString(16);
						while (hex.length < 8) {
							hex = "0" + hex;
						}

						$("#ValueReferenceEntryListBox").append(
						$('<option disabled></option>').val(deviceAlarmMapEntry.BitMask).html('0x' + hex + " - " + deviceAlarmMapEntry.TestName));
					});
					$("#ValueReferenceObject").val(JSON.stringify(valueReference));
				}
			},
			error: function (request, status, error) {
				FMErrorAndExceptionHandling.ShowError('GetValueEntryList failure');
			}
		});

	}


	return {
		initialiseView: _initialiseView,
		saveChanges: _saveChanges,
		fillValueReferenceEntryList: _fillValueReferenceEntryList,
		stack_bottomright_pointvalueconfigurationeditor: _stack_bottomright_pointvalueconfigurationeditor
	};
}();

$(document).ready(function () {
	// manually hookup to the submit the form to make sure we pass all the entries from the table
	$('#pointValueConfigurationEditorForm').submit(function () {
		var action = this.action;
		var method = this.method;
		FMPointValueConfigurationEditor.saveChanges(action, method);

		return false;
	});
	FMPointValueConfigurationEditor.initialiseView();
	FMErrorAndExceptionHandling.CloseNotifications();
});
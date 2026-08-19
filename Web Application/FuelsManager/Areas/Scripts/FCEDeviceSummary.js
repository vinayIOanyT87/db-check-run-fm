var FCEDeviceSummary = FCEDeviceSummary || {};
var AddKey;

FCEDeviceSummary.CreateFCEDevice = function () {
	console.log("FCEDeviceSummary.CreateFCEDevice ");
}

FCEDeviceSummary.EditFCEDevice = function (FCEDeviceGuidString) {
	$('body').modalmanager('loading');
	// close all the notifications currently openned
	FMErrorAndExceptionHandling.CloseNotifications();

	var token = $('#FCEDeviceSummaryForm input[name =__RequestVerificationToken]').val();
	var headers = {};

	headers['__RequestVerificationToken'] = token;

	var FCEDeviceEditorData = { FCEDeviceGuidString: FCEDeviceGuidString, FCEDeviceSummary: 'true' };

	var urlFCEDeviceEditor = $('#urlFCEDeviceEditor').val();

	AddKey = $('#addFCEDevice1').attr('accesskey');
	$('#addFCEDevice1').removeAttr('accesskey');
	$('#addFCEDevice2').removeAttr('accesskey');


	$.ajax({
		type: "GET",
		cache: false,
		headers: headers,
		url: urlFCEDeviceEditor,
		data: FCEDeviceEditorData,
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					// replace the holder with the partial view
					$('#FCEDeviceEditorScreen').html(data);
					// show the modal
					$('#EditFCEDeviceModal').modal('show');
				}
				else {
					// remove the loading of the modal
					var modalManager = $("body").data("modalmanager");
					modalManager.removeLoading();
				}
			});

		},
		error: function (xhr, textStatus, error) {
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () {
					// remove the loading of the modal
					var modalManager = $("body").data("modalmanager");
					modalManager.removeLoading();
				});
		}
	});
}

FCEDeviceSummary.DeleteFCEDevice = function (FCEDeviceGuid) {
	$.ajax({
		type: 'POST',
		url: 'Delete',
		cache: false,
		async: false,
		data: {
			guidString: FCEDeviceGuid,
			'__RequestVerificationToken': $('input[name=__RequestVerificationToken]').val()
		},
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					var tbl = document.getElementById("FCEDeviceTable");
					var tr = document.getElementById("row_" + FCEDeviceGuid);
					if (tr) {
						tbl.deleteRow(tr.rowIndex);
					}
					FCEDeviceSummary.rowCount--;
					document.getElementById("rowCountSpan").innerHTML = FCEDeviceSummary.rowCount;
				}
			});
		},
		error: function (xhr, textStatus, error) {
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () { });
		}
	});
	//Prevent Post
	return false;
}

FCEDeviceSummary.RestoreAccessKeys = function () {
	$('#addFCEDevice2').attr('accesskey', AddKey);
	$('#addFCEDevice1').attr('accesskey', AddKey);
}


$(document).ready(function () {
	FCEDeviceSummary.rowCount = $('#FCEDeviceTable >tbody >tr').length;
	document.getElementById("rowCountSpan").innerHTML = FCEDeviceSummary.rowCount;
});

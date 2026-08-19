var FCEDeviceEditor = FCEDeviceEditor || {};

$(document).ready(function () {
	if ($('#ReadOnly').val().toLowerCase() === 'true') {
		//editor.setReadOnly(true);
		$('#FCEDeviceEditorSaveButton').addClass('disabled').prop('disabled', true);
	}
	else {
		$('#FCEDeviceEditorSaveButton').removeClass('disabled').prop('disabled', false);
	}

	// This sets the nice scrolling on the Point Setting section of the page.
	$('#FCEDeviceEditorDivScroll').niceScroll({
		cursorwidth: '10px'
	, autohidemode: false
	, cursorcolor: "#486899"
	, background: "#f9f9f9"
	, railoffset: true
	, railpadding: {top: 0, right: 0, left: -5, bottom: 0 }
	, smoothscroll: true
		});



	$('#FCEDeviceEditorSaveButton').off('click');
	let form = $('#FCEDeviceEditorForm');
	form.attr("action","javascript:FCEDeviceEditor.Save();");

     });


FCEDeviceEditor.Save = function() {
	//debugger;
	var inError = false;

	if (!inError) {
		// close all the notifications currently openned
		FMErrorAndExceptionHandling.CloseNotifications();


	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = { };
	headers['__RequestVerificationToken'] = token;
	var FCEDevice = $('#FCEDeviceEditorForm').serializeArray();
		let url = $('#FCEDeviceAction').val();

	$.ajax({
		url: url,
		cache: false,
		type: 'POST',
		headers: headers,
		data: FCEDevice,
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					$('#EditFCEDeviceModal').modal('hide');
					if ($('#FCEDeviceLibrary').val() == 'true') {
						window.location.reload(true);
					}
					else {
						window.location.reload(true);
					}
				}

				// hide the saving animation
				$(".loadingDiv").remove();
			});
      },
		error: function (xhr, textStatus, error) {
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () {
					// hide the saving animation
					$(".loadingDiv").remove();
				});
			}
		});
	}
}






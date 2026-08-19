var FCEEMapping = FCEEMapping || {};
var AddKey;


var FCEEMapping = function () {
	var _stack_bottomright_vcfsettings = { "dir1": 'up', "dir2": 'left', "firstpos1": 75, "firstpos2": 25, "context": $('#ModulePropertyEditorPropertyScreen') };


	var _DeleteFCEEMapping = function (fceeMappingGuid) {
		var url = $('#urlDeleteFCEEMapping').val();
		var token = $('#FCEEMappingViewForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		var payload = $('#FCEEMappingViewForm').serializeArray();
		payload.push({ name: 'id', value: fceeMappingGuid });
		headers['__RequestVerificationToken'] = token;

		var messageAttributes = { addclass: 'stack-bottomright', stack: FCEEMapping.stack_bottomright_vcfsettings, width: '150px' };
		// remove any notification

		$.ajax({
			url: url,
			type: 'post',
			headers: headers,
			data: payload,
			success: function (result) {
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError) {
						$('#moduleEditorPropertyScreen').html(data);
					},
					messageAttributes);
			},
			error:
				function (request, status, error) {
					FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				}
		});
	};
	var _EditFCEEMapping = function (fceeMappingGuid, pointGuid, imei, msgType, index, device, tagSelection) {
		var inError = validate(msgType, index, device)==false;

		if (!inError) {
			// close all the notifications currently openned
			FMErrorAndExceptionHandling.CloseNotifications();

			var url = $('#urlEditFCEEMapping').val();
			var token = $('#FCEEMappingViewForm input[name=__RequestVerificationToken]').val();
			var headers = {};
			var payload = $('#FCEEMappingViewForm').serializeArray();
            payload.push({ name: 'device', value: device });
            payload.push({ name: 'tagSelection', value: tagSelection });
			payload.push({ name: 'index', value: index });
			payload.push({ name: 'msg', value: msgType });
			payload.push({ name: 'imei', value: imei });
			payload.push({ name: 'pointId', value: pointGuid });
			payload.push({ name: 'id', value: fceeMappingGuid });
			headers['__RequestVerificationToken'] = token;

		//	var messageAttributes = { addclass: 'stack-bottomright', stack: FCEEMapping.stack_bottomright_vcfsettings, width: '150px' };
			// remove any notification

			$.ajax({
				url: url,
				type: 'post',
				headers: headers,
				data: payload,
				success: function (result) {
					FMErrorAndExceptionHandling.HandleMessages(result,
						function (data, inError) {
							if (!inError) {
								debugger;
								$('#moduleEditorPropertyScreen').html(data);
								let fceeMappingViewUrl = $('#urlFCEEMappingView').val();
								window.location = fceeMappingViewUrl;
							}

						});
					//,messageAttributes);
				},
				error:
					function (request, status, error) {
						FMErrorAndExceptionHandling.ShowException(request, status, error, null);
							//, messageAttributes);
					}
			});
		}
	};
	var _AddFCEEMapping = function (fceeMappingwahGuid) {
		var url = $('#urlAddFCEEMapping').val();
		var token = $('#FCEEMappingViewForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		var payload = $('#FCEEMappingViewForm').serializeArray();
		payload.push({ name: 'id', value: fceeMappingwahGuid });
		headers['__RequestVerificationToken'] = token;

		var messageAttributes = { addclass: 'stack-bottomright', stack: FCEEMapping.stack_bottomright_vcfsettings, width: '150px' };
		// remove any notification

		$.ajax({
			url: url,
			type: 'post',
			headers: headers,
			data: payload,
			success: function (result) {
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError) {
						$('#moduleEditorPropertyScreen').html(data);
					},
					messageAttributes);
			},
			error:
				function (request, status, error) {
					FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				}
		});
	};
	var _updateSaveButton = function () {
		if ($('#StrapTableEditorTableBody').find('tr').length < 1) {
			$('#PEMPESavePropertyScreen').attr('disabled', true);
			$('#PEMPESavePropertyScreen').attr('style', "cursor: not-allowed !important; background-color: lightgrey !important");
		}
		else {
			$('#PEMPESavePropertyScreen').removeAttr('disabled');
			$('#PEMPESavePropertyScreen').removeAttr('style');
		}
	}
	return {
		AddFceeMapping: _AddFCEEMapping,
		EditFCEEMapping: _EditFCEEMapping,
		DeleteFCEEMapping: _DeleteFCEEMapping,
		stack_bottomright_vcfsettings: _stack_bottomright_vcfsettings,
	};
}();

$(document).ready(function () {
	let saveButton = $("#addMapping1");
    let inputElements = $("#StrapTableEditorTableBody :input"); 
    if (inputElements){
        inputElements.on("blur", function(){this.setCustomValidity("");this.reportValidity();});
    }

	if (saveButton) {
        saveButton.on("click", function(){ 
            for (let i = 0; i < inputElements.length ; i++){ 
                inputElements[i].setCustomValidity("");
                inputElements[i].reportValidity(); 
            }
        });
        let cmd = "javascript: FCEEMapping.EditFCEEMapping(document.getElementById('fceeguidT').innerHTML,document.getElementById('AddPoint').value,document.getElementById('AddFceDevice').value,document.getElementById('AddMsgType').value,document.getElementById('AddIndex').value,document.getElementById('AddDevice').value,document.getElementById('AddTagSelection').value);";
		let form = $('#FCEEMappingViewForm');
		form.attr("action", cmd);
	}

});

function openEditor() {
    document.getElementById('updatewindow').style.display = 'inline-block';
    document.getElementById('grayed-out').style.display = 'inline-block';
}

function resetEditor() {
    document.getElementById('AddMsgType').value = '';
    document.getElementById('AddIndex').value = '';
    document.getElementById('AddFceDevice').value = '';
    document.getElementById('fceeguidT').innerHTML = '';
    document.getElementById('AddPoint').value = '';
    document.getElementById('AddDevice').value = '';
}

function hideDeviceField() {
	//debugger;
	FMErrorAndExceptionHandling.CloseNotifications();
	//FMErrorAndExceptionHandling.ShowError("hello world.", function () { });
    if (document.getElementById('AddMsgType').value == 18 || document.getElementById('AddMsgType').value <= 15 || document.getElementById('AddMsgType').value >= 20) {
		document.getElementById('AddDevice').disabled = true;
		document.getElementById('AddDevice').value = '';
    } else {
		document.getElementById('AddDevice').disabled = false;
    }
}

function hideTagSelectionField() {
    if (document.getElementById('AddMsgType').value != 7 ) {
        document.getElementById('AddTagSelection').disabled = true;
        document.getElementById('AddTagSelection').value = 'None';
    } else {
        document.getElementById('AddTagSelection').disabled = false;
    }
}

function validate(msgStr, indexStr, deviceStr){
    let msg = parseInt(msgStr) ;
    let index =  parseInt(indexStr) ;
    let device =  parseInt(deviceStr) ;
    let ierr = '';
    let derr = '';
   // let perr = '';
    let merr = '';
	FMErrorAndExceptionHandling.CloseNotifications();
    FMErrorAndExceptionHandling.ClearControlErrors();
  //  debugger;
    FMErrorAndExceptionHandling.OnlyOneNotification=true;
    err = '';
    let i = $("#AddIndex");
    let d = $("#AddDevice");
 //   let p = $("#AddPoint");
    let m = $("#AddMsgType");
    /**/
    debugger;
    
    if (isNaN(msg) || msg < 1 || msg > 21)
    {
        if (m.length > 0){
            m[0].setCustomValidity("Invalid message type.");
            m[0].reportValidity();
        }   
        return false;
    }
    if (isNaN(index))//|| index < 0 || index > 255)
    {
         if (i.length > 0){
            i[0].setCustomValidity("Invalid Index value.");
            i[0].reportValidity();
        } 
        return false;
    }
    switch (msg)
    {
        case 1:
        case 2:

            if (index != 0)
            {
                ierr="Invalid Index value. Valid values: 0.";
            }

            break;

        case 3:
            if (index < 0 || index > 16)
            {
                ierr ="Invalid Index value. Valid values: 0-16.";
            }

            break;
        case 4:
            if (index < 0 || index > 119)
            {
                ierr = "Invalid Index value. Valid values: 0-119.";
            }
            break;
        case 5:
            if (index < 0 || index > 119)
            {
                ierr="Invalid Index value. Valid values: 0-119.";
            }
            break;
        case 6:
            if (index < 0 || index > 31)
            {
                ierr="Invalid Index value. Valid values: 0-31.";
            }
            break;
        case 7:
            if (index < 0 || index > 95)
            {
                ierr="Invalid Index value. Valid values: 0-95.";
            }
            break;
        case 8:
            if (index < 0 || index > 127)
            {
                ierr="Invalid Index value. Valid values: 0-127.";
            }
            break;
        case 9:
            if (index < 0 || index > 31)
            {
                ierr="Invalid Index value. Valid values: 0-31.";
            }
            break;
        case 10:
            if (index < 0 || index > 31)
            {
                ierr="Invalid Index value. Valid values: 0-31.";
            }
            break;
        case 11:
            if (index < 0 || index > 31)
            {
                ierr="Invalid Index value. Valid values: 0-31.";
            }
            break;
        case 12:
            if (index < 0 || index > 3)
            {
                ierr="Invalid Index value. Valid values: 0-3.";

            }
            break;
        case 13:
            if (index < 1 || index > 16)
            {
                ierr="Invalid Index value. Valid values: 1-16.";
            }
            break;
        case 14:
            if (index < 1 || index > 16)
            {
                ierr="Invalid Index value. Valid values: 1-16.";
            }
            break;
        case 15:
            if (index < 1 || index > 16)
            {
                ierr="Invalid Index value. Valid values: 1-16.";
            }
            break;
        case 16:
            if (index < 1 || index > 11)
            {
                ierr = "Invalid Index value. Valid values: 1-11.";
            }
            if (isNaN(device) || device < 1 || device > 9)
            {
                derr = "Invalid device number. Valid values are between 1 and 9.";
            }
            break;
        case 17:
            if (index < 1 || index > 11)
            {
                ierr= "Invalid Index value. Valid values: 1-11.";
            }
            if (isNaN(device) || device < 1 || device > 9)
            {
                derr="Invalid device number. Valid values are between 1 and 9.";
            }
            break;
        case 18:
            if (index < 1 || index > 11)
            {
                ierr="Invalid Index value. Valid values: 1-11.";

            }
            break;
        case 19:
            if (index < 1 || index > 2)
            {
                ierr = "Invalid Index value. Valid values: 1-2.\n";
            }
            if (isNaN(device) || device < 1 || device > 2)
            {
                derr="Invalid device number. Valid values are between 1 and 2.";
            }
            break;
        case 20:
            if (index < 0 || index > 119) {
                ierr = "Invalid Index value. Valid values: 0-119.\n";
            }

            break;
        case 21:
            if (index < 1 || index > 48) {
                ierr = "Invalid Index value. Valid values: 1-48.\n";
            }

            break;
        default:
            break;

    }
    if (ierr != ''){
        if (i.length > 0){
            i[0].setCustomValidity(ierr);
            i[0].reportValidity();
        }
        return false;
    }
    if (derr != ''){
        if (d.length > 0){
            d[0].setCustomValidity(derr);
            d[0].reportValidity();
        }
        return false;
    }
    if (merr != ''){
        if (m.length > 0){
            m[0].setCustomValidity(merr);
            m[0].reportValidity();
        }
        return false;
    }

    return true;
}


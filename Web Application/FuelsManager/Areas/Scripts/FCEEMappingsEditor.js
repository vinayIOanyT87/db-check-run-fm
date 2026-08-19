var FCEEMappingsEditor = FCEEMappingsEditor || {};
var AddKey;

window.ReloadAfterFCEE = false;
var FCEEMappingsEditor = function () {

	var _stack_bottomright_vcfsettings = { "dir1": 'up', "dir2": 'left', "firstpos1": 75, "firstpos2": 25, "context": $('#ModulePropertyEditorPropertyScreen') };
	var _isReadOnly = function () {
        var readOnly = $('#FCEEMappingsReadOnly').val().toLowerCase() === "true";
        //console.log("$('#FCEEMappingsReadOnly').val()=" + $('#FCEEMappingsReadOnly').val() + "  readOnly = " + readOnly );
		return readOnly;
	};
	var _SaveChanges = function () {
		if (_isReadOnly()) {
			return;
		}

		var url = $('#urlSaveFCEEMappings').val();
		var token = $('#FCEEMappingsEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		var payload = $('#FCEEMappingsEditorForm').serializeArray();
		headers['__RequestVerificationToken'] = token;

		var messageAttributes = { addclass: 'stack-bottomright', stack: FCEEMappingsEditor.stack_bottomright_vcfsettings, width: '150px' };
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
                            FCEEMappingsEditor.updateSaveButton(false);

                             window.ReloadAfterFCEE = true;

						}
					},
					messageAttributes);
			},
			error:
				function (request, status, error) {
					FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				}
		});
	};
	var _DeleteFCEEMapping = function (fceeMappingGuid) {
		if (_isReadOnly()) {
			return;
		}

		var url = $('#urlDeleteFCEEMapping').val();
		var token = $('#FCEEMappingsEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		var payload = $('#FCEEMappingsEditorForm').serializeArray();
		payload.push({ name: 'fceeMappingGuid', value: fceeMappingGuid });
		headers['__RequestVerificationToken'] = token;

		var messageAttributes = { addclass: 'stack-bottomright', stack: FCEEMappingsEditor.stack_bottomright_vcfsettings, width: '150px' };
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
                FCEEMappingsEditor.updateSaveButton(true);
			},
			error:
				function (request, status, error) {
					FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				}
		});
	};
    var _updateSaveButton = function (enable) {
		if (_isReadOnly()) {
			enable = false;
		}
		if (enable == false){//$('#StrapTableEditorTableBody').find('tr').length < 1) {
			$('#PEMPESavePropertyScreen').attr('disabled', true);
			$('#PEMPESavePropertyScreen').attr('style', "cursor: not-allowed !important; background-color: lightgrey !important");
		}
		else {
			$('#PEMPESavePropertyScreen').removeAttr('disabled');
			$('#PEMPESavePropertyScreen').removeAttr('style');
		}
        let inputElements = $("#StrapTableEditorTableBody :input"); 

        for (let i = 0; i < inputElements.length ; i++){ 
                inputElements[i].setCustomValidity("");
                inputElements[i].reportValidity(); 
        }

	}
	return {
		SaveChanges: _SaveChanges,
		stack_bottomright_vcfsettings: _stack_bottomright_vcfsettings,
		DeleteFCEEMapping: _DeleteFCEEMapping,
		updateSaveButton: _updateSaveButton,
		isReadOnly: _isReadOnly
	};
}();
// manually hookup to the submit the form
$(function () {
	$('#FCEEMappingsEditorForm').submit(function () {
		if (FCEEMappingsEditor.isReadOnly()) {
			return false;
		}

        if(validate() == false){
            return false;
        }
		var action = this.action;
		var method = this.method;

		FCEEMappingsEditor.SaveChanges();
		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	});
});


$("#addRow").click(function () {

	if (FCEEMappingsEditor.isReadOnly()) {
		return false;
	}

	var url = $('#urlAddFCEEMapping').val();
	var token = $('#FCEEMappingsEditorForm input[name=__RequestVerificationToken]').val();
	var pointGuid = $('#FCEEMappingsEditorForm input[name=PointGuid]').val();
	var headers = {};
	var payload = $('#FCEEMappingsEditorForm').serializeArray();
	headers['__RequestVerificationToken'] = token;

	var messageAttributes = { addclass: 'stack-bottomright', stack: FCEEMappingsEditor.stack_bottomright_vcfsettings, width: '150px' };
	// remove any notification
	$.ajax({
		url: url,
		type: 'post',
		headers: headers,
		data: payload,
		success: function (result) {
            $('#moduleEditorPropertyScreen').html(result.Data);
            FCEEMappingsEditor.updateSaveButton(true);

		},
		error:
			function (request, status, error) {
				FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
			}
	});
});

$(document).ready(function () {
	let x = $('#FCEEMappingsEditorTable :input ');
    //debugger;
	if (x && !FCEEMappingsEditor.isReadOnly()){
		x.change(function(){FCEEMappingsEditor.updateSaveButton(true)});
	}

    let inputElements = $("#StrapTableEditorTableBody :input"); 
    if (inputElements){
        inputElements.on("blur", function(){this.setCustomValidity("");this.reportValidity();});
    }

	if (FCEEMappingsEditor.isReadOnly()) {
		$('#FCEEMappingsEditorTable :input:not([type=hidden])').prop('disabled', true);
		$('#addRow').prop('disabled', true).hide();
		FCEEMappingsEditor.updateSaveButton(false);
	}
	/*
	x = $('#FCEEMappingsEditorTable input');
	if (x){
		x.change(function(){FCEEMappingsEditor.updateSaveButton(true)});
	}
	*/
	FCEEMappingsEditor.updateSaveButton(false);
});



function validate(){
    //debugger;
    let indexEl = $("#StrapTableEditorTableBody input[name^='FCEEMappings'][name$='Index']");
    let deviceEl = $("#StrapTableEditorTableBody input[name^='FCEEMappings'][name$='Device']");
    let msgTypeEl = $("#StrapTableEditorTableBody select[name^='FCEEMappings'][name$='MsgType']");
    let noErrors = true;

    for(let j=0; j < indexEl.length; j++){
    let i = indexEl[j];
    let d = deviceEl[j];
 //   let p = $("#AddPoint");
    let m = msgTypeEl[j];
    /**/

    let msgStr=m.value;
    let indexStr=i.value;
    let deviceStr=d.value;

    let msg = parseInt(msgStr) ;
    let index =  parseInt(indexStr) ;
    let device =  parseInt(deviceStr) ;
    let ierr = '';
    let derr = '';
   // let perr = '';
    let merr = '';
	FMErrorAndExceptionHandling.CloseNotifications();
    FMErrorAndExceptionHandling.ClearControlErrors();

    FMErrorAndExceptionHandling.OnlyOneNotification = true;
    err = '';

    
    if (isNaN(msg) || msg < 1 || msg > 21)
    {
        if (m){
            m.setCustomValidity("Invalid message type.");
            m.reportValidity();
        }   
        noErrors = false;
        
    }
    if (isNaN(index))//|| index < 0 || index > 255)
    {
         if (i){
            i.setCustomValidity("Invalid Index value.");
            i.reportValidity();
        } 
        noErrors= false;
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
        default:
            break;

    }
    if (ierr != ''){
        if (i){
            i.setCustomValidity(ierr);
            i.reportValidity();
        }
        noErrors= false;
    }
    if (derr != ''){
        if (d){
            d.setCustomValidity(derr);
            d.reportValidity();
        }
        noErrors= false;
    }
    if (merr != ''){
        if (m){
            m.setCustomValidity(merr);
            m.reportValidity();
        }
        noErrors= false;
    }
    if (noErrors == false){
        break;
    }
    }
    return noErrors;
}


var AckCommentDataEntry = AckCommentDataEntry || { notAllowed: '!@#$%^&*()=\\|{}~`[]+\':;?><,."' };

AckCommentDataEntry.notAllowed = '!@#$%^&*()=\\|{}~`[]+\':;?><,."';

AckCommentDataEntry.AckCommentDataEntrySelectionOkButtonPressAction = function () {
	var url = $('#ackCommentDataEntryUrlOkButtonPress').val();
	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var comment = $('#AckCommentDataEntryComment').val();
	comment = comment.replace(/<script.*?<\/script>/gi, "js injection detected");

	var modelString = document.getElementById('AckCommentDataEntry').value;

	AckCommentDataEntry.stack_bottomright_ackcomment = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#AckCommentDataEntryScreen') }

	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: AckCommentDataEntry.stack_bottomright_ackcomment };
	// remove previous notifications
	PNotify.removeStack(AckCommentDataEntry.stack_bottomright_ackcomment);
	
	$.ajax({
		url: url,
		cache: false,
		type: 'POST',
		dataType: 'json',
		contentType: 'application/json; charset=utf-8',
		headers: headers,
		data: JSON.stringify({
			comment: comment,
			modelString: modelString
		}),
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (retModel, inError) {
				if (inError) {
					return;
				}

				if (typeof retModel === 'string') {
					$('#ackCommentError').html(retModel);
					return;
				}
				$('#AckCommentDataEntryScreen').modal('hide');
			}, messageAttributes);
		},
		error: function (request, status, error) {
			FMErrorAndExceptionHandling.ShowError('Error Acknowledging With Comment', null, messageAttributes);
		}
	});
};

AckCommentDataEntry.Onkeypress = function (event) {
	event = event || window.event;
	var charCode = event.which || event.keyCode;
	var charStr = String.fromCharCode(charCode);
	if (AckCommentDataEntry.notAllowed.indexOf(charStr) >= 0) {
		return false;
	}
};

AckCommentDataEntry.OnPaste = function (e) {
	var element = e;
	setTimeout(function () {
		var text = $(element).val();
		var notAllowedSplit = AckCommentDataEntry.notAllowed.split("");
		for (var i = 0; i < notAllowedSplit.length; i++)
		{
			var unallowed = AckCommentDataEntry.ConditionSpecialCharactersForRegEx(notAllowedSplit[i]);
			text = text.replace(new RegExp(unallowed, 'g'), "");
		}
		$( element ).val( text );
	}, 0);
};

AckCommentDataEntry.ConditionSpecialCharactersForRegEx = function (chr)
{
	var specialCharacters = "+^.*()[]-\\?$|";
	if (specialCharacters.indexOf(chr) >= 0) {
		return '\\' + chr;
	}
	return chr;
};
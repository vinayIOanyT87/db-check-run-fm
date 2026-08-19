var ShelveDataEntry = ShelveDataEntry ||
	{
		 stack_bottomright_shelve: {
			 "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#SheveDataEntryPartial') 
		 }
	};

ShelveDataEntry.HandleRadioButton = function( radio, checked )
{
	var label = $("#" + radio.id).next();
	if ( checked )
	{
		label.addClass( 'shelveDataEntryRadioLabelChecked' );
	}
	else
	{
		label.removeClass('shelveDataEntryRadioLabelChecked');
	}
	radio.checked = checked;
}

ShelveDataEntry.RadioButtonChecked = function () {
	var allRadios = ShelveDataEntry.GetRadioButtons();
	for ( var x = 0; x < allRadios.length; x++ )
	{
		if ( this !== allRadios[x] )
		{
			ShelveDataEntry.HandleRadioButton(allRadios[x], false);
		}
		else
		{
			ShelveDataEntry.HandleRadioButton(allRadios[x], true);
		}
	}
	ShelveDataEntry.DisableTimeEntry(ShelveDataEntry.IsOneShot());
	return true;
}

ShelveDataEntry.DisableTimeEntry = function (disable)
{
	var mins = document.getElementById("ShelveDataEntryMinutes");
	var hours = document.getElementById("ShelveDataEntryHours");
	var days = document.getElementById("ShelveDataEntryDays");

	mins.disabled = disable;
	hours.disabled = disable;
	days.disabled = disable;
	var numTxt = "";
	if ( !disable )
	{
		numTxt = "0";
	}
	$('#' + hours.id).val(numTxt);
	$('#' + mins.id).val(numTxt);
	$('#' + days.id).val(numTxt);

}

ShelveDataEntry.IsOneShot = function()
{
	return document.getElementById("oneshotradiobutton").checked;
}

ShelveDataEntry.GetRadioButtons = function()
{
	var allRadios = [document.getElementById("oneshotradiobutton"), document.getElementById("timeshelveradiobutton")];
	return allRadios;
}

ShelveDataEntry.GetTextInputs = function ()
{
	var allTextInputs = [document.getElementById("ShelveDataEntryMinutes"), document.getElementById("ShelveDataEntryHours"), document.getElementById("ShelveDataEntryDays")];
	return allTextInputs;
}

ShelveDataEntry.Init = function()
{
	var allRadios = ShelveDataEntry.GetRadioButtons();
	for (var x = 0; x < allRadios.length; x++)
	{
		allRadios[x].onclick = ShelveDataEntry.RadioButtonChecked;
	}
	var allTextInputs = ShelveDataEntry.GetTextInputs();
	for (var i = 0; i < allRadios.length; i++) {
		allTextInputs[i].onkeypress = ShelveDataEntry.PreventEnterSubmit;
		allTextInputs[i].onkeydown = ShelveDataEntry.AllowNumbers;
	}
	ShelveDataEntry.HandleRadioButton(allRadios[0], true);
	ShelveDataEntry.DisableTimeEntry( true );
}

ShelveDataEntry.PreventEnterSubmit = function (e) {
	//Prevent Post
	e = e || event;
	return (e.keyCode || e.which || e.charCode || 0) !== 13;
}

ShelveDataEntry.AllowNumbers = function (e) {
	// Allow: backspace, delete, tab, escape, enter and .
	if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
		// Allow: Ctrl/cmd+A
		 (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
		// Allow: Ctrl/cmd+C
		 (e.keyCode == 67 && (e.ctrlKey === true || e.metaKey === true)) ||
		// Allow: Ctrl/cmd+X
		 (e.keyCode == 88 && (e.ctrlKey === true || e.metaKey === true)) ||
		// Allow: home, end, left, right
		 (e.keyCode >= 35 && e.keyCode <= 39)) {
		// let it happen, don't do anything
		return;
	}
	// Ensure that it is a number and stop the keypress
	if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
		e.preventDefault();
	}
}

ShelveDataEntry.ShelveDataEntrySelectionOkButtonPressAction = function () {
	var url = $('#shelveDataEntryUrlOkButtonPress').val();
	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var days = $('#ShelveDataEntryDays').val();
	var hours = $('#ShelveDataEntryHours').val();
	var minutes = $('#ShelveDataEntryMinutes').val();
	var oneShot = ShelveDataEntry.IsOneShot();

	var modelString = document.getElementById('ShelveDataEntry').value;

	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: ShelveDataEntry.stack_bottomright_shelve };
	// remove previous notifications
	PNotify.removeStack(ShelveDataEntry.stack_bottomright_shelve);

	$.ajax({
		url: url,
		cache: false,
		type: 'POST',
		headers: headers,
		data: 'days=' + days + '&hours=' + hours + '&minutes=' + minutes + '&oneShot=' + oneShot + '&modelString=' + modelString,
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (retModel, inError) {
				if (inError) {
					return;
				}

				if (typeof retModel === 'string') {
					$('#shelveError').html(retModel);
					return;
				}
			}, messageAttributes);
		},
		error: function (request, status, error) {
			FMErrorAndExceptionHandling.ShowError('Error shelving', null, messageAttributes);
		}
	});
}
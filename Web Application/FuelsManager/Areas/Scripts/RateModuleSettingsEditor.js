//debugger;

// create a class with helper functions for the vcf settings editor
var FMRateModuleSettingsEditor = function ()
{
	var stackBottomRightRateModulesettings = { "dir1": 'up', "dir2": 'left', "firstpos1": 75, "firstpos2": 25, "context": $('#ModulePropertyEditorPropertyScreen') };

	var saveChanges = function ()
	{
		var url = $('#urlSaveRateModuleSettings').val();
		var token = $('#RateModuleSettingsEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		var messageAttributes = { addclass: 'stack-bottomright', stack: FMRateModuleSettingsEditor.stack_bottomright_RateModuleSettings, width: '450px' };

		// remove any notification
		PNotify.removeStack(FMRateModuleSettingsEditor.stack_bottomright_RateModuleSettings);

		$.ajax({
			url: url,
			type: 'post',
			headers: headers,
			data: $('#RateModuleSettingsEditorForm').serialize(),
			success: function (result)
			{
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError)
					{
					},
					messageAttributes);
			},
			error:
				function (request, status, error)
				{
					FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
				}
		});
	};

	return	{
				SaveChanges: saveChanges,
				stack_bottomright_RateModuleSettings: stackBottomRightRateModulesettings
			};
}();

// manually hookup to the submit the form
$(function ()
{
	$('#RateModuleSettingsEditorForm').submit(function ()
	{
		var action = this.action;
		var method = this.method;

		FMRateModuleSettingsEditor.SaveChanges();

		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	});
});


//--------------------------------------- RUN after page has been loaded but before render -----------------------------

$(document).ready(function ()
{
	// Hide the Header, as RateModuleSettingsEditor provides one
	$('.modal-header').hide();

	if ($('#Readonly').val() === 'True')
	{
		$('#PEMPESavePropertyScreen').attr('disabled', true);
	}

	FMErrorAndExceptionHandling.CloseNotifications();
});
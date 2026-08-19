var ModuleLibrary = ModuleLibrary || {};
var AddKey;
var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };


ModuleLibrary.CreateModule = function()
{
	console.log("ModuleLibrary.CreateModule ");
}

ModuleLibrary.EditModule = function (moduleGuidString) {
	$('body').modalmanager('loading');
	// close all the notifications currently openned
	FMErrorAndExceptionHandling.CloseNotifications();

	var token = $('#moduleLibraryForm input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	var moduleEditorData = { moduleGuidString: moduleGuidString, moduleLibrary: 'true' };

	var urlModuleEditor = $('#urlModuleEditor').val();

	AddKey = $('#addModule1').attr('accesskey');
	$('#addModule1').removeAttr('accesskey');
	$('#addModule2').removeAttr('accesskey');


	$.ajax({
		type: "GET",
		cache: false,
		headers: headers,
		url: urlModuleEditor,
		data: moduleEditorData,
		success: function (response)
		{
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError)
			{
				if (!inError)
				{
					// replace the holder with the partial view
					$('#moduleEditorScreen').html(data);
					// show the modal
					$('#EditModuleModal').modal('show');
				}
				else
				{
					// remove the loading of the modal
					var modalManager = $("body").data("modalmanager");
					modalManager.removeLoading();
				}
			});

		},
		error: function (xhr, textStatus, error)
		{
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function ()
				{
					// remove the loading of the modal
					var modalManager = $("body").data("modalmanager");
					modalManager.removeLoading();
				});
		}
	});
}

ModuleLibrary.DeleteModule = function (moduleGuid) {
	var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator };

	// remove previous notifications
	PNotify.removeStack(stack_bottomright_operator);

		$.ajax({
			type: 'POST',
			url: 'Delete',
			cache: false,
			async: false,
			data: {
				id: moduleGuid,
				'__RequestVerificationToken': $('input[name=__RequestVerificationToken]').val()
			},
			success: function (response)
			{
				FMErrorAndExceptionHandling.HandleMessages(response, function( data, inError )
				{
					if ( !inError )
					{
						var tbl = document.getElementById("ModuleLibraryTable");
						var tr = document.getElementById("row_" + moduleGuid);
						if ( tr )
						{
							tbl.deleteRow(tr.rowIndex);
						}
					}
				});
			},
			error: function (xhr, textStatus, error)
			{
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () { },
					messageAttributes);
			}
		});
		//Prevent Post
		return false;
}

ModuleLibrary.SaveAllModules = function () {
	$('#saveAll1').addClass('disabled').prop('disabled', true);
	$('#saveAll2').addClass('disabled').prop('disabled', true);

	var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator };

	// remove previous notifications
	PNotify.removeStack(stack_bottomright_operator);

	$.ajax({
		type: 'POST',
		url: 'SaveAllModules',
		cache: false,
		async: true,
		data: {
			'__RequestVerificationToken': $('input[name=__RequestVerificationToken]').val()
		},
		success: function (response) {
			$('#saveAll1').removeClass('disabled').prop('disabled', false);
			$('#saveAll2').removeClass('disabled').prop('disabled', false);
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
				}
			});
		},
		error: function (xhr, textStatus, error) {
			$('#saveAll1').removeClass('disabled').prop('disabled', false);
			$('#saveAll2').removeClass('disabled').prop('disabled', false);
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () { },
				messageAttributes);
		}
	});

	//Prevent Post
	return false;
}


ModuleLibrary.RestoreAccessKeys = function()
{
	$('#addModule2').attr('accesskey', AddKey);
	$('#addModule1').attr('accesskey', AddKey);
}
var FMMovementUserDataEditor = function ()
{
	var _inError = false;
	var _emptyGuid = '00000000-0000-0000-0000-000000000000';
	var _stack_bottomright_movementuserdataeditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 200, "firstpos2": 50, "context": $('#MovementUserDataEditorPropertyScreen') };

	//===============================================================
	// This function is a hookup to the main property page.
	// It is called by the Save button (id = MovementUserDataEditorSavePropertyScreen)
	//===============================================================
	var _SaveChanges = function ()
	{
		// Update the model based on the UI changes.
		FMMovementUserDataEditor.UpdateModel();

		var url = $('#MovementUserDataSaveUrl').val();
		var token = $('#MovementUserDataEditorForm input[name=__RequestVerificationToken]').val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMMovementUserDataEditor.Stack_bottomright_movementuserdataeditor, width: '450px' };

		// remove previous notifications
		PNotify.removeStack(FMMovementUserDataEditor.Stack_bottomright_movementuserdataeditor);

		var movementUserDataEditorModelStr = FMMovementUserDataEditor.GetModelString();

		$.ajax({
			cache: false,
			url: url,
			type: 'POST',
			headers: headers,
			async: false,
			dataType: "json",
			contentType: 'application/json; charset=UTF-8',
			data: JSON.stringify({ 'movementUserDataEditorModelStr': movementUserDataEditorModelStr }),
			success: function (result)
			{
				FMErrorAndExceptionHandling.HandleMessages(result,
					function (data, inError)
					{
						if (!inError)
						{
						}
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

	//=====================================================================
	// This function initializes the movement module settings editor
	//=====================================================================
	var _Initialize = function ()
	{
		FMMovementUserDataEditor.LoadData();
	};

	//===================================================
	// This function returns the movement user data 
	// model as a string.
	//===================================================
	_GetModelString = function ()
	{
		return $('#MovementUserDataEditorModelStr').val();
	};

	//===================================================
	// This function returns the movement user data 
	// model as an object.
	//===================================================
	_GetModel = function ()
	{
		return JSON.parse(_GetModelString());
	};

	//===================================================
	// This function set the movement user data model as
	// a string.
	//===================================================
	_SetModelString = function (modelStr)
	{
		$('#MovementUserDataEditorModelStr').val(modelStr);
	};

	//===================================================
	// This function set the movement user data model as
	// into a hidden tag.
	//===================================================
	_SetModel = function (model)
	{
		var modelStr = JSON.stringify(model);
		_SetModelString(modelStr);
	};

	//=====================================================================
	// This function will load the page with data from the model.
	//=====================================================================
	_LoadData = function ()
	{
		var model = FMMovementUserDataEditor.GetModel();

		if (!model)
		{
			return;
		}

		$("#MovementUserData01TB").val(model.UserData01);
		$("#MovementUserData02TB").val(model.UserData02);
		$("#MovementUserData03TB").val(model.UserData03);
		$("#MovementUserData04TB").val(model.UserData04);
		$("#MovementUserData05TB").val(model.UserData05);
		$("#MovementUserData06TB").val(model.UserData06);
		$("#MovementUserData07TB").val(model.UserData07);
		$("#MovementUserData08TB").val(model.UserData08);
		$("#MovementUserData09TB").val(model.UserData09);
		$("#MovementUserData10TB").val(model.UserData10);
	};

	//========================================================================
	// This function will update the model based on the values from the UI.
	//========================================================================
	_UpdateModel = function ()
	{
		var model = FMMovementUserDataEditor.GetModel();

		if (model)
		{
			model.UserData01 = $.trim($("#MovementUserData01TB").val());
			model.UserData02 = $.trim($("#MovementUserData02TB").val());
			model.UserData03 = $.trim($("#MovementUserData03TB").val());
			model.UserData04 = $.trim($("#MovementUserData04TB").val());
			model.UserData05 = $.trim($("#MovementUserData05TB").val());
			model.UserData06 = $.trim($("#MovementUserData06TB").val());
			model.UserData07 = $.trim($("#MovementUserData07TB").val());
			model.UserData08 = $.trim($("#MovementUserData08TB").val());
			model.UserData09 = $.trim($("#MovementUserData09TB").val());
			model.UserData10 = $.trim($("#MovementUserData10TB").val());

			FMMovementUserDataEditor.SetModel(model);
		}
	};

	//======================================================
	// Return function pointers
	//======================================================
	return {
		inError: _inError
		, emptyGuid: _emptyGuid
		, SaveChanges: _SaveChanges
		, Initialize: _Initialize
		, GetModelString: _GetModelString
		, GetModel: _GetModel
		, SetModelString: _SetModelString
		, SetModel: _SetModel
		, Stack_bottomright_movementuserdataeditor: _stack_bottomright_movementuserdataeditor
		, LoadData: _LoadData
		, UpdateModel: _UpdateModel
	};
}();


//=======================================================
// This function manually hooks up to the submit the form
//=======================================================
$(function ()
{
	$('#MovementUserDataEditorForm').on('keyup keypress', function (e)
	{
		var keyCode = e.keyCode || e.which;
		if (keyCode === 13)
		{
			e.preventDefault();
			return false;
		}
	});

	$('#MovementUserDataEditorForm').submit(function ()
	{
		FMMovementUserDataEditor.SaveChanges();

		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	});
});


//=======================================================================
// RUN after page has been loaded but before render
//=======================================================================
$(document).ready(function ()
{
	// Initialize the movement user data
	FMMovementUserDataEditor.Initialize();

	// Hide the Header, as MovementUserDataEditor provides one
	$('.modal-header').hide();

	if ($('#Readonly').val() === 'True')
	{
		$('#MovementUserDataEditorSavePropertyScreen').attr('disabled', true);
	}

	FMErrorAndExceptionHandling.CloseNotifications();
	//$(window).resize(function () { MovementHistoryTab.HandleWindowResize(); });
});





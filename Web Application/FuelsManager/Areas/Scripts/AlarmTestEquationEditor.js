var AlarmTestEquationEditor = AlarmTestEquationEditor || {};

AlarmTestEquationEditor.Init = function ()
{
	// event handling
	$(".radiogroup input[type=radio][name=operatormode]").on("change", function () {
		var operatorMode = $("input[type=radio][name=operatormode]:checked").val();

		var model = AlarmTestEquationEditor.GetAlarmTestEquationEditorModel();
		var bitMaskInput = $("#AlarmEquationEditorBitmaskInput");
		if (operatorMode === "bitmap") {
			AlarmTestEquationEditor.UnHideBitmask();
			if ($("#AlarmTestEquationTagFieldDropDown").val() === "1") // if comparing status instead of value
			{
				bitMaskInput.val(model.Bitmask.substr(model.Bitmask.length - 8));
				bitMaskInput.attr("maxlength", 8);
				// reset the comparison operator
				$("#ComparisonOperatorDropDown option").prop("disabled", false);
				if ( $( "#ComparisonOperatorDropDown" ).hasClass( "select2-hidden-accessible" ) )
				{
					$( "#ComparisonOperatorDropDown" ).select2( 'destroy' );
				}
				$("#ComparisonOperatorDropDown").select2({ minimumResultsForSearch: -1 });
			}
			else
			{
				bitMaskInput.val(model.Bitmask.substr(model.Bitmask.length - model.BitMaskDigits));
				bitMaskInput.attr("maxlength", model.BitMaskDigits);
			}
			model.UseBitmask = true;
		}
		else {
			AlarmTestEquationEditor.HideBitmask();
			model.UseBitmask = false;
		}
		AlarmTestEquationEditor.SetAlarmTestEquationEditorModel( model );
	});


	$( "#AlarmTestEquationTagFieldDropDown" ).on( 'change', function()
	{
		var model = AlarmTestEquationEditor.GetAlarmTestEquationEditorModel();
		model.TagAttribute = parseInt($(this).val());
		AlarmTestEquationEditor.SetAlarmTestEquationEditorModel(model);

		var bitMaskInput = $("#AlarmEquationEditorBitmaskInput");
		if ($(this).val() === "1") {

			$("#AlarmTestEquationComparisonSelection").removeClass('hidden');
			bitMaskInput.val(model.Bitmask.substr(model.Bitmask.length - 8));
			bitMaskInput.attr("maxlength", 8);

			$("input[type=radio][name=operatormode]").filter("[value=bitmap]").prop('checked', true);
			$("input[type=radio][name=operatormode]").trigger("change");
		}
		else {
			$("input[type=radio][name=operatormode]").filter("[value=nobitmap]").prop('checked', true);
			$("input[type=radio][name=operatormode]").trigger("change");

			bitMaskInput.attr("maxlength", model.BitMaskDigits);
			
			if (!model.CanUseBitmask)
			{
				$("#AlarmTestEquationComparisonSelection").removeClass('hidden').addClass('hidden');
				if (model.TagType !== "System.Double" && model.TagType !== "System.Single") {
					// if tag is not numeric then we cannot use comparison other than equal/not equal
					// if the previous comparison was not equal or not equal then defeault to equal(4)
					if ($("#ComparisonOperatorDropDown").val() < 4) {
						$("#ComparisonOperatorDropDown").val(4);
					}
					$("#ComparisonOperatorDropDown option").each(function (index) {
						if ($(this).val() < 4) {
							$(this).prop("disabled", true);
						}
					});
					$("#ComparisonOperatorDropDown").select2('destroy');
					$("#ComparisonOperatorDropDown").select2({ minimumResultsForSearch: -1 });
				}
			}
		}
	} );

	// control initialization
	var model = AlarmTestEquationEditor.GetAlarmTestEquationEditorModel();
	// if we are using bitmask we need to default to it
	if ( model.UseBitmask )
	{
		$( "input[type=radio][name=operatormode]" ).filter( "[value=bitmap]" ).prop( 'checked', true );
	}
	else
	{
		$("input[type=radio][name=operatormode]").filter("[value=nobitmap]").prop('checked', true);
	}
	$("input[type=radio][name=operatormode]").trigger("change");

	// initialize select2 for the comparison selection
	$("#ComparisonOperatorDropDown").select2({ minimumResultsForSearch: -1 });

};

AlarmTestEquationEditor.GetAlarmTestEquationEditorModelString = function () {
	return $('#AlarmTestEquationEditorModel').val();
}

AlarmTestEquationEditor.GetAlarmTestEquationEditorModel = function () {
	return JSON.parse(AlarmTestEquationEditor.GetAlarmTestEquationEditorModelString());
}

AlarmTestEquationEditor.SetAlarmTestEquationEditorModelString = function (modelStr) {
	$('#AlarmTestEquationEditorModel').val(modelStr);
}

AlarmTestEquationEditor.SetAlarmTestEquationEditorModel = function (model) {
	var modelStr = JSON.stringify(model);
	AlarmTestEquationEditor.SetAlarmTestEquationEditorModelString(modelStr);
}

AlarmTestEquationEditor.EditHexKeyHandler = function (e) {
	e = e || event;
	if ((e.keyCode || e.which || e.charCode || 0) < 48) {
		return false;
	}
	if ((e.keyCode || e.which || e.charCode || 0) > 57 && (e.keyCode || e.which || e.charCode || 0) < 65) {
		return false;
	}

	if ((e.keyCode || e.which || e.charCode || 0) > 70 && (e.keyCode || e.which || e.charCode || 0) < 96) {
		return false;
	}
	if ((e.keyCode || e.which || e.charCode || 0) > 102) {
		return false;
	}
	return true;
}

AlarmTestEquationEditor.GetUpdates = function ()
{
	
	var model = AlarmTestEquationEditor.GetAlarmTestEquationEditorModel();
	model.TagAttribute = $("#AlarmTestEquationTagFieldDropDown").val();
	model.BitwiseOperator = $("#BitwiseOperatorDropDown").val();

	var rawBitmask = $("#AlarmEquationEditorBitmaskInput").val();
	if ( rawBitmask === "" || model.UseBitmask === false)
	{
		model.Bitmask = -1;
	}
	else
	{
		model.Bitmask = parseInt(rawBitmask, 16);
	}

	model.ComparisonOperator = document.getElementById("ComparisonOperatorDropDown").value;
	return model;
}

AlarmTestEquationEditor.HideBitmask = function()
{
	AlarmTestEquationEditor.UnHideBitmask();
	$("#AlarmTestEquationBitwiseOperatorHeader").addClass('hidden');
	$("#AlarmTestEquationBitwiseOperator").addClass('hidden');
	$("#AlarmTestEquationBitmaskHeader").addClass('hidden');
	$("#AlarmTestEquationBitmask").addClass('hidden');

	$("#AlarmTestEquationTagNameHeader").removeClass('col-sm-2').addClass('col-sm-3');
	$("#AlarmTestEquationTagAttributeHeader").removeClass('col-sm-2').addClass('col-sm-3');
	$("#AlarmTestEquationComparisonHeader").removeClass('col-sm-2').addClass('col-sm-3');
	$("#AlarmTestEquationLimitTagHeader").removeClass('col-sm-2').addClass('col-sm-3');

	$("#AlarmTestEquationTagName").removeClass('col-sm-2').addClass('col-sm-3');
	$("#AlarmTestEquationTagAttribute").removeClass('col-sm-2').addClass('col-sm-3');
	$("#AlarmTestEquationComparison").removeClass('col-sm-2').addClass('col-sm-3');
	$("#AlarmTestEquationLimitTag").removeClass('col-sm-2').addClass('col-sm-3');
}

AlarmTestEquationEditor.UnHideBitmask = function ()
{
	$("#AlarmTestEquationBitwiseOperatorHeader").removeClass('hidden');
	$("#AlarmTestEquationBitwiseOperator").removeClass('hidden');
	$("#AlarmTestEquationBitmaskHeader").removeClass('hidden');
	$("#AlarmTestEquationBitmask").removeClass('hidden');
	$("#AlarmTestEquationTagNameHeader").removeClass('col-sm-3').addClass('col-sm-2');
	$("#AlarmTestEquationTagAttributeHeader").removeClass('col-sm-3').addClass('col-sm-2');
	$("#AlarmTestEquationComparisonHeader").removeClass('col-sm-3').addClass('col-sm-2');
	$("#AlarmTestEquationLimitTagHeader").removeClass('col-sm-3').addClass('col-sm-2');

	$("#AlarmTestEquationTagName").removeClass('col-sm-3').addClass('col-sm-2');
	$("#AlarmTestEquationTagAttribute").removeClass('col-sm-3').addClass('col-sm-2');
	$("#AlarmTestEquationComparison").removeClass('col-sm-3').addClass('col-sm-2');
	$("#AlarmTestEquationLimitTag").removeClass('col-sm-3').addClass('col-sm-2');
}

AlarmTestEquationEditor.DecimalToHexString = function(number, numberOfDigits) 
{
	if (number < 0) {
		number = 0xFFFFFFFFFFFFFFFF + number + 1;
	}

	var retStr = number.toString(16).toUpperCase();
	var strLen = retStr.length;
	retStr = retStr.substring( strLen - numberOfDigits, strLen );
	return retStr;
}


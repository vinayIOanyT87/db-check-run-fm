var FMTAGFIELDSELECTION = FMTAGFIELDSELECTION || {
		VALUE: 0,
		ID: 1,
		TIMESTAMP: 2,
		UNITS: 3,
		ALARMSTATUS: 4,
		GetFieldString: function(field)
		{
			switch(field)
			{
				case FMTAGFIELDSELECTION.VALUE:
					return "VALUE";
				case FMTAGFIELDSELECTION.ID:
					return "ID";
				case FMTAGFIELDSELECTION.TIMESTAMP:
					return "TIMESTAMP";
				case FMTAGFIELDSELECTION.UNITS :
					return "UNITS";
				case FMTAGFIELDSELECTION.ALARMSTATUS:
					return "ALARM STATUS";
				default:
					return "UNKNOWN";
			}
		}
}

var TagSelection = TagSelection || {
	stack_bottomright_AddPoinTtag: { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $( '#AddPointTagsPartial' ) },
	TagSelectionOKCallBackFunction: undefined,
	TagSelectionSaveCallBackFunction: undefined
};

TagSelection.GetTagSelectionModelString = function () {
	return $('#TagSelectionModel').val();
};

TagSelection.GetTagSelectionModel = function () {
	var strModel = TagSelection.GetTagSelectionModelString();
	if (strModel === undefined)
		return undefined;
	var model = JSON.parse(strModel);
	return model;
};

TagSelection.SetTagSelectionModelString = function (modelStr) {
	$('#TagSelectionModel').val(modelStr);
};

TagSelection.SetTagSelectionModel = function (model) {

	var modelStr = JSON.stringify(model);
	TagSelection.SetTagSelectionModelString(modelStr);
};

TagSelection.GetModelStringWithValuesPopulated = function()
{
	var pointDropDownListItem = $('#PointDropDownList option:selected');
	var valueTypeDropDownListItem = $('#ValueTypeDropDownList option:selected');

	var pointTemplateTypeGuidString = $('#PointTemplateTypeDropDownList').val();
	var pointTemplateGuidString = $('#PointTemplateDropDownList').val();
	var pointCategoryGuidString = $('#PointCategoryDropDownList').val();
	var pointGuidString = pointDropDownListItem.val();
	var valueType = valueTypeDropDownListItem.text();

	var model = TagSelection.GetTagSelectionModel();
	model.PointTemplateTypeGuid = pointTemplateTypeGuidString;
	model.PointTemplateGuid = pointTemplateGuidString;
	model.PointCategoryGuid = pointCategoryGuidString;
	model.PointGuid = pointGuidString;
	model.ValueType = valueType;

	var modelStr = JSON.stringify(model);
	return modelStr;
}

TagSelection.SelectionPointTemplateTypeDropDownListChanged = function()
{
	var url = $( '#urlPointTemplateTypeSelectionChanged' ).val();
	var token = $( 'input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var pointTemplateTypeGuidString = $('#PointTemplateTypeDropDownList').val();
	var modelString = TagSelection.GetModelStringWithValuesPopulated(); //TagSelection.GetTagSelectionModelString();

	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: TagSelection.stack_bottomright_AddPoinTtag };
	// remove previous notifications
	PNotify.removeStack( TagSelection.stack_bottomright_AddPoinTtag );

	$.ajax( {
		url: url,
		cache: false,
		type: 'POST',
		headers: headers,
		dataType: 'json',
		contentType: 'application/json; charset=utf-8',
		data: JSON.stringify({ 'pointTemplateTypeGuidString': pointTemplateTypeGuidString, 'modelString': modelString }),
		success: function( response )
		{
				FMErrorAndExceptionHandling.HandleMessages( response, function( model )
				{
					TagSelection.SetTagSelectionModel( model );
					var templates = model.PointTemplateList;

					var items = '';

					$.each( templates, function( i, template )
					{
						items += '<option value=\'' + template.Key + '\'>' + template.Value + '</option>';
					} );

					$( '#PointTemplateDropDownList' ).empty().html( items );
					TagSelection.SelectionPointTemplateDropDownListChanged();
				}, messageAttributes );
		},
		error: function( request, status, error )
		{
				FMErrorAndExceptionHandling.ShowError( 'SelectionPointTemplateTypeDropDownListChanged failure', null, messageAttributes );
		}
	} );
};
TagSelection.SelectionPointTemplateDropDownListChanged = function()
{
	var url = $( '#urlPointTemplateSelectionChanged' ).val();
	var token = $( 'input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var pointTemplateGuidString = $( '#PointTemplateDropDownList' ).val();
	var modelString = TagSelection.GetModelStringWithValuesPopulated(); //TagSelection.GetTagSelectionModelString();

	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: TagSelection.stack_bottomright_AddPoinTtag };
	// remove previous notifications
	PNotify.removeStack( TagSelection.stack_bottomright_AddPoinTtag );

	$.ajax( {
		url: url,
		cache: false,
		type: 'POST',
		headers: headers,
		dataType: 'json',
		contentType: 'application/json; charset=utf-8',
		data: JSON.stringify({ 'pointTemplateGuidString': pointTemplateGuidString, 'modelString': modelString }),
		success: function( response )
		{
				FMErrorAndExceptionHandling.HandleMessages( response, function( model )
				{
					TagSelection.SetTagSelectionModel( model );

					var points = model.PointList;

					var items = '';

					$.each( points, function( i, point )
					{
						items += '<option value=\'' + point.Key + '\'>' + point.Value + '</option>';
					} );

					$( '#PointDropDownList' ).empty().html( items );
					TagSelection.SelectionPointOrValueTypeDropDownListChanged();
				}, messageAttributes );
		},
		error: function( request, status, error )
		{
				FMErrorAndExceptionHandling.ShowError( 'SelectionPointTemplateDropDownListChanged failure', null, messageAttributes );
		}
	} );
};
TagSelection.SelectionPointCategoryDropDownListChanged = function()
{
	var url = $( '#urlPointCategorySelectionChanged' ).val();
	var token = $( 'input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var pointCategoryGuidString = $( '#PointCategoryDropDownList' ).val();
	var modelString = TagSelection.GetModelStringWithValuesPopulated(); //TagSelection.GetTagSelectionModelString();

	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: TagSelection.stack_bottomright_AddPoinTtag };
	// remove previous notifications
	PNotify.removeStack( TagSelection.stack_bottomright_AddPoinTtag );

	$.ajax( {
		url: url,
		cache: false,
		type: 'POST',
		headers: headers,
		dataType: 'json',
		contentType: 'application/json; charset=utf-8',
		data: JSON.stringify({ 'pointCategoryGuidString': pointCategoryGuidString, 'modelString': modelString}),
		success: function( response )
		{
				FMErrorAndExceptionHandling.HandleMessages( response, function( model )
				{
					TagSelection.SetTagSelectionModel( model );

					var points = model.PointList;

					var items = '';

					$.each( points, function( i, point )
					{
						items += '<option value=\'' + point.Key + '\'>' + point.Value + '</option>';
					} );

					$( '#PointDropDownList' ).empty().html( items );
					TagSelection.SelectionPointOrValueTypeDropDownListChanged();
				}, messageAttributes );
		},
		error: function( request, status, error )
		{
				FMErrorAndExceptionHandling.ShowError( 'SelectionPointCategoryDropDownListChanged failure', null, messageAttributes );
		}
	} );
};

TagSelection.SelectionPointOrValueTypeDropDownListChanged = function()
{
	var url = $( '#urlPointOrValueTypeSelectionChanged' ).val();
	var token = $( 'input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var pointDropDownListItem = $( '#PointDropDownList option:selected' );
	var pointGuidString = pointDropDownListItem.val();
	var valueTypeDropDownListItem = $('#ValueTypeDropDownList option:selected');
	var valueType = valueTypeDropDownListItem.text();
	var modelString = TagSelection.GetModelStringWithValuesPopulated(); //TagSelection.GetTagSelectionModelString();

	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: TagSelection.stack_bottomright_AddPoinTtag };
	// remove previous notifications
	PNotify.removeStack( TagSelection.stack_bottomright_AddPoinTtag );

	$.ajax( {
		url: url,
		cache: false,
		type: 'POST',
		headers: headers,
		dataType: 'json',
		contentType: 'application/json; charset=utf-8',
		data: JSON.stringify({ 'pointGuidString': pointGuidString, 'valueType': valueType, 'modelString': modelString }),
		success: function( response )
		{
				FMErrorAndExceptionHandling.HandleMessages( response, function( model )
				{
					TagSelection.SetTagSelectionModel( model );

					var values = model.ValueList;
					var selectedValues = model.SelectedValues;

					var items = '';

					for (var i = 0; i < values.length; i++) {
						var isSelected = false;

						for (var j = 0; j < selectedValues.length; j++) {

							if (selectedValues[j].Key === values[i].Key) {
								isSelected = true;
								break;
							}
						}

						if (isSelected) {
							items += '<option value=\'' + values[i].Key + '\' selected=\'true\'>' + values[i].Value + '</option>';
						}
						else{
							items += '<option value=\'' + values[i].Key + '\'>' + values[i].Value + '</option>';
						}
					}

					$( '#ValueDropDownList' ).html( '' ).select2( { data: [{ id: '', text: '' }] } );
					$( '#ValueDropDownList' ).html( items ).select2();
				}, messageAttributes );
		},
		error: function( request, status, error )
		{
				FMErrorAndExceptionHandling.ShowError( 'SelectionPointOrValueTypeDropDownListChanged failure', null, messageAttributes );
		}
	} );
};

TagSelection.PointTemplateCheckBoxOrValueTypeDropDownListChanged = function ()
{
    var url = $('#urlPointTemplateCheckBoxOrValueTypeSelectionChanged').val();
    var token = $('input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;
    var pointTemplateGuidString = FMDrawIndex.GetDiagramModelDataValue( 'PointTemplateGuid' );
    var valueTypeDropDownListItem = $('#ValueTypeDropDownList option:selected');
    var valueType = valueTypeDropDownListItem.text();
    var modelString = TagSelection.GetModelStringWithValuesPopulated(); //TagSelection.GetTagSelectionModelString();

    // notification position
    var messageAttributes = { addclass: 'stack-bottomright', stack: TagSelection.stack_bottomright_AddPoinTtag };
    // remove previous notifications
    PNotify.removeStack(TagSelection.stack_bottomright_AddPoinTtag);

    $.ajax({
        url: url,
        cache: false,
        type: 'POST',
        headers: headers,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ 'pointTemplateGuidString': pointTemplateGuidString, 'valueType': valueType, 'modelString': modelString }),
        success: function (response) {
            FMErrorAndExceptionHandling.HandleMessages(response, function (model) {
                TagSelection.SetTagSelectionModel(model);

                var values = model.ValueList;
                var selectedValues = model.SelectedValues;

                var items = '';

                for (var i = 0; i < values.length; i++) {
                    var isSelected = false;

                    for (var j = 0; j < selectedValues.length; j++) {

                        if (selectedValues[j].Key === values[i].Key) {
                            isSelected = true;
                            break;
                        }
                    }

                    if (isSelected) {
                        items += '<option value=\'' + values[i].Key + '\' selected=\'true\'>' + values[i].Value + '</option>';
                    }
                    else {
                        items += '<option value=\'' + values[i].Key + '\'>' + values[i].Value + '</option>';
                    }
                }

                $('#ValueDropDownList').html('').select2({ data: [{ id: '', text: '' }] });
                $('#ValueDropDownList').html(items).select2();
            }, messageAttributes);
        },
        error: function (request, status, error) {
            FMErrorAndExceptionHandling.ShowError('PointTemplateCheckBoxOrValueTypeDropDownListChanged failure', null, messageAttributes);
        }
    } );
};



TagSelection.PointTagSelectionCancelButtonPressAction = function ()
{
    // we need to call this to clear the modal flag
    TagSelection.TagSelectionOKCallBackFunction("Cancel");
}

function isEmpty( val )
{
	return ( val === undefined || val == null || val.length <= 0 ) ? true : false;
}

TagSelection.PointTagSelectionOkButtonPressAction = function()
{
	var url = $( '#urlOkButtonPress' ).val();
	var token = $( 'input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var pointDropDownList = $( '#PointDropDownList option:selected' );
	var pointGuidString = pointDropDownList.val();
	var enableTagSelection = $('#EnableTagSelection').val();
	var trendButton = $('#PointTrendButton').val();
    // trendButton is set to true when trend or detail mode is selected we need to process this under this circumstance regardless
    // of tagselection which should only apply when command is selected
	if (enableTagSelection === 'True' || trendButton === "True")
	{
		var valueDropDownList = $( '#ValueDropDownList' );
		var valueIdentifierString = valueDropDownList.val();
		var valueFieldDropDownList = $( '#ValueFieldDropDownList' );
		var valueFieldListString = valueFieldDropDownList.val();

		if ( isEmpty( valueIdentifierString ) )
		{
			TagSelection.TagSelectionOKCallBackFunction( null );
		}
	}
	var modelString = TagSelection.GetModelStringWithValuesPopulated(); //TagSelection.GetTagSelectionModelString();

	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: TagSelection.stack_bottomright_AddPoinTtag };
	// remove previous notifications
	PNotify.removeStack( TagSelection.stack_bottomright_AddPoinTtag );

	$.ajax( {
		url: url,
		cache: false,
		type: 'POST',
		headers: headers,
		dataType: 'json',
		async: false,
		contentType: 'application/json; charset=utf-8',
		data: JSON.stringify({ 'pointGuidString': pointGuidString, 'pointValueIdentifierStringList': valueIdentifierString, 'pointTagFieldList': valueFieldListString != null ? valueFieldListString.join(): "", 'modelString': modelString }),
		success: function( response )
		{
				FMErrorAndExceptionHandling.HandleMessages( response, function( pointTags, inError )
				{
					if ( inError )
					{
						return;
					}
					TagSelection.TagSelectionOKCallBackFunction(pointTags);

					if (TagSelection.TagSelectionSaveCallBackFunction)
					{
						TagSelection.TagSelectionSaveCallBackFunction(pointTags);
					}

					if ( typeof pointTags === 'string' )
					{
						$( '#addPointTagsError' ).html( pointTags );
						return;
					}
				}, messageAttributes );
		},
		error: function( request, status, error )
		{
				FMErrorAndExceptionHandling.ShowError( 'Error getting tags', null, messageAttributes );
		}
	} );
};

TagSelection.UpdateTagSelectionDialogBasedOnPointTemplateCheckBox = function()
{
    var checked = document.getElementById('TSPointTemplateCheckBox').checked;
    var model = TagSelection.GetTagSelectionModel();
    var isPointTrendButton = (model) ? model.PointTrendButton : false;
    TagSelection.UpdatePointTemplateTagSelectionIndicator(checked);
    if ( !checked )
    {
        //Hide Point Type Selection
        $('#TSPointTypeLabel').show();
        $('#PointTemplateTypeDropDownListDiv').show();
        //Hide Point Template Selection
        $('#TSPointTemplateLabel').show();
        $('#PointTemplateDropDownListDiv').show();
        //Hide Point Category Selection
        $('#TSPointCategoryLabel').show();
        $('#PointCategoryDropDownListDiv').show();
        //Hide Point Point Selection
        $('#TSPointLabel').show();
        $('#PointDropDownListDiv').show();
        $('#PointTagSelectionScreen').text($('#DialogTitleForPointSelect').val());
        TagSelection.SelectionPointOrValueTypeDropDownListChanged();
    }
    else
    {
        //Hide Point Type Selection
        $( '#TSPointTypeLabel' ).hide();
        $( '#PointTemplateTypeDropDownListDiv' ).hide();
        //Hide Point Template Selection
        if ( !isPointTrendButton )
        {
            $( '#TSPointTemplateLabel' ).hide();
            $( '#PointTemplateDropDownListDiv' ).hide();
        }
        else
        {
            $('#TSPointTemplateLabel').show();
            $('#PointTemplateDropDownListDiv').show();
        }
        //Hide Point Category Selection
        $('#TSPointCategoryLabel').hide();
        $('#PointCategoryDropDownListDiv').hide();
        //Hide Point Point Selection
        $('#TSPointLabel').hide();
        $('#PointDropDownListDiv').hide();
        $('#PointTagSelectionScreen').text($('#DialogTitleForPointTemplateSelect').val());
        $('#TSIsPointDetailObject').val('true');
        TagSelection.PointTemplateCheckBoxOrValueTypeDropDownListChanged();
    }
   

    //Need to get this working to resolve issue where dialog gets cut off at bottom.
    //refreshDialogPosition( 'PointTagSelectScreen' );
};

TagSelection.UpdatePointTemplateTagSelectionIndicator = function(value)
{
    var model = TagSelection.GetTagSelectionModel();
    model.PointTemplateTagSelectionIndicator = value;
    $('#TSPointDetailObject').val(value ? 'true' : 'false');
    TagSelection.SetTagSelectionModel(model);
}

function refreshDialogPosition(id) {
    $("#" + id).position({
        my: "center",
        at: "center",
        of: window,
        collision: "fit",
        // Ensure the titlebar is always visible
        using: function (pos) {
            var topOffset = $(this).css(pos).offset().top;
            if (topOffset < 0) {
                $(this).css("top", pos.top - topOffset);
            }
        }
    });
}

TagSelection.Init = function()
{
    $.fn.modal.Constructor.prototype.enforceFocus = function()
    {
    };

    $('#PointCategoryDropDownList').select2();
    $('#PointDropDownList').select2();
    $('#PointTemplateDropDownList').select2();
    $('#PointTemplateTypeDropDownList').select2();
    $('#ValueTypeDropDownList').select2();
    $( '#ValueDropDownList' ).select2();
    $( '#ValueFieldDropDownList' ).select2();

    $( '#PointTemplateTypeDropDownList' ).change( function( e )
    {
        TagSelection.SelectionPointTemplateTypeDropDownListChanged();
    } );

    $( '#PointTemplateDropDownList' ).change( function( e )
    {
        TagSelection.SelectionPointTemplateDropDownListChanged();
    } );

    $( '#PointCategoryDropDownList' ).change( function( e )
    {
        TagSelection.SelectionPointCategoryDropDownListChanged();
    } );

    $( '#PointDropDownList' ).change( function( e )
    {
        TagSelection.SelectionPointOrValueTypeDropDownListChanged();
    } );

    $('#ValueTypeDropDownList').change(function(e)
    {
        var checked = document.getElementById('TSPointTemplateCheckBox').checked;
        if ( checked )
        {
            TagSelection.PointTemplateCheckBoxOrValueTypeDropDownListChanged();
        }
        else
        {
            TagSelection.SelectionPointOrValueTypeDropDownListChanged();
        }
    } );

    if (FMDrawIndex.GetDiagramModelDataValue('PanelType') === 'Detail' 
        && $('#TSPointDetailObject').val() === 'true')
    {
        $('#TSPointTemplateCheckBox').prop('checked', true);
        TagSelection.UpdatePointTemplateTagSelectionIndicator(true);
    }
    else
    {
        $('#TSPointTemplateCheckBox').prop('checked', false);
        TagSelection.UpdatePointTemplateTagSelectionIndicator( false );
    }

    TagSelection.UpdateTagSelectionDialogBasedOnPointTemplateCheckBox();

    $( '#TSPointTemplateCheckBox' ).change( function( e )
    {
        TagSelection.UpdateTagSelectionDialogBasedOnPointTemplateCheckBox();
    } );

    var allowPointSelect = $( '#AllowPointSelect' ).val();
    if ( allowPointSelect === 'false' )
    {
	    $( '#PointTemplateTypeDropDownListDiv' ).hide();
	    $( '#PointTemplateDropDownListDiv' ).hide();
	    $( '#PointCategoryDropDownListDiv' ).hide();
	    $( '#PointDropDownListDiv' ).hide();
    }
		if ($(window).height() <= 600) {
			$('#TagSelectionScrollDiv').addClass('tag-selection-scroll');
		}
};

TagSelection.CreateParameterDataForOpenTagDialog = function (showFields, modelParam, panelTypeStrParam, pointTemplateGuidStrParam, isPointDetailDrawing, dataTypeStr, fieldFilter) {
	var modelStrParam = (modelParam) ? JSON.stringify(modelParam) : '';
	if (isPointDetailDrawing) {
		return {
			showFields: showFields,
			modelStr: modelStrParam,
			panelTypeStr: panelTypeStrParam,
			pointTemplateGuidStr: pointTemplateGuidStrParam,
			dataTypeStr: dataTypeStr,
			fieldFilter: fieldFilter
		}
	}
	else {
		return {
			showFields: showFields,
			modelStr: modelStrParam,
			dataTypeStr: dataTypeStr,
			fieldFilter: fieldFilter
		}
	}
}

TagSelection.OpenTagDialogByDataType = function (showFields, selectionFinishCallBackFunction, tagSelectionModelSaveCallBackFunction, tagSelectionModelGetLastCallBackFunction, openFinishCallBackFunction, urlTagSelectionGetPointListWithPanelTemplateContextByDataType, urlTagSelectionGetPointListByDataType, pointTemplateGuidStr, panelType, sectionId, dataTypeStr, fieldFilter) {
	TagSelection.TagSelectionOKCallBackFunction = selectionFinishCallBackFunction;
	TagSelection.TagSelectionSaveCallBackFunction = tagSelectionModelSaveCallBackFunction;
	var isPointDetail = panelType === "Detail" ? true : false;
	var url = (isPointDetail) ? urlTagSelectionGetPointListWithPanelTemplateContextByDataType : urlTagSelectionGetPointListByDataType;
	// hide any other notification
	FMErrorAndExceptionHandling.CloseNotifications();

	$('body').modalmanager('loading');
	var model = tagSelectionModelGetLastCallBackFunction();

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var params = TagSelection.CreateParameterDataForOpenTagDialog(showFields, model, panelType, pointTemplateGuidStr, isPointDetail, dataTypeStr, fieldFilter);

	$.ajax({
		type: 'POST',
		url: url,
		headers: headers,
		cache: false,
		data: params,
		success: function (response) {
		    FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					// replace the holder with the partial view
					$(sectionId).html(data);
					openFinishCallBackFunction(true);
				}
				else {
					// remove the loading of the modal
					var modalManager = $('body').data('modalmanager');
					modalManager.removeLoading();
					openFinishCallBackFunction(false);
				}
			});
		},
		error: function (xhr, textStatus, error) {
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () {
					// remove the loading of the modal
					var modalManager = $('body').data('modalmanager');
					modalManager.removeLoading();
					openFinishCallBackFunction(false);
				});
		}
	});
};
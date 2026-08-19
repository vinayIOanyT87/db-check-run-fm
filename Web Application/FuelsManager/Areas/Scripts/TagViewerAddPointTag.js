
var FMAddPointTags = function()
{
	var _stack_bottomright_AddPoinTtag = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $( '#AddPointTagsPartial' ) };

	var _selectionPointTemplateTypeDropDownListChanged = function()
	{
		var url = $( '#urlPointTemplateTypeSelectionChanged' ).val();
		var token = $( '#AddPointTagsForm input[name=__RequestVerificationToken]' ).val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;
		var pointTemplateTypeGuidString = $( '#PointTemplateTypeDropDownList' ).val();

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMAddPointTags.stack_bottomright_AddPoinTtag };
		// remove previous notifications
		PNotify.removeStack( FMAddPointTags.stack_bottomright_AddPoinTtag );

		$.ajax( {
				url: url,
				cache: false,
				type: 'GET',
				headers: headers,
				data: 'pointTemplateTypeGuidString=' + pointTemplateTypeGuidString,
				success: function( response )
				{
					FMErrorAndExceptionHandling.HandleMessages( response, function( templates, inError )
					{
						if ( inError )
						{
								$( '#PointTemplateDropDownList' ).empty();
								return;
						}

						var items = '';

						$.each( templates, function( i, template )
						{
								items += '<option value=\'' + template.Key + '\'>' + template.Value + '</option>';
						} );

						$( '#PointTemplateDropDownList' ).empty().html( items );
						FMAddPointTags.selectionPointTemplateDropDownListChanged();
					}, messageAttributes );
				},
				error: function( request, status, error )
				{
					FMErrorAndExceptionHandling.ShowError( 'SelectionPointTemplateTypeDropDownListChanged failure' );
				}
		} );
	};

	var _selectionPointTemplateDropDownListChanged = function()
	{
		var url = $( '#urlPointTemplateSelectionChanged' ).val();
		var token = $( '#AddPointTagsForm input[name=__RequestVerificationToken]' ).val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;
		var pointTemplateGuidString = $( '#PointTemplateDropDownList' ).val();

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMAddPointTags.stack_bottomright_AddPoinTtag };
		// remove previous notifications
		PNotify.removeStack( FMAddPointTags.stack_bottomright_AddPoinTtag );

		$.ajax( {
				url: url,
				cache: false,
				type: 'GET',
				headers: headers,
				data: 'pointTemplateGuidString=' + pointTemplateGuidString,
				success: function( response )
				{
					FMErrorAndExceptionHandling.HandleMessages( response, function( points, inError )
					{
						if ( inError )
						{
								$( '#PointDropDownList' ).empty();
								return;
						}

						var items = '';
						$.each( points, function( i, point )
						{
								items += '<option value=\'' + point.Key + '\'>' + point.Value + '</option>';
						} );

						$( '#PointDropDownList' ).empty().html( items );
						FMAddPointTags.selectionPointDropDownListChanged();
					}, messageAttributes );
				},
				error: function( request, status, error )
				{
					FMErrorAndExceptionHandling.ShowError( 'SelectionPointTemplateDropDownListChanged failure' );
				}
		} );
	};

	var _selectionPointCategoryDropDownListChanged = function()
	{
		var url = $( '#urlPointCategorySelectionChanged' ).val();
		var token = $( '#AddPointTagsForm input[name=__RequestVerificationToken]' ).val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;
		var pointCategoryGuidString = $( '#PointCategoryDropDownList' ).val();

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMAddPointTags.stack_bottomright_AddPoinTtag };
		// remove previous notifications
		PNotify.removeStack( FMAddPointTags.stack_bottomright_AddPoinTtag );

		$.ajax( {
				url: url,
				cache: false,
				type: 'GET',
				headers: headers,
				data: 'pointCategoryGuidString=' + pointCategoryGuidString,
				success: function( response )
				{
					FMErrorAndExceptionHandling.HandleMessages( response, function( points, inError )
					{
						if ( inError )
						{
								$( '#PointDropDownList' ).empty();
								return;
						}

						var items = '';

						$.each( points, function( i, point )
						{
								items += '<option value=\'' + point.Key + '\'>' + point.Value + '</option>';
						} );

						$( '#PointDropDownList' ).empty().html( items );
						FMAddPointTags.selectionPointDropDownListChanged();
					}, messageAttributes );
				},
				error: function( request, status, error )
				{
					FMErrorAndExceptionHandling.ShowError( 'SelectionPointCategoryDropDownListChanged failure' );
				}
		} );
	};

	var _selectionPointDropDownListChanged = function()
	{
		var url = $( '#urlPointSelectionChanged' ).val();
		var token = $( '#AddPointTagsForm input[name=__RequestVerificationToken]' ).val();
		var headers = {};
		headers['__RequestVerificationToken'] = token;
		var pointDropDownList = $( '#PointDropDownList option:selected' );
		var pointGuidString = pointDropDownList.val();
		var pointID = pointDropDownList.text();

		// notification position
		var messageAttributes = { addclass: 'stack-bottomright', stack: FMAddPointTags.stack_bottomright_AddPoinTtag };
		// remove previous notifications
		PNotify.removeStack( FMAddPointTags.stack_bottomright_AddPoinTtag );

		$.ajax( {
				url: url,
				cache: false,
				type: 'GET',
				headers: headers,
				data: 'pointGuidString=' + pointGuidString + '&pointID=' + pointID,
				success: function( response )
				{
					FMErrorAndExceptionHandling.HandleMessages( response, function( pointTags, inError )
					{
						if ( inError )
						{
								$( '#PointTagDropDownList' ).empty();
								return;
						}

						var items = '';

						$.each( pointTags, function( i, pointTag )
						{
								items += '<option value=\'' + pointTag.Key + '\'>' + pointTag.Value + '</option>';
						} );

						$( '#PointTagDropDownList' ).empty().html( items );
					}, messageAttributes );
				},
				error: function( request, status, error )
				{
					FMErrorAndExceptionHandling.ShowError( 'SelectionPointDropDownListChanged failure' );
				}
		} );
	};

	var _selectionPointTagDropDownListChanged = function () {
		var values = $('#PointTagDropDownList').val();
		if (values != null && values.length > 1 && values[0] === '00000000-0000-0000-0000-000000000000') {
			$('#PointTagDropDownList').val('00000000-0000-0000-0000-000000000000').trigger('change');
		}
	}

	return {
		stack_bottomright_AddPoinTtag: _stack_bottomright_AddPoinTtag,
		selectionPointTemplateTypeDropDownListChanged: _selectionPointTemplateTypeDropDownListChanged,
		selectionPointTemplateDropDownListChanged: _selectionPointTemplateDropDownListChanged,
		selectionPointCategoryDropDownListChanged: _selectionPointCategoryDropDownListChanged,
		selectionPointDropDownListChanged: _selectionPointDropDownListChanged,
		selectionPointTagDropDownListChanged: _selectionPointTagDropDownListChanged
};
}();
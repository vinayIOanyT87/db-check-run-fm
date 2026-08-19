var AnimationManager = AnimationManager || {
	AnimationTestPropertyRowPrefix: "AnimationTestPropertiesRowId_",
	AnimationTestPropertyTableRowTdPrefix: "AnimationTestPropertiesRowTdId_",
	AnimationTestExpandPrefix: "AnimationTestExpandId_",
	AnimationTestDeletePrefix: "AnimationTestDelId_",
	AnimationPropertyDeletePrefix: "AnimationTestPropertyDelId_",
	AnimationTestGroupGuidForPointValueSelection: null,
	AnimationEmptyGuid: '00000000-0000-0000-0000-000000000000',
	AnimationTestGroupsTablePrefix: "AnimationTestGroupsTableId_",
	AnimationTestTableRowPrefix: "AnimationTestTableRowId_",
	AnimationTestDefinitionPrefix: "AnimationTestDefinitionId_",
	AnimationTestPropertyTablePrefix: "AnimationTestPropertyTableId_",
	TagSelectionMetaData: null,
	AnimationTestAddPrefix: "AnimationTestAddId_",
	CurrentFocusTransparencyTextboxValue: "",
	AnimationInEditMode: false,
	LastSectionId: "",
	ComparisonOperatorList: null,
	BitwiseOperatorList: null,
	AnimationTestRowEditComparisonOperatorDropdownId: "TestRowEditComparisonOperatorDropdown",
	AnimationTestRowEditCompareValueTextboxId: "TestRowEditCompareValueTextbox",
	AnimationTestRowEditBitwiseDropdownId: "TestRowEditBitwiseDropdown",
	AnimationTestRowEditBitMaskTextboxId: "TestRowEditBitMaskTextbox",
	AnimationTestEditHolderRowId: "AnimationTestEditHolderRow",
	AnimationTestEditRowId: "AnimationTestEditRow",
	AnimationTestRowEditDataTypeLabelId: "TestRowEditDataTypeLabel",
	AnimationTestRowEditOkButtonId: "TestRowEditOkButton",
	AnimationTestRowEditCancelButtonId: "TestRowEditCancelButton",
	AnimationTestEditTableId: "TestEditTable",
	AnimationTestGroupsTable: "AnimationTestGroupsTable",
	AnimationTestGroupRowPrefix: "AnimationTestGroupRow_",
	AnimationTestGroupExpandPrefix: "AnimationTestGroupExpand_",
	AnimationTestGroupIdIdPrefix: "AnimationTestGroupIdId_",
	AnimationTestGroupDataTypeIdPrefix: "AnimationTestGroupDataTypeId_",
	AnimationTestGroupDataTypeDropDownIdPrefix: "AnimationTestGroupDataTypeDropDownId_",
	AnimationTestGroupAddIdPrefix: "AnimationTestGroupAddId_",
	AnimationTestGroupDelIdPrefix: "AnimationTestGroupDelId_",
	AnimationTestGroupPointValueIdPrefix: "AnimationTestGroupPointValueId_",
	AnimationTestRowPrefix: "AnimationTestRow_",
	AnimationTestRowTdPrefix: "AnimationTestRowTd_",
	AnimationTestTableIdPrefix: "AnimationTestTableId_",
	AnimationTestPropertyTransparencyPrefix1: "AnimationTestPropertyTransparency1Id_",
	AnimationTestPropertyTransparencyPrefix2: "AnimationTestPropertyTransparency2Id_",
	AnimationTestPropertyTextBoxPrefix: "AnimationTestPropertyTextBoxId_",
	AnimationTestPropertyValuePrefix: "AnimationTestPropertyValueId_",
	AnimationTestPropertyVisibityPrefix: "AnimationTestPropertyVisibityId_",
	AnimationTestPropertyTableRowPrefix: "AnimationTestPropertyTableRowId_",
	AnimationTemporaryRowPrefix: "TemporaryRowId_",
	AnimationTestPropertyNamePrefix: "AnimationTestPropertyNameId_",
	EnterTestGroupNamePrefix: "EnterTestGroupName_",

	animationTestPreviewColPrefix: "AnimationTestPreviewColId_",
	animationTestPreviewHdBkColorPrefix: "AnimationTestPreviewHdBkColorId_",
	animationTestPreviewHdLnColorPrefix: "AnimationTestPreviewHdLnColorId_",
	animationTestPreviewHdFlColorPrefix: "AnimationTestPreviewHdFlColorId_",
	animationTestPreviewHdTxColorPrefix: "AnimationTestPreviewHdTxColorId_",
	animationTestPreviewHdVsColorPrefix: "AnimationTestPreviewHdVsColorId_",
	animationTestPreviewHdTextColorPrefix: "AnimationTestPreviewHdTextColorId_",
	animationTestPreviewBkColorPrefix: "AnimationTestPreviewBkColorId_",
	animationTestPreviewLnColorPrefix: "AnimationTestPreviewLnColorId_",
	animationTestPreviewFlColorPrefix: "AnimationTestPreviewFlColorId_",
	animationTestPreviewTxColorPrefix: "AnimationTestPreviewTxColorId_",
	animationTestPreviewVsColorPrefix: "AnimationTestPreviewVsColorId_",
	animationTestPreviewTextColorPrefix: "AnimationTestPreviewTextColorId_",
	animationTestPreviewBkDivPrefix1: "AnimationTestPreviewBkDivId1_",
	animationTestPreviewBkDivPrefix2: "AnimationTestPreviewBkDivId2_",
	animationTestPreviewLnDivPrefix1: "AnimationTestPreviewLnDivId1_",
	animationTestPreviewLnDivPrefix2: "AnimationTestPreviewLnDivId2_",
	animationTestPreviewFlDivPrefix1: "AnimationTestPreviewFlDivId1_",
	animationTestPreviewFlDivPrefix2: "AnimationTestPreviewFlDivId2_",
	animationTestPreviewTxDivPrefix1: "AnimationTestPreviewTxDivId1_",
	animationTestPreviewTxDivPrefix2: "AnimationTestPreviewTxDivId2_",
	animationTestPreviewVsLblPrefix: "AnimationTestPreviewVsLblId_",
	animationTestPreviewTxLblPrefix: "AnimationTestPreviewTxLblId_",
	animationTestPreviewTablePrefix: "AnimationTestPreviewTableId_",
	animationManagerOkButtonId: "AnimationManagerOkButton",
	inEditTestGroupNameMode: false,
	animationDeleteEnabled: true,
	AnimationDeactivatTextEditingTools: function(){},
	AnimationEmptyAnimation: "EMPTY",
	AnimationUnassigned: $("#AnimationMgrUnassignedTranslated").val(),
	AnimationNewModalScreenShowing: false,
	AnimationCopyAndRenameModalScreenShowing: false,
	AnimationPointValueModalScreenShowing: false,
	enablePointSelect: true,
	AnimationOriginalCurrentAnimation: null
};

if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	debugger;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

AnimationManager.CreateTagSelectionMetaDataObject = function( tagSelectionModelSaveCallBackFunction, tagSelectionModelGetLastCallBackFunction, openFinishCallBackFunction, urlTagSelectionGetPointListWithPanelTemplateContextByDataType, urlTagSelectionGetPointListByDataType, pointTemplateGuidStr, panelType, sectionId )
{
	var tagSelectionMetaData = {};
	tagSelectionMetaData.tagSelectionModelSaveCallBackFunction = tagSelectionModelSaveCallBackFunction;
	tagSelectionMetaData.tagSelectionModelGetLastCallBackFunction = tagSelectionModelGetLastCallBackFunction;
	tagSelectionMetaData.openFinishCallBackFunction = openFinishCallBackFunction;
	tagSelectionMetaData.urlTagSelectionGetPointListWithPanelTemplateContextByDataType = urlTagSelectionGetPointListWithPanelTemplateContextByDataType;
	tagSelectionMetaData.urlTagSelectionGetPointListByDataType = urlTagSelectionGetPointListByDataType;
	tagSelectionMetaData.pointTemplateGuidStr = pointTemplateGuidStr;
	tagSelectionMetaData.panelType = panelType;
	tagSelectionMetaData.sectionId = sectionId;
	return tagSelectionMetaData;
};

AnimationManager.Init = function()
{
	AnimationManager.inEditTestGroupNameMode = false;
	AnimationManager.ClearEditMode();
	$( '#AnimationManagerOutterDiv' ).niceScroll( {
		cursorwidth: '10px',
		autohidemode: false,
		cursorcolor: '#486899',
		background: '#f9f9f9',
		railoffset: true,
		railpadding: { top: 0, right: 0, left: -5, bottom: 0 },
		smoothscroll: true
	} );

	AnimationManager.CreateSpectrum( null );
	AnimationManager.UpdateDataTypeEditForAll();
	AnimationManager.EnableDisableAnimationDelete();
	AnimationManager.AnimationNewModalScreenShowing = false;
	AnimationManager.AnimationCopyAndRenameModalScreenShowing = false;
	AnimationManager.AnimationPointValueModalScreenShowing = false;
	AnimationManager.SetOriginalAnimationString();
	AnimationManager.ApplyAlternatingRowColors();
};

AnimationManager.AnimationModified = function()
{
	var model = AnimationManager.GetAnimationManagerModel();
	if ( model )
	{
		var animation = AnimationManager.GetCurrentAnimation(model);
		if ( animation )
		{
			return (AnimationManager.AnimationOriginalCurrentAnimation !== JSON.stringify(animation));
		}
	}
	return false;
};


AnimationManager.AnimationModifiedGetGuid = function ()
{
	var model = AnimationManager.GetAnimationManagerModel();
	if (model)
	{
		var animation = AnimationManager.GetCurrentAnimation(model);
		if (animation)
	    {
			return (animation.AnimationGuid);
		}
	}
	return false;
};

AnimationManager.SetOriginalAnimationString = function()
{
	var model = AnimationManager.GetAnimationManagerModel();
	if ( model )
	{
		var animation = AnimationManager.GetCurrentAnimation( model );
		if ( animation )
		{
			AnimationManager.AnimationOriginalCurrentAnimation = JSON.stringify( animation );
		}
		else
		{
			AnimationManager.AnimationOriginalCurrentAnimation = null;
		}
	}
	else
	{
		AnimationManager.AnimationOriginalCurrentAnimation = null;
	}
};

AnimationManager.RestoreAnimationToOriginal = function()
{
	var model = AnimationManager.GetAnimationManagerModel();
	if (model && model.SelectedAnimationGuid && model.SelectedAnimationGuid !== AnimationManager.AnimationEmptyGuid && AnimationManager.AnimationOriginalCurrentAnimation)
	{
		var animationGuid = model.SelectedAnimationGuid;
		var i = AnimationManager.GetIndexOfAnimationByAnimationGuid(model, animationGuid);
		if (i >= 0)
		{
			model.AnimationList[i] = JSON.parse( AnimationManager.AnimationOriginalCurrentAnimation );
		}
		AnimationManager.SetAnimationManagerModel( model );
	}
};

AnimationManager.UpdateDataTypeEditForAll = function()
{
	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );

	if ( animation )
	{
		for ( var i = 0; i < animation.AnimationTestGroups.length; i++ )
		{
			var testGroup = animation.AnimationTestGroups[i];
			AnimationManager.UpdateTestGroupDataTypeEditMode( model, testGroup );
		}

		// Initialize the test preview rows.
		AnimationManager.InitializeTestPreviews();
	}
};

//===============================================================
// This function will create the spectrum.
//===============================================================
AnimationManager.CreateSpectrum = function()
{
	$( ".full" ).spectrum( {
		color: tinycolor,
		showInput: true,
		className: "full-spectrum",
		containerClassName: 'FullContainerClass',
		showInitial: true,
		showPalette: true,
		showSelectionPalette: true,
		maxSelectionSize: 8,
		preferredFormat: "hex",
		cancelText: "Cancel",
		chooseText: "Ok",
		showAlpha: true,
		allowEmpty: true,
		clickoutFiresChange: false,
		flat: false,
		replacerClassName: 'spectrum-control',
		localStorageKey: "spectrum",
		move: function( color )
		{
		},
		show: function( color )
		{
			var parts = this.id.split( "|" );
			var animationTestPropertyColorButtonId = "AnimationTestPropertyColorButton" + parts[0] + "Id_" + parts[1];
			var rgbaStr = $( "#" + animationTestPropertyColorButtonId ).css( "background-color" );
			$( ".full" ).spectrum( "set", rgbaStr );
		},
		beforeShow: function( color )
		{
			var parts = this.id.split( "|" );
			var animationTestPropertyColorButtonId = "AnimationTestPropertyColorButton" + parts[0] + "Id_" + parts[1];
			var rgbaStr = $( "#" + animationTestPropertyColorButtonId ).css( "background-color" );
			$( ".full" ).spectrum( "set", rgbaStr );
		},
		hide: function( color )
		{
			var parts = this.id.split( "|" );
			var animationTestPropertyColorButtonId = "AnimationTestPropertyColorButton" + parts[0] + "Id_" + parts[1];
			var animationTestPropertyTransparencyId = "AnimationTestPropertyTransparency" + parts[0] + "Id_" + parts[1];

			var rgbastring = "transparent";
			$( "#" + animationTestPropertyTransparencyId ).val( "0" );

			if ( color != null )
			{
				rgbastring = "rgba(" + parseInt( color._r ) + "," + parseInt( color._g ) + "," + parseInt( color._b ) + "," + color._a + ")";
				$( "#" + animationTestPropertyTransparencyId ).val( AnimationManager.GetTransparencyPercent( rgbastring ) );
			}
			else
			{
                // if color is null then set transparency at 100 so we can track in operate
			    $("#" + animationTestPropertyTransparencyId).val(100);
			}

			$( "#" + animationTestPropertyColorButtonId ).css( "background-color", rgbastring );

			// This will trigger the update to the model.
			AnimationManager.HandleColorPickerChange( animationTestPropertyTransparencyId );
		},
		change: function( color )
		{
		},
		palette: [
			['#000000', '#FF0000', '#00FF00', '#FFFF00', '#0000FF', '#FF00FF', '#00FFFF', '#FFFFFF'],
			['#2D2D2D', '#D70000', '#00D700', '#D7D700', '#0000D7', '#D700D7', '#00D7D7', '#DCDCDC'],
			['#555555', '#AF0000', '#00AF00', '#AFAF00', '#0000AF', '#AF00AF', '#00AFAF', '#B9B9B9'],
			['#737373', '#640000', '#006400', '#646400', '#000064', '#640064', '#006464', '#969696'],
			['#FFC000', '#FF8200', '#FF5A2D', '#66400F', '#805236', '#EFEFC9', '#037392', '#B7E8FF'],
			['#0080FF', '#8080FF', '#FF80C0', '#800040', '#008080', '#004080', '#FF0080', '#000040'],
			['#0080C0', '#0000A0', '#8080C0', '#800080', '#008040', '#000080', '#8000FF', '#009FC5'],
			['#423A22', '#A28D68', '#AFAF61', '#5C5230', '#004000', '#599764', '#6F8938', '#35462B']
		]
	} );
};

AnimationManager.GetForm = function( url, finishFunction, sectionId, tagSelectionMetaData, deactivateTextEditingToolFunc, animationPointValueAssignments, enablePointSelect )
{
	AnimationManager.ClearEditMode();

	var pointValueListStr = JSON.stringify( animationPointValueAssignments );
	var callData = {
		pointValueListStr: pointValueListStr
	};

	if ( deactivateTextEditingToolFunc )
	{
		AnimationManager.AnimationDeactivatTextEditingTools = deactivateTextEditingToolFunc;
	}

	AnimationManager.enablePointSelect = enablePointSelect;
	FMErrorAndExceptionHandling.CloseNotifications();
	$( 'body' ).modalmanager( 'loading' );

	AnimationManager.LastSectionId = sectionId;
	AnimationManager.TagSelectionMetaData = tagSelectionMetaData;
	var token = $( 'input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	$.ajax( {
		type: 'Post',
		url: url,
		data: callData,
		headers: headers,
		cache: false,
		success: function( response )
		{
			var modalManager = $( 'body' ).data( 'modalmanager' );
			modalManager.removeLoading();
			FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
			{
				if ( !inError )
				{
					// replace the holder with the partial view
					$( AnimationManager.LastSectionId ).html( data );

					finishFunction( true );
				}
				else
				{
					finishFunction( false );
				}
			} );
		},
		error: function( xhr, textStatus, error )
		{
			var modalManager = $( 'body' ).data( 'modalmanager' );
			modalManager.removeLoading();
			FMErrorAndExceptionHandling.ShowException( xhr,
				textStatus,
				error,
				function()
				{
					// remove the loading of the modal
					finishFunction( false );
				} );
		}
	} );
};

AnimationManager.CollapseAnimation = function( currentItem )
{
	currentItem.innerHTML = '<img name="expand" draggable="false" title="collapsed" alt="collapsed" src="' + window.applicationRootName + '/fmwebapp/images/Arrow-rightBL.png">';
	$( '#AnimationTestGroupsRow' ).addClass( 'hidden' );
	currentItem.setAttribute( 'onClick', 'javascript: return AnimationManager.ExpandAnimation(this);' );
};

AnimationManager.ExpandAnimation = function( currentItem )
{
	currentItem.innerHTML = '<img name="collapse" draggable="false" title="expanded" alt="expanded" src="' + window.applicationRootName + '/fmwebapp/images/Arrow-downBl.png">';
	$( '#AnimationTestGroupsRow' ).removeClass( 'hidden' );
	currentItem.setAttribute( 'onClick', 'javascript: return AnimationManager.CollapseAnimation(this);' );
};

AnimationManager.CollapseTestGroup = function( currentItem )
{
	var animationTestGroupGuid = currentItem.id.replace( AnimationManager.AnimationTestGroupExpandPrefix, '' );
	currentItem.innerHTML = '<img name="expand" draggable="false" title="collapsed" alt="collapsed" src="' + window.applicationRootName + '/fmwebapp/images/Arrow-rightBL.png">';
	$( '#' + AnimationManager.AnimationTestRowPrefix + animationTestGroupGuid ).addClass( 'hidden' );
	currentItem.setAttribute( 'onClick', 'javascript: return AnimationManager.ExpandTestGroup(this);' );
};

AnimationManager.ExpandTestGroup = function( currentItem )
{
	var animationTestGroupGuid = currentItem.id.replace( AnimationManager.AnimationTestGroupExpandPrefix, '' );
	currentItem.innerHTML = '<img name="collapse" draggable="false" title="expanded" alt="expanded" src="' + window.applicationRootName + '/fmwebapp/images/Arrow-downBl.png">';
	$( '#' + AnimationManager.AnimationTestRowPrefix + animationTestGroupGuid ).removeClass( 'hidden' );
	currentItem.setAttribute( 'onClick', 'javascript: return AnimationManager.CollapseTestGroup(this);' );
};

AnimationManager.CollapseTest = function( currentItem )
{
	var animationTestGuid = currentItem.id.replace( AnimationManager.AnimationTestExpandPrefix, '' );
	currentItem.innerHTML = '<img name="expand" draggable="false" title="collapsed" alt="collapsed" src="' + window.applicationRootName + '/fmwebapp/images/Arrow-rightBL.png">';
	$( '#' + AnimationManager.AnimationTestPropertyRowPrefix + animationTestGuid ).addClass( 'hidden' );
	currentItem.setAttribute( 'onClick', 'javascript: return AnimationManager.ExpandTest(this);' );
};

AnimationManager.ExpandTest = function( currentItem )
{
	var animationTestGuid = currentItem.id.replace( AnimationManager.AnimationTestExpandPrefix, '' );
	currentItem.innerHTML = '<img name="collapse" draggable="false" title="expanded" alt="expanded" src="' + window.applicationRootName + '/fmwebapp/images/Arrow-downBl.png">';
	$( '#' + AnimationManager.AnimationTestPropertyRowPrefix + animationTestGuid ).removeClass( 'hidden' );
	currentItem.setAttribute( 'onClick', 'javascript: return AnimationManager.CollapseTest(this);' );
};

AnimationManager.confirmsaveanimation = function (animationGuidStr)
{
    var confirmationStringTranslated = $("#AnimationManagerSaveModifiedAnimationTranslated").val();
    FMLayout.ConfirmYesNo(confirmationStringTranslated,
                            "Save Animation",
    function ()
    {
        if(animationGuidStr === AnimationManager.AnimationEmptyGuid)
        {
            AnimationManager.SaveAnimation();
        }
        else
        {
            AnimationManager.UpdateForNewAnimationAndSave(animationGuidStr);
            AnimationManager.EnableDisableAnimationDelete();
        }
        return;
    },
    function ()
    {
        if(animationGuidStr === AnimationManager.AnimationEmptyGuid)
        {
            AnimationManager.OpenNewAnimation();
        }
        else
        {
		    AnimationManager.RestoreAnimationToOriginal();
			AnimationManager.UpdateForNewAnimation(animationGuidStr);
			AnimationManager.EnableDisableAnimationDelete();
		}
        return;
    });
}

AnimationManager.AnimationSelectionChanged = function( obj )
{
	var animationGuidStr = obj.value;
	if (animationGuidStr === AnimationManager.AnimationEmptyGuid)
	{
        if (AnimationManager.AnimationModified()) {
	        AnimationManager.confirmsaveanimation(animationGuidStr);
	    }
	    else {
	        AnimationManager.OpenNewAnimation();
        }
	}
    else
	{
		if ( animationGuidStr !== "EMPTY" )
		{
			if ( AnimationManager.AnimationModified() )
    		{
	            AnimationManager.confirmsaveanimation(animationGuidStr);
			}
			else if(animationGuidStr !== AnimationManager.AnimationEmptyGuid)
			{
				AnimationManager.UpdateForNewAnimation( animationGuidStr );
				AnimationManager.EnableDisableAnimationDelete();
			}
		}
	}
};

AnimationManager.CheckForRedundantAnimationName = function( newanimationname1, model )
{
	for ( var i = 0; i < model.AnimationList.length; i++ )
	{
		if ( model.AnimationList[i].ID.toLowerCase() === newanimationname1.toLowerCase() )
		{
			return true;
		}
	}
	return false;
};

AnimationManager.CreateNewAnimation = function( obj )
{
	var newanimationname1 = $( '#newanimationname1' ).val();
	var model = AnimationManager.GetAnimationManagerModel();

	if ( AnimationManager.CheckForRedundantAnimationName( newanimationname1, model ) === false )
	{
		$( '#AnimationNewModalScreen' ).modal( 'hide' );
		AnimationManager.AnimationNewModalScreenShowing = false;
		var newAnimationGuid = AnimationManager.CreateGuid();
		var newAnimation = {};
		newAnimation.AnimationGuid = newAnimationGuid;
		newAnimation.ID = newanimationname1;
		newAnimation.UseCount = 0;
		newAnimation.AnimationTestGroups = [];
		model.AnimationList.push( newAnimation );
		model.SelectedAnimationGuid = newAnimationGuid;
		AnimationManager.SetAnimationManagerModel( model );
		AnimationManager.UpdateForNewAnimation( newAnimationGuid );

		$( "#" + AnimationManager.animationManagerOkButtonId ).removeAttr( "disabled" );
	}
	else
	{
		FMErrorAndExceptionHandling.ShowError( "Name Already In Use: " + newanimationname1 );
		AnimationManager.CloseNewAnimation();
	}
};

AnimationManager.OpenNewAnimation = function( obj )
{
	FMErrorAndExceptionHandling.CloseNotifications();
	AnimationManager.AnimationDeactivatTextEditingTools();
	AnimationManager.AnimationNewModalScreenShowing = true;
	$( '#AnimationNewModalScreen' ).modal( 'show' );
	$( '#newanimationname1' ).val( '' );
};

AnimationManager.CloseNewAnimation = function( obj )
{
	$( '#AnimationNewModalScreen' ).modal( 'hide' );
	AnimationManager.AnimationNewModalScreenShowing = false;
	var model = AnimationManager.GetAnimationManagerModel();
	if ( model.SelectedAnimationGuid === AnimationManager.AnimationEmptyGuid )
	{
		$( '#AnnimationDropDown' ).val( AnimationManager.AnimationEmptyAnimation );
	}
	else
	{
		$( '#AnnimationDropDown' ).val( model.SelectedAnimationGuid );
	}
};

AnimationManager.UpdateForNewAnimation = function( animationGuidStr )
{
	var url = $( '#urlAnimationManagerAnimationSelectionChanged' ).val();
	AnimationManager.CallController( animationGuidStr, url, true );
};

AnimationManager.UpdateForNewAnimationAndSave = function( animationGuidStr )
    {
	var url = $( '#urlAnimationManagerAnimationSelectionChangedAndSave' ).val();
	AnimationManager.CallController( animationGuidStr, url, true );
};

AnimationManager.CallController = function( animationGuidStr, url, async )
{
	var token = $( 'input[name=__RequestVerificationToken]' ).val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var modelStr = AnimationManager.GetAnimationManagerModelString();
	var callData = { animationGuidStr: animationGuidStr, modelStr: modelStr };

	FMErrorAndExceptionHandling.CloseNotifications();
	$( 'body' ).modalmanager( 'loading' );

	$.ajax( {
		type: 'Post',
		url: url,
		data: callData,
		headers: headers,
		async: async,
		cache: false,
		success: function( response )
		{
			var modalManager = $( 'body' ).data( 'modalmanager' );
			modalManager.removeLoading();
			FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
			{
				if ( !inError )
				{
					// replace the holder with the partial view
					$( AnimationManager.LastSectionId ).html( data );
				}
				else
				{
				}
			} );
		},
		error: function( xhr, textStatus, error )
		{
			var modalManager = $( 'body' ).data( 'modalmanager' );
			modalManager.removeLoading();
			FMErrorAndExceptionHandling.ShowException( xhr,
				textStatus,
				error,
				function()
				{
				} );
		}
	} );
};

AnimationManager.AnimationDataTypeSelectionChanged = function( currentItem )
{
	var animationTestGroupGuid = currentItem.id.replace( AnimationManager.AnimationTestGroupDataTypeDropDownIdPrefix, '' );

	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	if ( animation )
	{
		var animationTestGroup = AnimationManager.GetAnimationTestGroup( animation, animationTestGroupGuid );
		if ( animationTestGroup )
		{
			animationTestGroup.DataType = currentItem.value;
			animationTestGroup.PointValueGuid = AnimationManager.AnimationEmptyGuid;
			animationTestGroup.PointGuid = AnimationManager.AnimationEmptyGuid;
			animationTestGroup.Field = 0;
			animationTestGroup.PointValueIsFromTemplate = false;
			animationTestGroup.PointValueAndFieldID = AnimationManager.AnimationUnassigned;
			animationTestGroup.ValueType = 0;
			animationTestGroup.PointID = "";
			animationTestGroup.PointValueID = "";

			var pointValueObj = document.getElementById( AnimationManager.AnimationTestGroupPointValueIdPrefix + animationTestGroupGuid );
			pointValueObj.innerHTML = AnimationManager.AnimationUnassigned;
			AnimationManager.SetAnimationManagerModel( model );
		}
	}
};

AnimationManager.GetIndexOfCurrentAnimation = function( model )
{
	var animationGuid = model.SelectedAnimationGuid;
	return AnimationManager.GetIndexOfAnimationByAnimationGuid( model, animationGuid );
};

AnimationManager.GetAnimationByAnimationGuid = function( model, animationGuid )
{
	var i = AnimationManager.GetIndexOfAnimationByAnimationGuid( model, animationGuid );
	if ( i >= 0 )
	{
		return model.AnimationList[i];
	}
	return undefined;
};

AnimationManager.GetIndexOfAnimationByAnimationGuid = function( model, animationGuid )
{
	for ( var i = model.AnimationList.length - 1; i >= 0; i-- )
	{
		if ( model.AnimationList[i].AnimationGuid === animationGuid )
		{
			return i;
		}
	}
	return -1;
};

AnimationManager.GetCurrentAnimation = function( model )
{
	var animationGuid = model.SelectedAnimationGuid;
	return AnimationManager.GetAnimationByAnimationGuid( model, animationGuid );
};

AnimationManager.GetIndexOfAnimationTestGroup = function( animation, animationTestGroupGuid )
{
	if ( animation )
	{
		for ( var j = animation.AnimationTestGroups.length - 1; j >= 0; j-- )
		{
			if ( animation.AnimationTestGroups[j].AnimationTestGroupGuid === animationTestGroupGuid )
			{
				return j;
			}
		}
	}
	return -1;
};

AnimationManager.GetAnimationTestGroup = function( animation, animationTestGroupGuid )
{
	var i = AnimationManager.GetIndexOfAnimationTestGroup( animation, animationTestGroupGuid );
	if ( i >= 0 )
	{
		return animation.AnimationTestGroups[i];
	}
	return undefined;
};

AnimationManager.GetAnimationTestGroupForAnimationTestGuid = function( animation, animationTestGuid )
{
	if ( animation )
	{
		for ( var j = animation.AnimationTestGroups.length - 1; j >= 0; j-- )
		{
			var animationTestGroup = animation.AnimationTestGroups[j];
			var animationTestIndex = AnimationManager.GetIndexOfAnimationTest( animationTestGroup, animationTestGuid );
			if ( animationTestIndex >= 0 )
			{
				return animationTestGroup;
			}
		}
	}
	return undefined;
};

AnimationManager.GetIndexOfAnimationTest = function( animationTestGroup, animationTestGuid )
{
	if ( animationTestGroup )
	{
		for ( var j = animationTestGroup.TestList.length - 1; j >= 0; j-- )
		{
			if ( animationTestGroup.TestList[j].AnimationTestGuid === animationTestGuid )
			{
				return j;
			}
		}
	}
	return -1;
};

AnimationManager.GetAnimationTest = function( animationTestGroup, animationTestGuid )
{
	var i = AnimationManager.GetIndexOfAnimationTest( animationTestGroup, animationTestGuid );
	if ( i >= 0 )
	{
		return animationTestGroup.TestList[i];
	}
	return undefined;
};

AnimationManager.GetAnimationTestByProperty = function( animation, animationPropertyGuid )
{
	for ( var nextTestGrpIndex = 0; nextTestGrpIndex < animation.AnimationTestGroups.length; nextTestGrpIndex++ )
	{
		var testGroup = animation.AnimationTestGroups[nextTestGrpIndex];
		for ( var nextTextIndex = 0; nextTextIndex < testGroup.TestList.length; nextTextIndex++ )
		{
			var test = testGroup.TestList[nextTextIndex];
			for ( var nextPropertyIndex = 0; nextPropertyIndex < test.PropertyList.length; nextPropertyIndex++ )
			{
				var property = test.PropertyList[nextPropertyIndex];
				if ( property.AnimationPropertyGuid === animationPropertyGuid )
				{
					return test;
				}
			}
		}
	}

	return null;
};

AnimationManager.GetAnimationTestGroupByTest = function( animation, animationTestGuid )
{
	for ( var nextTestGrpIndex = 0; nextTestGrpIndex < animation.AnimationTestGroups.length; nextTestGrpIndex++ )
	{
		var testGroup = animation.AnimationTestGroups[nextTestGrpIndex];
		for ( var nextTextIndex = 0; nextTextIndex < testGroup.TestList.length; nextTextIndex++ )
		{
			var test = testGroup.TestList[nextTextIndex];
			if ( test.AnimationTestGuid === animationTestGuid )
			{
				return testGroup;
			}
		}
	}

	return null;
};

AnimationManager.EditableTestGroupDataType = function( animationTestGrpGuid )
{
	var model = AnimationManager.GetAnimationManagerModel();
	if ( model )
	{
		var animation = AnimationManager.GetCurrentAnimation( model );
		if ( animation )
		{
			var testGroup = AnimationManager.GetAnimationTestGroup( animation, animationTestGrpGuid );
			if ( testGroup && testGroup.TestList && testGroup.TestList.length > 0 )
			{
				return false;
			}
		}
	}
	return true;
};

AnimationManager.DisableTestGroupDataTypeDropdown = function( testGrpGuid )
{
	var enabled = AnimationManager.EditableTestGroupDataType( testGrpGuid );
	if ( enabled === false )
	{
		return;
	}

	var dropdownId = AnimationManager.AnimationTestGroupDataTypeDropDownIdPrefix + testGrpGuid;
	var dropdown = document.getElementById( dropdownId );
	if ( dropdown )
	{
		dropdown.disabled = true;
	}
};

AnimationManager.EnableTestGroupDataTypeDropdown = function( testGrpGuid )
{
	var enabled = AnimationManager.EditableTestGroupDataType( testGrpGuid );
	if ( enabled === false )
	{
		return;
	}

	var dropdownId = AnimationManager.AnimationTestGroupDataTypeDropDownIdPrefix + testGrpGuid;
	var dropdown = document.getElementById( dropdownId );
	if ( dropdown )
	{
		dropdown.disabled = false;
	}
};

AnimationManager.GetIndexOfAnimationTestProperty = function( animationTest, animationPropertyGuid )
{
	if ( animationTest )
	{
		for ( var j = animationTest.PropertyList.length - 1; j >= 0; j-- )
		{
			if ( animationTest.PropertyList[j].AnimationPropertyGuid === animationPropertyGuid )
			{
				return j;
			}
		}
	}
	return -1;
};

AnimationManager.GetAnimationTestProperty = function( animationTest, animationPropertyGuid )
{
	var i = AnimationManager.GetIndexOfAnimationTestProperty( animationTest, animationPropertyGuid );
	if ( i >= 0 )
	{
		return animationTest.PropertyList[i];
	}
	return undefined;
};

AnimationManager.GetNewTestEditRowParameters = function()
{
	var testEditRowParms = new Object();
	testEditRowParms.testEditRow = null;
	testEditRowParms.testGroupType = null;
	testEditRowParms.testEditHolderRowId = null;
	testEditRowParms.currentItem = null;
	testEditRowParms.testRowBeforeEdit = null;
	testEditRowParms.mode = null;
	testEditRowParms.dataType = null;
	testEditRowParms.bitwiseOperator = null;
	testEditRowParms.bitmaskStr = null;
	testEditRowParms.comparisonOperator = null;
	testEditRowParms.comparisonValue = null;
	testEditRowParms.animationTestGuid = null;
	testEditRowParms.animationTestGrpGuid = null;

	return testEditRowParms;
};

AnimationManager.EnableDisableAnimationDelete = function()
{
	AnimationManager.animationDeleteEnabled = false;
	$( "#animationDelete" ).css( "cursor", "not-allowed" );
	$( "#" + AnimationManager.animationManagerOkButtonId ).attr( "disabled", "disabled" );

	var model = AnimationManager.GetAnimationManagerModel();

	if ( model )
	{
		var animation = AnimationManager.GetCurrentAnimation( model );
		if ( animation )
		{
			$( "#" + AnimationManager.animationManagerOkButtonId ).removeAttr( "disabled" );
			$( "#animationDelete" ).css( "cursor", "" );
			AnimationManager.animationDeleteEnabled = true;
		}
	}
};

AnimationManager.SetEditMode = function()
{
	AnimationManager.AnimationInEditMode = true;
	$( "#" + AnimationManager.animationManagerOkButtonId ).attr( "disabled", "disabled" );
};

AnimationManager.ClearEditMode = function()
{
	AnimationManager.AnimationInEditMode = false;
	$( "#" + AnimationManager.animationManagerOkButtonId ).removeAttr( "disabled" );
};

AnimationManager.GetDataTypeDisplayName = function( dataType )
{
	var model = AnimationManager.GetAnimationManagerModel();

	if ( dataType && model )
	{
		if ( dataType === "Alarm Status" )
		{
			return dataType;
		}

		for ( var nextItem = 0; nextItem < model.ValidTestGroupDataTypeList.length; nextItem++ )
		{
			var item = model.ValidTestGroupDataTypeList[nextItem];
			if ( item.Key === dataType )
			{
				return item.Value;
			}
		}
	}

	return "";
};

AnimationManager.DelAnimation = function()
{
	var model = AnimationManager.GetAnimationManagerModel();
	var animationGuid = model.SelectedAnimationGuid;
	var newAnimationGuid = AnimationManager.AnimationEmptyGuid;
	var i = AnimationManager.GetIndexOfCurrentAnimation( model );

	if ( i < 0 )
	{
		$( "#" + AnimationManager.animationManagerOkButtonId ).attr( "disabled", "disabled" );
		return;
	}

	if ( i > 0 )
	{
		newAnimationGuid = model.AnimationList[i - 1].AnimationGuid;
	}
	else
	{
		if ( model.AnimationList.length > i + 1 )
		{
			newAnimationGuid = model.AnimationList[i + 1].AnimationGuid;
		}
	}

	model.AnimationList.splice( i, 1 );
	model.SelectedAnimationGuid = newAnimationGuid;
	AnimationManager.SetAnimationManagerModel( model );
	var url = $( '#urlAnimationManagerDeleteAnimation' ).val();
	AnimationManager.CallController( animationGuid, url, true );
};

AnimationManager.RecordAnimationPointValueAssignments = function( model )
{
	var animation = AnimationManager.GetCurrentAnimation( model );
	var animationPointValueAssignment = {
		AnimationGuid: animation.AnimationGuid,
		AnimationID: animation.ID,
        TestGroupPointValueInfoList: []
	};

	for ( var i = 0; i < animation.AnimationTestGroups.length; i++ )
	{
		var testGroup = animation.AnimationTestGroups[i];
		if ( testGroup.PointValueGuid && testGroup.PointValueGuid !== AnimationManager.AnimationEmptyGuid )
		{
			var testGroupPointValueInfo = {
				AnimationTestGroupGuid: testGroup.AnimationTestGroupGuid,
				DataType: testGroup.DataType,
				PointValueGuid: testGroup.PointValueGuid,
				PointGuid: testGroup.PointGuid,
				Field: testGroup.Field,
				PointValueIsFromTemplate: testGroup.PointValueIsFromTemplate,
				ValueType: testGroup.ValueType,
				PointID: testGroup.PointID,
				PointValueID: testGroup.PointValueID
			};
			animationPointValueAssignment.TestGroupPointValueInfoList.push( testGroupPointValueInfo );
		}
	}

	if ( animationPointValueAssignment.TestGroupPointValueInfoList.length > 0 )
	{
		model.PointValueList = animationPointValueAssignment;
	}
	else
	{
		model.PointValueList = null;
	}
	AnimationManager.SetAnimationManagerModel( model );
};

AnimationManager.SaveAnimation = function()
{
	var model = AnimationManager.GetAnimationManagerModel();
	var animationGuid = model.SelectedAnimationGuid;
	AnimationManager.RecordAnimationPointValueAssignments( model );
	var url = $( '#urlAnimationManagerOKPressed' ).val();
	AnimationManager.CallController( animationGuid, url, true );
};

AnimationManager.AddAnimationTestGroup = function( currentItem )
{
	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	if ( animation == null || AnimationManager.AnimationInEditMode )
	{
		return;
	}

	var newGuid = AnimationManager.CreateGuid();

	AnimationManager.AnimationUnassigned = $("#AnimationMgrUnassignedTranslated").val();

	var newAnimationTestGroup = {
		AnimationTestGroupGuid: newGuid,
		ID: newGuid,
		DataType: model.TranslatedTextForAlarmStatusField,
		PointValueGuid: AnimationManager.AnimationEmptyGuid,
		PointGuid: AnimationManager.AnimationEmptyGuid,
		Field: FMTAGFIELDSELECTION.ALARMSTATUS,
		PointValueIsFromTemplate: false,
		ValueType: 0,
		PointID: "",
		PointValueID: "",
		//SRM Need Translated Text
		PointValueAndFieldID: AnimationManager.AnimationUnassigned,
		TestList: []
	};

	animation.AnimationTestGroups.push( newAnimationTestGroup );
	AnimationManager.SetAnimationManagerModel( model );
	AnimationManager.CreateNewTestGroup( currentItem, newAnimationTestGroup, model );
	AnimationManager.ApplyAlternatingRowColors();
	var testGroupNameItem = document.getElementById( AnimationManager.AnimationTestGroupIdIdPrefix + newAnimationTestGroup.AnimationTestGroupGuid );
	AnimationManager.PutTestGroupNameInEditMode( testGroupNameItem );
};

AnimationManager.CreateNewTestGroup = function( currentItem, newAnimationTestGroup, model )
{
	var tableRef = document.getElementById( AnimationManager.AnimationTestGroupsTable ).getElementsByTagName( 'tbody' )[0];
	if ( !tableRef )
	{
		return;
	}

	// Insert a row in the table at the last row
	var newGuid = newAnimationTestGroup.AnimationTestGroupGuid;

	var testGroupRow = tableRef.insertRow( -1 );
	testGroupRow.id = AnimationManager.AnimationTestGroupRowPrefix + newGuid;
	testGroupRow.setAttribute( "class", "rowBorder" );

	var cell1 = testGroupRow.insertCell( 0 );
	var cell2 = testGroupRow.insertCell( -1 );
	var cell3 = testGroupRow.insertCell( -1 );
	var cell4 = testGroupRow.insertCell( -1 );
	var cell5 = testGroupRow.insertCell( -1 );
	var cell6 = testGroupRow.insertCell( -1 );

	var collapseImg = document.createElement( "img" );
	collapseImg.name = "collapse";
	collapseImg.title = $("#AnimationMgrExpandedTranslated").val();
	collapseImg.draggable = false;
	collapseImg.alt = $("#AnimationMgrExpandedTranslated").val();
	collapseImg.src = window.applicationRootName + "/fmwebapp/images/Arrow-downBl.png";
	cell1.appendChild( collapseImg );
	cell1.setAttribute( "class", "testGroupCol1 text-left testGroupRowTd firstAnimationTestGroupTd" );
	cell1.id = AnimationManager.AnimationTestGroupExpandPrefix + newGuid;
	cell1.onclick = function()
	{
		return AnimationManager.CollapseTestGroup( this );
	};

	var addImage = document.createElement( "img" );
	addImage.name = "add";
	addImage.title = $("#AnimationMgrAddTestTranslated").val();
	addImage.draggable = false;
	addImage.alt = $("#AnimationMgrAddTestTranslated").val();
	addImage.src = window.applicationRootName + "/fmwebapp/images/Insert-Row.png";
	cell2.appendChild( addImage );
	cell2.setAttribute( "class", "testGroupCol2 text-left testGroupRowTd" );
	cell2.id = AnimationManager.AnimationTestGroupAddIdPrefix + newGuid;
	cell2.onclick = function()
	{
		return AnimationManager.AddTest( cell2 );
	};

	var trashcanImage = document.createElement( "img" );
	trashcanImage.name = "del";
	trashcanImage.title = $("#AnimationMgrDeleteTestGroupTranslated").val();
	trashcanImage.draggable = false;
	trashcanImage.alt = $("#AnimationMgrDeleteTestGroupTranslated").val();
	trashcanImage.src = window.applicationRootName + "/fmwebapp/images/Trash-icon.png";
	cell3.appendChild( trashcanImage );
	cell3.setAttribute( "class", "testGroupCol3 text-center testGroupRowTd" );
	cell3.id = AnimationManager.AnimationTestGroupDelIdPrefix + newGuid;
	cell3.onclick = function()
	{
		if ( AnimationManager.AnimationInEditMode )
		{
			return;
		}

	    //SRM Need Translated Text
		if ( confirm( "Are you sure you wish to delete this item?" ) )
		{
			AnimationManager.DelTestGroup( cell3 );
		}
	};

	cell4.innerHTML = newAnimationTestGroup.ID;
	cell4.setAttribute( "class", "testGroupCol4 text-left testGroupRowTd" );
	cell4.id = AnimationManager.AnimationTestGroupIdIdPrefix + newGuid;
	cell4.ondblclick = function()
	{
		return AnimationManager.PutTestGroupNameInEditMode( cell4 );
	}

	cell5.innerHTML = AnimationManager.GetDataTypeDisplayName( newAnimationTestGroup.DataType );
	cell5.setAttribute( "class", "testGroupCol5 text-center testGroupRowTd" );
	cell5.id = AnimationManager.AnimationTestGroupDataTypeIdPrefix + newGuid;

	cell6.innerHTML = newAnimationTestGroup.PointValueAndFieldID;
	cell6.setAttribute( "class", "testGroupCol6 text-left testGroupRowTd" );
	cell6.id = AnimationManager.AnimationTestGroupPointValueIdPrefix + newGuid;
	cell6.ondblclick = function()
	{
		return AnimationManager.TestGroupPointValueSelection( cell6 );
	};

	// Create the row and column that will contain the test table.
	var testRow = tableRef.insertRow( -1 );
	testRow.id = AnimationManager.AnimationTestRowPrefix + newGuid;
	testRow.setAttribute( "class", "rowBorder" );
	var testCell = testRow.insertCell( 0 );
	testCell.id = AnimationManager.AnimationTestRowTdPrefix + newGuid;
	testCell.setAttribute( "class", "text-left" );
	testCell.setAttribute( "colspan", "6" );

	// Create the test property table that will contain the properties.
	var testTableObj = AnimationManager.CreateTestTable( newGuid );
	testCell.appendChild( testTableObj );
	AnimationManager.UpdateTestGroupDataTypeEditMode( model, newAnimationTestGroup );
};

AnimationManager.UpdateTestGroupDataTypeEditMode = function( model, testGroup )
{
	var dataTypeItem = document.getElementById( AnimationManager.AnimationTestGroupDataTypeIdPrefix + testGroup.AnimationTestGroupGuid );
	if ( testGroup.TestList && testGroup.TestList.length > 0 )
	{
		dataTypeItem.innerHTML = AnimationManager.GetDataTypeDisplayName( testGroup.DataType );
	}
	else
	{
		dataTypeItem.innerHTML = AnimationManager.CreateEditableTestGroupDataType( model, testGroup );
	}
};

AnimationManager.PutTestGroupNameInEditMode = function( currentItem )
{
	if ( AnimationManager.inEditTestGroupNameMode === false )
	{
		var testGroupGuid = currentItem.id.replace( AnimationManager.AnimationTestGroupIdIdPrefix, "" );
		var model = AnimationManager.GetAnimationManagerModel();
		var animation = AnimationManager.GetCurrentAnimation( model );
		var testGroup = AnimationManager.GetAnimationTestGroup( animation, testGroupGuid );
		currentItem.innerHTML = AnimationManager.CreateEditableTestGroupName( testGroup.ID, testGroupGuid );
		AnimationManager.inEditTestGroupNameMode = true;
		AnimationManager.FocusOnTagNameInput( testGroupGuid );
	}
};

AnimationManager.CreateEditableTestGroupDataType = function( model, testGroup )
{
	var textForAlarmStatusSelection = '&lt' + model.TranslatedTextForAlarmStatusField + '&gt';
	var cell5SelectHtml = '<select id="' + AnimationManager.AnimationTestGroupDataTypeDropDownIdPrefix + testGroup.AnimationTestGroupGuid + '" class="animationDropDown" onchange="AnimationManager.AnimationDataTypeSelectionChanged( this )">';
	var selectedText = testGroup.Field === FMTAGFIELDSELECTION.ALARMSTATUS ? "selected" : "";
	cell5SelectHtml += '<option ' + selectedText + ' value="' + model.TranslatedTextForAlarmStatusField + '">' + textForAlarmStatusSelection + '</option>';

	for ( var i = 0; i < model.ValidTestGroupDataTypeList.length; i++ )
	{
		var dataType = model.ValidTestGroupDataTypeList[i];
		selectedText = testGroup.Field === FMTAGFIELDSELECTION.VALUE && dataType.Key === testGroup.DataType ? "selected" : "";
		cell5SelectHtml += '<option ' + selectedText + ' value="' + dataType.Key + '">' + dataType.Value + '</option>';
	}

	cell5SelectHtml += '</select>';
	return cell5SelectHtml;
};

AnimationManager.CreateEditableTestGroupName = function( testGroupName, testGroupGuid )
{
	return "<Span><input id=\"" + AnimationManager.EnterTestGroupNamePrefix + testGroupGuid + "\" type=\"text\" value=\"" + testGroupName + "\" onkeydown=\"javascript: return AnimationManager.TestGroupNameKeyHandler();\" onblur=\"javascript: return AnimationManager.TestGroupOnBlurHandler();\" maxlength=\"30\"/></Span>";
};

AnimationManager.TestGroupNameKeyHandler = function( e )
{
	e = e || event;
	if ( ( e.keyCode || e.which || e.charCode || 0 ) === 13 )
	{
		AnimationManager.SaveTestGroupName( e.target );
		return false;
	}
	else if ( ( e.keyCode || e.which || e.charCode || 0 ) === 27 )
	{
		AnimationManager.CancelTestGroupName( e.target );
		return false;
	}
	return true;
};

AnimationManager.TestGroupOnBlurHandler = function( e )
{
	e = e || event;
	AnimationManager.SaveTestGroupName( e.target );
};

AnimationManager.DoubleCheckNameRedundant = function( animation, testGroupGuid, newTestGroupName )
{
	if ( animation )
	{
		for ( var j = animation.AnimationTestGroups.length - 1; j >= 0; j-- )
		{
			if ( animation.AnimationTestGroups[j].AnimationTestGroupGuid !== testGroupGuid && animation.AnimationTestGroups[j].ID === newTestGroupName )
			{
				return false;
			}
		}
	}
	return true;
};

AnimationManager.FocusOnTagNameInput = function( testGroupGuid )
{
	var inputItem = document.getElementById( AnimationManager.EnterTestGroupNamePrefix + testGroupGuid );
	if ( inputItem )
	{
		inputItem.focus();
	}
};


AnimationManager.SaveTestGroupName = function( inputItem )
{
	var newTestGroupName = inputItem.value;
	var testGroupGuid = inputItem.id.replace( AnimationManager.EnterTestGroupNamePrefix, "" );
	if ( !newTestGroupName )
	{
		FMErrorAndExceptionHandling.ShowError( "Test Group Name cannot be empty!" );
		AnimationManager.FocusOnTagNameInput( testGroupGuid );
		return;
	}
	AnimationManager.inEditTestGroupNameMode = false;
	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );

	if ( AnimationManager.DoubleCheckNameRedundant( animation, testGroupGuid, newTestGroupName ) === false )
	{
		FMErrorAndExceptionHandling.ShowError( newTestGroupName + "already in use by another test group!" );
		AnimationManager.FocusOnTagNameInput( testGroupGuid );
		return;
	}

	var testGroup = AnimationManager.GetAnimationTestGroup( animation, testGroupGuid );

	var cell = document.getElementById( AnimationManager.AnimationTestGroupIdIdPrefix + testGroupGuid );
	if ( cell )
	{
		cell.innerHTML = newTestGroupName;
	}

	testGroup.ID = newTestGroupName;
	AnimationManager.SetAnimationManagerModel( model );
};

AnimationManager.CancelTestGroupName = function( inputItem )
{
	var testGroupGuid = inputItem.id.replace( AnimationManager.EnterTestGroupNamePrefix, "" );
	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	var testGroup = AnimationManager.GetAnimationTestGroup( animation, testGroupGuid );
	var cell = document.getElementById( AnimationManager.AnimationTestGroupIdIdPrefix + testGroupGuid );
	if ( cell )
	{
		cell.innerHTML = testGroup.ID;
	}
	AnimationManager.inEditTestGroupNameMode = false;
};


AnimationManager.CreateTestTable = function( newGuid )
{
	var testTableId = AnimationManager.AnimationTestTableIdPrefix + newGuid;

	var testTable = document.createElement( "table" );
	testTable.id = testTableId;
	testTable.setAttribute( "class", "animationTestsTable" );
	var header = testTable.createTHead();
	header.setAttribute( "class", "animationTableHeaderFormating" );
	var headerRow = header.insertRow( 0 );

	var colCell1 = document.createElement( "th" );
	var colCell2 = document.createElement( "th" );
	var colCell3 = document.createElement( "th" );
	var colCell4 = document.createElement( "th" );
	var colCell5 = document.createElement( "th" );

	headerRow.appendChild( colCell1 );
	headerRow.appendChild( colCell2 );
	headerRow.appendChild( colCell3 );
	headerRow.appendChild( colCell4 );
	headerRow.appendChild( colCell5 );

	//SRM Need Translated Text For the Following Strings
	colCell1.innerHTML = "";
	colCell2.innerHTML = $("#AnimationMgrAddPropertyTranslated").val();
	colCell3.innerHTML = $("#AnimationMgrDeleteTestTranslated").val();
	colCell4.innerHTML = $("#AnimationMgrTestDefinitionTranslated").val();
	colCell5.innerHTML = $("#AnimationMgrPreviewTranslated").val();

	colCell1.setAttribute( "class", "text-center testHeaderCol1 testGroupRowTd firstAnimationTestTd" );
	colCell2.setAttribute( "class", "text-left testGroupRowTd testHeaderCol2" );
	colCell3.setAttribute( "class", "text-left testGroupRowTd testHeaderCol3" );
	colCell4.setAttribute( "class", "text-left testGroupRowTd testHeaderCol4" );
	colCell5.setAttribute( "class", "text-left testGroupRowTd testHeaderCol5" );

	var testTbody = document.createElement( "tbody" );
	testTable.appendChild( testTbody );

	return testTable;
};

AnimationManager.DelTestGroup = function( currentItem )
{
	var animationTestGroupGuid = currentItem.id.replace( AnimationManager.AnimationTestGroupDelIdPrefix, '' );

	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	if ( animation )
	{
		var j = AnimationManager.GetIndexOfAnimationTestGroup( animation, animationTestGroupGuid );
		if ( j >= 0 )
		{
			animation.AnimationTestGroups.splice( j, 1 );
			AnimationManager.SetAnimationManagerModel( model );
			var row = document.getElementById( "AnimationTestRow_" + animationTestGroupGuid );
			row.parentNode.removeChild( row );
			row = document.getElementById( AnimationManager.AnimationTestGroupRowPrefix + animationTestGroupGuid );
			row.parentNode.removeChild( row );
		}
	}
	AnimationManager.ApplyAlternatingRowColors();
};

AnimationManager.DelTest = function( currentItem )
{
	var animationTestGuid = currentItem.id.replace( AnimationManager.AnimationTestDeletePrefix, '' );

	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	if ( animation )
	{
		for ( var j = animation.AnimationTestGroups.length - 1; j >= 0; j-- )
		{
			var animationTestGroup = animation.AnimationTestGroups[j];
			var animationTestIndex = AnimationManager.GetIndexOfAnimationTest( animationTestGroup, animationTestGuid );
			if ( animationTestIndex >= 0 )
			{
				animationTestGroup.TestList.splice( animationTestIndex, 1 );
				AnimationManager.SetAnimationManagerModel( model );
				var row = document.getElementById( AnimationManager.AnimationTestPropertyRowPrefix + animationTestGuid );
				row.parentNode.removeChild( row );
				row = document.getElementById( AnimationManager.AnimationTestTableRowPrefix + animationTestGuid );
				row.parentNode.removeChild( row );
				AnimationManager.UpdateTestGroupDataTypeEditMode( model, animationTestGroup );
			}
		}
	}
	AnimationManager.ApplyAlternatingRowColors();
};

AnimationManager.DelProperty = function( currentItem )
{
	var animationPropertyGuid = currentItem.id.replace( AnimationManager.AnimationPropertyDeletePrefix, '' );

	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	if ( animation )
	{
		for ( var j = animation.AnimationTestGroups.length - 1; j >= 0; j-- )
		{
			var animationTestGroup = animation.AnimationTestGroups[j];
			for ( var i = animationTestGroup.TestList.length - 1; i >= 0; i-- )
			{
				var animationTest = animationTestGroup.TestList[i];
				var animationPropertyIndex = AnimationManager.GetIndexOfAnimationTestProperty( animationTest, animationPropertyGuid );
				if ( animationPropertyIndex >= 0 )
				{
					animationTest.PropertyList.splice( animationPropertyIndex, 1 );
					AnimationManager.SetAnimationManagerModel( model );
					var row = document.getElementById( AnimationManager.AnimationTestPropertyTableRowPrefix + animationPropertyGuid );
					row.parentNode.removeChild( row );
				}
			}
		}
	}
	AnimationManager.ApplyAlternatingRowColors();
};

AnimationManager.HandleAnimationNameTextboxOnChange = function( textboxId, okButtonId )
{
    var textValueOk = true;
	var textValue = $( "#" + textboxId ).val();
	$( "#" + okButtonId ).attr( "disabled", "disabled" );
	if ( textValue != null && textValue !== "" )
	{
		for ( var i = 0; i < textValue.length; i++ )
		{
			var code = textValue.charCodeAt( i );

			if ( !( code > 47 && code < 58 ) && // numeric (0-9)
				!( code > 64 && code < 91 ) && // upper alpha (A-Z)
				!( code === 45 ) && // dash
				!( code === 95 ) && // underscore
				!( code > 96 && code < 123 ) ) // lower alpha (a-z)
			{
				textValueOk = false;
			}
		}

		if ( textValueOk )
		{
			$( "#" + okButtonId ).removeAttr( "disabled" );
		}
	}
};

//====================================================  This section is for adding Tests =================================================
AnimationManager.AddTest = function( currentItem )
{
	if ( AnimationManager.AnimationInEditMode === false )
	{
		AnimationManager.CreateTestEditRow( currentItem );
	}
	AnimationManager.ApplyAlternatingRowColors();
};

//===============================================================================
// This function will create a new test row, test row for containing the 
// properties, and the properties table.
//===============================================================================
AnimationManager.CreateNewTest = function( currentItem, testName )
{
	var animationTestGuid = currentItem.id.replace( AnimationManager.AnimationTestGroupAddIdPrefix, '' );
	var testTableId = AnimationManager.AnimationTestTableIdPrefix + animationTestGuid;

	var newGuid = AnimationManager.CreateGuid();
	var testTableObj = document.getElementById( testTableId );

	if ( testTableObj == null )
	{
		return null;
	}

	var testRow = testTableObj.insertRow( -1 );
	testRow.id = AnimationManager.AnimationTestTableRowPrefix + newGuid;
	testRow.setAttribute( "class", "rowBorder" );

	var cell1 = testRow.insertCell( 0 );
	var cell2 = testRow.insertCell( -1 );
	var cell3 = testRow.insertCell( -1 );
	var cell4 = testRow.insertCell( -1 );
	var cell5 = testRow.insertCell( -1 );

	var collapseImg = document.createElement( "img" );
	collapseImg.name = "collapse";
	collapseImg.title = $("#AnimationMgrExpandedTranslated").val();
	collapseImg.draggable = false;
	collapseImg.alt = $("#AnimationMgrExpandedTranslated").val();
	collapseImg.src = window.applicationRootName + "/fmwebapp/images/Arrow-downBl.png";
	cell1.appendChild( collapseImg );
	cell1.setAttribute( "class", "testCol1 text-left testGroupRowTd firstAnimationTestTd" );
	cell1.id = AnimationManager.AnimationTestExpandPrefix + newGuid;
	cell1.onclick = function()
	{
		return AnimationManager.CollapseTest( this );
	};

	var addImage = document.createElement( "img" );
	addImage.name = "add";
	addImage.title = $("#AnimationMgrAddPropertyTranslated").val();
	addImage.draggable = false;
	addImage.alt = $("#AnimationMgrAddPropertyTranslated").val();
	addImage.src = window.applicationRootName + "/fmwebapp/images/Insert-Row.png";
	cell2.appendChild( addImage );
	cell2.setAttribute( "class", "testCol2 text-center testGroupRowTd" );
	cell2.id = AnimationManager.AnimationTestAddPrefix + newGuid;
	cell2.onclick = function()
	{
		return AnimationManager.AddProperty( this );
	};

	var trashcanImage = document.createElement( "img" );
	trashcanImage.name = "del";
	trashcanImage.title = $("#AnimationMgrDeleteTestTranslated").val();
	trashcanImage.draggable = false;
	trashcanImage.alt = $("#AnimationMgrDeleteTestTranslated").val();
	trashcanImage.src = window.applicationRootName + "/fmwebapp/images/Trash-icon.png";
	cell3.appendChild( trashcanImage );
	cell3.id = AnimationManager.AnimationTestDeletePrefix + newGuid;
	cell3.setAttribute( "class", "testCol3 text-center testGroupRowTd" );
	cell3.onclick = function()
	{
		if ( AnimationManager.AnimationInEditMode )
		{
			return;
		}
		
		if ( confirm( 'Are you sure you wish to delete this item?' ) )
		{
			AnimationManager.NewTestDelete( testRow.id );
		}
	};

	cell4.id = AnimationManager.AnimationTestDefinitionPrefix + newGuid;
	cell4.innerHTML = testName;
	cell4.setAttribute( "class", "testCol4 text-left testGroupRowTd" );
	cell4.ondblclick = function()
	{
		AnimationManager.HandleTestEditEvent( newGuid, testTableId, testRow.id );
	};

	// Create the test preview table for column preview.
	var testPreviewTable = AnimationManager.CreateTestPreviewTable( newGuid );
	AnimationManager.CreateTestPreviewRow( newGuid, testPreviewTable );
	cell5.id = AnimationManager.animationTestPreviewColPrefix + newGuid;
	cell5.setAttribute( "class", "testCol5 text-left testGroupRowTd" );
	cell5.appendChild( testPreviewTable );

	// Create the row and column that will contain the property table.
	var testPropertiesRow = testTableObj.insertRow( -1 );
	testPropertiesRow.id = AnimationManager.AnimationTestPropertyRowPrefix + newGuid;
	testPropertiesRow.setAttribute( "class", "rowBorder" );
	var propertyCell = testPropertiesRow.insertCell( 0 );
	propertyCell.id = AnimationManager.AnimationTestPropertyTableRowTdPrefix + newGuid;
	propertyCell.setAttribute( "class", "animationTestGroupRowTd col-sm-2 col-md-2 text-left" );
	propertyCell.setAttribute( "colspan", "5" );

	// Create the test property table that will contain the properties.
	var testPropTableObj = AnimationManager.CreateTestPropertyTable( newGuid );
	propertyCell.appendChild( testPropTableObj );

	return newGuid;
};

//=============================================================
// This function will return the comparison operator dropdown.
//=============================================================
AnimationManager.GetComparisonOperatorDropdown = function( testTypeObj )
{
	var operatorObj;
	AnimationManager.ComparisonOperatorList = [];

	if (testTypeObj.Type === "TYPE_STRING")
    {
	    operatorObj = new Object();
	    operatorObj.Text = "=";
	    operatorObj.Value = "4";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);

	    operatorObj = new Object();
	    operatorObj.Text = "<>";
	    operatorObj.Value = "5";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);

	    operatorObj = new Object();
	    operatorObj.Text = "ELSE";
	    operatorObj.Value = "6";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);

	    operatorObj = new Object();
	    operatorObj.Text = "Contains";
	    operatorObj.Value = "7";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);

	    operatorObj = new Object();
	    operatorObj.Text = "Begins With";
	    operatorObj.Value = "8";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);
    }
	if (testTypeObj.Type === "TYPE_BOOLEAN" || testTypeObj.Type === "TYPE_OTHER")
	{
		operatorObj = new Object();
		operatorObj.Text = "=";
		operatorObj.Value = "4";
		AnimationManager.ComparisonOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "<>";
		operatorObj.Value = "5";
		AnimationManager.ComparisonOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "ELSE";
		operatorObj.Value = "6";
		AnimationManager.ComparisonOperatorList.push(operatorObj);
    }

	if ( testTypeObj.Type === "TYPE_CODED_VARIABLES" )
	{
		operatorObj = new Object();
		operatorObj.Text = "=";
		operatorObj.Value = "4";
		AnimationManager.ComparisonOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "<>";
		operatorObj.Value = "5";
		AnimationManager.ComparisonOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "ELSE";
		operatorObj.Value = "6";
		AnimationManager.ComparisonOperatorList.push( operatorObj );
	}

	if ( testTypeObj.Type === "TYPE_INT")
	{
		operatorObj = new Object();
		operatorObj.Text = "=";
		operatorObj.Value = "4";
		AnimationManager.ComparisonOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "<>";
		operatorObj.Value = "5";
		AnimationManager.ComparisonOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = ">";
		operatorObj.Value = "0";
		AnimationManager.ComparisonOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = ">=";
		operatorObj.Value = "1";
		AnimationManager.ComparisonOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "<";
		operatorObj.Value = "2";
		AnimationManager.ComparisonOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "<=";
		operatorObj.Value = "3";
		AnimationManager.ComparisonOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "ELSE";
		operatorObj.Value = "6";
		AnimationManager.ComparisonOperatorList.push(operatorObj);
    }
	if (testTypeObj.Type === "TYPE_DECIMAL") {
	    operatorObj = new Object();
	    operatorObj.Text = "=";
	    operatorObj.Value = "4";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);

	    operatorObj = new Object();
	    operatorObj.Text = "<>";
	    operatorObj.Value = "5";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);

	    operatorObj = new Object();
	    operatorObj.Text = ">";
	    operatorObj.Value = "0";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);

	    operatorObj = new Object();
	    operatorObj.Text = ">=";
	    operatorObj.Value = "1";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);

	    operatorObj = new Object();
	    operatorObj.Text = "<";
	    operatorObj.Value = "2";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);

	    operatorObj = new Object();
	    operatorObj.Text = "<=";
	    operatorObj.Value = "3";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);

	    operatorObj = new Object();
	    operatorObj.Text = "ELSE";
	    operatorObj.Value = "6";
	    AnimationManager.ComparisonOperatorList.push(operatorObj);
	}
	var selectList = document.createElement("select");
	selectList.id = AnimationManager.AnimationTestRowEditComparisonOperatorDropdownId;
	selectList.onchange = function()
	{
		AnimationManager.ComparisonOperatorDropdownOnChange( selectList.id );
	};

	for ( var i = 0; i < AnimationManager.ComparisonOperatorList.length; i++ )
	{
		var comparisonOpText = AnimationManager.ComparisonOperatorList[i].Text;
		var comparisonOpValue = AnimationManager.ComparisonOperatorList[i].Value;

		var option = document.createElement( "option" );
		option.value = comparisonOpValue;
		option.text = comparisonOpText;
		selectList.appendChild( option );
	}

	return selectList;
};

//====================================================================================
// This function handles the comparison operator dropdown on change event. If the
// selection is "ELSE", then the value textbox will be disabled.
//====================================================================================
AnimationManager.ComparisonOperatorDropdownOnChange = function( selectListId )
{
	$( "#TestRowEditCompareValueTextbox" ).removeAttr( "disabled" );

	if ( selectListId )
	{
		var selectedValue = $( "#" + selectListId ).val();

		// If ELSE is selected then disable the value textbox.
		if ( selectedValue === "6" )
		{
			$( "#TestRowEditCompareValueTextbox" ).val( "" );
			$( "#TestRowEditCompareValueTextbox" ).attr( "disabled", "disabled" );
		}
	}
};

//=============================================================
// This function will return the bitwise operator dropdown.
//=============================================================
AnimationManager.GetBitwiseOperatorDropdown = function()
{
	if ( AnimationManager.BitwiseOperatorList == null || AnimationManager.BitwiseOperatorList.length === 0 )
	{
		AnimationManager.BitwiseOperatorList = [];

		var operatorObj = new Object();
		operatorObj.Text = "<" + $("#AnimationMgrNoneTranslated").val() + ">";
		operatorObj.Value = "6";
		AnimationManager.BitwiseOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "AND";
		operatorObj.Value = "0";
		AnimationManager.BitwiseOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "OR";
		operatorObj.Value = "1";
		AnimationManager.BitwiseOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "NOR";
		operatorObj.Value = "3";
		AnimationManager.BitwiseOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "NAND";
		operatorObj.Value = "2";
		AnimationManager.BitwiseOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "XOR";
		operatorObj.Value = "5";
		AnimationManager.BitwiseOperatorList.push( operatorObj );

		operatorObj = new Object();
		operatorObj.Text = "XAND";
		operatorObj.Value = "4";
		AnimationManager.BitwiseOperatorList.push( operatorObj );
	}

	var selectList = document.createElement( "select" );
	selectList.id = AnimationManager.AnimationTestRowEditBitwiseDropdownId;
	selectList.onchange = function()
	{
		AnimationManager.HandleBitwiseDropdownOnChangeEvent( selectList.id );
	};

	for ( var i = 0; i < AnimationManager.BitwiseOperatorList.length; i++ )
	{
		var bitwiseText = AnimationManager.BitwiseOperatorList[i].Text;
		var bitwiseValue = AnimationManager.BitwiseOperatorList[i].Value;

		var option = document.createElement( "option" );
		option.value = bitwiseValue;
		option.text = bitwiseText;
		selectList.appendChild( option );
	}

	return selectList;
};

//==================================================================================
// This function will handle the bitwise operator dropdown on change event.
// It will enable or disable the bitwise textbox based on the selection.
//==================================================================================
AnimationManager.HandleBitwiseDropdownOnChangeEvent = function( selectId )
{
	if ( selectId == null )
	{
		return;
	}

	var bitmaskTextbox = document.getElementById( AnimationManager.AnimationTestRowEditBitMaskTextboxId );
	var selectedValue = $( "#" + selectId ).val();

	if ( selectedValue && bitmaskTextbox )
	{
		bitmaskTextbox.disabled = false;

		// NONE = 6
		if ( selectedValue === "6" )
		{
			bitmaskTextbox.value = "";
			bitmaskTextbox.disabled = true;
        }
	}
};

//===========================================================================
// This function will create the test edit table. It returns the table.
//===========================================================================
AnimationManager.CreateTestEditTable = function( testGroupType )
{
	var colCell1;
	var colCell2;
	var colCell3;
	var colCell4;
	var colCell5;
	var colCell6;
	var colCell7;

	var testEditRowTable = document.createElement( "table" );
	testEditRowTable.id = AnimationManager.AnimationTestEditTableId;
	testEditRowTable.setAttribute( "class", "animationTestsTable" );
	var header = testEditRowTable.createTHead();
	header.setAttribute( "class", "animationTableHeaderFormating" );
	var headerRow = header.insertRow( 0 );

	if ( testGroupType.Type === "TYPE_INT" )
	{
		colCell1 = document.createElement( "th" );
		colCell2 = document.createElement( "th" );
		colCell3 = document.createElement( "th" );
		colCell4 = document.createElement( "th" );
		colCell5 = document.createElement( "th" );
		colCell6 = document.createElement( "th" );
		colCell7 = document.createElement( "th" );

		headerRow.appendChild( colCell1 );
		headerRow.appendChild( colCell2 );
		headerRow.appendChild( colCell3 );
		headerRow.appendChild( colCell4 );
		headerRow.appendChild( colCell5 );
		headerRow.appendChild( colCell6 );
		headerRow.appendChild( colCell7 );

		colCell1.setAttribute( "class", "testEditIntCol1 text-left testGroupRowTd firstAnimationTestTd" );
		colCell2.setAttribute( "class", "testEditIntCol2 text-left testGroupRowTd" );
		colCell3.setAttribute( "class", "testEditIntCol3 text-left testGroupRowTd" );
		colCell4.setAttribute( "class", "testEditIntCol4 text-left testGroupRowTd" );
		colCell5.setAttribute( "class", "testEditIntCol5 text-left testGroupRowTd" );
		colCell6.setAttribute( "class", "testEditIntCol6 text-left testGroupRowTd" );
		colCell7.setAttribute( "class", "testEditIntCol7 text-left testGroupRowTd" );

		colCell1.innerHTML = $("#AnimationMgrDataTypeTranslated").val();
		colCell2.innerHTML = $("#AnimationMgrBitwiseOperatorTranslated").val();
		colCell3.innerHTML = $("#AnimationMgrBitMaskTranslated").val();
		colCell4.innerHTML = $("#AnimationMgrComparisonOperatorTranslated").val();
		colCell5.innerHTML = $("#AnimationMgrValueTranslated").val();
		colCell6.innerHTML = $("#AnimationMgrUpdateTranslated").val();
		colCell7.innerHTML = $("#AnimationMgrCancelTranslated").val();
	}
	else
	{
		colCell1 = document.createElement( "th" );
		colCell2 = document.createElement( "th" );
		colCell3 = document.createElement( "th" );
		colCell4 = document.createElement( "th" );
		colCell5 = document.createElement( "th" );

		headerRow.appendChild( colCell1 );
		headerRow.appendChild( colCell2 );
		headerRow.appendChild( colCell3 );
		headerRow.appendChild( colCell4 );
		headerRow.appendChild( colCell5 );

		colCell1.setAttribute( "class", "testEditNonIntCol1 text-left testGroupRowTd firstAnimationTestTd" );
		colCell2.setAttribute( "class", "testEditNonIntCol2 text-left testGroupRowTd" );
		colCell3.setAttribute( "class", "testEditNonIntCol3 text-left testGroupRowTd" );
		colCell4.setAttribute( "class", "testEditNonIntCol4 text-left testGroupRowTd" );
		colCell5.setAttribute( "class", "testEditNonIntCol5 text-left testGroupRowTd" );

		colCell1.innerHTML = $("#AnimationMgrDataTypeTranslated").val();
		colCell2.innerHTML = $("#AnimationMgrComparisonOperatorTranslated").val();
		colCell3.innerHTML = $("#AnimationMgrValueTranslated").val();
		colCell4.innerHTML = $("#AnimationMgrUpdateTranslated").val();
		colCell5.innerHTML = $("#AnimationMgrCancelTranslated").val();
	}

	var testEditTbody = document.createElement( "tbody" );
	testEditRowTable.appendChild( testEditTbody );

	return testEditRowTable;
};

//==========================================================================================
// This function will create a row for editing a new test.
//==========================================================================================
AnimationManager.CreateTestEditRow = function( currentItem )
{
	var animationTestGrpGuid = currentItem.id.replace( AnimationManager.AnimationTestGroupAddIdPrefix, '' );
	var testGroupType = AnimationManager.GetTestGroupType( animationTestGrpGuid );
	var testTableId = AnimationManager.AnimationTestTableIdPrefix + animationTestGrpGuid;

	var testTableObj = document.getElementById( testTableId );

	if ( testTableObj == null )
	{
		return;
	}

	AnimationManager.SetEditMode();
	AnimationManager.DisableTestGroupDataTypeDropdown( animationTestGrpGuid );

	// Create a place holder test edit row under a test group.
	var testEditHolderRow = testTableObj.insertRow( -1 );
	testEditHolderRow.id = AnimationManager.AnimationTestEditHolderRowId;
	testEditHolderRow.setAttribute( "class", "rowBorder" );

	var testEditHolderCell = testEditHolderRow.insertCell( 0 );
	testEditHolderCell.colSpan = 5;

	// Create a test edit table to contain the edit row.
	var testEditTable = AnimationManager.CreateTestEditTable( testGroupType );
	testEditHolderCell.appendChild( testEditTable );

	var testEditRow = testEditTable.insertRow( -1 );
	testEditRow.id = AnimationManager.AnimationTestEditRowId;
	testEditRow.setAttribute( "class", "rowBorder" );

	var testEditRowParms = AnimationManager.GetNewTestEditRowParameters();
	testEditRowParms.testEditRow = testEditRow;
	testEditRowParms.testGroupType = testGroupType;
	testEditRowParms.testEditHolderRowId = testEditHolderRow.id;
	testEditRowParms.currentItem = currentItem;
	testEditRowParms.mode = "ADD";
	testEditRowParms.animationTestGrpGuid = animationTestGrpGuid;

	AnimationManager.BuildTestEditRow( testEditRowParms );

	// Force the new test edit row to scroll in view.
	var row = document.getElementById( AnimationManager.AnimationTestEditHolderRowId );
	row.scrollIntoView( { block: "end", behavior: "smooth" } );
};

//=================================================================================================
// This function handles an existing test being put into edit mode.
//=================================================================================================
AnimationManager.HandleTestEditEvent = function( animationTestGuid, testTableId, testRowId )
{
	var testGrpGuid = testTableId.split( "_" )[1];
	var testGroupType = AnimationManager.GetTestGroupType( testGrpGuid );
	var testTableObj = document.getElementById( testTableId );

	if ( testTableObj == null )
	{
		return;
	}

	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	var animationTestGroup = AnimationManager.GetAnimationTestGroupByTest( animation, animationTestGuid );
	var animationTest = AnimationManager.GetAnimationTest( animationTestGroup, animationTestGuid );

	AnimationManager.SetEditMode();
	AnimationManager.DisableTestGroupDataTypeDropdown( testGrpGuid );

	var testRowBeforeEdit = document.getElementById( testRowId );
	var indexOfTestRowBeingEdited = testRowBeforeEdit.rowIndex;
	testRowBeforeEdit.classList.add( "hidden" );

	// Create a place holder test edit row under a test group.
	var testEditHolderRow = testTableObj.insertRow( indexOfTestRowBeingEdited );
	testEditHolderRow.id = AnimationManager.AnimationTestEditHolderRowId;
	testEditHolderRow.setAttribute( "class", "rowBorder" );

	// Create a test edit table to contain the edit row.
	var testEditTable = AnimationManager.CreateTestEditTable( testGroupType );
	testEditHolderRow.appendChild( testEditTable );

	var testEditRow = testEditTable.insertRow( -1 );
	testEditRow.id = AnimationManager.AnimationTestEditRowId;
	testEditRow.setAttribute( "class", "rowBorder" );

	var testEditRowParms = AnimationManager.GetNewTestEditRowParameters();
	testEditRowParms.testEditRow = testEditRow;
	testEditRowParms.testGroupType = testGroupType;
	testEditRowParms.testEditHolderRowId = testEditHolderRow.id;
	testEditRowParms.currentItem = null;
	testEditRowParms.testRowBeforeEdit = testRowBeforeEdit;
	testEditRowParms.mode = "EDIT";
	testEditRowParms.dataType = animationTestGroup.DataType;
	testEditRowParms.bitwiseOperator = animationTest.BitmaskOperator.toString();
	testEditRowParms.bitmaskStr = animationTest.BitmaskStr;
	testEditRowParms.comparisonOperator = animationTest.TestComparisonOperator.toString();
	testEditRowParms.comparisonValue = animationTest.ComparisonValue;
	testEditRowParms.animationTestGuid = animationTestGuid;
	testEditRowParms.animationTestGrpGuid = testGrpGuid;

	AnimationManager.BuildTestEditRow( testEditRowParms );
};

//=========================================================================================================
// This function will build the actual test edit row.
//=========================================================================================================
AnimationManager.BuildTestEditRow = function( testEditRowParms )
{
	var testEditRow = testEditRowParms.testEditRow;
	var testGroupType = testEditRowParms.testGroupType;
	var testEditHolderRowId = testEditRowParms.testEditHolderRowId;
	var currentItem = testEditRowParms.currentItem;
	var mode = testEditRowParms.mode;

	var cell1 = testEditRow.insertCell( 0 );
	var cell2 = testEditRow.insertCell( 1 );
	var cell3 = testEditRow.insertCell( 2 );
	var cell4 = testEditRow.insertCell( 3 );
	var cell5 = testEditRow.insertCell( 4 );

	// Add the data type first column.
	var dataTypeLabel = document.createElement( "label" );
	dataTypeLabel.innerHTML = AnimationManager.GetDataTypeDisplayName( testGroupType.Text );
	dataTypeLabel.id = AnimationManager.AnimationTestRowEditDataTypeLabelId;
	dataTypeLabel.setAttribute( "class", "testEditDataTypeLabel" );
	cell1.appendChild( dataTypeLabel );

	var updateImg = document.createElement( "img" );
	updateImg.name = "OK";
	updateImg.title = $("#AnimationMgrOkTranslated").val();
	updateImg.draggable = false;
	updateImg.alt = $("#AnimationMgrOkTranslated").val();;
	updateImg.src = window.applicationRootName + "/fmwebapp/images/update1.gif";

	var cancelImg = document.createElement( "img" );
	cancelImg.name = "Cancel";
	cancelImg.title = $("#AnimationMgrCancelTranslated").val();
	cancelImg.draggable = false;
	cancelImg.alt = $("#AnimationMgrCancelTranslated").val();
	cancelImg.src = window.applicationRootName + "/fmwebapp/images/Trash-icon.png";

	var comparisonOpDropdown;
	var compareValueTextbox;

	if ( testGroupType.Type === "TYPE_INT" )
	{
		cell1.setAttribute( "class", "testEditIntCol1 text-left testGroupRowTd firstAnimationTestTd" );

		var bitwiseDropdown = AnimationManager.GetBitwiseOperatorDropdown();
		cell2.appendChild( bitwiseDropdown );
		cell2.setAttribute( "class", "testEditIntCol2 text-left testGroupRowTd" );

		var bitMaskTextbox = document.createElement( "input" );
		bitMaskTextbox.id = AnimationManager.AnimationTestRowEditBitMaskTextboxId;
		bitMaskTextbox.disabled = true;
		cell3.appendChild( bitMaskTextbox );
		cell3.setAttribute( "class", "testEditIntCol3 text-left testGroupRowTd" );

		comparisonOpDropdown = AnimationManager.GetComparisonOperatorDropdown( testGroupType );
		cell4.appendChild( comparisonOpDropdown );
		cell4.setAttribute( "class", "testEditIntCol4 text-left testGroupRowTd" );

		compareValueTextbox = document.createElement( "input" );
		compareValueTextbox.id = AnimationManager.AnimationTestRowEditCompareValueTextboxId;
		cell5.appendChild( compareValueTextbox );
		cell5.setAttribute( "class", "testEditIntCol5 text-left testGroupRowTd" );

		var cell6 = testEditRow.insertCell( 5 );
		var cell7 = testEditRow.insertCell( 6 );

		cell6.appendChild( updateImg );
		cell6.setAttribute( "class", "testEditIntCol6 text-left testGroupRowTd" );
		cell6.id = AnimationManager.AnimationTestRowEditOkButtonId;

		cell7.appendChild( cancelImg );
		cell7.setAttribute( "class", "testEditIntCol7 text-left testGroupRowTd" );
		cell7.id = AnimationManager.AnimationTestRowEditCancelButtonId;

		if ( mode === "EDIT" )
		{
			bitMaskTextbox.value = testEditRowParms.bitmaskStr;
			compareValueTextbox.value = testEditRowParms.comparisonValue;
			bitwiseDropdown.value = testEditRowParms.bitwiseOperator;
			comparisonOpDropdown.value = testEditRowParms.comparisonOperator;

			if ( testEditRowParms.bitwiseOperator !== "6" )
			{
				bitMaskTextbox.disabled = false;
			}

			cell6.onclick = function()
			{
				return AnimationManager.ModifyTestEditingRow( testEditHolderRowId, testEditRowParms.testRowBeforeEdit, testGroupType, testEditRowParms )
			};
			cell7.onclick = function()
			{
				return AnimationManager.DeleteTestEditRow( testEditHolderRowId, mode, testEditRowParms );
			};
		}
		else
		{
			cell6.onclick = function()
			{
				return AnimationManager.UpdateTestEditRow( testEditHolderRowId, testGroupType, currentItem, testEditRowParms );
			};
			cell7.onclick = function()
			{
				return AnimationManager.DeleteTestEditRow( testEditHolderRowId, mode, testEditRowParms );
			};
		}
	}
	else
	{
		cell1.setAttribute( "class", "testEditNonIntCol1 text-left testGroupRowTd firstAnimationTestTd" );

		comparisonOpDropdown = AnimationManager.GetComparisonOperatorDropdown( testGroupType );
		cell2.appendChild( comparisonOpDropdown );
		cell2.setAttribute( "class", "testEditNonIntCol2 text-left testGroupRowTd" );

		compareValueTextbox = document.createElement( "input" );
		compareValueTextbox.id = AnimationManager.AnimationTestRowEditCompareValueTextboxId;
		cell3.appendChild( compareValueTextbox );
		cell3.setAttribute( "class", "testEditNonIntCol3	text-left testGroupRowTd" );

		cell4.appendChild( updateImg );
		cell4.setAttribute( "class", "testEditNonIntCol4 text-center testGroupRowTd" );
		cell4.id = AnimationManager.AnimationTestRowEditOkButtonId;

		cell5.appendChild( cancelImg );
		cell5.setAttribute( "class", "testEditNonIntCol5 text-left testGroupRowTd" );
		cell5.id = AnimationManager.AnimationTestRowEditCancelButtonId;

		if ( mode === "EDIT" )
		{
			compareValueTextbox.value = testEditRowParms.comparisonValue;
			comparisonOpDropdown.value = testEditRowParms.comparisonOperator;

			cell4.onclick = function()
			{
				return AnimationManager.ModifyTestEditingRow( testEditHolderRowId, testEditRowParms.testRowBeforeEdit, testGroupType, testEditRowParms )
			};
			cell5.onclick = function()
			{
				return AnimationManager.DeleteTestEditRow( testEditHolderRowId, mode, testEditRowParms );
			};
		}
		else
		{
			cell4.onclick = function()
			{
				return AnimationManager.UpdateTestEditRow( testEditHolderRowId, testGroupType, currentItem, testEditRowParms );
			};
			cell5.onclick = function()
			{
				return AnimationManager.DeleteTestEditRow( testEditHolderRowId, mode, testEditRowParms );
			};
		}
	}
};

//=====================================================================================================================
// This function will update the test editing row.
//=====================================================================================================================
AnimationManager.ModifyTestEditingRow = function( testEditHolderRowId, testEditingRow, testGroupType, testEditRowParms )
{
	var compareOpSelectId = "#" + AnimationManager.AnimationTestRowEditComparisonOperatorDropdownId;
	var compareValueId = "#" + AnimationManager.AnimationTestRowEditCompareValueTextboxId;
	var bitwiseOpSelectId = "#" + AnimationManager.AnimationTestRowEditBitwiseDropdownId;
	var bitMaskId = "#" + AnimationManager.AnimationTestRowEditBitMaskTextboxId;

	var testStr = "";
	var compareOperatorSelectionValue = $( compareOpSelectId ).val();
	var compareOperatorSelectionText = $( compareOpSelectId + " option:selected" ).text();
	var compareValue = $( compareValueId ).val();
	var bitMaskStr = "";
	var bitMask = -1;
	var bitwiseSelectionValue = $( bitwiseOpSelectId ).val();
	var bitwiseSelectionText = $( bitwiseOpSelectId + " option:selected" ).text();
	var result;

	if ( ( compareValue == null || compareValue === "" ) && compareOperatorSelectionValue !== "6" )
	{
		FMErrorAndExceptionHandling.ShowError( "Must enter a compare value." );
		return;
	}

	// Selection is ELSE, then do not prefix with the word test.
	if ( compareOperatorSelectionValue === "6" )
	{
		testStr = compareOperatorSelectionText;
	}
	else
	{
		result = AnimationManager.ComparisonValueValidator( testGroupType, compareValue );
		if ( result.Error )
		{
			FMErrorAndExceptionHandling.ShowError( result.ErrorMsg );
			return;
		}

		testStr = "Test " + compareOperatorSelectionText + " " + compareValue;
	}

	if ( testGroupType.Type === "TYPE_INT" && bitwiseSelectionValue !== "6" )
	{
		bitMaskStr = $( bitMaskId ).val();

		result = AnimationManager.BitMaskValidator( bitwiseSelectionValue, bitMaskStr );
		if ( result.Error )
		{
			// Display error message
			FMErrorAndExceptionHandling.ShowError( result.ErrorMsg );
			return;
		}

		testStr = "Test " + bitwiseSelectionText + " " + bitMaskStr + " " + testStr.replace( "Test ", "" );
		bitMask = result.NumericValue;
	}

	// Remove the test editing row.
	$( "#" + testEditHolderRowId ).remove();

	// Display the test row that has been edited.
	testEditingRow.classList.remove( "hidden" );

	var testDefColId = AnimationManager.AnimationTestDefinitionPrefix + testEditingRow.id.split( "_" )[1];
	var testDefCol = document.getElementById( testDefColId );
	testDefCol.innerHTML = testStr;

	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	var animationTestGroup = AnimationManager.GetAnimationTestGroupByTest( animation, testEditRowParms.animationTestGuid );
	var animationTest = AnimationManager.GetAnimationTest( animationTestGroup, testEditRowParms.animationTestGuid );

	animationTest.TestComparisonOperator = parseInt( compareOperatorSelectionValue );
	animationTest.BitmaskOperator = parseInt( bitwiseSelectionValue );
	animationTest.BitmaskStr = bitMaskStr;
	animationTest.Bitmask = bitMask;
	animationTest.ComparisonValue = compareValue;

	if ( animationTest.BitmaskOperator < 0 ||
        animationTest.BitmaskOperator > 6 ||
        isNaN(animationTest.BitmaskOperator))
	{
		animationTest.BitmaskOperator = 6;
		animationTest.BitmaskStr = "";
		animationTest.Bitmask = -1;
	}

	AnimationManager.SetAnimationManagerModel( model );
	AnimationManager.ClearEditMode();
	AnimationManager.EnableTestGroupDataTypeDropdown( testEditRowParms.animationTestGrpGuid );
	AnimationManager.UpdateTestGroupDataTypeEditMode( model, animationTestGroup );
	AnimationManager.ApplyAlternatingRowColors();
};

//==========================================================================
// This function will create a test property table to be associated to a
// test.
//==========================================================================
AnimationManager.CreateTestPropertyTable = function( newGuid )
{
	// Prefix: AnimationTestPropertyTableId_
	var testPropertyTableId = AnimationManager.AnimationTestPropertyTablePrefix + newGuid;

	var testPropertyTable = document.createElement( "table" );
	testPropertyTable.id = testPropertyTableId;
	testPropertyTable.setAttribute( "class", "animationTestsTable" );
	testPropertyTable.setAttribute( "style", "width: 100%" );
	var header = testPropertyTable.createTHead();
	header.setAttribute( "class", "animationTableHeaderFormating" );
	var headerRow = header.insertRow( 0 );

	var colCell1 = document.createElement( "th" );
	var colCell2 = document.createElement( "th" );
	var colCell3 = document.createElement( "th" );
	var colCell4 = document.createElement( "th" );
	var colCell5 = document.createElement( "th" );
	var colCell6 = document.createElement( "th" );

	headerRow.appendChild( colCell1 );
	headerRow.appendChild( colCell2 );
	headerRow.appendChild( colCell3 );
	headerRow.appendChild( colCell4 );
	headerRow.appendChild( colCell5 );
	headerRow.appendChild( colCell6 );

	colCell1.innerHTML = $("#AnimationMgrDeleteTranslated").val();
	colCell2.innerHTML = $("#AnimationMgrPropertyTranslated").val();
	colCell3.innerHTML = $("#AnimationMgrValueTranslated").val();
	colCell4.innerHTML = "";
	colCell5.innerHTML = "";
	colCell6.innerHTML = "";

	colCell1.setAttribute( "class", "text-left testGroupRowTd firstAnimationTestPropertyTd" );
	colCell2.setAttribute( "class", "text-left testGroupRowTd testPropertyHeaderCol2" );
	colCell3.setAttribute( "class", "text-left testGroupRowTd testPropertyHeaderCol3" );
	colCell4.setAttribute( "class", "text-left testGroupRowTd" );
	colCell5.setAttribute( "class", "text-left testGroupRowTd" );
	colCell6.setAttribute( "class", "text-left testGroupRowTd" );

	var testPropertyTbody = document.createElement( "tbody" );
	testPropertyTable.appendChild( testPropertyTbody );

	return testPropertyTable;
};

//==========================================================================
// This function will remove the test editing row.
//==========================================================================
AnimationManager.DeleteTestEditRow = function( testEditHolderRowId, mode, testEditRowParms )
{
	$( "#" + testEditHolderRowId ).remove();

	if ( mode === "EDIT" )
	{
		testEditRowParms.testRowBeforeEdit.classList.remove( "hidden" );
	}

	AnimationManager.ClearEditMode();
	AnimationManager.EnableTestGroupDataTypeDropdown( testEditRowParms.animationTestGrpGuid );
};

//==========================================================================
// This function will update the test editing row.
//==========================================================================
AnimationManager.UpdateTestEditRow = function( testEditHolderRowId, testGroupType, currentItem, testEditRowParms )
{
	var compareOpSelectId = "#" + AnimationManager.AnimationTestRowEditComparisonOperatorDropdownId;
	var compareValueId = "#" + AnimationManager.AnimationTestRowEditCompareValueTextboxId;
	var bitwiseOpSelectId = "#" + AnimationManager.AnimationTestRowEditBitwiseDropdownId;
	var bitMaskId = "#" + AnimationManager.AnimationTestRowEditBitMaskTextboxId;

	var testStr = "";
	var compareOperatorSelectionValue = $( compareOpSelectId ).val();
	var compareOperatorSelectionText = $( compareOpSelectId + " option:selected" ).text();
	var compareValue = $( compareValueId ).val();
	var bitMaskStr = "";
	var bitMask = -1;
	var bitwiseSelectionValue = $( bitwiseOpSelectId ).val();
	var bitwiseSelectionText = $( bitwiseOpSelectId + " option:selected" ).text();
	var result;

	if ( ( compareValue == null || compareValue === "" ) && compareOperatorSelectionValue !== "6" )
	{
		FMErrorAndExceptionHandling.ShowError( "Must enter a compare value." );
		return;
	}

	// Selection is ELSE, then do not prefix with the word test.
	if ( compareOperatorSelectionValue === "6" )
	{
		testStr = compareOperatorSelectionText;
	}
	else
	{
		result = AnimationManager.ComparisonValueValidator( testGroupType, compareValue );
		if ( result.Error )
		{
			FMErrorAndExceptionHandling.ShowError( result.ErrorMsg );
			return;
		}

		testStr = "Test " + compareOperatorSelectionText + " " + compareValue;
	}

	if ( testGroupType.Type === "TYPE_INT" && bitwiseSelectionValue !== "6" )
	{
		bitMaskStr = $( bitMaskId ).val();

		result = AnimationManager.BitMaskValidator( bitwiseSelectionValue, bitMaskStr );
		if ( result.Error )
		{
			// Display error message
			FMErrorAndExceptionHandling.ShowError( result.ErrorMsg );
			return;
		}

		testStr = "Test " + bitwiseSelectionText + " " + bitMaskStr + " " + testStr.replace( "Test ", "" );
		bitMask = result.NumericValue;
	}

	// Remove the test editing row.
	$( "#" + testEditHolderRowId ).remove();

	var newGuid = AnimationManager.CreateNewTest( currentItem, testStr );
	var model = AnimationManager.GetAnimationManagerModel();

	var newAnimationTest = {
		AnimationTestGuid: newGuid,
		TestComparisonOperator: parseInt( compareOperatorSelectionValue ),
		BitmaskOperator: parseInt( bitwiseSelectionValue ),
		BitmaskStr: bitMaskStr,
		Bitmask: bitMask,
		ComparisonValue: compareValue,
		PropertyList: []
	};

	if ( newAnimationTest.BitmaskOperator> 6 ||
        newAnimationTest.BitmaskOperator < 0 ||
        isNaN(newAnimationTest.BitmaskOperator))
	{
		newAnimationTest.BitmaskOperator = 6;
		newAnimationTest.BitmaskStr = "";
		newAnimationTest.Bitmask = -1;
	}

	var animation = AnimationManager.GetCurrentAnimation( model );
	var currentTestGrpGuid = currentItem.id.replace( AnimationManager.AnimationTestGroupAddIdPrefix, '' );
	var parentTestGrpIndex = AnimationManager.GetIndexOfAnimationTestGroup( animation, currentTestGrpGuid );

	if ( parentTestGrpIndex >= 0 )
	{
		animation.AnimationTestGroups[parentTestGrpIndex].TestList.push( newAnimationTest );
		AnimationManager.SetAnimationManagerModel( model );
		AnimationManager.UpdateTestGroupDataTypeEditMode( model, animation.AnimationTestGroups[parentTestGrpIndex] );
		AnimationManager.ApplyAlternatingRowColors();
	}

	AnimationManager.ClearEditMode();
	AnimationManager.EnableTestGroupDataTypeDropdown( testEditRowParms.animationTestGrpGuid );
};

//================================================================
// This function handles the delete new test row event.
//================================================================
AnimationManager.NewTestDelete = function( testRowId )
{
	var theGuid = testRowId.replace( AnimationManager.AnimationTestTableRowPrefix, "" );
	var testRowPropertyId = AnimationManager.AnimationTestPropertyRowPrefix + theGuid;

	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	var parentTestGroup = AnimationManager.GetAnimationTestGroupForAnimationTestGuid( animation, theGuid );

	if ( parentTestGroup )
	{
		var testIndex = AnimationManager.GetIndexOfAnimationTest( parentTestGroup, theGuid );
		if ( testIndex >= 0 )
		{
			parentTestGroup.TestList.splice( testIndex, 1 );
			AnimationManager.SetAnimationManagerModel( model );

			$( "#" + testRowId ).remove();
			$( "#" + testRowPropertyId ).remove();
		}
	}
	AnimationManager.UpdateTestGroupDataTypeEditMode( model, parentTestGroup );
	AnimationManager.ClearEditMode();
	AnimationManager.ApplyAlternatingRowColors();
};

//=================================================================================================
// This function will return the test group data type.
//=================================================================================================
AnimationManager.GetTestGroupType = function( animationTestGrpGuid )
{
	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	var animationTestGrp = AnimationManager.GetAnimationTestGroup( animation, animationTestGrpGuid );

	var testGrpType = new Object();
	testGrpType.Type = "TYPE_OTHER";
	testGrpType.Text = animationTestGrp.DataType;

	if ( animationTestGrp.DataType === "System.Int16"
		|| animationTestGrp.DataType === "System.UInt16"
		|| animationTestGrp.DataType === "System.Int32"
		|| animationTestGrp.DataType === "System.UInt32"
		|| animationTestGrp.DataType === "System.Int64"
		|| animationTestGrp.DataType === "System.UInt64"
		|| animationTestGrp.DataType === "FMBusinessObjects.DataObjects.DeviceAlarmMapReference")
	{
		testGrpType.Type = "TYPE_INT";
	}

	if ( animationTestGrp.DataType === "System.String" )
	{
		testGrpType.Type = "TYPE_STRING";
	}

	if ( animationTestGrp.DataType === "System.Boolean" )
	{
		testGrpType.Type = "TYPE_BOOLEAN";
	}

	if ( animationTestGrp.DataType === "System.Double"
		|| animationTestGrp.DataType === "System.Single" )
	{
		testGrpType.Type = "TYPE_DECIMAL";
	}

	if ( animationTestGrp.DataType === "FMBusinessObjects.DataObjects.CodedVariables.TankCommands"
		|| animationTestGrp.DataType === "FMBusinessObjects.DataObjects.CodedVariables.TransferModes"
		|| animationTestGrp.DataType === "FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode"
		|| animationTestGrp.DataType === "FMBusinessObjects.DataObjects.CodedVariables.VolumeTransferMode"
		|| animationTestGrp.DataType === "FMBusinessObjects.DataObjects.CodedVariables.TankStatuses"
		|| animationTestGrp.DataType === "FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses"
		|| animationTestGrp.DataType === "FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode"
		|| animationTestGrp.DataType === "FMBusinessObjects.DataObjects.CodedVariables.MovementNodeCommand")
	{
		testGrpType.Type = "TYPE_CODED_VARIABLES";
	}

	return testGrpType;
};
//====================================================  End section for adding Tests =================================================

//====================================================  This section is for adding Test Properties =================================================
//========================================================================
// This function handles the add property event for a given test.
//========================================================================
AnimationManager.AddProperty = function( currentItem )  //bds
{
	var animationTestGuid = currentItem.id.replace( AnimationManager.AnimationTestAddPrefix, '' );
	var model = AnimationManager.GetAnimationManagerModel();
	var propertyDropdownList = AnimationManager.FindUnusedProperties( animationTestGuid, model );

	if ( AnimationManager.AnimationInEditMode === false )
	{
	    var stripedId = currentItem.id.split('_');
	    if (stripedId.length > 1)
	    {
	        var animationTestExpandId = "AnimationTestExpandId_" + stripedId[1];
	        var buttonClick = document.getElementById(animationTestExpandId);
	        var buttonStatus = buttonClick.innerHTML.split("\"");
	        if (buttonStatus.length > 1)
	        {
	            if (buttonStatus[1] == "expand")
	            {
	                buttonClick.click();
	            }
	        }
	    }
		AnimationManager.AddTemporaryPropertyRow( currentItem, propertyDropdownList );
	}
	AnimationManager.ApplyAlternatingRowColors();
};

//========================================================================================
// This function creates a temporary property row with a dropdown that allows the user
// to select a property.
//========================================================================================
AnimationManager.AddTemporaryPropertyRow = function( currentItem, propertyDropdownList )
{
	if ( propertyDropdownList == null || propertyDropdownList.length === 0 )
	{
		return;
	}

	// Find the Animation Test Property Table matching the GUID.
	// Table ID prefix is: AnimationTestPropertyTableId_
	var animationTestPropertyTableGuid = currentItem.id.replace( AnimationManager.AnimationTestAddPrefix, '' );
	var animationTestPropertyTableId = AnimationManager.AnimationTestPropertyTablePrefix + animationTestPropertyTableGuid;

	var testPropertyTable = document.getElementById( animationTestPropertyTableId );
	if ( testPropertyTable )
	{
		var row = testPropertyTable.insertRow( 1 );
		var cell1 = row.insertCell( 0 );
		cell1.colSpan = 6;
		cell1.setAttribute( "class", "text-left testGroupRowTd firstAnimationTestPropertyTd" );
		cell1.setAttribute( 'style', 'width: 100%' );

		row.id = AnimationManager.AnimationTemporaryRowPrefix + animationTestPropertyTableGuid;
		//row.setAttribute("class", "col-sm-2 col-md-2 text-left testGroupRowTd firstAnimationTestPropertyTd");

		var selectList = document.createElement( "select" );
		selectList.id = "TemporarySelect_" + animationTestPropertyTableGuid;
		selectList.setAttribute( "class", "dropdownAndTextboxColor" );
		selectList.onchange = function()
		{
			AnimationManager.AddTemporaryPropertyRowOnchange( selectList.id, testPropertyTable );
		};
		cell1.appendChild( selectList );

		var option = document.createElement( "option" );
		option.value = "MakeSelection";
		option.text = "<" + $("#AnimationMgrMakeSelectionTranslated").val() + ">";
		selectList.appendChild(option);

		option = document.createElement( "option" );
		option.value = "Cancel";
		option.text = "<" + $("#AnimationMgrCancelInsertTranslated").val() + ">";;
		selectList.appendChild(option);

		for ( var i = 0; i < propertyDropdownList.length; i++ )
		{
			option = document.createElement( "option" );
			option.value = propertyDropdownList[i].LookupName;
			option.text = propertyDropdownList[i].Name;
			selectList.appendChild( option );
		}

		selectList.selectedIndex = "0";
		AnimationManager.SetEditMode();
	}
};

//========================================================================================
// This function handles the on change event for a property selection. Based on the
// selection, it will create a new property row for the selected test.
//========================================================================================
AnimationManager.AddTemporaryPropertyRowOnchange = function( selectedId, testTableObj )
{
	var selectedValue = $( "#" + selectedId ).val();
	var selectedText = $( "#" + selectedId + " option:selected" ).text();
	testTableObj.deleteRow( 1 );

	AnimationManager.ClearEditMode();

	if ( selectedValue === "Cancel" )
	{
		AnimationManager.ApplyAlternatingRowColors();
		return;
	}
	var row = testTableObj.insertRow( 1 );
	var cell1 = row.insertCell( 0 );
	var cell2 = row.insertCell( 1 );
	var cell3 = row.insertCell( 2 );
	var cell4;
	var cell5;
	var cell6;
	var visualState;

	var testTableGuid = testTableObj.id.split( "_" )[1];
	var newGuid = AnimationManager.CreateGuid();
	row.id = AnimationManager.AnimationTestPropertyTableRowPrefix + newGuid;
	row.setAttribute( "class", "rowBorder" );

	// Create the property object for adding to the model.
	var animationProperty = AnimationManager.CreatePropertyModel();
	animationProperty.AnimationPropertyGuid = newGuid;
	animationProperty.Name = selectedText;
	animationProperty.LookupName = selectedValue;

    // set the gols property name
	if (selectedValue === "BACKGROUND_COLOR")
	{
	    animationProperty.gojsPropertyName = "bgcolor";
	}
	else if (selectedValue === "FILL_COLOR") {
	    animationProperty.gojsPropertyName = "color";
	}
	else if (selectedValue === "LINE_COLOR")
	{
	    animationProperty.gojsPropertyName = "lineStroke";
	}
    else if (selectedValue === "TEXT")
	{
	    animationProperty.gojsPropertyName = "text";
	}
	else if (selectedValue === "TEXT_COLOR")
	{
	    animationProperty.gojsPropertyName = "stroke";

	}
	else if (selectedValue === "VISIBILITY")
	{
	    animationProperty.gojsPropertyName = "visible";

	}
	// Trashcan cell
	AnimationManager.CreateTrashImage( cell1, newGuid, row.id );
	cell1.setAttribute( "class", "testPropertyCol1 text-right testGroupRowTd firstAnimationTestPropertyTd" );

	// Property Name cell
	cell2.innerHTML = selectedText;
	cell2.id = AnimationManager.AnimationTestPropertyNamePrefix + newGuid;
	cell2.setAttribute( "class", "testPropertyCol2 text-left testGroupRowTd" );

	if ( selectedValue === "TEXT" )
	{
		cell3.setAttribute( "class", "testPropertyTextCol3 text-right testGroupRowTd" );
	}
	else
	{
		cell4 = row.insertCell( 3 );
		cell5 = row.insertCell( 4 );
		cell6 = row.insertCell( 5 );
	}

	if ( selectedValue === "BACKGROUND_COLOR"
		|| selectedValue === "LINE_COLOR"
		|| selectedValue === "FILL_COLOR" )
	{
		AnimationManager.CreateColorPickerHtml( cell3, 1, newGuid, "STANDARD_COLOR", "testPropertyStandardColorCol3" );
		AnimationManager.CreateSpectrum();
		AnimationManager.CreateTransparencyTextbox( cell4, AnimationManager.AnimationTestPropertyTransparencyPrefix1 + newGuid, "testPropertyStandardColorCol4" );

		AnimationManager.CreateColorPickerHtml( cell5, 2, newGuid, "STANDARD_COLOR", "testPropertyStandardColorCol5" );
		AnimationManager.CreateSpectrum();
		AnimationManager.CreateTransparencyTextbox( cell6, AnimationManager.AnimationTestPropertyTransparencyPrefix2 + newGuid, "testPropertyStandardColorCol6" );

		// Set colors to empty strings and transparency to zero..
		visualState = AnimationManager.CreateVisualStateModel();
		visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
		visualState.Value = "";
		animationProperty.VisualStates.push( visualState );

		visualState = AnimationManager.CreateVisualStateModel();
		visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
		visualState.Value = "0";
		animationProperty.VisualStates.push( visualState );

		visualState = AnimationManager.CreateVisualStateModel();
		visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
		visualState.Value = "";
		animationProperty.VisualStates.push( visualState );

		visualState = AnimationManager.CreateVisualStateModel();
		visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
		visualState.Value = "0";
		animationProperty.VisualStates.push( visualState );

		// Update the test preview row.
		AnimationManager.UpdateTestPreview( testTableGuid, animationProperty.LookupName, animationProperty.VisualStates );
	}

	if ( selectedValue === "TEXT_COLOR" )
	{
		AnimationManager.CreateColorPickerHtml( cell3, 1, newGuid, "TEXT_COLOR", "testPropertyTextColorCol3" );
		AnimationManager.CreateSpectrum();

		AnimationManager.CreateColorPickerHtml( cell4, 2, newGuid, "TEXT_COLOR", "testPropertyTextColorCol4" );
		AnimationManager.CreateSpectrum();

		// Set colors to empty strings.
		visualState = AnimationManager.CreateVisualStateModel();
		visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
		visualState.Value = "";
		animationProperty.VisualStates.push( visualState );

		visualState = AnimationManager.CreateVisualStateModel();
		visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
		visualState.Value = 0;
		animationProperty.VisualStates.push( visualState );

		visualState = AnimationManager.CreateVisualStateModel();
		visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
		visualState.Value = "";
		animationProperty.VisualStates.push(visualState);

		visualState = AnimationManager.CreateVisualStateModel();
		visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
		visualState.Value = 0;
		animationProperty.VisualStates.push(visualState);

	    // Update the test preview row.
		AnimationManager.UpdateTestPreview( testTableGuid, animationProperty.LookupName, animationProperty.VisualStates );
	}

	if ( selectedValue === "TEXT" )
	{
		var textBox = document.createElement( "input" );
		textBox.id = AnimationManager.AnimationTestPropertyTextBoxPrefix + newGuid;
		textBox.setAttribute( "class", "dropdownAndTextboxColor testPropertyTB" );
		textBox.onblur = function()
		{
			AnimationManager.HandlePropertyTextboxOnBlur( textBox.id );
		};
		cell3.appendChild( textBox );
		cell3.id = AnimationManager.AnimationTestPropertyValuePrefix + newGuid;
		cell3.setAttribute( "class", "testPropertyTextCol3 text-left testGroupRowTd" );
		cell3.setAttribute( "colspan", "4" );

		visualState = AnimationManager.CreateVisualStateModel();
		visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
		visualState.Value = textBox.value;
		animationProperty.VisualStates.push(visualState);

	    // Update the test preview row.
		AnimationManager.UpdateTestPreview(testTableGuid, animationProperty.LookupName, animationProperty.VisualStates);
    }

	if ( selectedValue === "VISIBILITY" )
	{
		var visibilityToggle = document.createElement( "img" );
		visibilityToggle.name = "ToggleOn";
		visibilityToggle.title = $("#AnimationMgrToggleOnTranslated").val();
		visibilityToggle.draggable = false;
		visibilityToggle.alt = $("#AnimationMgrToggleOnTranslated").val();
		visibilityToggle.src = window.applicationRootName + "/fmwebapp/images/On.png";
		visibilityToggle.id = AnimationManager.AnimationTestPropertyVisibityPrefix + newGuid;

		cell3.appendChild( visibilityToggle );
		cell3.id = AnimationManager.AnimationTestPropertyValuePrefix + newGuid;
		cell3.setAttribute( "class", "testPropertyStandardColorCol3 text-left testGroupRowTd" );
		cell3.onclick = function()
		{
			AnimationManager.HandleVisibilityPropertyOnClick( visibilityToggle.id );
		};

		visualState = AnimationManager.CreateVisualStateModel();
		visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
		visualState.Value = "ON";
		animationProperty.VisualStates.push( visualState );

		// Update the test preview row.
		AnimationManager.UpdateTestPreview( testTableGuid, animationProperty.LookupName, animationProperty.VisualStates );
	}

    // Make sure the test preview table is visible.
	AnimationManager.MakeTextPreviewTableVisible( testTableGuid );

	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );

	var testGuid = testTableObj.id.split( "_" )[1];

	var testGroup = AnimationManager.GetAnimationTestGroupForAnimationTestGuid( animation, testGuid );
	var test = AnimationManager.GetAnimationTest( testGroup, testGuid );
	test.PropertyList.push( animationProperty );

	AnimationManager.SetAnimationManagerModel( model );
	AnimationManager.ApplyAlternatingRowColors();
};

//=========================================================
// Create and return a new property model object
//=========================================================
AnimationManager.CreatePropertyModel = function()
{
	// Create the property object for adding to the model.
	var animationProperty = new Object();
	animationProperty.AnimationPropertyGuid = "";
	animationProperty.Name = "";
	animationProperty.LookupName = "";
	animationProperty.Value = "";
	animationProperty.VisualStates = [];

	return animationProperty;
};

//=========================================================
// Create and return a new property visual state model object
//=========================================================
AnimationManager.CreateVisualStateModel = function()
{
	// Create the property object for adding to the model.
	var visualState = new Object();
	visualState.AnimationPropertyVisualStateGuid = "";
	visualState.Value = "";

	return visualState;
};

//============================================================================
// This function will create the trashcan image and append it to the given
// cell.
//============================================================================
AnimationManager.CreateTrashImage = function( trashCanCell, newGuid, rowId )
{
	var trashcanImage = document.createElement( "img" );
	trashcanImage.name = "del";
	trashcanImage.title = $("#AnimationMgrDeletePropertyTranslated").val();
	trashcanImage.draggable = false;
	trashcanImage.alt = $("#AnimationMgrDeletePropertyTranslated").val();
	trashcanImage.src = window.applicationRootName + "/fmwebapp/images/Trash-icon.png";

	trashCanCell.appendChild( trashcanImage );
	trashCanCell.id = AnimationManager.AnimationPropertyDeletePrefix + newGuid;
	trashCanCell.onclick = function()
	{
		if ( AnimationManager.AnimationInEditMode )
		{
			return;
		}

		if ( confirm( 'Are you sure you wish to delete this item?' ) )
		{
			AnimationManager.NewPropertyDelete( rowId );
		}
	};
};

//========================================================================================
// This function is a helper to create the color picker HTML.
//========================================================================================
AnimationManager.CreateColorPickerHtml = function( cellObj, idNumber, newGuid, colorType, additionalClass )
{
	var showAlpha = "true";

	if ( colorType === "TEXT_COLOR" )
	{
		showAlpha = "false";
	}

	var animationTestPropertyColorButtonId = "AnimationTestPropertyColorButton" + idNumber + "Id_" + newGuid;
	var previewDiv = document.createElement( "div" );
	previewDiv.setAttribute( "class", "sp-preview" );

	var spectrumDiv = document.createElement( "div" );
	spectrumDiv.setAttribute( "class", "sp-preview-inner" );
	spectrumDiv.id = animationTestPropertyColorButtonId;
	spectrumDiv.style.backgroundColor = "transparent";

	previewDiv.appendChild( spectrumDiv );
	cellObj.appendChild( previewDiv );
	cellObj.id = idNumber + "|" + newGuid.toString();
	cellObj.setAttribute( "class", additionalClass + " text-left testGroupRowTd full" );
	cellObj.setAttribute( "type", "text" );
	cellObj.setAttribute( "data-show-alpha", showAlpha );
};

//========================================================================
// This function is a helper to create a transparency textbox.
//========================================================================
AnimationManager.CreateTransparencyTextbox = function( cellObj, id, additionalClass )
{
	var transparencyTextbox = document.createElement( "input" );
	transparencyTextbox.id = id;
	transparencyTextbox.setAttribute( "class", "animationTransPercentTextbox" );
	transparencyTextbox.value = "100";
	transparencyTextbox.onblur = function()
	{
		AnimationManager.HandleTransparencyTextboxOnBlur( transparencyTextbox.id );
	};
	transparencyTextbox.onfocus = function()
	{
		AnimationManager.HandleTransparencyTextboxOnFocus( transparencyTextbox.id );
	};
	cellObj.appendChild( transparencyTextbox );
	cellObj.setAttribute( "class", additionalClass + " text-left testGroupRowTd" );

	var percentLabel = document.createElement( "label" );
	percentLabel.innerHTML = "&nbsp;%";
	percentLabel.setAttribute( "class", "animationPercentLabel" );
	cellObj.appendChild( percentLabel );
};

//============================================================================
// This function will return a list of unused properties for a given animation
// test.
//============================================================================
AnimationManager.FindUnusedProperties = function( animationTestGuid, model )
{
	var possibleProperties = [];
	var inUsePropertiesList = [];

	var propertySelection = new Object();
	propertySelection.Name = $("#AnimationMgrTextTranslated").val();
	propertySelection.LookupName = "TEXT";
	possibleProperties.push( propertySelection );

	propertySelection = new Object();
	propertySelection.Name = $("#AnimationMgrTextColorTranslated").val();
	propertySelection.LookupName = "TEXT_COLOR";
	possibleProperties.push( propertySelection );

	propertySelection = new Object();
	propertySelection.Name = $("#AnimationMgrBackgroundColorTranslated").val();
	propertySelection.LookupName = "BACKGROUND_COLOR";
	possibleProperties.push( propertySelection );

	propertySelection = new Object();
	propertySelection.Name = $("#AnimationMgrLineColorTranslated").val();
	propertySelection.LookupName = "LINE_COLOR";
	possibleProperties.push( propertySelection );

	propertySelection = new Object();
	propertySelection.Name = $("#AnimationMgrFillColorTranslated").val();
	propertySelection.LookupName = "FILL_COLOR";
	possibleProperties.push( propertySelection );

	propertySelection = new Object();
	propertySelection.Name = $("#AnimationMgrVisibilityTranslated").val();
	propertySelection.LookupName = "VISIBILITY";
	possibleProperties.push( propertySelection );

	var property;
	var animation = AnimationManager.GetCurrentAnimation( model );
	var currentTestGroup = AnimationManager.GetAnimationTestGroupByTest( animation, animationTestGuid );

	// Get a list of all properties being used by other test groups.
	if ( animation.AnimationTestGroups != null && animation.AnimationTestGroups.length > 0 )
	{
		for ( var nextTestGrpIndex = 0; nextTestGrpIndex < animation.AnimationTestGroups.length; nextTestGrpIndex++ )
		{
			var testGroup = animation.AnimationTestGroups[nextTestGrpIndex];
			if ( currentTestGroup.AnimationTestGroupGuid !== testGroup.AnimationTestGroupGuid )
			{
				for ( var nextTestIndex = 0; nextTestIndex < testGroup.TestList.length; nextTestIndex++ )
				{
					var test = testGroup.TestList[nextTestIndex];
					for ( var nextPropertyIndex = 0; nextPropertyIndex < test.PropertyList.length; nextPropertyIndex++ )
					{
						property = test.PropertyList[nextPropertyIndex];
						AnimationManager.AddItemToList( inUsePropertiesList, property );
					}
				}
			}
		}
	}

	// Get all properties for the current test.
	var testObj = AnimationManager.GetAnimationTest( currentTestGroup, animationTestGuid );

	if ( testObj )
	{
		for ( var nextPropIndx = 0; nextPropIndx < testObj.PropertyList.length; nextPropIndx++ )
		{
			property = testObj.PropertyList[nextPropIndx];
			AnimationManager.AddItemToList( inUsePropertiesList, property );
		}
	}

	for ( var nextIndex = 0; nextIndex < inUsePropertiesList.length; nextIndex++ )
	{
		property = inUsePropertiesList[nextIndex];
		for ( var nextSelIndex = 0; nextSelIndex < possibleProperties.length; nextSelIndex++ )
		{
			var selectItem = possibleProperties[nextSelIndex];
			if ( selectItem.LookupName === property.LookupName )
			{
				possibleProperties.splice( nextSelIndex, 1 );
				break;
			}
		}
	}

	return possibleProperties;
};

//=====================================================================
// This function will add a property item to the list only if it
// does not already exists.
//=====================================================================
AnimationManager.AddItemToList = function( list, item )
{
	for ( var nextItem = 0; nextItem < list.length; nextItem++ )
	{
		if ( list[nextItem].LookupName === item.LookupName )
		{
			return;
		}
	}

	list.push( item );
};

//=====================================================================
// This function will delete a newly created row that is not present
// at the server.
//=====================================================================
AnimationManager.NewPropertyDelete = function( rowId )
{
	var parts = rowId.split( "_" );
	var animationPropertyGuid = parts[1];

	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	var animationTest = AnimationManager.GetAnimationTestByProperty( animation, animationPropertyGuid );

	if ( animationTest )
	{
		var propertyIndex = AnimationManager.GetIndexOfAnimationTestProperty( animationTest, animationPropertyGuid );
		var property = AnimationManager.GetAnimationTestProperty( animationTest, animationPropertyGuid );

		if ( propertyIndex >= 0 )
		{
			// Remove the assoicated test preview.
			var hdrId = null;
			var itemId = null;

			if ( property.LookupName === "BACKGROUND_COLOR" )
			{
				hdrId = AnimationManager.animationTestPreviewHdBkColorPrefix + animationTest.AnimationTestGuid;
				itemId = AnimationManager.animationTestPreviewBkColorPrefix + animationTest.AnimationTestGuid;
			}

			if ( property.LookupName === "LINE_COLOR" )
			{
				hdrId = AnimationManager.animationTestPreviewHdLnColorPrefix + animationTest.AnimationTestGuid;
				itemId = AnimationManager.animationTestPreviewLnColorPrefix + animationTest.AnimationTestGuid;
			}

			if ( property.LookupName === "FILL_COLOR" )
			{
				hdrId = AnimationManager.animationTestPreviewHdFlColorPrefix + animationTest.AnimationTestGuid;
				itemId = AnimationManager.animationTestPreviewFlColorPrefix + animationTest.AnimationTestGuid;
			}

			if ( property.LookupName === "TEXT_COLOR" )
			{
				hdrId = AnimationManager.animationTestPreviewHdTxColorPrefix + animationTest.AnimationTestGuid;
				itemId = AnimationManager.animationTestPreviewTxColorPrefix + animationTest.AnimationTestGuid;
			}

			if ( property.LookupName === "VISIBILITY" )
			{
				hdrId = AnimationManager.animationTestPreviewHdVsColorPrefix + animationTest.AnimationTestGuid;
				itemId = AnimationManager.animationTestPreviewVsColorPrefix + animationTest.AnimationTestGuid;
			}

			if (property.LookupName === "TEXT") {
			    hdrId = AnimationManager.animationTestPreviewHdTextColorPrefix + animationTest.AnimationTestGuid;
			    itemId = AnimationManager.animationTestPreviewTextColorPrefix + animationTest.AnimationTestGuid;
			}

			animationTest.PropertyList.splice(propertyIndex, 1);
			AnimationManager.SetAnimationManagerModel( model );
			$( "#" + rowId ).remove();

			// Make property item preview hidden.
			AnimationManager.MakeTestPreviewItemHidden( hdrId );
			AnimationManager.MakeTestPreviewItemHidden( itemId );
			AnimationManager.ApplyAlternatingRowColors();
		}
	}
};

//======================================================================
// This function will hanble the visibility on/off on click event.
//======================================================================
AnimationManager.HandleVisibilityPropertyOnClick = function( visibilityImgId )
{
	var visibilityObj = document.getElementById( visibilityImgId );

	if ( visibilityObj )
	{
		var animationPropertyGuid = visibilityImgId.replace( AnimationManager.AnimationTestPropertyVisibityPrefix, "" );
		var model = AnimationManager.GetAnimationManagerModel();
		var animation = AnimationManager.GetCurrentAnimation( model );

		var animationTest = AnimationManager.GetAnimationTestByProperty( animation, animationPropertyGuid );
		var currentProperty = AnimationManager.GetAnimationTestProperty( animationTest, animationPropertyGuid );

		if ( currentProperty )
		{
			if ( visibilityObj.name === "ToggleOn" )
			{
				visibilityObj.name = "ToggleOff";
				visibilityObj.title = $("#AnimationMgrToggleOffTranslated").val();
				visibilityObj.draggable = false;
				visibilityObj.alt = $("#AnimationMgrToggleOffTranslated").val();
				visibilityObj.src = window.applicationRootName + "/fmwebapp/images/Off.png";
				currentProperty.VisualStates[0].Value = "OFF";
			}
			else
			{
				visibilityObj.name = "ToggleOn";
				visibilityObj.title = $("#AnimationMgrToggleOnTranslated").val();
				visibilityObj.draggable = false;
				visibilityObj.alt = $("#AnimationMgrToggleOnTranslated").val();
				visibilityObj.src = window.applicationRootName + "/fmwebapp/images/On.png";
				currentProperty.VisualStates[0].Value = "ON";
			}
		}

		AnimationManager.SetAnimationManagerModel( model );
		AnimationManager.UpdateTestPreview( animationTest.AnimationTestGuid, currentProperty.LookupName, currentProperty.VisualStates );
		AnimationManager.ApplyAlternatingRowColors();
	}
};

//==========================================================================
// This function will return a new GUID.
//==========================================================================
AnimationManager.CreateGuid = function()
{
	function NextRandomNumber()
	{
		return ( ( ( 1 + Math.random() ) * 0x10000 ) | 0 ).toString( 16 ).substring( 1 );
	}

	var newGuid = ( NextRandomNumber()
		+ NextRandomNumber() + "-"
		+ NextRandomNumber() + "-4"
		+ NextRandomNumber().substr( 0, 3 ) + "-"
		+ NextRandomNumber() + "-"
		+ NextRandomNumber()
		+ NextRandomNumber()
		+ NextRandomNumber() ).toLowerCase();

	return newGuid;
};

//=======================================================================
// This function will return the transparency percentage from the 
// RGBA string. If there are any errors, it returns zero.
//=======================================================================
AnimationManager.GetTransparencyPercent = function( rgbaStr )
{
	if ( rgbaStr == null )
	{
		return "0";
	}

	var index = rgbaStr.indexOf( "rgba" );
	if ( index < 0 )
	{
		return "0";
	}

	var parts = rgbaStr.split( "," );
	if ( parts.length !== 4 )
	{
		return "0";
	}

	var transParts = parts[3].split( ")" );
	if ( transParts.length !== 2 )
	{
		return "0";
	}

	var transValue = parseFloat( transParts[0] );
	if ( transValue == null )
	{
		return "0";
	}

	var transPercent = 100 - parseInt( transValue * 100 );
	return transPercent;
};

//=======================================================================================
// This function will handle the property Text textbox on blur event. It will save
// the current text value to the model.
//=======================================================================================
AnimationManager.HandlePropertyTextboxOnBlur = function( textboxId )
{
	var animationPropertyGuid = textboxId.replace( AnimationManager.AnimationTestPropertyTextBoxPrefix, "" );

	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	var animationTest = AnimationManager.GetAnimationTestByProperty( animation, animationPropertyGuid );
	var currentProperty = AnimationManager.GetAnimationTestProperty( animationTest, animationPropertyGuid );

	var textValue = $( "#" + textboxId ).val();

	if ( currentProperty )
	{
		if ( currentProperty.VisualStates == null )
		{
			currentProperty.VisualStates = [];
		}

		if ( currentProperty.VisualStates.length === 0 )
		{
			var visualState = AnimationManager.CreateVisualStateModel;
			visualState.AnimationPropertyVisualStateGuid = AnimationManager.CreateGuid();
			visualState.Value = textValue;

			currentProperty.VisualStates.push( visualState );
		}
		else
		{
			currentProperty.VisualStates[0].Value = textValue;
		}

		AnimationManager.SetAnimationManagerModel( model );
		AnimationManager.UpdateTestPreview(animationTest.AnimationTestGuid, currentProperty.LookupName, currentProperty.VisualStates);
    }
};

//=======================================================================================
// This function will handle the transparency textbox on focus event. It will save
// the current value.
//=======================================================================================
AnimationManager.HandleTransparencyTextboxOnFocus = function( transparencyTextboxId )
{
	AnimationManager.CurrentFocusTransparencyTextboxValue = $( "#" + transparencyTextboxId ).val();
};

//========================================================================================
// This function will handle the transparency textbox on blur event. It will attempt
// to set the the color picker button with the new setting.
//========================================================================================
AnimationManager.HandleTransparencyTextboxOnBlur = function( transparencyTextboxId )
{
	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	var animationPropertyGuid = transparencyTextboxId.split( "_" )[1];

	var animationTest = AnimationManager.GetAnimationTestByProperty( animation, animationPropertyGuid );
	var property = AnimationManager.GetAnimationTestProperty( animationTest, animationPropertyGuid );

	var baseId = transparencyTextboxId.replace( "AnimationTestPropertyTransparency", "" );
	var animationTestPropertyColorButtonId = "AnimationTestPropertyColorButton" + baseId;
	var inputValueInt = 0;

	if ( property && property.LookupName !== "TEXT_COLOR" )
	{
		var inputValueStr = $( "#" + transparencyTextboxId ).val();

		if ( inputValueStr === AnimationManager.CurrentFocusTransparencyTextboxValue )
		{
			AnimationManager.CurrentFocusTransparencyTextboxValue = "";
			return;
		}

		if ( inputValueStr == null )
		{
			$( "#" + transparencyTextboxId ).val( AnimationManager.CurrentFocusTransparencyTextboxValue );
			AnimationManager.CurrentFocusTransparencyTextboxValue = "";
			return;
		}

		inputValueInt = parseInt( inputValueStr );

		if ( isNaN( inputValueInt ) )
		{
			alert( "Transparency Percent must be numeric." );
			$( "#" + transparencyTextboxId ).val( AnimationManager.CurrentFocusTransparencyTextboxValue );
			AnimationManager.CurrentFocusTransparencyTextboxValue = "";
			return;
		}

		if ( inputValueInt < 0 || inputValueInt > 100 )
		{
			alert( "Transparency Percent must be between 0 and 100 inclusively." );
			$( "#" + transparencyTextboxId ).val( AnimationManager.CurrentFocusTransparencyTextboxValue );
			AnimationManager.CurrentFocusTransparencyTextboxValue = "";
			return;
		}
	}

	var backgroundColor = $( "#" + animationTestPropertyColorButtonId ).css( "background-color" );

	if ( backgroundColor === "transparent" )
	{
		alert( "Must pick a color first." );
		$( "#" + transparencyTextboxId ).val( AnimationManager.CurrentFocusTransparencyTextboxValue );
		AnimationManager.CurrentFocusTransparencyTextboxValue = "";
		return;
	}

	// Transparency value of 1 = no transparency (0%), 0 = full transparency (100%).
	var transparency;

	if ( inputValueInt === 100 )
	{
		transparency = 0;
	}
	else if ( inputValueInt === 0 )
	{
		transparency = 1;
	}
	else
	{
		transparency = ( 100 - inputValueInt ) / 100.0;
	}

	// String could be "rgb(0,255,0)" or "rgba(0,255,0.5)".
	var mainParts = backgroundColor.split( "(" );
	var subParts;
	var subParts2;
	var newRgbaStr;

	if ( mainParts[0] === "rgb" )
	{
		// RGB means there is no transparency set in the color and transparency input value of zero
		// means the user is not adding transparency.  Therefore, just return.
		if ( inputValueInt === 0 )
		{
			AnimationManager.UpdateModelTransparencyAndColor( model, property, transparencyTextboxId, inputValueStr, backgroundColor );
			AnimationManager.UpdateTestPreview( animationTest.AnimationTestGuid, property.LookupName, property.VisualStates );
			return;
		}

		subParts = mainParts[1].split( "," );
		subParts2 = subParts[2].split( ")" );
		newRgbaStr = "rgba(" + subParts[0] + "," + subParts[1] + "," + subParts2[0] + "," + transparency + ")";

		$( "#" + animationTestPropertyColorButtonId ).css( "background-color", newRgbaStr );
		$( ".full" ).spectrum( "set", newRgbaStr );
		AnimationManager.CurrentFocusTransparencyTextboxValue = "";
		AnimationManager.UpdateModelTransparencyAndColor( model, property, transparencyTextboxId, inputValueStr, newRgbaStr );
		AnimationManager.UpdateTestPreview( animationTest.AnimationTestGuid, property.LookupName, property.VisualStates );
		return;
	}

	if ( mainParts[0] === "rgba" )
	{
		subParts = mainParts[1].split( "," );

		if ( inputValueInt === 0 )
		{
			var newRgbStr = "rgb(" + subParts[0] + "," + subParts[1] + "," + subParts[2] + ")";
			$( "#" + animationTestPropertyColorButtonId ).css( "background-color", newRgbStr );
			$( ".full" ).spectrum( "set", newRgbStr );

			AnimationManager.UpdateModelTransparencyAndColor( model, property, transparencyTextboxId, inputValueStr, newRgbStr );
			AnimationManager.UpdateTestPreview( animationTest.AnimationTestGuid, property.LookupName, property.VisualStates );
			return;
		}

		newRgbaStr = "rgba(" + subParts[0] + "," + subParts[1] + "," + subParts[2] + "," + transparency + ")";

		$( "#" + animationTestPropertyColorButtonId ).css( "background-color", newRgbaStr );
		$( ".full" ).spectrum( "set", newRgbaStr );

		AnimationManager.UpdateModelTransparencyAndColor( model, property, transparencyTextboxId, inputValueStr, newRgbaStr );
		AnimationManager.UpdateTestPreview( animationTest.AnimationTestGuid, property.LookupName, property.VisualStates );
	}

	AnimationManager.CurrentFocusTransparencyTextboxValue = "";
};

//========================================================================================
// This function will handle the color picker selection to update the model and the
// previewer.
//========================================================================================
AnimationManager.HandleColorPickerChange = function( transparencyTextboxId )
{
	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	var animationPropertyGuid = transparencyTextboxId.split( "_" )[1];

	var animationTest = AnimationManager.GetAnimationTestByProperty( animation, animationPropertyGuid );
	var property = AnimationManager.GetAnimationTestProperty( animationTest, animationPropertyGuid );

	var baseId = transparencyTextboxId.replace( "AnimationTestPropertyTransparency", "" );
	var animationTestPropertyColorButtonId = "AnimationTestPropertyColorButton" + baseId;

	var backgroundColor = $( "#" + animationTestPropertyColorButtonId ).css( "background-color" );
	var transValueStr = $( "#" + transparencyTextboxId ).val();
	AnimationManager.UpdateModelTransparencyAndColor( model, property, transparencyTextboxId, transValueStr, backgroundColor );
	AnimationManager.UpdateTestPreview( animationTest.AnimationTestGuid, property.LookupName, property.VisualStates );
};

//====================================================================================
// This function will update the color property and transparency property in the 
// model.
//====================================================================================
AnimationManager.UpdateModelTransparencyAndColor = function( model, propertyModel, transTextboxId, newValueStr, colorStr )
{
	if ( propertyModel )
	{
		//if ( propertyModel.LookupName === "TEXT_COLOR" )
		//{
		//	if ( transTextboxId.split( "_" )[0] === "AnimationTestPropertyTransparency1Id" )
		//	{
		//		propertyModel.VisualStates[0].Value = colorStr;
		//	}
		//	else
		//	{
		//		propertyModel.VisualStates[1].Value = colorStr;
		//	}
		//}
		//else
		{
			if ( transTextboxId.split( "_" )[0] === "AnimationTestPropertyTransparency1Id" )
			{
				propertyModel.VisualStates[0].Value = colorStr;
				propertyModel.VisualStates[1].Value = newValueStr;
			}
			else
			{
				propertyModel.VisualStates[2].Value = colorStr;
				propertyModel.VisualStates[3].Value = newValueStr;
			}
		}

		AnimationManager.SetAnimationManagerModel( model );
	}
};
//====================================================  End of section for adding Test Properties =================================================

//==================================================== Start of section to handle test preview ====================================================
//======================================================================
// This function will initialize the test preview to their initial
// state.
//======================================================================
AnimationManager.InitializeTestPreviews = function()
{
	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );

	if ( animation.AnimationTestGroups && animation.AnimationTestGroups.length > 0 )
	{
		for ( var nextTestGrpIndex = 0; nextTestGrpIndex < animation.AnimationTestGroups.length; nextTestGrpIndex++ )
		{
			var testGroup = animation.AnimationTestGroups[nextTestGrpIndex];
			for ( var nextTestIndex = 0; nextTestIndex < testGroup.TestList.length; nextTestIndex++ )
			{
				var test = testGroup.TestList[nextTestIndex];

				if ( test.PropertyList.length > 0 )
				{
					// Make test preview table visible.
					AnimationManager.MakeTextPreviewTableVisible( test.AnimationTestGuid );

					for ( var nextPropertyIndex = 0; nextPropertyIndex < test.PropertyList.length; nextPropertyIndex++ )
					{
						var property = test.PropertyList[nextPropertyIndex];
						AnimationManager.UpdateTestPreview( test.AnimationTestGuid, property.LookupName, property.VisualStates );
					}
				}
			}
		}
	}
};

//===========================================================================
// This function will make the test preview table visible.
//===========================================================================
AnimationManager.MakeTextPreviewTableVisible = function( testGuid )
{
	if ( testGuid )
	{
		var testPreviewTable = document.getElementById( AnimationManager.animationTestPreviewTablePrefix + testGuid );
		if ( testPreviewTable )
		{
			testPreviewTable.classList.remove( "hidden" );
		}
	}
};

AnimationManager.UpdateTextPreviewTextProp = function (textLabelId, textvalue) {
    if (textLabelId) {
        var element = document.getElementById(textLabelId);
        if (element) {
            element.innerHTML = "";
            if (textvalue != null && textvalue !== "") {
                var temptext = textvalue;
                if (textvalue.length > 14)
                {
                    temptext = textvalue.substr(0, 14);
                    temptext += "...";
                }
                element.innerHTML = temptext;
            }
        }
    }
};

//===========================================================================
// This function will sets the visibility property preview.
//===========================================================================
AnimationManager.UpdateTextPreviewVisibleProp = function( visibleLabelId, visibleValue )
{
	if ( visibleLabelId )
	{
		var element = document.getElementById( visibleLabelId );
		if ( element )
		{
			element.innerHTML = "ON";
			if ( visibleValue != null && visibleValue !== "" )
			{
				element.innerHTML = visibleValue;
			}
		}
	}
};

//===========================================================================
// This function will sets the color preview for a given property.
//===========================================================================
AnimationManager.UpdateTextPreviewColors = function( colorId1, colorId2, colorValue1, colorValue2 )
{
	var element;
	if ( colorId1 )
	{
		element = document.getElementById( colorId1 );
		if ( element )
		{
			element.style.backgroundColor = "transparent";
			if ( colorValue1 != null && colorValue1 !== "" )
			{
				element.style.backgroundColor = colorValue1;
			}
		}
	}

	if ( colorId2 )
	{
		element = document.getElementById( colorId2 );
		if ( element )
		{
			element.style.backgroundColor = "transparent";
			if ( colorValue2 != null && colorValue2 !== "" )
			{
				element.style.backgroundColor = colorValue2;
			}
		}
	}
};

//==============================================================
// This function will make the test preview visible.
//==============================================================
AnimationManager.MakeTestPreviewItemVisible = function( itemId )
{
	if ( itemId )
	{
		var element = document.getElementById( itemId );
		if ( element )
		{
			element.classList.remove( "hidden" );
		}
	}
};

//==============================================================
// This function will make the test preview not visible.
//==============================================================
AnimationManager.MakeTestPreviewItemHidden = function( itemId )
{
	if ( itemId )
	{
		var element = document.getElementById( itemId );
		if ( element )
		{
			element.classList.remove( "hidden" );
			element.classList.add( "hidden" );
		}
	}
};

//=================================================================================================
// This function will update the test preview row with configured data.
//=================================================================================================
AnimationManager.UpdateTestPreview = function( testGuid, propertyLookupName, propertyVisualStateList )
{
	if ( testGuid )
	{
		var hdrId = null;
		var itemId = null;
		if ( propertyLookupName === "BACKGROUND_COLOR" )
		{
			hdrId = AnimationManager.animationTestPreviewHdBkColorPrefix + testGuid;
			itemId = AnimationManager.animationTestPreviewBkColorPrefix + testGuid;

			AnimationManager.UpdateTextPreviewColors( AnimationManager.animationTestPreviewBkDivPrefix1 + testGuid,
				AnimationManager.animationTestPreviewBkDivPrefix2 + testGuid,
				propertyVisualStateList[0].Value,
				propertyVisualStateList[2].Value );
		}

		if ( propertyLookupName === "FILL_COLOR" )
		{
			hdrId = AnimationManager.animationTestPreviewHdFlColorPrefix + testGuid;
			itemId = AnimationManager.animationTestPreviewFlColorPrefix + testGuid;

			AnimationManager.UpdateTextPreviewColors( AnimationManager.animationTestPreviewFlDivPrefix1 + testGuid,
				AnimationManager.animationTestPreviewFlDivPrefix2 + testGuid,
				propertyVisualStateList[0].Value,
				propertyVisualStateList[2].Value );
		}

		if ( propertyLookupName === "LINE_COLOR" )
		{
			hdrId = AnimationManager.animationTestPreviewHdLnColorPrefix + testGuid;
			itemId = AnimationManager.animationTestPreviewLnColorPrefix + testGuid;

			AnimationManager.UpdateTextPreviewColors( AnimationManager.animationTestPreviewLnDivPrefix1 + testGuid,
				AnimationManager.animationTestPreviewLnDivPrefix2 + testGuid,
				propertyVisualStateList[0].Value,
				propertyVisualStateList[2].Value );
		}

		if ( propertyLookupName === "TEXT_COLOR" )
		{
			hdrId = AnimationManager.animationTestPreviewHdTxColorPrefix + testGuid;
			itemId = AnimationManager.animationTestPreviewTxColorPrefix + testGuid;

			AnimationManager.UpdateTextPreviewColors( AnimationManager.animationTestPreviewTxDivPrefix1 + testGuid,
				AnimationManager.animationTestPreviewTxDivPrefix2 + testGuid,
				propertyVisualStateList[0].Value,
				propertyVisualStateList[2].Value );
		}

		if ( propertyLookupName === "VISIBILITY" )
		{
			hdrId = AnimationManager.animationTestPreviewHdVsColorPrefix + testGuid;
			itemId = AnimationManager.animationTestPreviewVsColorPrefix + testGuid;

			AnimationManager.UpdateTextPreviewVisibleProp( AnimationManager.animationTestPreviewVsLblPrefix + testGuid, propertyVisualStateList[0].Value );
		}

		if (propertyLookupName === "TEXT") {
		    hdrId = AnimationManager.animationTestPreviewHdTextColorPrefix + testGuid;
		    itemId = AnimationManager.animationTestPreviewTextColorPrefix + testGuid;
		    AnimationManager.UpdateTextPreviewTextProp(AnimationManager.animationTestPreviewTxLblPrefix + testGuid, propertyVisualStateList[0].Value);
		}

	    // Make property item preview visible.
		AnimationManager.MakeTestPreviewItemVisible( hdrId );
		AnimationManager.MakeTestPreviewItemVisible( itemId );
	}
};

//==========================================================================================
// This function will create the test preview table.
//==========================================================================================
AnimationManager.CreateTestPreviewTable = function( testGuid )
{
	if ( testGuid == null )
	{
		return null;
	}

	// Prefix: AnimationTestPreviewTableId_
	var testPreviewTableId = AnimationManager.animationTestPreviewTablePrefix + testGuid;

	var testPreviewTable = document.createElement( "table" );
	testPreviewTable.id = testPreviewTableId;
	var header = testPreviewTable.createTHead();
	var headerRow = header.insertRow( 0 );

	var colCell1 = document.createElement( "th" );
	var colCell2 = document.createElement( "th" );
	var colCell3 = document.createElement( "th" );
	var colCell4 = document.createElement( "th" );
	var colCell5 = document.createElement( "th" );
	var colCell6 = document.createElement("th");

	headerRow.appendChild( colCell1 );
	headerRow.appendChild( colCell2 );
	headerRow.appendChild( colCell3 );
	headerRow.appendChild( colCell4 );
	headerRow.appendChild( colCell5 );
	headerRow.appendChild(colCell6);

	colCell1.innerHTML = "BkGrnd";
	colCell2.innerHTML = "Line";
	colCell3.innerHTML = "Fill";
	colCell4.innerHTML = "Text Color";
	colCell5.innerHTML = "Visibility";
	colCell6.innerHTML = "Text";

	colCell1.id = AnimationManager.animationTestPreviewHdBkColorPrefix + testGuid;
	colCell2.id = AnimationManager.animationTestPreviewHdLnColorPrefix + testGuid;
	colCell3.id = AnimationManager.animationTestPreviewHdFlColorPrefix + testGuid;
	colCell4.id = AnimationManager.animationTestPreviewHdTxColorPrefix + testGuid;
	colCell5.id = AnimationManager.animationTestPreviewHdVsColorPrefix + testGuid;
	colCell6.id = AnimationManager.animationTestPreviewHdTextColorPrefix + testGuid;

	colCell1.setAttribute( "class", "testPreviewHeader hidden" );
	colCell2.setAttribute( "class", "testPreviewHeader hidden" );
	colCell3.setAttribute( "class", "testPreviewHeader hidden" );
	colCell4.setAttribute( "class", "testPreviewHeader hidden" );
	colCell5.setAttribute( "class", "testPreviewHeader hidden" );
	colCell6.setAttribute("class", "testPreviewHeader hidden");

	// Initially hide the preview table.
	testPreviewTable.classList.add( "hidden" );

	var testPreviewTbody = document.createElement( "tbody" );
	testPreviewTable.appendChild( testPreviewTbody );

	return testPreviewTable;
};

//===========================================================================================
// This function will create the test preview row.
//===========================================================================================
AnimationManager.CreateTestPreviewRow = function( testGuid, testPreviewTable )
{
	if ( testGuid == null )
	{
		return;
	}

	if ( testPreviewTable )
	{
		var testPreviewRow = testPreviewTable.insertRow( -1 );

		var cell1 = testPreviewRow.insertCell( 0 );
		var cell2 = testPreviewRow.insertCell( 1 );
		var cell3 = testPreviewRow.insertCell( 2 );
		var cell4 = testPreviewRow.insertCell( 3 );
		var cell5 = testPreviewRow.insertCell( 4 );
		var cell6 = testPreviewRow.insertCell(5);

		cell1.id = AnimationManager.animationTestPreviewBkColorPrefix + testGuid;
		cell1.setAttribute( "class", "testPreviewPadding hidden" );

		cell2.id = AnimationManager.animationTestPreviewLnColorPrefix + testGuid;
		cell2.setAttribute( "class", "testPreviewPadding hidden" );

		cell3.id = AnimationManager.animationTestPreviewFlColorPrefix + testGuid;
		cell3.setAttribute( "class", "testPreviewPadding hidden" );

		cell4.id = AnimationManager.animationTestPreviewTxColorPrefix + testGuid;
		cell4.setAttribute( "class", "testPreviewPadding hidden" );

		cell5.id = AnimationManager.animationTestPreviewVsColorPrefix + testGuid;
		cell5.setAttribute( "class", "testPreviewPadding hidden" );

		cell6.id = AnimationManager.animationTestPreviewTextColorPrefix + testGuid;
		cell6.setAttribute("class", "testPreviewPadding hidden");

	    // Create the two color previews for background.
		var previewColorDiv1 = AnimationManager.CreateColorPreviewBox( AnimationManager.animationTestPreviewBkDivPrefix1 + testGuid );
		var previewColorDiv2 = AnimationManager.CreateColorPreviewBox( AnimationManager.animationTestPreviewBkDivPrefix2 + testGuid );
		cell1.appendChild( previewColorDiv1 );
		cell1.appendChild( previewColorDiv2 );

		// Create the two color previews for line.
		previewColorDiv1 = AnimationManager.CreateColorPreviewBox( AnimationManager.animationTestPreviewLnDivPrefix1 + testGuid );
		previewColorDiv2 = AnimationManager.CreateColorPreviewBox( AnimationManager.animationTestPreviewLnDivPrefix2 + testGuid );
		cell2.appendChild( previewColorDiv1 );
		cell2.appendChild( previewColorDiv2 );

		// Create the two color previews for fill.
		previewColorDiv1 = AnimationManager.CreateColorPreviewBox( AnimationManager.animationTestPreviewFlDivPrefix1 + testGuid );
		previewColorDiv2 = AnimationManager.CreateColorPreviewBox( AnimationManager.animationTestPreviewFlDivPrefix2 + testGuid );
		cell3.appendChild( previewColorDiv1 );
		cell3.appendChild( previewColorDiv2 );

		// Create the two color previews for text.
		previewColorDiv1 = AnimationManager.CreateColorPreviewBox( AnimationManager.animationTestPreviewTxDivPrefix1 + testGuid );
		previewColorDiv2 = AnimationManager.CreateColorPreviewBox( AnimationManager.animationTestPreviewTxDivPrefix2 + testGuid );
		cell4.appendChild( previewColorDiv1 );
		cell4.appendChild( previewColorDiv2 );

		var visibilityLabel = document.createElement( "label" );
		visibilityLabel.id = AnimationManager.animationTestPreviewVsLblPrefix + testGuid;
		visibilityLabel.setAttribute( "class", "testPreviewVisibilityLabel" );
		cell5.appendChild( visibilityLabel );

		var TextLabel = document.createElement("label");
		TextLabel.id = AnimationManager.animationTestPreviewTxLblPrefix + testGuid;
		TextLabel.setAttribute("class", "testPreviewVisibilityLabel");
		cell6.appendChild(TextLabel);
    }
};

//=====================================================================================
// This function will create a color preview div box and returns it.
//=====================================================================================
AnimationManager.CreateColorPreviewBox = function( previewColorBoxId )
{
	var previewColorDiv = document.createElement( "div" );
	var previewColorInnerDiv = document.createElement( "div" );
	previewColorDiv.appendChild( previewColorInnerDiv );
	previewColorDiv.setAttribute( "class", "sp-preview testPreviewColorSize" );
	previewColorInnerDiv.id = previewColorBoxId;
	previewColorInnerDiv.setAttribute( "class", "sp-preview-inner" );
	previewColorInnerDiv.style.backgroundColor = "transparent";

	return previewColorDiv;
};
//==================================================== End of section to handle test preview ======================================================

//===================================================================================
// This function will validate the value in the bit mask field. If valid, it
// returns an object with the converted numeric value and type. If not valid,
// it returns an object with the error flag set and an error message.
//===================================================================================
AnimationManager.BitMaskValidator = function( bitwiseOpSelection, bitmaskValue )
{
	var bitMaskResult = new Object();
	bitMaskResult.Error = false;
	bitMaskResult.ErrorMsg = "";
	bitMaskResult.NumberType = "";
	bitMaskResult.NumericValue = 0;

	// Bitwise operation is equal to NONE, ignore.
	if ( bitwiseOpSelection === "6" )
	{
		return bitMaskResult;
	}

	if ( bitmaskValue === "" )
	{
		bitMaskResult.Error = true;
		bitMaskResult.ErrorMsg = "Must enter a Bit Mask value (integer, octal, or hex).";

		return bitMaskResult;
	}

	// Check for only positive integer, octal, or hex characters.
	for ( var nextCharIndex = 0; nextCharIndex < bitmaskValue.length; nextCharIndex++ )
	{
		var code = bitmaskValue.charCodeAt( nextCharIndex );

		if ( !( code > 47 && code < 58 ) && // numeric (0-9)
			!( code > 64 && code < 71 ) && // upper alpha (A-F)
			!( code === 120 ) && // lower x
			!( code > 96 && code < 103 ) ) // lower alpha (a-f)
		{
			bitMaskResult.Error = true;
			bitMaskResult.ErrorMsg = "Bit Mask value must be a valid positive integer, octal, or hex.";
			return bitMaskResult;
		}
	}

	var intValue = parseInt( bitmaskValue );

	if ( isNaN( intValue ) )
	{
		bitMaskResult.Error = true;
		bitMaskResult.ErrorMsg = "Bit Mask value must be a valid integer, octal, or hex.";

		return bitMaskResult;
	}

	// Figure out the type of the value entered.
	var type = "INT";
	var octalPrefix = bitmaskValue.substring( 0, 1 );

	if ( octalPrefix === "0" )
	{
		type = "OCTAL";
		intValue = parseInt( bitmaskValue, 8 );
	}

	if ( bitmaskValue.length >= 3 )
	{
		var hexPrefix = bitmaskValue.substring( 0, 2 );
		if ( hexPrefix === "0x" )
		{
			type = "HEX";
			intValue = parseInt( bitmaskValue, 16 );
		}
	}

	bitMaskResult.Error = false;
	bitMaskResult.ErrorMsg = "";
	bitMaskResult.NumberType = type;
	bitMaskResult.NumericValue = intValue;

	return bitMaskResult;
};

//===================================================================================
// This function will validate the value in the comparison field. If valid, it
// returns an object with the error flag set to false. If not valid,
// it returns an object with the error flag set and an error message.
//===================================================================================
AnimationManager.ComparisonValueValidator = function( testGrpType, comparisonValue )
{
	var comparisonValueResult = new Object();
	comparisonValueResult.Error = false;
	comparisonValueResult.ErrorMsg = "";
	var nextCharIndex;
	var code;

	if ( testGrpType.Type === "TYPE_OTHER" )
	{
		return comparisonValueResult;
	}

	if ( testGrpType.Type === "TYPE_INT" )
	{
		for ( nextCharIndex = 0; nextCharIndex < comparisonValue.length; nextCharIndex++ )
		{
			code = comparisonValue.charCodeAt( nextCharIndex );

			if ( !( code > 47 && code < 58 ) && // numeric (0-9)
				!( code === 45 ) ) // Dash
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparion value must be a valid integer.";
				return comparisonValueResult;
			}
		}

		var intValue = parseInt( comparisonValue );

		if ( testGrpType.Text === "System.Int16" )
		{
			if ( intValue < -32768 || intValue > 32767 )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparion value must be an integer between -32,768 and 32,767.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "System.UInt16" )
		{
			if ( intValue < 0 || intValue > 65535 )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparion value must be an integer between 0 and 65,535.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "System.Int32" )
		{
			if ( intValue < -2147483648 || intValue > 2147483647 )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparion value must be an integer between -2,147,483,648 and 2,147,483,647.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "System.UInt32" )
		{
			if ( intValue < 0 || intValue > 4294967295 )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparion value must be an integer between  0 and 4,294,967,295.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "System.Int64" )
		{
			if ( intValue < -9223372036854775808 || intValue > 9223372036854775807 )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparion value must be an integer between -9,223,372,036,854,775,808 and 9,223,372,036,854,775,807.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "System.UInt64" )
		{
			if ( intValue < 0 || intValue > 18446744073709551615 )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparion value must be an integer between  0 and 18,446,744,073,709,551,615.";
				return comparisonValueResult;
			}
		}
	}

	if ( testGrpType.Type === "TYPE_STRING" )
	{
		return comparisonValueResult;
	}

	if ( testGrpType.Type === "TYPE_BOOLEAN" )
	{
		if ( comparisonValue.toLowerCase() !== "true" && comparisonValue.toLowerCase() !== "false" )
		{
			comparisonValueResult.Error = true;
			comparisonValueResult.ErrorMsg = "Comparison value must be a boolean value of 'true' or 'false'.";
			return comparisonValueResult;
		}
	}

	if ( testGrpType.Type === "TYPE_DECIMAL" )
	{
		for ( nextCharIndex = 0; nextCharIndex < comparisonValue.length; nextCharIndex++ )
		{
			code = comparisonValue.charCodeAt( nextCharIndex );

			if ( !( code > 47 && code < 58 ) && // numeric (0-9)
				!( code === 44 ) && // comma
				!( code === 45 ) && // dash
				!( code === 69 ) && // "E"
				!( code === 101 ) && // "e"
				!( code === 46 ) ) // period
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparison value must be a valid decimal number.";
				return comparisonValueResult;
			}
		}

		var floatValue = parseFloat( comparisonValue );

		if ( floatValue === Infinity || floatValue === -Infinity )
		{
			comparisonValueResult.Error = true;
			comparisonValueResult.ErrorMsg = "Comparion value is too large/small.";
			return comparisonValueResult;
		}

		if ( testGrpType.Text === "System.Double" )
		{
			if ( floatValue < -5.0E324 || floatValue > 1.7E308 )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparion value must be a double between -5.0 x 10^324 and +1.7 X 10^308.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "System.Single" )
		{
			if ( floatValue < -3.4E38 || floatValue > 3.4E38 )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparion value must be a double between -3.4 X 10^38 and 3.4 X 10^38.";
				return comparisonValueResult;
			}
		}
	}

	if ( testGrpType.Type === "TYPE_CODED_VARIABLES" )
	{
		if ( testGrpType.Text === "FMBusinessObjects.DataObjects.CodedVariables.TankCommands" )
		{
			if ( comparisonValue.toUpperCase() !== "STOP"
				&& comparisonValue.toUpperCase() !== "FILL"
				&& comparisonValue.toUpperCase() !== "EMPTY"
				&& comparisonValue.toUpperCase() !== "RUN"
				&& comparisonValue.toUpperCase() !== "TEST"
				&& comparisonValue.toUpperCase() !== "RESET" )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparison value must be either Stop, Fill, Empty, Run, Test, or Reset.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "FMBusinessObjects.DataObjects.CodedVariables.TransferModes" )
		{
			if ( comparisonValue.toUpperCase() !== "INACTIVE"
				&& comparisonValue.toUpperCase() !== "LEVEL"
				&& comparisonValue.toUpperCase() !== "BATCH" )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparison value must be either Inactive, Level, or Batch.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "FMBusinessObjects.DataObjects.CodedVariables.TankStatuses" )
		{
			if ( comparisonValue.toUpperCase() !== "STOPPED"
				&& comparisonValue.toUpperCase() !== "FILLING"
				&& comparisonValue.toUpperCase() !== "EMPTYING"
				&& comparisonValue.toUpperCase() !== "RUNNING"
				&& comparisonValue.toUpperCase() !== "TESTING" )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparison value must be either Stopped, Filling, Emptying, Running or Testing.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses" )
		{
			if ( comparisonValue.toUpperCase() !== "INACTIVE"
				&& comparisonValue.toUpperCase() !== "TRANSFER TARGET"
				&& comparisonValue.toUpperCase() !== "IN PROGRESS"
				&& comparisonValue.toUpperCase() !== "COMPLETE" )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparison value must be either Inactive, Transfer Target, In Progress, or Complete.";
				return comparisonValueResult;
			}
		}

		if (testGrpType.Text === "FMBusinessObjects.DataObjects.CodedVariables.TankOperationalMode") {
			if (comparisonValue.toUpperCase() !== "NORMAL"
				&& comparisonValue.toUpperCase() !== "QUANRANTINED"
				&& comparisonValue.toUpperCase() !== "MARKET") {
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparison value must be either Normal, Quanrantined, or Market.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "FMBusinessObjects.DataObjects.CodedVariables.MovementNodeCommand" )
		{
			if ( comparisonValue.toUpperCase() !== "START"
				&& comparisonValue.toUpperCase() !== "STOP")
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparison value must be either Start or Stop.";
				return comparisonValueResult;
			}
		}




		if ( testGrpType.Text === "FMBusinessObjects.DataObjects.CodedVariables.MovementCommand" )
		{
			if ( comparisonValue.toUpperCase() !== "ACTIVATE"
				&& comparisonValue.toUpperCase() !== "STOP" )
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparison value must be either Activate, or Stop.";
				return comparisonValueResult;
			}
		}

		if ( testGrpType.Text === "FMBusinessObjects.DataObjects.CodedVariables.MovementStatus" )
		{
			if ( comparisonValue.toUpperCase() !== "INACTIVE"
				&& comparisonValue.toUpperCase() !== "ACTIVE"
				&& comparisonValue.toUpperCase() !== "HOLD FOR HANDGAUGE DATA")
			{
				comparisonValueResult.Error = true;
				comparisonValueResult.ErrorMsg = "Comparison value must be either Inactive, Active, or Hold For Handgauge Data.";
				return comparisonValueResult;
			}
		}




		if (testGrpType.Text === "FMBusinessObjects.DataObjects.CodedVariables.PointCommandStatusListReference") {
		    if (comparisonValue == "")
            {
		        comparisonValueResult.Error = true;
		        comparisonValueResult.ErrorMsg = "Comparison value cannot be blank.";
		        return comparisonValueResult;
		    }
		}
    }

	return comparisonValueResult;
};

AnimationManager.GetFieldFromDataTypeSelection = function( dataType, model )
{
	var fieldFilter = FMTAGFIELDSELECTION.VALUE;
	if ( dataType === model.TranslatedTextForAlarmStatusField )
	{
		fieldFilter = FMTAGFIELDSELECTION.ALARMSTATUS;
	}
	if ( dataType === model.TranslatedTextForIDField )
	{
		fieldFilter = FMTAGFIELDSELECTION.ID;
	}
	if ( dataType === model.TranslatedTextForTimestampField )
	{
		fieldFilter = FMTAGFIELDSELECTION.TIMESTAMP;
	}
	if ( dataType === model.TranslatedTextForUnitsField )
	{
		fieldFilter = FMTAGFIELDSELECTION.UNITS;
	}
	return fieldFilter;
};

AnimationManager.TestGroupPointValueSelection = function( currentItem )
{
	if ( AnimationManager.enablePointSelect === false )
	{
		return;
	}
	AnimationManager.AnimationTestGroupGuidForPointValueSelection = currentItem.id;
	var animationTestGroupGuid = AnimationManager.AnimationTestGroupGuidForPointValueSelection.replace( AnimationManager.AnimationTestGroupPointValueIdPrefix, '' );
	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	if ( animation )
	{
		var animationTestGroup = AnimationManager.GetAnimationTestGroup( animation, animationTestGroupGuid );
		if ( animationTestGroup )
		{
			var fieldFilter = AnimationManager.GetFieldFromDataTypeSelection( animationTestGroup.DataType, model );
			var dataTypeStr = fieldFilter === FMTAGFIELDSELECTION.VALUE ? animationTestGroup.DataType : null;
			AnimationManager.AnimationPointValueModalScreenShowing = true;
			TagSelection.OpenTagDialogByDataType( false, AnimationManager.TestGroupPointValueSelectionCallback, AnimationManager.TagSelectionMetaData.tagSelectionModelSaveCallBackFunction, AnimationManager.TagSelectionMetaData.tagSelectionModelGetLastCallBackFunction, AnimationManager.TagSelectionMetaData.openFinishCallBackFunction, AnimationManager.TagSelectionMetaData.urlTagSelectionGetPointListWithPanelTemplateContextByDataType, AnimationManager.TagSelectionMetaData.urlTagSelectionGetPointListByDataType, AnimationManager.TagSelectionMetaData.pointTemplateGuidStr, AnimationManager.TagSelectionMetaData.panelType, AnimationManager.TagSelectionMetaData.sectionId, dataTypeStr, fieldFilter );
		}
	}
};

AnimationManager.TestGroupPointValueSelectionCallback = function( response )
{
	AnimationManager.AnimationPointValueModalScreenShowing = false;
	if ( response === "Cancel" )
	{
		return;
	}
	var animationTestGroupGuid = AnimationManager.AnimationTestGroupGuidForPointValueSelection.replace( AnimationManager.AnimationTestGroupPointValueIdPrefix, '' );
	var model = AnimationManager.GetAnimationManagerModel();
	var animation = AnimationManager.GetCurrentAnimation( model );
	if ( animation )
	{
		var animationTestGroup = AnimationManager.GetAnimationTestGroup( animation, animationTestGroupGuid );
		if ( animationTestGroup )
		{
			var tagSelectionModel = response;
			if ( response )
			{
				var pointValues = tagSelectionModel.PointValues;
				var pointValue = pointValues[0];
				var field = AnimationManager.GetFieldFromDataTypeSelection( animationTestGroup.DataType, model );
				var pointValueString = pointValue.PointID + "." + pointValue.ID + "." + FMTAGFIELDSELECTION.GetFieldString( field );
				animationTestGroup.PointValueGuid = pointValue.PointValueIdentifier.IdentityGuid;
				animationTestGroup.PointGuid = pointValue.PointGuid;
				animationTestGroup.Field = field;
				animationTestGroup.PointValueIsFromTemplate = tagSelectionModel.PointTemplateTagSelectionIndicator;
				animationTestGroup.ValueType = pointValue.PointValueIdentifier.PointValueType;
				animationTestGroup.PointID = pointValue.PointID;
				if (animationTestGroup.PointValueIsFromTemplate != true
					&& pointValue.PointValueIdentifier.PropertyID != null
					&& pointValue.PointValueIdentifier.PropertyID != "undefined") {
					animationTestGroup.PointValueID = pointValue.PointValueIdentifier.PropertyID;
				}
				else {
					animationTestGroup.PointValueID = pointValue.ID;
				}
				animationTestGroup.PointValueAndFieldID = pointValueString;
				AnimationManager.SetAnimationManagerModel( model );
				var pointValueObj = document.getElementById( AnimationManager.AnimationTestGroupGuidForPointValueSelection );
				pointValueObj.innerHTML = pointValueString;
			}
			else
			{
			    AnimationManager.AnimationUnassigned = $("#AnimationMgrUnassignedTranslated").val();
				animationTestGroup.PointValueGuid = AnimationManager.AnimationEmptyGuid;
				animationTestGroup.PointGuid = AnimationManager.AnimationEmptyGuid;
				animationTestGroup.PointValueIsFromTemplate = false;
				animationTestGroup.PointValueAndFieldID = AnimationManager.AnimationUnassigned;
				animationTestGroup.ValueType = 0;
				animationTestGroup.PointID = "";
				animationTestGroup.PointValueID = "";
				AnimationManager.SetAnimationManagerModel( model );
				var pointValueObj2 = document.getElementById( AnimationManager.AnimationTestGroupGuidForPointValueSelection );
				pointValueObj2.innerHTML = AnimationManager.AnimationUnassigned;
			}
		}
	}
};

AnimationManager.GetAnimationManagerModelString = function()
{
	return $( '#AnimationManagerModel' ).val();
};

AnimationManager.GetAnimationManagerModel = function()
{
	var strModel = AnimationManager.GetAnimationManagerModelString();
	if ( strModel === undefined )
	{
		return undefined;
	}
	var model = JSON.parse( strModel );
	return model;
};

AnimationManager.SetAnimationManagerModelString = function( modelStr )
{
	$( '#AnimationManagerModel' ).val( modelStr );
};

AnimationManager.SetAnimationManagerModel = function( model )
{
	var modelStr = JSON.stringify( model );
	AnimationManager.SetAnimationManagerModelString( modelStr );
};

AnimationManager.ApplyAlternatingRowColors = function ()
{
	//Tag Rows
	var backgroundWhite = true;
	var rows = $('#' + AnimationManager.AnimationTestGroupsTable + ' tbody tr').get();
	rows.forEach(function (row) {
		if ( row.id.indexOf( AnimationManager.AnimationTestGroupRowPrefix ) === 0 )
		{
			var backgroundColor = '#EFF6FC';
			if ( backgroundWhite )
			{
				backgroundColor = '#ffffff';
			}
			row.setAttribute( 'style', 'background-color: ' + backgroundColor );
			backgroundWhite = !backgroundWhite;

			var animationTestGroupGuid = row.id.replace( AnimationManager.AnimationTestGroupRowPrefix, '' );
			var animationTestRows = $( '#' + AnimationManager.AnimationTestTableIdPrefix + animationTestGroupGuid + ' tbody tr' ).get();
			var testBackgroundWhite = true;
			animationTestRows.forEach( function( animationTestRow )
			{
				if (animationTestRow.id.indexOf(AnimationManager.AnimationTestTableRowPrefix) === 0 || animationTestRow.id === AnimationManager.AnimationTestEditHolderRowId)
				{
					var testBackgroundColor = '#EFF6FC';
					if ( testBackgroundWhite )
					{
						testBackgroundColor = '#ffffff';
					}
					animationTestRow.setAttribute('style', 'background-color: ' + testBackgroundColor);
					testBackgroundWhite = !testBackgroundWhite;

					var animationTestGuid = animationTestRow.id.replace( AnimationManager.AnimationTestTableRowPrefix, '' );
					var animationPropertyRows = $( '#' + AnimationManager.AnimationTestPropertyTablePrefix + animationTestGuid + ' tbody tr' ).get();
					var propertyBackgroundWhite = true;
					animationPropertyRows.forEach( function( animationPropertyRow )
					{
						if (animationPropertyRow.id.indexOf(AnimationManager.AnimationTestPropertyTableRowPrefix) === 0 || animationPropertyRow.id.indexOf(AnimationManager.AnimationTemporaryRowPrefix === 0))
						{
							var propertyBackgroundColor = '#EFF6FC';
							if ( propertyBackgroundWhite )
							{
								propertyBackgroundColor = '#ffffff';
							}
							animationPropertyRow.setAttribute( 'style', 'background-color: ' + propertyBackgroundColor );
							propertyBackgroundWhite = !propertyBackgroundWhite;
						}
					} );
				}
			} );
		}
	});
};

AnimationManager.OpenCopyAndRenameAnimation = function( obj )
{
	var model = AnimationManager.GetAnimationManagerModel();
	if ( model )
	{
		var animation = AnimationManager.GetCurrentAnimation(model);
		if ( animation )
		{
			$( "#" + AnimationManager.animationManagerOkButtonId ).attr( "disabled", "disabled" );
			FMErrorAndExceptionHandling.CloseNotifications();
			AnimationManager.AnimationDeactivatTextEditingTools();
			AnimationManager.AnimationCopyAndRenameModalScreenShowing = true;
			$( '#AnimationCopyAndRenameModalScreen' ).modal( 'show' );
			$( '#CopyAndRenameAnimationname' ).val( '' );
		}
	}
};

AnimationManager.CloseCopyAndRenameAnimation = function( obj )
{
	$( '#AnimationCopyAndRenameModalScreen' ).modal( 'hide' );
	AnimationManager.AnimationCopyAndRenameModalScreenShowing = false;
	var model = AnimationManager.GetAnimationManagerModel();
	if ( model.SelectedAnimationGuid === AnimationManager.AnimationEmptyGuid )
	{
		$( '#AnnimationDropDown' ).val( AnimationManager.AnimationEmptyAnimation );
	}
	else
	{
		$( '#AnnimationDropDown' ).val( model.SelectedAnimationGuid );
	}
};

AnimationManager.CreateCopyAndRenameAnimation = function( obj )
{
	var copyAndRenameAnimationname = $( '#CopyAndRenameAnimationname' ).val();
	var model = AnimationManager.GetAnimationManagerModel();

	if ( AnimationManager.CheckForRedundantAnimationName( copyAndRenameAnimationname, model ) === false )
	{
		$( '#AnimationCopyAndRenameModalScreen' ).modal( 'hide' );
		var url = $('#urlAnimationManagerCopyAndRenameAnimation').val();
		AnimationManager.CallController(copyAndRenameAnimationname, url, false);
		$("#" +AnimationManager.animationManagerOkButtonId).removeAttr("disabled");
	}
	else
	{
		FMErrorAndExceptionHandling.ShowError( "Name Already In Use: " + copyAndRenameAnimationname );
		AnimationManager.CloseCopyAndRenameAnimation();
	}
};

AnimationManager.SetCollapsed = function ()
{
    var model = AnimationManager.GetAnimationManagerModel();
    if (model) {
        var animation = AnimationManager.GetCurrentAnimation(model);

        if (animation) {
            for (var i = 0; i < animation.AnimationTestGroups.length; i++)
            {
                var testGroup = animation.AnimationTestGroups[i];
                for (var testlistid = 0; testlistid < testGroup.TestList.length; testlistid++)
                {
                    var animationTest = testGroup.TestList[testlistid];
                    var animationTestExpandId = "AnimationTestExpandId_" + animationTest.AnimationTestGuid;
                    var buttonClick = document.getElementById(animationTestExpandId);
                    buttonClick.click();
                }
            }
        }
    }

}
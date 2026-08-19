var AlarmEditor = AlarmEditor || {
	TagRowPrefix: "TagRow_",
	AlarmsTablePrefix: "alarmsTable_",
	AlarmRowPrefix: "AlarmsTableAlarmRow_",
	TestTablePrefix: "AlarmTestTable_",
	TestRowPrefix: "testRow_",
	TagExpandPrefix: "TagExpand_",
	AlarmExpandPrefix: "AlarmExpand_",
	AlarmTestPrefix: "AlarmTests_",
	AlarmPrefix: "AlarmRow_",
	TestEnablePrefix: "testEnabledCellId_",
	AlarmEnablePrefix: "alarmsTableAlarmEnabledCellId_",
	AlarmEnableImgPrefix: "alarmsTableAlarmEnabledCellImgId_",
	AlarmNotifyImgPrefix: "alarmsTableAlarmNotifyCellImgId_",
	TagAlarmsNotifyImgPrefix: "TagAlarmsNotifyToggleImg_",
	LimitSprocketPrefix: "limitSprockedId_",
	EquationSprocketPrefix: "equationSprockedId_",
	HelpFileSprocketPrefix: "helpFileSprockedId_",
	DiagramSprocketPrefix: "diagramSprockedId_",
	SuppressedShelvedSprocketPrefix: "suppressedShelvedSprocketId_",
	TagAlarmsEnablePrefix: "AlarmsEnableToggle_",
	TagAlarmsEnableImgPrefix: "AlarmsEnableToggleImg_",
	TagHysteresisPrefix: "HysteresisInput_",
	TagHoldOffSecondsPrefix: "TimedHoldOffSecondsInput_",
	TagHoldOffMinutesPrefix: "TimedHoldOffMinutesInput_",
	AlarmTextInputPrefix: "alarmTextInputId_",
	AlarmStateInputPrefix: "alarmStateInputId_",
	LimitValueInputPrefix: "limitValueInputId_",
	TestTestCellDivPrefix: "testTestCellDivId_",
	TestModel: null,
	TestTestCellToUpdate: "",
	SelectedDiagramSprockId: null,
	PointDetailPrevFocus: "",
	PointDetailPrevPrevFocus: "",
	LastValidationId: "",
	LastValidationPassed: true
};

if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;

	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

AlarmEditor.Init = function()
{
    $( '#alarmEditorOutterDiv' ).niceScroll( {
        cursorwidth: '10px',
        autohidemode: false,
        cursorcolor: '#486899',
        background: '#f9f9f9',
        railoffset: true,
        railpadding: { top: 0, right: 0, left: -5, bottom: 0 },
        smoothscroll: true
    } );

    AlarmEditor.ApplyAlternatingRowColors();
    //Condition Alarm Model String so that we prevent deserialization issues with amperand characters in the Alarm Test String
    var model = AlarmEditor.GetAlarmEditorModel();
    AlarmEditor.SetAlarmEditorModel( model );
	 AlarmEditor.ContractEnabledAlarms( document.getElementById( "expandContractButton" ) );
};

AlarmEditor.ApplyAlternatingRowColors = function()
{
    //Tag Rows
    var backgroundWhite = true;
    var rows = $( '#alarmTagsTable tbody tr' ).get();
    rows.forEach( function( row )
    {
        if ( row.id.indexOf( AlarmEditor.TagRowPrefix ) === 0 )
        {
            var backgroundColor = '#ebebeb';
            if ( backgroundWhite )
            {
                backgroundColor = '#f7f7f7';
            }
            row.setAttribute( 'style', 'background-color: ' + backgroundColor );
            backgroundWhite = !backgroundWhite;

            //Alarm Rows
            var pointTagGuid = row.id.replace( AlarmEditor.TagRowPrefix, '' );
            var alarmRows = $( '#' + AlarmEditor.AlarmsTablePrefix + pointTagGuid + ' tbody tr' ).get();
            var alarmBackgroundWhite = true;
            alarmRows.forEach( function( alarmRow )
            {
                if ( alarmRow.id.indexOf( AlarmEditor.AlarmRowPrefix ) === 0 )
                {
                    var alarmBackgroundColor = '#ffffff';
                    if ( alarmBackgroundWhite )
                    {
                        alarmBackgroundColor = '#f5f5f5';
                    }
                    alarmRow.setAttribute( 'style', 'background-color: ' + alarmBackgroundColor );
                    alarmBackgroundWhite = !alarmBackgroundWhite;

                    //Test Rows
                    var alarmGuid = alarmRow.id.replace( AlarmEditor.AlarmRowPrefix, '' );
                    var testRows = $( '#' + AlarmEditor.TestTablePrefix + alarmGuid + ' tbody tr' ).get();
                    var testBackgroundWhite = true;
                    testRows.forEach( function( testRow )
                    {
                        if ( testRow.id.indexOf( AlarmEditor.TestRowPrefix ) === 0 )
                        {
                            var testBackgroundColor = '#eff6fc';
                            if ( testBackgroundWhite )
                            {
                                testBackgroundColor = '#ffffff';
                            }
                            testRow.setAttribute( 'style', 'background-color: ' + testBackgroundColor );
                            testBackgroundWhite = !testBackgroundWhite;
                        }
                    } );
                }
            } );
        }
    } );
};
AlarmEditor.GetAlarmEditorModelString = function()
{
    return $( '#AlarmEditorModel' ).val();
}; //This is a hack to overcome limitations of serializing and deserilazing JSON objects using the
//Newtonsoft JsonConvert.DeserializeObject method.  If &&s are in the string it causes an exception to be 
//thrown.
AlarmEditor.UpdateTestEquationString = function( model, characterToReplace, newCharacter )
{
    if ( model )
    {
        var expression = new RegExp( characterToReplace, 'g' );
        model.Tags.forEach( function( tag )
        {
            if ( tag.Alarms )
            {
                tag.Alarms.forEach( function( alarm )
                {
                    if ( alarm.AlarmTests )
                    {
                        alarm.AlarmTests.forEach( function( test )
                        {
                            if ( test )
                            {
                                test.AlarmTestEquation = test.AlarmTestEquation.replace( expression, newCharacter );
                            }
                        } );
                    }
                } );
            }
        } );
    }
};
AlarmEditor.GetAlarmEditorModel = function()
{
    var strModel = AlarmEditor.GetAlarmEditorModelString();
    if ( strModel === undefined )
        return undefined;
    var model = JSON.parse(strModel);
    //Replace all pound characters in each AlarmTextEquation with a amperand character
    AlarmEditor.UpdateTestEquationString( model, '#', '&' );
    return model;
};
AlarmEditor.SetAlarmEditorModelString = function( modelStr )
{
    $( '#AlarmEditorModel' ).val( modelStr );
};
AlarmEditor.SetAlarmEditorModel = function( model )
{
    //Replace all ampersand characters in each AlarmTextEquation with a pound character
    AlarmEditor.UpdateTestEquationString( model, '&', '#' );
	var modelStr = JSON.stringify(model);
    AlarmEditor.SetAlarmEditorModelString( modelStr );
};
AlarmEditor.ExpandAlarms = function( currentItem )
{
	var pointTagGuid = currentItem.id.replace(AlarmEditor.TagExpandPrefix, '');
	currentItem.innerHTML = '<img name="collapse" draggable="false" title="expanded" alt="expanded" src="' + window.applicationRootName+ '/fmwebapp/images/Arrow-downBl.png">';
    $( '#' + AlarmEditor.AlarmPrefix + pointTagGuid ).removeClass( 'hidden' );
    currentItem.setAttribute( 'onClick', 'javascript: return AlarmEditor.CollapseAlarms(this);' );
};
AlarmEditor.CollapseAlarms = function( currentItem )
{
    var pointTagGuid = currentItem.id.replace( AlarmEditor.TagExpandPrefix, '' );
	currentItem.innerHTML = '<img name="expand" draggable="false" title="collapsed" alt="collapsed" src="' + window.applicationRootName + '/fmwebapp/images/Arrow-rightBL.png">';
    $( '#' + AlarmEditor.AlarmPrefix + pointTagGuid ).addClass( 'hidden' );
    currentItem.setAttribute( 'onClick', 'javascript: return AlarmEditor.ExpandAlarms(this);' );
};
AlarmEditor.ExpandTests = function( currentItem )
{
    var pointTagGuid = currentItem.id.replace( AlarmEditor.AlarmExpandPrefix, '' );
	currentItem.innerHTML = '<img name="collapse" draggable="false" title="expanded" alt="expanded" src="' + window.applicationRootName + '/fmwebapp/images/Arrow-downBl.png">';
    $( '#' + AlarmEditor.AlarmTestPrefix + pointTagGuid ).removeClass( 'hidden' );
    currentItem.setAttribute( 'onClick', 'javascript: return AlarmEditor.CollapseTests(this);' );
};
AlarmEditor.CollapseTests = function( currentItem )
{
    var pointTagGuid = currentItem.id.replace( AlarmEditor.AlarmExpandPrefix, '' );
	currentItem.innerHTML = '<img name="expand" draggable="false" title="collapsed" alt="collapsed" src="' + window.applicationRootName + '/fmwebapp/images/Arrow-rightBL.png">';
    $( '#' + AlarmEditor.AlarmTestPrefix + pointTagGuid ).addClass( 'hidden' );
    currentItem.setAttribute( 'onClick', 'javascript: return AlarmEditor.ExpandTests(this);' );
};

//===============================================================================
// This function will disable the alarm group level.
//===============================================================================
AlarmEditor.DisableAlarm = function( currentItem )
{
	var imgId = "#" + currentItem.id;
	$(imgId).attr("onClick", "AlarmEditor.EnableAlarm(this);");

	var imageSource = $(imgId).attr("src");
	imageSource = imageSource.replace("On.png", "Off.png");
	$(imgId).attr("src", imageSource);

	var titleAltStr = $(imgId).attr("title");
	titleAltStr = titleAltStr.replace("toggleOn", "toggleOff");
	$(imgId).attr("title", titleAltStr);
	$(imgId).attr("alt", titleAltStr);

    //Use Guid to update Enabled Status
	var alarmGuid = currentItem.id.replace(AlarmEditor.AlarmEnableImgPrefix, '');
    var model = AlarmEditor.GetAlarmEditorModel();
    for ( var i = 0; i < model.Tags.length; i++ )
    {
        for ( var j = 0; j < model.Tags[i].Alarms.length; j++ )
        {
            if ( model.Tags[i].Alarms[j].AlarmGuid === alarmGuid )
            {
                model.Tags[i].Alarms[j].Enabled = false;
				AlarmEditor.SetAlarmEditorModel(model);
	
				let notifyItemId = currentItem.id.replace('Enabled', 'Notify');

				if (notifyItemId != currentItem.id) {
					let notifyItem = document.querySelector('#'+notifyItemId);
					if (notifyItem) {

						AlarmEditor.NotifyAlarm(notifyItem, false);
						notifyItem.setAttribute("class", "booleanImgSelect disabled");
						notifyItem.removeAttribute("onclick");

					}
				}
                return;
            }
        }
    }
};

//===============================================================================
// This function will enable the alarm group level.
//===============================================================================
AlarmEditor.EnableAlarm = function( currentItem )
{

	var imgId = "#" + currentItem.id;
	$(imgId).attr("onClick", "AlarmEditor.DisableAlarm(this);");

	var imageSource = $(imgId).attr("src");
	imageSource = imageSource.replace("Off.png", "On.png");
	$(imgId).attr("src", imageSource);

	var titleAltStr = $(imgId).attr("title");
	titleAltStr = titleAltStr.replace("toggleOff", "toggleOn");
	$(imgId).attr("title", titleAltStr);
	$(imgId).attr("alt", titleAltStr);

    //Use Guid to update Enabled Status
	var alarmGuid = currentItem.id.replace(AlarmEditor.AlarmEnableImgPrefix, '');
    var model = AlarmEditor.GetAlarmEditorModel();

    for ( var i = 0; i < model.Tags.length; i++ )
    {
        for ( var j = 0; j < model.Tags[i].Alarms.length; j++ )
        {
            if ( model.Tags[i].Alarms[j].AlarmGuid === alarmGuid )
            {
                model.Tags[i].Alarms[j].Enabled = true;
				AlarmEditor.SetAlarmEditorModel(model);

				let notifyItemId = currentItem.id.replace('Enabled', 'Notify');

				if (notifyItemId != currentItem.id) {
					let notifyItem = document.querySelector('#' + notifyItemId);
					if (notifyItem) {

						AlarmEditor.NotifyAlarm(notifyItem, false);
						notifyItem.removeAttribute("class");
						notifyItem.setAttribute("onclick", "javascript: return AlarmEditor.NotifyAlarm(this, true);");
					}
				}
                return;
            }
        }
	}

};
AlarmEditor.NotifyAlarm = function (currentItem, enableNotify) {
	let imgId = "#" + currentItem.id;
	let imageSource = $(imgId).attr("src");
	let titleAltStr = $(imgId).attr("title");
	if (enableNotify) {
		$(imgId).attr("onClick", "AlarmEditor.NotifyAlarm(this, false);");
		imageSource = imageSource.replace("Off.png", "On.png");
		titleAltStr = titleAltStr.replace("toggleOff", "toggleOn");
	}
	else {
		$(imgId).attr("onClick", "AlarmEditor.NotifyAlarm(this, true);");
		imageSource = imageSource.replace("On.png", "Off.png");
		titleAltStr = titleAltStr.replace("toggleOn", "toggleOff");
	}

	$(imgId).attr("src", imageSource);
	$(imgId).attr("title", titleAltStr);
	$(imgId).attr("alt", titleAltStr);

	//Use Guid to update Notify
	let alarmGuid = currentItem.id.replace(AlarmEditor.AlarmNotifyImgPrefix, '');
	let model = AlarmEditor.GetAlarmEditorModel();

	let tagGuid = '';
	let d = $(imgId).closest("[id ^= 'alarmsTableBody_']");
	if (d && d.length > 0) {
		tagGuid = d[0].id.replace('alarmsTableBody_', '');
	}
	
	//let tagGuid = currentItem.parent.parent.id.replace(AlarmEditor.TagAlarmsNotifyImgPrefix, '');
	let allDisabled = true;
	for (let i = 0; i < model.Tags.length; i++) {
		if (model.Tags[i].PointTagGuid === tagGuid) {

			for (let j = 0; j < model.Tags[i].Alarms.length; j++) {
				if (model.Tags[i].Alarms[j].AlarmGuid === alarmGuid) {
					model.Tags[i].Alarms[j].Notify = enableNotify;
					AlarmEditor.SetAlarmEditorModel(model);

				}
				allDisabled &= !model.Tags[i].Alarms[j].Notify;
			}
			let n = "#" + AlarmEditor.TagAlarmsNotifyImgPrefix + tagGuid;
			let x = $(n);
			if (x) {
				imageSource = x.attr("src");
				titleAltStr = x.attr("title");
				if (allDisabled) {
					x.attr("onClick", "AlarmEditor.NotifyAlarms(this, true);");
					imageSource = imageSource.replace("On.png", "Off.png");
					titleAltStr = titleAltStr.replace("toggleOn", "toggleOff");
				}
				else {
					x.attr("onClick", "AlarmEditor.NotifyAlarms(this, false);");
					imageSource = imageSource.replace("Off.png", "On.png");
					titleAltStr = titleAltStr.replace("toggleOff", "toggleOn");
				}

				x.attr("src", imageSource);
				x.attr("title", titleAltStr);
				x.attr("alt", titleAltStr);
			}

			return;
		}
	}


};

AlarmEditor.NotifyAlarms = function (currentItem, enableNotify, disableItem) {
	let imgId = "#" + currentItem.id;
	let imageSource = $(imgId).attr("src");
	let titleAltStr = $(imgId).attr("title");
	if (enableNotify) {
		$(imgId).attr("onClick", "AlarmEditor.NotifyAlarms(this, false);");
		imageSource = imageSource.replace("Off.png", "On.png");
		titleAltStr = titleAltStr.replace("toggleOff", "toggleOn");
	}
	else {
		$(imgId).attr("onClick", "AlarmEditor.NotifyAlarms(this, true);");
		imageSource = imageSource.replace("On.png", "Off.png");
		titleAltStr = titleAltStr.replace("toggleOn", "toggleOff");
	}

	$(imgId).attr("src", imageSource);
	$(imgId).attr("title", titleAltStr);
	$(imgId).attr("alt", titleAltStr);

	if (disableItem != undefined) {
		if (disableItem) {
			currentItem.removeAttribute("onclick");
			currentItem.setAttribute("class", "booleanImgSelect disabled");
		}
		else {
			currentItem.removeAttribute("class");
			currentItem.setAttribute("onclick", "javascript: return AlarmEditor.NotifyAlarms(this, " + !enableNotify + ");");
		}
	}

	//Use Guid to update Notify
	let tagGuid = currentItem.id.replace(AlarmEditor.TagAlarmsNotifyImgPrefix, '');
	let model = AlarmEditor.GetAlarmEditorModel();

	let d = $(imgId).closest("#alarmTagsTableBody");

	for (let i = 0; i < model.Tags.length; i++) {
		if (model.Tags[i].PointTagGuid === tagGuid) {
			for (let j = 0; j < model.Tags[i].Alarms.length; j++) {
				model.Tags[i].Alarms[j].Notify = enableNotify;
				AlarmEditor.SetAlarmEditorModel(model);

			}
			let n = d.find("[id='alarmsTableBody_" + tagGuid + "']");

			if (n) {
				let e = n.find("[id ^= '" + AlarmEditor.AlarmEnableImgPrefix + "']");
				n = n.find("[id ^= '" + AlarmEditor.AlarmNotifyImgPrefix + "']");

				let f = new Array();
				let j = 0;
				for (let i = 0; i < e.length; i++) {
					let m = n[i];
					if (e[i].alt == "toggleOn") {

						imageSource = m.src;
						titleAltStr = m.title;
						if (enableNotify) {
							m.setAttribute("onClick", "AlarmEditor.NotifyAlarm(this, false)");
							imageSource = imageSource.replace("Off.png", "On.png");
							titleAltStr = titleAltStr.replace("toggleOff", "toggleOn");
						}
						else {
							m.setAttribute("onClick", "AlarmEditor.NotifyAlarm(this, true)");
							imageSource = imageSource.replace("On.png", "Off.png");
							titleAltStr = titleAltStr.replace("toggleOn", "toggleOff");
						}
						m.src = imageSource;
						m.title = titleAltStr;
						m.alt = titleAltStr;
					}

					if (disableItem != undefined) { 
						if (disableItem) {
							m.setAttribute("class", "booleanImgSelect disabled");
							m.removeAttribute("onclick");
						}
						else {
							m.removeAttribute("class");
							//notifyItem.onclick = function () { return AlarmEditor.NotifyAlarms(this, false); };
							m.setAttribute("onclick", "javascript: return AlarmEditor.NotifyAlarm(this, true);");
						}
					}
				}
			}
			return;
		}
	}
};

//===============================================================================
// This function will disable the alarms at composite level.
//===============================================================================
AlarmEditor.DisableAlarms = function( currentItem )
{
	var imgId = "#" + currentItem.id;
	$(imgId).attr("onClick", "AlarmEditor.EnableAlarms(this);");

	var imageSource = $(imgId).attr("src");
	imageSource = imageSource.replace("On.png", "Off.png");
	$(imgId).attr("src", imageSource);

	var titleAltStr = $(imgId).attr("title");
	titleAltStr = titleAltStr.replace("toggleOn", "toggleOff");
	$(imgId).attr("title", titleAltStr);
	$(imgId).attr("alt", titleAltStr);

    //Use Guid to update Enabled Status
	var tagGuid = currentItem.id.replace(AlarmEditor.TagAlarmsEnableImgPrefix, '');
    var model = AlarmEditor.GetAlarmEditorModel();

    for ( var i = 0; i < model.Tags.length; i++ )
    {
        if ( model.Tags[i].PointTagGuid === tagGuid )
        {
            model.Tags[i].AlarmsEnabled = false;
            AlarmEditor.SetAlarmEditorModel( model );
			
			let notifyItemId = currentItem.id.replace('AlarmsEnable', 'TagAlarmsNotify');

			if (notifyItemId != currentItem.id) {
				let notifyItem = document.querySelector('#'+notifyItemId);
				if (notifyItem) {
					AlarmEditor.NotifyAlarms(notifyItem, false, true);

				}
			}

			return;
        }
    }
};

//===============================================================================
// This function will enable the alarms at composite level.
//===============================================================================
AlarmEditor.EnableAlarms = function (currentItem)
{
	var imgId = "#" + currentItem.id;
	$(imgId).attr("onClick", "AlarmEditor.DisableAlarms(this);");

	var imageSource = $(imgId).attr("src");
	imageSource = imageSource.replace("Off.png", "On.png");
	$(imgId).attr("src", imageSource);

	var titleAltStr = $(imgId).attr("title");
	titleAltStr = titleAltStr.replace("toggleOff", "toggleOn");
	$(imgId).attr("title", titleAltStr);
	$(imgId).attr("alt", titleAltStr);

    //Use Guid to update Enabled Status
	var tagGuid = currentItem.id.replace(AlarmEditor.TagAlarmsEnableImgPrefix, '');
	var model = AlarmEditor.GetAlarmEditorModel();

    for ( var i = 0; i < model.Tags.length; i++ )
    {
        if ( model.Tags[i].PointTagGuid === tagGuid )
        {
            model.Tags[i].AlarmsEnabled = true;
			AlarmEditor.SetAlarmEditorModel(model);
			
			let notifyItemId = currentItem.id.replace('AlarmsEnable', 'TagAlarmsNotify');

			if (notifyItemId != currentItem.id) {
				let notifyItem = document.querySelector('#' + notifyItemId);
				if (notifyItem) {
					AlarmEditor.NotifyAlarms(notifyItem, false, false);

				}
			}
			return;
        }
    }
};

AlarmEditor.EditTestEquation = function( currentItem )
{
    //Use Guid to update Test Equation
    var alarmTestGuid = currentItem.id.replace( AlarmEditor.EquationSprocketPrefix, '' );
    var model = AlarmEditor.GetAlarmEditorModel();
    var pointGuid = model.PointGuid;
    var token = $( '#pointPropertiesForm input[name=__RequestVerificationToken]' ).val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;
    var url = $( '#urlGetAlarmTestEquationEditor' ).val();

    $.ajax( {
        type: 'POST',
        cache: false,
        headers: headers,
        url: url,
        data: { pointGuid: pointGuid, alarmTestGuid: alarmTestGuid },
        success: function( response )
        {
            FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
            {
                if ( !inError || ( data != null ) )
                {
                    $( '#AlarmEditorEquationEditorScreen' ).html( data );
                    $( '#AlarmEditorEquationEditor' ).modal( 'show' );
                }
            } );
        },
        error: function( xhr, textStatus, error )
        {
            FMErrorAndExceptionHandling.ShowException( xhr, textStatus, error, function()
            {
            } );
        }
    } );
};
AlarmEditor.GetTagField = function( field )
{
    switch ( field )
    {
        case '0':
            return '.Value';
        case '1':
            return '.Status';
        case '2':
            return '.OpcUAStatusSubcode';
        default:
            return '';
    }
};
AlarmEditor.GetBitmask = function( tagAttribute, useBitmask, bitmask, bitwiseOperator )
{
    if ( useBitmask )
    {
        return ' ' + AlarmEditor.GetBitwiseOperator( bitwiseOperator.toString() ) + ' ' + bitmask + ' ';
    }
    return '';
};
AlarmEditor.GetBitwiseOperator = function( bitwiseOperator )
{
    switch ( bitwiseOperator )
    {
        case '0':
            return '&';
        case '1':
            return '|';
        case '3':
            return 'NOR';
        case '4':
            return '^';
        default:
            return '';
    }
};
AlarmEditor.GetComparison = function( comparisonOperator )
{
    switch ( comparisonOperator )
    {
        case '0':
            return '>';
        case '1':
            return '>=';
        case '2':
            return '<';
        case '3':
            return '<=';
        case '4':
            return '=';
        case '5':
            return '<>';
        default:
            return '';
    }
};
AlarmEditor.SaveTestEquationChanges = function()
{
    var ateModel = AlarmTestEquationEditor.GetUpdates();
    var model = AlarmEditor.GetAlarmEditorModel();
    var alarmTestEquation = ateModel.TagName + AlarmEditor.GetTagField( ateModel.TagAttribute ) + ' ' + AlarmEditor.GetBitmask( ateModel.TagAttribute, ateModel.UseBitmask, ateModel.Bitmask, ateModel.BitwiseOperator ) + AlarmEditor.GetComparison( ateModel.ComparisonOperator ) + ' ' + ateModel.LimitName;
    $( '#AlarmEditorEquationEditor' ).modal( 'hide' );

    //update model
    for ( var i = 0; i < model.Tags.length; i++ )
    {
        if ( model.Tags[i].PointTagGuid === ateModel.PointTagGuid )
        {
            for ( var j = 0; j < model.Tags[i].Alarms.length; j++ )
            {
                for ( var k = 0; k < model.Tags[i].Alarms[j].AlarmTests.length; k++ )
                {
                    if ( model.Tags[i].Alarms[j].AlarmTests[k].AlarmTestGuid === ateModel.AlarmTestGuid )
                    {
                        model.Tags[i].Alarms[j].AlarmTests[k].AlarmTestEquation = alarmTestEquation;
                    }
                }
            }
        }
    }
    AlarmEditor.SetAlarmEditorModel( model );

    //udate alarmtestequation field
    var alarmTestEquationDivId = AlarmEditor.TestTestCellDivPrefix + ateModel.AlarmTestGuid;
    var equationSprocketId = AlarmEditor.EquationSprocketPrefix + ateModel.AlarmTestGuid;
    var innerHtml = alarmTestEquation + '<Span class="alarmEditorSprocketSpan"><a id="' + equationSprocketId + '" class="equationSprocket glyphicon glyphicon-cog" href=\'javascript:;\' onclick=\'AlarmEditor.EditTestEquation( this );\'></a></Span>';
    var div = document.getElementById( alarmTestEquationDivId );
    if ( div )
    {
        div.innerHTML = innerHtml;
        div.title = alarmTestEquation;
    }
};
AlarmEditor.EditHelpFile = function( currentItem )
{
    //Use Guid to update HelpFile
    //var alarmTestGuid = currentItem.id.replace(AlarmEditor.HelpFileSprocketPrefix, "");
};
AlarmEditor.EditsuppressedShelved = function( currentItem )
{
    //Use Guid to update Shelved Status
    //var alarmGuid = currentItem.id.replace(AlarmEditor.SuppressedShelvedSprocketPrefix, "");
};
AlarmEditor.EditDoubleKeyHandler = function( e )
{
    e = e || event;
    if ( ( e.keyCode || e.which || e.charCode || 0 ) < 46 )
    {
        return false;
    }
    if ( ( e.keyCode || e.which || e.charCode || 0 ) > 57 )
    {
        return false;
    }
    if ( ( e.keyCode || e.which || e.charCode || 0 ) === 47 )
    {
        return false;
    }
    return true;
};
AlarmEditor.EditStringKeyHandler = function( e )
{
    e = e || event;
    if ( ( e.keyCode || e.which || e.charCode || 0 ) === 13 )
    {
        return false;
    }
    else if ( ( e.keyCode || e.which || e.charCode || 0 ) === 27 )
    {
        return false;
    }
    return true;
}; //==================================================================================
// This function will update the Test column based on the Limit Tag change.
//==================================================================================
AlarmEditor.HandleUpdatingEditLimitTagValue = function()
{
    if ( $( '#PointTagNewValueInputId' ).val() == null || $( '#PointTagNewValueInputId' ).val() === '' )
    {
        return;
    }
	
    try
    {
        var testCellValue = $( '#' + AlarmEditor.TestTestCellToUpdate ).text();
        var parts = testCellValue.split( ' ' );

        if ( parts.length > 1 )
        {
            var newStr = '';
            var length = parts.length - 1;

            for ( var nextPart = 0; nextPart < length; nextPart++ )
            {
                newStr = newStr + parts[nextPart] + ' ';
            }

            newStr = newStr + '[' + $( '#PointTagNewValueInputId' ).val() + ']';
            $( '#' + AlarmEditor.TestTestCellToUpdate ).text( newStr );
        }
    }
    catch ( e )
    {
        // On error ignore and do not update.
    }
}; //==================================================================================
// This function will close the edit value modal dialog.
//==================================================================================
AlarmEditor.CloseEditLimitTagValue = function()
{
    $( '#confirm-exceed-min-dialog' ).remove();
    $( '#confirm-exceed-max-dialog' ).remove();
    $( '#PointTagEditValueOkButton' ).unbind( 'click' );
    $( '#PointTagEditValueCancelButton' ).unbind( 'click' );
    $( '#AlarmEditorSetValueModalBody' ).html( '' );
}; //==================================================================================
// This function will display the Edit Value modal dialog. The body of the modal
// dialog is populated with the results from the Ajax call. This result is html
// data that is in EditValue.cshtm.
//==================================================================================
AlarmEditor.EditLimitTagValue = function( limitTagGuid, testTestCellId )
{
    AlarmEditor.TestTestCellToUpdate = '';
	
    if ( limitTagGuid == null || limitTagGuid === '' || testTestCellId == null || testTestCellId === '' )
    {
        return;
    }
	
    // Set the Test Test Cell ID that needs to be updated by the change in the limit tag.
    // Clear the new value input value.
    AlarmEditor.TestTestCellToUpdate = testTestCellId;
    $( '#PointTagNewValueInputId' ).val( '' );

    var pointValueIdentifier = { IdentityGuid: limitTagGuid, PointValueType: 'Tag', PropertyID: '' };

    // create the backdrop and wait for next modal to be triggered
    $( 'body' ).modalmanager( 'loading' );
    var token = $( 'input[name=__RequestVerificationToken]' ).val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    var url = $( '#urlAlarmEditorEditValue' ).val();

    $.ajax( {
        type: 'POST',
        headers: headers,
        url: url,
        cache: false,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify( { 'pointValueIdentifier': pointValueIdentifier } ),
        success: function( response, xhr, settings )
        {
            // instead of using notifications we are using alerts to force the user to acknowledge the error 
            if ( response && response.ErrorMessage )
            {
                var count = 0;

                $.each( response.ErrorMessage, function( key, message )
                {
                    count = count + 1;
                } );


                if ( count === 0 )
                {
                    $( '#PointTagEditValueScreenBody' ).html( response.Data );
                    $( '#PointTagEditValueScreen' ).modal( 'show' );
                }
                else
                {
                    // add messages but avoid duplicates
                    var seen = {};

                    $.each( response.ErrorMessage, function( key, message )
                    {
                        $.each( message, function( i )
                        {
                            if ( !seen[message[i]] )
                            {
                                seen[message[i]] = true;
                                FMLayout.Alert( message[i], 'Tag Value Editor' );
                            }
                        } );
                    } );

                    $( 'body' ).modalmanager( 'removeLoading' );;
                }
            }
        },
        error: function( xhr, textStatus, error )
        {
            FMErrorAndExceptionHandling.ShowException( xhr,
                textStatus,
                error,
                function()
                {
                    // remove the loading of the modal
                    var modalManager = $( 'body' ).data( 'modalmanager' );
                    modalManager.removeLoading();
                } );
        }
    } );
}; //==================================================================================
// This function will open a drawing select file dialog.
//==================================================================================
AlarmEditor.EditDiagram = function( currentItem )
{
    AlarmEditor.SelectedDiagramSprockId = null;

    var token = $( 'input[name=__RequestVerificationToken]' ).val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    var url = $( '#urlGetGraphicNames' ).val();

    $.ajax( {
        type: 'get',
        dataType: 'json',
        cache: false,
        headers: headers,
        url: url,
        success: function( response )
        {
            FMErrorAndExceptionHandling.HandleMessages( response, function( drawings, inError )
            {
                if ( inError )
                {
                    return;
                }

                AlarmEditor.GetDrawingNamesSuccess( drawings );
                AlarmEditor.HandleDisplayGraphicNameDialogEvent();

                var parts = currentItem.id.split( '_' );

                if ( parts != null && parts.length > 1 )
                {
                    AlarmEditor.SelectedDiagramSprockId = parts[1];
                }

                // Set the graphic name selection that was previously saved.
                AlarmEditor.SetPreviousSelectedGraphic();
            } );
        },
        error: function( ex )
        {
            FMErrorAndExceptionHandling.ShowError( 'Error loading drawing names: ' + ex.responseText );
        }
    } );
}; //==================================================================================
// This function will add the drawing file name selection to the list.
// Called by function "AlarmEditor.EditDiagram()".
//==================================================================================
AlarmEditor.GetDrawingNamesSuccess = function( result )
{
    var graphicNameSelectControl = document.getElementById( 'GraphicNameDropdown' );
    var selectOptLength = graphicNameSelectControl.length - 1;

    // Clear the dropdown list with the exception of the None entry.
    for ( var nextOptIndex = selectOptLength; nextOptIndex > 0; nextOptIndex-- )
    {
        graphicNameSelectControl.remove( nextOptIndex );
    }

    $.each( result, function( index, item )
    {
        $( '#GraphicNameDropdown' ).append( $( '<option/>', { value: item.DrawingGuid, text: item.ID } ) );
    } );
}; //=======================================================================================
// This function will set the graphic file name in the graphic dropdown.
//=======================================================================================
AlarmEditor.SetPreviousSelectedGraphic = function()
{
    if ( AlarmEditor.SelectedDiagramSprockId == null )
    {
        return;
    }

    var model = AlarmEditor.GetAlarmEditorModel();
    var alarmTestObj = AlarmEditor.FindAlarmTest( model );

    if ( alarmTestObj != null )
    {
        $( '#GraphicNameDropdown > option' ).each( function()
        {
            var drawingGuid = $( this ).val();
            if ( drawingGuid === alarmTestObj.DrawingGuid )
            {
                // Set the selected dropdown option.
                $( this ).prop( 'selected', true );
            }
        } );
    }
}; //=====================================================================================
// This function will handle the graphic name select dialog ok button event.
//=====================================================================================
AlarmEditor.HandleGraphicOkBtnEvent = function()
{
    // Get the Graphic selected.
    var selectedGraphicValue = $( '#GraphicNameDropdown' ).find( ':selected' ).val();
    var selectedGraphicId = $( '#GraphicNameDropdown' ).find( ':selected' ).text();

    var model = AlarmEditor.GetAlarmEditorModel();
    var alarmTestObj = AlarmEditor.FindAlarmTest( model );

    // Save the selected graphic file name in the model.
    if ( alarmTestObj != null )
    {
        if ( selectedGraphicValue === '-99' )
        {
            alarmTestObj.DrawingGuid = '00000000-0000-0000-0000-000000000000';
            alarmTestObj.DrawingId = null;
        }
        else
        {
            alarmTestObj.DrawingGuid = selectedGraphicValue;
            alarmTestObj.DrawingId = selectedGraphicId;
        }

        // Update the model with the changes.
        AlarmEditor.SetAlarmEditorModel( model );

        // Save to database
        AlarmEditor.UpdateAlarmTest( alarmTestObj.AlarmTestGuid );
    }
}; //=====================================================================================
// This function will update the Alarm Test object to the database.
//=====================================================================================
AlarmEditor.UpdateAlarmTest = function( alarmTestGuid )
{
    var modelStr = AlarmEditor.GetAlarmEditorModelString();

    var url = $( '#urlUpdateAlarmTest' ).val();
    var token = $( 'input[name=__RequestVerificationToken]' ).val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    $.ajax( {
        cache: false,
        type: 'POST',
        async: false,
        dataType: 'json',
        url: url,
        headers: headers,
        data: { modelStr: modelStr, alarmTestGuidStr: alarmTestGuid },
        success: function( result )
        {
            if ( result === 'ERROR' )
            {
                FMErrorAndExceptionHandling.ShowError( 'Error saving Graphic.', null, null );
            }
        },
        error: function( e )
        {
            FMErrorAndExceptionHandling.ShowError( 'Error saving Graphic.', null, null );
        }
    } );
}; //==========================================================================================
// This function will handle the right click on the table header.
//==========================================================================================
AlarmEditor.HandleDisplayGraphicNameDialogEvent = function()
{
    $( '#AlarmEditorGraphicModalDiv' ).modal( 'show' );
}; //==========================================================================================
// This function will find the alarm test object based on the selected diagram sprock.
// It returns the alarm test object if found, otherwise it returns null.
//==========================================================================================
AlarmEditor.FindAlarmTest = function( model )
{
    if ( model == null || AlarmEditor.SelectedDiagramSprockId == null )
    {
        return null;
    }

    for ( var nextTag = 0; nextTag < model.Tags.length; nextTag++ )
    {
        var tag = model.Tags[nextTag];

        if ( tag == null )
        {
            break;
        }

        for ( var nextAlarm = 0; nextAlarm < tag.Alarms.length; nextAlarm++ )
        {
            var alarm = tag.Alarms[nextAlarm];
            if ( alarm == null )
            {
                break;
            }

            for ( var nextAlarmTest = 0; nextAlarmTest < alarm.AlarmTests.length; nextAlarmTest++ )
            {
                var alarmTest = alarm.AlarmTests[nextAlarmTest];
                if ( alarmTest == null )
                {
                    break;
                }

                if ( alarmTest.AlarmTestGuid === AlarmEditor.SelectedDiagramSprockId )
                {
                    return alarmTest;
                }
            }
        }
    }

    return null;
};
AlarmEditor.ExpandEnabledAlarms = function( currentItem )
{
	var collapseDD = $("#CollapseDataDictionary").val();
	var innerHtml = '<Span style="vertical-align: middle; padding: 0; border: 0;"><img src="' + window.applicationRootName + '/FMWebApp/Images/minus.png" class="expandContractButtonImageClass" alt=@FMBaseController.TranslateText("Collapse") /></Span>' + collapseDD;
    currentItem.innerHTML = innerHtml;
    currentItem.setAttribute( 'onClick', 'javascript: return AlarmEditor.ContractEnabledAlarms(this);' );
    var model = AlarmEditor.GetAlarmEditorModel();
    for ( var i = 0; i < model.Tags.length; i++ )
    {
        if ( model.Tags[i].AlarmsEnabled )
        {
            var tag = document.getElementById( AlarmEditor.TagExpandPrefix + model.Tags[i].PointTagGuid );
            AlarmEditor.ExpandAlarms( tag );
            for ( var j = 0; j < model.Tags[i].Alarms.length; j++ )
            {
                if ( model.Tags[i].Alarms[j].Enabled )
                {
                    var alarm = document.getElementById( AlarmEditor.AlarmExpandPrefix + model.Tags[i].Alarms[j].AlarmGuid );
                    AlarmEditor.ExpandTests( alarm );
                }
            }
        }
    }
};
AlarmEditor.ContractEnabledAlarms = function( currentItem )
{
	var expandDD = $( "#ExpandDataDictionary" ).val();
	var innerHtml = '<Span style="vertical-align: middle; padding: 0; border: 0;"><img src="' + window.applicationRootName + '/FMWebApp/Images/plus.png" class="expandContractButtonImageClass" alt=@FMBaseController.TranslateText("Expand") /></Span>' + expandDD;
    currentItem.innerHTML = innerHtml;
    currentItem.setAttribute( 'onClick', 'javascript: return AlarmEditor.ExpandEnabledAlarms(this);' );
    var model = AlarmEditor.GetAlarmEditorModel();
    for ( var i = 0; i < model.Tags.length; i++ )
    {
        var tag = document.getElementById( AlarmEditor.TagExpandPrefix + model.Tags[i].PointTagGuid );
        AlarmEditor.CollapseAlarms( tag );
        for ( var j = 0; j < model.Tags[i].Alarms.length; j++ )
        {
            var alarm = document.getElementById( AlarmEditor.AlarmExpandPrefix + model.Tags[i].Alarms[j].AlarmGuid );
            AlarmEditor.CollapseTests( alarm );
        }
    }
};
AlarmEditor.SaveAlarmText = function( currentItem )
{
    //Need this code because of a refocus fix for IE that Javi put in
    if ( $( '#' + currentItem.id ).is( ':focus' ) )
    {
        return;
    }

    var model = AlarmEditor.GetAlarmEditorModel();
    var alarmTestGuid = currentItem.id.replace( AlarmEditor.AlarmTextInputPrefix, '' );
    for ( var i = 0; i < model.Tags.length; i++ )
    {
        for ( var j = 0; j < model.Tags[i].Alarms.length; j++ )
        {
            for ( var k = 0; k < model.Tags[i].Alarms[j].AlarmTests.length; k++ )
            {
                if ( model.Tags[i].Alarms[j].AlarmTests[k].AlarmTestGuid === alarmTestGuid )
                {
                    model.Tags[i].Alarms[j].AlarmTests[k].AlarmText = $( '#' + currentItem.id ).val();
                    var item = document.getElementById( currentItem.id );
                    if ( item )
                    {
                        item.title = model.Tags[i].Alarms[j].AlarmTests[k].AlarmText;
                    }
                    AlarmEditor.SetAlarmEditorModel( model );
                    return;
                }
            }
        }
    }
};
AlarmEditor.SaveAlarmState = function( currentItem )
{
    //Need this code because of a refocus fix for IE that Javi put in
    if ( $( '#' + currentItem.id ).is( ':focus' ) )
    {
        return;
    }

    var model = AlarmEditor.GetAlarmEditorModel();
    var alarmTestGuid = currentItem.id.replace( AlarmEditor.AlarmStateInputPrefix, '' );
    for ( var i = 0; i < model.Tags.length; i++ )
    {
        for ( var j = 0; j < model.Tags[i].Alarms.length; j++ )
        {
            for ( var k = 0; k < model.Tags[i].Alarms[j].AlarmTests.length; k++ )
            {
                if ( model.Tags[i].Alarms[j].AlarmTests[k].AlarmTestGuid === alarmTestGuid )
                {
	                if ($('#' + currentItem.id).val() != "")
	                {
		                model.Tags[i].Alarms[j].AlarmTests[k].AlarmState = $( '#' + currentItem.id ).val();
		                var item = document.getElementById( currentItem.id );
		                if ( item )
		                {
			                item.title = model.Tags[i].Alarms[j].AlarmTests[k].AlarmState;
		                }
		                AlarmEditor.SetAlarmEditorModel( model );
		                return;
	                }
	                else
	                {
							// if the box is blank, revert the change. The alarm state text should not be null. 
	                	$('#' + currentItem.id).val(model.Tags[i].Alarms[j].AlarmTests[k].AlarmState);
	                	FMErrorAndExceptionHandling.ShowError('Alarm state text cannot be null');
		                return;
	                }
                }
            }
        }
    }
};
AlarmEditor.SaveLimitValue = function( currentItem )
{
	//Need this code because of a refocus fix for IE that Javi put in
	if ( $( '#' + currentItem.id ).is( ':focus' ) )
	{
		return;
	}

	var model = AlarmEditor.GetAlarmEditorModel();
	var alarmTestGuid = currentItem.id.replace( AlarmEditor.LimitValueInputPrefix, "" );
	var limitItem = $( "#" + currentItem.id );
	for ( var i = 0; i < model.Tags.length; i++ )
	{
		for ( var j = 0; j < model.Tags[i].Alarms.length; j++ )
		{
			for ( var k = 0; k < model.Tags[i].Alarms[j].AlarmTests.length; k++ )
			{
				if ( model.Tags[i].Alarms[j].AlarmTests[k].AlarmTestGuid === alarmTestGuid )
				{
					var numFormatInfoString = $( '#alarmEditorNumberFormatInfo' ).val();
					var siteNumFormatInfo = JSON.parse( numFormatInfoString );
					siteNumFormatInfo.NumberDecimalDigits = model.Tags[i].Alarms[j].AlarmTests[k].LimitTagDecimalPlaces;
					var unitIndex = model.Tags[i].Alarms[j].AlarmTests[k].LimitTagUnits;

					try
					{
						var newValue = FMFormatValues.ParseValue( unitIndex, siteNumFormatInfo, limitItem.val(), true );

						if ( newValue === null || isNaN( newValue ) )
						{
							limitItem.val( model.Tags[i].Alarms[j].AlarmTests[k].LimitValue );
							FMErrorAndExceptionHandling.ShowError( "Limit Value Format Error" );
							AlarmEditor.LastValidationPassed = false;
							AlarmEditor.LastValidationId = currentItem.id;
							return;
						}

						//Need to make sure max and min apply for data types of limit tag
						if ( newValue < model.Tags[i].Alarms[j].AlarmTests[k].LimitTagMin )
						{
							limitItem.val( model.Tags[i].Alarms[j].AlarmTests[k].LimitValue );
							FMErrorAndExceptionHandling.ShowError( "Entered Limit Value Less Than Minimum: " + model.Tags[i].Alarms[j].AlarmTests[k].LimitTagMin );
							AlarmEditor.LastValidationPassed = false;
							AlarmEditor.LastValidationId = currentItem.id;
							return;
						}

						if ( newValue > model.Tags[i].Alarms[j].AlarmTests[k].LimitTagMax )
						{
							limitItem.val( model.Tags[i].Alarms[j].AlarmTests[k].LimitValue );
							FMErrorAndExceptionHandling.ShowError( "Entered Limit Value Greater Than Maximum: " + model.Tags[i].Alarms[j].AlarmTests[k].LimitTagMax );
							AlarmEditor.LastValidationPassed = false;
							AlarmEditor.LastValidationId = currentItem.id;
							return;
						}
						
						var newValueStr = FMFormatValues.FormatValueFullPrecision( unitIndex, siteNumFormatInfo, newValue );
						limitItem.val( newValueStr );
						model.Tags[i].Alarms[j].AlarmTests[k].LimitValue = newValueStr;
						AlarmEditor.SetAlarmEditorModel( model );
						AlarmEditor.LastValidationPassed = true;
						AlarmEditor.LastValidationId = currentItem.id;
						return;
					}
					catch ( err )
					{
						limitItem.val( model.Tags[i].Alarms[j].AlarmTests[k].LimitValue );
						FMErrorAndExceptionHandling.ShowError( "Limit Value Format Error" );
						AlarmEditor.LastValidationPassed = false;
						AlarmEditor.LastValidationId = currentItem.id;
						return;
					}
				}
			}
		}
	}
	AlarmEditor.LastValidationPassed = true;
	AlarmEditor.LastValidationId = currentItem.id;
};

AlarmEditor.SaveHysteresis = function( currentItem )
{
	//Need this code because of a refocus fix for IE that Javi put in
	if ( $( "#" + currentItem.id ).is( ":focus" ) )
	{
		return;
	}

	var limitItem = $( '#' + currentItem.id );
	var model = AlarmEditor.GetAlarmEditorModel();
	var tagGuid = currentItem.id.replace( AlarmEditor.TagHysteresisPrefix, '' );
	for ( var i = 0; i < model.Tags.length; i++ )
	{
		if ( model.Tags[i].PointTagGuid === tagGuid )
		{
			var numFormatInfoString = $( '#alarmEditorNumberFormatInfo' ).val();
			var siteNumFormatInfo = JSON.parse(numFormatInfoString);

			var inputTagInfo = jQuery.grep(FMPointEditor.tagList, function (row) { return row.PointTagGuid === tagGuid; });
			if ( inputTagInfo.length > 0 )
			{
				siteNumFormatInfo.NumberDecimalDigits = inputTagInfo[0].DecimalPlaces;
				var unitIndex = parseInt(inputTagInfo[0].Units);
				try
				{
					var newValue = FMFormatValues.ParseValue( unitIndex, siteNumFormatInfo, limitItem.val(), true );
					if ( newValue === null || isNaN( newValue ) )
					{
						limitItem.val( model.Tags[i].Hysteresis );
						FMErrorAndExceptionHandling.ShowError( "Deadband Format Error" );
						AlarmEditor.LastValidationPassed = false;
						AlarmEditor.LastValidationId = currentItem.id;
						return;
					}
					if ( newValue < 0 )
					{
						limitItem.val( model.Tags[i].Hysteresis );
						FMErrorAndExceptionHandling.ShowError( "Deadband Cannot Be Negative" );
						AlarmEditor.LastValidationPassed = false;
						AlarmEditor.LastValidationId = currentItem.id;
						return;
					}
					
					var newValueStr = FMFormatValues.FormatValueFullPrecision( unitIndex, siteNumFormatInfo, newValue );
					limitItem.val( newValueStr );
					model.Tags[i].Hysteresis = newValueStr;
					AlarmEditor.SetAlarmEditorModel( model );
					AlarmEditor.LastValidationPassed = true;
					AlarmEditor.LastValidationId = currentItem.id;
					return;
				}
				catch ( err )
				{
					limitItem.val( model.Tags[i].Hysteresis );
					FMErrorAndExceptionHandling.ShowError( "Deadband Format Error" );
					AlarmEditor.LastValidationPassed = false;
					AlarmEditor.LastValidationId = currentItem.id;
					return;
				}
			}
		}
	}
	AlarmEditor.LastValidationPassed = true;
	AlarmEditor.LastValidationId = currentItem.id;
};

AlarmEditor.SaveHoldOffSeconds = function (currentItem) {
	//Need this code because of a refocus fix for IE that Javi put in
	if ($("#" + currentItem.id).is(":focus")) {
		return;
	}

	var limitItem = $('#' + currentItem.id);
	var model = AlarmEditor.GetAlarmEditorModel();
	var alarmTestGuid = currentItem.id.replace(AlarmEditor.TagHoldOffSecondsPrefix, '');
	for (var i = 0; i < model.Tags.length; i++) 
	{
		for ( var j = 0; j < model.Tags[i].Alarms.length; j++ )
		{
			for ( var k = 0; k < model.Tags[i].Alarms[j].AlarmTests.length; k++ )
			{
				if ( model.Tags[i].Alarms[j].AlarmTests[k].AlarmTestGuid === alarmTestGuid )
				{
					var numFormatInfoString = $( '#alarmEditorNumberFormatInfo' ).val();
					var siteNumFormatInfo = JSON.parse( numFormatInfoString );
					siteNumFormatInfo.NumberDecimalDigits = 0;
					var unitIndex = 6; //FMT_Sec

					try
					{
						var newValue = FMFormatValues.ParseValue( unitIndex, siteNumFormatInfo, limitItem.val(), true );
						if ( newValue === null || isNaN( newValue ) )
						{
							limitItem.val(model.Tags[i].Alarms[j].AlarmTests[k].HoldOffSeconds);
							FMErrorAndExceptionHandling.ShowError( "HoldOff Seconds Format Error" );
							AlarmEditor.LastValidationPassed = false;
							AlarmEditor.LastValidationId = currentItem.id;
							return;
						}
						if ( newValue < 0 )
						{
							limitItem.val(model.Tags[i].Alarms[j].AlarmTests[k].HoldOffSeconds);
							FMErrorAndExceptionHandling.ShowError( "HoldOff Seconds Cannot Be Negative" );
							AlarmEditor.LastValidationPassed = false;
							AlarmEditor.LastValidationId = currentItem.id;
							return;
						}
						if ( newValue > 59 )
						{
							limitItem.val(model.Tags[i].Alarms[j].AlarmTests[k].HoldOffSeconds);
							FMErrorAndExceptionHandling.ShowError( "HoldOff Seconds Cannot Be Greater Than 59" );
							AlarmEditor.LastValidationPassed = false;
							AlarmEditor.LastValidationId = currentItem.id;
							return;
						}
						
						var newValueStr = FMFormatValues.FormatValueFullPrecision( unitIndex, siteNumFormatInfo, newValue );
						limitItem.val( newValueStr );
						model.Tags[i].Alarms[j].AlarmTests[k].HoldOffSeconds = newValueStr;
						AlarmEditor.SetAlarmEditorModel( model );
						AlarmEditor.LastValidationPassed = true;
						AlarmEditor.LastValidationId = currentItem.id;
						return;
					}
					catch ( err )
					{
						limitItem.val(model.Tags[i].Alarms[j].AlarmTests[k].HoldOffSeconds);
						FMErrorAndExceptionHandling.ShowError( "HoldOff Seconds Format Error" );
						AlarmEditor.LastValidationPassed = false;
						AlarmEditor.LastValidationId = currentItem.id;
						return;
					}
				}
			}
		}
	}
	AlarmEditor.LastValidationPassed = true;
	AlarmEditor.LastValidationId = currentItem.id;
};

AlarmEditor.SaveHoldOffMinutes = function (currentItem) {
	//Need this code because of a refocus fix for IE that Javi put in
	if ($("#" + currentItem.id).is(":focus")) {
		return;
	}

	var limitItem = $('#' + currentItem.id);
	var model = AlarmEditor.GetAlarmEditorModel();
	var alarmTestGuid = currentItem.id.replace(AlarmEditor.TagHoldOffMinutesPrefix, '');
	for (var i = 0; i < model.Tags.length; i++)
	{
		for ( var j = 0; j < model.Tags[i].Alarms.length; j++ )
		{
			for ( var k = 0; k < model.Tags[i].Alarms[j].AlarmTests.length; k++ )
			{
				if ( model.Tags[i].Alarms[j].AlarmTests[k].AlarmTestGuid === alarmTestGuid )
				{
					var numFormatInfoString = $( '#alarmEditorNumberFormatInfo' ).val();
					var siteNumFormatInfo = JSON.parse( numFormatInfoString );
					siteNumFormatInfo.NumberDecimalDigits = 0;
					var unitIndex = 6; //FMT_Sec

					try
					{
						var newValue = FMFormatValues.ParseValue( unitIndex, siteNumFormatInfo, limitItem.val(), true );
						if ( newValue === null || isNaN( newValue ) )
						{
							limitItem.val(model.Tags[i].Alarms[j].AlarmTests[k].HoldOffMinutes);
							FMErrorAndExceptionHandling.ShowError( "HoldOff Minutes Format Error" );
							AlarmEditor.LastValidationPassed = false;
							AlarmEditor.LastValidationId = currentItem.id;
							return;
						}
						if ( newValue < 0 )
						{
							limitItem.val(model.Tags[i].Alarms[j].AlarmTests[k].HoldOffMinutes);
							FMErrorAndExceptionHandling.ShowError( "HoldOff Minutes Cannot Be Negative" );
							AlarmEditor.LastValidationPassed = false;
							AlarmEditor.LastValidationId = currentItem.id;
							return;
						}
						if ( newValue > 1440 )
						{
							limitItem.val(model.Tags[i].Alarms[j].AlarmTests[k].HoldOffMinutes);
							FMErrorAndExceptionHandling.ShowError( "HoldOff Minutes Cannot Be Greater Than 1440" );
							AlarmEditor.LastValidationPassed = false;
							AlarmEditor.LastValidationId = currentItem.id;
							return;
						}
						
						var newValueStr = FMFormatValues.FormatValueFullPrecision( unitIndex, siteNumFormatInfo, newValue );
						limitItem.val( newValueStr );
						model.Tags[i].Alarms[j].AlarmTests[k].HoldOffMinutes = newValueStr;
						AlarmEditor.SetAlarmEditorModel( model );
						AlarmEditor.LastValidationPassed = true;
						AlarmEditor.LastValidationId = currentItem.id;
						return;
					}
					catch ( err )
					{
						limitItem.val(model.Tags[i].Alarms[j].AlarmTests[k].HoldOffMinutes);
						FMErrorAndExceptionHandling.ShowError( "HoldOff Minutes Format Error" );
						AlarmEditor.LastValidationPassed = false;
						AlarmEditor.LastValidationId = currentItem.id;
						return;
					}
				}
			}
		}
	}
	AlarmEditor.LastValidationPassed = true;
	AlarmEditor.LastValidationId = currentItem.id;
};

AlarmEditor.ReadyForSubmit = function(tagTable)
{
	AlarmEditor.UpdateAlarmModelFromPointTags( tagTable, false );
	if (AlarmEditor.PointDetailPrevPrevFocus.startsWith(AlarmEditor.TagHysteresisPrefix) || AlarmEditor.PointDetailPrevPrevFocus.startsWith(AlarmEditor.LimitValueInputPrefix)
		|| AlarmEditor.PointDetailPrevPrevFocus.startsWith(AlarmEditor.TagHoldOffSecondsPrefix) || AlarmEditor.PointDetailPrevPrevFocus.startsWith(AlarmEditor.TagHoldOffMinutesPrefix))
	{
		if ( AlarmEditor.LastValidationId !== AlarmEditor.PointDetailPrevPrevFocus )
		{
			return false;
		}
		else
		{
			return AlarmEditor.LastValidationPassed;
		}
	}
	return true;
};

AlarmEditor.OnFocusIn = function()
{
	// Save the previously clicked value for later
	AlarmEditor.PointDetailPrevPrevFocus = AlarmEditor.PointDetailPrevFocus;
	AlarmEditor.PointDetailPrevFocus = this.id;
};

AlarmEditor.GetFormattedTagTable = function (tagTable, siteNumFormatInfo) {
	var formattedTagTable = {};
	for(var i = 0; i < tagTable.length; i++)
	{
		var tag = tagTable[i];
		formattedTagTable[tag.PointTagGuid] = {
			Unit: parseInt(tag.Unit.replace(siteNumFormatInfo.NumberGroupSeparator, '')),
			DecimalPlaces: parseInt(tag.DecimalPlaces.replace(siteNumFormatInfo.NumberGroupSeparator, '')),
			Minimum: parseFloat(tag.Minimum.replace(siteNumFormatInfo.NumberGroupSeparator, '')),
			Maximum: parseFloat(tag.Maximum.replace(siteNumFormatInfo.NumberGroupSeparator, ''))
		};
	}
	return formattedTagTable;
}

AlarmEditor.UpdateAlarmModelFromPointTags = function(tagTable, rescale)
{
	var model = AlarmEditor.GetAlarmEditorModel();
	if ( model )
	{
		
		var numFormatInfoString = $( '#alarmEditorNumberFormatInfo' ).val();
		var siteNumFormatInfo = JSON.parse(numFormatInfoString);
		var formattedTagTable = AlarmEditor.GetFormattedTagTable(tagTable, siteNumFormatInfo);
		var errStr = "";
		model.Tags.forEach( function( tag )
		{
			var fromToAPI = false;
			var densityLimits = [];

			if ( tag.Alarms )
			{
				
				var formattedTagAttributes = formattedTagTable[tag.PointTagGuid];
				var hystItem = $("#" + AlarmEditor.TagHysteresisPrefix + tag.PointTagGuid);
				var inputTagInfo = jQuery.grep(FMPointEditor.tagList, function (row) { return row.PointTagGuid === tag.PointTagGuid; });
				if ( inputTagInfo.length > 0 )
				{
					siteNumFormatInfo.NumberDecimalDigits = inputTagInfo[0].DecimalPlaces;
					var unitIndex = parseInt(tag.TagUnits);
					var newValue2 = FMFormatValues.ParseValue( unitIndex, siteNumFormatInfo, hystItem.val(), true );
					if ( rescale )
					{
						if ( unitIndex > 0 && unitIndex < 5 )
						{
							if ( ( unitIndex === 2 || unitIndex === 4 ) && formattedTagAttributes.Unit !== 2 && formattedTagAttributes.Unit !== 4 )
							{
								newValue2 = newValue2.times( math.bignumber( 5 / 9 ) );
							}
							else if ( ( formattedTagAttributes.Unit === 2 || formattedTagAttributes.Unit === 4 ) && unitIndex !== 2 && unitIndex !== 4 )
							{
								newValue2 = newValue2.div( math.bignumber( 5 / 9 ) );
							}
						}
						else
						{
							if (newValue2 != 0)	// deadband is a magnitude and zero is zero no matter what the units are
							{
								newValue2 = FMConvertEngUnits.Convert(newValue2, unitIndex, formattedTagAttributes.Unit);
								// after the conversion we may be less than zero. Since this is a deadband just set at a positive value
								if(newValue2 < 0)
								{
									newValue2 = newValue2.times(-1.0);
								}
							}
						}
					}
					if ( newValue2 === null || isNaN( newValue2 ) )
					{
						errStr += "Deadband Format Error\r\n";
					}
					if ( newValue2 < 0 )
					{
						errStr += "Deadband Cannot Be Negative\r\n";
					}
					siteNumFormatInfo.NumberDecimalDigits = formattedTagAttributes.DecimalPlaces;
					
					var newValueStr2 = FMFormatValues.FormatValueFullPrecision( formattedTagAttributes.Unit, siteNumFormatInfo, newValue2 );
					hystItem.val( newValueStr2 );
					tag.Hysteresis = newValueStr2;
					tag.TagUnits = formattedTagAttributes.Unit;
					tag.TagDecimalPlaces = formattedTagAttributes.DecimalPlaces;

					tag.Alarms.forEach(function (alarm) {
						if (alarm.AlarmTests) {
							alarm.AlarmTests.forEach(function (alarmTest) {
								// Update tests that apply to the Value Field
								if (alarmTest.TagField !== 0) {
									return;
								}
								var limitItem = $("#" + AlarmEditor.LimitValueInputPrefix + alarmTest.AlarmTestGuid);
								var limitTagInfo = jQuery.grep(FMPointEditor.tagList, function (row) { return row.PointTagGuid === alarmTest.LimitTagGuid; });
								if (FMPointEditor.isTagNumeric(inputTagInfo[0].DataType)) {

									var newValue = FMFormatValues.ParseValue(alarmTest.LimitTagUnits, siteNumFormatInfo, limitItem.text(), true);
									if (rescale) {
										newValue = FMConvertEngUnits.Convert(newValue, unitIndex, limitTagInfo[0].Units);
									}
									limitItem.text(FMFormatValues.FormatValueFullPrecision(limitTagInfo[0].Units, siteNumFormatInfo, newValue));
								}

								if (unitIndex === 191 && limitTagInfo[0].Units !== 191
								|| unitIndex !== 191 && limitTagInfo[0].Units === 191) {
									fromToAPI = true;
									densityLimits.push(limitItem.text());
								}

								alarmTest.LimitTagMin = parseFloat(limitTagInfo[0].Minimum);
								alarmTest.LimitTagMax = parseFloat(limitTagInfo[0].Maximum);
								alarmTest.LimitTagUnits = limitTagInfo[0].Units;
								alarmTest.LimitValue = limitItem.text();

							});
						}
					});

					if (fromToAPI) {
						var densityLimitIndex = tag.Alarms.length - 1;
						tag.Alarms.forEach(function (alarm) {
							alarm.AlarmTests.forEach(function (alarmTest) {
								var limitTagInfo = jQuery.grep(FMPointEditor.tagList, function (row) { return row.PointTagGuid === alarmTest.LimitTagGuid; });
								var tagItem = $("#Tags_" + limitTagInfo[0].index + "__Value");
								if (tagItem.length === 1) {
									tagItem.val(densityLimits[densityLimitIndex]);
									tagItem.attr('data-raw-value', densityLimits[densityLimitIndex]);
								}
								var limitItem = $("#" + AlarmEditor.LimitValueInputPrefix + alarmTest.AlarmTestGuid);
								limitItem.text(densityLimits[densityLimitIndex]);
								alarmTest.LimitValue = densityLimits[densityLimitIndex];
								densityLimitIndex--;
							});
						});
					}
				}
			}
		} );
		//Save the model after updating
		AlarmEditor.SetAlarmEditorModel(model);
		if ( errStr.length > 0 )
		{
			FMErrorAndExceptionHandling.ShowError( errStr );
		}
	}
};

AlarmEditor.DeleteAlarmsForTagNoConfirmation = function (currentItem) {
	var row = $(currentItem).closest('tr');
	if (row.length == 0) {
		return;
	}
	var alarmTagGuid = $(currentItem).closest('tr').attr("id").replace("TagRow_", "");

	$(currentItem).closest('tr').remove();
	$("#AlarmRow_" + alarmTagGuid).remove();


	var model = AlarmEditor.GetAlarmEditorModel();

	// find the alarm we want to delete in the model
	var alarmTagIdx = model.Tags.map(function (x) { return x.PointTagGuid; }).indexOf(alarmTagGuid);
	if (alarmTagIdx >= 0) {
		model.Tags[alarmTagIdx].AlarmsEnabled = false;
		$.each(model.Tags[alarmTagIdx].Alarms, function (alarmIdx, alarm) {
			var statusTaginUse = AlarmEditor.isStatusTagUsed(alarm.AlarmStatusTagGuid, alarm.AlarmGuid, model);
			statusTaginUse = statusTaginUse || AlarmEditor.isTagUsedInModule(alarm.AlarmStatusTagGuid);
			if (!statusTaginUse) {
				FMPointEditor.deleteTagbyGuid(alarm.AlarmStatusTagGuid);
			}

			$.each(alarm.AlarmTests, function (alarmTestIdx, alarmTest) {
				var limitTaginUse = AlarmEditor.isLimitTagUsed(alarmTest.LimitTagGuid, alarmTest.AlarmTestGuid, model);
				limitTaginUse = limitTaginUse || AlarmEditor.isTagUsedInModule(alarmTest.LimitTagGuid);
				if (!limitTaginUse) {
					FMPointEditor.deleteTagbyGuid(alarmTest.LimitTagGuid);
				}
			});
		});

		// we can enable the columns for the source tag since they are not used for an alarm, only available for templates
		if (AlarmEditor.EnableTagColumnsForAlarmTags) {
			AlarmEditor.EnableTagColumnsForAlarmTags(model.Tags[alarmTagIdx].PointTagGuid);
		}

		model.Tags.splice(alarmTagIdx, 1);
	}
	AlarmEditor.SetAlarmEditorModel(model);

}


// delete all alarms for a template tag
AlarmEditor.DeleteAlarmsForTag = function (currentItem) {

	var alarmTagGuid = $(currentItem).closest('tr').attr("id").replace("TagRow_", "");
	var alarmTagName = $("#TagName_" + alarmTagGuid).text().trim();

	// Prompt to confirm delete
	FMLayout.ConfirmYesNo('Do you want to delete all the Alarms for the tag "' + alarmTagName + '"?',
		"Delete Alarms for Tag",
		function () {
			AlarmEditor.DeleteAlarmsForTagNoConfirmation(currentItem);
		});
}

// Check if the limit tag is being used by another alarm test
AlarmEditor.isLimitTagUsed = function (limitTagGuid, alarmTestGuid, model) {
	// check to see if the tag is being used in the alarm model
	var limitTaginUse = false;
	$.each(model.Tags, function (tagIndex, tag) {
		$.each(tag.Alarms, function (alarmIndex, alarm) {
			$.each(alarm.AlarmTests, function (alarmTestIndex, alarmTest) {
				if (alarmTest.LimitTagGuid === limitTagGuid && alarmTest.AlarmTestGuid !== alarmTestGuid) {
					limitTaginUse = true;
				}

			});
		});
	});
	return limitTaginUse;
}

AlarmEditor.UpdateAlarmTestHoldoff = function (alarmTestGuid, holdoffRawValue, holdoffValue) {
	var model = AlarmEditor.GetAlarmEditorModel();
	$.each(model.Tags, function (tagIndex, tag) {
		$.each(tag.Alarms, function (alarmIndex, alarm) {
			$.each(alarm.AlarmTests, function (alarmTestIndex, alarmTest) {
				if (alarmTest.AlarmTestGuid === alarmTestGuid) {
					$('#HysteresisInput_' + tag.PointTagGuid).val(holdoffValue);
					$('#HysteresisInput_' + tag.PointTagGuid).attr("data-value", holdoffValue);
					alarmTest.HoldOff = holdoffRawValue;
					AlarmEditor.SetAlarmEditorModel(model);
				}
			});
		});
	});

}

AlarmEditor.GetAlarmTestLimitValue = function (tagGuid) {
	var limitValue = 0.0;
	var model = AlarmEditor.GetAlarmEditorModel();
	$.each(model.Tags, function (tagIndex, tag) {
		$.each(tag.Alarms, function (alarmIndex, alarm) {
			$.each(alarm.AlarmTests, function (alarmTestIndex, alarmTest) {
				if (alarmTest.LimitTagGuid === tagGuid) {
					limitValue = alarmTest.LimitValue;
				}
			});
		});
	});

	return limitValue;
}



AlarmEditor.GetAlarmTestHoldoff = function (tagGuid) {
	var holdoff = 0.0;
	var model = AlarmEditor.GetAlarmEditorModel();
	$.each(model.Tags, function (tagIndex, tag) {
		$.each(tag.Alarms, function (alarmIndex, alarm) {
			$.each(alarm.AlarmTests, function (alarmTestIndex, alarmTest) {
				if (alarmTest.LimitTagGuid === tagGuid) {
					holdoff = alarmTest.HoldOff;
				}
			});
		});
	});

	return holdoff;
}

// Return the AlarmTestGuid if tag is a limit tag
AlarmEditor.GetAlarmTestGuid = function (tagGuid) {
	// check to see if the tag is being used in the alarm model
	var alarmTestGuid = null;
	var model = AlarmEditor.GetAlarmEditorModel();
	$.each(model.Tags, function (tagIndex, tag) {
		$.each(tag.Alarms, function (alarmIndex, alarm) {
			$.each(alarm.AlarmTests, function (alarmTestIndex, alarmTest) {
				if (alarmTest.LimitTagGuid === tagGuid) {
					alarmTestGuid = alarmTest.AlarmTestGuid;
				}
			});
		});
	});

	return alarmTestGuid;
}



// Check if the status tag is being used by another alarm
AlarmEditor.isStatusTagUsed = function (statusTagGuid, alarmGuid, model) {
	// check to see if the tag is being used in the alarm model
	var statusTaginUse = false;
	$.each(model.Tags, function (tagIndex, tag) {
		$.each(tag.Alarms, function (alarmIndex, alarm) {
			if (alarm.AlarmStatusTagGuid === statusTagGuid && alarm.AlarmGuid !== alarmGuid) {
				statusTaginUse = true;
			}
			$.each(alarm.AlarmTests, function (alarmTestIndex, alarmTest) {
				if (alarmTest.LimitTagGuid === statusTagGuid) {
					statusTaginUse = true;
				}

			});
		});
	});
	return statusTaginUse;
}

AlarmEditor.UpdateAlarmTestValue = function (alarmTestGuid, alarmLimitValue) {
	$('#limitValueInputId_' + alarmTestGuid).text(alarmLimitValue);
	$('#limitValueInputId_' + alarmTestGuid).attr("data-value", alarmLimitValue);

	var model = AlarmEditor.GetAlarmEditorModel();
	$.each(model.Tags, function (tagIndex, tag) {
		$.each(tag.Alarms, function (alarmIndex, alarm) {
			$.each(alarm.AlarmTests, function (alarmTestIndex, alarmTest) {
				if (alarmTest.AlarmTestGuid === alarmTestGuid) {
					alarmTest.LimitValue = alarmLimitValue;
				}
			});
		});
	});

	AlarmEditor.SetAlarmEditorModel(model);
}


// Check if the tag is being used by a module
AlarmEditor.isTagUsedInModule = function (tagGuid) {
	var taginUse = false;

	// check to see if used in a module (look in the tag tab)
	var tagInfo = jQuery.grep(FMPointEditor.tagList, function (row) { return row.PointTagGuid === tagGuid; });
	if (tagInfo.length > 0) {
		taginUse = ($("#Tags_" + tagInfo[0].index + "__Name").closest('tr').find('[name=module]').attr('data-value') == 'true');
	}
	return taginUse;
}

// Create alarm/alarm test definition programatically
AlarmEditor.AddDeviceAlarmEntry = function (tagGuid, deviceAlarmMap, deviceAlarmMapEntry, normalUnacknowledgeAlarmPriorityID, alarmCategoryID, order) {

	var model = AlarmEditor.GetAlarmEditorModel();

	var tagInfo = jQuery.grep(FMPointEditor.tagList, function (row) {
		return row.PointTagGuid === tagGuid;
	});

	if (tagInfo.length == 0) {
		return;
	}

	var tagName = tagInfo[0].Name;
	var newAlarmTag = $("#blankAlarmTag tbody").children().clone();

	// update the guids
	newAlarmTag.each(function (index) {
		if ($(this).attr("id") && $(this).attr("id").indexOf("{}") >= 0) {
			$(this).attr("id", $(this).attr("id").replace("{}", tagGuid));
		}
	});

	newAlarmTag.find('[id]').each(function (index) {
		if ($(this).attr("id") && $(this).attr("id").indexOf("{}") >= 0) {
				$(this).attr("id", $(this).attr("id").replace("{}", tagGuid));
		}
	});

	// set the defaults
	newAlarmTag.find('.tagName').text(tagName).attr("data-value", tagGuid);

	var newFormattedHisteresys = "0";
	var decimalPlaces = 0;
	var units = 255;
	var numFormatInfoString = $('#NumberFormatInfoString').val();
	var numformatInfo = JSON.parse(numFormatInfoString);
	decimalPlaces = tagInfo[0].DecimalPlaces;
	units = parseInt(tagInfo[0].Units);
	numformatInfo.NumberDecimalDigits = decimalPlaces;
	
	newFormattedHisteresys = FMFormatValues.FormatValueFullPrecision(tagInfo[0].Units, numformatInfo, 0);
	if (!FMPointEditor.isTagNumeric(tagInfo[0].DataType)) {
		newFormattedHisteresys = "";
		newAlarmTag.find('.tagHysteresis input').prop("disabled", true);
	}

	newAlarmTag.find('.tagHysteresis input').val(newFormattedHisteresys);

	var selectedAlarmTagModel = jQuery.grep(model.Tags, function (a) {
		return a.PointTagGuid === tagGuid;
	});

	if (selectedAlarmTagModel.length == 0) {
		newAlarmTag.appendTo("#alarmTagsTable>tbody");
		model.Tags.push({ AlarmsEnabled: false, PointTagGuid: tagGuid, Id: tagName, Hysteresis: newFormattedHisteresys, TagDecimalPlaces: decimalPlaces, TagUnits: units });
	}

	// we need to disable the tag properties
	if (AlarmEditor.DisableTagColumnsForAlarmTags) {
		AlarmEditor.DisableTagColumnsForAlarmTags(tagGuid);
	}


	// add the alarm definition
	var alarmGuid = FMPointEditor.newGuid();
	var newAlarmDefinition = $("#blankAlarmDefinition tbody").children().clone();

	// update the guids
	newAlarmDefinition.each(function (index) {
		if ($(this).attr("id") && $(this).attr("id").indexOf("{}") >= 0) {
			$(this).attr("id", $(this).attr("id").replace("{}", alarmGuid));
		}
	});
	newAlarmDefinition.find('[id]').each(function (index) {
		if ($(this).attr("id") && $(this).attr("id").indexOf("{}") >= 0) {
			$(this).attr("id", $(this).attr("id").replace("{}", alarmGuid));
		}
	});

	var alarmID = tagName + ' ' + deviceAlarmMapEntry.TestName;
	var alarmTagName = alarmID + ' Alarm';
	var alarmNotInAlarmText = deviceAlarmMap.NotAlarmText;
	var alarmCategory = alarmCategoryID;
	var alarmOrder = order;

	var reuseStatusTagGuid = '';

	// check if the alarm tag name already exists
	var validAlarmTagResult = AlarmEditor.isAlarmStateTagNameValid(alarmTagName);
	if (validAlarmTagResult.msg !== "") {
		return;
	} else {
		reuseStatusTagGuid = validAlarmTagResult.tagGuid;
	}

	var alarmStatusTagGuid = reuseStatusTagGuid != "" ? validAlarmTagResult.tagGuid : FMPointEditor.newGuid();

	newAlarmDefinition.find('#AlarmsTableAlarmIdCellId_' + alarmGuid).text(alarmID);
	newAlarmDefinition.find('#alarmsTableAlarmStateTagCellId_' + alarmGuid).text(alarmTagName);
	newAlarmDefinition.find('#alarmsTableAlarmCategoryCellId_' + alarmGuid).text(alarmCategory);
	newAlarmDefinition.find('#alarmsTableAlarmOrderCellId_' + alarmGuid).text(alarmOrder);

	var alarmModel = {
		AlarmGuid: alarmGuid,
		AlarmStatusTagGuid: alarmStatusTagGuid,
		AlarmStatusTagId: alarmTagName,
		NotAlarmState: alarmNotInAlarmText,
		Category: alarmCategory,
		Enabled: false,
		Exclusive: true,
		Id: alarmID,
		Order: alarmOrder,
		SuppressedAndShelvedStatus: "Active"
	};

	var selectedAlarmTagModel = jQuery.grep(model.Tags, function (a) {
		return a.PointTagGuid === tagGuid;
	});
	if (selectedAlarmTagModel.length > 0) {
		if (!selectedAlarmTagModel[0].Alarms)
			selectedAlarmTagModel[0].Alarms = [];
		selectedAlarmTagModel[0].Alarms.push(alarmModel);
	}

	newAlarmDefinition.appendTo("#alarmsTable_" + tagGuid + " > tbody");

	// add the new alarm Definition tag
	var dataType = 'System.String';
	var inputOutputType = '2'; // Calculated
	var unitsType = 'FmuNone'; // Dimensionless
	var units = '255'; // No Dimensions
	var decimalPlaces = 2;
	var maximum = 0;
	var minimum = 0;
	var applyPointDecimalPlaces = false;
	var applyPointUnits = false;
	var applyPointMaximum = false;
	var applyPointMinimum = false;

	// if we are not reusing an existing tag we need to create one
	if (reuseStatusTagGuid == "") {
		FMPointEditor.addTag(alarmStatusTagGuid, alarmTagName, dataType, inputOutputType, unitsType, units, decimalPlaces, '', maximum, minimum, applyPointDecimalPlaces, applyPointUnits, applyPointMaximum, applyPointMinimum);
	}

	// we need to disable the tag properties, only applicable for point template
	if (AlarmEditor.DisableTagColumnsForAlarmTags) {
		AlarmEditor.DisableTagColumnsForAlarmTags(alarmStatusTagGuid);
	}

	FMPointEditor.valuesChanged = true;

	AlarmEditor.initAlarmTestSorting();


	// add the alarm test definition
	var newAlarmTestDefinition = $("#blankAlarmTestDefinition tbody").children().clone();
	var alarmTestGuid = FMPointEditor.newGuid();
	// update the guids
	newAlarmTestDefinition.each(function (index) {
		if ($(this).attr("id") && $(this).attr("id").indexOf("{}") >= 0) {
			$(this).attr("id", $(this).attr("id").replace("{}", alarmTestGuid));
		}
	});
	newAlarmTestDefinition.find('[id]').each(function (index) {
		if ($(this).attr("id") && $(this).attr("id").indexOf("{}") >= 0) {
			$(this).attr("id", $(this).attr("id").replace("{}", alarmTestGuid));
		}
	});

	var alarmTestID = deviceAlarmMapEntry.TestName + ' Test';
	var alarmLimitTagName = tagName + ' ' + deviceAlarmMapEntry.TestName + ' Limit';
	var alarmLimitValue = deviceAlarmMapEntry.BitMask.toString();
	var alarmState = deviceAlarmMapEntry.TestName + ' Alarm';
	var alarmText = tagName + ' ' + deviceAlarmMapEntry.TestName + ' Alarm';
	var alarmHoldoffMin = 0;
	var alarmHoldoffSec = 0;
	var alarmTestNormalUnAcknowledgePriority = deviceAlarmMap.NormalUnacknowledgedAlarmPriority;
	var reuseLimitTagGuid = '';

	// check if the limit tag name already exists
	var validAlarmLimitTagResult = AlarmEditor.isAlarmLimitTagNameValid(alarmLimitTagName, tagGuid);
	if (validAlarmLimitTagResult.msg !== "") {
		return;
	} else {
		ruseLimitTagGuid = validAlarmLimitTagResult.tagGuid;
	}


	var alarmLimitTagGuid = reuseLimitTagGuid != "" ? reuseLimitTagGuid : FMPointEditor.newGuid();

	var alarmTestEquation = tagName + AlarmEditor.GetTagField(0) + ' ' + AlarmEditor.GetBitmask('0', true, deviceAlarmMapEntry.BitMask.toString(16).toUpperCase(), '0') + AlarmEditor.GetComparison('4') + ' ' + alarmLimitTagName;

	newAlarmTestDefinition.find('#testNameCellId_' + alarmTestGuid).text(alarmTestID);
	newAlarmTestDefinition.find('#testLimitTagCellId_' + alarmTestGuid).text(alarmLimitTagName);
	newAlarmTestDefinition.find('#limitValueInputId_' + alarmTestGuid).text(alarmLimitValue);
	newAlarmTestDefinition.find('#limitValueInputId_' + alarmTestGuid).attr("data-value", alarmLimitValue);
	if (newAlarmTestDefinition.find('#alarmStateInputId_' + alarmTestGuid).length === 1) {
		newAlarmTestDefinition.find('#alarmStateInputId_' + alarmTestGuid).val(alarmState);
	}
	else {
		newAlarmTestDefinition.find('#testAlarmStateCellId_' + alarmTestGuid).text(alarmState);
	}
	if (newAlarmTestDefinition.find('#alarmTextInputId_' + alarmTestGuid).length === 1) {
		newAlarmTestDefinition.find('#alarmTextInputId_' + alarmTestGuid).val(alarmText);
	}
	else {
		newAlarmTestDefinition.find('#testAlarmTextCellId_' + alarmTestGuid).text(alarmText);
	}
	if (newAlarmTestDefinition.find('#TimedHoldOffMinutesInput_' + alarmTestGuid).length === 1) {
		newAlarmTestDefinition.find('#TimedHoldOffMinutesInput_' + alarmTestGuid).val(alarmHoldoffMin);
		newAlarmTestDefinition.find('#TimedHoldOffSecondsInput_' + alarmTestGuid).val(alarmHoldoffSec);
	}
	else {
		newAlarmTestDefinition.find('#TimedHoldOffMinutesInput_' + alarmTestGuid).text(alarmHoldoffMin);
		newAlarmTestDefinition.find('#TimedHoldOffSecondsInput_' + alarmTestGuid).text(alarmHoldoffSec);
	}
	newAlarmTestDefinition.find('#testTestCellDivId_' + alarmTestGuid).text(alarmTestEquation);
	newAlarmTestDefinition.find('#testTestCellDivId_' + alarmTestGuid).attr('title', alarmTestEquation);

	newAlarmTestDefinition.appendTo("#AlarmTestTable_" + alarmGuid + " > tbody");

	// find the tag we will be working with
	var findTags = $.map(FMPointEditor.tagList, function (obj, index) {
		if (obj.PointTagGuid == tagGuid) {
			return index;
		}
	});

	var tagGridIndex = 0;
	if (findTags.length > 0) {
		tagGridIndex = findTags[0];
	}
	else {
		return;
	}

	// Update Alarm Editor Model
	var alarmTestModel = {
		AlarmState: alarmState,
		AlarmTestEquation: alarmTestEquation,
		AlarmTestGuid: alarmTestGuid,
		AlarmText: alarmText,
		DrawingGuid: "00000000-0000-0000-0000-000000000000",
		DrawingId: null,
		Enabled: false,
		HelpFile: null,
		HoldOff: 0,
		HoldOffMinutes: alarmHoldoffMin,
		HoldOffSeconds: alarmHoldoffSec,
		Id: alarmTestID,
		LimitTagDecimalPlaces: 0,
		LimitTagEditable: true,
		LimitTagGuid: alarmLimitTagGuid,
		LimitTagId: alarmLimitTagName,
		LimitTagMax: 4294967295,
		LimitTagMin: 0,
		LimitTagUnitsType: "FmuNone",
		LimitTagUnits: 255,
		LimitTagValueType: "System.UInt32",
		LimitValue: alarmLimitValue,
		Order: order,
		AlarmPriorityGuid: deviceAlarmMapEntry.AlarmPriority,
		NormalUnacknowledgedAlarmPriorityGuid: deviceAlarmMap.NormalUnacknowledgedPriority,
		BitMask: deviceAlarmMapEntry.BitMask,
		BitwiseOperator: 0,
		TagField: 0,
		TestType: 4
	};

	var selectedAlarmTagModel = jQuery.grep(model.Tags, function (a) {
		return a.PointTagGuid === tagGuid;
	});

	if (selectedAlarmTagModel.length > 0) {
		if (selectedAlarmTagModel[0].Alarms) {
			var selectedAlarmModel = jQuery.grep(selectedAlarmTagModel[0].Alarms, function (a) {
				return a.AlarmGuid === alarmGuid;
			});
			if (selectedAlarmModel.length > 0) {
				if (!selectedAlarmModel[0].AlarmTests)
					selectedAlarmModel[0].AlarmTests = [];

				// update all the existing alarm tests with the new Normal Unacknowldge Priority
				$.each(selectedAlarmModel[0].AlarmTests, function (index, obj) {
					obj.NormalUnacknowledgedAlarmPriorityGuid = alarmTestNormalUnAcknowledgePriority
				});
				alarmTestModel.Order = selectedAlarmModel[0].AlarmTests.length;
				selectedAlarmModel[0].AlarmTests.push(alarmTestModel);
			}
		}
	}

	// find the index in the actual grid (maybe different that in the internal array)
	var gridIndex = $("#TagEditTable .tagColumnPointTagGuid:contains('" + FMPointEditor.tagList[tagGridIndex].PointTagGuid + "')").closest('tr').index();

	// add the new alarm test tag if we are not reusing an existing one
	if (reuseLimitTagGuid == "") {

		var dataType = "System.UInt32";
		var maximum = 4294967295;
		var minimum = 0;
		var inputOutputType = '1'; // Manual
		var unitsType = "FmuNone";
		var units = 255;
		var decimalPlaces = 0;
		var applyPointDecimalPlaces = false;
		var applyPointUnits = false;
		var applyPointMaximum = false;
		var applyPointMinimum = false;

		FMPointEditor.addTag(alarmLimitTagGuid, alarmLimitTagName, dataType, inputOutputType, unitsType, units, decimalPlaces, alarmLimitValue, maximum, minimum, applyPointDecimalPlaces, applyPointUnits, applyPointMaximum, applyPointMinimum);
	} else {
		// update the existing tag with the proper datatype
		var tagInfo = jQuery.grep(FMPointEditor.tagList, function (row) { return row.PointTagGuid === alarmLimitTagGuid; });
		if (tagInfo.length > 0) {
			var rowIndex = tagInfo[0].index;
			tagInfo[0].DataType = "System.UInt32";
			tagInfo[0].EngineeringUnitsType = "FmuNone";
			tagInfo[0].Units = 255;
			tagInfo[0].DecimalPlaces = 0;
			tagInfo[0].Maximum = 4294967295;
			tagInfo[0].Minimum = 0;

			$("#Tags_" + rowIndex + "__DataType").val(tagInfo[0].DataType);
			$("#Tags_" + rowIndex + "__EngineeringUnitsType").val(tagInfo[0].EngineeringUnitsType);
			$("#Tags_" + rowIndex + "__Unit").val(tagInfo[0].Units);
			$("#Tags_" + rowIndex + "__DecimalPlaces").val(tagInfo[0].DecimalPlaces);
			$("#Tags_" + rowIndex + "__Minimum").attr("data-raw-value", 0);
			$("#Tags_" + rowIndex + "__Maximum").attr("data-raw-value", 65535);
			var row = $("#Tags_" + rowIndex + "__DataType").closest('tr');
			FMPointEditor.setTagMinimum(row);
			FMPointEditor.setTagMaximum(row);
			FMPointEditor.resetTagInputMask('#Tags_' + rowIndex + '__Minimum');
			FMPointEditor.resetTagInputMask('#Tags_' + rowIndex + '__Maximum');
		}
	}

	// we need to disable the tag properties, only applicable to point template
	if (AlarmEditor.DisableTagColumnsForAlarmTags) {
		AlarmEditor.DisableTagColumnsForAlarmTags(alarmLimitTagGuid);
	}

	AlarmEditor.SetAlarmEditorModel(model);
}

// Validate Alarm State Tag Name
AlarmEditor.isAlarmStateTagNameValid = function( alarmTagName ) {
	var errorMessage = "";
	var newTagGuid = "";
	var findDuplicateTagNames = $.map(FMPointEditor.tagList, function (obj, index){
		if (obj.Name.toUpperCase() == alarmTagName.toUpperCase()) {
			return index;
		}
	} );

	if (findDuplicateTagNames.length > 0){
		// if we found a tag with the same name check if the tag is of type string and not in use by another alarm
		if ( $("#Tags_" + findDuplicateTagNames[0] + "__DataType").val() !== "System.String") {
			errorMessage += "Alarm Status Tag Name already in use for a different data type, please use a different name.\n";
		} else {
			newTagGuid = $("#Tags_" + findDuplicateTagNames[0] + "__DataType").closest('tr').find('.tagColumnPointTagGuid').text().trim();
			if ( AlarmEditor.isStatusTagUsed( newTagGuid, "", AlarmEditor.GetAlarmEditorModel() )) {
				errorMessage += "Alarm Status Tag Name already in use.\n"
			}
		}
	}
	return { msg:errorMessage, tagGuid: newTagGuid};
}

// Validate Alarm Limit Tag Name
AlarmEditor.isAlarmLimitTagNameValid = function( alarmTagName, sourceTagGuid ) {
	var errorMessage = "";
	var newTagGuid = "";
	var useBitmask = false;
	var newTagDataType = '';
	var findDuplicateTagNames = $.map(FMPointEditor.tagList, function (obj, index){
		if (obj.Name.toUpperCase() == alarmTagName.toUpperCase()) {
			return index;
		}
	} );
			
	// find the source tag since we need to get the datatype
	var sourceTagInfo = jQuery.grep(FMPointEditor.tagList, function (row) { return row.PointTagGuid === sourceTagGuid; });
	if (sourceTagInfo.length > 0) {
		var sourceTagDataType = sourceTagInfo[0].DataType;
		if (findDuplicateTagNames.length > 0){
			var duplicateDataType = $("#Tags_" + findDuplicateTagNames[0] + "__DataType").val();
			newTagDataType = duplicateDataType;
			// if we found a tag with the same name check if the tag is of type string and not in use by another alarm
			if ( duplicateDataType !== sourceTagDataType && duplicateDataType != "System.UInt32") {
				errorMessage += "Alarm Limit Tag Name already in use for a different data type, please use a different name.\n";
			} else {
				var canUseBitmask = FMPointEditor.isTagNumeric( sourceTagDataType) && sourceTagDataType !== "System.Double" && sourceTagDataType !== "System.Single";
					
				// if the data type of the existing tag found is Uint32 then we will be using a bitmask 
				if ( canUseBitmask && duplicateDataType == "System.UInt32" && sourceTagDataType != "System.UInt32") {
					newTagGuid = $("#Tags_" + findDuplicateTagNames[0] + "__DataType").closest('tr').find('.tagColumnPointTagGuid').text().trim();
					useBitmask = true;
				} else if (!canUseBitmask && duplicateDataType == "System.UInt32" && sourceTagDataType != "System.UInt32" ) {
					errorMessage += "Existing Alarm Limit Tag Name has an invalid data type, please select a different Limit Tag.\n"
				} else {
					newTagGuid = $("#Tags_" + findDuplicateTagNames[0] + "__DataType").closest('tr').find('.tagColumnPointTagGuid').text().trim();
					if ( AlarmEditor.isLimitTagUsed( newTagGuid, "", AlarmEditor.GetAlarmEditorModel() )) {
						errorMessage += "Alarm Limit Tag Name already in use.\n"
					}
				}
			}
		}
	} else {
		errorMessage += "Source Tag not found.\n"
	}
	return { msg:errorMessage, tagGuid: newTagGuid, useBitmask: useBitmask, tagDataType: newTagDataType};
}

// Initialize sorting for the Alarm tests
AlarmEditor.initAlarmTestSorting = function () {

	$(".alarmTestsTable tbody").sortable({
		axis: 'y',
		handle: ".alarmtestsortcolumn",
		update: function (event, ui) {

			var alarmTestGuid = ui.item.attr("id").replace('testRow_', '');
			var alarmDefinitionGuid = ui.item.closest('table').attr('id').replace('AlarmTestTable_', '');
			var alarmTagGuid = ui.item.closest('table').parent().closest('table').parent().attr('id').replace('AlarmRowTd_', '');

			var model = AlarmEditor.GetAlarmEditorModel();

			selectedAlarmTag = jQuery.grep(model.Tags, function (a) {
				return a.PointTagGuid === alarmTagGuid;
			});
			if (selectedAlarmTag.length > 0) {
				// get the alarm test configuration
				selectedAlarmDefinition = jQuery.grep(selectedAlarmTag[0].Alarms, function (a) {
					return a.AlarmGuid === alarmDefinitionGuid;
				});

				if (selectedAlarmDefinition.length > 0) {
					$.each(selectedAlarmDefinition[0].AlarmTests, function (index, value) {
						value.Order = $("#testRow_" + value.AlarmTestGuid).index();
					});
				}
			}
			AlarmEditor.SetAlarmEditorModel(model);
		}
	});
}


AlarmEditor.PerformRescaling = function(tagTable, rescale) {
	AlarmEditor.UpdateAlarmModelFromPointTags(tagTable, rescale);
}

AlarmEditor.SortAlarmTable = function (tagGuid) {
	// Sort the Alarm Tags table
	var $tbody = $("#alarmTagsTable > tbody");

	//Find the tag rows and its next one, clone and wrap them into temp table.
	var $rows = $tbody.find('.tagrowTr').map(function () {
		return $(this).next().andSelf().clone().wrapAll('<table />')
	});

	//Give each table which contains the pair to be sorted
	$rows.sort(function (a, b) {
		var tda = $(a).find('.tagName').text().toUpperCase(); // Use your wished column index
		var tdb = $(b).find('.tagName').text().toUpperCase(); // Use your wished column index

		// if a > b return 1
		return tda > tdb ? 1
					// else if a < b return -1
					: tda < tdb ? -1
					// else they are equal - return 0
					: 0;
	});

	var $tbody = $tbody.empty();//empty the tbody
	$.each($rows, function (index, row) {
		$tbody.append($(row).unwrap());//Unwrap the table and get the rows alone.
	});


	// Sort the Alarms Table
	$tbody = $("#alarmsTable_" + tagGuid + " > tbody");

	//Find the alarm rows and its next one, clone and wrap them into temp table.
	$rows = $tbody.find('.alarmrowTr').map(function () {
		return $(this).next().andSelf().clone().wrapAll('<table />')
	});

	//Give each table which contains the pair to be sorted
	$rows.sort(function (a, b) {
		var tda = $(a).find('.alarmName').text().toUpperCase(); // Use your wished column index
		var tdb = $(b).find('.alarmName').text().toUpperCase(); // Use your wished column index

		// if a > b return 1
		return tda > tdb ? 1
					// else if a < b return -1
					: tda < tdb ? -1
					// else they are equal - return 0
					: 0;
	});

	var $tbody = $tbody.empty();//empty the tbody
	$.each($rows, function (index, row) {
		$tbody.append($(row).unwrap());//Unwrap the table and get the rows alone.
	});
}



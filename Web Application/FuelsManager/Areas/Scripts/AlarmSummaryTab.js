// There is a bug in IE11 with ScrollIntoView which causes the screen to shift to the left
// Using a pollyfill implementation of scrollIntoViewIfNeeded which resolves this issue

if (!Element.prototype.scrollIntoViewIfNeeded) {
	Element.prototype.scrollIntoViewIfNeeded = function (centerIfNeeded) {
		centerIfNeeded = arguments.length === 0 ? true : !!centerIfNeeded;

		var parent = this.parentNode,
			 parentComputedStyle = window.getComputedStyle(parent, null),
			 parentBorderTopWidth = parseInt(parentComputedStyle.getPropertyValue('border-top-width')),
			 parentBorderLeftWidth = parseInt(parentComputedStyle.getPropertyValue('border-left-width')),
			 overTop = this.offsetTop - parent.offsetTop < parent.scrollTop,
			 overBottom = (this.offsetTop - parent.offsetTop + this.clientHeight - parentBorderTopWidth) > (parent.scrollTop + parent.clientHeight),
			 overLeft = this.offsetLeft - parent.offsetLeft < parent.scrollLeft,
			 overRight = (this.offsetLeft - parent.offsetLeft + this.clientWidth - parentBorderLeftWidth) > (parent.scrollLeft + parent.clientWidth),
			 alignWithTop = overTop && !overBottom;

		if ((overTop || overBottom) && centerIfNeeded) {
			parent.scrollTop = this.offsetTop - parent.offsetTop - parent.clientHeight / 2 - parentBorderTopWidth + this.clientHeight / 2;
		}

		if ((overLeft || overRight) && centerIfNeeded) {
			parent.scrollLeft = this.offsetLeft - parent.offsetLeft - parent.clientWidth / 2 - parentBorderLeftWidth + this.clientWidth / 2;
		}

		if ((overTop || overBottom || overLeft || overRight) && !centerIfNeeded) {
			this.scrollIntoView(alignWithTop);
		}
	};
}

var AlarmSummaryTab = AlarmSummaryTab ||
{
		silence: false,
		IsSteadyColor: true,
		RefreshTimer: null,
		FindArr: null,
		CurrentFind: null,
		CurrentFindString: '',
		InAjaxRefreshCall: false,
		DatatableHandle: null,
		RefreshActiveCheckTimer: null,
		SummaryInitialized: false,
	  TabidNumber: null,
		Statistics: new Array(),
		// notification stack for the screen 
		stack_bottomright_alarmsummary: { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#AlarmSummaryTableSection') },
		messageAttributes: {},
		DOMElements: null,
		needToClearFindBorder: false,
		needToScrollUpdate: true,
		visibleAlarms: null,
model:null
};

if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

AlarmSummaryTab.GetAlarmModelString = function () {
	if (AlarmSummaryTab.model === null) {
		return $('#AlarmSummaryTabModel').val();
	}
	return JSON.stringify(AlarmSummaryTab.model);
}

AlarmSummaryTab.GetAlarmModel = function ()
{
	if ( AlarmSummaryTab.model === null )
	{
		AlarmSummaryTab.model = JSON.parse(AlarmSummaryTab.GetAlarmModelString());
	}

}

AlarmSummaryTab.SetAlarmModel = function (model)
{
	AlarmSummaryTab.model = model;
}


AlarmSummaryTab.AcknowledgeWorker = function (alarmIdList)
{
	var url = $('#AckUrl').val();
	// this function can be called from outside the summary tab so we need to make sure we have a valid url
	if ( !url )
	{
		url = $('#urlAlarmAcknowledge').val();
	}
	
	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// remove previous notifications
	PNotify.removeStack(AlarmSummaryTab.messageAttributes.stack);

	$.ajax({
		type: 'POST',
		url: url,
		cache: false,
		headers: headers,
		dataType: 'json',
		contentType: 'application/json; charset=utf-8',
		data: JSON.stringify({
			alarmGuidList: alarmIdList
		}),
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) { }, AlarmSummaryTab.messageAttributes);
		},
		error: function (xhr, textStatus, error) {
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () { },
				AlarmSummaryTab.messageAttributes);
		}
	});
	//Prevent Post
	return false;
}


AlarmSummaryTab.SilenceWorker = function (alarmIdList) {
	var url = $('#SilenceUrl').val();
	// this function can be called from outside the summary tab so we need to make sure we have a valid url
	if (!url) {
		url = $('#urlAlarmSilence').val();
	}

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	$.ajax({
		type: 'POST',
		url: url,
		cache: false,
		headers: headers,
		dataType: 'json',
		contentType: 'application/json; charset=utf-8',
		data: JSON.stringify({
			alarmGuidList: alarmIdList
		}),
		success: function (response) {
			// remove previous notifications
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) { }, AlarmSummaryTab.messageAttributes);
		},
		error: function (xhr, textStatus, error) {
			// remove previous notifications
			PNotify.removeStack(AlarmSummaryTab.messageAttributes.stack);
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () { },
				AlarmSummaryTab.messageAttributes);
		}
	});
	//Prevent Post
	return false;
}


AlarmSummaryTab.Acknowledge = function ()
{
	var selectedRows = AlarmSummaryTab.GetCurrentlySelectedViewableAlarms();
	var ackDict = AlarmSummaryTab.GetAcknowledgedDictionaryByAlarmGuid(); 
	var guidList = AlarmSummaryTab.UnAckAlarms(ackDict, selectedRows);
	return AlarmSummaryTab.AcknowledgeWorker(guidList);
}

AlarmSummaryTab.GetDataTableRow = function (table, id) {
	return table.row(id);
}

AlarmSummaryTab.DoFindHighlight = function(id, text )
{
	if ( AlarmSummaryTab.CurrentFindString && AlarmSummaryTab.CurrentFindString.length > 0 )
	{
		var pattern = new RegExp(AlarmSummaryTab.CurrentFindString, 'gi');
		var retText = text.replace(pattern, function (x)
		{
			return '<span class="alarmSummaryFindColoring">' + x + '</span>'; 
		});
		if ( retText !== text )
		{
			AlarmSummaryTab.FindArr.push( id );
		}
		return retText;
	}
	return text;
}

AlarmSummaryTab.AddTableRow = function (table, alarm, backgroundColor, textColor) {
	if (alarm == null)
		return;

	var html = '';
	var timestampIndexId = "TimestampIndex_" + alarm.AlarmGuid;
	var timestampId = 'Timestamp_' + alarm.AlarmGuid;
	var pointIdid = 'PointID_' + alarm.AlarmGuid;
	var descriptionId = 'DescriptionID_' + alarm.AlarmGuid;
	var tagIdid = 'TagID_' + alarm.AlarmGuid;
	var statusId = 'Status_' + alarm.AlarmGuid;
	var rowId = 'Row_' + alarm.AlarmGuid;
	var row = document.createElement('TR');
	row.setAttribute('id', rowId);
//	row.setAttribute('style', "border-collapse: collapse; border: none;")


	html += '<td class="col-sm-2 col-md-2 text-center" id=' + timestampIndexId + '>' + alarm.TimeStampUTCTicks + '</td>';
	html += '<td class="col-sm-2 col-md-2 text-center" id=' + timestampId + '>' + AlarmSummaryTab.DoFindHighlight(timestampId, alarm.FormattedTimestamp) + '</td>';
	html += '<td class="col-sm-2 col-md-2 text-center" id=' + pointIdid + '>' + AlarmSummaryTab.DoFindHighlight(pointIdid, alarm.PointID) + '</td>';
	html += '<td class="col-sm-2 col-md-2 text-center" id=' + descriptionId + '>' + AlarmSummaryTab.DoFindHighlight(descriptionId, alarm.Description) + '</td>';
	html += '<td class="col-sm-2 col-md-2 text-center" id=' + tagIdid + '>' + AlarmSummaryTab.DoFindHighlight(tagIdid, alarm.TagID) + '</td>';
	html += '<td class="col-sm-2 col-md-2 text-center" style="background: #' + backgroundColor + '; color: #' + textColor + ' "id=' + statusId + '>' + AlarmSummaryTab.DoFindHighlight(statusId, alarm.Status) + '</td>';

	row.innerHTML = html;
	table.row.add(row);
}

AlarmSummaryTab.SetRowElementValue = function( id, valueText )
{
	var e = document.getElementById(id);
	if ( e )
	{
		var replacementHtml = AlarmSummaryTab.DoFindHighlight(id, valueText);
		if ( e.innerHTML !== replacementHtml )
		{
			e.innerHTML = replacementHtml;
			return true;
		}
	}

	return false;
}

AlarmSummaryTab.UpdateRow = function (alarm, backgroundColor, textColor) {
	if (alarm == null) {
		return false;
	}

	var timestampIndexId = "TimestampIndex_" + alarm.AlarmGuid;
	var timestampId = 'Timestamp_' + alarm.AlarmGuid;
	var pointIdid = 'PointID_' + alarm.AlarmGuid;
	var descriptionId = 'DescriptionID_' + alarm.AlarmGuid;
	var tagIdid = 'TagID_' + alarm.AlarmGuid;
	var statusId = 'Status_' + alarm.AlarmGuid;

	AlarmSummaryTab.SetRowElementValue(timestampIndexId, alarm.TimeStampUTCTicks);

	if (AlarmSummaryTab.SetRowElementValue(timestampId, alarm.FormattedTimestamp)) {
		return true;
	}

	AlarmSummaryTab.SetRowElementValue(pointIdid, alarm.PointID);
	AlarmSummaryTab.SetRowElementValue(descriptionId, alarm.Description);
	AlarmSummaryTab.SetRowElementValue(tagIdid, alarm.TagID);
	//Status
	var statusNodeArr = $('#' + statusId);
	var statusNode = statusNodeArr[0];
	statusNode.style.backgroundColor = '#' + backgroundColor;
	statusNode.style.color = '#' + textColor;
	AlarmSummaryTab.SetRowElementValue(statusId, alarm.Status);

	return false;
}

AlarmSummaryTab.DeleteRow = function (table, rowId) {
	//Problem is that the Row_ was used to identify if the row is visible or not instead of it's own seperate column
	//so now we have to delete two versions of the RowId to make sure we get it.
	try
	{
		var row = table.row(['#' + "Row_" + rowId, '#' + rowId]);
		row.remove();
	}
	catch (e) {
		alert(e);
	}
}

//===========================================================================
// This function compares the new model data with the existing rows in order
// to determine deletions, additions, or updates.
//===========================================================================
AlarmSummaryTab.AddingDeletingUpdatingRows = function (table, model, rows)
{
	var modelDict = {};
	var rowDict = {};

	model.AlarmSummaries.forEach( function( alarm )
	{
		var id = 'Row_' + alarm.AlarmGuid;
		modelDict[id] = alarm;
	});

	var rowsDeletedOrAdded = false;

	// Loop through the existing rows comparing to the model
	// in order to delete rows and identify rows to be updated.
	for(var i = 0; i < rows.length; i++)
	{
		var row = rows[i];

		if ( row.id !== "" )
		{
			var alarm = modelDict[row.id];

			if ( !alarm )
			{
				rowsDeletedOrAdded = true;
				AlarmSummaryTab.DeleteRow( table, row.id );
			}
			else
			{
				// Load the rows that will be updated.
				rowDict[row.id] = row;
			}
		}
	}

	// Loop through the module and compare to the rows that have not been deleted in order
	// to either update the table or add to the table.
	for(var key in modelDict)
	{
		if ( modelDict.hasOwnProperty(key) )
		{
			var updateAddrow = rowDict[key];
			var newAlarm = modelDict[key];

			if (AlarmSummaryTab.UpdateGridRowForAlarm(table, newAlarm, updateAddrow))
			{
				rowsDeletedOrAdded = true;
			}
		}
	}

	return rowsDeletedOrAdded;
}

AlarmSummaryTab.UpdateGridRowForAlarm = function (table, alarm, row)
{
	var alarmBackgroundSteadyColor = alarm.AlarmBackgroundSteadyColor;
	var alarmTextSteadyColor = alarm.AlarmTextSteadyColor;
	var alarmBackgroundAlternateColor = alarm.AlarmBackgroundAlternateColor;
	var alarmTextAlternateColor = alarm.AlarmTextAlternateColor;

	if (alarm.IsNormal)
	{
		alarmBackgroundSteadyColor = alarm.NormalUnacknowledgedAlarmBackgroundSteadyColor;
		alarmTextSteadyColor = alarm.NormalUnacknowledgedAlarmTextSteadyColor;
		alarmBackgroundAlternateColor = alarm.NormalUnacknowledgedAlarmBackgroundAlternateColor;
		alarmTextAlternateColor = alarm.NormalUnacknowledgedAlarmTextAlternateColor;
	}

	var backgroundColor = alarmBackgroundSteadyColor;
	var textColor = alarmTextSteadyColor;

	if (!AlarmSummaryTab.IsSteadyColor && !alarm.Acknowledged)
	{
		backgroundColor = alarmBackgroundAlternateColor;
		textColor = alarmTextAlternateColor;
	}

	if (!row)
	{
		AlarmSummaryTab.AddTableRow(table, alarm, backgroundColor, textColor);
		return true;
	}
	else
	{
		if (AlarmSummaryTab.UpdateRow(alarm, backgroundColor, textColor)){
			AlarmSummaryTab.DeleteRow(table, row.id);
			AlarmSummaryTab.AddTableRow(table, alarm, backgroundColor, textColor);
			return true;
		}
		return false;
	}
}

AlarmSummaryTab.ReorderFindArr = function ()
{
	var tempFindArr = [];
	var t = document.getElementById("AlarmSummaryTable");
	var tds = t.getElementsByTagName("td");

	for (var n = 0; n < tds.length; n++)
	{
		if ( AlarmSummaryTab.FindArr.indexOf( tds[n].id ) >= 0 && tempFindArr.indexOf( tds[n].id ) < 0 )
		{
			tempFindArr.push( tds[n].id );
		}
	}
	AlarmSummaryTab.FindArr = tempFindArr;
}

AlarmSummaryTab.UpdateGridRows = function ()
{
	if ( $( "#AlarmSummaryTable" ).length === 0 )
	{
		return;
	}
	//var table = $("#AlarmSummaryTable").DataTable();
	var rows = AlarmSummaryTab.DatatableHandle.rows().nodes();
	AlarmSummaryTab.IsSteadyColor = !AlarmSummaryTab.IsSteadyColor;
	if(AlarmSummaryTab.model == null || AlarmSummaryTab.model == { })
		AlarmSummaryTab.GetAlarmModel()
	else
		setTimeout(AlarmSummaryTab.GetAlarmModel(),0)
	AlarmSummaryTab.FindArr = [];
	var rowsAddedOrDeleted = AlarmSummaryTab.AddingDeletingUpdatingRows(AlarmSummaryTab.DatatableHandle, AlarmSummaryTab.model, rows);
	if (rowsAddedOrDeleted) {
		AlarmSummaryTab.DatatableHandle.draw(false);
	}

	AlarmSummaryTab.ShowEntriesProcessing(rows.length);
		setTimeout(AlarmSummaryTab.DoFindWorker(), 0);
		setTimeout(	AlarmSummaryTab.HandleButtonEnableDisable(),0);
	if (AlarmSummaryTab.silence)
	{
		AlarmSummaryTab.Silence();
	}
}


AlarmSummaryTab.AcknowledgeWComment = function ()
{
	var selectedRows = AlarmSummaryTab.GetCurrentlySelectedViewableAlarms();
	var ackDict = AlarmSummaryTab.GetAcknowledgedDictionaryByAlarmGuid();
	var alarmIdList = AlarmSummaryTab.UnAckAlarms(ackDict, selectedRows);
	AlarmSummaryTab.AcknowledgeWithCommentWorker(alarmIdList);

	return false;
}

AlarmSummaryTab.AcknowledgeAllWComment = function ()
{
	AlarmSummaryTab.AlarmAckAllDropDownClicked = 0;
	if (AlarmSummaryTab.needToScrollUpdate) {
	AlarmSummaryTab.visibleAlarms = AlarmSummaryTab.GetCurrentlyViewableAlarms();
	AlarmSummaryTab.needToScrollUpdate = false;
	}
	var viewableRows = AlarmSummaryTab.visibleAlarms;
	var ackDict = AlarmSummaryTab.GetAcknowledgedDictionaryByAlarmGuid();
	var alarmIdList = AlarmSummaryTab.UnAckAlarms(ackDict, viewableRows);
	AlarmSummaryTab.AcknowledgeWithCommentWorker(alarmIdList);

	return false;
}

AlarmSummaryTab.AcknowledgeAll = function ()
{
	if(AlarmSummaryTab.needToScrollUpdate) {
	AlarmSummaryTab.visibleAlarms = AlarmSummaryTab.GetCurrentlyViewableAlarms();
	AlarmSummaryTab.needToScrollUpdate = false;
	}
	var viewableRows = AlarmSummaryTab.visibleAlarms;
	var ackDict = AlarmSummaryTab.GetAcknowledgedDictionaryByAlarmGuid();
	var guidList = AlarmSummaryTab.UnAckAlarms(ackDict, viewableRows);
	return AlarmSummaryTab.AcknowledgeWorker(guidList);
}


AlarmSummaryTab.Shelve = function () {
	//Statement below is just for testing purposes.  Really need to get single selected alarm and ensure there is only one alarm selected
	var alarmIdList = AlarmSummaryTab.GetCurrentlySelectedViewableAlarms();
	AlarmSummaryTab.ShelveWorker(alarmIdList);
}

AlarmSummaryTab.ShelveWorker = function (alarmIdList) {

	$('body').modalmanager('loading');
	var modelStr = AlarmSummaryTab.GetAlarmModelString();
	var url = $('#ShelveAlarmsUrl').val();

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// remove previous notifications
	PNotify.removeStack(AlarmSummaryTab.messageAttributes.stack);

	$.ajax({
		type: 'POST',
		url: url,
		cache: false,
		headers: headers,
		dataType: 'json',
		contentType: 'application/json; charset=utf-8',
		data: JSON.stringify({
			modelStr: modelStr,
			alarmGuidList: alarmIdList
		}),
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					// replace the holder with the partial view
					$('#Shelve').html(data);
					$('#ShelveDataEntryScreen').modal('show');
				}
				else {
					// remove the loading of the modal
					var modalManager = $('body').data('modalmanager');
					modalManager.removeLoading();
				}
			}, AlarmSummaryTab.messageAttributes);
		},
		error: function (xhr, textStatus, error) {
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () {
					// remove the loading of the modal
					var modalManager = $('body').data('modalmanager');
					modalManager.removeLoading();
				}, AlarmSummaryTab.messageAttributes);
		}
	});
	//Prevent Post
	return false;
}

AlarmSummaryTab.AcknowledgeWithCommentWorker = function (alarmIdList)
{

	$('body').modalmanager('loading');
	var modelStr = AlarmSummaryTab.GetAlarmModelString();
	var url = $('#AckWithCommentUrl').val();

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// remove previous notifications
	PNotify.removeStack(AlarmSummaryTab.messageAttributes.stack);

	$.ajax({
		type: 'POST',
		url: url,
		cache: false,
		headers: headers,
		dataType: 'json',
		contentType: 'application/json; charset=utf-8',
		data: JSON.stringify({ modelStr: modelStr, alarmGuidList: alarmIdList}),
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					// replace the holder with the partial view
					$('#AckComment').html(data);
					$('#AckCommentDataEntryScreen').modal('show');
				}
				else {
					// remove the loading of the modal
					var modalManager = $('body').data('modalmanager');
					modalManager.removeLoading();
				}
			}, AlarmSummaryTab.messageAttributes);
		},
		error: function (xhr, textStatus, error) {
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () {
					// remove the loading of the modal
					var modalManager = $('body').data('modalmanager');
					modalManager.removeLoading();
				}, AlarmSummaryTab.messageAttributes);
		}
	});
	//Prevent Post
	return false;
}


AlarmSummaryTab.Silence = function ()
{
	document.getElementById("silencebuttonimage").src = window.applicationRootName+ "/FMWebApp/Images/Silence-on.png";

	/*    AlarmSummaryTab.silence = !AlarmSummaryTab.silence;

    if (AlarmSummaryTab.silence == false) {
        document.getElementById("silencebuttonimage").src = window.applicationRootName+ "/FMWebApp/Images/Silence-off.png";
    }
    else {
        document.getElementById("silencebuttonimage").src = window.applicationRootName+ "/FMWebApp/Images/Silence-on.png";
    }

    AlarmSummaryTab.Silence = function () {
    }
*/
	if (AlarmSummaryTab.needToScrollUpdate) {
	AlarmSummaryTab.visibleAlarms = AlarmSummaryTab.GetCurrentlyViewableAlarms();
	AlarmSummaryTab.needToScrollUpdate = false;
			}
	var viewableRows = AlarmSummaryTab.visibleAlarms;
    var ackDict = AlarmSummaryTab.GetSilencedDictionaryByAlarmGuid();
    var guidList = AlarmSummaryTab.UnSilenceAlarms(ackDict, viewableRows);
    AlarmSummaryTab.SilenceWorker(guidList);

	window.setTimeout( function()
	{
		document.getElementById("silencebuttonimage").src = window.applicationRootName+ "/FMWebApp/Images/Silence-off.png";
	}, 1000 );

	//Change silence button picture to be Silence-Off.png if AlarmSummaryTab.silence = false
	//Change silence button picture to be Silence-On.png if AlarmSummaryTab.silence = true
	//Prevent Post
	return false;
}

AlarmSummaryTab.AlarmGraphic = function () {
	var guidList = AlarmSummaryTab.GetCurrentlySelectedViewableAlarms();

	if ( !guidList || guidList.length !== 1 )
	{
		FMErrorAndExceptionHandling.ShowError( 'Select Only One Alarm For This Operation', null, null );
	}
	else
	{
		var findAlarm = $.grep(AlarmSummaryTab.model.AlarmSummaries, function (obj) {
			return obj.AlarmGuid === guidList[0];
		});

		if (findAlarm.length > 0)
		{
			var pointName = findAlarm[0].PointID;
			var pointGuid = findAlarm[0].PointGuid;
			FMOperateIndex.openPoint( pointName, pointGuid );
		}
	}
	//Prevent Post
	return false;
}

AlarmSummaryTab.Help = function()
{
	var guidList = AlarmSummaryTab.GetCurrentlySelectedViewableAlarms();

	if ( !guidList || guidList.length !== 1 )
	{
		FMErrorAndExceptionHandling.ShowError( 'Select Only One Alarm For This Operation', null, null );
	}
	else
	{
		console.log( "AlarmSummaryTab.Help called" );
	}
	//Prevent Post
	return false;
}

AlarmSummaryTab.Details = function ()
{
	var guidList = AlarmSummaryTab.GetCurrentlySelectedViewableAlarms();

	if ( !guidList || guidList.length !== 1 )
	{
		FMErrorAndExceptionHandling.ShowError( 'Select Only One Alarm For This Operation', null, null );
	}
	else
	{
		console.log( "AlarmSummaryTab.Details called" );
	}
	//Prevent Post
	return false;
}

AlarmSummaryTab.PreventEnterSubmit = function (e) {
	//Prevent Post
	e = e || event;
	return (e.keyCode || e.which || e.charCode || 0) !== 13;
}

AlarmSummaryTab.Newdiv = function (force)
{
	var newDiv = "#findResultsRow"; 
	if ( AlarmSummaryTab.FindArr )
	{
		if ( AlarmSummaryTab.FindArr.length > 0 )
		{
			if ( $( newDiv ).is( ':hidden' ) || force )
			{
				$( newDiv ).removeClass( 'hidden' );
			}
		}
		else
		{
			$(newDiv).removeClass('hidden').addClass('hidden');
		}
	}
}

AlarmSummaryTab.ShowHideFindResults = function (show)
{
	AlarmSummaryTab.Newdiv(false);
	if ( show )
	{
		AlarmSummaryTab.SetFindResults();
		AlarmSummaryTab.SetFindCurrentRowIndicator();
	}
	else
	{
		if (AlarmSummaryTab.needToClearFindBorder == true) {
			AlarmSummaryTab.needToClearFindBorder = false
			AlarmSummaryTab.HideFindCurrentRowIndicator();
		}
	}
};

AlarmSummaryTab.HideFindCurrentRowIndicator = function ()
{
	var table = document.getElementById("AlarmSummaryTable");
	var td = table.getElementsByTagName("td");
	if (td) {
		for (var i = 0; i < td.length; i++) {
			td[i].style.border = "none";
		}
	}
	
}

AlarmSummaryTab.SetFindCurrentRowIndicator = function ()
{
	AlarmSummaryTab.HideFindCurrentRowIndicator();
	if (AlarmSummaryTab.CurrentFind)
	{
		var currentField = document.getElementById( AlarmSummaryTab.CurrentFind );
		if ( currentField )
		{
			currentField.style.border = "1px solid red"; 
		}
	} 
}

AlarmSummaryTab.SetFindResults = function ()
{

	var numFindResults = 0;
	if ( AlarmSummaryTab.FindArr )
	{
		numFindResults = AlarmSummaryTab.FindArr.length;
	}
	var findResultsString = "<i>" + numFindResults + " results</i>";
	var findResultsLabel = document.getElementById('findResultsLabel');
	findResultsLabel.innerHTML = findResultsString; 
}

AlarmSummaryTab.DoFindWorker = function ()
{

	if (AlarmSummaryTab.CurrentFindString && AlarmSummaryTab.CurrentFindString.length > 0)
	{
		if ( AlarmSummaryTab.FindArr.length > 0 )
		{
			AlarmSummaryTab.needToClearFindBorder = true;
			AlarmSummaryTab.ReorderFindArr();
			//Handle CurrentFind
			if ( !AlarmSummaryTab.CurrentFind || AlarmSummaryTab.FindArr.indexOf( AlarmSummaryTab.CurrentFind ) < 0 )
			{
				AlarmSummaryTab.CurrentFind = AlarmSummaryTab.FindArr[0];
				AlarmSummaryTab.ScrollToCurrent();
			}
			AlarmSummaryTab.ShowHideFindResults( true );
		}
		else
		{
			AlarmSummaryTab.ShowHideFindResults( false );
		}
	}
	else
	{
		AlarmSummaryTab.ShowHideFindResults(false);
	}
}


AlarmSummaryTab.DoFind = function (e)
{
	var text = e.target.value;
	AlarmSummaryTab.CurrentFindString = text;
	AlarmSummaryTab.FindArr = [];
	AlarmSummaryTab.CurrentFind = null;
};

AlarmSummaryTab.ScrollToCurrent = function()
{
	if ( AlarmSummaryTab.CurrentFind )
	{
		var currentFindElement = document.getElementById( AlarmSummaryTab.CurrentFind );
		if ( currentFindElement )
		{
			currentFindElement.scrollIntoViewIfNeeded( );
		}
	}
}

AlarmSummaryTab.FindNext = function ()
{
	if (AlarmSummaryTab.FindArr && AlarmSummaryTab.CurrentFind)
	{
		var currentFindIndex = AlarmSummaryTab.FindArr.indexOf(AlarmSummaryTab.CurrentFind);

		if (currentFindIndex >= 0 && currentFindIndex < AlarmSummaryTab.FindArr.length - 1)
		{
			AlarmSummaryTab.CurrentFind = AlarmSummaryTab.FindArr[currentFindIndex + 1];
			AlarmSummaryTab.SetFindCurrentRowIndicator();
			AlarmSummaryTab.ScrollToCurrent();
		}
		else
		{
			AlarmSummaryTab.CurrentFind = AlarmSummaryTab.FindArr[0];
			AlarmSummaryTab.SetFindCurrentRowIndicator();
			AlarmSummaryTab.ScrollToCurrent();
		}
	}
	return false;
};

AlarmSummaryTab.FindPrev = function ()
{
	if (AlarmSummaryTab.FindArr && AlarmSummaryTab.CurrentFind)
	{
		var currentFindIndex = AlarmSummaryTab.FindArr.indexOf(AlarmSummaryTab.CurrentFind);

		if (currentFindIndex > 0)
		{
			AlarmSummaryTab.CurrentFind = AlarmSummaryTab.FindArr[currentFindIndex - 1];
			AlarmSummaryTab.SetFindCurrentRowIndicator();
			AlarmSummaryTab.ScrollToCurrent();
		}
		else
		{
			AlarmSummaryTab.CurrentFind = AlarmSummaryTab.FindArr[AlarmSummaryTab.FindArr.length - 1];
			AlarmSummaryTab.SetFindCurrentRowIndicator();
			AlarmSummaryTab.ScrollToCurrent();
		}

	}
	return false;
};

AlarmSummaryTab.isElementInViewport = function(par, el, floatingHeader)
{
	var elRect = el.getBoundingClientRect();
	var parRect = par.getBoundingClientRect();
	var winBottom = $(window).height();
	var floatingHeaderHeight = 0;
	if ( floatingHeader )
	{
		var rect = floatingHeader.getBoundingClientRect();
		floatingHeaderHeight = rect.bottom - rect.top;
	}
	return (
		 elRect.top > 0 &&
		 elRect.top >= parRect.top + floatingHeaderHeight &&
		 elRect.left + 2 >= parRect.left &&
		 elRect.bottom <= parRect.bottom &&
		 elRect.bottom <= winBottom &&
		 elRect.right <= parRect.right
	);
}

AlarmSummaryTab.UnAckAlarms = function(dict, guidArr)
{
	var unAckedAlarms = [];
	for ( var i = 0; i < guidArr.length; i++ )
	{
		var result = (dict[guidArr[i]] === undefined);
		if ( !result && dict[guidArr[i]] === false )
		{
			unAckedAlarms.push( guidArr[i] );
		}
	}
	return unAckedAlarms;
}

AlarmSummaryTab.UnSilenceAlarms = function (dict, guidArr) {
	var UnSilenceAlarms = [];
	for (var i = 0; i < guidArr.length; i++) {
		var result = (dict[guidArr[i]] === undefined);
		if (!result && dict[guidArr[i]] === false) {
			UnSilenceAlarms.push(guidArr[i]);
		}
	}
	return UnSilenceAlarms;
}



AlarmSummaryTab.GetAcknowledgedDictionaryByAlarmGuid = function () {
	if(AlarmSummaryTab.model == null)
	{
AlarmSummaryTab.GetAlarmModel();
}
	var model = AlarmSummaryTab.model;
	var dict = {};
	model.AlarmSummaries.forEach(function (alarm) {
		var id = alarm.AlarmGuid;
		dict[id] = (alarm.Acknowledged || !alarm.Acknowledge) ? true : false;
	});
	return dict;
}


AlarmSummaryTab.GetSilencedDictionaryByAlarmGuid = function () {
	if (AlarmSummaryTab.model == null) {
		AlarmSummaryTab.GetAlarmModel();
	}
	var model = AlarmSummaryTab.model;
	var dict = {};
	model.AlarmSummaries.forEach(function (alarm) {
		var id = alarm.AlarmGuid;
		dict[id] = (alarm.Silenced) ? true : false;
	});
	return dict;
}

AlarmSummaryTab.InitDOMElements = function()
{
	AlarmSummaryTab.DOMElements = {};
	AlarmSummaryTab.DOMElements.shelveButton = document.getElementById("shelveButton");
	AlarmSummaryTab.DOMElements.ackButton = document.getElementById("ackButton");
	AlarmSummaryTab.DOMElements.ackAllButton = document.getElementById("ackAllButton");
	AlarmSummaryTab.DOMElements.dropDownArrow = document.getElementById("dropDownArrow");
	AlarmSummaryTab.DOMElements.dropDownArrow2 = document.getElementById("dropDownArrow2");
	AlarmSummaryTab.DOMElements.alarmGraphicButton = document.getElementById("alarmGraphicButton");
	AlarmSummaryTab.DOMElements.helpButton = document.getElementById("helpButton");
	AlarmSummaryTab.DOMElements.detailsButton = document.getElementById("detailsButton");
}

AlarmSummaryTab.HandleButtonEnableDisable = function()
{
	if ( $( "#AlarmSummaryTabView" ).length > 0 )
	{
		if (AlarmSummaryTab.ButDOMElementstons == null)
			AlarmSummaryTab.InitDOMElements();
		var selectedRows = AlarmSummaryTab.GetCurrentlySelectedViewableAlarms();
	if (AlarmSummaryTab.needToScrollUpdate) {
	AlarmSummaryTab.visibleAlarms = AlarmSummaryTab.GetCurrentlyViewableAlarms();
	AlarmSummaryTab.needToScrollUpdate = false;
		}
	var viewableRows = AlarmSummaryTab.visibleAlarms;
		var ackDict = AlarmSummaryTab.GetAcknowledgedDictionaryByAlarmGuid();
		var selectedUnAckAlarmsCount = AlarmSummaryTab.UnAckAlarms( ackDict, selectedRows ).length;
		var viewableUnAckAlarmsCount = AlarmSummaryTab.UnAckAlarms( ackDict, viewableRows ).length;
		var numSelectedRows = selectedRows.length;

		if ( numSelectedRows === 1 )
		{
			if (AlarmSummaryTab.DOMElements.shelveButton.disabled)
			{
				AlarmSummaryTab.DOMElements.shelveButton.disabled = false;
				AlarmSummaryTab.DOMElements.shelveButton.classList.remove("alarmSummaryDisableButtonClass");
				AlarmSummaryTab.DOMElements.alarmGraphicButton.disabled = false;
				AlarmSummaryTab.DOMElements.alarmGraphicButton.classList.remove("alarmSummaryDisableButtonClass");
				AlarmSummaryTab.DOMElements.helpButton.disabled = false;
				AlarmSummaryTab.DOMElements.helpButton.classList.remove("alarmSummaryDisableButtonClass");
				AlarmSummaryTab.DOMElements.detailsButton.disabled = false;
				AlarmSummaryTab.DOMElements.detailsButton.classList.remove("alarmSummaryDisableButtonClass");
			}
		}
		else
		{
			if (!AlarmSummaryTab.DOMElements.shelveButton.disabled)
			{
				AlarmSummaryTab.DOMElements.shelveButton.disabled = true;
				AlarmSummaryTab.DOMElements.shelveButton.classList.add("alarmSummaryDisableButtonClass");
				AlarmSummaryTab.DOMElements.alarmGraphicButton.disabled = true;
				AlarmSummaryTab.DOMElements.alarmGraphicButton.classList.add("alarmSummaryDisableButtonClass");
				AlarmSummaryTab.DOMElements.helpButton.disabled = true;
				AlarmSummaryTab.DOMElements.helpButton.classList.add("alarmSummaryDisableButtonClass");
				AlarmSummaryTab.DOMElements.detailsButton.disabled = true;
				AlarmSummaryTab.DOMElements.detailsButton.classList.add("alarmSummaryDisableButtonClass");
			}
		}

		if ( selectedUnAckAlarmsCount !== 0 )
		{
			if (AlarmSummaryTab.DOMElements.ackButton.disabled) {
				AlarmSummaryTab.DOMElements.ackButton.disabled = false;
				AlarmSummaryTab.DOMElements.ackButton.classList.remove("alarmSummaryAckDisableButtonClass");
				AlarmSummaryTab.DOMElements.dropDownArrow.disabled = false;
				AlarmSummaryTab.DOMElements.dropDownArrow.classList.remove("alarmSummaryDisableButtonClass");
			}
		}
		else
		{

			if (!AlarmSummaryTab.DOMElements.ackButton.disabled) {
				AlarmSummaryTab.DOMElements.ackButton.disabled = true;
				AlarmSummaryTab.DOMElements.ackButton.classList.add("alarmSummaryAckDisableButtonClass");
				AlarmSummaryTab.DOMElements.dropDownArrow.disabled = true;
				AlarmSummaryTab.DOMElements.dropDownArrow.classList.add("alarmSummaryDisableButtonClass");
				$( "#AckWCommentButton" ).removeClass( 'hidden' ).addClass( 'hidden' );
				$( "#dropDownArrow span" ).removeClass( 'glyphicon-triangle-bottom' ).addClass( "glyphicon-triangle-top" );
			}
		}

		//debugger;
		if ( viewableUnAckAlarmsCount !== 0 )
		{
			if (AlarmSummaryTab.DOMElements.ackAllButton.disabled && $('#HasAckAllRight').val() === 'True') {
				AlarmSummaryTab.DOMElements.ackAllButton.disabled = false;
				AlarmSummaryTab.DOMElements.ackAllButton.classList.remove("alarmSummaryAckDisableButtonClass");
			}
			if (AlarmSummaryTab.DOMElements.dropDownArrow2.disabled && $('#HasAckCommentsRight').val() === 'True') {
				AlarmSummaryTab.DOMElements.dropDownArrow2.disabled = false;
				AlarmSummaryTab.DOMElements.dropDownArrow2.classList.remove("alarmSummaryDisableButtonClass");
			}
		}
		else
		{
			if (!AlarmSummaryTab.DOMElements.ackAllButton.disabled || $('#HasAckAllRight').val() === 'False') {
				AlarmSummaryTab.DOMElements.ackAllButton.disabled = true;
				AlarmSummaryTab.DOMElements.ackAllButton.classList.add("alarmSummaryAckDisableButtonClass");
			}
			if (!AlarmSummaryTab.DOMElements.dropDownArrow2.disabled || $('#HasAckCommentsRight').val() === 'False') {
				AlarmSummaryTab.DOMElements.dropDownArrow2.classList.add("alarmSummaryDisableButtonClass");
				AlarmSummaryTab.DOMElements.dropDownArrow2.disabled = true;
				$( "#dropDownArrow2 span" ).removeClass( 'glyphicon-triangle-bottom' ).addClass( "glyphicon-triangle-top" );
				$( "#AckAllWCommentButton" ).removeClass( 'hidden' ).addClass( 'hidden' );
			}
		}
	}
}

AlarmSummaryTab.GetCurrentlyViewableAlarms = function()
{
	var container = document.getElementById( "AlarmSummaryTableContainer" );
	var tr = container.getElementsByTagName( "tr" );
	var visible = [];
	var header = container.getElementsByTagName("thead")[0];
	for (var i = 0; i < tr.length; i++) {
		var cur = tr[i];
		if ( cur.id.startsWith( "Row_" ) && AlarmSummaryTab.isElementInViewport( container, cur, header ) )
		{
			visible.push(cur.id.replace('Row_', ''));
		}
	}
	return visible;
}

AlarmSummaryTab.GetCurrentlySelectedViewableAlarms = function () {
	//var table = $("#AlarmSummaryTable").DataTable();
	var selectedRowIds = [];
	if (AlarmSummaryTab.needToScrollUpdate) {
		AlarmSummaryTab.visibleAlarms = AlarmSummaryTab.GetCurrentlyViewableAlarms();
		AlarmSummaryTab.needToScrollUpdate = false;
	}
		for (var i = 0; i < AlarmSummaryTab.visibleAlarms.length; i++) {
				var rowElement = document.getElementById("Row_" +AlarmSummaryTab.visibleAlarms[i]) ;
				if (rowElement) {
			for (var j = 0; j < rowElement.classList.length; j++) {
				if (rowElement.classList[j] === "selected") {
					selectedRowIds.push(AlarmSummaryTab.visibleAlarms[i]);
					break;
			}
				}
	}
	}
	return selectedRowIds;
}

AlarmSummaryTab.ShowEntriesProcessing = function (numEntries)
{
	var showEntriesString = "No Alarms";
	if ( numEntries > 0 )
	{
		showEntriesString = "Showing " + numEntries + " Alarms";
	}
	var entriesLabel = document.getElementById('ShowingEntriesLabel');//$( "#ShowingEntriesLabel" );
	entriesLabel.innerHTML = showEntriesString;
};

AlarmSummaryTab.Refresh = function () {
	var refreshTimeout = 1000;
	var refreshStartTime = Date.now();

	AlarmSummaryTab.UpdateGridRows();

    if (AlarmSummaryTab.InAjaxRefreshCall === true)
		{
		  return;
    }

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	if ($('#AlarmSummaryRefreshUrl').length === 0)
	{
			AlarmSummaryTab.RefreshTimer = setTimeout(function () { AlarmSummaryTab.Refresh(); }, getSummaryRefreshTimeout(refreshStartTime, refreshTimeout));
		return;
	}
	var url = $('#AlarmSummaryRefreshUrl').val();
	
	AlarmSummaryTab.InAjaxRefreshCall = true;
	var loadImage = $("#loadingimage");

	$.ajax({
		type: 'POST',
		url: url,
		cache: false,
		headers: headers,
		data: {
			'__RequestVerificationToken': token
		},
		success: function (response) {
		    // remove previous notifications
			PNotify.removeStack(AlarmSummaryTab.messageAttributes.stack);
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError)
				{
				    AlarmSummaryTab.SetAlarmModel(data);
				    if (loadImage.is(':visible') === true)
				        loadImage.fadeOut(1000);
                }
				AlarmSummaryTab.InAjaxRefreshCall = false;
			}, AlarmSummaryTab.messageAttributes);

				AlarmSummaryTab.RefreshTimer = setTimeout(function () { AlarmSummaryTab.Refresh(); }, getSummaryRefreshTimeout(refreshStartTime, refreshTimeout));
		},
		error: function (xhr, textStatus, error) {
			if (PNotify.notices.length === 0
			|| PNotify.notices[PNotify.notices.length - 1].state === 'closed') {
				PNotify.removeStack(AlarmSummaryTab.messageAttributes.stack);
				FMErrorAndExceptionHandling.ShowError($('#CommunicationsFailureText').val(),
					function () {
						AlarmSummaryTab.InAjaxRefreshCall = false;
					}, AlarmSummaryTab.messageAttributes);
			}
			else {
				AlarmSummaryTab.InAjaxRefreshCall = false;
			}

				AlarmSummaryTab.RefreshTimer = setTimeout(function () { AlarmSummaryTab.Refresh(); }, getSummaryRefreshTimeout(refreshStartTime, refreshTimeout));
		}
	});
}

//AlarmSummaryTab.StartTimer = function ()    //bds
//{
//	console.log("Starting StartTimer");
//    if (!AlarmSummaryTab.RefreshTimer)
//		{
//			AlarmSummaryTab.RefreshTimer = setTimeout(AlarmSummaryTab.Refresh, 1000);
//    }
//}

//AlarmSummaryTab.StopTimer = function ()
//{
//	console.log("Starting StopTimer");
//	if ( AlarmSummaryTab.RefreshTimer )
//	{
//		clearTimer(AlarmSummaryTab.RefreshTimer);
//	}
//	AlarmSummaryTab.RefreshTimer = null;
//}

AlarmSummaryTab.reinitializesummarydisplay = function ()
{
    var activeTab = FMOperateIndex.GetActiveTab("alarmSummary", AlarmSummaryTab.TabidNumber);
    if (activeTab === true) {
        clearInterval(AlarmSummaryTab.RefreshActiveCheckTimer);
        AlarmSummaryTab.RefreshActiveCheckTimer = null;
        AlarmSummaryTab.Initialize();
    }
    else if (FMOperateIndex.allScreensRestored === true) {
        clearInterval(AlarmSummaryTab.RefreshActiveCheckTimer);
        AlarmSummaryTab.RefreshActiveCheckTimer = null;
    }
}

AlarmSummaryTab.Init = function ()
{
    var h = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    h = h * 0.7;
    h = Math.round(h);
    var hString = h + 'px';
    var tabIDnumber = $("#alarmsummarytabname")[0].innerText;

    AlarmSummaryTab.TabidNumber = tabIDnumber;
    
	$('<div id="loadingimage" class="LoadingAnimation transparent"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#AlarmSummaryTableSection');
    AlarmSummaryTab.DatatableHandle = $("#AlarmSummaryTable").DataTable({
        "order": [[0, "desc"]],
        "columns": [
                  { "orderable": false, "visible": false } //TimeStampIndex
                  , { "orderable": false } //TimeStamp
                  , { "orderable": false } //PointID
                  , { "orderable": false } //Description
                  , { "orderable": false } //TagID
                  , { "orderable": false } //Status
        ],
        "ordering": true,
        "scrollY": hString,
        "sScrollX": '100%',
        "sScrollXInner": '100%',
        "scrollCollapse": false,
        "paging": false,
        "autoWidth": true,
        "bFilter": false,
        "bInfo": false
    });


    if (FMOperateIndex.openingNewAlarmSummary === false) {
        var activeTab = FMOperateIndex.GetActiveTab("alarmSummary", AlarmSummaryTab.TabidNumber);
        if (activeTab === false && AlarmSummaryTab.RefreshActiveCheckTimer === null) {  // not the active window so start a timer to check every 200msec. The operate index object is not very efficient
            // at determining when a window is active and when it is not
            AlarmSummaryTab.RefreshActiveCheckTimer = setInterval(AlarmSummaryTab.reinitializesummarydisplay, 500);
            return;
        }
    }

    AlarmSummaryTab.Initialize();
}

AlarmSummaryTab.Initialize = function () {
	// make sure that the init timer is not running
	if(AlarmSummaryTab.RefreshActiveCheckTimer !== null) {
	    clearInterval(AlarmSummaryTab.RefreshActiveCheckTimer);
	    AlarmSummaryTab.RefreshActiveCheckTimer = null;
	}

	if (AlarmSummaryTab.SummaryInitialized === true)
	    return;

	AlarmSummaryTab.SummaryInitialized = true;

	AlarmSummaryTab.stack_bottomright_alarmsummary = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#AlarmSummaryTableSection').parent() };
	AlarmSummaryTab.messageAttributes = { addclass: 'stack-bottomright', stack: AlarmSummaryTab.stack_bottomright_alarmsummary };

	$('#AlarmSummaryTable tbody').on('click', 'tr', function () {
		$(this).toggleClass('selected');
		AlarmSummaryTab.HandleButtonEnableDisable();
	});

	var find = document.getElementById('alarmSummaryTabFind');
	find.oninput = AlarmSummaryTab.DoFind;

	$("#ShelveDataEntryScreenOkButton").click(function() {
		ShelveDataEntry.ShelveDataEntrySelectionOkButtonPressAction();
		$('#ShelveDataEntryScreen').modal('hide');
	});
	$("#AckCommentDataEntryScreenOkButton").click(function() {
		AckCommentDataEntry.AckCommentDataEntrySelectionOkButtonPressAction();
	});
	AlarmSummaryTab.ShowEntriesProcessing();
	AlarmSummaryTab.ShowHideAckwCommentButtons(false);
	//AlarmSummaryTab.ShowHideAckAllwCommentButtons(false);
	AlarmSummaryTab.HandleButtonEnableDisable();

	$(document).click(function() {
		AlarmSummaryTab.ShowHideAckwCommentButtons(false);
		AlarmSummaryTab.ShowHideAckAllwCommentButtons(false);
	});

	$('.dataTables_scrollBody').on('scroll', function() {
	AlarmSummaryTab.needToScrollUpdate = true;
		AlarmSummaryTab.HandleButtonEnableDisable();
	});
	AlarmSummaryTab.Refresh();
};

AlarmSummaryTab.AcknowledgeDropdown = function () {
    event.stopPropagation();
    AlarmSummaryTab.ShowHideAckwCommentButtons(true);
    //Prevent Post
    return false;
}

AlarmSummaryTab.AcknowledgeAllDropdown = function () {
	event.stopPropagation();
    AlarmSummaryTab.ShowHideAckAllwCommentButtons(true);
    //Prevent Post
    return false;
}

AlarmSummaryTab.ShowHideAckwCommentButtons = function (show)
{

    if (show === false) {
    	$("#AckWCommentButton").removeClass('hidden').addClass('hidden');
    	$("#dropDownArrow span").removeClass('glyphicon-triangle-bottom').addClass("glyphicon-triangle-top");
    }
    else {
    	if ($("#AckWCommentButton").hasClass('hidden')) {
    		$("#AckWCommentButton").removeClass('hidden');
    		$("#dropDownArrow span").removeClass('glyphicon-triangle-top').addClass("glyphicon-triangle-bottom");
    	}
    	else {
    		$("#AckWCommentButton").removeClass('hidden').addClass('hidden');
    		$("#dropDownArrow span").removeClass('glyphicon-triangle-bottom').addClass("glyphicon-triangle-top");
    	}
	    AlarmSummaryTab.ShowHideAckAllwCommentButtons( false );
    }
};

AlarmSummaryTab.ShowHideAckAllwCommentButtons = function (show) {
   
	if ( show === false )
	{
		$("#AckAllWCommentButton").removeClass('hidden').addClass('hidden');
		$("#dropDownArrow2 span").removeClass('glyphicon-triangle-bottom').addClass("glyphicon-triangle-top");
	}
	else
	{
		if ( $( "#AckAllWCommentButton" ).hasClass( 'hidden' ) )
		{
			$("#AckAllWCommentButton").removeClass('hidden');
			$( "#dropDownArrow2 span" ).removeClass( 'glyphicon-triangle-top' ).addClass( "glyphicon-triangle-bottom" );
		}
		else
		{
			$( "#AckAllWCommentButton" ).removeClass( 'hidden' ).addClass( 'hidden' );
		}
		AlarmSummaryTab.ShowHideAckwCommentButtons( false );
	}

};

AlarmSummaryTab.ResizeColumns = function ()
{
    if (AlarmSummaryTab.SummaryInitialized === false)
    {
        AlarmSummaryTab.Initialize();
    }

	if ( AlarmSummaryTab.DatatableHandle != null )
	{
		AlarmSummaryTab.DatatableHandle.columns.adjust().draw();
	}
}

function getSummaryRefreshTimeout(startTime, refreshTimeout) {
  var elapsedTime = (Date.now() - startTime);
	var efficientRefreshTimeout = refreshTimeout - elapsedTime;
	efficientRefreshTimeout = (efficientRefreshTimeout < 0 ? 0 : efficientRefreshTimeout);
	//console.log("SUM " + efficientRefreshTimeout);
	AlarmSummaryTab.Statistics.push({ timestamp: Date.now(), elapsed: elapsedTime });

	return efficientRefreshTimeout;
}

AlarmSummaryTab.GetStatistics = function () {
	var minuteCount = 0;
	var minuteTotalTime = 0;
	var minuteMaxTime = 0;
	var sessionCount = 0;
	var sessionTotalTime = 0;
	var sessionMaxTime = 0;
	var timestamp = Date.now();

	for (i = AlarmSummaryTab.Statistics.length - 1; i > 0; i--) {
		var record = AlarmSummaryTab.Statistics[i];
		if (timestamp - record.timestamp <= 60000) {
			minuteCount++;
			minuteTotalTime += record.elapsed;
			if (record.elapsed > minuteMaxTime) {
				minuteMaxTime = record.elapsed;
			}
		}
		sessionCount++;
		sessionTotalTime += record.elapsed;
		if (record.elapsed > sessionMaxTime) {
			sessionMaxTime = record.elapsed;
		}
	}
	return {
		minuteAvgTime: minuteCount > 0 ? minuteTotalTime / minuteCount : 0,
		minuteMaxTime: minuteMaxTime,
		sessionAvgTime: sessionCount > 0 ? sessionTotalTime / sessionCount : 0,
		sessionMaxTime: sessionMaxTime
	};
}
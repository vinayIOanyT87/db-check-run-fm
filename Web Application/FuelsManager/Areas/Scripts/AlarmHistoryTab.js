var AlarmHistoryTab = AlarmHistoryTab ||
{
		FindArr: null,
		CurrentFind: null,
		CurrentFindString: '',
		dateTimeColumnIndex: 0,
		siteColumnIndex: 1,
		pointTypeColumnIndex: null,
		pointColumnIndex: null,
		pointDescriptionColumnIndex: null,
		variableColumnIndex: null,
		valueColumnIndex: null,
		unitsColumnIndex: null,
		alarmStateColumnIndex: null,
		priorityColumnIndex: null,
		actionColumnIndex: null,
		userColumnIndex: null,
		commentsColumnIndex: null,
		alarmOrTagGuidColumnIndex: null,
		alarmAndEventRecordGuidColumnIndex: null,
		commentUserNameColumnIndex: null,
		commentDateTimeColumnIndex: null,
		columnFilterCollection: null,
		previousColumnFilterCollection: null,
		originalEditValue: null,
		inEditMode: false,
		editModeId: null,
		checkboxFilterArray: [],
		commentIsEditableSpecialIndex: 77777,
		DatatableHandle: null,
		RefreshTimer: null,
		HistoryInitialized: false,
		TabidNumber: null,
		recordTypeFilter: null,
		alarmRecordType: 1,
		eventRecordType: 2,
		alarmEventRecordType: 3,
		// notification stack for the screen 
		stack_bottomright_alarmhistory: { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#AlarmHistoryTableSection') },
		messageAttributes: {}
};

if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

AlarmHistoryTab.GetAlarmModelString = function () {
	return $('#AlarmHistoryTabModel').val();
}

AlarmHistoryTab.GetAlarmModel = function () {
	return JSON.parse(AlarmHistoryTab.GetAlarmModelString());
}

AlarmHistoryTab.SetAlarmModelString = function (modelStr) {
	$('#AlarmHistoryTabModel').val(modelStr);
}

AlarmHistoryTab.SetAlarmModel = function (model) {
	var modelStr = JSON.stringify(model);
	AlarmHistoryTab.SetAlarmModelString(modelStr);
}

AlarmHistoryTab.AlarmGraphic = function () {
	var guidList = AlarmHistoryTab.GetCurrentlySelectedViewableAlarms();

	if (!guidList || guidList.length !== 1) {
		FMErrorAndExceptionHandling.ShowError('Select Only One Alarm For This Operation', null, AlarmHistoryTab.messageAttributes);
	}
	else {
		console.log("AlarmHistoryTab.AlarmGraphic called");
	}
	//Prevent Post
	return false;
}

AlarmHistoryTab.Help = function () {
	var guidList = AlarmHistoryTab.GetCurrentlySelectedViewableAlarms();

	if (!guidList || guidList.length !== 1) {
		FMErrorAndExceptionHandling.ShowError('Select Only One Alarm For This Operation', null, AlarmHistoryTab.messageAttributes);
	}
	else {
		console.log("AlarmHistoryTab.Help called");
	}
	//Prevent Post
	return false;
}

AlarmHistoryTab.Details = function () {
	var guidList = AlarmHistoryTab.GetCurrentlySelectedViewableAlarms();

	if (!guidList || guidList.length !== 1) {
		FMErrorAndExceptionHandling.ShowError('Select Only One Alarm For This Operation', null, AlarmHistoryTab.messageAttributes);
	}
	else {
		console.log("AlarmHistoryTab.Details called");
	}
	//Prevent Post
	return false;
}

AlarmHistoryTab.isElementInViewport = function (par, el, floatingHeader) {
	var elRect = el.getBoundingClientRect();
	var parRect = par.getBoundingClientRect();
	var winBottom = $(window).height();
	var floatingHeaderHeight = 0;
	if (floatingHeader) {
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

AlarmHistoryTab.GetCurrentlyViewableAlarms = function () {
	var container = document.getElementById("AlarmHistoryTableContainer");
	var tr = container.getElementsByTagName("tr");
	var visible = [];
	var header = container.getElementsByTagName("thead")[0];
	for (var i = 0; i < tr.length; i++) {
		var cur = tr[i];
		if (cur.id.startsWith("Row_") && AlarmHistoryTab.isElementInViewport(container, cur, header)) {
			visible.push(cur.id.replace('Row_', ''));
		}
	}
	return visible;
}

AlarmHistoryTab.GetCurrentlySelectedViewableAlarms = function () {
	var selectedRowIds = [];
	var visibleAlarms = AlarmHistoryTab.GetCurrentlyViewableAlarms();
	for (var i = 0; i < visibleAlarms.length; i++) {
		var rowElement = document.getElementById("Row_" + visibleAlarms[i]);
		if (rowElement) {
			for (var j = 0; j < rowElement.classList.length; j++) {
				if (rowElement.classList[j] === "selected") {
					selectedRowIds.push(visibleAlarms[i]);
					break;
				}
			}
		}
	}
	return selectedRowIds;
}

AlarmHistoryTab.CreateColumns = function () {
	var cols = [
		{ "data": "DateAndTime", "orderable": true, "visible": true  }
		, { "data": "Site", "orderable": true, "visible": true  }
		, { "data": "PointType", "orderable": true, "visible": true  }
		, { "data": "Point", "orderable": true, "visible": true  }
		, { "data": "PointDescription", "orderable": true, "visible": true  }
		, { "data": "Variable", "orderable": true, "visible": false  }
		, { "data": "Value", "orderable": true, "visible": false  }
		, { "data": "Units", "orderable": true, "visible": false  }
		, { "data": "AlarmState", "orderable": true, "visible": false  }
		, { "data": "Priority", "orderable": true, "visible": false  }
		, { "data": "Action", "orderable": true, "visible": false  }
		, { "data": "User", "orderable": true, "visible": false  }
		, { "data": "Comments", "orderable": true, "visible": false  }
		, { "data": "CommentUserName", "orderable": true, "visible": false }
		, { "data": "CommentDateTime", "orderable": true, "visible": false }
		, { "data": "AlarmOrTagGuid", "orderable": false, "visible": false }
		, { "data": "AlarmAndEventRecordGuid", "orderable": false, "visible": false }

	];

	// NOTE:  If the initial physical order of the columns change, the column indexes 
	// must be updated below.
	AlarmHistoryTab.dateTimeColumnIndex			= 0;
	AlarmHistoryTab.siteColumnIndex				= 1;
	AlarmHistoryTab.pointTypeColumnIndex		= 2;
	AlarmHistoryTab.pointColumnIndex			= 3;
	AlarmHistoryTab.pointDescriptionColumnIndex = 4;
	AlarmHistoryTab.variableColumnIndex			= 5;
	AlarmHistoryTab.valueColumnIndex			= 6;
	AlarmHistoryTab.unitsColumnIndex			= 7;
	AlarmHistoryTab.alarmStateColumnIndex		= 8;
	AlarmHistoryTab.priorityColumnIndex			= 9;
	AlarmHistoryTab.actionColumnIndex			= 10;
	AlarmHistoryTab.userColumnIndex				= 11;
	AlarmHistoryTab.commentsColumnIndex			= 12;
	AlarmHistoryTab.commentUserNameColumnIndex	= 13;
	AlarmHistoryTab.commentDateTimeColumnIndex	= 14;
	AlarmHistoryTab.alarmOrTagGuidColumnIndex		= 15;
	AlarmHistoryTab.alarmAndEventRecordGuidColumnIndex = 16;

	return cols;
}

AlarmHistoryTab.SetPageDataInAlarmModel = function (data) {
	var json = jQuery.parseJSON(data);
	var model = AlarmHistoryTab.GetAlarmModel();
	model.AlarmHistories = json.data;
	AlarmHistoryTab.SetAlarmModel(model);
	return data; // return JSON string
}

AlarmHistoryTab.OrderColumns = function ()
{
	var colOrder = [[0, "desc"]];
	return colOrder;
}

AlarmHistoryTab.SetTdId = function (table, idStr, row, index)
{
	var newIndex = table.colReorder.transpose( index );
	var td = $(row).find('td:eq(' + newIndex + ')')[0];
	if (td) {
		td.id = idStr;
	}
}

AlarmHistoryTab.GetDateAndTime = function (alarmAndEventRecordGuid)
{
	var model = AlarmHistoryTab.GetAlarmModel();
	var alarmHist = model.AlarmHistories;

	for (var i = 0; i < alarmHist.length; i++)
	{
		var alrm = alarmHist[i];

		if (alrm.AlarmAndEventRecordGuid === alarmAndEventRecordGuid)
		{
			return alrm.DateAndTime;
		}
	}

	return null;
}

AlarmHistoryTab.GetComment = function (alarmAndEventRecordGuid)
{
	var model = AlarmHistoryTab.GetAlarmModel();
	var alarmHist = model.AlarmHistories;

	for (var i = 0; i < alarmHist.length; i++)
	{
		var alrm = alarmHist[i];

		if (alrm.AlarmAndEventRecordGuid === alarmAndEventRecordGuid)
		{
			return alrm.Comments;
		}
	}

	return null;
}


AlarmHistoryTab.UpdateCommentInfo = function (newComment, alarmAndEventRecordGuid, commentTimestamp, commentUser)
{
	var model = AlarmHistoryTab.GetAlarmModel();
	var alarmHist = model.AlarmHistories;

	for ( var i = 0; i < alarmHist.length; i++ )
	{
		var alrm = alarmHist[i];

		if ( alrm.AlarmAndEventRecordGuid === alarmAndEventRecordGuid )
		{
			alrm.Comments = newComment;
			alrm.CommentUserName = commentUser;
			alrm.CommentDateTime = commentTimestamp;
			AlarmHistoryTab.SetAlarmModel(model);

			return;
		}
	}
}

AlarmHistoryTab.SaveComment = function (inputItem)
{
	var newComment = inputItem.value;
	var alarmAndEventRecordGuid = inputItem.id.replace("EnterComments_", "");
	var timeStampTicks = inputItem.getAttribute("alarmtimestampticks");
	var cell = document.getElementById(AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentsColumnIndex) + alarmAndEventRecordGuid);
	$( cell ).find( 'label' ).text( newComment );

	AlarmHistoryTab.inEditMode = false;

	var url = $("#AlarmHistoryUpdateCommentUrl").val();
	var token = $('#AlarmHistoryTabView input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	var dateAndTime = AlarmHistoryTab.GetDateAndTime();
	PNotify.removeStack(AlarmHistoryTab.messageAttributes.stack);
	$.ajax({
		cache: false,
		type: "POST",
		//async: false,
		contentType: 'application/json; charset=UTF-8',
		dataType: "json",
		url: url,
		headers: headers,
		data: JSON.stringify({
			timeStampTicks: timeStampTicks, alarmAndEventRecordGuid: alarmAndEventRecordGuid, comment: newComment
		}),
		success: function (data)
		{
			var t = data.Item2;
			var cu = data.Item1;
			AlarmHistoryTab.UpdateCommentInfo(newComment, alarmAndEventRecordGuid, t, cu);
			var c = document.getElementById(AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentDateTimeColumnIndex) + alarmAndEventRecordGuid);
			if (c) {
				c.innerHTML = t;
			}
			c = document.getElementById(AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentUserNameColumnIndex) + alarmAndEventRecordGuid);
			if (c) {
				c.innerHTML = cu;
			}
			AlarmHistoryTab.AdjustColumns();
		},
		error: function (e)
		{
			AlarmHistoryTab.CancelComment(inputItem);
			FMErrorAndExceptionHandling.ShowError('Error saving comment.', null, AlarmHistoryTab.messageAttributes);
		}
	});


}

AlarmHistoryTab.CancelComment = function (inputItem)
{
	var alarmAndEventRecordGuid = inputItem.id.replace("EnterComments_", "");
	var cell = document.getElementById(AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentsColumnIndex) + alarmAndEventRecordGuid);
	if ( cell )
	{
		$( cell ).html(AlarmHistoryTab.originalEditValue );
	}
	AlarmHistoryTab.inEditMode = false;
}

AlarmHistoryTab.CommentEditKeyHandler = function (e)
{
	e = e || event;
	if ((e.keyCode || e.which || e.charCode || 0) === 13)
	{
		AlarmHistoryTab.SaveComment(e.target);
		return false;
	}
	else if ((e.keyCode || e.which || e.charCode || 0) === 27)
	{
		// delete the onblur event so we don't save when cancelling
		$( e.target ).removeAttr( 'onblur' );
		AlarmHistoryTab.CancelComment(e.target);
		return false;
	}
	return true;
}

AlarmHistoryTab.AdjustColumns = function()
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var table = $("#AlarmHistoryTable").DataTable();
	table.columns.adjust();
}


AlarmHistoryTab.CreateEditableComment = function (rawComment, alarmAndEventRecordGuid, alarmTestGuid, alarmTimeStampTicks)
{
	return "<Span><input alarmtimestampTicks=\"" + alarmTimeStampTicks + "\" id=\"EnterComments_" + alarmAndEventRecordGuid + "\" type=\"text\" value=\"" + rawComment + "\" class=\"alarmSummaryShowCommentIsEditable\" autocomplete=\"off\" onkeyup=\"javascript: return AlarmHistoryTab.CommentEditKeyHandler();\" onblur=\"javascript: return AlarmHistoryTab.SaveComment( this );\" /></Span>";
}

AlarmHistoryTab.AddIdToTd = function (row, data, dataIndex)
{
	var alarmAndEventRecordGuid = row.id.replace('Row_', '');
	var timestampId				= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.dateTimeColumnIndex) + alarmAndEventRecordGuid;
	var siteId					= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.siteColumnIndex) + alarmAndEventRecordGuid;
	var pointTypeId				= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.pointTypeColumnIndex) +alarmAndEventRecordGuid;
	var pointId					= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.pointColumnIndex) + alarmAndEventRecordGuid;
	var pointDescriptionId		= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.pointDescriptionColumnIndex) +alarmAndEventRecordGuid;
	var variableId				= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.variableColumnIndex) + alarmAndEventRecordGuid;
	var valueId					= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.valueColumnIndex) + alarmAndEventRecordGuid;
	var unitsId					= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.unitsColumnIndex) + alarmAndEventRecordGuid;
	var alarmStateId			= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.alarmStateColumnIndex) +alarmAndEventRecordGuid;
	var priorityId				= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.priorityColumnIndex) + alarmAndEventRecordGuid;
	var actionId				= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.actionColumnIndex) + alarmAndEventRecordGuid;
	var userId					= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.userColumnIndex) + alarmAndEventRecordGuid;
	var commentsId				= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentsColumnIndex) + alarmAndEventRecordGuid;
	var commentUserNameId	= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentUserNameColumnIndex) + alarmAndEventRecordGuid;
	var commentDateTimeId	= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentDateTimeColumnIndex) + alarmAndEventRecordGuid;
	var alarmOrTagGuidId			= AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.alarmOrTagGuidColumnIndex) + alarmAndEventRecordGuid;

	if (AlarmHistoryTab.HistoryInitialized === false)
		return;

	var table = $("#AlarmHistoryTable").DataTable();

	AlarmHistoryTab.SetTdId(table, timestampId, row, AlarmHistoryTab.dateTimeColumnIndex);
	AlarmHistoryTab.SetTdId(table, siteId, row, AlarmHistoryTab.siteColumnIndex);
	AlarmHistoryTab.SetTdId(table, pointTypeId, row, AlarmHistoryTab.pointTypeColumnIndex);
	AlarmHistoryTab.SetTdId(table, pointId, row, AlarmHistoryTab.pointColumnIndex);
	AlarmHistoryTab.SetTdId(table, pointDescriptionId, row, AlarmHistoryTab.pointDescriptionColumnIndex);
	AlarmHistoryTab.SetTdId(table, variableId, row, AlarmHistoryTab.variableColumnIndex);
	AlarmHistoryTab.SetTdId(table, valueId, row, AlarmHistoryTab.valueColumnIndex);
	AlarmHistoryTab.SetTdId(table, unitsId, row, AlarmHistoryTab.unitsColumnIndex);
	AlarmHistoryTab.SetTdId(table, alarmStateId, row, AlarmHistoryTab.alarmStateColumnIndex);
	AlarmHistoryTab.SetTdId(table, priorityId, row, AlarmHistoryTab.priorityColumnIndex);
	AlarmHistoryTab.SetTdId(table, actionId, row, AlarmHistoryTab.actionColumnIndex);
	AlarmHistoryTab.SetTdId(table, userId, row, AlarmHistoryTab.userColumnIndex);
	AlarmHistoryTab.SetTdId(table, commentsId, row, AlarmHistoryTab.commentsColumnIndex);
	AlarmHistoryTab.SetTdId(table, commentUserNameId, row, AlarmHistoryTab.commentUserNameColumnIndex);
	AlarmHistoryTab.SetTdId(table, commentDateTimeId, row, AlarmHistoryTab.commentDateTimeColumnIndex);
	AlarmHistoryTab.SetTdId(table, alarmOrTagGuidId, row, AlarmHistoryTab.alarmOrTagGuidColumnIndex);
}


AlarmHistoryTab.GetIdPrefix = function (dataIndex)
{
	var prefix = "";
	switch(dataIndex)
	{
		case AlarmHistoryTab.dateTimeColumnIndex:
			prefix = "Timestamp_";
			break;
		case AlarmHistoryTab.siteColumnIndex:
			prefix = "Site_";
			break;
		case AlarmHistoryTab.pointTypeColumnIndex:
			prefix = "PointType_";
			break;
		case AlarmHistoryTab.pointColumnIndex:
			prefix = "Point_";
			break;
		case AlarmHistoryTab.pointDescriptionColumnIndex:
			prefix = "PointDescription_";
			break;
		case AlarmHistoryTab.variableColumnIndex:
			prefix = "Variable_";
			break;
		case AlarmHistoryTab.valueColumnIndex:
			prefix = "Value_";
			break;
		case AlarmHistoryTab.unitsColumnIndex:
			prefix = "Units_";
			break;
		case AlarmHistoryTab.alarmStateColumnIndex:
			prefix = "AlarmState_";
			break;
		case AlarmHistoryTab.priorityColumnIndex:
			prefix = "Priority_";
			break;
		case AlarmHistoryTab.actionColumnIndex:
			prefix = "Action_";
			break;
		case AlarmHistoryTab.userColumnIndex:
			prefix = "User_";
			break;
		case AlarmHistoryTab.commentsColumnIndex:
			prefix = "Comments_";
			break;
		case AlarmHistoryTab.commentUserNameColumnIndex:
			prefix = "CommentUserName_";
			break;
		case AlarmHistoryTab.commentDateTimeColumnIndex:
			prefix = "CommentDateTime_";
			break;
		case AlarmHistoryTab.alarmOrTagGuidColumnIndex:
			prefix = "AlarmOrTagGuid_";
			break;
		case AlarmHistoryTab.commentIsEditableSpecialIndex:
			prefix = "CommentIsEditable_";
			break;
	}
	return prefix;
}

AlarmHistoryTab.AddIdToCell = function (td, cellData, rowData, row, col)
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var alarmAndEventRecordGuid = rowData.AlarmAndEventRecordGuid;
	var table = $("#AlarmHistoryTable").DataTable();
	var origColIndex = table.colReorder.transpose(col, "toOriginal");
	var idStr = AlarmHistoryTab.GetIdPrefix(origColIndex) + alarmAndEventRecordGuid;
	td.id = idStr;
}

AlarmHistoryTab.getElementsByClassName = function (node, classname) {
	if (node.getElementsByClassName) { // use native implementation if available
		return node.getElementsByClassName(classname);
	} else {
		return (function getElementsByClass(searchClass, node) {
			if (node == null)
				node = document;
			var classElements = [],
				 els = node.getElementsByTagName("*"),
				 elsLen = els.length,
				 pattern = new RegExp("(^|\\s)" + searchClass + "(\\s|$)"), i, j;

			for (i = 0, j = 0; i < elsLen; i++) {
				if (pattern.test(els[i].className)) {
					classElements[j] = els[i];
					j++;
				}
			}
			return classElements;
		})(classname, node);
	}
}

AlarmHistoryTab.PutCommentInEditMode = function () {
	if ( AlarmHistoryTab.inEditMode === false )
	{
		var commentPrefix = AlarmHistoryTab.GetIdPrefix( AlarmHistoryTab.commentsColumnIndex );
		if ( this.id.indexOf( commentPrefix ) === 0 )
		{
			var alarmAndEventGuid = this.id.replace(commentPrefix, "");
			var model = AlarmHistoryTab.GetAlarmModel();
			var alarms = model.AlarmHistories;
			for ( var i = 0; i < alarms.length; i++ )
			{
				if ( alarms[i].AlarmAndEventRecordGuid === alarmAndEventGuid )
				{
					AlarmHistoryTab.originalEditValue = $(this).clone().html();
					this.innerHTML = AlarmHistoryTab.CreateEditableComment(alarms[i].Comments, alarmAndEventGuid, alarms[i].AlarmTestGuid, alarms[i].DateAndTimeTicks);
					AlarmHistoryTab.inEditMode = true;
					AlarmHistoryTab.editModeId = this.id;
					var commentInputTag = $( this ).find( 'input' );
					commentInputTag.focus();
					return;
				}
			}
		}
	}
}

AlarmHistoryTab.reinitializehistorydisplay = function ()
{
   var activeTab = FMOperateIndex.GetActiveTab("alarmHistory", AlarmHistoryTab.TabidNumber);
   if (activeTab === true) {
      clearInterval(AlarmHistoryTab.RefreshTimer);
		AlarmHistoryTab.RefreshTimer = null;
		AlarmHistoryTab.Initialize();
   }
   else if (FMOperateIndex.allScreensRestored === true)
   {
      clearInterval(AlarmHistoryTab.RefreshTimer);
      AlarmHistoryTab.RefreshTimer = null;
   }
}



AlarmHistoryTab.Init = function ()
{
   var tabIDnumber = $("#alarmhistorytabname")[0].innerText;

   AlarmHistoryTab.TabidNumber = tabIDnumber;

	$('<div id="loadingimageAlarmHistory" class="LoadingAnimation transparent"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#AlarmHistoryTableSection');

	if (FMOperateIndex.openingNewAlarmHistory === false) {
		var activeTab = FMOperateIndex.GetActiveTab("alarmHistory", AlarmHistoryTab.TabidNumber);
		if (activeTab === false && AlarmHistoryTab.RefreshTimer === null) {  // not the active window so start a timer to check every 200msec. The operate index object is not very efficient
			// at determining when a window is active and when it is not
			//AlarmHistoryTab.StartTimer();
			AlarmHistoryTab.RefreshTimer = setInterval(AlarmHistoryTab.reinitializehistorydisplay, 200);
			return;
		}
	}

	AlarmHistoryTab.Initialize();
};

AlarmHistoryTab.Initialize = function ()
{
	if (AlarmHistoryTab.HistoryInitialized === true)
		return;

	AlarmHistoryTab.HistoryInitialized = true;

	if (!$.fn.dataTable.isDataTable('#AlarmHistoryTable')) {
		console.log('No DataTable');
	} else {
		console.log('AlarmHistoryTab.Initialize : DataTable already exists');
		try {
			$("#AlarmHistoryTable").DataTable().destroy();
			AlarmHistoryTab.HistoryInitialized = false;
			setTimeout(AlarmHistoryTab.Initialize, 500);
			return;
		} catch {
			console.log('Unable to destroy pre-existing DataTable');
			AlarmHistoryTab.HistoryInitialized = false;
			setTimeout(AlarmHistoryTab.Initialize, 500);
			return;
		}
	}


	var model = AlarmHistoryTab.GetAlarmModel();
	var alarmHistoryViewStateObj = null;
	var loadImage = $("#loadingimageAlarmHistory");

    // make sure that the init timer is not running
	if (AlarmHistoryTab.RefreshTimer !== null)
	{
        clearInterval(AlarmHistoryTab.RefreshTimer);
        AlarmHistoryTab.RefreshTimer = null;
    }


	// Initialize the alarm and event type radio buttons.
	AlarmHistoryTab.InitializeAlarmAndEventType();

	$("#AlarmHistoryTable").removeClass("hidden");

	loadImage.show();

   if (model && model.ViewStateSettings && model.ViewStateSettings.JsonViewState && model.ViewStateSettings.JsonViewState.length > 0) {
      alarmHistoryViewStateObj = JSON.parse(model.ViewStateSettings.JsonViewState);
      if (alarmHistoryViewStateObj && alarmHistoryViewStateObj.Filters) {
         AlarmHistoryTab.columnFilterCollection = alarmHistoryViewStateObj.Filters;
      }
	}

   AlarmHistoryTab.InitializeDatePickers();

	var url = $('#AlarmHistoryGetDataUrl').val();
   var token = $('#AlarmHistoryTabView input[name =__RequestVerificationToken]').val();
   var headers = {};
   headers['__RequestVerificationToken'] = token;
	var cols = AlarmHistoryTab.CreateColumns();

	AlarmHistoryTab.DatatableHandle = $("#AlarmHistoryTable").DataTable({
		"columnDefs": [
			{
				"className": "col-sm-2 col-md-2 text-center alarmHistoryTableCellEx",
				"targets": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 15, 16],
				"createdCell": AlarmHistoryTab.AddIdToCell
			}, {
				"className": "col-sm-2 col-md-2 text-center alarmHistoryTableCommentCellEx",
				"targets": [12],
				"createdCell": AlarmHistoryTab.AddIdToCell
			}
		],
		"language":
		{
			"processing": "<div class='overlay'><i style=\"background-color: white !important;color: black !important;z-index:5\">" + $("#AlarmHistory_DD_Processing").val() + "</i></div>",
			"info": $("#AlarmHistory_DD_Showing").val() + " _START_ " + $("#AlarmHistory_DD_to").val() + " _END_ " + $("#AlarmHistory_DD_of").val() + " _TOTAL_ " + $("#AlarmHistory_DD_Alarms").val(),
			"lengthMenu": $("#AlarmHistory_DD_Show").val() + "  _MENU_  " + $("#AlarmHistory_DD_Alarms").val()
		},
		"processing": true,
		"serverSide": true,
		"deferLoading": 0,
		"ajax":
		{
			"url": url,
			"type": "POST",
			contentType: 'application/json; charset=UTF-8',
			dataType: 'json',
			"data": function (d) {
				d.columnFilterInfoList = AlarmHistoryTab.columnFilterCollection;
				d.originalColumnOrderIndex = AlarmHistoryTab.GetOriginalColumnOrderIndex();
				var orderDir = "desc";
				if (d.order.length > 0) {
					orderDir = d.order[0].dir;
				}
				return JSON.stringify({
					"draw": d.draw,
					"orderDir": orderDir,
					"start": d.start,
					"length": d.length,
					"columnFilterInfoList": AlarmHistoryTab.columnFilterCollection,
					"originalColumnOrderIndex": AlarmHistoryTab.GetOriginalColumnOrderIndex(),
					"recordTypeFilter": AlarmHistoryTab.recordTypeFilter
				});
			},
			"dataFilter": AlarmHistoryTab.SetPageDataInAlarmModel,
			'headers': headers,
			"error": function (xhr, error, thrown) {
				FMErrorAndExceptionHandling.ShowError(thrown, null, AlarmHistoryTab.messageAttributes);
			}
		},
		"order": AlarmHistoryTab.OrderColumns(),
		"columns": cols,
		"colReorder": {
			fixedColumnsLeft: 1
		},
		"ordering": true,
		"scrollY": "100px",
		"scrollX": true,
		"paging": true,
		"bFilter": false,
		"bInfo": true,
		"bAutoWidth": false,
		"lengthMenu": [[10, 25, 50, 100, 500], [10, 25, 50, 100, 500]],
		"createdRow": AlarmHistoryTab.AddIdToTd,
		"dom": '<"alarmhistory_top"l>rt<"alarmhistory_bottom"pi>',
		"fnDrawCallback": function (oSettings) {
			$('.alarmHistoryTableClass thead th.alarmHistoryTableCommentCellEx').removeClass('alarmHistoryTableCommentCellEx');
		}
	});
	// Must be in order to display correctly.
	$("#alarmHistoryTopCustomTypeControlDiv").appendTo("#AlarmHistoryTableSection .alarmhistory_top");
	$("#alarmHistoryTopCustomButtonDiv").appendTo("#AlarmHistoryTableSection .alarmhistory_top");
	$("#alarmHistoryTopCustomControlDiv").appendTo("#AlarmHistoryTableSection .alarmhistory_top");

   // Set the event for when the column sort.
   $("#AlarmHistoryTable").on('order.dt', AlarmHistoryTab.ColorSortColumn);

   // double click to edit a row 
   $('#AlarmHistoryTable tbody').on('dblclick', 'td', AlarmHistoryTab.PutCommentInEditMode);

   // Since the Date & Time column is the default sorted column,
   // color the entire column to indicated it is being sorted.
	var dateTimeColumn = AlarmHistoryTab.DatatableHandle.column(AlarmHistoryTab.dateTimeColumnIndex);
	if (dateTimeColumn.nodes() !== undefined) {
		dateTimeColumn.nodes().each(function (cell) {
			cell.classList.add("sortedColumnColor");
		});
	}


   $('#AlarmHistoryTable').on('draw.dt', AlarmHistoryTab.TablePageChanged);

   $('#AlarmHistoryTable').on('column-sizing.dt', function (e, settings) {
       //AlarmHistoryTab.StyleComments();
       AlarmHistoryTab.TablePageChanged();
   });

   var find = document.getElementById('alarmHistoryTabFind');
   find.oninput = AlarmHistoryTab.DoFind;

   $('.dataTables_scrollBody').on('scroll', function () {
       AlarmHistoryTab.HandleButtonEnableDisable();
   });

   $('#alarmHistoryTabFind').val(AlarmHistoryTab.CurrentFindString);


   AlarmHistoryTab.stack_bottomright_alarmhistory = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#AlarmHistoryTableSection').parent() };
   AlarmHistoryTab.messageAttributes = { addclass: 'stack-bottomright', stack: AlarmHistoryTab.stack_bottomright_alarmhistory };

   AlarmHistoryTab.HandleWindowResize();

	AlarmHistoryTab.InitializeColumnFilterDropdownCheckboxes();

   // The html helper sets always the attribute 'selected' for the options (sets to false if not selected).
   // Select2 expects the selected attribute only for those selected.
   $('#AvailableFilterDropdownId').find('option[selected=false]').removeAttr('selected');
   $('#AvailableFilterDropdownId').select2({ allowClear: true });
   $('#AvailableFilterDropdownId').on("select2:selecting", AlarmHistoryTab.HandleAvailableFilterDropdownSelectEvent);
   $('#AvailableFilterDropdownId').on("select2:unselect", AlarmHistoryTab.HandleAvailableFilterDropdownUnselectEvent);

   if (alarmHistoryViewStateObj) {
       AlarmHistoryTab.SetInitialVisibilityColumnReorder( alarmHistoryViewStateObj );
       if (alarmHistoryViewStateObj.PageLen) {
           AlarmHistoryTab.DatatableHandle.page.len(alarmHistoryViewStateObj.PageLen);
           AlarmHistoryTab.Refresh();
       }
   }
   else {
       AlarmHistoryTab.DatatableHandle.draw();
	}

   loadImage.fadeOut(500);

	window.onbeforeunload = function (e) {

		AlarmHistoryTab.SaveViewState();
		if (!$.fn.dataTable.isDataTable('#AlarmHistoryTable')) {
			console.log('No DataTable');
		} else {
			console.log('window.onbeforeunload : DataTable already exists');
			try {
				$("#AlarmHistoryTable").DataTable().destroy(true);
			} catch {
				console.log('Unable to destroy pre-existing DataTable');
			}
		}
	}
};

AlarmHistoryTab.CloseAlarmHistory = function (id) {
	var tabName = $("#alarmhistorytabname").text();

	if (tabName) {
		// Save the view state if the tab is closed.
		if (tabName === id) {
			AlarmHistoryTab.SaveViewState();
		}
	}
};

//==========================================================================
// This function will get the original column index for the column that
// is being sorted.  This is needed by the controller since it only knows
// the column indexes by their original index.
//==========================================================================
AlarmHistoryTab.GetOriginalColumnOrderIndex = function()
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return -1;
	var table = $("#AlarmHistoryTable").DataTable();
	var order = table.order();
	var selectedColumnIndex = order[0][0];

	// Get the original column index to be sorted on.
	var orderArray = table.colReorder.order();
	var originalColumnOrderIndex = orderArray[selectedColumnIndex];

	return originalColumnOrderIndex;
}

AlarmHistoryTab.ScrollYResize = function()
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var w = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
	var topDiv = document.getElementById("AlarmHistoryTable");
	if (topDiv !== null) {
		var divTop = topDiv.getBoundingClientRect().top;
		var h = w - divTop - 55;
		h = Math.round(h);
		var hString = h + 'px';
		$('#AlarmHistoryTable').dataTable().fnSettings().oScroll.sY = hString;
		$('.dataTables_scrollBody:has(#AlarmHistoryTable)').height(hString);
	}
}

//================================================================================
// This function will handle the coloring of the column that has been selected
// to be sorted.
//================================================================================
AlarmHistoryTab.ColorSortColumn = function()
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var table				= $( "#AlarmHistoryTable" ).DataTable();
	var order				= table.order();
	var selectedColumnIndex = order[0][0];
	var column				= table.column(selectedColumnIndex);

	var numberOfColumns = $( "#AlarmHistoryTable thead th" ).length;

	for (var nextColIndex = 0; nextColIndex <= numberOfColumns - 1; nextColIndex++)
	{
		var resetColumn = table.column(nextColIndex);
		resetColumn.nodes().each( function( resetCell )
		{
			resetCell.classList.remove("sortedColumnColor");
		} );
	}

	column.nodes().each( function( cell )
	{
		cell.classList.add( "sortedColumnColor" );
	} );
}

//================================================================================
// This function will handle the window resize event.
//================================================================================
AlarmHistoryTab.HandleWindowResize = function ()
{
	AlarmHistoryTab.Newdiv(true);
	AlarmHistoryTab.ScrollYResize();
	AlarmHistoryTab.HandleButtonEnableDisable();
	AlarmHistoryTab.Newdiv(true);
}

//================================================================================
// This function will initialize the alarm and event type radio button to
// Alarm type.
//================================================================================
AlarmHistoryTab.InitializeAlarmAndEventType = function ()
{
	var performRefresh = false;
	var elementId = "AlarmRadioBtn";
	AlarmHistoryTab.HandleColumnTypeFilterRadioChangeHelper(elementId, performRefresh);
	AlarmHistoryTab.HandleColumnTypeFilterDropdownExpandCollapse();
}

//=========================================================================
// This function will handle the alarm/event type radio button
// on change event.
//=========================================================================
AlarmHistoryTab.HandleColumnTypeFilterRadioChange = function (elementId)
{
	var performRefresh = true;
	AlarmHistoryTab.HandleColumnTypeFilterRadioChangeHelper(elementId, performRefresh);
}

//===================================================================
// This function will perform the radio button setting functionality.
//===================================================================
AlarmHistoryTab.HandleColumnTypeFilterRadioChangeHelper = function (elementId, performRefresh)
{
	var alarmElement = document.getElementById("AlarmRadioBtn");
	var eventElement = document.getElementById("EventRadioBtn");
	var alarmEventElement = document.getElementById("AlarmAndEventRadioBtn");
	var showLabelElement = document.getElementById("showHideColumnsTypeLabel");

	if (alarmElement == null || eventElement == null || alarmEventElement == null || showLabelElement == null)
	{
		return;
	}

	alarmElement.checked = false;
	eventElement.checked = false;
	alarmEventElement.checked = false;

	if (elementId === "AlarmRadioBtn")
	{
		alarmElement.checked = true;
		showLabelElement.innerHTML = $("#AlarmsDictionary").val(); // "Alarms"
		AlarmHistoryTab.recordTypeFilter = AlarmHistoryTab.alarmRecordType;
	}

	if (elementId === "EventRadioBtn")
	{
		eventElement.checked = true;
		showLabelElement.innerHTML = $("#EventsDictionary").val(); // "Events"
		AlarmHistoryTab.recordTypeFilter = AlarmHistoryTab.eventRecordType;
	}

	if (elementId === "AlarmAndEventRadioBtn")
	{
		alarmEventElement.checked = true;
		showLabelElement.innerHTML = $("#AlarmsEventsDictionary").val(); // "Alarms & Events"
		AlarmHistoryTab.recordTypeFilter = AlarmHistoryTab.alarmEventRecordType;
	}

	AlarmHistoryTab.HandleColumnTypeFilterDropdownExpandCollapse();

	if (performRefresh)
	{
		AlarmHistoryTab.HandleColumnFilterRefresh();
	}
}

//================================================================================
// This function will handle the column filter checkbox being checked/unchecked
// event.
//================================================================================
AlarmHistoryTab.HandleColumnFilterCheckboxChange = function (currentItem)
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var table = $("#AlarmHistoryTable").DataTable();
	var checked = $(currentItem).is(":checked");
	var inputId = $(currentItem).attr("id");

	if (typeof (checked) != "undefined" && typeof (inputId) != "undefined")
	{
		if (inputId === "DateTimeCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.dateTimeColumnIndex);
		}

		if (inputId === "SiteCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.siteColumnIndex);
		}

		if (inputId === "PointTypeCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.pointTypeColumnIndex);
		}

		if (inputId === "PointCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.pointColumnIndex);
		}

		if (inputId === "PointDescriptionCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.pointDescriptionColumnIndex);
		}

		if (inputId === "VariableCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.variableColumnIndex);
		}

		if (inputId === "ValueCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.valueColumnIndex);
		}

		if (inputId === "UnitsCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.unitsColumnIndex);
		}

		if (inputId === "AlarmStateCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.alarmStateColumnIndex);
		}

		if (inputId === "PriorityCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.priorityColumnIndex);
		}

		if (inputId === "ActionCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.actionColumnIndex);
		}

		if (inputId === "UserCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.userColumnIndex);
		}

		if (inputId === "CommentsCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.commentsColumnIndex);
			$('.alarmHistoryTableClass thead th.alarmHistoryTableCommentCellEx').removeClass('alarmHistoryTableCommentCellEx');
		}

		if (inputId === "CommentUserNameCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.commentUserNameColumnIndex);
		}

		if (inputId === "CommentDateTimeCheckbox")
		{
			AlarmHistoryTab.ToggleColumnVisibility(table, checked, AlarmHistoryTab.commentDateTimeColumnIndex);
		}
	}
}

AlarmHistoryTab.VisibilityColumnReorder = function (table)
{
	var orderArray = table.colReorder.order();
	var visibleArr = [];
	var invisibleArr = [];

	for ( var i = 0; i < orderArray.length; i++ )
	{
		var currentColIndex = table.colReorder.transpose( orderArray[i] );
		var column = table.column(currentColIndex);
		if ( column.visible() )
		{
			visibleArr.push( orderArray[i] );
		}
		else
		{
			invisibleArr.push( orderArray[i] );
		}
	}

	var newOrderArray = visibleArr;
	for (var nextIndex = 0; nextIndex < AlarmHistoryTab.checkboxFilterArray.length; nextIndex++) {
		var control = AlarmHistoryTab.checkboxFilterArray[nextIndex];
		var currentColIndex = table.colReorder.transpose(control.columnIndex);
		var column = table.column(currentColIndex);
		if (control.checked && !visibleArr.includes(control.columnIndex)) {
			newOrderArray.push(control.columnIndex);
			const index = invisibleArr.indexOf(control.columnIndex);
			if (index > -1) { // only splice array when item is found
				invisibleArr.splice(index, 1); // 2nd parameter means remove one item only
			}
			column.visible(true, false);
		} else if (!control.checked && !invisibleArr.includes(control.columnIndex)) {
			invisibleArr.push(control.columnIndex);
			const index = visibleArr.indexOf(control.columnIndex);
			if (index > -1) { // only splice array when item is found
				visibleArr.splice(index, 1); // 2nd parameter means remove one item only
			}
			column.visible(false, false);
		}
	}

	for ( var j = 0; j < invisibleArr.length; j++ )
	{
		newOrderArray.push( invisibleArr[j] );
	}

	table.colReorder.order(newOrderArray, true);
}

//================================================================================
// This function will toggle the column visibility.
//================================================================================
AlarmHistoryTab.ToggleColumnVisibility = function(table, checked, originalColumnIndex)
{
	//AlarmHistoryTab.DoFindLogic();
	var checkboxControl = new Object();
	checkboxControl.checked = checked;
	checkboxControl.columnIndex = originalColumnIndex;
	AlarmHistoryTab.checkboxFilterArray.push(checkboxControl);
}

//================================================================================
// This function will handle the column filter dropdown on click event. It will
// expand or collapse the dropdown based on the "hidden" class state.
//================================================================================
AlarmHistoryTab.HandleColumnFilterDropdownExpandCollapse = function ()
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var hiddenClass = $("#ColumnFilterDiv").attr('class');
	var table = $("#AlarmHistoryTable").DataTable();

	if (hiddenClass === "")
	{
		$("#ColumnFilterDiv").addClass('hidden');

		AlarmHistoryTab.VisibilityColumnReorder(table);
		table.columns.adjust().draw(false);
		AlarmHistoryTab.ResizeColumns();
		AlarmHistoryTab.checkboxFilterArray = [];
	}
	else
	{
		$("#ColumnFilterDiv").removeClass('hidden');
		AlarmHistoryTab.checkboxFilterArray = [];
	}
}

//================================================================================
// This function will handle the column filter dropdown on click event. It will
// expand or collapse the dropdown based on the "hidden" class state.
//================================================================================
AlarmHistoryTab.HandleColumnTypeFilterDropdownExpandCollapse = function ()
{
	var hiddenClass = $("#ColumnTypeFilterDiv").attr('class');

	if (hiddenClass === "") {
		$("#ColumnTypeFilterDiv").addClass('hidden');
	}
	else {
		$("#ColumnTypeFilterDiv").removeClass('hidden');
	}
}

//================================================================================
// This function will initialize all the column filter checkboxes to a checked
// state.  It is called by the AlarmHistoryTabView.cshtml document ready event.
//================================================================================
AlarmHistoryTab.InitializeColumnFilterDropdownCheckboxes = function()
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var table = $("#AlarmHistoryTable").DataTable();
	var numberOfColumns = $("#AlarmHistoryTable thead th").length;

	for ( var nextColIndex = 0; nextColIndex <= numberOfColumns - 1; nextColIndex++ )
	{
		AlarmHistoryTab.ToggleColumnVisibility(table, false, nextColIndex);
	}

	$('#ColumnFilterUl > li > label > input').each(function ()
	{
		var inputId = $(this).attr("id");

		if ( typeof ( inputId ) != "undefined" )
		{
			if (inputId === "DateTimeCheckbox")
			{
				$(this).attr('checked', 'checked');
				AlarmHistoryTab.ToggleColumnVisibility(table, true, AlarmHistoryTab.dateTimeColumnIndex);
			}

			if (inputId === "SiteCheckbox")
			{
				$(this).attr('checked', 'checked');
				AlarmHistoryTab.ToggleColumnVisibility(table, true, AlarmHistoryTab.siteColumnIndex);
			}

			if (inputId === "PointTypeCheckbox")
			{
				$(this).attr('checked', 'checked');
				AlarmHistoryTab.ToggleColumnVisibility(table, true, AlarmHistoryTab.pointTypeColumnIndex);
			}

			if (inputId === "PointCheckbox")
			{
				$(this).attr('checked', 'checked');
				AlarmHistoryTab.ToggleColumnVisibility(table, true, AlarmHistoryTab.pointColumnIndex);
			}

			if (inputId === "PointDescriptionCheckbox")
			{
				$(this).attr('checked', 'checked');
				AlarmHistoryTab.ToggleColumnVisibility(table, true, AlarmHistoryTab.pointDescriptionColumnIndex);
			}
		}
	});
}

AlarmHistoryTab.StyleComments = function ()
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var table = $("#AlarmHistoryTable").DataTable();
	var commentsColIndex = table.colReorder.transpose(AlarmHistoryTab.commentsColumnIndex);
	var commentsCol = table.column( commentsColIndex );
	//if comments column is visible 
	if (commentsCol && commentsCol.visible())
	{
		var c = table.cells(null, commentsColIndex);
		c.every( function()
		{
			var id = this.node().id;
			if ( AlarmHistoryTab.inEditMode === false || AlarmHistoryTab.editModeId !== id )
			{
				var n = $(this.node());
				if ( n && n.html() !== "" )
				{
					var alarmAndEventRecordGuid = id.replace(AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentsColumnIndex), "");
					var innerHtmlId = AlarmHistoryTab.GetIdPrefix( AlarmHistoryTab.commentIsEditableSpecialIndex ) + alarmAndEventRecordGuid;
					n.html("<label class=\"alarmSummaryShowCommentIsEditable\" id=\"" + innerHtmlId + "\">" + n.html() + "</label>");
					var rawComment = AlarmHistoryTab.GetComment(alarmAndEventRecordGuid);
					document.getElementById(innerHtmlId).setAttribute('title', rawComment);
				}
			}
		});
	}
}

//===================================================================
// This function handles the draw event. It is fired once the table
// has completed a draw.
//===================================================================
AlarmHistoryTab.TablePageChanged = function()
{
	AlarmHistoryTab.DoFindLogic();

	// Ensure that when the page has completed a draw to reset
	// the highlighted sort column.
	AlarmHistoryTab.ColorSortColumn();

	// Hide the column filter dropdown on table draw complete.
	$("#ColumnFilterDiv").addClass('hidden');

//Don't need the below line because it is in AlarmHistoryTab.DoFindLogic
	//AlarmHistoryTab.StyleComments();
}

AlarmHistoryTab.Newdiv = function (force)
{
	var newDiv = $("#alarmHistoryFindResultsRow");
	if (AlarmHistoryTab.FindArr && AlarmHistoryTab.FindArr.length > 0) {
			if ($(newDiv).is(':hidden') || force) {
				$(newDiv).removeClass('hidden');
			}
		}
	else
	{
		$(newDiv).removeClass('hidden').addClass('hidden');
	}
	
}


AlarmHistoryTab.HandleButtonEnableDisable = function () {
	var selectedRows = AlarmHistoryTab.GetCurrentlySelectedViewableAlarms();
	var numSelectedRows = selectedRows.length;
	var alarmGraphicButton = document.getElementById("alarmHistoryGraphicButton");
	var helpButton = document.getElementById("alarmHistoryHelpButton");
	var detailsButton = document.getElementById("alarmHistoryDetailsButton");

	if (numSelectedRows === 1)
	{
		if (alarmGraphicButton.disabled)
		{
			alarmGraphicButton.disabled = false;
			alarmGraphicButton.classList.remove("alarmHistoryDisableButtonClass");
			helpButton.disabled = false;
			helpButton.classList.remove("alarmHistoryDisableButtonClass");
			detailsButton.disabled = false;
			detailsButton.classList.remove("alarmHistoryDisableButtonClass");
		}
	}
	else
	{
		if (!alarmGraphicButton.disabled)
		{
			alarmGraphicButton.disabled = true;
			alarmGraphicButton.classList.add("alarmHistoryDisableButtonClass");
			helpButton.disabled = true;
			helpButton.classList.add("alarmHistoryDisableButtonClass");
			detailsButton.disabled = true;
			detailsButton.classList.add("alarmHistoryDisableButtonClass");
		}
	}
}

//=======================================================================================
// This function will refresh the data table based on the filters.
//=======================================================================================
AlarmHistoryTab.Refresh = function()
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var table = $("#AlarmHistoryTable").dataTable();
	table.fnPageChange( 0 );
}

//=======================================================================================
// This function will export the current page of the data table to a CSV file for download.
//=======================================================================================
AlarmHistoryTab.ExportCSV = function () {
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var table = $("#AlarmHistoryTable").dataTable();
	var textToParse = table[0].innerText;

	textToParse = textToParse.replace(/\n\t\n/g, ',')
	textToParse = textToParse.replace(/\t/g, ',')
	textToParse = textToParse.replace(/\n/, '')

	var blob = new Blob([textToParse], { type: 'text/html;charset=UTF-8' });
	var url = URL.createObjectURL(blob);

	var pom = document.createElement('a');
	pom.href = url;
	pom.setAttribute('download', 'export.csv');
	pom.click();
}

//=========================================================================================
// This function will handle the Refresh button refresh event. It will set the
// To date to the current date time.
//=========================================================================================
AlarmHistoryTab.HandleColumnFilterRefresh = function()
{
	var numFormatInfoString = $("#NumberFormatInfoString").val();
	var numFormatInfo = JSON.parse(numFormatInfoString);
	var timezoneOffsetStr = $("#TimezoneOffsetString").val();
	var timezoneOffset = parseFloat(timezoneOffsetStr) / 60.0;
	var currentToDate = ConvertToSiteTimezone(new Date(), timezoneOffset);

	var dateTimeFormatStr = numFormatInfo.ShortDatePattern.toUpperCase() + " " + ConvertToMomentUITimeFormat(numFormatInfo.TimePattern);
	var momentToStr = GetMomentDateTimeFormattedStr(currentToDate);
	$('#ColumnFilterToDateInput').datetimepicker("setDate", moment.utc(momentToStr).format(dateTimeFormatStr));

	AlarmHistoryTab.HandleAvailableFilterDateChangeEvent("TO");

	AlarmHistoryTab.previousColumnFilterCollection = AlarmHistoryTab.CopyColumnFilterInfo(AlarmHistoryTab.columnFilterCollection);
	AlarmHistoryTab.Refresh();
}

//=========================================================================================
// This function will handle the column filter modal OK button refresh.  It will save
// the current filters which will be used when the user cancel filtering to reset the
// filters.
//=========================================================================================
AlarmHistoryTab.HandleSaveCurrentFilterAndRefresh = function()
{
	AlarmHistoryTab.previousColumnFilterCollection = AlarmHistoryTab.CopyColumnFilterInfo(AlarmHistoryTab.columnFilterCollection);
	AlarmHistoryTab.Refresh();
}

//===========================================================================================
// This function will handle the column filter modal cancel. It will reset the column
// filter collection back what it was previously.
//===========================================================================================
AlarmHistoryTab.HandleCancelFiltering = function()
{
	if ( AlarmHistoryTab.previousColumnFilterCollection != null )
	{
		AlarmHistoryTab.columnFilterCollection = AlarmHistoryTab.CopyColumnFilterInfo(AlarmHistoryTab.previousColumnFilterCollection);

		for ( var nextFilterIndex = 0; nextFilterIndex < AlarmHistoryTab.columnFilterCollection.length; nextFilterIndex++ )
		{
			var filterObj = AlarmHistoryTab.columnFilterCollection[nextFilterIndex];

			if ( filterObj.Index === AlarmHistoryTab.dateTimeColumnIndex )
			{
				$('#ColumnFilterFromDateInput').val(filterObj.FromDateStr);
				$('#ColumnFilterToDateInput').val(filterObj.ToDateStr);
			}

			if (filterObj.Index === AlarmHistoryTab.commentDateTimeColumnIndex)
			{
				$('#ColumnFilterCommentFromDateInput').val(filterObj.FromDateStr);
				$('#ColumnFilterCommentToDateInput').val(filterObj.ToDateStr);
			}
		}
	}
}

//=============================================================================================
// This function will copy the filter collection to a new object and return it.
//=============================================================================================
AlarmHistoryTab.CopyColumnFilterInfo = function(fromFilterCollection)
{
	var toFilterCollection = [];

	for ( var nextFilterIndex = 0; nextFilterIndex < fromFilterCollection.length; nextFilterIndex++ )
	{
		var toFilter	= AlarmHistoryTab.CreateAvailableFilterObject();
		var filterObj	= fromFilterCollection[nextFilterIndex];

		toFilter.Name				= filterObj.Name;
		toFilter.Index				= filterObj.Index;
		toFilter.FromDateStr		= filterObj.FromDateStr;
		toFilter.ToDateStr			= filterObj.ToDateStr;
		toFilter.CommentFromDateStr = filterObj.CommentFromDateStr;
		toFilter.CommentToDateStr	= filterObj.CommentToDateStr;

		for ( var nextInfoIndex = 0; nextInfoIndex < filterObj.FilterCollection.length; nextInfoIndex++ )
		{
			toFilter.FilterCollection.push( filterObj.FilterCollection[nextInfoIndex] );
		}

		toFilterCollection.push( toFilter );
	}

	return toFilterCollection;
}

//==========================================================================================
// This function will handle the right click on the table header.
//==========================================================================================
AlarmHistoryTab.HandleTableHeaderOnContextMenuEvent = function(columnIndex)
{
	$("#ColumnDataFilterModalDiv").modal('show');

	// Create column data filter dropdown options.
	AlarmHistoryTab.CreateChooseColumnDropdownEntries(columnIndex);
}

//================================================================================================
// This function will create the entries in the Column Data Filter dropdown. It will have
// the original column index in the value field.
//================================================================================================
AlarmHistoryTab.CreateChooseColumnDropdownEntries = function (previousSelection)
{
	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var table = $("#AlarmHistoryTable").DataTable();
	var numberOfColumns = 15;

	var columnDataFilterSelectControl = document.getElementById("ChooseColumnDropdownId");
	var selectOptLength = columnDataFilterSelectControl.length - 1;

	// Clear the dropdown list with the exception of the None entry.
	for (var nextOptIndex = selectOptLength; nextOptIndex > 0; nextOptIndex--)
	{
		columnDataFilterSelectControl.remove( nextOptIndex );
	}

	// Only add the column names that are visible and reset the previous selection.
	for (var nextColumnIndex = 0; nextColumnIndex < numberOfColumns; nextColumnIndex++)
	{
		if (table.column(nextColumnIndex).visible())
		{
			var columnName = table.column(nextColumnIndex).header().innerText;
			var currentColIndex = table.column(nextColumnIndex).index();

			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.dateTimeColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.siteColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.pointTypeColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.pointColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.pointDescriptionColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.variableColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.valueColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.unitsColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.alarmStateColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.priorityColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.actionColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.userColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.commentsColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.commentUserNameColumnIndex);
			AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, AlarmHistoryTab.commentDateTimeColumnIndex);
		}
	}

	// Add and/or remove available filter from the collection based on the visible columns.
	AlarmHistoryTab.AddRemoveAvailableFilterFromCollection();
	AlarmHistoryTab.HandleChooseColumnDropdownEvent();
}

//================================================================================================
// This function is a helper for the create the entries in the Column Data Filter dropdown.
//================================================================================================
AlarmHistoryTab.CreateChooseColumnDropdownEntriesHelper = function (columnDataFilterSelectControl,
																	table,
																	columnName,
																	currentColIndex,
																	previousSelection,
																	originalColumnIndex)
{
	var transposedColIndex = table.colReorder.transpose(originalColumnIndex);

	if (transposedColIndex === currentColIndex)
	{
		var optionElement		= document.createElement("option");
		optionElement.innerHTML = columnName;
		optionElement.value		= originalColumnIndex;
		columnDataFilterSelectControl.appendChild(optionElement);

		if (previousSelection === originalColumnIndex)
		{
			$("#ChooseColumnDropdownId").val(originalColumnIndex.toString());
		}
	}
}

//==========================================================================================
// This function will initial the date time column filter with the initial date.  It is
// call by the AlarmHistoryTabView.cshtml.
//==========================================================================================
AlarmHistoryTab.SetInitialDateFilters = function()
{
	if ( AlarmHistoryTab.columnFilterCollection == null || AlarmHistoryTab.columnFilterCollection.length === 0 )
	{
		AlarmHistoryTab.columnFilterCollection = [];

		var columnFilterObj		= AlarmHistoryTab.CreateAvailableFilterObject();
		columnFilterObj.Index	= AlarmHistoryTab.dateTimeColumnIndex;
		columnFilterObj.Name	= "TimeStamp";

		AlarmHistoryTab.columnFilterCollection.push(columnFilterObj);

		// Need the initial site filter with an empty filter collection.
		// The initial site filter collection will be populated in the AandEArchiveDatabase file.
		columnFilterObj			= AlarmHistoryTab.CreateAvailableFilterObject();
		columnFilterObj.Index	= AlarmHistoryTab.siteColumnIndex;
		columnFilterObj.Name	= "Site";

		AlarmHistoryTab.columnFilterCollection.push(columnFilterObj);
	}

	AlarmHistoryTab.HandleAvailableFilterDateChangeEvent("FROM");
	AlarmHistoryTab.HandleAvailableFilterDateChangeEvent("TO");

	// Persist the column filter collection settings for when the user makes changes
	// and cancels the changes.
	AlarmHistoryTab.previousColumnFilterCollection = AlarmHistoryTab.CopyColumnFilterInfo(AlarmHistoryTab.columnFilterCollection);
}

//===============================================================================================
// This function will add and/or remove availabe filter objects from the collection.  It is based
// on the visible columns which are loaded in the column filter modal dialog.
//===============================================================================================
AlarmHistoryTab.AddRemoveAvailableFilterFromCollection = function()
{
	if ( AlarmHistoryTab.columnFilterCollection == null || AlarmHistoryTab.columnFilterCollection.length === 0 )
	{
		AlarmHistoryTab.columnFilterCollection = [];

		$("#ChooseColumnDropdownId > option").each(function ()
		{
			var name		= $( this ).text();
			var index		= $( this ).val();
			var columnIndex = parseInt( index );

			if ( columnIndex !== -99 )
			{
				var columnFilterObj		= AlarmHistoryTab.CreateAvailableFilterObject();
				columnFilterObj.Name	= name;
				columnFilterObj.Index	= columnIndex;

				AlarmHistoryTab.columnFilterCollection.push( columnFilterObj );
			}
		} );
	}
	else
	{
		var found;
		var nextFilterIndex;
		var selectionIndexList = [];

		// Add new column filter to collection.
		$("#ChooseColumnDropdownId > option").each(function ()
		{
			var name		= $(this).text();
			var index		= $(this).val();
			var columnIndex = parseInt(index);

			if (columnIndex !== -99)
			{
				selectionIndexList.push(columnIndex);
				found = false;

				for ( nextFilterIndex = 0; nextFilterIndex < AlarmHistoryTab.columnFilterCollection.length; nextFilterIndex++ )
				{
					if (AlarmHistoryTab.columnFilterCollection[nextFilterIndex].Index === columnIndex)
					{
						found = true;
					}
				}

				if ( found === false )
				{
					var columnFilterObj		= AlarmHistoryTab.CreateAvailableFilterObject();
					columnFilterObj.Name	= name;
					columnFilterObj.Index	= columnIndex;

					AlarmHistoryTab.columnFilterCollection.push(columnFilterObj);
				}
			}
		});

		var startIndex = AlarmHistoryTab.columnFilterCollection.length - 1;

		for (nextFilterIndex = startIndex; nextFilterIndex >= 0; nextFilterIndex--)
		{
			found = false;

			for (var nextSelectionIndex = 0; nextSelectionIndex < selectionIndexList.length; nextSelectionIndex++)
			{
				if (AlarmHistoryTab.columnFilterCollection[nextFilterIndex].Index === selectionIndexList[nextSelectionIndex])
				{
					found = true;
				}
			}

			if ( found === false )
			{
				AlarmHistoryTab.columnFilterCollection.splice(nextFilterIndex, 1);
			}
		}
	}
}

//=====================================================================================
// This function will handle the Choose Columns (column filter) dropdown change event.
//=====================================================================================
AlarmHistoryTab.HandleChooseColumnDropdownEvent = function()
{
	// Get the column selected.
	var selectedColumn = $("#ChooseColumnDropdownId").find(":selected").val();
	var selectedColumnInt = parseInt( selectedColumn );

	// For the date & time column, present a date range date/time picker
	// instead of the multi-select column data dropdown.
	if ( selectedColumnInt === AlarmHistoryTab.dateTimeColumnIndex )
	{
		$("#ColumnFilterDateRowDiv").removeClass( "hidden" );
		$("#ColumnFilterCommentDateRowDiv").addClass("hidden");
		$("#AvailableFilterDropdownLabelDivId").addClass("hidden");
		$("#AvailableFilterDropdownDivId").addClass( "hidden" );
	}
	else if (selectedColumnInt === AlarmHistoryTab.commentDateTimeColumnIndex)
	{
		$("#ColumnFilterCommentDateRowDiv").removeClass("hidden");
		$("#ColumnFilterDateRowDiv").addClass("hidden");
		$("#AvailableFilterDropdownLabelDivId").addClass("hidden");
		$("#AvailableFilterDropdownDivId").addClass("hidden");
	}
	else
	{
		$("#ColumnFilterDateRowDiv").addClass("hidden");
		$("#ColumnFilterCommentDateRowDiv").addClass("hidden");
		$("#AvailableFilterDropdownLabelDivId").removeClass("hidden");
		$("#AvailableFilterDropdownDivId").removeClass("hidden");

		AlarmHistoryTab.ClearSelectionInAvailableFiltersDropdown();
		AlarmHistoryTab.CreateColumnAvailableFilterDropdown();

		AlarmHistoryTab.ResetSelectedAvailableFilters(selectedColumnInt);
	}
}

//==========================================================================
// This function will clear the selected items from the column filter
// data dropdown. It is called when the column filter has changed.
//==========================================================================
AlarmHistoryTab.ClearSelectionInAvailableFiltersDropdown = function()
{
	// Must have this line or the empty() will not work.
	$("#AvailableFilterDropdownId").select2('val');
	$("#AvailableFilterDropdownId").empty();
}

//=================================================================================
// This function will reset the available filter selection.
//=================================================================================
AlarmHistoryTab.ResetSelectedAvailableFilters = function(selectedColumnIndex)
{
	for ( var nextColumn = 0; nextColumn < AlarmHistoryTab.columnFilterCollection.length; nextColumn++ )
	{
		var columnObj = AlarmHistoryTab.columnFilterCollection[nextColumn];

		if ( columnObj.Index === selectedColumnIndex )
		{
			var selectedValues = [];
			for ( var nextFilter = 0; nextFilter < columnObj.FilterCollection.length; nextFilter++ )
			{
				var filter = columnObj.FilterCollection[nextFilter];
				selectedValues.push( filter );
			}

			if ( selectedValues.length > 0 )
			{
				$('#AvailableFilterDropdownId').val(selectedValues).trigger('change.select2');
			}
		}
	}
}

//=============================================================================
// This function creates the available filter object that contains the filters
// for each of the selected columns.
//=============================================================================
AlarmHistoryTab.CreateAvailableFilterObject = function()
{
	var columnFilterObject						= new Object();
	columnFilterObject.Name						= "";
	columnFilterObject.Index					= -99;
	columnFilterObject.FilterCollection			= [];
	columnFilterObject.FromDateStr				= "";
	columnFilterObject.ToDateStr				= "";
	columnFilterObject.CommentFromDateStr		= "";
	columnFilterObject.CommentToDateStr			= "";

	return columnFilterObject;
}

//===================================================================================
// This function will retrieve available filter data for a selected column. It passes
// the selected column index and column filter information to the server.
//===================================================================================
AlarmHistoryTab.RetrieveAvailableFilters = function(selectedColumnIndex)
{
	var filterDataList = null;
	var getColumnFilterDataUrl = $("#AlarmHistoryGetColumnFilterDataUrl").val();
	var token = $('#AlarmHistoryTabView input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	PNotify.removeStack(AlarmHistoryTab.messageAttributes.stack);

	$.ajax( {
		cache: false,
		type: "POST",
		async: false,
		contentType: 'application/json; charset=UTF-8',
		dataType: "json",
		url: getColumnFilterDataUrl,
		headers: headers,
		data: JSON.stringify( {selectedColumn: selectedColumnIndex, filterInfo: AlarmHistoryTab.columnFilterCollection} ),
		success: function( dataList )
		{
			filterDataList = dataList;
		},
		error: function( e )
		{
			FMErrorAndExceptionHandling.ShowError('Error retrieving column filter data.', null, AlarmHistoryTab.messageAttributes);
		}
	});

	return filterDataList;
}

//===========================================================================================
// This function will load the available filter with available data based on the
// column selected.
//===========================================================================================
AlarmHistoryTab.CreateColumnAvailableFilterDropdown = function()
{
	// Get the column selected.
	var selectedColumn = $("#ChooseColumnDropdownId").find(":selected").val();
	var columnAvailableFilterControl = document.getElementById("AvailableFilterDropdownId");
	var selectedColumnInt = parseInt( selectedColumn );

	if ( selectedColumn === "-99" )
	{
		AlarmHistoryTab.ClearAvailableFilterDropdownEntries();
		return;
	}

	// Retrieve filter data from server.
	var filterDataList = AlarmHistoryTab.RetrieveAvailableFilters(selectedColumnInt);

	// Load dropdown selections.
	if ( filterDataList != null && filterDataList.length > 0 )
	{
		AlarmHistoryTab.ClearAvailableFilterDropdownEntries();

		for (var nextItem = 0; nextItem < filterDataList.length; nextItem++)
		{
			var filterData = filterDataList[nextItem];
			AlarmHistoryTab.CreateColumnAvailableFilterDropdownHelper(columnAvailableFilterControl, filterData);
		}
	}
}

//===========================================================================================
// This function is a helper for the create available filter dropdown. It will create the
// option items.
//===========================================================================================
AlarmHistoryTab.CreateColumnAvailableFilterDropdownHelper = function(columnAvailableFilterControl, filterData)
{
	var displayValue = filterData;
	var actualValue = filterData;

	// For level that are in feet/inches, the display value is different from the actual
	// value.
	if ( filterData.indexOf( "LV|" ) >= 0)
	{
		var parts = filterData.split("|");

		if ( parts.length === 3 )
		{
			displayValue = parts[1];
			actualValue = parts[2];
		}
	}

	// For Units, the display value is different from the actual value.
	if (filterData.indexOf("UN|") >= 0)
	{
		parts = filterData.split("|");

		if (parts.length === 3)
		{
			displayValue = parts[1];
			actualValue = parts[2];
		}
	}

	var optionElement = document.createElement("option");
	optionElement.innerHTML = displayValue;
	optionElement.value = actualValue;

	columnAvailableFilterControl.appendChild( optionElement );
}

//==============================================================================
// This function will clear all the option entries in the available filters
// select.
//==============================================================================
AlarmHistoryTab.ClearAvailableFilterDropdownEntries = function()
{
	$("#AvailableFilterDropdownId > option").each(function ()
	{
		$( this ).remove();
	});
}

//================================================================================
// This function will handle the clear all filters button event.
//================================================================================
AlarmHistoryTab.HandleClearAllFiltersBtnEvent = function()
{
	if ( AlarmHistoryTab.columnFilterCollection != null )
	{
		for ( var nextFilterIndex = 0; nextFilterIndex < AlarmHistoryTab.columnFilterCollection.length; nextFilterIndex++ )
		{
			var columnFilterObj = AlarmHistoryTab.columnFilterCollection[nextFilterIndex];
			columnFilterObj.CommentFromDateStr = "";
			columnFilterObj.CommentToDateStr = "";
			columnFilterObj.FilterCollection = [];
		}

		// This will clear the selected filters in view.
		var selectedValues = [];
		$('#AvailableFilterDropdownId').val(selectedValues).trigger('change.select2');

		$( "#ColumnFilterCommentFromDateInput" ).val("");
		$("#ColumnFilterCommentToDateInput").val("");
	}
}

//==================================================================================
// This function handles the available filter event when an item is selected.
// It will add the filter item to the appropriate column filter list.
//==================================================================================
AlarmHistoryTab.HandleAvailableFilterDropdownSelectEvent = function(evnt)
{
	// Get the column selected.
	var selectedColumn = $("#ChooseColumnDropdownId").find(":selected").val();
	var selectedColumnInt = parseInt(selectedColumn);

	if (selectedColumn === "-99")
	{
		return;
	}

	var selectedFilter = evnt.params.args.data.id;

	for (var next = 0; next < AlarmHistoryTab.columnFilterCollection.length; next++)
	{
		var columnObj = AlarmHistoryTab.columnFilterCollection[next];

		if (columnObj.Index === selectedColumnInt)
		{
			var found = false;
			for (var nextFilter = 0; nextFilter < columnObj.FilterCollection.length; nextFilter++)
			{
				if (selectedFilter === columnObj.FilterCollection[nextFilter])
				{
					found = true;
				}
			}

			if (found === false)
			{
				columnObj.FilterCollection.push(selectedFilter);
				break;
			}
		}
	}
}

//==================================================================================
// This function handles the available filter event when an item is selected.
// It will add the filter item to the appropriate column filter list.
//==================================================================================
AlarmHistoryTab.HandleAvailableFilterDropdownUnselectEvent = function (evnt)
{
	// Get the column selected.
	var selectedColumn = $("#ChooseColumnDropdownId").find(":selected").val();
	var selectedColumnInt = parseInt(selectedColumn);

	if (selectedColumn === "-99")
	{
		return;
	}

	var unSelectedFilter = evnt.params.data.id;

	for (var next = 0; next < AlarmHistoryTab.columnFilterCollection.length; next++)
	{
		var columnObj = AlarmHistoryTab.columnFilterCollection[next];

		if (columnObj.Index === selectedColumnInt)
		{
			for (var nextFilter = 0; nextFilter < columnObj.FilterCollection.length; nextFilter++)
			{
				if (unSelectedFilter === columnObj.FilterCollection[nextFilter])
				{
					columnObj.FilterCollection.splice(nextFilter, 1);
					break;
				}
			}
		}
	}
}

//====================================================================================================
// This function will update the filter collection with the date change.
//====================================================================================================
AlarmHistoryTab.HandleAvailableFilterDateChangeEvent = function (dateType)
{
	var filterObj = null;

	for ( var nextFilter = 0; nextFilter < AlarmHistoryTab.columnFilterCollection.length; nextFilter++ )
	{
		if ( AlarmHistoryTab.columnFilterCollection[nextFilter].Index === AlarmHistoryTab.dateTimeColumnIndex )
		{
			filterObj = AlarmHistoryTab.columnFilterCollection[nextFilter];
		}
	}

	if ( filterObj == null )
	{
		FMErrorAndExceptionHandling.ShowError('Could not find date filter object.', null, AlarmHistoryTab.messageAttributes);
		return;
	}

	if ( dateType === "FROM" )
	{
		var fromDateStr = $("#ColumnFilterFromDateInput").val();
		filterObj.FromDateStr = fromDateStr;
	}

	if (dateType === "TO")
	{
		var toDateStr = $("#ColumnFilterToDateInput").val();
		filterObj.ToDateStr = toDateStr;
	}
}

//====================================================================================================
// This function will update the filter collection with the comment date change.
//====================================================================================================
AlarmHistoryTab.HandleAvailableFilterCommentDateChangeEvent = function (dateType)
{
	var filterObj = null;

	for (var nextFilter = 0; nextFilter < AlarmHistoryTab.columnFilterCollection.length; nextFilter++)
	{
		if (AlarmHistoryTab.columnFilterCollection[nextFilter].Index === AlarmHistoryTab.commentDateTimeColumnIndex)
		{
			filterObj = AlarmHistoryTab.columnFilterCollection[nextFilter];
		}
	}

	if (filterObj == null)
	{
		FMErrorAndExceptionHandling.ShowError('Could not find comment date filter object.', null, AlarmHistoryTab.messageAttributes);
		return;
	}

	if (dateType === "FROM")
	{
		var fromDateStr = $("#ColumnFilterCommentFromDateInput").val();
		filterObj.CommentFromDateStr = fromDateStr;
	}

	if (dateType === "TO")
	{
		var toDateStr = $("#ColumnFilterCommentToDateInput").val();
		filterObj.CommentToDateStr = toDateStr;
	}
}

//============================================================================================
// This function will initialize the date pickers for the column filtering.
//============================================================================================
AlarmHistoryTab.InitializeDatePickers = function ()
{
	var numFormatInfoString = $("#NumberFormatInfoString").val();
	var numFormatInfo		= JSON.parse(numFormatInfoString);
	FMLayout.dateFormat		= ConvertToJQueryUIDateFormat(numFormatInfo.ShortDatePattern);
	FMLayout.timeFormat		= ConvertToJQueryUITimeFormat(numFormatInfo.TimePattern);

	$('#ColumnFilterFromDateInput').datetimepicker({
		buttonImage: FMLayout.calendarLocation + '/calendar.gif',
		buttonImageOnly: true,
		 showOn: "button",
		 showTimezone: false,
		 useLocalTimezone: false,
		 defaultTimezone: $("#datepickerTimezoneString").val(),
		dateFormat: FMLayout.dateFormat,
		timeFormat: FMLayout.timeFormat,
		beforeShow: function ()
		{
			setTimeout(function ()
			{
				$('.ui-datepicker').css('z-index', 1100);
			}, 0);
		},
		onSelect: function (d, i)
		{
			if (d !== i.lastVal)
			{
				$(this).change();
				AlarmHistoryTab.HandleAvailableFilterDateChangeEvent("FROM");
			}
		}
	});

	$('#ColumnFilterToDateInput').datetimepicker({
		buttonImage: FMLayout.calendarLocation + '/calendar.gif',
		buttonImageOnly: true,
		 showOn: "button",
		 showTimezone: false,
		 useLocalTimezone: false,
		 defaultTimezone: $("#datepickerTimezoneString").val(),
		dateFormat: FMLayout.dateFormat,
		timeFormat: FMLayout.timeFormat,
		beforeShow: function ()
		{
			setTimeout(function ()
			{
				$('.ui-datepicker').css('z-index', 1100);
			}, 0);
		},
		onSelect: function (d, i)
		{
			if (d !== i.lastVal)
			{
				$(this).change();
				AlarmHistoryTab.HandleAvailableFilterDateChangeEvent("TO");
			}
		}
	});

	$('#ColumnFilterCommentFromDateInput').datetimepicker({
		buttonImage: FMLayout.calendarLocation + '/calendar.gif',
		buttonImageOnly: true,
		 showOn: "button",
		 showTimezone: false,
		 useLocalTimezone: false,
		 defaultTimezone: $("#datepickerTimezoneString").val(),
		dateFormat: FMLayout.dateFormat,
		timeFormat: FMLayout.timeFormat,
		beforeShow: function ()
		{
			setTimeout(function ()
			{
				$('.ui-datepicker').css('z-index', 1100);
			}, 0);
		},
		onSelect: function (d, i)
		{
			if (d !== i.lastVal)
			{
				$(this).change();
				AlarmHistoryTab.HandleAvailableFilterCommentDateChangeEvent("FROM");
			}
		}
	});

	$('#ColumnFilterCommentToDateInput').datetimepicker({
		buttonImage: FMLayout.calendarLocation + '/calendar.gif',
		buttonImageOnly: true,
		 showOn: "button",
		 showTimezone: false,
		 useLocalTimezone: false,
		 defaultTimezone: $("#datepickerTimezoneString").val(),
		dateFormat: FMLayout.dateFormat,
		timeFormat: FMLayout.timeFormat,
		beforeShow: function ()
		{
			setTimeout(function ()
			{
				$('.ui-datepicker').css('z-index', 1100);
			}, 0);
		},
		onSelect: function (d, i)
		{
			if (d !== i.lastVal)
			{
				$(this).change();
				AlarmHistoryTab.HandleAvailableFilterCommentDateChangeEvent("TO");
			}
		}
	});

	// The From date is the oldest date and the To date is the most current.
	var currentDateMinuOne = new Date();
	currentDateMinuOne.setDate(currentDateMinuOne.getDate() - 1);

	var timezoneOffsetStr	= $("#TimezoneOffsetString").val();
	var timezoneOffset		= parseFloat(timezoneOffsetStr)/60.0;
	
	var currentFromDate = ConvertToSiteTimezone(currentDateMinuOne, timezoneOffset);
	var currentToDate	= ConvertToSiteTimezone(new Date(), timezoneOffset);

	// The From date is defaulted to one day in the past from the current date.
	var dateTimeFormatStr	= numFormatInfo.ShortDatePattern.toUpperCase() + " " + ConvertToMomentUITimeFormat(numFormatInfo.TimePattern);
	var momentToStr			= GetMomentDateTimeFormattedStr(currentToDate);
	var momentFromStr		= GetMomentDateTimeFormattedStr(currentFromDate);

	$('#ColumnFilterFromDateInput').datetimepicker("setDate", moment.utc(momentFromStr).format(dateTimeFormatStr));
	$('#ColumnFilterToDateInput').datetimepicker("setDate", moment.utc(momentToStr).format(dateTimeFormatStr));

	$('#ColumnFilterCommentFromDateInput').val("");
	$('#ColumnFilterCommentToDateInput').val("");

	// Set the column filter object with the date.
	AlarmHistoryTab.SetInitialDateFilters();
}

//////////////////////////////Find Processing///////////////////////////////////////////////////////////////////////////////////////////

AlarmHistoryTab.ShowHideFindResults = function (show) {
	AlarmHistoryTab.Newdiv(false);
	if (show) {
		AlarmHistoryTab.SetFindResults();
		AlarmHistoryTab.SetFindCurrentRowIndicator();
	}
	else {
		AlarmHistoryTab.HideFindCurrentRowIndicator();
	}
};

AlarmHistoryTab.HideFindCurrentRowIndicator = function () {
	var table = document.getElementById("AlarmHistoryTable");
	var td = table.getElementsByTagName("td");
	if (td) {
		for (var i = 0; i < td.length; i++) {
			td[i].classList.remove("alarmHistoryCurrentlySelectedCell");
		}
	}
}

AlarmHistoryTab.SetFindCurrentRowIndicator = function () {
	AlarmHistoryTab.HideFindCurrentRowIndicator();
	if (AlarmHistoryTab.CurrentFind) {
		var currentField = document.getElementById(AlarmHistoryTab.CurrentFind);
		if (currentField) {
			currentField.classList.add("alarmHistoryCurrentlySelectedCell");
		}
	}
}

AlarmHistoryTab.SetFindResults = function () {
	var numFindResults = 0;
	if (AlarmHistoryTab.FindArr) {
		numFindResults = AlarmHistoryTab.FindArr.length;
	}
	var findResultsString = "<i>" + numFindResults + " results</i>";
	var findResultsLabel = document.getElementById('alarmHistoryFindResultsLabel');
	findResultsLabel.innerHTML = findResultsString;
}

AlarmHistoryTab.ScrollToCurrent = function () {
	if (AlarmHistoryTab.CurrentFind) {
		var currentFindElement = document.getElementById(AlarmHistoryTab.CurrentFind);
		if (currentFindElement) {
			currentFindElement.scrollIntoView(true);
		}
	}
}

AlarmHistoryTab.ReorderFindArr = function () {
	var tempFindArr = [];
	var t = document.getElementById("AlarmHistoryTable");
	var tds = t.getElementsByTagName("td");

	for (var n = 0; n < tds.length; n++) {
		if (AlarmHistoryTab.FindArr.indexOf(tds[n].id) >= 0 && tempFindArr.indexOf(tds[n].id) < 0) {
			tempFindArr.push(tds[n].id);
		}
	}
	AlarmHistoryTab.FindArr = tempFindArr;
}

AlarmHistoryTab.DoFindWorker = function ()
{
	if (AlarmHistoryTab.CurrentFindString && AlarmHistoryTab.CurrentFindString.length > 0) {
		if (AlarmHistoryTab.FindArr.length > 0) {
			AlarmHistoryTab.ReorderFindArr();
			//Handle CurrentFind
			if (!AlarmHistoryTab.CurrentFind || AlarmHistoryTab.FindArr.indexOf(AlarmHistoryTab.CurrentFind) < 0) {
				AlarmHistoryTab.CurrentFind = AlarmHistoryTab.FindArr[0];
				AlarmHistoryTab.ScrollToCurrent();
			}
			AlarmHistoryTab.ShowHideFindResults(true);
		}
		else {
			AlarmHistoryTab.ShowHideFindResults(false);
		}
	}
	else {
		AlarmHistoryTab.ShowHideFindResults(false);
	}
}

AlarmHistoryTab.FindNext = function () {
	if (AlarmHistoryTab.FindArr && AlarmHistoryTab.CurrentFind) {
		var currentFindIndex = AlarmHistoryTab.FindArr.indexOf(AlarmHistoryTab.CurrentFind);

		if (currentFindIndex >= 0 && currentFindIndex < AlarmHistoryTab.FindArr.length - 1) {
			AlarmHistoryTab.CurrentFind = AlarmHistoryTab.FindArr[currentFindIndex + 1];
			AlarmHistoryTab.SetFindCurrentRowIndicator();
			AlarmHistoryTab.ScrollToCurrent();
		}
		else {
			AlarmHistoryTab.CurrentFind = AlarmHistoryTab.FindArr[0];
			AlarmHistoryTab.SetFindCurrentRowIndicator();
			AlarmHistoryTab.ScrollToCurrent();
		}
	}
	return false;
};

AlarmHistoryTab.FindPrev = function () {
	if (AlarmHistoryTab.FindArr && AlarmHistoryTab.CurrentFind) {
		var currentFindIndex = AlarmHistoryTab.FindArr.indexOf(AlarmHistoryTab.CurrentFind);

		if (currentFindIndex > 0) {
			AlarmHistoryTab.CurrentFind = AlarmHistoryTab.FindArr[currentFindIndex - 1];
			AlarmHistoryTab.SetFindCurrentRowIndicator();
			AlarmHistoryTab.ScrollToCurrent();
		}
		else {
			AlarmHistoryTab.CurrentFind = AlarmHistoryTab.FindArr[AlarmHistoryTab.FindArr.length - 1];
			AlarmHistoryTab.SetFindCurrentRowIndicator();
			AlarmHistoryTab.ScrollToCurrent();
		}

	}
	return false;
};

AlarmHistoryTab.PreventEnterSubmit = function (e) {
	//Prevent Post
	e = e || event;
	return (e.keyCode || e.which || e.charCode || 0) !== 13;
}

AlarmHistoryTab.DoFind = function (e) {
	var text = e.target.value;
	AlarmHistoryTab.CurrentFindString = text;
	AlarmHistoryTab.DoFindLogic();
};

AlarmHistoryTab.DoFindLogic = function () {
	AlarmHistoryTab.FindArr = [];
	AlarmHistoryTab.CurrentFind = null;
	AlarmHistoryTab.Search();
	AlarmHistoryTab.DoFindWorker();
	AlarmHistoryTab.StyleComments();


	// Hide the column filter dropdown on find search.
	$("#ColumnFilterDiv").addClass('hidden');
};

AlarmHistoryTab.GetSearchDictionaryWorker = function (searchDict, id, text)
{
	var result = AlarmHistoryTab.DoFindHighlight(id, text);
	//if ( result !== text )
	//{
		searchDict[id] = result;
	//}
}

AlarmHistoryTab.GetSearchDictionary = function () {
	var model = AlarmHistoryTab.GetAlarmModel();
	var alarmHist = model.AlarmHistories;
	var searchDict = {};
	for (var i = 0; i < alarmHist.length; i++) {
		var alrm = alarmHist[i];

		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.dateTimeColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.DateAndTime);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.siteColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.Site);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.pointTypeColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.PointType);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.pointColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.Point);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.pointDescriptionColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.PointDescription);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.variableColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.Variable);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.valueColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.Value);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.unitsColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.Units);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.alarmStateColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.AlarmState);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.priorityColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.Priority);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.actionColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.Action);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.userColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.User);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentsColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.Comments);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentUserNameColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.CommentUserName);
		AlarmHistoryTab.GetSearchDictionaryWorker(searchDict, AlarmHistoryTab.GetIdPrefix(AlarmHistoryTab.commentDateTimeColumnIndex) + alrm.AlarmAndEventRecordGuid, alrm.CommentDateTime);
	}

	return searchDict;
}

AlarmHistoryTab.Search = function () {
	var dict = AlarmHistoryTab.GetSearchDictionary();

	var table = document.getElementById("AlarmHistoryTable");
	var td = table.getElementsByTagName("td");
	if (td) {
		for (var i = 0; i < td.length; i++) {
			var id = td[i].id;
			if ( AlarmHistoryTab.inEditMode === false || id !== AlarmHistoryTab.editModeId )
			{
				var result = dict[id];
				if ( result !== undefined )
				{
					td[i].innerHTML = result;
				}
			}
		}
	}
}

AlarmHistoryTab.DoFindHighlight = function (id, text) {
	if ( text && id )
	{
		if ( AlarmHistoryTab.CurrentFindString && AlarmHistoryTab.CurrentFindString.length > 0 )
		{
			var pattern = new RegExp( AlarmHistoryTab.CurrentFindString, 'gi' );
			var retText = text.replace( pattern, function( x )
			{
				return '<span class="alarmHistoryFindColoring">' + x + '</span>';
			} );
			if ( retText !== text )
			{
				AlarmHistoryTab.FindArr.push( id );
			}
			return retText;
		}
	}
	return text;
}

AlarmHistoryTab.GetVisibilityColumnReorder = function (table)
{
	var orderArray = table.colReorder.order();
	var visibleArr = [];
	var invisibleArr = [];

	for (var i = 0; i < orderArray.length; i++)
	{
		var currentColIndex = table.colReorder.transpose(orderArray[i]);
		var column = table.column(currentColIndex);

		if (column.visible())
		{
			visibleArr.push(orderArray[i]);
		}
		else
		{
			invisibleArr.push(orderArray[i]);
		}
	}
	return { VisibleArr: visibleArr, InvisibleArr: invisibleArr };
}

AlarmHistoryTab.SetCheckBoxForColumn = function( visible, originalIndex )
{
	var checkBox = AlarmHistoryTab.GetCheckBoxForColumn(originalIndex);
	if ( checkBox )
	{
		checkBox.prop('checked', visible);
	}
}

AlarmHistoryTab.GetCheckBoxForColumn = function (originalIndex)
{
	var checkBoxId;
	switch(originalIndex)
	{
		case AlarmHistoryTab.dateTimeColumnIndex:
			checkBoxId = "DateTimeCheckbox";
			break;
		case AlarmHistoryTab.siteColumnIndex:
			checkBoxId = "SiteCheckbox";
			break;
		case AlarmHistoryTab.pointTypeColumnIndex:
			checkBoxId = "PointTypeCheckbox";
			break;
		case AlarmHistoryTab.pointColumnIndex:
			checkBoxId = "PointCheckbox";
			break;
		case AlarmHistoryTab.pointDescriptionColumnIndex:
			checkBoxId = "PointDescriptionCheckbox";
			break;
		case AlarmHistoryTab.variableColumnIndex:
			checkBoxId = "VariableCheckbox";
			break;
		case AlarmHistoryTab.valueColumnIndex:
			checkBoxId = "ValueCheckbox";
			break;
		case AlarmHistoryTab.unitsColumnIndex:
			checkBoxId = "UnitsCheckbox";
			break;
		case AlarmHistoryTab.alarmStateColumnIndex:
			checkBoxId = "AlarmStateCheckbox";
			break;
		case AlarmHistoryTab.priorityColumnIndex:
			checkBoxId = "PriorityCheckbox";
			break;
		case AlarmHistoryTab.actionColumnIndex:
			checkBoxId = "ActionCheckbox";
			break;
		case AlarmHistoryTab.userColumnIndex:
			checkBoxId = "UserCheckbox";
			break;
		case AlarmHistoryTab.commentsColumnIndex:
			checkBoxId = "CommentsCheckbox";
			break;
		case AlarmHistoryTab.commentUserNameColumnIndex:
			checkBoxId = "CommentUserNameCheckbox";
			break;
		case AlarmHistoryTab.commentDateTimeColumnIndex:
			checkBoxId = "CommentDateTimeCheckbox";
			break;
		default:
			return null;
	}
	var checkBox = $('#' + checkBoxId);
	return checkBox;
}




AlarmHistoryTab.SetInitialVisibilityColumnReorder = function (visInvisColumns)
{
	if (!visInvisColumns)
	{
		return;
	}

	var visibleArr = visInvisColumns.VisibleArr;
	var invisibleArr = visInvisColumns.InvisibleArr;

	if ( !visibleArr || !invisibleArr )
	{
		return;
	}

	if ( visibleArr.length === 0 && invisibleArr.length === 0 )
	{
		return;
	}

	if (AlarmHistoryTab.HistoryInitialized === false)
		return;
	var table = $("#AlarmHistoryTable").DataTable();
	var newOrderArray = Array.from(visibleArr);

	for (var j = 0; j < invisibleArr.length; j++)
	{
		newOrderArray.push(invisibleArr[j]);
	}

	table.colReorder.order(newOrderArray, true);

	for (var i = 0; i < visibleArr.length; i++)
	{
		var currentColIndex = table.colReorder.transpose(visibleArr[i]);
		var column = table.column(currentColIndex);
		column.visible(true);
		AlarmHistoryTab.SetCheckBoxForColumn( true, visibleArr[i] );
	}

	for (var k = 0; k < invisibleArr.length; k++)
	{
		var currentColIndex2 = table.colReorder.transpose(invisibleArr[k]);
		var column2 = table.column(currentColIndex2);
		column2.visible(false);
		AlarmHistoryTab.SetCheckBoxForColumn(false, invisibleArr[k]);
	}
}

//===================================================================================
// This function will save the view state settings for visible columns, column order, 
// and filters.
//===================================================================================
AlarmHistoryTab.SaveViewState = function () {
	var saveViewStateUrl = $("#AlarmHistorySaveViewStateUrl").val();
	if (saveViewStateUrl) {
		;
	}
	else {
		//Hardcode path for saveViewStateUrl here , since hidden control AlarmHistorySaveViewStateUrl will not be available if tab has been closed
		saveViewStateUrl = window.applicationRootName + "/InventoryManagement/AlarmHistoryTab/SaveViewState";
	}

	var token = $('#AlarmHistoryTabView input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;
	var d = AlarmHistoryTab.GetVisibilityColumnReorder(AlarmHistoryTab.DatatableHandle);
	var alarmHistoryViewStateObject = {};
	alarmHistoryViewStateObject.VisibleArr = d.VisibleArr;
	alarmHistoryViewStateObject.InvisibleArr = d.InvisibleArr;
	alarmHistoryViewStateObject.Filters = AlarmHistoryTab.columnFilterCollection;
	alarmHistoryViewStateObject.PageLen = AlarmHistoryTab.DatatableHandle.page.len();
	var alarmHistoryViewStateJson = JSON.stringify(alarmHistoryViewStateObject);

	PNotify.removeStack(AlarmHistoryTab.messageAttributes.stack);
	$.ajax({
		cache: false,
		type: "POST",
		//async: false,
		contentType: 'application/json; charset=UTF-8',
		dataType: "json",
		url: saveViewStateUrl,
		headers: headers,
		//data: { visibleArr: d.VisibleArr, invisibleArr: d.InvisibleArr, filters: AlarmHistoryTab.columnFilterCollection },
		data: JSON.stringify({ jsonViewState: alarmHistoryViewStateJson }),
		success: function( dummy )
		{
			return undefined;
		},
		error: function (e) {
			FMErrorAndExceptionHandling.ShowError(e, null, AlarmHistoryTab.messageAttributes);
			return false;
		}
	});
}


AlarmHistoryTab.ResizeColumns = function () {
	// this is called when the tab is selected to cause a redraw bds
	AlarmHistoryTab.Initialize();

	if (AlarmHistoryTab.DatatableHandle != null) {
		AlarmHistoryTab.DatatableHandle.columns.adjust();
	}
}
var AlarmSummary2 = AlarmSummary2 || {
	stack_bottomright_shelve: { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#AlarmSummary2Section') },
};

if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

AlarmSummary2.TabSwitch = function(evt, tabDiv) {
	// Declare all variables
	var i, tabcontent, tablinks;

	AlarmSummaryTab.StopTimer();

	// Get all elements with class="tabcontent" and hide them
	tabcontent = document.getElementsByClassName("tabcontent");
	for (i = 0; i < tabcontent.length; i++) {
		tabcontent[i].style.display = "none";
	}

	// Get all elements with class="tablinks" and remove the class "active"
	tablinks = document.getElementsByClassName("tablinks");
	for (i = 0; i < tablinks.length; i++) {
		tablinks[i].className = tablinks[i].className.replace(" active", "");
	}

	// Show the current tab, and add an "active" class to the button that opened the tab
	document.getElementById(tabDiv).style.display = "block";
	evt.currentTarget.className += " active";

	//Get Partial View For Tab
	if ( tabDiv === "AlarmSummary" )
	{
		AlarmSummary2.GetAlarmSummaryTab();
	}
	else if (tabDiv === "AlarmHistory")
	{
		AlarmSummary2.GetAlarmHistoryTab();
	}

	//Prevent Post
	return false;
}

AlarmSummary2.stringIsEmpty = function (value)
{
	return value ? value.trim().length == 0 : true;
}

AlarmSummary2.GetAlarmSummaryTab = function ()
{
	var tabInitialHtml = document.getElementById('AlarmSummaryTabSection').innerHTML;
	if ( AlarmSummary2.stringIsEmpty( tabInitialHtml ) == false )
	{
		AlarmSummaryTab.StartTimer();
		return;
	}
	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// notification position
	//var messageAttributes = { addclass: 'stack-bottomright', stack: AlarmSummary2.stack_bottomright_ackcomment };
	// remove previous notifications
	PNotify.removeStack(AlarmSummary2.stack_bottomright_ackcomment);

	$('<div id="loader" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#AlarmSummaryTabSection');
	
	$.ajax({
		type: 'Post',
		url: 'GetAlarmSummaryTab',
		cache: false,
		data: {
			'__RequestVerificationToken': $( 'input[name=__RequestVerificationToken]' ).val()
		},
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					// replace the holder with the partial view
					$('#AlarmSummaryTabSection').html(data);
					AlarmSummaryTab.StartTimer();
				}
				$('#loader').remove();
			});
		},
		error: function (xhr, textStatus, error) {
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () {
					$('#loader').remove();
				});
		}

	});
}

AlarmSummary2.GetAlarmHistoryTab = function ()
{
	var tabInitialHtml = document.getElementById('AlarmHistoryTabSection').innerHTML;
	if (AlarmSummary2.stringIsEmpty(tabInitialHtml) == false) {
		return;
	}

	$('<div id="loader" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#AlarmHistoryTabSection');

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// notification position
	//var messageAttributes = { addclass: 'stack-bottomright', stack: AlarmSummary2.stack_bottomright_ackcomment };
	// remove previous notifications
	PNotify.removeStack(AlarmSummary2.stack_bottomright_ackcomment);

	$.ajax({
		type: 'Post',
		url: 'GetAlarmHistoryTab',
		cache: false,
		data: {
			'__RequestVerificationToken': $('input[name=__RequestVerificationToken]').val()
		},
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					// replace the holder with the partial view
					$('#AlarmHistoryTabSection').html(data);
				}
				else {
				}
				$('#loader').remove();
			});
		},
		error: function (xhr, textStatus, error) {
			FMErrorAndExceptionHandling.ShowException(xhr,
				textStatus,
				error,
				function () {
					$('#loader').remove();
				});
		}

	});
}
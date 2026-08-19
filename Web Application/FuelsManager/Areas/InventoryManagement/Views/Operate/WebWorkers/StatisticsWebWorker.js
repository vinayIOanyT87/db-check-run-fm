var url = "";
var refreshTimeout = 1000; //allow rapid looping until the a message is received that configures the actual work timeout
var requestVerificationToken = "";
var failures = 0;
var statistics = null;
var windowName = '';


// message handler
onmessage = function (e) {
	if (e.data.messageType == "startup") {
		url = e.data.url;
		windowName = e.data.windowName;
		refreshTimeout = e.data.refreshTimeout;
		requestVerificationToken = e.data.requestVerificationToken;
	} else {
		if (e.data.messageType == "statistics alarmSummaryTab") {
			statistics.AvgMinuteTimeAlarmRefresh = Math.round(e.data.statistics.minuteAvgTime);
			statistics.MaxMinuteTimeAlarmRefresh = Math.round(e.data.statistics.minuteMaxTime);
			statistics.AvgSessionTimeAlarmRefresh = Math.round(e.data.statistics.sessionAvgTime);
			statistics.MaxSessionTimeAlarmRefresh = Math.round(e.data.statistics.sessionMaxTime);
		} else if (e.data.messageType == "statistics dynamicPointGroup") {
			statistics.AvgMinuteTimeDynamicPointGroup = Math.round(e.data.statistics.minuteAvgTime);
			statistics.MaxMinuteTimeDynamicPointGroup = Math.round(e.data.statistics.minuteMaxTime);
			statistics.AvgSessionTimeDynamicPointGroup = Math.round(e.data.statistics.sessionAvgTime);
			statistics.MaxSessionTimeDynamicPointGroup = Math.round(e.data.statistics.sessionMaxTime);
		} else if (e.data.messageType == "statistics alarmNotification") {
			statistics.AvgMinuteTimeAlarmNotifications = Math.round(e.data.statistics.minuteAvgTime);
			statistics.MaxMinuteTimeAlarmNotifications = Math.round(e.data.statistics.minuteMaxTime);
			statistics.AvgSessionTimeAlarmNotifications = Math.round(e.data.statistics.sessionAvgTime);
			statistics.MaxSessionTimeAlarmNotifications = Math.round(e.data.statistics.sessionMaxTime);
		} else if (e.data.messageType == "statistics tagUpdateValues") {
 			statistics.AvgMinuteTimeUpdateValues = Math.round(e.data.statistics.minuteAvgTime);
			statistics.MaxMinuteTimeUpdateValues = Math.round(e.data.statistics.minuteMaxTime);
			statistics.AvgSessionTimeUpdateValues = Math.round(e.data.statistics.sessionAvgTime);
			statistics.MaxSessionTimeUpdateValues = Math.round(e.data.statistics.sessionMaxTime);
		}

		if (statistics.AvgMinuteTimeAlarmRefresh > -1
			&& statistics.AvgMinuteTimeDynamicPointGroup > -1
			&& statistics.AvgMinuteTimeAlarmNotifications > -1
			&& statistics.AvgMinuteTimeUpdateValues > -1) {
			saveStatistics(statistics);
		}

	}
}

function main() {
	var mainStartTime = Date.now();
	if (url === "") {
		setTimeout(function () { main(); }, getTimeout(mainStartTime, refreshTimeout));
		return;
	}

	statistics = {};
	statistics.WindowName = '';
	statistics.AvgMinuteTimeAlarmRefresh = -1;
	statistics.AvgMinuteTimeDynamicPointGroup = -1;
	statistics.AvgMinuteTimeAlarmNotifications = -1;
	statistics.AvgMinuteTimeUpdateValues = -1;
	postMessage({ command: "GetStatistics" });
	//console.log(url);

	setTimeout(function () { main(); }, getTimeout(mainStartTime, refreshTimeout, windowName));
}

function saveStatistics(statistics) {
	var xmlhttp = new XMLHttpRequest();
	statistics.WindowName = windowName;
	var params = JSON.stringify({ statistics: statistics });

	xmlhttp.open("post", url + "?t=" + Math.random(), true);
	//Send the proper header information along with the request
	xmlhttp.setRequestHeader("Content-type", "application/json; charset=utf-8");
	xmlhttp.setRequestHeader("Cache-Control", "no-cache");
	xmlhttp.setRequestHeader("__RequestVerificationToken", requestVerificationToken);
	xmlhttp.setRequestHeader("X-Requested-With", "XMLHttpRequest");

	xmlhttp.onreadystatechange = function () {
		if (xmlhttp.readyState === 4) {
			if (xmlhttp.status === 200) {
				if (xmlhttp.responseText.startsWith('\"Exception\"')) {
					console.log("Statistics worker refresh has failed " + failures + " time(s).")
					if (failures > 3) {
						postMessage({ messageType: "Loss of server connection" });
						return;
					}
					failures++;

					xmlhttp = null;
					return;
				}

			}

			xmlhttp = null;
		}
	}

	xmlhttp.onerror = function () {
		setTimeout(function () { main(); }, getTimeout(mainStartTime, saveTimeout));
		xmlhttp = null;
	}

  xmlhttp.send(params);

}

function getTimeout(startTime, timeout) {
	var elapsedTime = (Date.now() - startTime);
	var efficientTimeout = timeout - elapsedTime;
	efficientTimeout = (efficientTimeout < 0 ? 0 : efficientTimeout);

	return efficientTimeout;
}

main();
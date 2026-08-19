var masterTagList = [];
var url = "";
var refreshTimeout = 5000;
var requestVerificationToken = "";
var statistics = new Array();
var siteTimeZone = "";
var AlarmGuid = null;
var AlarmTestGuid = null;
var isNormal = false;
var isSilenced = false;
var NumberOfAlarms = 0;
var NormalUnacknowledgedAlarmTextSteadyColor = "";
var NormalUnacknowledgedAlarmBackgroundSteadyColor = "";
var AlarmTextSteadyColor = "";
var AlarmBackgroundSteadyColor = "";
var SoundFile = "";
var failures = 0;
var Status = "";

/*---------------------- POLYFILLS ---------------------------*/
// Production steps of ECMA-262, Edition 5, 15.4.4.18

if (!String.prototype.startsWith) {
	String.prototype.startsWith = function (searchString, position) {
		position = position || 0;
		return this.substr(position, searchString.length) === searchString;
	};
}

/*---------------------- END OF POLYFILLS ---------------------------*/

// message handler
onmessage = function (e) {
	if (e.data && e.data.messageType && e.data.messageType == "getStatistics") {
		postMessage({ messageType: "alarmNotificationStatisticsResults", statistics: getStatistics() });
	} else {
		url = e.data.url;
		refreshTimeout = e.data.refreshTimeout;
		requestVerificationToken = e.data.requestVerificationToken;
	}
}


function main()
{
	var mainStartTime = Date.now();
	if (url === "")
	{
		setTimeout(function () { main(); }, getAlarmRefreshTimeout(mainStartTime, refreshTimeout));
		return;
	}

	var xmlhttp = new XMLHttpRequest();

	xmlhttp.open("get", url + "?t=" + Math.random(), true);
	//Send the proper header information along with the request
	xmlhttp.setRequestHeader("Content-type", "application/json; charset=utf-8");
	xmlhttp.setRequestHeader("Cache-Control", "no-cache");
	xmlhttp.setRequestHeader("__RequestVerificationToken", requestVerificationToken);
	xmlhttp.setRequestHeader("X-Requested-With", "XMLHttpRequest");

	xmlhttp.onreadystatechange = function ( ) {
		if (xmlhttp.readyState === 4) {
            if (xmlhttp.status === 200) {
                if (xmlhttp.responseText.startsWith('\"Exception\"')) {
                    console.log("Alarm web worker refresh has failed " + failures + " time(s).")
                    if (failures > 3) {
							  postMessage({ messageType: "Loss of server connection" });
							  return;
                    }
                    failures++;

  									setTimeout(function () { main(); }, getAlarmRefreshTimeout(mainStartTime, refreshTimeout));
                    return;
                }
				else if (xmlhttp.responseText.startsWith( '<script type="text/javascript">' ))
				{
					// what's returned its not an object, its code
					setTimeout(function () { main(); }, getAlarmRefreshTimeout(mainStartTime, refreshTimeout));
					return;
				}
				else if (!xmlhttp.responseText.startsWith( '{') ) {
					// response is not JSON
					setTimeout(function () { main(); }, getAlarmRefreshTimeout(mainStartTime, refreshTimeout));
					return;
				}

				var response = JSON.parse(xmlhttp.responseText);
				var results = response.AlarmDetail;

				if ( results.AlarmSummaries.length > 0 )
				{
					if (AlarmGuid !== results.AlarmSummaries[0].AlarmGuid
					|| AlarmTestGuid !== results.AlarmSummaries[0].AlarmTestGuid
					|| isNormal !== results.AlarmSummaries[0].IsNormal
					|| isSilenced !== results.AlarmSummaries[0].Silenced
					|| NumberOfAlarms !== parseInt(response.NumberOfAlarms)
					|| NormalUnacknowledgedAlarmTextSteadyColor !== results.AlarmSummaries[0].NormalUnacknowledgedAlarmTextSteadyColor
					|| NormalUnacknowledgedAlarmBackgroundSteadyColor !== results.AlarmSummaries[0].NormalUnacknowledgedAlarmBackgroundSteadyColor
					|| AlarmTextSteadyColor !== results.AlarmSummaries[0].AlarmTextSteadyColor
					|| AlarmBackgroundSteadyColor !== results.AlarmSummaries[0].AlarmBackgroundSteadyColor
					|| SoundFile !== results.AlarmSummaries[0].SoundFile
					|| Status !== results.AlarmSummaries[0].Status)
					{
						AlarmGuid = results.AlarmSummaries[0].AlarmGuid;
						AlarmTestGuid = results.AlarmSummaries[0].AlarmTestGuid;
						isNormal = results.AlarmSummaries[0].IsNormal;
						isSilenced = results.AlarmSummaries[0].Silenced;
						NumberOfAlarms = parseInt(response.NumberOfAlarms);
						NormalUnacknowledgedAlarmTextSteadyColor = results.AlarmSummaries[0].NormalUnacknowledgedAlarmTextSteadyColor;
						NormalUnacknowledgedAlarmBackgroundSteadyColor = results.AlarmSummaries[0].NormalUnacknowledgedAlarmBackgroundSteadyColor;
						AlarmTextSteadyColor = results.AlarmSummaries[0].AlarmTextSteadyColor;
						AlarmBackgroundSteadyColor = results.AlarmSummaries[0].AlarmBackgroundSteadyColor;
						SoundFile = results.AlarmSummaries[0].SoundFile;
						Status = results.AlarmSummaries[0].Status;

						postMessage(response);
					}
				}
				else
				{
					NumberOfAlarms = 0;
					if (AlarmGuid !== null) {
						AlarmGuid = null;
						postMessage(response);
					}
				}
			}
			else {
			}

			setTimeout(function () { main(); }, getAlarmRefreshTimeout(mainStartTime, refreshTimeout));
			xmlhttp = null;
		}
	}

	xmlhttp.onerror = function () {
		setTimeout(function () { main(); }, getAlarmRefreshTimeout(mainStartTime, refreshTimeout));
		xmlhttp = null;
	}

	xmlhttp.send();

};

function getAlarmRefreshTimeout(startTime, refreshTimeout) {
	var elapsedTime = (Date.now() - startTime);
	var efficientRefreshTimeout = refreshTimeout - elapsedTime;
	efficientRefreshTimeout = (efficientRefreshTimeout < 0 ? 0 : efficientRefreshTimeout);
	//console.log("ALM " + efficientRefreshTimeout);
	statistics.push({ timestamp: Date.now(), elapsed: elapsedTime });

	return efficientRefreshTimeout;
}

function getStatistics() {
	var minuteCount = 0;
	var minuteTotalTime = 0;
	var minuteMaxTime = 0;
	var sessionCount = 0;
	var sessionTotalTime = 0;
	var sessionMaxTime = 0;
	var timestamp = Date.now();

	for (i = statistics.length - 1; i > 0; i--) {
		var record = statistics[i];
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


main();

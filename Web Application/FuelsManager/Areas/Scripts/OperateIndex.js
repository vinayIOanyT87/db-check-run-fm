var FMAnimationTestComparisonOperators = {
	GreaterThan: 0,
	GreaterThanOrEqual: 1,
	LessThan: 2,
	LessThanOrEqual: 3,
	Equals: 4,
	NotEqual: 5,
	Else: 6,
	Contains: 7,
	BeginsWith: 8
};

var FMAnimationTestBitmaskOperators = {
	And: 0,
	Or: 1,
	Nand: 2,
	Nor: 3,
	Xand: 4,
	Xor: 5,
	None: 6
};

var FMPointValueFieldType = {
	VALUE: 0,
	ID: 1,
	TIMESTAMP: 2,
	UNITS: 3,
	ALARMSTATUS: 4
};

var FMPointValueType = {
	Tag: 0,
	Setting: 1,
	Point: 2,
	All: 3
};

if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

// main object for the operate screen
var FMOperateIndex = {
	tagRefreshFrequency: 1000,
	alarmRefreshFrequency: 1000,
	statisticsRefreshFrequency: 60000,

	mouseClick: new Date().getMilliseconds(),

	stopWatch: new StopWatch(),

	// This variable is used to keep closer track on objects that consume realtime data
	activeObjects: [],

	originalPointValueGrahicList: [],

	// notification stack for the main screen (each tab will have it's own stack to display errors realted to the drawing)
	stack_bottomright_operate: { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 },
	// operate window content
	contents: [],

	userId: 0,
	siteId: 0,
	lastTabCreated: 0,
	restoringView: false,
	numformatInfo: null,
	dateTimeFormatInfo: null,
	// this variable is used to keep track of all the tabs/tabgroups that are being loading when opening the screen
	restoringScreenQueueInProgress: {},
	restoringScreenactiveTabs: [],
	restoringSaveInitialConfig: false,
	tagWebWorker: null,
	alarmWebWorker: null,
	statisticsWebWorker: null,
	tagWebWorkerSubscriptions: [],
	pointWebWorkerSubscriptions: [],
	// list of grid controllers for the point groups
	staticPointGroupControllers: {},
	// list of grid controllers for the point histories
	staticPointHistoryControllers: {},
	// List of grid controllers for the movement summaries
	movementSummaryControllers: {},
	// List of grid controllers for the movement histories
	movementHistoryControllers: {},
	// need to refresh the hamburger menu next time its displayed
	refreshHamburgerMenu: false,
	deferredUpdateTagsAjaxRequest: null,
	alarmPriorities: null,
	displayAlternateIfUnacknowledged: false,
	pointProductGraphicList: [],
	currentContextMenu: null,
	viewPointGroupsRight: false,
	modifyPointGroupsRight: false,
	createPublicPointGroupsRight: false,
	modifyPublicPointGroupsRight: false,
	createSharedPointGroupsRight: false,
	modifySharedPointGroupsRight: false,
	viewMovementSummaryRight: false,
	modifyMovementSummaryRight: false,
	createPublicMovementSummaryRight: false,
	modifyPublicMovementSummaryRight: false,
	createSharedMovementSummaryRight: false,
	modifySharedMovementSummaryRight: false,
	viewMovementHistoryRight: false,
	modifyMovementHistoryRight: false,
	fullscreenDocument: null,
	openingNewAlarmSummary: false,
	openingNewAlarmHistory: false,
	openingNewMovementHistory: false,
	allScreensRestored: false,
	reportRedrawQueue: 0,
	screens: [],
	annunciationAnimationTimer: null,
	loggingOut: false
};

FMOperateIndex.GetOperateWindowName = FMOperateIndex.GetOperateWindowName || function () {
	var screenMatch = /^Screen(\d+)$/i.exec(window.name || '');
	if (screenMatch !== null) {
		var screenIndex = parseInt(screenMatch[1], 10);
		if (!isNaN(screenIndex) && screenIndex > 0) {
			return 'Screen' + screenIndex;
		}
	}

	return 'Screen1';
};

FMOperateIndex.GetCurrentMonitorCount = FMOperateIndex.GetCurrentMonitorCount || function (screenDetails) {
	var monitorCount = window.screen && window.screen.isExtended ? 2 : 1;
	if (screenDetails != null
		&& screenDetails.screens != null
		&& typeof screenDetails.screens.length === 'number') {
		monitorCount = screenDetails.screens.length;
	}

	console.log("Operate monitors detected: " + monitorCount);
	return monitorCount;
};

FMOperateIndex.GetCurrentOperateMonitorCount = FMOperateIndex.GetCurrentOperateMonitorCount || function () {
	if (typeof FMOperateIndex.currentMonitorCount === 'number' && FMOperateIndex.currentMonitorCount > 0) {
		return FMOperateIndex.currentMonitorCount;
	}

	return FMOperateIndex.GetCurrentMonitorCount(null);
};

FMOperateIndex.GetDetectedMonitorCount = function () {
	if (FMOperateIndex.GetCurrentOperateMonitorCount) {
		return FMOperateIndex.GetCurrentOperateMonitorCount();
	}

	return window.screen.isExtended ? 2 : 1;
};

FMOperateIndex.GetScreenBit = FMOperateIndex.GetScreenBit || function (screenNumber) {
	return Math.pow(2, screenNumber - 1);
};

FMOperateIndex.NormalizeOperateScreenMask = FMOperateIndex.NormalizeOperateScreenMask || function (screenMask) {
	screenMask = parseInt(screenMask, 10);
	if (isNaN(screenMask) || screenMask < 1) {
		screenMask = 1;
	}

	if (!FMOperateIndex.IsScreenAssigned(screenMask, 1)) {
		screenMask += 1;
	}

	return screenMask;
};

FMOperateIndex.IsScreenAssigned = FMOperateIndex.IsScreenAssigned || function (screenMask, screenNumber) {
	screenMask = parseInt(screenMask, 10);
	if (isNaN(screenMask) || screenNumber < 1) {
		return false;
	}

	var screenBit = FMOperateIndex.GetScreenBit(screenNumber);
	return Math.floor(screenMask / screenBit) % 2 === 1;
};

FMOperateIndex.GetScreenMaskForMonitorCount = FMOperateIndex.GetScreenMaskForMonitorCount || function (monitorCount) {
	var screenMask = 0;
	for (var screenNumber = 1; screenNumber <= monitorCount; screenNumber++) {
		screenMask += FMOperateIndex.GetScreenBit(screenNumber);
	}

	return FMOperateIndex.NormalizeOperateScreenMask(screenMask);
};

FMOperateIndex.GetHighestAssignedScreenNumber = FMOperateIndex.GetHighestAssignedScreenNumber || function (screenMask) {
	screenMask = FMOperateIndex.NormalizeOperateScreenMask(screenMask);
	var highestScreenNumber = 1;
	for (var screenNumber = 1; screenNumber <= 32; screenNumber++) {
		if (FMOperateIndex.IsScreenAssigned(screenMask, screenNumber)) {
			highestScreenNumber = screenNumber;
		}
	}

	return highestScreenNumber;
};

FMOperateIndex.GetAssignedAvailableScreenMask = FMOperateIndex.GetAssignedAvailableScreenMask || function (screenMask, monitorCount) {
	screenMask = FMOperateIndex.NormalizeOperateScreenMask(screenMask);
	var assignedAvailableMask = 1;
	for (var screenNumber = 2; screenNumber <= monitorCount; screenNumber++) {
		if (FMOperateIndex.IsScreenAssigned(screenMask, screenNumber)) {
			assignedAvailableMask += FMOperateIndex.GetScreenBit(screenNumber);
		}
	}

	return FMOperateIndex.NormalizeOperateScreenMask(assignedAvailableMask);
};

FMOperateIndex.GetNewAssignedScreenMask = FMOperateIndex.GetNewAssignedScreenMask || function (screenMask, previousScreenMask, monitorCount) {
	screenMask = FMOperateIndex.NormalizeOperateScreenMask(screenMask);
	previousScreenMask = FMOperateIndex.NormalizeOperateScreenMask(previousScreenMask);
	var newScreenMask = 0;
	for (var screenNumber = 2; screenNumber <= monitorCount; screenNumber++) {
		if (FMOperateIndex.IsScreenAssigned(screenMask, screenNumber)
			&& !FMOperateIndex.IsScreenAssigned(previousScreenMask, screenNumber)) {
			newScreenMask += FMOperateIndex.GetScreenBit(screenNumber);
		}
	}

	return newScreenMask;
};

FMOperateIndex.HasAdditionalAssignedScreens = FMOperateIndex.HasAdditionalAssignedScreens || function (screenMask) {
	return FMOperateIndex.GetHighestAssignedScreenNumber(screenMask) > 1;
};

FMOperateIndex.Delay = FMOperateIndex.Delay || function (timeoutMs) {
	return new Promise(function (resolve) {
		setTimeout(resolve, timeoutMs);
	});
};

FMOperateIndex.GetExpectedOperateMonitorCount = FMOperateIndex.GetExpectedOperateMonitorCount || function (monitorState) {
	var expectedMonitorCount = 1;
	if (monitorState != null) {
		var savedMonitorCount = parseInt(monitorState.monitorCount, 10);
		if (!isNaN(savedMonitorCount)) {
			expectedMonitorCount = Math.max(expectedMonitorCount, savedMonitorCount);
		}

		var maxConfiguredMonitorIndex = parseInt(monitorState.maxConfiguredMonitorIndex, 10);
		if (!isNaN(maxConfiguredMonitorIndex)) {
			expectedMonitorCount = Math.max(expectedMonitorCount, maxConfiguredMonitorIndex);
		}

		expectedMonitorCount = Math.max(expectedMonitorCount, FMOperateIndex.GetHighestAssignedScreenNumber(monitorState.screenMask));
	}

	return expectedMonitorCount;
};

FMOperateIndex.HasAvailableMonitorCountChanged = FMOperateIndex.HasAvailableMonitorCountChanged || function (monitorState, currentMonitorCount) {
	currentMonitorCount = Math.max(1, parseInt(currentMonitorCount, 10) || 1);
	if (currentMonitorCount <= 1) {
		return false;
	}

	var savedMonitorCount = monitorState == null ? NaN : parseInt(monitorState.monitorCount, 10);
	if (isNaN(savedMonitorCount)) {
		return true;
	}

	return savedMonitorCount !== currentMonitorCount;
};

FMOperateIndex.GetOperateMonitorDetectionResult = FMOperateIndex.GetOperateMonitorDetectionResult || function (screenDetails) {
	var hasScreenDetails = screenDetails != null
		&& screenDetails.screens != null
		&& typeof screenDetails.screens.length === 'number';
	var browserReportsSingleMonitor = window.screen && window.screen.isExtended === false;
	var monitorCount = FMOperateIndex.GetCurrentMonitorCount(screenDetails);
	FMOperateIndex.currentMonitorCount = monitorCount;
	return {
		monitorCount: monitorCount,
		screenDetails: screenDetails,
		isReliable: hasScreenDetails || browserReportsSingleMonitor
	};
};

FMOperateIndex.WaitForOperateMonitorDetection = FMOperateIndex.WaitForOperateMonitorDetection || async function (expectedMonitorCount, initialScreenDetails) {
	var detection = FMOperateIndex.GetOperateMonitorDetectionResult(initialScreenDetails);
	var attemptsRemaining = expectedMonitorCount > detection.monitorCount ? 10 : 0;

	while (attemptsRemaining > 0 && detection.monitorCount < expectedMonitorCount) {
		await FMOperateIndex.Delay(1000);
		detection = FMOperateIndex.GetOperateMonitorDetectionResult(await FMOperateIndex.getPermissionAndScreenDetails());
		attemptsRemaining -= 1;
	}

	return detection;
};

FMOperateIndex.ResolveWithTimeout = FMOperateIndex.ResolveWithTimeout || function (promise, timeoutMs, timeoutValue, timeoutMessage) {
	return new Promise(function (resolve) {
		var isResolved = false;
		var timer = setTimeout(function () {
			if (isResolved) {
				return;
			}

			isResolved = true;
			if (timeoutMessage) {
				console.warn(timeoutMessage);
			}
			resolve(timeoutValue);
		}, timeoutMs);

		Promise.resolve(promise).then(function (value) {
			if (isResolved) {
				return;
			}

			isResolved = true;
			clearTimeout(timer);
			resolve(value);
		}, function (err) {
			if (isResolved) {
				return;
			}

			isResolved = true;
			clearTimeout(timer);
			console.warn(timeoutMessage || 'Operate async request failed.', err);
			resolve(timeoutValue);
		});
	});
};

FMOperateIndex.getPermissionAndScreenDetails = FMOperateIndex.getPermissionAndScreenDetails || async function () {
	if (!('getScreenDetails' in window)) {
		return null;
	}

	try {
		if (navigator.permissions && navigator.permissions.query) {
			var permission = await FMOperateIndex.ResolveWithTimeout(
				navigator.permissions.query({ name: 'window-management' }),
				1500,
				null,
				'Timed out querying window-management permission.');
			if (permission != null) {
				console.log(permission, permission.state);
			}
			if (permission != null && permission.state === 'denied') {
				return null;
			}
		}

		return await FMOperateIndex.ResolveWithTimeout(
			window.getScreenDetails(),
			3000,
			null,
			'Timed out getting screen details for additional Operate windows.');
	}
	catch (err) {
		console.warn('Unable to get screen details for additional Operate windows.', err);
		return null;
	}
};

FMOperateIndex.GetOperateWindowFeatures = FMOperateIndex.GetOperateWindowFeatures || function (screen) {
	return "left=" + screen.left + ",top=" + screen.top + ",width=" + screen.width + ",height=" + screen.height;
};

FMOperateIndex.OpenOperateWindowOnScreen = FMOperateIndex.OpenOperateWindowOnScreen || function (screen, windowName) {
	return window.open(window.location.href, windowName, FMOperateIndex.GetOperateWindowFeatures(screen));
};

FMOperateIndex.GetOpenOperateWindowScreenMask = FMOperateIndex.GetOpenOperateWindowScreenMask || function (screenMask, monitorCount) {
	screenMask = FMOperateIndex.NormalizeOperateScreenMask(screenMask || 1);
	var childWindows = FMOperateIndex.operateChildWindowsByName || {};
	for (var windowName in childWindows) {
		if (!childWindows.hasOwnProperty(windowName)) {
			continue;
		}

		var screenMatch = /^Screen(\d+)$/i.exec(windowName);
		if (screenMatch == null) {
			continue;
		}

		var screenNumber = parseInt(screenMatch[1], 10);
		if (isNaN(screenNumber) || screenNumber < 2 || screenNumber > monitorCount) {
			continue;
		}

		var childWindow = childWindows[windowName];
		if (childWindow != null && childWindow.closed !== true && !FMOperateIndex.IsScreenAssigned(screenMask, screenNumber)) {
			screenMask += FMOperateIndex.GetScreenBit(screenNumber);
		}
	}

	return FMOperateIndex.NormalizeOperateScreenMask(screenMask);
};

FMOperateIndex.CloseUnassignedOperateWindows = FMOperateIndex.CloseUnassignedOperateWindows || function (screenMask, monitorCount) {
	screenMask = FMOperateIndex.NormalizeOperateScreenMask(screenMask);
	var childWindows = FMOperateIndex.operateChildWindowsByName || {};
	for (var windowName in childWindows) {
		if (!childWindows.hasOwnProperty(windowName)) {
			continue;
		}

		var screenMatch = /^Screen(\d+)$/i.exec(windowName);
		if (screenMatch == null) {
			continue;
		}

		var screenNumber = parseInt(screenMatch[1], 10);
		if (isNaN(screenNumber) || screenNumber < 2) {
			continue;
		}

		if (screenNumber <= monitorCount && FMOperateIndex.IsScreenAssigned(screenMask, screenNumber)) {
			continue;
		}

		var childWindow = childWindows[windowName];
		if (childWindow == null || childWindow.closed === true) {
			continue;
		}

		try {
			if (childWindow.FMOperateIndex) {
				childWindow.FMOperateIndex.skipPersistScreenConfiguration = true;
			}

			childWindow.close();
		}
		catch (err) {
			console.warn('Unable to close unassigned Operate screen ' + windowName + '.', err);
		}
	}
};

FMOperateIndex.OpenAdditionalOperateWindows = FMOperateIndex.OpenAdditionalOperateWindows || async function (screenDetails, firstScreenNumber, lastScreenNumber, screenMask) {
	var childWindows = [];
	FMOperateIndex.lastAdditionalOperateScreenOpenFailures = [];
	FMOperateIndex.operateChildWindowsByName = FMOperateIndex.operateChildWindowsByName || {};
	firstScreenNumber = firstScreenNumber || 2;
	lastScreenNumber = lastScreenNumber || Number.MAX_VALUE;
	screenMask = screenMask == null ? null : FMOperateIndex.NormalizeOperateScreenMask(screenMask);
	var screens = screenDetails || await FMOperateIndex.getPermissionAndScreenDetails();
	if (screens == null || screens.screens == null) {
		if (FMOperateIndex.GetCurrentOperateMonitorCount() > 1) {
			FMOperateIndex.lastAdditionalOperateScreenOpenFailures.push({ windowName: 'Screen2', screen: null });
		}

		return childWindows;
	}

	var secondaries = [];
	for (var i = 0; i < screens.screens.length; i++) {
		if (!screens.screens[i].isPrimary) {
			secondaries.push(screens.screens[i]);
		}
	}

	var screenNumber = 2;
	for (var j = 0; j < secondaries.length; j++) {
		var windowName = 'Screen' + screenNumber;
		if (screenNumber < firstScreenNumber
			|| screenNumber > lastScreenNumber
			|| (screenMask != null && !FMOperateIndex.IsScreenAssigned(screenMask, screenNumber))) {
			screenNumber++;
			continue;
		}

		screenNumber++;
		var childWindow = FMOperateIndex.OpenOperateWindowOnScreen(secondaries[j], windowName);
		if (childWindow == null) {
			FMOperateIndex.lastAdditionalOperateScreenOpenFailures.push({ windowName: windowName, screen: secondaries[j] });
		}
		else {
			FMOperateIndex.operateChildWindowsByName[windowName] = childWindow;
			childWindows.push(childWindow);
		}
	}

	return childWindows;
};

FMOperateIndex.OpenSavedAdditionalOperateWindows = FMOperateIndex.OpenSavedAdditionalOperateWindows || async function (screenDetails, showFailureMessage, firstScreenNumber, lastScreenNumber, screenMask) {
	var openedScreens = await FMOperateIndex.OpenAdditionalOperateWindows(screenDetails, firstScreenNumber, lastScreenNumber, screenMask);
	FMOperateIndex.screens = (FMOperateIndex.screens || []).concat(openedScreens);
	if (showFailureMessage === true && FMOperateIndex.lastAdditionalOperateScreenOpenFailures.length > 0 && typeof FMLayout !== 'undefined' && FMLayout.Alert) {
		FMLayout.Alert('Unable to open additional Operate Screens. Please allow pop-ups for this site and try again.', 'Displays are Extended', null);
	}

	return openedScreens;
};

FMOperateIndex.GetOperateMonitorState = FMOperateIndex.GetOperateMonitorState || function () {
	return new Promise(function (resolve) {
		$.ajax({
			url: 'GetOperateMonitorState',
			type: 'GET',
			cache: false,
			dataType: 'json',
			success: function (response) {
				var monitorState = { monitorCount: null, openAdditionalScreens: null, screenMask: 1, hasOperateScreenConfiguration: false, maxConfiguredMonitorIndex: null };
				try {
					FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
						if (!inError && data != null) {
							var parsedMonitorCount = parseInt(data.MonitorCount, 10);
							if (!isNaN(parsedMonitorCount)) {
								monitorState.monitorCount = parsedMonitorCount;
							}

							var parsedMaxConfiguredMonitorIndex = parseInt(data.MaxConfiguredMonitorIndex, 10);
							if (!isNaN(parsedMaxConfiguredMonitorIndex)) {
								monitorState.maxConfiguredMonitorIndex = parsedMaxConfiguredMonitorIndex;
							}

							if (data.OpenAdditionalScreens != null) {
								monitorState.openAdditionalScreens = data.OpenAdditionalScreens === true || data.OpenAdditionalScreens.toString().toLowerCase() === 'true';
							}

							var parsedScreenMask = parseInt(data.ScreenMask, 10);
							if (!isNaN(parsedScreenMask)) {
								monitorState.screenMask = FMOperateIndex.NormalizeOperateScreenMask(parsedScreenMask);
							}

							if (data.HasOperateScreenConfiguration != null) {
								monitorState.hasOperateScreenConfiguration = data.HasOperateScreenConfiguration === true || data.HasOperateScreenConfiguration.toString().toLowerCase() === 'true';
							}
						}
					});
				}
				catch (err) {
					console.error(err);
				}
				resolve(monitorState);
			},
			error: function () {
				resolve({ monitorCount: null, openAdditionalScreens: null, screenMask: 1, hasOperateScreenConfiguration: false, maxConfiguredMonitorIndex: null });
			}
		});
	});
};

FMOperateIndex.SetOperateMonitorState = FMOperateIndex.SetOperateMonitorState || function (monitorCount, openAdditionalScreens, mergeUnavailableScreens, screenMask) {
	return new Promise(function (resolve) {
		$.ajax({
			url: 'SetOperateMonitorState',
			type: 'POST',
			cache: false,
			contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
			dataType: 'json',
			headers: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
			data: {
				monitorCount: monitorCount,
				openAdditionalScreens: openAdditionalScreens,
				mergeUnavailableScreens: mergeUnavailableScreens === true,
				screenMask: screenMask == null ? null : FMOperateIndex.NormalizeOperateScreenMask(screenMask)
			},
			success: function (response) {
				var inErrorResult = false;
				try {
					FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
						inErrorResult = inError;
					});
				}
				catch (err) {
					console.error(err);
					inErrorResult = true;
				}
				resolve(!inErrorResult);
			},
			error: function () {
				resolve(false);
			}
		});
	});
};

FMOperateIndex.ShowOperateScreenIdentifier = FMOperateIndex.ShowOperateScreenIdentifier || function (screenNumber) {
	screenNumber = parseInt(screenNumber, 10);
	if (isNaN(screenNumber) || screenNumber < 1) {
		screenNumber = 1;
	}

	if (FMOperateIndex.operateScreenIdentifierTimer != null) {
		clearTimeout(FMOperateIndex.operateScreenIdentifierTimer);
	}

	$('.operate-screen-identifier').remove();

	var $identifier = $('<div class="operate-screen-identifier"></div>').text(screenNumber);
	$('body').append($identifier);

	// display the pop up for 5 seconds, fade out is currently 500 ms
	FMOperateIndex.operateScreenIdentifierTimer = setTimeout(function () {
		FMOperateIndex.operateScreenIdentifierTimer = null;
		$identifier.fadeOut(500, function () {
			$identifier.remove();
		});
	}, 5000);
};

FMOperateIndex.IdentifyOpenOperateScreens = FMOperateIndex.IdentifyOpenOperateScreens || function () {
	FMOperateIndex.ShowOperateScreenIdentifier(1);

	var childWindows = FMOperateIndex.operateChildWindowsByName || {};
	for (var windowName in childWindows) {
		if (!childWindows.hasOwnProperty(windowName)) {
			continue;
		}

		var screenMatch = /^Screen(\d+)$/i.exec(windowName);
		if (screenMatch == null) {
			continue;
		}

		var screenNumber = parseInt(screenMatch[1], 10);
		if (isNaN(screenNumber) || screenNumber < 2) {
			continue;
		}

		var childWindow = childWindows[windowName];
		if (childWindow == null || childWindow.closed === true) {
			continue;
		}

		try {
			if (childWindow.FMOperateIndex != null && childWindow.FMOperateIndex.ShowOperateScreenIdentifier != null) {
				childWindow.FMOperateIndex.ShowOperateScreenIdentifier(screenNumber);
			}
		}
		catch (err) {
			console.warn('Unable to identify Operate screen ' + windowName + '.', err);
		}
	}
};

FMOperateIndex.ShowOperateScreenAssignmentDialog = FMOperateIndex.ShowOperateScreenAssignmentDialog || function (screenDetails, currentMonitorCount, configuredScreenMask, openedScreenMask) {
	return new Promise(function (resolve) {
		if (typeof $ === 'undefined' || $.fn.dialog == null) {
			resolve({ saved: false });
			return;
		}

		currentMonitorCount = Math.max(1, parseInt(currentMonitorCount, 10) || 1);
		configuredScreenMask = FMOperateIndex.NormalizeOperateScreenMask(configuredScreenMask);
		openedScreenMask = FMOperateIndex.GetOpenOperateWindowScreenMask(openedScreenMask || 1, currentMonitorCount);

		var isResolved = false;
		var finish = function (result) {
			if (isResolved) {
				return;
			}

			isResolved = true;
			resolve(result);
		};

		var $dialog = $('<div class="operate-screen-assignments-dialog"></div>');
		$dialog.append($('<div class="operate-screen-assignment-title"></div>').text('Operate Screen Assignments'));

		for (var screenNumber = 1; screenNumber <= currentMonitorCount; screenNumber++) {
			var checkboxId = 'operateScreenAssignment' + screenNumber;
			var $checkbox = $('<input type="checkbox" />')
				.attr('id', checkboxId)
				.attr('data-screen-number', screenNumber)
				.prop('checked', FMOperateIndex.IsScreenAssigned(configuredScreenMask, screenNumber));
			if (screenNumber === 1) {
				$checkbox.prop('checked', true).prop('disabled', true);
			}

			$dialog.append(
				$('<label class="operate-screen-assignment-option"></label>')
					.attr('for', checkboxId)
					.append($checkbox)
					.append(document.createTextNode(' Screen ' + screenNumber)));
		}

		$dialog.dialog({
			title: 'Displays are Extended',
			modal: true,
			resizable: false,
			width: 360,
			dialogClass: 'dialog-title-icon-info',
			buttons: [
				{
					text: 'Identify',
					click: function () {
						FMOperateIndex.IdentifyOpenOperateScreens();
					}
				},
				{
					text: 'Save',
					click: async function () {
						var dialogElement = this;
						var screenMask = 1;
						$dialog.find('input[type=checkbox]').each(function () {
							var screenNumber = parseInt($(this).attr('data-screen-number'), 10);
							if (!isNaN(screenNumber) && $(this).prop('checked')) {
								var screenBit = FMOperateIndex.GetScreenBit(screenNumber);
								if (!FMOperateIndex.IsScreenAssigned(screenMask, screenNumber)) {
									screenMask += screenBit;
								}
							}
						});

						screenMask = FMOperateIndex.NormalizeOperateScreenMask(screenMask);
						$dialog.parent().find('button').prop('disabled', true);
						var isSaved = await FMOperateIndex.SetOperateMonitorState(currentMonitorCount, FMOperateIndex.HasAdditionalAssignedScreens(screenMask), false, screenMask);
						if (!isSaved) {
							$dialog.parent().find('button').prop('disabled', false);
							if (typeof FMLayout !== 'undefined' && FMLayout.Alert) {
								FMLayout.Alert('Unable to save Operate Screen Assignments.', 'Displays are Extended', null);
							}
							return;
						}

						FMOperateIndex.CloseUnassignedOperateWindows(screenMask, currentMonitorCount);
						var newAssignedScreenMask = FMOperateIndex.GetNewAssignedScreenMask(screenMask, openedScreenMask, currentMonitorCount);
						if (newAssignedScreenMask > 0) {
							await FMOperateIndex.OpenSavedAdditionalOperateWindows(screenDetails, true, 2, currentMonitorCount, newAssignedScreenMask);
						}

						finish({ saved: true, screenMask: screenMask });
						$(dialogElement).dialog('close');
					}
				},
				{
					text: 'Cancel',
					click: function () {
						finish({ saved: false });
						$(this).dialog('close');
					}
				}
			],
			close: function () {
				finish({ saved: false });
				$dialog.remove();
			}
		});
	});
};

FMOperateIndex.OpenOperateScreenAssignmentsFromMenu = FMOperateIndex.OpenOperateScreenAssignmentsFromMenu || async function () {
	if (!FMOperateIndex.IsOperateViewOnlyMode || !FMOperateIndex.IsOperateViewOnlyMode()) {
		return;
	}

	if (FMOperateIndex.GetOperateWindowName && FMOperateIndex.GetOperateWindowName() !== 'Screen1') {
		return;
	}

	var monitorState = await FMOperateIndex.GetOperateMonitorState();
	var expectedMonitorCount = FMOperateIndex.GetExpectedOperateMonitorCount(monitorState);
	var screenDetails = await FMOperateIndex.getPermissionAndScreenDetails();
	var detection = await FMOperateIndex.WaitForOperateMonitorDetection(expectedMonitorCount, screenDetails);
	var currentMonitorCount = detection.monitorCount;
	var configuredScreenMask = FMOperateIndex.NormalizeOperateScreenMask(monitorState.screenMask);
	var openedScreenMask = FMOperateIndex.GetOpenOperateWindowScreenMask(1, currentMonitorCount);
	await FMOperateIndex.ShowOperateScreenAssignmentDialog(detection.screenDetails, currentMonitorCount, configuredScreenMask, openedScreenMask);
};

FMOperateIndex.openMultipleScreens = FMOperateIndex.openMultipleScreens || async function () {
	if (FMOperateIndex.IsOperateViewOnlyMode && !FMOperateIndex.IsOperateViewOnlyMode()) {
		return;
	}

	if (FMOperateIndex.GetOperateWindowName() !== 'Screen1') {
		return;
	}

	var monitorState = await FMOperateIndex.GetOperateMonitorState();
	var screenDetails = await FMOperateIndex.getPermissionAndScreenDetails();
	var expectedMonitorCount = FMOperateIndex.GetExpectedOperateMonitorCount(monitorState);
	var detection = await FMOperateIndex.WaitForOperateMonitorDetection(expectedMonitorCount, screenDetails);
	var currentMonitorCount = detection.monitorCount;
	screenDetails = detection.screenDetails;
	var configuredScreenMask = FMOperateIndex.NormalizeOperateScreenMask(monitorState.screenMask);
	var assignedAvailableScreenMask = FMOperateIndex.GetAssignedAvailableScreenMask(configuredScreenMask, currentMonitorCount);
	var configuredMonitorCount = FMOperateIndex.GetHighestAssignedScreenNumber(configuredScreenMask);
	var availableConfiguredMonitorCount = Math.min(configuredMonitorCount, currentMonitorCount);
	var shouldRestoreConfiguredAdditionalScreens = FMOperateIndex.HasAdditionalAssignedScreens(assignedAvailableScreenMask);
	var hasAvailableMonitorCountChanged = FMOperateIndex.HasAvailableMonitorCountChanged(monitorState, currentMonitorCount);
	console.log('Operate monitor restore state: current=' + currentMonitorCount
		+ ', saved=' + monitorState.monitorCount
		+ ', openAdditional=' + monitorState.openAdditionalScreens
		+ ', maxConfigured=' + monitorState.maxConfiguredMonitorIndex
		+ ', screenMask=' + configuredScreenMask);

	if (currentMonitorCount < expectedMonitorCount) {
		if (detection.isReliable) {
			await FMOperateIndex.SetOperateMonitorState(currentMonitorCount, shouldRestoreConfiguredAdditionalScreens, true, assignedAvailableScreenMask);
			if (shouldRestoreConfiguredAdditionalScreens) {
				await FMOperateIndex.OpenSavedAdditionalOperateWindows(screenDetails, true, 2, currentMonitorCount, assignedAvailableScreenMask);
			}

			if (hasAvailableMonitorCountChanged) {
				Promise.resolve(FMOperateIndex.ShowOperateScreenAssignmentDialog(screenDetails, currentMonitorCount, assignedAvailableScreenMask, FMOperateIndex.GetOpenOperateWindowScreenMask(1, currentMonitorCount))).catch(function (err) {
					console.error(err);
				});
			}
		}

		return;
	}

	if (shouldRestoreConfiguredAdditionalScreens) {
		if (monitorState.openAdditionalScreens !== true || monitorState.hasOperateScreenConfiguration !== true || hasAvailableMonitorCountChanged) {
			await FMOperateIndex.SetOperateMonitorState(currentMonitorCount, true, false, configuredScreenMask);
		}

		await FMOperateIndex.OpenSavedAdditionalOperateWindows(screenDetails, true, 2, availableConfiguredMonitorCount, assignedAvailableScreenMask);
	}
	else if (hasAvailableMonitorCountChanged) {
		await FMOperateIndex.SetOperateMonitorState(currentMonitorCount, false, false, configuredScreenMask);
	}

	if (hasAvailableMonitorCountChanged) {
		Promise.resolve(FMOperateIndex.ShowOperateScreenAssignmentDialog(screenDetails, currentMonitorCount, configuredScreenMask, FMOperateIndex.GetOpenOperateWindowScreenMask(1, currentMonitorCount))).catch(function (err) {
			console.error(err);
		});
	}
};

FMOperateIndex.EnsureScrollingTabs = function (tabSelector) {
	var $tabList = $(tabSelector);
	if ($tabList.length === 0 || !$.fn.scrollingTabs) {
		return false;
	}

	if ($tabList.data('scrtabs') == null) {
		var $scrollerContainer = $tabList.closest('.scrtabs-tab-container');
		if ($scrollerContainer.length > 0) {
			$scrollerContainer.replaceWith($tabList);
		}

		$tabList.scrollingTabs({ scrollToTabEdge: true });
	}

	return $tabList.data('scrtabs') != null;
};

FMOperateIndex.RefreshScrollingTabs = function (tabSelector) {
	try {
		if (FMOperateIndex.EnsureScrollingTabs(tabSelector)) {
			$(tabSelector).scrollingTabs('refresh');
		}
	}
	catch (err) {
		console.error('Operate tab strip refresh failed for ' + tabSelector + '.', err);
	}
};

FMOperateIndex.EnsureOperateTabInfrastructure = function () {
	FMOperateIndex.EnsureScrollingTabs('#tabList');
	if (FMOperateIndex.isTabGroupEnabled
		&& $('#tabList .tab-add-group').length === 0
		&& typeof FMOperateIndex.initializeTabGroup === 'function') {
		FMOperateIndex.initializeTabGroup();
		FMOperateIndex.RefreshScrollingTabs('#tabList');
	}
};

FMOperateIndex.StartPersistedControlRestore = function (source) {
	if (FMOperateIndex.hasStartedPersistedControlRestore === true) {
		return;
	}

	FMOperateIndex.hasStartedPersistedControlRestore = true;
	var startRestore = function () {
		console.log('Operate starting persisted control restore from ' + source + ' for ' + FMOperateIndex.GetOperateWindowName());
		try {
			FMOperateIndex.EnsureOperateTabInfrastructure();
		}
		catch (err) {
			console.error('Operate tab infrastructure initialization failed before restore.', err);
		}
		try {
			FMOperateIndex.restoringView = true;
			FMOperateIndex.RestorePersistedControls();
		}
		catch (err) {
			FMOperateIndex.restoringView = false;
			window.localStorage.setItem('operateBeingRestored', "false");
			console.error('Operate persisted control restore failed.', err);
		}
	};

	if (FMOperateIndex.operateMonitorDetectionPromise != null
		&& typeof FMOperateIndex.operateMonitorDetectionPromise.then === 'function') {
		FMOperateIndex.operateMonitorDetectionPromise.then(startRestore, function (err) {
			console.error('Operate monitor detection failed before restore.', err);
			startRestore();
		});
		return;
	}

	startRestore();
};

FMOperateIndex.IsOperateViewOnlyMode = function () {
	return FMOperateIndex.isOperateViewOnlyMode === true
		|| (FMOperateIndex.isOperateViewOnlyMode != null
			&& FMOperateIndex.isOperateViewOnlyMode.toString().toLowerCase() === 'true');
};

FMOperateIndex.StartFallbackPersistedControlRestore = function () {
	if (FMOperateIndex.hasStartedPersistedControlRestore === true) {
		return;
	}

	FMOperateIndex.fallbackPersistedControlRestoreWaitCount = FMOperateIndex.fallbackPersistedControlRestoreWaitCount || 0;
	if (FMOperateIndex.inlineStartupStarted === true
		&& FMOperateIndex.inlineStartupCompleted !== true
		&& FMOperateIndex.fallbackPersistedControlRestoreWaitCount < 10) {
		FMOperateIndex.fallbackPersistedControlRestoreWaitCount += 1;
		setTimeout(FMOperateIndex.StartFallbackPersistedControlRestore, 1000);
		return;
	}

	console.warn('Operate inline restore did not start. Starting fallback restore.');
	if (FMOperateIndex.IsOperateViewOnlyMode()) {
		FMOperateIndex.operateMonitorDetectionPromise = Promise.resolve(FMOperateIndex.openMultipleScreens()).catch(function (err) {
			console.error(err);
		});
	}

	FMOperateIndex.StartPersistedControlRestore('external fallback');
};

FMOperateIndex.InitStatisticsWebWorker = function (path) {
	FMOperateIndex.statisticsWebWorker = new Worker(path);
	FMOperateIndex.statisticsWebWorker.postMessage({ messageType: "startup", url: $("#urlSaveStatistics").val(), refreshTimeout: FMOperateIndex.statisticsRefreshFrequency, windowName: FMOperateIndex.GetOperateWindowName ? FMOperateIndex.GetOperateWindowName() : window.name, "requestVerificationToken": $('input[name=__RequestVerificationToken]').val() });

	FMOperateIndex.statisticsWebWorker.onmessage = function (event) {
		//console.log(event.message, event);
		if (event.data.command == "GetStatistics") {
			FMOperateIndex.statisticsWebWorker.postMessage({ messageType: "statistics alarmSummaryTab", statistics: AlarmSummaryTab.GetStatistics() });
			FMOperateIndex.statisticsWebWorker.postMessage({ messageType: "statistics dynamicPointGroup", statistics: FMOperateIndex.GetStatistics() });

			if (FMOperateIndex.alarmWebWorker) {
				FMOperateIndex.alarmWebWorker.postMessage({ messageType: "getStatistics" });
			}
			else {
				FMOperateIndex.statisticsWebWorker.postMessage({ messageType: "statistics alarmNotification", statistics: AlarmSummaryTab.GetStatistics() });
			}

			FMOperateIndex.tagWebWorker.postMessage({ type: "getStatistics" });
		}
		else if (event.data.messageType && event.data.messageType == "Loss of server connection") {
			window.alert("Cannot Communicate with Server! Press OK to Retry and return to Log In Screen.");
			window.location.href = "~/FMWebApp/LogoutForm.aspx";
		}
	};

	FMOperateIndex.statisticsWebWorker.onerror = function (event) {
		console.log(event.message, event);
	};
}

FMOperateIndex.InitAlarmWebWorker = function (path) {
	if ((FMOperateIndex.GetOperateWindowName ? FMOperateIndex.GetOperateWindowName() : window.name) === 'Screen1' && $( "#ViewAlarmSummaryRight" ).val().toLowerCase() === "true" )
	{
		FMOperateIndex.alarmWebWorker = new Worker( path );
		FMOperateIndex.alarmWebWorker.postMessage( { "url": $( "#urlAlarmCheck" ).val(), "refreshTimeout": FMOperateIndex.alarmRefreshFrequency, "requestVerificationToken": $( 'input[name=__RequestVerificationToken]' ).val() } );

		FMOperateIndex.alarmWebWorker.onmessage = function( event )	{
			if (event.data.messageType && event.data.messageType == "alarmNotificationStatisticsResults") {
				FMOperateIndex.statisticsWebWorker.postMessage({ messageType: "statistics alarmNotification", statistics: event.data.statistics });
			}
			else if (event.data.messageType && event.data.messageType == "Loss of server connection") {
				window.alert("Cannot Communicate with Server! Press OK to Retry and return to Log In Screen.");
				window.location.href = "~/FMWebApp/LogoutForm.aspx";
			} else {
				var audio = document.getElementById("alarm-notification-audio");
				if (event.data) {
					if (event.data.AlarmDetail && event.data.AlarmDetail.AlarmSummaries.length > 0) {
						var alarm = event.data.AlarmDetail.AlarmSummaries[0];
						// open/update modal 
						audio.pause();
						try {
							if (alarm.SoundFile) {
								$("#alarm-notification-audio-alarm").attr("src", $("#alarm-notification-audio").attr("data-default-sound-path") + alarm.SoundFile);
							}
							else {
								$("#alarm-notification-audio-alarm").attr("src", $("#alarm-notification-audio").attr("data-default-audio"));
							}
						}
						catch (e) {
						}

						if (!alarm.Silenced) {
							if (event.data.AlarmDetail.HasSilenceRight) {
								$("#alarm-notification-silence-button").removeClass("disabled");
							}
							else {
								$("#alarm-notification-silence-button").removeClass("disabled").addClass("disabled");
							}

							try {
								audio.src = $("#alarm-notification-audio-alarm").attr("src");
							}
							catch (e) {
							}

							try {
								audio.load();
								var playPromise = audio.play();
								if (playPromise !== undefined) {
									playPromise.then(function () {
										// Automatic playback started!
									}).catch(function (error) {
										// Automatic playback failed.
										audio.src = $("#alarm-notification-audio").attr("data-default-audio");
										audio.load();
										audio.play();
									});
								}
								else // ie does not support promises and we need a different type of check
								{
									audio.onerror = function () {
										// Automatic playback failed.
										audio.src = $("#alarm-notification-audio").attr("data-default-audio");
										audio.load();
										audio.play();

									}
								}
							}
							catch (e) {
								audio.src = $("#alarm-notification-audio").attr("data-default-audio");
								audio.load();
								audio.play();
							}
						}
						else {
							$("#alarm-notification-silence-button").removeClass("disabled").addClass("disabled");
						}

						if (alarm.Acknowledge) {
							$("#alarm-notification-acknowledge-button").removeClass("disabled");
						}
						else {
							$("#alarm-notification-acknowledge-button").removeClass("disabled").addClass("disabled");
						}

						//alarm.SoundFile
						$("#alarm-notification-alarm-count").text(event.data.NumberOfAlarms);
						$("#alarm-notification-site").text(alarm.SiteID);
						$("#alarm-notification-point").text(alarm.PointID);
						$("#alarm-notification-tag").text(alarm.TagID);
						$("#alarm-notification-alarm").text(alarm.Status);
						$("#alarm-notification-priority").text(alarm.AlarmPriorityID);
						$("#alarm-notification-time").text(alarm.FormattedTimestamp);
						$("#alarm-notification-body-details").attr("data-alarm-guid", alarm.AlarmGuid);
						$("#alarm-notification-body-details").attr("data-point-guid", alarm.PointGuid);
						if (alarm.IsNormal) {
							$("#alarm-notification-detail").css("color", "#" + alarm.NormalUnacknowledgedAlarmTextSteadyColor);
							$("#alarm-notification-detail").css("background-color", "#" + alarm.NormalUnacknowledgedAlarmBackgroundSteadyColor);
						}
						else {
							$("#alarm-notification-detail").css("color", "#" + alarm.AlarmTextSteadyColor);
							$("#alarm-notification-detail").css("background-color", "#" + alarm.AlarmBackgroundSteadyColor);
						}

						$("#alarm-notification").removeClass("hidden");
					}
					else {
						// close modal
						$("#alarm-notification").removeClass("hidden").addClass("hidden");
						audio.pause();
					}
				}
				else {
					audio.pause();
				}
			}
		};

		FMOperateIndex.alarmWebWorker.onerror = function( event )
		{
			console.log( event.message, event );
		};
	}
}

FMOperateIndex.InitTagWebWorker = function( path )
{
	FMOperateIndex.tagWebWorker = new Worker(path);
	console.log("OperateIndex.js InitTagWebWorker path = " + path);
	console.log("OperateIndex.js InitTagWebWorker UpdateTagURL = " + $("#UpdateTagURL").val());
	FMOperateIndex.tagWebWorker.postMessage({ "type": 'configuration', "url": $("#UpdateTagURL").val(), "refreshTimeout": FMOperateIndex.tagRefreshFrequency, "requestVerificationToken": $('input[name=__RequestVerificationToken]').val(), "siteTimeZone": $("#SiteTimeZone").val() });

	FMOperateIndex.tagWebWorker.onmessage = function (event)
	{
		if (event.data.messageType && event.data.messageType == "tagUpdateValuesStatisticsResults") {
			FMOperateIndex.statisticsWebWorker.postMessage({ messageType: "statistics tagUpdateValues", statistics: event.data.statistics });
		} else if (Array.isArray(event.data)) {
			// we are supposed to get an array of subscriptions with the results that we are supposed to pass to the callback
			if (event.data.length > 0) {
				$.map(event.data, function (returnSubcriptionValue, i) {
					// for each subscription call the callback function passing the result as a parameter
					var foundSubscriptions = $.grep(FMOperateIndex.tagWebWorkerSubscriptions, function (obj) {
						return obj.id === returnSubcriptionValue.id;
					});

					if (foundSubscriptions.length > 0) {
						// if we found a subcription that matches the one received then call the callback function
						$.map(foundSubscriptions, function (val, i) {
							if (val.callback && (val.refreshAllTags || returnSubcriptionValue.tagList.length > 0)) {
								val.callback(returnSubcriptionValue.tagList);
							}
						});
					}
				});
			}
		}
		else {
			if (event.data.name != 'download-link') { 
			var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };
			// remove previous notifications
			PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

			FMErrorAndExceptionHandling.ShowError(event.data, function () { }, messageAttributes);
			}
		}
	};

	FMOperateIndex.tagWebWorker.onerror = function (event) {
		console.log(event.message, event);
	};

	FMOperateIndex.tagWebWorker.addEventListener("message", function (e) {
		switch (e.data.name) {
			case "download-link": {
				if (e.data.error) {
					console.error("Download error: ", e.data.error);
				}
				else {
					 const link = document.createElement("a");
					link.style.visibility = 'hidden';
					link.download = "data.csv";
					link.href = e.data.link;
					link.appendChild(new Text("Download data"));
					link.click();
					document.body.appendChild(link); 
				}
				break;
			}
			default:
		}
	});

	FMOperateIndex.ResubscribeTagWebWorkerSubscriptions();

}

FMOperateIndex.ResubscribeTagWebWorkerSubscriptions = function () {
	if (!FMOperateIndex.tagWebWorker) {
		return;
	}

	$.each(FMOperateIndex.tagWebWorkerSubscriptions, function (index, subscription) {
		FMOperateIndex.tagWebWorker.postMessage({
			"type": 'subscribe',
			"id": subscription.id,
			"tagList": subscription.tagList,
			"refreshAllTags": subscription.refreshAllTags
		});
	});
};

FMOperateIndex.subscribeTagWebWorker = function (id, tagList, refreshFunction, refreshAllTags)
{

	if ( !refreshAllTags )
	{
		refreshAllTags = false;
	}


	// make sure we are unsubscribing from the dynamic point group subscription in case we switch from static to dynamic group
	FMOperateIndex.unsubscribeDynamicPointGroup(id);

	// create a new subscription or update an existing one if already defined
	var findSubscriptions = $.grep(FMOperateIndex.tagWebWorkerSubscriptions, function (obj) {
		return obj.id === id;
	});

	if ( findSubscriptions.length > 0 )
	{
		// if we found a subcription that matches the one received then update the definition
		$.map(findSubscriptions, function (subscription, i)
		{
			subscription.tagList = tagList;
			subscription.callback = refreshFunction;
		});
	}
	else // if we don't have a subscription then add 
	{
		FMOperateIndex.tagWebWorkerSubscriptions.push({ "id": id, "tagList": tagList, "callback": refreshFunction, "refreshAllTags": refreshAllTags });
	}
	if (FMOperateIndex.tagWebWorker)
	FMOperateIndex.tagWebWorker.postMessage({ "type": 'subscribe', "id": id, "tagList": tagList, "refreshAllTags": refreshAllTags });
}

FMOperateIndex.unsubscribeTagWebWorker = function (id) {

	FMOperateIndex.tagWebWorkerSubscriptions = jQuery.grep(FMOperateIndex.tagWebWorkerSubscriptions, function (obj) {
		return obj.id !== id;
	});
	if ( FMOperateIndex.tagWebWorker )
		FMOperateIndex.tagWebWorker.postMessage({ "type": 'unsubscribe', "id": id });
}

FMOperateIndex.subscribeDynamicPointGroup = function (id) {
	// make sure we are unsubscribing from the tag list subscription
	FMOperateIndex.unsubscribeTagWebWorker(id);

	// create a new subscription or update an existing one if already defined
	var findSubscriptions = $.grep(FMOperateIndex.pointWebWorkerSubscriptions, function (obj) {
		return obj.id === id;
	});

	if (findSubscriptions.length > 0) {
		// do nothing since we already have the id
	}
	else // if we don't have a subscription then add 
	{
		FMOperateIndex.pointWebWorkerSubscriptions.push({ "id": id});
	}
}

FMOperateIndex.unsubscribeDynamicPointGroup = function (id) {

	FMOperateIndex.pointWebWorkerSubscriptions = jQuery.grep(FMOperateIndex.pointWebWorkerSubscriptions, function (obj) {
		return obj.id !== id;
	});

}

FMOperateIndex.isSubscribedDynamicPointGroup = function( id )
{
	// make sure we are unsubscribing from the tag list subscription
	FMOperateIndex.unsubscribeTagWebWorker( id );

	// create a new subscription or update an existing one if already defined
	var findSubscriptions = $.grep( FMOperateIndex.pointWebWorkerSubscriptions, function( obj )
	{
		return obj.id === id;
	});
	return ( findSubscriptions.length > 0 );
}

// Persist tabs after adding a new one
FMOperateIndex.PersistScreenConfiguration = function(asyncRequest)
{
	if (FMOperateIndex.skipPersistScreenConfiguration === true) {
		return;
	}

	// if we are printing the pointgroup no need to save
	if (FMOperateIndex.pointgroupreportgeneration != "") {
		return;
	}

	//no need to save while still restoring the initial configuration, there will be a save after the screen finished loading

	if (FMOperateIndex.restoringSaveInitialConfig === false)
	{
		return;
	}
	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };
	var useAsyncRequest = asyncRequest !== false;

	$.ajax( {
		url: 'SetScreenConfiguration',
		type: 'Post',
		async: useAsyncRequest,
		dataType: 'json',
		data: '{configuration:\'' + JSON.stringify(FMOperateIndex.contents) + '\', windowName: \'' + (FMOperateIndex.GetOperateWindowName ? FMOperateIndex.GetOperateWindowName() : window.name) + '\'}',
		success: function( data )
		{
			PNotify.removeStack(messageAttributes.stack);
			FMErrorAndExceptionHandling.HandleMessages(data, function()
			{
			}, messageAttributes );
		},
		error: function( request, status, error )
		{
			// remove previous notifications
			PNotify.removeStack(messageAttributes.stack);
			if (FMOperateIndex.LoggingOut === false) {
				FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
				}, messageAttributes);
			}
			else {
				console.log("**********************  Logging out ******************  ignore error  :  " + error);
			}
		}
	} );
};

FMOperateIndex.PersistOpenScreenConfigurations = function()
{
	if (FMOperateIndex.skipPersistScreenConfiguration === true) {
		return;
	}

	if (FMOperateIndex.contents != null) {
		console.log("FMOperateIndex.PersistScreenConfiguration called from FMOperateIndex.PersistOpenScreenConfigurations 1");
		FMOperateIndex.PersistScreenConfiguration(false);
	}

	if (FMOperateIndex.screens && FMOperateIndex.screens.length > 0) {
		for (let screen of FMOperateIndex.screens) {
			try {
				if (screen != null
					&& screen.closed !== true
					&& screen.FMOperateIndex != null
					&& screen.FMOperateIndex.skipPersistScreenConfiguration !== true
					&& screen.FMOperateIndex.contents != null) {
					console.log("FMOperateIndex.PersistScreenConfiguration called from FMOperateIndex.PersistOpenScreenConfigurations 2");
					screen.FMOperateIndex.PersistScreenConfiguration(false);
				}
			}
			catch (err) {
				console.error(err);
			}
		}
	}
};
FMOperateIndex.Logout = function(logoutPage)
{
	FMOperateIndex.PersistOpenScreenConfigurations();
	FMOperateIndex.LoggingOut = true;
	top.location.href = logoutPage;
	return false;
};

// Persist tabs after adding a new one
FMOperateIndex.PersistNewControl = function( parentId, newControlId, newName, newType, settings )
{
	// if we are adding to the main tab
	if ( parentId === 'mainTab' )
	{
		
		FMOperateIndex.contents.push( { "id": newControlId, "name": newName, "type": newType, active: true, "settings": settings } );
	}
	else
	{
		var parentGroup = $.grep( FMOperateIndex.contents, function( e )
		{
				return e.id === parentId;
		} );
		if ( parentGroup.length > 0 )
		{
			parentGroup[0].settings.push( { "id": newControlId, "name": newName, "type": newType, active: true, "settings": settings } );
		}
	}
	if ( FMOperateIndex.restoringView === false )
	{
		console.log("FMOperateIndex.PersistScreenConfiguration called from FMOperateIndex.PersistNewControl");
		FMOperateIndex.PersistScreenConfiguration();
	}
};

// update the persisted storage of the tabs when a tab gets set to active
FMOperateIndex.PersistUpdateActiveControl = function( parentId, controlId, settings )
{
	if ( parentId === 'mainTab' )
	{
		FMOperateIndex.contents = $.map( FMOperateIndex.contents, function( obj )
		{
				if ( obj.id === controlId )
				{
					obj.active = true;
					if ( settings )
					{
						obj.settings.position = settings.position;
						obj.settings.scale = settings.scale;
					}
				}
				else
				{
					obj.active = false;
				}
				return obj;
		} );
	}
	else
	{
		var subTabs = [];
		
		var parentGroupIdx = $.map( FMOperateIndex.contents, function( obj, index )
		{
				if ( obj.id === parentId )
				{
					return 1;
				}
				return 0;
		} );
		
		var foundTabGroupIdx = parentGroupIdx.indexOf(1);
		if (foundTabGroupIdx === -1)
		{
			foundTabGroupIdx = parentGroupIdx.indexOf(0);
		}
		if (foundTabGroupIdx >= 0)
		{
			if (FMOperateIndex.contents[foundTabGroupIdx]
				&& FMOperateIndex.contents[foundTabGroupIdx].settings
				&& FMOperateIndex.contents[foundTabGroupIdx].settings.length > 0) {
				//var xyz = FMOperateIndex.contents[foundTabGroupIdx].settings[0];
				subTabs = $.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj, i) {
					if (obj.id === controlId) {
						obj.active = true;
						if (settings) {
							obj.settings.position = settings.position;
							obj.settings.scale = settings.scale;
						}
						if ((obj.settings) && (obj.settings.drawingIndex))
							FMDrawIndex.UpdateactiveCanvas(obj.settings.drawingIndex);
						if (obj.type === "trend")
							FMTrendIndex.refreshTrend(obj.settings.drawingIndex);

						FMOperateIndex.UpdateOperateHelpKey(obj.type);
						if (obj.settings.pointGuid && obj.type == 'drawing')
							FMOperateIndex.UpdateOperateHelpKey('point');
					}
					else
					{
						obj.active = false;
					}
					return obj;
				} );
			}
		}

		if (foundTabGroupIdx >= 0 && subTabs.length > 0)
		{
			FMOperateIndex.contents[foundTabGroupIdx].settings = subTabs;

			// Change the Trend tab image from black to white
			for (var nextTabIndex = 0; nextTabIndex < subTabs.length; nextTabIndex++)
			{
				var tabType = subTabs[nextTabIndex].type;
				var tabActive = subTabs[nextTabIndex].active;
				var tabId = subTabs[nextTabIndex].id;

				if (tabType === 'trend')
				{
					var imageId = "TrendTabImageId-" + tabId;
					var imageTagObj = document.getElementById(imageId);

					if (imageTagObj != null)
					{
						if (tabActive)
						{
							imageTagObj.src = window.applicationRootName + "/FMWebApp/images/trendNoBorder.png";
						}
						else
						{
							imageTagObj.src = window.applicationRootName +  "/FMWebApp/images/trendNoBorderInactive.png";
                        }
                    }
                }
            }
		}
	}

	if ( FMOperateIndex.restoringView === false )
	{
		console.log("FMOperateIndex.PersistScreenConfiguration called from FMOperateIndex.PersistUpdateActiveControl");
		FMOperateIndex.PersistScreenConfiguration();
	}
};

FMOperateIndex.UpdateOperateHelpKey= function (tabType)
{
	switch (tabType) {
		case "alarmHistory":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexAlarmHistoryTab";
			break;
		case "alarmSummary":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexAlarmsTab";
			break;
		case "drawing":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexGraphicsTab";
			break;
		case "pointGroup":
		case "group":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexPointGroupsTab";
			break;
		case "point":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexPointsTab";
			break;
		case "report":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexReportsTab";
			break;
		case "trend":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexTrendsTab";
			break;
		case "pointHistory":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexPointHistory";
			break;
		case "pointCalculator":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexPointCalculator";
			break;
		case "movementSummary":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexMovementSummaryTab";
			break;
		case "movementHistory":
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/Operate/OperateIndexMovementHistoryTab";
			break;
	}
}


// update the persisted storage of the tabs when a tab gets set to active
FMOperateIndex.PersistUpdateDrawingSettings = function (parentId, controlId, settings) {

	if (parentId === 'mainTab') {
		FMOperateIndex.contents = $.map(FMOperateIndex.contents, function (obj) {
			if (obj.id === controlId) {
				if (settings) {
					obj.settings = settings;
				}
			}
			return obj;
		});
	}
	else {
		var subTabs = [];
		
		var parentGroupIdx = $.map(FMOperateIndex.contents, function (obj, index) {
			if (obj.id === parentId) {
				return 1;
			}
			return 0;
		});
		
		var foundTabGroupIdx = parentGroupIdx.indexOf(1);
		if (foundTabGroupIdx === -1)
		{
			foundTabGroupIdx = parentGroupIdx.indexOf(0);
		}
		if (foundTabGroupIdx >= 0) {
			subTabs = $.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj) {
				if (obj.id === controlId) {
					if (settings) {
						obj.settings = settings;
					}
				}
				return obj;
			});
		}

		if (foundTabGroupIdx >= 0 && subTabs.length > 0) {
			FMOperateIndex.contents[foundTabGroupIdx].settings = subTabs;
		}
	}
	
	if (FMOperateIndex.restoringView === false) {
		console.log("FMOperateIndex.PersistScreenConfiguration called from FMOperateIndex.PersistUpdateDrawingSettings");
		FMOperateIndex.PersistScreenConfiguration();
	}
};

// update the persisted storage of the tabs when a tab name change
FMOperateIndex.PersistUpdateNameControl = function( parentId, controlId, newName )
{
	var found;
	if ( parentId === 'mainTab' )
	{
		FMOperateIndex.contents = $.map( FMOperateIndex.contents, function( obj )
		{
				if ( obj.id === controlId )
				{
					obj.name = newName;
					found = true;
				}
				return obj;
		} );
	}
	else {
		var subTabs = [];

		var parentGroupIdx = $.map(FMOperateIndex.contents, function (obj, index) {
			if (obj.id === parentId) {
				return 1;
			}
			return 0;
		});

		var foundTabGroupIdx = parentGroupIdx.indexOf(1);
		if (foundTabGroupIdx >= 0) {
			subTabs = $.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj) {
				if (obj.id === controlId) {
						obj.name = newName;
				}
				return obj;
			});
		}

	}
	console.log("FMOperateIndex.PersistScreenConfiguration called from FMOperateIndex.PersistUpdateNameControl");
	FMOperateIndex.PersistScreenConfiguration();
};

// function to update filters parameters
FMOperateIndex.updateFilterParameters = function (grid, metadata) {
	var dataView = grid.getData();
	var columns = grid.getColumns();
	dataView.setFilterArgs({
		columnsToFilter: columns,
		metadata: metadata
	});
}


// update the persisted storage of the filter views in the menu
FMOperateIndex.PersistUpdateFilterView = function( controlId, newValue )
{
	var found = false;
	FMOperateIndex.contents = $.map( FMOperateIndex.contents, function( obj )
	{
		if ( obj.id === controlId )
		{
				obj.name = newValue;
				found = true;
		}
		return obj;
	} );
	if ( found === false )
	{
		
		FMOperateIndex.contents.push( { "id": controlId, "name": newValue, "type": 'filter_view', active: true, "settings": {} } );
	}
	if ( FMOperateIndex.restoringView === false )
	{
		console.log("FMOperateIndex.PersistScreenConfiguration called from FMOperateIndex.PersistUpdateFilterView");
		FMOperateIndex.PersistScreenConfiguration();
	}

};

// Persist screen display after removing a tab
FMOperateIndex.RemovePersistedControl = function( parentId, controlId )
{
	if ( parentId === 'mainTab' )
	{
		FMOperateIndex.contents = $.grep( FMOperateIndex.contents, function( e )
		{
				return e.id !== controlId;
		} );
	}
	else
	{

		var elementIdx = -1;
		var parentGroupIdx = -1;

		$.map(FMOperateIndex.contents, function (obj, index)
		{
			if (obj.id === parentId)
				{
					parentGroupIdx = index;
					return index;
				}
				return 0;
		} );

		if ( parentGroupIdx > -1 )
		{
			$.map( FMOperateIndex.contents[parentGroupIdx].settings, function( obj, index )
			{
				if ( obj.id === controlId )
				{
					elementIdx = index;
					return index;
				}
				return 0;
			} );
		}
		if (parentGroupIdx > -1 && elementIdx > -1)
		{
				FMOperateIndex.contents[parentGroupIdx].settings.splice( elementIdx, 1 );
		}
	}
	console.log("FMOperateIndex.PersistScreenConfiguration called from FMOperateIndex.RemovePersistedControl");
	FMOperateIndex.PersistScreenConfiguration();
};

// get the Alarm Priorities configured for the current site (colors)
FMOperateIndex.GetAlarmPriorities = function()
{
	var alarmPriorities = [];
	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };
	$.ajax({
		url: 'GetAlarmPriorities',
		type: 'Get',
		cache: false,
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function () {
				alarmPriorities = response.Data;

				// create classes for each alarm priority
				if (alarmPriorities && alarmPriorities.length > 0) {
					var dynamicStyleSheetAlarms = $("<style type='text/css' rel='stylesheet' />").appendTo($("head"));
					var rules = [];

					for (var i = 0; i < alarmPriorities.length; i++) {
						//if (i === 0)
						//{
						//	rules.push(".tankstatus" + " { background-color: red" + "; color: blue" + "; padding-left: 3px; padding-right: 3px; }");
						//}
						rules.push(".AlarmPriority-" + alarmPriorities[i].IdentityGuid + " { background-color: #" + alarmPriorities[i].BackgroundSteady + "; color: #" + alarmPriorities[i].TextSteady + "; padding-left: 3px; padding-right: 3px; }");
						rules.push(".AlarmPriority-blink-" + alarmPriorities[i].IdentityGuid + " { padding-left: 3px; padding-right: 3px; animation: anim-" + alarmPriorities[i].IdentityGuid + " 2s infinite}");
						rules.push("@keyframes anim-" + alarmPriorities[i].IdentityGuid + " { 0% {background-color: #" + alarmPriorities[i].BackgroundSteady + "; color: #" + alarmPriorities[i].TextSteady + "} 29% {background-color: #" + alarmPriorities[i].BackgroundSteady + "; color: #" + alarmPriorities[i].TextSteady + "} 30% {background-color: #" + alarmPriorities[i].BackgroundAlternate + "; color: #" + alarmPriorities[i].TextAlternate + "} 100% {background-color: #" + alarmPriorities[i].BackgroundAlternate + ";color: #" + alarmPriorities[i].TextAlternate + "}}");
					}

					if (dynamicStyleSheetAlarms[0].styleSheet) { // IE
						dynamicStyleSheetAlarms[0].styleSheet.cssText = rules.join(" ");
					} else {
						dynamicStyleSheetAlarms[0].appendChild(document.createTextNode(rules.join(" ")));
					}

				}
			}, messageAttributes);
		},
		error: FMErrorAndExceptionHandling.ShowException
		
	});
}

FMOperateIndex.GetTankStatusColors = function () {
	var tankStatusColorsCol = [];
	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };
	$.ajax({
		url: 'GetTankStatusColors',
		type: 'Get',
		cache: false,
		async: false,
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function () {
				tankStatusColorsCol = response.Data;
				// create classes for each alarm priority
				if (tankStatusColorsCol && tankStatusColorsCol.length === 1) {
					var ts = tankStatusColorsCol[0];
					var dynamicStyleSheetAlarms = $("<style type='text/css' rel='stylesheet' />").appendTo($("head"));
					var rules = [];
					if (ts.Enabled === "1") {
						if (ts.EmptyingBackground !== "" &&  ts.EmptyingText !== "")
							rules.push(".tankstatusEmptying" + " { background-color: " + ts.EmptyingBackground + "; color: " + ts.EmptyingText + "; padding-left: 3px; padding-right: 3px; }");
						if (ts.FillingBackground !== "" && ts.FillingText !== "")
							rules.push(".tankstatusFilling" + " { background-color: " + ts.FillingBackground + "; color: " + ts.FillingText + "; padding-left: 3px; padding-right: 3px; }");
						if (ts.RunningBackground !== "" && ts.RunningText !== "")
							rules.push(".tankstatusRunning" + " { background-color: " + ts.RunningBackground + "; color: " + ts.RunningText + "; padding-left: 3px; padding-right: 3px; }");
						if (ts.StoppedBackground !== "" && ts.StoppedText !== "")
							rules.push(".tankstatusStopped" + " { background-color: " + ts.StoppedBackground + "; color: " + ts.StoppedText + "; padding-left: 3px; padding-right: 3px; }");
						if (ts.TestingBackground !== "" && ts.TestingText !== "")
							rules.push(".tankstatusTesting" + " { background-color: " + ts.TestingBackground + "; color: " + ts.TestingText + "; padding-left: 3px; padding-right: 3px; }");
						if (ts.BadBackground !== "" && ts.BadText !== "")
							rules.push(".tankstatusBad" + " { background-color: " + ts.BadBackground + "; color: " + ts.BadText + "; padding-left: 3px; padding-right: 3px; }");

					}	// end check if status is enabled
					if (ts.PointGroupHdrBackground !== "" && ts.PointGroupHdrText !== "" && ts.PointGroupDefaultCellGridLineColor !== "")
						rules.push(".pointgrouptemp .slick-header-column.ui-state-default" + " { background-color:" + ts.PointGroupHdrBackground + " !important" + "; color:" + ts.PointGroupHdrText + " !important; border: 1px solid " + ts.PointGroupDefaultCellGridLineColor + " !important;}");
					else if (ts.PointGroupHdrBackground !== "" && ts.PointGroupHdrText !== "")
						rules.push(".pointgrouptemp .slick-header-column.ui-state-default" + " { background-color:" + ts.PointGroupHdrBackground + " !important" + "; color:" + ts.PointGroupHdrText + " !important; }");
					else if (ts.PointGroupDefaultCellGridLineColor !== "")
						rules.push(".pointgrouptemp .slick-header-column.ui-state-default" + " { border: 1px solid " + ts.PointGroupDefaultCellGridLineColor + " !important;}");

					if (ts.PointGroupDefaultCellBackground !== "" && ts.PointGroupDefaultCellText !== "" && ts.PointGroupDefaultCellGridLineColor !== "")
						rules.push(".pointgrouptemp .slick-cell" + " { background-color:" + ts.PointGroupDefaultCellBackground + " !important" + "; color:" + ts.PointGroupDefaultCellText + " !important; border: 1px solid " + ts.PointGroupDefaultCellGridLineColor + " !important;}");
					else if (ts.PointGroupDefaultCellBackground !== "" && ts.PointGroupDefaultCellText !== "")
						rules.push(".pointgrouptemp .slick-cell" + " { background-color:" + ts.PointGroupDefaultCellBackground + " !important" + "; color:" + ts.PointGroupDefaultCellText + " !important; }");
					else if (ts.PointGroupDefaultCellGridLineColor !== "")
						rules.push(".pointgrouptemp .slick-cell" + " { border: 1px solid " + ts.PointGroupDefaultCellGridLineColor + " !important;}");

					if (ts.PointCellBackground !== "" && ts.PointCellText !== "" )
						rules.push(".pointcellcolors" + " { background-color: " + ts.PointCellBackground + "; color: " + ts.PointCellText + "; padding-left: 3px; padding-right: 3px; }");


					if (rules.length > 0)
					{
						if (dynamicStyleSheetAlarms[0].styleSheet) { // IE
							dynamicStyleSheetAlarms[0].styleSheet.cssText = rules.join(" ");
						} else {
							dynamicStyleSheetAlarms[0].appendChild(document.createTextNode(rules.join(" ")));
						}
					}
				}
			}, messageAttributes);
		},
		error: FMErrorAndExceptionHandling.ShowException
	});
}

FMOperateIndex.GetAlarmPrioritiesByGuid = function (guid)
{
	var alarmPriorities = FMOperateIndex.alarmPriorities;
	if ( !guid || !alarmPriorities )
	{
		return null;
	}
	
	for ( var i = 0; i < alarmPriorities.length; i++ )
	{
		if ( alarmPriorities[i].IdentityGuid === guid )
		{
			return alarmPriorities[i];
		}
	}
	return null;
}

FMOperateIndex.GetAlarmPrioritiesOnOpen = function () {

	var alarmPriorities = [];
	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };
	// remove previous notifications
	PNotify.removeStack(messageAttributes.stack);
	$.ajax({
		url: 'GetAlarmPriorities',
		type: 'Get',
		cache: false,
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function () {
				alarmPriorities = response.Data;

				// create classes for each alarm priority
				if (alarmPriorities.length > 0)
				{
					FMOperateIndex.alarmPriorities = alarmPriorities;

				}
			}, messageAttributes);
		},
		error: FMErrorAndExceptionHandling.ShowException
	});
}

// populate the screen with the configuration saved
FMOperateIndex.RestorePersistedControls = function (controlList) {

	window.localStorage.setItem('operateBeingRestored', "true");

	// the mode is being set by the print Point Group and we want to print a single point group so we just need to open that tabonly
	if (FMOperateIndex.pointgroupreportgeneration != "") {
		var reportparameters = FMOperateIndex.pointgroupreportgeneration.split("||");
		var printPointGroupName = reportparameters[0];
		var printPointGroupGuid = reportparameters[1];
		controlList = JSON.parse('[{"id":"tab-1","name":"Group","type":"group","active":true,"settings":[{"id":"tab-2","name":"' + printPointGroupName + '","type":"pointGroup","active":true,"settings":{"pointGroupGuid":"' + printPointGroupGuid + '"}}]}]');
		FMOperateIndex.pointgroupreportgeneration = "";
	}

	var controls = [];
	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };
	// remove previous notifications
	PNotify.removeStack(messageAttributes.stack);

	// this function can be called recursibly, the first time we are not passing parameters and we read the list of controls from the persistent storage
	if (controlList === undefined) {
		var monitorCount = FMOperateIndex.GetDetectedMonitorCount();
		console.log("Operate requesting saved tabs for " + (FMOperateIndex.GetOperateWindowName ? FMOperateIndex.GetOperateWindowName() : window.name) + " with monitors detected: " + monitorCount);
		$.ajax({
			url: 'GetScreenConfiguration',
			type: 'Get',
			cache: false,
			async: false,
			dataType: 'json',
			data: {
				windowName: FMOperateIndex.GetOperateWindowName ? FMOperateIndex.GetOperateWindowName() : window.name
			},
			success: function (response) {
				FMErrorAndExceptionHandling.HandleMessages(response, function () {
					controls = JSON.parse(response.Data);
					var filters = [];
					if (controls.length > 0) {

						var parentGroup = $.grep(controls, function (e) {
							return e.type === 'group';
						});
						filters = $.grep(controls, function (e) {
							return e.type === 'filter_view';
						});
						// if we are not using groups of tabs but we have groups (it was changed from group to non group) then don't render anything
						if (!FMOperateIndex.isTabGroupEnabled && parentGroup.length > 0) {
							controls = filters;
						}

						// if we are using groups of tabs but we have no groups in the main tab group (it was changed from non group to group) then don't render anything
						if (FMOperateIndex.isTabGroupEnabled && parentGroup.length === 0 && controls.length !== filters.length) {
							controls = filters;
						}
					}

					FMOperateIndex.contents = filters;

					if (FMOperateIndex.isTabGroupEnabled && controls.length === filters.length) {
						FMOperateIndex.restoringView = false;
						FMOperateIndex.restoringSaveInitialConfig = true;
						FMOperateIndex.AddGroupTab();

						if ($("#OpenInAlarmSummary").val().toLowerCase() === "true") {
							FMOperateIndex.openAlarmSummary();
						}
						window.localStorage.setItem('operateBeingRestored', "false");
					}
					else {

						if (!FMOperateIndex.isTabGroupEnabled && controls.length === filters.length) {
							if ($("#OpenInAlarmSummary").val().toLowerCase() === "true") {
								FMOperateIndex.openAlarmSummary();
								FMOperateIndex.restoringView = false;
								FMOperateIndex.restoringSaveInitialConfig = true;
								window.localStorage.setItem('operateBeingRestored', "false");
								return;
							}
						}

						FMOperateIndex.RestorePersistedControls(controls);
						// if there are active groups (meaning only 1 group)
						if (FMOperateIndex.restoringScreenactiveTabs.length === 0) {
							FMOperateIndex.restoringView = false;
							FMOperateIndex.restoringSaveInitialConfig = true;
							window.localStorage.setItem('operateBeingRestored', "false");
						}
						else {
							// go through each active tab and make them visible
							$.map(FMOperateIndex.restoringScreenactiveTabs, function (val, i) {
								FMOperateIndex.SetActiveTabRestoringView(val);
							});
						}
					}
				}, messageAttributes);
			},
			error: FMErrorAndExceptionHandling.ShowException
		});

		return;
	}
	controls = controlList;

	$.each(controls, function (index, control)
	{
		try {
			var newTabId = '';

			if (FMOperateIndex.IsTabCountExceeded()) {
				return false;
			}

			if ( control.type === 'drawing' )
			{
				// we need to check if we actually have a drawingGUID because we may have failed to load the drawing before or not complete opening a drawing in the previous session
				if ( !jQuery.isEmptyObject( control.settings ) )
				{
					if (control.settings.pointGuid != null)
					{
						if ( $( '#ViewPointsRight' ).val() === "True" )
						{
							FMOperateIndex.restoringView = true;
							newTabId = FMOperateIndex.openPoint(control.name, control.settings.pointGuid);
							FMOperateIndex.restoringView = false;
						}
					}
					else
					{
						if ( $( '#ViewGraphicsRight' ).val() === "True" )
						{
							FMOperateIndex.restoringView = true;
							newTabId = FMOperateIndex.openDraw(control.name, control.settings);
							FMOperateIndex.restoringView = false;
						}
					}
				}
			}
			else if (control.type === 'trend') {
				if ($('#ViewTrendsRight').val() === "True") {
					FMOperateIndex.restoringView = true;
					// this is called during restoration of view //bds
					if (control.active === true)
						newTabId = FMOperateIndex.openTrend(control.name, control.settings, true);
					else
						newTabId = FMOperateIndex.openTrend(control.name, control.settings, false);
					FMOperateIndex.restoringView = false;
				}
			}
			else if (control.type === 'report')
			{
				FMOperateIndex.restoringView = true;
				if ($('#ViewIMReportsRight').val() === "True")
					newTabId = FMOperateIndex.OpenReport(control.name, control.settings, true);
				FMOperateIndex.restoringView = false;
			}
			else if ( control.type === 'point' )
			{
				if ( $( '#ViewPointsRight' ).val() === "True" )
				{
					FMOperateIndex.restoringView = true;
					newTabId = FMOperateIndex.openPoint(control.name, control.settings.pointGuid);
					FMOperateIndex.UpdateOperateHelpKey("point");
					FMOperateIndex.restoringView = false;
				}

			}
			else if (control.type === 'pointGroup') {
				if ( $( '#ViewPointGroupsRight' ).val() === "True" )
				{
					FMOperateIndex.restoringView = true;
					newTabId = FMOperateIndex.openPointGroup(control.name, control.settings.pointGroupGuid);
					FMOperateIndex.restoringView = false;
				}
			}
			else if (control.type === 'pointHistory') {
				if ($('#ViewPointHistoryRight').val() === "True") {
					FMOperateIndex.restoringView = true;
					newTabId = FMOperateIndex.openPointHistory(control.settings.pointId, control.settings.pointGuid, control.settings.start, control.settings.interval, control.settings.intervalQuantity, control.settings.range, control.settings.rangeQuantity, control.settings.columns);
					FMOperateIndex.UpdateOperateHelpKey("pointHistory");
					FMOperateIndex.restoringView = false;
				}
			}
			else if (control.type === 'movementSummary')
			{
				if ($('#ViewMovementSummaryRight').val() === "True")
				{
					FMOperateIndex.restoringView = true;
					newTabId = FMOperateIndex.OpenMovementSummary(control.name, control.settings.movementSummaryGuid);
					FMOperateIndex.UpdateOperateHelpKey("movementSummary");
					FMOperateIndex.restoringView = false;
				}
			}
			else if (control.type === 'movementHistory')
			{
				if ($('#ViewMovementHistoryRight').val() === "True")
				{
					FMOperateIndex.restoringView = true;
					newTabId = FMOperateIndex.OpenMovementHistory(control.name);
					FMOperateIndex.UpdateOperateHelpKey("movementHistory");
					FMOperateIndex.restoringView = false;
				}
			}
			else if (control.type === 'alarmSummary')
			{
				FMOperateIndex.restoringView = true;
				newTabId = FMOperateIndex.openAlarmSummary(control.name);
				FMOperateIndex.restoringView = false;
			}
			else if (control.type === 'alarmHistory')
			{
				FMOperateIndex.restoringView = true;
				newTabId = FMOperateIndex.openAlarmHistory(control.name);
				FMOperateIndex.UpdateOperateHelpKey("alarmHistory");
				FMOperateIndex.restoringView = false;
			}
			else if (control.type === 'group')
			{
				FMOperateIndex.restoringView = true;
				newTabId = FMOperateIndex.AddGroupTab(control.name);
				FMOperateIndex.UpdateOperateHelpKey("group");
				FMOperateIndex.restoringView = false;
				FMOperateIndex.restoringScreenQueueInProgress[newTabId] = true;

				// call the function recursibly to restore all the tabs from the group
			
				FMOperateIndex.RestorePersistedControls(control.settings);
				FMOperateIndex.restoringScreenQueueInProgress[newTabId] = false;
			}
			// for the views in the filter selection of the menu just add them back again
			else if ( control.type === 'filter_view' )
			{
				FMOperateIndex.PersistUpdateFilterView( control.id, control.name );
			}

			// if the tab was stored as active set it to active on the form asynchronously so we finish creating all the sibling tabs
			if (control.active === true)
			{
				if (control.type === 'group')
				{
					control.active = false;
				}
				// add to the list of active tabs which will be set to active in the return of the main call to this function
				FMOperateIndex.restoringScreenactiveTabs.push( newTabId );
			}
		}
		catch (err) {
			FMOperateIndex.restoringView = false;
			console.error('Operate failed to restore saved control.', control, err);
		}
	});
};

FMOperateIndex.SetActiveTabRestoringView = function( tabId )
{
	var stillProcessing = false;
	// check if we have finished loading all the tabs
	for (var property in FMOperateIndex.restoringScreenQueueInProgress) {
		if (FMOperateIndex.restoringScreenQueueInProgress.hasOwnProperty(property))
		{
			if (FMOperateIndex.restoringScreenQueueInProgress[property] === true)
			{
				stillProcessing = true;
			}
		}
	}

	if ( stillProcessing === false )
	{
		FMOperateIndex.restoringView = true;
		FMOperateIndex.allScreensRestored = true;
		// we can only set the active tab after all tabs have been restored
		$( 'a[data-target=\'#' + tabId + '\'' ).click();
		FMOperateIndex.restoringView = false;

		if ( $( "#OpenInAlarmSummary" ).val().toLowerCase() === "true" )
		{
			FMOperateIndex.openAlarmSummary();
		}
		// save the final configuration in 1 second to make sure we have time to set all active tabs
		setTimeout(function () { FMOperateIndex.SaveRestoredView(); }, 2000);
		window.localStorage.setItem('operateBeingRestored', "false");

	}
	else// if we are not finished processing do recursive calls until we are done processing
	{
		setTimeout( function() { FMOperateIndex.SetActiveTabRestoringView( tabId );}, 100 ); 
	}
	return;
}

FMOperateIndex.SaveRestoredView = function ()
{
	// we may call this function multiple time during the reload of the screen, only do it once.
	//bds
	if ( FMOperateIndex.restoringSaveInitialConfig === true )
	{
		return;
	}

	FMOperateIndex.restoringSaveInitialConfig = true;

	// if there was a problem restoring a drawing (maybe the drawing was deleted) we need to remove it from the configuration so 
	// we don't try to restore it again
	if ( FMOperateIndex.isTabGroupEnabled )
	{
		// go through each active tab and make them visible
		FMOperateIndex.contents = $.map(FMOperateIndex.contents, function (val, i) {
			if ( val.type === 'group' )
			{
				val.settings = $.grep(val.settings,
					function (e) {
						if (e.type === 'drawing') {
							return e.settings.hasOwnProperty('drawingGuid'); // we do not want drawings with no drawing guid
						}
						return true;
					});
			}
			return val;
		});
	}
	else
	{
		FMOperateIndex.contents = $.grep( FMOperateIndex.contents,
			function( e )
			{
				if ( e.type === 'drawing' )
				{
					return e.settings.hasOwnProperty('drawingGuid'); // we do not want drawings with no drawing guid
				}
				return true;
			} );
	}

	console.log("FMOperateIndex.PersistScreenConfiguration called from FMOperateIndex.SaveRestoredView");
	FMOperateIndex.PersistScreenConfiguration();
	return;
}

FMOperateIndex.IsTabCountExceeded = function () {
	var currentTabCount = $(FMOperateIndex.isTabGroupEnabled ? "#mainContent .nav.nav-tabs .tab-name" : "#tabList.nav.nav-tabs .tab-name").length;

	if (currentTabCount && FMOperateIndex.MaxOperateTabsAllowed && (currentTabCount + 1) > FMOperateIndex.MaxOperateTabsAllowed) {
		return true;
	}
	return false;
}
// add a regular tab
FMOperateIndex.AddTab = function( name , isTrendTab)
{
	if ( name == null )
	{
		name = 'Unknown';
	}

	if (FMOperateIndex.IsTabCountExceeded()) {
		FMLayout.Alert("Exceeds maximum allowed tabs in operate! Please close an existing one to add a new one!", "Operate");
		return null;
	}

	FMOperateIndex.lastTabCreated += 1;
	var newId = 'tab-' + FMOperateIndex.lastTabCreated.toString();
	// get active tab
	var activeTab;
	var activePanelGroup;

	if ( !FMOperateIndex.isTabGroupEnabled )
	{
		activeTab = '#tabList';
		activePanelGroup = '#mainContent';
	}
	else
	{
		activeTab = $( '#tabList > li.active > a' ).attr( 'data-target' ) + '-tabList';
		activePanelGroup = $( '#tabList > li.active > a' ).attr( 'data-target' ) + ' .tab-content';
	}
	//id was passed
	var clickTabCommand = ' FMOperateIndex.PersistUpdateActiveControl(\'' + (activeTab === '#tabList' ? 'mainTab' : activeTab.replace('-tabList', '').replace('#', '')) + '\', \'' + newId + '\');';
	if ( activeTab !== '#tabList' )
	{
		clickTabCommand += 'setTimeout( function(){FMOperateIndex.RefreshScrollingTabs(\'' + activeTab + '\')}, 1 );';
	}

	// if the tab contains a trend, refresh the trend.  We need to get the tab of the new selected tab and from there we can get the drawing number to see which drawing to update
	clickTabCommand += 'if( $( \'#' + newId + ' .FMTrendGraph\').length ) { var drawingIndex = $( \'#' + newId + ' .FMTrendGraph\').attr(\'data-drawing-number\'); if (drawingIndex) {FMTrendIndex.refreshTrend(drawingIndex);} };';
	// if the tab contains graphics call gojs to redraw the drawing or in intial load it will be blank.  We need to get the tab of the new selected tab and from there we can get the drawing number to see which drawing to update
	clickTabCommand += 'if( $( \'#' + newId + ' canvas\').length ) { var a = FMDrawIndex.tabCanvasContainerCollection[$( \'#' + newId + ' .FMOperate\').attr(\'data-drawing-number\')]; if(a && a.goJsDiagram) a.goJsDiagram.requestUpdate();};';
	// if tab contains point group resize the grid to match the new size (the refresh has to happen after the tab contents are displayed so it uses the proper coordinates)
	clickTabCommand += "if( FMOperateIndex.staticPointGroupControllers.hasOwnProperty('" + newId + "')) { setTimeout( function(){ FMOperateIndex.staticPointGroupControllers['" + newId + "']._grid.resizeCanvas()}, 200);};";
	// if tab contains point group resize the grid to match the new size (the refresh has to happen after the tab contents are displayed so it uses the proper coordinates)
	clickTabCommand += "if( FMOperateIndex.staticPointHistoryControllers.hasOwnProperty('" + newId + "')) { setTimeout( function(){ FMOperateIndex.staticPointHistoryControllers['" + newId + "']._grid.resizeCanvas()}, 200);};";
	// if tab contains alarm summary resize the grid to match the new size (the refresh has to happen after the tab contents are displayed so it uses the proper coordinates)
	clickTabCommand += "if( $( \'#" + newId + ' #AlarmSummaryTableSection\').length) { setTimeout( function(){ AlarmSummaryTab.ResizeColumns()}, 200);};';
	// if tab contains alarm history resize the grid to match the new size (the refresh has to happen after the tab contents are displayed so it uses the proper coordinates)
	clickTabCommand += "if( $( \'#" + newId + ' #AlarmHistoryTableSection\').length) { setTimeout( function(){ AlarmHistoryTab.ResizeColumns()}, 200);};';
	// if tab contains movement Summary resize the grid to match the new size (the refresh has to happen after the tab contents are displayed so it uses the proper coordinates)
	clickTabCommand += "if( FMOperateIndex.movementSummaryControllers.hasOwnProperty('" + newId + "')) { setTimeout( function(){ FMOperateIndex.movementSummaryControllers['" + newId + "']._grid.resizeCanvas()}, 200);};";
	// if tab contains movement history resize the grid to match the new size (the refresh has to happen after the tab contents are displayed so it uses the proper coordinates)
	clickTabCommand += "if( $( \'#" + newId + ' #MovementHistoryTableSection\').length) { setTimeout( function(){ MovementHistoryTab.ResizeColumns()}, 200);};';


	$('<li><a data-target="#' + newId + '" data-toggle="tab" onclick="' + clickTabCommand + '"><label id="' + newId + '-tab-name" class="tab-name">' + name + '</label><span class="pull-right tab-remove-icon" onclick="FMOperateIndex.RemoveTab($(this).parent().parent());"></span></a></li>').appendTo(activeTab);

	var tabPanel = $('<div class="tab-pane" id="' + newId + '"></div>').appendTo(activePanelGroup);	

	$( 'a[data-target="#' + newId + '"]' ).tab( 'show' );

	//if (isTrendTab) {
	//	$(tabPanel).css({ "overflow-x": "hidden" });
	//	$(tabPanel).niceScroll({ cursorwidth: '10px', autohidemode: true, cursorcolor: '#486899', background: 'white', horizrailenabled: false });
	//}
	//else
	//	$(tabPanel).niceScroll({ cursorwidth: '10px', autohidemode: true, cursorcolor: '#486899', background: 'white' });

	// refresh the scroll buttons for the tabs (do it async to give time to display the contents of the tab)
	FMOperateIndex.RefreshScrollingTabs(activeTab);

	FMOperateIndex.GetAlarmPrioritiesOnOpen();

	return newId;
};

// add a group tab (if the site is configured to use groups of tabs)
FMOperateIndex.AddGroupTab = function( name )
{
	if ( name == null )
	{
		name = 'Group';
	}

	FMOperateIndex.lastTabCreated += 1;
	var newId = 'tab-' + FMOperateIndex.lastTabCreated.toString();
	var newCurrentTimeTextId = 'CurrentTimeTextGroups' + FMOperateIndex.lastTabCreated.toString();
	var newCurrentUserTextId = 'CurrentUserTextGroups' + FMOperateIndex.lastTabCreated.toString();
	var newLicenseExpirationText = 'LicenseExpirationTextGroups' + FMOperateIndex.lastTabCreated.toString();
	var tabGroupName = newId + '-tabList';

	$('#tabList > :nth-child(' + ($('#tabList').children().length) + ')').before('<li class="group-tab"><a data-target="#' + newId + '" data-toggle="tab" onclick="FMOperateIndex.SwitchGroupTab( \'' + newId + '\', \'' + tabGroupName + '\' )" ondblclick="FMOperateIndex.RenameGroupTabs( this );" ><label class="tab-name">' + name + '</label><span class="pull-right tab-remove-icon" onclick="FMOperateIndex.RemoveTab($(this).parent().parent());"></span></a></li>');
	$('<div class="tab-pane" id="' + newId + '"><div class="groupTab"><div class="groupTab-tabList"><ul class="nav nav-tabs" id="' + tabGroupName + '"></ul></div><div class="tab-current-user-time"><label id="' + newCurrentUserTextId + '" class="tab-current-user-text" ></label><label id="' + newCurrentTimeTextId + '" class="tab-current-time-text" ></label><label id="' + newLicenseExpirationText + '" class="tab-current-license-expiration-text" ></label></div><div class="tab-configuration-menu"><a title="' + $("#TabConfigurationGear").val() + '" class="tab-configuration glyphicon glyphicon-cog"></a></div><div class="tab-content"></div></div></div>').appendTo('#mainContent');
	$( 'a[data-target="#' + newId + '"]' ).tab( 'show' );
	FMOperateIndex.EnsureScrollingTabs('#' + tabGroupName);

	// if we are not in the process of restoring the persisten state then open new groups so we can rename them
	if ( FMOperateIndex.restoringView === false )
	{
		setTimeout( function()
		{
				FMOperateIndex.RenameGroupTabs( $( 'a[data-target="#' + newId + '"]' ) );
		}, 1 );
	}
	
	FMOperateIndex.contents.push( { "id": newId, "name": name, "type": 'group', active: true, "settings": [] } );
	return newId;
};

FMOperateIndex.SwitchGroupTab = function( tabId, tabGroupName )
{
	setTimeout( function()
	{
		FMOperateIndex.RefreshScrollingTabs('#tabList');
		FMOperateIndex.RefreshScrollingTabs('#' + tabGroupName);
	}, 1 );

	FMOperateIndex.PersistUpdateActiveControl('mainTab', tabId);

	// if the selected tab contains graphics call gojs to redraw the drawing or in intial load it will be blank.  We need to get the tab of the new selected tab and from there we can get the drawing number to see which drawing to update
	var selectedTab = $('#' + tabGroupName + ' .active a').attr('data-target');
	if ($(selectedTab + ' .FMTrendGraph').length) {
		var drawingIndex = $(selectedTab + ' .FMTrendGraph').attr('data-drawing-number');
		if (drawingIndex)
			FMTrendIndex.refreshTrend(drawingIndex);
	}
	else if( $( selectedTab + ' canvas').length )
	{
		FMDrawIndex.tabCanvasContainerCollection[$( selectedTab + ' .FMOperate' ).attr( 'data-drawing-number' )].goJsDiagram.requestUpdate();
	};

	// if tab contains point group resize the grid to match the new size (the refresh has to happen after the tab contents are displayed so it uses the proper coordinates)
	if ( selectedTab )
	{
		selectedTab = selectedTab.replace( '#', '' );
		if ( FMOperateIndex.staticPointGroupControllers.hasOwnProperty( selectedTab ) )
		{
			setTimeout( function()
			{
				FMOperateIndex.staticPointGroupControllers[selectedTab]._grid.resizeCanvas();
			}, 200 );
		};
	}

	// if tab contains point history resize the grid to match the new size (the refresh has to happen after the tab contents are displayed so it uses the proper coordinates)
	if (selectedTab) {
		selectedTab = selectedTab.replace('#', '');
		if (FMOperateIndex.staticPointHistoryControllers.hasOwnProperty(selectedTab)) {
			setTimeout(function () {
				FMOperateIndex.staticPointHistoryControllers[selectedTab]._grid.resizeCanvas();
			}, 200);
		};
	}

	// if tab contains movement summary resize the grid to match the new size (the refresh has to happen after the tab contents are displayed so it uses the proper coordinates)
	if (selectedTab)
	{
		selectedTab = selectedTab.replace('#', '');
		if (FMOperateIndex.movementSummaryControllers.hasOwnProperty(selectedTab))
		{
			setTimeout(function ()
			{
				FMOperateIndex.movementSummaryControllers[selectedTab]._grid.resizeCanvas();
			}, 200);
		};
	}

	// if tab contains movement history resize the grid to match the new size (the refresh has to happen after the tab contents are displayed so it uses the proper coordinates)
	if (selectedTab)
	{
		selectedTab = selectedTab.replace('#', '');
		if (FMOperateIndex.movementHistoryControllers.hasOwnProperty(selectedTab))
		{
			setTimeout(function ()
			{
				FMOperateIndex.movementHistoryControllers[selectedTab]._grid.resizeCanvas();
			}, 200);
		};
	}
}

// change controls to be able to rename a group tab (a regular tab cannot be renamed)
FMOperateIndex.RenameGroupTabs = function( control )
{
	var firstChildren = $( control ).children().first();
	// the <a>nchor in the tab control either contains a label (to display the name) or an input box (to change the name)
	if ( firstChildren.is( 'label' ) )
	{
		var oldName = firstChildren.text();
		// replace the label for the input
		$( control ).html( '<input id=\'' + $( control ).attr( 'data-target' ).replace( '#', '' ) + '\' type=\'text\' class=\'tab-rename-group\' value=\'' + oldName + '\' onclick=\'event.stopPropagation();\' onblur=\'FMOperateIndex.EndRenameGroupTabs( this );\' onkeyup=\'if(event.keyCode == 13){ FMOperateIndex.EndRenameGroupTabs( this );}\'> </input> ' );
		FMOperateIndex.RefreshScrollingTabs('#tabList');
		$( control ).children().first().focus().val( oldName );

		// terminate the edit if user presses the Esc key
		$( '#' + $( control ).attr( 'data-target' ).replace( '#', '' ) ).on( 'keyup', function( e )
		{
				if ( e.keyCode === 27 )
				{ // escape key maps to keycode `27`
					FMOperateIndex.EndRenameGroupTabs( this );
				}
		} );
	}
};

// Finish renaming the group tab. When we lose focus on the group tab we want to finish the renaming.
FMOperateIndex.EndRenameGroupTabs = function( control )
{
	var name = $( control ).val();
	// replace the input for the label
	$( control ).parent().html( '<label class="tab-name">' + name + '</label><span class="pull-right tab-remove-icon" onclick="FMOperateIndex.RemoveTab($(this).parent().parent());"></span>' );
	var controlId = $( control ).attr( 'id' );

	FMOperateIndex.PersistUpdateNameControl( 'mainTab', controlId, name );
	FMOperateIndex.RefreshScrollingTabs('#tabList');
};

// change controls to be able to rename a group tab (a regular tab cannot be renamed)
FMOperateIndex.RenameTab = function (control)
{
	if ( $( '#ModifyPointGroupsRight' ).val() == 'False' )
	{
		return;
	}
	FMOperateIndex.restoringSaveInitialConfig = true;
	var firstChildren = $(control).children().first();
	// the <a>nchor in the tab control either contains a label (to display the name) or an input box (to change the name)
	if (firstChildren.is('label')) {
		var oldName = firstChildren.text();
		// replace the label for the input
		$(control).html('<input id=\'' + $(control).attr('data-target').replace('#', '') + '\' type=\'text\' class=\'tab-rename-group\' value=\'' + oldName + '\' onclick=\'event.stopPropagation();\' onblur=\'FMOperateIndex.EndRenameTab( this );\' onkeyup=\'if(event.keyCode == 13){ FMOperateIndex.EndRenameTab( this );}\'> </input> ');
		FMOperateIndex.RefreshScrollingTabs('#tabList');
		$(control).children().first().focus().val(oldName);

		// terminate the edit if user presses the Esc key
		$('#' + $(control).attr('data-target').replace('#', '')).on('keyup', function (e) {
			if (e.keyCode === 27) { // escape key maps to keycode `27`
				FMOperateIndex.EndRenameTab(this);
			}
		});
	}
};

// Finish renaming the group tab. When we lose focus on the group tab we want to finish the renaming.
FMOperateIndex.EndRenameTab = function (control)
{
	var name = $(control).val();
	// replace the input for the label
	$(control).parent().html('<label class="tab-name">' + name + '</label><span class="pull-right tab-remove-icon" onclick="FMOperateIndex.RemoveTab($(this).parent().parent());"></span>');
	var controlId = $(control).attr('id');

	var activePanelGroup = 'mainTab';
	if ( FMOperateIndex.isTabGroupEnabled )
	{
		activePanelGroup = $( '#tabList > li.active > a' ).attr( 'data-target' ).replace( '#', '' );
	}

	FMOperateIndex.PersistUpdateNameControl(activePanelGroup, controlId, name);
	FMOperateIndex.RefreshScrollingTabs('#tabList');

	// if the tab is a point group we also want to save the new point group name
	if (FMOperateIndex.staticPointGroupControllers.hasOwnProperty(controlId))
	{
		FMOperateIndex.PersistPointGroup(activePanelGroup, controlId, FMOperateIndex.staticPointGroupControllers[controlId]._grid);

		// refresh the hamburger menu (list of point groups) next time it's opened to reflect the new point group name
		if ($(".operateMenuItem.active a").attr("id") === "menuPointGroups")
		{
			FMOperateIndex.refreshHamburgerMenu = true;
		}
	}

	// if the tab is a movement summary we also want to save the new movement summary name
	if (FMOperateIndex.movementSummaryControllers.hasOwnProperty(controlId))
	{
		FMOperateIndex.PersistMovementSummary(activePanelGroup, controlId, FMOperateIndex.movementSummaryControllers[controlId]._grid);

		// refresh the hamburger menu (list of movement summaries) next time it's opened to reflect the new movement summary name
		if ($(".operateMenuItem.active a").attr("id") === "menuMovementSummary")
		{
			FMOperateIndex.refreshHamburgerMenu = true;
		}
	}
};


// initialize the Group Tabs when the site is configured for groups
FMOperateIndex.initializeTabGroup = function()
{
	$( '<li><a data-target="#tab-addgroup" class="tab-add-group" onclick="FMOperateIndex.AddGroupTab(' + '\'Group\'' + ')";><label class=""></span></label></a></li>' ).appendTo( '#tabList' );
};

// remove a tab (or group of tabs)
FMOperateIndex.RemoveTab = function( tab )
{
	// if the tab to delete is not the active then don't do anything
	if ( $( tab ).hasClass( 'active' ) )
	{
		//default to make next tab active
		var nexActivetTab = $( tab ).next();
		// if we are at the last tab in the group then look at the previous tab
		if ( nexActivetTab.length === 0 )
		{
				nexActivetTab = $( tab ).prev();
		}
		else
		// if the next tab is the + sign to add new tabs get the previous tab
		if ( $( nexActivetTab ).find( 'a' ).hasClass( 'tab-add-group' ) )
		{
				nexActivetTab = $( tab ).prev();
		}

		// if we deleted the last tab then there is no new active
		if ( nexActivetTab.length !== 0 )
		{
				$( nexActivetTab ).find( 'a' ).click();
		}
		FMOperateIndex.restoringSaveInitialConfig = true;
	}

	// find the parent
	var parentUlId = $( tab ).closest( 'ul' ).attr( 'id' );
	var parent;

	// if we are deleting the main tabgroup
	if ( parentUlId === 'tabList' )
	{
		parent = 'mainTab';
	}
	else
	{ // we are deleting a tab within a group of tabs, get the tab id of the group
		parent = parentUlId.replace( '-tabList', '' );
	}

	var tabContents = $(tab).find('a').attr('data-target');

	/* check to see if we are dealing with a static point group */
	var tabContentId = tabContents.replace('#', '');

	if (FMOperateIndex.staticPointGroupControllers.hasOwnProperty(tabContentId))
	{
		FMOperateIndex.closePointGroup(tabContentId);
	}
	else if (FMOperateIndex.movementSummaryControllers.hasOwnProperty(tabContentId))
	{
		FMOperateIndex.CloseMovementSummary(tabContentId);
	}
	else
	{
		var drawingNumber = null;
		if ($(tabContents + ' .FMTrendGraph').length > 0)
		{
			drawingNumber = $(tabContents + ' .FMTrendGraph').attr('data-drawing-number');
			FMTrendIndex.closeTrend(drawingNumber);
		}
		else
		{
			drawingNumber = $(tabContents + ' .FMOperate').attr('data-drawing-number');
			FMOperateIndex.CloseDrawing(drawingNumber);
		}

		// The close movement history function will determine if there a 
		// movement history tab.
		FMOperateIndex.CloseMovementHistory(tabContentId);

		AlarmHistoryTab.CloseAlarmHistory(tabContentId);
	}

	$( tabContents ).remove();
	$(tab).remove();

	// refresh the scroll buttons for the tabs
	FMOperateIndex.RefreshScrollingTabs('#tabList');

	FMOperateIndex.RemovePersistedControl(parent, tabContents.replace('#', ''));

	event.stopPropagation();

};

// Initialize a drawing that was just loaded into a panel
FMOperateIndex.InitializeDrawing = function( drawingNumber )
{
	var a = FMDrawIndex.tabCanvasContainerCollection[drawingNumber];
	if ( !a || !a.goJsDiagram )
	{
		return;
	}

	FMDrawIndex.tabCanvasContainerCollection[drawingNumber].goJsDiagram.nodes.each(function (node) {
			node.selectable = false;
			node.textEditable = false;

			if (node.name === 'Tag') {
				FMOperateIndex.activeObjects.push(node);
			}
			else if (node.name === 'Bar') {
				FMOperateIndex.activeObjects.push(node);
				var dataModel = node.diagram.model;
				dataModel.setDataProperty(node.data, 'val', NaN);
			}
			else if (node.data.AnimationPointValueAssignments && node.data.AnimationPointValueAssignments.TestGroupPointValueInfoList) {
				FMOperateIndex.activeObjects.push(node);
			}
			else if (node.name === 'button' && (node.data.buttonActionType === 'BUTTON_POINT_TREND' || node.data.buttonActionType === 'BUTTON_ACTION_DETAIL' || node.data.buttonActionType === 'BUTTON_ACTION_COMMAND' || node.data.buttonActionType === 'BUTTON_ACTION_POINT_HISTORY')) {
				FMOperateIndex.activeObjects.push(node);
			}
		});

		FMDrawIndex.tabCanvasContainerCollection[drawingNumber].goJsDiagram.links.each( function( link )
		{
			if ( link.data.AnimationPointValueAssignments && link.data.AnimationPointValueAssignments.TestGroupPointValueInfoList )
				FMOperateIndex.activeObjects.push(link);
		}


	);

	FMOperateIndex.UpdateTags();
};

FMOperateIndex.CloseDrawing = function( drawingNumber )
{
	var firstNode = null;
	var nodeCount = 0;

	var a = FMDrawIndex.tabCanvasContainerCollection[drawingNumber];
	if ( a && a.goJsDiagram )
	{
		if (a.goJsDiagram.model.modelData.timeoutID) {
			clearTimeout(a.goJsDiagram.model.modelData.timeoutID);
			a.goJsDiagram.model.modelData.timeoutID = null;
		}

		if (a.goJsDiagram.model.modelData.ajaxRequest != null) {
			var ajaxRequest = a.goJsDiagram.model.modelData.ajaxRequest;
			a.goJsDiagram.model.modelData.ajaxRequest = null;
			ajaxRequest.abort();
		}


		a.goJsDiagram.nodes.each( function( node ) {
				this.selectable = false;
				if ( node.name === 'Tag'
					|| node.name === 'Bar'
					|| node.data.AnimationPointValueAssignments && node.data.AnimationPointValueAssignments.TestGroupPointValueInfoList
					|| (node.name === 'button'
					&& (node.data.buttonActionType === 'BUTTON_POINT_TREND'
					|| node.data.buttonActionType === 'BUTTON_ACTION_DETAIL'
					|| node.data.buttonActionType === 'BUTTON_ACTION_COMMAND'))) {
					if ( !firstNode )
					{
						firstNode = node;
					}
					nodeCount++;
				}
		} );
	}

	var firstNodeIndex = FMOperateIndex.activeObjects.indexOf( firstNode );
	if ( firstNodeIndex !== -1 )
	{
		FMOperateIndex.activeObjects.splice( firstNodeIndex, nodeCount );
	}

	//Clear Memory in canvasCollection array item pointed to by tabIndex
	if ( FMDrawIndex.tabCanvasContainerCollection && drawingNumber >= 0 && drawingNumber <= FMDrawIndex.tabCanvasContainerCollection.length - 1 )
	{
		FMDrawIndex.tabCanvasContainerCollection[drawingNumber] = null;
	}

	FMOperateIndex.UpdateTags();
};

FMOperateIndex.findPointValueForTag = function (pointValues, pointTemplateTagGuid)
{
	for ( var i = 0; i < pointValues.length; i++ )
	{
		if ( pointValues[i].PointTemplateTagGuid === pointTemplateTagGuid )
		{
				return pointValues[i];
		}
	}
	return null;
}

FMOperateIndex.findPointValueForPoint = function (pointValues, id)
{
	for (var i = 0; i < pointValues.length; i++)
	{
		if (pointValues[i].ID === id)
		{
			return pointValues[i];
		}
	}
	return null;
}

FMOperateIndex.findPointValueForSetting = function (pointValues, id, pointTemplatePropertyGuid)
{
	for (var i = 0; i < pointValues.length; i++)
	{
		if (pointValues[i].PointTemplatePropertyGuid === pointTemplatePropertyGuid && pointValues[i].ID === id)
		{
			return pointValues[i];
		}
	}
	return null;
}

FMOperateIndex.UpdatePointTemplateObjects = function( drawingModelStr, operatePoint )
{
	var drawingData = JSON.parse( drawingModelStr );
	for ( var j = 0; j < drawingData.model.nodeDataArray.length; j++ )
	{
		var node = drawingData.model.nodeDataArray[j];
		//node.TagPointValueType
		//Tag = 0,
		//Setting = 1,
		//Point = 2,
		//All = 3
		if (node.TagPointValueType === 0)
		{
			FMOperateIndex.SetNodePointValues(node, FMOperateIndex.findPointValueForTag(operatePoint.Values, node.TagGUID));
		}
		if (node.TagPointValueType === 1)
		{
			FMOperateIndex.SetNodePointValues(node, FMOperateIndex.findPointValueForSetting(operatePoint.Values, node.TagTagID, node.TagGUID));
		}
		if ( node.TagPointValueType === 2 )
		{
				FMOperateIndex.SetNodePointValues(node, FMOperateIndex.findPointValueForPoint(operatePoint.Values, node.TagTagID));
		}

		//Fill in concrete point value guids for animation point value point template value guids
		if (node.AnimationPointValueAssignments && node.AnimationPointValueAssignments.TestGroupPointValueInfoList)
		{
			var testGroupPointValueInfoList = node.AnimationPointValueAssignments.TestGroupPointValueInfoList;
			for ( var i = 0; i < testGroupPointValueInfoList.length; i++ )
			{
				var testGroupPointValueInfo = testGroupPointValueInfoList[i];
				if (testGroupPointValueInfo && testGroupPointValueInfo.PointValueIsFromTemplate && operatePoint && operatePoint.Values)
				{
					var tagPointValue = null;
					if (testGroupPointValueInfo.ValueType === 0)
					{
						if ( testGroupPointValueInfo.PointValueGuid)
						{
							tagPointValue = FMOperateIndex.findPointValueForTag(operatePoint.Values, testGroupPointValueInfo.PointValueGuid);
						}
					}
					if (testGroupPointValueInfo.ValueType === 1)
					{
						if (testGroupPointValueInfo.PointValueGuid && testGroupPointValueInfo.PointValueID)
						{
							tagPointValue = FMOperateIndex.findPointValueForSetting(operatePoint.Values, testGroupPointValueInfo.PointValueID, testGroupPointValueInfo.PointValueGuid);
						}
					}
					if (testGroupPointValueInfo.ValueType === 2)
					{
						if (testGroupPointValueInfo.PointValueID)
						{
							tagPointValue = FMOperateIndex.findPointValueForPoint(operatePoint.Values, testGroupPointValueInfo.PointValueID);
						}
					}
					if (tagPointValue)
					{
						testGroupPointValueInfo.PointValueIsFromTemplate = false;
						testGroupPointValueInfo.PointValueGuid = tagPointValue.PointValueIdentifier.IdentityGuid;
						testGroupPointValueInfo.PointGuid = tagPointValue.PointGuid;
						testGroupPointValueInfo.PointID = tagPointValue.PointID;
						testGroupPointValueInfo.PointValueID = tagPointValue.PointValueIdentifier.PropertyID;
					}
				}
			}
		}
	}

	for ( var j = 0; j < drawingData.model.linkDataArray.length; j++ )
	{

		var link = drawingData.model.linkDataArray[j];
		//Fill in concrete point value guids for animation point value point template value guids
		if (link.AnimationPointValueAssignments && link.AnimationPointValueAssignments.TestGroupPointValueInfoList)
		{
			var testGroupPointValueInfoList = link.AnimationPointValueAssignments.TestGroupPointValueInfoList;
			for ( var i = 0; i < testGroupPointValueInfoList.length; i++ )
			{
				var testGroupPointValueInfo = testGroupPointValueInfoList[i];
				if (testGroupPointValueInfo && testGroupPointValueInfo.PointValueIsFromTemplate && operatePoint && operatePoint.Values)
				{
					var tagPointValue = null;
					if (testGroupPointValueInfo.ValueType === 0)
					{
						if ( testGroupPointValueInfo.PointValueGuid)
						{
							tagPointValue = FMOperateIndex.findPointValueForTag(operatePoint.Values, testGroupPointValueInfo.PointValueGuid);
						}
					}
					if (testGroupPointValueInfo.ValueType === 1)
					{
						if (testGroupPointValueInfo.PointValueGuid && testGroupPointValueInfo.PointValueID)
						{
							tagPointValue = FMOperateIndex.findPointValueForSetting(operatePoint.Values, testGroupPointValueInfo.PointValueID, testGroupPointValueInfo.PointValueGuid);
						}
					}
					if (testGroupPointValueInfo.ValueType === 2)
					{
						if (testGroupPointValueInfo.PointValueID)
						{
							tagPointValue = FMOperateIndex.findPointValueForPoint(operatePoint.Values, testGroupPointValueInfo.PointValueID);
						}
					}
					if (tagPointValue)
					{
						testGroupPointValueInfo.PointValueIsFromTemplate = false;
						testGroupPointValueInfo.PointValueGuid = tagPointValue.PointValueIdentifier.IdentityGuid;
						testGroupPointValueInfo.PointGuid = tagPointValue.PointGuid;
						testGroupPointValueInfo.PointID = tagPointValue.PointID;
						testGroupPointValueInfo.PointValueID = tagPointValue.ID;
					}
				}
			}
		}
	}

	return JSON.stringify(drawingData);
}

FMOperateIndex.SetNodePointValues = function( nodeData, pointValue )
{
	if ( !nodeData || !pointValue || !nodeData.PointTemplateTagSelectionIndicator )
	{
		return;
	}
	
	if (nodeData.TagPointValueType === 0 && nodeData.TagGUID !== pointValue.PointTemplateTagGuid)
	{
		return;
	}
	if (nodeData.TagPointValueType === 2 && nodeData.TagTagID !== pointValue.ID)
	{
		return;
	}
	//Clear PointTemplateSelectionIndicator since we are transforming this node into a concrete tag upon loading
	//into Operate.
	nodeData.PointTemplateTagSelectionIndicator = false;
	nodeData.TagGUID = pointValue.PointValueIdentifier.IdentityGuid;
	nodeData.PointGUID = pointValue.PointGuid;
	nodeData.TagPointID = pointValue.PointID;
	nodeData.TagTagID = pointValue.ID;
	nodeData.TagPointIDAndTagID = pointValue.PointID + ' : ' + pointValue.ID;
	nodeData.ToolTipString = pointValue.PointID + ' : ' + pointValue.ID + ' : ' + FMTAGFIELDSELECTION.GetFieldString(nodeData.TagFieldSelection);
	switch(nodeData.category)
	{
		case 'button':
				switch(nodeData.buttonActionType)
				{
					case 'BUTTON_POINT_TREND':
						nodeData.buttonActionObjectGuid = nodeData.PointGUID;
						nodeData.buttonActionObjectId = nodeData.TagPointID;
						break;
					case 'BUTTON_ACTION_COMMAND':
						nodeData.buttonActionObjectGuid = nodeData.TagGUID;
						nodeData.buttonActionObjectId = nodeData.TagPointIDAndTagID;
						break;
					case 'BUTTON_ACTION_DETAIL':
						nodeData.buttonActionObjectGuid = nodeData.PointGUID;
						nodeData.buttonActionObjectId = nodeData.TagPointID;
						break;
				}
				break;
	}
}
// Open a drawing from the menu
FMOperateIndex.openDraw = function( DrawingId, settings )
{
	// can't open drawing in a group if there are no groups (there is always 1 tab, the tab to add groups )
	if ( FMOperateIndex.isTabGroupEnabled && $( '#tabList' ).children().length === 1 )
	{
		FMLayout.Alert( 'Cannot open a Graphic without a Group.', 'Error' );
		return null;
	}

	// Persist the new tab so it can be re-open when the screen is reloaded
	var activeTab = 'mainTab';
	if (FMOperateIndex.isTabGroupEnabled) {
		activeTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
	}

	//check to see if we have one open already (only when clicking on the menu, not when restoring the screen)
	if (FMOperateIndex.restoringView === false) {
		var parentTab = "";
		var graphicTab = "";

		var parentGroup = FMOperateIndex.contents;

		if ( activeTab !== 'mainTab' )
		{
			parentTab = activeTab;
			var parentGroupFiltered = $.grep( parentGroup, function( e )
			{
				return e.id === activeTab;
			} );
			if ( parentGroupFiltered.length > 0 )
			{
				parentGroup = parentGroupFiltered[0];
			}
			$.each(parentGroup.settings, function (index, level1Tab) {
				if (level1Tab.type === "drawing" && level1Tab.settings.drawingGuid === settings.drawingGuid && level1Tab.settings.pointGuid === settings.pointGuid) {
					graphicTab = level1Tab.id;
				}
			});
		}
		else {
			$.each(parentGroup, function (index, level1Tab) {
				if (level1Tab.type === "drawing" && level1Tab.settings.drawingGuid === settings.drawingGuid && level1Tab.settings.pointGuid === settings.pointGuid) {
					graphicTab = level1Tab.id;
				}
			});
		}

		if (parentTab !== "") {
			$('a[data-target=\'#' + parentTab + '\'').click();
		}
		if (graphicTab !== "") {
			$('a[data-target=\'#' + graphicTab + '\'').click();
			return false;
		}
	}

	var newId = FMOperateIndex.AddTab(DrawingId);

	if (newId === null) return false;

	var activeDrawing = FMOperateIndex.numberDrawings = FMOperateIndex.numberDrawings + 1;

	FMOperateIndex.restoringScreenQueueInProgress[newId] = true;

	// we need to specify the coordinates since canvas hard code the canvas size
	$( '<div id="diagram' + activeDrawing + '" width="' + $( '.tab-content' ).width() + '" height="' + $( '.tab-content' ).height() + '" class="FMOperate FMDrawing" data-drawing-number="' + FMOperateIndex.numberDrawings + '"> Your browser does not support the HTML5 canvas.</div>' ).appendTo( '#' + newId );

	FMDrawIndex.AttatchGoJSDiagramToNewCanvas( activeDrawing );


	// do not specify the settings when creating the drawing.  We will update it when we retrieve the actual drawing
	FMOperateIndex.PersistNewControl( activeTab, newId, DrawingId, 'drawing', {} );


	$('<div id="loader' + activeDrawing + '0" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#' + newId);

	//$('<div id="Drawingtabname" style="hidden" >' + newId + '</div>').appendTo('#' + newId);

	// put messages on the actual tab
	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $( '#diagram' + activeDrawing ) };

	$.ajax( {
		type: 'get',
		dataType: 'json',
		cache: false,
		url: 'GetOperateDrawing',
		data: {
				id: ( settings.drawingGuid ) ? settings.drawingGuid : '',
				pointGuidStr: ( settings.pointGuid ) ? settings.pointGuid : ''
		},
		activeDrawing: activeDrawing,
		activeTab: activeTab,
		newId: newId,
		success: function( response )
		{
			var activeDrawing = this.activeDrawing;
			var activeTab = this.activeTab;
			var newId = this.newId;
			FMOperateIndex.UpdateOperateHelpKey("drawing");

			$('#loader' + activeDrawing + '0').remove();

			var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

			FMErrorAndExceptionHandling.HandleMessages( response,	function( data, inError ) {
				// if it was not in error load and update the drawing
				if ( !inError )
				{
					if ( !!window.chrome ) // in chrome,the gojs canvas does not render if the tab is inactive at the time it is loaded. temporarily show the tab on load as a workaround. 
						{
							$( 'a[data-target="#' + newId + '"]' ).tab( 'show' );
						}
						isDrawBoolean = false;
						var drawingData = (data.PointInformation) ? FMOperateIndex.UpdatePointTemplateObjects(data.Drawing, data.PointInformation) : data.Drawing;
						if (data.PointInformation) { FMOperateIndex.UpdateOperateHelpKey("point"); }
						FMDrawIndex.UpdateactiveCanvas(activeDrawing);
								
						FMDrawIndex.RenderDrawing( drawingData, false);
						var diagram = FMDrawIndex.tabCanvasContainerCollection[activeDrawing].goJsDiagram;
						diagram.skipsUndoManager = true; //Added to prevent undoManager from processing any undoable events
						diagram.allowUndo = false;
						diagram.grid.visible = false;
						diagram.allowClipboard = false;
						diagram.allowCopy = false;
						diagram.allowDelete = false;
						diagram.allowDragOut = false;
						diagram.allowDrop = false;
						diagram.allowGroup = false;
						diagram.allowInsert = false;
						diagram.allowLink = false;
						diagram.allowMove = false;
						diagram.allowRelink = false;
						diagram.allowReshape = false;
						diagram.allowResize = false;
						diagram.allowRotate = false;
						diagram.allowSelect = false;
						diagram.allowTextEdit = false;
						diagram.allowUngroup = false;
						diagram.animationManager.isEnabled = false;
					//Create animation dictionary
						var animationDictionary = [];
						for ( var i = 0; i < data.Animations.length; i++ )
						{
							var animation = data.Animations[i];
							animationDictionary[animation.AnimationGuid] = animation;
						}
						diagram.AnimationDictionary = animationDictionary;

						if ( settings.position )
						{
							diagram.initialPosition = new go.Point( settings.position.L, settings.position.M );
						}
						if ( settings.scale )
						{
							diagram.scale = settings.scale;
						}

						//diagram.alignDocument(go.Spot.Center, go.Spot.Center); //Jeff asked operate to zoom to fit until we have custom view saving
						//diagram.zoomToFit();
						diagram.scrollMode = go.Diagram.DocumentScroll;

						//Instruct GoJS to update Canavs Element Size
						diagram.requestUpdate();

						diagram.mouseOver = function( inputEvent )
						{
							var diagram = inputEvent.diagram;
							if ( diagram && diagram.model.modelData.viewportChanging )
							{
								diagram.model.modelData.viewportChanging = false;
								FMOperateIndex.PersistUpdateActiveControl(this.activeTab, this.newId, { drawingGuid: settings.drawingGuid, pointGuid: settings.pointGuid, position: diagram.position, scale: diagram.scale });
							}
						};
						diagram.addDiagramListener( 'ViewportBoundsChanged', function( diagramEvent )
						{
							var diagram = diagramEvent.diagram;
							if ( diagram )
							{
								diagram.model.modelData.viewportChanging = true;
							}
						} );

						FMOperateIndex.PersistUpdateDrawingSettings(activeTab, newId, { drawingGuid: settings.drawingGuid, pointGuid: settings.pointGuid, position: diagram.initialPosition, scale: diagram.initialScale });

						FMOperateIndex.InitializeDrawing(activeDrawing);

						if ( settings.pointGuid )
						{
							var canvas = $("#diagram" + activeDrawing + " > canvas");
							FMOperateIndex.InitContextMenu(canvas, data.PointInformation.PointID, data.PointInformation.PointGuid);
						}
				}
			},
			messageAttributes);
				
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		},
		error: function( xhr, textStatus, error )
		{
			var activeDrawing = this.activeDrawing;
			var activeTab = this.activeTab;
			var newId = this.newId;

			// need to make  sure that the error we are getting is because we close the page before getting the response
			if ( xhr.status != 0 )
			{
				FMErrorAndExceptionHandling.ShowException( xhr,
					textStatus,
					error,
					function()
					{
							$( '#loader' + activeDrawing + '0' ).remove();
					} );
			}
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		}
	} );
	return newId;
};

FMOperateIndex.deleteTrend = function(trendId, trendGuid){
	window.event.cancelBubble = true;

	FMLayout.Confirm(	'Delete Trend : ' + trendId + '?',
							'Delete Trend',
							function () {
								var controls = FMOperateIndex.contents;
								$.each(controls, function (index, control) {
									if (control.type === 'group') {
										$.each(control.settings, function (index, setting) {
											if(setting.type === 'trend'
											&& trendGuid === setting.settings.guid) {
												setTimeout(function () { FMOperateIndex.RemoveTab($('a[data-target=\'#' + setting.id + '\'').parent()); }, 0);
											}
										});
									}
									else if (control.type === 'trend'
									&& trendGuid === control.settings.guid) {
										FMOperateIndex.RemoveTab($('a[data-target=\'#' + control.id + '\'').parent());
									}
								});

							
								var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#trendSubMenuList') };

								$.ajax({
									type: 'get',
									dataType: 'json',
									cache: false,
									url: 'DeleteOperateTrend',
									data: 'id=' + trendGuid,
									success: function (response) {
										var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };
										FMErrorAndExceptionHandling.HandleMessages(response, null);
									},
									error: function (xhr, textStatus, error) {
									// need to make  sure that the error we are getting is because we close the page before getting the response
									if (xhr.status != 0) {
										FMErrorAndExceptionHandling.ShowException(xhr,textStatus,error, null);
									}
								}
							});
							}
							, null);
};

FMOperateIndex.openTrend = function (TrendId, settings, newTrendDisplay) {
	// this called directly when openning a new trend
	// can't open trend in a group if there are no groups (there is always 1 tab, the tab to add groups )
	if (FMOperateIndex.isTabGroupEnabled && $('#tabList').children().length === 1) {
		FMLayout.Alert('Cannot open a Trend without a Group.', 'Error');
		return null;
	}

	if (!settings || !settings.hasOwnProperty('guid')) {
		return null;
	}

	// Persist the new tab so it can be re-open when the screen is reloaded
	var activeTab = 'mainTab';
	if (FMOperateIndex.isTabGroupEnabled) {
		activeTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
	}

	FMOperateIndex.UpdateOperateHelpKey("trend");

	//check to see if we have one open already (only when clicking on the menu, not when restoring the screen)
	if (FMOperateIndex.restoringView === false) {
		var parentTab = "";
		var trendTab = "";
		
		var parentGroup = FMOperateIndex.contents;

		if (activeTab !== 'mainTab') {
			parentTab = activeTab;
			var parentGroupFiltered = $.grep(parentGroup, function (e) {
				return e.id === activeTab;
			});
			if (parentGroupFiltered.length > 0) {
				parentGroup = parentGroupFiltered[0];
			}
			$.each(parentGroup.settings, function (index, level1Tab) {
				if (level1Tab.type === "trend" && level1Tab.settings.guid === settings.guid) {
					trendTab = level1Tab.id;
				}
			});
		}
		else {
			$.each(parentGroup, function (index, level1Tab) {
				if (level1Tab.type === "trend" && level1Tab.settings.guid === settings.guid) {
					trendTab = level1Tab.id;
				}
			});
		}

		if (parentTab !== "") {
			$('a[data-target=\'#' + parentTab + '\'').click();
		}
		if (trendTab !== "") {
			$('a[data-target=\'#' + trendTab + '\'').click();
			return false;
		}
	}
	FMOperateIndex.restoringView = false;
	return FMTrendIndex.openTrend(TrendId, settings, newTrendDisplay);
}

//===============================================================
// This function will open a selected report.
//===============================================================
FMOperateIndex.OpenReport = function( reportName, settings,beingRestored )
{
	// Cannot open report in a group if there are no groups (there is always 1 tab, the tab to add groups )
	if (FMOperateIndex.isTabGroupEnabled && $('#tabList').children().length === 1)
	{
		FMLayout.Alert('Cannot open a Report without a Group.', 'Error');
		return null;
	}

	// Persist the new tab so it can be re-open when the screen is reloaded
	var activeTab = 'mainTab';
	if (FMOperateIndex.isTabGroupEnabled)
	{
		activeTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
	}

	//check to see if we have one open already (only when clicking on the menu, not when restoring the screen)
	if (FMOperateIndex.restoringView === false) {
		var parentTab = "";
		var reportTab = "";


		FMOperateIndex.UpdateOperateHelpKey("report");
		var parentGroup = FMOperateIndex.contents;

		if ( activeTab !== 'mainTab' )
		{
			parentTab = activeTab;
			var parentGroupFiltered = $.grep( parentGroup, function( e )
			{
				return e.id === activeTab;
			} );
			if ( parentGroupFiltered.length > 0 )
			{
				parentGroup = parentGroupFiltered[0];
			}
			$.each( parentGroup.settings, function( index, level1Tab )
			{
				if ( level1Tab.type === "report" && level1Tab.settings.guid === settings.guid )
				{
					reportTab = level1Tab.id;
				}
			} );
		}
		else
		{
			$.each( parentGroup, function( index, level1Tab )
			{
				if ( level1Tab.type === "report" && level1Tab.settings.guid === settings.guid )
				{
					reportTab = level1Tab.id;
				}
			} );
		}

		if (parentTab !== "") {
			$('a[data-target=\'#' + parentTab + '\'').click();
		}
		if (reportTab !== "") {
			$('a[data-target=\'#' + reportTab + '\'').click();
			return false;
		}
	}

	FMOperateIndex.restoringView = false;

	var newId = FMOperateIndex.AddTab(reportName, false);

	if (newId === null) return false;

	var activeDrawing = FMOperateIndex.numberDrawings = FMOperateIndex.numberDrawings + 1;

	// do not specify the settings when creating the drawing.  We will update it when we retrieve the actual drawing
	FMOperateIndex.PersistNewControl(activeTab, newId, reportName, 'report', {});

	FMOperateIndex.restoringScreenQueueInProgress[newId] = true;

	$('<div id="loader' + newId + '0" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#' + newId);

	// put messages on the actual tab
	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#diagram' + activeDrawing) };
	FMOperateIndex.PersistUpdateDrawingSettings(activeTab, newId, { guid: settings.guid, drawingIndex: activeDrawing });
	if (beingRestored === true)
	{
		setTimeout( function()
		{
					FMOperateIndex.RefreshReportTimer(activeDrawing, activeTab, newId, reportName, settings.guid, stack_bottomright_operatortab, 1, 5+FMOperateIndex.reportRedrawQueue * 5);
					FMOperateIndex.reportRedrawQueue++;
		}, 1000);
		FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		$('#loader' + newId + '0').remove();
	}
	else {
		$.ajax({
			type: 'get',
			dataType: 'json',
			cache: false,
			url: 'GetOperateReport',
			activeDrawing: activeDrawing,
			activeTab: activeTab,
			newId: newId,
			data: { reportName: reportName, reportGuidStr: settings.guid },
			success: function (response) {
					var activeDrawing = this.activeDrawing;
					var activeTab = this.activeTab;
					var newId = this.newId;

					$('#loader' + newId + '0').remove();
					var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

					FMErrorAndExceptionHandling.HandleMessages(response,
						function (data, inError) {
								if (!inError) {
									var reportInfo = response.Data;

									$('#' + newId + '-tab-name').text(reportName);

									$('<div id="reportDisplay' + activeDrawing + '" style="position: absolute; padding-top: 5px; padding-bottom: 5px; padding-left: 5px; padding-right: 5px; width: 100%; height: 100%"></div>').appendTo('#' + newId);

									if (reportInfo == null) {
										$('<label>Report (' + reportName + ') not found.</label>').appendTo('#reportDisplay' + activeDrawing);
									}
									else {
										var parts = reportInfo.split("|");
										var actualReportName = parts[0];
										var csrfToken = parts[1];

										var srcUrl = "../../FMReportWebMain/ReportMvcLandingPage.aspx?ReportType=1&ReportName=" + actualReportName + "&CSRFToken=" + csrfToken;

										//$('<iframe runat="server" id="ReportIFrame_' + activeDrawing + '" src="' + srcUrl + '" width="100%" height="' + iframeHeight + 'px"></iframe>').appendTo('#reportDisplay' + activeDrawing);
										$('<iframe runat="server" id="ReportIFrame_' + activeDrawing + '" src="' + srcUrl + '" style="width: 100%; height: 100%"></iframe>').appendTo('#reportDisplay' + activeDrawing);
									}
									
									FMOperateIndex.PersistUpdateDrawingSettings(activeTab, newId, { guid: settings.guid, drawingIndex: activeDrawing });
								}
						},
						messageAttributes);
					// done reloading the tab
					FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
			},
			error: function (xhr, textStatus, error) {
					var activeDrawing = this.activeDrawing;
					var activeTab = this.activeTab;
					var newId = this.newId;

					if (xhr.status !== 0) {
						FMErrorAndExceptionHandling.ShowException(xhr,
								textStatus,
								error,
								function () {
									$('#loader' + newId + '0').remove();
								});
					}

					// done reloading the tab
					FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
			}
		});
	}
	return newId;
}

FMOperateIndex.RefreshReportTimer = function (activeDrawing, activeTab, newId, reportName, guid, stack_bottomright_operatortab,iterationNumber, iterationMax)
{
	clearTimeout(FMOperateIndex.reportRedrawTimer);
	FMOperateIndex.reportRedrawTimer = null;

	var activeTab = FMOperateIndex.GetActiveTab("report", newId);
	if (iterationNumber < iterationMax && activeTab === false)
	{
		++iterationNumber;
			setTimeout(function ()
			{
				FMOperateIndex.RefreshReportTimer( activeDrawing, activeTab, newId, reportName, guid, stack_bottomright_operatortab, iterationNumber, iterationMax );
			}, 1000);
		return;
	}

	$.ajax({
		type: 'get',
		dataType: 'json',
		cache: false,
		url: 'GetOperateReport',
		activeDrawing: activeDrawing,
		activeTab: activeTab,
		newId: newId,
		data: { reportName: reportName, reportGuidStr: guid },
		success: function (response) {
				var activeDrawing = this.activeDrawing;
				var activeTab = this.activeTab;
				var newId = this.newId;

				$('#loader' + newId + '0').remove();
				var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

				FMErrorAndExceptionHandling.HandleMessages(response,
				function (data, inError) {
					if (!inError) {
						var reportInfo = response.Data;

						$('#' + newId + '-tab-name').text(reportName);

						$('<div id="reportDisplay' + activeDrawing + '" style="position: absolute; padding-top: 5px; padding-bottom: 5px; padding-left: 5px; padding-right: 5px; width: 100%; height: 100%"></div>').appendTo('#' + newId);

						if (reportInfo == null) {
								$('<label>Report (' + reportName + ') not found.</label>').appendTo('#reportDisplay' + activeDrawing);
						}
						else {
								var parts = reportInfo.split("|");
								var actualReportName = parts[0];
								var csrfToken = parts[1];

								var srcUrl = "../../FMReportWebMain/ReportMvcLandingPage.aspx?ReportType=1&ReportName=" + actualReportName + "&CSRFToken=" + csrfToken;

								//$('<iframe runat="server" id="ReportIFrame_' + activeDrawing + '" src="' + srcUrl + '" width="100%" height="' + iframeHeight + 'px"></iframe>').appendTo('#reportDisplay' + activeDrawing);
								$('<iframe runat="server" id="ReportIFrame_' + activeDrawing + '" src="' + srcUrl + '" style="width: 100%; height: 100%"></iframe>').appendTo('#reportDisplay' + activeDrawing);
						}
						
						FMOperateIndex.PersistUpdateDrawingSettings(activeTab, newId, { guid: guid, drawingIndex: activeDrawing });
					}
				},
				messageAttributes);
				// done reloading the tab
				FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		},
		error: function (xhr, textStatus, error) {
				var activeDrawing = this.activeDrawing;
				var activeTab = this.activeTab;
				var newId = this.newId;

				if (xhr.status !== 0) {
					FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
						$('#loader' + newId + '0').remove();
					});
				}

				// done reloading the tab
				FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		}
	});
}


// Get the selected view for the filters in the menu items that was stored previously
FMOperateIndex.GetSelectedFilterView = function( controlId )
{
	var value = 'detailView';
	FMOperateIndex.contents = $.map( FMOperateIndex.contents, function( obj )
	{
		if ( obj.id === controlId )
		{
				value = obj.name;
		}
		return obj;
	} );
	console.log("FMOperateIndex.PersistScreenConfiguration called from FMOperateIndex.GetSelectedFilterView");
	FMOperateIndex.PersistScreenConfiguration();
	return value;
};

// open a point from the menu
FMOperateIndex.openPoint = function( pointId, pointGuid )
{
	// can't open point in a group if there are no groups (there is always 1 tab, the tab to add groups )
	if ( FMOperateIndex.isTabGroupEnabled && $( '#tabList' ).children().length === 1 )
	{
		FMLayout.Alert( 'Cannot open a Point without a Group.', 'Error' );
		return null;
	}

	return FMOperateIndex.openDraw( pointId, { drawingGuid: null, pointGuid: pointGuid } );
};

FMOperateIndex.openPointHistory = function (pointId, pointGuid, start, interval, intervalQuantity, range, rangeQuantity, columns) {
	var pointHistoryId = pointId;
	
	// can't open point history in a group if there are no groups (there is always 1 tab, the tab to add groups )
	if (FMOperateIndex.isTabGroupEnabled && $('#tabList').children().length === 1) {
		FMLayout.Alert('Cannot open a Point History without a Group.', 'Error');
		return null;
	}

	var activeTab = 'mainTab';
	if (FMOperateIndex.isTabGroupEnabled) {
		activeTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
	}

	if (FMOperateIndex.restoringView === false) {
		var parentTab = "";
		var pointhistoryTab = "";

		var parentGroup = FMOperateIndex.contents;

		FMOperateIndex.UpdateOperateHelpKey("pointHistory");

		if (activeTab !== 'mainTab') {
			parentTab = activeTab;
			var parentGroupFiltered = $.grep(parentGroup, function (e) {
				return e.id === activeTab;
			});
			if (parentGroupFiltered.length > 0) {
				parentGroup = parentGroupFiltered[0];
			}

			$.each(parentGroup.settings, function (index, level1Tab) {
				if (level1Tab.type === "pointHistory" && level1Tab.settings.pointGuid === pointGuid) {
					pointhistoryTab = level1Tab.id;
				}
			});
		}
		else {
			$.each(parentGroup, function (index, level1Tab) {
				if (level1Tab.type === "pointHistory" && level1Tab.settings.pointGuid === pointGuid) {
					pointhistoryTab = level1Tab.id;
				}
			});
		}

		if (parentTab !== "") {
			$('a[data-target=\'#' + parentTab + '\'').click();
		}
		if (pointhistoryTab !== "") {
			$('a[data-target=\'#' + pointhistoryTab + '\'').click();
			return false;
		}
	}

	var newId = FMOperateIndex.AddTab(pointHistoryId);
	if (newId === null) return false;

		FMOperateIndex.PersistNewControl(activeTab, newId, pointHistoryId, 'pointHistory', { pointId: pointId, pointGuid: pointGuid});
	// start the process of restoring the tab
	FMOperateIndex.restoringScreenQueueInProgress[newId] = true;

	$('<div id="pointhistory' + newId + '" class="point-history pointhistorytemp" style="position: absolute; top: 5px; left: 5px;right: 5px;bottom: 5px;"></div>').appendTo('#' + newId);
	$('<div id="pointhistory' + newId + 'container" class="" style="overflow:hidden"></div>').appendTo('#pointhistory' + newId);

	$('<div id="loaderpointhistory' + newId + '" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#' + newId);

	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#pointhistory' + newId) };

	$('#' + newId + '-tab-name').text('');
	var imageId = "PointHistoryTabImageId-" + newId;
	console.log(activeTab);
	$('<id="TextImage' + newId + '0" ><img id="' + imageId + '" src="' + window.applicationRootName + '/FMWebApp/images/pointHistory.png" >' + pointHistoryId + '</>').appendTo('#' + newId + '-tab-name');

	$.ajax({
		type: 'post',
		dataType: 'html',
		cache: false,
		url: $('#urlGetPointHistoryTabView').val(),
		activeTab: activeTab,
		newId: newId,
		contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
		data: { "tabId": activeTab, "controlId": newId, "pointId": pointId, "pointGuid": pointGuid, "start": start, "interval": interval, "intervalQuantity": Math.trunc(intervalQuantity), "range": range, "rangeQuantity": Math.trunc(rangeQuantity), "columns": JSON.stringify(columns) },
		success: function (response) {
			var activeTab = this.activeTab;
			var newId = this.newId;

			var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

			$("#pointhistory" + newId + "container").html(response);
			$("#loaderpointhistory" + newId).remove();

			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
			PointHistoryTab.PersistPointHistory({ tabId: activeTab, controlId: newId, pointId: pointId, pointGuid: pointGuid, start: $("#PointHistoryStartTimePicker_" + newId).val(), interval: $("#PointHistoryIntervalSelect_" + newId).val(), intervalQuantity: $("#PointHistoryIntervalQuantity_" + newId).val(), range: $("#PointHistoryRangeSelect_" + newId).val(), rangeQuantity: $("#PointHistoryRangeQuantity_" + newId).val(), columns: $("#PointHistoryColumns_" + newId).val() });
		},
		error: function (xhr, textStatus, error) {
			var activeTab = this.activeTab;
			var newId = this.newId;

			// need to make sure that the error we are getting is because we close the page before getting the response
			if (xhr.status != 0) {
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
						$("#loaderpointhistory" + newId).remove();
					});
			}
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		}
	});

	return newId;
}

FMOperateIndex.openPointCalculator = function (pointId, pointGuid)
{
	var url = $("#urlPointCalculator").val();
	FMPointCalculator.GetForm(url, pointId, pointGuid, true);
}

FMOperateIndex.openPointLeakAnalysis = function (pointId, pointGuid) {
	var url = $('#urlPointLeakAnalysis').val();
	FMPointLeakAnalysis.GetForm(url, pointId, pointGuid);
}

FMOperateIndex.GetActiveTab = function (TabType, TabID) { //bds
	var returnValue = false;
	
	//if (FMOperateIndex.restoringView === false && FMOperateIndex.allScreensRestored === true) {
	if (FMOperateIndex.allScreensRestored === true) {
			var parentTab = "";
		var summaryTab = "";

		$.each(FMOperateIndex.contents, function (index, level1Tab) {
				if (level1Tab.type === "group") {
					$.each(FMOperateIndex.contents[index].settings, function (index, level2Tab) {
						if (level2Tab.type === TabType && level2Tab.id == TabID) {
								if (level2Tab.active === true)
									returnValue = true;
						}
					});
				} else if (level1Tab.type === TabType) {
					if (level1Tab.active === true)
						returnValue = true;
				}
		});
	}
	return returnValue;
}

FMOperateIndex.openAlarmSummary = function (alarmSummaryId) {
	//check to see if we have one open already (only when clicking on the menu, not when restoring the screen)
	if (FMOperateIndex.restoringView === false) {
		var parentTab = "";
		var summaryTab = "";
		FMOperateIndex.UpdateOperateHelpKey("alarmSummary");

		$.each(FMOperateIndex.contents, function (index, level1Tab) {
			if (level1Tab.type === "group") {
				$.each(FMOperateIndex.contents[index].settings, function (index, level2Tab) {
					if (level2Tab.type === "alarmSummary") {
						summaryTab = level2Tab.id;
						parentTab = level1Tab.id;
					}
				});
			} else if (level1Tab.type === "alarmSummary") {
				summaryTab = level1Tab.id;
				parentTab = "";
			}
		});

		if (parentTab !== "") {
			$('a[data-target=\'#' + parentTab + '\'').click();
		}
		if (summaryTab !== "") {
			$('a[data-target=\'#' + summaryTab + '\'').click();
			return false;
		}
	}

	// can't open Alarm Summary in a group if there are no groups (there is always 1 tab, the tab to add groups )
	if (FMOperateIndex.isTabGroupEnabled && $('#tabList').children().length === 1) {
		FMLayout.Alert('Cannot open a Alarm Summary without a Group.', 'Error');
		return null;
	}
	FMOperateIndex.restoringView = false;

	if (!alarmSummaryId) {
		// this should only be hit when openning a new summary. Not restoring.
		FMOperateIndex.openingNewAlarmSummary = true;
		alarmSummaryId = 'Alarm Summary';
	}
	else {
		FMOperateIndex.openingNewAlarmSummary = false;
	}

	var newId = FMOperateIndex.AddTab(alarmSummaryId);

	if (newId === null) return false;

	var activeTab = 'mainTab';
	if (FMOperateIndex.isTabGroupEnabled) {
		activeTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
	}
	FMOperateIndex.PersistNewControl(activeTab, newId, alarmSummaryId, 'alarmSummary', {});

	// start the process of restoring the tab
	FMOperateIndex.restoringScreenQueueInProgress[newId] = true;

	$("#" + newId).getNiceScroll().remove();

	$('<div id="alarmsummary' + newId + '" class="alarm-summary" style="position: absolute; top: 5px; left: 5px;right: 5px;bottom: 5px;"></div>').appendTo('#' + newId);
	$('<div id="alarmsummary' + newId + 'container" class="point-group-panel active" style="overflow:hidden"></div>').appendTo('#alarmsummary' + newId);

	$('<div id="alarmsummarytabname" style="hidden" >' + newId + '</div>').appendTo('#alarmsummary' + newId);

	$('<div id="loaderalarmsummary' + newId + '" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#' + newId);
	// put messages on the actual tab
	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#alarmsummary' + newId) };

	$.ajax({
		type: 'get',
		dataType: 'json',
		cache: false,
		url: $("#urlAlarmSummary").val(),
		activeTab: activeTab,
		newId: newId,
		data: {},
		success: function (response) {
			var activeTab = this.activeTab;
			var newId = this.newId;

			$("#loaderalarmsummary" + newId).remove();

			var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

			FMErrorAndExceptionHandling.HandleMessages(response,
				function (alarmSummaryView, inError) {
					// if it was not in error load and update the drawing
					if (!inError) {

						// Double click on the tab name to rename the point group
						$('a[data-target="#' + newId + '"]').attr('ondblclick', "FMOperateIndex.RenameTab( this );");

						$("#alarmsummary" + newId + "container").html(alarmSummaryView);
						//AlarmSummaryTab.StartTimer();

						// done with the process of restoring the tab
						FMOperateIndex.restoringScreenQueueInProgress[newId] = false;

					}
				},
				messageAttributes);
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		},
		error: function (xhr, textStatus, error) {
			var newId = this.newId;

			// need to make  sure that the error we are getting is because we close the page before getting the response
			if (xhr.status != 0) {
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
						$("#loaderalarmsummary" + newId).remove();
					});
			}
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		}
	});

	return newId;

};

FMOperateIndex.openAlarmHistory = function (alarmHistoryId) {

	//check to see if we have one open already (only when clicking on the menu, not when restoring the screen)
	if (FMOperateIndex.restoringView === false) {
		var parentTab = "";
		var summaryTab = "";

		FMOperateIndex.UpdateOperateHelpKey("alarmHistory");
		$.each(FMOperateIndex.contents, function (index, level1Tab) {
			if (level1Tab.type === "group") {
				$.each(FMOperateIndex.contents[index].settings, function (index, level2Tab) {
					if (level2Tab.type === "alarmHistory") {
						summaryTab = level2Tab.id;
						parentTab = level1Tab.id;
					}
				});
			} else if (level1Tab.type === "alarmHistory") {
				summaryTab = level1Tab.id;
				parentTab = "";
			}
		});

		if (parentTab !== "") {
			$('a[data-target=\'#' + parentTab + '\'').click();
		}
		if (summaryTab !== "") {
			$('a[data-target=\'#' + summaryTab + '\'').click();
			return false;
		}
	}

	// can't open Alarm Summary in a group if there are no groups (there is always 1 tab, the tab to add groups )
	if (FMOperateIndex.isTabGroupEnabled && $('#tabList').children().length === 1) {
		FMLayout.Alert('Cannot open a Alarm History without a Group.', 'Error');
		return null;
	}
	FMOperateIndex.restoringView = false;

	if (!alarmHistoryId) {
		// this should only be hit when openning a new summary. Not restoring.
		FMOperateIndex.openingNewAlarmHistory = true;

		alarmHistoryId = 'Alarm & Event History';
	}
	else
	{
		FMOperateIndex.openingNewAlarmHistory = false;
	}

	var newId = FMOperateIndex.AddTab(alarmHistoryId);

	if (newId === null) return false;

	var activeTab = 'mainTab';
	if (FMOperateIndex.isTabGroupEnabled) {
		activeTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
	}
	FMOperateIndex.PersistNewControl(activeTab, newId, alarmHistoryId, 'alarmHistory', {});

	// start the process of restoring the tab
	FMOperateIndex.restoringScreenQueueInProgress[newId] = true;

	$("#" + newId).getNiceScroll().remove();

	$('<div id="alarmhistory' + newId + '" class="alarm-history" style="position: absolute; top: 5px; left: 5px;right: 5px;bottom: 5px;"></div>').appendTo('#' + newId);
	$('<div id="alarmhistory' + newId + 'container" class="point-group-panel active" style="overflow:hidden"></div>').appendTo('#alarmhistory' + newId);

	$('<div id="alarmhistorytabname" style="hidden" >' + newId + '</div>').appendTo('#alarmhistory' + newId);

	$('<div id="loaderalarmhistory' + newId + '" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#' + newId);
	// put messages on the actual tab
	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#alarmhistory' + newId) };

	$.ajax({
		type: 'get',
		dataType: 'json',
		cache: false,
		url: $("#urlAlarmHistory").val(),
		activeTab: activeTab,
		newId: newId,
		data: {},
		success: function (response) {
			var activeTab = this.activeTab;
			var newId = this.newId;

			$("#loaderalarmhistory" + newId).remove();

			var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

			FMErrorAndExceptionHandling.HandleMessages(response,
				function (alarmHistoryView, inError) {
					// if it was not in error load and update the drawing
					if (!inError) {

						// Double click on the tab name to rename the point group
						$('a[data-target="#' + newId + '"]').attr('ondblclick', "FMOperateIndex.RenameTab( this );");

						$("#alarmhistory" + newId + "container").html(alarmHistoryView);

						// done with the process of restoring the tab
						FMOperateIndex.restoringScreenQueueInProgress[newId] = false;

					}
				},
				messageAttributes);
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		},
		error: function (xhr, textStatus, error) {
			var newId = this.newId;

			// need to make  sure that the error we are getting is because we close the page before getting the response
			if (xhr.status != 0) {
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
						$("#loaderalarmhistory" + newId).remove();
					});
			}
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		}
	});

	return newId;

}

// main event executed after the load of the screen is completed
$( document ).ready( function()
{
	console.log("Operate monitors detected: " + FMOperateIndex.GetDetectedMonitorCount());

	$.ajaxSetup( {
		type: 'POST',
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		headers: { __RequestVerificationToken: $( 'input[name="__RequestVerificationToken"]' ).val() },
		cache: false,
		traditional: true
	} );

	setTimeout(function () {
		FMOperateIndex.StartFallbackPersistedControlRestore();
	}, 1000);

	var numFormatInfoString = $( '#NumberFormatInfoString' ).val();
	var dateTimeFormatInfoString = $( '#DateTimeFormatInfoString' ).val();
	FMOperateIndex.numformatInfo = JSON.parse( numFormatInfoString );
	FMOperateIndex.dateTimeFormatInfo = JSON.parse( dateTimeFormatInfoString );

	// Retrieve Point Product Graphic info
	FMOperateIndex.RetrievePointProductGraphicInfo();

	// call to update the Tab Bar tag system time, must be performed post ready
	setTimeout(FMOperateIndex.UpdateTabBarTags, 0);

	FMOperateIndex.annunciationAnimationTimer = setTimeout(FMOperateIndex.DoAlarmAnnunciationAndAnimation, 1000);

	// call to update the tags
	FMOperateIndex.UpdateTags();

	window.onbeforeunload = function (e) {
		FMOperateIndex.PersistOpenScreenConfigurations();

		if (FMOperateIndex.screens.length > 0) {
			for (let screen of FMOperateIndex.screens) {
				screen.close();
			}
		}

		FMOperateIndex.StopOperateStatistics();

		if (FMOperateIndex.contents == null) {
			return;
		}

		if (FMOperateIndex.annunciationAnimationTimer != null) {
			clearTimeout(FMOperateIndex.annunciationAnimationTimer);
		}

		FMOperateIndex.contents.forEach(function (tab)
		{
			if (tab.type === 'group')
			{
				tab.settings.forEach(function (setting)
				{
					if (setting.type === 'trend')
					{
						FMTrendIndex.closeTrend(setting.settings.drawingIndex);
					}
					else if (setting.type === 'drawing') {
						FMOperateIndex.CloseDrawing($('#' + setting.id + ' .FMDrawing').attr('data-drawing-number'));
					}
					else if (setting.type === 'pointGroup')
					{
						FMOperateIndex.closePointGroup( setting.id );
					}
					else if (setting.type === 'movementSummary')
					{
						FMOperateIndex.CloseMovementSummary(setting.id);
					}
				});
			}
			else if (tab.type === 'trend')
			{
				FMTrendIndex.closeTrend(tab.settings.drawingIndex);
			}
			else if (tab.type === 'drawing') {
				FMOperateIndex.CloseDrawing($('#' + tab.settings.id + ' .FMDrawing').attr('data-drawing-number'));
			}
			else if (tab.type === 'pointGroup')
			{
				FMOperateIndex.closePointGroup( tab.id );
			}
			else if (tab.type === 'movementSummary')
			{
				FMOperateIndex.CloseMovementSummary(tab.id);
			}
			else if (tab.type === 'movementHistory')
			{
				FMOperateIndex.CloseMovementHistory(tab.id);
			}
			else if (tab.type === 'alarmHistory') {
				AlarmHistoryTab.CloseAlarmHistory();
			}
		});


		if (FMOperateIndex.deferredUpdateTagsAjaxRequest) {
			FMOperateIndex.deferredUpdateTagsAjaxRequest.forEach(function (ajaxRequest) {
				ajaxRequest.abort();
			});
		}

		FMOperateIndex.deferredUpdateTagsAjaxRequest = null;
	};

	// define click handling event for the configuration icon on the tabs
	$("#tabs").on('click', '.tab-configuration-menu', function (e)
	{
		// find the active tab since the configuration is going to affect this tab
		var tabPointer = $(this).parent().find("ul.nav-tabs > li.active > a");

		if ( tabPointer.length > 0 )
		{
			var tabContent = $($(this).parent().find("ul.nav-tabs > li.active > a").attr('data-target'));
			var parentGroupTab = 'mainTab';

			if (FMOperateIndex.isTabGroupEnabled)
			{
				parentGroupTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
			}

			// if dealing with point groups we can modify several settings
			if ( tabContent.children().first().hasClass( "point-group" ) )
			{
				FMOperateIndex.updatePointGroupSettings( this, $( tabContent ).attr( "id" ), parentGroupTab );
			}
			else if (tabContent.children().first().hasClass("FMTrend"))
			{
				FMTrendIndex.editTrend(this, tabContent.children().first().attr("id"), parentGroupTab);
			}
			else if (tabContent.children().first().hasClass("movement-summary"))
			{
				FMOperateIndex.UpdateMovementSummarySettings(this, $(tabContent).attr("id"), parentGroupTab);
			}
			else if (tabContent.children().first().hasClass("point-history")) {
				FMOperateIndex.UpdatePointHistorySettings(this, $(tabContent).attr("id"), parentGroupTab);
			}
		}
	});
});

FMOperateIndex.RemoveRedundantPointValueIdentfiers = function( originalArray )
{
	var obj = {};
	var returnArray = [];
	if ( !originalArray || originalArray.length < 1 )
	{
		return returnArray;
	}
	for ( var i = 0; i < originalArray.length; i++ )
	{
		var pointValueIdentifier = originalArray[i];
		var pointValueIdentifierKey = FMOperateIndex.CreatePointValueIdentifierKey(pointValueIdentifier);
		var existingPointValueIdentifierEntry = obj[pointValueIdentifierKey];
		if ( existingPointValueIdentifierEntry )
		{
			if ( pointValueIdentifier.IncludeAlarmLimits !== existingPointValueIdentifierEntry.IncludeAlarmLimits )
			{
				existingPointValueIdentifierEntry.IncludeAlarmLimits = true;
			}
		}
		else
		{
			obj[pointValueIdentifierKey] = pointValueIdentifier;
		}
	}
	for ( var key in obj )
	{
		if ( obj.hasOwnProperty( key ) )
		{
			returnArray.push( obj[key] );
		}
	}
	return returnArray;
};

FMOperateIndex.CreatePointValueIdentifierKey = function( pointValueIdentifier )
{
	if ( pointValueIdentifier )
	{
		var pointValueIdentifierKey = pointValueIdentifier.IdentityGuid + "," + pointValueIdentifier.PointValueType + "," + pointValueIdentifier.PropertyID;
		return pointValueIdentifierKey;
	}
	return null;
};

FMOperateIndex.CreatePointValueIdentifierFromNode = function( node )
{
	if ( node && node.data && node.name && node.data.TagGUID) {
		if (node.name === 'Tag' || node.name === 'Bar' || node.name === "button"){
			var includeAlarmLimits = false;
			if ( node.name === 'Bar' )
			{
				includeAlarmLimits = node.data.useAlarmLevel;
			}
			var pointValueIdentifier = { IdentityGuid: node.data.TagGUID, PointValueType: node.data.TagPointValueType, PropertyID: node.data.TagPropertyID, IncludeAlarmLimits: includeAlarmLimits, UtcTicks: '0' };
			return pointValueIdentifier;
		}
	}
	return null;
};

FMOperateIndex.CreatePointValueIdentifierFromTestGroupPointValueInfo = function( testGroupPointValueInfo )
{
	if ( testGroupPointValueInfo )
	{
		var pointValueIdentifier = { IdentityGuid: testGroupPointValueInfo.PointValueGuid, PointValueType: testGroupPointValueInfo.ValueType, PropertyID: testGroupPointValueInfo.PointValueID, IncludeAlarmLimits: false };

		//SRM -- Don't know why I have to do the following, but it throws an exception otherwise.  PointValueIdentifier should be fixed.
		if ( testGroupPointValueInfo.ValueType === 0 )
		{
			pointValueIdentifier.PropertyID = null;
		}
		return pointValueIdentifier;
	}

	return null;
};

FMOperateIndex.GetPointValueIdentifierArrayFromNodes = function( nodes )
{
	var pointValueIdentifiers = [];
	nodes.forEach( function( node )
	{
		if ( node.data.AnimationPointValueAssignments && node.data.AnimationPointValueAssignments.TestGroupPointValueInfoList )
		{
			var testGroupPointValueInfoList = node.data.AnimationPointValueAssignments.TestGroupPointValueInfoList;
			for ( var i = 0; i < testGroupPointValueInfoList.length; i++ )
			{
				var testGroupPointValueInfo = testGroupPointValueInfoList[i];
				var pointValueIdentifier2 = FMOperateIndex.CreatePointValueIdentifierFromTestGroupPointValueInfo(testGroupPointValueInfo);
				if ( pointValueIdentifier2 )
				{
					pointValueIdentifiers.push( pointValueIdentifier2 );
				}
			}
		}
		var pointValueIdentifier = FMOperateIndex.CreatePointValueIdentifierFromNode(node);
		if ( pointValueIdentifier )
		{
			pointValueIdentifiers.push( pointValueIdentifier );
		}
	} );

	return FMOperateIndex.RemoveRedundantPointValueIdentfiers( pointValueIdentifiers );
};

// get the latest values for the Tab Bar Tags
FMOperateIndex.UpdateTabBarTags = function() {
	var pointValueIdentifiers = [];

	var timePointValueIdentifier = { IdentityGuid: '089ede41-6b8e-4e3b-b498-22da5b79f2f2', PointValueType: 0, PropertyID: null, IncludeAlarmLimits: false, UtcTicks: '0' };
	pointValueIdentifiers.push(timePointValueIdentifier)
	var userPointValueIdentifier = { IdentityGuid: 'db8a725d-8c31-45b1-bfc9-439bec87b30c', PointValueType: 0, PropertyID: null, IncludeAlarmLimits: false, UtcTicks: '0'  };
	pointValueIdentifiers.push(userPointValueIdentifier)
	var licenseExpiryPointValueIdentifier = { IdentityGuid: '54838e9b-55e8-483a-ad59-df27691bb97a', PointValueType: 0, PropertyID: null, IncludeAlarmLimits: false, UtcTicks: '0'  };
	pointValueIdentifiers.push(licenseExpiryPointValueIdentifier)

	FMOperateIndex.subscribeTagWebWorker( 'tabbar', pointValueIdentifiers, function( data ) {
		FMOperateIndex.UpdateTabBarTagsSuccess( data );
	}, true );
};


// get the lates values for the Point Tags
FMOperateIndex.UpdateTags = function(){
	var nodes = FMOperateIndex.activeObjects;
	var pointValueIdentifiers = FMOperateIndex.GetPointValueIdentifierArrayFromNodes(nodes);
	if ( pointValueIdentifiers.length > 0 )	{
		FMOperateIndex.subscribeTagWebWorker( 'graphics', pointValueIdentifiers, function( data ) {
			FMOperateIndex.UpdateTagsSuccess( data );
		}, false );
	}
	else {
		FMOperateIndex.unsubscribeTagWebWorker( 'graphics' );
	}

};

FMOperateIndex.DoAlarmAnnunciationColoring = function(node, dataModel)
{
	var pointValue = node.data.TagPointValue;

	if (!pointValue.CommunicationsFailure) {

		var alarmPriority = FMOperateIndex.GetAlarmPrioritiesByGuid(pointValue.AlarmPriorityGuid);
		//First time through for the node preserve the original color and stroke for use when not alarmed and acknowledged
		if ( !node.data.originalColor || !node.data.originalStroke  )
		{
			dataModel.setDataProperty(node.data, 'originalColor', node.data.color);
			dataModel.setDataProperty(node.data, 'originalStroke', node.data.stroke);
		}

		if (alarmPriority && (pointValue.Access.View == true || pointValue.Access.Modify == true)) {  //don't render the alarm animation if we have no access
			var color = '#ffffff';
			var textColor = '#000000';

			if ( pointValue.Acknowledged === false && FMOperateIndex.displayAlternateIfUnacknowledged ) {
				color = '#' + alarmPriority.BackgroundAlternate;
				textColor = '#' + alarmPriority.TextAlternate;
			}
			else {
				color = '#' + alarmPriority.BackgroundSteady;
				textColor = '#' + alarmPriority.TextSteady;
			}

			var rgbObj = FMDrawPropertyMenu.Rgb2Hex(color);
			var rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString(rgbObj, 1);
			dataModel.setDataProperty(node.data, 'color', rgbaStr);
			rgbObj = FMDrawPropertyMenu.Rgb2Hex(textColor);
			rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString(rgbObj, 1);
			dataModel.setDataProperty(node.data, 'stroke', rgbaStr);
			node.data.ToolTipString = node.data.TagPointIDAndTagID + ' : ' + FMTAGFIELDSELECTION.GetFieldString(node.data.TagFieldSelection) + '\r\n Alarm State: ' + pointValue.AlarmState;
		}
		else {

			dataModel.setDataProperty(node.data, 'color', node.data.originalColor);
			dataModel.setDataProperty(node.data, 'stroke', node.data.originalStroke);
			node.data.ToolTipString = node.data.TagPointIDAndTagID + ' : ' + FMTAGFIELDSELECTION.GetFieldString(node.data.TagFieldSelection);
			if (pointValue.Access.View == false && pointValue.Access.Modify == false) {
				dataModel.setDataProperty(node.data, 'TagValue', $('#RestrictedText').val());
			}
		}
	}
};

FMOperateIndex.UpdateAlarmLimitsOnBar = function(pointValue, node)
{
	if ( pointValue && pointValue.AlarmLimits)
	{
		for ( var i = 0; i < pointValue.AlarmLimits.length; i++ )
		{
			var alarmLimit = pointValue.AlarmLimits[i];
			var sz = go.Size.parse(node.data.bgsize);
			var geostr = "F M0 0 L" + sz.width + " 0";
			var max = node.data.useTagLimits === "true" ? pointValue.Maximum : node.data.maxUserVal;
			var min = node.data.useTagLimits === "true" ? pointValue.Minimum : node.data.minUserVal;
			if ( alarmLimit.Value >= min && alarmLimit.Value <= max )
			{
				var percentage = ( alarmLimit.Value - min ) / ( max - min );

				var spot = go.Spot.Top;
				var newspot = spot.copy();
				var offsetY = sz.height * ( 1 - percentage );
				var offsetX = 0;
				newspot = newspot.setTo( spot.x, spot.y, offsetX, offsetY );

				var alarmPriority = FMOperateIndex.GetAlarmPrioritiesByGuid( alarmLimit.AlarmPriorityGuid );
				var lineColor = 'red';
				if ( alarmPriority )
				{
					lineColor = '#' + alarmPriority.BackgroundSteady; //'#' + alarmPriority.TextSteady;
				}

				var limitLine = node.findObject( alarmLimit.IdentityGuid );
				if ( limitLine )
				{
					limitLine.geometryString = geostr;
					limitLine.alignment = newspot;
					limitLine.fill = lineColor;
					limitLine.stroke = lineColor;
				}
				else
				{
					var shape = go.GraphObject.make( go.Shape,
						{
							name: alarmLimit.IdentityGuid,
							fill: lineColor,
							strokeWidth: 1.0,
							stroke: lineColor,
							geometryString: geostr,
							alignment: newspot
						} );

					node.add( shape );
				}
			}
		}
	}
}

FMOperateIndex.GetStatusCode = function (status) {
	var codeBits = status & 0x3FFF0000;

	if ((status & 0xC0000000) == 0) {
		switch (codeBits) {
			case 0x00000000:
				return 'Good';
			case 0x002D0000:
				return 'GoodSubscriptionTransferred';
			case 0x002E0000:
				return 'GoodCompletesAsynchronously';
			case 0x002F0000:
				return 'GoodOverload';
			case 0x00300000:
				return 'GoodClamped';
			case 0x00960000:
				return 'GoodLocalOverride';
			case 0x00A20000:
				return 'GoodEntryInserted';
			case 0x00A30000:
				return 'GoodEntryReplaced';
			case 0x00A50000:
				return 'GoodNoData';
			case 0x00A60000:
				return 'GoodMoreData';
			case 0x00A70000:
				return 'GoodCommunicationEvent';
			case 0x00A80000:
				return 'GoodShutdownEvent';
			case 0x00A90000:
				return 'GoodCallAgain';
			case 0x00AA0000:
				return 'GoodNonCriticalTimeout';
			case 0x00BA0000:
				return 'GoodResultsMayBeIncomplete';
			case 0x00D90000:
				return 'GoodDataIgnored';
			case 0x00DC0000:
				return 'GoodEdited';
			case 0x00DD0000:
				return 'GoodPostActionFailed';
			case 0x00E00000:
				return 'GoodDependentValueChanged';
			default:
				return 'Good';
		}
	}

	else if ((status & 0xC0000000) == 0x40000000) {
		switch (codeBits) {

			case 0x00000000:
				return 'Uncertain';
			case 0x006C0000:
				return 'UncertainReferenceOutOfServer';
			case 0x008F0000:
				return 'UncertainNoCommunicationLastUsableValue';
			case 0x00900000:
				return 'UncertainLastUsableValue';
			case 0x00910000:
				return 'UncertainSubstituteValue';
			case 0x00920000:
				return 'UncertainInitialValue';
			case 0x00930000:
				return 'UncertainSensorNotAccurate';
			case 0x00940000:
				return 'UncertainEngineeringUnitsExceeded';
			case 0x00950000:
				return 'UncertainSubNormal';
			case 0x00A40000:
				return 'UncertainDataSubNormal';
			case 0x00BC0000:
				return 'UncertainReferenceNotDeleted'
			case 0x00C00000:
				return 'UncertainNotAllNodesAvailable';
			case 0x00DE0000:
				return 'UncertainDominantValueChanged';
			case 0x00E20000:
				return 'UncertainDependentValueChanged';
			default:
				return 'Uncertain';
		}
	}

	else if ((status & 0xC0000000) < 0) {
		switch (codeBits) {
			case 0x00000000:
				return 'Bad';
			case 0x00010000:
				return 'BadUnexpectedError';
			case 0x000200000:
				return 'BadInternalError';
			case 0x00030000:
				return 'BadOutOfMemory';
			case 0x00040000:
				return 'BadResourceUnavailable';
			case 0x00050000:
				return 'BadCommunicationError';
			case 0x00060000:
				return 'BadEncodingError';
			case 0x00070000:
				return 'BadDecodingError';
			case 0x00080000:
				return 'BadEncodingLimitsExceeded';
			case 0x00B80000:
				return 'BadRequestTooLarge';
			case 0x00B90000:
				return 'BadResponseTooLarge';
			case 0x00090000:
				return 'BadUnknownResponse';
			case 0x000A0000:
				return 'BadTimeout';
			case 0x000B0000:
				return 'BadServiceUnsupported';
			case 0x000C0000:
				return 'BadShutdown';
			case 0x000D0000:
				return 'BadServerNotConnected';
			case 0x000E0000:
				return 'BadServerHalted';
			case 0x000F0000:
				return 'BadNothingToDo';
			case 0x00100000:
				return 'BadTooManyOperations';
			case 0x00DB0000:
				return 'BadTooManyMonitoredItems';
			case 0x00110000:
				return 'BadDataTypeIdUnknown';
			case 0x00120000:
				return 'BadCertificateInvalid';
			case 0x00130000:
				return 'BadSecurityChecksFailed';
			case 0x01140000:
				return 'BadCertificatePolicyCheckFailed';
			case 0x00140000:
				return 'BadCertificateTimeInvalid';
			case 0x00150000:
				return 'BadCertificateIssuerTimeInvalid';
			case 0x00160000:
				return 'BadCertificateHostNameInvalid';
			case 0x00170000:
				return 'BadCertificateUriInvalid';
			case 0x00180000:
				return 'BadCertificateUseNotAllowed';
			case 0x00190000:
				return 'BadCertificateIssuerUseNotAllowed';
			case 0x001A0000:
				return 'BadCertificateUntrusted';
			case 0x001B0000:
				return 'BadCertificateRevocationUnknown';
			case 0x001C0000:
				return 'BadCertificateIssuerRevocationUnknown';
			case 0x001D0000:
				return 'BadCertificateRevoked';
			case 0x001E0000:
				return 'BadCertificateIssuerRevoked';
			case 0x010D0000:
				return 'BadCertificateChainIncomplete';
			case 0x001F0000:
				return 'BadUserAccessDenied';
			case 0x00200000:
				return 'BadIdentityTokenInvalid';
			case 0x00210000:
				return 'BadIdentityTokenRejected';
			case 0x00220000:
				return 'BadSecureChannelIdInvalid';
			case 0x00230000:
				return 'BadInvalidTimestamp';
			case 0x00240000:
				return 'BadNonceInvalid';
			case 0x00250000:
				return 'BadSessionIdInvalid';
			case 0x00260000:
				return 'BadSessionClosed';
			case 0x00270000:
				return 'BadSessionNotActivated';
			case 0x00280000:
				return 'BadSubscriptionIdInvalid';
			case 0x002A0000:
				return 'BadRequestHeaderInvalid';
			case 0x002B0000:
				return 'BadTimestampsToReturnInvalid';
			case 0x002C0000:
				return 'BadRequestCancelledByClient';
			case 0x00E50000:
				return 'BadTooManyArguments';
			case 0x010E0000:
				return 'BadLicenseExpired';
			case 0x010F0000:
				return 'BadLicenseLimitsExceeded';
			case 0x01100000:
				return 'BadLicenseNotAvailable';
			case 0x00310000:
				return 'BadNoCommunication';
			case 0x00320000:
				return 'BadWaitingForInitialData';
			case 0x00330000:
				return 'BadNodeIdInvalid';
			case 0x00340000:
				return 'BadNodeIdUnknown';
			case 0x00350000:
				return 'BadAttributeIdInvalid';
			case 0x00360000:
				return 'BadIndexRangeInvalid';
			case 0x00370000:
				return 'BadIndexRangeNoData';
			case 0x00380000:
				return 'BadDataEncodingInvalid';
			case 0x00390000:
				return 'BadDataEncodingUnsupported';
			case 0x003A0000:
				return 'BadNotReadable';
			case 0x003B0000:
				return 'BadNotWritable';
			case 0x003C0000:
				return 'BadOutOfRange';
			case 0x003D0000:
				return 'BadNotSupported';
			case 0x003E0000:
				return 'BadNotFound';
			case 0x003F0000:
				return 'BadObjectDeleted';
			case 0x00400000:
				return 'BadNotImplemented';
			case 0x00410000:
				return 'BadMonitoringModeInvalid';
			case 0x00420000:
				return 'BadMonitoredItemIdInvalid';
			case 0x00430000:
				return 'BadMonitoredItemFilterInvalid';
			case 0x00440000:
				return 'BadMonitoredItemFilterUnsupported';
			case 0x00450000:
				return 'BadFilterNotAllowed';
			case 0x00460000:
				return 'BadStructureMissing';
			case 0x00470000:
				return 'BadEventFilterInvalid';
			case 0x00480000:
				return 'BadContentFilterInvalid';
			case 0x00C10000:
				return 'BadFilterOperatorInvalid';
			case 0x00C20000:
				return 'BadFilterOperatorUnsupported';
			case 0x00C30000:
				return 'BadFilterOperandCountMismatch';
			case 0x00490000:
				return 'BadFilterOperandInvalid';
			case 0x00C40000:
				return 'BadFilterElementInvalid';
			case 0x00C50000:
				return 'BadFilterLiteralInvalid';
			case 0x004A0000:
				return 'BadContinuationPointInvalid';
			case 0x004B0000:
				return 'BadNoContinuationPoints';
			case 0x004C0000:
				return 'BadReferenceTypeIdInvalid';
			case 0x004D0000:
				return 'BadBrowseDirectionInvalid';
			case 0x004E0000:
				return 'BadNodeNotInView';
			case 0x01120000:
				return 'BadNumericOverflow';
			case 0x004F0000:
				return 'BadServerUriInvalid';
			case 0x00500000:
				return 'BadServerNameMissing';
			case 0x00510000:
				return 'BadDiscoveryUrlMissing';
			case 0x00520000:
				return 'BadSempahoreFileMissing';
			case 0x00530000:
				return 'BadRequestTypeInvalid';
			case 0x00540000:
				return 'BadSecurityModeRejected';
			case 0x00550000:
				return 'BadSecurityPolicyRejected';
			case 0x00560000:
				return 'BadTooManySessions';
			case 0x00570000:
				return 'BadUserSignatureInvalid';
			case 0x00580000:
				return 'BadApplicationSignatureInvalid';
			case 0x00590000:
				return 'BadNoValidCertificates';
			case 0x00C60000:
				return 'BadIdentityChangeNotSupported';
			case 0x005A0000:
				return 'BadRequestCancelledByRequest';
			case 0x005B0000:
				return 'BadParentNodeIdInvalid';
			case 0x005C0000:
				return 'BadReferenceNotAllowed';
			case 0x005D0000:
				return 'BadNodeIdRejected';
			case 0x005E0000:
				return 'BadNodeIdExists';
			case 0x005F0000:
				return 'BadNodeClassInvalid';
			case 0x00600000:
				return 'BadBrowseNameInvalid';
			case 0x00610000:
				return 'BadBrowseNameDuplicated';
			case 0x00620000:
				return 'BadNodeAttributesInvalid';
			case 0x00630000:
				return 'BadTypeDefinitionInvalid';
			case 0x00640000:
				return 'BadSourceNodeIdInvalid';
			case 0x00650000:
				return 'BadTargetNodeIdInvalid';
			case 0x00660000:
				return 'BadDuplicateReferenceNotAllowed';
			case 0x00670000:
				return 'BadInvalidSelfReference';
			case 0x00680000:
				return 'BadReferenceLocalOnly';
			case 0x00690000:
				return 'BadNoDeleteRights';
			case 0x006A0000:
				return 'BadServerIndexInvalid';
			case 0x006B0000:
				return 'BadViewIdUnknown';
			case 0x00C90000:
				return 'BadViewTimestampInvalid';
			case 0x00CA0000:
				return 'BadViewParameterMismatch';
			case 0x00CB0000:
				return 'BadViewVersionInvalid';
			case 0x00C80000:
				return 'BadNotTypeDefinition';
			case 0x006D0000:
				return 'BadTooManyMatches';
			case 0x006E0000:
				return 'BadQueryTooComplex';
			case 0x006F0000:
				return 'BadNoMatch';
			case 0x00700000:
				return 'BadMaxAgeInvalid';
			case 0x00E60000:
				return 'BadSecurityModeInsufficient';
			case 0x00710000:
				return 'BadHistoryOperationInvalid';
			case 0x00720000:
				return 'BadHistoryOperationUnsupported';
			case 0x00BD0000:
				return 'BadInvalidTimestampArgument';
			case 0x00730000:
				return 'BadWriteNotSupported';
			case 0x00740000:
				return 'BadTypeMismatch';
			case 0x00750000:
				return 'BadInvalidMethod';
			case 0x00760000:
				return 'BadArgumentsMissing';
			case 0x01110000:
				return 'BadNotExecutable';
			case 0x00040000:
				return 'BadTooManySubscriptions';
			case 0x00780000:
				return 'BadTooManyPublishRequests';
			case 0x00790000:
				return 'BadNoSubscription';
			case 0x007A0000:
				return 'BadSequenceNumberUnknown';
			case 0x007B0000:
				return 'BadMessageNotAvailable';
			case 0x007C0000:
				return 'BadInsufficientClientProfile';
			case 0x00BF0000:
				return 'BadStateNotActive';
			case 0x01150000:
				return 'BadAlreadyExists';
			case 0x007D0000:
				return 'BadTcpServerTooBusy';
			case 0x007E0000:
				return 'BadTcpMessageTypeInvalid';
			case 0x007F0000:
				return 'BadTcpSecureChannelUnknown';
			case 0x00800000:
				return 'BadTcpMessageTooLarge';
			case 0x00810000:
				return 'BadTcpNotEnoughResources';
			case 0x00820000:
				return 'BadTcpInternalError';
			case 0x00830000:
				return 'BadTcpEndpointUrlInvalid';
			case 0x00840000:
				return 'BadRequestInterrupted';
			case 0x00850000:
				return 'BadRequestTimeout';
			case 0x00860000:
				return 'BadSecureChannelClosed';
			case 0x00870000:
				return 'BadSecureChannelTokenUnknown';
			case 0x00880000:
				return 'BadSequenceNumberInvalid';
			case 0x00BE0000:
				return 'BadProtocolVersionUnsupported';
			case 0x00890000:
				return 'BadConfigurationError';
			case 0x008A0000:
				return 'BadNotConnected';
			case 0x008B0000:
				return 'BadDeviceFailure';
			case 0x008C0000:
				return 'BadSensorFailure';
			case 0x008D0000:
				return 'BadOutOfService';
			case 0x008E0000:
				return 'BadDeadbandFilterInvalid';
			case 0x00970000:
				return 'BadRefreshInProgress';
			case 0x00980000:
				return 'BadConditionAlreadyDisabled';
			case 0x00CC0000:
				return 'BadConditionAlreadyEnabled';
			case 0x00990000:
				return 'BadConditionDisabled';
			case 0x009A0000:
				return 'BadEventIdUnknown';
			case 0x00BB0000:
				return 'BadEventNotAcknowledgeable';
			case 0x00CD0000:
				return 'BadDialogNotActive';
			case 0x00CE0000:
				return 'BadDialogResponseInvalid';
			case 0x00CF0000:
				return 'BadConditionBranchAlreadyAcked';
			case 0x00D00000:
				return 'BadConditionBranchAlreadyConfirmed';
			case 0x00D10000:
				return 'BadConditionAlreadyShelved';
			case 0x00D20000:
				return 'BadConditionNotShelved';
			case 0x00D30000:
				return 'BadShelvingTimeOutOfRange';
			case 0x009B0000:
				return 'BadNoData';
			case 0x00D70000:
				return 'BadBoundNotFound';
			case 0x00D80000:
				return 'BadBoundNotSupported';
			case 0x009D0000:
				return 'BadDataLost';
			case 0x009E0000:
				return 'BadDataUnavailable';
			case 0x009F0000:
				return 'BadEntryExists';
			case 0x00A00000:
				return 'BadNoEntryExists';
			case 0x00A10000:
				return 'BadTimestampNotSupported';
			case 0x00D40000:
				return 'BadAggregateListMismatch';
			case 0x00D50000:
				return 'BadAggregateNotSupported';
			case 0x00D60000:
				return 'BadAggregateInvalidInputs';
			case 0x00DA0000:
				return 'BadAggregateConfigurationRejected';
			case 0x00E40000:
				return 'BadRequestNotAllowed';
			case 0x01130000:
				return 'BadRequestNotComplete';
			case 0x00E10000:
				return 'BadDominantValueChanged';
			case 0x00E30000:
				return 'BadDependentValueChanged';
			case 0x00AB0000:
				return 'BadInvalidArgument';
			case 0x00AC0000:
				return 'BadConnectionRejected';
			case 0x00AD0000:
				return 'BadDisconnect';
			case 0x00AE0000:
				return 'BadConnectionClosed';
			case 0x00AF0000:
				return 'BadInvalidState';
			case 0x00040000:
				return 'BadEndOfStream';
			case 0x00B10000:
				return 'BadNoDataAvailable';
			case 0x00B20000:
				return 'BadWaitingForResponse';
			case 0x00B30000:
				return 'BadOperationAbandoned';
			case 0x00B40000:
				return 'BadExpectedStreamToBlock';
			case 0x00B50000:
				return 'BadWouldBlock';
			case 0x00B60000:
				return 'BadSyntaxError';
			case 0x00B70000:
				return 'BadMaxConnectionsReached';
			default:
				return 'Bad';
		}
	}
}

FMOperateIndex.UpdateTagOrBar = function (pointValueDictionary, node)
{
	var pointValue = null;
	var pointValueKey = FMOperateIndex.CreatePointValueIdentifierKey(FMOperateIndex.CreatePointValueIdentifierFromNode(node));
	if ( pointValueKey )
	{
		pointValue = pointValueDictionary[pointValueKey];
	}
	else
	{
		return;
	}
	if ( !pointValue || !pointValue.IdentityGuid )
	{
		return;
	}

	if ( node === null ||
		node === undefined ||
		!node.data ||
		pointValue.IdentityGuid !== node.data.TagGUID ||
		pointValue.PointValueType !== node.data.TagPointValueType ||
		pointValue.PropertyID !== node.data.TagPropertyID )
	{
		return;
	}

	var value = NaN;
	var maximum = NaN;
	var minimum = NaN;

	var newFormattedValue = '';
	var diagram = node.diagram;
	var dataModel = node.diagram.model;


	var activeData = node.data;
	var tagFieldSelection = FMTAGFIELDSELECTION.VALUE;
	if ( activeData.TagFieldSelection && activeData.TagFieldSelection !== null )
	{
		tagFieldSelection = activeData.TagFieldSelection;
	}

	if (pointValue.CommunicationsFailure)
	{
		newFormattedValue = $('#CommunicationsFailureText').val();
	}

	else {


		//Determine which attribute of the tag that you want to populate.
		switch (tagFieldSelection) {
			case FMTAGFIELDSELECTION.ALARMSTATUS:
				if (pointValue.Access.View == false && pointValue.Access.Modify == false) {
					newFormattedValue = $('#RestrictedText').val();
				}
				else {
					newFormattedValue = pointValue.AlarmState;
				}
				break;
			case FMTAGFIELDSELECTION.ID:
				if (pointValue.Access.View == false && pointValue.Access.Modify == false) {
					newFormattedValue = $('#RestrictedText').val();
				}
				else {
					newFormattedValue = pointValue.ID;
				}
				break;
			case FMTAGFIELDSELECTION.UNITS:
				if (pointValue.Access.View == false && pointValue.Access.Modify == false) {
					newFormattedValue = $('#RestrictedText').val();
				}
				else {

					if (activeData.TagUnits === FMENGINEERINGUNIT.FM_SiteUnits) {
						if (pointValue.Units === FMENGINEERINGUNIT.FM_NONE) {
							newFormattedValue = 'None';
						}
						else {
							newFormattedValue = FMConvertEngUnits.GetEngineeringUnitAbbreviation(pointValue.Units);
						}
					}
					else {
						if (activeData.TagUnits === FMENGINEERINGUNIT.FM_NONE) {
							newFormattedValue = 'None';
						}
						else {
							newFormattedValue = FMConvertEngUnits.GetEngineeringUnitAbbreviation(activeData.TagUnits);
						}
					}
				}
				break;
			case FMTAGFIELDSELECTION.TIMESTAMP:
				if (pointValue.Access.View == false && pointValue.Access.Modify == false) {
					newFormattedValue = $('#RestrictedText').val();
				}
				else {
					 var timezoneOffsetStr = $("#TimezoneOffsetString").val();
					 var timezoneOffset = parseInt(timezoneOffsetStr);
					 var momentTime = FMFormatValues.ConvertDateTimeOffsetToMoment(pointValue.ServerTimeStamp);
					 momentTime = momentTime.subtract(momentTime.utcOffset(), 'minutes'); //go to UTC time
					 momentTime = momentTime.add(timezoneOffset, 'minutes'); //go to site time
					 newFormattedValue = FMFormatValues.FormatDateTimeString(momentTime, FMOperateIndex.dateTimeFormatInfo);
				}
				break;
			case FMTAGFIELDSELECTION.VALUE:
				if(pointValue.Status & 0x80000000) {
					newFormattedValue = FMOperateIndex.GetStatusCode(pointValue.Status);
				}
				else{
					if (pointValue.ValueTypeString) {
						if (pointValue.ValueTypeString.indexOf('FMBusinessObjects.DataObjects.CodedVariables') !== -1) {
							if (pointValue.Value != null) {
								newFormattedValue = pointValue.Value;
							}
						}
						else {
							if (pointValue.Access.View == false && pointValue.Access.Modify == false) {
								newFormattedValue = $('#RestrictedText').val();
							}
							else {
								switch (pointValue.ValueTypeString) {
									case 'System.DateTime':
										if (pointValue.Value != null) {
											newFormattedValue = FMFormatValues.FormatDateString(FMFormatValues.ConvertDateTimeOffsetToMoment(pointValue.Value), FMOperateIndex.dateTimeFormatInfo);
										}
										break;
									case 'System.DateTimeOffset':
										if (pointValue.Value != null) {

											 //newFormattedValue = FMFormatValues.FormatDateTimeString(FMFormatValues.ConvertDateTimeOffsetToMoment(pointValue.Value), FMOperateIndex.dateTimeFormatInfo);


											 var timezoneOffsetStr = $("#TimezoneOffsetString").val();
											 var timezoneOffset = parseInt(timezoneOffsetStr);
											 var momentTime = FMFormatValues.ConvertDateTimeOffsetToMoment(pointValue.Value);
											 momentTime = momentTime.subtract(momentTime.utcOffset(), 'minutes'); //go to UTC time
											 momentTime = momentTime.add(timezoneOffset, 'minutes'); //go to site time
											 newFormattedValue = FMFormatValues.FormatDateTimeString(momentTime, FMOperateIndex.dateTimeFormatInfo);
											 //newFormattedValue = FMFormatValues.FormatDateTimeString(FMFormatValues.ConvertDateTimeOffsetToMoment(ConvertToSiteTimezone(pointValue.ServerTimeStamp, timezoneOffset)), FMOperateIndex.dateTimeFormatInfo);
										}
										break;
									case 'System.TimeSpan':
										if (pointValue.Value != null) {
											newFormattedValue = FMFormatValues.FormatTimeSpan(pointValue.Value);
										}
										break;
									case 'System.Int16': //Short16
									case 'System.Int32': //Integer32
									case 'System.UInt16': //UShort16
									case 'System.UInt32': //UInteger32
										if (pointValue.Value != null) {
											value = (!isNaN(pointValue.Value)) ? pointValue.Value : parseInt(pointValue.Value);
											maximum = (!isNaN(pointValue.Maximum)) ? pointValue.Maximum : parseInt(pointValue.Maximum);
											minimum = (!isNaN(pointValue.Minimum)) ? pointValue.Minimum : parseInt(pointValue.Minimum);
											newFormattedValue = JSON.stringify(pointValue.Value);
										}
										break;
									case 'System.Double':
									case 'System.Single':
										if (pointValue.Value != null) {
											value = (!isNaN(pointValue.Value)) ? pointValue.Value : parseFloat(pointValue.Value);
											maximum = (!isNaN(pointValue.Maximum)) ? pointValue.Maximum : parseFloat(pointValue.Maximum);
											minimum = (!isNaN(pointValue.Minimum)) ? pointValue.Minimum : parseFloat(pointValue.Minimum);
											if (node.name === 'Tag') {
												var nodeUnits = parseInt(node.data.TagUnits);
												var tagUnits = parseInt(pointValue.Units);
												FMOperateIndex.numformatInfo.NumberDecimalDigits = node.data.TagPrecision;
												if (nodeUnits != null
													&& nodeUnits !== 0
													&& nodeUnits !== tagUnits) {
													value = FMConvertEngUnits.Convert(value, tagUnits, nodeUnits);
													maximum = FMConvertEngUnits.Convert(maximum, tagUnits, nodeUnits);
													minimum = FMConvertEngUnits.Convert(minimum, tagUnits, nodeUnits);
												}
												else {
													nodeUnits = tagUnits;
												}
												newFormattedValue = FMFormatValues.FormatValue(nodeUnits, FMOperateIndex.numformatInfo, value);
											}
										}
										break;
									case 'System.Boolean':
										if (pointValue.Value != null) {
											newFormattedValue = pointValue.Value;
										}
										break;
									case 'System.String':
										if (pointValue.Value != null) {
											newFormattedValue = pointValue.Value;
										}
										break;
									case 'FMBusinessObjects.DataObjects.PointCommandStatusListReference':
										newFormattedValue = (pointValue.Value !== null && pointValue.Value.CurrentKey !== null) ? pointValue.Value.CurrentKey : '';
										break;
									case 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference':
										newFormattedValue = (pointValue.Value !== null && pointValue.Value.CurrentValue !== null) ? pointValue.Value.CurrentValue : '';
										break;
									default:
										newFormattedValue = JSON.stringify(pointValue.Value);
										break;
								}
							}
						}
					}
				}
				break;
		}
	}

	if (node.name === 'Tag')
	{
		diagram.skipsUndoManager = true; //Ensure that change is not recorded in goJS undoManager as users would never undo in operate
		var qualityText = '';
		if (pointValue.Access.View == false && pointValue.Access.Modify == false )
		{
			newFormattedValue = $('#RestrictedText').val();
		}
		else
		{
			qualityText = pointValue.QualityAbbreviation;
		}
		dataModel.setDataProperty(node.data, 'TagStatus', qualityText);
		dataModel.setDataProperty(node.data, 'TagValue', newFormattedValue);
		if (node.data.TagAlarmAnnunciation) {
			if (node.data.TagPointValue === undefined) {
				diagram.startTransaction('TagPointValue');
				dataModel.setDataProperty(node.data, 'TagPointValue', pointValue);
				diagram.commitTransaction('TagPoinValue');
			}
			else {
				dataModel.setDataProperty(node.data, 'TagPointValue', pointValue);
			}

			FMOperateIndex.DoAlarmAnnunciationColoring(node, dataModel);
		}
		diagram.skipsUndoManager = false;
	}
	else if ( node.name === 'Bar' )
	{
		//Ensure that change is not recorded in goJS undoManager as users would never undo in operate
		diagram.skipsUndoManager = true; 

		// Check to see if the “Use Value Limits” field in draw is set to false.
		if (node.data.useTagLimits === "false") {

			// We are using the range set by the user when they created the point detail in draw.
			var maxUserVal = node.data.maxUserVal;
			var minUserVal = node.data.minUserVal;

			dataModel.setDataProperty(node.data, 'maxVal', maxUserVal);
			dataModel.setDataProperty(node.data, 'minVal', minUserVal);

			// If the value of the slider is more than the limit set by the user 
			// when they created the point detail in draw, make the value equal to the max limit.
			if (value > maxUserVal) {
				value = maxUserVal;
				dataModel.setDataProperty(node.data, 'val', value);
			}
			// If the value of the slider is less than the limit set by the user 
			// when they created the point detail in draw, make the value equal to the min limit.
			else if (value < minUserVal) {
				value = minUserVal;
				dataModel.setDataProperty(node.data, 'val', value);
			}
			// Use the value of the slider since it doesn't go below the min or 
			// exceed the max limit set by the user in draw.
			else {
				dataModel.setDataProperty(node.data, 'val', value);
            }			
		}
		// Use the range of the point referenced in this point detail.
		else if (node.data.useTagLimits === "true") {

			dataModel.setDataProperty(node.data, 'maxVal', maximum);
			dataModel.setDataProperty(node.data, 'minVal', minimum);
			dataModel.setDataProperty(node.data, 'val', value);

			// If the value of the slider is more than the limit make the value equal to the max limit.
			if (value > maximum) {
				value = maximum;
				dataModel.setDataProperty(node.data, 'val', value);
			}
			// If the value of the slider is less than the limit make the value equal to the min limit.
			else if (value < minimum) {
				value = minimum;
				dataModel.setDataProperty(node.data, 'val', value);
			}
			// Use the value of the slider since it doesn't go below the min or exceed the max limit.
			else {
				dataModel.setDataProperty(node.data, 'val', value);
			}	
        }

		if (pointValue.Access.View == false && pointValue.Access.Modify == false)
		{
			dataModel.setDataProperty(node.data, 'bgcolor', '#000000'); // black background
			dataModel.setDataProperty(node.data, 'stroke', '#FFFFFF');  // white text
			dataModel.setDataProperty(node.data, 'text', $('#RestrictedText').val());
			return;
		}
		else
		{
			dataModel.setDataProperty(node.data, 'text',"");
		}

		// Load the point value with the point product graphic data.
		FMOperateIndex.LoadPointValueWithGraphicInfo( pointValue );

		if ( node.data.useProductColor && pointValue.HasProductGraphicInfo )
		{
			var fillColorTransStr;

			// Persist the original graphic info for the point value.
			FMOperateIndex.SaveOriginalPointGraphicInfo( pointValue, node );

			if ( pointValue.PatternNumber === 1 )
			{
				fillColorTransStr = node.data.transparency.toString();
				var fillColorTransparency = FMDrawPropertyMenu.ConvertTransparencyToFloat( fillColorTransStr );
				var rgbObj = FMDrawPropertyMenu.HexToRgb( pointValue.ProductColor );
				var rgbaStr = FMDrawPropertyMenu.ConvertToRgbaString( rgbObj, fillColorTransparency );

				dataModel.setDataProperty( node.data, 'color', rgbaStr );
				dataModel.setDataProperty( node.data, 'patternImageName', pointValue.PatternNumber.toString() );
			}
			else
			{
				fillColorTransStr = node.data.transparency.toString();
				FMDrawPatternPalette.newTransparencyValue = fillColorTransStr;

				// Set to use product color
				var dynamicPattern = FMDrawPatternPalette.CreatePatternForOperate( pointValue.PatternNumber, pointValue.ProductColor, pointValue.PatternColor );
				var brush = new go.Brush( go.Brush.Pattern );
				brush.pattern = dynamicPattern;

				dataModel.setDataProperty( node.data, 'color', brush );
				dataModel.setDataProperty( node.data, 'patternImageName', pointValue.PatternNumber.toString() );
			}
		}
		else
		{
			// Reset the point value graphic back to its original setting.  Used when a point value product is unassigned.
			if ( FMOperateIndex.originalPointValueGrahicList != null && FMOperateIndex.originalPointValueGrahicList.length > 0 )
			{
				for ( var nextItem = 0; nextItem < FMOperateIndex.originalPointValueGrahicList.length; nextItem++ )
				{
					var originalPointValue = FMOperateIndex.originalPointValueGrahicList[nextItem];

					if ( originalPointValue.PointValueGuid === pointValue.IdentityGuid && originalPointValue.NodeHashId === node.data.__gohashid )
					{
						dataModel.setDataProperty( node.data, 'color', originalPointValue.Color );

						if ( typeof ( originalPointValue.PatternNumber ) !== "undefined" )
						{
							dataModel.setDataProperty( node.data, 'patternImageName', originalPointValue.PatternNumber.toString() );
						}

						break;
					}
				}
			}
		}
		if ( node.data.useAlarmLevel )
		{
			FMOperateIndex.UpdateAlarmLimitsOnBar( pointValue, node );
		}
		diagram.skipsUndoManager = false;
	}
	else if ( node.name === 'button' )
	{
		diagram.skipsUndoManager = true; //Ensure that change is not recorded in goJS undoManager as users would never undo in operate
		if (node.data.buttonActionType === 'BUTTON_ACTION_COMMAND'
		&& (pointValue.InputOutputType === 0
		|| pointValue.Access.Modify == false
		|| ((pointValue.Access.Override == false
		|| pointValue.InhibitOverride == true)
		&& (pointValue.InputOutputType === 2
		|| pointValue.InputOutputType === 3)))) {
			dataModel.setDataProperty(node.data, 'visible', false);
		}
		else {
			dataModel.setDataProperty(node.data, 'visible', true);
		}
		diagram.skipsUndoManager = false;
	}
};

// Succesful retrieval of the Tab Bar Point Tag Values
FMOperateIndex.UpdateTabBarTagsSuccess = function (results) {
	if (results.length > 0 && results[0]) {
		$(".tab-current-time-text").text('Site Time : ' + results[0].Value);
	}
	if (results.length > 1 && results[1]) {
		$(".tab-current-user-text").text('User : ' + results[1].Value);
	}

	if (results.length > 2 && results[2] && results[2].Value) {
		var licensinfo = results[2].Value.split("|");

	if (licensinfo[2].toLowerCase() == 'false' || licensinfo[0].length == 0 ) {
		$(".tab-current-license-expiration-text").removeClass("enabled");
	} else {
		$(".tab-current-license-expiration-text").text(licensinfo[0]).removeClass("enabled").addClass("enabled");
		$(".tab-current-license-expiration-text").css("color", licensinfo[1]);
		}
	}
};

FMOperateIndex.DoAlarmAnnunciationAndAnimation = function () {

	var nodeArray = FMOperateIndex.activeObjects;

	if (nodeArray
		&& nodeArray instanceof Array
		&& nodeArray.length > 0) {

		FMOperateIndex.displayAlternateIfUnacknowledged = !FMOperateIndex.displayAlternateIfUnacknowledged;

		for (var nodeIndex = 0; nodeIndex < nodeArray.length; nodeIndex++) {

			var node = nodeArray[nodeIndex];

			if (node.name === 'Tag'
				&& node.data.TagAlarmAnnunciation
				&& node.data.TagPointValue !== undefined) {

				var diagram = node.diagram;
				var dataModel = node.diagram.model;
				diagram.skipsUndoManager = true;
				FMOperateIndex.DoAlarmAnnunciationColoring(node, dataModel);
				diagram.skipsUndoManager = false;
			}


			if (node.data.TagAnimationPointValueDictionary !== undefined) {

				var pointValue = null

				if (node.data.TagPointValue !== undefined) {
					pointValue = node.data.TagPointValue;
				}

				var animationPointValueAssignments = node.data.AnimationPointValueAssignments;
				var animationGuid = animationPointValueAssignments.AnimationGuid;
				var testGroupPointValueInfoList = animationPointValueAssignments.TestGroupPointValueInfoList;
				var animationDefinition = FMOperateIndex.GetAnimationByAnimationGuid(animationGuid, node.diagram);

				if (animationDefinition != undefined
					&& animationDefinition !== "") {

					var animationTestGroupDictionary = FMOperateIndex.CreateAnimationTestGroupDictionary(animationDefinition);
					FMOperateIndex.SaveRestoreOriginalAnimationProperties(node);
					for (var testGroupPointValueInfoListIndex = 0; testGroupPointValueInfoListIndex < testGroupPointValueInfoList.length; testGroupPointValueInfoListIndex++) {
						var testGroupPointValueInfo = testGroupPointValueInfoList[testGroupPointValueInfoListIndex];
						var testGroupPointValueInfoPointValueKey = FMOperateIndex.CreatePointValueIdentifierKey(FMOperateIndex.CreatePointValueIdentifierFromTestGroupPointValueInfo(testGroupPointValueInfo));
						if (testGroupPointValueInfoPointValueKey) {
							var pointValueOperate = node.data.TagAnimationPointValueDictionary[testGroupPointValueInfoPointValueKey];
							var animationTestGroupGuid = testGroupPointValueInfo.AnimationTestGroupGuid;
							var animationTestGroup = animationTestGroupDictionary[animationTestGroupGuid];
							if (pointValueOperate && animationTestGroup) {
								FMOperateIndex.EvaluateAnimationTestGroup(node, animationTestGroup, pointValueOperate, pointValue);
							}
						}
					}
				}
			}
		}
	}

	FMOperateIndex.annunciationAnimationTimer = setTimeout(FMOperateIndex.DoAlarmAnnunciationAndAnimation, 1000);
}

// Succesful retrieval of the Point Tag Values
FMOperateIndex.UpdateTagsSuccess = function( results ){
	var nodeArray = FMOperateIndex.activeObjects;
	
	//Defensive Coding to ensure that nodes is an array of greater than zero length
	if (!nodeArray
		|| !(nodeArray instanceof Array)
		||	nodeArray.length === 0 ) {
		return;
	}


	//Defensive Coding to ensure that results is an array of greater than zero length
	if (!results
		|| !(results instanceof Array)
		||	results.length === 0 ) {
		return;
	}

	var pointValueDictionary = FMOperateIndex.CreatePointValueDictionary(results);

	for ( var nodeIndex = 0; nodeIndex < nodeArray.length; nodeIndex++ )
	{
		var node = nodeArray[nodeIndex];

		FMOperateIndex.UpdateTagOrBar(pointValueDictionary, node);
		FMOperateIndex.EvaluateNodeAnimation( node, pointValueDictionary );
	}
};

FMOperateIndex.SaveRestoreOriginalAnimationProperties = function( node )
{
	if ( !node.data.OriginalAnimationProperties )
	{
		var defaultColor = "#FFFFFF";
		var originalAnimationProperties = {
			//text
			text: ( node.data.text ) ? node.data.text : "",
			//background color and transparency
			color: ( node.data.color ) ? node.data.color : defaultColor,
			//visible
			visible: ( node.data.visible ) ? node.data.visible : true,
			//line color and transparency
			lineStroke: ( node.data.lineStroke ) ? node.data.lineStroke : defaultColor,
			//text color
			stroke: ( node.data.stroke ) ? node.data.stroke : defaultColor,
			bgcolor: ( node.data.bgcolor ) ? node.data.bgcolor : defaultColor,
			bgtransparency: ( node.data.bgtransparency ) ? node.data.bgtransparency : 0,

		};
		node.data.OriginalAnimationProperties = originalAnimationProperties;
	}
};

FMOperateIndex.EvaluateNodeAnimation = function( node, pointValueDictionary )
{
	//AnimationEngine
	var pointValue = null;
	var pointValueKey = FMOperateIndex.CreatePointValueIdentifierKey(FMOperateIndex.CreatePointValueIdentifierFromNode(node));
	if (pointValueKey) {
		pointValue = pointValueDictionary[pointValueKey];
	}


	if ( node && node.data && node.data.AnimationPointValueAssignments && node.data.AnimationPointValueAssignments.TestGroupPointValueInfoList && node.diagram )
	{
		var animationPointValueAssignments = node.data.AnimationPointValueAssignments;
		var animationGuid = animationPointValueAssignments.AnimationGuid;
		var testGroupPointValueInfoList = animationPointValueAssignments.TestGroupPointValueInfoList;
		var animationDefinition = FMOperateIndex.GetAnimationByAnimationGuid(animationGuid, node.diagram);

		if (animationDefinition == undefined || animationDefinition === "") {
			return;
		}

		var dataModel = node.diagram.model;
		if (pointValue !== undefined
		&& pointValue !== null) {
			if (node.data.TagPointValue === undefined) {
				node.diagram.startTransaction('TagPointValue');
				dataModel.setDataProperty(node.data, 'TagPointValue', pointValue);
				node.diagram.commitTransaction('TagPointValue');
			}
			else {
				dataModel.setDataProperty(node.data, 'TagPointValue', pointValue);
			}
		}
		else if (node.data.TagPointValue !== undefined) {
			pointValue = node.data.TagPointValue;
		}

		if (node.data.TagAnimationPointValueDictionary === undefined) {
			var animationPointValueDictionary = {};
			node.diagram.startTransaction('TagAnimationPointValueDictionary');
			dataModel.setDataProperty(node.data, 'TagAnimationPointValueDictionary', animationPointValueDictionary);
			node.diagram.commitTransaction('TagAnimationPointValueDictionary');
		}

		var animationTestGroupDictionary = FMOperateIndex.CreateAnimationTestGroupDictionary(animationDefinition);
		FMOperateIndex.SaveRestoreOriginalAnimationProperties( node );
		for ( var testGroupPointValueInfoListIndex = 0; testGroupPointValueInfoListIndex < testGroupPointValueInfoList.length; testGroupPointValueInfoListIndex++ )
		{
			var testGroupPointValueInfo = testGroupPointValueInfoList[testGroupPointValueInfoListIndex];
			var testGroupPointValueInfoPointValueKey = FMOperateIndex.CreatePointValueIdentifierKey( FMOperateIndex.CreatePointValueIdentifierFromTestGroupPointValueInfo( testGroupPointValueInfo ) );
			if ( testGroupPointValueInfoPointValueKey )
			{
				var pointValueOperate = pointValueDictionary[testGroupPointValueInfoPointValueKey];

				var animationTestGroupGuid = testGroupPointValueInfo.AnimationTestGroupGuid;
				var animationTestGroup = animationTestGroupDictionary[animationTestGroupGuid];

				if (pointValueOperate && animationTestGroup) {
					node.data.TagAnimationPointValueDictionary[testGroupPointValueInfoPointValueKey] = pointValueOperate;
					FMOperateIndex.EvaluateAnimationTestGroup(node, animationTestGroup, pointValueOperate, pointValue);
				}
			}
		}
	}
};

FMOperateIndex.GetAnimationTestingValue = function( animationTestGroup, operatePointValue )
{
	if (animationTestGroup.Field === FMPointValueFieldType.ALARMSTATUS)
	{
		return operatePointValue.AlarmState;
	}
	else if (animationTestGroup.DataType.indexOf("FMBusinessObjects.DataObjects.PointCommandStatus") === 0)
	{
		if ( operatePointValue.Value != null )
			return operatePointValue.Value.CurrentKey;
		else
			return "Undefined";
	}
	else if (animationTestGroup.DataType.indexOf("FMBusinessObjects.DataObjects.DeviceAlarmMapReference") === 0) {
		if (operatePointValue.Value != null)
			return operatePointValue.Value.CurrentValue;
	else
		return "Undefined";
	}

	return operatePointValue.Value;
};

FMOperateIndex.GetAnimationTestingValueType = function( animationTestGroup )
{
	if (animationTestGroup.Field === FMPointValueFieldType.ALARMSTATUS)
	{
		return "System.String";
	}
	else if ( animationTestGroup.DataType.indexOf( "FMBusinessObjects.DataObjects.CodedVariables." ) === 0 )
	{
		return "System.String";
	}
	else if (animationTestGroup.DataType.indexOf("FMBusinessObjects.DataObjects.PointCommandStatus") === 0)
	{
		return "System.String";
	}
	else if (animationTestGroup.DataType.indexOf("FMBusinessObjects.DataObjects.DeviceAlarmMapReference") === 0)
	{
		return "System.UInt32";
	}
	return animationTestGroup.DataType;
};

//SRM: Talked with Brent.  This is where the actual evaluation of alarm tests for an alarm test group happens.
FMOperateIndex.EvaluateAnimationTestGroup = function (node, animationTestGroup, operatePointValue, pointValue)
{
	if ( !node || !animationTestGroup || !operatePointValue )
	{
		FMErrorAndExceptionHandling.ShowError("Incomplete Data in EvaluateAnimationTestGroup");
		return;
	}

	if ( animationTestGroup.TestList )
	{
		var testingValue = FMOperateIndex.GetAnimationTestingValue(animationTestGroup, operatePointValue);
		var valueType = FMOperateIndex.GetAnimationTestingValueType(animationTestGroup);

		if (pointValue
		&& node.name == "button"
		&& (pointValue.InputOutputType === 0
		|| !pointValue.Access.Modify
		|| ((pointValue.Access.Override == false
		|| pointValue.InhibitOverride == true)
		&& (pointValue.InputOutputType === 2
		|| pointValue.InputOutputType === 3)))) {
			var dataModel = node.diagram.model;
			dataModel.setDataProperty(node.data, 'visible', false);
		}

		else if (pointValue && pointValue.Access.View == false && pointValue.Access.Modify == false && node.name == "Tag") {
			var dataModel = node.diagram.model;
			dataModel.setDataProperty(node.data, 'visible', false);
		}

		else if (pointValue && pointValue.Access.View == false && pointValue.Access.Modify == false && node.name == "Bar") {
			var dataModel = node.diagram.model;
			dataModel.setDataProperty(node.data, 'bgcolor', '#000000');
			dataModel.setDataProperty(node.data, 'stroke', '#FFFFFF');
			dataModel.setDataProperty(node.data, 'text', $('#RestrictedText').val());
		}
		else {
			if (animationTestGroup.TestList.length > 0) {
				for (var i = (animationTestGroup.TestList.length - 1) ; i >= 0; i--) {
					//This assumes that the TestList is ordered properly
					var animationTest = animationTestGroup.TestList[i];

					if (animationTest) {
						if (FMOperateIndex.EvaluateAnimationTest(animationTest, testingValue, valueType)) {
							FMOperateIndex.AnimationApplyProperties(node, animationTest);

							if (animationTest.TestComparisonOperator !== FMAnimationTestComparisonOperators.Else) {
								return;
							}
						}
					}
				}
			}
		}
	}
};

FMOperateIndex.AnimationApplyProperties = function( node, animationTest )
{
	if ( animationTest.PropertyList )
	{
		for ( var i = 0; i < animationTest.PropertyList.length; i++ )
		{
			var animationProperty = animationTest.PropertyList[i];
			if (animationProperty)
			{
				FMOperateIndex.AnimationApplyVisualState(node, animationProperty);
			}
		}
	}
};

FMOperateIndex.AnimationApplyVisualState = function (node, animationProperty)
{
	if (animationProperty.VisualStates)
	{
		var numberOfVisualStates = animationProperty.VisualStates.length;
		if ( numberOfVisualStates > 0 )
		{
				// first lets make sure we have a valid visual state
			var visualStateIndex = 0;
			if (numberOfVisualStates > 2)
			{
				// make sure that the selection is a color and not transparent
				if (animationProperty.VisualStates[0].Value != null &&
						animationProperty.VisualStates[0].Value != "" &&
						animationProperty.VisualStates[0].Value != "transparent")
						visualStateIndex = 0;
				else if (animationProperty.VisualStates[2].Value != null &&
						animationProperty.VisualStates[2].Value != "" &&
						animationProperty.VisualStates[2].Value != "transparent")
						visualStateIndex = 2;
				else
						return;
				}
			if (numberOfVisualStates > 2 &&
					FMOperateIndex.displayAlternateIfUnacknowledged)
			{
				// make sure that the selection is a color and not transparent
				if (animationProperty.VisualStates[2].Value != null &&
						animationProperty.VisualStates[2].Value != "" &&
						animationProperty.VisualStates[2].Value != "transparent")
				visualStateIndex = 2;
			}
			var visualState = animationProperty.VisualStates[visualStateIndex];
			var visualStateTransperancy = animationProperty.VisualStates[visualStateIndex + 1];
			if (visualState)
			{
				node.diagram.skipsUndoManager = true;
				if (animationProperty.gojsPropertyName == "bgcolor") {
					// we need to set the transparency seperatelt for this option
					node.diagram.model.setDataProperty(node.data, animationProperty.gojsPropertyName, visualState.Value);
					if (visualStateTransperancy.Value != null) {
						node.diagram.model.setDataProperty(node.data, 'bgtransparency', visualStateTransperancy.Value);
					}
					else
					{
						node.diagram.model.setDataProperty(node.data, 'bgtransparency', 0);
					}
				}
				else if (animationProperty.gojsPropertyName == "visible") {
					if(visualState.Value == "ON")
					{
						node.diagram.model.setDataProperty(node.data, 'visible', true);
					}
					else
					{
						node.diagram.model.setDataProperty(node.data, 'visible', false);
					}
				}
				else if (animationProperty.gojsPropertyName == "text")
				{
					// if there are multiple lines they will be seperated by a \\n. We need to change this to a \n for it to work
					var strSplit = visualState.Value.toString().split('\\n');

					var resultingString = "";

					for (var loop = 0; loop < strSplit.length; loop++)
					{
							if (loop > 0)
								resultingString += '\n';
							resultingString += strSplit[loop];
					}

					node.diagram.model.setDataProperty(node.data, animationProperty.gojsPropertyName, resultingString);
				}
				else
				{
					if (node.data.useProductColor && animationProperty.gojsPropertyName == "color")
					{
						// ignore and do not override the color setting
					}
					else
					{
						node.diagram.model.setDataProperty(node.data, animationProperty.gojsPropertyName, visualState.Value);
					}
				}
					
				//node.diagram.model.setDataProperty(node.data, "lineStroke", visualState.Value);
				node.diagram.skipsUndoManager = false;
			}
		}
	}
};

FMOperateIndex.EvaluateAnimationTest = function( animationTest, testingValue, valueType )
{
	if (animationTest.TestComparisonOperator === FMAnimationTestComparisonOperators.Else)
	{
		return true;
	}
	var targetValue = FMOperateIndex.GetAnimationTestTargetValue(animationTest, valueType);
	testingValue = FMOperateIndex.HandleAnimationBitmask(testingValue, animationTest, valueType);
	return FMOperateIndex.HandleAnimationComparison(testingValue, targetValue, animationTest, valueType);
};

FMOperateIndex.HandleAnimationComparison = function (testingValue, targetValue, animationTest, valueType)
{
	var s1 = null;
	var s2 = null;

	if (testingValue == null ||
		targetValue == null)
		return false;

	if (valueType == "System.Boolean" ||
		valueType == "System.String") {
		// we need to make sure that the string
		s1 = testingValue.toString().toLowerCase();
		s2 = targetValue.toString().toLowerCase();
	}
	else
	{
		s1 = testingValue;
		s2 = targetValue;
	}

	switch (animationTest.TestComparisonOperator)
	{
		case FMAnimationTestComparisonOperators.GreaterThan:
			return (s1 > s2);
		case FMAnimationTestComparisonOperators.GreaterThanOrEqual:
			return (s1 >= s2);
		case FMAnimationTestComparisonOperators.LessThan:
			return (s1 < s2);
		case FMAnimationTestComparisonOperators.LessThanOrEqual:
			return (s1 <= s2);
		case FMAnimationTestComparisonOperators.Equals:
			{
					return (s1 == s2);
			}
		case FMAnimationTestComparisonOperators.NotEqual:
			return (s1 != s2);
		case FMAnimationTestComparisonOperators.Else:
			return true;
		case FMAnimationTestComparisonOperators.Contains:
			{
					var location = s1.indexOf(s2);
					if (location > -1)
						return true;
					else
						return false;
			}
		case FMAnimationTestComparisonOperators.BeginsWith:
			{
					var location = s1.indexOf(s2);
					if (location === 0)
						return true;
					else
						return false;
			}
	}
	return false;
};


FMOperateIndex.HandleAnimationBitmask = function (testingValue, animationTest, valueType)
{
	switch ( animationTest.BitmaskOperator )
	{
		case FMAnimationTestBitmaskOperators.And:
			{
				var returnValue = testingValue & animationTest.Bitmask;
				if (valueType === "System.UInt32") {
					var ttemp = returnValue >>> 0;
					return ttemp;
				}
				return returnValue;
			}
		case FMAnimationTestBitmaskOperators.Or:
			{
				var returnValue = testingValue | animationTest.Bitmask;
				if (valueType === "System.UInt32") {
					var ttemp = returnValue >>> 0;
					return ttemp;
				}
				return returnValue;
			}
		case FMAnimationTestBitmaskOperators.Nand:
			{
				var returnValue = FMOperateIndex.calculateNandValue(testingValue, animationTest.Bitmask, valueType);
				return returnValue;
			}
		case FMAnimationTestBitmaskOperators.Nor:
			{
					var returnValue = FMOperateIndex.calculateNorValue(testingValue, animationTest.Bitmask, valueType);
					return returnValue;
			}
		case FMAnimationTestBitmaskOperators.Xand:
			{
				var returnValue = FMOperateIndex.calculateXandValue(testingValue, animationTest.Bitmask, valueType);
				return returnValue;
			}
		case FMAnimationTestBitmaskOperators.Xor:
			{
				var returnValue = testingValue ^ animationTest.Bitmask;
				if (valueType === "System.UInt32") {
					var ttemp = returnValue >>> 0;
					return ttemp;
				}
			}
		case FMAnimationTestBitmaskOperators.None:
			return testingValue;
	}
	return testingValue;
};

FMOperateIndex.ParseStringToInt = function( stringVal )
{
	return parseInt( stringVal );
};

FMOperateIndex.GetAnimationTestTargetValue = function( animationTest, valueType )
{
	if ( valueType === "System.Boolean" )
	{
		if ( animationTest.ComparisonValue.trim().toLowerCase() === "true" )
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	else if ( valueType === "System.Double" || valueType === "System.Single" )
	{
		return parseFloat( animationTest.ComparisonValue );
	}
	else if ( valueType === "System.String" )
	{
		return animationTest.ComparisonValue;
	}
	return FMOperateIndex.ParseStringToInt(animationTest.ComparisonValue);
};

FMOperateIndex.CreateAnimationTestGroupDictionary = function (animation)
{
	var animationTestGroupDictionary = {};
	for (var animationTestGroupIndex = 0; animationTestGroupIndex < animation.AnimationTestGroupList.length; animationTestGroupIndex++)
	{
		var animationTestGroup = animation.AnimationTestGroupList[animationTestGroupIndex];
		if (animationTestGroup )
		{
			animationTestGroupDictionary[animationTestGroup.AnimationTestGroupGuid]= animationTestGroup;
		}	
	}
	return animationTestGroupDictionary;
};

FMOperateIndex.CreatePointValueDictionary = function( operatePointValueListArray )
{
	var pointValueDictionary = {};

	operatePointValueListArray.forEach(function (pointValue) {
			if (pointValue) {
				var pointValueIdentifierKey = FMOperateIndex.CreatePointValueIdentifierKey(pointValue);
				if (pointValueIdentifierKey) {
					pointValueDictionary[pointValueIdentifierKey] = pointValue;
				}
			}
		});

	return pointValueDictionary;
};

//===============================================================================================
// This method will persist the original point value graphic information.  The info will be
// used in cases where the point value product is assigned to none.
//===============================================================================================
FMOperateIndex.SaveOriginalPointGraphicInfo = function(pointValue, node)
{
	var found = false;

	if ( FMOperateIndex.originalPointValueGrahicList == null )
	{
		FMOperateIndex.originalPointValueGrahicList = [];
	}

	for ( var nextItem = 0; nextItem < FMOperateIndex.originalPointValueGrahicList.length; nextItem++ )
	{
		var originalPointValue = FMOperateIndex.originalPointValueGrahicList[nextItem];

		if (originalPointValue.PointValueGuid === pointValue.IdentityGuid && originalPointValue.NodeHashId === node.data.__gohashid)
		{
			found = true;
			break;
		}
	}

	if ( found === false )
	{
		var originalInfo			= new Object();
		originalInfo.PointValueGuid = pointValue.IdentityGuid;
		originalInfo.NodeHashId		= node.data.__gohashid;
		originalInfo.Color			= node.data.color;
		originalInfo.PatternNumber	= node.data.patternImageName;

		FMOperateIndex.originalPointValueGrahicList.push( originalInfo );
	}
}

//======================================================================
// This function will stop Operate Statistics for current window
//======================================================================
FMOperateIndex.StopOperateStatistics = function () {
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };

	$.ajax({
		url: 'StopOperateStatistics',
		type: 'Get',
		dataType: 'json',
		data: { windowName: FMOperateIndex.GetOperateWindowName ? FMOperateIndex.GetOperateWindowName() : window.name },
		success: function (response) {
			// remove previous notifications
			PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

			FMErrorAndExceptionHandling.HandleMessages(response,
				function (data) {
					FMOperateIndex.LoadPointProductGraphicInfo(data);
					FMOperateIndex.UpdateTags();
				}, messageAttributes);
		},
		error: function (request, status, error) {
			// remove previous notifications
			PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

			FMErrorAndExceptionHandling.ShowError('Failed to Stop Operate Statistics.',
				function () {
				}, messageAttributes);
		}
	});
};



//======================================================================
// This function will retrieve point product graphic information
// from the server.
//======================================================================
FMOperateIndex.RetrievePointProductGraphicInfo = function ()
{
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };

	$.ajax({
		url: 'RetrieveProductGraphicInfo',
		success: function (response)
		{
			// remove previous notifications
			PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

			FMErrorAndExceptionHandling.HandleMessages(response,
				function (data)
				{
						FMOperateIndex.LoadPointProductGraphicInfo(data);
						FMOperateIndex.UpdateTags();
					}, messageAttributes);
		},
		error: function (request, status, error)
		{
			// remove previous notifications
			PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

			FMErrorAndExceptionHandling.ShowError('Failed to Retrieve Point Product Graphic Info.',
				function ()
				{
				}, messageAttributes);
		}
	});
};

//=========================================================================
// This function loads the point product graphic information into an
// array of point product graphic data.
//=========================================================================
FMOperateIndex.LoadPointProductGraphicInfo = function( data )
{
	if (data) {
		FMOperateIndex.pointProductGraphicList = [];

		for (var nextItem = 0; nextItem < data.length; nextItem++) {
			var pointProductGraphicInfo = data[nextItem];
			var pointProductGraphic = new Object();

			pointProductGraphic.pointGuid = pointProductGraphicInfo.PointGuid;
			pointProductGraphic.productColor = pointProductGraphicInfo.ProductColor;
			pointProductGraphic.patternColor = pointProductGraphicInfo.PatternColor;
			pointProductGraphic.patternNumber = pointProductGraphicInfo.PatternNumber;
			pointProductGraphic.hasProductGraphicInfo = pointProductGraphicInfo.HasProductGraphicInfo;

			FMOperateIndex.pointProductGraphicList.push(pointProductGraphic);
		}
	}
};

//========================================================================
// This function will load the point value with the associated point/product
// graphic data.
//========================================================================
FMOperateIndex.LoadPointValueWithGraphicInfo = function( pointValue )
{
	if ( FMOperateIndex.pointProductGraphicList == null || FMOperateIndex.pointProductGraphicList.length === 0 )
	{
		pointValue.HasProductGraphicInfo = false;
		return;
	}

	for ( var nextItem = 0; nextItem < FMOperateIndex.pointProductGraphicList.length; nextItem++ )
	{
		var pointProductGraphic = FMOperateIndex.pointProductGraphicList[nextItem];

		if ( pointProductGraphic.pointGuid === pointValue.PointGuid )
		{
			pointValue.ProductColor = pointProductGraphic.productColor;
			pointValue.PatternColor = pointProductGraphic.patternColor;
			pointValue.PatternNumber = pointProductGraphic.patternNumber;
			pointValue.HasProductGraphicInfo = pointProductGraphic.hasProductGraphicInfo;
			return;
		}
	}
};

//=================================================================
// This function will open the context menu based on a given
// canvas.
//=================================================================
FMOperateIndex.InitContextMenu = function (canvas, pointId, pointGuid)
{
	FMOperateIndex.currentContextMenu = canvas.contextPopup({
		items: [
				{
					label: 'Point Calculator',
					icon: '../../FMWebApp/images/Ruler.png',
					action: function ()
					{
						FMOperateIndex.openPointCalculator(pointId, pointGuid);
					},
					isEnabled: function()
					{
						if ( $( '#PointCalculatorRight' ).val() == "False" )
							return false;
						else
							return true;
					}
				}
		]
	}, true);
};

// TODO: The code below is original from the prototype and has to be reviewed
FMOperateIndex.ReviveObject = function()
{
};
FMOperateIndex.SetImageSource = function( image )
{
	// Have to set explicitly or the object will take it's value from new source
	this.width = this.width;
	this.height = this.height;

	var url = FMOperateIndex.baseUrl + 'DisplayImage.ashx?PictureGuid=' + image.pictureGuid;

	image.setSrc( url, function()
	{
		FMOperateIndex.Redraw();
	} );
};
FMOperateIndex.Redraw = function()
{
	FMDrawIndex.RefreshAll();
};

FMOperateIndex.GetAnimationByAnimationGuid = function( animationGuid, diagram )
{
	var animationDictionary = diagram.AnimationDictionary;
	if ( animationDictionary )
	{
		return animationDictionary[animationGuid];
	}
	return null;
};

FMOperateIndex.calculateNorValue = function (testingValue, Bitmask, valueType)
{
	// this routine will calculate the AND value and return based on the following truth chart
	// all calculations need to be unsigned long since we are dealing with bits
	//	a	b	Result
	//	0	0	1
	//	0	1	0
	//	1	0	0
	//	1	1	0
	var returnValue = 0;
	var maxbitUsed = 0;

	var a = [];
	var b = [];

	if (testingValue == null ||
		Bitmask == null)
		return 0;

	// valueType contains the number of bits to set so set the max equal to this type

	if (valueType == "System.Int16" ||
		valueType == "System.UInt16")
	{
		maxbitUsed = 16;
	}
	else
	{
		maxbitUsed = 32;
	}
	for (var i = 0; i < maxbitUsed; i++)
	{
		a[i] = (testingValue >> i) & 1;
	}

	for (var i = 0; i < maxbitUsed; i++)
	{
		b[i] = (Bitmask >> i) & 1;
	}

	for (var i = 0; i < maxbitUsed; i++)
	{
		if(a[i] == 0 &&
		b[i] == 0)
		{
				returnValue += (1 << i);
		}
	}

	if (valueType == "System.Int16")
	{
		// this is kind of messy but js does not have signed,unsigned variable type so we will do it here
		// if we are a signed 16 bit value take the calculated value and subtract 65536 from the value to make it negative
		returnValue = returnValue - 65536;
	}
	else if (valueType == "System.Int32") {
		// this is kind of messy but js does not have signed,unsigned variable type so we will do it here
		// if we are a signed 32 bit value take the calculated value and subtract 4294967296 from the value to make it negative
		returnValue = returnValue - 4294967296;
	}

	return returnValue;
}

FMOperateIndex.calculateNandValue = function (testingValue, Bitmask, valueType) {
	// this routine will calculate the AND value and return based on the following truth chart
	// all calculations need to be unsigned long since we are dealing with bits
	//	a	b	Result
	//	0	0	1
	//	0	1	1
	//	1	0	1
	//	1	1	0

	var returnValue = 0;
	var maxbitUsed = 0;

	var a = [];
	var b = [];

	if (testingValue == null ||
		Bitmask == null)
		return 0;

	if (valueType == "System.Int16" ||
		valueType == "System.UInt16") {
		maxbitUsed = 16;
	}
	else {
		maxbitUsed = 32;
	}

	for (var i = 0; i < maxbitUsed; i++) {
		a[i] = (testingValue >> i) & 1;
	}

	for (var i = 0; i < maxbitUsed; i++) {
		b[i] = (Bitmask >> i) & 1;
	}

	for (var i = 0; i < maxbitUsed; i++) {
		if (a[i] == 1 &&
		b[i] == 1)
		{
		}
		else
		{
				returnValue += (1 << i);
		}
	}

	if (valueType == "System.Int16") {
		// this is kind of messy but js does not have signed,unsigned variable type so we will do it here
		// if we are a signed 16 bit value take the calculated value and subtract 65536 from the value to make it negative
		returnValue = returnValue - 65536;
	}
	else if (valueType == "System.Int32") {
		// this is kind of messy but js does not have signed,unsigned variable type so we will do it here
		// if we are a signed 32 bit value take the calculated value and subtract 4294967296 from the value to make it negative
		returnValue = returnValue - 4294967296;
	}

	return returnValue;
}

FMOperateIndex.calculateXandValue = function (testingValue, Bitmask, valueType) {
	// this routine will calculate the AND value and return based on the following truth chart
	// all calculations need to be unsigned long since we are dealing with bits
	//	a	b	Result
	//	0	0	1
	//	0	1	0
	//	1	0	0
	//	1	1	1
	var returnValue = 0;
	var maxbitUsed = 0;

	var a = [];
	var b = [];
	var i;

	if (testingValue == null ||
		Bitmask == null)
		return 0;

	if (valueType === "System.Int16" ||
		valueType === "System.UInt16") {
		maxbitUsed = 16;
	}
	else {
		maxbitUsed = 32;
	}

	for (i = 0; i < maxbitUsed; i++) {
		a[i] = (testingValue >> i) & 1;
	}

	for (i = 0; i < maxbitUsed; i++) {
		b[i] = (Bitmask >> i) & 1;
	}

	for (i = 0; i < maxbitUsed; i++) {
		if (a[i] === 1 && b[i] === 1) {
				returnValue += (1 << i);
		}
		else if (a[i] === 0 && b[i] === 0) {
				returnValue += (1 << i);
		}
	}

	if (valueType === "System.Int16") {
		// this is kind of messy but js does not have signed,unsigned variable type so we will do it here
		// if we are a signed 16 bit value take the calculated value and subtract 65536 from the value to make it negative
		returnValue = returnValue - 65536;
	}
	else if (valueType === "System.Int32") {
		// this is kind of messy but js does not have signed,unsigned variable type so we will do it here
		// if we are a signed 32 bit value take the calculated value and subtract 4294967296 from the value to make it negative
		returnValue = returnValue - 4294967296;
	}

	return returnValue;
}

//==============================================================================
// This function will set the hamburger menu options for full screen.
// It will call the report full screen settings.
//==============================================================================
FMOperateIndex.SetFullscreenMode = function (windowDocument)
{
	var isMainOperateDisplay = (FMOperateIndex.GetOperateWindowName ? FMOperateIndex.GetOperateWindowName() : window.name) === 'Screen1';
	$("#menuOptionFullScreenLi").removeClass("hidden").addClass("hidden");
	if (isMainOperateDisplay) {  // log out is only available on Screen1
		$("#menuOptionLogoutLi").removeClass("hidden");
	}

	if (FMOperateIndex.IsOperateViewOnlyMode() && isMainOperateDisplay) {
		$("#menuOptionchangePasswordLi").removeClass("hidden");
		$("#menuOptionOperateScreenAssignmentsLi").removeClass("hidden");
	}
	else {
		$("#menuOptionOperateScreenAssignmentsLi").removeClass("hidden").addClass("hidden");
	}

	$("#tabs").removeClass("fullscreen").addClass("fullscreen");

	FMOperateIndex.fullscreenDocument = windowDocument;
}

//==============================================================================
// This function will set the hamburger menu options for non-full screen.
//==============================================================================
FMOperateIndex.ClearFullscreenMode = function ()
{
	// Return if not in full screen mode.
	if (FMOperateIndex.fullscreenDocument != null
		&& (FMOperateIndex.fullscreenDocument.msFullscreenElement || FMOperateIndex.fullscreenDocument.webkitIsFullScreen === true))
	{
		return;
	}

	window.parent.FMMenuBar.SetIFrameSize(true);
	$("#tabs").removeClass("fullscreen");
	var isMainOperateDisplay = (FMOperateIndex.GetOperateWindowName ? FMOperateIndex.GetOperateWindowName() : window.name) === 'Screen1';

	// log out is only available when view operate omly is set and full screen is not
	if (FMOperateIndex.IsOperateViewOnlyMode() && isMainOperateDisplay)
	{
		$("#menuOptionchangePasswordLi").removeClass("hidden");
		$("#menuOptionLogoutLi").removeClass("hidden");
		$("#menuOptionOperateScreenAssignmentsLi").removeClass("hidden");
		$("#menuOptionFullScreenLi").removeClass("hidden").addClass("hidden");
	}
	else
	{
		$("#menuOptionFullScreenLi").removeClass("hidden");
		$("#menuOptionLogoutLi").removeClass("hidden").addClass("hidden");
		$("#menuOptionchangePasswordLi").removeClass("hidden").addClass("hidden");
		$("#menuOptionOperateScreenAssignmentsLi").removeClass("hidden").addClass("hidden");
	}
};

//===================================================================================================================================
// This function opens the Movement Module Settings Editor dialog.
//===================================================================================================================================
FMOperateIndex.OpenMovementModuleSettingsClickPropertyScreen = function (newId, paramIsPointTemplate, paramPointGuid, paramPointPropertyGuid, caller)
{
	var movementModuleSettingsData = { newId: newId, isTemplatePoint: paramIsPointTemplate, pointGuid: paramPointGuid, pointPropertyGuid: paramPointPropertyGuid, caller: caller };

	var openMovementModuleSettings = function ()
	{
		// create the backdrop and wait for next modal to be triggered
		$('body').modalmanager('loading');
		var movementModuleSettingsEditorUrl = $("#urlMovementModuleSettingsEditor").val();

		$.ajax({
			type: "GET",
			cache: false,
			url: movementModuleSettingsEditorUrl,
			data: movementModuleSettingsData,
			success: function (response, xhr, settings)
			{
				if (response && response.ErrorMessage)
				{
					var count = 0;

					if (response.ErrorMessage)
					{
						$.each(response.ErrorMessage, function (key, message) { count = count + 1; });
					}

					if (count === 0)
					{
						$('#PEMPESavePropertyScreen').removeAttr('disabled');
						$('#moduleEditorPropertyScreen').html(response.Data);
//						$('#PMEEditorNamePropertyScreen').text('@FMBaseController.TranslateText("Movement Editor")');
						if ($("#EditorSaveAllowed").val() == "False")
						{
							$('#PEMPESavePropertyScreen').attr('disabled', true);
							$('#PEMPESavePropertyScreen').attr('style', "cursor: not-allowed !important;");
						}
						$('#ModulePropertyEditorPropertyScreen').modal({ keyboard: false });
						$('#ModulePropertyEditorPropertyScreen').modal('show');
					}
					else
					{
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					}
				}
			},
			error: function (xhr, textStatus, error)
			{
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function ()
					{
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					});
			}
		});
	}

	// If no changes then open the modal form directly
	openMovementModuleSettings();
};

//======================================================================================
// This function will open the movement handgauge dialog.
//======================================================================================
FMOperateIndex.OpenMovementHandgaugeClickPropertyScreen = function (paramPointGuid, caller, paramMovementHistoryGuid)
{
	var movementHandgaugeData = { movementPointGuid: paramPointGuid, caller: caller, movementHistoryGuid: paramMovementHistoryGuid};

	var openMovementHandgauge = function ()
	{
		// create the backdrop and wait for next modal to be triggered
		$('body').modalmanager('loading');
		var movementHandgaugeDialogUrl = $("#urlMovementHandgauge").val();

		$.ajax({
			type: "GET",
			cache: false,
			url: movementHandgaugeDialogUrl,
			data: movementHandgaugeData,
			success: function (response, xhr, settings)
			{
				if (response && response.ErrorMessage)
				{
					var count = 0;

					if (response.ErrorMessage)
					{
						$.each(response.ErrorMessage, function (key, message) { count = count + 1; });
					}

					if (count === 0)
					{
						var dialogTitle = $("#MovementHandgaugeTitleText").val();
						$('#MovementHandgaugeEditorSavePropertyScreen').removeAttr('disabled');
						$('#MovementHandgaugeEditorPropertyScreenSection').html(response.Data);
						$('#MovementHandgaugeEditorNamePropertyScreen').text(dialogTitle);
						if ($("#EditorSaveAllowed").val() == "False")
						{
							$('#MovementHandgaugeEditorSavePropertyScreen').attr('disabled', true);
							$('#MovementHandgaugeEditorSavePropertyScreen').attr('style', "cursor: not-allowed !important;");
						}
						$('#MovementHandgaugeEditorPropertyScreen').modal({ keyboard: false });
						$('#MovementHandgaugeEditorPropertyScreen').modal('show');
					}
					else
					{
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					}
				}
			},
			error: function (xhr, textStatus, error)
			{
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function ()
					{
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					});
			}
		});
	}

	// If no changes then open the modal form directly
	openMovementHandgauge();
};

//======================================================================================
// This function will open the movement Edit start/closeout data dialog.
//======================================================================================
FMOperateIndex.OpenMovementHistoryNodeEditorClickPropertyScreen = function (callingType, paramMovementHistoryGuid)
{
	var movementHistoryNodeData = { callingType: callingType, movementHistoryGuid: paramMovementHistoryGuid };

	var openMovementHistoryNodeEditor = function ()
	{
		// create the backdrop and wait for next modal to be triggered
		$('body').modalmanager('loading');
		var movementHistoryNodeEditorDialogUrl = $("#urlMovementHistoryNodeEditor").val();

		$.ajax({
			type: "GET",
			cache: false,
			url: movementHistoryNodeEditorDialogUrl,
			data: movementHistoryNodeData,
			success: function (response, xhr, settings)
			{
				if (response && response.ErrorMessage)
				{
					var count = 0;

					if (response.ErrorMessage)
					{
						$.each(response.ErrorMessage, function (key, message) { count = count + 1; });
					}

					if (count === 0)
					{
						var dialogTitle = $("#MovementHistoryNodeEditorTitle").val();
						$('#MovementHistoryNodeEditorSavePropertyScreen').removeAttr('disabled');
						$('#MovementHistoryNodeEditorPropertyScreenSection').html(response.Data);
						$('#MovementHistoryNodeEditorNamePropertyScreen').text(dialogTitle);

						if ($("#EditorSaveAllowed").val() == "False")
						{
							$('#MovementHistoryNodeEditorSavePropertyScreen').attr('disabled', true);
							$('#MovementHistoryNodeEditorSavePropertyScreen').attr('style', "cursor: not-allowed !important;");
						}
						$('#MovementHistoryNodeEditorPropertyScreen').modal({ keyboard: false });
						$('#MovementHistoryNodeEditorPropertyScreen').modal('show');
					}
					else
					{
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					}
				}
			},
			error: function (xhr, textStatus, error)
			{
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function ()
					{
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					});
			}
		});
	}

	// If no changes then open the modal form directly
	openMovementHistoryNodeEditor();
};

//======================================================================================
// This function will open the movement data dialog.
//======================================================================================
FMOperateIndex.OpenMovementHistoryMovementDataEditorClickPropertyScreen = function (paramMovementHistoryGuid)
{
	var movementHistoryMovementData = { movementHistoryGuid: paramMovementHistoryGuid };

	var openMovementHistoryMovementDataEditor = function ()
	{
		// create the backdrop and wait for next modal to be triggered
		$('body').modalmanager('loading');
		var movementHistoryMovementDataEditorDialogUrl = $("#urlMovementHistoryMovementDataEditor").val();

		$.ajax({
			type: "GET",
			cache: false,
			url: movementHistoryMovementDataEditorDialogUrl,
			data: movementHistoryMovementData,
			success: function (response, xhr, settings)
			{
				if (response && response.ErrorMessage)
				{
					var count = 0;

					if (response.ErrorMessage)
					{
						$.each(response.ErrorMessage, function (key, message) { count = count + 1; });
					}

					if (count === 0)
					{
						var dialogTitle = $("#MovementHistoryMovementDataEditorTitle").val();
						$('#MovementHistoryMovementDataEditorSavePropertyScreen').removeAttr('disabled');
						$('#MovementHistoryMovementDataEditorPropertyScreenSection').html(response.Data);
						$('#MovementHistoryMovementDataEditorNamePropertyScreen').text(dialogTitle);

						if ($("#EditorSaveAllowed").val() == "False")
						{
							$('#MovementHistoryMovementDataEditorSavePropertyScreen').attr('disabled', true);
							$('#MovementHistoryMovementDataEditorSavePropertyScreen').attr('style', "cursor: not-allowed !important;");
						}
						$('#MovementHistoryMovementDataEditorPropertyScreen').modal({ keyboard: false });
						$('#MovementHistoryMovementDataEditorPropertyScreen').modal('show');
					}
					else
					{
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					}
				}
			},
			error: function (xhr, textStatus, error)
			{
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function ()
					{
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					});
			}
		});
	}

	// If no changes then open the modal form directly
	openMovementHistoryMovementDataEditor();
};

//===================================================================================================================================
// This function opens the Movement User Data Editor dialog.
//===================================================================================================================================
FMOperateIndex.OpenMovementUserDataClickPropertyScreen = function (paramPointGuid)
{
	var movementUserDataData = { movementPointGuid: paramPointGuid };

	var openMovementUserData = function ()
	{
		// create the backdrop and wait for next modal to be triggered
		$('body').modalmanager('loading');
		var movementUserDataEditorUrl = $("#urlMovementUserDataEditor").val();

		$.ajax({
			type: "GET",
			cache: false,
			url: movementUserDataEditorUrl,
			data: movementUserDataData,
			success: function (response, xhr, settings)
			{
				if (response && response.ErrorMessage)
				{
					var count = 0;

					if (response.ErrorMessage)
					{
						$.each(response.ErrorMessage, function (key, message) { count = count + 1; });
					}

					if (count === 0)
					{
						$('#MovementUserDataEditorSavePropertyScreen').removeAttr('disabled');
						$('#MovementUserDataEditorPropertyScreenSection').html(response.Data);
//						$('#MovementUserDataEditorNamePropertyScreen').text('@FMBaseController.TranslateText("Movement User Data Editor")');
						if ($("#EditorSaveAllowed").val() == "False")
						{
							$('#MovementUserDataEditorSavePropertyScreen').attr('disabled', true);
							$('#MovementUserDataEditorSavePropertyScreen').attr('style', "cursor: not-allowed !important;");
						}
						$('#MovementUserDataEditorPropertyScreen').modal({ keyboard: false });
						$('#MovementUserDataEditorPropertyScreen').modal('show');
					}
					else
					{
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					}
				}
			},
			error: function (xhr, textStatus, error)
			{
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function ()
					{
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					});
			}
		});
	}

	// If no changes then open the modal form directly
	openMovementUserData();
};


//===================================================================================================================================
// This function opens the Movement Start Data Editor dialog.
//===================================================================================================================================
FMOperateIndex.OpenMovementStartDataClickPropertyScreen = function (paramPointGuid) {
	var movementStartDataData = { movementPointGuid: paramPointGuid };

	var openMovementStartData = function () {
		// create the backdrop and wait for next modal to be triggered
		$('body').modalmanager('loading');
		var movementStartDataEditorUrl = $("#urlMovementStartDataEditor").val();

		$.ajax({
			type: "GET",
			cache: false,
			url: movementStartDataEditorUrl,
			data: movementStartDataData,
			success: function (response, xhr, settings) {
				if (response && response.ErrorMessage) {
					var count = 0;

					if (response.ErrorMessage) {
						$.each(response.ErrorMessage, function (key, message) { count = count + 1; });
					}

					if (count === 0) {

						$('#MovementStartDataEditorSavePropertyScreen').removeAttr('disabled');
						$('#MovementStartDataEditorPropertyScreenSection').html(response.Data);
//						$('#MovementStartDataEditorNamePropertyScreen').text('@FMBaseController.TranslateText("Movement Start Data Editor")');
						if ($("#EditorSaveAllowed").val() == "False") {
							$('#MovementStartDataEditorSavePropertyScreen').attr('disabled', true);
							$('#MovementStartDataEditorSavePropertyScreen').attr('style', "cursor: not-allowed !important;");
						}
						$('#MovementStartDataEditorPropertyScreen').modal({ keyboard: false });
						$('#MovementStartDataEditorPropertyScreen').modal('show');
					}
					else {
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					}
				}
			},
			error: function (xhr, textStatus, error) {
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					});
			}
		});
	}

	// If no changes then open the modal form directly
	openMovementStartData();
};

//===================================================================================================================================
// This function opens the Movement Node Start Data Editor dialog.
//===================================================================================================================================
FMOperateIndex.OpenMovementNodeStartDataClickPropertyScreen = function (paramPointGuid, paramNodePointGuid) {
	var movementNodeStartDataData = {
		movementPointGuid: paramPointGuid,
		movementNodePointGuid: paramNodePointGuid
	};

	var openMovementNodeStartData = function () {
		// create the backdrop and wait for next modal to be triggered
		$('body').modalmanager('loading');
		var movementNodeStartDataEditorUrl = $("#urlMovementNodeStartDataEditor").val();

		$.ajax({
			type: "GET",
			cache: false,
			url: movementNodeStartDataEditorUrl,
			data: movementNodeStartDataData,
			success: function (response, xhr, settings) {
				if (response && response.ErrorMessage) {
					var count = 0;

					if (response.ErrorMessage) {
						$.each(response.ErrorMessage, function (key, message) { count = count + 1; });
					}

					if (count === 0) {

						$('#MovementNodeStartDataEditorSavePropertyScreen').removeAttr('disabled');
						$('#MovementNodeStartDataEditorPropertyScreenSection').html(response.Data);
//						$('#MovementNodeStartDataEditorNamePropertyScreen').text('@FMBaseController.TranslateText("Movement Node Start Data Editor")');
						if ($("#EditorSaveAllowed").val() == "False") {
							$('#MovementNodeStartDataEditorSavePropertyScreen').attr('disabled', true);
							$('#MovementNodeStartDataEditorSavePropertyScreen').attr('style', "cursor: not-allowed !important;");
						}
						$('#MovementNodeStartDataEditorPropertyScreen').modal({ keyboard: false });
						$('#MovementNodeStartDataEditorPropertyScreen').modal('show');
					}
					else {
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					}
				}
			},
			error: function (xhr, textStatus, error) {
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					});
			}
		});
	}

	// If no changes then open the modal form directly
	openMovementNodeStartData();
};

//===================================================================================================================================
// This function opens the Movement Disabled By dialog.
//===================================================================================================================================
FMOperateIndex.OpenMovementDisabledBy = function (paramPointId, paramPointGuid) {
	var movementDisabledByData = { movementPointId: paramPointId, movementPointGuid: paramPointGuid };


	var openMovementDisabledBy = function () {
		// create the backdrop and wait for next modal to be triggered
		$('body').modalmanager('loading');
		var movementDisabledByUrl = $("#urlMovementDisabledBy").val();

		$.ajax({
			type: "GET",
			cache: false,
			url: movementDisabledByUrl,
			data: movementDisabledByData,
			success: function (response, xhr, settings) {
				if (response && response.ErrorMessage) {
					var count = 0;

					if (response.ErrorMessage) {
						$.each(response.ErrorMessage, function (key, message) { count = count + 1; });
					}

					if (count === 0) {

						$('#MovementDisabledByScreen').removeAttr('disabled');
						$('#MovementDisabledByScreenSection').html(response.Data);
//						$('#MovementDisabledByScreenTitle').text('@FMBaseController.TranslateText("Movement Disabled By")');
						$('#MovementDisabledByScreen').modal({ keyboard: false });
						$('#MovementDisabledByScreen').modal('show');
					}
					else {
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					}
				}
			},
			error: function (xhr, textStatus, error) {
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					});
			}
		});
	}

	// If no changes then open the modal form directly
	openMovementDisabledBy();
};


//===================================================================================================================================
// This helper function translates a javascript DateTime from client Time into a moment.js in the local site time 
//===================================================================================================================================
FMOperateIndex.translateClientDateTimeToSiteMomentTime = function (jsDateTime) {
	 var timezoneOffsetStr = $("#TimezoneOffsetString").val();
	 var timezoneOffset = parseInt(timezoneOffsetStr);
	 var momentTime = moment(jsDateTime);
	 momentTime = momentTime.subtract(momentTime.utcOffset(), 'minutes'); //go to UTC time
	 momentTime = momentTime.add(timezoneOffset, 'minutes'); //go to site time
	 return momentTime;
};

FMOperateIndex.getRemainingRefreshTime = function (startTime, refreshTime) {
	var elapsedTime = (Date.now() - startTime);
	var remainingTime = refreshTime - elapsedTime;
	remainingTime = (remainingTime < 0 ? 0 : remainingTime);
	return remainingTime;
}

FMOperateIndex.switchToFullScreen = 

	function () {
		console.log("IN FMOperateIndex.switchToFullScreen");
		// if we are running inside the iframe we want to maximize the iframe, otherwise do the window
		var element;

		if (window.self !== window.top) {
			element = window.parent.document.getElementById("iframeContent"); // Make the iframe go full screen.
		}
		else {
			element = document.documentElement;
		}

		// Supports most browsers and their versions.
		var requestMethod = element.requestFullscreen || element.requestFullScreen || element.webkitRequestFullScreen || element.mozRequestFullScreen || element.msRequestFullscreen;

		if (requestMethod) {
			// Native full screen.
			requestMethod.call(element);

			// in IE the we have to force to resize the screen
			window.parent.FMMenuBar.SetIFrameSize(true);
		}
		else if (typeof window.ActiveXObject !== "undefined") {
			// Older IE.
			var wscript = new ActiveXObject("WScript.Shell");

			if (wscript != null) {
				wscript.SendKeys("{F11}");
			}
		}

		// Set the full screen mode.
		FMOperateIndex.SetFullscreenMode(window.parent.document);
	};


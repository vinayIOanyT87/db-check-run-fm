/// <reference path="jquery-1.7.1.js" />
/// <reference path="dispatch.js" />

// ==============================================================================================================
// All methods in this file have this convention FuelsManagerServiceLib.<METHOD_NAME> which currently calls
// a method in DispatchRequestProxy.  If the call to <METHOD_NAME> succeeds then the 
// FuelsManagerServiceLib.<METHOD_NAME>Succeeded method is called next 
// otherwise the FuelsManagerServiceLib.<METHOD_NAME>Failed is called.
// ==============================================================================================================

// The FuelsManager service scope object.  Variables and functions specific to the FuelsManager
// service interface should be added to this object rather than the global windows object.
var FuelsManagerServiceLib = {};
FuelsManagerServiceLib.enableServiceRequests = true;
FuelsManagerServiceLib.serviceRequestsStopped = false;
FuelsManagerServiceLib.serviceRequestsStopTime = new Date();
FuelsManagerServiceLib.serviceRequestRefreshPeriod = 5;
FuelsManagerServiceLib.serviceRequestAutomaticRestartDelay = 30;
FuelsManagerServiceLib.failedRequestCount = 0;
FuelsManagerServiceLib.topTransactionVersion = 0;
FuelsManagerServiceLib.topEquipmentVersion = 0;
FuelsManagerServiceLib.topPersonnelVersion = 0;
FuelsManagerServiceLib.equipmentDataFM = undefined;
FuelsManagerServiceLib.personnelDataFM = undefined;
FuelsManagerServiceLib.requestsDataFM = undefined;
FuelsManagerServiceLib.standbyPersonnelDataFM = undefined;
FuelsManagerServiceLib.operatorStatusDataFM = undefined;
FuelsManagerServiceLib.equipmentStatusDataFM = undefined;
FuelsManagerServiceLib.operatorStatusCallback = undefined;
FuelsManagerServiceLib.serviceAddress = '';

// ==============================================================================================================
// Function to call EnumerateEquipment operation of DispatchRequestProxy WCF Service
// ==============================================================================================================
FuelsManagerServiceLib.CallDispatchRequestEnumerateEquipment = function (request) {
	if (FuelsManagerServiceLib.enableServiceRequests && !FuelsManagerServiceLib.serviceRequestsStopped) {
		// Required for cross-domain request; Server port number different from client port number is considered cross-domain
		jQuery.support.cors = true;
		$.ajax({
			type: 'POST', //GET or POST or PUT or DELETE verb
			url: FuelsManagerServiceLib.serviceAddress + '/EnumerateEquipment', // Location of the service
			data: JSON.stringify({ securityToken: request.securityToken, topVersion: request.topEquipmentVersion, siteGuid: request.siteGuid }), //Data sent to server
			contentType: 'application/json; charset=utf-8', // Content type sent to server
			dataType: 'json', //Expected data format from server
			success: function (msg) { // On Successfull service call
				FuelsManagerServiceLib.EnumerateEquipmentSucceeded(msg);
			},
			error: FuelsManagerServiceLib.EnumerateEquipmentFailed // On failed service call
		});
	}
};

FuelsManagerServiceLib.EnumerateEquipmentFailed = function (result) {
	FuelsManagerServiceLib.equipmentDataFM = undefined;
	var errorMessage = 'Enumerate Equipment service call failed: ' + result.status + ' ' + result.statusText;
	if (result.responseText && result.responseText != '') {
		errorMessage += '; Response Text: ' + result.responseText;
	}
	if (console) {
		console.log(errorMessage);
	}
	if (++FuelsManagerServiceLib.failedRequestCount > 3) {
		if (console) {
			console.log('More than 3 consecutive failed requests occurred. Stopping service requests.');
		}
		FuelsManagerServiceLib.serviceRequestsStopped = true;
		FuelsManagerServiceLib.serviceRequestsStopTime = new Date();
		FuelsManagerServiceLib.failedRequestCount = 0;
	};
};

FuelsManagerServiceLib.EnumerateEquipmentSucceeded = function (result) {
	if (result.EnumerateEquipmentResult.Refreshed) {
		if (console) {
			console.log('Updated equipment data found.  Count = ' + result.EnumerateEquipmentResult.Equipment.length);
		}

		FuelsManagerServiceLib.topEquipmentVersion = result.EnumerateEquipmentResult.TopVersion;
		FuelsManagerServiceLib.equipmentDataFM = result.EnumerateEquipmentResult.Equipment;
		window.DispatchingViewLib.updateEquipmentGrid();
	}
	else {
		if (console) {
			console.log('Equipment Engine: No equipment changes detected.');
		}
	}
	FuelsManagerServiceLib.failedRequestCount = 0;
};

// ==============================================================================================================
// Function to call EnumeratePersonnel operation of DispatchRequestProxy WCF Service
// ==============================================================================================================
FuelsManagerServiceLib.CallDispatchRequestEnumeratePersonnel = function (request) {
	if (FuelsManagerServiceLib.enableServiceRequests && !FuelsManagerServiceLib.serviceRequestsStopped) {
		// Required for cross-domain request; Server port number different from client port number is considered cross-domain
		jQuery.support.cors = true;
		$.ajax({
			type: 'POST', //GET or POST or PUT or DELETE verb
			url: FuelsManagerServiceLib.serviceAddress + '/EnumeratePersonnel', // Location of the service
			data: JSON.stringify({ securityToken: request.securityToken, topVersion: request.topPersonnelVersion, siteGuid: request.siteGuid }), //Data sent to server
			contentType: 'application/json; charset=utf-8', // Content type sent to server
			dataType: 'json', //Expected data format from server
			success: function (msg) { // On Successfull service call
				FuelsManagerServiceLib.EnumeratePersonnelSucceeded(msg);
			},
			error: FuelsManagerServiceLib.EnumeratePersonnelFailed // On failed service call
		});
	}
};

FuelsManagerServiceLib.EnumeratePersonnelFailed = function (result) {
	FuelsManagerServiceLib.personnelDataFM = undefined;
	var errorMessage = 'Enumerate Personnel service call failed: ' + result.status + ' ' + result.statusText;
	if (result.responseText && result.responseText != '') {
		errorMessage += '; Response Text: ' + result.responseText;
	}
	if (console) {
		console.log(errorMessage);
	}
	if (++FuelsManagerServiceLib.failedRequestCount > 3) {
		if (console) {
			console.log('More than 3 consecutive failed requests occurred. Stopping service requests.');
		}
		FuelsManagerServiceLib.serviceRequestsStopped = true;
		FuelsManagerServiceLib.serviceRequestsStopTime = new Date();
		FuelsManagerServiceLib.failedRequestCount = 0;
	};
};

FuelsManagerServiceLib.EnumeratePersonnelSucceeded = function (result) {
	if (result.EnumeratePersonnelResult.Refreshed) {
		if (console) {
			console.log('Updated personnel data found.  Count = ' + result.EnumeratePersonnelResult.Personnel.length);
		}

		FuelsManagerServiceLib.topPersonnelVersion = result.EnumeratePersonnelResult.TopVersion;
		FuelsManagerServiceLib.personnelDataFM = result.EnumeratePersonnelResult.Personnel;
		window.DispatchingViewLib.updatePersonnelGrid();

	}
	else {
		if (console) {
			console.log('Personnel Engine: No personnel changes detected.');
		}
	}
	FuelsManagerServiceLib.failedRequestCount = 0;
};

// ==============================================================================================================
// Function to call EnumerateStandbyPersonnel operation of DispatchRequestProxy WCF Service
// ==============================================================================================================
FuelsManagerServiceLib.CallDispatchRequestEnumerateStandbyPersonnel = function (request) {
	// Required for cross-domain request; Server port number different from client port number is considered cross-domain
	jQuery.support.cors = true;
	$.ajax({
		type: 'POST', //GET or POST or PUT or DELETE verb
		url: FuelsManagerServiceLib.serviceAddress + '/EnumerateStandbyPersonnel', // Location of the service
		data: JSON.stringify({ securityToken: request.securityToken, siteGuid: request.siteGuid }), //Data sent to server
		contentType: 'application/json; charset=utf-8', // Content type sent to server
		dataType: 'json', //Expected data format from server
		success: function (msg) { // On Successfull service call
			FuelsManagerServiceLib.EnumerateStandbyPersonnelSucceeded(msg);
		},
		error: FuelsManagerServiceLib.EnumerateStandbyPersonnelFailed // On failed service call
	});
};

FuelsManagerServiceLib.EnumerateStandbyPersonnelFailed = function (result) {
	FuelsManagerServiceLib.standbyPersonnelDataFM = undefined;
	var errorMessage = 'Enumerate Standby Personnel service call failed: ' + result.status + ' ' + result.statusText;
	if (result.responseText && result.responseText != '') {
		errorMessage += '; Response Text: ' + result.responseText;
	}
	alert(errorMessage);
};

FuelsManagerServiceLib.EnumerateStandbyPersonnelSucceeded = function (result) {
	if (console) {
		console.log('Enumerate Standby Personnel service call succeeded.  Count = ' + result.EnumerateStandbyPersonnelResult.length);
	}
	FuelsManagerServiceLib.standbyPersonnelDataFM = result.EnumerateStandbyPersonnelResult;
	window.TabularViewLib.DisplayStandbyStatusBoard();
};

// ==============================================================================================================
// Function to call EnumerateStandbyPersonnel operation of DispatchRequestProxy WCF Service
// ==============================================================================================================
FuelsManagerServiceLib.CallDispatchRequestEnumerateOperatorStatus = function (request) {
    // Required for cross-domain request; Server port number different from client port number is considered cross-domain
    jQuery.support.cors = true;

    var dataPacket = {
        securityToken: request.securityToken
    };

    $.ajax({
        type: 'POST', //GET or POST or PUT or DELETE verb
        url: 'TabularView.aspx/EnumerateOperatorStatus', // Location of the service
        data: JSON.stringify(dataPacket), //Data sent to server
        contentType: 'application/json; charset=utf-8', // Content type sent to server
        dataType: 'json', //Expected data format from server
        success: function (msg) { // On Successfull service call
            FuelsManagerServiceLib.EnumerateOperatorStatusSucceeded(msg);
        },
        error: FuelsManagerServiceLib.EnumerateOperatorStatusFailed // On failed service call
    });
};

FuelsManagerServiceLib.EnumerateOperatorStatusFailed = function (result) {
    FuelsManagerServiceLib.operatorStatusDataFM = undefined;
    var errorMessage = 'Enumerate Operator Status service call failed: ' + result.status + ' ' + result.statusText;
    if (result.responseText && result.responseText != '') {
        errorMessage += '; Response Text: ' + result.responseText;
    }
    alert(errorMessage);
};

FuelsManagerServiceLib.EnumerateOperatorStatusSucceeded = function (result) {
    if (console) {
        //console.log('Enumerate Operator Status service call succeeded.  Count = ' + result.EnumerateStandbyPersonnelResult.length);
    }
    FuelsManagerServiceLib.operatorStatusDataFM = result.d.StatusList;
    FuelsManagerServiceLib.equipmentStatusDataFM = result.d.EquipmentList;
    FuelsManagerServiceLib.operatorStatusCallback();
};

// ==============================================================================================================
// Function to call EnumerateTransactions operation of DispatchRequestProxy WCF Service
// ==============================================================================================================
FuelsManagerServiceLib.CallDispatchRequestEnumerateTransactions = function (request) {
	if (FuelsManagerServiceLib.enableServiceRequests && !FuelsManagerServiceLib.serviceRequestsStopped) {
		// Required for cross-domain request; Server port number different from client port number is considered cross-domain
		jQuery.support.cors = true;
		$.ajax({
			type: 'POST', //GET or POST or PUT or DELETE verb
			url: FuelsManagerServiceLib.serviceAddress + '/EnumerateTransactions', // Location of the service
			data: JSON.stringify({
					securityToken: request.securityToken,
					topVersion: request.topTransactionVersion,
					beginDate: request.beginDate,
					endDate: request.endDate,
					status: request.status,
					requestName: request.alias,
					siteGuid: request.siteGuid
				}),
			contentType: 'application/json; charset=utf-8', // Content type sent to server
			dataType: 'json', //Expected data format from server
			success: function (msg) { // On Successfull service call
				FuelsManagerServiceLib.EnumerateEnumerateTransactionsSucceeded(msg);
			},
			error: FuelsManagerServiceLib.EnumerateEnumerateTransactionsFailed // On failed service call
		});
	}
};

FuelsManagerServiceLib.EnumerateEnumerateTransactionsFailed = function (result) {
	FuelsManagerServiceLib.requestsDataFM = undefined;
	var errorMessage = 'Enumerate Transactions call failed: ' + result.status + ' ' + result.statusText;

	/* TMI
	if (result.responseText && result.responseText != '')
	{
		errorMessage += '; Response Text: ' + result.responseText;
	}
    */

	if (console) {
		console.log(errorMessage);
	}
	if (++FuelsManagerServiceLib.failedRequestCount > 3) {
		if (console) {
			console.log('More than 3 consecutive failed requests occurred. Stopping service requests.');
		}

		FuelsManagerServiceLib.serviceRequestsStopped = true;
		FuelsManagerServiceLib.serviceRequestsStopTime = new Date();
		FuelsManagerServiceLib.failedRequestCount = 0;
	};
};

FuelsManagerServiceLib.EnumerateEnumerateTransactionsSucceeded = function (result) {
	if (result.EnumerateTransactionsResult.Refreshed) {
		FuelsManagerServiceLib.requestsDataFM = result.EnumerateTransactionsResult.Transactions;
		FuelsManagerServiceLib.topTransactionVersion = result.EnumerateTransactionsResult.TopVersion;


		if (console) {
			console.log('(' + new Date().toLocaleTimeString() + ') - TX Engine - New tx data: Count = ' + window.FuelsManagerServiceLib.requestsDataFM.length);
		}

		if (window.TabularViewLib) {
			window.TabularViewLib.updateGrid();
		}
		else {
			window.DispatchingViewLib.updateRequestGrid();
		}
	}
	else {
		if (console) {
			console.log('(' + new Date().toLocaleTimeString() + ') - TX Engine: No tx changes detected .');
		}
	}

	FuelsManagerServiceLib.failedRequestCount = 0;
};

// ==============================================================================================================
// Function to call SetArrived operation of DispatchRequestProxy WCF Service
// ==============================================================================================================
FuelsManagerServiceLib.CallDispatchRequestSetArrived = function (request) {
	// Required for cross-domain request; Server port number different from client port number is considered cross-domain
	jQuery.support.cors = true;
	$.ajax({
		type: 'POST', //GET or POST or PUT or DELETE verb
		url: FuelsManagerServiceLib.serviceAddress + '/SetArrived', // Location of the service
		data: JSON.stringify({
				securityToken: request.securityToken,
				transactionIds: request.transactionIds,
				lineItemGuids: request.lineItemGuids,
				siteGuid: request.siteGuid
			}), //Data sent to server
		contentType: 'application/json; charset=utf-8', // Content type sent to server
		dataType: 'json', //Expected data format from server
		success: function (msg) { // On Successfull service call
			FuelsManagerServiceLib.SetArrivedSucceeded(msg);
		},
		error: FuelsManagerServiceLib.SetArrivedFailed // On failed service call
	});
};

FuelsManagerServiceLib.SetArrivedFailed = function (result) {
	var errorMessage = 'SetArrived Transactions call failed: ' + result.status + ' ' + result.statusText;
	if (result.responseText && result.responseText != '') {
		errorMessage += '; Response Text: ' + result.responseText;
	}
	alert(errorMessage);
};

FuelsManagerServiceLib.SetArrivedSucceeded = function (result) {
	var numArrived = result.SetArrivedResult;
	if (console) {
		console.log('Transaction Engine: ' + numArrived + ' transactions SetArrived.');
	}
	if (numArrived > 0) {
		if (window.TabularViewLib) {
			window.TabularViewLib.refreshData();
		}
	}
};

// ==============================================================================================================
// Function to call SetServiceStarted operation of DispatchRequestProxy WCF Service
// ==============================================================================================================
FuelsManagerServiceLib.CallDispatchRequestSetServiceStarted = function (request) {
	// Required for cross-domain request; Server port number different from client port number is considered cross-domain
	jQuery.support.cors = true;
	$.ajax({
		type: 'POST', //GET or POST or PUT or DELETE verb
		url: FuelsManagerServiceLib.serviceAddress + '/SetServiceStarted', // Location of the service
		data: JSON.stringify({
				securityToken: request.securityToken,
				transactionIds: request.transactionIds,
				lineItemGuids: request.lineItemGuids,
				siteGuid: request.siteGuid
			}), //Data sent to server
		contentType: 'application/json; charset=utf-8', // Content type sent to server
		dataType: 'json', //Expected data format from server
		success: function (msg) { // On Successfull service call
			FuelsManagerServiceLib.SetServiceStartedSucceeded(msg);
		},
		error: FuelsManagerServiceLib.SetServiceStartedFailed // On failed service call
	});
};

FuelsManagerServiceLib.SetServiceStartedFailed = function (result) {
	var errorMessage = 'SetServiceStarted Transactions call failed: ' + result.status + ' ' + result.statusText;
	if (result.responseText && result.responseText != '') {
		errorMessage += '; Response Text: ' + result.responseText;
	}
	alert(errorMessage);
};

FuelsManagerServiceLib.SetServiceStartedSucceeded = function (result) {
	var numStarted = result.SetServiceStartedResult;
	if (console) {
		console.log('Transaction Engine: ' + numStarted + ' transactions SetServiceStarted.');
	}
	if (numStarted > 0) {
		if (window.TabularViewLib) {
			window.TabularViewLib.refreshData();
		}
	}
};

// ==============================================================================================================
// Function to call SetServiceStopped operation of DispatchRequestProxy WCF Service
// ==============================================================================================================
FuelsManagerServiceLib.CallDispatchRequestSetServiceStopped = function (request) {
	// Required for cross-domain request; Server port number different from client port number is considered cross-domain
	jQuery.support.cors = true;
	$.ajax({
		type: 'POST', //GET or POST or PUT or DELETE verb
		url: FuelsManagerServiceLib.serviceAddress + '/SetServiceStopped', // Location of the service
		data: JSON.stringify({
				securityToken: request.securityToken,
				transactionIds: request.transactionIds,
				lineItemGuids: request.lineItemGuids,
				siteGuid: request.siteGuid
			}), //Data sent to server
		contentType: 'application/json; charset=utf-8', // Content type sent to server
		dataType: 'json', //Expected data format from server
		success: function (msg) { // On Successfull service call
			FuelsManagerServiceLib.SetServiceStoppedSucceeded(msg);
		},
		error: FuelsManagerServiceLib.SetServiceStoppedFailed // On failed service call
	});
};

FuelsManagerServiceLib.SetServiceStoppedFailed = function (result) {
	var errorMessage = 'SetServiceStopped Transactions call failed: ' + result.status + ' ' + result.statusText;
	if (result.responseText && result.responseText != '') {
		errorMessage += '; Response Text: ' + result.responseText;
	}
	alert(errorMessage);
};

FuelsManagerServiceLib.SetServiceStoppedSucceeded = function (result) {
	var numStopped = result.SetServiceStoppedResult;
	if (console) {
		console.log('Transaction Engine: ' + numStopped + ' transactions SetServiceStopped.');
	}
	if (numStopped > 0) {
		if (window.TabularViewLib) {
			window.TabularViewLib.refreshData();
		}
	}
};

// ==============================================================================================================
// Function to call RetrieveOptionalTimes operation of DispatchRequestProxy WCF Service
// ==============================================================================================================
FuelsManagerServiceLib.CallDispatchRequestRetrieveOptionalTimes = function (request)
{
	// Required for cross-domain request; Server port number different from client port number is considered cross-domain
	jQuery.support.cors = true;
	$.ajax({
		type: 'POST', //GET or POST or PUT or DELETE verb
		url: FuelsManagerServiceLib.serviceAddress + '/RetrieveOptionalTimes', // Location of the service
		data: JSON.stringify({
			securityToken: request.securityToken,
			siteGuid: request.siteGuid
		}), //Data sent to server
		contentType: 'application/json; charset=utf-8', // Content type sent to server
		dataType: 'json', //Expected data format from server
		success: function (msg)
		{ // On Successfull service call
			FuelsManagerServiceLib.RetrieveOptionalTimesSucceeded(msg);
		},
		error: FuelsManagerServiceLib.RetrieveOptionalTimesFailed // On failed service call
	});
};

FuelsManagerServiceLib.RetrieveOptionalTimesFailed = function (result)
{
	var errorMessage = 'RetrieveOptionalTimes call failed: ' + result.status + ' ' + result.statusText;
	if (result.responseText && result.responseText != '')
	{
		errorMessage += '; Response Text: ' + result.responseText;
	}
	alert(errorMessage);
};

FuelsManagerServiceLib.RetrieveOptionalTimesSucceeded = function (result)
{
	var optionalTimesStr = result.RetrieveOptionalTimesResult;

	if (console)
	{
		console.log('Transaction Engine: ' + optionalTimesStr + ' retrieve optional times.');
	}

	if (optionalTimesStr != null && optionalTimesStr.length > 0)
	{
		var parts = optionalTimesStr.split("|");

		var arriveValue = parts[0].split(":");
		var startValue = parts[1].split(":");
		var stopValue = parts[2].split(":");

		window.TabularViewLib.jsonOptionalTimesArrivalFlagValue = arriveValue[1];
		window.TabularViewLib.jsonOptionalTimesStartFlagValue = startValue[1];
		window.TabularViewLib.jsonOptionalTimesStopFlagValue = stopValue[1];

		if (window.TabularViewLib)
		{
			//window.TabularViewLib.refreshData();
			window.TabularViewLib.SetPopupMenuEnableDisable();
		}
	}
};

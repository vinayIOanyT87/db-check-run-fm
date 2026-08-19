importScripts('bluebird.min.js');

var subscriptions = [];
var masterTagList = [];
var url = "";
var refreshTimeout = 5000;
var requestVerificationToken = "";
var statistics = new Array();
var siteTimeZone = "";
var pointValues = {};
var downloadFlag = false;
var tagListId = "";

/*---------------------- POLYFILLS ---------------------------*/
// Production steps of ECMA-262, Edition 5, 15.4.4.18
// Reference: http://es5.github.io/#x15.4.4.18
if (!Array.prototype.forEach) {

	Array.prototype.forEach = function (callback, thisArg) {

		var T, k;

		if (this === null) {
			throw new TypeError(' this is null or not defined');
		}

		// 1. Let O be the result of calling toObject() passing the
		// |this| value as the argument.
		var O = Object(this);

		// 2. Let lenValue be the result of calling the Get() internal
		// method of O with the argument "length".
		// 3. Let len be toUint32(lenValue).
		var len = O.length >>> 0;

		// 4. If isCallable(callback) is false, throw a TypeError exception. 
		// See: http://es5.github.com/#x9.11
		if (typeof callback !== "function") {
			throw new TypeError(callback + ' is not a function');
		}

		// 5. If thisArg was supplied, let T be thisArg; else let
		// T be undefined.
		if (arguments.length > 1) {
			T = thisArg;
		}

		// 6. Let k be 0
		k = 0;

		// 7. Repeat, while k < len
		while (k < len) {

			var kValue;

			// a. Let Pk be ToString(k).
			//    This is implicit for LHS operands of the in operator
			// b. Let kPresent be the result of calling the HasProperty
			//    internal method of O with argument Pk.
			//    This step can be combined with c
			// c. If kPresent is true, then
			if (k in O) {

				// i. Let kValue be the result of calling the Get internal
				// method of O with argument Pk.
				kValue = O[k];

				// ii. Call the Call internal method of callback with T as
				// the this value and argument list containing kValue, k, and O.
				callback.call(T, kValue, k, O);
			}
			// d. Increase k by 1.
			k++;
		}
		// 8. return undefined
	};
}


if (!Array.prototype.find) {
	Object.defineProperty(Array.prototype, 'find', {
		value: function (predicate) {
			'use strict';
			if (this == null) {
				throw new TypeError('Array.prototype.find called on null or undefined');
			}
			if (typeof predicate !== 'function') {
				throw new TypeError('predicate must be a function');
			}
			var list = Object(this);
			var length = list.length >>> 0;
			var thisArg = arguments[1];
			var value;

			for (var i = 0; i < length; i++) {
				value = list[i];
				if (predicate.call(thisArg, value, i, list)) {
					return value;
				}
			}
			return undefined;
		}
	});
}

if (!Array.prototype.findIndex) {
	Object.defineProperty(Array.prototype, 'findIndex', {
		value: function (predicate) {
			'use strict';
			if (this == null) {
				throw new TypeError('Array.prototype.findIndex called on null or undefined');
			}
			if (typeof predicate !== 'function') {
				throw new TypeError('predicate must be a function');
			}
			var list = Object(this);
			var length = list.length >>> 0;
			var thisArg = arguments[1];
			var value;

			for (var i = 0; i < length; i++) {
				value = list[i];
				if (predicate.call(thisArg, value, i, list)) {
					return i;
				}
			}
			return -1;
		},
		enumerable: false,
		configurable: false,
		writable: false
	});
}

if (!String.prototype.startsWith) {
	String.prototype.startsWith = function (searchString, position) {
		position = position || 0;
		return this.substr(position, searchString.length) === searchString;
	};
}

/*---------------------- END OF POLYFILLS ---------------------------*/



// message handler
onmessage = function (e) {
	if (e.data && e.data.type && e.data.type == "getStatistics") {
		postMessage({ messageType: "tagUpdateValuesStatisticsResults", statistics: getStatistics() });
	} else
	if (e.data.type === 'configuration')
	{
		url = e.data.url;
		refreshTimeout = e.data.refreshTimeout;
		requestVerificationToken = e.data.requestVerificationToken;
		siteTimeZone = e.data.siteTimeZone;

	} else if (e.data.type === 'subscribe') {

		// create a new subscription or update an existing one if already defined
		var foundSubscription = false;
		subscriptions.forEach( function( elem )
		{
			if (elem.id === e.data.id)
			{
				foundSubscription = true;
				elem.tagList = e.data.tagList;
				elem.refreshValues = true;
				elem.refreshAllTags = e.data.refreshAllTags;
			}
		});

		if (!foundSubscription) {
			subscriptions.push({ id: e.data.id, tagList: e.data.tagList, refreshValues: true, refreshAllTags: e.data.refreshAllTags });
		}

		refreshTagList();

	} else if (e.data.type === 'unsubscribe')
	{
		// remove subscription from array
		var subscIdx = subscriptions.findIndex(function (elem) { return elem.id === e.data.id });
		if (subscIdx > -1) {
			subscriptions.splice(subscIdx, 1);
		}
		refreshTagList();
	}
	else if (e.data.name === 'download') {
		downloadFlag = true;
		 tagListId = e.data.subId;
		 subscriptions.forEach(function (elem) {
			  if (elem.id === e.data.subId) {
					elem.refreshValues = true;
					elem.refreshAllTags = true;
					elem.tagList.forEach((value, index) => value.UtcTicks = -1);
			  }
		 });

	}

}

function refreshTagList()
{
	
	// redo the taglist subscription
	masterTagList = [];
	// for each subscription add the tags to the MasterTagList that we don't have yet
	subscriptions.forEach( function (elem)
	{
		elem.tagList.forEach(function (elem) {
			// if the tag is not in the master list add it
			var tagIndex = masterTagList.findIndex(function(masterElem) { return masterElem.IdentityGuid === elem.IdentityGuid && masterElem.PointValueType === elem.PointValueType && masterElem.PropertyID === elem.PropertyID });
			if (tagIndex === -1) {
				masterTagList.push(elem);
			} else {
				// Reset UtcTicks for subscription items that are '0'
				if (!elem.hasOwnProperty('UtcTicks') || elem.UtcTicks === '0') {
					masterTagList[tagIndex].UtcTicks = '0';
				}

				// if its in the master list then update the Include alarm limits (set it to true if needed)
				var includeAlarmLimits = false;
				includeAlarmLimits = elem.hasOwnProperty('IncludeAlarmLimits') && elem.IncludeAlarmLimits;
				if ( includeAlarmLimits && ( !masterTagList[ tagIndex ].hasOwnProperty('IncludeAlarmLimits') || masterTagList[ tagIndex ].IncludeAlarmLimits == false )) {
					masterTagList[tagIndex].IncludeAlarmLimits = true;
				}
			}
	} );
	});
};


function ajaxPostCall(pointList) {
	var xmlhttp = new XMLHttpRequest();

	var params = JSON.stringify({ pointValueIdentifiers: pointList, siteTimeZone: siteTimeZone });
	xmlhttp.open("post", url + "?t=" + Math.random(), true);
	//Send the proper header information along with the request
	xmlhttp.setRequestHeader("Content-type", "application/json; charset=utf-8");
	xmlhttp.setRequestHeader("Cache-Control", "no-cache");
	xmlhttp.setRequestHeader("__RequestVerificationToken", requestVerificationToken);
	xmlhttp.setRequestHeader("X-Requested-With", "XMLHttpRequest");

	var promise = new Promise(function (resolve, reject) {

		xmlhttp.onreadystatechange = function () {
			if (xmlhttp.readyState === 4) {
				if (xmlhttp.status === 200) {
					if (xmlhttp.responseText.startsWith('<script type="text/javascript">')) {
						if (xmlhttp.responseText.indexOf('alert(') > 0) {
							reject( [xmlhttp.responseText.replace( '<script type="text/javascript">', '' )
									.replace( '</script>', '' )
									.replace( '<!--', '' )
									.replace( '-->', '' )
									.replace( 'alert("', '' )
									.replace( '");', '' )
									.replace( /(\r\n|\n|\r)/gm, "" )]
								); //also eliminate the line breaks
						}
						else 
							// what's returned its not an object, its code
							reject( [xmlhttp.statusText] );
						return;
					}
					else if (!xmlhttp.responseText.startsWith('{')) {
						// response is not JSON
						reject( [xmlhttp.statusText] );
						return;
					}

					var response = JSON.parse(xmlhttp.responseText);

					// count the number of error messages
					if (response && response.ErrorMessage && JSON.stringify(response.ErrorMessage) !== JSON.stringify({})) {
						var messageList = [];
						for (var errorProperty in response.ErrorMessage) {
							if (response.ErrorMessage.hasOwnProperty(errorProperty)) {
								if (typeof response.ErrorMessage[errorProperty] === 'string')
								{
									messageList.push(response.ErrorMessage[errorProperty]);
								}
								else
								{
									response.ErrorMessage[errorProperty].forEach(function (val, i) {
										messageList.push(val);
									});
								}
							}
						}
						reject(messageList);
						return;
					}

					var results = response.Data;

					resolve( results );
				}
				else {
					reject([xmlhttp.statusText]);
					return;
				}

				xmlhttp = null;
			}
		}

		xmlhttp.onerror = function () {
			reject(  [xmlhttp.statusText] );
			xmlhttp = null;
		}
	});
	xmlhttp.send(params);
	return promise;

}

function main()
{
	var mainStartTime = Date.now();

	if ( masterTagList.length === 0 || url === "")
	{
		setTimeout(function () { main(); }, getTagRefreshTimeout(mainStartTime, refreshTimeout));
		return;
	};


	if ( masterTagList.length > 0 )
	{
		// split into limited length arrays
		var pointValueIdentifiersSet = [];
		pointValueIdentifiersSet.push( new Array() );

		for ( var index = 0; index < masterTagList.length; index++ )
		{
			if ( pointValueIdentifiersSet[pointValueIdentifiersSet.length - 1].length === 1000 )
			{
				pointValueIdentifiersSet.push( new Array() );
			}

			pointValueIdentifiersSet[pointValueIdentifiersSet.length - 1].push( masterTagList[index] );
		}


		var deferredUpdateTagsAjaxRequest = [];
		
		 pointValueIdentifiersSet.forEach(function (pointValueIdentifiers)
		{
			deferredUpdateTagsAjaxRequest.push( ajaxPostCall(pointValueIdentifiers));
		});

		Promise.all(deferredUpdateTagsAjaxRequest).then(function (resultsArray, x, y, z)
		{
			if ( resultsArray && resultsArray.length > 0 )
			{

				// populate the subscriptions to send back to the caller
				var subscriptionsResult = [];

				resultsArray.forEach( function( results )
				{

					if ( results != null )
					{
						var changes = [];

						results.forEach( function( resultElem )
						{
							var dictionaryKey = resultElem.PointValueIdentifier_IdentityGuid + '_' + resultElem.PointValueIdentifier_PointValueType + '_' + ( resultElem.PointValueIdentifier_PropertyID ? resultElem.PointValueIdentifier_PropertyID : '' );
							if ( pointValues.hasOwnProperty( dictionaryKey ) )
							{
								if ( JSON.stringify( resultElem ) !== JSON.stringify( pointValues[dictionaryKey] ) )
								{
									changes.push( resultElem );
								}
								pointValues[dictionaryKey] = resultElem;
							}
							else
							{
								pointValues[dictionaryKey] = resultElem;
								changes.push( resultElem );
							}

						} );

						subscriptions.forEach( function( subsElem )
						{
							var subResult = { id: subsElem.id, tagList: [] };

							// for each tag in the subscription tag list check if we got it in the result that we got from the server
							subsElem.tagList.forEach( function( pointValueIdentifier )
							{
								var resultsIdx = -1;
								if ( subsElem.refreshValues || subsElem.refreshAllTags )
								{
									resultsIdx = results.findIndex( function( resultElem )
									{
										return resultElem.PointValueIdentifier_IdentityGuid === pointValueIdentifier.IdentityGuid && resultElem.PointValueIdentifier_PointValueType === pointValueIdentifier.PointValueType && resultElem.PointValueIdentifier_PropertyID === pointValueIdentifier.PropertyID;
									} );

									if ( resultsIdx > -1 )
									{
										pointValueIdentifier.UtcTicks = results[resultsIdx].PointValueIdentifier_UtcTicks;
										subResult.tagList.push( results[resultsIdx] );
									 }
								}
								else
								{
									resultsIdx = changes.findIndex( function( resultElem )
									{
										return resultElem.PointValueIdentifier_IdentityGuid === pointValueIdentifier.IdentityGuid && resultElem.PointValueIdentifier_PointValueType === pointValueIdentifier.PointValueType && resultElem.PointValueIdentifier_PropertyID === pointValueIdentifier.PropertyID;
									} );

									if ( resultsIdx > -1 )
									{
										pointValueIdentifier.UtcTicks = results[resultsIdx].PointValueIdentifier_UtcTicks;
										subResult.tagList.push( changes[resultsIdx] );
									} else {
										// get also the point values that are in alarm status so the graphics get refreshed
										resultsIdx = results.findIndex(function (resultElem) {
											return resultElem.PointValueIdentifier_IdentityGuid === pointValueIdentifier.IdentityGuid && resultElem.PointValueIdentifier_PointValueType === pointValueIdentifier.PointValueType && resultElem.PointValueIdentifier_PropertyID === pointValueIdentifier.PropertyID && resultElem.AlarmPriorityGuid !== "00000000-0000-0000-0000-000000000000";
										});

										if (resultsIdx > -1) {
											pointValueIdentifier.UtcTicks = results[resultsIdx].PointValueIdentifier_UtcTicks;
											subResult.tagList.push(results[resultsIdx]);
										}
									}
								}
							});

							if (subResult.tagList.length > 0 || subsElem.refreshAllTags)
							{
								subscriptionsResult.push( subResult );
							}

							// if initial load of the subscription wait until we read all tags to remove the flag to refresh the values
							if (subsElem.refreshValues) {
								if (subResult.tagList.length === subsElem.tagList.length) {
									subsElem.refreshValues = false;
								}
							}
						} );
					}
				});
				 if (subscriptionsResult.length > 0) {

					  if (downloadFlag) {
							var filteredJson = [];
							for (index in subscriptionsResult)
								 if (subscriptionsResult[index].id === tagListId)
									  filteredJson = filteredJson.concat(structuredClone(subscriptionsResult[index].tagList));
							if (filteredJson.length > 0) {
								 for (tag in filteredJson)
									  delete filteredJson[tag].Access;

								 //if (filteredJson.length != tagList.length) // 
								 //break;
								 filteredJson.sort(function (a, b) {
									  if (a.PointID < b.PointID)
											return -1;
									  else if (a.PointID > b.PointID)
											return 1;
									  else if (a.ID < b.ID)
											return -1;
									  else if (a.ID > b.ID)
											return 1;
									  else
											return 0;
								 })

								 const replacer = (key, value) => value === null ? '' : value // specify how you want to handle null values here
								 const header = Object.keys(filteredJson[0])
								 const csv = [
									  header.join(','), // header row first
									  ...filteredJson.map(row => header.map(fieldName => JSON.stringify(row[fieldName], replacer)).join(','))
								 ].join('\r\n')
								 const blob = new Blob([csv], { type: "octet/stream" });
								 const url = URL.createObjectURL(blob);
								 self.postMessage({ name: "download-link", link: url });
								 downloadFlag = false;
							}
					  }

					postMessage(subscriptionsResult);
				}
			}
			setTimeout(function () { main(); }, getTagRefreshTimeout(mainStartTime, refreshTimeout));
		}).catch(function (resultsArray) {

			// populate the subscriptions to send back to the caller
			var subscriptionsResult = [];

			subscriptions.forEach( function( subsElem )
			{
				var subResult = { id: subsElem.id, tagList: [] };

				// for each tag in the subscription tag list check if we got it in the result that we got from the server
				subsElem.tagList.forEach( function( pointValueIdentifier )
				{
					var resultElem = {
						Access: { View: true, Modify: false, Override: false },
						CommunicationsFailure: true,
						Value: null,
						QualityAbbreviation: '',
						HasProductGraphicInfo: false,
						InputOutputType: 0,
						InhibitOverride: true,
						IdentityGuid: pointValueIdentifier.IdentityGuid,
						PointValueType: pointValueIdentifier.PointValueType,
						PropertyID: pointValueIdentifier.PropertyID,
						PointValueIdentifier_IdentityGuid: pointValueIdentifier.IdentityGuid,
						PointValueIdentifier_PointValueType: pointValueIdentifier.PointValueType,
						PointValueIdentifier_PropertyID: pointValueIdentifier.PropertyID
					};

					if (subsElem.refreshValues || subsElem.refreshAllTags) {
						subResult.tagList.push(resultElem);
					}
					else {
						var dictionaryKey = resultElem.PointValueIdentifier_IdentityGuid + '_' + resultElem.PointValueIdentifier_PointValueType + '_' + (resultElem.PointValueIdentifier_PropertyID ? resultElem.PointValueIdentifier_PropertyID : '');
						if (pointValues.hasOwnProperty(dictionaryKey)) {
							if ( JSON.stringify( resultElem ) !== JSON.stringify( pointValues[dictionaryKey] ) )
							{
								subResult.tagList.push(resultElem);
							}
							pointValues[dictionaryKey] = resultElem;
						}
						else
						{
							pointValues[dictionaryKey] = resultElem;
							subResult.tagList.push(resultElem);
						}
					}
				} );

				if (subResult.tagList.length > 0)
				{
					subscriptionsResult.push(subResult);
				}
				// if initial load of the subscription wait until we read all tags to remove the flag to refresh the values
				if (subsElem.refreshValues) {
					if (subResult.tagList.length === subsElem.tagList.length) {
						subsElem.refreshValues = false;
					}
				}
			});

			if (subscriptionsResult.length > 0) {
				postMessage(subscriptionsResult);
			}

			setTimeout(function () { main(); }, getTagRefreshTimeout(mainStartTime, refreshTimeout));
		});
	}

};

function getTagRefreshTimeout(startTime, refreshTimeout) {
	var elapsedTime = (Date.now() - startTime);
	var efficientRefreshTimeout = refreshTimeout - elapsedTime;
	efficientRefreshTimeout = (efficientRefreshTimeout < 0 ? 0 : efficientRefreshTimeout);
	//console.log("TAG " + efficientRefreshTimeout);
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

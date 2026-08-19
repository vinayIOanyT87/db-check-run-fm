/// <reference path="lib/jquery-1.7.1.js" />

// The dispatch scope object.  Variables and functions specific to the dispatch web client
// should be added to this object rather than the global windows object.
var DispatchLib = {};
DispatchLib.displayCurrentTime = false;
DispatchLib.displayMilitaryJulianDate = false;
DispatchLib.resetTabularViewSessionOperation = '';

DispatchLib.currentUserGuid = '';
DispatchLib.tabularGridSettingKeySuffix = '_TabularGridSettings';
DispatchLib.requestGridSettingKeySuffix = '_RequestGridSettings';
DispatchLib.equipmentGridSettingKeySuffix = '_EquipmentGridSettings';
DispatchLib.personnelGridSettingKeySuffix = '_PersonnelGridSettings';
DispatchLib.useLocalStorage = true;

DispatchLib.applicationLoad = function () {
	DispatchLib.registerPrototypes();

	DispatchLib.resetTabularViewSessionStorage();

	// Start the clock
	window.setInterval(DispatchLib.updateTime, 200);
};

DispatchLib.clearGridUserSettings = function () {
	window.localStorage.removeItem(DispatchLib.currentUserGuid + DispatchLib.tabularGridSettingKeySuffix);
	window.localStorage.removeItem(DispatchLib.currentUserGuid + DispatchLib.requestGridSettingKeySuffix);
	window.localStorage.removeItem(DispatchLib.currentUserGuid + DispatchLib.equipmentGridSettingKeySuffix);
	window.localStorage.removeItem(DispatchLib.currentUserGuid + DispatchLib.personnelGridSettingKeySuffix);
};

DispatchLib.resetTabularViewSessionStorage = function () {
	if (window.sessionStorage.tabularViewInitialized != 'true' || DispatchLib.resetTabularViewSessionOperation == 'UserLogin') {
		window.sessionStorage.beginDateFilter = new Date().toLocaleDateString();
		window.sessionStorage.endDateFilter = new Date().toLocaleDateString();
		window.sessionStorage.statusFilter = '';
		window.sessionStorage.requestTypeFilter = '';
		window.sessionStorage.tabularGridSelectedRows = JSON.stringify([]);
		window.sessionStorage.tabularViewInitialized = 'true';
	} else if (DispatchLib.resetTabularViewSessionOperation == 'SiteChange') {
		window.sessionStorage.tabularGridSelectedRows = JSON.stringify([]);
	}
	DispatchLib.resetTabularViewSessionOperation = '';
};

DispatchLib.setGridColumnDefaults = function (gridSettings, columns, rowFormatter, linkFormatter) {
	for (var i = 0; i < gridSettings.columnOrder.length; ++i) {
		var colId = gridSettings.columnOrder[i];
		columns[i] = gridSettings.columnDef[colId];
		columns[i].headerCssClass = 'slick-column-center';

		var isRowNumCol = colId == 'RowNum';
		columns[i].resizable = !isRowNumCol;
		columns[i].sortable = !isRowNumCol;
		columns[i].selectable = !isRowNumCol;

		if (isRowNumCol) {
			columns[i].minWidth = 30;
			columns[i].cssClass = 'slick-column-rownum';
		} else {
			columns[i].minWidth = 60;
			if (colId == 'ControllerLog') {
				columns[i].formatter = linkFormatter;
			} else {
				columns[i].formatter = rowFormatter;
				columns[i].cssClass = 'slick-column-center';
			}
		}
	}
};

DispatchLib.getDefaultGridSettings = function (jsonGridColumnDefinitions) {
	var defaultGridSettings = {
		sortAscending: true,
		sortColumn: 'RowNum',
		columnDef: { 'RowNum': { id: 'RowNum', name: '#', field: 'RowNum', width: 50, cannotTriggerInsert: true } },
		columnOrder: ['RowNum']
	};
	try {
		var columnDefs = JSON.parse(jsonGridColumnDefinitions);
		var numCols = columnDefs.length;
		for (var i = 0; i < numCols; ++i) {
			var colId = columnDefs[i].Id;
			defaultGridSettings.columnDef[colId] = {};
			defaultGridSettings.columnDef[colId].id = columnDefs[i].Id;
			defaultGridSettings.columnDef[colId].name = columnDefs[i].DisplayName;
			defaultGridSettings.columnDef[colId].field = columnDefs[i].DataField;
			defaultGridSettings.columnDef[colId].width = columnDefs[i].Width;
			defaultGridSettings.columnOrder[i + 1] = colId;
		}
	} catch (err) {
		if (console) {
			console.log('Error loading grid column definitions');
		}
	}
	return defaultGridSettings;
};

DispatchLib.updateTime = function () {
	var currentTime = new Date();

	var tabularViewPanel = $('#tabularViewPanel')[0];

	// If the tabularViewPanel exists, we are looking at the Tabular View
	if (tabularViewPanel) {
		window.TabularViewLib.updateTime(currentTime);
		window.TabularViewLib.refreshData(currentTime);
	}
	else {
		var dispatchViewPanel = $('#dispatchingViewPanel')[0];

		// If the dispatchViewPanel exists, we are looking at the Dispatch View
		if (dispatchViewPanel) {
			window.DispatchingViewLib.updateTime(currentTime);
			window.DispatchingViewLib.refreshDispatchViewData(currentTime);
		}
	}
};

DispatchLib.registerPrototypes = function () {
	// This prototype is provided by the Mozilla foundation and
	// is distributed under the MIT license.
	// http://www.ibiblio.org/pub/Linux/LICENSES/mit.license
	// The prototype allows any browser to support the function if it
	// does not already
	if (!Array.prototype.forEach) {
		Array.prototype.forEach = function (fun /*, thisp*/) {
			var len = this.length;
			if (typeof fun != "function")
				throw new TypeError();

			var thisp = arguments[1];
			for (var i = 0; i < len; i++) {
				if (i in this)
					fun.call(thisp, this[i], i, this);
			}
		};
	}

	if (!Array.prototype.add) {
		Array.prototype.add = function (item) {
			this[this.length] = item;
		};
	}

	if (!String.prototype.format) {
		String.prototype.format = function () {
			var args = arguments;
			return this.replace(/\{(\d+)\}/g, function (m, n) { return args[n]; });
		};
	}

	/*
	if (CanvasRenderingContext2D) 
	{
	CanvasRenderingContext2D.prototype.dashedLineTo = function(fromX, fromY, toX, toY, pattern) {
	// Our growth rate for our line can be one of the following:
	//   (+,+), (+,-), (-,+), (-,-)
	// Because of this, our algorithm needs to understand if the x-coord and
	// y-coord should be getting smaller or larger and properly cap the values
	// based on (x,y).
	var lt = function(a, b) { return a <= b; };
	var gt = function(a, b) { return a >= b; };
	var capmin = function(a, b) { return Math.min(a, b); };
	var capmax = function(a, b) { return Math.max(a, b); };

	var checkX = { thereYet: gt, cap: capmin };
	var checkY = { thereYet: gt, cap: capmin };

	if (fromY - toY > 0) {
	checkY.thereYet = lt;
	checkY.cap = capmax;
	}
	if (fromX - toX > 0) {
	checkX.thereYet = lt;
	checkX.cap = capmax;
	}

	this.moveTo(fromX, fromY);
	var offsetX = fromX;
	var offsetY = fromY;
	var idx = 0, dash = true;
	while (!(checkX.thereYet(offsetX, toX) && checkY.thereYet(offsetY, toY))) {
	var ang = Math.atan2(toY - fromY, toX - fromX);
	var len = pattern[idx];

	offsetX = checkX.cap(toX, offsetX + (Math.cos(ang) * len));
	offsetY = checkY.cap(toY, offsetY + (Math.sin(ang) * len));

	if (dash) this.lineTo(offsetX, offsetY);
	else this.moveTo(offsetX, offsetY);

	idx = (idx + 1) % pattern.length;
	dash = !dash;
	}
	};
	}
	*/

	window.requestAnimFrame = (function () {
		return window.requestAnimationFrame ||
			window.webkitRequestAnimationFrame ||
				window.mozRequestAnimationFrame ||
					window.oRequestAnimationFrame ||
						window.msRequestAnimationFrame ||
							function (callback) {
								window.setTimeout(callback, 1000 / 60);
							};
	})();

};

// Algorithm for computing Julian Date from http://en.wikipedia.org/wiki/Julian_day
DispatchLib.computeJulianDate = function (year, month, day) {
	var a = Math.floor((14 - month) / 12);
	var y = year + 4800 - a;
	var m = month + 12 * a - 3;
	var julianDayNumber = day + Math.floor((153 * m + 2) / 5) + 365 * y + Math.floor(y / 4)
							- Math.floor(y / 100) + Math.floor(y / 400) - 32045;
	return julianDayNumber;
};

DispatchLib.militaryJulianDate = function (currentDate) {
	var julianDateCurrent = DispatchLib.computeJulianDate(currentDate.getFullYear(), currentDate.getMonth() + 1, currentDate.getDate());
	var julianDateJan01 = DispatchLib.computeJulianDate(currentDate.getFullYear(), 1, 1);
	var dayOfYear = julianDateCurrent - julianDateJan01 + 1;
	return currentDate.getFullYear() * 1000 + dayOfYear;
};

DispatchLib.getQueryStringParams = function () {
	var queryStringMap = [], param;
	var queryString = location.search.split('?')[1];
	if (queryString != undefined) {
		var q = queryString.split('&');
		for (var i = 0; i < q.length; i++) {
			param = q[i].split('=');
			queryStringMap[param[0]] = param[1];
		}
	}
	return queryStringMap;
};
var FMGeoTrackingMapPopupFormatter = FMGeoTrackingMapPopupFormatter ||
{
	width: 250,
	height: 70,
	imageRootPath: "",
	yellow: "#E6E600",
	red: "#FA4654",
	orange: "#FF8C00",
	green: "#0BA50E",
	white: "#FFFFFF",
	darkGray: "#A0A0A0",
	mediumGray: "#686868",
	lightGray: "#E8E8E8",
	blue: "#1364B5",
	black: "#000000",
	radioActiveSymbolCode: 0x2622
};

//===============================================================================
// This function is the main entry point for formatting the contents that will
// be displayed in the Map Icon Popup.
//===============================================================================
FMGeoTrackingMapPopupFormatter.FormatPopupContent = function (popupContentData)
{
	var contentParts = popupContentData.split("|");
	var formattedContent = "";
	
	if (contentParts[0] === "TYPE_EQUIP")
	{
		formattedContent = FMGeoTrackingMapPopupFormatter.FormatEquipmentType(popupContentData);
		FMGeoTrackingMapPopupFormatter.height = 180;
	}

	if (contentParts[0] === "TYPE_DELIVERYLOCATION")
	{
		formattedContent = FMGeoTrackingMapPopupFormatter.FormatDeliveryLocationType(popupContentData);
	}

	if (contentParts[0] === "TYPE_FACILITY")
	{
		formattedContent = FMGeoTrackingMapPopupFormatter.FormatFacilityType(popupContentData);
	}

	if (contentParts[0] === "TYPE_TANK")
	{
		formattedContent = FMGeoTrackingMapPopupFormatter.FormatTankType(popupContentData);
	}

	return formattedContent;
}

//===============================================================================
// This function will format the equipment type information.
//===============================================================================
FMGeoTrackingMapPopupFormatter.FormatEquipmentType = function (popupContentData)
{
	var contentParts		= popupContentData.split("|");
	var formattedContent	= "<div>";
	var tankCounter			= 1;
	var nextItem			= 0;
	var continueLoop		= true;

	var headerValue				= "";
	var productValue			= "";
	var timestampValue			= "";
	var tankTypeList			= [];
	var tankVolumeList			= [];
	var tankDensityList			= [];
	var tankItemColorList		= [];
	var tankItemContaminatedList = [];

	var model = FMGeoTrackingMap.GetMapViewModel();

	while (continueLoop)
	{
		if (contentParts[nextItem] === "HEADER")
		{
			headerValue = contentParts[++nextItem];

			formattedContent = formattedContent
				+ "<div style='width: 250px;'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.mediumGray + "; font-size: 10px;'>" + model.EquipmentLabelDictionary + "</label>"
				+ "&nbsp;&nbsp;&nbsp;&nbsp;"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.mediumGray + "; font-size: 10px;'>" + headerValue + "</label>"
				+ "</div>";

			FMGeoTrackingMapPopupFormatter.width = 170;

			if (headerValue.length > 15)
			{
				// Pixel range:  max - min = pixel difference;      250 - 170 = 80
				// Char range:  max - min = Char difference;        30 - 15 = 15
				// scale = pixel difference / char difference;      80/15 = 5.33333333333
				FMGeoTrackingMapPopupFormatter.width = 170 + Math.floor((headerValue.length - 15) * 5.33333);
			}
		}

		if (contentParts[nextItem] === "PRODUCT")
		{
			productValue = contentParts[++nextItem];
		}

		var tankType = "TANK" + tankCounter;

		if (contentParts[nextItem] === tankType)
		{
			var tankConfigNumber = contentParts[++nextItem];
			var volumeValue  = contentParts[++nextItem];
			var densityValue = contentParts[++nextItem];
			var itemColor = contentParts[++nextItem];
			var contaminated = contentParts[++nextItem];

			tankTypeList.push(tankType);
			tankVolumeList.push(volumeValue);
			tankDensityList.push(densityValue);
			tankItemColorList.push(itemColor);
			tankItemContaminatedList.push(contaminated);

			tankCounter++;
		}
		
		if (contentParts[nextItem] === "TIMESTAMP")
		{
			timestampValue = contentParts[++nextItem];
		}

		nextItem++;
		if (nextItem >= formattedContent.length)
		{
			continueLoop = false;
		}
	}

	var tankTableWidth = 27;

	if (tankTypeList.length > 0)
	{
		var nextTankIndex;
		var buttonStyle = "padding: 4px; "
						+ "border-top-width: 1px; "
						+ "border-left-width: 1px; "
						+ "border-right-width: 1px; "
						+ "border-top-style: solid; "
						+ "border-left-style: solid; "
						+ "border-right-style: solid; "
						+ "border-top-color: black; "
						+ "border-left-color: black; "
						+ "border-right-color: black; "
						+ "font-size: 10px; ";

		for (nextTankIndex = 0; nextTankIndex < tankTypeList.length; nextTankIndex++)
		{
			var tankValueIndex	= nextTankIndex + 1;
			var tankButtonId	= "TankButton" + tankValueIndex;

			if (nextTankIndex === 0)
			{
				formattedContent = formattedContent
					+ "<ul class='nav nav-tabs' style='width: 161px'>"
					+ "<li><button id='" + tankButtonId
					+ "' style='background-color:" + FMGeoTrackingMapPopupFormatter.blue + "; " + buttonStyle + " color: " + FMGeoTrackingMapPopupFormatter.white + "; ' onclick='FMGeoTrackingMapPopupFormatter.TankTabOnClick(" + tankValueIndex
					+ "); return false;'>Compartment</button></li>";
			}
			else
			{
				formattedContent = formattedContent
					+ "<li><button id='" + tankButtonId + "' style='" + buttonStyle + "' "
					+ " onclick='FMGeoTrackingMapPopupFormatter.TankTabOnClick(" + tankValueIndex + "); return false;'>" + tankValueIndex + "</button></li>";
			}
		}

		formattedContent = formattedContent + "</ul>";
		formattedContent = formattedContent + "<div class='tab-content' style='width: 600px'>";

		if (FMGeoTrackingMapPopupFormatter.width > 170)
		{
			// Pixel range:  max - min = pixel difference;      250 - 170 = 80
			// Percent range:  max - min = percent difference;   40 - 27 = 13
			// scale = percent difference / pixel difference;    13/80 = 0.1625
			var pixel = FMGeoTrackingMapPopupFormatter.width - 170;
			tankTableWidth = Math.floor(pixel * 0.1625) + 27;
		}

		for (nextTankIndex = 0; nextTankIndex < tankTypeList.length; nextTankIndex++)
		{
			if (nextTankIndex === 0)
			{
				formattedContent = formattedContent
					+ "<div id='" + tankTypeList[nextTankIndex] + "' style='display: block'>";
			}
			else
			{
				formattedContent = formattedContent
					+ "<div id='" + tankTypeList[nextTankIndex] + "' style='display: none'>";
			}

			formattedContent = formattedContent
				+ "<table style='width: " + tankTableWidth + "%'>";

			// Product row
			formattedContent = formattedContent
				+ "<tr style='background: " + FMGeoTrackingMapPopupFormatter.lightGray + "'><td style='padding-left: 3px;'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.ProductLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right; background: " + FMGeoTrackingMapPopupFormatter.lightGray + "; padding-right: 5px;'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + productValue + "</label>"
				+ "</td></tr>";

			// Volume row
			formattedContent = formattedContent
				+ "<tr><td style='padding-left: 3px;'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.VolumeLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right; padding-right: 5px;'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + tankVolumeList[nextTankIndex] + "</label>"
				+ "</td></tr>";

			// Density row
			var densityRowBg = FMGeoTrackingMapPopupFormatter.lightGray;
			var densityLabelColor = FMGeoTrackingMapPopupFormatter.darkGray;
			var densityValueColor = FMGeoTrackingMapPopupFormatter.blue;

			if (tankItemColorList[nextTankIndex] === "ORANGE")
			{
				densityRowBg = FMGeoTrackingMapPopupFormatter.orange;
				densityLabelColor = FMGeoTrackingMapPopupFormatter.blue;
				densityValueColor = FMGeoTrackingMapPopupFormatter.blue;
			}

			if (tankItemColorList[nextTankIndex] === "YELLOW")
			{
				densityRowBg = FMGeoTrackingMapPopupFormatter.yellow;
				densityLabelColor = FMGeoTrackingMapPopupFormatter.blue;
				densityValueColor = FMGeoTrackingMapPopupFormatter.blue;
			}

			if (tankItemColorList[nextTankIndex] === "RED")
			{
				densityRowBg = FMGeoTrackingMapPopupFormatter.red;
				densityLabelColor = FMGeoTrackingMapPopupFormatter.white;
				densityValueColor = FMGeoTrackingMapPopupFormatter.white;
			}

			if (tankItemColorList[nextTankIndex] === "GREEN")
			{
				densityRowBg = FMGeoTrackingMapPopupFormatter.green;
				densityLabelColor = FMGeoTrackingMapPopupFormatter.white;
				densityValueColor = FMGeoTrackingMapPopupFormatter.white;
			}

			var densityLabelName = model.DensityLabelDictionary;
			if (tankItemContaminatedList[nextTankIndex] === "YES")
			{
				densityLabelName = densityLabelName + " " + String.fromCharCode(FMGeoTrackingMapPopupFormatter.radioActiveSymbolCode);;
			}

			formattedContent = formattedContent
				+ "<tr style='background: " + densityRowBg + "'><td style='padding-left: 3px;'>"
				+ "<label style='color: " + densityLabelColor + "; font-size: 10px;'>" + densityLabelName + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right; background: " + densityRowBg + "; padding-right: 5px;'>"
				+ "<label style='color: " + densityValueColor + "; font-size: 10px;'>" + tankDensityList[nextTankIndex] + "</label>"
				+ "</td></tr>";

			formattedContent = formattedContent
				+ "</table></div>";
		}

		// End the div tab content tag.
		formattedContent = formattedContent + "</div>";
	}

	// End the div container tag.
	formattedContent = formattedContent + "</div>";

	// Timestamp section
	formattedContent = formattedContent + "<div>";
	formattedContent = formattedContent + "<table style='width: 104%'>";
	formattedContent = formattedContent
						+ "<tr><td>"
						+ "<label></label>"
						+ "</td></tr>";
	formattedContent = formattedContent
						+ "<tr><td>"
						+ "<label></label>"
						+ "</td></tr>";
	formattedContent = formattedContent
						+ "<tr><td style='padding-left: 3px;'>"
						+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.TimestampLabelDictionary + "</label>"
						+ "</td>";

	formattedContent = formattedContent
						+ "<td style='float: right; padding-right: 5px;'>"
						+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + timestampValue + "</label>"
						+ "</td></tr>";

	formattedContent = formattedContent
		+ "</table></div>";

	// Create view history link div.
	var historyIcon = FMGeoTrackingMapPopupFormatter.GetImagesPath() + "/History-icon.png"; 
	var viewHistoryBtnStyle = " style='border: none; background-color: white; font-size: 10px;'";
	formattedContent = formattedContent
		+ "<div style='padding-left: 30px;'>"
		+ "<img src='" + historyIcon + "' style='width: 19px; height: 19px;'/>"
		+ "<button onclick=\"BuildEquipmentHistoryPopup('" + headerValue + "');\" " + viewHistoryBtnStyle + ">" + model.ViewHistoryLabelDictionary + "</button>"
		+ "</div>";
	return formattedContent;
}

//===============================================================================
// This function will format the delivery location type information.
//===============================================================================
FMGeoTrackingMapPopupFormatter.FormatDeliveryLocationType = function (popupContentData)
{
	var contentParts		= popupContentData.split("|");
	var formattedContent	= "<div>" + "<table style='width: 100%'>";
	var nextItem			= 0;
	var continueLoop		= true;
	var model				= FMGeoTrackingMap.GetMapViewModel;

	while (continueLoop)
	{
		if (contentParts[nextItem] === "HEADER")
		{
			var headerValue = contentParts[++nextItem];

			formattedContent = formattedContent
				+ "<tr><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.mediumGray + "; font-size: 10px;'>" + model.DeliveryLocationLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.mediumGray + "; font-size: 10px;'>" + headerValue + "</label>"
				+ "</td></tr>";
		}

		if (contentParts[nextItem] === "COORDINATE")
		{
			var latitudeValue  = contentParts[++nextItem];
			var longitudeValue = contentParts[++nextItem];

			formattedContent = formattedContent
				+ "<tr style='background: " + FMGeoTrackingMapPopupFormatter.lightGray + "'><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.LatitudeLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + latitudeValue + "</label>"
				+ "</td></tr>";

			formattedContent = formattedContent
				+ "<tr><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.LongitudeLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + longitudeValue + "</label>"
				+ "</td></tr>";
		}

		nextItem++;
		if (nextItem >= formattedContent.length)
		{
			continueLoop = false;
		}
	}

	formattedContent = formattedContent + "</table></div>";
	FMGeoTrackingMapPopupFormatter.height = 80;
	return formattedContent;
}

//===============================================================================
// This function will format the facility type information.
//===============================================================================
FMGeoTrackingMapPopupFormatter.FormatFacilityType = function (popupContentData)
{
	var contentParts		= popupContentData.split("|");
	var formattedContent	= "<div>" + "<table style='width: 100%'>";
	var nextItem			= 0;
	var continueLoop		= true;
	var model				= FMGeoTrackingMap.GetMapViewModel();

	while (continueLoop)
	{
		if (contentParts[nextItem] === "HEADER")
		{
			var headerValue = contentParts[++nextItem];

			formattedContent = formattedContent
				+ "<tr><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.mediumGray + "; font-size: 10px;'>" + model.FacilityLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.mediumGray + "; font-size: 10px;'>" + headerValue + "</label>"
				+ "</td></tr>";
		}

		if (contentParts[nextItem] === "COORDINATE")
		{
			var latitudeValue = contentParts[++nextItem];
			var longitudeValue = contentParts[++nextItem];

			formattedContent = formattedContent
				+ "<tr style='background: " + FMGeoTrackingMapPopupFormatter.lightGray + "'><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.LatitudeLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + latitudeValue + "</label>"
				+ "</td></tr>";

			formattedContent = formattedContent
				+ "<tr><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.LongitudeLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + longitudeValue + "</label>"
				+ "</td></tr>";
		}

		nextItem++;
		if (nextItem >= formattedContent.length)
		{
			continueLoop = false;
		}
	}

	formattedContent = formattedContent + "</table></div>";
	FMGeoTrackingMapPopupFormatter.height = 80;
	return formattedContent;
}

//===============================================================================
// This function will format the tank type information.
//===============================================================================
FMGeoTrackingMapPopupFormatter.FormatTankType = function (popupContentData)
{
	var contentParts		= popupContentData.split("|");
	var formattedContent	= "<div>" + "<table style='width: 100%'>";
	var nextItem			= 0;
	var continueLoop		= true;
	var model               = FMGeoTrackingMap.GetMapViewModel();
	var rowCount            = 0;
	var rowBgColor          = FMGeoTrackingMapPopupFormatter.lightGray;

	while (continueLoop)
	{
		if (contentParts[nextItem] === "HEADER")
		{
		    var headerValue = contentParts[++nextItem];
		    rowCount++;

			formattedContent = formattedContent
				+ "<tr><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.mediumGray + "; font-size: 10px;'>" + model.TankLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.mediumGray + "; font-size: 10px;'>" + headerValue + "</label>"
				+ "</td></tr>";
		}

		if (contentParts[nextItem] === "COORDINATE")
		{
			var latitudeValue = contentParts[++nextItem];
			var longitudeValue = contentParts[++nextItem];
			rowCount+=2;

			formattedContent = formattedContent
				+ "<tr style='background: " + FMGeoTrackingMapPopupFormatter.lightGray + "'><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.LatitudeLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + latitudeValue + "</label>"
				+ "</td></tr>";

			formattedContent = formattedContent
				+ "<tr><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.LongitudeLabelDictionary + "</label>"
				+ "</td>";

			formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + longitudeValue + "</label>"
				+ "</td></tr>";
		}

		if (contentParts[nextItem] === "TEMPERATURE")
		{
		    var temperature = contentParts[++nextItem];
		    rowCount++;

		    formattedContent = formattedContent
				+ "<tr style='background: " + FMGeoTrackingMapPopupFormatter.lightGray + "'><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.TemperatureLabelDictionary + "</label>"
				+ "</td>";

		    formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + temperature + "</label>"
				+ "</td></tr>";
		}

		if (contentParts[nextItem] === "PRESSURE")
		{
		    var pressure = contentParts[++nextItem];
		    rowCount++;

		    formattedContent = formattedContent
				+ "<tr style='background: " + FMGeoTrackingMapPopupFormatter.white + "'><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.PressureLabelDictionary + "</label>"
				+ "</td>";

		    formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + pressure + "</label>"
				+ "</td></tr>";
		}

		if (contentParts[nextItem] === "GROSSVOLUME")
		{
		    var grossVolume = contentParts[++nextItem];
		    rowCount++;

		    formattedContent = formattedContent
				+ "<tr style='background: " + FMGeoTrackingMapPopupFormatter.lightGray + "'><td>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.darkGray + "; font-size: 10px;'>" + model.VolumeLabelDictionary + "</label>"
				+ "</td>";

		    formattedContent = formattedContent
				+ "<td style='float: right'>"
				+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + grossVolume + "</label>"
				+ "</td></tr>";
		}

		if (contentParts[nextItem] === "TIMESTAMP")
		{
		    var timestampValue = contentParts[++nextItem];
		    rowCount++;

		    formattedContent = formattedContent
						+ "<td style='float: right; padding-right: 5px;'>"
						+ "<label style='color: " + FMGeoTrackingMapPopupFormatter.blue + "; font-size: 10px;'>" + timestampValue + "</label>"
						+ "</td></tr>";
		}

		nextItem++;
		if (nextItem >= formattedContent.length)
		{
			continueLoop = false;
		}
	}

	formattedContent = formattedContent + "</table></div>";
	FMGeoTrackingMapPopupFormatter.height = 80;

	if (rowCount > 3)
	{
	    FMGeoTrackingMapPopupFormatter.height = rowCount / .05;
	}

	return formattedContent;
}

//=========================================================================
// This function will handle the tab click events which are really button
// events.  It will highlight the tab and change foreground color of the
// highlighted tab text.
//=========================================================================
FMGeoTrackingMapPopupFormatter.TankTabOnClick = function (tankTabIndex)
{
	var tankTabs = [];
	var tankButtons = [];
	var nextTabIndex;

	var tankDivObj1 = document.getElementById("TANK1");
	var tankDivObj2 = document.getElementById("TANK2");
	var tankDivObj3 = document.getElementById("TANK3");
	var tankDivObj4 = document.getElementById("TANK4");

	var tankButton1 = document.getElementById("TankButton1");
	var tankButton2 = document.getElementById("TankButton2");
	var tankButton3 = document.getElementById("TankButton3");
	var tankButton4 = document.getElementById("TankButton4");

	if (tankDivObj1 != null)
	{
		tankTabs.push(tankDivObj1);
		tankButtons.push(tankButton1);
	}

	if (tankDivObj2 != null)
	{
		tankTabs.push(tankDivObj2);
		tankButtons.push(tankButton2);
	}

	if (tankDivObj3 != null)
	{
		tankTabs.push(tankDivObj3);
		tankButtons.push(tankButton3);
	}

	if (tankDivObj4 != null)
	{
		tankTabs.push(tankDivObj4);
		tankButtons.push(tankButton4);
	}

	for (nextTabIndex = 0; nextTabIndex < tankTabs.length; nextTabIndex++)
	{
		tankTabs[nextTabIndex].style.display = "none";
		tankButtons[nextTabIndex].style.backgroundColor = FMGeoTrackingMapPopupFormatter.lightGray;
		tankButtons[nextTabIndex].innerText = (nextTabIndex + 1).toString();
		tankButtons[nextTabIndex].style.color = FMGeoTrackingMapPopupFormatter.black;
	}

	for (nextTabIndex = 0; nextTabIndex < tankTabs.length; nextTabIndex++)
	{
		var calTabIndex = tankTabIndex - 1;
		
		if (nextTabIndex === calTabIndex)
		{
			tankTabs[nextTabIndex].style.display = "block";
			tankButtons[nextTabIndex].style.backgroundColor = FMGeoTrackingMapPopupFormatter.blue;
			tankButtons[nextTabIndex].innerText = "Compartment";
			tankButtons[nextTabIndex].style.color = FMGeoTrackingMapPopupFormatter.white;
			break;
		}
	}
}

//================================================================
// This function will get the URL path for the "images".
//================================================================
FMGeoTrackingMapPopupFormatter.GetImagesPath = function ()
{
	if (FMGeoTrackingMapPopupFormatter.imageRootPath == null || FMGeoTrackingMapPopupFormatter.imageRootPath === "")
	{
		FMGeoTrackingMapPopupFormatter.imageRootPath = "FuelsManager/Areas/images/";
	}

	var protocol = window.location.protocol;
	var host = window.location.host;
	var sourcePath = protocol + "//" + host + "/" + FMGeoTrackingMapPopupFormatter.imageRootPath;

	return sourcePath;
}

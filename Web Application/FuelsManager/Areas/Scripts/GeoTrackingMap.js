var FMGeoTrackingMap = FMGeoTrackingMap ||
	{
	  currentMarkerUrl: ""
	, crumbMarkerUrl: ""
	, currentContaminatedMarkerUrl: ""
	, currentInvestigationMarkerUrl: ""
	, currentCompleteInvestigationFailedMarkerUrl: ""
	, currentCompleteInvestigationPassedMarkerUrl: ""
	, crumbContaminatedMarkerUrl: ""
	, crumbInvestigationMarkerUrl: ""
	, crumbCompleteInvestigationFailedMarkerUrl: ""
	, crumbCompleteInvestigationPassedMarkerUrl: ""
	, facilityMarkerUrl: ""
	, deliveryLocationMarkerUrl: ""
	, tankMarkerUrl: ""
	, wgs1984Projection: ""
	, sphericalMecatorProjection: ""
	, mapObj: ""
	, controls: ""
	, autoRefreshTime: 0
	, lastPopupFeature: null
	, retrievingDataErrorMsg: ""
	, retrievingDataErrorFlag: false
	, markerZorderPrime: 100
	, markerZorderSecond: 99
	, breadcrumbZorderPrime: 95
	, breadcrumbZorderSecond: 94
	, deliveryLocationZorderPrime: 90
	, deliveryLocationZorderSecond: 89
	, facilityZorderPrime: 85
	, facilityZorderSecond: 84
	, tankZorderPrime: 80
	, tankZorderSecond: 79
};

FMGeoTrackingMap.imageRootPath = null;

//=========================================================================
// This function will retrieve the equipment data associated to asset
// tracking devices to be placed on the map.
//=========================================================================
FMGeoTrackingMap.GetEquipmentMapData = function ()
{
	var equipmentMapDataUrl = "GetEquipmentMapDataJson";
	var equipmentMapDataList = null;
	var breadcrumbSelection = $("#DropdownModel_SelectedBreadcrumbIndex").val();

	var token = $('#MapBaseForm input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// Get the equipment map data.
	$.ajax({
		cache: false,
		type: "POST",
		async: false,
		headers: headers,
		beforeSend: function ()
		{
			$("#loadingIndicatorModal").css("visibility", "visible");
			$("#loadingIndicatorFade").css("visibility", "visible");
		},
		complete: function ()
		{
			$("#loadingIndicatorModal").css("visibility", "hidden");
			$("#loadingIndicatorFade").css("visibility", "hidden");
		},
		url: equipmentMapDataUrl,
		dataType: "json",
		data: { breadcrumbSelection: breadcrumbSelection },
		success: function (equipmentDataList)
		{
			equipmentMapDataList = equipmentDataList;
		},
		error: function ()
		{
			FMGeoTrackingMap.retrievingDataErrorFlag = true;
			FMGeoTrackingMap.AppendErrorMessage("equipment map data");
		}
	});

	return equipmentMapDataList;
}

//=========================================================================
// This function will retrieve the facility data that has map coordinates
// configured.
//=========================================================================
FMGeoTrackingMap.GetFacilityMapData = function ()
{
	var facilityMapDataUrl = "GetFacilityMapDataJson";
	var facilityMapDataList = null;

	var token = $('#MapBaseForm input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// Get the facility map data.
	$.ajax({
		cache: false,
		type: "POST",
		async: false,
		url: facilityMapDataUrl,
		headers: headers,
		dataType: "json",
		success: function (facilityDataList)
		{
			facilityMapDataList = facilityDataList;
		},
		error: function ()
		{
			FMGeoTrackingMap.retrievingDataErrorFlag = true;
			FMGeoTrackingMap.AppendErrorMessage("facility map data");
		}
	});

	return facilityMapDataList;
}

//=========================================================================
// This function will retrieve the facility data that has map coordinates
// configured.
//=========================================================================
FMGeoTrackingMap.GetTankMapData = function ()
{
	var tankMapDataUrl = "GetTankMapDataJson";
	var tankMapDataList = null;

	var token = $('#MapBaseForm input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// Get the facility map data.
	$.ajax({
		cache: false,
		type: "POST",
		async: false,
		url: tankMapDataUrl,
		headers: headers,
		dataType: "json",
		success: function (tankDataList)
		{
			tankMapDataList = tankDataList;
		},
		error: function ()
		{
			FMGeoTrackingMap.retrievingDataErrorFlag = true;
			FMGeoTrackingMap.AppendErrorMessage("tank map data");
		}
	});

	return tankMapDataList;
}

//=========================================================================
// This function will retrieve the delivery location data that has map 
// coordinates configured.
//=========================================================================
FMGeoTrackingMap.GetDeliveryLocationMapData = function ()
{
	var deliveryLocMapDataUrl = "GetDeliveryLocationMapDataJson";
	var deliveryLocMapDataList = null;

	var token = $('#MapBaseForm input[name =__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	// Get the facility map data.
	$.ajax({
		cache: false,
		type: "POST",
		async: false,
		url: deliveryLocMapDataUrl,
		headers: headers,
		dataType: "json",
		success: function (deliveryLocDataList)
		{
			deliveryLocMapDataList = deliveryLocDataList;
		},
		error: function ()
		{
			FMGeoTrackingMap.retrievingDataErrorFlag = true;
			FMGeoTrackingMap.AppendErrorMessage("delivery location map data");
		}
	});

	return deliveryLocMapDataList;
}

//==============================================================================
// This function will add the breadcrumbs for the equipment locations.
// The most currect location will use a different icon than the breadcrumbs.
//==============================================================================
FMGeoTrackingMap.AddBreadCrumbs = function (map, vectorLayer, equipmentMapDataList)
{
	if (equipmentMapDataList == null)
	{
		return;
	}

	for (var nextEquipIndex = 0; nextEquipIndex < equipmentMapDataList.length; nextEquipIndex++)
	{
		var equipmentMapData = equipmentMapDataList[nextEquipIndex];
		var equipmentId = equipmentMapData.EquipmentId.toString();

		for (var nextCoordinate = 0; nextCoordinate < equipmentMapData.LatitudeList.length; nextCoordinate++)
		{
			var markerLat		= equipmentMapData.LatitudeList[nextCoordinate];
			var markerLong		= equipmentMapData.LongitudeList[nextCoordinate];
			var description		= equipmentMapData.HoverDescriptionList[nextCoordinate];
			var itemColor		= equipmentMapData.ItemColorList[nextCoordinate];

			var point = new OpenLayers.Geometry.Point(markerLong, markerLat).transform(FMGeoTrackingMap.wgs1984Projection, FMGeoTrackingMap.sphericalMecatorProjection);
			var feature;

			if (equipmentMapData.MarkerTypeList[nextCoordinate] === "M")
			{
				var markerUrl = FMGeoTrackingMap.currentMarkerUrl;

				if (itemColor === "ORANGE")
				{
					markerUrl = FMGeoTrackingMap.currentContaminatedMarkerUrl;
				}

				if (itemColor === "YELLOW")
				{
					markerUrl = FMGeoTrackingMap.currentInvestigationMarkerUrl;
				}

				if (itemColor === "GREEN")
				{
					markerUrl = FMGeoTrackingMap.currentCompleteInvestigationPassedMarkerUrl;
				}

				if (itemColor === "RED")
				{
					markerUrl = FMGeoTrackingMap.currentCompleteInvestigationFailedMarkerUrl;
				}

				feature = new OpenLayers.Feature.Vector(point,
				{ description: description, type: "Equipment", id: equipmentId, subType: "M" },
				{ externalGraphic: markerUrl, graphicHeight: 25, graphicWidth: 21, graphicXOffset: -12, graphicYOffset: -25, graphicZIndex: FMGeoTrackingMap.markerZorderPrime });
				vectorLayer.addFeatures(feature);
			}
			else if (equipmentMapData.MarkerTypeList[nextCoordinate] === "C")
			{
				var crumbUrl = FMGeoTrackingMap.crumbMarkerUrl;

				if (itemColor === "ORANGE")
				{
					crumbUrl = FMGeoTrackingMap.crumbContaminatedMarkerUrl;
				}

				if (itemColor === "YELLOW")
				{
					crumbUrl = FMGeoTrackingMap.crumbInvestigationMarkerUrl;
				}

				if (itemColor === "GREEN")
				{
					crumbUrl = FMGeoTrackingMap.crumbCompleteInvestigationPassedMarkerUrl;
				}

				if (itemColor === "RED")
				{
					crumbUrl = FMGeoTrackingMap.crumbCompleteInvestigationFailedMarkerUrl;
				}

				feature = new OpenLayers.Feature.Vector(point,
				{ description: description, type: "Equipment", id: equipmentId, subType: "C" },
				{ externalGraphic: crumbUrl, graphicHeight: 15, graphicWidth: 15, graphicXOffset: -12, graphicYOffset: -25, graphicZIndex: FMGeoTrackingMap.breadcrumbZorderPrime });
				vectorLayer.addFeatures(feature);
			}
		}
	}

	FMGeoTrackingMap.mapObj.addLayer(vectorLayer);
}

//=====================================================================================
// This function will add facility markers.
//=====================================================================================
FMGeoTrackingMap.AddFacilityMarkers = function(map, vectorLayer, facilityDataList)
{
	if (facilityDataList == null)
	{
		return;
	}

	for (var nextFacilityIndex = 0; nextFacilityIndex < facilityDataList.length; nextFacilityIndex++)
	{
		var facilityMapData = facilityDataList[nextFacilityIndex];

		var markerLat = facilityMapData.LatitudeList[0];
		var markerLong = facilityMapData.LongitudeList[0];
		var description = facilityMapData.HoverDescriptionList[0];
		var facilityId = facilityMapData.FacilityId;

		var point = new OpenLayers.Geometry.Point(markerLong, markerLat).transform(FMGeoTrackingMap.wgs1984Projection, FMGeoTrackingMap.sphericalMecatorProjection);
		var feature = new OpenLayers.Feature.Vector(point,
		{ description: description, type: "Facility", id: facilityId, subType: "" },
		{ externalGraphic: FMGeoTrackingMap.facilityMarkerUrl, graphicHeight: 25, graphicWidth: 21, graphicXOffset: 0, graphicYOffset: 0, graphicZIndex: FMGeoTrackingMap.facilityZorderPrime });

		vectorLayer.addFeatures(feature);
	}

	FMGeoTrackingMap.mapObj.addLayer(vectorLayer);
}

//=====================================================================================
// This function will add tank markers.
//=====================================================================================
FMGeoTrackingMap.AddTankMarkers = function (map, vectorLayer, tankDataList)
{
	if (tankDataList == null)
	{
		return;
	}

	for (var nextTankIndex = 0; nextTankIndex < tankDataList.length; nextTankIndex++)
	{
		var tankMapData = tankDataList[nextTankIndex];

		var markerLat = tankMapData.LatitudeList[0];
		var markerLong = tankMapData.LongitudeList[0];
		var description = tankMapData.HoverDescriptionList[0];
		var tankId = tankMapData.TankId;

		var point = new OpenLayers.Geometry.Point(markerLong, markerLat).transform(FMGeoTrackingMap.wgs1984Projection, FMGeoTrackingMap.sphericalMecatorProjection);
		var feature = new OpenLayers.Feature.Vector(point,
		{ description: description, type: "Tank", id: tankId, subType: "" },
		{ externalGraphic: FMGeoTrackingMap.tankMarkerUrl, graphicHeight: 25, graphicWidth: 21, graphicXOffset: 0, graphicYOffset: 0, graphicZIndex: FMGeoTrackingMap.tankZorderPrime });

		vectorLayer.addFeatures(feature);
	}

	FMGeoTrackingMap.mapObj.addLayer(vectorLayer);
}

//=====================================================================================
// This function will add delivery location markers.
//=====================================================================================
FMGeoTrackingMap.AddDeliveryLocationMarkers = function (map, vectorLayer, deliveryLocationDataList)
{
	if (deliveryLocationDataList == null)
	{
		return;
	}

	for (var nextDeliveryLocIndex = 0; nextDeliveryLocIndex < deliveryLocationDataList.length; nextDeliveryLocIndex++)
	{
		var deliveryLocationMapData = deliveryLocationDataList[nextDeliveryLocIndex];

		var markerLat = deliveryLocationMapData.LatitudeList[0];
		var markerLong = deliveryLocationMapData.LongitudeList[0];
		var description = deliveryLocationMapData.HoverDescriptionList[0];
		var deliveryLocationId = deliveryLocationMapData.DeliveryLocationId;

		var point = new OpenLayers.Geometry.Point(markerLong, markerLat).transform(FMGeoTrackingMap.wgs1984Projection, FMGeoTrackingMap.sphericalMecatorProjection);
		var feature = new OpenLayers.Feature.Vector(point,
		{ description: description, type: "DeliveryLocation", id: deliveryLocationId, subType: "" },
		{ externalGraphic: FMGeoTrackingMap.deliveryLocationMarkerUrl, graphicHeight: 25, graphicWidth: 21, graphicXOffset: 0, graphicYOffset: 0, graphicZIndex: FMGeoTrackingMap.deliveryLocationZorderPrime });

		vectorLayer.addFeatures(feature);
	}

	FMGeoTrackingMap.mapObj.addLayer(vectorLayer);
}

//=======================================================================================
// This function will hide and show features on the map based on the menu selection.
//=======================================================================================
FMGeoTrackingMap.MapFilter = function (inSelectedItems, filterType)
{
	var layer;

	for (var nextLayer = 0; nextLayer < FMGeoTrackingMap.mapObj.layers.length; nextLayer++)
	{
		layer = FMGeoTrackingMap.mapObj.layers[nextLayer];

		if (layer != null && layer.name === "OverlayAll")
		{
			break;
		}
	}

	if (layer == null)
	{
		return;
	}

	if (layer.features == null || typeof (layer.features) === "undefined")
	{
		return;
	}

	var features = layer.features;
	var selectedItems = [];

	if (inSelectedItems != null)
	{
		var commaIndex = inSelectedItems.indexOf(",");

		if (commaIndex < 0)
		{
			selectedItems.push(inSelectedItems);
		}
		else
		{
			selectedItems = inSelectedItems.split(",");
		}
	}

	var next;
	for (next = 0; next < features.length; next++)
	{
		if (features[next].data.type === filterType)
		{
			// Hide all items based on filter type.
			features[next].style.fillOpacity = 0;
			features[next].style.graphicZIndex = FMGeoTrackingMap.GetZorderSecondary(filterType, features[next].data.subType);
		}
	}

	for (var nextItem = 0; nextItem < selectedItems.length; nextItem++)
	{
		// Show all the features (All = -88)
		if (selectedItems[nextItem] === "-88")
		{
			for (next = 0; next < features.length; next++)
			{
				if (features[next].data.type === filterType)
				{
					// Show all items based on filter type.
					features[next].style.fillOpacity = 1;
					features[next].style.graphicZIndex = FMGeoTrackingMap.GetZorderPrimary(filterType, features[next].data.subType);
				}
			}
		}

		// Show the selected feature and hide all others.
		if (selectedItems[nextItem] !== "-88" && selectedItems[nextItem] !== "")
		{
			for (next = 0; next < features.length; next++)
			{
				if (features[next].data.type === filterType)
				{
					if (features[next].data.id === selectedItems[nextItem])
					{
						// Show all items based on filter type.
						features[next].style.fillOpacity = 1;
						features[next].style.graphicZIndex = FMGeoTrackingMap.GetZorderPrimary(filterType, features[next].data.subType);
					}
				}
			}
		}
	}

	layer.redraw();
}

//=================================================================
// This function will return the Primary Z Order for the given
// filter type and sub type.
//=================================================================
FMGeoTrackingMap.GetZorderPrimary = function (filterType, subType)
{
	if (filterType === "Equipment" && subType === "M") return FMGeoTrackingMap.markerZorderPrime;
	if (filterType === "Equipment" && subType === "C") return FMGeoTrackingMap.breadcrumbZorderPrime;
	if (filterType === "Tank") return FMGeoTrackingMap.tankZorderPrime;
	if (filterType === "Facility") return FMGeoTrackingMap.facilityZorderPrime;
	if (filterType === "DeliveryLocation") return FMGeoTrackingMap.deliveryLocationZorderPrime;

	return 70;
}

//=================================================================
// This function will return the Secondary Z Order for the given
// filter type and sub type.
//=================================================================
FMGeoTrackingMap.GetZorderSecondary = function (filterType, subType)
{
	if (filterType === "Equipment" && subType === "M") return FMGeoTrackingMap.markerZorderSecond;
	if (filterType === "Equipment" && subType === "C") return FMGeoTrackingMap.breadcrumbZorderSecond;
	if (filterType === "Tank") return FMGeoTrackingMap.tankZorderSecond;
	if (filterType === "Facility") return FMGeoTrackingMap.facilityZorderSecond;
	if (filterType === "DeliveryLocation") return FMGeoTrackingMap.deliveryLocationZorderSecond;

	return 70;
}

//=======================================================================================
// This function handles the refresh button or icon button on click event. It will
// make a call back to the server to get the new data and refresh the map.
//=======================================================================================
FMGeoTrackingMap.HandleRefreshEvent = function (closeEquipmentHistory)
{
	var layer;

	for (var nextLayer = 0; nextLayer < FMGeoTrackingMap.mapObj.layers.length; nextLayer++)
	{
		layer = FMGeoTrackingMap.mapObj.layers[nextLayer];

		if (layer != null && layer.name === "OverlayAll")
		{
			break;
		}
	}

	if (layer == null)
	{
		return;
	}

	// Remove a feature (popup that may be displayed) and close the equipment history.
	FMGeoTrackingMap.destroyPopup(FMGeoTrackingMap.lastPopupFeature);

	if (closeEquipmentHistory)
	{
		FMGeoTrackingEquipmentHistory.CloseEquipmentHistory();
	}

	// Remove the old layer and create a new one.
	FMGeoTrackingMap.mapObj.removeLayer(layer);
	var vectorLayerAll = new OpenLayers.Layer.Vector("OverlayAll", { rendererOptions: { zIndexing: true } });

	// Clear retrieving error message and flag.
	FMGeoTrackingMap.retrievingDataErrorMsg = "";
	FMGeoTrackingMap.retrievingDataErrorFlag = false;

	// Retrieve the data from the server
	var equipmentMapDataList		= FMGeoTrackingMap.GetEquipmentMapData();
	var facilityMapDataList			= FMGeoTrackingMap.GetFacilityMapData();
	var tankMapDataList				= FMGeoTrackingMap.GetTankMapData();
	var deliveryLocationMapDataList = FMGeoTrackingMap.GetDeliveryLocationMapData();

	// Add the data back to the map
	FMGeoTrackingMap.AddBreadCrumbs(FMGeoTrackingMap.map, vectorLayerAll, equipmentMapDataList);
	FMGeoTrackingMap.AddFacilityMarkers(FMGeoTrackingMap.map, vectorLayerAll, facilityMapDataList);
	FMGeoTrackingMap.AddTankMarkers(FMGeoTrackingMap.map, vectorLayerAll, tankMapDataList);
	FMGeoTrackingMap.AddDeliveryLocationMarkers(FMGeoTrackingMap.map, vectorLayerAll, deliveryLocationMapDataList);

	// Add a selector control to the vectorLayer that contains the popup functions.
	FMGeoTrackingMap.controls = {
		selector: new OpenLayers.Control.SelectFeature(vectorLayerAll,
		{ hover: false, onSelect: FMGeoTrackingMap.createPopup, onUnselect: FMGeoTrackingMap.destroyPopup })
	};

	FMGeoTrackingMap.mapObj.addControl(FMGeoTrackingMap.controls['selector']);
	FMGeoTrackingMap.controls['selector'].activate();

	// Filter the map back to match the filter sections.
	FMGeoTrackingMap.MapFilter(FMGeoTrackingSideMenu.selectedEquipment, "Equipment");
	FMGeoTrackingMap.MapFilter(FMGeoTrackingSideMenu.selectedFacility, "Facility");
	FMGeoTrackingMap.MapFilter(FMGeoTrackingSideMenu.selectedTank, "Tank");
	FMGeoTrackingMap.MapFilter(FMGeoTrackingSideMenu.selectedDeliveryLocation, "DeliveryLocation");

	if (FMGeoTrackingMap.retrievingDataErrorFlag)
	{
		alert(FMGeoTrackingMap.retrievingDataErrorMsg);
	}

	FMGeoTrackingMap.AutoRefresh();
}

//========================================================================================
// This function will handle the destroy popup event.
//========================================================================================
FMGeoTrackingMap.destroyPopup = function(feature)
{
	if (feature != null && feature.popup != null)
	{
		feature.popup.destroy();
		feature.popup = null;
		FMGeoTrackingMap.lastPopupFeature = null;
	}
}

//========================================================================================
// This function will handle the create popup event.
//========================================================================================
FMGeoTrackingMap.createPopup = function(feature)
{
	// Return if the feature is not visible.
	if (feature.style.fillOpacity === 0) 
	{
		return;
	}

	var description = FMGeoTrackingMapPopupFormatter.FormatPopupContent(feature.attributes.description);
	var width		= FMGeoTrackingMapPopupFormatter.width;
	var height		= FMGeoTrackingMapPopupFormatter.height;

	// The size of 25 x 25 determines the area of the hover over. This in turn fixes the issue
	// where the popup would not stay present when close to the view port boundries.
	var anchor = { 'size': new OpenLayers.Size(25, 25), 'offset': new OpenLayers.Pixel(0, 0) };

	feature.popup = new OpenLayers.Popup.FramedCloud("pop",
		feature.geometry.getBounds().getCenterLonLat(),
		new OpenLayers.Size(width, height),
		description,
		anchor,
		true,
		function () { FMGeoTrackingMap.controls['selector'].unselectAll(); }
	);

	feature.popup.autoSize = false;
	FMGeoTrackingMap.mapObj.addPopup(feature.popup);
	FMGeoTrackingMap.lastPopupFeature = feature;

	// Need to disable the overflow on the popup.
	$("#pop_contentDiv").css("overflow", "hidden");
}

//========================================================================
// This function will handle the auto refresh event.
//========================================================================
FMGeoTrackingMap.AutoRefresh = function ()
{
	if (FMGeoTrackingMap.autoRefreshTime === 0)
	{
		return;
	}

	setTimeout("FMGeoTrackingMap.HandleRefreshEvent(true);", FMGeoTrackingMap.autoRefreshTime);
}

//========================================================================
// This function will append to the error message.
//========================================================================
FMGeoTrackingMap.AppendErrorMessage = function(msg)
{
	if (FMGeoTrackingMap.retrievingDataErrorMsg == null || FMGeoTrackingMap.retrievingDataErrorMsg === "")
	{
		FMGeoTrackingMap.retrievingDataErrorMsg = "Error retrieving " + msg;
	}
	else
	{
		FMGeoTrackingMap.retrievingDataErrorMsg = FMGeoTrackingMap.retrievingDataErrorMsg + ", " + msg;
	}
}

//=====================================================================
// This function will return the map view model as a string.
//=====================================================================
FMGeoTrackingMap.GetMapViewModelString = function ()
{
	return $('#MapViewModel').val();
};

//=====================================================================
// This function will return the map view model as an object.
//=====================================================================
FMGeoTrackingMap.GetMapViewModel = function ()
{
	var strModel = FMGeoTrackingMap.GetMapViewModelString();

	if (strModel === undefined)
	{
		return undefined;
	}

	var model = JSON.parse(strModel);
	return model;
};

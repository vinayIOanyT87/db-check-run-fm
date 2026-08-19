var MovementHistoryTab = MovementHistoryTab ||
{
    FindArr: null,
    CurrentFind: null,
    CurrentFindString: '',    
    columnFilterCollection: null,
    previousColumnFilterCollection: null,
    originalEditValue: null,
    inEditMode: false,
    editModeId: null,
    changingincludedcolumns: false,
    commentIsEditableSpecialIndex: 77777,
    DatatableHandle: null,
    RefreshTimer: null,
    HistoryInitialized: false,
    TabidNumber: null,
    // notification stack for the screen 
    stack_bottomright_movementhistory: { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#MovementHistoryTableSection') },
    messageAttributes: {},
    initialLoadRequest: false,
    selectedRowData: null,
    checkboxFilterArray: [],
    commentEditId: null,
    GaugeCheckboxChange: false,
    midnightRecordCheckboxChange: false,

    // Rights
    hasModifyMovementHistoryRight: false,
    hasViewMovementHistoryRight: false,

    // Column index variables
    timeStampColumnIndex: 0,
    movementNameColumnIndex: 1,
    movementNodeColumnIndex: 2,
    initiationCountColumnIndex: null,
    siteColumnIndex: null,
    commentColumnIndex: null,

    closeoutDataModifiedByColumnIndex: null,
    closeoutDensityProductInAirColumnIndex: null,
    closeoutDensityProductObservedColumnIndex: null,
    closeoutDensityProductObservedTimeColumnIndex: null,
    closeoutDensityProductStandardColumnIndex: null,
    closeoutDensityProductStandardTimeColumnIndex: null,
    closeoutDensityProductStandardInAirColumnIndex: null,
    closeoutLevelProductColumnIndex: null,
    closeoutLevelProductTimeColumnIndex: null,
    closeoutLevelWaterColumnIndex: null,
    closeoutMassLiquidColumnIndex: null,
    closeoutPercentBswColumnIndex: null,
    closeoutRoofMassColumnIndex: null,
    closeoutTankShellCorrectionColumnIndex: null,
    closeoutTemperatureAmbientColumnIndex: null,
    closeoutTemperatureAmbientTimeColumnIndex: null,
    closeoutTemperatureDensityColumnIndex: null,
    closeoutTemperatureProductColumnIndex: null,
    closeoutTimeColumnIndex: null,
    closeoutTransferGovColumnIndex: null,
    closeoutTransferNsvColumnIndex: null,
    closeoutTransferMassLiquidColumnIndex: null,
    closeoutTransferVolumeWaterColumnIndex: null,
    closeoutVolumeBswColumnIndex: null,
    closeoutVolumeCorrectionFactorColumnIndex: null,
    closeoutVolumeGrossObservedColumnIndex: null,
    closeoutVolumeGrossStandardColumnIndex: null,
    closeoutVolumeNetStandardColumnIndex: null,
    closeoutVolumeRoofCorrectionColumnIndex: null,
    closeoutVolumeTotalObservedColumnIndex: null,
    closeoutVolumeWaterColumnIndex: null,

    levelProductColumnIndex: null,
    typeColumnIndex: null,
    orderNumberColumnIndex: null,
    plannedStartTimeColumnIndex: null,
    productColumnIndex: null,
    productDescriptionColumnIndex: null,

    startTimeColumnIndex: null,
    startDensityProductObservedColumnIndex: null,
    startDensityProductObservedTimeColumnIndex: null,
    startDensityProductObservedInAirColumnIndex: null,
    startDensityProductStandardColumnIndex: null,
    startDensityProductStandardTimeColumnIndex: null,
    startDensityProductStandardInAirColumnIndex: null,
    startUserIdColumnIndex: null,
    startLevelProductColumnIndex: null,
    startLevelProductTimeColumnIndex: null,
    startLevelWaterColumnIndex: null,
    startLevelWaterTimeColumnIndex: null,
    startMassLiquidColumnIndex: null,
    startPercentBsw: null,
    startTankShellCorrectionColumnIndex: null,
    startTemperatureAmbientColumnIndex: null,
    startTemperatureAmbientTimeColumnIndex: null,
    startTemperatureProductColumnIndex: null,
    startTemperatureProductTimeColumnIndex: null,
    startTemperatureDensityColumnIndex: null,
    startTemperatureDensityTimeColumnIndex: null,
    startVolumeColumnIndex: null,
    startVolumeBswColumnIndex: null,
    startVolumeCorrectionFactorColumnIndex: null,
    startVolumeGrossObservedColumnIndex: null,
    startVolumeGrossStandardColumnIndex: null,
    startVolumeNetStandardColumnIndex: null,
    startVolumeRoofCorrectionColumnIndex: null,
    startVolumeTotalObservedColumnIndex: null,
    startVolumeWaterColumnIndex: null,
    stopTimeColumnIndex: null,
    statusColumnIndex: null,

    transferDeviationColumnIndex: null,
    transferPercentDeviationColumnIndex: null,
    transferDirectionColumnIndex: null,
    transferModeColumnIndex: null,
    transferStatusColumnIndex: null,
    transferTargetColumnIndex: null,
	 transferTargetUnitsColumnIndex: null,
    transferLevelTargetColumnIndex: null,
	 transferVolumeTargetColumnIndex: null,
    transferTimeRemainingColumnIndex: null,

    transferredVolumeWaterColumnIndex: null,
    transferredVolumeColumnIndex: null,

    unitsLevelProductColumnIndex: null,
    unitsTemperatureAmbientColumnIndex: null,
    unitsTemperatureDensityColumnIndex: null,
    unitsTemperatureProductColumnIndex: null,
    unitsDensityProductObservedColumnIndex: null,
    unitsDensityProductStandardColumnIndex: null,
    unitsVolumeColumnIndex: null,
    unitsMassColumnIndex: null,

    userData01ColumnIndex: null,
    userData02ColumnIndex: null,
    userData03ColumnIndex: null,
    userData04ColumnIndex: null,
    userData05ColumnIndex: null,
    userData06ColumnIndex: null,
    userData07ColumnIndex: null,
    userData08ColumnIndex: null,
    userData09ColumnIndex: null,
    userData10ColumnIndex: null,

    volumeWater: null,

    commentUserNameColumnIndex: null,
    commentDateTimeColumnIndex: null,

    recordTypeColumnIndex: null,
    parentGuidColumnIndex: null,
    movementHistoryGuidColumnIndex: null,
    pointGuidColumnIndex: null,
    rootParentGuidColumnIndex: null,
    recordSeqColumnIndex: null,
    midnightRecordColumnIndex: null,

    totalColumns: null
};

MovementHistoryTab.applicationRootName = MovementHistoryTab.applicationRootName || (window.location.pathname.length > 1 && window.location.pathname.indexOf('/', 1) > -1 ? window.location.pathname.substr(0, window.location.pathname.indexOf('/', 1)) : window.location.pathname);

//=============================================================
// This function will get the movement model string.
//=============================================================
MovementHistoryTab.GetMovementHistoryModelString = function ()
{
    return $('#MovementHistoryTabModel').val();
}

//=============================================================
// This function will get the movement model.
//=============================================================
MovementHistoryTab.GetMovementHistoryModel = function ()
{
    return JSON.parse(MovementHistoryTab.GetMovementHistoryModelString());
}

//=============================================================
// This function will set the movement model string.
//=============================================================
MovementHistoryTab.SetMovementHistoryModelString = function (modelStr)
{
    $('#MovementHistoryTabModel').val(modelStr);
}

//=============================================================
// This function will set the movement model.
//=============================================================
MovementHistoryTab.SetMovementHistoryModel = function (model)
{
    var modelStr = JSON.stringify(model);
    MovementHistoryTab.SetMovementHistoryModelString(modelStr);
}

//=================================================================
// This function return an error if more than one movement is 
// selected.
//=================================================================
MovementHistoryTab.Help = function ()
{
    var guidList = MovementHistoryTab.GetCurrentlySelectedViewableMovements();

    if (!guidList || guidList.length !== 1)
    {
        FMErrorAndExceptionHandling.ShowError('Select Only One Movement For This Operation', null, MovementHistoryTab.messageAttributes);
    }
    else
    {
        console.log("MovementHistoryTab.Help called");
    }
    //Prevent Post
    return false;
}

//=================================================================
// This function return an error if move than one movement is
// selected.
//=================================================================
MovementHistoryTab.Details = function ()
{
    var guidList = MovementHistoryTab.GetCurrentlySelectedViewableMovements();

    if (!guidList || guidList.length !== 1)
    {
        FMErrorAndExceptionHandling.ShowError('Select Only One Movement For This Operation', null, MovementHistoryTab.messageAttributes);
    }
    else
    {
        console.log("MovementHistoryTab.Details called");
    }
    //Prevent Post
    return false;
}

//========================================================================
// This function true if the element is in the view port.
//========================================================================
MovementHistoryTab.IsElementInViewport = function (par, el, floatingHeader)
{
    var elRect = el.getBoundingClientRect();
    var parRect = par.getBoundingClientRect();
    var winBottom = $(window).height();
    var floatingHeaderHeight = 0;

    if (floatingHeader)
    {
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

//=====================================================================
// This function gets the currently viewable movements.
//=====================================================================
MovementHistoryTab.GetCurrentlyViewableMovements = function ()
{
    var container = document.getElementById("MovementHistoryTableContainer");
    var tr = container.getElementsByTagName("tr");
    var visible = [];
    var header = container.getElementsByTagName("thead")[0];

    for (var i = 0; i < tr.length; i++)
    {
        var cur = tr[i];
        if (cur.id.startsWith("Row_") && MovementHistoryTab.IsElementInViewport(container, cur, header))
        {
            visible.push(cur.id.replace('Row_', ''));
        }
    }
    return visible;
}

//====================================================================
// This function gets the currently selected viewable movements.
//====================================================================
MovementHistoryTab.GetCurrentlySelectedViewableMovements = function ()
{
    var selectedRowIds = [];
    var visibleMovements = MovementHistoryTab.GetCurrentlyViewableMovements();

    for (var i = 0; i < visibleMovements.length; i++)
    {
        var rowElement = document.getElementById("Row_" + visibleMovements[i]);

        if (rowElement)
        {
            for (var j = 0; j < rowElement.classList.length; j++)
            {
                if (rowElement.classList[j] === "selected")
                {
                    selectedRowIds.push(visibleMovements[i]);
                    break;
                }
            }
        }
    }
    return selectedRowIds;
}

//=======================================================================
// This function creates the columns in the data table.
//=======================================================================
MovementHistoryTab.CreateColumns = function ()
{
    var cols = [
        { "data": "TimeStampStr", "orderable": true, "visible": true }
        , { "data": "Name", "orderable": true, "visible": true }
        , { "data": "Node", "orderable": false, "visible": true }
        , { "data": "InitiationCount", "orderable": false, "visible": false }
        , { "data": "SiteId", "orderable": false, "visible": true }
        , { "data": "Comment", "orderable": false, "visible": false }
        , { "data": "CloseoutDataModifiedBy", "orderable": false, "visible": false }
        , { "data": "CloseoutDensityProductInAirStr", "orderable": false, "visible": false }
        , { "data": "CloseoutDensityProductObservedStr", "orderable": false, "visible": false }
        , { "data": "CloseoutDensityProductObservedTimeStr", "orderable": false, "visible": false }
        , { "data": "CloseoutDensityProductStandardStr", "orderable": false, "visible": false }
        , { "data": "CloseoutDensityProductStandardTimeStr", "orderable": false, "visible": false }
        , { "data": "CloseoutDensityProductStandardInAirStr", "orderable": false, "visible": false }
        , { "data": "CloseoutLevelProductStr", "orderable": false, "visible": false }
        , { "data": "CloseoutLevelProductTimeStr", "orderable": false, "visible": false }
        , { "data": "CloseoutLevelWaterStr", "orderable": false, "visible": false }
        , { "data": "CloseoutMassLiquidStr", "orderable": false, "visible": false }
        , { "data": "CloseoutPercentBswStr", "orderable": false, "visible": false }
        , { "data": "CloseoutRoofMassStr", "orderable": false, "visible": false }
        , { "data": "CloseoutTankShellCorrectionStr", "orderable": false, "visible": false }
        , { "data": "CloseoutTemperatureAmbientStr", "orderable": false, "visible": false }
        , { "data": "CloseoutTemperatureAmbientTimeStr", "orderable": false, "visible": false }
        , { "data": "CloseoutTemperatureDensityStr", "orderable": false, "visible": false }
        , { "data": "CloseoutTemperatureProductStr", "orderable": false, "visible": false }
        , { "data": "CloseoutTimeStr", "orderable": false, "visible": false }
        , { "data": "CloseoutTransferGovStr", "orderable": false, "visible": false }
        , { "data": "CloseoutTransferNsvStr", "orderable": false, "visible": false }
        , { "data": "CloseoutTransferMassLiquidStr", "orderable": false, "visible": false }
        , { "data": "CloseoutTransferVolumeWaterStr", "orderable": false, "visible": false }
        , { "data": "CloseoutVolumeBswStr", "orderable": false, "visible": false }
        , { "data": "CloseoutVolumeCorrectionFactorStr", "orderable": false, "visible": false }
        , { "data": "CloseoutVolumeGrossObservedStr", "orderable": false, "visible": false }
        , { "data": "CloseoutVolumeGrossStandardStr", "orderable": false, "visible": false }
        , { "data": "CloseoutVolumeNetStandardStr", "orderable": false, "visible": false }
        , { "data": "CloseoutVolumeRoofCorrectionStr", "orderable": false, "visible": false }
        , { "data": "CloseoutVolumeTotalObservedStr", "orderable": false, "visible": false }
        , { "data": "CloseoutVolumeWaterStr", "orderable": false, "visible": false }
        , { "data": "LevelProductStr", "orderable": false, "visible": false }
        , { "data": "Type", "orderable": false, "visible": false }
        , { "data": "OrderNumber", "orderable": false, "visible": false }
        , { "data": "PlannedStartTimeStr", "orderable": false, "visible": false }
        , { "data": "Product", "orderable": false, "visible": false }
        , { "data": "ProductDescription", "orderable": false, "visible": false }
        , { "data": "StartTimeStr", "orderable": false, "visible": false }
        , { "data": "StartDensityProductObservedStr", "orderable": false, "visible": false }
        , { "data": "StartDensityProductObservedTimeStr", "orderable": false, "visible": false }
        , { "data": "StartDensityProductObservedInAirStr", "orderable": false, "visible": false }
        , { "data": "StartDensityProductStandardStr", "orderable": false, "visible": false }
        , { "data": "StartDensityProductStandardTimeStr", "orderable": false, "visible": false }
        , { "data": "StartDensityProductStandardInAirStr", "orderable": false, "visible": false }
        , { "data": "StartUserID", "orderable": false, "visible": false }
        , { "data": "StartLevelProductStr", "orderable": false, "visible": false }
        , { "data": "StartLevelProductTimeStr", "orderable": false, "visible": false }
        , { "data": "StartLevelWaterStr", "orderable": false, "visible": false }
        , { "data": "StartLevelWaterTimeStr", "orderable": false, "visible": false }
        , { "data": "StartMassLiquidStr", "orderable": false, "visible": false }
        , { "data": "StartPercentBswStr", "orderable": false, "visible": false }
        , { "data": "StartTankShellCorrectionStr", "orderable": false, "visible": false }
        , { "data": "StartTemperatureAmbientStr", "orderable": false, "visible": false }
        , { "data": "StartTemperatureAmbientTimeStr", "orderable": false, "visible": false }
        , { "data": "StartTemperatureProductStr", "orderable": false, "visible": false }
        , { "data": "StartTemperatureProductTimeStr", "orderable": false, "visible": false }
        , { "data": "StartTemperatureDensityStr", "orderable": false, "visible": false }
        , { "data": "StartTemperatureDensityTimeStr", "orderable": false, "visible": false }
        , { "data": "StartVolumeStr", "orderable": false, "visible": false }
        , { "data": "StartVolumeBswStr", "orderable": false, "visible": false }
        , { "data": "StartVolumeCorrectionFactorStr", "orderable": false, "visible": false }
        , { "data": "StartVolumeGrossObservedStr", "orderable": false, "visible": false }
        , { "data": "StartVolumeGrossStandardStr", "orderable": false, "visible": false }
        , { "data": "StartVolumeNetStandardStr", "orderable": false, "visible": false }
        , { "data": "StartVolumeRoofCorrectionStr", "orderable": false, "visible": false }
        , { "data": "StartVolumeTotalObservedStr", "orderable": false, "visible": false }
        , { "data": "StartVolumeWaterStr", "orderable": false, "visible": false }
        , { "data": "StopTimeStr", "orderable": false, "visible": false }
        , { "data": "StatusStr", "orderable": false, "visible": false }
        , { "data": "TransferDeviationStr", "orderable": false, "visible": false }
        , { "data": "TransferPercentDeviationStr", "orderable": false, "visible": false }
        , { "data": "TransferDirection", "orderable": false, "visible": false }
        , { "data": "TransferModeStr", "orderable": false, "visible": false }
        , { "data": "TransferStatusStr", "orderable": false, "visible": false }
        , { "data": "TransferTargetStr", "orderable": false, "visible": false }
		  , { "data": "TransferTargetUnits", "orderable": false, "visible": false }
		  , { "data": "TransferLevelTargetStr", "orderable": false, "visible": false }
		  , { "data": "TransferVolumeTargetStr", "orderable": false, "visible": false }
        , { "data": "TransferTimeRemainingStr", "orderable": false, "visible": false }
        , { "data": "TransferredVolumeWaterStr", "orderable": false, "visible": false }
        , { "data": "TransferredVolumeStr", "orderable": false, "visible": false }
        , { "data": "UnitsLevelProduct", "orderable": false, "visible": false }
        , { "data": "UnitsTemperatureAmbient", "orderable": false, "visible": false }
        , { "data": "UnitsTemperatureDensity", "orderable": false, "visible": false }
        , { "data": "UnitsTemperatureProduct", "orderable": false, "visible": false }
        , { "data": "UnitsDensityProductObserved", "orderable": false, "visible": false }
        , { "data": "UnitsDensityProductStandard", "orderable": false, "visible": false }
        , { "data": "UnitsVolume", "orderable": false, "visible": false }
        , { "data": "UnitsMass", "orderable": false, "visible": false }
        , { "data": "UserData01", "orderable": false, "visible": false }
        , { "data": "UserData02", "orderable": false, "visible": false }
        , { "data": "UserData03", "orderable": false, "visible": false }
        , { "data": "UserData04", "orderable": false, "visible": false }
        , { "data": "UserData05", "orderable": false, "visible": false }
        , { "data": "UserData06", "orderable": false, "visible": false }
        , { "data": "UserData07", "orderable": false, "visible": false }
        , { "data": "UserData08", "orderable": false, "visible": false }
        , { "data": "UserData09", "orderable": false, "visible": false }
        , { "data": "UserData10", "orderable": false, "visible": false }
        , { "data": "VolumeWaterStr", "orderable": false, "visible": false }
        , { "data": "CommentUserName", "orderable": false, "visible": false }
        , { "data": "CommentDateTimeStr", "orderable": false, "visible": false }
        , { "data": "RecordType", "orderable": false, "visible": false }
        , { "data": "MidnightRecord", "orderable": false, "visible": false }
        , { "data": "ParentGuid", "orderable": false, "visible": false }
        , { "data": "MovementHistoryGuid", "orderable": false, "visible": false }
        , { "data": "PointGuid", "orderable": false, "visible": false }
        , { "data": "RootParentGuid", "orderable": false, "visible": false }
        , { "data": "RecordSeq", "orderable": false, "visible": false }
    ];

    // NOTE:  If the initial physical order of the columns change, the column indexes 
    // must be updated below.
    var columnIndex = 0
    MovementHistoryTab.timeStampColumnIndex             = columnIndex++;
    MovementHistoryTab.movementNameColumnIndex          = columnIndex++;
    MovementHistoryTab.movementNodeColumnIndex          = columnIndex++;
    MovementHistoryTab.initiationCountColumnIndex       = columnIndex++;
    MovementHistoryTab.siteColumnIndex                  = columnIndex++;
    MovementHistoryTab.commentColumnIndex               = columnIndex++;

    MovementHistoryTab.closeoutDataModifiedByColumnIndex                = columnIndex++;
    MovementHistoryTab.closeoutDensityProductInAirColumnIndex           = columnIndex++;
    MovementHistoryTab.closeoutDensityProductObservedColumnIndex        = columnIndex++;
    MovementHistoryTab.closeoutDensityProductObservedTimeColumnIndex    = columnIndex++;
    MovementHistoryTab.closeoutDensityProductStandardColumnIndex        = columnIndex++;
    MovementHistoryTab.closeoutDensityProductStandardTimeColumnIndex    = columnIndex++;
    MovementHistoryTab.closeoutDensityProductStandardInAirColumnIndex   = columnIndex++;
    MovementHistoryTab.closeoutLevelProductColumnIndex                  = columnIndex++;
    MovementHistoryTab.closeoutLevelProductTimeColumnIndex              = columnIndex++;
    MovementHistoryTab.closeoutLevelWaterColumnIndex                    = columnIndex++;
    MovementHistoryTab.closeoutMassLiquidColumnIndex                    = columnIndex++;
    MovementHistoryTab.closeoutPercentBswColumnIndex                    = columnIndex++;
    MovementHistoryTab.closeoutRoofMassColumnIndex                      = columnIndex++;
    MovementHistoryTab.closeoutTankShellCorrectionColumnIndex           = columnIndex++;
    MovementHistoryTab.closeoutTemperatureAmbientColumnIndex            = columnIndex++;
    MovementHistoryTab.closeoutTemperatureAmbientTimeColumnIndex        = columnIndex++;
    MovementHistoryTab.closeoutTemperatureDensityColumnIndex            = columnIndex++;
    MovementHistoryTab.closeoutTemperatureProductColumnIndex            = columnIndex++;
    MovementHistoryTab.closeoutTimeColumnIndex                          = columnIndex++;
    MovementHistoryTab.closeoutTransferGovColumnIndex                   = columnIndex++;
    MovementHistoryTab.closeoutTransferNsvColumnIndex                   = columnIndex++;
    MovementHistoryTab.closeoutTransferMassLiquidColumnIndex            = columnIndex++;
    MovementHistoryTab.closeoutTransferVolumeWaterColumnIndex           = columnIndex++;
    MovementHistoryTab.closeoutVolumeBswColumnIndex                     = columnIndex++;
    MovementHistoryTab.closeoutVolumeCorrectionFactorColumnIndex        = columnIndex++;
    MovementHistoryTab.closeoutVolumeGrossObservedColumnIndex           = columnIndex++;
    MovementHistoryTab.closeoutVolumeGrossStandardColumnIndex           = columnIndex++;
    MovementHistoryTab.closeoutVolumeNetStandardColumnIndex             = columnIndex++;
    MovementHistoryTab.closeoutVolumeRoofCorrectionColumnIndex          = columnIndex++;
    MovementHistoryTab.closeoutVolumeTotalObservedColumnIndex           = columnIndex++;
    MovementHistoryTab.closeoutVolumeWaterColumnIndex                   = columnIndex++;

    MovementHistoryTab.levelProductColumnIndex          = columnIndex++;
    MovementHistoryTab.typeColumnIndex							= columnIndex++;
    MovementHistoryTab.orderNumberColumnIndex           = columnIndex++;
    MovementHistoryTab.plannedStartTimeColumnIndex      = columnIndex++;
    MovementHistoryTab.productColumnIndex               = columnIndex++;
    MovementHistoryTab.productDescriptionColumnIndex    = columnIndex++;

    MovementHistoryTab.startTimeColumnIndex                         = columnIndex++;
    MovementHistoryTab.startDensityProductObservedColumnIndex       = columnIndex++;
    MovementHistoryTab.startDensityProductObservedTimeColumnIndex   = columnIndex++;
    MovementHistoryTab.startDensityProductObservedInAirColumnIndex  = columnIndex++;
    MovementHistoryTab.startDensityProductStandardColumnIndex       = columnIndex++;
    MovementHistoryTab.startDensityProductStandardTimeColumnIndex   = columnIndex++;
    MovementHistoryTab.startDensityProductStandardInAirColumnIndex  = columnIndex++;
    MovementHistoryTab.startUserIdColumnIndex                       = columnIndex++;
    MovementHistoryTab.startLevelProductColumnIndex                 = columnIndex++;
    MovementHistoryTab.startLevelProductTimeColumnIndex             = columnIndex++;
    MovementHistoryTab.startLevelWaterColumnIndex                   = columnIndex++;
    MovementHistoryTab.startLevelWaterTimeColumnIndex               = columnIndex++;
    MovementHistoryTab.startMassLiquidColumnIndex                   = columnIndex++;
    MovementHistoryTab.startPercentBswColumnIndex                   = columnIndex++;
    MovementHistoryTab.startTankShellCorrectionColumnIndex          = columnIndex++;
    MovementHistoryTab.startTemperatureAmbientColumnIndex           = columnIndex++;
    MovementHistoryTab.startTemperatureAmbientTimeColumnIndex       = columnIndex++;
    MovementHistoryTab.startTemperatureProductColumnIndex           = columnIndex++;
    MovementHistoryTab.startTemperatureProductTimeColumnIndex       = columnIndex++;
    MovementHistoryTab.startTemperatureDensityColumnIndex           = columnIndex++;
    MovementHistoryTab.startTemperatureDensityTimeColumnIndex       = columnIndex++;
    MovementHistoryTab.startVolumeColumnIndex                       = columnIndex++;
    MovementHistoryTab.startVolumeBswColumnIndex                    = columnIndex++;
    MovementHistoryTab.startVolumeCorrectionFactorColumnIndex       = columnIndex++;
    MovementHistoryTab.startVolumeGrossObservedColumnIndex          = columnIndex++;
    MovementHistoryTab.startVolumeGrossStandardColumnIndex          = columnIndex++;
    MovementHistoryTab.startVolumeNetStandardColumnIndex            = columnIndex++;
    MovementHistoryTab.startVolumeRoofCorrectionColumnIndex         = columnIndex++;
    MovementHistoryTab.startVolumeTotalObservedColumnIndex          = columnIndex++;
    MovementHistoryTab.startVolumeWaterColumnIndex                  = columnIndex++;
    MovementHistoryTab.stopTimeColumnIndex                          = columnIndex++;
    MovementHistoryTab.statusColumnIndex                            = columnIndex++;

    MovementHistoryTab.transferDeviationColumnIndex         = columnIndex++;
    MovementHistoryTab.transferPercentDeviationColumnIndex  = columnIndex++;
    MovementHistoryTab.transferDirectionColumnIndex         = columnIndex++;
    MovementHistoryTab.transferModeColumnIndex              = columnIndex++;
    MovementHistoryTab.transferStatusColumnIndex            = columnIndex++;
    MovementHistoryTab.transferTargetColumnIndex            = columnIndex++;
	 MovementHistoryTab.transferTargetUnitsColumnIndex			= columnIndex++;
	 MovementHistoryTab.transferLevelTargetColumnIndex			= columnIndex++;
	 MovementHistoryTab.transferVolumeTargetColumnIndex		= columnIndex++;
    MovementHistoryTab.transferTimeRemainingColumnIndex     = columnIndex++;
    MovementHistoryTab.transferredVolumeWaterColumnIndex    = columnIndex++;
    MovementHistoryTab.transferredVolumeColumnIndex         = columnIndex++;

    MovementHistoryTab.unitsLevelProductColumnIndex             = columnIndex++;
    MovementHistoryTab.unitsTemperatureAmbientColumnIndex       = columnIndex++;
    MovementHistoryTab.unitsTemperatureDensityColumnIndex       = columnIndex++;
    MovementHistoryTab.unitsTemperatureProductColumnIndex       = columnIndex++;
    MovementHistoryTab.unitsDensityProductObservedColumnIndex   = columnIndex++;
    MovementHistoryTab.unitsDensityProductStandardColumnIndex   = columnIndex++;
    MovementHistoryTab.unitsVolumeColumnIndex                   = columnIndex++;
    MovementHistoryTab.unitsMassColumnIndex                     = columnIndex++;

    MovementHistoryTab.userData01ColumnIndex = columnIndex++;
    MovementHistoryTab.userData02ColumnIndex = columnIndex++;
    MovementHistoryTab.userData03ColumnIndex = columnIndex++;
    MovementHistoryTab.userData04ColumnIndex = columnIndex++;
    MovementHistoryTab.userData05ColumnIndex = columnIndex++;
    MovementHistoryTab.userData06ColumnIndex = columnIndex++;
    MovementHistoryTab.userData07ColumnIndex = columnIndex++;
    MovementHistoryTab.userData08ColumnIndex = columnIndex++;
    MovementHistoryTab.userData09ColumnIndex = columnIndex++;
    MovementHistoryTab.userData10ColumnIndex = columnIndex++;

    MovementHistoryTab.volumeWaterColumnIndex = columnIndex++;

    MovementHistoryTab.commentUserNameColumnIndex = columnIndex++;
    MovementHistoryTab.commentDateTimeColumnIndex = columnIndex++;

    MovementHistoryTab.recordTypeColumnIndex            = columnIndex++;
    MovementHistoryTab.midnightRecordColumnIndex        = columnIndex++;
    MovementHistoryTab.parentGuidColumnIndex            = columnIndex++;
    MovementHistoryTab.movementHistoryGuidColumnIndex   = columnIndex++;
    MovementHistoryTab.pointGuidColumnIndex             = columnIndex++;
    MovementHistoryTab.rootParentGuidColumnIndex        = columnIndex++;
    MovementHistoryTab.recordSeqColumnIndex             = columnIndex;


    // Ignore the last seven column indexes since they are not visible
    // on the UI.
    MovementHistoryTab.totalColumns = columnIndex - 7;

    return cols;
}

//=============================================================
// This function will set the data retrieved into the movement
// model.
//=============================================================
MovementHistoryTab.SetPageDataInMovementHistoryModel = function (data)
{
	var json = jQuery.parseJSON(data);
	var model = MovementHistoryTab.GetMovementHistoryModel();

	model.MovementHistories = json.data;
	MovementHistoryTab.SetMovementHistoryModel(model);

	// On initial load request, we are retrieving data based on the Top x rows.
	// Therefore, we want to set the start and end dates to be from that dataset.
	if (MovementHistoryTab.initialLoadRequest && model.MovementHistories.length > 0)
	{
		var index = MovementHistoryTab.FindMaxMinDateIndex(model.MovementHistories);
		var startDateTime = model.MovementHistories[index].MinDateTimeStr;
		var endDateTime = model.MovementHistories[index].MaxDateTimeStr;

		$("#StartTimePicker").val(startDateTime);
		$("#EndTimePicker").val(endDateTime);

		// Update the Time Stamp TO/FROM dates
		$("#MovementHistoryColumnFilterFromDateInput").val(startDateTime);
		$("#MovementHistoryColumnFilterToDateInput").val(endDateTime);

		// Ensure that the date filter object is updated.
		MovementHistoryTab.HandleAvailableFilterDateChangeEvent("FROM");
		MovementHistoryTab.HandleAvailableFilterDateChangeEvent("TO");
	}

	// Persist the column filter collection settings for when the user makes changes
	// and cancels the changes.
	MovementHistoryTab.previousColumnFilterCollection = MovementHistoryTab.CopyColumnFilterInfo(MovementHistoryTab.columnFilterCollection);

	// Set initial load request to false. It should only happen once to get the data
	// by the show line count.
	MovementHistoryTab.initialLoadRequest = false;

	return data; // return JSON string
}

//======================================================================
// This function will have the index that the max and min dates are
// located.
//======================================================================
MovementHistoryTab.FindMaxMinDateIndex = function (movementHistories)
{
    for (var nextIndex = 0; nextIndex < movementHistories.length; nextIndex++)
    {
        var startDateTime = movementHistories[nextIndex].MinDateTimeStr;
        var endDateTime = movementHistories[nextIndex].MaxDateTimeStr;

        if (startDateTime != null && startDateTime !== "" && endDateTime != null && endDateTime !== "")
        {
            return nextIndex;
        }
    }

    return 0;
}

//===============================================================
// This function gets the order columns.
//===============================================================
MovementHistoryTab.OrderColumns = function ()
{
    var colOrder = [[0, "desc"]];
    return colOrder;
}

//==================================================================
// This function sets the TD tag ID.
//==================================================================
MovementHistoryTab.SetTdId = function (table, idStr, row, index)
{
    var newIndex = table.colReorder.transpose(index);
    var td = $(row).find('td:eq(' + newIndex + ')')[0];

    if (td)
    {
        td.id = idStr;
    }
}

//===========================================================================
// This function will get the date/time.
//===========================================================================
MovementHistoryTab.GetDateAndTime = function (movementHistoryRecordGuid)
{
    var model = MovementHistoryTab.GetMovementHistoryModel();
    var movementHistList = model.MovementHistories;

    for (var i = 0; i < movementHistList.length; i++)
    {
        var movementHistory = movementHistList[i];

        if (movementHistory.MovementHistoryGuid === movementHistoryRecordGuid)
        {
            return movementHistory.TimeStampStr;
        }
    }

    return null;
}

//======================================================================
// This function will get the comment.
//======================================================================
MovementHistoryTab.GetComment = function (movementHistoryRecordGuid)
{
    var model = MovementHistoryTab.GetMovementHistoryModel();
    var movementHistList = model.MovementHistories;

    for (var i = 0; i < movementHistList.length; i++)
    {
        var movementHistory = movementHistList[i];

        if (movementHistory.MovementHistoryGuid === movementHistoryRecordGuid)
        {
            return movementHistory.Comment;
        }
    }

    return null;
}

//======================================================================
// This function will update the comments in the model.
//======================================================================
MovementHistoryTab.UpdateCommentInfo = function (newComment, movementHistoryRecordGuid, commentTimestamp, commentUser)
{
    var model = MovementHistoryTab.GetMovementHistoryModel();
    var movementHistList = model.MovementHistories;

    for (var i = 0; i < movementHistList.length; i++)
    {
        var movementHistory = movementHistList[i];

        if (movementHistory.MovementHistoryGuid === movementHistoryRecordGuid)
        {
            movementHistory.Comment = newComment;
            movementHistory.CommentUserName = commentUser;
            movementHistory.CommentDateTimeStr = commentTimestamp;
            MovementHistoryTab.SetMovementHistoryModel(model);

            return;
        }
    }
}

//==============================================================================
// This function will save the comment to the database.
//==============================================================================
MovementHistoryTab.SaveComment = function (inputItem)
{
    var newComment = inputItem.value;
    var movementHistoryRecordGuid = inputItem.id.replace("EnterComments_", "");
    var cell = document.getElementById(MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentsColumnIndex) + movementHistoryRecordGuid);
    $(cell).find('label').text(newComment);

    MovementHistoryTab.inEditMode = false;

    var url = $("#MovementHistoryUpdateCommentUrl").val();
    var token = $('#MovementHistoryTabView input[name =__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    PNotify.removeStack(MovementHistoryTab.messageAttributes.stack);
    $.ajax({
        cache: false,
        type: "POST",
        //async: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: "json",
        url: url,
        headers: headers,
        data: JSON.stringify({
            movementHistoryRecordGuidStr: movementHistoryRecordGuid, comment: newComment
        }),
        success: function (data)
        {
            var commentUserName = data.Item1;
            var commentDateTime = data.Item2;

            MovementHistoryTab.UpdateCommentInfo(newComment, movementHistoryRecordGuid, commentDateTime, commentUserName);

            var cntrl = document.getElementById(MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentDateTimeColumnIndex) + movementHistoryRecordGuid);

            if (cntrl)
            {
                cntrl.innerHTML = commentDateTime;
            }

            cntrl = document.getElementById(MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentUserNameColumnIndex) + movementHistoryRecordGuid);

            if (cntrl)
            {
                cntrl.innerHTML = commentUserName;
            }

            // Take out of edit mode.
            MovementHistoryTab.RemoveCommentFromEditMode();
            MovementHistoryTab.AdjustColumns();
        },
        error: function (e)
        {
            MovementHistoryTab.CancelComment(inputItem);
            FMErrorAndExceptionHandling.ShowError('Error saving comment.', null, MovementHistoryTab.messageAttributes);
        }
    });
}

//======================================================================
// This function will cancel the comment entry.
//======================================================================
MovementHistoryTab.CancelComment = function (inputItem)
{
    var movementHistoryRecordGuid = inputItem.id.replace("EnterComments_", "");
    var cell = document.getElementById(MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentColumnIndex) + movementHistoryRecordGuid);

    if (cell)
    {
        $(cell).html(MovementHistoryTab.originalEditValue);
    }

    MovementHistoryTab.inEditMode = false;
}

//===============================================================
// This function will handle the comment edit key event.
//===============================================================
MovementHistoryTab.CommentEditKeyHandler = function (e)
{
    e = e || event;

    if ((e.keyCode || e.which || e.charCode || 0) === 13)
    {
        MovementHistoryTab.SaveComment(e.target);
        return false;
    }
    else if ((e.keyCode || e.which || e.charCode || 0) === 27)
    {
        // delete the onblur event so we don't save when cancelling
        $(e.target).removeAttr('onblur');
        MovementHistoryTab.CancelComment(e.target);
        return false;
    }

    return true;
}

//========================================================
// This function will adjust the columns.
//========================================================
MovementHistoryTab.AdjustColumns = function ()
{
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }
    var table = $("#MovementHistoryTable").DataTable();
    table.columns.adjust();
}

//====================================================================
// This function will create an editable input for entering in the
// comment.
//====================================================================
MovementHistoryTab.CreateEditableComment = function (rawComment, movementHistoryRecordGuid, movementHistoryTimeStampTicks)
{
    return "<Span><input movementHistoryTimestampTicks=\"" + movementHistoryTimeStampTicks + "\" id=\"EnterComments_" + movementHistoryRecordGuid
        + "\" type=\"text\" value=\"" + rawComment
        + "\" class=\"MovementHistoryShowCommentIsEditable\" autocomplete=\"off\" onkeypress=\"javascript: return MovementHistoryTab.CommentEditKeyHandler();\" onblur=\"javascript: return MovementHistoryTab.SaveComment( this );\" /></Span>";
}

//=======================================================================
// This function will add an ID to a TD tag.
//=======================================================================
MovementHistoryTab.AddIdToTd = function (row, data, dataIndex)
{
    var movementHistoryRecordGuid = row.id.replace('Row_', '');

    var timestampId                             = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.timeStampColumnIndex) + movementHistoryRecordGuid;
    var movementNameId                          = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.movementNameColumnIndex) + movementHistoryRecordGuid;
    var movementNodeId                          = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.movementNodeColumnIndex) + movementHistoryRecordGuid;
    var initiationCountId                       = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.initiationCountColumnIndex) + movementHistoryRecordGuid;
    var siteId                                  = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.siteColumnIndex) + movementHistoryRecordGuid;
    var commentId                               = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentColumnIndex) + movementHistoryRecordGuid;
    var closeoutDataModifiedById                = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDataModifiedByColumnIndex) + movementHistoryRecordGuid;
    var closeoutDensityProductInAirId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductInAirColumnIndex) + movementHistoryRecordGuid;
    var closeoutDensityProductObservedId        = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductObservedColumnIndex) + movementHistoryRecordGuid;
    var closeoutDensityProductObservedTimeId    = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductObservedTimeColumnIndex) + movementHistoryRecordGuid;
    var closeoutDensityProductStandardId        = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductStandardColumnIndex) + movementHistoryRecordGuid;
    var closeoutDensityProductStandardTimeId    = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductStandardTimeColumnIndex) + movementHistoryRecordGuid;
    var closeoutDensityProductStandardInAirId   = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductStandardInAirColumnIndex) + movementHistoryRecordGuid;
    var closeoutLevelProductId                  = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutLevelProductColumnIndex) + movementHistoryRecordGuid;
    var closeoutLevelProductTimeId              = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutLevelProductTimeColumnIndex) + movementHistoryRecordGuid;
    var closeoutLevelWaterId                    = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutLevelWaterColumnIndex) + movementHistoryRecordGuid;
    var closeoutMassLiquidId                    = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutMassLiquidColumnIndex) + movementHistoryRecordGuid;
    var closeoutPercentBswId                    = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutPercentBswColumnIndex) + movementHistoryRecordGuid;
    var closeoutRoofMassId                      = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutRoofMassColumnIndex) + movementHistoryRecordGuid;
    var closeoutTankShellCorrectionId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTankShellCorrectionColumnIndex) + movementHistoryRecordGuid;
    var closeoutTemperatureAmbientId            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTemperatureAmbientColumnIndex) + movementHistoryRecordGuid;
    var closeoutTemperatureAmbientTimeId        = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTemperatureAmbientTimeColumnIndex) + movementHistoryRecordGuid;
    var closeoutTemperatureDensityId            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTemperatureDensityColumnIndex) + movementHistoryRecordGuid;
    var closeoutTemperatureProductId            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTemperatureProductColumnIndex) + movementHistoryRecordGuid;
    var closeoutTimeId                          = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTimeColumnIndex) + movementHistoryRecordGuid;
    var closeoutTransferGovId                   = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTransferGovColumnIndex) + movementHistoryRecordGuid;
    var closeoutTransferNsvId                   = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTransferNsvColumnIndex) + movementHistoryRecordGuid;
    var closeoutTransferMassLiquidId            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTransferMassLiquidColumnIndex) + movementHistoryRecordGuid;
    var closeoutTransferVolumeWaterId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTransferVolumeWaterColumnIndex) + movementHistoryRecordGuid;
    var closeoutVolumeBswId                     = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeBswColumnIndex) + movementHistoryRecordGuid;
    var closeoutVolumeCorrectionFactorId        = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeCorrectionFactorColumnIndex) + movementHistoryRecordGuid;
    var closeoutVolumeGrossObservedId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeGrossObservedColumnIndex) + movementHistoryRecordGuid;
    var closeoutVolumeGrossStandardId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeGrossStandardColumnIndex) + movementHistoryRecordGuid;
    var closeoutVolumeNetStandardId             = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeNetStandardColumnIndex) + movementHistoryRecordGuid;
    var closeoutVolumeRoofCorrectionId          = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeRoofCorrectionColumnIndex) + movementHistoryRecordGuid;
    var closeoutVolumeTotalObservedId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeTotalObservedColumnIndex) + movementHistoryRecordGuid;
    var closeoutVolumeWaterId                   = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeWaterColumnIndex) + movementHistoryRecordGuid;
    var levelProductId                          = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.levelProductColumnIndex) + movementHistoryRecordGuid;
    var typeId												= MovementHistoryTab.GetIdPrefix(MovementHistoryTab.typeColumnIndex) + movementHistoryRecordGuid;
    var orderNumberId                           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.orderNumberColumnIndex) + movementHistoryRecordGuid;
    var plannedStartTimeId                      = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.plannedStartTimeColumnIndex) + movementHistoryRecordGuid;
    var productId                               = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.productColumnIndex) + movementHistoryRecordGuid;
    var productDescriptionId                    = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.productDescriptionColumnIndex) + movementHistoryRecordGuid;
    var startTimeId                             = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTimeColumnIndex) + movementHistoryRecordGuid;
    var startDensityProductObservedId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductObservedColumnIndex) + movementHistoryRecordGuid;
    var startDensityProductObservedTimeId       = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductObservedTimeColumnIndex) + movementHistoryRecordGuid;
    var startDensityProductObservedInAirId      = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductObservedInAirColumnIndex) + movementHistoryRecordGuid;
    var startDensityProductStandardId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductStandardColumnIndex) + movementHistoryRecordGuid;
    var startDensityProductStandardTimeId       = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductStandardTimeColumnIndex) + movementHistoryRecordGuid;
    var startDensityProductStandardInAirId      = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductStandardInAirColumnIndex) + movementHistoryRecordGuid;
    var startUserId                             = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startUserIdColumnIndex) + movementHistoryRecordGuid;
    var startLevelProductId                     = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startLevelProductColumnIndex) + movementHistoryRecordGuid;
    var startLevelProductTimeId                 = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startLevelProductTimeColumnIndex) + movementHistoryRecordGuid;
    var startLevelWaterId                       = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startLevelWaterColumnIndex) + movementHistoryRecordGuid;
    var startLevelWaterTimeId                   = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startLevelWaterTimeColumnIndex) + movementHistoryRecordGuid;
    var startMassLiquidId                       = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startMassLiquidColumnIndex) + movementHistoryRecordGuid;
    var startPercentBswId                       = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startPercentBswColumnIndex) + movementHistoryRecordGuid;
    var startTankShellCorrectionId              = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTankShellCorrectionColumnIndex) + movementHistoryRecordGuid;
    var startTemperatureAmbientId               = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureAmbientColumnIndex) + movementHistoryRecordGuid;
    var startTemperatureAmbientTimeId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureAmbientTimeColumnIndex) + movementHistoryRecordGuid;
    var startTemperatureProductId               = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureProductColumnIndex) + movementHistoryRecordGuid;
    var startTemperatureProductTimeId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureProductTimeColumnIndex) + movementHistoryRecordGuid;
    var startTemperatureDensityId               = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureDensityColumnIndex) + movementHistoryRecordGuid;
    var startTemperatureDensityTimeId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureDensityTimeColumnIndex) + movementHistoryRecordGuid;
    var startVolumeId                           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeColumnIndex) + movementHistoryRecordGuid;
    var startVolumeBswId                        = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeColumnBswIndex) + movementHistoryRecordGuid;
    var startVolumeCorrectionFactorId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeCorrectionFactorColumnIndex) + movementHistoryRecordGuid;
    var startVolumeGrossObservedId              = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeGrossObservedColumnIndex) + movementHistoryRecordGuid;
    var startVolumeGrossStandardId              = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeGrossStandardColumnIndex) + movementHistoryRecordGuid;
    var startVolumeNetStandardId                = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeNetStandardColumnIndex) + movementHistoryRecordGuid;
    var startVolumeRoofCorrectionId             = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeRoofCorrectionColumnIndex) + movementHistoryRecordGuid;
    var startVolumeTotalObservedId              = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeTotalObservedColumnIndex) + movementHistoryRecordGuid;
    var startVolumeWaterId                      = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeWaterColumnIndex) + movementHistoryRecordGuid;
    var stopTimeId                              = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.stopTimeColumnIndex) + movementHistoryRecordGuid;
    var statusId                                = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.statusColumnIndex) + movementHistoryRecordGuid;
    var transferDeviationId                     = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferDeviationColumnIndex) + movementHistoryRecordGuid;
    var transferPercentDeviationId              = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferPercentDeviationColumnIndex) + movementHistoryRecordGuid;
    var transferDirectionId                     = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferDirectionColumnIndex) + movementHistoryRecordGuid;
    var transferModeId                          = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferModeColumnIndex) + movementHistoryRecordGuid;
    var transferStatusId                        = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferStatusColumnIndex) + movementHistoryRecordGuid;
    var transferTargetId                        = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferTargetColumnIndex) + movementHistoryRecordGuid;
 	 var transferTargetUnitsId							= MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferTargetUnitsColumnIndex) + movementHistoryRecordGuid;
	 var transferLevelTargetId							= MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferLevelTargetColumnIndex) + movementHistoryRecordGuid;
	 var transferVolumeTargetId						= MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferVolumeTargetColumnIndex) + movementHistoryRecordGuid;
    var transferTimeRemainingId                 = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferTimeRemainingColumnIndex) + movementHistoryRecordGuid;
    var transferredVolumeWaterId                = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferredVolumeWaterColumnIndex) + movementHistoryRecordGuid;
    var transferredVolumeId                     = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferredVolumeColumnIndex) + movementHistoryRecordGuid;
    var unitsLevelProductId                     = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsLevelProductColumnIndex) + movementHistoryRecordGuid;
    var unitsTemperatureAmbientId               = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsTemperatureAmbientColumnIndex) + movementHistoryRecordGuid;
    var unitsTemperatureDensityId               = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsTemperatureDensityColumnIndex) + movementHistoryRecordGuid;
    var unitsTemperatureProductId               = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsTemperatureProductColumnIndex) + movementHistoryRecordGuid;
    var unitsDensityProductObservedId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsDensityProductObservedColumnIndex) + movementHistoryRecordGuid;
    var unitsDensityProductStandardId           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsDensityProductStandardColumnIndex) + movementHistoryRecordGuid;
    var unitsVolumeId                           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsVolumeColumnIndex) + movementHistoryRecordGuid;
    var unitsMassId                             = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsMassColumnIndex) + movementHistoryRecordGuid;
    var userData01Id                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData01ColumnIndex) + movementHistoryRecordGuid;
    var userData02Id                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData02ColumnIndex) + movementHistoryRecordGuid;
    var userData03Id                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData03ColumnIndex) + movementHistoryRecordGuid;
    var userData04Id                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData04ColumnIndex) + movementHistoryRecordGuid;
    var userData05Id                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData05ColumnIndex) + movementHistoryRecordGuid;
    var userData06Id                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData06ColumnIndex) + movementHistoryRecordGuid;
    var userData07Id                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData07ColumnIndex) + movementHistoryRecordGuid;
    var userData08Id                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData08ColumnIndex) + movementHistoryRecordGuid;
    var userData09Id                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData09ColumnIndex) + movementHistoryRecordGuid;
    var userData10Id                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData10ColumnIndex) + movementHistoryRecordGuid;
    var volumeWaterId                           = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.volumeWaterColumnIndex) + movementHistoryRecordGuid;
    var commentUserNameId                       = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentUserNameColumnIndex) + movementHistoryRecordGuid;
    var commentDateTimeId                       = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentDateTimeColumnIndex) + movementHistoryRecordGuid;
    var recordTypeId                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.recordTypeColumnIndex) + movementHistoryRecordGuid;
    var parentGuidId                            = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.parentGuidColumnIndex) + movementHistoryRecordGuid;
    var movementHistoryId                       = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.movementHistoryGuidColumnIndex) + movementHistoryRecordGuid;
    var pointGuidId                             = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.pointGuidColumnIndex) + movementHistoryRecordGuid;
    var rootParentGuidId                        = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.rootParentGuidColumnIndex) + movementHistoryRecordGuid;
    var recordSeqId                             = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.recordSeqColumnIndex) + movementHistoryRecordGuid;
    var midnightRecordId                        = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.midnightRecordColumnIndex) + movementHistoryRecordGuid;

   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }

    var table = $("#MovementHistoryTable").DataTable();

    MovementHistoryTab.SetTdId(table, timestampId, row, MovementHistoryTab.timeStampColumnIndex);
    MovementHistoryTab.SetTdId(table, movementNameId, row, MovementHistoryTab.movementNameColumnIndex);
    MovementHistoryTab.SetTdId(table, movementNodeId, row, MovementHistoryTab.movementNodeColumnIndex);
    MovementHistoryTab.SetTdId(table, initiationCountId, row, MovementHistoryTab.initiationCountColumnIndex);
    MovementHistoryTab.SetTdId(table, siteId, row, MovementHistoryTab.siteColumnIndex);
    MovementHistoryTab.SetTdId(table, commentId, row, MovementHistoryTab.commentColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutDataModifiedById, row, MovementHistoryTab.closeoutDataModifiedByColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutDensityProductInAirId, row, MovementHistoryTab.closeoutDensityProductInAirColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutDensityProductObservedId, row, MovementHistoryTab.closeoutDensityProductObservedColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutDensityProductObservedTimeId, row, MovementHistoryTab.closeoutDensityProductObservedTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutDensityProductStandardId, row, MovementHistoryTab.closeoutDensityProductStandardColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutDensityProductStandardTimeId, row, MovementHistoryTab.closeoutDensityProductStandardTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutDensityProductStandardInAirId, row, MovementHistoryTab.closeoutDensityProductStandardInAirColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutLevelProductId, row, MovementHistoryTab.closeoutLevelProductColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutLevelProductTimeId, row, MovementHistoryTab.closeoutLevelProductTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutLevelWaterId, row, MovementHistoryTab.closeoutLevelWaterColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutMassLiquidId, row, MovementHistoryTab.closeoutMassLiquidColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutPercentBswId, row, MovementHistoryTab.closeoutPercentBswColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutRoofMassId, row, MovementHistoryTab.closeoutRoofMassColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutTankShellCorrectionId, row, MovementHistoryTab.closeoutTankShellCorrectionColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutTemperatureAmbientId, row, MovementHistoryTab.closeoutTemperatureAmbientColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutTemperatureAmbientTimeId, row, MovementHistoryTab.closeoutTemperatureAmbientTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutTemperatureDensityId, row, MovementHistoryTab.closeoutTemperatureDensityColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutTemperatureProductId, row, MovementHistoryTab.closeoutTemperatureProductColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutTimeId, row, MovementHistoryTab.closeoutTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutTransferGovId, row, MovementHistoryTab.closeoutTransferGovColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutTransferNsvId, row, MovementHistoryTab.closeoutTransferNsvColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutTransferMassLiquidId, row, MovementHistoryTab.closeoutTransferMassLiquidColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutTransferVolumeWaterId, row, MovementHistoryTab.closeoutTransferVolumeWaterColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutVolumeBswId, row, MovementHistoryTab.closeoutVolumeBswColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutVolumeCorrectionFactorId, row, MovementHistoryTab.closeoutVolumeCorrectionFactorColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutVolumeGrossObservedId, row, MovementHistoryTab.closeoutVolumeGrossObservedColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutVolumeGrossStandardId, row, MovementHistoryTab.closeoutVolumeGrossStandardColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutVolumeNetStandardId, row, MovementHistoryTab.closeoutVolumeNetStandardColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutVolumeRoofCorrectionId, row, MovementHistoryTab.closeoutVolumeRoofCorrectionColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutVolumeTotalObservedId, row, MovementHistoryTab.closeoutVolumeTotalObservedColumnIndex);
    MovementHistoryTab.SetTdId(table, closeoutVolumeWaterId, row, MovementHistoryTab.closeoutVolumeWaterColumnIndex);
    MovementHistoryTab.SetTdId(table, levelProductId, row, MovementHistoryTab.levelProductColumnIndex);
    MovementHistoryTab.SetTdId(table, typeId, row, MovementHistoryTab.typeColumnIndex);
    MovementHistoryTab.SetTdId(table, orderNumberId, row, MovementHistoryTab.orderNumberColumnIndex);
    MovementHistoryTab.SetTdId(table, plannedStartTimeId, row, MovementHistoryTab.plannedStartTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, productId, row, MovementHistoryTab.productColumnIndex);
    MovementHistoryTab.SetTdId(table, productDescriptionId, row, MovementHistoryTab.productDescriptionColumnIndex);
    MovementHistoryTab.SetTdId(table, startTimeId, row, MovementHistoryTab.startTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, startDensityProductObservedId, row, MovementHistoryTab.startDensityProductObservedColumnIndex);
    MovementHistoryTab.SetTdId(table, startDensityProductObservedTimeId, row, MovementHistoryTab.startDensityProductObservedTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, startDensityProductObservedInAirId, row, MovementHistoryTab.startDensityProductObservedInAirColumnIndex);
    MovementHistoryTab.SetTdId(table, startDensityProductStandardId, row, MovementHistoryTab.startDensityProductStandardColumnIndex);
    MovementHistoryTab.SetTdId(table, startDensityProductStandardTimeId, row, MovementHistoryTab.startDensityProductStandardTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, startDensityProductStandardInAirId, row, MovementHistoryTab.startDensityProductStandardInAirColumnIndex);
    MovementHistoryTab.SetTdId(table, startUserId, row, MovementHistoryTab.startUserIdColumnIndex);
    MovementHistoryTab.SetTdId(table, startLevelProductId, row, MovementHistoryTab.startLevelProductColumnIndex);
    MovementHistoryTab.SetTdId(table, startLevelProductTimeId, row, MovementHistoryTab.startLevelProductTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, startLevelWaterId, row, MovementHistoryTab.startLevelWaterColumnIndex);
    MovementHistoryTab.SetTdId(table, startLevelWaterTimeId, row, MovementHistoryTab.startLevelWaterTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, startMassLiquidId, row, MovementHistoryTab.startMassLiquidColumnIndex);
    MovementHistoryTab.SetTdId(table, startPercentBswId, row, MovementHistoryTab.startPercentBswColumnIndex);
    MovementHistoryTab.SetTdId(table, startTankShellCorrectionId, row, MovementHistoryTab.startTankShellCorrectionColumnIndex);
    MovementHistoryTab.SetTdId(table, startTemperatureAmbientId, row, MovementHistoryTab.startTemperatureAmbientColumnIndex);
    MovementHistoryTab.SetTdId(table, startTemperatureAmbientTimeId, row, MovementHistoryTab.startTemperatureAmbientTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, startTemperatureProductId, row, MovementHistoryTab.startTemperatureProductColumnIndex);
    MovementHistoryTab.SetTdId(table, startTemperatureProductTimeId, row, MovementHistoryTab.startTemperatureProductTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, startTemperatureDensityId, row, MovementHistoryTab.startTemperatureDensityColumnIndex);
    MovementHistoryTab.SetTdId(table, startTemperatureDensityTimeId, row, MovementHistoryTab.startTemperatureDensityTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, startVolumeId, row, MovementHistoryTab.startVolumeColumnIndex);
    MovementHistoryTab.SetTdId(table, startVolumeBswId, row, MovementHistoryTab.startVolumeBswColumnIndex);
    MovementHistoryTab.SetTdId(table, startVolumeCorrectionFactorId, row, MovementHistoryTab.startVolumeCorrectionFactorColumnIndex);
    MovementHistoryTab.SetTdId(table, startVolumeGrossObservedId, row, MovementHistoryTab.startVolumeGrossObservedColumnIndex);
    MovementHistoryTab.SetTdId(table, startVolumeGrossStandardId, row, MovementHistoryTab.startVolumeGrossStandardColumnIndex);
    MovementHistoryTab.SetTdId(table, startVolumeNetStandardId, row, MovementHistoryTab.startVolumeNetStandardColumnIndex);
    MovementHistoryTab.SetTdId(table, startVolumeRoofCorrectionId, row, MovementHistoryTab.startVolumeRoofCorrectionColumnIndex);
    MovementHistoryTab.SetTdId(table, startVolumeTotalObservedId, row, MovementHistoryTab.startVolumeTotalObservedColumnIndex);
    MovementHistoryTab.SetTdId(table, startVolumeWaterId, row, MovementHistoryTab.startVolumeWaterColumnIndex);
    MovementHistoryTab.SetTdId(table, stopTimeId, row, MovementHistoryTab.stopTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, statusId, row, MovementHistoryTab.statusColumnIndex);
    MovementHistoryTab.SetTdId(table, transferDeviationId, row, MovementHistoryTab.transferDeviationColumnIndex);
    MovementHistoryTab.SetTdId(table, transferPercentDeviationId, row, MovementHistoryTab.transferPercentDeviationColumnIndex);
    MovementHistoryTab.SetTdId(table, transferDirectionId, row, MovementHistoryTab.transferDirectionColumnIndex);
    MovementHistoryTab.SetTdId(table, transferModeId, row, MovementHistoryTab.transferModeColumnIndex);
    MovementHistoryTab.SetTdId(table, transferStatusId, row, MovementHistoryTab.transferStatusColumnIndex);
    MovementHistoryTab.SetTdId(table, transferTargetId, row, MovementHistoryTab.transferTargetColumnIndex);
    MovementHistoryTab.SetTdId(table, transferTargetUnitsId, row, MovementHistoryTab.transferTargetUnitsColumnIndex);
	 MovementHistoryTab.SetTdId(table, transferLevelTargetId, row, MovementHistoryTab.transferLevelTargetColumnIndex);
	 MovementHistoryTab.SetTdId(table, transferVolumeTargetId, row, MovementHistoryTab.transferVolumeTargetColumnIndex);
    MovementHistoryTab.SetTdId(table, transferTimeRemainingId, row, MovementHistoryTab.transferTimeRemainingColumnIndex);
    MovementHistoryTab.SetTdId(table, transferredVolumeWaterId, MovementHistoryTab.transferredVolumeWaterColumnIndex);
    MovementHistoryTab.SetTdId(table, transferredVolumeId, MovementHistoryTab.transferredVolumeColumnIndex);
    MovementHistoryTab.SetTdId(table, unitsLevelProductId, row, MovementHistoryTab.unitsLevelProductColumnIndex);
    MovementHistoryTab.SetTdId(table, unitsTemperatureAmbientId, row, MovementHistoryTab.unitsTemperatureAmbientColumnIndex);
    MovementHistoryTab.SetTdId(table, unitsTemperatureDensityId, row, MovementHistoryTab.unitsTemperatureDensityColumnIndex);
    MovementHistoryTab.SetTdId(table, unitsTemperatureProductId, row, MovementHistoryTab.unitsTemperatureProductColumnIndex);
    MovementHistoryTab.SetTdId(table, unitsDensityProductObservedId, row, MovementHistoryTab.unitsDensityProductObservedColumnIndex);
    MovementHistoryTab.SetTdId(table, unitsDensityProductStandardId, row, MovementHistoryTab.unitsDensityProductStandardColumnIndex);
    MovementHistoryTab.SetTdId(table, unitsVolumeId, row, MovementHistoryTab.unitsVolumeColumnIndex);
    MovementHistoryTab.SetTdId(table, unitsMassId, row, MovementHistoryTab.unitsMassColumnIndex);
    MovementHistoryTab.SetTdId(table, userData01Id, row, MovementHistoryTab.userData01ColumnIndex);
    MovementHistoryTab.SetTdId(table, userData02Id, row, MovementHistoryTab.userData02ColumnIndex);
    MovementHistoryTab.SetTdId(table, userData03Id, row, MovementHistoryTab.userData03ColumnIndex);
    MovementHistoryTab.SetTdId(table, userData04Id, row, MovementHistoryTab.userData04ColumnIndex);
    MovementHistoryTab.SetTdId(table, userData05Id, row, MovementHistoryTab.userData05ColumnIndex);
    MovementHistoryTab.SetTdId(table, userData06Id, row, MovementHistoryTab.userData06ColumnIndex);
    MovementHistoryTab.SetTdId(table, userData07Id, row, MovementHistoryTab.userData07ColumnIndex);
    MovementHistoryTab.SetTdId(table, userData08Id, row, MovementHistoryTab.userData08ColumnIndex);
    MovementHistoryTab.SetTdId(table, userData09Id, row, MovementHistoryTab.userData09ColumnIndex);
    MovementHistoryTab.SetTdId(table, userData10Id, row, MovementHistoryTab.userData10ColumnIndex);
    MovementHistoryTab.SetTdId(table, volumeWaterId, row, MovementHistoryTab.volumeWaterColumnIndex);
    MovementHistoryTab.SetTdId(table, commentUserNameId, row, MovementHistoryTab.commentUserNameColumnIndex);
    MovementHistoryTab.SetTdId(table, commentDateTimeId, row, MovementHistoryTab.commentDateTimeColumnIndex);
    MovementHistoryTab.SetTdId(table, recordTypeId, row, MovementHistoryTab.recordTypeColumnIndex);
    MovementHistoryTab.SetTdId(table, parentGuidId, row, MovementHistoryTab.parentGuidColumnIndex);
    MovementHistoryTab.SetTdId(table, movementHistoryId, row, MovementHistoryTab.movementHistoryGuidColumnIndex);
    MovementHistoryTab.SetTdId(table, pointGuidId, row, MovementHistoryTab.pointGuidColumnIndex);
    MovementHistoryTab.SetTdId(table, rootParentGuidId, row, MovementHistoryTab.rootParentGuidColumnIndex);
    MovementHistoryTab.SetTdId(table, recordSeqId, row, MovementHistoryTab.recordSeqColumnIndex);
    MovementHistoryTab.SetTdId(table, midnightRecordId, row, MovementHistoryTab.midnightRecordColumnIndex);
}

//==========================================================================
// This function will get the column prefix.
//==========================================================================
MovementHistoryTab.GetIdPrefix = function (dataIndex)
{
    var prefix = "";
    switch (dataIndex)
    {
        case MovementHistoryTab.timeStampColumnIndex:
            prefix = "Timestamp_";
            break;
        case MovementHistoryTab.movementNameColumnIndex:
            prefix = "MovementName_";
            break;
        case MovementHistoryTab.movementNodeColumnIndex:
            prefix = "MovementNode_";
            break;
        case MovementHistoryTab.initiationCountColumnIndex:
            prefix = "InitiationCount_";
            break;
        case MovementHistoryTab.siteColumnIndex:
            prefix = "Site_";
            break;
        case MovementHistoryTab.commentColumnIndex:
            prefix = "Comment_";
            break;
        case MovementHistoryTab.commentUserNameColumnIndex:
            prefix = "CommentUserName_";
            break;
        case MovementHistoryTab.commentDateTimeColumnIndex:
            prefix = "CommentDateTime_";
            break;
        case MovementHistoryTab.closeoutDataModifiedByColumnIndex:
            prefix = "CloseoutDataModifiedBy_";
            break;
        case MovementHistoryTab.closeoutDensityProductInAirColumnIndex:
            prefix = "CloseoutDensityProductInAir_";
            break;
        case MovementHistoryTab.closeoutDensityProductObservedColumnIndex:
            prefix = "CloseoutDensityProductObserved_";
            break;
        case MovementHistoryTab.closeoutDensityProductObservedTimeColumnIndex:
            prefix = "CloseoutDensityProductObservedTime_";
            break;
        case MovementHistoryTab.closeoutDensityProductStandardColumnIndex:
            prefix = "CloseoutDensityProductStandard_";
            break;
        case MovementHistoryTab.closeoutDensityProductStandardTimeColumnIndex:
            prefix = "CloseoutDensityProductStandardTime_";
            break;
        case MovementHistoryTab.closeoutDensityProductStandardInAirColumnIndex:
            prefix = "CloseoutDensityProductStandardInAir_";
            break;
        case MovementHistoryTab.closeoutLevelProductColumnIndex:
            prefix = "CloseoutLevelProduct_";
            break;
        case MovementHistoryTab.closeoutLevelProductTimeColumnIndex:
            prefix = "CloseoutLevelProductTime_";
            break;
        case MovementHistoryTab.closeoutLevelWaterColumnIndex:
            prefix = "CloseoutLevelWater_";
            break;
        case MovementHistoryTab.closeoutMassLiquidColumnIndex:
            prefix = "CloseoutMassLiquid_";
            break;
        case MovementHistoryTab.closeoutPercentBswColumnIndex:
            prefix = "CloseoutPercentBsw_";
            break;
        case MovementHistoryTab.closeoutRoofMassColumnIndex:
            prefix = "CloseoutRoofMas_";
            break;
        case MovementHistoryTab.closeoutTankShellCorrectionColumnIndex:
            prefix = "CloseoutTankShellCorrection_";
            break;
        case MovementHistoryTab.closeoutTemperatureAmbientColumnIndex:
            prefix = "CloseoutTemperatureAmbient_";
            break;
        case MovementHistoryTab.closeoutTemperatureAmbientTimeColumnIndex:
            prefix = "CloseoutTemperatureAmbientTime_";
            break;
        case MovementHistoryTab.closeoutTemperatureDensityColumnIndex:
            prefix = "CloseoutTemperatureDensity_";
            break;
        case MovementHistoryTab.closeoutTemperatureProductColumnIndex:
            prefix = "CloseoutTemperatureProduct_";
            break;
        case MovementHistoryTab.closeoutTimeColumnIndex:
            prefix = "CloseoutTime_";
            break;
        case MovementHistoryTab.closeoutTransferGovColumnIndex:
            prefix = "CloseoutTransferGov_";
            break;
        case MovementHistoryTab.closeoutTransferNsvColumnIndex:
            prefix = "CloseoutTransferNsv_";
            break;
        case MovementHistoryTab.closeoutTransferMassLiquidColumnIndex:
            prefix = "CloseoutTransferMassLiquid_";
            break;
        case MovementHistoryTab.closeoutTransferVolumeWaterColumnIndex:
            prefix = "CloseoutTransferVolumeWater_";
            break;
        case MovementHistoryTab.closeoutVolumeBswColumnIndex:
            prefix = "CloseoutVolumeBsw_";
            break;
        case MovementHistoryTab.closeoutVolumeCorrectionFactorColumnIndex:
            prefix = "CloseoutVolumeCorrectionFactor_";
            break;
        case MovementHistoryTab.closeoutVolumeGrossObservedColumnIndex:
            prefix = "CloseoutVolumeGrossObserved_";
            break;
        case MovementHistoryTab.closeoutVolumeGrossStandardColumnIndex:
            prefix = "CloseoutVolumeGrossStandard_";
            break;
        case MovementHistoryTab.closeoutVolumeNetStandardColumnIndex:
            prefix = "CloseoutVolumeNetStandard_";
            break;
        case MovementHistoryTab.closeoutVolumeRoofCorrectionColumnIndex:
            prefix = "CloseoutVolumeRoofCorrection_";
            break;
        case MovementHistoryTab.closeoutVolumeTotalObservedColumnIndex:
            prefix = "CloseoutVolumeTotalObserved_";
            break;
        case MovementHistoryTab.closeoutVolumeWaterColumnIndex:
            prefix = "CloseoutVolumeWater_";
            break;
        case MovementHistoryTab.levelProductColumnIndex:
            prefix = "LevelProduct_";
            break;
        case MovementHistoryTab.typeColumnIndex:
				prefix = "Type_";
				break;
        case MovementHistoryTab.orderNumberColumnIndex:
            prefix = "OrderNumber_";
            break;
        case MovementHistoryTab.plannedStartTimeColumnIndex:
            prefix = "PlannedStartTime_";
            break;
        case MovementHistoryTab.productColumnIndex:
            prefix = "Product_";
            break;
        case MovementHistoryTab.productDescriptionColumnIndex:
            prefix = "ProductDescription_";
            break;
        case MovementHistoryTab.startTimeColumnIndex:
            prefix = "StartTime_";
            break;
        case MovementHistoryTab.startDensityProductObservedColumnIndex:
            prefix = "StartDensityProductObserved_";
            break;
        case MovementHistoryTab.startDensityProductObservedTimeColumnIndex:
            prefix = "StartDensityProductObservedTime_";
            break;
        case MovementHistoryTab.startDensityProductObservedInAirColumnIndex:
            prefix = "StartDensityProductObservedInAir_";
            break;
        case MovementHistoryTab.startDensityProductStandardColumnIndex:
            prefix = "StartDensityProductStandard_";
            break;
        case MovementHistoryTab.startDensityProductStandardTimeColumnIndex:
            prefix = "StartDensityProductStandardTime_";
            break;
        case MovementHistoryTab.startDensityProductStandardInAirColumnIndex:
            prefix = "StartDensityProductStandardInAir_";
            break;
        case MovementHistoryTab.startUserIdColumnIndex:
            prefix = "StartUserId_";
            break;
        case MovementHistoryTab.startLevelProductColumnIndex:
            prefix = "StartLevelProduct_";
            break;
        case MovementHistoryTab.startLevelProductTimeColumnIndex:
            prefix = "StartLevelProductTime_";
            break;
        case MovementHistoryTab.startLevelWaterColumnIndex:
            prefix = "StartLevelWater_";
            break;
        case MovementHistoryTab.startLevelWaterTimeColumnIndex:
            prefix = "StartLevelWaterTime_";
            break;
        case MovementHistoryTab.startMassLiquidColumnIndex:
            prefix = "StartMassLiquid_";
            break;
        case MovementHistoryTab.startPercentBswColumnIndex:
            prefix = "StartPercentBsw_";
            break;
        case MovementHistoryTab.startTankShellCorrectionColumnIndex:
            prefix = "StartTankShellCorrection_";
            break;
        case MovementHistoryTab.startTemperatureAmbientColumnIndex:
            prefix = "StartTemperatureAmbient_";
            break;
        case MovementHistoryTab.startTemperatureAmbientTimeColumnIndex:
            prefix = "StartTemperatureAmbientTime_";
            break;
        case MovementHistoryTab.startTemperatureProductColumnIndex:
            prefix = "StartTemperatureProduct_";
            break;
        case MovementHistoryTab.startTemperatureProductTimeColumnIndex:
            prefix = "StartTemperatureProductTime_";
            break;
        case MovementHistoryTab.startTemperatureDensityColumnIndex:
            prefix = "StartTemperatureDensity_";
            break;
        case MovementHistoryTab.startTemperatureDensityTimeColumnIndex:
            prefix = "StartTemperatureDensityTime_";
            break;
        case MovementHistoryTab.startVolumeColumnIndex:
            prefix = "StartVolume_";
            break;
        case MovementHistoryTab.startVolumeBswColumnIndex:
            prefix = "StartVolumeBsw_";
            break;
        case MovementHistoryTab.startVolumeCorrectionFactorColumnIndex:
            prefix = "StartVolumeCorrectionFactor_";
            break;
        case MovementHistoryTab.startVolumeGrossObservedColumnIndex:
            prefix = "StartVolumeGrossObserved_";
            break;
        case MovementHistoryTab.startVolumeGrossStandardColumnIndex:
            prefix = "StartVolumeGrossStandard_";
            break;
        case MovementHistoryTab.startVolumeNetStandardColumnIndex:
            prefix = "StartVolumeNetStandard_";
            break;
        case MovementHistoryTab.startVolumeRoofCorrectionColumnIndex:
            prefix = "StartVolumeRoofCorrection_";
            break;
        case MovementHistoryTab.startVolumeTotalObservedColumnIndex:
            prefix = "StartVolumeTotalObserved_";
            break;
        case MovementHistoryTab.startVolumeWaterColumnIndex:
            prefix = "StartVolumeWater_";
            break;
        case MovementHistoryTab.stopTimeColumnIndex:
            prefix = "StopTime_";
            break;
        case MovementHistoryTab.statusColumnIndex:
            prefix = "Status_";
            break;
        case MovementHistoryTab.transferDeviationColumnIndex:
            prefix = "TransferDeviation_";
            break;
        case MovementHistoryTab.transferPercentDeviationColumnIndex:
            prefix = "TransferPercentDeviation_";
            break;
        case MovementHistoryTab.transferDirectionColumnIndex:
            prefix = "TransferDirection_";
            break;
        case MovementHistoryTab.transferModeColumnIndex:
            prefix = "transferMode_";
            break;
        case MovementHistoryTab.transferStatusColumnIndex:
            prefix = "TransferStatus_";
            break;
        case MovementHistoryTab.transferTargetColumnIndex:
            prefix = "TransferTarget_";
            break;
         case MovementHistoryTab.transferTargetUnitsColumnIndex:
            prefix = "TransferTargetUnits_";
            break;
		 case MovementHistoryTab.transferTargetLevelColumnIndex:
			 prefix = "TransferLevelTarget_";
			 break;
		 case MovementHistoryTab.transferVolumeTargetColumnIndex:
			 prefix = "TransferVolumeTarget_";
			 break;
        case MovementHistoryTab.transferTimeRemainingColumnIndex:
            prefix = "TransferTimeRemaining_";
            break;
        case MovementHistoryTab.transferredVolumeWaterColumnIndex:
            prefix = "TransferredVolumeWater_";
            break;
        case MovementHistoryTab.transferredVolumeColumnIndex:
            prefix = "TransferredVolume_";
            break;
            break;
        case MovementHistoryTab.unitsLevelProductColumnIndex:
            prefix = "UnitsLevelProduct_";
            break;
        case MovementHistoryTab.unitsTemperatureAmbientColumnIndex:
            prefix = "UnitsTemperatureAmbient_";
            break;
        case MovementHistoryTab.unitsTemperatureDensityColumnIndex:
            prefix = "UnitsTemperatureDensity_";
            break;
        case MovementHistoryTab.unitsTemperatureProductColumnIndex:
            prefix = "UnitsTemperatureProduct_";
            break;
        case MovementHistoryTab.unitsDensityProductObservedColumnIndex:
            prefix = "UnitsDensityProductObservedC_";
            break;
        case MovementHistoryTab.unitsDensityProductStandardColumnIndex:
            prefix = "UnitsDensityProductStandard_";
            break;
        case MovementHistoryTab.unitsVolumeColumnIndex:
            prefix = "UnitsVolume_";
            break;
        case MovementHistoryTab.unitsMassColumnIndex:
            prefix = "UnitsMass_";
            break;
        case MovementHistoryTab.userData01ColumnIndex:
            prefix = "UserData01_";
            break;
        case MovementHistoryTab.userData02ColumnIndex:
            prefix = "UserData02_";
            break;
        case MovementHistoryTab.userData03ColumnIndex:
            prefix = "UserData03_";
            break;
        case MovementHistoryTab.userData04ColumnIndex:
            prefix = "UserData04_";
            break;
        case MovementHistoryTab.userData05ColumnIndex:
            prefix = "UserData05_";
            break;
        case MovementHistoryTab.userData06ColumnIndex:
            prefix = "UserData06_";
            break;
        case MovementHistoryTab.userData07ColumnIndex:
            prefix = "UserData07_";
            break;
        case MovementHistoryTab.userData08ColumnIndex:
            prefix = "UserData08_";
            break;
        case MovementHistoryTab.userData09ColumnIndex:
            prefix = "UserData09_";
            break;
        case MovementHistoryTab.userData10ColumnIndex:
            prefix = "UserData10_";
            break;
        case MovementHistoryTab.volumeWaterColumnIndex:
            prefix = "VolumeWater_";
            break;
        case MovementHistoryTab.recordTypeColumnIndex:
            prefix = "RecordType_";
            break;
        case MovementHistoryTab.parentGuidColumnIndex:
            prefix = "ParentGuid_";
            break;
        case MovementHistoryTab.movementHistoryGuidColumnIndex:
            prefix = "MovementHistoryGuid_";
            break;
        case MovementHistoryTab.pointGuidColumnIndex:
            prefix = "PointGuid_";
            break;
        case MovementHistoryTab.rootParentGuidColumnIndex:
            prefix = "RootParentGuid_";
            break;
        case MovementHistoryTab.recordSeqColumnIndex:
            prefix = "RecordSeq_";
            break;
        case MovementHistoryTab.midnightRecordColumnIndex:
            prefix = "MidnightRecord_";
            break;
    }
    return prefix;
}

//=========================================================================
// This function will Add an ID to a Cell.
//=========================================================================
MovementHistoryTab.AddIdToCell = function (td, cellData, rowData, row, col)
{
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }
    var movementHistoryRecordGuid = rowData.MovementHistoryRecordGuid;
    var table = $("#MovementHistoryTable").DataTable();

    var origColIndex = table.colReorder.transpose(col, "toOriginal");
    var idStr = MovementHistoryTab.GetIdPrefix(origColIndex) + movementHistoryRecordGuid;
    td.id = idStr;
}

//===================================================================
// This function will get elements by the class name.
//===================================================================
MovementHistoryTab.getElementsByClassName = function (node, classname)
{
    if (node.getElementsByClassName)
    {
        // use native implementation if available
        return node.getElementsByClassName(classname);
    }
    else
    {
        return (function getElementsByClass(searchClass, node)
        {
            if (node == null)
            {
                node = document;
            }

            var classElements = [],
                els = node.getElementsByTagName("*"),
                elsLen = els.length,
                pattern = new RegExp("(^|\\s)" + searchClass + "(\\s|$)"), i, j;

            for (i = 0, j = 0; i < elsLen; i++)
            {
                if (pattern.test(els[i].className))
                {
                    classElements[j] = els[i];
                    j++;
                }
            }

            return classElements;
        })(classname, node);
    }
}

//=====================================================================
// This function will put the comment in edit mode.
//=====================================================================
MovementHistoryTab.PutCommentInEditMode = function ()
{
    if (MovementHistoryTab.inEditMode === false)
    {
        var commentPrefix = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentColumnIndex);

        if (this.id.indexOf(commentPrefix) === 0)
        {
            MovementHistoryTab.commentEditId = this.id;




            var movementHistoryGuid = this.id.replace(commentPrefix, "");
            var model = MovementHistoryTab.GetMovementHistoryModel();
            var movementHistoryList = model.MovementHistories;

            for (var i = 0; i < movementHistoryList.length; i++)
            {
                if (movementHistoryList[i].MovementHistoryGuid === movementHistoryGuid)
                {
                    MovementHistoryTab.originalEditValue = $(this).clone().html();
                    this.innerHTML = MovementHistoryTab.CreateEditableComment(movementHistoryList[i].Comment, movementHistoryGuid, movementHistoryList[i].CreatedDate);
                    MovementHistoryTab.inEditMode = true;
                    MovementHistoryTab.editModeId = this.id;
                    var commentInputTag = $(this).find('input');
                    commentInputTag.focus();
                    return;
                }
            }
        }
    }
}

//==============================================================================
// This function will remove the comment input tag and reset the comment
// edit ID to empty.
//==============================================================================
MovementHistoryTab.RemoveCommentFromEditMode = function ()
{
    MovementHistoryTab.inEditMode = false;
    var commentTdCntrl = $("#" + MovementHistoryTab.commentEditId);
    commentTdCntrl.find("span:first-child").remove();
    MovementHistoryTab.commentEditId = "";
}

MovementHistoryTab.reinitializehistorydisplay = function ()
{
    var activeTab = FMOperateIndex.GetActiveTab("MovementHistory", MovementHistoryTab.TabidNumber);
    if (activeTab === true)
    {
        clearInterval(MovementHistoryTab.RefreshTimer);
        MovementHistoryTab.RefreshTimer = null;
        MovementHistoryTab.Initialize(); //bds
    }
    else if (FMOperateIndex.allScreensRestored === true)
    {
        clearInterval(MovementHistoryTab.RefreshTimer);
        MovementHistoryTab.RefreshTimer = null;
    }
}


//=========================================================================
// This function performs the initialization.
//=========================================================================
MovementHistoryTab.Init = function ()
{
    var tabIDnumber = $("#MovementHistoryTabName")[0].innerText;

    // Hide the history row context menu.
    $("#HistoryRowContextMenuDiv").hide();

    MovementHistoryTab.TabidNumber = tabIDnumber;

    $('<div id="LoadingImageMovementHistory" class="LoadingAnimation transparent"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#MovementHistoryTableSection');

    if (FMOperateIndex.openingNewMovementHistory === false)
    {
        var activeTab = FMOperateIndex.GetActiveTab("MovementHistory", MovementHistoryTab.TabidNumber);

        if (activeTab === false && MovementHistoryTab.RefreshTimer === null)
        {  // not the active window so start a timer to check every 200msec. The operate index object is not very efficient
            // at determining when a window is active and when it is not
            //MovementHistoryTab.StartTimer();
            MovementHistoryTab.RefreshTimer = setInterval(MovementHistoryTab.reinitializehistorydisplay, 500);
            return;
        }
    }

    MovementHistoryTab.Initialize();
};

//==================================================================================
// This function initializes the movement history.
//==================================================================================
MovementHistoryTab.Initialize = function ()
{
    // Set the Y-scroll on the column selection dropdown
    $("#MovementHistoryColumnFilterDiv").css("overflow-y", "auto");

    var model = MovementHistoryTab.GetMovementHistoryModel();
    MovementHistoryTab.hasModifyMovementHistoryRight = model.HasModifyMovementHistoryRight;
    MovementHistoryTab.hasViewMovementHistoryRight = model.HasViewMovementHistoryRight;

    var movementHistoryViewStateObj = null;
    var loadImage = $("#LoadingImageMovementHistory");

    // make sure that the init timer is not running
    if (MovementHistoryTab.RefreshTimer !== null)
    {
        clearInterval(MovementHistoryTab.RefreshTimer);
        MovementHistoryTab.RefreshTimer = null;
    }

    if (MovementHistoryTab.HistoryInitialized === true)
    {
        return;
    }

	if (!$.fn.dataTable.isDataTable('#MovementHistoryTable')) {
	} else {
		try {
			$("#MovementHistoryTable").DataTable().destroy();
		} catch {
		}
	}


    $("#MovementHistoryTable").removeClass("hidden");

    loadImage.show();
    MovementHistoryTab.HistoryInitialized = true;

    if (model && model.ViewStateSettings && model.ViewStateSettings.JsonViewState && model.ViewStateSettings.JsonViewState.length > 0)
    {
        movementHistoryViewStateObj = JSON.parse(model.ViewStateSettings.JsonViewState);

        if (movementHistoryViewStateObj && movementHistoryViewStateObj.Filters)
        {
            MovementHistoryTab.columnFilterCollection = movementHistoryViewStateObj.Filters;
            for (var nextItem = 0; nextItem < MovementHistoryTab.columnFilterCollection.length; nextItem++)
            {
                var filterObj = MovementHistoryTab.columnFilterCollection[nextItem];
                if (filterObj.Index == MovementHistoryTab.midnightRecordColumnIndex)
                {
                    $("#MovementHistoryMidnightRecordCheckbox").prop("checked", filterObj.ShowMidnightRecord);
                    break;
                }
            }
        }
    }

    MovementHistoryTab.initialLoadRequest = true;

    var url = $('#MovementHistoryGetDataUrl').val();
    var token = $('#MovementHistoryTabView input[name =__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;
    var cols = MovementHistoryTab.CreateColumns();

    MovementHistoryTab.DatatableHandle = $("#MovementHistoryTable").DataTable({
        "columnDefs": [
            {
                "className": "col-sm-2 col-md-2 text-center movementHistoryTableCellEx",
                "targets": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 
                    10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
                    20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
                    30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
                    40, 41, 42, 43, 44, 45, 46, 47, 48, 49,
                    50, 51, 52, 53, 54, 55, 56, 57, 58, 59,
                    60, 61, 62, 63, 64, 65, 66, 67, 68, 69,
                    70, 71, 72, 73, 74, 75, 76, 77, 78, 79,
                    80, 81, 82, 83, 84, 85, 26, 87, 88, 89,
                    90, 91, 92, 93, 94, 95, 96, 97, 98, 99,
                    100, 101, 102, 103, 104, 105, 106, 107,
                    108, 109, 110],
                "createdCell": MovementHistoryTab.AddIdToCell
            }, {
                "className": "col-sm-2 col-md-2 text-center movementHistoryTableCommentCellEx",
                "targets": [5],
                "createdCell": MovementHistoryTab.AddIdToCell
            }
        ],
        "language":
        {
            "processing": "<div class='overlay'><i style=\"background-color: white !important;color: black !important;z-index:5\">" + $("#MovementHistory_DD_Processing").val() + "</i></div>",
            "info": $("#MovementHistory_DD_Showing").val() + " _START_ " + $("#MovementHistory_DD_to").val() + " _END_ " + $("#MovementHistory_DD_of").val() + " _TOTAL_ " + $("#MovementHistory_DD_Movements").val(),
            "lengthMenu": $("#MovementHistory_DD_Show").val() + "  _MENU_  " + $("#MovementHistory_DD_Movements").val()
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
            "data": function (d)
            {
                d.columnFilterInfoList = MovementHistoryTab.columnFilterCollection;
                d.originalColumnOrderIndex = MovementHistoryTab.GetOriginalColumnOrderIndex();
                var orderDir = "desc";
                if (d.order.length > 0)
                {
                    orderDir = d.order[0].dir;
                }
                return JSON.stringify({
                    "draw": d.draw,
                    "orderDir": orderDir,
                    "start": d.start,
                    "length": d.length,
                    "columnFilterInfoList": MovementHistoryTab.columnFilterCollection,
                    "originalColumnOrderIndex": MovementHistoryTab.GetOriginalColumnOrderIndex(),
                    "initialLoadRequest": MovementHistoryTab.initialLoadRequest
                });
            },
            "dataFilter": MovementHistoryTab.SetPageDataInMovementHistoryModel,
            'headers': headers,
            "error": function (xhr, error, thrown)
            {
                FMErrorAndExceptionHandling.ShowError(thrown, null, MovementHistoryTab.messageAttributes);
            }
        },
        "order": MovementHistoryTab.OrderColumns(),
        "columns": cols,
        "colReorder": {
            fixedColumnsLeft: 3
        },
        "ordering": true,
        "scrollY": "100px",
        "scrollX": true,
        "paging": true,
        "bFilter": false,
        "bInfo": true,
        "bAutoWidth": false,
        "pageLength": 500,
        "lengthMenu": [[10, 25, 50, 100, 500], [10, 25, 50, 100, 500]],
        "createdRow": MovementHistoryTab.AddIdToTd,
        "dom": '<"MovementHistory_top"l>rt<"MovementHistory_bottom"pi>',
        "fnDrawCallback": function (oSettings)
        {
            $('.MovementHistoryTableClass thead th.MovementHistoryTableCommentCellEx').removeClass('MovementHistoryTableCommentCellEx');
        }
    });

    // This event registration is for the history context menu.
    $('#MovementHistoryTable tbody').on('mousedown', 'tr', function (e)
    {
        // Check for right mouse (2) click. If not right mouse, then return.
        if (e.button !== 2) return;

        window.addEventListener("contextmenu", e => e.preventDefault());
        MovementHistoryTab.HandleRightMouseClick(event);
    });

    // Must be in order to display correctly.
    $("#MovementHistoryTopCustomTypeControlDiv").appendTo("#MovementHistoryTableSection .MovementHistory_top");
    $("#MovementHistoryTopCustomButtonDiv").appendTo("#MovementHistoryTableSection .MovementHistory_top");
    $("#MovementHistoryTopCustomControlDiv").appendTo("#MovementHistoryTableSection .MovementHistory_top");

    // Set the event for when the column sort.
    $("#MovementHistoryTable").on('order.dt', MovementHistoryTab.ColorSortColumn);

    if (MovementHistoryTab.hasModifyMovementHistoryRight)
    {
        // double click to edit a row 
        $('#MovementHistoryTable tbody').on('dblclick', 'td', MovementHistoryTab.PutCommentInEditMode);
    }

    MovementHistoryTab.InitializeDatePickers();

    // Since the Date & Time column is the default sorted column,
    // color the entire column to indicated it is being sorted.
    var dateTimeColumn = MovementHistoryTab.DatatableHandle.column(MovementHistoryTab.timeStampColumnIndex);
    dateTimeColumn.nodes().each(function (cell)
    {
        cell.classList.add("sortedColumnColor");
    });


    $('#MovementHistoryTable').on('draw.dt', MovementHistoryTab.TablePageChanged);

    $('#MovementHistoryTable').on('column-sizing.dt', function (e, settings)
    {
        //MovementHistoryTab.StyleComments();
        MovementHistoryTab.TablePageChanged();
    });

    var find = document.getElementById('MovementHistoryTabFind');
    find.oninput = MovementHistoryTab.DoFind;

    $('#MovementHistoryTabFind').val(MovementHistoryTab.CurrentFindString);

    // TODO: Future
    //$('#"#MovementHistoryTable"').dataTable({
    //    "fnCreatedRow": function (nRow, aData, iDataIndex)
    //    {
    //        $('td:eq(0)', nRow).append("<div class='col1d'><button class='editBut'><img >src='img/property32.png'></button></div>");
    //    },
    //});

    MovementHistoryTab.stack_bottomright_movementhistory = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#MovementHistoryTableSection').parent() };
    MovementHistoryTab.messageAttributes = { addclass: 'stack-bottomright', stack: MovementHistoryTab.stack_bottomright_movementhistory };

    MovementHistoryTab.HandleWindowResize();
   
    // The html helper sets always the attribute 'selected' for the options (sets to false if not selected).
    // Select2 expects the selected attribute only for those selected.
    $('#MovementHistoryAvailableFilterDropdownId').find('option[selected=false]').removeAttr('selected');
    $('#MovementHistoryAvailableFilterDropdownId').select2({ allowClear: true });
    $('#MovementHistoryAvailableFilterDropdownId').on("select2:selecting", MovementHistoryTab.HandleAvailableFilterDropdownSelectEvent);
    $('#MovementHistoryAvailableFilterDropdownId').on("select2:unselect", MovementHistoryTab.HandleAvailableFilterDropdownUnselectEvent);

    // Create all the column filter checkboxes for each of the columns.
    // This must happen before the view state setting.
    MovementHistoryTab.CreateAllColumnFilterCheckboxes();

    if (movementHistoryViewStateObj)
    {
        MovementHistoryTab.InitializeColumnFilterDropdownCheckboxes(false);
        MovementHistoryTab.SetInitialVisibilityColumnReorder(movementHistoryViewStateObj);

        if (movementHistoryViewStateObj.PageLen)
        {
            MovementHistoryTab.DatatableHandle.page.len(movementHistoryViewStateObj.PageLen);
            MovementHistoryTab.Refresh();
        }
    }
    else
    {
        MovementHistoryTab.InitializeColumnFilterDropdownCheckboxes(true);
        MovementHistoryTab.DatatableHandle.draw();
    }

    // Reset the select all checkbox based on the other checkbox settings.
    MovementHistoryTab.ResetSelectAllCheckbox();

    loadImage.fadeOut(500);

    $('#MovementHistoryTable').on('length.dt', function (e, settings, len) {
      MovementHistoryTab.SaveViewState();
    });
    $('#MovementHistoryTable').on('column-reorder.dt', function (e, settings, details) {
      if (!MovementHistoryTab.changingincludedcolumns) {
        MovementHistoryTab.SaveViewState();
      }
    });

};

//==========================================================================
// This function will get the original column index for the column that
// is being sorted.  This is needed by the controller since it only knows
// the column indexes by their original index.
//==========================================================================
MovementHistoryTab.GetOriginalColumnOrderIndex = function ()
{
   if (MovementHistoryTab.HistoryInitialized === false) {
      return -1;
   }
   var table = $("#MovementHistoryTable").DataTable();
    var order = table.order();
    var selectedColumnIndex = order[0][0];

    // Get the original column index to be sorted on.
    var orderArray = table.colReorder.order();
    var originalColumnOrderIndex = orderArray[selectedColumnIndex];

    return originalColumnOrderIndex;
}

//============================================================
// This function handles the scroll Y resize.
//============================================================
MovementHistoryTab.ScrollYResize = function ()
{
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }
   var w = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    var topDiv = document.getElementById("MovementHistoryTable");
    var divTop = topDiv.getBoundingClientRect().top;
    var h = w - divTop - 55;
    h = Math.round(h);
    var hString = h + 'px';
    $('#MovementHistoryTable').dataTable().fnSettings().oScroll.sY = hString;
    $('.dataTables_scrollBody:has(#MovementHistoryTable)').height(hString);
}

//================================================================================
// This function will handle the coloring of the column that has been selected
// to be sorted.
//================================================================================
MovementHistoryTab.ColorSortColumn = function ()
{
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }
   var table = $("#MovementHistoryTable").DataTable();
    var order = table.order();
    var selectedColumnIndex = order[0][0];
    var column = table.column(selectedColumnIndex);

    var numberOfColumns = $("#MovementHistoryTable thead th").length;

    for (var nextColIndex = 0; nextColIndex <= numberOfColumns - 1; nextColIndex++)
    {
        var resetColumn = table.column(nextColIndex);
        resetColumn.nodes().each(function (resetCell)
        {
            resetCell.classList.remove("sortedColumnColor");
        });
    }

    column.nodes().each(function (cell)
    {
        cell.classList.add("sortedColumnColor");
    });
}

//================================================================================
// This function will handle the window resize event.
//================================================================================
MovementHistoryTab.HandleWindowResize = function ()
{
    MovementHistoryTab.Newdiv(true);
    MovementHistoryTab.ScrollYResize();
    MovementHistoryTab.Newdiv(true);
}

MovementHistoryTab.ColumnCheckboxFilterHelper = function (checked, columnIndex)
{
    var checkboxControl = new Object();
    checkboxControl.checked = checked;
    checkboxControl.columnIndex = columnIndex;
    MovementHistoryTab.checkboxFilterArray.push(checkboxControl);
};

//================================================================================
// This function will handle the option filter for hand and auto gauge checkbox 
// being checked / unchecked event.
//================================================================================
MovementHistoryTab.HandleGaugeFilterCheckboxChange = function ()
{
    MovementHistoryTab.GaugeCheckboxChange = false;

    if (MovementHistoryTab.columnFilterCollection != null)
    {
        var found = false;

        for (var nextFilterIndex = 0; nextFilterIndex < MovementHistoryTab.columnFilterCollection.length; nextFilterIndex++)
        {
            var filterObj = MovementHistoryTab.columnFilterCollection[nextFilterIndex];

            if (filterObj.Index === MovementHistoryTab.recordTypeColumnIndex)
            {
                filterObj.ShowAutoGauge = $("#MovementHistoryAutoGaugeCheckbox").is(":checked");
                filterObj.ShowHandGauge = $("#MovementHistoryHandgaugeCheckbox").is(":checked");

                MovementHistoryTab.GaugeCheckboxChange = true;
                found = true;
                break;
            }
        }

        if (found == false)
        {
            columnFilterObj = MovementHistoryTab.CreateAvailableFilterObject();
            columnFilterObj.Index = MovementHistoryTab.recordTypeColumnIndex;
            columnFilterObj.Name = "RecordType";

            columnFilterObj.ShowAutoGauge = $("#MovementHistoryAutoGaugeCheckbox").is(":checked");
            columnFilterObj.ShowHandGauge = $("#MovementHistoryHandgaugeCheckbox").is(":checked");

            MovementHistoryTab.columnFilterCollection.push(columnFilterObj);
            MovementHistoryTab.GaugeCheckboxChange = true;
        }
  }
};

//================================================================================
// This function will handle the option for midnight records filter checkbox 
// being checked / unchecked event.
//================================================================================
MovementHistoryTab.MidnightRecordFilterCheckboxChange = function ()
{
    MovementHistoryTab.midnightRecordCheckboxChange = false;

    if (MovementHistoryTab.columnFilterCollection != null)
    {
        var found = false;

        for (var nextFilterIndex = 0; nextFilterIndex < MovementHistoryTab.columnFilterCollection.length; nextFilterIndex++)
        {
            var filterObj = MovementHistoryTab.columnFilterCollection[nextFilterIndex];

            if (filterObj.Index === MovementHistoryTab.midnightRecordColumnIndex)
            {
                filterObj.ShowMidnightRecord = $("#MovementHistoryMidnightRecordCheckbox").is(":checked");

                MovementHistoryTab.midnightRecordCheckboxChange = true;
                found = true;
                break;
            }
        }

        if (found == false)
        {
            columnFilterObj = MovementHistoryTab.CreateAvailableFilterObject();
            columnFilterObj.Index = MovementHistoryTab.midnightRecordColumnIndex;
            columnFilterObj.Name = "MidnightRecord";

            columnFilterObj.ShowMidnightRecord = $("#MovementHistoryMidnightRecordCheckbox").is(":checked");
            MovementHistoryTab.columnFilterCollection.push(columnFilterObj);
            MovementHistoryTab.midnightRecordCheckboxChange = true;
        }
    }
};

//================================================================================
// This function will handle the column filter checkbox being checked/unchecked
// event.
//================================================================================
MovementHistoryTab.HandleColumnFilterCheckboxChange = function (currentItem)
{
    var checked = $(currentItem).is(":checked");
    var inputId = $(currentItem).attr("id");

    if (typeof (checked) != "undefined" && typeof (inputId) != "undefined")
    {
        switch (inputId)
        {
            case "MovementHistorySelectAllCheckbox":
                MovementHistoryTab.ToggleAllColumnVisibility(checked);
                break;
            case "ColumnFilterCheckbox" + MovementHistoryTab.initiationCountColumnIndex:
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.initiationCountColumnIndex);
                break;
            case "ColumnFilterCheckbox" + MovementHistoryTab.siteColumnIndex:
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.siteColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.commentColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.commentColumnIndex);
                $('.movementHistoryTableClass thead th.movementHistoryTableCommentCellEx').removeClass('movementHistoryTableCommentCellEx');
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutDataModifiedByColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutDataModifiedByColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutDensityProductInAirColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutDensityProductInAirColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutDensityProductObservedColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutDensityProductObservedColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutDensityProductObservedTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutDensityProductObservedTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutDensityProductStandardColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutDensityProductStandardColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutDensityProductStandardTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutDensityProductStandardTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutDensityProductStandardInAirColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutDensityProductStandardInAirColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutLevelProductColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutLevelProductColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutLevelProductTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutLevelProductTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutLevelWaterColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutLevelWaterColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutMassLiquidColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutMassLiquidColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutPercentBswColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutPercentBswColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutRoofMassColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutRoofMassColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutTankShellCorrectionColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutTankShellCorrectionColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutTemperatureAmbientColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutTemperatureAmbientColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutTemperatureAmbientTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutTemperatureAmbientTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutTemperatureDensityColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutTemperatureDensityColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutTemperatureProductColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutTemperatureProductColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutTransferGovColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutTransferGovColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutTransferNsvColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutTransferNsvColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutTransferMassLiquidColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutTransferMassLiquidColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutTransferVolumeWaterColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutTransferVolumeWaterColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutVolumeBswColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutVolumeBswColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutVolumeCorrectionFactorColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutVolumeCorrectionFactorColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutVolumeGrossObservedColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutVolumeGrossObservedColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutVolumeGrossStandardColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutVolumeGrossStandardColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutVolumeNetStandardColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutVolumeNetStandardColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutVolumeRoofCorrectionColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutVolumeRoofCorrectionColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutVolumeTotalObservedColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutVolumeTotalObservedColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.closeoutVolumeWaterColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.closeoutVolumeWaterColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.levelProductColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.levelProductColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.typeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.typeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.orderNumberColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.orderNumberColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.plannedStartTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.plannedStartTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.productColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.productColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.productDescriptionColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.productDescriptionColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startDensityProductObservedColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startDensityProductObservedColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startDensityProductObservedTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startDensityProductObservedTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startDensityProductObservedInAirColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startDensityProductObservedInAirColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startDensityProductStandardColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startDensityProductStandardColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startDensityProductStandardTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startDensityProductStandardTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startDensityProductStandardInAirColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startDensityProductStandardInAirColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startUserIdColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startUserIdColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startLevelProductColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startLevelProductColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startLevelProductTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startLevelProductTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startLevelWaterColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startLevelWaterColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startLevelWaterTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startLevelWaterTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startMassLiquidColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startMassLiquidColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startPercentBswColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startPercentBswColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startTankShellCorrectionColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startTankShellCorrectionColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startTemperatureAmbientColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startTemperatureAmbientColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startTemperatureAmbientTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startTemperatureAmbientTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startTemperatureProductColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startTemperatureProductColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startTemperatureProductTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startTemperatureProductTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startTemperatureDensityColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startTemperatureDensityColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startTemperatureDensityTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startTemperatureDensityTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startVolumeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startVolumeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startVolumeBswColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startVolumeBswColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startVolumeCorrectionFactorColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startVolumeCorrectionFactorColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startVolumeGrossObservedColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startVolumeGrossObservedColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startVolumeGrossStandardColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startVolumeGrossStandardColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startVolumeNetStandardColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startVolumeNetStandardColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startVolumeRoofCorrectionColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startVolumeRoofCorrectionColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startVolumeTotalObservedColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startVolumeTotalObservedColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.startVolumeWaterColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.startVolumeWaterColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.stopTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.stopTimeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.statusColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.statusColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.transferDeviationColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferDeviationColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.transferPercentDeviationColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferPercentDeviationColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.transferDirectionColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferDirectionColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.transferModeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferModeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.transferStatusColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferStatusColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.transferTargetColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferTargetColumnIndex);
                break;
             case ("ColumnFilterCheckbox" + MovementHistoryTab.transferTargetUnitsColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferTargetUnitsColumnIndex);
                break;
			  case ("ColumnFilterCheckbox" + MovementHistoryTab.transferLevelTargetColumnIndex):
				  MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferLevelTargetColumnIndex);
				  break;
			  case ("ColumnFilterCheckbox" + MovementHistoryTab.transferVolumeTargetColumnIndex):
				  MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferVolumeTargetColumnIndex);
				  break;
           case ("ColumnFilterCheckbox" + MovementHistoryTab.transferTimeRemainingColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferTimeRemainingColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.transferredVolumeWaterColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferredVolumeWaterColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.transferredVolumeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.transferredVolumeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.unitsLevelProductColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.unitsLevelProductColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.unitsTemperatureAmbientColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.unitsTemperatureAmbientColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.unitsTemperatureDensityColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.unitsTemperatureDensityColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.unitsTemperatureProductColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.unitsTemperatureProductColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.unitsDensityProductObservedColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.unitsDensityProductObservedColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.unitsDensityProductStandardColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.unitsDensityProductStandardColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.unitsVolumeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.unitsVolumeColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.unitsMassColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.unitsMassColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.userData01ColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.userData01ColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.userData02ColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.userData02ColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.userData03ColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.userData03ColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.userData04ColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.userData04ColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.userData05ColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.userData05ColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.userData06ColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.userData06ColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.userData07ColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.userData07ColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.userData08ColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.userData08ColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.userData09ColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.userData09ColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.userData10ColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.userData10ColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.volumeWaterColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.volumeWaterColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.commentUserNameColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.commentUserNameColumnIndex);
                break;
            case ("ColumnFilterCheckbox" + MovementHistoryTab.commentDateTimeColumnIndex):
                MovementHistoryTab.ColumnCheckboxFilterHelper(checked, MovementHistoryTab.commentDateTimeColumnIndex);
                break;
        }
    }
}

//===========================================================================================
// This function handles the column reorder event.
//===========================================================================================
MovementHistoryTab.VisibilityColumnReorder = function (table, checked, originalColumnIndex)
{
    var orderArray = table.colReorder.order();
    var visibleArr = [];
    var invisibleArr = [];

    for (var i = 0; i < orderArray.length; i++)
    {
        if (orderArray[i] !== originalColumnIndex)
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
    }

    var newOrderArray = visibleArr;
    newOrderArray.push(originalColumnIndex);

    for (var j = 0; j < invisibleArr.length; j++)
    {
        newOrderArray.push(invisibleArr[j]);
    }

    table.colReorder.order(newOrderArray, true);
}

//================================================================================
// This function will toggle the column visibility.
//================================================================================
MovementHistoryTab.ToggleColumnVisibility = function (table, checked, originalColumnIndex)
{
    if (originalColumnIndex < 3) {
        return;
    }
    MovementHistoryTab.VisibilityColumnReorder(table, checked, originalColumnIndex);
    var currentColIndex = table.colReorder.transpose(originalColumnIndex);
    var column = table.column(currentColIndex);
    column.visible(checked);

};

//==================================================================================
// This function will reset the select all checkbox based on the other checkboxes.
//==================================================================================
MovementHistoryTab.ResetSelectAllCheckbox = function ()
{
    var items = $("#MovementHistoryColumnFilterUl li label input");
    var checked = true;

    if (items && items.length > 0)
    {
        for (var nextCbIndex = 0; nextCbIndex < items.length; nextCbIndex++)
        {
            var checkboxId = items[nextCbIndex].id;

            if (checkboxId === "MovementHistorySelectAllCheckbox"
                || checkboxId === "MovementHistoryDateTimeCheckbox"
                || checkboxId === "MovementNameCheckbox"
                || checkboxId === "MovementNodeCheckbox")
            {
                continue;
            }

            var isChecked = $("#" + checkboxId).is(":checked");
            checked = checked & isChecked;
        }

        if (checked == false) {
            $("#MovementHistorySelectAllCheckbox").prop("checked", false);
        }
        else {
            $("#MovementHistorySelectAllCheckbox").prop("checked", true);
        }
    }
}

//=============================================================================================
// This function will perform a select all or unselect all on the columns to be displayed.
//=============================================================================================
MovementHistoryTab.ToggleAllColumnVisibility = function (checked)
{
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }
    var table = $("#MovementHistoryTable").DataTable();

    if (checked) {
            var currentOrder = table.colReorder.order();
            for (var nextIndex = MovementHistoryTab.totalColumns; nextIndex >= 3; nextIndex--) {
                var currentColIndex = table.colReorder.transpose(nextIndex);
                var column = table.column(currentColIndex);
                column.visible(true, false);

            }
            // set column order to orginal order
            table.colReorder.reset();
    } else {
            //Hide all but first 3 columns since Timestamp, Name, and Node are always visible
            var currentOrder = table.colReorder.order();
            for (var nextIndex = currentOrder.length-1; nextIndex >= 3; nextIndex--)
            {
                var currentColIndex = table.colReorder.transpose(nextIndex);
                var column = table.column(currentColIndex);
                column.visible(false, false);
            }

     }
    table.columns.adjust().draw(false); 
    // Set all the checkboxes to the proper state.
    var items = $("#MovementHistoryColumnFilterUl li label input");
    if (items && items.length > 0)
    {
        for (var nextCbIndex = 0; nextCbIndex < items.length; nextCbIndex++)
        {
            var checkboxId = items[nextCbIndex].id;

            if (checkboxId === "MovementHistorySelectAllCheckbox"
                || checkboxId === "MovementHistoryDateTimeCheckbox"
                || checkboxId === "MovementNameCheckbox"
                || checkboxId === "MovementNodeCheckbox")
            {
                continue;
            }

            $("#" + checkboxId).prop("checked", checked);
        }
    }

    MovementHistoryTab.DoFindLogic();
    MovementHistoryTab.ResetSelectAllCheckbox();
    MovementHistoryTab.ResizeColumns();
};

//====================================================================================
// This function updates the ID for a column that was added to the table.
//====================================================================================
MovementHistoryTab.UpdateTdId = function (columnIndex)
{
    $("#MovementHistoryTable tr").each(function ()
    {
        var rowId = $(this).attr("id");

        if (!rowId)
        {
            return;
        }

        var rowGuid = rowId.replace("Row_", "");

        $(this).find("td").each(function ()
        {
            var colId = $(this).attr("id");
            if (colId)
            {
                colPrefix = MovementHistoryTab.GetIdPrefix(columnIndex);
                var parts = colId.split("_");

                if (parts && parts.length == 2)
                {
                    var prefix = parts[0] + "_";
                    if (colPrefix === prefix && parts[1] === "undefined")
                    {
                        $(this).attr("id", colPrefix + rowGuid);
                    }
                }
            }
        });
    });
};

//================================================================================
// This function will handle the option filter dropdown on click event. It will
// expand or collapse the dropdown based on the "hidden" class state.
//================================================================================
MovementHistoryTab.HandleGaugeFilterDropdownExpandCollapse = function ()
{
    var hiddenClass = $("#MovementHistoryGaugeFilterDiv").attr('class');

    if (hiddenClass === "")
    {
        $("#MovementHistoryGaugeFilterDiv").addClass('hidden');

        if (MovementHistoryTab.GaugeCheckboxChange || MovementHistoryTab.midnightRecordCheckboxChange)
        {
            MovementHistoryTab.HandleRefreshBtnEvent();
        }
        MovementHistoryTab.SaveViewState();
    }
    else
    {
        $("#MovementHistoryGaugeFilterDiv").removeClass('hidden');
    }

    MovementHistoryTab.GaugeCheckboxChange = false;
    MovementHistoryTab.midnightRecordCheckboxChange = false;
}

//================================================================================
// This function will handle the column filter dropdown on click event. It will
// expand or collapse the dropdown based on the "hidden" class state.
//================================================================================
MovementHistoryTab.HandleColumnFilterDropdownExpandCollapse = function ()
{
    var hiddenClass = $("#MovementHistoryColumnFilterDiv").attr('class');

    if (hiddenClass === "")
    {
        $("#MovementHistoryColumnFilterDiv").addClass('hidden');

        if (MovementHistoryTab.checkboxFilterArray != null && MovementHistoryTab.checkboxFilterArray.length > 0)
        {
            MovementHistoryTab.changingincludedcolumns = true;
            var table = MovementHistoryTab.DatatableHandle;
            var visability = MovementHistoryTab.GetVisibilityColumnReorder(table);
            var VisibleArr = visability.VisibleArr;
            var InvisibleArr = visability.InvisibleArr;
            for (var nextIndex = 0; nextIndex < MovementHistoryTab.checkboxFilterArray.length; nextIndex++)
            {
                var control = MovementHistoryTab.checkboxFilterArray[nextIndex];
                var currentColIndex = table.colReorder.transpose(control.columnIndex);
                var column = table.column(currentColIndex);
                if (control.checked && !VisibleArr.includes(control.columnIndex)) {
                    VisibleArr.push(control.columnIndex);
                    const index = InvisibleArr.indexOf(control.columnIndex);
                    if (index > -1) { // only splice array when item is found
                        InvisibleArr.splice(index, 1); // 2nd parameter means remove one item only
                    }
                    column.visible(true, false);
                } else if (!control.checked && !InvisibleArr.includes(control.columnIndex)) {
                    InvisibleArr.push(control.columnIndex);
                    const index = VisibleArr.indexOf(control.columnIndex);
                    if (index > -1) { // only splice array when item is found
                        VisibleArr.splice(index, 1); // 2nd parameter means remove one item only
                    }
                    column.visible(false, false);
                }
                MovementHistoryTab.UpdateTdId(control.columnIndex);
            }
            table.colReorder.order([...VisibleArr, ...InvisibleArr], true);
            table.columns.adjust().draw(false);
            MovementHistoryTab.DoFindLogic();
            MovementHistoryTab.ResetSelectAllCheckbox();
            MovementHistoryTab.ResizeColumns();
            MovementHistoryTab.checkboxFilterArray = [];
        }
        MovementHistoryTab.changingincludedcolumns = false;
        MovementHistoryTab.SaveViewState();
    }
    else
    {
        $("#MovementHistoryColumnFilterDiv").removeClass('hidden');
        MovementHistoryTab.checkboxFilterArray = [];
    }
}

//================================================================================
// This function will initialize all the column filter checkboxes to a checked
// state.  It is called by the MovementHistoryTabView.cshtml document ready event.
//================================================================================
MovementHistoryTab.InitializeColumnFilterDropdownCheckboxes = function (hideCols) {
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }
    var table = $("#MovementHistoryTable").DataTable();

    if (hideCols) {
        var numberOfColumns = $("#MovementHistoryTable thead th").length;

        for (var nextColIndex = 3; nextColIndex <= numberOfColumns - 1; nextColIndex++) {
            MovementHistoryTab.ToggleColumnVisibility(table, false, nextColIndex);
        }
    }

    $('#MovementHistoryColumnFilterUl > li > label > input').each(function ()
    {
        var inputId = $(this).attr("id");

        if (typeof (inputId) != "undefined")
        {
            let currentColIndex;
            if (inputId === "MovementHistoryDateTimeCheckbox")
            {
                $(this).attr('checked', 'checked');
                currentColIndex = table.colReorder.transpose(MovementHistoryTab.timeStampColumnIndex);
                MovementHistoryTab.ToggleColumnVisibility(table, true, currentColIndex);
            }

            if (inputId === "MovementNameCheckbox")
            {
                $(this).attr('checked', 'checked');
                currentColIndex = table.colReorder.transpose(MovementHistoryTab.movementNameColumnIndex);
                MovementHistoryTab.ToggleColumnVisibility(table, true, currentColIndex);
            }

            if (inputId === "MovementNodeCheckbox")
            {
                $(this).attr('checked', 'checked');
                currentColIndex = table.colReorder.transpose(MovementHistoryTab.movementNodeColumnIndex);
                MovementHistoryTab.ToggleColumnVisibility(table, true, currentColIndex);
            }

            if (inputId === "InitiationCountCheckbox")
            {
                $(this).attr('checked', 'checked');
                currentColIndex = table.colReorder.transpose(MovementHistoryTab.initiationCountColumnIndex);
                MovementHistoryTab.ToggleColumnVisibility(table, true, currentColIndex);
            }
        }
    });
}

//====================================================================
// This function handles the comment styles.
//====================================================================
MovementHistoryTab.StyleComments = function ()
{
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }
    var table = $("#MovementHistoryTable").DataTable();
    var commentColIndex = table.colReorder.transpose(MovementHistoryTab.commentColumnIndex);
    var commentCol = table.column(commentColIndex);

    // if comment column is visible 
    if (commentCol && commentCol.visible())
    {
        var c = table.cells(null, commentColIndex);
        c.every(function ()
        {
            var id = this.node().id;

            if (MovementHistoryTab.inEditMode === false || MovementHistoryTab.editModeId !== id)
            {
                var n = $(this.node());

                if (n && n.html() !== "")
                {
                    var movementHistoryRecordGuid = id.replace(MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentColumnIndex), "");
                    var innerHtmlId = MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentIsEditableSpecialIndex) + movementHistoryRecordGuid;

                    n.html("<label class=\"MovementHistoryShowCommentIsEditable\" id=\"" + innerHtmlId + "\">" + n.html() + "</label>");
                    var rawComment = MovementHistoryTab.GetComment(movementHistoryRecordGuid);
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
MovementHistoryTab.TablePageChanged = function ()
{
    MovementHistoryTab.DoFindLogic();

    // Ensure that when the page has completed a draw to reset
    // the highlighted sort column.
    MovementHistoryTab.ColorSortColumn();

    // Hide the column filter dropdown on table draw complete.
    $("#MovementHistoryColumnFilterDiv").addClass('hidden');

    //Don't need the below line because it is in MovementHistoryTab.DoFindLogic
    //MovementHistoryTab.StyleComments();
}

//==============================================================
// This function creates a new div for sizing.
//==============================================================
MovementHistoryTab.Newdiv = function (force)
{
    var newDiv = $("#MovementHistoryFindResultsRow");

    if (MovementHistoryTab.FindArr && MovementHistoryTab.FindArr.length > 0)
    {
        if ($(newDiv).is(':hidden') || force)
        {
            $(newDiv).removeClass('hidden');
        }
    }
    else
    {
        $(newDiv).removeClass('hidden').addClass('hidden');
    }

}

//=======================================================================================
// This function will refresh the data table based on the filters.
//=======================================================================================
MovementHistoryTab.Refresh = function ()
{
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }
    var table = $("#MovementHistoryTable").dataTable();
    table.fnPageChange(0);
}

//=========================================================================================
// This function will handle the Refresh button refresh event. It will set the
// To date to the current date time.
//=========================================================================================
MovementHistoryTab.HandleColumnFilterRefresh = function ()
{
    var numFormatInfoString = $("#NumberFormatInfoString").val();
    var numFormatInfo = JSON.parse(numFormatInfoString);
    var timezoneOffsetStr = $("#TimezoneOffsetString").val();
    var timezoneOffset = parseFloat(timezoneOffsetStr) / 60.0;
    var currentToDate = ConvertToSiteTimezone(new Date(), timezoneOffset);

    var dateTimeFormatStr = numFormatInfo.ShortDatePattern.toUpperCase() + " " + ConvertToMomentUITimeFormat(numFormatInfo.TimePattern);
    var momentToStr = GetMomentDateTimeFormattedStr(currentToDate);
    $('#MovementHistoryColumnFilterToDateInput').datetimepicker("setDate", moment.utc(momentToStr).format(dateTimeFormatStr));

    MovementHistoryTab.HandleAvailableFilterDateChangeEvent("TO");

    MovementHistoryTab.previousColumnFilterCollection = MovementHistoryTab.CopyColumnFilterInfo(MovementHistoryTab.columnFilterCollection);
    MovementHistoryTab.Refresh();
}

//=========================================================================================
// This function will handle the column filter modal OK button refresh.  It will save
// the current filters which will be used when the user cancel filtering to reset the
// filters.
//=========================================================================================
MovementHistoryTab.HandleSaveCurrentFilterAndRefresh = function ()
{
    MovementHistoryTab.previousColumnFilterCollection = MovementHistoryTab.CopyColumnFilterInfo(MovementHistoryTab.columnFilterCollection);
    MovementHistoryTab.SaveViewState();
    MovementHistoryTab.Refresh();
}

//===========================================================================================
// This function will handle the column filter modal cancel. It will reset the column
// filter collection back what it was previously.
//===========================================================================================
MovementHistoryTab.HandleCancelFiltering = function ()
{
    if (MovementHistoryTab.previousColumnFilterCollection != null)
    {
        MovementHistoryTab.columnFilterCollection = MovementHistoryTab.CopyColumnFilterInfo(MovementHistoryTab.previousColumnFilterCollection);

        for (var nextFilterIndex = 0; nextFilterIndex < MovementHistoryTab.columnFilterCollection.length; nextFilterIndex++)
        {
            var filterObj = MovementHistoryTab.columnFilterCollection[nextFilterIndex];

            if (filterObj.Index === MovementHistoryTab.timeStampColumnIndex)
            {
                $('#MovementHistoryColumnFilterFromDateInput').val(filterObj.FromDateStr);
                $('#MovementHistoryColumnFilterToDateInput').val(filterObj.ToDateStr);
            }

            if (filterObj.Index === MovementHistoryTab.commentDateTimeColumnIndex)
            {
               $('#MovementHistoryColumnFilterCommentFromDateInput').val(filterObj.CommentFromDateStr);
               $('#MovementHistoryColumnFilterCommentToDateInput').val(filterObj.CommentToDateStr);
            }

           if (filterObj.Index === MovementHistoryTab.plannedStartTimeColumnIndex)
           {
              $('#MovementHistoryColumnFilterPlannedStartTimeFromDateInput').val(filterObj.PlannedStartTimeFromDateStr);
              $('#MovementHistoryColumnFilterPlannedStartTimeToDateInput').val(filterObj.PlannedStartTimeToDateStr);
           }
       }
    }
}

//=============================================================================================
// This function will copy the filter collection to a new object and return it.
//=============================================================================================
MovementHistoryTab.CopyColumnFilterInfo = function (fromFilterCollection)
{
    var toFilterCollection = [];

    for (var nextFilterIndex = 0; nextFilterIndex < fromFilterCollection.length; nextFilterIndex++)
    {
        var toFilter = MovementHistoryTab.CreateAvailableFilterObject();
        var filterObj = fromFilterCollection[nextFilterIndex];

        toFilter.Name               = filterObj.Name;
        toFilter.Index              = filterObj.Index;
        toFilter.FromDateStr        = filterObj.FromDateStr;
        toFilter.ToDateStr          = filterObj.ToDateStr;
        toFilter.CommentFromDateStr = filterObj.CommentFromDateStr;
        toFilter.CommentToDateStr = filterObj.CommentToDateStr;
        toFilter.PlannedStartTimeFromDateStr = filterObj.PlannedStartTimeFromDateStr;
        toFilter.PlannedStartTimeToDateStr = filterObj.PlannedStartTimeToDateStr;
        toFilter.ShowAutoGauge      = filterObj.ShowAutoGauge;
        toFilter.ShowHandGauge      = filterObj.ShowHandGauge;
        toFilter.ShowMidnightRecord = filterObj.ShowMidnightRecord;

        for (var nextInfoIndex = 0; nextInfoIndex < filterObj.FilterCollection.length; nextInfoIndex++)
        {
            toFilter.FilterCollection.push(filterObj.FilterCollection[nextInfoIndex]);
        }

        toFilterCollection.push(toFilter);
    }

    return toFilterCollection;
}

//==========================================================================================
// This function will handle the right click on the table header.
//==========================================================================================
MovementHistoryTab.HandleTableHeaderOnContextMenuEvent = function (columnIndex)
{
    $("#MovementHistoryColumnDataFilterModalDiv").modal('show');

    // Create column data filter dropdown options.
    MovementHistoryTab.CreateChooseColumnDropdownEntries(columnIndex);
}

//================================================================================================
// This function will create the entries in the Column Data Filter dropdown. It will have
// the original column index in the value field.
//================================================================================================
MovementHistoryTab.CreateChooseColumnDropdownEntries = function (previousSelection)
{
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }
    var table = $("#MovementHistoryTable").DataTable();
    var numberOfColumns = 12;

    var columnDataFilterSelectControl = document.getElementById("MovementHistoryChooseColumnDropdownId");
    var selectOptLength = columnDataFilterSelectControl.length - 1;

    // Clear the dropdown list with the exception of the None entry.
    for (var nextOptIndex = selectOptLength; nextOptIndex > 0; nextOptIndex--)
    {
        columnDataFilterSelectControl.remove(nextOptIndex);
    }

    // Only add the column names that are visible and reset the previous selection.
    for (var nextColumnIndex = 0; nextColumnIndex < numberOfColumns; nextColumnIndex++)
    {
        if (table.column(nextColumnIndex).visible())
        {
            var columnName = table.column(nextColumnIndex).header().innerText;
            var currentColIndex = table.column(nextColumnIndex).index();

            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.timeStampColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.movementNameColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.movementNodeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.initiationCountColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.siteColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.commentColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutDataModifiedByColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutDensityProductInAirColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutDensityProductObservedColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutDensityProductObservedTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutDensityProductStandardColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutDensityProductStandardTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutDensityProductStandardInAirColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutLevelProductColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutLevelProductTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutLevelWaterColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutMassLiquidColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutPercentBswColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutRoofMassColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutTankShellCorrectionColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutTemperatureAmbientColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutTemperatureAmbientTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutTemperatureDensityColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutTemperatureProductColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutTransferGovColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutTransferNsvColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutTransferMassLiquidColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutTransferVolumeWaterColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutVolumeBswColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutVolumeCorrectionFactorColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutVolumeGrossObservedColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutVolumeGrossStandardColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutVolumeNetStandardColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutVolumeRoofCorrectionColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutVolumeTotalObservedColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.closeoutVolumeWaterColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.levelProductColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.typeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.orderNumberColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.plannedStartTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.productColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.productDescriptionColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startDensityProductObservedColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startDensityProductObservedTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startDensityProductObservedInAirColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startDensityProductStandardColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startDensityProductStandardTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startDensityProductStandardInAirColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startUserIdColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startLevelProductColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startLevelProductTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startLevelWaterColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startLevelWaterTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startMassLiquidColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startPercentBswColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startTankShellCorrectionColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startTemperatureAmbientColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startTemperatureAmbientTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startTemperatureProductColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startTemperatureProductTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startTemperatureDensityColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startTemperatureDensityTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startVolumeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startVolumeBswColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startVolumeCorrectionFactorColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startVolumeGrossObservedColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startVolumeGrossStandardColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startVolumeNetStandardColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startVolumeRoofCorrectionColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startVolumeTotalObservedColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.startVolumeWaterColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.stopTimeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.statusColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferDeviationColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferPercentDeviationColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferDirectionColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferModeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferStatusColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferTargetColumnIndex);
			   MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferTargetUnitsColumnIndex);
			   MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferLevelTargetColumnIndex);
			   MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferVolumeTargetColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferTimeRemainingColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferredVolumeWaterColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.transferredVolumeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.unitsLevelProductColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.unitsTemperatureAmbientColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.unitsTemperatureDensityColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.unitsTemperatureProductColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.unitsDensityProductObservedColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.unitsDensityProductStandardColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.unitsVolumeColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.unitsMassColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.userData01ColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.userData02ColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.userData03ColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.userData04ColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.userData05ColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.userData06ColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.userData07ColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.userData08ColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.userData09ColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.userData10ColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.volumeWaterColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.commentUserNameColumnIndex);
            MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper(columnDataFilterSelectControl, table, columnName, currentColIndex, previousSelection, MovementHistoryTab.commentDateTimeColumnIndex);
        }
    }

    // Add and/or remove available filter from the collection based on the visible columns.
    MovementHistoryTab.AddRemoveAvailableFilterFromCollection();
    MovementHistoryTab.HandleChooseColumnDropdownEvent();
}

//================================================================================================
// This function is a helper for the create the entries in the Column Data Filter dropdown.
//================================================================================================
MovementHistoryTab.CreateChooseColumnDropdownEntriesHelper = function (columnDataFilterSelectControl,
                                                                        table,
                                                                        columnName,
                                                                        currentColIndex,
                                                                        previousSelection,
                                                                        originalColumnIndex)
{
    var transposedColIndex = table.colReorder.transpose(originalColumnIndex);

    if (transposedColIndex === currentColIndex)
    {
        var optionElement = document.createElement("option");
        optionElement.innerHTML = columnName;
        optionElement.value = originalColumnIndex;
        columnDataFilterSelectControl.appendChild(optionElement);

        if (previousSelection === originalColumnIndex)
        {
            $("#MovementHistoryChooseColumnDropdownId").val(originalColumnIndex.toString());
        }
    }
}

//==========================================================================================
// This function will initial the date time column filter with the initial date.  It is
// call by the MovementHistoryTabView.cshtml.
//==========================================================================================
MovementHistoryTab.SetInitialDateFilters = function ()
{
    if (MovementHistoryTab.columnFilterCollection == null || MovementHistoryTab.columnFilterCollection.length === 0)
    {
        MovementHistoryTab.columnFilterCollection = [];

        var columnFilterObj = MovementHistoryTab.CreateAvailableFilterObject();
        columnFilterObj.Index = MovementHistoryTab.timeStampColumnIndex;
        columnFilterObj.Name = "TimeStamp";

        MovementHistoryTab.columnFilterCollection.push(columnFilterObj);

        // Need the initial site filter with an empty filter collection.
        columnFilterObj = MovementHistoryTab.CreateAvailableFilterObject();
        columnFilterObj.Index = MovementHistoryTab.siteColumnIndex;
        columnFilterObj.Name = "Site";

        MovementHistoryTab.columnFilterCollection.push(columnFilterObj);

        // The record type is tied to the show/hide option data dropdown filter.
        columnFilterObj = MovementHistoryTab.CreateAvailableFilterObject();
        columnFilterObj.Index = MovementHistoryTab.recordTypeColumnIndex;
        columnFilterObj.Name = "RecordType";

        MovementHistoryTab.columnFilterCollection.push(columnFilterObj);

        // The midnight is tied to the show/hide option data dropdown filter.
        columnFilterObj = MovementHistoryTab.CreateAvailableFilterObject();
        columnFilterObj.Index = MovementHistoryTab.midnightRecordColumnIndex;
        columnFilterObj.Name = "MidnightRecord";

        MovementHistoryTab.columnFilterCollection.push(columnFilterObj);
    }

    MovementHistoryTab.HandleAvailableFilterDateChangeEvent("FROM");
    MovementHistoryTab.HandleAvailableFilterDateChangeEvent("TO");

    // Persist the column filter collection settings for when the user makes changes
    // and cancels the changes.
    MovementHistoryTab.previousColumnFilterCollection = MovementHistoryTab.CopyColumnFilterInfo(MovementHistoryTab.columnFilterCollection);
}

//===============================================================================================
// This function will add and/or remove availabe filter objects from the collection.  It is based
// on the visible columns which are loaded in the column filter modal dialog.
//===============================================================================================
MovementHistoryTab.AddRemoveAvailableFilterFromCollection = function ()
{
    if (MovementHistoryTab.columnFilterCollection == null || MovementHistoryTab.columnFilterCollection.length === 0)
    {
        MovementHistoryTab.columnFilterCollection = [];

        $("#MovementHistoryChooseColumnDropdownId > option").each(function ()
        {
            var name = $(this).text();
            var index = $(this).val();
            var columnIndex = parseInt(index);

            if (columnIndex !== -99)
            {
                var columnFilterObj = MovementHistoryTab.CreateAvailableFilterObject();
                columnFilterObj.Name = name;
                columnFilterObj.Index = columnIndex;

                MovementHistoryTab.columnFilterCollection.push(columnFilterObj);
            }
        });
    }
    else
    {
        var found;
        var nextFilterIndex;
        var selectionIndexList = [];

        // Add new column filter to collection.
        $("#MovementHistoryChooseColumnDropdownId > option").each(function ()
        {
            var name = $(this).text();
            var index = $(this).val();
            var columnIndex = parseInt(index);

            if (columnIndex !== -99)
            {
                selectionIndexList.push(columnIndex);
                found = false;

                for (nextFilterIndex = 0; nextFilterIndex < MovementHistoryTab.columnFilterCollection.length; nextFilterIndex++)
                {
                    if (MovementHistoryTab.columnFilterCollection[nextFilterIndex].Index === columnIndex)
                    {
                        found = true;
                    }
                }

                if (found === false)
                {
                    var columnFilterObj = MovementHistoryTab.CreateAvailableFilterObject();
                    columnFilterObj.Name = name;
                    columnFilterObj.Index = columnIndex;

                    MovementHistoryTab.columnFilterCollection.push(columnFilterObj);
                }
            }
        });

        var startIndex = MovementHistoryTab.columnFilterCollection.length - 1;

        for (nextFilterIndex = startIndex; nextFilterIndex >= 0; nextFilterIndex--)
        {
            found = false;

            for (var nextSelectionIndex = 0; nextSelectionIndex < selectionIndexList.length; nextSelectionIndex++)
            {
                if (MovementHistoryTab.columnFilterCollection[nextFilterIndex].Index === selectionIndexList[nextSelectionIndex])
                {
                    found = true;
                }
            }

            if (found === false)
            {
                MovementHistoryTab.columnFilterCollection.splice(nextFilterIndex, 1);
            }
        }
    }
}

//=====================================================================================
// This function will handle the Choose Columns (column filter) dropdown change event.
//=====================================================================================
MovementHistoryTab.HandleChooseColumnDropdownEvent = function ()
{
    // Get the column selected.
    var selectedColumn = $("#MovementHistoryChooseColumnDropdownId").find(":selected").val();
    var selectedColumnInt = parseInt(selectedColumn);

    // For the date & time column, present a date range date/time picker
    // instead of the multi-select column data dropdown.
    if (selectedColumnInt === MovementHistoryTab.timeStampColumnIndex)
    {
        $("#MovementHistoryColumnFilterDateRowDiv").removeClass("hidden");
        $("#MovementHistoryColumnFilterCommentDateRowDiv").addClass("hidden");
        $("#MovementHistoryColumnFilterPlannedStartTimeRowDiv").addClass("hidden");
        $("#MovementHistoryAvailableFilterDropdownLabelDivId").addClass("hidden");
        $("#MovementHistoryAvailableFilterDropdownDivId").addClass("hidden");
    }
    else if (selectedColumnInt === MovementHistoryTab.commentDateTimeColumnIndex)
    {
        $("#MovementHistoryColumnFilterCommentDateRowDiv").removeClass("hidden");
        $("#MovementHistoryColumnFilterDateRowDiv").addClass("hidden");
        $("#MovementHistoryColumnFilterPlannedStartTimeRowDiv").addClass("hidden");
        $("#MovementHistoryAvailableFilterDropdownLabelDivId").addClass("hidden");
        $("#MovementHistoryAvailableFilterDropdownDivId").addClass("hidden");
    }
    else if (selectedColumnInt === MovementHistoryTab.plannedStartTimeColumnIndex) {
       $("#MovementHistoryColumnFilterPlannedStartTimeRowDiv").removeClass("hidden");
       $("#MovementHistoryColumnFilterCommentDateRowDiv").addClass("hidden");
       $("#MovementHistoryColumnFilterDateRowDiv").addClass("hidden");
       $("#MovementHistoryAvailableFilterDropdownLabelDivId").addClass("hidden");
       $("#MovementHistoryAvailableFilterDropdownDivId").addClass("hidden");
    }
    else
    {
        $("#MovementHistoryColumnFilterDateRowDiv").addClass("hidden");
        $("#MovementHistoryColumnFilterCommentDateRowDiv").addClass("hidden");
        $("#MovementHistoryColumnFilterPlannedStartTimeRowDiv").addClass("hidden");
        $("#MovementHistoryAvailableFilterDropdownLabelDivId").removeClass("hidden");
        $("#MovementHistoryAvailableFilterDropdownDivId").removeClass("hidden");

        MovementHistoryTab.ClearSelectionInAvailableFiltersDropdown();
        MovementHistoryTab.CreateColumnAvailableFilterDropdown();

        MovementHistoryTab.ResetSelectedAvailableFilters(selectedColumnInt);
    }
}

//==========================================================================
// This function will clear the selected items from the column filter
// data dropdown. It is called when the column filter has changed.
//==========================================================================
MovementHistoryTab.ClearSelectionInAvailableFiltersDropdown = function ()
{
    // Must have this line or the empty() will not work.
    $("#MovementHistoryAvailableFilterDropdownId").select2('val');
    $("#MovementHistoryAvailableFilterDropdownId").empty();
}

//=================================================================================
// This function will reset the available filter selection.
//=================================================================================
MovementHistoryTab.ResetSelectedAvailableFilters = function (selectedColumnIndex)
{
    for (var nextColumn = 0; nextColumn < MovementHistoryTab.columnFilterCollection.length; nextColumn++)
    {
        var columnObj = MovementHistoryTab.columnFilterCollection[nextColumn];

        if (columnObj.Index === selectedColumnIndex)
        {
            var selectedValues = [];
            for (var nextFilter = 0; nextFilter < columnObj.FilterCollection.length; nextFilter++)
            {
                var filter = columnObj.FilterCollection[nextFilter];
                selectedValues.push(filter);
            }

            if (selectedValues.length > 0)
            {
                $('#MovementHistoryAvailableFilterDropdownId').val(selectedValues).trigger('change.select2');
            }
        }
    }
}

//=============================================================================
// This function creates the available filter object that contains the filters
// for each of the selected columns.
//=============================================================================
MovementHistoryTab.CreateAvailableFilterObject = function ()
{
    var columnFilterObject = new Object();
    columnFilterObject.Name = "";
    columnFilterObject.Index = -99;
    columnFilterObject.FilterCollection = [];
    columnFilterObject.FromDateStr = "";
    columnFilterObject.ToDateStr = "";
    columnFilterObject.CommentFromDateStr = "";
    columnFilterObject.CommentToDateStr = "";
    columnFilterObject.PlannedStartTimeFromDateStr = "";
    columnFilterObject.PlannedStartTimeToDateStr = "";
    columnFilterObject.ShowAutoGauge = false;
    columnFilterObject.ShowHandGauge = false;
    columnFilterObject.ShowMidnightRecord = false;

    return columnFilterObject;
}

//===================================================================================
// This function will retrieve available filter data for a selected column. It passes
// the selected column index and column filter information to the server.
//===================================================================================
MovementHistoryTab.RetrieveAvailableFilters = function (selectedColumnIndex)
{
    // Ensure the timestamp filter date range is updated.
    MovementHistoryTab.UpdateTimeStampColumnFilter();

    var filterDataList = null;
    var getColumnFilterDataUrl = $("#MovementHistoryGetColumnFilterDataUrl").val();
    var token = $('#MovementHistoryTabView input[name =__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    PNotify.removeStack(MovementHistoryTab.messageAttributes.stack);

    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: "json",
        url: getColumnFilterDataUrl,
        headers: headers,
        data: JSON.stringify({ selectedColumn: selectedColumnIndex, filterInfo: MovementHistoryTab.columnFilterCollection }),
        success: function (dataList)
        {
            filterDataList = dataList;
        },
        error: function (e)
        {
            FMErrorAndExceptionHandling.ShowError('Error retrieving column filter data.', null, MovementHistoryTab.messageAttributes);
        }
    });

    return filterDataList;
}

//==================================================================
// This function will update the TimeStamp column filter with the
// current settings.
//==================================================================
MovementHistoryTab.UpdateTimeStampColumnFilter = function ()
{
    var timeStampColFilter = MovementHistoryTab.columnFilterCollection[0];

    timeStampColFilter.FromDateStr = $("#StartTimePicker").val();
    timeStampColFilter.ToDateStr = $("#EndTimePicker").val();
};

//===========================================================================================
// This function will load the available filter with available data based on the
// column selected.
//===========================================================================================
MovementHistoryTab.CreateColumnAvailableFilterDropdown = function ()
{
    // Get the column selected.
    var selectedColumn = $("#MovementHistoryChooseColumnDropdownId").find(":selected").val();
    var columnAvailableFilterControl = document.getElementById("MovementHistoryAvailableFilterDropdownId");
    var selectedColumnInt = parseInt(selectedColumn);

    if (selectedColumn === "-99")
    {
        MovementHistoryTab.ClearAvailableFilterDropdownEntries();
        return;
    }

    // Retrieve filter data from server.
    var filterDataList = MovementHistoryTab.RetrieveAvailableFilters(selectedColumnInt);

    // Load dropdown selections.
    if (filterDataList != null && filterDataList.length > 0)
    {
        MovementHistoryTab.ClearAvailableFilterDropdownEntries();

        for (var nextItem = 0; nextItem < filterDataList.length; nextItem++)
        {
            var filterData = filterDataList[nextItem];
            MovementHistoryTab.CreateColumnAvailableFilterDropdownHelper(columnAvailableFilterControl, filterData);
        }
    }
}

//===========================================================================================
// This function is a helper for the create available filter dropdown. It will create the
// option items.
//===========================================================================================
MovementHistoryTab.CreateColumnAvailableFilterDropdownHelper = function (columnAvailableFilterControl, filterData)
{
    var displayValue = filterData;
    var actualValue = filterData;

    // For level that are in feet/inches, the display value is different from the actual
    // value.
    if (filterData.indexOf("LV|") >= 0)
    {
        var parts = filterData.split("|");

        if (parts.length === 3)
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

    columnAvailableFilterControl.appendChild(optionElement);
}

//==============================================================================
// This function will clear all the option entries in the available filters
// select.
//==============================================================================
MovementHistoryTab.ClearAvailableFilterDropdownEntries = function ()
{
    $("#MovementHistoryAvailableFilterDropdownId > option").each(function ()
    {
        $(this).remove();
    });
}

//================================================================================
// This function will handle the clear all filters button event.
//================================================================================
MovementHistoryTab.HandleClearAllFiltersBtnEvent = function ()
{
   if (MovementHistoryTab.columnFilterCollection != null)
    {
        for (var nextFilterIndex = 0; nextFilterIndex < MovementHistoryTab.columnFilterCollection.length; nextFilterIndex++)
        {
            var columnFilterObj = MovementHistoryTab.columnFilterCollection[nextFilterIndex];
            columnFilterObj.CommentFromDateStr = "";
            columnFilterObj.CommentToDateStr = "";
            columnFilterObj.PlannedStartTimeFromDateStr = "";
            columnFilterObj.PlannedStartTimeToDateStr = "";
            columnFilterObj.FilterCollection = [];
        }
        // This will clear the selected filters in view.
        var selectedValues = [];
        $('#MovementHistoryAvailableFilterDropdownId').val(selectedValues).trigger('change.select2');

        $("#MovementHistoryColumnFilterCommentFromDateInput").val("");
        $("#MovementHistoryColumnFilterCommentToDateInput").val("");
    }
}

//==================================================================================
// This function handles the available filter event when an item is selected.
// It will add the filter item to the appropriate column filter list.
//==================================================================================
MovementHistoryTab.HandleAvailableFilterDropdownSelectEvent = function (evnt)
{
    // Get the column selected.
    var selectedColumn = $("#MovementHistoryChooseColumnDropdownId").find(":selected").val();
    var selectedColumnInt = parseInt(selectedColumn);

    if (selectedColumn === "-99")
    {
        return;
    }

    var selectedFilter = evnt.params.args.data.id;

    for (var next = 0; next < MovementHistoryTab.columnFilterCollection.length; next++)
    {
        var columnObj = MovementHistoryTab.columnFilterCollection[next];

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
MovementHistoryTab.HandleAvailableFilterDropdownUnselectEvent = function (evnt)
{
    // Get the column selected.
    var selectedColumn = $("#MovementHistoryChooseColumnDropdownId").find(":selected").val();
    var selectedColumnInt = parseInt(selectedColumn);

    if (selectedColumn === "-99")
    {
        return;
    }

    var unSelectedFilter = evnt.params.data.id;

    for (var next = 0; next < MovementHistoryTab.columnFilterCollection.length; next++)
    {
        var columnObj = MovementHistoryTab.columnFilterCollection[next];

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
MovementHistoryTab.HandleAvailableFilterDateChangeEvent = function (dateType)
{
    var filterObj = null;

    for (var nextFilter = 0; nextFilter < MovementHistoryTab.columnFilterCollection.length; nextFilter++)
    {
        if (MovementHistoryTab.columnFilterCollection[nextFilter].Index === MovementHistoryTab.timeStampColumnIndex)
        {
            filterObj = MovementHistoryTab.columnFilterCollection[nextFilter];
        }
    }

    if (filterObj == null)
    {
        FMErrorAndExceptionHandling.ShowError('Could not find date filter object.', null, MovementHistoryTab.messageAttributes);
        return;
    }

    if (dateType === "FROM")
    {
        var fromDateStr = $("#MovementHistoryColumnFilterFromDateInput").val();
        filterObj.FromDateStr = fromDateStr;

        // Also update the start date at the top of the tab with the same date.
        $("#StartTimePicker").val(fromDateStr);
    }

    if (dateType === "TO")
    {
        var toDateStr = $("#MovementHistoryColumnFilterToDateInput").val();
        filterObj.ToDateStr = toDateStr;

        // Also update the end date at the top of the tab with the same date.
        $("#EndTimePicker").val(toDateStr);
    }
}

//====================================================================================================
// This function will update the filter collection with the comment date change.
//====================================================================================================
MovementHistoryTab.HandleAvailableFilterCommentDateChangeEvent = function (dateType)
{
    var filterObj = null;

    for (var nextFilter = 0; nextFilter < MovementHistoryTab.columnFilterCollection.length; nextFilter++)
    {
        if (MovementHistoryTab.columnFilterCollection[nextFilter].Index === MovementHistoryTab.commentDateTimeColumnIndex)
        {
            filterObj = MovementHistoryTab.columnFilterCollection[nextFilter];
        }
    }

    if (filterObj == null)
    {
        FMErrorAndExceptionHandling.ShowError('Could not find comment date filter object.', null, MovementHistoryTab.messageAttributes);
        return;
    }

    if (dateType === "FROM")
    {
        var fromDateStr = $("#MovementHistoryColumnFilterCommentFromDateInput").val();
        filterObj.CommentFromDateStr = fromDateStr;
    }

    if (dateType === "TO")
    {
        var toDateStr = $("#MovementHistoryColumnFilterCommentToDateInput").val();
        filterObj.CommentToDateStr = toDateStr;
    }
}

//====================================================================================================
// This function will update the filter collection with the Planned Start date time change.
//====================================================================================================
MovementHistoryTab.HandleAvailableFilterPlannedStartTimeChangeEvent = function (dateType) {
   var filterObj = null;

   for (var nextFilter = 0; nextFilter < MovementHistoryTab.columnFilterCollection.length; nextFilter++) {
      if (MovementHistoryTab.columnFilterCollection[nextFilter].Index === MovementHistoryTab.plannedStartTimeColumnIndex) {
         filterObj = MovementHistoryTab.columnFilterCollection[nextFilter];
      }
   }

   if (filterObj == null) {
      FMErrorAndExceptionHandling.ShowError('Could not find planned start time filter object.', null, MovementHistoryTab.messageAttributes);
      return;
   }

   if (dateType === "FROM") {
      var fromDateStr = $("#MovementHistoryColumnFilterPlannedStartTimeFromDateInput").val();
      filterObj.PlannedStartTimeFromDateStr = fromDateStr;
   }

   if (dateType === "TO") {
      var toDateStr = $("#MovementHistoryColumnFilterPlannedStartTimeToDateInput").val();
      filterObj.PlannedStartTimeToDateStr = toDateStr;
   }
}
//============================================================================================
// This function will initialize the date pickers for the column filtering.
//============================================================================================
MovementHistoryTab.InitializeDatePickers = function ()
{
    var numFormatInfoString = $("#NumberFormatInfoString").val();
    var numFormatInfo = JSON.parse(numFormatInfoString);
    FMLayout.dateFormat = ConvertToJQueryUIDateFormat(numFormatInfo.ShortDatePattern);
    FMLayout.timeFormat = ConvertToJQueryUITimeFormat(numFormatInfo.TimePattern);

    $('#MovementHistoryColumnFilterFromDateInput').datetimepicker({
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
                MovementHistoryTab.HandleAvailableFilterDateChangeEvent("FROM");
            }
        }
    });

    $('#MovementHistoryColumnFilterToDateInput').datetimepicker({
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
                MovementHistoryTab.HandleAvailableFilterDateChangeEvent("TO");
            }
        }
    });

    $('#MovementHistoryColumnFilterCommentFromDateInput').datetimepicker({
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
                MovementHistoryTab.HandleAvailableFilterCommentDateChangeEvent("FROM");
            }
        }
    });

    $('#MovementHistoryColumnFilterCommentToDateInput').datetimepicker({
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
                MovementHistoryTab.HandleAvailableFilterCommentDateChangeEvent("TO");
            }
        }
    });

   $('#MovementHistoryColumnFilterPlannedStartTimeFromDateInput').datetimepicker({
      buttonImage: FMLayout.calendarLocation + '/calendar.gif',
      buttonImageOnly: true,
       showOn: "button",
       showTimezone: false,
       useLocalTimezone: false,
       defaultTimezone: $("#datepickerTimezoneString").val(),
      dateFormat: FMLayout.dateFormat,
      timeFormat: FMLayout.timeFormat,
      beforeShow: function () {
         setTimeout(function () {
            $('.ui-datepicker').css('z-index', 1100);
         }, 0);
      },
      onSelect: function (d, i) {
         if (d !== i.lastVal) {
            $(this).change();
            MovementHistoryTab.HandleAvailableFilterPlannedStartTimeChangeEvent("FROM");
         }
      }
   });

   $('#MovementHistoryColumnFilterPlannedStartTimeToDateInput').datetimepicker({
      buttonImage: FMLayout.calendarLocation + '/calendar.gif',
      buttonImageOnly: true,
       showOn: "button",
       showTimezone: false,
       useLocalTimezone: false,
       defaultTimezone: $("#datepickerTimezoneString").val(),
      dateFormat: FMLayout.dateFormat,
      timeFormat: FMLayout.timeFormat,
      beforeShow: function () {
         setTimeout(function () {
            $('.ui-datepicker').css('z-index', 1100);
         }, 0);
      },
      onSelect: function (d, i) {
         if (d !== i.lastVal) {
            $(this).change();
            MovementHistoryTab.HandleAvailableFilterPlannedStartTimeChangeEvent("TO");
         }
      }
   });
   //
   $('#StartTimePicker').datetimepicker({
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
                MovementHistoryTab.HandleStartDateTimeEvent();
            }
        }
    });

    $('#EndTimePicker').datetimepicker({
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
                MovementHistoryTab.HandleEndDateTimeEvent();
            }
        }
    });

    // The From date is the oldest date and the To date is the most current.
    var currentDateMinuOne = new Date();
    currentDateMinuOne.setDate(currentDateMinuOne.getDate() - 1);

    var timezoneOffsetStr = $("#TimezoneOffsetString").val();
    var timezoneOffset = parseFloat(timezoneOffsetStr) / 60.0;

    var currentFromDate = ConvertToSiteTimezone(currentDateMinuOne, timezoneOffset);
    var currentToDate = ConvertToSiteTimezone(new Date(), timezoneOffset);

    // The From date is defaulted to one day in the past from the current date.
    var dateTimeFormatStr = numFormatInfo.ShortDatePattern.toUpperCase() + " " + ConvertToMomentUITimeFormat(numFormatInfo.TimePattern);
    var momentToStr = GetMomentDateTimeFormattedStr(currentToDate);
    var momentFromStr = GetMomentDateTimeFormattedStr(currentFromDate);

    $('#MovementHistoryColumnFilterFromDateInput').datetimepicker("setDate", moment.utc(momentFromStr).format(dateTimeFormatStr));
    $('#MovementHistoryColumnFilterToDateInput').datetimepicker("setDate", moment.utc(momentToStr).format(dateTimeFormatStr));

    $('#MovementHistoryColumnFilterCommentFromDateInput').val("");
    $('#MovementHistoryColumnFilterCommentToDateInput').val("");

    $('#MovementHistoryColumnFilterPlannedStartTimeFromDateInput').val("");
    $('#MovementHistoryColumnFilterPlannedStartTimeToDateInput').val("");

    // Set the column filter object with the date.
    MovementHistoryTab.SetInitialDateFilters();
}

//////////////////////////////Find Processing///////////////////////////////////////////////////////////////////////////////////////////

//=============================================================
// This function shows and hides the find results.
//=============================================================
MovementHistoryTab.ShowHideFindResults = function (show)
{
    MovementHistoryTab.Newdiv(false);
    if (show)
    {
        MovementHistoryTab.SetFindResults();
        MovementHistoryTab.SetFindCurrentRowIndicator();
    }
    else
    {
        MovementHistoryTab.HideFindCurrentRowIndicator();
    }
};

//================================================================
// This function hides the find current row indicator.
//================================================================
MovementHistoryTab.HideFindCurrentRowIndicator = function ()
{
    var table = document.getElementById("MovementHistoryTable");
    var td = table.getElementsByTagName("td");
    if (td)
    {
        for (var i = 0; i < td.length; i++)
        {
            td[i].classList.remove("MovementHistoryCurrentlySelectedCell");
        }
    }
}

//===================================================================
// This function set the find current row indicator.
//===================================================================
MovementHistoryTab.SetFindCurrentRowIndicator = function ()
{
    MovementHistoryTab.HideFindCurrentRowIndicator();
    if (MovementHistoryTab.CurrentFind)
    {
        var currentField = document.getElementById(MovementHistoryTab.CurrentFind);

        if (currentField)
        {
            currentField.classList.add("MovementHistoryCurrentlySelectedCell");
        }
    }
}

//=======================================================================
// This function set the finds results.
//=======================================================================
MovementHistoryTab.SetFindResults = function ()
{
    var numFindResults = 0;

    if (MovementHistoryTab.FindArr)
    {
        numFindResults = MovementHistoryTab.FindArr.length;
    }

    var findResultsString = "<i>" + numFindResults + " results</i>";
    var findResultsLabel = document.getElementById('MovementHistoryFindResultsLabel');
    findResultsLabel.innerHTML = findResultsString;
}

//=========================================================================
// This function handles the scroll to current location.
//=========================================================================
MovementHistoryTab.ScrollToCurrent = function ()
{
    if (MovementHistoryTab.CurrentFind)
    {
        var currentFindElement = document.getElementById(MovementHistoryTab.CurrentFind);
        if (currentFindElement)
        {
            currentFindElement.scrollIntoView(true);
        }
    }
}

//==========================================================================
// This function reorders the find arrary.
//==========================================================================
MovementHistoryTab.ReorderFindArr = function ()
{
    var tempFindArr = [];
    var t = document.getElementById("MovementHistoryTable");
    var tds = t.getElementsByTagName("td");

    for (var n = 0; n < tds.length; n++)
    {
        if (MovementHistoryTab.FindArr.indexOf(tds[n].id) >= 0 && tempFindArr.indexOf(tds[n].id) < 0)
        {
            tempFindArr.push(tds[n].id);
        }
    }
    MovementHistoryTab.FindArr = tempFindArr;
}

//=======================================================================
// This function does the find worker.
//=======================================================================
MovementHistoryTab.DoFindWorker = function ()
{
    if (MovementHistoryTab.CurrentFindString && MovementHistoryTab.CurrentFindString.length > 0)
    {
        if (MovementHistoryTab.FindArr.length > 0)
        {
            MovementHistoryTab.ReorderFindArr();
            //Handle CurrentFind
            if (!MovementHistoryTab.CurrentFind || MovementHistoryTab.FindArr.indexOf(MovementHistoryTab.CurrentFind) < 0)
            {
                MovementHistoryTab.CurrentFind = MovementHistoryTab.FindArr[0];
                MovementHistoryTab.ScrollToCurrent();
            }
            MovementHistoryTab.ShowHideFindResults(true);
        }
        else
        {
            MovementHistoryTab.ShowHideFindResults(false);
        }
    }
    else
    {
        MovementHistoryTab.ShowHideFindResults(false);
    }
}

//===========================================================================
// This function does the find next.
//===========================================================================
MovementHistoryTab.FindNext = function ()
{
    if (MovementHistoryTab.FindArr && MovementHistoryTab.CurrentFind)
    {
        var currentFindIndex = MovementHistoryTab.FindArr.indexOf(MovementHistoryTab.CurrentFind);

        if (currentFindIndex >= 0 && currentFindIndex < MovementHistoryTab.FindArr.length - 1)
        {
            MovementHistoryTab.CurrentFind = MovementHistoryTab.FindArr[currentFindIndex + 1];
            MovementHistoryTab.SetFindCurrentRowIndicator();
            MovementHistoryTab.ScrollToCurrent();
        }
        else
        {
            MovementHistoryTab.CurrentFind = MovementHistoryTab.FindArr[0];
            MovementHistoryTab.SetFindCurrentRowIndicator();
            MovementHistoryTab.ScrollToCurrent();
        }
    }
    return false;
};

//===========================================================================
// This function does the find previous.
//===========================================================================
MovementHistoryTab.FindPrev = function ()
{
    if (MovementHistoryTab.FindArr && MovementHistoryTab.CurrentFind)
    {
        var currentFindIndex = MovementHistoryTab.FindArr.indexOf(MovementHistoryTab.CurrentFind);

        if (currentFindIndex > 0)
        {
            MovementHistoryTab.CurrentFind = MovementHistoryTab.FindArr[currentFindIndex - 1];
            MovementHistoryTab.SetFindCurrentRowIndicator();
            MovementHistoryTab.ScrollToCurrent();
        }
        else
        {
            MovementHistoryTab.CurrentFind = MovementHistoryTab.FindArr[MovementHistoryTab.FindArr.length - 1];
            MovementHistoryTab.SetFindCurrentRowIndicator();
            MovementHistoryTab.ScrollToCurrent();
        }

    }
    return false;
};

//===========================================================================
// This function prevents an enter submit on the form.
//===========================================================================
MovementHistoryTab.PreventEnterSubmit = function (e)
{
    //Prevent Post
    e = e || event;
    return (e.keyCode || e.which || e.charCode || 0) !== 13;
}

//===========================================================================
// This function does the find.
//===========================================================================
MovementHistoryTab.DoFind = function (e)
{
    var text = e.target.value;
    MovementHistoryTab.CurrentFindString = text;
    MovementHistoryTab.DoFindLogic();
};

//===========================================================================
// This function does the find logic.
//===========================================================================
MovementHistoryTab.DoFindLogic = function ()
{
    MovementHistoryTab.FindArr = [];
    MovementHistoryTab.CurrentFind = null;
    MovementHistoryTab.Search();
    MovementHistoryTab.DoFindWorker();
    MovementHistoryTab.StyleComments();

    // Hide the column filter dropdown on find search.
    $("#MovementHistoryColumnFilterDiv").addClass('hidden');
};

//===========================================================================
// This function does the get search dictionary worker.
//===========================================================================
MovementHistoryTab.GetSearchDictionaryWorker = function (searchDict, id, text)
{
    var result = MovementHistoryTab.DoFindHighlight(id, text);
    searchDict[id] = result;
}

//===========================================================================
// This function gets the search dictionary.
//===========================================================================
MovementHistoryTab.GetSearchDictionary = function ()
{
    var model = MovementHistoryTab.GetMovementHistoryModel();
    var movementHistList = model.MovementHistories;
    var searchDict = {};

    for (var i = 0; i < movementHistList.length; i++)
    {
        var historyRecord = movementHistList[i];

        if (historyRecord.InitiationCount == null || historyRecord.InitiationCount == undefined) {
           historyRecord.InitiationCount = 0;
        }

        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.timeStampColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TimeStampStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.movementNameColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.Name);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.movementNodeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.Node);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.initiationCountColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.InitiationCount.toString());
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.siteColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.Site);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.Comment);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDataModifiedByColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutDataModifiedBy);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductInAirColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutDensityProductInAirStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductObservedColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutDensityProductObservedStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductObservedTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutDensityProductObservedTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductStandardColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutDensityProductStandardStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductStandardTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutDensityProductStandardTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutDensityProductStandardInAirColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutDensityProductStandardInAirStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutLevelProductColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutLevelProductStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutLevelProductTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutLevelProductTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutLevelWaterColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutLevelWaterStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutMassLiquidColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutMassLiquidStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutPercentBswColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutPercentBswStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutRoofMassColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutRoofMassStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTankShellCorrectionColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutTankShellCorrectionStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTemperatureAmbientColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutTemperatureAmbientStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTemperatureAmbientTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutTemperatureAmbientTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTemperatureDensityColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutTemperatureDensityStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTemperatureProductColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutTemperatureProductStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTransferGovColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutTransferGovStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTransferNsvColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutTransferNsvStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTransferMassLiquidColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutTransferMassLiquidStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutTransferVolumeWaterColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutTransferVolumeWaterStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeBswColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutVolumeBswStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeCorrectionFactorColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutVolumeCorrectionFactorStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeGrossObservedColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutVolumeGrossObservedStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeGrossStandardColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutVolumeGrossStandardStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeNetStandardColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutVolumeNetStandardStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeRoofCorrectionColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutVolumeRoofCorrectionStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeTotalObservedColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutVolumeTotalObservedStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.closeoutVolumeWaterColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CloseoutVolumeWaterStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.levelProductColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.LevelProductStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.typeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.Type);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.orderNumberColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.OrderNumber);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.plannedStartTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.PlannedStartTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.productColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.Product);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.productDescriptionColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.ProductDescription);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductObservedColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartDensityProductObservedStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductObservedTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartDensityProductObservedTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductObservedInAirColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartDensityProductObservedInAirStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductStandardColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartDensityProductStandardStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductStandardTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartDensityProductStandardTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startDensityProductStandardInAirColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartDensityProductStandardInAirStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startUserIdColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartUserID);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startLevelProductColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartLevelProductStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startLevelProductTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartLevelProductTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startLevelWaterColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartLevelWaterStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startLevelWaterTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartLevelWaterTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startMassLiquidColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartMassLiquidStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startPercentBswColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartPercentBswStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTankShellCorrectionColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartTankShellCorrectionStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureAmbientColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartTemperatureAmbientStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureAmbientTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartTemperatureAmbientTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureProductColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartTemperatureProductStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureProductTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartTemperatureProductTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureDensityColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartTemperatureDensityStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startTemperatureDensityTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartTemperatureDensityTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartVolumeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeBswColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartVolumeBswStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeCorrectionFactorColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartVolumeCorrectionFactorStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeGrossObservedColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartVolumeGrossObservedStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeGrossStandardColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartVolumeGrossStandardStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeNetStandardColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartVolumeNetStandardStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeRoofCorrectionColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartVolumeRoofCorrectionStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeTotalObservedColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartVolumeTotalObservedStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.startVolumeWaterColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StartVolumeWaterStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.stopTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StopTimeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.statusColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.StatusStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferDeviationColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferDeviationStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferPercentDeviationColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferPercentDeviationStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferDirectionColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferDirection);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferModeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferModeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferStatusColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferStatusStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferTargetColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferTargetStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferTargetUnitsColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferTargetUnits);
		  MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferLevelTargetColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferLevelTargetStr);
		  MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferVolumeTargetColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferVolumeTargetStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferTimeRemainingColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferTimeRemainingStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferredVolumeWaterColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferredVolumeWaterStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.transferredVolumeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.TransferredVolumeStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsLevelProductColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UnitsLevelProduct);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsTemperatureAmbientColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UnitsTemperatureAmbient);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsTemperatureDensityColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UnitsTemperatureDensity);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsTemperatureProductColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UnitsTemperatureProduct);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsDensityProductObservedColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UnitsDensityProductObserved);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsDensityProductStandardColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UnitsDensityProductStandard);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsVolumeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UnitsVolume);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.unitsMassColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UnitsMass);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData01ColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UserData01);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData02ColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UserData02);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData03ColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UserData03);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData04ColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UserData04);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData05ColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UserData05);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData06ColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UserData06);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData07ColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UserData07);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData08ColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UserData08);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData09ColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UserData09);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.userData10ColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.UserData10);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.volumeWaterColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.VolumeWaterStr);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentUserNameColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CommentUserName);
        MovementHistoryTab.GetSearchDictionaryWorker(searchDict, MovementHistoryTab.GetIdPrefix(MovementHistoryTab.commentDateTimeColumnIndex) + historyRecord.MovementHistoryGuid, historyRecord.CommentDateTimeStr);
    }

    return searchDict;
}

//===========================================================================
// This function does the search.
//===========================================================================
MovementHistoryTab.Search = function ()
{
    var dict = MovementHistoryTab.GetSearchDictionary();

    var table = document.getElementById("MovementHistoryTable");
    var td = table.getElementsByTagName("td");
    if (td)
    {
        for (var i = 0; i < td.length; i++)
        {
            var id = td[i].id;
            if (MovementHistoryTab.inEditMode === false || id !== MovementHistoryTab.editModeId)
            {
                var result = dict[id];
                if (result !== undefined)
                {
                    td[i].innerHTML = result;
                }
            }
        }
    }
}

//===========================================================================
// This function does the find highlight.
//===========================================================================
MovementHistoryTab.DoFindHighlight = function (id, text)
{
    if (text && id)
    {
        if (MovementHistoryTab.CurrentFindString && MovementHistoryTab.CurrentFindString.length > 0)
        {
            var pattern = new RegExp(MovementHistoryTab.CurrentFindString, 'gi');
            var retText = text.replace(pattern, function (x)
            {
                return '<span class="MovementHistoryFindColoring">' + x + '</span>';
            });
            if (retText !== text)
            {
                MovementHistoryTab.FindArr.push(id);
            }
            return retText;
        }
    }
    return text;
}

//===========================================================================
// This function does the get visibility column re-order.
//===========================================================================
MovementHistoryTab.GetVisibilityColumnReorder = function (table)
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

//===========================================================================
// This function doess the set checkbox for a given column.
//===========================================================================
MovementHistoryTab.SetCheckBoxForColumn = function (visible, originalIndex)
{
    var checkBox = MovementHistoryTab.GetCheckBoxForColumn(originalIndex);

    if (checkBox)
    {
        checkBox.prop('checked', visible);
    }
}

//=========================================================================
// This function get the correct checkbox control. 
//=========================================================================
MovementHistoryTab.GetCheckBoxForColumn = function (originalIndex)
{
    var checkBoxId;
    switch (originalIndex)
    {
        case MovementHistoryTab.timeStampColumnIndex:
            checkBoxId = "MovementHistoryDateTimeCheckbox";
            break;
        case MovementHistoryTab.movementNameColumnIndex:
            checkBoxId = "MovementNameCheckbox";
            break;
        case MovementHistoryTab.movementNodeColumnIndex:
            checkBoxId = "MovementNodeCheckbox";
            break;
        case 999:
            return null;
        default:
            checkBoxId = "ColumnFilterCheckbox" + originalIndex.toString();
    }

    var checkBox = $('#' + checkBoxId);
    return checkBox;
}

//====================================================================================
// This function will create all the column filter checkboxes.
//====================================================================================
MovementHistoryTab.CreateAllColumnFilterCheckboxes = function ()
{
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.initiationCountColumnIndex, $("#MovementHistory_InitiationCountHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.siteColumnIndex, $("#MovementHistory_SiteHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutDataModifiedByColumnIndex, $("#MovementHistory_CloseoutDataModifiedHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutDensityProductInAirColumnIndex, $("#MovementHistory_CloseoutDensityProductInAirHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutDensityProductObservedColumnIndex, $("#MovementHistory_CloseoutDensityProductObservedHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutDensityProductObservedTimeColumnIndex, $("#MovementHistory_CloseoutDensityProductObservedTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutDensityProductStandardColumnIndex, $("#MovementHistory_CloseoutDensityProductStandardHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutDensityProductStandardTimeColumnIndex, $("#MovementHistory_CloseoutDensityProductStandardTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutDensityProductStandardInAirColumnIndex, $("#MovementHistory_CloseoutDensityProductStandardInAirHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutLevelProductColumnIndex, $("#MovementHistory_CloseoutLevelProductHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutLevelProductTimeColumnIndex, $("#MovementHistory_CloseoutLevelProductTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutLevelWaterColumnIndex, $("#MovementHistory_CloseoutLevelWaterHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutMassLiquidColumnIndex, $("#MovementHistory_CloseoutMassLiquidHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutPercentBswColumnIndex, $("#MovementHistory_CloseoutPercentBswHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutRoofMassColumnIndex, $("#MovementHistory_CloseoutRoofMassHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutTankShellCorrectionColumnIndex, $("#MovementHistory_CloseoutTankShellCorrectionHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutTemperatureAmbientColumnIndex, $("#MovementHistory_CloseoutTemperatureAmbientHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutTemperatureAmbientTimeColumnIndex, $("#MovementHistory_CloseoutTemperatureAmbientTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutTemperatureDensityColumnIndex, $("#MovementHistory_CloseoutTemperatureDensityHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutTemperatureProductColumnIndex, $("#MovementHistory_CloseoutTemperatureProductHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutTimeColumnIndex, $("#MovementHistory_CloseoutTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutTransferGovColumnIndex, $("#MovementHistory_CloseoutTransferGovHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutTransferNsvColumnIndex, $("#MovementHistory_CloseoutTransferNsvHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutTransferMassLiquidColumnIndex, $("#MovementHistory_CloseoutTransferMassLiquidHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutTransferVolumeWaterColumnIndex, $("#MovementHistory_CloseoutTransferVolumeWaterHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutVolumeBswColumnIndex, $("#MovementHistory_CloseoutVolumeBswHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutVolumeCorrectionFactorColumnIndex, $("#MovementHistory_CloseoutVolumeCorrectionFactorHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutVolumeGrossObservedColumnIndex, $("#MovementHistory_CloseoutVolumeGrossObservedHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutVolumeGrossStandardColumnIndex, $("#MovementHistory_CloseoutVolumeGrossStandardHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutVolumeNetStandardColumnIndex, $("#MovementHistory_CloseoutVolumeNetStandardHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutVolumeRoofCorrectionColumnIndex, $("#MovementHistory_CloseoutVolumeRoofCorrectionHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutVolumeTotalObservedColumnIndex, $("#MovementHistory_CloseoutVolumeTotalObservedHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.closeoutVolumeWaterColumnIndex, $("#MovementHistory_CloseoutVolumeWaterHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.commentColumnIndex, $("#MovementHistory_CommentHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.commentDateTimeColumnIndex, $("#MovementHistory_CommentDateTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.commentUserNameColumnIndex, $("#MovementHistory_CommentUserNameHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.levelProductColumnIndex, $("#MovementHistory_LevelProductHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.typeColumnIndex, $("#MovementHistory_TypeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.orderNumberColumnIndex, $("#MovementHistory_OrderNumberHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.plannedStartTimeColumnIndex, $("#MovementHistory_PlannedStartTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.productColumnIndex, $("#MovementHistory_ProductHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.productDescriptionColumnIndex, $("#MovementHistory_ProductDescriptionHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startDensityProductObservedColumnIndex, $("#MovementHistory_StartDensityProductObservedHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startDensityProductObservedTimeColumnIndex, $("#MovementHistory_StartDensityProductObservedTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startDensityProductObservedInAirColumnIndex, $("#MovementHistory_StartDensityProductObservedInAirHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startDensityProductStandardColumnIndex, $("#MovementHistory_StartDensityProductStandardHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startDensityProductStandardTimeColumnIndex, $("#MovementHistory_StartDensityProductStandardTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startDensityProductStandardInAirColumnIndex, $("#MovementHistory_StartDensityProductStandardInAirHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startLevelProductColumnIndex, $("#MovementHistory_StartLevelProductHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startLevelProductTimeColumnIndex, $("#MovementHistory_StartLevelProductTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startLevelWaterColumnIndex, $("#MovementHistory_StartLevelWaterHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startLevelWaterTimeColumnIndex, $("#MovementHistory_StartLevelWaterTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startMassLiquidColumnIndex, $("#MovementHistory_StartMassLiquidHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startPercentBswColumnIndex, $("#MovementHistory_StartPercentBswHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startTankShellCorrectionColumnIndex, $("#MovementHistory_StartTankShellCorrectionHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startTemperatureAmbientColumnIndex, $("#MovementHistory_StartTemperatureAmbientHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startTemperatureAmbientTimeColumnIndex, $("#MovementHistory_StartTemperatureAmbientTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startTemperatureProductColumnIndex, $("#MovementHistory_StartTemperatureProductHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startTemperatureProductTimeColumnIndex, $("#MovementHistory_StartTemperatureProductTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startTemperatureDensityColumnIndex, $("#MovementHistory_StartTemperatureDensityHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startTemperatureDensityTimeColumnIndex, $("#MovementHistory_StartTemperatureDensityTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startUserIdColumnIndex, $("#MovementHistory_StartUserIdHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startVolumeColumnIndex, $("#MovementHistory_StartVolumeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startVolumeBswColumnIndex, $("#MovementHistory_StartVolumeBswHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startVolumeCorrectionFactorColumnIndex, $("#MovementHistory_StartVolumeCorrectionFactorHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startVolumeGrossObservedColumnIndex, $("#MovementHistory_StartVolumeGrossObservedHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startVolumeGrossStandardColumnIndex, $("#MovementHistory_StartVolumeGrossStandardHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startVolumeNetStandardColumnIndex, $("#MovementHistory_StartVolumeNetStandardHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startVolumeRoofCorrectionColumnIndex, $("#MovementHistory_StartVolumeRoofCorrectionHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startVolumeTotalObservedColumnIndex, $("#MovementHistory_StartVolumeTotalObservedHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startVolumeWaterColumnIndex, $("#MovementHistory_StartVolumeWaterHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.stopTimeColumnIndex, $("#MovementHistory_StopTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.statusColumnIndex, $("#MovementHistory_StatusHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferDeviationColumnIndex, $("#MovementHistory_TransferDeviationHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferPercentDeviationColumnIndex, $("#MovementHistory_TransferPercentDeviationHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferDirectionColumnIndex, $("#MovementHistory_TransferDirectionHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferModeColumnIndex, $("#MovementHistory_TransferModeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.startTimeColumnIndex, $("#MovementHistory_StartTimeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferStatusColumnIndex, $("#MovementHistory_TransferStatusHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferTargetColumnIndex, $("#MovementHistory_TransferTargetHdr").val());
	 MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferTargetUnitsColumnIndex, $("#MovementHistory_TransferTargetUnitsHdr").val());
	 MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferLevelTargetColumnIndex, $("#MovementHistory_TransferLevelTargetHdr").val());
	 MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferVolumeTargetColumnIndex, $("#MovementHistory_TransferVolumeTargetHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferTimeRemainingColumnIndex, $("#MovementHistory_TransferTimeRemainingHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferredVolumeWaterColumnIndex, $("#MovementHistory_TransferredVolumeWaterHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.transferredVolumeColumnIndex, $("#MovementHistory_TransferredVolumeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.unitsLevelProductColumnIndex, $("#MovementHistory_UnitsLevelProductHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.unitsTemperatureAmbientColumnIndex, $("#MovementHistory_UnitsTemperatureAmbientHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.unitsTemperatureDensityColumnIndex, $("#MovementHistory_UnitsTemperatureDensityHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.unitsTemperatureProductColumnIndex, $("#MovementHistory_UnitsTemperatureProductHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.unitsDensityProductObservedColumnIndex, $("#MovementHistory_UnitsDensityProductObservedHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.unitsDensityProductStandardColumnIndex, $("#MovementHistory_UnitsDensityProductStandardHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.unitsVolumeColumnIndex, $("#MovementHistory_UnitsVolumeHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.unitsMassColumnIndex, $("#MovementHistory_UnitsMassHdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.userData01ColumnIndex, $("#MovementHistory_UserData01Hdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.userData02ColumnIndex, $("#MovementHistory_UserData02Hdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.userData03ColumnIndex, $("#MovementHistory_UserData03Hdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.userData04ColumnIndex, $("#MovementHistory_UserData04Hdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.userData05ColumnIndex, $("#MovementHistory_UserData05Hdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.userData06ColumnIndex, $("#MovementHistory_UserData06Hdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.userData07ColumnIndex, $("#MovementHistory_UserData07Hdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.userData08ColumnIndex, $("#MovementHistory_UserData08Hdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.userData09ColumnIndex, $("#MovementHistory_UserData09Hdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.userData10ColumnIndex, $("#MovementHistory_UserData10Hdr").val());
    MovementHistoryTab.CreateColumnCheckbox(MovementHistoryTab.volumeWaterColumnIndex, $("#MovementHistory_VolumeWaterHdr").val());
};

//=================================================================================
// This function will create a column filter checkbox in the show/hide columns
// dropdown.
//=================================================================================
MovementHistoryTab.CreateColumnCheckbox = function (colIndex, labelContent)
{
    var id = "ColumnFilterCheckbox" + colIndex;
    var columnFilterUl = document.getElementById("MovementHistoryColumnFilterUl");

    // Create the LI element.
    var li = document.createElement("li");
    li.id = id + "Li";
    li.classList.add("ColumnFilterLiClass");
    columnFilterUl.appendChild(li);

    // Create the label that contains the input tag and span
    var label = document.createElement("label");
    label.classList.add("customCheckbox");
    li.appendChild(label);

    // Create the input tag "checkbox"
    var checkbox = document.createElement("input");
    checkbox.id = id;
    checkbox.type = "checkbox";
    checkbox.value = colIndex;
    checkbox.setAttribute("onclick", "MovementHistoryTab.HandleColumnFilterCheckboxChange(this);");
    label.appendChild(checkbox);

    // Create the span tag.
    var span = document.createElement("span");
    label.appendChild(span);

    // Create the label that contains the name.
    var label2 = document.createElement("label");
    label2.htmlFor = id;
    label2.classList.add("ColumnFilterCheckboxLabelClass");
    label2.innerHTML = labelContent;
    li.appendChild(label2);
}

//===========================================================================
// This function does set initial visibility of column re-order.
//===========================================================================
MovementHistoryTab.SetInitialVisibilityColumnReorder = function (visInvisColumns)
{
    if (!visInvisColumns)
    {
        return;
    }

    var visibleArr = visInvisColumns.VisibleArr;
    var invisibleArr = visInvisColumns.InvisibleArr;

    if (!visibleArr || !invisibleArr)
    {
        return;
    }

    if (visibleArr.length === 0 && invisibleArr.length === 0)
    {
        return;
    }
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }

    var table = $("#MovementHistoryTable").DataTable();

    table.colReorder.order([...visibleArr, ...invisibleArr], false);

    for (var i = 0; i < visibleArr.length; i++)
    {
        var currentColIndex = table.colReorder.transpose(visibleArr[i]);
        var column = table.column(currentColIndex);
        column.visible(true,false);
        MovementHistoryTab.SetCheckBoxForColumn(true, visibleArr[i]);
    }

    for (var k = 0; k < invisibleArr.length; k++)
    {
        var currentColIndex2 = table.colReorder.transpose(invisibleArr[k]);
        var column2 = table.column(currentColIndex2,false);
        column2.visible(false);
        MovementHistoryTab.SetCheckBoxForColumn(false, invisibleArr[k]);
    }
    table.columns.adjust().draw(false); // adjust column sizing and redraw;
}

//===================================================================================
// This function will save the view state settings for visible columns, column order, 
// and filters.
//===================================================================================
MovementHistoryTab.SaveViewState = function ()
{
    //console.log('saving view state');
    var saveViewStateUrl = $("#MovementHistorySaveViewStateUrl").val();
    if (saveViewStateUrl) {
        ;
    }
    else { 
        //Hardcode path for saveViewStateUrl here , since hidden control MovementHistorySaveViewStateUrl will not be available if tab has been closed
        saveViewStateUrl = MovementHistoryTab.applicationRootName + "/InventoryManagement/MovementHistoryTab/SaveViewState";
    }
   var token = $('#MovementHistoryTabView input[name =__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;
    var d = MovementHistoryTab.GetVisibilityColumnReorder(MovementHistoryTab.DatatableHandle);
    var movementHistoryViewStateObject = {};
    movementHistoryViewStateObject.VisibleArr = d.VisibleArr;
    movementHistoryViewStateObject.InvisibleArr = d.InvisibleArr;
    movementHistoryViewStateObject.Filters = MovementHistoryTab.columnFilterCollection;
    movementHistoryViewStateObject.PageLen = MovementHistoryTab.DatatableHandle.page.len();
    var movementHistoryViewStateJson = JSON.stringify(movementHistoryViewStateObject);

    PNotify.removeStack(MovementHistoryTab.messageAttributes.stack);
    $.ajax({
        cache: false,
        type: "POST",
        //async: false,
        contentType: 'application/json; charset=UTF-8',
        dataType: "json",
        url: saveViewStateUrl,
        headers: headers,
        //data: { visibleArr: d.VisibleArr, invisibleArr: d.InvisibleArr, filters: MovementHistoryTab.columnFilterCollection },
        data: JSON.stringify({ jsonViewState: movementHistoryViewStateJson }),
        success: function (dummy)
        {
            return undefined;
        },
        error: function (e)
        {
            FMErrorAndExceptionHandling.ShowError(e, null, MovementHistoryTab.messageAttributes);
            return false;
        }
    });
};

//===========================================================================
// This function does column resize.
//===========================================================================
MovementHistoryTab.ResizeColumns = function ()
{
    // this is called when the tab is selected to cause a redraw bds
    if (MovementHistoryTab.HistoryInitialized == false)
    {
        MovementHistoryTab.Initialize();
    }
    if (MovementHistoryTab.DatatableHandle != null)
    {
        MovementHistoryTab.DatatableHandle.columns.adjust();
    }
};

//========================================================================
// This function handles the refresh button on click event. It will 
// validate the start and end dates.
//========================================================================
MovementHistoryTab.HandleRefreshBtnEvent = function ()
{
    var startDateTime = $("#StartTimePicker").val();
    var endDateTime = $("#EndTimePicker").val();

    if (startDateTime == null || startDateTime === "" || endDateTime == null || endDateTime === "")
    {
        FMErrorAndExceptionHandling.ShowError('Must have a Start and End Date/Time.', null, MovementHistoryTab.messageAttributes);
        return;
    }

    if (MovementHistoryTab.ValidateDateTime(startDateTime) == false)
    {
        FMErrorAndExceptionHandling.ShowError('Start Date/Time is invalid.', null, MovementHistoryTab.messageAttributes);
        return;
    }

    if (MovementHistoryTab.ValidateDateTime(endDateTime) == false)
    {
        FMErrorAndExceptionHandling.ShowError('End Date/Time is invalid.', null, MovementHistoryTab.messageAttributes);
        return;
    }

    // Update the Time Stamp TO/FROM dates
    $("#MovementHistoryColumnFilterFromDateInput").val(startDateTime);
    $("#MovementHistoryColumnFilterToDateInput").val(endDateTime);

    // Update the filters by using the existing timestamp date change event.
    MovementHistoryTab.HandleAvailableFilterDateChangeEvent("FROM");
    MovementHistoryTab.HandleAvailableFilterDateChangeEvent("TO");

    // Force Ajax to refresh
   if (MovementHistoryTab.HistoryInitialized === false) {
      return;
   }
    var table = $("#MovementHistoryTable").dataTable();
    table.fnPageChange(0);
};

//=================================================================
// This function handles the Start Date/Time change event.
//=================================================================
MovementHistoryTab.HandleStartDateTimeEvent = function ()
{
    var dateTimeStr = $("#StartTimePicker").val();
    if (MovementHistoryTab.ValidateDateTime(dateTimeStr) == false)
    {
        $("#StartTimePicker").val("");
        FMErrorAndExceptionHandling.ShowError('Start Date/Time is invalid.', null, MovementHistoryTab.messageAttributes);
    }
};

//=================================================================
// This function handles the End Date/Time change event.
//=================================================================
MovementHistoryTab.HandleEndDateTimeEvent = function ()
{
    var startDateTime = $("#StartTimePicker").val();

    if (startDateTime == null || startDateTime === "")
    {
        $("#EndTimePicker").val("");
        FMErrorAndExceptionHandling.ShowError('Must have a start time.', null, MovementHistoryTab.messageAttributes);
        return;
    }

    var dateTimeStr = $("#EndTimePicker").val();
    if (MovementHistoryTab.ValidateDateTime(dateTimeStr) == false)
    {
        $("#EndTimePicker").val("");
        FMErrorAndExceptionHandling.ShowError('End Date/Time is invalid.', null, MovementHistoryTab.messageAttributes);
    }
};

//=================================================================================
// This function handles the right mouse click event to display the context
// menu.
//=================================================================================
MovementHistoryTab.HandleRightMouseClick = function (event)
{
    //debugger;
    var model = MovementHistoryTab.GetMovementHistoryModel();
    var row = event.target._DT_CellIndex.row;

    if (row < 0 || row >= model.MovementHistories.length) return;

    MovementHistoryTab.selectedRowData = model.MovementHistories[row];
    var targetCellId = event.target.id;

    $("#HistoryRowContextMenuDiv").show();

    // When the user clicks off the context menu, it will close.
    $("body").click(function (e)
    {
        $("#HistoryRowContextMenuDiv").hide();
    });

    var grayedOutRgb = "rgb(140, 140, 140)";
    var normalRgb = "rgb(34, 34, 34)";
    $("#HistoryContextMenuPrintTicket").removeAttr("disabled");
    $("#HistoryContectMenuPreviewTicket").removeAttr("disabled");
    $("#HistoryContextMenuEditHandgauge").removeAttr("disabled");
    $("#HistoryContextMenuEditStartData").removeAttr("disabled");
    $("#HistoryContextMenuEditCloseoutData").removeAttr("disabled");
    $("#HistoryContextMenuEditMovementData").removeAttr("disabled");

    $("#HistoryContextMenuPrintTicket").css("color", normalRgb);
    $("#HistoryContectMenuPreviewTicket").css("color", normalRgb);
    $("#HistoryContextMenuEditHandgauge").css("color", normalRgb);
    $("#HistoryContextMenuEditStartData").css("color", normalRgb);
    $("#HistoryContextMenuEditCloseoutData").css("color", normalRgb);
    $("#HistoryContextMenuEditMovementData").css("color", normalRgb);

    // Disable "Print Ticket" if we are missing either the movement ticket printer or the report
    if (!model.HasMovementTicketPrinter || !model.HasMovementTicketReport)
    {
        $("#HistoryContextMenuPrintTicket").attr("disabled", "disabled");
        $("#HistoryContextMenuPrintTicket").css("color", grayedOutRgb);
    }

    // Disable "Print Preview" if we are missing either the movement ticket report
    if (!model.HasMovementTicketReport) {
        $("#HistoryContextMenuPreviewTicket").attr("disabled", "disabled");
        $("#HistoryContextMenuPreviewTicket").css("color", grayedOutRgb);
    }

    // Movement record
    if (MovementHistoryTab.selectedRowData.RecordType == 0)
    {
        $("#HistoryContextMenuEditHandgauge").attr("disabled", "disabled");
        $("#HistoryContextMenuEditStartData").attr("disabled", "disabled");
        $("#HistoryContextMenuEditCloseoutData").attr("disabled", "disabled");

        $("#HistoryContextMenuEditHandgauge").css("color", grayedOutRgb);
        $("#HistoryContextMenuEditStartData").css("color", grayedOutRgb);
        $("#HistoryContextMenuEditCloseoutData").css("color", grayedOutRgb);
        return;
    }

    // Auto gauge Record
    if (MovementHistoryTab.selectedRowData.RecordType == 1)
    {
        $("#HistoryContextMenuEditHandgauge").attr("disabled", "disabled");
        $("#HistoryContextMenuEditStartData").attr("disabled", "disabled");
        $("#HistoryContextMenuEditCloseoutData").attr("disabled", "disabled");
        $("#HistoryContextMenuEditMovementData").attr("disabled", "disabled");

        $("#HistoryContextMenuEditHandgauge").css("color", grayedOutRgb);
        $("#HistoryContextMenuEditStartData").css("color", grayedOutRgb);
        $("#HistoryContextMenuEditCloseoutData").css("color", grayedOutRgb);
        $("#HistoryContextMenuEditMovementData").css("color", grayedOutRgb);
        return;
    }

    // Handgauge Record
    if (MovementHistoryTab.selectedRowData.RecordType == 2)
    {
        $("#HistoryContextMenuEditStartData").attr("disabled", "disabled");
        $("#HistoryContextMenuEditCloseoutData").attr("disabled", "disabled");
        $("#HistoryContextMenuEditMovementData").attr("disabled", "disabled");

        $("#HistoryContextMenuEditStartData").css("color", grayedOutRgb);
        $("#HistoryContextMenuEditCloseoutData").css("color", grayedOutRgb);
        $("#HistoryContextMenuEditMovementData").css("color", grayedOutRgb);
        return;
    }

    // Final Record
    if (MovementHistoryTab.selectedRowData.RecordType == 3)
    {
        $("#HistoryContextMenuEditMovementData").attr("disabled", "disabled");
        $("#HistoryContextMenuEditHandgauge").attr("disabled", "disabled");

        $("#HistoryContextMenuEditMovementData").css("color", grayedOutRgb);
        $("#HistoryContextMenuEditHandgauge").css("color", grayedOutRgb);
        return;
    }
};

//==================================================================
// This function handles the history context menu select event.
//==================================================================
MovementHistoryTab.HandleContextMenuSelection = function (id)
{
    $("#HistoryRowContextMenuDiv").hide();
   
    if (id === "PrintTicket")
    {
        MovementHistoryTab.PrintClicked();
    }

    if (id === "PreviewTicket")
    {
        MovementHistoryTab.PrintPreviewClicked();
    }

    if (id === "EditHandgaugeValue")
    {
        MovementHistoryTab.OpenHandgaugeDialog();
    }

    if (id === "EditStartData")
    {
        MovementHistoryTab.OpenEditStartData();
    }

    if (id === "EditCloseoutData")
    {
        MovementHistoryTab.OpenEditCloseoutData();
    }

    if (id === "EditMovementData")
    {
        MovementHistoryTab.OpenEditMovementData();
    }
};

//========================================================================
// This function will open the handgauge dialog.
//========================================================================
MovementHistoryTab.OpenHandgaugeDialog = function ()
{
    var caller = 1; // CallerMovementHistory
    var row = MovementHistoryTab.selectedRowData;

    if (row == null)
    {
        return;
    }

    var paramPointGuid = row.PointGuid;
    var paramHistoryRecordGuid = row.MovementHistoryGuid;

    FMOperateIndex.OpenMovementHandgaugeClickPropertyScreen(paramPointGuid, caller, paramHistoryRecordGuid);
};

//========================================================================
// This function will open the Edit Start Data dialog.
//========================================================================
MovementHistoryTab.OpenEditStartData = function ()
{
    var callingType = 0; // Calling type is Start
    var row = MovementHistoryTab.selectedRowData;

    if (row == null)
    {
        return;
    }

    var paramHistoryRecordGuid = row.MovementHistoryGuid;
    FMOperateIndex.OpenMovementHistoryNodeEditorClickPropertyScreen(callingType, paramHistoryRecordGuid);
};

//========================================================================
// This function will open the Edit Closeout Data dialog.
//========================================================================
MovementHistoryTab.OpenEditCloseoutData = function ()
{
    var callingType = 1; // Calling type is Start
    var row = MovementHistoryTab.selectedRowData;

    if (row == null)
    {
        return;
    }

    var paramHistoryRecordGuid = row.MovementHistoryGuid;
    FMOperateIndex.OpenMovementHistoryNodeEditorClickPropertyScreen(callingType, paramHistoryRecordGuid);
};

//========================================================================
// This function will open the Edit Movement Data dialog.
//========================================================================
MovementHistoryTab.OpenEditMovementData = function ()
{
    var row = MovementHistoryTab.selectedRowData;

    if (row == null)
    {
        return;
    }

    var paramHistoryRecordGuid = row.MovementHistoryGuid;
    FMOperateIndex.OpenMovementHistoryMovementDataEditorClickPropertyScreen(paramHistoryRecordGuid);
};

//=====================================================================
// This function will validate whether the date/time is valid. False
// indicates invalid.
//=====================================================================
MovementHistoryTab.ValidateDateTime = function (inDateTime)
{
    if (inDateTime == null || inDateTime === "")
    {
        return true;
    }

    var numFormatInfoString = $("#NumberFormatInfoString").val();
    var numFormatInfo = JSON.parse(numFormatInfoString);
    var pattern = numFormatInfo.ShortDatePattern; 

    // Moment does not like the slashes in a pattern, but handles the slashes in the date.
    pattern = pattern.replaceAll("/", "-");
    pattern = pattern.replaceAll("y", "Y");
    pattern = pattern.replaceAll("d", "D");
    pattern = pattern.replaceAll("m", "M");

    pattern = pattern + " " + numFormatInfo.TimePattern;

    var dateTime = moment(inDateTime, pattern);
    var valid = dateTime.isValid();

    if (valid == false)
    {
        return false;
    }

    return true;
};

//============================================================================================================
// This function calls the controller to get the movement history data and displays the movement history
// tab.
//============================================================================================================
MovementHistoryTab.CreateMovementHistoryTab = function (activeTab, newId, stack_bottomright_operatortab)
{
    $.ajax({
        type: 'get',
        dataType: 'json',
        cache: false,
        url: $("#urlMovementHistory").val(),
        activeTab: activeTab,
        newId: newId,
        data: {},
        success: function (response)
        {
            var activeTab = this.activeTab;
            var newId = this.newId;

            $("#LoadingImageMovementHistory" + newId).remove();

            var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

            FMErrorAndExceptionHandling.HandleMessages(response,
                function (movementHistoryView, inError)
                {
                    // if it was not in error load and update the drawing
                    if (!inError)
                    {
                        // Double click on the tab name to rename the point group
                        $('a[data-target="#' + newId + '"]').attr('ondblclick', "FMOperateIndex.RenameTab( this );");

                        $("#MovementHistory" + newId + "container").html(movementHistoryView);

                        // done with the process of restoring the tab
                        FMOperateIndex.restoringScreenQueueInProgress[newId] = false;

                    }
                },
                messageAttributes);

            // done reloading the tab
            FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
        },
        error: function (xhr, textStatus, error)
        {
            var newId = this.newId;

            // need to make  sure that the error we are getting is because we close the page before getting the response
            if (xhr.status != 0)
            {
                FMErrorAndExceptionHandling.ShowException(xhr,
                    textStatus,
                    error,
                    function ()
                    {
                        $("#LoadingImageMovementHistory" + newId).remove();
                    });
            }

            // done reloading the tab
            FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
        }
    });
};


MovementHistoryTab.PrintPreviewClicked = function () {
    var movementHistoryGuid = MovementHistoryTab.selectedRowData.RootParentGuid;
    
    var movementTicketReportName = $('#Site_MovementTicketReportName').val();

    if (movementHistoryGuid === undefined || movementHistoryGuid === "00000000-0000-0000-0000-000000000000") {
        var movementHistoryGuid = MovementHistoryTab.selectedRowData.MovementHistoryGuid;
    }

    if (movementHistoryGuid === undefined || movementHistoryGuid === "00000000-0000-0000-0000-000000000000") {
        return;
    }

    url = $('#urlReportViewer').val();

    // remove previous notifications
    PNotify.removeStack();

    url += "?ReportType=10";
    url += "&ReportName=" + movementTicketReportName;
    url += "&movementGuid=" + movementHistoryGuid;
    url += "&CSRFToken=" + window.csrfTokenStr;
    window.open(url);
};

MovementHistoryTab.PrintClicked = function () {
    var movementHistoryGuid = MovementHistoryTab.selectedRowData.RootParentGuid;

    if (movementHistoryGuid === undefined || movementHistoryGuid === "00000000-0000-0000-0000-000000000000") {
        var movementHistoryGuid = MovementHistoryTab.selectedRowData.MovementHistoryGuid;
    }

    if (movementHistoryGuid === undefined || movementHistoryGuid === "00000000-0000-0000-0000-000000000000") {
        return;
    }

    if (movementHistoryGuid == '' || movementHistoryGuid == '00000000-0000-0000-0000-000000000000') {
        return;
    }
    var callData = {
        movementHistoryGuid: movementHistoryGuid
    };

    var token = $('input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;
    url = $('#MovementHistoryPrintReportUrl').val();

    // remove previous notifications
    PNotify.removeStack();

    $.ajax({
        type: 'Post',
        url: url,
        dataType: 'json',
        data: JSON.stringify(callData),
        headers: headers,
        cache: false,
        success: function (response) {

        },
        error: function (xhr, textStatus, error) {
        }
    });
};
var FMUserAdminAudit = FMUserAdminAudit ||
	{
	    imageRootPath: ""
        , ViewFilterDownArrow: "DownArrowGray.png"
        , ViewFilterLeftArrow: "LeftArrowGray.png"
        , viewFilterState: 0
        , daysPastState: 0 // 0 = none selected, 1 = 30 days, 2 = 60 days, 3 = 90 days,
        , auditDataTableHandle: null
        , beginDateTimePickerHandle: null
        , endDateTimePickerHandle: null
	};

//=======================================================
// This function will invoke the data table ajax call
// to refresh based on the filters.
//=======================================================
FMUserAdminAudit.HandleRefreshBtnEvent = function ()
{
    FMUserAdminAudit.auditDataTableHandle.ajax.reload();
    return false;
}

//================================================================
// This function will initialize the admininstration and audit
// page.
//================================================================
FMUserAdminAudit.Initialize = function ()
{
    $("#ViewFilterImg").attr("src", FMUserAdminAudit.imageRootPath + FMUserAdminAudit.ViewFilterLeftArrow);

    // Collapse the View Filter section.
    FMUserAdminAudit.viewFilterState = 1;
    FMUserAdminAudit.ToggleViewFilters();
    FMUserAdminAudit.InitializeFilterForSelect2();

    // Reset all controls
    FMUserAdminAudit.HandleResetBtnEvent();
    FMUserAdminAudit.RetrieveAuditFilterData();

    FMUserAdminAudit.InitializeDataTable();
    FMUserAdminAudit.InitializeDateTimePickers();

    // We want to hide the Search capabilities for now.
    $("#AuditLogTable_filter").hide();
}

//============================================================
// This function will expand and collapse the View Filter
// section based on the current state setting (0 = collapsed,
// 1 = expanded).
//============================================================
FMUserAdminAudit.ToggleViewFilters = function ()
{
    // This call will ensure the Refresh button is in the correct state.
    FMUserAdminAudit.SetRefreshBtnState();

    if (FMUserAdminAudit.viewFilterState === 0)
    {
        $("#ViewFilterImg").attr("src", FMUserAdminAudit.imageRootPath + FMUserAdminAudit.ViewFilterDownArrow);
        $("#ViewFilterImg").width("20px");
        $("#ViewFilterImg").height("20px");
        $("#ViewFiltersExpandDiv").show();
        FMUserAdminAudit.viewFilterState = 1;
    }
    else
    {
        $("#ViewFilterImg").attr("src", FMUserAdminAudit.imageRootPath + FMUserAdminAudit.ViewFilterLeftArrow);
        $("#ViewFilterImg").width("16px");
        $("#ViewFilterImg").height("16px");
        $("#ViewFiltersExpandDiv").hide();
        FMUserAdminAudit.viewFilterState = 0;
    }
}

//=====================================================================
// This function will retrieve the filter selections and return a 
// filter object.
//=====================================================================
FMUserAdminAudit.GetFilterSelections = function ()
{
    var filterObj = FMUserAdminAudit.CreateFilterObject();

    filterObj.SiteGuidStr = $("#SiteSelect option:selected").val();
    filterObj.ActionId = $("#ActionIdSelect option:selected").val();
    filterObj.TypeId = $("#TypeIdSelect option:selected").val();
    filterObj.Id = $("#IdSelect option:selected").val();

    if (filterObj.SiteGuidStr === "" || typeof filterObj.SiteGuidStr == "undefined") filterObj.SiteGuidStr = "00000000-0000-0000-0000-000000000000";
    if (filterObj.ActionId === "" || typeof filterObj.ActionId == "undefined") filterObj.ActionId = "";
    if (filterObj.TypeId === "" || typeof filterObj.TypeId == "undefined") filterObj.TypeId = "";
    if (filterObj.Id === "" || typeof filterObj.Id == "undefined") filterObj.Id = "";

    var dateFilterSelection = FMUserAdminAudit.GetDateFilterSelection();
    filterObj.BeginDateStr = "";
    filterObj.EndDateStr = "";

    if (dateFilterSelection.HasDate)
    {
        filterObj.BeginDateStr = dateFilterSelection.BeginDate;
        filterObj.EndDateStr = dateFilterSelection.EndDate;
        filterObj.HasDate = true;
    }

    return filterObj;
}

//=============================================================
// This function will set the hover title to the dropdown
// name after a selection.
//=============================================================
FMUserAdminAudit.SetDropdownHoverTitle = function ()
{
    var siteSelectionValue = $("#SiteSelect option:selected").val();
    var actionIdSelectionValue = $("#ActionIdSelect option:selected").val();
    var typeIdSelectionValue = $("#TypeIdSelect option:selected").val();
    var idSelectionValue = $("#IdSelect option:selected").val();

    if (siteSelectionValue !== "" && typeof siteSelectionValue != "undefined")
    {
        $("#select2-SiteSelect-container").attr("title", "Sites");
    }

    if (actionIdSelectionValue !== "" && typeof actionIdSelectionValue != "undefined")
    {
        $("#select2-ActionIdSelect-container").attr("title", "Action ID");
    }

    if (typeIdSelectionValue !== "" && typeof typeIdSelectionValue != "undefined")
    {
        $("#select2-TypeIdSelect-container").attr("title", "Type ID");
    }

    if (idSelectionValue !== "" || typeof idSelectionValue != "undefined")
    {
        $("#select2-IdSelect-container").attr("title", "ID");
    }
}

//===================================================================
// This function will create a new filter object.
//===================================================================
FMUserAdminAudit.CreateFilterObject = function ()
{
    var filterObj = new Object();
    filterObj.SiteGuidStr = "";
    filterObj.ActionId = "";
    filterObj.TypeId = "";
    filterObj.Id = "";
    filterObj.UserGuidStr = "";
    filterObj.Source = "";
    filterObj.BeginDateStr = "";
    filterObj.EndDateStr = "";
    filterObj.HasDate = false;

    return filterObj;
}

//=========================================================
// This function will initialize the dropdown to be a 
// select2 type.
//=========================================================
FMUserAdminAudit.InitializeFilterForSelect2 = function ()
{
    $("#SiteSelect").select2(
    {
        placeholder: "Sites {All}",
        allowClear: true
    });

    $("#ActionIdSelect").select2(
    {
        placeholder: "Action ID {All}",
        allowClear: true
    });

    $("#TypeIdSelect").select2(
    {
        placeholder: "Type ID {All}",
        allowClear: true
    });

    $("#IdSelect").select2(
    {
        placeholder: "ID",
        allowClear: true
    });
}

//=======================================================
// This function will initialize the date time pickers.
//=======================================================
FMUserAdminAudit.InitializeDateTimePickers = function ()
{

    var numFormatInfoString = $('#NumberFormatInfoString').val();
    var numFormatInfo = JSON.parse(numFormatInfoString);
    FMLayout.dateFormat = ConvertToJQueryUIDateFormat(numFormatInfo.ShortDatePattern);
    FMLayout.timeFormat = ConvertToJQueryUITimeFormat(numFormatInfo.TimePattern);

    FMUserAdminAudit.beginDateTimePickerHandle = $("#BeginDateTb").datetimepicker({
        dateFormat: FMLayout.dateFormat,
        timeFormat: FMLayout.timeFormat,
        showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true
    });

    FMUserAdminAudit.endDateTimePickerHandle = $("#EndDateTb").datetimepicker({
        dateFormat: FMLayout.dateFormat,
        timeFormat: FMLayout.timeFormat,
        showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true
    });
}

//=================================================================
// This function will populate retrieve the ID filter data that
// will be used to populate the ID filter dropdown.
//=================================================================
FMUserAdminAudit.RetrieveIdFilterData = function ()
{
    var filterObj = FMUserAdminAudit.GetFilterSelections();

    // Since the ID filter is dependent on site/action ID/Type ID, then
    // it must be cleared on any changes.
    FMUserAdminAudit.ClearIdFilterSelect();

    // Set the hover text to be the placeholder value.
    FMUserAdminAudit.SetDropdownHoverTitle();

    if (filterObj.TypeId === ""
        || typeof filterObj.TypeId == "undefined"
        || filterObj.HasDate === false
        || filterObj.SiteGuidStr === ""
        || typeof filterObj.SiteGuidStr == "undefined")
    {
        return;
    }

    var getAuditFilterDataForIdUrl = $("#GetAuditFilterDataForIdUrl").val();
    var filterStr = JSON.stringify(filterObj);

    var token = $('#UserConfigurationViewForm input[name =__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: getAuditFilterDataForIdUrl,
        headers: headers,
        dataType: "json",
        data: { filters: filterStr },
        success: function (results) {
            if (results.ErrorFlag)
            {
                alert(results.ErrorMsg);
                return;
            }

            var idFilterList = results.IdFilterList;
            FMUserAdminAudit.PopulateIdDropdown(idFilterList);
            return;
        },
        error: function () {
            alert("Error.");
        }
    });
}

//========================================================================
// This function will retrieve the audit independant filter data and
// call a function to populate them.
//========================================================================
FMUserAdminAudit.RetrieveAuditFilterData = function ()
{
    var getAuditFilterDataUrl = $("#GetAuditFilterDataUrl").val();

    var token = $('#UserConfigurationViewForm input[name =__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    $.ajax({
        cache: false,
        type: "POST",
        async: false,
        url: getAuditFilterDataUrl,
        headers: headers,
        dataType: "json",
        data: { },
        success: function (results) {
            if (results.ErrorFlag)
            {
                alert(results.ErrorMsg);
                return;
            }

            var auditFilterDataModel = results.SourceFilterDataModel;
            FMUserAdminAudit.PopulateIndependentDropdowns(auditFilterDataModel);
            return;
        },
        error: function () {
            alert("Error.");
        }
    });
}

//==============================================================================
// This function will populate the audit independant filter dropdowns (Site,
// Action ID, and Type ID).
//==============================================================================
FMUserAdminAudit.PopulateIndependentDropdowns = function (auditFilterDataModel)
{
    var siteList = auditFilterDataModel.SiteList;
    if (siteList != null && siteList.length > 0)
    {
        for (var nextSite = 0; nextSite < siteList.length; nextSite++)
        {
            var siteModel = siteList[nextSite];
            if (siteModel != null)
            {
                var siteGuidStr = siteModel.SiteGuidStr;
                var siteId = siteModel.SiteId;
                $("#SiteSelect").append($("<option />").val(siteGuidStr).text(siteId));
            }
        }

        // No selection
        $("#SiteSelect").select2("val", "");
    }

    var actionIdList = auditFilterDataModel.ActionIdList;
    if (actionIdList != null && actionIdList.length > 0)
    {
        for (var nextActionId = 0; nextActionId < actionIdList.length; nextActionId++)
        {
            var actionIdModel = actionIdList[nextActionId];
            if (actionIdModel != null)
            {
                var actionIdValue = actionIdModel.ActionIdValue;
                var actionId = actionIdModel.ActionId;
                $("#ActionIdSelect").append($("<option />").val(actionIdValue).text(actionId));
            }
        }

        // No selection
        $("#ActionIdSelect").select2("val", "");
    }

    var typeIdList = auditFilterDataModel.TypeIdList;
    if (typeIdList != null && typeIdList.length > 0)
    {
        for (var nextTypeId = 0; nextTypeId < typeIdList.length; nextTypeId++)
        {
            var typeIdModel = typeIdList[nextTypeId];
            if (typeIdModel != null)
            {
                var typeIdValue = typeIdModel.TypeIdValue;
                var typeId = typeIdModel.TypeId;
                $("#TypeIdSelect").append($("<option />").val(typeIdValue).text(typeId));
            }
        }

        // No selection
        $("#TypeIdSelect").select2("val", "");
    }
}

//========================================================================
// This function will populate the ID dropdown.
//========================================================================
FMUserAdminAudit.PopulateIdDropdown = function (idFilterList)
{
    FMUserAdminAudit.ClearIdFilterSelect();

    if (idFilterList != null && idFilterList.length > 0)
    {
        for (var nextId = 0; nextId < idFilterList.length; nextId++)
        {
            var idFilterModel = idFilterList[nextId];
            if (idFilterModel != null)
            {
                var id = idFilterModel.Id;
                var idValue = idFilterModel.IdValue;
                $("#IdSelect").append($("<option />").val(idValue).text(id));
            }
        }

        // No selection
        $("#IdSelect").select2("val", "");
    }
}

//========================================================
// This function will clear the ID select filter dropdown.
//========================================================
FMUserAdminAudit.ClearIdFilterSelect = function ()
{
    // Clear out all the options and clear the placeholder field.
    $("#IdSelect").empty();
    $("#IdSelect").val(null).trigger("change");
}

//===================================================================
// This function will handle the custom date checkbox on change
// event.
//===================================================================
FMUserAdminAudit.HandleCustomDateCheckboxEvent = function ()
{
    var checkboxState = $("#CustomDateCb").is(":checked");
    if (checkboxState)
    {
        $("#BeginDateTb").removeAttr("disabled");
        $("#EndDateTb").removeAttr("disabled");
        FMUserAdminAudit.SetClearDaysPast(0);
    }
    else
    {
        $("#BeginDateTb").val("");
        $("#EndDateTb").val("");
        $("#BeginDateTb").attr("disabled", "disabled");
        $("#EndDateTb").attr("disabled", "disabled");
    }

    // This call is to ensure the Refresh button state set correctly.
    FMUserAdminAudit.SetRefreshBtnState();
}

//===========================================================
// This function will handle the reset button event. It will
// reset all the controls.
//===========================================================
FMUserAdminAudit.HandleResetBtnEvent = function ()
{
    // Clear out all the options and clear the placeholder field.
    $("#SiteSelect").select2("val", "");
    $("#ActionIdSelect").select2("val", "");
    $("#TypeIdSelect").select2("val", "");
    $("#IdSelect").select2("val", "");

    $("#IdSelect").empty();
    $("#IdSelect").val(null).trigger("change");

    $("#BeginDateTb").attr("disabled", "disabled");
    $("#EndDateTb").attr("disabled", "disabled");
    $("#BeginDateTb").val("");
    $("#EndDateTb").val("");

    $("#CustomDateCb").prop('checked', false);
    $("#FilterRefreshBtn").prop('disabled', true);

    FMUserAdminAudit.SetClearDaysPast(0);
}

//=======================================================
// This function controls the selection of Days past
// buttons.
//=======================================================
FMUserAdminAudit.SetClearDaysPast = function (state)
{
    $("#ThirtyDaysDiv").removeClass("DaysPastSelected");
    $("#SixtyDaysDiv").removeClass("DaysPastSelected");
    $("#NinetyDaysDiv").removeClass("DaysPastSelected");

    $("#ThirtyDaysDiv").addClass("DaysPastNotSelected");
    $("#SixtyDaysDiv").addClass("DaysPastNotSelected");
    $("#NinetyDaysDiv").addClass("DaysPastNotSelected");

    FMUserAdminAudit.daysPastState = 0;
    var pastDate;
    var currentDate;

    var checkboxState = $("#CustomDateCb").is(":checked");
    if (checkboxState) return;

    if (state === 1)
    {
        $("#ThirtyDaysDiv").removeClass("DaysPastNotSelected");
        $("#ThirtyDaysDiv").addClass("DaysPastSelected");
        FMUserAdminAudit.daysPastState = 1;

        pastDate = FMUserAdminAudit.CalculateDate("MINUS", 30);
        FMUserAdminAudit.beginDateTimePickerHandle.datepicker("setDate", pastDate);

        currentDate = FMUserAdminAudit.CalculateDate("MINUS", 0);
        FMUserAdminAudit.endDateTimePickerHandle.datepicker("setDate", currentDate);
    }

    if (state === 2)
    {
        $("#SixtyDaysDiv").removeClass("DaysPastNotSelected");
        $("#SixtyDaysDiv").addClass("DaysPastSelected");
        FMUserAdminAudit.daysPastState = 2;

        pastDate = FMUserAdminAudit.CalculateDate("MINUS", 61);
        FMUserAdminAudit.beginDateTimePickerHandle.datepicker("setDate", pastDate);

        currentDate = FMUserAdminAudit.CalculateDate("MINUS", 0);
        FMUserAdminAudit.endDateTimePickerHandle.datepicker("setDate", currentDate);
    }

    if (state === 3)
    {
        $("#NinetyDaysDiv").removeClass("DaysPastNotSelected");
        $("#NinetyDaysDiv").addClass("DaysPastSelected");
        FMUserAdminAudit.daysPastState = 3;

        pastDate = FMUserAdminAudit.CalculateDate("MINUS", 92);
        FMUserAdminAudit.beginDateTimePickerHandle.datepicker("setDate", pastDate);

        currentDate = FMUserAdminAudit.CalculateDate("MINUS", 0);
        FMUserAdminAudit.endDateTimePickerHandle.datepicker("setDate", currentDate);
    }

    FMUserAdminAudit.SetRefreshBtnState();
}

//===========================================================================
// This function will calculate days in the past or future. It will return
// a new date with time set to 0.
//===========================================================================
FMUserAdminAudit.CalculateDate = function (direction, days)
{
    var dateStr = "";
    var dateObj = new Date();
    dateObj.setHours(0);
    dateObj.setMinutes(0);
    dateObj.setSeconds(0);

    if (direction === "MINUS")
    {
        dateStr = moment().subtract(days, 'days').format("YYYY-MM-DD");
    }

    if (direction === "PLUS")
    {
        dateStr = moment().add(days, 'days').format("YYYY-MM-DD");
    }

    if (dateStr !== "")
    {
        var parts = dateStr.split("-");
        var yearInt = parseInt(parts[0]);
        var monthInt = parseInt(parts[1] - 1);
        var dayInt = parseInt(parts[2]);

        dateObj.setYear(yearInt);
        dateObj.setMonth(monthInt);
        dateObj.setDate(dayInt);
    }

    return dateObj;
}

//========================================================
// This function will handle the date text boxes onBlur
// event. It will enable/disable the Refresh button base
// on whether they are populated.
//========================================================
FMUserAdminAudit.SetRefreshBtnState = function ()
{  
    var beginDateText = $("#BeginDateTb").val();
    var endDateText = $("#EndDateTb").val();

    $("#FilterRefreshBtn").prop('disabled', true);
    $("#FilterRefreshBtn").addClass("pushButtonDisable");

    if (FMUserAdminAudit.daysPastState !== 0)
    {
        $("#FilterRefreshBtn").prop('disabled', false);
        $("#FilterRefreshBtn").removeClass("pushButtonDisable");
        return;
    }

    if (beginDateText !== "" && typeof beginDateText !== "undefined"
        && endDateText !== "" && typeof endDateText !== "undefined")
    {
        $("#FilterRefreshBtn").prop('disabled', false);
        $("#FilterRefreshBtn").removeClass("pushButtonDisable");
    }
}

//============================================================
// This function will return the date filter select.
//============================================================
FMUserAdminAudit.GetDateFilterSelection = function ()
{
    var dateFilterObj = new Object();
    dateFilterObj.HasDate = false;
    dateFilterObj.BeginDate = "";
    dateFilterObj.EndDate = "";

    if (FMUserAdminAudit.daysPastState === 1)
    {
        dateFilterObj.BeginDate = "Days_30";
        dateFilterObj.EndDate = "Days_30";
        dateFilterObj.HasDate = true;
        return dateFilterObj;
    }

    if (FMUserAdminAudit.daysPastState === 2)
    {
        dateFilterObj.BeginDate = "Days_60";
        dateFilterObj.EndDate = "Days_60";
        dateFilterObj.HasDate = true;
        return dateFilterObj;
    }

    if (FMUserAdminAudit.daysPastState === 3)
    {
        dateFilterObj.BeginDate = "Days_90";
        dateFilterObj.EndDate = "Days_90";
        dateFilterObj.HasDate = true;
        return dateFilterObj;
    }

    var beginDateText = $("#BeginDateTb").val();
    var endDateText = $("#EndDateTb").val();

    if (beginDateText === "" || typeof beginDateText === "undefined"
        || endDateText === "" || typeof endDateText === "undefined")
    {
        return dateFilterObj;
    }

    dateFilterObj.BeginDate = beginDateText;
    dateFilterObj.EndDate = endDateText;
    dateFilterObj.HasDate = true;
    return dateFilterObj;
}

//=====================================================
// This function will create and return the table
// columns for the datatable.
//=====================================================
FMUserAdminAudit.CreateColumns = function ()
{
    var cols = [
		  { "data": "AuditDateTimeStr", "orderable": false, "visible": true  }
		, { "data": "ActionId", "orderable": false, "visible": true  }
		, { "data": "TypeId", "orderable": false, "visible": true  }
		, { "data": "Id", "orderable": false, "visible": true  }
		, { "data": "PropertyId", "orderable": false, "visible": true  }
		, { "data": "NewValue", "orderable": false, "visible": true  }
		, { "data": "OldValue", "orderable": false, "visible": true  }
		, { "data": "SiteId", "orderable": false, "visible": true  }
		, { "data": "Source", "orderable": false, "visible": true  }
    ];

    return cols;
}

//======================================================
// This function will initialize the data table with
// all the columns and the ajax call.
//======================================================
FMUserAdminAudit.InitializeDataTable = function ()
{
    var getAuditRecordsUrl = $("#AuditControllerGetAuditDataUrl").val();
    var token = $('#UserConfigurationViewForm input[name =__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    FMUserAdminAudit.auditDataTableHandle = $("#AuditLogTable").DataTable(
    {
        "columnDefs": [
			    {
			        "targets": [0, 1, 2, 3, 4, 5, 6, 7, 8]
			    }
        ],
        "language":
        {
            "processing": "<div class='overlay'><i style=\"background-color: white !important;color: black !important;z-index:5\">" + $("#AlarmHistory_DD_Processing").val() + "</i></div>",
            "info": $("#UserAdmin_DD_Showing").val() + " _START_ " + $("#UserAdmin_DD_to").val() + " _END_ " + $("#UserAdmin_DD_of").val() + " _TOTAL_ " + $("#UserAdmin_DD_entries").val(),
            "infoEmpty": $("#UserAdmin_DD_Showing").val() + " 0 " + $("#UserAdmin_DD_to").val() + " 0 " + $("#UserAdmin_DD_of").val() + " 0 " + $("#UserAdmin_DD_entries").val(),
            "lengthMenu": $("#UserAdmin_DD_Show").val() + "  _MENU_  " + $("#UserAdmin_DD_entries").val(),
            "zeroRecords": $("#UserAdmin_DD_NoMatchingRecords").val(),
            "paginate": {
                        "first": $("#UserAdmin_DD_FirstRecord").val(),
                        "last": $("#UserAdmin_DD_LastRecord").val(),
                        "next": $("#UserAdmin_DD_NextRecord").val(),
                        "previous": $("#UserAdmin_DD_PreviousRecord").val()
                        }
        },
        "processing": false,
        "serverSide": true,
        "ajax": {
            "url": getAuditRecordsUrl,
            "type": "POST",
            "contentType": "application/json; charset=UTF-8",
            "dataType": "json",
            "data": function (d) {
                var filterObj = FMUserAdminAudit.GetFilterSelections();
                var filterObjStr = JSON.stringify(filterObj);
                return JSON.stringify(
                {
                    "draw": d.draw,
                    "start": d.start,
                    "length": d.length,
                    "filterInfoStr": filterObjStr
                });
            },
            "headers": headers,
            "error": function (xhr, error, thrown) {
                alert("Error: " + thrown + "; " + xhr.message);
            }
        },
        "columns": FMUserAdminAudit.CreateColumns(),
        "ordering": false,
        "lengthMenu": [[10, 25, 50, 100, 500], [10, 25, 50, 100, 500]],
            // Show First, Previous, Next, and Last buttons for paging
        "pagingType": "full"
        , "scrollY":"300px", "scrollCollapse": true, "paging":true
            // Embed a div in the datatables controls so we can color the bottom of the grid
            // like a footer
        , "dom": '<"#DataTablesWrapper"lfrt<"#DataTablesFooter"ip>>'
    });

    FMUserAdminAudit.auditDataTableHandle.columns.adjust().draw();

    FMUserAdminAudit.auditDataTableHandle.on('init.dt', function () {
        // When datatables initialization is complete, hide the "Please Wait..." dialog
        $("#TableLoadPleaseWaitDiv").hide();
    });

    FMUserAdminAudit.auditDataTableHandle.on('processing.dt', function (e, settings, processing) {
        // When datatables is processing (e.g. sort/page/filter), show the "Please Wait..." dialog
        if (processing) {
            $("#TableLoadPleaseWaitDiv").show();
        }
        else {
            $("#TableLoadPleaseWaitDiv").hide();
        }
    });

    FMUserAdminAudit.auditDataTableHandle.on('draw.dt', function ()
    {
        var divScrollContainer = document.getElementsByClassName("dataTables_scrollHeadInner");
        if (divScrollContainer.length > 0) divScrollContainer[0].style.width = "1099px";

        var tableScroll = document.getElementsByClassName("table table-striped dataTable no-footer");
        if (tableScroll.length > 0) tableScroll[0].style.width = "1099px";
    });

    //$("#AuditLogTable").tablesorter(
    //    {
    //        dateFormat: 'mmddyyyy', // we always use mmddyyyy when sorting because although the dates are displayed according to regional settings, the column's data-text is formatted mmddyyyy
    //        theme: 'default', // the default tableSorter theme, which we slightly customize
    //        headers:
    //        {
    //            0: { sorter: false, parser: false }, // You can't sort the edit column
    //            1: { sorter: false, parser: false }, // You can't sort the delete column
    //            2: { sorter: false, parser: false }, // You can't sort the delete column
    //            3: { sorter: false, parser: false }, // You can't sort the delete column
    //            4: { sorter: false, parser: false }, // You can't sort the delete column
    //            5: { sorter: false, parser: false }, // You can't sort the delete column
    //            6: { sorter: false, parser: false }, // You can't sort the delete column
    //            7: { sorter: false, parser: false }, // You can't sort the delete column
    //            8: { sorter: false, parser: false } // You can't sort the delete column
    //        },
    //        // Use the pager widget to page the grid.
    //        // Use the saveSort widget to remember the sort order when the user leaves the page
    //        // Use the stickyHeaders widget so that when the user scrolls down on the grid the headers stay visible
    //        // Use the zebra widget so we get alternating row colors on the grid.
    //        widgets: ["pager", "saveSort", "stickyHeaders", "zebra"],
    //        widgetOptions: {
    //            zebra: ["even", "odd"],
    //            stickyHeaders_attachTo: $(".stickyHeaderGrid"),

    //            // Text that appears in the pager indicating the total number of rows and which page you're curently on
    //            pager_output: '{startRow} to {endRow} of {totalRows} rows',

    //            // Save pager page & size when the page is reloaded
    //            pager_savePages: true,

    //            // css class names of pager arrows
    //            pager_css: {
    //                container: 'tablesorter-pager',
    //                errorRow: 'tablesorter-errorRow', // error information row (don't include period at beginning)
    //                disabled: 'disabled'              // class added to arrows extremes (i.e. prev/first arrows "disabled" on first page)
    //            },

    //            // jQuery selectors which identify the bits and pieces used by the pager
    //            pager_selectors: {
    //                container: '.pager',  // target the pager markup (wrapper)
    //                first: '.first',      // go to first page arrow
    //                prev: '.prev',        // previous page arrow
    //                next: '.next',        // next page arrow
    //                last: '.last',        // go to last page arrow
    //                pageDisplay: '.pagedisplay', // location of where the pager output (1 to 10 of 100 rows) is displayed
    //                pageSize: '.pagesize'     // page size selector - select dropdown that sets the size option
    //            }
    //        }
    //    });
}

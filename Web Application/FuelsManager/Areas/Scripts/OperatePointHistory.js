FMOperateIndex.pointHistorySaveOnColumnResize = true;

// defines the menu to be displayed on headers
FMOperateIndex.PointHistoryCreateHeaderMenu = function () {
  return [
    {
      iconCssClass: "header-menu-cell-alignment",
      title: "Cell Alignment",
      items: [
        {
          iconCssClass: "header-menu-cell-alignment-left",
          title: "Left",
          command: "left-align"
        },
        {
          iconCssClass: "header-menu-cell-alignment-center",
          title: "Center",
          command: "center-align"
        },
        {
          iconCssClass: "header-menu-cell-alignment-right",
          title: "Right",
          command: "right-align"
        }
      ]

    },
    {
      iconCssClass: "header-menu-add-column",
      title: "Insert Column",
      items: [
        {
          iconCssClass: "header-menu-tag",
          title: "Tag",
          command: "insert-column-tag"
        },
        {
          iconCssClass: "header-menu-empty-column",
          title: "Empty Column",
          command: "insert-empty-column"
        }
      ]
    },
    {
      iconCssClass: "header-menu-delete",
      title: "Delete Column",
      command: "delete-column"
    },
    {
      iconCssClass: "header-menu-rename",
      title: "Rename",
      command: "rename"
    },
    {
      iconCssClass: "header-menu-set-display-precision",
      title: "Set Display Precision",
      command: "changeprecision"
    },
    {
      iconCssClass: "header-menu-set-display-unit",
      title: "Set Display Unit",
      command: "changeunit"
    },
    {
      iconCssClass: "header-menu-show-unit",
      title: "Show Units",
      command: "showunits"
    }
  ];
}

FMOperateIndex.OpenNewPointHistory = function (pointId, pointGuid){
  var messageAttributes = '';

  //load settings from DB or default and then open the point history
  $.ajax({
    type: 'get',
    dataType: 'json',
    cache: false,
    url: 'GetOperatePointHistory',
    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
    success: function (response) {
      FMErrorAndExceptionHandling.HandleMessages(response, function () {
        if (!response.Data.Columns) {
          newTabId = FMOperateIndex.openPointHistory(pointId, pointGuid, response.Data.Start, response.Data.Interval, response.Data.IntervalQuantity, response.Data.Range, response.Data.RangeQuantity, null);
        } else {
          newTabId = FMOperateIndex.openPointHistory(pointId, pointGuid, response.Data.Start, response.Data.Interval, response.Data.IntervalQuantity, response.Data.Range, response.Data.RangeQuantity, JSON.parse(response.Data.Columns));
        }
      }, messageAttributes);

    },
    error: function (xhr, textStatus, error) {
    }
  });

}

// update point history settings 
FMOperateIndex.UpdatePointHistorySettings = function (parentControl, pointHistoryControllerId, parentGroupTab) {
	var popover = $(parentControl).popover("destroy").popover({
		container: 'body',
		placement: 'bottom',
		html: true,
		content: $('#PointHistoryConfigurationSettings').html(),
		trigger: "manual"
	});
	var dataPopover = popover.data('bs.popover');
	$(parentControl).popover('show');
	$("#customModalBackground").removeClass("hidden");

	// click on Export CSV (from external service)
	dataPopover.tip().find('.popover-content').find('[name=configurationPointHistoryExportCSV]').on('click', function (event) {

		var grid = FMOperateIndex.staticPointHistoryControllers[pointHistoryControllerId].getGrid();

		var tabId = FMOperateIndex.staticPointHistoryControllers[pointHistoryControllerId]._id;

		var tabName = $("a[data-target='#" + tabId + "'] .tab-name").text();

		const format1 = "_YYYYMMDD-HHmmss";

		var filenameSuffix = moment(new Date()).format(format1);

		var exportCSVPlugin = new Slick.Plugins.ExportCSV({
			separator: ',',
			filename: tabName + filenameSuffix
		});

		grid.registerPlugin(exportCSVPlugin);

		$("#pointgroupExport").html('');
		exportCSVPlugin.exportToElement('#pointgroupExport');
		grid.unregisterPlugin(exportCSVPlugin);

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});

	// close the pop over when clicking cancel
	dataPopover.tip().find('.popover-content').find('[name=configurationPointHistoryCancel]').on('click', function (event) {
    // remove events
    dataPopover.tip().find('.popover-content').find('[name=configurationPointHistoryExportCSV]').off('click');
		dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupCancel]').off('click');

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});
};

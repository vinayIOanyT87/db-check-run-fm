namespace FuelsManager.Areas.App_Start
{
	using System.Web;
	using System.Web.Optimization;

	public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles( BundleCollection bundles )
		{

			BundleTable.EnableOptimizations = true;

			bundles.Add( new ScriptBundle( "~/bundles/jquery" ).Include(
						"~/Scripts/jquery-{version}.js" ) );

			bundles.Add( new ScriptBundle( "~/bundles/jqueryui" ).Include(
						"~/Scripts/jquery-ui-{version}.js" ) );

			bundles.Add( new ScriptBundle( "~/bundles/menu" ).Include(
						"~/MenuBar/FMMenuBar.js" ) );

			bundles.Add( new ScriptBundle( "~/bundles/fmlayout" ).Include(
						"~/Areas/Scripts/Layout.js",
						"~/Areas/Scripts/FMErrorAndExceptionHandling.js",
						"~/Areas/Lib/pnotify.custom.min.js"
                ) );

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
				"~/Scripts/bootstrap.js"));

			bundles.Add(
				new ScriptBundle("~/bundles/tablesorter").Include(
					"~/Areas/Lib/jquery.tablesorter.js",
					"~/Areas/Lib/jquery.tablesorter.widgets.js",
					"~/Areas/Lib/widget-pager.js"));

			bundles.Add(new ScriptBundle("~/bundles/dataTable").Include("~/Areas/Lib/jquery.dataTables.js"));

			bundles.Add(new StyleBundle("~/Content/dataTable").Include("~/Areas/Content/jquery.dataTables.css"));


         bundles.Add(new StyleBundle("~/Areas/Content/css").Include(
              "~/Areas/Content/GeneralStyles.css",
                "~/Areas/Content/pnotify.custom.min.css",
                "~/Areas/Content/pnotify.nonblock.css"
					));

			bundles.Add(new StyleBundle("~/Areas/Content/redmond/css").Include("~/Areas/Content/redmond/jquery.ui.all.css"));

			bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
				"~/Content/bootstrap.css"));

			bundles.Add(new StyleBundle("~/Content/tablesorterThemes").Include("~/Areas/Content/tablesorter/theme.default.css"));
			bundles.Add(new StyleBundle("~/Content/tablesorterPager").Include("~/Areas/Content/tablesorter/jquery.tablesorter.pager.css"));
			bundles.Add(new StyleBundle("~/Content/tablesorterCustom").Include("~/Areas/Content/tablesorter/tablesorterCustom.css"));

         bundles.Add( new StyleBundle( "~/Content/operate" ).Include(
            "~/Areas/Content/slick.grid.css",
            "~/Areas/Lib/controls/slick.columnpicker.css",
            "~/Areas/Lib/plugins/slick.cellcontextmenu.css",
            "~/Areas/Lib/plugins/slick.cellmenu.css",
            "~/Areas/Lib/plugins/slick.contextmenu.css",
            "~/Areas/Lib/plugins/slick.customtooltip.css",
            //"~/Areas/Lib/plugins/slick.headerbuttons.css",
            "~/Areas/Lib/plugins/slick.headercontextmenu.css",
            "~/Areas/Lib/plugins/slick.headerfilterindicator.css",
            "~/Areas/Lib/plugins/slick.headermenu.css",
            "~/Areas/Lib/plugins/slick.rowdetailview.css",
            "~/Areas/Lib/controls/slick.draggablegrouping.css",
            "~/Areas/Lib/controls/slick.gridmenu.css",
            //"~/Areas/Lib/controls/slick.pager.css",
            "~/Areas/Content/jquery.contextmenu.css",
            "~/Areas/Content/OperateIndex.css",
            "~/Areas/Content/TrendDisplay.css",
            "~/Areas/Content/jquery.scrolling-tabs-custom.css",
            "~/Areas/Content/bootstrap-modal-bs3patch.css",
            "~/Areas/Content/bootstrap-modal.css",
            "~/Areas/Lib/DataTables/DataTables-1.10.15/css/dataTables.bootstrap.css",
            "~/Areas/Lib/DataTables/Select-1.2.2/css/select.dataTables.css",
            "~/Areas/Content/select2.css",
            "~/Areas/Content/jquery-ui-timepicker-addon.css",
            "~/Areas/Content/themes/base/jquery.ui.spinner.css",
            "~/Areas/Content/spectrum.css",
            "~/Areas/Content/query-builder.default.css"
      
            ));

			
			bundles.Add(new ScriptBundle("~/bundles/core").Include(
				"~/Scripts/stopwatch.js",
				"~/Scripts/fmcore.js"));


         bundles.Add( new ScriptBundle( "~/bundles/operate" ).Include(
            "~/Areas/Lib/math.js",
            "~/Areas/Lib/NumberFormat.js",
            "~/Areas/Scripts/fmformatvalues.js",
            "~/Areas/Scripts/fmconvertengunits.js",
            "~/Areas/lib/jquery.contextmenu.js",
            "~/Areas/lib/moment.js",
            "~/Areas/Lib/DataTables/DataTables-1.10.15/js/jquery.dataTables.js",
            "~/Areas/Lib/DataTables/Select-1.2.2/js/dataTables.select.js",
            "~/Areas/Lib/DataTables/DataTables-1.10.15/js/dataTables.bootstrap.js",
            "~/Areas/Lib/DataTables/ColReorder-1.3.3/js/dataTables.colReorder.js",
            "~/Areas/Lib/DataTables/FixedColumns-3.2.2/js/dataTables.fixedColumns.js",
            "~/Areas/lib/jquery-ui-timepicker-addon.js",
            "~/Areas/Lib/spectrum.js",
            "~/Areas/Lib/bootstrap-modal.js",
            "~/Areas/Lib/bootstrap-modalmanager.js",
            "~/Areas/lib/jquery.scrolling-tabs-custom.js",
            "~/Areas/lib/jquery.nicescroll.min.js",
            "~/Areas/Scripts/FMErrorAndExceptionHandling.js",
            "~/Areas/Scripts/FMCommon.js",
            "~/Scripts/stopwatch.js",
            "~/Areas/Scripts/fmformatvalues.js",
            "~/Areas/Scripts/fmconvertengunits.js",
            "~/Areas/lib/jquery.contextmenu2.js",
            "~/Areas/Lib/jquery.mask.js",
            "~/Areas/Lib/jquery.numeric.js",
            "~/Areas/Lib/select2.full.js",
            "~/Areas/Lib/printThis.js",
            "~/Areas/lib/GoJS/go-debug.js",
            "~/Areas/scripts/goInit.js",
            "~/Areas/lib/GoJS/extensions/DisconnectedLinkingTool.js",
            "~/Areas/lib/GoJS/extensions/SnappingRelinkingTool.js",
            "~/Areas/lib/GoJS/extensions/SnapLinkReshapingTool.js",
            "~/Areas/lib/GoJS/extensions/GeometryReshapingTool.js",
            "~/Areas/Lib/GoJS/extensions/PolygonDrawingTool.js",
            "~/Areas/lib/GoJS/extensions/DragCreatingLineTool.js",
            "~/Areas/lib/GoJS/extensions/DragCreatingTool.js",
            "~/Areas/lib/GoJS/extensions/DrawCommandHandler.js",
            "~/Areas/lib/GoJS/extensions/RotateMultipleTool.js",
            "~/Areas/lib/GoJS/extensions/DragCreatingLineTool.js",
            "~/Areas/lib/CanvasJS/canvasjs.min.js",
            "~/Areas/scripts/drawindexConstants.js",
            "~/Areas/scripts/drawindexInit.js",
            "~/Areas/scripts/drawindexLayerObjects.js",
            "~/Areas/scripts/drawindexCanvasCollectionObject.js",
            "~/Areas/scripts/drawindex.js",
            "~/Areas/scripts/DrawPropertyMenu.js",
            "~/Areas/scripts/DrawPatternPalette.js",
            "~/Areas/scripts/operateIndex.js",
            "~/Areas/scripts/FMPointGroupGrid.js",
            "~/Areas/scripts/FMPointHistoryGrid.js",
            "~/Areas/scripts/FMMovementSummaryGrid.js",
            "~/Areas/scripts/OperatePointGroup.js",
            "~/Areas/scripts/OperatePointHistory.js",
            "~/Areas/scripts/MovementSummaryTab.js",
            "~/Areas/scripts/OperateMovementSummary.js",
            "~/Areas/scripts/MovementHandgauge.js",
            "~/Areas/Lib/jquery.event.drag-2.3.0.js",
            "~/Areas/Lib/jquery.event.drop-2.3.0.js",

            "~/Areas/Lib/slick.core.js",
            "~/Areas/Lib/slick.editors.js",
            "~/Areas/Lib/slick.formatters.js",
            "~/Areas/Lib/plugins/slick.rowselectionmodel.js",
            "~/Areas/Lib/slick.grid.js",
            "~/Areas/Lib/slick.dataview.js",
            "~/Areas/Lib/controls/slick.columnpicker.js",

            "~/Areas/Lib/plugins/slick.autocolumnsize.js",
            "~/Areas/Lib/plugins/slick.cellcontextmenu.js",
            "~/Areas/Lib/plugins/slick.headercontextmenu.js",
            "~/Areas/Lib/plugins/slick.headerfilterindicator.js",
            "~/Areas/Lib/plugins/slick.rowmovemanager.js",
            "~/Areas/Lib/plugins/slickgrid-print-plugin.js",
            "~/Areas/Lib/plugins/slickgrid-export-csv.js",

            //"~/Areas/Lib/slick.compositeeditor.js",
            //"~/Areas/Lib/slick.groupitemmetadataprovider.js",
            //"~/Areas/Lib/slick.remotemodel-yahoo.js",
            //"~/Areas/Lib/slick.remotemodel.js",
            //"~/Areas/Lib/plugins/slick.autotooltips.js",
            //"~/Areas/Lib/plugins/slick.cellcopymanager.js",
            //"~/Areas/Lib/plugins/slick.cellexternalcopymanager.js",
            //"~/Areas/Lib/plugins/slick.cellmenu.js",
            //"~/Areas/Lib/plugins/slick.cellrangedecorator.js",
            //"~/Areas/Lib/plugins/slick.cellrangeselector.js",
            //"~/Areas/Lib/plugins/slick.cellselectionmodel.js",
            //"~/Areas/Lib/plugins/slick.checkboxselectcolumn.js",
            //"~/Areas/Lib/plugins/slick.contextmenu.js",
            //"~/Areas/Lib/plugins/slick.customtooltip.js",
            //"~/Areas/Lib/plugins/slick.draggablegrouping.js",
            //"~/Areas/Lib/plugins/slick.headerbuttons.js",
            //"~/Areas/Lib/plugins/slick.headermenu.js",
            //"~/Areas/Lib/plugins/slick.resizer.js",
            //"~/Areas/Lib/plugins/slick.rowdetailview.js",
            //"~/Areas/Lib/plugins/slick.state.js",
            //"~/Areas/Lib/controls/slick.gridmenu.js",
            //"~/Areas/Lib/controls/slick.pager.js",

            "~/Areas/scripts/TagSelection.js",
            "~/Areas/Scripts/TrendIndex.js",
            "~/Areas/Scripts/TrendGraph.js",
            "~/Areas/Scripts/TrendMenuBar.js",
            "~/Areas/Scripts/TrendLegend.js",
            "~/Areas/Scripts/PointCalculator.js",
            "~/Areas/Scripts/PointLeakAnalysis.js",
            "~/Areas/Scripts/ShelveDataEntry.js",
            "~/Areas/Scripts/AckCommentDataEntry.js",
            "~/Areas/Scripts/AlarmHistoryTab.js",
            "~/Areas/Scripts/AlarmSummaryTab.js",
            "~/Areas/scripts/KioskKeyRestrictions.js",
            "~/Areas/scripts/MovementHistoryTab.js",
            "~/Areas/scripts/OperateMovementHistory.js"
               ));
      }
	}

	// this is used to rewrite the url's in css's when the application is a virtual in iis 
	// see: http://aspnetoptimization.codeplex.com/workitem/83
	public class CssRewriteUrlTransformWrapper : IItemTransform
	{
		public string Process(string includedVirtualPath, string input)
		{
			// The output of this is ignored if EnableOptimizations = false.
			return new CssRewriteUrlTransform().Process("~" + VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
		}
	}

}
(function ($) {
	// register namespace
	$.extend(true, window, {
		"Slick": {
			"Plugins": {
				"HeaderFilterIndicator": HeaderFilterIndicator
			}
		}
	});


	/***
	 * A plugin to add drop-down menus to column headers.
	 *
	 * USAGE:
	 *
	 * Add the plugin .js & .css files and register it with the grid.
	 *
	 * @param options {Object} Options:
	 *    buttonCssClass:   an extra CSS class to add to the menu button
	 *    buttonImage:      a url to the menu button image (default '../images/down.gif')
	 * @class Slick.Plugins.HeaderButtons
	 * @constructor
	 */
	function HeaderFilterIndicator(options) {
		var _grid;
		var _self = this;
		var _handler = new Slick.EventHandler();
		var _defaults = {
			IndicatorCssClass: "glyphicon glyphicon-filter"
		};

		function init(grid)
		{
			options = $.extend(true, {}, _defaults, options);
			_grid = grid;
			_handler
			  .subscribe(_grid.onHeaderCellRendered, handleHeaderCellRendered)
			  .subscribe(_grid.onBeforeHeaderCellDestroy, handleBeforeHeaderCellDestroy);

			// Force the grid to re-render the header now that the events are hooked up.
			_grid.setColumns(_grid.getColumns());
		}


		function destroy() {
			_handler.unsubscribeAll();
		}

		function handleHeaderCellRendered( e, args )
		{
			var column = args.column;
			var $el = $( "<div></div>" )
				.addClass( "slick-header-filterindicator" );

			if ( args.column.hasOwnProperty( 'filter' ) )
			{
				$el.css( "display", "block" );
				$el.attr( "title", decodeURIComponent(args.column.filter.description) );
				$( args.node ).css( "padding-right", "20px" );
			}
			else
			{
				$( args.node ).css( "padding-right", "4px" );
			}

			if ( options.IndicatorCssClass )
			{
				$el.addClass( options.IndicatorCssClass );
			}

			$el.appendTo(args.node);

		}

		function handleBeforeHeaderCellDestroy(e, args) {
			var column = args.column;

			if (column.header && column.header.menu) {
				$(args.node).find(".slick-header-filterindicator").remove();
			}
		}

		$.extend(this, {
			"init": init,
			"destroy": destroy
		});
	}
})(jQuery);

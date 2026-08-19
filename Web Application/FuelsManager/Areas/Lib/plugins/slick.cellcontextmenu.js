(function ($) {
	// register namespace
	$.extend(true, window, {
		"Slick": {
			"Plugins": {
				"CellContextMenu": CellContextMenu
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
	 * To specify a menu in a cell, extend the grid definition like so:
	 *
	 *   var cellMenu = 
	 *     {
	 *          cellMenu: {
	 *              items: [
	 *                {
	 *                  // menu item options
	 *                },
	 *                {
	 *                  // menu item options
	 *                }
	 *              ]
	 *          }
	 *		  };
	 *
	 *
	 * Available menu options:
	 *    tooltip:      Menu button tooltip.
	 *
	 *
	 * Available menu item options:
	 *    title:        Menu item text.
	 *    disabled:     Whether the item is disabled.
	 *    hidden:		  Whether the item is hidden.
	 *    tooltip:      Item tooltip.
	 *    command:      A command identifier to be passed to the onCommand event handlers.
	 *    iconCssClass: A CSS class to be added to the menu item icon.
	 *    iconImage:    A url to the icon image.
	 *
	 *
	 * The plugin exposes the following events:
	 *    onBeforeMenuShow:   Fired before the menu is shown.  You can customize the menu or dismiss it by returning false.
	 *        Event args:
	 *            grid:     Reference to the grid.
	 *            cell:     cell.
	 *            menu:     Menu options.  Note that you can change the menu items here.
	 *
	 *    onCommand:    Fired on menu item click for buttons with 'command' specified.
	 *        Event args:
	 *            grid:     Reference to the grid.
	 *            column:   Column definition.
	 *            row:      Row Values.
	 *            command:  Button command identified.
	 *
	 *
	 * @param options {Object} Options:
	 *    buttonCssClass:   an extra CSS class to add to the menu button
	 *    buttonImage:      a url to the menu button image (default '../images/down.gif')
	 * @class Slick.Plugins.HeaderButtons
	 * @constructor
	 */
	function CellContextMenu(options) {
		var _grid;
		var _self = this;
		var _handler = new Slick.EventHandler();
		var _defaults = {
			buttonCssClass: null,
			buttonImage: null
		};
		var $menu;
		var $activeHeaderColumn;


		function init(grid) {
			options = $.extend(true, {}, _defaults, options);
			_grid = grid;
			_handler.subscribe(_grid.onContextMenu, showMenu);

			// Hide the menu on outside click.
			$(document.body).on("mousedown", handleBodyMouseDown);
			$(document.body).on("click", handleBodyMouseDown);
		}

		function destroy() {
			_handler.unsubscribeAll();
			$(document.body).off("mousedown", handleBodyMouseDown);
			$(document.body).off("click", handleBodyMouseDown);
		}


		function handleBodyMouseDown(e)
		{
			if ($menu && $menu[0] != e.target && !$.contains($menu[0], e.target)) {
				hideMenu();
			}
		}


		function hideMenu() {
			if ($menu) {
				$menu.remove();
				$menu = null;
			}
		}

		function showMenu(e, gridContainer)
		{
			var _grid = gridContainer.grid;
			var cellClicked = _grid.getCellFromEvent(e);
			if (cellClicked === undefined) {
				e.preventDefault();
				e.stopPropagation();
				return;
			}
			var menu = _grid.getOptions().cellMenu;
			if (!menu) {
				e.preventDefault();
				e.stopPropagation();
				return;
			}

			var columnDef = _grid.getColumns()[cellClicked.cell];
			var rowDef = _grid.getDataItem(cellClicked.row );


			// Let the user modify the menu or cancel altogether,
			// or provide alternative menu implementation.
			if (_self.onBeforeMenuShow.notify({
				 "grid": _grid,
				 "column": columnDef,
				 "row": rowDef,
				 "menu": menu
			}, e, _self) == false) {
				return;
			}


			if (!$menu) {
				$menu = $("<ul class='slick-cellcontext-menu'></ul>")
				  .appendTo(_grid.getContainerNode());
			}
			$menu.empty();


			// Construct the menu items.
			for (var i = 0; i < menu.items.length; i++) {
				var item = menu.items[i];

				// check to see if it has subitems
				if (item.hasOwnProperty('items') && !item.hidden) {
					var $li = $("<li class='slick-cellcontext-menuitem dropdown-submenu'></li>")
						.data("command", item.command || '')
						.data("column", columnDef)
						.data("row", rowDef)
						.data("cellClicked", cellClicked)
						.data("item", item)
						.on("click", handleContextMenuItemClick);

					if (item.disabled) {
						$li.addClass("slick-cellcontext-menuitem-disabled");
					}

					if (item.tooltip) {
						$li.attr("title", item.tooltip);
					}

					var $icon = $("<div class='slick-cellcontext-menuicon'></div>")
						.appendTo($li);

					if (item.iconCssClass) {
						$icon.addClass(item.iconCssClass);
					}

					if (item.iconImage) {
						$icon.css("background-image", "url(" + item.iconImage + ")");
					}

					$("<span class='slick-cellcontext-menucontent'></span>")
						.text(item.title)
						.appendTo($li);

					$("<span class='caret'></span>").appendTo($li);
					var $submenu = $("<ul class='dropdown-menu'></ul>").appendTo($li);

					var submenuitems = menu.items[i].items;
					// Construct the menu items.
					for (var j = 0; j < submenuitems.length; j++) {
						var submenuitem = submenuitems[j];
						if ( !submenuitem.hidden )
						{
							var $sli = $( "<li class='slick-cellcontext-menuitem'></li>" )
								.data( "command", submenuitem.command || '' )
								.data( "column", columnDef )
								.data( "row", rowDef )
								.data( "cellClicked", cellClicked )
								.data( "item", submenuitem )
								.on( "click", handleContextMenuItemClick )
								.appendTo( $submenu );

							if ( submenuitem.disabled )
							{
								$sli.addClass( "slick-cellcontext-menuitem-disabled" );
							}

							if ( submenuitem.tooltip )
							{
								$sli.attr( "title", submenuitem.tooltip );
							}

							var $icon = $( "<div class='slick-cellcontext-menuicon'></div>" )
								.appendTo( $sli );

							if ( submenuitem.iconCssClass )
							{
								$icon.addClass( submenuitem.iconCssClass );
							}

							if ( submenuitem.iconImage )
							{
								$icon.css( "background-image", "url(" + submenuitem.iconImage + ")" );
							}

							$( "<span class='slick-cellcontext-menucontent'></span>" )
								.text( submenuitem.title )
								.appendTo( $sli );
						}
					}

					$li.appendTo($menu);
				}
				else {
					if ( !item.hidden )
					{
						var $li = $( "<li class='slick-cellcontext-menuitem'></li>" )
							.data( "command", item.command || '' )
							.data( "column", columnDef )
							.data( "row", rowDef )
							.data( "cellClicked", cellClicked )
							.data( "item", item )
							.on( "click", handleContextMenuItemClick )
							.appendTo( $menu );

						if ( item.disabled )
						{
							$li.addClass( "slick-cellcontext-menuitem-disabled" );
						}

						if ( item.tooltip )
						{
							$li.attr( "title", item.tooltip );
						}

						var $icon = $( "<div class='slick-cellcontext-menuicon'></div>" )
							.appendTo( $li );

						if ( item.iconCssClass )
						{
							$icon.addClass( item.iconCssClass );
						}

						if ( item.iconImage )
						{
							$icon.css( "background-image", "url(" + item.iconImage + ")" );
						}

						$( "<span class='slick-cellcontext-menucontent'></span>" )
							.text( item.title )
							.appendTo( $li );
					}
				}
			}

			var leftPos = $(e.target).offset().left;
			// check if there is no room to display the menu ( it's 185px width in the CSS ) 
			if ($("body").innerWidth() - $(e.target).offset().left < 185) {
				leftPos = e.clientX - 185;
			}

			// if we have submenus that do not fit on the screen we need to display them on the left of the menu instead of the defaul right
			if ($menu.find('.dropdown-submenu').length > 0) {
				if ($("body").innerWidth() - leftPos - 185 < 165) {
					$menu.find('.dropdown-submenu').addClass('pull-left');
				}
			}
			
			var rowRelativePosition = $(e.target).closest('.slick-row').position().top;
			var gridViewportHeight = $(e.target).closest('.slick-viewport').height();
			var gridViewportTop = $(e.target).closest('.slick-viewport').scrollTop();

			// if the menu will be hidden because it's cut by the end of the grid we need to show it on the top of the selected row
			if ((rowRelativePosition + $menu.outerHeight()) > gridViewportTop + gridViewportHeight)
			{
				// Position the menu below the current row
				$menu.offset({ top: $(e.target).offset().top - $menu.outerHeight(), left: leftPos });

				if ($menu.find('.dropdown-submenu').length > 0)
				{
					$menu.find( '.dropdown-submenu' ).addClass( 'pull-up' );
				}

			}
			else
			{
				// Position the menu below the current row
				$menu.offset({ top: $(e.target).offset().top + $(e.target).height(), left: leftPos });
				if ($menu.find('.dropdown-submenu').length > 0) {
					$menu.find('.dropdown-submenu').each(function (idx, elem) {
						if (rowRelativePosition + $menu.outerHeight() + $(elem).find('ul').outerHeight() > gridViewportTop + gridViewportHeight) {
							$(elem).addClass('pull-up');
						}
					});
				}
			}

			// Stop propagation so that it doesn't register as a header click event.
			e.preventDefault();
			e.stopPropagation();
		}


		function handleContextMenuItemClick(e)
		{
			var command = $(this).data("command");
			var columnDef = $(this).data("column");
			var rowDef = $(this).data("row");
			var item = $(this).data("item");
			var cellClicked = $(this).data("cellClicked");

			if (item.disabled || command === "" ) {
				return;
			}
			hideMenu();

			if (command != null && command != '') {
				_self.onCommand.notify({
					"grid": _grid,
					"column": columnDef,
					"row": rowDef,
					"cellClicked": cellClicked,
					"command": command,
					"item": item
				}, e, _self);
			}

			// Stop propagation so that it doesn't register as a header click event.
			e.preventDefault();
			e.stopPropagation();
		}

		$.extend(this, {
			"init": init,
			"destroy": destroy,

			"onBeforeMenuShow": new Slick.Event(),
			"onCommand": new Slick.Event()
		});
	}
})(jQuery);

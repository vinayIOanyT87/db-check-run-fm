// This is a custom version of this plug in to ae it work in FuelsManager.  It is used to print Point Groups

(function ($) {
	'use strict';

	var SlickPrint = function () {

		var _self = this;
		var _grid;

		this.init = function (grid) {
			_grid = grid;
		};

		this.printToHtml = function () {
			var numRows = _grid.getDataLength();
			var columns = _grid.getColumns();
			var numCols = columns.length;
			var r, c;
			var rows = [], cols = [], headers = [];
			var cellNode;
			var topRow = _grid.getRenderedRange().top;

			columns.forEach(function (col) {
				headers.push('<th class="' + col.headerCssClass + '">' + col.name + '</th>');
			});

			Slick.GlobalEditorLock.cancelCurrentEdit();

			_grid.scrollRowToTop(0);

			for (r = 0; r < numRows; r++) {
				cols = [];
				for (c = 0; c < numCols; c++) {
					cellNode = _grid.getCellNode(r, c);
					if (!cellNode) {
						_grid.scrollCellIntoView(r, c, true);
						cellNode = _grid.getCellNode(r, c);
					}
					cols.push('<td class="' + cellNode.className.replace("slick-cell", "").replace("ui-state-default", "") + '">' + $(cellNode).html() + '</td>');
				}
				rows.push(cols.join(''));
			}

			var table = [
				 '<table class="table table-bordered pointgroupprint">',
				 '<thead>',
				 '<tr>',
					  headers.join(''),
				 '</tr>',
				 '</thead>',
				 '<tbody>',
					  '<tr>' + rows.join('</tr>\n<tr>') + '</tr>',
				 '</tbody>',
				 '</table>'
			].join('\n');

			_grid.scrollRowToTop(topRow);

			return table;
		};

		this.printToElement = function ($element) {
			$($element).html(_self.printToHtml());

			// remove empty columns
			while ($($element + " thead > tr > th:last").html() === "") {
				$($element + ' tr').find('td:last,th:last').remove();
			}

		};

		this.printToWindow = function (w) {
			w.onload = function () {
				setTimeout(function () {
					_self.printToElement(w.document.body);
				});
			};
		};
	};

	// register namespace
	$.extend(true, window, {
		Slick: {
			Plugins: {
				Print: SlickPrint
			}
		}
	});
}(jQuery));
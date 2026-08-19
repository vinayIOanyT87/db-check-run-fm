// This is a custom slick-grid plug in used to export Point Groups

(function ($) {
	'use strict';

	var ExportCSV = function (options) {

		var _self = this;
		var _grid;
		var _defaults = {
			separator: ',',
			filename: "pointGroupExport"
		};

		this.init = function (grid) {
			options = $.extend(true, {}, _defaults, options);
			_grid = grid;
		};

		this.getCSVData = function () {

			try {
				var numRows = _grid.getDataLength();
				var columns = _grid.getColumns();
				var numCols = columns.length;
				var r, c;
				var rows = [], cols = [], headers = [];
				var cellNode;
				var topRow = _grid.getRenderedRange().top;

				// strip out empty columns
				const emptyColumns = [];
				for (let i = 0; i < numCols; i++) {
					if (columns[i].name.trim().length <= 0) {
						emptyColumns.push(i);
					}
				}

				for (let i = emptyColumns.length - 1; i >= 0; i--) {
					columns.splice(emptyColumns[i], 1);
				}

				numCols = columns.length;

				columns.forEach(function (col) {
					headers.push(col.name);
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

						var cellData = cellNode.innerText;

						var finalData = '';
						var cellDatum = cellData && cellData.split('\n');

						if (cellDatum.length > 0) {
							finalData = cellDatum[0].replace(/,/g, '');
						}

						if (cellDatum.length > 1) {
							finalData += ' [' + cellDatum[1] + ']';
						}
						if (cellDatum.length > 2) {
							finalData += ' [' + cellDatum[2] + ']';
						}

						cols.push(finalData);
					}
					var row = cols.join(options.separator);

					// strip out empty rows
					row = row.replace(/^[", ]*$/gm, '');

					if (row && row.trim().length > 0) {
						rows.push(cols.join(options.separator));
					}
				}

				var data = [
					headers.join(options.separator),
					rows.join('\n'),
				].join('\n');

				_grid.scrollRowToTop(topRow);

				return data;
			}
			catch (ex) {
				return ex;
			}
		};

		this.exportToElement = function ($element) {
			var csvFile = _self.getCSVData();

			//The Unicode character \UFEFF is the byte order mark, or BOM, and is used to tell the difference between big- and little-endian UTF-16 encoding
			var blob = new Blob(["\uFEFF" + csvFile], { type: 'text/csv;charset=UTF-8;' });
			if (navigator.msSaveBlob) { // IE 10+
				navigator.msSaveBlob(blob, options.filename + ".csv");
			} else {
				var link = document.createElement("a");
				if (link.download !== undefined) { // feature detection
					// Browsers that support HTML5 download attribute
					var url = URL.createObjectURL(blob);

					link.setAttribute("href", url);
					link.setAttribute("download", options.filename + ".csv");
					link.style.visibility = 'hidden';
					document.body.appendChild(link);
					link.click();
					// delete the internal blob reference, to let the browser clear memory from it
					URL.revokeObjectURL(link.href);
					document.body.removeChild(link);
				}
			}
		};

		this.saveToElement = function ($element) {
			var csvFile = _self.getCSVData();
			$($element).html(csvFile);
		};
	};

	// register namespace
	$.extend(true, window, {
		"Slick": {
			"Plugins": {
				"ExportCSV": ExportCSV
			}
		}
	});
}(jQuery));
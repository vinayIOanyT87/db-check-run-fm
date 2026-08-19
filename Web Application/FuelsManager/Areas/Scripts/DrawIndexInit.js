//Extend Array to do a deep copy clone
Object.defineProperty( Array.prototype, 'clone', {
    enumerable: false,
    writable: true,
    value: function()
    {
        return this.slice( 0 );
    }
} );

var undobutton = undobutton || undefined;
var redobutton = redobutton || undefined;
var flipverticalbutton = flipverticalbutton || undefined;
var fliphorizontalbutton = fliphorizontalbutton || undefined;
var exportbutton = exportbutton || undefined;
var xCordText = xCordText || undefined;
var yCordText = yCordText || undefined;

//Define FMDrawPlatform Object
var FMDrawIndex = {
	activeTabIndex: 0,
	activeTabCanvasContainerIndex: 0 // The active Container Index in the tablCanvasContainerCollection
	,
	tabCanvasContainerCollection: [],
	checkChangesEnabled: true,
	defaultShapeStrokeWidth: 2,
	defaultLineStrokeWidth: 2,
	shapeStrokeColor: 'black',
	shapeFillColor: '#99ccff',
	shapeRealStrokeWidth: 2,
	confirmSave: true // the user wishes to overwrite
	,
	currentDrawControl: '',
	selectionStyle: {
		cornerColor: 'black',
		transparentCorners: false,
		cornerSize: 10
	},
	shapeDragged: false,
	currentContextMenu: null,
	panningEnabled: true,
	unitTypeToUnitMap: new go.Map( 'string', go.Set ), //Store Units list by type in efficient go.Map
	percentViewPortSize: 60,
	clipboardDiagram: null,
	transactionTag: "new tags",
	transactionBar: "InitializeBar",
	switchingTags: false,
	defaultArchetype: {
		color: '#99ccff',
		textAlign: 'center',
		strokeWidth: 2,
		font: '12px sans-serif',
		isUnderline: false,
		strokeDashArray: [0, 0],
		stroke: '#000000',
		lineStroke: '#000000',
		patternFillColor: '#99ccff',
		transparency: 0,
		patternStrokeColor: '#ffffff',
		lineStyleTransparency: 0
	}
};

var FMDrawPatternPalette = {};
var FMDrawPropertyMenu = {};
var FMDrawAnimation = {};


// Polyfills
if (!Array.prototype.findIndex) {
	Object.defineProperty(Array.prototype, 'findIndex', {
		value: function (predicate) {
			'use strict';
			if (this == null) {
				throw new TypeError('Array.prototype.findIndex called on null or undefined');
			}
			if (typeof predicate !== 'function') {
				throw new TypeError('predicate must be a function');
			}
			var list = Object(this);
			var length = list.length >>> 0;
			var thisArg = arguments[1];
			var value;

			for (var i = 0; i < length; i++) {
				value = list[i];
				if (predicate.call(thisArg, value, i, list)) {
					return i;
				}
			}
			return -1;
		},
		enumerable: false,
		configurable: false,
		writable: false
	});
}
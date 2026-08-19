// The CanvasExtensions scope object.  Variables and functions specific to the CanvasExtensions
// interface should be added to this object rather than the global windows object.
var CanvasExtensions = {};

/**
* Draws a rounded rectangle using the current state of the canvas. 
* If you omit the last three params, it will draw a rectangle 
* outline with a 5 pixel border radius 
* @param {CanvasRenderingContext2D} ctx
* @param {number} x The top left x coordinate
* @param {number} y The top left y coordinate 
* @param {number} width The width of the rectangle 
* @param {number} height The height of the rectangle
* @param {number} radius The corner radius. Defaults to 5;
* @param {boolean} fill Whether to fill the rectangle. Defaults to false.
* @param {boolean} stroke Whether to stroke the rectangle. Defaults to true.
*/
CanvasExtensions.roundRect = function(ctx, x, y, width, height, radius, fill, stroke) {
	if (typeof stroke == "undefined") {
		stroke = true;
	}

	if (typeof fill == "undefined") {
		fill = true;
	}

	if (typeof radius == "undefined") {
		radius = 5;
	}

	ctx.beginPath();
	ctx.moveTo(x + radius, y);
	ctx.lineTo(x + width - radius, y);
	ctx.quadraticCurveTo(x + width, y, x + width, y + radius);
	ctx.lineTo(x + width, y + height - radius);
	ctx.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
	ctx.lineTo(x + radius, y + height);
	ctx.quadraticCurveTo(x, y + height, x, y + height - radius);
	ctx.lineTo(x, y + radius);
	ctx.quadraticCurveTo(x, y, x + radius, y);
	ctx.closePath();

	if (fill) {
		ctx.save();
		ctx.shadowOffsetX = 3;
		ctx.shadowOffsetY = 3;
		ctx.shadowBlur = 4;
		ctx.shadowColor = "#cacaca";

		ctx.fill();

		ctx.restore();
	}

	if (stroke) {
		ctx.stroke();
	}
};

CanvasExtensions.clearCanvas = function(context) {
	// Store the current transformation matrix
	context.save();

	// Use the identity matrix while clearing the canvas
	context.setTransform(1, 0, 0, 1, 0, 0);
	context.clearRect(0, 0, context.canvas.width, context.canvas.height);

	// Restore the transform
	context.restore();
};

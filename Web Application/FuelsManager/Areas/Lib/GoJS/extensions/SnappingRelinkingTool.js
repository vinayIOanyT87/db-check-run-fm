function SnappingRelinkingTool() {
	go.RelinkingTool.call(this);
}
go.Diagram.inherit(SnappingRelinkingTool, go.RelinkingTool);

SnappingRelinkingTool.prototype.doMouseMove = function () {

	var diagram = this.diagram;
	if (diagram.toolManager.draggingTool.isGridSnapEnabled === true) {
		var e = this.diagram.lastInput;
		var grid = this.diagram.grid;
		e.documentPoint = e.documentPoint.copy().snapToGrid(grid.gridOrigin.x, grid.gridOrigin.y, diagram.model.modelData.snapXCellSize, diagram.model.modelData.snapYCellSize);
		e.viewPoint = e.diagram.transformDocToView(e.documentPoint);
	}
	go.RelinkingTool.prototype.doMouseMove.call(this);
}


SnappingRelinkingTool.prototype.doMouseUp = function () {

	var diagram = this.diagram;
	if (diagram.toolManager.draggingTool.isGridSnapEnabled === true) {
		var e = this.diagram.lastInput;
		var grid = this.diagram.grid;
		e.documentPoint = e.documentPoint.copy().snapToGrid(grid.gridOrigin.x, grid.gridOrigin.y, diagram.model.modelData.snapXCellSize, diagram.model.modelData.snapYCellSize);
		e.viewPoint = e.diagram.transformDocToView(e.documentPoint);
	}
	go.RelinkingTool.prototype.doMouseUp.call(this);
}

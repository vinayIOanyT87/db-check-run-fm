function DisconnectedLinkingTool() {
	//go.Tool.call(this);
	go.LinkingTool.call(this);
	this.name = 'DisconnectedLinkingTool';

	this.isUnconnectedLinkValid = true;
	this._fakeStartPort = null;
	this.createdLink = null;
	this.temporaryLink.routing = go.Link.Orthogonal;
}
go.Diagram.inherit(DisconnectedLinkingTool, go.LinkingTool);

DisconnectedLinkingTool.prototype.canStart = function () {
	if (!this.isEnabled) return false;
	var diagram = this.diagram;
	if (diagram === null || diagram.isReadOnly || diagram.isModelReadOnly) return false;
	if (!diagram.allowLink) return false;
	var model = diagram.model;
	if (!(model instanceof go.GraphLinksModel) && !(model instanceof go.TreeModel)) return false;
	// require left button & that it has moved far enough away from the mouse down point, so it isn't a click
	if (!diagram.lastInput.left) return false;
	// don't include the following check when this tool is running modally
	if (diagram.currentTool !== this) {
		if (!this.isBeyondDragSize()) return false;
	}
	var port = this.findLinkablePort();
	if (port === null) {
		var $ = go.GraphObject.make;
		this._fakeStartPort = this.startObject =
        $(go.Shape, { width: 1, height: 1, portId: "", fromLinkable: true });
		var node =
        $(go.Node,
          { layerName: "Tool", locationSpot: go.Spot.Center, location: diagram.firstInput.documentPoint },
          this.startObject);
		diagram.add(node);
		node.ensureBounds();
	}
	return true;
};

DisconnectedLinkingTool.prototype.insertLink = function (fromnode, fromport, tonode, toport) {
	if (this._fakeStartPort !== null) {
		fromnode = fromport = null;
	}
	this.startTransaction("link");
	var link = go.LinkingTool.prototype.insertLink.call(this, fromnode, fromport, tonode, toport);
	if (link !== null) {
		link.defaultFromPoint = this.diagram.firstInput.documentPoint.copy();
		link.routing = go.Link.Orthogonal;
		link.category = this.archetypeNodeData.category;
		link.layerName = this.archetypeNodeData.layerName;
		//link.zOrder = this.archetypeNodeData.zOrder;
		var copy = JSON.parse(JSON.stringify(FMDrawIndex.defaultArchetype)); //copying default node data to get current default properties
		this.archetypePartData = copy;
		this.archetypePartData.category = 'lineLink';
		var layerManager = new FMDrawIndex._LayerManager();
		var primaryLayerName = layerManager.GetPrimaryLayerName();
		this.archetypePartData.layerName = primaryLayerName;
		this.archetypePartData.zOrder = FMDrawIndex.GetNextPartZOrder(primaryLayerName);
		link.stroke = this.archetypeNodeData.stroke;
		this.diagram.model.setDataProperty(link.data, 'width', this.archetypePartData.strokeWidth);
		this.diagram.model.setDataProperty(link.data, 'color', this.archetypePartData.color);
		this.diagram.model.setDataProperty(link.data, 'zOrder', this.archetypePartData.zOrder);
	}
	createdLink = link;
	return link;
};

DisconnectedLinkingTool.prototype.doStop = function () {
	if (this._fakeStartPort !== null) {
		this.diagram.remove(this._fakeStartPort.part);
		this._fakeStartPort = null;
	}
	go.LinkingTool.prototype.doStop.call(this);
	this.stopTransaction();
};
// end of DisconnectedLinkingTool
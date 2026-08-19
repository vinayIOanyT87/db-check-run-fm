FMDrawAnimation =
    {

    };

//====================================================================================
// This function will open the animation dialog.
//====================================================================================
FMDrawAnimation.OpenAnimationDialog = function (EnablePointSelect)
{
    var animationPointValueAssignments = null;
    var enablePointSelect = EnablePointSelect;
    if ((FMDrawPropertyMenu.PropertyActiveObject === null || FMDrawPropertyMenu.PropertyActiveObject === undefined) && EnablePointSelect == true) {
        enablePointSelect = false;
    }
    if (FMDrawPropertyMenu.PropertyActiveObject && FMDrawPropertyMenu.PropertyActiveObject.data && EnablePointSelect === true)
    {

		var activeData = FMDrawPropertyMenu.PropertyActiveObject.data;
		animationPointValueAssignments = activeData.AnimationPointValueAssignments;
	}
	var tagSelectionMetaData = AnimationManager.CreateTagSelectionMetaDataObject(FMDrawIndex.SaveLastTagSelectionModel,
                                                                                    FMDrawIndex.GetLastTagSelectionModel,
                                                                                    FMDrawIndex.FinishOpenTagSelectionDialog,
                                                                                    $('#urlTagSelectionGetPointListWithPanelTemplateContextByDataType').val(), $('#urlTagSelectionGetPointListByDataType').val(), FMDrawIndex.GetDiagramModelDataValue('PointTemplateGuid'),
                                                                                    FMDrawIndex.GetDiagramModelDataValue('PanelType'),
                                                                                    '#PointSelection');
	AnimationManager.GetForm($('#urlAnimationManagerView').val(),
                                FMDrawAnimation.FinishOpenAnimationDialog,
                                "#AnimationManager",
                                tagSelectionMetaData,
                                FMDrawIndex.DeactivateTextEditingTool,
                                animationPointValueAssignments,
                                enablePointSelect);

};


//====================================================================================
// This function will finish opening the animation dialog.
//====================================================================================
FMDrawAnimation.FinishOpenAnimationDialog = function (success)
{
	if (success)
	{
		$('#AnimationManagerScreen').modal('show');
	}
};

//====================================================================================
// This function will finish opening the animation dialog.
//====================================================================================
FMDrawAnimation.OkButtonPress = function ()
{
	if ( AnimationManager.AnimationNewModalScreenShowing === false && AnimationManager.AnimationPointValueModalScreenShowing === false )
	{
		AnimationManager.SaveAnimation();
		var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
		var dataModel = diagram.model;
		if (FMDrawPropertyMenu.PropertyActiveObject && FMDrawPropertyMenu.PropertyActiveObject.data && AnimationManager.enablePointSelect === true)
		{
			var activeData = FMDrawPropertyMenu.PropertyActiveObject.data;
			var pointValueList = AnimationManager.GetAnimationManagerModel().PointValueList;
			dataModel.setDataProperty(activeData, 'AnimationPointValueAssignments', pointValueList);
		}
		$( '#AnimationManagerScreen' ).modal( 'hide' );
		FMDrawPropertyMenu.SetAnimationID();
    }
	return true;
};

FMDrawAnimation.AppendArray = function( fromArray, toArray )
{
	if ( !toArray )
	{
		toArray = [];
	}
	if ( !fromArray || fromArray.length === 0 )
	{
		return toArray;
	}
	for ( var i = 0; i < fromArray.length; i++ )
	{
		toArray.push( fromArray[i] );
	}
	return toArray;
};

FMDrawAnimation.GetNodeAnimationGuids = function( o )
{
	var allNodes = [];
	var isNode = o instanceof go.Node;
	var isGroup = o instanceof go.Group;
	var isLink = o instanceof go.Link;
	var isNodeOrGroup = isNode || isGroup || isLink;
	if ( isNodeOrGroup )
	{
		if ( o.data )
		{
			var data = o.data;
			if ( data.AnimationPointValueAssignments )
			{
				var pointValueAssignments = data.AnimationPointValueAssignments;
				if ( pointValueAssignments.AnimationGuid )
				{
					var animationGuid = pointValueAssignments.AnimationGuid;
					allNodes.push(animationGuid);
					if (o.memberParts)
					{
						var subNodes = o.memberParts;
						subNodes.each(function (p) {
							var subAllNodes = FMDrawAnimation.GetNodeAnimationGuids(p);
							allNodes = FMDrawAnimation.AppendArray(subAllNodes, allNodes);
						});
					}

				}
			}
		}
	}
	return allNodes;
};

FMDrawAnimation.GetAllAnimationGuids = function( diagram )
{
	var allNodes = [];
	diagram.nodes.each( function( o )
	{
		var subAllNodes = FMDrawAnimation.GetNodeAnimationGuids( o );
		allNodes = FMDrawAnimation.AppendArray(subAllNodes, allNodes);
	});

	diagram.links.each(function (link) {
		var subAllNodes = FMDrawAnimation.GetNodeAnimationGuids(link);
		allNodes = FMDrawAnimation.AppendArray(subAllNodes, allNodes);
	});


	return FMDrawAnimation.RemoveStringDuplicates(allNodes);
};

FMDrawAnimation.RemoveStringDuplicates = function( originalArray )
{
	var obj = {};
	var returnArray = [];
	if ( !originalArray || originalArray.length < 1 )
	{
		return returnArray;
	}
	for ( var i = 0; i < originalArray.length; i++ )
	{
		var a = originalArray[i];
		obj[a] = a;
	}
	for ( var key in obj )
	{
		if ( obj.hasOwnProperty( key ) )
		{
			returnArray.push( key );
		}
	}
	return returnArray;
};

FMDrawAnimation.OpenCloseTests = function ()
{
    AnimationManager.SetCollapsed();
}
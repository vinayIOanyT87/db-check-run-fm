FMDrawIndex = FMDrawIndex || {};

FMDrawIndex._LayerDO = function( layerName, layerDisplayName, active, visible )
{
    this.layerName = layerName;
    this.layerDisplayName = layerDisplayName;
    this.active = active;
    this.visible = visible;
};

FMDrawIndex._LayerManager = function()
{
    this.__proto__.AddLayer = function( pLayerName, pDisplayName, pIsVisible, pIsActive )
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        if ( !diagram )
        {
            return null;
        }
        var isVisible = pIsVisible;
        var isActive = pIsActive;
        var layerName = pLayerName;
        var displayName = pDisplayName;
        var layerNumber = 0;
        if (!(isVisible || isVisible === false))
        {
        	isVisible = true;
        }
        if (!(isActive || isActive === false))
        {
        	isActive = true;
        }
        if ( !layerName )
        {
            layerNumber = this.GetNextLayerNumber();
            layerName = ( defaultLayerString + layerNumber );
        }
        if ( !displayName )
        {
            displayName = layerName;
        }
        var goJsLayer = this.GetLayerByDisplayName( displayName );
        while ( goJsLayer )
        {
            layerNumber++;
            layerName = ( defaultLayerString + layerNumber );
            displayName = layerName;
            goJsLayer = this.GetLayerByDisplayName( displayName );
        }
        var foreGroundLayer = diagram.findLayer( 'Foreground' );
        diagram.startTransaction( 'add layer' );
        goJsLayer = $$(go.Layer, { name: layerName, visible: isVisible, allowSelect: isActive });
        goJsLayer.displayName = displayName;
        diagram.addLayerBefore( goJsLayer, foreGroundLayer );
        diagram.commitTransaction( 'add layer' );
        return goJsLayer;
    };

    this.__proto__.IsValidLayerParameters = function( layerName, displayName )
    {
        if ( !layerName || !displayName )
        {
            return false;
        }
        if ( !this.IsLayerNameValid( layerName ) || this.IsLayerNameInUse( layerName ) )
        {
            return false;
        }
        if ( !this.IsLayerDisplayNameValid( displayName ) || this.IsLayerDisplayNameInUse( displayName ) )
        {
            return false;
        }
        return true;
    };

    this.__proto__.GetNextLayerNumber = function()
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        var layerIndex = 0;
        var index = 0;
        diagram.layers.each( function( layer )
        {
            if ( systemLayers.indexOf( layer.name.toLowerCase() ) < 0 )
            {
                //index = this.GetLayerNumber(layer.name);
                if ( layer.name.indexOf( defaultLayerString ) >= 0 )
                {
                    index = Number( layer.name.substring( defaultLayerString.length ) );
                }
                if ( index > layerIndex )
                {
                    layerIndex = index;
                }
            }
        } );
        layerIndex++;
        return layerIndex;
    };

    this.__proto__.GetNextLayer = function( layerName )
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        var targetLayer = undefined;
        var it = diagram.layers;
        var layer;
        var firstLayer = null;
        while ( it.next() )
        {
            layer = it.value;
            if ( systemLayers.indexOf( layer.name.toLowerCase() ) >= 0 )
            {
                continue;
            }
            if ( firstLayer == null )
            {
                firstLayer = layer;
            }
            if ( layer.name.toLowerCase() === layerName.toLowerCase() )
            {
                targetLayer = it.value;
                break;
            }
        }
        if ( !targetLayer )
        {
            return undefined;
        }
        if ( it.next() )
        {
            targetLayer = it.value;
            if ( systemLayers.indexOf( targetLayer.name.toLowerCase() ) >= 0 )
            {
                targetLayer = firstLayer;
            }
        }
        else
        {
            targetLayer = firstLayer;
        }
        return targetLayer;
    };

    this.__proto__.GetLayerNumber = function( layerName )
    {
        var index = 0;
        if ( layerName.indexOf( defaultLayerString ) >= 0 )
        {
            index = layerName.substring( defaultLayerString.length );
        }
        return Number( index );
    };

    this.__proto__.GetLayerByName = function( layerName )
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        var targetLayer = diagram.findLayer( layerName );
        return targetLayer;
    };

    this.__proto__.GetLayerByDisplayName = function( displayName )
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        var targetLayer = undefined;
        diagram.layers.each( function( layer )
        {
            if ( ( layer.displayName ) && ( layer.displayName.toLowerCase() === displayName.toLowerCase() ) )
            {
                targetLayer = layer;
            }
        } );
        return targetLayer;
    };

    this.__proto__.GetLayers = function()
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        var layers = [];
        var layer;
        var it = diagram.layers;
        while ( it.next() )
        {
            layer = it.value;
            if ( systemLayers.indexOf( layer.name.toLowerCase() ) >= 0 )
            {
                continue;
            }
            layers.push( layer );
        }
        return layers;
    };

    this.__proto__.SetPrimaryLayer = function( layerName )
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        var layer = this.GetLayerByName( layerName );
        if ( layer )
        {
            diagram.startTransaction( 'set primary layer' );
            layer.visible = true;
            layer.allowSelect = true;
            var oldval = diagram.primaryLayerName;
            diagram.primaryLayerName = layerName;
            diagram.model.raiseDataChanged(diagram, "primaryLayerName", oldval, layerName);
            diagram.commitTransaction( 'set primary layer' );
        }
    };

    this.__proto__.GetPrimaryLayerName = function()
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        if ( !diagram )
        {
            return 'Foreground';
        }
        return diagram.primaryLayerName;
    };

    this.__proto__.GetPrimaryLayer = function()
    {
        var primaryLayerName = this.GetPrimaryLayerName();
        return this.GetLayerByName( primaryLayerName );
    };

    this.__proto__.DeleteLayer = function( targetLayerName )
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        var targetLayer = this.GetLayerByName( targetLayerName );
        var primaryLayerName = this.GetPrimaryLayerName();
        var nextLayer = null;
        if ( targetLayerName === primaryLayerName )
        {
            nextLayer = this.GetNextLayer( targetLayerName );
        }
        if (targetLayer != null)
        {
            diagram.startTransaction( 'clear layer' );
            diagram.removeParts( targetLayer.parts );
            diagram.removeLayer( targetLayer );
            if ((targetLayerName === primaryLayerName)  && nextLayer)
            {
                var oldval = diagram.primaryLayerName;
                diagram.primaryLayerName = nextLayer.name;
                diagram.model.raiseDataChanged(diagram, "primaryLayerName", oldval, nextLayer.name);
            }
            diagram.commitTransaction( 'clear layer' );
        }
    };

    this.__proto__.ChangeLayerDisplayName = function( layerName, newDisplayName )
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        var targetLayer = this.GetLayerByName( layerName );
        if ( !targetLayer )
        {
            return false;
        }
        if ( ( typeof newDisplayName !== 'string' ) || ( !this.IsLayerDisplayNameValid( newDisplayName ) ) )
        {
            return false;
        }
        var layer = this.GetLayerByDisplayName( newDisplayName.trim() );
        if ( !layer || ( layer.name === layerName ) )
        {
            diagram.startTransaction('rename layer');
            var oldval = targetLayer.displayName;
            targetLayer.displayName = newDisplayName.trim();
            diagram.model.raiseDataChanged(targetLayer, "displayName", oldval, newDisplayName.trim());
            diagram.commitTransaction( 'rename layer' );
            return true;
        }
        return false;
    };

    this.__proto__.SetVisible = function( layerName, isVisible )
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        var layer = this.GetLayerByName( layerName );
        if ( !layer )
        {
            return;
        }
        diagram.startTransaction( 'set layer visibility' );
        layer.visible = isVisible;
        if ( !isVisible )
        {
            layer.allowSelect = false;
        }
        diagram.commitTransaction( 'set layer visibility' );
    };

    this.__proto__.SetAllowSelect = function( layerName, isActive )
    {
        var diagram = FMDrawIndex.GetActiveTabGoJSDiagramObject();
        // ReSharper disable once InconsistentNaming
        var layerManager = new FMDrawIndex._LayerManager();
        var layer = layerManager.GetLayerByName( layerName );
        if ( !layer )
        {
            return;
        }
        diagram.startTransaction( 'set layer allowSelect' );
        layer.allowSelect = isActive;

       var newSelectedNodeArr = [];
       var selectedParts = diagram.selection;

	    if ( selectedParts )
	    {
		    selectedParts.each( function( p )
		    {

		    	if ( p.layer.name !== layerName )
			    {
				    newSelectedNodeArr.push( p );
			    }
		    });
		    diagram.clearSelection();

		    if (newSelectedNodeArr.length !== 0)
		    {
		    	diagram.selectCollection(newSelectedNodeArr);
		    }
	    }

        diagram.commitTransaction( 'set layer allowSelect' );
    };

    this.__proto__.IsLayerNameValid = function( layerName )
    {
        if ( ( typeof layerName != 'string' ) || ( layerName.trim().length === 0 ) )
        {
            return false;
        }
        var layerNumber = this.GetLayerNumber( layerName );
        if ( ( !layerNumber ) || ( layerNumber <= 0 ) )
        {
            return false;
        }
        return true;
    };

    this.__proto__.IsLayerNameInUse = function( layerName )
    {
        var layer = this.GetLayerByName( layerName.trim() );
        if ( layer )
        {
            return true;
        }
        return false;
    };

    this.__proto__.IsLayerDisplayNameValid = function( layerName )
    {
        if ( ( typeof layerName != 'string' ) || ( layerName.trim().length === 0 ) )
        {
            return false;
        }
        return true;
    };

    this.__proto__.IsLayerDisplayNameInUse = function( layerDisplayName, ignoreLayerName )
    {
        var layer = this.GetLayerByDisplayName( layerDisplayName.trim() );
        if ( !layer )
        {
            return false;
        }
        if ( ( !ignoreLayerName ) || ( layer.name !== ignoreLayerName ) ) 
        {
            return true;
        }
        return false;
    };
};
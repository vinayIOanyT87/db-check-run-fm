// Object to manage Metadata for the Operate Point Group grids
var FMPointGroupGrid = function FMPointGroupGrid( id, grid, description, visibility, fontSize, owner, isOwnByMe, isEditable )
{
	var self = this;
	this._metadata = [];
	this._grid = grid;
	this._id = id;
	this.description = description;
	this.visibility = visibility;
	this.fontSize = fontSize;
	this.owner = owner;
	this.isOwnByMe = isOwnByMe;
	this.isEditable = isEditable;
	this._uniqueId = FMOperateIndex.newGuid();
	this._updateDynamicAjaxRequest = null;
	this._updateDynamicTimer = null;

	var gridData = this._grid.getData().getItems();

	jQuery.each( gridData, function( idx, elem )
	{
		if ( elem.hasOwnProperty( "pointguid" ) && elem.pointguid && FMOperateIndex.isGuid( elem.pointguid ) )
		{
			self._metadata.push({ point: elem.point, pointguid: elem.pointguid, tags: [] });

			// we need to update the metadata for static point groups (not filtered) since the metadata is already returned by the results for dynamic
			if ( !self.isDynamic() )
			{
				//self._refreshMetadataForPoint( elem.pointguid );
			}
		}
	} );

	var pointGuids = self._metadata.map((x => x.pointguid));
	self._refreshMetadataForAllPoints(pointGuids);
}

// Instance methods
FMPointGroupGrid.prototype = {

	isDynamic: function()
	{
		var isDynamic = false;
		// make sure we are still using a dynamic point group (we may have switch to a static point group before receiving this data)
		var columns = this._grid.getColumns();
		if ((columns.length > 0 && columns[0].hasOwnProperty('filter'))) {
			isDynamic = true;
		}
		return isDynamic;
	},

	// add a row ( Point ) to the grid	
	addRow: function( rowDefinition )
	{
		var existingPoint = jQuery.grep( this._metadata, function( elem, idx )
		{
			return ( elem.pointguid && rowDefinition.pointguid && elem.pointguid.toUpperCase() === rowDefinition.pointguid.toUpperCase() );
		} );

		if ( existingPoint.length > 0 )
		{
			existingPoint[0].point = rowDefinition.point;
		}
		else
		{
			this._metadata.push( { point: rowDefinition.point, pointguid: rowDefinition.pointguid, tags: [] } );
			if ( rowDefinition.pointguid )
			{
				this._refreshMetadataForPoint( rowDefinition.pointguid );
			}
		}
	},

	deleteRow: function( rowDefinition )
	{
		// if the row has a point then we need to update the metadata
		if ( rowDefinition.pointguid )
		{
			var pointGuid = rowDefinition.pointguid;
			var pointCount = 0;
			$.each(this._grid.getData().getItems(), function (key, value)
			{
				if ( value.pointguid === pointGuid )
				{
					pointCount++;
				}
			} );
			// if there is only one row for that point guid and we are trying to delete it we can remove it from the metadata
			if ( pointCount === 1 )
			{
				this._metadata = $.grep( this._metadata, function( e )
				{
					return e.pointguid !== pointGuid;
				} );

				var _uniqueId2 = this._uniqueId;
				var _metadata2 = this._metadata;
				var _grid2 = this._grid;
				var id = this._id;

				FMOperateIndex.staticPointGroupControllers[id]._subscribeUpdateTags(_uniqueId2, _metadata2, _grid2);
			}
		}
	},


	// add a column ( tag ) to the grid	
	addColumn: function( columnDefinition )
	{
		if ( columnDefinition.field )
		{
			this._refreshMetadataForNewTag( columnDefinition.field );
		}
	},

	// return the metadata object
	getMetadata: function ()
	{
		return this._metadata;
	},
	// set the metadata object
	setMetadata: function ( metadata ) {
		this._metadata = metadata;
	},
	getGrid: function () {
		return this._grid;
	},
	// return the metadata object
	geUniqueId: function () {
		return this._uniqueId;
	},
	convertToStaticPointGroup: function()
	{
		this._metadata = [];
		this._subscribeUpdateTags(this._uniqueId, this._metadata, this._grid);
	},

	convertToDynamicPointGroup: function () {
		FMOperateIndex.subscribeDynamicPointGroup(this._uniqueId);
	},

	// update metadata when a row in added
	_refreshMetadataForPoint: function( newPointGuid )
	{
		// get the list of unique columns (tags) we need the tag guids for
		var listColumns = this._grid.getColumns();
		var tagNames = [];
		jQuery.each( listColumns, function( idx, elem )
		{
			if ( elem.hasOwnProperty( "field" ) && elem.field && elem.field !== "" )
			{
				if ( jQuery.inArray( elem.field, tagNames ) === -1 )
				{
					tagNames.push( elem.field );
				}
			}
		} );
		// create copies of the context so we can use it in other functions
		var _uniqueId2 = this._uniqueId;
		var _metadata2 = this._metadata;
		var _grid2 = this._grid;
		var id = this._id;

		// call the server to retrieve the metadata
		$.ajax( {
			url: 'GetPointMetadataForPointGroup',
			type: 'Post',
			dataType: 'json',
			cache: false,
			data: JSON.stringify({
				"pointguid": newPointGuid,
				"tags": tagNames
			}),
			success: function( response )
			{
				FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
				{
					// if it was not in error load and update the drawing
					if ( !inError )
					{
						var existingPoint = jQuery.grep( _metadata2, function( elem, idx )
						{
							return ( elem.pointguid.toUpperCase() === newPointGuid.toUpperCase() );
						} );

						if ( existingPoint.length > 0 )
						{
							existingPoint[0].tags = data;
						}

						FMOperateIndex.staticPointGroupControllers[id]._subscribeUpdateTags( _uniqueId2, _metadata2, _grid2 );
					}
				} );
			},
			error: function( request, status, error )
			{
				FMErrorAndExceptionHandling.ShowException( request, status, error, function()
				{
				} );
			}
		} );
	},

	// update metadata when a row in added
	_refreshMetadataForAllPoints: function (pointGuids) {
		// get the list of unique columns (tags) we need the tag guids for
		var listColumns = this._grid.getColumns();
		var tagNames = [];
		jQuery.each(listColumns, function (idx, elem) {
			if (elem.hasOwnProperty("field") && elem.field && elem.field !== "") {
				if (jQuery.inArray(elem.field, tagNames) === -1) {
					tagNames.push(elem.field);
				}
			}
		});
		// create copies of the context so we can use it in other functions
		var _uniqueId2 = this._uniqueId;
		var _metadata2 = this._metadata;
		var _grid2 = this._grid;
		var id = this._id;

		// call the server to retrieve the metadata
		$.ajax({
			url: 'GetAllPointsMetadataForPointGroup',
			type: 'Post',
			dataType: 'json',
			cache: false,
			data: JSON.stringify({
				"pointguids": pointGuids,
				"tags": tagNames
			}),
			success: function (response) {
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
					// if not in error match the returned points to the points currently in the metadata array
					// and update.
					if (!inError) {
						jQuery.each(pointGuids, function (idx, newPointGuid) {
							var existingPoint = jQuery.grep(_metadata2, function (elem, idx) {
								return (elem.pointguid.toUpperCase() === newPointGuid.toUpperCase());
							});

							if (existingPoint.length > 0 && newPointGuid in data) {
								existingPoint[0].tags = data[newPointGuid];
							}
						});

						FMOperateIndex.staticPointGroupControllers[id]._subscribeUpdateTags(_uniqueId2, _metadata2, _grid2);
					}
				});
			},
			error: function (request, status, error) {
				FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
				});
			}
		});
	},

	// update metadata when a column in added
	_refreshMetadataForNewTag: function( tagId )
	{
		// we need to update the metadata for static point groups (not filtered) since the metadata is already returned by the results for dynamic
		if (this.isDynamic())
		{
			return;
		}

		// get the list of unique rows (points) we need the tag guids for
		var gridData = this._grid.getData().getItems();

		var PointGuids = [];

		jQuery.each( gridData, function( idx, elem )
		{
			if ( elem.hasOwnProperty( "pointguid" ) && elem.pointguid && FMOperateIndex.isGuid( elem.pointguid ) )
			{
				if ( jQuery.inArray( elem.pointguid, PointGuids ) === -1 )
				{
					PointGuids.push( elem.pointguid );
				}
			}
		} );
		// create copies of the context so we can use it in other functions
		var _uniqueId2 = this._uniqueId;
		var _metadata2 = this._metadata;
		var _grid2 = this._grid;
		var id = this._id;

		if ( PointGuids.length > 0 )
		{
			// call the server to retrieve the metadata
			$.ajax( {
				url: 'GetTagMetadataForPointList',
				type: 'Post',
				dataType: 'json',
				cache: false,
				data: JSON.stringify({
					"points": PointGuids,
					"tagName": tagId
				}),
				success: function( response )
				{
					FMErrorAndExceptionHandling.HandleMessages( response, function( data, inError )
					{
						// if it was not in error load and update the drawing
						if ( !inError )
						{
							// for each new tag that we got back check the points in the metadata and update them 
							data.forEach( function( tagValue )
							{
								// look for the tag in the grid list so we can find the point so we can identify the row
								var foundPoint = $.grep( _metadata2, function( point )
								{
									return tagValue.PointGuid.toUpperCase() === point.pointguid.toUpperCase();
								} );
								if ( foundPoint.length > 0 )
								{
									var foundTag = $.grep( foundPoint[0].tags, function( tag )
									{
										return tagValue.PointValueIdentifier_IdentityGuid.toUpperCase() === tag.PointValueIdentifier_IdentityGuid.toUpperCase() &&
											tagValue.PointValueIdentifier_PointValueType === tag.PointValueIdentifier_PointValueType &&
											tagValue.PointValueIdentifier_PropertyID === tag.PointValueIdentifier_PropertyID;
									} );
									if ( foundTag.length > 0 )
									{
										// update existing tag
										foundTag[0].DecimalPlaces = tagValue.DecimalPlaces;
										foundTag[0].Maximum = tagValue.Maximum;
										foundTag[0].Minimum = tagValue.Minimum;
										foundTag[0].Units = tagValue.Units;
										foundTag[0].QualityAbbreviation = tagValue.QualityAbbreviation;
									}
									else
									{
										// add tag to the point
										foundPoint[0].tags.push( tagValue );
									}
									FMOperateIndex.staticPointGroupControllers[id]._subscribeUpdateTags(_uniqueId2, _metadata2, _grid2);
								}
							} );
						}
					} );
				},
				error: function( request, status, error )
				{
					FMErrorAndExceptionHandling.ShowException( request, status, error );
				}
			} );
		}
	},

	_subscribeUpdateTags: function( uniqueId2, metadata2, grid2 )
	{
		var pointValueList = [];
		
		metadata2.forEach( function( point )
		{
			point.tags.forEach( function( pointValue )
			{
				var newPointValue = {
					IdentityGuid: pointValue.PointValueIdentifier_IdentityGuid,
					PointValueType: pointValue.PointValueIdentifier_PointValueType,
					PropertyID: pointValue.PointValueIdentifier_PropertyID  === "" ? null: pointValue.PointValueIdentifier_PropertyID
				};

				pointValueList.push(newPointValue);
			} );
		} );

		FMOperateIndex.subscribeTagWebWorker(this._uniqueId, pointValueList, function (data)
		{
			FMPointGroupGrid.updateGrid(data, uniqueId2, metadata2, grid2);
			// update totals
			FMPointGroupGrid.updateGridTotals(metadata2, grid2);
		}, false );
	},

	getNumericUnitsForTag: function( tagName )
	{
		var unitType = [];

		this._metadata.forEach( function( point )
		{
			point.tags.forEach( function( tag )
			{
				if ( tag.ID === tagName )
				{
					if ( unitType.indexOf( tag.EngineeringUnitsType ) < 0 )
					{
						unitType.push( tag.EngineeringUnitsType );
					}
				}
			} );
		} );
		return unitType;
	},
	getValueTypesForTag: function (tagName) {
		var unitType = [];

		this._metadata.forEach(function (point) {
			point.tags.forEach(function (tag) {
				if (tag.ID === tagName) {
					if (unitType.indexOf(tag.ValueTypeString) < 0) {
						unitType.push(tag.ValueTypeString);
					}
				}
			});
		});
		return unitType;
	},
	getWellKnownGuidsForTag: function (tagName) {
		var WellKnownIdentityGuid = [];
		this._metadata.forEach(function (point) {
			point.tags.forEach(function (tag) {
				if (tag.ID === tagName) {
					if (typeof tag.WellKnownIdentityGuid != 'undefined') {
						if (WellKnownIdentityGuid.indexOf(tag.WellKnownIdentityGuid.toUpperCase()) < 0) {
							WellKnownIdentityGuid.push(tag.WellKnownIdentityGuid.toUpperCase());
						}
					}
				}
			});
		});
		return WellKnownIdentityGuid;
	},


	// generic formatter for static point groups
	staticPointGroupFormatter: function( row, cell, value, columnDef, dataContext )
	{
		if ( value === undefined || value === '' || value === null )
		{
			var foundPoint = $.grep(this._metadata, function (point) {
				return dataContext.pointguid && point.pointguid && dataContext.pointguid.toUpperCase() === point.pointguid.toUpperCase();
			});

			if (foundPoint.length > 0)
			{
				var foundTag = $.grep(foundPoint[0].tags, function (tag)
				{
					if ( columnDef.field )
					{
						return columnDef.field.toUpperCase() === tag.ID.toUpperCase();
					}
					else
						return false;
				});
				// if we found the metadata for the tag then we need to check the formatting
				if ( foundTag.length > 0 )
				{
					var returnData = "";

					// get the tank status class based on tank status
					var returnStyle = '';
					var currentTankStatus = "Not Found";

					if (foundPoint[0].tags.length > 0) {
						for (var iloop = 0; iloop < foundPoint[0].tags.length; iloop++) {
							if (foundPoint[0].tags[iloop].WellKnownIdentityGuid.toUpperCase() === '834B9D8A-C17A-48F6-97FC-1B18EB562866') {
								currentTankStatus = foundPoint[0].tags[iloop].Value;;
								break;
							}
						}
					}

					if (currentTankStatus !== "Not Found") {
						returnStyle = "Class='";

						if (currentTankStatus === "Stopped") {
							returnStyle += "tankstatusStopped";
						}
						else if (currentTankStatus === "Emptying") {
							returnStyle += "tankstatusEmptying";
						}
						else if (currentTankStatus === "Filling") {
							returnStyle += "tankstatusFilling";
						}
						else if (currentTankStatus === "Running") {
							returnStyle += "tankstatusRunning";
						}
						else if (currentTankStatus === "Testing") {
							returnStyle += "tankstatusTesting";
						}
						else {
							returnStyle += "tankstatusBad";
						}

						returnStyle += "'";
					}

					var cellData = "<div style='padding-left: 3px; padding-right: 3px;' ";

					cellData += returnStyle;

					cellData += " title=''>";

					//this is used when the tag status is not good bds
					if (foundTag[0].CommunicationsFailure)
					{
						returnData = cellData + $('#CommunicationsFailureText').val() + "</div>";

						return returnData;
					}
					
					else if (foundTag[0].Access && !(foundTag[0].Access.View || foundTag[0].Access.Modify)) 
					{
						returnData = cellData + $('#RestrictedText').val() + "</div>";

						return returnData;
					}
					
					else if (foundTag[0].Status && (foundTag[0].Status & 0x80000000))
					{
						returnData = cellData + FMOperateIndex.GetStatusCode(foundTag[0].Status) + "</div>";

						return returnData;
					}
					
					else
					{
						// null value with status good
						returnData = cellData + "" + "</div>";

						return returnData;
					}
				}	// end if foundtag.length
			}
			else
			{
				// this is an empty column since there is no point associted with this just leave it blank
			}
			return '';
		}
		if (dataContext.type && (dataContext.type === "subtotal" || dataContext.type === "total"))
		{
			return this.staticTotalizerFormatter( row, cell, value, columnDef, dataContext );
		}
		return this.staticPointFormatter( row, cell, value, columnDef, dataContext );
	},
	
	// format total/subtotal cells in the grid
	staticTotalizerFormatter: function( row, cell, value, columnDef, dataContext )
	{
		if ( columnDef.field === 'point' )
		{
			return '<i>' + value + '</i>';
		}

		var rowGuid = dataContext.totalizerGuid;
		// if no value
		if ( !columnDef.totalizerValue || !columnDef.totalizerValue[rowGuid] )
		{
			return '';
		}
		// if cell is not configured for calculation
		if (!columnDef.totalizerConfig || !columnDef.totalizerConfig[rowGuid] || columnDef.totalizerConfig[rowGuid] === 'none') {
			return '';
		}

		// for Total and subtotals we are getting the value stored in the column object as <value>||<unit>||<precision>
		var formattingTotals = columnDef.totalizerValue[rowGuid].split('||');
		if ( formattingTotals.length === 4 )
		{
			if (formattingTotals[0] === '' || formattingTotals[1] === '' || formattingTotals[2] === '' || formattingTotals[3] === '')
			{
				return '';
			}
			if ( formattingTotals[3] === 'Numeric' )
			{
				var defaultValue = parseFloat( formattingTotals[0] );
				var defaultUnit = parseInt( formattingTotals[1] );
				var defaultPrecision = parseInt( formattingTotals[2] );

				// override the precision with the one from the column definition
				if ( columnDef.hasOwnProperty( 'DecimalPlaces' ) )
				{
					if ( columnDef.DecimalPlaces > -1 )
					{
						defaultPrecision = columnDef.DecimalPlaces;
					}
				}

				FMOperateIndex.numformatInfo.NumberDecimalDigits = defaultPrecision;
				// if the unit set for the value is different than the one for the column we need to do a conversion
				if ( columnDef.Unit && columnDef.Unit !== -1 && columnDef.Unit !== defaultUnit )
				{
					defaultValue = FMConvertEngUnits.Convert( defaultValue, defaultUnit, columnDef.Unit );
					defaultUnit = columnDef.Unit;
				}

				var newFormattedValue = FMFormatValues.FormatValue( defaultUnit, FMOperateIndex.numformatInfo, defaultValue );
				//display the unit
				if ( columnDef.showunit )
				{
					newFormattedValue += '<div class="supsub"><sub class="subscript">' + FMConvertEngUnits.GetEngineeringUnitAbbreviation( defaultUnit ) + '</sub></div>';
				}
				return '<i><b>' + newFormattedValue + '</b></i>';
			} else if ( formattingTotals[3] === 'DateTimeOffset' )
			{
				var newFormattedValue = "";
				// set the value as the one stored in the metadata
				if (formattingTotals[0] != null && formattingTotals[0] != "" && formattingTotals[0] != "undefined") {
					var re = /-?\d+/;
					var m = re.exec(formattingTotals[0]);
					newFormattedValue = new Date(parseInt(m[0]));
					newFormattedValue = moment(newFormattedValue).format(FMOperateIndex.numformatInfo.ShortDatePattern.toUpperCase() + " " + FMOperateIndex.numformatInfo.TimePattern.replace("tt", "A"));
				}
				return '<i><b>' + newFormattedValue + '</b></i>';

			} else if ( formattingTotals[3] === 'TimeSpan' )
			{
				var newFormattedValue = "";
				// set the value as the one stored in the metadata
				if ( formattingTotals[0] != null && formattingTotals[0] != "" && formattingTotals[0] != "undefined" )
				{
					var tempTotal = JSON.parse( formattingTotals[0] );
					newFormattedValue = (tempTotal.Days > 0 ? tempTotal.Days + "." : "") + ("000000" + tempTotal.Hours).slice(-2) + ":" + ("000000" + tempTotal.Minutes).slice(-2) + ":" + ("000000" + tempTotal.Seconds).slice(-2);
				}
				return '<i><b>' + newFormattedValue + '</b></i>';
			}
			else
			{
				return '';
			}
		}
		else
		{
			return '<i><b>' + columnDef.totalizerValue[rowGuid] + '</b></i>';
		}
	},

	// format point tags for display in the grid
	staticPointFormatter: function( row, cell, value, columnDef, dataContext )
	{
		var newFormattedValue = value;
		var foundPoint = $.grep(this._metadata, function (point) {
			return dataContext.pointguid && point.pointguid && dataContext.pointguid.toUpperCase() === point.pointguid.toUpperCase();
		});
		if (foundPoint.length > 0) {
			var foundTag = $.grep(foundPoint[0].tags, function (tag) {
				return columnDef.field.toUpperCase() === tag.ID.toUpperCase();
			});
			// if we found the metadata for the tag then we need to check the formatting
			if (foundTag.length > 0) {
				if (foundTag[0].ValueTypeString === "System.Double")
				{
					var unit = parseInt( foundTag[0].Units );
					var precision = parseInt( foundTag[0].DecimalPlaces );
					value = foundTag[0].Value; // set the value as the one stored in the metadata

					// override the precision with the one from the column definition
					if ( columnDef.hasOwnProperty( 'DecimalPlaces' ) )
					{
						if ( columnDef.DecimalPlaces > -1 )
						{
							precision = columnDef.DecimalPlaces;
						}
					}
					// override the unit with the one from the column definition
					if ( columnDef.hasOwnProperty( 'Unit' ) )
					{
						if ( columnDef.Unit > -1 && columnDef.Unit !== unit )
						{
							// if the units from the point and column are different we need to do a conversion.
							value = FMConvertEngUnits.Convert( value, unit, columnDef.Unit );
							unit = columnDef.Unit;
						}
					}

					FMOperateIndex.numformatInfo.NumberDecimalDigits = precision;
					//newFormattedValue = '<p style="background-color:red;">' + FMFormatValues.FormatValue( unit, FMOperateIndex.numformatInfo, value ) + '</p>';
					newFormattedValue = FMFormatValues.FormatValue(unit, FMOperateIndex.numformatInfo, value);


					newFormattedValue += '<div class="supsub">';
					//display the quality
					if (columnDef.showquality) {
						newFormattedValue += '<sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup>';
					}

					//display the unit
					if ( columnDef.showunit )
					{
						newFormattedValue += '<sub class="subscript">' + FMConvertEngUnits.GetEngineeringUnitAbbreviation(unit) + '</sub>';
					}
					newFormattedValue += '</div>';
				}
				else if ( foundTag[0].ValueTypeString === "System.Single" )
				{
					var unit = parseInt( foundTag[0].Units );
					var precision = parseInt( foundTag[0].DecimalPlaces );
					value = foundTag[0].Value; // set the value as the one stored in the metadata

					// override the precision with the one from the column definition
					if ( columnDef.hasOwnProperty( 'DecimalPlaces' ) )
					{
						if ( columnDef.DecimalPlaces > -1 )
						{
							precision = columnDef.DecimalPlaces;
						}
					}
					// override the unit with the one from the column definition
					if ( columnDef.hasOwnProperty( 'Unit' ) )
					{
						if ( columnDef.Unit > -1 && columnDef.Unit !== unit )
						{
							// if the units from the point and column are different we need to do a conversion.
							value = FMConvertEngUnits.Convert( value, unit, columnDef.Unit );
							unit = columnDef.Unit;
						}
					}

					FMOperateIndex.numformatInfo.NumberDecimalDigits = precision;
					newFormattedValue = FMFormatValues.FormatValue( unit, FMOperateIndex.numformatInfo, value );

					newFormattedValue += '<div class="supsub">';
					//display the quality
					if (columnDef.showquality) {
						newFormattedValue += '<sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup>';
					}

					//display the unit
					if ( columnDef.showunit )
					{
						newFormattedValue += '<sub class="subscript">' + FMConvertEngUnits.GetEngineeringUnitAbbreviation(unit) + '</sub>';
					}
					newFormattedValue += '</div>';
				}


				else if (foundTag[0].ValueTypeString === "System.Int16"
				|| foundTag[0].ValueTypeString === "System.Int32"
				|| foundTag[0].ValueTypeString === "System.Int64"
				|| foundTag[0].ValueTypeString === "System.UInt16"
				|| foundTag[0].ValueTypeString === "System.UInt32"
				|| foundTag[0].ValueTypeString === "System.UInt64") {
					var unit = parseInt(foundTag[0].Units);
					var precision = 0;
					value = foundTag[0].Value; // set the value as the one stored in the metadata

					// override the unit with the one from the column definition
					if (columnDef.hasOwnProperty('Unit')) {
						if (columnDef.Unit > -1 && columnDef.Unit !== unit) {
							// if the units from the point and column are different we need to do a conversion.
							value = FMConvertEngUnits.Convert(value, unit, columnDef.Unit);
							unit = columnDef.Unit;
						}
					}

					FMOperateIndex.numformatInfo.NumberDecimalDigits = precision;
					newFormattedValue = FMFormatValues.FormatValue(unit, FMOperateIndex.numformatInfo, value);

					newFormattedValue += '<div class="supsub">';
					//display the quality
					if (columnDef.showquality) {
						newFormattedValue += '<sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup>';
					}

					//display the unit
					if (columnDef.showunit) {
						newFormattedValue += '<sub class="subscript">' + FMConvertEngUnits.GetEngineeringUnitAbbreviation(unit) + '</sub>';
					}
					newFormattedValue += '</div>';
				}


				else if (foundTag[0].ValueTypeString === "System.DateTime") {
					newFormattedValue = "";
					// set the value as the one stored in the metadata
					if (value != null || value != "") {
						var re = /-?\d+/;
						var m = re.exec(value);
						newFormattedValue = new Date(parseInt(m[0]));
						newFormattedValue = moment(newFormattedValue).format(FMOperateIndex.numformatInfo.ShortDatePattern.toUpperCase() );
					}

					if (columnDef.showquality) {
						newFormattedValue += '<div class="supsub"><sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup></div>';
					}
				}
				else if (foundTag[0].ValueTypeString === "System.DateTimeOffset") {
					newFormattedValue = "";
					// set the value as the one stored in the metadata
					 if (value != null || value != "") {
						  var timezoneOffsetStr = $("#TimezoneOffsetString").val();
						  var timezoneOffset = parseInt(timezoneOffsetStr);
						var re = /-?\d+/;
						var m = re.exec(value);
						newFormattedValue = new Date(parseInt(m[0]));
						 newFormattedValue = moment(newFormattedValue);
						 newFormattedValue = newFormattedValue.subtract(newFormattedValue.utcOffset(), 'minutes'); //go to UTC time
						  newFormattedValue = newFormattedValue.add(timezoneOffset, 'minutes'); //go to site time
						  newFormattedValue = moment(newFormattedValue).format(FMOperateIndex.numformatInfo.ShortDatePattern.toUpperCase() + " " + FMOperateIndex.numformatInfo.TimePattern.replace("tt", "A"));
					}

					if (columnDef.showquality) {
						newFormattedValue += '<div class="supsub"><sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup></div>';
					}
				}

				else if (foundTag[0].ValueTypeString === "System.TimeSpan")
				{
					newFormattedValue = "";
					// set the value as the one stored in the metadata
					if ( value != null || value != "" )
					{
						newFormattedValue = (foundTag[0].Value.Days > 0 ? foundTag[0].Value.Days + "." : "") + ("000000" + foundTag[0].Value.Hours).slice(-2) + ":" + ("000000" + foundTag[0].Value.Minutes).slice(-2) + ":" + ("000000" + foundTag[0].Value.Seconds).slice(-2);
					}

					if (columnDef.showquality) {
						newFormattedValue += '<div class="supsub"><sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup></div>';
					}
				}
				else if (foundTag[0].ValueTypeString === "FMBusinessObjects.DataObjects.PointCommandStatusListReference") {
					newFormattedValue = "";
					if (foundTag[0].Value != null && foundTag[0].Value.CurrentKey != null)
					{
						newFormattedValue = foundTag[0].Value.CurrentKey;
					}
					if (columnDef.showquality) {
						newFormattedValue += '<div class="supsub"><sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup></div>';
					}
				}
				else if (foundTag[0].ValueTypeString === "FMBusinessObjects.DataObjects.DeviceAlarmMapReference") {
					newFormattedValue = "";
					if (foundTag[0].Value != null && foundTag[0].Value.CurrentValue != null) {
						newFormattedValue = foundTag[0].Value.CurrentValue;
					}
					if (columnDef.showquality) {
						newFormattedValue += '<div class="supsub"><sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup></div>';
					}
				}
				else
				{
					if ( columnDef.showquality )
					{
						newFormattedValue += '<div class="supsub"><sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup></div>';
					}
				}

				if ((foundTag[0].hasOwnProperty("AlarmPriorityGuid") && foundTag[0].AlarmPriorityGuid !== "")
					&& (foundTag[0].Access && (foundTag[0].Access.View || foundTag[0].Access.modify)))
				{
					// add the alarm style
					var alarmcellStyle = "<div style='padding-left: 3px; padding-right: 3px;' class='AlarmPriority-" + (foundTag[0].Acknowledged ? "" : "blink-") + foundTag[0].AlarmPriorityGuid;

					// add the tank status style
					alarmcellStyle += FMPointGroupGrid.setTankStatusStyle(false, foundPoint[0].tags);
					alarmcellStyle += "' title='" + (!!foundTag[0].AlarmState ? foundTag[0].AlarmState : "") + "'>";
					newFormattedValue = alarmcellStyle + newFormattedValue + "</div>";
				}
				else
				{
					var celldata = "<div style='padding-left: 3px; padding-right: 3px;' ";
					celldata += FMPointGroupGrid.setTankStatusStyle(true, foundPoint[0].tags);
					celldata +=  " title=''>";
					newFormattedValue = celldata + newFormattedValue + "</div>";
				}

			}
			else
			{
				// bds column 0 which is the point name

				//if (cell !== 0)
				{
					var celldata = "<div style='padding-left: 3px; padding-right: 3px;' ";
					celldata += FMPointGroupGrid.setTankStatusStyle(true, foundPoint[0].tags);
					celldata += " title=''>";
					newFormattedValue = celldata + newFormattedValue + "</div>";
				}
			}
		}
		else
		{
		}
		return newFormattedValue;
	}

};

FMPointGroupGrid.setTankStatusStyle = function(addClass,tags)
{
	var returnStyle = '';
	var currentTankStatus = "Not Found";

	if (tags.length > 0)
	{
		//bds
		for(var iloop = 0;iloop < tags.length;iloop++)
		{
			if(tags[iloop].WellKnownIdentityGuid.toUpperCase() === '834B9D8A-C17A-48F6-97FC-1B18EB562866')
			{
				currentTankStatus = tags[iloop].Value;;
				break;
			}
		}
	}

	if (currentTankStatus !== "Not Found")
	{
		if (addClass == true)
		{
			returnStyle = "Class='";
		}
		else
		{
			returnStyle = " ";
		}
		if (currentTankStatus === "Stopped")
		{
			returnStyle += "tankstatusStopped";
		}
		else if (currentTankStatus === "Emptying")
		{
			returnStyle += "tankstatusEmptying";
		}
		else if (currentTankStatus === "Filling")
		{
			returnStyle += "tankstatusFilling";
		}
		else if (currentTankStatus === "Running")
		{
			returnStyle += "tankstatusRunning";
		}
		else if (currentTankStatus === "Testing")
		{
			returnStyle += "tankstatusTesting";
		}
		else
		{
			returnStyle += "tankstatusBad";
		}
		if (addClass == true)
		{
			returnStyle += "'";
		}

	}
	return returnStyle;
}

// populate the grid with the results from the server
FMPointGroupGrid.updateGrid = function (data, uniqueId, metadata, grid)
{

	if (data.length > 0)
	{
		var dataView = grid.getData();
		var gridData = dataView.getItems();

		// create a list of all point/tags in the grid
		var listAllTagsInGrid = [];
		metadata.forEach(function (point) {
			var pointguid = point.pointguid;
			var tagsForPoint = point.tags.map(function (elem) {
				return jQuery.extend(elem, { pointguid: pointguid });
			});
			listAllTagsInGrid = listAllTagsInGrid.concat(tagsForPoint);
		});

		// for each tag retrieved look to see if we have it in the new list
		data.forEach(function (tagValue) {
			// look for the tag in the grid list so we can find the point so we can identify the row
			var foundTag = $.grep(listAllTagsInGrid, function (pointValueIdentifier) {
				return tagValue.PointValueIdentifier_IdentityGuid === pointValueIdentifier.PointValueIdentifier_IdentityGuid &&
					tagValue.PointValueIdentifier_PointValueType === pointValueIdentifier.PointValueIdentifier_PointValueType &&
					tagValue.PointValueIdentifier_PropertyID === pointValueIdentifier.PointValueIdentifier_PropertyID;
			});
			if (foundTag.length > 0)
			{
				// get the list of rows ( we could have multiple instances of the same point / tag in the grid
				var pointguid = foundTag[0].pointguid;
				var tagID = foundTag[0].ID;

				// override the display settings with the latest ones received
				if ( tagValue.hasOwnProperty( 'Units' ) )
				{
					foundTag[0].Units = tagValue.Units;
				}
				if (tagValue.hasOwnProperty('DecimalPlaces')) {
					foundTag[0].DecimalPlaces = tagValue.DecimalPlaces;
				}
				foundTag[0].Value = tagValue.Value;
				foundTag[0].ValueTypeString = tagValue.ValueTypeString;
				foundTag[0].Acknowledged = tagValue.Acknowledged;
				foundTag[0].AlarmPriorityGuid = tagValue.AlarmPriorityGuid;
				foundTag[0].AlarmState = tagValue.AlarmState;
				foundTag[0].QualityAbbreviation = tagValue.QualityAbbreviation;
				foundTag[0].Access = (tagValue.Access ? tagValue.Access : { View: false, Modify: false, ExceedRange: false, Override: false });
				foundTag[0].InputOutputType = tagValue.InputOutputType;
				foundTag[0].Status = tagValue.Status;
				foundTag[0].CommunicationsFailure = tagValue.CommunicationsFailure;


				gridData = $.map(gridData, function (elem, idx) {
					if (elem.pointguid === pointguid)
					{
						var newFormattedValue = tagValue.Value;
						if (tagValue.ValueTypeString === "System.Double")
						{
							if ( newFormattedValue != null )
							{
								newFormattedValue = '0'; // the grid formatter will take care of the numbers
							}
						}
						else if (tagValue.ValueTypeString === "FMBusinessObjects.DataObjects.PointCommandStatusListReference") {
							newFormattedValue = (tagValue.Value !== null && tagValue.Value.CurrentKey !== null) ? tagValue.Value.CurrentKey : "";
						}
						else if (tagValue.ValueTypeString === "FMBusinessObjects.DataObjects.DeviceAlarmMapReference") {
							newFormattedValue = (tagValue.Value !== null && tagValue.Value.CurrentValue !== null) ? tagValue.Value.CurrentValue : "";
						}
						elem[tagID] = newFormattedValue;
						return elem;
					}
					return elem;
				});

			}
		});

		FMOperateIndex.updateFilterParameters(grid, metadata);

		dataView.beginUpdate();
		dataView.setItems(gridData);
		dataView.endUpdate();

		// update totals
		FMPointGroupGrid.updateGridTotals(metadata, grid);

		grid.invalidateAllRows();
		grid.render();
	}

}

// calculate all total/subtotals in the grid
FMPointGroupGrid.updateGridTotals = function (metadata, grid)
{
	var rows = grid.getData().getFilteredItems();
	var columns = grid.getColumns();
	var subTotals = [];
	var totalRowNum = -1;
	// get list of totals and subtotals
	jQuery.each(rows, function (index, item) {
		if (item.type && item.type === "subtotal") {
			subTotals.push(index);
		}
		if (item.type && item.type === "total") {
			totalRowNum = index;
		}
	});

	// collect subtotal values
	var lastSubTotal = 0;
	jQuery.each(subTotals, function (index, item) {
		var subtotalMetadata = [];

		for ( var i = lastSubTotal; i < item; i++ )
		{
			// if a row with a point look for it in the metadata
			if (rows[i].pointguid)
			{
				// look for the tag in the grid list so we can find the point and identify the row
				var foundPoint = $.grep(metadata, function (point) {
					return rows[i].pointguid.toUpperCase() === point.pointguid.toUpperCase();
				});
				if (foundPoint.length > 0)
				{
					subtotalMetadata.push(foundPoint[0]);
				}
			}
		}
		lastSubTotal = item;

		FMPointGroupGrid.gridTotalizer(lastSubTotal, subtotalMetadata, rows, columns);
	});
	
	// collect Totals
	if ( totalRowNum !== -1 )
	{
		var totalMetadata = [];

		for (var i = 0; i < rows.length; i++) {
			// if a row with a point look for it in the metadata
			if (rows[i].pointguid) {
				// look for the tag in the grid list so we can find the point so we can identify the row
				var foundPoint = $.grep(metadata, function (point) {
					return rows[i].pointguid.toUpperCase() === point.pointguid.toUpperCase();
				});
				if (foundPoint.length > 0) {
					totalMetadata.push(foundPoint[0]);
				}
			}
		}
		FMPointGroupGrid.gridTotalizer(totalRowNum, totalMetadata, rows, columns);
	}

}


// calculate a single total/subtotal in the grid
FMPointGroupGrid.gridTotalizer = function (totalRowIdx, rowsMetadata, rows, columns) {
	// we are going to actually store the value in the column header since the same result could apply to multiple columns (multiple columns for the same tag) 
	// but display different things based on the column configuration. For example different units 

	jQuery.each(columns, function (colIndex, columnElement) {
		if (!columnElement.totalizerValue) {
			columnElement.totalizerValue = {};
		}

		var rowGuid = rows[totalRowIdx].totalizerGuid;
		// only do calculation if the column is setup for a calculation (ignore if there is no configuration or set to none )
		if (!columnElement.totalizerConfig || !columnElement.totalizerConfig[rowGuid] || columnElement.totalizerConfig[rowGuid] === 'none') {
			columnElement.totalizerValue[rowGuid] = '';
		}
		else {
			var cellMetadata = [];
			// if no blank column
			if (columnElement.field) {
				jQuery.each(rowsMetadata, function (i, val) {
					// look for the tag (column) in the metadata
					var foundtag = $.grep(val.tags, function (tag) {
						return columnElement.field.toLowerCase() === tag.ID.toLowerCase();
					});
					if (foundtag.length > 0) {
						cellMetadata.push(foundtag[0]);
					}
				});

				// once we have all cells for same column make sure they all are the same unit
				if (cellMetadata.length > 0) {
					var subTotal = undefined;
					var firstDataType = cellMetadata[0]["ValueTypeString"];
					var isSameDataType = cellMetadata.every(function (element) {
						return element.ValueTypeString && element.ValueTypeString === firstDataType;
					});
					if (isSameDataType && ( firstDataType === "System.Double" || firstDataType === "System.Int16" || firstDataType === "System.Int32" || firstDataType === "System.Int64")) {
						// if unit has been specified at the column level we need to do conversion
						var isSameUnit = false;
						var defaultUnit = -1;

						defaultUnit = cellMetadata[0]["Units"];
						isSameUnit = cellMetadata.every(function (element) {
							return element.Units === defaultUnit;
						});

						if (isSameUnit) {
							var valueCount = 0;
							$.each(cellMetadata, function (idx, cell) {
								if (!isNaN(cell.Value) && cell.Value !== null) {
									valueCount++;

									if (columnElement.totalizerConfig[rowGuid] === 'sum' || columnElement.totalizerConfig[rowGuid] === 'avg') {
										if (!subTotal) {
											subTotal = 0;
										}
										subTotal += cell.Value;
									}
									if (columnElement.totalizerConfig[rowGuid] === 'min') {
										if (subTotal === undefined) {
											subTotal = cell.Value;
										}
										subTotal = cell.Value < subTotal ? cell.Value : subTotal;
									}
									if (columnElement.totalizerConfig[rowGuid] === 'max') {
										if (subTotal === undefined) {
											subTotal = cell.Value;
										}
										subTotal = cell.Value > subTotal ? cell.Value : subTotal;
									}
								}
							});
							if (columnElement.totalizerConfig[rowGuid] === 'avg') {
								subTotal = subTotal / valueCount;
							}
							var numDecimals = 2;
							if ( firstDataType === "System.Int16" || firstDataType === "System.Int32" || firstDataType === "System.Int64" )
							{
								numDecimals = 0;
							}
							columnElement.totalizerValue[rowGuid] = subTotal + "||" + defaultUnit + "||" + numDecimals + "||" + "Numeric";
							rows[totalRowIdx][columnElement.field] = "0";
						}
						else if (columnElement.Unit && columnElement.Unit !== -1) {
							var valueCount = 0;
							$.each(cellMetadata, function (idx, cell) {
								if (!isNaN(cell.Value) && cell.Value !== null) {
									valueCount++;
									var cellValue = parseFloat(FMConvertEngUnits.Convert(cell.Value, cell.Units, columnElement.Unit));
									if (columnElement.totalizerConfig[rowGuid] === 'sum' || columnElement.totalizerConfig[rowGuid] === 'avg') {
										if (!subTotal) {
											subTotal = 0;
										}
										subTotal += cellValue;
									}
									if (columnElement.totalizerConfig[rowGuid] === 'min') {
										if (subTotal === undefined) {
											subTotal = cellValue;
										}
										subTotal = cellValue < subTotal ? cellValue : subTotal;
									}
									if (columnElement.totalizerConfig[rowGuid] === 'max') {
										if (subTotal === undefined) {
											subTotal = cellValue;
										}
										subTotal = cellValue > subTotal ? cellValue : subTotal;
									}
								}
							});
							if (columnElement.totalizerConfig[rowGuid] === 'avg') {
								subTotal = subTotal / valueCount;
							}
							defaultUnit = columnElement.Unit;
							columnElement.totalizerValue[rowGuid] = subTotal + "||" + defaultUnit + "||" + 2 + "||" + "Numeric";
							rows[totalRowIdx][columnElement.field] = "0";
						}
						else {
							columnElement.totalizerValue[rowGuid] = 'Incompatible Units';
							rows[totalRowIdx][columnElement.field] = '0';
						}
					}
					else if (isSameDataType && firstDataType === "System.DateTimeOffset")
					{

						$.each(cellMetadata, function (idx, cell) {
							if (cell.Value !== null && cell.Value !== "" ) {

								if (columnElement.totalizerConfig[rowGuid] === 'min')
								{
									if (!subTotal ) {
										subTotal = cell.Value;
										return;
									}
									var re = /-?\d+/;
									var rawValue = re.exec(cell.Value);
									var value = rawValue != null ? new Date(parseInt(rawValue[0])) : null;
									var rawSubTotal = re.exec(subTotal);
									var tempSubTotal = rawSubTotal != null ? new Date(parseInt(rawSubTotal[0])) : null;

									subTotal = value < tempSubTotal ? cell.Value : subTotal;
								}
								if (columnElement.totalizerConfig[rowGuid] === 'max') {
									if (!subTotal) {
										subTotal = cell.Value;
										return;
									}
									var re = /-?\d+/;
									var rawValue = re.exec(cell.Value);
									var value = rawValue != null ? new Date(parseInt(rawValue[0])) : null;
									var rawSubTotal = re.exec(subTotal);
									var tempSubTotal = rawSubTotal != null ? new Date(parseInt(rawSubTotal[0])) : null;

									subTotal = value > tempSubTotal ? cell.Value : subTotal;
								}
							}
						});
						columnElement.totalizerValue[rowGuid] = subTotal + "||" + 255 + "||" + 0 + "||" + "DateTimeOffset";
						rows[totalRowIdx][columnElement.field] = "0";
					}
					else if (isSameDataType && firstDataType === "System.TimeSpan")
					{

						$.each(cellMetadata, function (idx, cell) {
							if (cell.Value !== null && cell.Value !== "" ) {

								if (columnElement.totalizerConfig[rowGuid] === 'min')
								{
									if (subTotal === undefined) {
										subTotal = JSON.stringify(cell.Value);
										return;
									}
									var value = cell.Value != null ? cell.Value.Days + (((cell.Value.Hours * 60 * 60) + (cell.Value.Minutes * 60) + cell.Value.Seconds) / 86400) : null;
									var tempSubTotal = JSON.parse(subTotal);
									var expandTempSubTotal = tempSubTotal.Days + (((tempSubTotal.Hours * 60 * 60) + (tempSubTotal.Minutes * 60) + tempSubTotal.Seconds) / 86400);

									subTotal = value < expandTempSubTotal ? JSON.stringify(cell.Value): subTotal;
								}
								if (columnElement.totalizerConfig[rowGuid] === 'max') {
									if (subTotal === undefined) {
										subTotal = JSON.stringify(cell.Value);
										return;
									}
									var value = cell.Value != null ? cell.Value.Days + (((cell.Value.Hours * 60 * 60) + (cell.Value.Minutes * 60) + cell.Value.Seconds) / 86400) : null;
									var tempSubTotal = JSON.parse(subTotal);
									var expandTempSubTotal = tempSubTotal.Days + (((tempSubTotal.Hours * 60 * 60) + (tempSubTotal.Minutes * 60) + tempSubTotal.Seconds) / 86400);

									subTotal = value > expandTempSubTotal ? JSON.stringify(cell.Value) : subTotal;
								}
							}
						});
						columnElement.totalizerValue[rowGuid] = subTotal + "||" + 255 + "||" + 0 + "||" + "TimeSpan";
						rows[totalRowIdx][columnElement.field] = "0";
					}
					else {
						// the subtotal is going to be blank unless is the first column (Point)
						if (columnElement.field !== "point") {
							columnElement.totalizerValue[rowGuid] = '';
							rows[totalRowIdx][columnElement.field] = '';
						}
					}
				}
				else {
					// the subtotal is going to be blank unless is the first column (Point)
					if (columnElement.field !== "point") {
						columnElement.totalizerValue[rowGuid] = '';
						rows[totalRowIdx][columnElement.field] = '';
					}
				}
			}
		}
	});
}

// Get info for a tag
FMPointGroupGrid.getTagInfo = function (metadata, pointGuid, tagID) {
	var foundPoint = $.grep(metadata, function (point) {
		return pointGuid && point.pointguid && pointGuid.toUpperCase() === point.pointguid.toUpperCase();
	});
	if (foundPoint.length > 0) {
		var foundTag = $.grep(foundPoint[0].tags, function (tag) {
			return tagID.toUpperCase() === tag.ID.toUpperCase();
		});
		// if we found the metadata for the tag then we need to check the formatting
		if (foundTag.length > 0) {
			return foundTag[0];
		}
	}
	return null;
}

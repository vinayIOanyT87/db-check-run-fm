// Object to manage Metadata for the Operate Point Group grids
var FMPointHistoryGrid = function FMPointHistoryGrid( id, grid, pointId, pointGuid, fontSize)
{
	var self = this;
	this._metadata = [];
	this._grid = grid;
	this._id = id;
	this._pointGuid = pointGuid.toUpperCase();
	this.fontSize = fontSize;
	this._uniqueId = FMOperateIndex.newGuid();

	self._metadata.push({ point: pointId, pointguid: pointGuid, tags: [] });

	var pointGuids = [];
	pointGuids.push(pointGuid);

	self._refreshMetadataForAllPoints(pointGuids);
}

// Instance methods
FMPointHistoryGrid.prototype = {
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
			url: 'GetAllPointsMetadataForPointHistory',
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
		var PointGuids = [];
		PointGuids.push(this._pointGuid);

		// create copies of the context so we can use it in other functions
		var _uniqueId2 = this._uniqueId;
		var _metadata2 = this._metadata;
		var _grid2 = this._grid;
		var id = this._id;

		if ( PointGuids.length > 0 )
		{
			// call the server to retrieve the metadata
			$.ajax( {
				url: 'GetTagMetadataForPointHistoryList',
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
										foundTag[0].ValueTypeString = tagValue.ValueTypeString;
									}
									else
									{
										// add tag to the point
										foundPoint[0].tags.push( tagValue );
									}
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
	staticPointHistoryFormatter: function( row, cell, value, columnDef, dataContext )
	{
		if ( value === undefined || value === '' || value === null )
		{
			var pointGuid = this._pointGuid;
			var foundPoint = $.grep(this._metadata, function (point) {
				return point.pointguid && pointGuid.toUpperCase() === point.pointguid.toUpperCase();
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
						// defensive programming
						returnData = cellData + "Unknown" + "</div>";

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
		return this.staticPointFormatter( row, cell, value, columnDef, dataContext );
	},

	// format point tags for display in the grid
	staticPointFormatter: function( row, cell, value, columnDef, dataContext )
	{
		var newFormattedValue = value;
		var pointGuid = this._pointGuid;
		var foundPoint = $.grep(this._metadata, function (point) {
			return point.pointguid && pointGuid.toUpperCase() == point.pointguid.toUpperCase();
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
					value = dataContext[foundTag[0].ID];

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
					value = dataContext[foundTag[0].ID];

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

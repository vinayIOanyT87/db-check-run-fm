// Object to manage Metadata for the Operate Movement grids
var FMMovementSummaryGrid = function FMMovementSummaryGrid(activeTab, id, grid, description, visibility, fontSize, owner, isOwnByMe, isEditable) {
	var self = this;
	this._metadata = [];
	this._grid = grid;
	this._id = id;
	this._mutex = new AsyncMutex();
	this._activeTab = activeTab;
	this._rowsDeleted = false;
	this.description = description;
	this.visibility = visibility;
	this.fontSize = fontSize;
	this.owner = owner;
	this.isOwnByMe = isOwnByMe;
	this.isEditable = isEditable;
	this._uniqueId = FMOperateIndex.newGuid();
	this._updateDynamicAjaxRequest = null;
	this._updateDynamicTimer = null;
	this.movementSummaryGuid = null;
	this.movementSummaryGuidStr = '00000000-0000-0000-0000-000000000000';
	this.movementSummaryId = '';
	this.rowVersionStr = '00000000';
	this.columnDefinitions = null;
	this.rowDefinitions = null;
	this.refreshTimer = null;
	this.getSummaryIfNewerAjaxRequest = null;
	this.summaryColumnFormatter = this.movementSummaryFormatter.bind(this);

	var gridData = this._grid.getData().getItems();

	var movementguid;
	jQuery.each(gridData, function (idx, elem) {
		if (elem.rowType === 'movement' && elem.hasOwnProperty("pointguid") && elem.pointguid && FMOperateIndex.isGuid(elem.pointguid)) {
			self._metadata.push({ movementguid: elem.movementguid, point: elem.point, pointguid: elem.pointguid, tags: [] });
			self._refreshMetadataForPoint(movementguid, elem.pointguid);
		}
	});

}

// Instance methods
FMMovementSummaryGrid.prototype = {

	// add a row ( Point ) to the grid	
	addRow: function (rowDefinition) {
		var existingPoint = jQuery.grep(this._metadata, function (elem, idx) {
			return (elem.pointguid && rowDefinition.pointguid && elem.pointguid.toUpperCase() === rowDefinition.pointguid.toUpperCase());
		});

		if (existingPoint.length > 0) {
			existingPoint[0].point = rowDefinition.point;
		}
		else {
			this._metadata.push({ movementguid: rowDefinition.movementguid, point: rowDefinition.point, pointguid: rowDefinition.pointguid, tags: [] });
			if (rowDefinition.pointguid) {
				this._refreshMetadataForPoint(rowDefinition.movementguid, rowDefinition.pointguid);
			}
		}
	},

	deleteRow: function (rowDefinition) {
		// if the row has a point then we need to update the metadata
		if (rowDefinition && rowDefinition.pointguid) {
			var pointGuid = rowDefinition.pointguid;
			var pointCount = 0;
			$.each(this._grid.getData().getItems(), function (key, value) {
				if (value.pointguid === pointGuid) {
					pointCount++;
				}
			});

			// if there is only one row for that point guid and we are trying to delete it we can remove it from the metadata
			if (pointCount === 1) {
				this._metadata = $.grep(this._metadata, function (e) {
					return e.pointguid !== pointGuid;
				});

				var mutex = this._mutex;
				var uniqueId = this._uniqueId;
				var metadata = this._metadata;
				var grid = this._grid;
				var id = this._id;
				var activeTab = this._activeTab;

				FMOperateIndex.movementSummaryControllers[id]._subscribeUpdateTags(mutex, uniqueId, metadata, grid, id, activeTab);
			}
		}
	},

	// add a column ( tag ) to the grid	
	addColumn: function (columnDefinition) {
		if (columnDefinition && columnDefinition.field) {
			this._refreshMetadataForNewTag(columnDefinition.field);
		}
	},

	// delete a column from the metadata	
	deleteColumn: function (field) {
		this._metadata.forEach(function (point) {
			var pos = point.tags.map(function (e) {
				return e.ID;
			}).indexOf(field);
			point.tags.splice(pos, 1);
		});

		// create copies of the context so we can use it in other functions
		var mutex = this._mutex;
		var uniqueId = this._uniqueId;
		var metadata = this._metadata;
		var grid = this._grid;
		var id = this._id;
		var activeTab = this._activeTab;

		FMOperateIndex.movementSummaryControllers[id]._subscribeUpdateTags(mutex, uniqueId, metadata, grid, id, activeTab);
	},



	// return the metadata object
	getMetadata: function () {
		return this._metadata;
	},

	// set the metadata object
	setMetadata: function (metadata) {
		this._metadata = metadata;
	},

	getMutex: function() {
		return this._mutex;
	},

	getGrid: function () {
		return this._grid;
	},

	getActiveTab: function () {
		return this._activeTab;
	},

	getRowsDeleted: function () {
		return this._rowsDeleted;
	},

	clearRowsDeleted: function () {
		this._rowsDeleted = false;
	},

	// return the metadata object
	geUniqueId: function () {
		return this._uniqueId;
	},

	// update metadata when a row is added
	_refreshMetadataForPoint: function (movementGuid, newPointGuid) {
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

		// Additional Tags that are not displayed but must be monitored
		tagNames.push('IndividualNodeControl');

		// create copies of the context so we can use it in other functions
		var mutex = this._mutex;
		var uniqueId = this._uniqueId;
		var metadata = this._metadata;
		var grid = this._grid;
		var id = this._id;
		var activeTab = this._activeTab;
		var self = this;

		// call the server to retrieve the metadata
		$.ajax({
			url: 'GetPointMetadataForMovementSummary',
			type: 'Post',
			dataType: 'json',
			cache: false,
			data: JSON.stringify({
				"movementguid": movementGuid,
				"pointguid": newPointGuid,
				"tags": tagNames
			}),
			success: function (response) {
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
					// if it was not in error load and update the drawing
					if (!inError) {
						var existingPoint = jQuery.grep(metadata, function (elem, idx) {
							return ((elem.pointguid && elem.pointguid.toUpperCase()) === (newPointGuid && newPointGuid.toUpperCase()));
						});

						if (existingPoint.length > 0) {
							if (data.length === 0) {

								var dataView = grid.getData();
								var rows = dataView.getItems();

								for (var rowNumber = 0; rowNumber < rows.length; rowNumber++) {
									if (rows[rowNumber].movementguid !== existingPoint[0].movementguid) {
										continue;
									}

									// Removal of the Row will update the _metadata but not the copy passed
									// It might be more efficient to upeate metadata, but there will be no matching rows
									// so it shouldn't matter
									FMMovementSummaryTab.RemoveMovementRow(id, grid, rowNumber);
									self._rowsDeleted = true;
									break;
								}

							}
							else
							{
								existingPoint[0].tags = data;
								if (FMOperateIndex.movementSummaryControllers[id]) {
									FMOperateIndex.movementSummaryControllers[id]._subscribeUpdateTags(mutex, uniqueId, metadata, grid, id, activeTab);
								}
							}
						}

					}
				});
			},
			error: function (request, status, error) {
				FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
				});
			}
		});
	},

	// update metadata when a column is added
	_refreshMetadataForNewTag: function (tagId) {

		// get the list of unique rows (points) we need the tag guids for
		var gridData = this._grid.getData().getItems();

		var PointGuids = [];

		jQuery.each(gridData, function (idx, elem) {
			if (elem.rowType == 'movement' && elem.hasOwnProperty("pointguid") && elem.pointguid && FMOperateIndex.isGuid(elem.pointguid)) {
				if (jQuery.inArray(elem.pointguid, PointGuids) === -1) {
					PointGuids.push(elem.pointguid);
				}
			}
		});

		// create copies of the context so we can use it in other functions
		var mutex = this._mutex;
		var uniqueId = this._uniqueId;
		var metadata = this._metadata;
		var grid = this._grid;
		var id = this._id;
		var activeTab = this._activeTab;

		if (PointGuids.length > 0) {
			// call the server to retrieve the metadata
			$.ajax({
				url: 'GetTagMetadataForMovementSummaryPointList',
				type: 'Post',
				dataType: 'json',
				cache: false,
				data: JSON.stringify({
					"pointGuids": PointGuids,
					"tagName": tagId
				}),
				success: function (response) {
					FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
						if (!inError) {
							// the new tags are in the same order as the points, add them to the point tags in order
							for (var index = 0; index < data.length; index++) {
								if (index < metadata.length) {
									metadata[index].tags.push(data[index]);
								}
							}

							FMOperateIndex.movementSummaryControllers[id]._subscribeUpdateTags(mutex, uniqueId, metadata, grid, id, activeTab);
						}
					});
				},
				error: function (request, status, error) {
					FMErrorAndExceptionHandling.ShowException(request, status, error);
				}
			});
		}
	},

	_subscribeUpdateTags: function (mutex, uniqueId, metadata, grid, id, activeTab) {
		var pointValueList = [];
		metadata.forEach(function (point) {
			point.tags.forEach(function (pointValue) {

				// subscribe only to Settings
				if (pointValue.PointValueIdentifier_PointValueType !== 1) {
					return;
				}
				var newPointValue = {
					IdentityGuid: pointValue.PointValueIdentifier_IdentityGuid,
					PointValueType: pointValue.PointValueIdentifier_PointValueType,
					PropertyID: pointValue.PointValueIdentifier_PropertyID
				};

				pointValueList.push(newPointValue);
			});
		});

		FMOperateIndex.subscribeTagWebWorker(this._uniqueId, pointValueList, function (data) {
			FMMovementSummaryGrid.updateGridAsync(data, mutex, uniqueId, metadata, grid, id, activeTab);
		}, false);
	},

	getNumericUnitTypesForTag: function (tagName) {
		var unitType = [];

		if (this._metadata === null) {
			return unitType;
		}

		this._metadata.forEach(function (point) {
			point.tags.forEach(function (tag)			{
				if (tag.ID === tagName
					&& tag.hasOwnProperty('Value')
					&& tag.Value !== null) {
					tag.Value.forEach(function (Value) {
						if (Value !== null
						&& Value.EngineeringUnitsType !== 0) {
							if (unitType.indexOf(Value.EngineeringUnitsType) < 0) {
								unitType.push(Value.EngineeringUnitsType);
							}
						}
					});
				}
			});
		});
		return unitType;
	},

	getValueTypesForTag: function (tagName) {
		var valueType = [];

		if (this._metadata === null) {
			return valueType;
		}


		this._metadata.forEach(function (point) {
			point.tags.forEach(function (tag) {
				if (tag.ID === tagName
					&& tag.hasOwnProperty('Value')
					&& tag.Value !== null) {
					tag.Value.forEach(function (Value) {
						if (Value !== null
						&& Value.ValueTypeString !== null) {
							if (valueType.indexOf(Value.ValueTypeString) < 0) {
								valueType.push(Value.ValueTypeString);
							}
						}
					});
				}
			});
		});
		return valueType;
	},

	getWellKnownGuidsForTag: function (tagName) {
		var WellKnownIdentityGuid = [];

		if (this._metadata === null) {
			return WellKnownIdentityGuid;
		}

		this._metadata.forEach(function (point) {
			point.tags.forEach(function (tag) {
				if (tag.ID === tagName
					&& tag.hasOwnProperty('Value')
					&& tag.Value !== null) {
					tag.Value.forEach(function (Value) {
						if (Value !== null
						&& typeof Value.WellKnownIdentityGuid != 'undefined') {
							if (WellKnownIdentityGuid.indexOf(Value.WellKnownIdentityGuid.toUpperCase()) < 0) {
								WellKnownIdentityGuid.push(Value.WellKnownIdentityGuid.toUpperCase());
							}
						}
					});
				}
			});
		});
		return WellKnownIdentityGuid;
	},

	movementSummaryNameFormatter : function (row, cell, value, columnDef, dataContext) {
		var foundPoint = $.grep(this._metadata, function (point) {
			return dataContext.movementguid && point.pointguid && dataContext.movementguid.toUpperCase() === point.pointguid.toUpperCase();
		});
		if (foundPoint.length > 0) {
			var foundTagList = $.grep(foundPoint[0].tags, function (tag) {
				if (columnDef.field === undefined || columnDef.field === null) {
					return false;
				}
				return columnDef.field.toUpperCase() === tag.ID.toUpperCase();
			});

			// if we found the metadata for the tag then we need to check the formatting
			if (foundTagList.length > 0 && foundTagList[0].Value !== undefined) {
				var foundTag = $.grep(foundTagList[0].Value, function (value) {
					if (value == null) {
						return false;
					}
					return dataContext.pointguid && value.PointGuid && dataContext.pointguid.toUpperCase() === value.PointGuid.toUpperCase();
				});

				if (foundTag.length > 0
					&& foundTag[0].Value !== null) {
					value = foundTag[0].Value; // set the value as the one stored in the metadata

					value = value.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
					var spacer = "<span style='display:inline-block;height:1px;width:" + (15 * dataContext["indent"]) + "px'></span>";

					if (dataContext.rowType === "movement") {
						if (dataContext._collapsed) {
							spacer += " <span class='cell-toggle expand'></span>&nbsp;" + value;
							return spacer;
						}
						else {
							spacer += " <span class='cell-toggle collapse'></span>&nbsp;" + value;
							return spacer;
						}
					}
					else {
						spacer += " <span class='cell-toggle'></span>&nbsp;" + value;
						return spacer;
					}
				}
			}
		}
	},

	// generic formatter for movement summary
	movementSummaryFormatter: function (row, cell, value, columnDef, dataContext) {
		if (columnDef.field === "CreatedBy" && dataContext.rowType !== "movement") {
			return "";
		}

		return this.pointFormatter(row, cell, value, columnDef, dataContext);
	},

	// format point tags for display in the grid
	pointFormatter: function (row, cell, value, columnDef, dataContext) {
		var newFormattedValue = '';
		var foundPoint = $.grep(this._metadata, function (point) {
			return dataContext.movementguid && point.pointguid && dataContext.movementguid.toUpperCase() === point.pointguid.toUpperCase();
		});

		if (foundPoint.length > 0 && foundPoint[0].tags !== null) {
			var foundTagList = $.grep(foundPoint[0].tags, function (tag) {
				if (columnDef.field === undefined || columnDef.field === null) {
					return false;
				}
				return columnDef.field.toUpperCase() === tag.ID.toUpperCase();
			});

			// if we found the metadata for the tag then we need to check the formatting
			if (foundTagList.length > 0 && foundTagList[0].Value) {
				var foundTag = $.grep(foundTagList[0].Value, function (value) {
					if (value == null) {
						return false;
					}
					return dataContext.pointguid && value.PointGuid && dataContext.pointguid.toUpperCase() === value.PointGuid.toUpperCase();
				});

				if (foundTag.length > 0) {

					if (foundTagList[0].CommunicationsFailure) {
						return "<div style='padding-left: 3px; padding-right: 3px;' title=''>" + $('#CommunicationsFailureText').val() + "</div>";
					}

					else if (foundTagList[0].Access && !(foundTagList[0].Access.View || foundTagList[0].Access.Modify)) {
						return "<div style='padding-left: 3px; padding-right: 3px;' title=''>" + $('#RestrictedText').val() + "</div>";
					}

					else if (foundTag[0].Status && (foundTag[0].Status & 0x80000000)) {
						return "<div style='padding-left: 3px; padding-right: 3px;' title=''>" + FMOperateIndex.GetStatusCode(foundTag[0].Status) + "</div>";
					}

					else if (foundTag[0].Value === undefined) {
						return "<div style='padding-left: 3px; padding-right: 3px;' title=''>Undefined</div>";
					}

					var isCommentColumn = columnDef.field === "Comment";
					var cellTitle = "";
					var commentCellValueStyle = "overflow: hidden; text-align: left;";
					if (isCommentColumn) {
						cellTitle = FMMovementSummaryGrid.encodeHtmlAttribute(foundTag[0].Value);
					}

					if (foundTag[0].ValueTypeString === "System.Double") {
						var unit = parseInt(foundTag[0].Units);
						var precision = parseInt(foundTag[0].DecimalPlaces);
						value = foundTag[0].Value; // set the value as the one stored in the metadata

						// override the precision with the one from the column definition
						if (columnDef.hasOwnProperty('DecimalPlaces')) {
							if (columnDef.DecimalPlaces > -1) {
								precision = columnDef.DecimalPlaces;
							}
						}
						// override the unit with the one from the column definition
						if (columnDef.hasOwnProperty('Unit')) {
							if (columnDef.Unit > -1 && columnDef.Unit !== unit) {
								// if the units from the point and column are different we need to do a conversion.
								value = FMConvertEngUnits.Convert(value, unit, columnDef.Unit);
								unit = columnDef.Unit;
							}
						}

						FMOperateIndex.numformatInfo.NumberDecimalDigits = precision;
						if (value != null) {
							newFormattedValue = FMFormatValues.FormatValue(unit, FMOperateIndex.numformatInfo, value);
						}


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
					else if (foundTag[0].ValueTypeString === "System.Single") {
						var unit = parseInt(foundTag[0].Units);
						var precision = parseInt(foundTag[0].DecimalPlaces);
						value = foundTag[0].Value; // set the value as the one stored in the metadata

						// override the precision with the one from the column definition
						if (columnDef.hasOwnProperty('DecimalPlaces')) {
							if (columnDef.DecimalPlaces > -1) {
								precision = columnDef.DecimalPlaces;
							}
						}
						// override the unit with the one from the column definition
						if (columnDef.hasOwnProperty('Unit')) {
							if (columnDef.Unit > -1 && columnDef.Unit !== unit) {
								// if the units from the point and column are different we need to do a conversion.
								value = FMConvertEngUnits.Convert(value, unit, columnDef.Unit);
								unit = columnDef.Unit;
							}
						}

						FMOperateIndex.numformatInfo.NumberDecimalDigits = precision;
						if (value != null) {
							newFormattedValue = FMFormatValues.FormatValue(unit, FMOperateIndex.numformatInfo, value);
						}

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
						value = foundTag[0].Value; // set the value as the one stored in the metadata
						// set the value as the one stored in the metadata
						if (value != null || value != "") {
							var re = /-?\d+/;
							var m = re.exec(value);
							newFormattedValue = new Date(parseInt(m[0]));
							newFormattedValue = moment(newFormattedValue).format(FMOperateIndex.numformatInfo.ShortDatePattern.toUpperCase());
						}

						if (columnDef.showquality) {
							newFormattedValue += '<div class="supsub"><sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup></div>';
						}
					}
					else if (foundTag[0].ValueTypeString === "System.DateTimeOffset") {
						newFormattedValue = "";
						value = foundTag[0].Value; // set the value as the one stored in the metadata
						// set the value as the one stored in the metadata
						if (value != null && value != "") {
							var re = /-?\d+/;
							 var m = re.exec(value);
							 var timezoneOffsetStr = $("#TimezoneOffsetString").val();
							 var timezoneOffset = parseInt(timezoneOffsetStr);
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

					else if (foundTag[0].ValueTypeString === "System.TimeSpan") {
						newFormattedValue = "";
						value = foundTag[0].Value; // set the value as the one stored in the metadata
						// set the value as the one stored in the metadata
						if (value != null && value != "") {
							newFormattedValue = (foundTag[0].Value.Days > 0 ? foundTag[0].Value.Days + "." : "") + ("000000" + foundTag[0].Value.Hours).slice(-2) + ":" + ("000000" + foundTag[0].Value.Minutes).slice(-2) + ":" + ("000000" + foundTag[0].Value.Seconds).slice(-2);
						}

						if (columnDef.showquality) {
							newFormattedValue += '<div class="supsub"><sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup></div>';
						}
					}

					else if (foundTag[0].ValueTypeString === "FMBusinessObjects.DataObjects.PointCommandStatusListReference") {
						newFormattedValue = "";
						if (foundTag[0].Value != null && foundTag[0].Value.CurrentKey != null) {
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

					else {
						newFormattedValue = foundTag[0].Value;
						if (columnDef.showquality) {
							newFormattedValue += '<div class="supsub"><sup class="superscript">' + foundTag[0].QualityAbbreviation + '</sup></div>';
						}
					}

					if (isCommentColumn) {
						newFormattedValue = "<div style='" + commentCellValueStyle + "'>" + newFormattedValue + "</div>";
					}

					if ((foundTag[0].hasOwnProperty("AlarmPriorityGuid") && foundTag[0].AlarmPriorityGuid !== "")
						&& (foundTag[0].Access && (foundTag[0].Access.View || foundTag[0].Access.modify))) {
						// add the alarm style
						var alarmcellStyle = "<div style='padding-left: 3px; padding-right: 3px;' class='AlarmPriority-" + (foundTag[0].Acknowledged ? "" : "blink-") + foundTag[0].AlarmPriorityGuid;

						// add the tank status style
						alarmcellStyle += FMMovementSummaryGrid.setTankStatusStyle(false, foundPoint[0].tags);
						alarmcellStyle += "' title='" + (isCommentColumn ? cellTitle : (!!foundTag[0].AlarmState ? foundTag[0].AlarmState : "")) + "'>";
						newFormattedValue = alarmcellStyle + newFormattedValue + "</div>";
					}
					else {
						var celldata = "<div style='padding-left: 3px; padding-right: 3px;' ";
						celldata += FMMovementSummaryGrid.setTankStatusStyle(true, foundPoint[0].tags);
						celldata += " title='" + cellTitle + "'>";
						newFormattedValue = celldata + newFormattedValue + "</div>";
					}

				}
				else {
					var celldata = "<div style='padding-left: 3px; padding-right: 3px;' ";
					celldata += FMMovementSummaryGrid.setTankStatusStyle(true, foundPoint[0].tags);
					celldata += " title=''>";
					newFormattedValue = celldata + newFormattedValue + "</div>";
				}
			}
		}
		else {
		}

		return newFormattedValue;
	}

};

FMMovementSummaryGrid.encodeHtmlAttribute = function (value) {
	if (value === undefined || value === null) {
		return "";
	}

	return String(value)
		.replace(/&/g, "&amp;")
		.replace(/"/g, "&quot;")
		.replace(/'/g, "&#39;")
		.replace(/</g, "&lt;")
		.replace(/>/g, "&gt;");
};

FMMovementSummaryGrid.setTankStatusStyle = function (addClass, tags) {
	var returnStyle = '';
	var currentTankStatus = "Not Found";

	if (tags.length > 0) {
		//bds
		for (var iloop = 0; iloop < tags.length; iloop++) {
			if (tags[iloop].WellKnownIdentityGuid.toUpperCase() === '834B9D8A-C17A-48F6-97FC-1B18EB562866') {
				currentTankStatus = tags[iloop].Value;;
				break;
			}
		}
	}

	if (currentTankStatus !== "Not Found") {
		if (addClass == true) {
			returnStyle = "Class='";
		}
		else {
			returnStyle = " ";
		}
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
		if (addClass == true) {
			returnStyle += "'";
		}

	}
	return returnStyle;
}

// populate the grid with the results from the server
FMMovementSummaryGrid.updateGridAsync = async function (data, mutex, uniqueId, metadata, grid, id, activeTab) {
	await mutex.acquire();
	try {
		if (data.length > 0) {
			var dataView = grid.getData();
			dataView.beginUpdate();


			var summaryChanges = false

			for (var dataIndex = 0; dataIndex < data.length; dataIndex++) {

				// Guid Empty indicates the Point associated with the PointValueIdentifier has been deleted,
				// it will match tags[0].PointValueIdentifier_IdentityGuid which is the Point Property for the associated
				// movement
				if (data[dataIndex].PointGuid === '00000000-0000-0000-0000-000000000000') {
					for (var metadataIndex = 0; metadataIndex < metadata.length; metadataIndex++) {
						if (data[dataIndex].PointValueIdentifier_IdentityGuid !== metadata[metadataIndex].tags[0].PointValueIdentifier_IdentityGuid) {
							continue;
						}

						var rows = dataView.getItems();

						for (var rowNumber = 0; rowNumber < rows.length; rowNumber++) {
							if (rows[rowNumber].movementguid !== metadata[metadataIndex].movementguid) {
								continue;
							}

							// Removal of the Row will update the _metadata but not the copy passed
							// It might be more efficient to upeate metadata, but there will be no matching rows
							// so it shouldn't matter
							FMMovementSummaryTab.RemoveMovementRow(id, grid, rowNumber);
							summaryChanges = true;
							break;
						}

						break;
					}
				}

				else {
					for (var metadataIndex = 0; metadataIndex < metadata.length; metadataIndex++) {
						if (data[dataIndex].PointGuid !== metadata[metadataIndex].pointguid) {
							continue;
						}

						for (var tagIndex = 0; tagIndex < metadata[metadataIndex].tags.length; tagIndex++) {
							if (metadata[metadataIndex].tags[tagIndex].PointValueIdentifier_PropertyID !== data[dataIndex].PointValueIdentifier_PropertyID) {
								continue;
							}

							metadata[metadataIndex].tags[tagIndex].Value = data[dataIndex].Value;

							// Update the row data
							if (metadata[metadataIndex].tags[tagIndex].PointValueIdentifier_PropertyID === 'PointId'
								|| metadata[metadataIndex].tags[tagIndex].PointValueIdentifier_PropertyID === 'TransferDirection'
								|| metadata[metadataIndex].tags[tagIndex].PointValueIdentifier_PropertyID === 'IndividualNodeControl') {

								// Find the Parent Row in the Grid
								var rows = dataView.getItems();

								for (var rowNumber = 0; rowNumber < rows.length; rowNumber++) {
									if (rows[rowNumber].movementguid !== metadata[metadataIndex].movementguid) {
										continue;
									}

									var parentRow = rows[rowNumber];
									var movementRowCount = 0;

									// Update/Add rows for the movement
									for (valueIndex = 0; valueIndex < metadata[metadataIndex].tags[tagIndex].Value.length; valueIndex++) {
										// Last Row is the empty
										if (rowNumber + valueIndex >= rows.length - 1
										|| rows[rowNumber + valueIndex].movementguid !== metadata[metadataIndex].movementguid) {
											if (metadata[metadataIndex].tags[tagIndex].PointValueIdentifier_PropertyID === 'PointId') {

												var nodePointGuid = metadata[metadataIndex].tags[tagIndex].Value[valueIndex].PointGuid;
												var nodePointId = metadata[metadataIndex].tags[tagIndex].Value[valueIndex].Value;

												FMMovementSummaryTab.InsertMovementNodeRow(
													grid,
													rowNumber + movementRowCount,
													parentRow.id,
													parentRow.id,
													parentRow.movementguid,
													nodePointGuid,
													nodePointId,
													'Source',
													false);

												rows = dataView.getItems();
												summaryChanges = true;
												movementRowCount++;
											}
										}
										else {
											if (metadata[metadataIndex].tags[tagIndex].PointValueIdentifier_PropertyID === 'PointId') {
												if ((metadata[metadataIndex].tags[tagIndex].Value[valueIndex] !== null)
												&& (rows[rowNumber + valueIndex].pointguid !== metadata[metadataIndex].tags[tagIndex].Value[valueIndex].PointGuid)) {
													rows[rowNumber + valueIndex].pointguid = metadata[metadataIndex].tags[tagIndex].Value[valueIndex].PointGuid;
												}
												if ((metadata[metadataIndex].tags[tagIndex].Value[valueIndex] !== null)
												&& (rows[rowNumber + valueIndex].point !== metadata[metadataIndex].tags[tagIndex].Value[valueIndex].Value)) {
													rows[rowNumber + valueIndex].point = metadata[metadataIndex].tags[tagIndex].Value[valueIndex].Value;
												}
												movementRowCount++;
											}
											else if (metadata[metadataIndex].tags[tagIndex].PointValueIdentifier_PropertyID === 'TransferDirection') {
												if ((metadata[metadataIndex].tags[tagIndex].Value[valueIndex] !== null)
												&& (rows[rowNumber + valueIndex].direction !== metadata[metadataIndex].tags[tagIndex].Value[valueIndex].Value)) {
													rows[rowNumber + valueIndex].direction = metadata[metadataIndex].tags[tagIndex].Value[valueIndex].Value;
												}
											}
											else if (metadata[metadataIndex].tags[tagIndex].PointValueIdentifier_PropertyID === 'IndividualNodeControl') {
												if ((metadata[metadataIndex].tags[tagIndex].Value[valueIndex] !== null)
												&& (rows[rowNumber + valueIndex].individualNodeControl !== (metadata[metadataIndex].tags[tagIndex].Value[valueIndex].Value === 'False' ? false : true))) {
													rows[rowNumber + valueIndex].individualNodeControl = metadata[metadataIndex].tags[tagIndex].Value[valueIndex].Value === 'False' ? false : true;
												}
											}
										}
									}

									// Delete Rows
									if (metadata[metadataIndex].tags[tagIndex].PointValueIdentifier_PropertyID === 'PointId') {
										grid.suspend;
										var movementRows = $.grep(rows, function (row) {
											return (row.movementguid === metadata[metadataIndex].movementguid);
										});

										if (movementRows) {
											var movementRowCount = movementRows.length;

											while (movementRowCount > metadata[metadataIndex].tags[tagIndex].Value.length) {
												FMMovementSummaryTab.DeleteAssociatedNode(grid, movementRows[movementRowCount - 1]);
												movementRowCount--;
												summaryChanges = true;
											}
										}
									}

									break;
								}
							}


							break;
						}

						break;
					}
				}
			}

			if (summaryChanges) {

				// refresh the indented groups
				var rows = grid.getData().getItems();
				var itemMap = FMMovementSummaryTab.groupBy(rows, 'parentRowId');
				var data = FMMovementSummaryTab.initTree(rows, itemMap);

				FMOperateIndex.PersistMovementSummary(activeTab, id, grid);
			}

			dataView.endUpdate();

			grid.invalidateAllRows();
			grid.render();
		}
	} finally {
		mutex.release(); // Ensure the lock is released
		return Promise.resolve();
	}
}




// Get info for a tag
FMMovementSummaryGrid.getTagInfo = function (metadata, pointGuid, tagID) {
	var foundPoint = $.grep(metadata, function (point) {
		return pointGuid && point.pointguid && pointGuid.toUpperCase() === point.pointguid.toUpperCase();
	});
	if (foundPoint.length > 0) {
		var foundTag = $.grep(foundPoint[0].tags, function (tag) {
			return tagID.toUpperCase() === tag.ID.toUpperCase();
		});
		// if we found the metadata for the tag then we need to check the formatting
		if (foundTag.length > 0
			&& foundTag[0].hasOwnProperty('Value')
			&& foundTag[0].Value !== null
			&& foundTag[0].Value.length > 0) {
			return foundTag[0].Value[0];
		}
	}
	return null;
}

/* point group module */
FMOperateIndex.stack_bottomright_operatorScheduleModal = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#PointGroupReportSchedule') };
FMOperateIndex.pointGroupSaveOnColumnResize = true;
if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}
FMOperateIndex.Statistics = new Array(),

	// function to generate a unique guid
	FMOperateIndex.newGuid = function () {
		function s4() {
			return Math.floor((1 + Math.random()) * 0x10000)
				.toString(16)
				.substring(1);
		}
		return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
	}

// returns true if the value passed is a Guid
FMOperateIndex.isGuid = function (stringToTest) {
	if (stringToTest[0] === "{") {
		stringToTest = stringToTest.substring(1, stringToTest.length - 1);
	}
	var regexGuid = /^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$/gi;
	return regexGuid.test(stringToTest);
}

// initialize the event handler for the Select button in the modal
FMOperateIndex.PointGroupSelectionModalSelectButton = function () {
	// this function will be overriden depending on what is going to be selected, Points or Tags
};

FMOperateIndex.ClonePointGroup = function (ID, Description, pointGroupType, grid, pointGroupGuid) {

	if (!pointGroupGuid) {
		pointGroupGuid = '';
	}

	// we can reuse the same call to save the Drawing settings
	var rows = grid.getData().getItems();  // get the items in the dataview used by the grid
	// there is no need to save the header menu since it's being rebuilt
	var columns = grid.getColumns();
	var columnsWithNoMenu = $.extend(true, [], columns);  // copy array by value so we don't lose the original menu
	// we don't need to save the header menu since it's being rebuilt on load or the total/subtotals
	columnsWithNoMenu = $.map(columnsWithNoMenu, function (val, i) {
		val.header = null;
		val.totalizerValue = null;
		val.name = encodeURIComponent(val.name); // We have to encode strings so we can support escape characters ( '\', '|' )
		return val;
	});
	var fontSize = grid.getOptions().fontSize;

	var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };
	var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator };

	// remove previous notifications
	PNotify.removeStack(stack_bottomright_operator);
	$('<div id="loaderpointgroupmain" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('body');

	$.ajax({
		url: 'SavePointGroup',
		type: 'Post',
		dataType: 'json',
		contentType: "application/json",
		data: '{pointGroupGuid:\'' + pointGroupGuid + '\', id: ' + JSON.stringify(ID) + ', description: ' + JSON.stringify(Description) + ', rows: \'' + JSON.stringify(rows) + '\', columns: \'' + JSON.stringify(columnsWithNoMenu) + '\', fontSize: \'' + fontSize + '\', pointGroupType: \'' + parseInt(pointGroupType) + '\' }',
		success: function (response) {
			$("#loaderpointgroupmain").remove();
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					// we get back the pointGroupGuid during the save, we need to check is a new pointGroupId and not a duplicate
					var newPointGroupGuid = data.pointGroupGuid;
					// if we found an existing pointgroup with the same ID, visibility, site and owner
					if (data.duplicateFound) {
						// check if we want to overwrite the existing pointgroup
						FMLayout.ConfirmYesNo("There is already a Point Group with the same Name and Visibility.  Do you want to overwrite the existing Point Group?",
							"Duplicate Point Group",
							function () {
								FMOperateIndex.ClonePointGroup(ID, Description, pointGroupType, grid, newPointGroupGuid);
							});
					} else {
						$("#pointGroupSaveScreen").modal("hide");
					}
					// refresh the hamburger the menu (list of point groups)
					if ($(".operateMenuItem.active a").attr("id") === "menuPointGroups") {
						FMOperateIndex.refreshHamburgerMenu = true;
					}
				}
			}, messageAttributes);
		},
		error: function (request, status, error) {
			$("#loaderpointgroupmain").remove();
			FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
			}, messageAttributes);
		}
	});
}

FMOperateIndex.PersistPointGroup = function (parentId, controlId, grid) {

	if (!FMOperateIndex.staticPointGroupControllers[controlId].isEditable) {
		return;
	}

	// we can reuse the same call to save the Drawing settings
	var dataview = grid.getData();
	var rows = dataview.getItems();
	var rowsFiltered = $.extend(true, [], rows);  // copy array by value so we don't lose the original menu

	// there is no need to save the header menu since it's being rebuilt
	var columns = grid.getColumns();
	var columnsWithNoMenu = $.extend(true, [], columns);  // copy array by value so we don't lose the original menu
	// we don't need to save the header menu since it's being rebuilt on load or the total/subtotals
	columnsWithNoMenu = $.map(columnsWithNoMenu, function (val, i) {
		val.header = null;
		val.totalizerValue = null;
		val.name = encodeURIComponent(val.name); // We have to encode strings so we can support escape characters ( '\', '|' )
		return val;
	});

	// if point group is dynamic we don't need to store the rows since they are generated.
	if (columnsWithNoMenu && columnsWithNoMenu[0] && columnsWithNoMenu[0].hasOwnProperty('filter')) {
		rowsFiltered = $.map(rows, function (row, idx) {
			if (row.type === "total") {
				return row;
			}
		});
	}

	var fontSize = grid.getOptions().fontSize;

	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#pointgroup' + controlId) };
	var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab };

	// remove previous notifications
	PNotify.removeStack(stack_bottomright_operatortab);

	// Get the ID and description to save them
	var ID = "";
	var description = FMOperateIndex.staticPointGroupControllers[controlId].description;
	var pointGroupGuid = "";
	var pointGroupType = FMOperateIndex.staticPointGroupControllers[controlId].visibility;

	if (parentId === 'mainTab') {
		FMOperateIndex.contents = $.map(FMOperateIndex.contents, function (obj) {
			if (obj.id === controlId) {
				ID = obj.name;
				pointGroupGuid = obj.settings.pointGroupGuid;
			}
			return obj;
		});
	}
	else {
		var parentGroupIdx = $.map(FMOperateIndex.contents, function (obj, index) {
			if (obj.id === parentId) {
				return 1;
			}
			return 0;
		});

		var foundTabGroupIdx = parentGroupIdx.indexOf(1);
		if (foundTabGroupIdx >= 0) {
			$.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj) {
				if (obj.id === controlId) {
					ID = obj.name;
					pointGroupGuid = obj.settings.pointGroupGuid;
				}
				return obj;
			});
		}
	}

	$.ajax({
		url: 'SavePointGroup',
		type: 'Post',
		dataType: 'json',
		contentType: "application/json",
		data: JSON.stringify({ "pointGroupGuid": pointGroupGuid, "id": ID, "description": description, "rows": JSON.stringify(rowsFiltered), "columns": JSON.stringify(columnsWithNoMenu), "fontSize": fontSize, "pointGroupType": parseInt(pointGroupType) }),
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					// we get back the pointGroupGuid during the save, for when we add a new pointgroup
					var pointGroupGuid = data.pointGroupGuid;

					// if we found an existing pointgroup with the same ID, visibility, site and owner
					if (data.duplicateFound) {
						FMErrorAndExceptionHandling.ShowError("Cannot Save Changes, duplicate PointGroup.", null, messageAttributes);
						return;
					}
					if (parentId === 'mainTab') {
						FMOperateIndex.contents = $.map(FMOperateIndex.contents, function (obj) {
							if (obj.id === controlId) {
								obj.settings.pointGroupGuid = pointGroupGuid;
							}
							return obj;
						});
					}
					else {
						var parentGroupIdx = $.map(FMOperateIndex.contents, function (obj, index) {
							if (obj.id === parentId) {
								return 1;
							}
							return 0;
						});

						var foundTabGroupIdx = parentGroupIdx.indexOf(1);
						if (foundTabGroupIdx >= 0) {
							$.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj) {
								if (obj.id === controlId) {
									obj.settings.pointGroupGuid = pointGroupGuid;
								}
								return obj;
							});
						}
					}

				}

			}, messageAttributes);
		},
		error: function (request, status, error) {
			FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
			}, messageAttributes);
		}
	});
};

// defines the menu to be displayed on headers
FMOperateIndex.PointGroupCreateHeaderMenu = function () {
	return [
		{
			iconCssClass: "header-menu-cell-alignment",
			title: "Cell Alignment",
			items: [
				{
					iconCssClass: "header-menu-cell-alignment-left",
					title: "Left",
					command: "left-align"
				},
				{
					iconCssClass: "header-menu-cell-alignment-center",
					title: "Center",
					command: "center-align"
				},
				{
					iconCssClass: "header-menu-cell-alignment-right",
					title: "Right",
					command: "right-align"
				}
			]

		},
		{
			iconCssClass: "header-menu-add-column",
			title: "Insert Column",
			items: [
				{
					iconCssClass: "header-menu-tag",
					title: "Tag",
					command: "insert-column-tag"
				},
				{
					iconCssClass: "header-menu-product",
					title: "Product Name",
					command: "insert-product-name"
				},
				{
					iconCssClass: "header-menu-product-description",
					title: "Product Description",
					command: "insert-product-description"
				},
				{
					iconCssClass: "header-menu-empty-column",
					title: "Empty Column",
					command: "insert-empty-column"
				}
			]
		},
		{
			iconCssClass: "header-menu-delete",
			title: "Delete Column",
			command: "delete-column"
		},
		{
			iconCssClass: "header-menu-rename",
			title: "Rename",
			command: "rename"
		},
		{
			iconCssClass: "header-menu-filter",
			title: "Filter",
			command: "filter"
		},
		{
			iconCssClass: "header-menu-set-display-precision",
			title: "Set Display Precision",
			command: "changeprecision"
		},
		{
			iconCssClass: "header-menu-set-display-unit",
			title: "Set Display Unit",
			command: "changeunit"
		},
		{
			iconCssClass: "header-menu-show-unit",
			title: "Show Units",
			command: "showunits"
		},
		{
			iconCssClass: "header-menu-show-quality",
			title: "Show Quality",
			command: "showquality"
		},
		{
			iconCssClass: "header-menu-export",
			title: "Export Tag Data",
			command: "exportdata"
		}
	];
}

// defines the menu to be displayed on cells
FMOperateIndex.PointGroupCreateCellMenu = function () {
	return [
		{
			iconCssClass: "cell-menu-insert-row",
			title: "Insert Row",
			items: [
				{
					iconCssClass: "cell-menu-insert-point",
					title: "Insert Point",
					command: "insert-point"
				},
				{
					iconCssClass: "cell-menu-insert-subtotal",
					title: "Insert SubTotal",
					command: "insert-subtotal"
				},
				{
					iconCssClass: "cell-menu-insert-total",
					title: "Insert Total",
					command: "insert-total"
				},
				{
					iconCssClass: "cell-menu-insert-empty-row",
					title: "Insert Empty Row",
					command: "insert-emptyrow"
				}
			]

		},
		{
			iconCssClass: "cell-menu-remove-row",
			title: "Remove Row",
			command: "remove-row"
		},
		{
			iconCssClass: "cell-menu-calculations",
			title: "Calculation",
			hidden: true,
			items: [
				{
					iconCssClass: "glyphicon",
					title: "None",
					command: "totalizer-none"
				},
				{
					iconCssClass: "glyphicon",
					title: "Sum",
					command: "totalizer-sum"
				},
				{
					iconCssClass: "glyphicon",
					title: "Avg",
					command: "totalizer-avg"
				},
				{
					iconCssClass: "glyphicon",
					title: "Max",
					command: "totalizer-max"
				},
				{
					iconCssClass: "glyphicon",
					title: "Min",
					command: "totalizer-min"
				}
			]
		},
		{
			iconCssClass: "cell-menu-open-pointdetail",
			title: "Point Detail",
			command: "open-pointdetail"
		}
	];
}

FMOperateIndex.DeletePointGroup = function (pointGroupGuid, pointGroupID) {
	$("#menuPane").removeClass("hidden").addClass("hidden");
	$(".hamburger-menu").removeClass("active");

	FMLayout.ConfirmYesNo('Are you sure you want to close all open instances and delete the Point Group "' + pointGroupID + '"?', "Delete Point Group", function () {
		var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };

		$.ajax({
			type: 'Get',
			url: 'DeletePointGroup',
			dataType: "json",
			data: { "id": pointGroupGuid },
			cache: false,
			success: function (response) {
				var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator, width: '450px' };
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
					if (inError) {
						//--- We need to display some type of message
						return;
					}
					var tabsToDelete = [];
					// find all the instances of the point group that are open and close them
					$.each(FMOperateIndex.contents, function (index, tab) {
						// if dealing with tab groups go through each page
						if (tab.type === "group") {
							$.each(tab.settings, function (index, subtab) {
								if (subtab.settings.pointGroupGuid === pointGroupGuid) {
									tabsToDelete.push(subtab.id);
								}
							});
						}
						else if (tab.type === "pointGroup") {
							if (tab.settings.pointGroupGuid === pointGroupGuid) {
								tabsToDelete.push(tab.id);
							}
						}
					});

					// delete the tabs
					$.each(tabsToDelete, function (index, tabname) {
						// get the li control that is the tab
						FMOperateIndex.RemoveTab($("li a[data-target='#" + tabname + "']").parent());
					});
					// refresh the hamburger the menu (list of point groups)
					if ($(".operateMenuItem.active a").attr("id") === "menuPointGroups") {
						FMOperateIndex.refreshHamburgerMenu = true;
					}
				});
			},
			error: function (xhr, ajaxOptions, thrownError) {
				FMErrorAndExceptionHandling.ShowError(thrownError);
			}
		});
	});
}
FMOperateIndex.DeleteMultiplePointGroups = function () {
	$("#menuPane").removeClass("hidden").addClass("hidden");
	$(".hamburger-menu").removeClass("active");

	FMLayout.ConfirmYesNo('Are you sure you want to close all open instances and delete the listed Point Groups?', "Delete Point Groups", function () {
		var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };
		var pointGroupGuidList = $(".operatePointGroupSubMenuElement").not(".hidden").map(function () { return $(this).data("guid"); }).get();

		$.ajax({
			type: 'Post',
			url: 'DeleteMultiplePointGroups',
			dataType: "json",
			contentType: "application/json",
			data: JSON.stringify({ guidList: pointGroupGuidList }),
			cache: false,
			success: function (response) {
				var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator, width: '450px' };
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
					if (inError) {
						//--- We need to display some type of message
						return;
					}
					var tabsToDelete = [];
					// find all the instances of the point group that are open and close them
					$.each(FMOperateIndex.contents, function (index, tab) {
						// if dealing with tab groups go through each page
						for (var i = 0; i < pointGroupGuidList.length; i++) {
                            let pointGroupGuid = pointGroupGuidList[i];
							if (tab.type === "group") {
								$.each(tab.settings, function (index, subtab) {
									if (subtab.settings.pointGroupGuid === pointGroupGuid) {
										tabsToDelete.push(subtab.id);
									}
								});
							}
							else if (tab.type === "pointGroup") {
								if (tab.settings.pointGroupGuid === pointGroupGuid) {
									tabsToDelete.push(tab.id);
								}
							}
						}
					});

					// delete the tabs
					$.each(tabsToDelete, function (index, tabname) {
						// get the li control that is the tab
						FMOperateIndex.RemoveTab($("li a[data-target='#" + tabname + "']").parent());
					});
					// refresh the hamburger the menu (list of point groups)
					if ($(".operateMenuItem.active a").attr("id") === "menuPointGroups") {
						FMOperateIndex.refreshHamburgerMenu = true;
					}
				});
			},
			error: function (xhr, ajaxOptions, thrownError) {
				FMErrorAndExceptionHandling.ShowError(thrownError);
			}
		});
	});
}

FMOperateIndex.SavePointGroupAs = function (parentId, controlId) {
	var ID = "";
	if (parentId === 'mainTab') {
		FMOperateIndex.contents = $.map(FMOperateIndex.contents, function (obj) {
			if (obj.id === controlId) {
				ID = obj.name;
			}
			return obj;
		});
	}
	else {
		var parentGroupIdx = $.map(FMOperateIndex.contents, function (obj, index) {
			if (obj.id === parentId) {
				return 1;
			}
			return 0;
		});

		var foundTabGroupIdx = parentGroupIdx.indexOf(1);
		if (foundTabGroupIdx >= 0) {
			$.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj) {
				if (obj.id === controlId) {
					ID = obj.name;
				}
				return obj;
			});
		}
	}

	$("#pointGroupSaveScreen #pointGroupNewName").val(ID);
	$("#pointGroupSaveScreen #pointGroupNewDesc").val('');
	$("#pointGroupSaveScreen input[name=PointGroupPrivateSaveAs][value='1']").prop("checked", true); // default point groups to private

	$('body').modalmanager('loading');
	$("#pointGroupSaveScreen").modal("show");
	$("#pointGroupSaveScreen #pointGroupSaveSaveButton").off('click');

	$("#pointGroupSaveScreen #pointGroupSaveSaveButton").on('click', function () {
		// Try to save the point group
		var pointGroupVisibilityType = $("#pointGroupSaveScreen input[type=radio][name=PointGroupPrivateSaveAs]:checked").val();
		var pointName = $("#pointGroupSaveScreen #pointGroupNewName").val();
		var pointDescription = $("#pointGroupSaveScreen #pointGroupNewDesc").val();

		// make sure we have a name
		if (pointName === "") {
			$("#pointGroupSaveScreen #pointGroupNewName").parent().addClass('has-error');
			return false;
		}
		var sourceGrid = FMOperateIndex.staticPointGroupControllers[controlId]._grid;
		FMOperateIndex.ClonePointGroup(pointName, pointDescription, pointGroupVisibilityType, sourceGrid, null);
	});
}

FMOperateIndex.getPointFilterPointGroupOptions = function (popupContainer, args) {
	var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };
	var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator };

	$.ajax({
		url: 'GetDynamicPointGroupFilterOptions',
		type: 'GET',
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {

					var selected_point_types = [], selected_point_categories = [], selected_product_groups = [];
					if (args.column.hasOwnProperty('filter')) {
						selected_point_types = args.column.filter.point_type;
						selected_point_categories = args.column.filter.point_category;
						selected_product_groups = args.column.filter.product_group;
					}

					if (data.hasOwnProperty("point_type")) {
						$(popupContainer).find("[name=pointFilterPointType]").html('');
						var point_types = [];
						$.map(data["point_type"], function (obj, index2) {
							point_types.push({ id: obj.Item1, text: obj.Item2 });
						});

						var optns = point_types.map(function (item) {

							return '<option value="' + item.id + '" ' + (selected_point_types.indexOf(item.id) >= 0 ? ' selected ' : '') + '>' + item.text + '</option>';
						});
						$(popupContainer).find("[name=pointFilterPointType]").html(optns.join(""));
					}
					else {
						$(popupContainer).find("[name=pointFilterPointType]").html('');
					}

					if (data.hasOwnProperty("category")) {
						var categories = [];
						$.map(data["category"], function (obj, index2) {
							categories.push({ id: obj.Item1, text: obj.Item2 });
						});

						var optns = categories.map(function (item) {
							return '<option value="' + item.id + '" ' + (selected_point_categories.indexOf(item.id) >= 0 ? ' selected ' : '') + '>' + item.text + '</option>';
						});

						$(popupContainer).find("[name=pointFilterPointCategory]").html(optns.join(""));
					}
					else {
						$(popupContainer).find("[name=pointFilterPointCategory]").html('');
					}
					if (data.hasOwnProperty("product_group")) {

						var product_groups = [];
						$.map(data["product_group"], function (obj, index2) {
							product_groups.push({ id: obj.Item1, text: obj.Item2 });
						});

						var optns = product_groups.map(function (item) {
							return '<option value="' + item.id + '" ' + (selected_product_groups.indexOf(item.id) >= 0 ? ' selected ' : '') + '>' + item.text + '</option>';
						});

						$(popupContainer).find("[name=pointFilterProductGroup]").html(optns.join(""));
					}
					else {
						$(popupContainer).find("[name=pointFilterProductGroup]").html('');
					}

					$(popupContainer).find("[name=pointFilterPointType]").select2({ 'placeholder': 'Filter by Point Type' });
					$(popupContainer).find("[name=pointFilterPointCategory]").select2({ 'placeholder': 'Filter by Point Category' });
					$(popupContainer).find("[name=pointFilterProductGroup]").select2({ 'placeholder': 'Filter by Product Group' });
					$(popupContainer).find("[name=pointFilterPointType]").select2('focus');
				}
			}, messageAttributes);
		},
		error: function (request, status, error) {
			FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
			}, messageAttributes);
		}
	});
}

FMOperateIndex.addPointGroup = function () {

	$("#pointGroupSaveScreen #pointGroupNewName").val('');
	$("#pointGroupSaveScreen #pointGroupNewDesc").val('');

	if ($("#pointGroupSaveScreen input[name=PointGroupPrivateSaveAs][value='0']").attr('disabled') === "disabled") {
		$("#pointGroupSaveScreen input[name=PointGroupPrivateSaveAs][value='1']").prop("checked", true); // default point groups to private if cannot create public
	}
	else {
		$("#pointGroupSaveScreen input[name=PointGroupPrivateSaveAs][value='0']").prop("checked", true); // default point groups to public
	}

	$('body').modalmanager('loading');
	$("#pointGroupSaveScreen").modal("show");
	$("#pointGroupSaveScreen #pointGroupSaveSaveButton").off('click');

	$("#pointGroupSaveScreen #pointGroupSaveSaveButton").on('click', function () {
		// Try to save the point group
		var pointGroupVisibilityType = $("#pointGroupSaveScreen input[type=radio][name=PointGroupPrivateSaveAs]:checked").val();
		var pointName = $("#pointGroupSaveScreen #pointGroupNewName").val();
		var pointDescription = $("#pointGroupSaveScreen #pointGroupNewDesc").val();

		// make sure we have a name
		if (pointName === "") {
			$("#pointGroupSaveScreen #pointGroupNewName").parent().addClass('has-error');
			return false;
		}

		var stack_bottomright_operator = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };
		var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operator };

		// remove previous notifications
		PNotify.removeStack(stack_bottomright_operator);
		$('<div id="loaderpointgroupmain" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('body');

		$.ajax({
			url: 'AddPointGroup',
			type: 'Post',
			dataType: 'json',
			contentType: "application/json",
			data: JSON.stringify({ "id": pointName, "description": pointDescription, "pointGroupType": pointGroupVisibilityType }),
			success: function (response) {
				$("#loaderpointgroupmain").remove();
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
					if (!inError) {
						// if there is a duplicate show an error and go to the first page
						if (data.duplicateFound) {
							FMLayout.Alert("There is already a Point Group with the same Name and Visibility.", "Duplicate", null);

						}
						else {
							$("#pointGroupSaveScreen").modal("hide");
							FMOperateIndex.openPointGroup(pointName, data.pointGroupGuid);
						}
						// refresh the hamburger the menu (list of point groups)
						if ($(".operateMenuItem.active a").attr("id") === "menuPointGroups") {
							FMOperateIndex.refreshHamburgerMenu = true;
						}
					}
				}, messageAttributes);
			},
			error: function (request, status, error) {
				$("#loaderpointgroupmain").remove();
				FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
				}, messageAttributes);
			}
		});
	});

}

FMOperateIndex.convertArraysToCommaDelimited = function (obj) {
	if (obj != null) {
		if (obj.hasOwnProperty("value")) {
			if (Object.prototype.toString.call(obj.value) === '[object Array]') {
				obj.value = obj.value.join(", ");
			}
		}
		if (obj.hasOwnProperty("rules") && obj.rules != null) {
			for (var i = 0; i < obj.rules.length; i++) {
				FMOperateIndex.convertArraysToCommaDelimited(obj.rules[i]);
			}
		}
	}
}

FMOperateIndex.getAddPointGroupsFilterRules = function () {
	try {
		var res = $('#pointGroupAddQueryBuilder').queryBuilder('getRules');
		FMOperateIndex.convertArraysToCommaDelimited(res);
		return res;
	} catch (ex) {
		//console.log(ex);
		return null;
	}
}


FMOperateIndex.closePointGroup = function (id) {
	if (FMOperateIndex.staticPointGroupControllers.hasOwnProperty(id)) {
		var fmpointgroupgrid = FMOperateIndex.staticPointGroupControllers[id];
		if (!fmpointgroupgrid.isDynamic()) {
			FMOperateIndex.unsubscribeTagWebWorker(fmpointgroupgrid._uniqueId);
		}
		else {
			if (fmpointgroupgrid._updateDynamicTimer) {
				clearTimeout(fmpointgroupgrid._updateDynamicTimer);
				fmpointgroupgrid._updateDynamicTimer = null;
			}

			if (fmpointgroupgrid._updateDynamicAjaxRequest) {
				fmpointgroupgrid._updateDynamicAjaxRequest.abort();
				fmpointgroupgrid._updateDynamicAjaxRequest = null;
			}
		}

		delete FMOperateIndex.staticPointGroupControllers[id];
	}

}

// function to apply tag filters to the grid
FMOperateIndex.FilterPointGroupGrid = function (item, args) {
	var columns = args.columnsToFilter;
	var metadata = args.metadata;
	var returnValue = true;
	for (var i = 0; i < columns.length; i++) {
		if (columns[i].hasOwnProperty('filter') && item.hasOwnProperty('type') && item.type === "point") {

			// make sure that the row has a value for the filter we need to apply
			if (item.hasOwnProperty(columns[i].field)) {
				var tagMetadata = FMPointGroupGrid.getTagInfo(metadata, item.pointguid, columns[i].field);
				if (tagMetadata) {
					if (columns[i].filter.type === 'numeric' && (tagMetadata.ValueTypeString === "System.Double" || tagMetadata.ValueTypeString === "System.Int16" || tagMetadata.ValueTypeString === "System.Int32" || tagMetadata.ValueTypeString === "System.Int64")) {
						if (tagMetadata.Value != null || columns[i].filter.operator === "not_equal" || columns[i].filter.operator === "not_between") {
							var unit = tagMetadata.Units;
							var value = math.bignumber(tagMetadata.Value);
							var minRawValue = math.bignumber(columns[i].filter.minValue);
							var minFilterValue = 0;
							// if we don't have unit no need for unit conversion
							if (parseInt(unit) === -1) {
								minFilterValue = minRawValue;
							}
							else {
								minFilterValue = FMConvertEngUnits.Convert(minRawValue, parseInt(columns[i].filter.unit), parseInt(unit));

							}
							switch (columns[i].filter.operator) {
								case "equal":
									if (!math.equal(value, minFilterValue)) {
										returnValue &= false;
									}
									break;
								case "not_equal":
									if (math.equal(value, minFilterValue)) {
										returnValue &= false;
									}
									break;
								case "greater":
									if (!math.larger(value, minFilterValue)) {
										returnValue &= false;
									}
									break;
								case "greater_equal":
									if (!math.largerEq(value, minFilterValue)) {
										returnValue &= false;
									}
									break;
								case "less":
									if (!math.smaller(value, minFilterValue)) {
										returnValue &= false;
									}
									break;
								case "less_equal":
									if (!math.smallerEq(value, minFilterValue)) {
										returnValue &= false;
									}
									break;
								case "between":
									var maxFilterValue = math.bignumber(columns[i].filter.maxValue);
									// if we don't have unit no need for unit conversion
									if (parseInt(unit) !== -1) {
										maxFilterValue = FMConvertEngUnits.Convert(maxFilterValue, parseInt(columns[i].filter.unit), parseInt(unit));
									}

									if (!(math.largerEq(value, minFilterValue) && math.smallerEq(value, maxFilterValue))) {
										returnValue &= false;
									}
									break;
								case "not_between":
									var maxFilterValue = math.bignumber(columns[i].filter.maxValue);
									// if we don't have unit no need for unit conversion
									if (parseInt(unit) !== -1) {
										maxFilterValue = FMConvertEngUnits.Convert(maxFilterValue, parseInt(columns[i].filter.unit), parseInt(unit));
									}
									if (!(math.smaller(value, minFilterValue) || math.larger(value, maxFilterValue))) {
										returnValue &= false;
									}
									break;
								default:
									returnValue &= true;
							}
						}
						else  // if the column has no value then don't show the row
						{
							returnValue &= false;
						}
					}
					else if (columns[i].filter.type === 'boolean' && tagMetadata.ValueTypeString === "System.Boolean") {
						if (item[columns[i].field] !== null) {
							returnValue &= (item[columns[i].field].toLowerCase() === columns[i].filter.Value.toLowerCase());
						}
						else
							returnValue &= false; // convert to boolean and compare the filter and value
					}
					else if (columns[i].filter.type === 'enum' && tagMetadata.ValueTypeString.startsWith("FMBusinessObjects.DataObjects.CodedVariables")) {
						returnValue &= columns[i].filter.Value.indexOf(item[columns[i].field]) >= 0;  // value must be in the array of filters
					}
					else if (columns[i].filter.type === 'string' && tagMetadata.ValueTypeString === "System.String") {
						// if filter is specified
						if (columns[i].filter.Value !== "") {
							if (item[columns[i].field]) {
								var searchValue = columns[i].filter.Value.toLowerCase();
								returnValue &= (item[columns[i].field].toLowerCase().indexOf(searchValue) !== -1); // check if the value contains the string we are looking for
							}
							else {
								returnValue &= false;
							}
						}
						else //filter value is blank so show things with empty or no value
						{
							if (item[columns[i].field]) {
								returnValue &= false;
							}
							else {
								returnValue &= true;
							}
						}

					}
					else if (columns[i].filter.type === 'datetimeoffset' && tagMetadata.ValueTypeString === "System.DateTimeOffset") {
						if (tagMetadata.Value != null || columns[i].filter.operator === "not_equal" || columns[i].filter.operator === "not_between") {
							var re = /-?\d+/;
							var m = re.exec(tagMetadata.Value);
							var value = m != null ? new Date(parseInt(m[0])) : null;
							var minFilterValue = new Date(columns[i].filter.minValue);

							switch (columns[i].filter.operator) {
								case "equal":
									if (value !== minFilterValue) {
										returnValue &= false;
									}
									break;
								case "not_equal":
									if (value === minFilterValue) {
										returnValue &= false;
									}
									break;
								case "greater":
									if (value <= minFilterValue) {
										returnValue &= false;
									}
									break;
								case "greater_equal":
									if (value < minFilterValue) {
										returnValue &= false;
									}
									break;
								case "less":
									if (value >= minFilterValue) {
										returnValue &= false;
									}
									break;
								case "less_equal":
									if (value > minFilterValue) {
										returnValue &= false;
									}
									break;
								case "between":
									var maxFilterValue = new Date(columns[i].filter.maxValue);

									if (!(value >= minFilterValue && value <= maxFilterValue)) {
										returnValue &= false;
									}
									break;
								case "not_between":
									var maxFilterValue = new Date(columns[i].filter.maxValue);

									if (!(value < minFilterValue && value > maxFilterValue)) {
										returnValue &= false;
									}
									break;
								default:
									returnValue &= true;
							}
						}
						else  // if the column has no value then don't show the row
						{
							returnValue &= false;
						}
					}
					else if (columns[i].filter.type === 'timespan' && tagMetadata.ValueTypeString === "System.TimeSpan") {

						if (tagMetadata.Value != null || columns[i].filter.operator === "not_equal" || columns[i].filter.operator === "not_between") {
							var value = tagMetadata.Value != null ? tagMetadata.Value.Days + (((tagMetadata.Value.Hours * 60 * 60) + (tagMetadata.Value.Minutes * 60) + tagMetadata.Value.Seconds) / 86400) : null;
							var minFilterValue = columns[i].filter.minValue.days + (((columns[i].filter.minValue.hours * 60 * 60) + (columns[i].filter.minValue.minutes * 60) + columns[i].filter.minValue.seconds) / 86400);

							switch (columns[i].filter.operator) {
								case "equal":
									if (value !== minFilterValue) {
										returnValue &= false;
									}
									break;
								case "not_equal":
									if (value === minFilterValue) {
										returnValue &= false;
									}
									break;
								case "greater":
									if (value <= minFilterValue) {
										returnValue &= false;
									}
									break;
								case "greater_equal":
									if (value < minFilterValue) {
										returnValue &= false;
									}
									break;
								case "less":
									if (value >= minFilterValue) {
										returnValue &= false;
									}
									break;
								case "less_equal":
									if (value > minFilterValue) {
										returnValue &= false;
									}
									break;
								case "between":
									var maxFilterValue = columns[i].filter.maxValue.days + (((columns[i].filter.maxValue.hours * 60 * 60) + (columns[i].filter.maxValue.minutes * 60) + columns[i].filter.maxValue.seconds) / 86400);

									if (!(value >= minFilterValue && value <= maxFilterValue)) {
										returnValue &= false;
									}
									break;
								case "not_between":
									var maxFilterValue = columns[i].filter.maxValue.days + (((columns[i].filter.maxValue.hours * 60 * 60) + (columns[i].filter.maxValue.minutes * 60) + columns[i].filter.maxValue.seconds) / 86400);

									if (!(value < minFilterValue && value > maxFilterValue)) {
										returnValue &= false;
									}
									break;
								default:
									returnValue &= true;
							}
						}
						else  // if the column has no value then don't show the row
						{
							returnValue &= false;
						}
					}
				}
			}
		}
	}

	return returnValue;
}




// open a Point Group
FMOperateIndex.openPointGroup = function (pointGroupId, pointGroupGuid) {
	// can't open point in a group if there are no groups (there is always 1 tab, the tab to add groups )
	if (FMOperateIndex.isTabGroupEnabled && $('#tabList').children().length === 1) {
		FMLayout.Alert('Cannot open a Point without a Group.', 'Error');
		return null;
	}

	var activeTab = 'mainTab';
	if (FMOperateIndex.isTabGroupEnabled) {
		activeTab = $('#tabList > li.active > a').attr('data-target').replace('#', '');
	}

	//check to see if we have one open already (only when clicking on the menu, not when restoring the screen)
	if (FMOperateIndex.restoringView === false) {
		var parentTab = "";
		var pointgroupTab = "";

		var parentGroup = FMOperateIndex.contents;

		FMOperateIndex.UpdateOperateHelpKey("pointGroup");

		if (activeTab !== 'mainTab') {
			parentTab = activeTab;
			var parentGroupFiltered = $.grep(parentGroup, function (e) {
				return e.id === activeTab;
			});
			if (parentGroupFiltered.length > 0) {
				parentGroup = parentGroupFiltered[0];
			}

			$.each(parentGroup.settings, function (index, level1Tab) {
				if (level1Tab.type === "pointGroup" && level1Tab.settings.pointGroupGuid === pointGroupGuid) {
					pointgroupTab = level1Tab.id;
				}
			});
		}
		else {
			$.each(parentGroup, function (index, level1Tab) {
				if (level1Tab.type === "pointGroup" && level1Tab.settings.pointGroupGuid === pointGroupGuid) {
					pointgroupTab = level1Tab.id;
				}
			});
		}

		if (parentTab !== "") {
			$('a[data-target=\'#' + parentTab + '\'').click();
		}
		if (pointgroupTab !== "") {
			$('a[data-target=\'#' + pointgroupTab + '\'').click();
			return false;
		}
	}

	if (!pointGroupGuid)
		pointGroupGuid = '00000000-0000-0000-0000-000000000000';

	var isNewpointGroup = (pointGroupGuid === '00000000-0000-0000-0000-000000000000');

	if (!pointGroupId) {
		pointGroupId = 'Point Group ' + Math.floor(Math.random() * 100000);
	}

	var newId = FMOperateIndex.AddTab(pointGroupId);

	if (newId === null) return false;

	FMOperateIndex.PersistNewControl(activeTab, newId, pointGroupId, 'pointGroup', { pointGroupGuid: pointGroupGuid });

	// start the process of restoring the tab
	FMOperateIndex.restoringScreenQueueInProgress[newId] = true;

	$("#" + newId).getNiceScroll().remove();

	var dynamicStyleSheetAlarms = $("<style type='text/css' rel='stylesheet' />").appendTo($("head"));
	var rules = [];


	$('<div id="pointgroup' + newId + '" class="point-group pointgrouptemp" style="position: absolute; top: 5px; left: 5px;right: 5px;bottom: 5px;"></div>').appendTo('#' + newId);
	$('<div id="pointgroup' + newId + 'container" class="point-group-panel active" style="overflow:hidden"></div>').appendTo('#pointgroup' + newId);

	$('<div id="loaderpointgroup' + newId + '" class="LoadingAnimation"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('#' + newId);
	// put messages on the actual tab
	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#pointgroup' + newId) };

	$.ajax({
		type: 'get',
		dataType: 'json',
		cache: false,
		url: 'GetOperatePointGroup',
		activeTab: activeTab,
		newId: newId,
		data: { "id": pointGroupGuid, "pointName": pointGroupId },
		success: function (response) {
			var activeTab = this.activeTab;
			var newId = this.newId;

			$("#loaderpointgroup" + newId).remove();

			var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

			FMErrorAndExceptionHandling.HandleMessages(response,
				function (pointGroupConfiguration, inError) {
					// if it was not in error load and update the drawing
					if (!inError) {
						pointGroupGuid = pointGroupConfiguration.PointGroupGuid;
						var data = [];
						var columns = [];
						var dataView; // we are going to use a dataView instead of working with rows of data directly so we can apply filters
						var fontSize = (pointGroupConfiguration && pointGroupConfiguration.FontSize) ? parseInt(pointGroupConfiguration.FontSize) : 14;

						// Formatter for the cells in the slickgrid 
						//( this is just a proxy to call the formatter in FMPointGroup which may not have been defined yet, it gets defined after the grid is created but this function can be called before is actually created)
						function staticPointGroupFormatter(row, cell, value, columnDef, dataContext) {
							if (FMOperateIndex.staticPointGroupControllers[newId]) {
								return FMOperateIndex.staticPointGroupControllers[newId].staticPointGroupFormatter(row, cell, value, columnDef, dataContext);
							}
							else
								return '';
						}


						columns = JSON.parse(pointGroupConfiguration.Columns);
						data = JSON.parse(pointGroupConfiguration.Rows);
						// if we have a dynamic point group we only care about the total row since the rest will be retrieved based on the filter
						var isDynamic = (columns && columns[0] && columns[0].field === "point" && columns[0].hasOwnProperty("filter"));
						if (isDynamic) {
							data = $.map(data, function (row, idx) {
								if (row.type === "total") {
									return row;
								}
							});
						}

						data = $.map(data, function (row, idx) {
							if (row.type === "point") {
								// remove the values and get them from the server
								return { id: row.id, point: row.point, pointguid: row.pointguid, type: row.type };
							}
							else {
								return row;
							}

						});

						// Recreate menu items in columns
						for (var i = 0; i < columns.length; i++) {
							columns[i].header = {
								menu: { items: FMOperateIndex.PointGroupCreateHeaderMenu() }
							};
							columns[i].formatter = staticPointGroupFormatter;
							columns[i].name = decodeURIComponent(columns[i].name);
							if ($('#ModifyPointGroupsRight').val() == 'False') {
								columns[i].resizable = false;
							}
							else {
								columns[i].resizable = true;
							}
						}

						var options =
						{
							editable: true,
							enableCellNavigation: true,
							enableColumnReorder: true,
							forceFitColumns: false,
							frozenColumn: 0,
							asyncEditorLoading: false,
							autoEdit: false,
							fontSize: fontSize,
							rowHeight: 35,
							cellMenu: { items: FMOperateIndex.PointGroupCreateCellMenu() }
						};

						// Add menu items to columns
						for (var j = 0; j < columns.length; j++) {
							columns[j].header = {
								menu: { items: FMOperateIndex.PointGroupCreateHeaderMenu() }
							};
						}

						dataView = new Slick.Data.DataView({ inlineFilters: true });

						var grid = new Slick.Grid("#pointgroup" + newId + 'container', dataView, columns, options);
						grid.setSelectionModel(new Slick.RowSelectionModel());

						// wire up model events to drive the grid
						dataView.onRowCountChanged.subscribe(function (e, args) {
							grid.updateRowCount();
							grid.render();
						});
						dataView.onRowsChanged.subscribe(function (e, args) {
							grid.invalidateRows(args.rows);
							grid.render();
						});

						grid.onDblClick.subscribe (function (e) {
							// if we are not clicking on the left mouse button then ignore the event
							if (e.which !== 1) {
								return;
							}

							var cell = grid.getCellFromEvent(e);
							if (!cell) {
								return;
							}

							if (e.isImmediatePropagationStopped()) {
								return;
							}

							var rows = grid.getData().getFilteredItems();
							if (rows[cell.row].type != 'point') {
								return;
							}

							if (columns[cell.cell].field === 'point') {
								FMOperateIndex.openPoint(rows[cell.row].point, rows[cell.row].pointguid);
								return;
							}
						});

						grid.onClick.subscribe(function (e) {
							// if we are not clicking on the left mouse button then ignore the event
							if (e.which !== 1) {
								return;
							}

							var gridCell = grid.getCellFromEvent(e);
							if (!gridCell) {
								return;
							}

							if (e.isImmediatePropagationStopped()) {
								return;
							}

							var rows = grid.getData().getFilteredItems();
							if (rows[gridCell.row].type != 'point') {
								return;
							}


							var columns = grid.getColumns();
							if (columns[gridCell.cell].field == ''
								|| columns[gridCell.cell].field == 'Point'
								|| columns[gridCell.cell].field == 'ProductID'
								|| columns[gridCell.cell].field == 'ProductDescription') {
								return;
							}

							var container = grid.getContainerNode();
							var tabContent = $(container).parent().parent().parent();
							var pointGroupControllerId = $(tabContent).children('.active').attr("id");
							var pointGroupGrid = FMOperateIndex.staticPointGroupControllers[pointGroupControllerId];
							var metadata = pointGroupGrid.getMetadata();


							// metadata doesn't necessarily align with grid.
							var metaDataRow;
							for (metaDataRow = 0; metaDataRow < metadata.length; metaDataRow++) {
								if (metadata[metaDataRow].point == rows[gridCell.row].point) {
									break;
								}
							}

							if (metaDataRow == metadata.length) {
								return;
							}

							var metaDataCell;
							for (metaDataCell = 0; metaDataCell < metadata[metaDataRow].tags.length; metaDataCell++) {
								if (metadata[metaDataRow].tags[metaDataCell].ID == columns[gridCell.cell].field) {
									break;
								}
							}

							if (metaDataCell == metadata[metaDataRow].tags.length) {
								return;
							}

							var tag = metadata[metaDataRow].tags[metaDataCell];

							var pointValueIdentifier = { IdentityGuid: tag.PointTagGuid, PointValueType: 0, PropertyID: null };
							if (tag.Access
								&& tag.Access.Modify
								&& (tag.InputOutputType === 1
									|| (tag.Access.Override == true
										&& tag.InhibitOverride == false))) {
								FMOperateIndex.editValue(pointValueIdentifier);
							}
						});

						// initialize the model after all the events have been hooked up
						dataView.beginUpdate();
						dataView.setItems(data);

						FMOperateIndex.staticPointGroupControllers[newId] = new FMPointGroupGrid(newId, grid, pointGroupConfiguration.Description, pointGroupConfiguration.PointGroupType, pointGroupConfiguration.FontSize, pointGroupConfiguration.Owner, pointGroupConfiguration.IsOwnedByMe, pointGroupConfiguration.IsEditable);
						var shadowGrid = FMOperateIndex.staticPointGroupControllers[newId];

						FMOperateIndex.updateFilterParameters(grid, FMOperateIndex.staticPointGroupControllers[newId].getMetadata());
						dataView.setFilter(FMOperateIndex.FilterPointGroupGrid);
						dataView.endUpdate();

						// if you don't want the items that are not visible (due to being filtered out
						// or being on a different page) to stay selected, pass 'false' to the second arg
						dataView.syncGridSelection(grid, true);

						/*--------------- DISPLAY FILTER INDICATOR FOR COLUMN -----------------*/
						var filterIndicatorPlugin = new Slick.Plugins.HeaderFilterIndicator();

						grid.registerPlugin(filterIndicatorPlugin);
						/*--------------- DISPLAY FILTER INDICATOR FOR COLUMN  -----------------*/

						// if is dynamic we can start retrieving data
						if (isDynamic) {
							FMOperateIndex.subscribeDynamicPointGroup(FMOperateIndex.staticPointGroupControllers[newId].geUniqueId());
							FMOperateIndex.UpdateDynamicPointGroup(grid, newId);
						}

						// Persist the new tab so it can be re-open when the screen is reloaded
						var columnsWithNoMenu = $.extend(true, [], columns);  // copy array by value so we don't lose the original menu
						columnsWithNoMenu = $.map(columnsWithNoMenu, function (val, i) { val.header = null; return val; });

						var myGridColumns = grid.getColumns();
						// disable the column reordering in the first column (the name)
						grid.onColumnsReordered.subscribe(function (e, args) {
							if (myGridColumns[0].id !== grid.getColumns()[0].id) {
								grid.setColumns(myGridColumns);
							}
							else {
								myGridColumns = grid.getColumns();
							}

							FMOperateIndex.PersistPointGroup(activeTab, newId, grid);

						});

						/*--------------- EVENTS  -----------------*/
						// event to switch between configuration and grid
						$('#pointgroup' + newId + 'switch').on('click', function () {
							if ($('#pointgroup' + newId + 'container').hasClass('active')) {
								$('#pointgroup' + newId + 'container').fadeOut("slow", function () {
									$('#pointgroup' + newId + 'container').removeClass("active");
								});
								$('#pointgroup' + newId + 'settings').fadeIn("slow", function () {
									$('#pointgroup' + newId + 'settings').addClass("active");
									$('#pointgroup' + newId + 'switch').removeClass('glyphicon-cog').addClass('glyphicon-th').attr('title', 'Grid');
									$('#pointgroup' + newId + 'switch label').text('Grid');
								});
							}
							else {
								$('#pointgroup' + newId + 'settings').fadeOut("slow", function () {
									$('#pointgroup' + newId + 'settings').removeClass("active");
								});
								$('#pointgroup' + newId + 'container').fadeIn("slow", function () {
									$('#pointgroup' + newId + 'container').addClass("active");
									$('#pointgroup' + newId + 'switch').removeClass('glyphicon-th').addClass('glyphicon-cog').attr('title', 'Configuration');
									$('#pointgroup' + newId + 'switch label').text('Configuration');
									grid.resizeCanvas();
								});
							}
						});

						// if point group is not editable no need for menus or events
						if (pointGroupConfiguration.IsEditable) {

							// Double click on the tab name to rename the point group
							$('a[data-target="#' + newId + '"]').attr('ondblclick', "FMOperateIndex.RenameTab( this );");

							/*--------------- DRAG ROWS TO MOVE  -----------------*/
							grid.setSelectionModel(new Slick.RowSelectionModel());

							var moveRowsPlugin = new Slick.RowMoveManager({
								cancelEditOnDrag: true
							});
							moveRowsPlugin.onBeforeMoveRows.subscribe(function (e, data) {
								for (var i = 0; i < data.rows.length; i++) {
									// no point in moving before or after itself
									if (data.rows[i] == data.insertBefore || data.rows[i] == data.insertBefore - 1) {
										e.stopPropagation();
										return false;
									}
								}
								return true;
							});

							moveRowsPlugin.onMoveRows.subscribe(function (e, args) {
								var extractedRows = [], left, right;
								var rows = args.rows;
								var insertBefore = args.insertBefore;
								var dataView = grid.getData();
								var data = dataView.getItems();

								dataView.beginUpdate();

								// delete the rows from the grid
								for (var i = 0; i < rows.length; i++) {
									extractedRows.push(dataView.getItem(rows[i]));
									dataView.deleteItem(dataView.getItem(rows[i]).id);
								}

								// find where we need to add them
								if (dataView.getItem(insertBefore))  // if not in the last row
								{
									var insertBeforeId = dataView.getItem(insertBefore).id;
									var dataViewInsertBefore = dataView.getIdxById(insertBeforeId);

									for (var i = 0; i < extractedRows.length; i++) {
										dataView.insertItem(dataViewInsertBefore, extractedRows[i]);
									}
								}
								else {
									var lastPosition = dataView.getLength();

									for (var i = 0; i < extractedRows.length; i++) {
										dataView.insertItem(lastPosition, extractedRows[i]);
									}
								}
								dataView.endUpdate();
								FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
								// update totals
								var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
								FMPointGroupGrid.updateGridTotals(metadata, grid);
								grid.invalidateAllRows();
								grid.render();
							});

							grid.registerPlugin(moveRowsPlugin);

							/*--------------- END DRAG ROWS TO MOVE  -----------------*/

							/*--------------- COLUMN MENU  -----------------*/
							var headerMenuPlugin = new Slick.Plugins.HeaderContextMenu({});

							headerMenuPlugin.onBeforeMenuShow.subscribe(function (e, args) {
								// get the different engineering unit types that we have for the points selected in the grid
								var tagValueTypes = FMOperateIndex.staticPointGroupControllers[newId].getValueTypesForTag(args.column.field);
								var wellKnownGuids = FMOperateIndex.staticPointGroupControllers[newId].getWellKnownGuidsForTag(args.column.field);
								// default the menu items to be enabled
								args.menu.items[2].disabled = false;
								args.menu.items[4].disabled = false;
								args.menu.items[5].disabled = false;
								args.menu.items[6].disabled = false;
								args.menu.items[7].disabled = false;
								args.menu.items[8].disabled = false;

								// if no rights, disable all except the show commands
								if ($('#ModifyPointGroupsRight').val() == 'False') {
									args.menu.items[0].disabled = true;
									args.menu.items[1].disabled = true;
									args.menu.items[2].disabled = true;
									args.menu.items[3].disabled = true;
									args.menu.items[4].disabled = true;
									args.menu.items[5].disabled = true;
									args.menu.items[6].disabled = true;
								}

								// don't allow to delete the first column
								if (args.column.field === 'point') {
									args.menu.items[2].disabled = true;
									args.menu.items[5].disabled = true;
									args.menu.items[6].disabled = true;
									args.menu.items[7].disabled = true;
									args.menu.items[8].disabled = true;
								}
								// if we don't have any points (no unit types ) or we have points of multiple types then we cannot set the unit or precision
								else if ((tagValueTypes.length === 0 || tagValueTypes.length > 1)) {
									args.menu.items[4].disabled = true;
									args.menu.items[5].disabled = true;
									args.menu.items[6].disabled = true;
									args.menu.items[7].disabled = true;
									args.menu.items[8].disabled = true;
								}
								else {
									// disable filter for Transfer Target tag
									if (wellKnownGuids.indexOf('15117345-8C6B-45B0-8F4E-EFD82C5F0F10') > -1
										|| wellKnownGuids.indexOf('659BDE3D-E776-45AD-B5B9-9FA4A12CBD53') > -1
										|| wellKnownGuids.indexOf('74CE4476-BBE3-4B3B-AFB5-F3B496023845') > -1									) {
										args.menu.items[4].disabled = true;
									}

									var tagValue = tagValueTypes[0];
									if (tagValue === "System.Boolean") {
										args.menu.items[5].disabled = true;
										args.menu.items[6].disabled = true;
										args.menu.items[7].disabled = true;
									}
									else if (tagValue === "System.Double" || tagValue === "System.Int16" || tagValue === "System.Int32" || tagValue === "System.Int64") {
										var numericUnits = FMOperateIndex.staticPointGroupControllers[newId].getNumericUnitsForTag(args.column.field);
										var numericValueTypes = FMOperateIndex.staticPointGroupControllers[newId].getValueTypesForTag(args.column.field);

										var numericUnit = -9999;
										if (numericUnits.length === 1) {
											numericUnit = numericUnits[0];
										}

										var numericValueType = "None";
										if (numericValueTypes.length === 1) {
											numericValueType = numericValueTypes[0];
										}
										if (numericUnit !== -9999 && numericValueType !== "System.Double" && numericValueType !== "System.Int16" && numericValueType !== "System.Int32" && numericValueType !== "System.Int64") {
											args.menu.items[4].disabled = true;
											args.menu.items[5].disabled = true;
											args.menu.items[6].disabled = true;
											args.menu.items[7].disabled = true;
										}
										else if (numericUnit === 15) // if no units then disable the menus
										{
											args.menu.items[6].disabled = true;
											args.menu.items[7].disabled = true;
										}
										// integers don't need precision since they its always zero
										if (numericValueType === "System.Int16" && numericValueType === "System.Int32" && numericValueType === "System.Int64") {
											args.menu.items[5].disabled = true;
										}
									} else if (tagValue === "System.String") {
										args.menu.items[5].disabled = true;
										args.menu.items[6].disabled = true;
										args.menu.items[7].disabled = true;
									} else if (tagValue === "System.DateTimeOffset") {
										args.menu.items[5].disabled = true;
										args.menu.items[6].disabled = true;
										args.menu.items[7].disabled = true;
									} else if (tagValue === "System.TimeSpan") {
										args.menu.items[5].disabled = true;
										args.menu.items[6].disabled = true;
										args.menu.items[7].disabled = true;
									} else if (tagValue && tagValue.startsWith("FMBusinessObjects.DataObjects.CodedVariables")) {
										args.menu.items[5].disabled = true;
										args.menu.items[6].disabled = true;
										args.menu.items[7].disabled = true;
									} else // not a valid combination so disable the menus (no boolean, or numeric types)
									{
										args.menu.items[4].disabled = true;
										args.menu.items[5].disabled = true;
										args.menu.items[6].disabled = true;
										args.menu.items[7].disabled = true;
									}
								}

								if (args.column.hasOwnProperty('showunit')) {
									if (args.column.showunit) {
										args.menu.items[7].title = 'Hide Units';
										args.menu.items[7].iconCssClass = 'header-menu-hide-unit';
									}
									else {
										args.menu.items[7].title = 'Show Units';
										args.menu.items[7].iconCssClass = 'header-menu-show-unit';
									}
								}
								if (args.column.hasOwnProperty('showquality')) {
									if (args.column.showquality) {
										args.menu.items[8].title = 'Hide Quality';
										args.menu.items[8].iconCssClass = 'header-menu-hide-quality';
									}
									else {
										args.menu.items[8].title = 'Show Quality';
										args.menu.items[8].iconCssClass = 'header-menu-show-quality';
									}
								}

								// if we have a cell menu displayed we also want to close them before displaying the menu (we do it here because we stop the event propagation )
								$("#" + newId).find('.point-group').find('.slick-cellcontext-menu').each(function () {
									$(this).hide();
								});


								e.preventDefault();
							});

							headerMenuPlugin.onCommand.subscribe(function (e, args) {
								if (args.command === "insert-column-tag") {
									selectTagColumn(args);
								}
								else if (args.command === "insert-product-name") {
									insertPointPropertyColumn(args, "ProductID", "Product Name");
								}
								else if (args.command === "insert-product-description") {
									insertPointPropertyColumn(args, "ProductDescription", "Product Description");
								}
								else if (args.command === "insert-empty-column") {
									insertEmptyColumn(args);
								}
								else if (args.command === "center-align") {
									var headerCss = "";
									if (args.column.cssClass) {
										headerCss = args.column.cssClass.trim();
									}
									headerCss = headerCss.replace('text-left', '');
									headerCss = headerCss.replace('text-right', '');
									headerCss = headerCss.replace('text-center', '');
									headerCss += ' text-center';
									args.column.cssClass = headerCss;
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
								}
								else if (args.command === "left-align") {
									var headerCss = "";
									if (args.column.cssClass) {
										headerCss = args.column.cssClass.trim();
									}
									headerCss = headerCss.replace('text-left', '');
									headerCss = headerCss.replace('text-right', '');
									headerCss = headerCss.replace('text-center', '');
									headerCss += ' text-left';
									args.column.cssClass = headerCss;
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
								}
								else if (args.command === "right-align") {
									var headerCss = "";
									if (args.column.cssClass) {
										headerCss = args.column.cssClass.trim();
									}
									headerCss = headerCss.replace('text-left', '');
									headerCss = headerCss.replace('text-right', '');
									headerCss = headerCss.replace('text-center', '');
									headerCss += ' text-right';
									args.column.cssClass = headerCss;
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
								}
								else if (args.command === "rename") {
									var popover = $(args.headercell).popover("destroy").popover({
										container: 'body',
										placement: 'bottom',
										html: true,
										content: $('#renamePointGroupHeader').html(),
										trigger: "manual"
									});
									var dataPopover = popover.data('bs.popover');
									$("#customModalBackground").removeClass("hidden");
									$(args.headercell).popover('show');

									// update values in the popover
									dataPopover.tip().find('.popover-content').find('#fieldname').val(args.column.field);
									dataPopover.tip().find('.popover-content').find('#header').val(args.column.name);
									dataPopover.tip().find('.popover-content').find('#header').focus();

									// click on reset name
									dataPopover.tip().find('.popover-content').find('[name=renamepointgroupResetName]').on('click', function (event) {
										var resetName = args.column.field;
										//Point Properties need to be properly renamed since we cannot use the propertyID
										if (resetName === "ProductID") {
											resetName = "Product Name";
										}
										if (resetName === "ProductDescription") {
											resetName = "Product Description";
										}
										dataPopover.tip().find('.popover-content').find('#header').val(resetName);
									});

									// close the pop over when clicking cancel
									dataPopover.tip().find('.popover-content').find('[name=renamepointgroupcancel]').on('click', function (event) {
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										event.stopPropagation();
									});

									// update the column name when clicking ok
									dataPopover.tip().find('.popover-content').find('[name=renamepointgroupok]').on('click', function (event) {
										args.grid.updateColumnHeader(args.column.id, dataPopover.tip().find('.popover-content').find('#header').val());
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
										event.stopPropagation();
									});
								}
								else if (args.command === "delete-column") {
									FMLayout.ConfirmYesNo("Are you sure you want to delete the selected column: " + args.column.name + "?", "Delete Column", function () {
										var columns = grid.getColumns().slice(0);
										// insert the new column in the middle somewhere
										var pos = columns.map(function (e) {
											return e.id;
										}).indexOf(args.column.id);
										columns.splice(pos, 1);
										grid.setColumns(columns);

										FMOperateIndex.PersistPointGroup(activeTab, newId, grid);

										FMOperateIndex.updateFilterParameters(grid, FMOperateIndex.staticPointGroupControllers[newId].getMetadata());
										dataView.refresh();

									});
								}
								else if (args.command === "changeprecision") {
									var popover = $(args.headercell).popover("destroy").popover({
										container: 'body',
										placement: 'bottom',
										html: true,
										content: $('#changePrecisionPointGroupHeader').html(),
										trigger: "manual"
									});
									var dataPopover = popover.data('bs.popover');
									$("#customModalBackground").removeClass("hidden");
									$(args.headercell).popover('show');

									// add an id to the checkbox so the label automatically changes the checkbox (if specified in the html we get duplicates because it makes a copy of the html and it will not work)
									dataPopover.tip().find('.popover-content').find('input[type=checkbox]').attr("id", "changePrecisionDefaultToPoint");

									var precisionField = dataPopover.tip().find('.popover-content').find('[name=numDecimals]');
									$(precisionField).spinner({
										min: 0,
										max: 9,
										step: 1
									}).on('input', function () {
										if ($(this).data('onInputPrevented'))
											return;
										var val = this.value,
											$this = $(this),
											max = $this.spinner('option', 'max'),
											min = $this.spinner('option', 'min');
										if (this.value.length > 1) val = this.value.substring(0, 1);
										// We want only number, no alpha.
										// We set it to previous default value.         
										if (!val.match(/^[\d]$/))
											val = $(this).data('defaultValue');
										this.value = val > max ? max : val < min ? min : val;
									}).on('keydown', function (e) {
										// we set default value for spinner.
										if (!$(this).data('defaultValue'))
											$(this).data('defaultValue', this.value);
										// To handle backspace
										$(this).data('onInputPrevented', e.which === 8 ? true : false);
									}); // set default value
									// update values in the popover
									if (args.column.hasOwnProperty('DecimalPlaces')) {
										if (args.column['DecimalPlaces'] === -1) {
											$(precisionField).spinner("value", 0);
											$(precisionField).spinner('disable');
											$('input[name=changePrecisionDefaultToPoint]').prop('checked', true);
										}
										else {
											$(precisionField).spinner("value", args.column['DecimalPlaces']);
											$(precisionField).spinner('enable');
											$('input[name=changePrecisionDefaultToPoint]').prop('checked', false);
										}
									}
									else {
										$(precisionField).spinner("value", 0);
										$(precisionField).spinner('disable');
										$('input[name=changePrecisionDefaultToPoint]').prop('checked', true);
									}


									// close the pop over when clicking cancel
									dataPopover.tip().find('.popover-content').find('[name=changePrecisionPointGroupCancel]').on('click', function (event) {
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										event.stopPropagation();
									});
									// update the column name when clicking ok
									dataPopover.tip().find('.popover-content').find('[name=changePrecisionPointGroupOk]').on('click', function (event) {
										if ($('input[name=changePrecisionDefaultToPoint]').prop('checked')) {
											args.column['DecimalPlaces'] = -1;
										}
										else {
											args.column['DecimalPlaces'] = $(precisionField).spinner('value');
										}
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										grid.invalidateAllRows();
										grid.render();
										FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
										event.stopPropagation();
									});
								}
								else if (args.command === "changeunit") {
									var popover = $(args.headercell).popover("destroy").popover({
										container: 'body',
										placement: 'bottom',
										html: true,
										content: $('#changeUnitPointGroupHeader').html(),
										trigger: "manual"
									});
									var dataPopover = popover.data('bs.popover');
									$(args.headercell).popover('show');
									$("#customModalBackground").removeClass("hidden");

									// add an id to the checkbox so the label automatically changes the checkbox (if specified in the html we get duplicates because it makes a copy of the html and it will not work)
									dataPopover.tip().find('.popover-content').find('input[type=checkbox]').attr("id", "changeUnitDefaultToPoint");

									// add checked based upon current configuration
									if (args.column['Unit'] === -1) {
										$('input[name=changeUnitDefaultToPoint]').prop("checked", true);
									}


									// update values in the popover
									loadUnitsByUnitType(dataPopover.tip().find('.popover-content').find('[name=changeunitUOMList]'), args.column);

									// close the pop over when clicking cancel
									dataPopover.tip().find('.popover-content').find('[name=changeUnitPointGroupCancel]').on('click', function (event) {
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										event.stopPropagation();
									});

									// update unit when clicking ok
									dataPopover.tip().find('.popover-content').find('[name=changeUnitPointGroupOk]').on('click', function (event) {
										if (dataPopover.tip().find('.popover-content').find('[name=changeUnitDefaultToPoint]').prop('checked')) {
											args.column['Unit'] = -1;
										}
										else {
											var selectedUnit = dataPopover.tip().find('.popover-content').find('.list-group-item.active');
											if (selectedUnit.length === 1) {
												args.column['Unit'] = parseInt(selectedUnit.attr('data-value'));
											}
										}
										grid.invalidateAllRows();
										grid.render();
										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");
										FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
										event.stopPropagation();
									});
								}
								else if (args.command === "showunits") {
									if (args.column.hasOwnProperty('showunit')) {
										args.column.showunit = !args.column.showunit;
									}
									else {
										args.column.showunit = true;
									}
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
									grid.invalidateAllRows();
									grid.render();
								}
								else if (args.command === "showquality") {
									if (args.column.hasOwnProperty('showquality')) {
										args.column.showquality = !args.column.showquality;
									}
									else {
										args.column.showquality = true;
									}
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
									grid.invalidateAllRows();
									grid.render();
								}
								else if (args.command === "filter") {
									if (args.column.field === 'point')  // if point filter
									{
										pointFilter(args);
									}
									else // we are dealing with a tag filter
									{
										var tagValueTypes = FMOperateIndex.staticPointGroupControllers[newId].getValueTypesForTag(args.column.field);
										var valueType = tagValueTypes[0];
										if (valueType === "System.Double" || valueType === "System.Int16" || valueType === "System.Int32" || valueType === "System.Int64") {
											numericFilter(args);
										}
										else if (valueType === "System.Boolean") {
											booleanFilter(args);
										}
										else if (valueType === "System.DateTimeOffset") {
											dateTimeOffsetFilter(args);
										}
										else if (valueType === "System.TimeSpan") {
											timeSpanFilter(args);
										}
										else if (valueType === "System.String") {
											stringFilter(args);
										} else if (valueType.startsWith("FMBusinessObjects.DataObjects.CodedVariables")) {
											enumFilter(args);
										}
									}
								}
								else if (args.command == "exportdata") {
									if (FMOperateIndex.isTabGroupEnabled)
										var subscriptionGuid = FMOperateIndex.staticPointGroupControllers[$(".tab-pane.active .tab-pane.active").attr("id")]._uniqueId;
									else
										var subscriptionGuid = FMOperateIndex.staticPointGroupControllers[$(".tab-pane.active").attr("id")]._uniqueId;

									for (i in FMOperateIndex.tagWebWorkerSubscriptions) {
										if (FMOperateIndex.tagWebWorkerSubscriptions[i].id && FMOperateIndex.tagWebWorkerSubscriptions[i].id == subscriptionGuid)
											tagList = FMOperateIndex.tagWebWorkerSubscriptions[i].tagList;
									}

									FMOperateIndex.tagWebWorker.postMessage({ name: "download", subId: subscriptionGuid });


								}
							});

							// filter point column
							var pointFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#PointFilterPointGroupHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								dataPopover.tip().find("[name=pointFilterPointType]").select2({
									multiple: true,
									placeholder: "Loading..."
								});

								dataPopover.tip().find("[name=pointFilterPointCategory]").select2({
									multiple: true,
									placeholder: "Loading..."
								});

								dataPopover.tip().find("[name=pointFilterProductGroup]").select2({
									multiple: true,
									placeholder: "Loading..."
								});

								//populate values from the stored filter
								if (args.column.hasOwnProperty('filter')) {
									dataPopover.tip().find('.popover-content').find('[name=pointFilterPointName]').val(args.column.filter.point_name);
									dataPopover.tip().find('.popover-content').find('[name=pointFilterProductName]').val(args.column.filter.product_name);
								}

								FMOperateIndex.getPointFilterPointGroupOptions(dataPopover.tip().find('.popover-content'), args);

								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=pointFilterPointGroupReset]').on('click', function (event) {
									// remove the filter only if there was already a filter, otherwise ignore it and just close the modal
									if (args.column.hasOwnProperty('filter')) {
										$.map(args.grid.getColumns(), function (elem, idx) {
											if (args.column.field === elem.field) {
												delete elem.filter;
											}
										});

										$(args.headercell).popover('destroy');

										var newRowDefinition = { id: FMOperateIndex.newGuid(), type: "empty" };

										var rows = dataView.getItems();
										// we want to keep the total row if it has one
										var dataRows = $.map(rows, function (row, idx) {
											if (row.type === "total") {
												return row;
											}
										});

										grid.setColumns(grid.getColumns());
										dataRows.unshift(newRowDefinition); //always add an empty row at the begining
										var dataview = args.grid.getData();
										dataview.setItems(dataRows);

										// update totals
										FMPointGroupGrid.updateGridTotals(FMOperateIndex.staticPointGroupControllers[newId].getMetadata(), args.grid);

										grid.invalidateAllRows();
										grid.render();

										FMOperateIndex.staticPointGroupControllers[newId].convertToStaticPointGroup();

										FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
										event.stopPropagation();
									}
									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=pointFilterPointGroupCancel]').on('click', function (event) {
									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=pointFilterPointGroupApply]').on('click', function (event) {
									// validate that there is a filter specified
									var point_type = dataPopover.tip().find("[name=pointFilterPointType]").val() || [];
									var point_category = dataPopover.tip().find("[name=pointFilterPointCategory]").val() || [];
									var product_group = dataPopover.tip().find("[name=pointFilterProductGroup]").val() || [];
									var point_name = dataPopover.tip().find("[name=pointFilterPointName]").val();
									var product_name = dataPopover.tip().find("[name=pointFilterProductName]").val();

									if (point_type.length === 0 && point_category.length === 0 && product_group.length === 0 && point_name === "" && product_name === "") {
										FMLayout.Alert("No filter specified.", "Missing Filter");
										return;
									}

									// we need to reuse the save of the filter
									var saveColumnFilter = function () {
										var description = "";
										if (point_type.length > 0) {
											var point_type_names = $.map(dataPopover.tip().find("[name=pointFilterPointType]").select2('data'), function (val, i) {
												return val.text;
											});
											description += "Point Type: " + point_type_names.join(", ");
										}

										if (point_category.length > 0) {
											var point_category_names = $.map(dataPopover.tip().find("[name=pointFilterPointCategory]").select2('data'), function (val, i) {
												return val.text;
											});
											description += (description !== "" ? "\n" : "") + "Point Category: " + point_category_names.join(", ");
										}

										if (point_name !== "") {
											description += (description !== "" ? "\n" : "") + "Point Name Contains: " + point_name;
										}

										if (product_group.length > 0) {
											var product_group_names = $.map(dataPopover.tip().find("[name=pointFilterPointType]").select2('data'), function (val, i) {
												return val.text;
											});
											description += (description !== "" ? "\n" : "") + "Product Group: " + product_group_names.join(", ");
										}

										if (product_name !== "") {
											description += (description !== "" ? "\n" : "") + "Product Name Contains: " + product_name;
										}

										var filter = {
											type: 'point',
											point_type: point_type,
											point_category: point_category,
											point_name: point_name,
											product_group: product_group,
											product_name: product_name,
											description: encodeURIComponent(description)
										};

										$.map(args.grid.getColumns(), function (elem, idx) {
											if (args.column.field === elem.field) {
												elem.filter = filter;
											}
										});

										$(args.headercell).popover('destroy');
										$("#customModalBackground").removeClass("hidden").addClass("hidden");

										grid.setColumns(grid.getColumns());
										grid.invalidateAllRows();
										grid.render();
										// if we are a Static point group we need to start processing this new dynamic point group
										if (!FMOperateIndex.isSubscribedDynamicPointGroup(FMOperateIndex.staticPointGroupControllers[newId].geUniqueId())) {
											FMOperateIndex.staticPointGroupControllers[newId].convertToDynamicPointGroup();
											FMOperateIndex.UpdateDynamicPointGroup(grid, newId);
										}
										FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									}

									// if we don't have a filter but we have subtotals or point rows (a static point group) we need to warn of data loss
									var dataView = args.grid.getData();
									var rows = dataView.getItems();
									var dataRows = $.map(rows, function (row, idx) {
										if (row.type === "point" || row.type === "subtotal") {
											return row;
										}
									});

									if (!args.column.hasOwnProperty('filter') && dataRows.length > 0) {
										FMLayout.ConfirmYesNo("By applying the filter you will lose all the specified points. Are you sure you want to continue?",
											"Confirm Changes", function () {
												saveColumnFilter();
											}, null);
										event.stopPropagation();
									}
									else {
										saveColumnFilter();
									}

									event.stopPropagation();

								});
							}

							// filter numeric columns
							var numericFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#NumericFilterPointGroupHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								//populate values from the stored filter
								if (args.column.hasOwnProperty('filter')) {
									dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitOperator]').val(args.column.filter.operator);
									if (args.column.filter.operator === "between" || args.column.filter.operator === "not_between") {
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMaxLabel]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeClass('hidden');
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMaxLabel]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeClass('hidden').addClass('hidden');
									}

									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val(args.column.filter.minValue);
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val(args.column.filter.maxValue);
								}

								dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').focus();

								var numericUnits = FMOperateIndex.staticPointGroupControllers[newId].getNumericUnitsForTag(args.column.field);
								if (numericUnits.length === 1) {
									numericUnit = numericUnits[0];
								}

								// the default number of decimals is 2 unless is an int
								var defaultPrecision = 2;
								var numericValueTypes = FMOperateIndex.staticPointGroupControllers[newId].getValueTypesForTag(args.column.field);

								if (numericValueTypes.length === 1 && numericValueTypes[0].startsWith("System.Int")) {
									defaultPrecision = 0;
								}


								// if we have a unit type of  FMU_All, FMU_NODIM or FMU_NONE we don't need units
								if (numericUnit === 0 || numericUnit === 15 || numericUnit === 16) {
									dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').addClass("hidden");
									dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitLabel]').addClass("hidden");
								}
								else {
									loadUnitsByUnitType(dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]'), args.column);
								}

								// remember the unit before it can be changed
								dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').on('focus', function () {
									// Store the current value on focus
									$(this).data('oldValue', this.value);
								});
								// change the unit selection
								dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').on('change', function (event) {
									var newUnit = parseInt($(this).val());
									var oldUnit = parseInt($(this).data('oldValue'));

									// convert the unit
									var minValue = "";
									var maxValue = "";

									var numformatInfo = FMOperateIndex.numformatInfo;

									var numDecimals = defaultPrecision;
									if (args.column.hasOwnProperty('DecimalPlaces')) {
										if (args.column['DecimalPlaces'] !== -1)
											numDecimals = args.column['DecimalPlaces'];
									}
									numformatInfo.NumberDecimalDigits = numDecimals;

									// if old unit was feet-in-16th or feet-in-8th we were using a mask so we need to get the raw value and remove the mask
									if (oldUnit === 27 || oldUnit === 19) {
										minValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val();
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').unmask();
										maxValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val();
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').unmask();
									}
									else {
										minValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val();
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').removeNumeric();
										maxValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val();
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeNumeric();
									}

									if (minValue !== "") {
										var minRawValue = FMFormatValues.ParseValue(oldUnit, numformatInfo, minValue);
										var convertedMinRawValue = FMConvertEngUnits.Convert(minRawValue, oldUnit, newUnit);
										var newFormattedMinValue = FMFormatValues.FormatValue(newUnit, numformatInfo, convertedMinRawValue);
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val(newFormattedMinValue);
									}

									if (maxValue !== "") {
										var maxRawValue = FMFormatValues.ParseValue(oldUnit, numformatInfo, maxValue);
										var convertedMaxRawValue = FMConvertEngUnits.Convert(maxRawValue, oldUnit, newUnit);
										var newFormattedMaxValue = FMFormatValues.FormatValue(newUnit, numformatInfo, convertedMaxRawValue);
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val(newFormattedMaxValue);
									}

									// add the mask to the editor fields
									// if feet-in-16th or feet-in-8th use 00-00-00 as mask, otherwise is just plain numeric
									if (newUnit === 27) //FML_FtIn16th
									{
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').mask('S99-99-99', {
											translation: {
												'S': {
													pattern: /-/,
													optional: true
												}
											},
											placeholder: '__-__-__'
										});
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').mask('S99-99-99', {
											translation: {
												'S': {
													pattern: /-/,
													optional: true
												}
											},
											placeholder: '__-__-__'
										});
									}
									else if (newUnit === 19) //FML_FtIn8th
									{
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').mask('S99-99-9', {
											translation: {
												'S': {
													pattern: /-/,
													optional: true
												}
											},
											placeholder: '__-__-__'
										});
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').mask('S99-99-9', {
											translation: {
												'S': {
													pattern: /-/,
													optional: true
												}
											},
											placeholder: '__-__-__'
										});
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').attr('placeholder', '');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').attr('placeholder', '');

										var numDecimals = defaultPrecision;

										if (args.column.hasOwnProperty('DecimalPlaces')) {
											if (args.column['DecimalPlaces'] !== -1)
												numDecimals = args.column['DecimalPlaces'];
										}

										if (numDecimals === 0) {
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').numeric({
												decimal: false,
												negative: true
											});
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').numeric({
												decimal: false,
												negative: true
											});
										}
										else {
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').numeric({
												decimal: FMOperateIndex.numformatInfo.NumberDecimalSeparator,
												negative: true,
												decimalPlaces: numDecimals
											});
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').numeric({
												decimal: FMOperateIndex.numformatInfo.NumberDecimalSeparator,
												negative: true,
												decimalPlaces: numDecimals
											});
										}
									}

									// remember the old unit value
									$(this).data('oldValue', this.value);
								});

								// change the operator
								dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitOperator]').on('change', function (event) {
									var operator = $(this).val();
									if (operator === "between" || operator === "not_between") {
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMaxLabel]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeClass('hidden');
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMaxLabel]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val('');
									}
								});

								// change the values
								dataPopover.tip().find('.popover-content').find('[name=numericFilterMin], [name=numericFilterMax]').on('blur', function (event) {
									var numformatInfo = FMOperateIndex.numformatInfo;

									var numDecimals = defaultPrecision;
									if (args.column.hasOwnProperty('DecimalPlaces')) {
										if (args.column['DecimalPlaces'] !== -1)
											numDecimals = args.column['DecimalPlaces'];
									}

									var unit = dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').val();
									if ($(this).val() != "") {
										var newFormattedLevel = '';
										if (unit === '27') { //FML_FtIn16th
											newFormattedLevel = FMOperateIndex.convertFeetInch16thReadings($(this).val());
										}
										else if (unit === '19') { //FML_FtIn8th
											newFormattedLevel = FMOperateIndex.convertFeetInch8thReadings($(this).val());
										}
										else {
											numformatInfo.NumberDecimalDigits = numDecimals;
											var newLevel = FMFormatValues.ParseValue(unit, numformatInfo, $(this).val());
											newFormattedLevel = FMFormatValues.FormatValue(unit, numformatInfo, newLevel);
										}
										$(this).val(newFormattedLevel);
									}
								});


								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=numericFilterPointGroupReset]').on('click', function (event) {
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').removeNumeric();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeNumeric();

									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());

									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();

									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=numericFilterPointGroupCancel]').on('click', function (event) {
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').removeNumeric();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeNumeric();

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=numericFilterPointGroupApply]').on('click', function (event) {
									var numformatInfo = FMOperateIndex.numformatInfo;
									var unit = -1;
									if (!dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').hasClass("hidden")) {
										unit = parseInt(dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').val());
									}
									var minValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val();
									var maxValue = dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val();
									var operator = dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitOperator]').val();

									var missingMinValue = (minValue === "");
									var missingMaxValue = (maxValue === "" && (operator === "between" || operator === "not_between"));
									if (missingMinValue || missingMaxValue) {
										if (missingMinValue) {
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').parent().addClass('has-error');
										}
										if (missingMaxValue) {
											dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').parent().addClass('has-error');
										}
										return false;
									}

									// create a description to show in a tooltip
									var description = dataPopover.tip().find('.popover-content').find('[name=numericFilterUnitOperator] option:selected').text() +
										" " + dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').val();
									if (operator === "between" || operator === "not_between") {
										description += " and " + dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').val();
									}
									if (!dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit]').hasClass("hidden")) {
										description += " (" + dataPopover.tip().find('.popover-content').find('[name=numericFilterUnit] option:selected').text() + ")";
									}
									var filter = {
										type: 'numeric',
										unit: unit,
										operator: operator,
										minValue: minValue !== "" ? FMFormatValues.ParseValue(unit, numformatInfo, minValue).toString() : "",
										maxValue: maxValue !== "" ? FMFormatValues.ParseValue(unit, numformatInfo, maxValue).toString() : "",
										description: encodeURIComponent(description)
									};

									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').unmask();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMin]').removeNumeric();
									dataPopover.tip().find('.popover-content').find('[name=numericFilterMax]').removeNumeric();

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									// copy the filter to all the columns of the same field
									grid.setColumns(grid.getColumns());

									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();

									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});
							}

							// filter boolean column
							var booleanFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#PointGroupFilterBooleanGroupHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								//populate values from the stored filter
								if (args.column.hasOwnProperty('filter')) {
									dataPopover.tip().find('.popover-content').find('[name=PointGroupFilterBoolean][value=' + args.column.filter.Value + ']').prop("checked", true);
								}
								else {
									// default filter to true value
									dataPopover.tip().find('.popover-content').find('[name=PointGroupFilterBoolean][value=true]').prop("checked", true);
								}



								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=booleanFilterPointGroupReset]').on('click', function (event) {

									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=booleanFilterPointGroupCancel]').on('click', function (event) {

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=booleanFilterPointGroupApply]').on('click', function (event) {
									var setValue = dataPopover.tip().find('.popover-content').find('[name=PointGroupFilterBoolean]:checked').val();

									// create a description to show in a tooltip
									var description = "Value is: " + dataPopover.tip().find('.popover-content').find('[name=PointGroupFilterBoolean]:checked').parent().text().trim();

									var filter = {
										type: 'boolean',
										Value: setValue,
										description: encodeURIComponent(description)
									};

									// copy the filter to all the columns of the same field
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});
							}

							// filter string column
							var stringFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#PointGroupFilterStringGroupHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								//populate values from the stored filter
								if (args.column.hasOwnProperty('filter')) {
									dataPopover.tip().find('.popover-content').find('[name=stringFilterPointGroupValue]').val(args.column.filter.Value);
								}
								dataPopover.tip().find('.popover-content').find('[name=stringFilterPointGroupValue]').focus();

								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=stringFilterPointGroupReset]').on('click', function (event) {
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=stringFilterPointGroupCancel]').on('click', function (event) {
									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=stringFilterPointGroupApply]').on('click', function (event) {
									var setValue = dataPopover.tip().find('.popover-content').find('[name=stringFilterPointGroupValue]').val();

									// create a description to show in a tooltip
									var description = "Value contains: " + (setValue === "" ? "Empty" : setValue);

									var filter = {
										type: 'string',
										Value: setValue,
										description: encodeURIComponent(description)
									};

									// copy the filter to all the columns of the same field
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});
							}

							// filter dateTimeOffset column
							var dateTimeOffsetFilter = function (args) {

								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#PointGroupFilterDateTimeOffsetHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').datetimepicker({
									buttonImage: FMLayout.calendarLocation + '/calendar.gif',
									buttonImageOnly: true,
									showOn: "button",
									showTimezone: false,
									useLocalTimezone: false,
									defaultTimezone: $("#datepickerTimezoneString").val(),
									dateFormat: FMLayout.dateFormat,
									timeFormat: FMLayout.timeFormat,
									showSecond: (FMLayout.timeFormat.indexOf('ss') === -1) ? false : true,
									beforeShow: function () {
										setTimeout(function () {
											$('.ui-datepicker').css('z-index', 1100);
										}, 0);
									},
									onSelect: function (d, i) {
										if (d !== i.lastVal) {
											$(this).change();
										}
									}
								});

								dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').datetimepicker({
									buttonImage: FMLayout.calendarLocation + '/calendar.gif',
									buttonImageOnly: true,
									showOn: "button",
									showTimezone: false,
									useLocalTimezone: false,
									defaultTimezone: $("#datepickerTimezoneString").val(),
									dateFormat: FMLayout.dateFormat,
									timeFormat: FMLayout.timeFormat,
									showSecond: (FMLayout.timeFormat.indexOf('ss') === -1) ? false : true,
									beforeShow: function () {
										setTimeout(function () {
											$('.ui-datepicker').css('z-index', 1100);
										}, 0);
									},
									onSelect: function (d, i) {
										if (d !== i.lastVal) {
											$(this).change();
										}
									}
								});

								dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().find('img').addClass('hidden');

								//populate values from the stored filter
								if (args.column.hasOwnProperty('filter')) {
									dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterOperator]').val(args.column.filter.operator);
									if (args.column.filter.operator === "between" || args.column.filter.operator === "not_between") {
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMaxLabel]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().find('img').removeClass('hidden');
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMaxLabel]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().find('img').removeClass('hidden').addClass('hidden');
									}

									if (args.column.filter.minValue !== "") {
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').datetimepicker('setDate', new Date(args.column.filter.minValue));
									}
									if (args.column.filter.maxValue !== "" && (args.column.filter.operator === "between" || args.column.filter.operator === "not_between")) {
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').datetimepicker('setDate', new Date(args.column.filter.maxValue));
									}
								}

								// change the operator
								dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterOperator]').on('change', function (event) {
									var operator = $(this).val();
									if (operator === "between" || operator === "not_between") {
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMaxLabel]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().find('img').removeClass('hidden');

									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMaxLabel]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().find('img').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').val('');
									}
								});

								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterPointGroupReset]').on('click', function (event) {

									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterPointGroupCancel]').on('click', function (event) {

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterPointGroupApply]').on('click', function (event) {
									dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').parent().removeClass('has-error');
									var rawMinValue = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').val();
									// check to see if we have a valid date entered
									try {
										var validateMinDate = $.datepicker.parseDateTime(FMLayout.dateFormat, FMLayout.timeFormat, rawMinValue, {}, {});
									}
									catch (e) {
										dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').parent().addClass('has-error');
										return;
									}
									var operator = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterOperator]').val();

									var minValue = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMin]').datepicker('getDate');
									var minTime = { hour: minValue.getHours(), minute: minValue.getMinutes(), second: minValue.getSeconds(), timezone: minValue.getTimezoneOffset() }
									var formattedMinDateTime = $.datepicker.formatDate(FMLayout.dateFormat, minValue) + ' ' + $.datepicker.formatTime(FMLayout.timeFormat, minTime);

									var maxValue = "";
									var formattedMaxDateTime = "";
									if (operator === "between" || operator === "not_between") {
										var rawMaxValue = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').val();
										try {
											var validateMaxDate = $.datepicker.parseDateTime(FMLayout.dateFormat, FMLayout.timeFormat, rawMaxValue, {}, {});
										}
										catch (e) {
											dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').parent().addClass('has-error');
											return;
										}
										maxValue = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterMax]').datepicker('getDate');
										var maxTime = { hour: maxValue.getHours(), minute: maxValue.getMinutes(), second: maxValue.getSeconds(), timezone: maxValue.getTimezoneOffset() }

										formattedMaxDateTime = $.datepicker.formatDate(FMLayout.dateFormat, maxValue) + ' ' + $.datepicker.formatTime(FMLayout.timeFormat, maxTime);
									}
									// create a description to show in a tooltip
									var description = dataPopover.tip().find('.popover-content').find('[name=dateTimeOffsetFilterOperator] option:selected').text() +
										" " + formattedMinDateTime;
									if (operator === "between" || operator === "not_between") {
										description += " and " + formattedMaxDateTime;
									}

									var filter = {
										type: 'datetimeoffset',
										operator: operator,
										minValue: minValue !== "" ? minValue.toISOString() : "",
										maxValue: maxValue !== "" ? maxValue.toISOString() : "",
										description: encodeURIComponent(description)
									};

									// copy the filter to all the columns of the same field
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});
							}

							// filter timeSpan column
							var timeSpanFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#PointGroupFilterTimeSpanHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMin]').mask('###.00:00:00', { reverse: true, placeholder: "__:__:__" });
								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').mask('###.00:00:00', { reverse: true, placeholder: "__:__:__" });

								//populate values from the stored filter
								if (args.column.hasOwnProperty('filter')) {
									dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterOperator]').val(args.column.filter.operator);
									if (args.column.filter.operator === "between" || args.column.filter.operator === "not_between") {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMaxLabel]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').removeClass('hidden');
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMaxLabel]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').removeClass('hidden').addClass('hidden');
									}

									var formatMinValue = args.column.filter.minValue.days + "." + args.column.filter.minValue.hours + ":" + args.column.filter.minValue.minutes + ":" + args.column.filter.minValue.seconds;
									dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMin]').val(formatMinValue);

									if (args.column.filter.maxValue != null && args.column.filter.minValue.days != 0 && args.column.filter.minValue.hours != 0 && args.column.filter.minValue.minutes != 0 && args.column.filter.minValue.seconds != 0) {
										var formatMaxValue = args.column.filter.maxValue.days + "." + args.column.filter.maxValue.hours + ":" + args.column.filter.maxValue.minutes + ":" + args.column.filter.maxValue.seconds;
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').val(formatMaxValue);
									}
								}

								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMin]').focus();

								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterPointGroupReset]').on('click', function (event) {

									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterPointGroupCancel]').on('click', function (event) {

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterPointGroupApply]').on('click', function (event) {
									var minRawValue = dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMin]').val();
									var maxRawValue = dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').val();
									var operator = dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterOperator]').val();

									var minValue = validateTimeSpan(minRawValue);
									var maxValue = validateTimeSpan(maxRawValue);

									if (minValue == null) {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMin]').parent().addClass('has-error');
										return false;
									}
									if (maxValue == null && (operator === "between" || operator === "not_between")) {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').parent().addClass('has-error');
										return false;
									}

									// create a description to show in a tooltip
									var description = dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterOperator] option:selected').text() +
										" " + (minValue.days + "." + minValue.hours + ":" + minValue.minutes + ":" + minValue.seconds);
									if (operator === "between" || operator === "not_between") {
										description += " and " + (maxValue.days + "." + maxValue.hours + ":" + maxValue.minutes + ":" + maxValue.seconds);
									}

									var filter = {
										type: 'timespan',
										operator: operator,
										minValue: minValue,
										maxValue: maxValue,
										description: encodeURIComponent(description)
									};

									// copy the filter to all the columns of the same field
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});

								// change the operator
								dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterOperator]').on('change', function (event) {
									var operator = $(this).val();
									if (operator === "between" || operator === "not_between") {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMaxLabel]').removeClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').removeClass('hidden');
									}
									else {
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMaxLabel]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').removeClass('hidden').addClass('hidden');
										dataPopover.tip().find('.popover-content').find('[name=timeSpanFilterMax]').val('');
									}
								});

							}

							// filter for enumerated columns
							var enumFilter = function (args) {
								var popover = $(args.headercell).popover("destroy").popover({
									container: 'body',
									placement: 'bottom',
									html: true,
									content: $('#PointGroupFilterEnumGroupHeader').html(),
									trigger: "manual"
								});
								var dataPopover = popover.data('bs.popover');
								$(args.headercell).popover('show');
								$("#customModalBackground").removeClass("hidden");

								dataPopover.tip().find("[name=pointFilterEnumTagValues]").select2({
									multiple: true,
									placeholder: "Loading..."
								});

								loadOptionsForEnumValueType(dataPopover.tip().find('.popover-content').find('[name=pointFilterEnumTagValues]'), args.column);

								// remove filter
								dataPopover.tip().find('.popover-content').find('[name=enumFilterPointGroupReset]').on('click', function (event) {
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											delete elem.filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});

								// close the pop over when clicking cancel
								dataPopover.tip().find('.popover-content').find('[name=enumFilterPointGroupCancel]').on('click', function (event) {
									dataPopover.tip().find('.popover-content').find('[name=pointFilterEnumTagValues]').select2("destroy");
									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");
									event.stopPropagation();
								});

								// Apply filter
								dataPopover.tip().find('.popover-content').find('[name=enumFilterPointGroupApply]').on('click', function (event) {

									var setValue = $(dataPopover.tip().find('.popover-content').find('[name=pointFilterEnumTagValues]')).val();
									if (setValue === null) {
										//---
										dataPopover.tip().find('.popover-content').find('[name=pointFilterEnumTagValues]').parent().addClass('has-error');
										return false;
									}

									// create a description to show in a tooltip
									var description = "Value in: " + (setValue === "" ? "Empty" : setValue.join());

									var filter = {
										type: 'enum',
										Value: setValue,
										description: encodeURIComponent(description)
									};

									// copy the filter to all the columns of the same field
									$.map(args.grid.getColumns(), function (elem, idx) {
										if (args.column.field === elem.field) {
											elem.filter = filter;
										}
									});

									$(args.headercell).popover('destroy');
									$("#customModalBackground").removeClass("hidden").addClass("hidden");

									grid.setColumns(grid.getColumns());
									grid.getData().refresh();
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, grid);
									grid.invalidateAllRows();
									grid.render();
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
									event.stopPropagation();
								});
							}

							var defaultNumericInputMaskForFilter = function (unit, popupContainer, numberDecimalSeparator, numDecimals) {
								$(popupContainer).find('[name=numericFilterMin]').unmask();
								$(popupContainer).find('[name=numericFilterMax]').unmask();
								$(popupContainer).find('[name=numericFilterMin]').removeNumeric();
								$(popupContainer).find('[name=numericFilterMax]').removeNumeric();

								// set the mask for how to enter values
								if (unit === 27) //FML_FtIn16th
								{
									$(popupContainer).find('[name=numericFilterMin]').mask('S99-99-99', {
										translation: {
											'S': {
												pattern: /-/,
												optional: true
											}
										},
										placeholder: '__-__-__'
									});
									$(popupContainer).find('[name=numericFilterMax]').mask('S99-99-99', {
										translation: {
											'S': {
												pattern: /-/,
												optional: true
											}
										},
										placeholder: '__-__-__'
									});
								}
								else if (unit === 19) //FML_FtIn8th
								{
									$(popupContainer).find('[name=numericFilterMin]').mask('S99-99-9', {
										translation: {
											'S': {
												pattern: /-/,
												optional: true
											}
										},
										placeholder: '__-__-__'
									});
									$(popupContainer).find('[name=numericFilterMax]').mask('S99-99-9', {
										translation: {
											'S': {
												pattern: /-/,
												optional: true
											}
										},
										placeholder: '__-__-__'
									});
								}
								else {
									$(popupContainer).find('[name=numericFilterMin]').attr('placeholder', '');
									$(popupContainer).find('[name=numericFilterMax]').attr('placeholder', '');

									if (numDecimals === 0) {
										$(popupContainer).find('[name=numericFilterMin]').numeric({
											decimal: false,
											negative: true
										});
										$(popupContainer).find('[name=numericFilterMax]').numeric({
											decimal: false,
											negative: true
										});
									}
									else {
										$(popupContainer).find('[name=numericFilterMin]').numeric({
											decimal: FMOperateIndex.numformatInfo.NumberDecimalSeparator,
											negative: true,
											decimalPlaces: numDecimals
										});
										$(popupContainer).find('[name=numericFilterMax]').numeric({
											decimal: FMOperateIndex.numformatInfo.NumberDecimalSeparator,
											negative: true,
											decimalPlaces: numDecimals
										});
									}
								}
							}

							var loadUnitsByUnitType = function (container, column) {
								var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $("#pointgroup" + newId) };

								// get the different engineering unit types that we have for the points selected in the grid
								var unitTypes = FMOperateIndex.staticPointGroupControllers[newId].getNumericUnitsForTag(column.field);
								unitTypes = unitTypes[0];

								$.ajax({
									type: 'Get',
									url: 'GetUnitsByUnitType',
									dataType: "json",
									data: { "unitType": unitTypes },
									cache: false,
									success: function (response) {
										var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };
										FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
											if (inError) {
												//--- We need to display some type of message
												return;
											}
											$(container).html('');

											var columnUnit = -1;
											if (column.Unit) {
												columnUnit = column.Unit;
											}
											// sort the units alphabetically
											data = data.sort(function (a, b) {
												return a.UnitAbbreviation.localeCompare(b.UnitAbbreviation);
											});


											for (var i = 0; i < data.length; i++) {
												var uomtemplate = '';
												if ($(container).is('select')) {
													uomtemplate = '<option value="' + data[i].Unit + '">' + data[i].UnitAbbreviation + '</option>';
												}
												else {
													var uomEventHandler = "$(this).parent().find('.list-group-item').removeClass('active');$(this).addClass('active'); $('input[name=changeUnitDefaultToPoint]').prop('checked', false); ";
													uomtemplate = '<a href="#" class="list-group-item' + (columnUnit == data[i].Unit ? ' active' : '') + ' " data-value="' + data[i].Unit + '" onclick="' + uomEventHandler + '" title="' + data[i].UnitDescription + '">' + data[i].UnitAbbreviation + '</a>';
												}
												$(container).append(uomtemplate);
											}

											if (!$(container).is('select')) {
												if (columnUnit === -1) {
													$(container).closest('.popover-content').find('[name=changeUnitDefaultToPoint]').prop('checked', true);
												}
												else {
													$(container).closest('.popover-content').find('[name=changeUnitDefaultToPoint]').prop('checked', false);
												}

												$(container).niceScroll({ cursorwidth: '10px', autohidemode: true, cursorcolor: '#486899', background: 'transparent', horizrailenabled: false });

											}
											else {
												var numformatInfo = FMOperateIndex.numformatInfo;

												var numDecimals = 2;
												if (column.hasOwnProperty('DecimalPlaces')) {
													if (column['DecimalPlaces'] !== -1)
														numDecimals = column['DecimalPlaces'];
												}
												numformatInfo.NumberDecimalDigits = numDecimals;

												if (column.hasOwnProperty('filter')) {
													$(container).val(column.filter.unit);

													defaultNumericInputMaskForFilter(column.filter.unit, $(container).parent().find('.popover-content'), FMOperateIndex.numformatInfo.NumberDecimalSeparator, numDecimals);

													$(container).parent().find('[name=numericFilterMin]').val(FMFormatValues.FormatValue(column.filter.unit, numformatInfo, column.filter.minValue));
													if (column.filter.maxValue !== "") {
														$(container).parent().find('[name=numericFilterMax]').val(FMFormatValues.FormatValue(column.filter.unit, numformatInfo, column.filter.maxValue));
													}
												}
												else {
													if (columnUnit !== -1) {
														$(container).val(columnUnit);
													}

													// set the mask based on the default unit
													defaultNumericInputMaskForFilter(parseInt($(container).val()), $(container).parent(), FMOperateIndex.numformatInfo.NumberDecimalSeparator, numDecimals);

												}
											}
										}, messageAttributes);
									},
									error: function (xhr, ajaxOptions, thrownError) {
										FMErrorAndExceptionHandling.ShowError(thrownError);
									}
								});
							}

							var loadOptionsForEnumValueType = function (container, column) {
								var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $("#pointgroup" + newId) };

								// get the different value types that we have for the points selected in the grid
								var valueTypes = FMOperateIndex.staticPointGroupControllers[newId].getValueTypesForTag(column.field);
								valueTypes = valueTypes[0];

								$.ajax({
									type: 'Get',
									url: 'GetOptionsForEnumValueType',
									dataType: "json",
									data: { "valueType": valueTypes },
									cache: false,
									success: function (response) {
										var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };
										FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
											if (inError) {
												//--- We need to display some type of message
												return;
											}
											$(container).html('');

											for (var i = 0; i < data.length; i++) {
												var optiontemplate = '<option value="' + data[i].Text + '">' + data[i].Text + '</option>';
												$(container).append(optiontemplate);
											}

											if (column.hasOwnProperty('filter')) {
												$(container).val(column.filter.Value);
											}
											$(container).select2();

										}, messageAttributes);
									},
									error: function (xhr, ajaxOptions, thrownError) {
										FMErrorAndExceptionHandling.ShowError(thrownError);
									}
								});
							}

							var validateTimeSpan = function (newValue) {

								// make sure that the value is valid
								if (newValue !== "") {
									var days = 0;
									var hours = 0;
									var minutes = 0;
									var seconds = 0;
									var dateSeparator = newValue.split(".");
									var timePart = "";
									if (dateSeparator.length === 1) {
										timePart = dateSeparator[0];
									}
									else {
										days = parseInt(dateSeparator[0]);
										timePart = dateSeparator[1];
									}

									var timeParts = timePart.split(":");

									if (timeParts.length > 0) {
										hours = parseInt(timeParts[0]);
									}
									if (timeParts.length > 1) {
										minutes = parseInt(timeParts[1]);
									}
									if (timeParts.length > 2) {
										seconds = parseInt(timeParts[2]);
									}
									if (hours > 24 || minutes > 60 || seconds > 60) {
										return null;
									}
									else if (hours >= 24 && minutes > 0 && seconds > 0) {
										return null;
									}
									return { days: days, hours: hours, minutes: minutes, seconds: seconds }
								}
								return null;
							}

							grid.registerPlugin(headerMenuPlugin);

							/*--------------- END COLUMN MENU  -----------------*/

							/*--------------- ROW MENU  -----------------*/

							var cellMenuPlugin = new Slick.Plugins.CellContextMenu({});

							cellMenuPlugin.onBeforeMenuShow.subscribe(function (e, args) {
								var isDynamicPointGroup = args.grid.getColumns()[0].hasOwnProperty('filter');

								args.menu.items[0].items[0].disabled = false;
								args.menu.items[0].items[1].disabled = false;
								args.menu.items[0].items[2].disabled = false;
								args.menu.items[0].disabled = false;
								args.menu.items[1].disabled = false;
								args.menu.items[3].disabled = false;

								// disable based on rights
								if ($('#ModifyPointGroupsRight').val() == 'False') {
									args.menu.items[0].disabled = true;
									args.menu.items[1].disabled = true;
									args.menu.items[2].disabled = true
								}

								// disable add totals if there is already a total in the grid
								var dataView = args.grid.getData();
								if (args.row.type === "point") {
									args.menu.items[3].hidden = false;
								}
								else {
									args.menu.items[3].hidden = true;
								}
								var _rows = dataView.getItems();
								var foundTotal = $.grep(_rows, function (row) {
									return (row.type && row.type === "total");
								});
								if (foundTotal.length > 0) {
									args.menu.items[0].items[2].disabled = true;
								}
								// show/hide the menu option to display the calculation in the total/subtotal rows
								args.menu.items[2].hidden = true;
								if ((args.row.type && (args.row.type === 'subtotal' || args.row.type === 'total')) && args.column.field !== 'point') {
									args.menu.items[2].hidden = false;

									// add the check mark to next to the current for the calculation
									if (!args.column.totalizerConfig || !args.column.totalizerConfig[args.row.totalizerGuid] || args.column.totalizerConfig[args.row.totalizerGuid] === 'none') {
										args.menu.items[2].items[0].iconCssClass = "glyphicon glyphicon-ok";
									}
									else {
										args.menu.items[2].items[0].iconCssClass = "glyphicon";
									}

									if (args.column.totalizerConfig && args.column.totalizerConfig[args.row.totalizerGuid] && args.column.totalizerConfig[args.row.totalizerGuid] === 'sum') {
										args.menu.items[2].items[1].iconCssClass = "glyphicon glyphicon-ok";
									}
									else {
										args.menu.items[2].items[1].iconCssClass = "glyphicon";
									}

									if (args.column.totalizerConfig && args.column.totalizerConfig[args.row.totalizerGuid] && args.column.totalizerConfig[args.row.totalizerGuid] === 'avg') {
										args.menu.items[2].items[2].iconCssClass = "glyphicon glyphicon-ok";
									}
									else {
										args.menu.items[2].items[2].iconCssClass = "glyphicon";
									}

									if (args.column.totalizerConfig && args.column.totalizerConfig[args.row.totalizerGuid] && args.column.totalizerConfig[args.row.totalizerGuid] === 'max') {
										args.menu.items[2].items[3].iconCssClass = "glyphicon glyphicon-ok";
									}
									else {
										args.menu.items[2].items[3].iconCssClass = "glyphicon";
									}

									if (args.column.totalizerConfig && args.column.totalizerConfig[args.row.totalizerGuid] && args.column.totalizerConfig[args.row.totalizerGuid] === 'min') {
										args.menu.items[2].items[4].iconCssClass = "glyphicon glyphicon-ok";
									}
									else {
										args.menu.items[2].items[4].iconCssClass = "glyphicon";
									}
								}

								// if dynamic point group we cannot add subtotals, or points
								if (isDynamicPointGroup) {
									args.menu.items[0].items[0].disabled = true; // add point
									args.menu.items[0].items[1].disabled = true; // add subtotal
									args.menu.items[0].items[3].disabled = true; // add empty row
									if (args.menu.items[0].items[2].disabled === true) // if cannot add total we can disable the whole submenu
									{
										args.menu.items[0].disabled = true;
									}
									else {
										args.menu.items[0].disabled = false;
									}

									// cannot delete points in a dynamic group
									if (args.row.type && args.row.type === 'point') {
										args.menu.items[1].disabled = true; // delete point
									}
								}

								e.preventDefault();
							});

							cellMenuPlugin.onCommand.subscribe(function (e, args) {
								var _grid = args.grid;
								var rowNumber = args.cellClicked.row;

								if (!args.column.totalizerConfig) {
									args.column.totalizerConfig = {};
								}

								if (args.command === 'insert-point') {
									selectrow(rowNumber);
								}

								else if (args.command === 'insert-total') {
									var newRowDefinition = { id: FMOperateIndex.newGuid(), type: "total", point: "Total", totalizerGuid: FMOperateIndex.newGuid() };
									var dataview = _grid.getData();
									var id = dataview.getItem(rowNumber).id;
									dataView.insertItem(dataview.getIdxById(id), newRowDefinition);
									_grid.scrollRowIntoView(0, false);

									FMOperateIndex.PersistPointGroup(activeTab, newId, _grid);
								}
								else if (args.command === 'insert-subtotal') {
									var newRowDefinition = { id: FMOperateIndex.newGuid(), type: "subtotal", point: "Subtotal", totalizerGuid: FMOperateIndex.newGuid() };
									var dataview = _grid.getData();
									var id = dataview.getItem(rowNumber).id;
									dataView.insertItem(dataview.getIdxById(id), newRowDefinition);
									_grid.scrollRowIntoView(0, false);

									FMOperateIndex.PersistPointGroup(activeTab, newId, _grid);
								}
								else if (args.command === 'insert-emptyrow') {
									var newRowDefinition = { id: FMOperateIndex.newGuid(), type: "empty" };
									var dataview = _grid.getData();
									var id = dataview.getItem(rowNumber).id;
									dataView.insertItem(dataview.getIdxById(id), newRowDefinition);
									_grid.scrollRowIntoView(0, false);

									FMOperateIndex.PersistPointGroup(activeTab, newId, _grid);
								}
								else if (args.command === 'remove-row') {
									var deleteColumnMetadata = false;
									var totalizerGuid = '';
									if (args.row.type === "subtotal" || args.row.type === "total") {
										deleteColumnMetadata = true;
										totalizerGuid = args.row.totalizerGuid;
									}
									FMLayout.ConfirmYesNo("Are you sure you want to remove the selected row?", "Remove Row", function () {
										// delete column configuration for totalizer rows
										if (deleteColumnMetadata) {
											var _columns = _grid.getColumns();
											_columns = $.map(_columns, function (elem, idx) {
												if (elem.totalizerConfig) {
													delete elem.totalizerConfig[totalizerGuid];
												}
												if (elem.totalizerValue) {
													delete elem.totalizerValue[totalizerGuid];
												}
											});
										}
										var dataView = _grid.getData();
										shadowGrid.deleteRow(dataView.getItem(rowNumber));
										var id = dataView.getItem(rowNumber).id;
										dataView.deleteItem(id);
										// if we don't have any more rows displayed we need to add an empty row
										if (dataView.getFilteredItems().length === 0) {
											var newRowDefinition = { id: FMOperateIndex.newGuid(), type: "empty" };
											dataView.addItem(newRowDefinition);
											dataView.refresh();
										}
										_grid.scrollRowIntoView(rowNumber - 1);
										FMOperateIndex.PersistPointGroup(activeTab, newId, _grid);
									});
								}
								else if (args.command === 'totalizer-sum') {
									var rowGuid = args.row.totalizerGuid;
									args.column.totalizerConfig[rowGuid] = 'sum';
									FMOperateIndex.PersistPointGroup(activeTab, newId, _grid);
									// update totals							
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, _grid);
									_grid.invalidateAllRows();
									_grid.render();
								}
								else if (args.command === 'totalizer-avg') {
									var rowGuid = args.row.totalizerGuid;
									args.column.totalizerConfig[rowGuid] = 'avg';
									FMOperateIndex.PersistPointGroup(activeTab, newId, _grid);
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, _grid);
									_grid.invalidateAllRows();
									_grid.render();
								}
								else if (args.command === 'totalizer-max') {
									var rowGuid = args.row.totalizerGuid;
									args.column.totalizerConfig[rowGuid] = 'max';
									FMOperateIndex.PersistPointGroup(activeTab, newId, _grid);
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, _grid);
									_grid.invalidateAllRows();
									_grid.render();
								}
								else if (args.command === 'totalizer-min') {
									var rowGuid = args.row.totalizerGuid;
									args.column.totalizerConfig[rowGuid] = 'min';
									FMOperateIndex.PersistPointGroup(activeTab, newId, _grid);
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, _grid);
									_grid.invalidateAllRows();
									_grid.render();
								}
								else if (args.command === 'totalizer-none') {
									var rowGuid = args.row.totalizerGuid;
									args.column.totalizerConfig[rowGuid] = 'none';
									FMOperateIndex.PersistPointGroup(activeTab, newId, _grid);
									// update totals
									var metadata = FMOperateIndex.staticPointGroupControllers[newId].getMetadata();
									FMPointGroupGrid.updateGridTotals(metadata, _grid);
									_grid.invalidateAllRows();
									_grid.render();
								}
								else if (args.command === 'open-pointdetail') {
									var openPointDetailDataView = _grid.getData();
									var openPointDetailRow = openPointDetailDataView.getItem(rowNumber);
									FMOperateIndex.openPoint(openPointDetailRow.point, openPointDetailRow.pointguid);
								}
							});

							grid.registerPlugin(cellMenuPlugin);


							var selectrow = function (rowNumber) {
								// create the backdrop and wait for next modal to be triggered
								$('body').modalmanager('loading');

								$("#PointGroupSelectionModalBody").html('<div id="pointGroupModalMenuLoader" class="LoadingAnimation transparent"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>');
								$("#PointGroupSelectionModal").modal("show");

								$.ajax({
									type: 'Get',
									url: 'GetListOfPointsPartialView',
									dataType: "html",
									data: { "parentControl": "#PointGroupSelectionModalBody", "persistChanges": false },
									cache: false,
									success: function (view) {
										$("#PointGroupSelectionModalBody").html(view);

										$('#PointGroupSelectionModalBody .operateSubMenuList').css("height", $('.operateSubMenuList').parent().height());
										$('#PointGroupSelectionModalBody .operateSubMenuList').uncolumnize();

										// remove any points that are already in use
										$("#PointGroupSelectionModalBody .operateSubMenuElement").each(function () {
											var theGridData = grid.getData();

											for (var itemIndex = 0; itemIndex < theGridData.getLength(); itemIndex++) {
												if ($(this).attr('data-guid') === theGridData.getItem(itemIndex).pointguid) {
													$(this).addClass('hidden');
													break;
												}
											}
										});

										// we need to remove the onclick event since by default it will open a new point when clicked and instead we need to add a 'selected' class
										$("#PointGroupSelectionModalBody .operateSubMenuElement").each(function () {
											$(this).attr('onclick', "$(this).hasClass('selected') ? $(this).removeClass('selected'): $(this).addClass('selected')");
										});

										$("#PointGroupSelectionModalBody .operateSubMenuElement").each(function () {
											$(this).attr('ondblclick', "$(this).removeClass('selected').addClass('selected'); FMOperateIndex.PointGroupSelectionModalSelectButton();");
										});

										$('#PointGroupSelectionModalBody .operateSubMenuList').columnize({
											columns: 2,
											buildOnce: true,
											cssClassPrefix: "points",
											lastNeverTallest: true
										});
										$("#PointGroupSelectionModalBody .operateSubMenuList").niceScroll({ cursorwidth: '10px', horizrailenabled: false, autohidemode: false, cursorcolor: "#486899", background: "white" });

										// override the code executed on the Select Button of the selection modal to deal with new tags
										FMOperateIndex.PointGroupSelectionModalSelectButton = function () {
											if ($('.operateSubMenuElement.selected').length === 0) {
												FMLayout.Alert("No Point selected.");
											}
											else {
												$('.operateSubMenuElement.selected').sort(function (a, b) {  // sort in reverse since we are inserting in the same position
													return $(b).attr('data-name').toUpperCase().localeCompare($(a).attr('data-name').toUpperCase());
												}).each(function (index) {
													var newPoint = $(this).attr('data-name');
													var newPointGuid = $(this).attr('data-guid');

													var newRowDefinition = { id: FMOperateIndex.newGuid(), type: "point", point: newPoint, pointguid: newPointGuid };
													var dataview = grid.getData();
													var id = dataview.getItem(rowNumber).id;
													dataView.insertItem(dataview.getIdxById(id), newRowDefinition);
													grid.resizeCanvas();
													grid.scrollRowIntoView(0, false);

													shadowGrid.addRow(newRowDefinition);
												});

												$("#PointGroupSelectionModal").modal("hide");
												FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
											}
										}

										FMOperateIndex.GetPointGroupPointNames = function () {
											var theGridData = grid.getData();
											var thePointNames = new Array();

											for (var itemIndex = 0; itemIndex < theGridData.getLength(); itemIndex++) {
												if (theGridData.getItem(itemIndex).type === "point") {
													thePointNames.push(theGridData.getItem(itemIndex).point);
												}
											}

											return thePointNames;
										}
									},
									error: function (xhr, ajaxOptions, thrownError) {
										FMErrorAndExceptionHandling.ShowError(thrownError);
										$("#PointGroupSelectionModal").modal("hide");
									}
								});
							};

							var insertPointPropertyColumn = function (args, columnID, columnHeader) {
								var newColumn = columnID;
								var newColumnName = columnHeader;
								var newid = newColumn.replace(/ /g, '') + new Date().getTime().toString(); // generate a unique id (in case there are multiple columns for the same tag)
								var fontSize = grid.getOptions().fontSize;
								var columnDefinition = { id: newid, name: newColumnName, field: newColumn, headerCssClass: "text-center grid-font-" + fontSize, cssClass: "grid-font-" + fontSize, formatter: staticPointGroupFormatter };
								columnDefinition.header = {
									menu: { items: FMOperateIndex.PointGroupCreateHeaderMenu() }
								};

								// add the filter if already defined for a column for the same field
								$.each(grid.getColumns(), function (index, columnElem) {

									if (columnElem.field === newColumn) {
										columnDefinition.filter = columnElem.filter;
									}
								});

								var columns = grid.getColumns().slice(0);
								// insert the new column in the middle somewhere
								var pos = columns.map(function (e) {
									return e.id;
								}).indexOf(args.column.id);

								// if we are inserting in the Point Name put the column next to it, otherwise create it in the place the user clicked
								if (pos === 0) {
									pos++;
								}

								columns.splice(pos, 0, columnDefinition);

								grid.setColumns(columns);

								// update the filter parameters for the dataview
								FMOperateIndex.updateFilterParameters(grid, FMOperateIndex.staticPointGroupControllers[newId].getMetadata());
								grid.getData().refresh();

								grid.resizeCanvas();
								// resize the newly created column to fit the column name (doing this by double clicking on the resize handle of the header)
								var newCreatedColumn = $(grid.getContainerNode()).find(".slick-header-column")[pos];

								FMOperateIndex.pointGroupSaveOnColumnResize = false;
								$(newCreatedColumn).find('.slick-resizable-handle').dblclick();
								FMOperateIndex.pointGroupSaveOnColumnResize = true;

								shadowGrid.addColumn(columnDefinition);

								FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
							}


							var selectTagColumn = function (args) {
								// create the backdrop and wait for next modal to be triggered
								$('body').modalmanager('loading');

								$("#PointGroupSelectionModalBody").html('<div id="pointGroupModalMenuLoader" class="LoadingAnimation transparent"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>');
								$("#PointGroupSelectionModal").modal("show");

								// put messages on the actual tab
								var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $("#pointgroup" + newId) };

								$.ajax({
									type: 'Get',
									url: 'GetListOfTagNamesPartialView',
									dataType: "json",
									cache: false,
									success: function (response) {
										var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab, width: '450px' };

										FMErrorAndExceptionHandling.HandleMessages(response, function (view, inError) {
											if (inError) {
												$("#PointGroupSelectionModal").modal("hide");
												return;
											}
											$("#PointGroupSelectionModalBody").html(view);
											$('#PointGroupSelectionModalBody .operateSubMenuList').css("height", $('.operateSubMenuList').parent().height());

											$("#PointGroupSelectionModalBody .operateTagsSubMenuElement").each(function () {
												$(this).attr('ondblclick', "$(this).removeClass('selected').addClass('selected'); FMOperateIndex.PointGroupSelectionModalSelectButton();");
											});

											$('#PointGroupSelectionModalBody .operateSubMenuList').uncolumnize();
											$('#PointGroupSelectionModalBody .operateSubMenuList').columnize({
												columns: 2,
												buildOnce: true,
												cssClassPrefix: "points",
												lastNeverTallest: true
											});

											$("#PointGroupSelectionModalBody .operateSubMenuList").niceScroll({ cursorwidth: '10px', horizrailenabled: false, autohidemode: false, cursorcolor: "#486899", background: "white" });

											// override the code executed on the Select Button of the selection modal to deal with new tags
											FMOperateIndex.PointGroupSelectionModalSelectButton = function () {
												if ($('.operateTagsSubMenuElement.selected').length === 0) {
													FMLayout.Alert("No Tag selected.");
												}
												else {
													$('.operateTagsSubMenuElement.selected').sort(function (a, b) { // sort in reverse since we are inserting in the same position
														return $(a).attr('data-name').toUpperCase().localeCompare($(b).attr('data-name').toUpperCase());
													}).each(function (index) {
														var newColumn = $(this).attr('data-name');
														var newid = newColumn.replace(/ /g, '') + new Date().getTime().toString(); // generate a unique id (in case there are multiple columns for the same tag)
														var fontSize = grid.getOptions().fontSize;
														var columnDefinition = { id: newid, name: newColumn, field: newColumn, headerCssClass: "text-center grid-font-" + fontSize, cssClass: "grid-font-" + fontSize, formatter: staticPointGroupFormatter, DecimalPlaces: -1 };
														columnDefinition.header = {
															menu: { items: FMOperateIndex.PointGroupCreateHeaderMenu() }
														};

														// add the filter if already defined for a column for the same field
														$.each(grid.getColumns(), function (index, columnElem) {
															if (columnElem.field === newColumn) {
																columnDefinition.filter = columnElem.filter;
															}
														});

														var columns = grid.getColumns().slice(0);
														// insert the new column in the middle somewhere
														var pos = columns.map(function (e) {
															return e.id;
														}).indexOf(args.column.id);

														// if we are inserting in the Point Name put the column next to it, otherwise create it in the place the user clicked
														if (pos === 0) {
															pos++;
														}

														columns.splice(pos, 0, columnDefinition);

														grid.setColumns(columns);
														// update the filter parameters for the dataview
														FMOperateIndex.updateFilterParameters(grid, FMOperateIndex.staticPointGroupControllers[newId].getMetadata());
														grid.getData().refresh();

														grid.resizeCanvas();
														// resize the newly created column to fit the column name (doing this by double clicking on the resize handle of the header)
														var newCreatedColumn = $(grid.getContainerNode()).find(".slick-header-column")[pos];

														FMOperateIndex.pointGroupSaveOnColumnResize = false;
														$(newCreatedColumn).find('.slick-resizable-handle').dblclick();
														FMOperateIndex.pointGroupSaveOnColumnResize = true;

														shadowGrid.addColumn(columnDefinition);
													});
													$("#PointGroupSelectionModal").modal("hide");
													FMOperateIndex.PersistPointGroup(activeTab, newId, grid);

												}
											}
										}, messageAttributes);
									},
									error: function (xhr, ajaxOptions, thrownError) {
										FMErrorAndExceptionHandling.ShowError(thrownError);
										$("#PointGroupSelectionModal").modal("hide");
									}
								});
							};

							var insertEmptyColumn = function (args) {
								var newColumn = "empty";
								var newColumnName = "";
								var newid = newColumn.replace(/ /g, '') + new Date().getTime().toString(); // generate a unique id (in case there are multiple columns for the same tag)
								var fontSize = grid.getOptions().fontSize;
								var columnDefinition = { id: newid, name: newColumnName, field: newColumn, headerCssClass: "text-center grid-font-" + fontSize, cssClass: "grid-font-" + fontSize, formatter: staticPointGroupFormatter };
								columnDefinition.header = {
									menu: { items: FMOperateIndex.PointGroupCreateHeaderMenu() }
								};

								// add the filter if already defined for a column for the same field
								$.each(grid.getColumns(), function (index, columnElem) {

									if (columnElem.field === newColumn) {
										columnDefinition.filter = columnElem.filter;
									}
								});

								var columns = grid.getColumns().slice(0);
								// insert the new column in the middle somewhere
								var pos = columns.map(function (e) {
									return e.id;
								}).indexOf(args.column.id);

								// if we are inserting in the Point Name put the column next to it, otherwise create it in the place the user clicked
								if (pos === 0) {
									pos++;
								}

								columns.splice(pos, 0, columnDefinition);

								grid.setColumns(columns);

								grid.getData().refresh();

								grid.resizeCanvas();

								FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
							}

							/*--------------- END ROW MENU  -----------------*/

							/*--------------- AUTO SIZE COLUMNS -----------------*/
							var columnSizePlugin = new Slick.AutoColumnSize();
							grid.onColumnsResized.subscribe(function (e, data) {
								if (FMOperateIndex.pointGroupSaveOnColumnResize) {
									FMOperateIndex.PersistPointGroup(activeTab, newId, grid);
								}
							});

							grid.registerPlugin(columnSizePlugin);
							/*--------------- END AUTO SIZE COLUMNS  -----------------*/

						}

						// done with the process of restoring the tab
						FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
						// if we are not in the process of restoring the persisten state then open new groups so we can rename them
						if (FMOperateIndex.restoringView === false && isNewpointGroup) {
							setTimeout(function () {
								FMOperateIndex.RenameTab($('a[data-target="#' + newId + '"]'));
							}, 1);
						}

						// force a resize of the grid when resizing the window
						$(window).resize(function () {
							grid.resizeCanvas();
						});

					}
				},
				messageAttributes);
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		},
		error: function (xhr, textStatus, error) {
			var activeDrawing = this.activeDrawing;
			var activeTab = this.activeTab;
			var newId = this.newId;

			// need to make  sure that the error we are getting is because we close the page before getting the response
			if (xhr.status != 0) {
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
						$("#loaderpointgroup" + newId).remove();
					});
			}
			// done reloading the tab
			FMOperateIndex.restoringScreenQueueInProgress[newId] = false;
		}
	});

	return newId;
};

// update point group settings 
FMOperateIndex.updatePointGroupSettings = function (parentControl, pointGroupControllerId, parentGroupTab) {
	var popover = $(parentControl).popover("destroy").popover({
		container: 'body',
		placement: 'bottom',
		html: true,
		content: $('#PointGroupConfigurationSettings').html(),
		trigger: "manual"
	});
	var dataPopover = popover.data('bs.popover');
	$(parentControl).popover('show');
	$("#customModalBackground").removeClass("hidden");

	dataPopover.tip().find('.popover-content').find('[name=pointGroupSettingsDescription]').val(FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].description);
	dataPopover.tip().find('.popover-content').find('[name=pointGroupOwnerName]').text(FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].owner);
	dataPopover.tip().find('.popover-content').find('[name=pointgroup-fontsize]').val(FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].fontSize);
	dataPopover.tip().find('.popover-content').find("input[name=PointGroupVisibilitySetting][value='" + FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].visibility + "']").prop("checked", true);
	// if point group is shared and and we don't own it we can't change the settings
	if (!FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].isOwnByMe) {
		dataPopover.tip().find('.popover-content').find('[name=PointGroupVisibilitySetting]').prop("disabled", "disabled");
	}
	if (!FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].isEditable) {
		dataPopover.tip().find('.popover-content').find('[name=pointGroupSettingsDescription]').attr("disabled", "disabled");
		dataPopover.tip().find('.popover-content').find('[name=pointgroup-fontsize]').attr("disabled", "disabled");
	}

	// when losing focus on the description field save the changes 
	dataPopover.tip().find('.popover-content').find('[name=pointGroupSettingsDescription]').on('blur', function () {
		FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].description = $(this).val();
		FMOperateIndex.PersistPointGroup(parentGroupTab, pointGroupControllerId, FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].getGrid());
		FMOperateIndex.refreshHamburgerMenu = true;
	});

	// when changing the private flag force a save
	dataPopover.tip().find('.popover-content').find("input[name=PointGroupVisibilitySetting]").on("change", function () {
		FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].visibility = $(this).val();
		FMOperateIndex.PersistPointGroup(parentGroupTab, pointGroupControllerId, FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].getGrid());
		FMOperateIndex.refreshHamburgerMenu = true;
	});

	// event to switching the font size
	dataPopover.tip().find('.popover-content').find("[name=pointgroup-fontsize]").on('change', function () {
		var grid = FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].getGrid();
		var columns = grid.getColumns();
		for (var i = 0; i < columns.length; i++) {
			var headerCss = columns[i].headerCssClass ? columns[i].headerCssClass : '';
			headerCss = headerCss.replace('grid-font-8', '')
				.replace('grid-font-9', '')
				.replace('grid-font-10', '')
				.replace('grid-font-11', '')
				.replace('grid-font-12', '')
				.replace('grid-font-13', '')
				.replace('grid-font-14', '')
				.replace('grid-font-15', '')
				.replace('grid-font-16', '')
				.replace('grid-font-17', '')
				.replace('grid-font-18', '')
				.replace('grid-font-19', '')
				.replace('grid-font-20', '');
			headerCss += ' grid-font-' + $(this).val();
			columns[i].headerCssClass = headerCss;

			var columnCss = columns[i].cssClass ? columns[i].cssClass : '';
			columnCss = columnCss.replace('grid-font-8', '')
				.replace('grid-font-9', '')
				.replace('grid-font-10', '')
				.replace('grid-font-11', '')
				.replace('grid-font-12', '')
				.replace('grid-font-13', '')
				.replace('grid-font-14', '')
				.replace('grid-font-15', '')
				.replace('grid-font-16', '')
				.replace('grid-font-17', '')
				.replace('grid-font-18', '')
				.replace('grid-font-19', '')
				.replace('grid-font-20', '');
			columnCss += ' grid-font-' + $(this).val();
			columns[i].cssClass = columnCss;
		}
		grid.setOptions({
			fontSize: parseInt($(this).val())
		});
		grid.setColumns(columns);
		grid.invalidateAllRows();
		grid.render();
		grid.resizeCanvas();
		FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].fontSize = $(this).val();
		FMOperateIndex.PersistPointGroup(parentGroupTab, pointGroupControllerId, FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].getGrid());
});

	// click on Save As button
	dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupSaveAs]').on('click', function (event) {
		// remove events
		dataPopover.tip().find('.popover-content').find('[name=pointGroupSettingsDescription]').off('blur');
		dataPopover.tip().find('.popover-content').find("input[name=PointGroupVisibilitySetting]").off("change");
		dataPopover.tip().find('.popover-content').find("[name=pointgroup-fontsize]").off('change');
		dataPopover.tip().find('.popover-content').find("input[name=PointGroupVisibilitySetting]").off("change");
		dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupSaveAs]').off('click');
		dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupCancel]').off('click');

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();

		FMOperateIndex.SavePointGroupAs(parentGroupTab, pointGroupControllerId);
	});

	// click on Print button
	dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupPrint]').on('click', function (event) {

		var grid = FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].getGrid();

		var printPlugin = new Slick.Plugins.Print();
		grid.registerPlugin(printPlugin);

		$("#pointgroupprint").html('');
		printPlugin.printToElement('#pointgroupprint');
		
		grid.unregisterPlugin(printPlugin);

		var tabName = $("a[data-target='#" + FMOperateIndex.staticPointGroupControllers[pointGroupControllerId]._id + "'] .tab-name").text();

		let pointgroupprinttitle = document.getElementById("pointgroupprinttitle");
		if (pointgroupprinttitle) {
			pointgroupprinttitle.innerHTML = tabName;
		}
		$("#pointgroupprintwrapper").printThis({
			debug: false,
			importCSS: true,
			importStyle: true,
			copyTagClasses: true,
			removeInline: true,
			afterPrint: function () { $("#pointgroupprint").html(''); }

		})
			
		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
		//		$("#pointgroupprint").html('');
	});

	// click on Auto Print (from external service)
	dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupAutoPrintHidden]').on('click', function (event) {

		var grid = FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].getGrid();

		var printPlugin = new Slick.Plugins.Print();
		grid.registerPlugin(printPlugin);

		$("#pointgroupprint").html('');
		printPlugin.printToElement('#pointgroupprint');
		//$("#pointgroupprint").removeClass("hidden");
		grid.unregisterPlugin(printPlugin);

		var tabName = $("a[data-target='#" + FMOperateIndex.staticPointGroupControllers[pointGroupControllerId]._id + "'] .tab-name").text();

		//			header: "<h4 class='printpointgroupheader text-center'>" + tabName + "</h4>"
		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});

	// click on Export CSV (from external service)
	dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupExportCSVHidden]').on('click', function (event) {

		var grid = FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].getGrid();

		var exportCSVPlugin = new Slick.Plugins.ExportCSV({
			separator: ',',
		});

		grid.registerPlugin(exportCSVPlugin);

		$("#pointgroupExport").html('');
		exportCSVPlugin.saveToElement('#pointgroupExport');
		grid.unregisterPlugin(exportCSVPlugin);

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});

	// click on Export CSV (from UI)
	dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupExportCSV]').on('click', function (event) {

		var grid = FMOperateIndex.staticPointGroupControllers[pointGroupControllerId].getGrid();

		var tabId = FMOperateIndex.staticPointGroupControllers[pointGroupControllerId]._id;

		var tabName = $("a[data-target='#" + tabId + "'] .tab-name").text();

		const format1 = "_YYYYMMDD-HHmmss";

		var filenameSuffix = moment(new Date()).format(format1);

		var exportCSVPlugin = new Slick.Plugins.ExportCSV({
			separator: ',',
			filename: tabName + filenameSuffix
		});

		grid.registerPlugin(exportCSVPlugin);

		$("#pointgroupExport").html('');
		exportCSVPlugin.exportToElement('#pointgroupExport');
		grid.unregisterPlugin(exportCSVPlugin);

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});

	// click on Save As button
	dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupAutoSchedule]').on('click', function (event) {
		event.stopPropagation();

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");

		// get pointgroupguid
		var pointGroupGuid = "";
		var controlId = FMOperateIndex.staticPointGroupControllers[pointGroupControllerId]._id;

		if (parentGroupTab === 'mainTab') {
			FMOperateIndex.contents = $.map(FMOperateIndex.contents, function (obj) {
				if (obj.id === controlId) {
					ID = obj.name;
					pointGroupGuid = obj.settings.pointGroupGuid;
				}
				return obj;
			});
		}
		else {
			var parentGroupIdx = $.map(FMOperateIndex.contents, function (obj, index) {
				if (obj.id === parentGroupTab) {
					return 1;
				}
				return 0;
			});

			var foundTabGroupIdx = parentGroupIdx.indexOf(1);
			if (foundTabGroupIdx >= 0) {
				$.map(FMOperateIndex.contents[foundTabGroupIdx].settings, function (obj) {
					if (obj.id === controlId) {
						ID = obj.name;
						pointGroupGuid = obj.settings.pointGroupGuid;
					}
					return obj;
				});
			}
		}


		$('body').modalmanager('loading');

		$("#PointGroupReportScheduleRepeatNever").click();

		var tabName = $("a[data-target='#" + FMOperateIndex.staticPointGroupControllers[pointGroupControllerId]._id + "'] .tab-name").text();

		$("#PointGroupReportScheduleName").val(tabName);
		$("#PointGroupReportScheduleName").attr("data-guid", pointGroupGuid);

		FMOperateIndex.reportScheduleOpen(tabName, pointGroupGuid);
	});

	// close the pop over when clicking cancel
	dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupCancel]').on('click', function (event) {
		// remove events
		dataPopover.tip().find('.popover-content').find('[name=pointGroupSettingsDescription]').off('blur');
		dataPopover.tip().find('.popover-content').find("input[name=PointGroupVisibilitySetting]").off("change");
		dataPopover.tip().find('.popover-content').find("[name=pointgroup-fontsize]").off('change');
		dataPopover.tip().find('.popover-content').find("input[name=PointGroupVisibilitySetting]").off("change");
		dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupSaveAs]').off('click');
		dataPopover.tip().find('.popover-content').find('[name=configurationPointGroupCancel]').off('click');

		$(parentControl).popover('destroy');
		$("#customModalBackground").removeClass("hidden").addClass("hidden");
		event.stopPropagation();
	});
};

// helper to convert input readings
FMOperateIndex.convertFeetInch8thReadings = function (reading) {
	var convertedReading = '';
	reading = reading.trim();
	var negative = false;
	if (reading.substring(0, 1) === '-') {
		negative = true;
		reading = reading.substring(1);
	}


	var rawValues = reading.split('-');

	// if we have only 1 number assume that it's only feet
	if (rawValues.length === 1) {
		convertedReading = (isNaN(parseInt(rawValues[0])) ? '0' : parseInt(rawValues[0])) + '-00' + '-0';
	} // if we have only 2 number assume that its feet and inches
	else if (rawValues.length === 2) {
		convertedReading = (isNaN(parseInt(rawValues[0])) ? '0' : parseInt(rawValues[0])) + '-' + (isNaN(parseInt(rawValues[1])) ? '0' : parseInt(rawValues[1])) + '-00';
	}
	else {
		convertedReading = reading;
	}


	// convert the string 00-00-00 into an array of values
	var values = convertedReading.split('-');
	if (values.length === 3) {
		// we need to apply a conversion, for example 0-0-10 should be 0-1-2 since 10 1/8 is the equivalent to 1 foot and 2 1/8
		var eighth = parseInt(values[2]);
		if (isNaN(eighth)) {
			eighth = 0;
		}
		var moveForward = 0;
		if (eighth >= 8) {
			moveForward = parseInt(eighth / 8);
			eighth = eighth % 8;
		}

		var inches = parseInt(values[1]);
		if (isNaN(inches)) {
			inches = 0;
		}
		inches += moveForward;
		moveForward = 0;
		if (inches >= 12) {
			moveForward = parseInt(inches / 12);
			inches = inches % 12;
		}

		var feet = parseInt(values[0]);
		if (isNaN(feet)) {
			feet = 0;
		}

		feet += moveForward;

		return ((negative) ? '-' : '') + FMOperateIndex.pad(feet, 2) + '-' + FMOperateIndex.pad(inches, 2) + '-' + FMOperateIndex.pad(eighth, 1);
	}
	// if we cannot process the reading return it as is
	return reading;
};


FMOperateIndex.convertFeetInch16thReadings = function (reading) {
	var convertedReading;
	reading = reading.trim();
	var negative = false;
	if (reading.substring(0, 1) === '-') {
		negative = true;
		reading = reading.substring(1);
	}

	var rawValues = reading.split('-');

	// if we have only 1 number assume that it's only feet
	if (rawValues.length === 1) {
		convertedReading = (isNaN(parseInt(rawValues[0])) ? '0' : parseInt(rawValues[0])) + '-00' + '-00';
	} // if we have only 2 number assume that its feet and inches
	else if (rawValues.length === 2) {
		convertedReading = (isNaN(parseInt(rawValues[0])) ? '0' : parseInt(rawValues[0])) + '-' + (isNaN(parseInt(rawValues[1])) ? '00' : parseInt(rawValues[1])) + '-00';
	}
	else {
		convertedReading = reading;
	}


	// convert the string 00-00-00 into an array of values
	var values = convertedReading.split('-');
	if (values.length === 3) {
		var sixteenth = parseInt(values[2]);
		if (isNaN(sixteenth)) {
			sixteenth = 0;
		}
		var moveForward = 0;
		if (sixteenth >= 16) {
			moveForward = parseInt(sixteenth / 16);
			sixteenth = sixteenth % 16;
		}

		var inches = parseInt(values[1]);
		if (isNaN(inches)) {
			inches = 0;
		}
		inches += moveForward;
		moveForward = 0;
		if (inches >= 12) {
			moveForward = parseInt(inches / 12);
			inches = inches % 12;
		}

		var feet = parseInt(values[0]);
		if (isNaN(feet)) {
			feet = 0;
		}
		feet += moveForward;

		return ((negative) ? '-' : '') + FMOperateIndex.pad(feet, 2) + '-' + FMOperateIndex.pad(inches, 2) + '-' + FMOperateIndex.pad(sixteenth, 2);
	}
	// if we cannot process the reading return it as is
	return reading;
};

// pad number with leading zeros
FMOperateIndex.pad = function (str, max) {
	str = str.toString();
	return str.length < max ? FMOperateIndex.pad('0' + str, max) : str;
};

// get the lates values for the Point Tags if using a filter
FMOperateIndex.UpdateDynamicPointGroup = function (grid, gridControl) {
	var refreshStartTime = Date.now();

	var fmpointgroupgrid = FMOperateIndex.staticPointGroupControllers[gridControl];
	fmpointgroupgrid._updateDynamicTimer = null;

	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operate };

	// get the point filter
	var columns = grid.getColumns();
	var filter = null;
	if (columns.length > 0 && columns[0].hasOwnProperty('filter')) {
		filter = columns[0].filter;
		var tagList = $.map(columns, function (elem, id) {
			if (elem.hasOwnProperty('field') && elem['field'] !== "point") {
				return elem.field;
			}
		});

		// remove duplicate tags
		var uniquetagList = tagList.filter(function (item, pos, self) {
			return self.indexOf(item) == pos;
		});

		if (uniquetagList.length === 0) {
			uniquetagList.push("NoTagFiltered");
		}

		var siteTimeZone = $('#SiteTimeZone').val();

		// Call controller to get latest values
		fmpointgroupgrid._updateDynamicAjaxRequest = $.ajax({
			type: "post",
			url: 'UpdateTagsForDynamicGroup',
			dataType: "json",
			contentType: "application/json",
			cache: false,
			data: JSON.stringify({ filter: filter, tagList: uniquetagList, siteTimeZone: siteTimeZone }),
			success: function (response) {

				// remove previous notifications
				PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

				if (!fmpointgroupgrid._updateDynamicAjaxRequest) {
					return;
				}

				fmpointgroupgrid._updateDynamicAjaxRequest = null;


				FMErrorAndExceptionHandling.HandleMessages(response,
					function (data, inError) {
						if (!inError) {

							// make sure we are still using a dynamic point group (we may have switch to a static point group before receiving this data)
							var columns = grid.getColumns();
							if (!(columns.length > 0 && columns[0].hasOwnProperty('filter'))) {
								return;
							}

							// we need to rebuild the metadata, first thing is to get the points (look for the field id of 'point', it contains the name of the point)
							var gridMetadata = $.map(data, function (elem, i) {
								if (elem.ID === 'point') {
									return { point: elem.Value, pointguid: elem.PointGuid, tags: [] };
								}
							});
							// get the metadata for the tags for each point
							$.each(gridMetadata, function (index, row) {
								$.map(data, function (tag, i) {
									if (tag.PointGuid === row.pointguid && tag.ID !== "point") {
										row.tags.push(tag);
									}
								});
							});

							// the point group may have been closed between the request and the response, if that's the case no point on continue processing
							if (!FMOperateIndex.staticPointGroupControllers[gridControl]) {
								return;
							}

							FMOperateIndex.staticPointGroupControllers[gridControl].setMetadata(gridMetadata);

							// we have to build the rows for the display
							var dataView = grid.getData();
							var rows = dataView.getItems();
							// we need to look for a total row since we will want to keep the row and put it at the end of the list
							var existingTotalRow = $.map(rows, function (existingRow, index) {
								if (existingRow.type === "total") {
									return existingRow;
								}
							});

							var tempRows = $.map(gridMetadata, function (point, index) {
								var rowBuild = { id: point.pointguid, type: "point", point: point.point, pointguid: point.pointguid };
								$.map(point.tags, function (tag, i) {
									if (tag.PointGuid === point.pointguid) {
										if (tag.ValueTypeString === "System.Double") {
											rowBuild[tag.ID] = (tag.Value !== undefined && tag.Value !== null) ? "0" : "";  // for numeric values add a "0" in the value field and the formatter will take care of the rest
										}
										else if (tag.ValueTypeString === "FMBusinessObjects.DataObjects.PointCommandStatusListReference") {
											rowBuild[tag.ID] = (tag.Value !== null && tag.Value.CurrentKey !== null) ? tag.Value.CurrentKey : "";
										}
										else if (tag.ValueTypeString === "FMBusinessObjects.DataObjects.DeviceAlarmMapReference") {
											rowBuild[tag.ID] = (tag.Value !== null && tag.Value.CurrentValue !== null) ? tag.Value.CurrentValue : "";
										}
										else {
											rowBuild[tag.ID] = tag.Value;
										}

									}
								});
								return rowBuild;

							});

							if (existingTotalRow.length > 0) {
								tempRows.push(existingTotalRow[0]);
							}

							FMOperateIndex.updateFilterParameters(grid, gridMetadata);

							dataView.beginUpdate();
							dataView.setItems(tempRows);
							dataView.endUpdate();
							dataView.refresh();
							// update totals
							FMPointGroupGrid.updateGridTotals(gridMetadata, grid);
							grid.invalidate();
						}
						fmpointgroupgrid._updateDynamicTimer = setTimeout(function () {
							FMOperateIndex.UpdateDynamicPointGroup(grid, gridControl);
						}, FMOperateIndex.getDynamicPointGroupRefreshTimeout(refreshStartTime, FMOperateIndex.tagRefreshFrequency, gridControl));

					}, messageAttributes);
			},
			error: function (request, status, error) {
				// remove previous notifications
				PNotify.removeStack(FMOperateIndex.stack_bottomright_operate);

				if (!fmpointgroupgrid._updateDynamicAjaxRequest) {
					return;
				}

				fmpointgroupgrid._updateDynamicAjaxRequest = null;

				if (!(status == 'abort')) {
					// make sure we are still using a dynamic point group (we may have switch to a static point group before receiving this data)
					var columns = grid.getColumns();
					if (!(columns.length > 0 && columns[0].hasOwnProperty('filter'))) {
						return;
					}

					// the point group may have been closed between the request and the response, if that's the case no point on continue processing
					if (!FMOperateIndex.staticPointGroupControllers[gridControl]) {
						return;
					}

					var gridMetadata = FMOperateIndex.staticPointGroupControllers[gridControl].getMetadata();

					// we have to build the rows for the display
					var dataView = grid.getData();
					var rows = dataView.getItems();
					// we need to look for a total row since we will want to keep the row and put it at the end of the list
					var existingTotalRow = $.map(rows, function (existingRow, index) {
						if (existingRow.type === "total") {
							return existingRow;
						}
					});

					var tempRows = $.map(gridMetadata, function (point, index) {
						var rowBuild = { id: point.pointguid, type: "point", point: point.point, pointguid: point.pointguid };
						$.map(point.tags, function (tag, i) {
							if (tag.PointGuid === point.pointguid) {
								tag.CommunicationsFailure = true;
								tag.Value = null;
							}
						});
						return rowBuild;

					});

					if (existingTotalRow.length > 0) {
						tempRows.push(existingTotalRow[0]);
					}

					FMOperateIndex.updateFilterParameters(grid, gridMetadata);

					dataView.beginUpdate();
					dataView.setItems(tempRows);
					dataView.endUpdate();
					dataView.refresh();
					// update totals
					FMPointGroupGrid.updateGridTotals(gridMetadata, grid);
					grid.invalidate();

					fmpointgroupgrid._updateDynamicTimer = setTimeout(function () {
						FMOperateIndex.UpdateDynamicPointGroup(grid, gridControl);
					}, FMOperateIndex.getDynamicPointGroupRefreshTimeout(refreshStartTime, FMOperateIndex.tagRefreshFrequency, gridControl));
				}

			}
		});

	}

};


FMOperateIndex.reportScheduleOpen = function (pointGroupName, pointGroupGuid) {

	var stack_bottomright_operatortab = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 };
	var messageAttributes = { addclass: 'stack-bottomright', stack: stack_bottomright_operatortab };

	var numFormatInfoString = $('#NumberFormatInfoString').val();
	var numFormatInfo = JSON.parse(numFormatInfoString);

	// remove previous notifications
	PNotify.removeStack(stack_bottomright_operatortab);
	PNotify.removeStack(FMOperateIndex.stack_bottomright_operatorScheduleModal);

	$.ajax({
		url: 'GetPointGroupSchedule',
		type: 'Post',
		dataType: 'json',
		contentType: "application/json",
		data: JSON.stringify({ "pointGroupGuid": pointGroupGuid }),
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {

					var fileTypes = data.ExportFileFormat.map(function (fileType, index) {
						return '<option value="' + (index + 1) + '" >' + fileType + '</option>';
					});

					$("#PointGroupReportSchedule #PointGroupReportScheduleExportFileFormat").html(fileTypes.join(""));

					var optns = data.Printers.map(function (printer) {
						return '<option value="' + printer + '" >' + printer + '</option>';
					});
					var noPrinter = '<option value="" >None</option>'
					$("#PointGroupReportSchedule #PointGroupReportSchedulePrinter").html(noPrinter + optns.join(""));

					$('#PointGroupReportScheduleStartDate').datetimepicker({
						buttonImage: FMLayout.calendarLocation + '/calendar.gif',
						buttonImageOnly: true,
						showOn: "button",
						showTimezone: false,
						useLocalTimezone: false,
						defaultTimezone: $("#datepickerTimezoneString").val(),
						dateFormat: FMLayout.dateFormat,
						timeFormat: FMLayout.timeFormat,
						showSecond: true,
						beforeShow: function () {
							setTimeout(function () {
								$('.ui-datepicker').css('z-index', 1100);
							}, 0);
						},
						onSelect: function (d, i) {
							if (d !== i.lastVal) {
								$(this).change();
							}
						}
					});
					$("#PointGroupReportScheduleStartDate").blur(function () {
						val = $(this).val();
						var isValid = moment(val, numFormatInfo.ShortDatePattern.toUpperCase() + " " + ConvertToMomentUITimeFormat(numFormatInfo.TimePattern), true).isValid();

						if (!isValid) {
							$('#PointGroupReportScheduleStartDate').datetimepicker("setDate", moment().format(numFormatInfo.ShortDatePattern.toUpperCase() + " " + ConvertToMomentUITimeFormat(numFormatInfo.TimePattern)));
						}
					});

					$('#PointGroupReportScheduleEndDate').datepicker({
						buttonImage: FMLayout.calendarLocation + '/calendar.gif',
						buttonImageOnly: true,
						showOn: "button",
						dateFormat: FMLayout.dateFormat,
						beforeShow: function () {
							setTimeout(function () {
								$('.ui-datepicker').css('z-index', 1100);
							}, 0);
						},
						onSelect: function (d, i) {
							if (d !== i.lastVal) {
								$(this).change();
							}
						}
					});
					$("#PointGroupReportScheduleEndDate").blur(function () {
						val = $(this).val();
						var isValid = moment(val, numFormatInfo.ShortDatePattern.toUpperCase(), true).isValid();

						if (!isValid) {
							$('#PointGroupReportScheduleEndDate').datepicker("setDate", moment().add(7, 'days').format(numFormatInfo.ShortDatePattern.toUpperCase()));
						}
					});
					// configure numeric values
					$("#PointGroupReportScheduleRepeatHourlyEvery").removeNumeric(); // remove numeric mask if there was one
					$("#PointGroupReportScheduleRepeatEvery").removeNumeric();
					$("#PointGroupReportScheduleRepeatEveryMonth").removeNumeric();
					$("#PointGroupReportScheduleMonthlyRepeatDay").removeNumeric();
					$("#PointGroupReportScheduleRepeatEveryYear").removeNumeric();

					$("#PointGroupReportScheduleRepeatHourlyEvery").numeric({ decimal: false, negative: false });
					$("#PointGroupReportScheduleRepeatEvery").numeric({ decimal: false, negative: false });
					$("#PointGroupReportScheduleRepeatEveryMonth").numeric({ decimal: false, negative: false });
					$("#PointGroupReportScheduleMonthlyRepeatDay").numeric({ decimal: false, negative: false });
					$("#PointGroupReportScheduleRepeatEveryYear").numeric({ decimal: false, negative: false });

					FMOperateIndex.resetPointGroupReportScheduleModal();
					// if no schedule we need to hide the fields and set them blank
					if (data.PointGroupSchedule.PointGroupScheduleGuid === "00000000-0000-0000-0000-000000000000") {
						$("#PointGroupReportSchedule #noschedule").removeClass('hidden');
						$("#PointGroupReportSchedule #PointGroupReportScheduleSection").removeClass('hidden').addClass('hidden');
						$("#PointGroupReportSchedule #delivery").removeClass('hidden').addClass('hidden');
						$("#PointGroupReportSchedule #output").removeClass('hidden').addClass('hidden');
						$("#PointGroupReportSchedule #pointGroupReportScheduleDeleteButton").removeClass('hidden').addClass('hidden');
						$("#PointGroupReportSchedule #pointGroupReportScheduleSaveButton").removeClass('hidden').addClass('hidden');

					} else {
						$("#PointGroupReportSchedule #noschedule").removeClass('hidden').addClass('hidden');
						$("#PointGroupReportSchedule #PointGroupReportScheduleSection").removeClass('hidden');
						$("#PointGroupReportSchedule #delivery").removeClass('hidden');
						$("#PointGroupReportSchedule #output").removeClass('hidden');
						$("#PointGroupReportSchedule #pointGroupReportScheduleDeleteButton").removeClass('hidden');
						$("#PointGroupReportSchedule #pointGroupReportScheduleSaveButton").removeClass('hidden');

						$("#PointGroupReportSchedule #PointGroupReportScheduleExportFileFormat").val(data.PointGroupSchedule.ExportFileFormat);
						$("#PointGroupReportSchedule #PointGroupReportSchedulePrinter").val(data.PointGroupSchedule.Printer);
						$("#PointGroupReportSchedule #PointGroupReportScheduleEmail").val(data.PointGroupSchedule.EmailTo);

						if (data.PointGroupSchedule.CreateNewExportFile) {
							$("#fileExportOptions input[name=reportscheduleFileExportOverwrite][value='0']").prop("checked", true);//Create new file
							$("#fileExportOptions input[name=reportscheduleFileExportOverwrite][value='1']").prop("checked", false);//Overwrite previous file
						}
						else {
							$("#fileExportOptions input[name=reportscheduleFileExportOverwrite][value='0']").prop("checked", false);//Create new file
							$("#fileExportOptions input[name=reportscheduleFileExportOverwrite][value='1']").prop("checked", true);//Overwrite previous file
						}

						 $("#PointGroupReportSchedule #PointGroupReportScheduleLayout").val(data.PointGroupSchedule.Layout);

						 $("#PointGroupReportSchedule #PointGroupReportScheduleFitToPage").val(data.PointGroupSchedule.PointGroupReportScheduleFitToPage);
						// Startschedule gets returned as "/Date(1595427985000)/" so we need to convert it to a datetime we can work with
						var convertedStartDatetime = new Date(parseInt(data.PointGroupSchedule.StartSchedule.replace(/\/Date\((\d+)\)\//gi, "$1")))
						$('#PointGroupReportScheduleStartDate').datetimepicker("setDate", moment(convertedStartDatetime).format(numFormatInfo.ShortDatePattern.toUpperCase() + " " + ConvertToMomentUITimeFormat(numFormatInfo.TimePattern)));

						$("#PointGroupReportScheduleMonthlyRepeatDay").val(moment(convertedStartDatetime).date());

						if (data.PointGroupSchedule.EndSchedule == "") {
							$("#printscheduleend input[name=reportscheduleEndOptions][value='0']").click();
						} else {
							var endScheduleArr = data.PointGroupSchedule.EndSchedule.split(' ');

							if (endScheduleArr[0] == 'd') {
								$("#printscheduleend input[name=reportscheduleEndOptions][value='2']").click();
								$('#PointGroupReportScheduleEndDate').datetimepicker("setDate", moment(endScheduleArr[1]).format(numFormatInfo.ShortDatePattern.toUpperCase()));
							}
						}

						if (data.PointGroupSchedule.CronSchedule == "* * * * *") {
							$("#PointGroupReportScheduleRepeatNever").click();
						} else {
							var cronScheduleArr = data.PointGroupSchedule.CronSchedule.split(" ");
							if (cronScheduleArr.length < 5) {  //if invalid length default to none since there is a problem
								$("#PointGroupReportScheduleRepeatNever").click();
							} else {
								var seconds = convertedStartDatetime.getSeconds();
								var minutes = convertedStartDatetime.getMinutes();
								var hours = convertedStartDatetime.getHours();

								// check for hourly schedule
								if (cronScheduleArr[0] == "0" &&
									cronScheduleArr[1] == "0" &&
									cronScheduleArr[3] == "1/1" &&
									cronScheduleArr[4] == "*" &&
									cronScheduleArr[5] == "?" &&
									cronScheduleArr[6] == "*") {
									$("#PointGroupReportScheduleRepeatHourly").click();
									$("#PointGroupReportScheduleRepeatHourlyEvery").val((cronScheduleArr[2].split('/'))[1]);
								} else if (cronScheduleArr[0] == seconds &&  // check for daily schedule
									cronScheduleArr[1] == minutes &&
									cronScheduleArr[2] == hours &&
									cronScheduleArr[4] == "*" &&
									cronScheduleArr[5] == "?" &&
									cronScheduleArr[6] == "*") {
									$("#PointGroupReportScheduleRepeatDaily").click();
									$("#PointGroupReportScheduleRepeatEvery").val((cronScheduleArr[3].split('/'))[1]);
								} else if (cronScheduleArr[0] == seconds &&  // check for weekly schedule
									cronScheduleArr[1] == minutes &&
									cronScheduleArr[2] == hours &&
									cronScheduleArr[3] == "?" &&
									cronScheduleArr[4] == "*") {

									$("#PointGroupReportScheduleRepeatWeekly").click();
									$("#PointGroupReportScheduleRepeatSun").removeClass('active');
									$("#PointGroupReportScheduleRepeatMon").removeClass('active');
									$("#PointGroupReportScheduleRepeatTue").removeClass('active');
									$("#PointGroupReportScheduleRepeatWed").removeClass('active');
									$("#PointGroupReportScheduleRepeatThu").removeClass('active');
									$("#PointGroupReportScheduleRepeatFri").removeClass('active');
									$("#PointGroupReportScheduleRepeatSat").removeClass('active');
									if (cronScheduleArr[5] == '*') {
										// do nothing because no day was selected
									} else {
										var weekdays = cronScheduleArr[5].split(',');
										if (weekdays.indexOf("1") != -1) {
											$("#PointGroupReportScheduleRepeatSun").addClass('active');
										}
										if (weekdays.indexOf("2") != -1) {
											$("#PointGroupReportScheduleRepeatMon").addClass('active');
										}
										if (weekdays.indexOf("3") != -1) {
											$("#PointGroupReportScheduleRepeatTue").addClass('active');
										}
										if (weekdays.indexOf("4") != -1) {
											$("#PointGroupReportScheduleRepeatWed").addClass('active');
										}
										if (weekdays.indexOf("5") != -1) {
											$("#PointGroupReportScheduleRepeatThu").addClass('active');
										}
										if (weekdays.indexOf("6") != -1) {
											$("#PointGroupReportScheduleRepeatFri").addClass('active');
										}
										if (weekdays.indexOf("7") != -1) {
											$("#PointGroupReportScheduleRepeatSat").addClass('active');
										}
									}
									$("#PointGroupReportScheduleRepeatEvery").val((cronScheduleArr[3].split('/'))[1]);
								} else if (cronScheduleArr.length == 7) { //yearly schedule
									$("#PointGroupReportScheduleRepeatYearly").click();
									$("#PointGroupReportScheduleRepeatEveryYear").val((cronScheduleArr[6].split('/'))[1]);
								} else { //monthly schedule

									$("#PointGroupReportScheduleRepeatMonthly").click();
									$("#PointGroupReportScheduleRepeatEveryMonth").val((cronScheduleArr[4].split('/'))[1]);
									if (!isNaN(cronScheduleArr[3]) && cronScheduleArr[5] == "?") {
										$("#printschedulemonthly input[name=reportscheduleMonthly][value='0']").click();
										$("#PointGroupReportScheduleMonthlyRepeatDay").val(cronScheduleArr[3]);
									} else {
										$("#printschedulemonthly input[name=reportscheduleMonthly][value='1']").click();
										if (cronScheduleArr[5].indexOf("#") > -1) {
											var weekdays = cronScheduleArr[5].split('#');
											$("#reportscheduleMonthlyDay").val(parseInt(weekdays[0]) + 2);
											$("#reportscheduleMonthlyWeek").val(weekdays[1]);
										} else if (cronScheduleArr[3] == "1") {
											$("#reportscheduleMonthlyDay").val("1");
											$("#reportscheduleMonthlyWeek").val("1");
										} else if (cronScheduleArr[3] == "1W") {
											$("#reportscheduleMonthlyDay").val("2");
											$("#reportscheduleMonthlyWeek").val("1");
										} else if (cronScheduleArr[3] == "2W") {
											$("#reportscheduleMonthlyDay").val("2");
											$("#reportscheduleMonthlyWeek").val("2");
										} else if (cronScheduleArr[3] == "3W") {
											$("#reportscheduleMonthlyDay").val("2");
											$("#reportscheduleMonthlyWeek").val("3");
										} else if (cronScheduleArr[3] == "4W") {
											$("#reportscheduleMonthlyDay").val("2");
											$("#reportscheduleMonthlyWeek").val("4");
										} else if (cronScheduleArr[3] == "L") {
											$("#reportscheduleMonthlyDay").val("1");
											$("#reportscheduleMonthlyWeek").val("5");
										} else if (cronScheduleArr[3] == "LW") {
											$("#reportscheduleMonthlyDay").val("2");
											$("#reportscheduleMonthlyWeek").val("5");
										} else if (cronScheduleArr[5].indexOf("L") > -1) {
											cronScheduleArr[5] = cronScheduleArr[5].replace("L", "");
											$("#reportscheduleMonthlyDay").val(parseInt(cronScheduleArr[5]) + 2);
											$("#reportscheduleMonthlyWeek").val("5");
										}
									}

								}

							}
						}
					}
					$("#PointGroupReportSchedule .modal-has-error").removeClass('modal-has-error');

					$("#PointGroupReportSchedule").modal("show");
					FMOperateIndex.stack_bottomright_operatorScheduleModal = { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25, "context": $('#PointGroupReportSchedule') };
					PNotify.removeStack(FMOperateIndex.stack_bottomright_operatorScheduleModal);
				} else {
					// remove the loading of the modal
					var modalManager = $("body").data("modalmanager");
					modalManager.removeLoading();
				}

			}, messageAttributes);
		},
		error: function (request, status, error) {
			FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
				// remove the loading of the modal
				var modalManager = $("body").data("modalmanager");
				modalManager.removeLoading();
			}, messageAttributes);
		}
	});

};


FMOperateIndex.SavePointGroupSchedule = function () {
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operatorScheduleModal };

	// Try to save the point group schedule
	$("#PointGroupReportSchedule .modal-has-error").removeClass('modal-has-error');
	// get type of schedule
	var scheduletype = $("#PointGroupReportScheduleRepeat > .active").attr("id");

	var pointGroupGuid = $("#PointGroupReportScheduleName").attr("data-guid");
	var startschedule = $('#PointGroupReportScheduleStartDate').datetimepicker('getDate');
	var endschedule = "";
	if ($("#printscheduleend input[name=reportscheduleEndOptions][value='2']").prop("checked")) {
		endschedule = "d " + $('#PointGroupReportScheduleEndDate').datepicker('getDate').toISOString();

		if ($('#PointGroupReportScheduleEndDate').datepicker('getDate') < startschedule &&
			scheduletype !== "PointGroupReportScheduleRepeatNever") {
			FMErrorAndExceptionHandling.ShowError("End Date cannot be less than the Start Date.", null, messageAttributes);
			return;
		}
	}

	var fileExportOverwrite = true;
	if ($("#fileExportOptions input[name=reportscheduleFileExportOverwrite][value='0']").prop("checked")) { // Create new file
		fileExportOverwrite = false;
	}

	var exportFileType = $("#PointGroupReportScheduleExportFileFormat").val();
	var printer = $("#PointGroupReportSchedulePrinter").val();
	if (printer == null) {
		$("#PointGroupReportSchedulePrinter").parent().removeClass('modal-has-error').addClass('modal-has-error');
	}
	var emailto = $("#PointGroupReportScheduleEmail").val();
	 var layout = $("#PointGroupReportScheduleLayout").val();
	 var fitToPage = $("#PointGroupReportScheduleFitToPage").prop('checked');

	if (emailto == "" && printer == "") {
		FMErrorAndExceptionHandling.ShowError("Printer and/or Email must to be configured.", null, messageAttributes);
		return;

	}

	// create the cron exression for the scheduler
	var cronschedule = '* * * * *';
	switch (scheduletype) {
		case "PointGroupReportScheduleRepeatNever":
			break;
		case "PointGroupReportScheduleRepeatHourly":
			var numhours = $("#PointGroupReportScheduleRepeatHourlyEvery").val();
			if (numhours == "") {
				$("#PointGroupReportScheduleRepeatHourlyEvery").parent().removeClass('modal-has-error').addClass('modal-has-error');
			}
			cronschedule = '0 0 0/' + numhours + ' 1/1 * ? *';
			break;
		case "PointGroupReportScheduleRepeatDaily":
			var numdays = $("#PointGroupReportScheduleRepeatEvery").val();
			if (numdays == "") {
				$("#PointGroupReportScheduleRepeatEvery").parent().removeClass('modal-has-error').addClass('modal-has-error');
			}
			var seconds = startschedule.getSeconds();
			var minutes = startschedule.getMinutes();
			var hours = startschedule.getHours();
			cronschedule = seconds + ' ' + minutes + ' ' + hours + ' 1/' + numdays + ' * ? *';
			break;
		case "PointGroupReportScheduleRepeatWeekly":
			var weekdays = [];
			if ($("#PointGroupReportScheduleRepeatSun.active").length > 0) {
				weekdays.push(1);
			}
			if ($("#PointGroupReportScheduleRepeatMon.active").length > 0) {
				weekdays.push(2);
			}
			if ($("#PointGroupReportScheduleRepeatTue.active").length > 0) {
				weekdays.push(3);
			}
			if ($("#PointGroupReportScheduleRepeatWed.active").length > 0) {
				weekdays.push(4);
			}
			if ($("#PointGroupReportScheduleRepeatThu.active").length > 0) {
				weekdays.push(5);
			}
			if ($("#PointGroupReportScheduleRepeatFri.active").length > 0) {
				weekdays.push(6);
			}
			if ($("#PointGroupReportScheduleRepeatSat.active").length > 0) {
				weekdays.push(7);
			}
			var weekdayexpr = "*";
			if (weekdays.length > 0) weekdayexpr = weekdays.join(',');
			var seconds = startschedule.getSeconds();
			var minutes = startschedule.getMinutes();
			var hours = startschedule.getHours();

			cronschedule = seconds + ' ' + minutes + ' ' + hours + ' ? * ' + weekdayexpr;
			break;
		case "PointGroupReportScheduleRepeatMonthly":
			var nummonths = $("#PointGroupReportScheduleRepeatEveryMonth").val();
			if (nummonths == "") {
				$("#PointGroupReportScheduleRepeatEveryMonth").parent().removeClass('modal-has-error').addClass('modal-has-error');

			}
			var seconds = startschedule.getSeconds();
			var minutes = startschedule.getMinutes();
			var hours = startschedule.getHours();
			var month = '*';
			var repeatDay = '*';

			if ($("#printschedulemonthly input[name=reportscheduleMonthly][value='0']").prop("checked") == true) {
				repeatDay = $("#PointGroupReportScheduleMonthlyRepeatDay").val();
				if (repeatDay == "") {
					$("#PointGroupReportScheduleMonthlyRepeatDay").parent().removeClass('modal-has-error').addClass('modal-has-error');
					return;
				}
			}
			var weekrepeatpattern = '?'
			if ($("#printschedulemonthly input[name=reportscheduleMonthly][value='1']").prop("checked") == true) {

				var repeatdayofweek = $("#reportscheduleMonthlyDay").val();
				if (parseInt(repeatdayofweek) > 2) { //days of the week
					weekrepeatpattern = (parseInt(repeatdayofweek) - 2).toString();
				} else if (repeatdayofweek == "1") {  //day
					repeatDay = "1";
				} else if (repeatdayofweek == "2") { //weekday
					repeatDay = 'W';
				}

				var repeatweek = $("#reportscheduleMonthlyWeek").val();
				if (repeatweek == '5') // last of the month
				{
					if (repeatDay == 'W') repeatDay = 'LW';
					if (repeatDay == '1') repeatDay = 'L';
					if (weekrepeatpattern != '?') {
						repeatDay = '?';
						weekrepeatpattern = weekrepeatpattern + 'L';
					}
				} else {
					if (repeatDay == 'W') {
						repeatDay = repeatweek + 'W';
					} else if (weekrepeatpattern != '?') {
						repeatDay = '?';
						weekrepeatpattern = weekrepeatpattern + "#" + repeatweek;
					} else {
						repeatDay = repeatweek;
					}

				}

			}

			cronschedule = seconds + ' ' + minutes + ' ' + hours + ' ' + repeatDay + ' ' + month + '/' + nummonths + ' ' + weekrepeatpattern;
			break;
		case "PointGroupReportScheduleRepeatYearly":
			var numyears = $("#PointGroupReportScheduleRepeatEveryYear").val();
			if (numyears == "") {
				$("#PointGroupReportScheduleRepeatEveryYear").parent().removeClass('modal-has-error').addClass('modal-has-error');

			}
			var seconds = startschedule.getSeconds();
			var minutes = startschedule.getMinutes();
			var hours = startschedule.getHours();
			var year = startschedule.getFullYear();
			var month = startschedule.getMonth() + 1;
			var day = startschedule.getDate();
			cronschedule = seconds + ' ' + minutes + ' ' + hours + ' ' + day + ' ' + month + ' ? ' + year + '/' + numyears;

			break;
		default:
			break;
	}

	// if there are errors don't save
	if ($("#PointGroupReportSchedule .modal-has-error").length > 0) {
		return;
	}

	// remove previous notifications
	PNotify.removeStack(FMOperateIndex.stack_bottomright_operatorScheduleModal);


	$.ajax({
		url: 'SavePointGroupSchedule',
		type: 'Post',
		dataType: 'json',
		contentType: "application/json",
		 data: JSON.stringify({ "pointGroupGuid": pointGroupGuid, "cronschedule": cronschedule, "startschedule": startschedule.toISOString(), "endschedule": endschedule, "printer": printer, "emailto": emailto, "layout": layout, "fileType": exportFileType, "exportOptions": fileExportOverwrite, "fitToPage": fitToPage }),
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					// we get back the pointGroupGuid during the save, for when we add a new pointgroup
					var pointGroupGuid = data.pointGroupGuid;

					// if we found an existing pointgroup with the same ID, visibility, site and owner
					if (data.duplicateFound) {
						FMErrorAndExceptionHandling.ShowError("Cannot Save Changes, duplicate PointGroup.", null, messageAttributes);
						return;
					}
				}

			}, messageAttributes);
		},
		error: function (request, status, error) {
			FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
			}, messageAttributes);
		}
	});

};



FMOperateIndex.DeletePointGroupSchedule = function () {
	// Try to save the point group schedule
	var pointGroupGuid = $("#PointGroupReportScheduleName").attr("data-guid");

	var messageAttributes = { addclass: 'stack-bottomright', stack: FMOperateIndex.stack_bottomright_operatorScheduleModal };

	// remove previous notifications
	PNotify.removeStack(FMOperateIndex.stack_bottomright_operatorScheduleModal);


	$.ajax({
		url: 'DeletePointGroupSchedule',
		type: 'Post',
		dataType: 'json',
		contentType: "application/json",
		data: JSON.stringify({ "pointGroupGuid": pointGroupGuid }),
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					$("#noschedule").removeClass('hidden');
					$("#PointGroupReportSchedule #PointGroupReportScheduleSection").removeClass('hidden').addClass('hidden');
					$("#delivery").removeClass('hidden').addClass('hidden');
					$("#output").removeClass('hidden').addClass('hidden');
					$("#pointGroupReportScheduleDeleteButton").removeClass('hidden').addClass('hidden');
					$("#pointGroupReportScheduleSaveButton").removeClass('hidden').addClass('hidden');
					$(window).resize(); // force the modal to center
				}

			}, messageAttributes);
		},
		error: function (request, status, error) {
			FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
			}, messageAttributes);
		}
	});
}

FMOperateIndex.AddPointGroupSchedule = function () {
	$("#noschedule").removeClass('hidden').addClass('hidden');
	$("#PointGroupReportSchedule #PointGroupReportScheduleSection").removeClass('hidden');
	$("#delivery").removeClass('hidden');
	$("#output").removeClass('hidden');
	$("#pointGroupReportScheduleDeleteButton").removeClass('hidden');
	$("#pointGroupReportScheduleSaveButton").removeClass('hidden');

	FMOperateIndex.resetPointGroupReportScheduleModal();

	$(window).resize(); // force the modal to center
}

FMOperateIndex.reportscheduleEndOptions = function (option) {
	switch (option) {
		case '0':
			$("#PointGroupReportScheduleEndDate").prop("disabled", true);
			$('#PointGroupReportScheduleEndDate').datepicker('disable');
			break;
		case '1':
			$("#PointGroupReportScheduleEndDate").prop("disabled", true);
			$('#PointGroupReportScheduleEndDate').datepicker('disable');
			break;
		case '2':
			$("#PointGroupReportScheduleEndDate").prop("disabled", false);
			$('#PointGroupReportScheduleEndDate').datepicker('enable');


			break;
		default:
			break;
	}
}

FMOperateIndex.reportScheduleMonthOptions = function (option) {
	switch (option) {
		case '0':
			$("#PointGroupReportScheduleMonthlyRepeatDay").prop("disabled", false);
			$("#reportscheduleMonthlyWeek").prop("disabled", true);
			$("#reportscheduleMonthlyDay").prop("disabled", true);
			break;
		case '1':
			$("#PointGroupReportScheduleMonthlyRepeatDay").prop("disabled", true);
			$("#reportscheduleMonthlyWeek").prop("disabled", false);
			$("#reportscheduleMonthlyDay").prop("disabled", false);
			break;
		default:
			break;
	}
}

FMOperateIndex.reportscheduleFileExportOverwrite = function (option) {
	switch (option) {
		case '0': // Create new file
			$("#fileExportOptions input[name=reportscheduleFileExportOverwrite][value='0']").prop("checked", true);
			$("#fileExportOptions input[name=reportscheduleFileExportOverwrite][value='1']").prop("checked", false);
			break;
		case '1': // Overwrite previous file
			$("#fileExportOptions input[name=reportscheduleFileExportOverwrite][value='0']").prop("checked", false);
			$("#fileExportOptions input[name=reportscheduleFileExportOverwrite][value='1']").prop("checked", true);
			break;
		default:
			break;
	}
}

FMOperateIndex.resetPointGroupReportScheduleModal = function () {

	var numFormatInfoString = $('#NumberFormatInfoString').val();
	var numFormatInfo = JSON.parse(numFormatInfoString);

	$("#PointGroupReportSchedule #PointGroupReportScheduleExportFileFormat").val(1);//PDF
	$("#PointGroupReportSchedule #PointGroupReportSchedulePrinter").val('');
	$("#PointGroupReportSchedule #PointGroupReportScheduleEmail").val('');
	$("#PointGroupReportSchedule #PointGroupReportScheduleLayout").val(1);//Portrait
	$('#PointGroupReportScheduleStartDate').datetimepicker("setDate", moment().format(numFormatInfo.ShortDatePattern.toUpperCase() + " " + ConvertToMomentUITimeFormat(numFormatInfo.TimePattern)));
	$('#PointGroupReportScheduleEndDate').datetimepicker("setDate", moment().add(7, 'days').format(numFormatInfo.ShortDatePattern.toUpperCase()));
	$("#PointGroupReportScheduleRepeatHourlyEvery").val(1);
	$("#PointGroupReportScheduleRepeatEvery").val(1);
	$("#PointGroupReportScheduleRepeatEveryYear").val(1);
	$("#PointGroupReportScheduleRepeatEveryMonth").val(1);
	$("#PointGroupReportScheduleMonthlyRepeatDay").val((new Date()).getDate());
	$("#printschedulemonthly input[name=reportscheduleMonthly][value='0']").click(); // default option to the first one
	$("#printscheduleend input[name=reportscheduleEndOptions][value='0']").click();//Never
	$("#fileExportOptions input[name=reportscheduleFileExportOverwrite][value='0']").click();//Create new file

	$("#PointGroupReportScheduleRepeatSun").removeClass('active');
	$("#PointGroupReportScheduleRepeatMon").removeClass('active');
	$("#PointGroupReportScheduleRepeatTue").removeClass('active');
	$("#PointGroupReportScheduleRepeatWed").removeClass('active');
	$("#PointGroupReportScheduleRepeatThu").removeClass('active');
	$("#PointGroupReportScheduleRepeatFri").removeClass('active');
	$("#PointGroupReportScheduleRepeatSat").removeClass('active');
}

// input field validation for min/max numeric values
FMOperateIndex.enforceMinMax = function (el) {
	if (el.value != "") {
		if (parseInt(el.value) < parseInt(el.min)) {
			el.value = el.min;
		}
		if (parseInt(el.value) > parseInt(el.max)) {
			el.value = el.max;
		}
	}
}

FMOperateIndex.getDynamicPointGroupRefreshTimeout = function (startTime, refreshTimeout, id) {
	// id required for diagnostics and potentially future performance statistics
	// as each tab/dynamic point group maintains its own update loop
	var elapsedTime = (Date.now() - startTime);
	var efficientRefreshTimeout = refreshTimeout - elapsedTime;
	efficientRefreshTimeout = (efficientRefreshTimeout < 0 ? 0 : efficientRefreshTimeout);
	FMOperateIndex.Statistics.push({ timestamp: Date.now(), elapsed: elapsedTime });

	return efficientRefreshTimeout;
}

FMOperateIndex.GetStatistics = function () {
	var minuteCount = 0;
	var minuteTotalTime = 0;
	var minuteMaxTime = 0;
	var sessionCount = 0;
	var sessionTotalTime = 0;
	var sessionMaxTime = 0;
	var timestamp = Date.now();

	for (i = FMOperateIndex.Statistics.length - 1; i > 0; i--) {
		var record = FMOperateIndex.Statistics[i];
		if (timestamp - record.timestamp <= 60000) {
			minuteCount++;
			minuteTotalTime += record.elapsed;
			if (record.elapsed > minuteMaxTime) {
				minuteMaxTime = record.elapsed;
			}
		}
		sessionCount++;
		sessionTotalTime += record.elapsed;
		if (record.elapsed > sessionMaxTime) {
			sessionMaxTime = record.elapsed;
		}
	}
	return {
		minuteAvgTime: minuteCount > 0 ? minuteTotalTime / minuteCount : 0,
		minuteMaxTime: minuteMaxTime,
		sessionAvgTime: sessionCount > 0 ? sessionTotalTime / sessionCount : 0,
		sessionMaxTime: sessionMaxTime
	};
}

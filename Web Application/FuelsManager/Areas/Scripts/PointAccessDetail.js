if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

$(document).ready(function () {
	FMPointAccess.Init();

	FMPointAccess.activePanel = $("#accordion div.panel-pag:first");
	$(FMPointAccess.activePanel).addClass('active');

	$("#pointTemplateList, #pointList, #settingList, #tagList, #alarmList").niceScroll({
		cursorwidth: '10px',
		autohidemode: false,
		cursorcolor: '#486899',
		background: 'rgb(240, 240, 240)',
		horizrailenabled: false,
		railoffset: true,
		railpadding: { top: 0, right: 0, left: -10, bottom: 0 },
		smoothscroll: true
	});

	//----- click on the menu items (tab header)
	$("#PAGSecurity").click(function () {
		// hide the panels for the other editors
		$("#PAGMenuItems li").removeClass('active');
		$("#PAGSecurity").addClass("active");
		$("#sectionaccessrights").removeClass('hidden');
		$("#sectionusergroup").removeClass('hidden').addClass('hidden');
		$("#sectionuser").removeClass('hidden').addClass('hidden');
		if (FMPointAccess.screenMode === "UserGroupView") {
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointAccess/PointAccessUserGroupDetail";
		}
		else {
			window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointAccess/PointAccessGroupDetail";
		}
		});

	$("#PAGUserGroups").click(function () {
		// hide the panels for the other editors
		$("#PAGMenuItems li").removeClass('active');
		$("#PAGUserGroups").addClass("active");
		$("#sectionaccessrights").removeClass('hidden').addClass('hidden');
		$("#sectionusergroup").removeClass('hidden');
		$("#sectionuser").removeClass('hidden').addClass('hidden');
		window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointAccess/PointAccessGroupDetailUserGroupTab";
	});

	$("#PAGUsers").click(function () {
		// hide the panels for the other editors
		$("#PAGMenuItems li").removeClass('active');
		$("#PAGUsers").addClass("active");
		$("#sectionaccessrights").removeClass('hidden').addClass('hidden');
		$("#sectionusergroup").removeClass('hidden').addClass('hidden');
		$("#sectionuser").removeClass('hidden');
		window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../InventoryManagement/PointAccess/PointAccessGroupDetailUsersTab";
	});


	// expand the columns
	$("#accordion").on('click', '.panel-pag', function (e) {

		if (!$(this).is('.active')) {
			$(FMPointAccess.activePanel).removeClass("col-sm-4").addClass("col-sm-2");
			$(this).removeClass("col-sm-2").addClass("col-sm-4");
			$('#accordion .panel-pag').removeClass('active');
			$(this).addClass('active');
			FMPointAccess.activePanel = this;
			$("#pointTemplateList, #pointList, #settingList, #tagList, #alarmList").getNiceScroll().resize();
		};
	});

	$("#ApplyButtonId").on('click', function (event) {
		if (FMPointAccess.screenMode === "UserGroupView") {
			var pointTemplateGroupsNames = [];
			var messageWarning = "";
			$(".pointgroupfilter.active").each(function () {
				pointTemplateGroupsNames.push($(this).attr('data-name'));
			});

			if (pointTemplateGroupsNames.length > 0) {
				messageWarning = "<p>All changes will be applied to the Point Access Group(s): " + pointTemplateGroupsNames.join(", ") + ".<p>";
			}
			// Prompt to confirm changes
			FMLayout.ConfirmSaveCancel(messageWarning + "<p>Do you want to save changes?</p>",
				"Save Changes",
				function () {
					FMPointAccess.SaveChanges();
				},
				function () { });
		}
		else {
			FMPointAccess.SaveChanges();
		}

	});

	//----- click on an item in a list in the columns
	$("#pointTemplateList li").on('click', function (event) {

		if ($(event.target).hasClass('ignore-click')) {
			// if the screen is in readonly then ignore the clicks to the checkboxes
			if ($("#sectionaccessrights.readonly").length > 0) {
				event.preventDefault();
			}
			return;
		}

		if ($("#panel-pointTemplate").hasClass('active') === true) {
			// toggle the active status
			if ($(this).hasClass('active')) {
				if (FMPointAccess.Filter.pointGuid === "") {
					FMPointAccess.Filter.pointTemplateGuid = "";
					FMPointAccess.Filter.ignorePointGuidFilter = false;
				}
				else {
					FMPointAccess.Filter.ignorePointGuidFilter = true;
				}

			}
			else {
				var pointTemplateGuid = $(this).attr('data-guid');
				FMPointAccess.Filter.pointTemplateGuid = pointTemplateGuid;
				FMPointAccess.Filter.ignorePointGuidFilter = false;
				if ($("#pointList li.active").length > 0) {
					if ($("#pointList li.active").attr('data-template-guid') !== pointTemplateGuid) {
						$("#pointList li.active").removeClass('active');
						FMPointAccess.Filter.pointGuid = "";
					}
				}
			}
			$("#pointTemplateList li.active").removeClass('active');
			FMPointAccess.ApplyFilter();
		}

	});

	$("#pointList li").on('click', function (event) {
		if ($(event.target).hasClass('ignore-click')) {
			// if the screen is in readonly then ignore the clicks to the checkboxes
			if ($("#sectionaccessrights.readonly").length > 0) {
				event.preventDefault();
			}
			return;
		}

		if ($('#point-alarm-test-type').find(':selected').val() === "0") {
			// only allow the click if the panel is already expanded
			if ($("#panel-point").hasClass('active') === true) {

				if ($(this).hasClass('active')) {
					FMPointAccess.Filter.pointGuid = "";
					if (FMPointAccess.Filter.ignorePointGuidFilter === true) {
						FMPointAccess.Filter.pointTemplateGuid = "";
					}
				}
				else {
					var pointGuid = $(this).attr('data-guid');
					FMPointAccess.Filter.pointGuid = pointGuid;

					if (FMPointAccess.Filter.pointTemplateGuid === "") {
						FMPointAccess.Filter.ignorePointGuidFilter = true;
					}

					var templateGuid = $(this).attr('data-template-guid');
					FMPointAccess.Filter.pointTemplateGuid = templateGuid;
					$("#pointTemplateList li").removeClass('active');
				}

				$("#pointList li").removeClass('active');
			}
		}
		else {
			if ($("#panel-point").hasClass('active') === true) {
				$(this).toggleClass('active');

				FMPointAccess.Filter.pointGuid = "";
				$('#pointList li').each(function () {
					if ($(this).hasClass('active')) {
						FMPointAccess.Filter.pointGuid = FMPointAccess.Filter.pointGuid + ';' + $(this).attr('data-guid');
					}
				});
			}
		}

		FMPointAccess.ApplyFilter();
	});

	$("#settingList li").on('click', function (event) {

		if ($(event.target).hasClass('ignore-click')) {
			// if the screen is in readonly then ignore the clicks to the checkboxes
			if ($("#sectionaccessrights.readonly").length > 0) {
				event.preventDefault();
			}
			return;
		}
	});

	$("#tagList li").on('click', function (event) {
		if ($(event.target).hasClass('ignore-click')) {
			// if the screen is in readonly then ignore the clicks to the checkboxes
			if ($("#sectionaccessrights.readonly").length > 0) {
				event.preventDefault();
			}
			return;
		}

		if ($("#panel-tag").hasClass('active') === true) {
			$(this).toggleClass('active');

			FMPointAccess.Filter.tagGuid = "";
			$('#tagList li').each(function () {
				if ($(this).hasClass('active')) {
					FMPointAccess.Filter.tagGuid = FMPointAccess.Filter.tagGuid + ';' + $(this).attr('data-guid');
				}
			});

			FMPointAccess.ApplyFilter();
		}
	});

	$("#alarmList li").on('click', function (event) {

		if ($(event.target).hasClass('ignore-click')) {
			// if the screen is in readonly then ignore the clicks to the checkboxes
			if ($("#sectionaccessrights.readonly").length > 0) {
				event.preventDefault();
			}
			return;
		}
	});


	// events for typing on the search filter
	$("#pointtemplate-search").on('input', function () {
		var name = $(this).val();
		FMPointAccess.Filter.poinTemplateName = name;
		FMPointAccess.ApplyFilter();
	});

	$("#point-search").on('input', function () {
		var name = $(this).val();
		FMPointAccess.Filter.pointName = name;
		FMPointAccess.ApplyFilter();
	});

	$('#point-category-filter').on('change', function () {
		FMPointAccess.ApplyFilter();
	});

	$("#setting-search").on('input', function () {
		var name = $(this).val();
		FMPointAccess.Filter.settingsName = name;
		FMPointAccess.ApplyFilter();
	});

	$("#tag-search").on('input', function () {
		var name = $(this).val();
		FMPointAccess.Filter.tagsName = name;
		FMPointAccess.ApplyFilter();
	});

	$("#alarm-search").on('input', function () {
		var name = $(this).val();
		FMPointAccess.Filter.alarmsName = name;
		FMPointAccess.ApplyFilter();
	});

	$('#point-alarm-test-type').on('change', function () {
		//console.log($('#point-alarm-test-type').find(':selected').val())
		if ($('#point-alarm-test-type').find(':selected').val() === "0") {
			if ($('#pointList li active').length > 1) {
				$('#pointList li active').removeClass('active');
			}
		}

		FMPointAccess.ApplyFilter();
	});


	// clicking on a checkbox in the header (ignore the click if in read only mode)
	$('.column-header-checkbox-group').click(function () {
		if ($(event.target).hasClass('ignore-click')) {
			// if the screen is in readonly then ignore the clicks to the checkboxes
			if ($("#sectionaccessrights.readonly").length > 0) {
				event.preventDefault();
			}
			return;
		}
	});

	// click on check all for the point templates
	$('#ck-pt-all').click(function () {
		FMPointAccess.ProcessingHeaderCheckbox = true;
		var checkStatus = $(this).prop("checked");
		$("#pointTemplateList li:not('.hidden') .checkbox-template-access").each(function (index, item) {
			$(item).prop("checked", checkStatus).trigger('change');
		});
		FMPointAccess.ProcessingHeaderCheckbox = false;
		FMPointAccess.SetAssignmentNameColor();
		FMPointAccess.SetHeaderCheckboxes();
	});

	// click on a checkbox for a template
	$(".checkbox-template-access").on('change', function (event) {
		var pointTemplateGuid = $(this).closest('li').attr('data-guid');
		var isChecked = false;

		FMPointAccess.dataChanges = true;

		if ($(this).prop('checked') === true) {
			$(this).closest('li').find('.template-name').removeClass('unselected');

			// if we are checking a check box all points should have access
			$("#pointList li[data-template-guid=" + pointTemplateGuid + "]").each(function () {
				$(this).find(".checkbox-point-access").prop('checked', true);
				$(this).find(".point-name").removeClass('unselected');
				$(this).find(".point-template-name").removeClass('unselected');
				if (FMPointAccess.screenMode === "UserGroupView") {
					var checkboxControl = $(this).find(".checkbox-point-access + span");
					var tooltip = checkboxControl.attr("title");
					$(".pointgroupfilter.active").each(function () {

						var pointAccessGroupName = $(this).attr('data-name');
						if ($.inArray(pointAccessGroupName, tooltip.split("\n")) === -1) {
							tooltip = tooltip + "\n" + pointAccessGroupName;
							checkboxControl.attr("title", tooltip);
						}
					});


				}
			});
			$("#pointList li[data-template-guid=" + pointTemplateGuid + "] .checkbox-point-access").prop('checked', true);
			$("#pointList li[data-template-guid=" + pointTemplateGuid + "] .point-name").removeClass('unselected');
			$("#pointList li[data-template-guid=" + pointTemplateGuid + "] .point-template-name").removeClass('unselected');
			isChecked = true;

			// update the model for all the points for the point template that was just checked
			$("#pointList li[data-template-guid=" + pointTemplateGuid + "]").each(function (index, point) {
				var pointGuid = $(this).attr('data-guid');
				$.each(FMPointAccess.Model, function (index, modelEntry) {
					if (FMPointAccess.screenMode === "UserGroupView") {
						// if a user group with the point group not active then there is no need to update
						if ($(".pointgroupfilter.active[data-guid=" + modelEntry.PointAccessGroupGuid + "]").length === 0)
							return;
					}
					var selectedPointAccessGroupToPointAssignment = jQuery.grep(modelEntry.PointAccessGroupToPointAssignmentList, function (a) {
						return a.PointTemplateGuid === pointTemplateGuid && a.PointGuid === pointGuid;
					});
					if (selectedPointAccessGroupToPointAssignment.length > 0) {
						$.each(selectedPointAccessGroupToPointAssignment, function (index, pointAssignment) {
							pointAssignment.Assigned = isChecked;
						});
					}
					else {
						modelEntry.PointAccessGroupToPointAssignmentList.push({
							Assigned: isChecked,
							PointAccessGroupToPointGuid: "00000000-0000-0000-0000-000000000000",
							PointTemplateGuid: pointTemplateGuid,
							PointGuid: pointGuid
						});
					}
				});
			});

			// update the tooltip
			if (FMPointAccess.screenMode === "UserGroupView") {
				var checkboxDisplaycontrol = $(this).parent().find('span');
				// if a user group with the point group not active then there is no need to update
				$(".pointgroupfilter.active").each(function () {
					var tooltip = checkboxDisplaycontrol.attr('title');
					checkboxDisplaycontrol.attr('title', tooltip + "\n" + $(this).attr('data-name'));
				});

			}
		}
		else {
			$(this).closest('li').find('.template-name').removeClass('unselected').addClass('unselected');
			// remove the tooltip
			$(this).parent().find('span').attr('title', '');
		}

		if (FMPointAccess.ProcessingHeaderCheckbox === false) {
			FMPointAccess.SetHeaderCheckboxes();
		}

		// update Model
		$.each(FMPointAccess.Model, function (index, modelEntry) {
			if (FMPointAccess.screenMode === "UserGroupView") {
				// if a user group with the point group not active then there is no need to update
				if ($(".pointgroupfilter.active[data-guid=" + modelEntry.PointAccessGroupGuid + "]").length === 0)
					return;
			}

			var selectedPointAccessGroupToPointTemplateAssignment = jQuery.grep(modelEntry.PointAccessGroupToPointTemplateAssignmentList, function (a) {
				return a.PointTemplateGuid === pointTemplateGuid;
			});

			if (selectedPointAccessGroupToPointTemplateAssignment.length > 0) {
				selectedPointAccessGroupToPointTemplateAssignment[0].Assigned = isChecked;
			}
			else {
				modelEntry.PointAccessGroupToPointTemplateAssignmentList.push({
					Assigned: isChecked,
					PointAccessGroupToPointTemplateGuid: "00000000-0000-0000-0000-000000000000",
					PointTemplateGuid: pointTemplateGuid
				});
			}
		});
	});

	// click on check all for the point
	$('#ck-p-access').click(function () {
		FMPointAccess.ProcessingHeaderCheckbox = true;
		var checkStatus = $(this).prop("checked");

		// reset tooltip
		$("#pointList li:not('.hidden') .checkbox-point-access + span").attr('title', '');

		$("#pointList li:not('.hidden') .checkbox-point-access").each(function (index) {
			$(this).prop("checked", checkStatus).trigger('change');
		});
		FMPointAccess.ProcessingHeaderCheckbox = false;
		FMPointAccess.SetAssignmentNameColor();
		FMPointAccess.SetHeaderCheckboxes();
	});

	// click on a checkbox for a point
	$(".checkbox-point-access").on('change', function () {
		var pointTemplateGuid = $(this).closest('li').attr('data-template-guid');
		var pointGuid = $(this).closest('li').attr('data-guid');
		var isChecked = false;

		FMPointAccess.dataChanges = true;

		if ($(this).prop('checked') === true) {
			$(this).closest('li').find('.point-name').removeClass('unselected');
			$(this).closest('li').find('.point-template-name').removeClass('unselected');
			isChecked = true;

			// update the tooltip
			if (FMPointAccess.screenMode === "UserGroupView") {
				var checkboxDisplaycontrol = $(this).parent().find('span');
				// if a user group with the point group not active then there is no need to update
				$(".pointgroupfilter.active").each(function () {
					var tooltip = checkboxDisplaycontrol.attr('title');
					checkboxDisplaycontrol.attr('title', tooltip + "\n" + $(this).attr('data-name'));
				});

			}


		}
		else {
			// if we are unchecking a check box then we need to make sure the template does not have the check all set
			$(this).closest('li').find('.point-name').removeClass('unselected').addClass('unselected');
			$(this).closest('li').find('.point-template-name').removeClass('unselected').addClass('unselected');
			$("#pointTemplateList li[data-guid=" + pointTemplateGuid + "] .checkbox-template-access").prop('checked', false);
			$("#pointTemplateList li[data-guid=" + pointTemplateGuid + "] .template-name").removeClass('unselected').addClass('unselected');

			// update Model
			$.each(FMPointAccess.Model, function (index, modelEntry) {
				if (FMPointAccess.screenMode === "UserGroupView") {
					// if a user group with the point group not active then there is no need to update
					if ($(".pointgroupfilter.active[data-guid=" + modelEntry.PointAccessGroupGuid + "]").length === 0)
						return;
				}
				var selectedPointAccessGroupToPointTemplateAssignment = jQuery.grep(modelEntry.PointAccessGroupToPointTemplateAssignmentList, function (a) {
					return a.PointTemplateGuid === pointTemplateGuid;
				});
				if (selectedPointAccessGroupToPointTemplateAssignment.length > 0) {
					selectedPointAccessGroupToPointTemplateAssignment[0].Assigned = false;
				}
			});

			// remove the tooltip
			$(this).parent().find('span').attr('title', '');
		}

		if (FMPointAccess.ProcessingHeaderCheckbox === false) {
			FMPointAccess.SetHeaderCheckboxes();
		}


		// update Model
		$.each(FMPointAccess.Model, function (index, modelEntry) {
			if (FMPointAccess.screenMode === "UserGroupView") {
				// if a user group with the point group not active then there is no need to update
				if ($(".pointgroupfilter.active[data-guid=" + modelEntry.PointAccessGroupGuid + "]").length === 0)
					return;
			}
			var selectedPointAccessGroupToPointAssignment = jQuery.grep(modelEntry.PointAccessGroupToPointAssignmentList, function (a) {
				return a.PointGuid === pointGuid;
			});

			if (selectedPointAccessGroupToPointAssignment.length > 0) {
				selectedPointAccessGroupToPointAssignment[0].Assigned = isChecked;
			} else {
				modelEntry.PointAccessGroupToPointAssignmentList.push({
					Assigned: isChecked,
					PointAccessGroupToPointGuid: "00000000-0000-0000-0000-000000000000",
					PointTemplateGuid: pointTemplateGuid,
					PointGuid: pointGuid
				});
			}
		});
	});

	// click on check all for the View Settings
	$('#ck-s-view').click(function () {
		FMPointAccess.ProcessingHeaderCheckbox = true;

		// reset tooltip
		$("#settingList li:not('.hidden') .checkbox-setting-view + span").attr('title', '');

		var checkStatus = $(this).prop("checked");
		$("#settingList li:not('.hidden') .checkbox-setting-view").each(function (index) {
			$(this).prop("checked", checkStatus).trigger('change');
		});
		FMPointAccess.ProcessingHeaderCheckbox = false;
		FMPointAccess.SetAssignmentNameColor();
		FMPointAccess.SetHeaderCheckboxes();
	});

	// click on check all for the View Settings
	$('#ck-s-modify').click(function () {
		FMPointAccess.ProcessingHeaderCheckbox = true;

		// reset tooltip
		$("#settingList li:not('.hidden') .checkbox-setting-modify + span").attr('title', '');

		var checkStatus = $(this).prop("checked");
		$("#settingList li:not('.hidden') .checkbox-setting-modify:not('.disabled')").each(function (index) {
			$(this).prop("checked", checkStatus).trigger('change');
		});
		FMPointAccess.ProcessingHeaderCheckbox = false;
		FMPointAccess.SetAssignmentNameColor();
		FMPointAccess.SetHeaderCheckboxes();
	});

	// click on a checkbox for a setting
	$(".checkbox-setting-view, .checkbox-setting-modify").on('change', function () {
		var row = $(this).closest('li');
		var checkboxSetting = $(this);
		var settingGuid = row.attr('data-guid');
		var settingID = row.attr('data-id');

		var isViewChecked = row.find(".checkbox-setting-view").prop('checked');
		var isModifyChecked = row.find(".checkbox-setting-modify").prop('checked');

		FMPointAccess.dataChanges = true;

		if (isViewChecked === true || isModifyChecked === true) {
			row.find('.setting-name').removeClass('unselected');
			row.find('.setting-template-name').removeClass('unselected');
		}
		else {
			// if we are checking a check box then we need to make sure the template does not have the check all set
			row.find('.setting-name').removeClass('unselected').addClass('unselected');
			row.find('.setting-template-name').removeClass('unselected').addClass('unselected');
		}
		if (FMPointAccess.ProcessingHeaderCheckbox === false) {
			FMPointAccess.SetHeaderCheckboxes();
		}

		if ($(this).prop('checked') === true) {
			// update the tooltip
			if (FMPointAccess.screenMode === "UserGroupView") {
				var checkboxDisplaycontrol = $(this).parent().find('span');
				// if a user group with the point group not active then there is no need to update
				$(".pointgroupfilter.active").each(function () {
					var tooltip = checkboxDisplaycontrol.attr('title');
					checkboxDisplaycontrol.attr('title', tooltip + "\n" + $(this).attr('data-name'));
				});
			}
		}
		else {
			// remove the tooltip
			$(this).parent().find('span').attr('title', '');
		}

		// update Model
		$.each(FMPointAccess.Model, function (index, modelEntry) {
			if (FMPointAccess.screenMode === "UserGroupView") {
				// if a user group with the point group not active then there is no need to update
				if ($(".pointgroupfilter.active[data-guid=" + modelEntry.PointAccessGroupGuid + "]").length === 0)
					return;
			}

			var selectedPointAccessGroupToSettingAssignment = jQuery.grep(modelEntry.PointAccessGroupToSettingAssignmentList, function (a) {
				return a.ExposedSettingGuid === settingGuid && a.PropertyID === settingID;
			});

			if (selectedPointAccessGroupToSettingAssignment.length > 0) {
				if (checkboxSetting.hasClass("checkbox-setting-view")) {
					selectedPointAccessGroupToSettingAssignment[0].View = isViewChecked;
				}
				if (checkboxSetting.hasClass("checkbox-setting-modify")) {
					selectedPointAccessGroupToSettingAssignment[0].Modify = isModifyChecked;
				}

			}
		});
	});

	// click on check all for the View Tags
	$('#ck-t-view').click(function () {
		FMPointAccess.ProcessingHeaderCheckbox = true;

		// reset tooltip
		$("#tagList li:not('.hidden') .checkbox-tag-view + span").attr('title', '');

		var checkStatus = $(this).prop("checked");
		$("#tagList li:not('.hidden') .checkbox-tag-view").each(function (index) {
			$(this).prop("checked", checkStatus).trigger('change');
		});
		FMPointAccess.ProcessingHeaderCheckbox = false;
		FMPointAccess.SetAssignmentNameColor();
		FMPointAccess.SetHeaderCheckboxes();

	});

	// click on check all for the Modify Tags
	$('#ck-t-modify').click(function () {
		FMPointAccess.ProcessingHeaderCheckbox = true;

		// reset tooltip
		$("#tagList li:not('.hidden') .checkbox-tag-modify + span").attr('title', '');

		var checkStatus = $(this).prop("checked");
		$("#tagList li:not('.hidden') .checkbox-tag-modify").each(function (index) {
			$(this).prop("checked", checkStatus).trigger('change');
		});
		FMPointAccess.ProcessingHeaderCheckbox = false;
		FMPointAccess.SetAssignmentNameColor();
		FMPointAccess.SetHeaderCheckboxes();

	});

	// click on check all for the Exceed range Tags
	$('#ck-t-exceed').click(function () {
		FMPointAccess.ProcessingHeaderCheckbox = true;

		// reset tooltip
		$("#tagList li:not('.hidden') .checkbox-tag-exceed + span").attr('title', '');

		var checkStatus = $(this).prop("checked");
		$("#tagList li:not('.hidden') .checkbox-tag-exceed").each(function (index) {
			$(this).prop("checked", checkStatus).trigger('change');
		});
		FMPointAccess.ProcessingHeaderCheckbox = false;
		FMPointAccess.SetAssignmentNameColor();
		FMPointAccess.SetHeaderCheckboxes();

	});

	// click on check all for the override value Tags
	$('#ck-t-override').click(function () {
		FMPointAccess.ProcessingHeaderCheckbox = true;

		// reset tooltip
		$("#tagList li:not('.hidden') .checkbox-tag-override + span").attr('title', '');

		var checkStatus = $(this).prop("checked");
		$("#tagList li:not('.hidden') .checkbox-tag-override").each(function (index) {
			$(this).prop("checked", checkStatus).trigger('change');
		});
		FMPointAccess.ProcessingHeaderCheckbox = false;
		FMPointAccess.SetAssignmentNameColor();
		FMPointAccess.SetHeaderCheckboxes();
	});

	// click on a checkbox for a tag
	$(".checkbox-tag-view, .checkbox-tag-modify, .checkbox-tag-exceed, .checkbox-tag-override").on('change', function () {
		var row = $(this).closest('li');
		var checkboxTag = $(this);
		var tagGuid = row.attr('data-guid');
		var isViewChecked = row.find(".checkbox-tag-view").prop('checked');
		var isModifyChecked = row.find(".checkbox-tag-modify").prop('checked');
		var isExceedChecked = row.find(".checkbox-tag-exceed").prop('checked');
		var isoverrideChecked = row.find(".checkbox-tag-override").prop('checked');

		FMPointAccess.dataChanges = true;

		if (FMPointAccess.ProcessingHeaderCheckbox === false) {
			if (isViewChecked === true || isModifyChecked === true || isExceedChecked === true || isoverrideChecked === true) {
				row.find('.tag-name').removeClass('unselected');
				row.find('.tag-template-name').removeClass('unselected');
			}
			else {
				// if we are checking a check box then we need to make sure the template does not have the check all set
				row.find('.tag-name').removeClass('unselected').addClass('unselected');
				row.find('.tag-template-name').removeClass('unselected').addClass('unselected');
			}

			FMPointAccess.SetHeaderCheckboxes();
		}

		if ($(this).prop('checked') === true) {
			// update the tooltip
			if (FMPointAccess.screenMode === "UserGroupView") {
				var checkboxDisplaycontrol = $(this).parent().find('span');
				// if a user group with the point group not active then there is no need to update
				$(".pointgroupfilter.active").each(function () {
					var tooltip = checkboxDisplaycontrol.attr('title');
					checkboxDisplaycontrol.attr('title', tooltip + "\n" + $(this).attr('data-name'));
				});
			}
		}
		else {
			// remove the tooltip
			$(this).parent().find('span').attr('title', '');
		}

		// update Model
		$.each(FMPointAccess.Model, function (index, modelEntry) {
			if (FMPointAccess.screenMode === "UserGroupView") {

				var filterPointAccessGroups = document.querySelector('#PAGFilterPointAccessGroups').querySelectorAll('.pointgroupfilter.active');
				var ispointAccessGroupActive = false;
				for (var i = 0, len = filterPointAccessGroups.length; i < len; i++) {
					ispointAccessGroupActive = ispointAccessGroupActive || (filterPointAccessGroups[i].getAttribute('data-guid') === modelEntry.PointAccessGroupGuid);
				}
				if (!ispointAccessGroupActive) {
					return;
				}
			}

			if ($('#point-alarm-test-type').find(':selected').val() === "0") {
				var selectedPointAccessGroupToTagAssignment = jQuery.grep(modelEntry.PointAccessGroupToTagAssignmentList, function (a) {
					return a.PointTagGuid === tagGuid;
				});

				if (selectedPointAccessGroupToTagAssignment.length > 0) {
					if (checkboxTag.hasClass("checkbox-tag-view")) {
						selectedPointAccessGroupToTagAssignment[0].View = isViewChecked;
					}
					if (checkboxTag.hasClass("checkbox-tag-modify")) {
						selectedPointAccessGroupToTagAssignment[0].Modify = isModifyChecked;
					}
					if (checkboxTag.hasClass("checkbox-tag-exceed")) {
						selectedPointAccessGroupToTagAssignment[0].ExceedRange = isExceedChecked;
					}
					if (checkboxTag.hasClass("checkbox-tag-override")) {
						selectedPointAccessGroupToTagAssignment[0].Override = isoverrideChecked;
					}
				}
			}
			else {
				var selectedPointAccessGroupToPointTagAssignment = jQuery.grep(modelEntry.PointAccessGroupToPointTagAssignmentList, function (a) {
					return a.PointTagGuid === tagGuid;
				});

				if (selectedPointAccessGroupToPointTagAssignment.length > 0) {
					if (checkboxTag.hasClass("checkbox-tag-view")) {
						selectedPointAccessGroupToPointTagAssignment[0].View = isViewChecked;
					}
					if (checkboxTag.hasClass("checkbox-tag-modify")) {
						selectedPointAccessGroupToPointTagAssignment[0].Modify = isModifyChecked;
					}
					if (checkboxTag.hasClass("checkbox-tag-exceed")) {
						selectedPointAccessGroupToPointTagAssignment[0].ExceedRange = isExceedChecked;
					}
					if (checkboxTag.hasClass("checkbox-tag-override")) {
						selectedPointAccessGroupToPointTagAssignment[0].Override = isoverrideChecked;
					}
				}
			}
		});
	});

	// click on check all for the View Alarms
	$('#ck-a-view').click(function () {
		FMPointAccess.ProcessingHeaderCheckbox = true;

		// reset tooltip
		$("#alarmList li:not('.hidden') .checkbox-alarm-view + span").attr('title', '');

		var checkStatus = $(this).prop("checked");
		$("#alarmList li:not('.hidden') .checkbox-alarm-view").each(function (index) {
			$(this).prop("checked", checkStatus).trigger('change');
		});
		FMPointAccess.ProcessingHeaderCheckbox = false;
		FMPointAccess.SetAssignmentNameColor();
		FMPointAccess.SetHeaderCheckboxes();
	});

	// click on check all for the Acknowledge Alarms
	$('#ck-a-ack').click(function () {
		FMPointAccess.ProcessingHeaderCheckbox = true;

		// reset tooltip
		$("#alarmList li:not('.hidden') .checkbox-alarm-ack + span").attr('title', '');

		var checkStatus = $(this).prop("checked");
		$("#alarmList li:not('.hidden') .checkbox-alarm-ack").each(function (index) {
			$(this).prop("checked", checkStatus).trigger('change');
		});
		FMPointAccess.ProcessingHeaderCheckbox = false;
		FMPointAccess.SetAssignmentNameColor();
		FMPointAccess.SetHeaderCheckboxes();
	});

	// click on a checkbox for an alarm
	$(".checkbox-alarm-view, .checkbox-alarm-ack").on('change', function () {
		var row = $(this).closest('li');
		var checkboxAlarm = $(this);
		var alarmGuid = row.attr('data-guid');
		var pointGuid = row.attr('data-point-guid');
		var isViewChecked = row.find(".checkbox-alarm-view").prop('checked');
		var isAckChecked = row.find(".checkbox-alarm-ack").prop('checked');

		FMPointAccess.dataChanges = true;

		if (isViewChecked === true || isAckChecked === true) {
			row.find('.alarmtest-name').removeClass('unselected');
			row.find('.alarmtest-template-name').removeClass('unselected');
		}
		else {
			// if we are checking a check box then we need to make sure the template does not have the check all set
			row.find('.alarmtest-name').removeClass('unselected').addClass('unselected');
			row.find('.alarmtest-template-name').removeClass('unselected').addClass('unselected');
		}
		if (FMPointAccess.ProcessingHeaderCheckbox === false) {
			FMPointAccess.SetHeaderCheckboxes();
		}

		if ($(this).prop('checked') === true) {
			// update the tooltip
			if (FMPointAccess.screenMode === "UserGroupView") {
				var checkboxDisplaycontrol = $(this).parent().find('span');
				// if a user group with the point group not active then there is no need to update
				$(".pointgroupfilter.active").each(function () {
					var tooltip = checkboxDisplaycontrol.attr('title');
					checkboxDisplaycontrol.attr('title', tooltip + "\n" + $(this).attr('data-name'));
				});
			}
		}
		else {
			// remove the tooltip
			$(this).parent().find('span').attr('title', '');
		}

		// update Model
		$.each(FMPointAccess.Model, function (index, modelEntry) {
			if (FMPointAccess.screenMode === "UserGroupView") {
				// if a user group with the point group not active then there is no need to update
				if ($(".pointgroupfilter.active[data-guid=" + modelEntry.PointAccessGroupGuid + "]").length === 0)
					return;
			}
			if ($('#point-alarm-test-type').find(':selected').val() === "0") {
				var selectedPointAccessGroupToAlarmAssignment = jQuery.grep(modelEntry.PointAccessGroupToAlarmTestAssignmentList, function (a) {
					return a.AlarmTestTemplateGuid === alarmGuid;
				});

				if (selectedPointAccessGroupToAlarmAssignment.length > 0) {
					if (checkboxAlarm.hasClass("checkbox-alarm-view")) {
						selectedPointAccessGroupToAlarmAssignment[0].View = isViewChecked;
					}
					if (checkboxAlarm.hasClass("checkbox-alarm-ack")) {
						selectedPointAccessGroupToAlarmAssignment[0].Acknowledge = isAckChecked;
					}
				}
			}
			else {
				var selectedPointAccessGroupToPointAlarmAssignment = jQuery.grep(modelEntry.PointAccessGroupToPointAlarmTestAssignmentList, function (a) {
					return a.AlarmTestGuid === alarmGuid && a.PointGuid === pointGuid;
				});

				if (selectedPointAccessGroupToPointAlarmAssignment.length > 0) {
					if (checkboxAlarm.hasClass("checkbox-alarm-view")) {
						selectedPointAccessGroupToPointAlarmAssignment[0].View = isViewChecked;
					}
					if (checkboxAlarm.hasClass("checkbox-alarm-ack")) {
						selectedPointAccessGroupToPointAlarmAssignment[0].Acknowledge = isAckChecked;
					}
				}
			}
		});
	});

	$("#backbutton").click(function () {
		var url = $(this).attr('data-url');
		window.top.location.search = url;
	});

	// click on a Point Access Group in the User Group View
	$("#PAGFilterPointAccessGroups").on('click', ".pointgroupnamefilter", function () {
		var parent = $(this).parent();

		// create a callback funtion we may need to call if we need to save
		var processFilterChange = function () {
			parent.toggleClass('active');
			FMPointAccess.UserGroupViewRedrawFilter();
		}


		if (FMPointAccess.dataChanges === true) {
			var pointTemplateGroupsNames = [];
			var messageWarning = "";
			$(".pointgroupfilter.active").each(function () {
				pointTemplateGroupsNames.push($(this).attr('data-name'));
			});

			if (pointTemplateGroupsNames.length > 0) {
				messageWarning = "<p>All changes will be applied to the Point Access Group(s): " + pointTemplateGroupsNames.join(", ") + ".<p>";
			}
			// Prompt to confirm changes
			FMLayout.ConfirmSaveCancel(messageWarning + "<p>Do you want to save changes?</p>",
				"Save Changes",
				function () {
					FMPointAccess.SaveChanges(processFilterChange);
				},
				function () { });
		}
		else {
			processFilterChange();
		}


	});

	// click on a Point Access Group in the User View
	$("#PAGFilterUserGroups").on('click', ".pointgroupnamefilter", function () {
		var parent = $(this).parent();

		parent.toggleClass('active');
		FMPointAccess.UserViewRedrawFilter();
	});

	// delete point access group from the selection filter
	$("#PAGFilterPointAccessGroups").on('click', ".pointgroupfilter .remove", function () {
		var pointAccessGroupGuid = $(this).parent().attr('data-guid');
		var userGroupGuid = $("#PointTemplateTitle").attr('data-guid');
		var pointAccessGroupName = $(this).parent().attr('data-name');
		var parent = $(this).parent();

		// Prompt to confirm changes
		FMLayout.ConfirmYesNo("Do you want to remove the association with the following Point Access Group: " + pointAccessGroupName + "?",
			"Delete Point Access Group",
			function () {
				// hide any other notification
				FMErrorAndExceptionHandling.CloseNotifications();
				// notification position
				var messageAttributes = { addclass: 'stack-bottomright', stack: FMPointAccess.stack_bottomright };

				// display animation
				$('<div class=loadingDiv><img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif" /></img></div>').prependTo(document.body);

				var token = $('input[name=__RequestVerificationToken]').val();
				var headers = {};
				headers['__RequestVerificationToken'] = token;

				var url = $("#urlDeleteUserGroupFromPointAccessGroup").val();
				var data = { pointAccessGroupGuid: pointAccessGroupGuid, userGroupGuid: userGroupGuid };

				$.ajax({
					url: url,
					type: 'post',
					cache: false,
					headers: headers,
					dataType: 'json',
					contentType: 'application/json; charset=utf-8',
					data: JSON.stringify(data),
					success: function (response) {
						FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
							if (!inError) {
								FMPointAccess.Model = $.grep(FMPointAccess.Model,
										 function (o, i) { return o.PointAccessGroupGuid === pointAccessGroupGuid; },
										 true);
								$(".pointgroupfilter[data-guid=" + pointAccessGroupGuid + "]").remove();
								$("#PAGFilterPointAccessGroups > div").html($("#PAGFilterPointAccessGroups > div").html().replace("&nbsp;&nbsp;", "&nbsp;"));
								FMPointAccess.UserGroupViewRedrawFilter();
							}
							// hide the saving animation
							$(".loadingDiv").remove();
						}, messageAttributes);
					},
					error: function (xhr, ajaxOptions, thrownError) {
						FMErrorAndExceptionHandling.ShowException(xhr, ajaxOptions, thrownError, function () {
							// hide the saving animation
							$(".loadingDiv").remove();
						}, messageAttributes);
					}
				});
			},
			function () { });
	});

	// add point access group in the User Group View 
	$("#PAGFilterPointAccessGroups").on('click', "#AddPointAccessGroupToUserGroup", function () {
		_addPointAccessGroupToUserGroup = function () {
			// create the backdrop and wait for next modal to be triggered
			var modalManager = $('body').modalmanager('loading');

			if ($.fn.DataTable.isDataTable('#AddPointAccessGroupSelection')) {
				$('#AddPointAccessGroupSelection').DataTable().destroy();
			}
			$('#AddPointAccessGroupSelection tbody').empty();

			var token = $('input[name=__RequestVerificationToken]').val();
			var headers = {};
			headers['__RequestVerificationToken'] = token;

			$.ajax({
				url: $("#urlListPointAccessGroup").val(),
				cache: false,
				type: 'Get',
				headers: headers,
				success: function (response) {
					FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
						if (!inError) {
							var pointAccessGroupListTable = $('#AddPointAccessGroupSelection').DataTable(
								{
									"retrieve": true,
									"select": { style: 'single' },
									"ordering": false,
									"scrollY": '300px',
									"sScrollX": '100%',
									"sScrollXInner": '100%',
									"scrollCollapse": false,
									"paging": false,
									"autoWidth": true,
									"columnDefs": [
										{ "targets": [0], "name": 'PointAccessGroup', "orderable": false, className: 'PointAccessGroupSelectColumn text-center' },
										{ "targets": [1], "name": 'Guid', "orderable": false, "visible": false }
									],
									"dom": 'rt',
									"fnInitComplete": function () {
										// custom scroll bars
										$(this).parent()
											.niceScroll({
												cursorwidth: '10px',
												autohidemode: false,
												cursorcolor: '#486899',
												background: 'rgb(240, 240, 240)',
												horizrailenabled: false
											});
									}
								});

							data.forEach(function (dataRow) {
								if ($(".pointgroupfilter[data-guid=" + dataRow.PointAccessGroupGuid + "]").length === 0) {
									pointAccessGroupListTable.row.add([dataRow.ID, dataRow.PointAccessGroupGuid]).draw(false);
								}
							});

							$('#PointAccessGroupSelectionModal').modal('show');
							pointAccessGroupListTable.draw();
							pointAccessGroupListTable.columns.adjust().draw();

						}
						else {
							$('body').modalmanager('loading'); // remove the loading background
						}

					});
				},
				error: function (xhr, textStatus, error) {
					FMErrorAndExceptionHandling.ShowException(xhr, textStatus, error, function () { });
					$('body').modalmanager('loading'); // remove the loading background
				}

			});
		}

		// if the user made any changes prompt to save
		if (FMPointAccess.dataChanges) {
			var pointTemplateGroupsNames = [];
			var messageWarning = "";
			$(".pointgroupfilter.active").each(function () {
				pointTemplateGroupsNames.push($(this).attr('data-name'));
			});

			if (pointTemplateGroupsNames.length > 0) {
				messageWarning = "<p>All changes will be applied to the Point Access Group(s): " + pointTemplateGroupsNames.join(", ") + ".<p>";
			}
			FMLayout.ConfirmSaveCancel(messageWarning + "<p>Do you want to save changes?</p>",
				"Save Changes",
				function () {
					// call the save form and tell it to _addPointAccessGroupToUserGroup if successful
					FMPointAccess.SaveChanges(_addPointAccessGroupToUserGroup);
				});
		}
			// if no changes then _addPointAccessGroupToUserGroup directly
		else {
			_addPointAccessGroupToUserGroup();
		}

	});


	$('#PointAccessGroupSelectionModalSelectButton').on('click', function () {
		// If the user has selected a module open the module screen
		if ($('#AddPointAccessGroupSelection').DataTable().rows({ selected: true })[0].length > 0) {
			var selectedPointAccessGroup = $('#AddPointAccessGroupSelection').DataTable().rows({ selected: true }).data();
			var selectedPointAccessGroupName = selectedPointAccessGroup[0][0];
			var selectedPointAccessGroupGuid = selectedPointAccessGroup[0][1];
			var selectedUserGroupGuid = $("#PointTemplateTitle").attr('data-guid');

			$('#PointAccessGroupSelectionModal').modal('hide');

			// create the backdrop and wait for next modal to be triggered
			var modalManager = $('body').modalmanager('loading');

			var token = $('input[name=__RequestVerificationToken]').val();
			var headers = {};
			headers['__RequestVerificationToken'] = token;

			$.ajax({
				url: $("#urlAddPointAccessGroupToUserGroup").val(),
				cache: false,
				type: 'post',
				headers: headers,
				data: { pointAccessGroupGuid: selectedPointAccessGroupGuid, userGroupGuid: selectedUserGroupGuid },
				success: function (response) {
					FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {

						if (!inError) {

							var filterTag = '<span class="pointgroupfilter label active" data-guid="' + data.PointAccessGroupGuid + '" data-name="' + data.Name + '" title="Click to toggle Point Access Group rights view"><span class="pointgroupnamefilter">' + data.Name + '</span><i class="remove glyphicon glyphicon-remove-sign glyphicon-white" title="Delete Point Access Group Association"></i></span>';
							$(filterTag).insertBefore("#AddPointAccessGroupToUserGroup");
							$('#PAGFilterPointAccessGroups > div > span').sort(function (a, b) {
								return $(a).attr('data-name') > $(b).attr('data-name');
							}).appendTo('#PAGFilterPointAccessGroups > div ');

							// add a space after each label to have some separation
							$("#PAGFilterPointAccessGroups > div > span").each(function () {
								$(this)[0].outerHTML = $(this)[0].outerHTML + '&nbsp;';
							});

							// redraw the screen
							var newPointAccessGroup = response.Data;
							FMPointAccess.InitializePointGroupDataDefaults(newPointAccessGroup);
							FMPointAccess.Model.push(newPointAccessGroup);
							FMPointAccess.UserGroupViewRedrawFilter();

							$('body').modalmanager('loading'); // remove the loading background
						}
						else {
							$('body').modalmanager('loading'); // remove the loading background
						}

					});
				},
				error: function (xhr, textStatus, error) {
					FMErrorAndExceptionHandling.ShowException(xhr, textStatus, error, function () { });
					$('body').modalmanager('loading'); // remove the loading background
				}

			});
		}

	});

	$(window).on("load resize", function () {
		setTimeout(function () {
			let $accordion = $("#accordion");
			let $panelContent = $(".panelContent ul");

			if ($accordion.length && $panelContent.length) {
				let accordionHeight = $accordion.outerHeight();
				let newHeight = accordionHeight - 150;

				$panelContent.css({
					"height": newHeight + "px",
				});
			}
		}, 100);
	});

});

var FMPointAccess = {
	activePanel: null,
	SelectedTag: "",
	blankPointGroupAccessDisplayModel: {},
	Model: {},
	UserGroupToPointAccessGroupMap: {},
	Filter: { pointTemplateGuid: "", poinTemplateName: "", pointGuid: "", pointName: "", ignorePointGuidFilter: false, settingsName: "", tagsName: "", tagGuid: "", alarmsName: "" },
	ProcessingHeaderCheckbox: false,
	stack_bottomright: { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 },
	screenMode: "UserGroupView",
	dataChanges: false
};

FMPointAccess.Init = function () {

	FMPointAccess.screenMode = $('#PointAccessGroupScreenMode').val();
	$('#PointAccessGroupScreenMode').remove();

	// the model is being passed as a hidden input tag which we won't need anymore
	var strModel = $('#PointAccessGroupModel').val();
	$('#PointAccessGroupModel').remove();

	FMPointAccess.InitializePointGroupDataDefaults(FMPointAccess.blankPointGroupAccessDisplayModel);

	if (!strModel) {
		strModel = "{}";
	};
	FMPointAccess.Model = JSON.parse(strModel);
	console.log(FMPointAccess.Model);
	// the model is being passed as a hidden input tag which we won't need anymore
	var userGroupToPointAccessGroupMap = $('#UserGroupToPointAccessGroupMap').val();
	$('#UserGroupToPointAccessGroupMap').remove();
	FMPointAccess.UserGroupToPointAccessGroupMap = JSON.parse(userGroupToPointAccessGroupMap);

	if (FMPointAccess.Model) {
		FMPointAccess.ResetDrawPointGroupAccess();

		if (FMPointAccess.screenMode === "UserView") {
			$.each(FMPointAccess.Model, function (index, pointAccessGroupAccess) {
				FMPointAccess.InitializePointGroupDataDefaults(pointAccessGroupAccess);
			});
			FMPointAccess.UserViewRedrawFilter();
		}
		else {
			$.each(FMPointAccess.Model, function (index, pointAccessGroupAccess) {
				FMPointAccess.InitializePointGroupDataDefaults(pointAccessGroupAccess);
				FMPointAccess.DrawPointGroupAccess(pointAccessGroupAccess);
			});
		}
	}

	FMPointAccess.SetAssignmentNameColor();
	FMPointAccess.SetHeaderCheckboxes();

	FMErrorAndExceptionHandling.OnlyOneNotification = true;
	FMPointAccess.ApplyFilter();

	window.setTimeout(function () {
		FMPointAccess.NotifyFilterChanges();
	}, 1000);
};

FMPointAccess.ApplyFilter = function () {
	// filter template
	$("#pointTemplateList li").each(function (index) {
		var templateNameFilter = $(this).attr('data-name').toLowerCase();
		if (FMPointAccess.Filter.poinTemplateName !== "") {
			templateNameFilter = FMPointAccess.Filter.poinTemplateName.toLowerCase();
		}

		var guid = $(this).attr('data-guid');

		if ($(this).attr('data-name').toLowerCase().indexOf(templateNameFilter) > -1) {
			$(this).removeClass('hidden');
			if (guid === FMPointAccess.Filter.pointTemplateGuid && FMPointAccess.Filter.ignorePointGuidFilter === false) {
				$(this).addClass('active');
			}
		}
		else {
			$(this).removeClass('hidden').addClass('hidden');
		}
	});
	if ($("#pointTemplateList li.active").hasClass('hidden') === true) {
		$("#pointTemplateList li.active").removeClass('active');
		FMPointAccess.Filter.pointTemplateGuid = "";
	}


	// filter points
	$("#pointList li").removeClass('hidden').addClass('hidden');

	if ($('#point-alarm-test-type').find(':selected').val() === "0") {
		$("#pointList li").each(function (index) {
			var pointNameFilter = $(this).attr('data-name').toLowerCase();
			if (FMPointAccess.Filter.pointName !== "") {
				pointNameFilter = FMPointAccess.Filter.pointName.toLowerCase();
			}

			var pointTemplateFilterGuid = $(this).attr('data-template-guid');
			if (FMPointAccess.Filter.pointTemplateGuid !== "" && FMPointAccess.Filter.ignorePointGuidFilter === false) {
				pointTemplateFilterGuid = FMPointAccess.Filter.pointTemplateGuid;
			}
			var guid = $(this).attr('data-guid');

			if ($(this).attr('data-name').toLowerCase().indexOf(pointNameFilter) > -1 && $(this).attr('data-template-guid') === pointTemplateFilterGuid
				&& ($('#point-category-filter').find(':selected').val() === "" || $(this).attr('data-categories').indexOf(";" + $('#point-category-filter').find(':selected').val() + ";") > -1)) {
				$(this).removeClass('hidden');
				if (guid === FMPointAccess.Filter.pointGuid) {
					$(this).addClass('active');
				}
			}
			else {
				$(this).removeClass('hidden').addClass('hidden');
			}

			if ($('#point-alarm-test-type').find(':selected').val() === "0") {
				// show all points (with user filters)
			} else {
				// show only points with configured tags with DAM
			}

		});

		if ($("#pointList li.active").hasClass('hidden') === true) {
			$("#pointList li.active").removeClass('active');
			FMPointAccess.Filter.pointGuid = "";
		}
	}
	else {
		$("#pointList li").each(function (index) {
			var pointNameFilter = $(this).attr('data-name').toLowerCase();
			if (FMPointAccess.Filter.pointName !== "") {
				pointNameFilter = FMPointAccess.Filter.pointName.toLowerCase();
			}

			var pointTemplateFilterGuid = $(this).attr('data-template-guid');
			if (FMPointAccess.Filter.pointTemplateGuid !== "" && FMPointAccess.Filter.ignorePointGuidFilter === false) {
				pointTemplateFilterGuid = FMPointAccess.Filter.pointTemplateGuid;
			}
			var guid = $(this).attr('data-guid');

			if ($(this).attr('data-name').toLowerCase().indexOf(pointNameFilter) > -1 && $(this).attr('data-template-guid') === pointTemplateFilterGuid
				&& ($('#point-category-filter').find(':selected').val() === "" || $(this).attr('data-categories').indexOf(";" + $('#point-category-filter').find(':selected').val() + ";") > -1)) {
				$(this).removeClass('hidden');
			}
			else {
				$(this).removeClass('hidden').addClass('hidden');
			}

			// show only points with configured tags with DAM
			if ($(this).attr('data-has-device-alarm-map-tags') === "False") {
				$(this).removeClass('hidden').addClass('hidden');
			}
		});

		if ($("#pointList li.active").hasClass('hidden') === true) {
			$("#pointList li.active").removeClass('active');
			FMPointAccess.Filter.pointGuid = "";
		}
	}
	// filter settings
	$("#settingList li").removeClass('hidden').addClass('hidden');

	$("#settingList li").each(function (index) {
		var settingNameFilter = $(this).attr('data-name').toLowerCase();
		if (FMPointAccess.Filter.settingsName !== "") {
			settingNameFilter = FMPointAccess.Filter.settingsName.toLowerCase();
		}

		var pointTemplateFilterGuid = $(this).attr('data-template-guid');
		if (FMPointAccess.Filter.pointTemplateGuid !== "") {
			pointTemplateFilterGuid = FMPointAccess.Filter.pointTemplateGuid;
		}

		if ($(this).attr('data-name').toLowerCase().indexOf(settingNameFilter) > -1 && $(this).attr('data-template-guid') === pointTemplateFilterGuid) {
			$(this).removeClass('hidden');
		}
		else {
			$(this).removeClass('hidden').addClass('hidden');
		}
	});


	// filter tags
	$("#tagList li").removeClass('hidden').addClass('hidden');

	$("#tagList li").each(function (index) {
		var tagNameFilter = $(this).attr('data-name').toLowerCase();
		if (FMPointAccess.Filter.tagsName !== "") {
			tagNameFilter = FMPointAccess.Filter.tagsName.toLowerCase();
		}

		var pointTemplateFilterGuid = $(this).attr('data-template-guid');
		if (FMPointAccess.Filter.pointTemplateGuid !== "") {
			pointTemplateFilterGuid = FMPointAccess.Filter.pointTemplateGuid;
		}

		var guid = $(this).attr('data-guid');

		if ($(this).attr('data-name').toLowerCase().indexOf(tagNameFilter) > -1
		&& (
				(
					$(this).attr('data-template-guid') === pointTemplateFilterGuid
					&& $(this).attr('data-is-device-alarm-map-tag') === "False"
					&& $('#point-alarm-test-type').find(':selected').val() === "0"
				)
				||
				(
					(FMPointAccess.Filter.pointGuid.toLowerCase().indexOf($(this).attr('data-point-guid')) > -1
						||
						(
							$(this).attr('data-template-guid') === pointTemplateFilterGuid
							&& FMPointAccess.Filter.pointGuid === ""
						)
					)
					&& $(this).attr('data-is-device-alarm-map-tag') === "True"
					&& $('#point-alarm-test-type').find(':selected').val() === "1"
				)
			)
		) {
			if (guid === FMPointAccess.Filter.tagGuid) {
				$(this).addClass('active');
			}

			$(this).removeClass('hidden');
		}
		else {
			$(this).removeClass('hidden').addClass('hidden');
		}
	});

	if ($("#tagList li.active").hasClass('hidden') === true) {
		$("#tagList li.active").removeClass('active');
		FMPointAccess.Filter.tagGuid = "";
	}

	// filter alarm
	$("#alarmList li").removeClass('hidden').addClass('hidden');
	$("#alarmList li").each(function (index) {
		var alarmNameFilter = $(this).attr('data-name').toLowerCase();
		if (FMPointAccess.Filter.alarmsName !== "") {
			alarmNameFilter = FMPointAccess.Filter.alarmsName.toLowerCase();
		}

		var pointTemplateFilterGuid = $(this).attr('data-template-guid');
		if (FMPointAccess.Filter.pointTemplateGuid !== "") {
			pointTemplateFilterGuid = FMPointAccess.Filter.pointTemplateGuid;
		}

		if ($('#point-alarm-test-type').find(':selected').val() === "0") {

			if ($(this).attr('data-name').toLowerCase().indexOf(alarmNameFilter) > -1
			&& $(this).attr('data-template-guid') === pointTemplateFilterGuid
			&& $(this).attr('data-is-device-alarm-map-tag') === "False"
			&& (FMPointAccess.Filter.tagGuid.toLowerCase().indexOf($(this).attr('data-tag-guid')) > -1
			|| FMPointAccess.Filter.tagGuid === "")) {
				$(this).removeClass('hidden');
			}
			else {
				$(this).removeClass('hidden').addClass('hidden');
			}
		}
		else {

			if ($(this).attr('data-name').toLowerCase().indexOf(alarmNameFilter) > -1
			&& $(this).attr('data-template-guid') === pointTemplateFilterGuid
			&& $(this).attr('data-is-device-alarm-map-tag') === "True"
			&& (FMPointAccess.Filter.pointGuid.toLowerCase().indexOf($(this).attr('data-point-guid')) > -1
			|| FMPointAccess.Filter.pointGuid === "")
			&& (FMPointAccess.Filter.tagGuid.toLowerCase().indexOf($(this).attr('data-tag-guid')) > -1
			|| FMPointAccess.Filter.tagGuid === "")) {
				$(this).removeClass('hidden');
			}
			else {
				$(this).removeClass('hidden').addClass('hidden');
			}
		}
/*
		if ($(this).attr('data-name').toLowerCase().indexOf(alarmNameFilter) > -1
		&& (	($(this).attr('data-template-guid') === pointTemplateFilterGuid
  				&& $(this).attr('data-tag-guid') === tagFilterGuid
	  			&& $(this).attr('data-is-device-alarm-map-tag') === "False"
				&& $('#point-alarm-test-type').find(':selected').val() === "0")
			||	((FMPointAccess.Filter.pointGuid.toLowerCase().indexOf($(this).attr('data-point-guid')) > -1
				||	($(this).attr('data-template-guid') === pointTemplateFilterGuid
					&& $(this).attr('data-tag-guid') === tagFilterGuid
					&& FMPointAccess.Filter.pointGuid === ""
					)
				)
				&& $(this).attr('data-is-device-alarm-map-tag') === "True"
				&& $('#point-alarm-test-type').find(':selected').val() === "1"
				)
			)
		) {
			$(this).removeClass('hidden');
		}
		else {
			$(this).removeClass('hidden').addClass('hidden');
		}
*/
/*
		if ($('#point-alarm-test-type').find(':selected').val() === "0") {
			// show all tags (with user filters) that are not DAM tags
			if ($(this).attr('data-is-device-alarm-map-tag') === "True") {
				$(this).removeClass('hidden').addClass('hidden');
			}
		} else {
			// show only points with configured tags with DAM
			if ($(this).attr('data-is-device-alarm-map-tag') === "False") {
				$(this).removeClass('hidden').addClass('hidden');
			}
		}
*/
	});

	FMPointAccess.SetHeaderCheckboxes();
}

FMPointAccess.InitializePointGroupDataDefaults = function (pointGroupAccess) {
	if (!pointGroupAccess.hasOwnProperty('PointAccessGroupToPointTemplateAssignmentList'))
		pointGroupAccess.PointAccessGroupToPointTemplateAssignmentList = [];

	if (!pointGroupAccess.hasOwnProperty('PointAccessGroupToPointAssignmentList'))
		pointGroupAccess.PointAccessGroupToPointAssignmentList = [];

	if (!pointGroupAccess.hasOwnProperty('PointAccessGroupToSettingAssignmentList'))
		pointGroupAccess.PointAccessGroupToSettingAssignmentList = [];

	if (!pointGroupAccess.hasOwnProperty('PointAccessGroupToTagAssignmentList'))
		pointGroupAccess.PointAccessGroupToTagAssignmentList = [];

	if (!pointGroupAccess.hasOwnProperty('PointAccessGroupToPointTagAssignmentList'))
		pointGroupAccess.PointAccessGroupToPointTagAssignmentList = [];

	if (!pointGroupAccess.hasOwnProperty('PointAccessGroupToAlarmTestAssignmentList'))
		pointGroupAccess.PointAccessGroupToAlarmTestAssignmentList = [];

	if (!pointGroupAccess.hasOwnProperty('PointAccessGroupToUserGroupAssignmentList'))
		pointGroupAccess.PointAccessGroupToUserGroupAssignmentList = [];

	if (!pointGroupAccess.hasOwnProperty('PointAccessGroupToPointAlarmTestAssignmentList'))
		pointGroupAccess.PointAccessGroupToPointAlarmTestAssignmentList = [];


	$.each(pointGroupAccess.PointAccessGroupToPointTemplateAssignmentList, function (index, pointTemplateAssignment) {
		// if point template is assigned then all the points for the template are assigned too
		var pointTemplateGuid = pointTemplateAssignment.PointTemplateGuid;
		$("#pointList li[data-template-guid=" + pointTemplateGuid + "]").each(function (index, point) {
			var pointGuid = $(this).attr('data-guid');
			var selectedPointAccessGroupToPointAssignment = jQuery.grep(pointGroupAccess.PointAccessGroupToPointAssignmentList, function (a) {
				return a.PointTemplateGuid === pointTemplateGuid && a.PointGuid === pointGuid;
			});
			if (selectedPointAccessGroupToPointAssignment.length > 0) {
				$.each(selectedPointAccessGroupToPointAssignment, function (index, pointAssignment) {
					pointAssignment.Assigned = true;
				});
			}
			else {
				pointGroupAccess.PointAccessGroupToPointAssignmentList.push({
					Assigned: true,
					PointAccessGroupToPointGuid: "00000000-0000-0000-0000-000000000000",
					PointTemplateGuid: pointTemplateGuid,
					PointGuid: pointGuid
				});
			}
		});
	});

	// by default all the exposed settings are set
	$("#settingList li").each(function (index, setting) {
		var settingGuid = $(this).attr('data-guid');
		var propertyId = $(this).attr('data-id');
		var pointTemplateGuid = $(this).attr('data-template-guid');
		var isInhibitModifiedSet = ($(this).find('.checkbox-setting-modify:not(.disabled)').length === 0);
		var selectedPointAccessGroupToSettingAssignment = jQuery.grep(pointGroupAccess.PointAccessGroupToSettingAssignmentList, function (a) {
			return a.PointTemplateGuid === pointTemplateGuid && a.ExposedSettingGuid === settingGuid && a.PropertyID === propertyId;
		});
		if (selectedPointAccessGroupToSettingAssignment.length === 0) {
			pointGroupAccess.PointAccessGroupToSettingAssignmentList.push({
				PointAccessGroupToExposedSettingGuid: "00000000-0000-0000-0000-000000000000",
				PointTemplateGuid: pointTemplateGuid,
				ExposedSettingGuid: settingGuid,
				PropertyID: propertyId,
				View: true,
				Modify: (isInhibitModifiedSet ? false : true)
			});
		}
	});

	$("#tagList li").each(function (index, tag) {
		var tagGuid = $(this).attr('data-guid');
		var pointTemplateGuid = $(this).attr('data-template-guid');
		var pointGuid = $(this).attr('data-point-guid');

		if (pointGuid === "" || pointGuid === undefined) {
			var selectedPointAccessGroupToTagAssignment = jQuery.grep(pointGroupAccess.PointAccessGroupToTagAssignmentList, function (a) {
				return a.PointTemplateGuid === pointTemplateGuid && a.PointTagGuid === tagGuid;
			});
			if (selectedPointAccessGroupToTagAssignment.length === 0) {
				pointGroupAccess.PointAccessGroupToTagAssignmentList.push({
					PointAccessGroupToTagGuid: "00000000-0000-0000-0000-000000000000",
					PointTagGuid: tagGuid,
					PointTemplateGuid: pointTemplateGuid,
					View: true,
					Modify: true,
					ExceedRange: true,
					Override: true
				});
			}
		}
		else {
			var selectedPointAccessGroupToPointTagAssignment = jQuery.grep(pointGroupAccess.PointAccessGroupToPointTagAssignmentList, function (a) {
				return a.PointTagGuid === tagGuid;
			});
			if (selectedPointAccessGroupToPointTagAssignment.length === 0) {
				pointGroupAccess.PointAccessGroupToPointTagAssignmentList.push({
					PointAccessGroupToPointTagGuid: "00000000-0000-0000-0000-000000000000",
					PointTagGuid: tagGuid,
					View: true,
					Modify: true,
					ExceedRange: true,
					Override: true
				});
			}
		}
	});

	$("#alarmList li").each(function (index, alarm) {
		var alarmTestGuid = $(this).attr('data-guid');
		var pointTemplateGuid = $(this).attr('data-template-guid');
		var pointGuid = $(this).attr('data-point-guid');
		var isDeviceAlarmMapTag = $(this).attr('data-is-device-alarm-map-tag');

		if (isDeviceAlarmMapTag === "False") {
			var selectedPointAccessGroupToAlarmTestAssignment = jQuery.grep(pointGroupAccess.PointAccessGroupToAlarmTestAssignmentList, function (a) {
				return a.PointTemplateGuid === pointTemplateGuid && a.AlarmTestTemplateGuid === alarmTestGuid;
			});
			if (selectedPointAccessGroupToAlarmTestAssignment.length === 0) {
					pointGroupAccess.PointAccessGroupToAlarmTestAssignmentList.push({
					PointAccessGroupToAlarmTestGuid: "00000000-0000-0000-0000-000000000000",
					PointTemplateGuid: pointTemplateGuid,
					AlarmTestTemplateGuid: alarmTestGuid,
					View: true,
					Acknowledge: true
				});
			}
		}
		else {
			var selectedPointAccessGroupToPointAlarmTestAssignment = jQuery.grep(pointGroupAccess.PointAccessGroupToPointAlarmTestAssignmentList, function (a) {
				return a.AlarmTestGuid === alarmTestGuid && a.PointGuid === pointGuid;
			});
			if (selectedPointAccessGroupToPointAlarmTestAssignment.length === 0) {
				pointGroupAccess.PointAccessGroupToPointAlarmTestAssignmentList.push({
					PointAccessGroupToPointAlarmTestGuid: "00000000-0000-0000-0000-000000000000",
					PointGuid: pointGuid,
					AlarmTestGuid: alarmTestGuid,
					View: true,
					Acknowledge: true
				});
			}
		}
	});
};

FMPointAccess.ResetDrawPointGroupAccess = function () {
	$(".checkbox-template-access").prop("checked", false);
	$(".checkbox-template-access + span").attr("title", "");

	$(".checkbox-point-access").prop("checked", false);
	$(".checkbox-point-access + span").attr("title", "");

	$(".checkbox-setting-view").prop("checked", false);
	$(".checkbox-setting-view + span").attr("title", "");

	$(".checkbox-setting-modify").prop("checked", false);
	$(".checkbox-setting-modify + span").attr("title", "");

	$(".checkbox-tag-view").prop("checked", false);
	$(".checkbox-tag-view + span").attr("title", "");

	$(".checkbox-tag-modify").prop("checked", false);
	$(".checkbox-tag-modify + span").attr("title", "");

	$(".checkbox-tag-exceed").prop("checked", false);
	$(".checkbox-tag-exceed + span").attr("title", "");

	$(".checkbox-tag-override").prop("checked", false);
	$(".checkbox-tag-override + span").attr("title", "");

	$(".checkbox-alarm-view").prop("checked", false);
	$(".checkbox-alarm-view + span").attr("title", "");

	$(".checkbox-alarm-ack").prop("checked", false);
	$(".checkbox-alarm-ack + span").attr("title", "");
}

FMPointAccess.DrawPointGroupAccess = function (pointGroupAccess, userGroupName) {

	if (pointGroupAccess.hasOwnProperty('PointAccessGroupToPointTemplateAssignmentList')) {
		$.each(pointGroupAccess.PointAccessGroupToPointTemplateAssignmentList, function (index, pointTemplateAssignment) {
			if (pointTemplateAssignment.Assigned === true) {
				$("#ck-pt-all_" + pointTemplateAssignment.PointTemplateGuid).prop("checked", true);
				// set the tooltip
				if (FMPointAccess.screenMode === "UserGroupView" && pointTemplateAssignment.Assigned === true) {
					var tooltip = $("#ck-pt-all-display_" + pointTemplateAssignment.PointTemplateGuid).attr("title");
					$("#ck-pt-all-display_" + pointTemplateAssignment.PointTemplateGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && pointTemplateAssignment.Assigned === true) {
					var tooltip = $("#ck-pt-all-display_" + pointTemplateAssignment.PointTemplateGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {

						$("#ck-pt-all-display_" + pointTemplateAssignment.PointTemplateGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}
		});
	}

	if (pointGroupAccess.hasOwnProperty('PointAccessGroupToPointAssignmentList')) {
		$.each(pointGroupAccess.PointAccessGroupToPointAssignmentList, function (index, pointAssignment) {
			if (pointAssignment.Assigned === true) {
				$("#ck-p-access_" + pointAssignment.PointGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && pointAssignment.Assigned === true) {
					var tooltip = $("#ck-p-access-display_" + pointAssignment.PointGuid).attr("title");
					$("#ck-p-access-display_" + pointAssignment.PointGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && pointAssignment.Assigned === true) {
					var tooltip = $("#ck-p-access-display_" + pointAssignment.PointGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-p-access-display_" + pointAssignment.PointGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}
		});
	}

	if (pointGroupAccess.hasOwnProperty('PointAccessGroupToSettingAssignmentList')) {
		$.each(pointGroupAccess.PointAccessGroupToSettingAssignmentList, function (index, settingAssignment) {
			if (settingAssignment.View === false && $("#ck-s-view_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).is(':checked') === false) {
				$("#ck-s-view_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).prop("checked", false);
			}
			else {
				$("#ck-s-view_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && settingAssignment.View === true) {
					var tooltip = $("#ck-s-view-display_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).attr("title");
					$("#ck-s-view-display_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && settingAssignment.View === true) {
					var tooltip = $("#ck-s-view-display_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-s-view-display_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}

			if (settingAssignment.Modify === false && $("#ck-s-modify_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).is(':checked') === false) {
				$("#ck-s-modify_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).prop("checked", false);
			}
			else {
				$("#ck-s-modify_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && settingAssignment.Modify === true) {
					var tooltip = $("#ck-s-modify-display_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).attr("title");
					$("#ck-s-modify-display_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && settingAssignment.Modify === true) {
					var tooltip = $("#ck-s-modify-display_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-s-modify-display_" + settingAssignment.ExposedSettingGuid + "_" + settingAssignment.PropertyID).attr("title", tooltip + "\n" + userGroupName);
					}
				}

			}
		});
	}

	if (pointGroupAccess.hasOwnProperty('PointAccessGroupToTagAssignmentList')) {
		$.each(pointGroupAccess.PointAccessGroupToTagAssignmentList, function (index, tagAssignment) {
			if (tagAssignment.View === false && $("#ck-t-view_" + tagAssignment.PointTagGuid).is(':checked') === false) {
				$("#ck-t-view_" + tagAssignment.PointTagGuid).prop("checked", false);
			}
			else {
				$("#ck-t-view_" + tagAssignment.PointTagGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && tagAssignment.View === true) {
					var tooltip = $("#ck-t-view-display_" + tagAssignment.PointTagGuid).attr("title");
					$("#ck-t-view-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && tagAssignment.View === true) {
					var tooltip = $("#ck-t-view-display_" + tagAssignment.PointTagGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-t-view-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}

			if (tagAssignment.Modify === false && $("#ck-t-modify_" + tagAssignment.PointTagGuid).is(':checked') === false) {
				$("#ck-t-modify_" + tagAssignment.PointTagGuid).prop("checked", false);
			}
			else {
				$("#ck-t-modify_" + tagAssignment.PointTagGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && tagAssignment.Modify === true) {
					var tooltip = $("#ck-t-modify-display_" + tagAssignment.PointTagGuid).attr("title");
					$("#ck-t-modify-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && tagAssignment.Modify === true) {
					var tooltip = $("#ck-t-modify-display_" + tagAssignment.PointTagGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-t-modify-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}

			if (tagAssignment.ExceedRange === false && $("#ck-t-exceed_" + tagAssignment.PointTagGuid).is(':checked') === false) {
				$("#ck-t-exceed_" + tagAssignment.PointTagGuid).prop("checked", false);
			}
			else {
				$("#ck-t-exceed_" + tagAssignment.PointTagGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && tagAssignment.ExceedRange === true) {
					var tooltip = $("#ck-t-exceed-display_" + tagAssignment.PointTagGuid).attr("title");
					$("#ck-t-exceed-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && tagAssignment.ExceedRange === true) {
					var tooltip = $("#ck-t-exceed-display_" + tagAssignment.PointTagGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-t-exceed-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}

			if (tagAssignment.Override === false && $("#ck-t-override_" + tagAssignment.PointTagGuid).is(':checked') === false) {
				$("#ck-t-override_" + tagAssignment.PointTagGuid).prop("checked", false);
			}
			else {
				$("#ck-t-override_" + tagAssignment.PointTagGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && tagAssignment.Override === true) {
					var tooltip = $("#ck-t-override-display_" + tagAssignment.PointTagGuid).attr("title");
					$("#ck-t-override-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && tagAssignment.Override === true) {
					var tooltip = $("#ck-t-override-display_" + tagAssignment.PointTagGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-t-override-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}
		});
	}

	if (pointGroupAccess.hasOwnProperty('PointAccessGroupToPointTagAssignmentList')) {
		$.each(pointGroupAccess.PointAccessGroupToPointTagAssignmentList, function (index, tagAssignment) {
			if (tagAssignment.View === false && $("#ck-t-view_" + tagAssignment.PointTagGuid).is(':checked') === false) {
				$("#ck-t-view_" + tagAssignment.PointTagGuid).prop("checked", false);
			}
			else {
				$("#ck-t-view_" + tagAssignment.PointTagGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && tagAssignment.View === true) {
					var tooltip = $("#ck-t-view-display_" + tagAssignment.PointTagGuid).attr("title");
					$("#ck-t-view-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && tagAssignment.View === true) {
					var tooltip = $("#ck-t-view-display_" + tagAssignment.PointTagGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-t-view-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}

			if (tagAssignment.Modify === false && $("#ck-t-modify_" + tagAssignment.PointTagGuid).is(':checked') === false) {
				$("#ck-t-modify_" + tagAssignment.PointTagGuid).prop("checked", false);
			}
			else {
				$("#ck-t-modify_" + tagAssignment.PointTagGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && tagAssignment.Modify === true) {
					var tooltip = $("#ck-t-modify-display_" + tagAssignment.PointTagGuid).attr("title");
					$("#ck-t-modify-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && tagAssignment.Modify === true) {
					var tooltip = $("#ck-t-modify-display_" + tagAssignment.PointTagGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-t-modify-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}

			if (tagAssignment.ExceedRange === false && $("#ck-t-exceed_" + tagAssignment.PointTagGuid).is(':checked') === false) {
				$("#ck-t-exceed_" + tagAssignment.PointTagGuid).prop("checked", false);
			}
			else {
				$("#ck-t-exceed_" + tagAssignment.PointTagGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && tagAssignment.ExceedRange === true) {
					var tooltip = $("#ck-t-exceed-display_" + tagAssignment.PointTagGuid).attr("title");
					$("#ck-t-exceed-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && tagAssignment.ExceedRange === true) {
					var tooltip = $("#ck-t-exceed-display_" + tagAssignment.PointTagGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-t-exceed-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}

			if (tagAssignment.Override === false && $("#ck-t-override_" + tagAssignment.PointTagGuid).is(':checked') === false) {
				$("#ck-t-override_" + tagAssignment.PointTagGuid).prop("checked", false);
			}
			else {
				$("#ck-t-override_" + tagAssignment.PointTagGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && tagAssignment.Override === true) {
					var tooltip = $("#ck-t-override-display_" + tagAssignment.PointTagGuid).attr("title");
					$("#ck-t-override-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && tagAssignment.Override === true) {
					var tooltip = $("#ck-t-override-display_" + tagAssignment.PointTagGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-t-override-display_" + tagAssignment.PointTagGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}
		});
	}



	if (pointGroupAccess.hasOwnProperty('PointAccessGroupToAlarmTestAssignmentList')) {
		$.each(pointGroupAccess.PointAccessGroupToAlarmTestAssignmentList, function (index, alarmAssignment) {
			if (alarmAssignment.View === false && $("#ck-a-view_" + alarmAssignment.AlarmTestTemplateGuid).is(':checked') === false) {
				$("#ck-a-view_" + alarmAssignment.AlarmTestTemplateGuid).prop("checked", false);
			}
			else {
				$("#ck-a-view_" + alarmAssignment.AlarmTestTemplateGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && alarmAssignment.View === true) {
					var tooltip = $("#ck-a-view-display_" + alarmAssignment.AlarmTestTemplateGuid).attr("title");
					$("#ck-a-view-display_" + alarmAssignment.AlarmTestTemplateGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && alarmAssignment.View === true) {
					var tooltip = $("#ck-a-view-display_" + alarmAssignment.AlarmTestTemplateGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-a-view-display_" + alarmAssignment.AlarmTestTemplateGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}

			if (alarmAssignment.Acknowledge === false && $("#ck-a-ack_" + alarmAssignment.AlarmTestTemplateGuid).is(':checked') === false) {
				$("#ck-a-ack_" + alarmAssignment.AlarmTestTemplateGuid).prop("checked", false);
			}
			else {
				$("#ck-a-ack_" + alarmAssignment.AlarmTestTemplateGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && alarmAssignment.Acknowledge === true) {
					var tooltip = $("#ck-a-ack-display_" + alarmAssignment.AlarmTestTemplateGuid).attr("title");
					$("#ck-a-ack-display_" + alarmAssignment.AlarmTestTemplateGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && alarmAssignment.Acknowledge === true) {
					var tooltip = $("#ck-a-ack-display_" + alarmAssignment.AlarmTestTemplateGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-a-ack-display_" + alarmAssignment.AlarmTestTemplateGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}

			}
		});
	}

	if (pointGroupAccess.hasOwnProperty('PointAccessGroupToUserGroupAssignmentList')) {
		$.each(pointGroupAccess.PointAccessGroupToUserGroupAssignmentList, function (index, userGroupAssignment) {
			if (userGroupAssignment.Assigned === true) {
				$("#ck_ug_a_" + userGroupAssignment.UserGroupGuid).prop("checked", true);
			}
		});

		if (typeof FMPointAccessUserGroups !== "undefined") {
			FMPointAccessUserGroups.PopulateBoldedGroups();
			FMPointAccessUserGroups.ApplyUserFade();
		}
	}

	if (pointGroupAccess.hasOwnProperty('PointAccessGroupToPointAlarmTestAssignmentList')) {
		$.each(pointGroupAccess.PointAccessGroupToPointAlarmTestAssignmentList, function (index, alarmAssignment) {
			if (alarmAssignment.View === false && $("#ck-a-view_" + alarmAssignment.AlarmTestGuid).is(':checked') === false) {
				$("#ck-a-view_" + alarmAssignment.AlarmTestGuid).prop("checked", false);
			}
			else {
				$("#ck-a-view_" + alarmAssignment.AlarmTestGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && alarmAssignment.View === true) {
					var tooltip = $("#ck-a-view-display_" + alarmAssignment.AlarmTestGuid).attr("title");
					$("#ck-a-view-display_" + alarmAssignment.AlarmTestGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && alarmAssignment.View === true) {
					var tooltip = $("#ck-a-view-display_" + alarmAssignment.AlarmTestGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-a-view-display_" + alarmAssignment.AlarmTestGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}

			if (alarmAssignment.Acknowledge === false && $("#ck-a-ack_" + alarmAssignment.AlarmTestGuid).is(':checked') === false) {
				$("#ck-a-ack_" + alarmAssignment.AlarmTestGuid).prop("checked", false);
			}
			else {
				$("#ck-a-ack_" + alarmAssignment.AlarmTestGuid).prop("checked", true);
				if (FMPointAccess.screenMode === "UserGroupView" && alarmAssignment.Acknowledge === true) {
					var tooltip = $("#ck-a-ack-display_" + alarmAssignment.AlarmTestGuid).attr("title");
					$("#ck-a-ack-display_" + alarmAssignment.AlarmTestGuid).attr("title", tooltip + "\n" + pointGroupAccess.Name);
				}
				else if (FMPointAccess.screenMode === "UserView" && alarmAssignment.Acknowledge === true) {
					var tooltip = $("#ck-a-ack-display_" + alarmAssignment.AlarmTestGuid).attr("title");
					if ($.inArray(userGroupName, tooltip.split("\n")) === -1) {
						$("#ck-a-ack-display_" + alarmAssignment.AlarmTesteGuid).attr("title", tooltip + "\n" + userGroupName);
					}
				}
			}
		});
	}

}

FMPointAccess.SetAssignmentNameColor = function () {
	$("#pointTemplateList li").each(function (index) {
		$(this).find('.template-name').removeClass('unselected');
		if ($(this).find(' .checkbox-template-access').prop("checked") === false) {
			$(this).find('.template-name').addClass('unselected');
		}
	});

	$("#pointList li").each(function (index) {
		$(this).find('.point-name').removeClass('unselected');
		$(this).find('.point-template-name').removeClass('unselected');
		if ($(this).find('.checkbox-point-access').prop("checked") === false) {
			$(this).find('.point-name').addClass('unselected');
			$(this).find('.point-template-name').addClass('unselected');
		}
	});

	$("#settingList li").each(function (index) {
		$(this).find('.setting-name').removeClass('unselected');
		$(this).find('.setting-template-name').removeClass('unselected');
		if ($(this).find('.checkbox-setting-view').prop("checked") === false && $(this).find('.checkbox-setting-modify').prop("checked") === false) {
			$(this).find('.setting-name').addClass('unselected');
			$(this).find('.setting-template-name').addClass('unselected');
		}
	});

	$("#tagList li").each(function (index) {
		$(this).find('.tag-name').removeClass('unselected');
		$(this).find('.tag-template-name').removeClass('unselected');
		if ($(this).find('.checkbox-tag-view').prop("checked") === false
			&& $(this).find('.checkbox-tag-modify').prop("checked") === false
			&& $(this).find('.checkbox-tag-exceed').prop("checked") === false
			&& $(this).find('.checkbox-tag-override').prop("checked") === false) {
			$(this).find('.tag-name').addClass('unselected');
			$(this).find('.tag-template-name').addClass('unselected');
		}
	});

	$("#alarmList li").each(function (index) {
		$(this).find('.alarmtest-name').removeClass('unselected');
		$(this).find('.alarmtest-template-name').removeClass('unselected');
		if ($(this).find('.checkbox-alarm-view').prop("checked") === false
			&& $(this).find('.checkbox-alarm-ack').prop("checked") === false) {
			$(this).find('.alarmtest-name').addClass('unselected');
			$(this).find('.alarmtest-template-name').addClass('unselected');
		}
	});
}

FMPointAccess.SetHeaderCheckboxes = function () {
	// Point Templates All point access
	FMPointAccess.SetHeaderCheckboxHelper($("#pointTemplateList li:not('.hidden')").length,
		$("#pointTemplateList li:not('.hidden') .checkbox-template-access:checked").length,
		"ck-pt-all");

	// Points Access
	FMPointAccess.SetHeaderCheckboxHelper($("#pointList li:not('.hidden')").length,
		$("#pointList li:not('.hidden') .checkbox-point-access:checked").length,
		"ck-p-access");

	var settingList = $("#settingList li:not('.hidden')");
	var settingListLength = settingList.length;
	var settingViewListLength = 0;
	var settingModifyListLength = 0;

	settingList.each(function (index, row) {
		settingViewListLength += (row.querySelector('.checkbox-setting-view').checked ? 1 : 0);
		settingModifyListLength += (row.querySelector('.checkbox-setting-modify').checked ? 1 : 0);
	});

	// Setting View
	FMPointAccess.SetHeaderCheckboxHelper(settingListLength, settingViewListLength, "ck-s-view");

	// Setting Modify
	FMPointAccess.SetHeaderCheckboxHelper($("#settingList li:not('.hidden') .checkbox-setting-modify:not('.disabled')").length,
		settingModifyListLength,
		"ck-s-modify");

	var tagList = $("#tagList li:not('.hidden')");
	var tagListLength = tagList.length;
	var tagViewListLength = 0;
	var tagModifyListLength = 0;
	var tagExceedListLength = 0;
	var tagOverrideListLength = 0;
	tagList.each(function (index, row) {
		tagViewListLength += (row.querySelector('.checkbox-tag-view').checked ? 1 : 0);
		tagModifyListLength += (row.querySelector('.checkbox-tag-modify').checked ? 1 : 0);
		tagExceedListLength += (row.querySelector('.checkbox-tag-exceed').checked ? 1 : 0);
		tagOverrideListLength += (row.querySelector('.checkbox-tag-override').checked ? 1 : 0);
	});

	// Tag View
	FMPointAccess.SetHeaderCheckboxHelper(tagListLength, tagViewListLength, "ck-t-view");
	// Tag Modify
	FMPointAccess.SetHeaderCheckboxHelper(tagListLength, tagModifyListLength, "ck-t-modify");
	// Tag Exceed
	FMPointAccess.SetHeaderCheckboxHelper(tagListLength, tagExceedListLength, "ck-t-exceed");
	// Tag Override
	FMPointAccess.SetHeaderCheckboxHelper(tagListLength, tagOverrideListLength, "ck-t-override");


	var alarmList = $("#alarmList li:not('.hidden')");
	var alarmListLength = alarmList.length;
	var alarmViewListLength = 0;
	var alarmAckListLength = 0;

	alarmList.each(function (index, row) {
		alarmViewListLength += (row.querySelector('.checkbox-alarm-view').checked ? 1 : 0);
		alarmAckListLength += (row.querySelector('.checkbox-alarm-ack').checked ? 1 : 0);
	});

	// Alarm View
	FMPointAccess.SetHeaderCheckboxHelper(alarmListLength, alarmViewListLength, "ck-a-view");

	// Alarm Acknowledge
	FMPointAccess.SetHeaderCheckboxHelper(alarmListLength, alarmAckListLength, "ck-a-ack");
}

FMPointAccess.SetHeaderCheckboxHelper = function (numDisplayedElements, numCheckedElements, checkboxControl) {
	if (numDisplayedElements > 0 && numCheckedElements > 0) {
		if (numDisplayedElements !== numCheckedElements) {
			// the checkbox is displayed as checked with a color background
			$("#" + checkboxControl).prop("checked", false).removeClass('partial').addClass('partial');
		}
		else {
			// the checkbox is checked
			$("#" + checkboxControl).prop("checked", true).removeClass('partial');
		}
	}
	else // nothing displayed so the checkbox should be unchecked
	{
		$("#" + checkboxControl).prop("checked", false).removeClass('partial');
	}
}

FMPointAccess.SaveChanges = function (actionOnSuccessful) {
	// hide any other notification
	FMErrorAndExceptionHandling.CloseNotifications();
	// notification position
	var messageAttributes = { addclass: 'stack-bottomright', stack: FMPointAccess.stack_bottomright };

	// display animation
	$('<div class=loadingDiv><img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif" /></img></div>').prependTo(document.body);

	var token = $('input[name=__RequestVerificationToken]').val();
	var headers = {};
	headers['__RequestVerificationToken'] = token;

	var url = FMPointAccess.Model.Length === 1 ? $("#urlUpdatePointAccessGroup").val() : $("#urlUpdatePointAccessGroupList").val();
	var data = FMPointAccess.Model.Length === 1 ? { pointAccessGroupAssignment: FMPointAccess.Model[0] } : { pointAccessGroupAssignmentList: FMPointAccess.Model };
	$.ajax({
		url: url,
		type: 'post',
		cache: false,
		headers: headers,
		dataType: 'json',
		contentType: 'application/json; charset=utf-8',
		data: JSON.stringify(data),
		success: function (response) {
			FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
				if (!inError) {
					FMPointAccess.dataChanges = false;
					if (typeof actionOnSuccessful !== 'undefined') {
						actionOnSuccessful();
					}
				}
				// hide the saving animation
				$(".loadingDiv").remove();
			}, messageAttributes);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			FMErrorAndExceptionHandling.ShowException(xhr, ajaxOptions, thrownError, function () {
				// hide the saving animation
				$(".loadingDiv").remove();
			}, messageAttributes);
		}
	});
}

FMPointAccess.NotifyFilterChanges = function () {
	if (FMPointAccess.screenMode === "UserGroupView") {
		var pointTemplateGroupsNames = [];
		$(".pointgroupfilter.active").each(function () {
			pointTemplateGroupsNames.push($(this).attr('data-name'));
		});

		if (pointTemplateGroupsNames.length > 0) {
			FMErrorAndExceptionHandling.ShowNotification("All changes will apply to the Point Access Group(s): " + pointTemplateGroupsNames.join(", ") + ".",
				function (notificationMessage) {
					notificationMessage.update({
						hide: true,
						delay: 10000 // hide after 10 seconds
					});
				});
			$("#sectionaccessrights").removeClass('readonly');
		}
		else {
			FMErrorAndExceptionHandling.CloseNotifications();
			$("#sectionaccessrights").removeClass('readonly').addClass('readonly');
		}
	}
}

FMPointAccess.UserGroupViewRedrawFilter = function () {
	FMPointAccess.ResetDrawPointGroupAccess();
	$('.pointgroupfilter.active').each(function () {
		var pointAccessGroupGuid = $(this).attr('data-guid');
		var selectedPointAccessGroup = jQuery.grep(FMPointAccess.Model, function (a) {
			return a.PointAccessGroupGuid === pointAccessGroupGuid;
		});
		if (selectedPointAccessGroup.length > 0) {
			FMPointAccess.DrawPointGroupAccess(selectedPointAccessGroup[0]);
		}
	});

	FMPointAccess.SetAssignmentNameColor();
	FMPointAccess.SetHeaderCheckboxes();
	FMPointAccess.NotifyFilterChanges();
}


FMPointAccess.UserViewRedrawFilter = function () {
	FMPointAccess.ResetDrawPointGroupAccess();
	$('.pointgroupfilter.active').each(function () {
		var userGroupName = $(this).attr('data-name');

		$.each(FMPointAccess.UserGroupToPointAccessGroupMap[userGroupName], function (index, pointAccessGroupGuid) {
			var selectedPointAccessGroup = jQuery.grep(FMPointAccess.Model, function (a) {
				return a.PointAccessGroupGuid === pointAccessGroupGuid;
			});
			if (selectedPointAccessGroup.length > 0) {
				FMPointAccess.DrawPointGroupAccess(selectedPointAccessGroup[0], userGroupName);
			}
		});

	});
	FMPointAccess.SetAssignmentNameColor();
	FMPointAccess.SetHeaderCheckboxes();
}

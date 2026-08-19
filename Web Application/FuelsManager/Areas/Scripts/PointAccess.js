
if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

FMPointAccess = {
	pointAccessTableHandle: null,
	userGroupTableHandle: null,
	userTableHandle: null,
	stack_bottomright: { "dir1": 'up', "dir2": 'left', "firstpos1": 25, "firstpos2": 25 }
};

$(document).ready(function () {

	$(".panel1").click(function () {
		$("#pointaccessgroup").removeClass('hidden');
		$("#usergroup").removeClass('hidden').addClass('hidden');
		$("#user").removeClass('hidden').addClass('hidden');
		$("#PointControlSection > div").removeClass('selected');
		$(".panel1").addClass('selected');

		FMPointAccess.pointAccessTableHandle.columns.adjust().draw();
	});

	$(".panel2").click(function () {
		$("#pointaccessgroup").removeClass('hidden').addClass('hidden');
		$("#usergroup").removeClass('hidden');
		$("#user").removeClass('hidden').addClass('hidden');
		$("#PointControlSection > div").removeClass('selected');
		$(".panel2").addClass('selected');

		FMPointAccess.userGroupTableHandle.columns.adjust().draw();
	});

	$(".panel3").click(function () {
		$("#pointaccessgroup").removeClass('hidden').addClass('hidden');
		$("#usergroup").removeClass('hidden').addClass('hidden');
		$("#user").removeClass('hidden');
		$("#PointControlSection > div").removeClass('selected');
		$(".panel3").addClass('selected');

		FMPointAccess.userTableHandle.columns.adjust().draw();
	});

	FMPointAccess.pointAccessTableHandle = $('#pointAccessGroupTable').DataTable(
	{
		"ordering": false,
		"scrollY": 'calc( 100vh - 215px)',
		"sScrollX": '100%',
		"sScrollXInner": '100%',
		"scrollCollapse": false,
		"paging": true,
		"autoWidth": true,
		"columnDefs": [
			{ "name": 'Delete', "orderable": false },
			{ "name": 'Name', "orderable": false }
		],
		"dom": '<"top"fl>rt<"bottom"p>',
		"lengthMenu": [[25, 50, -1], ["Show 25", "Show 50", "Show All"]],
		"language": {
			"search": "",
			"lengthMenu": "_MENU_",
			"paginate": {
				previous: "<",
				next: ">"
			}
			},
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
	$("#pointAccessGroupTable_wrapper #pointAccessGroupTable_filter").addClass("col-sm-4").appendTo("#pointaccessgroup .section-header-name");
	$('<img alt="Filter Tags" title="Search" src="' + window.applicationRootName +'/fmwebapp/images/Search Icon.png">').appendTo("#pointAccessGroupTable_filter label");
	$("#pointAccessGroupTable_wrapper .dataTables_length").addClass("col-sm-4 text-right").appendTo("#pointaccessgroup .section-header-name");
	$("#pointAccessGroupTable_paginate").appendTo("#pointAccessButtonsPage");
	FMPointAccess.pointAccessTableHandle.draw();

	FMPointAccess.userGroupTableHandle = $('#userGroupTable').DataTable(
		{
			"ordering": false,
			"scrollY": 'calc( 100vh - 200px)',
			"sScrollX": '100%',
			"sScrollXInner": '100%',
			"scrollCollapse": false,
			"paging": true,
			"autoWidth": true,
			"columnDefs": [
				{ "name": 'Name', "orderable": false },
				{ "name": 'Description', "orderable": false, "visible": true },
			],
			"dom": '<"top"fl>rt<"bottom"p>',
			"lengthMenu": [[25, 50, -1], ["Show 25", "Show 50", "Show All"]],
			"language": {
				"search": "",
				"lengthMenu": "_MENU_",
				"paginate": {
					previous: "<",
					next: ">"
				}
			},
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
	$("#userGroupTable_wrapper #userGroupTable_filter").addClass("col-sm-4").appendTo("#usergroup .section-header-name");
	$('<img alt="Filter Tags" title="Search" src="' + window.applicationRootName + '/fmwebapp/images/Search Icon.png">').appendTo("#userGroupTable_filter label");
	$("#userGroupTable_wrapper .dataTables_length").addClass("col-sm-4 text-right").appendTo("#usergroup .section-header-name");


	FMPointAccess.userTableHandle = $('#userTable').DataTable(
		{
			"ordering": false,
			"scrollY": 'calc( 100vh - 200px)',
			"sScrollX": '100%',
			"sScrollXInner": '100%',
			"scrollCollapse": false,
			"paging": true,
			"autoWidth": true,
			"columnDefs": [
				{ "name": 'ID', "orderable": false },
				{ "name": 'Name', "orderable": false, "visible": true },
				{ "name": 'EmailAddress', "orderable": false, "visible": true },
			],
			"dom": '<"top"fl>rt<"bottom"p>',
			"lengthMenu": [[25, 50, -1], ["Show 25", "Show 50", "Show All"]],
			"language": {
				"search": "",
				"lengthMenu": "_MENU_",
				"paginate": {
					previous: "<",
					next: ">"
				}
			},
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
	$("#userTable_wrapper #userTable_filter").addClass("col-sm-4").appendTo("#user .section-header-name");
	$('<img alt="Filter Tags" title="Search" src="' + window.applicationRootName +'/fmwebapp/images/Search Icon.png">').appendTo("#userTable_filter label");
	$("#userTable_wrapper .dataTables_length").addClass("col-sm-4 text-right").appendTo("#user .section-header-name");


	// click on point access Group table
	$('#pointAccessGroupTable').on('click', 'tbody td', function () {
		var pointGroupGuid = $(this).closest('tr').attr('data-guid');
		var pointGroupName = FMPointAccess.pointAccessTableHandle.row(this).data()[1];
		var tableDataRow = FMPointAccess.pointAccessTableHandle.row(this);

		// check which column the user clicked
		if ($(this).index() == 0) {
			// delete point access group
			FMPointAccess.DeletePointGroup(pointGroupGuid, pointGroupName, tableDataRow);
		}
		else {
			// open point access group
			var url = $(this).closest('tr').attr('data-url');
			window.top.location.search = url;
		}
	});

	// click on user Group or user table
	$('#userGroupTable, #userTable').on('click', 'tbody td', function () {
		var url = $(this).closest('tr').attr('data-url');
		window.top.location.search = url;

	});

	FMPointAccess.DeletePointGroup = function (pointAccessGroupGuid, pointAccessGroupName, tableDataRow) {
			if ($('#PointAccessModifyRight').val() == 'False') {
				return;
			}

		FMLayout.ConfirmYesNo("Do you want to delete the Point Group Access: " + pointAccessGroupName + "  ?",
			'Delete Point Group Access',
			function () {
				FMErrorAndExceptionHandling.CloseNotifications();
				// display animation
				$('<div class=loadingDiv><img src="' + window.applicationRootName +'/fmwebapp/images/loader_squares_120.gif" /></img></div>').prependTo(document.body);

				var token = $('input[name=__RequestVerificationToken]').val();
				var headers = {};
				headers['__RequestVerificationToken'] = token;

				$.ajax({
					url: $("#DeletePointAccessGroupURL").val(),
					cache: false,
					type: 'Post',
					headers: headers,
					data: { pointAccessGroupGuid: pointAccessGroupGuid },
					success: function (response) {
						FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
							if (!inError) {
								tableDataRow.remove().draw();
							}
							// hide the saving animation
							$(".loadingDiv").remove();
						});
					},
					error: function (xhr, textStatus, error) {
						FMErrorAndExceptionHandling.ShowException(xhr, textStatus, error, function () { });
						// hide the saving animation
						$(".loadingDiv").remove();
					}

				});
			});
	}

	$("#addPointAccessGroup").on('click', function () {

		$("#PointAccessGroupNewScreen #PointAccessGroupNewName").val('');

		$('body').modalmanager('loading');
		$("#PointAccessGroupNewScreen").modal("show");
		$("#PointAccessGroupNewScreen #PointAccessGroupAddButton").off('click');

		$("#PointAccessGroupNewScreen #PointAccessGroupAddButton").on('click', function () {
			// Try to save the point access group
			var pointAccessGroupName = $("#PointAccessGroupNewScreen #PointAccessGroupNewName").val();

			// make sure we have a name
			if (pointAccessGroupName === "") {
				$("#PointAccessGroupNewScreen #PointAccessGroupNewName").parent().addClass('has-error');
				return false;
			}

			var messageAttributes = { addclass: 'stack-bottomright', stack: FMPointAccess.stack_bottomright };

			// remove previous notifications
			PNotify.removeStack(FMPointAccess.stack_bottomright);
			$('<div id="loaderpointgroupmain" class="LoadingAnimation"> <img src="' + window.applicationRootName +'/fmwebapp/images/loader_squares_120.gif"></div>').appendTo('body');

			var token = $('input[name =__RequestVerificationToken]').val();
			var headers = {};
			headers['__RequestVerificationToken'] = token;

			$.ajax({
				url: $("#AddPointAccessGroupURL").val(),
				type: 'Post',
				dataType: 'json',
				contentType: "application/json",
				headers: headers,
				cache: false,
				traditional: true,
				data: JSON.stringify({ "id": pointAccessGroupName }),
				success: function (response) {
					$("#loaderpointgroupmain").remove();
					FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
						if (!inError) {
							// if there is a duplicate show an error and go to the first page
							if (data.duplicateFound) {
								FMLayout.Alert("There is already a Point Access Group with the same Name.", "Duplicate", null);
							}
							else {
								$("#PointAccessGroupNewScreen").modal("hide");
								var newUrl = "?target=../InventoryManagement/PointAccess/PointAccessGroupDetail/" + data.PointAccessGroupGuid + "&CSRFToken=" + $("#CSRFToken").val();
								window.top.location.search = newUrl;
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

	});

});

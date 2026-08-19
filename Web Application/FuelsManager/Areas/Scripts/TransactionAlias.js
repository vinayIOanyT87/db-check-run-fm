// create a class with helper functions for the point editor
if (!window.applicationRootName) {
	let p = window.location.pathname.indexOf('/', 1);
	let p0 = window.location.pathname.indexOf('/(S(', 1);
	let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
	debugger;
	window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

var FMTransactionAlias = function () {
	math.config({ precision: 17 });
	var _valuesChanged = false;
	var localStorageKey = "FMTransactionAlias_";
	var _fieldList = [];
	var _notification_stack = FMErrorAndExceptionHandling.stack_bottomright;
	var _notification_class = "stack-bottomright ui-pnotify-translucent";


	var _saveChangesSuccessful = function (actionOnSuccessful, inError) {
		// hide the saving animation
		$(".loadingDiv").remove();

		// to determine if we have a succesful save we can check if the error panel is displayed
		if (!inError) {
			FMTransactionAlias.valuesChanged = false;
			actionOnSuccessful();
		}
	}


	var _saveChanges = function (action, method, actionOnSuccessful) {

		// hide any other notification
		FMErrorAndExceptionHandling.CloseNotifications();

		// display animation
		$('<div class=loadingDiv><img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif" /></img></div>').prependTo(document.body);
		var notificationAttributes = { addclass: FMTransactionAlias.notification_class, stack: FMTransactionAlias.notification_stack, width: (FMTransactionAlias.notification_stack === FMErrorAndExceptionHandling.stack_bar_top || FMTransactionAlias.notification_stack === FMErrorAndExceptionHandling.stack_bar_bottom ? "100%" : "450px") };

		$.ajax({
			url: action,
			type: method,
			headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
			contentType: 'application/x-www-form-urlencoded; charset=utf-8',
			data: (function () {
				// serialize the form
				var serializedData = $("#transactionAliasForm").serialize();

				var userGroups = [];
				$('#userGroupList .TAUserGroupSelected').each(function () {
					userGroups.push({
						ID: $(this).attr("data-name"), Guid: $(this).attr("data-guid"), Right: $(this).find( ".edit-right").hasClass( "active" ) ? 1: 0 });
				});
				serializedData += "&userGroups=" + encodeURIComponent(JSON.stringify(userGroups));

				var excludedProductList = [];
				$('#excludedProductList .TAProductSelected').each(function () {
					excludedProductList.push({ key: $(this).attr("data-name"), value: $(this).attr("data-guid") });
				});
				serializedData += "&excludedProductGuidList=" + encodeURIComponent(JSON.stringify(excludedProductList));

				var statusList = [];
				$('#statusList .TAStatusSelected').each(function () {
					statusList.push($(this).attr("data-value"));
				});
				serializedData += "&statusList=" + encodeURIComponent(JSON.stringify(statusList));

				var defaultStatus = $('#statusList .TAStatusSelected .status-default.active').parent().parent().attr("data-value");
				serializedData += "&defaultStatus=" + (defaultStatus == null ? "" : defaultStatus);

				serializedData += "&AssociatedReport=" + $("#AssociatedReport").val();
				serializedData += "&AssociatedPreloadReport=" + $("#AssociatedPreloadReport").val();
				serializedData += "&fieldGrid=" + encodeURIComponent(JSON.stringify(_getFieldTable()));

				return serializedData;
			})(),
			success: function (response) {

				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {

					_saveChangesSuccessful(actionOnSuccessful, inError);

					if (data != null) {
					}
				}, notificationAttributes);
			},
			error: function (xhr, ajaxOptions, thrownError) {
				FMErrorAndExceptionHandling.ShowException(xhr, ajaxOptions, thrownError, function () {
					// hide the saving animation
					$(".loadingDiv").remove();
				}, notificationAttributes);
			}
		});
	}

	var _loadReports = function (section) {
			$.ajax({
				type: 'Get',
				url: $("#urlGetAllReportsURL").val(),
				dataType: "json",
				success: function (results) {

					if (results.Data && results.Data.length > 0) {
						var currenReport = $("#AssociatedReport").val();
						var currenPreloadReport = $("#AssociatedPreloadReport").val();
						if (results.Data.length > 0) {
							$("#AssociatedReport").html($("<option></option>"));
							$("#AssociatedPreloadReport").html($("<option></option>"));
							$.each(results.Data, function (index, value) {
								var report = $("<option>" + value + "</option>");
								var preloadReport = $("<option>" + value + "</option>");
								$("#AssociatedReport").append(report);
								$("#AssociatedReport").val(currenReport);
								$("#AssociatedPreloadReport").append(preloadReport);
								$("#AssociatedPreloadReport").val(currenPreloadReport);
							});
						}
					}
				},
				error: function (xhr, ajaxOptions, thrownError) {
					alert(xhr.status);
					alert(thrownError);
				}
			});

	}

	// function to generate a unique guid
	_newGuid = function () {
		function s4() {
			return Math.floor((1 + Math.random()) * 0x10000)
				.toString(16)
				.substring(1);
		}
		return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
	}

	// returns true if the value passed is a Guid
	_isGuid = function (stringToTest) {
		if (stringToTest[0] === "{") {
			stringToTest = stringToTest.substring(1, stringToTest.length - 1);
		}
		var regexGuid = /^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$/gi;
		return regexGuid.test(stringToTest);
	}

	_NewTransactionDetailSettingsView = function () {
		$("#TAReportsHeader").removeClass("hidden").addClass("hidden");
		$("#TAReports").removeClass("hidden").addClass("hidden");
		$("#TARapidDataEntryHeader").removeClass("hidden");
		$("#TARapidDataEntry").removeClass("hidden");
		$("#LimitSelectionsBasedOnHierarchyOption").removeClass("hidden").addClass("hidden");
		$("#PermitNonReferenceDataOption").removeClass("hidden").addClass("hidden");
		$("#DistributedImpactOption").removeClass("hidden").addClass("hidden");
		$("#BulkShipmentOption").removeClass("hidden").addClass("hidden");
		$("#UseComboxControlsOption").removeClass("hidden").addClass("hidden");
		$("#MultipleWeightReadingsOption").removeClass("hidden").addClass("hidden");
		$("#MultipleLineItemsOption").removeClass("hidden").addClass("hidden");
		$("#EnableAutoCompleteControlsOption").removeClass("hidden").addClass("hidden");
		$("#IncludeInDispatchOption").removeClass("hidden").addClass("hidden");
		$("#MultipleTransportLineItemsOption").removeClass("hidden").addClass("hidden");
	}

	_OldTransactionDetailSettingsView = function () {
		$("#TAReportsHeader").removeClass("hidden");
		$("#TAReports").removeClass("hidden");
		$("#TARapidDataEntryHeader").removeClass("hidden").addClass("hidden");
		$("#TARapidDataEntry").removeClass("hidden").addClass("hidden");
		$("#LimitSelectionsBasedOnHierarchyOption").removeClass("hidden");
		$("#PermitNonReferenceDataOption").removeClass("hidden");
		$("#DistributedImpactOption").removeClass("hidden");
		$("#BulkShipmentOption").removeClass("hidden");
		$("#UseComboxControlsOption").removeClass("hidden");
		$("#MultipleWeightReadingsOption").removeClass("hidden");
		$("#MultipleLineItemsOption").removeClass("hidden");
		$("#EnableAutoCompleteControlsOption").removeClass("hidden");
		$("#IncludeInDispatchOption").removeClass("hidden");
		$("#MultipleTransportLineItemsOption").removeClass("hidden");
	}

	var _initializeFieldGrid = function () {
		$("#FieldEditTableWrap").niceScroll({
			cursorwidth: '10px'
			, autohidemode: false
			, cursorcolor: "#486899"
			, background: "rgb(240, 240, 240)"
		});

		$(".field-sortable").sortable({
			stack: '.field-sortable table'
		});


	}

	var _getFieldTable = function () {

		var fieldTable = [];
		$('#FieldEditTable tr').each(function ( idx) {
			if ($(this).find('td.fieldColumnGuid').length > 0) {
				var fieldGuid = $(this).find('td.fieldColumnGuid').text().trim();
				fieldTable.push({
					identityGuid: fieldGuid == "" ? "00000000-0000-0000-0000-000000000000" : fieldGuid,
					dbName: $(this).find('td .column-field-name').text().trim(),
					displayName: $(this).find('td.fieldColumnDisplayName input').val(),
					fieldRequired: $(this).find('td.fieldColumnRequired img').attr('data-value'),
					typeName: $(this).attr('data-type'),
					displayOrder: idx,
					clearOnNew: false,
					userGroupGuid: $(this).find('td.fieldColumnUserGroup select').val(),
					userGroupID: $(this).find('td.fieldColumnUserGroup select option:selected').text().trim(),
					visibility: parseInt($(this).find('td.fieldColumnVisibility button.active').val()),
					readOnly: $(this).find('td.fieldColumnReadonly img').attr('data-value'),
					defaultValue: $(this).find('td.fieldColumnDefaultValue input').val(),
				});
			}
		});
		return fieldTable;
	}

	var _setAssociatedAliasDropdown = function () {
		var currentAlias = $("#TransTypeID").val();
		if (currentAlias == "T18_SupplyOrder") {
			$("#associatedtransactiondiv").removeClass("hidden");
			$("#AssociatedAlias option").removeAttr("hidden");
			$("#AssociatedAlias option").removeAttr("disabled");
			$("#AssociatedAlias option[data-type=Order] ").each(function (i, obj) {
				$(obj).attr("hidden", "hidden");
				$(obj).attr("disabled", "disabled");
			});

		} else if (currentAlias == "T17_Order") {
			$("#associatedtransactiondiv").removeClass("hidden");
			$("#AssociatedAlias option").removeAttr("hidden");
			$("#AssociatedAlias option").removeAttr("disabled");
			$("#AssociatedAlias option[data-type=SupplyOrder]").each(function (i, obj) {
				$(obj).attr("hidden", "hidden");
				$(obj).attr("disabled", "disabled");
			});
		} else {
			$("#associatedtransactiondiv").removeClass("hidden").addClass("hidden");
			$("#AssociatedAlias option").removeAttr("hidden");
			$("#AssociatedAlias option").removeAttr("disabled");
			$("#AssociatedAlias option[data-type=Order]").each(function (i, obj) {
				$(obj).attr("hidden", "hidden");
				$(obj).attr("disabled", "disabled");
			});
			$("#AssociatedAlias option[data-type=SupplyOrder]").each(function (i, obj) {
				$(obj).attr("hidden", "hidden");
				$(obj).attr("disabled", "disabled");
			});
		}

	}

	return {
		valuesChanged: _valuesChanged
		, saveChanges: _saveChanges
		, notification_stack: _notification_stack
		, notification_class: _notification_class
		, newGuid: _newGuid
		, isGuid: _isGuid
		, oldTransactionDetailSettingsView: _OldTransactionDetailSettingsView
		, newTransactionDetailSettingsView: _NewTransactionDetailSettingsView
		, loadReports: _loadReports
		, fieldList: _fieldList
		, initializeFieldGrid: _initializeFieldGrid
		, getFieldTable: _getFieldTable
		, setAssociatedAliasDropdown: _setAssociatedAliasDropdown
	};

}();

// Alias Field Configuration Editor
function onFieldConfigurationEdit(fieldName) {

	var _openFieldConfigurationEditor = function () {


		// create the backdrop and wait for next modal to be triggered
		$('body').modalmanager('loading');

		// close all the notifications currently openned
		FMErrorAndExceptionHandling.CloseNotifications();

		$.ajax({
			type: "GET",
			cache: false,
			url: $("#urlGetAllEquipmentTypesURL").val(),
			success: function (response) {
				FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
					if (!inError || (inError && data != null)) {
						// replace the holder with the partial view
						$('#TAEquipmentTypes').html();

						function SortByEquipmentTypedName(a, b) {
							var aName = a.Item2.toLowerCase();
							var bName = b.Item2.toLowerCase();
							return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
						}

						$.each(data.sort(SortByEquipmentTypedName), function (index, value) {
							var equipmentTypeEntry = $("<option value='" + value.Item1 + "' >" + value.Item2 + "</option >");
							$("#TAEquipmentTypes").append(equipmentTypeEntry);
						});
						$("#TAEquipmentTypes").val("");
						$('#TAEquipmentTypes').select2();
						// show the modal
						$('#FieldConfigurationEditorTitle').text(fieldName);
						$('#FieldConfigurationEditorModal').modal('show');
						$('#FieldConfigurationEditorModal input[type=button], #FieldConfigurationEditorModal input[type=submit]').removeAttr("disabled");
					}
					else {
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					}
				});
			},
			error: function (xhr, textStatus, error) {
				FMErrorAndExceptionHandling.ShowException(xhr,
					textStatus,
					error,
					function () {
						// remove the loading of the modal
						var modalManager = $("body").data("modalmanager");
						modalManager.removeLoading();
					});
			}
		});
	}

	// if the user made any changes prompt to save
	if (FMTransactionAlias.valuesChanged) {
		FMLayout.ConfirmSaveCancel($("#dialog-save-confirm"),
			$("#dialog-save-confirm").attr('title'),
			function () {
				// call the save form and tell it to open the vcf corrections if successful
				FMTransactionAlias.saveChanges($('#pointPropertiesForm').attr("action"), $('#pointPropertiesForm').attr("method"), _openFieldConfigurationEditor, '', '');
			});
	}
	// if no changes then open the modal form directly
	else {
		_openFieldConfigurationEditor();
	}
}

$(document).ready(function () {
	//Ensure that only 1 Notification Dialog Appears
	FMErrorAndExceptionHandling.OnlyOneNotification = true;

	$.ajaxSetup({
		type: 'POST',
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		headers: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
		cache: false,
		traditional: true
	});

	$("#TAGeneral").click(function () {
		// hide the panels for the other editors
		$("#TAMenuItems li").removeClass("selected");
		$("#TAGeneral").addClass("selected");
		$("#TADetailHolder").removeClass('hidden');
		$("#TAFieldsHolder").addClass('hidden');
		$("#TALayoutHolder").addClass('hidden');

		window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../Accounting/TransactionAliasDetail";
	});

	$("#TAFields").click(function () {
		// hide the panels for the other editors
		$("#TAMenuItems li").removeClass("selected");
		$("#TAFields").addClass("selected");
		$("#TADetailHolder").addClass('hidden');
		$("#TAFieldsHolder").removeClass('hidden').addClass("selected");
		$("#TALayoutHolder").addClass('hidden');

		window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../Accounting/TransactionAliasFields";
	});

	$("#TALayout").click(function () {
		$("#TAMenuItems li").removeClass("selected");
		$("#TALayout").addClass("selected");
		$("#TADetailHolder").addClass('hidden');
		$("#TAFieldsHolder").addClass('hidden');
		$("#TALayoutHolder").removeClass('hidden');

		window.parent.CurrentHelpKey = "MenuBar/FMMenuBar.aspx?target=../Accounting/TransactionAliasLayout";

	});

     // the html helper sets always the attribute 'selected' for the options (sets to false if not selected). select2 expects the selected attribute only for those selected
	$('#TATransactionType').find('option[selected=false]').removeAttr('selected');

	$('input[type=radio][name=UseTransactionDetailWithLayout]').change(function () {
		if (this.value == 'true') {
			FMTransactionAlias.newTransactionDetailSettingsView();
		}
		else if (this.value == 'false') {
			FMTransactionAlias.oldTransactionDetailSettingsView();
		}
	});

	// manually hookup to the submit the form to make sure we pass all the entries from the table
	$('#transactionAliasForm').submit(function () {
		var action = this.action;
		var method = this.method;
		FMTransactionAlias.saveChanges(action, method, function () { }, '', '');
		// it is important to return false in order to
		// cancel the default submission of the form
		// and perform the AJAX call
		return false;
	});

	$('input[type=checkbox]').on('change', function () {
		var name = $(this).attr('name');
		if ($(this).is(':checked')) {
			$('input[name= "' + name + '"]').val(true);
			$(this).val(true);
		}
		else {
			$(this).val(false);
			$('input[name= "' + name + '"]').val(false);
		}
	});

	// populate the report dropdowns
	FMTransactionAlias.loadReports();
	FMTransactionAlias.initializeFieldGrid();

	if ($('input[type=radio][name=UseTransactionDetailWithLayout]').val() == 'true') {
		FMTransactionAlias.newTransactionDetailSettingsView();
	}
	else {
		FMTransactionAlias.oldTransactionDetailSettingsView();
	}

});



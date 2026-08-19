if (!window.applicationRootName) {
  let p = window.location.pathname.indexOf('/', 1);
  let p0 = window.location.pathname.indexOf('/(S(', 1);
  let p1 = p0 > 1 ? window.location.pathname.indexOf('))/', p0) : -1;
  window.applicationRootName = (p < p1 && p > 1 ? window.location.pathname.substr(0, p1 + 2) : (p > 1 ? window.location.pathname.substr(0, p) : "/FuelsManager"));
}

var FMMovementModuleSettingsEditor = function () {
  var _inError = false;
  var _processingEndEditRow = false;
  var _ignoreBlurEvent = false;
  var _dataTableHandle = null;
  var _movementNodeList = [];
  var _NodeModelList = [];
  var _valuesChanged = false;
  var _emptyGuid = '00000000-0000-0000-0000-000000000000';
  var _stack_bottomright_movementmodulesettingseditor = { "dir1": 'up', "dir2": 'left', "firstpos1": 135, "firstpos2": 25, "context": $('#ModulePropertyEditorPropertyScreen') };
  var _cellToUpdate = "";
  var _NewRowIsAdded = false;
  var _TransferTargetBeforeCommit;
  var _TransferModeBeforeCommit;
  var _EditedRowIndex = -1;
  var _ClickedRowIndex = -1;
  var _HandlingClick = false;

  const TransferMode = {
    Inactive: 0,
    Level: 1,
    Batch: 2
  };

  const TransferDirection = {
    Source: 0,
    Destination: 1,
  };

	const ModuleType = {
		None: 0,
		StandardTank: 1,
		StandardVolume: 2,
		StandardNode: 3,
	};
  const TransferVolumeMode = {
    GrossObservedVolume: 0,
    NetStandardVolume: 1,
  };

  const Columns = {
    NODE_ID: 0,
    DIRECTION: 1,
    TRANSFER_MODE: 2,
    TRANSFER_TARGET_SETPOINT: 3,
    INDIVIDUAL_CONTROL: 4,
    GUID: 5,
    UNITS: 6,
  };

	const LEVEL_PRODUCT_TAG = "Level Product";
	const VOLUME_GROSS_OBSERVED_TAG = "Volume Gross Observed";
	const VOLUME_NET_STANDARD_TAG = "Volume Net Standard";

	function __handleClick() {

		if (FMMovementModuleSettingsEditor.IsClickedOutsideMovementTable()) {
			if (FMMovementModuleSettingsEditor.HandlingClick === true) { return; }
			FMMovementModuleSettingsEditor.HandlingClick = true;
			var input = FMMovementModuleSettingsEditor.CellToUpdate;
      if (input !== undefined && input !== "") {
				FMMovementModuleSettingsEditor.EndEditRow(null, input, false);
			}
			else {
        var nodeIdEdited = $('#MovementNodeIdEdit option:selected').text();
				if (nodeIdEdited !== undefined
					&& nodeIdEdited !== ""
					&& nodeIdEdited !== "Select a node") {
					var table = FMMovementModuleSettingsEditor.dataTableHandle;
					var row = $('#MovementNodeIdEdit').closest('tr');
					var data = FMMovementModuleSettingsEditor.dataTableHandle.row(row).data();
					var receivedTargetSetpoint;// = table.cell(rowIndex, 3).data();

					receivedTargetSetpoint = $('#editTargetSPInputId').text();
					if (receivedTargetSetpoint === '') receivedTargetSetpoint = data[3];

          var selectedNode = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeGuid === $('#MovementNodeIdEdit').val());
					if (receivedTargetSetpoint === ''
						&& FMMovementModuleSettingsEditor.NewRowIsAdded
						&& selectedNode !== undefined
            && selectedNode.ModuleType !== ModuleType.StandardNode) {
						FMLayout.ConfirmYesNo('Continuing without a Target will discard your changes. Are you sure you wish to proceed?', 'Discard Changes?', function () {
							FMMovementModuleSettingsEditor.CancelEditRow($('#MovementNodeIdEdit'), true);
							FMMovementModuleSettingsEditor.EnableDisableAddButton();
						});
					}
					else {
						FMMovementModuleSettingsEditor.EndEditRow(null, $('#MovementNodeIdEdit'), false);
					}
					FMMovementModuleSettingsEditor.EnableDisableAddButton();
				}
				else {
					FMMovementModuleSettingsEditor.CancelEditRow($('#MovementNodeIdEdit'), true);
				}
			}
		}
		setTimeout(function () {
			FMMovementModuleSettingsEditor.HandlingClick = false;
			FMMovementModuleSettingsEditor.EnableDisableAddButton();
		}, 100);
	}

  $('#movementSettingsEditorPartial').on('click', __handleClick);
  $('#MovementSetupSection').on('click', __handleClick);
  $('#MovementModuleSettingsNodeTable_wrapper').on('click', __handleClick);

  //===============================================================
  // This function is a hookup to the main property page.
  // It is called by the Save button (id = PEMPESavePropertyScreen)
  // in OperateIndex.cshtml
  //===============================================================
  var _SaveChanges = function () {
    // Update the model based on the UI changes.
    FMMovementModuleSettingsEditor.UpdateModel();

    var url = $('#urlSaveMovementModuleSettings').val();
    var token = $('#MovementModuleSettingsEditorForm input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    // notification position
    var messageAttributes = { addclass: 'stack-bottomright', stack: FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor, width: '450px' };

    // remove previous notifications
    PNotify.removeStack(FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor);

    var movementModuleSettingsEditorModelStr = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModelString();

    $.ajax({
      cache: false,
      url: url,
      type: 'POST',
      headers: headers,
      async: false,
      dataType: "json",
      contentType: 'application/json; charset=UTF-8',
      data: JSON.stringify({ 'movementModuleSettingsEditorModel': movementModuleSettingsEditorModelStr }),
      success: function (result) {
        FMErrorAndExceptionHandling.HandleMessages(result,
          function (data, inError) {
            if (!inError) {
              FMMovementModuleSettingsEditor.valuesChanged = false;
            }
          },
          messageAttributes);
      },
      error:
        function (request, status, error) {
          FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
        }
    });
  };

  //============================================================================
  // This function will call pointCaclulator to recalculate the values.
  //============================================================================
  var _CallPointCalculator = function (givenTag, givenValue, givenUnits, givenTankNodeGuid, expectedTag, expectedUnits, expectedTankNodeGuid = null) {

    var result = "";
    var url = $("#urlMovementSettingsEditorPointCalculator").val();
    var token = $('#MovementModuleSettingsEditorForm input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    // notification position
    var messageAttributes = { addclass: 'stack-bottomright', stack: FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor, width: '450px' };

    // remove previous notifications
    PNotify.removeStack(FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor);

    $.ajax({
      cache: false,
      url: url,
      type: 'POST',
      headers: headers,
      async: false,
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify({
        "givenTag": givenTag,
        "givenValue": givenValue,
        "givenUnits": givenUnits,
        "pointGuid": givenTankNodeGuid,
        "expectedTag": expectedTag,
        "expectedUnits": expectedUnits,
        "expectedPointGuid": expectedTankNodeGuid
      }),
      success: function (response) {
        FMErrorAndExceptionHandling.HandleMessages(response,
          function (data, inError) {
            if (!inError) {
              result = data;
            }
          },
          messageAttributes);
      },
      error:
        function (request, status, error) {
          FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
        }
    });
    return result;
  };

  //============================================================================
  // This function will call pointCaclulator to recalculate the values.
  //============================================================================
  var _CallPointCalculatorForInterlockedNodes = function (_nodeID, _nodeGuid, _nodeDirection, _nodeMode) {
    var result = "";
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
    var nodeIdEdited = $('#MovementNodeIdEdit option:selected').text() || _nodeID;

    var _NodeGuid, _ModuleType, _Direction, _Mode, _TransferVolMode, _Value, _Units, _Setpoint;

    var nodeModel = model.MovementNodeModelList.find(n => n.MovementNodeId === nodeIdEdited);

    _NodeGuid = $('#MovementNodeIdEdit option:selected').val() || _nodeGuid;
    _Direction = $('#DirectionEdit option:selected').val() || _nodeDirection;
    _Mode = $('#TransferModeEdit option:selected').val() || _nodeMode;
    _Setpoint = $('#editTargetSPInputId').text();

    var otherNode = model.MovementNodeModelList.find(n => n.MovementNodeId !== nodeIdEdited);

    if (nodeModel === undefined) {

      var node = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeId === nodeIdEdited);

      if (node === undefined) return;

      if (_Setpoint == node.TransferTarget)
        return otherNode.TransferTarget

      _ModuleType = node.ModuleType;
      _TransferVolMode = node.NodeTransferVolumeMode;
      _Value = _Setpoint != '' ? _Setpoint : 0;
      _Units = (_Mode == TransferMode.Level) ? node.IntLevelUnits : node.IntVolumeUnits;
    }
    else {
      _ModuleType = nodeModel.ModuleType;
      _TransferVolMode = nodeModel.NodeTransferVolumeMode;

      if (_Setpoint == '') { // read from the model
        _Value = nodeModel.TransferTarget;
        _Units = (nodeModel.TransferMode == TransferMode.Level) ? nodeModel.IntLevelUnits : nodeModel.IntVolumeUnits;
      }
      else { // row in edit mode, so use latest values
        if (_Setpoint == nodeModel.TransferTarget)
          return otherNode.TransferTarget
        _Value = _Setpoint;
        _Units = (_Mode == TransferMode.Level) ? nodeModel.IntLevelUnits : nodeModel.IntVolumeUnits;
      }
    }


    if (otherNode === undefined) {
      return;
    }

    var requestParms;

    if (_Value == 0) {
      var refValue = otherNode.TransferTarget;
      var doubleRefValue;

      if (otherNode.ModuleType == ModuleType.StandardTank && otherNode.TransferMode == TransferMode.Batch) {
        var units = (otherNode.TransferMode == TransferMode.Level) ? otherNode.IntLevelUnits : otherNode.IntVolumeUnits;
        doubleRefValue = refValue.replace(model.NumberGroupSeparator, '').replace(model.NumberDecimalSeparator, '.');
      } else {
        doubleRefValue = refValue;
      }

      requestParms = {
        refPointGuid: otherNode.MovementNodeGuid
        , refTankOrVolume: (otherNode.ModuleType == ModuleType.StandardTank)
        , refSourceOrDest: (otherNode.TransferDirection == TransferDirection.Source)
        , refLevelOrBatch: (otherNode.TransferMode == TransferMode.Level)
        , refGrossOrNet: (otherNode.NodeTransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
        , refCurrentSP: doubleRefValue
        , refUnits: (otherNode.TransferMode == TransferMode.Level) ? otherNode.IntLevelUnits : otherNode.IntVolumeUnits
        , PointGuid: _NodeGuid
        , TankOrVolume: (_ModuleType == ModuleType.StandardTank)
        , SourceOrDest: (_Direction == TransferDirection.Source)
        , LevelOrBatch: (_Mode == TransferMode.Level)
        , GrossOrNet: (_TransferVolMode == TransferVolumeMode.GrossObservedVolume)
        , CurrentValue: _Value
        , Units: _Units
      };
    }
    else {
      var refValue = _Value, otherValue = otherNode.TransferTarget;
      var doubleRefValue, doubleOtherValue;

      if (_ModuleType == ModuleType.StandardTank && _Mode == TransferMode.Batch) {
        var units = _Units;
        doubleRefValue = refValue.replace(model.NumberGroupSeparator, '').replace(model.NumberDecimalSeparator, '.');
      } else {
        doubleRefValue = refValue;
      }

      if (otherNode.ModuleType == ModuleType.StandardTank && otherNode.TransferMode == TransferMode.Batch) {
        var units = (otherNode.TransferMode == TransferMode.Level) ? otherNode.IntLevelUnits : otherNode.IntVolumeUnits;
        doubleOtherValue = otherValue.replace(model.NumberGroupSeparator, '').replace(model.NumberDecimalSeparator, '.');
      } else {
        doubleOtherValue = otherValue;
      }

      requestParms = {
        refPointGuid: _NodeGuid
        , refTankOrVolume: (_ModuleType == ModuleType.StandardTank)
        , refSourceOrDest: (_Direction == TransferDirection.Source)
        , refLevelOrBatch: (_Mode == TransferMode.Level)
        , refGrossOrNet: (_TransferVolMode == TransferVolumeMode.GrossObservedVolume)
        , refCurrentSP: doubleRefValue
        , refUnits: _Units
        , PointGuid: otherNode.MovementNodeGuid
        , TankOrVolume: (otherNode.ModuleType == ModuleType.StandardTank)
        , SourceOrDest: (otherNode.TransferDirection == TransferDirection.Source)
        , LevelOrBatch: (otherNode.TransferMode == TransferMode.Level)
        , GrossOrNet: (otherNode.NodeTransferVolumeMode == TransferVolumeMode.GrossObservedVolume)
        , CurrentValue: doubleOtherValue
        , Units: (otherNode.TransferMode == TransferMode.Level) ? otherNode.IntLevelUnits : otherNode.IntVolumeUnits
      };
    }

    var url = $("#urlMovementSettingsEditorPointCalculatorForInterlockedNodes").val();
    var token = $('#MovementModuleSettingsEditorForm input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    // notification position
    var messageAttributes = { addclass: 'stack-bottomright', stack: FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor, width: '450px' };

    // remove previous notifications
    PNotify.removeStack(FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor);

    $.ajax({
      cache: false,
      async: false,
      dataType: "json",
      contentType: "application/json; charset=utf-8",
      type: 'POST',
      cache: false,
      headers: headers,
      url: url,
      data: JSON.stringify(requestParms),
      success: function (response) {
        FMErrorAndExceptionHandling.HandleMessages(response,
          function (data, inError) {
            if (!inError) {
              result = data;
            }
          },
          messageAttributes);
      },
      error:
        function (request, status, error) {
          FMErrorAndExceptionHandling.ShowError('Point calculator has returned an error!', null, messageAttributes);
        }
    });

    return result;
  };

  //=====================================================================
  // This function initializes the movement module settings editor
  //=====================================================================
  var _Initialize = function () {
    // Initialize tabs
    //FMMovementModuleSettingsEditor.ShowUserMovementSetupSection();
    FMMovementModuleSettingsEditor.InitializeDateControls();
    FMMovementModuleSettingsEditor.InitializeRecordingSection();
    FMMovementModuleSettingsEditor.LoadData();
    FMMovementModuleSettingsEditor.GetMovementControlPoints();
    FMMovementModuleSettingsEditor.ShowHideCreateNewMovementTab();
    FMMovementModuleSettingsEditor.SetInitialTab();

    FMMovementModuleSettingsEditor.dataTableHandle = $("#MovementModuleSettingsNodeTable").DataTable(
      {
        "retrieve": true
        , "select": { style: 'single' }
        , "ordering": true
        , "orderFixed": [1, 'desc']
        , "scrollY": '248px'
        , "sScrollX": '100%'
        , "sScrollXInner": '100%'
        , "scrollCollapse": false
        , "paging": false
        , "autoWidth": true
        , "columnDefs":
          [
            { "targets": [0], "name": 'MovementNodeId', "orderable": false, className: 'text-center' }
            , { "targets": [1], "name": 'Direction', "orderable": false, className: 'text-center' }
            , { "targets": [2], "name": 'TransferMode', "orderable": false, className: 'text-center' }
            , { "targets": [3], "name": 'TransferTarget', "orderable": false, className: 'text-center' }
            , { "targets": [4], "name": 'IndividualNodeControl', "orderable": false, className: 'text-center' }
            , { "targets": [5], "name": 'MovementNodeGuid', "visible": false, "orderable": false }
            , { "targets": [6], "name": 'Units', "orderable": false, className: 'text-center' }
          ]
        , "order": [[1, 'desc']]
        , "stateSave": true
        , "dom": 'rt'
        , "fnInitComplete": function () {
          // custom scroll bars
          //$(this).parent()
          $(this)
            .niceScroll({
              cursorwidth: '10px',
              autohidemode: true,
              cursorcolor: '#486899',
              background: 'rgb(240, 240, 240)',
              horizrailenabled: false
            });
        }
      });

    setTimeout(function () {
      $("#MovementModuleSettingsNodeTable").DataTable().columns.adjust().draw();
    }, 100);

    $('#MovementModuleSettingsNodeTable tbody').on('click', 'tr', function () {
      FMMovementModuleSettingsEditor.ClickedRowIndex = FMMovementModuleSettingsEditor.dataTableHandle.row(this).index();
      $(this).toggleClass('selected');
      FMMovementModuleSettingsEditor.EnableDisableDeleteButton();
    });

    // double click to edit a row 
    $('#MovementModuleSettingsNodeTable tbody').on('dblclick', 'tr', function () {
      FMMovementModuleSettingsEditor.EditRow(this, false);
    });

    FMMovementModuleSettingsEditor.EnableDisableAddButton();

    ProcessingEndEditRow = false;
    IgnoreBlurEvent = false;
  };

  //========================================================
  // This function adds a row to the movement node table.
  //========================================================
  var _RefreshGridOnSort = function () {

    FMMovementModuleSettingsEditor.dataTableHandle.clear();

    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    model.MovementNodeModelList.map((node) => {
      FMMovementModuleSettingsEditor.dataTableHandle.row.add({
        "0": node.MovementNodeId,
        "1": node.TransferDirection === 0 ? 'Source' : 'Destination',
        "2": node.TransferMode === 1 ? 'Level' : (node.TransferMode === 2 ? 'Batch' : 'Inactive'),
        "3": node.TransferTarget,
        "4": (node.IndividualNodeControl === true || node.IndividualNodeControl === 'True') ? 'True' : 'False',
        "5": node.MovementNodeGuid,
        "6": node.Units
      });

    });

    FMMovementModuleSettingsEditor.dataTableHandle.columns.adjust().draw();
  };


  var _UpdateTargetSetpointsOnInterlockEnable = function () {

    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
    var allNodes = model.MovementNodeModelList.map(node => node.TransferDirection);
    var sourceNodes = model.MovementNodeModelList.filter(node => node.TransferDirection === TransferDirection.Source);
    var destinationNodes = model.MovementNodeModelList.filter(node => node.TransferDirection === TransferDirection.Destination);

    // Update the model
    if (allNodes.length === 2 && sourceNodes.length === 1 && destinationNodes.length === 1) {

      var receivedTargetSetpoint = FMMovementModuleSettingsEditor.CallPointCalculatorForInterlockedNodes(sourceNodes[0].MovementNodeId,
        sourceNodes[0].MovementNodeGuid,
        sourceNodes[0].TransferDirection, sourceNodes[0].TransferMode);

      if (receivedTargetSetpoint !== "") {
        for (var index = 0; index < model.MovementNodeModelList.length; index++) {
          if (model.MovementNodeModelList[index].TransferDirection == TransferDirection.Destination && sourceNodes[0].TransferMode === destinationNodes[0].TransferMode) {
            model.MovementNodeModelList[index].TransferTarget = receivedTargetSetpoint;

            FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
            break;
          }
        }

        // Update the UI
        for (var index = 0; index < FMMovementModuleSettingsEditor.dataTableHandle.rows().count(); index++) {
          var rowData = FMMovementModuleSettingsEditor.dataTableHandle.row(index).data();
          if (rowData &&
            rowData[Columns.DIRECTION] === 'Destination' &&
            sourceNodes[0].TransferMode === destinationNodes[0].TransferMode) {
            rowData[Columns.TRANSFER_TARGET_SETPOINT] = receivedTargetSetpoint;
            FMMovementModuleSettingsEditor.dataTableHandle.row(index).data(rowData).draw(false);
            break;
          }
        }
      }
    }
  }

  var _UpdateIndividualNodeControlOnInterlockEnable = function () {

    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
    var allNodes = model.MovementNodeModelList.map(node => node.TransferDirection);
    var sourceNodes = model.MovementNodeModelList.filter(node => node.TransferDirection === TransferDirection.Source);
    var destinationNodes = model.MovementNodeModelList.filter(node => node.TransferDirection === TransferDirection.Destination);

    // Update the model
    if (allNodes.length === 2 && sourceNodes.length === 1 && destinationNodes.length === 1) {
      for (var index = 0; index < model.MovementNodeModelList.length; index++) {
        model.MovementNodeModelList[index].IndividualNodeControl = 'False';
      }
      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);

      // Update the UI
      for (var index = 0; index < FMMovementModuleSettingsEditor.dataTableHandle.rows().count(); index++) {
        var rowData = FMMovementModuleSettingsEditor.dataTableHandle.row(index).data();
        if (rowData) {
          rowData[Columns.INDIVIDUAL_CONTROL] = 'False';
          FMMovementModuleSettingsEditor.dataTableHandle.row(index).data(rowData).draw(false);
        }
      }
    }
  }
  //========================================================
  // This function detetes a row from the movement node table.
  //========================================================
  var _DeleteRow = function () {
    if (FMMovementModuleSettingsEditor.NewRowIsAdded) {
      FMMovementModuleSettingsEditor.NewRowIsAdded = false;
      FMMovementModuleSettingsEditor.EditedRowIndex = -1;
    }

    FMLayout.Confirm($('#MEDeleteConfirmation').val()
      , null
      , function () {
        var deletedRows = FMMovementModuleSettingsEditor.dataTableHandle.rows('.selected');
        var data = FMMovementModuleSettingsEditor.dataTableHandle.row(deletedRows[0]).data();
        if (data) {
          // Update model with removed row.
          var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
          for (var index = 0; index < model.MovementNodeModelList.length; index++) {
            if (model.MovementNodeModelList[index].MovementNodeGuid == data[Columns.GUID]) {
              model.MovementNodeModelList.splice(index, 1);
              FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
              break;
            }
          }
          FMMovementModuleSettingsEditor.dataTableHandle.rows('.selected').remove().draw(false);
        }
        FMMovementModuleSettingsEditor.valuesChanged = true;
        FMMovementModuleSettingsEditor.EnableDisableDeleteButton();
        FMMovementModuleSettingsEditor.inError = false;

        FMMovementModuleSettingsEditor.EnableDisableAddButton();
        $('#PEMPESavePropertyScreen').removeAttr('disabled');
      });
  };

  //========================================================================
  // This function populates the movement node list of the dropdown.
  // If nothing was returned, then the only item is None.
  //========================================================================
  var _PopulateMovementNodeList = function (data) {
    FMMovementModuleSettingsEditor.NodeModelList = data;

    FMMovementModuleSettingsEditor.movementNodeList = [];

    if (data == null || data.length == 0) {
      var movementNodeObj = new object();
      movementNodeObj.NodeID = 'None';
      movementNodeObj.NodeGuid = FMMovementModuleSettingsEditor.emptyGuid;

      FMMovementModuleSettingsEditor.movementNodeList.push(movementNodeObj);
      return;
    }

    for (nextIndex = 0; nextIndex < data.length; nextIndex++) {

      var movementNodeObj = new Object();
      movementNodeObj.NodeId = data[nextIndex].MovementNodeId;
      movementNodeObj.NodeGuid = data[nextIndex].MovementNodeGuid;
      movementNodeObj.ModuleType = data[nextIndex].ModuleType;

      FMMovementModuleSettingsEditor.movementNodeList.push(movementNodeObj);
    }
  };

  //========================================================================
  // This function gets the list of movement nodes to load in the movement
  // node dropdown when in edit mode. It will populate the movement node
  // list to be used in the dropdown.
  //========================================================================
  var _GetMovementNodes = function () {
    // hide any other notification
    FMErrorAndExceptionHandling.CloseNotifications();
    var url = $('#urlGetMovementNodes').val();
    var token = $('#opcUaServersForm input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    // notification position
    var messageAttributes = { addclass: 'stack-bottomright', stack: FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor, width: '450px' };

    // remove previous notifications
    PNotify.removeStack(FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor);

    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();


    $.ajax({
      cache: false,
      url: url,
      type: 'GET',
      headers: headers,
      async: false,
      dataType: "json",
      contentType: 'application/json; charset=UTF-8',
      data: { isTemplatePoint: model.IsTemplatePoint },
      success: function (result) {
        $("#MovementNodesLoading").remove();
        FMErrorAndExceptionHandling.HandleMessages(result,
          function (data, inError) {
            if (inError == false) {
              FMMovementModuleSettingsEditor.PopulateMovementNodeList(data);
            }
          },
          messageAttributes);
      },
      error:
        function (request, status, error) {
          FMErrorAndExceptionHandling.ShowException(request, status, error, function () {
            $("#MovementNodesLoading").remove();
          }, messageAttributes);
        }
    });
  };

  //=============================================================================
  // This function get the list of movement control tags to populate the movement
  // control tag dropdown.
  //=============================================================================
  var _GetMovementControlPoints = function () {
    var url = $('#urlGetMovementControlPoints').val();
    var token = $('#opcUaServersForm input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    // notification position
    var messageAttributes = { addclass: 'stack-bottomright', stack: FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor, width: '450px' };

    // remove previous notifications
    PNotify.removeStack(FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor);

    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    $.ajax({
      cache: false,
      url: url,
      type: 'GET',
      headers: headers,
      async: false,
      dataType: "json",
      contentType: 'application/json; charset=UTF-8',
      data: { movementIdentity: model.PointGuid },
      success: function (result) {
        FMErrorAndExceptionHandling.HandleMessages(result,
          function (data, inError) {
            if (inError == false) {
              FMMovementModuleSettingsEditor.PopulateMovementControlTagDropdown(data);
            }
          },
          messageAttributes);
      },
      error:
        function (request, status, error) {
          FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
        }
    });
  };

  //=================================================================================
  // This function is called by the UI to create a new movement.
  //=================================================================================
  var _CreateNewMovementOnClick = function (args) {
    var movementName = $("#MovementNameTb").val();

    if (movementName == null || movementName === "") {
      return;
    }

    var url = $('#urlCreateNewMovement').val();
    var token = $('#opcUaServersForm input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    // notification position
    var messageAttributes = { addclass: 'stack-bottomright', stack: FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor, width: '450px' };

    // remove previous notifications
    PNotify.removeStack(FMMovementModuleSettingsEditor.Stack_bottomright_movementmodulesettingseditor);

    $.ajax({
      cache: false,
      url: url,
      type: 'GET',
      headers: headers,
      async: false,
      dataType: "json",
      contentType: 'application/json; charset=UTF-8',
      data: { movementName: movementName },
      success: function (result) {
        FMErrorAndExceptionHandling.HandleMessages(result,
          async function (data, inError) {
            if (inError == false) {
              FMMovementModuleSettingsEditor.InitializeDateControls();
              FMMovementModuleSettingsEditor.InitializeRecordingSection();
              FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(data);
              FMMovementModuleSettingsEditor.LoadData();
              FMMovementModuleSettingsEditor.ShowUserMovementSetupSection();

              // Since we went to the Create New Movement tab first, the node table header is not set correctly.
              // This code is to set it correctly when we tab to the setup section where the table exists.
              FMMovementModuleSettingsEditor.UpdateNodeTableHeaderSizing();

              var title = data.PointId + " - " + data.PointPropertyId;
              $("#MovementModuleSettingEditorTitle").text(title);

              // Add the movement to the Summary
					var newId = $('#NewId').val()
              await FMMovementSummaryTab.AddMovementRowAsync(newId, FMOperateIndex.movementSummaryControllers[newId].getGrid(), data.PointId, data.PointGuid);
              FMOperateIndex.PersistMovementSummary(FMOperateIndex.movementSummaryControllers[newId].getActiveTab(), newId, FMOperateIndex.movementSummaryControllers[newId].getGrid());
              _RefreshGridOnSort();
            }
          },
          messageAttributes);
      },
      error:
        function (request, status, error) {
          FMErrorAndExceptionHandling.ShowException(request, status, error, null, messageAttributes);
        }
    });
  };

  //====================================================================
  // This function cancels the edit row.
  //====================================================================
  var _CancelEditRow = function (input, add) {
    var row = $(input).parent().parent();

    var data = FMMovementModuleSettingsEditor.dataTableHandle.row(row).data();
    var cell = $('>td', row);

    if (data !== undefined && data[0] !== "") {
      cell[0].innerHTML = data[0];
      cell[1].innerHTML = data[1];
      cell[2].innerHTML = data[2];
      cell[3].innerHTML = data[3];
      cell[4].innerHTML = data[4];
    }

    if (FMMovementModuleSettingsEditor.NewRowIsAdded || (data !== undefined && data[0] !== "" && data[0] === 'Select a node')) {
      var rowData = FMMovementModuleSettingsEditor.dataTableHandle.row(row).data();

      FMMovementModuleSettingsEditor.dataTableHandle.row(row).remove();
      FMMovementModuleSettingsEditor.dataTableHandle.columns.adjust().draw();

      var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

      for (var index = 0; index < model.MovementNodeModelList.length; index++) {
        var node = model.MovementNodeModelList[index];

        if (node.MovementNodeId === rowData[Columns.NODE_ID]) {
          model.MovementNodeModelList.slice(index);
          FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
          break;
        }
      }
      FMMovementModuleSettingsEditor.NewRowIsAdded = false;
      FMMovementModuleSettingsEditor.EditedRowIndex = -1;
    }

    FMMovementModuleSettingsEditor.inError = false;

    $('#PEMPESavePropertyScreen').removeAttr('disabled');
  };

  //========================================================
  // This function adds a row to the movement node table.
  //========================================================
  var _AddRow = function () {
    // Get the list of possible movement nodes for the dropdown.
    if (FMMovementModuleSettingsEditor.NodeModelList.length <= 0) {
      if (!$('#MovementNodesLoading').length) {
        $('<div id="MovementNodesLoading" class="MovementNodesLoadingDiv"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').prependTo(document.body);
        setTimeout(FMMovementModuleSettingsEditor.AddRow, 100);
        return;
      }
      else
        FMMovementModuleSettingsEditor.GetMovementNodes();
    }

    // if we are already in edit or add mode we cannot add a row
    if (FMMovementModuleSettingsEditor.NewRowIsAdded) return;

    FMMovementModuleSettingsEditor.EditedRowIndex = FMMovementModuleSettingsEditor.dataTableHandle.rows().count();

    FMMovementModuleSettingsEditor.dataTableHandle.row.add({
      "0": '',
      "1": '',
      "2": '',
      "3": '',
      "4": '',
      "5": '',
      "6": ''
    });
    FMMovementModuleSettingsEditor.NewRowIsAdded = true;
    FMMovementModuleSettingsEditor.dataTableHandle.columns.adjust().draw();
    FMMovementModuleSettingsEditor.EditRow(FMMovementModuleSettingsEditor.dataTableHandle.row(FMMovementModuleSettingsEditor.EditedRowIndex).node(), true);

    FMMovementModuleSettingsEditor.EnableDisableAddButton();
  };

  //========================================================================
  // This function is called when the user double clicks on a row in the
  // movement node table. It will place the row in edit mode by creating
  // dropdowns and input tags.
  //========================================================================

	var _EditRow = function (row, addingNewRow, nodeIdChanged = false) {

		addingNewRow = FMMovementModuleSettingsEditor.NewRowIsAdded;

		if ($("#PEMPESavePropertyScreen").is(':disabled') && !nodeIdChanged) {
			$('#MovementNodeIdEdit').focus();
			return;
		}

		$('#PEMPESavePropertyScreen').prop('disabled', true);

		var data = FMMovementModuleSettingsEditor.dataTableHandle.row(row).data();
    FMMovementModuleSettingsEditor.EditedRowIndex = FMMovementModuleSettingsEditor.dataTableHandle.row(row).index();
		var cell = $('>td', row);

		// Empty row, handle as add
		if (cell.length < 4) {
			FMMovementModuleSettingsEditor.AddRow();
			return;
		}

		var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
		var existingNodeIds = model.MovementNodeModelList.map(node => node.MovementNodeId);

		var currentNodeDirection = data[Columns.DIRECTION];
		var allNodes = model.MovementNodeModelList.map(node => node.TransferDirection);
		var sourceNodes = model.MovementNodeModelList.filter(node => node.TransferDirection === TransferDirection.Source);
		var destinationNodes = model.MovementNodeModelList.filter(node => node.TransferDirection === TransferDirection.Destination);


		var currentNodeId = data[Columns.NODE_ID];

		// Get the list of possible movement nodes for the dropdown.
		if (FMMovementModuleSettingsEditor.NodeModelList.length <= 0) {
			if (!$('#MovementNodesLoading').length) {
				$('<div id="MovementNodesLoading" class="MovementNodesLoadingDiv"> <img src="' + window.applicationRootName + '/fmwebapp/images/loader_squares_120.gif"></div>').prependTo(document.body);
				$('#PEMPESavePropertyScreen').prop('disabled', false);
				setTimeout(FMMovementModuleSettingsEditor.EditRow, 100, row, addingNewRow, nodeIdChanged);
				return;
			}
			else
				FMMovementModuleSettingsEditor.GetMovementNodes();
			}

			// Create the movement node dropdown for Cell 1.
			var movementNodeSelectStartTag = '<select id="MovementNodeIdEdit" name="MovementNodeIdEdit">\n ';
			var movementNodeSelectEndTag = '</select>\n ';
			var movementNodeOptionTags = '<option value = "00000000-0000-0000-0000-000000000000"' + (currentNodeId === '' ? ' selected' : '') + '>Select a node</option>\n ';

			for (nextIndex = 0; nextIndex < FMMovementModuleSettingsEditor.movementNodeList.length; nextIndex++) {
				if (model.InterlockSourceDestinationSetpoints
				&& FMMovementModuleSettingsEditor.movementNodeList[nextIndex].ModuleType === ModuleType.StandardNode) {
					continue;
				}

				var movementNodeGuid = FMMovementModuleSettingsEditor.movementNodeList[nextIndex].NodeGuid;
				var movementNodeId = FMMovementModuleSettingsEditor.movementNodeList[nextIndex].NodeId;

				if (movementNodeId === currentNodeId) {
					movementNodeOptionTags = movementNodeOptionTags + '<option value="' + movementNodeGuid + '" selected>' + movementNodeId + '</option>\n ';
				}
				else if (existingNodeIds.indexOf(movementNodeId) < 0) // Add to available only if not already in use
				{
					movementNodeOptionTags = movementNodeOptionTags + '<option value="' + movementNodeGuid + '">' + movementNodeId + '</option>\n ';
				}
			}

			$(cell[0]).html(movementNodeSelectStartTag + movementNodeOptionTags + movementNodeSelectEndTag);

			$('#MovementNodeIdEdit').on('change', function (e) {
				var rowIndex = $('#MovementNodeIdEdit').closest('tr').index();
				var nodeIdEdited = $('#MovementNodeIdEdit option:selected').text();

				var rowData = FMMovementModuleSettingsEditor.dataTableHandle.row(rowIndex).data(); // contains old data
				if (rowData) {
					var oldNodeId = rowData[Columns.NODE_ID]; // save old nodeId

					rowData[Columns.NODE_ID] = nodeIdEdited;
					rowData[Columns.TRANSFER_TARGET_SETPOINT] = "";
					FMMovementModuleSettingsEditor.dataTableHandle.row(rowIndex).data(rowData).draw(false);

					// remove from the model
					var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
					const index = model.MovementNodeModelList.findIndex(n => n.MovementNodeId === oldNodeId);
					if (index >= 0) {
						model.MovementNodeModelList.splice(index, 1);
						FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
					}
					FMMovementModuleSettingsEditor.EditRow(FMMovementModuleSettingsEditor.dataTableHandle.row(rowIndex).node(), (oldNodeId === ""), true);
				}

				FMMovementModuleSettingsEditor.EnableDisableAddButton();

				return true;
			});

			// Terminate the edit if user presses the Esc key
			$('#MovementNodeIdEdit').on('keyup', function (e) {
				// escape key maps to keycode `27`
				if (e.keyCode === 27) {
					FMMovementModuleSettingsEditor.CancelEditRow(this, addingNewRow);
					return false;
				}
				return true;
			});


			var selectedNode = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeGuid === $('#MovementNodeIdEdit').val());
			var _NodeId = $('#MovementNodeIdEdit option:selected').text();

			if (_NodeId !== 'Select a node') {

				var node = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeId === _NodeId);
				var _ModuleTypeVal = node.ModuleType;

				var currentNodeDirection = data[Columns.DIRECTION];
				var allNodes = model.MovementNodeModelList.map(node => node.TransferDirection);
				var sourceNodes = model.MovementNodeModelList.filter(node => node.TransferDirection === TransferDirection.Source);
				var destinationNodes = model.MovementNodeModelList.filter(node => node.TransferDirection === TransferDirection.Destination);

				// Create the direction dropdown for cell 1.
				var directionSelectStartTag = '<select id="DirectionEdit" name="DirectionEdit">\n ';
				var movementNodeDirectionOptions = '';

				var selectedRowIndex = FMMovementModuleSettingsEditor.dataTableHandle.row(row).index();


				if (!model.InterlockSourceDestinationSetpoints) {// interlock not checked
					if (model.MovementNodeModelList.length === 0 // No nodes exist
						|| allNodes.length === sourceNodes.length // All existing nodes are Sources
						|| allNodes.length === destinationNodes.length // All existing nodes are Destinations
						|| (sourceNodes.length === destinationNodes.length && sourceNodes.length === 1)) { // One Source & One Destination
						movementNodeDirectionOptions = '<option value="0"' + (currentNodeDirection === "Source" ? " selected" : "") + '>Source</option>\n ';
						movementNodeDirectionOptions += '<option value="1"' + (currentNodeDirection === "Destination" ? " selected" : "") + '>Destination</option>\n ';
					}
					else if (addingNewRow) {
						if (sourceNodes.length === 1) { // We have a Source then only allow more Destinations to be added (One-To-Many)
							movementNodeDirectionOptions = '<option value="1"' + (currentNodeDirection === "Destination" ? " selected" : "") + '>Destination</option>\n ';
						}
						else if (destinationNodes.length === 1) { // We have a Destination then only allow more Sources to be added (Many-To-One)
							movementNodeDirectionOptions = '<option value="0"' + (currentNodeDirection === "Source" ? " selected" : "") + '>Source</option>\n ';
						}
					}
					else {	// Editing an existing row
						if (currentNodeDirection === "Destination" && sourceNodes.length >= 1) { // If editing a Destination row and we have a Source then keep it as a Destination
							movementNodeDirectionOptions = '<option value="1"' + (currentNodeDirection === "Destination" ? " selected" : "") + '>Destination</option>\n ';
						}
						else if (currentNodeDirection === "Source" && destinationNodes.length >= 1) { // If editing a Source row and we have a Destination then keep it as a Source
							movementNodeDirectionOptions = '<option value="0"' + (currentNodeDirection === "Source" ? " selected" : "") + '>Source</option>\n ';
						}
						else if ((currentNodeDirection === "Destination" && sourceNodes.length === 0)	// If editing a Destination row and we don't have a Source yet
							|| (currentNodeDirection === "Source" && destinationNodes.length === 0)) {  // If editing a Source row and we don't have a Destination yet
							movementNodeDirectionOptions = '<option value="0"' + (currentNodeDirection === "Source" ? " selected" : "") + '>Source</option>\n ';
							movementNodeDirectionOptions += '<option value="1"' + (currentNodeDirection === "Destination" ? " selected" : "") + '>Destination</option>\n ';
						}
					}
				}
				else // interlock checked
				{
					if (addingNewRow) { // adding
						if (allNodes.length === 0)	// first node
						{
							movementNodeDirectionOptions = '<option value="0"' + (currentNodeDirection === "Source" ? " selected" : "") + '>Source</option>\n ';
							movementNodeDirectionOptions += '<option value="1"' + (currentNodeDirection === "Destination" ? " selected" : "") + '>Destination</option>\n ';
						}
						else if (sourceNodes.length === 1) // Source node exists
							movementNodeDirectionOptions = '<option value="1"' + (currentNodeDirection === "Destination" ? " selected" : "") + '>Destination</option>\n ';
						else //Destination node exists
							movementNodeDirectionOptions = '<option value="0"' + (currentNodeDirection === "Source" ? " selected" : "") + '>Source</option>\n ';
					}
					else { // editing
						if (currentNodeDirection === "Source" && destinationNodes.length == 1) // Editing a source node and we have another node that is a destination
							movementNodeDirectionOptions = '<option value="0"' + (currentNodeDirection === "Source" ? " selected" : "") + '>Source</option>\n ';
						else if (currentNodeDirection === "Destination" && sourceNodes.length === 1) // Editing a destination node and we have another node that is a source
							movementNodeDirectionOptions = '<option value="1"' + (currentNodeDirection === "Destination" ? " selected" : "") + '>Destination</option>\n ';
						else { // editing the only source node or the only destination node
							movementNodeDirectionOptions = '<option value="0"' + (currentNodeDirection === "Source" ? " selected" : "") + '>Source</option>\n ';
							movementNodeDirectionOptions += '<option value="1"' + (currentNodeDirection === "Destination" ? " selected" : "") + '>Destination</option>\n ';
						}
					}
				}

				var directionSelectEndTag = '</select>\n ';

				$(cell[1]).html(directionSelectStartTag + movementNodeDirectionOptions + directionSelectEndTag);


				// Create the transferMode dropdown for cell 2.
				var currentTransferMode = data[Columns.TRANSFER_MODE];

				$(cell[2]).html(FMMovementModuleSettingsEditor.GetTransferModesBasedOnSelectedNodeType(currentTransferMode));

				// Create an input tag for set point cell 3.
				if (_ModuleTypeVal !== ModuleType.StandardNode) {
					var currentTransferTarget = data[Columns.TRANSFER_TARGET_SETPOINT];

					if (model.InterlockSourceDestinationSetpoints) {
						if (selectedRowIndex === 1 || model.MovementNodeModelList.length >= 1) {
							var editedNode = model.MovementNodeModelList.find(n => n.MovementNodeId === currentNodeId);

							if (editedNode === undefined || editedNode.TransferTarget !== currentTransferTarget) {
								var calculatedTransferTarget = FMMovementModuleSettingsEditor.CallPointCalculatorForInterlockedNodes();

								if (calculatedTransferTarget !== "") {
									var node = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeId === _NodeId);
									node.TransferTarget = calculatedTransferTarget;
									var _ModuleTypeVal = node.ModuleType;
									var _DirectionVal = $('#DirectionEdit option:selected').val();
									var _ModeVal = $('#TransferModeEdit option:selected').val();

									currentTransferTarget = calculatedTransferTarget;

									if (_ModuleTypeVal == ModuleType.StandardTank &&
									_DirectionVal == TransferDirection.Source &&
									_ModeVal == TransferMode.Batch && calculatedTransferTarget.charAt(0) != '-')
										currentTransferTarget = '-' + calculatedTransferTarget;
								}
							}
						}
					}


					var targetTag = '<label id="editTargetSPInputId">' + currentTransferTarget + '</label>&nbsp;&nbsp;<span class="glyphicon glyphicon-pencil editTargetSPValue" />';
					$(cell[Columns.TRANSFER_TARGET_SETPOINT]).html(targetTag);
				}

				var currentIndividualNodeControl = data[Columns.INDIVIDUAL_CONTROL];

				// Create a checkbox for individual node control cell 4.
				var individualNodeControlTag = '<input id="IndividualNodeControlEdit" type="checkbox"' + (currentIndividualNodeControl === 'True' ? ' checked' : '');
				individualNodeControlTag += model.InterlockSourceDestinationSetpoints ? ' disabled' : '';
				individualNodeControlTag += '>';

				$(cell[Columns.INDIVIDUAL_CONTROL]).html(individualNodeControlTag);

				var selectedTransferMode = $('#TransferModeEdit').find("option:selected").text();
				var units = selectedNode && String((selectedTransferMode === "Level") ? selectedNode.LevelProductUnits : selectedNode.VolumeUnits);
				var unitsTag = '<label id="lblUnits">' + units + '</label>';

				$(cell[5]).html(unitsTag);


				$('#MovementNodeIdEdit').on('blur', function (e) {
					var that = this;
					setTimeout(function () { FMMovementModuleSettingsEditor.HandleDropdownBlur(null, that, addingNewRow) }, 100);
				});

				$('#DirectionEdit').on('keyup', function (e) {
					// escape key maps to keycode `27`
					if (e.keyCode === 27) {
						FMMovementModuleSettingsEditor.CancelEditRow(this, addingNewRow);
						return false;
					}

					return true;
				});

				$('#DirectionEdit').on('blur', function (e) {
					var that = this;
					setTimeout(function () { FMMovementModuleSettingsEditor.HandleDropdownBlur(null, that, addingNewRow) }, 100);
				});

				$('#TransferModeEdit').on('keyup', function (e) {
					// escape key maps to keycode `27`
					if (e.keyCode === 27) {
						FMMovementModuleSettingsEditor.CancelEditRow(this, addingNewRow);
						return false;
					}

					return true;
				});

				$('#TransferModeEdit').on('blur', function (e) {
					var that = this;
					setTimeout(function () { FMMovementModuleSettingsEditor.HandleDropdownBlur(null, that, addingNewRow) }, 100);
				});

				// Terminate the edit if user presses the Esc key
				$('#IndividualNodeControlEdit').on('keyup', function (e) {
					// escape key maps to keycode `27`
					if (e.keyCode === 27) {
						FMMovementModuleSettingsEditor.CancelEditRow(this, addingNewRow);
						return false;
					}

					return true;
				});

				$('#IndividualNodeControlEdit').on('blur', function (e) {
					if (!FMMovementModuleSettingsEditor.IgnoreBlurEvent) {
						if ($('#TargetEdit').val() != '') {
							FMMovementModuleSettingsEditor.EndEditRow(e, this, addingNewRow);
						}
					}
					return false;
				});
			}

    if (model.IsActive) {
      //disable some edit controls when the movement is active.
      $('#MovementNodeIdEdit').removeAttr('disabled').attr("disabled", true);
      $('#DirectionEdit').removeAttr('disabled').attr("disabled", true);
      $('#TransferModeEdit').removeAttr('disabled').attr("disabled", true);
      $('#IndividualNodeControlEdit').removeAttr('disabled').attr("disabled", true);
    }

		window.setTimeout(function () {
			$("#MovementModuleSettingsNodeTable").DataTable().columns.adjust().draw();
			$('#MovementNodeIdEdit').focus();
		}, 50);
	};

  //=========================================================================
  // This function is called to save the edit row into the model and change
  // the row back to labels.
  //=========================================================================
	var _EndEditRow = function (event, input, add) {

    var nodeIdEdited, receivedTargetSetpoint;
    if (event && event.shiftKey && event.keyCode === 9) {
      return;
    }

    // only run this method once, don't call it again if we are already in the middle of processing
    if (FMMovementModuleSettingsEditor.processingEndEditRow === true) {
      return;
    }

    FMMovementModuleSettingsEditor.processingEndEditRow = true;

    FMMovementModuleSettingsEditor.inError = false;
    var row = $(input).closest('tr'); // Input > TD > TR
    var selectedIndex = row.index();
    var activeNodeModel = null;

    if (selectedIndex >= 0) {	// if row still exists then commit
      var data = FMMovementModuleSettingsEditor.dataTableHandle.row(row).data();
      var editExistingRow = true;

      if (data) {

        // locate the selected Node
        for (index = 0; index < FMMovementModuleSettingsEditor.NodeModelList.length; index++) {
          if (FMMovementModuleSettingsEditor.NodeModelList[index].MovementNodeId === data[Columns.NODE_ID]) {
            activeNodeModel = FMMovementModuleSettingsEditor.NodeModelList[index];
            break;
          }
        }

        receivedTargetSetpoint = $('#editTargetSPInputId').text();
        if (data[Columns.NODE_ID] === "") {
          var cells = $('>td', row);
          editExistingRow = false;
        }
        else {
          if (receivedTargetSetpoint === '') receivedTargetSetpoint = data[Columns.TRANSFER_TARGET_SETPOINT];
          if (receivedTargetSetpoint === '') receivedTargetSetpoint = $(data[Columns.TRANSFER_TARGET_SETPOINT]).selector;
        }

        if (activeNodeModel !== null) {
          data[Columns.NODE_ID] = _nodeId = nodeIdEdited = $('#MovementNodeIdEdit option:selected').text();
          data[Columns.DIRECTION] = $('#DirectionEdit option:selected').text();
          data[Columns.TRANSFER_MODE] = _nodeMode = $('#TransferModeEdit option:selected').text();
          data[Columns.TRANSFER_TARGET_SETPOINT] = receivedTargetSetpoint;

          _nodeDirection = $('#DirectionEdit option:selected').val();
          _nodeMode = $('#TransferModeEdit option:selected').val();

          data[Columns.INDIVIDUAL_CONTROL] = ($('#IndividualNodeControlEdit').prop('checked')) ? "True" : "False";
          data[Columns.GUID] = _nodeGuid = $('#MovementNodeIdEdit option:selected').val();
          data[Columns.UNITS] = $('#lblUnits').text();

          var interlockSourceDestSetpoints = $("#InterlockCB").is(":checked");

          model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
          // if interlocked, verify that there are no error prior to commit
          if (interlockSourceDestSetpoints && editExistingRow) {

            var totalRows = FMMovementModuleSettingsEditor.dataTableHandle.rows().count();
            if (totalRows === 2) {
              var dtOtherRowIndex;
              var table = FMMovementModuleSettingsEditor.dataTableHandle;
              var indexes = table.rows().eq(0).filter(function (rowIdx) {
                return table.cell(rowIdx, 0).data() !== nodeIdEdited ? true : false;
              });

              dtOtherRowIndex = indexes[0];

              // get dataTable row & update TransferTarget
              var otherRowData = FMMovementModuleSettingsEditor.dataTableHandle.row(dtOtherRowIndex).data();

              var otherNodeModel = model.MovementNodeModelList.find(n => n.MovementNodeId === otherRowData[Columns.NODE_ID]);

              var computedTargetSetpoint = '';

              if (otherNodeModel !== null && otherNodeModel !== undefined) {
                computedTargetSetpoint = FMMovementModuleSettingsEditor.CallPointCalculatorForInterlockedNodes();//_nodeId, _nodeGuid, _nodeDirection, _nodeMode);

                if (computedTargetSetpoint !== "") {
                  // Update the model row
                  if (otherNodeModel.ModuleType == ModuleType.StandardTank &&
                    otherNodeModel.TransferDirection == TransferDirection.Source &&
                    otherNodeModel.TransferMode == TransferMode.Batch) {
                    if (computedTargetSetpoint.charAt(0) != '-')
                      computedTargetSetpoint = '-' + computedTargetSetpoint;
                  }

                  otherNodeModel.TransferTarget = computedTargetSetpoint;
                  var nodeIndex = model.MovementNodeModelList.findIndex(n => n.MovementNodeId === otherRowData[Columns.NODE_ID]);
                  model.MovementNodeModelList[nodeIndex] = otherNodeModel;
                  FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
                }
                else {
                  FMMovementModuleSettingsEditor.processingEndEditRow = false; // terminate processing
                  return;
                }
              }

              // Update the UI
              if (computedTargetSetpoint !== "" && otherRowData) {
                otherRowData[Columns.TRANSFER_TARGET_SETPOINT] = computedTargetSetpoint;
                FMMovementModuleSettingsEditor.dataTableHandle.row(dtOtherRowIndex).data(otherRowData).draw(false);
              }
            }
          }

          var cell = $('>td', row);

          cell[0].innerHTML = data[Columns.NODE_ID];		// 0 - NodeId
          cell[1].innerHTML = data[Columns.DIRECTION];	// 1 - Direction
          cell[2].innerHTML = data[Columns.TRANSFER_MODE]; // 2 - Mode
          cell[3].innerHTML = receivedTargetSetpoint; // 3 - Target SP
          cell[4].innerHTML = data[Columns.INDIVIDUAL_CONTROL]; // 4 - Individual control
          cell[5].innerHTML = data[Columns.UNITS]; // 6 - Units

          // update the table row data with any changes made
          FMMovementModuleSettingsEditor.dataTableHandle.row(row).data(data).draw(false);

          if (activeNodeModel != null) {
            activeNodeModel.TransferDirection = (data[Columns.DIRECTION] === 'Source') ? TransferDirection.Source : TransferDirection.Destination;

            activeNodeModel.TransferMode = (data[Columns.TRANSFER_MODE] === 'Level') ? TransferMode.Level :
              (data[Columns.TRANSFER_MODE] === 'Batch' ? TransferMode.Batch : TransferMode.Inactive);

            activeNodeModel.TransferModeName = data[Columns.TRANSFER_MODE];

            activeNodeModel.TransferTarget = data[Columns.TRANSFER_TARGET_SETPOINT];

            activeNodeModel.IndividualNodeControl = data[Columns.INDIVIDUAL_CONTROL];
            activeNodeModel.Units = data[Columns.UNITS];

            const position = model.MovementNodeModelList.findIndex(n => n.MovementNodeId === data[Columns.NODE_ID]);
            if (position < 0) {
              model.MovementNodeModelList.push(activeNodeModel);
            }
            else {
              model.MovementNodeModelList[position] = activeNodeModel;
            }

            model.MovementNodeModelList = model.MovementNodeModelList.sort(function (a, b) {
              return a.TransferDirection - b.TransferDirection;
            });

            FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);

            _RefreshGridOnSort();
          }
          FMMovementModuleSettingsEditor.dataTableHandle.columns.adjust().draw();
          $('#PEMPESavePropertyScreen').removeAttr('disabled');
          FMMovementModuleSettingsEditor.NewRowIsAdded = false;
          FMMovementModuleSettingsEditor.EditedRowIndex = -1;
        }
      }
    }
    FMMovementModuleSettingsEditor.processingEndEditRow = false;
    FMMovementModuleSettingsEditor.EnableDisableAddButton();
  };

  _ReCalculateTargetSetpointForANode = function () {
    var givenTag, givenUnits, givenSetpoint, expectedTag, expectedUnits;
    var TankNodeGuid;
    var selectedMode = $('#TransferModeEdit option:selected').text();
    var nodeIdEdited = $('#MovementNodeIdEdit option:selected').text();
    var nodeDirection = $('#DirectionEdit option:selected').val();
    var nodeMode = $('#TransferModeEdit option:selected').val();

    var nodeEdited = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeId === nodeIdEdited);
    var inputTagRowIndex = $('#TransferModeEdit').closest('tr').index();

    var rowData = FMMovementModuleSettingsEditor.dataTableHandle.row(inputTagRowIndex).data();
    var transferTarget;

    if (rowData) {
      transferTarget = $('#editTargetSPInputId').text();
      if (transferTarget === '') transferTarget = rowData[Columns.TRANSFER_TARGET_SETPOINT];
      if (transferTarget === '') transferTarget = $(rowData[Columns.TRANSFER_TARGET_SETPOINT]).selector;
    }

    if (transferTarget !== "" && nodeEdited) {
      if (nodeEdited.ModuleType === ModuleType.StandardTank && selectedMode === 'Level') {
        expectedTag = LEVEL_PRODUCT_TAG;
        expectedUnits = nodeEdited.IntLevelUnits;//nodeEdited.LevelProductUnits;
        TankNodeGuid = nodeEdited.MovementNodeGuid;

        givenTag = (nodeEdited.NodeTransferVolumeMode === TransferVolumeMode.GrossObservedVolume)
          ? VOLUME_GROSS_OBSERVED_TAG : VOLUME_NET_STANDARD_TAG;
        givenSetpoint = transferTarget;
        givenUnits = nodeEdited.IntVolumeUnits;//nodeEdited.VolumeUnits;
      }
      else if (nodeEdited.ModuleType === ModuleType.StandardTank && selectedMode === 'Batch') {
        expectedTag = (nodeEdited.NodeTransferVolumeMode === TransferVolumeMode.GrossObservedVolume)
          ? VOLUME_GROSS_OBSERVED_TAG : VOLUME_NET_STANDARD_TAG; // mode
        expectedUnits = nodeEdited.IntVolumeUnits;//nodeEdited.VolumeUnits;
        TankNodeGuid = nodeEdited.MovementNodeGuid;

        givenTag = LEVEL_PRODUCT_TAG;
        givenSetpoint = transferTarget;
        givenUnits = nodeEdited.IntLevelUnits;//nodeEdited.LevelProductUnits;
      }
      else if (nodeEdited.ModuleType === ModuleType.StandardVolume && selectedMode === 'Batch') {
        expectedTag = (nodeEdited.NodeTransferVolumeMode === TransferVolumeMode.GrossObservedVolume)
          ? VOLUME_GROSS_OBSERVED_TAG : VOLUME_NET_STANDARD_TAG; // mode
        expectedUnits = nodeEdited.IntVolumeUnits;//nodeEdited.VolumeUnits;
      }

      if (givenTag && givenSetpoint && givenUnits && TankNodeGuid && expectedTag && expectedUnits) {
        // Call calculator
        var receivedTargetSetpoint = FMMovementModuleSettingsEditor.CallPointCalculator(
          givenTag,
          givenSetpoint.toString(), // value
          givenUnits,
          TankNodeGuid, // PointGuid
          expectedTag,
          expectedUnits
        );
        var table = FMMovementModuleSettingsEditor.dataTableHandle;

        // add negative sign, if needed
        if (nodeEdited.ModuleType == ModuleType.StandardTank &&
          nodeDirection && nodeDirection == TransferDirection.Source &&
          nodeMode && nodeMode == TransferMode.Batch) {
          receivedTargetSetpoint = '-' + receivedTargetSetpoint.toString();
        }

        var setpointTag = '<label id="editTargetSPInputId">' + receivedTargetSetpoint + '</label>&nbsp;&nbsp;<span class="glyphicon glyphicon-pencil editTargetSPValue" />';
        table.cell(inputTagRowIndex, Columns.TRANSFER_TARGET_SETPOINT).data(setpointTag);//.draw(false); // update UI
        return receivedTargetSetpoint;
      }
      else if (nodeEdited.ModuleType === ModuleType.StandardVolume) // Standard Volume node
      {
        return "";
      }
      else
        return transferTarget;
    }
    return "";
  };

  _IsClickedOutsideMovementTable = function (e, input, addingNewRo) {
    return (document.activeElement.id !== 'MovementNodeIdEdit'
      && document.activeElement.id !== 'DirectionEdit'
      && document.activeElement.id !== 'TransferModeEdit'
      && (document.activeElement.id !== 'MovementModuleSettingsNodeTable' || (document.activeElement.id === 'MovementModuleSettingsNodeTable' && FMMovementModuleSettingsEditor.EditedRowIndex !== FMMovementModuleSettingsEditor.ClickedRowIndex))
      && document.activeElement.id !== 'IndividualNodeControlEdit');
  };

  _HandleDropdownBlur = function (e, input, addingNewRow) {
    if (FMMovementModuleSettingsEditor.IsClickedOutsideMovementTable()) {
      __handleClick();
      //			FMMovementModuleSettingsEditor.EndEditRow(e, input, addingNewRow);
    }
  };

  //===================================================
  // This function returns the movement node model as
  // a string.
  //===================================================
  _GetMovementModuleSettingsEditorModelString = function () {
    return $('#MovementModuleSettingsEditorModelStr').val();
  };

  //===================================================
  // This function returns the movement node model as
  // an object.
  //===================================================
  _GetMovementModuleSettingsEditorModel = function () {
    return JSON.parse(_GetMovementModuleSettingsEditorModelString());
  };

  //===================================================
  // This function set the movement node model as
  // a string.
  //===================================================
  _SetMovementModuleSettingsEditorModelString = function (modelStr) {
    $('#MovementModuleSettingsEditorModelStr').val(modelStr);
  };

  //===================================================
  // This function set the movement node model as
  // into a hidden tag.
  //===================================================
  _SetMovementModuleSettingsEditorModel = function (model) {
    var modelStr = JSON.stringify(model);
    _SetMovementModuleSettingsEditorModelString(modelStr);
  };

  //===============================================================================
  // This function will enable/disable the delete button.
  //===============================================================================
  _EnableDisableDeleteButton = function () {
    $('#deleteMovementNodeEntriesButton').addClass('MovementSettingsEditorDeleteButtonDisabled');

    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
    if (!model.IsActive) {
      var selectedCount = FMMovementModuleSettingsEditor.dataTableHandle.rows('.selected').count();

      $('#deleteMovementNodeEntriesButton').prop('disabled', (selectedCount === 0 || FMMovementModuleSettingsEditor.EditedRowIndex !== -1) ? 'disabled' : '');
      $('#deleteMovementNodeEntriesButton').removeClass('MovementSettingsEditorDeleteButtonDisabled');
    }
  };

	//===============================================================================
	// This function will enable/disable the add button.
	//===============================================================================
	_EnableDisableAddButton = function () {

		var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model.IsActive) {
      $('#addMovementNodeEntryButton').removeAttr('disabled').attr("disabled", true);
    }
    else
    {
      var allNodes = model.MovementNodeModelList.map(node => node.TransferDirection);
      var standardNodeNodes = model.MovementNodeModelList.map(node => node.ModuleType === ModuleType.StandardNode);
      var sourceNodes = model.MovementNodeModelList.filter(node => node.TransferDirection === TransferDirection.Source && node.ModuleType !== ModuleType.StandardNode);
      var destinationNodes = model.MovementNodeModelList.filter(node => node.TransferDirection === TransferDirection.Destination && node.ModuleType != ModuleType.StandardNode);

      var rowCount = FMMovementModuleSettingsEditor.dataTableHandle.rows().count();

      if ((allNodes.length <= 0
        && standardNodeNodes.length === 0)
        || (allNodes.length === 2 && sourceNodes.length == 1 && destinationNodes.length == 1)) {
        $("#InterlockCB").removeAttr("disabled");
      }
      else {
        $("#InterlockCB").attr("disabled", true);
      }

      $('#addMovementNodeEntryButton').removeAttr('disabled'); // Add but should always be enabled (if not editing or already adding)
      if (FMMovementModuleSettingsEditor.EditedRowIndex !== -1)
        $('#addMovementNodeEntryButton').attr("disabled", true);

      if (model.InterlockSourceDestinationSetpoints && rowCount === 2) { // Disable Add button if interlock checked and have a Source & a Destination
        $('#addMovementNodeEntryButton').attr('disabled', true);
      }
      if (allNodes.length > 0
        || FMMovementModuleSettingsEditor.EditedRowIndex != -1
        || model.InterlockSourceDestinationSetpoints) {
        $("#MovementTypeDD").attr("disabled", true);
      }
      else {
        $("#MovementTypeDD").removeAttr("disabled");
      }
    }
  };

  //=====================================================================
  // This function will load the page with data from the model.
  //=====================================================================
  _LoadData = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (!model) {
      return;
    }
    $("#InterlockCB").prop('checked', model.InterlockSourceDestinationSetpoints);
    $("#DeleteCompCB").prop('checked', model.DeleteAfterCompletion);
    $("#DeleteStopCB").prop('checked', model.DeleteAfterStop);
    $("#IncludeHandgaugeCB").prop('checked', model.IncludeHandgaugeValues);
    $("#SendToAccountingCB").prop('checked', model.SendToAccounting);
    $("#UseControlTagCB").prop('checked', model.UseControlTagStartStop);
    if (model.UseControlTagStartStop) {
      $("#ControlTagDD").removeAttr("disabled");
    }
    $("#StopHaltCB").prop('checked', model.StopHaltBasedOnZeroFlow);
    $("#StartTimeCB").prop('checked', model.StartTimeBasedOnNonZeroFlow);
    $("#SetPendingCB").prop('checked', model.SetPendingStatus);

    $("#OrderNumber").val(model.OrderNumber);
    $("#Comment").val(model.Comment);

    $("#ZeroFlowHoldTB").attr("disabled", "disabled");
    $("#ZeroFlowHoldTB").val("");

    if (model.StopHaltBasedOnZeroFlow) {
      $("#ZeroFlowHoldTB").removeAttr("disabled");
      $("#ZeroFlowHoldTB").val(model.ZeroFlowHoldOffTime);
    }

    $("#PlannedStartTimePicker").val("");
    $("#PlannedStartTimePicker").attr("disabled", "disabled");
    $("#PlannedStartTimePicker").datetimepicker("option", "disabled", true);

    if (model.SetPendingStatus) {
      $("#PlannedStartTimePicker").removeAttr("disabled");
      $("#PlannedStartTimePicker").datetimepicker("setDate", new Date(model.PlannedStartDateTime));
      $("#PlannedStartTimePicker").datetimepicker("enable");
    }

    if (model.IsActive) {
      // disable controls that should not be editable when the movement is active
      $("#MovementTypeDD").removeAttr("disabled").attr("disabled", "disabled");
      $("#InterlockCB").removeAttr("disabled").attr("disabled", "disabled");
      $("#SetPendingCB").removeAttr("disabled").attr("disabled", "disabled");
      $("#StartTimeCB").removeAttr("disabled").attr("disabled", "disabled");
      $("#UseControlTagCB").removeAttr("disabled").attr("disabled", "disabled");
    }

    FMMovementModuleSettingsEditor.SetUserControlTag();
  };

  //===========================================================================
  // This function will populate the Control Tag dropdown.
  //===========================================================================
  _PopulateMovementControlTagDropdown = function (data) {
    var selectedValue = "";
    $("#ControlTagDD").empty();

    if (data == null || data.length == 0) {
      $("#ControlTagDD").append(new Option("None", "None"));
      return;
    }

    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {
      selectedValue = model.SelectedControlTagGuid;
    }

    for (var nextIndex = 0; nextIndex < data.length; nextIndex++) {
      var text = data[nextIndex].ControlTagName;
      var value = data[nextIndex].ControlTagValue;
      $("#ControlTagDD").append(new Option(text, value));

      if (selectedValue == value) {
        $("#ControlTagDD").val(value);
      }
    }
  }

  //===================================================================
  // This function will reset the size of the node table on the
  // setup tab. This happens when another tab is set on initialization.
  //===================================================================
  var _UpdateNodeTableHeaderSizing = function () {
    // Since we went to the Create New Movement tab first, the node table header is not set correctly.
    // This code is to set it correctly when we tab to the setup section where the table exists.
    var divScrollHeaderTag = document.querySelector("#MovementModuleSettingsNodeTable_wrapper .dataTables_scrollHeadInner");
    divScrollHeaderTag.style.width = "100%";
    divScrollHeaderTag.setAttribute("id", "DivScrollHeaderId");

    var nodeTableTag = document.querySelector("#DivScrollHeaderId .table");
    nodeTableTag.style.width = "100%";
  }

  //============================================================
  // This function will handle the movement module setup 
  // section selection event.
  //============================================================
  _ShowUserMovementSetupSection = function () {
    $("#MovementRecordingItem").removeClass("selected");
    $("#MovementCreateNewMovementItem").removeClass("selected");
    $("#MovementSetupItem").addClass("selected");

    $("#MovementRecordingItemBtag").removeClass("selected");
    $("#MovementCreateNewMovementItemBtag").removeClass("selected");
    $("#MovementSetupItemBtag").addClass("selected");

    $("#MovementSetupSection").removeClass("hidden");
    $("#MovementRecordingSection").addClass("hidden");
    $("#MovementCreateNewMovementSection").addClass("hidden");

    $("#MovementSetupItem").show();
    $("#MovementRecordingItem").show();

    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    // Only do if the Create New Movement section was the initial tab.
    if (model && model.EnableCreateNewSection) {
      // Since we went to the Create New Movement tab first, the node table header is not set correctly.
      // This code is to set it correctly when we tab to the setup section where the table exists.
      FMMovementModuleSettingsEditor.UpdateNodeTableHeaderSizing();
    }
  };

  //============================================================
  // This function will handle the movement module 
  // Recording section selection event.
  //============================================================
  _ShowMovementRecordingSection = function () {
    $("#MovementSetupItem").removeClass("selected");
    $("#MovementCreateNewMovementItem").removeClass("selected");
    $("#MovementRecordingItem").addClass("selected");

    $("#MovementSetupItemBtag").removeClass("selected");
    $("#MovementCreateNewMovementItemBtag").removeClass("selected");
    $("#MovementRecordingItemBtag").addClass("selected");

    $("#MovementRecordingSection").removeClass("hidden");
    $("#MovementSetupSection").addClass("hidden");
    $("#MovementCreateNewMovementSection").addClass("hidden");
  };

  //============================================================
  // This function will handle the movement module 
  // Create New Movement section selection event.
  //============================================================
  _ShowMovementCreateNewMovementSection = function () {
    $("#MovementSetupItem").removeClass("selected");
    $("#MovementRecordingItem").removeClass("selected");
    $("#MovementCreateNewMovementItem").addClass("selected");

    $("#MovementSetupItemBtag").removeClass("selected");
    $("#MovementRecordingItemBtag").removeClass("selected");
    $("#MovementCreateNewMovementItemBtag").addClass("selected");

    $("#MovementCreateNewMovementSection").removeClass("hidden");
    $("#MovementSetupSection").addClass("hidden");
    $("#MovementRecordingSection").addClass("hidden");

    $('body').modalmanager('loading');
    var movementModuleDefaultNameUrl = $("#urlMovementModuleDefaultName").val();

    $.ajax({
      type: "GET",
      cache: false,
      url: movementModuleDefaultNameUrl,
      success: function (response, xhr, settings) {
        if (response && response.ErrorMessage) {
          var count = 0;

          if (response.ErrorMessage) {
            $.each(response.ErrorMessage, function (key, message) { count = count + 1; });
          }

          if (count === 0) {
            $("#MovementNameTb").val(response.Data);
            var modalManager = $("body").data("modalmanager");
            modalManager.removeLoading();
          }
          else {
            // remove the loading of the modal
            var modalManager = $("body").data("modalmanager");
            modalManager.removeLoading();
          }
        }
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

  };

  //==============================================================
  // This function will show/hide the create new movement tab.
  //==============================================================
  _ShowHideCreateNewMovementTab = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (!model) {
      return;
    }

    $("#MovementCreateNewMovementItem").hide();

    if (model.EnableCreateNewSection) {
      $("#MovementCreateNewMovementItem").show();
      $("#MovementSetupItem").hide();
      $("#MovementRecordingItem").hide();
    }
  };

  //=================================================================
  // This function will set the initial tab based on the the create
  // new movement setting.
  //=================================================================
  var _SetInitialTab = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (!model) {
      return;
    }

    if (model.EnableCreateNewSection) {
      FMMovementModuleSettingsEditor.ShowMovementCreateNewMovementSection();
    }
    else {
      FMMovementModuleSettingsEditor.ShowUserMovementSetupSection();
    }
  };

  //========================================================================
  // This function will update the model based on the values from the UI.
  // The checkboxes are already update on on-change events.
  //========================================================================
  _UpdateModel = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {

      model.OrderNumber = $.trim($("#OrderNumber").val());
      model.Comment = $.trim($("#Comment").val());

      model.SelectedControlTagGuid = $("#ControlTagDD option:selected").val();
      model.ZeroFlowHoldOffTime = $.trim($("#ZeroFlowHoldTB").val());

      model.PlannedStartDateTime = $.trim($("#PlannedStartTimePicker").val());

      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
    }
  };

  //==================================================================
  // This function initializes the Recording section.
  //==================================================================
  _InitializeRecordingSection = function () {
    $("#ControlTagDD").attr("disabled", true);
    $("#ZeroFlowHoldTB").attr("disabled", true);
    $("#PlannedStartTimePicker").attr("disabled", true);
    $("#PlannedStartTimePicker").datetimepicker("option", "disabled", true);

    $("#StopHaltCB").removeAttr("disabled");
    $("#StartTimeCB").removeAttr("disabled");
    $("#ControlTagDD").attr("disabled", true);
  };

	//==================================================================
	// This function handles the interlock checkbox on change event.
	// It will update the model based on the change.
	//==================================================================
	_InterLockCbOnChange = function () {
		var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

		if (model) {
			var checkboxValue = $("#InterlockCB").is(":checked");
			model.InterlockSourceDestinationSetpoints = checkboxValue;
			FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
			FMMovementModuleSettingsEditor.EnableDisableAddButton();

			if (checkboxValue) {
				FMMovementModuleSettingsEditor.UpdateTargetSetpointsOnInterlockEnable();
				FMMovementModuleSettingsEditor.UpdateIndividualNodeControlOnInterlockEnable();

				// Movement Type is Transfer for Interlocked Target
				$("#MovementTypeDD").val("0");
			}
		}
	};

  //==================================================================
  // This function handles the type selection on change event.
  // It will update the model based on the change.
  //==================================================================
  _TypeDdOnChange = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
    if (model) {
      model.Type = $("#MovementTypeDD option:selected").text().replaceAll(" ", "");
      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
    }
  };

  //==================================================================
  // This function handles the delete after completion checkbox on change event.
  // It will update the model based on the change.
  //==================================================================
  _DeleteCompletionCbOnChange = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {
      var checkboxValue = $("#DeleteCompCB").is(":checked");
      model.DeleteAfterCompletion = checkboxValue;
      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
    }
  };

  //==================================================================
  // This function handles the delete after checkbox on change event.
  // It will update the model based on the change.
  //==================================================================
  _DeleteStopCbOnChange = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {
      var checkboxValue = $("#DeleteStopCB").is(":checked");
      model.DeleteAfterStop = checkboxValue;
      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
    }
  };


  //=======================================================================
  // This function handles the include hand gauge checkbox on change event.
  // It will update the model based on the change.
  //=======================================================================
  _IncludeHandgaugeCbOnChange = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {
      var checkboxValue = $("#IncludeHandgaugeCB").is(":checked");
      model.IncludeHandgaugeValues = checkboxValue;
      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
    }
  };

  //=======================================================================
  // This function handles the send to accounting checkbox on change event.
  // It will update the model based on the change.
  //=======================================================================
  _SendToAccountingCbOnChange = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {
      var checkboxValue = $("#SendToAccountingCB").is(":checked");
      model.SendToAccounting = checkboxValue;
      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
    }
  };

  //=======================================================================
  // This function handles the use control tag checkbox on change event.
  // It will update the model based on the change. In addition, if not
  // checked it will clear the control tag dropdown.
  //=======================================================================
  _UseControlTagCbOnChange = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {
      var checkboxValue = $("#UseControlTagCB").is(":checked");
      model.UseControlTagStartStop = checkboxValue;

      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);

      $("#StopHaltCB").removeAttr("disabled");
      if (!model.IsActive) {
        $("#StartTimeCB").removeAttr("disabled");
      }
      $("#ControlTagDD").attr("disabled", true);

      if (checkboxValue) {
        $("#StopHaltCB").attr("disabled", true);
        $("#StartTimeCB").attr("disabled", true);
        $("#ControlTagDD").removeAttr("disabled");

        $("#StopHaltCB").prop('checked', false);
        $("#StartTimeCB").prop('checked', false);
        FMMovementModuleSettingsEditor.StopHaltBasedCbOnChange();
        FMMovementModuleSettingsEditor.StartTimeBasedCbOnChange();
      }
      else {
        $("#ControlTagDD").prop('selectedIndex', 0);
      }
    }
  };

  //==========================================================================================
  // This function sets the user control tag controls based on the stop and start checkboxes.
  //==========================================================================================
  _SetUserControlTag = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {
      $("#UseControlTagCB").attr("disabled", true);

      if (model.StopHaltBasedOnZeroFlow == false && model.StartTimeBasedOnNonZeroFlow == false && !model.IsActive) {
        $("#UseControlTagCB").removeAttr("disabled");;
      }
    }
  }

  //=======================================================================
  // This function handles the stop/halt checkbox on change event.
  // It will update the model based on the change.
  //=======================================================================
  _StopHaltBasedCbOnChange = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {
      var checkboxValue = $("#StopHaltCB").is(":checked");
      model.StopHaltBasedOnZeroFlow = checkboxValue;

      $("#ZeroFlowHoldTB").attr("disabled", true);

      if (checkboxValue) {
        $("#ZeroFlowHoldTB").removeAttr("disabled");
      }
      else {
        $("#ZeroFlowHoldTB").val("");
      }

      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
      FMMovementModuleSettingsEditor.SetUserControlTag();
    }
  };

  //=======================================================================
  // This function handles the start time based checkbox on change event.
  // It will update the model based on the change.
  //=======================================================================
  _StartTimeBasedCbOnChange = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {
      var checkboxValue = $("#StartTimeCB").is(":checked");
      model.StartTimeBasedOnNonZeroFlow = checkboxValue;

      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
      FMMovementModuleSettingsEditor.SetUserControlTag();
    }
  };

  //=======================================================================
  // This function handles the zero flow text box on blur event.
  //=======================================================================
  _ZeroFlowTbOnblur = function () {
    var zeroFlow = $("#ZeroFlowHoldTB").val();

    if (zeroFlow !== null && zeroFlow !== "") {
      var intValue = parseInt(zeroFlow);

      if (isNaN(intValue)) {
        $("#ZeroFlowHoldTB").val("");
      }
    }
  };

  //=======================================================================
  // This function handles the pending status on change event. It will
  // enable/disable the planned start time picker.
  //=======================================================================
  _SetPendingStatusCbOnChange = function () {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();

    if (model) {
      var checkboxValue = $("#SetPendingCB").is(":checked");
      model.SetPendingStatus = checkboxValue;

      FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
      $("#PlannedStartTimePicker").attr("disabled", true);
      $("#PlannedStartTimePicker").datetimepicker("option", "disabled", true);

      if (checkboxValue) {
        $("#PlannedStartTimePicker").removeAttr("disabled");
        $("#PlannedStartTimePicker").datetimepicker("enable");
      }
      else {
        $("#PlannedStartTimePicker").val("");
      }
    }
  };

  _GetTransferModesBasedOnSelectedNodeType = function (currentValue) {

    var selectedNode = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeGuid === $('#MovementNodeIdEdit').val());

    var transferModeSelectTags = '<select id="TransferModeEdit" name="TransferModeEdit">\n ';

    if (selectedNode !== undefined && selectedNode.ModuleType === ModuleType.StandardTank) {
      transferModeSelectTags += '<option value="1"' + (currentValue === "Level" ? " selected" : "") + '>Level</option>\n ';
      transferModeSelectTags += '<option value="2"' + (currentValue === "Batch" ? " selected" : "") + '>Batch</option>\n ';
    }
    else {
      transferModeSelectTags += '<option value="2"' + (currentValue === "Batch" ? " selected" : "") + '>Batch</option>\n ';
    }
    transferModeSelectTags += '</select>\n ';

    return transferModeSelectTags;
  }

  _SetUnitsBasedOnTransferMode = function () {

    //Get the node based on active MovementNodeIdEdit value
    var selectedNode = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeGuid === $('#MovementNodeIdEdit').val());
    var selectedMode = $('#TransferModeEdit option:selected').text();

    if (selectedNode) {
      $('#lblUnits').text(String((selectedMode === "Level") ? selectedNode.LevelProductUnits : selectedNode.VolumeUnits));
    }
  }
  //============================================================
  // This function initializes the date time functions.
  //============================================================
  _InitializeDateControls = function () {
    var numFormatInfoString = $('#NumberFormatInfoString').val();
    var numFormatInfo = JSON.parse(numFormatInfoString);
    FMLayout.dateFormat = ConvertToJQueryUIDateFormat(numFormatInfo.ShortDatePattern);
    FMLayout.timeFormat = ConvertToJQueryUITimeFormat(numFormatInfo.TimePattern);
    FMLayout.calendarLocation = $("#CalendarLocationUrl").val();

    $("#PlannedStartTimePicker").datetimepicker({
      buttonImage: FMLayout.calendarLocation + '/calendar.gif',
      buttonImageOnly: true,
      showOn: "button",
      showTimezone: false,
      useLocalTimezone: false,
      defaultTimezone: $("#datepickerTimezoneString").val(),
      dateFormat: FMLayout.dateFormat,
      timeFormat: FMLayout.timeFormat,
      showSecond: (FMLayout.timeFormat.indexOf("ss") === -1) ? false : true,
      beforeShow: function () {
        setTimeout(function () { $('.ui-datepicker').css('z-index', 99999999999) }, 0);
      }
    });
  };

  // redefine the function that will be executed when applying changes to tag values
  _HandleEditTagValueUpdate = function () {
    // get the new value
    var newVal = $("#PointTagNewValueInputId").val();
    var newRawVal = $("#PointTagNewRawValueInputId").val();

    if (newVal == null || newVal === '') {
      return;
    }
    var input = FMMovementModuleSettingsEditor.CellToUpdate;
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
    var inputParent = $(input).parent(); // A td
    var inputTagRowIndex = $(input).closest('tr').index();

    var rowData = FMMovementModuleSettingsEditor.dataTableHandle.row(inputTagRowIndex).data();

    var _NodeId, _NodeGuid, _Direction, _Mode;

    _NodeId = $('#MovementNodeIdEdit option:selected').text();
    _NodeGuid = $('#MovementNodeIdEdit option:selected').val();
    _Direction = $('#DirectionEdit option:selected').text();
    _Mode = $('#TransferModeEdit option:selected').text();

    var node = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeId === _NodeId);
    var _ModuleTypeVal = node.ModuleType;
    var _DirectionVal = $('#DirectionEdit option:selected').val();
    var _ModeVal = $('#TransferModeEdit option:selected').val();

    if (_ModuleTypeVal == ModuleType.StandardTank &&
      _DirectionVal == TransferDirection.Source &&
      _ModeVal == TransferMode.Batch &&
      newVal && newVal.charAt(0) !== '-')
      newVal = '-' + newVal;


    rowData[Columns.NODE_ID] = _NodeId;
    rowData[Columns.DIRECTION] = _Direction;
    rowData[Columns.TRANSFER_MODE] = _Mode;
    rowData[Columns.GUID] = _NodeGuid;
    rowData[Columns.INDIVIDUAL_CONTROL] = false;
    rowData[Columns.UNITS] = $('#lblUnits').text();

    // Update the UI
    var setpointTag = '<label id="editTargetSPInputId">' + newVal + '</label>&nbsp;&nbsp;<span class="glyphicon glyphicon-pencil editTargetSPValue" />';
    rowData[Columns.TRANSFER_TARGET_SETPOINT] = setpointTag;
    var table = FMMovementModuleSettingsEditor.dataTableHandle;
    table.cell(inputTagRowIndex, Columns.TRANSFER_TARGET_SETPOINT).data(setpointTag);//.draw(false);
    table.columns.adjust().draw();
  };
  //==================================================================================
  // This function will close the edit value modal dialog.
  //==================================================================================
  _CloseEditTagValueModal = function () {

    $('#confirm-exceed-min-dialog').remove();
    $('#confirm-exceed-max-dialog').remove();
    $('#MovementPointTagEditValueOkButton').unbind('click');
    $('#MovementPointTagEditValueCancelButton').unbind('click');
    $('#MovementPointTagEditValueScreenBody').html('');
    $('#MovementPointTagEditValueScreen'.close).click();
    FMMovementModuleSettingsEditor.CellToUpdate = "";
  };

  _AssignHotkeys = function () {

    var okButton = $('#MovementPointTagEditValueOkButton');
    var okButtonAccessKey = okButton.attr('accesskey');

    ampersandIndex = okButton.text().indexOf('&');
    if (ampersandIndex !== -1) {
      var hotKey = okButton.text().charAt(ampersandIndex + 1);
      okButton.html(okButton.text().replace('&' + hotKey, '<span style="text-decoration: underline;">' + hotKey + '</span>'));
      okButton.attr('accesskey', hotKey);
    }
    else if (okButtonAccessKey !== '' && okButtonAccessKey !== undefined) {
      okButton.html(okButton.text().replace(okButtonAccessKey, '<span style="text-decoration: underline;">' + okButtonAccessKey + '</span>'));
    }

    var cancelButton = $('#MovementPointTagEditValueCancelButton');
    var cancelButtonAccessKey = cancelButton.attr('accesskey');
    ampersandIndex = cancelButton.text().indexOf('&');
    if (ampersandIndex !== -1) {
      var hotKey = cancelButton.text().charAt(ampersandIndex + 1);
      cancelButton.html(cancelButton.text().replace('&' + hotKey, '<span style="text-decoration: underline;">' + hotKey + '</span>'));
      cancelButton.attr('accesskey', hotKey);
    }
    else if (cancelButtonAccessKey !== '' && cancelButtonAccessKey !== undefined) {
      cancelButton.html(cancelButton.text().replace(cancelButtonAccessKey, '<span style="text-decoration: underline;">' + cancelButtonAccessKey + '</span>'));
    }

  };

  _EditTagValue = function (input) {
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
    FMMovementModuleSettingsEditor.CellToUpdate = input;
    var inputTagRow = $(input).parent();
    var inputTagRowIndex = $(input).closest('tr').index();

    var nodeIdEdited = $('#MovementNodeIdEdit option:selected').text();

    rowData = FMMovementModuleSettingsEditor.dataTableHandle.row(inputTagRowIndex).data();

    var _NodeGuid, _ModuleType, _Direction, _Mode, _TransferVolMode, _Value, _Units;

    var nodeModel = model.MovementNodeModelList.find(n => n.MovementNodeId === rowData[Columns.NODE_ID]);

    _NodeGuid = $('#MovementNodeIdEdit option:selected').val();
    _Direction = $('#DirectionEdit option:selected').val();
    _Mode = $('#TransferModeEdit option:selected').val();

    if (nodeModel === undefined) {

      var node = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeId === nodeIdEdited);

      if (node === undefined) return;

      _ModuleType = node.ModuleType;
      _TransferVolMode = node.NodeTransferVolumeMode;
      _Value = $('#editTargetSPInputId').text();	// Uninitialized, so start at the min/max operating limit for level/batch-source
      _Units = (_Mode === TransferMode.Level) ? node.IntLevelUnits : node.IntVolumeUnits;
    }
    else {
      _ModuleType = nodeModel.ModuleType;
      _TransferVolMode = nodeModel.NodeTransferVolumeMode;
      _Value = $('#editTargetSPInputId').text();//nodeModel.TransferTarget;
      _Units = (nodeModel.TransferMode === TransferMode.Level) ? nodeModel.IntLevelUnits : nodeModel.IntVolumeUnits;
    }

    _Value = _Value.toString();

    FMMovementModuleSettingsEditor.TransferTargetBeforeCommit = _Value; // Store to revert to this value in EndEditRow, incase of an error for interlocked nodes
    FMMovementModuleSettingsEditor.TransferModeBeforeCommit = _Mode;// Store to revert to this value in EndEditRow, incase of an error for interlocked nodes

    var requestParams = {
      PointGuid: _NodeGuid
      , TankOrVolume: (_ModuleType == ModuleType.StandardTank)
      , SourceOrDest: (_Direction == TransferDirection.Source)
      , LevelOrBatch: (_Mode == TransferMode.Level)
      , GrossOrNet: (_TransferVolMode == TransferVolumeMode.GrossObservedVolume)
      , CurrentValue: _Value
      , Units: _Units
    }

    var url = $('#urlEditMovementPointTagValue').val();
    var token = $('#MovementModuleSettingsEditorForm input[name=__RequestVerificationToken]').val();
    var headers = {};
    headers['__RequestVerificationToken'] = token;

    FMErrorAndExceptionHandling.CloseNotifications();

    // create the backdrop and wait for next modal to be triggered
    var modalManager = $('body').modalmanager('loading');

    $.ajax({
      type: 'POST',
      cache: false,
      headers: headers,
      url: url,
      data: model.IsLaunchedFromSummary ? JSON.stringify(requestParams) : requestParams,
      success: function (response) {
        FMErrorAndExceptionHandling.HandleMessages(response, function (data, inError) {
          if (!inError || (data != null)) {
            $('#MovementPointTagEditValueScreenBody').html(response.Data);
            // Hide the Header, as MovementModuleSettingsEditor provides one
            $('.modal-header').hide();
            $('#MovementPointTagEditValueScreen').modal('show');
            FMMovementModuleSettingsEditor.AssignHotkeys();
          }
          else {
            $('body').modalmanager('loading'); // remove the loading background
          }
        });
      },
      error: function (xhr, textStatus, error) {
        FMErrorAndExceptionHandling.ShowException(xhr, textStatus, error, function () {
          $('body').modalmanager('loading'); // remove the loading background
        });
      }
    });
  };

  //======================================================
  // Return function pointers
  //======================================================
  return {
    inError: _inError
    , dataTableHandle: _dataTableHandle
    , movementNodeList: _movementNodeList
    , NodeModelList: _NodeModelList
    , valuesChanged: _valuesChanged
    , emptyGuid: _emptyGuid
    , SaveChanges: _SaveChanges
    , AddRow: _AddRow
    , DeleteRow: _DeleteRow
    , EditRow: _EditRow
    , PopulateMovementNodeList: _PopulateMovementNodeList
    , GetMovementNodes: _GetMovementNodes
    , CancelEditRow: _CancelEditRow
    , EndEditRow: _EndEditRow
    , HandleDropdownBlur: _HandleDropdownBlur
    , HandlingClick: _HandlingClick
    , Initialize: _Initialize
    , IgnoreBlurEvent: _ignoreBlurEvent
    , processingEndEditRow: _processingEndEditRow
    , GetMovementModuleSettingsEditorModelString: _GetMovementModuleSettingsEditorModelString
    , GetMovementModuleSettingsEditorModel: _GetMovementModuleSettingsEditorModel
    , SetMovementModuleSettingsEditorModelString: _SetMovementModuleSettingsEditorModelString
    , SetMovementModuleSettingsEditorModel: _SetMovementModuleSettingsEditorModel
    , EnableDisableDeleteButton: _EnableDisableDeleteButton
    , EnableDisableAddButton: _EnableDisableAddButton
    , Stack_bottomright_movementmodulesettingseditor: _stack_bottomright_movementmodulesettingseditor
    , ShowUserMovementSetupSection: _ShowUserMovementSetupSection
    , ShowMovementRecordingSection: _ShowMovementRecordingSection
    , ShowMovementCreateNewMovementSection: _ShowMovementCreateNewMovementSection
    , StopHaltBasedCbOnChange: _StopHaltBasedCbOnChange
    , ZeroFlowTbOnblur: _ZeroFlowTbOnblur
    , StartTimeBasedCbOnChange: _StartTimeBasedCbOnChange
    , UseControlTagCbOnChange: _UseControlTagCbOnChange
    , LoadData: _LoadData
    , InterLockCbOnChange: _InterLockCbOnChange
    , TypeDdOnChange: _TypeDdOnChange
    , DeleteCompletionCbOnChange: _DeleteCompletionCbOnChange
    , DeleteStopCbOnChange: _DeleteStopCbOnChange
    , IncludeHandgaugeCbOnChange: _IncludeHandgaugeCbOnChange
    , SendToAccountingCbOnChange: _SendToAccountingCbOnChange
    , SetPendingStatusCbOnChange: _SetPendingStatusCbOnChange
    , InitializeDateControls: _InitializeDateControls
    , UpdateModel: _UpdateModel
    , InitializeRecordingSection: _InitializeRecordingSection
    , GetMovementControlPoints: _GetMovementControlPoints
    , PopulateMovementControlTagDropdown: _PopulateMovementControlTagDropdown
    , SetUserControlTag: _SetUserControlTag
    , CreateNewMovementOnClick: _CreateNewMovementOnClick
    , ShowHideCreateNewMovementTab: _ShowHideCreateNewMovementTab
    , SetInitialTab: _SetInitialTab
    , UpdateNodeTableHeaderSizing: _UpdateNodeTableHeaderSizing
    , UpdateTargetSetpointsOnInterlockEnable: _UpdateTargetSetpointsOnInterlockEnable
    , UpdateIndividualNodeControlOnInterlockEnable: _UpdateIndividualNodeControlOnInterlockEnable
    , GetTransferModesBasedOnSelectedNodeType: _GetTransferModesBasedOnSelectedNodeType
    , SetUnitsBasedOnTransferMode: _SetUnitsBasedOnTransferMode
    , CallPointCalculator: _CallPointCalculator
    , CallPointCalculatorForInterlockedNodes: _CallPointCalculatorForInterlockedNodes
    , ReCalculateTargetSetpointForANode: _ReCalculateTargetSetpointForANode
    , CloseEditTagValueModal: _CloseEditTagValueModal
    , EditTagValue: _EditTagValue
    , HandleEditTagValueUpdate: _HandleEditTagValueUpdate
    , CellToUpdate: _cellToUpdate
    , AssignHotkeys: _AssignHotkeys
    , IsClickedOutsideMovementTable: _IsClickedOutsideMovementTable
    , NewRowIsAdded: _NewRowIsAdded
    , EditedRowIndex: _EditedRowIndex
    , ClickedRowIndex: _ClickedRowIndex
    , TransferTargetBeforeCommit: _TransferTargetBeforeCommit
    , TransferMode: TransferMode
    , TransferDirection: TransferDirection
    , ModuleType: ModuleType
    , TransferVolumeMode: TransferVolumeMode
    , TransferModeBeforeCommit: _TransferModeBeforeCommit
  };
}();


//=======================================================
// This function manually hooks up to the submit the form
//=======================================================
$(function () {
  $('#MovementModuleSettingsEditorForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
      e.preventDefault();
      return false;
    }
  });

  $('#MovementModuleSettingsEditorForm').submit(function () {
    var action = this.action;
    var method = this.method;

    FMMovementModuleSettingsEditor.SaveChanges();

    // it is important to return false in order to
    // cancel the default submission of the form
    // and perform the AJAX call
    return false;
  });
});


//=======================================================================
// RUN after page has been loaded but before render
//=======================================================================
$(document).ready(function () {
  // Initialize the movement node table
  FMMovementModuleSettingsEditor.Initialize();

  // Click to add a movement node table entry row
  $('#addMovementNodeEntryButton').on('click', function () {
    window.setTimeout(function () {
      if (FMMovementModuleSettingsEditor.inError === true) {
        return;
      }

      FMMovementModuleSettingsEditor.AddRow();
    }, 110);

  });

  // click on the delete button
  $('#deleteMovementNodeEntriesButton').on('click', function () {
    window.setTimeout(function () {
      FMMovementModuleSettingsEditor.DeleteRow();
    }, 110);

  });

  $('#MovementModuleSettingsNodeTable tbody').keyup(function (e) {
    if (e.keyCode === 46) {
      if ($('#MovementNodeIdEdit').length === 0 && FMMovementModuleSettingsEditor.dataTableHandle.rows('.selected').count() > 0) {
        FMMovementModuleSettingsEditor.DeleteRow();
      }
    }
  });

  $("input[name='MovementModuleSettings.HandGaugeData']").change(function () {
    // Update model with removed row.
    var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
    model.HandGaugeData = $("input[name='MovementModuleSettings.HandGaugeData']")[0].checked;
    FMMovementModuleSettingsEditor.SetMovementModuleSettingsEditorModel(model);
  });

  // Hide the Header, as MovementModuleSettingsEditor provides one
  $('.modal-header').hide();

  if ($('#Readonly').val() === 'True') {
    $('#PEMPESavePropertyScreen').attr('disabled', true);
  }

  $(document).on('change', '#TransferModeEdit', function () {
    var currentUnits = $('#lblUnits').text();
    FMMovementModuleSettingsEditor.SetUnitsBasedOnTransferMode();
    var updatedUnits = $('#lblUnits').text();
    if (currentUnits && updatedUnits && currentUnits == updatedUnits) return;
    var _editedNodeId = nodeIdEdited = $('#MovementNodeIdEdit option:selected').text();
    var _editedNodeDir = $('#DirectionEdit option:selected').text();
    var _editedNodeMode = $('#TransferModeEdit option:selected').val();
    var interlockSourceDestSetpoints = $("#InterlockCB").is(":checked");
    var totalRows = FMMovementModuleSettingsEditor.dataTableHandle.rows().count();
    var otherRowData, otherNodeModel;
    if (interlockSourceDestSetpoints && totalRows === 2) {
      // get dataTable row & update TransferTarget
      var nodeEdited = FMMovementModuleSettingsEditor.NodeModelList.find(n => n.MovementNodeId === _editedNodeId);

      var model = FMMovementModuleSettingsEditor.GetMovementModuleSettingsEditorModel();
      otherNodeModel = model.MovementNodeModelList.find(n => n.MovementNodeId !== _editedNodeId);
      if (_editedNodeMode == FMMovementModuleSettingsEditor.TransferMode.Batch && otherNodeModel &&
        otherNodeModel.TransferMode == FMMovementModuleSettingsEditor.TransferMode.Batch) {
        if (otherNodeModel.TransferDirection == FMMovementModuleSettingsEditor.TransferDirection.Source) {
          if (otherNodeModel.TransferTarget.charAt(0) == '-')
            $('#editTargetSPInputId').text(otherNodeModel.TransferTarget.substr(1));	// Destination is +ve for tanks or volume nodes
          else
            $('#editTargetSPInputId').text(otherNodeModel.TransferTarget);	// +ve value, so copy it
        }
        else {
          if (nodeEdited && nodeEdited.ModuleType == FMMovementModuleSettingsEditor.ModuleType.StandardTank)
            $('#editTargetSPInputId').text('-' + otherNodeModel.TransferTarget);// Add -ve sign for tanks that are sources
          else
            $('#editTargetSPInputId').text(otherNodeModel.TransferTarget);
        }
        return;
      }
    }
    FMMovementModuleSettingsEditor.ReCalculateTargetSetpointForANode();
  });

  FMErrorAndExceptionHandling.CloseNotifications();

  $('#MovementModuleSettingsNodeTable').on('click', '.editTargetSPValue', function () {
    $('.modal-header').show();
    FMMovementModuleSettingsEditor.EditTagValue(this);
  });

  // This function is called by the EditValue partial page.
  window.HandleEditTagValueUpdate = function () {
    FMMovementModuleSettingsEditor.HandleEditTagValueUpdate();
  }
});




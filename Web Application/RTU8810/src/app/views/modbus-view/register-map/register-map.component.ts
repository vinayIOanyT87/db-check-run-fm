// tslint:disable-next-line:max-line-length
import { Component, OnInit, OnDestroy, ViewEncapsulation, AfterViewInit, TemplateRef, ViewChild } from '@angular/core';
import { RtuconfigurationService, IRTUConfiguration, IPoint, modbusType } from 'src/app/services/rtuconfiguration.service';
import { RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';
import { IParameter, configClass, IParameterMap } from 'src/app/services/availablemodules.service';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
import { Subscription, Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { NgbTypeahead, NgbTypeaheadSelectItemEvent } from '@ng-bootstrap/ng-bootstrap';
import * as saveAs from 'node_modules/file-saver';

const registerMapSelectedImage = './assets/regmap-selected.png';
const registerMapUnSelectedImage = './assets/regmap-inactive.png';
const fpRegSelectedImage = './assets/fpreg-selected.png';
const fpRegUnSelectedImage = './assets/fpreg-inactive.png';
const intRegSelectedImage = './assets/intreg-selected.png';
const intRegUnSelectedImage = './assets/intreg-inactive.png';

const PointTypes = 'CPU Pnt,Interface Pnt,Port Pnt,FP Reg Pnt,INT Reg Pnt,GW Block Pnt,Tank Pnt,Alarm Pnt';

interface ITab {
  tabName: string;
}

interface ICell {
  tabIndex: number;
  object: any;
  disableOverride: boolean;
}

interface ITypeaheadOption {
  name: string;
  value: string;
}

// tslint:disable-next-line:class-name
class registerMap {
  pointId: number;
  registerMapIdx: number;
  img: string;
  label: string;
  labelLine1: string;
  labelLine2: string;
  selectedRegisterImg: string;
  activationimg: string;
  point: IPoint;

  constructor(pointId, registerMapIdx, label, point) {
    this.pointId = pointId;
    this.registerMapIdx = registerMapIdx;
    this.label = label;

    if (label !== 'Undefined') {
      this.labelLine1 = label.substring(0, label.indexOf('->'));
      this.labelLine2 = label.substring((label.indexOf('->') + 2), label.length);
    } else {
      this.labelLine1 = 'Undefined';
      this.labelLine2 = '';
    }
    this.point = point;
  }
}

class registerMapPoint implements IPoint {
  name: string;
  pointConfiguration: IRegisterMapParameterMap;
  computedName: string;
}

class IRegisterMapParameterMap implements IParameterMap {
  [identifier: number]: IRegisterMapParameter;
}

class IRegisterMapParameter implements IParameter {
  configClass: configClass;
  parameter: string;
  description: string;
  dataType: string;
  displayFormat: string;
  minimumValue: number;
  maximumValue: number;
  value: string;
  status: number;
  serverTimeStamp: Date;
  pendingValue: string;
  translatedPendingValue: string; // for PntIndex and PntParameter
  pendingStatus: number;
  pendingServerTimeStamp: Date;
  availableCommands: string;
  availableDeviceTypeValues: string;
  identifier: number;
  opcstartNodeID: number;
  tab: string;
  section: string;
  readableStatus: string;
  readableName: string;
  parameterIsVisible: number;
  availableCommandsOutputMatches: number;
  variableAlarmNumber: string;
  datatypeLength: string;
  disableOverride: boolean;
}


@Component({
  selector: 'app-register-map',
  templateUrl: './register-map.component.html',
  styleUrls: ['./register-map.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class RegisterMapComponent implements OnInit, OnDestroy, AfterViewInit {
  selectedRegisterMap: registerMap;
  selectedRegisterMaps: registerMap[] = [];
  registerMapViewTabs: ITab[];
  enabledRegisterMaps: registerMap[];
  registerMapPanel: registerMap[];
  registerMapPanelSets: any[];  // left panel: group of 4 reg map to display on a row
  ready = false;
  rtuconfiguration: IRTUConfiguration;
  rtuconfigurationSubscription: Subscription;
  registerMapParameters: IParameter[];
  searchRegisterMapString = '';
  searchRegisterMapToggle = false;
  searchConfigurationToggle = false;
  searchConfigurationString = '';
  labelIdentifier: number;
  typeaheadOptions: ITypeaheadOption[] = [];
  pntParametertypeaheadOptions: ITypeaheadOption[] = [];
  dragSelectActive = false;
  modalRef: BsModalRef;
  showPointIndexes = false;
  modbusType = modbusType;
  modbusTypeKeys: Array<string> = [];
  currentModBusType = '';

  modalConfig = {
    backdrop: true,
    ignoreBackdropClick: false,
    class: 'modal-lg'
  };

  @ViewChild('modalVerifyCancelAction', { static: true }) public modalVerifyCancelAction: TemplateRef<any>;
  @ViewChild('modalVerifyApplytoRTUAction', { static: true }) public modalVerifyApplytoRTUAction: TemplateRef<any>;


  constructor(
    private _rtuConfiguration: RtuconfigurationService,
    private _modalService: BsModalService) {
  }

  ngOnInit(): void {
    this.modbusTypeKeys = this._rtuConfiguration.modbusTypeKeys;
    this.showPointIndexes = (localStorage.getItem('showPointIndexes') === 'true') ? true : false;
    this.currentModBusType = (localStorage.getItem('currentModBusType'));
    if (this.currentModBusType == null) {
      this.currentModBusType = modbusType.REGISTERMAPS;
    }
    this.getRTUConfiguration();
  }

  ngOnDestroy() {
    if (this._rtuConfiguration) {
      this._rtuConfiguration.unsubscribeRealtimeParameters(this.registerMapParameters);
    }
    if (this.rtuconfigurationSubscription) {
      this.rtuconfigurationSubscription.unsubscribe();
    }
  }

  ngAfterViewInit() {
    setTimeout(() => this.ready = true);
  }

  getRTUConfiguration(): any {
    const instance = this;

    this.rtuconfigurationSubscription = this._rtuConfiguration.get().subscribe(data => {
      if (data.RTUConfiguration) {
        instance.rtuconfiguration = data.RTUConfiguration;
      } else {
        instance.rtuconfiguration = null;
      }

      this.updateSelectedRegisterMapLabels();

      instance.selectedRegisterMaps.forEach(function (registerMap) {
        let registerMapPoint = registerMap.point as registerMapPoint;

        switch (instance.currentModBusType) {
          case modbusType.REGISTERMAPS:
            // update registerMap Src type
            let pntParameterKey = Object.keys(registerMapPoint.pointConfiguration).find(
                s => registerMapPoint.pointConfiguration[s].parameter === 'SrcParameter');
            let PntParameterName = 'undefined';

            let pointRefMapNumberLookupDictionaryKey = Object.keys(instance.rtuconfiguration.pointRefMapNumberLookupDictionary).find(o => instance.rtuconfiguration.pointRefMapNumberLookupDictionary[o].alarmNumber === registerMapPoint.pointConfiguration[pntParameterKey].pendingValue);
            if (pointRefMapNumberLookupDictionaryKey) {
              PntParameterName = instance.rtuconfiguration.pointRefMapNumberLookupDictionary[pointRefMapNumberLookupDictionaryKey].variableName;
            }

            // update the SrcParameter text if necessary
            let pntParameter = registerMapPoint.pointConfiguration[pntParameterKey];
            // only update it if the control matches the config (don't want to change an edit the user has made)
            if (pntParameter.value === pntParameter.pendingValue) {
              pntParameter.temppendingValue = {
                name: pntParameter.translatedPendingValue,
                value: pntParameter.value
              };
            }

            let pntIndexParameterKey = Object.keys(registerMapPoint.pointConfiguration).find(
                s => registerMapPoint.pointConfiguration[s].parameter === 'SrcIndex');
            let pntIndexParameter = registerMapPoint.pointConfiguration[pntIndexParameterKey];
            // only update it if the control matches the config (don't want to change an edit the user has made)
            if (pntIndexParameter.value === pntIndexParameter.pendingValue) {
              const PntTypeKey = Object.keys(registerMap.point.pointConfiguration).find(
                  s => registerMap.point.pointConfiguration[s].parameter === 'SrcType');
              if (registerMap.point.pointConfiguration[PntTypeKey].pendingValue === '3') {
                pntIndexParameter.temppendingValue = {
                  name: instance.getPntIndexNameForValue(registerMapPoint, pntIndexParameterKey),
                  value: registerMapPoint.pointConfiguration[pntIndexParameterKey].pendingValue
                };
              }
            }

            // update registerMap Destination type
            pntParameterKey = Object.keys(registerMapPoint.pointConfiguration).find(
                s => registerMapPoint.pointConfiguration[s].parameter === 'DestParameter');
            pointRefMapNumberLookupDictionaryKey = Object.keys(instance.rtuconfiguration.pointRefMapNumberLookupDictionary).find(o => instance.rtuconfiguration.pointRefMapNumberLookupDictionary[o].alarmNumber === registerMapPoint.pointConfiguration[pntParameterKey].pendingValue);
            PntParameterName = 'undefined';

            pntParameter = registerMapPoint.pointConfiguration[pntParameterKey];
            // only update it if the control matches the config (don't want to change an edit the user has made)
            if (pntParameter.value === pntParameter.pendingValue) {
              pntParameter.temppendingValue = {
                name: pntParameter.translatedPendingValue,
                value: pntParameter.value
              };
            }

            pntIndexParameterKey = Object.keys(registerMapPoint.pointConfiguration).find(
                s => registerMapPoint.pointConfiguration[s].parameter === 'DestIndex');
            pntIndexParameter = registerMapPoint.pointConfiguration[pntIndexParameterKey];
            // only update it if the control matches the config (don't want to change an edit the user has made)
            if (pntIndexParameter.value === pntIndexParameter.pendingValue) {
              let PntTypeKey = Object.keys(registerMap.point.pointConfiguration).find(
                  s => registerMap.point.pointConfiguration[s].parameter === 'DestType');
              if (registerMap.point.pointConfiguration[PntTypeKey].pendingValue === '3') {
                pntIndexParameter.temppendingValue = {
                  name: instance.getPntIndexNameForValue(registerMapPoint, pntIndexParameterKey),
                  value: registerMapPoint.pointConfiguration[pntIndexParameterKey].pendingValue
                };
              }
            }

            registerMapPoint.computedName = instance.getRegisterMapComputedName(registerMapPoint);
            registerMap.label = registerMapPoint.computedName;
            break;

          case modbusType.FLOATINGPOINTREGISTERS:
          case modbusType.INTEGERREGISTERS:
            registerMapPoint.computedName = instance.getFPIntRegisterComputedName(registerMapPoint);
            registerMap.label = registerMapPoint.computedName;
            registerMap.labelLine1 = registerMapPoint.computedName;
            registerMap.labelLine2 = '';
            break;
        }

        instance.updateSelectedRegisterMapLabels();
      });

      instance.getEnabledRegisterMaps();
      instance.getRegisterMapSelectTab(); // get the list of alarms to display in the left panel
      instance.getRegisterMapViewTab();

    });
  }

  getEnabledRegisterMaps() {
    if (this._rtuConfiguration
      && this.registerMapParameters) {
      this._rtuConfiguration.unsubscribeRealtimeParameters(this.registerMapParameters);
    }

    this.registerMapParameters = [];
    const enabledRegisterMap = [];
    this.labelIdentifier = -1;
    const instance = this;
    let registerMapIdx = 0;

    if (this.rtuconfiguration != null
      && this.rtuconfiguration.points != null) {
      let numberOfRegisterMaps = 100;
      const moduleConfiguration = this.rtuconfiguration.module0.moduleConfiguration;
      // tslint:disable-next-line:max-line-length
      const numberOfRegisterMapsIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfRegMap');
      if (numberOfRegisterMapsIdentifier) {
        numberOfRegisterMaps = parseInt(moduleConfiguration[numberOfRegisterMapsIdentifier].pendingValue, 10);
        instance.registerMapParameters.push(moduleConfiguration[numberOfRegisterMapsIdentifier]);
      }

      this.rtuconfiguration.points.forEach(function (point, index) {
        if (registerMapIdx >= numberOfRegisterMaps) {
          return;
        }

        if (instance.currentModBusType === modbusType.REGISTERMAPS && point.name === ' Register Map ') {
          const registerMapPoint = point as registerMapPoint;

          registerMapPoint.computedName = instance.getRegisterMapComputedName(registerMapPoint);
          const newRegisterMap = new registerMap(index, registerMapIdx++, registerMapPoint.computedName, registerMapPoint);

          newRegisterMap.label = registerMapPoint.computedName;
          if (instance.showPointIndexes) {
            newRegisterMap.labelLine1 += ' (' + (newRegisterMap.registerMapIdx + 1).toString().padStart(3, '0') + ')';
          }
          instance.updateRegisterMapImage(newRegisterMap);
          enabledRegisterMap.push(newRegisterMap);
        }
        if ((instance.currentModBusType === modbusType.FLOATINGPOINTREGISTERS && point.name === ' Modbus Floating Point Reg. ')
            || (instance.currentModBusType === modbusType.INTEGERREGISTERS && point.name === ' Modbus Integer Reg. ')) {
          const registerMapPoint = point as registerMapPoint;

          registerMapPoint.computedName = instance.getFPIntRegisterComputedName(registerMapPoint);
          const newRegisterMap = new registerMap(index, registerMapIdx++, registerMapPoint.computedName, registerMapPoint);

          newRegisterMap.label = registerMapPoint.computedName;
          newRegisterMap.labelLine1 = registerMapPoint.computedName;
          newRegisterMap.labelLine2 = '';

          if (instance.showPointIndexes) {
            newRegisterMap.labelLine1 += ' (' + (newRegisterMap.registerMapIdx + 1).toString().padStart(3, '0') + ')';
          }
          instance.updateRegisterMapImage(newRegisterMap);
          enabledRegisterMap.push(newRegisterMap);
        }
      });
    }

    instance.enabledRegisterMaps = enabledRegisterMap.sort(instance.compareRegisterMaps);

    if (instance.selectedRegisterMap != null) {
      // tslint:disable-next-line:max-line-length
      Object.keys(instance.selectedRegisterMap.point.pointConfiguration).map(s => instance.selectedRegisterMap.point.pointConfiguration[s]).forEach(function (parameter) {
        instance.registerMapParameters.push(parameter);
      });
    }

    instance.enabledRegisterMaps.forEach(function (alarm) {
      // retrieving the necessary parameters to be able to update reg map names
      switch (instance.currentModBusType) {
        case modbusType.REGISTERMAPS:
          const SrcTypeKey = Object.keys(alarm.point.pointConfiguration).find(
            s => alarm.point.pointConfiguration[s].parameter === 'SrcType');
          const SrcIndexKey = Object.keys(alarm.point.pointConfiguration).find(
            s => alarm.point.pointConfiguration[s].parameter === 'SrcIndex');
          const SrcParameterKey = Object.keys(alarm.point.pointConfiguration).find(
            s => alarm.point.pointConfiguration[s].parameter === 'SrcParameter');
          const DestTypeKey = Object.keys(alarm.point.pointConfiguration).find(
            s => alarm.point.pointConfiguration[s].parameter === 'DestType');
          // tslint:disable-next-line:max-line-length
          const DestIndexKey = Object.keys(alarm.point.pointConfiguration).find(
            s => alarm.point.pointConfiguration[s].parameter === 'DestIndex');
          const DestParameterKey = Object.keys(alarm.point.pointConfiguration).find(
            s => alarm.point.pointConfiguration[s].parameter === 'DestParameter');

          instance.registerMapParameters.push(alarm.point.pointConfiguration[SrcTypeKey]);
          instance.registerMapParameters.push(alarm.point.pointConfiguration[SrcIndexKey]);
          instance.registerMapParameters.push(alarm.point.pointConfiguration[SrcParameterKey]);
          instance.registerMapParameters.push(alarm.point.pointConfiguration[DestTypeKey]);
          instance.registerMapParameters.push(alarm.point.pointConfiguration[DestIndexKey]);
          instance.registerMapParameters.push(alarm.point.pointConfiguration[DestParameterKey]);
          break;

        case modbusType.FLOATINGPOINTREGISTERS:
        case modbusType.INTEGERREGISTERS:
          const LabelKey = Object.keys(alarm.point.pointConfiguration).find(
            s => alarm.point.pointConfiguration[s].parameter === 'Label');

          instance.registerMapParameters.push(alarm.point.pointConfiguration[LabelKey]);
          break;
        }
    });

    if (instance._rtuConfiguration && instance.registerMapParameters) {
      instance._rtuConfiguration.subscribeRealtimeParameters(this.registerMapParameters);
    }
  }

  // get the reg maps to display n the left panel
  getRegisterMapSelectTab() {
    const filteredRegisterMap = [];
    const instance = this;
    const upperCaseSearchString = this.searchRegisterMapString.toUpperCase();
    this.enabledRegisterMaps.forEach( function(registerMap) {
      if (!instance.searchRegisterMapString
        || instance.searchRegisterMapString === ''
        || (instance.searchRegisterMapString !== ''
          && (registerMap.label.toUpperCase().indexOf(upperCaseSearchString) !== -1)
          || (registerMap.labelLine1.toUpperCase().indexOf(upperCaseSearchString) !== -1))) {
            filteredRegisterMap.push(registerMap);
      } else if (registerMap === instance.selectedRegisterMap) {
        instance.selectedRegisterMap = null;
      }
    });

    instance.registerMapPanel = filteredRegisterMap.sort(this.compareRegisterMaps);

    // left pane: divide the list of reg maps into groups of 4 for display on the left panel
    if (instance.registerMapPanel.length > 0) {
      instance.registerMapPanelSets = instance.registerMapPanel.reduce((resultArray, alarm, index) => {
        const chunkIndex = Math.floor((index) / 4 );

        if (!resultArray[chunkIndex]) {
          resultArray[chunkIndex] = []; // start a new chunk
        }

        resultArray[chunkIndex].push(alarm);

        return resultArray;
      }, []);
    } else {
      instance.registerMapPanelSets = [];
    }
  }

  getAlarmViewTab() {
    const instance = this;
    if (!instance.registerMapViewTabs) {
      instance.registerMapViewTabs = [{ tabName: 'Config' }, { tabName: 'Command' }];
    }

    if (!instance.selectedRegisterMap
      && instance.enabledRegisterMaps.length === 0) {
      return;
    }
  }

  getRegisterMapViewTab() {
    const instance = this;
    if (!instance.registerMapViewTabs) {
      instance.registerMapViewTabs = [{ tabName: 'Config' }, { tabName: 'Command' }];
    }


    if (!instance.selectedRegisterMap) {
      return;
    }
  }

  trackByPointId(index, item) {
    return item[0].pointId;
  }

  compareRegisterMaps( a: registerMap, b: registerMap) {
    if (a.label === 'Undefined' && b.label !== 'Undefined') {
      return 1;
    }

    if (b.label === 'Undefined' && a.label !== 'Undefined') {
      return -1;
    }

    if (a.label < b.label) {
      return -1;
    }

    if (a.label > b.label) {
      return 1;
    }
      return 1;
  }

  updateRegisterMapImage(registerMap: registerMap, path = null) {
    if (path) {
      registerMap.img = path;
    } else {
      const instance = this;
      let selectedImage: string;
      let unselectedImage: string;

      switch (instance.currentModBusType) {
        case modbusType.REGISTERMAPS:
          selectedImage = registerMapSelectedImage;
          unselectedImage = registerMapUnSelectedImage;
          break;

        case modbusType.FLOATINGPOINTREGISTERS:
          selectedImage = fpRegSelectedImage;
          unselectedImage = fpRegUnSelectedImage;
          break;

        case modbusType.INTEGERREGISTERS:
          selectedImage = intRegSelectedImage;
          unselectedImage = intRegUnSelectedImage;
          break;
      }
      if (instance.selectedRegisterMaps.findIndex(x => x.pointId === registerMap.pointId) !== -1) {
        registerMap.img = selectedImage;
      } else {
        registerMap.img = unselectedImage;
      }
    }
  }

  getRegisterMapComputedName(registerMapPoint: registerMapPoint) {
    const instance = this;
    const SrcTypeKey = Object.keys(registerMapPoint.pointConfiguration).find(s => registerMapPoint.pointConfiguration[s].parameter === 'SrcType');
    // tslint:disable-next-line:max-line-length
    const SrcIndexKey = Object.keys(registerMapPoint.pointConfiguration).find(s => registerMapPoint.pointConfiguration[s].parameter === 'SrcIndex');
    // tslint:disable-next-line:max-line-length
    const SrcParameterKey = Object.keys(registerMapPoint.pointConfiguration).find(s => registerMapPoint.pointConfiguration[s].parameter === 'SrcParameter');
    const DestTypeKey = Object.keys(registerMapPoint.pointConfiguration).find(s => registerMapPoint.pointConfiguration[s].parameter === 'DestType');
    const DestIndexKey = Object.keys(registerMapPoint.pointConfiguration).find(s => registerMapPoint.pointConfiguration[s].parameter === 'DestIndex');
    const DestParameterKey = Object.keys(registerMapPoint.pointConfiguration).find(s => registerMapPoint.pointConfiguration[s].parameter === 'DestParameter');

    if (!(SrcTypeKey && SrcIndexKey && SrcParameterKey && DestTypeKey && DestIndexKey && DestParameterKey)) {
      return;
    }

    let pointLabel = 'undefined';
    const pointSrcType = parseInt(registerMapPoint.pointConfiguration[SrcTypeKey].pendingValue, 10);
    let pointIndexLable = 'undefined';
    // Source Point
    switch (pointSrcType) {
      case 4: // FP Reg Pnt
        if (registerMapPoint.pointConfiguration[SrcIndexKey].pendingValue === '0') {
          pointLabel = 'undefined';
          pointIndexLable = 'undefined';
        } else {
          pointLabel = 'FPREG.' + ('000' + registerMapPoint.pointConfiguration[SrcIndexKey].pendingValue);
          pointIndexLable = 'FP Register ' + registerMapPoint.pointConfiguration[SrcIndexKey].pendingValue;
        }
        break;
      case 5: // INT Reg Pnt
        if (registerMapPoint.pointConfiguration[SrcIndexKey].pendingValue === '0') {
          pointLabel = 'undefined';
          pointIndexLable = 'undefined';
        } else {
          pointLabel = 'INT.' + ('000' + registerMapPoint.pointConfiguration[SrcIndexKey].pendingValue);
          pointIndexLable = 'Integer Register ' + registerMapPoint.pointConfiguration[SrcIndexKey].pendingValue;
        }
        break;
    }

    if (pointIndexLable !== 'undefined' && pointIndexLable) {
      registerMapPoint.pointConfiguration[SrcIndexKey].translatedPendingValue = pointIndexLable;
    } else {
      if (registerMapPoint.pointConfiguration[SrcIndexKey].pendingValue === '0') {
        registerMapPoint.pointConfiguration[SrcIndexKey].translatedPendingValue = 'None';
      } else {
        // tslint:disable-next-line:max-line-length
        registerMapPoint.pointConfiguration[SrcIndexKey].translatedPendingValue = registerMapPoint.pointConfiguration[SrcIndexKey].pendingValue;
      }
    }

    // Destination Point
    let destpointLabel = 'undefined';
    const destpointType = parseInt(registerMapPoint.pointConfiguration[DestTypeKey].pendingValue);
    let destpointIndexLable = 'undefined';

    switch (destpointType) {
      case 1: // CPU Pnt
        if (registerMapPoint.pointConfiguration[DestIndexKey].pendingValue === '1') {
          let PntLabelKey = Object.keys(instance.rtuconfiguration.module0.moduleConfiguration).find(
              s => instance.rtuconfiguration.module0.moduleConfiguration[s].parameter === 'Label');
          destpointLabel = instance.rtuconfiguration.module0.moduleConfiguration[PntLabelKey].pendingValue;
        } else {
          destpointLabel = 'None';
        }
        destpointIndexLable = destpointLabel;
        break;
      case 2: // Interface Pnt
        if (registerMapPoint.pointConfiguration[DestIndexKey].pendingValue >= 1 && registerMapPoint.pointConfiguration[DestIndexKey].pendingValue <= 6) {
            let moduleString = 'module' + registerMapPoint.pointConfiguration[DestIndexKey].pendingValue;
            const InterfacePntLabelKey = Object.keys(instance.rtuconfiguration[moduleString].moduleConfiguration).find(
                s => instance.rtuconfiguration[moduleString].moduleConfiguration[s].parameter === 'Label');
            destpointLabel = instance.rtuconfiguration[moduleString].moduleConfiguration[InterfacePntLabelKey].pendingValue;
        } else {
          destpointLabel = 'undefined';
        }
        destpointIndexLable = destpointLabel;
        break;
      case 3: // Port Pnt
        if (registerMapPoint.pointConfiguration[DestIndexKey].pendingValue === '0') {
          destpointLabel = 'undefined';
        } else {
          const moduleNumber = Math.floor((registerMapPoint.pointConfiguration[DestIndexKey].pendingValue - 1) / 8);

          registerMapPoint.pointConfiguration[DestIndexKey].temppendingValue = {name: instance.getPntIndexNameForValue(registerMapPoint, DestIndexKey), value: registerMapPoint.pointConfiguration[DestIndexKey].pendingValue};

          destpointLabel = registerMapPoint.pointConfiguration[DestIndexKey].temppendingValue.name;
        }
        destpointIndexLable = destpointLabel;
        break;
      case 4: // FP Reg Pnt
        if (registerMapPoint.pointConfiguration[DestIndexKey].pendingValue === '0') {
          destpointLabel = 'undefined';
          destpointIndexLable = 'undefined';
        } else {
          destpointLabel =  'FPREG.' + ('000' + registerMapPoint.pointConfiguration[DestIndexKey].pendingValue);
          destpointIndexLable = 'FP Register ' + registerMapPoint.pointConfiguration[DestIndexKey].pendingValue;
        }
        break;
      case 5: // INT Reg Pnt
        if (registerMapPoint.pointConfiguration[DestIndexKey].pendingValue === '0') {
          destpointLabel = 'undefined';
          destpointIndexLable = 'undefined';
        } else {
          destpointLabel = 'INT.' + ('000' + registerMapPoint.pointConfiguration[DestIndexKey].pendingValue);
          destpointIndexLable = 'Integer Register ' + registerMapPoint.pointConfiguration[DestIndexKey].pendingValue;
        }
        break;

      case 6: // GW Block Pnt
        if (registerMapPoint.pointConfiguration[DestIndexKey].pendingValue === '0') {
          destpointLabel = 'undefined';
        } else {
          destpointLabel = 'GW Block Pnt ' + registerMapPoint.pointConfiguration[DestIndexKey].pendingValue;
        }
        destpointIndexLable = destpointLabel;
        break;
      case 7: // Tank Pnt
        const tanks = this.rtuconfiguration.points.filter(x => x.name === 'Tank');
        const pointIndex = registerMapPoint.pointConfiguration[DestIndexKey].pendingValue;

        const tank = tanks[(parseInt(pointIndex, 10) - 1)];

        if (tank) {
          const PntLabelKey = Object.keys(tank.pointConfiguration).find(s => tank.pointConfiguration[s].parameter === 'Label');
          destpointLabel = tank.pointConfiguration[PntLabelKey].pendingValue;
        } else {
          destpointLabel = 'undefined';
        }
        destpointIndexLable = destpointLabel;
        break;
      case 8: // Alarm Pnt
        if (registerMapPoint.pointConfiguration[DestIndexKey].pendingValue === '0') {
          destpointLabel = 'undefined';
        } else {
          const zerofilled = 'Alarm ' + ('0000' + registerMapPoint.pointConfiguration[DestIndexKey].pendingValue).slice(-4);

          destpointLabel = 'Alarm.' + ('0000' + registerMapPoint.pointConfiguration[DestIndexKey].pendingValue).slice(-4);
          destpointIndexLable = zerofilled;
        }
        break;
    }

    if (destpointIndexLable !== 'undefined' && destpointIndexLable) {
      registerMapPoint.pointConfiguration[DestIndexKey].translatedPendingValue = destpointIndexLable;
    } else {
      if (registerMapPoint.pointConfiguration[DestIndexKey].pendingValue === '0') {
        registerMapPoint.pointConfiguration[DestIndexKey].translatedPendingValue = 'None';
      } else {
        // tslint:disable-next-line:max-line-length
        registerMapPoint.pointConfiguration[DestIndexKey].translatedPendingValue = registerMapPoint.pointConfiguration[DestIndexKey].pendingValue;
      }
    }

    // translate the value display on the screen for parameters
    let SrcParameterName = 'undefined';

    let pointRefMapNumberLookupDictionaryKey = Object.keys(instance.rtuconfiguration.pointRefMapNumberLookupDictionary).find(o => instance.rtuconfiguration.pointRefMapNumberLookupDictionary[o].alarmNumber === registerMapPoint.pointConfiguration[SrcParameterKey].pendingValue);
    if (pointRefMapNumberLookupDictionaryKey) {
      SrcParameterName = instance.rtuconfiguration.pointRefMapNumberLookupDictionary[pointRefMapNumberLookupDictionaryKey].variableName;
    }
    if (SrcParameterName !== 'undefined' && SrcParameterName) {
      registerMapPoint.pointConfiguration[SrcParameterKey].translatedPendingValue = SrcParameterName;
    } else {
      if (registerMapPoint.pointConfiguration[SrcParameterKey].pendingValue === '0') {
        registerMapPoint.pointConfiguration[SrcParameterKey].translatedPendingValue = 'None';
      } else {
        registerMapPoint.pointConfiguration[SrcParameterKey].translatedPendingValue = registerMapPoint.pointConfiguration[SrcParameterKey].pendingValue;
      }
    }

    let DestParameterName = 'undefined';
    pointRefMapNumberLookupDictionaryKey = Object.keys(instance.rtuconfiguration.pointRefMapNumberLookupDictionary).find(o => instance.rtuconfiguration.pointRefMapNumberLookupDictionary[o].alarmNumber === registerMapPoint.pointConfiguration[DestParameterKey].pendingValue);
    if (pointRefMapNumberLookupDictionaryKey) {
      DestParameterName = instance.rtuconfiguration.pointRefMapNumberLookupDictionary[pointRefMapNumberLookupDictionaryKey].variableName;
    }

    if (DestParameterName !== 'undefined' && DestParameterName) {
      registerMapPoint.pointConfiguration[DestParameterKey].translatedPendingValue = DestParameterName;
    } else {
      if (registerMapPoint.pointConfiguration[DestParameterKey].pendingValue === '0') {
        registerMapPoint.pointConfiguration[DestParameterKey].translatedPendingValue = 'None';
      } else {
        registerMapPoint.pointConfiguration[DestParameterKey].translatedPendingValue = registerMapPoint.pointConfiguration[DestParameterKey].pendingValue;
      }
    }

    let name = pointLabel + '.'
      + SrcParameterName + '->'
      + destpointLabel + '.'
      + DestParameterName;

    if ( name.indexOf('undefined') !== -1) {
      name = 'Undefined';
    }

    return name;
  }

  getFPIntRegisterComputedName(registerMapPoint: registerMapPoint) {
    const LabelKey = Object.keys(registerMapPoint.pointConfiguration).find(
      s => registerMapPoint.pointConfiguration[s].parameter === 'Label');
    const ChannelKey = Object.keys(registerMapPoint.pointConfiguration).find(
      s => registerMapPoint.pointConfiguration[s].parameter === 'Channel');

    if (!(LabelKey && ChannelKey)) {
      return;
    }

    const pointChannel = parseInt(registerMapPoint.pointConfiguration[ChannelKey].pendingValue, 10);
    let name = 'Undefined';
    // Channel Point
    if (pointChannel !== 0) {
      name = registerMapPoint.pointConfiguration[LabelKey].pendingValue;
    }

    return name;
  }

  updateSelectedRegisterMapLabels() {
    const instance = this;

    instance.selectedRegisterMaps.forEach(function (registerMap) {
      const registerMapPoint = registerMap.point as registerMapPoint;

      switch (instance.currentModBusType) {
        case modbusType.REGISTERMAPS:
          registerMapPoint.computedName = instance.getRegisterMapComputedName(registerMapPoint);
          registerMap.label = registerMapPoint.computedName;

          if (registerMap.label !== 'Undefined') {
            registerMap.labelLine1 = registerMapPoint.computedName.substring(0, registerMapPoint.computedName.indexOf('->'));
            registerMap.labelLine2 = registerMapPoint.computedName.substring((registerMapPoint.computedName.indexOf('->') + 2), registerMapPoint.computedName.length);
          } else {
            registerMap.labelLine1 = 'Undefined';
            registerMap.labelLine2 = '';
          }
          if (instance.showPointIndexes) {
            registerMap.labelLine1 += ' (' + (registerMap.registerMapIdx + 1).toString().padStart(3, '0') + ')';
          }
          break;

        case modbusType.FLOATINGPOINTREGISTERS:
        case modbusType.INTEGERREGISTERS:
          registerMapPoint.computedName = instance.getFPIntRegisterComputedName(registerMapPoint);
          registerMap.label = registerMapPoint.computedName;
          registerMap.labelLine1 = registerMapPoint.computedName;
          registerMap.labelLine2 = '';

          if (instance.showPointIndexes) {
            registerMap.labelLine1 += ' (' + (registerMap.registerMapIdx + 1).toString().padStart(3, '0') + ')';
          }
          break;
      }
    });
  }

  onRegisterMapImgClick(event: any, pointId: number, alarmId: number) {
    const instance = this;
    const clickedAlarm = instance.enabledRegisterMaps.find(s => s.pointId === pointId);

    if (instance.selectedRegisterMaps.indexOf(instance.selectedRegisterMaps.find(s => s.pointId === clickedAlarm.pointId)) === -1) {
      // regular clicked an unselected reg map. Clear selectedRegisterMaps and revert images, and then select clicked alarm

        instance.selectedRegisterMaps = [];
        instance.selectedRegisterMap = instance.enabledRegisterMaps.find(s => s.pointId === pointId);
        instance.selectedRegisterMaps.push(instance.selectedRegisterMap);
    } else {

        instance.selectedRegisterMaps.forEach(function (alarm) {
        });
        // if we are in batch edit mode and regular click, single select that reg map
        if (instance.selectedRegisterMaps.length > 1) {
          instance.selectedRegisterMap = instance.enabledRegisterMaps.find(s => s.pointId === pointId);
          instance.selectedRegisterMaps = [];
          instance.selectedRegisterMaps.push(instance.selectedRegisterMap);
        } else { // otherwise we clicked on the only one selected. deselect it.
          instance.selectedRegisterMaps = [];
          instance.selectedRegisterMap = null;
        }
    }
    instance.getEnabledRegisterMaps();
    instance.getRegisterMapSelectTab();
    instance.getAlarmViewTab();
  }



  // left panel search
  toggleRegisterMapSearch() {
    this.searchRegisterMapToggle = !this.searchRegisterMapToggle;
    this.searchRegisterMapString = '';
    this.getRegisterMapSelectTab();
    this.getAlarmViewTab();
    if (this.searchRegisterMapToggle) {
      const input = document.getElementById('registermapfilterinput');
      setTimeout(() => { input.focus(); }, 100);
    }
  }

  // right panel search
  toggleConfigurationSearch() {
    this.searchConfigurationToggle = !this.searchConfigurationToggle;
    this.searchConfigurationString = '';
    this.getAlarmViewTab();
    if (this.searchConfigurationToggle) {
      const input = document.getElementById('configurationfilterinput');
      setTimeout(() => { input.focus(); }, 100);
    }
  }

  searchRegisterMapStringChanged(newSearchValue) {
    this.searchRegisterMapString = newSearchValue;
    this.getRegisterMapSelectTab();
    this.getAlarmViewTab();
  }

  searchConfigurationChanged(newSearchValue) {
    this.searchConfigurationString = newSearchValue;
    this.getAlarmViewTab();
  }

  onRegisterMapPanelClick(event: any) {
    const instance = this;
    if (instance.dragSelectActive === false) {
      instance.selectedRegisterMaps = [];
      if (instance.selectedRegisterMap) {
        instance.selectedRegisterMap = null;
      }

      instance.getEnabledRegisterMaps();
      instance.getRegisterMapSelectTab();
      instance.getAlarmViewTab();
    } else {
      instance.dragSelectActive = false;
    }
  }

  public isRTUConnectedandChangesExist() {
    if (this._rtuConfiguration.connectionStatus !== RTUConnectionStatus.CONNECTED) {
      return true;
    } else if (this._rtuConfiguration.checkForRTUConfigChanges() === false) {
      return true;
    } else {
      return false;
    }
  }

  VerifyCancelChangesPrompt(): void {
    if (this._rtuConfiguration.checkForRTUConfigChanges()) {
      this.modalRef = this._modalService.show(this.modalVerifyCancelAction, this.modalConfig);
    }
  }

  public areThereNoChangesMade() {
    if (this._rtuConfiguration.checkForRTUConfigChanges()) {
      return false;
    } else {
      return true;
    }
  }

  setInputBoxStyle(anycell: any) {
    const styles = {};

    if (anycell.value.parameter === 'SrcParameter' ||
      anycell.value.parameter === 'SrcIndex' ||
      anycell.value.parameter === 'DestParameter' ||
      anycell.value.parameter === 'DestIndex') {
      const textinputbox = document.getElementById(anycell.key);

      if (textinputbox !== undefined) {
        const activeelement = document.activeElement;
        // HTMLSelectElement
        // I am doing this here because a race condition exists where we are setting the value but the options has not been updated
        if (textinputbox.className === 'typeahead ng-untouched ng-pristine ng-valid' &&
            anycell.value.translatedPendingValue !== '0' &&
            (<HTMLInputElement>textinputbox).value !== anycell.value.translatedPendingValue) {
          (<HTMLInputElement>textinputbox).value = anycell.value.translatedPendingValue;
        } else if (anycell.value.translatedPendingValue === 'None' &&
        (<HTMLInputElement>textinputbox).value === '0' &&
        (<HTMLInputElement>textinputbox).value !== anycell.value.translatedPendingValue) {

          (<HTMLInputElement>textinputbox).value = anycell.value.translatedPendingValue;

        } else if (anycell.value.translatedPendingValue !== '0' &&
          (<HTMLInputElement>textinputbox).value === '' &&
            (activeelement === undefined ||
            activeelement.id !== (<HTMLInputElement>textinputbox).id)) {
          (<HTMLInputElement>textinputbox).value = anycell.value.translatedPendingValue;
        }
      }
    }
    return styles;
  }

  isParameterCell(cell: ICell) {
    const cellType = typeof cell.object;
    return (cellType === 'string') ? false : true;
  }

  isFieldDisabled(cell: any) {
    if (cell.value.parameterIsVisible === 0) {
      return '';
    } else if (cell.value.configClass === configClass.DYNAMIC) {
      return '';
    } else if (cell.value.configClass === configClass.COMMAND
      && this._rtuConfiguration.connectionStatus !== RTUConnectionStatus.CONNECTED) {
      return '';
    } else if (cell.value.configClass === configClass.CONFIG
      && (this._rtuConfiguration.connectionStatus === RTUConnectionStatus.WRITINGCONFIGURATION
        || this._rtuConfiguration.connectionStatus === RTUConnectionStatus.ERRORWRITINGCONFIGURATION
        || this._rtuConfiguration.connectionStatus === RTUConnectionStatus.READINGCONFIGURATION
        || this._rtuConfiguration.connectionStatus === RTUConnectionStatus.ERRORREADINGCONFIGURATION)) {
      return '';
    } else {
      return null;
    }
  }

  updatePendingValue(e: NgbTypeaheadSelectItemEvent, registerMap: registerMap, parameterSelected: IParameter) {
    const originalPendingValue = parameterSelected.pendingValue;

    const parameter = parameterSelected;
    const instance = this;
    parameterSelected.pendingValue = e.item.value;
    parameterSelected['temppendingValue'] = e.item;

    // set the value incase the user cancels
    if (parameterSelected.parameter === 'SrcIndex' ||
    parameterSelected.parameter === 'SrcParameter' ||
    parameterSelected.parameter === 'DestIndex' ||
    parameterSelected.parameter === 'DestParameter') {
      let visibleIdentifier = Object.keys(instance.typeaheadOptions).find(
        s => instance.typeaheadOptions[s].value === parameterSelected.value);
      if (visibleIdentifier !== undefined) {
        const translatedValue = instance.typeaheadOptions[visibleIdentifier].name;
        if (parameterSelected['tempValue'] === undefined) {
          parameterSelected['tempValue'] = translatedValue;
        }
      }
    }

    this.checkForIncrementGlobalPendingChanges(parameter, originalPendingValue);
    let registerMapPoint = registerMap.point as registerMapPoint;

    switch (instance.currentModBusType) {
      case modbusType.REGISTERMAPS:
        registerMapPoint.computedName = instance.getRegisterMapComputedName(registerMapPoint);
        registerMap.label = registerMapPoint.computedName;
        if (registerMap.label !== 'Undefined') {
          registerMap.labelLine1 = registerMapPoint.computedName.substring(0, registerMapPoint.computedName.indexOf('->'));
          registerMap.labelLine2 = registerMapPoint.computedName.substring((registerMapPoint.computedName.indexOf('->') +
              2), registerMapPoint.computedName.length);
        } else {
          registerMap.labelLine1 = 'Undefined';
          registerMap.labelLine2 = '';
        }

        if (instance.showPointIndexes) {
          registerMap.labelLine1 += ' (' + (registerMap.registerMapIdx + 1).toString().padStart(3, '0') + ')';
        }
        break;

      case modbusType.FLOATINGPOINTREGISTERS:
      case modbusType.INTEGERREGISTERS:
        registerMapPoint.computedName = instance.getFPIntRegisterComputedName(registerMapPoint);
        registerMap.label = registerMapPoint.computedName;
        registerMap.labelLine1 = registerMapPoint.computedName;
        registerMap.labelLine2 = '';

        if (instance.showPointIndexes) {
          registerMap.labelLine1 += ' (' + (registerMap.registerMapIdx + 1).toString().padStart(3, '0') + ')';
        }
        break;
    }

    instance.getEnabledRegisterMaps();
    instance.getRegisterMapSelectTab();
  }

  populateTypeaheadOptions(e: Event, registerMap: registerMap, parameterSelected: IParameter) {
    let parameterNameString = parameterSelected.parameter;
    let parameter = parameterSelected;
    const instance = this;
    let moduleConfiguration = this.rtuconfiguration.module0.moduleConfiguration;
    let PntType = (parameterNameString === 'SrcIndex' || parameterNameString === 'SrcParameter') ? 'SrcType': 'DestType'
    let PntTypeKey = Object.keys(registerMap.point.pointConfiguration).find(s => registerMap.point.pointConfiguration[s].parameter === PntType);

    switch (parameterNameString) {
      case 'SrcIndex':
      case 'DestIndex':
        instance.typeaheadOptions = [];
        instance.typeaheadOptions.push({ 'name': 'None', 'value': '0' });
        const moduleStrings = ['module1', 'module2', 'module3', 'module4', 'module5', 'module6'];
        const channelStrings = ['channel1', 'channel2', 'channel3', 'channel4', 'channel5', 'channel6', 'channel7', 'channel8'];
        switch (registerMap.point.pointConfiguration[PntTypeKey].pendingValue) {
          case '0': // undef
            break;
          case '1': // CPU Module

            const PntLabelKey = Object.keys(instance.rtuconfiguration.module0.moduleConfiguration).find(s => instance.rtuconfiguration.module0.moduleConfiguration[s].parameter === 'Label');
            instance.typeaheadOptions.push({ 'name': instance.rtuconfiguration.module0.moduleConfiguration[PntLabelKey].pendingValue, 'value': "1" });


            break;
          case '2': // Interface Module

            let index = 1;
            moduleStrings.forEach(function (moduleString) {
              const InterfacePntLabelKey = Object.keys(instance.rtuconfiguration[moduleString].moduleConfiguration).find(s => instance.rtuconfiguration[moduleString].moduleConfiguration[s].parameter === 'Label');
              const pointLabel = instance.rtuconfiguration[moduleString].moduleConfiguration[InterfacePntLabelKey].pendingValue;
              instance.typeaheadOptions.push({ 'name': pointLabel, 'value': index.toString() });
              index++;
            });

            break;
          case '3': // Port

             for (let i = 1; i < 57; i++) {
                const moduleStringsWithCPU = ['module0', 'module1', 'module2', 'module3', 'module4', 'module5', 'module6'];
                const moduleNumber = Math.floor((i - 1) / 8);
                const selectedModule = instance.rtuconfiguration[moduleStringsWithCPU[moduleNumber]];
                let modLabelKey = Object.keys(selectedModule.moduleConfiguration).find(s => selectedModule.moduleConfiguration[s].parameter === 'Label');
                const moduleLabelString = selectedModule.moduleConfiguration[modLabelKey].pendingValue;
                const channelNumber = (((i - 1) % 8) + 1);
                const selectedChannel = selectedModule[channelStrings[channelNumber - 1]];
                let PntLabelKey = Object.keys(selectedChannel.channelConfiguration).find(s => selectedChannel.channelConfiguration[s].parameter === 'Label');
                const channelLabelString = selectedChannel.channelConfiguration[PntLabelKey].pendingValue;
                instance.typeaheadOptions.push({ 'name': moduleLabelString + ' ' + channelLabelString, 'value': i.toString() });
              }

            break;
          case '4': // FP Register
            if (parameterNameString === 'SrcIndex') {
              for (let i = 1; i < 101; i++) {
                instance.typeaheadOptions.push({ 'name': 'FP Register ' + i, 'value': i.toString() });
              }
            }
            break;
          case '5': // Integer Register
            if (parameterNameString === 'SrcIndex') {
              for (let i = 1; i < 101; i++) {
                instance.typeaheadOptions.push({ 'name': 'Integer Register ' + i, 'value': i.toString() });
              }
            }
            break;
          case '6': // Gateway Block
            break;
          case '7': // Tank

            let numberOfTanks;
            const numberOfTanksIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfTanks');
            if (numberOfTanksIdentifier) {
              numberOfTanks = parseInt(moduleConfiguration[numberOfTanksIdentifier].value, 10);
            }
            const tanks = this.rtuconfiguration.points.filter(x => x.name === 'Tank');

            for (let i = 1; i <= numberOfTanks; i++) {
              const tank = tanks[i - 1];
              const PntLabelKey = Object.keys(tank.pointConfiguration).find(s => tank.pointConfiguration[s].parameter === 'Label');
              instance.typeaheadOptions.push({ 'name': tank.pointConfiguration[PntLabelKey].pendingValue, 'value': i.toString() });
            }

            break;
          case '8': // Alarm
            let numberOfalarms = 0;
            let numberOfalarmsIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfAlarms');
            if (numberOfalarmsIdentifier) {
              numberOfalarms = parseInt(moduleConfiguration[numberOfalarmsIdentifier].pendingValue, 10);
            }
            for (let i = 1; i <= numberOfalarms; i++) {

              const zerofilled = 'Alarm ' + ('0000' + i).slice(-4);

              instance.typeaheadOptions.push({ 'name': zerofilled, 'value': i.toString() });

            }

            break;
        }

        break;
      case 'SrcParameter':
      case 'DestParameter':

        instance.pntParametertypeaheadOptions = [];

        // get the selected point type
        if (PntTypeKey === '') {
          return;
        }
        const pntTypeString = PointTypes.split(',')[parseInt(registerMap.point.pointConfiguration[PntTypeKey].pendingValue, 10) - 1];
        if (!pntTypeString || pntTypeString === '') {
          instance.typeaheadOptions = [{ 'name': 'None', 'value': '0' }];
          break;
        }

        instance.pntParametertypeaheadOptions.push({ 'name': 'None', 'value': '0' });
        // go through the rtuconfiguration object and add the variables needed for this point type
        for (const deviceType in this.rtuconfiguration.pointRefMapNumberLookupDictionary) {
          if (this.rtuconfiguration.pointRefMapNumberLookupDictionary.hasOwnProperty(deviceType)) {
            const pointNameString = this.rtuconfiguration.pointRefMapNumberLookupDictionary[deviceType].pointName;
            const nameString = this.rtuconfiguration.pointRefMapNumberLookupDictionary[deviceType].variableName;
            const valueString = this.rtuconfiguration.pointRefMapNumberLookupDictionary[deviceType].alarmNumber;
            if (pointNameString.toUpperCase() === pntTypeString.toUpperCase()) {
              instance.pntParametertypeaheadOptions.push({ 'name': nameString, 'value': valueString });
            }
          }
        }
        instance.typeaheadOptions = instance.pntParametertypeaheadOptions;
        break;
    }

    setTimeout(() => {
      let event;
      if (typeof (Event) === 'function') {
        event = new Event('input');
      } else {
        event = document.createEvent('Event');
        event.initEvent('input', true, true);
      }
      e.target.dispatchEvent(event);
    }, 0);
  }


  typeaheadSearch = (text$: Observable<string>) =>
  text$.pipe(
    debounceTime(200),
    distinctUntilChanged(),
    map(term => term === '' ? this.typeaheadOptions.slice(0, 101)
      : this.typeaheadOptions.filter(v => v.name.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 101))
  )


  revertTypeaheads() {
    const instance = this;

    switch (instance.currentModBusType) {
      case modbusType.REGISTERMAPS:
        instance.enabledRegisterMaps.forEach(function (registerMap) {
          // Src Point
          let pntIndexKey = Object.keys(registerMap.point.pointConfiguration).find(
              s => registerMap.point.pointConfiguration[s].parameter === 'SrcIndex');
          let PntTypeKey = Object.keys(registerMap.point.pointConfiguration).find(
              s => registerMap.point.pointConfiguration[s].parameter === 'SrcType');
          let pntIndexParameter = registerMap.point.pointConfiguration[pntIndexKey];

          pntIndexParameter.pendingValue = pntIndexParameter.value;
          // if port
          if (registerMap.point.pointConfiguration[PntTypeKey].pendingValue === '3') {
            let registerMapPoint = registerMap.point as registerMapPoint;
            pntIndexParameter.temppendingValue = {
              name: instance.getPntIndexNameForValue(registerMapPoint, pntIndexKey),
              value: pntIndexParameter.pendingValue
            };
          } else if (pntIndexParameter.tempValue) {
            pntIndexParameter.temppendingValue = pntIndexParameter.tempValue;
          }

          let pntParameterKey = Object.keys(registerMap.point.pointConfiguration).find(
              s => registerMap.point.pointConfiguration[s].parameter === 'SrcParameter');
          let pntParameterparameter = registerMap.point.pointConfiguration[pntParameterKey];
          if (pntParameterparameter.tempValue) {
            pntParameterparameter.temppendingValue = pntParameterparameter.tempValue;
          }

          // Dest Point
          pntIndexKey = Object.keys(registerMap.point.pointConfiguration).find(
                s => registerMap.point.pointConfiguration[s].parameter === 'DestIndex');
          PntTypeKey = Object.keys(registerMap.point.pointConfiguration).find(
                s => registerMap.point.pointConfiguration[s].parameter === 'DestType');
          pntIndexParameter = registerMap.point.pointConfiguration[pntIndexKey];

          pntIndexParameter.pendingValue = pntIndexParameter.value;
          // if port
          if (registerMap.point.pointConfiguration[PntTypeKey].pendingValue === '3') {
            let registerMapPoint = registerMap.point as registerMapPoint;
            pntIndexParameter.temppendingValue = {
              name: instance.getPntIndexNameForValue(registerMapPoint, pntIndexKey),
              value: pntIndexParameter.pendingValue
            };
          } else if (pntIndexParameter.tempValue) {
            pntIndexParameter.temppendingValue = pntIndexParameter.tempValue;
          }

          pntParameterKey = Object.keys(registerMap.point.pointConfiguration).find(
              s => registerMap.point.pointConfiguration[s].parameter === 'DestParameter');
          pntParameterparameter = registerMap.point.pointConfiguration[pntParameterKey];
          if (pntParameterparameter.tempValue) {
            pntParameterparameter.temppendingValue = pntParameterparameter.tempValue;
          }
        });
        break;

      default:
        break;
    }
  }

  cancelChanges() {
    const instance = this;

    this._rtuConfiguration.cancelPendingChanges();
    this.revertTypeaheads();

    // this is kind of screwy but the above will result if the type combo box being reset so we need to call cancelpendingchanges again
    setTimeout(() => { this._rtuConfiguration.cancelPendingChanges(); }, 1);
    this.modalRef.hide();
  }

  saveRtuConfigToDisk() {
    let configToSave;
    if (this.rtuconfiguration) {
      configToSave = JSON.stringify(this.rtuconfiguration);
    } else {
      configToSave = 'no data';
    }

    const data = new Blob([configToSave], { type: 'application/json' });

    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
      window.navigator.msSaveOrOpenBlob(data, 'config.rtuconfig');
    } else {
      saveAs(data, 'config.rtuconfig');
    }
  }


  onParameterChange(parameter: IParameter, registerMap: registerMap) {
    const instance = this;

    if (parameter.configClass === configClass.CONFIG) {
      instance.selectedRegisterMap = instance.enabledRegisterMaps.find(s => s.pointId === registerMap.pointId);
      const registerMapPoint = registerMap.point as registerMapPoint;

      switch (instance.currentModBusType) {
        case modbusType.REGISTERMAPS:

          // if the pnttype has changed populate the type ahead options for the pntparameter
          if (parameter.parameter === 'SrcType') {
            let PntIndexKey = Object.keys(registerMapPoint.pointConfiguration).find(
                s => registerMapPoint.pointConfiguration[s].parameter === 'SrcIndex');
            let PntParameterKey = Object.keys(registerMapPoint.pointConfiguration).find(
                s => registerMapPoint.pointConfiguration[s].parameter === 'SrcParameter');

            // if the user changed the point type we need to store the original value just incase the user presses cancel
            const mapRegisterPointIndex = registerMapPoint.pointConfiguration[PntIndexKey];

            if (mapRegisterPointIndex['tempValue'] === undefined &&
              mapRegisterPointIndex.translatedPendingValue !== undefined &&
              mapRegisterPointIndex.translatedPendingValue !== '') {
                mapRegisterPointIndex['tempValue'] = mapRegisterPointIndex.translatedPendingValue;
            }

            const mapRegisterPointParameter = registerMapPoint.pointConfiguration[PntParameterKey];

            if (mapRegisterPointParameter['tempValue'] === undefined &&
              mapRegisterPointParameter.translatedPendingValue !== undefined &&
              mapRegisterPointParameter.translatedPendingValue !== '') {
              mapRegisterPointParameter['tempValue'] = mapRegisterPointParameter.translatedPendingValue;
            }

            registerMapPoint.pointConfiguration[PntIndexKey].temppendingValue = 'None'; // what the typeahead is bound to
            registerMapPoint.pointConfiguration[PntParameterKey].temppendingValue = 'None';
            registerMapPoint.pointConfiguration[PntIndexKey].translatedPendingValue = 'None'; // what the placeholder is bound to
            registerMapPoint.pointConfiguration[PntParameterKey].translatedPendingValue = 'None';
            const pntIndexKeyOriginalPendingValue = registerMapPoint.pointConfiguration[PntIndexKey].pendingValue;
            const pntParamKeyOriginalPendingValue = registerMapPoint.pointConfiguration[PntParameterKey].pendingValue;

            registerMapPoint.pointConfiguration[PntIndexKey].pendingValue = '0';
            registerMapPoint.pointConfiguration[PntParameterKey].pendingValue = '0';
            this.checkForIncrementGlobalPendingChanges(registerMapPoint.pointConfiguration[PntIndexKey], pntIndexKeyOriginalPendingValue);
            this.checkForIncrementGlobalPendingChanges(registerMapPoint.pointConfiguration[PntParameterKey], pntParamKeyOriginalPendingValue);
          }

          if (parameter.parameter === 'DestType') {
            const PntIndexKey = Object.keys(registerMapPoint.pointConfiguration).find(
                  s => registerMapPoint.pointConfiguration[s].parameter === 'DestIndex');
            let PntParameterKey = Object.keys(registerMapPoint.pointConfiguration).find(
                  s => registerMapPoint.pointConfiguration[s].parameter === 'DestParameter');

            // if the user changed the point type we need to store the original value just incase the user presses cancel
            const mapRegisterPointIndex = registerMapPoint.pointConfiguration[PntIndexKey];

            if (mapRegisterPointIndex['tempValue'] === undefined &&
              mapRegisterPointIndex.translatedPendingValue !== undefined &&
              mapRegisterPointIndex.translatedPendingValue !== '') {
              mapRegisterPointIndex['tempValue'] = mapRegisterPointIndex.translatedPendingValue;
            }

            const mapRegisterPointParameter = registerMapPoint.pointConfiguration[PntParameterKey];

            if (mapRegisterPointParameter['tempValue'] === undefined &&
              mapRegisterPointParameter.translatedPendingValue !== undefined &&
              mapRegisterPointParameter.translatedPendingValue !== '') {
                mapRegisterPointParameter['tempValue'] = mapRegisterPointParameter.translatedPendingValue;
            }

            registerMapPoint.pointConfiguration[PntIndexKey].temppendingValue = 'None'; // what the typeahead is bound to
            registerMapPoint.pointConfiguration[PntParameterKey].temppendingValue = 'None';
            registerMapPoint.pointConfiguration[PntIndexKey].translatedPendingValue = 'None'; // what the placeholder is bound to
            registerMapPoint.pointConfiguration[PntParameterKey].translatedPendingValue = 'None';
            const pntIndexKeyOriginalPendingValue = registerMapPoint.pointConfiguration[PntIndexKey].pendingValue;
            const pntParamKeyOriginalPendingValue = registerMapPoint.pointConfiguration[PntParameterKey].pendingValue;

            registerMapPoint.pointConfiguration[PntIndexKey].pendingValue = '0';
            registerMapPoint.pointConfiguration[PntParameterKey].pendingValue = '0';
            this.checkForIncrementGlobalPendingChanges(registerMapPoint.pointConfiguration[PntIndexKey], pntIndexKeyOriginalPendingValue);
            this.checkForIncrementGlobalPendingChanges(registerMapPoint.pointConfiguration[PntParameterKey],
                pntParamKeyOriginalPendingValue);
          }

          registerMapPoint.computedName = instance.getRegisterMapComputedName(registerMapPoint);
          registerMap.label = registerMapPoint.computedName;

          if (registerMap.label !== 'Undefined') {
            registerMap.labelLine1 = registerMapPoint.computedName.substring(0, registerMapPoint.computedName.indexOf('->'));
            registerMap.labelLine2 = registerMapPoint.computedName.substring((registerMapPoint.computedName.indexOf('->') + 2),
                registerMapPoint.computedName.length);
          } else {
            registerMap.labelLine1 = 'Undefined';
            registerMap.labelLine2 = '';
          }

          if (instance.showPointIndexes) {
            registerMap.labelLine1 += ' (' + (registerMap.registerMapIdx + 1).toString().padStart(3, '0') + ')';
          }

          instance.getEnabledRegisterMaps();
          instance.getRegisterMapSelectTab();
          break;

        case modbusType.FLOATINGPOINTREGISTERS:
        case modbusType.INTEGERREGISTERS:
          registerMapPoint.computedName = instance.getFPIntRegisterComputedName(registerMapPoint);
          registerMap.label = registerMapPoint.computedName;
          registerMap.labelLine1 = registerMapPoint.computedName;
          registerMap.labelLine2 = '';

          if (instance.showPointIndexes) {
            registerMap.labelLine1 += ' (' + (registerMap.registerMapIdx + 1).toString().padStart(3, '0') + ')';
          }

          instance.getEnabledRegisterMaps();
          instance.getRegisterMapSelectTab();
          break;

        default:
          break;
      }
    } else if (parameter.configClass === configClass.COMMAND) {
      this._rtuConfiguration.applyCommandToRTU(parameter);
    }
  }

  togglePointIdx() {
    const instance = this;
    this.showPointIndexes = !this.showPointIndexes;
    localStorage.setItem('showPointIndexes', this.showPointIndexes.toString());
    this.getEnabledRegisterMaps();
    this.getRegisterMapSelectTab();
    this.getAlarmViewTab();
    this.updateSelectedRegisterMapLabels();
  }

  changeModbusType(newType: modbusType) {
    const instance = this;
    console.log('Switching Modbus Type to ' + newType);
    instance.currentModBusType = newType;
    localStorage.setItem('currentModBusType', newType.toString());
    instance.selectedRegisterMaps = [];
    instance.selectedRegisterMap = null;
    this.getEnabledRegisterMaps();
    this.getRegisterMapSelectTab();
    this.getAlarmViewTab();
    this.updateSelectedRegisterMapLabels();
  }

  public checkForIncrementGlobalPendingChanges(parameter: IParameter, originalPendingValue: string) {
    const instance = this;
    if (instance._rtuConfiguration.isParameterValueChanged(parameter)) {
      if (originalPendingValue === parameter.value) {
        instance._rtuConfiguration.incrementGlobalPendingChanges();
      }
    } else if (originalPendingValue !== parameter.value) {
        instance._rtuConfiguration.decrementGlobalPendingChanges();
    }
  }


  inputFormatter(value: any) {
    if (typeof value === 'string') {
      return value;
    } else if (value.value === 0 || value.value === '0') {
      return 0;
    }
      return value.name;
  }


  resultFormatter(value: any) {
    return value.name;
  }

  getPntIndexNameForValue(registerMapPoint: registerMapPoint, pointIndexKey: string) {
    const instance = this;
    const options = {};

    for ( let i = 1; i < 57; i++) {
      const moduleStringsWithCPU = ['module0', 'module1', 'module2', 'module3', 'module4', 'module5', 'module6'];
      const channelStrings = ['channel1', 'channel2', 'channel3', 'channel4', 'channel5', 'channel6', 'channel7', 'channel8'];

      const moduleNumber = Math.floor((i - 1) / 8);
      const selectedModule = instance.rtuconfiguration[moduleStringsWithCPU[moduleNumber]];
      const modLabelKey = Object.keys(selectedModule.moduleConfiguration).find(s => selectedModule.moduleConfiguration[s].parameter === 'Label');
      const moduleLabelString = selectedModule.moduleConfiguration[modLabelKey].pendingValue;
      const channelNumber = (((i - 1) % 8) + 1);
      const selectedChannel = selectedModule[channelStrings[channelNumber - 1]];
      const PntLabelKey = Object.keys(selectedChannel.channelConfiguration).find(s => selectedChannel.channelConfiguration[s].parameter === 'Label');
      const channelLabelString = selectedChannel.channelConfiguration[PntLabelKey].pendingValue;
      options[i] = moduleLabelString + ' ' + channelLabelString;
    }
    return options[registerMapPoint.pointConfiguration[pointIndexKey].pendingValue];

  }

  VerifyApplytoRTUPrompt(): void {
    this.modalRef = this._modalService.show(this.modalVerifyApplytoRTUAction, this.modalConfig);
  }

  applyToRTU() {
    const instance = this;

    this._rtuConfiguration.applyDataToRTU(false, this._rtuConfiguration.checkForRTUConfigChanges());
    if (this.modalRef !== undefined) {
      this.modalRef.hide();
    }

    switch (instance.currentModBusType) {
      case modbusType.REGISTERMAPS:
        // since we are doing our own change detect for the type ahead drop down list we need to reset the values here
        instance.enabledRegisterMaps.forEach(function (registerMap) {
          let pointIndexKey = Object.keys(registerMap.point.pointConfiguration).find(
              s => registerMap.point.pointConfiguration[s].parameter === 'SrcIndex');
          let pntIndexParameter = registerMap.point.pointConfiguration[pointIndexKey];

          if (pntIndexParameter['tempValue'] !== undefined && pntIndexParameter['tempValue'] !== 'None') {
            pntIndexParameter['tempValue'] = 'None';
          }

          let pointParameterKey = Object.keys(registerMap.point.pointConfiguration).find(
              s => registerMap.point.pointConfiguration[s].parameter === 'SrcParameter');
          let pntParameterParameter = registerMap.point.pointConfiguration[pointParameterKey];

          if (pntParameterParameter['tempValue'] !== undefined && pntParameterParameter['tempValue'] !== 'None') {
            pntParameterParameter['tempValue'] = 'None';
          }

          pointIndexKey = Object.keys(registerMap.point.pointConfiguration).find(
              s => registerMap.point.pointConfiguration[s].parameter === 'DestIndex');
          pntIndexParameter = registerMap.point.pointConfiguration[pointIndexKey];

          if (pntIndexParameter['tempValue'] !== undefined && pntIndexParameter['tempValue'] !== 'None') {
            pntIndexParameter['tempValue'] = 'None';
          }

          pointParameterKey = Object.keys(registerMap.point.pointConfiguration).find(
              s => registerMap.point.pointConfiguration[s].parameter === 'DestParameter');
          pntParameterParameter = registerMap.point.pointConfiguration[pointParameterKey];

          if (pntParameterParameter['tempValue'] !== undefined && pntParameterParameter['tempValue'] !== 'None') {
            pntParameterParameter['tempValue'] = 'None';
          }
        });
        break;

      default:
        break;
    }
  }
}

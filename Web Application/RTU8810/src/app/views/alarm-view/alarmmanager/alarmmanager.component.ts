import { Component, OnInit, ViewEncapsulation, ChangeDetectionStrategy, ElementRef, TemplateRef, ViewChild } from '@angular/core';
import { RtuconfigurationService, IRTUConfiguration, IPoint, IPointData } from 'src/app/services/rtuconfiguration.service';
import { IParameter, configClass, allProtocols, IParameterMap, alarmTypes } from 'src/app/services/availablemodules.service';
import * as saveAs from 'node_modules/file-saver';
import { FindValueSubscriber } from 'rxjs/internal/operators/find';
import { Subscription } from 'rxjs';
// import { instantiateRootComponent } from '@angular/core/src/render3/instructions';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';
import { Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { NgbTypeahead, NgbTypeaheadSelectItemEvent } from '@ng-bootstrap/ng-bootstrap';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { HostListener } from '@angular/core';


const alarmEnabledUndefinedImage = "./assets/alarm-active-undefined.png";
const alarmDisabledUndefinedImage = "./assets/alarm-inactive-undefined.png";
const alarmSelectedUndefinedImage = "./assets/alarm-selected-undefined.png";

const alarmEnabledImage = "./assets/alarm-active-channel.png";
const alarmDisabledImage = "./assets/alarm-inactive-channel.png";
const alarmSelectedImage = './assets/alarm-selected-channel.png';
const alarmWarningImage = './assets/alarm-warning-channel.png';
const alarmWarningSelectedImage = './assets/alarm-warning-channel-selected.png';

const alarmTankEnabledImage = "./assets/alarm-active-tank.png";
const alarmTankDisabledImage = "./assets/alarm-inactive-tank.png";
const alarmTankSelectedImage = './assets/alarm-selected-tank.png';
const alarmTankWarningImage = './assets/alarm-warning-tank.png';
const alarmTankWarningSelectedImage = './assets/alarm-warning-tank-selected.png';

interface ICell {
  tabIndex: number;
  object: any;
  disableOverride: boolean;
}

class IAlarmParameterMap implements IParameterMap {
  [identifier: number]: IAlarmParameter;
}

class IAlarmParameter implements IParameter {
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
  translatedPendingValue: string; //for PntIndex and PntParameter
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


interface ITab {
  tabName: string;
}

interface ISelectedAlarm {
  computedName: string;
  parameters: IParameter[];
}

interface typeaheadOption {
  name: string;
  value: string;
}

class alarmSet {
  alarms: alarm[];
}

class alarmPoint implements IPoint {
  name: string;
  pointConfiguration: IAlarmParameterMap;
  computedName: string;

}

class alarm {
  pointId: number;
  alarmId: number;
  label: string;
  labelLine1: string;
  labelLine2: string;
  alarmlistimg: string;
  activationimg: string;
  point: IPoint;
  outputs: any = {};
  outputPairs: string[];

  constructor(pointId, alarmId, label, point) {
    this.pointId = pointId;
    this.alarmId = alarmId;
    this.label = label;

    let outModKey = Object.keys(point.pointConfiguration).find(s => point.pointConfiguration[s].parameter === 'OutModules');
    let outChanKey = Object.keys(point.pointConfiguration).find(s => point.pointConfiguration[s].parameter === 'OutChannels');
    let modString = point.pointConfiguration[outModKey].pendingValue;
    let chanString = point.pointConfiguration[outChanKey].pendingValue;
    this.outputPairs = [];
    var m;
    var c;
    for (m = 1; m < 7; m++) {
      for (c = 1; c < 9; c++)
        this.outputs[m.toString() + c.toString()] = false;
    }

    if (modString) {
      for (var i = 0; i < modString.length; i++) {
        this.outputs[modString.charAt(i) + chanString.charAt(i)] = true;
        this.outputPairs.push(modString.charAt(i) + chanString.charAt(i));
      }
    }


    if (label != 'Undefined') {
      this.labelLine1 = label.substring(0, label.indexOf('.'));
      this.labelLine2 = label.substring((label.indexOf('.') + 1), label.length);
    }
    else {
      this.labelLine1 = 'Undefined';
      this.labelLine2 = '';
    }
    this.point = point;
    this.alarmlistimg = alarmEnabledImage;
    this.activationimg;
  }
}


@Component({
  selector: 'app-alarmmanager',
  templateUrl: './alarmmanager.component.html',
  styleUrls: ['./alarmmanager.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class AlarmmanagerComponent implements OnInit {
  configurationColumns = 3;
  selectedalarm: alarm;
  selectedalarms: alarm[] = [];
  batchEditalarm: alarm;
  alarmViewTabs: ITab[];
  alarmParameters: IParameter[];
  alarmsToChangeVisibility: IPointData[];
  enabledAlarms: alarm[];
  alarmPanelAlarms: alarm[];
  alarmPanelAlarmSets: any[];
  alarmsToActivate: number[];
  alarmVisibleIdentifier: number;
  labelIdentifier: number;
  searchalarmToggle = false;
  searchalarmString = '';
  searchConfigurationToggle = false;
  searchConfigurationString = '';
  isLoaded: boolean;
  alarmActivationMode: string;
  alarmActivationApplyButtonText: string;
  rtuconfiguration: IRTUConfiguration;
  activationActive = false;
  setActivateMultiLabel: string = '';
  autoIncrementParameterName: string;
  rtuconfigurationSubscription: Subscription;
  liveDataValuesSubscription: Subscription;
  dragSelectActive = false;
  typeaheadOptions: typeaheadOption[] = [];
  pntParametertypeaheadOptions: typeaheadOption[] = [];
  outputModChanOptions: any[] = [];
  ready = false;
  activeDropdown: any;
  showPointIndexes = false;

  setActiveDropdown(dropdown: any) {
    this.activeDropdown = dropdown;
  }

  dropdownFocus(dropdown: any) {

    setTimeout(function () {
      if (!dropdown.isOpen) //so as not to interfere with the actual control's implementation 
      {
        dropdown.show();
      }
    }, 90);

  }

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (event.keyCode == 9 && this.activeDropdown) {
      this.activeDropdown.hide();
    }
  }

  updateOutputParameters(alarm: alarm) {
    let outModKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'OutModules');
    let outChanKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'OutChannels');
    let originalModPendingValue = alarm.point.pointConfiguration[outModKey].pendingValue;
    let originalChanPendingValue = alarm.point.pointConfiguration[outChanKey].pendingValue;
    alarm.outputPairs = [];
    let constructedModString = "";
    let constructedChannelString = "";
    var m;
    var c;
    for (m = 1; m < 7; m++) {
      for (c = 1; c < 9; c++)
        if (alarm.outputs[m.toString() + c.toString()] == true) {
          constructedModString += m.toString();
          constructedChannelString += c.toString();
          alarm.outputPairs.push(m.toString() + c.toString());
        }
    }

    if (constructedModString == "") {
      constructedModString = null; //rtu default is null
      constructedChannelString = null;
    }
    alarm.point.pointConfiguration[outModKey].pendingValue = constructedModString;
    alarm.point.pointConfiguration[outChanKey].pendingValue = constructedChannelString;

    this.checkForIncrementGlobalPendingChanges(alarm.point.pointConfiguration[outModKey], originalModPendingValue);
    this.checkForIncrementGlobalPendingChanges(alarm.point.pointConfiguration[outChanKey], originalChanPendingValue);
  }

  valueAscOrder = (a: any, b: any): number => {
    return (a.value > b.value) ? -1 : 1;
  }


  updateOutputControl(alarm: alarm) {
    let outModKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'OutModules');
    let outChanKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'OutChannels');
    let modPendingValue = alarm.point.pointConfiguration[outModKey].pendingValue;
    let chanPendingValue = alarm.point.pointConfiguration[outChanKey].pendingValue;
    alarm.outputPairs = [];

    for (var m = 1; m < 7; m++) {
      for (var c = 1; c < 9; c++) {
        alarm.outputs[m.toString() + c.toString()] = false;
      }
    }

    if (modPendingValue)
      for (var i = 0; i < modPendingValue.length; i++) {
        alarm.outputs[modPendingValue.charAt(i) + chanPendingValue.charAt(i)] = true;
        alarm.outputPairs.push(modPendingValue.charAt(i) + chanPendingValue.charAt(i));
      }
  }

  onRemoveOuputPair(pair: string, alarm: alarm) {
    alarm.outputs[pair] = false;
    this.updateOutputParameters(alarm);
  }

  ngAfterViewInit() { setTimeout(() => this.ready = true); };
  typeaheadSearch = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map(term => term === '' ? this.typeaheadOptions.slice(0, 101)
        : this.typeaheadOptions.filter(v => v.name.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 101))
    );


  inputFormatter(value: any) {
    if (typeof value === 'string')
      return value;
    else if (value.value == 0 || value.value == "0")
      return 0;
    return value.name;
  }


  resultFormatter(value: any) {
    return value.name;
  }

  modalRef: BsModalRef;
  modalConfig = {
    backdrop: true,
    ignoreBackdropClick: false,
    class: 'modal-lg'
  };

  @ViewChild('activateLabelModalBackground', { static: false }) activateLabelModalBackground: ElementRef;
  @ViewChild('batchRenameLabel', { static: false }) batchRenameLabel: ElementRef;
  @ViewChild('modalVerifyApplytoRTUAction', { static: true }) public modalVerifyApplytoRTUAction: TemplateRef<any>;
  @ViewChild('modalVerifyCancelAction', { static: true }) public modalVerifyCancelAction: TemplateRef<any>;
  @ViewChild('modalVerifyCancelActivation', { static: true }) public modalVerifyCancelActivation: TemplateRef<any>;
  @ViewChild('modalNothingtowritetoRTUAction', { static: true }) public modalNothingtowritetoRTUAction: TemplateRef<any>;

  constructor(
    private _rtuConfiguration: RtuconfigurationService,
    private _modalService: BsModalService) {
    this.isLoaded = false;
  }

  ngOnInit() {
    this.showPointIndexes = (localStorage.getItem('showPointIndexes') === 'true') ? true : false;
    this.getRTUConfiguration();
  }

  ngOnDestroy() {
    if (this._rtuConfiguration) {
      this._rtuConfiguration.unsubscribeRealtimeParameters(this.alarmParameters);
    }
    if (this.rtuconfigurationSubscription) {
      this.rtuconfigurationSubscription.unsubscribe();
    }
  }

  trackByAlarmId(index, item) {
    return item[0].pointId;
  }
  comparealarms(a: alarm, b: alarm) {
    let AalarmStateKey = Object.keys(a.point.pointConfiguration).find(s => a.point.pointConfiguration[s].parameter === 'AlarmState');

    let AalarmStateValue = a.point.pointConfiguration[AalarmStateKey].pendingValue;

    let BalarmStateKey = Object.keys(b.point.pointConfiguration).find(s => b.point.pointConfiguration[s].parameter === 'AlarmState');

    let BlarmStateValue = b.point.pointConfiguration[BalarmStateKey].pendingValue;

    if (AalarmStateValue > BlarmStateValue)
      return -1;
    if (AalarmStateValue < BlarmStateValue)
      return 1;

    if (a.label == "Undefined" && b.label != "Undefined")
      return 1;
    if (b.label == "Undefined" && a.label != "Undefined")
      return -1;

    if (a.label < b.label)
      return -1;
    if (a.label > b.label)
      return 1;
    return 0;
  }

  getActivealarmClass(i: number) {
    if ((i - 1) % 4 === 0) {
      return "alarm-border";
    }
    else {
      return "alarm-plain";
    }
  }


  getActivationalarmClass(i: number) {
    if ((i - 1) % 4 === 0) {
      return "alarm-border";
    }
    else if ((i - 3) % 4 === 0) {
      return "alarm-margin-right";
    }
    else if ((i - 4) % 4 === 0) {
      return "alarm-margin-left";
    }
    else {
      return "alarm-plain";
    }
  }

  populateTypeaheadOptions(e: Event, alarm: alarm, parameterSelected: IParameter) {
    let parameterNameString = parameterSelected.parameter;
    let parameter = parameterSelected;
    let instance = this;
    let moduleConfiguration = this.rtuconfiguration.module0.moduleConfiguration;
    let PntTypeKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntType');

    switch (parameterNameString) {
      case "PntIndex":
        instance.typeaheadOptions = [];
        instance.typeaheadOptions.push({ 'name': 'None', 'value': '0' });
        let moduleStrings = ["module1", "module2", "module3", "module4", "module5", "module6"];
        let channelStrings = ["channel1","channel2","channel3","channel4","channel5","channel6","channel7","channel8"];
        switch (alarm.point.pointConfiguration[PntTypeKey].pendingValue) {
          case "0": //undef
            break;
          case "1": //CPU Module

            const PntLabelKey = Object.keys(instance.rtuconfiguration.module0.moduleConfiguration).find(s => instance.rtuconfiguration.module0.moduleConfiguration[s].parameter === 'Label');
            instance.typeaheadOptions.push({ 'name': instance.rtuconfiguration.module0.moduleConfiguration[PntLabelKey].pendingValue, 'value': "1" });


            break;
          case "2": //Interface Module

            let index = 1;
            moduleStrings.forEach(function (moduleString) {
              const InterfacePntLabelKey = Object.keys(instance.rtuconfiguration[moduleString].moduleConfiguration).find(s => instance.rtuconfiguration[moduleString].moduleConfiguration[s].parameter === 'Label');
              let pointLabel = instance.rtuconfiguration[moduleString].moduleConfiguration[InterfacePntLabelKey].pendingValue;
              instance.typeaheadOptions.push({ 'name': pointLabel, 'value': index.toString() })
              index++;
            });

            break;
          case "3": //Port

            var i;

              for (i = 1; i < 57; i++) {
                let moduleStringsWithCPU = ["module0","module1", "module2", "module3", "module4", "module5", "module6"];
                const moduleNumber = Math.floor((i - 1) / 8); 
                let selectedModule = instance.rtuconfiguration[moduleStringsWithCPU[moduleNumber]]; 
                let modLabelKey = Object.keys(selectedModule.moduleConfiguration).find(s => selectedModule.moduleConfiguration[s].parameter === 'Label');
                let moduleLabelString = selectedModule.moduleConfiguration[modLabelKey].pendingValue;
                const channelNumber =(((i - 1) % 8) + 1); 
                let selectedChannel = selectedModule[channelStrings[channelNumber-1]];
                let PntLabelKey = Object.keys(selectedChannel.channelConfiguration).find(s => selectedChannel.channelConfiguration[s].parameter === 'Label');
                let channelLabelString = selectedChannel.channelConfiguration[PntLabelKey].pendingValue;
                instance.typeaheadOptions.push({ 'name': moduleLabelString + " " + channelLabelString, 'value': i });
              }
            

            break;
          case "4": //FP Register
            var i;
            for (i = 1; i < 101; i++) {
              instance.typeaheadOptions.push({ 'name': "FP Register " + i, 'value': i });
            }
            break;
          case "5": //Integer Register
            var i;
            for (i = 1; i < 101; i++) {
              instance.typeaheadOptions.push({ 'name': "Integer Register " + i, 'value': i });
            }
            break;
          case "6": //Gateway Block
            var i;
            for (i = 1; i < 10; i++) {
              instance.typeaheadOptions.push({ 'name': "Gateway Block " + i, 'value': i });
            }
            break;
          case "7": //Tank

            let numberOfTanks;
            const numberOfTanksIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfTanks');
            if (numberOfTanksIdentifier) {
              numberOfTanks = parseInt(moduleConfiguration[numberOfTanksIdentifier].value);
            }
            var i;
            const tanks = this.rtuconfiguration.points.filter(x => x.name === 'Tank');

            for (i = 1; i <= numberOfTanks; i++) {
              const tank = tanks[i - 1];
              const PntLabelKey = Object.keys(tank.pointConfiguration).find(s => tank.pointConfiguration[s].parameter === 'Label');
              instance.typeaheadOptions.push({ 'name': tank.pointConfiguration[PntLabelKey].pendingValue, 'value': (parseInt(i)).toString() });
            }

            break;
          case "8": //Alarm
            let numberOfalarms;
            let numberOfalarmsIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfAlarms');
            if (numberOfalarmsIdentifier) {
              numberOfalarms = parseInt(moduleConfiguration[numberOfalarmsIdentifier].pendingValue);
            }

            var i;
            for (i = 1; i <= numberOfalarms; i++) {

              let foundAlarm = instance.enabledAlarms.find(s => s.pointId === i + 399);

              foundAlarm.alarmId++;
              // alarm selection will be alarm pnt ###
              var zerofilled = 'Alarm Pnt ' + ('0000' + foundAlarm.alarmId).slice(-4);

              instance.typeaheadOptions.push({ 'name': zerofilled, 'value': (parseInt(i)).toString() });

            }
            break;
        }

        break;
      case "PntParameter":

        instance.pntParametertypeaheadOptions = [];

        // get the selected point type
        if (PntTypeKey === "")
          return;
        let pntTypeString = alarm.point.pointConfiguration[PntTypeKey].availableCommands.split(',')[parseInt(alarm.point.pointConfiguration[PntTypeKey].pendingValue) - 1];
        if (!pntTypeString || pntTypeString === "") {
          instance.typeaheadOptions = [{ 'name': 'None', 'value': '0' }];
          break;
        }

        instance.pntParametertypeaheadOptions.push({ 'name': 'None', 'value': '0' });
        // go through the rtuconfiguration object and add the variables needed for this point type
        for (var deviceType in this.rtuconfiguration.pointAlarmNumberLookupDictionary) {
          var pointNameString = this.rtuconfiguration.pointAlarmNumberLookupDictionary[deviceType].pointName;
          var nameString = this.rtuconfiguration.pointAlarmNumberLookupDictionary[deviceType].variableName;
          var valueString = this.rtuconfiguration.pointAlarmNumberLookupDictionary[deviceType].alarmNumber;
          if (pointNameString.toUpperCase() === pntTypeString.toUpperCase()) {
            instance.pntParametertypeaheadOptions.push({ 'name': nameString, 'value': valueString });
          }
        }
        instance.typeaheadOptions = instance.pntParametertypeaheadOptions;
        break;
    }

    setTimeout(() => {
      var event;
      if (typeof (Event) === 'function') {
        event = new Event('input');
      } else {
        event = document.createEvent('Event');
        event.initEvent('input', true, true);
      }
      e.target.dispatchEvent(event);
    }, 0);
  }

  updatePendingValue(e: NgbTypeaheadSelectItemEvent, alarm: alarm, parameterSelected: IParameter) {
    let originalPendingValue = parameterSelected.pendingValue;
    let parameterNameString = parameterSelected.parameter;
    let parameter = parameterSelected;
    let instance = this;
    parameterSelected.pendingValue = e.item.value;
    parameterSelected["temppendingValue"] = e.item;

    // set the value incase the user cancels
    if(parameterSelected.parameter === 'PntIndex' || parameterSelected.parameter === 'PntParameter')
    {
      let visibleIdentifier = Object.keys(instance.typeaheadOptions).find(s => instance.typeaheadOptions[s].value === parameterSelected.value);
      if(visibleIdentifier != undefined)
      {
        var translatedValue = instance.typeaheadOptions[visibleIdentifier].name;
        if(parameterSelected["tempValue"] === undefined)// || parameterSelected["tempValue"] === 'None')
        {
          parameterSelected["tempValue"] = translatedValue;
        }
      }
    }

    this.checkForIncrementGlobalPendingChanges(parameter, originalPendingValue);

    let alarmPoint = alarm.point as alarmPoint;
    alarmPoint.computedName = instance.getAlarmComputedName(alarmPoint);
    alarm.label = alarmPoint.computedName;
    if (alarm.label != 'Undefined') {
      alarm.labelLine1 = alarmPoint.computedName.substring(0, alarmPoint.computedName.indexOf('.'));
      alarm.labelLine2 = alarmPoint.computedName.substring((alarmPoint.computedName.indexOf('.') + 1), alarmPoint.computedName.length);
    }
    else {
      alarm.labelLine1 = 'Undefined';
      alarm.labelLine2 = '';
    }

    //update alarm type restriction
    let pointAlarmNumberLookupDictionaryKey = Object.keys(this.rtuconfiguration.pointAlarmNumberLookupDictionary).find(o => this.rtuconfiguration.pointAlarmNumberLookupDictionary[o].alarmNumber === parameter.pendingValue);
    let PntParameterName = 'undefined';
    if (pointAlarmNumberLookupDictionaryKey)
      PntParameterName = this.rtuconfiguration.pointAlarmNumberLookupDictionary[pointAlarmNumberLookupDictionaryKey].variableName;
    let typeKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'Type');

    this.updateAlarmTypeRestriction(alarm, this.getPntParameterDatatype(PntParameterName, alarm), alarmPoint.pointConfiguration[typeKey]);

    if (instance.showPointIndexes)
      alarm.labelLine1 += " (" + (alarm.alarmId + 1).toString().padStart(4, '0') +")";

    instance.getEnabledAlarms();
    instance.getAlarmSelectTab();


  }

  getEnabledAlarms() {
    if (this._rtuConfiguration
      && this.alarmParameters) {
      this._rtuConfiguration.unsubscribeRealtimeParameters(this.alarmParameters);
    }

    this.alarmParameters = [];
    let enabledAlarms = [];
    this.labelIdentifier = -1;
    this.alarmVisibleIdentifier = -1;
    let instance = this;
    let alarmId = 0;


    if (this.rtuconfiguration != null
      && this.rtuconfiguration.points != null) {
      let numberOfalarms = 20;
      let moduleConfiguration = this.rtuconfiguration.module0.moduleConfiguration;
      let numberOfalarmsIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfAlarms');
      if (numberOfalarmsIdentifier) {
        numberOfalarms = parseInt(moduleConfiguration[numberOfalarmsIdentifier].pendingValue);
        instance.alarmParameters.push(moduleConfiguration[numberOfalarmsIdentifier]);
      }

      this.rtuconfiguration.points.forEach(function (point, index) {

        if (alarmId >= numberOfalarms) {
          return;
        }

        if (point.name === ' Alarms ') {

          let alarmPoint = point as alarmPoint;

          let typeKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'Type');
          let typeParameter = alarmPoint.pointConfiguration[typeKey];
          instance.updateMaskForType(typeParameter, alarmPoint);

          alarmPoint.computedName = instance.getAlarmComputedName(alarmPoint);
          let newalarm = new alarm(index, alarmId++, alarmPoint.computedName, alarmPoint);

          newalarm.label = alarmPoint.computedName;
          if (instance.showPointIndexes)
            newalarm.labelLine1 += " (" + (newalarm.alarmId + 1).toString().padStart(4, '0') + ")";
          instance.updateTypeRestriction(newalarm, typeParameter);

          //update alarm type restriction
          let pntParameterKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'PntParameter');
          let pointAlarmNumberLookupDictionaryKey = Object.keys(instance.rtuconfiguration.pointAlarmNumberLookupDictionary).find(o => instance.rtuconfiguration.pointAlarmNumberLookupDictionary[o].alarmNumber === alarmPoint.pointConfiguration[pntParameterKey].pendingValue);
          let PntParameterName = 'undefined';
          if (pointAlarmNumberLookupDictionaryKey)
            PntParameterName = instance.rtuconfiguration.pointAlarmNumberLookupDictionary[pointAlarmNumberLookupDictionaryKey].variableName;
          instance.updateAlarmTypeRestriction(newalarm, instance.getPntParameterDatatype(PntParameterName, newalarm), alarmPoint.pointConfiguration[typeKey]);


          instance.updateAlarmImage(newalarm);
          enabledAlarms.push(newalarm);
        }
      });
    }

    instance.enabledAlarms = enabledAlarms.sort(instance.comparealarms);

    if (instance.selectedalarm != null) {
      Object.keys(instance.selectedalarm.point.pointConfiguration).map(s => instance.selectedalarm.point.pointConfiguration[s]).forEach(function (parameter) {
        instance.alarmParameters.push(parameter);
      });
    }

    instance.enabledAlarms.forEach(function (alarm) {
      //retrieving the necessary parameters to be able to update all enabled alarms' names and icons

      let outputKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'Output');
      let PntTypeKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntType');
      let PntIndexKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntIndex');
      let PntParameterKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntParameter');
      let TypeKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'Type');
      let ThresholdKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'Threshold');
      let MaskKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'Mask');
      let CharArrayKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'CharArray');
      let AlarmStateKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'AlarmState');
      let outModKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'OutModules');
      let outChanKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'OutChannels');

      let alarmCmdKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'AlarmCmd');

      var alarmDef = alarm.point.pointConfiguration[alarmCmdKey];

      var alarmState = alarm.point.pointConfiguration[AlarmStateKey];

      if(alarmState.pendingValue === '1') // disabled
        alarmDef.parameterIsVisible = 0;
      else
        alarmDef.parameterIsVisible = 1;


      instance.alarmParameters.push(alarm.point.pointConfiguration[outputKey]);
      instance.alarmParameters.push(alarm.point.pointConfiguration[PntTypeKey]);
      instance.alarmParameters.push(alarm.point.pointConfiguration[PntIndexKey]);
      instance.alarmParameters.push(alarm.point.pointConfiguration[PntParameterKey]);
      instance.alarmParameters.push(alarm.point.pointConfiguration[TypeKey]);
      instance.alarmParameters.push(alarm.point.pointConfiguration[ThresholdKey]);
      instance.alarmParameters.push(alarm.point.pointConfiguration[MaskKey]);
      instance.alarmParameters.push(alarm.point.pointConfiguration[CharArrayKey]);
      instance.alarmParameters.push(alarm.point.pointConfiguration[AlarmStateKey]);
      instance.alarmParameters.push(alarm.point.pointConfiguration[outModKey]);
      instance.alarmParameters.push(alarm.point.pointConfiguration[outChanKey]);

    });

    if (instance._rtuConfiguration
      && instance.alarmParameters) {
      instance._rtuConfiguration.subscribeRealtimeParameters(this.alarmParameters);
    }
  }

  getAlarmSelectTab() {
    let filteredAlarms = [];
    let instance = this;
    let upperCaseSearchString = this.searchalarmString.toUpperCase();
    this.enabledAlarms.forEach(function (alarm) {
      if (!instance.searchalarmString
        || instance.searchalarmString === ''
        || (instance.searchalarmString !== ''
          && (alarm.label.toUpperCase().indexOf(upperCaseSearchString) != -1) || (alarm.labelLine1.toUpperCase().indexOf(upperCaseSearchString) != -1))) {
        filteredAlarms.push(alarm);
      }
      else if (alarm === instance.selectedalarm) {
        instance.selectedalarm = null;
      }
    });

    instance.alarmPanelAlarms = filteredAlarms.sort(this.comparealarms);

    if (instance.alarmPanelAlarms.length > 0) {
      instance.alarmPanelAlarmSets = instance.alarmPanelAlarms.reduce((resultArray, alarm, index) => {
        const chunkIndex = Math.floor((index) / 4)

        if (!resultArray[chunkIndex]) {
          resultArray[chunkIndex] = [] // start a new chunk
        }

        resultArray[chunkIndex].push(alarm)

        return resultArray;
      }, []);
    }
    else 
    instance.alarmPanelAlarmSets = [];
  }

  updateSelectedAlarmLabels(){
    let instance = this;
    instance.selectedalarms.forEach(function (alarm) {
      let alarmPoint = alarm.point as alarmPoint;
      alarmPoint.computedName = instance.getAlarmComputedName(alarmPoint);

      instance.updateOutputControl(alarm);
      alarm.label = alarmPoint.computedName;

      if (alarm.label != "Undefined") {
        alarm.labelLine1 = alarmPoint.computedName.substring(0, alarmPoint.computedName.indexOf('.'));
        alarm.labelLine2 = alarmPoint.computedName.substring((alarmPoint.computedName.indexOf('.') + 1), alarmPoint.computedName.length);
      }
      else {
        alarm.labelLine1 = 'Undefined';
        alarm.labelLine2 = '';
      }
      if (instance.showPointIndexes)
        alarm.labelLine1 += " (" + (alarm.alarmId + 1).toString().padStart(4, '0')+")";
    });
  }

  getRTUConfiguration(): any {
    let instance = this;

    this.rtuconfigurationSubscription = this._rtuConfiguration.get().subscribe(data => {
      if (data.RTUConfiguration) {
        instance.rtuconfiguration = data.RTUConfiguration;
      } else {
        instance.rtuconfiguration = null;
      }

      this.updateSelectedAlarmLabels();

      instance.selectedalarms.forEach(function (alarm) {
        let alarmPoint = alarm.point as alarmPoint;

        //update alarm type restriction
        let pntParameterKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'PntParameter');
        let pointAlarmNumberLookupDictionaryKey = Object.keys(instance.rtuconfiguration.pointAlarmNumberLookupDictionary).find(o => instance.rtuconfiguration.pointAlarmNumberLookupDictionary[o].alarmNumber === alarmPoint.pointConfiguration[pntParameterKey].pendingValue);
        let PntParameterName = 'undefined';
        if (pointAlarmNumberLookupDictionaryKey)
          PntParameterName = instance.rtuconfiguration.pointAlarmNumberLookupDictionary[pointAlarmNumberLookupDictionaryKey].variableName;
        let typeKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'Type');
        instance.updateAlarmTypeRestriction(alarm, instance.getPntParameterDatatype(PntParameterName, alarm), alarmPoint.pointConfiguration[typeKey]);

        //update the PntParameter text if necessary
        let pntParameter = alarmPoint.pointConfiguration[pntParameterKey];
        if (pntParameter.value == pntParameter.pendingValue) //only update it if the control matches the config (don't want to change an edit the user has made)
        {
          pntParameter.temppendingValue = {name: pntParameter.translatedPendingValue, value: pntParameter.value};
        }

        let pntIndexParameterKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'PntIndex');
        let pntIndexParameter = alarmPoint.pointConfiguration[pntIndexParameterKey];
        if (pntIndexParameter.value == pntIndexParameter.pendingValue) //only update it if the control matches the config (don't want to change an edit the user has made)
        {

          let PntTypeKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntType');
          if (alarm.point.pointConfiguration[PntTypeKey].pendingValue == '3')
          pntIndexParameter.temppendingValue = {name: instance.getPntIndexNameForValue(alarmPoint, pntIndexParameterKey), value: alarmPoint.pointConfiguration[pntIndexParameterKey].pendingValue};
        
          alarmPoint.computedName = instance.getAlarmComputedName(alarmPoint);
          alarm.label = alarmPoint.computedName;
          instance.updateSelectedAlarmLabels();
        }
      });



      instance.outputModChanOptions = [];
      let moduleStrings = ["module1", "module2", "module3", "module4", "module5", "module6"];
      let channelStrings = ["channel1", "channel2", "channel3", "channel4", "channel5", "channel6", "channel7", "channel8"];
      let moduleIndex = 1;
      let channelIndex = 1;
      if (instance.rtuconfiguration) {
        moduleStrings.forEach(function (moduleString) {
          const modConfiguredKey = Object.keys(instance.rtuconfiguration[moduleString].moduleConfiguration).find(s => instance.rtuconfiguration[moduleString].moduleConfiguration[s].parameter === 'ModConfigured');
          if (instance.rtuconfiguration[moduleString].moduleConfiguration[modConfiguredKey].pendingValue == "3") //digital I/O
          {
            channelStrings.forEach(function (channelString) {
              const protocolKey = Object.keys(instance.rtuconfiguration[moduleString][channelString].channelConfiguration).find(s => instance.rtuconfiguration[moduleString][channelString].channelConfiguration[s].parameter === 'Protocol');
              if (instance.rtuconfiguration[moduleString][channelString].channelConfiguration[protocolKey].pendingValue == "7") //digital output
              {
                instance.outputModChanOptions.push({ "text": "Mod X" + moduleIndex.toString() + " Chan X" + channelIndex.toString(), "value": moduleIndex.toString() + channelIndex.toString() });
              }
              channelIndex++;
            });
            channelIndex = 1;

          }
          moduleIndex++;
        });
      }

      instance.getEnabledAlarms();
      instance.getAlarmSelectTab();
      instance.getAlarmViewTab();

    });
  }

  onParameterChange(parameter: IParameter, alarm: alarm) {
    let instance = this;
    if (this.selectedalarms.length > 1) {
      let alarms = this.selectedalarms;
    }

    if (parameter.parameter === 'PntType'
    || parameter.parameter === 'PntIndex'
    || parameter.parameter === 'PntParameter'
    || parameter.parameter === 'Type'
    || parameter.parameter === 'Threshold'
    || parameter.parameter === "AlarmState"
    || parameter.parameter === 'Mask'
    || parameter.parameter === "CharArray") {

      instance.selectedalarm = instance.enabledAlarms.find(s => s.pointId === alarm.pointId);

      let alarmPoint = alarm.point as alarmPoint;
      if (parameter.parameter === 'Type') {
        this.updateMaskForType(parameter, alarmPoint);
      }

      // if the pnttype has changed populate the type ahead options for the pntparameter
      if (parameter.parameter === 'PntType') {
        let PntIndexKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'PntIndex');
        let PntParameterKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'PntParameter');

        // if the user changed the point type we need to store the original value just incase the user presses cancel

        var alarmpointIndex = alarmPoint.pointConfiguration[PntIndexKey];

        if(alarmpointIndex["tempValue"] === undefined &&
        alarmpointIndex.translatedPendingValue != undefined && alarmpointIndex.translatedPendingValue != '')
        {
          alarmpointIndex["tempValue"] = alarmpointIndex.translatedPendingValue;
        }
  
        var alarmpointParameter = alarmPoint.pointConfiguration[PntParameterKey];

        if(alarmpointParameter["tempValue"] === undefined &&
        alarmpointParameter.translatedPendingValue != undefined && alarmpointParameter.translatedPendingValue != '')
        {
          alarmpointParameter["tempValue"] = alarmpointParameter.translatedPendingValue;
        }

        alarmPoint.pointConfiguration[PntIndexKey].temppendingValue = 'None'; // what the typeahead is bound to
        alarmPoint.pointConfiguration[PntParameterKey].temppendingValue = 'None';
        alarmPoint.pointConfiguration[PntIndexKey].translatedPendingValue = 'None'; // what the placeholder is bound to
        alarmPoint.pointConfiguration[PntParameterKey].translatedPendingValue = 'None';
        let pntIndexKeyOriginalPendingValue = alarmPoint.pointConfiguration[PntIndexKey].pendingValue
        let pntParamKeyOriginalPendingValue = alarmPoint.pointConfiguration[PntParameterKey].pendingValue

        alarmPoint.pointConfiguration[PntIndexKey].pendingValue = '0';
        alarmPoint.pointConfiguration[PntParameterKey].pendingValue = '0';
        this.checkForIncrementGlobalPendingChanges(alarmPoint.pointConfiguration[PntIndexKey], pntIndexKeyOriginalPendingValue);
        this.checkForIncrementGlobalPendingChanges(alarmPoint.pointConfiguration[PntParameterKey], pntParamKeyOriginalPendingValue);
      }
      alarmPoint.computedName = instance.getAlarmComputedName(alarmPoint);
      alarm.label = alarmPoint.computedName;


      if (instance.showPointIndexes)
        alarm.labelLine1 += " (" + (alarm.alarmId + 1).toString().padStart(4, '0')+ ")";

      if (alarm.label != "Undefined") {
        alarm.labelLine1 = alarmPoint.computedName.substring(0, alarmPoint.computedName.indexOf('.'));
        alarm.labelLine2 = alarmPoint.computedName.substring((alarmPoint.computedName.indexOf('.') + 1), alarmPoint.computedName.length);
      }
      else {
        alarm.labelLine1 = 'Undefined';
        alarm.labelLine2 = '';
      }
      instance.getEnabledAlarms();
      instance.getAlarmSelectTab();
      if (parameter.parameter === "Type") {
        instance.updateTypeRestriction(alarm, parameter);
      }

    }
    else if (parameter.configClass === configClass.COMMAND) {
      this._rtuConfiguration.applyCommandToRTU(parameter);
    }
  }

  setnexttabbeditem() {
    const input1 = document.getElementById('Label');
    input1.focus();
  }



  isParameterCell(cell: ICell) {
    let cellType = typeof cell.object;
    return (cellType === 'string') ? false : true;
  }



  getAlarmViewTab() {
    let instance = this;
    if (!instance.alarmViewTabs) {
      instance.alarmViewTabs = [{ tabName: "Config" }, { tabName: "Command" }];
    }


    if (!instance.selectedalarm
      && instance.enabledAlarms.length === 0) {
      return;
    }
  }

  populateTab(instance: any) {
    let alarm = instance.selectedalarm;
    if (!alarm) {
      if (instance.enabledAlarms.length !== 0) {
        alarm = instance.enabledAlarms[0];
      }
    }
  }

  onalarmImgClick(event: any, pointId: number, alarmId: number) {
    let instance = this;
    var clickedAlarm = instance.enabledAlarms.find(s => s.pointId === pointId);

    if (instance.selectedalarms.indexOf(instance.selectedalarms.find(s => s.pointId === clickedAlarm.pointId)) === -1) {
      /*
      if (event.ctrlKey) { //ctrl clicked an unselected alarm. Add it to selectedalarms
        instance.selectedalarm = instance.enabledAlarms.find(s => s.pointId === pointId);
        instance.selectedalarms.push(instance.selectedalarm);


      }
      else if (event.shiftKey) {
        if (instance.selectedalarm) //a alarm is selected. select everything in between.
        {
          //var startIndex = instance.enabledAlarms.indexOf(instance.selectedalarm);
          var startIndex = instance.enabledAlarms.findIndex(alarms => alarms.pointId === instance.selectedalarm.pointId);
          var endIndex = instance.enabledAlarms.indexOf(instance.enabledAlarms.find(s => s.pointId === pointId));
          if (startIndex > endIndex)
            startIndex = endIndex + (endIndex = startIndex, 0)
          for (let index = startIndex; index <= endIndex; index++) {

            if (instance.selectedalarms.indexOf(instance.selectedalarms.find(s => s.pointId === instance.enabledAlarms[index].pointId)) === -1)
              instance.selectedalarms.push(instance.enabledAlarms[index]);
          }
          instance.selectedalarm = instance.enabledAlarms.find(s => s.pointId === pointId);
        }
        else //no alarm was selected. select everything from the beginning to this one. 
        {
          instance.selectedalarm = instance.enabledAlarms.find(s => s.pointId === pointId);
          var endIndex = instance.enabledAlarms.indexOf(instance.selectedalarm);
          for (let index = 0; index <= endIndex; index++) {
            instance.selectedalarms.push(instance.enabledAlarms[index]);
          }
        }

      }
      else   */
      { //regular clicked an unselected alarm. Clear selectedalarms and revert images, and then select clicked alarm

        instance.selectedalarms = [];
        instance.enabledAlarms.forEach(function (alarm) {
        });

        instance.selectedalarm = instance.enabledAlarms.find(s => s.pointId === pointId);
        instance.selectedalarms.push(instance.selectedalarm);


      }
    }
    else {
      /*
      if (event.ctrlKey) { //ctrl clicked a selected alarm. Deselect it. 
        var deselectedalarm = instance.enabledAlarms.find(s => s.pointId === pointId);


        let filteredalarms = instance.selectedalarms.filter(function (obj) {
          return obj.pointId !== pointId;
        });
        instance.selectedalarms = filteredalarms;
        if (instance.selectedalarms.length <= 1) //if we are reverting to single edit mode, set the last alarm in the array to the selectedalarm
          instance.selectedalarm = instance.selectedalarms[0];
      }
      else //regular clicked a selected alarm
      */
      {

        instance.selectedalarms.forEach(function (alarm) {
        });
        if (instance.selectedalarms.length > 1) { //if we are in batch edit mode and regular click a alarm, single select that alarm
          instance.enabledAlarms.forEach(function (alarm) {
          });
          instance.selectedalarm = instance.enabledAlarms.find(s => s.pointId === pointId);
          instance.selectedalarms = [];
          instance.selectedalarms.push(instance.selectedalarm);
        }
        else //otherwise we clicked on the only one selected. deselect it. 
        {
          instance.selectedalarms = [];
          //event.target.sEnabled;
          instance.selectedalarm = null;
        }
      }
    }
    instance.getEnabledAlarms();
    instance.getAlarmSelectTab();
    instance.getAlarmViewTab();
  }

  dragSelect(event: any) {
    /*
    if (event.length > 0) {
      let instance = this;
      instance.dragSelectActive = true;
      instance.selectedalarms = [];
      instance.enabledAlarms.forEach(function (alarm) {
        alarm.alarmlistimg;
      });
      instance.selectedalarm = event[0];
      event.forEach(function (alarm) {
        instance.selectedalarms.push(alarm);
        alarm.alarmlistimg = alarmSelectedImage;
      });
      instance.getEnabledAlarms();
      instance.getAlarmSelectTab();
      instance.getAlarmViewTab();
      setTimeout(function () { instance.dragSelectActive = false; }, 100);
    }
    */
  }

  onAlarmPanelClick(event: any) {
    let instance = this;
    if (instance.dragSelectActive == false) {
      instance.enabledAlarms.forEach(function (alarm) {
      });
      instance.selectedalarms = [];
      if (instance.selectedalarm) {
        instance.selectedalarm = null;
      }

      instance.getEnabledAlarms();
      instance.getAlarmSelectTab();
      instance.getAlarmViewTab();
    }
    else { instance.dragSelectActive = false; }
  }


  toggleAlarmSearch() {
    this.searchalarmToggle = !this.searchalarmToggle;
    this.searchalarmString = '';
    this.getAlarmSelectTab();
    this.getAlarmViewTab();
    if (this.searchalarmToggle) {
      const input = document.getElementById('alarmsfilterinput');
      setTimeout(() => { input.focus(); }, 100);
    }
  }

  searchalarmChanged(newSearchValue) {
    this.searchalarmString = newSearchValue;
    console.log('search alarm Value: ' + this.searchalarmString);
    this.getAlarmSelectTab();
    this.getAlarmViewTab();
  }

  toggleConfigurationSearch() {
    this.searchConfigurationToggle = !this.searchConfigurationToggle;
    this.searchConfigurationString = '';
    this.getAlarmViewTab();
    if (this.searchConfigurationToggle) {
      const input = document.getElementById('configurationfilterinput');
      setTimeout(() => { input.focus(); }, 100);
    }
  }

  searchConfigurationChanged(newSearchValue) {
    this.searchConfigurationString = newSearchValue;
    console.log('search Configuration Value: ' + this.searchConfigurationString);
    this.getAlarmViewTab();
  }

  saveToRTUConfiguration() {
    // if we are activating alarms we need to prompt for the new label
    if (this.alarmActivationMode === 'ACTIVATE alarmS') {
      this.setActivateMultiLabel = '';
      this.activateLabelModalBackground.nativeElement.classList.remove('d-none');
      this.batchRenameLabel.nativeElement.focus();
    } else {
      this._rtuConfiguration.setPointData(this.alarmsToChangeVisibility);
      this.alarmsToChangeVisibility = [];
    }
  }

  cancelBatchActivationRename() {
    this.activateLabelModalBackground.nativeElement.classList.add('d-none');
  }


  VerifyApplytoRTUPrompt(): void {
    this.modalRef = this._modalService.show(this.modalVerifyApplytoRTUAction, this.modalConfig);
  }

  applyToRTU() {
    var instance = this;

    this._rtuConfiguration.applyDataToRTU(false, this._rtuConfiguration.checkForRTUConfigChanges());
    if (this.modalRef !== undefined)
      this.modalRef.hide();

    // since we are doing our own change detect for the type ahead drop down list we need to reset the values here
    instance.enabledAlarms.forEach(function (alarm) {
      //retrieving the necessary parameters to be able to update all enabled alarms' names and icons

      let MaskKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'Mask');

      let maskParameter = alarm.point.pointConfiguration[MaskKey];

      if(maskParameter["OriginalDisplayUnits"] != undefined &&
      maskParameter["OriginalDisplayUnits"] != "None")
      {
        // change the values to match the defined units
        if(maskParameter["OriginalDisplayUnits"] != maskParameter.displayFormat)
        {
          if (maskParameter.displayFormat == '') {
            maskParameter.pendingValue = parseInt(maskParameter.pendingValue, 10).toString(16).toUpperCase();
            maskParameter.value = parseInt(maskParameter.value, 10).toString(16).toUpperCase();
          }
          else if (maskParameter.displayFormat == 'LHEX') {
            maskParameter.pendingValue = parseInt(maskParameter.pendingValue, 16).toString();
            maskParameter.value = parseInt(maskParameter.value, 16).toString();
          }
          maskParameter["OriginalDisplayUnits"] = maskParameter.displayFormat;
        }
      }

      let pointIndexKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntIndex');

      let pntIndexParameter = alarm.point.pointConfiguration[pointIndexKey];

      if(pntIndexParameter["tempValue"] != undefined &&
      pntIndexParameter["tempValue"] != "None")
      {
        pntIndexParameter["tempValue"] = 'None';
      }

      let pointParameterKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntParameter');

      let pntParameterParameter = alarm.point.pointConfiguration[pointParameterKey];

      if(pntParameterParameter["tempValue"] != undefined &&
      pntParameterParameter["tempValue"] != "None")
      {
        pntParameterParameter["tempValue"] = 'None';
      }


    });

  }

  VerifyCancelChangesPrompt(): void {
    if (this._rtuConfiguration.checkForRTUConfigChanges())
      this.modalRef = this._modalService.show(this.modalVerifyCancelAction, this.modalConfig);
  }

  public areThereNoChangesMade() {
    if (this._rtuConfiguration.checkForRTUConfigChanges())
      return false;
    else
      return true;
  }

  cancelChanges() {
    var instance = this;
    // before we call cancel pending changes we need to remap the units to what they were when first loaded updatemask
    instance.enabledAlarms.forEach(function (alarm) {
      //retrieving the necessary parameters to be able to update all enabled alarms' names and icons

      let MaskKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'Mask');

      let maskParameter = alarm.point.pointConfiguration[MaskKey];

      if(maskParameter["OriginalDisplayUnits"] != undefined &&
      maskParameter["OriginalDisplayUnits"] != maskParameter.displayFormat)
      {
        if(maskParameter.displayFormat != maskParameter["OriginalDisplayUnits"])
        {
          // change the values to match the defined units
          if (maskParameter.displayFormat == '') {
            maskParameter.pendingValue = parseInt(maskParameter.pendingValue, 10).toString(16).toUpperCase();
            maskParameter.value = parseInt(maskParameter.value, 10).toString(16).toUpperCase();
          }
          else if (maskParameter.displayFormat == 'LHEX') {
            maskParameter.pendingValue = parseInt(maskParameter.pendingValue, 16).toString();
            maskParameter.value = parseInt(maskParameter.value, 16).toString();
          }
          maskParameter.displayFormat = maskParameter["OriginalDisplayUnits"];
        }
      }
    });

    this._rtuConfiguration.cancelPendingChanges();

    this.revertTypeaheads();

    // this is kind of screwy but the above will result if the type combo box being reset so we need to call cancelpendingchanges again
    setTimeout(() => { this._rtuConfiguration.cancelPendingChanges() }, 1);

    this.modalRef.hide();
  }

  public saveRtuConfigToDisk() {
    let configToSave;
    if (this.rtuconfiguration) {
      configToSave = JSON.stringify(this.rtuconfiguration);
    }
    else {
      configToSave = 'no data';
    }

    const data = new Blob([configToSave], { type: 'application/json' });

    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
      window.navigator.msSaveOrOpenBlob(data, 'config.rtuconfig');
    }
    else {
      saveAs(data, 'config.rtuconfig');
    }
  }

  public getcancelButtontext() {
    if (this.alarmsToChangeVisibility.length > 0)
      return "Cancel";
    else
      return "Close";
  }

  public areNoalarmsSelected() {
    if (this.alarmsToChangeVisibility.length > 0)
      return false;
    else
      return true;
  }

  public isRTUConnectedandChangesExist() {
    if (this._rtuConfiguration.connectionStatus !== RTUConnectionStatus.CONNECTED) {
      return true;
    }
    else if (this._rtuConfiguration.checkForRTUConfigChanges() === false) {
      return true;
    }
    else {
      return false;
    }
  }

  public setTextFieldStyle() {
    let styles = {};

    if (this.autoIncrementParameterName == 'DeviceID') {
      styles = { 'visibility': 'hidden' };
    }
    else {
      styles = { 'visibility': 'visible' };
    }

    return styles;
  }

  public checkForIncrementGlobalPendingChanges(parameter: IParameter, originalPendingValue: string) 
  {
    let instance = this;
    if (instance._rtuConfiguration.isParameterValueChanged(parameter)) {
      if (originalPendingValue == parameter.value)
        instance._rtuConfiguration.incrementGlobalPendingChanges();
    }
    else {
      if (originalPendingValue != parameter.value)
        instance._rtuConfiguration.decrementGlobalPendingChanges();
    }
  }

  comparePointId(point1: any, point2: any) {
    if (point1.pointId < point2.pointId) {
      return -1;
    }
    if (point1.pointId > point2.pointId) {
      return 1;
    }
    return 0;
  }

  public setIncrementTitlestyle() {
    let styles = {};

    if (this.autoIncrementParameterName == 'DeviceID') {
      styles = { 'visibility': 'hidden' };
    }
    else {
      styles = { 'visibility': 'visible' };
    }

    return styles;
  }
  public setIncrementTitlestyle1() {
    let styles = {};

    if (this.autoIncrementParameterName != 'DeviceID') {
      styles = { 'visibility': 'hidden' };
    }
    else {
      styles = { 'visibility': 'visible', 'position': 'absolute', 'left': '20px' };
    }

    return styles;
  }

  togglePointIdx() {
    let instance = this;
    this.showPointIndexes = !this.showPointIndexes;
    localStorage.setItem('showPointIndexes',this.showPointIndexes.toString());
    this.getEnabledAlarms();
    this.getAlarmSelectTab();
    this.getAlarmViewTab();
    this.updateSelectedAlarmLabels();
  }

  getAlarmComputedName(alarmPoint: alarmPoint) {
    let instance = this;
    let PntTypeKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'PntType');
    let PntIndexKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'PntIndex');
    let PntParameterKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'PntParameter');
    let TypeKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'Type');
    let ThresholdKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'Threshold');
    let MaskKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'Mask');
    let CharArrayKey = Object.keys(alarmPoint.pointConfiguration).find(s => alarmPoint.pointConfiguration[s].parameter === 'CharArray');

    if (!(PntTypeKey && PntIndexKey && PntParameterKey && TypeKey && ThresholdKey && MaskKey && CharArrayKey))
      return;

    let pointLabel = 'undefined';
    let pointType = parseInt(alarmPoint.pointConfiguration[PntTypeKey].pendingValue);

    let pointIndexLable = 'undefined';

    switch (pointType) {
      case 1: // CPU Pnt
        if (alarmPoint.pointConfiguration[PntIndexKey].pendingValue == "1") {
          let PntLabelKey = Object.keys(instance.rtuconfiguration.module0.moduleConfiguration).find(s => instance.rtuconfiguration.module0.moduleConfiguration[s].parameter === 'Label');
          pointLabel = instance.rtuconfiguration.module0.moduleConfiguration[PntLabelKey].pendingValue;
        }
        else
          pointLabel = "None"; //pointLabel = 'CPU';

        pointIndexLable = pointLabel;
        break;
      case 2: // Interface Pnt

        if ( alarmPoint.pointConfiguration[PntIndexKey].pendingValue >= 1 && alarmPoint.pointConfiguration[PntIndexKey].pendingValue <= 6 ) {
            let moduleString = "module" + alarmPoint.pointConfiguration[PntIndexKey].pendingValue;
            const InterfacePntLabelKey = Object.keys(instance.rtuconfiguration[moduleString].moduleConfiguration).find(s => instance.rtuconfiguration[moduleString].moduleConfiguration[s].parameter === 'Label');
            pointLabel = instance.rtuconfiguration[moduleString].moduleConfiguration[InterfacePntLabelKey].pendingValue;
        }
        else {
        pointLabel = "undefined";
        //pointLabel = 'Module.' + alarmPoint.pointConfiguration[PntIndexKey].pendingValue; 
        }
        pointIndexLable = pointLabel;
        break;
      case 3: // Port Pnt
        if (alarmPoint.pointConfiguration[PntIndexKey].pendingValue == '0') {
          pointLabel = "undefined";
        } else {
        const moduleNumber = Math.floor((alarmPoint.pointConfiguration[PntIndexKey].pendingValue - 1) / 8);
        //pointIndexLable = (moduleNumber > 0 ? 'INTFM ' : 'CPU ') + moduleNumber + ' Channel ' + (((alarmPoint.pointConfiguration[PntIndexKey].pendingValue - 1) % 8) + 1);

          
        alarmPoint.pointConfiguration[PntIndexKey].temppendingValue = {name: instance.getPntIndexNameForValue(alarmPoint, PntIndexKey), value: alarmPoint.pointConfiguration[PntIndexKey].pendingValue};
        

          pointLabel = alarmPoint.pointConfiguration[PntIndexKey].temppendingValue.name;
      }
       pointIndexLable = pointLabel;
        break;
      case 4: // FP Reg Pnt
        if (alarmPoint.pointConfiguration[PntIndexKey].pendingValue == '0') {
          pointLabel = "undefined";
          pointIndexLable = "undefined";
        } else {
          pointLabel = 'FP Reg Pnt ' + alarmPoint.pointConfiguration[PntIndexKey].pendingValue;
          pointIndexLable = 'FP Register ' + alarmPoint.pointConfiguration[PntIndexKey].pendingValue;
        }
        break;
      case 5: // INT Reg Pnt
        if (alarmPoint.pointConfiguration[PntIndexKey].pendingValue == '0') {
          pointLabel = "undefined";
          pointIndexLable = "undefined";
        } else {
          pointLabel = 'INT Reg Pnt ' + alarmPoint.pointConfiguration[PntIndexKey].pendingValue;
          pointIndexLable = 'Integer Register ' + alarmPoint.pointConfiguration[PntIndexKey].pendingValue;
        }
        break;
      case 6: // GW Block Pnt
        if (alarmPoint.pointConfiguration[PntIndexKey].pendingValue == '0') {
          pointLabel = "undefined";
        } else {
          pointLabel = 'GW Block Pnt ' + alarmPoint.pointConfiguration[PntIndexKey].pendingValue;
        }
        pointIndexLable = pointLabel;
        break;
      case 7: // Tank Pnt
        const tanks = this.rtuconfiguration.points.filter(x => x.name === 'Tank');
        const pointIndex = alarmPoint.pointConfiguration[PntIndexKey].pendingValue;

        const tank = tanks[(parseInt(pointIndex, 10) - 1)];

        if (tank) {
          const PntLabelKey = Object.keys(tank.pointConfiguration).find(s => tank.pointConfiguration[s].parameter === 'Label');
          pointLabel = tank.pointConfiguration[PntLabelKey].pendingValue;
        }
        else {
          pointLabel = 'undefined';
        }
        pointIndexLable = pointLabel;
        break;
      case 8: // Alarm Pnt

        // Using the label here causes a recursive problem. 
         //let pointId = parseInt(alarmPoint.pointConfiguration[PntIndexKey].pendingValue);
         //let foundAlarm = this.enabledAlarms.find(s => s.pointId === pointId+399);
         //instance.typeaheadOptions.push({ 'name': foundAlarm.label + " (" + i + ")", 'value': (parseInt(i)).toString() });
         //if (foundAlarm)
         //pointLabel = foundAlarm.label + " (" + alarmPoint.pointConfiguration[PntIndexKey].pendingValue+  ")";
        // else
        if (alarmPoint.pointConfiguration[PntIndexKey].pendingValue == '0') {
          pointLabel = "undefined";
        } else {
          var zerofilled = 'Alarm Pnt ' + ('0000' + alarmPoint.pointConfiguration[PntIndexKey].pendingValue).slice(-4);

          pointLabel = 'Alarm Pnt.' + alarmPoint.pointConfiguration[PntIndexKey].pendingValue;
          pointIndexLable = zerofilled;
        }
        break;
    }
    if (pointIndexLable != "undefined" && pointIndexLable) {
      alarmPoint.pointConfiguration[PntIndexKey].translatedPendingValue = pointIndexLable;
      //alarmPoint.pointConfiguration[PntIndexKey].temppendingValue = pointIndexLable;
    }
    else 
    {
      if(alarmPoint.pointConfiguration[PntIndexKey].pendingValue === '0')
      {
        alarmPoint.pointConfiguration[PntIndexKey].translatedPendingValue = 'None';
      }
      else
      {
        alarmPoint.pointConfiguration[PntIndexKey].translatedPendingValue = alarmPoint.pointConfiguration[PntIndexKey].pendingValue;
      }
    }
    let typeValue = 'undefined';
    // Bitmap,Match,Mismatch,Low Threshold,High Threshold,Char Array
    switch (parseInt(alarmPoint.pointConfiguration[TypeKey].pendingValue, 10)) {
      case 1: // Bitmap
      case 2: // Match
      case 3: // Mismatch
        typeValue = alarmPoint.pointConfiguration[MaskKey].pendingValue;
        break;
      case 4: // Low Threshold
      case 5: // High Threshold
        typeValue = alarmPoint.pointConfiguration[ThresholdKey].pendingValue;
        break;
      case 6: // Char Array
        typeValue = alarmPoint.pointConfiguration[CharArrayKey].pendingValue;
        break;
    }



    let typeString = alarmTypes[parseInt(alarmPoint.pointConfiguration[TypeKey].pendingValue, 10)]
    if (typeString) {
      typeString = typeString.replace(' Threshold', '');
      typeString = typeString.replace(' ', '');
    }

    let pointAlarmNumberLookupDictionaryKey = Object.keys(this.rtuconfiguration.pointAlarmNumberLookupDictionary).find(o => this.rtuconfiguration.pointAlarmNumberLookupDictionary[o].alarmNumber === alarmPoint.pointConfiguration[PntParameterKey].pendingValue);
    let PntParameterName = 'undefined';
    if (pointAlarmNumberLookupDictionaryKey)
      PntParameterName = this.rtuconfiguration.pointAlarmNumberLookupDictionary[pointAlarmNumberLookupDictionaryKey].variableName;

    let name = pointLabel + "."
      + PntParameterName + "."
      + typeString + "."
      + typeValue;
    this.rtuconfiguration.pointAlarmNumberLookupDictionary
    // let PntParameterName =       this.rtuconfiguration.pointAlarnNumberLookupDictionary[parseInt(alarmPoint.pointConfiguration[PntParameterKey].pendingValue)].variableName;
    if (PntParameterName != "undefined" && PntParameterName) {
      alarmPoint.pointConfiguration[PntParameterKey].translatedPendingValue = PntParameterName;
    }
    else {
      if(alarmPoint.pointConfiguration[PntParameterKey].pendingValue === '0')
      {
        alarmPoint.pointConfiguration[PntParameterKey].translatedPendingValue = 'None';
      }
      else
      {
        alarmPoint.pointConfiguration[PntParameterKey].translatedPendingValue = alarmPoint.pointConfiguration[PntParameterKey].pendingValue;
      }
    }

    if (name.indexOf('undefined') != -1)
      name = "Undefined";
    return name;
  }

  updateAlarmImage(alarm: alarm, path = null) {
    if (path)
      alarm.alarmlistimg = path;
    else {
      let instance = this;

      let PntTypeKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntType');

      let pntTypeString = alarm.point.pointConfiguration[PntTypeKey].availableCommands.split(',')[parseInt(alarm.point.pointConfiguration[PntTypeKey].pendingValue) - 1];

      let alarmStateKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'AlarmState');

      let alarmStateValue = alarm.point.pointConfiguration[alarmStateKey].pendingValue;

      let alarmOutputKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'Output');

      let alarmOutputValue = alarm.point.pointConfiguration[alarmOutputKey].value;

      if (alarm.label == "Undefined") {
        if (instance.selectedalarms.findIndex(alarms => alarms.pointId === alarm.pointId) != -1)
          alarm.alarmlistimg = alarmSelectedUndefinedImage;
        else if (alarmStateValue == 1)
          alarm.alarmlistimg = alarmDisabledUndefinedImage;
        else
          alarm.alarmlistimg = alarmEnabledUndefinedImage;
      }

      else if (pntTypeString == "Tank Pnt") {
        //if (instance.selectedalarms.indexOf(alarm) != -1)
        if (alarmOutputValue == '1' && instance._rtuConfiguration.connectionStatus != RTUConnectionStatus.DISCONNECTED) {
          if (instance.selectedalarms.findIndex(alarms => alarms.pointId === alarm.pointId) != -1)
            alarm.alarmlistimg = alarmTankWarningSelectedImage;
          else
            alarm.alarmlistimg = alarmTankWarningImage;
        }
        else if (instance.selectedalarms.findIndex(alarms => alarms.pointId === alarm.pointId) != -1)

          alarm.alarmlistimg = alarmTankSelectedImage;
        else if (alarmStateValue == 1)
          alarm.alarmlistimg = alarmTankDisabledImage;
        else
          alarm.alarmlistimg = alarmTankEnabledImage;
      }
      else {
        if (alarmOutputValue == '1' && instance._rtuConfiguration.connectionStatus != RTUConnectionStatus.DISCONNECTED) {
          if (instance.selectedalarms.findIndex(alarms => alarms.pointId === alarm.pointId) != -1)
            alarm.alarmlistimg = alarmWarningSelectedImage;
          else
            alarm.alarmlistimg = alarmWarningImage;
        }
        else if (instance.selectedalarms.findIndex(alarms => alarms.pointId === alarm.pointId) != -1)
          alarm.alarmlistimg = alarmSelectedImage;
        else if (alarmStateValue == 1)
          alarm.alarmlistimg = alarmDisabledImage;
        else
          alarm.alarmlistimg = alarmEnabledImage;
      }
    }
  }

  updateTypeRestriction(alarm: alarm, typeParameter: IParameter = null) {
    let typeValue;
    let instance = this;
    if (typeParameter)
      typeValue = typeParameter.pendingValue;
    else {
      let typeKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'Type');
      let typeParameter = alarm.point.pointConfiguration[typeKey];
      typeValue = typeParameter.pendingValue;
    }

    if(!typeValue){
      return;
    }

    let thresholdKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'Threshold');
    let thresholdParameter = alarm.point.pointConfiguration[thresholdKey];
    let charArrayKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'CharArray');
    let charArrayParameter = alarm.point.pointConfiguration[charArrayKey];
    let MaskKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'Mask');
    let maskParameter = alarm.point.pointConfiguration[MaskKey];
    typeValue = typeValue.toString();
    switch (typeValue) {
      case "1": //bitmap
        thresholdParameter.disableOverride = true;
        //thresholdParameter.pendingValue = thresholdParameter.value;
        charArrayParameter.disableOverride = true;
        //charArrayParameter.pendingValue = charArrayParameter.value;
        maskParameter.disableOverride = false;
        break;
      case "2": //match
        thresholdParameter.disableOverride = true;
        //thresholdParameter.pendingValue = thresholdParameter.value;
        charArrayParameter.disableOverride = true;
        //charArrayParameter.pendingValue = charArrayParameter.value;
        maskParameter.disableOverride = false;
        break;
      case "3": //mismatch
        thresholdParameter.disableOverride = true;
        //thresholdParameter.pendingValue = thresholdParameter.value;
        charArrayParameter.disableOverride = true;
        //charArrayParameter.pendingValue = charArrayParameter.value;
        maskParameter.disableOverride = false;
        break;
      case "4": //low threshold
        thresholdParameter.disableOverride = false;
        charArrayParameter.disableOverride = true;
        //charArrayParameter.pendingValue = charArrayParameter.value;
        maskParameter.disableOverride = true;
        //maskParameter.pendingValue = maskParameter.value;
        break;
      case "5": //high threshold
        thresholdParameter.disableOverride = false;
        charArrayParameter.disableOverride = true;
        //charArrayParameter.pendingValue = charArrayParameter.value;
        maskParameter.disableOverride = true;
        //maskParameter.pendingValue = maskParameter.value;
        break;
      case "6": //char array
        thresholdParameter.disableOverride = true;
        //thresholdParameter.pendingValue = thresholdParameter.value;
        charArrayParameter.disableOverride = false;
        maskParameter.disableOverride = true;
        //maskParameter.pendingValue = maskParameter.value;
        break;
    }
  }

  revertTypeaheads() {
    let instance = this;
    instance.enabledAlarms.forEach(function (alarm) {
      let pntIndexKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntIndex');
      let PntTypeKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntType');
      let pntIndexParameter = alarm.point.pointConfiguration[pntIndexKey];
      //if (pntIndexParameter.temppendingValue) {
        //pntIndexParameter.temppendingValue = '';
      //}
        pntIndexParameter.pendingValue= pntIndexParameter.value;
        if (alarm.point.pointConfiguration[PntTypeKey].pendingValue == '3')
        {
        let alarmPoint = alarm.point as alarmPoint;
        pntIndexParameter.temppendingValue = {name: instance.getPntIndexNameForValue(alarmPoint, pntIndexKey), value:pntIndexParameter.pendingValue}; 
        }
        else if(pntIndexParameter.tempValue) {
          pntIndexParameter.temppendingValue = pntIndexParameter.tempValue;
        }

      
      let pntParameterKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntParameter');
      let pntParameterparameter = alarm.point.pointConfiguration[pntParameterKey];
      if (pntParameterparameter.tempValue){
        pntParameterparameter.temppendingValue = pntParameterparameter.tempValue;
      }
      //if (pntParameterparameter.temppendingValue) {
        //pntParameterparameter.temppendingValue = '';
      //}
    });
  }

  updateMaskForType(typeParameter: IParameter, alarmpoint: alarmPoint) {
    let maskKey = Object.keys(alarmpoint.pointConfiguration).find(s => alarmpoint.pointConfiguration[s].parameter === 'Mask');
    let maskParameter = alarmpoint.pointConfiguration[maskKey];

    if(maskParameter["OriginalDisplayUnits"] === undefined)
    {
      maskParameter["OriginalDisplayUnits"] = maskParameter.displayFormat;
    }
    if (typeParameter.pendingValue == '1') {
      if (maskParameter.displayFormat == '') {
        maskParameter.pendingValue = parseInt(maskParameter.pendingValue, 10).toString(16).toUpperCase();
        maskParameter.value = parseInt(maskParameter.value, 10).toString(16).toUpperCase();
      }

      maskParameter.displayFormat = 'LHEX';
    }
    else {
      if (maskParameter.displayFormat == 'LHEX') {
        maskParameter.pendingValue = parseInt(maskParameter.pendingValue, 16).toString();
        maskParameter.value = parseInt(maskParameter.value, 16).toString();
      }

      maskParameter.displayFormat = '';
    }
  }

  getPntParameterDatatype(pntParameterName: string, alarm: alarm) {
    let pntTypeKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntType');
    let pntTypeValue = alarm.point.pointConfiguration[pntTypeKey].pendingValue;
    let pntIndexKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntIndex');
    let pntIndexValue = alarm.point.pointConfiguration[pntIndexKey].pendingValue;

    let instance = this;
    switch (pntTypeValue) {
      case "1": // CPU Pnt
        if (pntIndexValue == "1") {
          let selectedParameterKey = Object.keys(instance.rtuconfiguration.module0.moduleConfiguration).find(s => instance.rtuconfiguration.module0.moduleConfiguration[s].parameter === pntParameterName);
          if (selectedParameterKey){
            return instance.rtuconfiguration.module0.moduleConfiguration[selectedParameterKey].dataType;
          }
          else{
            return null;
          }
        }
        else{
          return null;
        }
      case "2": // Interface Pnt
        if (pntIndexValue != '0') {
          let moduleString = "module" + pntIndexValue;
          let selectedParameterKey = Object.keys(instance.rtuconfiguration[moduleString].moduleConfiguration).find(s => instance.rtuconfiguration[moduleString].moduleConfiguration[s].parameter === pntParameterName);
          if (selectedParameterKey){
            return instance.rtuconfiguration[moduleString].moduleConfiguration[selectedParameterKey].dataType;
          }
          else{
            return null;
          }
        }
        else{
          return null;
        }
      case "3": // Port Pnt
      if (pntIndexValue != '0') {
        let indexValue = parseInt(pntIndexValue, 10);
        let moduleStringforPort = "module" + Math.floor(indexValue / 8);
        let channelString = "channel" + ((indexValue % 8) +1);
        let selectedParameterKey = Object.keys(instance.rtuconfiguration[moduleStringforPort][channelString].channelConfiguration).find(s => instance.rtuconfiguration[moduleStringforPort][channelString].channelConfiguration[s].parameter === pntParameterName);
        if (selectedParameterKey){
          return instance.rtuconfiguration[moduleStringforPort][channelString].channelConfiguration[selectedParameterKey].dataType;
        }
        else{
          return null;
        }
      }
      else{
        return null;
      }
    case "4": // FP Reg Pnt
      {
        let pntParameterKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntParameter');
        let pntParameterValue = alarm.point.pointConfiguration[pntParameterKey].pendingValue;
  
        if(pntParameterValue >= 99 && pntParameterValue <= 162){
          return 'double';
        }
        else if(pntParameterValue >= 40 && pntParameterValue <= 41){
          return 'unsigned long';
        }
        return null;
      }

      case "5": // INT Reg Pnt
      {
        let pntParameterKey = Object.keys(alarm.point.pointConfiguration).find(s => alarm.point.pointConfiguration[s].parameter === 'PntParameter');
        let pntParameterValue = alarm.point.pointConfiguration[pntParameterKey].pendingValue;

        if(pntParameterValue >= 99 && pntParameterValue <= 162){
          return 'unsigned long';
        }
        else if(pntParameterValue >= 40 && pntParameterValue <= 41){
          return 'unsigned long';
        }
        return null;
      }

      case "6": // GW Block Pnt
          return null;
      case "7": // Tank Pnt
        const tanks = instance.rtuconfiguration.points.filter(x => x.name === 'Tank');
        const tank = tanks[(parseInt(pntIndexValue, 10) - 1)];

        if (tank) {
          const selectedParameterKey = Object.keys(tank.pointConfiguration).find(s => tank.pointConfiguration[s].parameter === pntParameterName);
          if (selectedParameterKey)
            return tank.pointConfiguration[selectedParameterKey].dataType;
          }
          else {
            return null;
        }

        case "8": // Alarm Pnt

        // Using the label here causes a recursive problem. 
        if (instance.enabledAlarms) {
          let foundAlarm = instance.enabledAlarms.find(s => s.pointId === (parseInt(pntIndexValue, 10)+399));
          if (foundAlarm){
            const selectedParameterKey = Object.keys(foundAlarm.point.pointConfiguration).find(s => foundAlarm.point.pointConfiguration[s].parameter === pntParameterName);
            if(selectedParameterKey)
              return foundAlarm.point.pointConfiguration[selectedParameterKey].dataType;
            else
              return null;
          }
        }
        return null;
    }
  }

  updateAlarmTypeRestriction(alarm: alarm, datatypeString: string, alarmTypeParameter: IParameter) {
    switch (datatypeString) {
      case "string":
        if(alarmTypeParameter.availableCommands !== "Char Array"){
          alarmTypeParameter.availableCommands = "Char Array";
          if(alarmTypeParameter.pendingValue !== '6')
          {
            const textinputbox = document.getElementById(alarmTypeParameter.parameter);
            if(textinputbox != undefined)
            {
              let event;
              if (typeof(Event) === 'function') {
                event = new Event('change');
              } else {
                event = document.createEvent('Event');
                event.initEvent('change', true, true);
              }
              (<HTMLSelectElement>textinputbox).value = '6';
              this.updateTypeRestriction(alarm, alarmTypeParameter);
              setTimeout( function() {textinputbox.dispatchEvent(event);}, 1);
            }
          }
        }
        break;
      case "unsigned int":
      case "unsigned long":
        if(alarmTypeParameter.availableCommands !== "Bitmap,Match,Mismatch,Char Array"){
          alarmTypeParameter.availableCommands = "Bitmap,Match,Mismatch,Char Array";
          if(alarmTypeParameter.pendingValue !== '1'
          && alarmTypeParameter.pendingValue !== '2'
          && alarmTypeParameter.pendingValue !== '3'
          && alarmTypeParameter.pendingValue !== '6'){
            const textinputbox = document.getElementById(alarmTypeParameter.parameter);
            if(textinputbox != undefined)
            {
              let event;
              if (typeof(Event) === 'function') {
                event = new Event('change');
              } else {
                event = document.createEvent('Event');
                event.initEvent('change', true, true);
              }
              (<HTMLSelectElement>textinputbox).value = '1';
              this.updateTypeRestriction(alarm, alarmTypeParameter);
              setTimeout( function() {textinputbox.dispatchEvent(event);}, 1);
            }
          }
        }
        break;
      case "double":
        if(alarmTypeParameter.availableCommands !== "Low Threshold,High Threshold"){
          alarmTypeParameter.availableCommands = "Low Threshold,High Threshold";
          if(alarmTypeParameter.pendingValue !== '4'
          && alarmTypeParameter.pendingValue !== '5'){           
            const textinputbox = document.getElementById(alarmTypeParameter.parameter);
            if(textinputbox != undefined)
            {
              let event;
              if (typeof(Event) === 'function') {
                event = new Event('change');
              } else {
                event = document.createEvent('Event');
                event.initEvent('change', true, true);
              }
              (<HTMLSelectElement>textinputbox).value = '4';
              this.updateTypeRestriction(alarm, alarmTypeParameter);
              setTimeout( function() {textinputbox.dispatchEvent(event);}, 1);
            }
          }
        }
        break;
      default:
        {
          alarmTypeParameter.availableCommands = "Bitmap,Match,Mismatch,Low Threshold,High Threshold,Char Array";
          break;
        }
    }
  }

  getPntIndexNameForValue(alarmPoint: alarmPoint, pointIndexKey: string)
  {
    let instance = this;
    var i;
    let options= {};

    for (i = 1; i < 57; i++) {
      let moduleStringsWithCPU = ["module0","module1", "module2", "module3", "module4", "module5", "module6"];
      let channelStrings = ["channel1","channel2","channel3","channel4","channel5","channel6","channel7","channel8"];

      const moduleNumber = Math.floor((i - 1) / 8); 
      let selectedModule = instance.rtuconfiguration[moduleStringsWithCPU[moduleNumber]]; 
      let modLabelKey = Object.keys(selectedModule.moduleConfiguration).find(s => selectedModule.moduleConfiguration[s].parameter === 'Label');
      let moduleLabelString = selectedModule.moduleConfiguration[modLabelKey].pendingValue;
      const channelNumber =(((i - 1) % 8) + 1); 
      let selectedChannel = selectedModule[channelStrings[channelNumber-1]];
      let PntLabelKey = Object.keys(selectedChannel.channelConfiguration).find(s => selectedChannel.channelConfiguration[s].parameter === 'Label');
      let channelLabelString = selectedChannel.channelConfiguration[PntLabelKey].pendingValue;
      //instance.typeaheadOptions.push({ 'name': moduleLabelString + " " + channelLabelString, 'value': i });
      options[i] = moduleLabelString + " " + channelLabelString;
    }
    return options[alarmPoint.pointConfiguration[pointIndexKey].pendingValue];

  }

  setInputBoxStyle(anycell:any)
  {
    let styles = {};

    if(anycell.value.parameter == "PntParameter" || anycell.value.parameter == "PntIndex")
    {
      const textinputbox = document.getElementById(anycell.key);

      if(textinputbox != undefined)
      {
        var activeelement = document.activeElement;
        // HTMLSelectElement
        // I am doing this here because a race condition exists where we are setting the value but the options has not been updated
        if(textinputbox.className === "typeahead ng-untouched ng-pristine ng-valid" &&
        anycell.value.translatedPendingValue != '0' &&
        (<HTMLInputElement>textinputbox).value != anycell.value.translatedPendingValue)
        {
          (<HTMLInputElement>textinputbox).value = anycell.value.translatedPendingValue;
        }
        else if(anycell.value.translatedPendingValue === 'None' &&
        (<HTMLInputElement>textinputbox).value === '0' &&
        (<HTMLInputElement>textinputbox).value != anycell.value.translatedPendingValue)
        {
          (<HTMLInputElement>textinputbox).value = anycell.value.translatedPendingValue;
        }
        else if(anycell.value.translatedPendingValue != '0' &&
        (<HTMLInputElement>textinputbox).value === "" && (activeelement === undefined || activeelement.id != (<HTMLInputElement>textinputbox).id))
        {
          (<HTMLInputElement>textinputbox).value = anycell.value.translatedPendingValue;
        }
      }
    }
    return styles;
  }

  isFieldDisabled(cell:any)
  {
    if (cell.value.parameterIsVisible === 0)
      return '';
    else if (cell.value.configClass === configClass.DYNAMIC) {
      return '';
    }
    else if (cell.value.configClass === configClass.COMMAND
      && this._rtuConfiguration.connectionStatus !== RTUConnectionStatus.CONNECTED) {
      return '';
    }
    else if (cell.value.configClass === configClass.CONFIG
      && (this._rtuConfiguration.connectionStatus === RTUConnectionStatus.WRITINGCONFIGURATION
        || this._rtuConfiguration.connectionStatus === RTUConnectionStatus.ERRORWRITINGCONFIGURATION
        || this._rtuConfiguration.connectionStatus === RTUConnectionStatus.READINGCONFIGURATION
        || this._rtuConfiguration.connectionStatus === RTUConnectionStatus.ERRORREADINGCONFIGURATION)) {
      return '';
    }
    else {
      return null;
    }
  }
}

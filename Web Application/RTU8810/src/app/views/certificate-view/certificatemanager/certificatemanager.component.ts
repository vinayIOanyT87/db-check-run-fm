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

const certificateSelectedImage = './assets/certificate-selected.png';
const certificateUnSelectedImage = './assets/certificate-disabled.png';
const certificateDisabledImage = './assets/certificate-enabled.png';


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
class certificate {
  pointId: number;
  certificateIdx: number;
  label: string;
  labelLine1: string;
  labelLine2: string;
  activationimg: string;
  point: IPoint;
  img: string;

  constructor(pointId, certificateIdx, label, point) {
    this.pointId = pointId;
    this.certificateIdx = certificateIdx;
    this.label = label;

    if (label !== 'Undefined') {
      this.labelLine1 = label;
      this.labelLine2 = '';
    } else {
      this.labelLine1 = 'Undefined';
      this.labelLine2 = '';
    }
    this.point = point;
  }
}

class certificatePoint implements IPoint {
  name: string;
  pointConfiguration: IcertificateParameterMap;
  computedName: string;
}

class IcertificateParameterMap implements IParameterMap {
  [identifier: number]: IcertificateParameter;
}

class IcertificateParameter implements IParameter {
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
  selector: 'app-CertificateManager',
  templateUrl: './certificatemanager.component.html',
  styleUrls: ['./certificatemanager.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class CertificateManagerComponent implements OnInit, OnDestroy, AfterViewInit {
  selectedcertificate: certificate;
  selectedcertificates: certificate[] = [];
  certificateViewTabs: ITab[];
  enabledcertificates: certificate[];
  certificatePanel: certificate[];
  certificatePanelSets: any[];  // left panel: group of 4 reg map to display on a row
  ready = false;
  rtuconfiguration: IRTUConfiguration;
  rtuconfigurationSubscription: Subscription;
  certificateParameters: IParameter[];
  searchcertificateString = '';
  searchcertificateToggle = false;
  searchConfigurationString = '';
  labelIdentifier: number;
  dragSelectActive = false;
  modalRef: BsModalRef;
  showPointIndexes = false;
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
    this.showPointIndexes = (localStorage.getItem('showPointIndexes') === 'true') ? true : false;

    this.getRTUConfiguration();
  }

  ngOnDestroy() {
    if (this._rtuConfiguration) {
      this._rtuConfiguration.unsubscribeRealtimeParameters(this.certificateParameters);
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

      this.updateSelectedcertificateLabels();

      instance.selectedcertificates.forEach(function (certificate) {
        instance.updateSelectedcertificateLabels();
      });

      instance.getEnabledcertificates();
      instance.getcertificateSelectTab(); // get the list of certifictates to display in the left panel
      instance.getCertificateViewTab();

    });
  }

  getEnabledcertificates() {
    if (this._rtuConfiguration
      && this.certificateParameters) {
      this._rtuConfiguration.unsubscribeRealtimeParameters(this.certificateParameters);
    }

    this.certificateParameters = [];
    const enabledcertificate = [];
    this.labelIdentifier = -1;
    const instance = this;
    let certificateIdx = 0;

    if (this.rtuconfiguration != null
      && this.rtuconfiguration.points != null) {
      let numberOfcertificates = 20;
      const moduleConfiguration = this.rtuconfiguration.module0.moduleConfiguration;
      // tslint:disable-next-line:max-line-length
      const numberOfcertificatesIdentifier = 20;

      this.rtuconfiguration.points.forEach(function (point, index) {
        if (certificateIdx >= numberOfcertificates) {
          return;
        }

        if (point.name === ' X.509 Certificate ') {
          const certificatePoint = point as certificatePoint;

          certificatePoint.computedName = instance.getcertificateComputedName(certificatePoint);
          const newcertificate = new certificate(index, certificateIdx++, certificatePoint.computedName, certificatePoint);

          newcertificate.label = certificatePoint.computedName;
          if (instance.showPointIndexes) {
            newcertificate.labelLine1 += ' (' + (newcertificate.certificateIdx + 1).toString().padStart(3, '0') + ')';
          }
          instance.updatecertificateImage(newcertificate);
          enabledcertificate.push(newcertificate);
        }
      });
    }

    instance.enabledcertificates = enabledcertificate.sort(instance.comparecertificates);

    if (instance.selectedcertificate != null) {
      // tslint:disable-next-line:max-line-length
      Object.keys(instance.selectedcertificate.point.pointConfiguration).map(s => instance.selectedcertificate.point.pointConfiguration[s]).forEach(function (parameter) {
        instance.certificateParameters.push(parameter);
      });
    }

    if (instance._rtuConfiguration && instance.certificateParameters) {
      instance._rtuConfiguration.subscribeRealtimeParameters(this.certificateParameters);
    }
  }

  // get the reg maps to display n the left panel
  getcertificateSelectTab() {
    const filteredcertificate = [];
    const instance = this;
    const upperCaseSearchString = this.searchcertificateString.toUpperCase();
    this.enabledcertificates.forEach(function (certificate) {
      if (!instance.searchcertificateString
        || instance.searchcertificateString === ''
        || (instance.searchcertificateString !== ''
          && (certificate.label.toUpperCase().indexOf(upperCaseSearchString) !== -1)
          || (certificate.labelLine1.toUpperCase().indexOf(upperCaseSearchString) !== -1))) {
        filteredcertificate.push(certificate);
      } else if (certificate === instance.selectedcertificate) {
        instance.selectedcertificate = null;
      }
    });

    instance.certificatePanel = filteredcertificate.sort(this.comparecertificates);

    // left pane: divide the list of reg maps into groups of 4 for display on the left panel
    if (instance.certificatePanel.length > 0) {
      instance.certificatePanelSets = instance.certificatePanel.reduce((resultArray, alarm, index) => {
        const chunkIndex = Math.floor((index) / 4);

        if (!resultArray[chunkIndex]) {
          resultArray[chunkIndex] = []; // start a new chunk
        }

        resultArray[chunkIndex].push(alarm);

        return resultArray;
      }, []);
    } else {
      instance.certificatePanelSets = [];
    }
  }

  getCertificateViewTab() {
    const instance = this;
    if (!instance.certificateViewTabs) {
      instance.certificateViewTabs = [{ tabName: 'Config' }, { tabName: 'Command' }];
    }


    if (!instance.selectedcertificate) {
      return;
    }
  }

  trackByPointId(index, item) {
    return item[0].pointId;
  }

  comparecertificates(a: certificate, b: certificate) {
    if (a.certificateIdx > b.certificateIdx) {
      return 1;
    }
    else
      return -1;

  }

  updatecertificateImage(certificate: certificate, path = null) {
    if (path) {
      //certificate.img = path;
    } else {
      const instance = this;
      let selectedImage: string;
      let unselectedImage: string;
      let disabledImage: string;

      let certStateKey = Object.keys(certificate.point.pointConfiguration).find(s => certificate.point.pointConfiguration[s].parameter === 'CertState');

      let certStateValue = certificate.point.pointConfiguration[certStateKey].pendingValue;

      selectedImage = certificateSelectedImage;
      unselectedImage = certificateUnSelectedImage;
      disabledImage = certificateDisabledImage


      if (instance.selectedcertificates.findIndex(x => x.pointId === certificate.pointId) !== -1) {
        certificate.img = selectedImage;
      } else if (certStateValue == 1) {
        certificate.img = unselectedImage;
      }
      else {
        certificate.img = disabledImage;
      }
    }
  }

  getcertificateComputedName(certificatePoint: certificatePoint) {
    const instance = this;
    const labelKey = Object.keys(certificatePoint.pointConfiguration).find(s => certificatePoint.pointConfiguration[s].parameter === 'Label');

    return certificatePoint.pointConfiguration[labelKey].pendingValue;
  }

  updateSelectedcertificateLabels() {
    const instance = this;

    instance.selectedcertificates.forEach(function (certificate) {
      const certificatePoint = certificate.point as certificatePoint;

      certificatePoint.computedName = instance.getcertificateComputedName(certificatePoint);
      certificate.label = certificatePoint.computedName;
    });
  }

  oncertificateImgClick(event: any, pointId: number, alarmId: number) {
    const instance = this;
    const clickedAlarm = instance.enabledcertificates.find(s => s.pointId === pointId);

    if (instance.selectedcertificates.indexOf(instance.selectedcertificates.find(s => s.pointId === clickedAlarm.pointId)) === -1) {
      // regular clicked an unselected reg map. Clear selectedcertificates and revert images, and then select clicked alarm

      instance.selectedcertificates = [];
      instance.selectedcertificate = instance.enabledcertificates.find(s => s.pointId === pointId);
      instance.selectedcertificates.push(instance.selectedcertificate);
    } else {

      instance.selectedcertificates.forEach(function (alarm) {
      });
      // if we are in batch edit mode and regular click, single select that reg map
      if (instance.selectedcertificates.length > 1) {
        instance.selectedcertificate = instance.enabledcertificates.find(s => s.pointId === pointId);
        instance.selectedcertificates = [];
        instance.selectedcertificates.push(instance.selectedcertificate);
      } else { // otherwise we clicked on the only one selected. deselect it.
        instance.selectedcertificates = [];
        instance.selectedcertificate = null;
      }
    }
    instance.getEnabledcertificates();
    instance.getcertificateSelectTab();

  }



  // left panel search
  togglecertificateSearch() {
    this.searchcertificateToggle = !this.searchcertificateToggle;
    this.searchcertificateString = '';
    this.getcertificateSelectTab();
    if (this.searchcertificateToggle) {
      const input = document.getElementById('certificatefilterinput');
      setTimeout(() => { input.focus(); }, 100);
    }
  }

  searchcertificateStringChanged(newSearchValue) {
    this.searchcertificateString = newSearchValue;
    this.getcertificateSelectTab();
    this.getCertificateViewTab();
  }

  oncertificatePanelClick(event: any) {
    const instance = this;
    if (instance.dragSelectActive === false) {
      instance.selectedcertificates = [];
      if (instance.selectedcertificate) {
        instance.selectedcertificate = null;
      }

      instance.getEnabledcertificates();
      instance.getcertificateSelectTab();
      instance.getCertificateViewTab();
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

  updatePendingValue(e: NgbTypeaheadSelectItemEvent, certificate: certificate, parameterSelected: IParameter) {
    const originalPendingValue = parameterSelected.pendingValue;

    const parameter = parameterSelected;
    const instance = this;
    parameterSelected.pendingValue = e.item.value;
    parameterSelected['temppendingValue'] = e.item;

    this.checkForIncrementGlobalPendingChanges(parameter, originalPendingValue);
    let certificatePoint = certificate.point as certificatePoint;

    certificatePoint.computedName = instance.getcertificateComputedName(certificatePoint);
    certificate.label = certificatePoint.computedName;


    instance.getEnabledcertificates();
    instance.getcertificateSelectTab();
  }

  cancelChanges() {
    const instance = this;

    this._rtuConfiguration.cancelPendingChanges();

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


  onParameterChange(parameter: IParameter, certificate: certificate) {
    const instance = this;

    if (parameter.configClass === configClass.CONFIG) {
      instance.selectedcertificate = instance.enabledcertificates.find(s => s.pointId === certificate.pointId);
      const certificatePoint = certificate.point as certificatePoint;
      instance.getEnabledcertificates();
      instance.getcertificateSelectTab();

    } else if (parameter.configClass === configClass.COMMAND) {
      this._rtuConfiguration.applyCommandToRTU(parameter);
    }
  }

  togglePointIdx() {
    const instance = this;
    this.showPointIndexes = !this.showPointIndexes;
    localStorage.setItem('showPointIndexes', this.showPointIndexes.toString());
    this.getEnabledcertificates();
    this.getcertificateSelectTab();
    this.getCertificateViewTab();
    this.updateSelectedcertificateLabels();
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
        instance.enabledcertificates.forEach(function (certificate) {
          let pointIndexKey = Object.keys(certificate.point.pointConfiguration).find(
            s => certificate.point.pointConfiguration[s].parameter === 'SrcIndex');
          let pntIndexParameter = certificate.point.pointConfiguration[pointIndexKey];

          if (pntIndexParameter['tempValue'] !== undefined && pntIndexParameter['tempValue'] !== 'None') {
            pntIndexParameter['tempValue'] = 'None';
          }

          let pointParameterKey = Object.keys(certificate.point.pointConfiguration).find(
            s => certificate.point.pointConfiguration[s].parameter === 'SrcParameter');
          let pntParameterParameter = certificate.point.pointConfiguration[pointParameterKey];

          if (pntParameterParameter['tempValue'] !== undefined && pntParameterParameter['tempValue'] !== 'None') {
            pntParameterParameter['tempValue'] = 'None';
          }

          pointIndexKey = Object.keys(certificate.point.pointConfiguration).find(
            s => certificate.point.pointConfiguration[s].parameter === 'DestIndex');
          pntIndexParameter = certificate.point.pointConfiguration[pointIndexKey];

          if (pntIndexParameter['tempValue'] !== undefined && pntIndexParameter['tempValue'] !== 'None') {
            pntIndexParameter['tempValue'] = 'None';
          }

          pointParameterKey = Object.keys(certificate.point.pointConfiguration).find(
            s => certificate.point.pointConfiguration[s].parameter === 'DestParameter');
          pntParameterParameter = certificate.point.pointConfiguration[pointParameterKey];

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

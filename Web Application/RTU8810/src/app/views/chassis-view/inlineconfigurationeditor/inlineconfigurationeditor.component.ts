import { Component, OnInit, Input, Output, EventEmitter, TemplateRef, ViewChild, DoCheck } from '@angular/core';
import { configClass, allProtocols, IParameter, alarmTypes } from 'src/app/services/availablemodules.service';
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';
import { RtuconfigurationService } from 'src/app/services/rtuconfiguration.service';
import { Subscription } from 'rxjs';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';

@Component({
  selector: 'app-inlineconfigurationeditor',
  templateUrl: './inlineconfigurationeditor.component.html',
  styleUrls: ['./inlineconfigurationeditor.component.css']
})
export class InlineconfigurationeditorComponent implements OnInit {
  rtuConnectionStatusSubscription: Subscription;

  modalRef: BsModalRef;
  modalConfig = {
    backdrop: true,
    ignoreBackdropClick: false,
    class: 'modal-lg'
  };

  @Input() configurationItem: IParameter;
  oldConfigurationItem: IParameter;
  @Input() disableOverride: boolean;
  oldDisableOverride: boolean;
  @Input() tabIndex: number;
  @Output() protocolChange = new EventEmitter<string>();
  @Output() moduleInstalledChange = new EventEmitter<string>();
  @Output() moduleCommandApply = new EventEmitter<IParameter>();
  @Output() parameterChange = new EventEmitter<IParameter>();
  @ViewChild('modalConfirmIssueCommandToRTU', { static: true }) public modalConfirmIssueCommandToRTU: TemplateRef<any>;
  @ViewChild('modalPendingConfigurationToRTU', { static: true }) public modalPendingConfigurationToRTU: TemplateRef<any>;

  differ: any;

  //custom change detect to pick up changes to dropdowns
ngDoCheck() {
if (this.oldConfigurationItem.availableCommands != this.configurationItem.availableCommands)
{
    this.oldConfigurationItem = Object.assign({}, this.configurationItem);
    let availableCommands = this.configurationItem.availableCommands.split(',');
    if (this.configurationItem.parameter === 'Type') {
      this.options = [{ id: '', name: '' }];
      for (var loop = 0; loop < availableCommands.length; loop++) {
        this.options[loop] = { id: alarmTypes[availableCommands[loop]], name: availableCommands[loop] };
      }
      //this.configurationItem.pendingValue = this.options[0].id;
  }
}
if (this.disableOverride != this.oldDisableOverride)
{
    //change detect not working?
}

}
  changed: boolean;
  originalValue: string;
  connectionStatus: RTUConnectionStatus = RTUConnectionStatus.DISCONNECTED;
  commandPending = false;

  options: any = [{ id: '', name: '' }];

  customMaskPattern = { '0': { pattern: new RegExp('[0-9]') }, '9': { pattern: new RegExp('[0-9]') }, 'V': { pattern: new RegExp('[-?0-9.]') } };

  // constants
  PROTOCOL = 'Protocol';
  MODULECONFIGURED = 'ModConfigured';
  TANKVISIBLE = 'TankVisible';
  LABEL = 'Label';

  constructor(private _rtuConfiguration: RtuconfigurationService,
    private _RtuconnectionstatusService: RtuconnectionstatusService,
    private _modalService: BsModalService) {}

  ngOnInit() {
    this.oldConfigurationItem = Object.assign({}, this.configurationItem);
    this.oldDisableOverride =  this.disableOverride;

    this.rtuConnectionStatusSubscription = this._RtuconnectionstatusService.get().subscribe(data => this.connectionStatus = data);

    if (this.configurationItem) {
      if (this.configurationItem.availableCommands && this.configurationItem.availableCommands !== '') {
        // build the dropdown options
        // protocol will be built differently since we don't show all the options
        // Modules start with 0 instead of 1
        if (this.configurationItem.parameter.indexOf(" **") > 0) {
          var tempString = this.configurationItem.parameter.substring(0, this.configurationItem.parameter.indexOf(" **"));
          this.configurationItem.parameter = tempString;
        }
        if (this.configurationItem.parameter === 'Protocol') {
          const availableCommands = this.configurationItem.availableCommands.split(',');
          this.options = availableCommands.map(function (elem, x) { return { id: allProtocols[elem].toString(), name: elem }; });
        } else if (this.configurationItem.parameter === 'ModInstalled' || this.configurationItem.parameter === 'ModConfigured') {

          const availableCommands = this.configurationItem.availableCommands.split(',');
          // if the first entry is undefined remove it (it will be readded later)
          if (availableCommands[0].toLowerCase() === 'undefined') {
            availableCommands.shift();
          }

          this.options = availableCommands.map(function (elem, x) { return { id: (x + 1).toString(), name: elem }; });

          this.options.unshift({ id: '0', name: 'Undefined' });

        } else {
          // rules (they start with a 1 value, but there could be a 0 Undefined option preceeding it)

          const availableCommands = this.configurationItem.availableCommands.split(',');
          if (this.configurationItem.parameter === 'DeviceType' && availableCommands.length === 1 && availableCommands[0].toUpperCase() === "NONE") {
            // none. can only happen when multiple tanks are selected with incompatible protocals
            this.options.unshift({ id: '', name: '' });
            if (this.configurationItem.pendingValue === '') {
              this.configurationItem.pendingValue = '0';
            }
            return;
          }

          if (this.configurationItem.availableCommandsOutputMatches === 1) // if this is set then the displayed value and the sent values are the same
          {
            this.options = availableCommands.map(function (elem, x) { return { id: elem.toString(), name: elem }; });
          }
          /******************************************************************************************** */
          // This code needs to be removed once the gw blk pnt is readded to the alarm point type selection
          /******************************************************************************************** */

          else if (this.configurationItem.parameter === 'PntType') {
            var tempavailableCommands = "None, CPU Pnt,Interface Pnt,Port Pnt,FP Reg Pnt,INT Reg Pnt,Tank Pnt,Alarm Pnt";
            var tempavailableCommandValues = "0,1,2,3,4,5,7,8";
            const availableCommands1 = tempavailableCommands.split(',');
            const tempavailableCommandValues1 = tempavailableCommandValues.split(',');
            this.options = availableCommands1.map(function (elem, x) { return { id: (x + 1).toString(), name: elem }; });
            for (var loop = 0; loop < this.options.length; loop++) {
              this.options[loop].id = tempavailableCommandValues1[loop];
            }
          }

          else if (this.configurationItem.parameter === 'SrcType' || this.configurationItem.parameter === 'DestType') {
            var tempavailableCommands = "None,CPU Pnt,Interface Pnt,Port Pnt,FP Reg Pnt,INT Reg Pnt,GW Block Pnt,Tank Pnt,Alarm Pnt";
            var tempavailableCommandValues = "0,1,2,3,4,5,7,8";
            const availableCommands1 = tempavailableCommands.split(',');
            this.options = [];
            this.options.push({ id: '0', name: 'None' });

            for (var loop = 0; loop < availableCommands.length; loop++) {
              const arrIndex = availableCommands1.findIndex(k => k == availableCommands[loop]);
              if (arrIndex > 0) {
                this.options.push({ id: arrIndex.toString(), name: availableCommands[loop] });
              }
            }
          }

          else if (this.configurationItem.parameter === 'Type') {

            for (var loop = 0; loop < availableCommands.length; loop++) {
              this.options[loop] = { id: alarmTypes[availableCommands[loop]], name: availableCommands[loop] };
            }
          }

          /******************************************************************************************** */
          // This code needs to be removed once the gw blk pnt is readded to the alarm point type selection
          /******************************************************************************************** */
          else {
            // if the dropdown is just a subset of the list we have availableDeviceTypeValues with the sublist (i.e. DeviceCmd, NMSDeviceCmd)
            if (this.configurationItem.availableDeviceTypeValues !== undefined && this.configurationItem.parameter !== 'DeviceType') {
              const tempOptions = [];
              const commands = this.configurationItem.availableDeviceTypeValues.split(',');
              // go through each of the available commands and if is support it display it on the screen
              availableCommands.forEach(function (elem, x) {
                if ( commands.indexOf(elem) >= 0 ) {
                  tempOptions.push({ id: (x + 1).toString(), name: elem });
                }
              });
              this.options = tempOptions;
            } else {
               this.options = availableCommands.map(function (elem, x) { return { id: (x + 1).toString(), name: elem }; });
            }
           
          }

          // if this is devicetype then we need to map the id to the main list bds
          if (this.configurationItem.parameter === 'DeviceType') {
            const availableDeviceTypeValues = this.configurationItem.availableDeviceTypeValues.split(',');
            for (var loop = 0; loop < this.options.length; loop++) {
              this.options[loop].id = availableDeviceTypeValues[loop];
            }
          }
          // for command/dynamic we want to add a no option with the exception of device command
          //if (this.configurationItem.parameter !== 'DeviceCmd' && (this.configurationItem.configClass === configClass.DYNAMIC || this.configurationItem.configClass === configClass.COMMAND)) {
          //this.options.unshift({ id: '', name: '' });
          //if (this.configurationItem.pendingValue === '') {
          //this.configurationItem.pendingValue = '0';
          //}

          //}
        }
      }

      this.changed = false;

      if (this._rtuConfiguration.isParameterValueChanged(this.configurationItem)) {
        this.changed = true;
      }

      this.originalValue = this.configurationItem.pendingValue;
    }
  }

  ngOnDestroy() {
    this.rtuConnectionStatusSubscription.unsubscribe();
  }

  valueChange(newValue: string, event: { preventDefault: () => void; target: HTMLInputElement; }) {

    //parameter specific validation. May need to be incorporated into the XML in the future for better modularity
    if (this.configurationItem.parameter == 'Mask' && newValue == '') {
      // it looks like we need to set the value and then set it back or it does not update on the screen
      // new angular problem?
      (<HTMLInputElement>event.target).value = "Test";//this.originalValue;
      (<HTMLInputElement>event.target).value = "0";
      newValue = "0"
    }
    if (this.configurationItem.parameter == 'Deadband' && newValue.indexOf('-') != -1) {
      event.preventDefault();
      // it looks like we need to set the value and then set it back or it does not update on the screen
      // new angular problem?
      (<HTMLInputElement>event.target).value = "Test";//this.originalValue;
      (<HTMLInputElement>event.target).value = this.originalValue;
      return false;
    }

    if (!this._rtuConfiguration.isParameterValueChanged(this.configurationItem)) {
      this.changed = false;
    }

    if (this.configurationItem.dataType.toLowerCase() == 'double'
      && newValue == this.configurationItem.pendingValue) {
      return;
    }

    if (this.configurationItem.configClass === configClass.COMMAND) {
      this.configurationItem.pendingValue = newValue;
      this.configurationItem.pendingStatus = 0;

      if (this._rtuConfiguration.checkForRTUConfigChanges()) {
        this.modalRef = this._modalService.show(this.modalPendingConfigurationToRTU, this.modalConfig)
      }
      else {
        this.modalRef = this._modalService.show(this.modalConfirmIssueCommandToRTU, this.modalConfig);
      }
    }
    else {

      this.originalValue = newValue;
      this.configurationItem.pendingValue = newValue;
      this.configurationItem.pendingStatus = 0;


      if (this._rtuConfiguration.isParameterValueChanged(this.configurationItem)) {
        const codeBits = (this.configurationItem.status / 32767) * 32767;

        // If the value is changed but the current value is BadNodeIdInvalid or BadNodeIdUnknown then the change has already been counted
        if (codeBits !== 0x80340000
          && codeBits !== 0x80350000
          && !this.changed) {
          this.changed = true;
          if (this.configurationItem.opcstartNodeID != -999)
            this._rtuConfiguration.incrementGlobalPendingChanges();
        }
      }
      else {
        if (this.changed) {
          this.changed = false;
          if (this.configurationItem.opcstartNodeID != -999)
            this._rtuConfiguration.decrementGlobalPendingChanges();
        }
      }

      this.parameterChange.emit(this.configurationItem);
    }
  }

  getParameterCommand() {
    let parameterCommand = this.configurationItem.pendingValue;

    if (this.configurationItem.availableCommands
      && this.configurationItem.availableCommands !== '') {
      const commands = this.configurationItem.availableCommands.split(',');
      const commandIndex = parseInt(parameterCommand, 10);
      if (commandIndex > 0 && commands.length > commandIndex - 1) {
        parameterCommand = commands[commandIndex - 1];
      }
    }

    return parameterCommand;
  }



  clearDropdown(configurationItem: any) {
    if (this.commandPending == false && this.configurationItem.configClass === configClass.COMMAND)
      configurationItem.pendingValue = '';



  }

  valueDropDownChange(newValue: any) {
    this.commandPending = true;
    if (!this._rtuConfiguration.isParameterValueChanged(this.configurationItem)) {
      this.changed = false;
    }

    if (this.configurationItem.configClass === configClass.COMMAND) {
      this.configurationItem.pendingValue = newValue;
      this.configurationItem.pendingStatus = 0;

      if (this._rtuConfiguration.checkForRTUConfigChanges()) {
        this.modalRef = this._modalService.show(this.modalPendingConfigurationToRTU, this.modalConfig)
      }
      else {
        this.modalRef = this._modalService.show(this.modalConfirmIssueCommandToRTU, this.modalConfig);
      }
    }
    else {

      this.originalValue = newValue;
      this.configurationItem.pendingValue = newValue;
      this.configurationItem.pendingStatus = 0;


      if (this._rtuConfiguration.isParameterValueChanged(this.configurationItem)) {
        const codeBits = (this.configurationItem.status / 32767) * 32767;

        // If the value is changed but the current value is BadNodeIdInvalid or BadNodeIdUnknown then the change has already been counted
        if (codeBits !== 0x80340000
          && codeBits !== 0x80350000
          && !this.changed) {
          this.changed = true;
          if (this.configurationItem.opcstartNodeID != -999)
            this._rtuConfiguration.incrementGlobalPendingChanges();
        }
      }
      else {
        if (this.changed) {
          this.changed = true;
          if (this.configurationItem.opcstartNodeID != -999)
            this._rtuConfiguration.decrementGlobalPendingChanges();
        }
      }

      if (this.configurationItem.parameter === this.PROTOCOL) {
        this.protocolChange.emit(newValue);
        return
      }

      if (this.configurationItem.parameter === this.MODULECONFIGURED) {
        this.moduleInstalledChange.emit(newValue);
        return;
      }

      this.parameterChange.emit(this.configurationItem);
    }
  }

  cancelCommandToRTU() {
    this.commandPending = false;
    this.configurationItem.pendingValue = this.originalValue;
    this.modalRef.hide();
  }

  issueCommandToRTU() {
    this.originalValue = this.configurationItem.pendingValue;
    this.moduleCommandApply.emit(this.configurationItem);
    this.parameterChange.emit(this.configurationItem);
    this.modalRef.hide();
    this.commandPending = false;
  }

  revertValue(event: { preventDefault: () => void; target: HTMLInputElement; }) {
    event.preventDefault();
    // it looks like we need to set the value and then set it back or it does not update on the screen
    // new angular problem?
    (<HTMLInputElement>event.target).value = "Test";//this.originalValue;
    (<HTMLInputElement>event.target).value = this.originalValue;
    return false;
  }


  min() {
    let minValue: number = null;

    switch (this.configurationItem.dataType) {
      case 'int':
        minValue = (this.configurationItem.minimumValue) ? this.configurationItem.minimumValue : -2147483648;
        break;
      case 'long':
        minValue = (this.configurationItem.minimumValue) ? this.configurationItem.minimumValue : -9223372036854775808;
        break;
      case 'unsigned int':
        minValue = (this.configurationItem.minimumValue) ? this.configurationItem.minimumValue : 0;
        break;
      case 'unsigned long':
        minValue = (this.configurationItem.minimumValue) ? this.configurationItem.minimumValue : 0;
        break;
      case 'double':
        minValue = (this.configurationItem.minimumValue);
        break;
      default:
        minValue = 0;
    }
    return minValue;
  }

  max() {
    let maxValue: number = null;

    switch (this.configurationItem.dataType) {
      case 'int':
        maxValue = (this.configurationItem.maximumValue) ? this.configurationItem.maximumValue : 2147483647;
        break;
      case 'long':
        maxValue = (this.configurationItem.maximumValue) ? this.configurationItem.maximumValue : 9223372036854775807;
        break;
      case 'unsigned int':
        maxValue = (this.configurationItem.maximumValue) ? this.configurationItem.maximumValue : 4294967295;
        break;
      case 'unsigned long':
        maxValue = (this.configurationItem.maximumValue) ? this.configurationItem.maximumValue : 18446744073709551615;
        break;
      case 'double':
        maxValue = (this.configurationItem.maximumValue);
        break;
      default:
        maxValue = 0;
    }
    return maxValue;
  }

  isFieldDisabled() {
    if (this.disableOverride || this.configurationItem.parameterIsVisible === 0)
      return '';
    else if (this.configurationItem.configClass === configClass.DYNAMIC) {
      return '';
    }
    else if (this.configurationItem.configClass === configClass.COMMAND
      && this.connectionStatus !== RTUConnectionStatus.CONNECTED) {
      return '';
    }
    else if (this.configurationItem.configClass === configClass.CONFIG
      && (this.connectionStatus === RTUConnectionStatus.WRITINGCONFIGURATION
        || this.connectionStatus === RTUConnectionStatus.ERRORWRITINGCONFIGURATION
        || this.connectionStatus === RTUConnectionStatus.READINGCONFIGURATION
        || this.connectionStatus === RTUConnectionStatus.ERRORREADINGCONFIGURATION)) {
      return '';
    }
    else {
      return null;
    }
  }

  useSmallText (item: IParameter)
  {
    if (item.pendingValue && item.pendingValue.length>24)
    return true;
    return false;
  }

}

import { Component, OnInit, OnChanges, OnDestroy, Input, SimpleChanges, ViewEncapsulation,TemplateRef, ViewChild } from '@angular/core';
// tslint:disable-next-line:max-line-length
import { RtuconfigurationService, IRTUConfiguration, IRTUInterfaceModule, IPoint, IPointData } from 'src/app/services/rtuconfiguration.service';
import { SelectedmodulechannelService, ISelectedModuleChannel } from 'src/app/services/selectedmodulechannel.service';
// tslint:disable-next-line:max-line-length
import { configClass, AvailablemodulesService, IParameter, IAvailableConfiguration, IAvailableProtocol, IParameterMap } from 'src/app/services/availablemodules.service';
import * as saveAs from 'node_modules/file-saver';
// import { forEach } from '@angular/router/src/utils/collection';
import { Router } from '@angular/router';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
import { Subscription } from 'rxjs';
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';
// import { platformCoreDynamicTesting } from '@angular/platform-browser-dynamic/testing/src/platform_core_dynamic_testing';

interface IChassisTankInfo {
  index: number;
  name: String;
  assigned: Boolean;
}

@Component({
  selector: 'app-moduleconfiguration',
  templateUrl: './moduleconfiguration.component.html',
  styleUrls: ['./moduleconfiguration.component.css'],
  encapsulation: ViewEncapsulation.None
})

export class ModuleconfigurationComponent implements OnInit, OnDestroy, OnChanges {
  rtuConfigurationSubscription: Subscription;
  @Input() rtuconfiguration: IRTUConfiguration;
  @Input() availableConfiguration: IAvailableConfiguration;

  currentModule = 0;
  currentChannel = 0;
  searchString =  '';
  addTankMode = false;

  currentModuleDetails: IRTUInterfaceModule;
  rtuConfigModuleDetails: IParameter[];
  rtuDynamicModuleDetails: IParameter[];
  rtuChannelProtocol: IParameter[];
  rtuTankList: string[];
  rtuAllTanks: IChassisTankInfo[];

  // constants
  PROTOCOL = 'Protocol';

  constructor( private _SelectedmodulechannelService: SelectedmodulechannelService,
    private _rtuConfiguration: RtuconfigurationService,
    private _availableModuleService: AvailablemodulesService,
    private _modalService: BsModalService,

    private router: Router) {

      this.rtuConfigurationSubscription = this._rtuConfiguration.changeDetectionEmitter.subscribe(
        () => {
          if ( this.rtuconfiguration && this.rtuconfiguration.module0 ) {
          this.populateSections();
          }
        },
        (err) => { }
      );

    }

    modalRef: BsModalRef;
    modalConfig = {
      backdrop: true,
      ignoreBackdropClick: false,
      class: 'modal-lg'
    };
    @ViewChild('modalVerifyApplytoRTUAction', { static: true }) public modalVerifyApplytoRTUAction: TemplateRef<any>;
    @ViewChild('modalVerifyCancelAction', { static: true }) public modalVerifyCancelAction: TemplateRef<any>;

    ngOnInit() {
    this.rtuConfigModuleDetails = [];
    this.rtuDynamicModuleDetails = [];
    this.rtuTankList = [];

    if ( this.rtuconfiguration && this.rtuconfiguration.module0 ) {
        this.currentModuleDetails = this.rtuconfiguration[ 'module' + this.currentModule];
        this.populateSections();
    }
    this._SelectedmodulechannelService.get().subscribe( data => this.onSelectedModuleChannelChanged( data ) );
  }

  ngOnDestroy() {
    this.rtuConfigurationSubscription.unsubscribe();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.rtuconfiguration  && changes.rtuconfiguration.currentValue && changes.rtuconfiguration.currentValue.module0 ) {
      this.currentModuleDetails = changes.rtuconfiguration.currentValue[ 'module' + this.currentModule ];
      this.populateSections();
    }
  }

  onSelectedModuleChannelChanged(selectedModuleChannel: ISelectedModuleChannel) {
    this.currentModule = selectedModuleChannel.selectedModule;
    this.currentChannel = selectedModuleChannel.selectedChannel;

    if ( this.rtuconfiguration && this.rtuconfiguration.module0 ) {
        this.currentModuleDetails = this.rtuconfiguration[ 'module' + this.currentModule ];
      this.populateSections();
    }
    this.addTankMode = false;
  }

  populateSections() {
    this.rtuConfigModuleDetails = [];
    this.rtuDynamicModuleDetails = [];
    this.rtuChannelProtocol = null;
    this.rtuTankList = [];

    // the component variables cannot be directly accessed in arrow functions
    const rtuTankList = this.rtuTankList;
    const currentChannel = this.currentChannel;
    const currentModule = this.currentModule;
    const searchString = this.searchString;

    // generate the different detail section
    if ( this.currentChannel === 0 ) {
      if ( this.currentModuleDetails.moduleConfiguration ) {
        Object.keys(this.currentModuleDetails.moduleConfiguration).forEach(key => {
          if (this.currentModuleDetails.moduleConfiguration[key].configClass === configClass.CONFIG) {
            const parameterValue = this.currentModuleDetails.moduleConfiguration[key];
            if ( parameterValue.parameterIsVisible === 1 ) {
              if ( this.searchString === '') {
                this.rtuConfigModuleDetails.push(parameterValue);
              } else if ( parameterValue.parameter.toLowerCase().includes( this.searchString.toLowerCase() )) {
                this.rtuConfigModuleDetails.push(parameterValue);
              }
            }
          }

          if (this.currentModuleDetails.moduleConfiguration[key].configClass === configClass.COMMAND ||
              this.currentModuleDetails.moduleConfiguration[key].configClass === configClass.DYNAMIC  ) {
                const parameterValue = this.currentModuleDetails.moduleConfiguration[key];
                if ( this.searchString === '') {
                  this.rtuDynamicModuleDetails.push(parameterValue);
                } else if ( parameterValue.parameter.toLowerCase().includes( this.searchString.toLowerCase() )) {
                  this.rtuDynamicModuleDetails.push(parameterValue);
                }
              }
        });

        this.rtuconfiguration.points.forEach( function( value: IPoint, index: number) {
          // get the tank label, module and channel
          let label = '';
          let module = '-1';
          let channel = '-1';
          let tankVisible = '';
          Object.keys(value.pointConfiguration).forEach(key => {
            if ( value.pointConfiguration[ key ].parameter === 'Label' ) {
              label = value.pointConfiguration[key].pendingValue;
            }
            if ( value.pointConfiguration[ key ].parameter === 'Module' ) {
              module = value.pointConfiguration[key].pendingValue;
            }
            if ( value.pointConfiguration[ key ].parameter === 'Channel' ) {
              channel = value.pointConfiguration[key].pendingValue;
            }
            if ( value.pointConfiguration[ key ].parameter === 'TankVisible' ) {
              tankVisible = value.pointConfiguration[key].pendingValue;
            }
          });
          // display visible tanks that are assigned to the module
          if ( parseInt( module, 10 ) === currentModule && parseInt( channel, 10 ) > 0 && tankVisible === '2') {
            if ( searchString === '') {
              rtuTankList.push( label );
            } else if ( label.toLowerCase().includes( searchString.toLowerCase() )) {
              rtuTankList.push( label );
            }

          }
        });

      }
    } else {
        if ( this.currentModuleDetails.moduleConfiguration ) {
          // tslint:disable-next-line:max-line-length

          Object.keys(this.currentModuleDetails['channel' + this.currentChannel].channelConfiguration).forEach(key => {
            if (this.currentModuleDetails['channel' + this.currentChannel].channelConfiguration[key].configClass === configClass.CONFIG &&
                  this.currentModuleDetails['channel' + this.currentChannel].channelConfiguration[key].parameter !== 'Protocol') {

              const parameterValue = this.currentModuleDetails['channel' + this.currentChannel].channelConfiguration[key];
              if ( this.searchString === '') {
                this.rtuConfigModuleDetails.push(parameterValue);
              } else if (parameterValue.parameter.toLowerCase().includes( this.searchString.toLowerCase() )) {
                this.rtuConfigModuleDetails.push(parameterValue);
              }
            }

            if ((this.currentModuleDetails['channel' + this.currentChannel].channelConfiguration[key].configClass === configClass.COMMAND ||
                  this.currentModuleDetails['channel' + this.currentChannel].channelConfiguration[key].configClass === configClass.DYNAMIC) &&
                  this.currentModuleDetails['channel' + this.currentChannel].channelConfiguration[key].parameter !== 'Protocol' ) {

              const parameterValue = this.currentModuleDetails['channel' + this.currentChannel].channelConfiguration[key];
              if ( this.searchString === '') {
                this.rtuDynamicModuleDetails.push(parameterValue);
              } else if ( parameterValue.parameter.toLowerCase().includes( this.searchString.toLowerCase() )) {
                this.rtuDynamicModuleDetails.push(parameterValue);
              }
            }

            if (this.currentModuleDetails['channel' + this.currentChannel].channelConfiguration[key].parameter === 'Protocol' ) {
              this.rtuChannelProtocol = [];
              const parameterValue = this.currentModuleDetails['channel' + this.currentChannel].channelConfiguration[key];
              if ( this.searchString === '') {
                this.rtuChannelProtocol.push(parameterValue);
              } else if ( parameterValue.parameter.toLowerCase().includes( this.searchString.toLowerCase() )) {
                this.rtuChannelProtocol.push(parameterValue);
              }
            }
          });

          this.rtuconfiguration.points.forEach(function (value: IPoint, index: number) {

            // get the tank label, module and channel
            let label = '';
            let module = '-1';
            let channel = '-1';
            let tankVisible = '';
            Object.keys(value.pointConfiguration).forEach(key => {
              if ( value.pointConfiguration[ key ].parameter === 'Label' ) {
                label = value.pointConfiguration[ key ].pendingValue;
              }
              if ( value.pointConfiguration[ key ].parameter === 'Module' ) {
                module = value.pointConfiguration[key].pendingValue;
              }
              if ( value.pointConfiguration[ key ].parameter === 'Channel' ) {
                channel = value.pointConfiguration[key].pendingValue;
              }
              if ( value.pointConfiguration[ key ].parameter === 'TankVisible' ) {
                tankVisible = value.pointConfiguration[key].pendingValue;
              }
            });
            // display visible tanks that are assigned to the module
            if ( parseInt( module, 10 ) === currentModule &&  parseInt( channel, 10 ) === currentChannel && tankVisible === '2') {
              if ( searchString === '') {
                rtuTankList.push( label );
              } else if ( label.toLowerCase().includes( searchString.toLowerCase() )) {
                rtuTankList.push( label );
              }
            }
          });
        }
      }
  }

  getAllTanks() {
    this.rtuAllTanks = [];
    const allTanks = this.rtuAllTanks;
    const searchString = this.searchString;
    const currentModule = this.currentModule;
    const currentChannel = this.currentChannel;

    this.rtuconfiguration.points.forEach(function (value: IPoint, index: number) {

      // get the tank label, module and channel
      let label = '';
      let module = '-1';
      let channel = '-1';
      let tankVisible = '';
      Object.keys(value.pointConfiguration).forEach(key => {
        if ( value.pointConfiguration[ key ].parameter === 'Label' ) {
          label = value.pointConfiguration[ key ].pendingValue;
        }
        if ( value.pointConfiguration[ key ].parameter === 'Module' ) {
          module = value.pointConfiguration[key].pendingValue;
        }
        if ( value.pointConfiguration[ key ].parameter === 'Channel' ) {
          channel = value.pointConfiguration[key].pendingValue;
        }
        if ( value.pointConfiguration[ key ].parameter === 'TankVisible' ) {
          tankVisible = value.pointConfiguration[key].pendingValue;
        }
      });
      // display visible tanks that are assigned to the module
      if ( tankVisible === '2') {
        if ( searchString === '') {
          allTanks.push( {
              index: index, name: label, assigned: parseInt( module, 10 ) === currentModule && parseInt( channel, 10 ) === currentChannel
            });
        } else if ( label.toLowerCase().includes( searchString.toLowerCase() )) {
          allTanks.push( {
            index: index, name: label, assigned: parseInt( module, 10 ) === currentModule && parseInt( channel, 10 ) === currentChannel
          });
        }
      }
    });

    this.addTankMode = true;
  }

  assignTanks() {
    const allTanks = this.rtuAllTanks;
    const currentModule = this.currentModule;
    const currentChannel = this.currentChannel;
    const tanks = this.rtuconfiguration.points;
    const tankChanges: IPointData[] = [];

    let moduleKey = '-1';
    let channelKey = '-1';

    this.rtuAllTanks.forEach( function (value: IChassisTankInfo, index: number) {
      if ( value.index <= tanks.length ) {
        const tank = tanks[value.index];

        moduleKey = '-1';
        channelKey = '-1';
        // get the keys in the configuration for module and channel (it could be different numbers for different types of points)
        Object.keys(tank.pointConfiguration).forEach(key => {
          if ( tank.pointConfiguration[ key ].parameter === 'Module' ) {
            moduleKey = key;
          }
          if ( tank.pointConfiguration[ key ].parameter === 'Channel' ) {
            channelKey = key;
          }
        });
        // overwrite the module/channel value for the tanks with the new one and reset the values for the ones that got unassigned
        if ( moduleKey !== '-1' && channelKey !== '-1') {
          if ( value.assigned ) {
            tankChanges.push({ id: value.index, identifier: parseInt( moduleKey, 10 ), value: currentModule.toString() });
            tankChanges.push({ id: value.index, identifier: parseInt( channelKey, 10 ), value: currentChannel.toString() });
          } else {
            // tslint:disable-next-line:max-line-length
            if ( tank.pointConfiguration[moduleKey].pendingValue == currentModule && tank.pointConfiguration[channelKey].pendingValue == currentChannel) {
              tankChanges.push({ id: value.index, identifier: parseInt( moduleKey, 10 ), value: '0' });
              tankChanges.push({ id: value.index, identifier: parseInt( channelKey, 10 ), value: '0' });
            }
          }
        }
      }
    });
    this._rtuConfiguration.setPointData(tankChanges);
    this.addTankMode = false;
  }

  changeChannel(channelNumber) {
    this.currentChannel = channelNumber;
    this._SelectedmodulechannelService.selectedChannel( channelNumber, null );
    this.addTankMode = false;
  }

  onProtocolChange(newProtocol: string) {
    this._rtuConfiguration.updateProtocolParameters(newProtocol, this.currentModule, this.currentChannel);
    this._SelectedmodulechannelService.selectedChannel( this.currentChannel, newProtocol );
    this.addTankMode = false;
  }

  onModuleInstalledChange( newModule: string ) {
    const id = parseInt(newModule, 10);
    this._rtuConfiguration.buildBlankModule( id, this.currentModule );
    const  moduleConfigured = this.rtuconfiguration['module' + this.currentModule].name;
    this._SelectedmodulechannelService.selectedModule( this.currentModule, moduleConfigured);
    this.addTankMode = false;
  }

  VerifyApplytoRTUPrompt(): void {
    this.modalRef = this._modalService.show( this.modalVerifyApplytoRTUAction , this.modalConfig);
  }

  applyToRTU() {
    this._rtuConfiguration.applyDataToRTU(false, this._rtuConfiguration.checkForRTUConfigChanges());
    if (this.modalRef !== undefined) {
      this.modalRef.hide();
    }
  }

  VerifyCancelChangesPrompt(): void {
    // check if any changes have been made and only prompt then checkForRTUConfigChanges
    if (this._rtuConfiguration.checkForRTUConfigChanges()) {
      this.modalRef = this._modalService.show(this.modalVerifyCancelAction, this.modalConfig);
    }
  }

  public areThereNoChangesMade() {
    if ( this._rtuConfiguration.checkForRTUConfigChanges() ) {
      return false;
    } else {
      return true;
    }
  }

  cancelChanges() {
    this._rtuConfiguration.cancelPendingChanges();
    this.modalRef.hide();
    const  moduleConfigured = this.rtuconfiguration['module' + this.currentModule].name;
    let protocol = null;
    if ( this.currentChannel !== 0 ) {
      protocol = this.rtuconfiguration['module' + this.currentModule]['channel' + this.currentChannel].protocol;
    }
    this._SelectedmodulechannelService.selectedModuleChannel( this.currentModule, moduleConfigured, this.currentChannel, protocol);
  }

  onModuleCommandApply( command: IParameter) {
    this._rtuConfiguration.applyCommandToRTU( command );
  }

  public saveRtuConfigToDisk() {
    let configToSave;
    if ( this.rtuconfiguration ) {
      configToSave = JSON.stringify(this.rtuconfiguration);
    } else {
      configToSave = 'no data';
    }

    const data = new Blob([configToSave], {type: 'application/json'});

    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
      window.navigator.msSaveOrOpenBlob(data, 'config.rtuconfig');
    } else {
      saveAs(data, 'config.rtuconfig');
    }
  }

  searchChanged(newSearchValue) {
    this.searchString = newSearchValue;
    this.populateSections();
  }

  public isRTUConnectedandChangesExist() {
    if ( this._rtuConfiguration.connectionStatus !== RTUConnectionStatus.CONNECTED ) {
      return true;
    } else if ( this._rtuConfiguration.checkForRTUConfigChanges() === false ) {
      return true;
    } else {
      return false;
    }
  }

  showConfigWarning (parameterObject:any){
    if (parameterObject.parameter == "Watchdog")
    {
      if ((this.rtuconfiguration['module' + this.currentModule]['channel8'].protocol == "Digital Input") && !(parameterObject.pendingValue == '1' || parameterObject.pendingValue == '0'))
        return 'Channel 8 for this module should be set to Virtual or Digital Output protocol';

      else
      return '';
    }
    else
    {
      return '';
    }
  }
}

import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewEncapsulation  } from '@angular/core';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
// tslint:disable-next-line:max-line-length
import { AvailablemodulesService, IAvailableModule, IParameter, IAvailableConfiguration, IAvailableProtocol, IParameterMap, configClass } from 'src/app/services/availablemodules.service';
// tslint:disable-next-line:max-line-length
import { RtuconfigurationService, IRTUConfiguration } from 'src/app/services/rtuconfiguration.service';
import { SelectedmodulechannelService, ISelectedModuleChannel } from 'src/app/services/selectedmodulechannel.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-chassismoduleview',
  templateUrl: './chassismoduleview.component.html',
  styleUrls: ['./chassismoduleview.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class ChassismoduleviewComponent implements OnInit, OnChanges {
  selectedModuleSubscription : Subscription;
  rtuConfigurationSubscription : Subscription;

  @Input() availableConfiguration: IAvailableConfiguration;
  @Input() rtuconfiguration: IRTUConfiguration;

  currentModule = 0;
  currentChannel = 0;
  filteredAvailableModules:  IAvailableModule[];

  module1Img = 'url( "assets/emptymodule.png")';
  module2Img = 'url( "assets/emptymodule.png")';
  module3Img = 'url( "assets/emptymodule.png")';
  module4Img = 'url( "assets/emptymodule.png")';
  module5Img = 'url( "assets/emptymodule.png")';
  module6Img = 'url( "assets/emptymodule.png")';

   constructor( private _availableModuleService: AvailablemodulesService,
      private _rtuConfiguration: RtuconfigurationService,
      private _selectedModuleChannel: SelectedmodulechannelService
    ) {  }

  ngOnInit() {
    if ( this.availableConfiguration && this.availableConfiguration.modules && Array.isArray(this.availableConfiguration.modules)) {
      this.filteredAvailableModules = this.availableConfiguration.modules.filter(function (element, index) {
        return (element.name !== 'CPU');
      });
    }
    this.selectedModuleSubscription = this._selectedModuleChannel.get().subscribe( data => this.onSelectedModuleChannelChanged( data ) );
  }

  ngOnDestroy(){
    this.selectedModuleSubscription.unsubscribe();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.availableConfiguration && !changes.availableConfiguration.isFirstChange()) {
      if (this.availableConfiguration  ) {
        this.filteredAvailableModules = this.availableConfiguration.modules.filter(function (element, index) {
          return (element.name !== 'CPU');
        });
      }
    }
   }


  onSelectedModuleChannelChanged(selectedModuleChannel: ISelectedModuleChannel) {
    this.currentModule = selectedModuleChannel.selectedModule;
    this.currentChannel = selectedModuleChannel.selectedChannel;

    if(this.rtuconfiguration){
      this.module1Img = 'url( "assets/' + this.rtuconfiguration.module1.img + '")';
      this.module2Img = 'url( "assets/' + this.rtuconfiguration.module2.img + '")';
      this.module3Img = 'url( "assets/' + this.rtuconfiguration.module3.img + '")';
      this.module4Img = 'url( "assets/' + this.rtuconfiguration.module4.img + '")';
      this.module5Img = 'url( "assets/' + this.rtuconfiguration.module5.img + '")';
      this.module6Img = 'url( "assets/' + this.rtuconfiguration.module6.img + '")';
    }
  }

  dropped(event: CdkDragDrop<string[]>) {
    switch (event.container.id) {
      case 'module1':
        this.module1Img = 'url( "assets/' + event.item.data.img + '")';
        this.initializeModule( 'module1', 1, event.item.data);
        this.selectModule( 1 );
        break;
        case 'module2':
        this.module2Img = 'url( "assets/' + event.item.data.img + '")';
        this.initializeModule( 'module2', 2, event.item.data);
        this.selectModule( 2 );
        break;
        case 'module3':
        this.module3Img = 'url( "assets/' + event.item.data.img + '")';
        this.initializeModule( 'module3', 3, event.item.data);
        this.selectModule( 3 );
        break;
        case 'module4':
        this.module4Img = 'url( "assets/' + event.item.data.img + '")';
        this.initializeModule( 'module4', 4, event.item.data);
        this.selectModule( 4 );
        break;
        case 'module5':
        this.module5Img = 'url( "assets/' + event.item.data.img + '")';
        this.initializeModule( 'module5', 5, event.item.data);
        this.selectModule( 5 );
        break;
        case 'module6':
        this.module6Img = 'url( "assets/' + event.item.data.img + '")';
        this.initializeModule( 'module6', 6, event.item.data);
        this.selectModule( 6 );
        break;
      }
      this._rtuConfiguration.update( this.rtuconfiguration );
  }

  initializeModule( moduleNumber: string, moduleId: number, data) {

    const currentModuleConfigParms: IParameterMap = this.rtuconfiguration[moduleNumber].moduleConfiguration;

    Object.keys(currentModuleConfigParms).forEach(key => {
      const parameter = currentModuleConfigParms[key];

      if(parameter.configClass !== configClass.CONFIG){
        return;
      }

      if (this._rtuConfiguration.isParameterValueChanged(parameter)) {
        this._rtuConfiguration.decrementGlobalPendingChanges();
      }
    });

    const currentChannelConfigParams: IParameterMap [] = [];
    currentChannelConfigParams.push(this.rtuconfiguration[moduleNumber].channel1.channelConfiguration);
    currentChannelConfigParams.push(this.rtuconfiguration[moduleNumber].channel2.channelConfiguration);
    currentChannelConfigParams.push(this.rtuconfiguration[moduleNumber].channel3.channelConfiguration);
    currentChannelConfigParams.push(this.rtuconfiguration[moduleNumber].channel4.channelConfiguration);
    currentChannelConfigParams.push(this.rtuconfiguration[moduleNumber].channel5.channelConfiguration);
    currentChannelConfigParams.push(this.rtuconfiguration[moduleNumber].channel6.channelConfiguration);
    currentChannelConfigParams.push(this.rtuconfiguration[moduleNumber].channel7.channelConfiguration);
    currentChannelConfigParams.push(this.rtuconfiguration[moduleNumber].channel8.channelConfiguration);

    let instance = this;
    currentChannelConfigParams.forEach(function(channelParams){
      Object.keys(channelParams).forEach(key => {

        const parameter = channelParams[key];

        if(parameter.configClass !== configClass.CONFIG){
          return;
        }

        if (instance._rtuConfiguration.isParameterValueChanged(parameter)) {
          instance._rtuConfiguration.decrementGlobalPendingChanges();
        }
      });
    });


    // find the virtual channel (we will default the channels to be virtual)
    let virtualChannel = this.availableConfiguration.protocols.find( x => x.name === 'Virtual Chan' );
    if ( !virtualChannel ) {
      virtualChannel = { name: 'Virtual Chan', protocolConfiguration: [], availableDeviceTypes: [] };
    }

    this.rtuconfiguration[moduleNumber].img = data.img;
    this.rtuconfiguration[moduleNumber].id = data.id;
    this.rtuconfiguration[moduleNumber].name = data.name;

    this.rtuconfiguration[moduleNumber].channel1.type = data.channel1.type;
    this.rtuconfiguration[moduleNumber].channel1.top = data.channel1.top;
    this.rtuconfiguration[moduleNumber].channel1.left = data.channel1.left;
    this.rtuconfiguration[moduleNumber].channel1.width = data.channel1.width;
    this.rtuconfiguration[moduleNumber].channel1.height = data.channel1.height;
    this.rtuconfiguration[moduleNumber].channel1.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 1, data.channel1.channelProtocols );


    this.rtuconfiguration[moduleNumber].channel2.type = data.channel2.type;
    this.rtuconfiguration[moduleNumber].channel2.top = data.channel2.top;
    this.rtuconfiguration[moduleNumber].channel2.left = data.channel2.left;
    this.rtuconfiguration[moduleNumber].channel2.width = data.channel2.width;
    this.rtuconfiguration[moduleNumber].channel2.height = data.channel2.height;
    this.rtuconfiguration[moduleNumber].channel2.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 2, data.channel2.channelProtocols );

    this.rtuconfiguration[moduleNumber].channel3.type = data.channel3.type;
    this.rtuconfiguration[moduleNumber].channel3.top = data.channel3.top;
    this.rtuconfiguration[moduleNumber].channel3.left = data.channel3.left;
    this.rtuconfiguration[moduleNumber].channel3.width = data.channel3.width;
    this.rtuconfiguration[moduleNumber].channel3.height = data.channel3.height;
    this.rtuconfiguration[moduleNumber].channel3.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 3, data.channel3.channelProtocols );

    this.rtuconfiguration[moduleNumber].channel4.type = data.channel4.type;
    this.rtuconfiguration[moduleNumber].channel4.top = data.channel4.top;
    this.rtuconfiguration[moduleNumber].channel4.left = data.channel4.left;
    this.rtuconfiguration[moduleNumber].channel4.width = data.channel4.width;
    this.rtuconfiguration[moduleNumber].channel4.height = data.channel4.height;
    this.rtuconfiguration[moduleNumber].channel4.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 4, data.channel4.channelProtocols );

    this.rtuconfiguration[moduleNumber].channel5.type = data.channel5.type;
    this.rtuconfiguration[moduleNumber].channel5.top = data.channel5.top;
    this.rtuconfiguration[moduleNumber].channel5.left = data.channel5.left;
    this.rtuconfiguration[moduleNumber].channel5.width = data.channel5.width;
    this.rtuconfiguration[moduleNumber].channel5.height = data.channel5.height;
    this.rtuconfiguration[moduleNumber].channel5.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 5, data.channel5.channelProtocols );

    this.rtuconfiguration[moduleNumber].channel6.type = data.channel6.type;
    this.rtuconfiguration[moduleNumber].channel6.top = data.channel6.top;
    this.rtuconfiguration[moduleNumber].channel6.left = data.channel6.left;
    this.rtuconfiguration[moduleNumber].channel6.width = data.channel6.width;
    this.rtuconfiguration[moduleNumber].channel6.height = data.channel6.height;
    this.rtuconfiguration[moduleNumber].channel6.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 6, data.channel6.channelProtocols );

    this.rtuconfiguration[moduleNumber].channel7.type = data.channel7.type;
    this.rtuconfiguration[moduleNumber].channel7.top = data.channel7.top;
    this.rtuconfiguration[moduleNumber].channel7.left = data.channel7.left;
    this.rtuconfiguration[moduleNumber].channel7.width = data.channel7.width;
    this.rtuconfiguration[moduleNumber].channel7.height = data.channel7.height;
    this.rtuconfiguration[moduleNumber].channel7.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 7, data.channel7.channelProtocols );

    this.rtuconfiguration[moduleNumber].channel8.type = data.channel8.type;
    this.rtuconfiguration[moduleNumber].channel8.top = data.channel8.top;
    this.rtuconfiguration[moduleNumber].channel8.left = data.channel8.left;
    this.rtuconfiguration[moduleNumber].channel8.width = data.channel8.width;
    this.rtuconfiguration[moduleNumber].channel8.height = data.channel8.height;
    this.rtuconfiguration[moduleNumber].channel8.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 8, data.channel8.channelProtocols );

    const moduleConfigParms: IParameterMap = {};
 

    Object.keys(data.moduleConfiguration).forEach(key => {

      const parameter = data.moduleConfiguration[key];

      let currentValue: any = '';
      let pendingValue: any = '';

      // find the current value for the existing parameter so we can populate the same value
      if ( parameter.configClass === configClass.CONFIG ) {
        // tslint:disable-next-line:max-line-length
        const paramerrIdentifier = Object.keys(currentModuleConfigParms).find(s => currentModuleConfigParms[s].parameter === parameter.parameter);
        if (paramerrIdentifier ) {
          currentValue = currentModuleConfigParms[paramerrIdentifier].value;
          pendingValue = currentModuleConfigParms[paramerrIdentifier].pendingValue
        }
      }

      const moduleConfigParm: IParameter = {
        configClass: parameter.configClass,
        parameter: parameter.parameter,
        description: parameter.description,
        value:  parameter.configClass === configClass.DYNAMIC ? '' : (currentValue !== '' ? currentValue : parameter.value),
        status : parameter.status,
        serverTimeStamp : new Date(),
        pendingValue: parameter.configClass === configClass.DYNAMIC ? '' : (parameter.parameter === 'ModConfigured' ? data.id.toString() : (pendingValue !== '' ? pendingValue : parameter.pendingValue)),
        pendingStatus : parameter.pendingStatus,
        pendingServerTimeStamp : new Date(),
        dataType: parameter.dataType,
        displayFormat: parameter.displayFormat,
        minimumValue: parameter.minimumValue,
        maximumValue: parameter.maximumValue,
        availableCommands: parameter.availableCommands,
        availableDeviceTypeValues: parameter.availableDeviceTypeValues,
        identifier: parameter.opcstartNodeID + ( moduleId - 1),
        opcstartNodeID: parameter.opcstartNodeID,
        tab: parameter.tab,
        section: parameter.section,
        readableStatus: '',
        readableName: '',
        parameterIsVisible: parameter.parameterIsVisible,
        availableCommandsOutputMatches: parameter.availableCommandsOutputMatches,
        variableAlarmNumber: parameter.variableAlarmNumber,
        datatypeLength: parameter.datatypeLength,
      };
      moduleConfigParms[parameter.opcstartNodeID + ( moduleId - 1)] = moduleConfigParm;

      if(moduleConfigParm.configClass !== configClass.CONFIG){
        return;
      }

      if (instance._rtuConfiguration.isParameterValueChanged(moduleConfigParm)) {
        instance._rtuConfiguration.incrementGlobalPendingChanges();
      }
    });

    this.rtuconfiguration[moduleNumber].moduleConfiguration = moduleConfigParms;
   }

  selectModule( moduleId: number ) {
    this.currentModule = moduleId;
    this.currentChannel = 0;
    this._selectedModuleChannel.selectedModule( moduleId, this.rtuconfiguration['module' + moduleId].name );
  }

  selectModuleChannel( moduleId: number, channelId: number, event ) {
    this.currentModule = moduleId;
    this.currentChannel = channelId;
    const  moduleConfigured = this.rtuconfiguration['module' + this.currentModule].name;
    let protocol = null;
    if(this.currentChannel !== 0){
      protocol = this.rtuconfiguration['module' + this.currentModule]['channel' + this.currentChannel].protocol;
    }
    this._selectedModuleChannel.selectedModuleChannel( moduleId, moduleConfigured, channelId, protocol );
    event.stopPropagation();
  }

  selectChannel( channelId: number ) {
    this.currentChannel = channelId;
    let protocol = null;
    if(this.currentChannel !== 0){
      protocol = this.rtuconfiguration['module' + this.currentModule]['channel' + this.currentChannel].protocol;
    }
    this._selectedModuleChannel.selectedChannel( channelId, protocol );
  }


  getAvailableChannelConfiguration( channelInfo: IAvailableProtocol, moduleId: number, channelId: number, protocols : string [] ): IParameterMap {
    const channelConfiguration: IParameterMap = {};

    let protocolAvailableCommands = '';
    protocols.forEach(function(protocol, index){
      protocolAvailableCommands += protocol.trim();
      if(index < protocols.length - 1){
        protocolAvailableCommands += ',';
      }
    });
 
    Object.keys(channelInfo.protocolConfiguration).map(s => channelInfo.protocolConfiguration[s]).forEach(function(parameter: IParameter) {
      const newOpcNode = parameter.opcstartNodeID + (moduleId * 8) + ( channelId - 1);

      const channel1Configuration: IParameter = 
      {
        configClass: parameter.configClass,
        parameter: parameter.parameter,
        description: parameter.description,
        value: parameter.parameter === 'Protocol' ? '1' : parameter.value,
        status: parameter.status,
        serverTimeStamp : new Date(),
        pendingValue: parameter.parameter === 'Protocol' ? '1' : parameter.pendingValue,
        pendingStatus: parameter.pendingStatus,
        pendingServerTimeStamp : new Date(),
        dataType: parameter.dataType,
        displayFormat: parameter.displayFormat,
        minimumValue:parameter.minimumValue,
        maximumValue: parameter.maximumValue,
        availableCommands: parameter.parameter === 'Protocol' ? protocolAvailableCommands : parameter.availableCommands,
        availableDeviceTypeValues: parameter.availableDeviceTypeValues,
        identifier: newOpcNode,
        opcstartNodeID: parameter.opcstartNodeID,
        tab: parameter.tab,
        section: parameter.section,
        readableStatus: '',
        readableName: '',
        parameterIsVisible: parameter.parameterIsVisible,
        availableCommandsOutputMatches: parameter.availableCommandsOutputMatches,
        variableAlarmNumber: parameter.variableAlarmNumber,
        datatypeLength: parameter.datatypeLength,
      };
      channelConfiguration[newOpcNode] = channel1Configuration ;
    });

    return channelConfiguration;
  }


  setModuleChannelCoordinates( moduleId, channelId ) {
    let styles = {};

    // note: coordinates for the channels were taken for an image of size 800 x 205 but we need the coordinates for an image
    //      of size 315 x 74
    if ( this.rtuconfiguration ) {

     const currentModuleDetails = this.rtuconfiguration['module' + moduleId];

      const top = ( 315 * currentModuleDetails['channel' + channelId].top ) / 800;
      const left = ( 74 * currentModuleDetails['channel' + channelId].left) / 205;
      const width = ( 74 * currentModuleDetails['channel' + channelId].width ) / 205;
      const height = (  315 * currentModuleDetails['channel' + channelId].height ) / 800;

      styles = {
        'top': top + 'px',
        'left': left + 'px',
        'width': width + 'px',
        'height': height + 'px'
      };
      if ( currentModuleDetails['channel' + channelId].width === 0 && currentModuleDetails['channel' + channelId].height === 0) {
        styles[ 'border' ] = 'none';
      }
    }
    return styles;
  }

}

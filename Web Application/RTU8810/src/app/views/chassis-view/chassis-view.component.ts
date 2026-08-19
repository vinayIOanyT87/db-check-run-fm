import { Component, OnInit } from '@angular/core';
import { AvailablemodulesService, IAvailableConfiguration, IParameterMap } from 'src/app/services/availablemodules.service';
import { RtuconfigurationService, IRTUConfiguration } from 'src/app/services/rtuconfiguration.service';
import { SelectedmodulechannelService, ISelectedModuleChannel } from 'src/app/services/selectedmodulechannel.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-chassis-view',
  templateUrl: './chassis-view.component.html',
  styleUrls: ['./chassis-view.component.css']
})
export class ChassisViewComponent implements OnInit {
  availableConfiguration: IAvailableConfiguration;
  rtuconfiguration: IRTUConfiguration;
  //selectedModuleChannel: ISelectedModuleChannel = { selectedModule: 0, moduleConfigured: 'CPU', selectedChannel: 0};
  availableModuleSubscription: Subscription;
  rtuConfigurationSubscription: Subscription;
  selectedModuleChannelSubscription: Subscription;

  currentModule: number;
  currentChannel: number;

  constructor( private _availableModuleService: AvailablemodulesService,
    private _rtuConfiguration: RtuconfigurationService,
    private _selectedModuleChannel: SelectedmodulechannelService) {
  }

  ngOnInit() {
    this.availableModuleSubscription = this._availableModuleService.getAll().subscribe( data => this.availableConfiguration = data);
    this.rtuConfigurationSubscription = this._rtuConfiguration.get().subscribe( data => {
      if ( data.RTUConfiguration ) {
        this.rtuconfiguration = data.RTUConfiguration;
      } else {
        this.rtuconfiguration = null;
      }
    });
      this.selectedModuleChannelSubscription = this._selectedModuleChannel.get().subscribe( data => this.onSelectedModuleChannelChanged( data ));
  }

  ngOnDestroy() {
    this._rtuConfiguration.subscribeChassisRealtimeParameters([]);
    this.availableModuleSubscription.unsubscribe();
    this.rtuConfigurationSubscription.unsubscribe();
    this.selectedModuleChannelSubscription.unsubscribe();
  }

  onSelectedModuleChannelChanged(selectedModuleChannel: ISelectedModuleChannel) {
// we need to subscribe to all tanks (module, channel, active flag)
// also subscribe to the module/channel
    const parameterList = [];

    if ( this.rtuconfiguration ) {

      let numberOfTanks = 20;
      const moduleConfiguration = this.rtuconfiguration.module0.moduleConfiguration;
      const numberOfTanksIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfTanks');
      if(numberOfTanksIdentifier){
        numberOfTanks = parseInt(moduleConfiguration[numberOfTanksIdentifier].value);
        parameterList.push(moduleConfiguration[numberOfTanksIdentifier]);
      }
  
      let numberOfTanksProcessed = 0;
  

      this.rtuconfiguration.points.forEach( function( point ) {

        if(point.name === 'Tank'){
          numberOfTanksProcessed++;
          if(numberOfTanksProcessed > numberOfTanks){
            return;
          }
        }
   
        const labelIdentifier = Object.keys(point.pointConfiguration).find(s => point.pointConfiguration[s].parameter === 'Label');
        if ( labelIdentifier ) {
          parameterList.push( point.pointConfiguration[labelIdentifier]);
        }
        const moduleIdentifier = Object.keys(point.pointConfiguration).find(s => point.pointConfiguration[s].parameter === 'Module');
        if ( moduleIdentifier ) {
          parameterList.push( point.pointConfiguration[moduleIdentifier]);
        }
        const channelIdentifier = Object.keys(point.pointConfiguration).find(s => point.pointConfiguration[s].parameter === 'Channel');
        if ( channelIdentifier ) {
          parameterList.push( point.pointConfiguration[channelIdentifier]);
        }
        // tslint:disable-next-line:max-line-length
        const tankVisibleIdentifier = Object.keys(point.pointConfiguration).find(s => point.pointConfiguration[s].parameter === 'TankVisible');
        if ( tankVisibleIdentifier ) {
          parameterList.push( point.pointConfiguration[tankVisibleIdentifier]);
        }

      });

      let chassisParameterList: IParameterMap;
      if ( selectedModuleChannel.selectedChannel === 0) {
        chassisParameterList = this.rtuconfiguration[ 'module' + selectedModuleChannel.selectedModule ].moduleConfiguration ;
      } else {
        // tslint:disable-next-line:max-line-length
        chassisParameterList = this.rtuconfiguration[ 'module' + selectedModuleChannel.selectedModule ]['channel' + selectedModuleChannel.selectedChannel].channelConfiguration;
      }
      if ( chassisParameterList ) {
        Object.keys(chassisParameterList).forEach(key => {
          const parameter = chassisParameterList[key];
          if(parameter.parameterIsVisible){
            parameterList.push(parameter);
          }
        });
      }

      this._rtuConfiguration.subscribeChassisRealtimeParameters( parameterList );
    }

      this.currentModule = selectedModuleChannel.selectedModule;
      this.currentChannel = selectedModuleChannel.selectedChannel;
  }
}

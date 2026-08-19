import { Component, OnInit, OnChanges, Input, SimpleChanges } from '@angular/core';
import { IRTUConfiguration, IRTUInterfaceModule } from 'src/app/services/rtuconfiguration.service';
import { SelectedmodulechannelService, ISelectedModuleChannel } from 'src/app/services/selectedmodulechannel.service';

@Component({
  selector: 'app-moduledetail',
  templateUrl: './moduledetail.component.html',
  styleUrls: ['./moduledetail.component.css']
})
export class ModuledetailComponent implements OnInit, OnChanges {
  @Input() rtuconfiguration: IRTUConfiguration;

  currentModule = 0;
  currentChannel = 0;

  moduleImg = 'url( "assets/cpu.png")'; // default image to tbe the first card, the CPU
  currentModuleDetails: IRTUInterfaceModule;

  constructor(private _SelectedmodulechannelService: SelectedmodulechannelService) { }

  ngOnInit() {
    this._SelectedmodulechannelService.get().subscribe( data => this.onSelectedModuleChannelChanged( data ) );

     if ( this.rtuconfiguration ) {
          this.currentModuleDetails = this.rtuconfiguration[ 'module' + this.currentModule];
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.rtuconfiguration && this.rtuconfiguration.module0 ) {
      if ( !isNaN( this.currentModule ) && !isNaN( this.currentChannel )) {
        this.moduleImg = 'url( "assets/' + this.rtuconfiguration[ 'module' + this.currentModule].img + '")';
        this.currentModuleDetails = this.rtuconfiguration[ 'module' + this.currentModule];
      }
    }
  }

  onSelectedModuleChannelChanged(selectedModuleChannel: ISelectedModuleChannel) {
    this.currentModule = selectedModuleChannel.selectedModule;
    this.currentChannel = selectedModuleChannel.selectedChannel;

    if ( this.rtuconfiguration && this.rtuconfiguration.module0 ) {
      this.moduleImg = 'url( "assets/' + this.rtuconfiguration[ 'module' + this.currentModule].img + '")';
      this.currentModuleDetails = this.rtuconfiguration[ 'module' + this.currentModule];
    }
  }

  changeChannel(channelNumber, event) {
    this.currentChannel = channelNumber;
    let protocol = null;
    if(this.currentChannel !== 0){
      protocol = this.rtuconfiguration['module' + this.currentModule]['channel' + this.currentChannel].protocol;
    }
    this._SelectedmodulechannelService.selectedChannel( channelNumber, protocol );
    event.stopPropagation();
  }

  setChannelCoordinates( channelId ) {
    let styles = {};

    if ( this.currentModuleDetails ) {
      styles = {
        'top': this.currentModuleDetails['channel' + channelId].top + 'px',
        'left': this.currentModuleDetails['channel' + channelId].left + 'px',
        'width': this.currentModuleDetails['channel' + channelId].width + 'px',
        'height': this.currentModuleDetails['channel' + channelId].height + 'px'
      };
      if ( this.currentModuleDetails['channel' + channelId].width == 0 && this.currentModuleDetails['channel' + channelId].height == 0){
        styles[ 'border' ] = 'none';
      }
    }
    return styles;
  }

}

import { Component, OnInit } from '@angular/core';
import { RtuconfigurationService } from 'src/app/services/rtuconfiguration.service';
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-connect-header',
  templateUrl: './connect-header.component.html',
  styleUrls: ['./connect-header.component.css']
})
export class ConnectHeaderComponent implements OnInit {
  rtuConnectonSubscription : Subscription;
  rtuConfigurationSubscription : Subscription;
  ipaddress = '';
  status = RTUConnectionStatus.DISCONNECTED;

  constructor(private _rtuConfiguration: RtuconfigurationService,
    private _RtuconnectionstatusService: RtuconnectionstatusService) {}

  ngOnInit() {
    const self = this;
    

    // subscribe to the rtu connection status service
    this.rtuConnectonSubscription = this._RtuconnectionstatusService.get().subscribe( data => {
      this.status = data;
    });

    this.rtuConfigurationSubscription = this._rtuConfiguration.get().subscribe(data => {
      if ( data.RTUConfiguration && data.RTUConfiguration.module0) {
        self.ipaddress = data.url;
      }
    } );
    
  }

  ngOnDestroy() {
    this.rtuConnectonSubscription.unsubscribe();
    this.rtuConfigurationSubscription.unsubscribe();
  }

  setBackgroundColor( ) {
    let styles = {};

    if ( this.status === RTUConnectionStatus.CONNECTED ) {
      styles = { 'border-color': '#28a745', 'background-color': '#BEE4C7' };
    } else if (this.status === RTUConnectionStatus.CONNECTING ||
              this.status === RTUConnectionStatus.READINGCONFIGURATION ||
      this.status === RTUConnectionStatus.WRITINGCONFIGURATION ||
      this.status === RTUConnectionStatus.WRITINGCOMMAND) {
      styles = { 'border-color': '#ffc107', 'background-color': '#FFE69B' };
    } else {
      styles = { 'border-color': '#dc3545', 'background-color': '#F1AEB4' };
    }

    return styles;
  }
  
}


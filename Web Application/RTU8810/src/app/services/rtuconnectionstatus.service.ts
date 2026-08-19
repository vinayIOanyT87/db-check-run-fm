import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';

export enum RTUConnectionStatus {
  UNDEFINED = 'UNDEFINED',
  DISCONNECTED = 'DISCONNECTED',
  CONNECTED = 'CONNECTED',
  CONNECTING = 'CONNECTING',
  READINGCONFIGURATION = 'READING CONFIGURATION',
  ERRORREADINGCONFIGURATION = 'ERROR READING CONFIG',
  LOSTCONNECTION = 'LOST CONNECTION',
  ERRORCONNECTING = 'ERROR CONNECTING',
  WRITINGCONFIGURATION = 'WRITING CONFIGURATION',
  ERRORWRITINGCONFIGURATION = 'ERROR WRITING CONFIG',
  WRITINGCOMMAND = 'WRITING COMMAND',
  ERRORWRITINGCOMMAND = 'ERROR WRITING COMMAND',
  ERRORREADING = 'ERROR READING'
}


@Injectable({
  providedIn: 'root'
})
export class RtuconnectionstatusService {
  private connectionStatus: RTUConnectionStatus;

  RTUConnectionStatus: Observable<RTUConnectionStatus>;
  private _RTUConnectionStatus: BehaviorSubject<RTUConnectionStatus>;

  constructor() {
    this.connectionStatus = RTUConnectionStatus.UNDEFINED;
    this._RTUConnectionStatus = <BehaviorSubject<RTUConnectionStatus>>new BehaviorSubject({});
    this.RTUConnectionStatus = this._RTUConnectionStatus.asObservable();
  }

  get() {
    return this._RTUConnectionStatus.asObservable();
  }

  updateConnectionStatus(newStatus) {
    if (this.connectionStatus !== newStatus) {
      this.connectionStatus = newStatus;
      this._RTUConnectionStatus.next(this.connectionStatus);
    }
  }
}

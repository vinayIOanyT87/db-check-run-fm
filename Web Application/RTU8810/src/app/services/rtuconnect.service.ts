import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { NotificationService } from './notification.service';
import { environment } from '../../environments/environment';
// tslint:disable-next-line:max-line-length
import { IRTUConfigurationService, IRTUConfiguration, IRTUConfigurationWebService, IRTUCPUConfigurationWebService } from 'src/app/services/rtuconfiguration.service';
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';
import { Observable, BehaviorSubject } from 'rxjs';


export interface IRTUConnectionService {
  RTUConfiguration: IRTUConfiguration;
  ipaddress: string;
  rtuName: string;
  rtuFirmware: string;
}

export enum securityModeEnum {
  none = 'none',
  sign = 'sign',
  signAndEncrypt = 'signAndEncrypt'
}

export enum securityPolicyEnum {
  Basic256Sha256 = 'Basic256Sha256',
  AES128Sha256RsaOaep = 'AES128Sha256RsaOaep',
  AES256Sha256RsaPss = 'AES256Sha256RsaPss',
}

export enum userIdentityEnum {
  anonymous = 'anonymous',
  username = 'username',
  certificate = 'certificate',
}

@Injectable({
  providedIn: 'root'
})
export class RtuconnectService {
  RTUInitialConnection: Observable<IRTUConnectionService>;
  private _RTUInitialConnection: BehaviorSubject<IRTUConnectionService>;
  private baseUrl: string;
  private dataStore: IRTUConnectionService;
  private inError = false;

  constructor(private _http: HttpClient,
    private _notificationService: NotificationService,
    private _RtuconnectionstatusService: RtuconnectionstatusService) {
      this.baseUrl = environment.RTUWebApiPath + 'api/RTUConfiguration';
    this.dataStore = {
      RTUConfiguration: <IRTUConfiguration>{ globalPendingChanges: 0, defaultBlankConfiguration: true },
      ipaddress: '',
      rtuName: 'disconnected',
      rtuFirmware: 'disconnected'
    };

      this._RTUInitialConnection = <BehaviorSubject<IRTUConnectionService>>new BehaviorSubject({});
      this.RTUInitialConnection = this._RTUInitialConnection.asObservable();
  }

  init() {
    this._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.DISCONNECTED );
    this.dataStore.ipaddress = '';
    this.dataStore.rtuName = 'disconnected';
    this.dataStore.rtuFirmware = 'disconnected';
    this.clearConfiguration();
    return this._RTUInitialConnection.asObservable();
  }

  get() {
    return this._RTUInitialConnection.asObservable();
  }

  InitialConnection(connectionString: string, securityMode: securityModeEnum, securityPolicy: securityPolicyEnum, userIdentity: userIdentityEnum, certificateFilename: string, loginId: string, password: string, configSysVer: string) {
    this._RtuconnectionstatusService.updateConnectionStatus(RTUConnectionStatus.CONNECTING);
    this.dataStore.ipaddress = connectionString;
    this._RTUInitialConnection.next(this.dataStore);

    const connectionParms = {
      url: connectionString,
      securityMode: securityMode,
      securityPolicy: securityPolicy,
      userIdentity: userIdentity,
      loginId: loginId,
      loginPassword: password,
      returnPoints: false,
      fileName: configSysVer + '.rtuxml',
      certificateFilename: certificateFilename
    };

    const formData: FormData = new FormData();
    // if (certFile) {
    //   formData.append('certFile', certFile);
    // }

    formData.append('connectionParms', JSON.stringify(connectionParms));

    const httpOptions = {
      headers: new HttpHeaders({
        'Accept': 'multipart/form-data',
      })
    };

    this._http.post<IRTUConfigurationWebService>(`${this.baseUrl}` + '/ConnectToRTU', formData).subscribe(response => {
      if (response.errorMessage && Object.keys(response.errorMessage).length > 0) {
        console.log( response.errorMessage[Object.keys(response.errorMessage)[0]].join(',') );
        this._notificationService.error(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','), 'Cannot connect to RTU');
        this._RtuconnectionstatusService.updateConnectionStatus(RTUConnectionStatus.ERRORCONNECTING);
      } else {
        this._RtuconnectionstatusService.updateConnectionStatus(RTUConnectionStatus.READINGCONFIGURATION);
        this.getRTUCPUConfiguration(connectionParms);
      }

      this.dataStore.RTUConfiguration = response.data;

      this.dataStore.ipaddress = connectionParms.url;
      this._RTUInitialConnection.next(Object.assign({}, this.dataStore));

    }, error => {
      this._RtuconnectionstatusService.updateConnectionStatus(RTUConnectionStatus.ERRORCONNECTING);
      this._notificationService.error(error.message, 'Error retrieving RTU configuration');
      this.dataStore.ipaddress = '';
      this.dataStore.rtuName = 'disconnected';
      this.dataStore.rtuFirmware = 'disconnected';
      this.clearConfiguration();
    });
  }

  getRTUCPUConfiguration(connectionParms) {
    this._http.post<IRTUCPUConfigurationWebService>(`${this.baseUrl}` + '/GetRTUCPUModule', connectionParms ).subscribe(response => {
      if (response.errorMessage && Object.keys(response.errorMessage).length > 0) {
        // tslint:disable-next-line:max-line-length
        this._notificationService.error(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','), 'Error reading CPU Module');
        this._RtuconnectionstatusService.updateConnectionStatus(RTUConnectionStatus.ERRORCONNECTING);
      } else {
        this._RtuconnectionstatusService.updateConnectionStatus(RTUConnectionStatus.CONNECTED);
      }
      this.dataStore.RTUConfiguration.module0 = response.data;
      // doing a copy to force change detect
      this.dataStore = JSON.parse(JSON.stringify( this.dataStore));
      this._RTUInitialConnection.next(this.dataStore);

    }, error => {
      this._RtuconnectionstatusService.updateConnectionStatus(RTUConnectionStatus.ERRORREADINGCONFIGURATION);
      this.inError = true;
      this._notificationService.error(error.message, 'Error retrieving CPU configuration');
      this._RTUInitialConnection.next(this.dataStore);
    });
  }

  updateConnectionStatus( newStatus ) {
    this._RtuconnectionstatusService.updateConnectionStatus(  newStatus );
  }

  updateDisconnectStatus( newStatus ) {
    this._RtuconnectionstatusService.updateConnectionStatus(newStatus);
    this.dataStore.ipaddress = '';
    this.dataStore.rtuName = 'disconnected';
    this.dataStore.rtuFirmware = 'disconnected';
    this.clearConfiguration();
  }

  clearConfiguration() {
    this.dataStore.RTUConfiguration = null;
    this._RTUInitialConnection.next(this.dataStore);
  }
}

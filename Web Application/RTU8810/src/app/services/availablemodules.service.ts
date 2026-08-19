import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { HttpClient, HttpErrorResponse,HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';

export enum configClass { DYNAMIC = 32, CONFIG = 64, CONSTANT = 96, SCRATCH = 128, COMMAND = 160, SYSTEM = 192 }

export enum alarmTypes { 'Undefined' = 0, 'Bitmap' = 1, 'Match' = 2, 'Mismatch' = 3, 'Low Threshold' = 4, 'High Threshold' = 5, 'Char Array' = 6 }

export enum allProtocols { 'Virtual Chan' = 1 ,
                          'RTU Slave'=  2,
                          'Enraf Master' = 3,
                          'Modbus Master' = 4,
                          'Modbus Slave' = 5,
                          'Digital Input' = 6,
                          'Digital Output' = 7,
                          'Ethernet'= 8,
                          "Mark/Space"= 9,
                          "Tankway"= 10 }

export interface IParameterMap {
  [identifier: number] : IParameter;
}

export interface IParameter {
  configClass: configClass;
  parameter: string;
  description: string;
  dataType: string;
  displayFormat: string;
  minimumValue: number;
  maximumValue: number;
  value: string;
  status : number;
  serverTimeStamp : Date;
  pendingValue: string;
  pendingStatus : number;
  pendingServerTimeStamp : Date;
  availableCommands: string;
  availableDeviceTypeValues: string;
  identifier: number;
  opcstartNodeID: number;
  tab: string;
  section: string;
  readableStatus:string;
  readableName: string;
  parameterIsVisible: number;
  availableCommandsOutputMatches: number;
  variableAlarmNumber: string;
  datatypeLength:string;
  //actualparametername:string;

}

export interface IAvailableModule {
  id: number | string;
  name: string;
  img: string;
  moduleConfiguration: IParameterMap;
  channel1: IAvailableChannel;
  channel2: IAvailableChannel;
  channel3: IAvailableChannel;
  channel4: IAvailableChannel;
  channel5: IAvailableChannel;
  channel6: IAvailableChannel;
  channel7: IAvailableChannel;
  channel8: IAvailableChannel;
}

export interface IAvailableProtocol {
  name: string;
  protocolConfiguration: IParameterMap;
  availableDeviceTypes: IDeviceTypeMap;
}

export interface IDeviceTypeMap {
 [identifier: number] : IDeviceType;
}

export interface IDeviceType {
  id: string;
  name: string;
  deviceTypeValue: string;
  availableCommands: string[];
  availableDeviceTypeValues: string[];
}

export interface IAvailableChannel {
  type: number;
  channelProtocol: string[];
  top: number;
  left: number;
  width: number;
  height: number;
}

export interface IAvailablePoints {
  pointConfiguration: IParameter[];
}

export interface IAvailableConfiguration {
  modules:  IAvailableModule[];
  protocols:  IAvailableProtocol[];
  points: IAvailablePoints[];
}

export interface IAvailableConfigurationWebService {
  data:  IAvailableConfiguration;
  errorMessage: any;
  successMessage: any;
}


@Injectable({
  providedIn: 'root'
})

export class AvailablemodulesService {
  availablemodules: Observable<IAvailableConfiguration>;
  private _availablemodules: BehaviorSubject<IAvailableConfiguration>;
  private assetUrl: string;
  private dataStore:  IAvailableConfiguration;

  constructor( private _http: HttpClient) {
    const instance = this;
    instance.assetUrl = environment.RTUWebApiPath + 'api/AvailableModules';

    instance.dataStore = { modules: [], protocols: [], points: [] };

    instance._availablemodules = <BehaviorSubject<IAvailableConfiguration>>new BehaviorSubject({});
    instance.availablemodules = instance._availablemodules.asObservable();

    instance._http.get<IAvailableConfigurationWebService>(instance.assetUrl)
    .subscribe(response => {
      instance.dataStore.modules = response.data.modules;
      instance.dataStore.protocols = response.data.protocols;

      instance._availablemodules.next( instance.dataStore );
      console.log(response);
     }, error => {
      console.log('Could not load Available Configuration.');
      instance._availablemodules.next( instance.dataStore );
    });
  }

  loadXmlConfiguration(fileName:string)
  {
    const instance = this;
    const params = new HttpParams().set( 'filename', fileName);
    instance.assetUrl = environment.RTUWebApiPath + 'api/AvailableModules';

    instance.dataStore = { modules: [], protocols: [], points: [] };

    instance._availablemodules = <BehaviorSubject<IAvailableConfiguration>>new BehaviorSubject({});
    instance.availablemodules = instance._availablemodules.asObservable();

    instance._http.get<IAvailableConfigurationWebService>(instance.assetUrl, { params: params })
    .subscribe(response => {
      instance.dataStore.modules = response.data.modules;
      instance.dataStore.protocols = response.data.protocols;
      instance._availablemodules.next( instance.dataStore );
      console.log(response);
     }, error => {
      console.log('Could not load Available Configuration.');
      instance._availablemodules.next( instance.dataStore );
    });
}

  getAll() {
    return  this._availablemodules.asObservable();
  }
}

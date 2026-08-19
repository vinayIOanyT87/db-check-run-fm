import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpParams, HttpRequest } from '@angular/common/http';
import { Observable, BehaviorSubject, Subscription, config } from 'rxjs';
import { configClass, IParameterMap, IParameter } from './availablemodules.service';
import { environment } from '../../environments/environment';
import { NotificationService } from './notification.service';
import { RtuconnectService, securityModeEnum, securityPolicyEnum, userIdentityEnum} from 'src/app/services/rtuconnect.service';
import { AvailablemodulesService, IAvailableConfiguration, IAvailableProtocol, allProtocols} from 'src/app/services/availablemodules.service';
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';
import { timeout } from 'rxjs/operators';
import { findIndex, timeInterval, ignoreElements } from 'rxjs/operators';
import { ValueConverter } from '@angular/compiler/src/render3/view/template';
import { FormatWidth } from '@angular/common';
// import { instantiateDefaultStyleNormalizer } from '@angular/platform-browser/animations/src/providers';
import { ɵangular_packages_platform_browser_platform_browser_e } from '@angular/platform-browser';
// import { instantiateRootComponent } from '@angular/core/src/render3/instructions';
// import { Parameter } from '@angular/compiler-cli/src/ngtsc/host';
import { ModuledetailComponent } from '../views/chassis-view/moduledetail/moduledetail.component';


export interface IRTUChannel {
  protocol: string;
  type: string;
  channelConfiguration: IParameterMap;
  top: number;
  left: number;
  width: number;
  height: number;
}

export interface IPointData {
  id: number;
  identifier: number;
  value: string;
}

export interface IPoint {
  name: string;
  pointConfiguration: IParameterMap;
}

export interface IRTUInterfaceModule {
  id: number | string;
  name: string;
  img: string;
  moduleConfiguration: IParameterMap;
  channel1: IRTUChannel;
  channel2: IRTUChannel;
  channel3: IRTUChannel;
  channel4: IRTUChannel;
  channel5: IRTUChannel;
  channel6: IRTUChannel;
  channel7: IRTUChannel;
  channel8: IRTUChannel;
}


export interface IRTUCPUModule {
  name: string;
  numberOfTanks: object;
  moduleConfiguration: IParameterMap;
  channel1: IRTUChannel;
  channel2: IRTUChannel;
  channel3: IRTUChannel;
  channel4: IRTUChannel;
  channel5: IRTUChannel;
  channel6: IRTUChannel;
  channel7: IRTUChannel;
  channel8: IRTUChannel;
}

export interface IAlarmNumberingClass {
  pointName: string;
  variableName: string;
  alarmNumber: string;
}

export interface IAlarmNumberTypeMap {
  [identifier: number]: IAlarmNumberingClass;
}

export interface IRTUConfiguration {
  name: string;
  module0: IRTUCPUModule;
  module1: IRTUInterfaceModule;
  module2: IRTUInterfaceModule;
  module3: IRTUInterfaceModule;
  module4: IRTUInterfaceModule;
  module5: IRTUInterfaceModule;
  module6: IRTUInterfaceModule;
  points: IPoint[];
  diagViews: Diagview[];
  globalPendingChanges: number;
  defaultBlankConfiguration:boolean;
  pointAlarmNumberLookupDictionary: IAlarmNumberTypeMap;
  pointRefMapNumberLookupDictionary: IAlarmNumberTypeMap;
}

export interface Diagview {
  id: string;
  parameters: IParameter[];
  filterCollection: IFilterCollection;
}

export interface IFilterCollection {
  dataType: number; //STATIC = 0, NUMERIC = 1, TIMESTAMP = 2, STRING = 3
  filters: IFilter[];
}

export interface IFilter {
  operatorType: string; // IF, AND, OR, NOT
  comparator: string; // GREATERTHAN, GREATERTHANOREQUALTO, EQUALTO, LESSTHANOREQUALTO, LESSTHAN, CONTAINS
  value: string;
  date: Date;
  time: ITime;
}

export interface ITime {
  hour: number;
  minute:number;
  second:number;
}

export interface RtuDataValue {
  value: string;
  status: string;
  timeStamp: string;
  identifer: number;
  dataType: string;
}

export interface IRTUConfigurationService {
  RTUConfiguration: IRTUConfiguration;
  url: string;
}

export interface IRTUConfigurationWebService {
  data: IRTUConfiguration;
  errorMessage: any;
  successMessage: any;
}
export interface IRTUCPUConfigurationWebService {
  data: IRTUCPUModule;
  errorMessage: any;
  successMessage: any;
}

export interface IFileListnWebService {
  data: string[];
  errorMessage: any;
  successMessage: any;
}

export enum modbusType {
  REGISTERMAPS = 'Register Maps',
  FLOATINGPOINTREGISTERS = 'FP Registers',
  INTEGERREGISTERS = 'INT Registers'
}


@Injectable({
  providedIn: 'root'
})
export class RtuconfigurationService {
  RTUConfiguration: Observable<IRTUConfigurationService>;
  private updateInterval = 1000;
  private _RTUConfiguration: BehaviorSubject<IRTUConfigurationService>;
  private rtuConnectionSubscription: Subscription;
  private baseUrl: string;
  private dataStore: IRTUConfigurationService;
  private readInitialConfig = false;
  private inError = false;
  public ActiveDiagnosticView: BehaviorSubject<Diagview>;
  availableConfiguration: IAvailableConfiguration;
  private realtimeParameters: IParameter[] = [];
  public liveDataValues: BehaviorSubject<IParameter[]>;
  public updateTimerId: any;
  // public AvailableXmlFiles: Array<string> = [];
  public AvailableXmlFiles: string[] = [];
  private parametersBeingApplied: IParameter[] = null;
  private parametersBeingAppliedIndex: number;
  public connectionStatus: RTUConnectionStatus = RTUConnectionStatus.DISCONNECTED;
  private version: BehaviorSubject<string>;
  private versionNumber: string;
  private numberOfTanksProcessed: number;
  private numberOfAlarmsProcessed: number;
  private numberOfRegMapProcessed: number;
  private numberOf509CertsProcessed: number;
  private sessionUsername: string;
  private sessionPassword: string;
  private sessionSecurityMode: string;
  private sessionSecurityPolicy: string;
  private sessionUserIdentity: string;
  private sessionCertFileName: string;
  private updateSubscription: Subscription;
  public modbusTypeKeys = [];
  public viableXMLUpgradeVersions: string[] = [];
  public _sortedListOfAvailableXMLFiles: string[] = [];
  public xmlVersion: string;
  public canUpgradeXMLVersion: boolean;
  private currentRTUConfiguration: IRTUConfiguration;
  public upgradeButtonActive: boolean;
  private upgradeConfigurationInProgress: boolean = false;

  // event triggered when there are changes to the rtuconfiguration since the change detect may not fire
  changeDetectionEmitter: EventEmitter<void> = new EventEmitter<void>();

  // constants
  PROTOCOL = 'Protocol';
  MODULECONFIGURED = 'ModConfigured';

  constructor(  private _http: HttpClient,
    private _availableModuleService: AvailablemodulesService,
    private _notificationService: NotificationService,
    private _rtuconnectService: RtuconnectService,
    private _RtuconnectionstatusService: RtuconnectionstatusService, ) {
    this.baseUrl = environment.RTUWebApiPath + 'api/RTUConfiguration';

    this.dataStore = { RTUConfiguration: <IRTUConfiguration> { globalPendingChanges: 0, defaultBlankConfiguration:true}, url: '' };

    this._RTUConfiguration = <BehaviorSubject<IRTUConfigurationService>>new BehaviorSubject({});
    this.RTUConfiguration = this._RTUConfiguration.asObservable();

    this.ActiveDiagnosticView = <BehaviorSubject<Diagview>>new BehaviorSubject({});
    this.liveDataValues = <BehaviorSubject<IParameter[]>>new BehaviorSubject({});
    this.version = <BehaviorSubject<string>>new BehaviorSubject({});
    this.version.next('0.0.0.0');
    this.liveDataValues.next([]);
    this.setActiveDiagnosticView('');

    if (this.dataStore.RTUConfiguration.defaultBlankConfiguration) {
      this.upgradeButtonActive = false;
    } else {
      this.upgradeButtonActive = true;
    }

    // check for available rtuXmls from the server
    const instance = this;
    this._http.get<IFileListnWebService>(`${this.baseUrl}` + '/GetAvailableRtuxmls').subscribe(response => {
      console.log( 'available rtuxml files: ', response.data);
      // this.AvailableXmlFiles = new Array<string>(10);
      for ( let i = 0; i < response.data.length; i++) {
        instance.AvailableXmlFiles.push(response.data[i]);
      }
      this._sortedListOfAvailableXMLFiles = this.sortedListOfAvailableXMLFiles(response.data);
    });

    // Load the veRTUe version number
    this._http.get<IFileListnWebService>(`${this.baseUrl}` + '/GetVersion').subscribe(response => {
      console.log('VeRTUe version: ', response.data);
      instance.versionNumber = response.data.toString();
      instance.version.next(instance.versionNumber);
    });

    // load the configuration from the server
    this._http.get<IRTUConfigurationWebService>(`${this.baseUrl}` + '/GetNewConfiguration').subscribe(response => {
      instance.dataStore.RTUConfiguration = null;
      instance.dataStore.RTUConfiguration = response.data;

      instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.DISCONNECTED );
      instance.dataStore.url = '';
      instance._RTUConfiguration.next( instance.dataStore );
    }, error => {
      _notificationService.error( error.message ? error.message : error.statusText, 'load initial RTUConfiguration' );
      console.log('Could not load initial RTUConfiguration.');
      instance._RTUConfiguration.next( instance.dataStore );
    });
  // get the Available configuration (rtuxml file)
    this.subscribeAvailableModule();

    // subscribe to the rtu connection status service
    this.rtuConnectionSubscription = this._RtuconnectionstatusService.get().subscribe( data => instance.connectionStatus = data);

    this.updateTimerId = setTimeout(() => { this.updateSubscribedParameters(); }, this.updateInterval);

    this.modbusTypeKeys = Object.keys(modbusType);
  }

  subscribeAvailableModule() {
    // get the Available configuration (rtuxml file)
    const instance = this;
    this._availableModuleService.getAll().subscribe( data => instance.availableConfiguration = data);
  }

  onNgDestroy() {
    this.rtuConnectionSubscription.unsubscribe();
  }

  loadXmlConfiguration(fileName: string, upgradeConfigFile: boolean) {
    const params = new HttpParams().set( 'filename', fileName);
    const instance = this;
    this._http.get<IRTUConfigurationWebService>(`${this.baseUrl}` + '/GetXmlConfiguration', { params: params }).subscribe(response => {
      const persistedDiagnosticViews = JSON.parse( JSON.stringify( instance.dataStore.RTUConfiguration.diagViews ));
      instance.dataStore.RTUConfiguration = null;
      instance.dataStore.RTUConfiguration = response.data;
      instance.dataStore.RTUConfiguration.diagViews = persistedDiagnosticViews;
      instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.DISCONNECTED );
      instance.dataStore.url = '';
      instance._RTUConfiguration.next(instance.dataStore);
      instance.setActiveDiagnosticView('');

      // If the user wants to upgrade their config file the result should be true
      if (upgradeConfigFile === true) {
        // call function that is going to do all the updates
        this.upgradeConfigFile();
      }
    }, error => {
      this._notificationService.error( error, 'load initial RTUConfiguration' );
      console.log('Could not load RTUConfiguration.');
      instance._RTUConfiguration.next(instance.dataStore);
    });

    this._availableModuleService.loadXmlConfiguration(fileName);
    // FJM: somehow the subscription is lost and we need to renew it
    this.subscribeAvailableModule();
}

  reset() {
    // load the configuration from the server
    const instance = this;
    this._http.get<IRTUConfigurationWebService>(`${this.baseUrl}` + '/GetNewConfiguration').subscribe(response => {
      instance.dataStore.RTUConfiguration = null;
      instance.dataStore.RTUConfiguration =  response.data;
      instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.DISCONNECTED );
      instance.dataStore.url = '';
      instance._RTUConfiguration.next(instance.dataStore);
      instance.upgradeButtonActive = false;
    }, error => {
      console.log('Could not reset initial RTUConfiguration.');
      instance._RTUConfiguration.next(instance.dataStore);
    });
  }

  get() {
    return this._RTUConfiguration.asObservable();
  }

  getVersion() {
    if (this.version) {
      return this.version.asObservable();
    }
  }

  set(inputConfiguration: string) {
    try {
      this.dataStore.RTUConfiguration = JSON.parse(inputConfiguration);
      this.commitPendingChanges();
      this._RTUConfiguration.next(Object.assign({}, this.dataStore));

      this.xmlVersion = this.evaluateXMLVersion(this.dataStore.RTUConfiguration);
      this.viableXMLUpgradeVersions = this.evaluateViableXMLUpgradeVersions(this.xmlVersion, this._sortedListOfAvailableXMLFiles);

      return true;
    } catch (error) {
      console.log(error);
      return false;
    }
  }

  update( RTUConfiguration: IRTUConfiguration ) {
    this.dataStore.RTUConfiguration = RTUConfiguration;
    this._RTUConfiguration.next(this.dataStore);
  }

  connectToRTU(connectionString: string, securityMode: securityModeEnum, securityPolicy: securityPolicyEnum, userIdentity: userIdentityEnum, certificateFilename: string, loginId: string, password: string, configSysVer: string) {
    const instance = this;
    this.readInitialConfig = true;
    this._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.CONNECTING );
    this.dataStore.url = connectionString;
    this._RTUConfiguration.next(this.dataStore);

    const connectionParms = {
      url: connectionString,
      securityMode: securityMode,
      securityPolicy: securityPolicy,
      userIdentity: userIdentity,
      loginId: loginId,
      loginPassword: password,
      returnPoints: true,
      fileName: configSysVer,
      certificateFilename: certificateFilename
    };

    const formData: FormData = new FormData();
    // if (certFile) {
    //   formData.append('certFile', certFile, certFile.name);
    // }

    formData.append('connectionParms', JSON.stringify(connectionParms));

    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    };

    instance._RtuconnectionstatusService.updateConnectionStatus(RTUConnectionStatus.READINGCONFIGURATION);

    this._http.post<IRTUConfigurationWebService>(`${this.baseUrl}` + '/ConnectToRTU', formData).subscribe(response => {
       if ( response.errorMessage && Object.keys(response.errorMessage).length > 0) {
        console.log( response.errorMessage[Object.keys(response.errorMessage)[0]].join(',') );

        instance._notificationService.error( response.errorMessage[Object.keys(response.errorMessage)[0]].join(','), 'Cannot connect to RTU' );
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORCONNECTING );
      }

      const persistedDiagnosticViews = JSON.parse(JSON.stringify(instance.dataStore.RTUConfiguration.diagViews));
      instance.dataStore.RTUConfiguration = response.data;
      instance.dataStore.RTUConfiguration.diagViews = persistedDiagnosticViews;
      instance.dataStore.url = connectionParms.url;
      instance._RTUConfiguration.next(instance.dataStore);
      instance.setActiveDiagnosticView('');

      instance.getRTUCPUConfiguration( connectionParms );

     }, error => {
      this._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORCONNECTING );
      console.log('Could not load RTUConfiguration.');
      instance._notificationService.error( error.message, 'Error retrieving RTU configuration' );
      instance.dataStore.url = '';
      instance._RTUConfiguration.next( instance.dataStore );
    });
  }

  getRTUCPUConfiguration( connectionParms ) {
    const instance = this;
    this._http.post<IRTUCPUConfigurationWebService>(`${this.baseUrl}` + '/GetRTUCPUModule', connectionParms ).subscribe(response => {
      if ( response.errorMessage && Object.keys(response.errorMessage).length > 0) {
        console.log( response.errorMessage[Object.keys(response.errorMessage)[0]].join(',') );
        // tslint:disable-next-line:max-line-length
        instance._notificationService.error( response.errorMessage[Object.keys(response.errorMessage)[0]].join(','), 'Error reading CPU Module' );
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );
        return;
      }

      let dataWithHexValues = this.convertLHEXValues(response.data);;
      instance.dataStore.RTUConfiguration.module0 = dataWithHexValues;


      if ( instance.readInitialConfig = true ) {
        instance.getRTUInterfaceConfiguration( connectionParms, 1 );
      }
     }, error => {
      this._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );
      instance.inError = true;
      instance._notificationService.error( error.message, 'Error retrieving CPU configuration' );
      instance.dataStore.url = '';
      instance._RTUConfiguration.next( instance.dataStore );
      instance.readInitialConfig = false;
    });
  }

  getRTUInterfaceConfiguration( connectionParms, moduleId ) {
    let instance = this;
    const parms = { 'connectionParms': connectionParms, 'moduleId': moduleId };
    this._http.post<IRTUCPUConfigurationWebService>(`${this.baseUrl}` + '/GetRTUInterfaceModule', parms ).subscribe(response => {
      if ( response.errorMessage && Object.keys(response.errorMessage).length > 0) {
        console.log( response.errorMessage[Object.keys(response.errorMessage)[0]].join(',') );
        instance._notificationService.error( response.errorMessage[Object.keys(response.errorMessage)[0]].join(','),
            'Error reading Module ' + moduleId );
            instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );
            instance.inError = true;
            return;
      }

      if ( instance.readInitialConfig === false ) {
        if ( instance.inError === true ) {
          instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );

        } else {
          instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.CONNECTED );
        }
      } else if ( moduleId === 6 ) {
        if ( instance.inError === true ) {
          instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );

        } else {
          instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.CONNECTED );

        }
        instance.inError = false;
        instance.readInitialConfig = false;
      } else if (moduleId !== 6 ) {
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.READINGCONFIGURATION );
      } else {
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.READINGCONFIGURATION );

      }


      let dataWithHexValues = this.convertLHEXValues(response.data);
      instance.dataStore.RTUConfiguration[ 'module' + moduleId] = dataWithHexValues;
      console.log( 'read module ' + moduleId);

      if (instance.readInitialConfig = true && moduleId < 6) {
        instance.getRTUInterfaceConfiguration(connectionParms, (moduleId + 1));
      } else if (instance.dataStore.RTUConfiguration.points.length > 0) {
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.READINGCONFIGURATION );
        instance.numberOfTanksProcessed = 0;
        instance.numberOfAlarmsProcessed = 0;
        instance.numberOf509CertsProcessed = 0;
        let pointId = 0;
        pointId = instance.getPointToRead(pointId);
        instance.getRTUPointConfiguration(connectionParms, pointId);
      } else {
        // doing a copy to force change detect
        instance.dataStore = JSON.parse( JSON.stringify( instance.dataStore ));
        instance._RTUConfiguration.next( instance.dataStore );
      }

    }, error => {
      instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );
      instance.inError = true;
      console.log('Could not load Interface Module' + moduleId + ' Configuration from RTU.');
      instance._notificationService.error( error.message, 'Error retrieving Interface module configuration' );
      instance.dataStore.url = '';
      instance._RTUConfiguration.next( instance.dataStore );
    });
  }


  getRTUChassisConfiguration(connectionParams){
    const instance = this;
    const parameterList = [];
    const moduleConfiguration = this.dataStore.RTUConfiguration.module0.moduleConfiguration;

    const numberOfTanksIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfTanks');
    if(numberOfTanksIdentifier){
      parameterList.push(moduleConfiguration[numberOfTanksIdentifier]);
    }

    const numberOfAlarmsIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfAlarms');
    if(numberOfAlarmsIdentifier){
      parameterList.push(moduleConfiguration[numberOfAlarmsIdentifier])
    }

    const numberOfRegMapIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfRegMap');
    if(numberOfRegMapIdentifier){
      parameterList.push(moduleConfiguration[numberOfRegMapIdentifier]);
    }


    parameterList.push(Object.values(this.dataStore.RTUConfiguration.module0.channel1.channelConfiguration).find(s => s.parameter === this.PROTOCOL));
    parameterList.push(Object.values(this.dataStore.RTUConfiguration.module0.channel2.channelConfiguration).find(s => s.parameter === this.PROTOCOL));
    parameterList.push(Object.values(this.dataStore.RTUConfiguration.module0.channel3.channelConfiguration).find(s => s.parameter === this.PROTOCOL));
    parameterList.push(Object.values(this.dataStore.RTUConfiguration.module0.channel4.channelConfiguration).find(s => s.parameter === this.PROTOCOL));
    parameterList.push(Object.values(this.dataStore.RTUConfiguration.module0.channel5.channelConfiguration).find(s => s.parameter === this.PROTOCOL));
    parameterList.push(Object.values(this.dataStore.RTUConfiguration.module0.channel6.channelConfiguration).find(s => s.parameter === this.PROTOCOL));
    parameterList.push(Object.values(this.dataStore.RTUConfiguration.module0.channel7.channelConfiguration).find(s => s.parameter === this.PROTOCOL));
    parameterList.push(Object.values(this.dataStore.RTUConfiguration.module0.channel8.channelConfiguration).find(s => s.parameter === this.PROTOCOL));

    const modules : IRTUInterfaceModule [] = [];
    modules.push(this.dataStore.RTUConfiguration.module1);
    modules.push(this.dataStore.RTUConfiguration.module2);
    modules.push(this.dataStore.RTUConfiguration.module3);
    modules.push(this.dataStore.RTUConfiguration.module4);
    modules.push(this.dataStore.RTUConfiguration.module5);
    modules.push(this.dataStore.RTUConfiguration.module6);

    modules.forEach(function (module) {

      parameterList.push(Object.values(module.moduleConfiguration).find(s => s.parameter === instance.MODULECONFIGURED));

      const channels : IRTUChannel [] = [];
      channels.push(module.channel1);
      channels.push(module.channel2);
      channels.push(module.channel3);
      channels.push(module.channel4);
      channels.push(module.channel5);
      channels.push(module.channel6);
      channels.push(module.channel7);
      channels.push(module.channel8);

      channels.forEach( function( channel){
        parameterList.push(Object.values(channel.channelConfiguration).find(s => s.parameter === instance.PROTOCOL));
      });

    });

    const identifierList = [];
    parameterList.forEach(function(parameter){
      identifierList.push(parameter.identifier);
    });


    this._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.READINGCONFIGURATION );

    const parms = { 'connectionParms': connectionParams, 'identifierList': identifierList };
    this._http.post<IRTUCPUConfigurationWebService>(`${this.baseUrl}` + '/GetRtuData', parms).subscribe(response => {
      if (response.errorMessage && Object.keys(response.errorMessage).length > 0) {
        console.log(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','));
        instance._notificationService.error(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','),
          'Error reading Chassis Parameters');

        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );

        instance.inError = true;
        instance.parametersBeingApplied = null;
        return;
      }

      parameterList.forEach(function (parameter, index) {
        let value = response.data[index].value;
        if ((typeof parameter.value === 'undefined' && typeof value !== 'undefined')
          || (typeof parameter.value !== 'undefined' && typeof value === 'undefined')
          || parameter.value !== value
          || parameter.status !== response.data[index].status) {
          parameter.value = value;
          parameter.status = response.data[index].status;
          parameter.serverTimeStamp = response.data[index].serverTimeStamp;
          instance.incrementGlobalPendingChanges();
        }
      });

      this._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.WRITINGCONFIGURATION );

      this.writeDataToRtu(connectionParams);

    }, error => {
      instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );
      this.inError = true;
      instance.parametersBeingApplied = null;
      console.log('Could not load chassis configuration from RTU.');
      instance._notificationService.error(error.message, 'Error retrieving chassis configuration');
      instance.dataStore.url = '';
      instance._RTUConfiguration.next(instance.dataStore);
    });
  }

  getPointToRead(pointId) {
    const instance = this;
    let point = null;

    let numberOfTanks = 20;
    const moduleConfiguration = this.dataStore.RTUConfiguration.module0.moduleConfiguration;
    const numberOfTanksIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfTanks');
    if(numberOfTanksIdentifier){
      numberOfTanks = parseInt(moduleConfiguration[numberOfTanksIdentifier].value);
    }

    let numberOfAlarms = 250;
    const numberOfAlarmsIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfAlarms');
    if(numberOfAlarmsIdentifier){
      numberOfAlarms = parseInt(moduleConfiguration[numberOfAlarmsIdentifier].value);
    }

    let numberOfRegMap = 800;
    const numberOfRegMapIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfRegMap');
    if(numberOfRegMapIdentifier){
      numberOfRegMap = parseInt(moduleConfiguration[numberOfRegMapIdentifier].value);
    }

    let numberOf509Certs = 20;


    while(pointId < instance.dataStore.RTUConfiguration.points.length){
       point = this.dataStore.RTUConfiguration.points[pointId];

      if(point.name === 'Tank'){

        //Advance past the rest of the tank points
        if(instance.numberOfTanksProcessed > numberOfTanks){
          while(pointId < instance.dataStore.RTUConfiguration.points.length
          && instance.dataStore.RTUConfiguration.points[pointId].name === 'Tank'){
            pointId++;
           }
           continue;
          }
        else {
          instance.numberOfTanksProcessed++;
          break;
        }
      }

      if(point.name === ' Alarms '){

        //Advance past the rest of the alarm points
        if(instance.numberOfAlarmsProcessed > numberOfAlarms){
          while(pointId < instance.dataStore.RTUConfiguration.points.length
          && instance.dataStore.RTUConfiguration.points[pointId].name === ' Alarms '){
            pointId++;
          }
          continue;
        }
        else {
          instance.numberOfAlarmsProcessed++;
          break;
        }
      }

      if(point.name === ' Register Map '){
        //Advance past the rest of the register map points
        if(instance.numberOfRegMapProcessed > numberOfRegMap){
          while(pointId < instance.dataStore.RTUConfiguration.points.length
          && instance.dataStore.RTUConfiguration.points[pointId].name === ' Register Map '){
            pointId++;
          }
          continue;
        }
        else {
          instance.numberOfRegMapProcessed++;
          break;
        }
      }

      if(point.name === ' X.509 Certificate '){
        //Advance past the rest of the x 509 certificate points
        if(instance.numberOf509CertsProcessed > numberOf509Certs){
          while(pointId < instance.dataStore.RTUConfiguration.points.length
          && instance.dataStore.RTUConfiguration.points[pointId].name === ' X.509 Certificate '){
            pointId++;
          }
          continue;
        }
        else {
          instance.numberOf509CertsProcessed++;
          break;
        }
      }

      else {
        break;
      }
    }


    return pointId;
  }


  getRTUPointConfiguration(connectionParms, pointId) {
    const instance = this;
    const identifierList = [];
    const pointsRead = [];

    pointsRead.push(pointId);
    let point = this.dataStore.RTUConfiguration.points[pointId];
    Object.keys(point.pointConfiguration).map(s => point.pointConfiguration[s]).forEach(function(parameter) {
      identifierList.push(parameter.identifier);
    });


     while(instance.readInitialConfig = true && identifierList.length < 1024 && pointId < this.dataStore.RTUConfiguration.points.length-1) {
      pointId = this.getPointToRead(pointId + 1);
      if(pointId < this.dataStore.RTUConfiguration.points.length) {
        pointsRead.push(pointId);
        point = this.dataStore.RTUConfiguration.points[pointId];
        Object.keys(point.pointConfiguration).map(s => point.pointConfiguration[s]).forEach(function(parameter) {
          identifierList.push(parameter.identifier);
        });
      }
    }

    let pointsReadString = '';
    pointsRead.forEach(function(pointId){
      if(pointsReadString === ''){
        pointsReadString = pointId;
      }
      else{
        pointsReadString = pointsReadString + ', ' + pointId;
      }
    });

    console.log('reading point(s) ' + pointsReadString);

    const parms = { 'connectionParms': connectionParms, 'identifierList': identifierList };
    this._http.post<IRTUCPUConfigurationWebService>(`${this.baseUrl}` + '/GetRtuData', parms).subscribe(response => {
      if (response.errorMessage && Object.keys(response.errorMessage).length > 0) {
        console.log(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','));
        instance._notificationService.error(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','),
          'Error reading Point Parameters');

        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );

        instance.inError = true;
        return;
      }

      if (instance.readInitialConfig === false) {
        if (instance.inError === true) {
          instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );
        } else {
          instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.CONNECTED );
        }
      } else if (pointId === instance.dataStore.RTUConfiguration.points.length - 1) {
        if (this.inError === true) {
          instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );

        } else {
          instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.CONNECTED );
        }
        instance.inError = false;
        instance.readInitialConfig = false;
      } else if (pointId !== instance.dataStore.RTUConfiguration.points.length ) {
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.READINGCONFIGURATION );
      } else {
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.READINGCONFIGURATION );
      }

      // tslint:disable-next-line:max-line-length
      let responsePointOffset = 0;
      pointsRead.forEach(function(pointId){
        let maxParameterIndex = 0;
        Object.keys(instance.dataStore.RTUConfiguration.points[pointId].pointConfiguration).map(s => instance.dataStore.RTUConfiguration.points[pointId].pointConfiguration[s]).forEach(function(parameter, index){
          maxParameterIndex = index;
          let value = response.data[index + responsePointOffset].value;
          if (parameter.displayFormat == "LHEX")
          {
            if (parameter.parameter == "Mask")
            {
              let alarmTypeKey = Object.keys(instance.dataStore.RTUConfiguration.points[pointId].pointConfiguration).find(key => instance.dataStore.RTUConfiguration.points[pointId].pointConfiguration[key].parameter === 'Type');
              if (instance.dataStore.RTUConfiguration.points[pointId].pointConfiguration[alarmTypeKey].value == "2" ||instance.dataStore.RTUConfiguration.points[pointId].pointConfiguration[alarmTypeKey].value == "3" )
              {
                parameter.displayFormat = "";
                parameter.value = value;
                parameter.pendingValue = value;
              }
            }
            else{
              parameter.value = parseInt(value,10).toString(16).toUpperCase();
              parameter.pendingValue = parseInt(value,10).toString(16).toUpperCase();
            }
          }
          else
          {
            parameter.value = value;
            parameter.pendingValue = value;
          }

          parameter.status = response.data[index + responsePointOffset].status;
          parameter.serverTimeStamp = response.data[index + responsePointOffset].serverTimeStamp;

          parameter.pendingStatus = response.data[index + responsePointOffset].status;
          parameter.pendingServerTimeStamp = response.data[index + responsePointOffset].serverTimeStamp;
        });

        responsePointOffset += maxParameterIndex + 1;
      });

      pointId = this.getPointToRead(pointId + 1);

      if (instance.readInitialConfig = true && pointId < instance.dataStore.RTUConfiguration.points.length - 1 ) {
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.READINGCONFIGURATION );
        instance.getRTUPointConfiguration(connectionParms, pointId);
      }
      else {
        // doing a copy to force change detect
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.CONNECTED );
        instance.dataStore = JSON.parse(JSON.stringify(instance.dataStore));
        instance._RTUConfiguration.next(instance.dataStore);
      }
    }, error => {
      instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADINGCONFIGURATION );
      this.inError = true;
      console.log('Could not load point configuration from RTU.');
      instance._notificationService.error(error.message, 'Error retrieving point configuration');
      instance.dataStore.url = '';
      instance._RTUConfiguration.next(instance.dataStore);
    });
  }

  getViews(): Diagview[] {
    return this.dataStore.RTUConfiguration.diagViews;
    // tap(_ => this.log('fetched heroes')),
    // catchError(this.handleError('getHeroes', []))
    // );
  }

  addView(diagview: Diagview) {
    this.dataStore.RTUConfiguration.diagViews.push(diagview);
    // tslint:disable-next-line:max-line-length
    this.ActiveDiagnosticView.next(this.dataStore.RTUConfiguration.diagViews[this.dataStore.RTUConfiguration.diagViews.findIndex(view => view.id === diagview.id)]);
  }


  delView(id: string) {
    this.dataStore.RTUConfiguration.diagViews.splice(this.dataStore.RTUConfiguration.diagViews.findIndex(view => view.id === id), 1);
    this.ActiveDiagnosticView.next({id:'No view selected', parameters:[], filterCollection: {dataType:0, filters:[]}});
  }

  setActiveDiagnosticView(id: string) {
    if (id !== '') {
      const oldView = this.ActiveDiagnosticView.value;
      let newView: Diagview = this.dataStore.RTUConfiguration.diagViews[this.dataStore.RTUConfiguration.diagViews.findIndex(view => view.id === id)];
      this.ActiveDiagnosticView.next(newView);
      this.unsubscribeRealtimeParameters(oldView.parameters);
      this.subscribeRealtimeParameters(newView.parameters);
    } else {
      this.ActiveDiagnosticView.next({id: 'No view selected', parameters: [], filterCollection: {dataType:0, filters:[]}});
    }
  }

  retrieveDiagnosticView() {
    return this.ActiveDiagnosticView.asObservable();
  }

  subscribeRealtimeParameters(Parameters: IParameter[]) {
    const currentConfig = this.dataStore.RTUConfiguration;
    const tempParameters = this.realtimeParameters;
    Parameters.forEach( function (identifier) {
      if (tempParameters.indexOf(identifier) === -1) {
        tempParameters.push(identifier);
      }
      //checking if a parameter in the diagnostic view is inaccessible
      if (identifier.readableName!= undefined && identifier.readableName!= '')
      {
        let locationArray = identifier.readableName.split('.');
        if (locationArray[1] == 'moduleConfiguration')
        {
          if (currentConfig[locationArray[0]][locationArray[1]][identifier.identifier] == undefined)
          {
            identifier.readableStatus = "Not available";
          }
        }
        else
        {
          if (currentConfig[locationArray[0]][locationArray[1]].channelConfiguration[identifier.identifier] == undefined ||
            currentConfig[locationArray[0]].name == "Empty"  )
          {
            identifier.readableStatus = "Not available";
          }
        }
      }
    });

    // when waiting for update
    if (this.updateTimerId) {
      clearTimeout(this.updateTimerId);
      this.updateTimerId = setTimeout(() => { this.updateSubscribedParameters(); }, this.updateInterval);
    }

    if(this.updateSubscription){
      //this.updateSubscription.unsubscribe();
      this.updateSubscription = null;
    }

    this.realtimeParameters = tempParameters;
  }

  unsubscribeRealtimeParameters(Parameters: IParameter[]) {
    const tempParameters = this.realtimeParameters;
    Parameters.forEach( function (identifier) {
      const index = tempParameters.indexOf(identifier);
      if (index > -1) {
        tempParameters.splice(index, 1);
      }
    });

    // when waiting for update
    if (this.updateTimerId) {
      clearTimeout(this.updateTimerId);
      this.updateTimerId = setTimeout(() => { this.updateSubscribedParameters(); }, this.updateInterval);
    }

    if(this.updateSubscription){
      //this.updateSubscription.unsubscribe();
      this.updateSubscription = null;
    }


    this.realtimeParameters = tempParameters;
  }

  // parameters for the chassis view (we want to remove any previous list and update instantly instead of waiting 5 seconds)
  subscribeChassisRealtimeParameters(Parameters: IParameter[]) {
    const tempParameters = [];
    Parameters.forEach( function (identifier) {
      if (tempParameters.indexOf(identifier) === -1) {
        tempParameters.push(identifier);
      }
    });

    // when waiting for update
    if (this.updateTimerId) {
      clearTimeout(this.updateTimerId);
      this.updateTimerId = setTimeout(() => { this.updateSubscribedParameters(); }, this.updateInterval);
    }


    if(this.updateSubscription){
      this.updateSubscription = null;
    }

    this.realtimeParameters = tempParameters;
  }


  updateSubscribedParameters() {
    this.updateTimerId = null;

    if (this.realtimeParameters.length === 0) {
      console.log('updateSubscribedParameters has no identifiers to update.');
      this.updateTimerId = setTimeout(() => { this.updateSubscribedParameters(); }, this.updateInterval);
      return;
    }

    if(this.connectionStatus !== RTUConnectionStatus.CONNECTED
    && this.connectionStatus !== RTUConnectionStatus.ERRORREADING) {
      console.log('updateSubscribedParameters has identifiers ready to update, but the RTU is in status: ' + this.connectionStatus);
      this.updateTimerId = setTimeout(() => { this.updateSubscribedParameters(); }, this.updateInterval);
      return;
    }

    let changesAny = false;
    let changesManagement = false;

    let identifiersString = '';
    const connectionParms = {
      url: this.dataStore.url,
      loginId: this.sessionUsername,
      loginPassword: this.sessionPassword,
      securityMode: this.sessionSecurityMode,
      securityPolicy: this.sessionSecurityPolicy,
      userIdentity: this.sessionUserIdentity,
      certificateFilename: this.sessionCertFileName
    };
    const identifierList = [];
    this.realtimeParameters.forEach(function (parameter) {
      identifierList.push(parameter.identifier);
    });

    identifiersString = identifierList.toString();

    const parms = { 'connectionParms': connectionParms, 'identifierList': identifierList };
    console.log('updatesubscribedparameters calling /GetRtuData with ' + identifierList.length + ' identifiers');
    const instance = this;
    this.updateSubscription = this._http.post<IRTUCPUConfigurationWebService>(`${this.baseUrl}` + '/GetRtuData', parms,).pipe(timeout(2500)).subscribe(response => {
      instance.updateSubscription = null;
      if (response.errorMessage && Object.keys(response.errorMessage).length > 0) {
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADING );
        console.log(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','));
        instance._notificationService.error(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','), '', true);
        instance.updateTimerId = setTimeout(() => { this.updateSubscribedParameters(); }, instance.updateInterval);
        return;
      }

      instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.CONNECTED );

      if((this.connectionStatus === RTUConnectionStatus.CONNECTED
      || this.connectionStatus === RTUConnectionStatus.ERRORREADING)
      && response.data && instance.realtimeParameters.length == response.data["length"]) {
        for (let i = 0; i < instance.realtimeParameters.length; i++) {

          let parameter = instance.realtimeParameters[i];

          if(parameter.identifier != response.data[i].identifier)
            break;

          let value = response.data[i].value;

          if (parameter.displayFormat == "LHEX")
          {
            value = parseInt(value,10).toString(16).toUpperCase();
          }


          if ((typeof parameter.value === 'undefined' && typeof value !== 'undefined')
          || (typeof parameter.value !== 'undefined' && typeof value === 'undefined')
          || parameter.value !== value
          || parameter.status !== response.data[i].status) {

            // when pendingValue equal value update both pending and current with data read from RTU
            let match = false;

            const codeBits = (response.data[i].status / 32767) * 32767;

            // when pending value equal current value and the nodeId is not BadNodeIdInvalid and BadNodIdUnKnown then update the pendingValue as well as value
            if (!instance.isParameterValueChanged(parameter)){
              if(codeBits !== 0x80340000
              && codeBits !== 0x80350000
              && parameter.parameter !== instance.PROTOCOL
              && parameter.parameter !== instance.MODULECONFIGURED) {
                parameter.pendingValue = value;
                parameter.pendingServerTimeStamp = response.data[i].timeStamp;
                parameter.pendingStatus = response.data[i].status;
                parameter.readableStatus = instance.getStatusCode(response.data[i].status);
              }
              match = true;
            }

            if (codeBits !== 0x80340000) // don't populate the value when the status is BadNodeIdUnknown or it won't update when we are getting good values either
            {
            parameter.value = value;
            parameter.serverTimeStamp = response.data[i].timeStamp;
            parameter.status = response.data[i].status;
            }

            changesAny = true;

            if(parameter.configClass === configClass.CONFIG){
              changesManagement = true;

              // when updated value matched pendingValue increment globalPendingChanges
              if(match
              && instance.isParameterValueChanged(parameter)) {
                instance.incrementGlobalPendingChanges();
              }
              // when updated value didn't match pendingValue and now does and wasn't invalid
              else if(!match
              && !instance.isParameterValueChanged(parameter)){
                 instance.decrementGlobalPendingChanges();
              }
            }
            else if(parameter.parameter === 'Output'
            || parameter.parameter === 'CmdStatus'){
              changesManagement = true;
            }

            // Break if a change to certain parameters which signal a need to change the parameters being read
            if(parameter.parameter === 'NumberOfTanks'
            || parameter.parameter === 'NumberOfAlarms'
            || parameter.parameter === 'NumberOfRegMap'){
              break;
            }
          }
        }

        if (changesManagement) {
          instance._RTUConfiguration.next(instance.dataStore);
         }

        if(changesAny){
          instance.liveDataValues.next(instance.realtimeParameters);
          this.changeDetectionEmitter.emit();
        }

        instance.updateTimerId = setTimeout(() => { this.updateSubscribedParameters(); }, instance.updateInterval);
      }
      else{
        instance.updateTimerId = setTimeout(() => { this.updateSubscribedParameters(); }, instance.updateInterval);
      }
    },
    error => {
      instance.updateSubscription = null;
      instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORREADING );
      console.log('Error : updateSubscribedParameters.' + error.message);
      instance.updateTimerId = setTimeout(() => { this.updateSubscribedParameters(); }, instance.updateInterval);
    });
  }

  setPointData(pointData: IPointData []) {
    const instance = this;
    pointData.forEach(function (element) {
      const parameter = instance.dataStore.RTUConfiguration.points[element.id].pointConfiguration[element.identifier];

      // Test for a change
      if ((typeof parameter.pendingValue === 'undefined' && typeof element.value !== 'undefined')
      || (typeof parameter.pendingValue !== 'undefined' && typeof element.value === 'undefined')
      || (parameter.pendingValue !== element.value)) {

        let match = true

        if (instance.isParameterValueChanged(parameter)) {
          match = false;
        }

        parameter.pendingValue = element.value;
        parameter.pendingStatus = 0;
        parameter.pendingServerTimeStamp = new Date();

        if (instance.isParameterValueChanged(parameter)) {
          if (match) {
            instance.incrementGlobalPendingChanges();
          }
        }
        else {
          if (!match) {
            instance.decrementGlobalPendingChanges();
          }
        }
      }
    });

    this._RTUConfiguration.next(this.dataStore);
    this.changeDetectionEmitter.emit();
  }

  sortByModuleAndProtocol(a : IParameter, b : IParameter){
    // this is not available in this context
    if (a.parameter === 'ModConfigured' && b.parameter !== 'ModConfigured')
      return -1;
    if (a.parameter === 'Protocol' && (b.parameter !== 'Protocol' && b.parameter !== 'ModConfigured'))
      return -1;
    if (b.parameter === 'ModConfigured' && a.parameter !== 'ModConfigured')
      return 1;
    if (b.parameter === 'Protocol' && (a.parameter !== 'Protocol' && a.parameter !== 'ModConfigured'))
      return 1;
    return 0;
  }

  applyCommandToRTU(command: IParameter, command2?: IParameter) {
     if (this.connectionStatus === RTUConnectionStatus.DISCONNECTED
        || this.connectionStatus === RTUConnectionStatus.CONNECTING
        || this.connectionStatus === RTUConnectionStatus.READINGCONFIGURATION) {
      this._notificationService.error('RTU not connected.');
      return;
     }

    if (this.connectionStatus === RTUConnectionStatus.WRITINGCOMMAND) {
      this._notificationService.error('Apply Command to RTU in progress.');
      return;
    }

    if (this.parametersBeingApplied) {
      this._notificationService.error('Apply Configuration to RTU in progress.');
      return;
    }

    if (command.pendingValue === '' && command.parameter.startsWith('Value')) {
      this._notificationService.error('Command cannot be empty. Value has been reverted.');
      command.pendingValue = command.value;
      return;
    }

    const instance = this;

    const connectionParams = {
      url: this.dataStore.url,
      loginId: this.sessionUsername,
      loginPassword: this.sessionPassword,
      securityMode: this.sessionSecurityMode,
      securityPolicy: this.sessionSecurityPolicy,
      userIdentity: this.sessionUserIdentity,
      certificateFilename: this.sessionCertFileName
    };

    const rtuDataValueList = [];
    const rtuDataValue = { 'value': command.pendingValue,
                          'status': command.pendingStatus,
                          'timeStamp' : command.pendingServerTimeStamp,
                          'dataType' : command.dataType,
                          'displayFormat' : command.displayFormat,
                          'identifier' : command.identifier};
    rtuDataValueList.push(rtuDataValue);

    // process additional command if passed in (i.e. user id and password change requires two commands)
    // the SetRtuData web service will process both commands in the supplied array
    if (command2) {
      const rtuDataValue2 = { 'value': command2.pendingValue,
                            'status': command2.pendingStatus,
                            'timeStamp' : command2.pendingServerTimeStamp,
                            'dataType' : command2.dataType,
                            'displayFormat' : command2.displayFormat,
                            'identifier' : command2.identifier};
      rtuDataValueList.push(rtuDataValue2);
    }

    const parms = { 'connectionParms': connectionParams, 'rtuDataValueList': rtuDataValueList };

    this._RtuconnectionstatusService.updateConnectionStatus( RTUConnectionStatus.WRITINGCOMMAND );

    this._http.post<IRTUCPUConfigurationWebService>(`${this.baseUrl}` + '/SetRtuData', parms).subscribe(response => {
      if (response.errorMessage && Object.keys(response.errorMessage).length > 0) {
        console.log(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','));
        instance._notificationService.error(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','));
        this._RtuconnectionstatusService.updateConnectionStatus( RTUConnectionStatus.CONNECTED );
        command.pendingValue = command.value;
        command.pendingStatus = command.status;
        command.pendingServerTimeStamp = command.serverTimeStamp;

        instance._RTUConfiguration.next(instance.dataStore);
        this.changeDetectionEmitter.emit();
        return;
      } else {
        if (response.data) {
          // Test for Bad Status, however ModCmd is normal to result in BadSecureChannelClosed
          if (response.data[0] !== 0
              && (response.data[0] !== 0x80860000
              || command.parameter !== 'ModCmd')) {
            instance._notificationService.error('Error status code : ' + instance.getStatusCode(response.data[0]) + ' applying command to NodeId ' + rtuDataValueList[0].identifier);
            this._RtuconnectionstatusService.updateConnectionStatus( RTUConnectionStatus.CONNECTED );
            command.pendingValue = command.value;
            command.pendingStatus = command.status;
            command.pendingServerTimeStamp = command.serverTimeStamp;

            instance._RTUConfiguration.next(instance.dataStore);
            this.changeDetectionEmitter.emit();
            return;
          }
        }

        // Download Complete : apply pending data to current data
        command.value = command.pendingValue;
        command.status = command.pendingStatus;
        command.serverTimeStamp = command.pendingServerTimeStamp;

        instance._RTUConfiguration.next(instance.dataStore);
        this.changeDetectionEmitter.emit();
        this._RtuconnectionstatusService.updateConnectionStatus( RTUConnectionStatus.CONNECTED );
        this._notificationService.success('Command written successfully.', 'Admin Command');
      }
    },
    error => {
      this._RtuconnectionstatusService.updateConnectionStatus( RTUConnectionStatus.ERRORWRITINGCOMMAND );
      instance.inError = true;
      console.log('Error applying command to RTU.');
      instance._notificationService.error( error.message, 'Error writing command, command has been reset' );
      command.pendingValue = command.value;
      command.pendingStatus = command.status;
      command.pendingServerTimeStamp = command.serverTimeStamp;

      instance._RTUConfiguration.next(instance.dataStore);
      this.changeDetectionEmitter.emit();
    });
  }

  applyDataToRTU(completeDownload: boolean, dataavailabletowrite: boolean){

    if (this.connectionStatus !== RTUConnectionStatus.CONNECTED
          && this.connectionStatus !== RTUConnectionStatus.READINGCONFIGURATION
          && this.connectionStatus !== RTUConnectionStatus.CONNECTING) {
      this._notificationService.error('RTU not connected.');
      return;
    }

    if (dataavailabletowrite === false)
    {
      // no data so just set the error and return. This is what the stake holders and po wanted.
      this._notificationService.error('No changes detected to write to the RTU.');
      return;
    }
    if (this.parametersBeingApplied) {
      this._notificationService.error('Apply To RTU in progress.');
      return;
    }

    const instance = this;

    const connectionParams = {
      url: this.dataStore.url,
      loginId: this.sessionUsername,
      loginPassword: this.sessionPassword,
      securityMode: this.sessionSecurityMode,
      securityPolicy: this.sessionSecurityPolicy,
      userIdentity: this.sessionUserIdentity,
      certificateFilename: this.sessionCertFileName
    };

    this.parametersBeingApplied = [];

    const parameterMaps = [];


    parameterMaps.push(this.dataStore.RTUConfiguration.module0.moduleConfiguration);
    parameterMaps.push(this.dataStore.RTUConfiguration.module0.channel1.channelConfiguration);
    parameterMaps.push(this.dataStore.RTUConfiguration.module0.channel2.channelConfiguration);
    parameterMaps.push(this.dataStore.RTUConfiguration.module0.channel3.channelConfiguration);
    parameterMaps.push(this.dataStore.RTUConfiguration.module0.channel4.channelConfiguration);
    parameterMaps.push(this.dataStore.RTUConfiguration.module0.channel5.channelConfiguration);
    parameterMaps.push(this.dataStore.RTUConfiguration.module0.channel6.channelConfiguration);
    parameterMaps.push(this.dataStore.RTUConfiguration.module0.channel7.channelConfiguration);
    parameterMaps.push(this.dataStore.RTUConfiguration.module0.channel8.channelConfiguration);

    const modules = [];
    modules.push(this.dataStore.RTUConfiguration.module1);
    modules.push(this.dataStore.RTUConfiguration.module2);
    modules.push(this.dataStore.RTUConfiguration.module3);
    modules.push(this.dataStore.RTUConfiguration.module4);
    modules.push(this.dataStore.RTUConfiguration.module5);
    modules.push(this.dataStore.RTUConfiguration.module6);

    modules.forEach(function (module) {
      if (!module) {
        return;
      }

      parameterMaps.push(module.moduleConfiguration);

      const channels = [];
      channels.push(module.channel1);
      channels.push(module.channel2);
      channels.push(module.channel3);
      channels.push(module.channel4);
      channels.push(module.channel5);
      channels.push(module.channel6);
      channels.push(module.channel7);
      channels.push(module.channel8);

      channels.forEach(function(channel){
        parameterMaps.push(channel.channelConfiguration);
      });
    });

    parameterMaps.forEach(function(parameterMap) {
      Object.keys(parameterMap).map(s => parameterMap[s]).forEach(function(parameter) {
        if (parameter.parameterIsVisible === 1
        && parameter.configClass === configClass.CONFIG
        && (completeDownload
        || parameter.pendingValue !== parameter.value
        || parameter.pendingStatus !== parameter.status)) {
          instance.parametersBeingApplied.push(parameter);
        }
      });
    });

    let numberOfTanks = 20;
    let numberOfAlarms = 0;
    let numberOfRegMap = 0;
    let numberOf509Certs = 20;

    const moduleConfiguration = this.dataStore.RTUConfiguration.module0.moduleConfiguration;
    const numberOfTanksIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfTanks');
    if (numberOfTanksIdentifier) {
      numberOfTanks = parseInt(moduleConfiguration[numberOfTanksIdentifier].value);
    }
    const numberOfAlarmsIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfAlarms');
    if (numberOfAlarmsIdentifier) {
      numberOfAlarms = parseInt(moduleConfiguration[numberOfAlarmsIdentifier].value);
    }
    const numberOfRegMapIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfRegMap');
    if (numberOfRegMapIdentifier) {
      numberOfRegMap = parseInt(moduleConfiguration[numberOfRegMapIdentifier].value);
    }

    this.numberOfTanksProcessed = 0;
    this.numberOfAlarmsProcessed = 0;
    this.numberOfRegMapProcessed = 0;
    this.numberOf509CertsProcessed = 0;

    let setoreandsendPointdata = true;
    this.dataStore.RTUConfiguration.points.forEach(function(point)
    {
      setoreandsendPointdata = true;
      if (point.name === 'Tank')
      {
        instance.numberOfTanksProcessed++;
        if (instance.numberOfTanksProcessed > numberOfTanks)
        {
          instance.numberOfTanksProcessed = numberOfTanks + 1;
          setoreandsendPointdata = false;
        }
      }
      if (point.name.trim() === 'Alarms')
      {
        instance.numberOfAlarmsProcessed++;
        if (instance.numberOfAlarmsProcessed > numberOfAlarms)
        {
          instance.numberOfAlarmsProcessed = numberOfAlarms + 1;
          setoreandsendPointdata = false;
        }
      }
      if (point.name.trim() === 'Register Map')
      {
        instance.numberOfRegMapProcessed++;
        if (instance.numberOfRegMapProcessed > numberOfRegMap)
        {
          instance.numberOfRegMapProcessed = numberOfRegMap + 1;
          setoreandsendPointdata = false;
        }
      }
      if (point.name.trim() === 'X.509 Certificate')
      {
        instance.numberOf509CertsProcessed++;
        if (instance.numberOf509CertsProcessed > numberOf509Certs)
        {
          instance.numberOf509CertsProcessed = numberOf509Certs + 1;
          setoreandsendPointdata = false;
        }
      }

      if (setoreandsendPointdata === true) {
        Object.keys(point.pointConfiguration).map(s => point.pointConfiguration[s]).forEach(function(parameter) {
          if (parameter.parameterIsVisible === 1
              && parameter.configClass === configClass.CONFIG
              && (completeDownload
              || parameter.pendingValue !== parameter.value
              || parameter.pendingStatus !== parameter.status)) {
            instance.parametersBeingApplied.push(parameter);
          }
        });
      }
    });

    // Remove IpAddress, SubnetMask, and Gateway since they can only be configured through USB device
    // This prevents possibly writing null values to these 3 fields
    const ipAddressIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'IpAddress');
    if (ipAddressIdentifier) {
      const index = this.parametersBeingApplied.map(parameter => parameter.identifier.toString()).indexOf(ipAddressIdentifier);
      if (index !== -1) {
        this.parametersBeingApplied.splice(index, 1);
      }
    }
    const subnetMaskIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'SubnetMask');
    if (subnetMaskIdentifier) {
      const index = this.parametersBeingApplied.map(parameter => parameter.identifier.toString()).indexOf(subnetMaskIdentifier);
      if (index !== -1) {
        this.parametersBeingApplied.splice(index, 1);
      }
    }
    const gatewayIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'Gateway');
    if (gatewayIdentifier) {
      const index = this.parametersBeingApplied.map(parameter => parameter.identifier.toString()).indexOf(gatewayIdentifier);
      if (index !== -1) {
        this.parametersBeingApplied.splice(index, 1);
      }
    }

    // sort parameters so that ModConfigured and Protocol are first.  A delay will provide for RTU to add/delete nodes accordingly
    this.parametersBeingApplied.sort(this.sortByModuleAndProtocol);

    this.parametersBeingAppliedIndex = 0;

    if (this.parametersBeingApplied.length === 0) {
      this.parametersBeingApplied = null;
      this.dataStore.RTUConfiguration.globalPendingChanges = 0;
      console.log('globalPendingChanges = ' + this.dataStore.RTUConfiguration.globalPendingChanges);
      return;
    }

    this.getRTUChassisConfiguration(connectionParams);
  }

  writeDataToRtu(connectionParams) {
    const instance = this;
    const rtuDataValueList = [];
    const parametersBeingAppliedStartIndex = this.parametersBeingAppliedIndex;
    let parameterCount = 0;
    let timedelay = 0;

    while (this.parametersBeingAppliedIndex < this.parametersBeingApplied.length) {
      const parameter = this.parametersBeingApplied[this.parametersBeingAppliedIndex];

      // at end of ModConfigured and Protocol Parameters then break for delay
      if (timedelay !== 0
        && parameter.parameter !== this.MODULECONFIGURED
        && parameter.parameter !== this.PROTOCOL) {
          break;
        }

        let rtuDataValue;
        if (parameter.displayFormat == "LHEX")
        {
           rtuDataValue = { 'value': parseInt(parameter.pendingValue,16).toString(),
          'status': parameter.pendingStatus,
          'timeStamp' : parameter.pendingServerTimeStamp,
          'dataType' : parameter.dataType,
          'displayFormat' : parameter.displayFormat,
          'identifier' : parameter.identifier};
        }
      else
      {
         rtuDataValue = { 'value': parameter.pendingValue,
                           'status': parameter.pendingStatus,
                           'timeStamp' : parameter.pendingServerTimeStamp,
                           'dataType' : parameter.dataType,
                           'displayFormat' : parameter.displayFormat,
                           'identifier' : parameter.identifier};
      }

      rtuDataValueList.push(rtuDataValue);
      parameterCount++;
      this.parametersBeingAppliedIndex++;

      if(parameterCount >= 1024){
        break;
      }

      if(parameter.parameter === 'NumberOfTanks'
      && parameter.pendingValue !== parameter.value){
        timedelay = 30000;
        break;
      }

      if (parameter.parameter === 'NumberOfAlarms'
      && parameter.pendingValue !== parameter.value) {
        timedelay = 30000;
        break;
      }

      if (parameter.parameter === 'NumberOfRegMap'
      && parameter.pendingValue !== parameter.value) {
        timedelay = 30000;
        break;
      }


      if((parameter.parameter === this.MODULECONFIGURED
      || parameter.parameter === this.PROTOCOL)
      && parameter.pendingValue !== parameter.value){
        timedelay = 5000;
        break;
      }
    }

    const parms = { 'connectionParms': connectionParams, 'rtuDataValueList': rtuDataValueList };
    console.log('writeDataToRtu calling /SetRtuData with ' + rtuDataValueList.length + ' dataValues');
    this._http.post<IRTUCPUConfigurationWebService>(`${this.baseUrl}` + '/SetRtuData', parms).subscribe(response => {
      if (response.errorMessage && Object.keys(response.errorMessage).length > 0) {
        console.log(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','));
        instance._notificationService.error(response.errorMessage[Object.keys(response.errorMessage)[0]].join(','));
        instance.parametersBeingApplied = null;
        instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.CONNECTED );

      } else {
        if (response.data) {
          for ( let index = 0; index < rtuDataValueList.length; index++) {
            if (response.data[index] !== 0) {
              const parameter = instance.parametersBeingApplied[parametersBeingAppliedStartIndex + index];
              instance._notificationService.error("Error status code : " + instance.getStatusCode(response.data[index]) + " writing to parameter " + parameter.parameter);
              instance.parametersBeingApplied = null;
              instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.CONNECTED );
              return;
            }
          }
        }

        // Download Complete : apply pending data to current data
        if(timedelay === 0
        && instance.parametersBeingAppliedIndex === instance.parametersBeingApplied.length) {
          instance.parametersBeingApplied.forEach(function (parameter) {

            if (instance.isParameterValueChanged(parameter)) {
              parameter.value = parameter.pendingValue;
              parameter.status = parameter.pendingStatus;
              parameter.serverTimeStamp = parameter.pendingServerTimeStamp;
              instance.decrementGlobalPendingChanges();
            }
          });

          instance._RTUConfiguration.next(instance.dataStore);
          instance.liveDataValues.next(instance.realtimeParameters);
          this.changeDetectionEmitter.emit();

          instance.parametersBeingApplied = null;
          this._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.CONNECTED );
        } else {
          window.setTimeout(function () {instance.writeDataToRtu(connectionParams); }, timedelay);
        }
      }
    },
    error => {
      instance._RtuconnectionstatusService.updateConnectionStatus(  RTUConnectionStatus.ERRORWRITINGCONFIGURATION );
      instance.inError = true;
      instance.parametersBeingApplied = null;
      console.log('Error Writing Configuration to RTU.');
      instance._notificationService.error( error.message, 'Error writing configuration' );
    });
  }

  public incrementGlobalPendingChanges() {
    this.dataStore.RTUConfiguration.globalPendingChanges++;
    console.log("globalPendingChanges = " + this.dataStore.RTUConfiguration.globalPendingChanges);

    if(this.dataStore.RTUConfiguration.defaultBlankConfiguration === true ||
      this.dataStore.RTUConfiguration.defaultBlankConfiguration === undefined)
      this.dataStore.RTUConfiguration.defaultBlankConfiguration = false;
  }

  public decrementGlobalPendingChanges() {
    this.dataStore.RTUConfiguration.globalPendingChanges--;
    console.log("globalPendingChanges = " + this.dataStore.RTUConfiguration.globalPendingChanges);
  }

  checkForRTUConfigChanges()
  {
    if (this.dataStore.RTUConfiguration.globalPendingChanges === 0) {
      return false;
    }
    else {
      return true;
    }
  }

  isParameterValueChanged(parameter : IParameter){
    if((typeof parameter.pendingValue === 'undefined' && typeof parameter.value !== 'undefined')
    || (typeof parameter.pendingValue !== 'undefined' && typeof parameter.value === 'undefined')
    || parameter.pendingValue != parameter.value
    || parameter.pendingStatus != parameter.status) {
      return true;
    }

    return false;
  }

  commitPendingChanges()
  {
    const instance = this;

    const modules = [];
    modules.push(this.dataStore.RTUConfiguration.module0);
    modules.push(this.dataStore.RTUConfiguration.module1);
    modules.push(this.dataStore.RTUConfiguration.module2);
    modules.push(this.dataStore.RTUConfiguration.module3);
    modules.push(this.dataStore.RTUConfiguration.module4);
    modules.push(this.dataStore.RTUConfiguration.module5);
    modules.push(this.dataStore.RTUConfiguration.module6);

    modules.forEach(function (module, index) {
      const parameterMaps = [];
      let rebuildModule = false;
      let moduleId = '0';

      if (!module)
      {
        return;
      }

      parameterMaps.push(module.moduleConfiguration);

      const channels = [];
      channels.push(module.channel1);
      channels.push(module.channel2);
      channels.push(module.channel3);
      channels.push(module.channel4);
      channels.push(module.channel5);
      channels.push(module.channel6);
      channels.push(module.channel7);
      channels.push(module.channel8);

      channels.forEach(function(channel)
      {
        parameterMaps.push(channel.channelConfiguration);
      });

      parameterMaps.forEach(function(parameterMap) {
        Object.keys(parameterMap).map(s => parameterMap[s]).forEach(function(parameter)
        {
          if(parameter.configClass !== configClass.CONFIG){
            if (parameter.configClass == configClass.DYNAMIC)
            {
              parameter.pendingValue = parameter.value; //Dynamic parameter values and pending values should not differ in an rtuconfig
              parameter.pendingStatus = parameter.status;
            }
            return;
          }

          if (parameter.parameter === instance.MODULECONFIGURED){
            moduleId = parameter.value;
          }

          if(instance.isParameterValueChanged( parameter)
           || parameter.pendingStatus != parameter.status)
          {
            parameter.value = parameter.pendingValue;
            parameter.status = parameter.pendingStatus;
            parameter.serverTimeStamp = parameter.pendingServerTimeStamp;
          }

          // Patch to correct prior error in reading ModbusMap for Modbus Slave Channels
          if (parameter.parameter === 'ModbusMap'
            && parameter.pendingValue === 'Default') {
            parameter.pendingValue = 'Default Map';
            parameter.value = 'Default Map';
          }
        });
      });
    });


    this.dataStore.RTUConfiguration.points.forEach(function(point)
    {
      Object.keys(point.pointConfiguration).map(s => point.pointConfiguration[s]).forEach(function(parameter)
      {
        if(parameter.configClass !== configClass.CONFIG){
          return;
        }

        if(instance.isParameterValueChanged(parameter)
        || parameter.pendingStatus != parameter.status)
        {
          parameter.value = parameter.pendingValue;
          parameter.status = parameter.pendingStatus;
          parameter.serverTimeStamp = parameter.pendingServerTimeStamp;
      }
      });
    });

    this.dataStore.RTUConfiguration.globalPendingChanges = 0;
    console.log("globalPendingChanges = " + this.dataStore.RTUConfiguration.globalPendingChanges);

    this._RTUConfiguration.next(this.dataStore);
  }


  cancelPendingChanges()
  {
    const instance = this;

    const modules = [];
    modules.push(this.dataStore.RTUConfiguration.module0);
    modules.push(this.dataStore.RTUConfiguration.module1);
    modules.push(this.dataStore.RTUConfiguration.module2);
    modules.push(this.dataStore.RTUConfiguration.module3);
    modules.push(this.dataStore.RTUConfiguration.module4);
    modules.push(this.dataStore.RTUConfiguration.module5);
    modules.push(this.dataStore.RTUConfiguration.module6);

    modules.forEach(function (module, moduleNumber) {
      let updateModule = false;
      let newModule = 0;


      Object.keys(module.moduleConfiguration).map(s => module.moduleConfiguration[s]).forEach(function(parameter)
      {
        if(parameter.configClass !== configClass.CONFIG){
          return;
        }

        if(instance.isParameterValueChanged(parameter)) {
          parameter.pendingValue = parameter.value;
          parameter.pendingStatus = parameter.status;
          parameter.pendingServerTimeStamp = parameter.serverTimeStamp;

          if (parameter.parameter === instance.MODULECONFIGURED){
            updateModule = true;
            newModule = parseInt(parameter.value, 10);
          }
        }
      });

      if (updateModule) {
        instance.buildBlankModule(newModule, moduleNumber);
      } else {

        const channels = [];
        channels.push(module.channel1);
        channels.push(module.channel2);
        channels.push(module.channel3);
        channels.push(module.channel4);
        channels.push(module.channel5);
        channels.push(module.channel6);
        channels.push(module.channel7);
        channels.push(module.channel8);

        channels.forEach(function (channel, channelNumber) {
          let updateProtocol = false;
          let newProtocol = '1';

          Object.keys(channel.channelConfiguration).map(s => channel.channelConfiguration[s]).forEach(function (parameter) {
            if (parameter.configClass !== configClass.CONFIG) {
              return;
            }

            if (instance.isParameterValueChanged(parameter)) {
              parameter.pendingValue = parameter.value;
              parameter.pendingStatus = parameter.status;
              parameter.pendingServerTimeStamp = parameter.serverTimeStamp;

              if (parameter.parameter === instance.PROTOCOL) {
                updateProtocol = true;
                newProtocol = parameter.value;
              }
            }
          });

          if (updateProtocol) {
            instance.updateProtocolParameters(newProtocol, moduleNumber, channelNumber + 1);
          }
        });
      }
    });


    this.dataStore.RTUConfiguration.points.forEach(function(point)
    {
      Object.keys(point.pointConfiguration).map(s => point.pointConfiguration[s]).forEach(function(parameter)
      {
        if(parameter.configClass !== configClass.CONFIG){
          return;
        }

        if(parameter.pendingValue != parameter.value
        || parameter.pendingStatus != parameter.status)
        {
          parameter.pendingValue = parameter.value;
          parameter.pendingStatus = parameter.status;
          parameter.pendingServerTimeStamp = parameter.serverTimeStamp;
        }
      });
    });

    this.dataStore.RTUConfiguration.globalPendingChanges = 0;
    this.dataStore.RTUConfiguration.defaultBlankConfiguration = false;
    console.log("globalPendingChanges = " + this.dataStore.RTUConfiguration.globalPendingChanges);

    this._RTUConfiguration.next(this.dataStore);
    this.changeDetectionEmitter.emit();
  }

  updateProtocolParameters(newProtocol: string, moduleNumber : number, channelNumber: number) {
    const instance = this;

    let id = -1;
    if(moduleNumber > 0){
      id = this.dataStore.RTUConfiguration['module' + moduleNumber].id;
    }

    let module = this.availableConfiguration.modules.find( x => x.id == id );

    const protocols = module[ 'channel' + channelNumber].channelProtocols

    let protocolAvailableCommands = '';
    protocols.forEach(function(protocol, index){
      protocolAvailableCommands += protocol.trim();
      if(index < protocols.length - 1){
        protocolAvailableCommands += ',';
      }
    });

    const channelConfiguration: IParameterMap = this.dataStore.RTUConfiguration['module' + moduleNumber][ 'channel' + channelNumber].channelConfiguration;

    // get the configuration for the new protocol
    const protocolConfig = this.availableConfiguration.protocols[parseInt(newProtocol, 10) - 1];

    if (!protocolConfig) {
      return;
    }

    const newChannelConfiguration: IParameterMap = {};

    Object.keys( channelConfiguration ).forEach(key => {
      let parameterExists = false;
      Object.keys(protocolConfig.protocolConfiguration).forEach(innerKey => {
        if (protocolConfig.protocolConfiguration[innerKey].parameter === channelConfiguration[key].parameter ) {
          newChannelConfiguration[key] = channelConfiguration[key];
          if(channelConfiguration[key].parameter === 'Protocol'){
            let match = false;
            if(!instance.isParameterValueChanged(channelConfiguration[key])){
              match = true;
            }
            channelConfiguration[key].pendingValue = newProtocol;
            channelConfiguration[key].pendingStatus = 0;
            channelConfiguration[key].availableCommands =  protocolAvailableCommands;
            if(match
            && instance.isParameterValueChanged(channelConfiguration[key])){
              instance.incrementGlobalPendingChanges();
            }
          }
          parameterExists = true;
        }
      });

      // decrementGlobalPendingChanges for any parameter that had a pending change and will be removed from configuration
      if(!parameterExists){
        const parameter = channelConfiguration[key];
        if (parameter.configClass === configClass.CONFIG
        && instance.isParameterValueChanged(parameter)){
          instance.decrementGlobalPendingChanges();
        }
      }
    });

      // add protocol configuration not missing from the existing list
    Object.keys(protocolConfig.protocolConfiguration).forEach(key => {
        const notExistsInProtocolList: IParameterMap = {};
                  // if not found we need to add it
        Object.keys(newChannelConfiguration).forEach(innerKey => {
            if ( newChannelConfiguration[innerKey].parameter === protocolConfig.protocolConfiguration[key].parameter) {
              notExistsInProtocolList[innerKey] = newChannelConfiguration[innerKey];
            }
          });

          if (Object.keys(notExistsInProtocolList).length === 0) {
            const identifier = protocolConfig.protocolConfiguration[key].opcstartNodeID + (moduleNumber * 8) + (channelNumber - 1);
            const parameter = protocolConfig.protocolConfiguration[key];
            const protocolConfigParm: IParameter = {
              configClass: parameter.configClass,
              parameter: parameter.parameter,
              description: parameter.description,
              value: parameter.value,
              status : parameter.status,
              serverTimeStamp : new Date(),
              pendingValue: parameter.pendingValue,
              pendingStatus : parameter.pendingStatus,
              pendingServerTimeStamp : new Date(),
              dataType: parameter.dataType,
              displayFormat: parameter.displayFormat,
              minimumValue: parameter.minimumValue,
              maximumValue: parameter.maximumValue,
              availableCommands: parameter.availableCommands,
              availableDeviceTypeValues: parameter.availableDeviceTypeValues,
              identifier: identifier,
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
            newChannelConfiguration[identifier] = protocolConfigParm;

            if(protocolConfigParm.configClass === configClass.CONFIG
            && instance.isParameterValueChanged(protocolConfigParm)){
              instance.incrementGlobalPendingChanges();
            }
          }
      });


    this.dataStore.RTUConfiguration['module' + moduleNumber][ 'channel' + channelNumber].channelConfiguration = newChannelConfiguration;
    this.dataStore.RTUConfiguration['module' + moduleNumber][ 'channel' + channelNumber].protocol = protocolConfig.name;

    let numberOfTanks = 20;
    const moduleConfiguration = this.dataStore.RTUConfiguration.module0.moduleConfiguration;
    const numberOfTanksIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'NumberOfTanks');
    if(numberOfTanksIdentifier){
      numberOfTanks = parseInt(moduleConfiguration[numberOfTanksIdentifier].value);
    }

    this.dataStore.RTUConfiguration.points.forEach(function(point, index)
    {

      if(index > numberOfTanks){
        return;
      }

      let moduleMatch = false;
      let channelMatch = false;

      Object.keys(point.pointConfiguration).map(s => point.pointConfiguration[s]).forEach(function(parameter) {
        if(parameter.configClass !== configClass.CONFIG){
          return;
        }

        if(parameter.parameter === 'Module'
        && parameter.pendingValue == moduleNumber){
          moduleMatch = true;
        }

        if(parameter.parameter === 'Channel'
        && parameter.pendingValue == channelNumber){
          channelMatch = true;
        }

        if(parameter.parameter === 'DeviceType'
        && moduleMatch
        && channelMatch
        && parameter.pendingValue !== '0'){
          if(protocolConfig){
            for(var deviceType in protocolConfig.availableDeviceTypes) {
              if(protocolConfig.availableDeviceTypes[deviceType].deviceTypeValue === parameter.pendingValue){
                return;
              }
            }
          }

          parameter.pendingValue = '0';
          instance.incrementGlobalPendingChanges();
        }
      });
    });

    this._RTUConfiguration.next(this.dataStore);
    this.changeDetectionEmitter.emit();
  }


  getLiveDataValues() {
    return this.liveDataValues.asObservable();
  }

  buildBlankModule( moduleId: number, ModuleLocation: number ) {
    const instance = this;

    const moduleNumber = 'module' + ModuleLocation;

    const currentModuleConfigParms: IParameterMap = this.dataStore.RTUConfiguration[moduleNumber].moduleConfiguration;

    // decrement globalPendingChanges for prior module
    Object.keys( currentModuleConfigParms ).forEach(key => {
      const parameter = currentModuleConfigParms[key];

      if (parameter.configClass !== configClass.CONFIG) {
        return;
      }

      if (instance.isParameterValueChanged(parameter)){
        instance.decrementGlobalPendingChanges();
      }
    });

    // find the virtual channel (we will default the channels to be virtual)
    let module = this.availableConfiguration.modules.find( x => x.id === moduleId );
    if ( !module ) {  // if we can't find the module at least apply the unknown changes
      module = this.availableConfiguration.modules.find( x => x.id === 0 );
    }

    if ( !module ) {
      this._notificationService.error( 'The specified card cannot be configured');
      return;
    }

    // find the virtual channel (we will default the channels to be virtual)
    let virtualChannel = this.availableConfiguration.protocols.find( x => x.name === 'Virtual Chan' );
    if ( !virtualChannel ) {
      virtualChannel = { name: 'Virtual Chan', protocolConfiguration: [], availableDeviceTypes: [] };
    }
    this.dataStore.RTUConfiguration[moduleNumber].img = module.img;
    this.dataStore.RTUConfiguration[moduleNumber].id = module.id.toString();
    this.dataStore.RTUConfiguration[moduleNumber].name = module.name;

    for ( let i = 1; i <= 8; i++) {
      this.dataStore.RTUConfiguration[moduleNumber][ 'channel' + i].protocol = virtualChannel.name;
      this.dataStore.RTUConfiguration[moduleNumber][ 'channel' + i].type = module[ 'channel' + i].type;
      this.dataStore.RTUConfiguration[moduleNumber][ 'channel' + i].top = module[ 'channel' + i].top;
      this.dataStore.RTUConfiguration[moduleNumber][ 'channel' + i].left = module[ 'channel' + i].left;
      this.dataStore.RTUConfiguration[moduleNumber][ 'channel' + i].width = module[ 'channel' + i].width;
      this.dataStore.RTUConfiguration[moduleNumber][ 'channel' + i].height = module[ 'channel' + i].height;
      instance.updateProtocolParameters("1", ModuleLocation, i);
    }

    const moduleConfigParms: IParameterMap = {};


    Object.keys(module.moduleConfiguration).map(function(property, b) {
      const parameter = module.moduleConfiguration[ property ];

      let currentValue: any = '';
      let pendingValue: any = '';

      // find the current value for the existing parameter so we can populate the same value
      if ( parameter.configClass === configClass.CONFIG ) {
        // tslint:disable-next-line:max-line-length
        const identifier = Object.keys(currentModuleConfigParms).find(s => currentModuleConfigParms[s].parameter === parameter.parameter);
        if ( identifier ) {
          currentValue = currentModuleConfigParms[identifier].value;
          pendingValue = currentModuleConfigParms[identifier].pendingValue;
        }
      }

      // for dynamic values values set the value to empty so if we are connected we will retrieve the values
      const moduleConfigParm: IParameter = {
        configClass: parameter.configClass,
        parameter: parameter.parameter,
        description: parameter.description,
        value:  parameter.configClass === configClass.DYNAMIC ? '' : (currentValue !== '' ? currentValue : parameter.value),
        status: parameter.status,
        serverTimeStamp : new Date(),
        // tslint:disable-next-line:max-line-length
        pendingValue: parameter.configClass === configClass.DYNAMIC ? '' : (parameter.parameter === 'ModConfigured' ? module.id.toString() : (pendingValue !== '' ? pendingValue : parameter.pendingValue)),
        pendingStatus: parameter.pendingStatus,
        pendingServerTimeStamp : new Date(),
        dataType: parameter.dataType,
        displayFormat: parameter.displayFormat,
        minimumValue: parameter.minimumValue,
        maximumValue: parameter.maximumValue,
        availableCommands: parameter.availableCommands,
        availableDeviceTypeValues: parameter.availableDeviceTypeValues,
        identifier: parameter.opcstartNodeID +  ( ModuleLocation - 1 ),
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

      moduleConfigParms[ parameter.opcstartNodeID + ( ModuleLocation - 1 )] = moduleConfigParm;

      if(moduleConfigParm.configClass !== configClass.CONFIG){
        return;
      }

      if (instance.isParameterValueChanged(moduleConfigParm)) {
        instance.incrementGlobalPendingChanges();
      }
    });

    this.dataStore.RTUConfiguration[moduleNumber].moduleConfiguration = moduleConfigParms;

    this._RTUConfiguration.next( this.dataStore );

  }

  public setdefaultblankConfiguration(valueToSet: boolean) {
    if (valueToSet === true) {
      this.dataStore.RTUConfiguration.defaultBlankConfiguration = true;
      this.upgradeButtonActive = false;
    } else {
      this.dataStore.RTUConfiguration.defaultBlankConfiguration = false;
      this.upgradeButtonActive = true;
    }
  }

  public getdefaultblankConfiguration() {
    if (this.dataStore.RTUConfiguration.defaultBlankConfiguration === true) {
      this.upgradeButtonActive = false;
      return true;
    } else {
      this.upgradeButtonActive = true;
      return false;
    }
  }

  public getProtocolforModuleAndChannel(moduleNum, channelNum)
  {
    if (channelNum > 0 && channelNum< 9)
    {
      var channelConfig = this.dataStore.RTUConfiguration["module" + moduleNum]["channel" + channelNum].channelConfiguration;
      var protocol = channelConfig[Object.keys(channelConfig).find(s => channelConfig[s].parameter === 'Protocol')].pendingValue;

      return protocol;}
    return 0;
  }

  getStatusCode(status: number) {
    const codeBits = (status / 32767) * 32767;

    switch (codeBits) {
      case 0:
        return 'Good';
      case 0x80000000:
        return 'Bad';
      case 0x80010000:
        return 'BadUnexpectedError';
      case 0x800200000:
        return 'BadInternalError';
      case 0x80030000:
        return 'BadOutOfMemory';
      case 0x80040000:
        return 'BadResourceUnavailable';
      case 0x80050000:
        return 'BadCommunicationError';
      case 0x80060000:
        return 'BadEncodingError';
      case 0x80070000:
        return 'BadDecodingError';
      case 0x80080000:
        return 'BadEncodingLimitsExceeded';
      case 0x80B80000:
        return 'BadRequestTooLarge';
      case 0x80B90000:
        return 'BadResponseTooLarge';
      case 0x80090000:
        return 'BadUnknownResponse';
      case 0x800A0000:
        return 'BadTimeout';
      case 0x800B0000:
        return 'BadServiceUnsupported';
      case 0x800C0000:
        return 'BadShutdown';
      case 0x800D0000:
        return 'BadServerNotConnected';
      case 0x800E0000:
        return 'BadServerHalted';
      case 0x800F0000:
        return 'BadNothingToDo';
      case 0x80100000:
        return 'BadTooManyOperations';
      case 0x80DB0000:
        return 'BadTooManyMonitoredItems';
      case 0x80110000:
        return 'BadDataTypeIdUnknown';
      case 0x80120000:
        return 'BadCertificateInvalid';
      case 0x80130000:
        return 'BadSecurityChecksFailed';
      case 0x81140000:
        return 'BadCertificatePolicyCheckFailed';
      case 0x80140000:
        return 'BadCertificateTimeInvalid';
      case 0x80150000:
        return 'BadCertificateIssuerTimeInvalid';
      case 0x80160000:
        return 'BadCertificateHostNameInvalid';
      case 0x80170000:
        return 'BadCertificateUriInvalid';
      case 0x80180000:
        return 'BadCertificateUseNotAllowed';
      case 0x80190000:
        return 'BadCertificateIssuerUseNotAllowed';
      case 0x801A0000:
        return 'BadCertificateUntrusted';
      case 0x801B0000:
        return 'BadCertificateRevocationUnknown';
      case 0x801C0000:
        return 'BadCertificateIssuerRevocationUnknown';
      case 0x801D0000:
        return 'BadCertificateRevoked';
      case 0x801E0000:
        return 'BadCertificateIssuerRevoked';
      case 0x810D0000:
        return 'BadCertificateChainIncomplete';
      case 0x801F0000:
        return 'BadUserAccessDenied';
      case 0x80200000:
        return 'BadIdentityTokenInvalid';
      case 0x80210000:
        return 'BadIdentityTokenRejected';
      case 0x80220000:
        return 'BadSecureChannelIdInvalid';
      case 0x80230000:
        return 'BadInvalidTimestamp';
      case 0x80240000:
        return 'BadNonceInvalid';
      case 0x80250000:
        return 'BadSessionIdInvalid';
      case 0x80260000:
        return 'BadSessionClosed';
      case 0x80270000:
        return 'BadSessionNotActivated';
      case 0x80280000:
        return 'BadSubscriptionIdInvalid';
      case 0x802A0000:
        return 'BadRequestHeaderInvalid';
      case 0x802B0000:
        return 'BadTimestampsToReturnInvalid';
      case 0x802C0000:
        return 'BadRequestCancelledByClient';
      case 0x80E50000:
        return 'BadTooManyArguments';
      case 0x810E0000:
        return 'BadLicenseExpired';
      case 0x810F0000:
        return 'BadLicenseLimitsExceeded';
      case 0x81100000:
        return 'BadLicenseNotAvailable';
      case 0x80310000:
        return 'BadNoCommunication';
      case 0x80320000:
        return 'BadWaitingForInitialData';
      case 0x80330000:
        return 'BadNodeIdInvalid';
      case 0x80340000:
        return 'BadNodeIdUnknown';
      case 0x80350000:
        return 'BadAttributeIdInvalid';
      case 0x80360000:
        return 'BadIndexRangeInvalid';
      case 0x80370000:
        return 'BadIndexRangeNoData';
      case 0x80380000:
        return 'BadDataEncodingInvalid';
      case 0x80390000:
        return 'BadDataEncodingUnsupported';
      case 0x803A0000:
        return 'BadNotReadable';
      case 0x803B0000:
        return 'BadNotWritable';
      case 0x803C0000:
        return 'BadOutOfRange';
      case 0x803D0000:
        return 'BadNotSupported';
      case 0x803E0000:
        return 'BadNotFound';
      case 0x803F0000:
        return 'BadObjectDeleted';
      case 0x80400000:
        return 'BadNotImplemented';
      case 0x80410000:
        return 'BadMonitoringModeInvalid';
      case 0x80420000:
        return 'BadMonitoredItemIdInvalid';
      case 0x80430000:
        return 'BadMonitoredItemFilterInvalid';
      case 0x80440000:
        return 'BadMonitoredItemFilterUnsupported';
      case 0x80450000:
        return 'BadFilterNotAllowed';
      case 0x80460000:
        return 'BadStructureMissing';
      case 0x80470000:
        return 'BadEventFilterInvalid';
      case 0x80480000:
        return 'BadContentFilterInvalid';
      case 0x80C10000:
        return 'BadFilterOperatorInvalid';
      case 0x80C20000:
        return 'BadFilterOperatorUnsupported';
      case 0x80C30000:
        return 'BadFilterOperandCountMismatch';
      case 0x80490000:
        return 'BadFilterOperandInvalid';
      case 0x80C40000:
        return 'BadFilterElementInvalid';
      case 0x80C50000:
        return 'BadFilterLiteralInvalid';
      case 0x804A0000:
        return 'BadContinuationPointInvalid';
      case 0x804B0000:
        return 'BadNoContinuationPoints';
      case 0x804C0000:
        return 'BadReferenceTypeIdInvalid';
      case 0x804D0000:
        return 'BadBrowseDirectionInvalid';
      case 0x804E0000:
        return 'BadNodeNotInView';
      case 0x81120000:
        return 'BadNumericOverflow';
      case 0x804F0000:
        return 'BadServerUriInvalid';
      case 0x80500000:
        return 'BadServerNameMissing';
      case 0x80510000:
        return 'BadDiscoveryUrlMissing';
      case 0x80520000:
        return 'BadSempahoreFileMissing';
      case 0x80530000:
        return 'BadRequestTypeInvalid';
      case 0x80540000:
        return 'BadSecurityModeRejected';
      case 0x80550000:
        return 'BadSecurityPolicyRejected';
      case 0x80560000:
        return 'BadTooManySessions';
      case 0x80570000:
        return 'BadUserSignatureInvalid';
      case 0x80580000:
        return 'BadApplicationSignatureInvalid';
      case 0x80590000:
        return 'BadNoValidCertificates';
      case 0x80C60000:
        return 'BadIdentityChangeNotSupported';
      case 0x805A0000:
        return 'BadRequestCancelledByRequest';
      case 0x805B0000:
        return 'BadParentNodeIdInvalid';
      case 0x805C0000:
        return 'BadReferenceNotAllowed';
      case 0x805D0000:
        return 'BadNodeIdRejected';
      case 0x805E0000:
        return 'BadNodeIdExists';
      case 0x805F0000:
        return 'BadNodeClassInvalid';
      case 0x80600000:
        return 'BadBrowseNameInvalid';
      case 0x80610000:
        return 'BadBrowseNameDuplicated';
      case 0x80620000:
        return 'BadNodeAttributesInvalid';
      case 0x80630000:
        return 'BadTypeDefinitionInvalid';
      case 0x80640000:
        return 'BadSourceNodeIdInvalid';
      case 0x80650000:
        return 'BadTargetNodeIdInvalid';
      case 0x80660000:
        return 'BadDuplicateReferenceNotAllowed';
      case 0x80670000:
        return 'BadInvalidSelfReference';
      case 0x80680000:
        return 'BadReferenceLocalOnly';
      case 0x80690000:
        return 'BadNoDeleteRights';
      case 0x806A0000:
        return 'BadServerIndexInvalid';
      case 0x806B0000:
        return 'BadViewIdUnknown';
      case 0x80C90000:
        return 'BadViewTimestampInvalid';
      case 0x80CA0000:
        return 'BadViewParameterMismatch';
      case 0x80CB0000:
        return 'BadViewVersionInvalid';
      case 0x80C80000:
        return 'BadNotTypeDefinition';
      case 0x806D0000:
        return 'BadTooManyMatches';
      case 0x806E0000:
        return 'BadQueryTooComplex';
      case 0x806F0000:
        return 'BadNoMatch';
      case 0x80700000:
        return 'BadMaxAgeInvalid';
      case 0x80E60000:
        return 'BadSecurityModeInsufficient';
      case 0x80710000:
        return 'BadHistoryOperationInvalid';
      case 0x80720000:
        return 'BadHistoryOperationUnsupported';
      case 0x80BD0000:
        return 'BadInvalidTimestampArgument';
       case 0x81110000:
        return 'BadNotExecutable';
      case 0x80040000:
        return 'BadTooManySubscriptions';
      case 0x8073000:
        return 'BadWriteNotSupported';
      case 0x80740000:
        return 'BadTypeMismatch';
      case 0x80750000:
        return 'BadMethodInvalid';
      case 0x80760000:
        return 'BadArgumentsMissing';
      case 0x80770000:
        return 'BadTooManySubscriptions';
      case 0x80780000:
        return 'BadTooManyPublishRequests';
      case 0x80790000:
        return 'BadNoSubscription';
      case 0x807A0000:
        return 'BadSequenceNumberUnknown';
      case 0x807B0000:
        return 'BadMessageNotAvailable';
      case 0x807C0000:
        return 'BadInsufficientClientProfile';
      case 0x80BF0000:
        return 'BadStateNotActive';
      case 0x81150000:
        return 'BadAlreadyExists';
      case 0x807D0000:
        return 'BadTcpServerTooBusy';
      case 0x807E0000:
        return 'BadTcpMessageTypeInvalid';
      case 0x807F0000:
        return 'BadTcpSecureChannelUnknown';
      case 0x80800000:
        return 'BadTcpMessageTooLarge';
      case 0x80810000:
        return 'BadTcpNotEnoughResources';
      case 0x80820000:
        return 'BadTcpInternalError';
      case 0x80830000:
        return 'BadTcpEndpointUrlInvalid';
      case 0x80840000:
        return 'BadRequestInterrupted';
      case 0x80850000:
        return 'BadRequestTimeout';
      case 0x80860000:
        return 'BadSecureChannelClosed';
      case 0x80870000:
        return 'BadSecureChannelTokenUnknown';
      case 0x80880000:
        return 'BadSequenceNumberInvalid';
      case 0x80BE0000:
        return 'BadProtocolVersionUnsupported';
      case 0x80890000:
        return 'BadConfigurationError';
      case 0x808A0000:
        return 'BadNotConnected';
      case 0x808B0000:
        return 'BadDeviceFailure';
      case 0x808C0000:
        return 'BadSensorFailure';
      case 0x808D0000:
        return 'BadOutOfService';
      case 0x808E0000:
        return 'BadDeadbandFilterInvalid';
      case 0x80970000:
        return 'BadRefreshInProgress';
      case 0x80980000:
        return 'BadConditionAlreadyDisabled';
      case 0x80CC0000:
        return 'BadConditionAlreadyEnabled';
      case 0x80990000:
        return 'BadConditionDisabled';
      case 0x809A0000:
        return 'BadEventIdUnknown';
      case 0x80BB0000:
        return 'BadEventNotAcknowledgeable';
      case 0x80CD0000:
        return 'BadDialogNotActive';
      case 0x80CE0000:
        return 'BadDialogResponseInvalid';
      case 0x80CF0000:
        return 'BadConditionBranchAlreadyAcked';
      case 0x80D00000:
        return 'BadConditionBranchAlreadyConfirmed';
      case 0x80D10000:
        return 'BadConditionAlreadyShelved';
      case 0x80D20000:
        return 'BadConditionNotShelved';
      case 0x80D30000:
        return 'BadShelvingTimeOutOfRange';
      case 0x809B0000:
        return 'BadNoData';
      case 0x80D70000:
        return 'BadBoundNotFound';
      case 0x80D80000:
        return 'BadBoundNotSupported';
      case 0x809D0000:
        return 'BadDataLost';
      case 0x809E0000:
        return 'BadDataUnavailable';
      case 0x809F0000:
        return 'BadEntryExists';
      case 0x80A00000:
        return 'BadNoEntryExists';
      case 0x80A10000:
        return 'BadTimestampNotSupported';
      case 0x80D40000:
        return 'BadAggregateListMismatch';
      case 0x80D50000:
        return 'BadAggregateNotSupported';
      case 0x80D60000:
        return 'BadAggregateInvalidInputs';
      case 0x80DA0000:
        return 'BadAggregateConfigurationRejected';
      case 0x80E40000:
        return 'BadRequestNotAllowed';
      case 0x81130000:
        return 'BadRequestNotComplete';
      case 0x80E10000:
        return 'BadDominantValueChanged';
      case 0x80E30000:
        return 'BadDependentValueChanged';
      case 0x80AB0000:
        return 'BadInvalidArgument';
      case 0x80AC0000:
        return 'BadConnectionRejected';
      case 0x80AD0000:
        return 'BadDisconnect';
      case 0x80AE0000:
        return 'BadConnectionClosed';
      case 0x80AF0000:
        return 'BadInvalidState';
      case 0x80040000:
        return 'BadEndOfStream';
      case 0x80B10000:
        return 'BadNoDataAvailable';
      case 0x80B20000:
        return 'BadWaitingForResponse';
      case 0x80B30000:
        return 'BadOperationAbandoned';
      case 0x80B40000:
        return 'BadExpectedStreamToBlock';
      case 0x80B50000:
        return 'BadWouldBlock';
      case 0x80B60000:
        return 'BadSyntaxError';
      case 0x80B70000:
        return 'BadMaxConnectionsReached';
      default:
        return 'Bad';
    }
  }

  setSessionCredentials(connectionString: string, securityMode: securityModeEnum, securityPolicy: securityPolicyEnum, userIdentity: userIdentityEnum, certFile: string, loginId: string, password: string) {
    if (connectionString) {
      this.dataStore.url = connectionString;
    }
    this.sessionSecurityMode = securityMode;
    this.sessionSecurityPolicy = securityPolicy;
    this.sessionUserIdentity = userIdentity;
    this.sessionCertFileName = certFile;
    this.sessionUsername = loginId;
    this.sessionPassword = password;
  }

  setAdminCredentials(loginId: string, password: string) {
    this.sessionUsername = loginId;
    this.sessionPassword = password;
  }

  convertLHEXValues(data: IRTUCPUModule) {
    Object.keys(data.moduleConfiguration).forEach (function (key) {
      const parameter = data.moduleConfiguration[key];
      if (parameter.displayFormat === 'LHEX') {
        parameter.value = parseInt(parameter.value, 10).toString(16).toUpperCase();
        parameter.pendingValue = parseInt(parameter.pendingValue, 10).toString(16).toUpperCase();
      }
    });
    const channelStrings = ['channel1', 'channel2', 'channel3', 'channel4', 'channel5', 'channel6', 'channel7', 'channel8'];
    channelStrings.forEach(function(channelString) {
      Object.keys(data[channelString].channelConfiguration).forEach(function (key) {
        const parameter = data[channelString].channelConfiguration[key];
        if (parameter.displayFormat === 'LHEX') {
          parameter.value =  parseInt(parameter.value, 10).toString(16).toUpperCase();
          parameter.pendingValue = parseInt(parameter.pendingValue, 10).toString(16).toUpperCase();
        }
      });
    });
    return data;
  }

  // Evaluate the version currently used in the uploaded config file
  public evaluateXMLVersion(selectedRTUConfigFile: IRTUConfiguration) {
    let xmlVersion: string;
    const moduleConfigurationFromConfigFile = selectedRTUConfigFile.module0.moduleConfiguration;

    // Pull out the XML version that is used in the selected config file
    Object.keys(moduleConfigurationFromConfigFile).forEach(key =>
      {
        if (moduleConfigurationFromConfigFile[key].parameter === 'SysVer')
        {
          xmlVersion  = moduleConfigurationFromConfigFile[key].value;
          xmlVersion = xmlVersion + '.rtuxml';
        }
    });

    return xmlVersion;
  }

  // Sort list of available XML files in case they aren't already
  public sortedListOfAvailableXMLFiles(listOfAvailableXMLFiles: any) {

    // Sort available XML files
    const sortedListOfAvailableXMLFiles: Array<string> = listOfAvailableXMLFiles;
    sortedListOfAvailableXMLFiles.sort();

    // Pull out NRTU002.rtuxml and make it the first element in the array
    const index = sortedListOfAvailableXMLFiles.indexOf('NRTU002.rtuxml');
    if (index >= 0) {
      sortedListOfAvailableXMLFiles.splice(index, 1);
      sortedListOfAvailableXMLFiles.unshift('NRTU002.rtuxml');
    }

    return sortedListOfAvailableXMLFiles;
  }

  // Determine viable upgrade versions based on the XML version of the
  // selected RTU config file (don't support downgrade).
  public evaluateViableXMLUpgradeVersions(xmlVersion: string, sortedListOfAvailableXMLFiles: Array<any>) {
    let viableXMLUpgradeVersions: string[] = [];
    let index = 0;

    // Check to see if there are newer XML files available compared to
    // what is currently being used in the config file
    for (index; index < sortedListOfAvailableXMLFiles.length; index++) {
      if (xmlVersion.toUpperCase() === sortedListOfAvailableXMLFiles[index].toUpperCase() && index < sortedListOfAvailableXMLFiles.length) {


        // Assign all values that come after the current XML version to an array or list
        // These will be viable upgrade versions
        while (index < sortedListOfAvailableXMLFiles.length) {
          viableXMLUpgradeVersions.push(sortedListOfAvailableXMLFiles[index]);
          this.canUpgradeXMLVersion = true;
          index++;
        }

        // Double check to see if the current XML version is in the array and remove it
        if ( xmlVersion.toUpperCase() === viableXMLUpgradeVersions[0].toUpperCase()) {
          viableXMLUpgradeVersions.shift();
        }
      }
      else if ( xmlVersion.toUpperCase() === sortedListOfAvailableXMLFiles[index].toUpperCase()
                  && index === sortedListOfAvailableXMLFiles.length)
      {
        this.canUpgradeXMLVersion = false;
      }
      else if ( xmlVersion.toUpperCase() !== sortedListOfAvailableXMLFiles[index].toUpperCase())
      {
        this.canUpgradeXMLVersion = false;
        continue;
      }
      else
      {
        this.canUpgradeXMLVersion = false;
        break;
      }
    }

    return viableXMLUpgradeVersions;
  }

  // Starts the upgrade process for the config file
  public kickoffUpgradeConfigFile(fileName: string) {
    this.upgradeConfigurationInProgress = true;
    this.currentRTUConfiguration = JSON.parse(JSON.stringify(this.dataStore.RTUConfiguration));
    this.loadXmlConfiguration(fileName, this.upgradeConfigurationInProgress);
  }

  // Upgrade by migrating/mapping existing parameters and features into the new XML structure
  public upgradeConfigFile() {
    // -| -------------------
    // -| Point comparison
    // -| -------------------
    const self = this;

    // Comparing the names to see if they match
    // Ex: Tank, Alarms, Modbus Floating Point Reg., Modbus Integer Reg., and Register Map
    for (let index = 0; index < self.currentRTUConfiguration.points.length; index++)
    {
      if (self.currentRTUConfiguration.points[index].name === self.dataStore.RTUConfiguration.points[index].name)
      {
        // Get the identfier
        Object.keys(self.currentRTUConfiguration.points[index].pointConfiguration).forEach(function(key)
        {
          // Compare identfier to previous line. If they match the next line of code will execute
          if (self.dataStore.RTUConfiguration.points[index].pointConfiguration.hasOwnProperty(key))
          {
            // Comparing parameter and resultType to make sure they match
            // before assigning values (name.value)
            var upgradedConfiguration = self.dataStore.RTUConfiguration.points[index].pointConfiguration[key];
            var originalConfiguration = self.currentRTUConfiguration.points[index].pointConfiguration[key];
            if (upgradedConfiguration.parameter === originalConfiguration.parameter
                  && upgradedConfiguration.resultType === originalConfiguration.resultType
                  && originalConfiguration.configClass === configClass.CONFIG)
            {

              if (originalConfiguration.displayFormat == "LHEX" && originalConfiguration.parameter == "Mask") {
                let alarmTypeKey = Object.keys(self.currentRTUConfiguration.points[index].pointConfiguration).find(key => self.currentRTUConfiguration.points[index].pointConfiguration[key].parameter === 'Type');
                if ( self.currentRTUConfiguration.points[index].pointConfiguration[alarmTypeKey].value != "2"
                  && self.currentRTUConfiguration.points[index].pointConfiguration[alarmTypeKey].value != "3") {
                  upgradedConfiguration.displayFormat =  originalConfiguration.displayFormat;
                  upgradedConfiguration.value = originalConfiguration.value;
                  upgradedConfiguration.pendingValue = originalConfiguration.pendingValue;
                } else {
                  upgradedConfiguration.value = parseInt(originalConfiguration.value, 10).toString(16).toUpperCase();
                  upgradedConfiguration.pendingValue = parseInt(originalConfiguration.pendingValue, 10).toString(16).toUpperCase();
                  upgradedConfiguration.displayFormat = originalConfiguration.displayFormat;
                }
              } else {
                upgradedConfiguration.displayFormat = originalConfiguration.displayFormat;
                upgradedConfiguration.pendingValue = originalConfiguration.pendingValue;
                upgradedConfiguration.value = originalConfiguration.value;

              }
            }
          }
        });
      }
    }

    // -| -------------------
    // -| Module comparison
    // -| -------------------
    const maxModules = 6;
    const maxChannels = 8;
    const protocolList = allProtocols;

    // sometimes the protocol for each channel is not configured properly so before processing we eed to set the right name
    for (let moduleCounter = 0; moduleCounter <= maxModules; moduleCounter++) {
      const moduleNum = 'module' + moduleCounter;
      for (let channelCounter = 1; channelCounter <= maxChannels; channelCounter++) {
        const channelNum = 'channel' + channelCounter;

        Object.keys(self.currentRTUConfiguration[moduleNum][channelNum].channelConfiguration).forEach(key => {
          if (self.currentRTUConfiguration[moduleNum][channelNum].channelConfiguration[key].parameter.toLowerCase() === 'protocol') {
            self.currentRTUConfiguration[moduleNum][channelNum].protocol = protocolList[self.currentRTUConfiguration[moduleNum][channelNum].channelConfiguration[key].value];
          }
        });

      }
    }

    // reset the channels in the CPU
    const CPUmodule = self.availableConfiguration.modules.filter(obj => obj.id === -1 );
    for (let channelCounter = 1; channelCounter <= maxChannels; channelCounter++)
    {
      const channelNum = 'channel' + channelCounter;

      if (self.currentRTUConfiguration['module0'][channelNum].protocol !== '' &&
          self.currentRTUConfiguration['module0'][channelNum].protocol !== 'Virtual Chan')
      {
        const existingprotocol = self.availableConfiguration.protocols.filter(
                        obj => obj.name === self.currentRTUConfiguration['module0'][channelNum].protocol);

        if (existingprotocol.length > 0)
        {
          this.dataStore.RTUConfiguration['module0'][channelNum].channelConfiguration = this.getAvailableChannelConfiguration( existingprotocol[0], 0, channelCounter, CPUmodule[0][channelNum].channelProtocols );
          self.dataStore.RTUConfiguration['module0'][channelNum].protocol = self.currentRTUConfiguration['module0'][channelNum].protocol;
        }
      }
    }

    // Iterate through interface modules and update them with the configured module and channel
    for (let counter = 0; counter <= maxModules; counter++)
    {
      const moduleNum = 'module' + counter;
      let moduleId: number;

      if (self.currentRTUConfiguration[moduleNum].id != 0)
      {
        moduleId = counter;
        const result = self.availableConfiguration.modules.filter(obj => obj.id == self.currentRTUConfiguration[moduleNum].id);  // need to compare int and string

        if (result.length > 0) {
          // initialize the module with the existing one
          self.initializeModule(moduleNum, moduleId, result[0]);
          // initialize each channel with the existing one
          for (let channelCounter = 1; channelCounter <= maxChannels; channelCounter++)
          {
            const channelNum = 'channel' + channelCounter;

            if (self.currentRTUConfiguration[moduleNum][channelNum].protocol !== '' &&
                self.currentRTUConfiguration[moduleNum][channelNum].protocol !== 'Virtual Chan')
            {
              const existingprotocol = self.availableConfiguration.protocols.filter(
                              obj => obj.name === self.currentRTUConfiguration[moduleNum][channelNum].protocol);

              if (existingprotocol.length > 0)
              {
                this.dataStore.RTUConfiguration[moduleNum][channelNum].channelConfiguration = this.getAvailableChannelConfiguration( existingprotocol[0], moduleId, channelCounter, result[0][channelNum].channelProtocols );
                self.dataStore.RTUConfiguration[moduleNum][channelNum].protocol = self.currentRTUConfiguration[moduleNum][channelNum].protocol;
              }
            }
          }
        }
      }
    }
    // Comparison and assigning of values
    for (let moduleCounter = 0; moduleCounter <= maxModules; moduleCounter++)
    {
      const moduleNumber = 'module' + moduleCounter;

      // update the module parameters
      Object.keys(self.currentRTUConfiguration[moduleNumber].moduleConfiguration).forEach(key =>
      {
        // Compare identfier to previous line. If they match the next line of code will execute
        if (self.dataStore.RTUConfiguration[moduleNumber].moduleConfiguration.hasOwnProperty(key))
        {
          // Comparing parameter and resultType to make sure they match
          // before assigning values (name.value)
          if (self.currentRTUConfiguration[moduleNumber].moduleConfiguration[key].parameter
                === self.dataStore.RTUConfiguration[moduleNumber].moduleConfiguration[key].parameter
                && self.currentRTUConfiguration[moduleNumber].moduleConfiguration[key].resultType
                === self.dataStore.RTUConfiguration[moduleNumber].moduleConfiguration[key].resultType
                && self.currentRTUConfiguration[moduleNumber].moduleConfiguration[key].configClass === configClass.CONFIG
                && self.dataStore.RTUConfiguration[moduleNumber].moduleConfiguration[key].parameter !== 'SysVer')  // do not update the system version
          {
            self.dataStore.RTUConfiguration[moduleNumber].moduleConfiguration[key].pendingValue
            = self.currentRTUConfiguration[moduleNumber].moduleConfiguration[key].pendingValue; 
            self.dataStore.RTUConfiguration[moduleNumber].moduleConfiguration[key].value
            = self.currentRTUConfiguration[moduleNumber].moduleConfiguration[key].value; 
          }
        }
      });

      // update each of the channels
      for (let channelCounter = 1; channelCounter <= maxChannels; channelCounter++)
      {
        const channelNumber = 'channel' + channelCounter;
        if (self.currentRTUConfiguration[moduleNumber][channelNumber].protocol
              === self.dataStore.RTUConfiguration[moduleNumber][channelNumber].protocol)
        {
          // Get the identfier
          Object.keys(self.currentRTUConfiguration[moduleNumber][channelNumber].channelConfiguration).forEach(key =>
          {
            // Compare identfier to previous line. If they match the next line of code will execute
            if (self.dataStore.RTUConfiguration[moduleNumber][channelNumber].channelConfiguration.hasOwnProperty(key))
            {
              // Comparing parameter and resultType to make sure they match
              // before assigning values (name.value)
              var sourceParameter = self.currentRTUConfiguration[moduleNumber][channelNumber].channelConfiguration[key].parameter;
              var destinationParameter = self.dataStore.RTUConfiguration[moduleNumber][channelNumber].channelConfiguration[key].parameter;

              // Baudrate paramter name gets processed differently and we add **<number>** to the name. 
              sourceParameter = sourceParameter.startsWith('BaudRate **') ? 'BaudRate' : sourceParameter;
              destinationParameter = destinationParameter.startsWith('BaudRate **') ? 'BaudRate' : destinationParameter;

              if (sourceParameter === destinationParameter
                    && self.currentRTUConfiguration[moduleNumber][channelNumber].channelConfiguration[key].resultType
                    === self.dataStore.RTUConfiguration[moduleNumber][channelNumber].channelConfiguration[key].resultType
                    && self.currentRTUConfiguration[moduleNumber][channelNumber].channelConfiguration[key].configClass === configClass.CONFIG)
              {
                self.dataStore.RTUConfiguration[moduleNumber][channelNumber].channelConfiguration[key].pendingValue
                = self.currentRTUConfiguration[moduleNumber][channelNumber].channelConfiguration[key].pendingValue;
                self.dataStore.RTUConfiguration[moduleNumber][channelNumber].channelConfiguration[key].value
                = self.currentRTUConfiguration[moduleNumber][channelNumber].channelConfiguration[key].value;
              }
            }
          });
        }
      }
    }
    this.upgradeConfigurationInProgress = false;
    this._RTUConfiguration.next( this.dataStore );
  }

  // Copied from chassismoduleview\chassismoduleview.component.ts because we needed
  // to modify the function in order to use it for the config file upgrade process
  public initializeModule(moduleNumber: string, moduleId: number, result: any) {
    const currentModuleConfigParms: IParameterMap = this.dataStore.RTUConfiguration[moduleNumber].moduleConfiguration;

    // find the virtual channel (we will default the channels to be virtual)
    let virtualChannel = this.availableConfiguration.protocols.find( x => x.name === 'Virtual Chan' );
    if ( !virtualChannel ) {
      virtualChannel = { name: 'Virtual Chan', protocolConfiguration: [], availableDeviceTypes: [] };
    }

    this.dataStore.RTUConfiguration[moduleNumber].img = result.img;
    this.dataStore.RTUConfiguration[moduleNumber].id = result.id;
    this.dataStore.RTUConfiguration[moduleNumber].name = result.name;

    this.dataStore.RTUConfiguration[moduleNumber].channel1.type = result.channel1.type;
    this.dataStore.RTUConfiguration[moduleNumber].channel1.top = result.channel1.top;
    this.dataStore.RTUConfiguration[moduleNumber].channel1.left = result.channel1.left;
    this.dataStore.RTUConfiguration[moduleNumber].channel1.width = result.channel1.width;
    this.dataStore.RTUConfiguration[moduleNumber].channel1.height = result.channel1.height;
    this.dataStore.RTUConfiguration[moduleNumber].channel1.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 1, result.channel1.channelProtocols );


    this.dataStore.RTUConfiguration[moduleNumber].channel2.type = result.channel2.type;
    this.dataStore.RTUConfiguration[moduleNumber].channel2.top = result.channel2.top;
    this.dataStore.RTUConfiguration[moduleNumber].channel2.left = result.channel2.left;
    this.dataStore.RTUConfiguration[moduleNumber].channel2.width = result.channel2.width;
    this.dataStore.RTUConfiguration[moduleNumber].channel2.height = result.channel2.height;
    this.dataStore.RTUConfiguration[moduleNumber].channel2.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 2, result.channel2.channelProtocols );

    this.dataStore.RTUConfiguration[moduleNumber].channel3.type = result.channel3.type;
    this.dataStore.RTUConfiguration[moduleNumber].channel3.top = result.channel3.top;
    this.dataStore.RTUConfiguration[moduleNumber].channel3.left = result.channel3.left;
    this.dataStore.RTUConfiguration[moduleNumber].channel3.width = result.channel3.width;
    this.dataStore.RTUConfiguration[moduleNumber].channel3.height = result.channel3.height;
    this.dataStore.RTUConfiguration[moduleNumber].channel3.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 3, result.channel3.channelProtocols );

    this.dataStore.RTUConfiguration[moduleNumber].channel4.type = result.channel4.type;
    this.dataStore.RTUConfiguration[moduleNumber].channel4.top = result.channel4.top;
    this.dataStore.RTUConfiguration[moduleNumber].channel4.left = result.channel4.left;
    this.dataStore.RTUConfiguration[moduleNumber].channel4.width = result.channel4.width;
    this.dataStore.RTUConfiguration[moduleNumber].channel4.height = result.channel4.height;
    this.dataStore.RTUConfiguration[moduleNumber].channel4.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 4, result.channel4.channelProtocols );

    this.dataStore.RTUConfiguration[moduleNumber].channel5.type = result.channel5.type;
    this.dataStore.RTUConfiguration[moduleNumber].channel5.top = result.channel5.top;
    this.dataStore.RTUConfiguration[moduleNumber].channel5.left = result.channel5.left;
    this.dataStore.RTUConfiguration[moduleNumber].channel5.width = result.channel5.width;
    this.dataStore.RTUConfiguration[moduleNumber].channel5.height = result.channel5.height;
    this.dataStore.RTUConfiguration[moduleNumber].channel5.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 5, result.channel5.channelProtocols );

    this.dataStore.RTUConfiguration[moduleNumber].channel6.type = result.channel6.type;
    this.dataStore.RTUConfiguration[moduleNumber].channel6.top = result.channel6.top;
    this.dataStore.RTUConfiguration[moduleNumber].channel6.left = result.channel6.left;
    this.dataStore.RTUConfiguration[moduleNumber].channel6.width = result.channel6.width;
    this.dataStore.RTUConfiguration[moduleNumber].channel6.height = result.channel6.height;
    this.dataStore.RTUConfiguration[moduleNumber].channel6.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 6, result.channel6.channelProtocols );

    this.dataStore.RTUConfiguration[moduleNumber].channel7.type = result.channel7.type;
    this.dataStore.RTUConfiguration[moduleNumber].channel7.top = result.channel7.top;
    this.dataStore.RTUConfiguration[moduleNumber].channel7.left = result.channel7.left;
    this.dataStore.RTUConfiguration[moduleNumber].channel7.width = result.channel7.width;
    this.dataStore.RTUConfiguration[moduleNumber].channel7.height = result.channel7.height;
    this.dataStore.RTUConfiguration[moduleNumber].channel7.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 7, result.channel7.channelProtocols );

    this.dataStore.RTUConfiguration[moduleNumber].channel8.type = result.channel8.type;
    this.dataStore.RTUConfiguration[moduleNumber].channel8.top = result.channel8.top;
    this.dataStore.RTUConfiguration[moduleNumber].channel8.left = result.channel8.left;
    this.dataStore.RTUConfiguration[moduleNumber].channel8.width = result.channel8.width;
    this.dataStore.RTUConfiguration[moduleNumber].channel8.height = result.channel8.height;
    this.dataStore.RTUConfiguration[moduleNumber].channel8.channelConfiguration =
      this.getAvailableChannelConfiguration( virtualChannel, moduleId, 8, result.channel8.channelProtocols );

    const moduleConfigParms: IParameterMap = {};

    Object.keys(result.moduleConfiguration).forEach(key => {

      const parameter = result.moduleConfiguration[key];

      let currentValue: any = '';
      let pendingValue: any = '';

      // find the current value for the existing parameter so we can populate the same value
      if ( parameter.configClass === configClass.CONFIG ) {
        // tslint:disable-next-line:max-line-length
        const paramerrIdentifier = Object.keys(currentModuleConfigParms).find(s => currentModuleConfigParms[s].parameter === parameter.parameter);
        if (paramerrIdentifier ) {
          currentValue = currentModuleConfigParms[paramerrIdentifier].value;
          pendingValue = currentModuleConfigParms[paramerrIdentifier].pendingValue;
        }
      }

      const moduleConfigParm: IParameter = {
        configClass: parameter.configClass,
        parameter: parameter.parameter,
        description: parameter.description,
        value:  parameter.configClass === configClass.DYNAMIC ? '' : (currentValue !== '' ? currentValue : parameter.value),
        status : parameter.status,
        serverTimeStamp : new Date(),
        pendingValue: parameter.configClass === configClass.DYNAMIC ? '' : (parameter.parameter === 'ModConfigured' ? result.id.toString() : (pendingValue !== '' ? pendingValue : parameter.pendingValue)),
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
    });

    this.dataStore.RTUConfiguration[moduleNumber].moduleConfiguration = moduleConfigParms;
  }

  public isUpgradingConfiguration() {
    return this.upgradeConfigurationInProgress;
  }
  // Copied from chassismoduleview\chassismoduleview.component.ts because we
  // reference it in the initializeModule method that is in this file
  public getAvailableChannelConfiguration( channelInfo: IAvailableProtocol, moduleId: number, channelId: number, protocols : string [] ): IParameterMap {
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
}

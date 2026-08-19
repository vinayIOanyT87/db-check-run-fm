import { Component, OnInit, TemplateRef, ViewChild, ViewEncapsulation, OnDestroy } from '@angular/core';
import { RtuconfigurationService } from 'src/app/services/rtuconfiguration.service';
import { LocalStorageService } from 'src/app/services/localstorage.service';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
import { RtuconnectService, securityModeEnum, securityPolicyEnum, userIdentityEnum} from 'src/app/services/rtuconnect.service';
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';
import { Subscription } from 'rxjs';
import { Form, FormGroup } from '@angular/forms';

export interface IRTUConnectionInfo {
  ipaddress: string;
  loginId: string;
  name: string;
  lastconnection: string;
  securityMode: string;
  userIdentity: string;
  securityPolicy: string;
  certificateFilename: string;
}


@Component({
  selector: 'app-connect',
  templateUrl: './connect.component.html',
  styleUrls: ['./connect.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class ConnectComponent implements OnInit, OnDestroy {
  rtuInitialConnectionSubscription: Subscription;
  rtuconfigurationSubscription: Subscription;
  rtuconnectionstatusSubscription: Subscription;

  // display properties
  module1Img = 'url( "assets/emptymodule.png")';
  module2Img = 'url( "assets/emptymodule.png")';
  module3Img = 'url( "assets/emptymodule.png")';
  module4Img = 'url( "assets/emptymodule.png")';
  module5Img = 'url( "assets/emptymodule.png")';
  module6Img = 'url( "assets/emptymodule.png")';
  module1Id: string | number = 0;
  module2Id: string | number = 0;
  module3Id: string | number = 0;
  module4Id: string | number = 0;
  module5Id: string | number = 0;
  module6Id: string | number = 0;

  adminUsernameLength = '';
  adminPasswordLength = '';

  rtuname = '';
  ipaddress = '';
  dbfile = '';
  sysver = '';
  fwver = '';
  searchByIP = '';

  // display area next to connection indicator (current configuration values)
  configIPAddress = '';
  configRTUName = '';
  configfwver = '';
  configSysVer = '';
  configModule1Id: string | number = 0;
  configModule2Id: string | number = 0;
  configModule3Id: string | number = 0;
  configModule4Id: string | number = 0;
  configModule5Id: string | number = 0;
  configModule6Id: string | number = 0;

  modalConfigLocal = {
    backdrop: true,
    ignoreBackdropClick: false,
    class: 'modal-lg'
  };

  status = RTUConnectionStatus.DISCONNECTED;
  url = '';
  @ViewChild('modalRTUConnection', { static: true }) public modalRTUConnection: TemplateRef<any>;
  @ViewChild('modalRTUConnected', { static: true }) public modalRTUConnected: TemplateRef<any>;
  @ViewChild('modalVerifyWritetoRTUAction', { static: true }) public modalVerifyWritetoRTUAction: TemplateRef<any>;
  @ViewChild('modalVerifyReadtoRTUAction', { static: true }) public modalVerifyReadtoRTUAction: TemplateRef<any>;

  modalRef: BsModalRef;
  modalReflocal: BsModalRef;

  connectionString: string;
  loginId: string;
  password: string;
  lastconnection: string;
  securityMode: securityModeEnum;
  userIdentity: userIdentityEnum;
  securityPolicy: securityPolicyEnum;
  certificateFilename: string;

  connectionList: IRTUConnectionInfo[];
  filteredconnectionList: IRTUConnectionInfo[];
  activeListConnection = 0;

  constructor( private _rtuConfiguration: RtuconfigurationService,
    private _modalService: BsModalService,
    private _localStorage: LocalStorageService,
    private _rtuInitialConnectionService: RtuconnectService,
    private _RtuconnectionstatusService: RtuconnectionstatusService ) {
      const lastConnection: any = _localStorage.get( 'RTU8810lastConnection' );
      if ( !lastConnection || ( lastConnection && !Array.isArray( lastConnection ) )) {
        this.connectionList = [];
      } else {
        this.connectionList = lastConnection;
      }
      this.filteredconnectionList = this.connectionList;
      this.connectionString = '';
      this.loginId = '';
      this.password = '';
      this.securityMode = securityModeEnum.none;
      this.securityPolicy = securityPolicyEnum.Basic256Sha256;
      this.userIdentity = userIdentityEnum.anonymous;
      this.certificateFilename = '';
    }

  ngOnInit() {
    const instance = this;

    // subscribe to the rtu connection status service
    this.rtuconnectionstatusSubscription = this._RtuconnectionstatusService.get().subscribe( data => {
      this.status = data;
    });

    this.rtuInitialConnectionSubscription = this._rtuInitialConnectionService.get().subscribe(data => {
      if ( data.ipaddress ) {
        instance.url = data.ipaddress;

        if ( data.RTUConfiguration && data.RTUConfiguration.module0 ) {
          const moduleConfiguration = data.RTUConfiguration.module0.moduleConfiguration;

          Object.keys(moduleConfiguration).forEach(key => {

            if ( moduleConfiguration[key].parameter === 'Label' ) {
              instance.rtuname = moduleConfiguration[key].value;
            }
            if ( moduleConfiguration[key].parameter === 'IpAddress' ) {
              instance.ipaddress = moduleConfiguration[key].value;
            }
            if ( moduleConfiguration[key].parameter === 'DBFile' ) {
              instance.dbfile = moduleConfiguration[key].value;
            }
            if ( moduleConfiguration[key].parameter === 'SysVer' ) {
              instance.sysver = moduleConfiguration[key].value;
            }
            if ( moduleConfiguration[key].parameter === 'FwVer' ) {
              instance.fwver = moduleConfiguration[key].value;
            }
          });

          this.module1Img = 'url( "assets/' + data.RTUConfiguration.module1.img + '")';
          this.module2Img = 'url( "assets/' + data.RTUConfiguration.module2.img + '")';
          this.module3Img = 'url( "assets/' + data.RTUConfiguration.module3.img + '")';
          this.module4Img = 'url( "assets/' + data.RTUConfiguration.module4.img + '")';
          this.module5Img = 'url( "assets/' + data.RTUConfiguration.module5.img + '")';
          this.module6Img = 'url( "assets/' + data.RTUConfiguration.module6.img + '")';
          this.module1Id = data.RTUConfiguration.module1.id;
          this.module2Id = data.RTUConfiguration.module2.id;
          this.module3Id = data.RTUConfiguration.module3.id;
          this.module4Id = data.RTUConfiguration.module4.id;
          this.module5Id = data.RTUConfiguration.module5.id;
          this.module6Id = data.RTUConfiguration.module6.id;
        }

      } else {
        instance.status = RTUConnectionStatus.DISCONNECTED;
        instance.url = '';
        instance.rtuname = '';
        instance.ipaddress = '';
        instance.dbfile = '';
        instance.sysver = '';
        instance.fwver = '';
      }
    });

    this.rtuconfigurationSubscription = this._rtuConfiguration.get().subscribe(data => {
      if (data.RTUConfiguration && data.RTUConfiguration.module0) {
        const moduleConfiguration = data.RTUConfiguration.module0.moduleConfiguration;

        const adminUsernameLengthIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'AdminName');
        if (adminUsernameLengthIdentifier !== undefined) {
          instance.adminUsernameLength =  moduleConfiguration[adminUsernameLengthIdentifier].datatypeLength;
        } else {
          instance.adminUsernameLength = '32';
        }

        const adminPasswordLengthIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'AdminPassword');
        if (adminUsernameLengthIdentifier !== undefined) {
          instance.adminPasswordLength =  moduleConfiguration[adminPasswordLengthIdentifier].datatypeLength;
        } else {
          instance.adminPasswordLength = '32';
        }

        const configIPAddressIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'IpAddress');
        instance.configIPAddress = (configIPAddressIdentifier) ? moduleConfiguration[configIPAddressIdentifier].value : '';

        const configRTUNameIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'Label');
        instance.configRTUName = (configRTUNameIdentifier) ? moduleConfiguration[configRTUNameIdentifier].value : '';

        const configfwverIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'FwVer');
        instance.configfwver = (configfwverIdentifier) ? moduleConfiguration[configfwverIdentifier].value : '';

        const configSysVerIdentifier = Object.keys(moduleConfiguration).find(s => moduleConfiguration[s].parameter === 'SysVer');
        instance.configSysVer = (configSysVerIdentifier) ? moduleConfiguration[configSysVerIdentifier].value : '';
        instance.configModule1Id = data.RTUConfiguration.module1.id;
        instance.configModule2Id = data.RTUConfiguration.module2.id;
        instance.configModule3Id = data.RTUConfiguration.module3.id;
        instance.configModule4Id = data.RTUConfiguration.module4.id;
        instance.configModule5Id = data.RTUConfiguration.module5.id;
        instance.configModule6Id = data.RTUConfiguration.module6.id;

      }
    } );

  }

  ngOnDestroy() {
    this.rtuconnectionstatusSubscription.unsubscribe();
    this.rtuInitialConnectionSubscription.unsubscribe();
    this.rtuconfigurationSubscription.unsubscribe();
    this._rtuInitialConnectionService.clearConfiguration();
  }

  selectConnection(connection: IRTUConnectionInfo) {
    this.connectionString = connection.ipaddress;
    this.securityMode = securityModeEnum[connection.securityMode];
    this.securityPolicy = securityPolicyEnum[connection.securityPolicy];
    this.userIdentity = userIdentityEnum[connection.userIdentity];
    this.certificateFilename = connection.certificateFilename;
    this.loginId = connection.loginId;
    this.password = '';
  }

  filter(newValue) {
    this.filteredconnectionList = this.connectionList.filter(x => x.ipaddress.includes(newValue));
  }

  disconnectButton() {
    this._rtuInitialConnectionService.updateDisconnectStatus(RTUConnectionStatus.DISCONNECTED);
    this._RtuconnectionstatusService.updateConnectionStatus(RTUConnectionStatus.DISCONNECTED);
  }

  connectButton() {
    const modalConfig = {
      backdrop: true,
      ignoreBackdropClick: true,
      class: 'modal-lg'
    };

    this.searchByIP = '';
    this.filter( this.searchByIP );

    this.module1Img = 'url( "assets/emptymodule.png")';
    this.module2Img = 'url( "assets/emptymodule.png")';
    this.module3Img = 'url( "assets/emptymodule.png")';
    this.module4Img = 'url( "assets/emptymodule.png")';
    this.module5Img = 'url( "assets/emptymodule.png")';
    this.module6Img = 'url( "assets/emptymodule.png")';
    this.module1Id = 0;
    this.module2Id = 0;
    this.module3Id = 0;
    this.module4Id = 0;
    this.module5Id = 0;
    this.module6Id = 0;
    this.rtuname = 'unknown';
    this.ipaddress = 'unknown';
    this.dbfile = 'unknown';
    this.sysver = 'unknown';
    this.fwver = 'unknown';

    // use the ip address from the open configuration
    this.connectionString = this.configIPAddress;
    if ( this.configIPAddress !== '') {
      const configLoginIdIdx = this.connectionList.findIndex(x => x.ipaddress === this.configIPAddress);
      this.loginId = configLoginIdIdx > -1 ? this.connectionList[ configLoginIdIdx ].loginId : '';
    }
    // Always clear password to force user to re-enter password
    this.password = '';
    this.modalRef = this._modalService.show( this.modalRTUConnection, modalConfig);
  }

  connectToRTU() {
    this.modalRef.hide();
    const modalConfig = {
      backdrop: true,
      ignoreBackdropClick: true,
      class: 'modal-xlg'
    };

    // const formData = new FormData();
    // connectRTUForm should come from connectToRTU(connectRTUForm: FormGroup) {
    // formData.append('file', connectRTUForm.get('certFile').value);

    this._rtuInitialConnectionService.InitialConnection(this.connectionString, securityModeEnum[this.securityMode],
      securityPolicyEnum[this.securityPolicy], userIdentityEnum[this.userIdentity], this.certificateFilename,
      this.loginId, this.password, this.configSysVer);
    this._rtuConfiguration.setSessionCredentials(this.connectionString, securityModeEnum[this.securityMode],
      securityPolicyEnum[this.securityPolicy], userIdentityEnum[this.userIdentity], this.certificateFilename,
      this.loginId, this.password);
    this.modalRef = this._modalService.show(this.modalRTUConnected, modalConfig);
  }

  handleCertFileInput(files: FileList) {
    if (files[0]) {
      this.certificateFilename = files[0].name;
    }

    const file = document.getElementById('file');

    // this.certToUpload = files.item(0);
    // change the input type from text and back to file to clear the filelist so that everytime the file loads
    file.attributes['type'].value = 'text';
    file.attributes['type'].value = 'file';
  }

  verifyReadConfig() {
    // if this is a new cleared configurtion do not prompt
    let tt = 0;
    ++tt;
    if (this._rtuConfiguration.getdefaultblankConfiguration() === false) {
      this.modalReflocal = this._modalService.show( this.modalVerifyReadtoRTUAction , this.modalConfigLocal);
    } else {
      this.readConfig();
    }
  }

  readConfig() {
    if (this.modalReflocal !== undefined) {
      this.modalReflocal.hide();
    }
    const connectionIdx = this.connectionList.findIndex(x => x.ipaddress === this.connectionString);
    // tslint:disable-next-line:max-line-length
    const connectionObject = {
      ipaddress: this.connectionString,
      loginId: this.loginId,
      name: this.rtuname,
      lastconnection: new Date().toLocaleString(),
      securityMode: this.securityMode,
      userIdentity: this.userIdentity,
      securityPolicy: this.securityPolicy,
      certificateFilename: this.certificateFilename
    };

    if ( connectionIdx > -1 ) {
      this.connectionList.splice(connectionIdx, 1);
      this.connectionList.unshift(connectionObject);
    } else {
    this.connectionList.unshift(connectionObject);
    }

    this._localStorage.store( 'RTU8810lastConnection', this.connectionList );

    this._rtuInitialConnectionService.clearConfiguration();
    this._rtuConfiguration.connectToRTU(this.connectionString, this.securityMode, this.securityPolicy, this.userIdentity, this.certificateFilename, this.loginId, this.password, this.configSysVer.indexOf('.') > 0 ? this.configSysVer : this.configSysVer.concat('.rtuxml'));
    this.modalRef.hide();
  }

  verifyWriteConfig() {
    this.modalReflocal = this._modalService.show( this.modalVerifyWritetoRTUAction , this.modalConfigLocal);
  }

  writeConfig() {
    if (this.modalReflocal !== undefined) {
        this.modalReflocal.hide();
    }
    const connectionIdx = this.connectionList.findIndex(x => x.ipaddress === this.connectionString);
    const connectionObject = {
      ipaddress: this.connectionString,
      loginId: this.loginId,
      name: this.rtuname,
      lastconnection: new Date().toLocaleString(),
      securityMode: this.securityMode,
      userIdentity: this.userIdentity,
      securityPolicy: this.securityPolicy,
      certificateFilename: this.certificateFilename
    };

    if (connectionIdx > -1) {
      this.connectionList.splice(connectionIdx, 1);
      this.connectionList.unshift(connectionObject);
    } else {
      this.connectionList.unshift(connectionObject);
    }

    this._localStorage.store('RTU8810lastConnection', this.connectionList);
    this._rtuInitialConnectionService.clearConfiguration();
    this._rtuConfiguration.applyDataToRTU(true, true);
    this.modalRef.hide();
  }

  cancelConnection() {
    this.url = '';
    this.rtuname = '';
    this.ipaddress = '';
    this.dbfile = '';
    this.sysver = '';
    this.fwver = '';
    this._rtuInitialConnectionService.updateDisconnectStatus( RTUConnectionStatus.DISCONNECTED );
    this._RtuconnectionstatusService.updateConnectionStatus( RTUConnectionStatus.DISCONNECTED );
    this.modalRef.hide();
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

  isConnected() {
    return this.status === RTUConnectionStatus.CONNECTED || this.status === RTUConnectionStatus.ERRORREADING;
  }
  isInProgress() {
    return this.status === RTUConnectionStatus.CONNECTING || this.status === RTUConnectionStatus.READINGCONFIGURATION;
  }

  getIPAddressStyle(data: any, nameid: string) {
    let styles = {};

    const textinputbox = document.getElementById(nameid);

    var isFocused = (document.activeElement === textinputbox);

    if (data.value === null || data.value === '') {
      if (isFocused === true) {
        styles = { 'border-color': '#4a90e2', 'box-shadow': '0 0 4px #4a90e2' };
      } else {
        styles = { 'border-color': '#dddddd', 'box-shadow': 'none' };
      }
    } else {
      // we have data so see if it is in the right format
      var stValue = data.value;
      var ifounddot1 = 0;
      var ifounddot2 = 0;
      var ifounddot3 = 0;
      var ifounddot4 = 0;

      for (let iloop = 0; iloop < stValue.length; iloop++) {
        if (stValue[iloop] === '.') {
          if (ifounddot1 === 0) {
            ifounddot1 = iloop;
          } else if (ifounddot1 > 0 && ifounddot2 === 0) {
            ifounddot2 = iloop;
          } else if (ifounddot1 > 0 &&
            ifounddot2 > 0 &&
            ifounddot3 === 0) {
            ifounddot3 = iloop;
          } else if (ifounddot1 > 0 &&
            ifounddot2 > 0 &&
            ifounddot3 > 0 &&
            ifounddot4 === 0) {
            ifounddot4 = iloop;
          }
        }
      }
      if (ifounddot4 === 0 &&
          ifounddot1 > 0 &&
          ifounddot2 > (ifounddot1 + 1) &&
          ifounddot3 > (ifounddot2 + 1) &&
          ifounddot3 < (stValue.length - 1)) {
        if (isFocused === true) {
          styles = { 'border-color': '#4a90e2', 'box-shadow': '0 0 4px #4a90e2' };
        } else {
          styles = { 'border-color': '#dddddd', 'box-shadow': 'none' };
        }
      } else {
        if (isFocused === true) {
          styles = { 'border-color': '#c52039', 'box-shadow': '0 0 4px #c52039' };
        } else {
          styles = { 'border-color': '#c52039', 'box-shadow': 'none' };
        }
      }
    }

    return styles;
  }

  getlogonidStyle(data: any, nameid: string) {
    let styles = {};
    const instance = this;
    const textinputbox = document.getElementById(nameid);
    const isFocused = (document.activeElement === textinputbox);

    textinputbox.setAttribute('maxLength', instance.adminUsernameLength);

    if (data.value === null || data.value === '') {
      if (isFocused === true) {
        styles = { 'border-color': '#4a90e2', 'box-shadow': '0 0 4px #4a90e2' };
      } else {
        styles = { 'border-color': '#dddddd', 'box-shadow': 'none' };
      }
    } else {
      if (isFocused === true) {
        styles = { 'border-color': '#4a90e2', 'box-shadow': '0 0 4px #4a90e2' };
      } else {
        styles = { 'border-color': '#dddddd', 'box-shadow': 'none' };
      }
    }

    return styles;
  }

  getlogonpasswordStyle(data: any, nameid: string) {
    let styles = {};
    const instance = this;
    const textinputbox = document.getElementById(nameid);
    const isFocused = (document.activeElement === textinputbox);

    textinputbox.setAttribute('maxLength', instance.adminPasswordLength);

    if (data.value === null || data.value === '') {
      if (isFocused === true) {
        styles = { 'border-color': '#4a90e2', 'box-shadow': '0 0 4px #4a90e2' };
      } else {
        styles = { 'border-color': '#dddddd', 'box-shadow': 'none' };
      }
    } else {
      if (isFocused === true) {
        styles = { 'border-color': '#4a90e2', 'box-shadow': '0 0 4px #ced4da' };
      } else {
        styles = { 'border-color': '#dddddd', 'box-shadow': 'none' };
      }
    }

    return styles;
  }

  getRequiredTextboxStyle(data: any, nameid: string) {
    let styles = {};

    const textinputbox = document.getElementById(nameid);
    const isFocused = (document.activeElement === textinputbox);

    if (typeof data === 'undefined' || data.value === null || data.value === '') {
      if (isFocused === true) {
        styles = { 'border-color': '#4a90e2', 'box-shadow': '0 0 4px #4a90e2' };
      } else {
        styles = { 'border-color': '#ced4da', 'box-shadow': 'none' };
      }
    } else {
      if (isFocused === true) {
        styles = { 'border-color': '#4a90e2', 'box-shadow': '0 0 4px #ced4da' };
      } else {
        styles = { 'border-color': '#ced4da', 'box-shadow': 'none' };
      }
    }

    return styles;
  }

  connectButtonEnabled(connectRTUForm: FormGroup) {
    switch (this.userIdentity) {
    case 'anonymous':
      return (connectRTUForm.valid);
    case 'username':
      return (connectRTUForm.valid && this.loginId !== '' && this.password !== '');
    case 'certificate':
      return (connectRTUForm.valid && this.certificateFilename !== '');
    }
  }

  updateUserIdentity() {
    if (this.securityMode === securityModeEnum.none) {
      if (this.userIdentity === userIdentityEnum.certificate) {
        this.userIdentity = userIdentityEnum.anonymous;
      }
    } else if (this.userIdentity === userIdentityEnum.anonymous) {
      this.userIdentity = userIdentityEnum.username;
    }
  }
}

import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
//import { Component, OnInit } from '@angular/core';
import { RtuconfigurationService, IRTUConfiguration } from 'src/app/services/rtuconfiguration.service';
import * as saveAs from 'node_modules/file-saver'
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';

import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
// import { Button } from 'protractor';
// import { DISABLED } from '@angular/forms/src/model';

export interface IHomeCards {
  name: string;
  description: string;
  icon: string;
  action: string;
  routerLinkPath: string;
  info: string;
  infoimg: string;
  disabledCard: string;
  externalLink: string;
}

@Component({
  selector: 'app-home-view',
  templateUrl: './home-view.component.html',
  styleUrls: ['./home-view.component.css']
})

export class HomeViewComponent implements OnInit {
  ShowLoaded = false;
  ShowCleared = false;
  ShowError = false;
  ShowUpgrade = false;
  homeCards: IHomeCards[];
  constructor(private _rtuConfiguration: RtuconfigurationService,
    private _modalService: BsModalService) { }
  fileToUpload: File = null;
  xmlFileName: string = "";
  upgradeButtonActive: boolean = this._rtuConfiguration.upgradeButtonActive;
  modalRef: BsModalRef;
  modalConfig = {
    backdrop: true,
    ignoreBackdropClick: false,
    class: 'modal-lg'
  };

  @ViewChild('modalSelectRtuXmlFile', { static: true }) public selectRtuXmlFile: TemplateRef<any>;
  @ViewChild('modalVerifyClearConfigAction', { static: true }) public modalVerifyClearConfigAction: TemplateRef<any>;
  @ViewChild('modalVerifyLoadConfigAction', { static: true }) public modalVerifyLoadConfigAction: TemplateRef<any>;
  @ViewChild('modalVerifyLoadrtuxmlAction', { static: true }) public modalVerifyLoadrtuxmlAction: TemplateRef<any>;
  @ViewChild('modalUpgradeConfigFileAction', { static: true }) public modalUpgradeConfigFileAction: TemplateRef<any>;
  @ViewChild('modalSelectupgradeConfigFile', { static: true }) public selectUpgradeRtuXmlFile: TemplateRef<any>;
  @ViewChild('modalunavailableupgradeConfigFile', { static: true }) public unavailableUpgradeRtuXmlFile: TemplateRef<any>;

  ngOnInit() {
    this.homeCards = [];
    // Configure network
    this.homeCards.push({
      name: 'Host Network Settings',
      description: 'Configure the 8810 RTU to connect to your host network',
      icon: 'hostNetwordClass',
      action: 'Edit Config',
      routerLinkPath: "/settings/networkConfiguration",
      info: '',
      infoimg: '',
      disabledCard: 'false',
      externalLink: ''
    });

    // Admin Settings
    this.homeCards.push({
      name: 'Admin Settings',
      description: 'Configure the 8810 RTU Network, Device Network, Admin User, and Date & Time settings',
      icon: 'adminIconClass',
      action: 'Manage Settings',
      routerLinkPath: "/settings",
      info: '',
      infoimg: '',
      disabledCard: 'false',
      externalLink: ''
    });

    // Configure chassis
    this.homeCards.push({
      name: 'Chassis Setup and  Module Configuration',
      description: 'Configure the 8810 RTU Chassis, Modules, Tanks, Alarms, and Modbus connections',
      icon: 'chassisIconClass',
      action: 'Configure Device',
      routerLinkPath: '/chassis',
      info: 'Chassis Module Tanks Alarms Modbus',
      infoimg: 'rtusmall.png',
      disabledCard: 'false',
      externalLink: ''
    });

    // Configure tanks
    this.homeCards.push({
      name: 'RTU Tanks',
      description: 'Configure storage tanks for the 8810 RTU',
      icon: 'tankIconClass',
      action: 'Manage Tanks',
      routerLinkPath: '/tank',
      info: '',
      infoimg: '',
      disabledCard: 'false',
      externalLink: ''
    });

    // System Admin Setup
    this.homeCards.push({
      name: 'System Admin Commands',
      description: 'Perform Admin actions: reboot, reset to factory settings, backup database, ...',
      icon: 'systemadminIconClass',
      action: 'Admin',
      routerLinkPath: "/systemadmin",
      info: '',
      infoimg: '',
      disabledCard: 'false',
      externalLink: ''
    });

    // Diagnostics
    this.homeCards.push({
      name: 'RTU Diagnostics',
      description: 'Manage Diaganostics for your 8810 RTU',
      icon: 'diagnosticsIconClass',
      action: 'Monitor System',
      routerLinkPath: '',//'/diagnostics',
      info: '',
      infoimg: '',
      disabledCard: 'true',
      externalLink: ''
    });

    // Alarm Manager
    this.homeCards.push({
      name: 'RTU Alarms',
      description: 'Configure and Manage 8810 RTU Alarms',
      icon: 'alarmIconClass',
      action: 'Manage Alarms',
      routerLinkPath: '/alarm',//'/alarms',
      info: '',
      infoimg: '',
      disabledCard: 'false',
      externalLink: ''
    });

    // Modbus Manager
    this.homeCards.push({
      name: 'Modbus Manager',
      description: 'Configure and manage 8810 RTU Modbus connections',
      icon: 'modbusIconClass',
      action: 'Manage Modbus',
      routerLinkPath: '/modbus',//'/modbus',
      info: '',
      infoimg: '',
      disabledCard: 'false',
      externalLink: ''
    });

    // Certificate Manager
    this.homeCards.push({
      name: 'Certificate Manager',
      description: 'Configure and manage 8810 RTU Certificates',
      icon: 'certificateIconClass',
      action: 'Manage Certificates',
      routerLinkPath: '/certificate',//'/modbus',
      info: '',
      infoimg: '',
      disabledCard: 'false',
      externalLink: ''
    });

    // Documentation
    this.homeCards.push({
      name: 'Documentation',
      description: 'Access VeRTUe documentation',
      icon: 'documenationIconClass',
      action: 'View Documentation',
      routerLinkPath: '',
      info: '',
      infoimg: '',
      disabledCard: 'false',
      externalLink: '8810_Vertue.pdf'
    });

    // The state of the upgrade button when going between views
    if (this._rtuConfiguration.getdefaultblankConfiguration() === true) {
      this.upgradeButtonActive = false;
    }
    else {
      this.upgradeButtonActive = true;
    }
  }

  handleFileInput(files: FileList) {
    this.fileToUpload = files.item(0);
    const file = document.getElementById('file');

    // change the input type from text and back to file to clear the filelist so that everytime the file loads
    file.attributes['type'].value = 'text';
    file.attributes['type'].value = 'file';

    let fileReader = new FileReader();

    fileReader.onload = (e) => {

      if (this._rtuConfiguration.set(fileReader.result.toString())) {
        this.ShowLoaded = true;
        setTimeout(() => { this.ShowLoaded = false; }, 2500);
        fileReader.abort();
        this._rtuConfiguration.setActiveDiagnosticView('');

        if ( this._rtuConfiguration.canUpgradeXMLVersion === true) {
          this.upgradeButtonActive = true;
        }
        else{
          this.upgradeButtonActive = false;
        }
      }
      else {
        this.ShowError = true;
        setTimeout(() => { this.ShowError = false; }, 2500);
      }
    };
    fileReader.readAsText(this.fileToUpload);
  }

// verify action prompt
  VerifyLoadConfigPrompt(): void {
    if (this._rtuConfiguration.checkForRTUConfigChanges())
      this.modalRef = this._modalService.show(this.modalVerifyLoadConfigAction, this.modalConfig);
    else
      this.selectRtuConfig();
  }

  public selectRtuConfig = function () {
    document.getElementById('file').click();
    if (this.modalRef !== undefined){
      this.modalRef.hide();
    }
  };

  // verify action prompt
  VerifyClearConfigPrompt(): void {
    if (this._rtuConfiguration.checkForRTUConfigChanges()) {
      this.modalRef = this._modalService.show(this.modalVerifyClearConfigAction, this.modalConfig);
    }
    else
      this.clearRtuConfig();
  }

  public clearRtuConfig() {
    this._rtuConfiguration.reset();
    this._rtuConfiguration.setActiveDiagnosticView('');
    this.ShowCleared = true;
    setTimeout(() => { this.ShowCleared = false; }, 2500);
    this.upgradeButtonActive = false;
    this._rtuConfiguration.setdefaultblankConfiguration(true);
    if (this.modalRef !== undefined)
      this.modalRef.hide();
  }

  // verify action prompt
  VerifyloadRTUXMLFilePrompt(): void {
    if (this._rtuConfiguration.checkForRTUConfigChanges()) {
      this.modalRef = this._modalService.show(this.modalVerifyLoadrtuxmlAction, this.modalConfig);
    }
    else{
      this.loadRtuXmlFile();
    }
  }

  public loadRtuXmlFile() {
    if (this.modalRef !== undefined)
      this.modalRef.hide();

    // set the default to the first file in the array
    if (this._rtuConfiguration.AvailableXmlFiles.length > 0) {
      this.xmlFileName = this._rtuConfiguration.AvailableXmlFiles[0];
    }
    this.modalRef = this._modalService.show(this.selectRtuXmlFile, this.modalConfig);
  }

  // verify action prompt
  VerifyUpgradeConfigFilePrompt(): void {
    if (this._rtuConfiguration.checkForRTUConfigChanges()) {
      this.modalRef = this._modalService.show(this.modalUpgradeConfigFileAction, this.modalConfig);
    }
    else {
      this.modalRef = this._modalService.show(this.modalUpgradeConfigFileAction, this.modalConfig);
    }
  }

  // Dropdown with all the viable XML options for the user to choose
  public dropdownOfXMLVersionsForUpgrade() {
    if (this.modalRef !== undefined) {
      this.modalRef.hide();
    }

      // Set the default to the first file in the array then show available options in dropdpwn
      if (this._rtuConfiguration.canUpgradeXMLVersion === true) {
        this.xmlFileName = this._rtuConfiguration.viableXMLUpgradeVersions[0];
        this.modalRef = this._modalService.show(this.selectUpgradeRtuXmlFile, this.modalConfig);
      }
      // There is no need to upgrade because the user is working with the latest version.
      else if (this._rtuConfiguration.canUpgradeXMLVersion === false) {
        this.modalRef = this.modalRef = this._modalService.show(this.unavailableUpgradeRtuXmlFile, this.modalConfig);
      }
  }

  // Kicks off config file configuration upgrade
  public upgradeConfigFileSelected() {
    const self = this;
    this.modalRef.hide();
    this._rtuConfiguration.kickoffUpgradeConfigFile(this.xmlFileName);
    window.setTimeout ( () => { self.isUpgrading(); } , 1000);
  }

  public isUpgrading() {
    const self = this;
    if ( this._rtuConfiguration.isUpgradingConfiguration() ) {
      window.setTimeout ( () => { self.isUpgrading(); } , 1000);
    } else {
      this.ShowUpgrade = true;
      setTimeout(() => { this.ShowUpgrade = false; }, 2500);
    }
  }
  public saveToRTUSelectedXmlFile() {
    console.log(this.xmlFileName);
    this.modalRef.hide();
    this._rtuConfiguration.loadXmlConfiguration(this.xmlFileName, false);
  }

  public isRTUNotDisconnected() {
    if (this._rtuConfiguration.connectionStatus === RTUConnectionStatus.DISCONNECTED) {
      return false;
    }
    else {
      return true;
    }
  }
}

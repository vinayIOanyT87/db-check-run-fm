import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { RtuconfigurationService, IRTUConfiguration } from 'src/app/services/rtuconfiguration.service';
import { IParameter } from 'src/app/services/availablemodules.service';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
import { Subscription } from 'rxjs';

export interface ICommandCards {
  id: number;
  name: string;
  header: string;
  img: string;
  button: string;
}

@Component({
  selector: 'app-systemadmin-view',
  templateUrl: './systemadmin-view.component.html',
  styleUrls: ['./systemadmin-view.component.css']
})
export class SystemadminViewComponent implements OnInit {
  rtuConfigurationSubsciption : Subscription;
  searchTankToggle = false;
  searchTankString =  '';
  rtuconfiguration: IRTUConfiguration;
  ModuleCommand: IParameter;
  CommandStatus: IParameter;
  ModuleCommandList: string[] = [];
  CommandStatusList: string[] = [];
  CommandCards: ICommandCards[] = [];
  selectedCommand: string = '';

  commandStatusMsg = '';
  
  commandBehaviour = [
    {id: 'Copy FW to RTU', header: 'Apply New Firmware', img: 'apply firmware.png', button: 'Apply Firmware'},
    {id: 'Reset Module', header: 'Reset CPU Module', img: 'reboot rtu.png', button: 'Reset CPU'},
    {id: 'Copy DB to RTU', header: 'Apply New Database to RTU', img: 'restore db.png', button: 'Apply New DB'},
    {id: 'Copy DB to USB', header: 'Backup Database to USB Drive', img: 'db to usb.png', button: 'Backup DB'},
    {id: 'Factory Reset', header: 'Factory Reset', img: 'factory reset.png', button: 'Factory Reset'},
    {id: 'Lim Fac Reset', header: 'Factory Reset Limited', img: 'factory reset.png', button: 'Factory Reset Limited' },
    {id: 'Password Reset', header: 'Password Reset', img: 'reset pwd.png', button: '' }
  ];
  
  modalRef: BsModalRef;
  modalConfig = {
    backdrop: true,
    ignoreBackdropClick: false,
    class: 'modal-lg'
  };

  @ViewChild('modalPendingChanges', { static: true }) public modalPendingChanges: TemplateRef<any>;

  constructor( private _rtuConfiguration: RtuconfigurationService,
    private _modalService: BsModalService ) { }

  ngOnInit() {
    let instance = this;
    this.commandStatusMsg = '';
    let oldCommandStatus = null;

     this.rtuConfigurationSubsciption = this._rtuConfiguration.get().subscribe( data => {
      if ( data.RTUConfiguration ) {
        this.rtuconfiguration = data.RTUConfiguration;
        instance.ModuleCommand = null;
        oldCommandStatus = instance.CommandStatus;
        instance.CommandStatus = null;
        instance.ModuleCommandList = [];
        instance.CommandStatusList = [];   
        instance.CommandCards = [];
        instance.commandStatusMsg = '';
      

        let moduleIdentifier = Object.keys(instance.rtuconfiguration.module0.moduleConfiguration).find(s => instance.rtuconfiguration.module0.moduleConfiguration[s].parameter === 'ModCmd');
        if ( moduleIdentifier ) {
          instance.ModuleCommand = instance.rtuconfiguration.module0.moduleConfiguration[moduleIdentifier];
          instance.ModuleCommandList = instance.ModuleCommand.availableCommands.split(',').map(function(item) {
                                                                                      return item.trim();
                                                                                    });
        }

        moduleIdentifier = Object.keys(instance.rtuconfiguration.module0.moduleConfiguration).find(s => instance.rtuconfiguration.module0.moduleConfiguration[s].parameter === 'CmdStatus');
        if ( moduleIdentifier ) {
          instance.CommandStatus = instance.rtuconfiguration.module0.moduleConfiguration[moduleIdentifier];
          instance.CommandStatusList = instance.CommandStatus.availableCommands.split(',').map(function(item) {
                                                                                      return item.trim();
                                                                                    });
          if(instance.ModuleCommand.value != '0'){
            if ( parseInt( instance.CommandStatus.value ) <= instance.CommandStatusList.length ) {
              instance.CommandStatusList.unshift('Undefined');                                                                                    
            }

            instance.commandStatusMsg = instance.CommandStatusList[ parseInt( instance.CommandStatus.value ) ];
          }
          else {
            instance.commandStatusMsg = '';
          }

          if ( !oldCommandStatus
          || oldCommandStatus.identifier !== instance.CommandStatus.identifier) {
            if(oldCommandStatus){
              const parameterList = [];
              parameterList.push(oldCommandStatus);
              parameterList.push(instance.ModuleCommand);
              this._rtuConfiguration.unsubscribeRealtimeParameters( parameterList );
            }
            const parameterList = [];
            parameterList.push(instance.CommandStatus);
            parameterList.push(instance.ModuleCommand);
            this._rtuConfiguration.subscribeRealtimeParameters( parameterList );
          }
        }

        // populate the card to display in the left side
        instance.ModuleCommandList.forEach( function(command, i) {
          let tempDescription = instance.commandBehaviour.find(s => s.id === command);
          if ( tempDescription ) {
            instance.CommandCards.push(  {
              id: i + 1,
              name: command,
              header: tempDescription.header,
              img: tempDescription.img,
              button: tempDescription.button
            });
          }
        })

        // add the password reset card (not a command in the rtu)
        let tempDescription = instance.commandBehaviour.find(s => s.id === 'Password Reset');
        if (tempDescription) {
          instance.CommandCards.push({
            id: instance.ModuleCommandList.length + 1,
            name: 'Password Reset',
            header: tempDescription.header,
            img: tempDescription.img,
            button: tempDescription.button
          });
        }
      } else {
        instance.rtuconfiguration = null;
        instance.ModuleCommand = null;
        instance.CommandStatus = null;
        instance.commandStatusMsg = '';
        instance.ModuleCommandList = [];
        instance.CommandStatusList = [];   
        instance.CommandCards = [];
      }
     });
  }

  ngOnDestroy(){
    this.rtuConfigurationSubsciption.unsubscribe();
    const parameterList = [];
    parameterList.push(this.CommandStatus);
    parameterList.push(this.ModuleCommand);
    this._rtuConfiguration.unsubscribeRealtimeParameters( parameterList );
  }

  selectCommand( command ) { 
    if(command.button === 'Backup DB'
    && this._rtuConfiguration.checkForRTUConfigChanges()){
      this.modalRef = this._modalService.show( this.modalPendingChanges , this.modalConfig);
      return;
    }

    this.selectedCommand =  command;
  }
}

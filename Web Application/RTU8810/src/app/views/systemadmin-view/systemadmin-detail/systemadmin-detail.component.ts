import { Component, OnInit, Input,TemplateRef, ViewChild } from '@angular/core';
import { ICommandCards } from '../systemadmin-view.component';
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';
import * as saveAs from 'node_modules/file-saver'
import { IParameter } from 'src/app/services/availablemodules.service';
import { RtuconfigurationService, IRTUConfiguration } from 'src/app/services/rtuconfiguration.service';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-systemadmin-detail',
  templateUrl: './systemadmin-detail.component.html',
  styleUrls: ['./systemadmin-detail.component.css']
})
export class SystemadminDetailComponent implements OnInit {
  rtuConnectionStatusSubscription: Subscription;
  @Input() selectedCommand: ICommandCards = null;
  @Input() commandStatusMsg: string = '';
  @Input() moduleCommand: IParameter;
  applyIssued: boolean = false;

  status = RTUConnectionStatus.DISCONNECTED;

  modalRef: BsModalRef;
  modalConfig = {
    backdrop: true,
    ignoreBackdropClick: false,
    class: 'modal-lg'
  };

  @ViewChild('modalVerifyissuanceofCommand', { static: true }) public modalVerifyissuanceofCommand: TemplateRef<any>;

  constructor( private _RtuconnectionstatusService: RtuconnectionstatusService,
    private _rtuConfiguration: RtuconfigurationService,
    private _modalService: BsModalService) { }
    

  ngOnInit() {
    const self = this;
    // subscribe to the rtu connection status service
    this.rtuConnectionStatusSubscription = this._RtuconnectionstatusService.get().subscribe( data => {
      self.status = data;
    });

  }

  ngOnDestroy(){
    this.rtuConnectionStatusSubscription.unsubscribe();
  }

  isSystemOnline() {
    return this.status === RTUConnectionStatus.CONNECTED || this.status === RTUConnectionStatus.ERRORREADING;
  }

  canCreateFile() {
    if (this.selectedCommand.name === 'Factory Reset' || this.selectedCommand.name === 'Lim Fac Reset' || this.selectedCommand.name === 'Password Reset') {
      return true;
    }
    return false;
  }

  executeCommand() {
    if ( this.moduleCommand ) {
      // verify that the user actually wants to issue the command. This was decided to be added
      this.modalRef = this._modalService.show( this.modalVerifyissuanceofCommand , this.modalConfig);
    }
  }

  doexecuteCommand()
  {
    if(this.modalRef !== undefined)
      this.modalRef.hide();
    this.moduleCommand.pendingValue = this.selectedCommand.id.toString();
    this._rtuConfiguration.applyCommandToRTU( this.moduleCommand );
    this.applyIssued = true;
  }

  getCommandStatusMsg(){
    if(this.applyIssued){
      return this.commandStatusMsg;
    }
    else{
      return '';
    }

  }

  generateFile() {
    let content = '';
    let filename = '';
    if (this.selectedCommand.name === 'Factory Reset') {
      content = 'Factory Reset';
      filename = 'dbreset';
    } else if (this.selectedCommand.name === 'Lim Fac Reset') {
      content = 'Factory Reset Limited';
      filename = 'dbreset';
    } else if (this.selectedCommand.name === 'Password Reset') {
      content = 'Password Reset';
      filename = 'pwreset';
    }

    if ( content !== '') {
      var data = new Blob([content], {type: 'application/octet-stream'});

      if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        window.navigator.msSaveOrOpenBlob(data, filename);
      } else {
        saveAs(data, filename);
      }
    }
  }
}

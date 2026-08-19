import { Component, OnInit, ViewEncapsulation, HostListener } from '@angular/core';
import { AvailablemodulesService, IAvailableModule, IAvailableConfiguration } from 'src/app/services/availablemodules.service';
import { RtuconfigurationService, IRTUConfigurationService } from 'src/app/services/rtuconfiguration.service';
import { SelectedmodulechannelService, ISelectedModuleChannel } from 'src/app/services/selectedmodulechannel.service';
import { NotificationService, INotification } from './services/notification.service';
import { RtuconnectService } from 'src/app/services/rtuconnect.service';
import { Subscription } from 'rxjs';

// to make pnotify work with IE we need this workaround: https://github.com/sciactive/pnotify/issues/343
declare var PNotifyButtons: any;
declare var PNotify: any;
declare var PNotifyCallbacks: any;
/*
import PNotify from 'pnotify/dist/iife/PNotify';
import PNotifyButtons from 'pnotify/dist/iife/PNotifyButtons';
import PNotifyCallbacks from 'pnotify/dist/iife/PNotifyCallbacks';
*/

@Component({
  selector: 'app-root-rtu8810',
  templateUrl: './app.component.html',
  styleUrls: [
    './app.component.css',
    '../../node_modules/bootstrap/dist/css/bootstrap.css'],
  encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit {
  availableModuleSubscription : Subscription;
  rtuConfgiurationSubscription : Subscription;
  selectedModuleSubscription : Subscription;
  notificationSubscription : Subscription;

  title = 'VeRTUe';
  availableConfiguration: IAvailableConfiguration;
  rtuconfiguration: IRTUConfigurationService;
  selectedModuleChannel: ISelectedModuleChannel = { selectedModule: 0, moduleConfigured: 'CPU', selectedChannel: 0, protocol: null};
  stackBottomRight = { 'dir1': 'up', 'dir2': 'left', 'firstpos1': 25, 'firstpos2': 25 };

  @HostListener('window:beforeunload', [ '$event' ])
  beforeUnloadHander($event) {
    $event.returnValue = "Have you saved the RTU Configuration and are you sure you want to exit?";
  }

  constructor( private _availableModuleService: AvailablemodulesService,
    private _rtuConfiguration: RtuconfigurationService,
    private _selectedModuleChannel: SelectedmodulechannelService,
    private _notificationService: NotificationService,
    private _rtuInitialConnectionService: RtuconnectService) {
  }

  ngOnInit() {
    // initialize pNotify
    // tslint:disable-next-line:no-unused-expression
    PNotifyButtons;
    // tslint:disable-next-line:no-unused-expression
    PNotifyCallbacks;
    PNotify.defaults.styling = 'bootstrap4'; // Bootstrap version 4
    PNotify.defaults.icons = 'fontawesome4'; // Font Awesome 4
    PNotify.defaults.width = '400px';

    this.availableModuleSubscription = this._availableModuleService.getAll().subscribe( data => this.availableConfiguration = data);
    this.rtuConfgiurationSubscription = this._rtuConfiguration.get().subscribe( data => this.rtuconfiguration = data);
    this.selectedModuleSubscription = this._selectedModuleChannel.get().subscribe( data => this.selectedModuleChannel);
    this.notificationSubscription = this._notificationService.get().subscribe( data => this.onNotificationMessage( data ) );
    this._rtuInitialConnectionService.init();
  }

  ngOnDestroy(){
    this.availableModuleSubscription.unsubscribe();
    this.rtuConfgiurationSubscription.unsubscribe();
    this.selectedModuleSubscription.unsubscribe();
    this.notificationSubscription.unsubscribe();
  }

  onNotificationMessage( data: INotification[] ) {
    const notificationService = this._notificationService;
    const stackBottomRight = this.stackBottomRight;

    if ( data && data.length > 0) {
      data.map( function(value, index) {
        if ( value.removePreviousNotifications ) {
          PNotify.closeAll();
        }

        if (value.type === 'error') {
          const errorNotice = PNotify.error({
            title: value.header,
            text: value.text,
            cornerClass: 'ui-pnotify-sharp',
            hide: false,
            stack: stackBottomRight,
            modules: {
              Callbacks: {
                afterOpen: function (notice) {
                  console.log('afterOpen', notice);
                  setTimeout(function () { notice.addModuleClass('ui-pnotify-translucent'); }, 5000);
                }
              },
              Buttons: {
                sticker: false
              }
            }
          });
          notificationService.processed( value.id );

        } else if (value.type === 'success') {
          const successNotice = PNotify.success({
            title:  value.header,
            text: value.text,
            cornerClass: 'ui-pnotify-sharp',
            stack: stackBottomRight,
            modules: {
              Callbacks: {
                afterOpen: function (notice) {
                  console.log('afterOpen', notice);
                  setTimeout(function () { notice.addModuleClass('ui-pnotify-translucent'); }, 5000);
                }
              },
              Buttons: {
                sticker: false
              }
            }
          });
          notificationService.processed( value.id );
        }
      });

    }
  }

}

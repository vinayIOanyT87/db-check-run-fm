import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { DragDropModule} from '@angular/cdk/drag-drop';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { ContentComponent } from './content/content.component';
import { ContentHeaderComponent } from './content/content-header/content-header.component';
import { ContentDetailComponent } from './content/content-detail/content-detail.component';
import { ChassisViewComponent } from './views/chassis-view/chassis-view.component';
import { TankViewComponent } from './views/tank-view/tank-view.component';
import { CertificateViewComponent } from './views/certificate-view/certificate-view.component';
import { AlarmViewComponent } from './views/alarm-view/alarm-view.component';
import { ModbusViewComponent } from './views/modbus-view/modbus-view.component';
import { HomeViewComponent } from './views/home-view/home-view.component';
import { ConnectComponent } from './views/home-view/connect/connect.component';
import { HomeMenuComponent } from './menus/home-menu/home-menu.component';
import { AssetsMenuComponent } from './menus/assets-menu/assets-menu.component';
import { DiagnosticsMenuComponent } from './menus/diagnostics-menu/diagnostics-menu.component';
import { SettingsMenuComponent } from './menus/settings-menu/settings-menu.component';
import { ChassismoduleviewComponent } from './views/chassis-view/chassismoduleview/chassismoduleview.component';
import { ModuleconfigurationComponent } from './views/chassis-view/moduleconfiguration/moduleconfiguration.component';
import { ModuledetailComponent } from './views/chassis-view/moduledetail/moduledetail.component';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HomeCardComponent } from './views/home-view/home-card/home-card.component';
import { SettingsViewComponent } from './views/settings-view/settings-view.component';
import { NetworkconfigurationComponent } from './views/settings-view/networkconfiguration/networkconfiguration.component';
import { DiagnosticsViewComponent } from './views/diagnostics-view/diagnostics-view.component';
import { DiagnosticsviewpickerComponent } from './views/diagnostics-view/diagnosticsviewpicker/diagnosticsviewpicker.component';
import { DiagnosticstagviewerComponent } from './views/diagnostics-view/diagnosticstagviewer/diagnosticstagviewer.component';
import { NgxMaskModule } from 'ngx-mask';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatMenuModule} from '@angular/material/menu';
import {ScrollingModule} from '@angular/cdk/scrolling';

import { SystemadminViewComponent } from './views/systemadmin-view/systemadmin-view.component';
import { SystemadminCardComponent } from './views/systemadmin-view/systemadmin-card/systemadmin-card.component';
import { SystemadminDetailComponent } from './views/systemadmin-view/systemadmin-detail/systemadmin-detail.component';
import { SystemadminDescriptionComponent } from './views/systemadmin-view/systemadmin-description/systemadmin-description.component';
import { AdminUserConfigurationComponent } from './views/settings-view/admin-user-configuration/admin-user-configuration.component';
import { AlarmmanagerComponent } from './views/alarm-view/alarmmanager/alarmmanager.component';
import { RegisterMapComponent } from './views/modbus-view/register-map/register-map.component';

// bootstrap
import { AccordionModule, BsDropdownModule, ModalModule, ButtonsModule } from 'ngx-bootstrap';
import { TypeaheadModule } from 'ngx-bootstrap/typeahead';
import { BsModalService } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap';
import { TimepickerModule } from 'ngx-bootstrap';


// services
import { RtuconfigurationService } from './services/rtuconfiguration.service';
import { AvailablemodulesService } from './services/availablemodules.service';
import { SelectedmodulechannelService } from './services/selectedmodulechannel.service';
import { LocalStorageService } from './services/localstorage.service';
import { NotificationService } from './services/notification.service';
import { RtuconnectService } from './services/rtuconnect.service';
import { RtuconnectionstatusService } from './services/rtuconnectionstatus.service';

// custom scrollbar
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { InlineconfigurationeditorComponent } from './views/chassis-view/inlineconfigurationeditor/inlineconfigurationeditor.component';

// ngx-datatable
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

// ngx-drag-to-select
import { DragToSelectModule } from 'ngx-drag-to-select';

// directives
import { AllowOnlyDigitsDirective } from './directives/allow-only-digits.directive';
import { RestricMinMaxDirective } from './directives/restric-min-max.directive';
import { AllowOnlySignedDigitsDirective } from './directives/allow-only-signed-digits.directive';
import { AllowOnlyDecimalsDirective } from './directives/allow-only-decimals.directive';
import { AllowOnlyHexValuesDirective } from './directives/allow-only-hex-values.directive';
import { TankmanagerComponent } from './views/tank-view/tankmanager/tankmanager.component';
import { ConnectHeaderComponent } from './content/content-header/connect-header/connect-header.component';
import { CertificateManagerComponent } from './views/certificate-view/certificatemanager/certificatemanager.component';


const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

@NgModule({
  declarations: [
    AppComponent,
    SidebarComponent,
    ContentComponent,
    ContentHeaderComponent,
    ContentDetailComponent,
    ChassisViewComponent,
    TankViewComponent,
    CertificateViewComponent,
    AlarmViewComponent,
    ModbusViewComponent,
    HomeViewComponent,
    ConnectComponent,
    HomeMenuComponent,
    AssetsMenuComponent,
    DiagnosticsMenuComponent,
    SettingsMenuComponent,
    ChassismoduleviewComponent,
    ModuleconfigurationComponent,
    ModuledetailComponent,
    HomeCardComponent,
    SettingsViewComponent,
    NetworkconfigurationComponent,
    InlineconfigurationeditorComponent,
    AllowOnlyDigitsDirective,
    RestricMinMaxDirective,
    AllowOnlySignedDigitsDirective,
    AllowOnlyDecimalsDirective,
    AllowOnlyHexValuesDirective,
    DiagnosticsViewComponent,
    DiagnosticsviewpickerComponent,
    DiagnosticstagviewerComponent,
    TankmanagerComponent,
    ConnectHeaderComponent,
    SystemadminViewComponent,
    SystemadminCardComponent,
    SystemadminDetailComponent,
    SystemadminDescriptionComponent,
    ConnectHeaderComponent,
    AdminUserConfigurationComponent,
    AlarmmanagerComponent,
    RegisterMapComponent,
    CertificateManagerComponent
  ],
  imports: [
    BrowserModule,
    AccordionModule.forRoot(),
    BsDropdownModule.forRoot(),
    ModalModule.forRoot(),
    ButtonsModule.forRoot(),
    TypeaheadModule.forRoot(),
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    DragDropModule,
    PerfectScrollbarModule,
    FormsModule,
    NgxMaskModule.forRoot(),
    NgxDatatableModule,
    ReactiveFormsModule,
    NgbModule,
    MatMenuModule,
    ScrollingModule,
    BsDatepickerModule.forRoot(),
    DragToSelectModule.forRoot({
      selectedClass: 'my-selected-item',
      shortcuts: {
        toggleSingleItem: 'alt'
      }
    }),
    TimepickerModule.forRoot()
      ],
  providers: [AvailablemodulesService,
    RtuconfigurationService,
    SelectedmodulechannelService,
    LocalStorageService,
    NotificationService,
    RtuconnectService,
    RtuconnectionstatusService,
    BsModalService,
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }],
  bootstrap: [AppComponent],
  schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
})
export class AppModule { }

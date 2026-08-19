import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { KeepPingingService } from './services/keep-pinging.service';
import { LocalStorageService } from './services/local-storage.service';
import { ServerConfigService } from './services/server-config.service';
import { SiteService } from './services/site.service';
import { TransactionService } from './services/transaction.service';
import { MeterService } from './services/meter.service';
import { VCFService } from './services/vcf.service';
import { FocusDirective } from './directives/focus.directive';
import { EnterStopPropogationDirective } from './directives/enter-stop-propogation.directive';
import { FMLoadingScreenComponent } from './webparts/fm-loading-screen/fm-loading-screen.component';
import { FieldHostDirective } from './directives/field-host.directive';
import { TriggerChangeDetectionOnKeyUpDirective } from './directives/trigger-change-detection-on-key-up.directive';
import { TriggerFocusOnTabToNextElementDirective } from './directives/trigger-focus-on-tab-to-next-element.directive';
import { MatDialogModule } from '@angular/material/dialog';


@NgModule({
  imports: [
    CommonModule, HttpClientModule, MatDialogModule
  ],
  // exports: [KeepPingingService, LocalStorageService, ServerConfigService,
  //   SiteService, TransactionService, VCFService],
  exports: [FMLoadingScreenComponent, FocusDirective,
    EnterStopPropogationDirective, FieldHostDirective,
    TriggerChangeDetectionOnKeyUpDirective, TriggerFocusOnTabToNextElementDirective],
  declarations: [FMLoadingScreenComponent, FocusDirective,
    EnterStopPropogationDirective, FieldHostDirective,
    TriggerChangeDetectionOnKeyUpDirective, TriggerFocusOnTabToNextElementDirective]
})
export class FMCoreModule { }

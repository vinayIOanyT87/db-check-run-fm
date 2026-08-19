import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';



export interface ISelectedModuleChannel {
  selectedModule: number;
  moduleConfigured: string;
  selectedChannel: number;
  protocol: string;
}

@Injectable({
  providedIn: 'root'
})
export class SelectedmodulechannelService {
  SelectedModuleChannel: Observable<ISelectedModuleChannel>;
  private _selectedModuleChannel: BehaviorSubject<ISelectedModuleChannel>;
  private assetUrl: string;
  private dataStore: {
    selectedModuleChannel: ISelectedModuleChannel;
  };

  constructor() {
    this.dataStore = { selectedModuleChannel: <ISelectedModuleChannel> {
      selectedModule: 0,
      moduleConfigured: 'CPU',
      selectedChannel: 0,
      protocol: null
    } };
    this._selectedModuleChannel = <BehaviorSubject<ISelectedModuleChannel>>new BehaviorSubject(this.dataStore.selectedModuleChannel);
    this.SelectedModuleChannel = this._selectedModuleChannel.asObservable();
   }

   get() {
    return this._selectedModuleChannel.asObservable();
  }

   selectedModule( module: number, moduleConfigured: string) {
    this.dataStore.selectedModuleChannel.selectedModule = module;
    this.dataStore.selectedModuleChannel.moduleConfigured = moduleConfigured;
    this.dataStore.selectedModuleChannel.selectedChannel = 0;
    this.dataStore.selectedModuleChannel.protocol = null;
    this._selectedModuleChannel.next(this.dataStore.selectedModuleChannel);
   }

   selectedChannel( channel: number, protocol: string) {
    this.dataStore.selectedModuleChannel.selectedChannel = channel;
    this.dataStore.selectedModuleChannel.protocol = protocol;
    this._selectedModuleChannel.next(this.dataStore.selectedModuleChannel);
   }

   selectedModuleChannel( module: number, moduleConfigured: string, channel: number, protocol: string) {
    this.dataStore.selectedModuleChannel.selectedModule = module;
    this.dataStore.selectedModuleChannel.moduleConfigured = moduleConfigured;
    this.dataStore.selectedModuleChannel.selectedChannel = channel;
    this.dataStore.selectedModuleChannel.protocol = protocol;
    this._selectedModuleChannel.next(this.dataStore.selectedModuleChannel);
   }
}

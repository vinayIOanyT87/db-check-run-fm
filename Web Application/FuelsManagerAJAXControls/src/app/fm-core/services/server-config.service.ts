import { Injectable } from '@angular/core';

declare var serverUrl: string;
declare var pingTimeout: number;

@Injectable({
  providedIn: 'root'
})
export class ServerConfigService {
  constructor() { }

  /**
   * EX: http://localhost/FMWebAPI/api
   * EX: ../../FMWebAPI/api
   */
  getServerUrl(): string {
      return serverUrl;
  }
  /**
   * how many minutes should we keep pinging before stoping?
   */
  getPingTimeout(): number {
    if ((window as { [key: string]: any })['pingTimeout'] === null || (window as { [key: string]: any })['pingTimeout'] === undefined) {
      return 120;
    }
    return pingTimeout;
  }
}

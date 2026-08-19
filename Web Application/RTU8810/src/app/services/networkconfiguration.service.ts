import { Injectable } from '@angular/core';
import * as saveAs from 'node_modules/file-saver'

@Injectable({
  providedIn: 'root'
})
export class NetworkconfigurationService {

  constructor() { }

  public saveNetworkConfigurationtoDisk(ip: string, subnet:string, gateway:string){

    var data = new Blob([ip+"\r\n"+subnet+"\r\n"+gateway], {type: 'application/octet-stream'});

    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
      window.navigator.msSaveOrOpenBlob(data, "ipconfig");
  } else {
    saveAs(data, "ipconfig");
  }


  }
}

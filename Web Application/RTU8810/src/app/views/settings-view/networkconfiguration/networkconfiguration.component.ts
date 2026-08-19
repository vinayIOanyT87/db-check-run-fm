import { Component, OnInit } from '@angular/core';
import {NetworkconfigurationService} from 'src/app/services/networkconfiguration.service';

@Component({
  selector: 'app-networkconfiguration',
  templateUrl: './networkconfiguration.component.html',
  styleUrls: ['./networkconfiguration.component.css']
})
export class NetworkconfigurationComponent implements OnInit {
  model: any = {};

  onSubmit() {
    alert('SUCCESS!! :-)\n\n' + JSON.stringify(this.model))
  }

  

  constructor( private networkconfigurationService: NetworkconfigurationService) { }
ip="";
subnet="";
gateway="";

  ngOnInit() {}


public SaveNetworkConfiguration(ip: string, subnet:string, gateway:string)
{
  this.networkconfigurationService.saveNetworkConfigurationtoDisk(ip, subnet, gateway);
}



}

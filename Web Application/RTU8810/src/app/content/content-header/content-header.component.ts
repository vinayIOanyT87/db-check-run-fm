import { Component, OnInit } from '@angular/core';
import { RtuconfigurationService } from 'src/app/services/rtuconfiguration.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-content-header',
  templateUrl: './content-header.component.html',
  styleUrls: ['./content-header.component.css']
})
export class ContentHeaderComponent implements OnInit {
  versionSubscription : Subscription;
  version:string = "1.0.1"

  constructor(
    private _rtuConfiguration: RtuconfigurationService,

  ) {
    

    
  }

  ngOnInit() {
  {

    this.versionSubscription = this._rtuConfiguration.getVersion().subscribe(data => {
       this.version = data;
      });
    
  }
  }

}

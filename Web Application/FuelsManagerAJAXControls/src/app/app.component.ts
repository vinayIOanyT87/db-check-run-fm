import { Component } from '@angular/core';
import { SiteService } from './fm-core/services/site.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(private siteService: SiteService) {  }
  title = 'FuelsManagerAJAXControls';
}

import { Component, OnInit } from '@angular/core';
import { RtuconfigurationService } from 'src/app/services/rtuconfiguration.service';

@Component({
  selector: 'app-content',
  templateUrl: './content.component.html',
  styleUrls: ['./content.component.css']
})
export class ContentComponent implements OnInit {

  constructor(private _rtuconfigurationService: RtuconfigurationService) { }

  ngOnInit() {
  }

}

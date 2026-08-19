import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-home-card',
  templateUrl: './home-card.component.html',
  styleUrls: ['./home-card.component.css']
})
export class HomeCardComponent implements OnInit {
  @Input() description: string;
  @Input() icon: string;
  @Input() name: string;
  @Input() action: string;
  @Input() info: string;
  @Input() detailimg: string;
  @Input() routerLinkPath: string;
  @Input() disabledCard: string;
  @Input() externalLink: string;

  constructor() { }

  ngOnInit() {
    console.log(this.externalLink);
  }

  public setHomeCardStyle(descriptionpassedin:string)
  {
    let styles = {};
  
    if(descriptionpassedin.toLowerCase() === 'true')
    {
      styles = { 'opacity':'.3','cursor': 'not-allowed' , 'box-shadow':'none'};
    }
  
    return styles;
  }
  
  }

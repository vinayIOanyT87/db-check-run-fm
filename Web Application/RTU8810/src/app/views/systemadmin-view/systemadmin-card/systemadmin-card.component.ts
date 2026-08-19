import { Component, OnInit, Input } from '@angular/core';
import { ICommandCards } from '../systemadmin-view.component';

@Component({
  selector: 'app-systemadmin-card',
  templateUrl: './systemadmin-card.component.html',
  styleUrls: ['./systemadmin-card.component.css']
})
export class SystemadminCardComponent implements OnInit {
  @Input() command: ICommandCards;

  constructor() { }

  ngOnInit() {
  }

}

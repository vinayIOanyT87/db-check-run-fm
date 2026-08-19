import { Component, OnInit, OnChanges, Input } from '@angular/core';
import { ICommandCards } from '../systemadmin-view.component';

export interface IAdminCommandDescription {
  id: string;
  img: string;
  header: string;
  message: string;
}

@Component({
  selector: 'app-systemadmin-description',
  templateUrl: './systemadmin-description.component.html',
  styleUrls: ['./systemadmin-description.component.css']
})
export class SystemadminDescriptionComponent implements OnInit, OnChanges {
  @Input() selectedCommand: ICommandCards;

  descriptions: IAdminCommandDescription[] = [{id: '', header: 'Choose a Command', img: '', message: 'To begin, select a command to execute from the list.'},
    { id: 'Copy FW to RTU', header: 'Apply New Firmware', img: 'apply firmware.png', message: 'Copies the firmware from the External USB Flash Drive to the RTU. The CPU Module will be reset if the copy is successful.'},
    { id: 'Reset Module', header: 'Reset CPU Module', img: 'reboot rtu.png', message: 'Equivalent to powercycling the RTU.'},
    { id: 'Copy DB to RTU', header: 'Apply New Database to RTU', img: 'restore db.png',message: 'Copies the RTU configuration from a directory with a name specified by DBFile on the External USB Flash Drive to the RTU.'},
    { id: 'Copy DB to USB', header: 'Backup Database to USB Drive', img: 'db to usb.png',message: 'Copies the RTU configuration from the RTU to a directory with a name specified by DBFile on the External USB Flash Drive.'},
    { id: 'Factory Reset', header: 'Factory Reset', img: 'factory reset.png', message: "Restores the RTU's configuration to its factory settings."},
    { id: 'Lim Fac Reset', header: 'Factory Reset Limited', img: 'factory reset.png', message: "Restores the RTU's configuration to its factory settings, except for IpAddress, SubnetMask, and Gateway, which are unchanged." },
    { id: 'Password Reset', header: 'Password Reset', img: 'reset pwd.png', message: "Restores the 8810 RTU’s primary usernames and passwords as well as the security configurations to their default values." }

  ]

  description: IAdminCommandDescription = this.descriptions[0];

  constructor() { }

  ngOnInit() {
      this.description = this.descriptions[0];
  }

  ngOnChanges() {
    let self = this;
    let tempDescription = self.descriptions.find(s => s.id === self.selectedCommand.name);
    if ( tempDescription ) {
      this.description = tempDescription;
    }
  }
}

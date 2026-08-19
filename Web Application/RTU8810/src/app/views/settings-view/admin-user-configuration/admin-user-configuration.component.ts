import { Component, OnInit } from '@angular/core';
import { RtuconfigurationService, IRTUConfiguration } from 'src/app/services/rtuconfiguration.service';
import { RtuconnectionstatusService, RTUConnectionStatus } from 'src/app/services/rtuconnectionstatus.service';

@Component({
  selector: 'app-admin-user-configuration',
  templateUrl: './admin-user-configuration.component.html',
  styleUrls: ['./admin-user-configuration.component.css']
})
export class AdminUserConfigurationComponent implements OnInit {
  model: any = {};
  username: string;
  password: string;
  confirmPassword: string;
  rtuconfiguration: IRTUConfiguration;
  private connectionStatus: RTUConnectionStatus;

  constructor(private _rtuConfiguration: RtuconfigurationService,
      private _RtuconnectionstatusService: RtuconnectionstatusService) {
    this._RtuconnectionstatusService.get().subscribe( data => this.connectionStatus = data);
  }

  ngOnInit() {
    this.getRTUConfiguration();
  }

  getRTUConfiguration(): any {
    this._rtuConfiguration.get().subscribe(data => {
      this.rtuconfiguration = data.RTUConfiguration;
    });
  }

  public Apply(username: string, password: string) {
    const usernameParameter = this.rtuconfiguration.module0.moduleConfiguration[Object.keys(this.rtuconfiguration.module0.moduleConfiguration).find(s => this.rtuconfiguration.module0.moduleConfiguration[s].parameter === 'AdminName')];
    const passwordParameter = this.rtuconfiguration.module0.moduleConfiguration[Object.keys(this.rtuconfiguration.module0.moduleConfiguration).find(s => this.rtuconfiguration.module0.moduleConfiguration[s].parameter === 'AdminPassword')];

    usernameParameter.pendingValue = username;
    passwordParameter.pendingValue = password;

    this._rtuConfiguration.applyCommandToRTU(usernameParameter, passwordParameter);

    this._rtuConfiguration.setAdminCredentials(username, password);

    return;
  }
}

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AlarmViewComponent } from './views/alarm-view/alarm-view.component';
import { TankViewComponent } from './views/tank-view/tank-view.component';
import { CertificateViewComponent } from './views/certificate-view/certificate-view.component';
import { ChassisViewComponent } from './views/chassis-view/chassis-view.component';
import { HomeViewComponent } from './views/home-view/home-view.component';
import { SettingsViewComponent } from './views/settings-view/settings-view.component';
import { NetworkconfigurationComponent } from './views/settings-view/networkconfiguration/networkconfiguration.component'
import { DiagnosticsViewComponent } from './views/diagnostics-view/diagnostics-view.component';
import { AdminUserConfigurationComponent } from './views/settings-view/admin-user-configuration/admin-user-configuration.component';
import { SystemadminViewComponent } from './views/systemadmin-view/systemadmin-view.component';
import { RegisterMapComponent } from './views/modbus-view/register-map/register-map.component';


const routes: Routes = [
    {
        path: 'home',
        component: HomeViewComponent,
    },
    {
        path: 'chassis',
        component: ChassisViewComponent,
    },
    {
        path: 'tank',
        component: TankViewComponent,
    },
    {
        path: 'certificate',
        component: CertificateViewComponent,
    },
    {
        path: 'alarm',
        component: AlarmViewComponent,
    },
    {
        path: 'modbus',
        component: RegisterMapComponent,
    },
    {
        path: 'systemadmin',
        component: SystemadminViewComponent,
    },
    {
        path: 'settings',
        component: SettingsViewComponent,
        children: [
            {
                path: '',
                redirectTo: 'networkConfiguration',
                pathMatch: 'full'
            },

            {
                path: 'networkConfiguration',
                component: NetworkconfigurationComponent,
            },
            {
                path: 'admin-User-Configuration',
                component: AdminUserConfigurationComponent,
            }
        ]
    },
    {
        path: 'settings/',
        redirectTo: 'settings', pathMatch: 'prefix'
    }
    ,
    {
        path: 'diagnostics',
        component: DiagnosticsViewComponent,
    },
    {
        path: '',
        redirectTo: '/home',
        pathMatch: 'full',
    },
    {
        path: '**',
        redirectTo: '/home',
        pathMatch: 'full',
    }

];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }

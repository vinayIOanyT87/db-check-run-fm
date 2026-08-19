import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import { SiteService } from './app/fm-core/services/site.service';

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(AppModule)
  .then((module) => {
    // so we can mess with it in the console
    // siteService.login('aguirrej2', '', 'msp - swissport').subscribe()
    (<any>window)['siteService'] = module.injector.get(SiteService);
  })
  .catch(err => console.error(err));

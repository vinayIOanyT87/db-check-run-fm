import { TestBed } from '@angular/core/testing';

import { RtuconnectionstatusService } from './rtuconnectionstatus.service';

describe('RtuconnectionstatusService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: RtuconnectionstatusService = TestBed.get(RtuconnectionstatusService);
    expect(service).toBeTruthy();
  });
});

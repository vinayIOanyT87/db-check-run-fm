import { TestBed } from '@angular/core/testing';

import { RtuconnectService } from './rtuconnect.service';

describe('RtuinitialconnectionService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: RtuconnectService = TestBed.get(RtuconnectService);
    expect(service).toBeTruthy();
  });
});

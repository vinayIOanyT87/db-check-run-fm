import { TestBed } from '@angular/core/testing';

import { RtuconfigurationService } from './rtuconfiguration.service';

describe('RtuconfigurationService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: RtuconfigurationService = TestBed.get(RtuconfigurationService);
    expect(service).toBeTruthy();
  });
});

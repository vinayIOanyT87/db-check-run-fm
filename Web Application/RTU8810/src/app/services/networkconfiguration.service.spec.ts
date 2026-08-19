import { TestBed } from '@angular/core/testing';

import { NetworkconfigurationService } from './networkconfiguration.service';

describe('NetworkconfigurationService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: NetworkconfigurationService = TestBed.get(NetworkconfigurationService);
    expect(service).toBeTruthy();
  });
});

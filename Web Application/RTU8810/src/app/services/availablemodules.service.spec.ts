import { TestBed } from '@angular/core/testing';

import { AvailablemodulesService } from './availablemodules.service';

describe('AvailablemodulesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AvailablemodulesService = TestBed.get(AvailablemodulesService);
    expect(service).toBeTruthy();
  });
});

import { TestBed } from '@angular/core/testing';

import { LocalStorageService } from './localstorage.service';

describe('LocalstorageService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    expect(service).toBeTruthy();
  });
});

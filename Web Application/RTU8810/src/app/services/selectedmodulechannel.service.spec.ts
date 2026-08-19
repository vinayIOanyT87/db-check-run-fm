import { TestBed } from '@angular/core/testing';

import { SelectedmodulechannelService } from './selectedmodulechannel.service';

describe('SelectedmodulechannelService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SelectedmodulechannelService = TestBed.get(SelectedmodulechannelService);
    expect(service).toBeTruthy();
  });
});

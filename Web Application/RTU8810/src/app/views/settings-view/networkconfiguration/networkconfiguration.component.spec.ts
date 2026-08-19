import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NetworkconfigurationComponent } from './networkconfiguration.component';

describe('NetworkconfigurationComponent', () => {
  let component: NetworkconfigurationComponent;
  let fixture: ComponentFixture<NetworkconfigurationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NetworkconfigurationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NetworkconfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

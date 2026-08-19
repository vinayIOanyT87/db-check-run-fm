import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CertificateManagerComponent } from './certificatemanager.component';

describe('CertificatemanagerComponent', () => {
  let component: CertificateManagerComponent;
  let fixture: ComponentFixture<CertificateManagerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CertificateManagerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CertificateManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SystemadminDetailComponent } from './systemadmin-detail.component';

describe('SystemadminDetailComponent', () => {
  let component: SystemadminDetailComponent;
  let fixture: ComponentFixture<SystemadminDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SystemadminDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SystemadminDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

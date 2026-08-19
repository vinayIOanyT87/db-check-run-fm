import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SystemadminDescriptionComponent } from './systemadmin-description.component';

describe('SystemadminDescriptionComponent', () => {
  let component: SystemadminDescriptionComponent;
  let fixture: ComponentFixture<SystemadminDescriptionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SystemadminDescriptionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SystemadminDescriptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminUserConfigurationComponent } from './admin-user-configuration.component';

describe('AdminUserConfigurationComponent', () => {
  let component: AdminUserConfigurationComponent;
  let fixture: ComponentFixture<AdminUserConfigurationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdminUserConfigurationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminUserConfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

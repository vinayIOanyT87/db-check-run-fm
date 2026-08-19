import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AlarmmanagerComponent } from './alarmmanager.component';

describe('AlarmmanagerComponent', () => {
  let component: AlarmmanagerComponent;
  let fixture: ComponentFixture<AlarmmanagerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AlarmmanagerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AlarmmanagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChassismoduleviewComponent } from './chassismoduleview.component';

describe('ChassismoduleviewComponent', () => {
  let component: ChassismoduleviewComponent;
  let fixture: ComponentFixture<ChassismoduleviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChassismoduleviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChassismoduleviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

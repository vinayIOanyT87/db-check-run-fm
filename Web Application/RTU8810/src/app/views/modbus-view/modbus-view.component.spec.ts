import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ModbusViewComponent } from './modbus-view.component';

describe('ModbusViewComponent', () => {
  let component: ModbusViewComponent;
  let fixture: ComponentFixture<ModbusViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ModbusViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModbusViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

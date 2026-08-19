import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChassisViewComponent } from './chassis-view.component';

describe('ChassisViewComponent', () => {
  let component: ChassisViewComponent;
  let fixture: ComponentFixture<ChassisViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChassisViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChassisViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

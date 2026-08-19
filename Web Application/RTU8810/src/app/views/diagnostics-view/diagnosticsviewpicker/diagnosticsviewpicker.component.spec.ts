import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DiagnosticsviewpickerComponent } from './diagnosticsviewpicker.component';

describe('DiagnosticsviewpickerComponent', () => {
  let component: DiagnosticsviewpickerComponent;
  let fixture: ComponentFixture<DiagnosticsviewpickerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DiagnosticsviewpickerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DiagnosticsviewpickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

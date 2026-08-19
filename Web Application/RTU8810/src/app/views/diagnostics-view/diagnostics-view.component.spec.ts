import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DiagnosticsViewComponent } from './diagnostics-view.component';

describe('DiagnoticsViewComponent', () => {
  let component: DiagnosticsViewComponent;
  let fixture: ComponentFixture<DiagnosticsViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DiagnosticsViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DiagnosticsViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

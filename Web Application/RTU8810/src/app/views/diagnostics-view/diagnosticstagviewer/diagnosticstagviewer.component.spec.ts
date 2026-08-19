import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DiagnosticstagviewerComponent } from './diagnosticstagviewer.component';

describe('DiagnosticstagviewerComponent', () => {
  let component: DiagnosticstagviewerComponent;
  let fixture: ComponentFixture<DiagnosticstagviewerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DiagnosticstagviewerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DiagnosticstagviewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

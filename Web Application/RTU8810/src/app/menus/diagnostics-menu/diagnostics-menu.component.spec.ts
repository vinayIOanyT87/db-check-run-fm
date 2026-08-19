import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DiagnosticsMenuComponent } from './diagnostics-menu.component';

describe('DiagnosticsMenuComponent', () => {
  let component: DiagnosticsMenuComponent;
  let fixture: ComponentFixture<DiagnosticsMenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DiagnosticsMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DiagnosticsMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

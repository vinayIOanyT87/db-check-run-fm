import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InlineconfigurationeditorComponent } from './inlineconfigurationeditor.component';

describe('InlineconfigurationeditorComponent', () => {
  let component: InlineconfigurationeditorComponent;
  let fixture: ComponentFixture<InlineconfigurationeditorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InlineconfigurationeditorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InlineconfigurationeditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

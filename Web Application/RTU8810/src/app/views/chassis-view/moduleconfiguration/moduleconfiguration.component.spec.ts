import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ModuleconfigurationComponent } from './moduleconfiguration.component';

describe('ModuleconfigurationComponent', () => {
  let component: ModuleconfigurationComponent;
  let fixture: ComponentFixture<ModuleconfigurationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ModuleconfigurationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModuleconfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

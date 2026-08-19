import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ModuledetailComponent } from './moduledetail.component';

describe('ModuledetailComponent', () => {
  let component: ModuledetailComponent;
  let fixture: ComponentFixture<ModuledetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ModuledetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModuledetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

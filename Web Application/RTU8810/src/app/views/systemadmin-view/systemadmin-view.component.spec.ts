import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SystemadminViewComponent } from './systemadmin-view.component';

describe('SystemadminViewComponent', () => {
  let component: SystemadminViewComponent;
  let fixture: ComponentFixture<SystemadminViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SystemadminViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SystemadminViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

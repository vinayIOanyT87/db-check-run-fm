import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SystemadminCardComponent } from './systemadmin-card.component';

describe('SystemadminCardComponent', () => {
  let component: SystemadminCardComponent;
  let fixture: ComponentFixture<SystemadminCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SystemadminCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SystemadminCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

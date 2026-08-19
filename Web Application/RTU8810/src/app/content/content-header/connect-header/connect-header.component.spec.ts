import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConnectHeaderComponent } from './connect-header.component';

describe('ConnectHeaderComponent', () => {
  let component: ConnectHeaderComponent;
  let fixture: ComponentFixture<ConnectHeaderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConnectHeaderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConnectHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

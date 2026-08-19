import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterMapComponent } from './register-map.component';

describe('RegisterMapComponent', () => {
  let component: RegisterMapComponent;
  let fixture: ComponentFixture<RegisterMapComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RegisterMapComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TankmanagerComponent } from './tankmanager.component';

describe('TankmanagerComponent', () => {
  let component: TankmanagerComponent;
  let fixture: ComponentFixture<TankmanagerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TankmanagerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TankmanagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

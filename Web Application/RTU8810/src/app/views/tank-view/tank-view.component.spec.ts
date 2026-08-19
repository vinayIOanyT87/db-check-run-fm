import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TankViewComponent } from './tank-view.component';

describe('TankViewComponent', () => {
  let component: TankViewComponent;
  let fixture: ComponentFixture<TankViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TankViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TankViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

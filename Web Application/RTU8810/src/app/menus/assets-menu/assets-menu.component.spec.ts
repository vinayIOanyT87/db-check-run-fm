import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetsMenuComponent } from './assets-menu.component';

describe('AssetsMenuComponent', () => {
  let component: AssetsMenuComponent;
  let fixture: ComponentFixture<AssetsMenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssetsMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssetsMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

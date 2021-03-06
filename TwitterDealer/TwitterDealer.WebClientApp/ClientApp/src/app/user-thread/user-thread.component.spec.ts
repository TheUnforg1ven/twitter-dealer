import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserThreadComponent } from './user-thread.component';

describe('UserThreadComponent', () => {
  let component: UserThreadComponent;
  let fixture: ComponentFixture<UserThreadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserThreadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserThreadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

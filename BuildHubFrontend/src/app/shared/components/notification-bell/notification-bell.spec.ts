import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationBell } from './notification-bell';

describe('NotificationBell', () => {
  let component: NotificationBell;
  let fixture: ComponentFixture<NotificationBell>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotificationBell]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotificationBell);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

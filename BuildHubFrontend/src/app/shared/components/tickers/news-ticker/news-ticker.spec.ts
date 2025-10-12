import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewsTicker } from './news-ticker';

describe('NewsTicker', () => {
  let component: NewsTicker;
  let fixture: ComponentFixture<NewsTicker>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NewsTicker]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NewsTicker);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

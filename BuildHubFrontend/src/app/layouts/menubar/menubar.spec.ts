import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Menubar } from './menubar';

describe('Menubar', () => {
  let component: Menubar;
  let fixture: ComponentFixture<Menubar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Menubar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Menubar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

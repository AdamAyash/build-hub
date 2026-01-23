import { TestBed } from '@angular/core/testing';

import { TickerService } from './ticker.service';

describe('TickerService', () => {
  let service: TickerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TickerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

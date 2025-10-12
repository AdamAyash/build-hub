import { TestBed } from '@angular/core/testing';

import { Ticker } from './ticker';

describe('Ticker', () => {
  let service: Ticker;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Ticker);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

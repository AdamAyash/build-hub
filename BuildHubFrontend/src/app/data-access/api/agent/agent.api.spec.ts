import { TestBed } from '@angular/core/testing';

import { AgentApi } from './agent.api';

describe('AgentApi', () => {
  let service: AgentApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AgentApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

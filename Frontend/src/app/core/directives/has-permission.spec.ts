import { HasPermission } from './has-permission';

describe('HasPermission', () => {
  it('should create an instance', () => {
    const directive = new HasPermission();
    expect(directive).toBeTruthy();
  });
});

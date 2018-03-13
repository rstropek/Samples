/**
 * @license
 * Copyright (c) Ammann Schweiz AG. All Rights Reserved.
 */
import 'jasmine';
import { getAllPeople } from './people';

describe('people middleware', () => {
  it('returns some people', async (done: DoneFn) => {
    const contextMock: any = {};
    await getAllPeople(contextMock);
    expect(contextMock.body).toBeTruthy();
    expect(contextMock.body.length).toBeGreaterThan(0);
    done();
  });
});

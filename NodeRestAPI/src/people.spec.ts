import 'jasmine';

import { getAllPeople } from './people';

// Tip:
// For testing web apis, consider using a testing framework
// like supertest (https://github.com/visionmedia/supertest)

describe('people middleware', () => {
  it('returns some people', async (done: DoneFn) => {
    const contextMock: any = {};
    await getAllPeople(contextMock);
    expect(contextMock.body).toBeTruthy();
    expect(contextMock.body.length).toBeGreaterThan(0);
    done();
  });
});

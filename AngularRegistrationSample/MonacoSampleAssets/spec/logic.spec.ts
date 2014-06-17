/// <reference path="../typings/jasmine/jasmine.d.ts" />

describe("Registration ", function() {
  it("reports invalid in case of low age.", function() {
    var reg = new Registration({ name: "", salutation: "", age: 5 });
    expect(reg.isValid()).toBe(false);
  });
  it("reports valid in case of high age.", function() {
    var reg = new Registration({ name: "", salutation: "", age: 30 });
    expect(reg.isValid()).toBe(true);
  });
});
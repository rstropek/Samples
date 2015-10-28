/// <reference path="typings/tsd.d.ts" />

interface IVector {
  x: number;
  y: number;
  z?: number;
}

function add2DVectors(v1: IVector, v2: IVector): IVector {
  return { x: v1.x + v2.x, y: v1.y + v2.y };
}

function divideValues(v1: number, v2: number) : number {
  if (v2 === 0) {
    throw new Error("v2 must not be zero.");
  }
  
  return v1 / v2;
}

// We start by describing a test suite
describe("Math helper library", () => {
  
  // Next, we create a spec with expectations. If
  // all expectations are true, the spec succeeds.
  it("returns 3 if we add 1 and 2", () => {
    // The expectations takes the "actual" as a paramter ...
    expect(1 + 2)
    // ... and is followed by a "matcher
      .toBe(3);
  });

  it ("can check whether to numbers are identical", () => {
    // We can easily check for true and false
    expect(1 == 2).toBeFalsy();
    expect(1 == 1).toBeTruthy();
  });

  it("returns a positive value if we multiple two negative values", () => {
    // Get intellisense for matchers with TypeScript
    expect((-1) * (-1)).toBeGreaterThan(0);
    
    // Inverse matcher with "not"
    expect((-1) * (-1)).not.toBeLessThan(0);
    
    // You can even write custom matchers
    // (see http://jasmine.github.io/2.3/custom_matcher.html)
  });

  it("can add two vectors", () => {
    var v1: IVector = { x: 1, y: 1 };
    var v2: IVector = { x: 1, y: 1 };
    var v3: IVector = null;

    // We can check whether two objects are equal
    expect(add2DVectors(v1, v2)).toEqual({ x: 2, y: 2});
    expect(add2DVectors(v1, v2)).not.toEqual({ x: 3, y: 3});

    // We can check whether two objects are identical
    expect(v1).toBe(v1);
    expect(add2DVectors(v1, v2)).not.toBe({ x: 2, y: 2});

    // We can check for undefined, null, etc.
    expect(v1.z).toBeUndefined();
    expect(v1.x).toBeDefined();
    expect(v3).toBeNull();
  });
  
  it("can turn numbers into strings", () => {
    var num = -123;
    var s1 = num.toString();
    
    // Use a regular expression in a matcher
    expect(s1).toMatch("[+-]?\\d+");
  });
  
  it ("can work with floats", () => {
    // We can compare floating point values by rounding
    expect(1 / 3).toBeCloseTo(0.33, 2);
    expect(Math.PI).toBeCloseTo(Math.E, 0);
  });
  
  it ("can apply math to elements of an array", () => {
    var numbers = [1, 2, 3, 4, 5];
    
    // We can check whether a collection contains an element
    expect(numbers.map(val => val * val)).toContain(25);
    
    // We can do more advanced matchings using helper functions
    // in "jasmine". TypeScript will give you intellisense.
    expect(numbers.map(val => val + val)).toEqual(jasmine.arrayContaining([2, 4, 6]));
  });
  
  it ("can divide values", () => {
    // Check whether a function throws an error
    expect(() => divideValues(1, 0)).toThrowError();
    expect(() => divideValues(1, 0)).toThrowError("v2 must not be zero.");
    expect(() => divideValues(1, 1)).not.toThrowError();
  });
});

interface ICustomer {
  firstName: string;
  lastName: string;
}

describe("Customer management", () => {
  var customerRepository : ICustomer[];
  this.sampleCustomer = { firstName: "Max", lastName: "Muster" };
  
  // We can prepare/cleanup using beforeEach, afterEach,
  // beforeAll, and afterAll
  beforeEach(() => {
    // Cleanup customers and add some demo data
    customerRepository = [];
    // Note the use of "this" here
    customerRepository.push(this.sampleCustomer);
  });
  
  // Note the creation of a nested describe block
  describe("Writing Customers", () => {
    it ("can add a new customer", () => {
      customerRepository.push({ firstName: "Tom", lastName: "Turbo" });
      expect(customerRepository.length).toBe(2);
    });
    it ("can remove a customer", () => {
      var c = customerRepository.pop();
      expect(c).toBe(this.sampleCustomer);
      expect(customerRepository.length).toBe(0);
    });
    
    // Disable specs or suites with "x"
    xit("will fail", () => { fail("Just for demo purposes..."); });
  });
});
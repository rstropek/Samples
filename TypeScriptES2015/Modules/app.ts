// Default import from module
import math from "./module";
var m = new math();
console.log(m.add(5, 1).toString());

// Import declarations
import { AdvancedMath, div as division } from "./module";
var m2 = new AdvancedMath();
console.log(division(8, 2).toString());
console.log(m2.mult(10, 3));

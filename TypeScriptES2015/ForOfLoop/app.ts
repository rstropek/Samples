// Iterate over array elements with the new for..of loop
var numbers = [ 0, 1, 2, 3, 4, 5 ];
for (var n of numbers) {
	if (n % 2 == 0) {
		console.log(n.toString());
	}
}

// Use new decomposition feature in combination with for..of loop
var numberObjects = [
	{ value: 0, isEven: true },
	{ value: 1, isEven: false },
	{ value: 2, isEven: true },
	{ value: 3, isEven: false },
	{ value: 4, isEven: true },
	{ value: 5, isEven: false }];
for (var { value, isEven } of numberObjects) {
	if (value % 2 == 0) {
		console.log(value.toString());
	}
}

// Note that the following code will not work. It will print 'odd'
// two times as all arrow functions refer to the same binding of n.
// We could use 'for(let...)' instead of 'for(var...)' to create a 
// different binding for each iteration. However, 'for(let...)'
// cannot be transpiled to ES5 by the TypeScript compiler.  
var lazyResults: (() => boolean)[] = [];
for(var num of numbers){
	lazyResults.push(() => num % 2 == 0);
}
console.log(lazyResults[0]() ? 'even' : 'odd');
console.log(lazyResults[1]() ? 'even' : 'odd');

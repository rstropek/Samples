function doSomething(num: number) {
	if (num % 2 == 0){
		// Note that 'isEven' is function-scoped. Its scope is
		// not limited to the 'if' statement. If you replace 'var'
		// with 'let', you will get a syntax error as the scope
		// of 'isEven' changes to the current block. 
		var isEven = true;
	}
	else {
		isEven = false;
	}
	
	return isEven;
}

function doSomethingWithLet(num: number) {
	// Note that we have to define 'isEven' here to make
	// it function-scoped.
	let isEven: boolean;
	if (num % 2 == 0){
		// If you add 'let' here, this 'isEven' would refer
		// to a different variable than the 'isEven' shown above. 
		isEven = true;
	}
	else {
		isEven = false;
	}
	
	return isEven;
}

const numberToCheck: number = 2;

console.log(doSomething(numberToCheck));

// The following line would result in a syntax error.
//numberToCheck = 3;

console.log(doSomethingWithLet(numberToCheck));

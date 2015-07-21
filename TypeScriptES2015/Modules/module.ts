// Define some classes and functions
class SimpleMath {
	add(x: number, y: number): number {
		return x + y;
	}
}

// Note that the following class is immediately exported. 
// No need for any additional export statements.
export class AdvancedMath {
	mult(x: number, y:number): number {
		return x * y;
	}
}

function div(x: number, y:number) : number {
	return x / y;
}

// Export declaration of item defined above
export { div };

// Setup the default export of the module. Note that
// it is recommended that a module exports a single
// default export. You can export additional things
// (as shown here for demo purposes), but this is not
// recommended.
export default SimpleMath;


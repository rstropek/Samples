// A simple logger interface to demonstrate AngularJS depdency injection.
// The implementatio of the logger is published in index.ts.
interface ILogger {
	log: (string) => void;
}


// Reused business logic and data structure from the server
class DefaultLogger implements ILogger {
	public log(text: string) : void {
		console.log(text);
	}
}

interface IRegistration {
	salutation: string;
	name: string;
	age: number;
}

class Registration implements IRegistration {
	public salutation: string;
	public name: string;
	public age: number;

	constructor(registration: IRegistration) {
		this.salutation = registration.salutation;
		this.name = registration.name;
		this.age = registration.age;
	}

	public isValid() : boolean {
		return this.age >= 18;
	}
}

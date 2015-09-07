/// <reference path="typings/tsd.d.ts" />

interface IPerson {
	firstName: string;
	lastName: string;
}

class DemoController {
	public person: IPerson = { firstName: "", lastName: "" };
	public personList: IPerson[] = [ 
		{ firstName: "Tom", lastName: "Turbo" },
		{ firstName: "Max", lastName: "Muster" } ];
	
	getFullName() {
		// Note destructuring of person object in local variables
		// firstName and lastName
		var { firstName, lastName } = this.person;
		
		// Note using the new template string 
		return `${firstName} ${lastName}`;
	}
	
	getColleagues() {
		// Note destructuring of an array in local variables p1 and p2
		var [ p1, p2 ] = this.personList;
		
		// Swap p1 and p2 by using destructuring
		[p1, p2] = [p2, p1];

		// Note using the new template string
		// Output: "Your colleagues are Max and Tom"
		return `Your colleagues are ${p1.firstName} and ${p2.firstName}`;
	}
}

angular.module("demo", []).controller("demo", DemoController);
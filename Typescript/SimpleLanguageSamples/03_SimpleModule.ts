/// <reference path="../../../tsd/DefinitelyTyped/node/node-0.11.d.ts" />

module Crm {
	export class Customer {
		constructor(public custName: string) { }
	}
}

module Crm {
	export class Opportunity {
		constructor(public customer: Customer) { }
	}
}

var classesInCrmModule = "";
for(var key in Crm) {
	console.log(key);
}


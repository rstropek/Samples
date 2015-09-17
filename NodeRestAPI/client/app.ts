/// <reference path="../typings/tsd.d.ts" />

interface ICustomer {
    _id?: number;
    firstName: string;
    lastName: string;
}

class CustomerController {
	constructor(private $http: ng.IHttpService) {
		this.refresh();
	}
	
	public customers: ICustomer[];
	
	refresh() {
		return this.$http.get<ICustomer[]>("http://softarchsummit-node.azurewebsites.net/customers")
			.success(result => this.customers = result);
	}
	
	addCustomer() {
		var newCustomer: ICustomer = { firstName: "Test", lastName: "Turbo" };
		this.$http.post("http://softarchsummit-node.azurewebsites.net/customers", newCustomer)
			.then(() => this.refresh());
	}
}

angular.module("SoftArchSummitApp", [])
	.controller("CustomerController", CustomerController);
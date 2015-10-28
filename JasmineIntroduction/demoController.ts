/// <reference path="typings/tsd.d.ts" />

interface ICustomerDetails {
	address: string;
}

class DemoController {
	public firstName: string;
	public lastName: string;
	public isValid: boolean;
	public details: ICustomerDetails;
	public restApiError: boolean;
	
	constructor($scope: ng.IScope, InvalidLastName: string, private $http: ng.IHttpService) {			
		this.firstName = "Tom";
		this.lastName = "Turbo";
		this.isValid = true;
		this.restApiError = false;
		this.details = null;
		
		$scope.$watch(
			"vm.lastName", 
			() => this.isValid = this.lastName !== InvalidLastName);
	}
	
	getDetails() {
		this.$http.get<ICustomerDetails>("https://myserver.com/api/getCustomerDetails")
			.then(
				data => this.details = data.data, 
				err => this.restApiError = true);
	}
}

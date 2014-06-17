/// <reference path="../typings/azure-mobile-services-client/AzureMobileServicesClient.d.ts" />

interface IRegisterViewModel extends ng.IScope, IRegistration {
	save: () => void;
}

class RegisterViewModel {
	constructor($scope: IRegisterViewModel, $http:  ng.IHttpService, private logger: ILogger) {
		$scope.save = () => {
			var client = new WindowsAzure.MobileServiceClient("https://monacodemo.azure-mobile.net/", "");
			client.getTable("registrations").insert({ name: $scope.name, salutation: $scope.salutation, age: $scope.age })
				.then(_ => {
					alert("You are registered!");
				},
				_ => {
					alert("Sorry, not possible!");
				});
		}
	}
}

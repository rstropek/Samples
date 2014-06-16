// The interface the Angular's $scope. Used to access the data structure for
// data binding in a type-safe way. 
interface IRegistrationsViewModel extends ng.IScope {
	registrations: Array<IRegistration>;
	refresh: () => void;
}

// The controller class. Note that it uses Angular's dependency injection to
// get the $http service (for http requests) and the logger (see above).
// 
class RegistrationsViewModel {
	constructor($scope: IRegistrationsViewModel, $http: ng.IHttpService, private logger: ILogger) {
		$scope.registrations = new Array<IRegistration>();
		$scope.refresh = () => {
			logger.log("Requesting...");
			var client = new WindowsAzure.MobileServiceClient("https://monacodemo.azure-mobile.net/", "");
			client.getTable("registrations").read()
				.then(registrations => {
					while ($scope.registrations.length > 0){
						$scope.registrations.pop();
					}
					registrations.forEach(r => $scope.registrations.push(r));
					$scope.$apply();
				});
		};
	}
}

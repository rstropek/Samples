interface IRegisterViewModel extends ng.IScope, IRegistration {
	save: () => void;
}

class RegisterViewModel {
	constructor($scope: IRegisterViewModel, $http: ng.IHttpService, private logger: ILogger) {
		$scope.save = () => {
			$http.post("http://localhost:1337/register", { name: $scope.name, salutation: $scope.salutation, age: $scope.age }, { headers: { "Content-Type": "application/json" } })
				.success(_ => {
					alert("You are registered!");
				})
				.error(_ => {
					alert("Sorry, not possible!");
				});
		}
	}
}

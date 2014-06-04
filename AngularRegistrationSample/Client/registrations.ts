// A simple logger interface to demonstrate AngularJS depdency injection.
// The implementatio of the logger is published in index.ts.
interface ILogger {
	log: (string) => void;
}


// Reused business logic and data structure from the server
class DefaultLogger implements ILogger {
	public log(text: string) {
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

	public isValid() {
		return this.age >= 18;
	}
}


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
			$http.get<Array<IRegistration>>("/api/registrations")
				.success(registrations => {
					registrations.forEach(r => $scope.registrations.push(r));
				});
		};
	}
}

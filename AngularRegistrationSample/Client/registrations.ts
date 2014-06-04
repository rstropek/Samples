interface ILogger {
	log: (string) => void;
}

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


interface IRegistrationsViewModel extends ng.IScope {
	registrations: Array<IRegistration>;
	refresh: () => void;
}

class RegistrationsViewModel {
	constructor($scope: IRegistrationsViewModel, $http: ng.IHttpService, private logger: ILogger) {
		$scope.registrations = new Array<IRegistration>();
		$scope.refresh = () => {
			logger.log("Requesting...");
			$http.get<Array<IRegistration>>("http://localhost:1337/registrations")
				.success(registrations => {
					registrations.forEach(r => $scope.registrations.push(r));
				});
		};
	}
}

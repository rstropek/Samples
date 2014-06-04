angular.module("RegistrationApp", ["ngRoute"])
	.factory("logger", () => new DefaultLogger())
	.controller("RegistrationController", ViewModel)
	.controller("RegistrationController2", RegistrationViewModel)
	.config(($routeProvider: ng.route.IRouteProvider) => {
		$routeProvider
			.when("/registrations", {
				templateUrl: "registrations.html", controller: "RegistrationController"
			})
			.when("/registration", {
				templateUrl: "registration.html", controller: "RegistrationController2"
			});
	});
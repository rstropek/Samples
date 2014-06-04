angular.module("RegistrationApp", ["ngRoute"])
	.factory("logger", () => new DefaultLogger())
	.controller("RegistrationsController", RegistrationsViewModel)
	.controller("RegisterController", RegisterViewModel)
	.config(($routeProvider: ng.route.IRouteProvider) => {
		$routeProvider
			.when("/registrations", {
				templateUrl: "registrations.html", controller: "RegistrationsController"
			})
			.when("/register", {
				templateUrl: "register.html", controller: "RegisterController"
			});
	});
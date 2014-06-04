angular.module("RegistrationApp", ["ngRoute"])
	// The logger to demonstrate AngularJS dependency injection
	.factory("logger", () => new DefaultLogger())
	// Our controllers for the two views
	.controller("RegistrationsController", RegistrationsViewModel)
	.controller("RegisterController", RegisterViewModel)
	// The routes for the SPA
	.config(($routeProvider: ng.route.IRouteProvider) => {
		$routeProvider
			.when("/", {
				templateUrl: "registrations.html", controller: "RegistrationsController"
			})
			.when("/register", {
				templateUrl: "register.html", controller: "RegisterController"
			});
	});
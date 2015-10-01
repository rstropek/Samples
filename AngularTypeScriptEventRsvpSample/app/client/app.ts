/// <reference path="../../typings/tsd.d.ts" />
/// <reference path="register.ts" />
/// <reference path="list.ts" />
/// <reference path="mobileServicesTableService.ts" />

angular.module("EventRSVP", ["ngRoute"])
	// Register factor function for registrationTable service
	.factory("registrationTable", ["$http", ($http: ng.IHttpService) =>
        new MobileServicesDataAccess.Table($http, "eventrsvp", "reservation")])
	// Register controllers
	.controller("RegisterController", Registration.RegisterController)
	.controller("ListController", Registration.ListController)
	// Setup routing
	.config(["$routeProvider", ($routeProvider: angular.route.IRouteProvider) => {
		$routeProvider
			.when("/register", { templateUrl: "register.html", controller: "RegisterController", controllerAs: "vm"})
			.when("/list", { templateUrl: "list.html", controller: "ListController", controllerAs: "vm"})
			.otherwise({ redirectTo: "/register" });
	}]);

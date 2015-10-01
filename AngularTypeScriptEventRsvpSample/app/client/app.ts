/// <reference path="../../typings/tsd.d.ts" />
/// <reference path="register.ts" />
/// <reference path="mobileServicesTableService.ts" />

angular.module("EventRSVP", ["ngRoute"])
	// Register factor function for reservationTable service
	.factory("reservationTable", ["$http", ($http: ng.IHttpService) =>
        new MobileServicesDataAccess.Table($http, "eventrsvp", "reservation")])
	// Register controllers
	.controller("RegisterController", Registration.RegisterController)
	// Setup routing
	.config(["$routeProvider", ($routeProvider: angular.route.IRouteProvider) => {
		$routeProvider
			.when("/register", { templateUrl: "register/register.html", controller: "RegisterController", controllerAs: "vm"})
			.otherwise({ redirectTo: "/register" });
	}]);

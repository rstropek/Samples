/// <reference path="../../typings/tsd.d.ts" />
/// <reference path="register/register.ts" />
/// <reference path="register/mobileServicesTableService.ts" />

angular.module("EventRSVP", ["ngRoute"])
	.factory("reservationTable", ["$http", ($http: ng.IHttpService) =>
        new MobileServicesDataAccess.Table($http, "eventrsvp", "reservation")])
	.controller("RegisterController", Registration.RegisterController)
	.config(["$routeProvider", ($routeProvider: angular.route.IRouteProvider) => {
		$routeProvider
			.when("/register", { templateUrl: "register/register.html", controller: "RegisterController", controllerAs: "vm"})
			.otherwise({ redirectTo: "/register" });
	}]);

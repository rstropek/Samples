/// <reference path="ticket.ts" />
/// <reference path="feedback.ts" />
/// <reference path="profile.ts" />

angular.module("ConferenceFeedback", ["ngRoute"])
	.constant("webApiBaseUrl", "http://localhost:17219/api/")
	.controller("ProfileController", FeedbackController)
	.controller("FeedbackController", FeedbackController)
	.config(($routeProvider: angular.route.IRouteProvider) => {
		$routeProvider
			.when("/feedback/:ticketId", { templateUrl: "feedback.html" })
			.when("/profile/:ticketId", { templateUrl: "profile.html" })
			.otherwise({ templateUrl: "wrongLink.html" })
		});

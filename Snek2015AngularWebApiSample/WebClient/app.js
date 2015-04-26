angular.module("ConferenceFeedback", ["ngRoute"]).config(function ($routeProvider) {
    $routeProvider.when("/feedback/:ticketId", { templateUrl: "feedback.html" }).when("/profile/:ticketId", { templateUrl: "profile.html" }).otherwise({ templateUrl: "wrongLink.html" });
});
//# sourceMappingURL=app.js.map
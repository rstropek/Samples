/// <reference path="app.ts" />
var FeedbackController = (function () {
    function FeedbackController($scope, $routeParams, webApiBaseUrl, $http) {
        $scope.loadingTicket = true;
        $scope.loadingError = false;
        $scope.successfullySaved = false;
        $scope.save = function () { return $http.post(webApiBaseUrl + "ratings", $scope.ratings).success(function () { return $scope.successfullySaved = true; }); };
        $scope.getSessionTitle = (function (sessionId) { return $scope.sessions.filter(function (s) { return s.sessionId == sessionId; })[0].title; });
        $http.get(webApiBaseUrl + "tickets/" + $routeParams.ticketId).success(function (t) { return $scope.ticket = t; }).error(function () { return $scope.loadingError = true; }).finally(function () { return $scope.loadingTicket = false; });
        $http.get(webApiBaseUrl + "sessions").success(function (s) {
            $scope.sessions = s;
            $scope.ratings = s.map(function (s) {
                return { ticketId: $routeParams.ticketId, sessionId: s.sessionId, rating: 0 };
            });
        }).error(function () { return $scope.loadingError = true; });
    }
    return FeedbackController;
})();
/// <reference path="app.ts" />
var ProfileController = (function () {
    function ProfileController($scope, $routeParams, webApiBaseUrl, $http) {
        $http.get(webApiBaseUrl + "tickets/" + $routeParams.ticketId).success(function (t) { return $scope.ticket = t; });
        $scope.save = function () { return $http.put(webApiBaseUrl + "tickets" + $routeParams.ticketId, $scope.ticket); };
    }
    return ProfileController;
})();
/// <reference path="ticket.ts" />
/// <reference path="feedback.ts" />
/// <reference path="profile.ts" />
angular.module("ConferenceFeedback", ["ngRoute"]).constant("webApiBaseUrl", "http://localhost:17219/api/").controller("ProfileController", FeedbackController).controller("FeedbackController", FeedbackController).config(function ($routeProvider) {
    $routeProvider.when("/feedback/:ticketId", { templateUrl: "feedback.html" }).when("/profile/:ticketId", { templateUrl: "profile.html" }).otherwise({ templateUrl: "wrongLink.html" });
});
//# sourceMappingURL=confFeedback.js.map
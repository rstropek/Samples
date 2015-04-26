/// <reference path="app.ts" />

interface IProfileScope extends ng.IScope {
	ticket: ITicket;
	save: () => void;
}

class ProfileController {
	constructor($scope: IFeedbackScope, $routeParams, webApiBaseUrl: string, $http: ng.IHttpService) {
		$http.get<ITicket>(webApiBaseUrl + "tickets/" + $routeParams.ticketId)
			.success(t => $scope.ticket = t);
		$scope.save = () => $http.put(webApiBaseUrl + "tickets" + $routeParams.ticketId, $scope.ticket);
	}
}

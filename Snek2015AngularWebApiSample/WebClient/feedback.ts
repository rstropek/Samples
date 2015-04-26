/// <reference path="app.ts" />

interface ISession {
	sessionId: string;
	title: string;
}

interface IRating {
	ticketId: string;
	sessionId: string;
	rating: number;
}

interface IFeedbackScope extends ng.IScope {
	loadingTicket: boolean;
	loadingError: boolean;
	ticket: ITicket;
	ratings: IRating[];
	sessions: ISession[];
	getSessionTitle: (string) => string;
	save: () => void;
	successfullySaved: boolean;
}

class FeedbackController {
	constructor($scope: IFeedbackScope, $routeParams, webApiBaseUrl: string, $http: ng.IHttpService) {
		$scope.loadingTicket = true;
		$scope.loadingError = false;
		$scope.successfullySaved = false;
		$scope.save = () => $http.post(webApiBaseUrl + "ratings", $scope.ratings)
				.success(() => $scope.successfullySaved = true);
		$scope.getSessionTitle = (sessionId => $scope.sessions.filter(s => s.sessionId == sessionId)[0].title);

		$http.get<ITicket>(webApiBaseUrl + "tickets/" + $routeParams.ticketId)
			.success(t => $scope.ticket = t)
			.error(() => $scope.loadingError = true)
			.finally(() => $scope.loadingTicket = false);

		$http.get<ISession[]>(webApiBaseUrl + "sessions")
			.success(s => {
				$scope.sessions = s;
				$scope.ratings = s.map(s => { return { ticketId: $routeParams.ticketId, sessionId: s.sessionId, rating: 0 }; });
			})
			.error(() => $scope.loadingError = true);
	}
}

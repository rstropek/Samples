angular.module("HttpServiceModule", ["ui.bootstrap", "ngGrid"])
    .constant("paginationItemsPerPage", 10)
    .factory("eventTable", ($http: ng.IHttpService, paginationItemsPerPage: number) =>
        new MobileServicesDataAccess.Table($http, "ipc2014", "events",
			paginationItemsPerPage, "FpBoHOkwyPGtAqnFyOQEQTpBGGbbMI99"))
    .controller("HttpServiceController", HttpServiceModule.HttpServiceController);

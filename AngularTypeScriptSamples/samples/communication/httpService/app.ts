/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>
/// <reference path="MobileServicesTable.ts"/>
/// <reference path="HttpServiceController.ts"/>

angular.module("HttpServiceModule", ["ui.bootstrap", "ngGrid"])
    .constant("paginationItemsPerPage", 10)
    .factory("eventTable", ($http: ng.IHttpService, paginationItemsPerPage: number) =>
        new MobileServicesDataAccess.Table($http, "dotnetusergroupangular", "events",
            paginationItemsPerPage, "wqgnjGFeEjzVRAFvpRTdFghETsBwvr49"))
    .controller("HttpServiceController", HttpServiceModule.HttpServiceController);

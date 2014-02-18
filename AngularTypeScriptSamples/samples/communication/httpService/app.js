angular.module("HttpServiceModule", ["ui.bootstrap", "ngGrid"]).constant("paginationItemsPerPage", 10).factory("eventTable", function ($http, paginationItemsPerPage) {
    return new MobileServicesDataAccess.Table($http, "dotnetusergroupangular", "events", paginationItemsPerPage, "wqgnjGFeEjzVRAFvpRTdFghETsBwvr49");
}).controller("HttpServiceController", HttpServiceModule.HttpServiceController);
//# sourceMappingURL=app.js.map

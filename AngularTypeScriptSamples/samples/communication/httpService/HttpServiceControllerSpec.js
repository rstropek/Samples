/// <reference path="../../../Scripts/typings/jasmine/jasmine.d.ts"/>
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>
/// <reference path="../../../Scripts/typings/angularjs/angular-mocks.d.ts"/>
/// <reference path="MobileServicesTable.ts"/>
/// <reference path="HttpServiceController.ts"/>
describe("Mobile Services Table Test", function () {
    var $http;
    var $httpBackend;
    var $scope;
    var $rootScope;
    var $controller;
    var ctrl;

    var table;
    beforeEach(inject(function (_$http_, _$httpBackend_) {
        $http = _$http_;
        $httpBackend = _$httpBackend_;
        table = new MobileServicesDataAccess.Table($http, "dummyService", "dummyTable", 10, "dummyKey");
    }));
    beforeEach(inject(function (_$rootScope_, _$controller_) {
        $rootScope = _$rootScope_;
        $scope = _$rootScope_.$new();
        $controller = _$controller_;

        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id&$top=10").respond({ results: [], count: 0 });
        ctrl = $controller(HttpServiceModule.HttpServiceController, { $scope: $scope, eventTable: table, paginationItemsPerPage: 10 });
    }));

    it(' should get events after creation', function () {
        $httpBackend.flush();
    });

    it(' should load second page if clicked on pager', function () {
        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id&$top=10&$skip=10").respond({ results: [], count: 0 });
        $scope.currentPage = 2;
        $scope.$apply();
        $httpBackend.flush();
    });

    it(' should delete all events correctly', function () {
        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id").respond({
            results: [
                {
                    id: 1, eventCategory: "Concert", eventDescription: "Dummy",
                    eventTitle: "Dummy", eventDate: new Date(), maximumParticipants: 1
                }, {
                    id: 2, eventCategory: "Concert", eventDescription: "Dummy",
                    eventTitle: "Dummy", eventDate: new Date(), maximumParticipants: 1
                }], count: 2
        });
        $httpBackend.expectDELETE("https://dummyService.azure-mobile.net/tables/dummyTable/1").respond(204);
        $httpBackend.expectDELETE("https://dummyService.azure-mobile.net/tables/dummyTable/2").respond(204);
        $scope.vm.deleteEvents();
        $httpBackend.flush();
    });

    afterEach(function () {
        $httpBackend.verifyNoOutstandingExpectation();
        $httpBackend.verifyNoOutstandingRequest();
    });
});
//# sourceMappingURL=HttpServiceControllerSpec.js.map

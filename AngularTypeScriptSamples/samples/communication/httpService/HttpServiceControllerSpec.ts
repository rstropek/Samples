describe("Mobile Services Table Test", function () {
    var $http: ng.IHttpService;
    var $httpBackend: ng.IHttpBackendService;
    var $scope: HttpServiceModule.IServiceModuleScope;
    var $rootScope: ng.IRootScopeService;
    var $controller: ng.IControllerService;
    var ctrl: HttpServiceModule.HttpServiceController;

    var table: MobileServicesDataAccess.ITable<IDummyRow>;
    beforeEach(inject((_$http_, _$httpBackend_) => {
        $http = _$http_;
        $httpBackend = _$httpBackend_;
        table = new MobileServicesDataAccess.Table<HttpServiceModule.IEvent>($http, "dummyService", "dummyTable", 10, "dummyKey");
    }));
    beforeEach(inject(function (_$rootScope_: ng.IRootScopeService, _$controller_: ng.IControllerService) {
        $rootScope = _$rootScope_;
        $scope = <HttpServiceModule.IServiceModuleScope>_$rootScope_.$new();
        $controller = _$controller_;

        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id&$top=10")
            .respond({ results: [], count: 0 });
        ctrl = $controller(HttpServiceModule.HttpServiceController, { $scope: $scope, eventTable: table, paginationItemsPerPage: 10 });
    }));

    it(" should get events after creation", () => {
        $httpBackend.flush();
    });

    it(" should be loading second page if clicked on pager", () => {
        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id&$top=10&$skip=10")
            .respond({ results: [], count: 0 });
        $scope.currentPage = 2;
        $scope.$apply();
        $httpBackend.flush();
    });

    it(" should delete all events correctly", () => {
        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id")
            .respond({
                results: [{
                    id: 1, eventCategory: "Concert", eventDescription: "Dummy",
                    eventTitle: "Dummy", eventDate: new Date(), maximumParticipants: 1
                }, {
                        id: 2, eventCategory: "Concert", eventDescription: "Dummy",
                        eventTitle: "Dummy", eventDate: new Date(), maximumParticipants: 1
                    }], count: 2
            });
        $httpBackend.expectDELETE("https://dummyService.azure-mobile.net/tables/dummyTable/1")
            .respond(204);
        $httpBackend.expectDELETE("https://dummyService.azure-mobile.net/tables/dummyTable/2")
            .respond(204);
        $scope.vm.deleteEvents();
        $httpBackend.flush();
    });

    afterEach(() => {
        $httpBackend.verifyNoOutstandingExpectation();
        $httpBackend.verifyNoOutstandingRequest();
    });
});
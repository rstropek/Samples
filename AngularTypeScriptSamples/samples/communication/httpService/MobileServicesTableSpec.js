/// <reference path="../../../Scripts/typings/jasmine/jasmine.d.ts"/>
/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>
/// <reference path="../../../Scripts/typings/angularjs/angular-mocks.d.ts"/>
/// <reference path="MobileServicesTable.ts"/>

describe("Mobile Services Table Test", function () {
    var $http;
    var $httpBackend;
    var table;
    beforeEach(inject(function (_$http_, _$httpBackend_) {
        $http = _$http_;
        $httpBackend = _$httpBackend_;
        table = new MobileServicesDataAccess.Table($http, "dummyService", "dummyTable", 10, "dummyKey");
    }));
    var dummyResult = { results: [{ id: 1 }, { id: 2 }], count: 2 };

    it(' should query Azure Mobile Service without paging', function () {
        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id").respond(dummyResult);

        var result;
        table.query().success(function (r) {
            result = r.results;
        });
        $httpBackend.flush();
        expect(result.length).toEqual(2);
    });

    it(' should query Azure Mobile Service with paging', function () {
        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id&$top=10").respond(dummyResult);

        var result;
        table.query(1).success(function (r) {
            result = r.results;
        });
        $httpBackend.flush();
        expect(result.length).toEqual(2);

        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id&$top=10&$skip=10").respond(dummyResult);
        table.query(2);
        $httpBackend.flush();
    });

    it(' should issue a POST to Azure Mobile Service for insert', function () {
        $httpBackend.expectPOST("https://dummyService.azure-mobile.net/tables/dummyTable").respond(201);

        var data = {};
        table.insert(data);
        $httpBackend.flush();
    });

    it(' should issue a DELETE to Azure Mobile Service for delete', function () {
        $httpBackend.expectDELETE("https://dummyService.azure-mobile.net/tables/dummyTable/1").respond(204);

        table.deleteItemById(1);
        $httpBackend.flush();
    });

    it(' should issue a PATCH to Azure Mobile Service for delete', function () {
        $httpBackend.expect("PATCH", "https://dummyService.azure-mobile.net/tables/dummyTable/1", '{"id":1}').respond(200);

        table.update({ id: 1 });
        $httpBackend.flush();
    });

    afterEach(function () {
        $httpBackend.verifyNoOutstandingExpectation();
        $httpBackend.verifyNoOutstandingRequest();
    });
});
//# sourceMappingURL=MobileServicesTableSpec.js.map

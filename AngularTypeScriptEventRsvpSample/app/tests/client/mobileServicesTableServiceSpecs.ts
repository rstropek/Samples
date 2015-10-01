/// <reference path="../../../typings/tsd.d.ts" />
/// <reference path="../../../app/client/mobileServicesTableService.ts" />

interface IDummyRow extends MobileServicesDataAccess.ITableRow {
}

describe("Mobile Services Table Test", function () {
    var dummyResult: IDummyRow[] = [{ id: 1 }, { id: 2 }];
    var $httpBackend: ng.IHttpBackendService;
    var table: MobileServicesDataAccess.Table<IDummyRow>;
    beforeEach(inject((_$http_: ng.IHttpService, _$httpBackend_: ng.IHttpBackendService) => {
        var $http = _$http_;
        $httpBackend = _$httpBackend_;
        table = new MobileServicesDataAccess.Table<IDummyRow>($http, "dummyService", "dummyTable");
    }));

    it(" should query Azure Mobile Service", () => {
        // We want to test the Table<T> class without any real web requests. Therefore we have to 
        // mock the web request and return test data.
        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$orderby=id")
            .respond(dummyResult);

        // Run the query
        var result: IDummyRow[];
        table.query().success(r => result = r);
        $httpBackend.flush();

        // Check if query method returned correct data
        expect(result.length).toEqual(2);
    });

    it(" should be issuing a POST to Azure Mobile Service for insert", () => {
        $httpBackend.expectPOST("https://dummyService.azure-mobile.net/tables/dummyTable")
            .respond(201 /* Created */);

        var data: IDummyRow = {};
        var status: number;
        table.insert(data).then(r => status = r.status);
        $httpBackend.flush();
        
        expect(status).toBe(201);
    });

    afterEach(() => {
        $httpBackend.verifyNoOutstandingExpectation();
        $httpBackend.verifyNoOutstandingRequest();
    });
});
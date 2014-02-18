interface IDummyRow extends MobileServicesDataAccess.ITableRow {
}

describe("Mobile Services Table Test", function () {
    var $http: ng.IHttpService;
    var $httpBackend: ng.IHttpBackendService;
    var table: MobileServicesDataAccess.ITable<IDummyRow>;
    beforeEach(inject((_$http_, _$httpBackend_) => {
        $http = _$http_;
        $httpBackend = _$httpBackend_;
        table = new MobileServicesDataAccess.Table<IDummyRow>($http, "dummyService", "dummyTable", 10, "dummyKey");
    }));
    var dummyResult: MobileServicesDataAccess.IQueryResult<IDummyRow> = { results: [{ id: 1 }, { id: 2 }], count: 2 };

    it(" should query Azure Mobile Service without paging", () => {
        // We want to test the Table<T> class without any real web requests. Therefore we have to 
        // mock the web request and return test data.
        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id")
            .respond(dummyResult);

        // Run the query
        var result: IDummyRow[];
        table.query().success(r => {
            result = r.results;
        });
        $httpBackend.flush();

        // Check if query method returned correct data
        expect(result.length).toEqual(2);
    });

    it(" should query Azure Mobile Service with paging", () => {
        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id&$top=10")
            .respond(dummyResult);

        var result: IDummyRow[];
        table.query(1).success(r => {
            result = r.results;
        });
        $httpBackend.flush();
        expect(result.length).toEqual(2);

        $httpBackend.whenGET("https://dummyService.azure-mobile.net/tables/dummyTable?$inlinecount=allpages&$orderby=id&$top=10&$skip=10")
            .respond(dummyResult);
        table.query(2);
        $httpBackend.flush();
    });

    it(" should be issuing a POST to Azure Mobile Service for insert", () => {
        $httpBackend.expectPOST("https://dummyService.azure-mobile.net/tables/dummyTable")
            .respond(201 /* Created */);

        var data: IDummyRow = {};
        table.insert(data);
        $httpBackend.flush();
    });

    it(" should issue a DELETE to Azure Mobile Service for delete", () => {
        $httpBackend.expectDELETE("https://dummyService.azure-mobile.net/tables/dummyTable/1")
            .respond(204 /* No Content */);

        table.deleteItemById(1);
        $httpBackend.flush();
    });

    it(" should issue a PATCH to Azure Mobile Service for delete", () => {
        $httpBackend.expect("PATCH", "https://dummyService.azure-mobile.net/tables/dummyTable/1", "{\"id\":1}")
            .respond(200 /* OK */);

        table.update({ id: 1 });
        $httpBackend.flush();
    });

    afterEach(() => {
        $httpBackend.verifyNoOutstandingExpectation();
        $httpBackend.verifyNoOutstandingRequest();
    });
});
/// <reference path="../scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../scripts/typings/jquery/jquery.d.ts" />

interface ICustomer {
    ID: number;
    LastName: string;
    FirstName: string;
    HouseNumber: string;
    Street: string;
    City: string;
    State: string;
    ZipCode: string;
}

interface ICustomerScope extends ng.IScope {
    username: string;
    password: string;
    customers: ICustomer[];

    retrieveCustomers: () => void;
}

class SampleController {
    constructor($scope: ICustomerScope) {
        $scope.retrieveCustomers = () => this.retrieveCustomers($scope);
    }

    private retrieveCustomers($scope: ICustomerScope) : void {
        var basicAuthBase64 = btoa($scope.username + ":" + $scope.password);
        $.ajax("/api/Customer", { headers: { Authorization: "Basic " + basicAuthBase64 } })
            .done(result => {
                $scope.customers = result;
                $scope.$apply();
            });
    }
}

angular.module("WebApiSample", [])
    .controller("Controller", SampleController);
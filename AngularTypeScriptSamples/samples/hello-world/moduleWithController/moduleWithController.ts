/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>

interface IHelloScope extends ng.IScope {
    name: string;
}

module Hello {
    export class HelloCtrl {
        constructor($scope: IHelloScope) {
            $scope.name = "World!";
        }
    }
}

angular.module("Hello", [])
    .controller("HelloCtrl", Hello.HelloCtrl);


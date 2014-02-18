module Hello {
    export interface IHelloScope extends ng.IScope {
        name: string;
    }

    export class HelloCtrl {
        constructor($scope: IHelloScope) {
            $scope.name = "World!";
        }
    }
}

angular.module("Hello", [])
    .controller("HelloCtrl", Hello.HelloCtrl);


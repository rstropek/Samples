/// <reference path="../../../tsDeclarations/angularjs/angular.d.ts"/>
var HelloCtrl = function ($scope) {
    $scope.name = "World";
    $scope.getName = function () {
        return $scope.name;
    };
};
//# sourceMappingURL=code.js.map

/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>

var HelloCtrl = function ($scope) {
    $scope.name = "World";
    $scope.countries = [
        { isoCode: 'AT', name: 'Austria' },
        { isoCode: 'DE', name: 'Germany' },
        { isoCode: 'CH', name: 'Switzerland' }];
    $scope.getName = function () {
        return $scope.name;
    };
    $scope.getEnclosedName = function (tag) {
        return "<" + tag + ">" + $scope.name + "<" + tag + "/>";
    };
};
//# sourceMappingURL=helloWorldWithController.js.map

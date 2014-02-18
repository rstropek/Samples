/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>

var Hello;
(function (Hello) {
    var HelloCtrl = (function () {
        function HelloCtrl($scope) {
            $scope.name = "World!";
        }
        return HelloCtrl;
    })();
    Hello.HelloCtrl = HelloCtrl;
})(Hello || (Hello = {}));

angular.module("Hello", []).controller("HelloCtrl", Hello.HelloCtrl);
//# sourceMappingURL=moduleWithController.js.map

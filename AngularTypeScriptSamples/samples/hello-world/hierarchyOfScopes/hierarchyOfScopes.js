/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>

var WorldCtrl = function ($scope) {
    $scope.population = 7000;
    $scope.countries = [
        { name: "France", population: 63.1 },
        { name: "United Kingdom", population: 61.8 }
    ];
    $scope.worldsPercentage = function (countryPopulation) {
        return (countryPopulation / $scope.population) * 100;
    };
};
//# sourceMappingURL=hierarchyOfScopes.js.map

interface ICountry {
    name: string;
    population: number;
}

interface IHierarchyScope extends ng.IScope {
    population: number;
    countries: ICountry[];
    worldsPercentage: (countryPopulation: number) => number;
}

var WorldCtrl = function ($scope: IHierarchyScope) {
    $scope.population = 7000;
    $scope.countries = [
        { name: "France", population: 63.1 },
        { name: "United Kingdom", population: 61.8 }
    ];
    $scope.worldsPercentage = function (countryPopulation) {
        return (countryPopulation / $scope.population) * 100;
    };
};
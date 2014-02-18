// Create a custom scope based on angular's scope and define
// type-safe members which we will add in the controller function.
interface IHelloWorldScope extends ng.IScope {
    name: string;
    countries: ICountryInfo[];
    getName: () => string;
    getEnclosedName: (tag: string) => string;
}

interface ICountryInfo {
    isoCode: string;
    name: string;
}

var HelloCtrl = function ($scope: IHelloWorldScope) {
    // Note that custom scope interface gives us type-safe access to
    // the underlying model.
    $scope.name = "World";
    $scope.countries = [
        { isoCode: 'AT', name: 'Austria' },
        { isoCode: 'DE', name: 'Germany' },
        { isoCode: 'CH', name: 'Switzerland' }];
    $scope.getName = () => $scope.name;
    $scope.getEnclosedName = (tag) => "<" + tag + ">" + $scope.name + "<" + tag + "/>";
};
/// <reference path="../../../../typings/tsd.d.ts" />
/// <reference path="../../../../app/client/register/register.ts" />
/// <reference path="../../../../app/client/register/mobileServicesTableService.ts" />

angular.module("EventRSVPTest", [])
	.factory("reservationTable", ["$http", ($http: ng.IHttpService) =>
        new MobileServicesDataAccess.Table($http, "dummyService", "dummyTable")])
	.controller("RegisterController", Registration.RegisterController);
  
describe('RegisterController', function() {
  beforeEach(angular.mock.module('EventRSVPTest'));

  var controller : Registration.RegisterController;
  var $httpBackend: ng.IHttpBackendService;
  beforeEach(inject(function(_$controller_: ng.IControllerService, _$rootScope_: ng.IRootScopeService, _$httpBackend_: ng.IHttpBackendService){
    // The injector unwraps the underscores (_) from around the parameter names when matching
    var $scope = <Registration.IRegisterControllerScope>_$rootScope_.$new();
    $httpBackend = _$httpBackend_;
    controller = <Registration.RegisterController>_$controller_('RegisterController', { $scope: $scope });
  }));
  
  describe('showIncompleteDataError', function() {
    it('is orginally false', function() {
      expect(controller.showIncompleteDataError).toBeFalsy();
    });
    it('turns true if tried to submit incomplete data', function() {
      controller.saveRegistration();
      expect(controller.showIncompleteDataError).toBeTruthy();
    });
  });
  
  describe('registration data', function() {
    it('is is stored if data is complete', function() {
      controller.firstName = controller.lastName = "dummy";
      controller.email = "dummy@dummy.com";
      controller.saveRegistration();

      $httpBackend.expectPOST("https://dummyService.azure-mobile.net/tables/dummyTable")
          .respond(201 /* Created */);
      $httpBackend.flush();
      $httpBackend.verifyNoOutstandingExpectation();
      $httpBackend.verifyNoOutstandingRequest();
      
      expect(controller.showSuccess).toBeTruthy();
    });
  });
});

/// <reference path="../../../typings/tsd.d.ts" />
/// <reference path="../../../app/client/register.ts" />
/// <reference path="../../../app/client/mobileServicesTableService.ts" />

angular.module("EventRSVPTest", [])
	.factory("reservationTable", ["$http", ($http: ng.IHttpService) =>
        new MobileServicesDataAccess.Table($http, "dummyService", "dummyTable")])
	.controller("RegisterController", Registration.RegisterController);
  
describe('RegisterController', function() {
  beforeEach(angular.mock.module('EventRSVPTest'));

  var controller : Registration.RegisterController;
  var $httpBackend: ng.IHttpBackendService;
  beforeEach(inject(function(_$controller_: ng.IControllerService, _$rootScope_: ng.IRootScopeService, _$httpBackend_: ng.IHttpBackendService){
    var $scope = <Registration.IRegisterControllerScope>_$rootScope_.$new();
    $httpBackend = _$httpBackend_;
    controller = <Registration.RegisterController>_$controller_('RegisterController', { $scope: $scope });
  }));
  
  describe('showIncompleteDataError', function() {
    it(' is orginally false', function() {
      expect(controller.showIncompleteDataError).toBeFalsy();
    });
    it(' turns true if tried to submit incomplete data', function() {
      controller.saveRegistration();
      expect(controller.showIncompleteDataError).toBeTruthy();
    });
  });
  
  describe('Registration data', function() {
    it(' is is stored if data is complete', function() {
      // Fill form with valid data
      controller.firstName = controller.lastName = "dummy";
      controller.email = "dummy@dummy.com";
      
      // Expect REST insert call when calling saveRegistration
      $httpBackend.expectPOST("https://dummyService.azure-mobile.net/tables/dummyTable")
          .respond(201 /* Created */);
      controller.saveRegistration();
      $httpBackend.flush();
      
      // Verify that all expected requests appeared
      $httpBackend.verifyNoOutstandingExpectation();
      $httpBackend.verifyNoOutstandingRequest();
      
      // Check if controller set showSuccess
      expect(controller.showSuccess).toBeTruthy();
    });
  });
});

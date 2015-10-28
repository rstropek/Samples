/// <reference path="typings/tsd.d.ts" />
/// <reference path="./demoController.ts" />

// Setup module for unit testing. Note that we are mocking
// "InvalidLastName" here.
angular.module("DemoTest", [])
  .constant("InvalidLastName", "Muster2")
 	.controller("DemoController", DemoController);

describe('DemoController', () => {
  // Specify that we use "DemoTest" module for testing
  beforeEach(angular.mock.module('DemoTest'));

  // Setup dependency injection for each test
  var controller : DemoController;
  var $scope: any;
  var $httpBackend: ng.IHttpBackendService;
  beforeEach(inject(function(_$controller_: ng.IControllerService, _$rootScope_: ng.IRootScopeService, _$httpBackend_: ng.IHttpBackendService){
    $scope = <ng.IScope>_$rootScope_.$new();
    $httpBackend = _$httpBackend_;
    controller = <DemoController>_$controller_('DemoController', { $scope: $scope });
  }));
  
  describe('validity status', () => {
    it('is originally valid', () => {
      expect(controller.isValid).toBeTruthy();
    });
    
    it('turns to false if invalid data is entered', () => {
      // Set last name and initiate digest cycle so that $watch
      // will be fired.
      controller.lastName = "Muster2";
      $scope.$digest();
      
      expect(controller.isValid).toBeFalsy();
    });
  });

  describe('customer details', () => {
    it('queries server for details', () => {
      var dummyDetails : ICustomerDetails = { address: "Anywhere 99, Somewhere" }; 
      
      // Use $http mockup service to simulate response of web api
      $httpBackend.expectGET("https://myserver.com/api/getCustomerDetails")
          .respond(200, dummyDetails);

      controller.getDetails();
      $httpBackend.flush();
      expect(controller.details.address).toBe(dummyDetails.address);

      // Verify that all expected requests appeared
      $httpBackend.verifyNoOutstandingExpectation();
      $httpBackend.verifyNoOutstandingRequest();      
    });
  });
});
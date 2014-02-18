/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>
/// <reference path="NotificationsArchive.ts"/>
/// <reference path="NotificationsService.ts"/>
/// <reference path="NotificationsCtrl.ts"/>

angular.module("notificationsApp", [])
    .value("greeting", "Hello World")
    .constant("MAX_LEN", 10)
    .factory("notificationsArchive", () => new NotificationsModule.NotificationsArchive())
    .factory("notificationService", NotificationsModule.NotificationsService.Factory)
    .controller("NotificationsCtrl", NotificationsModule.NotificationsCtrl);

angular.module("notificationsApp", [])
    .value("greeting", "Hello World")
    .constant("MAX_LEN", 10)
    .factory("notificationsArchive", () => new NotificationsModule.NotificationsArchive())
    .factory("notificationService", NotificationsModule.NotificationsService.Factory)
    .controller("NotificationsCtrl", NotificationsModule.NotificationsCtrl);

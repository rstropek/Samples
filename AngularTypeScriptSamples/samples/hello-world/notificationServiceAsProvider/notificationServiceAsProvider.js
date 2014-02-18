/// <reference path="../../../tsDeclarations/angularjs/angular.d.ts"/>
/// <reference path="notificationsArchive.ts"/>
/// <reference path="notifications.ts"/>
var NotificationsModule;
(function (NotificationsModule) {
    var NotificationsCtrl = (function () {
        function NotificationsCtrl($scope, notificationService) {
            this.$scope = $scope;
            this.notificationService = notificationService;
            $scope.addNotification = this.addNotification;
            $scope.getNotifications = this.getNotifications;
            $scope.vm = this;
        }
        NotificationsCtrl.prototype.addNotification = function () {
            this.notificationService.push(this.$scope.notification);
            this.$scope.notification = "";
        };

        NotificationsCtrl.prototype.getNotifications = function () {
            return this.notificationService.getCurrent();
        };
        return NotificationsCtrl;
    })();
    NotificationsModule.NotificationsCtrl = NotificationsCtrl;
})(NotificationsModule || (NotificationsModule = {}));

angular.module("notificationsApp", []).constant("MAX_LEN", 10).controller("NotificationsCtrl", NotificationsModule.NotificationsCtrl).factory("notificationService", NotificationsModule.NotificationsService.Factory).factory("notificationsArchive", NotificationsModule.NotificationsArchive.Factory);
//# sourceMappingURL=notificationServiceAsProvider.js.map

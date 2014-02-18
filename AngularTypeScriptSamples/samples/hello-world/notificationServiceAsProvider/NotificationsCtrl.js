/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>
/// <reference path="NotificationsArchive.ts"/>
var NotificationsModule;
(function (NotificationsModule) {
    var NotificationsCtrl = (function () {
        function NotificationsCtrl($scope, notificationService) {
            this.$scope = $scope;
            this.notificationService = notificationService;
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
//# sourceMappingURL=NotificationsCtrl.js.map

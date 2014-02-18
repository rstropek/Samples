/// <reference path="INotificationsArchive.ts"/>
var NotificationsModule;
(function (NotificationsModule) {
    var NotificationsArchive = (function () {
        function NotificationsArchive() {
            this.archivedNotifications = [];
        }
        NotificationsArchive.prototype.archive = function (notification) {
            this.archivedNotifications.push(notification);
        };

        NotificationsArchive.prototype.getArchived = function () {
            return this.archivedNotifications;
        };
        return NotificationsArchive;
    })();
    NotificationsModule.NotificationsArchive = NotificationsArchive;
})(NotificationsModule || (NotificationsModule = {}));
//# sourceMappingURL=NotificationsArchive.js.map

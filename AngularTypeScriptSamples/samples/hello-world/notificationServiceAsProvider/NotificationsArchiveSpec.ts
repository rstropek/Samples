angular.module("notificationsApp", [])
    .factory("notificationsArchive", () => new NotificationsModule.NotificationsArchive());

describe("Notifications Archive Tests", function () {
    var notificationsArchive: NotificationsModule.INotificationsArchive;
    beforeEach(module('notificationsApp'));
    beforeEach(inject(function (_notificationsArchive_) {
        notificationsArchive = _notificationsArchive_;
    }));

    it(' should give access to the archived items', function () {
        var notification = 'Old message.';
        notificationsArchive.archive(notification);
        expect(notificationsArchive.getArchived()).toContain(notification);
    });
});
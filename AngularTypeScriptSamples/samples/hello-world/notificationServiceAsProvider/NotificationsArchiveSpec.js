angular.module("notificationsApp", []).factory("notificationsArchive", function () {
    return new NotificationsModule.NotificationsArchive();
});

describe("Notifications Archive Tests", function () {
    var notificationsArchive;
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
//# sourceMappingURL=NotificationsArchiveSpec.js.map

/// <reference path="../../../tsDeclarations/jasmine/jasmine.d.ts"/>
/// <reference path="../../../tsDeclarations/angularjs/angular.d.ts"/>
/// <reference path="../../../tsDeclarations/angularjs/angular-mocks.d.ts"/>
describe("Notifications Archive Tests", function () {
    var notificationsArchive;
    beforeEach(module('notificationsArchive'));
    beforeEach(inject(function (_notificationsArchive_) {
        notificationsArchive = _notificationsArchive_;
    }));

    it(' should give access to the archived items', function () {
        var notification = { msg: 'Old message.' };
        notificationsArchive.archive(notification);
        expect(notificationsArchive.getArchived()).toContain(notification);
    });
});
//# sourceMappingURL=NotificationsArchiveSpec.js.map

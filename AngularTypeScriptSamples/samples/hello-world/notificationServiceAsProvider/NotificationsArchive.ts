/// <reference path="INotificationsArchive.ts"/>

module NotificationsModule {
    export class NotificationsArchive implements INotificationsArchive {
        private archivedNotifications: string[];

        constructor() {
            this.archivedNotifications = [];
        }

        archive(notification: string) {
            this.archivedNotifications.push(notification);
        }

        public getArchived(): string[]{
            return this.archivedNotifications;
        }
    }
}
module NotificationsModule {
    export interface INotificationsArchive {
        archive(notification: string);
        getArchived(): string[];
    }
}
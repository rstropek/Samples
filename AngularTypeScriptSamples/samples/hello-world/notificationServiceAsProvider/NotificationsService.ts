module NotificationsModule {
    export class NotificationsService {
        private notifications: string[];

        public maxLen: number = 10;

        public static Factory(notificationsArchive: INotificationsArchive, MAX_LEN: number, greeting: string) {
            return new NotificationsService(notificationsArchive, MAX_LEN, greeting);
        }

        constructor(private notificationsArchive: INotificationsArchive, MAX_LEN: number, greeting: string) {
            this.notifications = [];
            this.maxLen = MAX_LEN;
        }

        public push(notification: string): void {
            var notificationToArchive: string;
            var newLen = this.notifications.unshift(notification);
            if (newLen > this.maxLen) {
                notificationToArchive = this.notifications.pop();
                this.notificationsArchive.archive(notificationToArchive);
            }
        }

        public getCurrent(): string[] {
            return this.notifications;
        }
    }
}

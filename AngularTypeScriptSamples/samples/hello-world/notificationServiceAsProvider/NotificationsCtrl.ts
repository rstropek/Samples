/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>
/// <reference path="NotificationsArchive.ts"/>

module NotificationsModule {
    export interface INotificationsCtrlScope extends ng.IScope {
        notification: string;
        vm: NotificationsCtrl;
    }

    export class NotificationsCtrl {
        constructor(private $scope: INotificationsCtrlScope, private notificationService: NotificationsService) {
            $scope.vm = this;
        }

        private addNotification(): void {
            this.notificationService.push(this.$scope.notification);
            this.$scope.notification = "";
        }

        private getNotifications(): string[] {
            return this.notificationService.getCurrent();
        }
    }
}
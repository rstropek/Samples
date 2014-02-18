/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>
/// <reference path="MobileServicesTable.ts"/>

module HttpServiceModule {
    export interface IEvent extends MobileServicesDataAccess.ITableRow {
        eventTitle: string;
        eventCategory: string;
        eventDescription: string;
        eventDate: Date;
        maximumParticipants: number;
    }

    export interface IServiceModuleScope extends ng.IScope {
        vm: HttpServiceController;

        events: IEvent[];
        currentEvent: IEvent;

        loading: boolean;

        gridOptions: any;
        paginationItemsPerPage: number;
        totalItems: number;
        currentPage: number;
    }

    export class HttpServiceController {
        constructor(private $scope: IServiceModuleScope, paginationItemsPerPage: number,
            private eventTable: MobileServicesDataAccess.ITable<IEvent>, private $q: ng.IQService) {
            $scope.vm = this;

            $scope.events = [];
            $scope.gridOptions = {
                data: 'events',
                totalServerItems: 'totalItems',
                showFooter: true,
                columnDefs: [
                    { field: "eventCategory", displayName: "Category" },
                    { field: "eventTitle", displayName: "Title" },
                    { field: "eventDescription", displayName: "Description" },
                    { field: "eventDate", displayName: "Date", cellFilter: "date" },
                    { field: "maximumParticipants", displayName: "Participant Limit", cellFilter: "number:0" }
                ]
            };

            $scope.paginationItemsPerPage = paginationItemsPerPage;
            $scope.totalItems = 0;
            $scope.currentPage = 1;

            $scope.$watch("currentPage", (_, __) => this.getEvents());

            this.addEvent = () => this.addEventInternal();
            this.getEvents = () => this.getEventsInternal();
            this.deleteEvents = () => this.deleteEventsInternal();

            $scope.loading = false;
            $scope.currentEvent = {
                eventCategory: "Concert", eventTitle: "",
                eventDescription: "", eventDate: new Date(), maximumParticipants: 1000
            };
            this.getEvents();
        }

        public addEvent: () => void;
        public getEvents: () => void;
        public deleteEvents: () => void;

        private getEventsInternal(): void {
            this.$scope.loading = true;
            var current = this;
            this.$scope.events = [];
            this.eventTable
                .query(this.$scope.currentPage)
                .success(result => {
                    current.$scope.events = result.results;
                    current.$scope.totalItems = result.count;
                    current.$scope.loading = false;
                });
        }

        private addEventInternal() {
            var current = this;
            this.$scope.loading = true;
            this.eventTable.insert(this.$scope.currentEvent).then(() => {
                current.getEvents();
                current.$scope.currentPage = 1;
            });
        }

        private deleteEventsInternal() {
            var current = this;
            this.$scope.loading = true;
            this.$scope.events = [];
            this.eventTable.query().success(result => {
                current.$q.all(result.results.map(current.eventTable.deleteItem))
                    .then(() => {
                        current.getEvents();
                        current.$scope.currentPage = 1;
                    });
            });
        }

        private generateEvents(numberOfEvents?: number): void {
            var current = this;
            this.$scope.loading = true;
            this.$scope.events = [];
            var events: IEvent[] = [];
            numberOfEvents = numberOfEvents || 25;

            for (var i = 0; i < (numberOfEvents / 2); i++) {
                events.push({
                    eventCategory: "Concert",
                    eventDescription: "Artist " + i.toString() + " live in concert at central opera hall",
                    eventTitle: "Artist " + i.toString() + " live in concert",
                    eventDate: new Date(2013, Math.random() * 100 % 12 + 1, Math.random() * 100 % 28 + 1),
                    maximumParticipants: i * 10000
                });
            }

            for (var i = (numberOfEvents / 2); i < numberOfEvents; i++) {
                events.push({
                    eventCategory: "Sport Event",
                    eventDescription: "Soccer Championship " + i.toString() + ". Who will be the new champion?",
                    eventTitle: "Soccer Campionship " + i.toString(),
                    eventDate: new Date(2013, Math.random() * 100 % 12 + 1, Math.random() * 100 % 28 + 1),
                    maximumParticipants: i * 10000
                });
            }

            this.$q.all(events.map(event => this.eventTable.insert(event)))
                .then(() => {
                    current.getEvents();
                    current.$scope.currentPage = 1;
                });
        }
    }
}


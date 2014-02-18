/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>
/// <reference path="MobileServicesTable.ts"/>
var HttpServiceModule;
(function (HttpServiceModule) {
    var HttpServiceController = (function () {
        function HttpServiceController($scope, paginationItemsPerPage, eventTable, $q) {
            var _this = this;
            this.$scope = $scope;
            this.eventTable = eventTable;
            this.$q = $q;
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

            $scope.$watch("currentPage", function (_, __) {
                return _this.getEvents();
            });

            this.addEvent = function () {
                return _this.addEventInternal();
            };
            this.getEvents = function () {
                return _this.getEventsInternal();
            };
            this.deleteEvents = function () {
                return _this.deleteEventsInternal();
            };

            $scope.loading = false;
            $scope.currentEvent = {
                eventCategory: "Concert", eventTitle: "",
                eventDescription: "", eventDate: new Date(), maximumParticipants: 1000
            };
            this.getEvents();
        }
        HttpServiceController.prototype.getEventsInternal = function () {
            this.$scope.loading = true;
            var current = this;
            this.$scope.events = [];
            this.eventTable.query(this.$scope.currentPage).success(function (result) {
                current.$scope.events = result.results;
                current.$scope.totalItems = result.count;
                current.$scope.loading = false;
            });
        };

        HttpServiceController.prototype.addEventInternal = function () {
            var current = this;
            this.$scope.loading = true;
            this.eventTable.insert(this.$scope.currentEvent).then(function () {
                current.getEvents();
                current.$scope.currentPage = 1;
            });
        };

        HttpServiceController.prototype.deleteEventsInternal = function () {
            var current = this;
            this.$scope.loading = true;
            this.$scope.events = [];
            this.eventTable.query().success(function (result) {
                current.$q.all(result.results.map(current.eventTable.deleteItem)).then(function () {
                    current.getEvents();
                    current.$scope.currentPage = 1;
                });
            });
        };

        HttpServiceController.prototype.generateEvents = function (numberOfEvents) {
            var _this = this;
            var current = this;
            this.$scope.loading = true;
            this.$scope.events = [];
            var events = [];
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

            this.$q.all(events.map(function (event) {
                return _this.eventTable.insert(event);
            })).then(function () {
                current.getEvents();
                current.$scope.currentPage = 1;
            });
        };
        return HttpServiceController;
    })();
    HttpServiceModule.HttpServiceController = HttpServiceController;
})(HttpServiceModule || (HttpServiceModule = {}));
//# sourceMappingURL=HttpServiceController.js.map

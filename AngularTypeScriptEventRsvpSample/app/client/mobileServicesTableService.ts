/// <reference path="../../typings/tsd.d.ts" />

module MobileServicesDataAccess {
    // Acts as the base type for interfaces representing
    // rows of Azure Mobile Services tables. It just contains an id
    // column as every table in Azure Mobile Services must have at least
    // this primary key.
    export interface ITableRow {
        id?: number;
    }

    // Implements a class used to access an Azure Mobile Services table.
    export class Table<T extends ITableRow> {
        constructor(private $http: ng.IHttpService, private serviceName: string, private tableName: string) {
        }

        // Note that the query method returns a type-safe promise referencing the
        // interface for a query result.
        public query(): ng.IHttpPromise<T[]> {
            var uri = this.buildBaseUri() + "?$orderby=id";

            // Returns a promise representing the async web request
            return this.$http.get(uri);
        }

        public insert(item: T): ng.IHttpPromise<any> {
            return this.$http.post(this.buildBaseUri(), item);
        }

        private buildBaseUri(): string {
            return "https://" + this.serviceName + ".azure-mobile.net/tables/" + this.tableName;
        }
    }
}

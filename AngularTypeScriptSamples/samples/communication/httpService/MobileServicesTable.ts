/// <reference path="../../../Scripts/typings/angularjs/angular.d.ts"/>

module MobileServicesDataAccess {
    export interface ITableRow {
        id?: number;
    }

    export interface ITable<T extends ITableRow> {
        query: (page?: number) => ng.IHttpPromise<IQueryResult<T>>;
        insert: (item: T) => ng.IHttpPromise<any>;
        update: (item: T) => ng.IHttpPromise<any>;
        deleteItem: (item: T) => ng.IHttpPromise<any>;
        deleteItemById: (id: number) => ng.IHttpPromise<any>;
    }

    export interface IQueryResult<T extends ITableRow> {
        results: T[];
        count: number;
    }

    export class Table<T extends ITableRow> implements ITable<T> {
        constructor(private $http: ng.IHttpService, private serviceName: string, private tableName: string, private pageSize: number, private apiKey: string) {
            // Set public methods using lambdas for proper "this" handling
            this.query = (page?) => this.queryInternal(page);
            this.insert = (item) => this.insertInternal(item);
            this.update = (item) => this.updateInternal(item);
            this.deleteItem = (id) => this.deleteItemInternal(id);
            this.deleteItemById = (id) => this.deleteItemByIdInternal(id);

            // Build http header with mobile service application key
            this.header = {
                headers: {
                    "X-ZUMO-APPLICATION": apiKey
                }
            };
        }

        public query: (page?: number) => ng.IHttpPromise<IQueryResult<T>>;
        public insert: (item: T) => ng.IHttpPromise<any>;
        public update: (item: T) => ng.IHttpPromise<any>;
        public deleteItem: (item: T) => ng.IHttpPromise<any>;
        public deleteItemById: (id: number) => ng.IHttpPromise<any>;

        private header: any;

        private queryInternal(page?: number): ng.IHttpPromise<IQueryResult<T>> {
            var uri = this.buildBaseUri() + "?$inlinecount=allpages&$orderby=id";
            if (page !== undefined) {
                // Add "skip" and "top" clause for paging
                uri += "&$top=" + this.pageSize.toString();
                if (page > 1) {
                    var skip = (page - 1) * this.pageSize;
                    uri += "&$skip=" + skip.toString();
                }
            }

            return this.$http.get(uri, this.header);
        }

        private insertInternal(item: T): ng.IHttpPromise<any> {
            return this.$http.post(this.buildBaseUri(), item, this.header);
        }

        private updateInternal(item: T): ng.IHttpPromise<any> {
            var uri = this.buildBaseUri() + "/" + item.id.toString();
            return this.$http({ method: "PATCH", url: uri, headers: this.header, data: item });
        }

        private deleteItemInternal(item: T): ng.IHttpPromise<any> {
            return this.deleteItemByIdInternal(item.id);
        }

        private deleteItemByIdInternal(id: number): ng.IHttpPromise<any> {
            var uri = this.buildBaseUri() + "/" + id.toString();
            return this.$http.delete(uri, this.header);
        }

        private buildBaseUri(): string {
            return "https://" + this.serviceName + ".azure-mobile.net/tables/" + this.tableName;
        }
    }
}

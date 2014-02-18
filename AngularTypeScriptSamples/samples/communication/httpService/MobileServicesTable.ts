module MobileServicesDataAccess {
    // Acts as the base type for interfaces representing
    // rows of Azure Mobile Services tables. It just contains an id
    // column as every table in Azure Mobile Services must have at least
    // this primary key.
    export interface ITableRow {
        id?: number;
    }

    // Contains async operations used to interact with
    // an Azure Mobile Services table.
    export interface ITable<T extends ITableRow> {
        query: (page?: number) => ng.IHttpPromise<IQueryResult<T>>;
        insert: (item: T) => ng.IHttpPromise<any>;
        update: (item: T) => ng.IHttpPromise<any>;
        deleteItem: (item: T) => ng.IHttpPromise<any>;
        deleteItemById: (id: number) => ng.IHttpPromise<any>;
    }

    // Represents a query result consisting of the total
    // result count (independent of which page was requested)
    // and the results for the requested page.
    export interface IQueryResult<T extends ITableRow> {
        results: T[];
        count: number;
    }

    // Implements a class used to access an Azure Mobile Services table.
    export class Table<T extends ITableRow> implements ITable<T> {
        constructor(private $http: ng.IHttpService, private serviceName: string, private tableName: string,
            private pageSize: number, private apiKey: string) {
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

        // Note that the query method returns a type-safe promise referencing the
        // interface for a query result.
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

            // Returns a promise representing the async web request
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

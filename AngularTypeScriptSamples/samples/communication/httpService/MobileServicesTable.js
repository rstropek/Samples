var MobileServicesDataAccess;
(function (MobileServicesDataAccess) {
    var Table = (function () {
        function Table($http, serviceName, tableName, pageSize, apiKey) {
            var _this = this;
            this.$http = $http;
            this.serviceName = serviceName;
            this.tableName = tableName;
            this.pageSize = pageSize;
            this.apiKey = apiKey;
            // Set public methods using lambdas for proper "this" handling
            this.query = function (page) {
                return _this.queryInternal(page);
            };
            this.insert = function (item) {
                return _this.insertInternal(item);
            };
            this.update = function (item) {
                return _this.updateInternal(item);
            };
            this.deleteItem = function (id) {
                return _this.deleteItemInternal(id);
            };
            this.deleteItemById = function (id) {
                return _this.deleteItemByIdInternal(id);
            };

            // Build http header with mobile service application key
            this.header = {
                headers: {
                    "X-ZUMO-APPLICATION": apiKey
                }
            };
        }
        Table.prototype.queryInternal = function (page) {
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
        };

        Table.prototype.insertInternal = function (item) {
            return this.$http.post(this.buildBaseUri(), item, this.header);
        };

        Table.prototype.updateInternal = function (item) {
            var uri = this.buildBaseUri() + "/" + item.id.toString();
            return this.$http({ method: "PATCH", url: uri, headers: this.header, data: item });
        };

        Table.prototype.deleteItemInternal = function (item) {
            return this.deleteItemByIdInternal(item.id);
        };

        Table.prototype.deleteItemByIdInternal = function (id) {
            var uri = this.buildBaseUri() + "/" + id.toString();
            return this.$http.delete(uri, this.header);
        };

        Table.prototype.buildBaseUri = function () {
            return "https://" + this.serviceName + ".azure-mobile.net/tables/" + this.tableName;
        };
        return Table;
    })();
    MobileServicesDataAccess.Table = Table;
})(MobileServicesDataAccess || (MobileServicesDataAccess = {}));
//# sourceMappingURL=MobileServicesTable.js.map

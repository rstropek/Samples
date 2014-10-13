/// <reference path="../lib/jquery.d.ts" />
import cust = require("app/classes/customer");

export class AppMain {
    public run() {
        $.get("/Customer/99").done(function (data) {
            var c = new cust.customer.Customer(JSON.parse(data));
            $("#fullname").text(c.fullName());
        });
    }
}
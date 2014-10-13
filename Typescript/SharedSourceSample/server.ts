/// <reference path="../../../tsd/DefinitelyTyped/express/express.d.ts" />
/// <reference path="./customer.ts" />
import express = require("express");
import crm = require("./customer");

var app = express();

app.get("/customer/:id", function (req, resp) {
    var customerId = <number>req.params.id;
    var c = new crm.customer.Customer({ firstName: "Max" + customerId.toString(), lastName: "Muster" });
    console.log(c.fullName()); 
    resp.send(JSON.stringify(c));
});

app.get("/customer", function (req, resp) {
    var customers: crm.customer.Customer [];
    customers = new Array();
    for (var i = 0; i<10; i++) {
        customers.push(new crm.customer.Customer({ firstName: "Max" + i.toString(), lastName: "Muster" }));
    }
    resp.send(JSON.stringify(customers));
});

app.use("/", express.static(__dirname + "/website/"));

var port = process.env.PORT || 1337; 
app.listen(port);
/// <reference path="typings/tsd.d.ts" />
import express = require('express');
import bodyParser = require('body-parser');
import cors = require('cors');

var app = express();
var port = process.env.port || 1337;
app.use(bodyParser.json());
app.use(express.static("client"));

var corsOptions = {
  origin: 'http://example.com'
};
app.use(cors(corsOptions));

interface ICustomer {
    _id?: number;
    firstName: string;
    lastName: string;
}

var customerId: number = 0;
var repository: ICustomer[] = [];

app.get('/customers', (req, res) => {
    if (repository.length) {
        res.status(200).send(repository);
    } else {
        res.status(404).end();
    }
});

app.post('/customers', (req, res) => {
    var customer = <ICustomer>req.body;
    if (customer && customer.firstName && customer.lastName) {
        var newCustomer = { _id: ++customerId, firstName: customer.firstName, lastName: customer.lastName };
        repository.push(newCustomer);
        res.setHeader("Location", "/customers/" + newCustomer._id);
        res.status(201).send(newCustomer);
    } else {
        res.status(400).end();
    }
});

app.get('/customers/:customerId', (req, res) => {
    var id = parseInt(req.params.customerId);
    if (isNaN(id)) {
      res.status(400).end();
    } else {
      var result = repository.filter((c, _, __) => c._id == id);
      if (result.length) {
        res.status(200).send(result[0]);
      } else {
        res.status(404).end();
      }
    }
});

var server = app.listen(port, function () {
  console.log('Listening on port %s', port);
});
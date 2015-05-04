import express = require('express');
var app = express();

app.get('/', (req, res) => {
    res.send('Hello World!');
});

app.get('/customer/:id',(req, res) => {
    var customer = {
        customerId: req.params.id,
        customerName: "Customer " + req.params.id
    };
    res.status(200).send(customer);
});

var server = app.listen(
    1337,() => console.log("Listening on port 1337 ..."));
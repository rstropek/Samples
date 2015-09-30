/// <reference path="../typings/tsd.d.ts" />

import express = require('express');

var app = express();
app.use("/", express.static("client"));
app.use("/tests", express.static("tests/client"));

var port = process.env.port || 1337;
var server = app.listen(port, function () {
  console.log('Listening on port %s', port);
});
import http = require("http");

var server = http.createServer((req, resp) => {
    console.log("Got request for " + req.url);

    resp.writeHead(200, { "Content-Type": "text/html" });
    resp.write('<!DOCTYPE "html"><html><body><h1>Hello from ');
    resp.write(req.url);
    resp.write('!</h1></body></html>');
    resp.end();
});

server.listen(1337);
console.log("Listening on port 1337 ...");

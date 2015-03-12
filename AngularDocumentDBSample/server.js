var express = require("express");
var app = express();
var bodyParser = require('body-parser');
var config = require("./config");
var DocumentDBClient = require('documentdb').DocumentClient;
var needle = require('needle');
var mandrill = require('mandrill-api/mandrill');

// Configure JSON parser
app.use(bodyParser.json());

// Handler for register API
app.post("/api/register", function (req, resp) {
    // check if body contains necessary fields
    var registration = req.body;
    if (registration && registration.firstName && registration.lastName
        && registration.email && registration.recaptchaResponse) {
        // check captcha
        needle.post(
            "https://www.google.com/recaptcha/api/siteverify",
            "secret=" + config.recaptchaSecret + "&response=" + registration.recaptchaResponse,
            {},
            function (err, needleResp) {
                if (!err && needleResp && needleResp.body && needleResp.body.success)
                {
                    // captcha is ok
                    
                    // save registration to docdb
                    var docDbClient = new DocumentDBClient(config.host, { masterKey: config.authKey });
                    docDbClient.createDocument(
                        config.collectionPath,
                        { 
                            "firstName": registration.firstName, 
                            "lastName": registration.lastName, 
                            "email": registration.email,
                            "registrationDate": Date.now()
                        },
                        function (err, doc) {
                            if (err) {
                                console.log("Error while creating document (" + JSON.stringify(err) + ")");
                                resp.sendStatus(500); // internal server error
                            } else {
                                // Create a mandrill client using mandrill's SDK
                                var mandrill_client = new mandrill.Mandrill(config.mandrillKey);
                                 
                                // The email content is driven by a template. Here we set up the
                                // merge fields used in the template.
                                var mergeVariables = [
                                    { "name": "FIRSTNAME", "content": registration.firstName },
                                    { "name": "LASTNAME", "content": registration.lastName },
                                    { "name": "EMAIL", "content": registration.email }
                                ];
                                var template_name = "Leondinger Bienen Registrierung";
                                var message = {
                                    "to": config.infoMailReceipients,
                                    "merge": true,
                                    "global_merge_vars": mergeVariables
                                };
                                 
                                // Send email using Mandrill
                                mandrill_client.messages.sendTemplate({
                                    "template_name": template_name, 
                                    "template_content": null, 
                                    "message": message});
                                
                                console.log("success");
                                resp.sendStatus(200); // OK
                            }
                        });
                }
                else {
                    console.log("Invalid captcha");
                    console.log("err: " + err);
                    console.log("response body: " + needleResp.body);
                    resp.sendStatus(400); // invalid request
                }
            });
    }
    else {
        console.log("Invalid registration: " + JSON.stringify(registration));
        resp.sendStatus(400); // invalid request
    }
});

app.use("/", express.static(__dirname + "/site/"));

var port = process.env.PORT || 1337; 
app.listen(port);
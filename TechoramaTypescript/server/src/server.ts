/// <reference path="../typings/main.d.ts" />
import * as express from "express";
import * as cors from "cors";
import * as bodyParser from "body-parser";
import * as config from "./config";
import reviver from "./middleware/reviver";
import addDataContext from "./middleware/add-data-context";
import * as eventApi from "./middleware/events-api";
import * as appinsights from "applicationinsights";

var app = express();

// Create express server
app.use(cors());
var bodyParserOptions = { reviver: reviver };
app.use(bodyParser.json(bodyParserOptions));

// Events API
app.get("/api/events", eventApi.getAll);
app.get("/api/events/:_id", eventApi.getById);
app.post("/api/events", eventApi.add);

// Start express server
var port: number = process.env.port || 1337;
addDataContext(config.MONGO_URL, app, () => {
    app.listen(port, () => {
        console.log(`Server is listening on port ${port}`);
    });
});
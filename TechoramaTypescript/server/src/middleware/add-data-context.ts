/// <reference path="../../typings/main.d.ts" />
import { Express } from "express";
import { MongoClient } from 'mongodb';
import { IDataContext } from '../dataAccess/contracts';
import EventStore from '../dataAccess/event-store';
import ParticipantStore from '../dataAccess/participant-store';

function addDataContext(mongoUrl: string, app: Express, cb: () => void) {
    MongoClient.connect(mongoUrl, (err, db) => {
        var dc : IDataContext = {
            events: new EventStore(db.collection("events")),
            participants: new ParticipantStore(db.collection("participants"))
        }; 
        (<any>app).dc = dc;
        cb();
    });
}

export default addDataContext;
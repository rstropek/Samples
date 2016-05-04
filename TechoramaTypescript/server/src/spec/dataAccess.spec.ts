/// <reference path="../../typings/main.d.ts" />
import { Db, MongoClient } from 'mongodb';
import * as config from './config';
import EventStore from '../dataAccess/event-store';
import ParticipantStore from '../dataAccess/participant-store';
import { IEvent, IParticipant } from '../model';

// NOTE THAT THIS FILE CONTAINS INTEGRATION TESTS
// The tests need access to a Mongo test DB. They will create/delete collections there.

async function dropCollectionIfExists(db: Db, collectionName: string) : Promise<any> {
    let collections = await db.listCollections({ name: collectionName }).toArray();
    if (collections.length > 0) {
        await db.dropCollection(collectionName);
    }
}

describe("Data access", () => {
    var originalTimeout: number;
    var db: Db;

    beforeEach(async (done) => {
        originalTimeout = jasmine.DEFAULT_TIMEOUT_INTERVAL;
        jasmine.DEFAULT_TIMEOUT_INTERVAL = 10000;

        db = await MongoClient.connect(config.MONGO_TEST_URL);
        done();
    });

    it("can maintain events", async (done) => {
        await dropCollectionIfExists(db, "events");

        let eventCollection = db.collection("events");
        let eventStore = new EventStore(eventCollection);

        // Create two events (one in the future, one in the past)
        let newEventFuture: IEvent = await eventStore.add({ date: new Date(Date.UTC(2030, 1, 31)), location: "Anywhere" });
        let newEventPast: IEvent = await eventStore.add({ date: new Date(Date.UTC(1990, 1, 31)), location: "Anywhere" });

        // Get event using ID
        expect(await eventStore.getById(newEventFuture._id.toHexString())).not.toBeNull();

        // Get all events (without/with past events)
        let events = await eventStore.getAll(false);
        expect(events.length).toBe(1);
        events = await eventStore.getAll(true);
        expect(events.length).toBe(2);

        done();
    });

    it("can maintain participants", async (done) => {
        await dropCollectionIfExists(db, "participants");

        let participantCollection = db.collection("participants");
        let participantStore = new ParticipantStore(participantCollection);

        // Create a participant
        let participant: IParticipant = await participantStore.add({ givenName: "John", familyName: "Doe" });

        // Get participant using ID
        var checkedInParticipant: any = await participantStore.getById(participant._id.toHexString());
        expect(checkedInParticipant).not.toBeNull();

        done();
    });

    afterEach(async (done) => {
        await db.close();
        jasmine.DEFAULT_TIMEOUT_INTERVAL = originalTimeout;
        done();
    });
});
/// <reference path="../../typings/main.d.ts" />
import { Collection } from 'mongodb';
import { IEvent } from '../model';
import { IEventStore } from './contracts';
import StoreBase from './store-base';

class EventStore extends StoreBase<IEvent> implements IEventStore {
    constructor(events: Collection) {
        super(events);
    }

    public async getAll(includePastEvents: boolean): Promise<IEvent[]> {
        if (includePastEvents) {
            return await this.collection.find({}).sort({ "date": 1 }).toArray();
        } else {
            let now = new Date();
            let result = await this.collection.find({
                date: { $gte: new Date(Date.UTC(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate())) }
            }).sort({ "date": 1 }).toArray();

            return result;
        }
    }

    public add(event: IEvent): Promise<IEvent> {
        event.date = new Date(Date.UTC(event.date.getUTCFullYear(), event.date.getUTCMonth(), event.date.getUTCDate()));
        return super.add(event);
    }
}

export default EventStore;
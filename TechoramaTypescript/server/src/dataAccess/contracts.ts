/// <reference path="../../typings/main.d.ts" />
import { IEvent, IParticipant } from '../model';

export interface IStoreBase<T> {
    add(item: T): Promise<T>;
    getById(_id: string): Promise<T>;
}

export interface IEventStore extends IStoreBase<IEvent> {
    getAll(includePastEvents: boolean): Promise<IEvent[]>;
}

export interface IParticipantStore extends IStoreBase<IParticipant> {
    getByName(givenName: string, familyName: string): Promise<IParticipant>;
}

export interface IDataContext {
    events?: IEventStore;
    participants?: IParticipantStore;
}

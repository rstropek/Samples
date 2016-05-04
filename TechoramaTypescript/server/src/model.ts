/// <reference path="../typings/main.d.ts" />
import { ObjectID } from 'mongodb';

export interface IMongoObject {
    _id?: ObjectID;
}

export interface IEvent extends IMongoObject {
    date: Date;
    location: string;
}

export interface IParticipant extends IMongoObject {
    givenName?: string;
    familyName?: string;
    email?: string;
}
/// <reference path="../../typings/main.d.ts" />
import { Collection } from 'mongodb';
import { IParticipant } from '../model';
import { IParticipantStore } from './contracts';
import StoreBase from './store-base';

class ParticipantStore extends StoreBase<IParticipant> implements IParticipantStore {
    constructor(participants: Collection) { 
        super(participants);
    }

    public async getByName(givenName: string, familyName: string): Promise<IParticipant> {
        let filter : any = { };
        if (givenName) {
            filter.givenName = givenName;
        }
        
        if (familyName) {
            filter.familyName = familyName;
        }
        
        let result = await this.collection.find(filter).limit(1).toArray();
        return (result.length !== 0) ? result[0] : null;
    }
}

export default ParticipantStore;
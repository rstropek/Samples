/// <reference path="../../typings/main.d.ts" />
import { Collection, ObjectID } from 'mongodb';
import { IMongoObject } from '../model';
import { IStoreBase } from './contracts';

class StoreBase<T extends IMongoObject> implements IStoreBase<T> {
    constructor(public collection: Collection) { }

    public async add(item: T): Promise<T> {
        await this.collection.insertOne(item);
        return item;
    }

    public async getById(_id: string): Promise<T> {
        let result = await this.collection.find({ _id: new ObjectID(_id) }).limit(1).toArray();
        return (result.length !== 0) ? result[0] : null;
    }
}

export default StoreBase;
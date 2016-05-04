/// <reference path="../../typings/main.d.ts" />
import { ObjectID } from 'mongodb';

const reDateDetect = /(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})/;
const reObjectIdDetect = /[0-9a-fA-F]{24}/;

/**
 * Implements a reviver function for JSON.parse. It detects date strings
 * and MongoDB object IDs and converts them into Date and ObjectID
 * instances.
 */
function reviver(key: any, value: any): any {
    if (typeof value === 'string') {
        if (reDateDetect.exec(value)) {
            return new Date(value);
        }

        if (key === '_id' && reObjectIdDetect.exec(value)) {
            return new ObjectID(value);
        }
    }

    return value;
}

export default reviver;
/// <reference path="../../typings/main.d.ts" />
import { ObjectID } from 'mongodb';
import reviver from '../middleware/reviver';

describe("Revivier", () => {
    it("converts date as expected", () => {
        expect(reviver("dummy", "2016-01T00:00:00Z")).not.toEqual(jasmine.any(Date));
        expect(reviver("dummy", "2016-02-01T00:00:00Z")).toEqual(jasmine.any(Date));
    });

    it("converts ObjectID as expected", () => {
        expect(reviver("_id", "asdf")).not.toEqual(jasmine.any(ObjectID));
        expect(reviver("_id", "56fb9efc316b5fb01e5629ff")).toEqual(jasmine.any(ObjectID));
    });
});
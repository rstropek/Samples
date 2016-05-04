/// <reference path="../../typings/main.d.ts" />
import { Request } from "express";
import { IDataContext } from '../dataAccess/contracts';

function getDataContext(req: Request): IDataContext {
    return <IDataContext>((<any>(req.app)).dc);
}

export default getDataContext;
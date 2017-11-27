import {Request, Response, Next} from 'restify';
import {customers} from './data';

export function getAll(req: Request, res: Response, next: Next): void {
    res.send(customers);
    next();
}
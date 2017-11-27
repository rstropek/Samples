import {CREATED} from 'http-status-codes';
import {Next, Request, Response} from 'restify';
import {BadRequestError} from 'restify-errors';
import {customers, ICustomer} from './data';

export function post(req: Request, res: Response, next: Next): void {
  if (!req.body.id || !req.body.firstName || !req.body.lastName) {
    next(new BadRequestError('Missing mandatory member(s)'));
  } else {
    const newCustomerId = parseInt(req.body.id);
    if (!newCustomerId) {
      next(new BadRequestError('ID has to be a numeric value'));
    } else {
      const newCustomer: ICustomer = { id: newCustomerId,
        firstName: req.body.firstName, lastName: req.body.lastName };
      customers.push(newCustomer);
      res.send(CREATED, newCustomer, {Location: `${req.path()}/${req.body.id}`});
    }
  }
}
import {Next, Request, Response} from 'restify';
import {BadRequestError, NotFoundError} from 'restify-errors';
import {customers} from './data';

export function getSingle(req: Request, res: Response, next: Next): void {
  const id = parseInt(req.params.id);
  if (id) {
    const customer = customers.find(c => c.id === id);
    if (customer) {
      res.send(customer);
      next();
    } else {
      next(new NotFoundError());
    }
  } else {
    next(new BadRequestError('Parameter id must be a number'));
  }
}
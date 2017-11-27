import {NO_CONTENT} from 'http-status-codes';
import {Next, Request, Response} from 'restify';
import {BadRequestError, NotFoundError} from 'restify-errors';
import {customers} from './data';

export function deleteSingle(req: Request, res: Response, next: Next): void {
  const id = parseInt(req.params.id);
  if (id) {
    const customerIndex = customers.findIndex(c => c.id === id);
    if (customerIndex !== (-1)) {
      customers.splice(customerIndex, 1);
      res.send(NO_CONTENT);
      next();
    } else {
      next(new NotFoundError());
    }
  } else {
    next(new BadRequestError('Parameter id must be a number'));
  }
}
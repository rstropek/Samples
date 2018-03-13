import * as Router from 'koa-router';

import { people, IPerson } from './data';

function getPeopleFromDatabase(): Promise<IPerson[]> {
  return new Promise<IPerson[]>((res, rej) => {
    res(people);
  })
}

export async function getAllPeople(context: Router.IRouterContext): Promise<any> {
  context.body = await getPeopleFromDatabase();
}

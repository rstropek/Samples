import * as rm from 'typed-rest-client/RestClient';

interface ICustomer {
  id: number;
  firstName: string;
  lastName: string;
}

async function workWithCustomers() {
  const rest = new rm.RestClient('demo', 'http://localhost:8080');
  const reply = await rest.get<ICustomer[]>('/api/customers');
  reply.result.forEach(c => console.log(c.lastName));

  const newCustomer:
      ICustomer = {'id': 1, 'firstName': 'Foo', 'lastName': 'Bar'};
  let createReply = await rest.create('/api/customers', newCustomer);
  if (createReply.statusCode !== 201) {
    console.log('An error occurred');
  }

  const newCustomer2:
      any = {'id': 'abc', 'firstName': 'Foo', 'lastName': 'Bar'};
  try {
    createReply = await rest.create('/api/customers', newCustomer2);
  } catch (err) {
    console.error(`Error ${(<rm.IRestResponse<{}>>err).statusCode} has occurred`);
  }
}

workWithCustomers();

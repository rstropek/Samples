export interface ICustomer {
  id: number;
  firstName: string;
  lastName: string;
}

export const customers: ICustomer[] = [
  {id: 1, firstName: 'Donald', lastName: 'Duck'},
  {id: 2, firstName: 'Mickey', lastName: 'Mouse'},
  {id: 3, firstName: 'Minnie', lastName: 'Mouse'},
  {id: 4, firstName: 'Scrooge', lastName: 'McDuck'}
];
export interface IPerson { name: string; }

export interface ITodoItem {
  id: number;
  assignedTo?: string;
  description: string;
  done?: boolean
}

export const people: IPerson[] = [{name: 'Adam'}, {name: 'Eve'}];
export const todos: ITodoItem[] = [];

let lastId = 0;

export function getNextId(): number {
  return lastId++;
}

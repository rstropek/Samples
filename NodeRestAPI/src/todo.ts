import { BAD_REQUEST, CREATED, NO_CONTENT, NOT_FOUND } from 'http-status-codes';
import * as Router from 'koa-router';

import { getNextId, ITodoItem, people, todos } from './data';

export function getAllTodo(context: Router.IRouterContext) {
  // Return todo items
  context.body = todos;
}

export function getTodo(context: Router.IRouterContext) {
  // Check if todo item exists
  const todoItem = todos.find(i => i.id == context.params.id);
  if (!todoItem) {
    context.status = NOT_FOUND;
  } else {
    context.body = todoItem;
  }
}

export function addTodo(context: Router.IRouterContext) {
  const body = context.request.body;

  if (!body.description) {
    // description field is mandatory
    context.status = BAD_REQUEST;
    context.body = {description: 'Missing description'};
    return;
  }

  const newItem: ITodoItem = {id: getNextId(), description: body.description};

  // Check if assigned-to person exists
  if (body.assignedTo) {
    if (people.find(p => p.name === body.assignedTo)) {
      newItem.assignedTo = body.assignedTo;
    } else {
      context.status = NOT_FOUND;
      context.body = {description: 'Unknown person'};
      return;
    }
  }

  todos.push(newItem);

  context.set('location', `/api/todos/${newItem.id}`);
  context.status = CREATED;
  context.body = newItem;
}

export function patchTodo(context: Router.IRouterContext) {
  // Check if todo item exists
  const todoItem = todos.find(i => i.id == context.params.id);
  if (!todoItem) {
    context.status = NOT_FOUND;
    return;
  }

  const body = context.request.body;

  // Update description if specified
  if (body.description) {
    todoItem.description = body.description;
  }

  // Update done if specified
  if (body.done === true || body.done === false) {
    todoItem.done = body.done;
  }

  // Update assigned-to if specified
  if (body.assignedTo) {
    // Check if assigned-to person exists
    if (people.find(p => p.name === body.assignedTo)) {
      todoItem.assignedTo = body.assignedTo;
    } else {
      context.status = NOT_FOUND;
      context.body = {description: 'Unknown person'};
      return;
    }
  }

  context.body = todoItem;
}

export function deleteTodo(context: Router.IRouterContext) {
  // Check if todo item exists
  const todoItemIndex = todos.findIndex(i => i.id == context.params.id);
  if (todoItemIndex === (-1)) {
    context.status = NOT_FOUND;
    return;
  }

  todos.splice(todoItemIndex, 1);

  context.status = NO_CONTENT;
}

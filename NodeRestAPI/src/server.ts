import * as cors from '@koa/cors';
import * as Koa from 'koa';
import * as bodyParser from 'koa-bodyparser';
import * as logger from 'koa-logger';
import * as Router from 'koa-router';
import * as views from 'koa-views';
import { FileSystemLoader } from 'nunjucks';
import * as path from 'path';

import { getAllPeople } from './people';
import { addTodo, deleteTodo, getAllTodo, getTodo, patchTodo } from './todo';

// Tip:
// For more complex applications, consider using a dependency injection
// framework like InversifyJS. There are plugins for express and koa (see also
// e.g. https://www.npmjs.com/package/inversify-koa-utils)

// Read more about routing at https://github.com/alexmingoia/koa-router
const router = new Router({prefix: '/api'});
router.get('/people', getAllPeople);
router.get('/todos', getAllTodo);
router.get('/todos/:id', getTodo);
router.post('/todos', addTodo);
router.patch('/todos/:id', patchTodo);
router.delete('/todos/:id', deleteTodo);

// Read more about koa at http://koajs.com/
const app = new Koa();
app.use(cors());
app.use(logger());
app.use(bodyParser());
app.use(router.routes());

// Read more about koa views at https://github.com/queckezz/koa-views
// Read more about Nunjucks at https://mozilla.github.io/nunjucks/
const viewPath = path.join(__dirname, 'views');
app.use(views(viewPath, {
  map: {html: 'nunjucks'},
  options: {loader: new FileSystemLoader(viewPath)}
}));
app.use(async (ctx, next) => {
  // If nothing else was found, render index (assumption: single-page app)
  await ctx.render('index');
});

const port: (number|string) = process.env.PORT || 8080;
app.listen(port, () => {
  console.log(`Server is listening on port ${port}...`);
});

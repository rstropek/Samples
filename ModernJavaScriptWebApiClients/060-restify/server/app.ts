import {createServer, plugins} from 'restify';
import corsMiddleware = require('restify-cors-middleware');

import {deleteSingle} from './delete-single';
import {getAll} from './get-all';
import {getSingle} from './get-single';
import {post} from './post';

var server = createServer();

// Add bodyParser plugin for parsing JSON in request body
server.use(plugins.bodyParser());

// Add CORS
const options: corsMiddleware.Options = {
    preflightMaxAge: 5,
    origins: ['*'],
    allowHeaders: ['API-Token'],
    exposeHeaders: ['API-Token-Expiry']
  };
const cors: corsMiddleware.CorsMiddleware = corsMiddleware(options);
server.pre(cors.preflight);
server.use(cors.actual);

// Add routes
server.get('/api/customers', getAll);
server.post('/api/customers', post);
server.get('/api/customers/:id', getSingle);
server.del('/api/customers/:id', deleteSingle);

server.listen(8080, () => console.log('API is listening'));
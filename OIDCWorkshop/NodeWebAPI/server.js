const express = require('express');
const cors = require('cors');
const app = express();
app.use(cors());
const jwt = require('express-jwt');
const jwksRsa = require('jwks-rsa');
const jwtAuthz = require('express-jwt-authz');

const checkJwt = jwt({
  secret: jwksRsa.expressJwtSecret({
    cache: true,
    rateLimit: true,
    jwksRequestsPerMinute: 5,
    jwksUri: `https://rainerdemo.eu.auth0.com/.well-known/jwks.json`
  }),

  audience: 'http://oidc-webapi/',
  issuer: `https://rainerdemo.eu.auth0.com/`,
  algorithms: ['RS256']
});

const readScope = jwtAuthz(['read:data']);
const writeScope = jwtAuthz(['write:data']);

app.get('/api/values', checkJwt, readScope, function(req, res) {
  res.json(['value1', 'value2']);
});

app.delete('/api/values', checkJwt, writeScope, function(req, res) {
  res.status(200).send();
});

app.listen(5001, function() {
  console.log('Listening...');
});
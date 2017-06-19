const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const jwt = require('express-jwt');
const jwksRsa = require('jwks-rsa');
const appInsights = require('applicationinsights');

// Setup application insights
appInsights.setup('b3d22c7a-573a-488e-b6a7-d901453f42f9');
appInsights.start();

// Setup express
const app = express();
app.use(cors());
app.use(bodyParser.json());

// Create middleware for checking JWT
const checkJwt = jwt({
  secret: jwksRsa.expressJwtSecret({
    cache: true,
    rateLimit: true,
    jwksRequestsPerMinute: 5,
    jwksUri: `http://localhost:5000/.well-known/openid-configuration/jwks`
  }),

  audience: 'api1',
  issuer: `http://localhost:5000`,
  algorithms: ['RS256']
});

// Implement demo Web API
app.post('/api/save', checkJwt, function(req, res) {
  if (req.user.scope.indexOf('api1') !== -1) {
    console.log(`Player ${req.user.sub} won with ${req.body.winner}!`);
    res.status(201).send();
  } else {
    res.status(401).send('Invalid scope');
  }
});

app.listen(5001, function() {
  console.log('Listening...');
});
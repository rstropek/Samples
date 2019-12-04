import * as WebSocket from 'ws';

// Create WebSockets server listening on port 3000
const wss = new WebSocket.Server({port: 3000});

wss.on('connection', ws => {
  // Called whenever a new client connects

  // Add handler for incoming messages
  ws.on('message', message => console.log('received: %s', message));

  // Send text message to new client
  ws.send('Welcome!');
});

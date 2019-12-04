import * as WebSocket from 'ws';
import { setInterval } from 'timers';

// Create WebSockets server listening on port 3000
const wss = new WebSocket.Server({port: 3000});

function broadcast(data: string) {
  // Iterate over all clients
  wss.clients.forEach(client => {
    // Send if connection is open
    if (client.readyState === WebSocket.OPEN) client.send(data);
  });
}

let i = 0;
setInterval(() => broadcast((i++).toString()), 1000);

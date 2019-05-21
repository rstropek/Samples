import * as WebSocket from 'ws';
import { setInterval } from 'timers';

interface Message {
  messageType: number;
}

interface SiloFillMessage extends Message {
  deviceId: number;
  value: number;
}

// Create WebSockets server listening on port 3000
const wss = new WebSocket.Server({port: 3000});

function broadcast(data: string) {
  // Iterate over all clients
  wss.clients.forEach(client => {
    // Send if connection is open
    if (client.readyState === WebSocket.OPEN) client.send(data);
  });
}

let i = 10;
let direction = 1;
setInterval(() => {
  if (i < 25) {
    i = 25;
    direction = 1;
  } else if (i > 85) {
    i = 85;
    direction = -1;
  } else {
    i += 5 * direction;
  }

  const message: SiloFillMessage = {
    messageType: 1,
    deviceId: 42,
    value: i
  };
  broadcast(JSON.stringify(message));
}, 250);

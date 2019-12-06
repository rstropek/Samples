package main

// Hub maintains the set of active clients and broadcasts messages to all clients.
type Hub struct {
	// Registered clients.
	//
	// Only the keys of the following map are interesting.
	// The bool value type is not relevant for the algorithm.
	clients map[*Client]bool

	// Inbound messages from the clients.
	broadcast chan []byte

	// Register requests from the clients.
	register chan *Client

	// Unregister requests from clients.
	unregister chan *Client
}

// Create a new Hub
func newHub() *Hub {
	return &Hub{
		broadcast:  make(chan []byte),
		register:   make(chan *Client),
		unregister: make(chan *Client),
		clients:    make(map[*Client]bool),
	}
}

func (h *Hub) removeClient(client *Client) {
	delete(h.clients, client)
	close(client.send)
}

// Run hub
//
// A single goroutine running the hub is started when starting the app.
func (h *Hub) run() {
	for {
		select {
		case client := <-h.register:
			// Store the new client in the clients map
			h.clients[client] = true
		case client := <-h.unregister:
			// Check if the client has been registered
			if _, ok := h.clients[client]; ok {
				h.removeClient(client)
			}
		case message := <-h.broadcast:
			// Iterate over all clients (iterates over keys of map)
			for client := range h.clients {
				// Execute an unblocking send
				select {
				case client.send <- message:
				default:
					// Send not possible -> remove client
					h.removeClient(client)
				}
			}
		}
	}
}

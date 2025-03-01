# FilePublisher

FilePublisher is a tool designed for workshop presenters to share their code files with attendees in real-time. It allows workshop participants to view and follow along with the presenter's code without needing to clone repositories or set up development environments.

## Overview

The system consists of two main components:

1. **PublisherApi**: A Node.js/Express backend server that provides access to files in specified directories
2. **Frontend**: A web client that allows attendees to view files using a session ID

## How It Works

### For Presenters

1. The presenter sets up the FilePublisher server on their machine
2. They configure session IDs in a `sessions.json` file, mapping each ID to a specific directory path
3. They share the relevant session ID with workshop attendees

### For Attendees

1. Attendees open the FilePublisher web interface in their browser
2. They enter the session ID provided by the presenter
3. They can browse and view all files in the presenter's shared directory
4. Files are automatically refreshed every 30 seconds to show updates

## Features

- **Session Management**: Access to files is controlled via session IDs
- **File Navigation**: Browse all files in the shared directory
- **Syntax Highlighting**: Code is displayed with proper syntax highlighting
- **Auto-Refresh**: Files are automatically refreshed to show the latest changes
- **Caching**: ETag-based caching to reduce bandwidth usage
- **Security**: Path normalization to prevent directory traversal attacks
- **Persistence**: Session IDs are saved in localStorage for convenience

## Technical Details

### Backend (PublisherApi)

- Built with Express.js and TypeScript
- Uses a JSON file (`sessions.json`) to map session IDs to directory paths
- Implements caching for session data with a 5-minute TTL
- Provides API endpoints for:
  - Listing all files in a session directory
  - Retrieving the content of a specific file
- Includes security features like path normalization
- Uses ETags for efficient caching

### Frontend

- Built with TypeScript and vanilla JavaScript
- Simple, intuitive UI for browsing and viewing files
- Implements automatic refresh every 30 seconds
- Uses highlight.js for syntax highlighting
- Stores the last used session ID in localStorage

## Setup

1. Clone the repository
2. Install dependencies for both the backend and frontend
3. Create a `sessions.json` file in the root of the PublisherApi directory with the following format:
   ```json
   {
     "session1": "/path/to/directory1",
     "session2": "/path/to/directory2"
   }
   ```
5. Build the frontend (`npm run build`, copies _dist_ into backend server)
4. Start the backend server

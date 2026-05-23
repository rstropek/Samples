# Stage 1: Build
FROM node:22-alpine AS builder

WORKDIR /app

COPY package*.json ./

# Use npm ci for clean, reproducible installs
RUN npm ci

COPY . .

# Build the application
RUN npm run build

# Stage 2: Production
FROM node:22-alpine

WORKDIR /app

COPY package*.json ./

# Install only production dependencies
RUN npm ci --only=production

# Copy built artifacts from the builder stage
COPY --from=builder /app/dist ./dist

EXPOSE 3000

CMD ["node", "dist/index.js"]

FROM node:lts-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:lts-alpine AS production
RUN addgroup -g 1001 -S nodejs
RUN adduser -S webapp -u 1001
WORKDIR /app
COPY package*.json ./
RUN npm ci --omit=dev && npm cache clean --force
COPY --from=builder /app/dist ./dist
RUN chown -R webapp:nodejs /app
USER webapp
ENV NODE_ENV=production
EXPOSE 3000
CMD ["node", "dist/app.js"]

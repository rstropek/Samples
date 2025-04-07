FROM node:23-alpine

WORKDIR /usr/src/app

RUN addgroup --system --gid 1001 nodejs \
    && adduser --system --uid 1001 --ingroup nodejs nodeuser \
    && mkdir -p /usr/src/app/data \
    && chown -R nodeuser:nodejs /usr/src/app

COPY package*.json ./
RUN npm install
COPY . .
RUN chown -R nodeuser:nodejs /usr/src/app
USER nodeuser

# Set environment variables
ENV NODE_ENV=production
ENV PORT=8081
EXPOSE 8081

# Start the application
CMD ["npx", "tsx", "--env-file", ".env", "./src/app.ts"] 

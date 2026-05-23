FROM node:alpine AS build
WORKDIR /app
COPY . .
RUN npm install \
    && npm run build

FROM node:alpine
WORKDIR /app
COPY --from=build /app/dist/ .
COPY package.json .
RUN npm install --omit=dev
CMD ["node", "/app/app.js"]

FROM node:22-bookworm-slim AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:22-bookworm-slim AS final
WORKDIR /app
ENV NODE_ENV=production
ENV PORT=4000
COPY --from=build /app/dist/frontend ./dist/frontend
EXPOSE 4000
CMD ["node", "dist/frontend/server/server.mjs"]

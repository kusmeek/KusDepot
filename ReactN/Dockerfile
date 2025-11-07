FROM node:latest AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build

FROM node:alpine AS runtime
WORKDIR /app
COPY --from=build /app/package.json ./
COPY --from=build /app/package-lock.json ./
RUN npm install --omit=dev --prefer-offline --no-audit --progress=false
COPY --from=build /app/proto/shapeapi.proto ./proto/shapeapi.proto
COPY --from=build /app/target ./target
EXPOSE 8087 8088
CMD ["node", "target/StartUp.js"]
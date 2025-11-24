FROM rust:1-alpine AS build
WORKDIR /app
COPY . .
RUN apk add --no-cache musl-dev build-base protobuf-dev protobuf
RUN cargo build --release --bin KusDepot-ReactR

FROM alpine:latest
WORKDIR /app
COPY --from=build /app/target/release/KusDepot-ReactR ./KusDepot-ReactR
EXPOSE 8091 8092
ENTRYPOINT ["./KusDepot-ReactR"]
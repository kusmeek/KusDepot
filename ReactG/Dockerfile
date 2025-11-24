FROM golang:1-alpine AS build
WORKDIR /app
COPY . .
RUN go build -o kusdepot.reactg ./StartUp.go

FROM alpine:latest
WORKDIR /app
COPY --from=build /app/kusdepot.reactg .
EXPOSE 8083 8084
ENTRYPOINT ["./kusdepot.reactg"]
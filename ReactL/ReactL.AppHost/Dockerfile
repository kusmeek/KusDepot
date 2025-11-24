FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-noble AS fsbuild
WORKDIR /src
COPY ["ReactF/ReactF.fsproj", "ReactF/"]
COPY ["ReactG/ReactG.csproj", "ReactG/"]
RUN dotnet restore "./ReactF/ReactF.fsproj"
COPY ["ReactF/", "ReactF/"]
COPY ["ReactG/", "ReactG/"]
WORKDIR "/src/ReactF"
RUN dotnet publish "./ReactF.fsproj" -c Release -o /app/publish /p:UseAppHost=false /p:ContainerBuild=true

FROM golang:1-alpine AS gobuild
WORKDIR /src/ReactG
COPY ReactG .
RUN go build -o /out/kusdepot.reactg ./StartUp.go

FROM maven:3-eclipse-temurin-24 AS javabuild
WORKDIR /src/ReactJ
COPY ReactJ .
RUN mvn dependency:go-offline -B
RUN mvn clean package -DskipTests -B

FROM node:latest AS nodebuild
WORKDIR /src/ReactN
COPY ReactN/package.json ReactN/package-lock.json ./
RUN npm install
COPY ReactN/. ./
RUN npm run build

FROM rust:1-alpine AS rustbuild
WORKDIR /src/ReactR
COPY ReactR .
RUN apk add --no-cache musl-dev build-base protobuf-dev protobuf
RUN cargo build --release --bin KusDepot-ReactR

FROM ubuntu:latest
WORKDIR /app
ARG JAR_VERSION=0.0.1

RUN apt-get update && apt-get install -y wget && apt-get clean && rm -rf /var/lib/apt/lists/*

RUN wget -qO- https://deb.nodesource.com/setup_24.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

RUN wget https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/10.0.0-preview.7.25380.108/aspnetcore-runtime-10.0.0-preview.7.25380.108-linux-x64.tar.gz -O dotnet.tar.gz \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
    && rm dotnet.tar.gz

RUN wget https://github.com/adoptium/temurin24-binaries/releases/download/jdk-24.0.2%2B12/OpenJDK24U-jre_x64_linux_hotspot_24.0.2_12.tar.gz -O temurin24-jre.tar.gz \
    && mkdir -p /usr/lib/jvm/temurin-24-jre \
    && tar -xzf temurin24-jre.tar.gz -C /usr/lib/jvm/temurin-24-jre --strip-components=1 \
    && rm temurin24-jre.tar.gz
ENV JAVA_HOME=/usr/lib/jvm/temurin-24-jre
ENV PATH="$JAVA_HOME/bin:$PATH"

RUN apt-get update && \
    apt-get install -y python3-pip python3-venv gnupg ca-certificates libicu74 && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

COPY --from=fsbuild /app/publish ./reactf
COPY --from=gobuild /out/kusdepot.reactg ./
COPY --from=javabuild /src/ReactJ/target/ShapeAPI-${JAR_VERSION}-jar-with-dependencies.jar ./ShapeAPI.jar
COPY --from=nodebuild /src/ReactN/proto/shapeapi.proto ./reactn/proto/shapeapi.proto
COPY --from=nodebuild /src/ReactN/node_modules ./reactn/node_modules
COPY --from=nodebuild /src/ReactN/target ./reactn/target
COPY ReactP/reactp ./reactp
COPY ReactP/requirements.txt ./requirements.txt
RUN python3 -m venv /app/venv \
    && /app/venv/bin/pip install --no-cache-dir -r requirements.txt
COPY --from=rustbuild /src/ReactR/target/release/KusDepot-ReactR ./
COPY ReactL/ReactL.AppHost/entry.sh ./entry.sh
RUN chmod +x entry.sh
EXPOSE 8081 8082 8083 8084 8085 8086 8087 8088 8089 8090 8091 8092
ENTRYPOINT ["./entry.sh"]
FROM maven:3-eclipse-temurin-24 AS build
WORKDIR /app
COPY . .
RUN mvn dependency:go-offline -B
RUN mvn clean package -DskipTests -B

FROM eclipse-temurin:24-jre-alpine
WORKDIR /app
ARG JAR_VERSION=0.0.1
COPY --from=build /app/target/ShapeAPI-${JAR_VERSION}-jar-with-dependencies.jar ShapeAPI.jar
EXPOSE 8085 8086
ENTRYPOINT ["java", "--enable-native-access=ALL-UNNAMED", "-jar", "ShapeAPI.jar"]
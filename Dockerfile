FROM eclipse-temurin:17-jdk-focal AS GRADLE_CACHE
WORKDIR /cache
COPY ./gradle/ ./gradle
COPY ./build.gradle.kts ./
COPY ./gradlew ./
COPY ./gradlew.bat ./
COPY ./settings.gradle.kts ./
RUN ./gradlew clean

FROM eclipse-temurin:17-jdk-focal AS BUILD
WORKDIR /build
COPY --from=GRADLE_CACHE /cache/.gradle ./
COPY --from=GRADLE_CACHE /root/.gradle /root/.gradle
COPY . .
RUN ./gradlew bootJar

FROM eclipse-temurin:17-jre-focal AS RUNNER
WORKDIR /app
COPY --from=BUILD /build/build/libs/platform*.jar ./server_application.jar
ENTRYPOINT ["java", "-jar", "server_application.jar"]
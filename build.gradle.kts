import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    id("org.springframework.boot") version "3.1.2"
    id("io.spring.dependency-management") version "1.1.2"
    id("org.springdoc.openapi-gradle-plugin") version "1.6.0"
    kotlin("jvm") version "1.8.22"
    kotlin("plugin.spring") version "1.8.22"
    kotlin("plugin.jpa") version "1.8.22"
    id("org.liquibase.gradle") version "2.2.0"
}

group = "com.infrastructure.education"
version = "0.0.1-SNAPSHOT"

java {
    sourceCompatibility = JavaVersion.VERSION_17
}

configurations {
    compileOnly {
        extendsFrom(configurations.annotationProcessor.get())
    }

    liquibase {
        activities.register("main") {
            val dbHostWithPort = project.ext.properties.get("dbHost")
            val dbName = project.ext.properties.get("dbName")
            val dbUsername = project.ext.properties.get("dbUsername")
            val dbPassword = project.ext.properties.get("dbPassword")
            val dbJdbcUrl = "jdbc:postgresql://${dbHostWithPort}/${dbName}"

            this.arguments = mapOf(
                    "changelogFile" to "src/main/resources/db/db.changelog-master.yml",
                    "url" to dbJdbcUrl,
                    "username" to dbUsername,
                    "password" to dbPassword,
                    "driver" to "org.postgresql.Driver",
                    "classpath" to "src/main/resources",
                    "referenceUrl" to "hibernate:spring:com.infrastructure.education?dialect=org.hibernate.dialect.PostgreSQL10Dialect&hibernate.physical_naming_strategy=org.hibernate.boot.model.naming.CamelCaseToUnderscoresNamingStrategy&hibernate.implicit_naming_strategy=org.springframework.boot.orm.jpa.hibernate.SpringImplicitNamingStrategy",
                    "referenceDriver" to "liquibase.ext.hibernate.database.connection.HibernateDriver"
            )
        }
        runList = "main"
    }

    liquibaseRuntime {
        extendsFrom(configurations.compileClasspath.get())
    }
}

repositories {
    mavenCentral()
}

dependencies {
    api("org.springdoc:springdoc-openapi-starter-webmvc-ui:2.2.0")
    implementation("org.springframework.boot:spring-boot-starter-data-jdbc")
    implementation("org.springframework.boot:spring-boot-starter-data-jpa")
    implementation("org.springframework.boot:spring-boot-starter-data-redis")
    implementation("org.springframework.boot:spring-boot-starter-web")
    implementation("org.springframework.boot:spring-boot-starter-webflux")
    implementation("org.springframework.boot:spring-boot-starter-security")
    implementation("io.jsonwebtoken:jjwt-api:0.11.2")
    implementation("io.jsonwebtoken:jjwt-impl:0.11.2")
    implementation("io.jsonwebtoken:jjwt-jackson:0.11.2")
    implementation("com.fasterxml.jackson.module:jackson-module-kotlin")
    implementation("org.jetbrains.kotlin:kotlin-reflect")
    implementation("org.postgresql:postgresql")
    implementation("com.github.f4b6a3:ulid-creator:5.2.1")
    developmentOnly("org.springframework.boot:spring-boot-devtools")
    runtimeOnly("org.postgresql:postgresql")
    annotationProcessor("org.springframework.boot:spring-boot-configuration-processor")
    testImplementation("org.springframework.boot:spring-boot-starter-test")
    testImplementation("org.springframework.boot:spring-boot-testcontainers")
    testImplementation("org.testcontainers:junit-jupiter")
    testImplementation("org.testcontainers:postgresql")

    val kotestVersion = "5.6.2"
    testImplementation("io.kotest:kotest-runner-junit5:$kotestVersion")
    testImplementation("io.kotest:kotest-assertions-core:$kotestVersion")
    testImplementation("io.kotest.extensions:kotest-extensions-spring:1.1.3")
    testImplementation("io.kotest.extensions:kotest-extensions-testcontainers:2.0.2")

    val mockkVersion = "1.13.5"
    testImplementation("io.mockk:mockk:${mockkVersion}")

    // Liquibase
    liquibaseRuntime("org.liquibase:liquibase-core:4.23.1")
    liquibaseRuntime("org.liquibase:liquibase-groovy-dsl:3.0.3")
    liquibaseRuntime("org.postgresql:postgresql")
    liquibaseRuntime("org.liquibase.ext:liquibase-hibernate6:4.23.0")
    liquibaseRuntime("org.yaml:snakeyaml:2.0")
    liquibaseRuntime("info.picocli:picocli:4.6.1")
    liquibaseRuntime(sourceSets.getByName("main").output)
}

tasks.withType<KotlinCompile> {
    kotlinOptions {
        freeCompilerArgs += "-Xjsr305=strict"
        jvmTarget = "17"
    }
}

tasks.withType<Test> {
    useJUnitPlatform()
}

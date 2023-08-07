package com.infrastructure.education

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.runApplication

@SpringBootApplication
class PlatformCoreApplication

fun main(args: Array<String>) {
	runApplication<PlatformCoreApplication>(*args)
}

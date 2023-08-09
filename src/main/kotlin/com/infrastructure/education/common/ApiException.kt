package com.infrastructure.education.common

import org.springframework.http.HttpStatus
import org.springframework.http.HttpStatusCode

class ApiException(
    val statusCode: HttpStatus,
    val errorMessage: String,
    val errorTitle: ErrorTitle
): RuntimeException()
package com.infrastructure.education.common

import org.springframework.http.HttpStatus

class ApiException(
        val statusCode: HttpStatus,
        val errorMessage: String,
        val errorTitle: ErrorTitle
) : RuntimeException()
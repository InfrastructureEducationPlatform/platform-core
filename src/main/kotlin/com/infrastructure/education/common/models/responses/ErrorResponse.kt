package com.infrastructure.education.common.models.responses

import com.infrastructure.education.common.ErrorTitle
import org.springframework.http.HttpStatusCode

class ErrorResponse(
    val statusCode: HttpStatusCode,
    val errorMessage: String,
    val errorTitle: ErrorTitle
)
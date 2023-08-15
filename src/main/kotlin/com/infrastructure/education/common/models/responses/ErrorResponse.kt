package com.infrastructure.education.common.models.responses

import com.infrastructure.education.common.ErrorTitle

class ErrorResponse(
        val statusCode: Int,
        val errorMessage: String,
        val errorTitle: ErrorTitle
)
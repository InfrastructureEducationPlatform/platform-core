package com.infrastructure.education.common

import com.infrastructure.education.common.models.responses.ErrorResponse
import org.springframework.http.HttpStatus
import org.springframework.http.ResponseEntity
import org.springframework.web.bind.annotation.ExceptionHandler
import org.springframework.web.bind.annotation.RestControllerAdvice

@RestControllerAdvice
class GlobalExceptionHandler {
    @ExceptionHandler(ApiException::class)
    fun handleApiException(apiException: ApiException): ResponseEntity<ErrorResponse> {
        val errorResponse = ErrorResponse(apiException.statusCode.value(), apiException.errorMessage, apiException.errorTitle)
        return ResponseEntity<ErrorResponse>(errorResponse, apiException.statusCode)
    }

    @ExceptionHandler(Exception::class)
    fun handleGlobalException(exception: Exception): ResponseEntity<ErrorResponse> {
        return ResponseEntity<ErrorResponse>(ErrorResponse(HttpStatus.INTERNAL_SERVER_ERROR.value(), "Unknown Error occurred", ErrorTitle.UNKNOWN_INTERNAL_ERROR), HttpStatus.INTERNAL_SERVER_ERROR)
    }
}
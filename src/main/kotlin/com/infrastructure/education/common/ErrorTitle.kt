package com.infrastructure.education.common

enum class ErrorTitle(
        val title: String
) {
    ACCOUNT_EMAIL_CONFLICT("User email already exists."),
    ACCOUNT_NOT_FOUND("User is not found!"),
    UNAUTHORIZED("Request is not authorized!"),
    ACCOUNT_CREDENTIAL_NOT_CORRECT("Credential client provided is not correct!"),
    UNKNOWN_INTERNAL_ERROR("Internal Server error occurred"),
}
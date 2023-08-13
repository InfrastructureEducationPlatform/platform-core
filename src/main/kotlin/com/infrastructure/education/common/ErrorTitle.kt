package com.infrastructure.education.common

enum class ErrorTitle(
        val title: String
) {
    ACCOUNT_EMAIL_CONFLICT("User email already exists."),
    UNKNOWN_INTERNAL_ERROR("Internal Server error occurred")
}
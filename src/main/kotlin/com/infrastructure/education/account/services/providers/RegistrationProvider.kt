package com.infrastructure.education.account.services.providers

import com.infrastructure.education.account.dto.requests.RegisterRequestDto
import com.infrastructure.education.account.models.Account
import com.infrastructure.education.account.models.Credential

interface RegistrationProvider {
    fun createCredential(registerRequestDto: RegisterRequestDto, account: Account): Credential
}
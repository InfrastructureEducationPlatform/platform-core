package com.infrastructure.education.account.services.providers

import com.infrastructure.education.account.dto.requests.RegisterRequestDto
import com.infrastructure.education.account.models.Account
import com.infrastructure.education.account.models.Credential
import com.infrastructure.education.account.models.CredentialId
import org.springframework.security.crypto.password.PasswordEncoder
import org.springframework.stereotype.Service

@Service
class SelfRegistrationProvider(
        private val passwordEncoder: PasswordEncoder,
) : RegistrationProvider {
    override fun createCredential(registerRequestDto: RegisterRequestDto, account: Account): Credential {
        return Credential(
                id = CredentialId(credentialId = registerRequestDto.credentialId, provider = registerRequestDto.credentialProvider),
                credentialKey = passwordEncoder.encode(registerRequestDto.credentialKey),
                account = account
        )
    }
}
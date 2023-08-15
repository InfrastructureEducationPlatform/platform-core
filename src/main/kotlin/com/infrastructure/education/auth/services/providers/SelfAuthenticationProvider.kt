package com.infrastructure.education.auth.services.providers

import com.infrastructure.education.account.models.CredentialId
import com.infrastructure.education.account.models.CredentialProvider
import com.infrastructure.education.account.repositories.CredentialRepository
import com.infrastructure.education.auth.models.CustomUserDetails
import com.infrastructure.education.auth.models.SelfAuthenticationToken
import com.infrastructure.education.common.ApiException
import com.infrastructure.education.common.ErrorTitle
import org.springframework.data.repository.findByIdOrNull
import org.springframework.http.HttpStatus
import org.springframework.security.authentication.AuthenticationProvider
import org.springframework.security.core.Authentication
import org.springframework.security.crypto.password.PasswordEncoder
import org.springframework.stereotype.Service
import org.springframework.transaction.annotation.Transactional

@Service
class SelfAuthenticationProvider(
        private val passwordEncoder: PasswordEncoder,
        private val credentialRepository: CredentialRepository
) : AuthenticationProvider {

    @Transactional
    override fun authenticate(authentication: Authentication): Authentication {
        // Get UserEmail, Password from Authentication
        val userEmail = authentication.principal as String
        val userPassword = authentication.credentials as String

        // Get Credential from Repository
        val userCredential = credentialRepository.findByIdOrNull(CredentialId(userEmail, CredentialProvider.Self))
                ?: throw ApiException(HttpStatus.UNAUTHORIZED, "Cannot find user with credential.", ErrorTitle.ACCOUNT_NOT_FOUND)

        // Check password matches
        val userPasswordMatched = passwordEncoder.matches(userPassword, userCredential.credentialKey)
        if (!userPasswordMatched)
            throw ApiException(HttpStatus.UNAUTHORIZED, "Credential client provided is incorrect!", ErrorTitle.ACCOUNT_CREDENTIAL_NOT_CORRECT)

        // Create UserDetail
        val customUserDetails = CustomUserDetails(userCredential.account)

        // Return Authenticated Authentication Object
        return SelfAuthenticationToken(customUserDetails)
    }

    override fun supports(authentication: Class<*>): Boolean {
        return SelfAuthenticationToken::class.java.isAssignableFrom(authentication)
    }
}
package com.infrastructure.education.account.services

import com.infrastructure.education.account.dto.requests.LoginRequestDto
import com.infrastructure.education.account.dto.requests.RegisterRequestDto
import com.infrastructure.education.account.dto.responses.TokenResponse
import com.infrastructure.education.account.models.CredentialId
import com.infrastructure.education.account.models.CredentialProvider
import com.infrastructure.education.account.repositories.AccountRepository
import com.infrastructure.education.account.repositories.CredentialRepository
import com.infrastructure.education.auth.models.CustomUserDetails
import com.infrastructure.education.auth.models.SelfAuthenticationToken
import com.infrastructure.education.auth.services.JwtService
import com.infrastructure.education.common.ApiException
import com.infrastructure.education.common.ErrorTitle
import org.springframework.data.repository.findByIdOrNull
import org.springframework.http.HttpStatus
import org.springframework.security.authentication.AuthenticationManager
import org.springframework.security.core.Authentication
import org.springframework.stereotype.Service
import org.springframework.transaction.annotation.Transactional

@Service
class AccountService(
        private val accountRepository: AccountRepository,
        private val credentialRepository: CredentialRepository,
        private val jwtService: JwtService,
        private val authenticationManager: AuthenticationManager
) {
    @Transactional
    fun createAccount(registerRequestDto: RegisterRequestDto): TokenResponse {
        val previousExistingCredential = credentialRepository.findByIdOrNull(CredentialId(registerRequestDto.credentialId, registerRequestDto.credentialProvider))
        val previousExistingAccount = accountRepository.findByEmail(registerRequestDto.email)

        if (previousExistingAccount != null || previousExistingCredential != null) {
            throw ApiException(HttpStatus.CONFLICT, "${registerRequestDto.email} or user with ${registerRequestDto.credentialProvider} already exists!", ErrorTitle.ACCOUNT_EMAIL_CONFLICT)
        }

        val account = registerRequestDto.toAccount()
        accountRepository.save(account)

        val credential = registerRequestDto.toCredential(account)
        credentialRepository.save(credential)

        return TokenResponse(
                token = jwtService.generateJwt(CustomUserDetails(account))
        )
    }

    @Transactional
    fun loginAccount(loginRequestDto: LoginRequestDto): TokenResponse {
        // Create Authentication Token based on Provider
        val authenticationToken: Authentication = when (loginRequestDto.provider) {
            CredentialProvider.Self -> SelfAuthenticationToken(loginRequestDto.id, loginRequestDto.password)
        }

        // Authenticate(it will throw ApiException when authentication fails
        val authenticatedObject = authenticationManager.authenticate(authenticationToken)!!
        val userDetails = authenticatedObject.principal as CustomUserDetails

        return TokenResponse(jwtService.generateJwt(userDetails))
    }
}
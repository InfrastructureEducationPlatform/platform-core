package com.infrastructure.education.account.services

import com.infrastructure.education.account.dto.requests.RegisterRequestDto
import com.infrastructure.education.account.dto.responses.TokenResponse
import com.infrastructure.education.account.models.CredentialId
import com.infrastructure.education.account.repositories.AccountRepository
import com.infrastructure.education.account.repositories.CredentialRepository
import com.infrastructure.education.auth.models.CustomUserDetails
import com.infrastructure.education.auth.services.JwtService
import com.infrastructure.education.common.ApiException
import com.infrastructure.education.common.ErrorTitle
import org.springframework.data.repository.findByIdOrNull
import org.springframework.http.HttpStatus
import org.springframework.stereotype.Service
import org.springframework.transaction.annotation.Transactional

@Service
class AccountService(
        private val accountRepository: AccountRepository,
        private val credentialRepository: CredentialRepository,
        private val jwtService: JwtService
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
}
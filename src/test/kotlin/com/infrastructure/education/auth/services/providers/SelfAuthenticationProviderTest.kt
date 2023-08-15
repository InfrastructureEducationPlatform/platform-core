package com.infrastructure.education.auth.services.providers

import com.github.f4b6a3.ulid.UlidCreator
import com.infrastructure.education.account.models.Account
import com.infrastructure.education.account.models.Credential
import com.infrastructure.education.account.models.CredentialId
import com.infrastructure.education.account.models.CredentialProvider
import com.infrastructure.education.account.repositories.CredentialRepository
import com.infrastructure.education.auth.models.SelfAuthenticationToken
import com.infrastructure.education.common.ApiException
import com.infrastructure.education.common.ErrorTitle
import io.kotest.assertions.throwables.shouldThrowExactly
import io.kotest.core.spec.style.BehaviorSpec
import io.kotest.matchers.shouldBe
import io.kotest.matchers.shouldNotBe
import io.mockk.every
import io.mockk.mockk
import org.springframework.data.repository.findByIdOrNull
import org.springframework.http.HttpStatus
import org.springframework.security.crypto.password.PasswordEncoder

class SelfAuthenticationProviderTest : BehaviorSpec() {
    private val mockPasswordEncoder = mockk<PasswordEncoder>(relaxed = true)
    private val mockCredentialRepository = mockk<CredentialRepository>(relaxed = true)
    private val authenticationProvider = SelfAuthenticationProvider(mockPasswordEncoder, mockCredentialRepository)

    init {
        Given("authenticate") {
            When("Credential does not exists") {
                val selfAuthenticationToken = SelfAuthenticationToken("kangdroid@test.com", "testPassword@")
                every {
                    mockCredentialRepository.findByIdOrNull(CredentialId("kangdroid@test.com", CredentialProvider.Self))
                } returns null
                Then("Should throw ApiException with 401 Unauthorized.") {
                    shouldThrowExactly<ApiException> {
                        authenticationProvider.authenticate(selfAuthenticationToken)
                    }.let {
                        it.statusCode shouldBe HttpStatus.UNAUTHORIZED
                        it.errorTitle shouldBe ErrorTitle.ACCOUNT_NOT_FOUND
                        it.errorMessage shouldNotBe ""
                    }
                }
            }

            When("Password does not match") {
                val testAccount = createAccountAndSelfCredential()
                val testCredential = testAccount.credentialList.first()
                val selfAuthenticationToken = testAccount.createNonAuthenticationToken(overridePassword = "1234")
                every { mockCredentialRepository.findByIdOrNull(testCredential.id) } returns testCredential
                Then("Should throw ApiException with 401 Unauthorized.") {
                    shouldThrowExactly<ApiException> {
                        authenticationProvider.authenticate(selfAuthenticationToken)
                    }.let {
                        it.statusCode shouldBe HttpStatus.UNAUTHORIZED
                        it.errorTitle shouldBe ErrorTitle.ACCOUNT_CREDENTIAL_NOT_CORRECT
                        it.errorMessage shouldNotBe ""
                    }
                }
            }

            When("Successfully authenticated") {
                val testAccount = createAccountAndSelfCredential()
                val testCredential = testAccount.credentialList.first()
                val selfAuthenticationToken = testAccount.createNonAuthenticationToken()
                every { mockCredentialRepository.findByIdOrNull(testCredential.id) } returns testCredential
                every { mockPasswordEncoder.matches(selfAuthenticationToken.credentials as String, testCredential.credentialKey) } returns true
                Then("Should return SelfAuthenticationToken with authentication flag on.") {
                    val authenticationResult = authenticationProvider.authenticate(selfAuthenticationToken)
                    (authenticationResult is SelfAuthenticationToken) shouldBe true

                    val selfAuthenticationResult = authenticationResult as SelfAuthenticationToken
                    with(selfAuthenticationResult) {
                        isAuthenticated shouldBe true
                    }
                }
            }
        }
    }

    private fun createAccountAndSelfCredential(accountConfigurator: ((Account) -> Unit)? = null, credentialConfigurator: ((Credential) -> Unit)? = null): Account {
        val account = Account(
                id = UlidCreator.getUlid().toString(),
                name = "KangDroid",
                email = "kangdroid@test.com",
                profilePictureImageUrl = null,
                credentialList = mutableListOf()
        )
        val credential = Credential(
                id = CredentialId(
                        credentialId = "kangdroid@test.com",
                        provider = CredentialProvider.Self
                ),
                credentialKey = "testPassword@",
                account = account
        )

        // Custom Configurator
        accountConfigurator?.invoke(account)
        credentialConfigurator?.invoke(credential)

        credential.id.provider = CredentialProvider.Self
        account.credentialList.add(credential)

        return account
    }

    private fun Account.createNonAuthenticationToken(overrideEmail: String? = null, overridePassword: String? = null): SelfAuthenticationToken = SelfAuthenticationToken(
            email = overrideEmail ?: this.email,
            password = overridePassword ?: this.credentialList.first().credentialKey!!
    )
}
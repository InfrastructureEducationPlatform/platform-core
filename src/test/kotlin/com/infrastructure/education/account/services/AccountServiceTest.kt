package com.infrastructure.education.account.services

import com.infrastructure.education.account.dto.requests.RegisterRequestDto
import com.infrastructure.education.account.models.Account
import com.infrastructure.education.account.models.Credential
import com.infrastructure.education.account.models.CredentialProvider
import com.infrastructure.education.account.repositories.AccountRepository
import com.infrastructure.education.account.repositories.CredentialRepository
import com.infrastructure.education.auth.services.JwtService
import com.infrastructure.education.common.ApiException
import com.infrastructure.education.common.ErrorTitle
import io.kotest.assertions.throwables.shouldThrow
import io.kotest.core.spec.IsolationMode
import io.kotest.core.spec.style.BehaviorSpec
import io.kotest.matchers.shouldBe
import io.kotest.matchers.shouldNotBe
import io.mockk.every
import io.mockk.mockk
import io.mockk.slot
import io.mockk.verify
import org.springframework.data.repository.findByIdOrNull
import org.springframework.http.HttpStatus

class AccountServiceTest : BehaviorSpec({
    isolationMode = IsolationMode.InstancePerLeaf

    // Default Declaration
    val mockAccountRepository = mockk<AccountRepository>(relaxed = true)
    val mockCredentialRepository = mockk<CredentialRepository>(relaxed = true)
    val mockJwtService = mockk<JwtService>(relaxed = true)
    val accountService = AccountService(mockAccountRepository, mockCredentialRepository, mockJwtService)

    Given("createAccount") {
        val registerDto = RegisterRequestDto(
                credentialKey = "kangdroid",
                email = "kangdroid@test.com",
                credentialId = "asdf",
                name = "KangDroid",
                credentialProvider = CredentialProvider.Self,
                profileImageUrl = null
        )
        val desiredAccount = registerDto.toAccount()
        val desiredCredential = registerDto.toCredential(desiredAccount)
        val randomJwt = "test.test.test"
        When("Fresh new Account") {
            every { mockAccountRepository.findByEmail(registerDto.email) } returns null
            every { mockCredentialRepository.findByIdOrNull(any()) } returns null
            every { mockAccountRepository.save(any()) } returns desiredAccount
            every { mockCredentialRepository.save(any()) } returns desiredCredential
            every { mockJwtService.generateJwt(any()) } returns randomJwt

            val tokenResponse = accountService.createAccount(registerDto)
            Then("Should Saves to Account Repository well.") {
                val captureAccountSlot = slot<Account>()
                verify {
                    mockAccountRepository.save(capture(captureAccountSlot))
                }

                // Check account value captured
                captureAccountSlot.isCaptured shouldBe true
                with(captureAccountSlot.captured) {
                    id shouldNotBe ""
                    name shouldBe registerDto.name
                    email shouldBe registerDto.email
                    profilePictureImageUrl shouldBe registerDto.profileImageUrl
                }
            }

            Then("Should save credential to repository well.") {
                val captureCredentialSlot = slot<Credential>()
                verify {
                    mockCredentialRepository.save(capture(captureCredentialSlot))
                }

                // Check Credential Captured
                captureCredentialSlot.isCaptured shouldBe true
                with(captureCredentialSlot.captured) {
                    id.credentialId shouldBe desiredCredential.id.credentialId
                    id.provider shouldBe desiredCredential.id.provider
                    credentialKey shouldBe desiredCredential.credentialKey
                }
            }

            Then("Should have called jwt logic properly.") {
                verify { mockJwtService.generateJwt(any()) }
            }

            Then("Should have returned mock jwt.") {
                tokenResponse.token shouldBe randomJwt
            }
        }

        When("Account with same email exists") {
            every { mockAccountRepository.findByEmail(registerDto.email) } returns desiredAccount
            every { mockCredentialRepository.findByIdOrNull(any()) } returns desiredCredential
            Then("Should throw ApiException with HTTP Conflict") {
                val exception = shouldThrow<ApiException> {
                    accountService.createAccount(registerDto)
                }
                with(exception) {
                    statusCode shouldBe HttpStatus.CONFLICT
                    errorTitle shouldBe ErrorTitle.ACCOUNT_EMAIL_CONFLICT
                    errorMessage shouldNotBe ""
                }
            }
        }

        When("Account does not exists, but Credential exists") {
            every { mockAccountRepository.findByEmail(registerDto.email) } returns null
            every { mockCredentialRepository.findByIdOrNull(any()) } returns desiredCredential
            Then("Should throw ApiException with HTTP Conflict") {
                val exception = shouldThrow<ApiException> {
                    accountService.createAccount(registerDto)
                }
                with(exception) {
                    statusCode shouldBe HttpStatus.CONFLICT
                    errorTitle shouldBe ErrorTitle.ACCOUNT_EMAIL_CONFLICT
                    errorMessage shouldNotBe ""
                }
            }
        }
    }
})
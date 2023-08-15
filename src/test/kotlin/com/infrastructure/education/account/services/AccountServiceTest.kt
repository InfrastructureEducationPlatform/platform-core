package com.infrastructure.education.account.services

import com.infrastructure.education.account.dto.requests.LoginRequestDto
import com.infrastructure.education.account.dto.requests.RegisterRequestDto
import com.infrastructure.education.account.models.Account
import com.infrastructure.education.account.models.Credential
import com.infrastructure.education.account.models.CredentialId
import com.infrastructure.education.account.models.CredentialProvider
import com.infrastructure.education.account.repositories.AccountRepository
import com.infrastructure.education.account.repositories.CredentialRepository
import com.infrastructure.education.account.services.providers.SelfRegistrationProvider
import com.infrastructure.education.auth.models.CustomUserDetails
import com.infrastructure.education.auth.models.SelfAuthenticationToken
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
import org.springframework.security.authentication.AuthenticationManager

// SEE: https://github.com/mockk/mockk/issues/796 for using capturing vs withArgs
class AccountServiceTest : BehaviorSpec() {
    // Default Declaration
    private val mockAccountRepository = mockk<AccountRepository>(relaxed = true)
    private val mockCredentialRepository = mockk<CredentialRepository>(relaxed = true)
    private val mockJwtService = mockk<JwtService>(relaxed = true)
    private val mockAuthenticationManager = mockk<AuthenticationManager>(relaxed = true)
    private val mockSelfRegistrationProvider = mockk<SelfRegistrationProvider>(relaxed = true)
    private val accountService = AccountService(mockAccountRepository, mockCredentialRepository, mockJwtService, mockAuthenticationManager, mockSelfRegistrationProvider)

    init {
        isolationMode = IsolationMode.InstancePerLeaf

        Given("createAccount") {
            val registerDto = createRegisterDto()
            val desiredAccount = registerDto.toAccount()
            val desiredCredential = registerDto.createSelfCredential(desiredAccount)
            val randomJwt = "test.test.test"
            When("Fresh new Account") {
                val accountCaptured = slot<Account>()
                every { mockAccountRepository.findByEmail(registerDto.email) } returns null
                every { mockAccountRepository.save(capture(accountCaptured)) } returns desiredAccount
                every { mockCredentialRepository.findByIdOrNull(any()) } returns null
                every { mockCredentialRepository.save(any()) } returns desiredCredential
                every { mockJwtService.generateJwt(any()) } returns randomJwt
                every { mockSelfRegistrationProvider.createCredential(any(), any()) } returns desiredCredential

                // Do
                val tokenResponse = accountService.createAccount(registerDto)

                Then("Should save account information to repository well.") {
                    // Verify Account Saved
                    verify { mockAccountRepository.save(capture(accountCaptured)) }
                    accountCaptured.isCaptured shouldBe true
                    with(accountCaptured.captured) {
                        id shouldNotBe ""
                        name shouldBe registerDto.name
                        email shouldBe registerDto.email
                        profilePictureImageUrl shouldBe registerDto.profileImageUrl
                    }
                }

                Then("Should save credential information to repository well.") {
                    // Verify Credential Saved
                    val credentialSlot = slot<Credential>()
                    verify {
                        mockCredentialRepository.save(capture(credentialSlot))
                    }
                    with(credentialSlot) {
                        captured.id.credentialId shouldBe desiredCredential.id.credentialId
                        captured.id.provider shouldBe desiredCredential.id.provider
                        captured.credentialKey shouldBe desiredCredential.credentialKey
                    }
                }

                Then("Should create JWT and return it well.") {
                    // Verify JWT
                    verify {
                        mockJwtService.generateJwt(withArg {
                            it.accountId shouldBe accountCaptured.captured.id
                        })
                    }
                    tokenResponse.token shouldBe randomJwt
                }
            }

            When("Account with same email exists") {
                every { mockAccountRepository.findByEmail(registerDto.email) } returns desiredAccount
                every { mockCredentialRepository.findByIdOrNull(any()) } returns desiredCredential

                // Do
                val exception = shouldThrow<ApiException> {
                    accountService.createAccount(registerDto)
                }

                Then("Should tried to search credential repository.") {
                    verify {
                        mockCredentialRepository.findByIdOrNull(withArg {
                            it.credentialId shouldBe registerDto.credentialId
                            it.provider shouldBe registerDto.credentialProvider
                        })
                    }
                }

                Then("Should tried to search account repository.") {
                    verify { mockAccountRepository.findByEmail(registerDto.email) }
                }

                Then("Should throw ApiException with HTTP Conflict") {
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

                // Do
                val exception = shouldThrow<ApiException> {
                    accountService.createAccount(registerDto)
                }

                Then("Should have called accountRepository logic") {
                    verify {
                        mockAccountRepository.findByEmail(registerDto.email)
                    }
                }

                Then("Should have called credentialRepository logic") {
                    verify {
                        mockCredentialRepository.findByIdOrNull(withArg {
                            it.credentialId shouldBe registerDto.credentialId
                            it.provider shouldBe registerDto.credentialProvider
                        })
                    }
                }

                Then("Should throw ApiException with HTTP Conflict") {
                    with(exception) {
                        statusCode shouldBe HttpStatus.CONFLICT
                        errorTitle shouldBe ErrorTitle.ACCOUNT_EMAIL_CONFLICT
                        errorMessage shouldNotBe ""
                    }
                }
            }
        }

        Given("loginAccount") {
            val loginRequestDto = createLoginRequestDto()
            val mockCustomUserDetails = mockk<CustomUserDetails>()
            When("Self Provider Request succeed") {
                every { mockCustomUserDetails.accountId } returns "kangdroid"
                every { mockAuthenticationManager.authenticate(any()) } returns SelfAuthenticationToken(mockCustomUserDetails)
                every { mockJwtService.generateJwt(any()) } returns "testJwt"

                // Do
                val response = accountService.loginAccount(loginRequestDto)

                Then("Should have called authentication method with SelfAuthenticationToken") {
                    verify {
                        mockAuthenticationManager.authenticate(withArg {
                            (it is SelfAuthenticationToken) shouldBe true
                        })
                    }
                }

                Then("Should have returned mocked JWT.") {
                    response.token shouldBe "testJwt"
                }
            }
        }
    }

    private fun createRegisterDto(editAction: ((RegisterRequestDto) -> Unit)? = null): RegisterRequestDto {
        val registerDto = RegisterRequestDto(
                credentialKey = "kangdroid",
                email = "kangdroid@test.com",
                credentialId = "asdf",
                name = "KangDroid",
                credentialProvider = CredentialProvider.Self,
                profileImageUrl = null
        )

        editAction?.invoke(registerDto)

        return registerDto
    }

    private fun createLoginRequestDto(editAction: ((LoginRequestDto) -> Unit)? = null): LoginRequestDto {
        val loginRequestDto = LoginRequestDto(
                provider = CredentialProvider.Self,
                id = "kangdroid@test.com",
                password = "testPassword@"
        )

        editAction?.invoke(loginRequestDto)
        return loginRequestDto
    }

    private fun RegisterRequestDto.createSelfCredential(account: Account) = Credential(
            id = CredentialId(credentialId = credentialId, provider = credentialProvider),
            credentialKey = credentialKey,
            account = account
    )
}
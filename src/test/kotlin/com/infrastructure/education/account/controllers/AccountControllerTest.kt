package com.infrastructure.education.account.controllers

import com.github.f4b6a3.ulid.UlidCreator
import com.infrastructure.education.Transaction
import com.infrastructure.education.account.dto.requests.RegisterRequestDto
import com.infrastructure.education.account.models.CredentialProvider
import com.infrastructure.education.account.repositories.AccountRepository
import com.infrastructure.education.account.repositories.CredentialRepository
import io.kotest.core.spec.style.BehaviorSpec
import io.kotest.extensions.spring.SpringExtension
import io.kotest.matchers.shouldBe
import io.kotest.matchers.shouldNotBe
import org.springframework.boot.test.context.SpringBootTest
import org.springframework.http.HttpStatus
import org.springframework.http.MediaType
import org.springframework.test.context.DynamicPropertyRegistry
import org.springframework.test.context.DynamicPropertySource
import org.springframework.test.web.reactive.server.WebTestClient
import org.springframework.web.reactive.function.BodyInserters
import org.testcontainers.containers.PostgreSQLContainer
import org.testcontainers.junit.jupiter.Container
import org.testcontainers.junit.jupiter.Testcontainers

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@Testcontainers
class AccountControllerTest(
        private val webTestClient: WebTestClient,
        private val accountRepository: AccountRepository,
        private val credentialRepository: CredentialRepository,
        private val transaction: Transaction
) : BehaviorSpec() {
    override fun extensions() = listOf(SpringExtension)

    private val clearData: suspend () -> Unit = {
        transaction {
            credentialRepository.deleteAll()
            accountRepository.deleteAll()
        }
    }

    companion object {
        @Container
        val postgreSQLContainer = PostgreSQLContainer<Nothing>("postgres:15.3")
                .apply {
                    withDatabaseName(UlidCreator.getUlid().toString())
                    withUsername("admin")
                    withPassword("testPassword@")
                }

        init {
            postgreSQLContainer.start()
        }

        @DynamicPropertySource
        @JvmStatic
        fun dynamicProperties(registry: DynamicPropertyRegistry) {
            registry.add("spring.datasource.url", postgreSQLContainer::getJdbcUrl)
            registry.add("spring.datasource.username", postgreSQLContainer::getUsername)
            registry.add("spring.datasource.password", postgreSQLContainer::getPassword)
        }
    }

    init {
        Given("HTTP POST /v1/accounts") {
            val registerRequestDto = RegisterRequestDto(
                    credentialKey = "kangdroid",
                    email = "kangdroid@test.com",
                    credentialId = "kangdroid@test.com",
                    name = "KangDroid",
                    credentialProvider = CredentialProvider.Self,
                    profileImageUrl = null
            )

            When("Registering Fresh Account") {
                val exchangeSpec = webTestClient.post()
                        .uri("/v1/accounts/register")
                        .contentType(MediaType.APPLICATION_JSON)
                        .body(BodyInserters.fromValue(registerRequestDto))
                        .exchange()

                Then("Should return HTTP 204 NoContent") {
                    exchangeSpec.expectStatus().isNoContent
                }

                Then("Should save account and its data well.") {
                    transaction {
                        val accountList = accountRepository.findAll()
                        accountList.count() shouldBe 1
                        val account = accountList.first()
                        account.id shouldNotBe ""
                        account.email shouldBe registerRequestDto.email
                        account.name shouldBe registerRequestDto.name
                        account.profilePictureImageUrl shouldBe registerRequestDto.profileImageUrl
                        account.credentialList.count() shouldBe 1
                    }
                }

                clearData()
            }

            When("Registering Same Account") {
                transaction {
                    val account = registerRequestDto.toAccount()
                    accountRepository.save(account)
                    val credential = registerRequestDto.toCredential(account)
                    credentialRepository.save(credential)
                }

                val exchangeSpec = webTestClient.post()
                        .uri("/v1/accounts/register")
                        .contentType(MediaType.APPLICATION_JSON)
                        .body(BodyInserters.fromValue(registerRequestDto))
                        .exchange()

                Then("Should return HTTP 409 Conflict") {
                    exchangeSpec.expectStatus().isEqualTo(HttpStatus.CONFLICT)
                }

                Then("Should not register additional account.") {
                    transaction {
                        val accountList = accountRepository.findAll()
                        accountList.count() shouldBe 1
                        with(accountList.first()) {
                            id shouldNotBe ""
                            email shouldBe registerRequestDto.email
                            name shouldBe registerRequestDto.name
                            profilePictureImageUrl shouldBe registerRequestDto.profileImageUrl
                            credentialList.count() shouldBe 1
                        }
                    }
                }

                clearData()
            }
        }
    }
}
package com.infrastructure.education.account.controllers

import com.infrastructure.education.account.dto.requests.RegisterRequestDto
import com.infrastructure.education.account.services.AccountService
import io.swagger.v3.oas.annotations.Operation
import io.swagger.v3.oas.annotations.responses.ApiResponse
import io.swagger.v3.oas.annotations.tags.Tag
import org.springframework.http.ResponseEntity
import org.springframework.web.bind.annotation.PostMapping
import org.springframework.web.bind.annotation.RequestBody
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RestController

@RestController
@RequestMapping("/v1/accounts")
@Tag(name = "Accounts", description = "Account related API.")
class AccountController(
    private val accountService: AccountService
) {
    @PostMapping("/register")
    @Operation(summary = "Register Account.")
    @ApiResponse(responseCode = "204", description = "When successfully registered account to the server.")
    fun registerAccount(@RequestBody registerRequestDto: RegisterRequestDto): ResponseEntity<Unit> {
        accountService.createAccount(registerRequestDto)
        return ResponseEntity.noContent().build();
    }
}
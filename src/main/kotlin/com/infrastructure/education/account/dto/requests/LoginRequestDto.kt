package com.infrastructure.education.account.dto.requests

import com.infrastructure.education.account.models.CredentialProvider
import io.swagger.v3.oas.annotations.media.Schema

data class LoginRequestDto(
        @Schema(description = "ID(")
        val id: String,

        @Schema(description = "Password(Password for Self Provider, Auth Token for Other.)")
        val password: String,

        @Schema(description = "Credential Provider(Such as self, oauth, etc)")
        val provider: CredentialProvider
)
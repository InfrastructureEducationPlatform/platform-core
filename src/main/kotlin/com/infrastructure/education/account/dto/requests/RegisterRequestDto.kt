package com.infrastructure.education.account.dto.requests

import com.github.f4b6a3.ulid.UlidCreator
import com.infrastructure.education.account.models.Account
import com.infrastructure.education.account.models.CredentialProvider
import io.swagger.v3.oas.annotations.media.Schema

data class RegisterRequestDto(
        @Schema(description = "User Nickname")
        val name: String,

        @Schema(description = "User Email")
        val email: String,

        @Schema(description = "Profile Image URL(Nullable)")
        val profileImageUrl: String?,

        @Schema(description = "Credential ID")
        val credentialId: String,

        @Schema(description = "Credential Key")
        val credentialKey: String,

        @Schema(description = "Credential Provider(Such as self, oauth, etc)")
        val credentialProvider: CredentialProvider
) {
    fun toAccount(): Account = Account(
            name = name,
            email = email,
            profilePictureImageUrl = profileImageUrl,
            credentialList = mutableListOf(),
            id = UlidCreator.getUlid().toString()
    )
}
package com.infrastructure.education.account.models

import jakarta.persistence.*
import java.io.Serializable

enum class CredentialProvider {
    Self
}

@Embeddable
data class CredentialId(
    @Column
    var credentialId: String,

    @Column
    @Enumerated(EnumType.STRING)
    var provider: CredentialProvider
): Serializable

@Entity
@Table(name = "Credentials")
class Credential(
    @EmbeddedId
    var id: CredentialId,

    @Column
    var credentialKey: String,

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "account_id")
    val account: Account,
)
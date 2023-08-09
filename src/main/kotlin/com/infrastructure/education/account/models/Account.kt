package com.infrastructure.education.account.models

import jakarta.persistence.CascadeType
import jakarta.persistence.Column
import jakarta.persistence.Entity
import jakarta.persistence.Id
import jakarta.persistence.OneToMany
import jakarta.persistence.Table

@Entity
@Table(name = "Accounts")
class Account(
    @Id
    val id: String,

    @Column
    var name: String,

    @Column(unique = true)
    var email: String,

    @Column
    var profilePictureImageUrl: String?,

    @OneToMany(mappedBy = "account")
    var credentialList: MutableList<Credential>
)
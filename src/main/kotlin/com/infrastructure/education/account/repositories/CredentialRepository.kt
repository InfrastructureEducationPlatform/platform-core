package com.infrastructure.education.account.repositories

import com.infrastructure.education.account.models.Credential
import com.infrastructure.education.account.models.CredentialId
import org.springframework.data.jpa.repository.JpaRepository

interface CredentialRepository : JpaRepository<Credential, CredentialId>
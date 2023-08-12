package com.infrastructure.education.account.repositories

import com.infrastructure.education.account.models.Account
import org.springframework.data.jpa.repository.JpaRepository

interface AccountRepository : JpaRepository<Account, String> {
    fun findByEmail(email: String): Account?
}
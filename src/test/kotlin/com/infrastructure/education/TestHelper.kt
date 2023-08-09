package com.infrastructure.education

import io.kotest.core.names.TestName
import io.kotest.core.spec.style.scopes.BehaviorSpecGivenContainerScope
import io.kotest.core.spec.style.scopes.BehaviorSpecWhenContainerScope
import io.kotest.core.test.TestScope
import org.springframework.data.jpa.repository.JpaRepository
import org.springframework.stereotype.Component
import org.springframework.transaction.annotation.Transactional


@Component
class Transaction {
    @Transactional
    suspend operator fun <T> invoke(transactionScopeFunction: suspend () -> T?): T? {
        return transactionScopeFunction()
    }
}
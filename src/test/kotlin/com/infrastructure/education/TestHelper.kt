package com.infrastructure.education

import org.springframework.stereotype.Component
import org.springframework.transaction.annotation.Transactional


@Component
class Transaction {
    @Transactional
    suspend operator fun <T> invoke(transactionScopeFunction: suspend () -> T?): T? {
        return transactionScopeFunction()
    }
}
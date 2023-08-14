package com.infrastructure.education.auth.models

import org.springframework.security.authentication.AbstractAuthenticationToken

class SelfAuthenticationToken private constructor(
        private val emailId: String?,
        private val password: String?,
        private val userDetails: CustomUserDetails?
) : AbstractAuthenticationToken(null) {

    // When Unauthenticated
    constructor(email: String, password: String) : this(email, password, null) {
        isAuthenticated = false
    }

    // When Authenticated
    constructor(userDetails: CustomUserDetails) : this(null, null, userDetails) {
        isAuthenticated = true
    }

    // Credentials -> Password
    override fun getCredentials(): Any? = password

    // Principal -> Email ID when Not Authenticated
    // Principal -> User Details when Authenticated
    override fun getPrincipal(): Any? = if (isAuthenticated) userDetails else emailId
}
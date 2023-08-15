package com.infrastructure.education.auth.services

import com.infrastructure.education.account.repositories.CredentialRepository
import com.infrastructure.education.auth.services.providers.SelfAuthenticationProvider
import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration
import org.springframework.http.HttpMethod
import org.springframework.security.authentication.AuthenticationManager
import org.springframework.security.config.annotation.authentication.builders.AuthenticationManagerBuilder
import org.springframework.security.config.annotation.web.builders.HttpSecurity
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity
import org.springframework.security.config.http.SessionCreationPolicy
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder
import org.springframework.security.crypto.password.PasswordEncoder
import org.springframework.security.web.SecurityFilterChain


@Configuration
@EnableWebSecurity
class SecurityConfiguration(
        private val credentialRepository: CredentialRepository
) {
    private val allowedUrls = arrayOf(
            "/swagger-resources/**",
            "/swagger-ui/**",
            "/api-docs/**",
            "/v3/api-docs/**",
            "/v1/accounts/register",
            "/v1/accounts/login"
    )

    @Bean
    fun securityFilterChain(http: HttpSecurity): SecurityFilterChain = http
            .csrf { it.disable() }
            .authorizeHttpRequests {
                it.requestMatchers(*allowedUrls)
                        .permitAll()
                it.requestMatchers(HttpMethod.OPTIONS)
                        .permitAll()
                        .anyRequest()
                        .authenticated()
            }
            .sessionManagement { it.sessionCreationPolicy(SessionCreationPolicy.STATELESS) }
            .build()

    @Bean
    fun passwordEncoder(): PasswordEncoder = BCryptPasswordEncoder()

    @Bean
    fun authManager(http: HttpSecurity): AuthenticationManager {
        val authBuilder = http.getSharedObject(AuthenticationManagerBuilder::class.java)!!

        return authBuilder
                .authenticationProvider(SelfAuthenticationProvider(passwordEncoder(), credentialRepository))
                .build()
    }
}
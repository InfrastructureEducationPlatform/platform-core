package com.infrastructure.education.auth.services

import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration
import org.springframework.http.HttpMethod
import org.springframework.security.config.annotation.web.builders.HttpSecurity
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity
import org.springframework.security.config.http.SessionCreationPolicy
import org.springframework.security.web.SecurityFilterChain


@Configuration
@EnableWebSecurity
class SecurityConfiguration {
    private val allowedUrls = arrayOf(
            "/swagger-resources/**",
            "/swagger-ui/**",
            "/api-docs/**",
            "/v3/api-docs/**",
            "/v1/accounts/register"
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
}
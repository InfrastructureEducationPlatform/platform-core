package com.infrastructure.education.auth.services

import com.infrastructure.education.auth.models.CustomUserDetails
import io.jsonwebtoken.Jwts
import io.jsonwebtoken.SignatureAlgorithm
import io.jsonwebtoken.io.Decoders
import io.jsonwebtoken.security.Keys
import org.springframework.stereotype.Service
import java.security.Key
import java.util.*


@Service
class JwtService {
    private val key: String = "fuyh7fSzXcDFoJZ4gu2E/19dK+JnzsKsy9cu0UTVjKc="
    private val expiration: Int = 1000 * 60 * 10 // 10 min

    fun generateJwt(customUserDetail: CustomUserDetails): String {
        return Jwts.builder()
                .setClaims(mapOf<String, Any>())
                .setSubject(customUserDetail.accountId)
                .setExpiration(Date(System.currentTimeMillis() + expiration))
                .signWith(getSigningKey(), SignatureAlgorithm.HS256).compact()
    }

    private fun getSigningKey(): Key {
        val keyBytes = Decoders.BASE64.decode(key)
        return Keys.hmacShaKeyFor(keyBytes)
    }
}
# Error Examples (RFC 9457)

Examples of error responses following [RFC 9457](https://www.rfc-editor.org/rfc/rfc9457) — `application/problem+json`. RFC 9457 obsoletes [RFC 7807](https://www.rfc-editor.org/rfc/rfc7807) (July 2023) and is backward-compatible.

## Fields

- `type` — URI identifying the problem type (dereferenceable if possible).
- `title` — short, human-readable summary.
- `status` — HTTP status code.
- `detail` — human-readable explanation specific to this occurrence.
- `instance` — URI identifying the specific occurrence.

Extension members are allowed (e.g. `errors`, `traceId`).

---

## 400 Bad Request — Validation Error

```http
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json
```

```json
{
  "type": "https://example.com/probs/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "The request body failed validation.",
  "instance": "/orders",
  "errors": {
    "email": ["The email field is required."],
    "quantity": ["Quantity must be greater than 0."]
  },
  "traceId": "00-2f1b7c8a4d3e6f5a-9b8c7d6e5f4a3b2c-01"
}
```

---

## 401 Unauthorized

```http
HTTP/1.1 401 Unauthorized
Content-Type: application/problem+json
```

```json
{
  "type": "https://example.com/probs/unauthorized",
  "title": "Authentication required.",
  "status": 401,
  "detail": "The access token is missing or expired.",
  "instance": "/account/me"
}
```

---

## 403 Forbidden

```http
HTTP/1.1 403 Forbidden
Content-Type: application/problem+json
```

```json
{
  "type": "https://example.com/probs/forbidden",
  "title": "Insufficient permissions.",
  "status": 403,
  "detail": "You do not have permission to access this resource.",
  "instance": "/admin/users/42"
}
```

---

## 404 Not Found

```http
HTTP/1.1 404 Not Found
Content-Type: application/problem+json
```

```json
{
  "type": "https://example.com/probs/not-found",
  "title": "Resource not found.",
  "status": 404,
  "detail": "No product exists with id 9999.",
  "instance": "/products/9999"
}
```

---

## 409 Conflict

```http
HTTP/1.1 409 Conflict
Content-Type: application/problem+json
```

```json
{
  "type": "https://example.com/probs/conflict",
  "title": "Resource conflict.",
  "status": 409,
  "detail": "A user with the email 'jane@example.com' already exists.",
  "instance": "/users"
}
```

---

## 422 Unprocessable Entity — Business Rule Violation

```http
HTTP/1.1 422 Unprocessable Entity
Content-Type: application/problem+json
```

```json
{
  "type": "https://example.com/probs/insufficient-funds",
  "title": "Insufficient funds.",
  "status": 422,
  "detail": "Your current balance is 30.00, but the charge is 50.00.",
  "instance": "/accounts/12345/transactions",
  "balance": 30.0,
  "required": 50.0
}
```

---

## 429 Too Many Requests

```http
HTTP/1.1 429 Too Many Requests
Content-Type: application/problem+json
Retry-After: 60
```

```json
{
  "type": "https://example.com/probs/rate-limited",
  "title": "Too many requests.",
  "status": 429,
  "detail": "Rate limit exceeded. Try again in 60 seconds.",
  "instance": "/search",
  "retryAfterSeconds": 60
}
```

---

## 500 Internal Server Error

```http
HTTP/1.1 500 Internal Server Error
Content-Type: application/problem+json
```

```json
{
  "type": "about:blank",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred. Please contact support with the traceId.",
  "instance": "/reports/generate",
  "traceId": "00-2f1b7c8a4d3e6f5a-9b8c7d6e5f4a3b2c-01"
}
```

> Note: `type` defaults to `"about:blank"` when no specific problem type is provided. In that case, `title` SHOULD be the standard HTTP status reason phrase.

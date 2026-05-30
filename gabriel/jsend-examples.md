# JSend Response Examples

JSend is a simple specification for formatting JSON responses from APIs. It provides a consistent envelope so clients always know how to parse the result.

Full spec: https://github.com/omniti-labs/jsend

---

## The 3 Response Types

| Status    | When to use                          | Required fields          |
|-----------|--------------------------------------|--------------------------|
| `success` | Request worked, here's the data      | `status`, `data`         |
| `fail`    | Request was invalid (client error)   | `status`, `data`         |
| `error`   | Something broke on the server        | `status`, `message`      |

---

## Success Responses

### Single Resource

**GET /api/games/1** → 200 OK

```json
{
  "status": "success",
  "data": {
    "id": 1,
    "title": "Street Fighter II",
    "genre": "Fighting",
    "price": 19.99
  }
}
```

### Collection

**GET /api/games** → 200 OK

```json
{
  "status": "success",
  "data": [
    { "id": 1, "title": "Street Fighter II", "genre": "Fighting", "price": 19.99 },
    { "id": 2, "title": "Minecraft", "genre": "Sandbox", "price": 29.99 }
  ]
}
```

### Empty Collection

**GET /api/games** → 200 OK (no games exist yet)

```json
{
  "status": "success",
  "data": []
}
```

### Create Resource

**POST /api/games** → 201 Created

```json
{
  "status": "success",
  "data": {
    "id": 3,
    "title": "Zelda",
    "genre": "Adventure",
    "price": 59.99
  }
}
```

### Delete Resource

**DELETE /api/games/1** → 200 OK

```json
{
  "status": "success",
  "data": null
}
```

---

## Fail Responses (Client Errors)

Use `fail` when the client sent something wrong — validation errors, missing fields, bad IDs, etc.

### Validation Error

**POST /api/games** (missing required fields) → 400 Bad Request

```json
{
  "status": "fail",
  "data": {
    "title": "Title is required",
    "price": "Price must be a positive number"
  }
}
```

### Not Found

**GET /api/games/999** → 404 Not Found

```json
{
  "status": "fail",
  "data": {
    "id": "No game with id 999 exists"
  }
}
```

### Unauthorized

**GET /api/admin/users** (no token) → 401 Unauthorized

```json
{
  "status": "fail",
  "data": {
    "authorization": "Authentication token is required"
  }
}
```

### Forbidden

**DELETE /api/games/1** (not an admin) → 403 Forbidden

```json
{
  "status": "fail",
  "data": {
    "authorization": "You do not have permission to delete games"
  }
}
```

---

## Error Responses (Server Errors)

Use `error` when something went wrong on the server — database down, unhandled exception, etc.

### Internal Server Error

**GET /api/games** → 500 Internal Server Error

```json
{
  "status": "error",
  "message": "An unexpected error occurred"
}
```

### With Optional Code and Data

```json
{
  "status": "error",
  "message": "Database connection failed",
  "code": "DB_CONNECTION_ERROR",
  "data": {
    "retryAfter": 30
  }
}
```

---

## Pagination (Custom Extension)

JSend doesn't define pagination, but you can add a `meta` field:

**GET /api/games?page=2&pageSize=10** → 200 OK

```json
{
  "status": "success",
  "data": [
    { "id": 11, "title": "Zelda", "price": 59.99 },
    { "id": 12, "title": "Mario", "price": 49.99 }
  ],
  "meta": {
    "totalItems": 50,
    "totalPages": 5,
    "currentPage": 2,
    "pageSize": 10
  }
}
```

---

## Summary

| Field     | Type             | Required in        | Description                        |
|-----------|------------------|--------------------|------------------------------------|
| `status`  | string           | All responses      | `"success"`, `"fail"`, or `"error"` |
| `data`    | object/array/null | success, fail      | The payload or field-level errors  |
| `message` | string           | error              | Human-readable error description   |
| `code`    | string/number    | Optional (error)   | Machine-readable error code        |

---

## Quick Decision Guide

```
Was the request successful?
├── Yes → { "status": "success", "data": { ... } }
│
├── No, client's fault (bad input, not found, unauthorized)
│   → { "status": "fail", "data": { "field": "reason" } }
│
└── No, server's fault (crash, timeout, DB down)
    → { "status": "error", "message": "what went wrong" }
```


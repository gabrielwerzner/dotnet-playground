# JSON:API Response Examples

JSON:API is a specification for building APIs that return JSON. It defines a consistent structure so clients always know what to expect.

Full spec: https://jsonapi.org/

---

## Single Resource

**GET /api/games/1**

```json
{
  "data": {
    "type": "games",
    "id": "1",
    "attributes": {
      "title": "Street Fighter II",
      "genre": "Fighting",
      "price": 19.99
    }
  }
}
```

**Key points:**
- `type` — what kind of resource this is
- `id` — unique identifier (always a string)
- `attributes` — the actual fields/properties

---

## Collection of Resources

**GET /api/games**

```json
{
  "data": [
    {
      "type": "games",
      "id": "1",
      "attributes": {
        "title": "Street Fighter II",
        "genre": "Fighting",
        "price": 19.99
      }
    },
    {
      "type": "games",
      "id": "2",
      "attributes": {
        "title": "Minecraft",
        "genre": "Sandbox",
        "price": 29.99
      }
    }
  ]
}
```

---

## Resource with Relationships

**GET /api/games/1?include=genre**

```json
{
  "data": {
    "type": "games",
    "id": "1",
    "attributes": {
      "title": "Street Fighter II",
      "price": 19.99
    },
    "relationships": {
      "genre": {
        "data": { "type": "genres", "id": "5" }
      }
    }
  },
  "included": [
    {
      "type": "genres",
      "id": "5",
      "attributes": {
        "name": "Fighting"
      }
    }
  ]
}
```

**Key points:**
- `relationships` — links this resource to another
- `included` — the full related resource (avoids extra API calls)

---

## Empty Collection

**GET /api/games** (no games exist yet)

```json
{
  "data": []
}
```

---

## Error Response

**GET /api/games/999** (not found)

```json
{
  "errors": [
    {
      "status": "404",
      "title": "Not Found",
      "detail": "No game with id 999 exists."
    }
  ]
}
```

---

## Pagination

**GET /api/games?page[number]=2&page[size]=10**

```json
{
  "data": [
    { "type": "games", "id": "11", "attributes": { "title": "Zelda" } },
    { "type": "games", "id": "12", "attributes": { "title": "Mario" } }
  ],
  "meta": {
    "totalItems": 50,
    "totalPages": 5,
    "currentPage": 2
  },
  "links": {
    "self": "/api/games?page[number]=2&page[size]=10",
    "first": "/api/games?page[number]=1&page[size]=10",
    "prev": "/api/games?page[number]=1&page[size]=10",
    "next": "/api/games?page[number]=3&page[size]=10",
    "last": "/api/games?page[number]=5&page[size]=10"
  }
}
```

---

## Summary

| Section         | Purpose                                      |
|-----------------|----------------------------------------------|
| `data`          | The main resource(s)                         |
| `type`          | Resource type (like a table name)            |
| `id`            | Unique identifier                            |
| `attributes`    | The resource's fields                        |
| `relationships` | Links to related resources                   |
| `included`      | Full related resources (to avoid N+1 calls)  |
| `meta`          | Extra info (pagination counts, etc.)         |
| `links`         | Navigation URLs                              |
| `errors`        | Array of error objects                       |


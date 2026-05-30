# API Design Rules

## Rule 1: Use plural nouns for your endpoints

When you name a URL for a group of things, use the plural form, and stick with it everywhere:

```
GET  /products         → list all products
GET  /products/123     → get one product
POST /products         → create a product
```

**Not** `/product/123` for one and `/products` for the list. Pick plural and use it for both the collection and the single-item URL.

**Why this matters:**

- It's the convention every major API uses (Stripe, GitHub, etc.), so anyone calling your API already expects it.
- Mixing singular and plural forces consumers to memorize which form you picked for each resource. That's wasted effort for zero benefit.
- `GET /products/123` reads naturally as "from the products collection, give me 123."

**One exception:** if there's only ever one of something for the caller — like the current user or account — singular is fine and clearer:

```
GET /me
GET /account
```

Don't force `/accounts/current` just to keep things plural.

## Rule 2: Keep single-item URLs flat

If an ID is globally unique (UUIDs, prefixed IDs like `ord_abc123`, or just a number that's unique across the whole table), the URL only needs that one ID to find the thing:

```
GET /orders/ord_abc123          ✅ good
GET /customers/cus_42/orders/ord_abc123   ❌ unnecessary
```

The server doesn't need the customer ID to look up the order — the order ID alone is enough. Adding the parent just makes the URL longer and locks the relationship into your public API forever (now you can never reassign an order to a different customer without breaking links).

**Two important exceptions:**

**1. Listing sub-resources — nesting is good here.**

```
GET /customers/cus_42/orders    → list one customer's orders
```

This reads naturally and is clearer than `/orders?customer_id=cus_42`.

**Note:** this rule is about *API* URLs, not page URLs in a web app. For a website, hierarchical URLs like `/categories/tech/threads/42` give you nice breadcrumbs and shareable links at every level — that's a UX choice for human navigation, separate from API design.

## Rule 3: Don't put file extensions in URLs

Skip `.json`, `.xml`, and friends in your endpoint paths:

```
GET /products/123        ✅ good
GET /products/123.json   ❌ noise
```

**Why this matters:**

- Every endpoint returns JSON anyway, so `.json` adds no information.
- It looks like you're serving files off disk, which you're not. It just confuses new developers.

If a client ever really needs a different format, they can ask for it with an `Accept` header — but honestly, this almost never comes up.

## Rule 4: Wrap list responses in an object

When an endpoint returns a list of things, don't make the top-level response a raw array. Put it inside an object:

```json
// ❌ avoid
[
  { "id": "prod_1", "name": "Shirt" },
  { "id": "prod_2", "name": "Hat" }
]

// ✅ prefer
{
  "data": [
    { "id": "prod_1", "name": "Shirt" },
    { "id": "prod_2", "name": "Hat" }
  ]
}
```

**Why this matters:**

- A bare array gives you nowhere to add new fields later.
- Almost every list endpoint eventually needs extras: pagination info, a total count, warnings, deprecation notices. With a wrapper, you can add them without breaking any client:

```json
{
  "data": [ ... ],
  "pagination": { "next_cursor": "abc", "has_more": true }
}
```

- Single-item endpoints already return an object (`GET /products/123`). Wrapping lists keeps the shape predictable everywhere — clients always parse the top level the same way.

**Pick one wrapper key and stick with it.** Common choices are `data`, `items`, or `results`. Any of them is fine — just don't mix them across endpoints.

## Rule 5: Return lists as arrays, not maps

When returning a collection, use an array of objects — not an object keyed by ID.

```json
// ❌ avoid
{
  "data": {
    "prod_1": { "name": "Shirt" },
    "prod_2": { "name": "Hat" }
  }
}

// ✅ prefer
{
  "data": [
    { "id": "prod_1", "name": "Shirt" },
    { "id": "prod_2", "name": "Hat" }
  ]
}
```

**Why this matters:**

- **Order is preserved and obvious.** Arrays are explicitly ordered; object key order is fragile to rely on.
- **Each object is self-describing.** With an array, the `id` lives inside the object, so you can pass a single `Product` around your code and it still knows what it is. With a map, the ID only exists as the outer key — every function that handles one item needs the ID passed separately.
- **It matches a clean TypeScript interface:**

```ts
interface Product { id: string; name: string; }
type Response = { data: Product[] };
```

## Rule 6: Use strings for IDs, even when they're numbers

Whenever your API returns an ID, send it as a string — never a raw number.

```json
// ❌ avoid
{ "id": 12345 }

// ✅ prefer
{ "id": "12345" }
```

This applies even if the ID is just an auto-incrementing integer in your database. The wire format and the storage format don't have to match.

**Why this matters:**

- **Future-proofing.** Numeric IDs lock you into one format forever. The day you want to switch to UUIDs, add prefixes like `ord_abc123`, or use a distributed ID scheme, every client that parsed `id` as a number breaks. String IDs absorb all those changes silently.

- **JavaScript can't handle large integers.** JS numbers lose precision past 2^53. A big database ID like `9007199254740993` will silently round to `9007199254740992` when parsed as a number. Stringify it and the problem disappears. (Twitter hit this exact issue years ago and had to migrate their whole API.)

- **It hides implementation details.** A response of `{ "id": 123 }` tells the world "I'm using sequential integers." Clients will then start assuming they can sort by ID, guess the next one (`/products/124`), or do arithmetic on them. None of that is your API's promise — but you've leaked enough to make it look like one.

- **It prevents a class of client bugs.** Clients can't accidentally compare IDs with `<`/`>` or treat them as numeric. An ID is an opaque identifier, not a quantity.

**Rule of thumb:** if it identifies something, make it a string. Numbers are for things you'd do math on — prices, counts, durations.

## Rule 7: Give your IDs a type prefix

Don't return bare IDs. Give them a short prefix that tells you what kind of thing the ID refers to.

```json
// ❌ avoid
{ "id": "abc123" }

// ✅ prefer
{ "id": "ord_abc123" }   // order
{ "id": "cus_abc123" }   // customer
{ "id": "prod_abc123" }  // product
```

This is the convention Stripe popularized, and it's now common in well-designed APIs.

**Why this matters:**

- **You can tell what an ID is just by looking at it.** When `ord_abc123` shows up in a log line, a support ticket, an error message, or a customer's screenshot, you immediately know it's an order — no need to ask, look it up, or guess.

- **It catches mix-up bugs early.** If a client accidentally passes a customer ID where an order ID was expected, your server can reject it with a clear message like *"expected an order ID (ord_*), got a customer ID (cus_*)"* — much better than a confusing 404.

- **You can encode environment too.** Some APIs use `ord_live_abc123` vs `ord_test_abc123` so a test ID can never be confused for a real one. That single convention prevents whole categories of "oops, I charged a real customer" mistakes.

- **It's friendlier for humans.** IDs end up in dashboards, chat messages, and emails. A readable, self-describing ID makes your whole product feel more polished.

**Important:** add prefixes **from day one**. Retrofitting them onto a shipped API is painful — every existing ID is already out there in client databases. Pick your prefix scheme early and stick with it forever.

**Rule of thumb:** 3–4 lowercase letters, an underscore, then the unique part. Keep it consistent across every resource type.

## Rule 10: Use a consistent error shape

Every endpoint that returns an error should return it in the same structure. Don't mix bare strings, ad-hoc objects, or different field names per endpoint.

```json
// ❌ avoid — different shape every time
"Card expired"
{ "msg": "Card expired" }
{ "errors": ["Card expired"] }

// ✅ prefer — same shape everywhere
{
  "error": {
    "type": "card_expired",
    "message": "The card has expired"
  }
}
```

**Always include two things:**

- **`type`** — a stable, machine-readable code (e.g. `card_expired`, `invalid_email`, `rate_limited`). This is what client code branches on.
- **`message`** — a human-readable description, suitable for logs or showing to a developer. This is *not* what code should check against — wording changes, codes don't.

**Why this matters:**

- Clients write error-handling code once, not per endpoint.
- If a client checks `error.type === "card_expired"`, you can reword the message anytime without breaking them. If they check `message === "Card expired"`, you can't.
- Monitoring and alerting work cleanly — you can graph "rate of `rate_limited` errors" or alert on specific types.

**For validation errors that affect multiple fields**, return a list:

```json
{
  "error": {
    "type": "validation_failed",
    "message": "Some fields are invalid",
    "fields": [
      { "field": "email", "type": "invalid_format", "message": "Not a valid email" },
      { "field": "age", "type": "out_of_range", "message": "Must be 18 or older" }
    ]
  }
}
```

**Match `type` to the HTTP status code.** A `400 Bad Request` with `type: "card_expired"` is confusing — the status and the type should describe the same kind of failure.

**Consider the standard:** [RFC 9457](https://www.rfc-editor.org/rfc/rfc9457) (`application/problem+json`) defines an off-the-shelf error format. It obsoletes [RFC 7807](https://www.rfc-editor.org/rfc/rfc7807) and is backward-compatible. Using it costs nothing and gives you tooling support and a spec to point at.

## Rule 11: Make unsafe operations idempotent

Networks fail. A client sends a request, the connection drops, and now the client doesn't know whether it went through. If it retries blindly, you can end up with two orders, two charges, or two emails sent.

The fix: give clients a way to retry safely.

**Which methods need this?**

| Method | Retry safe by default? |
|--------|------------------------|
| `GET`, `HEAD` | ✅ read-only, no state change |
| `PUT` | ✅ same value applied twice = same result |
| `DELETE` | ✅ once gone, it's gone |
| `PATCH` | ⚠️ depends — `{"name": "Bob"}` is, `{"counter": "+1"}` isn't |
| `POST` | ❌ every call creates a new thing |

The main one to worry about is **`POST`**.

**How to make `POST` safe to retry: the `Idempotency-Key` header.**

The client generates a unique key (a UUID works) and sends it with the request. The server stores the key → response for a while (usually 24h). If the same key comes in again, the server returns the cached response without re-running the operation.

```
POST /orders
Idempotency-Key: 7f3c9d2e-4a8b-11ee-be56-0242ac120002

{ "items": [...] }
```

Retry as many times as you want — the order only gets created once.

**Rule of thumb:** every `POST` that creates something or charges money should accept an `Idempotency-Key`. The day a payment retry double-charges a customer, you'll be very glad you added it.

## Rule 12: Use ISO8601 strings for timestamps

Return timestamps as ISO8601 strings, not epoch numbers.

```json
// ❌ avoid
{ "created_at": 1703157432 }

// ✅ prefer
{ "created_at": "2023-12-21T11:17:12Z" }
```

**Why:**

- **Readable in logs, debuggers, and bug reports.** No mental conversion every time you look at a response.
- **Every language has a parser** — `new Date(s)`, `datetime.fromisoformat(s)`, `time.Parse(...)`. The "epoch is easier" argument hasn't been true in decades.
- **Timezone is explicit.** The trailing `Z` means UTC. With epoch numbers, the UTC assumption is implicit and easy to get wrong.

**A few things to nail down:**

- **Always use UTC (`Z`)** on the wire. Let clients render in whatever timezone they want.
- **Pick a precision and stick with it** (milliseconds is the common choice).
- **Use `YYYY-MM-DD` for calendar dates** (birthdays, etc.) — not full timestamps. A birthday has no time-of-day, and treating it as midnight UTC causes off-by-one-day bugs depending on the viewer's timezone.
- **Don't return both formats.** Pick ISO8601, document it, move on.



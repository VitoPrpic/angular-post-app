# Blog API (.NET + Swagger)

API koristi SQLite bazu iz `database/blog.db` i automatski je stvara pri prvom
pokretanju.

## Pokretanje

```bash
dotnet run
```

Swagger se otvara na `http://localhost:5180/swagger`.

## Redoslijed u Swaggeru

1. Kreiraj administratora kroz `POST /api/admin-users`.
2. Po želji kreiraj kategoriju kroz `POST /api/categories`.
3. Dodaj post kroz `POST /api/posts` koristeći dobiveni `adminUserId`.

Postovi imaju ove metode:

- `GET /api/posts`
- `GET /api/posts/{id}`
- `POST /api/posts`
- `PUT /api/posts/{id}`
- `DELETE /api/posts/{id}`

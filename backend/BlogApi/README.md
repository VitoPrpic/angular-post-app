# Blog API (.NET + Swagger)

API koristi SQLite bazu iz `database/blog.db` i automatski je stvara pri prvom
pokretanju.

## Pokretanje

```bash
dotnet run
```

Swagger se otvara na `http://localhost:5180/swagger`.

## API ključ

Čitanje postova je javno, dok su administratorski podaci te dodavanje,
uređivanje i brisanje zaštićeni `Authorization` zaglavljem.

Lokalni razvojni ključ iz `launchSettings.json` je:

```text
lokalni-blog-kljuc
```

U Swaggeru klikni **Authorize** i upiši samo tu vrijednost, bez riječi
`Bearer`. Za Railway postavi varijablu `ApiKey` na vlastitu dugu i nasumičnu
vrijednost. Nemoj koristiti lokalni razvojni ključ u produkciji.

## Redoslijed u Swaggeru

1. Klikni **Authorize** i upiši API ključ.
2. Kreiraj administratora kroz `POST /api/admin-users`.
3. Po želji kreiraj kategoriju kroz `POST /api/categories`.
4. Dodaj post kroz `POST /api/posts` koristeći dobiveni `adminUserId`.

Kada koristiš Angular aplikaciju, ona pri prvom spajanju automatski kreira tehničkog
vlasnika bloga ako u bazi još nema administratora.

Postovi imaju ove metode:

- `GET /api/posts`
- `GET /api/posts/{id}`
- `POST /api/posts`
- `PUT /api/posts/{id}`
- `DELETE /api/posts/{id}`

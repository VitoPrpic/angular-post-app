# Angular aplikacija za postove

Jednostavna Angular aplikacija u kojoj se postovi dodaju kroz obrazac i prikazuju
kao responzivne kartice. Postovi se spremaju kroz ASP.NET Core API u SQLite bazu i
ostaju dostupni nakon osvježavanja stranice.

## Pokretanje

U prvom terminalu pokreni API:

```powershell
cd backend/BlogApi
dotnet run
```

U drugom terminalu pokreni Angular:

```powershell
pnpm install
pnpm start
```

Zatim otvori `http://localhost:4200`. Swagger je dostupan na
`http://localhost:5180/swagger`.

## Što je uključeno

- obrazac za naslov, tekst i do osam slika po postu
- reusable `PostCard` komponenta
- pregled, pojedinačno uklanjanje i smanjivanje JPG, PNG i WebP slika
- automatski datum i procjena vremena čitanja
- trenutno vrijeme u zaglavlju
- otvaranje cijelog posta klikom na karticu
- dodavanje, uređivanje i uklanjanje postova
- spremanje postova i slika u SQLite bazu preko .NET API-ja
- responzivan prikaz za mobitel i računalo

SQL shema za `AdminUsers`, `Posts` i `Categories` nalazi se u
[`database/schema.sql`](database/schema.sql).

ASP.NET Core API sa Swaggerom nalazi se u [`backend/BlogApi`](backend/BlogApi).

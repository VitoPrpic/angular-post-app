# Angular aplikacija za postove

Jednostavna Angular aplikacija u kojoj se postovi dodaju kroz obrazac i prikazuju
kao responzivne kartice. Dodani postovi čuvaju se u `localStorage` preglednika, pa
ostaju dostupni i nakon osvježavanja stranice.

## Pokretanje

```bash
pnpm install
pnpm start
```

Zatim otvori `http://localhost:4200`.

## Što je uključeno

- obrazac za naslov, tekst i do osam slika po postu
- reusable `PostCard` komponenta
- pregled, pojedinačno uklanjanje, smanjivanje i lokalno spremanje JPG, PNG i WebP slika
- automatski datum i procjena vremena čitanja
- trenutno vrijeme u zaglavlju
- otvaranje cijelog posta klikom na karticu
- dodavanje i uklanjanje kartica
- lokalno spremanje postova
- responzivan prikaz za mobitel i računalo

SQL shema za `AdminUsers`, `Posts` i `Categories` nalazi se u
[`database/schema.sql`](database/schema.sql).

ASP.NET Core API sa Swaggerom nalazi se u [`backend/BlogApi`](backend/BlogApi).

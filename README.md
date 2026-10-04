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

- obrazac za naslov, tekst i kategoriju posta
- reusable `PostCard` komponenta
- automatski datum i procjena vremena čitanja
- dodavanje i uklanjanje kartica
- lokalno spremanje postova
- responzivan prikaz za mobitel i računalo

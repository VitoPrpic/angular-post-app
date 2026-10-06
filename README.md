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

## Railway objava

Projekt se na Railwayu gradi iz korijenskog `Dockerfile`a. Angular i API rade na
istoj domeni, a Angular šalje zahtjeve na `/api`.

1. Prenesi cijeli projekt u GitHub repozitorij.
2. U Railwayu odaberi **GitHub Repository** i taj repozitorij.
3. Na Railway servisu dodaj **Volume** i postavi mount path na `/data`.
4. Kao health check path postavi `/health`.
5. U **Networking** odaberi **Generate Domain**.

Railway sam postavlja `PORT`, a aplikacija SQLite bazu automatski sprema u
`RAILWAY_VOLUME_MOUNT_PATH`, odnosno u priključeni Volume. Bez Volumea će se
podaci izgubiti nakon novog deploya.

**UPDATE**
STRANICA NA www.vitoprpic.blog

> Prije javne objave treba dodati prijavu/autorizaciju jer su trenutačno API
> rute za dodavanje, uređivanje i brisanje postova javno dostupne.

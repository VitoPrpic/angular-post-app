# Angular aplikacija za postove

Jednostavna Angular aplikacija u kojoj se postovi dodaju kroz obrazac i prikazuju
kao responzivne kartice. Postovi se spremaju kroz ASP.NET Core API u SQLite bazu i
ostaju dostupni nakon osvježavanja stranice.

Backend koristi odvojene `API`, `Application`, `Domain` i `Data` projekte, kao
višeslojna poslovna aplikacija. Rješenje se nalazi u `backend/BlogApi.sln`.

## Railway objava

Projekt se na Railwayu gradi iz korijenskog `Dockerfile`a. Angular i API rade na
istoj domeni, a Angular šalje zahtjeve na `/api`.

Railway sam postavlja `PORT`, a aplikacija SQLite bazu automatski sprema u
`RAILWAY_VOLUME_MOUNT_PATH`, odnosno u priključeni Volume. Bez Volumea će se
podaci izgubiti nakon novog deploya.

Za dodavanje, uređivanje ili brisanje posta na Railwayu treba postaviti varijablu
`ApiKey`. Ključ se upisuje u polje **Admin API ključ** i čuva samo u trenutnoj
kartici preglednika.

**STRANICA NA**
www.vitoprpic.blog

> Čitanje postova je javno, dok su dodavanje, uređivanje i brisanje zaštićeni
> API ključem.

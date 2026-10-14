# Angular aplikacija za postove

Jednostavna Angular aplikacija u kojoj se postovi dodaju kroz obrazac i prikazuju
kao responzivne kartice. Postovi se spremaju kroz ASP.NET Core API u SQLite bazu i
ostaju dostupni nakon osvježavanja stranice.

## Railway objava

Projekt se na Railwayu gradi iz korijenskog `Dockerfile`a. Angular i API rade na
istoj domeni, a Angular šalje zahtjeve na `/api`.

Railway sam postavlja `PORT`, a aplikacija SQLite bazu automatski sprema u
`RAILWAY_VOLUME_MOUNT_PATH`, odnosno u priključeni Volume. Bez Volumea će se
podaci izgubiti nakon novog deploya.

**STRANICA NA**
www.vitoprpic.blog

> Prije javne objave treba dodati prijavu/autorizaciju jer su trenutačno API
> rute za dodavanje, uređivanje i brisanje postova javno dostupne.

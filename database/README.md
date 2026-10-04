# Baza za blog

Datoteka `schema.sql` sadrži SQLite/Cloudflare D1-kompatibilnu strukturu s tri
tražene tablice:

- `AdminUsers` — administratorski račun; sprema se samo hash lozinke
- `Posts` — naslov, puni tekst, status objave i JSON popis slika
- `Categories` — neobavezne kategorije za buduću upotrebu

`Posts.category_id` je neobavezan, pa sadašnje sučelje može nastaviti raditi bez
kategorija. `Posts.image_urls` sadrži JSON polje, primjerice:

```json
["/uploads/putovanje-1.webp", "/uploads/putovanje-2.webp"]
```

U bazu se spremaju samo URL-ovi ili putanje slika. Same datoteke slika trebaju
biti spremljene u object storageu ili u backend direktoriju za učitane datoteke.

## Lokalno stvaranje baze

Ako je SQLite instaliran:

```bash
sqlite3 blog.db < schema.sql
```

Angular aplikacija ne smije se izravno spajati na SQL bazu. Za čitanje i pisanje
potreban je backend API; ova shema je spremna da ga koristi.

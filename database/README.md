# Jednostavna SQL baza

`schema.sql` sadrži tri tablice:

- `AdminUsers` za administratora
- `Posts` za postove i popis slika
- `Categories` za neobavezne kategorije

Više slika sprema se u `image_urls` kao JSON tekst:

```json
["slika1.webp", "slika2.webp"]
```

Bazu možeš napraviti naredbom:

```bash
sqlite3 blog.db < schema.sql
```

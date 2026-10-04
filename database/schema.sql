-- SQLite / Cloudflare D1 schema za blog aplikaciju.
-- Pokreni ovaj dokument nad praznom bazom.

PRAGMA foreign_keys = ON;

BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS "AdminUsers" (
    "id"            INTEGER PRIMARY KEY AUTOINCREMENT,
    "username"      TEXT NOT NULL COLLATE NOCASE,
    "email"         TEXT NOT NULL COLLATE NOCASE,
    "password_hash" TEXT NOT NULL,
    "display_name"  TEXT,
    "is_active"     INTEGER NOT NULL DEFAULT 1
                    CHECK ("is_active" IN (0, 1)),
    "created_at"    TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now')),
    "updated_at"    TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_admin_users_username"
ON "AdminUsers" ("username");

CREATE UNIQUE INDEX IF NOT EXISTS "idx_admin_users_email"
ON "AdminUsers" ("email");

CREATE TABLE IF NOT EXISTS "Categories" (
    "id"          INTEGER PRIMARY KEY AUTOINCREMENT,
    "name"        TEXT NOT NULL COLLATE NOCASE,
    "slug"        TEXT NOT NULL COLLATE NOCASE,
    "description" TEXT,
    "created_at"  TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now')),
    "updated_at"  TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now'))
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_categories_name"
ON "Categories" ("name");

CREATE UNIQUE INDEX IF NOT EXISTS "idx_categories_slug"
ON "Categories" ("slug");

CREATE TABLE IF NOT EXISTS "Posts" (
    "id"            INTEGER PRIMARY KEY AUTOINCREMENT,
    "admin_user_id" INTEGER NOT NULL,
    "category_id"   INTEGER,
    "title"         TEXT NOT NULL,
    "slug"          TEXT NOT NULL COLLATE NOCASE,
    "excerpt"       TEXT,
    "content"       TEXT NOT NULL,
    "image_urls"    TEXT NOT NULL DEFAULT '[]'
                    CHECK (json_valid("image_urls") AND json_type("image_urls") = 'array'),
    "status"        TEXT NOT NULL DEFAULT 'draft'
                    CHECK ("status" IN ('draft', 'published')),
    "published_at"  TEXT,
    "created_at"    TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now')),
    "updated_at"    TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ', 'now')),

    CONSTRAINT "fk_posts_admin_user"
        FOREIGN KEY ("admin_user_id")
        REFERENCES "AdminUsers" ("id")
        ON UPDATE CASCADE
        ON DELETE RESTRICT,

    CONSTRAINT "fk_posts_category"
        FOREIGN KEY ("category_id")
        REFERENCES "Categories" ("id")
        ON UPDATE CASCADE
        ON DELETE SET NULL,

    CONSTRAINT "chk_posts_published_at"
        CHECK ("status" <> 'published' OR "published_at" IS NOT NULL)
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_posts_slug"
ON "Posts" ("slug");

CREATE INDEX IF NOT EXISTS "idx_posts_admin_created_at"
ON "Posts" ("admin_user_id", "created_at" DESC);

CREATE INDEX IF NOT EXISTS "idx_posts_category_id"
ON "Posts" ("category_id");

CREATE INDEX IF NOT EXISTS "idx_posts_published"
ON "Posts" ("published_at" DESC)
WHERE "status" = 'published';

CREATE TRIGGER IF NOT EXISTS "trg_admin_users_updated_at"
AFTER UPDATE ON "AdminUsers"
FOR EACH ROW
WHEN NEW."updated_at" = OLD."updated_at"
BEGIN
    UPDATE "AdminUsers"
    SET "updated_at" = strftime('%Y-%m-%dT%H:%M:%fZ', 'now')
    WHERE "id" = NEW."id";
END;

CREATE TRIGGER IF NOT EXISTS "trg_categories_updated_at"
AFTER UPDATE ON "Categories"
FOR EACH ROW
WHEN NEW."updated_at" = OLD."updated_at"
BEGIN
    UPDATE "Categories"
    SET "updated_at" = strftime('%Y-%m-%dT%H:%M:%fZ', 'now')
    WHERE "id" = NEW."id";
END;

CREATE TRIGGER IF NOT EXISTS "trg_posts_updated_at"
AFTER UPDATE ON "Posts"
FOR EACH ROW
WHEN NEW."updated_at" = OLD."updated_at"
BEGIN
    UPDATE "Posts"
    SET "updated_at" = strftime('%Y-%m-%dT%H:%M:%fZ', 'now')
    WHERE "id" = NEW."id";
END;

COMMIT;

PRAGMA optimize;

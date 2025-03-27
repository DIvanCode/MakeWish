CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Users" (
    id varchar NOT NULL,
    email varchar NOT NULL,
    password_hash varchar NOT NULL,
    name varchar NOT NULL,
    surname varchar NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY (id)
);

CREATE UNIQUE INDEX "IX_Users_email" ON "Users" (email);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250327094530_AddUsersTable', '9.0.3');

CREATE TABLE "Friendships" (
    first_user varchar NOT NULL,
    second_user varchar NOT NULL,
    is_confirmed bool NOT NULL DEFAULT FALSE,
    CONSTRAINT "PK_Friendships" PRIMARY KEY (first_user, second_user),
    CONSTRAINT "FK_Friendships_Users_first_user" FOREIGN KEY (first_user) REFERENCES "Users" (id) ON DELETE CASCADE,
    CONSTRAINT "FK_Friendships_Users_second_user" FOREIGN KEY (second_user) REFERENCES "Users" (id) ON DELETE CASCADE
);

CREATE INDEX "IX_Friendships_first_user" ON "Friendships" (first_user);

CREATE INDEX "IX_Friendships_second_user" ON "Friendships" (second_user);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250327094618_AddFriendshipsTable', '9.0.3');

COMMIT;


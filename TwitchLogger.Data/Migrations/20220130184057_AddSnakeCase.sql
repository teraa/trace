-- BACKUP
-- /usr/bin/pg_dump -O twitchlogger | gzip > ~/downloads/twitchlogger.sql.gz

-- up
-- 1. add .UseSnakeCaseNamingConvention() in OnConfiguring
-- 2. dotnet-ef migrations add AddSnakeCase
-- 3. run SQL below
-- 4. dotnet-ef database update
begin;
ALTER TABLE "__EFMigrationsHistory" RENAME COLUMN "MigrationId" TO "migration_id";
ALTER TABLE "__EFMigrationsHistory" RENAME COLUMN "ProductVersion" TO "product_version";
commit;

-- down (not exactly reverse order)
-- 1. dotnet-ef database update <migration name before AddSnakeCase>
-- 2. run SQL below
-- 3. remove .UseSnakeCaseNamingConvention() from OnConfiguring
-- 4. dotnet-ef migrations remove
begin;
ALTER TABLE "__EFMigrationsHistory" RENAME COLUMN "migration_id" TO "MigrationId";
ALTER TABLE "__EFMigrationsHistory" RENAME COLUMN "product_version" TO "ProductVersion";
commit;

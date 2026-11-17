# AroundMe — Deliverable 2 (MVC + ASP.NET Core Identity)

## What's included
- D1 frontend migrated to MVC (`HomeController`, `PlacesController`, `AccountController` and Razor Views).
- ASP.NET Core Identity registration/login/logout with EF Core SQL Server in **AroundMeIdentityDB**.
- Existing D1 places, categories, search, filters, and add-place flow in **AroundMeDB**.
- Preserved D1 CSS/Bootstrap and uploads.

## Setup on Windows with Visual Studio 2022 / .NET 10
1. Open `AroundMe.slnx`. Install .NET 10 SDK if prompted.
2. Ensure SQL Server LocalDB is installed.
3. **Important:** D1's `AroundMeDB` database and tables (`Users`, `Categories`, `Places`, `Likes`) must already exist. The ZIP did not include a database creation script or backup. Restore your D1 database before running. You may change the connection strings in `AroundMe/appsettings.json`.
4. Restore NuGet packages and run the project. The separate Identity database (`AroundMeIdentityDB`) is automatically created on first run using `EnsureCreated`.
5. Register a **new email/username**. Existing D1 users are not silently migrated: D1 used BCrypt and the new project uses ASP.NET Core Identity hashes. New registrations create a linked record in D1's Users table so posts work.
6. Test register, login, logout, category filters, search and Share a Place with image upload.

## Known limitations
- Existing D1 accounts require a deliberate password migration/reset before they can log in with Identity. Use a fresh account for D2 demonstration.
- Original D1 cards linked to `/PlaceDetails`, but D1 ZIP had no such page. D2 does not invent a details page; cards link back to Home.
- D1 ZIP does not include SQL schema/data; database must be supplied separately.
- Identity database uses EnsureCreated for this coursework demo; production deployments should use EF migrations.
- D2 was prepared without a local .NET SDK/SQL Server runtime and therefore has **not** been compiled or integration-tested here.

## GitHub submission
From a *clone of your existing repository*, copy the contents of this ZIP over the tracked project (retain the clone's `.git` directory), then run:

```powershell
git status
git add -A
git commit -m "YYYY-MM-DD HH:mm D2 submission"
git push origin HEAD
```

Replace the timestamp with your actual local submission time. Confirm the commit on GitHub. Do not upload passwords, real connection credentials or `.vs/bin/obj` folders.

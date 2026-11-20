# AroundMe – D3 (MVC + AJAX + Dapper + Repository + Identity)

This version continues D2; **do not replace your existing Git repository history**.

## Requirements implemented

- AJAX: `Views/Home/Index.cshtml` and `wwwroot/js/places-ajax.js` call `Home/SearchResults` using `$.ajax({ type: "GET" })`. The `_PlaceResults.cshtml` partial is replaced without refreshing the page.
- Dapper: `Repositories/*Repository.cs` use `QueryAsync` and `ExecuteScalarAsync` with parameterized SQL.
- Repository + interface + DI: `IPlaceRepository`, `ICategoryRepository`, `IStatsRepository`, `ILegacyUserRepository` and corresponding implementations are registered in `Program.cs`.
- Identity customization: `ApplicationUser : IdentityUser` has `FullName`, `City` and `LegacyUserId`. Register form includes optional City.
- Layout sections: `_Layout.cshtml` renders `Styles`, `PageBanner`, and `Scripts`; Privacy page uses the first two, Home uses Scripts.
- Privacy Policy: `/Home/Privacy` is linked in footer. This is coursework sample text, not legal advice.
- IIS deployment: instructions below. **Actual IIS deployment must be completed and tested on your Windows computer.**

## IMPORTANT: existing D2 database

The original D2 code used `EnsureCreated()` for the Identity database. This does **not** add new columns to an existing database. Before running D3 with an existing `AroundMeIdentityDB`, execute `AroundMe/Database/D3_Identity_AddCity.sql` in SSMS or SQL Server Object Explorer. Back up first. New databases are created with the City column automatically.

The original `AroundMeDB` and `AroundMeIdentityDB` must be accessible. Adjust both connection strings in `appsettings.json` for your own SQL Server. Do not commit secrets. The D1 legacy Users table remains because Places.UserId uses its integer keys.

## Local test checklist

1. Open `AroundMe.slnx` in Visual Studio 2022 with .NET 10 support.
2. Restore NuGet packages (including Dapper) and Build Solution.
3. Check Home page, search and category filter; verify **Network** shows `GET /Home/SearchResults` and the browser does not reload.
4. Register a new account with City; check `AspNetUsers.City` and the legacy `Users` row.
5. Log in, create a place with a photo, verify the place appears in Home.
6. Check `/Home/Privacy`, Login, Logout, and error handling.

## Local IIS deployment (Windows)

1. Enable **Internet Information Services** in Windows Features, including IIS Management Console and World Wide Web Services.
2. Install the matching **.NET 10 ASP.NET Core Hosting Bundle** from Microsoft. Restart IIS (`iisreset`) after installation.
3. In Visual Studio, right-click AroundMe → **Publish** → **Folder**; choose a local publish folder such as `C:\inetpub\AroundMe` (or publish elsewhere and copy the output there).
4. Open IIS Manager → Application Pools → Add Application Pool, name `AroundMePool`, select **No Managed Code** and Integrated pipeline.
5. IIS Manager → Sites → Add Website. Set physical path to the **published output folder**, select `AroundMePool`, and an unused local port such as 8085. Bind HTTPS for non-local deployments.
6. Give the application pool identity read/execute permissions on the publish directory and write permission to `wwwroot\Uploads`. Do not give broad write permissions elsewhere.
7. **LocalDB is not recommended under IIS**. Use SQL Server Express/Developer, create/restore both databases, configure connection strings securely, and grant the IIS app pool's account appropriate SQL permissions. Back up the databases.
8. Open `http://localhost:8085` and test Home, AJAX, Register, Login, Add Place, and Privacy. Verify the app pool and Event Viewer logs if errors occur.
9. Do not publish a development-only connection string or secrets to a public server. Configure HTTPS and production security before exposing the app outside localhost.

## GitHub submission

From your **existing cloned repository** folder, copy the D3 files over the matching project files without deleting `.git`. Review changes and secrets first. Then:

```powershell
git status
git add .
git diff --cached --stat
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm"
git commit -m "$timestamp D3 submission"
git push origin master
```

Do not push before local build, database migration, and IIS test.

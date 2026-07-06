# SummerBot 

A Discord bot for the **Top.gg Summer Bot Jam 2026** - built with C#, DSharpPlus, and SQLite.

## Features
- **Summer Bucket List** - Add, view, complete, remove, and track stats on your personal summer bucket list items

## Setup
1. **Restore packages**
   ```powershell
   dotnet restore
   ```
2. **Configure** - copy `SummerBot.Core/appsettings.TEMPLATE.json` to `appsettings.json` and fill in `Discord:Token` and `Database:ConnectionString`
3. **Run**
   ```powershell
   dotnet run --project SUmmerBot.Core
   ```
Migrations run automatically on startup.

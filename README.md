# SummerBot

A Discord bot for the **Top.gg Summer Bot Jam 2026** - built with C#, DSharpPlus, and SQLite.

## Features

- **Summer Bucket List** - Add, view (paginated), complete, remove, and track stats on your personal summer bucket list items
- **Weather** - Current weather conditions via OpenWeatherMap (temperature, humidity, wind speed, description)
- **Photo Contest** - Submit, browse, and rank summer photos
  - `/photo_submit` - Upload a `.png`/`.jpeg`/`.jpg` with an optional description
  - `/photo_list` - Paginated gallery of all submissions with vote counts and jump links
  - `/photo_leaderboard` - Top 10 ranked by net upvotes
  - `/photo_remove` - (Manage Guild) Delete a specific submission
  - `/photo_reset` - (Manage Guild) Wipe all submissions and votes

## Setup
1. **Restore packages**
   ```powershell
   dotnet restore
   ```
2. **Configure** - copy `SummerBot.Core/appsettings.TEMPLATE.json` to `appsettings.json` and fill in `Discord:Token`, `Database:ConnectionString`, and `OpenWeatherMap:ApiKey`
3. **Run**
   ```powershell
   dotnet run --project SUmmerBot.Core
   ```
Migrations run automatically on startup.

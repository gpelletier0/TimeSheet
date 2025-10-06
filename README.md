# TimeSheet (MAUI)

A cross‑platform Time Sheet application built with .NET MAUI. It lets you manage Clients, Projects, and Timesheets, with local persistence via SQLite. The app uses MVVM (CommunityToolkit.MVVM), DI, and a small repository/specification pattern over SQLite.

Note: This is a personal project, built for my own workflow. Expect rough edges. It has only been tested on a Windows platform, Android to be implemented at a later date.

## Overview
- Platforms: Android, Windows
- UI framework: .NET MAUI
- Patterns & Libraries:
  - MVVM with CommunityToolkit.MVVM
  - CommunityToolkit.Maui UI helpers
  - SQLite (sqlite-net-pcl) for local storage
  - Repository + Specification over SQLite
  - Mapster for DTO/entity mapping
  - Dependency Injection
- Features (as implemented):
  - Clients: list, filter, create/edit, details
  - Projects: list, filter, create/edit, details
  - Timesheets: list, filter, create/edit
  - Built-in status catalogue for timesheets (Opened, Invoiced, Paid, Voided)

## Tech Stack
- Language: C# 13 / .NET 9
- Framework: .NET MAUI 9
- Key packages (see TimeSheet/TimeSheet.csproj):
  - CommunityToolkit.Maui 12.2.0
  - CommunityToolkit.Mvvm 8.4.0
  - Microsoft.Maui.Controls 9.0.110
  - sqlite-net-pcl 1.9.172
  - Mapster 7.4.0 (+ Mapster.DependencyInjection)

## Entry points and architecture
- App startup: TimeSheet/MauiProgram.cs → CreateMauiApp builds the DI container, registers configs, and returns the MauiApp.
- Platforms call CreateMauiApp from:
  - Platforms/Android/MainApplication.cs
  - Platforms/Windows/App.xaml.cs
- Shell: AppShell (AppShell.xaml + AppShell.xaml.cs) initializes navigation and routes are auto-registered.
- DI/Configuration:
  - Configs/DatabaseConfig.cs: registers DatabaseService (SQLite), Repository<>, and DataSeederService
  - Configs/ViewModelConfig.cs: auto-registers all ObservableViewModel and ObservableValidatorViewModel subclasses as transient
  - Configs/PagesConfig.cs: auto-registers all ContentPage subclasses as transient and registers Shell routes
- Data access:
  - Services/DatabaseService.cs: owns SQLiteAsyncConnection; creates tables; enables foreign keys; seeds status table
  - Data/Repository.cs: typed repository with Mapster projections for DTOs
  - Specifications/*: query helpers that produce SQL and parameters

## Requirements
- OS: 
  - Windows 11 for Windows desktop and Android development
- .NET SDK 9.x
- .NET MAUI workloads for relevant platforms:
  - dotnet workload install maui
  - dotnet workload install android

## Setup
1. Clone the repository
   git clone <your-fork-or-origin-url>
   cd TimeSheet

2. Install workloads (only once per machine)
   dotnet workload install maui android

3. Restore packages
   dotnet restore TimeSheet/TimeSheet.csproj

4. Build
   dotnet build TimeSheet/TimeSheet.csproj -c Debug

## Data
- The app stores data in a local SQLite database at: %LocalAppData%\User Name\com.guillaumepelletier.timesheet\Data\TimeSheet.db3 (created on first run).
- Timesheet statuses are (re)upserted on startup of DatabaseService.

## Environment variables / Configuration
- No external environment variables are required for local runs.
- Database name is currently hardcoded to "TimeSheet.db3" in Configs/DatabaseConfig.cs.

## Tests
- TODO

## Project structure (high-level)
- TimeSheet.sln
- TimeSheet/ (single MAUI project)
  - Attributes/
  - Behaviors/
  - Configs/
    - DatabaseConfig.cs (SQLite/DI)
    - PagesConfig.cs (pages + Shell routes)
    - ViewModelConfig.cs (VM DI)
  - Controls/
  - Converters/
  - Data/ (Repository over SQLite)
  - Extensions/
  - Interfaces/ (IRepository, IDatabaseService, IDataSeederService, IConfigStrategy, etc.)
  - Models/
    - Dtos/
    - Entities/
  - Platforms/ (Android, Windows)
  - Resources/ (AppIcon, Fonts, Images, Raw, Splash)
  - Services/ (DatabaseService, DataSeederService, etc.)
  - Specifications/ (query helpers)
  - ViewModels/ (Main, Clients, Projects, Timesheets)
  - Views/ (XAML pages for Clients, Projects, Timesheets, Main)
  - MauiProgram.cs (app builder/DI entry)
  - App.xaml, App.xaml.cs (app)
  - AppShell.xaml, AppShell.xaml.cs (navigation)

## Troubleshooting
- Ensure the required workloads are installed (dotnet workload list)
- For Windows, ensure Windows 11 and MAUI dependencies are available

## Roadmap
- Invoicing to PDF, CSV, Excel
- Database encryption
- Database export
- Unit Tests
- Android compatibility
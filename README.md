# 🧸 CrochetToysShop

A web shop for handmade crochet toys, built with ASP.NET Core MVC. 
Customers can browse the catalogue and place orders without registration. 
Admins manage the products and track incoming orders.

---

## 🔧 Requirements

- .NET 8 SDK
- Docker Desktop
- Visual Studio 2022 or later

---

## 🚀 How to Run

### 1. Start the database

From the root folder (where `docker-compose.yml` is):

```bash
docker-compose up -d
```

This starts SQL Server 2022 on port 1433. The password matches the one in `appsettings.json` — no changes needed.

### 2. Run the app

Open the solution in Visual Studio and press **F5**, or from the terminal:

```bash
cd CrochetToysShop
dotnet run
```

Migrations run automatically on startup. Categories and the admin account are seeded on first run — no manual setup needed.

### 3. Open in browser

Go to `https://localhost:<port>` shown in the terminal or Visual Studio output.

---

## 🔑 Admin Account

- **Email:** yoanna@admin.com  
- **Password:** admin1

---

## 🛠️ Tech Stack

- ASP.NET Core 8 MVC
- Entity Framework Core 8 + SQL Server
- ASP.NET Core Identity
- Bootstrap 5

---

## 📁 Structure

```
Controllers/   – request handling
Services/      – business logic
Data/          – DbContext and migrations
Models/        – entities and view models
Views/         – Razor pages
wwwroot/       – CSS, JS, images
```
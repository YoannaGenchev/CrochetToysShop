# 🧶 CrochetToysShop — Handmade Toys Store Platform

A web application for browsing, managing, and ordering handmade crochet toys, combined with a learning module for crochet courses — built with **ASP.NET Core MVC**, **Entity Framework Core**, and **ASP.NET Identity**.

The goal of the project is to simulate a real-world online store with **role-based access**, **structured data management**, and **user interaction flows**.

---

# 📑 Contents
- [Project at a glance](#-project-at-a-glance)
- [Key features](#-key-features)
- [Architecture](#-architecture)
- [Access & Roles](#-access--roles)
- [Database model](#-database-model)
- [UI structure](#-ui-structure)
- [Tech stack](#-tech-stack)
- [Getting started](#-getting-started)
- [EF Core migrations](#-ef-core-migrations)
- [Testing](#-testing)
- [My main achievements](#-my-main-achievements)
- [What I learned](#-what-i-learned)
- [Future improvements](#-future-improvements)

---

# 🚀 Project at a glance

**CrochetToysShop** is a simplified e-commerce and learning platform that allows users to:

- 🧸 Browse crochet toys by category  
- 🔎 Search and filter toys  
- 🛒 Place toy orders  
- 🎓 Enroll in crochet courses  
- ⚙️ Manage content through an admin panel  

---

# ⭐ Key features

## 🛍️ Toys
- Full CRUD for administrators  
- Public browsing  
- Category filtering  
- Search  
- Availability tracking  

## 📦 Orders
- Place orders  
- Admin manages orders  

## 🎓 Courses
- Browse courses  
- User-only enrollment  

## 🔐 Authentication
- ASP.NET Identity  
- Roles: Anonymous / User / Admin  

---

# 🏗️ Architecture

- Web (Controllers + Views)  
- Services (Business logic)  
- Data (EF Core)  

Flow:
Controller → Service → Database → View

---

# 👥 Roles

- Anonymous → browsing  
- User → enroll  
- Admin → manage  

---

# 🛠️ Tech stack
- ASP.NET Core MVC  
- EF Core  
- SQL Server  
- Identity  
- Bootstrap  

---

# ⚙️ Setup

```bash
dotnet ef database update
dotnet run
```

---

# 🧪 Testing
Unit tests for services.

---

# 🏆 Achievements
- Layered architecture  
- Role-based access  
- CRUD + search + filtering  

---

# 🔮 Future improvements
- Cart  
- Payments  
- Better UI  

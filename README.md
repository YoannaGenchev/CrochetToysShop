# 🧶 CrochetToysShop — Handmade Toys Store Platform

A full-stack ASP.NET Core MVC web application for browsing, managing, and ordering handmade crochet toys, combined with a learning module for crochet courses.

Built with **ASP.NET Core MVC**, **Entity Framework Core**, and **ASP.NET Identity**.

The project simulates a real-world online store with **role-based access**, **structured data management**, and **user interaction flows**.

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
- Public browsing  
- Category filtering  
- Search functionality  
- Availability tracking  
- Full CRUD for administrators  

## 📦 Orders
- Visitors can submit orders  
- Admin manages order lifecycle  

## 🎓 Courses
- Browse available courses  
- Enrollment available for registered users  

## 🔐 Authentication & Authorization
- ASP.NET Core Identity  
- Roles: Anonymous / User / Admin  
- Automatic role assignment for new users  

---

# 🏗️ Architecture

The project follows a **layered architecture**:

- **Web Layer** → Controllers + Views  
- **Service Layer** → Business logic  
- **Data Layer** → EF Core + Database  

Flow:
Controller → Service → Database → View

---

# 👥 Access & Roles

- **Anonymous users**
  - Browse toys and courses  

- **Registered users**
  - Enroll in courses  
  - Interact with user-only features  

- **Admin**
  - Manage toys (Create/Edit/Delete)  
  - Manage orders  

---
### Admin
Email: yoanna@admin.com  
Password: Admin1 (default during development)

---

# 🛠️ Tech stack
- ASP.NET Core MVC (.NET 8)  
- Entity Framework Core  
- SQL Server / LocalDB  
- ASP.NET Core Identity  
- Razor Views & Razor Pages  
- Bootstrap  

---

# ⚙️ Setup

```bash
##1. Apply database migrations
dotnet ef database update
##2. Run the application
dotnet run
```

---

# 🧪 Testing
Unit tests for service layer
Integration tests for key flows

---

# 🏆 Achievements
- Implemented clean layered architecture
- Built role-based authorization system
- Designed full CRUD + filtering + search flows
- Integrated ASP.NET Identity with custom role logic
- Implemented safe database seeding
- Improved UI consistency and layout

---

# 🔮 Future improvements
- Shopping cart and checkout system
- Payment integration
- Owner-based toy management
- UI/UX enhancements
- Deployment and CI/CD pipeline 

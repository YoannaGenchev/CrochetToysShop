# CrochetToysShop

CrochetToysShop is an ASP.NET Core MVC final project focused on two clearly separated domains:

- **Handmade toys catalog and single-item order requests**
- **Crochet learning courses with user enrollment**

The project is built with a layered architecture, role-based access, database seeding, and automated tests.

## 1) Project overview

CrochetToysShop is a web platform where users can discover handmade crochet toys and course offerings.

- Toys are presented through a catalog with filtering/search and details pages.
- Toy ordering is implemented as a **single-item direct order/request flow** (no shopping cart in current scope).
- Courses are presented as a separate enrollment domain.

## 2) Target users

- **Anonymous visitors**
  - Browse toys and courses
  - Open toy/course details
  - Submit toy order requests
- **Registered users (User role)**
  - Enroll in courses
  - View enrolled courses in **My Courses**
- **Administrators (Admin role)**
  - Access custom **Admin Area**
  - Manage toy create/edit/delete
  - Manage order lifecycle (mark completed)

## 3) Core features

### Toys
- Public catalog with pagination
- Category filtering and search
- Availability tracking
- Details page for each toy
- Admin create/edit/delete management

### Orders (separate from courses)
- Single-item direct order/request for a selected toy
- Validation on order form
- Admin order list and status update (new/completed)

### Courses (separate from toy ordering)
- Public course list with filtering and pagination
- Course details with difficulty/duration/price/capacity
- Enrollment flow for authenticated users in User role
- **My Courses** page showing current user enrollments

## 4) Architecture and project structure

The solution uses a layered project structure:

- `CrochetToysShop.Web` - MVC UI layer (controllers, views, startup)
- `CrochetToysShop.Services.Core` - business logic/services
- `CrochetToysShop.Services.Models` - service DTO/models
- `CrochetToysShop.Data` - EF Core DbContext, migrations, seeding
- `CrochetToysShop.Data.Models` - entity models
- `CrochetToysShop.Web.ViewModels` - MVC view models
- `CrochetToysShop.Web.Infrastructure` - web extensions/helpers
- `CrochetToysShop.Services.Tests` - service-layer test suite
- `CrochetToysShop.IntegrationTests` - minimal HTTP integration tests

Request flow:

`Controller -> Service -> DbContext/EF Core -> ViewModel/View`

## 5) Domain model summary

Main entities include:

- `Toy`
- `Category`
- `Order`
- `Course`
- `Enrollment`
- `OrderRequest` (legacy model kept in data model set)

Current active flows use `Order` for toy orders and `Enrollment` for course participation.

## 6) Roles and authorization

Authentication/authorization is based on ASP.NET Core Identity with roles:

- `User`
- `Admin`

Authorization is applied on protected actions and area controllers.

## 🔐 Demo Credentials

A default administrator account may be seeded on application startup:

**Admin account:**
- Email: yoanna@admin.com

Admin password seeding behavior:

- In development, the password can be provided from `appsettings.Development.json`.
- For non-development environments, provide `AdminSeed:Password` via environment/user secrets.
- Example (PowerShell):

```powershell
$env:AdminSeed__Password="YourStrongAdminPassword"
```

> If the account is not available, ensure the database is created and seeding has been executed.

## 7) Admin Area

The project includes a real custom MVC area under:

- `Areas/Admin/Controllers`
- `Areas/Admin/Views`

Implemented Admin Area capabilities:

- Dashboard landing page
- Orders management (index + mark completed)
- Toys management actions (create/edit/delete)

Admin behavior clarification:

- Administrators manage platform data and operations (orders, toys, course enrollments overview).
- Administrators cannot enroll in courses as learners.

Admin entry is available from the main navigation for Admin users.

## 8) Security and validation

- **Identity-based authentication** with roles
- **Authorization attributes** on protected routes/actions
- **Global antiforgery validation** via MVC filter + form tokens
- **Server-side validation** with DataAnnotations and `ModelState`
- **Client-side validation** through validation scripts/Tag Helpers
- **XSS-safe Razor rendering** through default HTML encoding in views
- **EF Core LINQ usage** (no raw SQL in application flow)
- Custom error pages for:
  - `404`
  - `500`

### Security summary

- Antiforgery protection is applied globally for state-changing requests.
- Role-based authorization separates public, user, and admin-only actions.
- Validation is applied on both client side and server side.
- Razor views rely on encoded output by default, supporting XSS-safe rendering.

## 9) Seeding

Database seeding includes:

- Roles (`Admin`, `User`)
- Seed admin account
- Categories
- Toys
- Courses

Seeder logic includes duplicate-safe behavior and seeded data updates for maintained records.

## 10) Database and setup instructions

### Prerequisites

- .NET SDK 8.0+
- SQL Server (LocalDB supported)
- EF Core CLI tools (`dotnet-ef`)

### Connection string

Default connection is configured in:

- `CrochetToysShop.Web/appsettings.json`

Example uses LocalDB:

- `Server=(localdb)\MSSQLLocalDB;Database=CrochetToysShop;...`

## 11) Migrations and run instructions

From the solution folder containing `CrochetToysShopSolution.sln`:

```bash
dotnet restore
dotnet ef database update --project CrochetToysShop.Data --startup-project CrochetToysShop.Web
dotnet run --project CrochetToysShop.Web
```

On startup, migrations and seeding are applied by the app initialization pipeline.

## 12) Testing

### Test projects

- `CrochetToysShop.Services.Tests`
- `CrochetToysShop.IntegrationTests`

### Current verified status

- **Service tests:** 63 passed
- **Integration tests:** 9 passed

### Run tests

```bash
dotnet test
dotnet test CrochetToysShop.Services.Tests/CrochetToysShop.Services.Tests.csproj
dotnet test CrochetToysShop.IntegrationTests/CrochetToysShop.IntegrationTests.csproj
```

## 13) Coverage evidence

Coverage is collected with the already configured `coverlet.collector`.

Command used:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Verified service-layer coverage for `CrochetToysShop.Services.Core`:

- **Line coverage:** ~97%
- **Branch coverage:** ~74%

This reflects strong business-logic test coverage in the service layer.

## 14) Current scope and intentional limitations

To keep scope focused and stable for final submission:

- Toy ordering is implemented as a **single-item direct order/request flow**.
- The project **does not** implement a full shopping cart/checkout pipeline in the current version.
- Courses are a separate enrollment domain and are not mixed with toy ordering.

## 15) Optional future improvements

- Full shopping cart and checkout flow
- Extended admin analytics/dashboard widgets
- Additional integration tests for authenticated role scenarios
- CI/CD and deployment pipeline

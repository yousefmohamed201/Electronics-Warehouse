# Electronics Warehouse Management System

🎓 **Graduation Project**

Electronics Warehouse Management System is a web-based graduation project developed as part of the .NET track at ITI (Information Technology Institute) – Menoufia.

The system is designed to manage and organize the operations of an electronics warehouse, including products, inventory, suppliers, purchases, customers, orders, and warehouse activities.

---

## 🎯 Project Objectives

- Manage electronic products and categories.
- Track inventory and stock levels.
- Manage warehouses and product locations.
- Manage suppliers and purchasing operations.
- Manage customers and orders.
- Track product serial numbers.
- Handle returns and warranty information.
- Provide reports and statistics.
- Implement authentication and role-based authorization.
- Build a maintainable and scalable .NET-based system.

---

## 👥 Team

| Member | Branch |
| --- | --- |
| Yousef | `yousef` |
| Almaghraby | `almaghraby` |
| Ibrahim | `ibrahim` |
| Tayel | `tayel` |
| Mohamed | `mohamed` |

---

## 🛠️ Technology Stack

### Backend & Core
- ASP.NET Core MVC & Web API
- Entity Framework Core
- C#
- SQL Server
- ASP.NET Core Identity

### Frontend
- Razor Views
- Bootstrap 5
- Bootstrap Icons

### Development Tools
- Visual Studio
- Git & GitHub

---

## 🌿 Branching Strategy

The `main` branch represents the stable version of the project.
Each team member has a dedicated branch for development:

```
main
├── yousef
├── almaghraby
├── ibrahim
├── tayel
└── mohamed
```

---

## 📦 Products & Categories Implementation (Mohamed)

### Overview
This implementation completes the foundation and CRUD functionality for the **Categories** and **Products** modules in the Electronics Warehouse project.

---

### Phase 1 – Data Models & Validation

#### Category Model (`Category.cs`)
- Added `[Required]`, `[StringLength(100)]`, and `[Display]` data annotations.
- Added `[ValidateNever]` to navigation properties (`Products`) to prevent MVC binding validation errors.

#### Product Model (`Product.cs`)
- Added `[Required]`, `[StringLength]`, `[Range]`, `[Column]`, and `[Display]` data annotations.
- Added `[ValidateNever]` to navigation properties (`Category`, `PurchaseItems`, `SaleItems`).

---

### Phase 2 – Categories CRUD (`CategoriesController`)

#### Implemented Features
- Display the number of products associated with each category on Index and Details pages.
- Prevent duplicate category names during creation and editing.
- Deletion Guard: Prevent deleting categories with linked products, displaying warning alerts.

---

### Phase 2 – Products CRUD (`ProductsController`)

#### Implemented Features
- Added SKU uniqueness validation on Create and Edit actions.
- Included Category details in Index, Details, and Delete views.
- Deletion Guard: Prevent deleting products linked to purchase or sale history.
- Added low-stock alert badges (`StockQuantity <= LowStockThreshold`).

---

### User Interface & Navigation
- Enhanced Views (`Index`, `Create`, `Edit`, `Details`, `Delete`) for both Categories and Products with modern Bootstrap cards, icons, and alerts.
- Updated `Shared/_Layout.cshtml` header navbar with direct navigation links.

---

## 🚀 Build & Verification

```bash
dotnet build
```

- **0 Errors**, **0 Warnings** related to Categories and Products CRUD.


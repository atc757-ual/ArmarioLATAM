
# 📄 README.md (ARCHIVO FINAL) - ArmarioLATAM

# 🧥 ArmarioLATAM – Full Stack E-commerce & Logística LATAM

**ArmarioLATAM** es una plataforma **e-commerce & logística** para **LATAM**, enfocada en **gestión de inventario, catálogo de productos, usuarios y pedidos**. Construida con **Clean Architecture**, **Blazor Server**, **ASP.NET Core API** y **EF Core**. [https://github.com/atc757-ual/ArmarioLATAM]

---

## 🧰 Tecnologías

![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)
![Blazor](https://img.shields.io/badge/Blazor-Server-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET-Core-orange)
![SQL Server](https://img.shields.io/badge/SQL-Server-brightgreen)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-yellow)

---

## 🏗️ Arquitectura (Clean Architecture)

```
ArmarioLATAM
├── Client (Blazor Server)      # UI Razor Components
├── API (ASP.NET Core Web API)  # REST Controllers
│   ├── Controllers
│   ├── Application (DTOs + Services)
│   ├── Domain (Entities + Interfaces)
│   └── Infrastructure (EF Core + SQL Server)
├── Shared (DTOs compartidos)
├── Data (DbContext + Migrations)
└── Tests (xUnit + Moq)
```

**Lenguajes**: HTML 45.9% | C# 31.0% | CSS 23.1% [attached_file:1]

---

## 🚀 Stack Tecnológico

| Capa | Tecnología | Patrón |
|------|------------|--------|
| UI | Blazor Server | Razor Components |
| API | ASP.NET Core 8 | REST + MVC |
| Persistencia | SQL Server + EF Core | Code-First Migrations |
| Seguridad | JWT Bearer | Roles & Claims |
| Documentación | Swagger OpenAPI | Auto-generado |
| Testing | bUnit  | TDD Clean |

---

## 📦 Instalación Rápida

### 🚀 Backend API + Migraciones

```bash
git clone https://github.com/atc757-ual/ArmarioLATAM.git
cd ArmarioLATAM

# Restaurar + Migraciones EF Core
dotnet restore
dotnet ef migrations add InitialCreate --project Data --startup-project API -v
dotnet ef database update --project Data --startup-project API --verbose

# Ejecutar API
cd API
dotnet run

# Swagger: https://localhost:7001/swagger
```

### 🌐 Frontend Blazor Server

```bash
cd Client
dotnet restore
dotnet run

# App: https://localhost:5001
```

**Connection String Ejemplo** (appsettings.json):
```json
"DefaultConnection": "Server=db;Database=ArmarioLATAM;User=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;"
```

---

## 🔗 Endpoints Principales

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/products` | Catálogo completo | ❌ |
| GET | `/api/products/{id}` | Detalle producto | ❌ |
| POST | `/api/products` | Crear/editar | ✅ Admin |
| POST | `/api/auth/login` | Autenticación JWT | ❌ |
| POST | `/api/auth/register` | Nuevo usuario | ❌ |
| POST | `/api/cart/add` | Añadir al carrito | ✅ |
| GET | `/api/orders` | Mis pedidos | ✅ |

**Login Ejemplo**:
```bash
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@armariolatam.com","password":"Admin123!"}'
```
Respuesta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "guid-refresh",
  "user": {
    "id": 1,
    "email": "admin@armariolatam.com",
    "role": "Admin"
  }
}
```

---

## 🛡️ Seguridad Implementada

- **JWT Bearer Authentication** (Access: 15min | Refresh: 7 días)
- **Roles**: Admin, User, Guest
- **Claims Custom**: userId, email, permissions
- **CORS** Blazor + Swagger
- **HTTPS** enforced en producción
- **Rate Limiting** contra brute-force

---

## 🗄️ Base de Datos - Esquema

**Entidades Clave**:
- **Products**: Id, Name, Price, Stock, Category, Status
- **Users**: Id, Email, PasswordHash, Role, CreatedAt
- **Orders**: Id, UserId, Total, Status (Pending/Shipped), OrderItems[]
- **OrderItems**: Quantity, ProductId, UnitPrice
- **Categories**: Id, Name, Description

**Migraciones EF Core** 100% versionadas y seguras.

---

## 🧪 Testing & Calidad

```bash
# Todos los tests
dotnet test --collect:"XPlat Code Coverage"

# Blazor tests
dotnet test Client.Tests

# API Integration
dotnet test API.Tests
```

- **Cobertura**: >85% (xUnit, Moq, EF InMemory)
- **bUnit** para Blazor components
- **Swagger** docs auto-generados

---
- **Variables Ambiente**:
   ```
   ConnectionStrings__DefaultConnection=...
   JWT__Key=32+chars-super-secret-key-2026
   AzureAd__Instance=https://login.microsoftonline.com/...
   ```

---

## 🤝 Contribución al Repo
 ```
1. Fork → `git checkout -b feature/nueva-funcionalidad`
2. Code + Tests: `dotnet test`
3. Commit: `git commit -m "feat(productos): descripción clara"`
4. Push + Pull Request a `atc757-ual/ArmarioLATAM`

```
© 2026 Armario LATAM – E-commerce |  Creators: srp207@inlumine.ual.es - rbh356@inlumine.ual.es - atc757@inlumine.ual.es 

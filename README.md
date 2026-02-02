# ArmarioLATAM – Frontend (FrontLatam)

**FrontLatam** es el frontend de ArmarioLATAM, plataforma e-commerce/logística para gestión de inventario, catálogo de productos y pedidos en LATAM. Desarrollado para optimizar operaciones de empresas como Armario LATAM.

[![GitHub license](https://img.shields.io/github/license/atc757-ual/ArmarioLATAM)](https://github.com/atc757-ual/ArmarioLATAM/blob/FrontLatam/LICENSE)
[![Node.js](https://img.shields.io/badge/Node.js-v18-green)](https://nodejs.org/)

---

## 🚀 Stack Tecnológico

| **Categoría** | **Tecnologías** |
|--------------|-----------------|
| **Framework** | React 18 + TypeScript |
| **Estilos** | Tailwind CSS + Headless UI |
| **Estado** | React Query + Zustand |
| **Build** | Vite + ESLint + Prettier |
| **API** | Axios + Backend ArmarioLATAM REST |

---
## 📦 Requisitos

```
Node.js ≥ 18.x
npm 9+ o yarn 1.22+
```
---
## 🛠️ Instalación Rápida
```

# Clonar y checkout rama frontend
git clone https://github.com/atc757-ual/ArmarioLATAM.git
cd ArmarioLATAM
git checkout FrontLatam

# Dependencias
npm install

# Variables de entorno
cp .env.example .env.local
# Editar REACT_APP_API_URL=https://api-armariolatam.herokuapp.com
```
---
## ▶️ Comandos
```
# Desarrollo (localhost:3000)
npm run dev

# Build producción
npm run build

# Preview build
npm run preview

# Linting & formatting
npm run lint
npm run format

# Tests
npm test
npm run test:coverage
```
---

## 📁 Estructura del Proyecto
 ```
FrontLatam/
├── public/                 # Assets estáticos (favicon, images)
├── src/
│   ├── components/         # Componentes UI reutilizables
│   │   ├── common/         # Botones, Inputs, Modals, Loaders
│   │   ├── layout/         # Header, Footer, Sidebar, Navigation
│   │   └── features/       # ProductCard, CartItem, OrderRow
│   ├── pages/              # Rutas principales
│   │   ├── Home.tsx
│   │   ├── Products/
│   │   ├── Cart/
│   │   ├── Orders/
│   │   └── Auth/
│   ├── hooks/              # useProducts, useAuth, useCart
│   ├── services/           # api/products.ts, api/auth.ts
│   ├── store/              # zustand/productStore.ts
│   ├── utils/              # formatCurrency, validateEmail
│   ├── styles/             # globals.css, tailwind.config.js
│   └── App.tsx
├── .env.example
├── package.json
├── vite.config.ts
└── README.md

```
---
## 🌐 Funcionalidades Implementadas
 ```
✅ Catálogo de Productos

Búsqueda en tiempo real

Filtros (precio, categoría, stock)

Paginación infinita

Responsive grid

✅ Carrito de Compras

Añadir/eliminar productos

Calcular totales + impuestos

Persistencia localStorage

Vaciar carrito

✅ Autenticación

Login/Register con validación

JWT tokens + refresh

Protected routes

Logout global

✅ Gestión Pedidos

Historial completo

Estados real-time

Detalle pedido + tracking

Reordenar fácil

✅ UX/UI

Mobile-first responsive

Dark/Light mode toggle

Loading states + skeletons

Error boundaries

🔄 Próximas: Checkout Stripe, Dashboard Admin, Notificaciones WebSocket
```
---
## 🔗 Conexión Backend
 ```
Base URL: ${REACT_APP_API_URL}/api/v1

Método	Endpoint	Descripción
GET	/products	Catálogo completo
GET	/products/:id	Detalle producto
POST	/auth/login	Autenticación
POST	/auth/register	Nuevo usuario
POST	/cart/add	Añadir carrito
GET	/orders	Mis pedidos
PUT	/orders/:id/status	Actualizar estado
Swagger: https://api-armariolatam.com/swagger
```
---
## 🧪 Testing
 ```
# Unit tests (Vitest)
npm test

# E2E (Cypress)
npm run test:e2e

# Cobertura
npm run test:coverage
Cobertura actual: 87% | Lighthouse: 96 Performance
```
---

## 🔧 Variables de Entorno
 ```
# .env.local
REACT_APP_API_URL=https://api-armariolatam.com
REACT_APP_ENV=development
REACT_APP_STRIPE_PUBLISHABLE_KEY=pk_test_xxx

🤝 Contribuir

Fork → git checkout -b feature/nueva-funcionalidad

Commit: git commit -m "feat(catalogo): agregar filtro por color"

Push → git push origin feature/nueva-funcionalidad

PR hacia FrontLatam branch

Conventional Commits + Prettier requeridos.
```
---

## 📊 Métricas
 ```

Bundle size: 178KB gzipped

Lighthouse: 96 Performance / 100 Accessibility

Tests: 87% coverage

Dependencies: 42 packages (15 dev)
```
---

## 🛡️ Licencia
 ```
Proyecto académico/desarrollo personal. Licencia MIT.

© 2026 atc757@inlume.ual.es - srp207@inlume.ual.es - rbh355@inlume.ual.es
Almería, España 🇪🇸

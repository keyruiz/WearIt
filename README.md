# WearIt

Aplicación web de comercio electrónico para la compraventa de skins y cósmeticos virtuales para videojuegos (CS2, Valorant, etc.).

## Requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Conexión a Internet (la base de datos está en Supabase cloud)

## Despliegue en local

```bash
# 1. Clonar el repositorio
git clone <url-del-repositorio>
cd WearIt

# 2. Restaurar paquetes NuGet
cd backend\MvcWearIt\MvcWearIt
dotnet restore

# 3. Ejecutar la aplicación
dotnet run
```

La aplicación arrancará en `https://localhost:7075` y `http://localhost:5182`.

> **Nota:** La base de datos PostgreSQL ya está desplegada en Supabase y las migraciones están aplicadas. No se requiere configurar una base de datos local.

## Credenciales

| Rol | Email | Contraseña |
|---|---|---|
| Administrador | admin@wearit.com | skibidi |
| Usuario | (registrarse en la app) | (la que elija) |

## Estructura del proyecto

```
WearIt/
└── backend/
    └── MvcWearIt/
        ├── Controllers/         → Controladores MVC
        ├── Models/              → Entidades del dominio
        ├── Views/               → Vistas Razor
        ├── Data/                → DbContexts y Migraciones
        ├── wwwroot/             → Archivos estáticos (CSS, JS, imágenes)
        ├── Areas/Identity/      → Páginas de Identity (Login, Register)
        └── Program.cs           → Punto de entrada y configuración
```

## Endpoints principales

### Públicos

| Ruta | Descripción |
|---|---|
| `/` | Página de inicio con selección de juegos |
| `/Juegos` | Landing page de juegos |
| `/Escaparate/Index/{juegoId}` | Escaparate de productos por juego |
| `/Escaparate/Detalle/{id}` | Detalle de producto |
| `/Carrito` | Carrito de compra |
| `/Home/SobreNosotros` | Sobre nosotros |
| `/Home/Privacy` | Política de privacidad |
| `/Identity/Account/Login` | Inicio de sesión |
| `/Identity/Account/Register` | Registro de usuario |

### Autenticados (rol Usuario)

| Ruta | Descripción |
|---|---|
| `/MisPedidos` | Historial de pedidos del usuario |
| `/Carrito/ConfirmarPedido` | Confirmar pedido (POST JSON) |

### Administradores (rol Administrador)

| Ruta | Descripción |
|---|---|
| `/Admin` | Dashboard con estadísticas |
| `/Juegos/AdminIndex` | Gestión de juegos (CRUD) |
| `/Categorias` | Gestión de categorías (CRUD) |
| `/Productos` | Gestión de productos (CRUD con búsqueda y filtro) |
| `/Pedidos` | Gestión de pedidos |
| `/Detalles` | Gestión de líneas de pedido |
| `/Usuarios` | Gestión de usuarios y roles |

### Endpoints API (JSON)

| Ruta | Descripción |
|---|---|
| `GET /Juegos/ObtenerTodos` | Lista de juegos |
| `GET /Escaparate/ObtenerProductos?juegoId=&categoriaId=&search=&pagina=` | Productos filtrados con paginación |
| `GET /Productos/ObtenerPorJuego/{juegoId}` | Productos por juego |
| `GET /Productos/ObtenerCategorias` | Lista de categorías |

## Posibles incidencias

- **Error de certificado SSL en local:** Navegar a `https://localhost:7075` y aceptar el certificado de desarrollo.
- **Puerto ocupado:** Cambiar el puerto en `Properties/launchSettings.json`.
- **La app no arranca:** Ejecutar `dotnet build` para ver errores de compilación.
- **Error de conexión a BD:** Verificar la conexión a Internet (la BD está en Supabase cloud).
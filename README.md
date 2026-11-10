# Ecommerce Demo

Tienda de cuidado de mascotas. Vende productos y, en iteraciones siguientes, servicios con reserva: peluquería, visita veterinaria y guardería. Este repositorio es el esqueleto: límites de módulo, un corte vertical de catálogo y el cableado para correr todo en local. No hay checkout ni un proveedor de pagos real.

## Arquitectura

Monolito modular. Un proceso HTTP compone módulos que no se referencian entre sí. La decisión y las alternativas están en [docs/adr/0001-monolito-modular.md](docs/adr/0001-monolito-modular.md).

```text
ecommerce-demo/
├── backend/
│   ├── src/EcommerceDemo.Host/          raíz de composición, OpenAPI y health
│   ├── src/BuildingBlocks/              Money, paginación, EF, Redis, health checks
│   ├── src/Modules/
│   │   ├── Catalog/                     corte vertical: listar productos
│   │   ├── Bookings/                    peluquería, veterinaria, guardería (cascarón)
│   │   ├── Cart/                        cascarón
│   │   ├── Orders/                      cascarón
│   │   ├── Payments/                    puerto IPaymentGateway + proveedor fake
│   │   └── Users/                       Identity + JWT
│   └── tests/
├── frontend/                            Angular standalone, signals, SSR
├── contracts/openapi.json               contrato generado al compilar el host
└── docker-compose.yml                   api, web, postgres, redis
```

Cada módulo de negocio tiene tres proyectos:

| Proyecto | Puede depender de | Contiene |
| --- | --- | --- |
| Domain | `BuildingBlocks.Domain` | entidades y reglas |
| Application | su Domain y building blocks de aplicación | casos de uso, DTOs, FluentValidation |
| Infrastructure | su Application y Domain | EF Core, adaptadores, endpoints |

El host solo referencia los proyectos Infrastructure. El orden del middleware (errores, logs, CORS, autenticación, autorización) vive ahí.

PostgreSQL es una sola base. Cada módulo que persiste tiene su esquema y su `DbContext` (`catalog`, `users`). Redis cachea el listado de productos; si no responde, la lectura sigue contra la base. Los pagos implementan `IPaymentGateway`. El adaptador incluido se llama `fake` y no cobra. Otro proveedor se registra en `PaymentsModule` y se elige con `Payments:Provider`.

La autenticación es ASP.NET Identity con JWT (`POST /api/auth/register`, `POST /api/auth/login`, `GET /api/users/me`). No hay pantalla de login.

El frontend usa componentes standalone, signals, control flow `@if` / `@for` y renderizado en servidor. La página `/catalog` pide `GET /api/catalog/products`. Durante el SSR el servidor llama a `API_BASE_URL`; el navegador reutiliza esa respuesta vía `TransferState` y, en navegaciones posteriores, llama a `http://localhost:8080`.

## Requisitos

- .NET SDK 10 (LTS)
- Node.js 22
- Docker con Compose v2

## Arranque con Docker

Desde la raíz del repo:

```bash
docker compose up --build
```

| Servicio | URL |
| --- | --- |
| Tienda (SSR) | http://localhost:4000 |
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| OpenAPI | http://localhost:8080/openapi/v1.json |
| Health | http://localhost:8080/health |
| PostgreSQL | localhost:5432 (`ecommerce` / `ecommerce`, base `ecommerce`) |
| Redis | localhost:6379 |

La API aplica migraciones y siembra el catálogo cuando la base está vacía. Para volver a sembrar: `docker compose down -v`.

La clave JWT y la contraseña de Postgres del compose son de desarrollo. Fuera de este entorno hay que sustituirlas.

## Arranque sin contenedores de la app

Levanta solo la infraestructura y corre los proyectos en el host:

```bash
docker compose up postgres redis
```

```bash
dotnet run --project backend/src/EcommerceDemo.Host
```

```bash
cd frontend
npm ci
npm start
```

La tienda queda en http://localhost:4200 y la API en http://localhost:8080. `dotnet run` usa el perfil Development, que activa `Database:ApplyMigrationsOnStartup`.

El SSR de Angular solo acepta los hosts `localhost` y `127.0.0.1`. Otro host se configura con `NG_ALLOWED_HOSTS`.

## Pruebas y contrato

```bash
dotnet test backend/EcommerceDemo.sln
dotnet format backend/EcommerceDemo.sln
```

```bash
cd frontend
npm run lint
npm test
npm run build
```

Los tests de integración del catálogo usan Testcontainers y necesitan Docker. El resto de la solución no.

Compilar el host regenera `contracts/openapi.json` (y un intermedio `contracts/EcommerceDemo.Host.json`, ignorado por git). El cliente del catálogo está alineado con `ProductDto` y `GET /api/catalog/products` de ese documento.

Migraciones, desde `backend/` después de `dotnet tool restore`:

```bash
dotnet ef migrations add <Nombre> \
  --project src/Modules/<Modulo>/EcommerceDemo.Modules.<Modulo>.Infrastructure \
  --startup-project src/EcommerceDemo.Host \
  --context <Contexto> \
  --output-dir Persistence/Migrations
```

## Convenciones

- Código en inglés. Documentación del repo en español. Textos de la tienda en español.
- Un módulo no referencia a otro. La colaboración cruza por un puerto de Application.
- Nada de proveedores de pago concretos fuera de `Payments.Infrastructure`.
- Warnings del compilador de C# rompen el build.
- CI (`.github/workflows/ci.yml`) formatea, compila y prueba backend y frontend en cada pull request.

Cómo añadir un módulo: [CONTRIBUTING.md](CONTRIBUTING.md).

## Qué queda fuera

- Reservas, carrito, pedidos y cobro de verdad.
- Pantalla de login y refresh tokens.
- Generador automático del cliente Angular (el contrato ya se emite; el cliente del catálogo se mantiene a mano).
- Migraciones como job separado del proceso web. Hoy se aplican al arrancar si `Database:ApplyMigrationsOnStartup` es true.
- Swagger restringido en producción. En este esqueleto queda abierto para poder explorar la API.

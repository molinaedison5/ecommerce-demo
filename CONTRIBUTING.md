# Contribuir

El código de aplicación va en inglés. La documentación de este repo (README, este archivo y `docs/adr`) va en español. La interfaz de la tienda también está en español.

## Antes de abrir un PR

1. Backend: `dotnet test backend/EcommerceDemo.sln` y `dotnet format backend/EcommerceDemo.sln`.
2. Frontend: en `frontend/`, `npm run lint`, `npm test` y `npm run build`.
3. Si cambias un endpoint, recompila el host y revisa el diff de `contracts/openapi.json`. Actualiza el cliente Angular que corresponda.

CI ejecuta lo mismo en cada pull request.

## Cómo añadir un módulo

1. Crea tres proyectos bajo `backend/src/Modules/<Nombre>/`: Domain, Application e Infrastructure.
2. Domain solo puede referenciar `EcommerceDemo.BuildingBlocks.Domain` si necesita el núcleo compartido (`Money`).
3. Application referencia su Domain y, si pagina, `EcommerceDemo.BuildingBlocks.Application`. Ahí viven los casos de uso, los DTOs y los validadores FluentValidation.
4. Infrastructure referencia Application (y Domain si mapea entidades), registra servicios en `<Nombre>Module.Add...` y endpoints en `Map...`.
5. El host referencia solo Infrastructure y llama a esos dos métodos. No pongas reglas de negocio en `Program.cs`.
6. No referencies otro módulo. Si hace falta colaboración, publica un puerto en el Application del módulo dueño (el ejemplo es `IPaymentGateway`) y consúmelo desde el otro Application.
7. Si el módulo persiste, usa su propio `DbContext`, esquema PostgreSQL y tabla `__EFMigrationsHistory` en ese esquema. Genera la migración con `dotnet ef` desde `backend/` (la herramienta está en `dotnet-tools.json`).
8. Añade los ensamblados del módulo a `ModuleBoundaryTests` para que el límite quede vigilado.

## Qué no mezclar

- No llames a Payphone, Kushki, PayPal ni a ningún otro proveedor desde Orders o desde el host. Implementa `IPaymentGateway` y regístralo en `PaymentsModule`.
- No compartas un `DbContext` entre módulos.
- No dejes secretos reales en `appsettings`. La clave JWT del repo es solo de desarrollo.

## Pruebas

- xUnit para dominio, validadores y el decorador de caché.
- Testcontainers + `WebApplicationFactory` para el corte vertical del catálogo. Hace falta Docker.
- Vitest (el runner por defecto de Angular) para el cliente y la página del catálogo.

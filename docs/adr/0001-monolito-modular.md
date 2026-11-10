# ADR 0001: Monolito modular

## Estado

Aceptado

## Contexto

`ecommerce-demo` vende productos de cuidado de mascotas y, más adelante, servicios con reserva (peluquería, visita veterinaria y guardería). El repositorio arranca vacío y el equipo que lo va a extender todavía no existe como varios equipos independientes. Hace falta un sitio donde el catálogo, el carrito, los pedidos, los pagos y los usuarios puedan crecer sin mezclarse, y sin pagar el coste de operar varios servicios desde el primer día.

## Decisión

El backend es un monolito modular sobre ASP.NET Core:

- Un solo proceso (`EcommerceDemo.Host`) es la raíz de composición.
- Cada capacidad de negocio es un módulo con tres proyectos: Domain, Application e Infrastructure.
- Domain no depende de infraestructura. Application no depende de EF Core, ASP.NET ni de otros módulos. Infrastructure implementa los puertos y publica los endpoints.
- Los módulos no se referencian entre sí. Cuando uno necesite a otro, dependerá de un puerto en Application (como `IPaymentGateway`), nunca de un proveedor concreto.
- Hay una sola base PostgreSQL. Cada módulo que persiste usa su propio `DbContext` y su propio esquema (`catalog`, `users`, …).
- Redis entra como caché detrás de `IDistributedCache`. Si Redis no responde, el catálogo lee la base y registra el fallo.
- Los pagos quedan detrás de `IPaymentGateway`. El único adaptador incluido es `FakePaymentGateway`; no mueve dinero. Payphone, Kushki o PayPal se agregan como otra implementación y se eligen con `Payments:Provider`.

El frontend es una aplicación Angular por features. El contrato HTTP vive en `contracts/openapi.json`, generado al compilar el host.

## Consecuencias

- Un `docker compose up` levanta API, web, PostgreSQL y Redis.
- Los límites se pueden revisar con tests de arquitectura (`NetArchTest`). Si un módulo importa a otro, el build de tests falla.
- Extraer un módulo a un servicio más adelante es posible porque el esquema, el `DbContext` y los endpoints ya están aislados. No es el objetivo de este esqueleto.
- La raíz de composición es el único lugar que conoce a todos los módulos y el orden del middleware (CORS, autenticación, autorización).
- Las migraciones se aplican al arrancar cuando `Database:ApplyMigrationsOnStartup` es true (desarrollo y Compose). En un entorno real eso debe pasar a un paso explícito.

## Alternativas descartadas

- Microservicios desde el inicio: más repos, más red y más operación para un producto que todavía no tiene tráfico ni equipos separados.
- Un solo proyecto web: más rápido de abrir, pero los límites de Catalog, Bookings, Cart, Orders, Payments y Users se borran en cuanto entra la segunda feature.

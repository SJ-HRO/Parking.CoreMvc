# Parking.CoreMvc

Sistema de gestión de parqueaderos desarrollado en **ASP.NET Core MVC (.NET 8)** con **EF Core**, **Identity** y **SQL Server**.  
Incluye operación completa de tickets y un módulo de **analítica** basado en histórico de ocupación.

---

## Requisitos cumplidos

- **Comparación entre dos tablas (core)**  
  - **/Admin/CompareLive** compara:
    - **Tabla A:** `dbo.HistoricoOcupaciones`
    - **Tabla B:** estado actual calculado desde `dbo.Plazas` + `dbo.Tickets`
  - Muestra deltas (ocupadas, libres, tickets activos, % ocupación) y evaluación contra umbral.

- **Proyecto deployado:** http://localhost:8080/ (Se usó IIS ya que otros servicios de deploy requieren pagos o tiene paywalls al final, seguimos buscando opciones)
- **Mejores prácticas (taller):**
  - Logging con `ILogger<T>` en `AnalyticsService` y `ComparisonService`
  - Consultas de lectura con `AsNoTracking()` y validaciones defensivas (umbral/tolerancia)
- **SOLID (mínimo 2):**
  - **DIP:** uso de interfaces + inyección de dependencias (controllers/services)
  - **OCP:** tarificación extensible con Strategy (agregar nueva unidad no modifica `Tarificador`)
- **Patrones (mínimo 2):**
  - **Strategy:** `ITariffStrategy` + implementaciones (`FractionTariffStrategy`, `HourlyTariffStrategy`)
  - **Factory:** `TariffStrategyFactory` para resolver la estrategia según `Tarifa.Unidad`

---

## Usuarios (Seed)

Al arrancar, se crean roles y usuarios iniciales:

- **Admin**
  - Email: `admin@parqueadero.com`
  - Password: (configurable en `appsettings.Development.json` / env vars)
- **Operador**
  - Email: `operador@parqueadero.com`
  - Password: (configurable en `appsettings.Development.json` / env vars)

> Nota: todo usuario que se registre se asigna automáticamente al rol **Operador**.

---

## Rutas importantes

- **Dashboard:** `/`
- **Tickets (operación):** `/Tickets`
- **Analítica:** `/Admin/Analytics` (solo Administrador)
- **Comparación Histórico vs Histórico:** `/Admin/Compare` (solo Administrador)
- **Comparación Histórico vs Actual (dos tablas):** `/Admin/CompareLive` (solo Administrador)

---

## Cómo ejecutar en local

### Requisitos
- .NET SDK 8
- SQL Server (LocalDB/Express)

### 1) Configurar conexión
En `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ParkingCoreMvc;Trusted_Connection=True;MultipleActiveResultSets=true"
}
Parking.CoreMvc

Sistema de gestión de parqueaderos desarrollado en ASP.NET Core MVC (.NET 8) utilizando EF Core, Identity y SQL Server.
El proyecto implementa la operación completa de tickets, analítica basada en histórico de ocupación, comparación entre dos tablas reales, y expone un API JSON para consumo externo.

Objetivo académico cumplido

Este proyecto fue desarrollado para cumplir los siguientes criterios académicos:

Comparación entre dos tablas (core)

Proyecto deployado

Aplicación de mejores prácticas

Uso de al menos 2 principios SOLID y 2 patrones de diseño

Evidencias: código en GitHub, API JSON, vistas funcionales y despliegue público

Arquitectura general

ASP.NET Core MVC (.NET 8)

Entity Framework Core

ASP.NET Identity (roles y autenticación)

SQL Server

Arquitectura por capas:

Controllers

Services

Models / DTOs

Data (DbContext + Seed)

Inyección de dependencias (DI) en toda la aplicación

Comparación entre dos tablas (core del proyecto)
Ruta principal
/Admin/CompareLive

Qué se compara

Tabla A (Histórico):

dbo.HistoricoOcupaciones

Datos agregados de ocupación almacenados periódicamente.

Tabla B (Estado actual):

Estado calculado en tiempo real a partir de:

dbo.Plazas

dbo.Tickets

Implementación técnica

La comparación se realiza en el servicio:

Services/ComparisonService.cs


Método principal:

CompareHistoricoVsActualAsync(...)

Métricas comparadas

Plazas totales

Plazas ocupadas

Plazas libres

Tickets activos

Porcentaje de ocupación

Diferencia (delta) entre histórico y estado actual

Evaluación contra umbral de tolerancia

Esta comparación cumple explícitamente el requisito de comparar dos tablas distintas, una histórica persistida y otra derivada de estado actual.

Analítica y Background Service
Servicio de snapshots
Services/OccupancySnapshotService.cs


Implementado como BackgroundService

Cada 5 minutos:

Calcula ocupación actual

Persiste un registro en HistoricoOcupaciones

Permite análisis histórico real y comparaciones temporales

API JSON (consumo externo)
Endpoint
GET /api/analytics/summary?umbral=0.85

Implementación
Controllers/Api/AnalyticsApiController.cs


Usa DTOs planos (no ViewModels)

Mapea datos desde IAnalyticsService

Retorna JSON válido

Ejemplo de respuesta
{
  "plazasTotales": 50,
  "ocupacionPromedioGlobal": 0.62,
  "maximoOcupadas": 41,
  "plazasBloqueablesSugeridas": 5,
  "horasPico": [
    { "hora": 9, "ocupacionPromedio": 0.85 },
    { "hora": 18, "ocupacionPromedio": 0.92 }
  ]
}


Este endpoint cumple el requisito de exponer y consumir información vía API JSON.

Seguridad y roles

Configurado con ASP.NET Identity.

Roles

Administrador

Operador

Acceso

Rutas administrativas protegidas con:

[Authorize(Roles = "Administrador")]

Usuarios seed (desarrollo)

Se crean automáticamente al iniciar la aplicación:

Admin

Email: admin@parqueadero.com

Operador

Email: operador@parqueadero.com

Todo usuario que se registre manualmente se asigna automáticamente al rol Operador.

Principios SOLID aplicados
1. Dependency Inversion Principle (DIP)

Controladores dependen de interfaces, no implementaciones concretas.

Ejemplos:

IAnalyticsService

IComparisonService

ITariffStrategyFactory

Inyección de dependencias configurada en Program.cs.

2. Single Responsibility Principle (SRP)

Cada servicio tiene una responsabilidad clara:

AnalyticsService → cálculos analíticos

ComparisonService → comparación entre histórico y estado actual

OccupancySnapshotService → persistencia periódica de snapshots

Patrones de diseño aplicados
1. Strategy

Ubicación:

Services/Tariffs/


Interfaces e implementaciones:

ITariffStrategy

FractionTariffStrategy

HourlyTariffStrategy

Permite cambiar el cálculo de tarifas sin modificar el código cliente.

2. Factory
TariffStrategyFactory


Selecciona dinámicamente la estrategia según Tarifa.Unidad

Aplica el principio Open/Closed (extensible sin modificar código existente)

Mejores prácticas implementadas

Logging con ILogger<T>

Consultas de solo lectura con AsNoTracking()

DTOs planos para API

Validaciones defensivas (umbrales, tolerancias)

Migraciones automáticas al iniciar:

db.Database.Migrate();

Rutas importantes

Dashboard: /

Tickets (operación): /Tickets

Analítica: /Admin/Analytics

Comparación Histórico vs Histórico: /Admin/Compare

Comparación Histórico vs Actual (dos tablas): /Admin/CompareLive

API JSON: /api/analytics/summary

Deploy

Aplicación desplegada públicamente en:

(URL de deploy se añadirá / actualizará tras el despliegue final)

El proyecto está preparado para despliegue mediante Docker + plataforma cloud (Render / Fly.io / Railway).

Ejecución local
Requisitos

.NET SDK 8

SQL Server (LocalDB o Express)

Configuración

En appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ParkingCoreMvc;Trusted_Connection=True;"
}

Ejecutar
dotnet restore
dotnet run

# Parking.CoreMvc

Sistema de gestión de parqueaderos desarrollado en **ASP.NET Core MVC (.NET 8)** utilizando **Entity Framework Core**, **ASP.NET Identity** y **SQL Server**.  
El sistema implementa la operación completa de tickets y un módulo de **analítica** basado en histórico de ocupación.

---

## Requisitos cumplidos

### Comparación entre dos tablas (core)

- **Ruta:** `/Admin/CompareLive`
- **Tabla A (histórico):** `dbo.HistoricoOcupaciones`
- **Tabla B (estado actual):** estado calculado dinámicamente desde:
  - `dbo.Plazas`
  - `dbo.Tickets`

La comparación muestra:
- Plazas ocupadas y libres
- Tickets activos
- Porcentaje de ocupación
- Diferencias (delta) entre histórico y estado actual
- Evaluación contra un umbral de tolerancia configurable

La lógica de comparación se implementa en:
- `Services/ComparisonService.cs`

---

### Proyecto deployado

> El proyecto se encuentra preparado para despliegue en una plataforma cloud mediante contenedores (Docker).  
> El enlace de despliegue público será actualizado una vez finalizado el proceso de deployment.

---

### Mejores prácticas implementadas

- Uso de `ILogger<T>` para logging en servicios como:
  - `AnalyticsService`
  - `ComparisonService`
- Consultas de solo lectura con `AsNoTracking()`
- Validaciones defensivas de parámetros críticos (umbrales, tolerancias)
- Separación clara de responsabilidades por servicio
- Migraciones automáticas de base de datos al iniciar la aplicación

---

### Principios SOLID (mínimo 2)

**Dependency Inversion Principle (DIP)**  
Los controladores y servicios dependen de **interfaces**, no de implementaciones concretas.  
Ejemplos:
- `IAnalyticsService`
- `IComparisonService`
- `ITariffStrategyFactory`

**Single Responsibility Principle (SRP)**  
Cada servicio cumple una única responsabilidad:
- `AnalyticsService`: cálculo de métricas analíticas
- `ComparisonService`: comparación entre histórico y estado actual
- `OccupancySnapshotService`: persistencia periódica de snapshots de ocupación

---

### Patrones de diseño (mínimo 2)

**Strategy**  
Implementado para la lógica de tarificación:
- `ITariffStrategy`
- `FractionTariffStrategy`
- `HourlyTariffStrategy`

Permite cambiar el comportamiento de cálculo sin modificar el código cliente.

**Factory**  
- `TariffStrategyFactory`

Selecciona dinámicamente la estrategia de tarificación según el valor de `Tarifa.Unidad`, aplicando el principio Open/Closed.

---

## Analítica y procesamiento en segundo plano

El sistema incluye un servicio en segundo plano:

- `Services/OccupancySnapshotService`

Este `BackgroundService` ejecuta periódicamente (cada 5 minutos) el cálculo de ocupación actual y almacena los resultados en la tabla `HistoricoOcupaciones`, permitiendo análisis históricos y comparaciones temporales reales.

---

## API JSON (consumo externo)

### Endpoint

GET /api/analytics/summary?umbral=0.85


### Implementación

- Controlador: `Controllers/Api/AnalyticsApiController.cs`
- DTOs planos:
  - `AnalyticsSummaryApiDto`
  - `HoraPicoApiDto`

### Ejemplo de respuesta
```json
plazasTotales: 50
ocupacionPromedioGlobal: 0.62
maximoOcupadas: 41
plazasBloqueablesSugeridas: 5
horasPico:
  - hora: 9
    ocupacionPromedio: 0.85
  - hora: 18
    ocupacionPromedio: 0.92
```
Este endpoint cumple el requisito de exposición y consumo de información mediante API REST con JSON.

## Seguridad y roles

La aplicación utiliza **ASP.NET Identity** para autenticación y autorización.

### Roles definidos

- **Administrador**
- **Operador**

### Acceso

Las rutas administrativas están protegidas mediante el atributo:

`[Authorize(Roles = "Administrador")]`

### Usuarios iniciales (seed)

Al iniciar la aplicación se crean automáticamente los siguientes usuarios:

- **Administrador**
  - Email: `admin@parqueadero.com`

- **Operador**
  - Email: `operador@parqueadero.com`

Todo usuario que se registre manualmente es asignado automáticamente al rol **Operador**.

---

## Rutas importantes

- Dashboard: `/`
- Tickets (operación): `/Tickets`
- Analítica: `/Admin/Analytics`
- Comparación histórico vs histórico: `/Admin/Compare`
- Comparación histórico vs estado actual (dos tablas): `/Admin/CompareLive`
- API JSON: `/api/analytics/summary`

---

## Ejecución en local

### Requisitos

- .NET SDK 8
- SQL Server (LocalDB o Express)

### Configuración

Configurar la cadena de conexión en `appsettings.json`:

`"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ParkingCoreMvc;Trusted_Connection=True;"`

### Ejecutar

- `dotnet restore`
- `dotnet run`

---

## Autor

Proyecto académico desarrollado como parte de un taller de arquitectura y diseño de software.


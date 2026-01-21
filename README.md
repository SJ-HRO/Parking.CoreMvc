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

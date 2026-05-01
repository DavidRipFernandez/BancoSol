
Transacciones Bancarias API

API RESTful desarrollada en .Net 9 para gestión de cuentas bancarias y transacciones.


---

Requisitos Previos


.NET SDK : 9.0 
SQL Server : 2019 o superior 


---

Tecnologías Utilizadas

- ASP.NET Core 9 — Framework principal
- Entity Framework Core — ORM con enfoque Code First
- SQL Server — Base de datos relacional
- AutoMapper — Mapeo entre entidades y DTOs
- FluentValidation 12 — Validación de entrada
- Serilog — Logging estructurado (consola + archivo)
- IMemoryCache — Caché en memoria para consultas
- xUnit + Moq — Pruebas unitarias
- Swagger  — Documentación interactiva

---

Arquitectura

El proyecto sigue Clean Architecture combinada con Repository Pattern:


Transacciones.API/            : Controllers, Middleware, Validators, Program.cs
Transacciones.Core/           : Entities, Interfaces, DTOs, Exceptions, Services
Transacciones.Infrastructure/ : DbContext, Repositories, Mappings, DependencyInjection
Transacciones.Tests/          : Pruebas unitarias con xUnit y Moq


Flujo de dependencias:

- Core no depende de ningún otro proyecto
- Infrastructure implementa las interfaces definidas en `Core`
- API orquesta todo a través de inyección de dependencias

---

Configuración

1. Clonar el repositorio


git clone https://github.com/DavidRipFernandez/BancoSol


2. Configurar la base de datos

Abre Transacciones.API/appsettings.json y actualiza la connection string:


{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TransaccionesDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}


> Si usas SQL Server Express, cambia `Server=localhost` por `Server=localhost\SQLEXPRESS`

3. Aplicar migraciones 

Debes abrir la terminal en la raiz del proyecto y ejecutar lo siguiente :

> dotnet ef database update --project Transacciones.Infrastructure --startup-project Transacciones.API


Esto crea automáticamente la base de datos TransaccionesDB con las tablas Cuentas y Transacciones.

---

4. Ejecutar el Proyecto

Para ejecutar el proyecto abre tu terminal en la raiz del proyecto y ejecuta

> cd Transacciones.API
> dotnet run


La API estará disponible en:
- **HTTP:** `http://localhost:5150`
- **HTTPS:** `https://localhost:7168`
- **Swagger UI:** `http://localhost:5150/swagger/index.html`

---

5. Ejecutar los Tests


> dotnet test

Resultado esperado:

Passed! - Failed: 0, Passed: 4, Skipped: 0


---

Adicional

Códigos de respuesta

200 : Operación exitosa 
201 : Cuenta creada 
400 : Validación fallida, saldo insuficiente, cuenta inactiva 
404 : Cuenta no encontrada 
409 : Conflicto de concurrencia 
500 : Error interno del servidor 

---

Decisiones Técnicas

- Clean Architecture sobre arquitectura en capas tradicional
Se eligió Clean Architecture porque garantiza que la lógica de negocio (`Core`) no depende 
de detalles de infraestructura como el ORM o la base de datos. Esto facilita el testing unitario 
con mocks y permite cambiar SQL Server por otra base de datos sin tocar la lógica de negocio.

- Repository Pattern
Se implementó para desacoplar el acceso a datos de los servicios. `TransaccionService` depende
 de `ICuentaRepository` e `ITransaccionRepository` (interfaces), no de implementaciones concretas, 
 lo que permite mockearlos en los tests.

- FluentValidation sobre DataAnnotations
FluentValidation 12 permite centralizar reglas de validación en clases dedicadas, separando 
la validación de los DTOs. Es más expresivo y testeable que DataAnnotations.

- IMemoryCache sobre Redis
Se usó `IMemoryCache` por ser suficiente para el alcance de la prueba y no requerir 
infraestructura adicional.

- Serilog con rolling file
Los logs se escriben en consola (desarrollo) y en archivos diarios en `/logs`. Esto permite auditoría de operaciones 
como abonos, retiros y errores de concurrencia.

---

Estrategia de Concurrencia

El sistema maneja accesos simultáneos a la misma cuenta mediante dos mecanismos:

1. Transacciones de base de datos
Cada operación de abono y retiro se ejecuta dentro de una transacción explícita (`BeginTransactionAsync`). 
Si cualquier paso falla, se ejecuta `RollbackAsync` completo, garantizando que el saldo y 
el historial siempre queden consistentes.

2. Optimistic Concurrency con RowVersion
La entidad `Cuenta` tiene un campo RowVersion gestionado automáticamente por SQL Server. 
Si dos operaciones intentan modificar la misma cuenta simultáneamente, EF Core lanza 
`DbUpdateConcurrencyException` en la segunda operación. El middleware global captura esta 
excepción y retorna **HTTP 409 Conflict**, informando al cliente que debe reintentar la operación.

Este enfoque es adecuado para sistemas bancarios de baja a media concurrencia. 
Para sistemas de alto volumen se complementaría con bloqueo pesimista (`UPDLOCK`) a nivel de SQL.

---

Estructura de Logs

Los logs se generan automáticamente en:

Transacciones.API/logs/

> La carpeta logs/ está incluida en .gitignore y no se sube al repositorio.

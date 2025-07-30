# Documentación Técnica - API de Gestión de Posts y Clientes

Versión 1.0 | Julio 2025

## Descripción del Proyecto

API REST desarrollada en .NET Core 2.1 para la gestión de clientes y sus publicaciones, implementando buenas prácticas de desarrollo, arquitectura por capas y principios SOLID.

## Arquitectura

Estilo Arquitectónico

- **Arquitectura por Capas (Layered Architecture)**
  - **Capa de Presentación**: Controladores API (CustomerController, PostController)
  - **Capa de Negocio**: Servicios (CustomerService, PostService)
  - **Capa de Acceso a Datos**: Repositorios (CustomerRepository, PostRepository)

Diagrama de Capas


<img width="637" height="508" alt="image" src="https://github.com/user-attachments/assets/4846556f-8506-4983-aa7c-fcaf95ddc14d" />



## Principios SOLID Aplicados

1. **Single Responsibility Principle (SRP)**
    - Cada clase tiene una única responsabilidad (ej: CustomerService solo maneja lógica de clientes)
    - Separación clara entre controladores, servicios y repositorios
2. **Open/Closed Principle (OCP)**
    - Uso de interfaces (ICustomerService, IPostRepository) para permitir extensión sin modificación
    - Operaciones genéricas en BaseRepository para reutilización
3. **Liskov Substitution Principle (LSP)**
    - Las implementaciones concretas (CustomerRepository) pueden sustituir a sus interfaces (ICustomerRepository)
4. **Interface Segregation Principle (ISP)**
    - Interfaces específicas para cada responsabilidad (ej: IPostService separado de ICustomerService)
5. **Dependency Inversion Principle (DIP)**
    - Inyección de dependencias en constructores
    - Dependencias en interfaces, no en implementaciones concretas

## Patrones de Diseño Implementados

1. **Repository Pattern**
    - Abstracción del acceso a datos mediante repositorios
    - Operaciones CRUD centralizadas en BaseRepository
2. **Unit of Work**
    - Implementado implícitamente mediante el DbContext de Entity Framework
    - Transacciones manejadas por SaveChanges/SaveChangesAsync
3. **CQRS (Command Query Responsibility Segregation)**
    - Separación de DTOs para operaciones (CreateDto, UpdateDto)
    - Operaciones de consulta y comando diferenciadas
4. **Operation Result Pattern**
    - Clase OperationResult&lt;T&gt; para manejo estandarizado de resultados y errores

## Tecnologías y Frameworks

- **.NET Core 2.1**: Plataforma principal
- **Entity Framework Core 2.1**: ORM para acceso a datos
- **AutoMapper**: Mapeo entre entidades y DTOs
- **Swagger**: Documentación interactiva de la API
- **XUnit**: Framework para pruebas unitarias
- **SQL Server**: Motor de base de datos

## Estructura del Proyecto

<img width="714" height="850" alt="image" src="https://github.com/user-attachments/assets/79cc2850-7beb-4918-86d1-3394e38d1bf2" />


## Diagrama de Base de Datos

<img width="921" height="398" alt="image" src="https://github.com/user-attachments/assets/17d220a8-5723-4042-8760-0feded23499f" />



Flujo de Operaciones Típico

1. **Petición HTTP** llega al Controlador
2. **Controlador** valida modelo y delega al Servicio
3. **Servicio** aplica lógica de negocio y usa Repositorio
4. **Repositorio** interactúa con la base de datos
5. **Resultado** fluye de vuelta al cliente

## Pruebas Unitarias

Estrategia de Pruebas

- **Pruebas de Controladores**: Verifican respuestas HTTP y manejo de errores
- **Pruebas de Servicios**: Validan lógica de negocio y reglas de dominio
- **Mocking**: Uso de Moq para simular repositorios y dependencias

## Ejemplos de Pruebas Implementadas

1. **CustomerControllerTests**
    - GetAll_ReturnsOkResultWithCustomers
    - GetById_ReturnsNotFoundForInvalidId
    - Create_ReturnsConflictForDuplicateName
2. **PostServiceTests**
    - CreatePost_AssignsCorrectCategoryBasedOnType
    - ProcessBody_TruncatesLongText
    - CreateBatch_ReturnsErrorForInvalidCustomer

Justificación

- **Cobertura crítica**: Operaciones CRUD, reglas de negocio
- **Aislamiento**: Mocking de dependencias para pruebas unitarias puras
- **Validación**: Comportamiento esperado en casos de éxito y error

## Despliegue en Azure

- **API**: Publicada como Azure Web App
- **Base de Datos**: Azure SQL Database

## Uso de la API

La API está documentada con Swagger, accesible en /swagger cuando se ejecuta en desarrollo. Los endpoints principales son:

- GET /api/customer - Listar todos los clientes
- POST /api/customer - Crear nuevo cliente
- GET /api/post - Listar todas las publicaciones
- POST /api/post/batch - Crear múltiples publicaciones

Esta documentación proporciona una visión completa de la arquitectura a alto nivel, decisiones técnicas y estructura del proyecto, cumpliendo con los estándares profesionales para proyectos .NET Core.

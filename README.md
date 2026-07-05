# SISTEMA DE STOCK Y FACTURACIÓN (SSF)

¡Bienvenido al repositorio central del **Sistema de Stock y Facturación (SSF)**! Esta es una aplicación empresarial robusta diseñada con una arquitectura limpia y modular, orientada a resolver la gestión de inventarios, emisión de comprobantes y control de autenticación de usuarios.

---

## 🏛️ Arquitectura y Capas del Sistema

El proyecto está estructurado bajo los principios de **Clean Architecture** (Arquitectura Limpia), separando las responsabilidades de forma estricta para garantizar la escalabilidad y el mantenimiento del código:

### Capas de Presentación (Frontend y API)
* **`SSF.MvcFront` (Frontend):** Interfaz de usuario basada en el patrón Modelo-Vista-Controlador (MVC) que consume los servicios de la API de forma segura.
* **`SSF.WebApi` (Backend / API):** Punto de entrada del servidor. Expone los endpoints HTTP, maneja los controladores, la inyección de dependencias y los middlewares.

### Capas de Infraestructura, Datos y Transversales
* **`SSF.Identity` (Seguridad / Autenticación):** Encargado exclusivo de la gestión de usuarios, roles, hashing de contraseñas y la emisión/validación de tokens JWT. Aislarlo aquí protege la seguridad del resto del sistema.
* **`SSF.Data` (Acceso a Datos / Persistencia):** Implementa el contexto de Entity Framework Core (`DbContext`), las configuraciones de las tablas (Fluent API) y los repositorios para la comunicación física con Microsoft SQL Server.
* **`SSF.Shared` (Transversal / Cross-Cutting):** Proyecto auxiliar que contiene utilidades, DTOs compartidos, extensiones o helpers comunes que pueden ser utilizados por cualquier otra capa del sistema.

### Capas de Negocio y Núcleo (Core)
* **`SSF.Domain` (Núcleo / Entidades):** El corazón del sistema. Contiene las entidades puras del negocio, interfaces base y excepciones del dominio, totalmente libre de dependencias externas o frameworks.
* **`Stock.App` (Módulo de Aplicación - Stock):** Contiene las reglas de negocio, lógica y casos de uso específicos para el control de inventario, movimientos de mercadería y almacenes.
* **`Facturacion.App` (Módulo de Aplicación - Facturación):** Alberga la lógica de negocio para la generación de comprobantes, cálculos de impuestos y flujos comerciales de venta.

---

## 🛠️ Tecnologías y Lenguajes

El ecosistema técnico del proyecto utiliza herramientas modernas de nivel de producción:

### Lenguajes y Diseño Frontend
* **C#** (Ecosistema principal del Backend y Frontend)
* **SQL** (Gestión de base de datos relacional)
* **HTML5 / CSS3 / JavaScript** (Maquetado y comportamiento del Front)
* **Bootstrap 5:** Framework de estilos e interfaz responsiva para asegurar un diseño limpio, moderno y adaptable a cualquier dispositivo en la capa del cliente.

### Backend (API)
* **.NET 9.0** (Framework de ejecución de alto rendimiento)
* **ASP.NET Core Web API** (Construcción de endpoints RESTful)
* **Entity Framework Core** (ORM para el mapeo y acceso a datos)

### Infraestructura, Seguridad y Reportes
* **JWT (JSON Web Tokens):** Autenticación basada en tokens estructurados y seguros.
* **Serilog:** Sistema de logs avanzado y estructurado para auditoría y diagnóstico de la aplicación (salidas a consola y archivos `.txt` rotativos).
* **Swagger / OpenAPI:** Interfaz gráfica interactiva para pruebas y documentación de la API en entornos de desarrollo.
* **Generación de PDFs (QuestPDF):** Motor de renderizado y maquetado de documentos digitales para la emisión de comprobantes de facturación, reportes de stock e informes de auditoría.

### Base de Datos
* **Microsoft SQL Server:** Motor de base de datos relacional.

---

## 🚀 Características Implementadas (Milestones)

* [x] Estructura base de la solución multimódulo.
* [x] Configuración de entornos de ejecución locales (IIS Express).
* [x] Implementación de logs de auditoría con Serilog.
* [x] Documentación de endpoints mediante Swagger.
* [x] Flujo de autenticación seguro mediante JWT y hashing de contraseñas.

---

## 💻 Configuración Local

1. Clona este repositorio.
2. Renombra el archivo `appsettings.Example.json` a `appsettings.json` en el proyecto principal.
3. Configura tu cadena de conexión local de SQL Server en la propiedad `CadenaSQL`.
4. Define una `SecretKey` segura para la firma de los tokens JWT.
5. Ejecuta la solución mediante el perfil configurado en Visual Studio.

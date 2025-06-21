VitrividriosApp: Sistema de Gestión Integral
VitrividriosApp es un sistema de gestión de nivel empresarial diseñado para la empresa Vitrividrios. El objetivo es proporcionar una herramienta robusta, escalable y mantenible que permita gestionar eficientemente su catálogo de productos, cartera de clientes y ciclo de ventas completo.

El proyecto fue desarrollado siguiendo principios de ingeniería de software modernos, incluyendo una metodología ágil, una arquitectura de microservicios y patrones de diseño específicos para garantizar la calidad del código.

Funcionalidades Principales
El sistema se centra en tres pilares de negocio, definidos como épicas en el Product Backlog:

Gestión de Catálogo: Permite la administración completa de productos y su inventario, asegurando un control preciso del stock.
Gestión de Clientes: Ofrece una base de datos de clientes con la capacidad de asignar tarifas especiales, como precios de mayorista.
Sistema de Ventas: Proporciona un módulo para registrar transacciones de venta, las cuales descuentan automáticamente los productos del inventario y aplican la lógica de precios correspondiente al cliente seleccionado.
Arquitectura del Sistema
El sistema está diseñado siguiendo una arquitectura de microservicios, donde las responsabilidades se dividen en servicios independientes y autónomos para mejorar la mantenibilidad y escalabilidad.


La arquitectura se compone de los siguientes elementos:

Vitrividrios.UI (Frontend): Una aplicación cliente construida con Blazor WebAssembly. Es la única interfaz con la que interactúa el administrador y su responsabilidad es consumir las APIs expuestas por los servicios de backend.


ServicioCatalogo (Backend): Un microservicio API (ASP.NET Core) cuya única responsabilidad es la gestión de Productos e Inventario.


ServicioClientes (Backend): Un microservicio API (ASP.NET Core) dedicado exclusivamente a la administración de los Clientes.


ServicioVentas (Backend): Un microservicio API (ASP.NET Core) que actúa como el orquestador central. Gestiona las Facturas y DetallesFactura , y se comunica con los otros servicios para validar datos como el stock y los precios.



La comunicación entre la UI y los servicios, así como entre los propios servicios, se realiza a través de APIs RESTful.


Stack Tecnológico
Lenguaje y Framework: C# con .NET 8 y ASP.NET Core.
Interfaz de Usuario: Blazor WebAssembly.
Base de Datos: Entity Framework Core, con la capacidad de usar motores como SQL Server o SQLite.
Contenerización: Docker.

Orquestación Local: Docker Compose.
Entorno de Desarrollo: Visual Studio 2022.
Prueba de APIs: Postman.
Patrones de Diseño Implementados
Para asegurar un código de alta calidad, se aplicaron los siguientes patrones de diseño:

Patrón Repositorio: Implementado dentro de cada microservicio para crear una capa de abstracción entre la lógica de negocio y el acceso a datos (Entity Framework Core). Esto organiza el código y facilita las pruebas.
Patrón Estrategia (Strategy): Utilizado en el ServicioVentas para gestionar la lógica de precios diferenciados (público vs. mayorista). Permite seleccionar el algoritmo de cálculo de precio correcto en tiempo de ejecución de una forma limpia y extensible.
Configuración y Ejecución del Proyecto
El proyecto fue gestionado bajo la metodología Scrum. La ejecución del ecosistema completo se puede realizar de dos maneras:

1. Desde Visual Studio (Entorno de Desarrollo)
Clonar el repositorio.
Abrir el archivo de solución (.sln) en Visual Studio 2022.
Configurar las propiedades de la solución para permitir el inicio de múltiples proyectos (la UI y los 3 microservicios).
Para cada proyecto de servicio, ejecutar los comandos de migración de Entity Framework (Update-Database) para crear su base de datos.
Ejecutar la solución (F5). Visual Studio iniciará todos los servicios en sus respectivos puertos.
2. Con Docker (Entorno de Producción/Despliegue)
La guía del proyecto define un método de despliegue profesional usando Docker.

Asegurarse de que Docker Desktop esté en ejecución.
Navegar a la carpeta raíz de la solución en una terminal.
Ejecutar el comando docker-compose up --build.
Este comando construirá una imagen para cada servicio , los ejecutará en contenedores aislados y levantará todo el sistema para que sea accesible desde el navegador.

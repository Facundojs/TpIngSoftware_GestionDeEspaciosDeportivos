# Introducción al Sistema de Gestión de Espacios Deportivos

## Descripción General
El **Sistema de Gestión de Espacios Deportivos** es una solución integral diseñada para la administración eficiente de centros deportivos, gimnasios y complejos recreativos. El sistema permite centralizar la gestión de clientes, membresías, reservas de espacios, seguimiento de rutinas de entrenamiento y control de ingresos, facilitando la toma de decisiones mediante reportes de movimientos financieros y auditorías de eventos.

## Objetivos y Alcance
### Objetivos:
*   Automatizar los procesos de reserva y facturación de servicios deportivos.
*   Proveer un control de acceso seguro y auditable para los socios.
*   Gestionar planes de entrenamiento personalizados mediante rutinas y ejercicios.
*   Asegurar la integridad y disponibilidad de la información mediante un robusto sistema de copias de seguridad.

### Alcance:
El sistema abarca desde la registración de nuevos clientes y la asignación de membresías hasta el control financiero diario. Incluye herramientas para la administración de la agenda de espacios, gestión de ejercicios para rutinas, procesamiento de pagos y generación de comprobantes.

## Características
*   **Arquitectura Robusta:** Basada en capas (N-Tier) para facilitar el mantenimiento y la escalabilidad.
*   **Gestión de Permisos:** Implementa un control de acceso basado en roles (RBAC) utilizando el patrón Composite.
*   **Auditoría Completa:** Registro detallado de acciones significativas mediante una bitácora de eventos.
*   **Multi-Idioma:** Soporte para internacionalización y cambio dinámico de idioma.
*   **Integridad de Datos:** Uso de transacciones y validaciones consistentes en la lógica de negocio.

## Funcionalidades Principales:
1.  **Gestión de Clientes y Membresías:** Registro de socios, estados de cuenta y planes de suscripción.
2.  **Agenda y Reservas:** Calendario interactivo para la reserva de turnos en diferentes espacios del establecimiento.
3.  **Módulo de Entrenamiento:** Creación y asignación de rutinas de ejercicios personalizadas para los socios.
4.  **Gestión Financiera:** Registro de pagos, generación de comprobantes y consulta de movimientos de caja.
5.  **Control de Ingresos:** Registro y validación de la entrada de socios al establecimiento.
6.  **Administración del Sistema:** Gestión de usuarios, roles, permisos y backups.

## Seguridad y Cumplimiento:
La seguridad es un pilar fundamental del sistema. Se implementa:
*   **RBAC (Role-Based Access Control):** Permisos granulares asignados a familias (roles) y patentes (permisos individuales).
*   **Cifrado:** Las credenciales sensibles son protegidas mediante algoritmos de hash.
*   **Bitácora:** Seguimiento de auditoría de todas las operaciones críticas realizadas por los usuarios.

## Resumen de la Arquitectura
El proyecto sigue un patrón de arquitectura en capas bien definido:
*   **Presentación (UI):** Desarrollada en Windows Forms, provee la interfaz de interacción con el usuario.
*   **Capa de Negocio (BLL):** Contiene la lógica central del sistema, servicios y validaciones. Utiliza DTOs para el transporte de datos.
*   **Capa de Datos (DAL):** Gestiona la persistencia de información utilizando los patrones Repository y Unit of Work.
*   **Dominio (Domain):** Define las entidades del negocio, enumeraciones y estructuras compartidas.
*   **Servicios Transversales (Service):** Ofrece funcionalidades comunes como seguridad, gestión de idiomas y bitácora.

## Tecnologías Utilizadas
*   **Lenguaje:** C#
*   **Framework:** .NET Framework 4.7.2
*   **Interfaz Gráfica:** Windows Forms (WinForms)
*   **Base de Datos:** Microsoft SQL Server
*   **Documentación:** DocFX
*   **Persistencia:** ADO.NET / SQL Nativo

## Conclusión
Este sistema proporciona una herramienta profesional y escalable para la administración de centros deportivos, garantizando un control total sobre las operaciones diarias, la seguridad de la información y la satisfacción del cliente a través de procesos automatizados y eficientes.

## Espacios de nombres
*   **UI:** Interfaz de usuario y lógica de formularios.
*   **BLL:** Servicios de lógica de negocio, Mappers y DTOs.
*   **DAL:** Contratos de repositorio e implementación de acceso a SQL.
*   **Domain:** Entidades principales y objetos de dominio.
*   **Service:** Lógica de infraestructura, seguridad y soporte.

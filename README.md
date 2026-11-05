 — Grupo 4

Sistema de Seguimiento Académico y Alerta Temprana

compilar: dotnet build -f net10.0-windows10.0.19041.0

correr: dotnet run -f net10.0-windows10.0.19041.0

![Diagrama de Estructura](App_v3/docs/diagrama%20de%20estructura%20Grupo4.jpg)


# Arquitectura y Estructura del Sistema (Grupo4)

## 1. Descripción General
Esta aplicación está desarrollada en **.NET MAUI** y está orientada al **seguimiento académico y alerta temprana** de estudiantes. El sistema permite al personal gestionar la matrícula de alumnos, administrar comisiones, registrar la asistencia diaria y realizar el seguimiento de entregas de trabajos prácticos.

A partir de estos datos, el motor de dominio evalúa de forma continua indicadores de riesgo académico y genera paneles de alerta, historiales de contacto y reportes detallados para intervenir a tiempo.

---

## 2. Componentes

### 2.1. Experiencia de la Aplicación
Capa de interfaz de usuario (ViewModels) que gestiona la interacción del personal con el sistema:
* **Gestión de Estudiantes:** `EstudiantesViewModel.cs`
* **Asistencia y Entregas:** `App_v3/ViewModels/`
* **Calendario:** `CalendarioViewModel.cs`
* **Informes:** `ReportesViewModels.cs`

### 2.2. Registros Académicos
Servicios de dominio dedicados a la lógica de negocio y estructura del dictado de clases:
* **Estudiantes e Inscripciones:** `InscripcionService.cs`
* **Asignaturas y Comisiones:** `ComisionService.cs`
* **Clases y Asignaciones:** `CalendarioService.cs`
* **Registros de Asistencia:** `AsistenciaService.cs`
* **Entregas de Tareas:** `EntregaService.cs`

### 2.3. Seguimiento y Análisis
Módulos encargados de procesar reglas de negocio para la prevención del desgranamiento escolar:
* **Historial de Contactos:** `ContactoService.cs`
* **Umbrales de Riesgo:** `UmbralesService.cs`
* **Panel de Riesgo y Perfil:** `SeguimientoService.cs`
* **Generación de Informes:** `ReporteService.cs`

### 2.4. Persistencia
Capa encargada de abstraer y almacenar la información del sistema en almacenamiento local:
* **Contrato del Repositorio:** `IRepositorio.cs` (Interfaz base)
* **Repositorio SQLite:** `RepositorioSqlite.cs` (Implementación concreta en base de datos local)

### 2.5. Otros Componentes
* **Servicio Central de Base de Datos:** `DatabaseService.cs` (Encargado de coordinar el acceso a datos entre la app y SQLite).

### 2.6. Roles e Interacciones Externas
* **Personal Administrativo / Docente:** Actores encargados de la carga de datos, revisión del calendario y consulta de paneles de alerta.

---

## 3. Conexiones

El flujo de información dentro de la arquitectura se organiza de la siguiente forma:

1. **Usuario del Personal → Experiencia de App:**
   * Registra asistencias y entregas.
   * Planifica clases e hitos en el calendario.
   * Revisa métricas e informes.

2. **ViewModels → Servicios de Dominio:**
   * La vista de estudiantes solicita diagnósticos al **Panel de Riesgo y Perfil**.
   * Las vistas de registro impactan directamente en **Asistencia**, **Calendario** y **Entregas**.

3. **Lógica de Dominio → Análisis:**
   * El módulo de **Panel de Riesgo** consulta los **Umbrales de Riesgo** y los registros de **Inscripciones**, **Asistencia** y **Entregas** para evaluar el nivel de alerta del estudiante.

4. **Dominio → Persistencia:**
   * Todos los servicios de dominio (`InscripcionService`, `AsistenciaService`, `EntregaService`, etc.) consumen la interfaz `IRepositorio` para guardar y consultar datos.
   * La interfaz `IRepositorio` se conecta mediante `RepositorioSqlite` / `DatabaseService` hacia la base de datos **SQLite**.

---

## 4. Aspectos Clave e Interesantes de la Arquitectura

* **Patrón MVVM Claro:** Separación limpia entre la capa de presentación (`ViewModels`), las reglas de negocio (`Core/Dominio`) y la infraestructura (`Plataforma/RepositorioSqlite`).
* **Desacoplamiento mediante Repositorio:** El uso de `IRepositorio` permite que la lógica de negocio no dependa directamente de SQLite, facilitando la ejecución de pruebas unitarias.
* **Motor de Alerta Temprana Independiente:** La lógica de cálculo de riesgo no está acoplada a la vista; el dominio procesa los datos de forma autónoma cada vez que se consulta el estado de un alumno.









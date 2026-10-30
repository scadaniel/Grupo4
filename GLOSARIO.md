# Glosario y modelo del dominio

Los nombres del código son los de las historias de usuario (`HISTORIAS.odt`).
Si una historia dice "estudiante", la clase se llama `Estudiante`, nunca "alumno" ni "persona".

## Vocabulario

| Término de las historias | Clase / nombre | Historias |
|---|---|---|
| Estudiante | `Estudiante` | H1, H9, H13 |
| Materia | `Materia` | H1, H2 |
| Comisión | `Comision` | todas |
| Inscripción | `Inscripcion` | H1, H13 |
| Clase (una fecha del calendario) | `ClaseDictada` | H2, H3, H4 |
| Trabajo práctico | `TrabajoPractico` | H2, H5 |
| Asistencia | `Asistencia` | H3, H4 |
| Entrega | `Entrega` | H5 |
| Umbrales | `UmbralesRiesgo` | H7 |
| Nivel de riesgo (Alto, Medio, Bajo) | `NivelRiesgo` | H6, H8 |
| Contacto | `Contacto` | H10, H11 |

`ClaseDictada` es la única diferencia con el texto de las historias: se llama así para no
confundirla con la materia ni con la comisión.

## Relaciones

```
Materia 1──N Comision 1──N ClaseDictada
                      1──N TrabajoPractico
Estudiante N──N Comision        (a través de Inscripcion)
Asistencia = (ClaseDictada, Estudiante)
Entrega    = (TrabajoPractico, Estudiante)
Contacto   = (Estudiante, Comision)
```

## Decisiones

- **Activo / inactivo vive en `Inscripcion`** (`Activa` y `FechaBaja`), no en `Estudiante`. Un estudiante
  puede abandonar una comisión y seguir en otra. La baja es lógica: no se borra nada (H1 CA 1.4, H13).
- **Legajo y DNI son únicos en todo el sistema**, no por comisión (H1 CA 1.2).
- **El riesgo no se guarda**: se calcula en el momento con `UmbralesRiesgo.CalcularNivel` a partir de las
  asistencias, las entregas y los umbrales. Es el único lugar donde se calcula (H6). Lo usan el panel de
  seguimiento (H8), la ficha (H9) y los reportes, siempre a través de `SeguimientoService`.
- **El dominio no conoce SQLite**: solo conoce `IRepositorio`. La app lo implementa con la base de datos.
- **Los umbrales son globales** por ahora (H7). Por comisión queda como mejora.
- **DNI**: solo números, 7 u 8 dígitos (se aceptan puntos y espacios al escribirlo). **Email**: formato `a@b.c`.

## Capas

```
App_v3.Core   (Dominio)  Entidades, reglas y servicios. No usa paquetes: ni SQLite ni MAUI.
App_v3        (App)      Pantallas (Views), ViewModels, SQLite (Plataforma), inyección de dependencias.
App_v3.Tests             Pruebas del dominio y de los ViewModels. Compila los ViewModels sin abrir la app.
```

Las dependencias van hacia el dominio: `App` conoce a `Core`, `Core` no conoce a nadie.

| Pieza | Dónde | Qué hace |
|---|---|---|
| `*Service` (`EstudianteService`, `AsistenciaService`, `SeguimientoService`...) | Core/Dominio | Reglas de cada historia. Reciben `IRepositorio` y `TimeProvider`. |
| `IRepositorio` | Core/Dominio | Lo único que el dominio pide a la persistencia. |
| `RepositorioSqlite` | App/Plataforma | Implementa `IRepositorio` con SQLite. Es el único archivo que conoce SQLite. |
| `ShellDialogService`, `ShellNavegacionService`, `LauncherEnlaceService` | App/Plataforma | Mensajes, navegación y mailto/tel. Los ViewModels solo ven las interfaces. |
| ViewModels | App/ViewModels | Sin MAUI: solo .NET. Reciben todo por el constructor. Probados en `App_v3.Tests`. |
| Pantallas | App/Views | XAML + un constructor que recibe su ViewModel. Sin lógica. |
| `MauiProgram` | App | Registra todo en el contenedor. Las pantallas nunca usan `new` para crear ViewModels. |

## Cada dato en un solo lugar

| Dato | Único lugar |
|---|---|
| Nivel de riesgo (Alto, Medio, Bajo) | `NivelRiesgo` + `UmbralesRiesgo.CalcularNivel`. No se guarda ni se copia como texto. |
| Cuándo una ausencia es inasistencia | `Asistencia.CuentaComoInasistencia` |
| Cuándo un TP está vencido / sin entregar | `ReglasDeEntrega` (Dominio/EstadoEntrega.cs) |
| Qué cuenta como contacto "reciente" (7 días) | `SeguimientoService` |
| Texto con que se muestra cada estado | `Textos` (ViewModels/Textos.cs) |
| Qué color corresponde a cada estado | `PaletaSemantica` (nombre del color) |
| Valor de cada color (código hexadecimal) | `Resources/Styles/Colors.xaml`: `ColorExito`, `ColorPeligro`, `ColorAdvertencia`, `ColorNeutro` |
| Validación de legajo, DNI y email | `ValidacionEstudiante` / `EstudianteService` |
| Formulario de contacto (panel y ficha, H10) | `FormularioContactoViewModel` |

## Validación de datos

Toda entrada se valida en el dominio y, si no es válida, se lanza `ReglaDeNegocioException`.
`BaseViewModel` la muestra como un mensaje "Atención" en lugar de cerrar la app. Ejemplos: alta de estudiante
con DNI no numérico, email inválido o legajo repetido; umbrales que no son números enteros o contradictorios;
asistencia ya registrada para esa clase; contacto sin fecha, resultado o nota.

## Pruebas

- `Dominio/`: una clase de pruebas por servicio, con casos límite (justo en el umbral, 7 días de contacto,
  estudiante dado de baja, fechas repetidas). Usan `RepositorioEnMemoria` y `RelojFijo` (fecha fija).
- `ViewModels/`: lo que ve el usuario en cada pantalla (orden del panel, textos, avisos, formularios),
  con diálogos, navegación y enlaces falsos.
- Cada prueba lleva el nombre del criterio de aceptación de la historia (`CA8_1_...`).
- Para ejecutarlas: `dotnet test App_v3/App_v3.Tests/App_v3.Tests.csproj`.

## Datos de ejemplo

En Configuración, "Cargar datos de ejemplo" reemplaza lo cargado por 2 materias, 3 comisiones, 12 estudiantes,
clases, asistencias, entregas y contactos con distintos niveles de riesgo. "Borrar todos los datos" deja la app vacía.
Ambos botones piden confirmación.

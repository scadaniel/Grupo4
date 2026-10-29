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
  asistencias, las entregas y los umbrales. Es el único lugar donde se calcula (H6).
- **El dominio no conoce SQLite**: solo conoce `IRepositorio`. La app lo implementa con la base de datos.
- **Los umbrales son globales** por ahora (H7). Por comisión queda como mejora.
- **DNI**: solo números, 7 u 8 dígitos (se aceptan puntos y espacios al escribirlo). **Email**: formato `a@b.c`.

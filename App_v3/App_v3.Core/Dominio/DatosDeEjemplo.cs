
using System.Globalization;
using System.Text;

namespace App_v3.Dominio;

/// <summary>
/// Datos de ejemplo para probar la aplicación: materias, comisiones, estudiantes, clases,
/// asistencias, entregas y contactos con niveles de riesgo variados. Reemplaza lo que haya cargado.
/// </summary>
public class DatosDeEjemplo(IRepositorio repositorio, TimeProvider reloj)
{
    private static readonly string[] Nombres =
    [
        "Martín", "Sofía", "Lucas", "Valentina", "Joaquín", "Camila",
        "Tomás", "Agustina", "Mateo", "Lucía", "Diego", "Elena",
        "Nicolás", "Florencia", "Santiago", "Milagros", "Franco", "Julieta",
        "Facundo", "Carolina", "Federico", "Rocío", "Bruno", "Micaela",
        "Gabriel", "Daniela", "Ignacio", "Paula", "Andrés", "Victoria"
    ];

    private static readonly string[] Apellidos =
    [
        "González", "Rodríguez", "Fernández", "López", "Martínez", "Sánchez",
        "Díaz", "Romero", "Álvarez", "Torres", "Ruiz", "Gómez",
        "Acosta", "Benítez", "Medina", "Herrera", "Suárez", "Aguirre",
        "Castro", "Molina", "Ortiz", "Silva", "Núñez", "Rojas",
        "Cabrera", "Vega", "Méndez", "Pereyra", "Flores", "Campos"
    ];

    // Cantidad de inasistencias y de TP entregados por estudiante.
    private sealed record Cursada(int[] Ausencias, int[] Entregados);

    public async Task CargarAsync()
    {
        await repositorio.VaciarAsync();

        var hoy = reloj.GetLocalNow().DateTime.Date;
        var comisiones = await CrearComisionesAsync();
        var estudiantes = await CrearEstudiantesAsync();

        // Distribución de los 30 estudiantes en las 7 comisiones.
        // Se registran tres bajas: 1P1, 2P2 y 1AS1.

        // 1P1: 5 inscriptos, 1 baja, 4 activos.
        await InscribirAsync(comisiones[0], estudiantes.Take(5),
            bajaDelUltimo: hoy.AddDays(-10));

        // 1P2: 4 inscriptos, 4 activos.
        await InscribirAsync(comisiones[1], estudiantes.Skip(5).Take(4),
            bajaDelUltimo: null);

        // 2BD1: 5 inscriptos, 5 activos.
        await InscribirAsync(comisiones[2], estudiantes.Skip(9).Take(5),
            bajaDelUltimo: null);

        // 2P1: 4 inscriptos, 4 activos.
        await InscribirAsync(comisiones[3], estudiantes.Skip(14).Take(4),
            bajaDelUltimo: null);

        // 2P2: 4 inscriptos, 1 baja, 3 activos.
        await InscribirAsync(comisiones[4], estudiantes.Skip(18).Take(4),
            bajaDelUltimo: hoy.AddDays(-10));

        // 1AS1: 4 inscriptos, 1 baja, 3 activos.
        await InscribirAsync(comisiones[5], estudiantes.Skip(22).Take(4),
            bajaDelUltimo: hoy.AddDays(-10));

        // 1AS2: 4 inscriptos, 4 activos.
        await InscribirAsync(comisiones[6], estudiantes.Skip(26).Take(4),
            bajaDelUltimo: null);

        // 1P1: 1 alto, 3 bajo y 1 baja.
        await CargarCursadaAsync(comisiones[0], estudiantes.Take(5).ToList(), hoy,
            new Cursada(
                [5, 0, 1, 2, 0],
                [3, 3, 3, 3, 3]));

        // 1P2: 1 medio y 3 bajo.
        await CargarCursadaAsync(comisiones[1], estudiantes.Skip(5).Take(4).ToList(), hoy,
            new Cursada(
                [0, 0, 1, 2],
                [2, 3, 3, 3]));

        // 2BD1: 1 alto, 1 medio y 3 bajo.
        await CargarCursadaAsync(comisiones[2], estudiantes.Skip(9).Take(5).ToList(), hoy,
            new Cursada(
                [0, 3, 0, 1, 2],
                [0, 3, 3, 3, 3]));

        // 2P1: 1 alto y 3 bajo.
        await CargarCursadaAsync(comisiones[3], estudiantes.Skip(14).Take(4).ToList(), hoy,
            new Cursada(
                [5, 0, 1, 2],
                [3, 3, 3, 3]));

        // 2P2: 1 medio, 2 bajo y 1 baja.
        await CargarCursadaAsync(comisiones[4], estudiantes.Skip(18).Take(4).ToList(), hoy,
            new Cursada(
                [0, 0, 1, 2],
                [1, 3, 3, 3]));

        // 1AS1: 1 alto, 2 bajo y 1 baja.
        await CargarCursadaAsync(comisiones[5], estudiantes.Skip(22).Take(4).ToList(), hoy,
            new Cursada(
                [0, 0, 1, 2],
                [0, 3, 3, 3]));

        // 1AS2: 1 medio y 3 bajo.
        await CargarCursadaAsync(comisiones[6], estudiantes.Skip(26).Take(4).ToList(), hoy,
            new Cursada(
                [3, 0, 1, 2],
                [2, 3, 3, 3]));

        // Contactos de dos estudiantes activos de 1P1.
        await CargarContactosAsync(comisiones[0], estudiantes.Take(5).ToList(), hoy);
    }

    private async Task<List<Comision>> CrearComisionesAsync()
    {
        var programacion = await GuardarAsync(new Materia { Nombre = "Programación I" });
        var bases = await GuardarAsync(new Materia { Nombre = "Bases de Datos" });
        var programacionII = await GuardarAsync(new Materia { Nombre = "Programación II" });
        var analisis = await GuardarAsync(new Materia { Nombre = "Análisis de Sistemas" });

        return
        [
            await GuardarAsync(new Comision { MateriaId = programacion.Id, Nombre = "1P1" }),
            await GuardarAsync(new Comision { MateriaId = programacion.Id, Nombre = "1P2" }),
            await GuardarAsync(new Comision { MateriaId = bases.Id, Nombre = "1BD1" }),
            await GuardarAsync(new Comision { MateriaId = programacionII.Id, Nombre = "2P1" }),
            await GuardarAsync(new Comision { MateriaId = programacionII.Id, Nombre = "2P2" }),
            await GuardarAsync(new Comision { MateriaId = analisis.Id, Nombre = "1AS1" }),
            await GuardarAsync(new Comision { MateriaId = analisis.Id, Nombre = "1AS2" })
        ];
    }

    private async Task<List<Estudiante>> CrearEstudiantesAsync()
    {
        var estudiantes = new List<Estudiante>();

        for (var i = 0; i < Nombres.Length; i++)
            estudiantes.Add(await GuardarAsync(NuevoEstudiante(i)));

        return estudiantes;
    }

    private static Estudiante NuevoEstudiante(int indice) => new()
    {
        Legajo = $"L-{1001 + indice}",
        Dni = $"4011{1001 + indice}",
        Nombre = Nombres[indice],
        Apellido = Apellidos[indice],
        Email = SinTildes($"{Nombres[indice]}.{Apellidos[indice]}@ejemplo.com").ToLowerInvariant(),
        Telefono = $"11-5555-{1001 + indice}"
    };

    private static string SinTildes(string texto) =>
        new(texto.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

    private async Task InscribirAsync(
        Comision comision,
        IEnumerable<Estudiante> estudiantes,
        DateTime? bajaDelUltimo)
    {
        var lista = estudiantes.ToList();

        foreach (var estudiante in lista)
        {
            var esElDeLaBaja = bajaDelUltimo is not null && estudiante == lista[^1];

            await GuardarAsync(new Inscripcion
            {
                EstudianteId = estudiante.Id,
                ComisionId = comision.Id,
                Activa = !esElDeLaBaja,
                FechaBaja = esElDeLaBaja ? bajaDelUltimo : null
            });
        }
    }

    private async Task CargarCursadaAsync(
        Comision comision,
        List<Estudiante> estudiantes,
        DateTime hoy,
        Cursada cursada)
    {
        var clases = await CrearClasesAsync(comision, hoy);
        var trabajos = await CrearTrabajosAsync(comision, hoy);

        for (var k = 0; k < estudiantes.Count; k++)
        {
            await RegistrarAsistenciasAsync(clases, estudiantes[k], cursada.Ausencias[k]);
            await RegistrarEntregasAsync(trabajos, estudiantes[k], cursada.Entregados[k]);
        }
    }

    

    // Cinco clases ya dictadas, una por semana, y una futura sin asistencia.
private async Task<List<ClaseDictada>> CrearClasesAsync(
    Comision comision,
    DateTime hoy)
{
    var clases = new List<ClaseDictada>();

    var materias = await repositorio.ListarAsync<Materia>();
    var materia = materias.First(m => m.Id == comision.MateriaId);

    string[] temas = materia.Nombre switch
    {
        "Programación I" =>
        [
            "Introducción a la programación",
            "Variables y tipos de datos",
            "Estructuras condicionales",
            "Estructuras repetitivas",
            "Funciones y métodos",
            "Listas y colecciones"
        ],

        "Bases de Datos" =>
        [
            "Introducción a las bases de datos",
            "Modelo entidad-relación",
            "Tablas y relaciones",
            "Consultas SQL",
            "Claves primarias y foráneas",
            "Joins y consultas relacionadas"
        ],

        "Programación II" =>
        [
            "Repaso de programación",
            "Programación orientada a objetos",
            "Clases y objetos",
            "Herencia y polimorfismo",
            "Colecciones y excepciones",
            "Persistencia de datos"
        ],

        "Análisis de Sistemas" =>
        [
            "Introducción al análisis de sistemas",
            "Relevamiento de requerimientos",
            "Casos de uso",
            "Diagramas UML",
            "Modelado de procesos",
            "Diseño de sistemas"
        ],

        _ => throw new InvalidOperationException(
            $"No hay temas definidos para la materia {materia.Nombre}.")
    };

    int[] dias = [-35, -28, -21, -14, -7, 7];

    for (var i = 0; i < temas.Length; i++)
    {
        clases.Add(await GuardarAsync(new ClaseDictada
        {
            ComisionId = comision.Id,
            Numero = i + 1,
            Tema = temas[i],
            Fecha = hoy.AddDays(dias[i])
        }));
    }

    return clases;
}


    // Tres trabajos con la fecha límite vencida y uno todavía en plazo.
    private async Task<List<TrabajoPractico>> CrearTrabajosAsync(
        Comision comision,
        DateTime hoy)
    {
        var trabajos = new List<TrabajoPractico>();
        int[] dias = [-28, -14, -3, 14];

        for (var i = 0; i < dias.Length; i++)
        {
            trabajos.Add(await GuardarAsync(new TrabajoPractico
            {
                ComisionId = comision.Id,
                Nombre = $"TP {i + 1}",
                FechaLimite = hoy.AddDays(dias[i])
            }));
        }

        return trabajos;
    }

    // El estudiante falta a las primeras clases ya dictadas.
    private async Task RegistrarAsistenciasAsync(
        List<ClaseDictada> clases,
        Estudiante estudiante,
        int ausencias)
    {
        for (var c = 0; c < 5; c++)
        {
            await GuardarAsync(new Asistencia
            {
                ClaseDictadaId = clases[c].Id,
                EstudianteId = estudiante.Id,
                Presente = c >= ausencias
            });
        }
    }

    // Registra los tres TP vencidos. El TP en plazo queda sin registro.
    private async Task RegistrarEntregasAsync(
        List<TrabajoPractico> trabajos,
        Estudiante estudiante,
        int entregados)
    {
        for (var t = 0; t < 3; t++)
        {
            await GuardarAsync(new Entrega
            {
                TrabajoPracticoId = trabajos[t].Id,
                EstudianteId = estudiante.Id,
                Entregado = t < entregados
            });
        }
    }

    // Un contacto reciente y uno viejo, para probar el filtro
    // "No contactados esta semana".
    private async Task CargarContactosAsync(
        Comision comision,
        List<Estudiante> estudiantes,
        DateTime hoy)
    {
        await GuardarAsync(NuevoContacto(
            comision,
            estudiantes[2],
            hoy.AddDays(-3),
            MedioContacto.Llamada,
            "Respondió",
            "Se comprometió a entregar el TP 3."));

        await GuardarAsync(NuevoContacto(
            comision,
            estudiantes[3],
            hoy.AddDays(-12),
            MedioContacto.Email,
            "No respondió",
            "Se reenviará el mensaje."));
    }

    private static Contacto NuevoContacto(
        Comision comision,
        Estudiante estudiante,
        DateTime fecha,
        MedioContacto medio,
        string resultado,
        string nota) => new()
        {
            ComisionId = comision.Id,
            EstudianteId = estudiante.Id,
            Fecha = fecha,
            Medio = medio,
            Resultado = resultado,
            Nota = nota
        };

    private async Task<T> GuardarAsync<T>(T entidad)
        where T : class, IEntidad, new()
    {
        await repositorio.GuardarAsync(entidad);
        return entidad;
    }
}

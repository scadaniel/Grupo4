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
        ["Martín", "Sofía", "Lucas", "Valentina", "Joaquín", "Camila", "Tomás", "Agustina", "Mateo", "Lucía", "Diego", "Elena"];

    private static readonly string[] Apellidos =
        ["González", "Rodríguez", "Fernández", "López", "Martínez", "Sánchez", "Díaz", "Romero", "Álvarez", "Torres", "Ruiz", "Gómez"];

    // Una cursada de ejemplo: cuántas clases faltó cada estudiante y cuántos de los trabajos vencidos entregó.
    private sealed record Cursada(int[] Ausencias, int[] Entregados);

    public async Task CargarAsync()
    {
        await repositorio.VaciarAsync();
        var hoy = reloj.GetLocalNow().DateTime.Date;
        var comisiones = await CrearComisionesAsync();
        var estudiantes = await CrearEstudiantesAsync();

        await InscribirAsync(comisiones[0], estudiantes.Take(9), bajaDelUltimo: hoy.AddDays(-10));
        await InscribirAsync(comisiones[1], estudiantes.Skip(9), bajaDelUltimo: null);
        await InscribirAsync(comisiones[2], estudiantes.Skip(3).Take(5), bajaDelUltimo: null);

        await CargarCursadaAsync(comisiones[0], estudiantes.Take(9).ToList(), hoy,
            new Cursada([0, 1, 2, 3, 5, 5, 0, 1, 4], [3, 3, 2, 1, 0, 2, 1, 3, 1]));
        await CargarCursadaAsync(comisiones[2], estudiantes.Skip(3).Take(5).ToList(), hoy,
            new Cursada([1, 0, 3, 0, 2], [3, 3, 1, 2, 3]));
        await CargarContactosAsync(comisiones[0], estudiantes, hoy);
    }

    private async Task<List<Comision>> CrearComisionesAsync()
    {
        var programacion = await GuardarAsync(new Materia { Nombre = "Programación I" });
        var bases = await GuardarAsync(new Materia { Nombre = "Bases de Datos" });

        return
        [
            await GuardarAsync(new Comision { MateriaId = programacion.Id, Nombre = "1K1" }),
            await GuardarAsync(new Comision { MateriaId = programacion.Id, Nombre = "1K2" }),
            await GuardarAsync(new Comision { MateriaId = bases.Id, Nombre = "2K1" })
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

    private async Task InscribirAsync(Comision comision, IEnumerable<Estudiante> estudiantes, DateTime? bajaDelUltimo)
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

    private async Task CargarCursadaAsync(Comision comision, List<Estudiante> estudiantes, DateTime hoy, Cursada cursada)
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
    private async Task<List<ClaseDictada>> CrearClasesAsync(Comision comision, DateTime hoy)
    {
        var clases = new List<ClaseDictada>();
        string[] temas = ["Introducción", "Variables", "Condicionales", "Ciclos", "Funciones", "Listas"];
        int[] dias = [-35, -28, -21, -14, -7, 7];
        for (var i = 0; i < temas.Length; i++)
            clases.Add(await GuardarAsync(new ClaseDictada
            {
                ComisionId = comision.Id, Numero = i + 1, Tema = temas[i], Fecha = hoy.AddDays(dias[i])
            }));
        return clases;
    }

    // Tres trabajos con la fecha límite vencida y uno todavía en plazo.
    private async Task<List<TrabajoPractico>> CrearTrabajosAsync(Comision comision, DateTime hoy)
    {
        var trabajos = new List<TrabajoPractico>();
        int[] dias = [-28, -14, -3, 14];
        for (var i = 0; i < dias.Length; i++)
            trabajos.Add(await GuardarAsync(new TrabajoPractico
            {
                ComisionId = comision.Id, Nombre = $"TP {i + 1}", FechaLimite = hoy.AddDays(dias[i])
            }));
        return trabajos;
    }

    // El estudiante falta a las primeras clases ya dictadas; la clase futura queda sin asistencia.
    private async Task RegistrarAsistenciasAsync(List<ClaseDictada> clases, Estudiante estudiante, int ausencias)
    {
        for (var c = 0; c < 5; c++)
            await GuardarAsync(new Asistencia { ClaseDictadaId = clases[c].Id, EstudianteId = estudiante.Id, Presente = c >= ausencias });
    }

    // Entregó los primeros trabajos vencidos; el que está en plazo no tiene registro todavía.
    private async Task RegistrarEntregasAsync(List<TrabajoPractico> trabajos, Estudiante estudiante, int entregados)
    {
        for (var t = 0; t < 3; t++)
            await GuardarAsync(new Entrega { TrabajoPracticoId = trabajos[t].Id, EstudianteId = estudiante.Id, Entregado = t < entregados });
    }

    // Un contacto reciente y uno viejo, para probar el filtro "No contactados esta semana".
    private async Task CargarContactosAsync(Comision comision, List<Estudiante> estudiantes, DateTime hoy)
    {
        await GuardarAsync(NuevoContacto(comision, estudiantes[4], hoy.AddDays(-3), MedioContacto.Llamada, "Respondió", "Se comprometió a entregar el TP 3."));
        await GuardarAsync(NuevoContacto(comision, estudiantes[5], hoy.AddDays(-12), MedioContacto.Email, "No respondió", "Se reenviará el mensaje."));
    }

    private static Contacto NuevoContacto(Comision comision, Estudiante estudiante, DateTime fecha, MedioContacto medio, string resultado, string nota) => new()
    {
        ComisionId = comision.Id, EstudianteId = estudiante.Id, Fecha = fecha, Medio = medio, Resultado = resultado, Nota = nota
    };

    private async Task<T> GuardarAsync<T>(T entidad) where T : class, IEntidad, new()
    {
        await repositorio.GuardarAsync(entidad);
        return entidad;
    }
}

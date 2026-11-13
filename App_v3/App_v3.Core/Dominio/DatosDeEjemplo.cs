
using System.Globalization;
using System.Text;


namespace App_v3.Dominio;


public class DatosDeEjemplo(IRepositorio repositorio, TimeProvider reloj)
{
    private static readonly string[] ListaMujeres =
    [
        "Lucía González", "Camila Benítez", "Martina Duarte", "Valentina Rossi", "Sofía Ferreira",
        "María Belén Giménez", "Florencia Villalba", "Julieta Cardozo", "Natalia Ojeda", "Micaela Martínez",
        "Victoria Pereyra", "Romina Almada", "Sabrina Insaurralde", "Mariana Acuña", "Daniela Bogado",
        "Agustina Vera", "Carolina Méndez", "Andrea Fernández", "Paula Galeano", "Luciana Maidana",
        "Noel Cabrera", "Rocío Rivero", "Antonella Maciel", "Brenda Silvero", "Estefanía Valdez",
        "Valeria Godoy", "María José Britos", "Noelia Franco", "Sol Viera", "Daiana Centurión",
        "Gimena Olivera", "Celeste Fleitas", "Candela Sosa", "Carla Rolón", "Georgina Escobar",
        "Melina Ayala", "Pamela Cáceres", "Yazmín Riquelme", "Lourdes Cristaldo", "Milagros Sanabria",
        "Evelyn Soria", "Tamara Núñez", "Tamara Benítez", "Guillermina Ledesma", "Patricia Bareiro",
        "Laura Cantero", "Ana Clara Larrosa", "Lorena Paredes", "Gabriela Servín", "Yamila Amarilla",
        "Dahiana Denis", "Sabrina Delgado", "Dahiana Ortiz", "Yanina Ortiz", "Nicole Leiva",
        "Malena Bustos", "María Paz Espínola", "Araceli Galeano", "Jessica Fernández", "Paola Domínguez",
        "Cynthia Chaparro", "Laura Barrios", "Macarena Ríos", "Florencia Varela", "Jimena Leguizamón",
        "Melani Oviedo", "Johana Aquino", "Mónica Caballero", "María Elena Cabañas", "Nadia Portillo",
        "Karen Monges", "Claudia Florentín", "Silvina Ramos", "Viviana Rotela", "Tamara Giménez",
        "Mercedes D'Angelo", "Adriana Alvarenga", "Josefina Saravia", "Belén Samudio", "María Luján Maidana"
    ];


    private static readonly string[] ListaHombres =
    [
        "Mateo González", "Joaquín Benítez", "Lucas Duarte", "Santiago Rossi", "Tomás Ferreira",
        "Nicolás Giménez", "Agustín Villalba", "Enzo Cardozo", "Matías Ojeda", "Franco Martínez",
        "Maximiliano Pereyra", "Bruno Almada", "Rodrigo Insaurralde", "Juan Pablo Acuña", "Diego Bogado",
        "Gonzalo Vera", "Ignacio Méndez", "Facundo Fernández", "Thiago Galeano", "Gabriel Maidana",
        "Sebastián Cabrera", "Federico Rivero", "Emiliano Maciel", "Bautista Silvero", "Leandro Valdez",
        "Nahuel Godoy", "Julián Britos", "Emanuel Franco", "Ramiro Viera", "Valentín Centurión",
        "Damián Olivera", "Alan Fleitas", "Marcos Sosa", "Esteban Rolón", "Luciano Escobar",
        "Iván Ayala", "Alex Cáceres", "Brian Riquelme", "Cristian Cristaldo", "Kevin Sanabria",
        "Gustavo Soria", "Hernán Núñez", "Fabricio Ledesma", "Carlos Bareiro", "Jorge Cantero",
        "Juan Manuel Larrosa", "Martín Paredes", "Víctor Servín", "Adrián Amarilla", "Pablo Denis",
        "Guillermo Delgado", "Matías Ortiz", "Oscar Samudio", "Néstor Leiva", "Lautaro Bustos",
        "Felipe Espínola", "Alexis Galeano", "Jonathan Fernández", "Mauricio Domínguez", "Claudio Chaparro",
        "Marcelo Barrios", "Darío Ríos", "Daniel Varela", "Alejandro Leguizamón", "Fernando Oviedo",
        "Ariel Aquino", "Raúl Caballero", "Aldo Cabañas", "César Portillo", "Osvaldo Monges",
        "Sergio Florentín", "Mario Ramos", "Hugo Rotela", "Walter Giménez", "Gonzalo D'Angelo",
        "Julio Alvarenga", "Alfredo Saravia", "Roberto Samudio", "Nelson Maidana", "Juan Carlos Ferreyra"
    ];


    public async Task CargarAsync()
    {
        await repositorio.VaciarAsync();
        var hoy = reloj.GetLocalNow().DateTime.Date;


        // 1. Generar 4 planteles de 45 estudiantes únicos por comisión
        var estudiantes10111 = await CrearEstudiantesRangoAsync(45, 10001, 0);
        var estudiantes10112 = await CrearEstudiantesRangoAsync(45, 10501, 45);
        var estudiantes20111 = await CrearEstudiantesRangoAsync(45, 20001, 90);
        var estudiantes20112 = await CrearEstudiantesRangoAsync(45, 20501, 135);


        // ==========================================
        // CREACIÓN DE MATERIAS (Las 7 en común y las específicas)
        // ==========================================

        // Troncales / En común
        var matTecnicas = await GuardarAsync(new Materia { Nombre = "Técnicas de Programación" });
        var matBaseDatos = await GuardarAsync(new Materia { Nombre = "Administración de Base de Datos" });
        var matMatematica = await GuardarAsync(new Materia { Nombre = "Elementos de Análisis Matemático" });
        var matLogica = await GuardarAsync(new Materia { Nombre = "Lógica Computacional" });
        var matPoo = await GuardarAsync(new Materia { Nombre = "Desarrollo de Sistemas Orientado a Objetos" });
        var matModelado = await GuardarAsync(new Materia { Nombre = "Modelado y Diseño de Software" });
        var matIngles = await GuardarAsync(new Materia { Nombre = "Inglés" });


        // Específicas / Variantes de planes
        var matEstadisticaSoft = await GuardarAsync(new Materia { Nombre = "Estadística y Probabilidad (Software)" });
        var matEstadisticaSys = await GuardarAsync(new Materia { Nombre = "Estadística y probabilidades (Sistemas)" });
        var matPpSoft = await GuardarAsync(new Materia { Nombre = "PP I : Campo Profesional (Software)" });
        var matPpSys = await GuardarAsync(new Materia { Nombre = "PP I : Campo Laboral (Sistemas)" });


        // ==========================================
        // COMISIÓN 10111 (Desarrollo de Software - 1er Cuatrimestre)
        // ==========================================
        foreach (var mat in new[] { matTecnicas, matBaseDatos, matMatematica, matLogica })
        {
            var com = await GuardarAsync(new Comision { MateriaId = mat.Id, Nombre = "10111" });
            await InscribirYCurarAsync(com, estudiantes10111, hoy);
        }


        // ==========================================
        // COMISIÓN 10112 (Desarrollo de Software - 2do Cuatrimestre)
        // ==========================================
        foreach (var mat in new[] { matPoo, matModelado, matEstadisticaSoft, matIngles, matPpSoft })
        {
            var com = await GuardarAsync(new Comision { MateriaId = mat.Id, Nombre = "10112" });
            await InscribirYCurarAsync(com, estudiantes10112, hoy);
        }


        // ==========================================
        // COMISIÓN 20111 (Análisis de Sistemas - 1er Cuatrimestre)
        // (Comparte las mismas materias de 1er cuatrimestre pero con su propia cohorte)
        // ==========================================
        foreach (var mat in new[] { matTecnicas, matBaseDatos, matMatematica, matLogica })
        {
            var com = await GuardarAsync(new Comision { MateriaId = mat.Id, Nombre = "20111" });
            await InscribirYCurarAsync(com, estudiantes20111, hoy);
        }


        // ==========================================
        // COMISIÓN 20112 (Análisis de Sistemas - 2do Cuatrimestre)
        // ==========================================
        foreach (var mat in new[] { matPoo, matModelado, matEstadisticaSys, matIngles, matPpSys })
        {
            var com = await GuardarAsync(new Comision { MateriaId = mat.Id, Nombre = "20112" });
            await InscribirYCurarAsync(com, estudiantes20112, hoy);
        }


        // H12: comisiones de demostración del reporte de estudiantes en riesgo
        await CargarDemoRiesgoAsync(hoy);
    }


    // ==========================================================================
    // H12 · DEMO DEL REPORTE DE ESTUDIANTES EN RIESGO
    // Datos fijos (sin Random) para que los números de la demo sean siempre los mismos.
    // Con los umbrales por defecto (inasistencias 3/5, TPs sin entregar 1/3):
    //   1°A: 30 estudiantes activos, 7 en riesgo (2 Alto y 5 Medio, ver PerfilDe1A) -> "7 de 30".
    //   1°B: 30 estudiantes inscriptos, sin clases ni TPs y SIN ninguna asistencia
    //        ni entrega registrada -> "todavía no hay datos".
    // No se toca el modelo ni el cálculo de riesgo: solo se cargan registros.
    // ==========================================================================
    private async Task CargarDemoRiesgoAsync(DateTime hoy)
    {
        var materia = await GuardarAsync(new Materia { Nombre = "Demo H12 · Riesgo de abandono" });

        await CargarComision1AAsync(materia, hoy);
        await CargarComision1BAsync(materia, hoy);
    }

    private async Task CargarComision1AAsync(Materia materia, DateTime hoy)
    {
        var comision = await GuardarAsync(new Comision { MateriaId = materia.Id, Nombre = "1°A" });
        var estudiantes = await CrearEstudiantesRangoAsync(30, 30001, 60);
        var clases = await CrearClasesAsync(comision, hoy);
        var trabajos = await CrearTrabajosAsync(comision, hoy);

        for (var i = 0; i < estudiantes.Count; i++)
        {
            var (ausentes, justificadas, entregados) = PerfilDe1A(i);
            await InscribirAsync(comision, estudiantes[i]);
            await RegistrarAsistenciasPorClaseAsync(clases, estudiantes[i], ausentes, justificadas);
            await RegistrarEntregasAsync(trabajos, estudiantes[i], entregados);
        }
    }

    private async Task CargarComision1BAsync(Materia materia, DateTime hoy)
    {
        var comision = await GuardarAsync(new Comision { MateriaId = materia.Id, Nombre = "1°B" });
        var estudiantes = await CrearEstudiantesRangoAsync(30, 30501, 100);

        // Comisión recién creada: tiene sus estudiantes inscriptos y nada más. Sin clases, sin trabajos prácticos
        // y sin asistencias ni entregas registradas (CA3). Así no hay nada que sume al riesgo en el panel.
        foreach (var estudiante in estudiantes)
            await InscribirAsync(comision, estudiante);
    }

    // Qué clases (de las 5 ya dictadas, numeradas desde 0) faltó cada estudiante, si las faltas fueron
    // justificadas y cuántos de los 3 TPs vencidos entregó. Los 7 primeros (L-30001 a L-30007) están en riesgo.
    private static (int[] Ausentes, bool Justificadas, int Entregados) PerfilDe1A(int i) => i switch
    {
        // Tres faltas cada uno (nivel Medio), pero en momentos distintos. El cálculo actual no los distingue.
        0 => (new[] { 2, 3, 4 }, false, 3),   // L-30001: faltó a las últimas tres clases
        1 => (new[] { 0, 2, 4 }, false, 3),   // L-30002: tres faltas salteadas
        2 => (new[] { 0, 1, 2 }, false, 3),   // L-30003: tres faltas hace un mes y después vino siempre

        3 => (new[] { 0, 1, 2, 3, 4 }, false, 3),   // L-30004: 5 faltas -> Alto
        4 => (Array.Empty<int>(), false, 0),        // L-30005: 3 TPs sin entregar -> Alto
        5 => (Array.Empty<int>(), false, 2),        // L-30006: 1 TP sin entregar -> Medio
        6 => (new[] { 0, 1, 3, 4 }, false, 3),     // L-30007: 4 faltas -> Medio

        // 4 ausencias, pero justificadas: no suman al riesgo (CA 3.5) -> Bajo
        7 => (new[] { 0, 1, 2, 3 }, true, 3),

        // El resto: 0, 1 o 2 faltas y todos los TPs entregados -> Bajo
        _ => (Enumerable.Range(0, i % 3).ToArray(), false, 3)
    };

    private Task InscribirAsync(Comision comision, Estudiante estudiante) =>
        GuardarAsync(new Inscripcion
        {
            EstudianteId = estudiante.Id,
            ComisionId = comision.Id,
            Activa = true,
            FechaBaja = null
        });

    private async Task RegistrarAsistenciasPorClaseAsync(
        List<ClaseDictada> clases, Estudiante estudiante, int[] ausentes, bool justificadas)
    {
        for (var c = 0; c < 5; c++)
        {
            var ausente = ausentes.Contains(c);
            await GuardarAsync(new Asistencia
            {
                ClaseDictadaId = clases[c].Id,
                EstudianteId = estudiante.Id,
                Presente = !ausente,
                Justificada = ausente && justificadas
            });
        }
    }


    private async Task<List<Estudiante>> CrearEstudiantesRangoAsync(int cantidad, int legajoBase, int indiceInicio)
    {
        var lista = new List<Estudiante>();
        var nombresCompletos = MezclarListas(ListaMujeres, ListaHombres);


        for (var i = 0; i < cantidad; i++)
        {
            int index = (indiceInicio + i) % nombresCompletos.Length;
            SepararNombreApellido(nombresCompletos[index], out var nombre, out var apellido);


            var est = await GuardarAsync(new Estudiante
            {
                Legajo = $"L-{legajoBase + i}",
                Dni = $"4{legajoBase + i:D7}".Substring(0, 8),
                Nombre = nombre,
                Apellido = apellido,
                Email = $"{SinTildes(nombre).ToLowerInvariant().Replace(" ", string.Empty)}.{SinTildes(apellido).ToLowerInvariant()}{legajoBase + i}@ejemplo.com",
                Telefono = $"11-5555-{(legajoBase + i)}"
            });
            lista.Add(est);
        }
        return lista;
    }


    private static void SepararNombreApellido(string completo, out string nombre, out string apellido)
    {
        var partes = completo.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length <= 1)
        {
            nombre = completo;
            apellido = "SinApellido";
            return;
        }
        apellido = partes[^1];
        nombre = string.Join(" ", partes[..^1]);
    }


    private static string[] MezclarListas(string[] mujeres, string[] hombres)
    {
        var combinadas = new string[mujeres.Length + hombres.Length];
        int m = 0, h = 0, c = 0;
        while (m < mujeres.Length || h < hombres.Length)
        {
            if (m < mujeres.Length) combinadas[c++] = mujeres[m++];
            if (h < hombres.Length) combinadas[c++] = hombres[h++];
        }
        return combinadas;
    }


    private async Task InscribirYCurarAsync(Comision comision, List<Estudiante> estudiantes, DateTime hoy)
    {
        var clases = await CrearClasesAsync(comision, hoy);
        var trabajos = await CrearTrabajosAsync(comision, hoy);
        Random rand = new(comision.Id + comision.MateriaId);


        foreach (var estudiante in estudiantes)
        {
            // El último estudiante de cada plantel está dado de baja (baja lógica, H13): conserva su historial.
            var esBaja = estudiante == estudiantes[^1];

            await GuardarAsync(new Inscripcion
            {
                EstudianteId = estudiante.Id,
                ComisionId = comision.Id,
                Activa = !esBaja,
                FechaBaja = esBaja ? hoy.AddDays(-10) : null
            });

            // Los tres primeros del plantel ya fueron contactados esta semana (H10 y H11); el resto, no.
            if (estudiantes.IndexOf(estudiante) < 3)
            {
                await GuardarAsync(new Contacto
                {
                    EstudianteId = estudiante.Id,
                    ComisionId = comision.Id,
                    Fecha = hoy.AddDays(-2),
                    Medio = MedioContacto.Llamada,
                    Resultado = "Atendió",
                    Nota = "Contacto de ejemplo"
                });
            }


            int ausencias = rand.Next(0, 6);
            int entregados = rand.Next(1, 4);


            await RegistrarAsistenciasAsync(clases, estudiante, ausencias);
            await RegistrarEntregasAsync(trabajos, estudiante, entregados);
        }
    }


    private async Task<List<ClaseDictada>> CrearClasesAsync(Comision comision, DateTime hoy)
    {
        var clases = new List<ClaseDictada>();
        int[] dias = [-35, -28, -21, -14, -7, 7];
        for (var i = 0; i < dias.Length; i++)
        {
            clases.Add(await GuardarAsync(new ClaseDictada
            {
                ComisionId = comision.Id,
                Numero = i + 1,
                Tema = $"Unidad Temática {i + 1}",
                Fecha = hoy.AddDays(dias[i])
            }));
        }
        return clases;
    }


    private async Task<List<TrabajoPractico>> CrearTrabajosAsync(Comision comision, DateTime hoy)
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


    private async Task RegistrarAsistenciasAsync(List<ClaseDictada> clases, Estudiante estudiante, int ausencias)
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


    private async Task RegistrarEntregasAsync(List<TrabajoPractico> trabajos, Estudiante estudiante, int entregados)
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


    private static string SinTildes(string texto) =>
        new(texto.Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());


    private async Task<T> GuardarAsync<T>(T entidad) where T : class, IEntidad, new()
    {
        await repositorio.GuardarAsync(entidad);
        return entidad;
    }
}


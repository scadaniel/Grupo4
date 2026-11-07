
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
                Email = $"{SinTildes(nombre).ToLowerInvariant()}.{SinTildes(apellido).ToLowerInvariant()}{legajoBase + i}@ejemplo.com",
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
            await GuardarAsync(new Inscripcion
            {
                EstudianteId = estudiante.Id,
                ComisionId = comision.Id,
                Activa = true,
                FechaBaja = null
            });


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


using System.Globalization;

namespace App_v3.Dominio;

/// <summary>
/// Importa historiales masivos desde CSV (2023, 2024, 2025) en la comisión seleccionada,
/// registrando a los estudiantes como inactivos (Activa = false) con su historial intacto.
/// </summary>
public class ImportadorCsvService(IRepositorio repositorio)
{
    public async Task<int> ImportarHistorialAsync(int comisionId, Stream flujoArchivo)
    {
        var comisiones = await repositorio.ListarAsync<Comision>();
        if (!comisiones.Any(c => c.Id == comisionId))
            throw new ReglaDeNegocioException("Seleccione una materia y comisión válida en la barra superior antes de importar.");

        using var lector = new StreamReader(flujoArchivo, System.Text.Encoding.UTF8);
        var cabecera = await lector.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(cabecera))
            throw new ReglaDeNegocioException("El archivo CSV está vacío.");

        // Cachés en memoria para optimizar la carga de las 300 filas
        var estudiantesPorDni = (await repositorio.ListarAsync<Estudiante>())
            .ToDictionary(e => e.Dni, StringComparer.OrdinalIgnoreCase);
        
        var inscripcionesExistentes = (await repositorio.ListarAsync<Inscripcion>())
            .Where(i => i.ComisionId == comisionId)
            .ToDictionary(i => i.EstudianteId);

        var clasesExistentes = (await repositorio.ListarAsync<ClaseDictada>())
            .Where(c => c.ComisionId == comisionId)
            .ToDictionary(c => c.Fecha.Date);

        var trabajosExistentes = (await repositorio.ListarAsync<TrabajoPractico>())
            .Where(t => t.ComisionId == comisionId)
            .ToDictionary(t => $"{t.Nombre}|{t.FechaLimite:yyyy-MM-dd}", StringComparer.OrdinalIgnoreCase);

        int numeroClaseSecuencial = clasesExistentes.Count == 0 ? 1 : clasesExistentes.Values.Max(c => c.Numero) + 1;
        int importados = 0;

        while (!lector.EndOfStream)
        {
            var linea = await lector.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(linea)) continue;

            var cols = linea.Split(';');
            if (cols.Length < 37) continue;

            string legajo = cols[0].Trim();
            string dni = ValidacionEstudiante.LimpiarDni(cols[1]);
            string nombre = cols[2].Trim();
            string apellido = cols[3].Trim();
            string email = cols[4].Trim();
            string telefono = cols[5].Trim();
            DateTime fechaBaja = DateTime.ParseExact(cols[7].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

            // 1. Obtener o crear el Estudiante global
            if (!estudiantesPorDni.TryGetValue(dni, out var estudiante))
            {
                estudiante = new Estudiante
                {
                    Legajo = legajo,
                    Dni = dni,
                    Nombre = nombre,
                    Apellido = apellido,
                    Email = email,
                    Telefono = telefono
                };
                await repositorio.GuardarAsync(estudiante);
                estudiantesPorDni[dni] = estudiante;
            }

            // 2. Registrar Inscripción INACTIVA (histórico de años anteriores)
            if (!inscripcionesExistentes.TryGetValue(estudiante.Id, out var inscripcion))
            {
                inscripcion = new Inscripcion
                {
                    EstudianteId = estudiante.Id,
                    ComisionId = comisionId,
                    Activa = false,
                    FechaBaja = fechaBaja
                };
                await repositorio.GuardarAsync(inscripcion);
                inscripcionesExistentes[estudiante.Id] = inscripcion;
            }

            // 3. Procesar las 5 clases
            for (int c = 0; c < 5; c++)
            {
                int baseIdx = 8 + (c * 4);
                DateTime fechaClase = DateTime.ParseExact(cols[baseIdx].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string temaClase = cols[baseIdx + 1].Trim();
                bool presente = bool.Parse(cols[baseIdx + 2].Trim());
                bool justificada = bool.Parse(cols[baseIdx + 3].Trim());

                if (!clasesExistentes.TryGetValue(fechaClase.Date, out var clase))
                {
                    clase = new ClaseDictada
                    {
                        ComisionId = comisionId,
                        Fecha = fechaClase.Date,
                        Numero = numeroClaseSecuencial++,
                        Tema = temaClase
                    };
                    await repositorio.GuardarAsync(clase);
                    clasesExistentes[fechaClase.Date] = clase;
                }

                await repositorio.GuardarAsync(new Asistencia
                {
                    ClaseDictadaId = clase.Id,
                    EstudianteId = estudiante.Id,
                    Presente = presente,
                    Justificada = !presente && justificada
                });
            }

            // 4. Procesar los 3 Trabajos Prácticos
            for (int t = 0; t < 3; t++)
            {
                int baseIdx = 28 + (t * 3);
                string nombreTp = cols[baseIdx].Trim();
                DateTime fechaLimite = DateTime.ParseExact(cols[baseIdx + 1].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                bool entregado = bool.Parse(cols[baseIdx + 2].Trim());

                string claveTp = $"{nombreTp}|{fechaLimite:yyyy-MM-dd}";
                if (!trabajosExistentes.TryGetValue(claveTp, out var tp))
                {
                    tp = new TrabajoPractico
                    {
                        ComisionId = comisionId,
                        Nombre = nombreTp,
                        FechaLimite = fechaLimite.Date
                    };
                    await repositorio.GuardarAsync(tp);
                    trabajosExistentes[claveTp] = tp;
                }

                await repositorio.GuardarAsync(new Entrega
                {
                    TrabajoPracticoId = tp.Id,
                    EstudianteId = estudiante.Id,
                    Entregado = entregado
                });
            }

            importados++;
        }

        return importados;
    }
}
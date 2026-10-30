using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>
/// H10: formulario para registrar un contacto con un estudiante. Lo usan el panel de seguimiento y la ficha
/// del estudiante (CA 10.1), así que la lógica está una sola vez.
/// </summary>
public class FormularioContactoViewModel : BaseViewModel
{
    private readonly SelectorComisionViewModel _selector;
    private readonly ContactoService _contactos;
    private readonly IEnlaceService _enlaces;
    private readonly TimeProvider _reloj;

    private Estudiante? _estudiante;
    private bool _visible;
    private string _titulo = string.Empty;
    private DateTime _fecha;
    private string _medioSeleccionado = Textos.De(MedioContacto.Llamada);
    private string? _resultadoSeleccionado;
    private string _nota = string.Empty;
    private string _mensajeError = string.Empty;

    public FormularioContactoViewModel(
        SelectorComisionViewModel selector, ContactoService contactos, IEnlaceService enlaces, TimeProvider reloj, IDialogService dialogos)
        : base(dialogos)
    {
        _selector = selector;
        _contactos = contactos;
        _enlaces = enlaces;
        _reloj = reloj;
        _fecha = Hoy;
        RegistrarContactoCommand = Comando(RegistrarAsync);
        CancelarContactoCommand = Comando(() => { Cerrar(); return Task.CompletedTask; });
        EnviarEmailCommand = Comando(EnviarEmailAsync);
        LlamarCommand = Comando(LlamarAsync);
    }

    private DateTime Hoy => _reloj.GetLocalNow().DateTime.Date;

    /// <summary>Lo que la pantalla que usa el formulario necesita hacer cuando el contacto quedó guardado (volver a leer sus datos).</summary>
    public Func<Task>? AlRegistrar { get; set; }

    public IReadOnlyList<string> Medios { get; } = Enum.GetValues<MedioContacto>().Select(Textos.De).ToList();

    // Opciones del formulario. Si el equipo quiere otros resultados, se cambian acá.
    public IReadOnlyList<string> Resultados { get; } =
        ["Respondió", "No respondió", "Se acordó un compromiso", "Derivado a tutoría"];

    public ICommand RegistrarContactoCommand { get; }
    public ICommand CancelarContactoCommand { get; }
    public ICommand EnviarEmailCommand { get; }
    public ICommand LlamarCommand { get; }

    public bool FormularioVisible { get => _visible; private set => SetProperty(ref _visible, value); }
    public string TituloFormulario { get => _titulo; private set => SetProperty(ref _titulo, value); }
    public DateTime FechaContacto { get => _fecha; set => SetProperty(ref _fecha, value); }
    public string MedioSeleccionado { get => _medioSeleccionado; set => SetProperty(ref _medioSeleccionado, value); }
    public string? ResultadoSeleccionado { get => _resultadoSeleccionado; set => SetProperty(ref _resultadoSeleccionado, value); }
    public string Nota { get => _nota; set => SetProperty(ref _nota, value); }
    public string MensajeError { get => _mensajeError; private set => SetProperty(ref _mensajeError, value); }

    public bool TieneEmail => !string.IsNullOrWhiteSpace(_estudiante?.Email);
    public bool TieneTelefono => !string.IsNullOrWhiteSpace(_estudiante?.Telefono);
    public string TextoEmail => TieneEmail ? $"Email: {_estudiante!.Email}" : "Sin email cargado";
    public string TextoTelefono => TieneTelefono ? $"Teléfono: {_estudiante!.Telefono}" : "Sin teléfono cargado";

    /// <summary>Abre el formulario, vacío, para registrar un contacto con ese estudiante.</summary>
    public void Abrir(Estudiante estudiante)
    {
        _estudiante = estudiante;
        TituloFormulario = $"Contacto con {estudiante.NombreCompleto}";
        FechaContacto = Hoy;
        MedioSeleccionado = Textos.De(MedioContacto.Llamada);
        ResultadoSeleccionado = null;
        Nota = string.Empty;
        MensajeError = string.Empty;
        NotificarDatosDeContacto();
        FormularioVisible = true;
    }

    public void Cerrar()
    {
        _estudiante = null;
        FormularioVisible = false;
        MensajeError = string.Empty;
        NotificarDatosDeContacto();
    }

    // CA 10.1 y CA 10.2: guarda el contacto o indica qué campos faltan, dejando el formulario abierto.
    private async Task RegistrarAsync()
    {
        var estudiante = _estudiante ?? throw new ReglaDeNegocioException("Elija un estudiante.");
        var datos = new DatosContacto(
            estudiante.Id, _selector.ExigirComision(), FechaContacto, MedioElegido(), ResultadoSeleccionado, Nota);

        try
        {
            await _contactos.RegistrarAsync(datos);
        }
        catch (ReglaDeNegocioException ex)
        {
            MensajeError = ex.Message;
            return;
        }

        Cerrar();
        if (AlRegistrar is not null) await AlRegistrar();
    }

    private MedioContacto MedioElegido() =>
        Enum.GetValues<MedioContacto>().First(m => Textos.De(m) == MedioSeleccionado);

    // Abre el programa de correo y deja elegido el medio "Email"; después se completa el resultado y se registra.
    private async Task EnviarEmailAsync()
    {
        if (!TieneEmail) return;

        MedioSeleccionado = Textos.De(MedioContacto.Email);
        var asunto = Uri.EscapeDataString("Seguimiento de la cursada");
        await AbrirEnlaceAsync($"mailto:{_estudiante!.Email}?subject={asunto}", "No se pudo abrir el programa de correo.");
    }

    // En una PC sin aplicación de llamadas avisa y deja el número a la vista.
    private async Task LlamarAsync()
    {
        if (!TieneTelefono) return;

        MedioSeleccionado = Textos.De(MedioContacto.Llamada);
        var telefono = _estudiante!.Telefono;
        var numero = new string(telefono.Where(c => char.IsDigit(c) || c == '+').ToArray());
        await AbrirEnlaceAsync($"tel:{numero}", $"Esta computadora no puede iniciar llamadas. Llame al {telefono} desde otro teléfono.");
    }

    private async Task AbrirEnlaceAsync(string direccion, string mensajeSiFalla)
    {
        MensajeError = string.Empty;
        if (!await _enlaces.AbrirAsync(direccion)) MensajeError = mensajeSiFalla;
    }

    private void NotificarDatosDeContacto()
    {
        OnPropertyChanged(nameof(TieneEmail));
        OnPropertyChanged(nameof(TieneTelefono));
        OnPropertyChanged(nameof(TextoEmail));
        OnPropertyChanged(nameof(TextoTelefono));
    }
}

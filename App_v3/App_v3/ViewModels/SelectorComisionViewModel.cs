using System.Collections.ObjectModel;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>
/// Materia y comisión con las que se está trabajando. Es una sola para toda la app:
/// se elige arriba en cada pantalla y todas las pestañas muestran datos de esa comisión.
/// </summary>
public class SelectorComisionViewModel(
    MateriaService materias,
    ComisionService comisiones,
    IDialogService dialogos) : BaseViewModel(dialogos)
{
    private Materia? _materiaSeleccionada;
    private Comision? _comisionSeleccionada;
    private bool _cargando;

    public ObservableCollection<Materia> Materias { get; } = [];
    public ObservableCollection<Comision> Comisiones { get; } = [];

    /// <summary>Se dispara cuando el usuario (o una recarga) deja elegida otra comisión.</summary>
    public event EventHandler? ComisionCambiada;

    public int? ComisionId => ComisionSeleccionada?.Id;
    public bool HayComision => ComisionSeleccionada is not null;
    public bool SinComision => !HayComision;
    public bool HayMateria => MateriaSeleccionada is not null;
    public bool SinMateria => !HayMateria;

    public string Resumen => ComisionSeleccionada is null
        ? "No hay ninguna comisión. Cree una materia y una comisión en la pestaña Materias."
        : $"{MateriaSeleccionada?.Nombre} · Comisión {ComisionSeleccionada.Nombre}";

    public Materia? MateriaSeleccionada
    {
        get => _materiaSeleccionada;
        set
        {
            if (!SetProperty(ref _materiaSeleccionada, value)) return;
            OnPropertyChanged(nameof(HayMateria));
            OnPropertyChanged(nameof(SinMateria));
            if (_cargando) return;
            _ = EjecutarAsync(CambiarMateriaAsync);
        }
    }

    public Comision? ComisionSeleccionada
    {
        get => _comisionSeleccionada;
        set
        {
            if (!SetProperty(ref _comisionSeleccionada, value) || _cargando) return;
            AvisarCambioDeComision();
        }
    }

    /// <summary>La comisión elegida o, si no hay ninguna, un aviso para el usuario.</summary>
    public int ExigirComision() =>
        ComisionId
        ?? throw new ReglaDeNegocioException(
            "Elija una comisión antes de continuar. Si no hay ninguna, cree una en la pestaña Materias.");

    /// <summary>Vuelve a leer materias y comisiones. Conserva lo elegido, salvo que se indique otra cosa.</summary>
    public async Task CargarAsync(int? materiaId = null, int? comisionId = null)
    {
        var comisionAnterior = ComisionId;
        _cargando = true;
        try
        {
            Reemplazar(Materias, await materias.ListarAsync());
            MateriaSeleccionada = Materias.FirstOrDefault(m => m.Id == (materiaId ?? MateriaSeleccionada?.Id)) ?? Materias.FirstOrDefault();
            await CargarComisionesAsync(comisionId ?? ComisionId);
        }
        finally
        {
            _cargando = false;
        }

        NotificarComision();
        if (comisionAnterior != ComisionId) ComisionCambiada?.Invoke(this, EventArgs.Empty);
    }

    private async Task CambiarMateriaAsync()
    {
        _cargando = true;
        try
        {
            await CargarComisionesAsync(null);
        }
        finally
        {
            _cargando = false;
        }

        AvisarCambioDeComision();
    }

    private async Task CargarComisionesAsync(int? comisionId)
    {
        var lista = MateriaSeleccionada is null ? [] : await comisiones.ListarAsync(MateriaSeleccionada.Id);
        Reemplazar(Comisiones, lista);
        ComisionSeleccionada = Comisiones.FirstOrDefault(c => c.Id == comisionId) ?? Comisiones.FirstOrDefault();
    }

    private void AvisarCambioDeComision()
    {
        NotificarComision();
        ComisionCambiada?.Invoke(this, EventArgs.Empty);
    }

    private void NotificarComision()
    {
        OnPropertyChanged(nameof(ComisionId));
        OnPropertyChanged(nameof(HayComision));
        OnPropertyChanged(nameof(SinComision));
        OnPropertyChanged(nameof(HayMateria));
        OnPropertyChanged(nameof(SinMateria));
        OnPropertyChanged(nameof(Resumen));
    }

    private static void Reemplazar<T>(ObservableCollection<T> coleccion, IEnumerable<T> nuevos)
    {
        coleccion.Clear();
        foreach (var item in nuevos) coleccion.Add(item);
    }
}

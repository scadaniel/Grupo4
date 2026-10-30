using System.Collections.ObjectModel;
using System.Windows.Input;
using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Una fila elegible de la lista de materias o de comisiones.</summary>
public sealed class FilaOpcionViewModel(string nombre, bool elegida, ICommand elegir)
{
    public string Texto { get; } = elegida ? $"✓ {nombre}" : nombre;
    public bool Elegida { get; } = elegida;
    public ICommand ElegirCommand { get; } = elegir;
}

/// <summary>
/// Materias y comisiones: lista de materias, comisiones de la materia elegida y dos paneles internos
/// (nueva materia / nueva comisión). Elegir una fila usa el selector global; la lógica de datos no cambia.
/// </summary>
public class MateriasViewModel : BaseViewModel, IPantalla
{
    private enum Vista { Lista, NuevaMateria, NuevaComision }

    private readonly MateriaService _materias;
    private readonly ComisionService _comisiones;
    private string _nombreMateria = string.Empty;
    private string _nombreComision = string.Empty;
    private Vista _vista = Vista.Lista;

    public MateriasViewModel(
        SelectorComisionViewModel selector, MateriaService materias, ComisionService comisiones, IDialogService dialogos)
        : base(dialogos)
    {
        Selector = selector;
        _materias = materias;
        _comisiones = comisiones;
        CrearMateriaCommand = Comando(CrearMateriaAsync);
        CrearComisionCommand = Comando(CrearComisionAsync);
        AbrirNuevaMateriaCommand = Comando(() => { IrA(Vista.NuevaMateria); return Task.CompletedTask; });
        AbrirNuevaComisionCommand = Comando(AbrirNuevaComisionAsync);
        VolverCommand = Comando(() => { Volver(); return Task.CompletedTask; });
        Selector.PropertyChanged += (_, _) => Refrescar();
        Refrescar();
    }

    public SelectorComisionViewModel Selector { get; }
    public ICommand CrearMateriaCommand { get; }
    public ICommand CrearComisionCommand { get; }
    public ICommand AbrirNuevaMateriaCommand { get; }
    public ICommand AbrirNuevaComisionCommand { get; }
    public ICommand VolverCommand { get; }

    public ObservableCollection<FilaOpcionViewModel> FilasMaterias { get; } = [];
    public ObservableCollection<FilaOpcionViewModel> FilasComisiones { get; } = [];

    public bool MostrandoLista => _vista == Vista.Lista;
    public bool MostrandoNuevaMateria => _vista == Vista.NuevaMateria;
    public bool MostrandoNuevaComision => _vista == Vista.NuevaComision;

    public bool SinMaterias => Selector.Materias.Count == 0;
    public bool HayMateria => Selector.MateriaSeleccionada is not null;
    public bool SinComisiones => HayMateria && Selector.Comisiones.Count == 0;
    public string TituloComisiones => $"Comisiones de {Selector.MateriaSeleccionada?.Nombre}";
    public string TituloNuevaComision => $"Nueva comisión en {Selector.MateriaSeleccionada?.Nombre}";

    public string NombreMateria
    {
        get => _nombreMateria;
        set => SetProperty(ref _nombreMateria, value);
    }

    public string NombreComision
    {
        get => _nombreComision;
        set => SetProperty(ref _nombreComision, value);
    }

    public Task AlMostrarseAsync()
    {
        Volver();
        return EjecutarAsync(() => Selector.CargarAsync());
    }

    private async Task CrearMateriaAsync()
    {
        var materia = await _materias.AltaAsync(NombreMateria);
        NombreMateria = string.Empty;
        await Selector.CargarAsync(materiaId: materia.Id);
        IrA(Vista.Lista);
    }

    private async Task CrearComisionAsync()
    {
        var materia = Selector.MateriaSeleccionada
            ?? throw new ReglaDeNegocioException("Primero elija o cree la materia a la que pertenece la comisión.");

        var comision = await _comisiones.AltaAsync(materia.Id, NombreComision);
        NombreComision = string.Empty;
        await Selector.CargarAsync(materia.Id, comision.Id);
        IrA(Vista.Lista);
    }

    private Task AbrirNuevaComisionAsync()
    {
        if (Selector.MateriaSeleccionada is null)
            throw new ReglaDeNegocioException("Primero elija o cree la materia a la que pertenece la comisión.");

        IrA(Vista.NuevaComision);
        return Task.CompletedTask;
    }

    private void Volver()
    {
        NombreMateria = string.Empty;
        NombreComision = string.Empty;
        IrA(Vista.Lista);
    }

    private void IrA(Vista vista)
    {
        _vista = vista;
        OnPropertyChanged(nameof(MostrandoLista));
        OnPropertyChanged(nameof(MostrandoNuevaMateria));
        OnPropertyChanged(nameof(MostrandoNuevaComision));
        Refrescar();
    }

    private void Refrescar()
    {
        FilasMaterias.Clear();
        foreach (var m in Selector.Materias)
        {
            var materia = m;
            FilasMaterias.Add(new FilaOpcionViewModel(
                materia.Nombre,
                materia.Id == Selector.MateriaSeleccionada?.Id,
                new ComandoAsync(() => { Selector.MateriaSeleccionada = materia; return Task.CompletedTask; })));
        }

        FilasComisiones.Clear();
        foreach (var c in Selector.Comisiones)
        {
            var comision = c;
            FilasComisiones.Add(new FilaOpcionViewModel(
                $"Comisión {comision.Nombre}",
                comision.Id == Selector.ComisionSeleccionada?.Id,
                new ComandoAsync(() => { Selector.ComisionSeleccionada = comision; return Task.CompletedTask; })));
        }

        OnPropertyChanged(nameof(SinMaterias));
        OnPropertyChanged(nameof(HayMateria));
        OnPropertyChanged(nameof(SinComisiones));
        OnPropertyChanged(nameof(TituloComisiones));
        OnPropertyChanged(nameof(TituloNuevaComision));
    }
}
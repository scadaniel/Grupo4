using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Alta de materias y de comisiones. Las comisiones se crean dentro de la materia elegida arriba.</summary>
public class MateriasViewModel : BaseViewModel, IPantalla
{
    private readonly MateriaService _materias;
    private readonly ComisionService _comisiones;
    private string _nombreMateria = string.Empty;
    private string _nombreComision = string.Empty;

    public MateriasViewModel(
        SelectorComisionViewModel selector, MateriaService materias, ComisionService comisiones, IDialogService dialogos)
        : base(dialogos)
    {
        Selector = selector;
        _materias = materias;
        _comisiones = comisiones;
        CrearMateriaCommand = Comando(CrearMateriaAsync);
        CrearComisionCommand = Comando(CrearComisionAsync);
    }

    public SelectorComisionViewModel Selector { get; }
    public System.Windows.Input.ICommand CrearMateriaCommand { get; }
    public System.Windows.Input.ICommand CrearComisionCommand { get; }

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

    public Task AlMostrarseAsync() => EjecutarAsync(() => Selector.CargarAsync());

    private async Task CrearMateriaAsync()
    {
        var materia = await _materias.AltaAsync(NombreMateria);
        NombreMateria = string.Empty;
        await Selector.CargarAsync(materiaId: materia.Id);
    }

    private async Task CrearComisionAsync()
    {
        var materia = Selector.MateriaSeleccionada
            ?? throw new ReglaDeNegocioException("Primero elija o cree la materia a la que pertenece la comisión.");

        var comision = await _comisiones.AltaAsync(materia.Id, NombreComision);
        NombreComision = string.Empty;
        await Selector.CargarAsync(materia.Id, comision.Id);
    }
}

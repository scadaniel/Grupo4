using App_v3.Dominio;

namespace App_v3.ViewModels;

/// <summary>Un estudiante en la grilla de asistencia: presente por defecto, se desmarca para marcarlo ausente.</summary>
public class LineaAsistenciaViewModel(LineaAsistencia linea, bool editable) : NotificadorBase
{
    private bool _presente = linea.Presente;
    private bool _justificada = linea.Justificada;
    private bool _editable = editable;

    public Estudiante Estudiante { get; } = linea.Estudiante;
    public string NombreCompleto => $"{Estudiante.Apellido}, {Estudiante.Nombre}";
    public string Legajo => $"Legajo {Estudiante.Legajo}";

    public bool Presente
    {
        get => _presente;
        set
        {
            if (!SetProperty(ref _presente, value)) return;
            if (value) Justificada = false; // solo se justifica una ausencia
            OnPropertyChanged(nameof(PuedeJustificar));
            OnPropertyChanged(nameof(TextoEstado));
            OnPropertyChanged(nameof(Ausente));
        }
    }

    public string TextoEstado => Presente ? "Presente" : "Ausente";
    public bool Ausente => !Presente;

    public bool Justificada
    {
        get => _justificada;
        set => SetProperty(ref _justificada, value);
    }

    public bool Editable
    {
        get => _editable;
        set
        {
            if (SetProperty(ref _editable, value)) OnPropertyChanged(nameof(PuedeJustificar));
        }
    }

    public bool PuedeJustificar => Editable && !Presente;

    public LineaAsistencia ALinea() => new(Estudiante, Presente, !Presente && Justificada);
}
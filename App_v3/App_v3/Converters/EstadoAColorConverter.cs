using System.Globalization;
using App_v3.ViewModels;

namespace App_v3.Converters;

/// <summary>
/// Convierte un estado (riesgo, asistencia, entrega, activo/inactivo) en su color de Colors.xaml.
/// Qué color corresponde a cada estado lo decide PaletaSemantica; el valor del color está solo en Colors.xaml.
/// </summary>
public sealed class EstadoAColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var clave = PaletaSemantica.ClaveDe(value);
        return Application.Current?.Resources.TryGetValue(clave, out var color) == true ? color : null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

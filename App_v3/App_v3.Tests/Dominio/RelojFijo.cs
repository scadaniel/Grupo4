namespace App_v3.Tests;

/// <summary>Reloj de las pruebas: siempre es la misma fecha, así los resultados no dependen del día en que se corren.</summary>
public sealed class RelojFijo(DateTime ahora) : TimeProvider
{
    private DateTime _ahora = ahora;

    public DateTime Hoy => _ahora.Date;

    public void AvanzarDias(int dias) => _ahora = _ahora.AddDays(dias);

    public override DateTimeOffset GetUtcNow() => new(DateTime.SpecifyKind(_ahora, DateTimeKind.Utc));

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}

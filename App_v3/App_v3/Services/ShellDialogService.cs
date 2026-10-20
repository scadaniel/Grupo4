namespace App_v3.Services;

public class ShellDialogService : IDialogService
{
    public Task AlertAsync(string titulo, string mensaje) =>
        Shell.Current?.DisplayAlertAsync(titulo, mensaje, "OK") ?? Task.CompletedTask;
}
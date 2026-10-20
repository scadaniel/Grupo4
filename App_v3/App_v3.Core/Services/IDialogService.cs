namespace App_v3.Services;

public interface IDialogService
{
    Task AlertAsync(string titulo, string mensaje);
}
namespace taskmanager_api.Services.Interfaces;

public interface IHealthService
{
    object GetHealthStatus();
    Task<bool> CanConnectToDatabaseAsync();
}
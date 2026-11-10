using taskmanager_api.Services.Interfaces;

namespace taskmanager_api.Services;

public class HealthService : IHealthService
{
    public object GetHealthStatus()
    {
        return new
        {
            Status = "Healthy",
            TimestampUtc = DateTime.UtcNow
        };
    }
}
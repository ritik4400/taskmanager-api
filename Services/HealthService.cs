using taskmanager_api.Services.Interfaces;
using taskmanager_api.Data;

namespace taskmanager_api.Services;

public class HealthService : IHealthService
{
    private readonly AppDbContext _db;

    public HealthService(AppDbContext db)
    {
        _db = db;
    }
    public object GetHealthStatus()
    {
        return new
        {
            Status = "Healthy",
            service = "TaskManager.Api",
            TimestampUtc = DateTime.UtcNow
        };
    }
    public Task<bool> CanConnectToDatabaseAsync()
    {
        return _db.Database.CanConnectAsync();
    }
}
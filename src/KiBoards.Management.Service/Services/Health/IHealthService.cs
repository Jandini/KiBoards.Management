namespace KiBoards.Management.Service.Services
{
    public interface IHealthService
    {
        Task<HealthInfo> GetHealthInfoAsync();
    }
}
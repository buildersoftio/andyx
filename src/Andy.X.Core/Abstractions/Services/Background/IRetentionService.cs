namespace Buildersoft.Andy.X.Core.Abstractions.Services.Background
{
    public interface IRetentionService
    {
        void StartService();
        void StopService();

        bool IsServiceRunning();
    }
}

using EdgeManager.Interfaces.Settings;

namespace EdgeManager.Interfaces.Services
{
    public interface ISettingsService
    {
        ApplicationSettings Settings { get; set; }
    }
}
using MIOS.net.Models;

namespace MIOS.net.Interfaces
{
    public interface IIniService
    {
        ConfigDto? GetDefaultIniData(string InstanceType);
    }
}
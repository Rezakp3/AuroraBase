
using Application.Common.Models;

namespace Application.Common.Interfaces.Services;

public interface IDeviceService
{
    ClientDeviceInfo GetClientInfo();
    string GetIpAddress();
}

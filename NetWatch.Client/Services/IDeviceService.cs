using System;
using System.Threading.Tasks;

namespace NetWatch.Client.Services
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Error { get; set; } = string.Empty;
    }

    public class DeviceData
    {
        public string DeviceName { get; set; } = "";
        public int Latency { get; set; }
        public string StatusColor { get; set; } = "";
    }

    public interface IDeviceService
    {
        Task ConnectAndListenAsync(IProgress<ServiceResult<DeviceData>> progress);
    }
}
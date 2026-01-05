using Grpc.Net.Client;
using LatencyMonitor;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetWatch.Client.Services
{
    public class GrpcDeviceService : IDeviceService
    {
        private readonly string _address;

        public GrpcDeviceService(string address)
        {
            _address = address;
        }

        public async Task ConnectAndListenAsync(IProgress<ServiceResult<DeviceData>> progress)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress(_address);
                var client = new NetworkMonitor.NetworkMonitorClient(channel);
                
                using var call = client.StreamLatencies(new EmptyRequest());

                while (await call.ResponseStream.MoveNext(CancellationToken.None))
                {
                    var update = call.ResponseStream.Current;

                    var data = new DeviceData
                    {
                        DeviceName = update.TargetName,
                        Latency = update.LatencyMs,
                        StatusColor = update.StatusColor
                    };

                    progress.Report(new ServiceResult<DeviceData> { Success = true, Data = data });
                }
            }
            catch (Exception ex)
            {
                progress.Report(new ServiceResult<DeviceData> { Success = false, Error = ex.Message });
            }
        }
    }
}
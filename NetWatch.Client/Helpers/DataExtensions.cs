using System.Collections.Generic;
using NetWatch.Client.Services;

namespace NetWatch.Client.Helpers
{
    public static class DataExtensions
    {
        public static string ToSecureId(this string deviceName)
        {
            return $"SEC-{deviceName.GetHashCode():X}-V1";
        }

        public static IEnumerable<DeviceData> FilterSpikes(this IEnumerable<DeviceData> history, int threshold)
        {
            foreach (var item in history)
            {
                if (item.Latency > threshold)
                {
                    yield return item;
                }
            }
        }
    }
}
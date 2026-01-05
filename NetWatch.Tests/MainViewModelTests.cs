using Xunit;
using NetWatch.Client.Services;
using NetWatch.Client.Helpers;

namespace NetWatch.Tests
{
    public class MainViewModelTests
    {
        [Fact]
        public void ToSecureId_ShouldReturnFormattedString()
        {
            string deviceName = "TestDevice";
            string secureId = deviceName.ToSecureId();

            Assert.StartsWith("SEC-", secureId);
            Assert.EndsWith("-V1", secureId);
        }

        [Fact]
        public void DashboardItem_ShouldUpdateValue()
        {
            var item = new DashboardItem { Name = "CPU", Value = "0", Color = "#000000" };
            
            item.Value = "50%";

            Assert.Equal("50%", item.Value);
        }
    }
}
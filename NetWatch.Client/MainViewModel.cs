using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.Linq;
using NetWatch.Client.Services;
using NetWatch.Client.Helpers;

namespace NetWatch.Client
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _connectionStatusColor = "#808080";
        
        private readonly IDeviceService _deviceService;
        private Dictionary<string, LineSeries<int>> _targetSeriesMap;
        
        private readonly int _historyLength;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; } 
        public Axis[] YAxes { get; set; }

        public ObservableCollection<DashboardItem> DashboardItems { get; set; }

        public string ConnectionStatusColor
        {
            get => _connectionStatusColor;
            set { _connectionStatusColor = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string address = config["ServerSettings:Address"] ?? "http://localhost:50051";
            _historyLength = int.Parse(config["GraphSettings:HistoryLength"] ?? "50");

            Series = new ISeries[] { };
            _targetSeriesMap = new Dictionary<string, LineSeries<int>>();
            DashboardItems = new ObservableCollection<DashboardItem>();

            var darkGridPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(30));

            XAxes = new Axis[]
            {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    TextSize = 10,
                    SeparatorsPaint = darkGridPaint
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    TextSize = 10,
                    SeparatorsPaint = darkGridPaint
                }
            };

            _deviceService = new GrpcDeviceService(address);
            var progress = new Progress<ServiceResult<DeviceData>>(OnDataReceived);
            
            Task.Run(async () => {
                Application.Current.Dispatcher.Invoke(() => ConnectionStatusColor = "#FFA500");
                await _deviceService.ConnectAndListenAsync(progress);
            });
        }

        private void OnDataReceived(ServiceResult<DeviceData> result)
        {
            if (!result.Success)
            {
                ConnectionStatusColor = "#FF0000"; 
                return;
            }

            ConnectionStatusColor = "#00FF00";

            var data = result.Data!;
            UpdateDashboard(data);
            UpdateChart(data);
        }

        private void UpdateDashboard(DeviceData data)
        {
            var existingItem = DashboardItems.FirstOrDefault(x => x.Name == data.DeviceName);
            if (existingItem == null)
            {
                existingItem = new DashboardItem { Name = data.DeviceName, Color = data.StatusColor };
                DashboardItems.Add(existingItem);
            }

            if (data.DeviceName.Contains("Net"))
                existingItem.Value = $"{data.Latency} KB/s";
            else
                existingItem.Value = $"{data.Latency}%";
        }

        private void UpdateChart(DeviceData data)
        {
            if (!_targetSeriesMap.TryGetValue(data.DeviceName, out var lineSeries))
            {
                lineSeries = new LineSeries<int>
                {
                    Name = data.DeviceName,
                    Values = new ObservableCollection<int>(),
                    GeometrySize = 0,               
                    LineSmoothness = 1,             
                    Stroke = new SolidColorPaint(SKColor.Parse(data.StatusColor)) { StrokeThickness = 1.5f },
                    Fill = null
                };
                
                var newSeriesList = new List<ISeries>(Series) { lineSeries };
                Series = newSeriesList.ToArray();
                OnPropertyChanged(nameof(Series));
                _targetSeriesMap[data.DeviceName] = lineSeries;
            }

            var values = (ObservableCollection<int>)lineSeries.Values!;
            values.Add(data.Latency);
            
            if (values.Count > _historyLength) 
            {
                values.RemoveAt(0);
            }

            var avg = values.Average();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
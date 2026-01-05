using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetWatch.Client.Services
{
    public class DashboardItem : INotifyPropertyChanged
    {
        private string _value = "0";
        
        public string Name { get; set; } = "";
        public string Color { get; set; } = "#FFFFFF";

        public string Value 
        { 
            get => _value;
            set 
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}